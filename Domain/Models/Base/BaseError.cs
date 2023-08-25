namespace Domain.Models.Base;

/// <summary>
/// Стандартная модель ошибки
/// </summary>
public class BaseError
{
    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public BaseError()
    {
        
    }

    /// <summary>
    /// Конструктор с кодом и текстом ошибки
    /// </summary>
    /// <param name="code"></param>
    /// <param name="message"></param>
    public BaseError(int code, string message): this()
    {
        Code = code;
        Message = message;
    }

    /// <summary>
    /// Код ошибки
    /// </summary>
    public int? Code { get; set; }

    /// <summary>
    /// Текст ошибки
    /// </summary>
    public string? Message { get; set; }
}
