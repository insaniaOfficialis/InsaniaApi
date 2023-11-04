using Data;
using Domain.Entities.General.File;
using Domain.Models.Base;
using Domain.Models.Exclusion;
using Domain.Models.General.Files.Request;
using Microsoft.EntityFrameworkCore;
using FileEntity = Domain.Entities.General.File.File;

namespace Services.General.Files;

/// <summary>
/// Сервис файлов
/// </summary>
public class Files : IFiles
{
    private readonly ApplicationContext _repository; //репозиторий сущности
    private readonly List<string> _allowedExtensions = new() { "pdf", "png", "jpeg", "jpg", "bmp" }; //доступные расширения

    /// <summary>
    /// Конструктор сервиса файлов
    /// </summary>
    /// <param name="repository"></param>
    public Files(ApplicationContext repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Метод добавления файлов
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<BaseResponse> AddFile(AddFileRequest? request)
    {
        try
        {
            long id; //id файла

            //Проверяем корректность данных
            if (request == null)
                throw new InnerException("Пустой запрос");

            if (request.Id == null)
                throw new InnerException("Не указан id сущности");

            if (string.IsNullOrEmpty(request.Type))
                throw new InnerException("Не указан тип файла");

            if (string.IsNullOrEmpty(request.Name))
                throw new InnerException("Не указано наименование файла");

            if (request.Stream == null)
                throw new InnerException("Не указан файл");

            var fileType = _repository.FileTypes.Where(x => x.Alias == request.Type).FirstOrDefault()
                ?? throw new InnerException("Не найден указанный тип файла");

            //Начинаем транзакцию
            using var transaction = _repository.Database.BeginTransaction();

            try
            {
                //Объявляем новый файл и записываем его в базу
                FileEntity file = new(null, false, request.Name, fileType.Id);

                if (file.Extention == null || !_allowedExtensions.Contains(file.Extention))
                    throw new InnerException("Недопустимое расширение");

                _repository.Files.Add(file);

                switch (fileType.Alias)
                {
                    case "Pol'zovatel'":
                        {
                            var user = await _repository
                                .Users
                                .Where(x => x.Id == request.Id)
                                .FirstOrDefaultAsync()
                                ?? throw new InnerException("Не найден пользователь");
                            FileUser fileUser = new(null, file, user);
                            await _repository.FilesUsers.AddAsync(fileUser);
                        }
                        break;
                    case "Informatsionnaya_stat'ya":
                        {
                            //Получаем максимальный порядковый номер имеющихся записей
                            long ordinalNumber = 0;
                            if ((request.OrdinalNumber ?? 0) <= 0)
                                ordinalNumber = (await _repository.FilesInformationArticleDetails
                                    .MaxAsync(x => (long?)x.OrdinalNumber) ?? 0) + 1;
                            
                            //Получаем связи с другими сущностями
                            var informationArticleDetail = await _repository
                                .InformationArticlesDetails
                                .Where(x => x.Id == request.Id)
                                .FirstOrDefaultAsync()
                                ?? throw new InnerException("Не найдена детальная часть информационной статьи");

                            //Формируем экземпляр сущности и сохраняем в базу
                            FileInformationArticleDetail fileInformationArticleDetail = new(null, file, informationArticleDetail,
                                request.OrdinalNumber ?? ordinalNumber);
                            await _repository.FilesInformationArticleDetails.AddAsync(fileInformationArticleDetail);
                        }
                        break;
                    case "Novost'":
                        {
                            //Получаем максимальный порядковый номер имеющихся записей
                            long ordinalNumber = 0;
                            if ((request.OrdinalNumber ?? 0) <= 0)
                                ordinalNumber = (await _repository.FilesNewsDetails.MaxAsync(x => (long?)x.OrdinalNumber) ?? 0) + 1;

                            //Получаем связи с другими сущностями
                            var newsDetail = await _repository
                                .NewsDetails
                                .Where(x => x.Id == request.Id)
                                .FirstOrDefaultAsync()
                                ?? throw new InnerException("Не найдена детальная часть новости");

                            //Формируем экземпляр сущности и сохраняем в базу
                            FileNewsDetail fileNewsDetail = new(null, file, newsDetail, request.OrdinalNumber ?? ordinalNumber);
                            await _repository.FilesNewsDetails.AddAsync(fileNewsDetail);
                        }
                        break;
                }

                await _repository.SaveChangesAsync();

                //Проверяем наличие пути сохранения файла, и если нет, сохраняем
                var path = Path.Combine(fileType.Path, request.Id.ToString()!);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //Проверяем наличие такого файла
                var pathFile = Path.Combine(path, request.Name);

                if (Directory.Exists(pathFile))
                    throw new InnerException("Файл с таким наименованием, типом и сущностью уже существует");

                //Записываем файл
                await using var fs = new FileStream(pathFile, FileMode.CreateNew);

                request.Stream.Position = 0;
                await request.Stream.CopyToAsync(fs);

                request.Stream.Position = 0;

                //Фиксируем транзакцию
                transaction.Commit();

                //Записывае id для вывода
                id = file.Id;
            }
            //Обрабатываем внутренние исключения
            catch (InnerException ex)
            {
                transaction.Rollback();
                throw new InnerException(ex.Message);
            }
            //Обрабатываем системные исключения
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message, ex);
            }

            return new BaseResponse(true, id);
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
}