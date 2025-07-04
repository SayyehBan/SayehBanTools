﻿using Microsoft.Extensions.Caching.Distributed;
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

        try
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
        catch (Exception ex)
        {
            // ثبت خطا برای دیباگ (اختیاری)
            throw new InvalidOperationException($"Error retrieving cache for key '{key}': {ex.Message}", ex);
        }
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
    /// <summary>
    /// دریافت مقدار از کش به صورت لیست
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<List<T>?> TryGetListAsync<T>(string key)
    {
        return await TryGetValueAsync<List<T>>(key);
    }
    /// <summary>
    /// ثبت مقدار در کش به صورت لیست
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public async Task SetListAsync<T>(string key, List<T> value, object options)
    {
        await SetAsync(key, value, options);
    }
    /// <summary>
    /// ثبت مقدار در کش به صورت هش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task SetHashAsync<T>(string key, Dictionary<string, T> values, DistributedCacheEntryOptions options)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not configured.");

        var serializedDict = values.ToDictionary(
            kvp => kvp.Key,
            kvp => JsonSerializer.Serialize(kvp.Value));

        await _distributedCache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(serializedDict), options);
    }
    /// <summary>
    /// دریافت مقدار از کش به صورت هش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<T?> GetHashValueAsync<T>(string key, string field)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not configured.");

        var data = await _distributedCache.GetAsync(key);
        if (data == null) return default;

        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
        return dict != null && dict.ContainsKey(field)
            ? JsonSerializer.Deserialize<T>(dict[field])
            : default;
    }
    /// <summary>
    /// دریافت همه مقادیر به صورت هش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<Dictionary<string, T>> GetHashAllAsync<T>(string key)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not configured.");

        var data = await _distributedCache.GetAsync(key);
        if (data == null) return new Dictionary<string, T>();

        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
        return dict!.ToDictionary(
             kvp => kvp.Key,
             kvp => JsonSerializer.Deserialize<T>(kvp.Value)!);
    }
    /// <summary>
    /// حذف مقدار از کش به صورت هش
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DeleteHashFieldAsync(string key, string field)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not configured.");

        var data = await _distributedCache.GetAsync(key);
        if (data == null) return;

        var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(data);
        if (dict == null || !dict.ContainsKey(field)) return;

        dict.Remove(field);
        await _distributedCache.SetAsync(key, JsonSerializer.SerializeToUtf8Bytes(dict), new DistributedCacheEntryOptions());
    }
    /// <summary>
    /// بررسی وجود مقدار کلید در کش به صورت هش
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> KeyExistsAsync(string key)
    {
        if (_distributedCache == null)
            throw new InvalidOperationException("Distributed cache is not configured.");

        var data = await _distributedCache.GetAsync(key);
        return data != null;
    }
}