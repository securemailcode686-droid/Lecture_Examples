using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using LogHub.Model;
using LogHub.Importing;
using LogHub.Composition;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Processing;
using LogHub.Tests.Doubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
    [TestClass]
    public sealed class LogImporterTests
    {
        [TestMethod]
        public void Nginx_РазбираетКодОтвета()
        {
            var importer = new NginxLogImporter();

            var record = importer.Import(LogInput.FromLines(
                new[]
                {
                    "2026/11/24 10:00:00 [error] request failed, status: 502"
                })).Single();

            Assert.AreEqual<int?>(502, record?.StatusCode);
            Assert.AreEqual("Error", record?.Severity);
            Assert.AreEqual(
                new DateTime(2026, 11, 24, 10, 0, 0),
                record?.Date);
        }

        [TestMethod]
        public void Nginx_ЗаписьБезКодаДопустима()
        {
            var record = new NginxLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "2026/11/24 10:00:00 [error] upstream timed out"
                })).Single();

            Assert.IsNotNull(record);
            Assert.IsNull(record.StatusCode);
        }

        [TestMethod]
        public void Nginx_ХукОтбрасываетНедопустимыйКод()
        {
            var record = new NginxLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "2026/11/24 10:00:00 [error] failed, status: 999"
                })).Single();

            Assert.IsNull(record);
        }

        [TestMethod]
        public void Nginx_НекорректнаяСтрока()
        {
            var record = new NginxLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "invalid log line"
                })).Single();

            Assert.IsNull(record);
        }

        [TestMethod]
        public void Csv_РазбираетСообщениеСКавычками()
        {
            var record = new CsvLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "2026-11-24T10:00:00,Error,\"request failed, timeout\""
                })).Single();

            Assert.AreEqual(
                "request failed, timeout", record?.Message);
            Assert.AreEqual("Error", record?.Severity);
        }

        [TestMethod]
        public void Csv_НекорректнаяСтрока()
        {
            var record = new CsvLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "invalid,date"
                })).Single();

            Assert.IsNull(record);
        }

        [TestMethod]
        public void InMemory_ЧитаетБезДиска()
        {
            var records = new InMemoryLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "2026-11-24T10:00:00|Info|First",
                    "2026-11-24T10:00:01|Error|Second"
                })).ToArray();

            CollectionAssert.AreEqual(
                new[] { "First", "Second" },
                records.Select(r => r?.Message).ToArray());
        }

        [TestMethod]
        public void Text_ХукПоУмолчаниюНеОтбрасываетЗапись()
        {
            var record = new TextLogImporter()
                .Import(LogInput.FromLines(new[]
                {
                    "2026-11-24T10:00:00|Info|Test"
                })).Single();

            Assert.IsNotNull(record);
            Assert.AreEqual("Test", record.Message);
        }

        [TestMethod]
        public void Стратегия_ПодменаМеняетРезультат()
        {
            var lines = new[]
            {
                "2026-11-24T10:00:00,Error,Test"
            };

            var filter = new SeverityFilter(
                Configs.With(("minSeverity", "Info")));

            var csvSink = new SpySink();
            var textSink = new SpySink();

            var csv = new LogProcessor(
                csvSink, filter,
                new PlainFormatter(),
                new CsvLogImporter());

            var text = new LogProcessor(
                textSink, filter,
                new PlainFormatter(),
                new TextLogImporter());

            Assert.AreEqual(
                new ProcessingStats(1, 0),
                csv.Process(lines));

            Assert.AreEqual(
                new ProcessingStats(0, 1),
                text.Process(lines));
        }

        [TestMethod]
        public void ШаблонныйМетод_ПравильныйПорядокШагов()
        {
            var importer = new TrackingImporter();

            _ = importer.Import(
                LogInput.FromLines(Array.Empty<string>()))
                .ToArray();

            CollectionAssert.AreEqual(
                new[]
                {
                    "Open", "Read", "Parse",
                    "Validate", "Read", "Dispose"
                },
                importer.Calls.ToArray());
        }

        [TestMethod]
        public void ШаблонныйМетод_ОсвобождаетРесурсы()
        {
            var importer = new TrackingImporter();

            foreach (var record in importer.Import(
                LogInput.FromLines(Array.Empty<string>())))
            {
                break;
            }

            Assert.AreEqual(
                "Dispose", importer.Calls.Last());
        }

        [TestMethod]
        public void Конфигурация_ВыбираетИмпортёр()
        {
            var config = Configs.With(
                ("importer", "nginx"));

            var type = CompositionRoot.SelectImporter(
                config, typeof(NginxLogImporter).Assembly);

            Assert.AreEqual(
                typeof(NginxLogImporter), type);
        }

        private sealed class TrackingImporter
            : LogImporterBase
        {
            public List<string> Calls { get; } = new();

            protected override TextReader Open(LogInput input)
            {
                Calls.Add("Open");

                return new TrackingReader(
                    "test",
                    () => Calls.Add("Dispose"));
            }

            protected override string? Read(TextReader reader)
            {
                Calls.Add("Read");
                return reader.ReadLine();
            }

            protected override LogRecord? Parse(string line)
            {
                Calls.Add("Parse");

                return new LogRecord(
                    new DateTime(2026, 11, 24),
                    "Info", line);
            }

            protected override bool Validate(LogRecord record)
            {
                Calls.Add("Validate");
                return true;
            }
        }

        private sealed class TrackingReader : StringReader
        {
            private readonly Action _onDispose;

            public TrackingReader(
                string value, Action onDispose)
                : base(value)
            {
                _onDispose = onDispose;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    _onDispose();

                base.Dispose(disposing);
            }
        }
    }
}
