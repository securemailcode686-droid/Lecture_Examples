using System.Collections.Generic;

namespace LogHub.Configuration
{
	/// <summary>
	/// Разбор текста конфигурации вида <c>ключ = значение</c>.
	/// Пустые строки и строки, начинающиеся с <c>#</c>, пропускаются;
	/// строки без <c>=</c> игнорируются; повторный ключ переопределяет предыдущий.
	/// </summary>
	public static class ConfigParser
	{
		public static LogHubConfig Parse(IEnumerable<string> lines)
		{
			var values = new Dictionary<string, string>();

			foreach (var line in lines)
			{
				if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
				{
					continue;
				}

				var parts = line.Split('=', 2);
				if (parts.Length == 2)
				{
					values[parts[0].Trim()] = parts[1].Trim();
				}
			}

			return new LogHubConfig(values);
		}
	}
}
