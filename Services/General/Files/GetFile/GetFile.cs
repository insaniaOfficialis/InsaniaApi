using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.General.Files.Response;
using Microsoft.EntityFrameworkCore;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFile;

/// <summary>
/// Сервис получения файла
/// </summary>
public class GetFile : IGetFile
{
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения списка файлов детальной части информационной статьи
    /// </summary>
    /// <param name="repository"></param>
    public GetFile(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(long? id, long? entityId)
    {
        //Проверяем на пустой запрос
        if (id == null)
            throw new InnerException(Errors.EmptyRequest);
        
        //Проверяем на пустой запрос
        if (entityId == null)
            throw new InnerException(Errors.EmptyEntityId);

        //Проверяем на не существующую детальную часть информационной статьи
        if (!await _repository.Files.AnyAsync(x => x.Id == id))
            throw new InnerException(Errors.NotExistsFile);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="id"></param>
    /// <param name="entityId"></param>
    /// <returns></returns>
    public async Task<GetFileReponse> Handler(long? id, long? entityId)
    {
        try
        {
            //Проверяем входные данные
            var validate = await Validator(id, entityId);

            //Если проверка успешная
            if (validate)
            {
                //Получаем результат запроса
                var response = await Query(id);

                //Формируем ответ
                string path = string.Format("{0}\\{1}\\{2}", response!.Type!.Path, entityId, response.Name);
                string name = response!.Name;
                string contentType = ContentTypes.DictionaryContentTypes.First(x => x.Key == response.Extention).Value;
                return new GetFileReponse(true, path, name, contentType);
            }
            //Иначе
            else
            {
                return new GetFileReponse(false, new BaseError(400, Errors.NotValidate));
            }
        }
        //Обрабатываем внутренние исключения
        catch (InnerException ex)
        {
            return new GetFileReponse(false, new BaseError(400, ex.Message));
        }
        //Обрабатываем системные исключения
        catch (Exception ex)
        {
            return new GetFileReponse(false, new BaseError(500, ex.Message));
        }
    }

    /// <summary>
    /// Метод формирования запроса
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<FileEntity> Query(long? id)
    {
        //Строим запрос
        IQueryable<FileEntity?> query = _repository
            .Files
            .Include(x => x.Type)
            .Where(x => x.Id == id);

        //Получаем данные с базы
        var entity = await query.FirstAsync();

        //Формируем ответ
        return entity!;
    }
}