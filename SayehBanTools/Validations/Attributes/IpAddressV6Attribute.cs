using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace SayehBanTools.Validations.Attributes;

/// <summary>
/// Attribute برای اعتبار‌سنجی آدرس IPv6
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class IpAddressV6Attribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "آدرس IP نامعتبر است. باید یک آدرس IPv6 معتبر (مانند 2001:0db8:85a3:0000:0000:8a2e:0370:7334) باشد.";

    /// <summary>
    /// کلاس سازنده
    /// </summary>
    public IpAddressV6Attribute() : base(DefaultErrorMessage)
    {
    }

    /// <summary>
    /// کلاس سازنده با پیام خطای سفارشی
    /// </summary>
    /// <param name="errorMessage">پیام خطای سفارشی</param>
    public IpAddressV6Attribute(string errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    /// بررسی اعتبار
    /// </summary>
    /// <param name="value">مقداری که باید بررسی شود</param>
    /// <param name="validationContext">زمینه اعتبار‌سنجی</param>
    /// <returns>نتیجه اعتبار‌سنجی</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // بررسی null بودن
        if (value == null)
        {
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

        // بررسی نوع مقدار (باید رشته باشد)
        if (value is not string ipAddress)
        {
            return new ValidationResult("آدرس IP باید یک رشته باشد.");
        }

        // بررسی خالی بودن
        if (string.IsNullOrWhiteSpace(ipAddress))
        {
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

        // بررسی فرمت IPv6 با regex
        // پشتیبانی از فرمت کامل (8 گروه) و فشرده (با ::)
        if (!Regex.IsMatch(ipAddress, @"^(([0-9a-fA-F]{1,4}:){7}[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,7}:|([0-9a-fA-F]{1,4}:){1,6}:[0-9a-fA-F]{1,4}|([0-9a-fA-F]{1,4}:){1,5}(:[0-9a-fA-F]{1,4}){1,2}|([0-9a-fA-F]{1,4}:){1,4}(:[0-9a-fA-F]{1,4}){1,3}|([0-9a-fA-F]{1,4}:){1,3}(:[0-9a-fA-F]{1,4}){1,4}|([0-9a-fA-F]{1,4}:){1,2}(:[0-9a-fA-F]{1,4}){1,5}|[0-9a-fA-F]{1,4}:(:[0-9a-fA-F]{1,4}){1,6}|:((:[0-9a-fA-F]{1,4}){1,7}|:))$"))
        {
            return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
        }

        // بررسی معتبر بودن IPv6 با TryParse و AddressFamily
        if (!IPAddress.TryParse(ipAddress, out IPAddress? parsedIp) ||
            parsedIp.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
        }

        return ValidationResult.Success;
    }
}