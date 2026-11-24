using System.IO;
using System.Collections.Generic;
using LogHub.Model;

namespace LogHub.Importing
{
    public abstract class LogImporterBase : ILogImporter
    {
        public IEnumerable<LogRecord?> Import(LogInput input)
        {
            using var reader = Open(input);

            string? line;

            while ((line = Read(reader)) != null)
            {
                var record = Parse(line);

                yield return record != null && Validate(record)
                    ? record
                    : null;
            }
        }

        protected virtual TextReader Open(LogInput input)
        {
            return input.Open();
        }

        protected abstract string? Read(TextReader reader);

        protected abstract LogRecord? Parse(string line);

        protected virtual bool Validate(LogRecord record)
        {
            return true;
        }
    }
}
