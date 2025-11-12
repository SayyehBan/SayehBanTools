using System.Security.Cryptography;
using System.Text;
namespace SayehBanTools.Encryptor;
/// <summary>
/// کلاس مدیریت رمزگذاری
/// </summary>
public static class StringEncryptor
{
    private static readonly int KeySize = 256; // اندازه کلید 256 بیت
    private static readonly int BlockSize = 128; // اندازه بلوک AES همیشه 128 بیت است

    /// <summary>
    /// رمزگذاری رشته
    /// </summary>
    /// <param name="plainText">متن ورودی</param>
    /// <param name="initVector">وکتور اولیه (باید 16 بایت باشد)</param>
    /// <param name="passPhrase">عبارت عبور</param>
    /// <returns>رشته رمزنگاری‌شده به‌صورت Base64</returns>
    public static async Task<string> EncryptAsync(string plainText, string initVector, string passPhrase)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(initVector))
            throw new ArgumentNullException(nameof(initVector));
        if (string.IsNullOrEmpty(passPhrase))
            throw new ArgumentNullException(nameof(passPhrase));

        byte[] ivBytes = GetValidIV(initVector);
        byte[] keyBytes = DeriveKeyFromPassPhrase(passPhrase);
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7; // پدینگ استاندارد
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                await cs.WriteAsync(plainBytes, 0, plainBytes.Length);
                await cs.FlushFinalBlockAsync();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    /// <summary>
    /// رمزگشایی رشته
    /// </summary>
    /// <param name="cipherText">رشته رمزنگاری‌شده (Base64)</param>
    /// <param name="initVector">وکتور اولیه</param>
    /// <param name="passPhrase">عبارت عبور</param>
    /// <returns>رشته رمزگشایی‌شده</returns>
    public static async Task<string> DecryptAsync(string cipherText, string initVector, string passPhrase)
    {
        if (string.IsNullOrEmpty(cipherText))
            throw new ArgumentNullException(nameof(cipherText));
        if (string.IsNullOrEmpty(initVector))
            throw new ArgumentNullException(nameof(initVector));
        if (string.IsNullOrEmpty(passPhrase))
            throw new ArgumentNullException(nameof(passPhrase));

        byte[] ivBytes = GetValidIV(initVector);
        byte[] keyBytes = DeriveKeyFromPassPhrase(passPhrase);
        byte[] cipherBytes = Convert.FromBase64String(cipherText);

        using (Aes aes = Aes.Create())
        {
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = keyBytes;
            aes.IV = ivBytes;

            using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (MemoryStream ms = new MemoryStream(cipherBytes))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (StreamReader sr = new StreamReader(cs))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }

    /// <summary>
    /// رمزگشایی رشته اتصال
    /// </summary>
    /// <param name="plainText">رشته ورودی</param>
    /// <param name="initVector">وکتور اولیه</param>
    /// <param name="passPhrase">عبارت عبور</param>
    /// <returns>رشته رمزگشایی‌شده</returns>
    public static async Task<string> DecryptConnectionStringAsync(string plainText, string initVector, string passPhrase)
    {
        int separatorIndex = plainText.IndexOf("||", StringComparison.Ordinal);
        if (separatorIndex == -1)
        {
            return await DecryptAsync(plainText, initVector, passPhrase);
        }

        string prefix = plainText.Substring(0, separatorIndex);
        string cipherText = plainText.Substring(separatorIndex + 2);

        try
        {
            string decrypted = await DecryptAsync(cipherText, initVector, passPhrase);
            return prefix + decrypted;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// تولید IV معتبر از رشته ورودی
    /// </summary>
    private static byte[] GetValidIV(string initVector)
    {
        if (string.IsNullOrEmpty(initVector))
            throw new ArgumentNullException(nameof(initVector));

        byte[] ivBytes = Encoding.UTF8.GetBytes(initVector);
        if (ivBytes.Length != BlockSize / 8)
            throw new ArgumentException("IV must be exactly 16 bytes for AES.", nameof(initVector));

        return ivBytes;
    }

    /// <summary>
    /// تولید کلید امن از عبارت عبور با PBKDF2
    /// </summary>
    private static byte[] DeriveKeyFromPassPhrase(string passPhrase)
    {
        // استفاده از PBKDF2 با متد جدید
        return Rfc2898DeriveBytes.Pbkdf2(
            password: passPhrase,
            salt: Array.Empty<byte>(),       // اگر salt نداری، خالی بذار
            iterations: 100_000,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: KeySize / 8);      // 32 بایت برای AES-256
    }
}
/*
 طریقه استفاده از دستور
string encryptedText =await StringEncryptor.EncryptAsync(plainText, initVector, passPhrase);
string decryptedText =await StringEncryptor.DecryptAsync(encryptedText, initVector, passPhrase);

 */
