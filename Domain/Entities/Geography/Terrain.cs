using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность рельефа
/// </summary>
[Table("dir_terrains")]
[Comment("Рельеф")]
public class Terrain : Directory
{
    /// <summary>
    /// Цвет на карте
    /// </summary>
    [Column("color")]
    [Comment("Цвет на карте")]
    public string Color { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности рельефа
    /// </summary>
    public Terrain() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности рельефа
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public Terrain(long id, string user, string name, string color) : base(id, user, name)
    {
        Color = color;
    }

    /// <summary>
    /// Конструктор сущности рельефа без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="color"></param>
    public Terrain(string user, string name, string color) : base(user, name)
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
