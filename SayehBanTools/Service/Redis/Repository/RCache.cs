using Microsoft.Extensions.Caching.Distributed;
using SayehBanTools.Service.Redis.Interface;
using SayehBanTools.Utilities.Caching;

namespace SayehBanTools.Service.Redis.Repository;
/// <summary>
/// ریاپزیتوری کش
/// </summary>
public class RCache : ICache
{
    private readonly CacheManager _cacheManager;
    /// <summary>
    /// Initializes a new instance of the <see cref="RCache"/> class.
    /// </summary>
    /// <param name="cache"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public RCache(IDistributedCache cache)
    {
        _cacheManager = new CacheManager(cache ?? throw new ArgumentNullException(nameof(cache)));
    }
    /// <summary>
    /// ریست کش برای هر زبان و شناسه
    /// </summary>
    /// <param name="cacheKeyPattern"></param>
    /// <param name="languageCode"></param>
    /// <param name="identifier"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ResetCacheAsync(string cacheKeyPattern, string languageCode, string identifier)
    {
        await _cacheManager.ResetCacheAsync(cacheKeyPattern, languageCode, identifier);
    }
    /// <summary>
    /// ثبت کش برای هر زبان و شناسه
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SetAsync<T>(string cacheKey, T value, DistributedCacheEntryOptions options)
    {
        await _cacheManager.SetAsync(cacheKey, value, options);
    }
    /// <summary>
    /// دریافت مقدار از کش برای هر زبان و شناسه
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="cacheKey"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<T?> TryGetValueAsync<T>(string cacheKey)
    {
        return await _cacheManager.TryGetValueAsync<T>(cacheKey);
    }
}
