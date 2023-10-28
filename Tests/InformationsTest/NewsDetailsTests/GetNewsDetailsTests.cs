using Services;
using Services.General.Files.GetFilesNewsDetails;
using Services.Informations.NewsDetails.GetNewsDetails;

namespace Tests.InformationsTest.NewsDetailsTests;

/// <summary>
/// Тесты метода получения файлов детальной части новости
/// </summary>
public class GetNewsDetailsTests : BaseTest
{
    GetFilesNewsDetails _getFilesNewDetails; //сервис получения файлов детальной части новости

    /// <summary>
    /// Конструктор тестов метода получения файлов детальной части новости
    /// </summary>
    public GetNewsDetailsTests() : base()
    {
        _getFilesNewDetails = new(_mapper, _repository);
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetNewsDetails getNewsDetails = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await getNewsDetails.Handler(1);

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
        GetNewsDetails getNewsDetails = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await getNewsDetails.Handler(1);

        //Проверяем результат
        Assert.Single(result.Items!);
    }

    /// <summary>
    /// Тест на проверку пустого id
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        GetNewsDetails getNewsDetails = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await getNewsDetails.Handler(null);

        //Проверяем результат
        Assert.Equal(Errors.EmptyRequest, result.Error!.Message);
    }

    /// <summary>
    /// Тест на проверку некорректного id
    /// </summary>
    [Fact]
    public async void IncorrectId()
    {
        //Создаём новый экземпляр сервиса
        GetNewsDetails getNewsDetails = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await getNewsDetails.Handler(0);

        //Проверяем результат
        Assert.Equal(Errors.NotExistsNews, result.Error!.Message);
    }
}