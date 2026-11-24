using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogHub.Composition;
using LogHub.Configuration;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Processing;
using LogHub.Sinks;
using LogHub.Tests.Doubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	/// <summary>
	/// Композиционный корень и маршрутизация: какой приёмник и формат
	/// получит приложение при данном профиле конфигурации.
	/// Приёмник «file» только регистрируется, но не создаётся — файл не открывается.
	/// </summary>
	[TestClass]
	public sealed class CompositionRootTests
	{
		private static readonly Assembly LogHubAssembly = typeof(Program).Assembly;

		[TestMethod]
		[DataRow("console", typeof(ConsoleSink))]
		[DataRow("file", typeof(FileSink))]
		[DataRow("database", typeof(DatabaseSink))]
		public void Профиль_ВыбираетПриёмникПоИмени(string sink, Type expected)
		{
			Assert.AreEqual(expected, CompositionRoot.SelectSink(Configs.Profile(sink), LogHubAssembly));
		}

		[TestMethod]
		public void ПрофильFile_ВКонтейнереРегистрируетсяFileSink()
		{
			var services = new ServiceCollection();

			CompositionRoot.ConfigureServices(services, Configs.Profile("file"), LogHubAssembly);

			var sink = services.Single(d => d.ServiceType == typeof(ILogSink));
			Assert.AreEqual(typeof(FileSink), sink.ImplementationType);
			Assert.AreEqual(ServiceLifetime.Singleton, sink.Lifetime);
		}

		[TestMethod]
		public void НеизвестныйПриёмник_NotSupportedСТемЖеТекстомЧтоИсходныйМодуль()
		{
			var error = Assert.ThrowsExactly<NotSupportedException>(
				() => { CompositionRoot.SelectSink(Configs.Profile("telegram"), LogHubAssembly); });

			Assert.AreEqual("Приёмник 'telegram' не поддерживается", error.Message);
		}

		[TestMethod]
		public void НеизвестныйФормат_ТолькоСообщение()
		{
			Assert.AreEqual(
				typeof(MessageOnlyFormatter),
				CompositionRoot.SelectFormatter(Configs.Profile("console", format: "xml"), LogHubAssembly));
		}

		[TestMethod]
		public void ПрофильConsole_ГрафСобираетсяСПроверкойИВременамиЖизни()
		{
			var services = new ServiceCollection();
			CompositionRoot.ConfigureServices(services, Configs.Profile("console", format: "json"), LogHubAssembly);
			using var provider = services.BuildServiceProvider(new ServiceProviderOptions
			{
				ValidateScopes = true,
				ValidateOnBuild = true
			});

			Assert.IsInstanceOfType(provider.GetRequiredService<ILogSink>(), typeof(ConsoleSink));
			Assert.IsInstanceOfType(provider.GetRequiredService<ILogFormatter>(), typeof(JsonFormatter));
			Assert.AreSame(provider.GetRequiredService<ILogSink>(), provider.GetRequiredService<ILogSink>());
			Assert.AreNotSame(provider.GetRequiredService<LogProcessor>(), provider.GetRequiredService<LogProcessor>());
		}

		[TestMethod]
		public void СвояРеализацияIConfigService_ПодставляетсяБезСбросаСтатики()
		{
			var config = new FixedConfig(new Dictionary<string, string>
			{
				[ConfigKeys.Sink] = "console",
				[ConfigKeys.Format] = "plain",
				[ConfigKeys.MinSeverity] = "Error"
			});
			var services = new ServiceCollection();
			CompositionRoot.ConfigureServices(services, config, LogHubAssembly);
			using var provider = services.BuildServiceProvider();

			var filter = provider.GetRequiredService<SeverityFilter>();

			Assert.IsFalse(filter.Passes(Records.Of("Warning")));
			Assert.IsTrue(filter.Passes(Records.Of("Error")));
			Assert.AreSame(config, provider.GetRequiredService<IConfigService>());
		}

		[TestMethod]
		public void КлючиЧитаютсяВПорядкеИсходногоМодуля_СначалаПриёмник()
		{
			var config = new FixedConfig(new Dictionary<string, string>
			{
				[ConfigKeys.Sink] = "console",
				[ConfigKeys.Format] = "plain",
				[ConfigKeys.MinSeverity] = "Info"
			});

			CompositionRoot.ConfigureServices(new ServiceCollection(), config, LogHubAssembly);

			Assert.AreEqual(ConfigKeys.Sink, config.Requested.First());
		}
	}
}
