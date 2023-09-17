using Domain.Methods.Transliteration;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Base;

/// <summary>
/// Модель сущности справочника
/// </summary>
public abstract class Directory : Base
{
    /// <summary>
    /// Наименование
    /// </summary>
    [Column("name")]
    [Comment("Наименование")]
    public string Name { get; private set; }

    /// <summary>
    /// Английское наименование
    /// </summary>
    [Column("alias")]
    [Comment("Английское наименование")]
    public string Alias { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public Directory()
    {
        
    }

    /// <summary>
    /// Конструктор модели сущности справочника без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Directory(string user, string name) :
        base(user)
    {
        Name = name;
        Transliteration transliteration = new();
        Alias = transliteration.Translit(name);
    }

    /// <summary>
    /// Конструктор модели сущности справочника
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public Directory(long id, string user, string name):
        base(id, user)
    {
        Name = name;
        Transliteration transliteration = new();
        Alias = transliteration.Translit(name);
    }

    /// <summary>
    /// Метод записи наименования
    /// </summary>
    /// <param name="name"></param>
    public void SetName(string name)
    {
        Name = name;
        Transliteration transliteration = new();
        Alias = transliteration.Translit(name);
    }
}
