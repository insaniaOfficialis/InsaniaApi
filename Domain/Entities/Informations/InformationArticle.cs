using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Informations;

/// <summary>
/// Сущность информационных статей
/// </summary>
[Table("re_information_articles")]
[Comment("Информационные статьи")]
public class InformationArticle : Reestr
{
    /// <summary>
    /// Заголовок
    /// </summary>
    [Column("title")]
    [Comment("Заголовок")]
    public string Title { get; private set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    [Column("ordinal_number")]
    [Comment("Порядковый номер")]
    public long OrdinalNumber { get; private set; }

    /// <summary>
    /// Пустой конструктор информационных статей
    /// </summary>
    public InformationArticle() : base()
    {

    }

    /// <summary>
    /// Конструктор информационных статей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="title"></param>
    /// <param name="ordinalNumber"></param>
    public InformationArticle(string? user, bool isSystem, string title, long ordinalNumber) : base(user, isSystem)
    {
        Title = title;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Конструктор информационных статей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="title"></param>
    /// <param name="ordinalNumber"></param>
    public InformationArticle(long id, string? user, bool isSystem, string title, long ordinalNumber) : base(id, user, isSystem)
    {
        Title = title;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Метод записи заголовка
    /// </summary>
    /// <param name="title"></param>
    public void SetTitle(string title)
    {
        Title = title;
    }

    /// <summary>
    /// Метод записи порядкового номера
    /// </summary>
    /// <param name="ordinalNumber"></param>
    public void SetOrdinalNumber(long ordinalNumber)
    {
        OrdinalNumber = ordinalNumber;
    }
}