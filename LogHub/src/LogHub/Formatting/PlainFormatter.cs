using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary><c>2026-01-15 10:00:01 [Info] сообщение</c></summary>
	public sealed class PlainFormatter : ILogFormatter
	{
		public string Name => "plain";

		public string Format(LogRecord record)
		{
			return $"{record.Date:yyyy-MM-dd HH:mm:ss} [{record.Severity}] {record.Message}";
		}
	}
}
