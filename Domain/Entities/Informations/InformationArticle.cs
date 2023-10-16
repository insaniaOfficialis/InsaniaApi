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
    public InformationArticle(string? user, bool isSystem, string title) : base(user, isSystem)
    {
        Title = title;
    }

    /// <summary>
    /// Конструктор информационных статей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="title"></param>
    public InformationArticle(long id, string? user, bool isSystem, string title) : base(id, user, isSystem)
    {
        Title = title;
    }

    /// <summary>
    /// Метод записи заголовка
    /// </summary>
    /// <param name="title"></param>
    public void SetTitle(string title)
    {
        Title = title;
    }
}
