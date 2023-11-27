using Data;
using Domain.Entities.General.File;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Microsoft.EntityFrameworkCore;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files.EditOrdinalNumberFile;

/// <summary>
/// Изменение порядкового номера файла
/// </summary>
public class EditOrdinalNumberFile : IEditOrdinalNumberFile
{
    /// <summary>
    /// Конструктор изменения порядкового номера файла
    /// </summary>
    /// <param name="repository"></param>
    public EditOrdinalNumberFile(ApplicationContext repository)
    {
        _repository = repository;
    }

    private FileEntity? _entity; //сущность файла
    
    private readonly ApplicationContext _repository; //репозиторий сущности

    /// <summary>
    /// Проверка входных данных
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="InnerException"></exception>
    public async Task<bool> Validator(string? user, long? ordinalNumber, long? id)
    {
        //Проверяем на пустоту текущего пользователя
        if (string.IsNullOrEmpty(user))
            throw new InnerException(Errors.EmptyCurrentUser);

        //Проверяем на отсутсвие id
        if (id == null)
            throw new InnerException(Errors.EmptyId);

        //Проверяем на отсутсвие порядкового номера
        if(ordinalNumber == null)
            throw new InnerException(Errors.EmptyOrdinalNumber);

        //Проверяем на корректность порядкового номера
        if (ordinalNumber < 0)
            throw new InnerException(Errors.IncorrectOrdinalNumber);

        //Получаем сущность
        _entity = await _repository.Files.Include(x => x.Type).FirstOrDefaultAsync(x => x.Id == id) 
            ?? throw new InnerException(Errors.NotExistsFile);

        //Проверяем наличие расширения в зависимости от типа
        switch(_entity.Type?.Alias)
        {
            case "Pol'zovatel'":
                {
                    if (!await _repository.FilesUsers.AnyAsync(x => x.FileId == id))
                        throw new InnerException(Errors.NotExistsFilesUsers);
                }
                break;
            case "Informatsionnaya_stat'ya":
                {
                    if (!await _repository.FilesInformationArticleDetails.AnyAsync(x => x.FileId == id))
                        throw new InnerException(Errors.NotExistsFileInformationArticleDetail);
                }
                break;
            case "Novost'":
                {
                    if (!await _repository.FilesNewsDetails.AnyAsync(x => x.FileId == id))
                        throw new InnerException(Errors.NotExistsFileNewsDetail);
                }
                break;
            default:
                throw new InnerException(Errors.NotExistsFileEntity);
        }

        //Возвращаем результат
        return true;
    }

    /// <summary>
    /// Обработка
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<BaseResponse> Handler(string? user, long? ordinalNumber, long? id)
    {
        try
        {
            //Проверяем на ошибки
            var resultValidator = await Validator(user, ordinalNumber, id);

            //Если успешно
            if (resultValidator)
            {
                //Получаем результат запроса
                var response = await Query(user, ordinalNumber, id);

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
    /// Запрос к базе
    /// </summary>
    /// <param name="user"></param>
    /// <param name="ordinalNumber"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<long?> Query(string? user, long? ordinalNumber, long? id)
    {
        //Открываем транзакцию
        using var transaction = _repository.Database.BeginTransaction();

        //Сохраняем данные в базу
        try
        {
            //Меняем порядковый номер, в зависимости от типа сущности
            switch (_entity!.Type!.Alias)
            {
                case "Pol'zovatel'":
                    {
                        FileUser entity = await _repository.FilesUsers.FirstAsync(x => x.FileId == id);
                        entity.SetUpdate(user);
                        _repository.FilesUsers.Update(entity);
                    }
                    break;
                case "Informatsionnaya_stat'ya":
                    {
                        FileInformationArticleDetail entity = await _repository
                            .FilesInformationArticleDetails
                            .FirstAsync(x => x.FileId == id);
                        entity.SetOrdinalNumber(ordinalNumber ?? 0);
                        entity.SetUpdate(user);
                        _repository.FilesInformationArticleDetails.Update(entity);
                    }
                    break;
                case "Novost'":
                    {
                        FileNewsDetail entity = await _repository
                            .FilesNewsDetails
                            .FirstAsync(x => x.FileId == id);
                        entity.SetOrdinalNumber(ordinalNumber ?? 0);
                        entity.SetUpdate(user);
                        _repository.FilesNewsDetails.Update(entity);
                    }
                    break;
            }
            
            //Сохраняем в базу
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