using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.RemovalNews;

/// <summary>
/// Сервис удаления/восстановления новости
/// </summary>
public class RemovalNews : IRemovalNews
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса удаления/восстановления новости
    /// </summary>
    /// <param name="repository"></param>
    public RemovalNews(ApplicationContext repository)
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

        //Проверяем на отсутсвующую новость
        if (!await _repository.News.AnyAsync(x => x.Id == id))
            throw new InnerException(Errors.NotExistsNews);

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
            NewsEntity news = await _repository.News.FirstAsync(x => x.Id == id);

            //Если указано удаление и элемент - не удалён, помечаем его удаленным
            if (isDeleted == true && !news.IsDeleted)
                news.SetDeleted();

            //Если указано восстановление и элемент - удалён, помечаем его не удаленным
            if (isDeleted == false && news.IsDeleted)
                news.SetRestored();

            //Записываем изменения
            news.SetUpdate(user);

            //Сохраняем в базу
            _repository.News.Update(news);
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