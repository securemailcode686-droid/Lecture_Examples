using System;
using System.Linq;
using LogHub.Composition;
using LogHub.Model;
using LogHub.Sinks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogHub.Tests
{
	[TestClass]
	public sealed class TypeCatalogTests
	{
		[TestMethod]
		public void СборкаLogHub_СодержитПриёмникиИсходногоМодуля()
		{
			var catalog = TypeCatalog<ILogSink>.FromAssembly(typeof(Program).Assembly);

			CollectionAssert.AreEquivalent(new[] { "console", "database", "file" }, catalog.Names.ToArray());
		}

		[TestMethod]
		public void ИменаСравниваютсяСУчётомРегистра()
		{
			var catalog = new TypeCatalog<ILogSink>(new[] { typeof(SinkA) });

			Assert.IsTrue(catalog.TryGet("a", out _));
			Assert.IsFalse(catalog.TryGet("A", out _));
		}

		[TestMethod]
		public void ДваТипаСОднимИменем_ОшибкаПриСоздании()
		{
			Assert.ThrowsExactly<ArgumentException>(
				() => { _ = new TypeCatalog<ILogSink>(new[] { typeof(SinkA), typeof(AnotherSinkA) }); });
		}

		[TestMethod]
		public void ТипБезИмени_ОшибкаПриСоздании()
		{
			Assert.ThrowsExactly<ArgumentException>(
				() => { _ = new TypeCatalog<ILogSink>(new[] { typeof(UnnamedSink) }); });
		}

		[ConfigName("a")]
		private sealed class SinkA : ILogSink
		{
			public void Write(LogRecord record, string text)
			{
			}
		}

		[ConfigName("a")]
		private sealed class AnotherSinkA : ILogSink
		{
			public void Write(LogRecord record, string text)
			{
			}
		}

		private sealed class UnnamedSink : ILogSink
		{
			public void Write(LogRecord record, string text)
			{
			}
		}
	}
}
