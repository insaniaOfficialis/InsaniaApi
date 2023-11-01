using AutoMapper;
using Data;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.GetFilesUser;

/// <summary>
/// Сервис получения списка файлов пользователя
/// </summary>
public class GetFilesUser : IGetFilesUser
{
    private readonly IMapper _mapper; //маппер моделей
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Конструктор сервиса получения списка файлов пользователя
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="repository"></param>
    public GetFilesUser(IMapper mapper, ApplicationContext repository)
    {
        _mapper = mapper;
        _repository = repository;
    }

    /// <summary>
    /// Метод проверки
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(long? id)
    {
        //Проверяем на пустой запрос
        if (id == null)
            throw new InnerException(Errors.EmptyRequest);

        //Проверяем на не существующего пользователя
        if (!await _repository.FilesUsers.AnyAsync(x => x.UserId == id))
            throw new InnerException(Errors.NotExistsFilesUsers);

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Метод обработки
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<BaseResponseList> Handler(long? id)
    {
        try
        {
            //Проверяем входные данные
            var validate = await Validator(id);

            //Если проверка успешная
            if (validate)
            {
                //Получаем результат запроса
                var response = await Query(id);

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
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<List<FileEntity?>> Query(long? id)
    {
        //Строим запрос
        IQueryable<FileEntity?> query = _repository
            .FilesUsers
            .Include(x => x.File)
            .Where(x => x.DateDeleted == null && x.UserId == id)
            .Select(x => x.File);

        //Получаем данные с базы
        var entities = await query.ToListAsync();

        //Формируем ответ
        return entities;
    }
}