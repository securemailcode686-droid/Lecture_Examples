using System;
using LogHub.Configuration;

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

		/// <summary>Строка подключения — ключ <c>connection</c>, по умолчанию <c>local</c>.</summary>
		public ConsoleDatabaseStub(IConfigService config)
		{
			_connectionString = config.GetValueOrDefault(ConfigKeys.Connection, "local");
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
