using Services;
using Services.Politics.Regions.CheckingRegionsColors;

namespace Tests.PoliticsTests.RegionsTests;

/// <summary>
/// Тесты проверки цвета регионов
/// </summary>
public class CheckingRegionsColorsTests : BaseTest
{
    /// <summary>
    /// Тесты проверки цвета регионов
    /// </summary>
    public CheckingRegionsColorsTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        CheckingRegionsColors service = new(_repository);

        //Получаем результат
        var result = await service.Handler("#000000");

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку пустого запроса
    /// </summary>
    [Fact]
    public async void EmptyRequest()
    {
        //Создаём новый экземпляр сервиса
        CheckingRegionsColors service = new(_repository);

        //Получаем результат
        var result = await service.Handler(null);

        //Проверяем результат
        Assert.Equal(Errors.EmptyRequest, result.Error?.Message);
    }

    /// <summary>
    /// Тест на проверку занятого цвета
    /// </summary>
    [Fact]
    public async void BusyColor()
    {
        //Создаём новый экземпляр сервиса
        CheckingRegionsColors service = new(_repository);

        //Получаем результат
        var result = await service.Handler("#0004FF");

        //Проверяем результат
        Assert.Equal(Errors.BusyColor, result.Error?.Message);
    }
}