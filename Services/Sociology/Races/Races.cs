using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Services.Sociology.Races;

/// <summary>
/// Сервис рас
/// </summary>
public class Races : IRaces
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly ILogger<Races> _logger; //сервис записи логов

    /// <summary>
    /// Конструктор сервиса стран
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public Races(IMapper mapper, ApplicationContext repository, ILogger<Races> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получения списка рас
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseList> GetRacesList()
    {
        try
        {
            //Получаем расы с базы
            _logger.LogInformation("Races. GetRacesList. Получаем расы с базы");
            var racesDb = await _repository.Races.ToListAsync() ?? throw new InnerException("Не удалось найти расы в базе");

            //Получаем сущность рас
            _logger.LogInformation("Races. GetRacesList. Выбираем не удалённые расы");
            var racesEntity = racesDb.Where(x => !x.IsDeleted).ToList()
                ?? throw new InnerException("Не удалось найти не удалённые расы");

            //Преобразовываем модели
            _logger.LogInformation("Races. GetRacesList. Преобразуем расы из базы в стандартный ответ");
            var races = racesEntity.Select(_mapper.Map<BaseResponseListItem>).ToList()
                ?? throw new InnerException("Не удалось преобразовать модель базы данных в модель ответа");

            //Формируем ответ
            _logger.LogInformation("Races. GetRacesList. Возвращаем результат");
            return new BaseResponseList(true, null, races!);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogInformation("Races. GetRacesList. Внутренняя ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogInformation("Races. GetRacesList. Системная ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }
}
