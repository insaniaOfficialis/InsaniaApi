using AutoMapper;
using Data;
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
    private readonly RoleManager<Role> _roleManager; //менеджер ролей
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор класса роли
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="roleManager"></param>
    public Roles(IMapper mapper, RoleManager<Role> roleManager, ApplicationContext repository)
    {
        _mapper = mapper;
        _roleManager = roleManager;
        _repository = repository;
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
                return new BaseResponse(true, role.Id);
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

    /// <summary>
    /// Метод получения списка ролей
    /// </summary>
    /// <returns></returns>
    public async Task<BaseResponseList> GetRoles()
    {
        try
        {
            /*Получаем роли с базы*/
            var rolesEntity = _repository.Roles.ToList() ?? throw new InnerException("Не удалось найти роли");

            /*Преобразовываем модели*/
            var roles = rolesEntity.Select(_mapper.Map<BaseResponseListItem>).ToList() ?? throw new InnerException("Не удалось преобразовать модель базы данных в модель ответа");

            /*Формируем ответ*/
            return new BaseResponseList(true, null, roles!);
        }
        /*Обрабатываем внутренние исключения*/
        catch (InnerException ex)
        {
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        /*Обрабатываем системные исключения*/
        catch (Exception ex)
        {
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }
}
