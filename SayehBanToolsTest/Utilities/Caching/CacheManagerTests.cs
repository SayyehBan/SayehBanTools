using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using SayehBanTools.Utilities.Caching;
using System.Text;
using System.Text.Json;
using Xunit;

namespace SayehBanToolsTest.Utilities.Caching;

public class CacheManagerTests
{
    private readonly IMemoryCache _memoryCache;
    private readonly Mock<IDistributedCache> _distributedCacheMock;
    private readonly CacheManager _memoryCacheManager;
    private readonly CacheManager _distributedCacheManager;

    public CacheManagerTests()
    {
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _distributedCacheMock = new Mock<IDistributedCache>();
        _memoryCacheManager = new CacheManager(_memoryCache);
        _distributedCacheManager = new CacheManager(_distributedCacheMock.Object);
    }

    // تست 1: بررسی دریافت مقدار موجود از کش حافظه (Memory Cache)
    // هدف: اطمینان از اینکه متد TryGetValueAsync در صورت وجود مقدار در کش حافظه، مقدار صحیح را برمی‌گرداند.
    [Fact]
    public async Task TryGetValueAsync_MemoryCache_ValueExists_ReturnsValue()
    {
        // Arrange: آماده‌سازی یک کلید و مقدار تستی و ذخیره آن در کش حافظه
        string key = "testKey";
        string expectedValue = "testValue";
        _memoryCache.Set(key, expectedValue);

        // Act: فراخوانی متد TryGetValueAsync برای دریافت مقدار
        var result = await _memoryCacheManager.TryGetValueAsync<string>(key);

        // Assert: بررسی اینکه مقدار برگشتی با مقدار مورد انتظار برابر است
        Assert.Equal(expectedValue, result);
    }

    // تست 2: بررسی دریافت مقدار ناموجود از کش حافظه
    // هدف: اطمینان از اینکه اگر کلیدی در کش حافظه وجود نداشته باشد، متد TryGetValueAsync مقدار null برمی‌گرداند.
    [Fact]
    public async Task TryGetValueAsync_MemoryCache_ValueNotExists_ReturnsDefault()
    {
        // Arrange: تعریف یک کلید که در کش وجود ندارد
        string key = "nonExistentKey";

        // Act: فراخوانی متد TryGetValueAsync برای کلیدی که وجود ندارد
        var result = await _memoryCacheManager.TryGetValueAsync<string>(key);

        // Assert: بررسی اینکه مقدار برگشتی null است
        Assert.Null(result);
    }

    // تست 3: بررسی دریافت مقدار موجود از کش توزیع‌شده (Distributed Cache)
    // هدف: اطمینان از اینکه متد TryGetValueAsync مقدار صحیح را از کش توزیع‌شده بازیابی می‌کند.
    [Fact]
    public async Task TryGetValueAsync_DistributedCache_ValueExists_ReturnsValue()
    {
        // Arrange: آماده‌سازی کلید، مقدار تستی و شبیه‌سازی رفتار کش توزیع‌شده
        string key = "testKey";
        string expectedValue = "testValue";
        var serializedValue = JsonSerializer.Serialize(expectedValue);
        var byteValue = Encoding.UTF8.GetBytes(serializedValue);
        _distributedCacheMock
            .Setup(x => x.GetAsync(key, default))
            .ReturnsAsync(byteValue);

        // Act: فراخوانی متد TryGetValueAsync برای دریافت مقدار
        var result = await _distributedCacheManager.TryGetValueAsync<string>(key);

        // Assert: بررسی اینکه مقدار برگشتی با مقدار مورد انتظار برابر است
        Assert.Equal(expectedValue, result);
    }

    // تست 4: بررسی دریافت مقدار ناموجود از کش توزیع‌شده
    // هدف: اطمینان از اینکه اگر کلیدی در کش توزیع‌شده وجود نداشته باشد، متد TryGetValueAsync مقدار null برمی‌گرداند.
    [Fact]
    public async Task TryGetValueAsync_DistributedCache_ValueNotExists_ReturnsDefault()
    {
        // Arrange: تعریف یک کلید که در کش وجود ندارد و شبیه‌سازی رفتار کش توزیع‌شده
        string key = "nonExistentKey";
        _distributedCacheMock
            .Setup(x => x.GetAsync(key, default))
            .ReturnsAsync((byte[])null!);

        // Act: فراخوانی متد TryGetValueAsync برای کلیدی که وجود ندارد
        var result = await _distributedCacheManager.TryGetValueAsync<string>(key);

        // Assert: بررسی اینکه مقدار برگشتی null است
        Assert.Null(result);
    }

    // تست 5: بررسی ثبت مقدار در کش حافظه
    // هدف: اطمینان از اینکه متد SetAsync مقدار را به درستی در کش حافظه ذخیره می‌کند.
    [Fact]
    public async Task SetAsync_MemoryCache_SetsValue()
    {
        // Arrange: آماده‌سازی کلید، مقدار و تنظیمات کش حافظه
        string key = "testKey";
        string value = "testValue";
        var options = new MemoryCacheEntryOptions();

        // Act: فراخوانی متد SetAsync برای ذخیره مقدار
        await _memoryCacheManager.SetAsync(key, value, options);

        // Assert: بررسی اینکه مقدار به درستی در کش ذخیره شده است
        var cachedValue = _memoryCache.Get<string>(key);
        Assert.Equal(value, cachedValue);
    }

    // تست 6: بررسی ثبت مقدار در کش توزیع‌شده
    // هدف: اطمینان از اینکه متد SetAsync مقدار را به درستی در کش توزیع‌شده ذخیره می‌کند.
    [Fact]
    public async Task SetAsync_DistributedCache_SetsValue()
    {
        // Arrange: آماده‌سازی کلید، مقدار، تنظیمات و شبیه‌سازی رفتار کش توزیع‌شده
        string key = "testKey";
        string value = "testValue";
        var options = new DistributedCacheEntryOptions();
        var serializedValue = JsonSerializer.Serialize(value);
        var byteValue = Encoding.UTF8.GetBytes(serializedValue);
        _distributedCacheMock
            .Setup(x => x.SetAsync(key, byteValue, options, default))
            .Returns(Task.CompletedTask);

        // Act: فراخوانی متد SetAsync برای ذخیره مقدار
        await _distributedCacheManager.SetAsync(key, value, options);

        // Assert: بررسی اینکه متد SetAsync در کش توزیع‌شده فراخوانی شده است
        _distributedCacheMock.Verify(x => x.SetAsync(key, byteValue, options, default), Times.Once());
    }

    // تست 7: بررسی حذف مقدار از کش حافظه
    // هدف: اطمینان از اینکه متد ResetCacheAsync مقدار را از کش حافظه حذف می‌کند.
    [Fact]
    public async Task ResetCacheAsync_MemoryCache_RemovesValue()
    {
        // Arrange: آماده‌سازی کلید با پیشوند و ذخیره مقدار تستی در کش حافظه
        string prefix = "prefix_";
        string keyPart = "testKey";
        string cacheKey = $"{prefix}{keyPart}";
        _memoryCache.Set(cacheKey, "testValue");

        // Act: فراخوانی متد ResetCacheAsync برای حذف مقدار
        await _memoryCacheManager.ResetCacheAsync(prefix, keyPart);

        // Assert: بررسی اینکه مقدار از کش حذف شده است
        var cachedValue = _memoryCache.Get(cacheKey);
        Assert.Null(cachedValue);
    }

    // تست 8: بررسی حذف مقدار از کش توزیع‌شده
    // هدف: اطمینان از اینکه متد ResetCacheAsync مقدار را از کش توزیع‌شده حذف می‌کند.
    [Fact]
    public async Task ResetCacheAsync_DistributedCache_RemovesValue()
    {
        // Arrange: آماده‌سازی کلید با پیشوند و شبیه‌سازی رفتار کش توزیع‌شده
        string prefix = "prefix_";
        string keyPart = "testKey";
        string cacheKey = $"{prefix}{keyPart}";
        _distributedCacheMock
            .Setup(x => x.RemoveAsync(cacheKey, default))
            .Returns(Task.CompletedTask);

        // Act: فراخوانی متد ResetCacheAsync برای حذف مقدار
        await _distributedCacheManager.ResetCacheAsync(prefix, keyPart);

        // Assert: بررسی اینکه متد RemoveAsync در کش توزیع‌شده فراخوانی شده است
        _distributedCacheMock.Verify(x => x.RemoveAsync(cacheKey, default), Times.Once());
    }

    // تست 9: بررسی رفتار در صورت استفاده از کلیدهای نامعتبر (null)
    // هدف: اطمینان از اینکه متد ResetCacheAsync در صورت دریافت کلید null خطای مناسب تولید می‌کند.
    [Fact]
    public async Task ResetCacheAsync_NullKeyParts_ThrowsException()
    {
        // Act & Assert: بررسی اینکه فراخوانی متد با کلید null منجر به خطای InvalidOperationException می‌شود
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _memoryCacheManager.ResetCacheAsync("prefix", null!));
    }

    // تست 10: بررسی رفتار در صورت عدم پیکربندی کش
    // هدف: اطمینان از اینکه متد TryGetValueAsync در صورت عدم وجود کش (Memory یا Distributed) خطای مناسب تولید می‌کند.
    [Fact]
    public async Task TryGetValueAsync_NoCacheProvider_ThrowsException()
    {
        // Arrange: ایجاد یک نمونه CacheManager بدون کش معتبر
        var cacheManager = new CacheManager((IMemoryCache)null!);

        // Act & Assert: بررسی اینکه فراخوانی متد منجر به خطای InvalidOperationException می‌شود
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            cacheManager.TryGetValueAsync<string>("testKey"));
    }

    // تست 11: بررسی رفتار در صورت استفاده از گزینه‌های نامعتبر
    // هدف: اطمینان از اینکه متد SetAsync در صورت دریافت گزینه‌های نامعتبر (نه MemoryCacheEntryOptions و نه DistributedCacheEntryOptions) خطای مناسب تولید می‌کند.
    [Fact]
    public async Task SetAsync_InvalidOptions_ThrowsException()
    {
        // Arrange: آماده‌سازی کلید، مقدار و یک شیء گزینه نامعتبر
        string key = "testKey";
        string value = "testValue";
        var invalidOptions = new object();

        // Act & Assert: بررسی اینکه فراخوانی متد با گزینه نامعتبر منجر به خطای InvalidOperationException می‌شود
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _memoryCacheManager.SetAsync(key, value, invalidOptions));
    }
}