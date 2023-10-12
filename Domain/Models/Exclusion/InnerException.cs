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

    /// <summary>
    /// Конструктор с текстом ошибки и исключением
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exception"></param>
    public InnerException(string message, Exception exception) : base(message, exception)
    {
    }
}
