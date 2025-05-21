namespace SayehBanTools.Validations;


/// <summary>
/// کلاس بررسی ساختار درختی
/// </summary>
public static class HierarchyValidator
{
    /// <summary>
    /// بررسی اعتبار سنجی ساختار درختی
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static bool IsValidHierarchyPath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return true;

        // باید با / شروع و پایان یابد
        if (!path.StartsWith("/") || !path.EndsWith("/"))
            return false;

        // بین /ها باید اعداد باشند
        var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        return parts.All(part => int.TryParse(part, out _));
    }
    /// <summary>
    /// بررسی صحت رشته مسیر سلسله مراتب والد.
    /// </summary>
    /// <param name="parentHierarchyPath">رشته مسیر سلسله مراتب (مثال: "/" یا "/1/")</param>
    /// <exception cref="ArgumentException">در صورتی که فرمت مسیر سلسله مراتب نامعتبر باشد.</exception>
    public static void ValidateHierarchyPath(string parentHierarchyPath)
    {
        if (!string.IsNullOrEmpty(parentHierarchyPath) &&
            !IsValidHierarchyPath(parentHierarchyPath))
        {
            throw new ArgumentException("فرمت سلسله مراتب نامعتبر است. باید به صورت / یا /1/ باشد.",
                nameof(parentHierarchyPath));
        }
    }
}
