using System.Collections.Generic;

namespace LogHub.Configuration
{
	/// <summary>
	/// Настройки LogHub: набор пар «ключ = значение».
	/// Знает имена ключей и значения по умолчанию, но не знает,
	/// откуда и в каком формате настройки пришли.
	/// </summary>
	public sealed class LogHubConfig
	{
		public const string SinkKey = "sink";
		public const string MinSeverityKey = "minSeverity";
		public const string FormatKey = "format";

		private readonly IReadOnlyDictionary<string, string> _values;

		public LogHubConfig(IReadOnlyDictionary<string, string> values)
		{
			_values = values;
		}

		/// <summary>
		/// Настройки, которые действуют, когда файла конфигурации нет.
		/// </summary>
		public static LogHubConfig Defaults { get; } = new LogHubConfig(new Dictionary<string, string>
		{
			[SinkKey] = "console",
			[MinSeverityKey] = "Info",
			[FormatKey] = "plain"
		});

		/// <summary>
		/// Значение обязательного ключа.
		/// </summary>
		/// <exception cref="KeyNotFoundException">Ключа нет в конфигурации.</exception>
		public string this[string key] => _values[key];

		public string GetValueOrDefault(string key, string defaultValue)
		{
			return _values.TryGetValue(key, out var value) ? value : defaultValue;
		}
	}
}
