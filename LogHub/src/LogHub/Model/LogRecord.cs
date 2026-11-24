using System;

namespace LogHub.Model
{
    /// <summary>Разобранная запись лога.</summary>
    public sealed record LogRecord(
        DateTime Date,
        string Severity,
        string Message)
    {
        public int? StatusCode { get; init; }
    }
}
