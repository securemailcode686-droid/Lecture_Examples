using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary>
	/// <c>{"date":"…","level":"info","message":"…"}</c>.
	/// Экранируются только двойные кавычки — ровно как в исходном модуле;
	/// исправление экранирования — отдельная задача, не рефакторинг.
	/// </summary>
	public sealed class JsonFormatter : ILogFormatter
	{
		public string Name => "json";

		public string Format(LogRecord record)
		{
			return "{\"date\":\"" + record.Date.ToString("O") + "\"," +
			       "\"level\":\"" + record.Severity.ToLowerInvariant() + "\"," +
			       "\"message\":\"" + record.Message.Replace("\"", "\\\"") + "\"}";
		}
	}
}
