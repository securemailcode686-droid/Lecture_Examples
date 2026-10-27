using System;

namespace LogHub.Sinks
{
	/// <summary>
	/// Заглушка вместо настоящей базы: печатает команды в консоль.
	/// Вывод совпадает с заглушкой учебного репозитория — от этого
	/// зависит сравнение поведения с исходным модулем.
	/// </summary>
	public sealed class ConsoleDatabaseStub : ILogDatabase
	{
		private readonly string _connectionString;
		private bool _open;

		public ConsoleDatabaseStub(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void Open()
		{
			_open = true;
			Console.WriteLine($"[БД] соединение открыто: {_connectionString}");
		}

		public void Execute(string sql, DateTime date, string severity, string text)
		{
			if (!_open)
			{
				throw new InvalidOperationException("Соединение не открыто");
			}

			Console.WriteLine($"[БД] {sql} -> {date:O}, {severity}, {text}");
		}

		public void Close()
		{
			_open = false;
			Console.WriteLine("[БД] соединение закрыто");
		}
	}
}
