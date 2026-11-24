using LogHub.Configuration;

namespace LogHub.Tests.Doubles
{
	/// <summary>
	/// Конфигурация для теста — свой экземпляр обычного <see cref="ConfigService"/>.
	/// Ничего сбрасывать после теста не нужно: экземпляр больше никто не видит.
	/// </summary>
	internal static class Configs
	{
		public static ConfigService With(params (string Key, string Value)[] pairs)
		{
			var config = new ConfigService();
			foreach (var (key, value) in pairs)
			{
				config.SetValue(key, value);
			}

			return config;
		}

		public static ConfigService Profile(string sink, string minSeverity = "Info", string format = "plain")
		{
			return With(
				(ConfigKeys.Sink, sink),
				(ConfigKeys.MinSeverity, minSeverity),
				(ConfigKeys.Format, format));
		}
	}
}
