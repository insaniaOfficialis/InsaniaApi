namespace Domain.Models.Informations.NewsDetails.Request;

/// <summary>
/// Модель запроса добавления детальной части новости
/// </summary>
public class AddNewsDetailRequest
{
    /// <summary>
    /// Конструктор модели добавления детальной части новости
    /// </summary>
    /// <param name="text"></param>
    /// <param name="newsId"></param>
    /// <param name="ordinalNumber"></param>
    public AddNewsDetailRequest(string? text, long? newsId, long? ordinalNumber)
    {
        Text = text;
        NewsId = newsId;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Тест
    /// </summary>
    public string? Text { get; set; }

    /// <summary>
    /// Ссылка на новость
    /// </summary>
    public long? NewsId { get; set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    public long? OrdinalNumber { get; set; }
}