using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFilesInformationArticleDetails;

/// <summary>
/// Сервис получения списка файлов детальной части информационной статьи
/// </summary>
public class GetFilesInformationArticleDetails : IGetFilesInformationArticleDetails
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения списка файлов детальной части информационной статьи
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    public GetFilesInformationArticleDetails(IMapper mapper, ApplicationContext repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="informationArticleDetailId"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(long? informationArticleDetailId)
    {
        //Проверяем на пустой запрос
        if (informationArticleDetailId == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на не существующую детальную часть информационной статьи
        if (!await _repository.FilesInformationArticleDetails.AnyAsync(x => x.InformationArticleDetailId == informationArticleDetailId))
            throw new InnerException(Errors.NotExistsInformationArticleDetail);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="informationArticleDetailId"></param>
    /// <returns></returns>
    public async Task<BaseResponseList> Handler(long? informationArticleDetailId)
    {
        try
        {
            //Проверяем входные данные
            var validate = await Validator(informationArticleDetailId);

            //Если проверка успешная
            if (validate)
            {
                //Получаем результат запроса
                var response = await Query(informationArticleDetailId);

                //Преобразовываем модели
                var entities = response.Select(_mapper.Map<BaseResponseListItem>).ToList();

                //Формируем ответ
                return new BaseResponseList(true, null, entities!);
            }
            //Иначе
            else
            {
                return new BaseResponseList(false, new BaseError(400, Errors.NotValidate));
            }
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new BaseResponseList(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new BaseResponseList(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="informationArticleDetailId"></param>
    /// <returns></returns>
    public async Task<List<FileEntity?>> Query(long? informationArticleDetailId)
    {
        //Строим запрос
        IQueryable<FileEntity?> query = _repository
            .FilesInformationArticleDetails
            .Include(x => x.File)
            .Where(x => x.DateDeleted == null && x.InformationArticleDetail.Id == informationArticleDetailId)
            .Select(x => x.File);

        //Получаем данные с базы
        var entities = await query.ToListAsync();

        //Формируем ответ
        return entities;
    }
}