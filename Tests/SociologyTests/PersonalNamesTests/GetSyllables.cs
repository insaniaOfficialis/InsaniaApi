using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

/// <summary>
/// Тест получения слогов
/// </summary>
public class GetSyllables : BaseTest
{
    Mock<ILogger<PersonalNames>> _mockLogger; //зашитый логгер

    /// <summary>
    /// Конструктор теста получения cлогов
    /// </summary>
    public GetSyllables() : base()
    {
        _mockLogger = new Mock<ILogger<PersonalNames>>();
    }

    /// <summary>
    /// Тест на проверку успешность завершения
    /// </summary>
    [Fact]
    public void Success()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = personalNames.GetSyllables("Альтаир");

        //Проверяем, что результат успешный
        Assert.DoesNotContain(result, string.IsNullOrEmpty);
    }

    /// <summary>
    /// Тест на корректный результат
    /// </summary>
    [Fact]
    public void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = personalNames.GetSyllables("Альтаир");

        //Формируем корректный результат
        List<string> correctResult = new()
        {
            "Альта",
            "ир"
        };

        //Проверяем, что результат успешный
        Assert.Equal(correctResult, result);
    }

    /// <summary>
    /// Тест на пустой результат
    /// </summary>
    [Fact]
    public void EmptyResult()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = personalNames.GetSyllables("");

        //Проверяем, что результат успешный
        Assert.False(result.Any());
    }
}
