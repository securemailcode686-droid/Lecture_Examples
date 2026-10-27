using System;
using System.Collections.Generic;

namespace LogHub.Filtering
{
	/// <summary>
	/// Шкала уровней важности: чем больше ранг, тем важнее запись.
	/// </summary>
	public static class SeverityScale
	{
		/// <summary>Ранг неизвестного уровня — такой же, как у Info.</summary>
		public const int UnknownRank = 1;

		private static readonly Dictionary<string, int> Ranks = new Dictionary<string, int>(StringComparer.Ordinal)
		{
			["Debug"] = 0,
			["Info"] = 1,
			["Warning"] = 2,
			["Error"] = 3,
			["Critical"] = 4
		};

		public static int Rank(string severity)
		{
			return Ranks.TryGetValue(severity, out var rank) ? rank : UnknownRank;
		}
	}
}
