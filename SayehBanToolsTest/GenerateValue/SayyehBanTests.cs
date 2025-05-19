using SayehBanTools.GenerateValue;

namespace SayehBanToolsTest.GenerateValue;
/// <summary>
/// این کلاس برای تست کدهای سایهبن استفاده میشود
/// </summary>
public class SayyehBanTests
{
    [Fact]
    public void IsValidCode_ValidCode_ReturnsTrue()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string validCode = "0010403122014"; // یک کد معتبر فرضی
        bool result = SayyehBan.IsValidCode(validCode);
        Assert.True(result, "کد معتبر باید true برگرداند.");
    }

    [Fact]
    public void IsValidCode_AllDigitsSame_ReturnsFalse()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string invalidCode = "1111111111111";
        bool result = SayyehBan.IsValidCode(invalidCode);
        Assert.False(result, "کدی که تمام ارقام آن یکسان است باید false برگرداند.");
    }

    [Fact]
    public void IsValidCode_NullOrEmpty_ThrowsException()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        var exception = Assert.Throws<Exception>(() => SayyehBan.IsValidCode(""));
        Assert.Equal("لطفا کد سایه‌بان را صحیح وارد نمایید", exception.Message);
    }

    [Fact]
    public void IsValidCode_WrongLength_ThrowsException()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        var exception = Assert.Throws<Exception>(() => SayyehBan.IsValidCode("1234567890"));
        Assert.Equal("طول کد سایه‌بان باید سیزده کاراکتر باشد", exception.Message);
    }

    [Fact]
    public void IsValidCode_NonNumeric_ThrowsException()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        var exception = Assert.Throws<Exception>(() => SayyehBan.IsValidCode("1234567890abc"));
        Assert.Equal("کد سایه‌بان باید شامل سیزده رقم عددی باشد", exception.Message);
    }

    [Fact]
    public void GenerateRandom_Returns13DigitCode()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string code = SayyehBan.GenerateRandom();
        Assert.Equal(13, code.Length);
        Assert.Matches(@"^\d{13}$", code);
    }

    [Fact]
    public void GenerateRandom_ProducesValidCode()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string code = SayyehBan.GenerateRandom();
        bool isValid = SayyehBan.IsValidCode(code);
        Assert.True(isValid, "کد تولیدشده توسط GenerateRandom باید معتبر باشد.");
    }

    [Fact]
    public void GenerateRound_Returns13DigitCode()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string code = SayyehBan.GenerateRound();
        Assert.Equal(13, code.Length);
        Assert.Matches(@"^\d{13}$", code);
    }

    [Fact]
    public void GenerateRound_ProducesValidCode()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string code = SayyehBan.GenerateRound();
        bool isValid = SayyehBan.IsValidCode(code);
        Assert.True(isValid, "کد تولیدشده توسط GenerateRound باید معتبر باشد.");
    }

    [Fact]
    public void GenerateRound_StartsWith00AndUsesLimitedDigits()
    {
        // ترتیب: آماده‌سازی، اجرا، تأیید
        string code = SayyehBan.GenerateRound();
        Assert.StartsWith("00", code);
        for (int i = 2; i < 12; i++)
        {
            int digit = int.Parse(code[i].ToString());
            Assert.InRange(digit, 0, 4);
        }
    }
}