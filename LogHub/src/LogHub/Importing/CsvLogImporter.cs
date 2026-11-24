using System;
using System.IO;
using System.Globalization;
using LogHub.Model;
using LogHub.Composition;
using Microsoft.VisualBasic.FileIO;

namespace LogHub.Importing
{
    [ConfigName("csv")]
    public sealed class CsvLogImporter : LogImporterBase
    {
        protected override string? Read(TextReader reader)
        {
            return reader.ReadLine();
        }

        protected override LogRecord? Parse(string line)
        {
            using var parser = new TextFieldParser(
                new StringReader(line));

            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.HasFieldsEnclosedInQuotes = true;

            string[]? fields;

            try
            {
                fields = parser.ReadFields();
            }
            catch (MalformedLineException)
            {
                return null;
            }

            if (fields == null || fields.Length != 3)
                return null;

            if (!DateTime.TryParse(
                fields[0],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
                return null;

            return new LogRecord(
                date,
                fields[1].Trim(),
                fields[2]);
        }
    }
}
