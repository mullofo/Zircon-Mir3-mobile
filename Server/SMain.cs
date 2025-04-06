using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Library;
using MirDB;
using Server.Envir;
using Server.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;

namespace Server
{
    public partial class SMain : DevExpress.XtraBars.Ribbon.RibbonForm
    {
        public List<Control> Windows = new List<Control>();
        public static Session Session;
        private DateTime NextLogTime = Time.Now;
        public SMain()
        {
            InitializeComponent();  //初始化组件
                                    //服务管理器 安全协议
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 |
                                                   SecurityProtocolType.Tls12;

        }

        private void SMain_Load(object sender, EventArgs e)   //读取
        {
            ShowView(typeof(ConfigView));
            ShowView(typeof(SystemLogView));   //显示视图(系统日志视图)
            //var assemblies = new Assembly[] {
            //    Assembly.GetAssembly(typeof(ItemInfo)),
            //    Assembly.GetAssembly(typeof(AccountInfo))
            //};

            //Session = new Session(SessionMode.System, assemblies, Config.EnableDBEncryption, Config.DBPassword)   //数据库读取
            //{
            //    BackUpDelay = 60,   //备份延迟
            //};

            UpdateInterface();  //更新接口

            Application.Idle += Application_Idle;  //应用程序空闲
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            try
            {
                while (AppStillIdle)
                {
                    MapViewer.CurrentViewer?.Process();
                    DiyMonShowViewer.CurrentDiyMonViewer?.Process();
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }

        }

        protected override void OnClosing(CancelEventArgs e)  //关闭时
        {
            base.OnClosing(e);

            if (XtraMessageBox.Show(this, "确定要关闭服务器吗？", "关闭服务器", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            //Session.BackUpDelay = 0;   //数据库备份延迟
            //Session?.Save(true);    //数据库备份开启

            if (SEnvir.OriginalLoop)
            {
                if (SEnvir.EnvirThread == null) return;
                SEnvir.Started = false;
                while (SEnvir.EnvirThread != null) Thread.Sleep(1);
            }
            else
            {
                SEnvir.Stopping = true;
                while (SEnvir.Started) Thread.Sleep(1);
            }

        }
        private void ShowView(Type type)   //显示视图
        {
            try
            {
                foreach (Control item in Windows)
                    if (item.GetType() == type)
                    {
                        tabbedView1.ActivateDocument(item);
                        return;
                    }

                Form view = (Form)Activator.CreateInstance(type);
                view.MdiParent = this;
                view.Disposed += View_Disposed;
                view.Tag = type.Name;
                Windows.Add(view);
                view.Show();
            }
            finally
            { }
        }
        private void View_Disposed(object sender, EventArgs e)  //视图释放
        {
            Windows.Remove((Control)sender);
        }

        private void InterfaceTimer_Tick(object sender, EventArgs e)  //接口计时器
        {
            UpdateInterface();

            if (SEnvir.OriginalLoop)
            {
                if (!SEnvir.Started && SEnvir.EnvirThread == null)
                    InterfaceTimer.Enabled = false;
            }
            else
            {
                if (Time.Now >= NextLogTime)
                {
                    SEnvir.WriteLogs();
                    NextLogTime = Time.Now.AddSeconds(10);
                }

                if (!SEnvir.Started && !SEnvir.Starting && !SEnvir.Stopping)
                    InterfaceTimer.Enabled = false;
            }
        }

        private void UpdateInterface()  //更新接口
        {
            if (SEnvir.OriginalLoop)
            {
                StartServerButton.Enabled = SEnvir.EnvirThread == null;
                StopServerButton.Enabled = SEnvir.Started;
            }
            else
            {
                StartServerButton.Enabled = !SEnvir.Started && !SEnvir.Starting;
                StopServerButton.Enabled = SEnvir.Started && !SEnvir.Stopping;
            }

            ConnectionLabel.Caption = string.Format(@"连接人数: {0:#,##0}", SEnvir.Connections.Count);
            ObjectLabel.Caption = string.Format(@"活动对象: {0} /总对象: {1:#,##0}", SEnvir.ActiveObjects.Count, SEnvir.Objects.Count);//全游戏的活动对象个数
            ProcessLabel.Caption = string.Format(@"进程对象计数: {0:#,##0}", SEnvir.ProcessObjectCount);
            LoopLabel.Caption = string.Format(@"循环计数: {0:#,##0}", SEnvir.LoopCount);
            EMailsSentLabel.Caption = string.Format(@"邮件发送: {0:#,##0}", SEnvir.EMailsSent);

            ConDelay.Caption = string.Format(@"主循环: 连接延迟{0:#,##0}ms | 对象延迟{1:#,##0}ms | 脚本延迟{2:#,##0}ms", SEnvir.ConDelay, SEnvir.MainDelay, SEnvir.ProcessDelay);
            SaveDelay.Caption = string.Format(@"保存延迟: {0:#,##0}ms", SEnvir.SaveDelay);

            const decimal KB = 1024;
            const decimal MB = KB * 1024;
            const decimal GB = MB * 1024;

            if (SEnvir.TotalBytesReceived > GB)
                TotalDownloadLabel.Caption = string.Format(@"接收: {0:#,##0.0}GB", SEnvir.TotalBytesReceived / GB);
            else if (SEnvir.TotalBytesReceived > MB)
                TotalDownloadLabel.Caption = string.Format(@"接收: {0:#,##0.0}MB", SEnvir.TotalBytesReceived / MB);
            else if (SEnvir.TotalBytesReceived > KB)
                TotalDownloadLabel.Caption = string.Format(@"接收: {0:#,##0}KB", SEnvir.TotalBytesReceived / KB);
            else
                TotalDownloadLabel.Caption = string.Format(@"接收: {0:#,##0}B", SEnvir.TotalBytesReceived);

            if (SEnvir.TotalBytesSent > GB)
                TotalUploadLabel.Caption = string.Format(@"发送: {0:#,##0.0}GB", SEnvir.TotalBytesSent / GB);
            else if (SEnvir.TotalBytesSent > MB)
                TotalUploadLabel.Caption = string.Format(@"发送: {0:#,##0.0}MB", SEnvir.TotalBytesSent / MB);
            else if (SEnvir.TotalBytesSent > KB)
                TotalUploadLabel.Caption = string.Format(@"发送: {0:#,##0}KB", SEnvir.TotalBytesSent / KB);
            else
                TotalUploadLabel.Caption = string.Format(@"发送: {0:#,##0}B", SEnvir.TotalBytesSent);


            if (SEnvir.DownloadSpeed > GB)
                DownloadSpeedLabel.Caption = string.Format(@"接收速度: {0:#,##0.0}GBps", SEnvir.DownloadSpeed / GB);
            else if (SEnvir.DownloadSpeed > MB)
                DownloadSpeedLabel.Caption = string.Format(@"接收速度: {0:#,##0.0}MBps", SEnvir.DownloadSpeed / MB);
            else if (SEnvir.DownloadSpeed > KB)
                DownloadSpeedLabel.Caption = string.Format(@"接收速度: {0:#,##0}KBps", SEnvir.DownloadSpeed / KB);
            else
                DownloadSpeedLabel.Caption = string.Format(@"接收速度: {0:#,##0}Bps", SEnvir.DownloadSpeed);

            if (SEnvir.UploadSpeed > GB)
                UploadSpeedLabel.Caption = string.Format(@"发送速度: {0:#,##0.0}GBps", SEnvir.UploadSpeed / GB);
            else if (SEnvir.UploadSpeed > MB)
                UploadSpeedLabel.Caption = string.Format(@"发送速度: {0:#,##0.0}MBps", SEnvir.UploadSpeed / MB);
            else if (SEnvir.UploadSpeed > KB)
                UploadSpeedLabel.Caption = string.Format(@"发送速度: {0:#,##0}KBps", SEnvir.UploadSpeed / KB);
            else
                UploadSpeedLabel.Caption = string.Format(@"发送速度: {0:#,##0}Bps", SEnvir.UploadSpeed);

        }
        //开启服务器按钮
        private void StartServerButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            InterfaceTimer.Enabled = true;
            SEnvir.StartServer();
            UpdateInterface();
        }
        //停止服务器按钮
        private void StopServerButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (SEnvir.OriginalLoop)
            {
                SEnvir.Started = false;
            }
            else
            {
                SEnvir.Stopping = true;
            }

            UpdateInterface();
        }

        //py统计按钮
        private void PyMetricsButton_LinkClicked(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowView(typeof(PyMetricsView));
        }

        //日志按钮
        private void LogNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(SystemLogView));
        }
        //聊天记录按钮
        private void ChatLogNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ChatLogView));
        }
        //设置按钮
        private void ConfigButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ConfigView));
        }
        //地图信息按钮
        private void MapInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MapInfoView));
        }
        //怪物信息按钮
        private void MonsterInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MonsterInfoView));
        }

        public static void SetUpView(GridView view)  //设置界面
        {
            view.BestFitColumns();
            view.KeyPress += PasteData_KeyPress;
            view.KeyDown += DeleteRows_KeyDown;
            view.OptionsSelection.MultiSelect = true;
            view.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CellSelect;
        }
        private static void DeleteRows_KeyDown(object sender, KeyEventArgs e)   //删除行
        {
            if (e.KeyCode != Keys.Delete) return;

            if (MessageBox.Show("删除行？", "确认", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            GridView view = (GridView)sender;
            //view.DeleteSelectedRows();

            int[] rows = view.GetSelectedRows();

            List<DBObject> objects = new List<DBObject>();

            foreach (int index in rows)
                objects.Add((DBObject)view.GetRow(index));

            foreach (DBObject ob in objects)
                ob?.Delete();
        }
        //复制黏贴数据
        public static void PasteData_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((GridView)sender).Name == "MapRegionGridView" || ((GridView)sender).Name == "MapInfoGridView" || ((GridView)sender).Name == "MonsterInfoGridView" || ((GridView)sender).Name == "ItemInfoGridView") return;
            //杨伟，以上四个grid的名字不要随便去改，因为他们支持关联复制，是在本身窗体实现的

            if (e.KeyChar == 0x16)
            {
                e.Handled = true;

                GridView view = (GridView)sender;
                string data = Clipboard.GetText();
                string[] copied = data.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                var rows = view.GetSelectedRows();

                if (rows.Length == 0)
                {
                    //Pasting Column
                    for (int i = 1; i < copied.Length; i++) //Avoid Header
                    {
                        view.AddNewRow();
                        string[] row = copied[i].Split('\t');


                        for (int c = 0; c < row.Length; c++)
                        {
                            if (c >= view.Columns.Count) break;

                            GridColumn column = view.GetVisibleColumn(view.FocusedColumn.VisibleIndex + c);

                            if (column.ColumnType.IsSubclassOf(typeof(DBObject)))
                            {
                                RepositoryItemLookUpEdit tmep = column.ColumnEdit as
                                    RepositoryItemLookUpEdit;

                                if (tmep == null) return;

                                view.SetRowCellValue(view.FocusedRowHandle, column, Session.GetObject(column.ColumnType, tmep.DisplayMember, row[c]));

                            }
                            else if (column.ColumnType.IsSubclassOf(typeof(Enum)))
                            {//2019-11-08 杨伟

                                try
                                {//枚举有两种，一种是有备注的,如LightSetting,另外一种是没有备注的，如SoundIndex，优先走第一种方法，出异常后走第二种
                                    object setting = ExportImportHelp.GetEnumName(row[c], column.ColumnType);
                                    view.SetRowCellValue(view.FocusedRowHandle, column, setting);
                                }
                                catch (System.Exception)
                                {
                                    try
                                    {
                                        object objectEnum = Enum.Parse(column.ColumnType, row[c]);
                                        view.SetRowCellValue(view.FocusedRowHandle, column, objectEnum);
                                    }
                                    catch (System.Exception)
                                    {

                                    }
                                }

                            }
                            else if (column.ColumnType == typeof(bool))
                                view.SetRowCellValue(view.FocusedRowHandle, column, row[c] == "Checked");
                            else if (column.ColumnType == typeof(decimal) && row[c].EndsWith("%"))
                                view.SetRowCellValue(view.FocusedRowHandle, column, decimal.Parse(row[c].TrimEnd('%', ' ')) / 100M);
                            else
                                view.SetRowCellValue(view.FocusedRowHandle, column, row[c]);
                        }
                    }
                    return;
                }

                for (int i = 0; i < rows.Length; i++)
                {
                    //Could paste multiple cells;
                    if (i + 1 >= copied.Length) break;
                    string[] row = copied[i + 1].Split('\t');

                    var cells = view.GetSelectedCells(rows[i]);

                    if (cells.Length != row.Length)
                    {
                        XtraMessageBox.Show("列计数不复制列计数");
                        return;
                    }

                    for (int c = 0; c < cells.Length; c++)
                    {
                        GridColumn column = view.Columns[cells[c].FieldName];

                        if (column.ColumnType.IsSubclassOf(typeof(DBObject)))
                        {
                            RepositoryItemLookUpEdit tmep = column.ColumnEdit as RepositoryItemLookUpEdit;

                            if (tmep == null) return;

                            view.SetRowCellValue(rows[i], column, Session.GetObject(column.ColumnType, tmep.DisplayMember, row[c]));

                        }
                        else if (column.ColumnType.IsSubclassOf(typeof(Enum)))
                        {//2019-11-07 杨伟

                            try
                            {//枚举有两种，一种是有备注的,如LightSetting,另外一种是没有备注的，如SoundIndex，优先走第一种方法，出异常后走第二种
                                object setting = ExportImportHelp.GetEnumName(row[c], column.ColumnType);
                                view.SetRowCellValue(view.FocusedRowHandle, column, setting);
                            }
                            catch (System.Exception)
                            {
                                try
                                {
                                    object objectEnum = Enum.Parse(column.ColumnType, row[c]);
                                    view.SetRowCellValue(view.FocusedRowHandle, column, objectEnum);
                                }
                                catch (System.Exception)
                                {

                                }
                            }

                        }
                        else if (column.ColumnType == typeof(bool))
                            view.SetRowCellValue(rows[i], column, row[c] == "Checked");
                        else if (column.ColumnType == typeof(decimal) && row[c].EndsWith("%"))
                            view.SetRowCellValue(rows[i], column, decimal.Parse(row[c].TrimEnd('%', ' ')) / 100M);
                        else
                            view.SetRowCellValue(rows[i], column, row[c]);
                    }
                }

            }
        }

        //道具信息按钮
        private void ItemInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ItemInfoView));
        }
        //NPC信息按钮
        private void NPCInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(NPCInfoView));
        }
        //NPC页按钮
        private void NPCPageButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(NPCPageView));
        }
        //魔法技能信息按钮
        private void MagicInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MagicInfoView));
        }
        //角色信息按钮
        private void CharacterInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CharacterView));
        }
        //账号信息按钮
        private void AccountInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(AccountView));
        }
        //地图链接信息按钮
        private void MovementInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MovementInfoView));
        }
        //道具属性信息按钮
        private void ItemInfoStatButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ItemInfoStatView));
        }
        //自定义道具特效按钮
        private void ItemCustomInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(ItemCustomInfoView));
        }
        //制作合成道具按钮
        private void ItemComposeButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CraftItemInfoView));
        }
        //道具套装信息按钮
        private void SetInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(SetInfoView));
        }
        //商城信息按钮
        private void StoreInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(StoreInfoView));
        }
        //基础属性设置按钮
        private void BaseStatButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(BaseStatView));
        }

        private void RewardPollNavItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(RewardPoolView));
        }

        #region Idle Check
        private static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        [SuppressUnmanagedCodeSecurity]
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin,
                                               uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            private readonly IntPtr hWnd;
            private readonly Message msg;
            private readonly IntPtr wParam;
            private readonly IntPtr lParam;
            private readonly uint time;
            private readonly Point p;
        }
        #endregion

        //地图查看按钮
        private void barButtonItem1_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }
        //安全区信息按钮
        private void SafeZoneInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(SafeZoneInfoView));
        }
        //钓鱼区信息按钮
        private void FishingAreaInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(FishingAreaInfoView));
        }
        //怪物刷新信息按钮
        private void RespawnInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(RespawnInfoView));
        }
        //地图链接按钮
        private void MapRegionButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MapRegionView));
        }
        //爆率信息按钮
        private void DropInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(DropInfoView));
        }
        //角色爆率按钮
        private void UserDropButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserDropView));
        }
        //人物成就按钮
        private void UserAchievementButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserAchievementView));
        }
        //任务信息按钮
        private void QuestInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(QuestInfoView));
        }
        //宠物信息按钮
        private void CompanionInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CompanionInfoView));
        }
        //事件信息按钮
        private void EventInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(EventInfoView));
        }
        //怪物属性信息按钮
        private void MonsterInfoStatButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MonsterInfoStatView));
        }
        //自定义怪物信息按钮
        private void MonsterCustomInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MonAnimationFrameView));
        }
        //城堡信息按钮
        private void CastleInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CastleInfoView));
        }
        //元宝充值按钮
        private void PaymentButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(GameGoldPaymentView));
        }
        //商城购买记录按钮
        private void StoreSalesButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(GameStoreSaleView));
        }
        //判断工具按钮
        private void DiagnosticButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(DiagnosticView));
        }
        //所有角色道具查询按钮
        private void navBarItem2_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserItemView));
        }
        //所有攻城统计信息按钮
        private void navBarItem3_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserConquestStatsView));
        }
        //所有角色邮件查询按钮
        private void navBarItem4_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserMailView));
        }
        //武器工艺设置按钮
        private void navBarItem5_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(WeaponCraftStatInfoView));
        }
        //配置服务器按钮
        private void barButtonItem2_ItemClick_1(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowView(typeof(ConfigView));
        }
        //成就信息按钮
        private void AchievementInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(AchievementInfoView));
        }
        //触发设置按钮
        private void TriggerButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(TriggerInfoView));
        }
        //角色任务查询按钮
        private void UserQuestButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(UserQuestView));
        }
        //自定义BUFF按钮
        private void BuffInfoButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(BuffInfoView));
        }
        //新版武器升级按钮
        private void WeaponUpgrade_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(WeaponUpgradeInfoView));
        }

        private void GuildButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(GuildInfoView));
        }

        private void DiyMagicEffectButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(DiyMagicEffectView));
        }

        private void MonDiyAiActionButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(MonDiyAiActionView));
        }

        private void InternationalButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ShowView(typeof(LangInfoView));
        }

        private void navBarItem7_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(QuestInfoEditForm));
        }

        private void ImportantLogViewNavBarItem_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CurrencyLogView));
        }

        private void CleanAccountButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CleanAccountView));
        }
    }

}
