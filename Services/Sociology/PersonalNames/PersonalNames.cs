using AutoMapper;
using Data;
using Domain.Entities.General.System;
using Domain.Entities.Sociology;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Sociology.Names;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text;
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
    private char[] _vowels = "аоуиэыяюеё".ToCharArray(); //массив гласных букв

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
            if (generateLastName && nationId != null)
            {
                //Формируем запрос к базе
                _logger.LogInformation("PersonalNames. GetGeneratedName. Формируем запрос к базе за префиксами");
                var generatePrefixQuery = _repository
                    .NationsPrefixNames
                    .Include(x => x.Nation)
                    .Include(x => x.PrefixName)
                    .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.PrefixName.DateDeleted == null && x.NationId == nationId);

                //Получаем данные с базы
                _logger.LogInformation("PersonalNames. GetGeneratedName. Получаем данные за префиксами с базы");
                var generatePrefixBd = await generatePrefixQuery.FirstOrDefaultAsync();

                //Если префикс найден, записываем его
                if (generatePrefixBd != null)
                    prefix = generatePrefixBd.PrefixName.Name;

                //Формируем запрос к базе
                _logger.LogInformation("PersonalNames. GetGeneratedName. Формируем запрос к базе за фамилиями");
                var generateLastNameQuery = _repository
                    .NationsLastNames
                    .Include(x => x.Nation)
                    .Include(x => x.LastName)
                    .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.LastName.DateDeleted == null && x.NationId == nationId);

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

    /// <summary>
    /// Метод получения начал имён
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    public async Task<BaseResponseList> GetListBeginningsNames(long? nationId, bool? gender)
    {
        try
        {
            //Объявляем переменные

            //Формируем запрос к базе
            _logger.LogInformation("PersonalNames. GetListBeginningsNames. Формируем запрос к базе");
            var namesQuery = _repository
                .NationsPersonalNames
                .Include(x => x.Nation)
                .Include(x => x.PersonalName)
                .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.PersonalName.DateDeleted == null);

            //Если передали нацию
            if (nationId != null)
            {
                _logger.LogInformation("PersonalNames. GetListBeginningsNames. Дополняем запрос фильтром по нации");
                namesQuery = namesQuery.Where(x => x.NationId == nationId);
            }

            //Если передали пол
            if (gender != null)
            {
                _logger.LogInformation("PersonalNames. GetListBeginningsNames. Дополняем запрос фильтром по полу");
                namesQuery = namesQuery.Where(x => x.PersonalName.Gender == gender);
            }

            //Получаем данные с базы
            _logger.LogInformation("PersonalNames. GetListBeginningsNames. Получаем данные с базы");
            var namesBd = await namesQuery.ToListAsync();

            //Преобразовываем модели
            _logger.LogInformation("Races. GetListBeginningsNames. Преобразуем данные из базы в стандартный ответ");
            var names = namesBd
                .Select(x => new BaseResponseListItem(GetFirstSyllable(x.PersonalName.Name)))
                .Where(x => !String.IsNullOrEmpty(x.Name))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .ToList();

            //Формируем ответ
            _logger.LogInformation("PersonalNames. GetListBeginningsNames. Возвращаем результат");
            return new BaseResponseList(true, null, names!);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogInformation("PersonalNames. GetListBeginningsNames. Внутренняя ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogInformation("PersonalNames. GetListBeginningsNames. Системная ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод получения первого слога
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string? GetFirstSyllable(string? value)
    {
        try
        {
            //Разбиваем строки на слоги
            var syllables = GetSyllables(value);

            //Получаем первый слог
            var firstSyllable = syllables.FirstOrDefault();

            //Если слово не одинаковое со слогом
            if (value != firstSyllable)
                //Возвращаем первый слог
                return firstSyllable;
            //Иначе возвращаем пустоту
            else
                return null;
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    /// <summary>
    /// Метод получения последнего слога
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public string? GetLastSyllable(string? value)
    {
        try
        {
            //Разбиваем строки на слоги
            var syllables = GetSyllables(value);

            //Получаем последний слог
            var lastSyllable = syllables.LastOrDefault();

            //Если слово не одинаковое со слогом
            if(value != lastSyllable)
                //Возвращаем последний слог
                return lastSyllable;
            //Иначе возвращаем пустоту
            else
                return null;
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }

    /// <summary>
    /// Метод получения слогов
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public IEnumerable<string?> GetSyllables(string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            var sb = new StringBuilder();
            int i = 0;
            for (; value.Skip(i).Count(_vowels.Contains) > 1; i++)
            {
                sb.Append(value[i]);
                if (_vowels.Contains(value[i]))
                {
                    yield return sb.ToString();
                    sb.Clear();
                }
            }
            yield return value[i..];
        }
    }

    /// <summary>
    /// Метод получения окончания имён
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <returns></returns>
    public async Task<BaseResponseList> GetListEndingsNames(long? nationId, bool? gender)
    {
        try
        {
            //Объявляем переменные

            //Формируем запрос к базе
            _logger.LogInformation("PersonalNames. GetListEndingsNames. Формируем запрос к базе");
            var namesQuery = _repository
                .NationsPersonalNames
                .Include(x => x.Nation)
                .Include(x => x.PersonalName)
                .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.PersonalName.DateDeleted == null);

            //Если передали нацию
            if (nationId != null)
            {
                _logger.LogInformation("PersonalNames. GetListEndingsNames. Дополняем запрос фильтром по нации");
                namesQuery = namesQuery.Where(x => x.NationId == nationId);
            }

            //Если передали пол
            if (gender != null)
            {
                _logger.LogInformation("PersonalNames. GetListEndingsNames. Дополняем запрос фильтром по полу");
                namesQuery = namesQuery.Where(x => x.PersonalName.Gender == gender);
            }

            //Получаем данные с базы
            _logger.LogInformation("PersonalNames. GetListEndingsNames. Получаем данные с базы");
            var namesBd = await namesQuery.ToListAsync();

            //Преобразовываем модели
            _logger.LogInformation("Races. GetListEndingsNames. Преобразуем данные из базы в стандартный ответ");
            var names = namesBd
                .Select(x => new BaseResponseListItem(GetLastSyllable(x.PersonalName.Name)))
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .DistinctBy(x => x.Name)
                .OrderBy(x => x.Name)
                .ToList();

            //Формируем ответ
            _logger.LogInformation("PersonalNames. GetListEndingsNames. Возвращаем результат");
            return new BaseResponseList(true, null, names!);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogError("PersonalNames. GetListEndingsNames. Внутренняя ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogError("PersonalNames. GetListEndingsNames. Системная ошибка: {0}", ex);
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод генерации нового имени
    /// </summary>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="firstSyllable"></param>
    /// <param name="lastSyllable"></param>
    /// <returns></returns>
    public async Task<GeneratedName> GetGeneratingNewName(long? nationId, bool? gender, string? firstSyllable,
        string? lastSyllable)
    {
        try
        {
            //Формируем запрос к базе
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Формируем запрос к базе");
            var namesQuery = _repository
                .NationsPersonalNames
                .Include(x => x.Nation)
                .Include(x => x.PersonalName)
                .Where(x => x.DateDeleted == null && x.Nation.DateDeleted == null && x.PersonalName.DateDeleted == null);

            //Если передали нацию
            if (nationId != null)
            {
                _logger.LogInformation("PersonalNames. GetGeneratingNewName. Дополняем запрос фильтром по нации: {0}", nationId);
                namesQuery = namesQuery.Where(x => x.NationId == nationId);
            }

            //Если передали пол
            if (gender != null)
            {
                _logger.LogInformation("PersonalNames. GetGeneratingNewName. Дополняем запрос фильтром по полу: {0}", gender);
                namesQuery = namesQuery.Where(x => x.PersonalName.Gender == gender);
            }

            //Получаем данные с базы
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем данные с базы");
            var namesBd = await namesQuery.ToListAsync();

            //Получаем слоги слов
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем слоги слов");
            List<string?> syllables = namesBd
                .SelectMany(x => GetSyllables(x.PersonalName.Name))
                .Where(x => x != firstSyllable && x != lastSyllable)
                .ToList();

            //Генерируем количество слогов в имени
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Генерируем количество слогов в имени");
            Random random = new();
            int maxCountNewSyllables = 2;
            //Если не указано ни начало ни конец
            if (string.IsNullOrEmpty(firstSyllable) && string.IsNullOrEmpty(lastSyllable))
                maxCountNewSyllables = 4;
            //Если есть или начало, или конец
            if (!string.IsNullOrEmpty(firstSyllable) && string.IsNullOrEmpty(lastSyllable) 
                || string.IsNullOrEmpty(firstSyllable) && !string.IsNullOrEmpty(lastSyllable))
                maxCountNewSyllables = 3;

            int countNewSyllables = random.Next(1, maxCountNewSyllables);

            //Формируем список случайных слогов
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Формируем список случайных слогов");
            List<string> randomSyllables = new();

            //Если есть первый слог, добавляем в начало
            if (!string.IsNullOrEmpty(firstSyllable))
                randomSyllables.Add(firstSyllable);

            //Получаем слоги, начинающиеся с прописных букв
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем слоги, начинающиеся с прописных букв");
            List<string?> syllabalesUpper = new();
            if(string.IsNullOrEmpty(firstSyllable))
                syllabalesUpper = syllables.Where(x => char.IsUpper(x![0])).ToList();

            //Получаем слоги, начинающиеся со строчных букв
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем слоги, начинающиеся со строчных букв");
            List<string?> syllablesLower = syllables.Where(x => !char.IsUpper(x![0])).ToList();


            //Проходим циклом столько раз, сколько нужно новых слогов
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Проходим циклом столько раз, сколько нужно новых слогов");
            for (int i = 0; i < countNewSyllables; i++)
            {
                string randomSyllable = string.Empty;
                //Если нет первого слога на вход и это первый проход
                if (string.IsNullOrEmpty(firstSyllable) && i == 0)
                {
                    //Получаем общее количество слогов
                    _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем общее количество слогов");
                    int countSyllables = syllabalesUpper.Count - 1;

                    //Генерируем случайных индекс слога
                    _logger.LogInformation("PersonalNames. GetGeneratingNewName. Генерируем случайных индекс слога");
                    int randomSyllableIndex = random.Next(0, countSyllables);

                    //Получаем случайный слог
                    _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем случайный слог");
                    randomSyllable = syllabalesUpper[randomSyllableIndex]!;
                }
                //Иначе, находим слоги с незаглавных букв
                else
                {
                    //Получаем общее количество слогов
                    _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем общее количество слогов");
                    int countSyllables = syllablesLower.Count - 1;

                    //Генерируем случайных индекс слога
                    _logger.LogInformation("PersonalNames. GetGeneratingNewName. Генерируем случайных индекс слога");
                    int randomSyllableIndex = random.Next(0, countSyllables);

                    //Получаем случайный слог
                    _logger.LogInformation("PersonalNames. GetGeneratingNewName. Получаем случайный слог");
                    randomSyllable = syllablesLower[randomSyllableIndex]!;
                }

                //Добавляем случайный слог в коллекцию
                _logger.LogInformation("PersonalNames. GetGeneratingNewName. Добавляем случайный слог в коллекцию");
                randomSyllables.Add(randomSyllable);
            }

            //Если есть полсдений сло, добавляем в конец
            if (!string.IsNullOrEmpty(lastSyllable))
                randomSyllables.Add(lastSyllable);

            //Объединяем строку
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Объединяем строку");
            string name = string.Join("", randomSyllables.ToArray());

            //Формируем ответ
            _logger.LogInformation("PersonalNames. GetGeneratingNewName. Возвращаем результат");
            return new GeneratedName(true, name, null);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogError("PersonalNames. GetGeneratingNewName. Внутренняя ошибка: {0}", ex);
            return new GeneratedName(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogError("PersonalNames. GetGeneratingNewName. Системная ошибка: {0}", ex);
            return new GeneratedName(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод добавления имени
    /// </summary>
    /// <param name="user"></param>
    /// <param name="nationId"></param>
    /// <param name="gender"></param>
    /// <param name="name"></param>
    /// <param name="probability"></param>
    /// <returns></returns>
    public async Task<BaseResponse> AddName(string? user, long? nationId, bool? gender, string? name,
        double? probability)
    {
        try
        {
            //Объявляем переменную первичного ключа имени
            long id = 0;

            //Получаем экземпляр имени
            Nation? nation = await _repository.Nations.FirstOrDefaultAsync(x => x.Id == nationId);

            //Проверяем входящие переменные
            if (string.IsNullOrEmpty(user))
                throw new InnerException("Не найден текущий пользователь");
            if (nationId == null)
                throw new InnerException("Не указана нация");
            if (gender == null)
                throw new InnerException("Не указан пол");
            if (string.IsNullOrEmpty(name))
                throw new InnerException("Не указано имя");
            if(nation == null)
                throw new InnerException("Не найдена указанная нация");
            if(_repository.PersonalNames.Any(x => x.Name == name))
                throw new InnerException("Указанное имя уже существует");

            //Если не указана частота встречи имени, генерируем её
            if((probability ?? 0) <= 0)
            {
                Random random = new();
                double probabilityRandom = Convert.ToDouble(random.Next(1, 200)) / 100.00;
                probability = Math.Round(probabilityRandom, 2);
            }

            //Открываем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            //Сохраняем данные в базу
            try
            {
                //Формируем экземпляр имени и сохраняем в базу
                PersonalName personalName = new(user, name, gender ?? false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();

                //Формируем экземпляр связи нации с именем
                NationPersonalName nationPersonalName = new(user, probability ?? 0, nation, personalName);
                _repository.NationsPersonalNames.Add(nationPersonalName);
                await _repository.SaveChangesAsync();

                //Фиксируем транзакцию
                await transaction.CommitAsync();

                //Записываем id имени
                id = personalName.Id;
            }
            catch(Exception ex)
            {
                //Откатываем транзакцию
                transaction.Rollback();

                //Прокидываем исключение
                throw new Exception(ex.Message, ex);
            }

            //Формируем ответ
            _logger.LogInformation("PersonalNames. AddName. Возвращаем результат");
            return new BaseResponse(true, id);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            _logger.LogInformation("PersonalNames. AddName. Внутренняя ошибка: {0}", ex);
            return new BaseResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            _logger.LogInformation("PersonalNames. AddName. Системная ошибка: {0}", ex);
            return new BaseResponse(false, new BaseError(500, ex.Message));
        }
    }
}