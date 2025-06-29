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
    /// ریست کش برای هر زبان و شناسه
    /// </summary>
    /// <param name="cacheKeyPattern"></param>
    /// <param name="languageCode"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ResetCacheAsync(string cacheKeyPattern, string languageCode)
    {
        await _cacheManager.ResetCacheAsync(cacheKeyPattern, languageCode);
    }
    /// <summary>
    /// ریست کش برای هر زبان و شناسه
    /// </summary>
    /// <param name="cacheKeyPattern"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task ResetCacheAsync(string cacheKeyPattern)
    {
        await _cacheManager.ResetCacheAsync(cacheKeyPattern);
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
    /// <summary>
    /// اضافه کردن یک آیتم به لیست کش‌شده
    /// </summary>
    public async Task AddItemToListAsync<T>(string cacheKey, T item, DistributedCacheEntryOptions options)
    {
        var currentList = await _cacheManager.TryGetValueAsync<List<T>>(cacheKey) ?? new List<T>();
        currentList.Add(item);
        await _cacheManager.SetAsync(cacheKey, currentList, options);
    }
    /// <summary>
    /// حذف یک آیتم از لیست کش‌شده
    /// </summary>
    public async Task RemoveItemFromListAsync<T>(string cacheKey, Func<T, bool> predicate, DistributedCacheEntryOptions options)
    {
        var currentList = await _cacheManager.TryGetValueAsync<List<T>>(cacheKey);
        if (currentList == null) return;

        var itemToRemove = currentList.FirstOrDefault(predicate);
        if (itemToRemove != null)
        {
            currentList.Remove(itemToRemove);
            await _cacheManager.SetAsync(cacheKey, currentList, options);
        }
    }
    /// <summary>
    /// به‌روزرسانی یک آیتم در لیست کش‌شده
    /// </summary>
    public async Task UpdateItemInListAsync<T>(string cacheKey, Func<T, bool> predicate, T newItem, DistributedCacheEntryOptions options)
    {
        var currentList = await _cacheManager.TryGetValueAsync<List<T>>(cacheKey);
        if (currentList == null) return;

        var index = currentList.FindIndex(predicate.Invoke);
        if (index >= 0)
        {
            currentList[index] = newItem;
            await _cacheManager.SetAsync(cacheKey, currentList, options);
        }
    }
}
/*
 طریقه استفاده از دستورات
public class RCategoriesNameTranslations : ICategoriesNameTranslations
{
    // متدهای موجود...
    
    public async Task AddCategoryNameAsync(CategoriesNameGet newItem, string languageCode)
    {
        await ValidateInputAsync(languageCode);
        var cacheKey = $"{StaticsValue.CategoriesNameGet}_{languageCode}";
        
        await _lock.WaitAsync();
        try
        {
            // اضافه به SQL
            await _sqlCategoriesNameTranslations.AddCategoryNameAsync(newItem);
            
            // اضافه به Elasticsearch
            var indexName = $"{StaticsValue.CategoriesNameGet.ToLower()}_{languageCode}";
            await _ieCategoriesNameTranslations.CategoriesNameGetIndexAsync(indexName, newItem);
            
            // اضافه به Redis
            await _cache.AddItemToListAsync(cacheKey, newItem, _cacheOptions);
        }
        finally
        {
            _lock.Release();
        }
    }
    
    public async Task RemoveCategoryNameAsync(int categoryNameId, string languageCode)
    {
        await ValidateInputAsync(languageCode);
        var cacheKey = $"{StaticsValue.CategoriesNameGet}_{languageCode}";
        
        await _lock.WaitAsync();
        try
        {
            // حذف از SQL
            await _sqlCategoriesNameTranslations.DeleteCategoryNameAsync(categoryNameId);
            
            // حذف از Elasticsearch
            var indexName = $"{StaticsValue.CategoriesNameGet.ToLower()}_{languageCode}";
            await _ieCategoriesNameTranslations.CategoriesNameDeleteAsync(categoryNameId, indexName);
            
            // حذف از Redis
            await _cache.RemoveItemFromListAsync<CategoriesNameGet>(
                cacheKey, 
                x => x.CategoryNameId == categoryNameId, 
                _cacheOptions);
        }
        finally
        {
            _lock.Release();
        }
    }
    
    public async Task UpdateCategoryNameAsync(CategoriesNameGet updatedItem, string languageCode)
    {
        await ValidateInputAsync(languageCode);
        var cacheKey = $"{StaticsValue.CategoriesNameGet}_{languageCode}";
        
        await _lock.WaitAsync();
        try
        {
            // به‌روزرسانی در SQL
            await _sqlCategoriesNameTranslations.UpdateCategoryNameAsync(updatedItem);
            
            // به‌روزرسانی در Elasticsearch
            var indexName = $"{StaticsValue.CategoriesNameGet.ToLower()}_{languageCode}";
            await _ieCategoriesNameTranslations.CategoriesNameGetUpdateAsync(indexName, updatedItem);
            
            // به‌روزرسانی در Redis
            await _cache.UpdateItemInListAsync<CategoriesNameGet>(
                cacheKey, 
                x => x.CategoryNameId == updatedItem.CategoryNameId, 
                updatedItem, 
                _cacheOptions);
        }
        finally
        {
            _lock.Release();
        }
    }
}

public async Task<CategoriesNameDeleteResult> DeleteCategoryNameAsync(int CategoryNameId)
{
    if (CategoryNameId < 0)
    {
        throw new ArgumentException("CategoryNameId must be a positive integer.");
    }
    
    var result = await _sqlCategoriesNameTranslations.DeleteCategoryNameAsync(CategoryNameId);
    
    if (result.Message?.Code == 200)
    {
        await _lock.WaitAsync();
        try
        {
            var languageCodes = await _API_LanguageCodes.GetLanguageCodesAsync();

            foreach (var languageCode in languageCodes)
            {
                var cacheKey = $"{StaticsValue.CategoriesNameGet}_{languageCode}";
                
                // حذف از Redis
                await _cache.RemoveItemFromListAsync<CategoriesNameGet>(
                    cacheKey,
                    x => x.CategoryNameId == CategoryNameId,
                    _cacheOptions);

                // حذف از Elasticsearch
                var indexName = $"{StaticsValue.CategoriesNameGet.ToLower()}_{languageCode}";
                await _ieCategoriesNameTranslations.CategoriesNameDeleteAsync(CategoryNameId, indexName);
            }
        }
        finally
        {
            _lock.Release();
        }
    }
    
    return result;
}
 */