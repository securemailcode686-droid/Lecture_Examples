using System;
using System.IO;
using LogHub.Configuration;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Processing;
using LogHub.Sinks;

namespace LogHub
{
	/// <summary>
	/// Композиционный корень: единственное место, где выбираются конкретные
	/// классы и где программа касается диска и консоли напрямую.
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

			var assembly = typeof(Program).Assembly;
			var sinks = SinkRegistry.FromAssembly(assembly);
			var formatters = FormatterRegistry.FromAssembly(assembly, fallback: new MessageOnlyFormatter());

			// Порядок шагов — как у исходного модуля: конфигурация, приёмник,
			// порог, затем чтение лога. От него зависит вывод при ошибках.
			var config = ConfigFile.Load(ConfigFile.DefaultPath);
			var sink = sinks.Create(config[LogHubConfig.SinkKey], config);

			ProcessingStats stats;
			try
			{
				var filter = new SeverityFilter(config[LogHubConfig.MinSeverityKey]);
				var formatter = formatters.Resolve(config[LogHubConfig.FormatKey]);
				var processor = new LogProcessor(filter, formatter, sink);

				stats = processor.Process(File.ReadLines(args[0]));
			}
			finally
			{
				(sink as IDisposable)?.Dispose();
			}

			Console.WriteLine($"Записано: {stats.Written}, пропущено: {stats.Skipped}");
			return 0;
		}
	}
}
