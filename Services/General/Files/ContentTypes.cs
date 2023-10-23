namespace Services.General.Files;

public static class ContentTypes
{
    public static readonly Dictionary<string, string> DictionaryContentTypes = new()
    {
        { "gif", "image/gif" },
        { "jpeg", "image/jpeg" },
        { "jpg", "image/jpeg" },
        { "png", "image/png" },
        { "tiff", "image/tiff" },
        { "webp", "image/webp" }
    };
}
