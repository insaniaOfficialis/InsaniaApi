using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Geography.Countries;

/// <summary>
/// Сервис стран
/// </summary>
public class Countries: ICountries
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly ILogger<Countries> _logger; //сервис записи логов

    /// <summary>
    /// Конструктор сервиса стран
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public Countries(IMapper mapper, ApplicationContext repository, ILogger<Countries> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получения списка стран
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseList> GetCountries()
    {
        try
        {
            /*Получаем страны с базы*/
            var countriesDb = await _repository.Countries.ToListAsync() ?? throw new InnerException("Не удалось найти страны в базе");

            /*Получаем сущность стран*/
            var countriesEntity = countriesDb.Where(x => !x.IsDeleted).ToList() 
                ?? throw new InnerException("Не удалось найти не удалённые страны");

            /*Преобразовываем модели*/
            var countries = countriesEntity.Select(_mapper.Map<BaseResponseListItem>).ToList() 
                ?? throw new InnerException("Не удалось преобразовать модель базы данных в модель ответа");

            /*Формируем ответ*/
            return new BaseResponseList(true, null, countries!);
        }
        /*Обрабатываем внутренние исключения*/
        catch (InnerException ex)
        {
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        /*Обрабатываем системные исключения*/
        catch (Exception ex)
        {
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }
}
