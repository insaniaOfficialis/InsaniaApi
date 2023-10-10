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
    /// Пол (истина - мужской/ложь - женский)
    /// </summary>
    [Column("gender")]
    [Comment("Пол (истина - мужской/ложь - женский)")]
    public bool Gender { get; private set; }

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
    /// <param name="gender"></param>
    public PersonalName(long id, string user, string name, bool gender) : base(id, user, name)
    {
        Gender = gender;
    }

    /// <summary>
    /// Конструктор сущности имён без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="gender"></param>
    public PersonalName(string user, string name, bool gender) : base(user, name)
    {
        Gender = gender;
    }
}