namespace Services.Identification.Token;

/// <summary>
/// Интерфейс токенов
/// </summary>
public interface IToken
{
    /// <summary>
    /// Метод генерации токена
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    string CreateToken(string username);
}
