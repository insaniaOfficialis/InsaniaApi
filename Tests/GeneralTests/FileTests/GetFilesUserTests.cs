using Services;
using Services.General.Files.GetFilesUser;

namespace Tests.GeneralTests.FileTests;

/// <summary>
/// Тесты метода получения файлов пользователя
/// </summary>
public class GetFilesUserTests : BaseTest
{
    /// <summary>
    /// Конструктор тестов метода получения файлов пользователя
    /// </summary>
    public GetFilesUserTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetFilesUser getFilesUser = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesUser.Handler(1);

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
        GetFilesUser getFilesUser = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesUser.Handler(1);

        //Проверяем, что результат успешный
        Assert.Single(result.Items!);
    }

    /// <summary>
    /// Тест на проверку пустого id
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        GetFilesUser getFilesUser = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesUser.Handler(null);

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
        GetFilesUser getFilesUser = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesUser.Handler(0);

        //Проверяем, что результат успешный
        Assert.Equal(Errors.NotExistsFilesUsers, result.Error!.Message);
    }
}