using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using Services.Politics.Regions.CheckingRegionsColors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Politics.Regions.GenerationRegionColor;

/// <summary>
/// Генерация цвета региона
/// </summary>
public class GenerationRegionColor : IGenerationRegionColor
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Генерация цвета региона
    /// </summary>
    /// <param name="repository"></param>
    public GenerationRegionColor(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<BaseResponseValue> Handler()
    {
        try
        {
            //Получаем результат запроса
            var response = await Query();

            //Формируем ответ
            return new BaseResponseValue(true, null, response);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new BaseResponseValue(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new BaseResponseValue(false, new BaseError(500, ex.Message));
        }

    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<string> Query()
    {
        //Объявляем переменные
        int count = 0; //количество
        int i = 0; //иттерации
        string color = string.Empty; //цвет

        //Выполняем пока не получим уникальный цвет или пока не перейдём порог в 50 иттераций
        do
        {
            //Формируем цвет
            color = string.Concat("#", Guid.NewGuid().ToString().AsSpan(1, 6));

            //Получаем количество данных с базы
            count = await _repository.Regions.Where(x => x.Color == color).CountAsync();

            //Увеличиваем текущую иттерацию
            i++;
        }
        while (count > 0 && i < 50);

        //Если мы вышли не изза предела иттераций
        if (i < 50)
            //Формируем ответ
            return color;
        //Иначе выбиваем ошибку
        else
            throw new Exception(Errors.TryAgain);
    }
}