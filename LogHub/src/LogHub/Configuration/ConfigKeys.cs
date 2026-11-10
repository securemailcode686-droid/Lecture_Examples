using System.Collections.Generic;

namespace LogHub.Configuration
{
	/// <summary>Имена ключей конфигурации и значения по умолчанию.</summary>
	public static class ConfigKeys
	{
		public const string Sink = "sink";
		public const string MinSeverity = "minSeverity";
		public const string Format = "format";
		public const string Connection = "connection";

		/// <summary>Действуют, только когда файла конфигурации нет вовсе — как у исходного модуля.</summary>
		public static IReadOnlyDictionary<string, string> Defaults { get; } = new Dictionary<string, string>
		{
			[Sink] = "console",
			[MinSeverity] = "Info",
			[Format] = "plain"
		};
	}
}
