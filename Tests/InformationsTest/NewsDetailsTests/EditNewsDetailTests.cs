using Services;
using Services.Informations.NewsDetails.EditNewsDetail;

namespace Tests.InformationsTest.NewsDetailsTests;

/// <summary>
/// Тест метода добавления детальной части новости
/// </summary>
public class EditNewsDetailTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public EditNewsDetailTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler("system", new("Тестирование", 4, 2), id);

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler("system", new("Тестирование", 4, 2), id);

        //Проверяем, что результат корректный
        Assert.Equal(id, result.Id);
    }

    /// <summary>
    /// Тест на отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task NotFoundUser()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler(null, new("Тестирование", 4, 2), id);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.EmptyCurrentUser, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустой запрос
    /// </summary>
    [Fact]
    public async Task EmptyRequest()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler("system", null, id);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.EmptyRequest, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие текста
    /// </summary>
    [Fact]
    public async Task NotFoundText()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler("system", new("", 4, 2), id);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.EmptyText, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие ссылки на новость
    /// </summary>
    [Fact]
    public async Task EmptyNewsId()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler(null, new("Тестирование", null, 2), id);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.EmptyNewsId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутсвующую новость
    /// </summary>
    [Fact]
    public async void NotExistingNews()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.First(x => x.IsSystem == false).Id;

        //Получаем результат
        var result = await service.Handler(null, new("Тестирование", -1, 2), id);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.NotExistsNews, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие ссылки на детальную часть новости
    /// </summary>
    [Fact]
    public async void EmptyNewsDetailId()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("Тестирование", 4, 2), null);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.EmptyId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствующую детальную часть новости
    /// </summary>
    [Fact]
    public async void NotExistsNewsDetail()
    {
        //Создаём новый экземпляр сервиса
        EditNewsDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("Тестирование", 4, 2), -1);

        //Проверяем, что результат корректный
        Assert.Equal(Errors.NotExistsNewsDetail, result.Error?.Message);
    }
}