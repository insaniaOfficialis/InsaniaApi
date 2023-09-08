using AutoMapper;
using Domain.Entities.Identification;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Identification.Registration.Request;
using Microsoft.AspNetCore.Identity;

namespace Services.Identification.Registration;

/// <summary>
/// Сервис регистрации
/// </summary>
public class Registration: IRegistration
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly UserManager<User> _userManager; //менеджер пользователей

    /// <summary>
    /// Конструктор класса регистрации
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="userManager"></param>
    public Registration(IMapper mapper, UserManager<User> userManager)
    {
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// Метод добавления пользователя
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseResponse> AddUser(AddUserRequest? request)
    {
        try
        {
            /*Проверяем корректность данных*/
            if (request == null)
                throw new InnerException("Пустой запрос");

            if (String.IsNullOrEmpty(request.UserName))
                throw new InnerException("Не указан логин");

            if (String.IsNullOrEmpty(request.Email))
                throw new InnerException("Не указана почта");

            if (String.IsNullOrEmpty(request.PhoneNumber))
                throw new InnerException("Не указан номер телефона");

            if (String.IsNullOrEmpty(request.Password))
                throw new InnerException("Не указан пароль");

            if (String.IsNullOrEmpty(request.LastName) || String.IsNullOrEmpty(request.FirstName) || String.IsNullOrEmpty(request.Patronymic))
                throw new InnerException("Не указана фамилия, имя или отчество");

            if (request.Roles?.Any() != true)
                throw new InnerException("Не указаны роли");

            /*Преобразуем модель запроса в модель пользователя*/
            var user = _mapper.Map<User>(request) ?? throw new InnerException("Не удалось преобразовать модель запроса в модель пользователя");

            /*Регистрируем пользователя*/
            var result = await _userManager.CreateAsync(user, request.Password) ?? throw new InnerException("Не удалось создать пользователя");

            /*Если успешно, создаём роли*/
            if (result.Succeeded)
            {
                /*Добавляем роли пользователю*/
                result = await _userManager.AddToRolesAsync(user, request.Roles) ?? throw new InnerException("Не удалось добавить роли");

                /*Если успешно, выводим результат*/
                if (result.Succeeded)
                    return new BaseResponse(true, user.Id);
                /*Иначе выбиваем ошибку*/
                else
                    throw new InnerException(result?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
            }
            /*Иначе выбиваем ошибку*/
            else
                throw new InnerException(result?.Errors?.FirstOrDefault()?.Description ?? "Неопознанная ошибка");
        }
        /*Обрабатываем внутренние исключения*/
        catch(InnerException ex)
        {
            return new BaseResponse(false, new BaseError(400, ex.Message));
        }
        /*Обрабатываем системные исключения*/
        catch(Exception ex)
        {
            return new BaseResponse(false, new BaseError(500, ex.Message));
        }
    }
}