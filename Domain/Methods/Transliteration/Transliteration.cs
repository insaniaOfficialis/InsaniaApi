namespace Domain.Methods.Transliteration;

/// <summary>
/// Сервис транслитерации
/// </summary>
public class Transliteration
{
    /*Объявляем статические списки символов*/
    readonly string[] LatUp = { "A", "B", "V", "G", "D", "E", "Yo", "Zh", "Z", "I", "Y", "K", "L", "M", "N", "O", "P", "R", "S", "T", "U", "F", "Kh", "Ts", "Ch", "Sh",
        "Shch", "\"", "Y", "'", "E", "Yu", "Ya" }; //прописные латинские символы
    readonly string[] LatLow = { "a", "b", "v", "g", "d", "e", "yo", "zh", "z", "i", "y", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "f", "kh", "ts", "ch", "sh",
        "shch", "\"", "y", "'", "e", "yu", "ya" }; //строчные латинские символы
    readonly string[] RusUp = { "А", "Б", "В", "Г", "Д", "Е", "Ё", "Ж", "З", "И", "Й", "К", "Л", "М", "Н", "О", "П", "Р", "С", "Т", "У", "Ф", "Х", "Ц", "Ч", "Ш", "Щ",
        "Ъ", "Ы", "Ь", "Э", "Ю", "Я" }; //прописные кириллические символы
    readonly string[] RusLow = { "а", "б", "в", "г", "д", "е", "ё", "ж", "з", "и", "й", "к", "л", "м", "н", "о", "п", "р", "с", "т", "у", "ф", "х", "ц", "ч", "ш", "щ",
        "ъ", "ы", "ь", "э", "ю", "я" }; //строчные кириллические символы

    /// <summary>
    /// Конструктор сервиса транслитерации
    /// </summary>
    public Transliteration()
    {
        
    }

    /// <summary>
    /// Метод транслитерации из кириллицы в латиницу
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string Translit(string str)
    {
        /*Проходимся циклом 32 раза*/
        for (int i = 0; i <= 32; i++)
        {
            /*Меняем символы*/
            str = str.Replace(RusUp[i], LatUp[i]);
            str = str.Replace(RusLow[i], LatLow[i]);
        }

        /*Возвращаем результат*/
        return str;
    }
}
