using System.Text.Json.Serialization;

namespace Domain.Models.Base;

/// <summary>
/// Стандартная модель ответа
/// </summary>
public class BaseResponse
{
    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public BaseResponse()
    {
        
    }

    /// <summary>
    /// Конструктор с признаком успешности
    /// </summary>
    /// <param name="success"></param>
    public BaseResponse(bool success): this()
    {
        Success = success;        
    }

    /// <summary>
    /// Конструктор с id
    /// </summary>
    /// <param name="success"></param>
    /// <param name="id"></param>
    public BaseResponse(bool success, long id): this(success)
    {
        Id = id;
    }

    /// <summary>
    /// Конструктор с ошибкой
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public BaseResponse(bool success, BaseError? error): this(success)
    {
        Error = error;
    }

    /// <summary>
    /// Признак успешности ответа
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Id записи
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Id { get; set; }

    /// <summary>
    /// Ошибка
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BaseError? Error { get; set; }
}
