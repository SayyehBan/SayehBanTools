using Newtonsoft.Json;
using System.Net;
using static SayehBanTools.Model.Entities.PublicModel;

namespace SayehBanTools.NetworkUtilities
{
    /// <summary>
    /// بررسی شبکه
    /// </summary>
    public class NetworkChecker
    {
        /// <summary>
        /// بررسی اتصال اینترنت با امکان تغییر آدرس تست
        /// </summary>
        /// <returns></returns>
        public bool CheckInternetConnection(string hostToPing)
        {
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var result = ping.Send(hostToPing);
                    return result.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// بررسی اتصال اینترنت با امکان تغییر آدرس تست
        /// </summary>
        /// <returns></returns>
        public bool CheckInternetConnection()
        {
            try
            {
                using (var ping = new System.Net.NetworkInformation.Ping())
                {
                    var result = ping.Send("www.google.com");
                    return result.Status == System.Net.NetworkInformation.IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// دریافت آدرس IP محلی سیستم
        /// </summary>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "127.0.0.1";
        }
        /// <summary>
        /// دریافت IP عمومی (با متد جایگزین، چون وای‌فای ممکن است مشکل‌ساز باشد)
        /// </summary>
        /// <remarks>
        /// در نسخه net11 به‌جای System.Net.WebClient (که در NET. جدید منسوخ است) از HttpClient استفاده می‌شود.
        /// </remarks>
        public static async Task<string> GetPublicIPAddressAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string json = await client.GetStringAsync("https://ipwho.is/");

                    // Deserialize کامل JSON
                    var ipData = JsonConvert.DeserializeObject<IpResponse>(json);

                    return ipData?.ip ?? "127.0.0.1";
                }
            }
            catch
            {
                return "127.0.0.1";
            }
        }
    }
}