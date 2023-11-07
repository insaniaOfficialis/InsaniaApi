using Domain.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Domain.Models.Informations.NewsDetails.Response;

/// <summary>
/// Модель ответа списка детальных частей новости
/// </summary>
public class GetNewsDetailsResponse : BaseResponseList
{
    /// <summary>
    /// Простой конструктор модели ответа списка детальных частей новости
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetNewsDetailsResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор с элементами модели ответа списка детальных частей новости
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public GetNewsDetailsResponse(bool success, BaseError? error, List<GetNewsDetailsResponseItem?>? items) :
        base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public List<GetNewsDetailsResponseItem?>? Items { get; set; }
}

/// <summary>
/// Модель элемента ответа списка детальных частей новости
/// </summary>
public class GetNewsDetailsResponseItem : BaseResponseListItem
{
    /// <summary>
    /// Пустой конструктор модели элемента ответа списка детальных частей новости
    /// </summary>
    public GetNewsDetailsResponseItem()
    {
    }

    /// <summary>
    /// Конструктор модели элемента ответа списка детальных частей новости
    /// </summary>
    /// <param name="text"></param>
    /// <param name="files"></param>
    public GetNewsDetailsResponseItem(string? text, List<long> files)
    {
        Text = text;
        Files = files;
    }

    /// <summary>
    /// Конструктор модели элемента ответа списка детальных частей новости с id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="text"></param>
    /// <param name="files"></param>
    public GetNewsDetailsResponseItem(long? id, string? text, List<long> files) : base(id)
    {
        Text = text;
        Files = files;
    }

    /// <summary>
    /// Текст
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Text { get; set; }

    /// <summary>
    /// Список ссылок на изображения
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<long>? Files { get; set; }
}