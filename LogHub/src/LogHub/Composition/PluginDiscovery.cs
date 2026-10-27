using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LogHub.Composition
{
	/// <summary>
	/// Находит реализации интерфейса в сборке. Благодаря этому новый приёмник
	/// или форматтер подключается добавлением файла, без правки списков.
	/// </summary>
	public static class PluginDiscovery
	{
		/// <summary>
		/// Создаёт по экземпляру каждого открытого неабстрактного класса сборки,
		/// который реализует <typeparamref name="T"/> и имеет конструктор без параметров.
		/// Порядок — по полному имени типа, чтобы результат не зависел от компилятора.
		/// </summary>
		public static IReadOnlyList<T> CreateAll<T>(Assembly assembly)
			where T : class
		{
			return assembly.GetExportedTypes()
				.Where(type => type.IsClass
				               && !type.IsAbstract
				               && typeof(T).IsAssignableFrom(type)
				               && type.GetConstructor(Type.EmptyTypes) is not null)
				.OrderBy(type => type.FullName, StringComparer.Ordinal)
				.Select(type => (T)Activator.CreateInstance(type)!)
				.ToList();
		}
	}
}
