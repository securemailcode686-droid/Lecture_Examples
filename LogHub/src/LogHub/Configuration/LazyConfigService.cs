using System;
using System.Collections.Concurrent;

namespace LogHub.Configuration
{
	/// <summary>
	/// Синглтон через <see cref="Lazy{T}"/>: потокобезопасную однократную
	/// инициализацию гарантирует сам тип, ручная блокировка не нужна.
	/// Это исправляет потокобезопасность, но не тестируемость: экземпляр
	/// по-прежнему один на процесс и доступен глобально. В приложении не используется.
	/// </summary>
	public sealed class LazyConfigService : IConfigService
	{
		private static readonly Lazy<LazyConfigService> Holder =
			new Lazy<LazyConfigService>(() => new LazyConfigService());

		private readonly ConcurrentDictionary<string, string> _values =
			new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

		private LazyConfigService()
		{
		}

		public static LazyConfigService Instance => Holder.Value;

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
