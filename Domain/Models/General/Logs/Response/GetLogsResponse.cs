using Domain.Models.Base;

namespace Domain.Models.General.Logs.Response;

/// <summary>
/// Модель ответа для списка логов
/// </summary>
public class GetLogsResponse : BaseResponseList
{
    /// <summary>
    /// Конструктор модели ответа для списка логов с ошибкой
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetLogsResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор модели ответа для списка логов
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public GetLogsResponse(bool success, BaseError? error, List<GetLogsResponseItem?>? items) : base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public new List<GetLogsResponseItem?>? Items { get; set; }
}

/// <summary>
/// Модель элемента списка
/// </summary>
public class GetLogsResponseItem : BaseResponseListItem
{
    /// <summary>
    /// Первичный ключ лога
    /// </summary>
    public long Id { get; private set; }

    /// <summary>
    /// Наименование вызываемого метода
    /// </summary>
    public string Method { get; private set; }

    /// <summary>
    /// Тип вызываемого метода
    /// </summary>
    public string Type { get; private set; }

    /// <summary>
    /// Признак успешного выполнения
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Дата начала
    /// </summary>
    public DateTime DateStart { get; private set; }

    /// <summary>
    /// Дата окончания
    /// </summary>
    public DateTime? DateEnd { get; private set; }

    /// <summary>
    /// Данные на вход
    /// </summary>
    public string? DataIn { get; private set; }

    /// <summary>
    /// Данные на выход
    /// </summary>
    public string? DataOut { get; private set; }
}