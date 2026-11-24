using System;
using LogHub.Model;

namespace LogHub.Tests.Doubles
{
	internal static class Records
	{
		public static readonly DateTime Moment = new DateTime(2026, 1, 15, 10, 0, 2);

		public static LogRecord Of(string severity, string message = "сообщение")
		{
			return new LogRecord(Moment, severity, message);
		}
	}
}
