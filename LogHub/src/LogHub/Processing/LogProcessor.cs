using System.Collections.Generic;
using LogHub.Filtering;
using LogHub.Formatting;
using LogHub.Importing;
using LogHub.Sinks;

namespace LogHub.Processing
{
    /// <summary>
    /// Импорт, фильтрация, форматирование и запись.
    /// </summary>
    public sealed class LogProcessor
    {
        private readonly ILogSink _sink;
        private readonly SeverityFilter _filter;
        private readonly ILogFormatter _formatter;
        private readonly ILogImporter _importer;

        public LogProcessor(
            ILogSink sink,
            SeverityFilter filter,
            ILogFormatter formatter,
            ILogImporter importer)
        {
            _sink = sink;
            _filter = filter;
            _formatter = formatter;
            _importer = importer;
        }

        public ProcessingStats Process(IEnumerable<string> lines)
        {
            return Process(LogInput.FromLines(lines));
        }

        public ProcessingStats Process(LogInput input)
        {
            var written = 0;
            var skipped = 0;

            foreach (var record in _importer.Import(input))
            {
                if (record == null || !_filter.Passes(record))
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
