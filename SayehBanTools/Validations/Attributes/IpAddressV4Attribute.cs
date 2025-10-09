using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.RegularExpressions;

namespace SayehBanTools.Validations.Attributes;

/// <summary>
/// Attribute برای اعتبار‌سنجی آدرس IPv4
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class IpAddressV4Attribute : ValidationAttribute
{
    private const string DefaultErrorMessage = "آدرس IP نامعتبر است. باید یک آدرس IPv4 معتبر (مانند 192.168.1.1) باشد.";
    /// <summary>
    /// کلاس سازنده
    /// </summary>
    public IpAddressV4Attribute() : base(DefaultErrorMessage)
    {
    }
    /// <summary>
    /// کلاس سازنده
    /// </summary>
    /// <param name="errorMessage"></param>
    public IpAddressV4Attribute(string errorMessage) : base(errorMessage)
    {
    }
    /// <summary>
    /// بررسی اعتبار
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validationContext"></param>
    /// <returns></returns>
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

        // بررسی فرمت IPv4 با regex
        if (!Regex.IsMatch(ipAddress, @"^(\d{1,3})\.(\d{1,3})\.(\d{1,3})\.(\d{1,3})$"))
        {
            return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
        }

        // بررسی معتبر بودن IPv4 با TryParse و AddressFamily
        if (!IPAddress.TryParse(ipAddress, out IPAddress? parsedIp) ||
            parsedIp.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
        {
            return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
        }

        // بررسی محدوده اعداد (0-255 برای هر بخش)
        string[] octets = ipAddress.Split('.');
        foreach (var octet in octets)
        {
            if (!int.TryParse(octet, out int number) || number < 0 || number > 255)
            {
                return new ValidationResult($"{ErrorMessage ?? DefaultErrorMessage} مقدار واردشده: '{ipAddress}'");
            }
        }

        return ValidationResult.Success;
    }
}