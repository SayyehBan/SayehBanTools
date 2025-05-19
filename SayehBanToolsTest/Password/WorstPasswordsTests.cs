using SayehBanTools.Password;
using System.Security.Cryptography;
using System.Text;
namespace SayehBanToolsTest.Password;
public class WorstPasswordsTests
{
    private readonly WorstPasswords _worstPasswords;

    public WorstPasswordsTests()
    {
        _worstPasswords = new WorstPasswords();
    }

    /// <summary>
    /// تست بررسی عملکرد محاسبه هش SHA256
    /// </summary>
    [Fact]
    public void ComputeSha256Hash_ShouldReturnCorrectHash()
    {
        // Arrange
        string input = "testPassword";
        string expectedHash = GetExpectedHash(input); // محاسبه هش به صورت دستی برای مقایسه

        // Act
        string actualHash = _worstPasswords.ComputeSha256Hash(input);

        // Assert
        Assert.Equal(expectedHash, actualHash);
    }

    /// <summary>
    /// تست بررسی عملکرد لود کردن پسوردهای ضعیف از فایل
    /// </summary>
    [Fact]
    public async Task LoadCommonPasswords_ShouldLoadHashedPasswords()
    {
        // Arrange
        string testFilePath = "test_passwords.txt";
        File.WriteAllText(testFilePath, "password1\npassword2\npassword3");

        // Act
        await _worstPasswords.LoadCommonPasswords(testFilePath);

        // Assert
        Assert.Equal(3, _worstPasswords.CommonPassword.Count);
        Assert.Contains(_worstPasswords.ComputeSha256Hash("password1"), _worstPasswords.CommonPassword);
        Assert.Contains(_worstPasswords.ComputeSha256Hash("password2"), _worstPasswords.CommonPassword);
        Assert.Contains(_worstPasswords.ComputeSha256Hash("password3"), _worstPasswords.CommonPassword);

        // Cleanup
        File.Delete(testFilePath);
    }

    /// <summary>
    /// تست بررسی عدم لود مجدد پسوردها اگر قبلا لود شده‌اند
    /// </summary>
    [Fact]
    public async Task LoadCommonPasswords_ShouldNotReloadIfAlreadyLoaded()
    {
        // Arrange
        string testFilePath = "test_passwords.txt";
        File.WriteAllText(testFilePath, "password1\npassword2");
        await _worstPasswords.LoadCommonPasswords(testFilePath);
        int initialCount = _worstPasswords.CommonPassword.Count;

        // Act
        await _worstPasswords.LoadCommonPasswords(testFilePath); // تلاش برای لود مجدد

        // Assert
        Assert.Equal(initialCount, _worstPasswords.CommonPassword.Count);

        // Cleanup
        File.Delete(testFilePath);
    }

    /// <summary>
    /// تست بررسی رفتار متد با فایل خالی
    /// </summary>
    [Fact]
    public async Task LoadCommonPasswords_ShouldHandleEmptyFile()
    {
        // Arrange
        string testFilePath = "empty_test_passwords.txt";
        File.WriteAllText(testFilePath, string.Empty);

        // Act
        await _worstPasswords.LoadCommonPasswords(testFilePath);

        // Assert
        Assert.Empty(_worstPasswords.CommonPassword);

        // Cleanup
        File.Delete(testFilePath);
    }

    private string GetExpectedHash(string input)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = sha256.ComputeHash(inputBytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++)
            {
                builder.Append(hashedBytes[i].ToString("x2"));
            }

            return builder.ToString();
        }
    }
}