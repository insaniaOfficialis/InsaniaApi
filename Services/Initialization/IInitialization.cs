namespace Services.Initialization;

/// <summary>
/// Интерфейс инициализации
/// </summary>
public interface IInitialization
{
    /// <summary>
    /// Метод инициализации базы данных
    /// </summary>
    /// <returns></returns>
    Task<bool> InitializeDatabase();
}
