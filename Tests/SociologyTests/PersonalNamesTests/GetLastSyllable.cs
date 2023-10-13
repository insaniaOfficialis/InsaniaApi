using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

/// <summary>
/// Тест получения последнего слога
/// </summary>
public class GetLastSyllable : BaseTest
{
    Mock<ILogger<PersonalNames>> _mockLogger; //зашитый логгер

    /// <summary>
    /// Конструктор теста получения успешного кода
    /// </summary>
    public GetLastSyllable() : base()
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
        var result = personalNames.GetLastSyllable("Альтаир");

        //Проверяем, что результат успешный
        Assert.True(!string.IsNullOrEmpty(result));
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
        var result = personalNames.GetLastSyllable("Альтаир");

        //Проверяем, что результат успешный
        Assert.Equal("ир", result);
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
        var result = personalNames.GetLastSyllable("");

        //Проверяем, что результат успешный
        Assert.Equal("", result);
    }
}