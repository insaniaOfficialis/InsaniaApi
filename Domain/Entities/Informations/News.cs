using Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Informations;

/// <summary>
/// Сущность новостей
/// </summary>
[Table("re_news")]
[Comment("Новости")]
public class News : Reestr
{
    /// <summary>
    /// Залоговок
    /// </summary>
    [Column("title")]
    [Comment("Заголовок")]
    public string Title { get; private set; }

    /// <summary>
    /// Вступление
    /// </summary>
    [Column("introduction")]
    [Comment("Вступление")]
    public string Introduction { get; private set; }

    /// <summary>
    /// Ссылка на тип новости
    /// </summary>
    [Column("type_id")]
    [Comment("Ссылка на тип новости")]
    public long TypeId { get; private set; }

    /// <summary>
    /// Навигационное свойство типа новости
    /// </summary>
    [ForeignKey("TypeId")]
    public NewsType Type { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности новостей
    /// </summary>
    public News()
    {

    }

    /// <summary>
    /// Конструктор сущности новостей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="title"></param>
    /// <param name="introduction"></param>
    /// <param name="type"></param>
    public News(string user, bool isSystem, string title, string introduction, NewsType type) :
        base(user, isSystem)
    {
        Title = title;
        Introduction = introduction;
        Type = type;
        TypeId = type.Id;
    }

    /// <summary>
    /// Конструктор сущности новостей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="isSystem"></param>
    /// <param name="title"></param>
    /// <param name="introduction"></param>
    /// <param name="type"></param>
    public News(long id, string user, bool isSystem, string title, string introduction, NewsType type) : 
        base(id, user, isSystem)
    {
        Title = title;
        Introduction = introduction;
        Type = type;
        TypeId = type.Id;
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
    /// Метод записи вступления
    /// </summary>
    /// <param name="introduction"></param>
    public void SetIntroduction(string introduction)
    {
        Introduction = introduction;
    }

    /// <summary>
    /// Метод записи типа
    /// </summary>
    /// <param name="type"></param>
    public void SetType(NewsType type)
    {
        Type = type;
        TypeId = type.Id;
    }
}
