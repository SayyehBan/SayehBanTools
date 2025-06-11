namespace SayehBanTools.Model.Entities;
/// <summary>
/// View Model برای تنظیمات زبان
/// </summary>
public class VM_LanguageSettings
{
    /// <summary>
    /// کلاس برای تنظیمات زبان
    /// </summary>
    public class LanguageSettingsGetAll
    {
        /// <summary>
        /// شناسه تنظیمات زبان
        /// </summary>
        public int LanguageSettingID { get; set; }
        /// <summary>
        /// کد زبان
        /// </summary>
        public string? LanguageCode { get; set; }
        /// <summary>
        /// کد زبان منطقه
        /// </summary>
        public string LanguageCodeRegion { get; set; } = string.Empty;
        /// <summary>
        /// نام تحلیلگر
        /// </summary>
        public string? AnalyzerName { get; set; }
        /// <summary>
        /// نوکن ساز
        /// </summary>
        public string? Tokenizer { get; set; }
        /// <summary>
        /// فیلترها
        /// </summary>
        public string? Filters { get; set; }
    }
}
