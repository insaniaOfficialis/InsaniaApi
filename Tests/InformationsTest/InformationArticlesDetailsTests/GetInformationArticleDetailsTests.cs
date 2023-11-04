using Services;
using Services.General.Files.GetFilesInformationArticleDetails;
using Services.Informations.InformationArticlesDetails.GetInformationArticleDetails;

namespace Tests.InformationsTest.InformationArticlesDetailsTests;

/// <summary>
/// Тесты метода получения файлов детальной части информационной статьи
/// </summary>
public class GetInformationArticleDetailsTests : BaseTest
{
    GetFilesInformationArticleDetails _getFilesInformationArticleDetails; //сервис получения файлов детальной части информационной статьи

    /// <summary>
    /// Конструктор тестов метода получения файлов детальной части информационной статьи
    /// </summary>
    public GetInformationArticleDetailsTests() : base()
    {
        _getFilesInformationArticleDetails = new(_mapper, _repository);
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetInformationArticleDetails getInformationArticleDetails = new(_repository, _getFilesInformationArticleDetails);

        //Получаем результат
        var result = await getInformationArticleDetails.Handler(6);

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
        GetInformationArticleDetails getInformationArticleDetails = new(_repository, _getFilesInformationArticleDetails);

        //Получаем результат
        var result = await getInformationArticleDetails.Handler(6);

        //Проверяем результат
        Assert.NotNull(result.Items);
    }

    /// <summary>
    /// Тест на проверку пустого id
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        GetInformationArticleDetails getInformationArticleDetails = new(_repository, _getFilesInformationArticleDetails);

        //Получаем результат
        var result = await getInformationArticleDetails.Handler(null);

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
        GetInformationArticleDetails getInformationArticleDetails = new(_repository, _getFilesInformationArticleDetails);

        //Получаем результат
        var result = await getInformationArticleDetails.Handler(0);

        //Проверяем результат
        Assert.Equal(Errors.NotExistsInformationArticle, result.Error!.Message);
    }
}