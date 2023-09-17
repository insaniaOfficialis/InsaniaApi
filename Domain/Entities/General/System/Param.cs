using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.General.System;

/// <summary>
/// Сущность параметров
/// </summary>
[Table("dir_parameters")]
[Comment("Параметры")]
public class Parametr: Directory
{
    /// <summary>
    /// Значение
    /// </summary>
    [Column("value")]
    [Comment("Значение")]
    public string Value { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public Parametr() : base()
    {
        
    }

    /// <summary>
    /// Конструктор сущности параметров
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public Parametr(long id, string user, string name, string value) : base(id, user, name)
    {
        Value = value;
    }

    /// <summary>
    /// Конструктор сущности параметров без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="value"></param>
    public Parametr(string user, string name, string value) : base(user, name)
    {
        Value = value;
    }

    /// <summary>
    /// Метод записи значения
    /// </summary>
    /// <param name="value"></param>
    public void SetValue(string value)
    {
        Value = value;
    }
}
