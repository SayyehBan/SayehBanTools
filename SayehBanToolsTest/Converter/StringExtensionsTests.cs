using SayehBanTools.Converter;

namespace SayehBanToolsTest.Converter;
public class StringExtensionsTests
{
    // تست نرمال‌سازی رشته به اعداد فارسی
    [Fact]
    public void NormalizeByLanguage_ToPersianDigits_ConvertsEnglishToPersian()
    {
        // توضیح: بررسی می‌کند که آیا متد NormalizeByLanguage اعداد انگلیسی را به اعداد فارسی تبدیل می‌کند
        string input = "123";
        string expected = "۱۲۳";
        string result = input.NormalizeByLanguage("fa", true);
        Assert.Equal(expected, result);
    }

    // تست نرمال‌سازی رشته به اعداد انگلیسی
    [Fact]
    public void NormalizeByLanguage_ToEnglishDigits_ConvertsPersianToEnglish()
    {
        // توضیح: بررسی می‌کند که آیا متد NormalizeByLanguage اعداد فارسی را به اعداد انگلیسی تبدیل می‌کند
        string input = "۱۲۳";
        string expected = "123";
        string result = input.NormalizeByLanguage("fa", false);
        Assert.Equal(expected, result);
    }

    // تست نرمال‌سازی رشته خالی
    [Fact]
    public void NormalizeByLanguage_EmptyString_ReturnsDefaultDigit()
    {
        // توضیح: بررسی می‌کند که آیا برای رشته خالی، مقدار پیش‌فرض (رقم صفر) برای زبان مشخص‌شده بازمی‌گردد
        string input = "";
        string expected = "۰";
        string result = input.NormalizeByLanguage("fa", true);
        Assert.Equal(expected, result);
    }

    // تست نرمال‌سازی با زبان ناشناخته
    [Fact]
    public void NormalizeByLanguage_UnknownLanguage_UsesDefaultRules()
    {
        // توضیح: بررسی می‌کند که آیا برای کد زبان ناشناخته، قوانین پیش‌فرض اعمال می‌شوند
        string input = "۱۲۳";
        string expected = "123";
        string result = input.NormalizeByLanguage("xx", false);
        Assert.Equal(expected, result);
    }

    // تست پاک‌سازی رشته
    [Fact]
    public void CleanString_RemovesUnwantedCharactersAndTrims()
    {
        // توضیح: بررسی می‌کند که آیا متد CleanString کاراکترهای ناخواسته را حذف و رشته را تریم می‌کند
        string input = " ۱۲۳۴ ";
        string expected = "1234";
        string result = input.CleanString("fa");
        Assert.Equal(expected, result);
    }

    // تست تبدیل به عدد صحیح
    [Fact]
    public void ToInt_ConvertsPersianNumberToInt()
    {
        // توضیح: بررسی می‌کند که آیا متد ToInt رشته حاوی اعداد فارسی را به عدد صحیح تبدیل می‌کند
        string input = "۱۲۳";
        int expected = 123;
        int result = input.ToInt("fa");
        Assert.Equal(expected, result);
    }

    // تست تبدیل به عدد صحیح با ورودی نامعتبر
    [Fact]
    public void ToInt_EmptyString_ThrowsArgumentNullException()
    {
        // توضیح: بررسی می‌کند که آیا متد ToInt برای رشته خالی، استثنای مناسب را پرتاب می‌کند
        string input = "";
        Assert.Throws<ArgumentNullException>(() => input.ToInt("fa"));
    }

    // تست تبدیل به عدد اعشاری
    [Fact]
    public void ToDecimal_ConvertsPersianNumberToDecimal()
    {
        // توضیح: بررسی می‌کند که آیا متد ToDecimal رشته حاوی اعداد فارسی را به عدد اعشاری تبدیل می‌کند
        string input = "۱۲۳٫۴۵";
        decimal expected = 123.45m;
        decimal result = input.ToDecimal("fa");
        Assert.Equal(expected, result);
    }

    // تست فرمت‌بندی عدد صحیح به‌صورت عددی
    [Fact]
    public void ToNumeric_Int_FormatsWithPersianSeparators()
    {
        // توضیح: بررسی می‌کند که آیا متد ToNumeric برای عدد صحیح، جداکننده‌های مناسب را برای زبان فارسی اعمال می‌کند
        int input = 123456;
        string expected = "۱۲۳٬۴۵۶";
        string result = input.ToNumeric("fa");
        Assert.Equal(expected, result);
    }

    // تست فرمت‌بندی عدد اعشاری به‌صورت ارز
    [Fact]
    public void ToCurrency_Decimal_FormatsWithPersianCurrency()
    {
        // توضیح: بررسی می‌کند که آیا متد ToCurrency برای عدد اعشاری، فرمت ارز مناسب را برای زبان فارسی اعمال می‌کند
        decimal input = 123456.78m;
        string result = input.ToCurrency("fa");
        Assert.Contains("۱۲۳٬۴۵۷", result); // جداکننده فارسی و بخشی از عدد
        Assert.Contains("ریال", result); // بررسی واحد ارز
    }

    // تست حذف جداکننده‌ها
    [Fact]
    public void RemovePoint_RemovesSeparators()
    {
        // توضیح: بررسی می‌کند که آیا متد RemovePoint جداکننده‌های نقطه، کاما و اسلش را حذف می‌کند
        string input = "1,234.56/78";
        string expected = "12345678";
        string result = input.RemovePoint();
        Assert.Equal(expected, result);
    }

    // تست پاک‌سازی پیام خطا
    [Fact]
    public void Msg_CleansErrorMessage()
    {
        // توضیح: بررسی می‌کند که آیا متد Msg پیشوندهای ناخواسته را حذف و رشته را پاک‌سازی می‌کند
        string input = "Core .Net SqlClient Data Provider-Error 123";
        string expected = "Error 123";
        string result = input.Msg("en");
        Assert.Equal(expected, result);
    }

    // تست بررسی وجود مقدار
    [Fact]
    public void HasValue_WithWhitespace_ReturnsFalseWhenIgnoringWhitespace()
    {
        // توضیح: بررسی می‌کند که آیا متد HasValue برای رشته فقط حاوی فضای خالی، مقدار false برمی‌گرداند
        string input = "   ";
        bool result = input.HasValue(true);
        Assert.False(result);
    }

    [Fact, Trait("ContentType", "text/plain")]
    public void NullIfEmpty_EmptyString_ReturnsNull()
    {
        // توضیح: بررسی می‌کند که آیا متد NullIfEmpty برای رشته خالی، مقدار null برمی‌گرداند
        string input = "";
        string? result = input.NullIfEmpty();
        Assert.Null(result);
    }

    // تست حذف تگ‌های HTML
    [Fact]
    public void HtmlTags_RemovesHtmlTags()
    {
        // توضیح: بررسی می‌کند که آیا متد HtmlTags تگ‌های HTML و کاراکترهای خاص را حذف می‌کند
        string input = "<p>Hello</p> «World»";
        string expected = "Hello World";
        string result = input.HtmlTags();
        Assert.Equal(expected, result);
    }

    // تست جایگزینی خطوط جدید با فاصله
    [Fact]
    public void ASCII_ReplacesNewlinesWithSpace()
    {
        // توضیح: بررسی می‌کند که آیا متد ASCII خطوط جدید را با فاصله جایگزین می‌کند
        string input = "Hello\r\nWorld";
        string expected = "Hello World";
        string result = input.ASCII();
        Assert.Equal(expected, result);
    }

    // تست تبدیل فاصله به خط‌تیره
    [Fact]
    public void Slug_ReplacesSpaceWithHyphen()
    {
        // توضیح: بررسی می‌کند که آیا متد Slug فاصله‌ها را با خط‌تیره جایگزین می‌کند
        string input = "Hello World";
        string expected = "Hello-World";
        string result = input.Slug();
        Assert.Equal(expected, result);
    }

    // تست حذف خط‌تیره و تبدیل به فاصله
    [Fact]
    public void RemoveSlug_ReplacesHyphenWithSpace()
    {
        // توضیح: بررسی می‌کند که آیا متد RemoveSlug خط‌تیره‌ها را با فاصله جایگزین می‌کند
        string input = "Hello-World";
        string expected = "Hello World";
        string result = input.RemoveSlug();
        Assert.Equal(expected, result);
    }

    // تست حذف پیشوند wwwroot/
    [Fact]
    public void RemoveDirectWWWROOT_RemovesWwwrootPrefix()
    {
        // توضیح: بررسی می‌کند که آیا متد RemoveDirectWWWROOT پیشوند wwwroot/ را حذف می‌کند
        string input = "wwwroot/images/test.jpg";
        string expected = "images/test.jpg";
        string? result = input.RemoveDirectWWWROOT(); // Allow result to be nullable
        Assert.Equal(expected, result ?? string.Empty); // Handle null case with a fallback
    }

    // تست جایگزینی ::1 با 127.0.0.1
    [Fact]
    public void AnonymousIP_ReplacesLocalhostIPv6()
    {
        // توضیح: بررسی می‌کند که آیا متد AnonymousIP آدرس ::1 را با 127.0.0.1 جایگزین می‌کند
        string input = "::1";
        string expected = "127.0.0.1";
        string result = input.AnonymousIP();
        Assert.Equal(expected, result);
    }

    // تست متدهای قدیمی En2Fa
    [Fact]
    public void En2Fa_ConvertsToPersianDigits()
    {
        // توضیح: بررسی می‌کند که آیا متد En2Fa اعداد انگلیسی را به فارسی تبدیل می‌کند
        string input = "123";
        string expected = "۱۲۳";
        string result = input.En2Fa();
        Assert.Equal(expected, result);
    }

    // تست متدهای قدیمی Fa2En
    [Fact]
    public void Fa2En_ConvertsToEnglishDigits()
    {
        // توضیح: بررسی می‌کند که آیا متد Fa2En اعداد فارسی را به انگلیسی تبدیل می‌کند
        string input = "۱۲۳";
        string expected = "123";
        string result = input.Fa2En();
        Assert.Equal(expected, result);
    }

    // تست فیکس کردن کاراکترهای فارسی
    [Fact]
    public void FixPersianChars_NormalizesPersianCharacters()
    {
        // توضیح: بررسی می‌کند که آیا متد FixPersianChars کاراکترهای فارسی را نرمال‌سازی می‌کند
        string input = "كيومرث";
        string expected = "کیومرث";
        string result = input.FixPersianChars();
        Assert.Equal(expected, result);
    }
}