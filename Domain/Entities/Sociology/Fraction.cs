using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность фракций
/// </summary>
[Table("dir_fractions")]
[Comment("Фракции")]

public class Fraction : Directory
{
    /// <summary>
    /// Пустой конструктор сущности фракций
    /// </summary>
    public Fraction() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности фракций
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Fraction(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности фракций без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Fraction(string user, string name) : base(user, name)
    {
    }
}
