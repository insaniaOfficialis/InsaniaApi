using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Informations;

/// <summary>
/// Сущность типов новостей
/// </summary>
[Table("dir_news_types")]
[Comment("Типы новостей")]
public class NewsType : Directory
{
    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public NewsType()
    {

    }

    /// <summary>
    /// Конструктор сущности типов новостей без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public NewsType(string user, string name) : base(user, name)
    {

    }

    /// <summary>
    /// Конструктор сущности типов новостей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    public NewsType(long id, string user, string name) : base(id, user, name)
    {

    }
}
