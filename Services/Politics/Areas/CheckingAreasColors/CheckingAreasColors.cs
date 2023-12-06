using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;

namespace Services.Politics.Areas.CheckingAreasColors;

/// <summary>
/// Проверка цветов областей
/// </summary>
public class CheckingAreasColors : ICheckingAreasColors
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Проверка цветов областей
    /// </summary>
    /// <param name="repository"></param>
    public CheckingAreasColors(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки ошибки
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public bool Validator(string? value)
    {
        //Проверяем на пустоту значеняи для проверки
        if (string.IsNullOrEmpty(value))
            throw new InnerException(Errors.EmptyRequest);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? value)
    {
        try
        {
            //Проверяем на ошибки
            var resultValidator = Validator(value);

            //Если успешно
            if (resultValidator)
            {
                //Получаем результат запроса
                var response = await Query(value);

                //Формируем ответ
                return new BaseResponse(response);
            }
            else
            {
                return new BaseResponse(false, new BaseError(400, Errors.NotValidate));
            }
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new BaseResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new BaseResponse(false, new BaseError(500, ex.Message));
        }

    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Query(string? value)
    {
        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Получаем количество данных с базы
        var count = await _repository.Areas.Where(x => x.Color == value).CountAsync();

        //Если нашли записи, выбиываем количество
        if (count > 0)
            throw new InnerException(Errors.BusyColor);

        //Формируем ответ
        return true;
    }
}