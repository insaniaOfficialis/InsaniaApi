namespace Domain.Models.Informations.InformationArticlesDetails.Request;

/// <summary>
/// Модель запроса добавления детальной части информационной статьи
/// </summary>
public class AddInformationArticleDetailRequest
{
    /// <summary>
    /// Тест
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Ссылка на информационнцю статью
    /// </summary>
    public long? InformationArticleId { get; set; }

    /// <summary>
    /// Конструктор модели добавления детальной части информационной статьи
    /// </summary>
    /// <param name="text"></param>
    /// <param name="informationArticleId"></param>
    public AddInformationArticleDetailRequest(string? text, long? informationArticleId)
    {
        Text = text;
        InformationArticleId = informationArticleId;
    }
}