using LogHub.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class ConfigParserTests
	{
		[TestMethod]
		public void КомментарииПустыеИСтрокиБезРавенства_Пропускаются()
		{
			var values = ConfigParser.Parse(new[]
			{
				"# приёмник",
				"",
				"   ",
				"просто текст",
				"sink = file"
			});

			Assert.AreEqual(1, values.Count);
			Assert.AreEqual("file", values["sink"]);
		}

		[TestMethod]
		public void ЗначениеСоЗнакомРавенства_СохраняетсяЦеликом()
		{
			var values = ConfigParser.Parse(new[] { "connection = Server=db;User=app" });

			Assert.AreEqual("Server=db;User=app", values["connection"]);
		}

		[TestMethod]
		public void ПовторныйКлюч_ПереопределяетПредыдущий()
		{
			var values = ConfigParser.Parse(new[] { "sink=file", "sink=console" });

			Assert.AreEqual("console", values["sink"]);
		}

		[TestMethod]
		public void ЗначенияПоУмолчанию_КонсольInfoPlain()
		{
			Assert.AreEqual("console", ConfigKeys.Defaults[ConfigKeys.Sink]);
			Assert.AreEqual("Info", ConfigKeys.Defaults[ConfigKeys.MinSeverity]);
			Assert.AreEqual("plain", ConfigKeys.Defaults[ConfigKeys.Format]);
		}
	}
}
