using LogHub.Filtering;
using LogHub.Tests.Doubles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class SeverityFilterTests
	{
		[TestMethod]
		[DataRow("Info", "Debug", false)]
		[DataRow("Info", "Info", true)]
		[DataRow("Warning", "Error", true)]
		[DataRow("Error", "Warning", false)]
		[DataRow("Critical", "Critical", true)]
		[DataRow("Info", "Trace", true, DisplayName = "неизвестный уровень записи = Info")]
		[DataRow("Verbose", "Debug", false, DisplayName = "неизвестный порог = Info")]
		[DataRow("Warning", "error", false, DisplayName = "регистр важен: error ≠ Error")]
		public void Фильтр_ПропускаетЗаписиНеНижеПорога(string minSeverity, string severity, bool expected)
		{
			var filter = new SeverityFilter(minSeverity);

			Assert.AreEqual(expected, filter.Passes(Records.Of(severity)));
		}
	}
}
