# LogHub

[![build](https://github.com/securemailcode686-droid/Lecture_Examples/actions/workflows/build.yml/badge.svg?branch=develop)](https://github.com/securemailcode686-droid/Lecture_Examples/actions/workflows/build.yml)

Сквозной проект практикума «Технологии и методы программирования», раздел 2
«Паттерны программирования». Вариант 12: логи Nginx (`error.log`),
приёмники SQLite и файл, разбор кода ответа.

## Сборка и тесты

```bash
cd LogHub
dotnet build
dotnet test
```

## Запуск

```bash
cd LogHub
dotnet run --project src/LogHub -- samples/god-object-input.log
```

Настройки читаются из `loghub.config` в текущем каталоге (если файла нет —
консоль, уровень от Info, формат plain):

```ini
# console | file | database
sink = file
# Debug | Info | Warning | Error | Critical
minSeverity = Warning
# plain | json | csv | message
format = csv
# только для sink = database
connection = Server=db;Database=logs
```

## Сравнение с исходным god-object

```bash
dotnet build LogHub/LogHub.sln -c Release
bash LogHub/tools/compare-with-god-object.sh
```

Скрипт запускает LogHub и `LogProcessingModule` учебного репозитория
(через `tools/LegacyRunner`, который ссылается на `DesignPatterns/Implementation`)
на одном входе с девятью разными `loghub.config` и сравнивает вывод,
код возврата и созданные файлы. Выполняется в CI.

## Профили конфигурации

Готовые профили лежат в `profiles/`. Переключение — без перекомпиляции:
скопировать нужный файл в `loghub.config` рабочего каталога и запустить заново.

```bash
cd LogHub
cp profiles/file/loghub.config .     # или profiles/console/loghub.config
dotnet run --project src/LogHub -- samples/god-object-input.log
```

Оба профиля прогоняются в CI одной и той же сборкой (`tools/run-profiles.sh`).

## Как добавить приёмник

Один новый файл в `src/LogHub/Sinks/`, существующие не меняются: открытый
класс, реализующий `ILogSink` (и `IDisposable`, если держит ресурсы),
с атрибутом `[ConfigName("имя")]`. Имя — значение ключа `sink`.
Зависимости приходят через конструктор, класс создаёт контейнер.
Композиционный корень находит приёмник в сборке сам.

## Отчёты

| Работа | Отчёт |
|---|---|
| ЛР 1 (4). Git, среда и SOLID | [`docs/ЛР1.md`](../docs/ЛР1.md) |
| ЛР 2 (5). IoC, DI-контейнер и Синглтон | [`docs/ЛР2.md`](../docs/ЛР2.md) |
