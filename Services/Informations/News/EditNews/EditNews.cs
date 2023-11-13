using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.News.Request;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.News.EditNews;

/// <summary>
/// Сервис редактирования новости
/// </summary>
public class EditNews : IEditNews
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса редактирования новости
    /// </summary>
    /// <param name="repository"></param>
    public EditNews(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод обработки ошибки
    /// </summary>
    /// <param name="user"></param>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(string? user, AddNewsRequest? request, long? id)
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
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? user, AddNewsRequest? request, long? id)
    {
        try
        {
            //Проверяем на ошибки
            var resultValidator = await Validator(user, request, id);

            //Если успешно
            if (resultValidator)
            {
                //Получаем результат запроса
                var response = await Query(user, request, id);

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
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<long?> Query(string? user, AddNewsRequest? request, long? id)
    {
        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Сохраняем данные в базу
        try
        {
            //Получаем сущность новости
            NewsEntity news = await _repository.News.FirstAsync(x => x.Id == id);

            //Если указан заголовок отличный от текущего, меняем его
            if(news.Title != request!.Title)
                news.SetTitle(request.Title!);

            //Если указано вступление отличное от текущего, меняем его
            if (news.Introduction != request.Introduction)
                news.SetIntroduction(request.Introduction!);

            //Если указан тип отличный от текущего, меняем его
            if (news.TypeId != request.TypeId)
            {
                //Получаем связи с другими сущностями
                NewsType newsType = await _repository.NewsTypes.FirstAsync(x => x.Id == request!.TypeId);

                news.SetType(newsType);
            }

            //Если указан порядковый номер отличный от текущего, меняем его
            if (news.OrdinalNumber != request.OrdinalNumber && request.OrdinalNumber != null)
                news.SetOrdinalNumber(request.OrdinalNumber ?? 0);

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