using System;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using LogHub.Model;
using LogHub.Composition;

namespace LogHub.Importing
{
    [ConfigName("nginx")]
    public sealed class NginxLogImporter : LogImporterBase
    {
        private static readonly Regex LinePattern = new(
            @"^(?<date>\d{4}/\d{2}/\d{2} \d{2}:\d{2}:\d{2})\s+\[(?<level>\w+)\]\s+(?<message>.+)$");

        private static readonly Regex StatusPattern = new(
            @"(?:^|,\s*)(?:upstream\s+status|status(?:_code)?|response\s+code)\s*[:=]\s*(?<code>\d+)\b",
            RegexOptions.IgnoreCase);

        protected override string? Read(TextReader reader)
        {
            return reader.ReadLine();
        }

        protected override LogRecord? Parse(string line)
        {
            var match = LinePattern.Match(line);

            if (!match.Success)
                return null;

            if (!DateTime.TryParseExact(
                match.Groups["date"].Value,
                "yyyy/MM/dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
                return null;

            var severity = match.Groups["level"].Value switch
            {
                "debug" => "Debug",
                "info" => "Info",
                "notice" => "Info",
                "warn" => "Warning",
                "error" => "Error",
                "crit" => "Critical",
                "alert" => "Critical",
                "emerg" => "Critical",
                _ => null
            };

            if (severity == null)
                return null;

            var message = match.Groups["message"].Value;
            var status = StatusPattern.Match(message);

            int? statusCode = null;

            if (status.Success)
            {
                if (!int.TryParse(
                    status.Groups["code"].Value,
                    out var code))
                    return null;

                statusCode = code;
            }

            return new LogRecord(date, severity, message)
            {
                StatusCode = statusCode
            };
        }

        protected override bool Validate(LogRecord record)
        {
            return record.StatusCode is null or (>= 100 and <= 599);
        }
    }
}
