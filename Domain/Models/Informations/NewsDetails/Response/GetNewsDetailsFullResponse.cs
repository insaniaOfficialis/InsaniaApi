using Domain.Models.Base;
using System.Text.Json.Serialization;

namespace Domain.Models.Informations.NewsDetails.Response;

/// <summary>
/// Модель ответа получения полного списка детальных частей новости
/// </summary>
public class GetNewsDetailsFullResponse : BaseResponseList
{
    /// <summary>
    /// Простой конструктор модели ответа получения полного списка детальных частей новости
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetNewsDetailsFullResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор с элементами модели ответа получения полного списка детальных частей новости
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public GetNewsDetailsFullResponse(bool success, BaseError? error, List<GetNewsDetailsFullResponseItem?>? items) :
        base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public new List<GetNewsDetailsFullResponseItem?>? Items { get; set; }
}

/// <summary>
/// Модель элемента ответа получения полного списка детальных частей новости
/// </summary>
public class GetNewsDetailsFullResponseItem : BaseResponseListItem
{
    /// <summary>
    /// Пустой конструктор модели элемента ответа получения полного списка детальных частей новости
    /// </summary>
    public GetNewsDetailsFullResponseItem() : base()
    {
    }

    /// <summary>
    /// Конструктор модели элемента ответа получения полного списка детальных частей новости с id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="isDeleted"></param>
    /// <param name="files"></param>
    public GetNewsDetailsFullResponseItem(long? id, string? text, long? ordinalNumber, bool? isDeleted, List<long> files) : base(id)
    {
        Text = text;
        OrdinalNumber = ordinalNumber;
        IsDeleted = isDeleted;
        Files = files;
    }

    /// <summary>
    /// Текст
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public long? OrdinalNumber { get; set; }

    /// <summary>
    /// Признак удаления
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? IsDeleted { get; set; }

    /// <summary>
    /// Список ссылок на изображения
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<long>? Files { get; set; }
}