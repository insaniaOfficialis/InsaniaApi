using Services.Informations.InformationArticles.GetInformationArticles;

namespace Tests.InformationsTest.InformationArticlesTests;

/// <summary>
/// Тест метода получения списка информационных статей
/// </summary>
public class GetListInformationArticlesTests : BaseTest
{

    /// <summary>
    /// Конструктор теста метода получения списка информационных статей
    /// </summary>
    public GetListInformationArticlesTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetListInformationArticles service = new(_mapper, _repository);

        //Получаем результат
        var result = await service.Handler(null);

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
        GetListInformationArticles service = new(_mapper, _repository);

        //Получаем результат
        var result = await service.Handler(null);

        //Проверяем, что результат успешный
        Assert.True(result.Items?.Any());
    }

    /// <summary>
    /// Тест на проверку корректности завершения со строкой поиска
    /// </summary>
    [Fact]
    public async void CorrectResultWithSearch()
    {
        //Создаём новый экземпляр сервиса
        GetListInformationArticles service = new(_mapper, _repository);

        //Получаем результат
        var result = await service.Handler("Тест");

        //Проверяем, что результат успешный
        Assert.True(result.Items?.Any());
    }
}