using System.IO;

namespace LogHub.Tests.Doubles
{
	/// <summary>Текстовый поток в памяти, который считает сбросы и закрытие.</summary>
	internal sealed class CountingWriter : StringWriter
	{
		public int FlushCalls { get; private set; }

		public bool IsDisposed { get; private set; }

		public override void Flush()
		{
			FlushCalls++;
			base.Flush();
		}

		protected override void Dispose(bool disposing)
		{
			IsDisposed = true;
			base.Dispose(disposing);
		}
	}
}
