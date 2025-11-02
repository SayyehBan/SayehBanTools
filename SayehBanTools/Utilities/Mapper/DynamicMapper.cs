using System.Collections.Concurrent;
using System.Reflection;
using static SayehBanTools.Model.Entities.PublicModel;

namespace SayehBanTools.Utilities.Mapper;

/// <summary>
/// پویا کردن mapper
/// </summary>
public static class DynamicMapper
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo> _idPropertyCache = new();
    private static readonly ConcurrentDictionary<Type, HashSet<string>> _auditablePropsCache = new();

    /// <summary>
    /// تبدیل dynamic (از Dapper) به DTO با پشتیبانی از ستون‌های داینامیک
    /// بدون هیچ نام دستی (کاملاً Generic)
    /// </summary>
    public static T MapTo<T>(dynamic result) where T : AuditableEntity, new()
    {
        var dto = new T();
        var dict = result as IDictionary<string, object>;

        if (dict == null) return dto;

        var dtoType = typeof(T);

        // کش کردن Auditable Properties
        var auditableProps = _auditablePropsCache.GetOrAdd(
            typeof(AuditableEntity),
            _ => typeof(AuditableEntity).GetProperties()
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase));

        // پیدا کردن پراپرتی Translations
        var translationsProp = dtoType.GetProperty("Translations",
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (translationsProp == null ||
            !typeof(IDictionary<string, string>).IsAssignableFrom(translationsProp.PropertyType))
        {
            throw new InvalidOperationException($"DTO نوع {dtoType.Name} باید پراپرتی Translations از نوع Dictionary<string, string> داشته باشد.");
        }

        // ایجاد دیکشنری
        var translations = (IDictionary<string, string>?)
            (translationsProp.GetValue(dto) ?? Activator.CreateInstance(translationsProp.PropertyType));
        translationsProp.SetValue(dto, translations);

        // تشخیص خودکار پراپرتی Id (مثل CategoryNameId, CityId, ...)
        var idProp = _idPropertyCache.GetOrAdd(dtoType, type =>
            type.GetProperties()
                .FirstOrDefault(p =>
                    (p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase)) &&
                    p.PropertyType == typeof(int) &&
                    p.CanWrite)
            ?? throw new InvalidOperationException($"No suitable ID property found for type {type.Name}"));

        foreach (var kvp in dict)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            // 1. اگر فیلد در DTO وجود داره → مپ مستقیم
            var prop = dtoType.GetProperty(key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                try
                {
                    var convertedValue = ConvertValue(value, prop.PropertyType);
                    prop.SetValue(dto, convertedValue);
                }
                catch
                {
                    // نادیده بگیر
                }
                continue;
            }

            // 2. در غیر این صورت → ترجمه (داینامیک)
            // شرط: نه Auditable، نه Id، نه Translations
            if (!auditableProps.Contains(key) &&
                (idProp == null || !string.Equals(key, idProp.Name, StringComparison.OrdinalIgnoreCase)) &&
                !string.Equals(key, "Translations", StringComparison.OrdinalIgnoreCase) &&
                translations != null)
            {
                var langCode = key;
                var translation = value?.ToString() ?? string.Empty;
                translations[langCode] = translation;
            }
        }

        return dto;
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null || value is DBNull)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        try
        {
            if (targetType == typeof(Guid) && value is string s && Guid.TryParse(s, out var g))
                return g;

            if (targetType == typeof(DateTime) && value is DateTime dt)
                return dt;

            if (targetType == typeof(int) && int.TryParse(value.ToString(), out var i))
                return i;

            if (targetType == typeof(bool) && bool.TryParse(value.ToString(), out var b))
                return b;

            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }
}
/*
 نحوه استفاده
 return DynamicMapper.MapTo<CategoryTranslationDynamicResult>(dynamicResult);
 */