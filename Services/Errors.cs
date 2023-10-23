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
}
