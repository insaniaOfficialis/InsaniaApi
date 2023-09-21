using AutoMapper;
using Data;
using Domain.Entities.Geography;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Geography.Countries.Request;
using Domain.Models.Geography.Countries.Response;
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
            //Получаем страны с базы
            var countriesDb = await _repository.Countries.ToListAsync() ?? throw new InnerException("Не удалось найти страны в базе");

            //Получаем сущность стран
            var countriesEntity = countriesDb.Where(x => !x.IsDeleted).ToList() 
                ?? throw new InnerException("Не удалось найти не удалённые страны");

            //Преобразовываем модели
            var countries = countriesEntity.Select(_mapper.Map<BaseResponseListItem>).ToList() 
                ?? throw new InnerException("Не удалось преобразовать модель базы данных в модель ответа");

            //Формируем ответ
            return new BaseResponseList(true, null, countries!);
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
    /// Метод получения списка стран с полной информацией
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    public async Task<CountriesResponseList> GetCountriesFullInformation(string? search, int? skip, int? take, List<BaseSortRequest?>? sort,
        bool isDeleted)
    {
        try
        {
            //Строим запрос
            IQueryable<Country> countriesQuery = _repository.Countries;

            //Если передали строку поиска
            if (!String.IsNullOrEmpty(search))
                countriesQuery = countriesQuery.Where(x => x.Name.ToLower().Contains(search.ToLower()));

            //Если передали признак удалённых записей
            if (isDeleted)
                countriesQuery = countriesQuery.Where(x => x.DateDeleted != null);

            //Если передали поле сортировки
            if (sort?.Any() == true)
            {
                //Сортируем по первому элементу сортировки
                IOrderedQueryable<Country> countriesOrderQuery = (sort.FirstOrDefault()!.SortKey, sort.FirstOrDefault()!.IsAscending) switch
                {
                    ("name", true) => countriesQuery.OrderBy(x => x.Name),
                    ("number", true) => countriesQuery.OrderBy(x => x.Number),
                    ("color", true) => countriesQuery.OrderBy(x => x.Color),
                    ("name", false) => countriesQuery.OrderByDescending(x => x.Name),
                    ("number", false) => countriesQuery.OrderByDescending(x => x.Number),
                    ("color", false) => countriesQuery.OrderByDescending(x => x.Color),
                    _ => countriesQuery.OrderBy(x => x.Number),
                };

                //Если есть ещё поля для сортировки
                if (sort.Count > 1)
                {
                    //Проходим по всем элементам сортировки кроме первой
                    foreach (var sortElement in sort.Skip(1))
                    {
                        //Сортируем по каждому элементу
                        countriesOrderQuery = (sortElement!.SortKey, sortElement!.IsAscending) switch
                        {
                            ("name", true) => countriesOrderQuery.ThenBy(x => x.Name),
                            ("number", true) => countriesOrderQuery.ThenBy(x => x.Number),
                            ("color", true) => countriesOrderQuery.ThenBy(x => x.Color),
                            ("name", false) => countriesOrderQuery.ThenByDescending(x => x.Name),
                            ("number", false) => countriesOrderQuery.ThenByDescending(x => x.Number),
                            ("color", false) => countriesOrderQuery.ThenByDescending(x => x.Color),
                            _ => countriesOrderQuery.ThenBy(x => x.Number),
                        };
                    }
                }

                //Приводим в список отсортированный список
                countriesQuery = countriesOrderQuery;
            }

            //Если передали сколько строк пропустить
            if (skip != null)
                countriesQuery = countriesQuery.Skip(skip ?? 0);

            //Если передали сколько строк выводить
            if (take != null)
                countriesQuery = countriesQuery.Take(take ?? 10);

            //Получаем страны с базы
            var countriesDb = await countriesQuery.ToListAsync();

            //Преобразовываем модели
            var countries = countriesDb.Select(_mapper.Map<CountriesResponseListItem>).ToList()
                ?? throw new InnerException("Не удалось преобразовать модель базы данных в модель ответа");

            //Формируем ответ
            return new CountriesResponseList(true, null, countries!);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new CountriesResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new CountriesResponseList(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод добавления страны
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<BaseResponse> AddCountry(AddCountryRequest? request, string? user)
    {
        try
        {
            long id; //id страны

            //Проверяем корректность данных
            if (request == null)
                throw new InnerException("Пустой запрос");
            if (String.IsNullOrEmpty(request.Name))
                throw new InnerException("Не указано наименование");
            if (String.IsNullOrEmpty(request.Color))
                throw new InnerException("Не указан цвет");
            if (request.Number == null)
                throw new InnerException("Не указан номер на карте");
            if (_repository.Countries.Any(x => x.Color == request.Color))
                throw new InnerException("Данный цвет уже используется");
            if (_repository.Countries.Any(x => x.Number == request.Number))
                throw new InnerException("Данный номер уже используется");
            if (String.IsNullOrEmpty(user))
                throw new InnerException("Пользователь незарегистрирован");

            //Начинаем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                //Объявляем новую страну и записываем его в базу
                Country country = new(user, request.Name, request.Number ?? 0, request.Color, request.LanguageForNames);
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();

                //Фиксируем транзакцию
                transaction.Commit();

                //Записывае id для вывода
                id = country.Id;
            }
            //Обрабатываем системные исключения
            catch (Exception ex)
            {
                //Откатываем транзакцию
                transaction.Rollback();

                //Вызываем исключение
                throw new Exception(ex.Message, ex);
            }

            //Возвращаем результат
            return new BaseResponse(true, id);
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
    /// Метод обновления страны
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<BaseResponse> UpdateCountry(AddCountryRequest? request, string? user, long? id)
    {
        try
        {
            //Проверяем корректность данных
            if (request == null)
                throw new InnerException("Пустой запрос");
            if (id == null)
                throw new InnerException("Не указан id записи");
            if (String.IsNullOrEmpty(user))
                throw new InnerException("Пользователь не зарегистрирован");
            if (!String.IsNullOrEmpty(request.Color) && _repository.Countries.Any(x => x.Color == request.Color && x.Id != id))
                throw new InnerException("Данный цвет уже используется");
            if (request.Number != null && _repository.Countries.Any(x => x.Number == request.Number && x.Id != id))
                throw new InnerException("Данный номер уже используется");

            //Начинаем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                //Находим страну в базе
                Country country = _repository.Countries.FirstOrDefault(x => x.Id == id)
                    ?? throw new InnerException("Страна не найдена в базе");

                //Если указано наименование и оно отличается от того, что есть в базе
                if (request.Name != null && request.Name != country.Name)
                    //Устанавливаем наименование
                    country.SetName(request.Name);

                //Если указан цвет и он отличается от того, что есть в базе
                if (request.Color != null && request.Color != country.Color)
                    //Устанавливаем цвет
                    country.SetColor(request.Color);

                //Если указан цвет и он отличается от того, что есть в базе
                if (request.Number != null && request.Number != country.Number)
                    //Устанавливаем цвет
                    country.SetNumber(request.Number ?? 0);

                //Если указан язык для наименований и он отличается от того, что есть в базе
                if (request.LanguageForNames != null && request.LanguageForNames != country.LanguageForNames)
                    //Устанавливаем язык для наименований
                    country.SetLanguageForNames(request.LanguageForNames);

                //Устанавливаем дату обновления
                country.SetUpdate(user);

                //Обновляем данные в базе
                _repository.Countries.Update(country);
                await _repository.SaveChangesAsync();

                //Фиксируем транзакцию
                transaction.Commit();
            }
            //Обрабатываем системные исключения
            catch (Exception ex)
            {
                //Откатываем транзакцию
                transaction.Rollback();

                //Вызываем исключение
                throw new Exception(ex.Message, ex);
            }

            //Возвращаем результат
            return new BaseResponse(true);
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
    /// Метод обновления удалённости данных
    /// </summary>
    /// <param name="delete"></param>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    public async Task<BaseResponse> UpdateCountry(bool? delete, long? id, string? user)
    {
        try
        {
            //Проверяем корректность данных
            if (delete == null)
                throw new InnerException("Не указан признак удаления/восстановления данных");
            if (id == null)
                throw new InnerException("Не указан id записи");
            if (String.IsNullOrEmpty(user))
                throw new InnerException("Пользователь не зарегистрирован");

            //Начинаем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                //Находим страну в базе
                Country country = _repository.Countries.FirstOrDefault(x => x.Id == id)
                    ?? throw new InnerException("Страна не найдена в базе");

                //Проверяем удаляем или восстановливаем запись
                if (delete == true)
                    country.SetDeleted();
                else
                    country.SetRestored();

                //Устанавливаем дату обновления
                country.SetUpdate(user);

                //Обновляем данные в базе
                _repository.Countries.Update(country);
                await _repository.SaveChangesAsync();

                //Фиксируем транзакцию
                transaction.Commit();
            }
            //Обрабатываем системные исключения
            catch (Exception ex)
            {
                //Откатываем транзакцию
                transaction.Rollback();

                //Вызываем исключение
                throw new Exception(ex.Message, ex);
            }

            //Возвращаем результат
            return new BaseResponse(true);
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
}
