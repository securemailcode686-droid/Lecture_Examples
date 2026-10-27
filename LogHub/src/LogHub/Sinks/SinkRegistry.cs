using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogHub.Composition;
using LogHub.Configuration;

namespace LogHub.Sinks
{
	/// <summary>
	/// Выбор приёмника по имени. Заменяет три <c>switch</c> по типу приёмника
	/// исходного модуля: список приёмников не хранится нигде, кроме самих фабрик.
	/// </summary>
	public sealed class SinkRegistry
	{
		private readonly Dictionary<string, ISinkFactory> _factories;

		/// <param name="factories">Фабрики; имена должны быть уникальны.</param>
		public SinkRegistry(IEnumerable<ISinkFactory> factories)
		{
			_factories = factories.ToDictionary(f => f.Name, StringComparer.Ordinal);
		}

		/// <summary>Все открытые реализации <see cref="ISinkFactory"/> из сборки.</summary>
		public static SinkRegistry FromAssembly(Assembly assembly)
		{
			return new SinkRegistry(PluginDiscovery.CreateAll<ISinkFactory>(assembly));
		}

		public IReadOnlyCollection<string> Names => _factories.Keys;

		/// <exception cref="NotSupportedException">Приёмника с таким именем нет.</exception>
		public ILogSink Create(string name, LogHubConfig config)
		{
			if (!_factories.TryGetValue(name, out var factory))
			{
				throw new NotSupportedException($"Приёмник '{name}' не поддерживается");
			}

			return factory.Create(config);
		}
	}
}
