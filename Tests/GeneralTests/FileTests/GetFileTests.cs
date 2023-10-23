using Microsoft.Extensions.Logging;
using Services;
using Services.General.Files.GetFile;
using Services.General.Logs.GetLogs;

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
        var result = await getFile.Handler(6);

        //Проверяем, что результат успешный
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
        var result = await getFile.Handler(6);

        //Проверяем, что результат успешный
        Assert.Equal("MyProfile.jpg", result.Name);
        Assert.Equal("I:\\Insania\\ПО\\Files\\Users\\6\\MyProfile.jpg", result.Path);
        Assert.Equal("image/jpeg", result.ContentType);
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
        var result = await getFile.Handler(null);

        //Проверяем, что результат успешный
        Assert.Equal(Errors.EmptyRequest, result.Error!.Message);
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
        var result = await getFile.Handler(0);

        //Проверяем, что результат успешный
        Assert.Equal(Errors.NotExistsFile, result.Error!.Message);
    }
}
