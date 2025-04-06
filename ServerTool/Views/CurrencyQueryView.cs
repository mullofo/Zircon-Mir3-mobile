using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using DevExpress.XtraBars;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server.Views
{
    public partial class CurrencyQueryView : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        List<DummyCurrencyLogEntry> _logList = new List<DummyCurrencyLogEntry>();
        public CurrencyQueryView()
        {
            InitializeComponent();

            CurrencyQuery_GridControl.DataSource = _logList;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SMain.SetUpView(CurrencyQuery_GridView);
        }

        private void SaveButton_ItemClick(object sender, ItemClickEventArgs e)
        {
            SMain.Session.Save(true, MirDB.SessionMode.ServerTool);
        }

        private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
        {
            _logList.Clear();
            CurrencyQuery_GridControl.RefreshDataSource();
        }

        private async void barButtonItem2_ItemClick(object sender, ItemClickEventArgs e)
        {
            openFileDialog1.Multiselect = true;
            openFileDialog1.Filter = "CSV 日志文件 (*.csv)|*.csv";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                progressBar1.Maximum = openFileDialog1.FileNames.Length;
                //progressBar1.Step = 1;
                var progress = new Progress<int>(v =>
                {
                    progressBar1.Value = v;
                });
                await Task.Run(() => loadCsvLogs(progress, openFileDialog1.FileNames));
                // 载入完毕
                CurrencyQuery_GridControl.RefreshDataSource();
                progressBar1.Hide();
            }
        }

        private void loadCsvLogs(IProgress<int> progress, string[] files)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                BadDataFound = null,
                MissingFieldFound = args => SEnvir.DisplayLogs.Enqueue($"缺失数据: index = {args.Index}"),
            };


            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                List<DummyCurrencyLogEntry> records;
                try
                {
                    // load
                    using (StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("GB2312")))
                    using (CsvReader csv = new CsvReader(reader, config))
                    {
                        records = csv.GetRecords<DummyCurrencyLogEntry>().ToList();
                    }
                    _logList.AddRange(records);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "导入失败", MessageBoxButtons.OK);
                }

                progress?.Report(i + 1);
            }
        }
    }

    /// <summary>
    /// 避免引入ServerLibrary污染ServerTool
    /// 这里新建一个假的日志class
    /// </summary>
    public class DummyCurrencyLogEntry
    {
        [Index(0)]
        public string LogLevel { get; set; }
        [Index(1)]
        public string Module { get; set; }
        [Index(2)]
        public string Time { get; set; }
        [Index(3)]
        public string PlayerName { get; set; }
        [Index(4)]
        public string Account { get; set; }
        [Index(5)]
        public string LastIP { get; set; }
        [Index(6)]
        public string Currency { get; set; }
        [Index(7)]
        public string Action { get; set; }
        [Index(8)]
        public decimal Amount { get; set; }
        [Index(9)]
        public string Source { get; set; }
        [Index(10)]
        public string ExtraInfo { get; set; }
    }
}