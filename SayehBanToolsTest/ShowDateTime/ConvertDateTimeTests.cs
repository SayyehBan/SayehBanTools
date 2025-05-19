using SayehBanTools.ShowDateTime;

namespace SayehBanToolsTest.ShowDateTime;
public class ConvertDateTimeTests
{
    /// <summary>
    /// تست تبدیل تاریخ میلادی به شمسی با فرمت صحیح
    /// این تست بررسی می‌کند که متد miladi2shamsi تاریخ میلادی را به درستی به تاریخ شمسی تبدیل می‌کند
    /// و روز هفته را به فارسی نمایش می‌دهد
    /// </summary>
    [Fact]
    public void Miladi2Shamsi_ShouldReturnCorrectPersianDateWithWeekDay()
    {
        // Arrange
        var miladiDate = new DateTime(2023, 12, 22); // جمعه 1 دی 1402

        // Act
        var result = ConvertDateTime.miladi2shamsi(miladiDate);

        // Assert
        Assert.Contains("1402/10/01", result);
        Assert.Contains("جمعه", result);
    }

    /// <summary>
    /// تست تبدیل تاریخ UTC به زمان محلی
    /// این تست بررسی می‌کند که متد ConvertToLocalDateTime
    /// زمان UTC را به درستی به زمان محلی سیستم تبدیل می‌کند
    /// </summary>
    [Fact]
    public void ConvertToLocalDateTime_ShouldConvertUtcToLocalTime()
    {
        // Arrange
        var utcDate = new DateTime(2023, 12, 22, 12, 0, 0, DateTimeKind.Utc);
        var expectedLocal = TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.Local);

        // Act
        var result = ConvertDateTime.ConvertToLocalDateTime(utcDate);

        // Assert
        Assert.Equal(expectedLocal, result);
    }

    /// <summary>
    /// تست تبدیل تاریخ میلادی به شمسی بدون زمان
    /// این تست بررسی می‌کند که متد MiladiToShamsi
    /// فقط بخش تاریخ را بدون اطلاعات زمان تبدیل می‌کند
    /// </summary>
    [Theory]
    [InlineData(2023, 12, 22, "1402/10/01")]
    [InlineData(2024, 3, 20, "1403/01/01")] // نوروز 1403
    public void MiladiToShamsi_ShouldReturnOnlyDateWithoutTime(int year, int month, int day, string expected)
    {
        // Arrange
        var miladiDate = new DateTime(year, month, day);

        // Act
        var result = ConvertDateTime.MiladiToShamsi(miladiDate);

        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// تست تبدیل تاریخ شمسی به میلادی
    /// این تست بررسی می‌کند که متد ShamsiToMiladi
    /// تاریخ شمسی را به درستی به میلادی تبدیل می‌کند
    /// </summary>
    [Theory]
    [InlineData("1402/10/01", 2023, 12, 22)] // 1 دی 1402
    [InlineData("1403/01/01", 2024, 3, 20)] // نوروز 1403
    public void ShamsiToMiladi_ShouldConvertPersianDateToGregorian(string shamsiDate, int expectedYear, int expectedMonth, int expectedDay)
    {
        // Act
        var result = ConvertDateTime.ShamsiToMiladi(shamsiDate);

        // Assert
        Assert.Equal(expectedYear, result.Year);
        Assert.Equal(expectedMonth, result.Month);
        Assert.Equal(expectedDay, result.Day);
    }

    /// <summary>
    /// تست رفتار متد ShamsiToMiladi با ورودی نامعتبر
    /// این تست بررسی می‌کند که متد با ورودی نامعتبر یا خالی چگونه رفتار می‌کند
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("invalid-date")]
    [InlineData("1402/10")] // فرمت ناقص
    public void ShamsiToMiladi_ShouldHandleInvalidInput(string invalidInput)
    {
        // Act
        var result = ConvertDateTime.ShamsiToMiladi(invalidInput);

        // Assert
        Assert.Equal(default(DateTime), result);
    }

    /// <summary>
    /// تست نمایش صحیح روزهای هفته در متد miladi2shamsi
    /// این تست بررسی می‌کند که تمام روزهای هفته به درستی به فارسی ترجمه می‌شوند
    /// </summary>
    [Theory]
    [InlineData(DayOfWeek.Saturday, "شنبه")]
    [InlineData(DayOfWeek.Sunday, "يكشنبه")]
    [InlineData(DayOfWeek.Monday, "دوشنبه")]
    [InlineData(DayOfWeek.Tuesday, "سه شنبه")]
    [InlineData(DayOfWeek.Wednesday, "چهار شنبه")]
    [InlineData(DayOfWeek.Thursday, "بنچ شنبه")]
    [InlineData(DayOfWeek.Friday, "جمعه")]
    public void Miladi2Shamsi_ShouldCorrectlyTranslateAllWeekDays(DayOfWeek dayOfWeek, string expectedPersianDay)
    {
        // Arrange
        // ایجاد یک تاریخ با روز هفته مشخص (مثلا اولین شنبه بعد از یک تاریخ مشخص)
        var date = new DateTime(2023, 1, 1); // یک تاریخ دلخواه
        while (date.DayOfWeek != dayOfWeek)
        {
            date = date.AddDays(1);
        }

        // Act
        var result = ConvertDateTime.miladi2shamsi(date);

        // Assert
        Assert.Contains(expectedPersianDay, result);
    }
}