using AutoMapper;
using Domain.Entities.Identification;
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
    private readonly SignInManager<User> _signInManager; //менеджер авторизации
    private readonly IToken _token; //сервис токенов

    /// <summary>
    /// Конструктор сервиса авторизации
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="signInManager"></param>
    /// <param name="token"></param>
    public Authorization(UserManager<User> userManager, SignInManager<User> signInManager, IToken token)
    {
        _userManager = userManager;
        _signInManager = signInManager;
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
        var validPassword = await _signInManager.CheckPasswordSignInAsync(user, password, true) ?? throw new InnerException("Некорректный пароль");

        /*Проверяем, что не исчерпано количество ввода пароля*/
        if (validPassword.IsLockedOut)
            throw new InnerException("Количество попыток ввода пароля исчерпано");

        /*Генерируем токен*/
        var token = _token.CreateToken(username);

        /*Возвращаем результат, где генерируем токен*/
        return new AuthorizationResponse { Success = true, Token = token };
    }
}
