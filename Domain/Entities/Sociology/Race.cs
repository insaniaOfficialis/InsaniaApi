using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность рас
/// </summary>
[Table("dir_races")]
[Comment("Расы")]
public class Race : Directory
{
    /// <summary>
    /// Пустой конструктор сущности рас
    /// </summary>
    public Race() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности рас
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Race(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности рас без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Race(string user, string name) : base(user, name)
    {
    }
}
