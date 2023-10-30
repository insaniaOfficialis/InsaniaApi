using Services;
using Services.Informations.News.AddNews;

namespace Tests.InformationsTest.NewsTests;

/// <summary>
/// Тест метода добавления новости
/// </summary>
public class AddNewsTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public AddNewsTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность новости
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        AddNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), "ААА", 1));

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
        AddNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), "ААА", 1));


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
        AddNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), "ААА", 1));


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
        AddNews service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", null);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyRequest, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие заголовка
    /// </summary>
    [Fact]
    public async Task EmptyTitle()
    {
        //Создаём новый экземпляр сервиса
        AddNews service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("", "ААА", 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyTitle, result.Error?.Message);
    }

    /// <summary>
    /// Тест на существующюю новость
    /// </summary>
    [Fact]
    public async void ExistingNews()
    {
        //Создаём новый экземпляр сервиса
        AddNews service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("Запуск новостей", "ААА", 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.ExistingNews, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустое вступление
    /// </summary>
    [Fact]
    public async void EmptyIntroduction()
    {
        //Создаём новый экземпляр сервиса
        AddNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), "", 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyIntroduction, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустой тип новости
    /// </summary>
    [Fact]
    public async void EmptyTypeNews()
    {
        //Создаём новый экземпляр сервиса
        AddNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), "ААА", 0));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyTypeNews, result.Error?.Message);
    }

    /// <summary>
    /// Тест на не существующий тип новости
    /// </summary>
    [Fact]
    public async void NotExistsTypeNews()
    {
        //Создаём новый экземпляр сервиса
        AddNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), "ААА", -1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.NotExistsTypeNews, result.Error?.Message);
    }
}