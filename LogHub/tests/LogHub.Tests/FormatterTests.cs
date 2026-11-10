using System;
using System.Linq;
using LogHub.Composition;
using LogHub.Formatting;
using LogHub.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class FormatterTests
	{
		private static readonly LogRecord Record =
			new LogRecord(new DateTime(2026, 1, 15, 10, 0, 1), "Warning", "ответ \"502\" от upstream");

		[TestMethod]
		public void Plain_ДатаУровеньВСкобкахСообщение()
		{
			Assert.AreEqual(
				"2026-01-15 10:00:01 [Warning] ответ \"502\" от upstream",
				new PlainFormatter().Format(Record));
		}

		[TestMethod]
		public void Json_УровеньСтрочнымиКавычкиЭкранированы()
		{
			Assert.AreEqual(
				"{\"date\":\"2026-01-15T10:00:01.0000000\",\"level\":\"warning\",\"message\":\"ответ \\\"502\\\" от upstream\"}",
				new JsonFormatter().Format(Record));
		}

		[TestMethod]
		public void Csv_ПоляЧерезТочкуСЗапятой()
		{
			Assert.AreEqual(
				"2026-01-15T10:00:01.0000000;Warning;ответ \"502\" от upstream",
				new CsvFormatter().Format(Record));
		}

		[TestMethod]
		public void СборкаLogHub_СодержитВсеФорматыИсходногоМодуля()
		{
			var catalog = TypeCatalog<ILogFormatter>.FromAssembly(typeof(Program).Assembly);

			CollectionAssert.AreEquivalent(new[] { "plain", "json", "csv", "message" }, catalog.Names.ToArray());
		}
	}
}
