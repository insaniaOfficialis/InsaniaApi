using Domain.Models.Base;
using System.Text.Json.Serialization;

namespace Domain.Models.Informations.News.Response;

/// <summary>
/// Модель ответа списка новостей
/// </summary>
public class GetNewsListResponse : BaseResponseList
{
    /// <summary>
    /// Простой конструктор модели ответа списка новостей
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetNewsListResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор с элементами модели ответа списка новостей
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public GetNewsListResponse(bool success, BaseError? error, List<GetNewsResponseItem>? items) :
        base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public new List<GetNewsResponseItem>? Items { get; set; }
}

/// <summary>
/// Модель элемента ответа списка новостей
/// </summary>
public class GetNewsResponseItem : BaseResponseListItem
{
    /// <summary>
    /// Пустой конструктор модели элемента ответа списка новостей
    /// </summary>
    public GetNewsResponseItem() : base()
    {
    }

    /// <summary>
    /// Конструктор модели элемента ответа списка новостей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="title"></param>
    /// <param name="inproduction"></param>
    /// <param name="color"></param>
    public GetNewsResponseItem(long id, string title, string inproduction, string color) : base(id)
    {
        Title = title;
        Introduction = inproduction;
        Color = color;
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
}