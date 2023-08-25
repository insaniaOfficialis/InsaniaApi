namespace Domain.Models.Exclusion;

/// <summary>
/// Модель исключения аутентификации
/// </summary>
public class InnerException: Exception
{
    /// <summary>
    /// Конструктор с текстом ошибки
    /// </summary>
    /// <param name="message"></param>
    public InnerException(string message): base(message)
    {
    }
}
