using System;
using Implementation.AntiPatterns.GodObject;

namespace LegacyRunner
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.Error.WriteLine("Использование: LegacyRunner <путь к файлу лога>");
				return 2;
			}

			new LogProcessingModule().Run(args[0]);
			return 0;
		}
	}
}
