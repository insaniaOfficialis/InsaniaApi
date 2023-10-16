using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Informations;

/// <summary>
/// Сущность детальной части информационных статей
/// </summary>
[Table("re_information_articles_details")]
[Comment("Детальная часть информационных статей")]
public class InformationArticleDetail : Reestr
{
    /// <summary>
    /// Текст
    /// </summary>
    [Column("text")]
    [Comment("Текст")]
    public string Text { get; private set; }

    /// <summary>
    /// Ссылка на информационную статью
    /// </summary>
    [Column("information_article_id")]
    [Comment("Ссылка на информационную статью")]
    public long InformationArticleId { get; private set; }

    /// <summary>
    /// Навигационное свойство информационной статьи
    /// </summary>
    public InformationArticle InformationArticle { get; private set; }

    /// <summary>
    /// Пустой конструктор детальной части информационных статей
    /// </summary>
    public InformationArticleDetail() : base()
    {

    }

    /// <summary>
    /// Конструктор детальной части информационных статей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="text"></param>
    /// <param name="informationArticle"></param>
    public InformationArticleDetail(string? user, bool isSystem, string text,
        InformationArticle informationArticle) : base(user, isSystem)
    {
        Text = text;
        InformationArticle = informationArticle;
        InformationArticleId = informationArticle.Id;
    }

    /// <summary>
    /// Конструктор детальной части информационных статей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="text"></param>
    /// <param name="informationArticle"></param>
    public InformationArticleDetail(long id, string? user, bool isSystem, string text,
        InformationArticle informationArticle) : base(id, user, isSystem)
    {
        Text = text;
        InformationArticle = informationArticle;
        InformationArticleId = informationArticle.Id;
    }

    /// <summary>
    /// Метод записи текста
    /// </summary>
    /// <param name="text"></param>
    public void SetText(string text)
    {
        Text = text;
    }

    /// <summary>
    /// Метод записи информационных статей
    /// </summary>
    /// <param name="informationArticle"></param>
    public void SetInformationArticle(InformationArticle informationArticle)
    {
        InformationArticle = informationArticle;
        InformationArticleId = informationArticle.Id;
    }
}
