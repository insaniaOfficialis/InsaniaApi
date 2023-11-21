using Services;
using Services.Informations.News.EditNews;

namespace Tests.InformationsTest.NewsTests;

/// <summary>
/// Тест метода редактирования новости
/// </summary>
public class EditNewsTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода редактирования новости
    /// </summary>
    public EditNewsTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность новости
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id + 1), "БББ", 1, 2), id);

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
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id + 1), "БББ", 1, 2), id);

        //Проверяем корректность резульата
        Assert.Equal(id, result.Id);
    }

    /// <summary>
    /// Тест на отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task NotFoundUser()
    {
        //Создаём новый экземпляр сервиса
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id + 1), "БББ", 1, 2), id);


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
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", null, id);

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
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", new("", "БББ", 1, 2), id);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyTitle, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустое вступление
    /// </summary>
    [Fact]
    public async void EmptyIntroduction()
    {
        //Создаём новый экземпляр сервиса
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id + 1), "", 1, 2), id);

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
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), "БББ", 0, 2), id);

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
        EditNews service = new(_repository);

        //Получаем максимальный id
        long id = _repository.News.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id + 1), "БББ", -1, 2), id);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.NotExistsTypeNews, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустую ссылку на новость
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        EditNews service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", 3), "БББ", 1, 2), null);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на не существующий новость
    /// </summary>
    [Fact]
    public async void NotExistsNews()
    {
        //Создаём новый экземпляр сервиса
        EditNews service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", 3), "БББ", 1, 2), -1);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.NotExistsNews, result.Error?.Message);
    }
}