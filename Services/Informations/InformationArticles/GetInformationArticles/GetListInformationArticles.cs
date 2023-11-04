using AutoMapper;
using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;

namespace Services.Informations.InformationArticles.GetInformationArticles;

/// <summary>
/// Сервис получения списка информационных статей
/// </summary>
public class GetListInformationArticles : IGetListInformationArticles
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения списка информационных статей
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    public GetListInformationArticles(IMapper mapper, ApplicationContext repository)
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

            //Преобразовываем модели
            var entities = response.Select(_mapper.Map<BaseResponseListItem>).ToList();

            //Формируем ответ
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
    public async Task<List<InformationArticle>> Query(string? search)
    {
        //Строим запрос
        IQueryable<InformationArticle> query = _repository
            .InformationArticles
            .Where(x => x.DateDeleted == null);

        //Если передали строку поиска
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.ToLower().Contains(search.ToLower()));

        //Сортируем список
        query = query.OrderBy(x => x.OrdinalNumber);

        //Получаем данные с базы
        var entities = await query.ToListAsync();

        //Формируем ответ
        return entities;
    }
}