using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.Sociology;

/// <summary>
/// Сущность связи нации с префиксами имён
/// </summary>
[Table("un_nations_prefix_names")]
[Comment("Связь наций с префиксами имён")]
public class NationPrefixName : BaseEntity
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
    /// Ссылка на префикс имени
    /// </summary>
    [Column("prefix_name_id")]
    [Comment("Ссылка на префикс имени")]
    public long PrefixNameId { get; private set; }

    /// <summary>
    /// Навигационное свойство префикса имени
    /// </summary>
    public PrefixName PrefixName { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности связи нации с префиксами имён
    /// </summary>
    public NationPrefixName() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности связи нации с префиксами имён
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="probability"></param>
    /// <param name="nation"></param>
    /// <param name="prefixName"></param>
    public NationPrefixName(long id, string user, double probability, Nation nation, PrefixName prefixName) : base(id, user)
    {
        Probability = probability;
        Nation = nation;
        NationId = nation.Id;
        PrefixName = prefixName;
        PrefixNameId = prefixName.Id;
    }

    /// <summary>
    /// Конструктор сущности связи нации с префиксами имён без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="probability"></param>
    /// <param name="nation"></param>
    /// <param name="prefixName"></param>
    public NationPrefixName(string user, double probability, Nation nation, PrefixName prefixName) : base(user)
    {
        Probability = probability;
        Nation = nation;
        NationId = nation.Id;
        PrefixName = prefixName;
        PrefixNameId = prefixName.Id;
    }

    /// <summary>
    /// Метод записи вероятности выпадения префикса имени
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
    /// Метод записи префикса имени
    /// </summary>
    /// <param name="prefixName"></param>
    public void SetPrefixName(PrefixName prefixName)
    {
        PrefixName = prefixName;
        PrefixNameId = prefixName.Id;
    }
}
