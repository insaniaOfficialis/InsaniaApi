using Microsoft.EntityFrameworkCore;
using Services;
using Services.General.Files.EditOrdinalNumberFile;

namespace Tests.GeneralTests.FileTests;

/// <summary>
/// Тесты редактирования порядкового номера файла
/// </summary>
public class EditOrdinalNumberFileTests : BaseTest
{
    /// <summary>
    /// Конструктор теста редактирования порядкового номера файла
    /// </summary>
    public EditOrdinalNumberFileTests() : base()
    {
    }

    /// <summary>
    /// Тест на проверку успешности завершения
    /// </summary>
    [Fact]
    public async void Success()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем максимальный id
        long id = await _repository.Files.Where(x => x.IsSystem == false && x.TypeId == 4).MaxAsync(x => x.Id);

        //Получаем предыдущий порядковый номер
        var ordinalNumber = (await _repository.FilesNewsDetails.FirstAsync(x => x.FileId == id)).OrdinalNumber;

        //Получаем результат
        var result = await service.Handler("system", 2, id);

        //Проверяем, что результат успешный
        Assert.True(result.Success);

        //Возвращаем данные к исходному состоянию
        await service.Handler("system", ordinalNumber, id);
    }

    /// <summary>
    /// Тест на проверку правильного завершения
    /// </summary>
    [Fact]
    public async void CorrectResult()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем максимальный id
        long id = await _repository.Files.Where(x => x.IsSystem == false && x.TypeId == 4).MaxAsync(x => x.Id);

        //Получаем предыдущий порядковый номер
        var ordinalNumber = (await _repository.FilesNewsDetails.FirstAsync(x => x.FileId == id)).OrdinalNumber;

        //Получаем результат
        var result = await service.Handler("system", 2, id);

        //Получаем текущий порядковый номер
        var correctOrdinalNumber = (await _repository.FilesNewsDetails.FirstAsync(x => x.FileId == id)).OrdinalNumber;

        //Проверяем корректность резульата
        Assert.Equal(2, correctOrdinalNumber);

        //Возвращаем данные к исходному состоянию
        await service.Handler("system", ordinalNumber, id);
    }

    /// <summary>
    /// Тест на отсутствие пользователя
    /// </summary>
    [Fact]
    public async Task NotFoundUser()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем максимальный id
        long id = await _repository.Files.Where(x => x.IsSystem == false && x.TypeId == 4).MaxAsync(x => x.Id);

        //Получаем результат
        var result = await service.Handler(null, 2, id);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyCurrentUser, result.Error?.Message);
    }

    /// <summary>
    /// Тест на пустую ссылку на файл
    /// </summary>
    [Fact]
    public async void EmptyId()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", 2, null);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyId, result.Error?.Message);
    }

    /// <summary>
    /// Тест на не существующий файл
    /// </summary>
    [Fact]
    public async void NotExistsFiles()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем результат
        var result = await service.Handler("system", 2, -1);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.NotExistsFile, result.Error?.Message);
    }

    /// <summary>
    /// Тест на отсутсвие порядкового номера
    /// </summary>
    [Fact]
    public async Task EmptyOrdinalNumber()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем максимальный id
        long id = await _repository.Files.Where(x => x.IsSystem == false && x.TypeId == 4).MaxAsync(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", null, id);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.EmptyOrdinalNumber, result.Error?.Message);
    }

    /// <summary>
    /// Тест на некорректный порядковый номер
    /// </summary>
    [Fact]
    public async Task IncorrectOrdinalNumber()
    {
        //Создаём новый экземпляр сервиса
        EditOrdinalNumberFile service = new(_repository);

        //Получаем максимальный id
        long id = await _repository.Files.Where(x => x.IsSystem == false && x.TypeId == 4).MaxAsync(x => x.Id);

        //Получаем результат
        var result = await service.Handler("system", -1, id);

        //Проверяем, что результат возвращён с корректной ошибкой
        Assert.Equal(Errors.IncorrectOrdinalNumber, result.Error?.Message);
    }
}