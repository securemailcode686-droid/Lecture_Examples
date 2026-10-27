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

## Как добавить приёмник

Два новых файла в `src/LogHub/Sinks/`, существующие не меняются:

* класс приёмника, реализующий `ILogSink` (и `IDisposable`, если держит ресурсы);
* открытая фабрика с конструктором без параметров, реализующая `ISinkFactory`;
  её `Name` — значение ключа `sink`.

`SinkRegistry` находит фабрики в сборке сам.

## Отчёты

| Работа | Отчёт |
|---|---|
| ЛР 1. Git, среда и SOLID | [`docs/ЛР1.md`](../docs/ЛР1.md) |
