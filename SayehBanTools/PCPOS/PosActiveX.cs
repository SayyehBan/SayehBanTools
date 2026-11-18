using Newtonsoft.Json;

namespace SayehBanTools.PCPOS;

/// <summary>
/// نمای JSON از عملیات کارت‌خوان (معادل ActiveX اصلی)؛ مناسب برای برنامه‌هایی که با رشته کار می‌کنند.
/// </summary>
public class PosActiveX
{
    public string Purchase(string additionalData, string amount, bool print = true, string carId = "", string carOwner = "", string subject = "")
        => Serialize(PosCommander.Instance.Purchase(additionalData, amount, print, carId, carOwner, subject));

    public string BalanceInquiry(string additionalData, bool print = true)
        => Serialize(PosCommander.Instance.BalanceInquiry(additionalData, print));

    public string BillPayment(string additionalData, string billId, string paymentId, bool print)
        => Serialize(PosCommander.Instance.BillPayment(additionalData, billId, paymentId, print));

    public string GetPosInfo() => Serialize(PosCommander.Instance.GetPosInfo());

    public string GetLastTransaction() => Serialize(PosCommander.Instance.LastTransaction());

    public string GetLastPcPosTransaction() => Serialize(PosCommander.Instance.LastPcPosTransaction());

    public string PrintText(string text, Language lang = Language.fa)
        => Serialize(PosCommander.Instance.PrintText(text, lang));

    public string PrintBarcode(string barcode, Language lang = Language.fa)
        => Serialize(PosCommander.Instance.PrintBarcode(barcode, lang));

    public bool Init(string com, int timeOut, string baudRate = "38400") =>
        PosCommander.Instance.Init(new PosConnectionSettings
        {
            ConnectionType = PosConnectionType.SerialPort,
            SerialPort = com,
            BaudRate = int.Parse(baudRate),
            SerialTimeout = timeOut
        });

    public bool InitSocket(string host, int lanPort = 16028, int timeOut = 60) =>
        PosCommander.Instance.Init(new PosConnectionSettings
        {
            ConnectionType = PosConnectionType.Lan,
            LanIp = host,
            LanPort = lanPort,
            LanTimeout = timeOut
        });

    private static string Serialize(Response response) => JsonConvert.SerializeObject(response);
}