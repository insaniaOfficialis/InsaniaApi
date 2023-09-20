using Domain.Models.Base;
using Domain.Models.Geography.Countries.Request;
using Domain.Models.Geography.Countries.Response;

namespace Services.Geography.Countries;

/// <summary>
/// Интерфейс стран
/// </summary>
public interface ICountries
{
    /// <summary>
    /// Метод получения списка стран
    /// </summary>
    /// <returns></returns>
    Task<BaseResponseList> GetCountries();

    /// <summary>
    /// Метод получения списка стран с полной информацией
    /// </summary>
    /// <param name="search"></param>
    /// <param name="skip"></param>
    /// <param name="take"></param>
    /// <param name="sort"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    Task<CountriesResponseList> GetCountriesFullInformation(string? search, int? skip, int? take, List<BaseSortRequest?>? sort,
        bool isDeleted);

    /// <summary>
    /// Метод добавления страны
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Task<BaseResponse> AddCountry(AddCountryRequest? request, string? user);

    /// <summary>
    /// Метод обновления страны
    /// </summary>
    /// <param name="request"></param>
    /// <param name="user"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<BaseResponse> UpdateCountry(AddCountryRequest? request, string? user, long? id);
}
