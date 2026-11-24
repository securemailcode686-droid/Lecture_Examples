using System.Collections.Generic;
using System.IO;

namespace LogHub.Configuration
{
	/// <summary>
	/// Где лежит конфигурация. Единственное место, которое читает её с диска.
	/// </summary>
	public static class ConfigFile
	{
		/// <summary>Файл в текущем каталоге — как у исходного модуля.</summary>
		public const string DefaultPath = "loghub.config";

		/// <summary>
		/// Переносит настройки из файла в <paramref name="target"/>, а если файла
		/// нет — значения по умолчанию из <see cref="ConfigKeys.Defaults"/>.
		/// </summary>
		public static void LoadInto(IConfigService target, string path)
		{
			var values = File.Exists(path)
				? ConfigParser.Parse(File.ReadAllLines(path))
				: ConfigKeys.Defaults;

			foreach (KeyValuePair<string, string> pair in values)
			{
				target.SetValue(pair.Key, pair.Value);
			}
		}
	}
}
