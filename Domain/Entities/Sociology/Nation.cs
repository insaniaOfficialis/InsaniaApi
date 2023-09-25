using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность нации
/// </summary>
[Table("dir_nations")]
[Comment("Нации")]
public class Nation : Directory
{
    /// <summary>
    /// Ссылка на расу
    /// </summary>
    [Column("race_id")]
    [Comment("Ссылка на расу")]
    public long RaceId { get; private set; }

    /// <summary>
    /// Навигационное свойство расы
    /// </summary>
    public Race Race { get; private set; }

    /// <summary>
    /// Язык для имён
    /// </summary>
    [Column("language_for_personal_names")]
    [Comment("Язык для имён")]
    public string? LanguageForPersonalNames { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности нации
    /// </summary>
    public Nation() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности нации
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="race"></param>
    /// <param name="languageForPersonalNames"></param>
    public Nation(long id, string user, string name, Race race, string? languageForPersonalNames) : base(id, user, name)
    {
        RaceId = race.Id;
        Race = race;
        LanguageForPersonalNames = languageForPersonalNames;
    }

    /// <summary>
    /// Конструктор сущности нации без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="race"></param>
    /// <param name="languageForPersonalNames"></param>
    public Nation(string user, string name, Race race, string? languageForPersonalNames) : base(user, name)
    {
        RaceId = race.Id;
        Race = race;
        LanguageForPersonalNames = languageForPersonalNames;
    }

    /// <summary>
    /// Метод записи расы
    /// </summary>
    /// <param name="race"></param>
    public void SetRace(Race race)
    {
        RaceId = race.Id;
        Race = race;
    }

    /// <summary>
    /// Метод записи языка для имён
    /// </summary>
    /// <param name="languageForPersonalNames"></param>
    public void SetLanguageForPersonalNames(string languageForPersonalNames)
    {
        LanguageForPersonalNames = languageForPersonalNames;
    }
}
