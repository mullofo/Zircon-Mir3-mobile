using Client.Controls;
using Client.Extentions;
using Client.Models;
using Client.Scenes;
using Client.Scenes.Views;
using Client.UserModels;
using FontStashSharp;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Xna.Framework.Input;
using Mir3.Mobile;
using MirDB;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using Point = System.Drawing.Point;
using Session = MirDB.Session;
using Size = System.Drawing.Size;

namespace Client.Envir
{
    /// <summary>
    /// 环境设置
    /// </summary>
    public static class CEnvir
    {
        #region MonoGame
        public static long GameDelay;

        public static long GameDelayCounter;

        public static long ImageDelay;

        public static long ImageDelayCounter;

        public static long LogicDelay;

        public static long LogicDelayCounter;

        public static long BEsDelay;

        public static long BEsDelayCounter;

        public static int BSCounter;

        private static int BSCount;

        public static float Density;

        public static float DeviceHeight;

        public static int DevicePercent = 16;

        public static float UIScale = 1f;

        public static int UI_Offset_X;

        //public static float GameBtnScale = 1f;

        //public static int ChatDialogWidth;

        public static Size GameSize;

        public static int BatteryLevel;

        public static int BatteryScale = 100;

        public static DateTime LastLoopTime;

        public static Microsoft.Xna.Framework.Graphics.Effect GrayscaleShader;

        //public static string[] DotString = new string[6];

        //public static int DotNum;

        public static FontSystem Fonts { get; set; }

        public static string MobileClientPath { get; set; }

        public static void UpdateScale(bool isGame = false)
        {
            //由于登录 选人场景默认分辨率是640*480， 所以默认采用这个分辨率设置缩放率
            UIScale = (float)Target.Height / Config.IntroSceneSize.Height;
            UI_Offset_X = (int)(Target.Width - Config.IntroSceneSize.Width * UIScale) / 2;

            //如果是游戏场景，设置游戏场景缩放率
            if (isGame)
            {
                UIScale = (float)Target.Height / Config.GameSize.Height;
                float offset = Target.Width - Config.GameSize.Width * UIScale;
                GameSize = new Size((int)Math.Round(Config.GameSize.Width + (offset / UIScale)), Config.GameSize.Height);
                UI_Offset_X = (int)Math.Round((Target.Width - GameSize.Width * UIScale) / 2);
            }
        }
        #endregion
        /// <summary>
        /// 显示设备实际分辨率 手机系统实际分辨率
        /// </summary>
        public static Size Target;
        /// <summary>
        /// 随机数
        /// </summary>
        public static Random Random = new Random();

        /// <summary>
        /// FPS帧数时间
        /// </summary>
        private static DateTime _FPSTime;
        /// <summary>
        /// FPS帧数计数器
        /// </summary>
        private static int FPSCounter;
        /// <summary>
        /// FPS帧数计数
        /// </summary>
        public static int FPSCount;
        /// <summary>
        /// DPS计数器
        /// </summary>
        public static int DPSCounter;
        /// <summary>
        /// DPS计数
        /// </summary>
        private static int DPSCount;
        /// <summary>
        /// 帧生成时间
        /// </summary>
        public static long FrameTime;
        /// <summary>
        /// FrameTimeTicks
        /// </summary>
        //private static long FrameTimeTicksBefore, FrameTimeTicksAfter;
        /// <summary>
        /// 按SHIFT键
        /// </summary>
        public static bool Shift;
        /// <summary>
        /// 按ALT键
        /// </summary>
        public static bool Alt;
        /// <summary>
        /// 按CTRL键
        /// </summary>
        public static bool Ctrl;
        /// <summary>
        /// 系统当前时间
        /// </summary>
        public static DateTime Now = Time.Now;
        /// <summary>
        /// 鼠标位置
        /// </summary>
        public static Point MouseLocation;

        /// <summary>
        /// 连接
        /// </summary>
        public static CConnection Connection;
        /// <summary>
        /// 版本效验
        /// </summary>
        public static bool WrongVersion;

        /// <summary>
        /// 库文件
        /// </summary>
        public static Dictionary<LibraryFile, MirLibrary> LibraryList = new Dictionary<LibraryFile, MirLibrary>();

        /// <summary>
        /// 仓库格子
        /// </summary>
        public static ClientUserItem[] Storage, MainStorage;
        /// <summary>
        /// 碎片格子
        /// </summary>
        public static ClientUserItem[] PatchGrid, MainPatchGrid;
        /// <summary>
        /// 宠物背包格子
        /// </summary>
        public static ClientUserItem[] CompanionGrid, MainCompanionGrid;

        /// <summary>
        /// 黑名单列表
        /// </summary>
        public static List<ClientBlockInfo> BlockList = new List<ClientBlockInfo>();

        /// <summary>
        /// 记忆传送列表
        /// </summary>
        public static List<ClientFixedPointInfo> FixedPointList = new List<ClientFixedPointInfo>();
        /// <summary>
        /// 记忆传送计数
        /// </summary>
        public static int FixedPointTCount;
        /// <summary>
        /// 快捷键绑定信息
        /// </summary>
        public static DBCollection<KeyBindInfo> KeyBinds;
        /// <summary>
        /// 系统窗体设置
        /// </summary>
        public static DBCollection<WindowSetting> WindowSettings;
        /// <summary>
        /// 城堡信息列表
        /// </summary>
        public static DBCollection<CastleInfo> CastleInfoList;
        /// <summary>
        /// 钓鱼区域列表
        /// </summary>
        public static DBCollection<FishingAreaInfo> FishingAreaInfoList;
        /// <summary>
        /// 刷怪区域列表
        /// </summary>
        public static DBCollection<RespawnInfo> RespawnInfoList;
        /// <summary>
        /// 变量储存
        /// </summary>
        public static Session Session;
        /// <summary>
        /// 并发队列 聊天日志
        /// </summary>
        public static ConcurrentQueue<string> ChatLog = new ConcurrentQueue<string>();
        /// <summary>
        /// 并发队列 系统日志
        /// </summary>
        public static ConcurrentQueue<string> SystemLog = new ConcurrentQueue<string>();
        /// <summary>
        /// 加载
        /// </summary>
        public static bool Loaded;
        /// <summary>
        /// 购买地址
        /// </summary>
        public static string BuyAddress;
        /// <summary>
        /// 求和效验
        /// </summary>
        public static string C;
        /// <summary>
        /// 测试服务器
        /// </summary>
        public static bool TestServer;
        /// <summary>
        /// 辅助控制
        /// </summary>
        public static ClientControl ClientControl { get; set; } = new ClientControl();

        /// <summary>
        /// 显示物品来源 不放在Config 防止用户自己修改配置文件
        /// </summary>
        public static bool DisplayItemSource { get; set; } = false;
        /// <summary>
        /// 显示GM刷的物品来源 不放在Config 防止用户自己修改配置文件
        /// </summary>
        public static bool DisplayGMItemSource { get; set; } = false;
        /// <summary>
        /// 显示物品套装信息 不放在Config 防止用户自己修改配置文件
        /// </summary>
        public static bool DisplayItemSuitInfo { get; set; } = false;
        /// <summary>
        /// 自定义技能动画
        /// </summary>
        public static Dictionary<MonsterInfo, Dictionary<MirAnimation, Library.Frame>> DiyMonActFrame = new Dictionary<MonsterInfo, Dictionary<MirAnimation, Library.Frame>>();
        public static Dictionary<MonsterInfo, Dictionary<MirAnimation, DXSound>> DiyMonActSound = new Dictionary<MonsterInfo, Dictionary<MirAnimation, DXSound>>();
        public static Dictionary<MonsterInfo, Dictionary<MirAnimation, MonAnimationEffect>> DiyMonActEffect = new Dictionary<MonsterInfo, Dictionary<MirAnimation, MonAnimationEffect>>();
        public static Dictionary<MonsterInfo, Dictionary<MirAnimation, DiyMagicEffect>> DiyMonActSpellEffect = new Dictionary<MonsterInfo, Dictionary<MirAnimation, DiyMagicEffect>>();

        public static void InitDiyMonFrams()
        {
            MonAnimationFrame MonAnimationFrame = null;
            Dictionary<MirAnimation, Library.Frame> DefaultMonFrame = null;
            Dictionary<MirAnimation, DXSound> DefaultMonActSound = null;
            Dictionary<MirAnimation, MonAnimationEffect> DefaultMonActEffect = null;

            for (int i = 0; i < Globals.MonAnimationFrameList.Binding.Count; i++)
            {
                MonAnimationFrame = Globals.MonAnimationFrameList.Binding[i];
                if (MonAnimationFrame.Monster == null) continue;
                //动作
                if (!DiyMonActFrame.TryGetValue(MonAnimationFrame.Monster, out DefaultMonFrame))
                {
                    DefaultMonFrame = new Dictionary<MirAnimation, Library.Frame>();
                    DefaultMonFrame[MonAnimationFrame.MonAnimation] = new Library.Frame(MonAnimationFrame.startIndex, MonAnimationFrame.frameCount, MonAnimationFrame.offSet, TimeSpan.FromMilliseconds(MonAnimationFrame.frameDelay)) { Reversed = MonAnimationFrame.Reversed, StaticSpeed = MonAnimationFrame.StaticSpeed };
                    DiyMonActFrame.Add(MonAnimationFrame.Monster, DefaultMonFrame);
                }
                DefaultMonFrame[MonAnimationFrame.MonAnimation] = new Library.Frame(MonAnimationFrame.startIndex, MonAnimationFrame.frameCount, MonAnimationFrame.offSet, TimeSpan.FromMilliseconds(MonAnimationFrame.frameDelay)) { Reversed = MonAnimationFrame.Reversed, StaticSpeed = MonAnimationFrame.StaticSpeed };

                //声音
                if (MonAnimationFrame.ActSound != SoundIndex.None || MonAnimationFrame.ActSoundStr != "")
                {
                    if (!DiyMonActSound.TryGetValue(MonAnimationFrame.Monster, out DefaultMonActSound))
                    {
                        DefaultMonActSound = new Dictionary<MirAnimation, DXSound>();
                        DiyMonActSound.Add(MonAnimationFrame.Monster, DefaultMonActSound);
                    }

                    if (MonAnimationFrame.ActSound != SoundIndex.None)
                    {
                        DefaultMonActSound[MonAnimationFrame.MonAnimation] = DXSoundManager.SoundList[MonAnimationFrame.ActSound];
                    }
                    else
                    {
                        DefaultMonActSound[MonAnimationFrame.MonAnimation] = DXSoundManager.AddDiySound(MonAnimationFrame.ActSoundStr, SoundType.Monster);
                    }
                }
                //伴随魔法
                if (MonAnimationFrame.effectfile != LibraryFile.None)
                {
                    if (!DiyMonActEffect.TryGetValue(MonAnimationFrame.Monster, out DefaultMonActEffect))
                    {
                        DefaultMonActEffect = new Dictionary<MirAnimation, MonAnimationEffect>();
                        DiyMonActEffect.Add(MonAnimationFrame.Monster, DefaultMonActEffect);
                    }

                    DefaultMonActEffect[MonAnimationFrame.MonAnimation] = new MonAnimationEffect()
                    {
                        effectfile = MonAnimationFrame.effectfile,
                        effectfrom = MonAnimationFrame.effectfrom,
                        effectframe = MonAnimationFrame.effectframe,
                        effectdelay = MonAnimationFrame.effectdelay,
                        effectdir = MonAnimationFrame.effectdir,
                    };
                }
            }
        }

        //古墓任务序号
        public static int PenetraliumKeyA { get; set; } = 172;
        public static int PenetraliumKeyB { get; set; } = 173;
        public static int PenetraliumKeyC { get; set; } = 174;

        #region 数据库加密相关
        //是否收到了DB信息 收到了才知道System.db是否加密了
        private static bool _dbInfoReceived;

        public static bool DBInfoReceived
        {
            get => _dbInfoReceived;
            set
            {
                _dbInfoReceived = value;
                //如果断开重连，会重新加载，已加载先释放。 
                Session?.Dispose();
                LoadDatabase();
            }
        }
        public static bool DBEncrypted { get; set; }
        // DB的哈希值
        public static string DBHash { get; set; }
        public static string DBPassword { get; set; }

        #endregion

        #region 沙巴克相关

        public static Dictionary<CastleInfo, Point> WarFlagsDict { get; set; } = new Dictionary<CastleInfo, Point>();
        public static bool AllowTeleportMagicNearFlag { get; set; }
        public static int TeleportMagicRadiusRange { get; set; }

        #endregion

        #region 反挂

        /// <summary>
        /// 30 秒发一次 可以调
        /// </summary>
        //public static TimeSpan SendProcessHashDelay = TimeSpan.FromSeconds(30);
        /// <summary>
        /// 上次请求的时间
        /// </summary>
        //public static DateTime NextProcessHashSendTime { get; set; } = Time.Now;
        /// <summary>
        /// 是否应该发送请求
        /// </summary>
        //public static bool ShouldSendProcessHash => Time.Now > NextProcessHashSendTime;

        #endregion

        //地图技能限制
        public static Dictionary<string, HashSet<MagicType>> MapMagicRestrictions { get; set; } = new Dictionary<string, HashSet<MagicType>>();
        /// <summary>
        /// 缓存的NPC外观更新
        /// </summary>
        public static Dictionary<uint, Library.Network.ServerPackets.UpdateNPCLook> UpdatedNPCLooks { get; set; } =
            new Dictionary<uint, Library.Network.ServerPackets.UpdateNPCLook>();

        /// <summary>
        /// 天气设置
        /// </summary>
        public static Dictionary<int, WeatherSetting> WeatherOverrides = new Dictionary<int, WeatherSetting>();

        /// <summary>
        /// 启动客户端安全接入组件(只能启动一次)
        /// </summary>
        /// <param name="key">sdk配置密钥(从自己的实例中可以获取该密钥)</param>
        /// <returns>返回150表示成功，其它的为失败</returns>
        //[DllImport("clinkAPI.dll", CallingConvention = CallingConvention.Winapi)]
        //public static extern int clinkStart(string key);

        /// <summary>
        /// 获取角色所在的道具位置方向
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Item"></param>
        /// <returns></returns>
        public static string GetDirName(Point User, Point Item)
        {
            string result;
            if (Item.X < User.X)
            {
                //左边
                if (Item.Y < User.Y)
                {
                    //上↖↑↗
                    //  ←⟳→
                    //  ↙↓↘
                    /*
					 * ⇱
					 * 
					 * ⇲
					 */
                    result = "左上↖".Lang();
                }
                else if (Item.Y == User.Y)
                {
                    result = "正左←".Lang();
                }
                else
                {
                    result = "左下↙".Lang();
                }
            }
            else if (Item.X == User.X)
            {
                //正
                if (Item.Y < User.Y)
                {
                    //上
                    result = "正上↑".Lang();
                }
                else if (Item.Y == User.Y)
                {
                    result = "脚下㊉".Lang();
                }
                else
                {
                    result = "正下↓".Lang();
                }
            }
            else
            {
                //右
                //左边
                if (Item.Y < User.Y)
                {
                    //上
                    result = "右上↗".Lang();
                }
                else if (Item.Y == User.Y)
                {
                    result = "正右→".Lang();
                }
                else
                {
                    result = "右下↘".Lang();
                }
            }
            return result;
        }
        static CEnvir()
        {
            //Thread workThread = new Thread(SaveChatLoop) { IsBackground = true };
            DiyMonActSound = new Dictionary<MonsterInfo, Dictionary<MirAnimation, DXSound>>();
            // 测试 给聊天记录线程更低的优先级
            //workThread.Priority = ThreadPriority.BelowNormal;
            //workThread.Start();

            try
            {
                A();
            }
            catch
            {
                //if (Config.SentryEnabled)
                //{
                //    SentrySdk.CaptureException(ex);
                //}
            }
        }

        /// <summary>
        /// 检查效验
        /// </summary>
        private static void A()
        {
            string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Mir3");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (File.Exists(path + @"/CheckSum.bin"))
            {
                using (BinaryReader E = new BinaryReader(File.OpenRead(path + @"/CheckSum.bin")))
                    C = E.ReadString();
            }
            else
            {
                using (BinaryWriter E = new BinaryWriter(File.Create(path + @"/CheckSum.bin")))
                    E.Write(C = Functions.RandomString(Random, 20));
            }

        }
        /// <summary>
        /// 保存聊天记录
        /// </summary>
        public static void SaveChatLoop()
        {
            List<string> linechat = new List<string>();
            List<string> linesys = new List<string>();
            while (true)
            {
                while (ChatLog.IsEmpty && SystemLog.IsEmpty)
                    Thread.Sleep(1000);

                while (!ChatLog.IsEmpty)
                {
                    string line;

                    if (!ChatLog.TryDequeue(out line)) continue;

                    linechat.Add(line);
                }
                while (!SystemLog.IsEmpty)
                {
                    string line;

                    if (!SystemLog.TryDequeue(out line)) continue;

                    linesys.Add(line);
                }
                try
                {
                    string file = DateTime.Now.ToString("yyyy-MM-dd");   //日期

                    File.AppendAllLines($".\\ChatLogs\\{file}.txt", linechat);  //聊天记录
                    linechat.Clear();

                    File.AppendAllLines($".\\SysLogs\\{file}.txt", linesys);   //系统信息记录
                    linesys.Clear();
                }
                catch (IOException)
                {
                    //文件无法写入 没啥问题不用上报
                }
                catch (Exception ex)
                {
                    //if (Config.SentryEnabled)
                    //{
                    //    SentrySdk.CaptureException(ex);
                    //}
                    SaveError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 装备幻化信息
        /// </summary>
        /// <param name="clientUserItem"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public static int GetItemIllusionItemInfo(ClientUserItem clientUserItem, int DefaultValue = 0)
        {
            int result = DefaultValue;
            if (clientUserItem != null)
            {
                if (clientUserItem.AddedStats[Stat.Illusion] > 0)
                {
                    ItemInfo Info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == clientUserItem.AddedStats[Stat.Illusion]);
                    result = Info.Image;
                }
                else
                {
                    result = clientUserItem.Info.Image;
                }
            }
            return result;
        }
        /// <summary>
        /// 装备幻化名字
        /// </summary>
        /// <param name="clientUserItem"></param>
        /// <returns></returns>
        public static string GetItemIllusionItemName(ClientUserItem clientUserItem)
        {
            string itemname = "未知";
            if (clientUserItem != null)
            {
                if (clientUserItem.AddedStats[Stat.Illusion] > 0)
                {
                    ItemInfo Info = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == clientUserItem.AddedStats[Stat.Illusion]);
                    itemname = Info.ItemName;
                }
                else
                {
                    itemname = clientUserItem.Info.ItemName;
                }
            }
            return itemname;
        }
        /// 获取客户端MAC信息
        /// </summary>
        //public static string MacInfo = GetLocalMac();
        /// <summary>
        /// 获取客户端CPU信息
        /// </summary>
        //public static string CpuInfo = GetCPUSerialNumber();
        /// <summary>
        /// 获取客户端HDD信息
        /// </summary>
        //public static string HDDnfo = GetHardDiskSerialNumber();
        /// <summary>
        /// 获取MAC序列号
        /// </summary>
        /// <returns></returns>
        //public static string GetLocalMac()
        //{
        //    try
        //    {
        //        string mac = null;
        //        ManagementObjectSearcher query = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration");
        //        ManagementObjectCollection queryCollection = query.Get();
        //        foreach (ManagementObject mo in queryCollection)
        //        {
        //            if (mo["IPEnabled"].ToString() == "True")
        //                mac = mo["MacAddress"].ToString();
        //        }
        //        return (mac);
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}
        /// <summary>
        /// 获取CPU序列号
        /// </summary>
        /// <returns></returns>
        //public static string GetCPUSerialNumber()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Processor");
        //        string sCPUSerialNumber = "";
        //        foreach (ManagementObject mo in searcher.Get())
        //        {
        //            sCPUSerialNumber = mo["ProcessorId"].ToString().Trim();
        //        }
        //        return sCPUSerialNumber;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}
        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        //public static string GetHardDiskSerialNumber()
        //{
        //    try
        //    {
        //        ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
        //        string sHardDiskSerialNumber = "";
        //        foreach (ManagementObject mo in searcher.Get())
        //        {
        //            sHardDiskSerialNumber = mo["SerialNumber"].ToString().Trim();
        //            break;
        //        }
        //        return sHardDiskSerialNumber;
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        /// <summary>
        /// 游戏循环
        /// </summary>
        //public static void GameLoop()
        //{
        //    UpdateGame();
        //    RenderGame();

        //    //if (Config.LimitFPS)
        //    //    Thread.Sleep(1);
        //}
        /// <summary>
        /// 更新游戏
        /// </summary>
        public static void UpdateGame()
        {
            Now = Time.Now;

            DXControl.ActiveScene?.OnMouseMove(new MouseEventArgs(MouseButtons.None, 0, (int)Math.Round(MouseLocation.X * UIScale) + UI_Offset_X, (int)Math.Round(MouseLocation.Y * UIScale), 0));

            if (Time.Now >= _FPSTime)
            {
                _FPSTime = Time.Now.AddSeconds(1);
                FPSCount = FPSCounter;
                FPSCounter = 0;
                DPSCount = DPSCounter;
                DPSCounter = 0;
                BSCount = BSCounter;
                BSCounter = 0;
                ImageDelay = ImageDelayCounter / 10000;
                ImageDelayCounter = 0L;
                LogicDelay = LogicDelayCounter / 10000;
                LogicDelayCounter = 0L;
                GameDelay = GameDelayCounter / 10000;
                GameDelayCounter = 0L;
                BEsDelay = BEsDelayCounter / 10000;
                BEsDelayCounter = 0L;
                //FrameTime = (FrameTimeTicksAfter - FrameTimeTicksBefore) * Time.NanoSecPerTick / 1000L;
                //System.Console.WriteLine("LogicDelay:" + LogicDelay.ToString() + " " + "GameDelay:" + GameDelay.ToString() + " " + "ImageDelay:" + ImageDelay.ToString());
                if (DXManager.ForceCleanSync)
                    DXManager.CleanALLForced();
                else
                    DXManager.MemoryClear();
            }

            Connection?.Process();

            DXControl.ActiveScene?.Process();

            //var scaleMouseLocation = UIScale == 1F ? MouseLocation : new Point((int)Math.Round((MouseLocation.X - CEnvir.UI_Offset_X) / UIScale), (int)Math.Round(MouseLocation.Y / UIScale));

            //  顶部调试
            if (Config.DebugLabel)
            {
                string debugText = $"帧数".Lang() + $": {FPSCount}";

                //debugText += $", " + "帧生成时间".Lang() + $": {FrameTime} " + "微秒".Lang();

                if (DXControl.MouseControl != null)
                    debugText += $", " + "鼠标控制".Lang() + $": Cid:{DXControl.MouseControl._Cid} .{DXControl.MouseControl.GetType().Name} location:{DXControl.MouseControl.Location.ToString()},size:{DXControl.MouseControl.Size.ToString()}";

                if (DXControl.FocusControl != null)
                    debugText += $", " + "聚焦控制".Lang() + $": {DXControl.FocusControl.GetType().Name}";

                if (GameScene.Game != null)
                {
                    if (DXControl.MouseControl is MapControl)
                        debugText += $", " + "指针坐标".Lang() + $": {GameScene.Game.MapControl.MapLocation}";

                    debugText += $", " + "对象".Lang() + $": {GameScene.Game.MapControl.Objects.Count}";

                    if (MapObject.MouseObject != null)
                        debugText += $", " + "鼠标对象".Lang() + $": {MapObject.MouseObject.Name}";
                }
                debugText += $"M:X={MouseLocation.X}Y={MouseLocation.Y},";
                debugText += $", " + "每秒绘制".Lang() + $": {DPSCount}";

                DXControl.DebugLabel.ZoomRate = UIScale;
                DXControl.DebugLabel.UI_Offset_X = CEnvir.UI_Offset_X;
                DXControl.DebugLabel.Text = debugText;
            }

            //ping 显示信息
            if (Connection != null)
            {
                const decimal KB = 1024;
                const decimal MB = KB * 1024;

                string sent, received;

                if (Connection.TotalBytesSent > MB)
                    sent = $"{Connection.TotalBytesSent / MB:#,##0.0}MB";
                else if (Connection.TotalBytesSent > KB)
                    sent = $"{Connection.TotalBytesSent / KB:#,##0}KB";
                else
                    sent = $"{Connection.TotalBytesSent:#,##0}B";

                if (Connection.TotalBytesReceived > MB)
                    received = $"{Connection.TotalBytesReceived / MB:#,##0.0}MB";
                else if (Connection.TotalBytesReceived > KB)
                    received = $"{Connection.TotalBytesReceived / KB:#,##0}KB";
                else
                    received = $"{Connection.TotalBytesReceived:#,##0}B";

                DXControl.PingLabel.ZoomRate = UIScale;
                DXControl.PingLabel.UI_Offset_X = CEnvir.UI_Offset_X;
                DXControl.PingLabel.Text = $"Cenvir.PingLabel".Lang(Connection.Ping, sent, received);
                DXControl.PingLabel.Location = new Point(DXControl.DebugLabel.DisplayArea.Right + 5, DXControl.DebugLabel.DisplayArea.Y);
            }
            else
            {
                DXControl.PingLabel.Text = String.Empty;
            }

            //鼠标焦点提示信息
            if (DXControl.MouseControl != null && DXControl.ActiveScene != null)
            {
                DXControl.HintLabel.Text = DXControl.MouseControl.Hint;

                Point location = new Point(MouseLocation.X, MouseLocation.Y + 17);

                if (location.X + DXControl.HintLabel.Size.Width > DXControl.ActiveScene.Size.Width)
                    location.X = DXControl.ActiveScene.Size.Width - DXControl.HintLabel.Size.Width - 1;

                if (location.Y + DXControl.HintLabel.Size.Height > DXControl.ActiveScene.Size.Height)
                    location.Y = DXControl.ActiveScene.Size.Height - DXControl.HintLabel.Size.Height - 1;

                if (location.X < 0) location.X = 0;
                if (location.Y < 0) location.Y = 0;

                DXControl.HintLabel.ZoomRate = UIScale;
                DXControl.HintLabel.UI_Offset_X = CEnvir.UI_Offset_X;
                DXControl.HintLabel.Location = location;
            }
            else
            {
                DXControl.HintLabel.Text = null;
            }

#if Mobile
#else
            //顶部标签 时间
            if (GameScene.Game != null && GameScene.Game.TopInfoBox != null)
            {
                GameScene.Game.TopInfoBox.Visible = Config.ChkDisplayOthers;
                GameScene.Game.TopInfoBox.TimeLabel.Text = $"Cenvir.TimeLabel".Lang(DateTime.Now.ToString("HH:mm:ss"));
            }

            #region 反外挂


            //if (Connection is { Connected: true } && ShouldSendProcessHash)
            //{
            //    NextProcessHashSendTime = Time.Now + SendProcessHashDelay;

            //    Task.Run(() =>
            //    {
            //        Enqueue(new ResponseProcessHash
            //        {
            //            HashList = AntiCheat.ComputeAllHashes(),
            //            DateTime = CEnvir.Now,
            //        });
            //    });
            //}

            #endregion
#endif



            LogicDelayCounter += (Time.Now - Now).Ticks;
        }
        /// <summary>
        /// 渲染游戏
        /// </summary>
        public static void RenderGame()
        {
            try
            {
                //if (Target.ClientSize.Width == 0 || Target.ClientSize.Height == 0)
                //{
                //    Thread.Sleep(1);
                //    return;
                //}

                //if (DXManager.DeviceLost)
                //{
                //    DXManager.AttemptReset();
                //    Thread.Sleep(1);
                //    return;
                //}
                DateTime now = Time.Now;

                //FrameTimeTicksBefore = Time.Stopwatch.ElapsedTicks;
                DXManager.Device.Clear(ClearFlags.Target, Color.Black, 1, 0);
                //DXManager.Device.BeginScene();
                DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);
                DXControl.ActiveScene?.Draw();

                Game1.Game.RenderVirtualStick();

                DXManager.Sprite.End();
                //DXManager.Device.EndScene();

                //DXManager.Device.Present();

                //FrameTimeTicksAfter = Time.Stopwatch.ElapsedTicks;
                FPSCounter++;
                GameDelayCounter += (Time.Now - now).Ticks;
            }
            catch (Exception ex)
            {
                SaveError(ex.ToString());
                //if (Config.SentryEnabled)
                //{
                //    SentrySdk.CaptureException(ex);
                //}
                DXManager.AttemptRecovery();
            }
        }

        /// <summary>
        /// 返回登录
        /// </summary>
        public static void ReturnToLogin()
        {
            if (DXControl.ActiveScene is LoginScene) return;  //如果（DX控制 活动场景是登录）返回处理

            DXSoundManager.StopAllSounds();
            DXControl.ActiveScene.Dispose();

            UpdateScale(isGame: true);
            DXControl.ActiveScene = new LoginScene(CEnvir.GameSize); //控制活动场景 = 新的登录场景大小 （登录界面）

            BlockList = new List<ClientBlockInfo>();
            FixedPointList = new List<ClientFixedPointInfo>();
        }

        /// <summary>
        /// 加载数据库
        /// </summary>
        public static Task LoadDatabase()
        {
            var assemblies = new Assembly[] { Assembly.GetAssembly(typeof(ItemInfo)), Assembly.GetAssembly(typeof(WindowSetting)) };

            Session = new Session(SessionMode.Client, assemblies, DBEncrypted, DBPassword, CEnvir.MobileClientPath + @"Data/") { BackUp = false };
            Session.Init();
            //国际化
            Globals.LangInfoList = Session.GetCollection<LangInfo>();
            LangEx.Init();
            return Task.Run(() =>
            {
                Globals.ItemInfoList = Session.GetCollection<ItemInfo>();                               //道具
                Globals.MagicInfoList = Session.GetCollection<MagicInfo>();                             //技能
                Globals.MapInfoList = Session.GetCollection<MapInfo>();                                 //地图
                //Globals.NPCPageList = Session.GetCollection<NPCPage>();                                 //NPC页面
                Globals.MonsterInfoList = Session.GetCollection<MonsterInfo>();                         //怪物
                Globals.StoreInfoList = Session.GetCollection<StoreInfo>();                             //商城
                Globals.StoreDic = GetStoreDic();
                Globals.NPCInfoList = Session.GetCollection<NPCInfo>();                                 //NPC列表
                Globals.MovementInfoList = Session.GetCollection<MovementInfo>();                       //刷怪
                Globals.QuestInfoList = Session.GetCollection<QuestInfo>();                             //任务
                Globals.QuestTaskList = Session.GetCollection<QuestTask>();                             //任务完成
                Globals.CompanionInfoList = Session.GetCollection<CompanionInfo>();                     //宠物
                Globals.CompanionLevelInfoList = Session.GetCollection<CompanionLevelInfo>();           //宠物等级               
                Globals.ItemDisplayEffectList = Session.GetCollection<ItemDisplayEffect>();             //道具特效
                Globals.CraftItemInfoList = Session.GetCollection<CraftItemInfo>();                     //制作道具列
                Globals.GamePlayEXPInfoList = Session.GetCollection<PlayerExpInfo>();                   //等级经验
                Globals.GameWeaponEXPInfoList = Session.GetCollection<WeaponExpInfo>();                 //武器等级经验
                Globals.GameAccessoryEXPInfoList = Session.GetCollection<AccessoryExpInfo>();           //首饰等级经验
                Globals.GameCraftExpInfoList = Session.GetCollection<CraftExpInfo>();                   //制作等级经验
                Globals.AchievementInfoList = Session.GetCollection<AchievementInfo>();                 //成就信息表
                Globals.AchievementRequirementList = Session.GetCollection<AchievementRequirement>();   //成就要求表
                Globals.CustomBuffInfoList = Session.GetCollection<CustomBuffInfo>();                   //自定义buff列表
                Globals.WeaponUpgradeInfoList = Session.GetCollection<WeaponUpgradeNew>();              //新版武器升级
                Globals.MonAnimationFrameList = Session.GetCollection<MonAnimationFrame>();             //自定义怪物
                Globals.DiyMagicEffectList = Session.GetCollection<DiyMagicEffect>();                   //自定义技能动画
                Globals.GuildLevelExpList = Session.GetCollection<GuildLevelExp>();                     //行会等级升级

                Globals.SetInfoList ??= Session.GetCollection<SetInfo>();
                Globals.SetGroupList ??= Session.GetCollection<SetGroup>();
                Globals.SetGroupItemList ??= Session.GetCollection<SetGroupItem>();
                Globals.SetInfoStatList ??= Session.GetCollection<SetInfoStat>();

                KeyBinds = Session.GetCollection<KeyBindInfo>();
                WindowSettings = Session.GetCollection<WindowSetting>();
                CastleInfoList = Session.GetCollection<CastleInfo>();
                FishingAreaInfoList = Session.GetCollection<FishingAreaInfo>(); //钓鱼区域列表
                RespawnInfoList = Session.GetCollection<RespawnInfo>();

                Globals.GoldInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Effect == ItemEffect.Gold);

                foreach (CraftExpInfo expInfo in Globals.GameCraftExpInfoList.Binding)
                {
                    Globals.CraftExpDict[expInfo.Level] = expInfo.Exp;
                }

                CheckKeyBinds();

                //加载地图自定义背景音乐
                foreach (MapInfo map in Globals.MapInfoList.Binding)
                {
                    if (map.Music != SoundIndex.None) continue;
                    DXSoundManager.AddDiySound(map.MapSound, SoundType.Music);
                }

                InitDiyMonFrams();
                Loaded = true;
            });
        }

        private static Dictionary<string, string[]> GetStoreDic()
        {
            var dic = new Dictionary<string, string[]>();
            var arr = Globals.StoreInfoList.Select(p => p.Filter.Split(',')[0]).Distinct().ToArray();
            foreach (var item in arr)
            {
                var sub = Globals.StoreInfoList.Where(p => p.Filter.StartsWith($"{item},")).Select(p => p.Filter.Substring(item.Length + 1)).ToArray();
                var subStr = string.Join(",", sub);
                dic.Add(item, subStr.Split(',').Distinct().ToArray());
            }
            return dic;
        }

        /// <summary>
        /// 获取键值操作
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IEnumerable<KeyBindAction> GetKeyAction(Keys key)
        {
            if (!Loaded) yield break;

            switch (key)
            {
                case Keys.NumPad0:
                    key = Keys.D0;
                    break;
                case Keys.NumPad1:
                    key = Keys.D1;
                    break;
                case Keys.NumPad2:
                    key = Keys.D2;
                    break;
                case Keys.NumPad3:
                    key = Keys.D3;
                    break;
                case Keys.NumPad4:
                    key = Keys.D4;
                    break;
                case Keys.NumPad5:
                    key = Keys.D5;
                    break;
                case Keys.NumPad6:
                    key = Keys.D6;
                    break;
                case Keys.NumPad7:
                    key = Keys.D7;
                    break;
                case Keys.NumPad8:
                    key = Keys.D8;
                    break;
                case Keys.NumPad9:
                    key = Keys.D9;
                    break;
            }

            foreach (KeyBindInfo bind in KeyBinds.Binding)
            {
                if ((bind.Control1 == Ctrl && bind.Alt1 == Alt && bind.Shift1 == Shift && bind.Key1 == key) ||
                    (bind.Control2 == Ctrl && bind.Alt2 == Alt && bind.Shift2 == Shift && bind.Key2 == key))
                    yield return bind.Action;
            }
        }

        /// <summary>
        /// 填充仓库
        /// </summary>
        /// <param name="items"></param>
        /// <param name="observer"></param>
        public static void FillStorage(List<ClientUserItem> items, bool observer)
        {
            Storage = new ClientUserItem[1000];

            if (!observer)
                MainStorage = Storage;

            foreach (ClientUserItem item in items)
                Storage[item.Slot] = item;
        }

        /// <summary>
        /// 填充宠物包裹
        /// </summary>
        /// <param name="items"></param>
        /// <param name="observer"></param>
        public static void FillCompanionGrid(List<ClientUserItem> items, bool observer)
        {
            CompanionGrid = new ClientUserItem[1000];

            if (!observer)
                MainCompanionGrid = CompanionGrid;

            foreach (ClientUserItem item in items)
                CompanionGrid[item.Slot] = item;
        }

        /// <summary>
        /// 填充碎片包裹
        /// </summary>
        /// <param name="items"></param>
        /// <param name="observer"></param>
        public static void FillPatchGrid(List<ClientUserItem> items, bool observer)
        {
            PatchGrid = new ClientUserItem[1002];

            if (!observer)
                MainPatchGrid = PatchGrid;

            ///foreach (ClientUserItem item in items)
                //PatchGrid[item.Slot] = item;
        }

        /// <summary>
        /// 队列发送封包
        /// </summary>
        /// <param name="packet"></param>
        public static void Enqueue(Packet packet)
        {
            Connection?.Enqueue(packet);
        }

        /// <summary>
        /// 重置按键绑定
        /// </summary>
        public static void ResetKeyBinds()
        {
            for (int i = KeyBinds.Count - 1; i >= 0; i--)
                KeyBinds[i].Delete();

            CheckKeyBinds();
        }
        /// <summary>
        /// 检查按键绑定
        /// </summary>
        public static void CheckKeyBinds()
        {
            foreach (KeyBindAction action in Enum.GetValues(typeof(KeyBindAction)).Cast<KeyBindAction>())
            {
                switch (action)
                {
                    case KeyBindAction.None:
                        break;
                    default:
                        if (KeyBinds.Binding.Any(x => x.Action == action)) continue;

                        ResetKeyBind(action);
                        break;
                }
            }
        }
        /// <summary>
        /// 快捷键绑定
        /// </summary>
        /// <param name="action"></param>
        public static void ResetKeyBind(KeyBindAction action)
        {
            KeyBindInfo bind = KeyBinds.CreateNewObject();
            bind.Action = action;

            switch (action)
            {
                case KeyBindAction.ConfigWindow:      //游戏设置N
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.N;
                    bind.Key2 = Keys.N;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.CharacterWindow:   //人物界面Q
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.Q;
                    bind.Key2 = Keys.Q;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.InventoryWindow:   //人物背包W
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.W;
                    bind.Key2 = Keys.W;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.MagicWindow:       //技能界面E
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.E;
                    bind.Key2 = Keys.E;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.MagicBarWindow:   //魔法技能快捷栏B
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.B;
                    bind.Key2 = Keys.B;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.RankingWindow:    //排行榜Ctrl+R
                    if (!ClientControl.RankingShowCheck) return;  //排行榜设置不显示就不设置快捷
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.X;
                    break;
                case KeyBindAction.GameStoreWindow:  //商城Y
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.Y;
                    break;
                //case KeyBindAction.CompanionWindow:  //宠物界面I
                //    if (Globals.CompanionInfoList.Count == 0) return;
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.I;
                //    break;
                case KeyBindAction.GroupWindow:      //组队G
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.G;
                    bind.Key2 = Keys.G;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.BigPatchBoxWindow:   //大补贴辅助快捷按钮HOME键
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.Home;
                    break;
                //case KeyBindAction.StorageWindow:     //远程仓库Ctrl+S
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.S;
                //    bind.Control1 = true;
                //    break;
                case KeyBindAction.GuildWindow:       //行会快捷F
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.F;
                    bind.Key2 = Keys.F;
                    bind.Control2 = true;
                    break;
                //case KeyBindAction.QuestLogWindow:    //任务模式L
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.L;
                //    bind.Key2 = Keys.L;
                //    bind.Control2 = true;
                //    break;
                //case KeyBindAction.QuestTrackerWindow:  //任务栏显示L
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.L;
                //    break;
                case KeyBindAction.BeltWindow:   //物品快捷栏Z
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.Z;
                    bind.Key2 = Keys.Z;
                    bind.Control2 = true;
                    break;
                case KeyBindAction.MarketPlaceWindow:  //寄售Ctrl+Y
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.Y;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.MapMiniWindow:   //小地图隐藏V
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.V;
                    break;
                case KeyBindAction.MapMiniWindowSize:   //小地图缩放T
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.T;
                    break;
                case KeyBindAction.MapBigWindow:   //大地图S
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.S;
                    break;
                case KeyBindAction.CommunicationBoxWindow:   //交流,
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.L;
                    break;
                case KeyBindAction.ExitGameWindow:  //退出游戏 Alt+Q  Alt+X
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.Q;
                    bind.Alt1 = true;
                    bind.Key2 = Keys.X;
                    bind.Alt2 = true;
                    break;
                case KeyBindAction.ChangeAttackMode:  //攻击模式切换Ctrl+H
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.H;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.ChangePetMode: //宠物模式切换Ctrl+A
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.A;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.GroupAllowSwitch://删除组队Alt+G
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.G;
                    bind.Alt1 = true;
                    break;
                case KeyBindAction.GroupTarget:  //将目标添加到组队Alt+V
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.V;
                    bind.Alt1 = true;
                    break;
                case KeyBindAction.TradeRequest:   //交易C
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.C;
                    break;
                case KeyBindAction.TradeAllowSwitch:  //交易开关Ctrl+C
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.C;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.MountToggle:   //上下马M
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.M;
                    break;
                case KeyBindAction.AutoRunToggle:   //跑不停A
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.A;
                    break;
                case KeyBindAction.ChangeChatMode:  //切换聊天模式K
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.K;
                    break;
                case KeyBindAction.ItemPickUp:  //快捷捡取TAB
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.Tab;
                    break;
                case KeyBindAction.PartnerTeleport:   //夫妻传送Shift+Z
                    bind.Category = "快捷".Lang();
                    bind.Control1 = true;
                    bind.Alt1 = true;
                    //bind.Key1 = Keys.Menu;
                    break;
                case KeyBindAction.ToggleItemLock:  //锁定道具Scroll
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.Scroll;
                    break;
                //物品快捷栏
                case KeyBindAction.UseBelt01:
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.D1;
                    bind.Key2 = Keys.D1;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.UseBelt02:
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.D2;
                    bind.Key2 = Keys.D2;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.UseBelt03:
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.D3;
                    bind.Key2 = Keys.D3;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.UseBelt04:
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.D4;
                    bind.Key2 = Keys.D4;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.UseBelt05:
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.D5;
                    bind.Key2 = Keys.D5;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.UseBelt06:
                    bind.Category = "道具".Lang();
                    bind.Key1 = Keys.D6;
                    bind.Key2 = Keys.D6;
                    bind.Shift2 = true;
                    break;
                //魔法技能快捷栏
                case KeyBindAction.SpellSet01:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F1;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.SpellSet02:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F2;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.SpellSet03:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F3;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.SpellSet04:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F4;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.SpellUse01:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F1;
                    break;
                case KeyBindAction.SpellUse02:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F2;
                    break;
                case KeyBindAction.SpellUse03:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F3;
                    break;
                case KeyBindAction.SpellUse04:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F4;
                    break;
                case KeyBindAction.SpellUse05:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F5;
                    break;
                case KeyBindAction.SpellUse06:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F6;
                    break;
                case KeyBindAction.SpellUse07:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F7;
                    break;
                case KeyBindAction.SpellUse08:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F8;
                    break;
                case KeyBindAction.SpellUse09:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F9;
                    break;
                case KeyBindAction.SpellUse10:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F10;
                    break;
                case KeyBindAction.SpellUse11:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F11;
                    break;
                case KeyBindAction.SpellUse12:
                    bind.Category = "技能".Lang();
                    bind.Key1 = Keys.F12;
                    break;
                case KeyBindAction.SpellUse13:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F1;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse14:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F2;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse15:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F3;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse16:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F4;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse17:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F5;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse18:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F6;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse19:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F7;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse20:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F8;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse21:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F9;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse22:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F10;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse23:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F11;
                    bind.Shift2 = true;
                    break;
                case KeyBindAction.SpellUse24:
                    bind.Category = "技能".Lang();
                    bind.Key2 = Keys.F12;
                    bind.Shift2 = true;
                    break;
                //case KeyBindAction.FortuneWindow:  //爆率查询Ctrl+W
                //    if (!ClientControl.RateQueryShowCheck) return;  //爆率查询设置不显示就不设置快捷
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.W;
                //    bind.Control1 = true;
                //    break;
                case KeyBindAction.NPCDKeyWindow:  //D键功能
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.D;
                    break;
                //case KeyBindAction.CraftWindow:  //制作界面Ctrl+M
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.M;
                //    bind.Control1 = true;
                //    break;
                //case KeyBindAction.FixedPointWindow:  //记忆传送功能
                //    if (!ClientControl.UseFixedPoint) return;
                //    bind.Category = "功能".Lang();
                //    bind.Key1 = Keys.U;
                //    break;
                case KeyBindAction.BuffWindow:    //BUFF框
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.O;
                    bind.Control1 = true;
                    break;
                case KeyBindAction.TownReviveWindow:   //死亡复活Alt+F
                    bind.Category = "快捷".Lang();
                    bind.Key1 = Keys.F;
                    bind.Alt1 = true;
                    break;
                case KeyBindAction.ChatWindow:  //聊天框缩放R
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.R;
                    break;
                case KeyBindAction.AuctionsWindow:
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.P;
                    break;
                case KeyBindAction.BonusPoolWindow:
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.J;
                    break;
                case KeyBindAction.WarWeaponWindow:
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.U;
                    break;
                case KeyBindAction.GroupFrameWindow:
                    bind.Category = "功能".Lang();
                    bind.Key1 = Keys.I;
                    break;
            }
        }
        public static void ResetChatColourBinds()
        {
            Config.LocalTextColour = Color.White;                          //本地信息 白色
            Config.GMWhisperInTextColour = Color.Red;                      //GM信息 红色
            Config.WhisperInTextColour = Color.FromArgb(230, 120, 5);      //收到私聊 金黄色
            Config.WhisperOutTextColour = Color.Yellow;                    //发出私聊 黄色
            Config.GroupTextColour = Color.Yellow;                         //组队聊天 黄色
            Config.GuildTextColour = Color.Lime;                           //公会信息 绿色
            Config.ShoutTextColour = Color.Black;                          //喊话信息 黑色
            Config.GlobalTextColour = Color.Black;                          //全服信息 黑色
            Config.ObserverTextColour = Color.Silver;                      //观察者信息 浅白
            Config.HintTextColour = Color.AntiqueWhite;                    //提示信息 粉色
            Config.SystemTextColour = Color.Red;                           //系统信息
            Config.GainsTextColour = Color.GreenYellow;                    //收益信息 浅绿
            Config.AnnouncementTextColour = Color.White;                   //公告信息 白色
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static float FontSize(float size)
        {
            return size;
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        /// <param name="font"></param>
        /// <returns></returns>
        public static Size GetFontSize(Font font)
        {
            return TextRenderer.MeasureText(DXManager.Graphics, "字", font);
        }
        /// <summary>
        /// 错误计数
        /// </summary>
        public static int ErrorCount;
        /// <summary>
        /// 最后一个错误
        /// </summary>
        private static string LastError;
        /// <summary>
        /// 保存错误记录
        /// </summary>
        /// <param name="ex"></param>
        public static void SaveError(string ex)
        {
            try
            {

                if (++ErrorCount > 20 || String.Compare(ex, LastError, StringComparison.OrdinalIgnoreCase) == 0) return;

                string LogPath = Path.Combine(MobileClientPath, "Errors");

                LastError = string.Format("[{0:F}]: {1}", Time.Now, ex);
                //string state = $"State = {Target?.DisplayRectangle}";
                //System.Diagnostics.Debug.WriteLine(LastError);

                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                File.AppendAllText(Path.Combine(LogPath, $"{Now.Year}-{Now.Month}-{Now.Day}.txt"), LastError + System.Environment.NewLine);
                //if (Config.SentryEnabled)
                //{
                //    SentrySdk.CaptureMessage($"Cenvir.SaveError".Lang(NetworkConfig.ClientName, ex), SentryLevel.Error);
                //}
            }
            catch
            { }
        }
        /// <summary>
        /// 卸载
        /// </summary>
        public static void Unload()
        {
            CConnection con = Connection;

            Connection = null;

            con?.Disconnect();
        }
        /// <summary>
        /// 获取键绑定
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static KeyBindInfo GetKeyBind(KeyBindAction action)
        {
            return KeyBinds.Binding.FirstOrDefault(x => x.Action == action);
        }
        /// <summary>
        /// 获取按键文本
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetText(Keys key)
        {
            switch (key)
            {
                case Keys.None:
                    return string.Empty;
                case Keys.Back:
                    return "Backspace";
                case Keys.CapsLock:
                    return "Cap Lock";
                case Keys.Scroll:
                    return "Scroll Lock";
                case Keys.NumLock:
                    return "Num Lock";
                case Keys.PageUp:
                    return "Page Up";
                case Keys.PageDown:
                    return "Page Down";
                case Keys.Multiply:
                    return "Num Pad *";
                case Keys.Add:
                    return "Num Pad +";
                case Keys.Subtract:
                    return "Num Pad -";
                case Keys.Decimal:
                    return "Num Pad .";
                case Keys.Divide:
                    return "Num Pad /";
                case Keys.OemSemicolon:
                    return ";";
                case Keys.OemPlus:
                    return "=";
                case Keys.OemComma:
                    return ",";
                case Keys.OemMinus:
                    return "-";
                case Keys.OemPeriod:
                    return ".";
                case Keys.OemQuestion:
                    return "/";
                case Keys.OemTilde:
                    return "'";
                case Keys.OemOpenBrackets:
                    return "[";
                case Keys.OemCloseBrackets:
                    return "]";
                case Keys.OemQuotes:
                    return "#";
                case Keys.Oem8:
                    return "`";
                case Keys.OemBackslash:
                    return "\\";
                case Keys.D1:
                    return "1";
                case Keys.D2:
                    return "2";
                case Keys.D3:
                    return "3";
                case Keys.D4:
                    return "4";
                case Keys.D5:
                    return "5";
                case Keys.D6:
                    return "6";
                case Keys.D7:
                    return "7";
                case Keys.D8:
                    return "8";
                case Keys.D9:
                    return "9";
                case Keys.D0:
                    return "0";
                default:
                    return key.ToString();
            }
        }
    }
}
