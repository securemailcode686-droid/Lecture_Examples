using LogHub.Model;

namespace LogHub.Filtering
{
	/// <summary>
	/// Пропускает записи не ниже заданного уровня.
	/// </summary>
	public sealed class SeverityFilter
	{
		private readonly int _minRank;

		public SeverityFilter(string minSeverity)
		{
			_minRank = SeverityScale.Rank(minSeverity);
		}

		public bool Passes(LogRecord record)
		{
			return SeverityScale.Rank(record.Severity) >= _minRank;
		}
	}
}
