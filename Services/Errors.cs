namespace Services;

/// <summary>
/// Класс ошибок
/// </summary>
public static class Errors
{
    //ОБЩИЕ
    public const string EmptyRequest = "Пустой запрос";
    public const string NotValidate = "Проверки не пройдены";
    public const string EmptyCurrentUser = "Не определён текущий пользователь";
    public const string ErrorConvertModel = "Ошибка преобразования модели запроса в модель ответа";
    public const string EmptyId = "Пустая ссылка на запись";
    public const string EmptyOrdinalNumber = "Пустой порядковый номер";
    public const string IncorrectOrdinalNumber = "Некорректный порядковый номер";
    public const string BusyColor = "Данный цвет уже занят";
    public const string TryAgain = "Попробуйте снова";

    //ИНФОРМАЦИОННЫЕ СТАТЬИ
    public const string EmptyTitle = "Пустой заголовок";
    public const string ExistingInformationArticle = "Данная информационна статья уже существует";
    public const string NotExistsInformationArticle = "Не существующая информационная статья";

    //ДЕТАЛЬНЫЕ ЧАСТИ ИНФОРМАЦИОННЫХ СТАТЕЙ
    public const string EmptyText = "Пустой текст";
    public const string EmptyInformationArticleId = "Пустая ссылка на информационную статью";
    public const string NotExistsInformationArticleDetail = "Не существующая детальная часть информационной статьи";

    //ФАЙЛЫ
    public const string NotExistsFile = "Не существует файл";
    public const string EmptyEntityId = "Пустой идентификатор сущности файла";
    public const string NotExistsFilesUsers = "Не существует файлы пользователей";
    public const string NotExistsFileInformationArticleDetail = "Не существует файл информационной статьи";
    public const string NotExistsFileNewsDetail = "Не существует файл новости";
    public const string NotExistsFileEntity = "Не существует файл сущности";

    //НОВОСТИ
    public const string NotExistsNews = "Не существующая новость";
    public const string ExistingNews = "Новость с таким заголовком уже существует";
    public const string EmptyIntroduction = "Пустое вступление";

    //ДЕТАЛЬНЫЕ ЧАТИ НОВОСТЕЙ
    public const string EmptyNewsId = "Пустая ссылка на новость";
    public const string NotExistsNewsDetail = "Не существующая детальная часть новости";

    //ТИПЫ НОВСТЕЙ
    public const string EmptyTypeNews = "Пустой тип новсти";
    public const string NotExistsTypeNews = "Не существущий тип новости";

    //ПОЛЬЗОВАТЕЛИ
}