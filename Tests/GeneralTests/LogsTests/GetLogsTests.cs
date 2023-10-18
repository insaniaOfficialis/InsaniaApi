using Microsoft.Extensions.Logging;
using Services.General.Logs.GetLogs;

namespace Tests.GeneralTests.LogsTests;

/// <summary>
/// Тест сервиса получения списка логов
/// </summary>
public class GetLogsTests : BaseTest
{
    Mock<ILogger<GetLogs>> _mockLogger; //зашитый логгер

    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public GetLogsTests() : base()
    {
        _mockLogger = new Mock<ILogger<GetLogs>>();
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, 0, 20, null, null, null, null);

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку успешности завершения без данных
    /// </summary>
    [Fact]
    public async void SuccessWithountAll()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, null, null, null, null, null, null);

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, 0, 20, null, null, null, null);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }

    /// <summary>
    /// Тест на проверку корректности завершения со всеми входящими параметрами
    /// </summary>
    [Fact]
    public async void CorrectResultWithAll()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler("Get", 0, 20, new() { new() { SortKey = "id", IsAscending = false } },
            new DateTime(2023, 01, 01), new DateTime(2024, 01, 01), true);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }

    /// <summary>
    /// Тест на проверку корректности завершения со строкой поиска
    /// </summary>
    [Fact]
    public async void CorrectResultWithSearch()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler("Get", 0, 20, null, null, null, null);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }

    /// <summary>
    /// Тест на проверку корректности завершения с сортировкой
    /// </summary>
    [Fact]
    public async void CorrectResultWithSort()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, 0, 20, new() { new() { SortKey = "id", IsAscending = false } },
            null, null, null);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }

    /// <summary>
    /// Тест на проверку корректности завершения с датой от
    /// </summary>
    [Fact]
    public async void CorrectResultWithFrom()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, 0, 20, null, new DateTime(2023, 01, 01), null, null);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }

    /// <summary>
    /// Тест на проверку корректности завершения с датой до
    /// </summary>
    [Fact]
    public async void CorrectResultWithTo()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, 0, 20, null, null, new DateTime(2024, 01, 01), null);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }

    /// <summary>
    /// Тест на проверку корректности завершения с успешностью
    /// </summary>
    [Fact]
    public async void CorrectResultWithSuccess()
    {
        //Создаём новый экземпляр сервиса
        GetLogs getLogs = new(_mapper, _repository, _mockLogger.Object);

        //Получаем результат
        var result = await getLogs.Handler(null, 0, 20, null, null, null, true);

        //Проверяем, что результат успешный
        Assert.Equal(20, result.Items?.Count);
    }
}