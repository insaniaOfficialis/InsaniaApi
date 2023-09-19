using AutoMapper;
using Domain.Entities.Identification;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Identification.Authorization.Response;
using Microsoft.AspNetCore.Identity;
using Services.Identification.Token;

namespace Services.Identification.Authorization;

/// <summary>
/// Сервис авторизации
/// </summary>
public class Authorization: IAuthorization
{
    private readonly UserManager<User> _userManager; //менеджер пользователей
    private readonly IToken _token; //сервис токенов

    /// <summary>
    /// Конструктор сервиса авторизации
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="token"></param>
    public Authorization(UserManager<User> userManager, IToken token)
    {
        _userManager = userManager;
        _token = token;
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
            /*Проверяем, что передали логин*/
            if (String.IsNullOrEmpty(username))
                throw new InnerException("Не указан логин");

            /*Проверяем, что передали пароль*/
            if (String.IsNullOrEmpty(password))
                throw new InnerException("Не указан пароль");

            /*Проверяем наличие пользователя*/
            var user = await _userManager.FindByNameAsync(username) ?? throw new InnerException("Пользователь не найден");

            /*Проверяем, что пользователь не заблокирован*/
            if (user.IsBlocked)
                throw new InnerException("Пользователь заблокирован");

            /*Проверяем корректность пароля*/
            PasswordHasher<User> passwordHasher = new();
            var validatePassword = passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, password);
            if (validatePassword != PasswordVerificationResult.Success)
                throw new InnerException("Пароль некорректный");

            /*Генерируем токен*/
            var token = _token.CreateToken(username);

            /*Возвращаем результат, где генерируем токен*/
            return new AuthorizationResponse(true, null, token);
        }

        /*Обрабатываем внутренние исключения*/
        catch (InnerException ex)
        {
            return new AuthorizationResponse(false, new BaseError(400, ex.Message));
        }
        /*Обрабатываем системные исключения*/
        catch (Exception ex)
        {
            return new AuthorizationResponse(false, new BaseError(500, ex.Message));
        }
    }
}
