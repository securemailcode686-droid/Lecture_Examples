using System;
using LogHub.Model;
using LogHub.Parsing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class LogLineParserTests
	{
		[TestMethod]
		public void КорректнаяСтрока_РазбираетсяНаДатуУровеньИСообщение()
		{
			var record = ParseValid("2026-01-15T10:00:02|Error|Не удалось открыть файл");

			Assert.AreEqual(new DateTime(2026, 1, 15, 10, 0, 2), record.Date);
			Assert.AreEqual("Error", record.Severity);
			Assert.AreEqual("Не удалось открыть файл", record.Message);
		}

		[TestMethod]
		public void ПробелыВокругУровняИСообщения_Обрезаются()
		{
			var record = ParseValid("2026-01-15T10:00:00|  Warning |  диск заполнен на 90%  ");

			Assert.AreEqual("Warning", record.Severity);
			Assert.AreEqual("диск заполнен на 90%", record.Message);
		}

		[TestMethod]
		public void ВертикальныеЧертыПослеВторой_ОстаютсяВСообщении()
		{
			var record = ParseValid("2026-01-15T10:00:00|Info|a|b|c");

			Assert.AreEqual("a|b|c", record.Message);
		}

		[TestMethod]
		[DataRow("строка без разделителей — будет пропущена", DisplayName = "нет разделителей")]
		[DataRow("2026-01-15T10:00:00|Info", DisplayName = "два поля")]
		[DataRow("вчера|Info|сообщение", DisplayName = "не дата")]
		[DataRow("", DisplayName = "пустая строка")]
		public void НеразборчиваяСтрока_НеРазбирается(string line)
		{
			var parsed = LogLineParser.TryParse(line, out var record);

			Assert.IsFalse(parsed);
			Assert.IsNull(record);
		}

		private static LogRecord ParseValid(string line)
		{
			var parsed = LogLineParser.TryParse(line, out var record);
			Assert.IsTrue(parsed, $"Строка должна разбираться: {line}");
			return record!;
		}
	}
}
