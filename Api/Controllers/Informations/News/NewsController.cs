using Api.Controllers.Base;
using Domain.Models.Informations.News.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Informations.News.AddNews;
using Services.Informations.News.GetNewsList;
using Services.Informations.NewsDetails.GetNewsDetails;

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
    private readonly IGetNewsDetails _getNewsDetails; //интерфейс получения детальных частей новости
    private readonly IAddNews _addNews; //интерфейс добавления новости

    /// <summary>
    /// Конструктор контроллера новостей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="getNews"></param>
    /// <param name="getNewsDetails"></param>
    /// <param name="addNews"></param>
    public NewsController(ILogger<NewsController> logger, IGetNewsList getNews, IGetNewsDetails getNewsDetails, IAddNews addNews)
        : base(logger)
    {
        _logger = logger;
        _getNews = getNews;
        _getNewsDetails = getNewsDetails;
        _addNews = addNews;
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

    /// <summary>
    /// Метод получения детальных частей новости
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("details")]
    public async Task<IActionResult> GetNewsDetails([FromQuery] long? id)
        => await GetAnswerAsync(async () =>
        {
            return await _getNewsDetails.Handler(id);
        });

    /// <summary>
    /// Метод добавления новости
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddNews([FromBody] AddNewsRequest? request)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _addNews.Handler(user, request);
        });
}