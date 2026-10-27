using LogHub.Configuration;
using LogHub.Sinks;

namespace LogHub.Tests.Doubles
{
	/// <summary>Фабрика с заданным именем; запоминает конфигурацию, с которой её вызвали.</summary>
	internal sealed class FakeSinkFactory : ISinkFactory
	{
		public FakeSinkFactory(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public SpySink Created { get; } = new SpySink();

		public LogHubConfig? ReceivedConfig { get; private set; }

		public ILogSink Create(LogHubConfig config)
		{
			ReceivedConfig = config;
			return Created;
		}
	}
}
