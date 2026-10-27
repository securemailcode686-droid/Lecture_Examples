using System.Collections.Generic;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Parsing;
using LogHub.Sinks;

namespace LogHub.Processing
{
	/// <summary>
	/// Конвейер: разбор → фильтрация → форматирование → запись.
	/// Не знает ни откуда пришли строки, ни куда уходят записи.
	/// </summary>
	public sealed class LogProcessor
	{
		private readonly SeverityFilter _filter;
		private readonly ILogFormatter _formatter;
		private readonly ILogSink _sink;

		public LogProcessor(SeverityFilter filter, ILogFormatter formatter, ILogSink sink)
		{
			_filter = filter;
			_formatter = formatter;
			_sink = sink;
		}

		public ProcessingStats Process(IEnumerable<string> lines)
		{
			var written = 0;
			var skipped = 0;

			foreach (var line in lines)
			{
				if (!LogLineParser.TryParse(line, out var record) || !_filter.Passes(record))
				{
					skipped++;
					continue;
				}

				_sink.Write(record, _formatter.Format(record));
				written++;
			}

			return new ProcessingStats(written, skipped);
		}
	}
}
