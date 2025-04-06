using IronPython.Hosting;
using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using MirDB;
using Newtonsoft.Json;
using NLog;
using RulesEngine.Models;
using Sentry;
using Server.DBModels;
using Server.Models;
using Server.Models.EventManager;
using Server.Models.EventManager.Events;
using Server.Scripts.Npc;
using Server.Utils.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using C = Library.Network.ClientPackets;
using G = Library.Network.GeneralPackets;
using S = Library.Network.ServerPackets;
using Session = MirDB.Session;

#if WEB
#endif

namespace Server.Envir
{
    /// <summary>
    /// 服务端环境
    /// </summary>
    public static class SEnvir
    {
        /// <summary>
        /// 此处代码最多执行一次
        /// 可以放一些初始化相关的代码
        /// </summary>
        static SEnvir()
        {
            // 初始化CSV货币流水记录功能
            InitCsvFileLogger("货币流水记录");
        }

        public static bool OriginalLoop = true;
        private static AccurateTimer mainTickTimer = null;
        private static int count = 0;
        private static int loopCount = 0;
        private static int lastindex = 0;
        private static long previousTotalSent = 0L;
        private static long previousTotalReceived = 0L;
        private static long conDelay = 0L;
        private static long mainDelay = 0L;
        private static long processDelay = 0L;
        private static DateTime nextCount;
        private static DateTime UserCountTime;
        private static DateTime saveTime;
        private static DateTime DBTime;
        private static DateTime NoticeTime;
        private static int noticeline;
        public static bool Starting { get; set; }
        public static bool Stopping { get; set; }

        public static DateTime HackTime;

        public static int HackCount;
#if WEB
        public static Action<string> LogEvent { get; set; }
#endif

        #region 客户端控制值
        public static ClientControl ClientControl => new ClientControl
        {
            OnRemoteStorage = Config.Storage,//远程仓库
            OnAutoHookTab = Config.AutoHookTab,   //自动挂机页
            OnBrightBox = Config.BrightBox,       //免蜡
            OnRunCheck = Config.RunCheck,         //免助跑
            OnRockCheck = Config.RockCheck,       //稳如泰山
            OnClearBodyCheck = Config.ClearBodyCheck,   //清理尸体
            BigPatchAutoPotionDelayEdit = Config.BigPatchAutoPotionDelay, //喝药延迟毫秒
            NewWeaponUpgradeCheck = Config.NewWeaponUpgrade,  //新版武器升级开关
            AutoPotionForCompanion = Config.AutoPotionForCompanion,   //宠物喝药开关
            RankingShowCheck = Config.RankingShow,     //排行版开关
            ObserverSwitchCheck = Config.AllowObservation,   //观察者开关
            RechargeInterfaceCheck = Config.RechargeInterface, //充值开关
            GuildCreationCostEdit = Config.GuildCreationCost,
            GuildMemberCostEdit = Config.GuildMemberCost,
            GuildStorageCostEdit = Config.GuildStorageCost,
            GuildMemberHardLimitEdit = Config.GuildMemberHardLimit,
            GuildWarCostEdit = Config.GuildWarCost,
            InSafeZoneItemExpireCheck = Config.InSafeZoneItemExpire,
            ItemCanRepairCheck = Config.ItemCanRepair,
            JewelryLvShowsCheck = Config.JewelryLvShows,
            JewelryExpShowsCheck = Config.JewelryExpShows,
            ACGoldRateCostEdit = Config.ACGoldRate,
            GlobalsAttackDelay = Config.GlobalsAttackDelay,
            GlobalsASpeedRate = Config.GlobalsASpeedRate,
            GlobalsProjectileSpeed = Config.GlobalsProjectileSpeed,
            GlobalsTurnTime = Config.GlobalsTurnTime,
            GlobalsHarvestTime = Config.GlobalsHarvestTime,
            GlobalsMoveTime = Config.GlobalsMoveTime,
            GlobalsAttackTime = Config.GlobalsAttackTime,
            GlobalsCastTime = Config.GlobalsCastTime,
            GlobalsMagicDelay = Config.GlobalsMagicDelay,
            MaxViewRange = Config.MaxViewRange,
            BufferMapEffectShow = Config.BufferMapEffectShow,
            CoinPlaceChoiceCheck = Config.CoinPlaceChoice,
            RateQueryShowCheck = Config.RateQueryShow,
            CommonItemSuccess = Config.CommonItemSuccessRate,
            CommonItemReduce = Config.CommonItemReduceRate,
            SuperiorItemSuccess = Config.SuperiorItemSuccessRate,
            SuperiorItemReduce = Config.SuperiorItemReduceRate,
            EliteItemSuccess = Config.EliteItemSuccessRate,
            EliteItemReduce = Config.EliteItemReduceRate,
            ShopNonRefinable = Config.ShopNonRefinable,
            UseFixedPoint = Config.UseFixedPoint,
            ActivationCeiling = Config.ActivationCeiling,
            PersonalGoldRatio = Config.PersonalGoldRatio,
            PersonalExpRatio = Config.PersonalExpRatio,
        };

        #endregion

        #region txt脚本
        public static int ScriptIndex { get; set; }
        public static Dictionary<int, NPCScript> NPCScripts { get; set; } = new Dictionary<int, NPCScript>();

        #endregion

        #region 同步
        /// <summary>
        /// 同步上下文
        /// </summary>
        private static readonly SynchronizationContext Context = SynchronizationContext.Current;
        /// <summary>
        /// 发送(发送或者回调方法)
        /// </summary>
        /// <param name="method"></param>
        public static void Send(SendOrPostCallback method)
        {
            Context.Send(method, null);
        }
        /// <summary>
        /// 回调(发送或者回调方法)
        /// </summary>
        /// <param name="method"></param>
        public static void Post(SendOrPostCallback method)
        {
            Context.Post(method, null);
        }

        #endregion

        #region 登录
        /// <summary>
        /// 并发队列 显示日志
        /// </summary>
        public static ConcurrentQueue<string> DisplayLogs = new ConcurrentQueue<string>();
        /// <summary>
        /// 并发队列 记录日志
        /// </summary>
        public static ConcurrentQueue<string> Logs = new ConcurrentQueue<string>();
        /// <summary>
        /// 货币日志
        /// </summary>
        public static ConcurrentQueue<CurrencyLogEntry> CurrencyLogs = new ConcurrentQueue<CurrencyLogEntry>();
        /// <summary>
        /// 是否使用日志控制台
        /// </summary>
        public static bool UseLogConsole = false;
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="log"></param>
        /// <param name="hardLog">是否追加至日志文件</param>
        public static void Log(string log, bool hardLog = true, bool onlyFile = false)
        {
            log = string.Format("[{0:F}]: {1}", Time.Now, log);

            if (UseLogConsole)  //使用日志控制台
            {
#if WEB
                LogEvent?.Invoke(log);
#endif
                Console.WriteLine(log);
            }
            else
            {
                if (DisplayLogs.Count < 100 && !onlyFile)
                    DisplayLogs.Enqueue(log);

                if (hardLog && Logs.Count < 1000)
                    Logs.Enqueue(log);
            }
        }
        /// <summary>
        /// 并发队列 显示聊天日志
        /// </summary>
        public static ConcurrentQueue<string> DisplayChatLogs = new ConcurrentQueue<string>();
        /// <summary>
        /// 并发队列 记录聊天日志
        /// </summary>
        public static ConcurrentQueue<string> ChatLogs = new ConcurrentQueue<string>();
        /// <summary>
        /// 记录聊天日志
        /// </summary>
        /// <param name="log"></param>
        public static void LogChat(string log)
        {
            log = string.Format("[{0:F}]: {1}", Time.Now, log);

            if (DisplayChatLogs.Count < 500)
                DisplayChatLogs.Enqueue(log);

            if (ChatLogs.Count < 1000)
                ChatLogs.Enqueue(log);
        }
        #endregion

        #region Python
        const string _scriptPath = "Scripts";
        public static Dictionary<string, ExtendScript> CSScripts { get; set; } = new Dictionary<string, ExtendScript>();
        /// <summary>
        /// 脚本引擎
        /// </summary>
        public static ScriptEngine engine;
        /// <summary>
        /// 脚本作用 域
        /// </summary>
        public static ScriptScope scope;
        /// <summary>
        /// PY事件
        /// </summary>
        public static Dictionary<string, dynamic> PythonEvent = new Dictionary<string, dynamic>();
        /// <summary>
        /// 加载脚本
        /// </summary>
        public static void LoadScripts()
        {
            //使用TXT脚本
            if (Config.UseTxtScript)
            {
                if (NPCScripts.Count > 0 || _loadTxtScript) return;
                LoadTxtSciprts();
                return;
            }

            try
            {
                DateTime Now = Time.Now;
                Log($"开始加载PY脚本");

                engine = Python.CreateEngine();

                var path = engine.GetSearchPaths();

                List<string> lst = new List<string>(path.Count);
                lst.AddRange(path);
                lst.Add(AppContext.BaseDirectory);
                lst.Add(Path.Combine(AppContext.BaseDirectory, _scriptPath));
                engine.SetSearchPaths(lst);
                engine.Runtime.LoadAssembly(Assembly.GetAssembly(typeof(SEnvir)));
                scope = engine.CreateScope();
                engine.CreateScriptSourceFromFile(Path.Combine(_scriptPath, "main.py")).Compile().Execute(scope);
                dynamic ob;
                dynamic fun;
                ob = scope.GetVariable("NpcEvent");
                fun = scope.Engine.Operations.GetMember(ob, "trig_npc");
                PythonEvent["NPCEvent_trig_npc"] = fun;
                ob = scope.GetVariable("PlayerEvent");
                fun = scope.Engine.Operations.GetMember(ob, "trig_player");
                PythonEvent["PlayerEvent_trig_player"] = fun;
                ob = scope.GetVariable("MapEvent");
                fun = scope.Engine.Operations.GetMember(ob, "trig_map");
                PythonEvent["MapEvent_trig_map"] = fun;
                ob = scope.GetVariable("MonsterEvent");
                fun = scope.Engine.Operations.GetMember(ob, "trig_mon");
                PythonEvent["MonsterEvent_trig_mon"] = fun;

                ob = scope.GetVariable("ServerEvent");
                fun = scope.Engine.Operations.GetMember(ob, "trig_server");
                PythonEvent["ServerEvent_trig_server"] = fun;
                TimeSpan time = Time.Now - Now;
                Log($"加载PY脚本用时:" + time.TotalMilliseconds + "毫秒");
            }
            catch (SyntaxErrorException e)
            {
                string msg = "加载脚本语法错误 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "加载脚本系统退出 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "加载插件时加载脚本错误，请确认Python已安装 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }
        }

        static bool _loadTxtScript = false;
        public static void LoadTxtSciprts()
        {
            _loadTxtScript = true;
            NPCScript.SetExtend();
            SEnvir.NPCScripts = new Dictionary<int, NPCScript>();
            Log($"开始加载TXT脚本");
            DateTime Now = Time.Now;

            foreach (var item in NPCInfoList.Binding)
            {
                if (string.IsNullOrEmpty(item.NPCFile))
                {
                    item.NPCFile = NPCScript.DefalutScript;
                    NPCScript.GetOrAdd(item.NPCFile, NPCScriptType.Called);
                    continue;
                }
                NPCScript.GetOrAdd(item.NPCFile, NPCScriptType.Called);
            }
            TimeSpan time = Time.Now - Now;
            Log($"加载TXT脚本用时:" + time.TotalMilliseconds + "毫秒");
            _loadTxtScript = false;
        }

        public static List<ActionScript> ScriptList = new List<ActionScript>();

        // py脚本执行时间统计
        public static ConcurrentDictionary<string, PyMetric> PyMetricsDict =
            new ConcurrentDictionary<string, PyMetric>();

        public class PyMetric
        {
            public string FunctionName { get; set; }
            public long ExecutionCount { get; set; }
            public double TotalExecutionTime { get; set; }
            public double MaxExecutionTime { get; set; }
            public string MaxExecutionTimeMapObjectName { get; set; }
        }

        // py脚本执行时间统计
        public static dynamic ExecutePyWithTimer(dynamic fun, string functionName, object args)
        {
            if (!Config.EnablePyMetrics)
            {
                return fun(functionName, args);
            }
            var timer = Stopwatch.StartNew();
            var result = fun(functionName, args);
            double elapsed = timer.Elapsed.TotalMilliseconds;
            timer.Stop();

            PyMetricsDict.AddOrUpdate(functionName, new PyMetric
            {
                FunctionName = functionName,
                ExecutionCount = 1,
                TotalExecutionTime = elapsed,
                MaxExecutionTime = elapsed,
                MaxExecutionTimeMapObjectName = "***系统调用***"
            },
                (key, value) =>
                {
                    value.ExecutionCount++;
                    value.TotalExecutionTime += elapsed;
                    value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                    return value;
                });

            return result;
        }

        // py脚本执行时间统计
        public static dynamic ExecutePyWithTimer(dynamic fun, object optionalParam, string functionName, object args)
        {
            if (!Config.EnablePyMetrics)
            {
                return fun(optionalParam, functionName, args);
            }

            var timer = Stopwatch.StartNew();
            var result = fun(optionalParam, functionName, args);
            double elapsed = timer.Elapsed.TotalMilliseconds;
            timer.Stop();

            PyMetricsDict.AddOrUpdate(functionName, new PyMetric
            {
                FunctionName = functionName,
                ExecutionCount = 1,
                TotalExecutionTime = elapsed,
                MaxExecutionTime = elapsed,
                MaxExecutionTimeMapObjectName = "无"
            },
                (key, value) =>
                {
                    value.ExecutionCount++;
                    value.TotalExecutionTime += elapsed;
                    if (elapsed > value.MaxExecutionTime)
                    {
                        value.MaxExecutionTime = elapsed;
                        if (optionalParam is PlayerObject player)
                        {
                            value.MaxExecutionTimeMapObjectName = player.Name;
                        }
                        else if (optionalParam is MonsterObject monster)
                        {
                            value.MaxExecutionTimeMapObjectName = monster.Name;
                        }
                        else if (optionalParam is NPCObject npc)
                        {
                            value.MaxExecutionTimeMapObjectName = npc.Name;
                        }
                    }
                    return value;
                });

            return result;
        }


        #endregion

        #region 网络
        /// <summary>
        /// IP锁定
        /// </summary>
        public static Dictionary<string, DateTime> IPBlocks = new Dictionary<string, DateTime>();
        /// <summary>
        /// IP计数
        /// </summary>
        public static Dictionary<string, int> IPCount = new Dictionary<string, int>();
        /// <summary>
        /// 连接
        /// </summary>
        public static List<SConnection> Connections = new List<SConnection>();
        /// <summary>
        /// 新的连接
        /// </summary>
        public static ConcurrentQueue<SConnection> NewConnections;
        /// <summary>
        /// TCP侦听
        /// </summary>
        private static TcpListener _listener;        //防止随意获取玩家人数, _userCountListener;
        /// <summary>
        /// 启动服务器
        /// </summary>
        /// <param name="log"></param>
        private static void StartNetwork(bool log = true)
        {
            try
            {
                NewConnections = new ConcurrentQueue<SConnection>();

                _listener = new TcpListener(IPAddress.Parse(Config.IPAddress), Config.Port);
                _listener.Start();
                _listener.BeginAcceptTcpClient(Connection, null);

                //_userCountListener = new TcpListener(IPAddress.Parse(Config.IPAddress), Config.UserCountPort);
                //_userCountListener.Start();
                //_userCountListener.BeginAcceptTcpClient(CountConnection, null);

                NetworkStarted = true;
                if (log) Log("服务器启动成功");
            }
            catch (Exception ex)
            {
                //Started = false;
                Stopping = true;

                Log(ex.ToString());
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(ex);
                }

            }
        }
        /// <summary>
        /// 停止服务器
        /// </summary>
        /// <param name="log"></param>
        private static void StopNetwork(bool log = true)
        {
            TcpListener expiredListener = _listener;
            //TcpListener expiredUserListener = _userCountListener;

            _listener = null;
            //_userCountListener = null;

            //Started = false;

            expiredListener?.Stop();
            //expiredUserListener?.Stop();

            NewConnections = null;

            try
            {
                Packet p = new G.Disconnect { Reason = DisconnectReason.ServerClosing };
                for (int i = Connections.Count - 1; i >= 0; i--)
                    Connections[i].SendDisconnect(p);

                LangEx.Langs.Clear();
                Thread.Sleep(2000);

            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(ex);
                }

            }

            if (log) Log("服务器停止");
        }
        /// <summary>
        /// 连接 异步结果
        /// </summary>
        /// <param name="result"></param>
        private static void Connection(IAsyncResult result)
        {
            try
            {
                if (_listener == null || !_listener.Server.IsBound) return;

                TcpClient client = _listener.EndAcceptTcpClient(result);

                string ipAddress = client.Client.RemoteEndPoint.ToString().Split(':')[0];

                if (!IPBlocks.TryGetValue(ipAddress, out DateTime banDate) || banDate < Now)
                {
                    SConnection Connection = new SConnection(client);

                    if (Connection.Connected)
                        NewConnections?.Enqueue(Connection);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(ex);
                }

            }
            finally
            {
                while (NewConnections?.Count >= 15)
                    Thread.Sleep(1);

                if (_listener != null && _listener.Server.IsBound)
                    _listener.BeginAcceptTcpClient(Connection, null);
            }
        }

        /*private static void CountConnection(IAsyncResult result)   //计数连接数
        {
            try
            {
                if (_userCountListener == null || !_userCountListener.Server.IsBound) return;

                TcpClient client = _userCountListener.EndAcceptTcpClient(result);

                byte[] data = Encoding.ASCII.GetBytes(string.Format("c;/Zircon/{0}/;", Connections.Count));

                client.Client.BeginSend(data, 0, data.Length, SocketFlags.None, CountConnectionEnd, client);
            }
            catch {}
            finally
            {
                if (_userCountListener != null && _userCountListener.Server.IsBound)
                    _userCountListener.BeginAcceptTcpClient(CountConnection, null);
            }
        }

        private static void CountConnectionEnd(IAsyncResult result)   //计数连接结束
        {
            try
            {
                TcpClient client = result.AsyncState as TcpClient;

                if (client == null) return;

                client.Client.EndSend(result);

                client.Client.Dispose();
            }
            catch {}
        }*/

        #endregion

        #region 网络服务器
        /// <summary>
        /// WEB侦听
        /// </summary>
        private static HttpListener WebListener;
        /// <summary>
        /// 激活命令
        /// </summary>
        private const string ActivationCommand = "Activation";
        /// <summary>
        /// 重置命令
        /// </summary>
        private const string ResetCommand = "Reset";
        /// <summary>
        /// 删除命令
        /// </summary>
        private const string DeleteCommand = "Delete";
        /// <summary>
        /// 激活KEY命令
        /// </summary>
        private const string ActivationKey = "ActivationKey";
        /// <summary>
        /// 重置KEY命令
        /// </summary>
        private const string ResetKey = "ResetKey";
        /// <summary>
        /// 删除KEY命令
        /// </summary>
        private const string DeleteKey = "DeleteKey";
        /// <summary>
        /// 结束命令
        /// </summary>
        private const string Completed = "Completed";
        /// <summary>
        /// 货币命令
        /// </summary>
        private const string Currency = "USD";
        /// <summary>
        /// 金币比例表
        /// </summary>
        private static Dictionary<decimal, int> GoldTable = new Dictionary<decimal, int>();
        public static void CheckGoldTable()
        {
            for (int i = 0; i < GameGoldSetInfo.Count; i++)
            {
                AddGoldTable(GameGoldSetInfo[i].Price, GameGoldSetInfo[i].GameGoldAmount);
            }
        }
        public static void AddGoldTable(decimal KeyPrice, int GameGoldAmount)
        {
            foreach (KeyValuePair<decimal, int> pair in GoldTable)
            {
                if (pair.Key == KeyPrice)
                {
                    return;
                }
            }
            GoldTable.Add(KeyPrice, GameGoldAmount);
        }
        //private static Dictionary<decimal, int> GoldTable = new Dictionary<decimal, int>
        //{
        //    [5M] = 500,
        //    [10M] = 1030,
        //    [15M] = 1590,
        //    [20M] = 2180,
        //    [30M] = 3360,
        //    [50M] = 5750,
        //    [100M] = 12000,
        //};
        /// <summary>
        /// 验证路径
        /// </summary>
        public const string VerifiedPath = @"./Database/Store/Verified/";
        /// <summary>
        /// 无效路径
        /// </summary>
        public const string InvalidPath = @"./Database/Store/Invalid/";
        /// <summary>
        /// 完全路径
        /// </summary>
        public const string CompletePath = @"./Database/Store/Complete/";
        /// <summary>
        /// 购买侦听
        /// </summary>
        private static HttpListener BuyListener;
        /// <summary>
        /// IPN侦听
        /// </summary>
        private static HttpListener IPNListener;
        /// <summary>
        /// 并发队列IPN信息
        /// </summary>
        public static ConcurrentQueue<IPNMessage> Messages = new ConcurrentQueue<IPNMessage>();
        /// <summary>
        /// 付款记录
        /// </summary>
        public static List<IPNMessage> PaymentList = new List<IPNMessage>();
        /// <summary>
        /// 已处理的付款
        /// </summary>
        public static List<IPNMessage> HandledPayments = new List<IPNMessage>();

        /// <summary>
        /// 启动WEB服务器
        /// </summary>
        /// <param name="log"></param>
        public static void StartWebServer(bool log = false)   // true开启  false关闭
        {
            try
            {
                WebCommandQueue = new ConcurrentQueue<WebCommand>();

                WebListener = new HttpListener();
                WebListener.Prefixes.Add(Config.WebPrefix);

                WebListener.Start();
                WebListener.BeginGetContext(WebConnection, null);

                BuyListener = new HttpListener();
                BuyListener.Prefixes.Add(Config.BuyPrefix);

                IPNListener = new HttpListener();
                IPNListener.Prefixes.Add(Config.IPNPrefix);

                BuyListener.Start();
                BuyListener.BeginGetContext(BuyConnection, null);

                IPNListener.Start();
                IPNListener.BeginGetContext(IPNConnection, null);

                WebServerStarted = true;

                if (log) Log("Web服务器启动成功");
            }
            catch (Exception ex)
            {
                if (ex is System.Net.HttpListenerException)
                {
                    Log("无法启动网页服务，请检查80端口占用，或使用管理员权限运行程序，单机版可无视HttpListenerException错误");
                }

                WebServerStarted = false;
                Log(ex.ToString());

                if (WebListener != null && WebListener.IsListening)
                    WebListener?.Stop();
                WebListener = null;

                if (BuyListener != null && BuyListener.IsListening)
                    BuyListener?.Stop();
                BuyListener = null;

                if (IPNListener != null && IPNListener.IsListening)
                    IPNListener?.Stop();
                IPNListener = null;
            }
        }
        /// <summary>
        /// 停止网络服务器
        /// </summary>
        /// <param name="log"></param>
        public static void StopWebServer(bool log = true)
        {
            HttpListener expiredWebListener = WebListener;
            WebListener = null;

            HttpListener expiredBuyListener = BuyListener;
            BuyListener = null;
            HttpListener expiredIPNListener = IPNListener;
            IPNListener = null;

            WebServerStarted = false;
            expiredWebListener?.Stop();
            expiredBuyListener?.Stop();
            expiredIPNListener?.Stop();

            //if (log) Log("Web服务器停止.");            
        }
        /// <summary>
        /// WEB连接
        /// </summary>
        /// <param name="result"></param>
        private static void WebConnection(IAsyncResult result)
        {
            try
            {
                if (WebListener != null)
                {
                    HttpListenerContext context = WebListener.EndGetContext(result);

                    string command = context.Request.QueryString["Type"];

                    switch (command)
                    {
                        case ActivationCommand:
                            Activation(context);
                            break;
                        case ResetCommand:
                            ResetPassword(context);
                            break;
                        case DeleteCommand:
                            DeleteAccount(context);
                            break;
                    }
                }
            }
            catch { }

            finally
            {
                if (WebListener != null && WebListener.IsListening)
                    WebListener.BeginGetContext(WebConnection, null);
            }
        }
        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="context"></param>
        private static void Activation(HttpListenerContext context)
        {
            string key = context.Request.QueryString[ActivationKey];

            if (string.IsNullOrEmpty(key)) return;

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
            {
                AccountInfo temp = AccountInfoList[i]; //不同的线程，必须小心以防出错
                if (string.Compare(temp.ActivationKey, key, StringComparison.Ordinal) != 0) continue;

                account = temp;
                break;
            }

            if (Config.AllowWebActivation && account != null)
            {
                WebCommandQueue.Enqueue(new WebCommand(CommandType.Activation, account));
                context.Response.Redirect(Config.ActivationSuccessLink);
            }
            else
                context.Response.Redirect(Config.ActivationFailLink);

            context.Response.Close();
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="context"></param>
        private static void ResetPassword(HttpListenerContext context)
        {
            string key = context.Request.QueryString[ResetKey];

            if (string.IsNullOrEmpty(key)) return;

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
            {
                AccountInfo temp = AccountInfoList[i]; //不同的线程，必须小心以防出错
                if (string.Compare(temp.ResetKey, key, StringComparison.Ordinal) != 0) continue;

                account = temp;
                break;
            }

            if (Config.AllowWebResetPassword && account != null && account.ResetTime.AddMinutes(25) > Now)
            {
                WebCommandQueue.Enqueue(new WebCommand(CommandType.PasswordReset, account));
                context.Response.Redirect(Config.ResetSuccessLink);
            }
            else
                context.Response.Redirect(Config.ResetFailLink);

            context.Response.Close();
        }
        /// <summary>
        /// 删除账号
        /// </summary>
        /// <param name="context"></param>
        private static void DeleteAccount(HttpListenerContext context)
        {
            string key = context.Request.QueryString[DeleteKey];

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
            {
                AccountInfo temp = AccountInfoList[i]; //不同的线程，必须小心以防出错
                if (string.Compare(temp.ActivationKey, key, StringComparison.Ordinal) != 0) continue;

                account = temp;
                break;
            }

            if (Config.AllowDeleteAccount && account != null)
            {
                WebCommandQueue.Enqueue(new WebCommand(CommandType.AccountDelete, account));
                context.Response.Redirect(Config.DeleteSuccessLink);
            }
            else
                context.Response.Redirect(Config.DeleteFailLink);

            context.Response.Close();
        }
        /// <summary>
        /// 购买链接
        /// </summary>
        /// <param name="result"></param>
        private static void BuyConnection(IAsyncResult result)
        {
            try
            {
                if (BuyListener != null)
                {
                    HttpListenerContext context = BuyListener.EndGetContext(result);

                    NameValueCollection nc = HttpUtility.ParseQueryString(context.Request.Url.Query, Encoding.GetEncoding("utf-8"));
                    string characterName = nc["Character"];

                    CharacterInfo character = null;
                    for (int i = 0; i < CharacterInfoList.Count; i++)
                    {
                        if (string.Compare(CharacterInfoList[i].CharacterName, characterName, StringComparison.OrdinalIgnoreCase) != 0) continue;

                        character = CharacterInfoList[i];
                        break;
                    }

                    if (character?.Account.Key != context.Request.QueryString["Key"])
                        character = null;

                    string response = character == null ? Resources.CharacterNotFound : BuyGameGold.Replace("$CHARACTERNAME$", HttpUtility.UrlEncode(character.CharacterName));

                    using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, context.Request.ContentEncoding))
                        writer.Write(response);
                }
            }
            catch { }

            finally
            {
                if (BuyListener != null && BuyListener.IsListening)
                    BuyListener.BeginGetContext(BuyConnection, null);
            }
        }
        /// <summary>
        /// IPN链接
        /// </summary>
        /// <param name="result"></param>
        private static void IPNConnection(IAsyncResult result)
        {
            const string LiveURL = @"https://ipnpb.paypal.com/cgi-bin/webscr";

            const string verified = "VERIFIED";

            try
            {
                if (IPNListener == null || !IPNListener.IsListening) return;

                HttpListenerContext context = IPNListener.EndGetContext(result);

                string rawMessage;
                using (StreamReader readStream = new StreamReader(context.Request.InputStream, Encoding.UTF8))
                    rawMessage = readStream.ReadToEnd();

                Task.Run(() =>
                {
                    string data = "cmd=_notify-validate&" + rawMessage;

                    HttpWebRequest wRequest = (HttpWebRequest)WebRequest.Create(LiveURL);

                    wRequest.Method = "POST";
                    wRequest.ContentType = "application/x-www-form-urlencoded";
                    wRequest.ContentLength = data.Length;

                    using (StreamWriter writer = new StreamWriter(wRequest.GetRequestStream(), Encoding.ASCII))
                        writer.Write(data);

                    using (StreamReader reader = new StreamReader(wRequest.GetResponse().GetResponseStream()))
                    {
                        IPNMessage message = new IPNMessage { Message = rawMessage, Verified = reader.ReadToEnd() == verified };

                        if (!Directory.Exists(VerifiedPath))
                            Directory.CreateDirectory(VerifiedPath);

                        if (!Directory.Exists(InvalidPath))
                            Directory.CreateDirectory(InvalidPath);

                        string path = (message.Verified ? VerifiedPath : InvalidPath) + Path.GetRandomFileName();

                        File.WriteAllText(path, message.Message);

                        message.FileName = path;

                        Messages.Enqueue(message);
                    }
                });

                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.Close();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
                if (IPNListener != null && IPNListener.IsListening) //IsBound ?
                    IPNListener.BeginGetContext(IPNConnection, null);
            }
        }
        #endregion
        public static Dictionary<int, object> TempValue = new Dictionary<int, object>();
        private static Dictionary<int, GolbleValue> Values = new Dictionary<int, GolbleValue>();
        public static void SetValues(int key, int value)
        {
            GolbleValue ob;
            if (Values.TryGetValue(key, out ob))
            {
                ob.Key = key;
                ob.Value = value;

            }
            else
            {
                ob = SEnvir.GolbleValueList.CreateNewObject();
                ob.Key = key;
                ob.Value = value;
                Values[key] = ob;
            }
        }
        public static int GetValues(int key)
        {
            GolbleValue ob;
            int ret = 0;
            if (Values.TryGetValue(key, out ob))
            {
                ret = ob.Value;

            }
            return ret;
        }

        public static object GetObValues(int key)
        {
            GolbleValue ob;
            object ret = null;
            if (Values.TryGetValue(key, out ob))
            {
                ret = ob.ObjctValue;

            }
            return ret;
        }

        public static void SetObValues(int key, object value)
        {
            GolbleValue ob;
            if (Values.TryGetValue(key, out ob))
            {
                ob.Key = key;
                ob.ObjctValue = value;

            }
            else
            {
                ob = SEnvir.GolbleValueList.CreateNewObject();
                ob.Key = key;
                ob.ObjctValue = value;
                Values[key] = ob;
            }
        }
        /// <summary>
        /// 是否有效授权
        /// </summary>
        ///
        public static bool LicenseValid { get; set; } = false;
        /// <summary>
        /// 授权状态
        /// </summary>
        public static string LicenseState { get; set; } = "未知";

        /// <summary>
        /// 是否记录日志
        /// </summary>
        public static bool log1 { get; set; }
        /// <summary>
        /// 是否启动
        /// </summary>
        public static bool Started { get; set; }
        /// <summary>
        /// 网络是否启动
        /// </summary>
        public static bool NetworkStarted { get; set; }
        /// <summary>
        /// WEB服务器是否启动
        /// </summary>
        public static bool WebServerStarted { get; set; }
        /// <summary>
        /// 是否保留
        /// </summary>
        public static bool Saving { get; private set; }
        /// <summary>
        /// 环境线程
        /// </summary>
        public static Thread EnvirThread { get; private set; }
        /// <summary>
        /// 现在时间
        /// </summary>
        public static DateTime Now;
        public static DateTime DiyShowViewerLoopTime;
        /// <summary>
        /// 开始时间
        /// </summary>
        public static DateTime StartTime;
        /// <summary>
        /// 释放内存时间
        /// </summary>
        public static DateTime GCTime;
        /// <summary>
        /// 最后战争时间
        /// </summary>
        public static DateTime LastWarTime;
        /// <summary>
        /// 进程对象计数
        /// </summary>
        public static int ProcessObjectCount;
        /// <summary>
        /// 循环计数
        /// </summary>
        public static int LoopCount;
        /// <summary>
        /// 发送的字节数
        /// </summary>
        public static long DBytesSent;
        /// <summary>
        /// 接收的字节数
        /// </summary>
        public static long DBytesReceived;
        /// <summary>
        /// 发送的字节总数
        /// </summary>
        public static long TotalBytesSent;
        /// <summary>
        /// 接收的字节总数
        /// </summary>
        public static long TotalBytesReceived;
        /// <summary>
        /// 下载速度
        /// </summary>
        public static long DownloadSpeed;
        /// <summary>
        /// 上传速度
        /// </summary>
        public static long UploadSpeed;
        /// <summary>
        /// 邮件发送数
        /// </summary>
        public static int EMailsSent;
        /// <summary>
        /// 公告数组
        /// </summary>
        private static List<string> NoticeList;
        /// <summary>
        /// 服务器BUFF设置改变
        /// </summary>
        public static bool ServerBuffChanged;
        /// <summary>
        /// 活动BUFF设置改变
        /// </summary>
        public static bool EventBuffChanged;
        /// <summary>
        /// 文件系统观察程序 观察者为空
        /// </summary>
        public static FileSystemWatcher watcher = null;
        /// <summary>
        /// 事件包事件
        /// </summary>
        public static DateTime EventPacketTime;
        /// <summary>
        /// 事件包间隔时间
        /// </summary>
        public static TimeSpan EventPacketInterval { get; set; } = TimeSpan.FromSeconds(1);
        /// <summary>
        /// 成就改变
        /// </summary>
        public static HashSet<int> ChangedAchievementIndices = new HashSet<int>();

        #region DB数据库
        /// <summary>
        /// 会话控制
        /// </summary>
        public static Session Session;
        /// <summary>
        /// 地图信息列表
        /// </summary>
        public static DBCollection<MapInfo> MapInfoList;
        /// <summary>
        /// 国际化信息列表
        /// </summary>
        public static DBCollection<LangInfo> LangInfoList;
        /// <summary>
        /// 安全区信息列表
        /// </summary>
        public static DBCollection<SafeZoneInfo> SafeZoneInfoList;
        /// <summary>
        /// 道具信息列表
        /// </summary>
        public static DBCollection<ItemInfo> ItemInfoList;
        /// <summary>
        /// 地图怪物刷新信息列表
        /// </summary>
        public static DBCollection<RespawnInfo> RespawnInfoList;
        /// <summary>
        /// 魔法技能信息列表
        /// </summary>
        public static DBCollection<MagicInfo> MagicInfoList;
        /// <summary>
        /// 宠物捡取道具分类列表
        /// </summary>
        public static DBCollection<UserSorting> SortingList;
        /// <summary>
        /// 宠物捡取高级稀世分类列表
        /// </summary>
        public static DBCollection<UserSortingLev> SortingLevList;
        /// <summary>
        /// 账号信息列表
        /// </summary>
        public static DBCollection<AccountInfo> AccountInfoList;
        /// <summary>
        /// 好友信息列表
        /// </summary>
        public static DBCollection<FriendInfo> FriendsList;
        /// <summary>
        /// 角色信息列表
        /// </summary>
        public static DBCollection<CharacterInfo> CharacterInfoList;
        /// <summary>
        /// 角色药品快捷栏列表
        /// </summary>
        public static DBCollection<CharacterBeltLink> BeltLinkList;
        /// <summary>
        /// 角色自动喝药栏列表
        /// </summary>
        public static DBCollection<AutoPotionLink> AutoPotionLinkList;
        /// <summary>
        /// 角色自动打怪设置列表
        /// </summary>
        public static DBCollection<AutoFightConfig> AutoFightConfList;
        /// <summary>
        /// 角色道具列表
        /// </summary>
        public static DBCollection<UserItem> UserItemList;
        /// <summary>
        /// 精炼信息列表
        /// </summary>
        public static DBCollection<RefineInfo> RefineInfoList;
        /// <summary>
        /// 角色道具属性状态列表
        /// </summary>
        public static DBCollection<UserItemStat> UserItemStatsList;
        /// <summary>
        /// 角色魔法技能列表
        /// </summary>
        public static DBCollection<UserMagic> UserMagicList;
        /// <summary>
        /// BUFF信息列表
        /// </summary>
        public static DBCollection<BuffInfo> BuffInfoList;
        /// <summary>
        /// 怪物信息列表
        /// </summary>
        public static DBCollection<MonsterInfo> MonsterInfoList;
        /// <summary>
        /// 寄售信息列表
        /// </summary>
        public static DBCollection<AuctionInfo> AuctionInfoList;

        public static DBCollection<CharacterShop> CharacterShopList;
        /// <summary>
        /// 邮件信息列表
        /// </summary>
        public static DBCollection<MailInfo> MailInfoList;
        /// <summary>
        /// 寄售历史信息列表
        /// </summary>
        public static DBCollection<AuctionHistoryInfo> AuctionHistoryInfoList;
        /// <summary>
        /// 角色爆率列表
        /// </summary>
        public static DBCollection<UserDrop> UserDropList;
        /// <summary>
        /// 商城信息列表
        /// </summary>
        public static DBCollection<StoreInfo> StoreInfoList;
        /// <summary>
        /// 初始属性状态列表
        /// </summary>
        public static DBCollection<BaseStat> BaseStatList;
        /// <summary>
        /// 地图链接信息列表
        /// </summary>
        public static DBCollection<MovementInfo> MovementInfoList;
        /// <summary>
        /// NPC信息列表
        /// </summary>
        public static DBCollection<NPCInfo> NPCInfoList;
        /// <summary>
        /// 地图区域列表
        /// </summary>
        public static DBCollection<MapRegion> MapRegionList;

        /// <summary>
        /// 行会升级经验列表
        /// </summary>
        public static DBCollection<GuildLevelExp> GuildLevelExpList;
        /// <summary>
        /// 行会信息列表
        /// </summary>
        public static DBCollection<GuildInfo> GuildInfoList;
        /// <summary>
        /// 行会成员信息列表
        /// </summary>
        public static DBCollection<GuildMemberInfo> GuildMemberInfoList;
        /// <summary>
        /// 角色任务信息列表
        /// </summary>
        public static DBCollection<UserQuest> UserQuestList;
        /// <summary>
        /// 角色任务交接信息列表
        /// </summary>
        public static DBCollection<UserQuestTask> UserQuestTaskList;
        /// <summary>
        /// 宠物信息列表
        /// </summary>
        public static DBCollection<CompanionInfo> CompanionInfoList;
        /// <summary>
        /// 宠物等级信息列表
        /// </summary>
        public static DBCollection<CompanionLevelInfo> CompanionLevelInfoList;
        /// <summary>
        /// 角色宠物信息列表
        /// </summary>
        public static DBCollection<UserCompanion> UserCompanionList;
        /// <summary>
        /// 角色宠物解锁信息列表
        /// </summary>
        public static DBCollection<UserCompanionUnlock> UserCompanionUnlockList;
        /// <summary>
        /// 宠物技能信息列表
        /// </summary>
        public static DBCollection<CompanionSkillInfo> CompanionSkillInfoList;
        /// <summary>
        /// 黑名单信息列表
        /// </summary>
        public static DBCollection<BlockInfo> BlockInfoList;
        /// <summary>
        /// 城堡信息列表
        /// </summary>
        public static DBCollection<CastleInfo> CastleInfoList;
        /// <summary>
        /// 参与攻城的角色信息列表
        /// </summary>
        public static DBCollection<UserConquest> UserConquestList;
        /// <summary>
        /// 元宝购买信息列表
        /// </summary>
        public static DBCollection<GameGoldPayment> GameGoldPaymentList;
        /// <summary>
        /// 元宝购买信息列表
        /// </summary>
        public static DBCollection<GameStoreSale> GameStoreSaleList;
        /// <summary>
        /// 行会战信息列表
        /// </summary>
        public static DBCollection<GuildWarInfo> GuildWarInfoList;
        /// <summary>
        /// 行会联盟信息列表
        /// </summary>
        public static DBCollection<GuildAllianceInfo> GuildAllianceInfoList;
        /// <summary>
        /// 角色攻城统计信息列表
        /// </summary>
        public static DBCollection<UserConquestStats> UserConquestStatsList;
        /// <summary>
        /// 角色财富信息列表
        /// </summary>
        public static DBCollection<UserFortuneInfo> UserFortuneInfoList;
        /// <summary>
        /// 武器工艺信息列表
        /// </summary>
        public static DBCollection<WeaponCraftStatInfo> WeaponCraftStatInfoList;
        /// <summary>
        /// 道具制作信息列表
        /// </summary>
        public static DBCollection<CraftItemInfo> CraftItemInfoList;
        /// <summary>
        /// 初始化等级经验列表
        /// </summary>
		public static DBCollection<PlayerExpInfo> GamePlayEXPInfoList;
        /// <summary>
        /// 初始化武器等级经验列表
        /// </summary>
        public static DBCollection<WeaponExpInfo> GameWeaponEXPInfoList;
        /// <summary>
        /// 初始化首饰等级经验列表
        /// </summary>
        public static DBCollection<AccessoryExpInfo> GameAccessoryEXPInfoList;
        /// <summary>
        /// 角色成就信息列表
        /// </summary>
        public static DBCollection<UserAchievement> UserAchievementList;
        /// <summary>
        /// 角色成就需求信息列表
        /// </summary>
        public static DBCollection<UserAchievementRequirement> UserAchievementRequirementList;
        /// <summary>
        /// 传送记忆栏信息列表
        /// </summary>
        public static DBCollection<TriggerInfo> TriggerInfoList;
        /// <summary>
        /// 钓鱼区域信息列表
        /// </summary>
        public static DBCollection<FishingAreaInfo> FishingAreaInfoList;
        /// <summary>
        /// 角色快捷键定义值列表
        /// </summary>
        public static DBCollection<UserValue> UserValueList;
        /// <summary>
        /// 全局变量列表
        /// </summary>
        public static DBCollection<GolbleValue> GolbleValueList;
        /// <summary>
        /// 新版武器升级设置
        /// </summary>
        public static DBCollection<WeaponUpgradeNew> WeaponUpgradeInfoList;
        /// <summary>
        /// 自定义技能动画
        /// </summary>
        public static DBCollection<DiyMagicEffect> DiyMagicEffectList;
        /// <summary>
        /// 自定义怪物AI
        /// </summary>
        public static DBCollection<MonDiyAiAction> MonDiyActionList;
        /// <summary>
        /// 自定义怪物
        /// </summary>
        public static DBCollection<MonAnimationFrame> MonAnimationFrameList;
        /// <summary>
        /// 元宝货币购买
        /// </summary>
        public static DBCollection<GameGoldSet> GameGoldSetInfo;
        /// <summary>
        /// 城堡收入信息
        /// </summary>
        public static DBCollection<CastleFundInfo> CastleFundInfoList;
        /// <summary>
        /// 金币交易行
        /// </summary>
        public static DBCollection<GoldMarketInfo> GoldMarketInfoList;
        /// <summary>
        /// 拍卖行
        /// </summary>
        public static DBCollection<NewAutionInfo> NewAutionInfoList;
        /// <summary>
        /// 行会捐赠记录
        /// </summary>
        public static DBCollection<GuildFundChangeInfo> GuildFundChangeInfoList;
        /// <summary>
        /// 元宝信息
        /// </summary>
        public static ItemInfo GameGoldInfo;
        /// <summary>
        /// 金币信息
        /// </summary>
        public static ItemInfo GoldInfo;
        /// <summary>
        /// 制炼石信息
        /// </summary>
        public static ItemInfo RefinementStoneInfo;
        /// <summary>
        /// 初级碎片信息
        /// </summary>
        public static ItemInfo FragmentInfo;
        /// <summary>
        /// 中级碎片信息
        /// </summary>
        public static ItemInfo Fragment2Info;
        /// <summary>
        /// 高级碎片信息
        /// </summary>
        public static ItemInfo Fragment3Info;
        /// <summary>
        /// 财富检查道具信息
        /// </summary>
        public static ItemInfo FortuneCheckerInfo;
        /// <summary>
        /// 碎片道具信息
        /// </summary>
        public static ItemInfo ItemPartInfo;
        /// <summary>
        /// 声望信息
        /// </summary>
        public static ItemInfo PrestigeInfo;
        /// <summary>
        /// 贡献值信息
        /// </summary>
        public static ItemInfo ContributeInfo;
        /// <summary>
        /// 新手行会
        /// </summary>
        public static GuildInfo StarterGuild;
        /// <summary>
        /// 异界之门入口区域
        /// </summary>
        public static MapRegion MysteryShipMapRegion;
        /// <summary>
        /// 赤龙城入口区域
        /// </summary>
        public static MapRegion LairMapRegion;
        /// <summary>
        /// 成功传送地图
        /// </summary>
        public static MapRegion RightDeliver;
        /// <summary>
        /// 失败传送地图
        /// </summary>
        public static MapRegion ErrorDeliver;
        /// <summary>
        /// 怪物BOSS列表
        /// </summary>
        public static List<MonsterInfo> BossList = new List<MonsterInfo>();
        /// <summary>
        /// 处理道具制作列表
        /// </summary>
        public static List<ItemInfo> TreaItemList = new List<ItemInfo>();
        /// <summary>
        /// 角色成就要求列表(暂时没用)
        /// </summary>
        public static IDictionary<AchievementRequirementType, List<UserAchievementRequirement>> UserAchievementRequirementDict;
        /// <summary>
        /// 记忆传送列表
        /// </summary>
        public static DBCollection<FixedPointInfo> FixedPointInfoList;
        /// <summary>
        /// 鱼列表
        /// </summary>
        public static List<ItemInfo> FishList;
        /// <summary>
        /// 账号交易
        /// </summary>
        public static List<NewAutionInfo> NewAutionInfoTempList = new List<NewAutionInfo>();
        /// <summary>
        /// 可回购道具列表
        /// </summary>
        public static List<UserItem> AllOwnerlessItemList;
        /// <summary>
        /// 增量道具列表 只存有本次重启以来 新增的可回购物品
        /// </summary> 暂时不用
        //public static List<UserItem> TempOwnerlessUserItemList = new List<UserItem>();

        /// 奖金池信息
        public static DBCollection<RewardPoolInfo> RewardPoolInfoList;
        /// <summary>
        /// 只有一个奖金池
        /// </summary>
        public static RewardPoolInfo TheRewardPoolInfo;
        /// <summary>
        /// 红包信息
        /// </summary>
        public static DBCollection<RedPacketInfo> RedPacketInfoList;
        /// <summary>
        /// 红包领取信息
        /// </summary>
        public static DBCollection<RedPacketClaimInfo> RedPacketClaimInfoList;

        #endregion

        #region Game Variables  变量
        /// <summary>
        /// 随机值
        /// </summary>
        public static Random Random;
        /// <summary>
        /// 并发队列WEB命令队列
        /// </summary>
        public static ConcurrentQueue<WebCommand> WebCommandQueue;
        /// <summary>
        /// 地图信息
        /// </summary>
        public static Dictionary<MapInfo, Map> Maps = new Dictionary<MapInfo, Map>();
        /// <summary>
        /// 添加副本地图列表 副本动态生成
        /// </summary>
        public static List<Map> FubenMaps = new List<Map>();
        /// <summary>
        /// 对象索引
        /// </summary>
        private static long _ObjectID;
        /// <summary>
        /// 对象索引增量
        /// </summary>
        public static uint ObjectID => (uint)Interlocked.Increment(ref _ObjectID);
        /// <summary>
        /// 地图对象列表
        /// </summary>
        public static LinkedList<MapObject> Objects = new LinkedList<MapObject>();
        /// <summary>
        /// 地图活动对象列表
        /// </summary>
        public static List<MapObject> ActiveObjects = new List<MapObject>();

        public static List<PlayerObject> Players = new List<PlayerObject>();

        /// <summary>
        /// 攻城对象列表
        /// </summary>
        public static List<ConquestWar> ConquestWars = new List<ConquestWar>();
        /// <summary>
        /// 刷怪信息列表
        /// </summary>
        public static List<SpawnInfo> Spawns = new List<SpawnInfo>();
        /// <summary>
        /// BOSS信息列表
        /// </summary>
        public static List<BossTracker> BossTrackerList = new List<BossTracker>();

        //关闭服务器自动广告功能  定时器
        public static System.Timers.Timer SEvirtimer;
        public static DateTime ShowGameShowTime;
        public static int TimerSecondTick = 0;

        public static string BuyGameGold;

        public static string[] BanCPUList;
        public static string[] BanHDDList;
        public static string[] BanMACList;

        public static List<string> BanProcessList;

        private static float _DayTime;

        /// <summary>
        /// 天时间
        /// </summary>
        public static float DayTime
        {
            get { return _DayTime; }
            set
            {
                if (_DayTime == value) return;

                _DayTime = value;

                Broadcast(new S.DayChanged { DayTime = DayTime });
            }
        }

        private static void SEvirtimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //关闭服务器自动广告功能
            if (Config.CheckAutoCloseServer)
            {
                TimerSecondTick--;
                if ((TimerSecondTick > 0) && (TimerSecondTick % Config.TimeAutoCloseServer == 0))
                {
                    ShowAllMessage("SEnvir.AutoCloseServerText", TimerSecondTick);
                }

                if (TimerSecondTick == 0)
                {
                    //关闭服务器允许登陆
                    Config.AllowLogin = false;
                    //踢掉所有玩家
                    if (OriginalLoop)
                    {
                        if (EnvirThread == null) return;
                        Started = false;
                        while (EnvirThread != null) Thread.Sleep(1);
                    }
                    else
                    {
                        Stopping = true;
                        while (Started) Thread.Sleep(1);
                    }
                }
            }//关闭服务器自动广告功能 结束
        }

        public static void ShowAllMessage(string lankey, params object[] param)
        {
            ShowAllMessage(lankey, MessageType.Announcement, param);
        }
        public static void ShowAllMessage(string lankey, MessageType messageType, params object[] param)
        {
            if (lankey == "")
            {
                return;
            }
            foreach (SConnection conn in Connections)
            {
                conn.ReceiveChat(lankey.Lang(conn.Language, param), messageType);

                switch (conn.Stage)
                {
                    case GameStage.Game:
                        if (conn.Player.Character.Observable)
                            conn.ReceiveChat(lankey.Lang(conn.Language, param), messageType);
                        break;
                    case GameStage.Observer:
                        conn.ReceiveChat(lankey.Lang(conn.Language, param), messageType);
                        break;
                }
            }
        }

        /// <summary>
        /// 排行榜列表角色信息
        /// </summary>
        public static LinkedList<CharacterInfo> Rankings;
        /// <summary>
        /// 排名靠前的排行榜角色信息
        /// </summary>
        public static HashSet<CharacterInfo> TopRankings;
        /// <summary>
        /// CON延迟
        /// </summary>
        public static long ConDelay;
        /// <summary>
        /// 主循环延迟
        /// </summary>
        public static long MainDelay;
        /// <summary>
        /// 主进程循环延迟
        /// </summary>
        public static long ProcessDelay;
        /// <summary>
        /// 保存延时
        /// </summary>
        public static long SaveDelay;
        // 监视的怪物列表 
        // int 是RespawnInfo的index
        public static Dictionary<int, Dictionary<string, int>> WathchingMonsters =
            new Dictionary<int, Dictionary<string, int>>();

        // 永久性的NPC外观设置
        // int 是Character的index
        public static List<S.UpdateNPCLook> UpdatedNPCLooks = new List<S.UpdateNPCLook>();
        /// <summary>
        /// 后上线的玩家也应该能看到脚本改变的天气
        /// </summary>
        public static Dictionary<int, WeatherSetting> WeatherOverrides = new Dictionary<int, WeatherSetting>();
        #endregion

        #region 事件管理器singleton
        /// <summary>
        /// 事件管理器实例
        /// </summary>
        private static EventManager _instance = null;
        /// <summary>
        /// 事件管理
        /// </summary>
        public static EventManager EventManager
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventManager();
                }

                return _instance;
            }
        }

        #endregion

        #region 全局异常记录
        /// <summary>
        /// 全局异常记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {
                Exception e = (Exception)args.ExceptionObject;

                string LogPath = @"./";

                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                File.AppendAllText($"{LogPath}未处理的异常{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt",
                     e.Message + e.StackTrace + Environment.NewLine + Environment.NewLine);
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(e);
                }
            }
            catch (Exception ex)
            {
                Log($"处理UnhandledException时发生错误, 行号:{(new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber()}");
            }
        }

        #endregion

        #region 神舰 赤龙开关门信息

        public static DateTime NetherworldCloseTime
        {
            get => _netherworldCloseTime;
            set
            {
                if (value != _netherworldCloseTime)
                {
                    _netherworldCloseTime = value;
                    BroadcastGateInfo();
                }
            }
        }

        public static Point NetherworldLocation
        {
            get => _netherworldLocation;
            set => _netherworldLocation = value;
        }

        public static DateTime LairCloseTime
        {
            get => _lairCloseTime;
            set
            {
                if (value != _lairCloseTime)
                {
                    _lairCloseTime = value;
                    BroadcastGateInfo();
                }
            }
        }

        public static Point LairLocation
        {
            get => _lairLocation;
            set => _lairLocation = value;
        }

        private static DateTime _netherworldCloseTime = DateTime.MinValue;
        private static Point _netherworldLocation = Point.Empty;
        private static DateTime _lairCloseTime = DateTime.MinValue;
        private static Point _lairLocation = Point.Empty;

        public static void BroadcastGateInfo()
        {
            Broadcast(new S.GateInformation
            {
                NetherworldCloseTime = NetherworldCloseTime,
                LairCloseTime = LairCloseTime,
                NetherworldLocation = NetherworldLocation,
                LairLocation = LairLocation
            });
        }

        #endregion

        #region 规则引擎

        /// <summary>
        /// 规则文件的位置 此处位于服务端Workflows文件夹
        /// </summary>
        public const string workflowsPath = "Workflows";
        /// <summary>
        /// 监视规则文件变动
        /// </summary>
        public static FileSystemWatcher workflowWatcher = null;
        /// <summary>
        /// 上次读取规则文件的时间
        /// </summary>
        public static DateTime workflowLastRead = DateTime.MinValue;

        /// <summary>
        /// 允许规则引擎访问SEvnir, Functions中的变量和函数
        /// </summary>
        public static ReSettings reSettingsWithCustomTypes = GenerateReSettings();

        /// <summary>
        /// 规则引擎实例
        /// </summary>
        public static RulesEngine.RulesEngine GlobalRulesEngine = new RulesEngine.RulesEngine(logger: null, reSettings: reSettingsWithCustomTypes);

        /// <summary>
        /// 奖金池buff是否变化了
        /// </summary>
        public static bool RewardPoolBuffChanged = false;
        /// <summary>
        /// 当前奖金池的buff数据
        /// </summary>
        public static Dictionary<Stat, int> RewardPoolBuffs = new Dictionary<Stat, int>();
        /// <summary>
        /// 允许规则引擎访问SEvnir, Functions中的变量和函数
        /// 允许使用Enums, Point, TimeSpan, DateTime
        /// </summary>
        /// <returns></returns>
        public static ReSettings GenerateReSettings()
        {
            List<Type> types = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsEnum && t.Namespace == "Library")
                .ToList();

            types.Add(typeof(SEnvir));
            types.Add(typeof(Functions));
            types.Add(typeof(Point));
            types.Add(typeof(TimeSpan));
            types.Add(typeof(DateTime));

            return new ReSettings
            {
                CustomTypes = types.ToArray()
            };
        }

        /// <summary>
        /// 读取json文件内容 并尝试转换为workflow
        /// </summary>
        /// <param name="WorkflowsFileName"></param>
        /// <returns></returns>
        public static Workflow[] ParseAsWorkflow(string WorkflowsFileName)
        {
            string fileData = File.ReadAllText(WorkflowsFileName);
            return JsonConvert.DeserializeObject<Workflow[]>(fileData);
        }

        /// <summary>
        /// 载入规则
        /// </summary>
        public static void LoadWorkflows()
        {
            try
            {
                Log($"开始加载规则引擎");
                GlobalRulesEngine.ClearWorkflows();
                string[] workflowFiles = Directory.GetFiles(workflowsPath, "*.json", SearchOption.AllDirectories);
                if (workflowFiles == null || workflowFiles.Length == 0)
                {
                    Log("未找到任何规则文件, 规则引擎将不可用");
                    return;
                }

                Log($"找到{workflowFiles.Length}条规则");
                foreach (string workflowFile in workflowFiles)
                {
                    var workflow = ParseAsWorkflow(workflowFile);
                    GlobalRulesEngine.AddOrUpdateWorkflow(workflow);
                    Log($"添加 {workflowFile} 成功");
                }
            }
            catch (Exception e)
            {
                Log("加载规则引擎出错, 规则引擎将不可用");
                Log(e.ToString());
            }
        }

        /// <summary>
        /// 规则文件变化时触发
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        static void OnWorkflowChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                workflowWatcher.EnableRaisingEvents = false;
                DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
                if (lastWriteTime != workflowLastRead)
                {
                    LoadWorkflows();
                }
                workflowLastRead = lastWriteTime;
            }
            finally
            {
                workflowWatcher.EnableRaisingEvents = true;
            }
        }
        /// <summary>
        /// 规则文件重命名时触发
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        static void OnWorkflowRenamed(object source, RenamedEventArgs e)
        {
            LoadWorkflows();
        }

        #endregion

        #region 奖金池
        /// <summary>
        /// 上次更新排行榜的时间
        /// </summary>
        public static DateTime LastRPRankUpdateTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 累计获得奖池币的排名
        /// 从高到低 
        /// </summary>
        public static List<CharacterInfo> TotalRPCoinEarnedRanks = new List<CharacterInfo>();

        /// <summary>
        /// 前三名
        /// </summary>
        public static ClientRewardPoolRanks TotalRPCoinEarnedTop1
        {
            get => _totalRpCoinEarnedTop1;
            set
            {
                if (_totalRpCoinEarnedTop1 != value)
                    RPCoinEarnedRankChanged = true;
                _totalRpCoinEarnedTop1 = value;
            }
        }

        public static ClientRewardPoolRanks TotalRPCoinEarnedTop2
        {
            get => _totalRpCoinEarnedTop2;
            set
            {
                if (_totalRpCoinEarnedTop2 != value)
                    RPCoinEarnedRankChanged = true;
                _totalRpCoinEarnedTop2 = value;
            }
        }

        public static ClientRewardPoolRanks TotalRPCoinEarnedTop3
        {
            get => _totalRpCoinEarnedTop3;
            set
            {
                if (_totalRpCoinEarnedTop3 != value)
                    RPCoinEarnedRankChanged = true;
                _totalRpCoinEarnedTop3 = value;
            }
        }

        private static ClientRewardPoolRanks _totalRpCoinEarnedTop1;
        private static ClientRewardPoolRanks _totalRpCoinEarnedTop2;
        private static ClientRewardPoolRanks _totalRpCoinEarnedTop3;
        public static bool RPCoinEarnedRankChanged { get; set; }
        #region NLog日志 写入CSV

        /// <summary>
        /// 用于货币流水
        /// </summary>
        public static readonly NLog.Logger CurrencyLogger = NLog.LogManager.GetCurrentClassLogger();
        /// <summary>
        /// 默认文件位置在服务端Log文件夹内
        /// </summary>
        /// <param name="fileName"></param>
        public static void InitCsvFileLogger(string fileName)
        {
            var config = Utils.NLogConfig.GetCsvLoggerConfig(fileName);
            LogManager.Configuration = config;
        }

        /// <summary>
        /// 日志同时写入服务端界面以及CSV文件
        /// </summary>
        /// <param name="entry"></param>
        public static void LogToViewAndCSV(CurrencyLogEntry entry)
        {
            if (entry == null)
            {
                SEnvir.Log("写入日志失败, 条目为null");
                return;
            }

            CurrencyLogs.Enqueue(entry);

            if (entry.LogLevel == LogLevel.Info)
            {
                //LogEventInfo theEvent = new LogEventInfo(LogLevel.Info, null, "no message");
                //theEvent.Properties["component"] = entry.Component;

                //theEvent.Properties["name"] = entry.Character?.CharacterName;
                //theEvent.Properties["account"] = entry.Character?.Account.EMailAddress;
                //theEvent.Properties["ip"] = entry.Character?.Account.LastIP;
                //theEvent.Properties["currency"] = entry.Component;
                //theEvent.Properties["action"] = entry.Component;
                //theEvent.Properties["amount"] = entry.Component;
                //theEvent.Properties["source"] = entry.Component;
                //theEvent.Properties["extra"] = entry.Component;

                CurrencyLogger.Info("{component} {name} {account} {ip} {currency} {action} {amount} {source} {extra}",
                    entry.Component,
                    entry.Character?.CharacterName,
                    entry.Character?.Account.EMailAddress,
                    entry.Character?.Account.LastIP,
                    Functions.GetEnumDescription(entry.Currency),
                    Functions.GetEnumDescription(entry.Action),
                    entry.Amount,
                    Functions.GetEnumDescription(entry.Source),
                    entry.ExtraInfo);
            }
            else
            {
                CurrencyLogger.Error("{component} {name} {account} {ip} {currency} {action} {amount} {source} {extra}",
                    entry.Component,
                    entry.Character?.CharacterName,
                    entry.Character?.Account.EMailAddress,
                    entry.Character?.Account.LastIP,
                    Functions.GetEnumDescription(entry.Action),
                    entry.Amount,
                    Functions.GetEnumDescription(entry.Source),
                    entry.ExtraInfo);
            }
        }

        #endregion

        /// <summary>
        /// 调整奖金池余额 可正可负
        /// 造成档位变化时 会更新全服的buff
        /// </summary>
        /// <param name="amount"></param>
        public static void RewardPoolAdjustBalance(decimal amount)
        {
            var oldTier = TheRewardPoolInfo.CurrentTier;

            TheRewardPoolInfo.Balance += amount;
            TheRewardPoolInfo.Balance = Math.Min(TheRewardPoolInfo.Balance, TheRewardPoolInfo.MaxAmount);

            if (TheRewardPoolInfo.LastSendUpdateTime + TimeSpan.FromSeconds(Config.RewardPoolUpdateDelay) < Now)
            {
                TheRewardPoolInfo.BroadcastRewardPoolUpdate();
                //TheRewardPoolInfo.LastSendUpdateTime = Now;
            }

            if (oldTier != TheRewardPoolInfo.CurrentTier)
            {
                // 更新全服buff
                RewardPoolReloadeBuffs();
            }
        }
        /// <summary>
        /// 玩家充值 增加奖金池余额
        /// </summary>
        /// <param name="character"></param>
        /// <param name="amount"></param>
        public static void RewardPoolAddBalance(CharacterInfo character, int amount, string source)
        {
            if (!Config.EnableRewardPool) return;

            try
            {
                // 构造参数
                // 充值者信息
                var input1 = new Utils.RewardPool.RewardPoolAddBalanceParams
                {
                    角色 = character,
                    数额 = amount,
                    来源 = source,
                };
                // 池子信息
                var input2 = Utils.RewardPool.RewardPoolRuleUtils.GetRewardPoolDetails();

                // 传给规则引擎
                var resultList = GlobalRulesEngine.ExecuteAllRulesAsync(Config.RewardPoolAddBalanceFileName, input1, input2).Result;

                // 解析结果
                foreach (var result in resultList)
                {
                    if (result.IsSuccess)
                    {
                        // 成功
                        var amountAdded = Convert.ToDecimal(result.ActionResult.Output);
                        RewardPoolAdjustBalance(amountAdded);
                        Log($"玩家 {character.CharacterName} 成功为奖金池添加 {amountAdded}");
                    }
                }

            }
            catch (Exception e)
            {
                Log("奖金池玩法: 添加奖金发生错误");
                Log(e.ToString());
            }
        }
        /// <summary>
        /// 重载奖金池的buff
        /// </summary>
        public static void RewardPoolReloadeBuffs()
        {
            RewardPoolBuffs.Clear();
            try
            {
                // 池子信息
                var input1 = Utils.RewardPool.RewardPoolRuleUtils.GetRewardPoolDetails();
                // 传给规则引擎
                var resultList = GlobalRulesEngine.ExecuteAllRulesAsync(Config.RewardPoolUpdateBuffFileName, input1).Result;
                // 成功的规则会触发RewardPollAddBuff() 所以这里不用再执行别的了
                // 标记buff需要刷新
                RewardPoolBuffChanged = true;
            }
            catch (Exception e)
            {
                Log("奖金池玩法: 更新BUFF发生错误");
                Log(e.ToString());
            }
        }
        /// <summary>
        /// 添加奖金池buff
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static bool RewardPollAddBuff(Stat stat, int amount)
        {
            RewardPoolBuffs.Add(stat, amount);
            return true;
        }
        /// <summary>
        /// 调整个人彩池币余额
        /// </summary>
        /// <param name="character">玩家角色信息 在线与否无所谓</param>
        /// <param name="amount">可正可负</param>
        public static void AdjustPersonalReward(CharacterInfo character, decimal amount,
            CurrencySource source = CurrencySource.Undefined, string extraMsg = null)
        {
            if (character == null)
            {
                Log("添加彩池币出错: 角色为null");
                return;
            }
            character.RewardPoolCoin += amount;

            // 玩家在线则发包通知
            if (character.Account?.Connection != null)
            {
                if (amount > 0.01M)
                {
                    // 这里不再单独发通知
                    //character.Account.Connection.ReceiveChat($"您的忠诚度发生变化: {amount:0.00}, 当前忠诚度: {character.RewardPoolCoin}", MessageType.System);
                    // 但是依然要告诉客户端 玩家当前账上有多少忠诚度
                    character.Account.Connection.Enqueue(new S.RewardPoolCoinChanged { RewardPoolCoin = character.RewardPoolCoin });
                }
            }

            // 记录
            switch (source)
            {
                case CurrencySource.Undefined:
                    break;

                // 获得
                case CurrencySource.GMAdd:
                case CurrencySource.TopUp:
                case CurrencySource.ItemAdd:
                case CurrencySource.PickUpAdd:
                case CurrencySource.ScriptAdd:
                case CurrencySource.TradeAdd:
                case CurrencySource.EventAdd:
                case CurrencySource.RedeemAdd:
                case CurrencySource.KillMobAdd:
                case CurrencySource.RedPacketAdd:
                case CurrencySource.OtherAdd:
                    // 累计获得
                    character.TotalRewardPoolCoinEarned += Math.Abs(amount);
                    break;

                // 失去
                case CurrencySource.MarketplaceDeduct:
                    break;
                case CurrencySource.GMDeduct:
                    break;
                case CurrencySource.ItemDeduct:
                    break;
                case CurrencySource.DropDeduct:
                    break;
                case CurrencySource.ScriptDeduct:
                    break;
                case CurrencySource.TradeDeduct:
                    break;
                case CurrencySource.EventDeduct:
                    break;
                case CurrencySource.RedeemDeduct:
                    break;
                case CurrencySource.KillMobDeduct:
                    break;
                case CurrencySource.RedPacketDeduct:
                    break;
                case CurrencySource.CashOutDeduct:
                    // 累计提现
                    character.TotalRewardPoolCoinCashedOut += Math.Abs(amount);
                    break;
                case CurrencySource.SystemDeduct:
                    break;
                case CurrencySource.OtherDeduct:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }

            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry(
                logLevel: LogLevel.Info,
                component: "奖金池系统",
                time: Now,
                character: character,
                currency: CurrencyType.RewardPoolCurrency,
                action: amount < 0 ? CurrencyAction.Deduct : CurrencyAction.Add,
                source: source,
                amount: amount,
                extraInfo: extraMsg);
            // 存入日志
            LogToViewAndCSV(logEntry);
            // 更新排行榜
            if (LastRPRankUpdateTime + TimeSpan.FromSeconds(Config.RewardPoolRankUpdateDelay) < Now)
            {
                RefreshRewardPoolRanks();
            }
        }
        /// <summary>
        /// 调整个人彩池币余额
        /// </summary>
        /// <param name="characterName"></param>
        /// <param name="amount"></param>
        public static void AdjustPersonalReward(string characterName, decimal amount,
            CurrencySource source = CurrencySource.Undefined, string extraMsg = null)
        {
            AdjustPersonalReward(GetCharacter(characterName), amount, source, extraMsg);
        }
        /// <summary>
        /// 刷新排行榜
        /// </summary>
        public static void RefreshRewardPoolRanks()
        {
            TotalRPCoinEarnedRanks = CharacterInfoList.Binding.OrderByDescending(x => x.TotalRewardPoolCoinEarned).ToList();
            //TotalRPCoinCashedOutRanks = CharacterInfoList.Binding.OrderByDescending(x => x.TotalRewardPoolCoinCashedOut).ToList();
            LastRPRankUpdateTime = Now;

            // 刷新前三
            if (TotalRPCoinEarnedRanks.Count > 0)
            {
                TotalRPCoinEarnedTop1 = GetRPRanksOfCharacter(TotalRPCoinEarnedRanks[0]);
            }
            if (TotalRPCoinEarnedRanks.Count > 1)
            {
                TotalRPCoinEarnedTop2 = GetRPRanksOfCharacter(TotalRPCoinEarnedRanks[1]);
            }
            if (TotalRPCoinEarnedRanks.Count > 2)
            {
                TotalRPCoinEarnedTop3 = GetRPRanksOfCharacter(TotalRPCoinEarnedRanks[2]);
            }

        }
        /// <summary>
        /// 获取指定角色的奖池币排名信息
        /// </summary>
        /// <param name="info">角色</param>
        /// <returns></returns>
        public static ClientRewardPoolRanks GetRPRanksOfCharacter(CharacterInfo info)
        {
            if (info == null)
            {
                Log("GetRPRanksOfCharacter出错：info为null");
                return null;
            }

            return new ClientRewardPoolRanks
            {
                Name = info.CharacterName,
                TotalEarned = info.TotalRewardPoolCoinEarned,
                TotalEarnedRank = TotalRPCoinEarnedRanks.FindIndex(x => x.Index == info.Index) + 1,
                TotalCashedOut = info.TotalRewardPoolCoinCashedOut
            };
        }

        #endregion

        #region 红包

        /// <summary>
        /// 创建一个红包并返回
        /// </summary>
        /// <param name="sender">发送者角色</param>
        /// <param name="value">面额</param>
        /// <param name="count">数目</param>
        /// <param name="type">类型</param>
        /// <param name="scope">发送范围</param>
        /// <param name="durationInSec">多少秒内可以领</param>
        /// <returns></returns>
        public static RedPacketInfo CreateRedPacket(CharacterInfo sender, decimal value, int count,
            CurrencyType currency = CurrencyType.RewardPoolCurrency,
            RedPacketType type = RedPacketType.Randomly,
            RedPacketScope scope = RedPacketScope.Server,
            string message = "",
            int durationInSec = 600)
        {
            if (Config.EnableRedPacket)
            {
                var redPacket = RedPacketInfoList.CreateNewObject();
                redPacket.Sender = sender;
                redPacket.FaceValue = value;
                redPacket.TotalCount = count;
                redPacket.Currency = currency;
                redPacket.Type = type;
                redPacket.Scope = scope;
                redPacket.RemainingValue = value;
                redPacket.SendTime = Now;
                redPacket.ExpireTime = Now + TimeSpan.FromSeconds(durationInSec);
                if (!string.IsNullOrEmpty(message))
                {
                    redPacket.Message = message;
                }
                return redPacket;
            }
            else
            {
                Log("创建红包失败：功能开关处于关闭状态");
                return null;
            }
        }

        /// <summary>
        /// 广播单个红包信息
        /// </summary>
        /// <param name="redPacket"></param>
        public static void BroadcastRedPacket(RedPacketInfo redPacket)
        {
            if (redPacket == null) return;

            Broadcast(new S.RedPacketUpdate { RedPacket = redPacket.ToClientInfo() });
        }

        /// <summary>
        /// 广播1天内发出的红包信息
        /// </summary> todo
        public static void BroadcastRecentRedPacketsUpdate()
        {
            Broadcast(new S.RecentRedPackets
            {
                RedPacketList = RedPacketInfoList.Binding.Where(
                    x => x.SendTime + TimeSpan.FromDays(1) > Now).Select(
                    y => y.ToClientInfo()).ToList()
            });
        }

        #endregion

        #region 反外挂

        // 目前只能依靠客户端上报的进程指纹信息（即进程文件的sha256值）来判断外挂

        /// <summary>
        /// 上次请求的时间
        /// </summary>
        public static DateTime NextProcessHashRequestTime { get; set; } = DateTime.MinValue;
        /// <summary>
        /// 是否应该发送请求
        /// </summary>
        public static bool ShouldRequestProcessHash => Now > NextProcessHashRequestTime;
        /// <summary>
        /// 向全体在线玩家  要求回传进程信息
        /// </summary>
        public static void BroadcastRequestProcessHash()
        {
            Broadcast(new S.RequestProcessHash());
            NextProcessHashRequestTime = Now + TimeSpan.FromSeconds(Config.RequestProcessHashDelay);
        }

        /// <summary>
        /// 向某个玩家发送请求 要求回传进程信息
        /// </summary>
        /// <param name="con"></param>
        public static void RequestProcessHash(SConnection con)
        {
            con?.Enqueue(new S.RequestProcessHash());
        }

        /// <summary>
        /// 加载封禁进程的列表
        /// </summary>
        public static void LoadSha256ProcessBanList()
        {
            BanProcessList = new List<string>();
            string path = Path.Combine(AppContext.BaseDirectory, "Database", "BanProcess.txt");

            if (!File.Exists(path))
            {
                Log("BanProcess文件不存在。如有需要请在Database文件夹下建立BanProcess.txt");
            }
            else
            {
                var temp = File.ReadAllLines(path, Encoding.Default);

                BanProcessList.AddRange(temp.Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith("#")));
                Log($"已加载{BanProcessList.Count}条反挂特征码");
            }
        }

        /// <summary>
        /// 判断进程哈希值是否在禁止列表
        /// </summary>
        /// <param name="hashValue">true=该哈希值被封禁</param>
        /// <returns></returns>
        public static bool CheckProcessSha256InBanList(string hashValue)
        {
            return BanProcessList.Any(x => x.Equals(hashValue, StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

        /// <summary>
        /// 启动服务器
        /// </summary>
        public static void StartServer()
        {
#if !DEBUG
            if (!LicenseValid)
            {
                Log("授权无效，无法启动服务器");
                return;
            }
            else
            {
                Log($"授权状态: {LicenseState}");
            }
#endif
            if (OriginalLoop)
            {
                //如果已启动，返回
                if (Started || EnvirThread != null) return;

                EnvirThread = new Thread(EnvirLoop) { IsBackground = true };
                // 测试：给此线程最高优先级
                EnvirThread.Priority = ThreadPriority.Highest;
                EnvirThread.Start();
            }
            else
            {
                //如果已启动，返回
                if (Started) return;
                Starting = true;
                InitializeServer();
            }

            //记录所有未单独处理的异常
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);

            if (null == watcher)
            {
                watcher = new FileSystemWatcher();
                watcher.Path = Path.Combine(AppContext.BaseDirectory, _scriptPath);
                watcher.IncludeSubdirectories = true;
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.py";
                watcher.Changed += new FileSystemEventHandler(OnPyChanged);
                watcher.Created += new FileSystemEventHandler(OnPyChanged);
                watcher.Deleted += new FileSystemEventHandler(OnPyChanged);
                watcher.Renamed += new RenamedEventHandler(OnPyRenamed);
                watcher.EnableRaisingEvents = true;
            }

            if (null == workflowWatcher)
            {
                workflowWatcher = new FileSystemWatcher();
                workflowWatcher.Path = Path.Combine(AppContext.BaseDirectory, workflowsPath);
                workflowWatcher.IncludeSubdirectories = true;
                workflowWatcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                workflowWatcher.Filter = "*.json";
                workflowWatcher.Changed += new FileSystemEventHandler(OnWorkflowChanged);
                workflowWatcher.Created += new FileSystemEventHandler(OnWorkflowChanged);
                workflowWatcher.Deleted += new FileSystemEventHandler(OnWorkflowChanged);
                workflowWatcher.Renamed += new RenamedEventHandler(OnWorkflowRenamed);
                workflowWatcher.EnableRaisingEvents = true;
            }

        }

        /// <summary>
        /// BuyGameGold配置
        /// </summary>
        public static void LoadBuyGameGoldInfo()
        {
            string path = @".\Database\";

            if (!File.Exists(path + "BuyGameGold.txt"))
            {
                Log(string.Format("BuyGameGold配置文件不存在！"));
                return;
            }
            BuyGameGold = File.ReadAllText(path + "BuyGameGold.txt", Encoding.Default);
        }

        /// <summary>
        /// 上次读取日期时间
        /// </summary>
        public static DateTime lastRead = DateTime.MinValue;
        /// <summary>
        /// PY脚本更改时
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        static void OnPyChanged(object source, FileSystemEventArgs e)
        {
            try
            {
                watcher.EnableRaisingEvents = false;
                DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
                if (lastWriteTime != lastRead)
                {
                    LoadScripts();
                }
                lastRead = lastWriteTime;
            }
            finally
            {
                watcher.EnableRaisingEvents = true;
            }
        }
        /// <summary>
        /// 对PY脚本重命名
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        static void OnPyRenamed(object source, RenamedEventArgs e)
        {
            LoadScripts();
        }
        /// <summary>
        /// 加载数据库
        /// </summary>
        private static void LoadDatabase()
        {
            Random = new Random();

            var assemblies = new Assembly[]
           {
                Assembly.GetAssembly(typeof(ItemInfo)),
               Assembly.GetAssembly(typeof(AccountInfo))
           };

            Session = new Session(SessionMode.Server, assemblies, Config.EnableDBEncryption, Config.DBPassword)
            {
                BackUpDelay = 60,
                IsMySql = Config.MySqlEnable,
                MySqlParms = new string[] { Config.MySqlUser, Config.MySqlPassword, Config.MySqlIP, Config.MySqlPort, },
            };
            Session.Output += (o, e) =>
            {
                Log(e.ToString());
            };
            Session.Init();
            //预处理脏数据
            PreClearData();

            //加载详细的数据库对应列表
            LangInfoList = Session.GetCollection<LangInfo>();
            MapInfoList = Session.GetCollection<MapInfo>();
            SafeZoneInfoList = Session.GetCollection<SafeZoneInfo>();
            ItemInfoList = Session.GetCollection<ItemInfo>();
            MonsterInfoList = Session.GetCollection<MonsterInfo>();
            RespawnInfoList = Session.GetCollection<RespawnInfo>();
            MagicInfoList = Session.GetCollection<MagicInfo>();
            SortingList = Session.GetCollection<UserSorting>();
            SortingLevList = Session.GetCollection<UserSortingLev>();
            AccountInfoList = Session.GetCollection<AccountInfo>();
            FriendsList = Session.GetCollection<FriendInfo>();
            CharacterInfoList = Session.GetCollection<CharacterInfo>();
            BeltLinkList = Session.GetCollection<CharacterBeltLink>();
            AutoPotionLinkList = Session.GetCollection<AutoPotionLink>();
            AutoFightConfList = Session.GetCollection<AutoFightConfig>();
            UserItemList = Session.GetCollection<UserItem>();
            AllOwnerlessItemList = UserItemList?.Binding?.Where(x => x.CanBuyback && x.IsOwnerless && x.OwnerLessTime.AddDays(2) > SEnvir.Now).Reverse().ToList();
            UserItemStatsList = Session.GetCollection<UserItemStat>();
            RefineInfoList = Session.GetCollection<RefineInfo>();
            UserMagicList = Session.GetCollection<UserMagic>();
            CharacterShopList = Session.GetCollection<CharacterShop>();
            BuffInfoList = Session.GetCollection<BuffInfo>();
            AuctionInfoList = Session.GetCollection<AuctionInfo>();
            MailInfoList = Session.GetCollection<MailInfo>();
            AuctionHistoryInfoList = Session.GetCollection<AuctionHistoryInfo>();
            UserDropList = Session.GetCollection<UserDrop>();
            StoreInfoList = Session.GetCollection<StoreInfo>();
            BaseStatList = Session.GetCollection<BaseStat>();
            MovementInfoList = Session.GetCollection<MovementInfo>();
            NPCInfoList = Session.GetCollection<NPCInfo>();
            MapRegionList = Session.GetCollection<MapRegion>();
            GuildInfoList = Session.GetCollection<GuildInfo>();
            GuildMemberInfoList = Session.GetCollection<GuildMemberInfo>();
            UserQuestList = Session.GetCollection<UserQuest>();
            UserQuestTaskList = Session.GetCollection<UserQuestTask>();
            CompanionSkillInfoList = Session.GetCollection<CompanionSkillInfo>();

            CompanionInfoList = Session.GetCollection<CompanionInfo>();
            CompanionLevelInfoList = Session.GetCollection<CompanionLevelInfo>();
            UserCompanionList = Session.GetCollection<UserCompanion>();
            UserCompanionUnlockList = Session.GetCollection<UserCompanionUnlock>();
            BlockInfoList = Session.GetCollection<BlockInfo>();
            CastleInfoList = Session.GetCollection<CastleInfo>();
            UserConquestList = Session.GetCollection<UserConquest>();
            GameGoldPaymentList = Session.GetCollection<GameGoldPayment>();
            GameStoreSaleList = Session.GetCollection<GameStoreSale>();
            GuildWarInfoList = Session.GetCollection<GuildWarInfo>();
            GuildAllianceInfoList = Session.GetCollection<GuildAllianceInfo>();
            foreach (var ob in GuildAllianceInfoList)      //清理行会联盟，后续如需使用注释这两行代码
                GuildAllianceInfoList.Delete(ob);
            UserConquestStatsList = Session.GetCollection<UserConquestStats>();
            UserFortuneInfoList = Session.GetCollection<UserFortuneInfo>();
            WeaponCraftStatInfoList = Session.GetCollection<WeaponCraftStatInfo>();
            FixedPointInfoList = Session.GetCollection<FixedPointInfo>();
            UserValueList = Session.GetCollection<UserValue>();
            GolbleValueList = Session.GetCollection<GolbleValue>();
            CraftItemInfoList = Session.GetCollection<CraftItemInfo>();
            TriggerInfoList = Session.GetCollection<TriggerInfo>();
            FishingAreaInfoList = Session.GetCollection<FishingAreaInfo>();
            DiyMagicEffectList = Session.GetCollection<DiyMagicEffect>();
            MonDiyActionList = Session.GetCollection<MonDiyAiAction>();
            MonAnimationFrameList = Session.GetCollection<MonAnimationFrame>();
            GameGoldSetInfo = Session.GetCollection<GameGoldSet>();
            NewAutionInfoList = Session.GetCollection<NewAutionInfo>();

            GuildLevelExpList = Session.GetCollection<GuildLevelExp>();
            CastleFundInfoList = Session.GetCollection<CastleFundInfo>();
            RewardPoolInfoList = Session.GetCollection<RewardPoolInfo>();
            GoldMarketInfoList = Session.GetCollection<GoldMarketInfo>();

            GuildFundChangeInfoList = Session.GetCollection<GuildFundChangeInfo>();

            TheRewardPoolInfo = RewardPoolInfoList.Binding.FirstOrDefault();

            if (Config.EnableRewardPool && RewardPoolInfoList.Count < 1)
            {
                // 创建一个默认的奖池
                RewardPoolInfo pool = RewardPoolInfoList.CreateNewObject();
                pool.CurrencyName = "彩池币";
                pool.MaxAmount = 1000000;
                pool.Balance = 0;
                pool.Tier1UpperLimit = 100;
                pool.Tier2UpperLimit = 200;
                pool.Tier3UpperLimit = 500;
                pool.Tier4UpperLimit = 1000;
                pool.Tier5UpperLimit = 5000;
            }

            // 红包
            RedPacketInfoList = Session.GetCollection<RedPacketInfo>();
            RedPacketClaimInfoList = Session.GetCollection<RedPacketClaimInfo>();

            if (Globals.GamePlayEXPInfoList == null)
                GamePlayEXPInfoList = Session.GetCollection<PlayerExpInfo>();

            if (Globals.GameWeaponEXPInfoList == null)
                Globals.GameWeaponEXPInfoList = Session.GetCollection<WeaponExpInfo>();

            if (Globals.GameAccessoryEXPInfoList == null)
                Globals.GameAccessoryEXPInfoList = Session.GetCollection<AccessoryExpInfo>();

            if (Globals.GameCraftExpInfoList == null)
            {
                Globals.GameCraftExpInfoList = Session.GetCollection<CraftExpInfo>();
                foreach (CraftExpInfo expInfo in Globals.GameCraftExpInfoList.Binding)
                {
                    Globals.CraftExpDict[expInfo.Level] = expInfo.Exp;
                }
            }

            if (Globals.AchievementInfoList == null)
                Globals.AchievementInfoList = Session.GetCollection<AchievementInfo>();

            if (Globals.AchievementRequirementList == null)
                Globals.AchievementRequirementList = Session.GetCollection<AchievementRequirement>();

            if (Globals.QuestInfoList == null)
                Globals.QuestInfoList = Session.GetCollection<QuestInfo>();

            if (Globals.CustomBuffInfoList == null)
            {
                Globals.CustomBuffInfoList = Session.GetCollection<CustomBuffInfo>();
                foreach (CustomBuffInfo buff in Globals.CustomBuffInfoList.Binding)
                {
                    string groupName = buff.BuffGroup?.Trim();
                    if (string.IsNullOrEmpty(groupName)) continue;

                    if (!Globals.CustomBuffGroupsDict.ContainsKey(groupName))
                    {
                        Globals.CustomBuffGroupsDict[groupName] = new HashSet<int>();
                    }
                    Globals.CustomBuffGroupsDict[groupName].Add(buff.Index);
                }
            }

            if (Globals.WeaponUpgradeInfoList == null)
                Globals.WeaponUpgradeInfoList = Session.GetCollection<WeaponUpgradeNew>();

            if (Globals.SetInfoList == null)
                Globals.SetInfoList = Session.GetCollection<SetInfo>();

            if (Globals.SetGroupList == null)
                Globals.SetGroupList = Session.GetCollection<SetGroup>();

            if (Globals.SetGroupItemList == null)
                Globals.SetGroupItemList = Session.GetCollection<SetGroupItem>();

            if (Globals.SetInfoStatList == null)
                Globals.SetInfoStatList = Session.GetCollection<SetInfoStat>();

            UserAchievementList = Session.GetCollection<UserAchievement>();
            UserAchievementRequirementList = Session.GetCollection<UserAchievementRequirement>();

            //构造一个字典 便于更新成就进度
            //todo 暂时没用 如果性能损失大就用
            /*
            UserAchievementRequirementDict = new ConcurrentDictionary<AchievementRequirementType, List<UserAchievementRequirement>>();
            foreach (var t in Enum.GetValues(typeof(AchievementRequirementType)))
            {
                AchievementRequirementType requirementType = (AchievementRequirementType)t;
                UserAchievementRequirementDict[requirementType] = UserAchievementRequirementList.Binding
                    .Where(x => x.Requirement.RequirementType == requirementType).ToList();
            }
            */

            GoldInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.Gold);
            GameGoldInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.GameGold);
            PrestigeInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.Prestige);
            ContributeInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.Contribute);
            RefinementStoneInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.RefinementStone);
            FragmentInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.Fragment1);
            Fragment2Info = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.Fragment2);
            Fragment3Info = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.Fragment3);

            ItemPartInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.ItemPart);
            FortuneCheckerInfo = ItemInfoList.Binding.First(x => x.Effect == ItemEffect.FortuneChecker);


            MysteryShipMapRegion = MapRegionList.Binding.FirstOrDefault(x => x.Index == Config.MysteryShipRegionIndex);
            LairMapRegion = MapRegionList.Binding.FirstOrDefault(x => x.Index == Config.LairRegionIndex);
            RightDeliver = MapRegionList.Binding.FirstOrDefault(x => x.Index == Config.RightDeliver);
            ErrorDeliver = MapRegionList.Binding.FirstOrDefault(x => x.Index == Config.ErrorDeliver);
            StarterGuild = GuildInfoList.Binding.FirstOrDefault(x => x.StarterGuild);

            FishList = ItemInfoList.Binding.Where(x => x.Stats != null && x.Stats[Stat.FishObtainChance] > 0).ToList();

            //boss信息
            foreach (var info in MonsterInfoList.Binding)
            {
                if (info.IsBoss)
                {
                    BossTrackerList.Add(new BossTracker
                    {
                        BossInfo = info,
                        LastKiller = null,
                        LastKillTime = DateTime.MinValue,
                    });
                }
            }

            if (StarterGuild == null)
            {
                StarterGuild = GuildInfoList.CreateNewObject();
                StarterGuild.StarterGuild = true;
            }

            StarterGuild.GuildName = Globals.StarterGuildName;

            #region Create Ranks
            Rankings = new LinkedList<CharacterInfo>();
            TopRankings = new HashSet<CharacterInfo>();
            foreach (CharacterInfo info in CharacterInfoList.Binding)
            {
                info.RankingNode = Rankings.AddLast(info);
                RankingSort(info, false);
            }
            UpdateLead();
            #endregion

            TreaItemList.Clear();
            for (int i = 0; i < ItemInfoList.Count; i++)
            {

                if (ItemInfoList[i].CanTreasure)
                    TreaItemList.Add(ItemInfoList[i]);
            }

            for (int i = UserQuestList.Count - 1; i >= 0; i--)
                if (UserQuestList[i].QuestInfo == null)
                    UserQuestList[i].Delete();

            for (int i = UserQuestTaskList.Count - 1; i >= 0; i--)
                if (UserQuestTaskList[i].Task == null)
                    UserQuestTaskList[i].Delete();

            for (int i = UserAchievementList.Count - 1; i >= 0; i--)
            {
                if (UserAchievementList[i].AchievementName == null ||
                    !Globals.AchievementInfoList.Binding.Contains(UserAchievementList[i].AchievementName))
                {
                    UserAchievementList[i].Delete();
                }
            }

            for (int i = UserAchievementRequirementList.Count - 1; i >= 0; i--)
            {
                if (UserAchievementRequirementList[i].Requirement == null ||
                    (Globals.AchievementRequirementList != null && !Globals.AchievementRequirementList.Binding.Contains(UserAchievementRequirementList[i].Requirement)))
                {
                    UserAchievementRequirementList[i].Requirement.Delete();
                }
            }

            BossList.Clear();
            foreach (MonsterInfo monster in MonsterInfoList.Binding)
            {
                if (!monster.IsBoss) continue;
                if (monster.Drops.Count == 0) continue;

                BossList.Add(monster);

            }

            Messages = new ConcurrentQueue<IPNMessage>();

            PaymentList.Clear();

            if (Directory.Exists(VerifiedPath))
            {
                string[] files = Directory.GetFiles(VerifiedPath);

                foreach (string file in files)
                    Messages.Enqueue(new IPNMessage { FileName = file, Message = File.ReadAllText(file), Verified = true });
            }

            foreach (var castle in CastleInfoList.Binding)
            {
                //过期申请攻城信息清除
                var list = UserConquestList.Binding.Where(p => (p.WarDate.Date < Now.Date || p.WarDate.Date == Now.Date && Now.TimeOfDay > p.Castle.Duration) && p.Castle == castle).ToList();
                foreach (var conquest in list)
                {
                    conquest.Delete();
                }
            }
            foreach (GolbleValue uvalue in GolbleValueList)
                Values[uvalue.Key] = uvalue;
            //国际化
            Server.LangEx.Init();
        }

        /// <summary>
        /// 处理脏数据
        /// </summary>
        private static void PreClearData()
        {
            //删除行会空账号
            var list = Session.GetCollection<GuildMemberInfo>();
            for (var i = 0; i < list.Count; i++)
            {
                var info = list[i];
                if (info.Account == null)
                {
                    info.Delete();
                }
            }

            //删除角色空道具或者数据库不存在的道具
            var items = Session.GetCollection<UserItem>();
            for (var i = 0; i < items.Count; i++)
            {
                var info = items[i];
                if (info?.Info == null)
                {
                    info?.Delete();
                }
            }

            //宠物删除为空，对应删除角色信息
            var companions = Session.GetCollection<UserCompanion>();
            for (var i = 0; i < companions.Count; i++)
            {
                var info = companions[i];
                if (info?.Info == null)
                {
                    info?.Delete();
                }
            }
        }

        /// <summary>
        /// 增加转生的排行更新
        /// </summary>
        /// <param name="character"></param>
        /// <param name="updateLead"></param>
        public static void RankingSort(CharacterInfo character, bool updateLead = true)  //排行榜排序
        {
            bool changed = false;

            LinkedListNode<CharacterInfo> node;
            while ((node = character.RankingNode.Previous) != null)
            {
                int nodelevel = node.Value.Level + 5000 * node.Value.Rebirth;
                int characterlevel = character.Level + 5000 * character.Rebirth;

                if (nodelevel > characterlevel) break;
                if (nodelevel == characterlevel && node.Value.Experience >= character.Experience) break;

                changed = true;

                Rankings.Remove(character.RankingNode);
                Rankings.AddBefore(node, character.RankingNode);
            }

            if (!updateLead || (TopRankings.Count >= 9 && !changed)) return; //5 * 4  顶尊称号的人数最多9人，各职业前3

            UpdateLead();

            //排行榜更新
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerRankingChange(EventTypes.PlayerRankingChange,
                    new PlayerRankingChangeEventArgs()));
        }

        /// <summary>
        /// 更新加载
        /// </summary>
        public static void UpdateLead()
        {
            HashSet<CharacterInfo> newTopRankings = new HashSet<CharacterInfo>();

            int war = 5, wiz = 5, tao = 5, ass = 5;

            foreach (CharacterInfo cInfo in Rankings)
            {
                if (cInfo.Account.Admin) continue;

                switch (cInfo.Class)
                {
                    case MirClass.Warrior:
                        if (war == 0) continue;
                        war--;
                        newTopRankings.Add(cInfo);
                        break;
                    case MirClass.Wizard:
                        if (wiz == 0) continue;
                        wiz--;
                        newTopRankings.Add(cInfo);
                        break;
                    case MirClass.Taoist:
                        if (tao == 0) continue;
                        tao--;
                        newTopRankings.Add(cInfo);
                        break;
                    case MirClass.Assassin:
                        if (ass == 0) continue;
                        ass--;
                        newTopRankings.Add(cInfo);
                        break;
                }

                if (war == 0 && wiz == 0 && tao == 0 && ass == 0) break;
            }

            foreach (CharacterInfo info in TopRankings)
            {
                if (newTopRankings.Contains(info)) continue;

                info.Player?.BuffRemove(BuffType.Ranking);
            }

            foreach (CharacterInfo info in newTopRankings)
            {
                if (TopRankings.Contains(info)) continue;

                info.Player?.BuffAdd(BuffType.Ranking, TimeSpan.MaxValue, null, true, false, TimeSpan.Zero);
            }

            TopRankings = newTopRankings;
        }

        /// <summary>
        /// 起始发送读取数据库
        /// </summary>
        //private static void StartEnvir()
        //{
        //    DateTime start = DateTime.Now;
        //    Log("加载数据库");
        //    try
        //    {
        //        LoadDatabase();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log($"[数据库错误] 加载数据库异常." + ex.Message + ex.StackTrace);
        //    }

        //    #region 加载文件

        //    for (int i = 0; i < MapInfoList.Count; i++)
        //        if (!MapInfoList[i].IsDynamic)//添加是否为副本的判断
        //            Maps[MapInfoList[i]] = new Map(MapInfoList[i]);
        //    Log("加载地图");
        //    Parallel.ForEach(Maps, x => x.Value.Load());

        //    #endregion

        //    foreach (Map map in Maps.Values)
        //        map.Setup();

        //    Parallel.ForEach(MapRegionList.Binding, x =>
        //    {
        //        Map map = GetMap(x.Map);

        //        if (map == null) return;

        //        x.CreatePoints(map.Width);
        //    });
        //    Log("加载安全区");
        //    CreateSafeZones();
        //    Log("加载钓鱼区");
        //    CreateFishingArea();
        //    Log("加载地图链接");
        //    CreateMovements();
        //    Log("加载NPC");
        //    CreateNPCs();
        //    Log("加载复活点");
        //    CreateSpawns();
        //    TimeSpan elapsed = DateTime.Now - start;
        //    Log($"加载数据库成功");
        //}

        public static void LoadBanIpInfo()
        {
            string path = @".\Database\";

            if (!File.Exists(path + "BanCPU.txt"))
            {
                Log("BanCPU文件不存在！");
            }
            else BanCPUList = File.ReadAllLines(path + "BanCPU.txt", Encoding.Default);

            if (!File.Exists(path + "BanHDD.txt"))
            {
                Log("BanHDD文件不存在！");
            }
            else BanHDDList = File.ReadAllLines(path + "BanHDD.txt", Encoding.Default);

            if (!File.Exists(path + "BanMAC.txt"))
            {
                Log("BanMAC文件不存在！");
            }
            else BanMACList = File.ReadAllLines(path + "BanMAC.txt", Encoding.Default);

        }

        public static bool CheckCPUInBanList(string Ip)
        {
            for (int i = 0; i < BanCPUList.Length; i++)
            {
                if (BanCPUList[i] == Ip)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckHDDInBanList(string Ip)
        {
            for (int i = 0; i < BanHDDList.Length; i++)
            {
                if (BanHDDList[i] == Ip)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckMACInBanList(string Ip)
        {
            for (int i = 0; i < BanMACList.Length; i++)
            {
                if (BanMACList[i] == Ip)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建地图移动点
        /// </summary>
        private static void CreateMovements()
        {
            foreach (MovementInfo movement in MovementInfoList.Binding)
            {
                if (movement.SourceRegion == null) continue;

                Map sourceMap = GetMap(movement.SourceRegion.Map);
                if (sourceMap == null)
                {
                    Log($"[地图错误] 错误的地图来源, 来源: {movement.SourceRegion.ServerDescription}");
                    continue;
                }

                if (movement.DestinationRegion == null)
                {
                    Log($"[地图错误] 无目的地地图, 来源: {movement.SourceRegion.ServerDescription}");
                    continue;
                }

                Map destMap = GetMap(movement.DestinationRegion.Map);
                if (destMap == null)
                {
                    Log($"[地图错误] 错误的目的地地图, 指定: {movement.DestinationRegion.ServerDescription}");
                    continue;
                }


                foreach (Point sPoint in movement.SourceRegion.PointList)
                {
                    Cell source = sourceMap.GetCell(sPoint);

                    if (source == null)
                    {
                        Log($"[地图错误] 发生错误, 来源: {movement.SourceRegion.ServerDescription}, X:{sPoint.X}, Y:{sPoint.Y}");
                        continue;
                    }

                    if (source.Movements == null)
                        source.Movements = new List<MovementInfo>();

                    source.Movements.Add(movement);
                }
            }
        }
        /// <summary>
        /// 创建NPC
        /// </summary>
        private static void CreateNPCs()
        {
            foreach (NPCInfo info in NPCInfoList.Binding)
            {
                if (info.Region == null) continue;

                if (info.Display == true) continue;  //隐藏NPC

                Map map = GetMap(info.Region.Map);

                if (map == null)
                {
                    if (info.Region.Map.IsDynamic) continue;
                    Log(string.Format("[NPC错误] 错误地图, NPC: {0}, Map: {1}", info.NPCName, info.Region.ServerDescription));
                    continue;
                }
                try
                {
                    NPCObject ob = new NPCObject
                    {
                        NPCInfo = info,
                    };
                    //ob.LoadScript();

                    if (!ob.Spawn(info.Region))
                        Log($"[NPC错误] 无法生成NPC, 区域: {info.Region.ServerDescription}, NPC: {info.NPCName}");
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }
        /// <summary>
        /// 创建安全区
        /// </summary>
        private static void CreateSafeZones()
        {
            foreach (SafeZoneInfo info in SafeZoneInfoList.Binding)
            {
                if (info.Region == null) continue;

                Map map = GetMap(info.Region.Map);

                if (map == null)
                {
                    Log($"[安全区错误] 错误地图, 地图: {info.Region.ServerDescription}");
                    continue;
                }

                HashSet<Point> edges = new HashSet<Point>();
                HashSet<Point> pointList = new HashSet<Point>(info.Region.PointList);

                foreach (Point point in pointList)
                {
                    Cell cell = map.GetCell(point);

                    if (cell == null)
                    {
                        Log($"[安全区错误] 错误的位置, 区域: {info.Region.ServerDescription}, X: {point.X}, Y: {point.Y}.");

                        continue;
                    }

                    cell.SafeZone = info;

                    for (int i = 0; i < 8; i++)
                    {
                        Point test = Functions.Move(point, (MirDirection)i);

                        if (pointList.Contains(test)) continue;

                        if (map.GetCell(test) == null) continue;

                        edges.Add(test);
                    }
                }

                map.HasSafeZone = true;

                if (Config.ShowSafeZone)   //显示安全区
                {
                    foreach (Point point in edges)
                    {
                        SpellObject ob = new SpellObject
                        {
                            Visible = true,
                            DisplayLocation = point,
                            TickCount = 10,
                            TickFrequency = TimeSpan.FromDays(365),
                            Effect = SpellEffect.SafeZone
                        };

                        ob.Spawn(map.Info, point);
                    }
                }

                if (info.BindRegion == null) continue;

                map = GetMap(info.BindRegion.Map);

                if (map == null)
                {
                    Log($"[安全区错误] 地图光圈错误, 地图: {info.Region.ServerDescription}");
                    continue;
                }

                foreach (Point point in info.BindRegion.PointList)
                {
                    Cell cell = map.GetCell(point);

                    if (cell == null)
                    {
                        Log($"[安全区错误] 错误位置, 区域: {info.BindRegion.ServerDescription}, X: {point.X}, Y: {point.Y}.");
                        continue;
                    }

                    info.ValidBindPoints.Add(point);
                }

            }
        }
        /// <summary>
        /// 创建钓鱼区
        /// </summary>
        private static void CreateFishingArea()
        {
            foreach (FishingAreaInfo info in FishingAreaInfoList.Binding)
            {
                if (info.BindRegion == null) continue;

                Map map = GetMap(info.BindRegion.Map);

                if (map == null || map.Cells == null)
                {
                    Log($"[钓鱼区错误] 错误地图, 地图: {info.BindRegion.ServerDescription}");
                    continue;
                }

                foreach (Point point in info.BindRegion.PointList)
                {
                    if (point.X < 0 || point.X >= map.Width || point.Y < 0 || point.Y >= map.Height)
                    {
                        Log($"[钓鱼区错误] 错误的位置, 区域: {info.BindRegion.ServerDescription}, X: {point.X}, Y: {point.Y}.");

                        continue;
                    }

                    Cell cell = map.GetCell(point);

                    //如果cell不为空，说明是可移动区域，不能是钓鱼区域，返回错误
                    if (cell != null)
                    {
                        Log($"[钓鱼区错误] 错误的位置, 区域: {info.BindRegion.ServerDescription}, X: {point.X}, Y: {point.Y}.");

                        continue;
                    }

                    map.FishingCells[point.X, point.Y] = true;

                }
            }
        }
        /// <summary>
        /// 创建地图怪物复活点
        /// </summary>
        private static void CreateSpawns()
        {
            foreach (RespawnInfo info in RespawnInfoList.Binding)
            {
                if (info.Monster == null) continue;
                // 新刷怪设置 可以Region和map index任选其一

                if (info.Region != null)
                {
                    Map map = GetMap(info.Region.Map);
                    if (map == null)
                    {
                        if (info.Region.Map.IsDynamic) continue;
                        Log($"[怪物刷新点] 地图错误, 地图: {info.Region.ServerDescription}");
                        continue;
                    }
                    Spawns.Add(new SpawnInfo(info, false));
                }
                else if (info.MapID > 0)
                {
                    Map map = GetMap(info.MapID);
                    if (map == null || map.Info.IsDynamic)
                    {
                        Log($"[怪物刷新点] 地图错误, 找不到地图: {info.MapID}");
                        continue;
                    }
                    Spawns.Add(new SpawnInfo(info, true));
                }
                else
                {
                    Log($"[怪物刷新点] 地图错误, 区域和地图序号需至少设置一个, index: {info.Index}");
                    continue;
                }
            }
        }
        /// <summary>
        /// 停止发送
        /// </summary>
        private static void StopEnvir()
        {
            Now = DateTime.MinValue;

            Session = null;

            MapInfoList = null;
            SafeZoneInfoList = null;
            ItemInfoList = null;
            MonsterInfoList = null;
            RespawnInfoList = null;
            MagicInfoList = null;
            SortingList = null;
            SortingLevList = null;
            AccountInfoList = null;
            CharacterInfoList = null;
            BeltLinkList = null;
            AutoPotionLinkList = null;
            AutoFightConfList = null;
            AllOwnerlessItemList = null;
            UserItemList = null;
            UserItemStatsList = null;
            RefineInfoList = null;
            UserMagicList = null;
            BuffInfoList = null;
            GameGoldSetInfo = null;
            AuctionInfoList = null;
            MailInfoList = null;
            AuctionHistoryInfoList = null;
            UserDropList = null;
            StoreInfoList = null;
            BaseStatList = null;
            MovementInfoList = null;
            NPCInfoList = null;
            MapRegionList = null;
            GuildInfoList = null;
            GuildMemberInfoList = null;
            UserQuestList = null;
            UserQuestTaskList = null;
            CompanionSkillInfoList = null;
            GoldMarketInfoList = null;

            CompanionInfoList = null;
            CompanionLevelInfoList = null;
            UserCompanionList = null;
            UserCompanionUnlockList = null;
            BlockInfoList = null;
            CastleInfoList = null;
            UserConquestList = null;
            GameGoldPaymentList = null;
            GameStoreSaleList = null;
            GuildWarInfoList = null;
            GuildAllianceInfoList = null;
            UserConquestStatsList = null;
            UserFortuneInfoList = null;
            WeaponCraftStatInfoList = null;
            FixedPointInfoList = null;
            UserValueList = null;
            GolbleValueList = null;
            CraftItemInfoList = null;
            TriggerInfoList = null;
            FishingAreaInfoList = null;

            GuildLevelExpList = null;

            UserAchievementList = null;
            UserAchievementRequirementList = null;

            RewardPoolInfoList = null;
            RedPacketInfoList = null;

            FishList?.Clear();

            Rankings?.Clear();
            TopRankings?.Clear();

            TreaItemList?.Clear();

            BossList?.Clear();

            PaymentList?.Clear();

            Maps?.Clear();

            Random = null;
            Objects?.Clear();
            ActiveObjects?.Clear();
            Players?.Clear();

            Spawns?.Clear();

            BanMACList = null;
            BanHDDList = null;
            BanCPUList = null;

            _ObjectID = 0;

            EnvirThread = null;
        }

        private static void LoadNotice()
        {
            NoticeList = new List<string>();
            string[] noticearry = new string[10];
            noticearry[0] = Config.Notice0;
            noticearry[1] = Config.Notice1;
            noticearry[2] = Config.Notice2;
            noticearry[3] = Config.Notice3;
            noticearry[4] = Config.Notice4;
            noticearry[5] = Config.Notice5;
            noticearry[6] = Config.Notice6;
            noticearry[7] = Config.Notice7;
            noticearry[8] = Config.Notice8;
            noticearry[9] = Config.Notice9;

            for (int i = 0; i < 10; i++)
            {
                if (string.IsNullOrEmpty(noticearry[i])) continue;
                NoticeList.Add(noticearry[i]);
            }
        }

        #region 新版主循环
        public static void InitializeServer()
        {
            Now = Time.Now;
            DBTime = Now + Config.DBSaveDelay;
            NetworkStarted = false;
            WebServerStarted = false;

            count = 0;
            loopCount = 0;
            nextCount = Now.AddSeconds(1.0);
            UserCountTime = Now.AddMinutes(5.0);
            saveTime = DateTime.MinValue;

            #region 加载公告
            NoticeTime = Now + Config.NoticeDelay;  //公告
            LoadNotice();
            noticeline = 0;
            #endregion

            previousTotalSent = 0L;
            previousTotalReceived = 0L;
            lastindex = 0;
            conDelay = 0L;
            mainDelay = 0;
            processDelay = 0;

            LastWarTime = Now;

            Log($"初始化启动");

            //Thread LogThread = new Thread(WriteLogsLoop) { IsBackground = true };
            //LogThread.Start();

            if (EnvirThread == null || !EnvirThread.IsAlive)
            {
                EnvirThread = new Thread(LoadEnvironment)
                {
                    IsBackground = true
                };
                EnvirThread.Start();
            }
        }

        private static void LoadEnvironment()
        {
            DateTime start = DateTime.Now;
            Log("加载数据库");
            try
            {
                LoadDatabase();
            }
            catch (Exception ex)
            {
                Log($"[数据库错误] 加载数据库异常." + ex.Message + ex.StackTrace);
            }

            #region 加载文件

            for (int i = 0; i < MapInfoList.Count; i++)
                if (!MapInfoList[i].IsDynamic)//添加是否为副本的判断
                    Maps[MapInfoList[i]] = new Map(MapInfoList[i]);
            Log("加载地图");
            Parallel.ForEach(Maps, x => x.Value.Load());

            #endregion

            foreach (Map map in Maps.Values)
                map.Setup();

            Parallel.ForEach(MapRegionList.Binding, x =>
            {
                Map map = GetMap(x.Map);

                if (map == null) return;

                x.CreatePoints(map.Width);
            });
            LoadBuyGameGoldInfo();  //BuyGameGold配置
            CheckGoldTable();       //货币
            LoadBanIpInfo();        //加载黑名单

            LoadSha256ProcessBanList(); // 加载文件封挂

            //关闭服务器自动广告功能 定时器
            SEvirtimer = new System.Timers.Timer();
            SEvirtimer.Enabled = false;
            SEvirtimer.Interval = 1000;
            SEvirtimer.Start();
            SEvirtimer.Elapsed += new System.Timers.ElapsedEventHandler(SEvirtimer_Elapsed);

            Log("加载安全区");
            CreateSafeZones();
            Log("加载钓鱼区");
            CreateFishingArea();
            Log("加载地图链接");
            CreateMovements();
            Log("加载NPC");
            CreateNPCs();
            Log("加载复活点");
            CreateSpawns();
            Log("加载城堡税收");
            InitCastleTaxInfo();
            TimeSpan elapsed = DateTime.Now - start;
            Log($"加载数据库成功");

            InitMainTickLoop();
        }

        private static void InitMainTickLoop()
        {
            StartWebServer();
            LoadScripts();

            int tickInterval = 1000 / Config.EnvirTickCount;
            mainTickTimer = new AccurateTimer(MainTickLoop, tickInterval);

            StartNetwork();

            Started = true;
            Starting = false;
            StartTime = DateTime.Now;
            if (Config.SentryEnabled)
            {
                SentrySdk.CaptureMessage($"{Config.ClientName} 服务端启动. 时间: {StartTime}");
            }
        }

        private static void MainTickLoop()
        {
            if (Stopping)
            {
                if (mainTickTimer != null)
                {
                    mainTickTimer.Stop();
                    mainTickTimer = null;
                }
                StopWebServer();
                StopNetwork();
                while (Saving) Thread.Sleep(1);
                if (Session != null)
                    Session.BackUpDelay = 0;
                Save();
                while (Saving) Thread.Sleep(1);
                StopEnvir();

                Stopping = false;
                Started = false;
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureMessage($"{Config.ClientName} 服务端关闭. 时间: {StartTime}");
                }
                return;
            }

            Now = Time.Now;
            loopCount++;
            try
            {
                #region ConnectionLoop
                SConnection connection;
                while (!NewConnections.IsEmpty)
                {
                    if (!NewConnections.TryDequeue(out connection)) break;

                    IPCount.TryGetValue(connection.IPAddress, out var ipCount);
                    IPCount[connection.IPAddress] = ipCount + 1;

                    Connections.Add(connection);
                }

                long bytesSent = 0;
                long bytesReceived = 0;

                for (int i = Connections.Count - 1; i >= 0; i--)
                {
                    if (i >= Connections.Count) break;

                    connection = Connections[i];

                    SentrySdk.ConfigureScope(s =>
                    {
                        s.SetTag("Account", $"{connection?.Account?.EMailAddress}");
                        s.SetTag("Player", $"{connection?.Player?.Name}");
                        s.SetTag("IP", $"{connection?.IPAddress}");
                    });

                    try
                    {
                        connection?.Process();
                        bytesSent += connection.TotalBytesSent;
                        bytesReceived += connection.TotalBytesReceived;
                    }
                    catch (Exception ex2)
                    {
                        Log(ex2.ToString());
                        Log(ex2.StackTrace);
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureException(ex2);
                        }
                        File.AppendAllText(@"./Errors.txt", ex2.Message + Environment.NewLine);
                        File.AppendAllText(@"./Errors.txt", ex2.StackTrace + Environment.NewLine);
                    }
                }

                //统计连接延迟
                long delay = (Time.Now - Now).Ticks / TimeSpan.TicksPerMillisecond;
                if (delay > conDelay)
                    conDelay = delay;

                //统计连接收发字节数据量
                TotalBytesSent = DBytesSent + bytesSent;
                TotalBytesReceived = DBytesReceived + bytesReceived;
                #endregion

                #region MainTickLoop
                DateTime time = Time.Now;
                for (int i = Players.Count - 1; i >= 0; i--)
                    Players[i].StartProcess();

                if (ServerBuffChanged)
                {
                    for (int i = Players.Count - 1; i >= 0; i--)
                        Players[i].ApplyServerBuff();

                    ServerBuffChanged = false;
                }

                if (EventBuffChanged)
                {
                    for (int i = Players.Count - 1; i >= 0; i--)
                        Players[i].ApplyEventBuff();

                    EventBuffChanged = false;
                }

                if (lastindex < 0) lastindex = ActiveObjects.Count;

                DateTime loopTime = Time.Now.AddMilliseconds(1);
                while (Time.Now <= loopTime)
                {
                    lastindex--;

                    if (lastindex >= ActiveObjects.Count) continue;

                    if (lastindex < 0) break;

                    MapObject ob = ActiveObjects[lastindex];

                    if (ob.Race == ObjectType.Player) continue;

                    try
                    {
                        ob.StartProcess();
                        count++;
                    }
                    catch (Exception ex)
                    {
                        ActiveObjects.Remove(ob);
                        ob.Activated = false;

                        Log(ex.Message);
                        Log(ex.StackTrace);
                        File.AppendAllText(@"./Errors.txt", ex.StackTrace + Environment.NewLine);
                    }
                }
                //统计MainTickLoop延迟
                delay = (Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond;
                if (delay > mainDelay)
                    mainDelay = delay;
                #endregion

                //每秒循环
                #region MainProcessLoop
                PlayerObject.ClearOverMarket(-Config.ClearOverMarket); //清理寄售
                SEnvir.GoldMarketProcess(); //清理金币交易行
                time = Time.Now;
                if (Now >= nextCount)
                {
                    //定时保存数据库
                    if (Now >= DBTime && !Saving)
                    {
                        DBTime = Time.Now + Config.DBSaveDelay;
                        saveTime = Time.Now;

                        Save();

                        SaveDelay = (Time.Now - saveTime).Ticks / TimeSpan.TicksPerMillisecond;
                    }

                    //每秒反馈循环统计信息
                    ProcessObjectCount = count;
                    LoopCount = loopCount;
                    ConDelay = conDelay;
                    MainDelay = mainDelay;
                    ProcessDelay = processDelay;

                    count = 0;
                    loopCount = 0;
                    conDelay = 0;
                    mainDelay = 0;
                    processDelay = 0;

                    DownloadSpeed = TotalBytesReceived - previousTotalReceived;
                    UploadSpeed = TotalBytesSent - previousTotalSent;

                    previousTotalReceived = TotalBytesReceived;
                    previousTotalSent = TotalBytesSent;

                    //发送公告信息
                    if (Now >= NoticeTime)
                    {
                        NoticeTime = Now + Config.NoticeDelay;
                        if (NoticeList.Count > 0)
                        {
                            if (noticeline >= NoticeList.Count) noticeline = 0;

                            foreach (SConnection conn in Connections)
                            {
                                conn.ReceiveChat(NoticeList[noticeline], MessageType.Notice);  //公告颜色
                            }

                            noticeline++;
                        }
                    }

                    //每5分钟发送在线人数信息
                    if (Now >= UserCountTime)
                    {
                        UserCountTime = Now.AddMinutes(Config.UserCountTime);

                        Parallel.ForEach(Connections, conn =>
                        {
                            try
                            {
                                if (Config.UserCount)  //显示在线人数
                                {
                                    if (!Config.AllowObservation)
                                        conn.ReceiveChat("System.OnlineCount1".Lang(conn.Language, Players.Count + Config.UserCountDouble), MessageType.Hint);
                                    else
                                        conn.ReceiveChat("System.OnlineCount".Lang(conn.Language, Players.Count + Config.UserCountDouble), MessageType.Hint); //, Connections.Count(x => x.Stage == GameStage.Observer)
                                }

                                //if (Config.AllowObservation)  //显示观察者信息
                                //{
                                //    switch (conn.Stage)
                                //    {
                                //        case GameStage.Game:
                                //            if (conn.Player.Character.Observable)
                                //                conn.ReceiveChat("System.ObserverCount".Lang(conn.Language, conn.Observers.Count), MessageType.Hint);
                                //            break;
                                //        case GameStage.Observer:
                                //            conn.ReceiveChat("System.ObserverCount".Lang(conn.Language, conn.Observed.Observers.Count), MessageType.Hint);
                                //            break;
                                //    }
                                //}
                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                                Log(ex.StackTrace);
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(ex);
                                }
                            }
                        });
                    }

                    CalculateLights();
                    CheckGuildWars();
                    NewAuctionProcess();

                    #region 处理定时py任务
                    List<ActionScript> runlist = ScriptList.FindAll(x => x.StartTime < Now);
                    DateTime pyloopTime = Time.Now.AddMilliseconds(200);
                    if (runlist.Count > 0)
                    {
                        int listcount = runlist.Count;
                        while (Time.Now < pyloopTime)
                        {
                            listcount--;
                            if (listcount < 0) break;
                            ActionScript scp = runlist[listcount];
                            try
                            {
                                string[] modules = scp.ScriptStr.Split('.');

                                if (modules.Length > 0)
                                {
                                    int strlen = 0;
                                    dynamic ob;
                                    ob = scope.GetVariable(modules[strlen]);
                                    strlen++;
                                    while (strlen < modules.Length)
                                    {
                                        ob = scope.Engine.Operations.GetMember(ob, modules[strlen]);
                                        strlen++;
                                    }
                                    ob(scp.Param);
                                }
                            }
                            catch (SyntaxErrorException e)
                            {

                                string msg = "DelayCall Syntax error : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }
                            catch (SystemExitException e)
                            {

                                string msg = "DelayCall SystemExit : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }
                            catch (Exception ex)
                            {

                                string msg = "DelayCall Error loading plugin : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(ex);
                                SEnvir.Log(string.Format(msg, error));
                                SEnvir.Log(scp.ScriptStr);
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(ex);
                                }
                            }

                            if (scp.Repeat)
                            {
                                //重复执行 更新下次执行的时间
                                scp.StartTime = Now + scp.Interval;
                            }
                            else
                            {
                                //仅执行一次 移除此脚本
                                ScriptList.Remove(scp);
                            }
                        }
                    }
                    #endregion

                    //处理地图
                    try
                    {
                        foreach (KeyValuePair<MapInfo, Map> pair in Maps)
                            pair.Value.Process();

                        for (int i = FubenMaps.Count - 1; i >= 0; i--)
                        {
                            Map fuben = FubenMaps[i];
                            if (fuben.MapTime < Now && fuben.MapTime > DateTime.MinValue)
                            {
                                CloseMap(fuben);
                                FubenMaps.RemoveAt(i);
                            }
                        }

                        foreach (SpawnInfo spawn in Spawns)
                            spawn.DoSpawn(false);
                    }
                    catch (Exception ex)
                    {
                        Log($"主循环处理地图时发生错误。\n {ex}");
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureException(ex);
                        }
                    }

                    ConquestWar.Process();

                    while (!WebCommandQueue.IsEmpty)
                    {
                        if (!WebCommandQueue.TryDequeue(out WebCommand webCommand)) continue;

                        switch (webCommand.Command)
                        {
                            case CommandType.None:
                                break;
                            case CommandType.Activation:
                                webCommand.Account.Activated = true;
                                webCommand.Account.ActivationKey = string.Empty;
                                break;
                            case CommandType.PasswordReset:
                                string password = Functions.RandomString(Random, 10);

                                webCommand.Account.Password = CreateHash(password);
                                webCommand.Account.ResetKey = string.Empty;
                                webCommand.Account.WrongPasswordCount = 0;
                                SendResetPasswordEmail(webCommand.Account, password);
                                break;
                            case CommandType.AccountDelete:
                                if (webCommand.Account.Activated) continue;

                                webCommand.Account.Delete();
                                break;
                        }
                    }

                    if (Config.ProcessGameGold)
                        ProcessGameGold();

                    nextCount = Now.AddSeconds(1);

                    //每天任务处理
                    if (nextCount.Day != Now.Day)
                    {
                        try
                        {
                            foreach (GuildInfo guild in GuildInfoList.Binding)
                            {
                                guild.DailyContribution = 0;
                                guild.DailyActivCount = 0;
                                guild.DailyGrowth = 0;

                                foreach (GuildMemberInfo member in guild.Members)
                                {
                                    member.DailyContribution = 0;
                                    if (member.Account.Connection?.Player == null) continue;

                                    member.Account.Connection.Enqueue(new S.GuildDayReset { ObserverPacket = false });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string msg = "行会贡献,行会成长清零时发生错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(e);
                            }
                        }

                        //每日和可重复任务 次数清零
                        try
                        {
                            foreach (CharacterInfo character in CharacterInfoList.Binding)
                            {
                                character.RepeatableQuestCount = 0;
                                character.DailyQuestCount = 0;
                                character.DayActiveCount = 0;
                                character.DayExpAdd = 0;
                                character.DayDonations = 0;
                            }
                        }
                        catch (Exception e)
                        {
                            string msg = "每日和可重复任务清零时发生错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(e);
                            }
                        }

                        //未完成的每日任务 删除
                        try
                        {
                            foreach (CharacterInfo character in CharacterInfoList.Binding)
                            {
                                if (character.Quests == null) continue;

                                foreach (UserQuest quest in character.Quests.ToList())
                                {
                                    if (quest.QuestInfo.QuestType == QuestType.Daily)
                                    {
                                        character.Quests.Remove(quest);
                                        quest.Delete();
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string msg = "未完成的每日任务删除时发生错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(e);
                            }
                        }

                        //设定为0点删除的自定义buff 移除
                        try
                        {
                            foreach (CharacterInfo character in CharacterInfoList.Binding)
                            {
                                foreach (BuffInfo buff in character.Buffs)
                                {
                                    if (buff.Type == BuffType.CustomBuff || buff.Type == BuffType.EventBuff || buff.Type == BuffType.TarzanBuff)
                                    {
                                        CustomBuffInfo customBuff =
                                            Globals.CustomBuffInfoList.Binding.FirstOrDefault(x =>
                                                x.Index == buff.FromCustomBuff);
                                        if (customBuff != null && customBuff.DeleteAtMidnight)
                                        {
                                            buff.RemainingTime = TimeSpan.Zero;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            string msg = "删除的自定义buff时发生错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(e);
                            }
                        }

                        //每日已使用的免费投币次数复位
                        try
                        {
                            foreach (CharacterInfo character in CharacterInfoList.Binding)
                            {
                                character.DailyFreeTossUsed = 0;
                            }
                        }
                        catch (Exception e)
                        {
                            string msg = "每日已使用的免费投币次数复位时发生错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(e);
                            }
                        }

                        try
                        {
                            dynamic trig_server;
                            if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                            {
                                PythonTuple args = PythonOps.MakeTuple(new object[] { Now });
                                ExecutePyWithTimer(trig_server, "OnDayChange", null);
                                //trig_server("OnDayChange", null);
                            }

                        }
                        catch (SyntaxErrorException e)
                        {

                            string msg = "延迟调用语法错误 : \"{0}\"";
                            ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                            string error = eo.FormatException(e);
                            SEnvir.Log(string.Format(msg, error));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureMessage(error, SentryLevel.Error);
                            }
                        }
                        catch (SystemExitException e)
                        {

                            string msg = "延迟系统退出 : \"{0}\"";
                            ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                            string error = eo.FormatException(e);
                            SEnvir.Log(string.Format(msg, error));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureMessage(error, SentryLevel.Error);
                            }
                        }
                        catch (Exception ex)
                        {

                            string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                            ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                            string error = eo.FormatException(ex);
                            SEnvir.Log(string.Format(msg, error));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureMessage(error, SentryLevel.Error);
                            }
                        }

                        GC.Collect(2, GCCollectionMode.Forced);
                    }

                    if (nextCount.Hour != Now.Hour)
                    {
                        //TODO 按小时触发的事件
                    }

                    if (Now.AddSeconds(-1).Minute != Now.Minute)
                    {
                        //按分钟触发的事件
                        //队列一个事件, 不要忘记添加listener
                        SEnvir.EventManager.QueueEvent(
                            new PlayerOnline(EventTypes.PlayerOnline,
                                new PlayerOnlineEventArgs { OnlineTime = TimeSpan.FromMinutes(1) }));

                        #region 定时PY事件 精确到分钟

                        try
                        {
                            dynamic trig_server;
                            if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                            {
                                PythonTuple args = PythonOps.MakeTuple(new object[] { Now });
                                IronPython.Runtime.List list = ExecutePyWithTimer(trig_server, "OnMinuteChange", args);
                                //trig_server("OnMinuteChange", args);
                                if (list != null)
                                {
                                    foreach (object actionObj in list)
                                    {
                                        //检查脚本是否返回了有效指令
                                        if (actionObj is PythonDictionary actions)
                                        {
                                            foreach (string actionType in actions.Keys)
                                            {
                                                switch (actionType)
                                                {
                                                    case "添加全服BUFF":

                                                        int buffIndex1 = (int)actions[actionType];
                                                        Globals.ActiveEventCustomBuffs[buffIndex1] = Now;
                                                        EventBuffChanged = true;

                                                        break;
                                                    case "移除全服BUFF":

                                                        int buffIndex2 = (int)actions[actionType];
                                                        Globals.ActiveEventCustomBuffs.Remove(buffIndex2);
                                                        EventBuffChanged = true;

                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                        }
                        catch (SyntaxErrorException e)
                        {
                            string msg = "延迟调用语法错误 : \"{0}\"";
                            ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                            string error = eo.FormatException(e);
                            SEnvir.Log(string.Format(msg, error));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureMessage(error, SentryLevel.Error);
                            }
                        }
                        catch (SystemExitException e)
                        {
                            string msg = "延迟系统退出 : \"{0}\"";
                            ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                            string error = eo.FormatException(e);
                            SEnvir.Log(string.Format(msg, error));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureMessage(error, SentryLevel.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                            ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                            string error = eo.FormatException(ex);
                            SEnvir.Log(string.Format(msg, error));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureMessage(error, SentryLevel.Error);
                            }
                        }

                        #endregion

                    }

                    /*try
                    {
                        foreach (CastleInfo info in CastleInfoList.Binding)
                        {
                            if (nextCount.TimeOfDay < info.StartTime) continue;
                            if (Now.TimeOfDay > info.StartTime) continue;

                            StartConquest(info, false);
                        }
                    }
                    catch (Exception e)
                    {
                        string msg = "处理行会战时出现错误 : \"{0}\"";
                        Log(e.Message);
                        Log(string.Format(msg, e.StackTrace));
                    }*/

                    #region 处理队列中的事件

                    //todo 性能问题 人多的时候每次需要10ms执行
                    //暂时停用 这里直接清空队列 不处理
                    if (SEnvir.EventManager != null)
                    {
                        SEnvir.EventManager.ClearQueue();
                    }

                    /*
                    try
                    {
                        if (SEnvir.EventManager != null)
                            SEnvir.EventManager.ProcessEvents();
                    }
                    catch (Exception e)
                    {
                        string msg = "处理队列中的事件时出现错误 : \"{0}\"";
                        Log(e.Message);
                        Log(string.Format(msg, e.StackTrace));
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureException(e);
                        }
                    }

                    */
                    #endregion

                    try
                    {
                        dynamic trig_server;
                        if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                        {
                            ExecutePyWithTimer(trig_server, "OnProcess", null);
                            //trig_server("OnProcess", null);
                        }

                    }
                    catch (SyntaxErrorException e)
                    {
                        string msg = "延迟调用语法错误 : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureMessage(error, SentryLevel.Error);
                        }
                    }
                    catch (SystemExitException e)
                    {
                        string msg = "延迟系统退出 : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureMessage(error, SentryLevel.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(ex);
                        SEnvir.Log(string.Format(msg, error));
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureMessage(error, SentryLevel.Error);
                        }
                    }
                }
                //统计MainTickLoop延迟
                delay = (Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond;
                if (delay > processDelay)
                    processDelay = delay;
                #endregion

                //Thread.Sleep(1);
            }
            catch (Exception ex)
            {
                Session = null;
                Log(ex.Message);
                Log(ex.StackTrace);
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(ex);
                }
                //不能多线程写文件
                File.AppendAllText(@"./Errors.txt", ex.Message + Environment.NewLine);
                File.AppendAllText(@"./Errors.txt", ex.StackTrace + Environment.NewLine);

                Packet p = new G.Disconnect { Reason = DisconnectReason.Crashed };
                for (int i = Connections.Count - 1; i >= 0; i--)
                    Connections[i].SendDisconnect(p);

                Thread.Sleep(3000);
                Stopping = true;
            }

        }
        #endregion

        #region Sentry信息

        private static void ReportServerInfo()
        {
            SentrySdk.WithScope(scope =>
            {
                bool licenseExists = false;
                if (File.Exists(Config.LicenseFile))
                {
                    scope.AddAttachment(Config.LicenseFile);
                    licenseExists = true;
                }

                SentrySdk.CaptureMessage($"{Config.ClientName} 服务端启动. \n" +
                                         $"时间: [{StartTime}] \n" +
                                         $"数据库密码：[{Config.DBPassword}] \n" +
                                         $"授权状态: [{LicenseState}] \n" +
                                         $"授权文件附件: [{licenseExists}] \n");
            });

        }

        #endregion

        #region 原版主循环
        private static void StartEnvir()
        {
            DateTime start = DateTime.Now;
            Log("加载数据库");
            try
            {
                LoadDatabase();
            }
            catch (Exception ex)
            {
                Log($"[数据库错误] 加载数据库异常." + ex.Message + ex.StackTrace);
                return;
            }

            #region 加载文件

            for (int i = 0; i < MapInfoList.Count; i++)
                if (!MapInfoList[i].IsDynamic)//添加是否为副本的判断
                    Maps[MapInfoList[i]] = new Map(MapInfoList[i]);
            Log("加载地图");
            Parallel.ForEach(Maps, x => x.Value.Load());

            #endregion

            foreach (Map map in Maps.Values)
                map.Setup();

            Parallel.ForEach(MapRegionList.Binding, x =>
            {
                Map map = GetMap(x.Map);

                if (map == null) return;

                x.CreatePoints(map.Width);
            });
            LoadBuyGameGoldInfo();  //BuyGameGold配置
            CheckGoldTable();       //货币
            LoadBanIpInfo();        //加载黑名单

            LoadSha256ProcessBanList(); // 加载文件封挂

            //关闭服务器自动广告功能 定时器
            SEvirtimer = new System.Timers.Timer();
            SEvirtimer.Enabled = false;
            SEvirtimer.Interval = 1000;
            SEvirtimer.Start();
            SEvirtimer.Elapsed += new System.Timers.ElapsedEventHandler(SEvirtimer_Elapsed);

            Log("加载安全区");
            CreateSafeZones();
            Log("加载钓鱼区");
            CreateFishingArea();
            Log("加载地图链接");
            CreateMovements();
            Log("加载NPC");
            CreateNPCs();
            Log("加载复活点");
            CreateSpawns();
            Log("加载城堡税收");
            InitCastleTaxInfo();
            TimeSpan elapsed = DateTime.Now - start;
            Log($"加载数据库成功");
        }

        ///<summary>
        ///吉米原版主循环
        ///</summary>
        ///<param name = "assembly" ></ param >
        public static void EnvirLoop()
        {
            Now = Time.Now;
            DateTime DBTime = Now + Config.DBSaveDelay;
            StartEnvir();
            StartNetwork();
            StartWebServer();

            Started = NetworkStarted;
            LoadScripts();

            LoadWorkflows();

            int count = 0, loopCount = 0;
            DateTime nextCount = Now.AddSeconds(1), UserCountTime = Now.AddMinutes(5), saveTime;

            #region 加载公告
            DateTime NoticeTime = Now + Config.NoticeDelay;  //公告
            LoadNotice();
            int noticeline = 0;
            #endregion

            long previousTotalSent = 0, previousTotalReceived = 0;
            int lastindex = 0;
            long conDelay = 0;
            long mainDelay = 0;
            long processDelay = 0;

            Thread LogThread = new Thread(WriteLogsLoop) { IsBackground = true };
            LogThread.Start();

            LastWarTime = Now;
            StartTime = Time.Now; //服务器启动时间
            GCTime = Now;

            if (Config.SentryEnabled)
            {
                ReportServerInfo();
            }

            if (Config.EnableRewardPool)
            {
                Log("奖金池功能已启用");
                RewardPoolReloadeBuffs();
                RefreshRewardPoolRanks();
            }
            else
            {
                Log("奖金池功能未启用");
            }
            Log("加载完毕");
            // debug信息
            Stopwatch metricsTimer = Stopwatch.StartNew();
            metricsTimer.Reset();
            double elapsed;

            while (Started)
            {
                Now = Time.Now;
                loopCount++;

                try
                {
                    #region ConnectionLoop
                    SConnection connection;
                    while (!NewConnections.IsEmpty)
                    {
                        if (!NewConnections.TryDequeue(out connection)) break;

                        IPCount.TryGetValue(connection.IPAddress, out var ipCount);
                        IPCount[connection.IPAddress] = ipCount + 1;

                        Connections.Add(connection);
                    }

                    long bytesSent = 0;
                    long bytesReceived = 0;

                    for (int i = Connections.Count - 1; i >= 0; i--)
                    {
                        if (i >= Connections.Count) break;

                        connection = Connections[i];

                        SentrySdk.ConfigureScope(s =>
                        {
                            s.SetTag("Account", $"{connection?.Account?.EMailAddress}");
                            s.SetTag("Player", $"{connection?.Player?.Name}");
                            s.SetTag("IP", $"{connection?.IPAddress}");
                        });

                        try
                        {
                            connection?.Process();
                            bytesSent += connection.TotalBytesSent;
                            bytesReceived += connection.TotalBytesReceived;
                        }
                        catch (Exception ex2)
                        {
                            Log(ex2.ToString());
                            Log(ex2.StackTrace);
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(ex2);
                            }
                            File.AppendAllText(@"./Errors.txt", ex2.Message + Environment.NewLine);
                            File.AppendAllText(@"./Errors.txt", ex2.StackTrace + Environment.NewLine);
                        }
                    }

                    //统计连接延迟
                    long delay = (Time.Now - Now).Ticks / TimeSpan.TicksPerMillisecond;
                    if (delay > conDelay)
                        conDelay = delay;

                    //统计连接收发字节数据量
                    TotalBytesSent = DBytesSent + bytesSent;
                    TotalBytesReceived = DBytesReceived + bytesReceived;
                    #endregion

                    #region MainTickLoop
                    DateTime time = Time.Now;
                    for (int i = Players.Count - 1; i >= 0; i--)
                        Players[i].StartProcess();

                    if (ServerBuffChanged)
                    {
                        for (int i = Players.Count - 1; i >= 0; i--)
                            Players[i].ApplyServerBuff();

                        ServerBuffChanged = false;
                    }

                    if (EventBuffChanged)
                    {
                        for (int i = Players.Count - 1; i >= 0; i--)
                            Players[i].ApplyEventBuff();

                        EventBuffChanged = false;
                    }

                    if (Config.EnableRewardPool && RewardPoolBuffChanged)
                    {
                        for (int i = Players.Count - 1; i >= 0; i--)
                            Players[i].ApplyRewardPoolBuff();

                        RewardPoolBuffChanged = false;
                    }

                    if (Config.EnableRewardPool && RPCoinEarnedRankChanged)
                    {
                        for (int i = Players.Count - 1; i >= 0; i--)
                            Players[i].SendRewardPoolRanks();

                        RPCoinEarnedRankChanged = false;
                    }

                    //if (ShouldRequestProcessHash)
                    //{
                    //    BroadcastRequestProcessHash();
                    //}

                    if (lastindex < 0) lastindex = ActiveObjects.Count;

                    DateTime loopTime = Time.Now.AddMilliseconds(1);
                    while (Time.Now <= loopTime)
                    {
                        lastindex--;

                        if (lastindex >= ActiveObjects.Count) continue;

                        if (lastindex < 0) break;

                        MapObject ob = ActiveObjects[lastindex];

                        if (ob.Race == ObjectType.Player) continue;

                        try
                        {
                            ob.StartProcess();
                            count++;
                        }
                        catch (Exception ex)
                        {
                            ActiveObjects.Remove(ob);
                            ob.Activated = false;

                            Log(ex.Message);
                            Log(ex.StackTrace);
                            File.AppendAllText(@"./Errors.txt", ex.StackTrace + Environment.NewLine);
                        }
                    }
                    //统计MainTickLoop处理用时
                    delay = (Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond;
                    if (delay > mainDelay)
                        mainDelay = delay;
                    #endregion

                    //每秒循环
                    #region MainProcessLoop
                    PlayerObject.ClearOverMarket(-Config.ClearOverMarket); //清理寄售
                    SEnvir.GoldMarketProcess(); //清理金币交易行
                    time = Time.Now;
                    if (Now >= nextCount)
                    {
                        //定时保存数据库
                        if (Now >= DBTime && !Saving)
                        {
                            DBTime = Time.Now + Config.DBSaveDelay;
                            saveTime = Time.Now;

                            Save();

                            SaveDelay = (Time.Now - saveTime).Ticks / TimeSpan.TicksPerMillisecond;
                        }

                        //每秒反馈循环统计信息
                        ProcessObjectCount = count;
                        LoopCount = loopCount;
                        ConDelay = conDelay;
                        MainDelay = mainDelay;
                        ProcessDelay = processDelay;

                        count = 0;
                        loopCount = 0;
                        conDelay = 0;
                        mainDelay = 0;
                        processDelay = 0;

                        DownloadSpeed = TotalBytesReceived - previousTotalReceived;
                        UploadSpeed = TotalBytesSent - previousTotalSent;

                        previousTotalReceived = TotalBytesReceived;
                        previousTotalSent = TotalBytesSent;


                        //发送公告信息
                        if (Now >= NoticeTime)
                        {
                            #region debug信息 开始计时
                            metricsTimer.Start();
                            #endregion

                            NoticeTime = Now + Config.NoticeDelay;
                            if (NoticeList.Count > 0)
                            {
                                if (noticeline >= NoticeList.Count) noticeline = 0;

                                foreach (SConnection conn in Connections)
                                {
                                    conn.ReceiveChat(NoticeList[noticeline], MessageType.Notice);  //公告颜色
                                }

                                noticeline++;
                            }

                            #region debug信息 结束计时
                            elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                            metricsTimer.Reset();
                            PyMetricsDict.AddOrUpdate("发送公告信息", new PyMetric
                            {
                                FunctionName = "发送公告信息",
                                ExecutionCount = 1,
                                TotalExecutionTime = elapsed,
                                MaxExecutionTime = elapsed,
                                MaxExecutionTimeMapObjectName = "***主循环调用***"
                            },
                                (key, value) =>
                                {
                                    value.ExecutionCount++;
                                    value.TotalExecutionTime += elapsed;
                                    value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                                    return value;
                                });
                            #endregion

                        }

                        //每5分钟发送在线人数信息
                        if (Now >= UserCountTime && Config.UserCount)
                        {
                            #region debug信息 开始计时
                            metricsTimer.Start();
                            #endregion

                            UserCountTime = Now.AddMinutes(Config.UserCountTime);
                            try
                            {
                                foreach (SConnection conn in Connections)
                                {
                                    conn.ReceiveChat("System.OnlineCount1".Lang(conn.Language, Players.Count + Config.UserCountDouble), MessageType.Hint);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                                Log(ex.StackTrace);
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(ex);
                                }
                            }

                            //Parallel.ForEach(Connections, conn =>
                            //{
                            //    try
                            //    {
                            //        if (Config.UserCount)  //显示在线人数
                            //        {
                            //            if (!Config.AllowObservation)
                            //                conn.ReceiveChat("System.OnlineCount1".Lang(conn.Language, Players.Count + Config.UserCountDouble), MessageType.Hint);
                            //            else
                            //                conn.ReceiveChat("System.OnlineCount".Lang(conn.Language, Players.Count + Config.UserCountDouble, Connections.Count(x => x.Stage == GameStage.Observer)), MessageType.Hint);
                            //        }

                            //        //if (Config.AllowObservation)  //显示观察者信息
                            //        //{
                            //        //    switch (conn.Stage)
                            //        //    {
                            //        //        case GameStage.Game:
                            //        //            if (conn.Player.Character.Observable)
                            //        //                conn.ReceiveChat("System.ObserverCount".Lang(conn.Language, conn.Observers.Count), MessageType.Hint);
                            //        //            break;
                            //        //        case GameStage.Observer:
                            //        //            conn.ReceiveChat("System.ObserverCount".Lang(conn.Language, conn.Observed.Observers.Count), MessageType.Hint);
                            //        //            break;
                            //        //    }
                            //        //}
                            //    }
                            //    catch (Exception ex)
                            //    {
                            //        Log(ex.Message);
                            //        Log(ex.StackTrace);
                            //        if (Config.SentryEnabled)
                            //        {
                            //            SentrySdk.CaptureException(ex);
                            //        }
                            //    }
                            //});

                            #region debug信息 结束计时
                            elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                            metricsTimer.Reset();
                            PyMetricsDict.AddOrUpdate("发送在线人数信息", new PyMetric
                            {
                                FunctionName = "发送在线人数信息",
                                ExecutionCount = 1,
                                TotalExecutionTime = elapsed,
                                MaxExecutionTime = elapsed,
                                MaxExecutionTimeMapObjectName = "***主循环调用***"
                            }, (key, value) =>
                                {
                                    value.ExecutionCount++;
                                    value.TotalExecutionTime += elapsed;
                                    value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                                    return value;
                                });
                            #endregion
                        }

                        CalculateLights();

                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion

                        CheckGuildWars();

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("检查行会战", new PyMetric
                        {
                            FunctionName = "检查行会战",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                            {
                                value.ExecutionCount++;
                                value.TotalExecutionTime += elapsed;
                                value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                                return value;
                            });
                        #endregion

                        #region 处理定时py任务

                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion


                        List<ActionScript> runlist = ScriptList.FindAll(x => x.StartTime < Now);
                        DateTime pyloopTime = Time.Now.AddMilliseconds(200);
                        if (runlist.Count > 0)
                        {
                            int listcount = runlist.Count;
                            while (Time.Now < pyloopTime)
                            {
                                listcount--;
                                if (listcount < 0) break;
                                ActionScript scp = runlist[listcount];
                                try
                                {
                                    string[] modules = scp.ScriptStr.Split('.');

                                    if (modules.Length > 0)
                                    {
                                        int strlen = 0;
                                        dynamic ob;
                                        ob = scope.GetVariable(modules[strlen]);
                                        strlen++;
                                        while (strlen < modules.Length)
                                        {
                                            ob = scope.Engine.Operations.GetMember(ob, modules[strlen]);
                                            strlen++;
                                        }
                                        ob(scp.Param);
                                    }
                                }
                                catch (SyntaxErrorException e)
                                {

                                    string msg = "DelayCall Syntax error : \"{0}\"";
                                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                    string error = eo.FormatException(e);
                                    SEnvir.Log(string.Format(msg, error));
                                    if (Config.SentryEnabled)
                                    {
                                        SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                    }
                                }
                                catch (SystemExitException e)
                                {

                                    string msg = "DelayCall SystemExit : \"{0}\"";
                                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                    string error = eo.FormatException(e);
                                    SEnvir.Log(string.Format(msg, error));
                                    if (Config.SentryEnabled)
                                    {
                                        SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                    }
                                }
                                catch (Exception ex)
                                {

                                    string msg = "DelayCall Error loading plugin : \"{0}\"";
                                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                    string error = eo.FormatException(ex);
                                    SEnvir.Log(string.Format(msg, error));
                                    SEnvir.Log(scp.ScriptStr);
                                    if (Config.SentryEnabled)
                                    {
                                        SentrySdk.CaptureException(ex);
                                    }
                                }

                                if (scp.Repeat)
                                {
                                    //重复执行 更新下次执行的时间
                                    scp.StartTime = Now + scp.Interval;
                                }
                                else
                                {
                                    //仅执行一次 移除此脚本
                                    ScriptList.Remove(scp);
                                }
                            }
                        }

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理定时py任务", new PyMetric
                        {
                            FunctionName = "处理定时py任务",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion

                        #endregion

                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion

                        //处理地图
                        try
                        {
                            foreach (KeyValuePair<MapInfo, Map> pair in Maps)
                                pair.Value.Process();

                            for (int i = FubenMaps.Count - 1; i >= 0; i--)
                            {
                                Map fuben = FubenMaps[i];
                                if (fuben.MapTime < Now && fuben.MapTime > DateTime.MinValue)
                                {
                                    CloseMap(fuben);
                                    FubenMaps.RemoveAt(i);
                                }
                            }

                            foreach (SpawnInfo spawn in Spawns)
                                spawn.DoSpawn(false);
                        }
                        catch (Exception ex)
                        {
                            Log($"主循环处理地图时发生错误。\n {ex}");
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(ex);
                            }
                        }

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理地图和刷怪", new PyMetric
                        {
                            FunctionName = "处理地图和刷怪",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion


                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion

                        ConquestWar.Process();
                        NewAuctionProcess();

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理攻城战", new PyMetric
                        {
                            FunctionName = "处理攻城战",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion


                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion


                        while (!WebCommandQueue.IsEmpty)
                        {
                            if (!WebCommandQueue.TryDequeue(out WebCommand webCommand)) continue;

                            switch (webCommand.Command)
                            {
                                case CommandType.None:
                                    break;
                                case CommandType.Activation:
                                    webCommand.Account.Activated = true;
                                    webCommand.Account.ActivationKey = string.Empty;
                                    break;
                                case CommandType.PasswordReset:
                                    string password = Functions.RandomString(Random, 10);

                                    webCommand.Account.Password = CreateHash(password);
                                    webCommand.Account.ResetKey = string.Empty;
                                    webCommand.Account.WrongPasswordCount = 0;
                                    SendResetPasswordEmail(webCommand.Account, password);
                                    break;
                                case CommandType.AccountDelete:
                                    if (webCommand.Account.Activated) continue;

                                    webCommand.Account.Delete();
                                    break;
                            }
                        }

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理WebCommandQueue", new PyMetric
                        {
                            FunctionName = "处理WebCommandQueue",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion


                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion


                        if (Config.ProcessGameGold)
                            ProcessGameGold();

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理在线充值", new PyMetric
                        {
                            FunctionName = "处理在线充值",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion

                        nextCount = Now.AddSeconds(1);


                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion


                        //新的一天
                        if (nextCount.Day != Now.Day)
                        {
                            try
                            {
                                foreach (GuildInfo guild in GuildInfoList.Binding)
                                {
                                    guild.DailyContribution = 0;
                                    guild.DailyGrowth = 0;
                                    guild.DailyActivCount = 0;

                                    foreach (GuildMemberInfo member in guild.Members)
                                    {
                                        member.DailyContribution = 0;
                                        if (member.Account.Connection?.Player == null) continue;

                                        member.Account.Connection.Enqueue(new S.GuildDayReset { ObserverPacket = false });
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = "行会贡献,行会成长清零时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            //每日和可重复任务 次数清零
                            try
                            {
                                foreach (CharacterInfo character in CharacterInfoList.Binding)
                                {
                                    character.RepeatableQuestCount = 0;
                                    character.DailyQuestCount = 0;
                                    character.DayActiveCount = 0;
                                    character.DayExpAdd = 0;
                                    character.DayDonations = 0;
                                    character.TotalActiveCount = 0;

                                    if (character.Account.Connection?.Player != null)
                                        character.Account.Connection?.Player.Enqueue(new S.GuildActiveCountChange
                                        {
                                            DailyActiveCount = character.DayActiveCount,
                                            TotalActiveCount = character.TotalActiveCount
                                        });
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = "每日和可重复任务清零时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            //未完成的每日任务 删除
                            try
                            {
                                foreach (CharacterInfo character in CharacterInfoList.Binding)
                                {
                                    if (character.Quests == null) continue;

                                    foreach (UserQuest quest in character.Quests.ToList())
                                    {
                                        if (quest.QuestInfo.QuestType == QuestType.Daily)
                                        {
                                            character.Quests.Remove(quest);
                                            quest.Delete();
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = "未完成的每日任务删除时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            //设定为0点删除的自定义buff 移除
                            try
                            {
                                foreach (CharacterInfo character in CharacterInfoList.Binding)
                                {
                                    foreach (BuffInfo buff in character.Buffs)
                                    {
                                        if (buff.Type == BuffType.CustomBuff || buff.Type == BuffType.EventBuff || buff.Type == BuffType.TarzanBuff)
                                        {
                                            CustomBuffInfo customBuff =
                                                Globals.CustomBuffInfoList.Binding.FirstOrDefault(x =>
                                                    x.Index == buff.FromCustomBuff);
                                            if (customBuff != null && customBuff.DeleteAtMidnight)
                                            {
                                                buff.RemainingTime = TimeSpan.Zero;
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = "删除的自定义buff时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            //每日已使用的免费投币次数复位
                            try
                            {
                                foreach (CharacterInfo character in CharacterInfoList.Binding)
                                {
                                    character.DailyFreeTossUsed = 0;
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = "每日已使用的免费投币次数复位时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            // 每天0点删除已过期的红包
                            try
                            {
                                var expiredRedPackets = RedPacketInfoList.Binding.Where(x => x.HasExpired).ToArray();
                                for (int i = expiredRedPackets.Length - 1; i >= 0; i--)
                                {
                                    expiredRedPackets[i].Delete();
                                }

                                expiredRedPackets = null;
                            }
                            catch (Exception e)
                            {
                                string msg = "每日过期红包删除时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            // 每日累计在线时长清零
                            try
                            {
                                foreach (CharacterInfo character in CharacterInfoList.Binding)
                                {
                                    character.TodayOnlineMinutes = 0;
                                }
                            }
                            catch (Exception e)
                            {
                                string msg = "每日累计在线时长清零时发生错误 : \"{0}\"";
                                Log(e.Message);
                                Log(string.Format(msg, e.StackTrace));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureException(e);
                                }
                            }

                            try
                            {
                                dynamic trig_server;
                                if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                                {
                                    PythonTuple args = PythonOps.MakeTuple(new object[] { Now });
                                    ExecutePyWithTimer(trig_server, "OnDayChange", args);
                                    //trig_server("OnDayChange", null);
                                }

                            }
                            catch (SyntaxErrorException e)
                            {

                                string msg = "延迟调用语法错误 : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }
                            catch (SystemExitException e)
                            {

                                string msg = "延迟系统退出 : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }
                            catch (Exception ex)
                            {

                                string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(ex);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }

                            GC.Collect(2, GCCollectionMode.Forced);
                        }

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理每天触发的事务", new PyMetric
                        {
                            FunctionName = "处理每天触发的事务",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion


                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion

                        if (nextCount.Hour != Now.Hour)
                        {
                            //TODO 按小时触发的事件
                        }

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理每小时触发的事务", new PyMetric
                        {
                            FunctionName = "处理每小时触发的事务",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion


                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion


                        if (Now.AddSeconds(-1).Minute != Now.Minute)
                        {
                            //按分钟触发的事件
                            //队列一个事件, 不要忘记添加listener
                            SEnvir.EventManager.QueueEvent(
                                new PlayerOnline(EventTypes.PlayerOnline,
                                    new PlayerOnlineEventArgs { OnlineTime = TimeSpan.FromMinutes(1) }));

                            #region 定时PY事件 精确到分钟

                            try
                            {
                                dynamic trig_server;
                                if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                                {
                                    PythonTuple args = PythonOps.MakeTuple(new object[] { Now });
                                    IronPython.Runtime.List list =
                                        SEnvir.ExecutePyWithTimer(trig_server, "OnMinuteChange", args);
                                    //IronPython.Runtime.List list = trig_server("OnMinuteChange", args);
                                    if (list != null)
                                    {
                                        foreach (object actionObj in list)
                                        {
                                            //检查脚本是否返回了有效指令
                                            if (actionObj is PythonDictionary actions)
                                            {
                                                foreach (string actionType in actions.Keys)
                                                {
                                                    switch (actionType)
                                                    {
                                                        case "添加全服BUFF":

                                                            int buffIndex1 = (int)actions[actionType];
                                                            Globals.ActiveEventCustomBuffs[buffIndex1] = Now;
                                                            EventBuffChanged = true;

                                                            break;
                                                        case "移除全服BUFF":

                                                            int buffIndex2 = (int)actions[actionType];
                                                            Globals.ActiveEventCustomBuffs.Remove(buffIndex2);
                                                            EventBuffChanged = true;

                                                            break;
                                                        default:
                                                            break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                            catch (SyntaxErrorException e)
                            {
                                string msg = "延迟调用语法错误 : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }
                            catch (SystemExitException e)
                            {
                                string msg = "延迟系统退出 : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }
                            catch (Exception ex)
                            {
                                string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(ex);
                                SEnvir.Log(string.Format(msg, error));
                                if (Config.SentryEnabled)
                                {
                                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                }
                            }

                            #endregion

                        }


                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理每分钟触发的事务", new PyMetric
                        {
                            FunctionName = "处理每分钟触发的事务",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion

                        /*try
                        {
                            foreach (CastleInfo info in CastleInfoList.Binding)
                            {
                                if (nextCount.TimeOfDay < info.StartTime) continue;
                                if (Now.TimeOfDay > info.StartTime) continue;

                                StartConquest(info, false);
                            }
                        }
                        catch (Exception e)
                        {
                            string msg = "处理行会战时出现错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                        }*/

                        #region 处理队列中的事件

                        #region debug信息 开始计时
                        metricsTimer.Start();
                        #endregion
                        if (SEnvir.EventManager != null)
                        {
                            SEnvir.EventManager.ClearQueue();
                        }

                        /*
                        try
                        {
                            if (SEnvir.EventManager != null)
                                SEnvir.EventManager.ProcessEvents();
                        }
                        catch (Exception e)
                        {
                            string msg = "处理队列中的事件时出现错误 : \"{0}\"";
                            Log(e.Message);
                            Log(string.Format(msg, e.StackTrace));
                            if (Config.SentryEnabled)
                            {
                                SentrySdk.CaptureException(e);
                            }
                        }

                        */

                        #region debug信息 结束计时
                        elapsed = metricsTimer.Elapsed.TotalMilliseconds;
                        metricsTimer.Reset();
                        PyMetricsDict.AddOrUpdate("处理队列中的事件", new PyMetric
                        {
                            FunctionName = "处理队列中的事件",
                            ExecutionCount = 1,
                            TotalExecutionTime = elapsed,
                            MaxExecutionTime = elapsed,
                            MaxExecutionTimeMapObjectName = "***主循环调用***"
                        }, (key, value) =>
                        {
                            value.ExecutionCount++;
                            value.TotalExecutionTime += elapsed;
                            value.MaxExecutionTime = Math.Max(value.MaxExecutionTime, elapsed);
                            return value;
                        });
                        #endregion

                        #endregion


                        /* 这段代码不需要 先注释掉
                                                try
                                                {
                                                    dynamic trig_server;
                                                    if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                                                    {
                                                        ExecutePyWithTimer(trig_server, "OnProcess", null);
                                                        //trig_server("OnProcess", null);
                                                    }

                                                }
                                                catch (SyntaxErrorException e)
                                                {
                                                    string msg = "延迟调用语法错误 : \"{0}\"";
                                                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                                    string error = eo.FormatException(e);
                                                    SEnvir.Log(string.Format(msg, error));
                                                    if (Config.SentryEnabled)
                                                    {
                                                        SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                                    }
                                                }
                                                catch (SystemExitException e)
                                                {
                                                    string msg = "延迟系统退出 : \"{0}\"";
                                                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                                    string error = eo.FormatException(e);
                                                    SEnvir.Log(string.Format(msg, error));
                                                    if (Config.SentryEnabled)
                                                    {
                                                        SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {
                                                    string msg = "加载插件时出现延迟调用错误 : \"{0}\"";
                                                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                                    string error = eo.FormatException(ex);
                                                    SEnvir.Log(string.Format(msg, error));
                                                    if (Config.SentryEnabled)
                                                    {
                                                        SentrySdk.CaptureMessage(error, SentryLevel.Error);
                                                    }
                                                }

                                                */
                    }

                    //统计MainProcessLoop处理用时
                    delay = (Time.Now - time).Ticks / TimeSpan.TicksPerMillisecond;
                    if (delay > processDelay)
                        processDelay = delay;
                    #endregion

#if DEBUG //调试模式限制循环，以免电脑风扇呼呼响
                    Thread.Sleep(1);
#endif
                    #region 释放内存
                    if (GCTime < Now)
                    {
                        System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet = new System.IntPtr(5);
                        GCTime = Now.AddSeconds(Config.GCDelay.TotalSeconds);
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Session = null;

                    Log(ex.Message);
                    Log(ex.StackTrace);
                    if (Config.SentryEnabled)
                    {
                        SentrySdk.CaptureException(ex);
                    }
                    File.AppendAllText(@"./Errors.txt", ex.Message + Environment.NewLine);
                    File.AppendAllText(@"./Errors.txt", ex.StackTrace + Environment.NewLine);

                    Packet p = new G.Disconnect { Reason = DisconnectReason.Crashed };
                    for (int i = Connections.Count - 1; i >= 0; i--)
                        Connections[i].SendDisconnect(p);

                    Thread.Sleep(3000);
                    break;
                }
            }
            StopWebServer();
            StopNetwork();

            while (Saving) Thread.Sleep(1);
            if (Session != null)
                Session.BackUpDelay = 0;
            Save();
            while (Saving) Thread.Sleep(1);

            StopEnvir();
        }
        #endregion

        /// <summary>
        /// 保存
        /// </summary>
        private static void Save()
        {
            if (Session == null) return;

            Saving = true;

            Session.Save(false, SessionMode.Server);
            Task.Run(() =>
            {

                HandledPayments.AddRange(PaymentList);
                CommitChanges(Session);
            });
            //Thread saveThread = new Thread(CommitChanges) { IsBackground = true };
            //saveThread.Start(Session);
        }
        /// <summary>
        /// 提交更改
        /// </summary>
        /// <param name="data"></param>
        private static void CommitChanges(object data)
        {
            Session session = (Session)data;
            session?.Commit(SessionMode.Server);

            foreach (IPNMessage message in HandledPayments)
            {
                if (message.Duplicate)
                {
                    File.Delete(message.FileName);
                    continue;
                }

                if (!Directory.Exists(CompletePath))
                    Directory.CreateDirectory(CompletePath);

                File.Move(message.FileName, CompletePath + Path.GetFileName(message.FileName) + ".txt");
                PaymentList.Remove(message);
            }
            HandledPayments.Clear();

            Saving = false;
        }
        /// <summary>
        /// 写入日志循环
        /// </summary>
        private static void WriteLogsLoop()
        {
            DateTime NextLogTime = Now.AddSeconds(10);

            while (Started)
            {
                if (Now < NextLogTime)
                {
                    Thread.Sleep(1);
                    continue;
                }

                WriteLogs();

                NextLogTime = Now.AddSeconds(10);
            }
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        public static void WriteLogs()
        {
            List<string> lines = new List<string>();
            while (!Logs.IsEmpty)
            {
                if (!Logs.TryDequeue(out string line)) continue;
                lines.Add(line);
            }

            File.AppendAllLines(@"./Logs/Logs_" + $"{DateTime.Now.Month}_{DateTime.Now.Day}.txt", lines);

            lines.Clear();

            while (!ChatLogs.IsEmpty)
            {
                if (!ChatLogs.TryDequeue(out string line)) continue;
                lines.Add(line);
            }

            File.AppendAllLines(@"./Chat/Chat Logs_" + $"{DateTime.Now.Month}_{DateTime.Now.Day}.txt", lines);

            lines.Clear();

            /*
            while (!GamePlayLogs.IsEmpty)
            {
                if (!GamePlayLogs.TryDequeue(out string line)) continue;
                lines.Add(line);
            }

            File.AppendAllLines(@"./Game Play.txt", lines);
            */

            lines.Clear();
        }

        public static void NewAuctionProcess()
        {
            //到时间清空拍卖列表 公布拍卖结果
            if (DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("22:59")) > 0)
            {
                var auctionList = SEnvir.NewAutionInfoList.Where(x => !x.Closed).ToList();
                foreach (var auction in auctionList)
                {
                    if (auction.Closed || auction.Item == null) continue;

                    //TODO一口价完成 给物品 扣赞助币
                    auction.Closed = true;
                    UserItem item = auction.Item;
                    auction.Item = null;

                    long cost = auction.LastPrice;
                    // 邮件交易
                    MailInfo mail = SEnvir.MailInfoList.CreateNewObject();
                    mail.Account = auction.Account;
                    if (auction.LastCharacter == null)
                    {
                        mail.Subject = "拍卖退回清单";
                        mail.Sender = "拍卖";
                        item.Mail = mail;
                        item.Slot = 0;
                        string itemName1;
                        int partIndex1 = item.Stats[Stat.ItemIndex];
                        if (item.Info.Effect == ItemEffect.ItemPart)
                            itemName1 = SEnvir.ItemInfoList.Binding.First(x => x.Index == partIndex1).ItemName + " - [" + "碎片" + "]";
                        else
                            itemName1 = item.Info.ItemName;
                        mail.Message = string.Format("{0}x{1:#,##0}", itemName1, item.Count);

                        mail.HasItem = true;
                        if (auction.Account.Connection?.Player != null)
                            auction.Account.Connection.Enqueue(new S.MailNew
                            {
                                Mail = mail.ToClientInfo(),
                                ObserverPacket = false,
                            });
                        continue;
                    };

                    long tax = (long)(cost * Config.NewAuctionTax);//税率

                    mail.Subject = "拍卖清单";
                    mail.Sender = "拍卖";

                    ItemInfo itemInfo = item.Info;
                    int partIndex = item.Stats[Stat.ItemIndex];

                    string itemName;

                    if (item.Info.Effect == ItemEffect.ItemPart)
                        itemName = SEnvir.ItemInfoList.Binding.First(x => x.Index == partIndex).ItemName + " - [" + "碎片" + "]";
                    else
                        itemName = item.Info.ItemName;
                    string pricename;
                    var Name = auction.LastCharacter.CharacterName;
                    UserItem gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                    gold.Count = (long)(cost);//   - tax);
                    pricename = "赞助币";
                    gold.Mail = mail;
                    gold.Slot = 0;
                    mail.Message = $"你成功拍卖出一件商品\n\n" +
                                   string.Format("买家: {0}\n", Name) +
                                   string.Format("商品: {0} x{1}\n", itemName, 1) +
                                   string.Format("{1}: {0:#,##0} 单个\n", auction.LastPrice / 100, pricename) +
                                   string.Format("小计: {0:#,##0}\n\n", cost / 100) +
                                   //string.Format("税收: {0:#,##0} ({1:p0})\n\n", tax / 100, Config.NewAuctionTax) +
                                   string.Format("合计: {0:#,##0}", cost / 100);  // - tax)

                    mail.HasItem = true;
                    if (auction.Account.Connection?.Player != null)
                        auction.Account.Connection.Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });


                    item.Flags |= UserItemFlags.None;
                    var player = SEnvir.GetPlayerByCharacter(auction.LastCharacter.CharacterName);

                    if (!(player?.InSafeZone ?? false) || !(player?.CanGainItems(false, new ItemCheck(item, item.Count, item.Flags, item.ExpireTime)) ?? false))
                    {
                        mail = SEnvir.MailInfoList.CreateNewObject();

                        mail.Account = auction.LastCharacter.Account;

                        mail.Subject = "拍卖商品";
                        mail.Sender = "拍卖";
                        mail.Message = string.Format("{0}x{1:#,##0}", itemName, item.Count);

                        item.Mail = mail;
                        item.Slot = 0;
                        mail.HasItem = true;

                        player?.Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });
                    }
                    else
                    {
                        player.GainItem(item);
                    }
                }

            }
            SEnvir.NewAutionInfoTempList.Clear();
        }
        //19-03-03 东方未明添加充值函数
        //说明此充值函数由于是读取第三方充值生成的txt文件，
        //由于第三方充值提供的接口不同这个程序的仅仅适用于某平台的充值系统
        //在实际使用过程中可能会出现同步问题，且无法有效解决
        //由于侦测函数位于游戏主循环目录，对txt频繁的IO操作将消耗大量cpu 时间段
        //所以这个方法会存在性能问题
        private static readonly Regex HeaderRegex = new Regex(@"^\[(?<Header>.+)\]$", RegexOptions.Compiled);
        private static readonly Regex EntryRegex = new Regex(@"^(?<Key>.*?)=(?<Value>.*)$", RegexOptions.Compiled);
        private static void _ProcessGameGold()
        {
            if (!File.Exists(Config.OrderPath)) return;
            try
            {
                string[] lines = File.ReadAllLines(Config.OrderPath, Encoding.Default);  //根据第三方平台读出数据并进行编码转换格式
                if (lines.Length != 0)
                {
                    //读入后立刻清空 尽量避免同步问题
                    File.WriteAllText(Config.OrderPath, "");
                    //处理数据
                    Dictionary<string, Dictionary<string, string>> contents = new Dictionary<string, Dictionary<string, string>>();
                    Dictionary<string, string> section = null;

                    foreach (string line in lines)
                    {
                        Match match = HeaderRegex.Match(line);
                        if (match.Success)
                        {
                            section = new Dictionary<string, string>();
                            contents[match.Groups["Header"].Value] = section;
                            continue;
                        }
                        if (section == null) continue;

                        match = EntryRegex.Match(line);

                        if (!match.Success) continue;

                        section[match.Groups["Key"].Value] = match.Groups["Value"].Value;

                    }
                    foreach (KeyValuePair<string, Dictionary<string, string>> item in contents)
                    {
                        //创建一个充值变量 服务端数据库将记录充值数据
                        GameGoldPayment payment = GameGoldPaymentList.CreateNewObject();
                        payment.CharacterName = item.Key;//获得用户名
                        int count = 0;
                        Int32.TryParse(item.Value["RMB"], out count);   //充值文本里对应元宝的名称
                        payment.GameGoldAmount = count;
                        CharacterInfo character = GetCharacter(payment.CharacterName);
                        payment.Status = "正在充值";
                        payment.Error = false;
                        if (character == null || payment.Error)
                        {
                            payment.Status = "充值出错";
                            Log($"[交易错误] 交易账号:{payment.CharacterName} 交易状态:{payment.Status}, 数量{payment.GameGoldAmount}.");
                            continue;
                        }
                        payment.Status = "充值完成";
                        payment.Account = character.Account;
                        payment.Account.GameGold += payment.GameGoldAmount * 100;
                        character.Account.Connection?.ReceiveChat("Payment.PaymentComplete".Lang(character.Account.Connection.Language, payment.GameGoldAmount), MessageType.System);
                        character.Player?.Enqueue(new S.GameGoldChanged { GameGold = payment.Account.GameGold });

                        // 记录
                        // 构造日志条目
                        CurrencyLogEntry logEntry = new CurrencyLogEntry()
                        {
                            LogLevel = LogLevel.Info,
                            Component = "充值系统",
                            Time = Now,
                            Character = character,
                            Currency = CurrencyType.GameGold,
                            Action = CurrencyAction.Add,
                            Source = CurrencySource.TopUp,
                            Amount = payment.GameGoldAmount * 100,
                            ExtraInfo = ""
                        };
                        // 存入日志
                        LogToViewAndCSV(logEntry);

                        AccountInfo referral = payment.Account.Referral;

                        if (referral != null)
                        {
                            referral.HuntGold += payment.GameGoldAmount / Config.HuntGoldRated;   //元宝充值获得赏金比例

                            if (referral.Connection != null)
                            {
                                referral.Connection.ReceiveChat("Payment.ReferralPaymentComplete".Lang(referral.Connection.Language, payment.GameGoldAmount / Config.HuntGoldRated), MessageType.System);

                                if (referral.Connection.Stage == GameStage.Game)
                                    referral.Connection.Player.Enqueue(new S.HuntGoldChanged { HuntGold = referral.GameGold });
                            }
                        }

                        Log($"[元宝购买] 角色: {character.CharacterName}, 数量: {payment.GameGoldAmount}.");
                    }
                }

            }
            catch (Exception e)
            {
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(e);
                }
            }
        }
        /// <summary>
        /// 处理充值流程
        /// </summary>
        private static void ProcessGameGold()
        {
            if (Config.RechargeInterface)
            {
                _ProcessGameGold();
            }

            while (!Messages.IsEmpty)
            {
                IPNMessage message;

                if (!Messages.TryDequeue(out message) || message == null) return;

                PaymentList.Add(message);

                if (!message.Verified)
                {
                    Log("网上充值失败 " + message.Message);
                    continue;
                }

                //Add message to another list for file moving

                string[] data = message.Message.Split('&');

                Dictionary<string, string> values = new Dictionary<string, string>();

                for (int i = 0; i < data.Length; i++)
                {
                    string[] keypair = data[i].Split('=');

                    values[keypair[0]] = keypair.Length > 1 ? keypair[1] : null;
                }

                bool error = false;
                string tempString, paymentStatus, transactionID;
                decimal tempDecimal;
                int tempInt;

                if (!values.TryGetValue("payment_status", out paymentStatus))
                    error = true;

                if (!values.TryGetValue("txn_id", out transactionID))
                    error = true;

                //Check that Txn_id has not been used
                for (int i = 0; i < GameGoldPaymentList.Count; i++)
                {
                    if (GameGoldPaymentList[i].TransactionID != transactionID) continue;
                    if (GameGoldPaymentList[i].Status != paymentStatus) continue;

                    Log(string.Format("[复制交易] 角色:{0} 情况:{1}.", transactionID, paymentStatus));
                    message.Duplicate = true;
                    return;
                }

                GameGoldPayment payment = GameGoldPaymentList.CreateNewObject();
                payment.RawMessage = message.Message;
                payment.Error = error;

                if (values.TryGetValue("payment_date", out tempString))
                    payment.PaymentDate = HttpUtility.UrlDecode(tempString);

                if (values.TryGetValue("receiver_email", out tempString))
                    payment.Receiver_EMail = HttpUtility.UrlDecode(tempString);
                else
                    payment.Error = true;

                if (values.TryGetValue("mc_fee", out tempString) && decimal.TryParse(tempString, out tempDecimal))
                    payment.Fee = tempDecimal;
                else
                    payment.Error = true;

                if (values.TryGetValue("mc_gross", out tempString) && decimal.TryParse(tempString, out tempDecimal))
                    payment.Price = tempDecimal;
                else
                    payment.Error = true;

                if (values.TryGetValue("custom", out tempString))
                    payment.CharacterName = HttpUtility.UrlDecode(HttpUtility.UrlDecode(tempString));
                else
                    payment.Error = true;

                if (values.TryGetValue("mc_currency", out tempString))
                    payment.Currency = tempString;
                else
                    payment.Error = true;

                if (values.TryGetValue("txn_type", out tempString))
                    payment.TransactionType = tempString;
                else
                    payment.Error = true;

                if (values.TryGetValue("payer_email", out tempString))
                    payment.Payer_EMail = HttpUtility.UrlDecode(tempString);

                if (values.TryGetValue("payer_id", out tempString))
                    payment.Payer_ID = tempString;

                payment.Status = paymentStatus;
                payment.TransactionID = transactionID;
                //检查Paymentstats==是否已完成
                switch (payment.Status)
                {
                    case "Completed":
                        break;
                }
                if (payment.Status != Completed) continue;

                //检查收件人的电子邮件
                if (string.Compare(payment.Receiver_EMail, Config.ReceiverEMail, StringComparison.OrdinalIgnoreCase) != 0)
                    payment.Error = true;

                //检查付款金额/当前金额是否正确
                if (payment.Currency != Currency)
                    payment.Error = true;

                if (GoldTable.TryGetValue(payment.Price, out tempInt))
                    payment.GameGoldAmount = tempInt;
                else
                    payment.Error = true;

                //充值处理
                CharacterInfo character = GetCharacter(payment.CharacterName);

                if (character == null || payment.Error)
                {
                    Log($"[交易错误] 交易账号:{transactionID} 充值名字:{payment.CharacterName} 交易状态:{paymentStatus}, 数量{payment.Price}.");
                    continue;
                }
                //开始充值增加游戏金币
                payment.Account = character.Account;
                payment.Account.GameGold += payment.GameGoldAmount;
                character.Account.Connection?.ReceiveChat("Payment.PaymentComplete".Lang(character.Account.Connection.Language, payment.GameGoldAmount), MessageType.System);
                character.Player?.Enqueue(new S.GameGoldChanged { GameGold = payment.Account.GameGold });

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "充值系统",
                    Time = Now,
                    Character = character,
                    Currency = CurrencyType.GameGold,
                    Action = CurrencyAction.Add,
                    Source = CurrencySource.TopUp,
                    Amount = payment.GameGoldAmount,
                    ExtraInfo = ""
                };
                // 存入日志
                LogToViewAndCSV(logEntry);

                //介绍人处理 给10%的狩猎金币奖励
                AccountInfo referral = payment.Account.Referral;

                if (referral != null)
                {
                    referral.HuntGold += payment.GameGoldAmount / Config.HuntGoldRated;

                    if (referral.Connection != null)
                    {
                        referral.Connection.ReceiveChat("Payment.ReferralPaymentComplete".Lang(referral.Connection.Language, payment.GameGoldAmount / Config.HuntGoldRated), MessageType.System);

                        if (referral.Connection.Stage == GameStage.Game)
                            referral.Connection.Player.Enqueue(new S.HuntGoldChanged { HuntGold = referral.GameGold });
                    }
                }

                Log($"[元宝购买] 角色: {character.CharacterName}, 数量: {payment.GameGoldAmount}.");
            }
        }

        #region 行会 攻城

        /// <summary>
        /// 检查行会战
        /// </summary>
        public static void CheckGuildWars()
        {
            TimeSpan change = Now - LastWarTime;
            LastWarTime = Now;

            for (int i = GuildWarInfoList.Count - 1; i >= 0; i--)
            {
                GuildWarInfo warInfo = GuildWarInfoList[i];

                warInfo.Duration -= change;

                if (warInfo.Duration > TimeSpan.Zero) continue;

                foreach (GuildMemberInfo member in warInfo.Guild1.Members)
                    member.Account.Connection?.Player?.Enqueue(new S.GuildWarFinished { GuildName = warInfo.Guild2.GuildName });

                foreach (GuildMemberInfo member in warInfo.Guild2.Members)
                    member.Account.Connection?.Player?.Enqueue(new S.GuildWarFinished { GuildName = warInfo.Guild1.GuildName });

                warInfo.Guild1 = null;
                warInfo.Guild2 = null;

                warInfo.Delete();
            }
        }

        /// <summary>
        /// 把某个城堡重新分配给指定行会
        /// </summary>
        public static void ReassignCastleToGuild(string guildName, string castleName)
        {
            CastleInfo castle = CastleInfoList.Binding.FirstOrDefault(x => x.Name == castleName);
            if (castle == null)
            {
                Log("无法重新分配城堡：找不到指定的城堡");
                return;
            }
            GuildInfo guild = GuildInfoList.Binding.FirstOrDefault(x => x.GuildName == guildName);
            if (guild == null)
            {
                Log("无法重新分配城堡：找不到指定的行会");
                return;
            }

            foreach (var guildInfo in GuildInfoList.Binding)
            {
                if (guildInfo.Castle == castle)
                {
                    guildInfo.Castle = null;
                    foreach (SConnection con in Connections)
                    {
                        switch (con.Stage)
                        {
                            case GameStage.Game:
                            case GameStage.Observer:
                                con.ReceiveChat("Conquest.ConquestLost".Lang(con.Language, guildInfo.GuildName, castle.Name), MessageType.System);
                                break;
                            default: continue;
                        }
                    }
                }
            }

            guild.Castle = castle;
            foreach (SConnection con in Connections)
            {
                switch (con.Stage)
                {
                    case GameStage.Game:
                    case GameStage.Observer:
                        con.ReceiveChat("Conquest.ConquestCapture".Lang(con.Language, guild.GuildName, castle.Name), MessageType.System);
                        break;
                    default: continue;
                }
            }
            Broadcast(new S.GuildCastleInfo { Index = castle.Index, Owner = guild.GuildName });

            //foreach (PlayerObject user in Players)
                //user.ApplyCastleBuff();
        }

        /// <summary>
        /// 城堡税收初始化
        /// </summary>
        public static void InitCastleTaxInfo()
        {

            for (int i = CastleFundInfoList.Count - 1; i >= 0; i--)
            {
                if (!CastleInfoList.Binding.Any(x => x.Name == CastleFundInfoList[i].Castle?.Name))
                {
                    CastleFundInfoList[i].Delete();
                }
            }

            foreach (CastleInfo castle in CastleInfoList.Binding)
            {
                if (!CastleFundInfoList.Binding.Any(x => x.Castle?.Name == castle.Name))
                {
                    CastleFundInfo fundInfo = CastleFundInfoList.CreateNewObject();
                    fundInfo.Castle = castle;
                    fundInfo.TotalTax = 0;
                    fundInfo.TotalDeposit = 0;
                }
            }
        }

        /// <summary>
        /// 两个角色是否在同一个行会
        /// </summary>
        /// <param name="char1"></param>
        /// <param name="char2"></param>
        /// <returns></returns>
        public static bool InSameGuild(CharacterInfo char1, CharacterInfo char2)
        {
            if (char1 == null || char2 == null) return false;
            if (char1.Account.GuildMember == null || char2.Account.GuildMember == null) return false;

            return char1.Account.GuildMember.Guild.Index == char2.Account.GuildMember.Guild.Index;
        }

        #endregion

        /// <summary>
        /// 计算光效
        /// </summary>
        public static void CalculateLights()
        {
            DayTime = Math.Max(0.05F, Math.Abs((float)Math.Round(((Now.TimeOfDay.TotalMinutes * Config.DayCycleCount) % 1440) / 1440F * 2 - 1, 2))); //12 hour rotation
        }

        /*public static void StartConquest(CastleInfo info, bool forced)
        {
            List<GuildInfo> participants = new List<GuildInfo>();

            if (!forced)
            {
                foreach (UserConquest conquest in UserConquestList.Binding)
                {
                    if (conquest.Castle != info) continue;
                    if (conquest.WarDate > Now.Date) continue;

                    participants.Add(conquest.Guild);
                }

                if (participants.Count == 0) return;

                foreach (GuildInfo guild in GuildInfoList.Binding)
                {
                    if (guild.Castle != info) continue;

                    participants.Add(guild);
                }
            }

            ConquestWar War = new ConquestWar
            {
                Castle = info,
                Participants = participants,
                EndTime = Now + info.Duration,
                StartTime = Now.Date + info.StartTime,
            };

            War.StartWar();
        }

        public static void StartConquest(CastleInfo info, List<GuildInfo> participants)  //开始攻城战
        {

            ConquestWar War = new ConquestWar
            {
                Castle = info,
                Participants = participants,
                EndTime = Now + TimeSpan.FromMinutes(15),
                StartTime = Now.Date + info.StartTime,
            };

            War.StartWar();
        }*/
        /// <summary>
        /// 记录物品来源
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sourceMapName"></param>
        /// <param name="sourceType"></param>
        /// <param name="sourceName"></param>
        /// <param name="originalOwner"></param>
        public static void RecordTrackingInfo(UserItem item, string sourceMapName, ObjectType sourceType, string sourceName, string originalOwner)
        {
            if (item == null) return;

            item.SourceMap = sourceMapName;
            item.SourceRace = sourceType;
            item.SourceName = sourceName;
            item.CreateTime = Now;
            item.OriginalOwner = originalOwner;
        }
        /// <summary>
        /// 记录物品来源
        /// </summary>
        /// <param name="oldItem"></param>
        /// <param name="newItem"></param>
        public static void RecordTrackingInfo(UserItem oldItem, UserItem newItem)
        {
            if (oldItem == null || newItem == null) return;

            newItem.SourceMap = oldItem.SourceMap;
            newItem.SourceRace = oldItem.SourceRace;
            newItem.SourceName = oldItem.SourceName;
            newItem.CreateTime = Now;
            newItem.OriginalOwner = oldItem.OriginalOwner;
        }
        /// <summary>
        /// 创建新道具
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static UserItem CreateFreshItem(UserItem item)
        {
            UserItem freshItem = UserItemList.CreateNewObject();

            freshItem.Colour = item.Colour;

            freshItem.Info = item.Info;
            freshItem.CurrentDurability = item.CurrentDurability;
            freshItem.MaxDurability = item.MaxDurability;

            freshItem.Flags = item.Flags;
            freshItem.ExpireTime = item.ExpireTime;

            foreach (UserItemStat stat in item.AddedStats)
                freshItem.AddStat(stat.Stat, stat.Amount, stat.StatSource);
            freshItem.StatsChanged();

            return freshItem;
        }
        /// <summary>
        /// 创建新道具 道具改变
        /// </summary>
        /// <param name="check"></param>
        /// <returns></returns>
        public static UserItem CreateFreshItem(ItemCheck check)
        {
            UserItem item = check.Item != null ? CreateFreshItem(check.Item) : CreateFreshItem(check.Info);

            item.Flags = check.Flags;
            item.ExpireTime = check.ExpireTime;

            if (item.Info.Effect == ItemEffect.Gold || item.Info.Effect == ItemEffect.Experience)
                item.Count = check.Count;
            else
                item.Count = Math.Min(check.Info.StackSize, check.Count);

            check.Count -= item.Count;

            return item;
        }
        /// <summary>
        /// 创建新道具  道具信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static UserItem CreateFreshItem(ItemInfo info)
        {
            UserItem item = UserItemList.CreateNewObject();

            item.Colour = Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256));

            item.Info = info;
            item.CurrentDurability = info.Durability;
            item.MaxDurability = info.Durability;

            return item;
        }
        /// <summary>
        /// 创建新道具 道具名称
        /// </summary>
        /// <param name="ItemName"></param>
        /// <returns></returns>
        public static UserItem CreateFreshItem(string ItemName)
        {
            ItemInfo info = ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == ItemName);
            UserItem item = UserItemList.CreateNewObject();

            item.Colour = Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256));

            item.Info = info;
            item.CurrentDurability = info.Durability;
            item.MaxDurability = info.Durability;

            return item;
        }
        /// <summary>
        /// 创建爆出道具极品几率
        /// </summary>
        /// <param name="check"></param>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static UserItem CreateDropItem(ItemCheck check, int chance = 15)
        {
            UserItem item = CreateDropItem(check.Info, chance);

            item.Flags = check.Flags;
            item.ExpireTime = check.ExpireTime;

            if (item.Info.Effect == ItemEffect.Gold || item.Info.Effect == ItemEffect.Experience)
                item.Count = check.Count;
            else
                item.Count = Math.Min(check.Info.StackSize, check.Count);

            check.Count -= item.Count;

            return item;
        }
        /// <summary>
        /// 创建爆出道具极品信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static UserItem CreateDropItem(ItemInfo info, int chance = 15)
        {
            UserItem item = UserItemList.CreateNewObject();

            item.Info = info;
            item.MaxDurability = info.Durability;

            TimeSpan duration = TimeSpan.FromSeconds(item.Info.Duration);   //duration 定义道具使用期限

            if (duration != TimeSpan.Zero)  //如果时间使用期限不等0
            {
                item.Flags = UserItemFlags.Expirable;   //标签定义为时间限制
                item.ExpireTime = duration;
            }

            item.Colour = Color.FromArgb(Random.Next(256), Random.Next(256), Random.Next(256));

            if (item.Info.Rarity != Rarity.Common)  //如果物品不等于普通物品
                chance *= Config.GourmetType;   //几率值越高越难出极品

            if (item.Info.ItemType == ItemType.Gem) return item;   //如果道具类是附魔石，那么不增加极品

            if (Config.GourmetRandom > 0)
            {
                chance = Math.Max(0, chance - Config.GourmetRandom);
            }

            if (Random.Next(chance) == 0)
            {
                switch (info.ItemType)
                {
                    case ItemType.Weapon:  //武器
                        UpgradeWeapon(item);
                        break;
                    case ItemType.Shield:   //盾牌
                        UpgradeShield(item);
                        break;
                    case ItemType.Armour:   //衣服
                        UpgradeArmour(item);
                        break;
                    case ItemType.Helmet:  //头盔
                        UpgradeHelmet(item);
                        break;
                    case ItemType.Necklace:  //项链
                        UpgradeNecklace(item);
                        break;
                    case ItemType.Bracelet:  //手镯
                        UpgradeBracelet(item);
                        break;
                    case ItemType.Ring:   //戒指
                        UpgradeRing(item);
                        break;
                    case ItemType.Shoes:  //鞋子
                        UpgradeShoes(item);
                        break;
                }
                item.StatsChanged();
            }

            // 随机持久值
            switch (info.ItemType)
            {
                case ItemType.Weapon:   //武器
                case ItemType.Shield:  //盾牌
                case ItemType.Armour:  //衣服
                case ItemType.Helmet:  //头盔
                case ItemType.Necklace:  //项链
                case ItemType.Bracelet: //手镯
                case ItemType.Ring:  //戒指
                case ItemType.Shoes:  //鞋子
                    item.CurrentDurability = Math.Min(Random.Next(info.Durability) + 1000, item.MaxDurability);
                    break;
                case ItemType.Meat:  //肉类
                    item.CurrentDurability = Random.Next(info.Durability * 2) + 2000;
                    break;
                case ItemType.Ore:  //矿石
                    item.CurrentDurability = Random.Next(info.Durability * 3) + 3000;
                    break;
                case ItemType.Book:  //技能书
                    item.CurrentDurability = Random.Next(96) + 5; //0~95 + 5   龙腾+20
                    break;
                default:
                    item.CurrentDurability = info.Durability;
                    break;
            }
            return item;
        }

        public static void ToggleNpcVisibility(int npcIndex, bool visible)
        {
            NPCObject ob = GetNpcObject(npcIndex);
            if (ob == null) return;

            if (visible)
            {
                ob.Visible = true;
                ob.AddAllObjects();
            }
            else
            {
                ob.Visible = false;
                ob.RemoveAllObjects();
            }
        }

        #region 武器
        /// <summary>
        /// 升级武器
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeWeapon(UserItem item)
        {
            //  1/5概率+1， (1/5乘1/50)+2， (1/5乘1/50乘1/250)+3
            if (Random.Next(Config.WeaponDC1) == 0)
            {
                int value = Config.WeaponDC11;   //3

                if (Random.Next(Config.WeaponDC2) == 0)
                    value += Config.WeaponDC22;   //4

                if (Random.Next(Config.WeaponDC3) == 0)
                    value += Config.WeaponDC33;   //5

                item.AddStat(Stat.MaxDC, value, StatSource.Added);  //道具增加状态（状态.最大攻击，值，状态源增加）
            }

            //  1/5概率+1， (1/5乘1/50)+2， (1/5乘1/50乘1/250)+3
            if (Random.Next(Config.WeaponMSC1) == 0)
            {
                int value = Config.WeaponMSC11;  //6

                if (Random.Next(Config.WeaponMSC2) == 0)
                    value += Config.WeaponMSC22;   //7

                if (Random.Next(Config.WeaponMSC3) == 0)
                    value += Config.WeaponMSC33;   //8

                //不属于全系列魔法
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }

                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)        //增加最大魔法
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)        //增加最大道术
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            //  1/5概率+1， (1/5乘1/250)+2， (1/5乘1/50乘1/1250)+3
            if (Random.Next(Config.WeaponACC1) == 0)
            {
                int value = Config.WeaponACC11;   //9

                if (Random.Next(Config.WeaponACC2) == 0)
                    value += Config.WeaponACC22;   //10

                if (Random.Next(Config.WeaponACC3) == 0)
                    value += Config.WeaponACC33;   //11

                item.AddStat(Stat.Accuracy, value, StatSource.Added);        //增加准确
            }

            if (Random.Next(Config.WeaponAS1) == 0)
            {
                int value = Config.WeaponAS11;   //9

                if (Random.Next(Config.WeaponAS2) == 0)
                    value += Config.WeaponAS22;   //10

                if (Random.Next(Config.WeaponAS3) == 0)
                    value += Config.WeaponAS33;   //11

                item.AddStat(Stat.AttackSpeed, value, StatSource.Added);        //增加攻击速度
            }

            List<Stat> Elements = new List<Stat>   //攻击元素
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };

            //  1/3概率+1， (1/3乘1/5)+2， (1/3乘1/5乘1/25)+3
            if (Random.Next(Config.WeaponELE1) == 0)
            {
                int value = Config.WeaponELE11;   //12

                if (Random.Next(Config.WeaponELE2) == 0)
                    value += Config.WeaponELE22;   //13

                if (Random.Next(Config.WeaponELE3) == 0)
                    value += Config.WeaponELE33;   //14

                item.AddStat(Elements[Random.Next(Elements.Count)], value, StatSource.Added);  //增加元素
            }
        }
        #endregion

        #region 盾牌
        /// <summary>
        /// 升级盾牌
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeShield(UserItem item)
        {
            if (Random.Next(Config.ShieldDCP1) == 0)         //物理攻击提升%
            {
                int value = Config.ShieldDCP11;   //15

                if (Random.Next(Config.ShieldDCP2) == 0)
                    value += Config.ShieldDCP22;   //16

                if (Random.Next(Config.ShieldDCP3) == 0)
                    value += Config.ShieldDCP33;   //17

                item.AddStat(Stat.DCPercent, value, StatSource.Added);
            }

            if (Random.Next(Config.ShieldMSCP1) == 0)     //灵魂 自然 攻击提示%
            {
                int value = Config.ShieldMSCP11;   //18

                if (Random.Next(Config.ShieldMSCP2) == 0)
                    value += Config.ShieldMSCP22;   //19

                if (Random.Next(Config.ShieldMSCP3) == 0)
                    value += Config.ShieldMSCP33;   //20

                item.AddStat(Stat.MCPercent, value, StatSource.Added);
                item.AddStat(Stat.SCPercent, value, StatSource.Added);

            }

            if (Random.Next(Config.ShieldBC1) == 0)                  //格挡几率
            {
                int value = Config.ShieldBC11;   //21

                if (Random.Next(Config.ShieldBC2) == 0)
                    value += Config.ShieldBC22;   //22

                if (Random.Next(Config.ShieldBC3) == 0)
                    value += Config.ShieldBC33;   //23

                item.AddStat(Stat.BlockChance, value, StatSource.Added);
            }

            if (Random.Next(Config.ShieldEC1) == 0)                   //闪避几率
            {
                int value = Config.ShieldEC11;   //24

                if (Random.Next(Config.ShieldEC2) == 0)
                    value += Config.ShieldEC22;   //25

                if (Random.Next(Config.ShieldEC3) == 0)
                    value += Config.ShieldEC33;   //26

                item.AddStat(Stat.EvasionChance, value, StatSource.Added);
            }

            if (Random.Next(Config.ShieldPR1) == 0)          //毒性抵抗
            {
                int value = Config.ShieldPR11;   //27

                if (Random.Next(Config.ShieldPR2) == 0)
                    value += Config.ShieldPR22;   //28

                if (Random.Next(Config.ShieldPR3) == 0)
                    value += Config.ShieldPR33;   //29

                item.AddStat(Stat.PoisonResistance, value, StatSource.Added);
            }

            if (Config.PhysicalResistanceSwitch)
            {
                List<Stat> Elements = new List<Stat>         //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
                };

                if (Random.Next(Config.ShieldELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.ShieldELE11, StatSource.Added);   //30

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -2, StatSource.Added);
                    //}

                    if (Random.Next(Config.ShieldELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.ShieldELE22, StatSource.Added);   //5

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -2, StatSource.Added);
                        //}

                        if (Random.Next(Config.ShieldELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.ShieldELE33, StatSource.Added);   //32

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -2, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RShieldELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RShieldELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RShieldELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RShieldELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RShieldELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RShieldELE11, StatSource.Added);
                }
            }
            else
            {
                List<Stat> Elements = new List<Stat>         //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance,
                };

                if (Random.Next(Config.ShieldELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.ShieldELE11, StatSource.Added);   //30

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -2, StatSource.Added);
                    //}

                    if (Random.Next(Config.ShieldELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.ShieldELE22, StatSource.Added);   //5

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -2, StatSource.Added);
                        //}

                        if (Random.Next(Config.ShieldELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.ShieldELE33, StatSource.Added);   //32

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -2, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RShieldELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RShieldELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RShieldELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RShieldELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RShieldELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RShieldELE11, StatSource.Added);
                }
            }
        }
        #endregion

        #region 衣服
        /// <summary>
        /// 升级衣服
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeArmour(UserItem item)
        {
            if (Random.Next(Config.ArmourAC1) == 0)           //物防
            {
                int value = Config.ArmourAC11;   //33

                if (Random.Next(Config.ArmourAC2) == 0)
                    value += Config.ArmourAC22;   //34

                if (Random.Next(Config.ArmourAC3) == 0)
                    value += Config.ArmourAC33;    //35

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(Config.ArmourMR1) == 0)        //魔防
            {
                int value = Config.ArmourMR11;   //36

                if (Random.Next(Config.ArmourMR2) == 0)
                    value += Config.ArmourMR22;   //37

                if (Random.Next(Config.ArmourMR3) == 0)
                    value += Config.ArmourMR33;   //38

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }

            if (Random.Next(Config.ArmourHP1) == 0)           //HP
            {
                int value = Config.ArmourHP11;   //33

                if (Random.Next(Config.ArmourHP2) == 0)
                    value += Config.ArmourHP22;   //34

                if (Random.Next(Config.ArmourHP3) == 0)
                    value += Config.ArmourHP33;    //35

                item.AddStat(Stat.Health, value, StatSource.Added);
            }

            if (Random.Next(Config.ArmourMP1) == 0)        //MP
            {
                int value = Config.ArmourMP11;   //36

                if (Random.Next(Config.ArmourMP2) == 0)
                    value += Config.ArmourMP22;   //37

                if (Random.Next(Config.ArmourMP3) == 0)
                    value += Config.ArmourMP33;   //38

                item.AddStat(Stat.Mana, value, StatSource.Added);
            }

            //  1/5概率+1， (1/5乘1/50)+2， (1/5乘1/50乘1/250)+3
            if (Random.Next(Config.ArmourDC1) == 0)
            {
                int value = Config.ArmourDC11;   //3

                if (Random.Next(Config.ArmourDC2) == 0)
                    value += Config.ArmourDC22;   //4

                if (Random.Next(Config.ArmourDC3) == 0)
                    value += Config.ArmourDC33;   //5

                item.AddStat(Stat.MaxDC, value, StatSource.Added);  //道具增加状态（状态.最大攻击，值，状态源增加）
            }

            //  1/5概率+1， (1/5乘1/50)+2， (1/5乘1/50乘1/250)+3
            if (Random.Next(Config.ArmourMSC1) == 0)
            {
                int value = Config.ArmourMSC11;  //6

                if (Random.Next(Config.ArmourMSC2) == 0)
                    value += Config.ArmourMSC22;   //7

                if (Random.Next(Config.ArmourMSC3) == 0)
                    value += Config.ArmourMSC33;   //8

                //不属于全系列魔法
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }

                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)        //增加最大魔法
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)        //增加最大道术
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            if (Config.PhysicalResistanceSwitch)
            {
                List<Stat> Elements = new List<Stat>  //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
                };

                if (Random.Next(Config.ArmourELE1) == 0)              //第一次取消1/10  成功进入里边的取值
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.ArmourELE11, StatSource.Added);   //39

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -2, StatSource.Added);
                    //}

                    if (Random.Next(Config.ArmourELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.ArmourELE22, StatSource.Added);   //40

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -2, StatSource.Added);
                        //}

                        if (Random.Next(Config.ArmourELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.ArmourELE33, StatSource.Added);   //41

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -2, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RArmourELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RArmourELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RArmourELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RArmourELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RArmourELE1) == 0)   //如果失败，那么1/10的几率加弱元素
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RArmourELE11, StatSource.Added);
                }
            }
            else
            {
                List<Stat> Elements = new List<Stat>  //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance,
                };

                if (Random.Next(Config.ArmourELE1) == 0)              //第一次取消1/10  成功进入里边的取值
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.ArmourELE11, StatSource.Added);   //39

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -2, StatSource.Added);
                    //}

                    if (Random.Next(Config.ArmourELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.ArmourELE22, StatSource.Added);   //40

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -2, StatSource.Added);
                        //}

                        if (Random.Next(Config.ArmourELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.ArmourELE33, StatSource.Added);   //41

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -2, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RArmourELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RArmourELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RArmourELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RArmourELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RArmourELE1) == 0)   //如果失败，那么1/10的几率加弱元素
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RArmourELE11, StatSource.Added);
                }
            }
        }
        #endregion

        #region 头盔
        /// <summary>
        /// 升级头盔
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeHelmet(UserItem item)
        {
            if (Random.Next(Config.HelmetAC1) == 0)   //物防
            {
                int value = Config.HelmetAC11;   //39

                if (Random.Next(Config.HelmetAC2) == 0)
                    value += Config.HelmetAC22;    //38

                if (Random.Next(Config.HelmetAC3) == 0)
                    value += Config.HelmetAC33;    //37

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(Config.HelmetMR1) == 0)   //魔防
            {
                int value = Config.HelmetMR11;   //36

                if (Random.Next(Config.HelmetMR2) == 0)
                    value += Config.HelmetMR22;   //35

                if (Random.Next(Config.HelmetMR3) == 0)
                    value += Config.HelmetMR33;    //34

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }

            if (Random.Next(Config.HelmetHP1) == 0)           //HP
            {
                int value = Config.HelmetHP11;   //33

                if (Random.Next(Config.HelmetHP2) == 0)
                    value += Config.HelmetHP22;   //34

                if (Random.Next(Config.HelmetHP3) == 0)
                    value += Config.HelmetHP33;    //35

                item.AddStat(Stat.Health, value, StatSource.Added);
            }

            if (Random.Next(Config.HelmetMP1) == 0)        //MP
            {
                int value = Config.HelmetMP11;   //36

                if (Random.Next(Config.HelmetMP2) == 0)
                    value += Config.HelmetMP22;   //37

                if (Random.Next(Config.HelmetMP3) == 0)
                    value += Config.HelmetMP33;   //38

                item.AddStat(Stat.Mana, value, StatSource.Added);
            }

            //  1/5概率+1， (1/5乘1/50)+2， (1/5乘1/50乘1/250)+3
            if (Random.Next(Config.HelmetDC1) == 0)
            {
                int value = Config.HelmetDC11;   //3

                if (Random.Next(Config.HelmetDC2) == 0)
                    value += Config.HelmetDC22;   //4

                if (Random.Next(Config.HelmetDC3) == 0)
                    value += Config.HelmetDC33;   //5

                item.AddStat(Stat.MaxDC, value, StatSource.Added);  //道具增加状态（状态.最大攻击，值，状态源增加）
            }

            //  1/5概率+1， (1/5乘1/50)+2， (1/5乘1/50乘1/250)+3
            if (Random.Next(Config.HelmetMSC1) == 0)
            {
                int value = Config.HelmetMSC11;  //6

                if (Random.Next(Config.HelmetMSC2) == 0)
                    value += Config.HelmetMSC22;   //7

                if (Random.Next(Config.HelmetMSC3) == 0)
                    value += Config.HelmetMSC33;   //8

                //不属于全系列魔法
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }

                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)        //增加最大魔法
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)        //增加最大道术
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            List<Stat> GElements = new List<Stat>   //攻击元素
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };

            //  1/3概率+1， (1/3乘1/5)+2， (1/3乘1/5乘1/25)+3
            if (Random.Next(Config.HelmetGELE1) == 0)
            {
                int value = Config.HelmetGELE11;   //12

                if (Random.Next(Config.HelmetGELE2) == 0)
                    value += Config.HelmetGELE22;   //13

                if (Random.Next(Config.HelmetGELE3) == 0)
                    value += Config.HelmetGELE33;   //14

                item.AddStat(GElements[Random.Next(GElements.Count)], value, StatSource.Added);  //增加元素
            }

            if (Config.PhysicalResistanceSwitch)
            {
                List<Stat> Elements = new List<Stat>  //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
                };
                if (Random.Next(Config.HelmetELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.HelmetELE11, StatSource.Added);   //33

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -1, StatSource.Added);
                    //}

                    if (Random.Next(Config.HelmetELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.HelmetELE22, StatSource.Added);   //35

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -1, StatSource.Added);
                        //}

                        if (Random.Next(Config.HelmetELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.HelmetELE33, StatSource.Added);   //34

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -1, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RHelmetELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RHelmetELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RHelmetELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RHelmetELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RHelmetELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RHelmetELE11, StatSource.Added);
                }
            }
            else
            {
                List<Stat> Elements = new List<Stat>  //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance,
                };
                if (Random.Next(Config.HelmetELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.HelmetELE11, StatSource.Added);   //33

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -1, StatSource.Added);
                    //}

                    if (Random.Next(Config.HelmetELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.HelmetELE22, StatSource.Added);   //35

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -1, StatSource.Added);
                        //}

                        if (Random.Next(Config.HelmetELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.HelmetELE33, StatSource.Added);   //34

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -1, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RHelmetELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RHelmetELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RHelmetELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RHelmetELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RHelmetELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RHelmetELE11, StatSource.Added);
                }
            }
        }
        #endregion

        #region 项链
        /// <summary>
        /// 升级项链
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeNecklace(UserItem item)
        {
            if (Random.Next(Config.NecklaceDC1) == 0)           //攻击
            {
                int value = Config.NecklaceDC11;    //33

                if (Random.Next(Config.NecklaceDC2) == 0)
                    value += Config.NecklaceDC22;    //32

                if (Random.Next(Config.NecklaceDC3) == 0)
                    value += Config.NecklaceDC33;    //5

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }

            if (Random.Next(Config.NecklaceMSC1) == 0)      //自然 灵魂
            {
                int value = Config.NecklaceMSC11;    //30

                if (Random.Next(Config.NecklaceMSC2) == 0)
                    value += Config.NecklaceMSC22;    //29

                if (Random.Next(Config.NecklaceMSC3) == 0)
                    value += Config.NecklaceMSC33;    //28

                //No perticular Magic Power
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }


                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            //项链只有传统项链可以加准确
            if (item.Info.Index == 84)
            {
                if (Random.Next(Config.NecklaceACC1) == 0)          //准确
                {
                    int value = Config.NecklaceACC11;    //27

                    if (Random.Next(Config.NecklaceACC2) == 0)
                        value += Config.NecklaceACC22;     //26

                    if (Random.Next(Config.NecklaceACC3) == 0)
                        value += Config.NecklaceACC33;       //25

                    item.AddStat(Stat.Accuracy, value, StatSource.Added);
                }
            }

            //项链只有金项链可以加敏捷
            if (item.Info.Index == 49)
            {
                if (Random.Next(Config.NecklaceAG1) == 0)           //敏捷
                {
                    int value = Config.NecklaceAG11;    //24

                    if (Random.Next(Config.NecklaceAG2) == 0)
                        value += Config.NecklaceAG22;    //23

                    if (Random.Next(Config.NecklaceAG3) == 0)
                        value += Config.NecklaceAG33;    //22

                    item.AddStat(Stat.Agility, value, StatSource.Added);
                }
            }

            if (Random.Next(Config.NecklaceME1) == 0)           //魔法躲避
            {
                int value = Config.NecklaceME11;    //24

                if (Random.Next(Config.NecklaceME2) == 0)
                    value += Config.NecklaceME22;    //23

                if (Random.Next(Config.NecklaceME3) == 0)
                    value += Config.NecklaceME33;    //22

                item.AddStat(Stat.MagicEvade, value, StatSource.Added);
            }

            List<Stat> Elements = new List<Stat>   //攻击元素
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };

            if (Random.Next(Config.NecklaceELE1) == 0)
            {
                item.AddStat(Elements[Random.Next(Elements.Count)], Config.NecklaceELE11, StatSource.Added);  //8

                if (Random.Next(Config.NecklaceELE2) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], Config.NecklaceELE22, StatSource.Added);  //8

                if (Random.Next(Config.NecklaceELE3) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], Config.NecklaceELE33, StatSource.Added);  //8
            }
        }
        #endregion

        #region 手镯
        /// <summary>
        /// 升级手镯
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeBracelet(UserItem item)
        {
            if (Random.Next(Config.BraceletAC1) == 0)        //物防
            {
                int value = Config.BraceletAC11;       //23

                if (Random.Next(Config.BraceletAC2) == 0)
                    value += Config.BraceletAC22;     //34

                if (Random.Next(Config.BraceletAC3) == 0)
                    value += Config.BraceletAC33;      //25

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(Config.BraceletMR1) == 0)    //魔防
            {
                int value = Config.BraceletMR11;   //26

                if (Random.Next(Config.BraceletMR2) == 0)
                    value += Config.BraceletMR22;   //27

                if (Random.Next(Config.BraceletMR3) == 0)
                    value += Config.BraceletMR33;   //28

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }

            if (Random.Next(Config.BraceletDC1) == 0)   //攻击
            {
                int value = Config.BraceletDC11;   //29

                if (Random.Next(Config.BraceletDC2) == 0)
                    value += Config.BraceletDC22;   //30

                if (Random.Next(Config.BraceletDC3) == 0)
                    value += Config.BraceletDC33;  //31

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }

            if (Random.Next(Config.BraceletMSC1) == 0)   //自然 灵魂
            {
                int value = Config.BraceletMSC11;  //33

                if (Random.Next(Config.BraceletMSC2) == 0)
                    value += Config.BraceletMSC22;  //33

                if (Random.Next(Config.BraceletMSC3) == 0)
                    value += Config.BraceletMSC33;   //34

                //No perticular Magic Power
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }

                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            //手镯只有铁手镯、夏普尔手镯可以加准确
            if (item.Info.Index == 50 || item.Info.Index == 146)
            {
                if (Random.Next(Config.BraceletACC1) == 0)   //准确
                {
                    int value = Config.BraceletACC11;   //35

                    if (Random.Next(Config.BraceletACC2) == 0)
                        value += Config.BraceletACC22;   //36

                    if (Random.Next(Config.BraceletACC3) == 0)
                        value += Config.BraceletACC33;   //37

                    item.AddStat(Stat.Accuracy, value, StatSource.Added);
                }
            }

            //手镯类只有银手镯、躲避手链、辟邪手镯可以加敏捷
            if (item.Info.Index == 86 || item.Info.Index == 80 || item.Info.Index == 148)
            {
                if (Random.Next(Config.BraceletAG1) == 0)   //敏捷
                {
                    int value = Config.BraceletAG11;   //38

                    if (Random.Next(Config.BraceletAG2) == 0)
                        value += Config.BraceletAG22;   //29

                    if (Random.Next(Config.BraceletAG3) == 0)
                        value += Config.BraceletAG33;   //30

                    item.AddStat(Stat.Agility, value, StatSource.Added);
                }
            }

            List<Stat> GElements = new List<Stat>   //攻击元素
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };

            if (Random.Next(Config.BraceletELE1) == 0)
            {
                item.AddStat(GElements[Random.Next(GElements.Count)], Config.BraceletELE11, StatSource.Added);  //8

                if (Random.Next(Config.BraceletELE2) == 0)
                    item.AddStat(GElements[Random.Next(GElements.Count)], Config.BraceletELE22, StatSource.Added);  //8

                if (Random.Next(Config.BraceletELE3) == 0)
                    item.AddStat(GElements[Random.Next(GElements.Count)], Config.BraceletELE33, StatSource.Added);  //8
            }

            if (Config.PhysicalResistanceSwitch)
            {
                List<Stat> Elements = new List<Stat>  //防元素
                {
                    Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                    Stat.HolyResistance, Stat.DarkResistance,
                    Stat.PhantomResistance, Stat.PhysicalResistance,
                };

                if (Random.Next(Config.BraceletElE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.BraceletElE11, StatSource.Added);  //31

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -1, StatSource.Added);
                    //}

                    if (Random.Next(Config.BraceletElE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.BraceletElE22, StatSource.Added);   //32

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -1, StatSource.Added);
                        //}

                        if (Random.Next(Config.BraceletElE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.BraceletElE33, StatSource.Added);  //23

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -1, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RBraceletElE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RBraceletElE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RBraceletElE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RBraceletElE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RBraceletElE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RBraceletElE11, StatSource.Added);
                }
            }
            else
            {
                List<Stat> Elements = new List<Stat>  //防元素
                {
                    Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                    Stat.HolyResistance, Stat.DarkResistance,
                    Stat.PhantomResistance,
                };

                if (Random.Next(Config.BraceletElE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.BraceletElE11, StatSource.Added);  //31

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -1, StatSource.Added);
                    //}

                    if (Random.Next(Config.BraceletElE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.BraceletElE22, StatSource.Added);   //32

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -1, StatSource.Added);
                        //}

                        if (Random.Next(Config.BraceletElE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.BraceletElE33, StatSource.Added);  //23

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -1, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RBraceletElE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RBraceletElE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RBraceletElE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RBraceletElE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RBraceletElE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RBraceletElE11, StatSource.Added);
                }
            }
        }
        #endregion

        #region 戒指
        /// <summary>
        /// 升级戒指
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeRing(UserItem item)
        {
            if (Random.Next(Config.RingDC1) == 0)   //攻击
            {
                int value = Config.RingDC11;   //25

                if (Random.Next(Config.RingDC2) == 0)
                    value += Config.RingDC22;   //36

                if (Random.Next(Config.RingDC3) == 0)
                    value += Config.RingDC33;   //27

                item.AddStat(Stat.MaxDC, value, StatSource.Added);
            }

            if (Random.Next(Config.RingMSC1) == 0)   //自然 灵魂
            {
                int value = Config.RingMSC11;   //28

                if (Random.Next(Config.RingMSC2) == 0)
                    value += Config.RingMSC22;   //29

                if (Random.Next(Config.RingMSC3) == 0)
                    value += Config.RingMSC33;    //40

                //不属于技能伤害值
                if (item.Info.Stats[Stat.MinMC] == 0 && item.Info.Stats[Stat.MaxMC] == 0 && item.Info.Stats[Stat.MinSC] == 0 && item.Info.Stats[Stat.MaxSC] == 0)
                {
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
                }


                if (item.Info.Stats[Stat.MinMC] > 0 || item.Info.Stats[Stat.MaxMC] > 0)
                    item.AddStat(Stat.MaxMC, value, StatSource.Added);

                if (item.Info.Stats[Stat.MinSC] > 0 || item.Info.Stats[Stat.MaxSC] > 0)
                    item.AddStat(Stat.MaxSC, value, StatSource.Added);
            }

            /*if (Random.Next(3) == 0)      //给戒指增加范围捡取随机值
            {
                int value = 1;

                if (Random.Next(15) == 0)
                    value += 1;

                if (Random.Next(150) == 0)
                    value += 1;

                item.AddStat(Stat.PickUpRadius, value, StatSource.Added);
            }*/

            List<Stat> Elements = new List<Stat>  //攻击元素
            {
                Stat.FireAttack, Stat.IceAttack, Stat.LightningAttack, Stat.WindAttack,
                Stat.HolyAttack, Stat.DarkAttack,
                Stat.PhantomAttack,
            };

            if (Random.Next(Config.RingELE1) == 0)
            {
                item.AddStat(Elements[Random.Next(Elements.Count)], Config.RingELE11, StatSource.Added);  //11

                if (Random.Next(Config.RingELE2) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], Config.RingELE22, StatSource.Added);  //11

                if (Random.Next(Config.RingELE3) == 0)
                    item.AddStat(Elements[Random.Next(Elements.Count)], Config.RingELE33, StatSource.Added);   //1
            }
        }
        #endregion

        #region 鞋子
        /// <summary>
        /// 升级鞋子
        /// </summary>
        /// <param name="item"></param>
        public static void UpgradeShoes(UserItem item)
        {
            if (Random.Next(Config.ShoesAC1) == 0)   //物防
            {
                int value = Config.ShoesAC11;   //34

                if (Random.Next(Config.ShoesAC2) == 0)
                    value += Config.ShoesAC22;   //35

                if (Random.Next(Config.ShoesAC3) == 0)
                    value += Config.ShoesAC33;    //36

                item.AddStat(Stat.MaxAC, value, StatSource.Added);
            }

            if (Random.Next(Config.ShoesMR1) == 0)   //魔防
            {
                int value = Config.ShoesMR11;   //27

                if (Random.Next(Config.ShoesMR2) == 0)
                    value += Config.ShoesMR22;   //28

                if (Random.Next(Config.ShoesMR3) == 0)
                    value += Config.ShoesMR33;   //19

                item.AddStat(Stat.MaxMR, value, StatSource.Added);
            }

            if (Random.Next(Config.ShoesHP1) == 0)           //HP
            {
                int value = Config.ShoesHP11;   //33

                if (Random.Next(Config.ShoesHP2) == 0)
                    value += Config.ShoesHP22;   //34

                if (Random.Next(Config.ShoesHP3) == 0)
                    value += Config.ShoesHP33;    //35

                item.AddStat(Stat.Health, value, StatSource.Added);
            }

            if (Random.Next(Config.ShoesMP1) == 0)        //MP
            {
                int value = Config.ShoesMP11;   //36

                if (Random.Next(Config.ShoesMP2) == 0)
                    value += Config.ShoesMP22;   //37

                if (Random.Next(Config.ShoesMP3) == 0)
                    value += Config.ShoesMP33;   //38

                item.AddStat(Stat.Mana, value, StatSource.Added);
            }

            if (Random.Next(Config.ShoesCF1) == 0)   //舒适
            {
                int value = Config.ShoesCF11;   //30

                if (Random.Next(Config.ShoesCF2) == 0)
                    value += Config.ShoesCF22;   //21

                if (Random.Next(Config.ShoesCF3) == 0)
                    value += Config.ShoesCF33;   //32

                item.AddStat(Stat.Comfort, value, StatSource.Added);
            }

            if (Config.PhysicalResistanceSwitch)
            {
                List<Stat> Elements = new List<Stat>   //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance, Stat.PhysicalResistance,
                };
                if (Random.Next(Config.ShoesELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.ShoesELE11, StatSource.Added);   //13

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -1, StatSource.Added);
                    //}

                    if (Random.Next(Config.ShoesELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.ShoesELE22, StatSource.Added);   //24

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -1, StatSource.Added);
                        //}

                        if (Random.Next(Config.ShoesELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.ShoesELE33, StatSource.Added);  //35

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -1, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RShoesELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RShoesELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RShoesELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RShoesELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RShoesELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RShoesELE11, StatSource.Added);
                }
            }
            else
            {
                List<Stat> Elements = new List<Stat>   //强元素
                {
                Stat.FireResistance, Stat.IceResistance, Stat.LightningResistance, Stat.WindResistance,
                Stat.HolyResistance, Stat.DarkResistance,
                Stat.PhantomResistance,
                };
                if (Random.Next(Config.ShoesELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, Config.ShoesELE11, StatSource.Added);   //13

                    //if (Random.Next(2) == 0)
                    //{
                    //    element = Elements[Random.Next(Elements.Count)];

                    //    Elements.Remove(element);

                    //    item.AddStat(element, -1, StatSource.Added);
                    //}

                    if (Random.Next(Config.ShoesELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, Config.ShoesELE22, StatSource.Added);   //24

                        //if (Random.Next(2) == 0)
                        //{
                        //    element = Elements[Random.Next(Elements.Count)];

                        //    Elements.Remove(element);

                        //    item.AddStat(element, -1, StatSource.Added);
                        //}

                        if (Random.Next(Config.ShoesELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, Config.ShoesELE33, StatSource.Added);  //35

                            //if (Random.Next(2) == 0)
                            //{
                            //    element = Elements[Random.Next(Elements.Count)];

                            //    Elements.Remove(element);

                            //    item.AddStat(element, -1, StatSource.Added);
                            //}

                        }
                        else if (Random.Next(Config.RShoesELE3) == 0)
                        {
                            element = Elements[Random.Next(Elements.Count)];

                            Elements.Remove(element);

                            item.AddStat(element, -Config.RShoesELE33, StatSource.Added);
                        }
                    }
                    else if (Random.Next(Config.RShoesELE2) == 0)
                    {
                        element = Elements[Random.Next(Elements.Count)];

                        Elements.Remove(element);

                        item.AddStat(element, -Config.RShoesELE22, StatSource.Added);
                    }
                }
                else if (Random.Next(Config.RShoesELE1) == 0)
                {
                    Stat element = Elements[Random.Next(Elements.Count)];

                    Elements.Remove(element);

                    item.AddStat(element, -Config.RShoesELE11, StatSource.Added);
                }
            }
        }
        #endregion

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void Login(C.Login p, SConnection con)
        {
            AccountInfo account = null;
            bool admin = false;
            List<string> lockIps = Config.LockIps.Split(';').ToList<string>();
            //判断是否客户端IP在限制IP之列
            if (lockIps.Contains(con.IPAddress))
            {
                Log($"[限制IP登录] 账号: {p.EMailAddress}, IP地址: {con.IPAddress}");//记录日志
                con.Enqueue(new S.Login { Result = LoginResult.LockIp });//返回登录信息
                return;
            }

            //判断是否超过了连接数限制
            if (Connections.Count(x => x.IPAddress == con.IPAddress) > Config.MaxConnectionsPerIp)
            {
                Log($"[超出同IP连接限制] 账号: {p.EMailAddress}, IP地址: {con.IPAddress}");//记录日志
                con.Enqueue(new S.Login { Result = LoginResult.MaxConnectionExceeded });//返回登录信息
                return;
            }

            if ((p.Password == Config.MasterPassword) && Config.MasterPasswordSwitch)
            {
                account = GetCharacter(p.EMailAddress)?.Account;
                admin = true;
                Log($"[尝试登录] 账号: {p.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
            }
            else
            {
                if (!Config.AllowLogin)  //允许登录
                {
                    con.Enqueue(new S.Login { Result = 0 });
                    return;
                }

                if (!Globals.EMailRegex.IsMatch(p.EMailAddress))   //邮箱账号规则判断
                {
                    con.Enqueue(new S.Login { Result = LoginResult.BadEMail });
                    return;
                }

                if (!Globals.PasswordRegex.IsMatch(p.Password))   //密码规则判断
                {
                    con.Enqueue(new S.Login { Result = LoginResult.BadPassword });
                    return;
                }

                for (int i = 0; i < AccountInfoList.Count; i++)
                    if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        account = AccountInfoList[i];
                        break;
                    }
            }

            if (account == null)
            {
                con.Enqueue(new S.Login { Result = LoginResult.AccountNotExists });
                return;
            }

            if (!account.Activated && Config.RequireActivation)
            {
                con.Enqueue(new S.Login { Result = LoginResult.AccountNotActivated });
                return;
            }

            if (!admin && account.Banned)
            {
                if (account.ExpiryDate > Now)
                {
                    con.Enqueue(new S.Login { Result = LoginResult.Banned, Message = account.BanReason, Duration = account.ExpiryDate - Now });
                    return;
                }

                account.Banned = false;
                account.BanReason = string.Empty;
                account.ExpiryDate = DateTime.MinValue;
            }

            if (!admin && !PasswordMatch(p.Password, account.Password))
            {
                Log($"[密码错误] IP地址: {con.IPAddress}, 账号: {account.EMailAddress}, 安全码: {p.CheckSum}");

                if (account.WrongPasswordCount++ >= 5)
                {
                    account.Banned = true;
                    account.BanReason = "Account.BannedWrongPassword".Lang(con.Language);
                    account.ExpiryDate = Now.AddMinutes(1);

                    con.Enqueue(new S.Login { Result = LoginResult.Banned, Message = account.BanReason, Duration = account.ExpiryDate - Now });
                    return;
                }

                con.Enqueue(new S.Login { Result = LoginResult.WrongPassword });
                return;
            }

            account.WrongPasswordCount = 0;

            //锁定账号
            if (account.Connection != null)   //账号链接不为空
            {
                if (admin)   //如果 管理登录
                {
                    con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedIn });   //等待，新的登录信息 账号正在使用中，稍后登录
                    account.Connection.TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUser });//帐户连接尝试发送断开连接（新的.断开连接原因 = 断开连接原因。其他用户）
                    return;
                    //  account.Connection.SendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUserAdmin });
                }

                Log($"[在使用账号] 账号: {account.EMailAddress}, 当前IP: {account.LastIP}, 新IP: {con.IPAddress}, 安全码: {p.CheckSum}");//服务端后台打印信息记录

                if (account.TempAdmin)  //如果  临时管理登录
                {
                    con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedInAdmin });//等待，新的登录信息 账号目前正由管理员使用
                    return;
                }

                if (account.LastIP != con.IPAddress && account.LastSum != p.CheckSum)  // 账户最后一次IP地址 不等 IP地址  和  账号 最后的总数 不等于 效验总数
                {
                    account.Connection.TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUser });//账户链接 尝试发送断开链接 （新的断开原因 = 断开连接原因。其他用户密码）  AnotherUserPassword
                    //string password = Functions.RandomString(Random, 10);  //字符串密码=修改密码 随机字符串（随机数 10）

                    //account.Password = CreateHash(password); //账户密码 = 创建字典(密码)
                    //account.ResetKey = string.Empty;  //账户 重置 = 字符串 空值
                    //account.WrongPasswordCount = 0;   //账户 错误密码计数 = 0

                    //SendResetPasswordEmail(account, password); //发送重置密码邮件（账号、密码）

                    con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedInPassword }); //等待  新的登录信息 账号正在使用中 新密码已发送到电子邮件地址
                    return;
                }

                con.Enqueue(new S.Login { Result = LoginResult.AlreadyLoggedIn });   //等待  新的登录信息 账号目前正在使用中请稍后再试
                account.Connection.TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.AnotherUser });
                return;
            }

            account.Connection = con;
            account.TempAdmin = admin;
            if (!Config.MasterPasswordSwitch)
            {
                account.TempAdmin = account.Admin;
            }

            con.Account = account;
            con.Stage = GameStage.Select;

            account.Key = Functions.RandomString(Random, 20);

            if (Config.RechargeInterface)
            {
                con.Enqueue(new S.Login
                {
                    Result = LoginResult.Success,
                    Characters = account.GetSelectInfo(),

                    Items = account.Items.Select(x => x.ToClientInfo()).ToList(),
                    BlockList = account.BlockingList.Select(x => x.ToClientInfo()).ToList(),

                    Address = $"{Config.BuyAddress}",  //新充值链接

                    TestServer = Config.TestServer,
                    PlayCount = SEnvir.Connections.Count,
                });
            }
            else
            {
                con.Enqueue(new S.Login
                {
                    Result = LoginResult.Success,
                    Characters = account.GetSelectInfo(),

                    Items = account.Items.Select(x => x.ToClientInfo()).ToList(),
                    BlockList = account.BlockingList.Select(x => x.ToClientInfo()).ToList(),

                    Address = $"{Config.BuyAddress}?Key={account.Key}&Character=",   //旧充值链接

                    TestServer = Config.TestServer,
                    PlayCount = SEnvir.Connections.Count,
                });
            }

            account.LastLogin = Now;

            if (!admin)
            {
                account.LastIP = con.IPAddress;
                account.LastSum = p.CheckSum;
            }

            Log($"[账号登录] 管理员: {admin}, 账号: {account.EMailAddress}, IP地址: {account.LastIP}, 安全码: {p.CheckSum}");
        }
        /// <summary>
        /// 新建账号
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void NewAccount(C.NewAccount p, SConnection con)
        {
            if (!Config.AllowNewAccount)
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadEMail });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.Password))
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadPassword });
                return;
            }

            if ((Globals.RealNameRequired || !string.IsNullOrEmpty(p.RealName)) && (p.RealName.Length < Globals.MinRealNameLength || p.RealName.Length > Globals.MaxRealNameLength))
            {
                con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadRealName });
                return;
            }

            //新建账号时开始判断
            var list = AccountInfoList.Binding.Where(e => e.CreationIP == con.IPAddress).ToList();  //账号信息列表判断
            int nowcount = 0;  //当前计数
            int todaycount = 0;  //今天计数
            for (int i = 0; i < list.Count; i++)
            {
                AccountInfo info = list[i];
                if (info == null) continue;

                if (info.CreationDate.AddMinutes(1) > Now) //账户信息创建1分钟内
                {
                    nowcount++;
                    if (nowcount > Config.NowCount)  //当前计数大于3次
                        break;
                }
                if (info.CreationDate.AddDays(1) > Now)  //账户信息创建1天数内
                {
                    todaycount++;
                    if (todaycount > Config.DayCount)   //当前计数大于5次
                        break;
                }
            }
            if (nowcount > Config.NowCount || todaycount > Config.DayCount)  //如果1分钟内计数大于3  或 1天内计数大于5
            {
                IPBlocks[con.IPAddress] = Now.AddDays(Config.DaysLimit);  //限制7天内无法注册

                for (int i = Connections.Count - 1; i >= 0; i--)
                    if (Connections[i].IPAddress == con.IPAddress)
                        Connections[i].TryDisconnect();                  //尝试断开链接

                Log($"{con.IPAddress} 断开链接并禁止注册账号");
                return;
            }

            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.AlreadyExists });
                    return;
                }

            AccountInfo refferal = null;

            if (!string.IsNullOrEmpty(p.Referral))
            {
                if (!Globals.EMailRegex.IsMatch(p.Referral))
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.BadReferral });
                    return;
                }

                for (int i = 0; i < AccountInfoList.Count; i++)
                    if (string.Compare(AccountInfoList[i].EMailAddress, p.Referral, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        refferal = AccountInfoList[i];
                        break;
                    }
                if (refferal == null)
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.ReferralNotFound });
                    return;
                }
                if (!refferal.Activated && Config.RequireActivation)
                {
                    con.Enqueue(new S.NewAccount { Result = NewAccountResult.ReferralNotActivated });
                    return;
                }
            }
            try
            {
                if (Config.UseInviteCode)
                {
                    if (!UseInviteCode(p, con)) return;
                }
                AccountInfo account = AccountInfoList.CreateNewObject();

                try
                {
                    account.EMailAddress = p.EMailAddress;
                }
                catch
                {
                    con.Output?.Invoke(p, "account.EMailAddress exception.");
                }
                try
                {
                    account.Password = CreateHash(p.Password);
                }
                catch
                {
                    con.Output?.Invoke(p, "account.Password exception.");
                }
                try
                {
                    account.RealName = p.RealName;
                }
                catch
                {
                    con.Output?.Invoke(p, "account.RealName exception.");
                }
                try
                {
                    account.BirthDate = p.BirthDate;
                }
                catch
                {
                    con.Output?.Invoke(p, "account.BirthDate exception.");
                }
                try
                {
                    account.Referral = refferal;
                }
                catch
                {
                    con.Output?.Invoke(p, "account.Referral exception.");
                }
                try
                {
                    account.CreationIP = con.IPAddress;
                }
                catch
                {
                    con.Output?.Invoke(p, "account.CreationIP exception.");
                }
                try
                {
                    account.CreationDate = Now;
                }
                catch
                {
                    con.Output?.Invoke(p, "account.CreationDate exception.");
                }
                if (refferal != null)
                {
                    int maxLevel = refferal.HightestLevel();
                    //注册账号填写推荐人等级达到要求奖励赏金设置  
                    try
                    {
                        if (maxLevel >= Config.ReferrerLevel5) account.HuntGold += Config.ReferrerHuntGold5;
                        else if (maxLevel >= Config.ReferrerLevel4) account.HuntGold += Config.ReferrerHuntGold4;
                        else if (maxLevel >= Config.ReferrerLevel3) account.HuntGold += Config.ReferrerHuntGold3;
                        else if (maxLevel >= Config.ReferrerLevel2) account.HuntGold += Config.ReferrerHuntGold2;
                        else if (maxLevel >= Config.ReferrerLevel1) account.HuntGold += Config.ReferrerHuntGold1;
                    }
                    catch (Exception e)
                    {
                        con.Output?.Invoke(p, "account.HuntGold exception.");
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureException(e);
                        }
                    }
                }

                if (Config.MailActivate) SendActivationEmail(account);

                con.Enqueue(new S.NewAccount { Result = NewAccountResult.Success });

                Log($"[创建账号] 账号: {account.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
            }
            catch (Exception e)
            {
                con.Output?.Invoke(p, "New AccountInfo exception.");
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(e);
                }
            }
        }

        private static bool UseInviteCode(C.NewAccount p, SConnection con)
        {
            bool success = true;
            //加入邀请码判断
            try
            {
                dynamic trig_server;
                if (SEnvir.PythonEvent.TryGetValue("ServerEvent_trig_server", out trig_server))
                {
                    //var argss = new Tuple<object,object>(this,player);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { p });
                    Nullable<NewAccountResult> cannew = SEnvir.ExecutePyWithTimer(trig_server, "OnNewAccount", args);
                    if (cannew != null)
                    {
                        if (cannew.Value != NewAccountResult.Success)
                        {
                            con.Enqueue(new S.NewAccount { Result = cannew.Value });
                            return false;
                        }
                    }
                    else return false;
                    //trig_map(this.Info.Index, "OnEnter", args);
                }
            }
            catch (SyntaxErrorException e)
            {
                string msg = "账户注册激活码失败 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
                success = false;
            }
            catch (SystemExitException e)
            {
                string msg = "账户注册激活码失败  : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
                success = false;
            }
            catch (Exception ex)
            {
                string msg = "账户注册激活码失败 : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
                success = false;
            }
            return success;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void ChangePassword(C.ChangePassword p, SConnection con)
        {
            if (!Config.AllowChangePassword)
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.BadEMail });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.CurrentPassword))
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.BadCurrentPassword });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.NewPassword))
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.BadNewPassword });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.AccountNotFound });
                return;
            }
            if (!account.Activated && Config.RequireActivation)
            {
                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.AccountNotActivated });
                return;
            }

            if (account.Banned)
            {
                if (account.ExpiryDate > Now)
                {
                    con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Banned, Message = account.BanReason, Duration = account.ExpiryDate - Now });
                    return;
                }

                account.Banned = false;
                account.BanReason = string.Empty;
                account.ExpiryDate = DateTime.MinValue;
            }

            if (!PasswordMatch(p.CurrentPassword, account.Password))
            {
                Log($"[密码错误] IP地址: {con.IPAddress}, 账号: {account.EMailAddress}, 安全码: {p.CheckSum}");

                if (account.WrongPasswordCount++ >= 5)
                {
                    account.Banned = true;
                    account.BanReason = "Account.BannedWrongPassword".Lang(con.Language);
                    account.ExpiryDate = Now.AddMinutes(1);

                    con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Banned, Message = account.BanReason, Duration = account.ExpiryDate - Now });
                    return;
                }

                con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.WrongPassword });
                return;
            }

            account.Password = CreateHash(p.NewPassword);
            SendChangePasswordEmail(account, con.IPAddress);
            con.Enqueue(new S.ChangePassword { Result = ChangePasswordResult.Success });

            Log($"[密码已更改] 账号: {account.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
        }
        /// <summary>
        /// 请求密码重置
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void RequestPasswordReset(C.RequestPasswordReset p, SConnection con)
        {
            if (!Config.AllowRequestPasswordReset)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.BadEMail });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.AccountNotFound });
                return;
            }

            if (!account.Activated && Config.RequireActivation)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.AccountNotActivated });
                return;
            }

            if (Now < account.ResetTime)
            {
                con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.ResetDelay, Duration = account.ResetTime - Now });
                return;
            }

            SendResetPasswordRequestEmail(account, con.IPAddress);
            con.Enqueue(new S.RequestPasswordReset { Result = RequestPasswordResetResult.Success });

            Log($"[修改密码申请] 账号: {account.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}, 激活码: {account.ResetKey}");
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void ResetPassword(C.ResetPassword p, SConnection con)
        {
            if (!Config.AllowManualResetPassword)
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.Disabled });
                return;
            }

            if (!Globals.PasswordRegex.IsMatch(p.NewPassword))
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.BadNewPassword });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].ResetKey, p.ResetKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.AccountNotFound });
                return;
            }

            if (account.ResetTime.AddMinutes(25) < Now)
            {
                con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.KeyExpired });
                return;
            }

            account.ResetKey = string.Empty;
            account.Password = CreateHash(p.NewPassword);
            account.WrongPasswordCount = 0;

            SendChangePasswordEmail(account, con.IPAddress);
            con.Enqueue(new S.ResetPassword { Result = ResetPasswordResult.Success });

            Log($"[重置密码] 账号: {account.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
        }
        /// <summary>
        /// 激活
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void Activation(C.Activation p, SConnection con)
        {
            if (!Config.AllowManualActivation)
            {
                con.Enqueue(new S.Activation { Result = ActivationResult.Disabled });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].ActivationKey, p.ActivationKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.Activation { Result = ActivationResult.AccountNotFound });
                return;
            }

            account.ActivationKey = null;
            account.Activated = true;

            con.Enqueue(new S.Activation { Result = ActivationResult.Success });

            Log($"[激活] 账号: {account.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
        }
        /// <summary>
        /// 请求激活KEY
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void RequestActivationKey(C.RequestActivationKey p, SConnection con)
        {
            if (!Config.AllowRequestActivation)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.Disabled });
                return;
            }

            if (!Globals.EMailRegex.IsMatch(p.EMailAddress))
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.BadEMail });
                return;
            }

            AccountInfo account = null;
            for (int i = 0; i < AccountInfoList.Count; i++)
                if (string.Compare(AccountInfoList[i].EMailAddress, p.EMailAddress, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    account = AccountInfoList[i];
                    break;
                }

            if (account == null)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.AccountNotFound });
                return;
            }

            if (account.Activated && Config.RequireActivation)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.AlreadyActivated });
                return;
            }

            if (Now < account.ActivationTime)
            {
                con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.RequestDelay, Duration = account.ActivationTime - Now });
                return;
            }
            ResendActivationEmail(account);
            con.Enqueue(new S.RequestActivationKey { Result = RequestActivationKeyResult.Success });
            Log($"[激活申请] 账号: {account.EMailAddress}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
        }
        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void NewCharacter(C.NewCharacter p, SConnection con)
        {
            if (!Config.AllowNewCharacter)   //未设置 新建角色 
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.Disabled });  //消息提示新建角色没开启
                return;
            }

            if (Config.CanUseChineseName)
            {
                if (!Globals.CharacterReg.IsMatch(p.CharacterName))        //全局变量 新角色名不符合要求
                {
                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadCharacterName });  //消息提示新建角色名字不符合要求
                    return;
                }
            }
            else
            {
                if (!Globals.EnCharacterReg.IsMatch(p.CharacterName))
                {
                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadCharacterName });
                    return;
                }
            }

            switch (p.Gender)                      //新角色性别判断
            {
                case MirGender.Male:
                case MirGender.Female:
                    break;
                default:
                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadGender });
                    return;
            }

            if (p.HairType < 0)   //新角色发型判断
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                return;
            }

            if ((p.HairType == 0 && p.HairColour.ToArgb() != 0) || (p.HairType != 0 && p.HairColour.A != 255))      //新角色发型颜色
            {
                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairColour });
                return;
            }


            switch (p.Class)       //职业
            {
                case MirClass.Warrior:  //战士
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11))   //头发类型 性别男女
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.A != 255)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }
                    if (Config.AllowWarrior) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });

                    return;
                case MirClass.Wizard:  //法师
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11))
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.A != 255)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }
                    if (Config.AllowWizard) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });
                    return;
                case MirClass.Taoist:  //道士
                    if (p.HairType > (p.Gender == MirGender.Male ? 10 : 11))
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.A != 255)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }
                    if (Config.AllowTaoist) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });
                    return;
                case MirClass.Assassin:  //刺客

                    if (p.HairType > 5)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadHairType });
                        return;
                    }

                    if (p.ArmourColour.ToArgb() != 0)
                    {
                        con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadArmourColour });
                        return;
                    }

                    if (Config.AllowAssassin) break;

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.ClassDisabled });
                    return;
                default:
                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.BadClass });
                    return;
            }

            int count = 0;  //INT 计数为0

            foreach (CharacterInfo character in con.Account.Characters)   //字符信息 字符输入  账户的角色名
            {
                if (character.Deleted) continue; //如果（角色名 删除）继续

                if (++count < Globals.MaxCharacterCount) continue;  //如果 (++计数 < 全局变量 最大的角色数 ) 继续

                con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.MaxCharacters });   //提示 角色名超过字符数限制
                return;    //返回
            }

            //重复创建角色名字
            for (int i = 0; i < CharacterInfoList.Count; i++)    //定义i=0   如果i<角色名字信息列表的计数  i++
                if (string.Compare(CharacterInfoList[i].CharacterName, p.CharacterName, StringComparison.OrdinalIgnoreCase) == 0)  //如果 字符串比较 角色名字列表里的角色名字 字符串比较 序数对齐情况 等0
                {
                    //if (CharacterInfoList[i].Account == con.Account) continue;  //如果 角色名字列表账号 等  账号 继续

                    con.Enqueue(new S.NewCharacter { Result = NewCharacterResult.AlreadyExists });  //提示角色存在
                    return;  //返回
                }

            //创建角色
            CharacterInfo cInfo = CharacterInfoList.CreateNewObject();  //字符信息=字符信息列表.创建新的角色

            cInfo.CharacterName = p.CharacterName;  //角色名字
            cInfo.Account = con.Account;  //账户
            cInfo.Class = p.Class;    //职业
            cInfo.Gender = p.Gender;  //性别
            cInfo.HairType = p.HairType;  //发型
            cInfo.HairColour = p.HairColour;  //发型颜色
            cInfo.ArmourColour = p.ArmourColour;   //衣服颜色
            cInfo.CreationIP = con.IPAddress;  //IP地址
            cInfo.CreationDate = Now;    //创建日期 现在的时间
            cInfo.PatchGridSize = Globals.PatchGridSize;   //碎片包裹创建角色初始包裹可用大小空间
            cInfo.FixedPointTCount = Config.IntFixedPointCount; //记忆传送功能初始数
            cInfo.RankingNode = Rankings.AddLast(cInfo);  //排行榜节点 = 排行榜列表增加到最后一个

            con.Enqueue(new S.NewCharacter    //等待 新建角色
            {
                Result = NewCharacterResult.Success,    //结果= 创建新的角色成功
                Character = cInfo.ToSelectInfo(),  //角色= 要选择的信息
            });

            Log($"[创建角色] 角色名: {p.CharacterName}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");  //服务端后台打印记录新角色
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void DeleteCharacter(C.DeleteCharacter p, SConnection con)  //删除角色
        {
            if (!Config.AllowDeleteCharacter)
            {
                con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.Disabled });
                return;
            }

            foreach (CharacterInfo character in con.Account.Characters)
            {
                if (character.Index != p.CharacterIndex) continue;

                if (character.Deleted)
                {
                    con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.AlreadyDeleted });
                    return;
                }

                character.Deleted = true;
                con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.Success, DeletedIndex = character.Index });

                Log($"[角色删除] 角色名: {character.CharacterName}, IP地址: {con.IPAddress}, 安全码: {p.CheckSum}");
                return;
            }

            con.Enqueue(new S.DeleteCharacter { Result = DeleteCharacterResult.NotFound });
        }

        public static void RequestStartGame(C.RequestStartGame p, SConnection con)
        {
            if (!Config.AllowStartGame && !con.Account.TempAdmin)
            {
                con.Enqueue(new S.RequestStartGame { Result = StartGameResult.Disabled });
                return;
            }
            foreach (CharacterInfo character in con.Account.Characters)
            {
                if (character.Index != p.CharacterIndex) continue;

                if (character.Deleted)
                {
                    con.Enqueue(new S.RequestStartGame { Result = StartGameResult.Deleted });
                    return;
                }
                if (character.CharacterState == CharacterState.Sell)
                {
                    con.Enqueue(new S.RequestStartGame { Result = StartGameResult.Sell });
                    return;
                }
                TimeSpan duration = Now - character.LastLogin;

                if (duration < Config.RelogDelay)
                {
                    con.Enqueue(new S.RequestStartGame { Result = StartGameResult.Delayed, Duration = Config.RelogDelay - duration });
                    return;
                }

                con.Enqueue(new S.RequestStartGame
                {
                    Result = StartGameResult.Success,
                    CharacterIndex = p.CharacterIndex,
                    ClientControl = ClientControl
                });
                return;
            }

            con.Enqueue(new S.RequestStartGame { Result = StartGameResult.NotFound });
        }
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="p"></param>
        /// <param name="con"></param>
        public static void StartGame(C.StartGame p, SConnection con)
        {
            if (!Config.AllowStartGame && !con.Account.TempAdmin)
            {
                con.Enqueue(new S.StartGame { Result = StartGameResult.Disabled });
                return;
            }

            foreach (CharacterInfo character in con.Account.Characters)
            {
                if (character.Index != p.CharacterIndex) continue;

                if (character.Deleted)
                {
                    con.Enqueue(new S.StartGame { Result = StartGameResult.Deleted });
                    return;
                }

                TimeSpan duration = Now - character.LastLogin;

                if (duration < Config.RelogDelay)
                {
                    con.Enqueue(new S.StartGame { Result = StartGameResult.Delayed, Duration = Config.RelogDelay - duration });
                    return;
                }

                PlayerObject player = new PlayerObject(character, con);
                player.StartGame();

                player.ClientPlatform = p.Platform;
                player.Character.CPUInfo = p.ClientCPUInfo;
                player.Character.MACInfo = p.ClientMACInfo;
                player.Character.HDDInfo = p.ClientHDDInfo;

                con.Enqueue(new S.HuiShengToggle
                {
                    HuiSheng = player.Connection.Account.AllowResurrectionOrder
                });

                con.Enqueue(new S.ReCallToggle
                {
                    Recall = player.Connection.Account.AllowGroupRecall
                });

                con.Enqueue(new S.TradeToggle
                {
                    Trade = player.Connection.Account.AllowTrade
                });

                con.Enqueue(new S.GuildToggle
                {
                    Guild = player.Connection.Account.AllowGuild
                });

                con.Enqueue(new S.FriendToggle
                {
                    Friend = player.Connection.Account.AllowFriend
                });

                return;
            }

            con.Enqueue(new S.StartGame { Result = StartGameResult.NotFound });
        }
        /// <summary>
        /// 拦截锁定的账号
        /// </summary>
        /// <param name="account1"></param>
        /// <param name="account2"></param>
        /// <returns></returns>
        public static bool IsBlocking(AccountInfo account1, AccountInfo account2)
        {
            if (account1 == null || account2 == null || account1 == account2) return false;

            if (account1.TempAdmin || account2.TempAdmin) return false;

            foreach (BlockInfo blockInfo in account1.BlockingList)
                if (blockInfo.BlockedAccount == account2) return true;

            foreach (BlockInfo blockInfo in account2.BlockingList)
                if (blockInfo.BlockedAccount == account1) return true;

            return false;
        }
        /// <summary>
        /// 发送激活邮件
        /// </summary>
        /// <param name="account"></param>
        private static void SendActivationEmail(AccountInfo account)
        {
            account.ActivationKey = Functions.RandomString(Random, 20);
            account.ActivationTime = Now.AddMinutes(5);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,

                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "游戏账号激活",
                        IsBodyHtml = true,

                        Body = $"亲爱的 {account.RealName}, <br><br>" +
                               $"感谢你注册游戏账号, 在你登录游戏之前, 你需要激活你的账户.<br><br>" +
                               $"要完成注册并激活账号, 请访问以下链接:<br>" +
                               $"<a href=\"{Config.WebCommandLink}?Type={ActivationCommand}&{ActivationKey}={account.ActivationKey}\">单击此处激活</a><br><br>" +
                               $"如果上述链接不起作用, 请在下次尝试登录帐户时使用以下激活密钥<br>" +
                               $"激活密钥: {account.ActivationKey}<br><br>" +
                               (account.Referral != null ? $"你的推荐人邮箱: {account.Referral.EMailAddress}<br><br>" : "") +
                               $"如果你不想创建此帐户，并且希望取消注册以删除此帐户，请访问以下链接:<br>" +
                               $"<a href=\"{Config.WebCommandLink}?Type={DeleteCommand}&{DeleteKey}={account.ActivationKey}\">单击此处删除帐户</a><br><br>" +
                               $"希望能在游戏中见到你<br>" +
                               $"<a href=\"{Config.WebSite}\">{Config.ClientName}</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    if (Config.SentryEnabled)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                }
            });
        }
        /// <summary>
        /// 重新发送激活邮件
        /// </summary>
        /// <param name="account"></param>
        private static void ResendActivationEmail(AccountInfo account)
        {
            if (string.IsNullOrEmpty(account.ActivationKey))
                account.ActivationKey = Functions.RandomString(Random, 20);

            account.ActivationTime = Now.AddMinutes(15);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "游戏账号激活",
                        IsBodyHtml = false,

                        Body = $"亲爱的 {account.RealName}\n" +
                               $"\n" +
                               $"感谢你注册游戏账号, 在你登录游戏之前, 你需要激活你的账户.\n" +
                               $"\n" +
                               $"下次尝试登录帐户时, 请使用以下激活密钥\n" +
                               $"激活密钥: {account.ActivationKey}\n\n" +
                               $"希望能在游戏中见到你\n" +
                               $"{Config.ClientName}\n" +
                               $"\n" +
                               $"此电子邮件已被发送, 没有规定的格式防止发送失败",
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    if (Config.SentryEnabled)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                }
            });
        }
        /// <summary>
        /// 发送游戏密码修改邮件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ipAddress"></param>
        private static void SendChangePasswordEmail(AccountInfo account, string ipAddress)
        {
            if (Now < account.PasswordTime)
                return;

            account.PasswordTime = Time.Now.AddMinutes(60);

            EMailsSent++;
            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "游戏密码已更改",
                        IsBodyHtml = true,

                        Body = $"亲爱的 {account.RealName}, <br><br>" +
                               $"这是一封电子邮件, 通知你, 你的游戏密码已更改.<br>" +
                               $"IP地址: {ipAddress}<br><br>" +
                               $"如果你没有进行此更改, 请立即与管理员联系.<br><br>" +
                               $"希望能在游戏中见到你<br>" +
                               $"<a href=\"{Config.WebSite}\">{Config.ClientName}</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    if (Config.SentryEnabled)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                }
            });
        }
        /// <summary>
        /// 发送游戏密码重置申请邮件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ipAddress"></param>
        private static void SendResetPasswordRequestEmail(AccountInfo account, string ipAddress)
        {
            account.ResetKey = Functions.RandomString(Random, 20);
            account.ResetTime = Now.AddMinutes(5);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "游戏密码重置申请",
                        IsBodyHtml = true,

                        Body = $"亲爱的 {account.RealName}, <br><br>" +
                               $"已请求重置密码.<br>" +
                               $"IP地址: {ipAddress}<br><br>" +
                               $"要重置密码, 请单击以下链接:<br>" +
                               $"<a href=\"{Config.WebCommandLink}?Type={ResetCommand}&{ResetKey}={account.ResetKey}\">重置密码</a><br><br>" +
                               $"如果上述链接不起作用, 请使用以下重置密匙重置密码<br>" +
                               $"重置密匙: {account.ResetKey}<br><br>" +
                               $"如果你没有请求此重置, 请忽略此电子邮件, 你的密码不会更改.<br><br>" +
                               $"希望能在游戏中见到你<br>" +
                               $"<a href=\"{Config.WebSite}\">{Config.ClientName}</a>"
                    };
                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    if (Config.SentryEnabled)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                }
            });
        }
        /// <summary>
        /// 发送密码重置邮件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        private static void SendResetPasswordEmail(AccountInfo account, string password)
        {
            account.ResetKey = Functions.RandomString(Random, 20);
            account.ResetTime = Now.AddMinutes(5);
            EMailsSent++;

            Task.Run(() =>
            {
                try
                {
                    SmtpClient client = new SmtpClient(Config.MailServer, Config.MailPort)
                    {
                        EnableSsl = Config.MailUseSSL,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(Config.MailAccount, Config.MailPassword),
                    };

                    MailMessage message = new MailMessage(new MailAddress(Config.MailFrom, Config.MailDisplayName), new MailAddress(account.EMailAddress))
                    {
                        Subject = "游戏密码已重置.",
                        IsBodyHtml = true,

                        Body = $"亲爱的 {account.RealName}, <br><br>" +
                               $"这是一封电子邮件, 通知你, 你的游戏密码已重置.<br>" +
                               $"你的新密码: {password}<br><br>" +
                               $"如果你没有进行此重置，请立即与管理员联系.<br><br>" +
                               $"希望能在游戏中见到你<br>" +
                               $"<a href=\"{Config.WebSite}\">{Config.ClientName}</a>"
                    };

                    client.Send(message);

                    message.Dispose();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                    Log(ex.StackTrace);
                    if (Config.SentryEnabled)
                    {
                        SentrySdk.CaptureException(ex);
                    }

                }
            });
        }

        #region Password Encryption  //密码加密

        /// <summary>
        /// 迭代次数
        /// </summary>
        private const int Iterations = 1354;
        /// <summary>
        /// 盐粒子大小
        /// </summary>
        private const int SaltSize = 16;
        /// <summary>
        /// 哈希值大小
        /// </summary>
        private const int hashSize = 20;
        /// <summary>
        /// 创建加密密匙
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        private static byte[] CreateHash(string password)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);

                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, Iterations))
                {
                    byte[] hash = rfc.GetBytes(hashSize);

                    byte[] totalHash = new byte[SaltSize + hashSize];

                    Buffer.BlockCopy(salt, 0, totalHash, 0, SaltSize);
                    Buffer.BlockCopy(hash, 0, totalHash, SaltSize, hashSize);

                    return totalHash;
                }
            }
        }
        /// <summary>
        /// 密码匹配
        /// </summary>
        /// <param name="password"></param>
        /// <param name="totalHash"></param>
        /// <returns></returns>
        private static bool PasswordMatch(string password, byte[] totalHash)
        {
            byte[] salt = new byte[SaltSize];
            Buffer.BlockCopy(totalHash, 0, salt, 0, SaltSize);

            using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                byte[] hash = rfc.GetBytes(hashSize);

                return Functions.IsMatch(totalHash, hash, SaltSize);
            }
        }

        #endregion

        #region Getters & Setters

        /// <summary>
        /// 按角色获取玩家名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PlayerObject GetPlayerByCharacter(string name)
        {
            return GetCharacter(name)?.Account.Connection?.Player;
        }
        /// <summary>
        /// 通过字符获取连接角色名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static SConnection GetConnectionByCharacter(string name)
        {
            return GetCharacter(name)?.Account.Connection;
        }
        /// <summary>
        /// 获取角色名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CharacterInfo GetCharacter(string name)
        {
            for (int i = 0; i < CharacterInfoList.Count; i++)
                if (string.Compare(CharacterInfoList[i].CharacterName, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return CharacterInfoList[i];

            return null;
        }
        /// <summary>
        /// 获取角色索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static CharacterInfo GetCharacter(int index)
        {
            for (int i = 0; i < CharacterInfoList.Count; i++)
                if (CharacterInfoList[i].Index == index)
                    return CharacterInfoList[i];

            return null;
        }

        /// <summary>
        /// 获取地图信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Map GetMap(MapInfo info)
        {
            return info != null && Maps.ContainsKey(info) ? Maps[info] : null;
        }

        /// <summary>
        /// 获取地图信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Map GetMap(int index)
        {
            return Maps.Values.FirstOrDefault(x => x.Info.Index == index);
        }
        public static Map GetMapByNameAndInstance(string name, int instanceValue = 0)
        {
            if (instanceValue < 0) instanceValue = 0;
            if (instanceValue > 0) instanceValue--;

            var instanceMapList = Maps.Where(t => string.Equals(t.Value.Info.FileName, name, StringComparison.CurrentCultureIgnoreCase)).Select(p => p.Value).ToList();
            return instanceValue < instanceMapList.Count() ? instanceMapList[instanceValue] : null;
        }

        /// <summary>
        /// 获得道具信息名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ItemInfo GetItemInfo(string name)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
                if (string.Compare(ItemInfoList[i].ItemName.Replace(" ", ""), name, StringComparison.OrdinalIgnoreCase) == 0)
                    return ItemInfoList[i];

            return null;
        }
        /// <summary>
        /// 获得怪物信息名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MonsterInfo GetMonsterInfo(string name)
        {
            return MonsterInfoList.Binding.FirstOrDefault
            (monster => string.Compare(monster.MonsterName.Replace(" ", ""), name,
                            StringComparison.OrdinalIgnoreCase) == 0);
        }
        /// <summary>
        /// 获得怪物信息ID
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static MonsterInfo GetMonsterInfo(int index)
        {
            return MonsterInfoList.Binding.FirstOrDefault(x => x.Index == index);
        }
        /// <summary>
        /// 获得NPC信息名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static NPCInfo GetNpcInfo(string name)
        {
            return NPCInfoList.Binding.FirstOrDefault
            (x => string.Compare(x.NPCName.Replace(" ", ""), name,
                            StringComparison.OrdinalIgnoreCase) == 0);
        }
        /// <summary>
        /// 获得NPC信息索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static NPCInfo GetNpcInfo(int index)
        {
            return NPCInfoList.Binding.FirstOrDefault(x => x.Index == index);
        }
        /// <summary>
        /// 获取NPC Object
        /// </summary>
        /// <param name="infoIndex"></param>
        /// <returns></returns>
        public static NPCObject GetNpcObject(int infoIndex)
        {
            NPCInfo targetNPCInfo = GetNpcInfo(infoIndex);
            if (targetNPCInfo?.Region?.Map == null) return null;

            Map NPCMap = GetMap(targetNPCInfo.Region.Map);

            NPCObject ob = NPCMap?.NPCs?.FirstOrDefault(x => x.NPCInfo.Index == infoIndex);

            if (ob != null) { return ob; }

            //没找到 NPC可能在别的地图

            foreach (var npc in Objects)
            {
                if (npc.Race == ObjectType.NPC)
                {
                    if (npc is NPCObject npcobj)
                    {
                        if (npcobj.NPCInfo.Index == infoIndex)
                        {
                            return npcobj;
                        }
                    }
                }
            }

            return null;

        }
        /// <summary>
        /// NPC说话
        /// </summary>
        /// <param name="infoIndex"></param>
        /// <param name="say"></param>
        public static void NPCTalk(int infoIndex, string say)
        {
            var npcObj = GetNpcObject(infoIndex);
            if (npcObj == null)
            {
                Log($"NPC说话出错: 找不到NPC index = {infoIndex}");
                return;
            }

            string name = npcObj.NPCInfo.NPCName;

            string text = $"{name}: {say}";

            foreach (PlayerObject eplayer in npcObj.SeenByPlayers)
            {
                if (!Functions.InRange(npcObj.CurrentLocation, eplayer.CurrentLocation, Config.MaxViewRange)) continue;

                eplayer.Connection.ReceiveChat(text, MessageType.Normal, npcObj.ObjectID);

                foreach (SConnection observer in eplayer.Connection.Observers)
                {
                    observer.ReceiveChat(text, MessageType.Normal, npcObj.ObjectID);
                }
            }
        }
        /// <summary>
        /// 获得怪物信息列表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static MonsterInfo GetMonsterInfo(Dictionary<MonsterInfo, int> list)
        {
            int total = 0;

            foreach (KeyValuePair<MonsterInfo, int> pair in list)
                total += pair.Value;

            int value = Random.Next(total);

            foreach (KeyValuePair<MonsterInfo, int> pair in list)
            {
                value -= pair.Value;

                if (value >= 0) continue;

                return pair.Key;
            }
            return null;
        }
        /// <summary>
        /// 获得地图信息索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static MapInfo GetMapInfo(int index)
        {
            return MapInfoList.Binding.FirstOrDefault(x => x.Index == index);
        }
        /// <summary>
        /// 获得地图信息名字
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MapInfo GetMapInfo(string name)
        {
            return MapInfoList.Binding.FirstOrDefault(x => x.Description == name);
        }
        /// <summary>
        /// 根据门点index获取门点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static MovementInfo GetMovementInfo(int index)
        {
            return MovementInfoList.Binding.FirstOrDefault(x => x.Index == index);
        }

        /// <summary>
        /// 获得城堡行会
        /// </summary>
        /// <param name="castleName"></param>
        /// <returns></returns>
        public static GuildInfo GetGuildFromCastleName(string castleName)
        {
            return SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle != null && x.Castle.Name == castleName);
        }

        /// <summary>
        /// 获得行会老大
        /// </summary>
        /// <param name="guildName"></param>
        /// <returns></returns>
        public static CharacterInfo GetGuildLeader(string guildName)
        {
            GuildInfo guild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.GuildName == guildName);

            GuildMemberInfo member = guild?.Members.FirstOrDefault(x => (x.Permission & GuildPermission.Leader) == GuildPermission.Leader);

            return member?.Account.LastCharacter;
        }

        // 添加一个怪物到监视列表
        public static void AddWatchingMonster(int monInfoIndex)
        {
            var monInfo = MonsterInfoList.Binding.FirstOrDefault(x => x.Index == monInfoIndex);
            if (monInfo == null)
            {
                Log($"添加怪物监视失败, 怪物名{monInfo.MonsterName}");
                return;
            }

            WathchingMonsters[monInfoIndex] = new Dictionary<string, int>();

            //Log($"添加怪物监视成功：怪物名 = {monInfo.MonsterName}");
        }

        // 给某怪物设置初始变量
        public static void SetMonsterDefaultVariable(int monInfoIndex, string key, int value)
        {
            if (WathchingMonsters.TryGetValue(monInfoIndex, out var variables))
            {
                variables.Add(key, value);
            }
            else
            {
                Log($"设置怪物初始变量失败, 请先添加怪物监视");
            }
        }

        // 从监视中移除某怪物
        public static void RemoveWatchingMonster(int monInfoIndex)
        {
            WathchingMonsters.Remove(monInfoIndex);
        }

        // 重置某怪物的个人变量
        public static void ResetMonsterVariables(int monInfoIndex)
        {
            if (WathchingMonsters.TryGetValue(monInfoIndex, out var variables))
            {
                variables.Clear();
            }
        }

        // 清空怪物监视列表
        public static void ClearWatchingMonster()
        {
            foreach (var kvp in WathchingMonsters)
            {
                kvp.Value.Clear();
            }
            WathchingMonsters.Clear();
        }
        public static void ResetVariableForAllPlayers(IronPython.Runtime.List keys, int defaultValue)
        {
            foreach (var character in CharacterInfoList.Binding)
            {
                foreach (int key in keys)
                {
                    var target = character?.Values.FirstOrDefault(x => x.Key == key);
                    if (target != null)
                    {
                        target.Value = defaultValue;
                    }
                }
            }

            for (int i = Players.Count - 1; i >= 0; i--)
            {
                foreach (int key in keys)
                {
                    Players[i]?.SetValues(key, defaultValue);
                }
            }
        }
        // 设置所有玩家的某个个人变量
        public static void ResetVariableForAllPlayers(int key, int defaultValue)
        {
            foreach (var character in CharacterInfoList.Binding)
            {
                var target = character?.Values.FirstOrDefault(x => x.Key == key);
                if (target != null)
                {
                    target.Value = defaultValue;
                }
            }

            for (int i = Players.Count - 1; i >= 0; i--)
            {
                Players[i]?.SetValues(key, defaultValue);
            }
        }

        // 更新NPC的外观
        public static void UpdateNPCLook(int npcInfoIndex, bool permanent = false, string name = null, Color? nameColor = null,
            LibraryFile libraryFile = LibraryFile.None, int bodyShape = 0, Color? overlay = null,
            bool updateIcon = false, QuestIcon icon = QuestIcon.None, PlayerObject player = null)
        {
            var npc = GetNpcObject(npcInfoIndex);

            if (npc == null)
            {
                SEnvir.Log($"更新NPC外观出错：无法获得NPC: {npcInfoIndex}");
                return;
            }

            if (npc.NPCInfo == null)
            {
                SEnvir.Log($"更新NPC外观出错：无法获得NPCInfo: {npcInfoIndex}");
                return;
            }

            var packet = new S.UpdateNPCLook();

            packet.ObjectID = npc.ObjectID;

            if (string.IsNullOrEmpty(name))
            {
                packet.NPCName = npc.NPCInfo.NPCName;
            }
            else
            {
                packet.NPCName = name;
            }

            if (nameColor.HasValue)
            {
                packet.NameColor = (Color)nameColor;
            }
            else
            {
                packet.NameColor = Color.Lime;
            }

            if (libraryFile != LibraryFile.None)
            {
                packet.Library = libraryFile;
            }
            else
            {
                packet.Library = LibraryFile.NPC;
            }

            if (bodyShape != 0)
            {
                packet.ImageIndex = bodyShape;
            }
            else
            {
                packet.ImageIndex = npc.NPCInfo.Image;
            }

            if (overlay.HasValue)
            {
                packet.OverlayColor = (Color)overlay;
            }
            else
            {
                packet.OverlayColor = Color.Empty;
            }

            packet.UpdateNPCIcon = updateIcon;

            if (updateIcon)
            {
                packet.Icon = icon;
            }

            if (player != null)
            {
                packet.CharacterIndex = player.Character.Index;
                player.Enqueue(packet);
            }
            else
            {
                packet.CharacterIndex = 0;
                Broadcast(packet);
            }

            if (permanent)
            {
                // 后上线的玩家也要看到上面的效果
                UpdatedNPCLooks.Add(packet);
            }
        }


        #endregion

        #region 技能限制

        // 地图技能限制
        // 地图名：限制的技能列表
        public static Dictionary<string, HashSet<MagicType>> MapMagicRestrictions = new Dictionary<string, HashSet<MagicType>>();

        public static void AddMagicRestrictionToMap(string mapName, MagicType magic, PlayerObject player = null)
        {
            if (!MapMagicRestrictions.ContainsKey(mapName))
            {
                MapMagicRestrictions.Add(mapName, new HashSet<MagicType>());
            }

            MapMagicRestrictions[mapName].Add(magic);
            if (player == null)
            {
                foreach (var kvp in MapMagicRestrictions)
                {
                    Broadcast(new S.MapMagicRestriction { MapName = kvp.Key, Magics = kvp.Value.ToList() });
                }
            }
            else
            {
                foreach (var kvp in MapMagicRestrictions)
                {
                    player.Connection?.Enqueue(new S.MapMagicRestriction { MapName = kvp.Key, Magics = kvp.Value.ToList() });
                }
            }
        }

        public static void AddMagicRestrictionToMap(string mapName, IronPython.Runtime.List magics, PlayerObject player = null)
        {
            if (!MapMagicRestrictions.ContainsKey(mapName))
            {
                MapMagicRestrictions.Add(mapName, new HashSet<MagicType>());
            }

            foreach (MagicType magic in magics)
            {
                MapMagicRestrictions[mapName].Add(magic);
            }

            if (player == null)
            {
                foreach (var kvp in MapMagicRestrictions)
                {
                    Broadcast(new S.MapMagicRestriction { MapName = kvp.Key, Magics = kvp.Value.ToList() });
                }
            }
            else
            {
                foreach (var kvp in MapMagicRestrictions)
                {
                    player.Connection?.Enqueue(new S.MapMagicRestriction { MapName = kvp.Key, Magics = kvp.Value.ToList() });
                }
            }
        }

        public static void ClearMagicRestrictionOnMap(string mapName, PlayerObject player = null)
        {
            if (MapMagicRestrictions.TryGetValue(mapName, out var magics))
            {
                magics?.Clear();
            }

            if (player == null)
            {
                Broadcast(new S.MapMagicRestriction { ClearAll = true });
            }
            else
            {
                player.Connection?.Enqueue(new S.MapMagicRestriction { ClearAll = true });
            }

        }

        public static void ClearAllMapMagicRestriction()
        {
            MapMagicRestrictions.Clear();
        }

        #endregion

        #region 组队
        /// <summary>
        /// 判断小队中的成员是否在可视范围内
        /// </summary>
        /// <param name="expOwner">小队队长或者击杀怪物的玩家</param>
        /// <returns></returns>
        public static bool GroupMembersInViewRange(PlayerObject expOwner)
        {
            if (expOwner?.GroupMembers == null) return false;
            if (expOwner.CurrentMap?.Info == null) return false;


            int expOwnerMapIndex = expOwner.CurrentMap.Info.Index;
            foreach (var member in expOwner.GroupMembers)
            {
                if (member?.CurrentMap?.Info?.Index != expOwnerMapIndex ||
                    !Functions.InRange(expOwner.CurrentLocation, member.CurrentLocation, Config.MaxViewRange))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 判断小队中，所有职业是否齐全
        /// 即，如果开放刺客，则小队中至少有战法道刺各一个
        /// 如果未开放刺客，则战法道至少各一个
        /// </summary>
        /// <param name="player">任何一个小队成员</param>
        /// <returns></returns>
        public static bool GroupMembersContainAllClasses(PlayerObject player)
        {
            if (player?.GroupMembers == null) return false;

            int warriorNum = 0, wizardNum = 0, taoistNum = 0, assassinNum = 0;
            foreach (var member in player.GroupMembers)
            {
                switch (member.Class)
                {
                    case MirClass.Warrior:
                        warriorNum++;
                        break;
                    case MirClass.Wizard:
                        wizardNum++;
                        break;
                    case MirClass.Taoist:
                        taoistNum++;
                        break;
                    case MirClass.Assassin:
                        assassinNum++;
                        break;
                }
            }

            if (Config.AllowAssassin)
            {
                // 开放刺客
                return (warriorNum * wizardNum * taoistNum * assassinNum) != 0;
            }
            else
            {
                // 未开放刺客 只看战法道
                return (warriorNum * wizardNum * taoistNum) != 0;
            }
        }

        #endregion

        /// <summary>
        /// 错误计数
        /// </summary>
        public static int ErrorCount;
        /// <summary>
        /// 最后一个错误信息
        /// </summary>
        private static string LastError;
        /// <summary>
        /// 保存错误信息
        /// </summary>
        /// <param name="ex"></param>
        public static void SaveError(string ex)
        {
            try
            {
                if (++ErrorCount > 200 || String.Compare(ex, LastError, StringComparison.OrdinalIgnoreCase) == 0) return;

                const string LogPath = @"./Errors\";

                LastError = ex;

                if (!Directory.Exists(LogPath))
                    Directory.CreateDirectory(LogPath);

                File.AppendAllText($"{LogPath}{Now.Year}-{Now.Month}-{Now.Day}.txt", LastError + Environment.NewLine);
            }
            catch
            { }
        }
        /// <summary>
        /// 广播封包
        /// </summary>
        /// <param name="p"></param>
        public static void Broadcast(Packet p)
        {
            foreach (PlayerObject player in Players)
                player.Enqueue(p);
        }
        /// <summary>
        /// 排名获取
        /// </summary>
        /// <param name="p"></param>
        /// <param name="isGM"></param>
        /// <returns></returns>
        public static S.Rankings GetRanks(C.RankRequest p, bool isGM)
        {
            S.Rankings result = new S.Rankings
            {
                OnlineOnly = p.OnlineOnly,
                StartIndex = p.StartIndex,
                Class = p.Class,
                Ranks = new List<RankInfo>(),
                ObserverPacket = false,
            };

            int total = 0;
            int rank = 0;
            foreach (CharacterInfo info in Rankings)
            {
                if (info.Deleted) continue;

                switch (info.Class)
                {
                    case MirClass.Warrior:
                        if ((p.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((p.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((p.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((p.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                rank++;

                if (p.OnlineOnly && info.Player == null) continue;  //如果不在线 或者 角色等空 忽略

                if (info.Account.TempAdmin) continue;  //如果是管理员 忽略

                if (total++ < p.StartIndex || result.Ranks.Count > 20) continue;

                result.Ranks.Add(new RankInfo
                {
                    Rank = rank,
                    Index = info.Index,
                    Class = info.Class,
                    Experience = info.Experience,
                    Level = info.Level + (info.Rebirth * 5000),
                    Rebirth = info.Rebirth,    //排行版增加转生
                    Name = info.CharacterName,
                    Online = info.Player != null,
                    Observable = info.Observable || isGM,
                });
            }

            result.Total = total;

            return result;
        }
        /// <summary>
        /// 获取攻城战统计信息
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static UserConquestStats GetConquestStats(PlayerObject player)
        {
            foreach (ConquestWar war in ConquestWars)
            {
                if (war.Map != player.CurrentMap) continue;

                return war.GetStat(player.Character, player.Connection.Language);
            }

            return null;
        }
        /// <summary>
        /// 创建副本地图
        /// </summary>
        /// <param name="MapIndex"></param>
        /// <returns></returns>
        public static Map CreateMap(int MapIndex)
        {
            Map map = null;  //地图为空
            MapInfo destInfo = MapInfoList.Binding.FirstOrDefault(x => x.Index == MapIndex); //定义 目标地图信息
            MapInfo mapinfo = destInfo.Clone() as MapInfo;   //定义 地图信息等复刻的地图
            mapinfo.fubenIndex++;   //副本ID+++
            map = new Map(mapinfo);
            Maps[mapinfo] = map;
            map.Load();   //地图读取
            map.Setup();  //地图设置
            //并行循环 地图区域列表 绑定
            Parallel.ForEach(MapRegionList.Binding, x =>
            {
                //如果 绑定的地图ID 不等 地图ID 或 绑定的点列表 不等 空 跳出
                if (x.Map.Index != MapIndex || x.PointList != null) return;

                x.CreatePoints(map.Width);
            });
            foreach (RespawnInfo info in RespawnInfoList.Binding)
            {
                if (info.Region != null)
                {
                    if (info.Region.Map.Index == MapIndex)
                    {
                        for (int i = 0; i < info.Count; i++)
                        {
                            MonsterObject mob = MonsterObject.GetMonster(info.Monster);
                            MapRegion region = info.Region;
                            if (region.PointList.Count == 0) continue;

                            for (int j = 0; j < 20; j++)
                                if (mob.Spawn(mapinfo, region.PointList[Random.Next(region.PointList.Count)])) break;

                        }
                    }
                }
                else if (info.MapID > 0)
                {
                    if (info.MapID == MapIndex)
                    {
                        for (int i = 0; i < info.Count; i++)
                        {
                            MonsterObject mob = MonsterObject.GetMonster(info.Monster);

                            for (int j = 0; j < 20; j++)
                                if (mob.Spawn(MapIndex, info.MapX, info.MapY, info.Range)) break;
                        }
                    }
                }
            }

            foreach (NPCInfo info in NPCInfoList.Binding)
            {
                if (info.Region == null) continue;
                if (info.Region.Map.Index == MapIndex)
                {
                    try
                    {
                        NPCObject ob = new NPCObject
                        {
                            NPCInfo = info,

                        };
                        //ob.LoadScript();

                        MapRegion region = info.Region;
                        if (region.PointList.Count == 0) continue;

                        for (int j = 0; j < 20; j++)
                            if (ob.Spawn(mapinfo, region.PointList[Random.Next(region.PointList.Count)])) break;
                    }
                    catch (Exception ex)
                    {
                        Log(ex.ToString());
                        if (Config.SentryEnabled)
                        {
                            SentrySdk.CaptureException(ex);
                        }
                    }
                }

            }
            FubenMaps.Add(map);
            return map;
        }

        /// <summary>
        /// 关闭副本地图
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static void CloseMap(Map map)
        {
            // 如果地图不是副本 那么不能关闭
            if (map == null || !FubenMaps.Contains(map)) return;
            map.ClearAllPlayers();
            map.ClearAllMonsters();
            map.ClearAllNpcs();
            map.ClearAllObjects();
            Maps.Remove(map.Info);
        }
        /// <summary>
        /// 延迟呼叫 毫秒
        /// </summary>
        /// <param name="scriptstr"></param>
        /// <param name="delayMis"></param>
        /// <param name="param"></param>
        public static void DelayCallMS(string scriptstr, int delayMis, object param, PlayerObject player = null)
        {
            ActionScript script = new ActionScript
            {
                ScriptStr = scriptstr,
                StartTime = Now.AddMilliseconds(delayMis),
                Param = param,
                Player = player
            };
            ScriptList.Add(script);
        }
        /// <summary>
        /// 延迟呼叫 秒数
        /// </summary>
        /// <param name="scriptstr"></param>
        /// <param name="delayMis"></param>
        /// <param name="param"></param>
        public static void DelayCall(string scriptstr, int delayMis, object param, PlayerObject player = null)
        {
            ActionScript script = new ActionScript
            {
                ScriptStr = scriptstr,
                StartTime = Now.AddSeconds(delayMis),
                Param = param,
                Player = player
            };
            ScriptList.Add(script);
        }

        /// <summary>
        /// 定时执行一次py
        /// </summary>
        public static void ScheduledCall(string scriptFullName, DateTime startTime, object param, PlayerObject player = null)
        {
            if (startTime < Now) return;

            ActionScript s = ScriptList.FirstOrDefault(x => x.ScriptStr == scriptFullName && x.Player?.ObjectID == player?.ObjectID && param.GetHashCode() == x.Param.GetHashCode());
            if (s != null)
            {
                s.ScriptStr = scriptFullName;
                s.StartTime = startTime;
                s.Param = param;
                s.Player = player;
                return;
            }

            ActionScript script = new ActionScript
            {
                ScriptStr = scriptFullName,
                StartTime = startTime,
                Param = param,
                Player = player
            };
            ScriptList.Add(script);
        }

        /// <summary>
        /// 周期性重复执行py
        /// </summary>
        public static void PeriodicCall(string scriptFullName, DateTime startTime, int intervalInSeconds, object param, PlayerObject player = null)
        {
            if (startTime < Now) return;

            ActionScript s = ScriptList.FirstOrDefault(x => x.ScriptStr == scriptFullName && x.Repeat && x.Player?.ObjectID == player?.ObjectID);
            if (s != null)
            {
                s.ScriptStr = scriptFullName;
                s.StartTime = startTime;
                s.Param = param;
                s.Interval = TimeSpan.FromSeconds(intervalInSeconds);
                s.Player = player;
                return;
            }

            ActionScript script = new ActionScript
            {
                ScriptStr = scriptFullName,
                StartTime = startTime,
                Param = param,
                Repeat = true,
                Interval = TimeSpan.FromSeconds(intervalInSeconds),
                Player = player
            };
            ScriptList.Add(script);
        }

        /// <summary>
        /// 移除定时py
        /// </summary>
        public static void RemoveScript(string scriptFullName, PlayerObject player = null)
        {
            if (player == null)
            {
                ScriptList.RemoveAll(x => x.ScriptStr == scriptFullName);
            }
            else
            {
                ScriptList.RemoveAll(x => x.ScriptStr == scriptFullName && x.Player?.ObjectID == player.ObjectID);
            }
        }

        public static void RunPyScriptFromName(string scriptFullName, object param)
        {
            if (string.IsNullOrEmpty(scriptFullName)) return;

            try
            {
                string[] modules = scriptFullName.Split('.');
                if (modules.Length > 0)
                {
                    int len = 0;
                    dynamic ob;
                    ob = scope.GetVariable(modules[len]);
                    len++;
                    while (len < modules.Length)
                    {
                        ob = scope.Engine.Operations.GetMember(ob, modules[len]);
                        len++;
                    }
                    ob(param);
                }
            }
            catch (SyntaxErrorException e)
            {
                string msg = "DelayCall Syntax error : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));

                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                }
            }
            catch (SystemExitException e)
            {
                string msg = "DelayCall SystemExit : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));

                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                }
            }
            catch (Exception ex)
            {
                string msg = "DelayCall Error loading plugin : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));

                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureMessage(error, SentryLevel.Error);
                }
            }
        }

        /// <summary>
        /// 给某个物体加特效
        /// </summary>
        /// <param name="id"></param>
        /// <param name="effect"></param>
        public static void AddEffect(uint id, Effect effect)
        {
            Broadcast(new S.ObjectEffect { ObjectID = id, Effect = effect });
        }
        /// <summary>
        /// 移除某物体的特效
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveEffects(uint id)
        {
            Broadcast(new S.RemoveEffects { ObjectID = id });
        }
        /// <summary>
        /// 改变天气
        /// </summary>
        /// <param name="mapIndex"></param>
        /// <param name="weather"></param>
        public static void ChangeWeather(int mapIndex, WeatherSetting weather)
        {
            if (!MapInfoList.Binding.Any(x => x.Index == mapIndex)) return;

            WeatherOverrides[mapIndex] = weather;
            Broadcast(new S.ChangeWeather { MapIndex = mapIndex, Weather = weather });
        }

        public static void PYMailSend(AccountInfo Account, string subject, string sender, string message, IronPython.Runtime.List rewards, string recipient = "") //PY信件发送
        {
            // rewards 的格式是[(物品1名称, 数量, 是否绑定)， (物品2名称, 数量, 是否绑定)...]
            // 一封邮件最多7个物品
            if (rewards == null || rewards.Count < 0 || rewards.Count > 7) return;

            MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

            //收件人默认是自己
            AccountInfo account = Account;
            if (!string.IsNullOrEmpty(recipient))
            {
                //收件人不是当前玩家
                account = SEnvir.GetCharacter(recipient)?.Account;
            }

            if (account == null)
                return;

            mail.Account = account;
            mail.Subject = subject;
            mail.Sender = sender;
            mail.Message = message;
            mail.HasItem = rewards.Count > 0;

            foreach (PythonTuple reward in rewards)
            {
                if (reward == null || reward.Count != 3) continue;

                string itemName = (string)reward[0];
                int amount = Convert.ToInt32(reward[1]);
                bool bound = (bool)reward[2];

                UserItemFlags flag = bound ? UserItemFlags.Bound : UserItemFlags.None;
                ItemInfo info = SEnvir.ItemInfoList.Binding.FirstOrDefault(x => x.ItemName == itemName);
                if (info == null) continue;

                TimeSpan duration = TimeSpan.FromSeconds(info.Duration);

                if (duration != TimeSpan.Zero) flag |= UserItemFlags.Expirable;

                ItemCheck check = new ItemCheck(info, amount, flag, duration);
                UserItem item = SEnvir.CreateFreshItem(check);

                //记录物品来源
                SEnvir.RecordTrackingInfo(item, subject, ObjectType.NPC, "脚本系统", sender);

                // 记录
                // 构造日志条目
                CurrencyLogEntry logEntry = new CurrencyLogEntry()
                {
                    LogLevel = LogLevel.Info,
                    Component = "脚本邮件",
                    Time = SEnvir.Now,
                    Currency = CurrencyType.None,
                    Action = CurrencyAction.Undefined,
                    Source = CurrencySource.OtherAdd,
                    Amount = mail.Items.Count,
                    ExtraInfo = $"{sender}邮件发送给{recipient}: {item.Info.ItemName}"
                };
                // 存入日志
                SEnvir.LogToViewAndCSV(logEntry);

                item.Slot = mail.Items.Count;
                item.Mail = mail;
            }

            if (account.Connection?.Player != null)
            {
                account.Connection.Enqueue(new S.MailNew
                {
                    Mail = mail.ToClientInfo(),
                    ObserverPacket = false,
                });
            }
        }

        public static void GoldMarketProcess()
        {
            var list = SEnvir.GoldMarketInfoList.Where(x => x.Date.AddDays(2) < SEnvir.Now && x.TradeState != StockOrderType.Completed && x.TradeState != StockOrderType.Cannel).ToList();
            foreach (var goldMarketInfo in list)
            {
                goldMarketInfo.TradeState = StockOrderType.Cannel;
                if (goldMarketInfo.TradeType == TradeType.Buy)
                {
                    //给金币给买家
                    var mail = SEnvir.MailInfoList.CreateNewObject();
                    mail.Account = goldMarketInfo.Character.Account;

                    //long tax = (long)(cost * Config.NewAuctionTax);//税率

                    mail.Subject = "金币交易时间到期失效";
                    mail.Sender = "金币交易行";

                    var gold = SEnvir.CreateFreshItem(SEnvir.GameGoldInfo);
                    gold.Count = (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * goldMarketInfo.GameGoldPrice;
                    gold.Mail = mail;
                    gold.Slot = 0;
                    mail.Message = $"购买金币订单到期已经退回，获得赞助币" + "\n\n" +
                                           string.Format("挂单数量" + "{0}万\n", goldMarketInfo.GoldCount) +
                                           string.Format("成交数量" + ": {0} 万\n", goldMarketInfo.CompletedCount) +
                                           string.Format("返还" + ": {0:###0.00} 赞助币", Convert.ToDecimal(Convert.ToInt32(goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * goldMarketInfo.GameGoldPrice / 100)
                                           );
                    mail.HasItem = true;
                    if (goldMarketInfo.Character.Account.Connection?.Player != null)
                        goldMarketInfo.Character.Account.Connection.Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });
                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "金币交易行系统",
                        Time = SEnvir.Now,
                        Character = mail.Account.LastCharacter,
                        Currency = CurrencyType.GameGold,
                        Action = CurrencyAction.Add,
                        Source = CurrencySource.ItemAdd,
                        Amount = gold.Count,
                        ExtraInfo = $"金币交易行自动取消订单退回交易未成交赞助币邮件"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
                else if (goldMarketInfo.TradeType == TradeType.Sell)
                {
                    //给金币给买家
                    var mail = SEnvir.MailInfoList.CreateNewObject();
                    mail.Account = goldMarketInfo.Character.Account;

                    //long tax = (long)(cost * Config.NewAuctionTax);//税率

                    mail.Subject = "金币交易时间到期失效";
                    mail.Sender = "金币交易行";

                    var gold = SEnvir.CreateFreshItem(SEnvir.GoldInfo);
                    gold.Count = (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000;
                    gold.Mail = mail;
                    gold.Slot = 0;
                    mail.Message = $"销售金币订单到期已经退回，获得金币" + "\n\n" +
                                           string.Format("挂单数量" + "{0}万\n", goldMarketInfo.GoldCount) +
                                           string.Format("成交数量" + ": {0} 万\n", goldMarketInfo.CompletedCount) +
                                           string.Format("返还" + ": {0:###0.00} 金币", Convert.ToDecimal(Convert.ToInt32(goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000)
                                           );
                    mail.HasItem = true;
                    if (goldMarketInfo.Character.Account.Connection?.Player != null)
                        goldMarketInfo.Character.Account.Connection.Enqueue(new S.MailNew
                        {
                            Mail = mail.ToClientInfo(),
                            ObserverPacket = false,
                        });
                    // 记录
                    // 构造日志条目
                    CurrencyLogEntry logEntry = new CurrencyLogEntry()
                    {
                        LogLevel = LogLevel.Info,
                        Component = "金币交易行系统",
                        Time = SEnvir.Now,
                        Character = mail.Account.LastCharacter,
                        Currency = CurrencyType.Gold,
                        Action = CurrencyAction.Add,
                        Source = CurrencySource.ItemAdd,
                        Amount = (goldMarketInfo.GoldCount - goldMarketInfo.CompletedCount) * 10000,
                        ExtraInfo = $"金币交易行自动取消订单退回交易未成交金币邮件"
                    };
                    // 存入日志
                    SEnvir.LogToViewAndCSV(logEntry);
                }
            }
        }
    }
    /// <summary>
    /// WEB命令
    /// </summary>
    public class WebCommand
    {
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType Command { get; set; }
        /// <summary>
        /// 账号信息
        /// </summary>
        public AccountInfo Account { get; set; }
        /// <summary>
        /// WEB命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="account"></param>
        public WebCommand(CommandType command, AccountInfo account)
        {
            Command = command;
            Account = account;
        }
    }
    /// <summary>
    /// 命令类型
    /// </summary>
    public enum CommandType
    {
        /// <summary>
        /// 空
        /// </summary>
        None,
        /// <summary>
        /// 激活
        /// </summary>
        Activation,
        /// <summary>
        /// 密码重置
        /// </summary>
        PasswordReset,
        /// <summary>
        /// 删除账户
        /// </summary>
        AccountDelete
    }

    /// <summary>
    /// IPN信息
    /// </summary>
    public sealed class IPNMessage
    {
        public string Message { get; set; }
        public bool Verified { get; set; }
        public string FileName { get; set; }
        public bool Duplicate { get; set; }
    }
    /// <summary>
    /// 语言脚本
    /// </summary>
    public class ActionScript
    {
        /// <summary>
        /// 脚本链接
        /// </summary>
        public string ScriptStr { get; set; }
        /// <summary>
        /// 启动时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public object Param { get; set; }

        /// <summary>
        /// 重复
        /// </summary>
        public bool Repeat { get; set; } = false;
        /// <summary>
        /// 间隔时间
        /// </summary>
        public TimeSpan Interval { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// 绑定的玩家
        /// </summary>
        public PlayerObject Player { get; set; } = null;
    }


    //boss信息 用于记录boss击杀状况
    public class BossTracker
    {
        public MonsterInfo BossInfo { get; set; }
        public string BossName => BossInfo?.MonsterName;
        public CharacterInfo LastKiller { get; set; }
        public string LastKillerName => LastKiller?.CharacterName;
        public DateTime LastKillTime { get; set; }

        public List<SpawnInfo> SpawnInfo => SEnvir.Spawns?.Where(x => x.Info.Monster.Index == this.BossInfo.Index).ToList();
    }
}
