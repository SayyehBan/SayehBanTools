using System.ComponentModel.DataAnnotations;

namespace SayehBanTools.Validations.Attributes;

/// <summary>
/// Attribute برای اعتبار‌سنجی مقدار RowVersion (Timestamp) به صورت رشته Base64
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class RowVersionAttribute : ValidationAttribute
{
    /// <summary>
    /// پیام خطای پیش‌فرض برای زمانی که مقدار RowVersion نامعتبر است
    /// </summary>
    private const string DefaultErrorMessage = "مقدار RowVersion نامعتبر است. باید یک رشته Base64 معتبر با طول 12 کاراکتر باشد.";

    /// <summary>
    /// سازنده پیش‌فرض
    /// </summary>
    public RowVersionAttribute() : base(DefaultErrorMessage)
    {
    }

    /// <summary>
    /// سازنده با پیام خطای سفارشی
    /// </summary>
    /// <param name="errorMessage">پیام خطای سفارشی</param>
    public RowVersionAttribute(string errorMessage) : base(errorMessage)
    {
    }

    /// <summary>
    /// بررسی صحت مقدار RowVersion
    /// </summary>
    /// <param name="value">مقداری که باید بررسی شود</param>
    /// <param name="validationContext">زمینه اعتبار‌سنجی</param>
    /// <returns>نتیجه اعتبار‌سنجی</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // اگر مقدار null باشد، نامعتبر است
        if (value == null)
        {
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

        // بررسی نوع مقدار (باید رشته باشد)
        if (value is not string rowVersion)
        {
            return new ValidationResult("مقدار RowVersion باید یک رشته باشد.");
        }

        // بررسی خالی نبودن
        if (string.IsNullOrWhiteSpace(rowVersion))
        {
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

        // بررسی معتبر بودن رشته Base64
        try
        {
            // بررسی طول رشته (برای RowVersion که 8 بایت است، Base64 باید 12 کاراکتر باشد)
            if (rowVersion.Length != 12)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }

            // تلاش برای تبدیل رشته Base64 به آرایه بایت
            byte[] decodedBytes = Convert.FromBase64String(rowVersion);

            // بررسی طول آرایه بایت (باید 8 بایت باشد)
            if (decodedBytes.Length != 8)
            {
                return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
            }
        }
        catch (FormatException)
        {
            // اگر رشته Base64 معتبر نباشد
            return new ValidationResult(ErrorMessage ?? DefaultErrorMessage);
        }

        return ValidationResult.Success;
    }
}