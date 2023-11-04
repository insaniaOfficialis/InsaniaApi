using Domain.Entities.Informations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using BaseEntity = Domain.Entities.Base.Base;

namespace Domain.Entities.General.File;

/// <summary>
/// Сущности связи файлов с детальными частями новостей
/// </summary>
[Table("un_files_news_details")]
[Comment("Связь файлов с детальными частями новостей")]
public class FileNewsDetail : BaseEntity
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
    /// Ссылка на детальную часть новости
    /// </summary>
    [Column("news_detail_id")]
    [Comment("Ссылка на детальную часть новости")]
    public long NewsDetailId { get; private set; }

    /// <summary>
    /// Навигационное свойство детальной части новости
    /// </summary>
    public NewsDetail NewsDetail { get; private set; }

    /// <summary>
    /// Порядковый номер
    /// </summary>
    [Column("ordinal_number")]
    [Comment("Порядковый номер")]
    public long OrdinalNumber { get; private set; }

    /// <summary>
    /// Пустой конструктор
    /// </summary>
    public FileNewsDetail() : base()
    {

    }

    /// <summary>
    /// Конструктор связи файлов с детальными частями новостей без id сущности
    /// </summary>
    /// <param name="user"></param>
    /// <param name="file"></param>
    /// <param name="newsDetail"></param>
    /// <param name="ordinalNumber"></param>
    public FileNewsDetail(string? user, File file, NewsDetail newsDetail, long ordinalNumber) : base(user)
    {
        File = file;
        FileId = file.Id;
        NewsDetail = newsDetail;
        NewsDetailId = newsDetail.Id;
        OrdinalNumber = ordinalNumber;
    }

    /// <summary>
    /// Конструктор связи файлов с детальными частями новостей
    /// </summary>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <param name="file"></param>
    /// <param name="newsDetail"></param>
    /// <param name="ordinalNumber"></param>
    public FileNewsDetail(long id, string user, File file, NewsDetail newsDetail, long ordinalNumber) : base(id, user)
    {
        File = file;
        FileId = file.Id;
        NewsDetail = newsDetail;
        NewsDetailId = newsDetail.Id;
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
    /// Метод записи детальной части новости
    /// </summary>
    /// <param name="newsDetail"></param>
    public void SetNewsDetail(NewsDetail newsDetail)
    {
        NewsDetail = newsDetail;
        NewsDetailId = newsDetail.Id;
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