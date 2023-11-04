using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.News.Request;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.AddNews;

/// <summary>
/// Сервис добавления новости
/// </summary>
public class AddNews : IAddNews
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса добавления новости
    /// </summary>
    /// <param name="repository"></param>
    public AddNews(ApplicationContext repository)
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
    public async Task<bool> Validator(string? user, AddNewsRequest? request)
    {
        //Проверяем на пустой запрос
        if (request == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на пустой заголовок
        if (string.IsNullOrEmpty(request.Title))
            throw new InnerException(Errors.EmptyTitle);

        //Проверяем на пустое вступление
        if (string.IsNullOrEmpty(request.Introduction))
            throw new InnerException(Errors.EmptyIntroduction);

        //Проверяем на пустой тип новости
        if ((request.TypeId ?? 0) == 0)
            throw new InnerException(Errors.EmptyTypeNews);

        //Проверяем на не существующий тип новости
        if (!await _repository.NewsTypes.AnyAsync(x => x.Id == request.TypeId))
            throw new InnerException(Errors.NotExistsTypeNews);

        //Проверяем на пустоту текущего пользователя
        if (string.IsNullOrEmpty(user))
            throw new InnerException(Errors.EmptyCurrentUser);

        //Проверяем на существующий элемент
        if (await _repository.News.AnyAsync(x => x.Title == request.Title))
            throw new InnerException(Errors.ExistingNews);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? user, AddNewsRequest? request)
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
    public async Task<long?> Query(string? user, AddNewsRequest? request)
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
                ordinalNumber = (await _repository.News.MaxAsync(x => (long?)x.OrdinalNumber) ?? 0) + 1;

            //Получаем связи с другими сущностями
            NewsType newsType = await _repository.NewsTypes.FirstAsync(x => x.Id == request!.TypeId);

            //Формируем экземпляр сущности и сохраняем в базу
            NewsEntity news = new(user!, false, request!.Title!, request!.Introduction!, newsType, request?.OrdinalNumber ?? ordinalNumber);
            _repository.News.Add(news);
            await _repository.SaveChangesAsync();

            //Фиксируем транзакцию
            await transaction.CommitAsync();

            //Записываем id имени
            id = news.Id;
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