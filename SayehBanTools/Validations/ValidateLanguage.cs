using RestSharp;
using SayehBanTools.Model.Entities;
using System.Text.RegularExpressions;

namespace SayehBanTools.Validations;
/// <summary>
/// کلاس بررسی صحت فرمت کد زبان.
/// </summary>
public class ValidateLanguage
{
    /// <summary>
    /// بررسی صحت فرمت کد زبان.
    /// </summary>
    /// <param name="languageCode">کد زبان برای بررسی (باید دو کاراکتر حروفی باشد).</param>
    /// <exception cref="ArgumentException">در صورتی که کد زبان معتبر نباشد.</exception>
    public static void ValidateLanguageCodeFormat(string languageCode)
    {
        if (string.IsNullOrEmpty(languageCode) || languageCode.Length != 2 ||
            !Regex.IsMatch(languageCode, @"^[a-zA-Z]{2}$"))
            throw new ArgumentException("کد زبان باید دقیقاً دو کاراکتر حروفی باشد.", nameof(languageCode));
    }

    /// <summary>
    /// اعتبارسنجی کد زبان از طریق درخواست به API.
    /// </summary>
    /// <param name="apiLink">آدرس API برای دریافت کدهای زبان (پیش‌فرض: http://localhost:90).</param>
    /// <param name="languageCode">کد زبان مورد نظر برای اعتبارسنجی.</param>
    /// <returns>یک Task که نشان‌دهنده تکمیل عملیات است.</returns>
    /// <exception cref="InvalidOperationException">در صورت خطا در دریافت داده از API.</exception>
    /// <exception cref="ArgumentException">در صورتی که کد زبان معتبر نباشد.</exception>
    public static async Task ValidateLanguageCodeViaApiAsync(string apiLink, string languageCode)
    {
        var client = new RestClient(new RestClientOptions(apiLink ?? "http://localhost:90")
        {
            Timeout = TimeSpan.FromSeconds(10)
        });
        var request = new RestRequest("api/LanguagesCode/LanguagesCodeGetAll", Method.Get);
        try
        {
            var response = await client.ExecuteAsync<List<Language>>(request);
            if (!response.IsSuccessful || response.Data == null || !response.Data.Any())
                throw new InvalidOperationException($"خطا در دریافت داده از API کدهای زبان. کد وضعیت: {response.StatusCode}");

            if (!response.Data.Any(l => l.LanguageCode != null &&
                l.LanguageCode.Equals(languageCode, StringComparison.OrdinalIgnoreCase)))
                throw new ArgumentException("کد زبان معتبر نیست.", nameof(languageCode));
        }
        catch (ArgumentException)
        {
            throw; // استثنای ArgumentException را مستقیماً پرتاب کن
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"خطا در دریافت داده از API: {ex.Message}", ex);
        }
    }
}