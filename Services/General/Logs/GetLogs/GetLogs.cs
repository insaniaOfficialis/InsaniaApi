using AutoMapper;
using Data;
using Domain.Entities.General.Log;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.General.Logs.Response;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.General.Logs.GetLogs;

/// <summary>
/// Сервис получения списка стран
/// </summary>
public class GetLogs : IGetLogs
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly ILogger<GetLogs> _logger; //сервис записи логов

    /// <summary>
    /// Конструктор сервиса получения списка стран
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public GetLogs(IMapper mapper, ApplicationContext repository, ILogger<GetLogs> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    public async Task<GetLogsResponse> Handler(string? search, int? skip, int? take, List<BaseSortRequest?>? sort,
        DateTime? from, DateTime? to, bool? success)
    {
        try
        {
            //Получаем результат запроса
            var response = await Query(search, skip, take, sort, from, to, success);

            //Преобразовываем модели
            var logs = response.Select(_mapper.Map<GetLogsResponseItem>).ToList();

            //Формируем ответ
            return new GetLogsResponse(true, null, logs!);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new GetLogsResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new GetLogsResponse(false, new BaseError(500, ex.Message));
        }

    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="success"></param>
    /// <returns></returns>
    public async Task<List<Log>> Query(string? search, int? skip, int? take, List<BaseSortRequest?>? sort,
        DateTime? from, DateTime? to, bool? success)
    {
        //Строим запрос
        IQueryable<Log> logsQuery = _repository.Logs.Where(x => x.DateDeleted == null);

        //Если передали строку поиска
        if (!string.IsNullOrEmpty(search))
            logsQuery = logsQuery
                .Where(x => x.Method.ToLower().Contains(search.ToLower())
                    || x.Type.ToLower().Contains(search.ToLower()));

        //Если передали признак успешности
        if (success != null)
            logsQuery = logsQuery.Where(x => x.Success == success);

        //Если передали дату от
        if (from != null)
            logsQuery = logsQuery
                .Where(x => x.DateStart >= DateTime.SpecifyKind(from ?? DateTime.Now, DateTimeKind.Utc));

        //Если передали дату до
        if (to != null)
            logsQuery = logsQuery
                .Where(x => x.DateEnd <= DateTime.SpecifyKind(to ?? DateTime.Now, DateTimeKind.Utc));

        //Если передали поле сортировки
        if (sort?.Any() == true)
        {
            //Сортируем по первому элементу сортировки
            IOrderedQueryable<Log> logsOrderQuery = (sort.FirstOrDefault()!.SortKey,
                sort.FirstOrDefault()!.IsAscending) switch
            {
                ("id", true) => logsQuery.OrderBy(x => x.Id),
                ("method", true) => logsQuery.OrderBy(x => x.Method),
                ("type", true) => logsQuery.OrderBy(x => x.Type),
                ("success", true) => logsQuery.OrderBy(x => x.Success),
                ("dateStart", true) => logsQuery.OrderBy(x => x.DateStart),
                ("dateEnd", true) => logsQuery.OrderBy(x => x.DateEnd),
                ("id", false) => logsQuery.OrderByDescending(x => x.Id),
                ("method", false) => logsQuery.OrderByDescending(x => x.Method),
                ("type", false) => logsQuery.OrderByDescending(x => x.Type),
                ("success", false) => logsQuery.OrderByDescending(x => x.Success),
                ("dateStart", false) => logsQuery.OrderByDescending(x => x.DateStart),
                ("dateEnd", false) => logsQuery.OrderByDescending(x => x.DateEnd),
                _ => logsQuery.OrderBy(x => x.Id),
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
                        ("method", true) => logsOrderQuery.ThenBy(x => x.Method),
                        ("type", true) => logsOrderQuery.ThenBy(x => x.Type),
                        ("success", true) => logsOrderQuery.ThenBy(x => x.Success),
                        ("dateStart", true) => logsOrderQuery.ThenBy(x => x.DateStart),
                        ("dateEnd", true) => logsOrderQuery.ThenBy(x => x.DateEnd),
                        ("id", false) => logsOrderQuery.ThenByDescending(x => x.Id),
                        ("method", false) => logsOrderQuery.ThenByDescending(x => x.Method),
                        ("type", false) => logsOrderQuery.ThenByDescending(x => x.Type),
                        ("success", false) => logsOrderQuery.ThenByDescending(x => x.Success),
                        ("dateStart", false) => logsOrderQuery.ThenByDescending(x => x.DateStart),
                        ("dateEnd", false) => logsOrderQuery.ThenByDescending(x => x.DateEnd),
                        _ => logsOrderQuery.ThenBy(x => x.Id),
                    };
                }
            }

            //Приводим в список отсортированный список
            logsQuery = logsOrderQuery;
        }

        //Если передали сколько строк пропустить
        if (skip != null)
            logsQuery = logsQuery.Skip(skip ?? 0);

        //Если передали сколько строк выводить
        if (take != null)
            logsQuery = logsQuery.Take(take ?? 10);

        //Получаем страны с базы
        var logsDb = await logsQuery.ToListAsync();

        //Формируем ответ
        return logsDb;
    }
}
