using SayehBanTools.MessagingBus.RabbitMQ.Model;

namespace SayehBanTools.MessagingBus.RabbitMQ.SendMessage;

/// <summary>
/// رابط برای ارسال پیام به RabbitMQ
/// </summary>
public interface ISendMessages
{
    /// <summary>
    /// ارسال پیام به RabbitMQ به‌صورت async
    /// </summary>
    /// <param name="message">پیام</param>
    /// <param name="exchange">نام اکسچنج</param>
    /// <param name="queueName">نام صف</param>
    /// <returns>Task</returns>
    Task SendMessageAsync(BaseMessage message, string? exchange, string? queueName);
}