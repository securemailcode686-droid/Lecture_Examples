using System;
using System.IO;
using LogHub.Composition;
using LogHub.Model;

namespace LogHub.Sinks
{
	/// <summary>
	/// Запись строк в файл <c>logs-ГГГГММДД.txt</c> текущего каталога (дописывание в конец).
	/// </summary>
	[ConfigName("file")]
	public sealed class FileSink : ILogSink, IDisposable
	{
		private readonly TextWriter _writer;

		/// <summary>
		/// Конструктор, который выбирает контейнер: <see cref="TextWriter"/> в нём
		/// не зарегистрирован, поэтому второй конструктор ему не подходит.
		/// </summary>
		public FileSink()
			: this(new StreamWriter(FileName(DateTime.Now), append: true))
		{
		}

		/// <summary>Для тестов: вместо файла — любой поток, например <see cref="StringWriter"/>.</summary>
		public FileSink(TextWriter writer)
		{
			_writer = writer;
		}

		public static string FileName(DateTime date)
		{
			return $"logs-{date:yyyyMMdd}.txt";
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
