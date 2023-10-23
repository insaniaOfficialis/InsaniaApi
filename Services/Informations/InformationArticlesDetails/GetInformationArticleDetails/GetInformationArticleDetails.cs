using Data;
using Domain.Entities.Informations;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.Informations.InformationArticlesDetails.Response;
using Microsoft.EntityFrameworkCore;
using Services.General.Files.GetFilesInformationArticleDetails;

namespace Services.Informations.InformationArticlesDetails.GetInformationArticleDetails;

/// <summary>
/// Сервис получения списка детальных частей информационной статьи
/// </summary>
public class GetInformationArticleDetails : IGetInformationArticleDetails
{
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly IGetFilesInformationArticleDetails _getFilesInformationArticleDetails; //сервис получения списка файлов детальной части информационной статьи
    
    /// <summary>
    /// Конструктор сервиса получения списка детальных частей информационной статьи
    /// </summary>
    /// <param name="repository"></param>
    /// <param name="getFilesInformationArticleDetails"></param>
    public GetInformationArticleDetails(ApplicationContext repository, IGetFilesInformationArticleDetails getFilesInformationArticleDetails)
    {
        _repository = repository;
        _getFilesInformationArticleDetails = getFilesInformationArticleDetails;
    }

    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="informationArticleId"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(long? informationArticleId)
    {
        //Проверяем на пустой запрос
        if (informationArticleId == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на не существующую информационную статью
        if (!await _repository.InformationArticles.AnyAsync(x => x.Id == informationArticleId))
            throw new InnerException(Errors.NotExistsInformationArticle);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="informationArticleId"></param>
    /// <returns></returns>
    public async Task<GetInformationArticleDetailsResponse> Handler(long? informationArticleId)
    {
        try
        {
            //Проверяем входные данные
            var validate = await Validator(informationArticleId);

            //Если проверка успешная
            if (validate)
            {
                //Получаем результат запроса
                var response = await Query(informationArticleId);

                //Формируем ответ
                return await RequestBuilder(response);
            }
            //Иначе
            else
            {
                return new GetInformationArticleDetailsResponse(false, new BaseError(400, Errors.NotValidate));
            }
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new GetInformationArticleDetailsResponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new GetInformationArticleDetailsResponse(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="informationArticleId"></param>
    /// <returns></returns>
    public async Task<List<InformationArticleDetail>> Query(long? informationArticleId)
    {
        //Строим запрос
        IQueryable<InformationArticleDetail> query = _repository
            .InformationArticlesDetails
            .Where(x => x.DateDeleted == null && x.InformationArticleId == informationArticleId);

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
    public async Task<GetInformationArticleDetailsResponse> RequestBuilder(List<InformationArticleDetail> request)
    {
        //Формируем модель ответа
        GetInformationArticleDetailsResponse response = new(true, null, new());

        //Проходим циклом по списку детальных частей
        foreach (var item in request)
        {
            //Получаем файлы
            BaseResponseList filesItem = await _getFilesInformationArticleDetails.Handler(item.Id);

            //Если данные пришли
            if (filesItem != null && filesItem.Items != null && filesItem.Items.Any())
            {
                //Формируем новый элемент
                GetInformationArticleDetailsResponseItem responseItem = new(item.Text, filesItem.Items.Select(x => x!.Id ?? 0).ToList());

                //Добавляем в ответ новый элемент
                response.Items!.Add(responseItem);
            }
        }

        //Возвращаем ответ
        return response;
    }
}