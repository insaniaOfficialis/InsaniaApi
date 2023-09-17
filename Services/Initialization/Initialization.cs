using Data;
using Domain.Entities.General.File;
using Domain.Entities.General.System;
using Domain.Entities.Geography;
using Domain.Entities.Identification;
using Domain.Models.Exclusion;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
            /*РОЛИ*/

            /*Проверяем наличие роли админа*/
            if (_roleManager.FindByNameAsync("admin").Result == null)
            {
                /*Добавляем роль админа*/
                Role role = new("admin");
                var resultRole = await _roleManager.CreateAsync(role) ?? throw new Exception("Не удалось создать роль");

                /*Если не успешно, выдаём ошибку*/
                if (!resultRole.Succeeded)
                    throw new Exception(resultRole?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
            }


            /*ПОЛЬЗОВАТЕЛИ*/

            /*Проверяем наличие пользователя инсании*/
            if (_userManager.FindByNameAsync("insania").Result == null)
            {
                /*Добавляем пользователя инсании*/
                User user = new("insania", "insania_officialis@vk.com", "+79996370439", false);
                var resultUser = await _userManager.CreateAsync(user, "K02032018v.") ?? throw new Exception("Не удалось создать пользователя");

                /*Если успешно*/
                if (resultUser.Succeeded)
                {
                    /*Добавляем к инсании роль админа*/
                    var resultUserRole = await _userManager.AddToRoleAsync(user, "admin") ?? throw new InnerException("Не удалось добавить роль пользователю");

                    /*Если не успешно, выдаём ошибку*/
                    if (!resultUserRole.Succeeded)
                        throw new InnerException(resultUserRole?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
                }
                /*Иначе выдаём ошибку*/
                else
                    throw new Exception(resultUser?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
            }

            /*Открываем транзакцию*/
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                /*ПАРАМЕТРЫ*/

                /*Проверяем наличие параметра цвета чисел на карте*/
                if(!_repository.Parametrs.Any(x => x.Name == "Цвет чисел на карте"))
                {
                    /*Создаём параметр для цвета чисел на карте*/
                    Parametr parametr = new("system", "Цвет чисел на карте", "#7E0000");
                    _repository.Parametrs.Add(parametr);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие параметра размера чисел стран на карте*/
                if (!_repository.Parametrs.Any(x => x.Name == "Размер чисел стран на карте"))
                {
                    /*Создаём параметр для размера чисел стран на карте*/
                    Parametr parametr = new("system", "Размер чисел стран на карте", "80");
                    _repository.Parametrs.Add(parametr);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие параметра шрифт чисел стран на карте*/
                if (!_repository.Parametrs.Any(x => x.Name == "Шрифт чисел стран на карте"))
                {
                    /*Создаём параметр для шрифта чисел стран на карте*/
                    Parametr parametr = new("system", "Шрифт чисел стран на карте", "Times New Roman");
                    _repository.Parametrs.Add(parametr);
                    await _repository.SaveChangesAsync();
                }

                /*ТИПЫ ФАЙЛОВ*/

                /*Проверяем наличие типа файла для пользователей*/
                if (!_repository.FileTypes.Any(x => x.Name == "Пользователь"))
                {
                    /*Создаём тип файла для пользователей*/
                    FileType fileType = new("system", "Пользователь", "I:\\Insania\\ПО\\Files");
                    _repository.FileTypes.Add(fileType);
                    await _repository.SaveChangesAsync();
                }


                /*СТРАНЫ*/

                /*Проверяем наличие страны "Альвраатская империя"*/
                if (!_repository.Countries.Any(x => x.Name == "Альвраатская империя"))
                {
                    /*Создаём страну "Альвраатская империя"*/
                    Country country = new("system", "Альвраатская империя", 1, "#20D1DB", "Исландский");
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Княжество Саорса"*/
                if (!_repository.Countries.Any(x => x.Name == "Княжество Саорса"))
                {
                    /*Создаём страну "Княжество Саорса"*/
                    Country country = new("system", "Княжество Саорса", 2, "#808080", "Шотландский");
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Королевство Берген"*/
                if (!_repository.Countries.Any(x => x.Name == "Королевство Берген"))
                {
                    /*Создаём страну "Королевство Берген"*/
                    Country country = new("system", "Королевство Берген", 3, "#00687C", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Фесгарское княжество"*/
                if (!_repository.Countries.Any(x => x.Name == "Фесгарское княжество"))
                {
                    /*Создаём страну "Фесгарское княжество"*/
                    Country country = new("system", "Фесгарское княжество", 4, "#B200FF", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Сверденский каганат"*/
                if (!_repository.Countries.Any(x => x.Name == "Сверденский каганат"))
                {
                    /*Создаём страну "Сверденский каганат"*/
                    Country country = new("system", "Сверденский каганат", 5, "#7F3B00", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Ханство Тавалин"*/
                if (!_repository.Countries.Any(x => x.Name == "Ханство Тавалин"))
                {
                    /*Создаём страну "Ханство Тавалин"*/
                    Country country = new("system", "Ханство Тавалин", 6, "#7F006D", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Княжество Саргиб"*/
                if (!_repository.Countries.Any(x => x.Name == "Княжество Саргиб"))
                {
                    /*Создаём страну "Княжество Саргиб"*/
                    Country country = new("system", "Княжество Саргиб", 7, "#007F0E", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Царство Банду"*/
                if (!_repository.Countries.Any(x => x.Name == "Царство Банду"))
                {
                    /*Создаём страну "Царство Банду"*/
                    Country country = new("system", "Царство Банду", 8, "#47617C", "Индийский");
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Королевство Нордер"*/
                if (!_repository.Countries.Any(x => x.Name == "Королевство Нордер"))
                {
                    /*Создаём страну "Королевство Нордер"*/
                    Country country = new("system", "Королевство Нордер", 9, "#D82929", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Альтерское княжество"*/
                if (!_repository.Countries.Any(x => x.Name == "Альтерское княжество"))
                {
                    /*Создаём страну "Альтерское княжество"*/
                    Country country = new("system", "Альтерское княжество", 10, "#4ACC39", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Орлиадарская конведерация"*/
                if (!_repository.Countries.Any(x => x.Name == "Орлиадарская конведерация"))
                {
                    /*Создаём страну "Орлиадарская конведерация"*/
                    Country country = new("system", "Орлиадарская конведерация", 11, "#AF9200", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Королевство Удстир"*/
                if (!_repository.Countries.Any(x => x.Name == "Королевство Удстир"))
                {
                    /*Создаём страну "Королевство Удстир"*/
                    Country country = new("system", "Королевство Удстир", 12, "#8CAF00", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Королевство Вервирунг"*/
                if (!_repository.Countries.Any(x => x.Name == "Королевство Вервирунг"))
                {
                    /*Создаём страну "Королевство Вервирунг"*/
                    Country country = new("system", "Королевство Вервирунг", 13, "#7F1700", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Дестинский орден"*/
                if (!_repository.Countries.Any(x => x.Name == "Дестинский орден"))
                {
                    /*Создаём страну "Дестинский орден"*/
                    Country country = new("system", "Дестинский орден", 14, "#2B7C55", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Вольный город Лийсет"*/
                if (!_repository.Countries.Any(x => x.Name == "Вольный город Лийсет"))
                {
                    /*Создаём страну "Вольный город Лийсет"*/
                    Country country = new("system", "Вольный город Лийсет", 15, "#7B7F00", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Лисцийская империя"*/
                if (!_repository.Countries.Any(x => x.Name == "Лисцийская империя"))
                {
                    /*Создаём страну "Лисцийская империя"*/
                    Country country = new("system", "Лисцийская империя", 16, "#7F002E", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Королевство Вальтир"*/
                if (!_repository.Countries.Any(x => x.Name == "Королевство Вальтир"))
                {
                    /*Создаём страну "Королевство Вальтир"*/
                    Country country = new("system", "Королевство Вальтир", 17, "#B05BFF", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Вассальное княжество Гратис"*/
                if (!_repository.Countries.Any(x => x.Name == "Вассальное княжество Гратис"))
                {
                    /*Создаём страну "Вассальное княжество Гратис"*/
                    Country country = new("system", "Вассальное княжество Гратис", 18, "#005DFF", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Княжество Ректа"*/
                if (!_repository.Countries.Any(x => x.Name == "Княжество Ректа"))
                {
                    /*Создаём страну "Княжество Ректа"*/
                    Country country = new("system", "Княжество Ректа", 19, "#487F00", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Волар"*/
                if (!_repository.Countries.Any(x => x.Name == "Волар"))
                {
                    /*Создаём страну "Волар"*/
                    Country country = new("system", "Волар", 20, "#32217A", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Союз Иль-Ладро"*/
                if (!_repository.Countries.Any(x => x.Name == "Союз Иль-Ладро"))
                {
                    /*Создаём страну "Союз Иль-Ладро"*/
                    Country country = new("system", "Союз Иль-Ладро", 21, "#35513B", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Проверяем наличие страны "Уния Ангуи"*/
                if (!_repository.Countries.Any(x => x.Name == "Уния Ангуи"))
                {
                    /*Создаём страну "Уния Ангуи"*/
                    Country country = new("system", "Уния Ангуи", 22, "#BC3CB4", null);
                    _repository.Countries.Add(country);
                    await _repository.SaveChangesAsync();
                }

                /*Фиксируем транзакцию*/
                await transaction.CommitAsync();
                return true;
            }
            catch(Exception ex)
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
