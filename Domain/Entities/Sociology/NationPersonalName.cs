using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность связи нации с именами
/// </summary>
[Table("un_nations_personal_names")]
[Comment("Связь наций с именами")]
public class NationPersonalName : BaseEntity
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
    /// Ссылка на имя
    /// </summary>
    [Column("personal_name_id")]
    [Comment("Ссылка на нацию")]
    public long PersonalNameId { get; private set; }

    /// <summary>
    /// Навигационное свойство имени
    /// </summary>
    public PersonalName PersonalName { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности связи нации с именами
    /// </summary>
    public NationPersonalName() : base()
    { 
    }

    /// <summary>
    /// Конструктор сущности связи нации с именами
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="probability"></param>
    /// <param name="nation"></param>
    /// <param name="personalName"></param>
    public NationPersonalName(long id, string user, double probability, Nation nation, PersonalName personalName) : base(id, user)
    {
        Probability = probability;
        Nation = nation;
        NationId = nation.Id;
        PersonalName = personalName;
        PersonalNameId = personalName.Id;
    }

    /// <summary>
    /// Конструктор сущности связи нации с именами без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="probability"></param>
    /// <param name="nation"></param>
    /// <param name="personalName"></param>
    public NationPersonalName(string user, double probability, Nation nation, PersonalName personalName) : base(user)
    {
        Probability = probability;
        Nation = nation;
        NationId = nation.Id;
        PersonalName = personalName;
        PersonalNameId = personalName.Id;
    }

    /// <summary>
    /// Метод записи вероятности выпадения имени
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
    /// Метод записи имени
    /// </summary>
    /// <param name="personalName"></param>
    public void SetPersonalName(PersonalName personalName)
    {
        PersonalName = personalName;
        PersonalNameId = personalName.Id;
    }
}
