using Services;
using Services.General.Files.GetFilesNewsDetails;
using Services.Informations.NewsDetails.GetNewsDetailsFull;

namespace Tests.InformationsTest.NewsDetailsTests;

/// <summary>
/// Тесты получения всех детальных частей новости
/// </summary>
public class GetNewsDetailsFullTests : BaseTest
{
    GetFilesNewsDetails _getFilesNewDetails; //сервис получения файлов детальной части новости

    /// <summary>
    /// Тесты получения всех детальных частей новости
    /// </summary>
    public GetNewsDetailsFullTests() : base()
    {
        _getFilesNewDetails = new(_mapper, _repository);
    }

    /// <summary>
    /// Тест на проверку успешности
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetNewsDetailsFull service = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await service.Handler(1);

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        GetNewsDetailsFull service = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await service.Handler(1);

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
        GetNewsDetailsFull service = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await service.Handler(null);

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
        GetNewsDetailsFull service = new(_repository, _getFilesNewDetails);

        //Получаем результат
        var result = await service.Handler(-1);

        //Проверяем результат
        Assert.Equal(Errors.NotExistsNews, result.Error!.Message);
    }
}