using Domain.Models.Base;

namespace Domain.Models.General.Files.Response;

/// <summary>
/// Модель ответа получения файла
/// </summary>
public class GetFileReponse : BaseResponse
{
    /// <summary>
    /// Путь к файлу
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Наименование файла
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Тип контента файла
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Конструктор модели ответа получения файла
    /// </summary>
    /// <param name="success"></param>
    /// <param name="path"></param>
    /// <param name="name"></param>
    /// <param name="contentType"></param>
    public GetFileReponse(bool success, string path, string name, string contentType) : base(success)
    {
        Path = path;
        Name = name;
        ContentType = contentType;
    }

    /// <summary>
    /// Конструктор модели ответа получения файла с ошибкой
    /// </summary>
    /// <param name="success"></param>
    /// <param name="error"></param>
    public GetFileReponse(bool success, BaseError? error) : base(success, error)
    {
    }
}