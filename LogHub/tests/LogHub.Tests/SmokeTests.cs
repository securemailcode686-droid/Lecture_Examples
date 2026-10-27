using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	/// <summary>
	/// Проверка того, что тестовый проект видит основной.
	/// </summary>
	[TestClass]
	public sealed class SmokeTests
	{
		[TestMethod]
		public void ТестовыйПроект_ВидитСборкуLogHub()
		{
			Assert.AreEqual("LogHub", typeof(Program).Assembly.GetName().Name);
		}
	}
}
