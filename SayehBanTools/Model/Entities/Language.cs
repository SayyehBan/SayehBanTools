namespace SayehBanTools.Model.Entities;

/// <summary>
/// دسته‌بندی‌ زبان
/// </summary>
public class Language
{
    /// <summary>
    /// شناسه زبان
    /// </summary>
    public int LanguageID { get; set; }
    /// <summary>
    /// کد زبان
    /// </summary>
    public string? LanguageCode { get; set; }
    /// <summary>
    /// نام زبان
    /// </summary>
    public string? LanguageName { get; set; }
    /// <summary>
    /// نام فارسی زبان
    /// </summary>
    public string? LanguageNameFarsi { get; set; }
    /// <summary>
    /// جهت نمایش
    /// </summary>
    public string? TextDirection { get; set; }
}