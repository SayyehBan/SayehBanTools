using SayehBanTools.AccessDenied;

namespace SayehBanToolsTest.AccessDenied;
/// <summary>
/// تست IPAccess
/// </summary>
public class IPAccessTests
{
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_ValidIPWithinRange_ReturnsTrue()
    {
        // Arrange
        string ipAddress = "192.168.1.100";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act
        bool result = IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd);

        // Assert
        Assert.True(result);
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار ندارد
    /// </summary>
    [Fact]
    public void IsInRange_IPBelowRange_ReturnsFalse()
    {
        // Arrange
        string ipAddress = "192.168.1.0";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act
        bool result = IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd);

        // Assert
        Assert.False(result);
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده ق
    /// </summary>
    [Fact]
    public void IsInRange_IPAboveRange_ReturnsFalse()
    {
        // Arrange
        string ipAddress = "192.168.1.201";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act
        bool result = IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd);

        // Assert
        Assert.False(result);
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_IPAtRangeStart_ReturnsTrue()
    {
        // Arrange
        string ipAddress = "192.168.1.1";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act
        bool result = IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd);

        // Assert
        Assert.True(result);
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_IPAtRangeEnd_ReturnsTrue()
    {
        // Arrange
        string ipAddress = "192.168.1.200";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act
        bool result = IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd);

        // Assert
        Assert.True(result);
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_InvalidIPFormat_ThrowsArgumentException()
    {
        // Arrange
        string ipAddress = "192.168.1";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd));
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_NonNumericIPPart_ThrowsArgumentException()
    {
        // Arrange
        string ipAddress = "192.168.1.abc";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd));
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_NullIPAddress_ThrowsArgumentNullException()
    {
        // Arrange
        string? ipAddress = null;
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => IPAccess.IsInRange(ipAddress!, rangeStart, rangeEnd));
    }
    /// <summary>
    /// تست IsInRange برای بررسی اینکه آیا یک IP در محدوده مشخص شده قرار دارد یا خیر
    /// </summary>
    [Fact]
    public void IsInRange_IPOutOfValidRange_ThrowsArgumentException()
    {
        // Arrange
        string ipAddress = "192.168.1.256";
        string rangeStart = "192.168.1.1";
        string rangeEnd = "192.168.1.200";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => IPAccess.IsInRange(ipAddress, rangeStart, rangeEnd));
    }
}