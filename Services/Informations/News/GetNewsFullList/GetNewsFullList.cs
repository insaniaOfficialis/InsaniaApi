using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.GetNewsFullList;

/// <summary>
/// Сервис получения полного списка новостей
/// </summary>
public class GetNewsFullList : IGetNewsFullList
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения полного списка новостей
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    public GetNewsFullList(IMapper mapper, ApplicationContext repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<BaseResponseList> Handler(string? search)
    {
        try
        {
            //Получаем результат запроса
            var response = await Query(search);

            //Формируем ответ
            var entities = response.Select(_mapper.Map<BaseResponseListItem>).ToList();

            //Возвращаем ответ
            return new BaseResponseList(true, null, entities!);
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
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<NewsEntity>> Query(string? search)
    {
        //Строим запрос
        IQueryable<NewsEntity> query = _repository.News.Where(x => x.DateDeleted == null);

        //Если передали строку поиска
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.ToLower().Contains(search.ToLower()));

        //Получаем данные с базы
        var entities = await query.OrderBy(x => x.OrdinalNumber).ToListAsync();

        //Формируем ответ
        return entities;
    }
}