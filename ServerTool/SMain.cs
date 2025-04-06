using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;
using Library.SystemModels;
using MirDB;
using Server.DBModels;
using Server.Envir;
using Server.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
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
        private static GridView gridView;
        private static int[] selectRows = new int[0];
        /// <summary>
        /// 数据库对角所在程序集
        /// </summary>
        public Assembly[] Assemblies { get; private set; }
        private string[] MySqlParms;
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
            ShowView(typeof(SystemLogView));   //显示视图(系统日志视图)

            Assemblies = new Assembly[] {
                Assembly.GetAssembly(typeof(ItemInfo)),
                Assembly.GetAssembly(typeof(AccountInfo)),
            };

            MySqlParms = new string[]
            {
                Config.MySqlUser,
                Config.MySqlPassword,
                Config.MySqlIP,
                Config.MySqlPort,
            };

            Session = new Session(SessionMode.ServerTool, Assemblies, Config.EnableDBEncryption, Config.DBPassword)
            {
                IsMySql = Config.MySqlEnable,
                MySqlParms = new string[] { Config.MySqlUser, Config.MySqlPassword, Config.MySqlIP, Config.MySqlPort, },
            };
            Session.Output += (o, s) =>
            {
                SEnvir.Log(s.ToString());
            };
            Session.Init();

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

            if (XtraMessageBox.Show(this, "确定要关闭数据库编辑器吗？", "关闭数据库编辑器", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            Session.BackUpDelay = 0;   //数据库备份延迟
            Session?.Save(true, SessionMode.ServerTool);    //数据库备份开启

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

        }

        //数据库初始化按钮
        private void FileToSqlButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //if (Session.IsMySql)
            //{
            //    XtraMessageBox.Show("当前数据源为Mysql，不能初始化。\r\n请在配置页面关闭'启用MySql'，从二进制文件启动，然后初始化。。。", "错误");
            //    return;
            //}
            if (XtraMessageBox.Show("注意： 当前数据源：" + (Session.IsMySql ? "MySql" : "二进制文件") + "\r\n1，请确保已经设置MySql Ip地址和端口；\r\n2，请确保已经创建好2个空库，'SystemDB'和'UserDB'。", "提示", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            //如果是mysql数据库，初始化前备份下到文件
            if (Session.IsMySql)
            {
                Session.BackUpDelay = 0;
                Session?.SqlToFile();
            }
            Session?.InitDataBase();
        }
        //MySql数据转换到Z版二进制文件
        private void SqlToFileButton_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (!Session.IsMySql)
            {
                XtraMessageBox.Show("当前数据源为二进制文件，不需要转换。。。。", "提示");
                return;
            }
            Session.BackUpDelay = 0;
            if (Session.SqlToFile())
                XtraMessageBox.Show("转换成功。", "提示");
            else
                XtraMessageBox.Show("转换失败。", "提示");
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
            try
            {
                if (e.KeyChar == 0x3)
                {
                    e.Handled = true;

                    gridView = (GridView)sender;
                    selectRows = gridView.GetSelectedRows();

                    if (selectRows.Length == 0) return;

                    List<DBObject> objects = new List<DBObject>();

                    foreach (int index in selectRows)
                        objects.Add((DBObject)gridView.GetRow(index));

                    Clipboard.SetAudio(Session.CopyObject(objects));
                }

                if (e.KeyChar == 0x16)
                {
                    e.Handled = true;

                    GridView view = (GridView)sender;
                    int[] rows = view.GetSelectedRows();
                    IBindingList list = (IBindingList)view.DataSource;

                    //if (rows.Length > 0 && selectRows.Length > 0)
                    //{
                    //    foreach (int RowIndex in selectRows)
                    //    {
                    //        DBObject oldOb = gridView.GetRow(RowIndex) as DBObject;
                    //        if (list.GetType().GenericTypeArguments[0] != oldOb.GetType()) return;
                    //        Session.PasteObject(oldOb.GetType(), oldOb, (DBObject)list.AddNew());
                    //    }
                    //    XtraMessageBox.Show("复制成功，请到最后一行查看。", "提示");
                    //}
                    //else
                    {
                        MemoryStream mem = Clipboard.GetAudioStream() as MemoryStream;
                        if (mem != null)
                        {
                            using (mem)
                            using (BinaryReader reader = new BinaryReader(mem))
                            {
                                int count = reader.ReadInt32();
                                for (int i = 0; i < count; i++)
                                {
                                    DBMapping mapping = new DBMapping(Session.Assemblies, reader);
                                    if (mapping.Type == null) return;
                                    if (list.GetType().GenericTypeArguments[0] != mapping.Type) return;
                                    Session.PasteObject(reader, mapping, (DBObject)list.AddNew());
                                }
                                XtraMessageBox.Show("复制成功，请到最后一行查看。", "提示");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                XtraMessageBox.Show(ex.ToString());
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

        private void navBarItem8_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            ShowView(typeof(CurrencyQueryView));
        }
    }

}
