using Domain.Entities.Base;
using Domain.Entities.Sociology;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Politics;

/// <summary>
/// Сущность населения населённых пунктов
/// </summary>
[Table("re_population_settlements")]
[Comment("Население населённых пунктов")]
public class PopulationSettlement : Reestr
{
    /// <summary>
    /// Ссылка на населённый пуцнкт
    /// </summary>
    [Column("settlement_id")]
    [Comment("Ссылка на населённый пуцнкт")]
    public long SettlementId { get; private set; }

    /// <summary>
    /// Навигационное свойство населённого пункта
    /// </summary>
    public Settlement Settlement { get; private set; }

    /// <summary>
    /// Ссылка на нацию
    /// </summary>
    [Column("nation_id")]
    [Comment("Ссылка на область")]
    public long NationId { get; private set; }

    /// <summary>
    /// Навигационное свойство нации
    /// </summary>
    public Nation Nation { get; private set; }

    /// <summary>
    /// Количество разумных
    /// </summary>
    [Column("quantity")]
    [Comment("Количество разумных")]
    public int Quantity { get; private set; }

    /// <summary>
    /// Пол (истина - мужской/ложь - женский)
    /// </summary>
    [Column("gender")]
    [Comment("Пол (истина - мужской/ложь - женский)")]
    public bool Gender { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности населения населённых пунктов
    /// </summary>
    public PopulationSettlement() : base()
    {

    }

    /// <summary>
    /// Конструктор сущности населения населённых пунктов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="settlement"></param>
    /// <param name="nation"></param>
    /// <param name="quantity"></param>
    /// <param name="gender"></param>
    public PopulationSettlement(long id, string user, bool isSystem, Settlement settlement, Nation nation, int quantity, bool gender) : base(id, user, isSystem)
    {
        Settlement = settlement;
        SettlementId = settlement.Id;
        Nation = nation;
        NationId = nation.Id;
        Quantity = quantity;
        Gender = gender;
    }

    /// <summary>
    /// Конструктор сущности населения населённых пунктов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="settlement"></param>
    /// <param name="nation"></param>
    /// <param name="quantity"></param>
    /// <param name="gender"></param>
    public PopulationSettlement(string user, bool isSystem, Settlement settlement, Nation nation, int quantity, bool gender) : base(user, isSystem)
    {
        Settlement = settlement;
        SettlementId = settlement.Id;
        Nation = nation;
        NationId = nation.Id;
        Quantity = quantity;
        Gender = gender;
    }

    /// <summary>
    /// Метод записи населённого пункта
    /// </summary>
    /// <param name="settlement"></param>
    public void SetSettlement(Settlement settlement)
    {
        Settlement = settlement;
        SettlementId = settlement.Id;
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
    /// Метод записи количества
    /// </summary>
    /// <param name="quantity"></param>
    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
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
