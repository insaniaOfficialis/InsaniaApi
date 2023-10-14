using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность климата
/// </summary>
[Table("dir_climates")]
[Comment("Климат")]
public class Climate : Directory
{
    /// <summary>
    /// Цвет на карте
    /// </summary>
    [Column("color")]
    [Comment("Цвет на карте")]
    public string Color { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности климата
    /// </summary>
    public Climate() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности климата
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public Climate(long id, string user, string name, string color) : base(id, user, name)
    {
        Color = color;
    }

    /// <summary>
    /// Конструктор сущности климата без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public Climate(string user, string name, string color) : base(user, name)
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
