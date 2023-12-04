using Services.Informations.NewsTypes.GetNewsTypesList;

namespace Tests.InformationsTest.NewsTypesTests;

/// <summary>
/// Тесты получения списка типов новостей
/// </summary>
public class GetNewsTypesListTests : BaseTest
{
    /// <summary>
    /// Конструктор тестов получения списка типов новостей
    /// </summary>
    public GetNewsTypesListTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetNewsTypesList service = new(_repository, _mapper);

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
        GetNewsTypesList service = new(_repository, _mapper);

        //Получаем результат
        var result = await service.Handler();

        //Проверяем результат
        Assert.True(result.Items?.Any());
    }
}