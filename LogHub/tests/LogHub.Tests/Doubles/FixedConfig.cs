using System;
using System.Collections.Generic;
using LogHub.Configuration;

namespace LogHub.Tests.Doubles
{
	/// <summary>
	/// Своя реализация <see cref="IConfigService"/> для теста: значения заданы
	/// заранее, запись запрещена. Подставляется через конструктор — никакой
	/// статики сбрасывать не нужно.
	/// </summary>
	internal sealed class FixedConfig : IConfigService
	{
		private readonly IReadOnlyDictionary<string, string> _values;

		public FixedConfig(IReadOnlyDictionary<string, string> values)
		{
			_values = values;
		}

		public List<string> Requested { get; } = new List<string>();

		public string GetValue(string key)
		{
			Requested.Add(key);
			return _values.TryGetValue(key, out var value) ? value : throw new KeyNotFoundException(key);
		}

		public void SetValue(string key, string value)
		{
			throw new InvalidOperationException("Тестовая конфигурация только для чтения.");
		}
	}
}
