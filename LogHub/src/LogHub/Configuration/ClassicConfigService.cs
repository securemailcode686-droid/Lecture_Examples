using System;
using System.Collections.Generic;

namespace LogHub.Configuration
{
	/// <summary>
	/// Классический синглтон: приватный конструктор и статический <see cref="Instance"/>.
	/// Единственность и способ доступа зашиты в сам класс, поэтому любой код,
	/// который его использует, делит одно состояние на весь процесс, —
	/// в том числе все тесты. В приложении не используется.
	/// </summary>
	public sealed class ClassicConfigService : IConfigService
	{
		private static readonly object InstanceLock = new object();
		private static ClassicConfigService? _instance;

		private readonly object _valuesLock = new object();
		private readonly Dictionary<string, string> _values = new Dictionary<string, string>(StringComparer.Ordinal);

		private ClassicConfigService()
		{
		}

		/// <summary>
		/// Блокировка на каждый вызов, без «двойной проверки»: проще и заведомо
		/// корректно. Двойная проверка без volatile — разбор в отчёте ЛР2.
		/// </summary>
		public static ClassicConfigService Instance()
		{
			lock (InstanceLock)
			{
				return _instance ??= new ClassicConfigService();
			}
		}

		public string GetValue(string key)
		{
			lock (_valuesLock)
			{
				return _values.TryGetValue(key, out var value) ? value : throw ConfigErrors.KeyNotFound(key);
			}
		}

		public void SetValue(string key, string value)
		{
			lock (_valuesLock)
			{
				_values[key] = value;
			}
		}
	}
}
