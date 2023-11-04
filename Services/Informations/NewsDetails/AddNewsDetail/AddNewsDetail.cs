using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.NewsDetails.Request;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.NewsDetails.AddNewsDetail;

/// <summary>
/// Сервис добавления детальной части новости
/// </summary>
public class AddNewsDetail : IAddNewsDetail
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса добавления детальной части новости
    /// </summary>
    /// <param name="repository"></param>
    public AddNewsDetail(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки ошибки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(string? user, AddNewsDetailRequest? request)
    {
        //Проверяем на пустой запрос
        if (request == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на пустой текст
        if (string.IsNullOrEmpty(request.Text))
            throw new InnerException(Errors.EmptyText);

        //Проверяем на пустую ссылку новости
        if (request.NewsId == null)
            throw new InnerException(Errors.EmptyNewsId);

        //Проверяем на не существующую новость
        if (!await _repository.News.AnyAsync(x => x.Id == request.NewsId))
            throw new InnerException(Errors.NotExistsNews);

        //Проверяем на пустоту текущего пользователя
        if (string.IsNullOrEmpty(user))
            throw new InnerException(Errors.EmptyCurrentUser);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? user, AddNewsDetailRequest? request)
    {
        try
        {
            //Проверяем на ошибки
            var resultValidator = await Validator(user, request);

            //Если успешно
            if (resultValidator)
            {
                //Получаем результат запроса
                var response = await Query(user, request);

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
    /// <param name="request"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<long?> Query(string? user, AddNewsDetailRequest? request)
    {
        //Создаём переменную первичного ключа
        long? id;

        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Сохраняем данные в базу
        try
        {
            //Получаем максимальный порядковый номер имеющихся записей
            long ordinalNumber = 0;
            if (request?.OrdinalNumber == null)
                ordinalNumber = (await _repository.NewsDetails.MaxAsync(x => (long?)x.OrdinalNumber) ?? 0) + 1;

            //Получаем связи с другими таблицами
            NewsEntity? newsEntity = _repository
                .News
                .FirstOrDefault(x => x.Id == request!.NewsId);

            //Формируем экземпляр сущности и сохраняем в базу
            NewsDetail entity = new(user, false, request!.Text!, newsEntity!, request?.OrdinalNumber ?? ordinalNumber);
            _repository.NewsDetails.Add(entity);
            await _repository.SaveChangesAsync();

            //Фиксируем транзакцию
            await transaction.CommitAsync();

            //Записываем id имени
            id = entity.Id;
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