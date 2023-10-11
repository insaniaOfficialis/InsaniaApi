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
    /// Пол (истина - мужской/ложь - женский)
    /// </summary>
    [Column("gender")]
    [Comment("Пол (истина - мужской/ложь - женский)")]
    public bool Gender { get; private set; }

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
    /// <param name="gender"></param>
    public LastName(long id, string user, string name, bool gender) : base(id, user, name)
    {
        Gender = gender;
    }

    /// <summary>
    /// Конструктор сущности фамилий без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="gender"></param>
    public LastName(string user, string name, bool gender) : base(user, name)
    {
        Gender = gender;
    }

    /// <summary>
    /// Метод записи пола
    /// </summary>
    /// <param name="gender"></param>
    public void SetGender(bool gender)
    {
        Gender = gender;
    }
}
