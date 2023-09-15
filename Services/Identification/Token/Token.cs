using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.Identification.Token;

/// <summary>
/// Сервис токенов
/// </summary>
public class Token: IToken
{
    private readonly IConfiguration _configuration; //конфигурация

    /// <summary>
    /// Конструктор сервиса токенов
    /// </summary>
    /// <param name="configuration"></param>
    public Token(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Метод генерации токена
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public string CreateToken(string username)
    {
        /*Объявляем переменные*/
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
        string? issuer = _configuration["TokenOptions:Issuer"];
        string? audience = _configuration["TokenOptions:Audience"];
        double expires = Convert.ToDouble(_configuration["TokenOptions:Expires"]);
        string? key = _configuration["TokenOptions:Key"];

        /*Проверяем корректность данных*/
        if (string.IsNullOrEmpty(username))
            throw new InnerException("Не указан логин");

        if (string.IsNullOrEmpty(issuer))
            throw new InnerException("В файле конфигурации не указан отправитель");

        if (string.IsNullOrEmpty(audience))
            throw new InnerException("В файле конфигурации не указан получатель");

        if (string.IsNullOrEmpty(key))
            throw new InnerException("В файле конфигурации не указан ключ");

        if(Convert.ToDouble(expires) == 0)
            throw new InnerException("В файле конфигурации не указано время жизни токена");

        /*Создаем JWT-токен*/
        var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(expires),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256));

        /*Возвращаем токен*/
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    /// <summary>
    /// Метод генерации ключа
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static SymmetricSecurityKey GetSymmetricSecurityKey(string key) =>
        new(Encoding.ASCII.GetBytes(key));
}
