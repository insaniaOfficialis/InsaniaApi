using AutoMapper;
using Domain.Entities.Identification;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Identification.Roles.Request;
using Microsoft.AspNetCore.Identity;

namespace Services.Identification.Roles;

/// <summary>
/// Сервис ролей
/// </summary>
public class Roles: IRoles
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly RoleManager<Role> _roleManager; //менеджер пользователей

    /// <summary>
    /// Конструктор класса роли
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="roleManager"></param>
    public Roles(IMapper mapper, RoleManager<Role> roleManager)
    {
        _mapper = mapper;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Метод добавления роли
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseResponse> AddRole(AddRoleRequest? request)
    {
        try
        {
            /*Проверяем корректность данных*/
            if (request == null)
                throw new InnerException("Пустой запрос");

            if (String.IsNullOrEmpty(request.Name))
                throw new InnerException("Не указано наименование");

            /*Преобразуем модель запроса в модель роли*/
            var role = _mapper.Map<Role>(request) ?? throw new InnerException("Не удалось преобразовать модель запроса в модель роли");

            /*Создаём роль*/
            var result = await _roleManager.CreateAsync(role) ?? throw new InnerException("Не удалось создать роль");

            /*Если успешно, выводим результат*/
            if (result.Succeeded)
                return new BaseResponse(true);
            /*Иначе выбиваем ошибку*/
            else
                throw new InnerException(result?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
        }
        /*Обрабатываем внутренние исключения*/
        catch (InnerException ex)
        {
            return new BaseResponse(false, new BaseError(400, ex.Message));
        }
        /*Обрабатываем системные исключения*/
        catch (Exception ex)
        {
            return new BaseResponse(false, new BaseError(500, ex.Message));
        }
    }
}
