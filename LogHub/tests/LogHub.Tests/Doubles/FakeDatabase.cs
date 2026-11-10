using System;
using System.Collections.Generic;
using LogHub.Sinks;

namespace LogHub.Tests.Doubles
{
	/// <summary>База в памяти: запоминает вызовы вместо выполнения SQL.</summary>
	internal sealed class FakeDatabase : ILogDatabase
	{
		public List<(string Sql, DateTime Date, string Severity, string Text)> Executed { get; } =
			new List<(string Sql, DateTime Date, string Severity, string Text)>();

		public bool IsOpen { get; private set; }

		public int CloseCalls { get; private set; }

		public void Open()
		{
			IsOpen = true;
		}

		public void Execute(string sql, DateTime date, string severity, string text)
		{
			Executed.Add((sql, date, severity, text));
		}

		public void Close()
		{
			IsOpen = false;
			CloseCalls++;
		}
	}
}
