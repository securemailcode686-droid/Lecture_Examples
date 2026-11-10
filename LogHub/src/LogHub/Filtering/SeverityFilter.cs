using LogHub.Configuration;
using LogHub.Model;

namespace LogHub.Filtering
{
	/// <summary>
	/// Пропускает записи не ниже уровня из ключа <c>minSeverity</c>.
	/// </summary>
	public sealed class SeverityFilter
	{
		private readonly int _minRank;

		/// <exception cref="System.Collections.Generic.KeyNotFoundException">В конфигурации нет порога.</exception>
		public SeverityFilter(IConfigService config)
		{
			_minRank = SeverityScale.Rank(config.GetValue(ConfigKeys.MinSeverity));
		}

		public bool Passes(LogRecord record)
		{
			return SeverityScale.Rank(record.Severity) >= _minRank;
		}
	}
}
