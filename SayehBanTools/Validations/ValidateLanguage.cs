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
    /// <param name="client">یک نمونه RestClient اختیاری برای اهداف تستی.</param>
    /// <returns>یک Task که نشان‌دهنده تکمیل عملیات است.</returns>
    /// <exception cref="InvalidOperationException">در صورت خطا در دریافت داده از API.</exception>
    /// <exception cref="ArgumentException">در صورتی که کد زبان معتبر نباشد.</exception>
    public static async Task ValidateLanguageCodeViaApiAsync(string apiLink, string languageCode, IRestClient? client = null)
    {
        // اگر client null باشد، یک RestClient جدید ایجاد می‌کند
        // در غیر این صورت، از client ارسالی استفاده می‌کند (که برای تست‌ها مفید است)
        client ??= new RestClient(new RestClientOptions(apiLink ?? "http://localhost:90")
        {
            Timeout = TimeSpan.FromSeconds(10)
        });

        var request = new RestRequest("api/LanguagesCode/LanguagesCodeGetAll", Method.Get);
        try
        {
            var response = await client.ExecuteAsync<List<Language>>(request);

            // !!! تغییر در این قسمت !!!
            // اگر درخواست ناموفق بود یا داده‌ای برنگشت، ErrorMessage از RestResponse را هم لحاظ می‌کنیم.
            if (!response.IsSuccessful || response.Data == null || !response.Data.Any())
            {
                string errorMessage = $"خطا در دریافت داده از API کدهای زبان. کد وضعیت: {response.StatusCode}";
                if (!string.IsNullOrEmpty(response.ErrorMessage))
                {
                    errorMessage += $" - پیام خطا: {response.ErrorMessage}";
                }
                throw new InvalidOperationException(errorMessage);
            }

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
            // این بلوک catch برای خطاهای غیرمنتظره مانند مشکلات شبکه قبل از دریافت پاسخ از RestSharp است.
            // در این حالت، ex.Message حاوی پیام خطای اصلی خواهد بود.
            string errorMessage = $"خطا در دریافت داده از API: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $" - Inner Exception: {ex.InnerException.Message}";
            }
            throw new InvalidOperationException(errorMessage, ex);
        }
    }
}