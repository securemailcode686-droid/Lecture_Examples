using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LogHub.Model;

namespace LogHub.Parsing
{
	/// <summary>
	/// Разбор строки формата <c>дата|уровень|сообщение</c>.
	/// Единственный класс, который знает этот формат.
	/// </summary>
	public static class LogLineParser
	{
		/// <summary>
		/// Пытается разобрать строку. Строка не разбирается, если в ней меньше
		/// трёх полей или первое поле — не дата. Вертикальные черты после
		/// второй остаются частью сообщения.
		/// </summary>
		public static bool TryParse(string line, [NotNullWhen(true)] out LogRecord? record)
		{
			record = null;

			var parts = line.Split('|', 3);
			if (parts.Length != 3)
			{
				return false;
			}

			if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
			{
				return false;
			}

			record = new LogRecord(date, parts[1].Trim(), parts[2].Trim());
			return true;
		}
	}
}
