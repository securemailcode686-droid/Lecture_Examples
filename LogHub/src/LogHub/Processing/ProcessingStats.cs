namespace LogHub.Processing
{
	/// <summary>
	/// Итог прогона. <see cref="Skipped"/> — и неразобранные строки,
	/// и отфильтрованные записи: исходный модуль их не различал.
	/// </summary>
	public readonly record struct ProcessingStats(int Written, int Skipped);
}
