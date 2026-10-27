using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary>
	/// Превращает запись в строку вывода.
	/// Новый формат — новый класс, реализующий этот интерфейс:
	/// <see cref="FormatterRegistry"/> найдёт его сам.
	/// </summary>
	public interface ILogFormatter
	{
		/// <summary>Значение ключа <c>format</c> в конфигурации.</summary>
		string Name { get; }

		string Format(LogRecord record);
	}
}
