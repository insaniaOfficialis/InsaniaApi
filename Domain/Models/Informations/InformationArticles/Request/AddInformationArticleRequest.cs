namespace Domain.Models.Informations.InformationArticles.Request;

/// <summary>
/// Модель запроса добавления статьи
/// </summary>
public class AddInformationArticleRequest
{
    /// <summary>
    /// Конструктор модели запроса добавления статьи
    /// </summary>
    /// <param name="title"></param>
    /// <param name="ordinalNumber"></param>
    public AddInformationArticleRequest(string title, long? ordinalNumber)
    {
        Title = title;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Заголовок
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    public long? OrdinalNumber { get; set; }
}