using System.Collections.Generic;
using LogHub.Model;
using LogHub.Sinks;

namespace LogHub.Tests.Doubles
{
	/// <summary>Приёмник, который запоминает всё, что в него записали.</summary>
	internal sealed class SpySink : ILogSink
	{
		public List<(LogRecord Record, string Text)> Written { get; } = new List<(LogRecord Record, string Text)>();

		public void Write(LogRecord record, string text)
		{
			Written.Add((record, text));
		}
	}
}
