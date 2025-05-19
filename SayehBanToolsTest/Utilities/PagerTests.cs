using SayehBanTools.Utilities;

namespace SayehBanToolsTest.Utilities;
public class PagerTests
{
    /// <summary>
    /// تست مقداردهی اولیه با مقادیر پیش‌فرض
    /// بررسی می‌کند که با مقادیر پیش‌فرض صحیح کار می‌کند
    /// </summary>
    [Fact]
    public void Pager_InitializeWithDefaultValues_ShouldWorkCorrectly()
    {
        // Arrange
        long totalItems = 100;
        int expectedTotalPages = 10; // 100 آیتم با صفحه‌بندی 10 تایی = 10 صفحه

        // Act
        var pager = new Pager(totalItems);

        // Assert
        Assert.Equal(expectedTotalPages, pager.TotalPages);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, pager.Pages); // پیش‌فرض maxPages=5
    }

    /// <summary>
    /// تست صفحه جاری کمتر از 1
    /// بررسی می‌کند که اگر صفحه جاری کمتر از 1 باشد، روی 1 تنظیم شود
    /// </summary>
    [Fact]
    public void Pager_CurrentPageLessThanOne_ShouldSetToOne()
    {
        // Arrange
        long totalItems = 100;
        int currentPage = -1;

        // Act
        var pager = new Pager(totalItems, currentPage);

        // Assert
        Assert.Equal(1, pager.Pages.First());
    }

    /// <summary>
    /// تست صفحه جاری بیشتر از کل صفحات
    /// بررسی می‌کند که اگر صفحه جاری بیشتر از کل صفحات باشد، روی آخرین صفحه تنظیم شود
    /// </summary>
    [Fact]
    public void Pager_CurrentPageGreaterThanTotalPages_ShouldSetToLastPage()
    {
        // Arrange
        long totalItems = 100; // 10 صفحه
        int currentPage = 15; // بیشتر از کل صفحات
        int expectedPage = 10;

        // Act
        var pager = new Pager(totalItems, currentPage);

        // Assert
        Assert.Equal(expectedPage, pager.Pages.Last());
    }

    /// <summary>
    /// تست تعداد صفحات کمتر از حداکثر صفحات
    /// بررسی می‌کند که وقتی کل صفحات کمتر از maxPages باشد، همه صفحات نمایش داده شوند
    /// </summary>
    [Fact]
    public void Pager_TotalPagesLessThanMaxPages_ShouldShowAllPages()
    {
        // Arrange
        long totalItems = 30; // 3 صفحه با صفحه‌بندی 10 تایی
        int maxPages = 5;

        // Act
        var pager = new Pager(totalItems, 1, 10, maxPages);

        // Assert
        Assert.Equal(3, pager.TotalPages);
        Assert.Equal(new[] { 1, 2, 3 }, pager.Pages);
    }

    /// <summary>
    /// تست صفحه جاری در ابتدا
    /// بررسی می‌کند که وقتی صفحه جاری در ابتدای محدوده باشد، صفحات به درستی نمایش داده شوند
    /// </summary>
    [Fact]
    public void Pager_CurrentPageNearStart_ShouldShowCorrectPages()
    {
        // Arrange
        long totalItems = 200; // 20 صفحه
        int currentPage = 2;
        int maxPages = 5;

        // Act
        var pager = new Pager(totalItems, currentPage, 10, maxPages);

        // Assert
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, pager.Pages);
    }

    /// <summary>
    /// تست صفحه جاری در میانه
    /// بررسی می‌کند که وقتی صفحه جاری در میانه محدوده باشد، صفحات به درستی نمایش داده شوند
    /// </summary>
    [Fact]
    public void Pager_CurrentPageInMiddle_ShouldShowCorrectPages()
    {
        // Arrange
        long totalItems = 200; // 20 صفحه
        int currentPage = 10;
        int maxPages = 5;

        // Act
        var pager = new Pager(totalItems, currentPage, 10, maxPages);

        // Assert
        Assert.Equal(new[] { 8, 9, 10, 11, 12 }, pager.Pages);
    }

    /// <summary>
    /// تست صفحه جاری در انتها
    /// بررسی می‌کند که وقتی صفحه جاری در انتهای محدوده باشد، صفحات به درستی نمایش داده شوند
    /// </summary>
    [Fact]
    public void Pager_CurrentPageNearEnd_ShouldShowCorrectPages()
    {
        // Arrange
        long totalItems = 200; // 20 صفحه
        int currentPage = 19;
        int maxPages = 5;

        // Act
        var pager = new Pager(totalItems, currentPage, 10, maxPages);

        // Assert
        Assert.Equal(new[] { 16, 17, 18, 19, 20 }, pager.Pages);
    }

    /// <summary>
    /// تست صفحه‌بندی با تعداد آیتم‌های صفر
    /// بررسی می‌کند که وقتی هیچ آیتمی وجود ندارد، رفتار صحیح داشته باشد
    /// </summary>
    [Fact]
    public void Pager_WithZeroItems_ShouldHandleCorrectly()
    {
        // Arrange
        long totalItems = 0;

        // Act
        var pager = new Pager(totalItems);

        // Assert
        Assert.Equal(0, pager.TotalPages);
        Assert.Empty(pager.Pages);
    }

    /// <summary>
    /// تست صفحه‌بندی با اندازه صفحه سفارشی
    /// بررسی می‌کند که با اندازه صفحه غیرپیش‌فرض صحیح کار کند
    /// </summary>
    [Theory]
    [InlineData(100, 20, 5)] // 100 آیتم، 20 آیتم در هر صفحه = 5 صفحه
    [InlineData(105, 10, 11)] // 105 آیتم، 10 آیتم در هر صفحه = 11 صفحه
    [InlineData(15, 5, 3)] // 15 آیتم، 5 آیتم در هر صفحه = 3 صفحه
    public void Pager_WithCustomPageSize_ShouldCalculateCorrectly(long totalItems, int pageSize, int expectedTotalPages)
    {
        // Act
        var pager = new Pager(totalItems, 1, pageSize);

        // Assert
        Assert.Equal(expectedTotalPages, pager.TotalPages);
    }

    /// <summary>
    /// تست maxPages فرد و زوج
    /// بررسی می‌کند که با مقادیر فرد و زوج برای maxPages صحیح کار کند
    /// </summary>
    [Theory]
    [InlineData(5)] // فرد
    [InlineData(6)] // زوج
    public void Pager_WithOddAndEvenMaxPages_ShouldWorkCorrectly(int maxPages)
    {
        // Arrange
        long totalItems = 200; // 20 صفحه
        int currentPage = 10;

        // Act
        var pager = new Pager(totalItems, currentPage, 10, maxPages);

        // Assert
        Assert.Equal(maxPages, pager.Pages.Count());
    }
}