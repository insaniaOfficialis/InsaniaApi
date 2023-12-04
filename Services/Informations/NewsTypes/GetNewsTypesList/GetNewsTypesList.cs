using AutoMapper;
using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;

namespace Services.Informations.NewsTypes.GetNewsTypesList;

/// <summary>
/// Получение списка типов новостей
/// </summary>
public class GetNewsTypesList : IGetNewsTypesList
{
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly IMapper _mapper; //маппер моделей

    /// <summary>
    /// Получение списка типов новостей
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="mapper"></param>
    public GetNewsTypesList(ApplicationContext repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseList> Handler()
    {
        try
        {
            //Получаем результат запроса
            var response = await Query();

            //Формируем ответ
            var result = response.Select(_mapper.Map<BaseResponseListItem?>).ToList();

            //Формируем ответ
            return new BaseResponseList(true, null, result);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <returns></returns>
    public async Task<List<NewsType>> Query()
    {
        //Получаем данные с базы
        var entities =  await _repository.NewsTypes.Where(x => x.DateDeleted == null).ToListAsync();

        //Формируем ответ
        return entities;
    }
}