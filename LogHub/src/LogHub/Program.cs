using System;
using System.IO;
using LogHub.Composition;
using LogHub.Configuration;
using LogHub.Processing;
using Microsoft.Extensions.DependencyInjection;

namespace LogHub
{
	/// <summary>
	/// Точка входа и вершина композиционного корня.
	///
	///     dotnet run --project src/LogHub -- путь/к/файлу.log
	///
	/// Конфигурация читается из loghub.config в текущем каталоге.
	/// </summary>
	public static class Program
	{
		public const string Usage = "Использование: LogHub <путь к файлу лога>";

		public static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine(Usage);
				return 2;
			}

			var config = new ConfigService();
			ConfigFile.LoadInto(config, ConfigFile.DefaultPath);

			var services = new ServiceCollection();
			CompositionRoot.ConfigureServices(services, config, typeof(Program).Assembly);

			ProcessingStats stats;
			using (var provider = services.BuildServiceProvider(new ServiceProviderOptions
			       {
				       ValidateScopes = true,
				       ValidateOnBuild = true
			       }))
			{
				// Единственное обращение к контейнеру во всём приложении.
				var processor = provider.GetRequiredService<LogProcessor>();
				stats = processor.Process(File.ReadLines(args[0]));
			}

			// Итог печатается после освобождения контейнера: приёмники уже закрыты,
			// как у исходного модуля.
			Console.WriteLine($"Записано: {stats.Written}, пропущено: {stats.Skipped}");
			return 0;
		}
	}
}
