using Services;
using Services.Informations.InformationArticles.AddInformationArticle;

namespace Tests.InformationsTest.InformationArticlesTests;

/// <summary>
/// Тест метода добавления информационной статьи
/// </summary>
public class AddInformationArticleTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода добавления имени
    /// </summary>
    public AddInformationArticleTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность информационной статьи
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticle service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticles.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id)));

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
        AddInformationArticle service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticles.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler("system", new(string.Format("Тест_{0}", id)));


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
        AddInformationArticle service = new(_repository);

        //Получаем максимальный id
        long id = _repository.InformationArticles.Max(x => x.Id) + 1;

        //Получаем результат
        var result = await service.Handler(null, new(string.Format("Тест_{0}", id)));


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
        AddInformationArticle service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", null);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyRequest, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие заголовка
    /// </summary>
    [Fact]
    public async Task NotFoundTitle()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticle service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new(""));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyTitle, result.Error?.Message);
    }

    /// <summary>
    /// Тест на существующюю информационную статью
    /// </summary>
    [Fact]
    public async void ExistingInformationArticle()
    {
        //Создаём новый экземпляр сервиса
        AddInformationArticle service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", new("Тест_1"));

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.ExistingInformationArticle, result.Error?.Message);
    }
}