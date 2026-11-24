namespace LogHub.Configuration
{
	/// <summary>
	/// Настройки LogHub: пары «ключ = значение».
	/// Реализаций три — классический синглтон, синглтон через <see cref="System.Lazy{T}"/>
	/// и обычный класс, временем жизни которого управляет контейнер. Приложение
	/// использует только последний (см. <see cref="Composition.CompositionRoot"/>);
	/// первые два оставлены, чтобы на тестах показать разницу.
	/// </summary>
	public interface IConfigService
	{
		/// <exception cref="System.Collections.Generic.KeyNotFoundException">Ключа нет.</exception>
		string GetValue(string key);

		void SetValue(string key, string value);
	}
}
