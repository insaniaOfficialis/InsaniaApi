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
    /// <param name="ordinalNumber"></param>
    public AddNewsRequest(string title, string introduction, long typeId, long? ordinalNumber)
    {
        Title = title;
        Introduction = introduction;
        TypeId = typeId;
        OrdinalNumber = ordinalNumber;
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

    /// <summary>
    /// Порядковый номер
    /// </summary>
    public long? OrdinalNumber { get; set; }
}