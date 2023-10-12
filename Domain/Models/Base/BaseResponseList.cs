using System.Text.Json.Serialization;

namespace Domain.Models.Base;

/// <summary>
/// Модель стандартного ответа для списка
/// </summary>
public class BaseResponseList : BaseResponse
{
    /// <summary>
    /// Простой конструктор ответа модели ответа списка
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public BaseResponseList(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор с элементами модели ответа списка
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public BaseResponseList(bool success, BaseError? error, List<BaseResponseListItem?>? items) : base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public List<BaseResponseListItem?>? Items { get; set; }
}

/// <summary>
/// Модель элемента списка
/// </summary>
public class BaseResponseListItem
{
    /// <summary>
    /// Пустой конструктор модели элемента списка
    /// </summary>
    public BaseResponseListItem()
    {
    }

    /// <summary>
    /// Конструктор модели элемента списка
    /// </summary>
    /// <param name="name"></param>
    public BaseResponseListItem(string? name)
    {
        Name = name;
    }

    /// <summary>
    /// Первичный ключ
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? Id { get; set; }

    /// <summary>
    /// Наименование
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Name { get; set; }
}
