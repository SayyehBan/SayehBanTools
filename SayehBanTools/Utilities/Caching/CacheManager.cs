using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace SayehBanTools.Utilities.Caching;
/// <summary>
/// مدیریت کش
/// </summary>
public class CacheManager
{
    private readonly IMemoryCache? _memoryCache;
    private readonly IDistributedCache? _distributedCache;
    /// <summary>
    /// سازنده کلاس
    /// </summary>
    /// <param name="memoryCache"></param>
    public CacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _distributedCache = null;
    }
    /// <summary>
    /// سازنده کلاس
    /// </summary>
    /// <param name="distributedCache"></param>
    public CacheManager(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
        _memoryCache = null;
    }
    /// <summary>
    /// دریافت مقدار کش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<T?> TryGetValueAsync<T>(string key)
    {
        if (_memoryCache != null)
        {
            return _memoryCache.TryGetValue(key, out T? value) ? value : default;
        }
        else if (_distributedCache != null)
        {
            var cachedData = await _distributedCache.GetStringAsync(key);
            return cachedData != null ? JsonSerializer.Deserialize<T>(cachedData) : default;
        }
        throw new InvalidOperationException("No cache provider configured.");
    }
    /// <summary>
    /// ثبت مقدار کش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SetAsync<T>(string key, T value, object options)
    {
        if (_memoryCache != null && options is MemoryCacheEntryOptions memOptions)
        {
            _memoryCache.Set(key, value, memOptions);
        }
        else if (_distributedCache != null && options is DistributedCacheEntryOptions distOptions)
        {
            var serializedData = JsonSerializer.Serialize(value);
            await _distributedCache.SetStringAsync(key, serializedData, distOptions);
        }
        else
        {
            throw new InvalidOperationException("Invalid cache options or no cache provider configured.");
        }
    }
    /// <summary>
    /// ریست کش
    /// </summary>
    /// <param name="prefix"></param>
    /// <param name="keyParts"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task ResetCacheAsync(string prefix, params string?[] keyParts)
    {
        if (keyParts == null || keyParts.All(part => string.IsNullOrEmpty(part)))
        {
            throw new InvalidOperationException("برای ریست کش، حداقل یک پارامتر باید مشخص شود.");
        }
        var cacheKey = $"{prefix}{string.Join("_", keyParts.Where(p => !string.IsNullOrEmpty(p)))}";
        if (_memoryCache != null)
        {
            _memoryCache.Remove(cacheKey);
        }
        else if (_distributedCache != null)
        {
            await _distributedCache.RemoveAsync(cacheKey);
        }
    }
}
