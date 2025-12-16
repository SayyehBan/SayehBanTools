using System.Text;
using OllamaSharp;

namespace SayehBanTools.AI.Ollama.gemma;

/// <summary>
/// دستیار هوشمند محلی با قابلیت Vision (تحلیل تصویر) که با مدل‌های Ollama کار می‌کند.
/// </summary>
public class LocalAiVisionAssistant : IDisposable
{
    private readonly OllamaApiClient _ollamaClient;
    private readonly Chat _chat;
    private bool _isInitialized = false;

    private readonly string? _pendingSystemPrompt;

    /// <summary>
    /// سازنده کلاس
    /// </summary>
    /// <param name="baseUrl">آدرس پایه Ollama (مثال: http://localhost:11434)</param>
    /// <param name="modelName">نام مدل vision (مثال: qwen3-vl:8b, llava, llama3.2-vision)</param>
    /// <param name="systemPrompt">پرامپت سیستم (اختیاری)</param>
    public LocalAiVisionAssistant(string baseUrl, string modelName, string? systemPrompt = null)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("آدرس پایه Ollama نمی‌تواند خالی باشد.", nameof(baseUrl));
        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException("نام مدل نمی‌تواند خالی باشد.", nameof(modelName));

        _ollamaClient = new OllamaApiClient(new Uri(baseUrl), modelName);

        // پرامپت سیستم مستقیماً در Chat تنظیم می‌شه
        _chat = string.IsNullOrWhiteSpace(systemPrompt)
            ? new Chat(_ollamaClient)
            : new Chat(_ollamaClient, systemPrompt);
    }

    /// <summary>
    /// مقداردهی اولیه (ارسال پرامپت سیستم اگر وجود داشته باشه)
    /// </summary>
    public Task InitializeAsync()
    {
        _isInitialized = true;
        return Task.CompletedTask; // فوراً تمام می‌شه
    }

    /// <summary>
    /// ارسال یک پرامپت متنی ساده (بدون تصویر)
    /// </summary>
    public async Task<string> AskAsync(string userPrompt)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("دستیار مقداردهی اولیه نشده.");

        var result = new StringBuilder();
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90)); // حداکثر ۹۰ ثانیه

        try
        {
            await foreach (var chunk in _chat.SendAsAsync(OllamaSharp.Models.Chat.ChatRole.User, userPrompt)
                .WithCancellation(cts.Token))
            {
                // اگر chunk خالی بود یا فقط فضای خالی، ممکنه مدل در حال "فکر کردن" باشه
                if (!string.IsNullOrWhiteSpace(chunk))
                    result.Append(chunk);
            }
        }
        catch (OperationCanceledException)
        {
            result.Append("\n\n[هشدار: پاسخ به دلیل timeout قطع شد]");
        }

        return result.ToString().Trim();
    }
    /// <summary>
    /// تحلیل خودکار تصویر - بدون نیاز به سوال کاربر
    /// </summary>
    public async Task<string> AnalyzeImageAutomaticallyAsync(params string[] imagePaths)
    {
        string autoPrompt = """
            این تصویر را با دقت بررسی کن و بسته به نوع آن، بهترین تحلیل ممکن را انجام بده:

            - اگر کارت ملی، شناسنامه، گواهینامه یا سند رسمی ایرانی است → تمام اطلاعات (نام، نام خانوادگی، کد ملی، تاریخ تولد، شماره سریال و ...) را دقیق استخراج کن.
            - اگر فاکتور، قبض یا سند مالی است → مبلغ، تاریخ، شماره و موارد مهم را استخراج کن.
            - اگر پلاک خودرو است → شماره پلاک را دقیق بخوان.
            - اگر متن دست‌نویس یا چاپی دارد → تمام متن‌ها را با OCR دقیق استخراج کن.
            - اگر عکس از شخص، مکان یا شیء است → توصیف کامل و دقیق به زبان فارسی بده.

            فقط اطلاعات مهم و دقیق را بده. از توضیح اضافه، سلام یا مقدمه خودداری کن.
            پاسخ را مرتب و به زبان فارسی بنویس.
            """;

        return await AskWithImagesAsync(autoPrompt, imagePaths);
    }
    /// <summary>
    /// توصیف دقیق یک تصویر (با پرامپت پیش‌فرض)
    /// </summary>
    /// <param name="imagePaths">لیست مسیر فایل‌های تصویر (می‌تونی چندتا بفرستی)</param>
    /// <returns>توصیف مدل از تصویر</returns>
    public async Task<string> DescribeImageAsync(params string[] imagePaths)
    {
        return await AskWithImagesAsync("این تصویر را به زبان فارسی دقیق و کامل توصیف کن. فقط توصیف بده.", imagePaths);
    }

    /// <summary>
    /// سوال دلخواه در مورد تصویر (مثلاً OCR، شناسایی اشیا، تحلیل و غیره)
    /// </summary>
    /// <param name="question">سوال به فارسی یا انگلیسی</param>
    /// <param name="imagePaths">مسیر فایل‌های تصویر</param>
    /// <returns>پاسخ مدل</returns>
    public async Task<string> AskAboutImageAsync(string question, params string[] imagePaths)
    {
        return await AskWithImagesAsync(question, imagePaths);
    }

    /// <summary>
    /// متد اصلی برای ارسال پرامپت + تصویر(ها)
    /// </summary>
    private async Task<string> AskWithImagesAsync(string prompt, params string[] imagePaths)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("دستیار مقداردهی اولیه نشده.");
        if (imagePaths == null || imagePaths.Length == 0)
            throw new ArgumentException("حداقل یک تصویر باید ارسال شود.");

        var imagesAsBase64 = new List<string>();
        foreach (var path in imagePaths)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"فایل تصویر یافت نشد: {path}");

            byte[] imageBytes = await File.ReadAllBytesAsync(path);
            string base64 = Convert.ToBase64String(imageBytes);
            imagesAsBase64.Add(base64);
        }

        var result = new StringBuilder();
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

        try
        {
            await foreach (var chunk in _chat.SendAsAsync(
                OllamaSharp.Models.Chat.ChatRole.User,
                prompt,
                imagesAsBase64: imagesAsBase64).WithCancellation(cts.Token))
            {
                result.Append(chunk);
            }
        }
        catch (OperationCanceledException)
        {
            result.Append("\n\n[پاسخ ناتمام - مدل استریم را قطع نکرد]");
        }

        return result.ToString().Trim().Trim('"').Trim();
    }
    /// <summary>
    /// Releases all resources used by the current instance.
    /// </summary>
    /// <remarks>Call this method when you are finished using the object to free unmanaged resources and
    /// perform other cleanup operations. After calling this method, the object should not be used.</remarks>
    public void Dispose()
    {
        // در صورت نیاز dispose اضافه کن
    }
}