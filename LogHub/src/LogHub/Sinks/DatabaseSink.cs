using System;
using LogHub.Composition;
using LogHub.Model;

namespace LogHub.Sinks
{
	/// <summary>
	/// Запись в таблицу <c>logs</c>. Открывает соединение при создании
	/// и закрывает при освобождении — освобождает его контейнер.
	/// </summary>
	[ConfigName("database")]
	public sealed class DatabaseSink : ILogSink, IDisposable
	{
		public const string InsertSql = "INSERT INTO logs(date, severity, message) VALUES(@d, @s, @m)";

		private readonly ILogDatabase _database;

		public DatabaseSink(ILogDatabase database)
		{
			_database = database;
			_database.Open();
		}

		public void Write(LogRecord record, string text)
		{
			_database.Execute(InsertSql, record.Date, record.Severity, text);
		}

		public void Dispose()
		{
			_database.Close();
		}
	}
}
