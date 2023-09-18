namespace Domain.Models.Files.Request;

/// <summary>
/// Модель запроса на добавление файла
/// </summary>
public class AddFileRequest
{
    /// <summary>
    /// Id сущности, на которую загружаются данные
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// Наименование файла
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Id типа файла
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Поток файла
    /// </summary>
    public Stream? Stream { get; set; }

    /// <summary>
    /// Конструктор модели запроса на добавление файла
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="type"></param>
    /// <param name="stream"></param>
    public AddFileRequest(long? id, string? name, string? type, Stream? stream)
    {
        Id = id;
        Name = name;
        Type = type;
        Stream = stream;
    }
}
