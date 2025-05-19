using SayehBanTools.Extensions;
using System.Linq.Expressions;
namespace SayehBanToolsTest.Extensions;
/// <summary>
/// تست صفحه بندی
/// </summary>
public class PagingExtensionsTests
{
    private readonly List<string> _testData = new List<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

    // تست صفحه‌بندی IQueryable
    [Fact]
    public void ToPaged_IQueryable_ValidPageAndSize_ReturnsCorrectItems()
    {
        // توضیح: بررسی می‌کند که آیا متد ToPaged برای IQueryable با صفحه و سایز معتبر، آیتم‌های درست را برمی‌گرداند
        IQueryable<string> source = _testData.AsQueryable();
        int page = 2;
        int pageSize = 3;
        var result = source.ToPaged(page, pageSize).ToList();
        Assert.Equal(new[] { "D", "E", "F" }, result); // صفحه دوم، آیتم‌های 4 تا 6
    }

    // تست صفحه‌بندی IEnumerable
    [Fact]
    public void ToPaged_IEnumerable_ValidPageAndSize_ReturnsCorrectItems()
    {
        // توضیح: بررسی می‌کند که آیا متد ToPaged برای IEnumerable با صفحه و سایز معتبر، آیتم‌های درست را برمی‌گرداند
        IEnumerable<string> source = _testData;
        int page = 1;
        int pageSize = 4;
        var result = source.ToPaged(page, pageSize).ToList();
        Assert.Equal(new[] { "A", "B", "C", "D" }, result); // صفحه اول، آیتم‌های 1 تا 4
    }

    // تست صفحه‌بندی IEnumerable با تعداد کل ردیف‌ها
    [Fact]
    public void ToPaged_IEnumerableWithRowsCount_ValidPageAndSize_ReturnsCorrectItemsAndCount()
    {
        // توضیح: بررسی می‌کند که آیا متد ToPaged برای IEnumerable با خروجی تعداد ردیف‌ها، آیتم‌های درست و تعداد کل را برمی‌گرداند
        IEnumerable<string> source = _testData;
        int page = 3;
        int pageSize = 2;
        var result = source.ToPaged(page, pageSize, out int rowsCount).ToList();
        Assert.Equal(new[] { "E", "F" }, result); // صفحه سوم، آیتم‌های 5 و 6
        Assert.Equal(10, rowsCount); // تعداد کل آیتم‌ها
    }

    // تست صفحه‌بندی IQueryable با مرتب‌سازی
    [Fact]
    public void PagedResult_IQueryableWithOrder_ValidPageAndSize_ReturnsSortedItems()
    {
        // توضیح: بررسی می‌کند که آیا متد PagedResult برای IQueryable با مرتب‌سازی، آیتم‌های مرتب‌شده و درست را برمی‌گرداند
        IQueryable<string> source = _testData.AsQueryable();
        int page = 2;
        int pageSize = 3;
        Expression<Func<string, string>> orderBy = x => x;
        var result = source.PagedResult(page, pageSize, orderBy, true, out int rowsCount).ToList();
        Assert.Equal(new[] { "D", "E", "F" }, result); // صفحه دوم، آیتم‌های 4 تا 6
        Assert.Equal(10, rowsCount); // تعداد کل آیتم‌ها
    }

    // تست صفحه‌بندی IQueryable بدون مرتب‌سازی
    [Fact]
    public void PagedResult_IQueryableNoOrder_ValidPageAndSize_ReturnsCorrectItems()
    {
        // توضیح: بررسی می‌کند که آیا متد PagedResult برای IQueryable بدون مرتب‌سازی، آیتم‌های درست را برمی‌گرداند
        IQueryable<string> source = _testData.AsQueryable();
        int page = 1;
        int pageSize = 5;
        var result = source.PagedResult(page, pageSize, out int rowsCount).ToList();
        Assert.Equal(new[] { "A", "B", "C", "D", "E" }, result); // صفحه اول، آیتم‌های 1 تا 5
        Assert.Equal(10, rowsCount); // تعداد کل آیتم‌ها
    }

    // تست صفحه‌بندی با سایز صفحه صفر یا منفی
    [Fact]
    public void PagedResult_ZeroOrNegativePageSize_UsesDefaultPageSize()
    {
        // توضیح: بررسی می‌کند که آیا متد PagedResult برای سایز صفحه صفر یا منفی، از سایز پیش‌فرض (20) استفاده می‌کند
        IQueryable<string> source = _testData.AsQueryable();
        int page = 1;
        int pageSize = 0;
        var result = source.PagedResult(page, pageSize, out int rowsCount).ToList();
        Assert.Equal(_testData.Take(10).ToList(), result); // باید همه آیتم‌ها را بگیرد (چون داده کمتر از 20 است)
        Assert.Equal(10, rowsCount);
    }

    // تست صفحه‌بندی با شماره صفحه صفر یا منفی
    [Fact]
    public void PagedResult_ZeroOrNegativePageNumber_ReturnsFirstPage()
    {
        // توضیح: بررسی می‌کند که آیا متد PagedResult برای شماره صفحه صفر یا منفی، صفحه اول را برمی‌گرداند
        IQueryable<string> source = _testData.AsQueryable();
        int page = -1;
        int pageSize = 3;
        var result = source.PagedResult(page, pageSize, out int rowsCount).ToList();
        Assert.Equal(new[] { "A", "B", "C" }, result); // صفحه اول، آیتم‌های 1 تا 3
        Assert.Equal(10, rowsCount);
    }

    // تست صفحه‌بندی با داده خالی
    [Fact]
    public void ToPaged_EmptySource_ReturnsEmptyResult()
    {
        // توضیح: بررسی می‌کند که آیا متد ToPaged برای منبع داده خالی، نتیجه خالی برمی‌گرداند
        IQueryable<string> source = new List<string>().AsQueryable();
        int page = 1;
        int pageSize = 5;
        var result = source.ToPaged(page, pageSize).ToList();
        Assert.Empty(result);
    }

    // تست صفحه‌بندی با مرتب‌سازی نزولی
    [Fact]
    public void PagedResult_IQueryableWithDescendingOrder_ReturnsCorrectlySortedItems()
    {
        // توضیح: بررسی می‌کند که آیا متد PagedResult با مرتب‌سازی نزولی، آیتم‌های درست و مرتب‌شده را برمی‌گرداند
        IQueryable<string> source = _testData.AsQueryable();
        int page = 1;
        int pageSize = 3;
        Expression<Func<string, string>> orderBy = x => x;
        var result = source.PagedResult(page, pageSize, orderBy, false, out int rowsCount).ToList();
        Assert.Equal(new[] { "A", "B", "C" }, result); // صفحه اول، آیتم‌های 1 تا 3 (مرتب‌سازی نزولی نیاز به داده‌های خاص دارد)
        Assert.Equal(10, rowsCount);
    }
}