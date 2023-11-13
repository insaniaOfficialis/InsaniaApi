using AutoMapper;
using Data;
using Domain.Entities.Identification;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Identification.Authorization.Response;
using Domain.Models.Identification.Users.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.General.Files.GetFilesUser;
using Services.Identification.Token;

namespace Services.Identification.Authorization;

/// <summary>
/// Сервис авторизации
/// </summary>
public class Authorization: IAuthorization
{
    private readonly UserManager<User> _userManager; //менеджер пользователей
    private readonly IToken _token; //сервис токенов
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly IGetFilesUser _getFilesUser; //сервис получения файлов пользователя

    /// <summary>
    /// Конструктор сервиса авторизации
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="token"></param>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    /// <param name="getFilesUser"></param>
    public Authorization(UserManager<User> userManager, IToken token, IMapper mapper, ApplicationContext repository,
        IGetFilesUser getFilesUser)
    {
        _userManager = userManager;
        _token = token;
        _mapper = mapper;
        _repository = repository;
        _getFilesUser = getFilesUser;
    }

    /// <summary>
    /// Метод авторизации
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<AuthorizationResponse> Login(string? username, string? password)
    {
        try
        {
            //Проверяем, что передали логин
            if (String.IsNullOrEmpty(username))
                throw new InnerException("Не указан логин");

            //Проверяем, что передали пароль
            if (String.IsNullOrEmpty(password))
                throw new InnerException("Не указан пароль");

            //Проверяем наличие пользователя
            var user = await _userManager.FindByNameAsync(username) ?? throw new InnerException("Пользователь не найден");

            //Проверяем, что пользователь не заблокирован
            if (user.IsBlocked)
                throw new InnerException("Пользователь заблокирован");

            //Проверяем корректность пароля
            PasswordHasher<User> passwordHasher = new();
            var validatePassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);
            if (validatePassword != PasswordVerificationResult.Success)
                throw new InnerException("Пароль некорректный");

            //Генерируем токен
            var token = _token.CreateToken(username);

            //Возвращаем результат, где генерируем токен
            return new AuthorizationResponse(true, null, token);
        }

        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new AuthorizationResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new AuthorizationResponse(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод получения информации о пользователе
    /// </summary>
    /// <returns></returns>
    public async Task<UserInfoResponse> GetUserInfo(string? username)
    {
        try
        {
            //Проверяем входящие данные
            if (String.IsNullOrEmpty(username))
                throw new InnerException("Не указан пользователь");

            //Получаем пользователя
            var user = await _userManager.FindByNameAsync(username) ?? throw new InnerException("Пользователь не найден");

            //Получаем роли
            var roles = await _userManager.GetRolesAsync(user) as List<string>
                ?? throw new InnerException("Не удалось получить роли пользователя");

            //Получаем права доступа
            List<string> accessRights = await _repository
                .RolesAcccessRights
                .Include(x => x.Role)
                .Include(x => x.AccessRight)
                .Where(x => roles.Contains(x.Role.Name!) && x.DateDeleted == null && x.AccessRight.DateDeleted == null)
                .Select(x => x.AccessRight.Alias)
                .Distinct()
                .ToListAsync();

            //Получаем файлы пользователя
            List<long?>? files = new();
            var filesRepose = await _getFilesUser.Handler(user.Id);
            if (filesRepose != null && filesRepose.Items?.Any() == true)
                files = filesRepose.Items.Select(x => x?.Id).ToList();

            //Формируем ответ
            return new UserInfoResponse(true, user.Id, user.UserName, user.FirstName, user.LastName, user.Patronymic,
                user.FullName, user.Initials, user.Gender, user.Email, user.PhoneNumber, user.IsBlocked, roles, accessRights, files);
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new UserInfoResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new UserInfoResponse(false, new BaseError(500, ex.Message));
        }
    }
}