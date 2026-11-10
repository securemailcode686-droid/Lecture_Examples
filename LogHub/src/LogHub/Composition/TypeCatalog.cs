using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace LogHub.Composition
{
	/// <summary>
	/// Реализации <typeparamref name="T"/>, помеченные <see cref="ConfigNameAttribute"/>,
	/// по их именам. Хранит только типы и ничего не создаёт: создаёт контейнер.
	/// Благодаря каталогу новый приёмник или формат подключается новым файлом,
	/// без правки композиционного корня.
	/// </summary>
	public sealed class TypeCatalog<T>
		where T : class
	{
		private readonly Dictionary<string, Type> _types;

		/// <param name="types">Типы с атрибутом <see cref="ConfigNameAttribute"/>; имена должны быть уникальны.</param>
		public TypeCatalog(IEnumerable<Type> types)
		{
			_types = types.ToDictionary(NameOf, StringComparer.Ordinal);
		}

		/// <summary>Все открытые неабстрактные реализации <typeparamref name="T"/> с атрибутом имени.</summary>
		public static TypeCatalog<T> FromAssembly(Assembly assembly)
		{
			return new TypeCatalog<T>(assembly.GetExportedTypes()
				.Where(type => type.IsClass
				               && !type.IsAbstract
				               && typeof(T).IsAssignableFrom(type)
				               && type.GetCustomAttribute<ConfigNameAttribute>() is not null));
		}

		public IReadOnlyCollection<string> Names => _types.Keys;

		public bool TryGet(string name, [NotNullWhen(true)] out Type? type)
		{
			return _types.TryGetValue(name, out type);
		}

		private static string NameOf(Type type)
		{
			if (!typeof(T).IsAssignableFrom(type))
			{
				throw new ArgumentException($"{type.FullName} не реализует {typeof(T).Name}.", nameof(type));
			}

			return type.GetCustomAttribute<ConfigNameAttribute>()?.Name
			       ?? throw new ArgumentException($"У {type.FullName} нет атрибута ConfigName.", nameof(type));
		}
	}
}
