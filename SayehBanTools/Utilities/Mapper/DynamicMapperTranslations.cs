using SayehBanTools.Model.Entities;
using System.Collections.Concurrent;
using System.Reflection;
using static SayehBanTools.Model.Entities.PublicModel;

namespace SayehBanTools.Utilities.Mapper;

/// <summary>
/// ابزارهای مپینگ پویا برای تبدیل dynamic به مدل‌های DTO
/// </summary>
public static class DynamicMapperTranslations
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo> _idPropertyCache = new();
    private static readonly ConcurrentDictionary<Type, HashSet<string>> _auditablePropsCache = new();

    #region SelectDynamic - MapTo<T>

    /// <summary>
    /// تبدیل dynamic (از Dapper) به DTO با پشتیبانی از ستون‌های داینامیک و Translations
    /// </summary>
    /// <typeparam name="T">نوع DTO که باید از AuditableEntity ارث‌بری کند</typeparam>
    /// <param name="result">نتیجه dynamic از Dapper</param>
    /// <returns>نمونه پر شده از T</returns>
    public static T MapTo<T>(dynamic result) where T : PublicModel.AuditableEntity, new()
    {
        var dto = new T();
        if (result is not IDictionary<string, object> dict) return dto;

        var dtoType = typeof(T);

        // کش پراپرتی‌های Auditable
        var auditableProps = _auditablePropsCache.GetOrAdd(
            typeof(PublicModel.AuditableEntity),
            _ => typeof(PublicModel.AuditableEntity).GetProperties()
                .Select(p => p.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase));

        // پراپرتی Translations
        var translationsProp = dtoType.GetProperty("Translations",
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (translationsProp == null ||
            !typeof(IDictionary<string, string>).IsAssignableFrom(translationsProp.PropertyType))
        {
            throw new InvalidOperationException($"نوع {dtoType.Name} باید پراپرتی Translations از نوع IDictionary<string, string> داشته باشد.");
        }

        var translations = (IDictionary<string, string>?)
            (translationsProp.GetValue(dto) ?? Activator.CreateInstance(translationsProp.PropertyType));
        translationsProp.SetValue(dto, translations);

        // تشخیص خودکار Id
        var idProp = _idPropertyCache.GetOrAdd(dtoType, type =>
            type.GetProperties()
                .FirstOrDefault(p =>
                    (p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.StartsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.StartsWith("ID", StringComparison.OrdinalIgnoreCase)) &&
                    p.PropertyType == typeof(int) &&
                    p.CanWrite)
            ?? throw new InvalidOperationException($"پراپرتی ID مناسب در نوع {type.Name} پیدا نشد."));

        foreach (var kvp in dict)
        {
            var key = kvp.Key;
            var value = kvp.Value;

            // 1. مپ مستقیم به پراپرتی DTO
            var prop = dtoType.GetProperty(key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop != null && prop.CanWrite)
            {
                try
                {
                    var converted = ConvertValue(value, prop.PropertyType);
                    prop.SetValue(dto, converted);
                }
                catch { /* ignore */ }
                continue;
            }

            // 2. بقیه → ترجمه (داینامیک)
            if (!auditableProps.Contains(key) &&
                !string.Equals(key, idProp.Name, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(key, "Translations", StringComparison.OrdinalIgnoreCase) &&
                translations != null)
            {
                translations[key] = value?.ToString() ?? string.Empty;
            }
        }

        return dto;
    }

    #endregion

    #region ConvertToGetModelInsertOrUpdate

    /// <summary>
    /// تبدیل dynamic به مدل Get برای ذخیره در NoSQL (Redis/Elasticsearch)
    /// </summary>
    /// <typeparam name="T">نوع مدل خروجی (مثل LocationNamesGet)</typeparam>
    /// <param name="insertedData">داده درج شده از دیتابیس</param>
    /// <param name="languageCode">کد زبان (مثل fa)</param>
    /// <param name="idPropertyName">نام ستون Id (مثل LocationNameId)</param>
    /// <param name="namePropertyTemplate">الگوی نام (مثل LocationName → faLocationName)</param>
    /// <returns>مدل پر شده یا null در صورت عدم وجود نام ترجمه</returns>
    public static T? ConvertToGetModelInsertOrUpdate<T>(
        dynamic insertedData,
        string languageCode,
        string idPropertyName,
        string namePropertyTemplate) where T : class, new()
    {
        if (insertedData is not IDictionary<string, object> dict)
            return null;

        var namePropertyKey = $"{languageCode}{namePropertyTemplate}";
        if (!dict.ContainsKey(namePropertyKey))
            return null;

        var result = new T();
        var type = typeof(T);

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!prop.CanWrite) continue;

            object? value = null;

            // 1. RowVersion
            if (string.Equals(prop.Name, "RowVersion", StringComparison.OrdinalIgnoreCase))
            {
                if (dict.TryGetValue("RowVersion", out var rowVersionObj))
                {
                    value = rowVersionObj switch
                    {
                        byte[] bytes => bytes,
                        string str => Convert.FromBase64String(str),
                        _ => null
                    };
                }
            }
            // 2. Id (مثل LocationNameId)
            else if (string.Equals(prop.Name, idPropertyName, StringComparison.OrdinalIgnoreCase) && prop.PropertyType == typeof(int))
            {
                if (dict.TryGetValue(idPropertyName, out var idValue))
                {
                    value = Convert.ToInt32(idValue);
                }
            }
            // 3. Name (مثل LocationName) → از faLocationName
            else if (string.Equals(prop.Name, namePropertyTemplate, StringComparison.OrdinalIgnoreCase))
            {
                if (dict.TryGetValue(namePropertyKey, out var nameValue))
                {
                    value = nameValue?.ToString() ?? string.Empty;
                }
            }
            // 4. سایر پراپرتی‌ها (IsApproved, CreatedAtDate, ...)
            else
            {
                var key = GetKeyForProperty(prop.Name, dict.Keys);
                if (key != null && dict.TryGetValue(key, out var rawValue))
                {
                    value = ConvertValue(rawValue, prop.PropertyType);
                }
            }

            if (value != null)
                prop.SetValue(result, value);
        }

        return result;
    }

    #endregion

    #region Helper Methods (مشترک)

    /// <summary>
    /// پیدا کردن کلید مشابه در دیکشنری (Case-Insensitive)
    /// </summary>
    private static string? GetKeyForProperty(string propName, IEnumerable<string> keys)
    {
        return keys.FirstOrDefault(k => string.Equals(k, propName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// تبدیل امن نوع داده
    /// </summary>
    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null || value is DBNull)
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

        if (targetType.IsAssignableFrom(value.GetType()))
            return value;

        try
        {
            if (targetType == typeof(int) && int.TryParse(value.ToString(), out var i)) return i;
            if (targetType == typeof(bool) && bool.TryParse(value.ToString(), out var b)) return b;
            if (targetType == typeof(DateTime) && DateTime.TryParse(value.ToString(), out var dt)) return dt;
            if (targetType == typeof(byte[]) && value is string s) return Convert.FromBase64String(s);
            if (targetType == typeof(Guid) && value is string gs && Guid.TryParse(gs, out var g)) return g;

            return Convert.ChangeType(value, targetType);
        }
        catch
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }
    }

    #endregion
    /// <summary>
    /// تبدیل خروجی SP جدید GetTranslationsByIdUltraDynamic (با ستون‌هایی مثل faGender, enSex, faRawFirstName)
    /// به مدل TranslationResult با ساختار تودرتو و کاملاً انعطاف‌پذیر
    /// </summary>
    /// <summary>
    /// تبدیل خروجی SP جدید GetTranslationsByIdUltraDynamic
    /// ستون‌ها: faGender, enSex, faRawFirstName, ...
    /// فقط پسوندهایی که در @FieldSuffixes داده شده رو برمی‌گردونه
    /// </summary>
    public static T MapToUltraDynamic<T>(dynamic result) where T : new()
    {
        var dto = new T();
        if (result is not IDictionary<string, object> dict) return dto;

        var dtoType = typeof(T);

        // پراپرتی Translations
        var translationsProp = dtoType.GetProperty("Translations",
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (translationsProp == null ||
            translationsProp.PropertyType != typeof(Dictionary<string, Dictionary<string, string>>))
        {
            throw new InvalidOperationException("مدل باید پراپرتی Translations از نوع Dictionary<string, Dictionary<string, string>> داشته باشد.");
        }

        var translations = (Dictionary<string, Dictionary<string, string>>)
            (translationsProp.GetValue(dto) ?? Activator.CreateInstance(translationsProp.PropertyType)!);

        translationsProp.SetValue(dto, translations);

        // تشخیص خودکار Id
        var idProp = _idPropertyCache.GetOrAdd(dtoType, type =>
            type.GetProperties()
                .FirstOrDefault(p =>
                    p.PropertyType == typeof(int) &&
                    p.CanWrite &&
                    (p.Name.StartsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.StartsWith("ID", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase) ||
                     p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase)))
            ?? throw new InvalidOperationException($"پراپرتی Id در نوع {type.Name} پیدا نشد."));

        foreach (var kvp in dict)
        {
            var columnName = kvp.Key;
            var value = kvp.Value;

            // 1. ستون Id
            if (string.Equals(columnName, idProp.Name, StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(value?.ToString(), out var id))
                    idProp.SetValue(dto, id);
                continue;
            }

            // 2. ستون‌های AuditableEntity (IsApproved, RowVersion, ...)
            var auditableProp = typeof(AuditableEntity)
                .GetProperty(columnName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (auditableProp != null && auditableProp.CanWrite)
            {
                try
                {
                    var converted = ConvertValue(value, auditableProp.PropertyType);
                    auditableProp.SetValue(dto, converted);
                }
                catch { }
                continue;
            }

            // 3. ستون‌های ترجمه: faGender, enSex, faRawFirstName, ...
            if (columnName.Length > 2 &&
                columnName.Substring(0, 2).All(char.IsLetter) &&
                char.IsUpper(columnName[2]))
            {
                var langCode = columnName.Substring(0, 2).ToLower();
                var fieldName = columnName.Substring(2); // Gender, Sex, RawFirstName, ...

                if (!translations.ContainsKey(langCode))
                    translations[langCode] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                translations[langCode][fieldName] = value?.ToString() ?? string.Empty;
            }
        }

        return dto;
    }
}