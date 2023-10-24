using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Informations;

/// <summary>
/// Сущность типов новостей
/// </summary>
[Table("dir_news_types")]
[Comment("Типы новостей")]
public class NewsType : Directory
{
    /// <summary>
    /// Цвет
    /// </summary>
    [Column("color")]
    [Comment("Цвет")]
    public string Color { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public NewsType()
    {

    }

    /// <summary>
    /// Конструктор сущности типов новостей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public NewsType(string user, string name, string color) : base(user, name)
    {
        Color = color;
    }

    /// <summary>
    /// Конструктор сущности типов новостей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public NewsType(long id, string user, string name, string color) : base(id, user, name)
    {
        Color = color;
    }

    /// <summary>
    /// Метод записи цвета
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(string color)
    {
        Color = color;
    }
}
