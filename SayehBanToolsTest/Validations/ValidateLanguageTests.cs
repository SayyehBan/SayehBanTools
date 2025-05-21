using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
using SayehBanTools.Model.Entities;
using SayehBanTools.Validations;
using Xunit;

namespace SayehBanTools.Tests
{
    public class ValidateLanguageTests
    {
        #region ValidateLanguageCodeFormat Tests
        // این بخش شامل تست‌هایی برای متد ValidateLanguageCodeFormat است که فرمت کد زبان را بررسی می‌کند.

        [Fact]
        // تست بررسی کد زبان معتبر (مثل "en") که نباید استثنا پرتاب کند.
        public void ValidateLanguageCodeFormat_ValidCode_DoesNotThrow()
        {
            // Arrange: آماده‌سازی داده‌های ورودی
            string languageCode = "en"; // کد زبان معتبر (دو کاراکتر حروفی)

            // Act & Assert: اجرا و بررسی
            ValidateLanguage.ValidateLanguageCodeFormat(languageCode); // فراخوانی متد برای بررسی فرمت
            // اگر استثنایی پرتاب نشود، تست موفق است
        }

        [Fact]
        // تست بررسی کد زبان نال که باید استثنای ArgumentException پرتاب کند.
        public void ValidateLanguageCodeFormat_NullCode_ThrowsArgumentException()
        {
            // Arrange: آماده‌سازی داده‌های ورودی
            string? languageCode = null; // کد زبان نال

            // Act & Assert: اجرا و بررسی
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode!)); // فراخوانی متد با ورودی نال
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر حروفی باشد.", exception.Message); // بررسی پیام استثنا
        }

        [Fact]
        // تست بررسی کد زبان خالی که باید استثنای ArgumentException پرتاب کند.
        public void ValidateLanguageCodeFormat_EmptyCode_ThrowsArgumentException()
        {
            // Arrange: آماده‌سازی داده‌های ورودی
            string languageCode = ""; // کد زبان خالی

            // Act & Assert: اجرا و بررسی
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode)); // فراخوانی متد
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر حروفی باشد.", exception.Message); // بررسی پیام استثنا
        }

        [Fact]
        // تست بررسی کد زبان با طول نادرست (مثل "eng") که باید استثنای ArgumentException پرتاب کند.
        public void ValidateLanguageCodeFormat_WrongLengthCode_ThrowsArgumentException()
        {
            // Arrange: آماده‌سازی داده‌های ورودی
            string languageCode = "eng"; // کد زبان با طول نادرست (بیش از دو کاراکتر)

            // Act & Assert: اجرا و بررسی
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode)); // فراخوانی متد صحیح
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر حروفی باشد.", exception.Message); // بررسی پیام استثنا
        }

        [Fact]
        // تست بررسی کد زبان غیرحروفی (مثل "12") که باید استثنای ArgumentException پرتاب کند.
        public void ValidateLanguageCodeFormat_NonAlphabeticCode_ThrowsArgumentException()
        {
            // Arrange: آماده‌سازی داده‌های ورودی
            string languageCode = "12"; // کد زبان غیرحروفی

            // Act & Assert: اجرا و بررسی
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode)); // فراخوانی متد
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر حروفی باشد.", exception.Message); // بررسی پیام استثنا
        }

        #endregion

        #region ValidateLanguageCodeViaApiAsync Tests
        // این بخش شامل تست‌هایی برای متد ValidateLanguageCodeViaApiAsync است که کد زبان را از طریق API بررسی می‌کند.

        // متد کمکی برای تولید داده‌های نمونه زبان‌ها
        private static List<Language> GetSampleLanguages()
        {
            // لیستی از اشیاء Language با داده‌های نمونه (زبان‌های فارسی، انگلیسی، امهری) برمی‌گرداند
            return new List<Language>
            {
                new Language { LanguageID = 1, LanguageCode = "fa", LanguageName = "فارسی", LanguageNameFarsi = "فارسی", TextDirection = "RTL" },
                new Language { LanguageID = 2, LanguageCode = "en", LanguageName = "English", LanguageNameFarsi = "انگلیسی", TextDirection = "LTR" },
                new Language { LanguageID = 32, LanguageCode = "am", LanguageName = "አማርኛ", LanguageNameFarsi = "امهری", TextDirection = "LTR" }
            };
        }

        // کلاس کمکی برای شبیه‌سازی پاسخ‌های HTTP
        private class MockHttpMessageHandler : HttpMessageHandler
        {
            private readonly HttpStatusCode _statusCode; // کد وضعیت HTTP (مثل 200 یا 404)
            private readonly string? _responseContent; // محتوای پاسخ (مثل JSON)
            private readonly bool _throwException; // آیا باید استثنا پرتاب کند
            private readonly string? _exceptionMessage; // پیام استثنای اختیاری

            // سازنده برای تنظیم مقادیر اولیه
            public MockHttpMessageHandler(HttpStatusCode statusCode, string? responseContent = null, bool throwException = false, string? exceptionMessage = null)
            {
                _statusCode = statusCode;
                _responseContent = responseContent;
                _throwException = throwException;
                _exceptionMessage = exceptionMessage;
            }

            // متد اصلی که پاسخ HTTP را شبیه‌سازی می‌کند
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                // لاگ برای دیباگ: نمایش اطلاعات پاسخ
                Console.WriteLine($"MockHttpMessageHandler: StatusCode={_statusCode}, Content={_responseContent}, ThrowException={_throwException}");

                // اگر پرچم throwException فعال باشد، استثنای HttpRequestException پرتاب می‌کند
                if (_throwException)
                {
                    throw new HttpRequestException(_exceptionMessage ?? "API Error");
                }

                // یک پاسخ HTTP جدید ایجاد می‌کند
                var response = new HttpResponseMessage
                {
                    StatusCode = _statusCode // تنظیم کد وضعیت (مثل 200 یا 404)
                };

                // اگر محتوای پاسخ وجود داشته باشد، آن را به پاسخ اضافه می‌کند
                if (_responseContent != null)
                {
                    response.Content = new StringContent(_responseContent, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
                }
                else
                {
                    // در غیر این صورت، محتوای پاسخ را یک لیست خالی JSON تنظیم می‌کند
                    response.Content = new StringContent("[]", Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
                }

                // پاسخ را به‌صورت ناهمگام برمی‌گرداند
                return await Task.FromResult(response);
            }
        }

        [Fact]
        // تست بررسی کد زبان معتبر از طریق API که نباید استثنا پرتاب کند
        public async Task ValidateLanguageCodeViaApiAsync_ValidCode_DoesNotThrow()
        {
            // Arrange: آماده‌سازی داده‌ها و تنظیمات
            string languageCode = "fa"; // کد زبان معتبر
            string apiLink = "http://localhost:90"; // آدرس پایه API
            var responseContent = JsonSerializer.Serialize(GetSampleLanguages()); // تبدیل داده‌های نمونه به JSON
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, responseContent); // شبیه‌ساز پاسخ HTTP با کد 200 و داده‌های معتبر
            var client = new RestClient(new RestClientOptions(apiLink)
            {
                ConfigureMessageHandler = _ => handler, // استفاده از شبیه‌ساز برای پاسخ‌های HTTP
                Timeout = TimeSpan.FromSeconds(10) // تنظیم زمان انتظار برای درخواست
            });

            // Act & Assert: اجرا و بررسی
            await ValidateLanguage.ValidateLanguageCodeViaApiAsync(apiLink, languageCode); // فراخوانی متد برای بررسی کد زبان
            // اگر استثنایی پرتاب نشود، تست موفق است
        }

        [Fact]
        // تست بررسی کد زبان با آدرس API نال (باید از آدرس پیش‌فرض استفاده کند)
        public async Task ValidateLanguageCodeViaApiAsync_NullApiLink_UsesDefaultLink()
        {
            // Arrange: آماده‌سازی داده‌ها و تنظیمات
            string languageCode = "en"; // کد زبان معتبر
            string? apiLink = null; // آدرس API نال
            var responseContent = JsonSerializer.Serialize(GetSampleLanguages()); // تبدیل داده‌های نمونه به JSON
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, responseContent); // شبیه‌ساز پاسخ HTTP با کد 200
            var client = new RestClient(new RestClientOptions("http://localhost:90")
            {
                ConfigureMessageHandler = _ => handler, // استفاده از شبیه‌ساز
                Timeout = TimeSpan.FromSeconds(10) // تنظیم زمان انتظار
            });

            // Act & Assert: اجرا و بررسی
            await ValidateLanguage.ValidateLanguageCodeViaApiAsync(apiLink, languageCode); // فراخوانی متد با آدرس نال
            // اگر استثنایی پرتاب نشود، تست موفق است
        }

        [Fact]
        // تست بررسی پاسخ ناموفق API (مثل کد 404) که باید استثنای InvalidOperationException پرتاب کند
        public async Task ValidateLanguageCodeViaApiAsync_UnsuccessfulResponse_ThrowsInvalidOperationException()
        {
            // Arrange: آماده‌سازی داده‌ها و تنظیمات
            string languageCode = "en"; // کد زبان
            string apiLink = "http://localhost:90"; // آدرس پایه API
            var handler = new MockHttpMessageHandler(HttpStatusCode.NotFound, null); // شبیه‌ساز پاسخ HTTP با کد 404 (ناموفق)
            var client = new RestClient(new RestClientOptions(apiLink)
            {
                ConfigureMessageHandler = _ => handler, // استفاده از شبیه‌ساز
                Timeout = TimeSpan.FromSeconds(10) // تنظیم زمان انتظار
            });

            // Act & Assert: اجرا و بررسی
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(apiLink, languageCode)); // فراخوانی متد
            Assert.Contains("خطا در دریافت داده از API کدهای زبان", exception.Message); // بررسی پیام استثنا
        }

        [Fact]
        // تست بررسی پاسخ API با داده خالی (لیست خالی) که باید استثنای InvalidOperationException پرتاب کند
        public async Task ValidateLanguageCodeViaApiAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            // Arrange: آماده‌سازی داده‌ها و تنظیمات
            string languageCode = "en"; // کد زبان
            string apiLink = "http://localhost:90"; // آدرس پایه API
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, "[]"); // شبیه‌ساز پاسخ HTTP با کد 200 و لیست خالی
            var client = new RestClient(new RestClientOptions(apiLink)
            {
                ConfigureMessageHandler = _ => handler, // استفاده از شبیه‌ساز
                Timeout = TimeSpan.FromSeconds(10) // تنظیم زمان انتظار
            });

            // Act & Assert: اجرا و بررسی
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(apiLink, languageCode)); // فراخوانی متد
            Assert.Contains("خطا در دریافت داده از API کدهای زبان", exception.Message); // بررسی پیام استثنا
        }

        [Fact]
        // تست بررسی کد زبان نامعتبر که باید استثنای ArgumentException پرتاب کند
        public async Task ValidateLanguageCodeViaApiAsync_InvalidCode_ThrowsArgumentException()
        {
            // Arrange: آماده‌سازی داده‌ها و تنظیمات
            string languageCode = "xx"; // کد زبان نامعتبر
            string apiLink = "http://localhost:90"; // آدرس پایه API
            var responseContent = JsonSerializer.Serialize(GetSampleLanguages()); // تبدیل داده‌های نمونه به JSON
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, responseContent); // شبیه‌ساز پاسخ HTTP با کد 200
            var client = new RestClient(new RestClientOptions(apiLink)
            {
                ConfigureMessageHandler = _ => handler, // استفاده از شبیه‌ساز
                Timeout = TimeSpan.FromSeconds(10) // تنظیم زمان انتظار
            });

            // Act & Assert: اجرا و بررسی
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(apiLink, languageCode)); // فراخوانی متد
            Assert.Contains("کد زبان معتبر نیست.", exception.Message); // بررسی پیام استثنا
        }

        [Fact]
        // تست بررسی پرتاب استثنای API که باید استثنای InvalidOperationException پرتاب کند
        public async Task ValidateLanguageCodeViaApiAsync_ApiThrowsException_ThrowsInvalidOperationException()
        {
            // Arrange: آماده‌سازی داده‌ها و تنظیمات
            string languageCode = "en"; // کد زبان
            string apiLink = "http://localhost:90"; // آدرس پایه API
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, null, true, "API Error"); // شبیه‌ساز پرتاب استثنای HttpRequestException
            var client = new RestClient(new RestClientOptions(apiLink)
            {
                ConfigureMessageHandler = _ => handler, // استفاده از شبیه‌ساز
                Timeout = TimeSpan.FromSeconds(10) // تنظیم زمان انتظار
            });

            // Act & Assert: اجرا و بررسی
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(apiLink, languageCode)); // فراخوانی متد
            Assert.Contains("API Error", exception.Message); // بررسی پیام استثنا
        }

        #endregion
    }
}