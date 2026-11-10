using System;
using System.Reflection;
using LogHub.Configuration;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Processing;
using LogHub.Sinks;
using Microsoft.Extensions.DependencyInjection;

namespace LogHub.Composition
{
	/// <summary>
	/// Композиционный корень: какие реализации и с каким временем жизни
	/// использует приложение. Вместе с <see cref="Program.Main"/> — единственное
	/// место, где выбираются конкретные классы. Конфигурация читается до сборки
	/// контейнера, поэтому смена профиля в файле меняет регистрации без перекомпиляции.
	/// </summary>
	public static class CompositionRoot
	{
		public static void ConfigureServices(IServiceCollection services, IConfigService config, Assembly plugins)
		{
			// Порядок обращений к ключам — как у исходного модуля: сначала приёмник.
			services.AddSingleton(typeof(ILogSink), SelectSink(config, plugins));
			services.AddSingleton(typeof(ILogFormatter), SelectFormatter(config, plugins));

			// Конфигурация — один экземпляр на приложение. Единственность обеспечивает
			// регистрация, а не сам класс: ConfigService — обычный класс.
			services.AddSingleton(config);

			services.AddSingleton<ILogDatabase, ConsoleDatabaseStub>();
			services.AddSingleton<SeverityFilter>();
			services.AddTransient<LogProcessor>();
		}

		/// <exception cref="NotSupportedException">Приёмника с таким именем нет.</exception>
		public static Type SelectSink(IConfigService config, Assembly plugins)
		{
			var name = config.GetValue(ConfigKeys.Sink);
			return TypeCatalog<ILogSink>.FromAssembly(plugins).TryGet(name, out var type)
				? type
				: throw new NotSupportedException($"Приёмник '{name}' не поддерживается");
		}

		/// <summary>Неизвестный формат — только текст сообщения, как у исходного модуля.</summary>
		public static Type SelectFormatter(IConfigService config, Assembly plugins)
		{
			var name = config.GetValue(ConfigKeys.Format);
			return TypeCatalog<ILogFormatter>.FromAssembly(plugins).TryGet(name, out var type)
				? type
				: typeof(MessageOnlyFormatter);
		}
	}
}
