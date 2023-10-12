using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Sociology.Nations;

/// <summary>
/// Сервис наций
/// </summary>
public class Nations : INations
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly ILogger<Nations> _logger; //сервис записи логов

    /// <summary>
    /// Конструктор сервиса наций
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public Nations(IMapper mapper, ApplicationContext repository, ILogger<Nations> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получения списка наций
    /// </summary>
    /// <param name="raceId"></param>
    /// <returns></returns>
    public async Task<BaseResponseList> GetNationsList(long? raceId)
    {
        try
        {
            //Формируем запрос к базе
            _logger.LogInformation("Nations. GetNationsList. Формируем запрос к базе");
            var nationsQuery = _repository.Nations.Where(x => x.DateDeleted == null);

            //Если передали расу
            if (raceId != null)
            {
                _logger.LogInformation("Nations. GetNationsList. Дополняем запрос фильтром по расе");
                nationsQuery = nationsQuery.Where(x => x.RaceId == raceId);
            }

            //Получаем данные с базы
            _logger.LogInformation("Nations. GetNationsList. Получаем данные с базы");
            var nationsBd = await nationsQuery.ToListAsync();

            //Преобразовываем модели
            _logger.LogInformation("Nations. GetNationsList. Преобразуем данные из базы в стандартный ответ");
            var nations = nationsBd.Select(_mapper.Map<BaseResponseListItem?>).ToList();

            //Формируем ответ
            _logger.LogInformation("Nations. GetNationsList. Возвращаем результат");
            return new BaseResponseList(true, null, nations);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogInformation("Nations. GetNationsList. Внутренняя ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogInformation("Races. GetNationsList. Системная ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }
}
