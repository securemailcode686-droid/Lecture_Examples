#!/usr/bin/env bash
# Один и тот же собранный LogHub запускается с двумя профилями конфигурации.
# Между запусками ничего не пересобирается — меняется только loghub.config.
#
# Запуск из корня репозитория:  bash LogHub/tools/run-profiles.sh
# LogHub должен быть собран в Release.
set -euo pipefail

loghub_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
dll="$loghub_root/src/LogHub/bin/Release/net10.0/LogHub.dll"
input="$loghub_root/samples/god-object-input.log"
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

stamp_before="$(stat -c %Y "$dll")"

for profile in console file; do
  mkdir -p "$work/$profile"
  cp "$loghub_root/profiles/$profile/loghub.config" "$work/$profile/"
  echo "=== профиль $profile: $(grep -v '^#' "$loghub_root/profiles/$profile/loghub.config" | tr '\n' ' ')"
  (cd "$work/$profile" && dotnet "$dll" "$input")
  for f in "$work/$profile"/logs-*.txt; do
    [[ -e "$f" ]] || continue
    echo "--- создан $(basename "$f"), строк: $(wc -l <"$f")"
    head -n 3 "$f"
  done
done

# Проверки: профили действительно дали разное поведение одной и той же сборкой.
[[ "$(stat -c %Y "$dll")" == "$stamp_before" ]] || { echo "сборка изменилась между запусками" >&2; exit 1; }
ls "$work/console"/logs-*.txt >/dev/null 2>&1 && { echo "профиль console не должен создавать файл" >&2; exit 1; }
ls "$work/file"/logs-*.txt >/dev/null 2>&1 || { echo "профиль file должен создать файл" >&2; exit 1; }
echo "=== профили дают разное поведение без перекомпиляции"
