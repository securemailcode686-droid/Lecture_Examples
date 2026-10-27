using System;
using LogHub.Model;

namespace LogHub.Sinks
{
	/// <summary>
	/// Запись в таблицу <c>logs</c>. Получает уже открытое соединение
	/// и закрывает его при освобождении.
	/// </summary>
	public sealed class DatabaseSink : ILogSink, IDisposable
	{
		public const string InsertSql = "INSERT INTO logs(date, severity, message) VALUES(@d, @s, @m)";

		private readonly ILogDatabase _database;

		public DatabaseSink(ILogDatabase database)
		{
			_database = database;
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
