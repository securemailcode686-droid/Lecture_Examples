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
		/// Читает файл, а если его нет — возвращает <see cref="LogHubConfig.Defaults"/>.
		/// </summary>
		public static LogHubConfig Load(string path)
		{
			return File.Exists(path)
				? ConfigParser.Parse(File.ReadAllLines(path))
				: LogHubConfig.Defaults;
		}
	}
}
