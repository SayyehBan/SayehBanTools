using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RestSharp;
using SayehBanTools.Model.Entities;
using SayehBanTools.Validations;

namespace SayehBanTools.Tests
{
    public class ValidateLanguageTests
    {
        #region ValidateLanguageCodeFormat Tests
        // این بخش شامل تست‌هایی برای متد ValidateLanguageCodeFormat است که فرمت کد زبان را بررسی می‌کند.

        /// <summary>
        /// تست بررسی کد زبان معتبر (مثل "en"): متد ValidateLanguageCodeFormat نباید استثنا پرتاب کند.
        /// </summary>
        [Fact]
        public void ValidateLanguageCodeFormat_ValidCode_DoesNotThrow()
        {
            // Arrange
            string languageCode = "en"; // کد زبان معتبر (دو کاراکتر حروفی)

            // Act & Assert
            ValidateLanguage.ValidateLanguageCodeFormat(languageCode);
            // اگر استثنایی پرتاب نشود، تست موفق است.
        }

        /// <summary>
        /// تست بررسی کد زبان نال: متد ValidateLanguageCodeFormat باید استثنای ArgumentException پرتاب کند.
        /// </summary>
        [Fact]
        public void ValidateLanguageCodeFormat_NullCode_ThrowsArgumentException()
        {
            // Arrange
            string? languageCode = null; // کد زبان نال

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode!));
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر انگلیسی حروفی باشد.", exception.Message);
            Assert.Equal(nameof(languageCode), exception.ParamName);
        }

        /// <summary>
        /// تست بررسی کد زبان خالی: متد ValidateLanguageCodeFormat باید استثنای ArgumentException پرتاب کند.
        /// </summary>
        [Fact]
        public void ValidateLanguageCodeFormat_EmptyCode_ThrowsArgumentException()
        {
            // Arrange
            string languageCode = ""; // کد زبان خالی

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode));
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر انگلیسی حروفی باشد.", exception.Message);
            Assert.Equal(nameof(languageCode), exception.ParamName);
        }

        /// <summary>
        /// تست بررسی کد زبان با طول نادرست (مثل "eng"): متد ValidateLanguageCodeFormat باید استثنای ArgumentException پرتاب کند.
        /// </summary>
        [Fact]
        public void ValidateLanguageCodeFormat_WrongLengthCode_ThrowsArgumentException()
        {
            // Arrange
            string languageCode = "eng"; // کد زبان با طول نادرست (بیش از دو کاراکتر)

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode));
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر انگلیسی حروفی باشد.", exception.Message);
            Assert.Equal(nameof(languageCode), exception.ParamName);
        }

        /// <summary>
        /// تست بررسی کد زبان غیرحروفی (مثل "12"): متد ValidateLanguageCodeFormat باید استثنای ArgumentException پرتاب کند.
        /// </summary>
        [Fact]
        public void ValidateLanguageCodeFormat_NonAlphabeticCode_ThrowsArgumentException()
        {
            // Arrange
            string languageCode = "12"; // کد زبان غیرحروفی

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode));
            Assert.Contains("کد زبان باید دقیقاً دو کاراکتر انگلیسی حروفی باشد.", exception.Message);
            Assert.Equal(nameof(languageCode), exception.ParamName);
        }

        /// <summary>
        /// تست بررسی کد زبان با پیام خطای سفارشی: متد ValidateLanguageCodeFormat باید استثنای ArgumentException با پیام سفارشی پرتاب کند.
        /// </summary>
        [Fact]
        public void ValidateLanguageCodeFormat_InvalidCodeWithCustomMessage_ThrowsArgumentExceptionWithCustomMessage()
        {
            // Arrange
            string languageCode = "12"; // کد زبان غیرحروفی
            string customMessage = "کد زبان نامعتبر است!";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                ValidateLanguage.ValidateLanguageCodeFormat(languageCode, customMessage));
            Assert.Contains(customMessage, exception.Message);
            Assert.Equal(nameof(languageCode), exception.ParamName);
        }

        #endregion

        #region ValidateLanguageCodeViaApiAsync Tests
        // این بخش شامل تست‌هایی برای متد ValidateLanguageCodeViaApiAsync است که کد زبان را از طریق API بررسی می‌کند.

        // متد کمکی برای تولید داده‌های نمونه زبان‌ها
        private static List<VM_Language.LanguagesCodeGetAll> GetSampleLanguages()
        {
            // لیستی از اشیاء Language با داده‌های نمونه (زبان‌های فارسی، انگلیسی، امهری) برمی‌گرداند
            return new List<VM_Language.LanguagesCodeGetAll>
            {
                new VM_Language.LanguagesCodeGetAll { LanguageID = 1, LanguageCode = "fa", LanguageName = "فارسی", LanguageNameFarsi = "فارسی", TextDirection = "RTL" },
                new VM_Language.LanguagesCodeGetAll { LanguageID = 2, LanguageCode = "en", LanguageName = "English", LanguageNameFarsi = "انگلیسی", TextDirection = "LTR" },
                new VM_Language.LanguagesCodeGetAll { LanguageID = 32, LanguageCode = "am", LanguageName = "አማርኛ", LanguageNameFarsi = "امهری", TextDirection = "LTR" }
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
                // لاگ برای دیباگ
                Console.WriteLine($"MockHttpMessageHandler: StatusCode={_statusCode}, Content={_responseContent}, ThrowException={_throwException}");

                // اگر پرچم throwException فعال باشد، استثنای HttpRequestException پرتاب می‌کند
                if (_throwException)
                {
                    throw new HttpRequestException(_exceptionMessage ?? "API Error");
                }

                // یک پاسخ HTTP جدید ایجاد می‌کند
                var response = new HttpResponseMessage
                {
                    StatusCode = _statusCode
                };

                // اگر محتوای پاسخ وجود داشته باشد، آن را به پاسخ اضافه می‌کند
                if (_responseContent != null)
                {
                    response.Content = new StringContent(_responseContent, Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
                }
                else
                {
                    response.Content = new StringContent("[]", Encoding.UTF8, new MediaTypeHeaderValue("application/json"));
                }

                return await Task.FromResult(response);
            }
        }

        /// <summary>
        /// تست بررسی کد زبان معتبر از طریق API: متد ValidateLanguageCodeViaApiAsync نباید استثنا پرتاب کند.
        /// </summary>
        [Fact]
        public async Task ValidateLanguageCodeViaApiAsync_ValidCode_DoesNotThrow()
        {
            // Arrange
            string languageCode = "fa";
            string API_Link = "http://localhost:90";
            string API_Address = "api/LanguagesCode/LanguagesCodeGetAll";
            var responseContent = JsonSerializer.Serialize(GetSampleLanguages());
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, responseContent);

            var client = new RestClient(new RestClientOptions(API_Link)
            {
                ConfigureMessageHandler = _ => handler,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // Act & Assert
            await ValidateLanguage.ValidateLanguageCodeViaApiAsync(API_Link, API_Address, languageCode, client);
        }

        /// <summary>
        /// تست بررسی کد زبان با آدرس API نال: متد ValidateLanguageCodeViaApiAsync باید از آدرس پیش‌فرض استفاده کند و بدون استثنا اجرا شود.
        /// </summary>
        [Fact]
        public async Task ValidateLanguageCodeViaApiAsync_NullApiLink_UsesDefaultLink()
        {
            // Arrange
            string languageCode = "en";
            string? API_Link = null;
            string? API_Address = null;
            var responseContent = JsonSerializer.Serialize(GetSampleLanguages());
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, responseContent);

            var client = new RestClient(new RestClientOptions("http://localhost:90")
            {
                ConfigureMessageHandler = _ => handler,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // Act & Assert
            await ValidateLanguage.ValidateLanguageCodeViaApiAsync(API_Link ?? "", API_Address ?? "", languageCode, client);
        }

        /// <summary>
        /// تست بررسی پاسخ ناموفق API (مثل کد 404): متد ValidateLanguageCodeViaApiAsync باید استثنای InvalidOperationException پرتاب کند.
        /// </summary>
        [Fact]
        public async Task ValidateLanguageCodeViaApiAsync_UnsuccessfulResponse_ThrowsInvalidOperationException()
        {
            // Arrange
            string languageCode = "en";
            string API_Link = "http://localhost:90";
            string API_Address = "api/LanguagesCode/LanguagesCodeGetAll";
            var handler = new MockHttpMessageHandler(HttpStatusCode.NotFound);

            var client = new RestClient(new RestClientOptions(API_Link)
            {
                ConfigureMessageHandler = _ => handler,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(API_Link, API_Address, languageCode, client));
            Assert.Contains("خطا در دریافت داده از API کدهای زبان", exception.Message);
            Assert.Contains("کد وضعیت: NotFound", exception.Message);
        }

        /// <summary>
        /// تست بررسی پاسخ API با داده خالی (لیست خالی): متد ValidateLanguageCodeViaApiAsync باید استثنای InvalidOperationException پرتاب کند.
        /// </summary>
        [Fact]
        public async Task ValidateLanguageCodeViaApiAsync_NullResponseData_ThrowsInvalidOperationException()
        {
            // Arrange
            string languageCode = "en";
            string API_Link = "http://localhost:90";
            string API_Address = "api/LanguagesCode/LanguagesCodeGetAll";
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, "[]");

            var client = new RestClient(new RestClientOptions(API_Link)
            {
                ConfigureMessageHandler = _ => handler,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(API_Link, API_Address, languageCode, client));
            Assert.Contains("خطا در دریافت داده از API کدهای زبان", exception.Message);
        }

        /// <summary>
        /// تست بررسی کد زبان نامعتبر: متد ValidateLanguageCodeViaApiAsync باید استثنای ArgumentException پرتاب کند.
        /// </summary>
        [Fact]
        public async Task ValidateLanguageCodeViaApiAsync_InvalidCode_ThrowsArgumentException()
        {
            // Arrange
            string languageCode = "xx";
            string API_Link = "http://localhost:90";
            string API_Address = "api/LanguagesCode/LanguagesCodeGetAll";
            var responseContent = JsonSerializer.Serialize(GetSampleLanguages());
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, responseContent);

            var client = new RestClient(new RestClientOptions(API_Link)
            {
                ConfigureMessageHandler = _ => handler,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(API_Link, API_Address, languageCode, client));
            Assert.Contains("کد زبان معتبر نیست.", exception.Message);
            Assert.Equal(nameof(languageCode), exception.ParamName);
        }

        /// <summary>
        /// تست بررسی پرتاب استثنای API: متد ValidateLanguageCodeViaApiAsync باید استثنای InvalidOperationException پرتاب کند.
        /// </summary>
        [Fact]
        public async Task ValidateLanguageCodeViaApiAsync_ApiThrowsException_ThrowsInvalidOperationException()
        {
            // Arrange
            string languageCode = "en";
            string API_Link = "http://localhost:90";
            string API_Address = "api/LanguagesCode/LanguagesCodeGetAll";
            var handler = new MockHttpMessageHandler(HttpStatusCode.OK, null, true, "API Error during request");

            var client = new RestClient(new RestClientOptions(API_Link)
            {
                ConfigureMessageHandler = _ => handler,
                Timeout = TimeSpan.FromSeconds(10)
            });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await ValidateLanguage.ValidateLanguageCodeViaApiAsync(API_Link, API_Address, languageCode, client));
            Assert.Contains("خطا در دریافت داده از API", exception.Message);
            Assert.Contains("API Error during request", exception.Message);
        }

        #endregion
    }
}