using Services.Identification.Authorization;

namespace Tests.IdentificationTests.AuthorizationTests;

public class GetUserInfo : BaseTest
{

    /// <summary>
    /// Конструктор теста получения успешного кода
    /// </summary>
    public GetUserInfo() : base()
    {
    }

    /// <summary>
    /// Тест на успешность завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        Authorization authorization = new(_userManager, _token, _mapper, _repository);

        //Получаем результат
        var result = await authorization.GetUserInfo("insania");

        //Проверяем, что результат успешный
        Assert.True(result.Success);
    }

    /// <summary>
    /// Тест на корректный результат
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        Authorization authorization = new(_userManager, _token, _mapper, _repository);

        //Получаем результат
        var result = await authorization.GetUserInfo("insania");

        //Проверяем, что результат успешный
        Assert.NotNull(result.UserName);
    }

    /// <summary>
    /// Тест на некорректный результат
    /// </summary>
    [Fact]
    public async void NotSuccess()
    {
        //Создаём новый экземпляр сервиса
        Authorization authorization = new(_userManager, _token, _mapper, _repository);

        //Получаем результат
        var result = await authorization.GetUserInfo("");

        //Проверяем, что результат не успешный
        Assert.False(result.Success);
    }

    /// <summary>
    /// Тест на пустой входящий логин
    /// </summary>
    [Fact]
    public async Task IncorrectUsername()
    {
        //Создаём новый экземпляр сервиса
        Authorization authorization = new(_userManager, _token, _mapper, _repository);

        //Получаем результат 
        var result = await authorization.GetUserInfo(null);

        //Проверяем, что результат успешный
        Assert.Equal("Не указан пользователь", result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task NotFoundUser()
    {
        //Создаём новый экземпляр сервиса
        Authorization authorization = new(_userManager, _token, _mapper, _repository);

        //Получаем результат 
        var result = await authorization.GetUserInfo("asdfg");

        //Проверяем, что результат успешный
        Assert.Equal("Пользователь не найден", result.Error?.Message);
    }
}