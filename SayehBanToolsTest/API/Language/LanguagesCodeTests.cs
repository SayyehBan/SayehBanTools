using Moq;
using RestSharp;
using SayehBanTools.API.Language;
using System;
using Xunit;

namespace SayehBanTools.Tests.API.Language
{
    public class LanguagesCodeTests
    {
        [Fact]
        public void CreateApiRequest_WithDefaultParameters_ReturnsDefaultClientAndRequest()
        {
            // ترتیب (Arrange): تنظیم مقادیر مورد انتظار برای تست
            // هدف: بررسی اینکه وقتی هیچ پارامتری به متد داده نشود، از مقادیر پیش‌فرض استفاده می‌شود
            string expectedApiLink = "http://localhost:90/"; // اضافه کردن اسلش انتهایی
            string expectedApiAddress = "api/LanguagesCode/LanguageCodeList";

            // اجرا (Act): فراخوانی متد مورد تست
            var (client, request) = LanguagesCode.CreateApiRequest();

            // ادعا (Assert): بررسی نتایج
            // اطمینان از اینکه client و request null نیستند
            // بررسی اینکه آدرس API و متد درخواست درست تنظیم شده‌اند
            // بررسی اینکه BaseUrl و Timeout به درستی مقداردهی شده‌اند
            Assert.NotNull(client);
            Assert.NotNull(request);
            Assert.Equal(expectedApiAddress, request.Resource);
            Assert.Equal(Method.Get, request.Method);
            Assert.Equal(expectedApiLink, client.Options.BaseUrl?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(10), client.Options.Timeout);
        }

        [Fact]
        public void CreateApiRequest_WithCustomApiLink_UsesProvidedApiLink()
        {
            // ترتیب (Arrange): تنظیم مقادیر برای تست با apiLink سفارشی
            // هدف: بررسی اینکه وقتی یک apiLink سفارشی ارائه می‌شود، از آن استفاده می‌شود
            string customApiLink = "http://custom.api.com/"; // اضافه کردن اسلش انتهایی
            string expectedApiAddress = "api/LanguagesCode/LanguageCodeList";

            // اجرا (Act): فراخوانی متد با apiLink سفارشی
            var (client, request) = LanguagesCode.CreateApiRequest(apiLink: customApiLink);

            // ادعا (Assert): بررسی اینکه client و request به درستی با apiLink سفارشی ایجاد شده‌اند
            Assert.NotNull(client);
            Assert.NotNull(request);
            Assert.Equal(expectedApiAddress, request.Resource);
            Assert.Equal(Method.Get, request.Method);
            Assert.Equal(customApiLink, client.Options.BaseUrl?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(10), client.Options.Timeout);
        }

        [Fact]
        public void CreateApiRequest_WithCustomApiAddress_UsesProvidedApiAddress()
        {
            // ترتیب (Arrange): تنظیم مقادیر برای تست با apiAddress سفارشی
            // هدف: بررسی اینکه وقتی apiAddress سفارشی ارائه می‌شود، از آن استفاده می‌شود
            string expectedApiLink = "http://localhost:90/"; // اضافه کردن اسلش انتهایی
            string customApiAddress = "api/custom/languages";

            // اجرا (Act): فراخوانی متد با apiAddress سفارشی
            var (client, request) = LanguagesCode.CreateApiRequest(apiAddress: customApiAddress);

            // ادعا (Assert): بررسی اینکه client و request به درستی با apiAddress سفارشی ایجاد شده‌اند
            Assert.NotNull(client);
            Assert.NotNull(request);
            Assert.Equal(customApiAddress, request.Resource);
            Assert.Equal(Method.Get, request.Method);
            Assert.Equal(expectedApiLink, client.Options.BaseUrl?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(10), client.Options.Timeout);
        }

        [Fact]
        public void CreateApiRequest_WithCustomClient_UsesProvidedClient()
        {
            // ترتیب (Arrange): تنظیم یک client موک برای تست
            // هدف: بررسی اینکه وقتی یک client سفارشی ارائه می‌شود، همان client استفاده می‌شود
            var mockClient = new Mock<IRestClient>();
            var options = new RestClientOptions
            {
                BaseUrl = new Uri("http://mock.api.com/"), // اضافه کردن اسلش انتهایی
                Timeout = TimeSpan.FromSeconds(5)
            };
            mockClient.Setup(c => c.Options).Returns(new ReadOnlyRestClientOptions(options));
            string expectedApiAddress = "api/LanguagesCode/LanguageCodeList";

            // اجرا (Act): فراخوانی متد با client سفارشی
            var (client, request) = LanguagesCode.CreateApiRequest(client: mockClient.Object);

            // ادعا (Assert): بررسی اینکه client ارسالی استفاده شده و request درست است
            Assert.NotNull(client);
            Assert.NotNull(request);
            Assert.Equal(expectedApiAddress, request.Resource);
            Assert.Equal(Method.Get, request.Method);
            Assert.Same(mockClient.Object, client); // بررسی اینکه همان client ارسالی استفاده شده
        }

        [Fact]
        public void CreateApiRequest_WithCustomApiLinkAndAddress_UsesProvidedValues()
        {
            // ترتیب (Arrange): تنظیم مقادیر برای تست با apiLink و apiAddress سفارشی
            // هدف: بررسی اینکه وقتی هر دو apiLink و apiAddress سفارشی ارائه می‌شوند، از آنها استفاده می‌شود
            string customApiLink = "http://custom.api.com/"; // اضافه کردن اسلش انتهایی
            string customApiAddress = "api/custom/languages";

            // اجرا (Act): فراخوانی متد با apiLink و apiAddress سفارشی
            var (client, request) = LanguagesCode.CreateApiRequest(customApiLink, customApiAddress);

            // ادعا (Assert): بررسی اینکه client و request با مقادیر سفارشی درست ایجاد شده‌اند
            Assert.NotNull(client);
            Assert.NotNull(request);
            Assert.Equal(customApiAddress, request.Resource);
            Assert.Equal(Method.Get, request.Method);
            Assert.Equal(customApiLink, client.Options.BaseUrl?.ToString());
            Assert.Equal(TimeSpan.FromSeconds(10), client.Options.Timeout);
        }

        [Fact]
        public void CreateApiRequest_WithNullApiLinkAndCustomClient_IgnoresApiLink()
        {
            // ترتیب (Arrange): تنظیم یک client موک و apiLink null
            // هدف: بررسی اینکه وقتی apiLink null و client سفارشی ارائه می‌شود، apiLink نادیده گرفته می‌شود
            var mockClient = new Mock<IRestClient>();
            var options = new RestClientOptions
            {
                BaseUrl = new Uri("http://mock.api.com/"), // اضافه کردن اسلش انتهایی
                Timeout = TimeSpan.FromSeconds(5)
            };
            mockClient.Setup(c => c.Options).Returns(new ReadOnlyRestClientOptions(options));
            string expectedApiAddress = "api/LanguagesCode/LanguageCodeList";

            // اجرا (Act): فراخوانی متد با apiLink null و client سفارشی
            var (client, request) = LanguagesCode.CreateApiRequest(apiLink: null, client: mockClient.Object);

            // ادعا (Assert): بررسی اینکه client سفارشی استفاده شده و apiLink نادیده گرفته شده
            Assert.NotNull(client);
            Assert.NotNull(request);
            Assert.Equal(expectedApiAddress, request.Resource);
            Assert.Equal(Method.Get, request.Method);
            Assert.Same(mockClient.Object, client); // بررسی اینکه client تغییر نکرده
        }
    }
}