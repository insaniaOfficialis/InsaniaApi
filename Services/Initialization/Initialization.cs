using Data;
using Domain.Entities.General.File;
using Domain.Entities.General.System;
using Domain.Entities.Politics;
using Domain.Entities.Identification;
using Domain.Models.Exclusion;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Domain.Entities.Geography;
using Domain.Entities.Sociology;
using Microsoft.Extensions.Configuration;

namespace Services.Initialization;

/// <summary>
/// Сервис инициализации
/// </summary>
public class Initialization : IInitialization
{
    private readonly RoleManager<Role> _roleManager; //менеджер пользователей
    private readonly UserManager<User> _userManager; //менеджер пользователей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly ILogger<Initialization> _logger; //сервис для записи логов
    public readonly IConfiguration _configuration; //интерфейс конфигурации

    /// <summary>
    /// Конструктор сервиса инициализации
    /// </summary>
    /// <param name="roleManager"></param>
    /// <param name="userManager"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    /// <param name="configuration"></param>
    public Initialization(RoleManager<Role> roleManager, UserManager<User> userManager, ApplicationContext repository, ILogger<Initialization> logger,
        IConfiguration configuration)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _repository = repository;
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>
    /// Метод инициализации базы данных
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> InitializeDatabase()
    {
        try
        {
            //РОЛИ
            if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeRoles"]))
            {
                //Проверяем наличие роли админа
                if (_roleManager.FindByNameAsync("admin").Result == null)
                {
                    //Добавляем роль админа
                    Role role = new("admin");
                    var resultRole = await _roleManager.CreateAsync(role) ?? throw new Exception("Не удалось создать роль");

                    //Если не успешно, выдаём ошибку
                    if (!resultRole.Succeeded)
                        throw new Exception(resultRole?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
                }
            }

            //ПОЛЬЗОВАТЕЛИ
            if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeUsers"]))
            {
                //Проверяем наличие пользователя инсании
                if (_userManager.FindByNameAsync("insania").Result == null)
                {
                    //Добавляем пользователя инсании
                    User user = new("insania", "insania_officialis@vk.com", "+79996370439", false);
                    var resultUser = await _userManager.CreateAsync(user, "K02032018v.") ?? throw new Exception("Не удалось создать пользователя");

                    //Если успешно
                    if (resultUser.Succeeded)
                    {
                        //Добавляем к инсании роль админа
                        var resultUserRole = await _userManager.AddToRoleAsync(user, "admin") ?? throw new InnerException("Не удалось добавить роль пользователю");

                        //Если не успешно, выдаём ошибку
                        if (!resultUserRole.Succeeded)
                            throw new InnerException(resultUserRole?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
                    }
                    //Иначе выдаём ошибку
                    else
                        throw new Exception(resultUser?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
                }
            }

            //Открываем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeParametrs"]))
                {
                    //ПАРАМЕТРЫ
                    await InitializeParametrs();
                }

                //ТИПЫ ФАЙЛОВ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeFileTypes"]))
                {
                    await InitializeFileTypes();
                }

                //КЛИМАТЫ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeClimates"]))
                {
                    await InitializeClimates();
                }

                //РЕЛЬЕФЫ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeTerrains"]))
                {
                    await InitializeTerrains();
                }

                //РАСЫ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeRaces"]))
                {
                    await InitializeRaces();
                }

                //НАЦИИ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeNations"]))
                {
                    await InitializeNations();
                }

                //ИМЕНА
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializePersonalNames"]))
                {
                    await InitializePersonalNames();
                }

                //ФАМИЛИИ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeLastNames"]))
                {
                    await InitializeLastNames();
                }

                //ПРЕФИКСЫ ИМЁН
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializePrefixNames"]))
                {
                    await InitializePrefixNames();
                }

                //СВЯЗЬ ИМЁН С НАЦИЯМИ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeNationsPersonalNames"]))
                {
                    await InitializeNationsPersonalNames();
                }

                //СВЯЗЬ ФАМИЛИЙ С НАЦИЯМИ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeNationsLastNames"]))
                {
                    await InitializeNationsLastNames();
                }

                //СВЯЗЬ ПРЕФИКСОВ С НАЦИЯМИ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeNationsPrefixNames"]))
                {
                    await InitializeNationsPrefixNames();
                }

                //СТРАНЫ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeCountries"]))
                {
                    await InitializeCountries();
                }

                //РЕГИОНЫ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeRegions"]))
                {
                    await InitializeRegions();
                }

                //ТИПЫ НАСЕЛЁННЫХ ПУНКТОВ
                if (Convert.ToBoolean(_configuration["InitializeOptions:InitializeTypesSettlements"]))
                {
                    await InitializeTypesSettlements();
                }

                //Фиксируем транзакцию
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("InitializeDatabase. Ошибка: " + ex.Message);
                await transaction.RollbackAsync();
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("InitializeDatabase. Ошибка: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Метод инициализации параметров
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeParametrs()
    {
        try
        {
            //Проверяем наличие параметра цвета чисел на карте
            if (!_repository.Parametrs.Any(x => x.Name == "Цвет чисел на карте"))
            {
                //Создаём параметр для цвета чисел на карте
                Parametr parametr = new("system", "Цвет чисел на карте", "#7E0000");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие параметра шрифт чисел на карте
            if (!_repository.Parametrs.Any(x => x.Name == "Шрифт чисел на карте"))
            {
                //Создаём параметр для шрифта чисел на карте
                Parametr parametr = new("system", "Шрифт чисел на карте", "Times New Roman");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие параметра размера чисел стран на карте
            if (!_repository.Parametrs.Any(x => x.Name == "Размер чисел стран на карте"))
            {
                //Создаём параметр для размера чисел стран на карте
                Parametr parametr = new("system", "Размер чисел стран на карте", "80");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие параметра размера чисел регионов на карте
            if (!_repository.Parametrs.Any(x => x.Name == "Размер чисел регионов на карте"))
            {
                //Создаём параметр для размера чисел регионов на карте
                Parametr parametr = new("system", "Размер чисел регионов на карте", "20");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие параметра размера чисел областей на карте
            if (!_repository.Parametrs.Any(x => x.Name == "Размер чисел областей на карте"))
            {
                //Создаём параметр для размера чисел областей на карте
                Parametr parametr = new("system", "Размер чисел областей на карте", "10");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие параметра цвета границ областей на карте
            if (!_repository.Parametrs.Any(x => x.Name == "Цвет границ областей на карте"))
            {
                //Создаём параметр для цвета границ областей на карте
                Parametr parametr = new("system", "Цвет границ областей на карте", "#5C5C5C");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие параметра размера пикселей в квадратных километрах
            if (!_repository.Parametrs.Any(x => x.Name == "Размер пикселей в квадратных километрах"))
            {
                //Создаём параметр для размера пикселей в квадратных километрах
                Parametr parametr = new("system", "Размер пикселей в квадратных километрах", "1.69");
                _repository.Parametrs.Add(parametr);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeParametrs. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации типов файлов
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeFileTypes()
    {
        try
        {
            //Проверяем наличие типа файла для пользователей
            if (!_repository.FileTypes.Any(x => x.Name == "Пользователь"))
            {
                //Создаём тип файла для пользователей
                FileType fileType = new("system", "Пользователь", "I:\\Insania\\ПО\\Files");
                _repository.FileTypes.Add(fileType);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeFileTypes. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации климатов
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeClimates()
    {
        try
        {
            //Проверяем наличие климата "Полярный"
            if (!_repository.Climates.Any(x => x.Name == "Полярный"))
            {
                //Создаём климат "Полярный"
                Climate climate = new("system", "Полярный");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие климата "Субполярный"
            if (!_repository.Climates.Any(x => x.Name == "Субполярный"))
            {
                //Создаём климат "Субполярный"
                Climate climate = new("system", "Субполярный");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие климата "Умеренный"
            if (!_repository.Climates.Any(x => x.Name == "Умеренный"))
            {
                //Создаём климат "Умеренный"
                Climate climate = new("system", "Умеренный");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие климата "Субтропический"
            if (!_repository.Climates.Any(x => x.Name == "Субтропический"))
            {
                //Создаём климат "Субтропический"
                Climate climate = new("system", "Субтропический");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие климата "Тропический"
            if (!_repository.Climates.Any(x => x.Name == "Тропический"))
            {
                //Создаём климат "Тропический"
                Climate climate = new("system", "Тропический");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие климата "Субэкваториальный"
            if (!_repository.Climates.Any(x => x.Name == "Субэкваториальный"))
            {
                //Создаём климат "Субэкваториальный"
                Climate climate = new("system", "Субэкваториальный");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие климата "Экваториальный"
            if (!_repository.Climates.Any(x => x.Name == "Экваториальный"))
            {
                //Создаём климат "Экваториальный"
                Climate climate = new("system", "Экваториальный");
                _repository.Climates.Add(climate);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeClimates. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации рельефов
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeTerrains()
    {
        try
        {
            //Проверяем наличие рельефа "Горный"
            if (!_repository.Terrains.Any(x => x.Name == "Горный"))
            {
                //Создаём рельеф "Горный"
                Terrain terrain = new("system", "Горный");
                _repository.Terrains.Add(terrain);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие рельефа "Холмистый"
            if (!_repository.Terrains.Any(x => x.Name == "Холмистый"))
            {
                //Создаём рельеф "Холмистый"
                Terrain terrain = new("system", "Холмистый");
                _repository.Terrains.Add(terrain);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие рельефа "Лесистый"
            if (!_repository.Terrains.Any(x => x.Name == "Лесистый"))
            {
                //Создаём рельеф "Лесистый"
                Terrain terrain = new("system", "Лесистый");
                _repository.Terrains.Add(terrain);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие рельефа "Равнинный"
            if (!_repository.Terrains.Any(x => x.Name == "Равнинный"))
            {
                //Создаём рельеф "Равнинный"
                Terrain terrain = new("system", "Равнинный");
                _repository.Terrains.Add(terrain);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeTerrains. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации рас
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeRaces()
    {
        try
        {
            //Проверяем наличие расы "Альв"
            if (!_repository.Races.Any(x => x.Name == "Альв"))
            {
                //Создаём расу "Альв"
                Race race = new("system", "Альв");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Мраат"
            if (!_repository.Races.Any(x => x.Name == "Мраат"))
            {
                //Создаём расу "Мраат"
                Race race = new("system", "Мраат");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Человек"
            if (!_repository.Races.Any(x => x.Name == "Человек"))
            {
                //Создаём расу "Человек"
                Race race = new("system", "Человек");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Троль"
            if (!_repository.Races.Any(x => x.Name == "Троль"))
            {
                //Создаём расу "Троль"
                Race race = new("system", "Троль");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Вампир"
            if (!_repository.Races.Any(x => x.Name == "Вампир"))
            {
                //Создаём расу "Вампир"
                Race race = new("system", "Вампир");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Орк"
            if (!_repository.Races.Any(x => x.Name == "Орк"))
            {
                //Создаём расу "Орк"
                Race race = new("system", "Орк");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Ихтид"
            if (!_repository.Races.Any(x => x.Name == "Ихтид"))
            {
                //Создаём расу "Ихтид"
                Race race = new("system", "Ихтид");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Дворф"
            if (!_repository.Races.Any(x => x.Name == "Дворф"))
            {
                //Создаём расу "Дворф"
                Race race = new("system", "Дворф");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Дану"
            if (!_repository.Races.Any(x => x.Name == "Дану"))
            {
                //Создаём расу "Дану"
                Race race = new("system", "Дану");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Эльф"
            if (!_repository.Races.Any(x => x.Name == "Эльф"))
            {
                //Создаём расу "Эльф"
                Race race = new("system", "Эльф");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Гоблин"
            if (!_repository.Races.Any(x => x.Name == "Гоблин"))
            {
                //Создаём расу "Гоблин"
                Race race = new("system", "Гоблин");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Огр"
            if (!_repository.Races.Any(x => x.Name == "Огр"))
            {
                //Создаём расу "Огр"
                Race race = new("system", "Огр");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Оборотень"
            if (!_repository.Races.Any(x => x.Name == "Оборотень"))
            {
                //Создаём расу "Оборотень"
                Race race = new("system", "Оборотень");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Элвин"
            if (!_repository.Races.Any(x => x.Name == "Элвин"))
            {
                //Создаём расу "Элвин"
                Race race = new("system", "Элвин");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие расы "Антропозавр"
            if (!_repository.Races.Any(x => x.Name == "Антропозавр"))
            {
                //Создаём расу "Антропозавр"
                Race race = new("system", "Антропозавр");
                _repository.Races.Add(race);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeRaces. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации наций
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeNations()
    {
        try
        {
            //Проверяем наличие расы "Альв"
            if (_repository.Races.Any(x => x.Name == "Альв"))
            {
                //Получаем расу "Альв"
                Race? race = _repository.Races.FirstOrDefault(x => x.Name == "Альв");

                //Проверяем наличие расы "Альв"
                if (race != null)
                {
                    //Проверяем наличие нации "Альв"
                    if (!_repository.Nations.Any(x => x.Name == "Альв"))
                    {
                        //Создаём нацию "Альв"
                        Nation nation = new("system", "Альв", race, "Эльфийский");
                        _repository.Nations.Add(nation);
                        await _repository.SaveChangesAsync();
                    }
                }
            }

            //Проверяем наличие расы "Мраат"
            if (_repository.Races.Any(x => x.Name == "Мраат"))
            {
                //Получаем расу "Мраат"
                Race? race = _repository.Races.FirstOrDefault(x => x.Name == "Мраат");

                //Проверяем наличие расы "Мраат"
                if (race != null)
                {
                    //Проверяем наличие нации "Цивилизованный мраат"
                    if (!_repository.Nations.Any(x => x.Name == "Цивилизованный мраат"))
                    {
                        //Создаём нацию "Цивилизованный мраат"
                        Nation nation = new("system", "Цивилизованный мраат", race, "Одно- двухслоговые с обилием твёрдых согласных");
                        _repository.Nations.Add(nation);
                        await _repository.SaveChangesAsync();
                    }

                    //Проверяем наличие нации "Дикий мраат"
                    if (!_repository.Nations.Any(x => x.Name == "Дикий мраат"))
                    {
                        //Создаём нацию "Дикий мраат"
                        Nation nation = new("system", "Дикий мраат", race, "Мыслеообразы подобные индейским прозвищам");
                        _repository.Nations.Add(nation);
                        await _repository.SaveChangesAsync();
                    }
                }
            }

            //Проверяем наличие расы "Вампир"
            if (_repository.Races.Any(x => x.Name == "Вампир"))
            {
                //Получаем расу "Вампир"
                Race? race = _repository.Races.FirstOrDefault(x => x.Name == "Вампир");

                //Проверяем наличие расы "Вампир"
                if (race != null)
                {
                    //Проверяем наличие нации "Западный вампир"
                    if (!_repository.Nations.Any(x => x.Name == "Западный вампир"))
                    {
                        //Создаём нацию "Западный вампир"
                        Nation nation = new("system", "Западный вампир", race, "Шотландский");
                        _repository.Nations.Add(nation);
                        await _repository.SaveChangesAsync();
                    }

                    //Проверяем наличие нации "Восточный вампир"
                    if (!_repository.Nations.Any(x => x.Name == "Восточный вампир"))
                    {
                        //Создаём нацию "Восточный вампир"
                        Nation nation = new("system", "Восточный вампир", race, "Гэльский");
                        _repository.Nations.Add(nation);
                        await _repository.SaveChangesAsync();
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeNations. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации персональных имён
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializePersonalNames()
    {
        try
        {
            //Проверяем наличие имени "Амакир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Амакир"))
            {
                //Создаём имя "Амакир"
                PersonalName personalName = new("system", "Амакир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арден"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арден"))
            {
                //Создаём имя "Арден"
                PersonalName personalName = new("system", "Арден", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Амамион"
            if (!_repository.PersonalNames.Any(x => x.Name == "Амамион"))
            {
                //Создаём имя "Амамион"
                PersonalName personalName = new("system", "Амамион", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хайден"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хайден"))
            {
                //Создаём имя "Хайден"
                PersonalName personalName = new("system", "Хайден", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Халиодар"
            if (!_repository.PersonalNames.Any(x => x.Name == "Халиодар"))
            {
                //Создаём имя "Халиодар"
                PersonalName personalName = new("system", "Халиодар", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рарнолил"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рарнолил"))
            {
                //Создаём имя "Рарнолил"
                PersonalName personalName = new("system", "Рарнолил", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эдоен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эдоен"))
            {
                //Создаём имя "Эдоен"
                PersonalName personalName = new("system", "Эдоен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аридир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аридир"))
            {
                //Создаём имя "Аридир"
                PersonalName personalName = new("system", "Аридир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Леневелон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Леневелон"))
            {
                //Создаём имя "Леневелон"
                PersonalName personalName = new("system", "Леневелон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хальнитрен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хальнитрен"))
            {
                //Создаём имя "Хальнитрен"
                PersonalName personalName = new("system", "Хальнитрен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хальнасай"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хальнасай"))
            {
                //Создаём имя "Хальнасай"
                PersonalName personalName = new("system", "Хальнасай", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фалерил"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фалерил"))
            {
                //Создаём имя "Фалерил"
                PersonalName personalName = new("system", "Фалерил", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кайнаэль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кайнаэль"))
            {
                //Создаём имя "Кайнаэль"
                PersonalName personalName = new("system", "Кайнаэль", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Вален"
            if (!_repository.PersonalNames.Any(x => x.Name == "Вален"))
            {
                //Создаём имя "Вален"
                PersonalName personalName = new("system", "Вален", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фесолор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фесолор"))
            {
                //Создаём имя "Фесолор"
                PersonalName personalName = new("system", "Фесолор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фенменор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фенменор"))
            {
                //Создаём имя "Фенменор"
                PersonalName personalName = new("system", "Фенменор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Галакир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Галакир"))
            {
                //Создаём имя "Галакир"
                PersonalName personalName = new("system", "Галакир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эарониан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эарониан"))
            {
                //Создаём имя "Эарониан"
                PersonalName personalName = new("system", "Эарониан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аматал"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аматал"))
            {
                //Создаём имя "Аматал"
                PersonalName personalName = new("system", "Аматал", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кинрел"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кинрел"))
            {
                //Создаём имя "Кинрел"
                PersonalName personalName = new("system", "Кинрел", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рававарис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рававарис"))
            {
                //Создаём имя "Рававарис"
                PersonalName personalName = new("system", "Рававарис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кифарен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кифарен"))
            {
                //Создаём имя "Кифарен"
                PersonalName personalName = new("system", "Кифарен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тингол"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тингол"))
            {
                //Создаём имя "Тингол"
                PersonalName personalName = new("system", "Тингол", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тирон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тирон"))
            {
                //Создаём имя "Тирон"
                PersonalName personalName = new("system", "Тирон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ангрод"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ангрод"))
            {
                //Создаём имя "Ангрод"
                PersonalName personalName = new("system", "Ангрод", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бауглир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бауглир"))
            {
                //Создаём имя "Бауглир"
                PersonalName personalName = new("system", "Бауглир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гундор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гундор"))
            {
                //Создаём имя "Гундор"
                PersonalName personalName = new("system", "Гундор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элурад"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элурад"))
            {
                //Создаём имя "Элурад"
                PersonalName personalName = new("system", "Элурад", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ирмо"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ирмо"))
            {
                //Создаём имя "Ирмо"
                PersonalName personalName = new("system", "Ирмо", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ваньяр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ваньяр"))
            {
                //Создаём имя "Ваньяр"
                PersonalName personalName = new("system", "Ваньяр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анайрэ"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анайрэ"))
            {
                //Создаём имя "Анайрэ"
                PersonalName personalName = new("system", "Анайрэ", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Галион"
            if (!_repository.PersonalNames.Any(x => x.Name == "Галион"))
            {
                //Создаём имя "Галион"
                PersonalName personalName = new("system", "Галион", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иминиэ"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иминиэ"))
            {
                //Создаём имя "Иминиэ"
                PersonalName personalName = new("system", "Иминиэ", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Индис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Индис"))
            {
                //Создаём имя "Индис"
                PersonalName personalName = new("system", "Индис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Маэдрос"
            if (!_repository.PersonalNames.Any(x => x.Name == "Маэдрос"))
            {
                //Создаём имя "Маэдрос"
                PersonalName personalName = new("system", "Маэдрос", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Неллас"
            if (!_repository.PersonalNames.Any(x => x.Name == "Неллас"))
            {
                //Создаём имя "Неллас"
                PersonalName personalName = new("system", "Неллас", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Румил"
            if (!_repository.PersonalNames.Any(x => x.Name == "Румил"))
            {
                //Создаём имя "Румил"
                PersonalName personalName = new("system", "Румил", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Феанор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Феанор"))
            {
                //Создаём имя "Феанор"
                PersonalName personalName = new("system", "Феанор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Энель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Энель"))
            {
                //Создаём имя "Энель"
                PersonalName personalName = new("system", "Энель", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эльмо"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эльмо"))
            {
                //Создаём имя "Эльмо"
                PersonalName personalName = new("system", "Эльмо", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Цислейрис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Цислейрис"))
            {
                //Создаём имя "Цислейрис"
                PersonalName personalName = new("system", "Цислейрис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Саотан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Саотан"))
            {
                //Создаём имя "Саотан"
                PersonalName personalName = new("system", "Саотан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эстра"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эстра"))
            {
                //Создаём имя "Эстра"
                PersonalName personalName = new("system", "Эстра", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Друзилла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Друзилла"))
            {
                //Создаём имя "Друзилла"
                PersonalName personalName = new("system", "Друзилла", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эрис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эрис"))
            {
                //Создаём имя "Эрис"
                PersonalName personalName = new("system", "Эрис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глинда"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глинда"))
            {
                //Создаём имя "Глинда"
                PersonalName personalName = new("system", "Глинда", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фасьенн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фасьенн"))
            {
                //Создаём имя "Фасьенн"
                PersonalName personalName = new("system", "Фасьенн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Виэслана"
            if (!_repository.PersonalNames.Any(x => x.Name == "Виэслана"))
            {
                //Создаём имя "Виэслана"
                PersonalName personalName = new("system", "Виэслана", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Верития"
            if (!_repository.PersonalNames.Any(x => x.Name == "Верития"))
            {
                //Создаём имя "Верития"
                PersonalName personalName = new("system", "Верития", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фейнтелин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фейнтелин"))
            {
                //Создаём имя "Фейнтелин"
                PersonalName personalName = new("system", "Фейнтелин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Велая"
            if (!_repository.PersonalNames.Any(x => x.Name == "Велая"))
            {
                //Создаём имя "Велая"
                PersonalName personalName = new("system", "Велая", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Корлейна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Корлейна"))
            {
                //Создаём имя "Корлейна"
                PersonalName personalName = new("system", "Корлейна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Верозания"
            if (!_repository.PersonalNames.Any(x => x.Name == "Верозания"))
            {
                //Создаём имя "Верозания"
                PersonalName personalName = new("system", "Верозания", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кайэсния"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кайэсния"))
            {
                //Создаём имя "Кайэсния"
                PersonalName personalName = new("system", "Кайэсния", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элинбель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элинбель"))
            {
                //Создаём имя "Элинбель"
                PersonalName personalName = new("system", "Элинбель", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фенелла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фенелла"))
            {
                //Создаём имя "Фенелла"
                PersonalName personalName = new("system", "Фенелла", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Белилия"
            if (!_repository.PersonalNames.Any(x => x.Name == "Белилия"))
            {
                //Создаём имя "Белилия"
                PersonalName personalName = new("system", "Белилия", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Беллами"
            if (!_repository.PersonalNames.Any(x => x.Name == "Беллами"))
            {
                //Создаём имя "Беллами"
                PersonalName personalName = new("system", "Беллами", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эмери"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эмери"))
            {
                //Создаём имя "Эмери"
                PersonalName personalName = new("system", "Эмери", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гарральэль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гарральэль"))
            {
                //Создаём имя "Гарральэль"
                PersonalName personalName = new("system", "Гарральэль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Холалет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Холалет"))
            {
                //Создаём имя "Холалет"
                PersonalName personalName = new("system", "Холалет", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ветрокрылая"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ветрокрылая"))
            {
                //Создаём имя "Ветрокрылая"
                PersonalName personalName = new("system", "Ветрокрылая", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Идриль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Идриль"))
            {
                //Создаём имя "Идриль"
                PersonalName personalName = new("system", "Идриль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арене"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арене"))
            {
                //Создаём имя "Арене"
                PersonalName personalName = new("system", "Арене", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Амани"
            if (!_repository.PersonalNames.Any(x => x.Name == "Амани"))
            {
                //Создаём имя "Амани"
                PersonalName personalName = new("system", "Амани", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иримэ"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иримэ"))
            {
                //Создаём имя "Иримэ"
                PersonalName personalName = new("system", "Иримэ", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Индис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Индис"))
            {
                //Создаём имя "Индис"
                PersonalName personalName = new("system", "Индис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ниенор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ниенор"))
            {
                //Создаём имя "Ниенор"
                PersonalName personalName = new("system", "Ниенор", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Нерданель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Нерданель"))
            {
                //Создаём имя "Нерданель"
                PersonalName personalName = new("system", "Нерданель", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мормегиль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мормегиль"))
            {
                //Создаём имя "Мормегиль"
                PersonalName personalName = new("system", "Мормегиль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Вана"
            if (!_repository.PersonalNames.Any(x => x.Name == "Вана"))
            {
                //Создаём имя "Вана"
                PersonalName personalName = new("system", "Вана", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эйлинель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эйлинель"))
            {
                //Создаём имя "Эйлинель"
                PersonalName personalName = new("system", "Эйлинель", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эльвинг"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эльвинг"))
            {
                //Создаём имя "Эльвинг"
                PersonalName personalName = new("system", "Эльвинг", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элентари"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элентари"))
            {
                //Создаём имя "Элентари"
                PersonalName personalName = new("system", "Элентари", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Уртель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Уртель"))
            {
                //Создаём имя "Уртель"
                PersonalName personalName = new("system", "Уртель", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Валандиль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Валандиль"))
            {
                //Создаём имя "Валандиль"
                PersonalName personalName = new("system", "Валандиль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рана"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рана"))
            {
                //Создаём имя "Рана"
                PersonalName personalName = new("system", "Рана", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Миримэ"
            if (!_repository.PersonalNames.Any(x => x.Name == "Миримэ"))
            {
                //Создаём имя "Миримэ"
                PersonalName personalName = new("system", "Миримэ", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анариэль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анариэль"))
            {
                //Создаём имя "Анариэль"
                PersonalName personalName = new("system", "Анариэль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элениэль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элениэль"))
            {
                //Создаём имя "Элениэль"
                PersonalName personalName = new("system", "Элениэль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Миримэ"
            if (!_repository.PersonalNames.Any(x => x.Name == "Миримэ"))
            {
                //Создаём имя "Миримэ"
                PersonalName personalName = new("system", "Миримэ", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анкалимэ"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анкалимэ"))
            {
                //Создаём имя "Анкалимэ"
                PersonalName personalName = new("system", "Анкалимэ", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эариль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эариль"))
            {
                //Создаём имя "Эариль"
                PersonalName personalName = new("system", "Эариль", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аркуэне"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аркуэне"))
            {
                //Создаём имя "Аркуэне"
                PersonalName personalName = new("system", "Аркуэне", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Таурэтари"
            if (!_repository.PersonalNames.Any(x => x.Name == "Таурэтари"))
            {
                //Создаём имя "Таурэтари"
                PersonalName personalName = new("system", "Таурэтари", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эдделос"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эдделос"))
            {
                //Создаём имя "Эдделос"
                PersonalName personalName = new("system", "Эдделос", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Алан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Алан"))
            {
                //Создаём имя "Алан"
                PersonalName personalName = new("system", "Алан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аластер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аластер"))
            {
                //Создаём имя "Аластер"
                PersonalName personalName = new("system", "Аластер", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Албан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Албан"))
            {
                //Создаём имя "Албан"
                PersonalName personalName = new("system", "Албан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Алпин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Алпин"))
            {
                //Создаём имя "Алпин"
                PersonalName personalName = new("system", "Алпин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ангус"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ангус"))
            {
                //Создаём имя "Ангус"
                PersonalName personalName = new("system", "Ангус", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Блан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Блан"))
            {
                //Создаём имя "Блан"
                PersonalName personalName = new("system", "Блан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Блейер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Блейер"))
            {
                //Создаём имя "Блейер"
                PersonalName personalName = new("system", "Блейер", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бойд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бойд"))
            {
                //Создаём имя "Бойд"
                PersonalName personalName = new("system", "Бойд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Вильям"
            if (!_repository.PersonalNames.Any(x => x.Name == "Вильям"))
            {
                //Создаём имя "Вильям"
                PersonalName personalName = new("system", "Вильям", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дерек"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дерек"))
            {
                //Создаём имя "Дерек"
                PersonalName personalName = new("system", "Дерек", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Джек"
            if (!_repository.PersonalNames.Any(x => x.Name == "Джек"))
            {
                //Создаём имя "Джек"
                PersonalName personalName = new("system", "Джек", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Джиллиан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Джиллиан"))
            {
                //Создаём имя "Джиллиан"
                PersonalName personalName = new("system", "Джиллиан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дональд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дональд"))
            {
                //Создаём имя "Дональд"
                PersonalName personalName = new("system", "Дональд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дугал"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дугал"))
            {
                //Создаём имя "Дугал"
                PersonalName personalName = new("system", "Дугал", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дуглас"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дуглас"))
            {
                //Создаём имя "Дуглас"
                PersonalName personalName = new("system", "Дуглас", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дункан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дункан"))
            {
                //Создаём имя "Дункан"
                PersonalName personalName = new("system", "Дункан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гевин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гевин"))
            {
                //Создаём имя "Гевин"
                PersonalName personalName = new("system", "Гевин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гиллис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гиллис"))
            {
                //Создаём имя "Гиллис"
                PersonalName personalName = new("system", "Гиллис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гордон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гордон"))
            {
                //Создаём имя "Гордон"
                PersonalName personalName = new("system", "Гордон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Грир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Грир"))
            {
                //Создаём имя "Грир"
                PersonalName personalName = new("system", "Грир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Камерон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Камерон"))
            {
                //Создаём имя "Камерон"
                PersonalName personalName = new("system", "Камерон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Клайд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Клайд"))
            {
                //Создаём имя "Клайд"
                PersonalName personalName = new("system", "Клайд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кеннет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кеннет"))
            {
                //Создаём имя "Кеннет"
                PersonalName personalName = new("system", "Кеннет", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Колин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Колин"))
            {
                //Создаём имя "Колин"
                PersonalName personalName = new("system", "Колин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Крег"
            if (!_repository.PersonalNames.Any(x => x.Name == "Крег"))
            {
                //Создаём имя "Крег"
                PersonalName personalName = new("system", "Крег", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лахлан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лахлан"))
            {
                //Создаём имя "Лахлан"
                PersonalName personalName = new("system", "Лахлан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лорн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лорн"))
            {
                //Создаём имя "Лорн"
                PersonalName personalName = new("system", "Лорн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Льюс"
            if (!_repository.PersonalNames.Any(x => x.Name == "Льюс"))
            {
                //Создаём имя "Льюс"
                PersonalName personalName = new("system", "Льюс", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Магнус"
            if (!_repository.PersonalNames.Any(x => x.Name == "Магнус"))
            {
                //Создаём имя "Магнус"
                PersonalName personalName = new("system", "Магнус", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Малькольм"
            if (!_repository.PersonalNames.Any(x => x.Name == "Малькольм"))
            {
                //Создаём имя "Малькольм"
                PersonalName personalName = new("system", "Малькольм", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Миррей"
            if (!_repository.PersonalNames.Any(x => x.Name == "Миррей"))
            {
                //Создаём имя "Миррей"
                PersonalName personalName = new("system", "Миррей", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Нейл"
            if (!_repository.PersonalNames.Any(x => x.Name == "Нейл"))
            {
                //Создаём имя "Нейл"
                PersonalName personalName = new("system", "Нейл", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Невин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Невин"))
            {
                //Создаём имя "Невин"
                PersonalName personalName = new("system", "Невин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Олей"
            if (!_repository.PersonalNames.Any(x => x.Name == "Олей"))
            {
                //Создаём имя "Олей"
                PersonalName personalName = new("system", "Олей", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Райтор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Райтор"))
            {
                //Создаём имя "Райтор"
                PersonalName personalName = new("system", "Райтор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ренальд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ренальд"))
            {
                //Создаём имя "Ренальд"
                PersonalName personalName = new("system", "Ренальд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рой"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рой"))
            {
                //Создаём имя "Рой"
                PersonalName personalName = new("system", "Рой", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рори"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рори"))
            {
                //Создаём имя "Рори"
                PersonalName personalName = new("system", "Рори", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Росс"
            if (!_repository.PersonalNames.Any(x => x.Name == "Росс"))
            {
                //Создаём имя "Росс"
                PersonalName personalName = new("system", "Росс", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Стюарт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Стюарт"))
            {
                //Создаём имя "Стюарт"
                PersonalName personalName = new("system", "Стюарт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тевиш"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тевиш"))
            {
                //Создаём имя "Тевиш"
                PersonalName personalName = new("system", "Тевиш", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Уоллес"
            if (!_repository.PersonalNames.Any(x => x.Name == "Уоллес"))
            {
                //Создаём имя "Уоллес"
                PersonalName personalName = new("system", "Уоллес", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Шолто"
            if (!_repository.PersonalNames.Any(x => x.Name == "Шолто"))
            {
                //Создаём имя "Шолто"
                PersonalName personalName = new("system", "Шолто", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фергус"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фергус"))
            {
                //Создаём имя "Фергус"
                PersonalName personalName = new("system", "Фергус", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фингал"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фингал"))
            {
                //Создаём имя "Фингал"
                PersonalName personalName = new("system", "Фингал", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Финлей"
            if (!_repository.PersonalNames.Any(x => x.Name == "Финлей"))
            {
                //Создаём имя "Финлей"
                PersonalName personalName = new("system", "Финлей", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хэмиш"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хэмиш"))
            {
                //Создаём имя "Хэмиш"
                PersonalName personalName = new("system", "Хэмиш", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хьюго"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хьюго"))
            {
                //Создаём имя "Хьюго"
                PersonalName personalName = new("system", "Хьюго", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эван"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эван"))
            {
                //Создаём имя "Эван"
                PersonalName personalName = new("system", "Эван", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эндрю"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эндрю"))
            {
                //Создаём имя "Эндрю"
                PersonalName personalName = new("system", "Эндрю", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Алва"
            if (!_repository.PersonalNames.Any(x => x.Name == "Алва"))
            {
                //Создаём имя "Алва"
                PersonalName personalName = new("system", "Алва", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Андрина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Андрина"))
            {
                //Создаём имя "Андрина"
                PersonalName personalName = new("system", "Андрина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аннабель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аннабель"))
            {
                //Создаём имя "Аннабель"
                PersonalName personalName = new("system", "Аннабель", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арабелла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арабелла"))
            {
                //Создаём имя "Арабелла"
                PersonalName personalName = new("system", "Арабелла", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арлайн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арлайн"))
            {
                //Создаём имя "Арлайн"
                PersonalName personalName = new("system", "Арлайн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бета"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бета"))
            {
                //Создаём имя "Бета"
                PersonalName personalName = new("system", "Бета", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бонни"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бонни"))
            {
                //Создаём имя "Бонни"
                PersonalName personalName = new("system", "Бонни", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бренда"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бренда"))
            {
                //Создаём имя "Бренда"
                PersonalName personalName = new("system", "Бренда", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Вилма"
            if (!_repository.PersonalNames.Any(x => x.Name == "Вилма"))
            {
                //Создаём имя "Вилма"
                PersonalName personalName = new("system", "Вилма", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Давина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Давина"))
            {
                //Создаём имя "Давина"
                PersonalName personalName = new("system", "Давина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дженет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дженет"))
            {
                //Создаём имя "Дженет"
                PersonalName personalName = new("system", "Дженет", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Джесси"
            if (!_repository.PersonalNames.Any(x => x.Name == "Джесси"))
            {
                //Создаём имя "Джесси"
                PersonalName personalName = new("system", "Джесси", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Джинни"
            if (!_repository.PersonalNames.Any(x => x.Name == "Джинни"))
            {
                //Создаём имя "Джинни"
                PersonalName personalName = new("system", "Джинни", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дональдина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дональдина"))
            {
                //Создаём имя "Дональдина"
                PersonalName personalName = new("system", "Дональдина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дональда"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дональда"))
            {
                //Создаём имя "Дональда"
                PersonalName personalName = new("system", "Дональда", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ина"))
            {
                //Создаём имя "Ина"
                PersonalName personalName = new("system", "Ина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иннес"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иннес"))
            {
                //Создаём имя "Иннес"
                PersonalName personalName = new("system", "Иннес", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Исла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Исла"))
            {
                //Создаём имя "Исла"
                PersonalName personalName = new("system", "Исла", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иона"))
            {
                //Создаём имя "Иона"
                PersonalName personalName = new("system", "Иона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Катриона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Катриона"))
            {
                //Создаём имя "Катриона"
                PersonalName personalName = new("system", "Катриона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кателла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кателла"))
            {
                //Создаём имя "Кателла"
                PersonalName personalName = new("system", "Кателла", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кенна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кенна"))
            {
                //Создаём имя "Кенна"
                PersonalName personalName = new("system", "Кенна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Колина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Колина"))
            {
                //Создаём имя "Колина"
                PersonalName personalName = new("system", "Колина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кирсти"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кирсти"))
            {
                //Создаём имя "Кирсти"
                PersonalName personalName = new("system", "Кирсти", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лесли"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лесли"))
            {
                //Создаём имя "Лесли"
                PersonalName personalName = new("system", "Лесли", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лилиас"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лилиас"))
            {
                //Создаём имя "Лилиас"
                PersonalName personalName = new("system", "Лилиас", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лорна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лорна"))
            {
                //Создаём имя "Лорна"
                PersonalName personalName = new("system", "Лорна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Маири"
            if (!_repository.PersonalNames.Any(x => x.Name == "Маири"))
            {
                //Создаём имя "Маири"
                PersonalName personalName = new("system", "Маири", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мальвина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мальвина"))
            {
                //Создаём имя "Мальвина"
                PersonalName personalName = new("system", "Мальвина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мариотта"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мариотта"))
            {
                //Создаём имя "Мариотта"
                PersonalName personalName = new("system", "Мариотта", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Морна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Морна"))
            {
                //Создаём имя "Морна"
                PersonalName personalName = new("system", "Морна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мери"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мери"))
            {
                //Создаём имя "Мери"
                PersonalName personalName = new("system", "Мери", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Несса"
            if (!_repository.PersonalNames.Any(x => x.Name == "Несса"))
            {
                //Создаём имя "Несса"
                PersonalName personalName = new("system", "Несса", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Несси"
            if (!_repository.PersonalNames.Any(x => x.Name == "Несси"))
            {
                //Создаём имя "Несси"
                PersonalName personalName = new("system", "Несси", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Осла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Осла"))
            {
                //Создаём имя "Осла"
                PersonalName personalName = new("system", "Осла", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рона"))
            {
                //Создаём имя "Рона"
                PersonalName personalName = new("system", "Рона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сельма"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сельма"))
            {
                //Создаём имя "Сельма"
                PersonalName personalName = new("system", "Сельма", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фанни"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фанни"))
            {
                //Создаём имя "Фанни"
                PersonalName personalName = new("system", "Фанни", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фиона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фиона"))
            {
                //Создаём имя "Фиона"
                PersonalName personalName = new("system", "Фиона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хейзер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хейзер"))
            {
                //Создаём имя "Хейзер"
                PersonalName personalName = new("system", "Хейзер", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Шина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Шина"))
            {
                //Создаём имя "Шина"
                PersonalName personalName = new("system", "Шина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Шона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Шона"))
            {
                //Создаём имя "Шона"
                PersonalName personalName = new("system", "Шона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эдана"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эдана"))
            {
                //Создаём имя "Эдана"
                PersonalName personalName = new("system", "Эдана", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эдвина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эдвина"))
            {
                //Создаём имя "Эдвина"
                PersonalName personalName = new("system", "Эдвина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эйлин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эйлин"))
            {
                //Создаём имя "Эйлин"
                PersonalName personalName = new("system", "Эйлин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эйли"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эйли"))
            {
                //Создаём имя "Эйли"
                PersonalName personalName = new("system", "Эйли", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эльза"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эльза"))
            {
                //Создаём имя "Эльза"
                PersonalName personalName = new("system", "Эльза", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элисон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элисон"))
            {
                //Создаём имя "Элисон"
                PersonalName personalName = new("system", "Элисон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эна"))
            {
                //Создаём имя "Эна"
                PersonalName personalName = new("system", "Эна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эффи"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эффи"))
            {
                //Создаём имя "Эффи"
                PersonalName personalName = new("system", "Эффи", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аерон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аерон"))
            {
                //Создаём имя "Аерон"
                PersonalName personalName = new("system", "Аерон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Айолин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Айолин"))
            {
                //Создаём имя "Айолин"
                PersonalName personalName = new("system", "Айолин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Айоло"
            if (!_repository.PersonalNames.Any(x => x.Name == "Айоло"))
            {
                //Создаём имя "Айоло"
                PersonalName personalName = new("system", "Айоло", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Айорверт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Айорверт"))
            {
                //Создаём имя "Айорверт"
                PersonalName personalName = new("system", "Айорверт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Алед"
            if (!_repository.PersonalNames.Any(x => x.Name == "Алед"))
            {
                //Создаём имя "Алед"
                PersonalName personalName = new("system", "Алед", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Алун"
            if (!_repository.PersonalNames.Any(x => x.Name == "Алун"))
            {
                //Создаём имя "Алун"
                PersonalName personalName = new("system", "Алун", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анеирин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анеирин"))
            {
                //Создаём имя "Анеирин"
                PersonalName personalName = new("system", "Анеирин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анеурин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анеурин"))
            {
                //Создаём имя "Анеурин"
                PersonalName personalName = new("system", "Анеурин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арвэль"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арвэль"))
            {
                //Создаём имя "Арвэль"
                PersonalName personalName = new("system", "Арвэль", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Афон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Афон"))
            {
                //Создаём имя "Афон"
                PersonalName personalName = new("system", "Афон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бедвир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бедвир"))
            {
                //Создаём имя "Бедвир"
                PersonalName personalName = new("system", "Бедвир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бели"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бели"))
            {
                //Создаём имя "Бели"
                PersonalName personalName = new("system", "Бели", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Беруин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Беруин"))
            {
                //Создаём имя "Беруин"
                PersonalName personalName = new("system", "Беруин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бренин ллвид"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бренин ллвид"))
            {
                //Создаём имя "Бренин ллвид"
                PersonalName personalName = new("system", "Бренин ллвид", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бриин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бриин"))
            {
                //Создаём имя "Бриин"
                PersonalName personalName = new("system", "Бриин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брин"))
            {
                //Создаём имя "Брин"
                PersonalName personalName = new("system", "Брин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бринмор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бринмор"))
            {
                //Создаём имя "Бринмор"
                PersonalName personalName = new("system", "Бринмор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бринн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бринн"))
            {
                //Создаём имя "Бринн"
                PersonalName personalName = new("system", "Бринн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бричэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бричэн"))
            {
                //Создаём имя "Бричэн"
                PersonalName personalName = new("system", "Бричэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брэйт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брэйт"))
            {
                //Создаём имя "Брэйт"
                PersonalName personalName = new("system", "Брэйт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бэл"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бэл"))
            {
                //Создаём имя "Бэл"
                PersonalName personalName = new("system", "Бэл", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Вайнн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Вайнн"))
            {
                //Создаём имя "Вайнн"
                PersonalName personalName = new("system", "Вайнн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ваугхан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ваугхан"))
            {
                //Создаём имя "Ваугхан"
                PersonalName personalName = new("system", "Ваугхан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ваугхн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ваугхн"))
            {
                //Создаём имя "Ваугхн"
                PersonalName personalName = new("system", "Ваугхн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Винн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Винн"))
            {
                //Создаём имя "Винн"
                PersonalName personalName = new("system", "Винн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Винфор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Винфор"))
            {
                //Создаём имя "Винфор"
                PersonalName personalName = new("system", "Винфор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Вмффре"
            if (!_repository.PersonalNames.Any(x => x.Name == "Вмффре"))
            {
                //Создаём имя "Вмффре"
                PersonalName personalName = new("system", "Вмффре", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Галахад"
            if (!_repository.PersonalNames.Any(x => x.Name == "Галахад"))
            {
                //Создаём имя "Галахад"
                PersonalName personalName = new("system", "Галахад", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гарет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гарет"))
            {
                //Создаём имя "Гарет"
                PersonalName personalName = new("system", "Гарет", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гваллтер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гваллтер"))
            {
                //Создаём имя "Гваллтер"
                PersonalName personalName = new("system", "Гваллтер", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвизир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвизир"))
            {
                //Создаём имя "Гвизир"
                PersonalName personalName = new("system", "Гвизир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвил"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвил"))
            {
                //Создаём имя "Гвил"
                PersonalName personalName = new("system", "Гвил", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвилим"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвилим"))
            {
                //Создаём имя "Гвилим"
                PersonalName personalName = new("system", "Гвилим", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвиллим"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвиллим"))
            {
                //Создаём имя "Гвиллим"
                PersonalName personalName = new("system", "Гвиллим", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвинн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвинн"))
            {
                //Создаём имя "Гвинн"
                PersonalName personalName = new("system", "Гвинн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвинфор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвинфор"))
            {
                //Создаём имя "Гвинфор"
                PersonalName personalName = new("system", "Гвинфор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвледиг"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвледиг"))
            {
                //Создаём имя "Гвледиг"
                PersonalName personalName = new("system", "Гвледиг", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гволкхгвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гволкхгвин"))
            {
                //Создаём имя "Гволкхгвин"
                PersonalName personalName = new("system", "Гволкхгвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гераинт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гераинт"))
            {
                //Создаём имя "Гераинт"
                PersonalName personalName = new("system", "Гераинт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гесим"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гесим"))
            {
                //Создаём имя "Гесим"
                PersonalName personalName = new("system", "Гесим", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глин"))
            {
                //Создаём имя "Глин"
                PersonalName personalName = new("system", "Глин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глиндр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глиндр"))
            {
                //Создаём имя "Глиндр"
                PersonalName personalName = new("system", "Глиндр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глинн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глинн"))
            {
                //Создаём имя "Глинн"
                PersonalName personalName = new("system", "Глинн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глоу"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глоу"))
            {
                //Создаём имя "Глоу"
                PersonalName personalName = new("system", "Глоу", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Говэннон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Говэннон"))
            {
                //Создаём имя "Говэннон"
                PersonalName personalName = new("system", "Говэннон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Горонви"
            if (!_repository.PersonalNames.Any(x => x.Name == "Горонви"))
            {
                //Создаём имя "Горонви"
                PersonalName personalName = new("system", "Горонви", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Грвн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Грвн"))
            {
                //Создаём имя "Грвн"
                PersonalName personalName = new("system", "Грвн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Грзэирн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Грзэирн"))
            {
                //Создаём имя "Грзэирн"
                PersonalName personalName = new("system", "Грзэирн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гри"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гри"))
            {
                //Создаём имя "Гри"
                PersonalName personalName = new("system", "Гри", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Григор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Григор"))
            {
                //Создаём имя "Григор"
                PersonalName personalName = new("system", "Григор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гриффин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гриффин"))
            {
                //Создаём имя "Гриффин"
                PersonalName personalName = new("system", "Гриффин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гронв"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гронв"))
            {
                //Создаём имя "Гронв"
                PersonalName personalName = new("system", "Гронв", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Груффадд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Груффадд"))
            {
                //Создаём имя "Груффадд"
                PersonalName personalName = new("system", "Груффадд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Груффидд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Груффидд"))
            {
                //Создаём имя "Груффидд"
                PersonalName personalName = new("system", "Груффидд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Груффуд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Груффуд"))
            {
                //Создаём имя "Груффуд"
                PersonalName personalName = new("system", "Груффуд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гуин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гуин"))
            {
                //Создаём имя "Гуин"
                PersonalName personalName = new("system", "Гуин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гуинет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гуинет"))
            {
                //Создаём имя "Гуинет"
                PersonalName personalName = new("system", "Гуинет", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гуортиджирн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гуортиджирн"))
            {
                //Создаём имя "Гуортиджирн"
                PersonalName personalName = new("system", "Гуортиджирн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гуто"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гуто"))
            {
                //Создаём имя "Гуто"
                PersonalName personalName = new("system", "Гуто", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Деинайол"
            if (!_repository.PersonalNames.Any(x => x.Name == "Деинайол"))
            {
                //Создаём имя "Деинайол"
                PersonalName personalName = new("system", "Деинайол", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Делвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Делвин"))
            {
                //Создаём имя "Делвин"
                PersonalName personalName = new("system", "Делвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дерог"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дерог"))
            {
                //Создаём имя "Дерог"
                PersonalName personalName = new("system", "Дерог", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дженкин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дженкин"))
            {
                //Создаём имя "Дженкин"
                PersonalName personalName = new("system", "Дженкин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Джераллт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Джераллт"))
            {
                //Создаём имя "Джераллт"
                PersonalName personalName = new("system", "Джераллт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Джетэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Джетэн"))
            {
                //Создаём имя "Джетэн"
                PersonalName personalName = new("system", "Джетэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дилан"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дилан"))
            {
                //Создаём имя "Дилан"
                PersonalName personalName = new("system", "Дилан", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дилвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дилвин"))
            {
                //Создаём имя "Дилвин"
                PersonalName personalName = new("system", "Дилвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Диллон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Диллон"))
            {
                //Создаём имя "Диллон"
                PersonalName personalName = new("system", "Диллон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дристэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дристэн"))
            {
                //Создаём имя "Дристэн"
                PersonalName personalName = new("system", "Дристэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дьюи"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дьюи"))
            {
                //Создаём имя "Дьюи"
                PersonalName personalName = new("system", "Дьюи", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дьюидд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дьюидд"))
            {
                //Создаём имя "Дьюидд"
                PersonalName personalName = new("system", "Дьюидд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дэфидд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дэфидд"))
            {
                //Создаём имя "Дэфидд"
                PersonalName personalName = new("system", "Дэфидд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Идвал"
            if (!_repository.PersonalNames.Any(x => x.Name == "Идвал"))
            {
                //Создаём имя "Идвал"
                PersonalName personalName = new("system", "Идвал", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иллтид"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иллтид"))
            {
                //Создаём имя "Иллтид"
                PersonalName personalName = new("system", "Иллтид", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иллтуд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иллтуд"))
            {
                //Создаём имя "Иллтуд"
                PersonalName personalName = new("system", "Иллтуд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Илэр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Илэр"))
            {
                //Создаём имя "Илэр"
                PersonalName personalName = new("system", "Илэр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иорэт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иорэт"))
            {
                //Создаём имя "Иорэт"
                PersonalName personalName = new("system", "Иорэт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ислвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ислвин"))
            {
                //Создаём имя "Ислвин"
                PersonalName personalName = new("system", "Ислвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Истин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Истин"))
            {
                //Создаём имя "Истин"
                PersonalName personalName = new("system", "Истин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Итель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Итель"))
            {
                //Создаём имя "Итель"
                PersonalName personalName = new("system", "Итель", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иуон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иуон"))
            {
                //Создаём имя "Иуон"
                PersonalName personalName = new("system", "Иуон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иуэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иуэн"))
            {
                //Создаём имя "Иуэн"
                PersonalName personalName = new("system", "Иуэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ифор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ифор"))
            {
                //Создаём имя "Ифор"
                PersonalName personalName = new("system", "Ифор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ифэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ифэн"))
            {
                //Создаём имя "Ифэн"
                PersonalName personalName = new("system", "Ифэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Иэнто"
            if (!_repository.PersonalNames.Any(x => x.Name == "Иэнто"))
            {
                //Создаём имя "Иэнто"
                PersonalName personalName = new("system", "Иэнто", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Йель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Йель"))
            {
                //Создаём имя "Йель"
                PersonalName personalName = new("system", "Йель", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кадел"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кадел"))
            {
                //Создаём имя "Кадел"
                PersonalName personalName = new("system", "Кадел", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Калхвч"
            if (!_repository.PersonalNames.Any(x => x.Name == "Калхвч"))
            {
                //Создаём имя "Калхвч"
                PersonalName personalName = new("system", "Калхвч", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Карвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Карвин"))
            {
                //Создаём имя "Карвин"
                PersonalName personalName = new("system", "Карвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Киндделв"
            if (!_repository.PersonalNames.Any(x => x.Name == "Киндделв"))
            {
                //Создаём имя "Киндделв"
                PersonalName personalName = new("system", "Киндделв", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кинриг"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кинриг"))
            {
                //Создаём имя "Кинриг"
                PersonalName personalName = new("system", "Кинриг", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кистениэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кистениэн"))
            {
                //Создаём имя "Кистениэн"
                PersonalName personalName = new("system", "Кистениэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кледвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кледвин"))
            {
                //Создаём имя "Кледвин"
                PersonalName personalName = new("system", "Кледвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдваллэдер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдваллэдер"))
            {
                //Создаём имя "Кэдваллэдер"
                PersonalName personalName = new("system", "Кэдваллэдер", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдвалэдер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдвалэдер"))
            {
                //Создаём имя "Кэдвалэдер"
                PersonalName personalName = new("system", "Кэдвалэдер", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдвгон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдвгон"))
            {
                //Создаём имя "Кэдвгон"
                PersonalName personalName = new("system", "Кэдвгон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдок"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдок"))
            {
                //Создаём имя "Кэдок"
                PersonalName personalName = new("system", "Кэдок", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдомедд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдомедд"))
            {
                //Создаём имя "Кэдомедд"
                PersonalName personalName = new("system", "Кэдомедд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдфэель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдфэель"))
            {
                //Создаём имя "Кэдфэель"
                PersonalName personalName = new("system", "Кэдфэель", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэдфэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэдфэн"))
            {
                //Создаём имя "Кэдфэн"
                PersonalName personalName = new("system", "Кэдфэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэрэдог"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэрэдог"))
            {
                //Создаём имя "Кэрэдог"
                PersonalName personalName = new("system", "Кэрэдог", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэрэдок"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэрэдок"))
            {
                //Создаём имя "Кэрэдок"
                PersonalName personalName = new("system", "Кэрэдок", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Леолин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Леолин"))
            {
                //Создаём имя "Леолин"
                PersonalName personalName = new("system", "Леолин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллеелин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллеелин"))
            {
                //Создаём имя "Ллеелин"
                PersonalName personalName = new("system", "Ллеелин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллей"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллей"))
            {
                //Создаём имя "Ллей"
                PersonalName personalName = new("system", "Ллей", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллеу"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллеу"))
            {
                //Создаём имя "Ллеу"
                PersonalName personalName = new("system", "Ллеу", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лливелин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лливелин"))
            {
                //Создаём имя "Лливелин"
                PersonalName personalName = new("system", "Лливелин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лливеллин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лливеллин"))
            {
                //Создаём имя "Лливеллин"
                PersonalName personalName = new("system", "Лливеллин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллир"))
            {
                //Создаём имя "Ллир"
                PersonalName personalName = new("system", "Ллир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллойд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллойд"))
            {
                //Создаём имя "Ллойд"
                PersonalName personalName = new("system", "Ллойд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лльюелин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лльюелин"))
            {
                //Создаём имя "Лльюелин"
                PersonalName personalName = new("system", "Лльюелин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мадок"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мадок"))
            {
                //Создаём имя "Мадок"
                PersonalName personalName = new("system", "Мадок", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Максен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Максен"))
            {
                //Создаём имя "Максен"
                PersonalName personalName = new("system", "Максен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меериг"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меериг"))
            {
                //Создаём имя "Меериг"
                PersonalName personalName = new("system", "Меериг", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меерик"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меерик"))
            {
                //Создаём имя "Меерик"
                PersonalName personalName = new("system", "Меерик", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меикэл"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меикэл"))
            {
                //Создаём имя "Меикэл"
                PersonalName personalName = new("system", "Меикэл", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меирайон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меирайон"))
            {
                //Создаём имя "Меирайон"
                PersonalName personalName = new("system", "Меирайон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мередидд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мередидд"))
            {
                //Создаём имя "Мередидд"
                PersonalName personalName = new("system", "Мередидд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меркэр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меркэр"))
            {
                //Создаём имя "Меркэр"
                PersonalName personalName = new("system", "Меркэр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меррайон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меррайон"))
            {
                //Создаём имя "Меррайон"
                PersonalName personalName = new("system", "Меррайон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меррик"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меррик"))
            {
                //Создаём имя "Меррик"
                PersonalName personalName = new("system", "Меррик", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мерфин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мерфин"))
            {
                //Создаём имя "Мерфин"
                PersonalName personalName = new("system", "Мерфин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мирддин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мирддин"))
            {
                //Создаём имя "Мирддин"
                PersonalName personalName = new("system", "Мирддин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Михэнгель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Михэнгель"))
            {
                //Создаём имя "Михэнгель"
                PersonalName personalName = new("system", "Михэнгель", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Моезен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Моезен"))
            {
                //Создаём имя "Моезен"
                PersonalName personalName = new("system", "Моезен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Молдвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Молдвин"))
            {
                //Создаём имя "Молдвин"
                PersonalName personalName = new("system", "Молдвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Морген"
            if (!_repository.PersonalNames.Any(x => x.Name == "Морген"))
            {
                //Создаём имя "Морген"
                PersonalName personalName = new("system", "Морген", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Морт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Морт"))
            {
                //Создаём имя "Морт"
                PersonalName personalName = new("system", "Морт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мостин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мостин"))
            {
                //Создаём имя "Мостин"
                PersonalName personalName = new("system", "Мостин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэбон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэбон"))
            {
                //Создаём имя "Мэбон"
                PersonalName personalName = new("system", "Мэбон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэдог"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэдог"))
            {
                //Создаём имя "Мэдог"
                PersonalName personalName = new("system", "Мэдог", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэксен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэксен"))
            {
                //Создаём имя "Мэксен"
                PersonalName personalName = new("system", "Мэксен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэредадд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэредадд"))
            {
                //Создаём имя "Мэредадд"
                PersonalName personalName = new("system", "Мэредадд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Надд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Надд"))
            {
                //Создаём имя "Надд"
                PersonalName personalName = new("system", "Надд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Неирин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Неирин"))
            {
                //Создаём имя "Неирин"
                PersonalName personalName = new("system", "Неирин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Неифайон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Неифайон"))
            {
                //Создаём имя "Неифайон"
                PersonalName personalName = new("system", "Неифайон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Нние"
            if (!_repository.PersonalNames.Any(x => x.Name == "Нние"))
            {
                //Создаём имя "Нние"
                PersonalName personalName = new("system", "Нние", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Овен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Овен"))
            {
                //Создаём имя "Овен"
                PersonalName personalName = new("system", "Овен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Осваллт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Осваллт"))
            {
                //Создаём имя "Осваллт"
                PersonalName personalName = new("system", "Осваллт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Остин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Остин"))
            {
                //Создаём имя "Остин"
                PersonalName personalName = new("system", "Остин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Оуен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Оуен"))
            {
                //Создаём имя "Оуен"
                PersonalName personalName = new("system", "Оуен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Оуин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Оуин"))
            {
                //Создаём имя "Оуин"
                PersonalName personalName = new("system", "Оуин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Оуинн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Оуинн"))
            {
                //Создаём имя "Оуинн"
                PersonalName personalName = new("system", "Оуинн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Оуэин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Оуэин"))
            {
                //Создаём имя "Оуэин"
                PersonalName personalName = new("system", "Оуэин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Оуэйн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Оуэйн"))
            {
                //Создаём имя "Оуэйн"
                PersonalName personalName = new("system", "Оуэйн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Офидд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Офидд"))
            {
                //Создаём имя "Офидд"
                PersonalName personalName = new("system", "Офидд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Парри"
            if (!_repository.PersonalNames.Any(x => x.Name == "Парри"))
            {
                //Создаём имя "Парри"
                PersonalName personalName = new("system", "Парри", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Пвилл"
            if (!_repository.PersonalNames.Any(x => x.Name == "Пвилл"))
            {
                //Создаём имя "Пвилл"
                PersonalName personalName = new("system", "Пвилл", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Педр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Педр"))
            {
                //Создаём имя "Педр"
                PersonalName personalName = new("system", "Педр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Пенллин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Пенллин"))
            {
                //Создаём имя "Пенллин"
                PersonalName personalName = new("system", "Пенллин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Прайс"
            if (!_repository.PersonalNames.Any(x => x.Name == "Прайс"))
            {
                //Создаём имя "Прайс"
                PersonalName personalName = new("system", "Прайс", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Придери"
            if (!_repository.PersonalNames.Any(x => x.Name == "Придери"))
            {
                //Создаём имя "Придери"
                PersonalName personalName = new("system", "Придери", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Прис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Прис"))
            {
                //Создаём имя "Прис"
                PersonalName personalName = new("system", "Прис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Пэдеро"
            if (!_repository.PersonalNames.Any(x => x.Name == "Пэдеро"))
            {
                //Создаём имя "Пэдеро"
                PersonalName personalName = new("system", "Пэдеро", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Пэдриг"
            if (!_repository.PersonalNames.Any(x => x.Name == "Пэдриг"))
            {
                //Создаём имя "Пэдриг"
                PersonalName personalName = new("system", "Пэдриг", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ренфрю"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ренфрю"))
            {
                //Создаём имя "Ренфрю"
                PersonalName personalName = new("system", "Ренфрю", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рис"))
            {
                //Создаём имя "Рис"
                PersonalName personalName = new("system", "Рис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ролэнт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ролэнт"))
            {
                //Создаём имя "Ролэнт"
                PersonalName personalName = new("system", "Ролэнт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхеиналлт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхеиналлт"))
            {
                //Создаём имя "Рхеиналлт"
                PersonalName personalName = new("system", "Рхеиналлт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхиддерч"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхиддерч"))
            {
                //Создаём имя "Рхиддерч"
                PersonalName personalName = new("system", "Рхиддерч", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхизиарт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхизиарт"))
            {
                //Создаём имя "Рхизиарт"
                PersonalName personalName = new("system", "Рхизиарт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхис"))
            {
                //Создаём имя "Рхис"
                PersonalName personalName = new("system", "Рхис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхоберт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхоберт"))
            {
                //Создаём имя "Рхоберт"
                PersonalName personalName = new("system", "Рхоберт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рходри"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рходри"))
            {
                //Создаём имя "Рходри"
                PersonalName personalName = new("system", "Рходри", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сайор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сайор"))
            {
                //Создаём имя "Сайор"
                PersonalName personalName = new("system", "Сайор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сайорис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сайорис"))
            {
                //Создаём имя "Сайорис"
                PersonalName personalName = new("system", "Сайорис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сайорс"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сайорс"))
            {
                //Создаём имя "Сайорс"
                PersonalName personalName = new("system", "Сайорс", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сайорус"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сайорус"))
            {
                //Создаём имя "Сайорус"
                PersonalName personalName = new("system", "Сайорус", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сеиссилт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сеиссилт"))
            {
                //Создаём имя "Сеиссилт"
                PersonalName personalName = new("system", "Сеиссилт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Селиф"
            if (!_repository.PersonalNames.Any(x => x.Name == "Селиф"))
            {
                //Создаём имя "Селиф"
                PersonalName personalName = new("system", "Селиф", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиарл"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиарл"))
            {
                //Создаём имя "Сиарл"
                PersonalName personalName = new("system", "Сиарл", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Силиддон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Силиддон"))
            {
                //Создаём имя "Силиддон"
                PersonalName personalName = new("system", "Силиддон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Силин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Силин"))
            {
                //Создаём имя "Силин"
                PersonalName personalName = new("system", "Силин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Синкин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Синкин"))
            {
                //Создаём имя "Синкин"
                PersonalName personalName = new("system", "Синкин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сион"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сион"))
            {
                //Создаём имя "Сион"
                PersonalName personalName = new("system", "Сион", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сирвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сирвин"))
            {
                //Создаём имя "Сирвин"
                PersonalName personalName = new("system", "Сирвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сифин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сифин"))
            {
                //Создаём имя "Сифин"
                PersonalName personalName = new("system", "Сифин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиффр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиффр"))
            {
                //Создаём имя "Сиффр"
                PersonalName personalName = new("system", "Сиффр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Соил"
            if (!_repository.PersonalNames.Any(x => x.Name == "Соил"))
            {
                //Создаём имя "Соил"
                PersonalName personalName = new("system", "Соил", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Стеффэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Стеффэн"))
            {
                //Создаём имя "Стеффэн"
                PersonalName personalName = new("system", "Стеффэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сэдрн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сэдрн"))
            {
                //Создаём имя "Сэдрн"
                PersonalName personalName = new("system", "Сэдрн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Таффи"
            if (!_repository.PersonalNames.Any(x => x.Name == "Таффи"))
            {
                //Создаём имя "Таффи"
                PersonalName personalName = new("system", "Таффи", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тведр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тведр"))
            {
                //Создаём имя "Тведр"
                PersonalName personalName = new("system", "Тведр", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Твм"
            if (!_repository.PersonalNames.Any(x => x.Name == "Твм"))
            {
                //Создаём имя "Твм"
                PersonalName personalName = new("system", "Твм", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Теирту"
            if (!_repository.PersonalNames.Any(x => x.Name == "Теирту"))
            {
                //Создаём имя "Теирту"
                PersonalName personalName = new("system", "Теирту", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Томос"
            if (!_repository.PersonalNames.Any(x => x.Name == "Томос"))
            {
                //Создаём имя "Томос"
                PersonalName personalName = new("system", "Томос", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Трев"
            if (!_repository.PersonalNames.Any(x => x.Name == "Трев"))
            {
                //Создаём имя "Трев"
                PersonalName personalName = new("system", "Трев", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тревор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тревор"))
            {
                //Создаём имя "Тревор"
                PersonalName personalName = new("system", "Тревор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Трефор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Трефор"))
            {
                //Создаём имя "Трефор"
                PersonalName personalName = new("system", "Трефор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Трэхэерн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Трэхэерн"))
            {
                //Создаём имя "Трэхэерн"
                PersonalName personalName = new("system", "Трэхэерн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тудер"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тудер"))
            {
                //Создаём имя "Тудер"
                PersonalName personalName = new("system", "Тудер", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тудир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тудир"))
            {
                //Создаём имя "Тудир"
                PersonalName personalName = new("system", "Тудир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тэлисин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тэлисин"))
            {
                //Создаём имя "Тэлисин"
                PersonalName personalName = new("system", "Тэлисин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тэлфрин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тэлфрин"))
            {
                //Создаём имя "Тэлфрин"
                PersonalName personalName = new("system", "Тэлфрин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тюдор"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тюдор"))
            {
                //Создаём имя "Тюдор"
                PersonalName personalName = new("system", "Тюдор", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Уриен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Уриен"))
            {
                //Создаём имя "Уриен"
                PersonalName personalName = new("system", "Уриен", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Фолэнт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фолэнт"))
            {
                //Создаём имя "Фолэнт"
                PersonalName personalName = new("system", "Фолэнт", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хаул"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хаул"))
            {
                //Создаём имя "Хаул"
                PersonalName personalName = new("system", "Хаул", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хеддвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хеддвин"))
            {
                //Создаём имя "Хеддвин"
                PersonalName personalName = new("system", "Хеддвин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хеилин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хеилин"))
            {
                //Создаём имя "Хеилин"
                PersonalName personalName = new("system", "Хеилин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хенбеддестир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хенбеддестир"))
            {
                //Создаём имя "Хенбеддестир"
                PersonalName personalName = new("system", "Хенбеддестир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хенвас"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хенвас"))
            {
                //Создаём имя "Хенвас"
                PersonalName personalName = new("system", "Хенвас", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хенвинеб"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хенвинеб"))
            {
                //Создаём имя "Хенвинеб"
                PersonalName personalName = new("system", "Хенвинеб", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хеулог"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хеулог"))
            {
                //Создаём имя "Хеулог"
                PersonalName personalName = new("system", "Хеулог", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хефин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хефин"))
            {
                //Создаём имя "Хефин"
                PersonalName personalName = new("system", "Хефин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хивель"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хивель"))
            {
                //Создаём имя "Хивель"
                PersonalName personalName = new("system", "Хивель", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хопкин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хопкин"))
            {
                //Создаём имя "Хопкин"
                PersonalName personalName = new("system", "Хопкин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хув"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хув"))
            {
                //Создаём имя "Хув"
                PersonalName personalName = new("system", "Хув", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хэдин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хэдин"))
            {
                //Создаём имя "Хэдин"
                PersonalName personalName = new("system", "Хэдин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эериг"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эериг"))
            {
                //Создаём имя "Эериг"
                PersonalName personalName = new("system", "Эериг", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эилиэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эилиэн"))
            {
                //Создаём имя "Эилиэн"
                PersonalName personalName = new("system", "Эилиэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эинайон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эинайон"))
            {
                //Создаём имя "Эинайон"
                PersonalName personalName = new("system", "Эинайон", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элидир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элидир"))
            {
                //Создаём имя "Элидир"
                PersonalName personalName = new("system", "Элидир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элизуд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элизуд"))
            {
                //Создаём имя "Элизуд"
                PersonalName personalName = new("system", "Элизуд", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элис"))
            {
                //Создаём имя "Элис"
                PersonalName personalName = new("system", "Элис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элиэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элиэн"))
            {
                //Создаём имя "Элиэн"
                PersonalName personalName = new("system", "Элиэн", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эмир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эмир"))
            {
                //Создаём имя "Эмир"
                PersonalName personalName = new("system", "Эмир", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эмлин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эмлин"))
            {
                //Создаём имя "Эмлин"
                PersonalName personalName = new("system", "Эмлин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эмрис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эмрис"))
            {
                //Создаём имя "Эмрис"
                PersonalName personalName = new("system", "Эмрис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Энфис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Энфис"))
            {
                //Создаём имя "Энфис"
                PersonalName personalName = new("system", "Энфис", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эрквлфф"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эрквлфф"))
            {
                //Создаём имя "Эрквлфф"
                PersonalName personalName = new("system", "Эрквлфф", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Юеин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Юеин"))
            {
                //Создаём имя "Юеин"
                PersonalName personalName = new("system", "Юеин", true);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Адерин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Адерин"))
            {
                //Создаём имя "Адерин"
                PersonalName personalName = new("system", "Адерин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аерона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аерона"))
            {
                //Создаём имя "Аерона"
                PersonalName personalName = new("system", "Аерона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аеронвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аеронвен"))
            {
                //Создаём имя "Аеронвен"
                PersonalName personalName = new("system", "Аеронвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Аеронви"
            if (!_repository.PersonalNames.Any(x => x.Name == "Аеронви"))
            {
                //Создаём имя "Аеронви"
                PersonalName personalName = new("system", "Аеронви", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анвен"))
            {
                //Создаём имя "Анвен"
                PersonalName personalName = new("system", "Анвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Анвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Анвин"))
            {
                //Создаём имя "Анвин"
                PersonalName personalName = new("system", "Анвин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ангэрэд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ангэрэд"))
            {
                //Создаём имя "Ангэрэд"
                PersonalName personalName = new("system", "Ангэрэд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ангэрэт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ангэрэт"))
            {
                //Создаём имя "Ангэрэт"
                PersonalName personalName = new("system", "Ангэрэт", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арвидд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арвидд"))
            {
                //Создаём имя "Арвидд"
                PersonalName personalName = new("system", "Арвидд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ариэнрход"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ариэнрход"))
            {
                //Создаём имя "Ариэнрход"
                PersonalName personalName = new("system", "Ариэнрход", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Арэнрход"
            if (!_repository.PersonalNames.Any(x => x.Name == "Арэнрход"))
            {
                //Создаём имя "Арэнрход"
                PersonalName personalName = new("system", "Арэнрход", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Афэнен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Афэнен"))
            {
                //Создаём имя "Афэнен"
                PersonalName personalName = new("system", "Афэнен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бетрис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бетрис"))
            {
                //Создаём имя "Бетрис"
                PersonalName personalName = new("system", "Бетрис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бетэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бетэн"))
            {
                //Создаём имя "Бетэн"
                PersonalName personalName = new("system", "Бетэн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Блодвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Блодвен"))
            {
                //Создаём имя "Блодвен"
                PersonalName personalName = new("system", "Блодвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Блодеуведд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Блодеуведд"))
            {
                //Создаём имя "Блодеуведд"
                PersonalName personalName = new("system", "Блодеуведд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Блодеуедд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Блодеуедд"))
            {
                //Создаём имя "Блодеуедд"
                PersonalName personalName = new("system", "Блодеуедд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Блодеуин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Блодеуин"))
            {
                //Создаём имя "Блодеуин"
                PersonalName personalName = new("system", "Блодеуин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бриаллен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бриаллен"))
            {
                //Создаём имя "Бриаллен"
                PersonalName personalName = new("system", "Бриаллен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брон"))
            {
                //Создаём имя "Брон"
                PersonalName personalName = new("system", "Брон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бронвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бронвен"))
            {
                //Создаём имя "Бронвен"
                PersonalName personalName = new("system", "Бронвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Бронвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Бронвин"))
            {
                //Создаём имя "Бронвин"
                PersonalName personalName = new("system", "Бронвин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брэнвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брэнвен"))
            {
                //Создаём имя "Брэнвен"
                PersonalName personalName = new("system", "Брэнвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брэнвенн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брэнвенн"))
            {
                //Создаём имя "Брэнвенн"
                PersonalName personalName = new("system", "Брэнвенн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брэнгвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брэнгвен"))
            {
                //Создаём имя "Брэнгвен"
                PersonalName personalName = new("system", "Брэнгвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Брэнгви"
            if (!_repository.PersonalNames.Any(x => x.Name == "Брэнгви"))
            {
                //Создаём имя "Брэнгви"
                PersonalName personalName = new("system", "Брэнгви", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Винни"
            if (!_repository.PersonalNames.Any(x => x.Name == "Винни"))
            {
                //Создаём имя "Винни"
                PersonalName personalName = new("system", "Винни", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвар"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвар"))
            {
                //Создаём имя "Гвар"
                PersonalName personalName = new("system", "Гвар", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвен"))
            {
                //Создаём имя "Гвен"
                PersonalName personalName = new("system", "Гвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвенда"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвенда"))
            {
                //Создаём имя "Гвенда"
                PersonalName personalName = new("system", "Гвенда", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвендолайн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвендолайн"))
            {
                //Создаём имя "Гвендолайн"
                PersonalName personalName = new("system", "Гвендолайн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвендолен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвендолен"))
            {
                //Создаём имя "Гвендолен"
                PersonalName personalName = new("system", "Гвендолен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвендолин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвендолин"))
            {
                //Создаём имя "Гвендолин"
                PersonalName personalName = new("system", "Гвендолин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвенет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвенет"))
            {
                //Создаём имя "Гвенет"
                PersonalName personalName = new("system", "Гвенет", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвенит"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвенит"))
            {
                //Создаём имя "Гвенит"
                PersonalName personalName = new("system", "Гвенит", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвенллиэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвенллиэн"))
            {
                //Создаём имя "Гвенллиэн"
                PersonalName personalName = new("system", "Гвенллиэн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвеннет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвеннет"))
            {
                //Создаём имя "Гвеннет"
                PersonalName personalName = new("system", "Гвеннет", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвенфрюи"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвенфрюи"))
            {
                //Создаём имя "Гвенфрюи"
                PersonalName personalName = new("system", "Гвенфрюи", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвенхвивэр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвенхвивэр"))
            {
                //Создаём имя "Гвенхвивэр"
                PersonalName personalName = new("system", "Гвенхвивэр", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвинеира"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвинеира"))
            {
                //Создаём имя "Гвинеира"
                PersonalName personalName = new("system", "Гвинеира", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвинет"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвинет"))
            {
                //Создаём имя "Гвинет"
                PersonalName personalName = new("system", "Гвинет", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гвлэдус"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гвлэдус"))
            {
                //Создаём имя "Гвлэдус"
                PersonalName personalName = new("system", "Гвлэдус", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Генерис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Генерис"))
            {
                //Создаём имя "Генерис"
                PersonalName personalName = new("system", "Генерис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гленда"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гленда"))
            {
                //Создаём имя "Гленда"
                PersonalName personalName = new("system", "Гленда", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глинис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глинис"))
            {
                //Создаём имя "Глинис"
                PersonalName personalName = new("system", "Глинис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глодуса"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глодуса"))
            {
                //Создаём имя "Глодуса"
                PersonalName personalName = new("system", "Глодуса", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глэдис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глэдис"))
            {
                //Создаём имя "Глэдис"
                PersonalName personalName = new("system", "Глэдис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Глэнис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Глэнис"))
            {
                //Создаём имя "Глэнис"
                PersonalName personalName = new("system", "Глэнис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Гуендолен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Гуендолен"))
            {
                //Создаём имя "Гуендолен"
                PersonalName personalName = new("system", "Гуендолен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Двин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Двин"))
            {
                //Создаём имя "Двин"
                PersonalName personalName = new("system", "Двин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Двинвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Двинвен"))
            {
                //Создаём имя "Двинвен"
                PersonalName personalName = new("system", "Двинвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Двинвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Двинвин"))
            {
                //Создаём имя "Двинвин"
                PersonalName personalName = new("system", "Двинвин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Делит"
            if (!_repository.PersonalNames.Any(x => x.Name == "Делит"))
            {
                //Создаём имя "Делит"
                PersonalName personalName = new("system", "Делит", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Диилис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Диилис"))
            {
                //Создаём имя "Диилис"
                PersonalName personalName = new("system", "Диилис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дилвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дилвен"))
            {
                //Создаём имя "Дилвен"
                PersonalName personalName = new("system", "Дилвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Дилис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Дилис"))
            {
                //Создаём имя "Дилис"
                PersonalName personalName = new("system", "Дилис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Игрэйн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Игрэйн"))
            {
                //Создаём имя "Игрэйн"
                PersonalName personalName = new("system", "Игрэйн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Исбэйл"
            if (!_repository.PersonalNames.Any(x => x.Name == "Исбэйл"))
            {
                //Создаём имя "Исбэйл"
                PersonalName personalName = new("system", "Исбэйл", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кади"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кади"))
            {
                //Создаём имя "Кади"
                PersonalName personalName = new("system", "Кади", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Креиддилэд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Креиддилэд"))
            {
                //Создаём имя "Креиддилэд"
                PersonalName personalName = new("system", "Креиддилэд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэрис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэрис"))
            {
                //Создаём имя "Кэрис"
                PersonalName personalName = new("system", "Кэрис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Кэрон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Кэрон"))
            {
                //Создаём имя "Кэрон"
                PersonalName personalName = new("system", "Кэрон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллеулу"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллеулу"))
            {
                //Создаём имя "Ллеулу"
                PersonalName personalName = new("system", "Ллеулу", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ллинос"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ллинос"))
            {
                //Создаём имя "Ллинос"
                PersonalName personalName = new("system", "Ллинос", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Лунед"
            if (!_repository.PersonalNames.Any(x => x.Name == "Лунед"))
            {
                //Создаём имя "Лунед"
                PersonalName personalName = new("system", "Лунед", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Маллт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Маллт"))
            {
                //Создаём имя "Маллт"
                PersonalName personalName = new("system", "Маллт", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Марджед"
            if (!_repository.PersonalNames.Any(x => x.Name == "Марджед"))
            {
                //Создаём имя "Марджед"
                PersonalName personalName = new("system", "Марджед", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Маред"
            if (!_repository.PersonalNames.Any(x => x.Name == "Маред"))
            {
                //Создаём имя "Маред"
                PersonalName personalName = new("system", "Маред", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меган"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меган"))
            {
                //Создаём имя "Меган"
                PersonalName personalName = new("system", "Меган", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меинвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меинвен"))
            {
                //Создаём имя "Меинвен"
                PersonalName personalName = new("system", "Меинвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меинир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меинир"))
            {
                //Создаём имя "Меинир"
                PersonalName personalName = new("system", "Меинир", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Меирайона"
            if (!_repository.PersonalNames.Any(x => x.Name == "Меирайона"))
            {
                //Создаём имя "Меирайона"
                PersonalName personalName = new("system", "Меирайона", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мерерид"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мерерид"))
            {
                //Создаём имя "Мерерид"
                PersonalName personalName = new("system", "Мерерид", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Миф"
            if (!_repository.PersonalNames.Any(x => x.Name == "Миф"))
            {
                //Создаём имя "Миф"
                PersonalName personalName = new("system", "Миф", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мифэнви"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мифэнви"))
            {
                //Создаём имя "Мифэнви"
                PersonalName personalName = new("system", "Мифэнви", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Модлен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Модлен"))
            {
                //Создаём имя "Модлен"
                PersonalName personalName = new("system", "Модлен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Морвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Морвен"))
            {
                //Создаём имя "Морвен"
                PersonalName personalName = new("system", "Морвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Морвенна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Морвенна"))
            {
                //Создаём имя "Морвенна"
                PersonalName personalName = new("system", "Морвенна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэбли"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэбли"))
            {
                //Создаём имя "Мэбли"
                PersonalName personalName = new("system", "Мэбли", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэйр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэйр"))
            {
                //Создаём имя "Мэйр"
                PersonalName personalName = new("system", "Мэйр", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Мэрвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Мэрвен"))
            {
                //Создаём имя "Мэрвен"
                PersonalName personalName = new("system", "Мэрвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Нерис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Нерис"))
            {
                //Создаём имя "Нерис"
                PersonalName personalName = new("system", "Нерис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Нест"
            if (!_repository.PersonalNames.Any(x => x.Name == "Нест"))
            {
                //Создаём имя "Нест"
                PersonalName personalName = new("system", "Нест", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Неста"
            if (!_repository.PersonalNames.Any(x => x.Name == "Неста"))
            {
                //Создаём имя "Неста"
                PersonalName personalName = new("system", "Неста", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ниму"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ниму"))
            {
                //Создаём имя "Ниму"
                PersonalName personalName = new("system", "Ниму", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ния"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ния"))
            {
                //Создаём имя "Ния"
                PersonalName personalName = new("system", "Ния", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Олвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Олвен"))
            {
                //Создаём имя "Олвен"
                PersonalName personalName = new("system", "Олвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Олвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Олвин"))
            {
                //Создаём имя "Олвин"
                PersonalName personalName = new("system", "Олвин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Олвинн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Олвинн"))
            {
                //Создаём имя "Олвинн"
                PersonalName personalName = new("system", "Олвинн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Риэннон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Риэннон"))
            {
                //Создаём имя "Риэннон"
                PersonalName personalName = new("system", "Риэннон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхиэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхиэн"))
            {
                //Создаём имя "Рхиэн"
                PersonalName personalName = new("system", "Рхиэн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхиэнвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхиэнвен"))
            {
                //Создаём имя "Рхиэнвен"
                PersonalName personalName = new("system", "Рхиэнвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхиэннон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхиэннон"))
            {
                //Создаём имя "Рхиэннон"
                PersonalName personalName = new("system", "Рхиэннон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхиэнон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхиэнон"))
            {
                //Создаём имя "Рхиэнон"
                PersonalName personalName = new("system", "Рхиэнон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхиэну"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхиэну"))
            {
                //Создаём имя "Рхиэну"
                PersonalName personalName = new("system", "Рхиэну", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхозин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхозин"))
            {
                //Создаём имя "Рхозин"
                PersonalName personalName = new("system", "Рхозин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхонвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхонвен"))
            {
                //Создаём имя "Рхонвен"
                PersonalName personalName = new("system", "Рхонвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Рхэмэнтус"
            if (!_repository.PersonalNames.Any(x => x.Name == "Рхэмэнтус"))
            {
                //Создаём имя "Рхэмэнтус"
                PersonalName personalName = new("system", "Рхэмэнтус", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сайонед"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сайонед"))
            {
                //Создаём имя "Сайонед"
                PersonalName personalName = new("system", "Сайонед", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Серен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Серен"))
            {
                //Создаём имя "Серен"
                PersonalName personalName = new("system", "Серен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиань"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиань"))
            {
                //Создаём имя "Сиань"
                PersonalName personalName = new("system", "Сиань", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиинвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиинвен"))
            {
                //Создаём имя "Сиинвен"
                PersonalName personalName = new("system", "Сиинвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиридвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиридвен"))
            {
                //Создаём имя "Сиридвен"
                PersonalName personalName = new("system", "Сиридвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сирис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сирис"))
            {
                //Создаём имя "Сирис"
                PersonalName personalName = new("system", "Сирис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сирридвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сирридвин"))
            {
                //Создаём имя "Сирридвин"
                PersonalName personalName = new("system", "Сирридвин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиуон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиуон"))
            {
                //Создаём имя "Сиуон"
                PersonalName personalName = new("system", "Сиуон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиэна"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиэна"))
            {
                //Создаём имя "Сиэна"
                PersonalName personalName = new("system", "Сиэна", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Сиэни"
            if (!_repository.PersonalNames.Any(x => x.Name == "Сиэни"))
            {
                //Создаём имя "Сиэни"
                PersonalName personalName = new("system", "Сиэни", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Таррен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Таррен"))
            {
                //Создаём имя "Таррен"
                PersonalName personalName = new("system", "Таррен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тегвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тегвен"))
            {
                //Создаём имя "Тегвен"
                PersonalName personalName = new("system", "Тегвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тегэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тегэн"))
            {
                //Создаём имя "Тегэн"
                PersonalName personalName = new("system", "Тегэн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Террвин"
            if (!_repository.PersonalNames.Any(x => x.Name == "Террвин"))
            {
                //Создаём имя "Террвин"
                PersonalName personalName = new("system", "Террвин", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тивлип"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тивлип"))
            {
                //Создаём имя "Тивлип"
                PersonalName personalName = new("system", "Тивлип", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Тэлэйт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Тэлэйт"))
            {
                //Создаём имя "Тэлэйт"
                PersonalName personalName = new("system", "Тэлэйт", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ффайон"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ффайон"))
            {
                //Создаём имя "Ффайон"
                PersonalName personalName = new("system", "Ффайон", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Ффрэйд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Ффрэйд"))
            {
                //Создаём имя "Ффрэйд"
                PersonalName personalName = new("system", "Ффрэйд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хеледд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хеледд"))
            {
                //Создаём имя "Хеледд"
                PersonalName personalName = new("system", "Хеледд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хеулвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хеулвен"))
            {
                //Создаём имя "Хеулвен"
                PersonalName personalName = new("system", "Хеулвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хефина"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хефина"))
            {
                //Создаём имя "Хефина"
                PersonalName personalName = new("system", "Хефина", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хиледд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хиледд"))
            {
                //Создаём имя "Хиледд"
                PersonalName personalName = new("system", "Хиледд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хэбрен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хэбрен"))
            {
                //Создаём имя "Хэбрен"
                PersonalName personalName = new("system", "Хэбрен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хэф"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хэф"))
            {
                //Создаём имя "Хэф"
                PersonalName personalName = new("system", "Хэф", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Хэфрен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Хэфрен"))
            {
                //Создаём имя "Хэфрен"
                PersonalName personalName = new("system", "Хэфрен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эервен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эервен"))
            {
                //Создаём имя "Эервен"
                PersonalName personalName = new("system", "Эервен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эигир"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эигир"))
            {
                //Создаём имя "Эигир"
                PersonalName personalName = new("system", "Эигир", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эигр"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эигр"))
            {
                //Создаём имя "Эигр"
                PersonalName personalName = new("system", "Эигр", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эилвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эилвен"))
            {
                //Создаём имя "Эилвен"
                PersonalName personalName = new("system", "Эилвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эилунед"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эилунед"))
            {
                //Создаём имя "Эилунед"
                PersonalName personalName = new("system", "Эилунед", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эира"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эира"))
            {
                //Создаём имя "Эира"
                PersonalName personalName = new("system", "Эира", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эирвен"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эирвен"))
            {
                //Создаём имя "Эирвен"
                PersonalName personalName = new("system", "Эирвен", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эириэн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эириэн"))
            {
                //Создаём имя "Эириэн"
                PersonalName personalName = new("system", "Эириэн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эирлис"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эирлис"))
            {
                //Создаём имя "Эирлис"
                PersonalName personalName = new("system", "Эирлис", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элери"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элери"))
            {
                //Создаём имя "Элери"
                PersonalName personalName = new("system", "Элери", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элунед"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элунед"))
            {
                //Создаём имя "Элунед"
                PersonalName personalName = new("system", "Элунед", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Элэйн"
            if (!_repository.PersonalNames.Any(x => x.Name == "Элэйн"))
            {
                //Создаём имя "Элэйн"
                PersonalName personalName = new("system", "Элэйн", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Энайд"
            if (!_repository.PersonalNames.Any(x => x.Name == "Энайд"))
            {
                //Создаём имя "Энайд"
                PersonalName personalName = new("system", "Энайд", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Энид"
            if (!_repository.PersonalNames.Any(x => x.Name == "Энид"))
            {
                //Создаём имя "Энид"
                PersonalName personalName = new("system", "Энид", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Энит"
            if (!_repository.PersonalNames.Any(x => x.Name == "Энит"))
            {
                //Создаём имя "Энит"
                PersonalName personalName = new("system", "Энит", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эсиллт"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эсиллт"))
            {
                //Создаём имя "Эсиллт"
                PersonalName personalName = new("system", "Эсиллт", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие имени "Эфа"
            if (!_repository.PersonalNames.Any(x => x.Name == "Эфа"))
            {
                //Создаём имя "Эфа"
                PersonalName personalName = new("system", "Эфа", false);
                _repository.PersonalNames.Add(personalName);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializePersonalNames. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации фамилий
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeLastNames()
    {
        try
        {
            //Проверяем наличие фамилии "Миркраниис"
            if (!_repository.LastNames.Any(x => x.Name == "Миркраниис"))
            {
                //Создаём фамилию "Миркраниис"
                LastName lastName = new("system", "Миркраниис");
                _repository.LastNames.Add(lastName);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeLastNames. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации префиксов
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializePrefixNames()
    {
        try
        {
            //Проверяем наличие префикса "из дома"
            if (!_repository.PrefixNames.Any(x => x.Name == "из дома"))
            {
                //Создаём префикс "из дома"
                PrefixName prefixName = new("system", "из дома");
                _repository.PrefixNames.Add(prefixName);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializePrefixNames. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации связи персональных имён с нациями
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeNationsPersonalNames()
    {
        try
        {
            //Проверяем наличие нации "Альв"
            if (_repository.Nations.Any(x => x.Name == "Альв"))
            {
                //Получаем нацию "Альв"
                Nation? nation = _repository.Nations.FirstOrDefault(x => x.Name == "Альв");

                //Проверяем наличие нации "Альв"
                if (nation != null)
                {
                    //Проверяем наличие имени "Амакир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Амакир"))
                    {
                        //Получаем имя "Амакир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Амакир");

                        //Проверяем наличие имени "Амакир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Амакир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Амакир"
                                NationPersonalName nationPersonalName = new("system", 1.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арден"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арден"))
                    {
                        //Получаем имя "Арден"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арден");

                        //Проверяем наличие имени "Арден"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Арден"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Арден"
                                NationPersonalName nationPersonalName = new("system", 0.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Амамион"
                    if (_repository.PersonalNames.Any(x => x.Name == "Амамион"))
                    {
                        //Получаем имя "Амамион"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Амамион");

                        //Проверяем наличие имени "Амамион"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Амамион"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Амамион"
                                NationPersonalName nationPersonalName = new("system", 0.43, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хайден"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хайден"))
                    {
                        //Получаем имя "Хайден"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хайден");

                        //Проверяем наличие имени "Хайден"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Хайден"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Хайден"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Халиодар"
                    if (_repository.PersonalNames.Any(x => x.Name == "Халиодар"))
                    {
                        //Получаем имя "Халиодар"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Халиодар");

                        //Проверяем наличие имени "Халиодар"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Халиодар"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Халиодар"
                                NationPersonalName nationPersonalName = new("system", 0.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рарнолил"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рарнолил"))
                    {
                        //Получаем имя "Рарнолил"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рарнолил");

                        //Проверяем наличие имени "Рарнолил"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Рарнолил"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Рарнолил"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эдоен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эдоен"))
                    {
                        //Получаем имя "Эдоен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эдоен");

                        //Проверяем наличие имени "Эдоен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эдоен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эдоен"
                                NationPersonalName nationPersonalName = new("system", 1.64, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аридир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аридир"))
                    {
                        //Получаем имя "Аридир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аридир");

                        //Проверяем наличие имени "Аридир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Аридир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Аридир"
                                NationPersonalName nationPersonalName = new("system", 1.41, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Леневелон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Леневелон"))
                    {
                        //Получаем имя "Леневелон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Леневелон");

                        //Проверяем наличие имени "Леневелон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Леневелон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Леневелон"
                                NationPersonalName nationPersonalName = new("system", 1.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хальнитрен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хальнитрен"))
                    {
                        //Получаем имя "Хальнитрен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хальнитрен");

                        //Проверяем наличие имени "Хальнитрен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Хальнитрен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Хальнитрен"
                                NationPersonalName nationPersonalName = new("system", 0.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хальнасай"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хальнасай"))
                    {
                        //Получаем имя "Хальнасай"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хальнасай");

                        //Проверяем наличие имени "Хальнасай"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Хальнасай"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Хальнасай"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фалерил"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фалерил"))
                    {
                        //Получаем имя "Фалерил"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фалерил");

                        //Проверяем наличие имени "Фалерил"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Фалерил"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Фалерил"
                                NationPersonalName nationPersonalName = new("system", 0.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кайнаэль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кайнаэль"))
                    {
                        //Получаем имя "Кайнаэль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кайнаэль");

                        //Проверяем наличие имени "Кайнаэль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Кайнаэль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Кайнаэль"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Вален"
                    if (_repository.PersonalNames.Any(x => x.Name == "Вален"))
                    {
                        //Получаем имя "Вален"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Вален");

                        //Проверяем наличие имени "Вален"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Вален"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Вален"
                                NationPersonalName nationPersonalName = new("system", 0.07, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фесолор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фесолор"))
                    {
                        //Получаем имя "Фесолор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фесолор");

                        //Проверяем наличие имени "Фесолор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Фесолор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Фесолор"
                                NationPersonalName nationPersonalName = new("system", 0.33, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фенменор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фенменор"))
                    {
                        //Получаем имя "Фенменор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фенменор");

                        //Проверяем наличие имени "Фенменор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Фенменор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Фенменор"
                                NationPersonalName nationPersonalName = new("system", 0.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Галакир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Галакир"))
                    {
                        //Получаем имя "Галакир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Галакир");

                        //Проверяем наличие имени "Галакир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Галакир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Галакир"
                                NationPersonalName nationPersonalName = new("system", 1.54, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эарониан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эарониан"))
                    {
                        //Получаем имя "Эарониан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эарониан");

                        //Проверяем наличие имени "Эарониан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эарониан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эарониан"
                                NationPersonalName nationPersonalName = new("system", 0.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аматал"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аматал"))
                    {
                        //Получаем имя "Аматал"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аматал");

                        //Проверяем наличие имени "Аматал"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Аматал"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Аматал"
                                NationPersonalName nationPersonalName = new("system", 0.89, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кинрел"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кинрел"))
                    {
                        //Получаем имя "Кинрел"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кинрел");

                        //Проверяем наличие имени "Кинрел"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Кинрел"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Кинрел"
                                NationPersonalName nationPersonalName = new("system", 0.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рававарис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рававарис"))
                    {
                        //Получаем имя "Рававарис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рававарис");

                        //Проверяем наличие имени "Рававарис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Рававарис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Рававарис"
                                NationPersonalName nationPersonalName = new("system", 0.08, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кифарен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кифарен"))
                    {
                        //Получаем имя "Кифарен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кифарен");

                        //Проверяем наличие имени "Кифарен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Кифарен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Кифарен"
                                NationPersonalName nationPersonalName = new("system", 1.38, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тингол"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тингол"))
                    {
                        //Получаем имя "Тингол"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тингол");

                        //Проверяем наличие имени "Тингол"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Тингол"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Тингол"
                                NationPersonalName nationPersonalName = new("system", 0.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тирон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тирон"))
                    {
                        //Получаем имя "Тирон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тирон");

                        //Проверяем наличие имени "Тирон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Тирон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Тирон"
                                NationPersonalName nationPersonalName = new("system", 1.8, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ангрод"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ангрод"))
                    {
                        //Получаем имя "Ангрод"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ангрод");

                        //Проверяем наличие имени "Ангрод"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Ангрод"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Ангрод"
                                NationPersonalName nationPersonalName = new("system", 1.74, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бауглир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бауглир"))
                    {
                        //Получаем имя "Бауглир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бауглир");

                        //Проверяем наличие имени "Бауглир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Бауглир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Бауглир"
                                NationPersonalName nationPersonalName = new("system", 0.45, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гундор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гундор"))
                    {
                        //Получаем имя "Гундор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гундор");

                        //Проверяем наличие имени "Гундор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Гундор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Гундор"
                                NationPersonalName nationPersonalName = new("system", 0.5, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элурад"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элурад"))
                    {
                        //Получаем имя "Элурад"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элурад");

                        //Проверяем наличие имени "Элурад"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Элурад"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Элурад"
                                NationPersonalName nationPersonalName = new("system", 1.75, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ирмо"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ирмо"))
                    {
                        //Получаем имя "Ирмо"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ирмо");

                        //Проверяем наличие имени "Ирмо"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Ирмо"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Ирмо"
                                NationPersonalName nationPersonalName = new("system", 1.79, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ваньяр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ваньяр"))
                    {
                        //Получаем имя "Ваньяр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ваньяр");

                        //Проверяем наличие имени "Ваньяр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Ваньяр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Ваньяр"
                                NationPersonalName nationPersonalName = new("system", 1.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анайрэ"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анайрэ"))
                    {
                        //Получаем имя "Анайрэ"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анайрэ");

                        //Проверяем наличие имени "Анайрэ"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Анайрэ"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Анайрэ"
                                NationPersonalName nationPersonalName = new("system", 1.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Галион"
                    if (_repository.PersonalNames.Any(x => x.Name == "Галион"))
                    {
                        //Получаем имя "Галион"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Галион");

                        //Проверяем наличие имени "Галион"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Галион"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Галион"
                                NationPersonalName nationPersonalName = new("system", 1.95, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иминиэ"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иминиэ"))
                    {
                        //Получаем имя "Иминиэ"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иминиэ");

                        //Проверяем наличие имени "Иминиэ"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Иминиэ"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Иминиэ"
                                NationPersonalName nationPersonalName = new("system", 1.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Индис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Индис"))
                    {
                        //Получаем имя "Индис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Индис");

                        //Проверяем наличие имени "Индис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Индис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Индис"
                                NationPersonalName nationPersonalName = new("system", 1.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Маэдрос"
                    if (_repository.PersonalNames.Any(x => x.Name == "Маэдрос"))
                    {
                        //Получаем имя "Маэдрос"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Маэдрос");

                        //Проверяем наличие имени "Маэдрос"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Маэдрос"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Маэдрос"
                                NationPersonalName nationPersonalName = new("system", 0.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Неллас"
                    if (_repository.PersonalNames.Any(x => x.Name == "Неллас"))
                    {
                        //Получаем имя "Неллас"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Неллас");

                        //Проверяем наличие имени "Неллас"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Неллас"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Неллас"
                                NationPersonalName nationPersonalName = new("system", 1.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Румил"
                    if (_repository.PersonalNames.Any(x => x.Name == "Румил"))
                    {
                        //Получаем имя "Румил"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Румил");

                        //Проверяем наличие имени "Румил"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Румил"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Румил"
                                NationPersonalName nationPersonalName = new("system", 1.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Феанор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Феанор"))
                    {
                        //Получаем имя "Феанор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Феанор");

                        //Проверяем наличие имени "Феанор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Феанор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Феанор"
                                NationPersonalName nationPersonalName = new("system", 0.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Энель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Энель"))
                    {
                        //Получаем имя "Энель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Энель");

                        //Проверяем наличие имени "Энель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Энель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Энель"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эльмо"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эльмо"))
                    {
                        //Получаем имя "Эльмо"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эльмо");

                        //Проверяем наличие имени "Эльмо"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эльмо"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эльмо"
                                NationPersonalName nationPersonalName = new("system", 1.86, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Цислейрис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Цислейрис"))
                    {
                        //Получаем имя "Цислейрис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Цислейрис");

                        //Проверяем наличие имени "Цислейрис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Цислейрис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Цислейрис"
                                NationPersonalName nationPersonalName = new("system", 0.38, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Саотан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Саотан"))
                    {
                        //Получаем имя "Саотан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Саотан");

                        //Проверяем наличие имени "Саотан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Саотан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Саотан"
                                NationPersonalName nationPersonalName = new("system", 0.54, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эстра"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эстра"))
                    {
                        //Получаем имя "Эстра"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эстра");

                        //Проверяем наличие имени "Эстра"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эстра"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эстра"
                                NationPersonalName nationPersonalName = new("system", 0.59, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Друзилла"
                    if (_repository.PersonalNames.Any(x => x.Name == "Друзилла"))
                    {
                        //Получаем имя "Друзилла"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Друзилла");

                        //Проверяем наличие имени "Друзилла"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Друзилла"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Друзилла"
                                NationPersonalName nationPersonalName = new("system", 0.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эрис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эрис"))
                    {
                        //Получаем имя "Эрис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эрис");

                        //Проверяем наличие имени "Эрис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эрис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эрис"
                                NationPersonalName nationPersonalName = new("system", 1.6, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глинда"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глинда"))
                    {
                        //Получаем имя "Глинда"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глинда");

                        //Проверяем наличие имени "Глинда"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Глинда"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Глинда"
                                NationPersonalName nationPersonalName = new("system", 0.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фасьенн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фасьенн"))
                    {
                        //Получаем имя "Фасьенн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фасьенн");

                        //Проверяем наличие имени "Фасьенн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Фасьенн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Фасьенн"
                                NationPersonalName nationPersonalName = new("system", 0.23, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Виэслана"
                    if (_repository.PersonalNames.Any(x => x.Name == "Виэслана"))
                    {
                        //Получаем имя "Виэслана"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Виэслана");

                        //Проверяем наличие имени "Виэслана"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Виэслана"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Виэслана"
                                NationPersonalName nationPersonalName = new("system", 1.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Верития"
                    if (_repository.PersonalNames.Any(x => x.Name == "Верития"))
                    {
                        //Получаем имя "Верития"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Верития");

                        //Проверяем наличие имени "Верития"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Верития"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Верития"
                                NationPersonalName nationPersonalName = new("system", 1.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фейнтелин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фейнтелин"))
                    {
                        //Получаем имя "Фейнтелин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фейнтелин");

                        //Проверяем наличие имени "Фейнтелин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Фейнтелин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Фейнтелин"
                                NationPersonalName nationPersonalName = new("system", 0.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Велая"
                    if (_repository.PersonalNames.Any(x => x.Name == "Велая"))
                    {
                        //Получаем имя "Велая"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Велая");

                        //Проверяем наличие имени "Велая"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Велая"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Велая"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Корлейна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Корлейна"))
                    {
                        //Получаем имя "Корлейна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Корлейна");

                        //Проверяем наличие имени "Корлейна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Корлейна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Корлейна"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Верозания"
                    if (_repository.PersonalNames.Any(x => x.Name == "Верозания"))
                    {
                        //Получаем имя "Верозания"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Верозания");

                        //Проверяем наличие имени "Верозания"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Верозания"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Верозания"
                                NationPersonalName nationPersonalName = new("system", 1.07, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кайэсния"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кайэсния"))
                    {
                        //Получаем имя "Кайэсния"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кайэсния");

                        //Проверяем наличие имени "Кайэсния"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Кайэсния"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Кайэсния"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элинбель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элинбель"))
                    {
                        //Получаем имя "Элинбель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элинбель");

                        //Проверяем наличие имени "Элинбель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Элинбель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Элинбель"
                                NationPersonalName nationPersonalName = new("system", 0.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фенелла"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фенелла"))
                    {
                        //Получаем имя "Фенелла"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фенелла");

                        //Проверяем наличие имени "Фенелла"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Фенелла"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Фенелла"
                                NationPersonalName nationPersonalName = new("system", 0.67, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Белилия"
                    if (_repository.PersonalNames.Any(x => x.Name == "Белилия"))
                    {
                        //Получаем имя "Белилия"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Белилия");

                        //Проверяем наличие имени "Белилия"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Белилия"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Белилия"
                                NationPersonalName nationPersonalName = new("system", 1.76, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Беллами"
                    if (_repository.PersonalNames.Any(x => x.Name == "Беллами"))
                    {
                        //Получаем имя "Беллами"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Беллами");

                        //Проверяем наличие имени "Беллами"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Беллами"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Беллами"
                                NationPersonalName nationPersonalName = new("system", 1.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эмери"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эмери"))
                    {
                        //Получаем имя "Эмери"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эмери");

                        //Проверяем наличие имени "Эмери"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эмери"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эмери"
                                NationPersonalName nationPersonalName = new("system", 0.33, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гарральэль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гарральэль"))
                    {
                        //Получаем имя "Гарральэль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гарральэль");

                        //Проверяем наличие имени "Гарральэль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Гарральэль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Гарральэль"
                                NationPersonalName nationPersonalName = new("system", 1.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Холалет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Холалет"))
                    {
                        //Получаем имя "Холалет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Холалет");

                        //Проверяем наличие имени "Холалет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Холалет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Холалет"
                                NationPersonalName nationPersonalName = new("system", 1.2, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ветрокрылая"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ветрокрылая"))
                    {
                        //Получаем имя "Ветрокрылая"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ветрокрылая");

                        //Проверяем наличие имени "Ветрокрылая"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Ветрокрылая"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Ветрокрылая"
                                NationPersonalName nationPersonalName = new("system", 0.99, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Идриль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Идриль"))
                    {
                        //Получаем имя "Идриль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Идриль");

                        //Проверяем наличие имени "Идриль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Идриль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Идриль"
                                NationPersonalName nationPersonalName = new("system", 1.35, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арене"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арене"))
                    {
                        //Получаем имя "Арене"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арене");

                        //Проверяем наличие имени "Арене"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Арене"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Арене"
                                NationPersonalName nationPersonalName = new("system", 0.72, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Амани"
                    if (_repository.PersonalNames.Any(x => x.Name == "Амани"))
                    {
                        //Получаем имя "Амани"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Амани");

                        //Проверяем наличие имени "Амани"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Амани"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Амани"
                                NationPersonalName nationPersonalName = new("system", 1.28, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иримэ"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иримэ"))
                    {
                        //Получаем имя "Иримэ"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иримэ");

                        //Проверяем наличие имени "Иримэ"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Иримэ"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Иримэ"
                                NationPersonalName nationPersonalName = new("system", 1.82, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Индис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Индис"))
                    {
                        //Получаем имя "Индис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Индис");

                        //Проверяем наличие имени "Индис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Индис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Индис"
                                NationPersonalName nationPersonalName = new("system", 0.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ниенор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ниенор"))
                    {
                        //Получаем имя "Ниенор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ниенор");

                        //Проверяем наличие имени "Ниенор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Ниенор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Ниенор"
                                NationPersonalName nationPersonalName = new("system", 1.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Нерданель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Нерданель"))
                    {
                        //Получаем имя "Нерданель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Нерданель");

                        //Проверяем наличие имени "Нерданель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Нерданель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Нерданель"
                                NationPersonalName nationPersonalName = new("system", 1.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мормегиль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мормегиль"))
                    {
                        //Получаем имя "Мормегиль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мормегиль");

                        //Проверяем наличие имени "Мормегиль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Мормегиль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Мормегиль"
                                NationPersonalName nationPersonalName = new("system", 1.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Вана"
                    if (_repository.PersonalNames.Any(x => x.Name == "Вана"))
                    {
                        //Получаем имя "Вана"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Вана");

                        //Проверяем наличие имени "Вана"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Вана"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Вана"
                                NationPersonalName nationPersonalName = new("system", 1.35, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эйлинель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эйлинель"))
                    {
                        //Получаем имя "Эйлинель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эйлинель");

                        //Проверяем наличие имени "Эйлинель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эйлинель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эйлинель"
                                NationPersonalName nationPersonalName = new("system", 1.61, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эльвинг"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эльвинг"))
                    {
                        //Получаем имя "Эльвинг"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эльвинг");

                        //Проверяем наличие имени "Эльвинг"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эльвинг"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эльвинг"
                                NationPersonalName nationPersonalName = new("system", 0.89, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элентари"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элентари"))
                    {
                        //Получаем имя "Элентари"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элентари");

                        //Проверяем наличие имени "Элентари"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Элентари"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Элентари"
                                NationPersonalName nationPersonalName = new("system", 0.84, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Уртель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Уртель"))
                    {
                        //Получаем имя "Уртель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Уртель");

                        //Проверяем наличие имени "Уртель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Уртель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Уртель"
                                NationPersonalName nationPersonalName = new("system", 1.68, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Валандиль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Валандиль"))
                    {
                        //Получаем имя "Валандиль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Валандиль");

                        //Проверяем наличие имени "Валандиль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Валандиль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Валандиль"
                                NationPersonalName nationPersonalName = new("system", 0.74, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рана"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рана"))
                    {
                        //Получаем имя "Рана"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рана");

                        //Проверяем наличие имени "Рана"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Рана"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Рана"
                                NationPersonalName nationPersonalName = new("system", 0.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Миримэ"
                    if (_repository.PersonalNames.Any(x => x.Name == "Миримэ"))
                    {
                        //Получаем имя "Миримэ"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Миримэ");

                        //Проверяем наличие имени "Миримэ"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Миримэ"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Миримэ"
                                NationPersonalName nationPersonalName = new("system", 0.97, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анариэль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анариэль"))
                    {
                        //Получаем имя "Анариэль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анариэль");

                        //Проверяем наличие имени "Анариэль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Анариэль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Анариэль"
                                NationPersonalName nationPersonalName = new("system", 1.63, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элениэль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элениэль"))
                    {
                        //Получаем имя "Элениэль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элениэль");

                        //Проверяем наличие имени "Элениэль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Элениэль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Элениэль"
                                NationPersonalName nationPersonalName = new("system", 0.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Миримэ"
                    if (_repository.PersonalNames.Any(x => x.Name == "Миримэ"))
                    {
                        //Получаем имя "Миримэ"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Миримэ");

                        //Проверяем наличие имени "Миримэ"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Миримэ"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Миримэ"
                                NationPersonalName nationPersonalName = new("system", 1.45, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анкалимэ"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анкалимэ"))
                    {
                        //Получаем имя "Анкалимэ"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анкалимэ");

                        //Проверяем наличие имени "Анкалимэ"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Анкалимэ"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Анкалимэ"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эариль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эариль"))
                    {
                        //Получаем имя "Эариль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эариль");

                        //Проверяем наличие имени "Эариль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эариль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эариль"
                                NationPersonalName nationPersonalName = new("system", 0.58, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аркуэне"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аркуэне"))
                    {
                        //Получаем имя "Аркуэне"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аркуэне");

                        //Проверяем наличие имени "Аркуэне"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Аркуэне"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Аркуэне"
                                NationPersonalName nationPersonalName = new("system", 1.72, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Таурэтари"
                    if (_repository.PersonalNames.Any(x => x.Name == "Таурэтари"))
                    {
                        //Получаем имя "Таурэтари"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Таурэтари");

                        //Проверяем наличие имени "Таурэтари"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Таурэтари"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Таурэтари"
                                NationPersonalName nationPersonalName = new("system", 0.2, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эдделос"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эдделос"))
                    {
                        //Получаем имя "Эдделос"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эдделос");

                        //Проверяем наличие имени "Эдделос"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с именем "Эдделос"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Альв" с именем "Эдделос"
                                NationPersonalName nationPersonalName = new("system", 0.93, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            //Проверяем наличие нации "Западный вампир"
            if (_repository.Nations.Any(x => x.Name == "Западный вампир"))
            {
                //Получаем нацию "Западный вампир"
                Nation? nation = _repository.Nations.FirstOrDefault(x => x.Name == "Западный вампир");

                //Проверяем наличие нации "Западный вампир"
                if (nation != null)
                {
                    //Проверяем наличие имени "Алан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Алан"))
                    {
                        //Получаем имя "Алан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Алан");

                        //Проверяем наличие имени "Алан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Алан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Алан"
                                NationPersonalName nationPersonalName = new("system", 1.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аластер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аластер"))
                    {
                        //Получаем имя "Аластер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аластер");

                        //Проверяем наличие имени "Аластер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Аластер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Аластер"
                                NationPersonalName nationPersonalName = new("system", 0.6, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Албан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Албан"))
                    {
                        //Получаем имя "Албан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Албан");

                        //Проверяем наличие имени "Албан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Албан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Албан"
                                NationPersonalName nationPersonalName = new("system", 1.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Алпин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Алпин"))
                    {
                        //Получаем имя "Алпин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Алпин");

                        //Проверяем наличие имени "Алпин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Алпин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Алпин"
                                NationPersonalName nationPersonalName = new("system", 0.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ангус"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ангус"))
                    {
                        //Получаем имя "Ангус"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ангус");

                        //Проверяем наличие имени "Ангус"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Ангус"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Ангус"
                                NationPersonalName nationPersonalName = new("system", 0.08, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Блан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Блан"))
                    {
                        //Получаем имя "Блан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Блан");

                        //Проверяем наличие имени "Блан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Блан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Блан"
                                NationPersonalName nationPersonalName = new("system", 0.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Блейер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Блейер"))
                    {
                        //Получаем имя "Блейер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Блейер");

                        //Проверяем наличие имени "Блейер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Блейер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Блейер"
                                NationPersonalName nationPersonalName = new("system", 0.04, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бойд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бойд"))
                    {
                        //Получаем имя "Бойд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бойд");

                        //Проверяем наличие имени "Бойд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Бойд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Бойд"
                                NationPersonalName nationPersonalName = new("system", 1.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Вильям"
                    if (_repository.PersonalNames.Any(x => x.Name == "Вильям"))
                    {
                        //Получаем имя "Вильям"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Вильям");

                        //Проверяем наличие имени "Вильям"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Вильям"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Вильям"
                                NationPersonalName nationPersonalName = new("system", 1.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дерек"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дерек"))
                    {
                        //Получаем имя "Дерек"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дерек");

                        //Проверяем наличие имени "Дерек"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дерек"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дерек"
                                NationPersonalName nationPersonalName = new("system", 0.14, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Джек"
                    if (_repository.PersonalNames.Any(x => x.Name == "Джек"))
                    {
                        //Получаем имя "Джек"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Джек");

                        //Проверяем наличие имени "Джек"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Джек"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Джек"
                                NationPersonalName nationPersonalName = new("system", 1.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Джиллиан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Джиллиан"))
                    {
                        //Получаем имя "Джиллиан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Джиллиан");

                        //Проверяем наличие имени "Джиллиан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Джиллиан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Джиллиан"
                                NationPersonalName nationPersonalName = new("system", 1.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дональд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дональд"))
                    {
                        //Получаем имя "Дональд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дональд");

                        //Проверяем наличие имени "Дональд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дональд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дональд"
                                NationPersonalName nationPersonalName = new("system", 0.06, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дугал"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дугал"))
                    {
                        //Получаем имя "Дугал"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дугал");

                        //Проверяем наличие имени "Дугал"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дугал"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дугал"
                                NationPersonalName nationPersonalName = new("system", 0.68, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дуглас"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дуглас"))
                    {
                        //Получаем имя "Дуглас"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дуглас");

                        //Проверяем наличие имени "Дуглас"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дуглас"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дуглас"
                                NationPersonalName nationPersonalName = new("system", 1.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дункан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дункан"))
                    {
                        //Получаем имя "Дункан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дункан");

                        //Проверяем наличие имени "Дункан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дункан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дункан"
                                NationPersonalName nationPersonalName = new("system", 1.13, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гевин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гевин"))
                    {
                        //Получаем имя "Гевин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гевин");

                        //Проверяем наличие имени "Гевин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Гевин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Гевин"
                                NationPersonalName nationPersonalName = new("system", 1.13, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гиллис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гиллис"))
                    {
                        //Получаем имя "Гиллис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гиллис");

                        //Проверяем наличие имени "Гиллис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Гиллис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Гиллис"
                                NationPersonalName nationPersonalName = new("system", 1.8, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гордон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гордон"))
                    {
                        //Получаем имя "Гордон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гордон");

                        //Проверяем наличие имени "Гордон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Гордон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Гордон"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Грир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Грир"))
                    {
                        //Получаем имя "Грир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Грир");

                        //Проверяем наличие имени "Грир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Грир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Грир"
                                NationPersonalName nationPersonalName = new("system", 1.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Камерон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Камерон"))
                    {
                        //Получаем имя "Камерон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Камерон");

                        //Проверяем наличие имени "Камерон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Камерон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Камерон"
                                NationPersonalName nationPersonalName = new("system", 1.33, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Клайд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Клайд"))
                    {
                        //Получаем имя "Клайд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Клайд");

                        //Проверяем наличие имени "Клайд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Клайд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Клайд"
                                NationPersonalName nationPersonalName = new("system", 1.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кеннет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кеннет"))
                    {
                        //Получаем имя "Кеннет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кеннет");

                        //Проверяем наличие имени "Кеннет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Кеннет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Кеннет"
                                NationPersonalName nationPersonalName = new("system", 1.26, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Колин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Колин"))
                    {
                        //Получаем имя "Колин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Колин");

                        //Проверяем наличие имени "Колин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Колин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Колин"
                                NationPersonalName nationPersonalName = new("system", 1.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Крег"
                    if (_repository.PersonalNames.Any(x => x.Name == "Крег"))
                    {
                        //Получаем имя "Крег"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Крег");

                        //Проверяем наличие имени "Крег"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Крег"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Крег"
                                NationPersonalName nationPersonalName = new("system", 1.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лахлан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лахлан"))
                    {
                        //Получаем имя "Лахлан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лахлан");

                        //Проверяем наличие имени "Лахлан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Лахлан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Лахлан"
                                NationPersonalName nationPersonalName = new("system", 0.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лорн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лорн"))
                    {
                        //Получаем имя "Лорн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лорн");

                        //Проверяем наличие имени "Лорн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Лорн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Лорн"
                                NationPersonalName nationPersonalName = new("system", 0.79, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Льюс"
                    if (_repository.PersonalNames.Any(x => x.Name == "Льюс"))
                    {
                        //Получаем имя "Льюс"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Льюс");

                        //Проверяем наличие имени "Льюс"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Льюс"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Льюс"
                                NationPersonalName nationPersonalName = new("system", 1.95, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Магнус"
                    if (_repository.PersonalNames.Any(x => x.Name == "Магнус"))
                    {
                        //Получаем имя "Магнус"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Магнус");

                        //Проверяем наличие имени "Магнус"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Магнус"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Магнус"
                                NationPersonalName nationPersonalName = new("system", 1.03, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Малькольм"
                    if (_repository.PersonalNames.Any(x => x.Name == "Малькольм"))
                    {
                        //Получаем имя "Малькольм"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Малькольм");

                        //Проверяем наличие имени "Малькольм"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Малькольм"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Малькольм"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Миррей"
                    if (_repository.PersonalNames.Any(x => x.Name == "Миррей"))
                    {
                        //Получаем имя "Миррей"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Миррей");

                        //Проверяем наличие имени "Миррей"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Миррей"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Миррей"
                                NationPersonalName nationPersonalName = new("system", 1.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Нейл"
                    if (_repository.PersonalNames.Any(x => x.Name == "Нейл"))
                    {
                        //Получаем имя "Нейл"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Нейл");

                        //Проверяем наличие имени "Нейл"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Нейл"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Нейл"
                                NationPersonalName nationPersonalName = new("system", 1.16, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Невин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Невин"))
                    {
                        //Получаем имя "Невин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Невин");

                        //Проверяем наличие имени "Невин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Невин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Невин"
                                NationPersonalName nationPersonalName = new("system", 1.2, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Олей"
                    if (_repository.PersonalNames.Any(x => x.Name == "Олей"))
                    {
                        //Получаем имя "Олей"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Олей");

                        //Проверяем наличие имени "Олей"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Олей"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Олей"
                                NationPersonalName nationPersonalName = new("system", 1.94, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Райтор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Райтор"))
                    {
                        //Получаем имя "Райтор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Райтор");

                        //Проверяем наличие имени "Райтор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Райтор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Райтор"
                                NationPersonalName nationPersonalName = new("system", 1.93, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ренальд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ренальд"))
                    {
                        //Получаем имя "Ренальд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ренальд");

                        //Проверяем наличие имени "Ренальд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Ренальд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Ренальд"
                                NationPersonalName nationPersonalName = new("system", 1.76, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рой"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рой"))
                    {
                        //Получаем имя "Рой"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рой");

                        //Проверяем наличие имени "Рой"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Рой"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Рой"
                                NationPersonalName nationPersonalName = new("system", 0.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рори"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рори"))
                    {
                        //Получаем имя "Рори"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рори");

                        //Проверяем наличие имени "Рори"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Рори"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Рори"
                                NationPersonalName nationPersonalName = new("system", 0.75, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Росс"
                    if (_repository.PersonalNames.Any(x => x.Name == "Росс"))
                    {
                        //Получаем имя "Росс"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Росс");

                        //Проверяем наличие имени "Росс"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Росс"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Росс"
                                NationPersonalName nationPersonalName = new("system", 0.13, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Стюарт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Стюарт"))
                    {
                        //Получаем имя "Стюарт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Стюарт");

                        //Проверяем наличие имени "Стюарт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Стюарт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Стюарт"
                                NationPersonalName nationPersonalName = new("system", 1.93, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тевиш"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тевиш"))
                    {
                        //Получаем имя "Тевиш"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тевиш");

                        //Проверяем наличие имени "Тевиш"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Тевиш"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Тевиш"
                                NationPersonalName nationPersonalName = new("system", 1.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Уоллес"
                    if (_repository.PersonalNames.Any(x => x.Name == "Уоллес"))
                    {
                        //Получаем имя "Уоллес"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Уоллес");

                        //Проверяем наличие имени "Уоллес"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Уоллес"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Уоллес"
                                NationPersonalName nationPersonalName = new("system", 1.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Шолто"
                    if (_repository.PersonalNames.Any(x => x.Name == "Шолто"))
                    {
                        //Получаем имя "Шолто"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Шолто");

                        //Проверяем наличие имени "Шолто"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Шолто"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Шолто"
                                NationPersonalName nationPersonalName = new("system", 1.5, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фергус"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фергус"))
                    {
                        //Получаем имя "Фергус"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фергус");

                        //Проверяем наличие имени "Фергус"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Фергус"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Фергус"
                                NationPersonalName nationPersonalName = new("system", 0.59, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фингал"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фингал"))
                    {
                        //Получаем имя "Фингал"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фингал");

                        //Проверяем наличие имени "Фингал"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Фингал"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Фингал"
                                NationPersonalName nationPersonalName = new("system", 0.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Финлей"
                    if (_repository.PersonalNames.Any(x => x.Name == "Финлей"))
                    {
                        //Получаем имя "Финлей"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Финлей");

                        //Проверяем наличие имени "Финлей"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Финлей"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Финлей"
                                NationPersonalName nationPersonalName = new("system", 1.82, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хэмиш"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хэмиш"))
                    {
                        //Получаем имя "Хэмиш"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хэмиш");

                        //Проверяем наличие имени "Хэмиш"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Хэмиш"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Хэмиш"
                                NationPersonalName nationPersonalName = new("system", 1.37, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хьюго"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хьюго"))
                    {
                        //Получаем имя "Хьюго"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хьюго");

                        //Проверяем наличие имени "Хьюго"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Хьюго"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Хьюго"
                                NationPersonalName nationPersonalName = new("system", 0.82, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эван"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эван"))
                    {
                        //Получаем имя "Эван"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эван");

                        //Проверяем наличие имени "Эван"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эван"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эван"
                                NationPersonalName nationPersonalName = new("system", 0.71, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эндрю"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эндрю"))
                    {
                        //Получаем имя "Эндрю"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эндрю");

                        //Проверяем наличие имени "Эндрю"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эндрю"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эндрю"
                                NationPersonalName nationPersonalName = new("system", 1.67, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Алва"
                    if (_repository.PersonalNames.Any(x => x.Name == "Алва"))
                    {
                        //Получаем имя "Алва"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Алва");

                        //Проверяем наличие имени "Алва"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Алва"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Алва"
                                NationPersonalName nationPersonalName = new("system", 1.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Андрина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Андрина"))
                    {
                        //Получаем имя "Андрина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Андрина");

                        //Проверяем наличие имени "Андрина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Андрина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Андрина"
                                NationPersonalName nationPersonalName = new("system", 1.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аннабель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аннабель"))
                    {
                        //Получаем имя "Аннабель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аннабель");

                        //Проверяем наличие имени "Аннабель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Аннабель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Аннабель"
                                NationPersonalName nationPersonalName = new("system", 1.73, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арабелла"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арабелла"))
                    {
                        //Получаем имя "Арабелла"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арабелла");

                        //Проверяем наличие имени "Арабелла"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Арабелла"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Арабелла"
                                NationPersonalName nationPersonalName = new("system", 1.11, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арлайн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арлайн"))
                    {
                        //Получаем имя "Арлайн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арлайн");

                        //Проверяем наличие имени "Арлайн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Арлайн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Арлайн"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бета"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бета"))
                    {
                        //Получаем имя "Бета"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бета");

                        //Проверяем наличие имени "Бета"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Бета"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Бета"
                                NationPersonalName nationPersonalName = new("system", 0.59, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бонни"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бонни"))
                    {
                        //Получаем имя "Бонни"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бонни");

                        //Проверяем наличие имени "Бонни"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Бонни"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Бонни"
                                NationPersonalName nationPersonalName = new("system", 0.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бренда"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бренда"))
                    {
                        //Получаем имя "Бренда"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бренда");

                        //Проверяем наличие имени "Бренда"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Бренда"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Бренда"
                                NationPersonalName nationPersonalName = new("system", 0.71, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Вилма"
                    if (_repository.PersonalNames.Any(x => x.Name == "Вилма"))
                    {
                        //Получаем имя "Вилма"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Вилма");

                        //Проверяем наличие имени "Вилма"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Вилма"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Вилма"
                                NationPersonalName nationPersonalName = new("system", 0.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Давина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Давина"))
                    {
                        //Получаем имя "Давина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Давина");

                        //Проверяем наличие имени "Давина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Давина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Давина"
                                NationPersonalName nationPersonalName = new("system", 0.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дженет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дженет"))
                    {
                        //Получаем имя "Дженет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дженет");

                        //Проверяем наличие имени "Дженет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дженет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дженет"
                                NationPersonalName nationPersonalName = new("system", 0.72, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Джесси"
                    if (_repository.PersonalNames.Any(x => x.Name == "Джесси"))
                    {
                        //Получаем имя "Джесси"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Джесси");

                        //Проверяем наличие имени "Джесси"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Джесси"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Джесси"
                                NationPersonalName nationPersonalName = new("system", 1.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Джинни"
                    if (_repository.PersonalNames.Any(x => x.Name == "Джинни"))
                    {
                        //Получаем имя "Джинни"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Джинни");

                        //Проверяем наличие имени "Джинни"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Джинни"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Джинни"
                                NationPersonalName nationPersonalName = new("system", 1.62, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дональдина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дональдина"))
                    {
                        //Получаем имя "Дональдина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дональдина");

                        //Проверяем наличие имени "Дональдина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дональдина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дональдина"
                                NationPersonalName nationPersonalName = new("system", 0.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дональда"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дональда"))
                    {
                        //Получаем имя "Дональда"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дональда");

                        //Проверяем наличие имени "Дональда"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Дональда"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Дональда"
                                NationPersonalName nationPersonalName = new("system", 0.14, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ина"))
                    {
                        //Получаем имя "Ина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ина");

                        //Проверяем наличие имени "Ина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Ина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Ина"
                                NationPersonalName nationPersonalName = new("system", 1.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иннес"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иннес"))
                    {
                        //Получаем имя "Иннес"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иннес");

                        //Проверяем наличие имени "Иннес"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Иннес"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Иннес"
                                NationPersonalName nationPersonalName = new("system", 1.41, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Исла"
                    if (_repository.PersonalNames.Any(x => x.Name == "Исла"))
                    {
                        //Получаем имя "Исла"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Исла");

                        //Проверяем наличие имени "Исла"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Исла"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Исла"
                                NationPersonalName nationPersonalName = new("system", 0.38, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иона"))
                    {
                        //Получаем имя "Иона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иона");

                        //Проверяем наличие имени "Иона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Иона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Иона"
                                NationPersonalName nationPersonalName = new("system", 1.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Катриона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Катриона"))
                    {
                        //Получаем имя "Катриона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Катриона");

                        //Проверяем наличие имени "Катриона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Катриона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Катриона"
                                NationPersonalName nationPersonalName = new("system", 0.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кателла"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кателла"))
                    {
                        //Получаем имя "Кателла"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кателла");

                        //Проверяем наличие имени "Кателла"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Кателла"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Кателла"
                                NationPersonalName nationPersonalName = new("system", 0.28, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кенна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кенна"))
                    {
                        //Получаем имя "Кенна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кенна");

                        //Проверяем наличие имени "Кенна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Кенна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Кенна"
                                NationPersonalName nationPersonalName = new("system", 0.31, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Колина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Колина"))
                    {
                        //Получаем имя "Колина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Колина");

                        //Проверяем наличие имени "Колина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Колина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Колина"
                                NationPersonalName nationPersonalName = new("system", 1.76, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кирсти"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кирсти"))
                    {
                        //Получаем имя "Кирсти"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кирсти");

                        //Проверяем наличие имени "Кирсти"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Кирсти"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Кирсти"
                                NationPersonalName nationPersonalName = new("system", 1.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лесли"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лесли"))
                    {
                        //Получаем имя "Лесли"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лесли");

                        //Проверяем наличие имени "Лесли"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Лесли"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Лесли"
                                NationPersonalName nationPersonalName = new("system", 0.82, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лилиас"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лилиас"))
                    {
                        //Получаем имя "Лилиас"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лилиас");

                        //Проверяем наличие имени "Лилиас"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Лилиас"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Лилиас"
                                NationPersonalName nationPersonalName = new("system", 1.16, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лорна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лорна"))
                    {
                        //Получаем имя "Лорна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лорна");

                        //Проверяем наличие имени "Лорна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Лорна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Лорна"
                                NationPersonalName nationPersonalName = new("system", 1.63, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Маири"
                    if (_repository.PersonalNames.Any(x => x.Name == "Маири"))
                    {
                        //Получаем имя "Маири"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Маири");

                        //Проверяем наличие имени "Маири"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Маири"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Маири"
                                NationPersonalName nationPersonalName = new("system", 0.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мальвина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мальвина"))
                    {
                        //Получаем имя "Мальвина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мальвина");

                        //Проверяем наличие имени "Мальвина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Мальвина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Мальвина"
                                NationPersonalName nationPersonalName = new("system", 1.8, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мариотта"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мариотта"))
                    {
                        //Получаем имя "Мариотта"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мариотта");

                        //Проверяем наличие имени "Мариотта"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Мариотта"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Мариотта"
                                NationPersonalName nationPersonalName = new("system", 1.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Морна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Морна"))
                    {
                        //Получаем имя "Морна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Морна");

                        //Проверяем наличие имени "Морна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Морна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Морна"
                                NationPersonalName nationPersonalName = new("system", 0.94, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мери"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мери"))
                    {
                        //Получаем имя "Мери"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мери");

                        //Проверяем наличие имени "Мери"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Мери"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Мери"
                                NationPersonalName nationPersonalName = new("system", 1.33, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Несса"
                    if (_repository.PersonalNames.Any(x => x.Name == "Несса"))
                    {
                        //Получаем имя "Несса"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Несса");

                        //Проверяем наличие имени "Несса"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Несса"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Несса"
                                NationPersonalName nationPersonalName = new("system", 0.6, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Несси"
                    if (_repository.PersonalNames.Any(x => x.Name == "Несси"))
                    {
                        //Получаем имя "Несси"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Несси");

                        //Проверяем наличие имени "Несси"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Несси"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Несси"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Осла"
                    if (_repository.PersonalNames.Any(x => x.Name == "Осла"))
                    {
                        //Получаем имя "Осла"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Осла");

                        //Проверяем наличие имени "Осла"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Осла"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Осла"
                                NationPersonalName nationPersonalName = new("system", 1.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рона"))
                    {
                        //Получаем имя "Рона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рона");

                        //Проверяем наличие имени "Рона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Рона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Рона"
                                NationPersonalName nationPersonalName = new("system", 0.46, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сельма"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сельма"))
                    {
                        //Получаем имя "Сельма"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сельма");

                        //Проверяем наличие имени "Сельма"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Сельма"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Сельма"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фанни"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фанни"))
                    {
                        //Получаем имя "Фанни"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фанни");

                        //Проверяем наличие имени "Фанни"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Фанни"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Фанни"
                                NationPersonalName nationPersonalName = new("system", 0.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фиона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фиона"))
                    {
                        //Получаем имя "Фиона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фиона");

                        //Проверяем наличие имени "Фиона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Фиона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Фиона"
                                NationPersonalName nationPersonalName = new("system", 1.23, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хейзер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хейзер"))
                    {
                        //Получаем имя "Хейзер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хейзер");

                        //Проверяем наличие имени "Хейзер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Хейзер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Хейзер"
                                NationPersonalName nationPersonalName = new("system", 1.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Шина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Шина"))
                    {
                        //Получаем имя "Шина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Шина");

                        //Проверяем наличие имени "Шина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Шина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Шина"
                                NationPersonalName nationPersonalName = new("system", 1.89, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Шона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Шона"))
                    {
                        //Получаем имя "Шона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Шона");

                        //Проверяем наличие имени "Шона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Шона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Шона"
                                NationPersonalName nationPersonalName = new("system", 0.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эдана"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эдана"))
                    {
                        //Получаем имя "Эдана"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эдана");

                        //Проверяем наличие имени "Эдана"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эдана"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эдана"
                                NationPersonalName nationPersonalName = new("system", 1.06, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эдвина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эдвина"))
                    {
                        //Получаем имя "Эдвина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эдвина");

                        //Проверяем наличие имени "Эдвина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эдвина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эдвина"
                                NationPersonalName nationPersonalName = new("system", 0.86, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эйлин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эйлин"))
                    {
                        //Получаем имя "Эйлин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эйлин");

                        //Проверяем наличие имени "Эйлин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эйлин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эйлин"
                                NationPersonalName nationPersonalName = new("system", 0.35, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эйли"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эйли"))
                    {
                        //Получаем имя "Эйли"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эйли");

                        //Проверяем наличие имени "Эйли"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эйли"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эйли"
                                NationPersonalName nationPersonalName = new("system", 0.16, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эльза"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эльза"))
                    {
                        //Получаем имя "Эльза"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эльза");

                        //Проверяем наличие имени "Эльза"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эльза"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эльза"
                                NationPersonalName nationPersonalName = new("system", 0.07, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элисон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элисон"))
                    {
                        //Получаем имя "Элисон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элисон");

                        //Проверяем наличие имени "Элисон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Элисон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Элисон"
                                NationPersonalName nationPersonalName = new("system", 0.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эна"))
                    {
                        //Получаем имя "Эна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эна");

                        //Проверяем наличие имени "Эна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эна"
                                NationPersonalName nationPersonalName = new("system", 1.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эффи"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эффи"))
                    {
                        //Получаем имя "Эффи"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эффи");

                        //Проверяем наличие имени "Эффи"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Западный вампир" с именем "Эффи"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Западный вампир" с именем "Эффи"
                                NationPersonalName nationPersonalName = new("system", 1.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            //Проверяем наличие нации "Восточный вампир"
            if (_repository.Nations.Any(x => x.Name == "Восточный вампир"))
            {
                //Получаем нацию "Восточный вампир"
                Nation? nation = _repository.Nations.FirstOrDefault(x => x.Name == "Восточный вампир");

                //Проверяем наличие нации "Восточный вампир"
                if (nation != null)
                {
                    //Проверяем наличие имени "Аерон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аерон"))
                    {
                        //Получаем имя "Аерон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аерон");

                        //Проверяем наличие имени "Аерон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Аерон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Аерон"
                                NationPersonalName nationPersonalName = new("system", 0.71, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Айолин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Айолин"))
                    {
                        //Получаем имя "Айолин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Айолин");

                        //Проверяем наличие имени "Айолин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Айолин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Айолин"
                                NationPersonalName nationPersonalName = new("system", 1.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Айоло"
                    if (_repository.PersonalNames.Any(x => x.Name == "Айоло"))
                    {
                        //Получаем имя "Айоло"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Айоло");

                        //Проверяем наличие имени "Айоло"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Айоло"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Айоло"
                                NationPersonalName nationPersonalName = new("system", 1.71, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Айорверт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Айорверт"))
                    {
                        //Получаем имя "Айорверт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Айорверт");

                        //Проверяем наличие имени "Айорверт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Айорверт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Айорверт"
                                NationPersonalName nationPersonalName = new("system", 0.43, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Алед"
                    if (_repository.PersonalNames.Any(x => x.Name == "Алед"))
                    {
                        //Получаем имя "Алед"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Алед");

                        //Проверяем наличие имени "Алед"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Алед"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Алед"
                                NationPersonalName nationPersonalName = new("system", 0.59, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Алун"
                    if (_repository.PersonalNames.Any(x => x.Name == "Алун"))
                    {
                        //Получаем имя "Алун"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Алун");

                        //Проверяем наличие имени "Алун"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Алун"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Алун"
                                NationPersonalName nationPersonalName = new("system", 1.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анеирин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анеирин"))
                    {
                        //Получаем имя "Анеирин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анеирин");

                        //Проверяем наличие имени "Анеирин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Анеирин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Анеирин"
                                NationPersonalName nationPersonalName = new("system", 1.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анеурин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анеурин"))
                    {
                        //Получаем имя "Анеурин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анеурин");

                        //Проверяем наличие имени "Анеурин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Анеурин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Анеурин"
                                NationPersonalName nationPersonalName = new("system", 0.35, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арвэль"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арвэль"))
                    {
                        //Получаем имя "Арвэль"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арвэль");

                        //Проверяем наличие имени "Арвэль"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Арвэль"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Арвэль"
                                NationPersonalName nationPersonalName = new("system", 0.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Афон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Афон"))
                    {
                        //Получаем имя "Афон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Афон");

                        //Проверяем наличие имени "Афон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Афон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Афон"
                                NationPersonalName nationPersonalName = new("system", 1.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бедвир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бедвир"))
                    {
                        //Получаем имя "Бедвир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бедвир");

                        //Проверяем наличие имени "Бедвир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бедвир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бедвир"
                                NationPersonalName nationPersonalName = new("system", 0.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бели"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бели"))
                    {
                        //Получаем имя "Бели"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бели");

                        //Проверяем наличие имени "Бели"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бели"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бели"
                                NationPersonalName nationPersonalName = new("system", 0.54, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Беруин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Беруин"))
                    {
                        //Получаем имя "Беруин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Беруин");

                        //Проверяем наличие имени "Беруин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Беруин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Беруин"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бренин ллвид"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бренин ллвид"))
                    {
                        //Получаем имя "Бренин ллвид"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бренин ллвид");

                        //Проверяем наличие имени "Бренин ллвид"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бренин ллвид"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бренин ллвид"
                                NationPersonalName nationPersonalName = new("system", 1.48, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бриин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бриин"))
                    {
                        //Получаем имя "Бриин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бриин");

                        //Проверяем наличие имени "Бриин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бриин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бриин"
                                NationPersonalName nationPersonalName = new("system", 1.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брин"))
                    {
                        //Получаем имя "Брин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брин");

                        //Проверяем наличие имени "Брин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брин"
                                NationPersonalName nationPersonalName = new("system", 1.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бринмор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бринмор"))
                    {
                        //Получаем имя "Бринмор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бринмор");

                        //Проверяем наличие имени "Бринмор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бринмор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бринмор"
                                NationPersonalName nationPersonalName = new("system", 1.86, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бринн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бринн"))
                    {
                        //Получаем имя "Бринн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бринн");

                        //Проверяем наличие имени "Бринн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бринн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бринн"
                                NationPersonalName nationPersonalName = new("system", 0.96, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бричэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бричэн"))
                    {
                        //Получаем имя "Бричэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бричэн");

                        //Проверяем наличие имени "Бричэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бричэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бричэн"
                                NationPersonalName nationPersonalName = new("system", 1.69, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брэйт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брэйт"))
                    {
                        //Получаем имя "Брэйт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брэйт");

                        //Проверяем наличие имени "Брэйт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брэйт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брэйт"
                                NationPersonalName nationPersonalName = new("system", 0.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бэл"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бэл"))
                    {
                        //Получаем имя "Бэл"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бэл");

                        //Проверяем наличие имени "Бэл"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бэл"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бэл"
                                NationPersonalName nationPersonalName = new("system", 1.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Вайнн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Вайнн"))
                    {
                        //Получаем имя "Вайнн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Вайнн");

                        //Проверяем наличие имени "Вайнн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Вайнн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Вайнн"
                                NationPersonalName nationPersonalName = new("system", 1.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ваугхан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ваугхан"))
                    {
                        //Получаем имя "Ваугхан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ваугхан");

                        //Проверяем наличие имени "Ваугхан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ваугхан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ваугхан"
                                NationPersonalName nationPersonalName = new("system", 1.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ваугхн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ваугхн"))
                    {
                        //Получаем имя "Ваугхн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ваугхн");

                        //Проверяем наличие имени "Ваугхн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ваугхн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ваугхн"
                                NationPersonalName nationPersonalName = new("system", 1.98, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Винн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Винн"))
                    {
                        //Получаем имя "Винн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Винн");

                        //Проверяем наличие имени "Винн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Винн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Винн"
                                NationPersonalName nationPersonalName = new("system", 1.97, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Винфор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Винфор"))
                    {
                        //Получаем имя "Винфор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Винфор");

                        //Проверяем наличие имени "Винфор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Винфор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Винфор"
                                NationPersonalName nationPersonalName = new("system", 0.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Вмффре"
                    if (_repository.PersonalNames.Any(x => x.Name == "Вмффре"))
                    {
                        //Получаем имя "Вмффре"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Вмффре");

                        //Проверяем наличие имени "Вмффре"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Вмффре"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Вмффре"
                                NationPersonalName nationPersonalName = new("system", 1.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Галахад"
                    if (_repository.PersonalNames.Any(x => x.Name == "Галахад"))
                    {
                        //Получаем имя "Галахад"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Галахад");

                        //Проверяем наличие имени "Галахад"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Галахад"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Галахад"
                                NationPersonalName nationPersonalName = new("system", 0.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гарет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гарет"))
                    {
                        //Получаем имя "Гарет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гарет");

                        //Проверяем наличие имени "Гарет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гарет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гарет"
                                NationPersonalName nationPersonalName = new("system", 0.99, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гваллтер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гваллтер"))
                    {
                        //Получаем имя "Гваллтер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гваллтер");

                        //Проверяем наличие имени "Гваллтер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гваллтер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гваллтер"
                                NationPersonalName nationPersonalName = new("system", 0.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвизир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвизир"))
                    {
                        //Получаем имя "Гвизир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвизир");

                        //Проверяем наличие имени "Гвизир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвизир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвизир"
                                NationPersonalName nationPersonalName = new("system", 1.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвил"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвил"))
                    {
                        //Получаем имя "Гвил"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвил");

                        //Проверяем наличие имени "Гвил"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвил"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвил"
                                NationPersonalName nationPersonalName = new("system", 1.2, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвилим"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвилим"))
                    {
                        //Получаем имя "Гвилим"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвилим");

                        //Проверяем наличие имени "Гвилим"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвилим"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвилим"
                                NationPersonalName nationPersonalName = new("system", 1.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвиллим"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвиллим"))
                    {
                        //Получаем имя "Гвиллим"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвиллим");

                        //Проверяем наличие имени "Гвиллим"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвиллим"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвиллим"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвинн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвинн"))
                    {
                        //Получаем имя "Гвинн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвинн");

                        //Проверяем наличие имени "Гвинн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвинн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвинн"
                                NationPersonalName nationPersonalName = new("system", 1.31, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвинфор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвинфор"))
                    {
                        //Получаем имя "Гвинфор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвинфор");

                        //Проверяем наличие имени "Гвинфор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвинфор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвинфор"
                                NationPersonalName nationPersonalName = new("system", 1.48, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвледиг"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвледиг"))
                    {
                        //Получаем имя "Гвледиг"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвледиг");

                        //Проверяем наличие имени "Гвледиг"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвледиг"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвледиг"
                                NationPersonalName nationPersonalName = new("system", 1.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гволкхгвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гволкхгвин"))
                    {
                        //Получаем имя "Гволкхгвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гволкхгвин");

                        //Проверяем наличие имени "Гволкхгвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гволкхгвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гволкхгвин"
                                NationPersonalName nationPersonalName = new("system", 0.95, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гераинт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гераинт"))
                    {
                        //Получаем имя "Гераинт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гераинт");

                        //Проверяем наличие имени "Гераинт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гераинт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гераинт"
                                NationPersonalName nationPersonalName = new("system", 1.63, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гесим"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гесим"))
                    {
                        //Получаем имя "Гесим"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гесим");

                        //Проверяем наличие имени "Гесим"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гесим"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гесим"
                                NationPersonalName nationPersonalName = new("system", 1.2, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глин"))
                    {
                        //Получаем имя "Глин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глин");

                        //Проверяем наличие имени "Глин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глин"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глиндр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глиндр"))
                    {
                        //Получаем имя "Глиндр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глиндр");

                        //Проверяем наличие имени "Глиндр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глиндр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глиндр"
                                NationPersonalName nationPersonalName = new("system", 0.39, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глинн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глинн"))
                    {
                        //Получаем имя "Глинн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глинн");

                        //Проверяем наличие имени "Глинн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глинн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глинн"
                                NationPersonalName nationPersonalName = new("system", 1.77, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глоу"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глоу"))
                    {
                        //Получаем имя "Глоу"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глоу");

                        //Проверяем наличие имени "Глоу"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глоу"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глоу"
                                NationPersonalName nationPersonalName = new("system", 1.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Говэннон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Говэннон"))
                    {
                        //Получаем имя "Говэннон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Говэннон");

                        //Проверяем наличие имени "Говэннон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Говэннон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Говэннон"
                                NationPersonalName nationPersonalName = new("system", 1.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Горонви"
                    if (_repository.PersonalNames.Any(x => x.Name == "Горонви"))
                    {
                        //Получаем имя "Горонви"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Горонви");

                        //Проверяем наличие имени "Горонви"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Горонви"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Горонви"
                                NationPersonalName nationPersonalName = new("system", 0.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Грвн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Грвн"))
                    {
                        //Получаем имя "Грвн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Грвн");

                        //Проверяем наличие имени "Грвн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Грвн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Грвн"
                                NationPersonalName nationPersonalName = new("system", 0.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Грзэирн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Грзэирн"))
                    {
                        //Получаем имя "Грзэирн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Грзэирн");

                        //Проверяем наличие имени "Грзэирн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Грзэирн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Грзэирн"
                                NationPersonalName nationPersonalName = new("system", 0.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гри"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гри"))
                    {
                        //Получаем имя "Гри"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гри");

                        //Проверяем наличие имени "Гри"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гри"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гри"
                                NationPersonalName nationPersonalName = new("system", 0.71, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Григор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Григор"))
                    {
                        //Получаем имя "Григор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Григор");

                        //Проверяем наличие имени "Григор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Григор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Григор"
                                NationPersonalName nationPersonalName = new("system", 0.38, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гриффин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гриффин"))
                    {
                        //Получаем имя "Гриффин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гриффин");

                        //Проверяем наличие имени "Гриффин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гриффин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гриффин"
                                NationPersonalName nationPersonalName = new("system", 1.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гронв"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гронв"))
                    {
                        //Получаем имя "Гронв"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гронв");

                        //Проверяем наличие имени "Гронв"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гронв"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гронв"
                                NationPersonalName nationPersonalName = new("system", 0.85, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Груффадд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Груффадд"))
                    {
                        //Получаем имя "Груффадд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Груффадд");

                        //Проверяем наличие имени "Груффадд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Груффадд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Груффадд"
                                NationPersonalName nationPersonalName = new("system", 1.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Груффидд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Груффидд"))
                    {
                        //Получаем имя "Груффидд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Груффидд");

                        //Проверяем наличие имени "Груффидд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Груффидд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Груффидд"
                                NationPersonalName nationPersonalName = new("system", 0.8, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Груффуд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Груффуд"))
                    {
                        //Получаем имя "Груффуд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Груффуд");

                        //Проверяем наличие имени "Груффуд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Груффуд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Груффуд"
                                NationPersonalName nationPersonalName = new("system", 0.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гуин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гуин"))
                    {
                        //Получаем имя "Гуин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гуин");

                        //Проверяем наличие имени "Гуин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гуин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гуин"
                                NationPersonalName nationPersonalName = new("system", 1.35, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гуинет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гуинет"))
                    {
                        //Получаем имя "Гуинет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гуинет");

                        //Проверяем наличие имени "Гуинет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гуинет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гуинет"
                                NationPersonalName nationPersonalName = new("system", 0.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гуортиджирн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гуортиджирн"))
                    {
                        //Получаем имя "Гуортиджирн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гуортиджирн");

                        //Проверяем наличие имени "Гуортиджирн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гуортиджирн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гуортиджирн"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гуто"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гуто"))
                    {
                        //Получаем имя "Гуто"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гуто");

                        //Проверяем наличие имени "Гуто"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гуто"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гуто"
                                NationPersonalName nationPersonalName = new("system", 0.43, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Деинайол"
                    if (_repository.PersonalNames.Any(x => x.Name == "Деинайол"))
                    {
                        //Получаем имя "Деинайол"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Деинайол");

                        //Проверяем наличие имени "Деинайол"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Деинайол"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Деинайол"
                                NationPersonalName nationPersonalName = new("system", 0.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Делвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Делвин"))
                    {
                        //Получаем имя "Делвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Делвин");

                        //Проверяем наличие имени "Делвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Делвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Делвин"
                                NationPersonalName nationPersonalName = new("system", 0.13, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дерог"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дерог"))
                    {
                        //Получаем имя "Дерог"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дерог");

                        //Проверяем наличие имени "Дерог"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дерог"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дерог"
                                NationPersonalName nationPersonalName = new("system", 1.98, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дженкин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дженкин"))
                    {
                        //Получаем имя "Дженкин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дженкин");

                        //Проверяем наличие имени "Дженкин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дженкин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дженкин"
                                NationPersonalName nationPersonalName = new("system", 0.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Джераллт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Джераллт"))
                    {
                        //Получаем имя "Джераллт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Джераллт");

                        //Проверяем наличие имени "Джераллт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Джераллт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Джераллт"
                                NationPersonalName nationPersonalName = new("system", 1.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Джетэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Джетэн"))
                    {
                        //Получаем имя "Джетэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Джетэн");

                        //Проверяем наличие имени "Джетэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Джетэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Джетэн"
                                NationPersonalName nationPersonalName = new("system", 0.07, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дилан"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дилан"))
                    {
                        //Получаем имя "Дилан"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дилан");

                        //Проверяем наличие имени "Дилан"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дилан"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дилан"
                                NationPersonalName nationPersonalName = new("system", 1.39, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дилвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дилвин"))
                    {
                        //Получаем имя "Дилвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дилвин");

                        //Проверяем наличие имени "Дилвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дилвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дилвин"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Диллон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Диллон"))
                    {
                        //Получаем имя "Диллон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Диллон");

                        //Проверяем наличие имени "Диллон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Диллон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Диллон"
                                NationPersonalName nationPersonalName = new("system", 0.03, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дристэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дристэн"))
                    {
                        //Получаем имя "Дристэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дристэн");

                        //Проверяем наличие имени "Дристэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дристэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дристэн"
                                NationPersonalName nationPersonalName = new("system", 0.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дьюи"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дьюи"))
                    {
                        //Получаем имя "Дьюи"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дьюи");

                        //Проверяем наличие имени "Дьюи"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дьюи"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дьюи"
                                NationPersonalName nationPersonalName = new("system", 1.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дьюидд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дьюидд"))
                    {
                        //Получаем имя "Дьюидд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дьюидд");

                        //Проверяем наличие имени "Дьюидд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дьюидд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дьюидд"
                                NationPersonalName nationPersonalName = new("system", 0.48, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дэфидд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дэфидд"))
                    {
                        //Получаем имя "Дэфидд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дэфидд");

                        //Проверяем наличие имени "Дэфидд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дэфидд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дэфидд"
                                NationPersonalName nationPersonalName = new("system", 0.8, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Идвал"
                    if (_repository.PersonalNames.Any(x => x.Name == "Идвал"))
                    {
                        //Получаем имя "Идвал"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Идвал");

                        //Проверяем наличие имени "Идвал"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Идвал"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Идвал"
                                NationPersonalName nationPersonalName = new("system", 0.62, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иллтид"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иллтид"))
                    {
                        //Получаем имя "Иллтид"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иллтид");

                        //Проверяем наличие имени "Иллтид"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Иллтид"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Иллтид"
                                NationPersonalName nationPersonalName = new("system", 1.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иллтуд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иллтуд"))
                    {
                        //Получаем имя "Иллтуд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иллтуд");

                        //Проверяем наличие имени "Иллтуд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Иллтуд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Иллтуд"
                                NationPersonalName nationPersonalName = new("system", 0.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Илэр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Илэр"))
                    {
                        //Получаем имя "Илэр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Илэр");

                        //Проверяем наличие имени "Илэр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Илэр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Илэр"
                                NationPersonalName nationPersonalName = new("system", 1.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иорэт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иорэт"))
                    {
                        //Получаем имя "Иорэт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иорэт");

                        //Проверяем наличие имени "Иорэт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Иорэт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Иорэт"
                                NationPersonalName nationPersonalName = new("system", 0.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ислвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ислвин"))
                    {
                        //Получаем имя "Ислвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ислвин");

                        //Проверяем наличие имени "Ислвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ислвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ислвин"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Истин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Истин"))
                    {
                        //Получаем имя "Истин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Истин");

                        //Проверяем наличие имени "Истин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Истин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Истин"
                                NationPersonalName nationPersonalName = new("system", 0.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Итель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Итель"))
                    {
                        //Получаем имя "Итель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Итель");

                        //Проверяем наличие имени "Итель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Итель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Итель"
                                NationPersonalName nationPersonalName = new("system", 0.89, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иуон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иуон"))
                    {
                        //Получаем имя "Иуон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иуон");

                        //Проверяем наличие имени "Иуон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Иуон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Иуон"
                                NationPersonalName nationPersonalName = new("system", 1.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иуэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иуэн"))
                    {
                        //Получаем имя "Иуэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иуэн");

                        //Проверяем наличие имени "Иуэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Иуэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Иуэн"
                                NationPersonalName nationPersonalName = new("system", 1.71, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ифор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ифор"))
                    {
                        //Получаем имя "Ифор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ифор");

                        //Проверяем наличие имени "Ифор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ифор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ифор"
                                NationPersonalName nationPersonalName = new("system", 1.96, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ифэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ифэн"))
                    {
                        //Получаем имя "Ифэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ифэн");

                        //Проверяем наличие имени "Ифэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ифэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ифэн"
                                NationPersonalName nationPersonalName = new("system", 0.73, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Иэнто"
                    if (_repository.PersonalNames.Any(x => x.Name == "Иэнто"))
                    {
                        //Получаем имя "Иэнто"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Иэнто");

                        //Проверяем наличие имени "Иэнто"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Иэнто"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Иэнто"
                                NationPersonalName nationPersonalName = new("system", 1.68, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Йель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Йель"))
                    {
                        //Получаем имя "Йель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Йель");

                        //Проверяем наличие имени "Йель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Йель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Йель"
                                NationPersonalName nationPersonalName = new("system", 1.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кадел"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кадел"))
                    {
                        //Получаем имя "Кадел"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кадел");

                        //Проверяем наличие имени "Кадел"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кадел"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кадел"
                                NationPersonalName nationPersonalName = new("system", 0.33, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Калхвч"
                    if (_repository.PersonalNames.Any(x => x.Name == "Калхвч"))
                    {
                        //Получаем имя "Калхвч"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Калхвч");

                        //Проверяем наличие имени "Калхвч"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Калхвч"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Калхвч"
                                NationPersonalName nationPersonalName = new("system", 0.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Карвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Карвин"))
                    {
                        //Получаем имя "Карвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Карвин");

                        //Проверяем наличие имени "Карвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Карвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Карвин"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Киндделв"
                    if (_repository.PersonalNames.Any(x => x.Name == "Киндделв"))
                    {
                        //Получаем имя "Киндделв"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Киндделв");

                        //Проверяем наличие имени "Киндделв"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Киндделв"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Киндделв"
                                NationPersonalName nationPersonalName = new("system", 1.76, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кинриг"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кинриг"))
                    {
                        //Получаем имя "Кинриг"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кинриг");

                        //Проверяем наличие имени "Кинриг"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кинриг"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кинриг"
                                NationPersonalName nationPersonalName = new("system", 0.5, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кистениэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кистениэн"))
                    {
                        //Получаем имя "Кистениэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кистениэн");

                        //Проверяем наличие имени "Кистениэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кистениэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кистениэн"
                                NationPersonalName nationPersonalName = new("system", 1.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кледвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кледвин"))
                    {
                        //Получаем имя "Кледвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кледвин");

                        //Проверяем наличие имени "Кледвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кледвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кледвин"
                                NationPersonalName nationPersonalName = new("system", 0.39, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдваллэдер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдваллэдер"))
                    {
                        //Получаем имя "Кэдваллэдер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдваллэдер");

                        //Проверяем наличие имени "Кэдваллэдер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдваллэдер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдваллэдер"
                                NationPersonalName nationPersonalName = new("system", 0.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдвалэдер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдвалэдер"))
                    {
                        //Получаем имя "Кэдвалэдер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдвалэдер");

                        //Проверяем наличие имени "Кэдвалэдер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдвалэдер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдвалэдер"
                                NationPersonalName nationPersonalName = new("system", 0.03, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдвгон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдвгон"))
                    {
                        //Получаем имя "Кэдвгон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдвгон");

                        //Проверяем наличие имени "Кэдвгон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдвгон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдвгон"
                                NationPersonalName nationPersonalName = new("system", 0.7, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдок"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдок"))
                    {
                        //Получаем имя "Кэдок"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдок");

                        //Проверяем наличие имени "Кэдок"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдок"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдок"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдомедд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдомедд"))
                    {
                        //Получаем имя "Кэдомедд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдомедд");

                        //Проверяем наличие имени "Кэдомедд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдомедд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдомедд"
                                NationPersonalName nationPersonalName = new("system", 1.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдфэель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдфэель"))
                    {
                        //Получаем имя "Кэдфэель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдфэель");

                        //Проверяем наличие имени "Кэдфэель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдфэель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдфэель"
                                NationPersonalName nationPersonalName = new("system", 1.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэдфэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэдфэн"))
                    {
                        //Получаем имя "Кэдфэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэдфэн");

                        //Проверяем наличие имени "Кэдфэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэдфэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэдфэн"
                                NationPersonalName nationPersonalName = new("system", 1.31, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэрэдог"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэрэдог"))
                    {
                        //Получаем имя "Кэрэдог"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэрэдог");

                        //Проверяем наличие имени "Кэрэдог"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэрэдог"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэрэдог"
                                NationPersonalName nationPersonalName = new("system", 1.57, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэрэдок"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэрэдок"))
                    {
                        //Получаем имя "Кэрэдок"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэрэдок");

                        //Проверяем наличие имени "Кэрэдок"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэрэдок"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэрэдок"
                                NationPersonalName nationPersonalName = new("system", 0.58, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Леолин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Леолин"))
                    {
                        //Получаем имя "Леолин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Леолин");

                        //Проверяем наличие имени "Леолин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Леолин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Леолин"
                                NationPersonalName nationPersonalName = new("system", 0.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллеелин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллеелин"))
                    {
                        //Получаем имя "Ллеелин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллеелин");

                        //Проверяем наличие имени "Ллеелин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллеелин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллеелин"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллей"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллей"))
                    {
                        //Получаем имя "Ллей"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллей");

                        //Проверяем наличие имени "Ллей"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллей"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллей"
                                NationPersonalName nationPersonalName = new("system", 1.16, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллеу"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллеу"))
                    {
                        //Получаем имя "Ллеу"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллеу");

                        //Проверяем наличие имени "Ллеу"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллеу"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллеу"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лливелин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лливелин"))
                    {
                        //Получаем имя "Лливелин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лливелин");

                        //Проверяем наличие имени "Лливелин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Лливелин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Лливелин"
                                NationPersonalName nationPersonalName = new("system", 1.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лливеллин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лливеллин"))
                    {
                        //Получаем имя "Лливеллин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лливеллин");

                        //Проверяем наличие имени "Лливеллин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Лливеллин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Лливеллин"
                                NationPersonalName nationPersonalName = new("system", 0.62, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллир"))
                    {
                        //Получаем имя "Ллир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллир");

                        //Проверяем наличие имени "Ллир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллир"
                                NationPersonalName nationPersonalName = new("system", 1.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллойд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллойд"))
                    {
                        //Получаем имя "Ллойд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллойд");

                        //Проверяем наличие имени "Ллойд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллойд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллойд"
                                NationPersonalName nationPersonalName = new("system", 1.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лльюелин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лльюелин"))
                    {
                        //Получаем имя "Лльюелин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лльюелин");

                        //Проверяем наличие имени "Лльюелин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Лльюелин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Лльюелин"
                                NationPersonalName nationPersonalName = new("system", 0.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мадок"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мадок"))
                    {
                        //Получаем имя "Мадок"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мадок");

                        //Проверяем наличие имени "Мадок"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мадок"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мадок"
                                NationPersonalName nationPersonalName = new("system", 1.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Максен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Максен"))
                    {
                        //Получаем имя "Максен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Максен");

                        //Проверяем наличие имени "Максен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Максен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Максен"
                                NationPersonalName nationPersonalName = new("system", 1.43, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меериг"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меериг"))
                    {
                        //Получаем имя "Меериг"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меериг");

                        //Проверяем наличие имени "Меериг"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меериг"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меериг"
                                NationPersonalName nationPersonalName = new("system", 0.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меерик"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меерик"))
                    {
                        //Получаем имя "Меерик"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меерик");

                        //Проверяем наличие имени "Меерик"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меерик"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меерик"
                                NationPersonalName nationPersonalName = new("system", 0.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меикэл"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меикэл"))
                    {
                        //Получаем имя "Меикэл"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меикэл");

                        //Проверяем наличие имени "Меикэл"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меикэл"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меикэл"
                                NationPersonalName nationPersonalName = new("system", 0.77, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меирайон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меирайон"))
                    {
                        //Получаем имя "Меирайон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меирайон");

                        //Проверяем наличие имени "Меирайон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меирайон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меирайон"
                                NationPersonalName nationPersonalName = new("system", 1.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мередидд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мередидд"))
                    {
                        //Получаем имя "Мередидд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мередидд");

                        //Проверяем наличие имени "Мередидд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мередидд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мередидд"
                                NationPersonalName nationPersonalName = new("system", 1.54, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меркэр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меркэр"))
                    {
                        //Получаем имя "Меркэр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меркэр");

                        //Проверяем наличие имени "Меркэр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меркэр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меркэр"
                                NationPersonalName nationPersonalName = new("system", 1.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меррайон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меррайон"))
                    {
                        //Получаем имя "Меррайон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меррайон");

                        //Проверяем наличие имени "Меррайон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меррайон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меррайон"
                                NationPersonalName nationPersonalName = new("system", 1.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меррик"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меррик"))
                    {
                        //Получаем имя "Меррик"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меррик");

                        //Проверяем наличие имени "Меррик"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меррик"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меррик"
                                NationPersonalName nationPersonalName = new("system", 1.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мерфин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мерфин"))
                    {
                        //Получаем имя "Мерфин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мерфин");

                        //Проверяем наличие имени "Мерфин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мерфин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мерфин"
                                NationPersonalName nationPersonalName = new("system", 1.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мирддин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мирддин"))
                    {
                        //Получаем имя "Мирддин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мирддин");

                        //Проверяем наличие имени "Мирддин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мирддин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мирддин"
                                NationPersonalName nationPersonalName = new("system", 0.03, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Михэнгель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Михэнгель"))
                    {
                        //Получаем имя "Михэнгель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Михэнгель");

                        //Проверяем наличие имени "Михэнгель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Михэнгель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Михэнгель"
                                NationPersonalName nationPersonalName = new("system", 0.29, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Моезен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Моезен"))
                    {
                        //Получаем имя "Моезен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Моезен");

                        //Проверяем наличие имени "Моезен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Моезен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Моезен"
                                NationPersonalName nationPersonalName = new("system", 1.09, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Молдвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Молдвин"))
                    {
                        //Получаем имя "Молдвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Молдвин");

                        //Проверяем наличие имени "Молдвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Молдвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Молдвин"
                                NationPersonalName nationPersonalName = new("system", 1.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Морген"
                    if (_repository.PersonalNames.Any(x => x.Name == "Морген"))
                    {
                        //Получаем имя "Морген"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Морген");

                        //Проверяем наличие имени "Морген"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Морген"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Морген"
                                NationPersonalName nationPersonalName = new("system", 0.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Морт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Морт"))
                    {
                        //Получаем имя "Морт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Морт");

                        //Проверяем наличие имени "Морт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Морт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Морт"
                                NationPersonalName nationPersonalName = new("system", 0.5, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мостин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мостин"))
                    {
                        //Получаем имя "Мостин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мостин");

                        //Проверяем наличие имени "Мостин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мостин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мостин"
                                NationPersonalName nationPersonalName = new("system", 0.97, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэбон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэбон"))
                    {
                        //Получаем имя "Мэбон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэбон");

                        //Проверяем наличие имени "Мэбон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэбон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэбон"
                                NationPersonalName nationPersonalName = new("system", 1.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэдог"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэдог"))
                    {
                        //Получаем имя "Мэдог"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэдог");

                        //Проверяем наличие имени "Мэдог"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэдог"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэдог"
                                NationPersonalName nationPersonalName = new("system", 0.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэксен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэксен"))
                    {
                        //Получаем имя "Мэксен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэксен");

                        //Проверяем наличие имени "Мэксен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэксен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэксен"
                                NationPersonalName nationPersonalName = new("system", 0.98, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэредадд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэредадд"))
                    {
                        //Получаем имя "Мэредадд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэредадд");

                        //Проверяем наличие имени "Мэредадд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэредадд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэредадд"
                                NationPersonalName nationPersonalName = new("system", 0.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Надд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Надд"))
                    {
                        //Получаем имя "Надд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Надд");

                        //Проверяем наличие имени "Надд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Надд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Надд"
                                NationPersonalName nationPersonalName = new("system", 1.45, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Неирин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Неирин"))
                    {
                        //Получаем имя "Неирин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Неирин");

                        //Проверяем наличие имени "Неирин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Неирин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Неирин"
                                NationPersonalName nationPersonalName = new("system", 0.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Неифайон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Неифайон"))
                    {
                        //Получаем имя "Неифайон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Неифайон");

                        //Проверяем наличие имени "Неифайон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Неифайон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Неифайон"
                                NationPersonalName nationPersonalName = new("system", 0.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Нние"
                    if (_repository.PersonalNames.Any(x => x.Name == "Нние"))
                    {
                        //Получаем имя "Нние"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Нние");

                        //Проверяем наличие имени "Нние"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Нние"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Нние"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Овен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Овен"))
                    {
                        //Получаем имя "Овен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Овен");

                        //Проверяем наличие имени "Овен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Овен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Овен"
                                NationPersonalName nationPersonalName = new("system", 1.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Осваллт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Осваллт"))
                    {
                        //Получаем имя "Осваллт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Осваллт");

                        //Проверяем наличие имени "Осваллт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Осваллт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Осваллт"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Остин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Остин"))
                    {
                        //Получаем имя "Остин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Остин");

                        //Проверяем наличие имени "Остин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Остин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Остин"
                                NationPersonalName nationPersonalName = new("system", 1.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Оуен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Оуен"))
                    {
                        //Получаем имя "Оуен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Оуен");

                        //Проверяем наличие имени "Оуен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Оуен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Оуен"
                                NationPersonalName nationPersonalName = new("system", 1.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Оуин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Оуин"))
                    {
                        //Получаем имя "Оуин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Оуин");

                        //Проверяем наличие имени "Оуин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Оуин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Оуин"
                                NationPersonalName nationPersonalName = new("system", 0.86, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Оуинн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Оуинн"))
                    {
                        //Получаем имя "Оуинн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Оуинн");

                        //Проверяем наличие имени "Оуинн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Оуинн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Оуинн"
                                NationPersonalName nationPersonalName = new("system", 0.41, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Оуэин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Оуэин"))
                    {
                        //Получаем имя "Оуэин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Оуэин");

                        //Проверяем наличие имени "Оуэин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Оуэин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Оуэин"
                                NationPersonalName nationPersonalName = new("system", 1.67, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Оуэйн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Оуэйн"))
                    {
                        //Получаем имя "Оуэйн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Оуэйн");

                        //Проверяем наличие имени "Оуэйн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Оуэйн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Оуэйн"
                                NationPersonalName nationPersonalName = new("system", 0.77, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Офидд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Офидд"))
                    {
                        //Получаем имя "Офидд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Офидд");

                        //Проверяем наличие имени "Офидд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Офидд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Офидд"
                                NationPersonalName nationPersonalName = new("system", 1.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Парри"
                    if (_repository.PersonalNames.Any(x => x.Name == "Парри"))
                    {
                        //Получаем имя "Парри"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Парри");

                        //Проверяем наличие имени "Парри"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Парри"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Парри"
                                NationPersonalName nationPersonalName = new("system", 1.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Пвилл"
                    if (_repository.PersonalNames.Any(x => x.Name == "Пвилл"))
                    {
                        //Получаем имя "Пвилл"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Пвилл");

                        //Проверяем наличие имени "Пвилл"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Пвилл"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Пвилл"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Педр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Педр"))
                    {
                        //Получаем имя "Педр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Педр");

                        //Проверяем наличие имени "Педр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Педр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Педр"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Пенллин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Пенллин"))
                    {
                        //Получаем имя "Пенллин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Пенллин");

                        //Проверяем наличие имени "Пенллин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Пенллин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Пенллин"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Прайс"
                    if (_repository.PersonalNames.Any(x => x.Name == "Прайс"))
                    {
                        //Получаем имя "Прайс"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Прайс");

                        //Проверяем наличие имени "Прайс"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Прайс"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Прайс"
                                NationPersonalName nationPersonalName = new("system", 0.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Придери"
                    if (_repository.PersonalNames.Any(x => x.Name == "Придери"))
                    {
                        //Получаем имя "Придери"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Придери");

                        //Проверяем наличие имени "Придери"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Придери"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Придери"
                                NationPersonalName nationPersonalName = new("system", 1.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Прис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Прис"))
                    {
                        //Получаем имя "Прис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Прис");

                        //Проверяем наличие имени "Прис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Прис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Прис"
                                NationPersonalName nationPersonalName = new("system", 1.84, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Пэдеро"
                    if (_repository.PersonalNames.Any(x => x.Name == "Пэдеро"))
                    {
                        //Получаем имя "Пэдеро"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Пэдеро");

                        //Проверяем наличие имени "Пэдеро"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Пэдеро"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Пэдеро"
                                NationPersonalName nationPersonalName = new("system", 1.96, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Пэдриг"
                    if (_repository.PersonalNames.Any(x => x.Name == "Пэдриг"))
                    {
                        //Получаем имя "Пэдриг"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Пэдриг");

                        //Проверяем наличие имени "Пэдриг"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Пэдриг"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Пэдриг"
                                NationPersonalName nationPersonalName = new("system", 0.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ренфрю"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ренфрю"))
                    {
                        //Получаем имя "Ренфрю"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ренфрю");

                        //Проверяем наличие имени "Ренфрю"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ренфрю"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ренфрю"
                                NationPersonalName nationPersonalName = new("system", 1.2, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рис"))
                    {
                        //Получаем имя "Рис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рис");

                        //Проверяем наличие имени "Рис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рис"
                                NationPersonalName nationPersonalName = new("system", 0.64, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ролэнт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ролэнт"))
                    {
                        //Получаем имя "Ролэнт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ролэнт");

                        //Проверяем наличие имени "Ролэнт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ролэнт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ролэнт"
                                NationPersonalName nationPersonalName = new("system", 0.61, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхеиналлт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхеиналлт"))
                    {
                        //Получаем имя "Рхеиналлт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхеиналлт");

                        //Проверяем наличие имени "Рхеиналлт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхеиналлт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхеиналлт"
                                NationPersonalName nationPersonalName = new("system", 1.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхиддерч"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхиддерч"))
                    {
                        //Получаем имя "Рхиддерч"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхиддерч");

                        //Проверяем наличие имени "Рхиддерч"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхиддерч"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхиддерч"
                                NationPersonalName nationPersonalName = new("system", 0.26, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхизиарт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхизиарт"))
                    {
                        //Получаем имя "Рхизиарт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхизиарт");

                        //Проверяем наличие имени "Рхизиарт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхизиарт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхизиарт"
                                NationPersonalName nationPersonalName = new("system", 1.74, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхис"))
                    {
                        //Получаем имя "Рхис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхис");

                        //Проверяем наличие имени "Рхис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхис"
                                NationPersonalName nationPersonalName = new("system", 1.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхоберт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхоберт"))
                    {
                        //Получаем имя "Рхоберт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхоберт");

                        //Проверяем наличие имени "Рхоберт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхоберт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхоберт"
                                NationPersonalName nationPersonalName = new("system", 0.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рходри"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рходри"))
                    {
                        //Получаем имя "Рходри"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рходри");

                        //Проверяем наличие имени "Рходри"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рходри"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рходри"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сайор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сайор"))
                    {
                        //Получаем имя "Сайор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сайор");

                        //Проверяем наличие имени "Сайор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сайор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сайор"
                                NationPersonalName nationPersonalName = new("system", 0.27, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сайорис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сайорис"))
                    {
                        //Получаем имя "Сайорис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сайорис");

                        //Проверяем наличие имени "Сайорис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сайорис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сайорис"
                                NationPersonalName nationPersonalName = new("system", 1.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сайорс"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сайорс"))
                    {
                        //Получаем имя "Сайорс"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сайорс");

                        //Проверяем наличие имени "Сайорс"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сайорс"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сайорс"
                                NationPersonalName nationPersonalName = new("system", 1.11, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сайорус"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сайорус"))
                    {
                        //Получаем имя "Сайорус"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сайорус");

                        //Проверяем наличие имени "Сайорус"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сайорус"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сайорус"
                                NationPersonalName nationPersonalName = new("system", 1.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сеиссилт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сеиссилт"))
                    {
                        //Получаем имя "Сеиссилт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сеиссилт");

                        //Проверяем наличие имени "Сеиссилт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сеиссилт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сеиссилт"
                                NationPersonalName nationPersonalName = new("system", 1.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Селиф"
                    if (_repository.PersonalNames.Any(x => x.Name == "Селиф"))
                    {
                        //Получаем имя "Селиф"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Селиф");

                        //Проверяем наличие имени "Селиф"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Селиф"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Селиф"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиарл"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиарл"))
                    {
                        //Получаем имя "Сиарл"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиарл");

                        //Проверяем наличие имени "Сиарл"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиарл"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиарл"
                                NationPersonalName nationPersonalName = new("system", 0.63, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Силиддон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Силиддон"))
                    {
                        //Получаем имя "Силиддон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Силиддон");

                        //Проверяем наличие имени "Силиддон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Силиддон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Силиддон"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Силин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Силин"))
                    {
                        //Получаем имя "Силин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Силин");

                        //Проверяем наличие имени "Силин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Силин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Силин"
                                NationPersonalName nationPersonalName = new("system", 1.47, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Синкин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Синкин"))
                    {
                        //Получаем имя "Синкин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Синкин");

                        //Проверяем наличие имени "Синкин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Синкин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Синкин"
                                NationPersonalName nationPersonalName = new("system", 1.11, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сион"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сион"))
                    {
                        //Получаем имя "Сион"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сион");

                        //Проверяем наличие имени "Сион"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сион"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сион"
                                NationPersonalName nationPersonalName = new("system", 0.09, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сирвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сирвин"))
                    {
                        //Получаем имя "Сирвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сирвин");

                        //Проверяем наличие имени "Сирвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сирвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сирвин"
                                NationPersonalName nationPersonalName = new("system", 0.09, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сифин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сифин"))
                    {
                        //Получаем имя "Сифин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сифин");

                        //Проверяем наличие имени "Сифин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сифин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сифин"
                                NationPersonalName nationPersonalName = new("system", 0.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиффр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиффр"))
                    {
                        //Получаем имя "Сиффр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиффр");

                        //Проверяем наличие имени "Сиффр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиффр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиффр"
                                NationPersonalName nationPersonalName = new("system", 1.96, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Соил"
                    if (_repository.PersonalNames.Any(x => x.Name == "Соил"))
                    {
                        //Получаем имя "Соил"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Соил");

                        //Проверяем наличие имени "Соил"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Соил"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Соил"
                                NationPersonalName nationPersonalName = new("system", 1.57, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Стеффэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Стеффэн"))
                    {
                        //Получаем имя "Стеффэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Стеффэн");

                        //Проверяем наличие имени "Стеффэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Стеффэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Стеффэн"
                                NationPersonalName nationPersonalName = new("system", 0.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сэдрн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сэдрн"))
                    {
                        //Получаем имя "Сэдрн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сэдрн");

                        //Проверяем наличие имени "Сэдрн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сэдрн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сэдрн"
                                NationPersonalName nationPersonalName = new("system", 0.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Таффи"
                    if (_repository.PersonalNames.Any(x => x.Name == "Таффи"))
                    {
                        //Получаем имя "Таффи"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Таффи");

                        //Проверяем наличие имени "Таффи"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Таффи"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Таффи"
                                NationPersonalName nationPersonalName = new("system", 0.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тведр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тведр"))
                    {
                        //Получаем имя "Тведр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тведр");

                        //Проверяем наличие имени "Тведр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тведр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тведр"
                                NationPersonalName nationPersonalName = new("system", 0.77, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Твм"
                    if (_repository.PersonalNames.Any(x => x.Name == "Твм"))
                    {
                        //Получаем имя "Твм"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Твм");

                        //Проверяем наличие имени "Твм"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Твм"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Твм"
                                NationPersonalName nationPersonalName = new("system", 1.19, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Теирту"
                    if (_repository.PersonalNames.Any(x => x.Name == "Теирту"))
                    {
                        //Получаем имя "Теирту"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Теирту");

                        //Проверяем наличие имени "Теирту"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Теирту"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Теирту"
                                NationPersonalName nationPersonalName = new("system", 0.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Томос"
                    if (_repository.PersonalNames.Any(x => x.Name == "Томос"))
                    {
                        //Получаем имя "Томос"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Томос");

                        //Проверяем наличие имени "Томос"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Томос"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Томос"
                                NationPersonalName nationPersonalName = new("system", 0.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Трев"
                    if (_repository.PersonalNames.Any(x => x.Name == "Трев"))
                    {
                        //Получаем имя "Трев"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Трев");

                        //Проверяем наличие имени "Трев"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Трев"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Трев"
                                NationPersonalName nationPersonalName = new("system", 0.27, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тревор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тревор"))
                    {
                        //Получаем имя "Тревор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тревор");

                        //Проверяем наличие имени "Тревор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тревор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тревор"
                                NationPersonalName nationPersonalName = new("system", 1.97, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Трефор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Трефор"))
                    {
                        //Получаем имя "Трефор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Трефор");

                        //Проверяем наличие имени "Трефор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Трефор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Трефор"
                                NationPersonalName nationPersonalName = new("system", 1.15, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Трэхэерн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Трэхэерн"))
                    {
                        //Получаем имя "Трэхэерн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Трэхэерн");

                        //Проверяем наличие имени "Трэхэерн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Трэхэерн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Трэхэерн"
                                NationPersonalName nationPersonalName = new("system", 0.06, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тудер"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тудер"))
                    {
                        //Получаем имя "Тудер"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тудер");

                        //Проверяем наличие имени "Тудер"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тудер"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тудер"
                                NationPersonalName nationPersonalName = new("system", 1.84, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тудир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тудир"))
                    {
                        //Получаем имя "Тудир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тудир");

                        //Проверяем наличие имени "Тудир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тудир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тудир"
                                NationPersonalName nationPersonalName = new("system", 0.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тэлисин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тэлисин"))
                    {
                        //Получаем имя "Тэлисин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тэлисин");

                        //Проверяем наличие имени "Тэлисин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тэлисин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тэлисин"
                                NationPersonalName nationPersonalName = new("system", 1.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тэлфрин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тэлфрин"))
                    {
                        //Получаем имя "Тэлфрин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тэлфрин");

                        //Проверяем наличие имени "Тэлфрин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тэлфрин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тэлфрин"
                                NationPersonalName nationPersonalName = new("system", 1.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тюдор"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тюдор"))
                    {
                        //Получаем имя "Тюдор"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тюдор");

                        //Проверяем наличие имени "Тюдор"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тюдор"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тюдор"
                                NationPersonalName nationPersonalName = new("system", 0.73, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Уриен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Уриен"))
                    {
                        //Получаем имя "Уриен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Уриен");

                        //Проверяем наличие имени "Уриен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Уриен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Уриен"
                                NationPersonalName nationPersonalName = new("system", 1.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Фолэнт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Фолэнт"))
                    {
                        //Получаем имя "Фолэнт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Фолэнт");

                        //Проверяем наличие имени "Фолэнт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Фолэнт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Фолэнт"
                                NationPersonalName nationPersonalName = new("system", 1.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хаул"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хаул"))
                    {
                        //Получаем имя "Хаул"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хаул");

                        //Проверяем наличие имени "Хаул"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хаул"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хаул"
                                NationPersonalName nationPersonalName = new("system", 1.99, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хеддвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хеддвин"))
                    {
                        //Получаем имя "Хеддвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хеддвин");

                        //Проверяем наличие имени "Хеддвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хеддвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хеддвин"
                                NationPersonalName nationPersonalName = new("system", 0.07, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хеилин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хеилин"))
                    {
                        //Получаем имя "Хеилин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хеилин");

                        //Проверяем наличие имени "Хеилин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хеилин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хеилин"
                                NationPersonalName nationPersonalName = new("system", 0.59, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хенбеддестир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хенбеддестир"))
                    {
                        //Получаем имя "Хенбеддестир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хенбеддестир");

                        //Проверяем наличие имени "Хенбеддестир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хенбеддестир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хенбеддестир"
                                NationPersonalName nationPersonalName = new("system", 1.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хенвас"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хенвас"))
                    {
                        //Получаем имя "Хенвас"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хенвас");

                        //Проверяем наличие имени "Хенвас"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хенвас"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хенвас"
                                NationPersonalName nationPersonalName = new("system", 0.63, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хенвинеб"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хенвинеб"))
                    {
                        //Получаем имя "Хенвинеб"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хенвинеб");

                        //Проверяем наличие имени "Хенвинеб"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хенвинеб"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хенвинеб"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хеулог"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хеулог"))
                    {
                        //Получаем имя "Хеулог"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хеулог");

                        //Проверяем наличие имени "Хеулог"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хеулог"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хеулог"
                                NationPersonalName nationPersonalName = new("system", 1.02, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хефин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хефин"))
                    {
                        //Получаем имя "Хефин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хефин");

                        //Проверяем наличие имени "Хефин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хефин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хефин"
                                NationPersonalName nationPersonalName = new("system", 0.99, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хивель"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хивель"))
                    {
                        //Получаем имя "Хивель"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хивель");

                        //Проверяем наличие имени "Хивель"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хивель"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хивель"
                                NationPersonalName nationPersonalName = new("system", 1.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хопкин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хопкин"))
                    {
                        //Получаем имя "Хопкин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хопкин");

                        //Проверяем наличие имени "Хопкин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хопкин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хопкин"
                                NationPersonalName nationPersonalName = new("system", 0.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хув"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хув"))
                    {
                        //Получаем имя "Хув"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хув");

                        //Проверяем наличие имени "Хув"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хув"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хув"
                                NationPersonalName nationPersonalName = new("system", 0.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хэдин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хэдин"))
                    {
                        //Получаем имя "Хэдин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хэдин");

                        //Проверяем наличие имени "Хэдин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хэдин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хэдин"
                                NationPersonalName nationPersonalName = new("system", 1.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эериг"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эериг"))
                    {
                        //Получаем имя "Эериг"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эериг");

                        //Проверяем наличие имени "Эериг"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эериг"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эериг"
                                NationPersonalName nationPersonalName = new("system", 0.95, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эилиэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эилиэн"))
                    {
                        //Получаем имя "Эилиэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эилиэн");

                        //Проверяем наличие имени "Эилиэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эилиэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эилиэн"
                                NationPersonalName nationPersonalName = new("system", 0.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эинайон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эинайон"))
                    {
                        //Получаем имя "Эинайон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эинайон");

                        //Проверяем наличие имени "Эинайон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эинайон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эинайон"
                                NationPersonalName nationPersonalName = new("system", 1.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элидир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элидир"))
                    {
                        //Получаем имя "Элидир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элидир");

                        //Проверяем наличие имени "Элидир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элидир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элидир"
                                NationPersonalName nationPersonalName = new("system", 0.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элизуд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элизуд"))
                    {
                        //Получаем имя "Элизуд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элизуд");

                        //Проверяем наличие имени "Элизуд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элизуд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элизуд"
                                NationPersonalName nationPersonalName = new("system", 1.94, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элис"))
                    {
                        //Получаем имя "Элис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элис");

                        //Проверяем наличие имени "Элис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элис"
                                NationPersonalName nationPersonalName = new("system", 1.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элиэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элиэн"))
                    {
                        //Получаем имя "Элиэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элиэн");

                        //Проверяем наличие имени "Элиэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элиэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элиэн"
                                NationPersonalName nationPersonalName = new("system", 1.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эмир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эмир"))
                    {
                        //Получаем имя "Эмир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эмир");

                        //Проверяем наличие имени "Эмир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эмир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эмир"
                                NationPersonalName nationPersonalName = new("system", 1.09, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эмлин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эмлин"))
                    {
                        //Получаем имя "Эмлин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эмлин");

                        //Проверяем наличие имени "Эмлин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эмлин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эмлин"
                                NationPersonalName nationPersonalName = new("system", 1.96, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эмрис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эмрис"))
                    {
                        //Получаем имя "Эмрис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эмрис");

                        //Проверяем наличие имени "Эмрис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эмрис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эмрис"
                                NationPersonalName nationPersonalName = new("system", 0.95, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Энфис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Энфис"))
                    {
                        //Получаем имя "Энфис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Энфис");

                        //Проверяем наличие имени "Энфис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Энфис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Энфис"
                                NationPersonalName nationPersonalName = new("system", 0.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эрквлфф"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эрквлфф"))
                    {
                        //Получаем имя "Эрквлфф"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эрквлфф");

                        //Проверяем наличие имени "Эрквлфф"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эрквлфф"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эрквлфф"
                                NationPersonalName nationPersonalName = new("system", 1.64, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Юеин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Юеин"))
                    {
                        //Получаем имя "Юеин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Юеин");

                        //Проверяем наличие имени "Юеин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Юеин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Юеин"
                                NationPersonalName nationPersonalName = new("system", 1.68, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Адерин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Адерин"))
                    {
                        //Получаем имя "Адерин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Адерин");

                        //Проверяем наличие имени "Адерин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Адерин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Адерин"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аерона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аерона"))
                    {
                        //Получаем имя "Аерона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аерона");

                        //Проверяем наличие имени "Аерона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Аерона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Аерона"
                                NationPersonalName nationPersonalName = new("system", 0.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аеронвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аеронвен"))
                    {
                        //Получаем имя "Аеронвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аеронвен");

                        //Проверяем наличие имени "Аеронвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Аеронвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Аеронвен"
                                NationPersonalName nationPersonalName = new("system", 1.02, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Аеронви"
                    if (_repository.PersonalNames.Any(x => x.Name == "Аеронви"))
                    {
                        //Получаем имя "Аеронви"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Аеронви");

                        //Проверяем наличие имени "Аеронви"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Аеронви"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Аеронви"
                                NationPersonalName nationPersonalName = new("system", 0.43, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анвен"))
                    {
                        //Получаем имя "Анвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анвен");

                        //Проверяем наличие имени "Анвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Анвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Анвен"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Анвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Анвин"))
                    {
                        //Получаем имя "Анвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Анвин");

                        //Проверяем наличие имени "Анвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Анвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Анвин"
                                NationPersonalName nationPersonalName = new("system", 1.39, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ангэрэд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ангэрэд"))
                    {
                        //Получаем имя "Ангэрэд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ангэрэд");

                        //Проверяем наличие имени "Ангэрэд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ангэрэд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ангэрэд"
                                NationPersonalName nationPersonalName = new("system", 1.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ангэрэт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ангэрэт"))
                    {
                        //Получаем имя "Ангэрэт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ангэрэт");

                        //Проверяем наличие имени "Ангэрэт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ангэрэт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ангэрэт"
                                NationPersonalName nationPersonalName = new("system", 1.61, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арвидд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арвидд"))
                    {
                        //Получаем имя "Арвидд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арвидд");

                        //Проверяем наличие имени "Арвидд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Арвидд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Арвидд"
                                NationPersonalName nationPersonalName = new("system", 0.86, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ариэнрход"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ариэнрход"))
                    {
                        //Получаем имя "Ариэнрход"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ариэнрход");

                        //Проверяем наличие имени "Ариэнрход"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ариэнрход"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ариэнрход"
                                NationPersonalName nationPersonalName = new("system", 0.03, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Арэнрход"
                    if (_repository.PersonalNames.Any(x => x.Name == "Арэнрход"))
                    {
                        //Получаем имя "Арэнрход"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Арэнрход");

                        //Проверяем наличие имени "Арэнрход"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Арэнрход"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Арэнрход"
                                NationPersonalName nationPersonalName = new("system", 0.58, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Афэнен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Афэнен"))
                    {
                        //Получаем имя "Афэнен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Афэнен");

                        //Проверяем наличие имени "Афэнен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Афэнен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Афэнен"
                                NationPersonalName nationPersonalName = new("system", 1.53, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бетрис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бетрис"))
                    {
                        //Получаем имя "Бетрис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бетрис");

                        //Проверяем наличие имени "Бетрис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бетрис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бетрис"
                                NationPersonalName nationPersonalName = new("system", 1.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бетэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бетэн"))
                    {
                        //Получаем имя "Бетэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бетэн");

                        //Проверяем наличие имени "Бетэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бетэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бетэн"
                                NationPersonalName nationPersonalName = new("system", 1.93, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Блодвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Блодвен"))
                    {
                        //Получаем имя "Блодвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Блодвен");

                        //Проверяем наличие имени "Блодвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Блодвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Блодвен"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Блодеуведд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Блодеуведд"))
                    {
                        //Получаем имя "Блодеуведд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Блодеуведд");

                        //Проверяем наличие имени "Блодеуведд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Блодеуведд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Блодеуведд"
                                NationPersonalName nationPersonalName = new("system", 0.52, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Блодеуедд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Блодеуедд"))
                    {
                        //Получаем имя "Блодеуедд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Блодеуедд");

                        //Проверяем наличие имени "Блодеуедд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Блодеуедд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Блодеуедд"
                                NationPersonalName nationPersonalName = new("system", 1.99, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Блодеуин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Блодеуин"))
                    {
                        //Получаем имя "Блодеуин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Блодеуин");

                        //Проверяем наличие имени "Блодеуин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Блодеуин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Блодеуин"
                                NationPersonalName nationPersonalName = new("system", 1.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бриаллен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бриаллен"))
                    {
                        //Получаем имя "Бриаллен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бриаллен");

                        //Проверяем наличие имени "Бриаллен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бриаллен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бриаллен"
                                NationPersonalName nationPersonalName = new("system", 1.37, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брон"))
                    {
                        //Получаем имя "Брон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брон");

                        //Проверяем наличие имени "Брон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брон"
                                NationPersonalName nationPersonalName = new("system", 0.06, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бронвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бронвен"))
                    {
                        //Получаем имя "Бронвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бронвен");

                        //Проверяем наличие имени "Бронвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бронвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бронвен"
                                NationPersonalName nationPersonalName = new("system", 1.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Бронвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Бронвин"))
                    {
                        //Получаем имя "Бронвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Бронвин");

                        //Проверяем наличие имени "Бронвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Бронвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Бронвин"
                                NationPersonalName nationPersonalName = new("system", 0.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брэнвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брэнвен"))
                    {
                        //Получаем имя "Брэнвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брэнвен");

                        //Проверяем наличие имени "Брэнвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брэнвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брэнвен"
                                NationPersonalName nationPersonalName = new("system", 1.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брэнвенн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брэнвенн"))
                    {
                        //Получаем имя "Брэнвенн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брэнвенн");

                        //Проверяем наличие имени "Брэнвенн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брэнвенн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брэнвенн"
                                NationPersonalName nationPersonalName = new("system", 1.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брэнгвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брэнгвен"))
                    {
                        //Получаем имя "Брэнгвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брэнгвен");

                        //Проверяем наличие имени "Брэнгвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брэнгвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брэнгвен"
                                NationPersonalName nationPersonalName = new("system", 1.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Брэнгви"
                    if (_repository.PersonalNames.Any(x => x.Name == "Брэнгви"))
                    {
                        //Получаем имя "Брэнгви"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Брэнгви");

                        //Проверяем наличие имени "Брэнгви"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Брэнгви"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Брэнгви"
                                NationPersonalName nationPersonalName = new("system", 1.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Винни"
                    if (_repository.PersonalNames.Any(x => x.Name == "Винни"))
                    {
                        //Получаем имя "Винни"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Винни");

                        //Проверяем наличие имени "Винни"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Винни"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Винни"
                                NationPersonalName nationPersonalName = new("system", 1.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвар"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвар"))
                    {
                        //Получаем имя "Гвар"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвар");

                        //Проверяем наличие имени "Гвар"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвар"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвар"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвен"))
                    {
                        //Получаем имя "Гвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвен");

                        //Проверяем наличие имени "Гвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвен"
                                NationPersonalName nationPersonalName = new("system", 1.83, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвенда"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвенда"))
                    {
                        //Получаем имя "Гвенда"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвенда");

                        //Проверяем наличие имени "Гвенда"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвенда"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвенда"
                                NationPersonalName nationPersonalName = new("system", 1.39, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвендолайн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвендолайн"))
                    {
                        //Получаем имя "Гвендолайн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвендолайн");

                        //Проверяем наличие имени "Гвендолайн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвендолайн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвендолайн"
                                NationPersonalName nationPersonalName = new("system", 1.77, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвендолен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвендолен"))
                    {
                        //Получаем имя "Гвендолен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвендолен");

                        //Проверяем наличие имени "Гвендолен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвендолен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвендолен"
                                NationPersonalName nationPersonalName = new("system", 1.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвендолин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвендолин"))
                    {
                        //Получаем имя "Гвендолин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвендолин");

                        //Проверяем наличие имени "Гвендолин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвендолин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвендолин"
                                NationPersonalName nationPersonalName = new("system", 0.14, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвенет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвенет"))
                    {
                        //Получаем имя "Гвенет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвенет");

                        //Проверяем наличие имени "Гвенет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвенет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвенет"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвенит"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвенит"))
                    {
                        //Получаем имя "Гвенит"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвенит");

                        //Проверяем наличие имени "Гвенит"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвенит"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвенит"
                                NationPersonalName nationPersonalName = new("system", 1.85, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвенллиэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвенллиэн"))
                    {
                        //Получаем имя "Гвенллиэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвенллиэн");

                        //Проверяем наличие имени "Гвенллиэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвенллиэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвенллиэн"
                                NationPersonalName nationPersonalName = new("system", 0.73, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвеннет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвеннет"))
                    {
                        //Получаем имя "Гвеннет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвеннет");

                        //Проверяем наличие имени "Гвеннет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвеннет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвеннет"
                                NationPersonalName nationPersonalName = new("system", 1.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвенфрюи"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвенфрюи"))
                    {
                        //Получаем имя "Гвенфрюи"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвенфрюи");

                        //Проверяем наличие имени "Гвенфрюи"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвенфрюи"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвенфрюи"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвенхвивэр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвенхвивэр"))
                    {
                        //Получаем имя "Гвенхвивэр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвенхвивэр");

                        //Проверяем наличие имени "Гвенхвивэр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвенхвивэр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвенхвивэр"
                                NationPersonalName nationPersonalName = new("system", 1.98, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвинеира"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвинеира"))
                    {
                        //Получаем имя "Гвинеира"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвинеира");

                        //Проверяем наличие имени "Гвинеира"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвинеира"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвинеира"
                                NationPersonalName nationPersonalName = new("system", 1.04, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвинет"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвинет"))
                    {
                        //Получаем имя "Гвинет"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвинет");

                        //Проверяем наличие имени "Гвинет"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвинет"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвинет"
                                NationPersonalName nationPersonalName = new("system", 0.48, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гвлэдус"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гвлэдус"))
                    {
                        //Получаем имя "Гвлэдус"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гвлэдус");

                        //Проверяем наличие имени "Гвлэдус"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гвлэдус"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гвлэдус"
                                NationPersonalName nationPersonalName = new("system", 1.47, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Генерис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Генерис"))
                    {
                        //Получаем имя "Генерис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Генерис");

                        //Проверяем наличие имени "Генерис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Генерис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Генерис"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гленда"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гленда"))
                    {
                        //Получаем имя "Гленда"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гленда");

                        //Проверяем наличие имени "Гленда"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гленда"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гленда"
                                NationPersonalName nationPersonalName = new("system", 1.98, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глинис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глинис"))
                    {
                        //Получаем имя "Глинис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глинис");

                        //Проверяем наличие имени "Глинис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глинис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глинис"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глодуса"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глодуса"))
                    {
                        //Получаем имя "Глодуса"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глодуса");

                        //Проверяем наличие имени "Глодуса"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глодуса"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глодуса"
                                NationPersonalName nationPersonalName = new("system", 0.88, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глэдис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глэдис"))
                    {
                        //Получаем имя "Глэдис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глэдис");

                        //Проверяем наличие имени "Глэдис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глэдис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глэдис"
                                NationPersonalName nationPersonalName = new("system", 0.61, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Глэнис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Глэнис"))
                    {
                        //Получаем имя "Глэнис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Глэнис");

                        //Проверяем наличие имени "Глэнис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Глэнис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Глэнис"
                                NationPersonalName nationPersonalName = new("system", 0.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Гуендолен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Гуендолен"))
                    {
                        //Получаем имя "Гуендолен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Гуендолен");

                        //Проверяем наличие имени "Гуендолен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Гуендолен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Гуендолен"
                                NationPersonalName nationPersonalName = new("system", 0.87, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Двин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Двин"))
                    {
                        //Получаем имя "Двин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Двин");

                        //Проверяем наличие имени "Двин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Двин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Двин"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Двинвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Двинвен"))
                    {
                        //Получаем имя "Двинвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Двинвен");

                        //Проверяем наличие имени "Двинвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Двинвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Двинвен"
                                NationPersonalName nationPersonalName = new("system", 0.17, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Двинвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Двинвин"))
                    {
                        //Получаем имя "Двинвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Двинвин");

                        //Проверяем наличие имени "Двинвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Двинвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Двинвин"
                                NationPersonalName nationPersonalName = new("system", 1.8, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Делит"
                    if (_repository.PersonalNames.Any(x => x.Name == "Делит"))
                    {
                        //Получаем имя "Делит"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Делит");

                        //Проверяем наличие имени "Делит"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Делит"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Делит"
                                NationPersonalName nationPersonalName = new("system", 0.05, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Диилис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Диилис"))
                    {
                        //Получаем имя "Диилис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Диилис");

                        //Проверяем наличие имени "Диилис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Диилис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Диилис"
                                NationPersonalName nationPersonalName = new("system", 0.6, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дилвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дилвен"))
                    {
                        //Получаем имя "Дилвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дилвен");

                        //Проверяем наличие имени "Дилвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дилвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дилвен"
                                NationPersonalName nationPersonalName = new("system", 1.12, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Дилис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Дилис"))
                    {
                        //Получаем имя "Дилис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Дилис");

                        //Проверяем наличие имени "Дилис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Дилис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Дилис"
                                NationPersonalName nationPersonalName = new("system", 0.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Игрэйн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Игрэйн"))
                    {
                        //Получаем имя "Игрэйн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Игрэйн");

                        //Проверяем наличие имени "Игрэйн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Игрэйн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Игрэйн"
                                NationPersonalName nationPersonalName = new("system", 1.81, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Исбэйл"
                    if (_repository.PersonalNames.Any(x => x.Name == "Исбэйл"))
                    {
                        //Получаем имя "Исбэйл"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Исбэйл");

                        //Проверяем наличие имени "Исбэйл"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Исбэйл"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Исбэйл"
                                NationPersonalName nationPersonalName = new("system", 0.61, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кади"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кади"))
                    {
                        //Получаем имя "Кади"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кади");

                        //Проверяем наличие имени "Кади"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кади"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кади"
                                NationPersonalName nationPersonalName = new("system", 0.48, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Креиддилэд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Креиддилэд"))
                    {
                        //Получаем имя "Креиддилэд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Креиддилэд");

                        //Проверяем наличие имени "Креиддилэд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Креиддилэд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Креиддилэд"
                                NationPersonalName nationPersonalName = new("system", 0.21, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэрис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэрис"))
                    {
                        //Получаем имя "Кэрис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэрис");

                        //Проверяем наличие имени "Кэрис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэрис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэрис"
                                NationPersonalName nationPersonalName = new("system", 0.66, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Кэрон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Кэрон"))
                    {
                        //Получаем имя "Кэрон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Кэрон");

                        //Проверяем наличие имени "Кэрон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Кэрон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Кэрон"
                                NationPersonalName nationPersonalName = new("system", 0.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллеулу"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллеулу"))
                    {
                        //Получаем имя "Ллеулу"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллеулу");

                        //Проверяем наличие имени "Ллеулу"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллеулу"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллеулу"
                                NationPersonalName nationPersonalName = new("system", 1.95, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ллинос"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ллинос"))
                    {
                        //Получаем имя "Ллинос"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ллинос");

                        //Проверяем наличие имени "Ллинос"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ллинос"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ллинос"
                                NationPersonalName nationPersonalName = new("system", 1.68, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Лунед"
                    if (_repository.PersonalNames.Any(x => x.Name == "Лунед"))
                    {
                        //Получаем имя "Лунед"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Лунед");

                        //Проверяем наличие имени "Лунед"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Лунед"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Лунед"
                                NationPersonalName nationPersonalName = new("system", 1.6, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Маллт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Маллт"))
                    {
                        //Получаем имя "Маллт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Маллт");

                        //Проверяем наличие имени "Маллт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Маллт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Маллт"
                                NationPersonalName nationPersonalName = new("system", 0.65, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Марджед"
                    if (_repository.PersonalNames.Any(x => x.Name == "Марджед"))
                    {
                        //Получаем имя "Марджед"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Марджед");

                        //Проверяем наличие имени "Марджед"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Марджед"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Марджед"
                                NationPersonalName nationPersonalName = new("system", 0.04, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Маред"
                    if (_repository.PersonalNames.Any(x => x.Name == "Маред"))
                    {
                        //Получаем имя "Маред"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Маред");

                        //Проверяем наличие имени "Маред"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Маред"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Маред"
                                NationPersonalName nationPersonalName = new("system", 1.23, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меган"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меган"))
                    {
                        //Получаем имя "Меган"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меган");

                        //Проверяем наличие имени "Меган"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меган"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меган"
                                NationPersonalName nationPersonalName = new("system", 0.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меинвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меинвен"))
                    {
                        //Получаем имя "Меинвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меинвен");

                        //Проверяем наличие имени "Меинвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меинвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меинвен"
                                NationPersonalName nationPersonalName = new("system", 0.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меинир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меинир"))
                    {
                        //Получаем имя "Меинир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меинир");

                        //Проверяем наличие имени "Меинир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меинир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меинир"
                                NationPersonalName nationPersonalName = new("system", 1.82, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Меирайона"
                    if (_repository.PersonalNames.Any(x => x.Name == "Меирайона"))
                    {
                        //Получаем имя "Меирайона"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Меирайона");

                        //Проверяем наличие имени "Меирайона"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Меирайона"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Меирайона"
                                NationPersonalName nationPersonalName = new("system", 1.94, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мерерид"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мерерид"))
                    {
                        //Получаем имя "Мерерид"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мерерид");

                        //Проверяем наличие имени "Мерерид"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мерерид"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мерерид"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Миф"
                    if (_repository.PersonalNames.Any(x => x.Name == "Миф"))
                    {
                        //Получаем имя "Миф"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Миф");

                        //Проверяем наличие имени "Миф"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Миф"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Миф"
                                NationPersonalName nationPersonalName = new("system", 0.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мифэнви"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мифэнви"))
                    {
                        //Получаем имя "Мифэнви"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мифэнви");

                        //Проверяем наличие имени "Мифэнви"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мифэнви"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мифэнви"
                                NationPersonalName nationPersonalName = new("system", 0.62, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Модлен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Модлен"))
                    {
                        //Получаем имя "Модлен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Модлен");

                        //Проверяем наличие имени "Модлен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Модлен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Модлен"
                                NationPersonalName nationPersonalName = new("system", 1.96, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Морвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Морвен"))
                    {
                        //Получаем имя "Морвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Морвен");

                        //Проверяем наличие имени "Морвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Морвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Морвен"
                                NationPersonalName nationPersonalName = new("system", 1.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Морвенна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Морвенна"))
                    {
                        //Получаем имя "Морвенна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Морвенна");

                        //Проверяем наличие имени "Морвенна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Морвенна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Морвенна"
                                NationPersonalName nationPersonalName = new("system", 0.32, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэбли"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэбли"))
                    {
                        //Получаем имя "Мэбли"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэбли");

                        //Проверяем наличие имени "Мэбли"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэбли"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэбли"
                                NationPersonalName nationPersonalName = new("system", 1.26, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэйр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэйр"))
                    {
                        //Получаем имя "Мэйр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэйр");

                        //Проверяем наличие имени "Мэйр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэйр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэйр"
                                NationPersonalName nationPersonalName = new("system", 0.89, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Мэрвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Мэрвен"))
                    {
                        //Получаем имя "Мэрвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Мэрвен");

                        //Проверяем наличие имени "Мэрвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Мэрвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Мэрвен"
                                NationPersonalName nationPersonalName = new("system", 0.14, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Нерис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Нерис"))
                    {
                        //Получаем имя "Нерис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Нерис");

                        //Проверяем наличие имени "Нерис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Нерис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Нерис"
                                NationPersonalName nationPersonalName = new("system", 0.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Нест"
                    if (_repository.PersonalNames.Any(x => x.Name == "Нест"))
                    {
                        //Получаем имя "Нест"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Нест");

                        //Проверяем наличие имени "Нест"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Нест"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Нест"
                                NationPersonalName nationPersonalName = new("system", 0.57, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Неста"
                    if (_repository.PersonalNames.Any(x => x.Name == "Неста"))
                    {
                        //Получаем имя "Неста"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Неста");

                        //Проверяем наличие имени "Неста"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Неста"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Неста"
                                NationPersonalName nationPersonalName = new("system", 0.91, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ниму"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ниму"))
                    {
                        //Получаем имя "Ниму"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ниму");

                        //Проверяем наличие имени "Ниму"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ниму"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ниму"
                                NationPersonalName nationPersonalName = new("system", 0.75, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ния"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ния"))
                    {
                        //Получаем имя "Ния"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ния");

                        //Проверяем наличие имени "Ния"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ния"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ния"
                                NationPersonalName nationPersonalName = new("system", 1.23, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Олвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Олвен"))
                    {
                        //Получаем имя "Олвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Олвен");

                        //Проверяем наличие имени "Олвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Олвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Олвен"
                                NationPersonalName nationPersonalName = new("system", 0.3, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Олвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Олвин"))
                    {
                        //Получаем имя "Олвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Олвин");

                        //Проверяем наличие имени "Олвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Олвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Олвин"
                                NationPersonalName nationPersonalName = new("system", 1.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Олвинн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Олвинн"))
                    {
                        //Получаем имя "Олвинн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Олвинн");

                        //Проверяем наличие имени "Олвинн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Олвинн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Олвинн"
                                NationPersonalName nationPersonalName = new("system", 0.34, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Риэннон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Риэннон"))
                    {
                        //Получаем имя "Риэннон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Риэннон");

                        //Проверяем наличие имени "Риэннон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Риэннон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Риэннон"
                                NationPersonalName nationPersonalName = new("system", 1.72, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхиэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхиэн"))
                    {
                        //Получаем имя "Рхиэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхиэн");

                        //Проверяем наличие имени "Рхиэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхиэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхиэн"
                                NationPersonalName nationPersonalName = new("system", 1.93, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхиэнвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхиэнвен"))
                    {
                        //Получаем имя "Рхиэнвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхиэнвен");

                        //Проверяем наличие имени "Рхиэнвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхиэнвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхиэнвен"
                                NationPersonalName nationPersonalName = new("system", 1.39, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхиэннон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхиэннон"))
                    {
                        //Получаем имя "Рхиэннон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхиэннон");

                        //Проверяем наличие имени "Рхиэннон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхиэннон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхиэннон"
                                NationPersonalName nationPersonalName = new("system", 0.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхиэнон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхиэнон"))
                    {
                        //Получаем имя "Рхиэнон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхиэнон");

                        //Проверяем наличие имени "Рхиэнон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхиэнон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхиэнон"
                                NationPersonalName nationPersonalName = new("system", 0.5, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхиэну"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхиэну"))
                    {
                        //Получаем имя "Рхиэну"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхиэну");

                        //Проверяем наличие имени "Рхиэну"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхиэну"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхиэну"
                                NationPersonalName nationPersonalName = new("system", 0.13, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхозин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхозин"))
                    {
                        //Получаем имя "Рхозин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхозин");

                        //Проверяем наличие имени "Рхозин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхозин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхозин"
                                NationPersonalName nationPersonalName = new("system", 1.64, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхонвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхонвен"))
                    {
                        //Получаем имя "Рхонвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхонвен");

                        //Проверяем наличие имени "Рхонвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхонвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхонвен"
                                NationPersonalName nationPersonalName = new("system", 1.79, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Рхэмэнтус"
                    if (_repository.PersonalNames.Any(x => x.Name == "Рхэмэнтус"))
                    {
                        //Получаем имя "Рхэмэнтус"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Рхэмэнтус");

                        //Проверяем наличие имени "Рхэмэнтус"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Рхэмэнтус"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Рхэмэнтус"
                                NationPersonalName nationPersonalName = new("system", 1.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сайонед"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сайонед"))
                    {
                        //Получаем имя "Сайонед"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сайонед");

                        //Проверяем наличие имени "Сайонед"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сайонед"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сайонед"
                                NationPersonalName nationPersonalName = new("system", 0.41, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Серен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Серен"))
                    {
                        //Получаем имя "Серен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Серен");

                        //Проверяем наличие имени "Серен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Серен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Серен"
                                NationPersonalName nationPersonalName = new("system", 0.1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиань"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиань"))
                    {
                        //Получаем имя "Сиань"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиань");

                        //Проверяем наличие имени "Сиань"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиань"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиань"
                                NationPersonalName nationPersonalName = new("system", 0.02, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиинвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиинвен"))
                    {
                        //Получаем имя "Сиинвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиинвен");

                        //Проверяем наличие имени "Сиинвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиинвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиинвен"
                                NationPersonalName nationPersonalName = new("system", 1.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиридвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиридвен"))
                    {
                        //Получаем имя "Сиридвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиридвен");

                        //Проверяем наличие имени "Сиридвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиридвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиридвен"
                                NationPersonalName nationPersonalName = new("system", 1.63, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сирис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сирис"))
                    {
                        //Получаем имя "Сирис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сирис");

                        //Проверяем наличие имени "Сирис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сирис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сирис"
                                NationPersonalName nationPersonalName = new("system", 0.85, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сирридвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сирридвин"))
                    {
                        //Получаем имя "Сирридвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сирридвин");

                        //Проверяем наличие имени "Сирридвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сирридвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сирридвин"
                                NationPersonalName nationPersonalName = new("system", 1.06, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиуон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиуон"))
                    {
                        //Получаем имя "Сиуон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиуон");

                        //Проверяем наличие имени "Сиуон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиуон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиуон"
                                NationPersonalName nationPersonalName = new("system", 1.51, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиэна"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиэна"))
                    {
                        //Получаем имя "Сиэна"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиэна");

                        //Проверяем наличие имени "Сиэна"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиэна"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиэна"
                                NationPersonalName nationPersonalName = new("system", 1.18, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Сиэни"
                    if (_repository.PersonalNames.Any(x => x.Name == "Сиэни"))
                    {
                        //Получаем имя "Сиэни"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Сиэни");

                        //Проверяем наличие имени "Сиэни"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Сиэни"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Сиэни"
                                NationPersonalName nationPersonalName = new("system", 1.62, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Таррен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Таррен"))
                    {
                        //Получаем имя "Таррен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Таррен");

                        //Проверяем наличие имени "Таррен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Таррен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Таррен"
                                NationPersonalName nationPersonalName = new("system", 1.56, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тегвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тегвен"))
                    {
                        //Получаем имя "Тегвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тегвен");

                        //Проверяем наличие имени "Тегвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тегвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тегвен"
                                NationPersonalName nationPersonalName = new("system", 0.14, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тегэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тегэн"))
                    {
                        //Получаем имя "Тегэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тегэн");

                        //Проверяем наличие имени "Тегэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тегэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тегэн"
                                NationPersonalName nationPersonalName = new("system", 1.74, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Террвин"
                    if (_repository.PersonalNames.Any(x => x.Name == "Террвин"))
                    {
                        //Получаем имя "Террвин"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Террвин");

                        //Проверяем наличие имени "Террвин"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Террвин"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Террвин"
                                NationPersonalName nationPersonalName = new("system", 0.85, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тивлип"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тивлип"))
                    {
                        //Получаем имя "Тивлип"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тивлип");

                        //Проверяем наличие имени "Тивлип"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тивлип"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тивлип"
                                NationPersonalName nationPersonalName = new("system", 1.04, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Тэлэйт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Тэлэйт"))
                    {
                        //Получаем имя "Тэлэйт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Тэлэйт");

                        //Проверяем наличие имени "Тэлэйт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Тэлэйт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Тэлэйт"
                                NationPersonalName nationPersonalName = new("system", 1.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ффайон"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ффайон"))
                    {
                        //Получаем имя "Ффайон"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ффайон");

                        //Проверяем наличие имени "Ффайон"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ффайон"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ффайон"
                                NationPersonalName nationPersonalName = new("system", 1.78, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Ффрэйд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Ффрэйд"))
                    {
                        //Получаем имя "Ффрэйд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Ффрэйд");

                        //Проверяем наличие имени "Ффрэйд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Ффрэйд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Ффрэйд"
                                NationPersonalName nationPersonalName = new("system", 0.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хеледд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хеледд"))
                    {
                        //Получаем имя "Хеледд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хеледд");

                        //Проверяем наличие имени "Хеледд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хеледд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хеледд"
                                NationPersonalName nationPersonalName = new("system", 0.44, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хеулвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хеулвен"))
                    {
                        //Получаем имя "Хеулвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хеулвен");

                        //Проверяем наличие имени "Хеулвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хеулвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хеулвен"
                                NationPersonalName nationPersonalName = new("system", 0.01, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хефина"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хефина"))
                    {
                        //Получаем имя "Хефина"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хефина");

                        //Проверяем наличие имени "Хефина"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хефина"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хефина"
                                NationPersonalName nationPersonalName = new("system", 1.36, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хиледд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хиледд"))
                    {
                        //Получаем имя "Хиледд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хиледд");

                        //Проверяем наличие имени "Хиледд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хиледд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хиледд"
                                NationPersonalName nationPersonalName = new("system", 1.22, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хэбрен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хэбрен"))
                    {
                        //Получаем имя "Хэбрен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хэбрен");

                        //Проверяем наличие имени "Хэбрен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хэбрен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хэбрен"
                                NationPersonalName nationPersonalName = new("system", 1.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хэф"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хэф"))
                    {
                        //Получаем имя "Хэф"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хэф");

                        //Проверяем наличие имени "Хэф"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хэф"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хэф"
                                NationPersonalName nationPersonalName = new("system", 1.07, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Хэфрен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Хэфрен"))
                    {
                        //Получаем имя "Хэфрен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Хэфрен");

                        //Проверяем наличие имени "Хэфрен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Хэфрен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Хэфрен"
                                NationPersonalName nationPersonalName = new("system", 1.99, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эервен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эервен"))
                    {
                        //Получаем имя "Эервен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эервен");

                        //Проверяем наличие имени "Эервен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эервен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эервен"
                                NationPersonalName nationPersonalName = new("system", 1.23, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эигир"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эигир"))
                    {
                        //Получаем имя "Эигир"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эигир");

                        //Проверяем наличие имени "Эигир"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эигир"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эигир"
                                NationPersonalName nationPersonalName = new("system", 1.23, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эигр"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эигр"))
                    {
                        //Получаем имя "Эигр"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эигр");

                        //Проверяем наличие имени "Эигр"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эигр"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эигр"
                                NationPersonalName nationPersonalName = new("system", 1.82, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эилвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эилвен"))
                    {
                        //Получаем имя "Эилвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эилвен");

                        //Проверяем наличие имени "Эилвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эилвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эилвен"
                                NationPersonalName nationPersonalName = new("system", 0.9, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эилунед"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эилунед"))
                    {
                        //Получаем имя "Эилунед"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эилунед");

                        //Проверяем наличие имени "Эилунед"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эилунед"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эилунед"
                                NationPersonalName nationPersonalName = new("system", 0.25, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эира"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эира"))
                    {
                        //Получаем имя "Эира"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эира");

                        //Проверяем наличие имени "Эира"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эира"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эира"
                                NationPersonalName nationPersonalName = new("system", 1, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эирвен"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эирвен"))
                    {
                        //Получаем имя "Эирвен"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эирвен");

                        //Проверяем наличие имени "Эирвен"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эирвен"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эирвен"
                                NationPersonalName nationPersonalName = new("system", 1.55, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эириэн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эириэн"))
                    {
                        //Получаем имя "Эириэн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эириэн");

                        //Проверяем наличие имени "Эириэн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эириэн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эириэн"
                                NationPersonalName nationPersonalName = new("system", 1.28, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эирлис"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эирлис"))
                    {
                        //Получаем имя "Эирлис"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эирлис");

                        //Проверяем наличие имени "Эирлис"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эирлис"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эирлис"
                                NationPersonalName nationPersonalName = new("system", 0.49, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элери"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элери"))
                    {
                        //Получаем имя "Элери"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элери");

                        //Проверяем наличие имени "Элери"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элери"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элери"
                                NationPersonalName nationPersonalName = new("system", 1.92, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элунед"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элунед"))
                    {
                        //Получаем имя "Элунед"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элунед");

                        //Проверяем наличие имени "Элунед"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элунед"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элунед"
                                NationPersonalName nationPersonalName = new("system", 1.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Элэйн"
                    if (_repository.PersonalNames.Any(x => x.Name == "Элэйн"))
                    {
                        //Получаем имя "Элэйн"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Элэйн");

                        //Проверяем наличие имени "Элэйн"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Элэйн"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Элэйн"
                                NationPersonalName nationPersonalName = new("system", 0.16, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Энайд"
                    if (_repository.PersonalNames.Any(x => x.Name == "Энайд"))
                    {
                        //Получаем имя "Энайд"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Энайд");

                        //Проверяем наличие имени "Энайд"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Энайд"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Энайд"
                                NationPersonalName nationPersonalName = new("system", 1.24, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Энид"
                    if (_repository.PersonalNames.Any(x => x.Name == "Энид"))
                    {
                        //Получаем имя "Энид"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Энид");

                        //Проверяем наличие имени "Энид"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Энид"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Энид"
                                NationPersonalName nationPersonalName = new("system", 0.27, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Энит"
                    if (_repository.PersonalNames.Any(x => x.Name == "Энит"))
                    {
                        //Получаем имя "Энит"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Энит");

                        //Проверяем наличие имени "Энит"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Энит"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Энит"
                                NationPersonalName nationPersonalName = new("system", 1.42, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эсиллт"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эсиллт"))
                    {
                        //Получаем имя "Эсиллт"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эсиллт");

                        //Проверяем наличие имени "Эсиллт"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эсиллт"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эсиллт"
                                NationPersonalName nationPersonalName = new("system", 0.37, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }

                    //Проверяем наличие имени "Эфа"
                    if (_repository.PersonalNames.Any(x => x.Name == "Эфа"))
                    {
                        //Получаем имя "Эфа"
                        PersonalName? personalName = _repository.PersonalNames.FirstOrDefault(x => x.Name == "Эфа");

                        //Проверяем наличие имени "Эфа"
                        if (personalName != null)
                        {
                            //Проверяем наличие связи нации "Восточный вампир" с именем "Эфа"
                            if (!_repository.NationsPersonalNames.Any(x => x.Nation == nation && x.PersonalName == personalName))
                            {
                                //Создаём связь нации "Восточный вампир" с именем "Эфа"
                                NationPersonalName nationPersonalName = new("system", 1.4, nation, personalName);
                                _repository.NationsPersonalNames.Add(nationPersonalName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeNations. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации связи фамилий с нациями
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeNationsLastNames()
    {
        try
        {
            //Проверяем наличие нации "Альв"
            if (_repository.Nations.Any(x => x.Name == "Альв"))
            {
                //Получаем нацию "Альв"
                Nation? nation = _repository.Nations.FirstOrDefault(x => x.Name == "Альв");

                //Проверяем наличие нации "Альв"
                if (nation != null)
                {
                    //Проверяем наличие фамилии "Миркраниис"
                    if (_repository.LastNames.Any(x => x.Name == "Миркраниис"))
                    {
                        //Получаем фамилию "Миркраниис"
                        LastName? lastName = _repository.LastNames.FirstOrDefault(x => x.Name == "Миркраниис");

                        //Проверяем наличие фамилии "Миркраниис"
                        if (lastName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с фамилией "Миркраниис"
                            if (!_repository.NationsLastNames.Any(x => x.Nation == nation && x.LastName == lastName))
                            {
                                //Создаём связь нации "Альв" с фамилией "Миркраниис"
                                NationLastName nationLastName = new("system", 0.17, nation, lastName);
                                _repository.NationsLastNames.Add(nationLastName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeNationsLastNames. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации связи префиксов с нациями
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeNationsPrefixNames()
    {
        try
        {
            //Проверяем наличие нации "Альв"
            if (_repository.Nations.Any(x => x.Name == "Альв"))
            {
                //Получаем нацию "Альв"
                Nation? nation = _repository.Nations.FirstOrDefault(x => x.Name == "Альв");

                //Проверяем наличие нации "Альв"
                if (nation != null)
                {
                    //Проверяем наличие префикса "из дома"
                    if (_repository.PrefixNames.Any(x => x.Name == "из дома"))
                    {
                        //Получаем префикс "из дома"
                        PrefixName? prefixName = _repository.PrefixNames.FirstOrDefault(x => x.Name == "из дома");

                        //Проверяем наличие префикса "из дома"
                        if (prefixName != null)
                        {
                            //Проверяем наличие связи нации "Альв" с префиксом "из дома"
                            if (!_repository.NationsPrefixNames.Any(x => x.Nation == nation && x.PrefixName == prefixName))
                            {
                                //Создаём связь нации "Альв" с префиксом "из дома"
                                NationPrefixName nationsPrefixName = new("system", 0.17, nation, prefixName);
                                _repository.NationsPrefixNames.Add(nationsPrefixName);
                                await _repository.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeNationsPrefixNames. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации стран
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeCountries()
    {
        try
        {
            //Проверяем наличие страны "Альвраатская империя"
            if (!_repository.Countries.Any(x => x.Name == "Альвраатская империя"))
            {
                //Создаём страну "Альвраатская империя"
                Country country = new("system", "Альвраатская империя", 1, "#20D1DB", "Исландский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Княжество Саорса"
            if (!_repository.Countries.Any(x => x.Name == "Княжество Саорса"))
            {
                //Создаём страну "Княжество Саорса"
                Country country = new("system", "Княжество Саорса", 2, "#808080", "Шведский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Королевство Берген"
            if (!_repository.Countries.Any(x => x.Name == "Королевство Берген"))
            {
                //Создаём страну "Королевство Берген"
                Country country = new("system", "Королевство Берген", 3, "#00687C", "Финский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Фесгарское княжество"
            if (!_repository.Countries.Any(x => x.Name == "Фесгарское княжество"))
            {
                //Создаём страну "Фесгарское княжество"
                Country country = new("system", "Фесгарское княжество", 4, "#B200FF", "Шотландский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Сверденский каганат"
            if (!_repository.Countries.Any(x => x.Name == "Сверденский каганат"))
            {
                //Создаём страну "Сверденский каганат"
                Country country = new("system", "Сверденский каганат", 5, "#7F3B00", "Немецкий");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Ханство Тавалин"
            if (!_repository.Countries.Any(x => x.Name == "Ханство Тавалин"))
            {
                //Создаём страну "Ханство Тавалин"
                Country country = new("system", "Ханство Тавалин", 6, "#7F006D", "Венгерский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Княжество Саргиб"
            if (!_repository.Countries.Any(x => x.Name == "Княжество Саргиб"))
            {
                //Создаём страну "Княжество Саргиб"
                Country country = new("system", "Княжество Саргиб", 7, "#007F0E", "Австрийский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Царство Банду"
            if (!_repository.Countries.Any(x => x.Name == "Царство Банду"))
            {
                //Создаём страну "Царство Банду"
                Country country = new("system", "Царство Банду", 8, "#47617C", "Индийский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Королевство Нордер"
            if (!_repository.Countries.Any(x => x.Name == "Королевство Нордер"))
            {
                //Создаём страну "Королевство Нордер"
                Country country = new("system", "Королевство Нордер", 9, "#D82929", "Датский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Альтерское княжество"
            if (!_repository.Countries.Any(x => x.Name == "Альтерское княжество"))
            {
                //Создаём страну "Альтерское княжество"
                Country country = new("system", "Альтерское княжество", 10, "#4ACC39", "Французский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Орлиадарская конведерация"
            if (!_repository.Countries.Any(x => x.Name == "Орлиадарская конведерация"))
            {
                //Создаём страну "Орлиадарская конведерация"
                Country country = new("system", "Орлиадарская конведерация", 11, "#AF9200", "Французский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Королевство Удстир"
            if (!_repository.Countries.Any(x => x.Name == "Королевство Удстир"))
            {
                //Создаём страну "Королевство Удстир"
                Country country = new("system", "Королевство Удстир", 12, "#8CAF00", "Испанский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Королевство Вервирунг"
            if (!_repository.Countries.Any(x => x.Name == "Королевство Вервирунг"))
            {
                //Создаём страну "Королевство Вервирунг"
                Country country = new("system", "Королевство Вервирунг", 13, "#7F1700", "Португальский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Дестинский орден"
            if (!_repository.Countries.Any(x => x.Name == "Дестинский орден"))
            {
                //Создаём страну "Дестинский орден"
                Country country = new("system", "Дестинский орден", 14, "#2B7C55", "Испанский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Вольный город Лийсет"
            if (!_repository.Countries.Any(x => x.Name == "Вольный город Лийсет"))
            {
                //Создаём страну "Вольный город Лийсет"
                Country country = new("system", "Вольный город Лийсет", 15, "#7B7F00", "Испанский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Лисцийская империя"
            if (!_repository.Countries.Any(x => x.Name == "Лисцийская империя"))
            {
                //Создаём страну "Лисцийская империя"
                Country country = new("system", "Лисцийская империя", 16, "#7F002E", "Испанский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Королевство Вальтир"
            if (!_repository.Countries.Any(x => x.Name == "Королевство Вальтир"))
            {
                //Создаём страну "Королевство Вальтир"
                Country country = new("system", "Королевство Вальтир", 17, "#B05BFF", "Швейарский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Вассальное княжество Гратис"
            if (!_repository.Countries.Any(x => x.Name == "Вассальное княжество Гратис"))
            {
                //Создаём страну "Вассальное княжество Гратис"
                Country country = new("system", "Вассальное княжество Гратис", 18, "#005DFF", "Испанский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Княжество Ректа"
            if (!_repository.Countries.Any(x => x.Name == "Княжество Ректа"))
            {
                //Создаём страну "Княжество Ректа"
                Country country = new("system", "Княжество Ректа", 19, "#487F00", "Македонский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Волар"
            if (!_repository.Countries.Any(x => x.Name == "Волар"))
            {
                //Создаём страну "Волар"
                Country country = new("system", "Волар", 20, "#32217A", "Греческий");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Союз Иль-Ладро"
            if (!_repository.Countries.Any(x => x.Name == "Союз Иль-Ладро"))
            {
                //Создаём страну "Союз Иль-Ладро"
                Country country = new("system", "Союз Иль-Ладро", 21, "#35513B", "Итальянский");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие страны "Уния Ангуи"
            if (!_repository.Countries.Any(x => x.Name == "Уния Ангуи"))
            {
                //Создаём страну "Уния Ангуи"
                Country country = new("system", "Уния Ангуи", 22, "#BC3CB4", "Латынь");
                _repository.Countries.Add(country);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeCountries. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации регионов
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeRegions()
    {
        try
        {
            //Проверяем наличие региона "Восточный Зимний архипелаг"
            if (!_repository.Regions.Any(x => x.Name == "Восточный Зимний архипелаг"))
            {
                //Добавляем регион
                Region region = new("system", "Восточный Зимний архипелаг", 1, "#0004FF");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Южный Зимний архипелаг"
            if (!_repository.Regions.Any(x => x.Name == "Южный Зимний архипелаг"))
            {
                //Добавляем регион
                Region region = new("system", "Южный  Зимний архипелаг", 2, "#00FFFF");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Западный Зимний архипелаг"
            if (!_repository.Regions.Any(x => x.Name == "Западный Зимний архипелаг"))
            {
                //Добавляем регион
                Region region = new("system", "Западный Зимний архипелаг", 3, "#26FF00");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Северный Зимний архипелаг"
            if (!_repository.Regions.Any(x => x.Name == "Северный Зимний архипелаг"))
            {
                //Добавляем регион
                Region region = new("system", "Северный Зимний архипелаг", 4, "#FF0AB9");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Дамхан"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Дамхан"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Дамхан", 1, "#8C0275");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Анлион"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Анлион"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Анлион", 2, "#100089");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Маиран"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Маиран"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Маиран", 3, "#068700");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Алаид"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Алаид"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Алаид", 4, "#005684");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Сеолт"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Сеолт"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Сеолт", 5, "#658200");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Гхоул"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Гхоул"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Гхоул", 6, "#007F37");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Фуил"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Фуил"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Фуил", 7, "#7C002F");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Ятаг"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Ятаг"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Ятаг", 8, "#7A2400");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Земли клана Сеар"
            if (!_repository.Regions.Any(x => x.Name == "Земли клана Сеар"))
            {
                //Добавляем регион
                Region region = new("system", "Земли клана Сеар", 9, "#BC0000");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Западный зубец"
            if (!_repository.Regions.Any(x => x.Name == "Западный зубец"))
            {
                //Добавляем регион
                Region region = new("system", "Западный зубец", 1, "#F2DE00");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Светлый берег"
            if (!_repository.Regions.Any(x => x.Name == "Светлый берег"))
            {
                //Добавляем регион
                Region region = new("system", "Светлый берег", 2, "#28E5EF");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие региона "Центральный зубец"
            if (!_repository.Regions.Any(x => x.Name == "Центральный зубец"))
            {
                //Добавляем регион
                Region region = new("system", "Центральный зубец", 3, "#6568ED");
                _repository.Regions.Add(region);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeRegions. Ошибка: {0}", ex);
        }
    }

    /// <summary>
    /// Метод инициализации типов населённых пунктов
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool> InitializeTypesSettlements()
    {
        try
        {
            //Проверяем наличие типа "Укрепление"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Укрепление"))
            {
                //Создаём тип "Укрепление"
                TypeSettlement typeSettlement = new("system", "Укрепление", 3, 4);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Маленький посёлок"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Маленький посёлок"))
            {
                //Создаём тип "Маленький посёлок"
                TypeSettlement typeSettlement = new("system", "Маленький посёлок", 5, 7);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Посёлок"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Посёлок"))
            {
                //Создаём тип "Посёлок"
                TypeSettlement typeSettlement = new("system", "Посёлок", 8, 9);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Замок"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Замок"))
            {
                //Создаём тип "Замок"
                TypeSettlement typeSettlement = new("system", "Замок", 10, 11);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Большой посёлок"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Большой посёлок"))
            {
                //Создаём тип "Большой посёлок"
                TypeSettlement typeSettlement = new("system", "Большой посёлок", 12, 13);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Маленький город"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Маленький город"))
            {
                //Создаём тип "Маленький город"
                TypeSettlement typeSettlement = new("system", "Маленький город", 14, 16);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Крепость"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Крепость"))
            {
                //Создаём тип "Крепость"
                TypeSettlement typeSettlement = new("system", "Крепость", 17, 18);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Город"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Город"))
            {
                //Создаём тип "Город"
                TypeSettlement typeSettlement = new("system", "Город", 19, 20);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Большой город"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Большой город"))
            {
                //Создаём тип "Большой город"
                TypeSettlement typeSettlement = new("system", "Большой город", 21, 23);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Цитадель"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Цитадель"))
            {
                //Создаём тип "Цитадель"
                TypeSettlement typeSettlement = new("system", "Цитадель", 24, 25);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Огромный город"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Огромный город"))
            {
                //Создаём тип "Огромный город"
                TypeSettlement typeSettlement = new("system", "Огромный город", 26, 27);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            //Проверяем наличие типа "Столица"
            if (!_repository.TypesSettlements.Any(x => x.Name == "Столица"))
            {
                //Создаём тип "Столица"
                TypeSettlement typeSettlement = new("system", "Столица", 28, 30);
                _repository.TypesSettlements.Add(typeSettlement);
                await _repository.SaveChangesAsync();
            }

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("Initialization. InitializeTypesSettlements. Ошибка: {0}", ex);
        }
    }
}
