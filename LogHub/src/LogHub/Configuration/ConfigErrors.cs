using System.Collections.Generic;

namespace LogHub.Configuration
{
	internal static class ConfigErrors
	{
		/// <summary>
		/// Текст совпадает с исключением индексатора <see cref="Dictionary{TKey, TValue}"/>,
		/// которое бросал исходный god-object: от этого зависит сравнение их вывода.
		/// </summary>
		public static KeyNotFoundException KeyNotFound(string key)
		{
			return new KeyNotFoundException($"The given key '{key}' was not present in the dictionary.");
		}
	}
}
