using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

/// <summary>
/// Тест метода генрации нового имени
/// </summary>
public class GetGeneratingNewName : BaseTest
{
    Mock<ILogger<PersonalNames>> _mockLogger; //зашитый логгер

    /// <summary>
    /// Конструктор теста метода генрации нового имени
    /// </summary>
    public GetGeneratingNewName() : base()
    {
        _mockLogger = new Mock<ILogger<PersonalNames>>();
    }

    /// <summary>
    /// Тест на проверку успешность завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.GetGeneratingNewName(1, true, "Ама", "гиль");

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку правильного овтета
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.GetGeneratingNewName(1, true, "Ама", "гиль");

        //Проверяем, что результат успешный
        Assert.True(!string.IsNullOrEmpty(result.PersonalName));
    }

    /// <summary>
    /// Тест на проверку правильного овтета без первого слога
    /// </summary>
    [Fact]
    public async void CorrectResultWithoutFirstSyllable()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.GetGeneratingNewName(1, true, null, "гиль");

        //Проверяем, что результат успешный
        Assert.True(char.IsUpper(result.PersonalName![0]));
    }

    /// <summary>
    /// Тест на проверку правильного овтета без входящих параметров
    /// </summary>
    [Fact]
    public async void CorrectResultWithoutInParametr()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.GetGeneratingNewName(null, null, null, null);

        //Проверяем, что результат успешный
        Assert.True(char.IsUpper(result.PersonalName![0]));
    }
}
