namespace Domain.Models.Informations.News.Request;

/// <summary>
/// Модель запроса добавления новости
/// </summary>
public class AddNewsRequest
{
    /// <summary>
    /// Пустой конструктор добавления новости
    /// </summary>
    public AddNewsRequest()
    {
        
    }

    /// <summary>
    /// Конструктор модели запроса добавления новости
    /// </summary>
    /// <param name="title"></param>
    /// <param name="introduction"></param>
    /// <param name="typeId"></param>
    public AddNewsRequest(string title, string introduction, long typeId)
    {
        Title = title;
        Introduction = introduction;
        TypeId = typeId;
    }

    /// <summary>
    /// Заголовок
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Вступление
    /// </summary>
    public string? Introduction { get; set; }

    /// <summary>
    /// Тип новости
    /// </summary>
    public long? TypeId { get; set; }
}