using SayehBanTools.Validations;

namespace SayehBanToolsTest.Validations;

public class HierarchyValidatorTests
{
    /// <summary>
    /// تست بررسی مسیر خالی: متد IsValidHierarchyPath باید برای رشته خالی مقدار true برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_EmptyString_ReturnsTrue()
    {
        // Arrange
        string path = "";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// تست بررسی مسیر نال: متد IsValidHierarchyPath باید برای مقدار نال مقدار true برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_NullString_ReturnsTrue()
    {
        // Arrange
        string? path = null;

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path ?? "");

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// تست بررسی مسیر ریشه معتبر ("/"): متد IsValidHierarchyPath باید مقدار true برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_ValidRootPath_ReturnsTrue()
    {
        // Arrange
        string path = "/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// تست بررسی مسیر عددی معتبر (مانند "/1/"): متد IsValidHierarchyPath باید مقدار true برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_ValidNumericPath_ReturnsTrue()
    {
        // Arrange
        string path = "/1/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// تست بررسی مسیر با چند بخش عددی (مانند "/1/2/3/"): متد IsValidHierarchyPath باید مقدار true برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_MultipleNumericParts_ReturnsTrue()
    {
        // Arrange
        string path = "/1/2/3/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// تست بررسی مسیر بدون اسلش ابتدایی: متد IsValidHierarchyPath باید مقدار false برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_MissingStartSlash_ReturnsFalse()
    {
        // Arrange
        string path = "1/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// تست بررسی مسیر بدون اسلش انتهایی: متد IsValidHierarchyPath باید مقدار false برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_MissingEndSlash_ReturnsFalse()
    {
        // Arrange
        string path = "/1";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// تست بررسی مسیر با بخش غیرعددی (مانند "/1/a/"): متد IsValidHierarchyPath باید مقدار false برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_NonNumericPart_ReturnsFalse()
    {
        // Arrange
        string path = "/1/a/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// تست بررسی مسیر با عدد منفی (مانند "/-1/"): متد IsValidHierarchyPath باید مقدار true برگرداند.
    /// </summary>
    [Fact]
    public void IsValidHierarchyPath_NegativeNumber_ReturnsTrue()
    {
        // Arrange
        string path = "/-1/";

        // Act
        bool result = HierarchyValidator.IsValidHierarchyPath(path);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// تست بررسی مسیر خالی: متد ValidateHierarchyPath باید برای مسیر خالی بدون خطا اجرا شود.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_EmptyString_DoesNotThrow()
    {
        // Arrange
        string path = "";

        // Act & Assert
        HierarchyValidator.ValidateHierarchyPath(path);
        // اگر استثنایی پرتاب نشود، تست موفق است.
    }

    /// <summary>
    /// تست بررسی مسیر نال: متد ValidateHierarchyPath باید برای مسیر نال بدون خطا اجرا شود.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_NullString_DoesNotThrow()
    {
        // Arrange
        string? path = null;

        // Act & Assert
        HierarchyValidator.ValidateHierarchyPath(path!);
        // اگر استثنایی پرتاب نشود، تست موفق است.
    }

    /// <summary>
    /// تست بررسی مسیر ریشه معتبر ("/"): متد باید بدون خطا اجرا شود.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_ValidRootPath_DoesNotThrow()
    {
        // Arrange
        string path = "/";

        // Act & Assert
        HierarchyValidator.ValidateHierarchyPath(path);
        // اگر استثنایی پرتاب نشود، تست موفق است.
    }

    /// <summary>
    /// تست بررسی مسیر عددی معتبر (مانند "/1/"): متد باید بدون خطا اجرا شود.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_ValidNumericPath_DoesNotThrow()
    {
        // Arrange
        string path = "/1/";

        // Act & Assert
        HierarchyValidator.ValidateHierarchyPath(path);
        // اگر استثنایی پرتاب نشود، تست موفق است.
    }

    /// <summary>
    /// تست بررسی مسیر با چند بخش عددی (مانند "/1/2/3/"): متد باید بدون خطا اجرا شود.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_MultipleNumericParts_DoesNotThrow()
    {
        // Arrange
        string path = "/1/2/3/";

        // Act & Assert
        HierarchyValidator.ValidateHierarchyPath(path);
        // اگر استثنایی پرتاب نشود، تست موفق است.
    }

    /// <summary>
    /// تست بررسی مسیر نامعتبر (بدون اسلش ابتدایی): متد باید استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_MissingStartSlash_ThrowsArgumentException()
    {
        // Arrange
        string path = "1/";
        string expectedMessage = "فرمت سلسله مراتب نامعتبر است. باید به صورت / یا /1/ باشد. (Parameter 'parentHierarchyPath')";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => HierarchyValidator.ValidateHierarchyPath(path));
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal("parentHierarchyPath", exception.ParamName);
    }

    /// <summary>
    /// تست بررسی مسیر نامعتبر (بدون اسلش انتهایی): متد باید استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_MissingEndSlash_ThrowsArgumentException()
    {
        // Arrange
        string path = "/1";
        string expectedMessage = "فرمت سلسله مراتب نامعتبر است. باید به صورت / یا /1/ باشد. (Parameter 'parentHierarchyPath')";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => HierarchyValidator.ValidateHierarchyPath(path));
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal("parentHierarchyPath", exception.ParamName);
    }

    /// <summary>
    /// تست بررسی مسیر با بخش غیرعددی (مانند "/1/a/"): متد باید استثنای ArgumentException پرتاب کند.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_NonNumericPart_ThrowsArgumentException()
    {
        // Arrange
        string path = "/1/a/";
        string expectedMessage = "فرمت سلسله مراتب نامعتبر است. باید به صورت / یا /1/ باشد. (Parameter 'parentHierarchyPath')";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => HierarchyValidator.ValidateHierarchyPath(path));
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal("parentHierarchyPath", exception.ParamName);
    }

    /// <summary>
    /// تست بررسی مسیر با پیام خطای سفارشی: متد باید استثنای ArgumentException با پیام سفارشی پرتاب کند.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_InvalidPathWithCustomMessage_ThrowsArgumentExceptionWithCustomMessage()
    {
        // Arrange
        string path = "/1/a/";
        string customMessage = "مسیر سلسله‌مراتب نامعتبر است!";
        string expectedMessage = $"{customMessage} (Parameter 'parentHierarchyPath')";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => HierarchyValidator.ValidateHierarchyPath(path, customMessage));
        Assert.Equal(expectedMessage, exception.Message);
        Assert.Equal("parentHierarchyPath", exception.ParamName);
    }

    /// <summary>
    /// تست بررسی مسیر معتبر با عدد منفی (مانند "/-1/"): متد باید بدون خطا اجرا شود.
    /// </summary>
    [Fact]
    public void ValidateHierarchyPath_NegativeNumber_DoesNotThrow()
    {
        // Arrange
        string path = "/-1/";

        // Act & Assert
        HierarchyValidator.ValidateHierarchyPath(path);
        // اگر استثنایی پرتاب نشود، تست موفق است.
    }
}