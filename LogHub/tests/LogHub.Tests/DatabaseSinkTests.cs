using System.Linq;
using LogHub.Sinks;
using LogHub.Tests.Doubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class DatabaseSinkTests
	{
		[TestMethod]
		public void Запись_ВставкаВТаблицуLogsСДатойУровнемИТекстом()
		{
			var database = new FakeDatabase();
			var sink = new DatabaseSink(database);
			var record = Records.Of("Error", "исходное сообщение");

			sink.Write(record, "отформатированный текст");

			var call = database.Executed.Single();
			Assert.AreEqual("INSERT INTO logs(date, severity, message) VALUES(@d, @s, @m)", call.Sql);
			Assert.AreEqual(record.Date, call.Date);
			Assert.AreEqual("Error", call.Severity);
			Assert.AreEqual("отформатированный текст", call.Text, "В колонку message уходит отформатированный текст");
		}

		[TestMethod]
		public void Освобождение_ЗакрываетСоединениеОдинРаз()
		{
			var database = new FakeDatabase();
			database.Open();

			new DatabaseSink(database).Dispose();

			Assert.IsFalse(database.IsOpen);
			Assert.AreEqual(1, database.CloseCalls);
		}
	}
}
