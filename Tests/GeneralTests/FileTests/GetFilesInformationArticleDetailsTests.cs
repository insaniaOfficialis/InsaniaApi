using Services;
using Services.General.Files.GetFilesInformationArticleDetails;

namespace Tests.GeneralTests.FileTests;

/// <summary>
/// Тесты метода получения файлов детальной части информационной статьи
/// </summary>
public class GetFilesInformationArticleDetailsTests : BaseTest
{
    /// <summary>
    /// Конструктор тестов метода получения файлов детальной части информационной статьи
    /// </summary>
    public GetFilesInformationArticleDetailsTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetFilesInformationArticleDetails getFilesInformationArticleDetails = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesInformationArticleDetails.Handler(1);

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
        GetFilesInformationArticleDetails getFilesInformationArticleDetails = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesInformationArticleDetails.Handler(1);

        //Проверяем, что результат успешный
        Assert.NotNull(result.Items);
    }

    /// <summary>
    /// Тест на проверку пустого id
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        GetFilesInformationArticleDetails getFilesInformationArticleDetails = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesInformationArticleDetails.Handler(null);

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
        GetFilesInformationArticleDetails getFilesInformationArticleDetails = new(_mapper, _repository);

        //Получаем результат
        var result = await getFilesInformationArticleDetails.Handler(0);

        //Проверяем, что результат успешный
        Assert.Equal(Errors.NotExistsInformationArticleDetail, result.Error!.Message);
    }
}