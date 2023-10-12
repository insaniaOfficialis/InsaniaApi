using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность фамилий
/// </summary>
[Table("dir_last_names")]
[Comment("Фамилии")]
public class LastName : Directory
{
    /// <summary>
    /// Пустой конструктор сущности фамилий
    /// </summary>
    public LastName() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности фамилий
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public LastName(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности фамилий без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public LastName(string user, string name) : base(user, name)
    {
    }
}
