using LogHub.Configuration;

namespace LogHub.Sinks
{
	public sealed class ConsoleSinkFactory : ISinkFactory
	{
		public string Name => "console";

		public ILogSink Create(LogHubConfig config)
		{
			return new ConsoleSink();
		}
	}
}
