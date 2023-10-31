namespace Domain.Models.Informations.NewsDetails.Request;

/// <summary>
/// Модель запроса добавления детальной части новости
/// </summary>
public class AddNewsDetailRequest
{
    /// <summary>
    /// Тест
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Ссылка на новость
    /// </summary>
    public long? NewsId { get; set; }

    /// <summary>
    /// Конструктор модели добавления детальной части новости
    /// </summary>
    /// <param name="text"></param>
    /// <param name="newsId"></param>
    public AddNewsDetailRequest(string? text, long? newsId)
    {
        Text = text;
        NewsId = newsId;
    }
}