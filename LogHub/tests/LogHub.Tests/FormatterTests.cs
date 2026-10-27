using System;
using System.Linq;
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
		public void НеизвестныйФормат_ВыводитТолькоСообщение()
		{
			var registry = FormatterRegistry.FromAssembly(typeof(Program).Assembly, new MessageOnlyFormatter());

			Assert.AreEqual("ответ \"502\" от upstream", registry.Resolve("xml").Format(Record));
		}

		[TestMethod]
		public void СборкаLogHub_СодержитВсеФорматыИсходногоМодуля()
		{
			var registry = FormatterRegistry.FromAssembly(typeof(Program).Assembly, new MessageOnlyFormatter());

			CollectionAssert.IsSubsetOf(new[] { "plain", "json", "csv" }, registry.Names.ToArray());
			Assert.IsInstanceOfType(registry.Resolve("json"), typeof(JsonFormatter));
		}
	}
}
