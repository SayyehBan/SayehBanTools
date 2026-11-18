# SayehBanTools - راهنمای استفاده

[![NuGet](https://img.shields.io/nuget/v/SayehBanTools.svg)](https://www.nuget.org/packages/SayehBanTools)
[![Downloads](https://img.shields.io/nuget/dt/SayehBanTools.svg)](https://www.nuget.org/packages/SayehBanTools)
![Target](https://img.shields.io/badge/Target-net11.0--windows-512BD4)

![SayehBanTools Logo](https://github.com/SayyehBan/SayyehBanTools/logo.png)

یک کتابخانه چندمنظوره برای توسعه‌دهندگان دات‌نت با قابلیت‌های متنوع

> این پکیج به دلیل دارا بودن کنترل‌های WinForms، با Target Framework **net11.0-windows** منتشر می‌شود. برای پروژه‌های .NET Framework 4.8 نسخه‌ی [SayehBanTools.Framework](https://www.nuget.org/packages/SayehBanTools.Framework) را نصب کنید (کد یکسان است).

## فهرست مطالب

- [نصب](#نصب)
- [کلاس‌ها و قابلیت‌ها](#کلاسها-و-قابلیتها)
  - [IPAccess](#ipaccess)
  - [Calculator](#calculator)
  - [RabbitMQ](#rabbitmq)
  - [مدیریت فایل‌ها](#مدیریت-فایلها)
  - [تبدیل اعداد](#تبدیل-اعداد)
  - [مدیریت رشته‌ها](#مدیریت-رشتهها)
  - [رمزنگاری](#رمزنگاری)
  - [تولید مقادیر](#تولید-مقادیر)
  - [ارسال پیامک](#ارسال-پیامک)
  - [مدیریت تاریخ](#مدیریت-تاریخ)
  - [ترجمه](#ترجمه)
  - [صفحه‌بندی](#صفحهبندی)
  - [کارت‌خوان PC-POS](#کارتخوان-pc-pos)
  - [کمکی‌های فرم ویندوزی (Helper)](#کمکیهای-فرم-ویندوزی-helper)
  - [پخش فیلم آموزشی](#پخش-فیلم-آموزشی)
- [مثال‌های کد](#مثالهای-کد)
- [مشارکت](#مشارکت)
- [مجوز](#مجوز)

## نصب

برای نصب کتابخانه از NuGet استفاده کنید:

```bash
dotnet add package SayehBanTools
```

یا در Visual Studio از طریق Package Manager Console:

```powershell
Install-Package SayehBanTools
```

## کلاس‌ها و قابلیت‌ها

### IPAccess

بررسی محدوده IP:

```csharp
bool isInRange = IPAccess.IsInRange("192.168.1.5", "192.168.1.1", "192.168.1.10");
// true اگر IP در محدوده باشد
```

### Calculator

انجام محاسبات ریاضی:

```csharp
// جمع
double sum = Calculator.Add(5, 10, 15); // 30

// ضرب
double product = Calculator.Multiply(2, 3, 4); // 24

// تخفیف
decimal discounted = Calculator.Discount(1000, 20); // 800

// مالیات
decimal taxed = Calculator.Taxation(1000, 9); // 1090
```

### RabbitMQ

#### پیکربندی سرویس‌ها:

```csharp
services.AddTransient<RabbitMQConnection, RabbitMQConnection>();
services.AddTransient<ISendMessages, RabbitMQMessageBus>();
```

#### ارسال پیام:

```csharp
var message = new BaseMessage();
await _messageBus.SendMessageAsync(message, "exchange_name", "queue_name");
```

### مدیریت فایل‌ها

```csharp
var fileManager = new ManageFiles();

// آپلود فایل
string uploadedPath = await fileManager.UploadFileAsync("/uploads", file);

// حذف فایل
fileManager.DeleteFileServer("/uploads/file.txt");
```

### تبدیل اعداد

```csharp
// عدد به حروف
string words = ConvertNumToString.convert("123456789");
// "صد و بیست و سه میلیون و چهارصد و پنجاه و شش هزار و هفتصد و هشتاد و نه"

// عدد به رشته فارسی
string persianNumber = "123456".En2Fa(); // "۱۲۳۴۵۶"
```

### مدیریت رشته‌ها

```csharp
// تبدیل اعداد انگلیسی به فارسی
string persianNum = "123".En2Fa(); // "۱۲۳"

// تبدیل اعداد فارسی به انگلیسی
string englishNum = "۱۲۳".Fa2En(); // "123"

// حذف تگ‌های HTML
string cleanHtml = "<div>Test</div>".HtmlTags(); // "Test"
```

### رمزنگاری

```csharp
// رمزنگاری
string encrypted = await StringEncryptor.EncryptAsync("text", "initVector", "passPhrase");

// رمزگشایی
string decrypted = await StringEncryptor.DecryptAsync(encrypted, "initVector", "passPhrase");
```

> ⚠️ **هشدار:** `StringEncryptor` رمزنگاری **متقارن (Symmetric)** است؛ یعنی `initVector` و `passPhrase` هر دو در سمت مصرف‌کننده لازم هستند و متن رمزشده بدون آن‌ها قابل بازیابی است. این روش برای امن‌سازی در برابر نگاه ساده (مثلاً رشته اتصال در Config) مناسب است، اما **نه برای ذخیره رمز عبور کاربر** — برای آن از Hashing (مثل BCrypt) استفاده کنید. تازگی Salt/IV در هر دو پکیج یکسان است؛ رشته رمزشده‌ای که با یک نسخه ساخته شده فقط با همان نسخه رمزگشایی می‌شود.

### تولید مقادیر

```csharp
// کد ملی معتبر
string nationalCode = NationalCode.GenerateRandom();

// کد تصادفی 16 رقمی
string randomValue = GenerateValues.Generate16ValueRandome();
```

### ارسال پیامک

```csharp
// ارسال پیامک پترن
var data = new Dictionary<string, object> { { "code", "12345" } };
var result = await SMS_System.SendPatternAsync(apiLink, apiKey, data, "pattern_code", "sender", "09123456789", null);

// بررسی اعتبار
var credit = await SMS_System.GetCreditAsync(apiLink, apiKey);
```

### مدیریت تاریخ

```csharp
// تبدیل میلادی به شمسی
string shamsiDate = ConvertDateTime.MiladiToShamsi(DateTime.Now);

// تبدیل شمسی به میلادی
DateTime miladiDate = ConvertDateTime.ShamsiToMiladi("1402/05/15");
```

### ترجمه

```csharp
var translator = new TranslateTexts(new HttpClient());
var request = new TranslationRequest
{
    OriginalText = "Hello",
    InputLanguage = "en",
    OutputLanguage = "fa"
};

var response = await translator.TranslateTextAsync(request);
// response.Translations[0] = "سلام"
```

### صفحه‌بندی

```csharp
var query = dbContext.Products.AsQueryable();
var pager = new Pager(totalItems: 100, currentPage: 2, pageSize: 10);

var pagedResults = query.PagedResult(pager.CurrentPage, pager.PageSize, out int totalCount);
```

## مثال‌های کد

### مثال کامل RabbitMQ

```csharp
// Startup.cs
services.Configure<RabbitMqConnectionSettings>(Configuration.GetSection("RabbitMQ"));
services.AddTransient<RabbitMQConnection>();
services.AddTransient<ISendMessages, RabbitMQMessageBus>();

// Controller
public class MessageController : Controller
{
    private readonly ISendMessages _messageBus;
    
    public MessageController(ISendMessages messageBus)
    {
        _messageBus = messageBus;
    }
    
    [HttpPost]
    public async Task<IActionResult> Send([FromBody] MessageDto message)
    {
        var baseMessage = new BaseMessage();
        await _messageBus.SendMessageAsync(baseMessage, "notifications", "emails");
        return Ok();
    }
}
```

### مثال مدیریت فایل

```csharp
[HttpPost]
public async Task<IActionResult> Upload(IFormFile file)
{
    var fileManager = new ManageFiles();
    string path = await fileManager.UploadFileAsync("wwwroot/uploads", file);
    return Ok(new { Path = path });
}
```

## کارت‌خوان PC-POS

اتصال به کارت‌خوان‌های PC-POS بانکی (پروتکل CPML) از طریق پورت سریال یا شبکه. پورت‌شده از پروژه PC-POS Aramestan.

```csharp
using SayehBanTools.PCPOS;

// تنظیمات اتصال
var settings = new PosConnectionSettings
{
    Brand = PosBrand.SayanKart,            // برند دستگاه
    ConnectionType = PosConnectionType.SerialPort,
    SerialPort = "COM3",
    BaudRate = 38400,
    SerialTimeout = 30
    // یا حالت شبکه:
    // ConnectionType = PosConnectionType.Lan,
    // LanIp = "192.168.1.100",
    // LanPort = 16028,                     // پورت پیش‌فرض CPML
    // LanTimeout = 60
};

// راه اول: از طریق Singleton
var pos = PosCommander.Instance;
if (pos.Init(settings))
{
    Response result = pos.Purchase("فاکتور ۱۲۳", "50000", print: true);
    if (result.Success && result.TransactionInfo != null)
        Console.WriteLine($"خرید موفق - RRN: {result.TransactionInfo.RRN}، مبلغ: {result.TransactionInfo.Amount}");
}
else
{
    Console.WriteLine("اتصال به کارت‌خوان برقرار نشد");
}

// راه دوم: از طریق Factory (بدون Singleton) - مناسب تزریق وابستگی
IPosProtocol protocol = PosProtocolFactory.Create(PosBrand.SayanKart, settings);
Response info = protocol.GetPosInfo();
Console.WriteLine($"ترمینال: {info.PosInformation.TerminalId}");
```

سایر عملیات: `BalanceInquiry`، `BillPayment`، `LastTransaction`، `LastPcPosTransaction`، `PrintText`، `PrintBarcode`.
کدهای خطای `Response.ErrorCode`: 201 عدم اتصال، 202 خطای ارسال، 203 پاسخ خالی، 204 پاسخ نامعتبر.

## کمکی‌های فرم ویندوزی (Helper)

کلاس‌های کمکی عمومی برای برنامه‌های Windows Forms (namespace `SayehBanTools.Utilities.win.Helper` و `SayehBanTools.Utilities.Helper`):

- `GhahShomar` — تقویم شمسی با نام روز هفته، مناسبت‌های رسمی ایران و تاریخ ساده: `new GhahShomar(DateTime.Now).SimpleDate()`
- `InventoryUi` — پیام‌های راست‌به‌چپ (`Info/Warn/Error/Ask`)، تبدیل شمسی ↔ میلادی، بازه UTC، مالیات و تخفیف درصدی اعشاری و تجزیه عدد از متن فارسی (`ParseDecimal`، `ParsePercent`)
- `FormValidation` — قواعد مشترک اعتبارسنجی فرم (کامبو، موبایل، نام کاربری، رمز و تکرار، تصویر) با نمایش خطای RTL
- `DateInput` — ورود تاریخ شمسی/میلادی در `SmartTextBox` با چک‌باکس تعیین نوع تاریخ؛ میلادی خودکار به شمسی تبدیل می‌شود
- `TimeInput` — ورود ساعت HH:mm با پذیرش فرمت‌های 8، 08:30، 0830 و ارقام فارسی
- `DocumentNumber` — پیشوند شماره اسناد (سال / سال‌وماه / سال‌وماه‌وروز) با تقویم شمسی؛ فرمت با `DocumentNumber.FormatSelector` انتخاب می‌شود
- `GridRightAlign` — راست‌چین کردن داده‌های همه جدول‌ها (به‌ویژه در فرم‌های RTL)؛ استثنا با Tag ستون «NoRightAlign»
- `EnterNavigation` — حرکت با Enter بین کنترل‌ها و خانه‌های جدول (DGV و cmbbox سایه‌بان را هم می‌شناسد)
- `ScreenFit` — جا شدن فرم در صفحه‌های کوچک (اسکرول و وسط‌چین) و `AddFittedForm<TForm>()` برای ثبت فرم در DI با اتصال خودکار ScreenFit + EnterNavigation + AccessControl
- `AccessControl` — کنترل دسترسی (RBAC) مبتنی بر منوی اصلی: همگام‌سازی درخت دسترسی، مخفی‌سازی منو و غیرفعال‌سازی دکمه‌ها. پیاده‌سازی:
  ```csharp
  AccessControl.ServiceFactory = () => myPermissionService;   // پیاده‌سازی IPermissionService
  AccessControl.Map = new[] { ("MnUsers", "FrmUsers", "SEDP") }; // منو ← فرم ← عملیات‌ها
  await AccessControl.InitializeAsync(mainMenuStrip, userId);
  bool ok = AccessControl.Can("MnUsers", "Save");
  ```
- `PlayerSlider` — نوار کشویی باریک (زمان فیلم و صدا) با رویداد Scrubbing/ValueCommitted

## پخش فیلم آموزشی

`LearningVideo` + `FrmLearningPlayer` (namespace `SayehBanTools.Utilities.win.Learning`) با موتور VLC تقریباً همه فرمت‌ها را پخش می‌کند و محل توقف فیلم را برای ادامه‌ی دوباره ذخیره می‌کند:

```csharp
LearningVideo.FormsNamespace = "MyApp.Forms";   // پیشوند namespace فرم‌های برنامه شما
LearningVideo.Show(this);                       // پوشه Learning کنار exe، با مسیر فرم
```

الزامات: موتور بومی VLC باید در پوشه `libvlc\win-x64` (یا win-x86) کنار exe باشد. برای این کار بسته `VideoLAN.LibVLC.Windows` را به **برنامه مصرف‌کننده** اضافه کنید:

```xml
<PackageReference Include="VideoLAN.LibVLC.Windows" Version="3.0.21" />
```

پکیج SayehBanTools خودش به `LibVLCSharp` و `LibVLCSharp.WinForms` وابسته است (این بسته‌ها نativه ندارند)؛ اگر پوشه libvlc نباشد فقط هنگام پخش فیلم پیام خطا می‌دهد و بقیه کتابخانه سالم است.

## مشارکت

مشارکت‌های شما استقبال می‌شود! برای مشارکت:

1. ریپوی را Fork کنید
2. شاخه جدید ایجاد کنید (`git checkout -b feature/AmazingFeature`)
3. تغییرات را Commit کنید (`git commit -m 'Add some AmazingFeature'`)
4. Push به شاخه (`git push origin feature/AmazingFeature`)
5. Pull Request باز کنید

## مجوز

این پروژه تحت مجوز MIT منتشر شده است. برای اطلاعات بیشتر فایل [LICENSE](LICENSE) را مطالعه کنید.

---

📧 برای ارتباط: sdvp1991david@gmail.com  
🌍 وبسایت: [https://sayehban.ir](https://sayehban.ir)  
💻 مخزن کد: [https://github.com/SayyehBan/SayehBanTools](https://github.com/SayyehBan/SayehBanTools)