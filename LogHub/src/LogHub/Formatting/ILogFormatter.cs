using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary>
	/// Превращает запись в строку вывода.
	/// Новый формат — новый открытый класс, реализующий этот интерфейс,
	/// с атрибутом <see cref="Composition.ConfigNameAttribute"/>: композиционный
	/// корень найдёт его сам.
	/// </summary>
	public interface ILogFormatter
	{
		string Format(LogRecord record);
	}
}
