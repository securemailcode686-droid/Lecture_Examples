using System.Collections.Generic;
using System.Threading.Tasks;
using LogHub.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	/// <summary>
	/// Обычный класс, время жизни задаёт контейнер. Каждый тест создаёт свой
	/// экземпляр, поэтому тесты независимы и идут параллельно.
	/// </summary>
	[TestClass]
	public sealed class ConfigServiceTests
	{
		/// <summary>
		/// Та же проверка, что у отключённого
		/// <see cref="ClassicConfigServiceTests.НовыйСервис_ПорогНеЗадан"/>.
		/// Здесь она проходит в любом порядке: экземпляр свой.
		/// </summary>
		[TestMethod]
		public void НовыйСервис_ПорогНеЗадан()
		{
			var config = new ConfigService();

			Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = config.GetValue(ConfigKeys.MinSeverity); });
		}

		[TestMethod]
		public void SetValue_ЗатемGetValue_ВозвращаетЗаписанное()
		{
			var config = new ConfigService();

			config.SetValue(ConfigKeys.MinSeverity, "Warning");

			Assert.AreEqual("Warning", config.GetValue(ConfigKeys.MinSeverity));
		}

		[TestMethod]
		public void ТекстОшибкиОтсутствующегоКлюча_КакУСловаря()
		{
			// От этого текста зависит совпадение вывода с исходным god-object.
			var expected = Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = new Dictionary<string, string>()["sink"]; });

			var actual = Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = new ConfigService().GetValue("sink"); });

			Assert.AreEqual(expected.Message, actual.Message);
		}

		[TestMethod]
		public void AddSingleton_ВОдномКонтейнере_ОдинЭкземпляр()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IConfigService, ConfigService>();
			using var provider = services.BuildServiceProvider();

			Assert.AreSame(provider.GetRequiredService<IConfigService>(), provider.GetRequiredService<IConfigService>());
		}

		[TestMethod]
		public void AddSingleton_РазныеКонтейнеры_НеВидятЗаписейДругДруга()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IConfigService, ConfigService>();
			using var first = services.BuildServiceProvider();
			using var second = services.BuildServiceProvider();

			first.GetRequiredService<IConfigService>().SetValue(ConfigKeys.Sink, "file");

			Assert.AreNotSame(first.GetRequiredService<IConfigService>(), second.GetRequiredService<IConfigService>());
			Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = second.GetRequiredService<IConfigService>().GetValue(ConfigKeys.Sink); });
		}

		[TestMethod]
		public void ЗаписиИзМногихПотоков_НеТеряются()
		{
			var config = new ConfigService();

			Parallel.For(0, 1000, i => config.SetValue($"key{i}", i.ToString(System.Globalization.CultureInfo.InvariantCulture)));

			for (var i = 0; i < 1000; i++)
			{
				Assert.AreEqual(i.ToString(System.Globalization.CultureInfo.InvariantCulture), config.GetValue($"key{i}"));
			}
		}
	}
}
