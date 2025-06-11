using Microsoft.Extensions.Caching.Distributed;

namespace SayehBanTools.Service.Redis.Interface;
/// <summary>
/// اینترفیس برای کش کردن
/// </summary>
public interface ICache
{
    /// <summary>
    /// دریافت اطلاعات از کش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    Task<T?> TryGetValueAsync<T>(string cacheKey);
    /// <summary>
    /// اضافه کردن اطلاعات به کش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task SetAsync<T>(string cacheKey, T value, DistributedCacheEntryOptions options);
    /// <summary>
    /// ریست کش
    /// </summary>
    /// <param name="cacheKeyPattern"></param>
    /// <param name="languageCode"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    Task ResetCacheAsync(string cacheKeyPattern, string languageCode, string identifier);
    /// <summary>
    /// ریست کش
    /// </summary>
    /// <param name="cacheKeyPattern"></param>
    /// <param name="languageCode"></param>
    /// <returns></returns>
    Task ResetCacheAsync(string cacheKeyPattern, string languageCode);
    /// <summary>
    /// ریست کش
    /// </summary>
    /// <param name="cacheKeyPattern"></param>
    /// <returns></returns>
    Task ResetCacheAsync(string cacheKeyPattern);
}

