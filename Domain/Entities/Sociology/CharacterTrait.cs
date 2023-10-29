using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность черт характера
/// </summary>
[Table("dir_character_traits")]
[Comment("Черты характера")]
public class CharacterTrait : Directory
{
    /// <summary>
    /// Описание
    /// </summary>
    [Column("description")]
    [Comment("Описание")]
    public string Description { get; private set; }

    /// <summary>
    /// Положительность (истина - позитивный/пустой - нецтральный/ложь - отрицательный)
    /// </summary>
    [Column("positivity")]
    [Comment("Положительность (истина - позитивный/пустой - нецтральный/ложь - отрицательный)")]
    public bool? Positivity { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности черт характера
    /// </summary>
    public CharacterTrait() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности черт характера
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="positivity"></param>
    public CharacterTrait(long id, string user, string name, string description, bool? positivity) : base(id, user, name)
    {
        Description = description;
        Positivity = positivity;
    }

    /// <summary>
    /// Конструктор сущности черт характера без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <param name="positivity"></param>
    public CharacterTrait(string user, string name, string description, bool? positivity) : base(user, name)
    {
        Description = description;
        Positivity = positivity;
    }

    /// <summary>
    /// Метод записи описания
    /// </summary>
    /// <param name="description"></param>
    public void SetDescription(string description)
    {
        Description = description;
    }

    /// <summary>
    /// Метод записи положительности
    /// </summary>
    /// <param name="positivity"></param>
    public void SetPositivity(bool? positivity)
    {
        Positivity = positivity;
    }
}