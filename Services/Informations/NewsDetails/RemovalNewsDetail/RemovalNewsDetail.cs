using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;

namespace Services.Informations.NewsDetails.RemovalNewsDetail;

/// <summary>
/// Сервис удаления/восстановления детальной части новости
/// </summary>
public class RemovalNewsDetail : IRemovalNewsDetail
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса удаления/восстановления детальной части новости
    /// </summary>
    /// <param name="repository"></param>
    public RemovalNewsDetail(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки ошибки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(string? user, long? id)
    {
        //Проверяем на пустоту текущего пользователя
        if (string.IsNullOrEmpty(user))
            throw new InnerException(Errors.EmptyCurrentUser);

        //Проверяем на отсутсвие id
        if (id == null)
            throw new InnerException(Errors.EmptyId);

        //Проверяем на отсутсвие детальной части новости
        if (!await _repository.NewsDetails.AnyAsync(x => x.Id == id))
            throw new InnerException(Errors.NotExistsNewsDetail);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? user, long? id, bool? isDeleted)
    {
        try
        {
            //Проверяем на ошибки
            var resultValidator = await Validator(user, id);

            //Если успешно
            if (resultValidator)
            {
                //Получаем результат запроса
                var response = await Query(user, id, isDeleted);

                //Формируем ответ
                return new BaseResponse(true, response ?? 0);
            }
            else
            {
                return new BaseResponse(false, new BaseError(400, Errors.NotValidate));
            }
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new BaseResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new BaseResponse(false, new BaseError(500, ex.Message));
        }

    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<long?> Query(string? user, long? id, bool? isDeleted)
    {
        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Сохраняем данные в базу
        try
        {
            //Получаем сущность новости
            NewsDetail entity = await _repository.NewsDetails.FirstAsync(x => x.Id == id);

            //Если указано удаление и элемент - не удалён, помечаем его удаленным
            if (isDeleted == true && !entity.IsDeleted)
                entity.SetDeleted();

            //Если указано восстановление и элемент - удалён, помечаем его не удаленным
            if (isDeleted == false && entity.IsDeleted)
                entity.SetRestored();

            //Записываем изменения
            entity.SetUpdate(user);

            //Сохраняем в базу
            _repository.NewsDetails.Update(entity);
            await _repository.SaveChangesAsync();

            //Фиксируем транзакцию
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            //Откатываем транзакцию
            transaction.Rollback();

            //Прокидываем исключение
            throw new Exception(ex.Message, ex);
        }

        //Формируем ответ
        return id;
    }
}