using Services;
using Services.General.Files.ManagingFileDeletion;

namespace Tests.GeneralTests.FileTests;

/// <summary>
/// Тест метода управления удалением файла
/// </summary>
public class ManagingFileDeletionTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода управления удалением файла
    /// </summary>
    public ManagingFileDeletionTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность удаления файла
    /// </summary>
    [Fact]
    public async void SuccessRemove()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем максимальный id
        long id = _repository.Files.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, true);

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку успешность восстановления файла
    /// </summary>
    [Fact]
    public async void SuccessRecover()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем максимальный id
        long id = _repository.Files.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, false);

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности удаления файла
    /// </summary>
    [Fact]
    public async void CorrectRemove()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем максимальный id
        long id = _repository.Files.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, true);

        //Получаем сущность
        var entity = _repository.Files.First(x => x.Id == id);

        //Проверяем результат
        Assert.NotNull(entity.DateDeleted);
    }

    /// <summary>
    /// Тест на проверку корректности восстановления файла
    /// </summary>
    [Fact]
    public async void CorrectRecover()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем максимальный id
        long id = _repository.Files.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, false);

        //Получаем сущность
        var entity = _repository.Files.First(x => x.Id == id);

        //Проверяем результат
        Assert.Null(entity.DateDeleted);
    }

    /// <summary>
    /// Тест на проверку пустого пользователя
    /// </summary>
    [Fact]
    public async void EmptyUser()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем максимальный id
        long id = _repository.Files.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler(null, id, false);

        //Проверяем результат
        Assert.Equal(Errors.EmptyCurrentUser, result.Error?.Message);
    }

    /// <summary>
    /// Тест на проверку пустой ссылки сущности
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", null, false);

        //Проверяем результат
        Assert.Equal(Errors.EmptyId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на проверку отсутствия файла
    /// </summary>
    [Fact]
    public async void NotExistsFileId()
    {
        //Создаём новый экземпляр сервиса
        ManagingFileDeletion service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", -1, false);

        //Проверяем результат
        Assert.Equal(Errors.NotExistsFile, result.Error?.Message);
    }
}