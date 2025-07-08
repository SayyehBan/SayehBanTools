namespace SayehBanTools.Model.Entities;
/// <summary>
/// پیغام خروجی جدول
/// </summary>
public class OutputMessageTableType
{
    /// <summary>
    /// کد خطا
    /// </summary>
    public int Code { get; set; }
    /// <summary>
    /// پیغام خطا
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// تعداد
    /// </summary>
    public int Count { get; set; }
    /// <summary>
    ///  شناسه ها
    /// </summary>
    public string? Ids { get; set; }
}
