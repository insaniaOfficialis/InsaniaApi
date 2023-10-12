namespace Domain.Models.Base;

/// <summary>
/// Модель стандартного ответа со значением
/// </summary>
public class BaseResponseValue : BaseResponse
{
    /// <summary>
    /// Значение
    /// </summary>
    public object? Value { get; set; }

    /// <summary>
    /// Простой конструктор модели стандартного ответа со значением
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public BaseResponseValue(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор модели стандартного ответа со значением
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="value"></param>
    public BaseResponseValue(bool success, BaseError? error, object value) : base(success, error)
    {
        Value = value;
    }
}
