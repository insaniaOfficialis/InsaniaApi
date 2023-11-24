using Api.Controllers.Base;
using Domain.Models.Base;
using Domain.Models.Informations.News.Request;
using Domain.Models.Informations.NewsDetails.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Informations.News.AddNews;
using Services.Informations.News.EditNews;
using Services.Informations.News.GetNewsFullList;
using Services.Informations.News.GetNewsList;
using Services.Informations.News.GetNewsTable;
using Services.Informations.News.RemovalNews;
using Services.Informations.NewsDetails.AddNewsDetail;
using Services.Informations.NewsDetails.EditNewsDetail;
using Services.Informations.NewsDetails.GetNewsDetails;
using Services.Informations.NewsDetails.RemovalNewsDetail;

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
    private readonly IAddNewsDetail _addNewsDetail; //интерфейс добавления детальной части новости
    private readonly IGetNewsFullList _getNewsFullList; //интерфейс получения полного списка новостей
    private readonly IGetNewsTable _getNewsTable; //интерфейс получения списка новостей для таблицы
    private readonly IEditNews _editNews; //интерфейс редактирования новости
    private readonly IEditNewsDetail _editNewsDetail; //интерфейс редактирования детальной части новости
    private readonly IRemovalNews _removalNews; //интерфейс удаления/восстановления новости
    private readonly IRemovalNewsDetail _removalNewsDetail; //интерфейс удаления/восстановления детальной части новости

    /// <summary>
    /// Конструктор контроллера новостей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="getNews"></param>
    /// <param name="getNewsDetails"></param>
    /// <param name="addNews"></param>
    /// <param name="addNewsDetail"></param>
    /// <param name="getNewsFullList"></param>
    /// <param name="getNewsTable"></param>
    /// <param name="editNews"></param>
    /// <param name="editNewsDetail"></param>
    /// <param name="removalNews"></param>
    /// <param name="removalNewsDetail"></param>
    public NewsController(ILogger<NewsController> logger, IGetNewsList getNews, IGetNewsDetails getNewsDetails, IAddNews addNews,
        IAddNewsDetail addNewsDetail, IGetNewsFullList getNewsFullList, IGetNewsTable getNewsTable, IEditNews editNews,
        IEditNewsDetail editNewsDetail, IRemovalNews removalNews, IRemovalNewsDetail removalNewsDetail) : base(logger)
    {
        _logger = logger;
        _getNews = getNews;
        _getNewsDetails = getNewsDetails;
        _addNews = addNews;
        _addNewsDetail = addNewsDetail;
        _getNewsFullList = getNewsFullList;
        _getNewsTable = getNewsTable;
        _editNews = editNews;
        _editNewsDetail = editNewsDetail;
        _removalNews = removalNews;
        _removalNewsDetail = removalNewsDetail;
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

    /// <summary>
    /// Метод добавления детальной части новости
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("details")]
    public async Task<IActionResult> AddNewsDetail([FromBody] AddNewsDetailRequest? request)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _addNewsDetail.Handler(user, request);
        });

    /// <summary>
    /// Метод получения полного списка новостей
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("fullList")]
    public async Task<IActionResult> GetNewsFullList([FromQuery] string? search)
        => await GetAnswerAsync(async () =>
        {
            return await _getNewsFullList.Handler(search);
        });

    /// <summary>
    /// Метод получения списка новостей для таблицы
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("table")]
    public async Task<IActionResult> GetNewsTable([FromQuery] string? search, [FromQuery] int? skip, [FromQuery] int? take,
        [FromQuery] List<BaseSortRequest?>? sort, [FromQuery] bool? isDeleted)
        => await GetAnswerAsync(async () =>
        {
            return await _getNewsTable.Handler(search, skip, take, sort, isDeleted);
        });

    /// <summary>
    /// Метод редактирования новости
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> EditNews([FromBody] AddNewsRequest? request, [FromRoute] long id)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _editNews.Handler(user, request, id);
        });

    /// <summary>
    /// Метод редактирования детальной части новости
    /// </summary>
    /// <param name="request"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("details/{id}")]
    public async Task<IActionResult> EditNewsDetail([FromBody] AddNewsDetailRequest? request, [FromRoute] long id)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _editNewsDetail.Handler(user, request, id);
        });

    /// <summary>
    /// Метод удаления/восстановления детальной части новости
    /// </summary>
    /// <param name="isDeleted"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> RemovalNews([FromQuery] bool? isDeleted, [FromRoute] long id)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _removalNews.Handler(user, id, isDeleted);
        });

    /// <summary>
    /// Метод удаления/восстановления детальной части новости
    /// </summary>
    /// <param name="isDeleted"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete]
    [Route("details/{id}")]
    public async Task<IActionResult> RemovalNewsDetail([FromQuery] bool? isDeleted, [FromRoute] long id)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _removalNewsDetail.Handler(user, id, isDeleted);
        });
}