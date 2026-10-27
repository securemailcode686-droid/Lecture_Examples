using System;

namespace LogHub.Sinks
{
	/// <summary>
	/// То, что <see cref="DatabaseSink"/> требует от клиента базы данных.
	/// Сейчас реализован заглушкой <see cref="ConsoleDatabaseStub"/>;
	/// по варианту 12 в ЛР 5 появится реализация поверх SQLite.
	/// </summary>
	public interface ILogDatabase
	{
		void Open();

		void Execute(string sql, DateTime date, string severity, string text);

		void Close();
	}
}
