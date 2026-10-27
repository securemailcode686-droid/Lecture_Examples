using System;
using System.IO;
using LogHub.Configuration;

namespace LogHub.Sinks
{
	/// <summary>
	/// Файл <c>logs-ГГГГММДД.txt</c> в текущем каталоге, дописывание в конец.
	/// </summary>
	public sealed class FileSinkFactory : ISinkFactory
	{
		public string Name => "file";

		public ILogSink Create(LogHubConfig config)
		{
			return new FileSink(new StreamWriter(FileName(DateTime.Now), append: true));
		}

		public static string FileName(DateTime date)
		{
			return $"logs-{date:yyyyMMdd}.txt";
		}
	}
}
