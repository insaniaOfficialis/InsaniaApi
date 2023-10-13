using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

/// <summary>
/// Тест генерации имени
/// </summary>
public class GetGeneratedName : BaseTest
{
    /// <summary>
    /// Конструктор теста генрации имени
    /// </summary>
    public GetGeneratedName() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём мзашитый логгер
        var _mockLogger = new Mock<ILogger<PersonalNames>>();

        //Создаём новый экземпляр сервиса личных имён
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат сгенерированного имени
        var result = await personalNames.GetGeneratedName(1, true, false);

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }
}