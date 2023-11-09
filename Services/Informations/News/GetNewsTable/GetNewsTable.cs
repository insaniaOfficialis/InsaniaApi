using Data;
using Domain.Entities.General.Log;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.News.Response;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.GetNewsTable;

/// <summary>
/// Сервис получения списка новостей для таблицы
/// </summary>
public class GetNewsTable : IGetNewsTable
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения списка новостей
    /// </summary>
    /// <param name="repository"></param>
    public GetNewsTable(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    public async Task<GetNewsTableResponse> Handler(string? search, int? skip, int? take, List<BaseSortRequest?>? sort, bool? isDeleted)
    {
        try
        {
            //Получаем результат запроса
            var response = await Query(search, skip, take, sort, isDeleted);

            //Формируем ответ
            var result = ResponseBuilder(response);

            //Формируем ответ
            return result;
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new GetNewsTableResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new GetNewsTableResponse(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    public async Task<List<NewsEntity>> Query(string? search, int? skip, int? take, List<BaseSortRequest?>? sort, bool? isDeleted)
    {
        //Строим запрос
        IQueryable<NewsEntity> query = _repository.News.Include(x => x.Type).Where(x => x.DateDeleted == null);

        //Если передали строку поиска
        if (!string.IsNullOrEmpty(search))
            query = query
                .Where(x => x.Title.ToLower().Contains(search.ToLower())
                    || x.Type.Name.ToLower().Contains(search.ToLower())
                    || x.Introduction.ToLower().Contains(search.ToLower()));

        //Если передали признак удалённых записей
        if (isDeleted == true)
            query = query.Where(x => x.DateDeleted != null);
        else
            query = query.Where(x => x.DateDeleted == null);

        //Если передали поле сортировки
        if (sort?.Any() == true)
        {
            //Сортируем по первому элементу сортировки
            IOrderedQueryable<NewsEntity> logsOrderQuery = (sort.FirstOrDefault()!.SortKey,
                sort.FirstOrDefault()!.IsAscending) switch
            {
                ("id", true) => query.OrderBy(x => x.Id),
                ("title", true) => query.OrderBy(x => x.Title),
                ("introduction", true) => query.OrderBy(x => x.Introduction),
                ("ordinalNumber", true) => query.OrderBy(x => x.OrdinalNumber),
                ("type", true) => query.OrderBy(x => x.Type.Name),
                ("id", false) => query.OrderByDescending(x => x.Id),
                ("title", false) => query.OrderByDescending(x => x.Title),
                ("introduction", false) => query.OrderByDescending(x => x.Introduction),
                ("ordinalNumber", false) => query.OrderByDescending(x => x.OrdinalNumber),
                ("type", false) => query.OrderByDescending(x => x.Type.Name),
                _ => query.OrderBy(x => x.OrdinalNumber),
            };

            //Если есть ещё поля для сортировки
            if (sort.Count > 1)
            {
                //Проходим по всем элементам сортировки кроме первой
                foreach (var sortElement in sort.Skip(1))
                {
                    //Сортируем по каждому элементу
                    logsOrderQuery = (sortElement!.SortKey, sortElement!.IsAscending) switch
                    {
                        ("id", true) => logsOrderQuery.ThenBy(x => x.Id),
                        ("title", true) => logsOrderQuery.ThenBy(x => x.Title),
                        ("introduction", true) => logsOrderQuery.ThenBy(x => x.Introduction),
                        ("ordinalNumber", true) => logsOrderQuery.ThenBy(x => x.OrdinalNumber),
                        ("type", true) => logsOrderQuery.ThenBy(x => x.Type.Name),
                        ("id", false) => logsOrderQuery.ThenByDescending(x => x.Id),
                        ("title", false) => logsOrderQuery.ThenByDescending(x => x.Title),
                        ("introduction", false) => logsOrderQuery.ThenByDescending(x => x.Introduction),
                        ("ordinalNumber", false) => logsOrderQuery.ThenByDescending(x => x.OrdinalNumber),
                        ("type", false) => logsOrderQuery.ThenByDescending(x => x.Type.Name),
                        _ => logsOrderQuery.ThenBy(x => x.OrdinalNumber),
                    };
                }
            }

            //Приводим в список отсортированный список
            query = logsOrderQuery;
        }

        //Если передали сколько строк пропустить
        if (skip != null)
            query = query.Skip(skip ?? 0);

        //Если передали сколько строк выводить
        if (take != null)
            query = query.Take(take ?? 10);

        //Получаем данные с базы
        var entities = await query.ToListAsync();

        //Формируем ответ
        return entities;
    }

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public GetNewsTableResponse ResponseBuilder(List<NewsEntity> entities)
    {
        //Преобразуем элементы
        var items = entities
            .Select(x => new GetNewsTableResponseItem(x.Id, x.Title, x.Introduction, x.Type.Color, x.OrdinalNumber,
                new(x.Type.Name, x.TypeId)))
            .ToList();

        //Возвращаем сформированный ответ
        return new(true, null, items);
    }
}