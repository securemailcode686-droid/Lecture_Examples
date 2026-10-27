using System;
using System.IO;
using LogHub.Model;

namespace LogHub.Sinks
{
	/// <summary>
	/// Запись строк в текстовый поток. Куда ведёт поток — решает фабрика,
	/// поэтому в тестах вместо файла подставляется <see cref="StringWriter"/>.
	/// </summary>
	public sealed class FileSink : ILogSink, IDisposable
	{
		private readonly TextWriter _writer;

		public FileSink(TextWriter writer)
		{
			_writer = writer;
		}

		public void Write(LogRecord record, string text)
		{
			_writer.WriteLine(text);
			// Сброс после каждой строки: запись не теряется, если процесс упадёт.
			_writer.Flush();
		}

		public void Dispose()
		{
			_writer.Dispose();
		}
	}
}
