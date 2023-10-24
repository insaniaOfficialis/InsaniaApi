using Services.Informations.News.GetNewsList;

namespace Tests.InformationsTest.NewsTests;

/// <summary>
/// Тест метода получения списка новостей
/// </summary>
public class GetNewsListTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода получения списка новостей
    /// </summary>
    public GetNewsListTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetNewsList service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null);

        //Проверяем на соответствие результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        GetNewsList service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null);

        //Проверяем на соответствие результат
        Assert.True(result.Items?.Any());
    }

    /// <summary>
    /// Тест на проверку корректности завершения со строкой поиска
    /// </summary>
    [Fact]
    public async void CorrectResultWithSearch()
    {
        //Создаём новый экземпляр сервиса
        GetNewsList service = new(_repository);

        //Получаем результат
        var result = await service.Handler("Запуск");

        //Проверяем на соответствие результат
        Assert.True(result.Items?.Any());
    }
}