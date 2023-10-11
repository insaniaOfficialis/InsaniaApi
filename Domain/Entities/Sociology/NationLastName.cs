using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность связи нации с фамилиями
/// </summary>
[Table("un_nations_last_names")]
[Comment("Связь наций с фамилиями")]
public class NationLastName : BaseEntity
{
    /// <summary>
    /// Вероятность выпадения
    /// </summary>
    [Column("probability")]
    [Comment("Вероятность выпадения")]
    public double Probability { get; private set; }

    /// <summary>
    /// Ссылка на нацию
    /// </summary>
    [Column("nation_id")]
    [Comment("Ссылка на нацию")]
    public long NationId { get; private set; }

    /// <summary>
    /// Навигационное свойство нации
    /// </summary>
    public Nation Nation { get; private set; }

    /// <summary>
    /// Ссылка на фамилию
    /// </summary>
    [Column("last_name_id")]
    [Comment("Ссылка на фамилию")]
    public long LastNameId { get; private set; }

    /// <summary>
    /// Навигационное свойство фамилии
    /// </summary>
    public LastName LastName { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности связи нации с фамилиями
    /// </summary>
    public NationLastName() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности связи нации с фамилиями
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="probability"></param>
    /// <param name="nation"></param>
    /// <param name="lastName"></param>
    public NationLastName(long id, string user, double probability, Nation nation, LastName lastName) : base(id, user)
    {
        Probability = probability;
        Nation = nation;
        NationId = nation.Id;
        LastName = lastName;
        LastNameId = lastName.Id;
    }

    /// <summary>
    /// Конструктор сущности связи нации с фамилиями без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="probability"></param>
    /// <param name="nation"></param>
    /// <param name="lastName"></param>
    public NationLastName(string user, double probability, Nation nation, LastName lastName) : base(user)
    {
        Probability = probability;
        Nation = nation;
        NationId = nation.Id;
        LastName = lastName;
        LastNameId = lastName.Id;
    }

    /// <summary>
    /// Метод записи вероятности выпадения фамилии
    /// </summary>
    /// <param name="probability"></param>
    public void SetProbability(double probability)
    {
        Probability = probability;
    }

    /// <summary>
    /// Метод записи нации
    /// </summary>
    /// <param name="nation"></param>
    public void SetNation(Nation nation)
    {
        Nation = nation;
        NationId = nation.Id;
    }

    /// <summary>
    /// Метод записи фамилии
    /// </summary>
    /// <param name="lastName"></param>
    public void SetLastName(LastName lastName)
    {
        LastName = lastName;
        LastNameId = lastName.Id;
    }
}
