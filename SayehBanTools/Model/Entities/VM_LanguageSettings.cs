namespace SayehBanTools.Model.Entities;
/// <summary>
/// View Model برای تنظیمات زبان
/// </summary>
public class VM_LanguageSettings
{
    /// <summary>
    /// View Model دریافت تمامی تنظیمات زبان
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
        /// کد زبان منطقی
        /// </summary>
        public string? LanguageCodeRegion { get; set; }
        /// <summary>
        /// نام تحلیل گر
        /// </summary>
        public string? AnalyzerName { get; set; }
        /// <summary>
        /// توکن سازی
        /// </summary>
        public string? Tokenizer { get; set; }
        /// <summary>
        /// فیلترکردن
        /// </summary>
        public string? Filters { get; set; }

    }
}
