using Services;
using Services.Informations.NewsDetails.RemovalNewsDetail;

namespace Tests.InformationsTest.NewsDetailsTests;

/// <summary>
/// Тест метода удаления/восстановления детальной части детальной части новости
/// </summary>
public class RemovalNewsDetailDetailTests : BaseTest
{
    /// <summary>
    /// Конструктор теста метода удаления/восстановления детальной части новости
    /// </summary>
    public RemovalNewsDetailDetailTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешность удаления детальной части новости
    /// </summary>
    [Fact]
    public async void SuccessRemove()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, true);

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку успешность восстановления детальной части новости
    /// </summary>
    [Fact]
    public async void SuccessRecover()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, false);

        //Проверяем результат
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на проверку корректности удаления детальной части новости
    /// </summary>
    [Fact]
    public async void CorrectRemove()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, true);

        //Получаем сущность
        var entity = _repository.NewsDetails.First(x => x.Id == id);

        //Проверяем результат
        Assert.NotNull(entity.DateDeleted);
    }

    /// <summary>
    /// Тест на проверку корректности восстановления детальной части новости
    /// </summary>
    [Fact]
    public async void CorrectRecover()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", id, false);

        //Получаем сущность
        var entity = _repository.NewsDetails.First(x => x.Id == id);

        //Проверяем результат
        Assert.Null(entity.DateDeleted);
    }

    /// <summary>
    /// Тест на проверку пустого пользователя
    /// </summary>
    [Fact]
    public async void EmptyUser()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем максимальный id
        long id = _repository.NewsDetails.Where(x => x.IsSystem == false).Max(x => x.Id);

        //Получаем результат
        var result = await service.Handler(null, id, false);

        //Проверяем результат
        Assert.Equal(Errors.EmptyCurrentUser, result.Error?.Message);
    }

    /// <summary>
    /// Тест на проверку пустой ссылки сущности
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", null, false);

        //Проверяем результат
        Assert.Equal(Errors.EmptyId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на проверку отсутсвющей детальной части новости
    /// </summary>
    [Fact]
    public async void NotExistsNewsId()
    {
        //Создаём новый экземпляр сервиса
        RemovalNewsDetail service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", -1, false);

        //Проверяем результат
        Assert.Equal(Errors.NotExistsNewsDetail, result.Error?.Message);
    }
}