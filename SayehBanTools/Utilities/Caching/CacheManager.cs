using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace SayehBanTools.Utilities.Caching;

/// <summary>
/// مدیریت کش برای ذخیره و بازیابی داده‌ها در حافظه یا کش توزیع‌شده
/// </summary>
public class CacheManager
{
    private readonly IMemoryCache? _memoryCache;
    private readonly IDistributedCache? _distributedCache;

    /// <summary>
    /// سازنده کلاس برای استفاده از کش حافظه
    /// </summary>
    /// <param name="memoryCache">نمونه کش حافظه</param>
    /// <exception cref="ArgumentNullException">در صورت null بودن memoryCache</exception>
    public CacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _distributedCache = null;
    }

    /// <summary>
    /// سازنده کلاس برای استفاده از کش توزیع‌شده
    /// </summary>
    /// <param name="distributedCache">نمونه کش توزیع‌شده</param>
    /// <exception cref="ArgumentNullException">در صورت null بودن distributedCache</exception>
    public CacheManager(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        _memoryCache = null;
    }

    /// <summary>
    /// دریافت مقدار از کش
    /// </summary>
    /// <typeparam name="T">نوع داده کش</typeparam>
    /// <param name="key">کلید کش</param>
    /// <returns>مقدار کش‌شده یا null در صورت عدم وجود</returns>
    /// <exception cref="ArgumentNullException">در صورت null یا خالی بودن key</exception>
    /// <exception cref="InvalidOperationException">در صورت عدم پیکربندی ارائه‌دهنده کش</exception>
    public async Task<T?> TryGetValueAsync<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

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
    /// ثبت مقدار در کش
    /// </summary>
    /// <typeparam name="T">نوع داده کش</typeparam>
    /// <param name="key">کلید کش</param>
    /// <param name="value">مقدار برای کش کردن</param>
    /// <param name="options">تنظیمات کش (MemoryCacheEntryOptions یا DistributedCacheEntryOptions)</param>
    /// <returns>وظیفه ناهمگام</returns>
    /// <exception cref="ArgumentNullException">در صورت null یا خالی بودن key</exception>
    /// <exception cref="InvalidOperationException">در صورت نادرست بودن تنظیمات یا عدم پیکربندی ارائه‌دهنده کش</exception>
    public async Task SetAsync<T>(string key, T value, object options)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));

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
    /// حذف مقدار از کش
    /// </summary>
    /// <param name="prefix">پیشوند کلید کش</param>
    /// <param name="keyParts">بخش‌های اضافی کلید کش (اختیاری)</param>
    /// <returns>وظیفه ناهمگام</returns>
    /// <exception cref="ArgumentNullException">در صورت null یا خالی بودن prefix</exception>
    /// <exception cref="InvalidOperationException">در صورت عدم پیکربندی ارائه‌دهنده کش</exception>
    public async Task ResetCacheAsync(string prefix, params string?[] keyParts)
    {
        if (string.IsNullOrEmpty(prefix))
            throw new ArgumentNullException(nameof(prefix));

        var cacheKey = prefix;
        if (keyParts != null && keyParts.Any(p => !string.IsNullOrEmpty(p)))
        {
            cacheKey += string.Join("_", keyParts.Where(p => !string.IsNullOrEmpty(p)));
        }

        if (_memoryCache != null)
        {
            _memoryCache.Remove(cacheKey);
        }
        else if (_distributedCache != null)
        {
            await _distributedCache.RemoveAsync(cacheKey);
        }
        else
        {
            throw new InvalidOperationException("No cache provider configured.");
        }
    }
}