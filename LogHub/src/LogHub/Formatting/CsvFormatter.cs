using LogHub.Composition;
using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary><c>дата;уровень;сообщение</c></summary>
	[ConfigName("csv")]
	public sealed class CsvFormatter : ILogFormatter
	{
		public string Format(LogRecord record)
		{
			return $"{record.Date:O};{record.Severity};{record.Message}";
		}
	}
}
