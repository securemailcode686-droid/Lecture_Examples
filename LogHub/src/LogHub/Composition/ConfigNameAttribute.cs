using System;

namespace LogHub.Composition
{
	/// <summary>
	/// Имя, под которым реализация выбирается в конфигурации:
	/// <c>[ConfigName("file")]</c> у приёмника — это <c>sink = file</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ConfigNameAttribute : Attribute
	{
		public ConfigNameAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; }
	}
}
