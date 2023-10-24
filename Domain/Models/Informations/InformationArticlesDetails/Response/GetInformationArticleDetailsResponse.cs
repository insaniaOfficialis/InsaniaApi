using Domain.Models.Base;
using System.Text.Json.Serialization;

namespace Domain.Models.Informations.InformationArticlesDetails.Response;

/// <summary>
/// Модель ответа списка детальных частей иинформационной статьи
/// </summary>
public class GetInformationArticleDetailsResponse : BaseResponseList
{
    /// <summary>
    /// Простой конструктор модели ответа списка детальных частей иинформационной статьи
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetInformationArticleDetailsResponse(bool success, BaseError? error) : base(success, error)
    {

    }

    /// <summary>
    /// Конструктор с элементами модели ответа списка детальных частей иинформационной статьи
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    /// <param name="items"></param>
    public GetInformationArticleDetailsResponse(bool success, BaseError? error, List<GetInformationArticleDetailsResponseItem?>? items) : 
        base(success, error)
    {
        Items = items;
    }

    /// <summary>
    /// Список
    /// </summary>
    public List<GetInformationArticleDetailsResponseItem?>? Items { get; set; }
}

/// <summary>
/// Модель элемента ответа списка детальных частей иинформационной статьи
/// </summary>
public class GetInformationArticleDetailsResponseItem : BaseResponseListItem
{
    /// <summary>
    /// Пустой конструктор модели элемента ответа списка детальных частей иинформационной статьи
    /// </summary>
    public GetInformationArticleDetailsResponseItem()
    {
    }

    /// <summary>
    /// Конструктор модели элемента ответа списка детальных частей иинформационной статьи
    /// </summary>
    /// <param name="text"></param>
    /// <param name="files"></param>
    public GetInformationArticleDetailsResponseItem(string? text, List<long> files)
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