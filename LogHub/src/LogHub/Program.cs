using System;

namespace LogHub
{
	/// <summary>
	/// Точка входа LogHub. Пока только каркас: обработка логов появится
	/// в части B лабораторной работы 1.
	/// </summary>
	public static class Program
	{
		public const string Usage = "Использование: LogHub <путь к файлу лога>";

		public static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine(Usage);
				return 2;
			}

			Console.WriteLine($"LogHub: каркас, файл {args[0]} пока не обрабатывается.");
			return 0;
		}
	}
}
