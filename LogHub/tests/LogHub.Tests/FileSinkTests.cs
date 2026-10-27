using System;
using LogHub.Sinks;
using LogHub.Tests.Doubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class FileSinkTests
	{
		[TestMethod]
		public void КаждаяЗапись_ОтдельнаяСтрокаИСбросБуфера()
		{
			var writer = new CountingWriter();
			using var sink = new FileSink(writer);

			sink.Write(Records.Of("Info"), "первая");
			sink.Write(Records.Of("Error"), "вторая");

			Assert.AreEqual("первая" + Environment.NewLine + "вторая" + Environment.NewLine, writer.ToString());
			Assert.AreEqual(2, writer.FlushCalls);
		}

		[TestMethod]
		public void Освобождение_ЗакрываетПоток()
		{
			var writer = new CountingWriter();

			new FileSink(writer).Dispose();

			Assert.IsTrue(writer.IsDisposed);
		}

		[TestMethod]
		public void ИмяФайла_ПоДатеЗапуска()
		{
			Assert.AreEqual("logs-20260115.txt", FileSinkFactory.FileName(new DateTime(2026, 1, 15, 23, 59, 0)));
		}
	}
}
