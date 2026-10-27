using System;
using LogHub.Model;

namespace LogHub.Sinks
{
	/// <summary>
	/// Вывод в консоль: Error и Critical — красным, остальное — серым.
	/// </summary>
	public sealed class ConsoleSink : ILogSink
	{
		public void Write(LogRecord record, string text)
		{
			var previous = Console.ForegroundColor;
			Console.ForegroundColor = IsAlarm(record.Severity) ? ConsoleColor.Red : ConsoleColor.Gray;
			Console.WriteLine(text);
			Console.ForegroundColor = previous;
		}

		private static bool IsAlarm(string severity)
		{
			return severity == "Error" || severity == "Critical";
		}
	}
}
