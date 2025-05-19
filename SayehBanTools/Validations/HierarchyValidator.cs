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
}
