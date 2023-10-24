using Api.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Informations.News.GetNewsList;

namespace Api.Controllers.Informations.News;

/// <summary>
/// Контроллер новостей
/// </summary>
[Authorize]
[Route("api/v1/news")]
public class NewsController : BaseController
{
    private readonly ILogger<NewsController> _logger; //интерфейс логгирования
    private readonly IGetNewsList _getNews; //интерфейс получения списка новостей

    /// <summary>
    /// Конструктор контроллера новостей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="getNews"></param>
    public NewsController(ILogger<NewsController> logger, IGetNewsList getNews)
        : base(logger)
    {
        _logger = logger;
        _getNews = getNews;
    }

    /// <summary>
    /// Метод получения списка новостей
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("list")]
    public async Task<IActionResult> GetNewsList([FromQuery] string? search)
        => await GetAnswerAsync(async () =>
        {
            return await _getNews.Handler(search);
        });
}
