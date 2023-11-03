using Services;
using Services.Informations.InformationArticlesDetails.AddInformationArticleDetail;

namespace Tests.InformationsTest.InformationArticlesDetailsDetailsTests;

/// <summary>
/// Тест метода добавления детальной части информационной статьи
/// </summary>
public class AddInformationArticleDetailDetailTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public AddInformationArticleDetailDetailTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность детальной части информационной статьи
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticleDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticlesDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), 1, 1));

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
        AddInformationArticleDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticlesDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id), 1, 1));

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
        AddInformationArticleDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticlesDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), 1, 1));

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
        AddInformationArticleDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", null);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyRequest, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие текста
    /// </summary>
    [Fact]
    public async Task NotFoundTitle()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticleDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("", 1, 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyText, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие ссылки на информационную статью
    /// </summary>
    [Fact]
    public async Task NotFoundInformationArticleId()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticleDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticlesDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), null, 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyInformationArticleId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутсвующую информационную статью
    /// </summary>
    [Fact]
    public async void NotExistingInformationArticle()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticleDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticlesDetails.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id), 100003245, 1));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.NotExistsInformationArticle, result.Error?.Message);
    }
}