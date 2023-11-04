using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.NewsDetails.Response;
using Microsoft.EntityFrameworkCore;
using Services.General.Files.GetFilesNewsDetails;

namespace Services.Informations.NewsDetails.GetNewsDetails;

/// <summary>
/// Сервис получения списка детальных частей новости
/// </summary>
public class GetNewsDetails : IGetNewsDetails
{
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly IGetFilesNewsDetails _getFilesNewsDetails; //сервис получения списка файлов детальной части новости

    /// <summary>
    /// Конструктор сервиса получения списка детальных частей новости
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="getFilesNewsDetails"></param>
    public GetNewsDetails(ApplicationContext repository, IGetFilesNewsDetails getFilesNewsDetails)
    {
        _repository = repository;
        _getFilesNewsDetails = getFilesNewsDetails;
    }

    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="newsId"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(long? newsId)
    {
        //Проверяем на пустой запрос
        if (newsId == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на не существующую новость
        if (!await _repository.News.AnyAsync(x => x.Id == newsId))
            throw new InnerException(Errors.NotExistsNews);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="newsId"></param>
    /// <returns></returns>
    public async Task<GetNewsDetailsResponse> Handler(long? newsId)
    {
        try
        {
            //Проверяем входные данные
            var validate = await Validator(newsId);

            //Если проверка успешная
            if (validate)
            {
                //Получаем результат запроса
                var response = await Query(newsId);

                //Формируем ответ
                return await RequestBuilder(response);
            }
            //Иначе
            else
            {
                return new GetNewsDetailsResponse(false, new BaseError(400, Errors.NotValidate));
            }
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new GetNewsDetailsResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new GetNewsDetailsResponse(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="newsId"></param>
    /// <returns></returns>
    public async Task<List<NewsDetail>> Query(long? newsId)
    {
        //Строим запрос
        IQueryable<NewsDetail> query = _repository
            .NewsDetails
            .Where(x => x.DateDeleted == null && x.NewsId == newsId)
            .OrderBy(x => x.OrdinalNumber);

        //Получаем данные с базы
        var entities = await query.ToListAsync();

        //Формируем ответ
        return entities;
    }

    /// <summary>
    /// Метод формирования ответа
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<GetNewsDetailsResponse> RequestBuilder(List<NewsDetail> request)
    {
        //Формируем модель ответа
        GetNewsDetailsResponse response = new(true, null, new());

        //Проходим циклом по списку детальных частей
        foreach (var item in request)
        {
            //Получаем файлы
            BaseResponseList filesItem = await _getFilesNewsDetails.Handler(item.Id);

            //Если данные пришли
            if (filesItem != null && filesItem.Items != null && filesItem.Items.Any())
            {
                //Формируем новый элемент
                GetNewsDetailsResponseItem responseItem = new(item.Text, filesItem.Items.Select(x => x!.Id ?? 0).ToList());

                //Добавляем в ответ новый элемент
                response.Items!.Add(responseItem);
            }
        }

        //Возвращаем ответ
        return response;
    }
}