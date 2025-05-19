using SayehBanTools.GenerateValue;

namespace SayehBanToolsTest.GenerateValue;
/// <summary>
/// تست تولید کد ملی
/// </summary>
public class NationalCodeTests
{
    // تست تولید کد ملی تصادفی
    [Fact]
    public void GenerateRandom_ReturnsValidNationalCode()
    {
        // توضیح: بررسی می‌کند که آیا متد GenerateRandom یک کد ملی 10 رقمی تولید می‌کند
        string result = NationalCode.GenerateRandom();
        Assert.Equal(10, result.Length);
        Assert.Matches(@"^\d{10}$", result); // فقط شامل ارقام
    }

    // تست منحصربه‌فرد بودن کدهای تصادفی
    [Fact]
    public void GenerateRandom_GeneratesDifferentCodes()
    {
        // توضیح: بررسی می‌کند که آیا متد GenerateRandom در چند اجرا، کدهای متفاوتی تولید می‌کند
        string result1 = NationalCode.GenerateRandom();
        string result2 = NationalCode.GenerateRandom();
        Assert.NotEqual(result1, result2); // احتمال برابر بودن بسیار کم است
    }

    // تست تولید کد ملی رند
    [Fact]
    public void GenerateRound_ReturnsValidRoundNationalCode()
    {
        // توضیح: بررسی می‌کند که آیا متد GenerateRound یک کد ملی 10 رقمی رند تولید می‌کند
        string result = NationalCode.GenerateRound();
        Assert.Equal(10, result.Length);
        Assert.Matches(@"^\d{10}$", result); // فقط شامل ارقام

        // بررسی الگوی رند
        string nineDigits = result.Substring(0, 9);
        bool isRoundPattern = new[] { "123456789", "987654321", "112233445", "554433221", "121212121" }.Contains(nineDigits);
        Assert.True(isRoundPattern, "کد باید یکی از الگوهای رند باشد");
    }

    // تست کد ملی معتبر (کد ارائه‌شده توسط کاربر)
    [Fact]
    public void IsValidNationalCode_ValidCode_ReturnsTrue()
    {
        // توضیح: بررسی می‌کند که آیا متد IsValidNationalCode برای یک کد ملی معتبر، مقدار true برمی‌گرداند
        string validCode = "0014401509"; // کد ملی معتبر ارائه‌شده
        bool result = NationalCode.IsValidNationalCode(validCode);
        Assert.True(result);
    }

    // تست کد ملی با ارقام یکسان (معتبر)
    [Fact]
    public void IsValidNationalCode_AllDigitsEqual_ReturnsTrue()
    {
        // توضیح: بررسی می‌کند که آیا متد IsValidNationalCode برای کد با ارقام یکسان، مقدار true برمی‌گرداند
        string validCode = "0014401509"; // کد با ارقام یکسان، معتبر است
        bool result = NationalCode.IsValidNationalCode(validCode);
        Assert.True(result);
    }

    // تست کد ملی خالی
    [Fact]
    public void IsValidNationalCode_EmptyCode_ThrowsException()
    {
        // توضیح: بررسی می‌کند که آیا متد IsValidNationalCode برای کد خالی استثنا پرتاب می‌کند
        string emptyCode = "";
        var exception = Assert.Throws<Exception>(() => NationalCode.IsValidNationalCode(emptyCode));
        Assert.Equal("لطفا کد ملی را صحیح وارد نمایید", exception.Message);
    }

    // تست کد ملی با طول نادرست
    [Fact]
    public void IsValidNationalCode_IncorrectLength_ThrowsException()
    {
        // توضیح: بررسی می‌کند که آیا متد IsValidNationalCode برای کد با طول نادرست استثنا پرتاب می‌کند
        string invalidCode = "123456789";
        var exception = Assert.Throws<Exception>(() => NationalCode.IsValidNationalCode(invalidCode));
        Assert.Equal("طول کد ملی باید ده کاراکتر باشد", exception.Message);
    }

    // تست کد ملی با کاراکتر غیرعددی
    [Fact]
    public void IsValidNationalCode_NonNumericCode_ThrowsException()
    {
        // توضیح: بررسی می‌کند که آیا متد IsValidNationalCode برای کد با کاراکتر غیرعددی استثنا پرتاب می‌کند
        string invalidCode = "123456789a";
        var exception = Assert.Throws<Exception>(() => NationalCode.IsValidNationalCode(invalidCode));
        Assert.Equal("کد ملی تشکیل شده از ده رقم عددی می‌باشد؛ لطفا کد ملی را صحیح وارد نمایید", exception.Message);
    }

    // تست کد ملی با رقم کنترل نادرست
    [Fact]
    public void IsValidNationalCode_InvalidControlDigit_ReturnsFalse()
    {
        // توضیح: بررسی می‌کند که آیا متد IsValidNationalCode برای کد با رقم کنترل نادرست مقدار false برمی‌گرداند
        string invalidCode = "0014401508"; // رقم کنترل نادرست (باید 9 باشد نه 8)
        bool result = NationalCode.IsValidNationalCode(invalidCode);
        Assert.False(result);
    }
}