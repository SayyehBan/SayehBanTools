using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows.Forms;

namespace SayehBanTools.Utilities.win.component.CommonControls
{
    public class DGV : DataGridView
    {
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public bool GONextCell { get; set; }

        public DGV()
        {
            DoubleBuffered = true;
            // فعال‌سازی امکان جابه‌جایی ستون‌ها با درگ و دراپ
            this.AllowUserToOrderColumns = true;
        }

        #region حفظ و بازیابی ترتیب ستون‌ها (Column Reordering Persist)

        // نگهداری ساده کلید/مقدار؛ جایگزین Properties.Settings در NET Framework.
        // (در پروژه SDK-style فایل Settings.Designer.cs کپی نمی‌شود)
        private static class DgvSettingsStore
        {
            private static readonly string FilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "SayehBanTools", "DGVColumnOrders.json");

            public static Dictionary<string, List<string>> Load()
            {
                try
                {
                    if (!File.Exists(FilePath)) return new Dictionary<string, List<string>>();
                    string json = File.ReadAllText(FilePath);
                    return JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json)
                           ?? new Dictionary<string, List<string>>();
                }
                catch
                {
                    return new Dictionary<string, List<string>>();
                }
            }

            public static void Save(Dictionary<string, List<string>> data)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
                    string json = JsonSerializer.Serialize(data);
                    File.WriteAllText(FilePath, json);
                }
                catch
                {
                    // نادیده گرفتن خطاهای احتمالی دسترسی به تنظیمات
                }
            }
        }

        private string GetSettingsKey()
        {
            Form? parentForm = this.FindForm();
            string formName = parentForm != null ? parentForm.Name : "UnknownForm";
            return $"{formName}_{this.Name}_ColumnOrders";
        }

        /// <summary>
        /// ذخیره‌سازی ترتیب ستون‌ها در تنظیمات برنامه
        /// </summary>
        public void SaveColumnOrders()
        {
            try
            {
                if (this.Columns.Count == 0) return;

                string key = GetSettingsKey();
                List<string> columnOrders = new List<string>();

                foreach (DataGridViewColumn col in this.Columns)
                {
                    // ذخیره نام ستون و اندیس نمایشی آن به فرمت Name:DisplayIndex
                    columnOrders.Add($"{col.Name}:{col.DisplayIndex}");
                }

                // ذخیره در تنظیمات
                Dictionary<string, List<string>> settings = DgvSettingsStore.Load();

                string data = key + "=" + string.Join(";", columnOrders);
                settings[key] = new List<string> { data };
                DgvSettingsStore.Save(settings);
            }
            catch
            {
                // نادیده گرفتن خطاهای احتمالی دسترسی به تنظیمات
            }
        }

        /// <summary>
        /// بازیابی ترتیب ستون‌ها از تنظیمات برنامه
        /// </summary>
        public void RestoreColumnOrders()
        {
            try
            {
                if (this.Columns.Count == 0) return;

                string key = GetSettingsKey();
                Dictionary<string, List<string>> settings = DgvSettingsStore.Load();
                if (!settings.TryGetValue(key, out List<string>? entries) || entries == null || entries.Count == 0)
                    return;

                string keyPrefix = key + "=";
                string? savedData = null;
                foreach (string item in entries)
                {
                    if (item.StartsWith(keyPrefix, StringComparison.Ordinal))
                    {
                        savedData = item.Substring(keyPrefix.Length);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(savedData)) return;

                string[] colPairs = savedData.Split(';');
                Dictionary<string, int> orders = new Dictionary<string, int>();

                foreach (string pair in colPairs)
                {
                    string[] parts = pair.Split(':');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int displayIndex))
                    {
                        orders[parts[0]] = displayIndex;
                    }
                }

                // اعمال اندیس نمایشی جدید به ستون‌ها
                foreach (DataGridViewColumn col in this.Columns)
                {
                    if (orders.ContainsKey(col.Name))
                    {
                        int targetIndex = orders[col.Name];
                        if (targetIndex >= 0 && targetIndex < this.Columns.Count)
                        {
                            col.DisplayIndex = targetIndex;
                        }
                    }
                }
            }
            catch
            {
                // نادیده گرفتن خطاهای احتمالی بازیابی
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            Form? parentForm = this.FindForm();
            if (parentForm != null)
            {
                // ذخیره‌سازی خودکار هنگام بسته شدن فرم
                parentForm.FormClosing += (s, args) => SaveColumnOrders();
            }
        }

        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);

            // شماره‌گذاری سطرهای هدر
            this.RowHeadersWidth = 41;
            for (int i = 0; i < this.Rows.Count; i++)
            {
                this.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }

            this.ClearSelection();
            this.AlternatingRowsDefaultCellStyle.BackColor = SystemColors.Control;

            // بازیابی ترتیب ستون‌ها پس از کامل شدن DataBinding
            RestoreColumnOrders();
        }

        #endregion

        #region سایر متدهای قبلی (تغییر فوکوس، اکسل و ...)

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (GONextCell && keyData == Keys.Enter)
            {
                base.ProcessTabKey(Keys.Tab);
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (GONextCell && e.KeyCode == Keys.Enter)
            {
                base.ProcessTabKey(Keys.Tab);
                return true;
            }
            return base.ProcessDataGridViewKey(e);
        }

        private Color _Alternating = Color.Yellow;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public Color AlteraningColor
        {
            get => _Alternating;
            set
            {
                _Alternating = value;
                this.AlternatingRowsDefaultCellStyle.BackColor = this._Alternating;
            }
        }

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            base.OnEditingControlShowing(e);
            e.CellStyle.BackColor = Color.Pink;
        }

        public int Selected_Row = -1, Selected_Column = -1;

        protected override void OnColumnHeaderMouseDoubleClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnColumnHeaderMouseDoubleClick(e);

            if (this.Rows.Count == 0) return;

            try
            {
                ExportToExcelSafe();
            }
            catch (Exception ex)
            {
                MessageBox.Show("کتابخانه اکسل یافت نشد. لطفاً مطمئن شوید نرم افزار Microsoft Excel روی سیستم نصب است.\n\n" + ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void ExportToExcelSafe()
        {
            // استفاده از Late Binding برای جلوگیری از نیاز به رفرنس دادن DLL های آفیس
            Type? excelType = Type.GetTypeFromProgID("Excel.Application");

            if (excelType == null)
            {
                MessageBox.Show("نرم افزار اکسل روی سیستم شما یافت نشد.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RtlReading);
                return;
            }

            dynamic application = Activator.CreateInstance(excelType);
            dynamic workbook = application.Workbooks.Add();
            dynamic activeSheet = workbook.ActiveSheet;
            activeSheet.Name = "Sheet1";

            int excelColIndex = 1;

            // مرتب‌سازی ستون‌ها بر اساس DisplayIndex فعلی برای خروجی اکسل درست
            var sortedColumns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in this.Columns) sortedColumns.Add(col);
            sortedColumns.Sort((a, b) => a.DisplayIndex.CompareTo(b.DisplayIndex));

            // ۱. خروجی عنوان ستون‌ها
            for (int col = 0; col < sortedColumns.Count; col++)
            {
                if (sortedColumns[col].Visible)
                {
                    activeSheet.Cells[1, excelColIndex] = sortedColumns[col].HeaderText;
                    excelColIndex++;
                }
            }

            // ۲. خروجی مقادیر سلول‌ها
            for (int row = 0; row < this.Rows.Count; row++)
            {
                if (this.Rows[row].IsNewRow) continue;

                excelColIndex = 1;
                for (int col = 0; col < sortedColumns.Count; col++)
                {
                    if (sortedColumns[col].Visible)
                    {
                        var cellValue = this.Rows[row].Cells[sortedColumns[col].Index].Value;
                        activeSheet.Cells[row + 2, excelColIndex] = cellValue?.ToString() ?? string.Empty;
                        excelColIndex++;
                    }
                }
            }

            application.Visible = true;
        }

        #endregion
    }
}