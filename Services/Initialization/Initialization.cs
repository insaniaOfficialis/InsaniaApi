using Data;
using Domain.Entities.General.File;
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
                /*Проверяем наличие типа файла для пользователей*/
                if (!_repository.FileTypes.Any(x => x.Name == "Пользователь"))
                {
                    /*Создаём тип файла для пользователей*/
                    FileType fileType = new("system", "Пользователь", "User", "I:\\Insania\\ПО\\Files");
                    _repository.FileTypes.Add(fileType);
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
