using LogHub.Model;

namespace LogHub.Sinks
{
	/// <summary>
	/// Приёмник записей. Один метод вместо восьми у «универсального»
	/// интерфейса исходного модуля: транзакции, ротация и цвет нужны
	/// не всем приёмникам, и вызывающему коду они не нужны вовсе.
	/// Приёмник, который держит файл или соединение, дополнительно
	/// реализует <see cref="System.IDisposable"/>.
	/// </summary>
	public interface ILogSink
	{
		/// <param name="record">Запись — для приёмников, которым нужны поля (уровень, дата).</param>
		/// <param name="text">Запись, уже отформатированная для вывода.</param>
		void Write(LogRecord record, string text);
	}
}
