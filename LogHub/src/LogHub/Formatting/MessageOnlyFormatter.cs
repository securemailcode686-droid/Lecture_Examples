using LogHub.Composition;
using LogHub.Model;

namespace LogHub.Formatting
{
	/// <summary>
	/// Только текст сообщения. Используется и для неизвестного значения
	/// <c>format</c> — так вёл себя исходный модуль.
	/// </summary>
	[ConfigName("message")]
	public sealed class MessageOnlyFormatter : ILogFormatter
	{
		public string Format(LogRecord record)
		{
			return record.Message;
		}
	}
}
