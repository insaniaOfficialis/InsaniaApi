using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Politics;

/// <summary>
/// Сущность регионов
/// </summary>
[Table("dir_regions")]
[Comment("Регионы")]
public class Region : Directory
{
    /// <summary>
    /// Номер на карте
    /// </summary>
    [Column("number")]
    [Comment("Номер на карте")]
    public int Number { get; private set; }

    /// <summary>
    /// Цвет на карте
    /// </summary>
    [Column("color")]
    [Comment("Цвет на карте")]
    public string Color { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности регионов
    /// </summary>
    public Region() : base()
    {

    }

    /// <summary>
    /// Конструктор сущности регионов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    public Region(long id, string user, string name, int number, string color) : base(id, user, name)
    {
        Number = number;
        Color = color;
    }

    /// <summary>
    /// Конструктор сущности регионов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="number"></param>
    /// <param name="color"></param>
    public Region(string user, string name, int number, string color) : base(user, name)
    {
        Number = number;
        Color = color;
    }

    /// <summary>
    /// Метод записи номера на карте
    /// </summary>
    /// <param name="number"></param>
    public void SetNumber(int number)
    {
        Number = number;
    }

    /// <summary>
    /// Метод записи цвета на карте
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(string color)
    {
        Color = color;
    }
}