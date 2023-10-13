using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

public class GetListEndingsNames : BaseTest
{
    Mock<ILogger<PersonalNames>> _mockLogger; //зашитый логгер

    /// <summary>
    /// Конструктор теста получения успешного кода
    /// </summary>
    public GetListEndingsNames() : base()
    {
        _mockLogger = new Mock<ILogger<PersonalNames>>();
    }

    /// <summary>
    /// Тест на успешность завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.GetListEndingsNames(1, true);

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на корректный результат
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.GetListEndingsNames(1, true);

        //Проверяем, что результат успешный
        Assert.NotNull(result.Items);
    }
}