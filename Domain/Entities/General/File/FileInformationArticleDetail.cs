using Domain.Entities.Informations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.General.File;

/// <summary>
/// Сущности связи файлов с детальными частями информационных статей
/// </summary>
[Table("un_files_information_articles_details")]
[Comment("Связь файлов с детальными частями информационных статей")]
public class FileInformationArticleDetail : BaseEntity
{
    /// <summary>
    /// Файл
    /// </summary>
    [Column("file_id")]
    [Comment("Файл")]
    public long FileId { get; private set; }

    /// <summary>
    /// Навигационное свойство файла
    /// </summary>
    public File? File { get; private set; }

    /// <summary>
    /// Ссылка на детальную часть информационной статьи
    /// </summary>
    [Column("information_article_detail_id")]
    [Comment("Ссылка на детальную часть информационной статьи")]
    public long InformationArticleDetailId { get; private set; }

    /// <summary>
    /// Навигационное свойство детальной части информационной статьи
    /// </summary>
    public InformationArticleDetail InformationArticleDetail { get; private set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    [Column("ordinal_number")]
    [Comment("Порядковый номер")]
    public long OrdinalNumber { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public FileInformationArticleDetail() : base()
    {

    }

    /// <summary>
    /// Конструктор связи файлов с детальными частями информационных статей без id сущности
    /// </summary>
    /// <param name="user"></param>
    /// <param name="file"></param>
    /// <param name="informationArticleDetail"></param>
    /// <param name="ordinalNumber"></param>
    public FileInformationArticleDetail(string? user, File file,
        InformationArticleDetail informationArticleDetail, long ordinalNumber)
        : base(user)
    {
        File = file;
        FileId = file.Id;
        InformationArticleDetail = informationArticleDetail;
        InformationArticleDetailId = informationArticleDetail.Id;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Конструктор связи файлов с детальными частями информационных статей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="file"></param>
    /// <param name="informationArticleDetail"></param>
    /// <param name="ordinalNumber"></param>
    public FileInformationArticleDetail(long id, string user, File file,
        InformationArticleDetail informationArticleDetail, long ordinalNumber) : base(id, user)
    {
        File = file;
        FileId = file.Id;
        InformationArticleDetail = informationArticleDetail;
        InformationArticleDetailId = informationArticleDetail.Id;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Метод записи файла
    /// </summary>
    /// <param name="file"></param>
    public void SetFile(File file)
    {
        FileId = file.Id;
        File = file;
    }

    /// <summary>
    /// Метод записи детальной части информационной статьи
    /// </summary>
    /// <param name="informationArticleDetail"></param>
    public void SetInformationArticleDetail(InformationArticleDetail informationArticleDetail)
    {
        InformationArticleDetail = informationArticleDetail;
        InformationArticleDetailId = informationArticleDetail.Id;
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