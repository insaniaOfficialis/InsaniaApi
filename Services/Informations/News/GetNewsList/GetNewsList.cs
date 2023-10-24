using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.News.Response;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.GetNewsList;

/// <summary>
/// Сервис получения списка новостей
/// </summary>
public class GetNewsList : IGetNewsList
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения списка новостей
    /// </summary>
    /// <param name="repository"></param>
    public GetNewsList(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<GetNewsListResponse> Handler(string? search)
    {
        try
        {
            //Получаем результат запроса
            var response = await Query(search);

            //Формируем ответ
            var result = ResponseBuilder(response);

            //Формируем ответ
            return result;
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new GetNewsListResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new GetNewsListResponse(false, new BaseError(500, ex.Message));
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
        IQueryable<NewsEntity> query = _repository.News.Include(x => x.Type).Where(x => x.DateDeleted == null);

        //Если передали строку поиска
        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Title.ToLower().Contains(search.ToLower()));

        //Получаем данные с базы
        var entities = await query.Skip(0).Take(5).OrderByDescending(x => x.DateCreate).ToListAsync();

        //Формируем ответ
        return entities;
    }

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public GetNewsListResponse ResponseBuilder(List<NewsEntity> entities)
    {
        //Преобразуем элементы
        var items = entities.Select(x => new GetNewsResponseItem(x.Id, x.Title, x.Introduction, x.Type.Color)).ToList();

        //Возвращаем сформированный ответ
        return new(true, null, items);
    }
}