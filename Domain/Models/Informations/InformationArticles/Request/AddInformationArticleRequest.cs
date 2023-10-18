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
    public AddInformationArticleRequest(string title)
    {
        Title = title;
    }

    /// <summary>
    /// Заголовок
    /// </summary>
    public string? Title { get; set; }
}
