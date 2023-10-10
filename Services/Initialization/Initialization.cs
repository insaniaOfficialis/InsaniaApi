using Data;
using Domain.Entities.General.File;
using Domain.Entities.General.System;
using Domain.Entities.Politics;
using Domain.Entities.Identification;
using Domain.Models.Exclusion;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Domain.Entities.Geography;

namespace Services.Initialization;

/// <summary>
/// Сервис инициализации
/// </summary>
public class Initialization: IInitialization
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
}
