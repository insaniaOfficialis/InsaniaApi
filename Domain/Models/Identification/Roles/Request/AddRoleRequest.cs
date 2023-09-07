namespace Domain.Models.Identification.Roles.Request;

/// <summary>
/// Модель запроса добавления ролей
/// </summary>
public class AddRoleRequest
{
    /// <summary>
    /// Наименование роли
    /// </summary>
    public string? Name { get; set; }
}
