using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.NewsDetails.Request;
using Microsoft.EntityFrameworkCore;
using NewsEntity = Domain.Entities.Informations.News;

namespace Services.Informations.NewsDetails.EditNewsDetail;

/// <summary>
/// Сервис редактирования детальной части новости
/// </summary>
public class EditNewsDetail : IEditNewsDetail
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса редактирования детальной части новости
    /// </summary>
    /// <param name="repository"></param>
    public EditNewsDetail(ApplicationContext repository)
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
    public async Task<bool> Validator(string? user, AddNewsDetailRequest? request, long? id)
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

        //Проверяем на пустоту ссылку на детальную часть новости
        if(id == null)
            throw new InnerException(Errors.EmptyId);

        //Проверяем на не существующую детальную часть новости
        if(!await _repository.NewsDetails.AnyAsync(x => x.Id == id))
            throw new InnerException(Errors.NotExistsNewsDetail);

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
    public async Task<BaseResponse> Handler(string? user, AddNewsDetailRequest? request, long? id)
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
    public async Task<long?> Query(string? user, AddNewsDetailRequest? request, long? id)
    {
        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Сохраняем данные в базу
        try
        {
            //Получаем экземпляр сущности
            NewsDetail entity = await _repository.NewsDetails.FirstAsync(x => x.Id == id);

            //Меняем текст, если он изменился
            if (entity.Text != request!.Text)
                entity.SetText(request.Text!);

            //Меняем новость, если она изменилась
            if (entity.NewsId != request.NewsId)
            {
                //Получаем связи с другими таблицами
                NewsEntity newsEntity = await _repository.News.FirstAsync(x => x.Id == request!.NewsId);

                //Записываем новую новость
                entity.SetNews(newsEntity);
            }

            //Меняем порядковый номер, елси он изменился
            if (entity.OrdinalNumber != request.OrdinalNumber)
                entity.SetOrdinalNumber(request.OrdinalNumber ?? 0);

            //Записываем сведения об изменении
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