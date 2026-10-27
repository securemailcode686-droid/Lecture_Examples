using System;
using System.Collections.Generic;
using System.Linq;
using LogHub.Configuration;
using LogHub.Sinks;
using LogHub.Tests.Doubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	/// <summary>Маршрутизация: какой приёмник получит записи при данном значении <c>sink</c>.</summary>
	[TestClass]
	public sealed class SinkRegistryTests
	{
		private static readonly LogHubConfig Config =
			new LogHubConfig(new Dictionary<string, string> { ["sink"] = "b", ["token"] = "42" });

		[TestMethod]
		public void ИзвестноеИмя_ПриёмникСоздаётСвоейФабрикойСТекущейКонфигурацией()
		{
			var a = new FakeSinkFactory("a");
			var b = new FakeSinkFactory("b");
			var registry = new SinkRegistry(new[] { a, b });

			var sink = registry.Create("b", Config);

			Assert.AreSame(b.Created, sink);
			Assert.AreSame(Config, b.ReceivedConfig);
			Assert.IsNull(a.ReceivedConfig, "Чужая фабрика вызываться не должна");
		}

		[TestMethod]
		public void НеизвестноеИмя_БросаетNotSupportedСТемЖеТекстомЧтоИсходныйМодуль()
		{
			var registry = new SinkRegistry(new[] { new FakeSinkFactory("console") });

			var error = Assert.ThrowsExactly<NotSupportedException>(() => { registry.Create("telegram", Config); });

			Assert.AreEqual("Приёмник 'telegram' не поддерживается", error.Message);
		}

		[TestMethod]
		public void ИменаПриёмников_СравниваютсяСУчётомРегистра()
		{
			var registry = new SinkRegistry(new[] { new FakeSinkFactory("file") });

			Assert.ThrowsExactly<NotSupportedException>(() => { registry.Create("File", Config); });
		}

		[TestMethod]
		public void СборкаLogHub_СодержитПриёмникиИсходногоМодуля()
		{
			// Фабрики только находятся, но не вызываются: ни файл, ни «база» не открываются.
			var registry = SinkRegistry.FromAssembly(typeof(Program).Assembly);

			CollectionAssert.AreEquivalent(
				new[] { "console", "database", "file" },
				registry.Names.ToArray());
		}
	}
}
