using Services;
using Services.Informations.NewsDetails.AddNewsDetail;

namespace Tests.InformationsTest.NewsDetailsTests;

/// <summary>
/// Тест метода добавления детальной части новости
/// </summary>
public class AddNewsDetailTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public AddNewsDetailTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность детальной части новости
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        AddNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), 1));

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку правильного завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        AddNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), 1));

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
        AddNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyCurrentUser, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустой запрос
    /// </summary>
    [Fact]
    public async Task EmptyRequest()
    {
        //Создаём новый экземпляр сервиса
        AddNewsDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", null);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyRequest, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие текста
    /// </summary>
    [Fact]
    public async Task NotFoundText()
    {
        //Создаём новый экземпляр сервиса
        AddNewsDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("", 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyText, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие ссылки на новость
    /// </summary>
    [Fact]
    public async Task NotFoundNewsId()
    {
        //Создаём новый экземпляр сервиса
        AddNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), null));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyNewsId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутсвующую новость
    /// </summary>
    [Fact]
    public async void NotExistingNews()
    {
        //Создаём новый экземпляр сервиса
        AddNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), 100003245));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.NotExistsNews, result.Error?.Message);
    }
}