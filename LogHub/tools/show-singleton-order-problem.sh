#!/usr/bin/env bash
# Воспроизводит проблему классического синглтона: тест
# ClassicConfigServiceTests.НовыйСервис_ПорогНеЗадан проходит в одиночку
# и падает в наборе. В обычной сборке он отключён [Ignore], здесь сборка
# идёт с -p:ShowOrderProblem=true, и атрибут снимается.
#
# Запуск из корня репозитория:  bash LogHub/tools/show-singleton-order-problem.sh
# Собирает в Debug, чтобы не трогать Release-сборку остальных шагов CI.
set -uo pipefail

project="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/tests/LogHub.Tests/LogHub.Tests.csproj"
alone="FullyQualifiedName~ClassicConfigServiceTests.НовыйСервис_ПорогНеЗадан"
suite="FullyQualifiedName~ClassicConfigServiceTests"

dotnet build "$project" --configuration Debug -p:ShowOrderProblem=true --nologo --verbosity quiet || exit 1

echo
echo "=== 1. Тест в одиночку"
dotnet test "$project" --configuration Debug --no-build --nologo --filter "$alone"
alone_code=$?

echo
echo "=== 2. Тот же тест в наборе ClassicConfigServiceTests"
dotnet test "$project" --configuration Debug --no-build --nologo --filter "$suite"
suite_code=$?

echo
if [[ $alone_code -eq 0 && $suite_code -ne 0 ]]; then
  echo "Воспроизведено: в одиночку тест проходит, в наборе падает."
  echo "Состояние синглтона перенеслось из Порог_ЗаписываетсяИЧитается."
  exit 0
fi

echo "Не воспроизведено: в одиночку код $alone_code, в наборе код $suite_code." >&2
exit 1
