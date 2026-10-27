using LogHub.Configuration;

namespace LogHub.Sinks
{
	/// <summary>
	/// Ключ <c>connection</c> — строка подключения, по умолчанию <c>local</c>.
	/// </summary>
	public sealed class DatabaseSinkFactory : ISinkFactory
	{
		public const string ConnectionKey = "connection";

		public string Name => "database";

		public ILogSink Create(LogHubConfig config)
		{
			var database = new ConsoleDatabaseStub(config.GetValueOrDefault(ConnectionKey, "local"));
			database.Open();
			return new DatabaseSink(database);
		}
	}
}
