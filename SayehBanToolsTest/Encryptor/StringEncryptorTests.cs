using SayehBanTools.Encryptor;

namespace SayehBanToolsTest.Encryptor;
/// <summary>
/// تست رمزنگاری و رمزگشایی
/// </summary>
public class StringEncryptorTests
{
    private const string TestPassPhrase = "MySecurePassPhrase123";
    private const string TestInitVector = "1234567890123456"; // دقیقاً 16 بایت
    private const string TestPlainText = "Hello, World!";

    // تست رمزگذاری و رمزگشایی رشته
    [Fact]
    public async Task EncryptAndDecryptAsync_ValidInput_ReturnsOriginalText()
    {
        // توضیح: بررسی می‌کند که آیا متد EncryptAsync رشته را رمزگذاری کرده و متد DecryptAsync همان رشته اصلی را بازمی‌گرداند
        string encrypted = await StringEncryptor.EncryptAsync(TestPlainText, TestInitVector, TestPassPhrase);
        string decrypted = await StringEncryptor.DecryptAsync(encrypted, TestInitVector, TestPassPhrase);
        Assert.Equal(TestPlainText, decrypted);
    }

    // تست رمزگذاری با ورودی خالی
    [Fact]
    public async Task EncryptAsync_EmptyPlainText_ThrowsArgumentNullException()
    {
        // توضیح: بررسی می‌کند که آیا متد EncryptAsync برای رشته ورودی خالی استثنای ArgumentNullException پرتاب می‌کند
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            StringEncryptor.EncryptAsync("", TestInitVector, TestPassPhrase));
    }

    // تست رمزگذاری با وکتور اولیه خالی
    [Fact]
    public async Task EncryptAsync_EmptyInitVector_ThrowsArgumentNullException()
    {
        // توضیح: بررسی می‌کند که آیا متد EncryptAsync برای وکتور اولیه خالی استثنای ArgumentNullException پرتاب می‌کند
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            StringEncryptor.EncryptAsync(TestPlainText, "", TestPassPhrase));
    }

    // تست رمزگذاری با عبارت عبور خالی
    [Fact]
    public async Task EncryptAsync_EmptyPassPhrase_ThrowsArgumentNullException()
    {
        // توضیح: بررسی می‌کند که آیا متد EncryptAsync برای عبارت عبور خالی استثنای ArgumentNullException پرتاب می‌کند
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            StringEncryptor.EncryptAsync(TestPlainText, TestInitVector, ""));
    }

    // تست رمزگشایی با ورودی نامعتبر (Base64 نامعتبر)
    [Fact]
    public async Task DecryptAsync_InvalidBase64_ThrowsFormatException()
    {
        // توضیح: بررسی می‌کند که آیا متد DecryptAsync برای رشته رمزنگاری‌شده نامعتبر (غیر Base64) استثنای FormatException پرتاب می‌کند
        await Assert.ThrowsAsync<FormatException>(() =>
            StringEncryptor.DecryptAsync("InvalidBase64!", TestInitVector, TestPassPhrase));
    }

    // تست رمزگشایی رشته اتصال بدون جداکننده
    [Fact]
    public async Task DecryptConnectionStringAsync_NoSeparator_DecryptsCorrectly()
    {
        // توضیح: بررسی می‌کند که آیا متد DecryptConnectionStringAsync برای رشته بدون جداکننده، رمزگشایی را به‌درستی انجام می‌دهد
        string encrypted = await StringEncryptor.EncryptAsync(TestPlainText, TestInitVector, TestPassPhrase);
        string result = await StringEncryptor.DecryptConnectionStringAsync(encrypted, TestInitVector, TestPassPhrase);
        Assert.Equal(TestPlainText, result);
    }

    // تست رمزگشایی رشته اتصال با جداکننده
    [Fact]
    public async Task DecryptConnectionStringAsync_WithSeparator_DecryptsWithPrefix()
    {
        // توضیح: بررسی می‌کند که آیا متد DecryptConnectionStringAsync برای رشته با جداکننده، پیشوند را حفظ کرده و رمزگشایی را انجام می‌دهد
        string prefix = "prefix:";
        string encrypted = await StringEncryptor.EncryptAsync(TestPlainText, TestInitVector, TestPassPhrase);
        string input = $"{prefix}||{encrypted}";
        string result = await StringEncryptor.DecryptConnectionStringAsync(input, TestInitVector, TestPassPhrase);
        Assert.Equal($"{prefix}{TestPlainText}", result);
    }

    // تست رمزگشایی رشته اتصال با ورودی رمزنگاری‌شده نامعتبر
    [Fact]
    public async Task DecryptConnectionStringAsync_InvalidCipherText_ReturnsEmptyString()
    {
        // توضیح: بررسی می‌کند که آیا متد DecryptConnectionStringAsync برای رشته رمزنگاری‌شده نامعتبر، رشته خالی بازمی‌گرداند
        string input = "prefix:||InvalidBase64!";
        string result = await StringEncryptor.DecryptConnectionStringAsync(input, TestInitVector, TestPassPhrase);
        Assert.Equal("", result);
    }

    // تست وکتور اولیه با طول نامعتبر
    [Fact]
    public async Task EncryptAsync_InvalidIVLength_ThrowsArgumentException()
    {
        // توضیح: بررسی می‌کند که آیا متد EncryptAsync برای وکتور اولیه با طول نامناسب (غیر 16 بایت) استثنای ArgumentException پرتاب می‌کند
        string invalidIV = "12345"; // کمتر از 16 بایت
        await Assert.ThrowsAsync<ArgumentException>(() =>
            StringEncryptor.EncryptAsync(TestPlainText, invalidIV, TestPassPhrase));
    }

    // تست تولید کلید از عبارت عبور
    [Fact]
    public async Task EncryptAsync_DifferentPassPhrases_ProducesDifferentEncryptedOutput()
    {
        // توضیح: بررسی می‌کند که آیا استفاده از عبارت‌های عبور متفاوت، خروجی‌های رمزنگاری متفاوتی تولید می‌کند
        string passPhrase1 = "PassPhrase1";
        string passPhrase2 = "PassPhrase2";
        string encrypted1 = await StringEncryptor.EncryptAsync(TestPlainText, TestInitVector, passPhrase1);
        string encrypted2 = await StringEncryptor.EncryptAsync(TestPlainText, TestInitVector, passPhrase2);
        Assert.NotEqual(encrypted1, encrypted2);
    }
}