using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogHub.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	/// <summary>
	/// Синглтон через Lazy&lt;T&gt;. Потокобезопасность создания теперь гарантирует
	/// тип, но экземпляр по-прежнему один на процесс: тесты делят состояние,
	/// и каждый вынужден брать уникальный ключ, чтобы не мешать соседям.
	/// </summary>
	[TestClass]
	[DoNotParallelize]
	public sealed class LazyConfigServiceTests
	{
		[TestMethod]
		public void Instance_ВсегдаОдинИТотЖеОбъект()
		{
			Assert.AreSame(LazyConfigService.Instance, LazyConfigService.Instance);
		}

		[TestMethod]
		public void ОдновременноеОбращениеИзМногихПотоков_ОдинЭкземпляр()
		{
			var seen = new LazyConfigService[64];

			Parallel.For(0, seen.Length, i => seen[i] = LazyConfigService.Instance);

			Assert.AreEqual(1, seen.Distinct().Count());
		}

		[TestMethod]
		public void ЗаписьВидныВсем_СостояниеГлобальное()
		{
			// Уникальный ключ — единственная защита от соседних тестов.
			var key = "key-" + Guid.NewGuid();

			LazyConfigService.Instance.SetValue(key, "value");

			Assert.AreEqual("value", LazyConfigService.Instance.GetValue(key));
		}

		[TestMethod]
		public void НетКлюча_KeyNotFound()
		{
			Assert.ThrowsExactly<KeyNotFoundException>(() => { _ = LazyConfigService.Instance.GetValue("нет-" + Guid.NewGuid()); });
		}
	}
}
