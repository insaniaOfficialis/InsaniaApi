using Services.Politics.Regions.GenerationRegionColor;

namespace Tests.PoliticsTests.RegionsTests;

/// <summary>
/// Тесты проверки генерации цвета регионов
/// </summary>
public class GenreationRegionColorTests : BaseTest
{
    /// <summary>
    /// Тесты проверки генерации цвета регионов
    /// </summary>
    public GenreationRegionColorTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GenerationRegionColor service = new(_repository);

        //Получаем результат
        var result = await service.Handler();

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности
    /// </summary>
    [Fact]
    public async void Correct()
    {
        //Создаём новый экземпляр сервиса
        GenerationRegionColor service = new(_repository);

        //Получаем результат
        var result = await service.Handler();

        //Проверяем результат
        Assert.True(!string.IsNullOrEmpty(result.Value?.ToString()));
    }
}