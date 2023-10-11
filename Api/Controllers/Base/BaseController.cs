using Domain.Models.Base;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Base;

/// <summary>
/// Базовый контроллер
/// </summary>
public class BaseController : Controller
{
    private readonly ILogger<BaseController> _logger; //интерфейс для записи логов

    /// <summary>
    /// Конструктор базового контроллера
    /// </summary>
    /// <param name="logger"></param>
    public BaseController(ILogger<BaseController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Построение станадртного ответа
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="action"></param>
    /// <returns></returns>
    protected async Task<IActionResult> GetAnswerAsync<T>(Func<Task<T>> action)
    {
        try
        {
            T result = await action();

            if (result is BaseResponse baseResponse)
            {
                if (baseResponse.Success)
                {
                    _logger.LogInformation("{0}. Успешно", action.Method.Name);
                    return Ok(result);
                }
                else
                {
                    if (baseResponse != null && baseResponse.Error != null)
                    {
                        if (baseResponse.Error.Code != 500)
                        {
                            _logger.LogError("{0}. Обработанная ошибка: {1}", action.Method.Name, baseResponse.Error);
                            return StatusCode(baseResponse.Error.Code ?? 400, result);
                        }
                        else
                        {
                            _logger.LogError("{0}. Необработанная ошибка:  {1}", action.Method.Name, baseResponse.Error);
                            return StatusCode(baseResponse.Error.Code ?? 500, result);
                        }
                    }
                    else
                    {
                        _logger.LogError("{0}. Непредвиденная ошибка", action.Method.Name);
                        BaseResponse response = new(false, new BaseError(500, "Непредвиденная ошибка"));
                        return StatusCode(500, response);
                    }
                }
            }
            else
            {
                _logger.LogError("{0}. Нестандартная модель ответа", action.Method.Name);
                BaseResponse response = new(false, new BaseError(500, "Нестандартная модель ответа"));
                return StatusCode(500, response);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("{0}. Необработанная ошибка:  {1}", action.Method.Name, ex);
            return StatusCode(500, new BaseResponse(false, new BaseError(500, ex.Message)));

        }
    }
}
