using Domain.Models.Base;
using System.Text.Json.Serialization;

namespace Domain.Models.Informations.News.Response;

/// <summary>
/// Модель ответа списка новостей для таблицы
/// </summary>
public class GetNewsTableResponse : BaseResponseList
{
    /// <summary>
    /// Простой конструктор модели ответа списка новостей для таблицы
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetNewsTableResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор с элементами модели ответа списка новостей для таблицы
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public GetNewsTableResponse(bool success, BaseError? error, List<GetNewsTableResponseItem>? items) :
        base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public new List<GetNewsTableResponseItem>? Items { get; set; }
}

/// <summary>
/// Модель элемента ответа списка новостей для таблицы
/// </summary>
public class GetNewsTableResponseItem : BaseResponseListItem
{
    /// <summary>
    /// Пустой конструктор модели элемента ответа списка новостей для таблицы
    /// </summary>
    public GetNewsTableResponseItem() : base()
    {
    }

    /// <summary>
    /// Конструктор модели элемента ответа списка новостей для таблицы
    /// </summary>
    /// <param name="id"></param>
    /// <param name="title"></param>
    /// <param name="inproduction"></param>
    /// <param name="color"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="type"></param>
    public GetNewsTableResponseItem(long id, string title, string inproduction, string color, long? ordinalNumber,
        BaseResponseListItem? type) : base(id)
    {
        Title = title;
        Introduction = inproduction;
        Color = color;
        OrdinalNumber = ordinalNumber;
        Type = type;
    }

    /// <summary>
    /// Заголовок
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Title { get; set; }

    /// <summary>
    /// Вступление
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Introduction { get; set; }

    /// <summary>
    /// Цвет
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Color { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? OrdinalNumber { get; set; }

    /// <summary>
    /// Тип
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public BaseResponseListItem? Type { get; set; }
}