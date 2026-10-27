#!/usr/bin/env bash
# Сравнивает LogHub с исходным god-классом учебного репозитория.
#
# Оба запускаются на одном входе (samples/god-object-input.log) в отдельных
# каталогах с одинаковым loghub.config. Совпасть должны:
#   * стандартный вывод (включая итог «Записано: X, пропущено: Y»);
#   * код возврата (ноль / не ноль);
#   * первая строка stderr (текст необработанного исключения);
#   * созданные файлы logs-*.txt и их содержимое.
#
# Запуск из корня репозитория:  bash LogHub/tools/compare-with-god-object.sh
# LogHub должен быть собран в Release; LegacyRunner скрипт собирает сам.
set -euo pipefail

loghub_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
input="$loghub_root/samples/god-object-input.log"
loghub_dll="$loghub_root/src/LogHub/bin/Release/net10.0/LogHub.dll"
legacy_dll="$loghub_root/tools/LegacyRunner/bin/Release/net10.0/LegacyRunner.dll"

dotnet build "$loghub_root/tools/LegacyRunner/LegacyRunner.csproj" --configuration Release --nologo --verbosity quiet

if [[ ! -f "$loghub_dll" ]]; then
  echo "Нет $loghub_dll — сначала: dotnet build LogHub/LogHub.sln -c Release" >&2
  exit 1
fi

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

# Имя случая -> содержимое loghub.config. Значение «-» — файла конфигурации нет.
declare -a names=()
declare -A configs=()
add_case() { names+=("$1"); configs["$1"]="$2"; }

add_case "нет-конфигурации" "-"
add_case "консоль-debug-json" $'sink=console\nminSeverity=Debug\nformat=json'
add_case "файл-warning-csv" $'sink=file\nminSeverity=Warning\nformat=csv'
add_case "бд-со-строкой-подключения" $'# приёмник — база\nsink = database\nconnection = Server=db;Database=logs\nminSeverity = Info\nformat = plain'
add_case "бд-по-умолчанию-неизвестный-формат" $'sink=database\nminSeverity=Error\nformat=xml'
add_case "комментарии-и-переопределение" $'\n   \n# комментарий\nsink=file\nsink=console\nстрока без равенства\nminSeverity=Critical\nformat=plain'
add_case "неизвестный-порог" $'sink=console\nminSeverity=Verbose\nformat=plain'
add_case "неизвестный-приёмник" $'sink=telegram\nminSeverity=Info\nformat=plain'
add_case "нет-ключа-sink" $'minSeverity=Info\nformat=plain'

run_one() { # каталог, dll
  local dir="$1" dll="$2" code=0
  (cd "$dir" && dotnet "$dll" "$input" >stdout.txt 2>stderr.txt) || code=$?
  if [[ $code -eq 0 ]]; then echo "ноль" >"$dir/exit.txt"; else echo "не ноль" >"$dir/exit.txt"; fi
  head -n 1 "$dir/stderr.txt" >"$dir/stderr-first.txt"
}

failed=0
for name in "${names[@]}"; do
  for side in legacy loghub; do
    mkdir -p "$work/$name/$side"
    if [[ "${configs[$name]}" != "-" ]]; then
      printf '%s\n' "${configs[$name]}" >"$work/$name/$side/loghub.config"
    fi
  done

  run_one "$work/$name/legacy" "$legacy_dll"
  run_one "$work/$name/loghub" "$loghub_dll"

  if diff -r -u -x stderr.txt "$work/$name/legacy" "$work/$name/loghub" >"$work/$name.diff"; then
    written="$(grep -h '^Записано' "$work/$name/loghub/stdout.txt" || echo "исключение: $(cat "$work/$name/loghub/stderr-first.txt")")"
    files="$(cd "$work/$name/loghub" && ls logs-*.txt 2>/dev/null | tr '\n' ' ' || true)"
    echo "совпадает  $name  ${written}${files:+  + $files}"
  else
    echo "РАСХОДИТСЯ $name"
    cat "$work/$name.diff"
    failed=1
  fi
done

exit $failed
