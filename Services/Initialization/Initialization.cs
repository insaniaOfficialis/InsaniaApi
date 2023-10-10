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

    /// <summary>
    /// Конструктор сервиса инициализации
    /// </summary>
    /// <param name="roleManager"></param>
    /// <param name="userManager"></param>
    /// <param name="repository"></param>
    /// <param name="logger"></param>
    public Initialization(RoleManager<Role> roleManager, UserManager<User> userManager, ApplicationContext repository, ILogger<Initialization> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _repository = repository;
        _logger = logger;
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


            //ПОЛЬЗОВАТЕЛИ

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

            //Открываем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                //ПАРАМЕТРЫ

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


                //ТИПЫ ФАЙЛОВ

                //Проверяем наличие типа файла для пользователей
                if (!_repository.FileTypes.Any(x => x.Name == "Пользователь"))
                {
                    //Создаём тип файла для пользователей
                    FileType fileType = new("system", "Пользователь", "I:\\Insania\\ПО\\Files");
                    _repository.FileTypes.Add(fileType);
                    await _repository.SaveChangesAsync();
                }


                //КЛИМАТЫ

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


                //РЕЛЬЕФЫ

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


                //РАСЫ

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


                //НАЦИИ

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


                //ИМЕНА

                await InitializePersonalNames();

                //СТРАНЫ

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


                //РЕГИОНЫ

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


                //ТИПЫ НАСЕЛЁННЫХ ПУНКТОВ

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

            //Проверяем наличие имени "Фенелла"
            if (!_repository.PersonalNames.Any(x => x.Name == "Фенелла"))
            {
                //Создаём имя "Фенелла"
                PersonalName personalName = new("system", "Фенелла", false);
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
        catch(Exception ex)
        {
            throw new Exception("Initialization. InitializePersonalNames. Ошибка: {0}", ex);
        }
    }
}
