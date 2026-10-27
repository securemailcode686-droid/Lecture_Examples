using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary><c>дата;уровень;сообщение</c></summary>
	public sealed class CsvFormatter : ILogFormatter
	{
		public string Name => "csv";

		public string Format(LogRecord record)
		{
			return $"{record.Date:O};{record.Severity};{record.Message}";
		}
	}
}
