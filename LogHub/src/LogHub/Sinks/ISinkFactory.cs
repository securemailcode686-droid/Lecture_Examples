using LogHub.Configuration;

namespace LogHub.Sinks
{
	/// <summary>
	/// Создаёт приёмник по конфигурации. Каждая фабрика знает своё имя
	/// (значение ключа <c>sink</c>) и свои настройки; <see cref="SinkRegistry"/>
	/// находит фабрики сам, поэтому новый приёмник — это только новые файлы.
	/// </summary>
	public interface ISinkFactory
	{
		/// <summary>Значение ключа <c>sink</c> в конфигурации.</summary>
		string Name { get; }

		/// <summary>Создаёт приёмник, готовый к записи (файл открыт, соединение установлено).</summary>
		ILogSink Create(LogHubConfig config);
	}
}
