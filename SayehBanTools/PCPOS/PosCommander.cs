namespace SayehBanTools.PCPOS;

/// <summary>
/// نقطه راحتی Singleton (ترد-سیف) برای کار با کارت‌خوان، هم‌شکل نسخه اصلی Aramestan.
/// استفاده از طریق PosProtocolFactory هم بدون Singleton ممکن است.
/// </summary>
public class PosCommander
{
    private static readonly object Lock = new object();
    private static PosCommander? instance;

    private IPosProtocol? protocol;

    private PosCommander() { }

    public static PosCommander Instance
    {
        get
        {
            lock (Lock)
            {
                return instance ??= new PosCommander();
            }
        }
    }

    /// <summary>آماده‌سازی پروتکل و تست اتصال، بر اساس تنظیمات.</summary>
    public bool Init(PosConnectionSettings settings)
    {
        try
        {
            protocol = PosProtocolFactory.Create(settings);
            return true;
        }
        catch (Exception)
        {
            protocol = null;
            return false;
        }
    }

    public Response Purchase(string additionalData, string amount, bool print, string carId = "", string carOwner = "", string subject = "")
        => RequireProtocol().Purchase(additionalData, amount, print, carId, carOwner, subject);

    public Response BalanceInquiry(string additionalData, bool print)
        => RequireProtocol().BalanceInquiry(additionalData, print);

    public Response BillPayment(string additionalData, string billId, string paymentId, bool print)
        => RequireProtocol().BillPayment(additionalData, billId, paymentId, print);

    public Response GetPosInfo() => RequireProtocol().GetPosInfo();

    public Response LastTransaction() => RequireProtocol().LastTransaction();

    public Response LastPcPosTransaction() => RequireProtocol().LastPcPosTransaction();

    public Response PrintText(string text, Language lang) => RequireProtocol().PrintText(text, lang);

    public Response PrintBarcode(string barcode, Language lang) => RequireProtocol().PrintBarcode(barcode, lang);

    private IPosProtocol RequireProtocol()
        => protocol ?? throw new InvalidOperationException("کارت‌خوان مقداردهی نشده است. اول PosCommander.Instance.Init(settings) را صدا بزنید.");
}