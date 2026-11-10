using System;

namespace LogHub.Model
{
	/// <summary>
	/// Разобранная запись лога. Неизменяемая: после разбора её никто не правит.
	/// </summary>
	public sealed record LogRecord(DateTime Date, string Severity, string Message);
}
