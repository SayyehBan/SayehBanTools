using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SayehBanTools.Utilities.win.Helper;

/// <summary>گره درخت دسترسی: منو (Group)، فرم (Form) یا عملیات (Action)</summary>
public enum NodeTypes { Group, Form, Action }

/// <summary>یک گره دسترسی برای همگام‌سازی با دیتابیس</summary>
public class Permission_Item
{
    public string PermissionKey { get; set; }
    public string ParentKey { get; set; }
    public string Title { get; set; }
    public NodeTypes NodeType { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// سرویس دسترسی که برنامه مصرف‌کننده پیاده‌سازی می‌کند:
/// SyncAsync = همگام‌سازی درخت دسترسی، GetByUserAsync = دسترسی کاربر (مدیر + کلیدهای مجاز)
/// </summary>
public interface IPermissionService
{
    Task SyncAsync(List<Permission_Item> catalog);
    Task<(bool IsAdmin, List<string> Keys)> GetByUserAsync(int userId);
}

/// <summary>
/// سطح دسترسی کاربران (RBAC مبتنی بر منو):
/// - درخت دسترسی هنگام اجرا از روی منوی اصلی ساخته و در دیتابیس همگام می‌شود
/// - دسترسی کاربر یک بار بعد از ورود در حافظه (static) بارگذاری می‌شود؛ فرم‌ها برای بررسی دسترسی به سرور درخواست نمی‌دهند
/// - روی هر فرم ثبت‌شده با AddFittedForm خودکار اعمال می‌شود: فرم بدون مجوز باز نمی‌شود، دکمه‌های ذخیره / ویرایش / حذف / چاپ
///   بدون مجوز غیرفعال و منوهای آن‌ها مخفی می‌شوند
/// - مدیر سیستم (IsAdmin) به همه چیز دسترسی دارد
/// قبل از استفاده: <see cref="ServiceFactory"/> را به سرویس خود وصل کنید و جدول <see cref="Map"/> را پر کنید.
/// </summary>
public static class AccessControl
{
    public const string Save = "Save";
    public const string Edit = "Edit";
    public const string Delete = "Delete";
    public const string Print = "Print";

    private static readonly Dictionary<string, string> ActionTitles = new Dictionary<string, string>
    {
        { Save, "ثبت / ذخیره" }, { Edit, "ویرایش" }, { Delete, "حذف" }, { Print, "چاپ و گزارش" }
    };

    /// <summary>سرویس دسترسی برنامه (دیتابیس / API). قبل از InitializeAsync تنظیم شود.</summary>
    public static Func<IPermissionService> ServiceFactory { get; set; }

    /// <summary>
    /// منوهای باز کننده فرم: نام منو (کلید دسترسی)، نام کلاس فرم (فرمی که فقط از یک منو باز می‌شود) و عملیات‌های دارای مجوز
    /// (S ذخیره، E ویرایش، D حذف، P چاپ). فرمی که از چند منو باز می‌شود (گزارش‌ها) با SetFormKey کلید می‌گیرد.
    /// این جدول مال خود برنامه مصرف‌کننده است و باید هنگام راه‌اندازی پر شود.
    /// </summary>
    public static (string Menu, string Form, string Actions)[] Map { get; set; } =
        Array.Empty<(string Menu, string Form, string Actions)>();

    /// <summary>منوهایی که برای همه آزادند (در درخت دسترسی نمی‌آیند)</summary>
    public static HashSet<string> AlwaysAllowed { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "MnExit", "MnChangePassword", "MnAboutUs"
    };

    /// <summary>منوهایی که فقط مدیر سیستم می‌بیند</summary>
    public static HashSet<string> AdminOnly { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "MnUserPermissions"
    };

    /// <summary>نام‌هایی که شامل این رشته هستند (منوی آموزش و ...) در درخت دسترسی نمی‌آیند</summary>
    public static string SkipNamePart { get; set; } = "Learning";

    private static HashSet<string> _keys = new HashSet<string>();
    private static readonly ConditionalWeakTable<Form, string> _formKeys = new ConditionalWeakTable<Form, string>();
    private static MenuStrip _mainMenu;
    private static int _userId;
    private static bool _refreshing;

    /// <summary>دسترسی کاربر بارگذاری شده است (قبل از ورود همه چیز آزاد است)</summary>
    public static bool IsLoaded { get; private set; }
    /// <summary>مدیر سیستم</summary>
    public static bool IsAdmin { get; private set; }

    public static void Reset()
    {
        IsLoaded = false;
        IsAdmin = false;
        _keys = new HashSet<string>();
    }

    /// <summary>دسترسی به کلید (منو / فرم) یا «کلید.عملیات»</summary>
    public static bool Can(string key)
    {
        if (!IsLoaded || IsAdmin || string.IsNullOrEmpty(key)) return true;
        return _keys.Contains(key);
    }

    /// <summary>دسترسی به عملیات یک فرم؛ عملیاتی که برای آن فرم تعریف نشده آزاد است</summary>
    public static bool Can(string key, string action)
    {
        if (string.IsNullOrEmpty(key) || !HasAction(key, action)) return true;
        return Can(key + "." + action);
    }

    public static bool HasAction(string key, string action)
    {
        var entry = Map.FirstOrDefault(m => m.Menu == key);
        if (entry.Menu == null) return false;
        return (entry.Actions ?? "").Contains(action[0]);
    }

    /// <summary>کلید دسترسی فرمی که از چند منو باز می‌شود (قبل از ShowDialog)</summary>
    public static void SetFormKey(Form form, string key)
    {
        if (form == null || string.IsNullOrEmpty(key)) return;
        _formKeys.Remove(form);
        _formKeys.Add(form, key);
    }

    public static string KeyOf(Form form)
    {
        if (form == null) return null;
        if (_formKeys.TryGetValue(form, out string key)) return key;
        string name = form.GetType().Name;
        var entry = Map.FirstOrDefault(m => m.Form == name);
        return entry.Menu;
    }

    // ------------------------------------------------------------------ بارگذاری و همگام‌سازی

    /// <summary>
    /// بعد از ورود (فرم اصلی): ساخت درخت دسترسی از روی منو، همگام‌سازی با دیتابیس، بارگذاری دسترسی کاربر و اعمال روی منو
    /// </summary>
    public static async Task InitializeAsync(MenuStrip mainMenu, int userId)
    {
        _mainMenu = mainMenu;
        _userId = userId;
        var service = ServiceFactory?.Invoke();
        if (service == null) return;

        try
        {
            await service.SyncAsync(BuildCatalog(mainMenu));
        }
        catch { /* همگام‌سازی درخت نباید ورود را متوقف کند */ }

        try
        {
            var (isAdmin, keys) = await service.GetByUserAsync(userId);
            IsAdmin = isAdmin;
            _keys = new HashSet<string>(keys ?? new List<string>());
            IsLoaded = true;
        }
        catch
        {
            // دیتابیس هنوز به‌روزرسانی نشده (جدول‌های دسترسی ندارد): رفتار قبلی، همه چیز آزاد
            Reset();
        }
        ApplyMainMenu(mainMenu);
    }

    /// <summary>
    /// همگام‌سازی دوره‌ای (تایمر فرم اصلی): دسترسی کاربر دوباره از دیتابیس خوانده می‌شود تا تغییری که مدیر از رایانه دیگری داده
    /// بدون خروج و ورود دوباره اعمال شود. منوی اصلی در هر دو جهت (دادن / گرفتن دسترسی) به‌روز می‌شود؛ در فرم‌های باز فقط
    /// دکمه‌ها و منوهای عملیاتی که دسترسی‌شان گرفته شده غیرفعال می‌شوند (دسترسی تازه داده‌شده با باز کردن دوباره فرم اعمال می‌شود).
    /// خطای شبکه / دیتابیس دسترسی فعلی را تغییر نمی‌دهد. خروجی: آیا دسترسی تغییر کرد.
    /// </summary>
    public static async Task<bool> RefreshAsync()
    {
        if (_mainMenu == null || _userId <= 0 || _refreshing) return false;
        var service = ServiceFactory?.Invoke();
        if (service == null) return false;

        _refreshing = true;
        try
        {
            var (isAdmin, keys) = await service.GetByUserAsync(_userId);
            var newKeys = new HashSet<string>(keys ?? new List<string>());
            if (IsLoaded && isAdmin == IsAdmin && newKeys.SetEquals(_keys)) return false;

            IsAdmin = isAdmin;
            _keys = newKeys;
            IsLoaded = true;
            if (!_mainMenu.IsDisposed) ApplyMainMenu(_mainMenu);
            ApplyOpenForms();
            return true;
        }
        catch
        {
            return false;   // شبکه یا دیتابیس در دسترس نیست: دسترسی فعلی حفظ می‌شود
        }
        finally { _refreshing = false; }
    }

    /// <summary>اعمال دسترسی‌های گرفته‌شده روی فرم‌های باز (بدون بستن فرم تا داده‌های در حال ورود از دست نرود)</summary>
    private static void ApplyOpenForms()
    {
        if (IsAdmin) return;
        foreach (var form in Application.OpenForms.Cast<Form>().ToList())
        {
            if (form.IsDisposed) continue;
            string key = KeyOf(form);
            if (string.IsNullOrEmpty(key)) continue;
            ApplyControls(form, key);
            foreach (var strip in ToolStripsOf(form))
                ApplyToolStrip(strip.Items, key);
        }
    }

    /// <summary>درخت دسترسی: گروه‌های منو، منوهای باز کننده فرم و عملیات‌های هر فرم</summary>
    public static List<Permission_Item> BuildCatalog(MenuStrip mainMenu)
    {
        var list = new List<Permission_Item>();
        int order = 0;
        foreach (ToolStripItem item in mainMenu.Items)
            AddNode(list, item, null, ref order);
        return list;
    }

    private static bool IsAlwaysAllowed(string name) =>
        AlwaysAllowed.Contains(name) || (!string.IsNullOrEmpty(SkipNamePart) && name.Contains(SkipNamePart));

    private static bool AddNode(List<Permission_Item> list, ToolStripItem item, string parentKey, ref int order)
    {
        if (!(item is ToolStripMenuItem mi) || string.IsNullOrEmpty(mi.Name)) return false;
        if (IsAlwaysAllowed(mi.Name) || AdminOnly.Contains(mi.Name)) return false;

        var children = mi.DropDownItems.OfType<ToolStripMenuItem>().ToList();
        string title = CleanTitle(mi.Text);
        if (children.Count > 0)
        {
            var node = new Permission_Item { PermissionKey = mi.Name, ParentKey = parentKey, Title = title, NodeType = NodeTypes.Group, SortOrder = order++ };
            list.Add(node);
            bool any = false;
            foreach (var child in children)
                any |= AddNode(list, child, mi.Name, ref order);
            if (!any) list.Remove(node);
            return any;
        }

        list.Add(new Permission_Item { PermissionKey = mi.Name, ParentKey = parentKey, Title = title, NodeType = NodeTypes.Form, SortOrder = order++ });
        var entry = Map.FirstOrDefault(m => m.Menu == mi.Name);
        foreach (var action in new[] { Save, Edit, Delete, Print })
        {
            if ((entry.Actions ?? "").Contains(action[0]))
                list.Add(new Permission_Item
                {
                    PermissionKey = mi.Name + "." + action,
                    ParentKey = mi.Name,
                    Title = ActionTitles[action],
                    NodeType = NodeTypes.Action,
                    SortOrder = order++
                });
        }
        return true;
    }

    private static string CleanTitle(string text) => (text ?? "").Replace("&", "").Trim();

    // ------------------------------------------------------------------ اعمال روی منوی اصلی

    /// <summary>منوهای بدون مجوز مخفی می‌شوند؛ گروهی که هیچ زیرمنوی مجاز ندارد هم مخفی می‌شود</summary>
    public static void ApplyMainMenu(MenuStrip mainMenu)
    {
        foreach (ToolStripItem item in mainMenu.Items)
            ApplyMenuNode(item);
    }

    private static bool ApplyMenuNode(ToolStripItem item)
    {
        if (!(item is ToolStripMenuItem mi)) return item.Available;
        if (IsAlwaysAllowed(mi.Name)) return true;
        if (AdminOnly.Contains(mi.Name))
        {
            mi.Available = !IsLoaded || IsAdmin;
            return mi.Available;
        }

        var children = mi.DropDownItems.OfType<ToolStripMenuItem>().ToList();
        bool allowed;
        if (children.Count > 0)
        {
            allowed = false;
            foreach (var child in children)
                allowed |= ApplyMenuNode(child);
        }
        else
            allowed = Can(mi.Name);

        mi.Available = allowed;
        return allowed;
    }

    // ------------------------------------------------------------------ اعمال روی فرم‌ها

    /// <summary>در AddFittedForm صدا زده می‌شود: هنگام Load دسترسی فرم اعمال می‌شود</summary>
    public static void Attach(Form form)
    {
        if (form == null) return;
        form.Load += (s, e) => Apply(form);
    }

    public static void Apply(Form form)
    {
        if (!IsLoaded || IsAdmin) return;
        string key = KeyOf(form);
        if (string.IsNullOrEmpty(key)) return;

        if (!Can(key))
        {
            MessageBox.Show("شما به این فرم دسترسی ندارید. برای گرفتن دسترسی با مدیر سیستم تماس بگیرید.", "عدم دسترسی",
                MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            // بستن بدون پرسش «خروج از فرم؟»: فرم مودال با DialogResult، فرم عادی با Dispose
            form.BeginInvoke(new Action(() =>
            {
                if (form.Modal) form.DialogResult = DialogResult.Cancel;
                else form.Dispose();
            }));
            return;
        }

        ApplyControls(form, key);
        foreach (var strip in ToolStripsOf(form))
            ApplyToolStrip(strip.Items, key);
    }

    /// <summary>همه نوارابزارها و منوهای فرم (حتی منوی راست‌کلیکی که به کنترلی وصل نیست)</summary>
    private static IEnumerable<ToolStrip> ToolStripsOf(Form form)
    {
        var found = new HashSet<ToolStrip>();
        foreach (var f in form.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (typeof(ToolStrip).IsAssignableFrom(f.FieldType) && f.GetValue(form) is ToolStrip ts)
                found.Add(ts);
        }
        return found;
    }

    private static void ApplyControls(Control root, string key)
    {
        foreach (Control c in root.Controls)
        {
            if (c is Button btn)
            {
                var actions = RequiredActions(btn.Name);
                if (actions.Count > 0 && !actions.All(a => Can(key, a)))
                {
                    btn.Enabled = false;
                    btn.EnabledChanged += (s, e) => { if (btn.Enabled) btn.Enabled = false; };
                }
            }
            if (c.HasChildren) ApplyControls(c, key);
        }
    }

    private static void ApplyToolStrip(ToolStripItemCollection items, string key)
    {
        foreach (ToolStripItem item in items)
        {
            var actions = RequiredActions(item.Name);
            if (actions.Count > 0 && !actions.All(a => Can(key, a)))
            {
                item.Enabled = false;    // کلید میانبر منوی مخفی هم کار نکند
                item.Available = false;
                item.EnabledChanged += (s, e) => { if (item.Enabled) item.Enabled = false; };
                item.AvailableChanged += (s, e) => { if (item.Available) item.Available = false; };
            }
            if (item is ToolStripDropDownItem dd && dd.HasDropDownItems)
                ApplyToolStrip(dd.DropDownItems, key);
        }
    }

    /// <summary>عملیات لازم برای یک دکمه / منو از روی نام آن (BtnSave، MnDelete، BtnSaveAndPrint، ذخیرهToolStripMenuItem ...)</summary>
    public static List<string> RequiredActions(string name)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(name) || (!string.IsNullOrEmpty(SkipNamePart) && name.Contains(SkipNamePart))) return result;

        bool english = name.StartsWith("Btn", StringComparison.OrdinalIgnoreCase) || name.StartsWith("Mn", StringComparison.OrdinalIgnoreCase);
        bool persian = name.EndsWith("ToolStripMenuItem") && name.Length > 0 && name[0] > 127;
        if (!english && !persian) return result;

        if (english)
        {
            string n = name.ToLowerInvariant();
            if (n.Contains("save")) result.Add(Save);
            if (n.Contains("edit") || n.Contains("update")) result.Add(Edit);
            if (n.Contains("delete")) result.Add(Delete);
            if (n.Contains("print")) result.Add(Print);
        }
        else
        {
            if (name.StartsWith("ذخیره")) result.Add(Save);
            if (name.StartsWith("ویرایش") || name.StartsWith("بروزرسانی")) result.Add(Edit);
            if (name.StartsWith("حذف")) result.Add(Delete);
            if (name.StartsWith("چاپ")) result.Add(Print);
        }
        return result;
    }
}