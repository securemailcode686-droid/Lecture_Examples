using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LogHub.Composition;

namespace LogHub.Formatting
{
	/// <summary>
	/// Выбор форматтера по имени из конфигурации.
	/// </summary>
	public sealed class FormatterRegistry
	{
		private readonly Dictionary<string, ILogFormatter> _formatters;
		private readonly ILogFormatter _fallback;

		/// <param name="formatters">Известные форматтеры; имена должны быть уникальны.</param>
		/// <param name="fallback">Что использовать для неизвестного имени.</param>
		public FormatterRegistry(IEnumerable<ILogFormatter> formatters, ILogFormatter fallback)
		{
			_formatters = formatters.ToDictionary(f => f.Name, StringComparer.Ordinal);
			_fallback = fallback;
		}

		/// <summary>Все открытые реализации <see cref="ILogFormatter"/> из сборки.</summary>
		public static FormatterRegistry FromAssembly(Assembly assembly, ILogFormatter fallback)
		{
			return new FormatterRegistry(PluginDiscovery.CreateAll<ILogFormatter>(assembly), fallback);
		}

		public IReadOnlyCollection<string> Names => _formatters.Keys;

		public ILogFormatter Resolve(string name)
		{
			return _formatters.TryGetValue(name, out var formatter) ? formatter : _fallback;
		}
	}
}
