using Services;
using Services.General.Files.GetFile;

namespace Tests.GeneralTests.FileTests;

/// <summary>
/// Тесты метода получения файлов
/// </summary>
public class GetFileTests : BaseTest
{
    /// <summary>
    /// Конструктор тестов метода получения файлов
    /// </summary>
    public GetFileTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetFile getFile = new(_repository);

        //Получаем результат
        var result = await getFile.Handler(1, 1);

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        GetFile getFile = new(_repository);

        //Получаем результат
        var result = await getFile.Handler(1, 1);

        //Проверяем результат
        Assert.Equal("demiurge.png", result.Name);
        Assert.Equal("I:\\Insania\\ПО\\Files\\Users\\1\\demiurge.png", result.Path);
        Assert.Equal("image/png", result.ContentType);
    }

    /// <summary>
    /// Тест на проверку пустого id
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        GetFile getFile = new(_repository);

        //Получаем результат
        var result = await getFile.Handler(null, 1);

        //Проверяем результат
        Assert.Equal(Errors.EmptyRequest, result.Error!.Message);
    }

    /// <summary>
    /// Тест на проверку пустого id сущности
    /// </summary>
    [Fact]
    public async void EmptyEntityId()
    {
        //Создаём новый экземпляр сервиса
        GetFile getFile = new(_repository);

        //Получаем результат
        var result = await getFile.Handler(1, null);

        //Проверяем результат
        Assert.Equal(Errors.EmptyEntityId, result.Error!.Message);
    }

    /// <summary>
    /// Тест на проверку некорректного id
    /// </summary>
    [Fact]
    public async void IncorrectId()
    {
        //Создаём новый экземпляр сервиса
        GetFile getFile = new(_repository);

        //Получаем результат
        var result = await getFile.Handler(0, 1);

        //Проверяем результат
        Assert.Equal(Errors.NotExistsFile, result.Error!.Message);
    }
}
