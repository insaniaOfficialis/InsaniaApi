using Services.Informations.News.GetNewsTable;

namespace Tests.InformationsTest.NewsTests;

/// <summary>
/// Тест сервиса получения списка новостей для таблицы
/// </summary>
public class GetNewsTableTests : BaseTest
{
    /// <summary>
    /// Конструктор теста сервиса списка новостей для таблицы
    /// </summary>
    public GetNewsTableTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null, 0, 20, null, null);

        //Проверяем корректность результата
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null, 0, 20, null, null);

        //Проверяем корректность результата
        Assert.True(result.Items?.Any());
    }

    /// <summary>
    /// Тест на проверку успешности завершения без параметров
    /// </summary>
    [Fact]
    public async void SuccessWithountAll()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null, null, null, null, null);

        //Проверяем корректность результата
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности завершения со всеми входящими параметрами
    /// </summary>
    [Fact]
    public async void CorrectResultWithAll()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler("Запуск", 0, 20, new() { new() { SortKey = "Id", IsAscending = false } }, false);

        //Проверяем корректность результата
        Assert.True(result.Items?.Any());
    }

    /// <summary>
    /// Тест на проверку корректности завершения со строкой поиска
    /// </summary>
    [Fact]
    public async void CorrectResultWithSearch()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler("Запуск", 0, 20, null, null);

        //Проверяем корректность результата
        Assert.True(result.Items?.Any());
    }

    /// <summary>
    /// Тест на проверку корректности завершения с сортировкой
    /// </summary>
    [Fact]
    public async void CorrectResultWithSort()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null, 0, 20, new() { new() { SortKey = "Id", IsAscending = false } }, null);

        //Проверяем корректность результата
        Assert.True(result.Items?.Any());
    }

    /// <summary>
    /// Тест на проверку корректности завершения с признаком удаления
    /// </summary>
    [Fact]
    public async void CorrectResultWithIsDeleted()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTable service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null, 0, 20, null, false);

        //Проверяем корректность результата
        Assert.True(result.Items?.Any());
    }
}