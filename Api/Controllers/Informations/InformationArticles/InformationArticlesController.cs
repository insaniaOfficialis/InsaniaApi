using Api.Controllers.Base;
using Domain.Models.Informations.InformationArticles.Request;
using Domain.Models.Informations.InformationArticlesDetails.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Informations.InformationArticles.AddInformationArticle;
using Services.Informations.InformationArticlesDetails.AddInformationArticleDetail;

namespace Api.Controllers.Informations.InformationArticles;

/// <summary>
/// Контроллер информационных статей
/// </summary>
[Authorize]
[Route("api/v1/informationArticles")]
public class InformationArticlesController : BaseController
{
    private readonly ILogger<InformationArticlesController> _logger; //интерфейс логгирования
    private readonly IAddInformationArticle _addInformationArticle; //интерфейс добавления информационных статей
    private readonly IAddInformationArticleDetail _addInformationArticleDetail; //интерфейс добавления детальных часте информационных статей

    /// <summary>
    /// Конструктор контроллера информационных статей
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="addInformationArticle"></param>
    /// <param name="addInformationArticleDetail"></param>
    public InformationArticlesController(ILogger<InformationArticlesController> logger,
        IAddInformationArticle addInformationArticle, IAddInformationArticleDetail addInformationArticleDetail)
        : base(logger)
    {
        _logger = logger;
        _addInformationArticle = addInformationArticle;
        _addInformationArticleDetail = addInformationArticleDetail;
    }

    /// <summary>
    /// Метод добавления информационной статьи
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddInformationArticle([FromBody] AddInformationArticleRequest? request)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _addInformationArticle.Handler(user, request);
        });

    /// <summary>
    /// Метод добавления детальной части информационной статьи
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("detail")]
    public async Task<IActionResult> AddInformationArticleDetail([FromBody] AddInformationArticleDetailRequest? request)
        => await GetAnswerAsync(async () =>
        {
            string? user = User?.Identity?.Name;
            return await _addInformationArticleDetail.Handler(user, request);
        });
}