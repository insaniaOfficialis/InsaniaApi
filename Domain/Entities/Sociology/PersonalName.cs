using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность имён
/// </summary>
[Table("dir_personal_names")]
[Comment("Имена")]
public class PersonalName : Directory
{
    /// <summary>
    /// Пустой конструктор сущности имён
    /// </summary>
    public PersonalName() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности имён
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public PersonalName(long id, string user, string name) : base(id, user, name)
    {
    }

    /// <summary>
    /// Конструктор сущности имён без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public PersonalName(string user, string name) : base(user, name)
    {
    }
}