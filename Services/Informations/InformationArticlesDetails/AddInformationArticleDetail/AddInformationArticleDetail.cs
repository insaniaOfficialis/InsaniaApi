using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.InformationArticlesDetails.Request;

namespace Services.Informations.InformationArticlesDetails.AddInformationArticleDetail;

/// <summary>
/// Сервис добавления детальной части информационной статьи
/// </summary>
public class AddInformationArticleDetail : IAddInformationArticleDetail
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса добавления детальной части информационной статьи
    /// </summary>
    /// <param name="repository"></param>
    public AddInformationArticleDetail(ApplicationContext repository)
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
    public async Task<bool> Validator(string? user, AddInformationArticleDetailRequest? request)
    {
        //Проверяем на пустой запрос
        if (request == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на пустой текст
        if (string.IsNullOrEmpty(request.Text))
            throw new InnerException(Errors.EmptyText);

        //Проверяем на пустую ссылку информационной статьи
        if (request.InformationArticleId == null)
            throw new InnerException(Errors.EmptyInformationArticleId);

        //Проверяем на не существующую информационную статью
        if (!_repository.InformationArticles.Any(x => x.Id == request.InformationArticleId))
            throw new InnerException(Errors.NotExistsInformationArticle);

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
    public async Task<BaseResponse> Handler(string? user, AddInformationArticleDetailRequest? request)
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
    public async Task<long?> Query(string? user, AddInformationArticleDetailRequest? request)
    {
        //Создаём переменную первичного ключа
        long? id;

        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Сохраняем данные в базу
        try
        {
            //Получаем связи с другими таблицами
            InformationArticle? informationArticle = _repository
                .InformationArticles
                .FirstOrDefault(x => x.Id == request!.InformationArticleId);

            //Формируем экземпляр сущности и сохраняем в базу
            InformationArticleDetail entity = new(user, false, request!.Text!, informationArticle!);
            _repository.InformationArticlesDetails.Add(entity);
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