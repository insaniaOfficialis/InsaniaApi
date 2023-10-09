using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Geography;

/// <summary>
/// Сущность климата
/// </summary>
[Table("dir_climate")]
[Comment("Климат")]
public class Climate : Directory
{
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
    public Climate(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности климата без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Climate(string user, string name) : base(user, name)
    {
    }
}
