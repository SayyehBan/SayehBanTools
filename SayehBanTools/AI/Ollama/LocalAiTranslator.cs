using System.Text;
using OllamaSharp;
namespace SayehBanTools.AI.Ollama;

/// <summary>
/// یک ترجمه‌کننده هوشمند محلی (Local AI Translator) که با مدل‌های Ollama کار می‌کند.
/// قابل تنظیم کامل از بیرون و استفاده مجدد در هر پروژه.
/// </summary>
public class LocalAiTranslator : IDisposable
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly Chat _chat;
    private bool _isInitialized = false;

    /// <summary>
    /// سازنده کلاس - تمام تنظیمات از بیرون تزریق می‌شود
    /// </summary>
    /// <param name="baseUrl">آدرس پایه Ollama (مثال: http://localhost:11434)</param>
    /// <param name="modelName">نام مدل (مثال: llama3.1, phi3, mistral و ...)</param>
    /// <param name="systemPrompt">پرامپت سیستم برای کنترل رفتار مدل (اختیاری)</param>
    public LocalAiTranslator(string baseUrl, string modelName, string? systemPrompt = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("آدرس پایه Ollama نمی‌تواند خالی باشد.", nameof(baseUrl));

        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("نام مدل نمی‌تواند خالی باشد.", nameof(modelName));

        _ollamaClient = new OllamaApiClient(new Uri(baseUrl), modelName);
        _chat = new Chat(_ollamaClient);

        // پرامپت سیستم رو ذخیره کن، اما هنوز ارسال نکن
        _pendingSystemPrompt = systemPrompt;
    }
    private readonly string? _pendingSystemPrompt;
    /// <summary>
    /// مقداردهی اولیه با پرامپت سیستم (اگر بعداً بخوای تغییر بدی)
    /// </summary>
    public async Task InitializeAsync()
    {
        if (_pendingSystemPrompt != null)
        {
            await foreach (var _ in _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.System, _pendingSystemPrompt))
            {
                // منتظر می‌مونیم تا ارسال بشه
            }
        }

        _isInitialized = true;
    }

    /// <summary>
    /// ترجمه یک متن از زبان مبدا به مقصد
    /// </summary>
    /// <param name="text">متن برای ترجمه</param>
    /// <param name="sourceLang">زبان مبدا (مثال: en, fa, fr)</param>
    /// <param name="targetLang">زبان مقصد</param>
    /// <returns>متن ترجمه شده</returns>
    public async Task<string> TranslateAsync(string text, string sourceLang = "en", string targetLang = "fa")
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        if (!_isInitialized)
            throw new InvalidOperationException("ترجمه‌کننده مقداردهی اولیه نشده. ابتدا InitializeAsync را فراخوانی کنید یا پرامپت سیستم را در سازنده بدهید.");

        string prompt = $"Translate this text from {sourceLang} to {targetLang} exactly as instructed:\n\n{text}";

        var result = new StringBuilder();

        await foreach (var chunk in _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.User, prompt))
        {
            result.Append(chunk);
        }

        return result.ToString().Trim().Trim('"').Trim();
    }

    /// <summary>
    /// ارسال یک پرامپت دلخواه و دریافت پاسخ (برای استفاده‌های عمومی، نه فقط ترجمه)
    /// </summary>
    public async Task<string> AskAsync(string userPrompt)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("ترجمه‌کننده مقداردهی اولیه نشده.");

        var result = new StringBuilder();

        await foreach (var chunk in _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.User, userPrompt))
        {
            result.Append(chunk);
        }

        return result.ToString().Trim();
    }

    /// <summary>
    /// پاک‌سازی منابع
    /// </summary>
    public void Dispose()
    {
        // OllamaSharp خودش dispose نمی‌خواد، اما اگر بعداً HttpClient اضافه شد، اینجا بذار
        // فعلاً خالی
    }
}