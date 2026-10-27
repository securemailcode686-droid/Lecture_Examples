using System.Linq;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Processing;
using LogHub.Tests.Doubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class LogProcessorTests
	{
		/// <summary>Те же строки, что пишет демо <c>god-object</c> учебного репозитория.</summary>
		private static readonly string[] DemoLines =
		{
			"2026-01-15T10:00:00|Debug|Запуск приложения",
			"2026-01-15T10:00:01|Info|Конфигурация загружена",
			"строка без разделителей — будет пропущена",
			"2026-01-15T10:00:02|Error|Не удалось открыть файл",
			"2026-01-15T10:00:03|Critical|Нет связи с базой данных"
		};

		[TestMethod]
		public void ДемоВход_ТриЗаписаныДвеПропущены_КакУИсходногоМодуля()
		{
			var sink = new SpySink();
			var processor = new LogProcessor(new SeverityFilter("Info"), new PlainFormatter(), sink);

			var stats = processor.Process(DemoLines);

			Assert.AreEqual(new ProcessingStats(Written: 3, Skipped: 2), stats);
			CollectionAssert.AreEqual(
				new[]
				{
					"2026-01-15 10:00:01 [Info] Конфигурация загружена",
					"2026-01-15 10:00:02 [Error] Не удалось открыть файл",
					"2026-01-15 10:00:03 [Critical] Нет связи с базой данных"
				},
				sink.Written.Select(w => w.Text).ToArray());
		}

		[TestMethod]
		public void НеразобранныеИОтфильтрованные_СчитаютсяВместеИВПриёмникНеПопадают()
		{
			var sink = new SpySink();
			var processor = new LogProcessor(new SeverityFilter("Critical"), new PlainFormatter(), sink);

			var stats = processor.Process(DemoLines);

			Assert.AreEqual(new ProcessingStats(Written: 1, Skipped: 4), stats);
			Assert.AreEqual("Critical", sink.Written.Single().Record.Severity);
		}

		[TestMethod]
		public void Приёмник_ПолучаетИсходнуюЗаписьИТекстВыбранногоФормата()
		{
			var sink = new SpySink();
			var processor = new LogProcessor(new SeverityFilter("Debug"), new CsvFormatter(), sink);

			processor.Process(new[] { "2026-01-15T10:00:00|Debug|Запуск приложения" });

			var (record, text) = sink.Written.Single();
			Assert.AreEqual("Debug", record.Severity);
			Assert.AreEqual("Запуск приложения", record.Message);
			Assert.AreEqual("2026-01-15T10:00:00.0000000;Debug;Запуск приложения", text);
		}

		[TestMethod]
		public void ПустойВход_НичегоНеЗаписано()
		{
			var sink = new SpySink();
			var processor = new LogProcessor(new SeverityFilter("Info"), new PlainFormatter(), sink);

			var stats = processor.Process(System.Array.Empty<string>());

			Assert.AreEqual(new ProcessingStats(0, 0), stats);
			Assert.AreEqual(0, sink.Written.Count);
		}
	}
}
