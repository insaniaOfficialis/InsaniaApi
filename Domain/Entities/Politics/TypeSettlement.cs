using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Directory = Domain.Entities.Base.Directory;

namespace Domain.Entities.Politics;

/// <summary>
/// Типы населённых пунктов
/// </summary>
[Table("dir_types_settlements")]
[Comment("Типы населённых пунктов")]
public class TypeSettlement : Directory
{
    /// <summary>
    /// Минимальный размер в пикселях
    /// </summary>
    [Column("minimum_size_in_pixels")]
    [Comment("Минимальный размер в пикселях")]
    public int MinimumSizeInPixels { get; private set; }

    /// <summary>
    /// Максимальный размер в пикселях
    /// </summary>
    [Column("maximum_size_in_pixels")]
    [Comment("Максимальный размер в пикселях")]
    public int MaximumSizeInPixels { get; private set; }

    /// <summary>
    /// Пустой конструктор сущности типов населённых пунктов
    /// </summary>
    public TypeSettlement() : base()
    {
    }

    /// <summary>
    /// Конструктор сущности типов населённых пунктов
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="minimumSizeInPixels"></param>
    /// <param name="maximumSizeInPixels"></param>
    public TypeSettlement(long id, string user, string name, int minimumSizeInPixels, int maximumSizeInPixels) : base(id, user, name)
    {
        MinimumSizeInPixels = minimumSizeInPixels;
        MaximumSizeInPixels = maximumSizeInPixels;
    }

    /// <summary>
    /// Конструктор сущности типов населённых пунктов без id
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <param name="minimumSizeInPixels"></param>
    /// <param name="maximumSizeInPixels"></param>
    public TypeSettlement(string user, string name, int minimumSizeInPixels, int maximumSizeInPixels) : base(user, name)
    {
        MinimumSizeInPixels = minimumSizeInPixels;
        MaximumSizeInPixels = maximumSizeInPixels;
    }

    /// <summary>
    /// Метод записи минимального размера в пикселях
    /// </summary>
    /// <param name="minimumSizeInPixels"></param>
    public void SetMinimumSizeInPixels(int minimumSizeInPixels)
    {
        MinimumSizeInPixels = minimumSizeInPixels;
    }

    /// <summary>
    /// Метод записи максимального размера в пикселях
    /// </summary>
    /// <param name="maximumSizeInPixels"></param>
    public void SetMaximumSizeInPixels(int maximumSizeInPixels)
    {
        MaximumSizeInPixels = maximumSizeInPixels;
    }
}
