using System.Collections.Generic;
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
			var config = ConfigParser.Parse(new[]
			{
				"# приёмник",
				"",
				"   ",
				"просто текст",
				"sink = file"
			});

			Assert.AreEqual("file", config["sink"]);
			Assert.AreEqual("нет", config.GetValueOrDefault("просто текст", "нет"));
		}

		[TestMethod]
		public void ЗначениеСоЗнакомРавенства_СохраняетсяЦеликом()
		{
			var config = ConfigParser.Parse(new[] { "connection = Server=db;User=app" });

			Assert.AreEqual("Server=db;User=app", config["connection"]);
		}

		[TestMethod]
		public void ПовторныйКлюч_ПереопределяетПредыдущий()
		{
			var config = ConfigParser.Parse(new[] { "sink=file", "sink=console" });

			Assert.AreEqual("console", config["sink"]);
		}

		[TestMethod]
		public void ОтсутствующийОбязательныйКлюч_БросаетKeyNotFound()
		{
			var config = ConfigParser.Parse(new[] { "sink=console" });

			Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = config["format"]; });
		}

		[TestMethod]
		public void ЗначенияПоУмолчанию_КонсольInfoPlain()
		{
			var config = LogHubConfig.Defaults;

			Assert.AreEqual("console", config[LogHubConfig.SinkKey]);
			Assert.AreEqual("Info", config[LogHubConfig.MinSeverityKey]);
			Assert.AreEqual("plain", config[LogHubConfig.FormatKey]);
		}
	}
}
