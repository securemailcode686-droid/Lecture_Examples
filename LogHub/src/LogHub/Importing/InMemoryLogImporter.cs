using System.IO;
using LogHub.Model;
using LogHub.Parsing;
using LogHub.Composition;

namespace LogHub.Importing
{
    [ConfigName("memory")]
    public sealed class InMemoryLogImporter : LogImporterBase
    {
        protected override TextReader Open(LogInput input)
        {
            return input.OpenMemory();
        }

        protected override string? Read(TextReader reader)
        {
            return reader.ReadLine();
        }

        protected override LogRecord? Parse(string line)
        {
            return LogLineParser.TryParse(line, out var record)
                ? record
                : null;
        }
    }
}
