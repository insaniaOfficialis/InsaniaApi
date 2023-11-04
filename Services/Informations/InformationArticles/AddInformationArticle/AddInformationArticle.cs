using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.InformationArticles.Request;
using Microsoft.EntityFrameworkCore;

namespace Services.Informations.InformationArticles.AddInformationArticle;

/// <summary>
/// Сервис добавления информационной статьи
/// </summary>
public class AddInformationArticle : IAddInformationArticle
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса добавления информационной статьи
    /// </summary>
    /// <param name="repository"></param>
    public AddInformationArticle(ApplicationContext repository)
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
    public async Task<bool> Validator(string? user, AddInformationArticleRequest? request)
    {
        //Проверяем на пустой запрос
        if (request == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на пустой заголовок
        if (string.IsNullOrEmpty(request.Title))
            throw new InnerException(Errors.EmptyTitle);

        //Проверяем на пустоту текущего пользователя
        if (string.IsNullOrEmpty(user))
            throw new InnerException(Errors.EmptyCurrentUser);

        //Проверяем на существующий элемент
        if (_repository.InformationArticles.Any(x => x.Title == request.Title))
            throw new InnerException(Errors.ExistingInformationArticle);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? user, AddInformationArticleRequest? request)
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
    public async Task<long?> Query(string? user, AddInformationArticleRequest? request)
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
            if(request?.OrdinalNumber == null)
                ordinalNumber = (await _repository.InformationArticles.MaxAsync(x => (long?)x.OrdinalNumber) ?? 0) + 1;

            //Формируем экземпляр сущности и сохраняем в базу
            InformationArticle informationArticle = new(user, false, request?.Title!, request?.OrdinalNumber ?? ordinalNumber);
            _repository.InformationArticles.Add(informationArticle);
            await _repository.SaveChangesAsync();

            //Фиксируем транзакцию
            await transaction.CommitAsync();

            //Записываем id имени
            id = informationArticle.Id;
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