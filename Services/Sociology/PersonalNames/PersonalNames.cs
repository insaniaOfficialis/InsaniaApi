using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Sociology.Names;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Services.Sociology.PersonalNames;

/// <summary>
/// Сервис имён
/// </summary>
public class PersonalNames : IPersonalNames
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly ILogger<PersonalNames> _logger; //сервис записи логов

    /// <summary>
    /// Конструктор сервиса имён
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public PersonalNames(IMapper mapper, ApplicationContext repository, ILogger<PersonalNames> logger)
    {
        _mapper = mapper;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Метод получения сгенерированного имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="generateLastName"></param>
    /// <returns></returns>
    public async Task<GeneratedName> GetGeneratedName(long? nationId, bool? gender, bool generateLastName)
    {
        try
        {
            //Объявляем переменные
            string name = String.Empty; //имя
            string? prefix = null; //префикс
            string? lastName = null; //фамилия
            Random random = new(); //рандомайзер

            //Формируем запрос к базе
            _logger.LogInformation("PersonalNames. GetGeneratedName. Формируем запрос к базе");
            var generateNamesQuery = _repository
                .NationsPersonalNames
                .Include(x => x.Nation)
                .Include(x => x.PersonalName)
                .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.PersonalName.DateDeleted == null);

            //Если передали нацию
            if (nationId != null)
            {
                _logger.LogInformation("PersonalNames. GetGeneratedName. Дополняем запрос фильтром по нации");
                generateNamesQuery = generateNamesQuery.Where(x => x.NationId == nationId);
            }

            //Если передали пол
            if (gender != null)
            {
                _logger.LogInformation("PersonalNames. GetGeneratedName. Дополняем запрос фильтром по полу");
                generateNamesQuery = generateNamesQuery.Where(x => x.PersonalName.Gender == gender);
            }

            //Получаем данные с базы
            _logger.LogInformation("PersonalNames. GetGeneratedName. Получаем данные с базы");
            var generateNamesBd = await generateNamesQuery.ToListAsync();

            //Выбираем случайное значение
            _logger.LogInformation("PersonalNames. GetGeneratedName. Выбираем случайные значения");
            //Получаем сумму вероятностей
            int totalWeight = Convert.ToInt32(generateNamesBd.Sum(x => x.Probability * 100));
            //Выбираем случайное число до этой суммы
            int weightedPick = random.Next(totalWeight);
            //Проходим по массиву имён
            foreach (var item in generateNamesBd)
            {
                //Если выпавшее число меньше случайного
                if (weightedPick <= item.Probability * 100)
                {
                    //Присваиваем имя и закрываем цикл
                    name = item.PersonalName.Name;
                    break;
                }

                //Уменьшемвыпавшее число на вероятность элемента
                weightedPick -= Convert.ToInt32(item.Probability * 100);
            }

            //Генерируем фамилию
            if(generateLastName)
            {
                //Формируем запрос к базе
                _logger.LogInformation("PersonalNames. GetGeneratedName. Формируем запрос к базе за префиксами");
                var generatePrefixQuery = _repository
                    .NationsPrefixNames
                    .Include(x => x.Nation)
                    .Include(x => x.PrefixName)
                    .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.PrefixName.DateDeleted == null);

                //Получаем данные с базы
                _logger.LogInformation("PersonalNames. GetGeneratedName. Получаем данные за префиксами с базы");
                var generatePrefixBd = await generatePrefixQuery.FirstOrDefaultAsync();

                //Если префикс найден, записываем его
                if(generatePrefixBd != null)
                    prefix = generatePrefixBd.PrefixName.Name;

                //Формируем запрос к базе
                _logger.LogInformation("PersonalNames. GetGeneratedName. Формируем запрос к базе за фамилиями");
                var generateLastNameQuery = _repository
                    .NationsLastNames
                    .Include(x => x.Nation)
                    .Include(x => x.LastName)
                    .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.LastName.DateDeleted == null);

                //Получаем данные с базы
                _logger.LogInformation("PersonalNames. GetGeneratedName. Получаем данные за фамилиями с базы");
                var generateLastNameBd = await generateLastNameQuery.ToListAsync();

                //Выбираем случайное значение
                _logger.LogInformation("PersonalNames. GetGeneratedName. Выбираем случайные значения фамилий");
                //Получаем сумму вероятностей
                int totalWeightLastName = Convert.ToInt32(generateLastNameBd.Sum(x => x.Probability * 100));
                //Выбираем случайное число до этой суммы
                int weightedPickLastName = random.Next(totalWeightLastName);
                //Проходим по массиву имён
                foreach (var item in generateLastNameBd)
                {
                    //Если выпавшее число меньше случайного
                    if (weightedPickLastName <= item.Probability * 100)
                    {
                        //Присваиваем имя и закрываем цикл
                        lastName = item.LastName.Name;
                        break;
                    }

                    //Уменьшемвыпавшее число на вероятность элемента
                    weightedPickLastName -= Convert.ToInt32(item.Probability * 100);
                }
            }

            //Формируем ответ
            _logger.LogInformation("PersonalNames. GetGeneratedName. Возвращаем результат");
            return new GeneratedName(true, name, prefix, lastName);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogInformation("PersonalNames. GetGeneratedName. Внутренняя ошибка: {0}", ex);
            return new GeneratedName(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogInformation("PersonalNames. GetGeneratedName. Системная ошибка: {0}", ex);
            return new GeneratedName(false, new BaseError(500, ex.Message));
        }
    }
}
