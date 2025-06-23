namespace SayehBanTools.Model.Entities;
/// <summary>
/// کلاس پیام خطا
/// </summary>
public class SqlMessage
{
    /// <summary>
    /// کد خطا
    /// </summary>
    public int Code { get; set; } = 0;
    /// <summary>
    /// پیام خطا
    /// </summary>
    public string? Message { get; set; }
}
