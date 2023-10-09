using Domain.Entities.Base;
using Domain.Entities.Sociology;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Politics;

/// <summary>
/// Сущность населения областей
/// </summary>
[Table("re_population_areas")]
[Comment("Население областей")]
public class PopulationArea : Reestr
{
    /// <summary>
    /// Ссылка на область
    /// </summary>
    [Column("area_id")]
    [Comment("Ссылка на область")]
    public long AreaId { get; private set; }

    /// <summary>
    /// Навигационное свойство области
    /// </summary>
    public Area Area { get; private set; }

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
    /// Пустой конструктор сущности населения областей
    /// </summary>
    public PopulationArea() : base()
    {

    }

    /// <summary>
    /// Конструктор сущности населения областей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="area"></param>
    /// <param name="nation"></param>
    /// <param name="quantity"></param>
    /// <param name="gender"></param>
    public PopulationArea(long id, string user, bool isSystem, Area area, Nation nation, int quantity, bool gender) : base(id, user, isSystem)
    {
        Area = area;
        AreaId = area.Id;
        Nation = nation;
        NationId = nation.Id;
        Quantity = quantity;
        Gender = gender;
    }

    /// <summary>
    /// Конструктор сущности населения областей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="area"></param>
    /// <param name="nation"></param>
    /// <param name="quantity"></param>
    /// <param name="gender"></param>
    public PopulationArea(string user, bool isSystem, Area area, Nation nation, int quantity, bool gender) : base(user, isSystem)
    {
        Area = area;
        AreaId = area.Id;
        Nation = nation;
        NationId = nation.Id;
        Quantity = quantity;
        Gender = gender;
    }

    /// <summary>
    /// Метод записи области
    /// </summary>
    /// <param name="area"></param>
    public void SetArea(Area area)
    {
        Area = area;
        AreaId = area.Id;
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
