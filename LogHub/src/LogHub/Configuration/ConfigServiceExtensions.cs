using System.Collections.Generic;

namespace LogHub.Configuration
{
	public static class ConfigServiceExtensions
	{
		/// <summary>Значение необязательного ключа.</summary>
		public static string GetValueOrDefault(this IConfigService config, string key, string defaultValue)
		{
			try
			{
				return config.GetValue(key);
			}
			catch (KeyNotFoundException)
			{
				return defaultValue;
			}
		}
	}
}
