using System.Collections.Generic;
using LogHub.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	/// <summary>
	/// Классический синглтон. Экземпляр один на весь тестовый прогон,
	/// а сбросить его нельзя — конструктор приватный. Поэтому тесты этого
	/// класса делят состояние и зависят от порядка выполнения.
	/// Параллельно не запускаются: состояние общее.
	/// </summary>
	[TestClass]
	[DoNotParallelize]
	public sealed class ClassicConfigServiceTests
	{
		[TestMethod]
		public void Instance_ВсегдаОдинИТотЖеОбъект()
		{
			Assert.AreSame(ClassicConfigService.Instance(), ClassicConfigService.Instance());
		}

		[TestMethod]
		public void ЗаписьЧерезОднуСсылку_ВиднаЧерезЛюбуюДругую()
		{
			var writer = ClassicConfigService.Instance();
			var reader = ClassicConfigService.Instance();

			writer.SetValue(ConfigKeys.Sink, "file");

			Assert.AreEqual("file", reader.GetValue(ConfigKeys.Sink));
		}

		/// <summary>
		/// Обычный тест, сам по себе правильный. Но после него в общем
		/// экземпляре навсегда остаётся minSeverity = Warning.
		/// </summary>
		[TestMethod]
		public void Порог_ЗаписываетсяИЧитается()
		{
			ClassicConfigService.Instance().SetValue(ConfigKeys.MinSeverity, "Warning");

			Assert.AreEqual("Warning", ClassicConfigService.Instance().GetValue(ConfigKeys.MinSeverity));
		}

		/// <summary>
		/// АНТИПРИМЕР, намеренно оставлен. Проверяет то же, что
		/// <see cref="ConfigServiceTests.НовыйСервис_ПорогНеЗадан"/>, но для синглтона.
		/// В одиночку проходит: экземпляр создаётся впервые и пуст.
		/// В наборе падает: раньше него выполнился <see cref="Порог_ЗаписываетсяИЧитается"/>.
		/// Воспроизведение — tools/show-singleton-order-problem.sh (шаг CI):
		/// с -p:ShowOrderProblem=true атрибут Ignore снимается.
		/// </summary>
		[TestMethod]
#if !SHOW_ORDER_PROBLEM
		[Ignore("Зависит от порядка: проходит в одиночку и падает в наборе, потому что Порог_ЗаписываетсяИЧитается уже записал minSeverity в общий экземпляр, а сбросить синглтон нельзя. Разбор — docs/ЛР2.md")]
#endif
		public void НовыйСервис_ПорогНеЗадан()
		{
			var config = ClassicConfigService.Instance();

			Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = config.GetValue(ConfigKeys.MinSeverity); });
		}
	}
}
