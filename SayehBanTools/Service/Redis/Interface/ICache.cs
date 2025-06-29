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

    /// <summary>
    /// اضافه کردن یک آیتم به لیست کش‌شده
    /// </summary>
    Task AddItemToListAsync<T>(string cacheKey, T item, DistributedCacheEntryOptions options);

    /// <summary>
    /// حذف یک آیتم از لیست کش‌شده
    /// </summary>
    Task RemoveItemFromListAsync<T>(string cacheKey, Func<T, bool> predicate, DistributedCacheEntryOptions options);

    /// <summary>
    /// به‌روزرسانی یک آیتم در لیست کش‌شده
    /// </summary>
    Task UpdateItemInListAsync<T>(string cacheKey, Func<T, bool> predicate, T newItem, DistributedCacheEntryOptions options);
    /// <summary>
    /// جستجو در لیست کش‌شده با استفاده از شرط
    /// </summary>
    Task<IEnumerable<T>> SearchInListAsync<T>(string cacheKey, Func<T, bool> predicate);

    /// <summary>
    /// جستجوی پیشرفته در لیست کش‌شده با چندین شرط
    /// </summary>
    Task<IEnumerable<T>> AdvancedSearchInListAsync<T>(string cacheKey, List<Func<T, bool>> predicates);

    /// <summary>
    /// جستجو و صفحه‌بندی در لیست کش‌شده
    /// </summary>
    Task<(IEnumerable<T> Results, int TotalCount)> SearchWithPaginationAsync<T>(
        string cacheKey,
        Func<T, bool> predicate,
        int pageNumber,
        int pageSize);
    /// <summary>
    /// ثبت به صورت هش همزمانی
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="values"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    Task SetHashAsync<T>(string key, Dictionary<string, T> values, DistributedCacheEntryOptions options);
    /// <summary>
    /// دریافت مقدار یک فیلد از یک هش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<T?> GetHashValueAsync<T>(string key, string field);
    /// <summary>
    /// دریافت همه مقادیر یک هش
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<Dictionary<string, T>> GetHashAllAsync<T>(string key);
    /// <summary>
    /// حذف یک فیلد از یک هش
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task DeleteHashFieldAsync(string key, string field);
    /// <summary>
    /// بررسی وجود یک کلید در کش
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> KeyExistsAsync(string key);
}

