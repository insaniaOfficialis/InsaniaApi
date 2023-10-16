using Microsoft.Extensions.Logging;
using Services.Sociology.PersonalNames;

namespace Tests.SociologyTests.PersonalNamesTests;

/// <summary>
/// Тест метода добавления имени
/// </summary>
public class AddName : BaseTest
{
    Mock<ILogger<PersonalNames>> _mockLogger; //зашитый логгер

    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public AddName() : base()
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

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName("system", 1, true, string.Format("Альтаир_{0}", id), 0.89);

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку успешность завершения без вероятности
    /// </summary>
    [Fact]
    public async void SuccessWithoutProbability()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName("system", 1, true, string.Format("Альтаир_{0}", id), null);

        //Проверяем, что совпадают id
        Assert.Equal(id, result.Id);
    }

    /// <summary>
    /// Тест на проверку правильного завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName("system", 1, true, string.Format("Альтаир_{0}", id), 0.89);

        //Проверяем, что совпадают id
        Assert.Equal(id, result.Id);
    }

    /// <summary>
    /// Тест на отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task NotFoundUser()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName(null, 1, true, string.Format("Альтаир_{0}", id), 0.89);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal("Не найден текущий пользователь", result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие нации
    /// </summary>
    [Fact]
    public async Task NotFoundNation()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName("system", null, true, string.Format("Альтаир_{0}", id), 0.89);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal("Не указана нация", result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие пола
    /// </summary>
    [Fact]
    public async Task NotFoundGender()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName("system", 1, null, string.Format("Альтаир_{0}", id), 0.89);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal("Не указан пол", result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие имени
    /// </summary>
    [Fact]
    public async Task NotFoundName()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.AddName("system", 1, true, null, 0.89);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal("Не указано имя", result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсуществующюю нацию
    /// </summary>
    [Fact]
    public async void NotExistingNation()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем максимальный id
        long id = _repository.PersonalNames.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await personalNames.AddName("system", 100002, true, string.Format("Альтаир_{0}", id), 0.89);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal("Не найдена указанная нация", result.Error?.Message);
    }

    /// <summary>
    /// Тест на существующее имя
    /// </summary>
    [Fact]
    public async void ExistingName()
    {
        //Создаём новый экземпляр сервиса
        PersonalNames personalNames = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await personalNames.AddName("system", 1, true, "Верития", 0.89);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal("Указанное имя уже существует", result.Error?.Message);
    }
}
