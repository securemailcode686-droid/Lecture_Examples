using System;
using System.Collections.Concurrent;

namespace LogHub.Configuration
{
	/// <summary>
	/// Обычный класс с публичным конструктором. Он не знает, сколько у него
	/// экземпляров: в приложении он один, потому что так зарегистрирован
	/// в контейнере, а каждый тест создаёт свой.
	/// </summary>
	public sealed class ConfigService : IConfigService
	{
		private readonly ConcurrentDictionary<string, string> _values =
			new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

		public string GetValue(string key)
		{
			return _values.TryGetValue(key, out var value) ? value : throw ConfigErrors.KeyNotFound(key);
		}

		public void SetValue(string key, string value)
		{
			_values[key] = value;
		}
	}
}
