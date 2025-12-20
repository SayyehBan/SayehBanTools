using System.Text;

namespace SayehBanTools.Converter
{
    /// <summary>
    /// کلاس جامع برای تمیز کردن و فرار کردن متن‌های ارسالی به Stored Procedure
    /// مخصوص متن‌های طولانی مثل معنی نام، توضیحات و ...
    /// </summary>
    public static class SafeSqlTextCleaner
    {
        // کاراکترهایی که باید فرار بشن یا جایگزین بشن
        private static readonly Dictionary<string, string> EscapeReplacements = new()
        {
            { "'", "''" },     // تک کوتیشن → دوتا (استاندارد SQL برای فرار)
            { "\"", "\"\"" },  // دابل کوتیشن → دوتا (اختیاری، اما برای امنیت)
            { "--", "––" },    // کامنت SQL → جایگزین با خط تیره مشابه اما بی‌خطر
            { "/*", "/ *" },   // شروع کامنت بلوکی → فاصله اضافه کن
            { "*/", "* /" },   // پایان کامنت بلوکی → فاصله اضافه کن
            { ";--", "; --" }, // پایان دستور + کامنت → فاصله
            { ";", "؛" },      // سمی‌کالن → جایگزین با سمی‌کالن فارسی (بی‌خطر)
            { "\0", "" },      // نال کاراکتر → حذف
            { "\b", "" },      // بک‌اسپیس → حذف
            { "\f", "" },      // فرم فید → حذف
        };

        // کاراکترهای اضافی که فقط تمیز می‌شن (نه خطرناک، اما بهتره حذف/جایگزین بشن)
        private static readonly Dictionary<string, string> CleanReplacements = new()
        {
            { "*", "×" },      // ستاره → علامت ضرب (یا می‌تونی " * " بذاری)
            { "-", "–" },      // خط تیره معمولی → خط تیره بلند (زیباتر)
            { "\r\n", "\n" },  // خط جدید ویندوز → یونیکس
            { "\r", "\n" },    // خط جدید مک قدیمی → یونیکس
            { "\n", " " },     // خط جدید → فاصله (یا اگر می‌خوای حفظ بشه، این خط رو کامنت کن)
            { "\t", " " },     // تب → فاصله
            { "  ", " " },     // فاصله‌های متعدد → یکی
        };

        /// <summary>
        /// تمیز کردن کامل متن برای ارسال امن به SP (Dynamic SQL)
        /// </summary>
        /// <param name="input">متن ورودی (مثل معنی نام)</param>
        /// <param name="preserveNewLines">آیا خط جدید حفظ بشه؟ (پیش‌فرض: خیر، به فاصله تبدیل می‌شه)</param>
        /// <returns>متن امن و تمیز</returns>
        public static string CleanForSql(this string? input, bool preserveNewLines = false)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var sb = new StringBuilder(input.Trim());

            // اول: فرار کاراکترهای خطرناک SQL
            foreach (var replacement in EscapeReplacements)
            {
                sb.Replace(replacement.Key, replacement.Value);
            }

            // دوم: تمیز کردن کاراکترهای اضافی
            foreach (var replacement in CleanReplacements)
            {
                if (replacement.Key == "\n" && preserveNewLines)
                    continue; // اگر می‌خوای خط جدید حفظ بشه، رد شو

                sb.Replace(replacement.Key, replacement.Value);
            }

            // حذف فاصله‌های متعدد
            while (sb.ToString().Contains("  "))
            {
                sb.Replace("  ", " ");
            }

            // حذف فاصله از ابتدا و انتها
            return sb.ToString().Trim();
        }

        /// <summary>
        /// نسخه ساده‌تر: فقط فرار تک کوتیشن (کافی برای اکثر موارد)
        /// </summary>
        public static string EscapeSingleQuote(this string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return input.Replace("'", "''");
        }

        /// <summary>
        /// اگر می‌خوای خط جدید حفظ بشه (مثل پاراگراف)
        /// </summary>
        public static string CleanForSqlPreserveLines(this string? input)
        {
            return input.CleanForSql(preserveNewLines: true);
        }
    }
}