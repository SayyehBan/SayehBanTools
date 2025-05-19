using SayehBanTools.ConfigureService.Exceptions;

namespace SayehBanToolsTest.ConfigureService.Exceptions;
/// <summary>
/// تست‌های واحد برای کلاس NotFoundException
/// </summary>
public class NotFoundExceptionTests
{
    /// <summary>
    /// تست بررسی ایجاد صحیح استثنا با پیام سفارشی
    /// </summary>
    [Fact]
    public void Constructor_WithCustomMessage_SetsMessageCorrectly()
    {
        // Arrange
        string expectedMessage = "طلاعات مورد نظر یافت نشد";

        // Act
        var exception = new NotFoundException(expectedMessage);

        // Assert
        Assert.Equal(expectedMessage, exception.Message);
    }

    /// <summary>
    /// تست بررسی اینکه استثنا از نوع Exception است
    /// </summary>
    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new NotFoundException("خطای تست");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    /// <summary>
    /// تست بررسی رفتار استثنا هنگام استفاده از پیام خالی
    /// </summary>
    [Fact]
    public void Constructor_WithEmptyMessage_SetsEmptyMessage()
    {
        // Arrange
        string emptyMessage = string.Empty;

        // Act
        var exception = new NotFoundException(emptyMessage);

        // Assert
        Assert.Equal(emptyMessage, exception.Message);
    }
    /// <summary>
    /// تست بررسی رفتار استثنا هنگام استفاده از پیام null
    /// </summary>
    [Fact]
    public void Constructor_WithNullMessage_UsesDefaultExceptionMessage()
    {
        // Arrange & Act
        var exception = new NotFoundException(null!);

        // Assert
        Assert.NotNull(exception.Message); // پیام نباید null باشد
        Assert.Contains("NotFoundException", exception.Message); // پیام پیش‌فرض حاوی نام کلاس است
    }
}