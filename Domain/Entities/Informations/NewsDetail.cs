using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Informations;

/// <summary>
/// Сущность детальной части новостей
/// </summary>
[Table("re_news_details")]
[Comment("Детальная часть новостей")]
public class NewsDetail : Reestr
{
    /// <summary>
    /// Текст
    /// </summary>
    [Column("text")]
    [Comment("Текст")]
    public string Text { get; private set; }

    /// <summary>
    /// Ссылка на новость
    /// </summary>
    [Column("news_id")]
    [Comment("Ссылка на новость")]
    public long NewsId { get; private set; }

    /// <summary>
    /// Навигационное свойство новости
    /// </summary>
    public News News { get; private set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    [Column("ordinal_number")]
    [Comment("Порядковый номер")]
    public long OrdinalNumber { get; private set; }

    /// <summary>
    /// Пустой конструктор детальной части новостей
    /// </summary>
    public NewsDetail() : base()
    {

    }

    /// <summary>
    /// Конструктор детальной части новостей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="text"></param>
    /// <param name="news"></param>
    /// <param name="ordinalNumber"></param>
    public NewsDetail(string? user, bool isSystem, string text, News news, long ordinalNumber) : base(user, isSystem)
    {
        Text = text;
        News = news;
        NewsId = news.Id;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Конструктор детальной части новостей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="text"></param>
    /// <param name="news"></param>
    /// <param name="ordinalNumber"></param>
    public NewsDetail(long id, string? user, bool isSystem, string text, News news, long ordinalNumber) : base(id, user, isSystem)
    {
        Text = text;
        News = news;
        NewsId = news.Id;
        OrdinalNumber = ordinalNumber;
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
    /// Метод записи новости
    /// </summary>
    /// <param name="news"></param>
    public void SetNews(News news)
    {
        News = news;
        NewsId = news.Id;
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