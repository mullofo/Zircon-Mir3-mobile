using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Sentry;
using Server.DBModels;
using Server.Models;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using C = Library.Network.ClientPackets;
using G = Library.Network.GeneralPackets;
using S = Library.Network.ServerPackets;

namespace Server.Envir
{
    /// <summary>
    /// 基础链接
    /// </summary>
    public sealed class SConnection : BaseConnection
    {
        private static int SessionCount;

        protected override TimeSpan TimeOutDelay => Config.TimeOut;

        private DateTime PingTime;
        private bool PingSent;
        public int Ping { get; private set; }
        /// <summary>
        /// 游戏状态
        /// </summary>
        public GameStage Stage { get; set; }
        public AccountInfo Account { get; set; }
        public PlayerObject Player { get; set; }
        public string IPAddress { get; }
        public int SessionID { get; }
        /// <summary>
        /// 观察者连接
        /// </summary>
        public SConnection Observed;
        /// <summary>
        /// 观察者列表
        /// </summary>
        public List<SConnection> Observers = new List<SConnection>();

        public List<AuctionInfo> MPSearchResults = new List<AuctionInfo>();
        public List<AuctionInfo> MySearchResults = new List<AuctionInfo>();
        public HashSet<AuctionInfo> VisibleResults = new HashSet<AuctionInfo>();

        public List<NewAutionInfo> AutionSearchResults = new List<NewAutionInfo>();
        public HashSet<NewAutionInfo> AutionVisibleResults = new HashSet<NewAutionInfo>();

        public Language Language;
        private bool FirstPacket;

        public SConnection(TcpClient client) : base(client)  //连接客户端
        {
            IPAddress = client.Client.RemoteEndPoint.ToString().Split(':')[0];
            SessionID = ++SessionCount;

            Language = Language.SimplifiedChinese; //Todo Language Selections

            OnException += (o, e) =>
            {
                SEnvir.Log(string.Format("崩溃: 账号: {0}, 角色: {1}.", Account?.EMailAddress, Player?.Name));
                SEnvir.Log(e.ToString());
                SEnvir.Log(e.StackTrace.ToString());

                File.AppendAllText(@"./Errors.txt", e.StackTrace + Environment.NewLine);
                if (Config.SentryEnabled)
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.SetTag("Server", Config.Server1Name);
                        scope.SetTag("Error IP", IPAddress);
                        scope.SetTag("DB Password", Config.DBPassword);
                    });
                    SentrySdk.CaptureException(e);
                    SentrySdk.CaptureMessage($"角色 {Player?.Name} 引起崩溃", SentryLevel.Error);
                }
            };
            Output += (o, e) =>
            {
                SEnvir.Log(e.ToString());
                if (Config.SentryEnabled)
                {
                    SentrySdk.ConfigureScope(scope =>
                    {
                        scope.SetTag("Server", Config.Server1Name);
                        scope.SetTag("Error IP", IPAddress);
                        scope.SetTag("DB Password", Config.DBPassword);
                    });
                    SentrySdk.CaptureMessage(e, SentryLevel.Error);
                }
            };

            //凡是尝试建立连接的地址都只记录到文件，方便排查
            SEnvir.Log(string.Format("[连接] IP地址:{0} 尝试连接！！！", IPAddress), onlyFile: true);

            UpdateTimeOut();
            BeginReceive();

            //向建立连接的客户端发一个Connected空包，告诉客户端连接就绪
            Enqueue(new G.Connected());
            FirstPacket = true;
        }

        public override void ReceivePacket()   //处理封包
        {
            int length = Packet.CheckPacketLength(_rawData);
            if (length == -1) return;

            //如果第一个封包不是Version就断开连接
            if (FirstPacket)
            {
                FirstPacket = false;
                int id = ((_rawData[5] ^ Packet.xorma) << 8) | (_rawData[4] ^ Packet.xorma);
                if (id < 0 || id >= Packet.Packets.Count || (Packet.Packets[id].Name != "Version" && Packet.Packets[id].Name != "Disconnect"))
                {
                    SEnvir.Log(string.Format("客户端初始封包错误：{0}", Packet.Packets[id].Name));
                    SendDisconnect(new G.Disconnect { Reason = DisconnectReason.WrongVersion });
                    Disconnect();
                    return;
                }
            }
            base.ReceivePacket();
        }

        public override void Disconnect()  //断开连接
        {
            if (!Connected) return;

            base.Disconnect();

            CleanUp();

            if (!SEnvir.Connections.Contains(this))
                throw new InvalidOperationException("在列表中找不到连接");

            SEnvir.Connections.Remove(this);

            SEnvir.IPCount[IPAddress]--;
            SEnvir.DBytesSent += TotalBytesSent;
            SEnvir.DBytesReceived += TotalBytesReceived;
        }

        public override void SendDisconnect(Packet p)  //发送断开连接
        {
            base.SendDisconnect(p);

            CleanUp();
        }
        public override void TryDisconnect()  //尝试断开连接
        {
            if (Stage == GameStage.Game)
            {
                if (SEnvir.Now >= Player.CombatTime.AddSeconds(10))
                {
                    Disconnect();
                    return;
                }

                if (!Disconnecting)
                {
                    Disconnecting = true;
                    TimeOutTime = Time.Now.AddSeconds(10);
                }

                if (SEnvir.Now <= TimeOutTime) return;
            }

            Disconnect();
        }
        public override void TrySendDisconnect(Packet p)  //尝试发送断开连接
        {
            if (Stage == GameStage.Game)
            {
                if (SEnvir.Now >= Player.CombatTime.AddSeconds(10))
                {
                    Disconnect();
                    return;
                }

                if (!Disconnecting)
                {
                    base.SendDisconnect(p);

                    TimeOutTime = Time.Now.AddSeconds(10);
                }

                if (SEnvir.Now <= TimeOutTime) return;

            }

            SendDisconnect(p);
        }

        public void EndObservation()  //结束观察者
        {
            Observed.Observers.Remove(this);
            Observed = null;

            if (Account != null)
            {
                Stage = GameStage.Select;
                Enqueue(new S.GameLogout { Characters = Account.GetSelectInfo() });
            }
            else
            {
                Stage = GameStage.Login;
                Enqueue(new S.SelectLogout());
            }
        }
        public void CleanUp()  //清理
        {
            Stage = GameStage.Disconnected;

            if (Account != null && Account.Connection == this)
            {
                Account.TempAdmin = false;
                Account.Connection = null;
            }

            Account = null;
            Player?.StopGame();
            Player = null;

            Observed?.Observers.Remove(this);
            Observed = null;

            //ItemList.Clear();
            //MagicList.Clear();
        }
        public override void Process()  //过程
        {
            try
            {
                if (SEnvir.Now >= PingTime && !PingSent && Stage != GameStage.None)
                {
                    PingTime = SEnvir.Now;
                    PingSent = true;
                    Enqueue(new G.Ping { ObserverPacket = false });
                }

                if (ReceiveList != null && ReceiveList.Count > Config.MaxPacket)
                {
                    TryDisconnect();
                    SEnvir.IPBlocks[IPAddress] = SEnvir.Now.Add(Config.PacketBanTime);

                    for (int i = SEnvir.Connections.Count - 1; i >= 0; i--)
                        if (SEnvir.Connections[i]?.IPAddress == IPAddress)
                            SEnvir.Connections[i]?.TryDisconnect();

                    SEnvir.Log($"{IPAddress} 断开连接,大量数据包");
                    return;
                }
            }
            catch (Exception ex)
            {
                if (Config.SentryEnabled)
                {
                    SentrySdk.CaptureException(ex);
                }
            }


            base.Process();
        }

        public override void Enqueue(Packet p)  //队列(数据包)
        {
            base.Enqueue(p);

            if (p == null || !p.ObserverPacket) return;

            foreach (SConnection observer in Observers)
                observer.Enqueue(p);
        }

        public void ReceiveChat(string text, int type, uint objectID = 0, int npcFace = 0)  //接收聊天
        {
            ReceiveChat(text, (MessageType)type, objectID, npcFace);
        }
        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="text">信息文本内容</param>
        /// <param name="type">信息类型</param>
        /// <param name="objectID">对象ID</param>
        public void ReceiveChat(string text, MessageType type, uint objectID = 0, int npcFace = 0)
        {
            switch (Stage)
            {
                case GameStage.Game:
                case GameStage.Observer:
                    var chat = new S.Chat
                    {
                        Text = text,
                        Type = type,
                        ObjectID = objectID,
                        NpcFace = npcFace,
                        ObserverPacket = false,
                    };
                    Enqueue(chat);
                    break;
                default:
                    return;
            }
        }

        public void Process(C.SelectLanguage p)  //语言切换
        {
            Language = p.Language;

        }
        public void Process(G.Disconnect p)  //断开
        {
            Disconnecting = true;
        }
        //public void Process(G.Connected p)   //有关联的
        //{
        //    if (Config.CheckVersion)
        //    {
        //        Enqueue(new G.CheckVersion());
        //        return;
        //    }

        //    Stage = GameStage.Login;
        //    Enqueue(new G.GoodVersion());
        //}
        public void Process(G.Version p)   //版本号
        {
            if (Stage != GameStage.None) return;

            if (p.Platform == Platform.Mobile)
            {
                if (Config.CheckPhoneVersion)
                {
                    if (!Functions.IsMatch(System.Text.Encoding.Default.GetBytes(Config.PhoneVersionNumber), p.ClientHash))
                    {
                        SendDisconnect(new G.Disconnect { Reason = DisconnectReason.WrongVersion });
                        return;
                    }

                    if (!Functions.IsMatch(Config.ClientSystemDBHash, p.ClientSystemDBHash))
                    {
                        SendDisconnect(new G.Disconnect { Reason = DisconnectReason.WrongClientSystemDBVersion });
                        return;
                    }

                }
            }
            else
            {
                if (Config.CheckVersion)
                {
                    if (!Functions.IsMatch(Config.ClientHash, p.ClientHash) && !Functions.IsMatch(Config.ClientHash1, p.ClientHash))
                    {
                        SendDisconnect(new G.Disconnect { Reason = DisconnectReason.WrongVersion });
                        return;
                    }

                    if (!Functions.IsMatch(Config.ClientSystemDBHash, p.ClientSystemDBHash))
                    {
                        SendDisconnect(new G.Disconnect { Reason = DisconnectReason.WrongClientSystemDBVersion });
                        return;
                    }

                    if (SEnvir.CheckCPUInBanList(p.ClientCPUInfo) || SEnvir.CheckHDDInBanList(p.ClientHDDInfo) || SEnvir.CheckMACInBanList(p.ClientMACInfo))
                    {
                        SendDisconnect(new G.Disconnect { Reason = DisconnectReason.Banned });
                        return;
                    }
                }
            }

            Stage = GameStage.Login;

            // 客户端连接成功 发送数据库密钥和区服信息
            Enqueue(new G.GoodVersion
            {
                DBEncrypted = Config.EnableDBEncryption,
                DBPassword = Config.DBPassword,
                Server1Name = Config.ClientName,
                PlayCount = SEnvir.Connections.Count,
            });

            //与客户端验证了连接后打印连接日志
            SEnvir.Log(string.Format("[连接] IP地址:{0}", IPAddress));
        }
        public void Process(G.Ping p)   //PING值
        {
            if (Stage == GameStage.None) return;

            int ping = (int)(SEnvir.Now - PingTime).TotalMilliseconds / 2;
            PingSent = false;
            PingTime = SEnvir.Now + Config.PingDelay;

            Ping = ping;
            Enqueue(new G.PingResponse { Ping = Ping, ObserverPacket = false });
        }

        public void Process(C.NewAccount p)   //新建账号
        {
            if (Stage != GameStage.Login) return;

            SEnvir.NewAccount(p, this);
        }
        public void Process(C.ChangePassword p)   //修改密码
        {
            if (Stage != GameStage.Login) return;

            SEnvir.ChangePassword(p, this);
        }
        public void Process(C.RequestPasswordReset p)   //请求重置密码
        {
            if (Stage != GameStage.Login) return;

            SEnvir.RequestPasswordReset(p, this);
        }
        public void Process(C.ResetPassword p)   //重置密码
        {
            if (Stage != GameStage.Login) return;

            SEnvir.ResetPassword(p, this);
        }
        public void Process(C.Activation p)   //激活
        {
            if (Stage != GameStage.Login) return;

            SEnvir.Activation(p, this);
        }
        public void Process(C.RequestActivationKey p)   //请求激活KEY
        {
            if (Stage != GameStage.Login) return;

            SEnvir.RequestActivationKey(p, this);
        }
        public void Process(C.Login p)   //登录
        {
            if (Stage != GameStage.Login) return;

            SEnvir.Login(p, this);
        }
        public void Process(C.Logout p)   //退出游戏
        {

            switch (Stage)
            {
                case GameStage.Select:   //退到选择界面
                    Stage = GameStage.Login;
                    Account.Connection = null;
                    Account = null;

                    Enqueue(new S.SelectLogout());
                    break;
                case GameStage.Game:    //退出时 是否在游戏判断

                    //if (SEnvir.Now < Player.CombatTime.AddSeconds(10)) return;

                    Player.StopGame();

                    Stage = GameStage.Select;

                    Enqueue(new S.GameLogout { Characters = Account.GetSelectInfo() });
                    break;
                case GameStage.Observer:   //观察者退出
                    EndObservation();
                    break;
            };
        }

        public void Process(C.NewCharacter p)   //新建角色
        {
            if (Stage != GameStage.Select) return;

            SEnvir.NewCharacter(p, this);
        }
        public void Process(C.DeleteCharacter p)  //删除角色
        {
            if (Stage != GameStage.Select) return;

            SEnvir.DeleteCharacter(p, this);
        }
        public void Process(C.RequestStartGame p)   //开始游戏
        {
            if (Stage != GameStage.Select) return;


            SEnvir.RequestStartGame(p, this);
        }

        public void Process(C.StartGame p)   //开始游戏
        {
            if (Stage != GameStage.Select) return;


            SEnvir.StartGame(p, this);
        }
        public void Process(C.TownRevive p)   //城镇复活
        {
            if (Stage != GameStage.Game) return;

            Player.TownRevive();
        }
        public void Process(C.Turn p)  //转身
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Turn(p.Direction);
        }
        public void Process(C.Harvest p)  //割肉
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Harvest(p.Direction);
        }
        public void Process(C.Move p)  //移动
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            /*  if (p.Distance > 1 && (Player.BagWeight > Player.Stats[Stat.BagWeight] || Player.WearWeight > Player.Stats[Stat.WearWeight]))
              {
                  Enqueue(new S.UserLocation { Direction = Player.Direction, Location = Player.CurrentLocation });
                  return;
              }*/

            Player.Move(p.Direction, p.Distance);
        }
        public void Process(C.Mount p)   //登上
        {
            if (Stage != GameStage.Game) return;

            Player.Mount();
        }
        public void Process(C.Attack p)   //攻击
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Attack(p.Direction, p.AttackMagic);
        }
        public void Process(C.Magic p)   //魔法技能
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Magic(p);
        }
        public void Process(C.MagicToggle p)   //魔法切换
        {
            if (Stage != GameStage.Game) return;

            Player.MagicToggle(p);
        }
        public void Process(C.Mining p)   //挖矿
        {
            if (Stage != GameStage.Game) return;

            if (p.Direction < MirDirection.Up || p.Direction > MirDirection.UpLeft) return;

            Player.Mining(p.Direction);
        }

        public void Process(C.StorageItemRefresh p) //刷新仓库
        {
            if (Stage != GameStage.Game) return;

            Player.StorageItemRefresh(p);
        }

        public void Process(C.CompanionGridRefresh p)
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionGridItemRefresh(p);
        }

        public void Process(C.InventoryRefresh p) //排序包裹物品
        {
            if (Stage != GameStage.Game) return;

            Player.InventoryRefresh(p);
        }

        public void Process(C.ItemMove p)   //道具移动
        {
            if (Stage != GameStage.Game) return;

            Player.ItemMove(p);
        }

        public void Process(C.ItemDrop p)   //道具爆出
        {
            if (Stage != GameStage.Game) return;

            Player.ItemDrop(p);
        }

        public void Process(C.ItemDelete p)   //道具删除
        {
            if (Stage != GameStage.Game) return;

            Player.ItemDelete(p);
        }

        public void Process(C.PickUp p)  //捡取道具
        {
            if (Stage != GameStage.Game) return;

            Player.PickUp(p.Xpos, p.Ypos, p.ItemIdx);
        }
        public void Process(C.GoldDrop p)   //金币爆出
        {
            if (Stage != GameStage.Game) return;

            Player.GoldDrop(p);
        }
        public void Process(C.ItemUse p)  //道具用途
        {
            if (Stage != GameStage.Game) return;

            Player.ItemUse(p.Link);
        }

        public void Process(C.BeltLinkChanged p)  //药水快捷栏变化
        {
            if (Stage != GameStage.Game) return;

            Player.BeltLinkChanged(p);
        }
        public void Process(C.AutoPotionLinkChanged p)   //自动喝药栏变化
        {
            if (Stage != GameStage.Game) return;

            Player.AutoPotionLinkChanged(p);
        }
        public void Process(C.ComSortingConfChanged p)
        {
            if (Stage != GameStage.Game) return;
            Player.ComSortingConfChanged(p);
        }
        public void Process(C.ComSortingConf1Changed p)
        {
            if (Stage != GameStage.Game) return;
            Player.ComSortingConf1Changed(p);
        }
        public void Process(C.AutoFightConfChanged p)  //自动挂机变化
        {
            if (Stage != GameStage.Game) return;

            Player.AutoFightConfChanged(p);
        }
        public void Process(C.Chat p)  //聊天
        {
            if (p.Text.Length > Globals.MaxChatLength) return;

            if (Stage == GameStage.Game)
                Player.Chat(p.Text, p.Links);

            if (Stage == GameStage.Observer)
                Observed.Player.ObserverChat(this, p.Text);
        }
        public void Process(C.NPCCall p)  //NPC对话
        {
            p.isDKey = false;
            if (Stage != GameStage.Game) return;
            if (Config.UseTxtScript)
            {
                Player.NPCCall(p.ObjectID, p.Key);
            }
            else
            {
                Player.NPCCall(p.ObjectID, p.isDKey);
            }
        }

        public void Process(C.DKey p)     //D菜单
        {
            if (Stage != GameStage.Game) return;
            // TODO 加封包参数？
            //Player.NPCCall(184, true);
            Player.DKeyCall();
        }

        public void Process(C.NPCButton p)   //NPC按钮
        {
            if (Stage != GameStage.Game) return;

            Player.NPCButton(p.ButtonID, p.links, p.UserInput);
        }
        public void Process(C.NPCBuy p)   //NPC买
        {
            if (Stage != GameStage.Game) return;

            Player.NPCBuy(p);
        }

        public void Process(C.NPCBuyBack p)   //NPC回购
        {
            if (Stage != GameStage.Game) return;
            Player.NPCBuyBack(p);
        }

        public void Process(C.NPCBuyBackSeach p)   //NPC回购
        {
            if (Stage != GameStage.Game) return;
            Player.NPCBuyBackSeach(p);

        }

        public void Process(C.NPCSell p)  //NPC卖
        {
            if (Stage != GameStage.Game) return;

            Player.NPCSell(p.Links);
        }
        public void Process(C.NPCRootSell p)  //NPC一键出售
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRootSell(p.Links);
        }
        public void Process(C.NPCRepair p)  //NPC修理
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRepair(p);
        }

        public void Process(C.NPCSpecialRepair p)  //NPC特修
        {
            if (Stage != GameStage.Game) return;

            Player.NPCSpecialRepair(p);
        }

        public void Process(C.NPCBookRefine p)   //技能书合成
        {
            if (Stage != GameStage.Game) return;

            Player.NPCBookRefine(p);
        }

        public void Process(C.NPCRefinementStone p)  //NPC精炼石
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRefinementStone(p);
        }
        public void Process(C.NPCRefine p)   //NPC精炼
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRefine(p);
        }
        public void Process(C.NPCRefineRetrieve p)  //NPC精炼索引
        {
            if (Stage != GameStage.Game) return;

            Player.NPCRefineRetrieve(p.Index);
        }
        public void Process(C.NPCMasterRefine p)  //NPC大师精炼
        {
            if (Stage != GameStage.Game) return;

            Player.NPCMasterRefine(p);
        }
        public void Process(C.NPCMasterRefineEvaluate p)  //NPC大师精炼评估
        {
            if (Stage != GameStage.Game) return;

            Player.NPCMasterRefineEvaluate(p);
        }
        public void Process(C.NPCClose p)   //NPC关闭
        {
            if (Stage != GameStage.Game) return;

            Player.NPC = null;
            Player.NPCPage = null;

            foreach (SConnection con in Observers)
            {
                con.Enqueue(new S.NPCClose());
            }
        }
        public void Process(C.NPCFragment p)  //NPC分解碎片
        {
            if (Stage != GameStage.Game) return;

            Player.NPCFragment(p.Links);
        }
        public void Process(C.NPCAccessoryLevelUp p)  //NPC配件等级升级
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryLevelUp(p);
        }
        public void Process(C.NPCAccessoryUpgrade p)  //NPC配件升级
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryUpgrade(p);
        }

        public void Process(C.NPCEnchantmentSynthesis p)  //NPC附魔石合成
        {
            if (Stage != GameStage.Game) return;

            Player.NPCEnchantmentSynthesis(p);
        }

        public void Process(C.MagicKey p)   //魔法技能对应的键值列
        {
            if (Stage != GameStage.Game) return;

            foreach (KeyValuePair<MagicType, UserMagic> pair in Player.Magics)
            {
                if (pair.Value.Set1Key == p.Set1Key)
                    pair.Value.Set1Key = SpellKey.None;

                if (pair.Value.Set2Key == p.Set2Key)
                    pair.Value.Set2Key = SpellKey.None;

                if (pair.Value.Set3Key == p.Set3Key)
                    pair.Value.Set3Key = SpellKey.None;

                if (pair.Value.Set4Key == p.Set4Key)
                    pair.Value.Set4Key = SpellKey.None;
            }

            UserMagic magic;

            if (!Player.Magics.TryGetValue(p.Magic, out magic)) return;

            magic.Set1Key = p.Set1Key;
            magic.Set2Key = p.Set2Key;
            magic.Set3Key = p.Set3Key;
            magic.Set4Key = p.Set4Key;
        }

        public void Process(C.GroupSwitch p)  //组队开关
        {
            if (Stage != GameStage.Game) return;

            Player.GroupSwitch(p.Allow);
        }
        public void Process(C.GroupInvite p)  //组队邀请
        {
            if (Stage != GameStage.Game) return;

            Player.GroupInvite(p.Name);
        }
        public void Process(C.GroupRemove p)  //组队离开
        {
            if (Stage != GameStage.Game) return;

            Player.GroupRemove(p.Name);
        }
        public void Process(C.GroupResponse p)  //组队响应
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.GroupJoin();

            Player.GroupInvitation = null;
        }

        public void Process(C.Inspect p)  //检查
        {
            if (Stage == GameStage.Game)
                Player.Inspect(p.Index, this);

            if (Stage == GameStage.Observer)
                Observed.Player.Inspect(p.Index, this);
        }
        public void Process(C.RankRequest p)  //排行要求
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer && Stage != GameStage.Login) return;

            Enqueue(SEnvir.GetRanks(p, Account != null && (Account.TempAdmin || Account.Observer)));
        }

        public void Process(C.ObserverRequest p)  //观察者请求
        {
            if (!Config.AllowObservation && (Account == null || (!Account.TempAdmin && !Account.Observer))) return;  //不是观察者  不是管理员

            PlayerObject player = SEnvir.GetPlayerByCharacter(p.Name);  //获取玩家名字

            if (player == null || player == Player) return;   //玩家为空

            if (!player.Character.Observable && (Account == null || (!Account.TempAdmin && !Account.Observer))) return;  //观察者开关未开启  如果是管理员无视开关是否关闭

            if (Stage == GameStage.Game)
                Player.StopGame();

            if (Stage == GameStage.Observer)
            {
                Observed.Observers.Remove(this);
                Observed = null;
            }

            player.SetUpObserver(this);
        }
        public void Process(C.ObservableSwitch p)  //观察者开关
        {
            if (Stage != GameStage.Game) return;

            Player.ObservableSwitch(p.Allow);
        }

        // 发起添加好友请求
        public void Process(C.FriendSwitch p)
        {
            if (Stage != GameStage.Game) return;

            Player.FriendSwitch(p.Allow);
        }

        public void Process(C.FriendRequest p)
        {
            if (Stage != GameStage.Game) return;

            Player.FriendInvite(p.Name);

        }

        // 服务端添加好友成功交互接口
        public void Process(C.FriendResponse p)
        {

            if (Stage != GameStage.Game) return;

            if (p.Accept)
            {

                PlayerObject inviter = SEnvir.GetPlayerByCharacter(p.Name);
                // 生成时间戳
                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string LinkID = p.Name + Player.Name + Convert.ToInt64(ts.TotalSeconds).ToString();

                // 接受对方添加好友
                Player.FriendAdd(p.Name, LinkID);
                Player.FriendInvitation = null;

                // 发起者也添加好友
                inviter.FriendAdd(Player.Name, LinkID);
                inviter.FriendInvitation = null;
            }
            else
            {
                PlayerObject inviter = SEnvir.GetPlayerByCharacter(p.Name);
                inviter.Connection.ReceiveChat($"{Player.Name} 拒绝添加好友", MessageType.System);
                inviter.FriendInvitation = null;
                Player.FriendInvitation = null;
            }
        }

        public void Process(C.FriendDeleteRequest p)
        {
            if (Stage != GameStage.Game) return;
            // 删除发起者要删除的目标好友
            Player.FriendDelete(p.LinkID, true);
        }

        public void Process(C.Hermit p)  //隐藏属性加点
        {
            if (Stage != GameStage.Game) return;

            Player.AssignHermit(p.Stat);
        }

        public void Process(C.MarketPlaceHistory p)  //寄售历史记录
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;


            S.MarketPlaceHistory result = new S.MarketPlaceHistory { Index = p.Index, Display = p.Display, ObserverPacket = false };
            Enqueue(result);

            AuctionHistoryInfo info = SEnvir.AuctionHistoryInfoList.Binding.FirstOrDefault(x => x.Info == p.Index && x.PartIndex == p.PartIndex);

            if (info == null) return;

            result.SaleCount = info.SaleCount;
            result.LastPrice = info.LastPrice;

            long average = 0;
            int count = 0;

            foreach (int value in info.Average)
            {
                if (value == 0) break;

                average += value;
                count++;
            }

            if (count != 0)
            {
                result.AveragePrice = average / count;
            }
            average = 0;
            count = 0;
            foreach (int value in info.GameGoldAverage)
            {
                if (value == 0) break;

                average += value;
                count++;
            }
            if (count != 0)
            {
                result.GameGoldAveragePrice = average / count;
            }
            result.GameGoldLastPrice = info.LastGameGoldPrice;
        }
        public void Process(C.MarketPlaceConsign p)  //寄售委托
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceConsign(p);
        }

        public void Process(C.MarketPlaceSearch p)  //寄售索引
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return; //如果不在游戏  是观察者 退出 

            MPSearchResults.Clear();  //寄售搜索内容清除
            VisibleResults.Clear();   //寄售可见的内容清除

            HashSet<int> matches = new HashSet<int>();

            foreach (ItemInfo info in SEnvir.ItemInfoList.Binding)  //遍历道具信息绑定
            {
                try
                {
                    if (!string.IsNullOrEmpty(p.Name) && info.ItemName.IndexOf(p.Name, StringComparison.OrdinalIgnoreCase) < 0) continue;

                    matches.Add(info.Index);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }

            foreach (AuctionInfo info in SEnvir.AuctionInfoList.Binding)  //遍历寄售信息绑定
            {
                if (info.Item == null || info.PriceType != p.PriceType) continue;

                if (p.ItemTypeFilter && info.Item.Info.ItemType != p.ItemType) continue;

                switch (info.Item.Info.Effect)
                {
                    case ItemEffect.ItemPart:
                        if (!matches.Contains(info.Item.Stats[Stat.ItemIndex])) continue;
                        break;
                    default:
                        if (!matches.Contains(info.Item.Info.Index)) continue;
                        break;
                }
                if (!string.IsNullOrEmpty(p.SellName) && p.SellName != info.Character.CharacterName) continue;
                MPSearchResults.Add(info);
            }

            switch (p.Sort)  //商城分类
            {
                case MarketPlaceSort.Newest:
                    MPSearchResults.Sort((x1, x2) => x2.Index.CompareTo(x1.Index));
                    break;
                case MarketPlaceSort.Oldest:
                    MPSearchResults.Sort((x1, x2) => x1.Index.CompareTo(x2.Index));
                    break;
                case MarketPlaceSort.HighestPrice:
                    MPSearchResults.Sort((x1, x2) => x2.Price.CompareTo(x1.Price));
                    break;
                case MarketPlaceSort.LowestPrice:
                    MPSearchResults.Sort((x1, x2) => x1.Price.CompareTo(x2.Price));
                    break;
            }

            //Send Rows 1 ~ 9
            List<ClientMarketPlaceInfo> results = new List<ClientMarketPlaceInfo>();

            foreach (AuctionInfo info in MPSearchResults)
            {
                if (results.Count >= 9) break;

                results.Add(info.ToClientInfo(Account));
                VisibleResults.Add(info);
            }

            Enqueue(new S.MarketPlaceSearch { Count = MPSearchResults.Count, Results = results, ObserverPacket = false });
        }
        public void Process(C.MarketPlaceSearchIndex p)  //寄售搜索索引
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            if (p.Index < 0 || p.Index >= MPSearchResults.Count) return;

            AuctionInfo info = MPSearchResults[p.Index];

            if (VisibleResults.Contains(info)) return;

            VisibleResults.Add(info);

            Enqueue(new S.MarketPlaceSearchIndex { Index = p.Index, Result = info.ToClientInfo(Account), ObserverPacket = false });
        }
        public void Process(C.MarketPlaceCancelConsign p)  //取消寄售
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceCancelConsign(p);
        }
        public void Process(C.MarketPlaceBuy p)  //寄售购买
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceBuy(p);
        }
        public void Process(C.MarketPlaceStoreBuy p)  //商城购买
        {
            if (Stage != GameStage.Game) return;

            Player.MarketPlaceStoreBuy(p);
        }

        public void Process(C.MailOpened p)  //邮件已打开
        {
            if (Stage != GameStage.Game) return;

            MailInfo mail = Account.Mail.FirstOrDefault(x => x.Index == p.Index);

            if (mail == null) return;

            mail.Opened = true;
        }
        public void Process(C.GoldMarketTrade p)  //交易
        {
            if (Stage != GameStage.Game) return;

            Player.GoldMarketTrade(p);
        }

        public void Process(C.GoldMarketCannel p)  //交易
        {
            if (Stage != GameStage.Game) return;

            Player.GoldMarketCannel(p);
        }
        public void Process(C.GoldMarketFlash p)  //交易
        {
            if (Stage != GameStage.Game) return;

            Player.GoldMarketFlash();
        }
        public void Process(C.GoldMarketGetOwnOrder p)  //交易
        {
            if (Stage != GameStage.Game) return;

            Player.GoldMarketGetOwnOrder();
        }

        public void Process(C.AutionsAdd p)
        {
            if (Stage != GameStage.Game) return;
            Player.AutionsAdd(p);
        }
        public void Process(C.AutinsBuy p)
        {
            if (Stage != GameStage.Game) return;
            Player.AutionsBuy(p);
        }
        public void Process(C.AutionsFlash p)
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return; //如果不在游戏  是观察者 退出 
            if (DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("9:00")) < 0 || DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("22:59")) > 0) return;
            AutionSearchResults.Clear();
            // AutionVisibleResults.Clear();
            if (string.IsNullOrEmpty(p.Name))
                AutionSearchResults = SEnvir.NewAutionInfoList.Where(x => x.Closed == false).ToList();
            else
                AutionSearchResults = SEnvir.NewAutionInfoList.Where(x => x.Closed == false && x.Item.Info.ItemName.Equals(p.Name)).ToList();
            //int count = 0;
            //foreach (var info in AutionSearchResults)
            //{
            //    if (count >= 13) break;
            //    AutionVisibleResults.Add(info);
            //     count++;
            //}
            Enqueue(new S.NewAuctionFlash { NewAuctionsList = AutionSearchResults.Where(x => true).Select(x => x.ToClientInfo()).ToList() });
        }
        public void Process(C.AutionsFlashIndex p)  //寄售搜索索引
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;
            if (DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("8:00")) < 0 || DateTime.Compare(Convert.ToDateTime(SEnvir.Now.ToString("HH:mm")), Convert.ToDateTime("22:59")) > 0) return;
            if (p.Index < 0 || p.Index >= AutionSearchResults.Count) return;
            var info = AutionSearchResults[p.Index];
            if (AutionVisibleResults.Contains(info)) return;

            AutionVisibleResults.Add(info);

            Enqueue(new S.NewAuctionFlashIndex { Index = p.Index, Result = info.ToClientInfo(), ObserverPacket = false });
        }
        public void Process(C.MailGetItem p)  //邮件获得道具
        {
            if (Stage != GameStage.Game) return;

            Player.MailGetItem(p);
        }
        public void Process(C.MailDelete p)   //邮件删除
        {
            if (Stage != GameStage.Game) return;

            Player.MailDelete(p.Index);
        }
        public void Process(C.MailSend p)  //发邮件
        {
            if (Stage != GameStage.Game) return;

            Player.MailSend(p);
        }

        public void Process(C.ChangeAttackMode p)  //攻击模式改变
        {
            if (Stage != GameStage.Game) return;

            if (Player.CurrentMap.Info.AttackMode != AttackMode.Peace)
            {
                Player.AttackMode = Player.CurrentMap.Info.AttackMode;
                Enqueue(new S.ChangeAttackMode { Mode = Player.CurrentMap.Info.AttackMode });
            }
            else
            {
                switch (p.Mode)
                {
                    case AttackMode.Peace:
                    case AttackMode.Group:
                    case AttackMode.Guild:
                    case AttackMode.WarRedBrown:
                    case AttackMode.All:
                        Player.AttackMode = p.Mode;
                        Enqueue(new S.ChangeAttackMode { Mode = p.Mode });
                        break;
                }
            }
        }
        public void Process(C.ChangePetMode p)   //宠物攻击模式改变
        {
            if (Stage != GameStage.Game) return;

            switch (p.Mode)
            {
                case PetMode.Both:
                case PetMode.Move:
                case PetMode.Attack:
                case PetMode.PvP:
                case PetMode.None:
                    Player.PetMode = p.Mode;
                    Enqueue(new S.ChangePetMode { Mode = p.Mode });
                    break;
            }
        }

        public void Process(C.ItemSplit p)  //道具拆分
        {
            if (Stage != GameStage.Game) return;

            Player.ItemSplit(p);
        }

        public void Process(C.ItemLock p)  //道具锁定
        {
            if (Stage != GameStage.Game) return;

            Player.ItemLock(p);
        }

        public void Process(C.TradeRequest p)  //交易请求
        {
            if (Stage != GameStage.Game) return;

            Player.TradeRequest(p.Index);
        }

        public void Process(C.GetFixedPointinfo p)  //快捷记忆栏传送
        {
            if (Stage != GameStage.Game) return;
            Player.Enqueue(new S.sc_FixedPointList
            {
                FixedPointTCount = Player.Character.FixedPointTCount,
                Info = Player.FPoints.Select(x => x.ToClientInfo()).ToList()
            });
        }

        public void Process(C.TradeRequestResponse p)  //交易请求响应
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.TradeAccept();

            Player.TradePartnerRequest = null;
        }
        public void Process(C.TradeClose p)  //交易取消
        {
            if (Stage != GameStage.Game) return;

            Player.TradeClose();
        }
        public void Process(C.TradeAddItem p)  //交易增加道具
        {
            if (Stage != GameStage.Game) return;

            Player.TradeAddItem(p.Cell);
        }
        public void Process(C.TradeAddGold p)   //交易增加金币
        {
            if (Stage != GameStage.Game) return;

            Player.TradeAddGold(p.Gold);
        }
        public void Process(C.TradeConfirm p)  //交易确认
        {
            if (Stage != GameStage.Game) return;

            Player.TradeConfirm();
        }

        public void Process(C.GuildCreate p)  //行会创建
        {
            if (Stage != GameStage.Game) return;

            Player.GuildCreate(p);
        }
        public void Process(C.GuildEditNotice p)  //行会编辑公告
        {
            if (Stage != GameStage.Game) return;

            Player.GuildEditNotice(p);
        }
        public void Process(C.GuildEditVaultNotice p)  //行会编辑金库公告
        {
            if (Stage != GameStage.Game) return;

            Player.GuildEditVaultNotice(p.Notice);
        }

        public void Process(C.GuildDonation p)
        {
            if (Stage != GameStage.Game) return;

            Player.DonateGoldToGuild(p.Amount);
        }

        public void Process(C.GuildGameGold p)//行会赞助币的提取或捐赠
        {
            if (Stage != GameStage.Game) return;
            Player.UpdateGuildGameGoldTotal(p);
        }

        public void Process(C.GuildWithdrawFund p)
        {
            if (Stage != GameStage.Game) return;

            Player.WithdrawGoldFromGuild(p.Amount);
        }
        public void Process(C.GuildUpdate p)
        {
            if (Stage != GameStage.Game) return;
            Player.GuildlLevelUp();
        }
        public void Process(C.GuildEditMember p)  //行会编辑成员
        {
            if (Stage != GameStage.Game) return;

            Player.GuildEditMember(p);
        }
        public void Process(C.GuildTax p)  //行会税率
        {
            if (Stage != GameStage.Game) return;

            Player.GuildTax(p);
        }
        public void Process(C.GuildFlag p)   //行会旗帜
        {
            if (Stage != GameStage.Game) return;

            Player.GuildFlag(p);
        }
        public void Process(C.GuildIncreaseMember p)  //行会增加成员
        {
            if (Stage != GameStage.Game) return;

            Player.GuildIncreaseMember(p);
        }
        public void Process(C.GuildIncreaseStorage p)  //行会增加仓库容量
        {
            if (Stage != GameStage.Game) return;

            Player.GuildIncreaseStorage(p);
        }
        public void Process(C.GuildInviteMember p)   //行会邀请成员
        {
            if (Stage != GameStage.Game) return;

            Player.GuildInviteMember(p);
        }
        public void Process(C.GuildKickMember p)   //行会踢除成员
        {
            if (Stage != GameStage.Game) return;

            Player.GuildKickMember(p);
        }
        public void Process(C.GuildResponse p)  //行会响应
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.GuildJoin();

            Player.GuildInvitation = null;
        }
        public void Process(C.GuildWar p)   //行会战
        {
            if (Stage != GameStage.Game) return;

            Player.GuildWar(p.GuildName);
        }
        public void Process(C.GuildAlliance p)   //行会联盟
        {
            if (Stage != GameStage.Game) return;

            Player.RequestGuildAlliance(p.GuildName);
        }
        public void Process(C.EndGuildAlliance p)   //行会取消联盟
        {
            if (Stage != GameStage.Game) return;

            Player.EndGuildAlliance(p.GuildName);
        }
        public void Process(C.GuildRequestConquest p)  //行会请求攻城战
        {
            if (Stage != GameStage.Game) return;

            Player.GuildConquest(p.Index);
        }

        public void Process(C.QuestAccept p)  //任务接受
        {
            if (Stage != GameStage.Game) return;

            Player.QuestAccept(p.Index);
        }
        public void Process(C.QuestComplete p)  //任务完成
        {
            if (Stage != GameStage.Game) return;

            Player.QuestComplete(p);
        }
        public void Process(C.QuestTrack p)  //任务追踪
        {
            if (Stage != GameStage.Game) return;

            Player.QuestTrack(p);
        }

        public void Process(C.CompanionUnlock p)  //宠物解锁
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionUnlock(p.Index);
        }
        public void Process(C.CompanionAutoFeedUnlock p)//宠物自动喂食解锁
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionAutoFeedUnlock(p.Index);
        }

        public void Process(C.CompanionAdopt p)   //宠物领养
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionAdopt(p);
        }
        public void Process(C.CompanionRetrieve p)  //宠物取回
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionRetrieve(p.Index);
        }

        public void Process(C.CompanionStore p)     //宠物寄存
        {
            if (Stage != GameStage.Game) return;

            Player.CompanionStore(p.Index);
        }

        public void Process(C.MarriageResponse p)  //结婚响应
        {
            if (Stage != GameStage.Game) return;

            if (p.Accept)
                Player.MarriageJoin();

            Player.MarriageInvitation = null;
        }
        public void Process(C.MarriageMakeRing p)  //结婚戒指制作
        {
            if (Stage != GameStage.Game) return;

            Player.MarriageMakeRing(p.Slot);

        }
        public void Process(C.MarriageTeleport p)  //结婚传送
        {
            if (Stage != GameStage.Game) return;

            Player.MarriageTeleport();
        }

        public void Process(C.BlockAdd p)  //黑名单增加
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            if (Account == null) return;
            //获取用户信息
            CharacterInfo info = SEnvir.GetCharacter(p.Name);

            //用户未找到
            if (info == null)
            {
                //给出提示信息：未找到用户
                ReceiveChat("System.CannotFindPlayer".Lang(Language, p.Name), MessageType.System);

                return;
            }
            //判断是否已经在黑名单
            foreach (BlockInfo blockInfo in Account.BlockingList)
            {
                if (blockInfo.BlockedAccount == info.Account)
                {
                    //给出提示信息用户已经在黑名单
                    ReceiveChat("System.AlreadyBlocked".Lang(Language, p.Name), MessageType.System);
                    return;
                }
            }

            //创建一个黑名单数据库泛型
            BlockInfo block = SEnvir.BlockInfoList.CreateNewObject();

            block.Account = Account;
            block.BlockedAccount = info.Account;
            block.BlockedName = info.CharacterName;  //角色名

            List<string> LinkIDs = new List<string>();

            foreach (CharacterInfo targetCharacter in Account.Characters)
            {
                foreach (FriendInfo friendInfo in targetCharacter.Friends)
                {
                    if (friendInfo.Name == p.Name)
                    {
                        LinkIDs.Add(friendInfo.LinkID);
                    }
                }
            }

            foreach (string targetLinkID in LinkIDs)
            {
                Player.FriendDelete(targetLinkID, true);

            }

            Enqueue(new S.BlockAdd { Info = block.ToClientInfo(), ObserverPacket = false });
        }
        public void Process(C.BlockRemove p)  //黑名单移除
        {
            if (Stage != GameStage.Game && Stage != GameStage.Observer) return;

            BlockInfo block = Account?.BlockingList.FirstOrDefault(x => x.Index == p.Index);

            if (block == null) return;

            block.Delete();

            Enqueue(new S.BlockRemove { Index = p.Index, ObserverPacket = false });
        }

        public void Process(C.HelmetToggle p)            //显示头盔
        {
            if (Stage != GameStage.Game) return;

            Player.HelmetToggle(p.HideHelmet);
        }

        public void Process(C.ShieldToggle p)           //显示盾牌
        {
            if (Stage != GameStage.Game) return;

            Player.ShieldToggle(p.HideShield);
        }

        public void Process(C.FashionToggle p)           //显示时装
        {
            if (Stage != GameStage.Game) return;

            Player.FashionToggle(p.HideFashion);
        }

        public void Process(C.GenderChange p)  //性别变化
        {
            if (Stage != GameStage.Game) return;


            Player.GenderChange(p);
        }
        public void Process(C.HairChange p)   //发型变化
        {
            if (Stage != GameStage.Game) return;

            Player.HairChange(p);

        }

        public void Process(C.cs_FixedPoint p)   //快捷记忆栏传送
        {
            if (Stage != GameStage.Game) return;
            Player.FixedPointSet(p);
        }
        public void Process(C.cs_FixedPointMove p)
        {
            if (Stage != GameStage.Game) return;
            Player.FixedPointMove(p);
        }

        public void Process(C.ArmourDye p)  //衣服染色
        {
            if (Stage != GameStage.Game) return;

            Player.ArmourDye(p.ArmourColour);
        }
        public void Process(C.NameChange p)  //名字改变
        {
            if (Stage != GameStage.Game) return;

            Player.NameChange(p.Name);
        }

        public void Process(C.FortuneCheck p)  //财富改变
        {
            if (Stage != GameStage.Game) return;

            Player.FortuneCheck(p.ItemIndex);
        }
        public void Process(C.TeleportRing p)  //传送戒指
        {
            if (Stage != GameStage.Game) return;

            Player.TeleportRing(p.Location, p.Index);

        }
        public void Process(C.JoinStarterGuild p)  //加入新手行会
        {
            if (Stage != GameStage.Game) return;

            Player.JoinStarterGuild();

        }
        public void Process(C.NPCAccessoryReset p)  //NPC附件重置
        {
            if (Stage != GameStage.Game) return;

            Player.NPCAccessoryReset(p);
        }

        public void Process(C.NPCWeaponCraft p)  //NPC武器工艺
        {
            if (Stage != GameStage.Game) return;

            Player.NPCWeaponCraft(p);
        }
        public void Process(C.TreasureSelect p)  //传奇宝箱
        {
            if (Stage != GameStage.Game) return;
            Player.SelectTrea(p);
        }
        public void Process(C.TreasureChange p)  //传奇宝箱变化
        {
            if (Stage != GameStage.Game) return;
            Player.ChangeTrea(p);
        }

        public void Process(C.AttachGem p)  //宝石
        {
            if (Stage == GameStage.Game)
            {
                Player.AttachGemToItem(p);
            }
        }

        public void Process(C.AddHole p)   //宝石
        {
            if (Stage == GameStage.Game)
            {
                Player.AddHole(p);
            }
        }

        public void Process(C.RemoveGem p)  //宝石
        {
            if (Stage == GameStage.Game)
            {
                Player.RemoveAllGems(p);
            }
        }

        public void Process(C.CraftStart p) //打造
        {
            if (Stage == GameStage.Game)
            {
                Player.InitializeCrating(p.TargetItemIndex);
            }
        }

        public void Process(C.CraftCancel p) //打造
        {
            if (Stage == GameStage.Game)
            {
                Player.CancelCrafting();
            }
        }

        public void Process(C.WearAchievementTitle p)   //佩戴成就名称
        {
            UserAchievement achievement = Player.Character.Achievements.FirstOrDefault(x => x.AchievementName.Index == p.AchievementIndex);
            if (achievement != null)
            {
                Player.Character.AchievementTitle = achievement.AchievementName.Title;
                Player.Broadcast(new S.AchievementTitleChanged
                {
                    ObjectID = Player.ObjectID,
                    NewTitle = achievement.AchievementName.Title
                });
            }
        }

        public void Process(C.TakeOffAchievementTitle p)  //取下成就名称
        {
            Player.Character.AchievementTitle = string.Empty;
            Player.Broadcast(new S.AchievementTitleChanged
            {
                ObjectID = Player.ObjectID,
                NewTitle = string.Empty
            });
        }

        public void Process(C.GetDailyQuest p)  //获取每日任务
        {
            if (Stage == GameStage.Game)
            {
                Player.GetDailyQuest();
            }
        }

        public void Process(C.ShortcutDialogClicked p)    //菜单栏
        {
            if (Stage == GameStage.Game)
            {
                Player.LoadShortcutConfig();
            }
        }

        public void Process(C.FishingCast p)  //钓鱼抛竿
        {
            if (Stage == GameStage.Game)
            {
                Player.FishingCast();
            }
        }

        public void Process(C.FishingReel p)  //钓鱼收杆
        {
            if (Stage == GameStage.Game)
            {
                Player.FishingReel(SEnvir.Now);
            }
        }

        public void Process(C.NPCWeaponUpgrade p)  //新版武器升级
        {
            if (Stage != GameStage.Game) return;

            Player.WeaponUpgrade(p);
        }
        public void Process(C.NPCWeaponUpgradeRetrieve p)  //NPC精炼取回
        {
            if (Stage != GameStage.Game) return;

            Player.WeaponUpgradeRetrieve(p.Index);
        }

        public void Process(C.PyTextBoxResponse p)
        {
            if (Stage != GameStage.Game) return;
            AsyncPyCall action = Player.AsyncPyCallList.FirstOrDefault(x => x.ID == p.ID);
            if (action != null)
            {
                if (p.IsOK)
                {
                    SEnvir.RunPyScriptFromName(action.OKScriptName, PythonOps.MakeTuple(new object[] { Player, p.UserInput }));

                }
                else
                {
                    if (!string.IsNullOrEmpty(action.cancelScriptName))
                    {
                        SEnvir.RunPyScriptFromName(action.cancelScriptName, action.OverriddenParams ?? PythonOps.MakeTuple(new object[] { Player, p.UserInput }));
                    }
                }
                Player.AsyncPyCallList.RemoveAll(x => x.ID == p.ID);
            }
        }

        public void Process(C.EnterAncientTomb p)
        {
            if (Stage != GameStage.Game) return;

            // 古墓任务的序号对应设置的序号
            string correctChar1 = Player.Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == Config.PenetraliumKeyA && !string.IsNullOrEmpty(x.ExtraInfo))?.ExtraInfo;
            string correctChar2 = Player.Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == Config.PenetraliumKeyB && !string.IsNullOrEmpty(x.ExtraInfo))?.ExtraInfo;
            string correctChar3 = Player.Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == Config.PenetraliumKeyC && !string.IsNullOrEmpty(x.ExtraInfo))?.ExtraInfo;

            if (string.IsNullOrEmpty(correctChar1) || string.IsNullOrEmpty(correctChar2) || string.IsNullOrEmpty(correctChar3))
            {
                Player.Connection.ReceiveChat("墓碑字符尚未集齐".Lang(Language), MessageType.Hint);
                return;
            }

            if (string.IsNullOrEmpty(p.FirstChar) || string.IsNullOrEmpty(p.SecondChar) ||
                string.IsNullOrEmpty(p.ThirdChar))
            {
                Player.Connection.ReceiveChat("三个墓碑字符必须全部输入".Lang(Language), MessageType.Hint);
                return;
            }

            if (p.FirstChar == correctChar1 && p.SecondChar == correctChar2 && p.ThirdChar == correctChar3)
            {
                //成功进入等待室
                Map Map = SEnvir.GetMap(SEnvir.RightDeliver.Map);
                Player.Teleport(Map, Map.GetRandomLocation());
                //移除三个集字任务
                IEnumerable<int> firstQuests = Player.GetUserQuestsByQuestIndex(Config.PenetraliumKeyA).Select(x => x.Index);
                foreach (int i in firstQuests)
                {
                    Player.QuestRemoveByUserQuestIndex(i);
                }

                IEnumerable<int> secondQuests = Player.GetUserQuestsByQuestIndex(Config.PenetraliumKeyB).Select(x => x.Index);
                foreach (int i in secondQuests)
                {
                    Player.QuestRemoveByUserQuestIndex(i);
                }

                IEnumerable<int> thirdQuests = Player.GetUserQuestsByQuestIndex(Config.PenetraliumKeyC).Select(x => x.Index);
                foreach (int i in thirdQuests)
                {
                    Player.QuestRemoveByUserQuestIndex(i);
                }
            }
            else
            {
                Map mishiMap = SEnvir.GetMap(SEnvir.ErrorDeliver.Map);
                Player.Teleport(mishiMap, mishiMap.GetRandomLocation());
                Player.Connection.ReceiveChat("Sconnection.ErrorDeliver".Lang(Language), MessageType.Hint);
                //移除三个集字任务
                IEnumerable<int> firstQuests = Player.GetUserQuestsByQuestIndex(Config.PenetraliumKeyA).Select(x => x.Index);
                foreach (int i in firstQuests)
                {
                    Player.QuestRemoveByUserQuestIndex(i);
                }

                IEnumerable<int> secondQuests = Player.GetUserQuestsByQuestIndex(Config.PenetraliumKeyB).Select(x => x.Index);
                foreach (int i in secondQuests)
                {
                    Player.QuestRemoveByUserQuestIndex(i);
                }

                IEnumerable<int> thirdQuests = Player.GetUserQuestsByQuestIndex(Config.PenetraliumKeyC).Select(x => x.Index);
                foreach (int i in thirdQuests)
                {
                    Player.QuestRemoveByUserQuestIndex(i);
                }
                return;
            }
        }

        public void Process(C.GuildAllowApplyChanged p)
        {
            if (Stage != GameStage.Game) return;
            if (Player.Character.Account.GuildMember == null) return;
            if ((Player.Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader) return;
            Player.Character.Account.GuildMember.Guild.AllowApply = p.AllowApply;
        }

        public void Process(C.ApplyJoinGuildPacket p)
        {
            if (Stage != GameStage.Game) return;
            if (Player.Character.Account.GuildMember != null) return;

            Player.ApplyJoinGuild(p.GuildIndex);
        }

        public void Process(C.GetGuileWithDrawal p)
        {
            if (Stage != GameStage.Game) return;
            if (Player?.Character?.Account?.GuildMember == null) return;

            if (Player?.Character?.Account?.GuildMember?.Guild?.PendingWithdrawal == null) return;

            //解决查询问题 
            if ((Player.Character.Account.GuildMember.Permission & GuildPermission.Leader) == GuildPermission.Leader)
            {
                List<string> applicants = new List<string>();
                foreach (var characterIndex in Player?.Character?.Account?.GuildMember?.Guild?.PendingWithdrawal.Keys)
                {
                    CharacterInfo applicant = SEnvir.GetCharacter(characterIndex);
                    if (applicant == null) continue;
                    applicants.Add($"{applicant.Index},{applicant.Account.Connection != null},{applicant.CharacterName},{Player?.Character?.Account?.GuildMember?.Guild?.PendingWithdrawal[characterIndex]},{applicant.LastLogin.ToString("MM/dd HH:mm")}");

                }
                Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });

                Enqueue(new S.UpdateGuildGameTotal
                {
                    Amount = Player.Character.Account.GuildMember.Guild.GameGoldTotal
                });
            }
            else
            {
                int characterIndex = Player?.Character?.Index ?? -1;
                List<string> applicants = new List<string>();
                if (Player?.Character?.Account?.GuildMember?.Guild?.PendingWithdrawal.TryGetValue(characterIndex, out var Value) ?? false)
                {
                    CharacterInfo applicant = SEnvir.GetCharacter(characterIndex);
                    if (applicant != null)
                        applicants.Add($"{applicant.Index},{applicant.Account.Connection != null},{applicant.CharacterName},{Player?.Character?.Account?.GuildMember?.Guild?.PendingWithdrawal[characterIndex]},{applicant.LastLogin.ToString("MM/dd HH:mm")}");
                    Enqueue(new S.GuildWithDrawal { WithDrawals = applicants });
                }
                Enqueue(new S.UpdateGuildGameTotal
                {
                    Amount = Player.Character.Account.GuildMember.GameGoldTotal
                });

            }

        }
        public void Process(C.GetGuildApplications p)
        {
            if (Stage != GameStage.Game) return;
            if (Player?.Character?.Account?.GuildMember == null) return;
            if ((Player.Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember) return;
            if (Player?.Character?.Account?.GuildMember?.Guild?.PendingApplications == null) return;

            List<string> applicants = new List<string>();
            foreach (int characterIndex in Player.Character.Account.GuildMember.Guild.PendingApplications)
            {
                CharacterInfo applicant = SEnvir.GetCharacter(characterIndex);
                if (applicant == null) continue;
                applicants.Add($"{applicant.Index},{applicant.Account.Connection != null},{applicant.CharacterName},{Functions.GetEnumDescription(applicant.Class)},{applicant.Level},{applicant.LastLogin.ToString("MM/dd HH:mm")}");
            }

            Enqueue(new S.GuildApplications { Applicants = applicants });
        }
        /// <summary>
        /// 会长同意
        /// </summary>
        /// <param name="p"></param>
        public void Process(C.GuileWithdrawalApplyChoice p)
        {
            if (Stage != GameStage.Game) return;
            if (Player.Character.Account.GuildMember == null) return;
            if ((Player.Character.Account.GuildMember.Permission & GuildPermission.Leader) != GuildPermission.Leader) return;
            //处理玩家提取申请
            Player.ProcessWithdrawal(p.PlayerIndex, p.Approved);
        }
        public void Process(C.GuildApplyChoice p)
        {
            if (Stage != GameStage.Game) return;
            if (Player.Character.Account.GuildMember == null) return;
            if ((Player.Character.Account.GuildMember.Permission & GuildPermission.AddMember) != GuildPermission.AddMember) return;

            Player.ProcessApplication(p.PlayerIndex, p.Approved);
        }

        public void Process(C.TossCoin p)
        {
            if (Stage != GameStage.Game) return;
            Player.LuckyCoinToss(p.Angle, p.InitialDistance, p.SelectedDistance, p.TossOption);
        }

        public void Process(C.RemoveTaishanBuff p)
        {
            if (Stage != GameStage.Game) return;
            Player.RemoveTaishanBuff(p.BuffIndex);
        }

        public void Process(C.GiveUpQuest p)
        {
            if (Stage != GameStage.Game) return;
            Player.GiveUpQuest(p.QuestIndex);
        }

        public void Process(C.AccessoryCombineRequest p)  //新版首饰合成
        {
            if (Stage != GameStage.Game) return;

            Player.AccessoryCombine(p);
        }

        public void Process(C.CompanionPickUpSkipUpdate p)
        {
            if (Stage != GameStage.Game) return;
            if (Player?.CompanionPickUpSkips == null) return;
            if (p.CompanionPickupSkipList == null) return;

            Player.CompanionPickUpSkips.Clear();
            Player.CompanionPickUpSkips.UnionWith(p.CompanionPickupSkipList);
        }

        public void Process(C.ClaimRedpacket p)
        {
            if (Stage == GameStage.Game)
            {
                var rp = SEnvir.RedPacketInfoList.Binding.FirstOrDefault(x => x.Index == p.RedpacketIndex);
                if (rp != null)
                {
                    Player.OpenRedPacket(rp);
                }
                else
                {
                    Player.Connection.ReceiveChat($"系统找不到指定的红包。Index = {p.RedpacketIndex}", MessageType.System);
                }
            }
        }
        public void Process(C.HuiShengToggle p)
        {
            if (Stage == GameStage.Game)
            {
                Player.Connection.Account.AllowResurrectionOrder = p.HuiSheng;
            }
        }

        public void Process(C.ReCallToggle p)
        {
            if (Stage == GameStage.Game)
            {
                Player.Connection.Account.AllowGroupRecall = p.Recall;
            }
        }

        public void Process(C.TradeToggle p)
        {
            if (Stage == GameStage.Game)
            {
                Player.Connection.Account.AllowTrade = p.Trade;
            }
        }

        public void Process(C.GuildToggle p)
        {
            if (Stage == GameStage.Game)
            {
                Player.Connection.Account.AllowGuild = p.Guild;
            }
        }

        public void Process(C.Ammunition p)
        {
            if (Stage != GameStage.Game) return;
            //判断有没有攻城器
            bool isTou = false;
            MonsterObject owPet = null;
            foreach (var pet in Player.Pets)
            {
                if (pet.MonsterInfo.Image == MonsterImage.Catapult)  //攻城车
                {
                    isTou = true;
                    owPet = pet;
                    break;
                }
                if (pet.MonsterInfo.Image == MonsterImage.Ballista)  //投石车
                {
                    owPet = pet;
                    break;
                }
            }
            if (owPet == null) return;
            //判断背包物品类型 和数量
            if (p.Slot < 0 || p.Slot >= Globals.InventorySize) return;
            var item = Player.Inventory[p.Slot];
            if (item == null) return;
            // 判断攻城器类型 
            if (isTou)
            {
                if (item.Info.Image != 311) return;
            }
            else
            {
                if (item.Info.Image != 310) return;
            }

            if (item.Count >= p.Count)
            {
                item.Count -= p.Count;

                Enqueue(new S.ItemChanged { Link = new CellLinkInfo { GridType = GridType.Inventory, Slot = p.Slot, Count = item.Count }, Success = true });
                if (item.Count == 0)
                {

                    Player.RemoveItem(item);//移除项目（道具）}
                    Player.Inventory[p.Slot] = null;
                    item.Delete();//道具 删除
                }
            }
            else return;
            //装填
            (owPet as SiegeEquipment).ZhuangDan(p.Count);
        }

        public void Process(C.WarWeapMove p)
        {
            if (Stage != GameStage.Game) return;
            MonsterObject owPet = null;
            foreach (var pet in Player.Pets)
            {
                if (pet.MonsterInfo.Image == MonsterImage.Catapult || pet.MonsterInfo.Image == MonsterImage.Ballista)  //攻城车
                {
                    owPet = pet;
                    break;
                }
            }
            if (owPet == null) return;

            (owPet as SiegeEquipment).AttackStop = true;
            (owPet as SiegeEquipment).MoveMode = true;
        }
        public void Process(C.WarWeapAttackStop p)
        {
            if (Stage != GameStage.Game) return;
            MonsterObject owPet = null;
            foreach (var pet in Player.Pets)
            {
                if (pet.MonsterInfo.Image == MonsterImage.Catapult || pet.MonsterInfo.Image == MonsterImage.Ballista)  //攻城车
                {
                    owPet = pet;
                    break;
                }
            }
            if (owPet == null) return;

            (owPet as SiegeEquipment).AttackStop = true;
            (owPet as SiegeEquipment).MoveMode = false;
        }
        public void Process(C.WarWeapAttackCoordinates p)
        {
            if (Stage != GameStage.Game) return;
            MonsterObject owPet = null;
            foreach (var pet in Player.Pets)
            {
                if (pet.MonsterInfo.Image == MonsterImage.Catapult || pet.MonsterInfo.Image == MonsterImage.Ballista)  //攻城车
                {
                    owPet = pet;
                    break;
                }
            }
            if (owPet == null) return;
            (owPet as SiegeEquipment).MoveMode = false;
            (owPet as SiegeEquipment).miaozhuangmubiao(p.X, p.Y);
        }

        public void Process(C.FriendToggle p)
        {
            if (Stage == GameStage.Game)
            {
                Player.Connection.Account.AllowFriend = p.Friend;
            }
        }

        /// <summary>
        /// 客户端发来的所有进程的哈希值
        /// </summary>
        /// <param name="p"></param>
        public void Process(C.ResponseProcessHash p)
        {
            if (Stage == GameStage.Game)
            {
                // Player.Connection.Account.AllowFriend = p.Friend;
                if (Player.HackTime.AddMilliseconds(30000 * 96 / 100) >= SEnvir.Now)
                {
                    //SEnvir.Log("123");
                    Player.HackCount++;
                }
                else
                {
                    Player.HackCount = 0;
                }
                Player.HackTime = SEnvir.Now;
                if (Player.HackCount >= 5)
                {
                    Player.HackCount = 0;
                    SEnvir.Log($"****** 发现玩家开挂 ******");
                    //SEnvir.Log($"****** 账号: {Account?.EMailAddress}, 角色: {this.Account?.LastCharacter?.CharacterName}, IP: {IPAddress} ******");
                    SEnvir.Log(string.Format("****** 账号: {0}, 角色: {1}, IP: {2} ******", Account?.EMailAddress, Account?.LastCharacter?.CharacterName, IPAddress));
                    SendDisconnect(new G.Disconnect { Reason = DisconnectReason.PlugInDetection });
                }
            }

            foreach (var hash in p.HashList)
            {
                if (SEnvir.CheckProcessSha256InBanList(hash))
                {
                    SEnvir.Log($"****** 发现玩家开挂 ******");
                    //SEnvir.Log($"****** 账号: {Account?.EMailAddress}, 角色: {this.Account?.LastCharacter?.CharacterName}, IP: {IPAddress} ******");
                    SEnvir.Log(string.Format("****** 账号: {0}, 角色: {1}, IP: {2} ******", Account?.EMailAddress, Account?.LastCharacter?.CharacterName, IPAddress));
                    SendDisconnect(new G.Disconnect { Reason = DisconnectReason.PlugInDetection });
                    break;
                }
            }
        }

        public void Process(C.SellCharacter p)
        {
            if (Stage != GameStage.Game) return;

            if (Player.Character.ProhibitListing == true)
            {
                Player.Connection.ReceiveChat($"角色被限制，禁止上架。".Lang(Language), MessageType.System);
                return;
            }

            //TODO 
            //手续费验证 
            long count = 10;

            if (count * 100 > Player.Character.Account.GameGold)
            {
                Player.Connection.ReceiveChat($"手续费不足{count}赞助币，无法上架。".Lang(Language), MessageType.System);
                return;
            }

            if (Player.Character.Horse == HorseType.Black && p.Price < 2700)
            {
                Player.Connection.ReceiveChat($"拥有黑马，价格不能低于2700，无法上架。".Lang(Language), MessageType.System);
                return;
            }

            if (Player.Character.Horse == HorseType.Red && p.Price < 900)
            {
                Player.Connection.ReceiveChat($"拥有红马，价格不能低于900，无法上架。".Lang(Language), MessageType.System);
                return;
            }

            if (Player.Character.Horse == HorseType.White && p.Price < 180)
            {
                Player.Connection.ReceiveChat($"拥有白马，价格不能低于180，无法上架。".Lang(Language), MessageType.System);
                return;
            }

            //扣除手续费
            Player.Character.Account.GameGold -= (int)count * 100;
            Player.GameGoldChanged();
            CharacterShop characterShop = SEnvir.CharacterShopList.CreateNewObject();
            characterShop.Character = Player.Character;
            characterShop.Account = Player.Character.Account;
            characterShop.Price = p.Price;
            characterShop.IsSell = true;
            Player.Character.CharacterState = CharacterState.Sell;

            foreach (SConnection con in SEnvir.Connections)
            {
                con.ReceiveChat(" <{0}> 在角色寄售行上架，出售价格 {1} 赞助币。".Lang(con.Language, Player.Character.CharacterName, p.Price), MessageType.System);
                con.ReceiveChat(" <{0}> 在角色寄售行上架，出售价格 {1} 赞助币。".Lang(con.Language, Player.Character.CharacterName, p.Price), MessageType.System);
                con.ReceiveChat(" <{0}> 在角色寄售行上架，出售价格 {1} 赞助币。".Lang(con.Language, Player.Character.CharacterName, p.Price), MessageType.System);
                con.ReceiveChat(" <{0}> 在角色寄售行上架，出售价格 {1} 赞助币。".Lang(con.Language, Player.Character.CharacterName, p.Price), MessageType.System);
                con.ReceiveChat(" <{0}> 在角色寄售行上架，出售价格 {1} 赞助币。".Lang(con.Language, Player.Character.CharacterName, p.Price), MessageType.System);
            }

            //角色下线
            TrySendDisconnect(new G.Disconnect { Reason = DisconnectReason.TimedOut });
        }

        public void Process(C.CanelSellCharacter p)
        {
            if (Stage != GameStage.Select) return;
            //TODO 
            CharacterShop characterShop = SEnvir.CharacterShopList.Where(d => d.Character.Index == p.CharacterIndex && d.Character.CharacterState == CharacterState.Sell && d.Account == this.Account && d.IsSell).FirstOrDefault<CharacterShop>();
            if (characterShop != null)
            {
                characterShop.Character.CharacterState = CharacterState.Normal;
                characterShop.IsSell = false;
                Enqueue(new S.ReFlashSellCharState()
                {
                    CharacterIndex = p.CharacterIndex,
                });
            }
        }

        public void Process(C.SellCharacterSearch p)
        {
            if (Stage != GameStage.Game) return;
            // List<CharacterShop> characterShops = SEnvir.CharacterShopList.Where(d => d.Date.AddHours(48) > SEnvir.Now && d.Character.CharacterState == CharacterState.Sell && d.Account == this.Account).ToList();

            List<CharacterShop> characterShops = SEnvir.CharacterShopList.Where(d => d.Character.CharacterState == CharacterState.Sell && d.IsSell).ToList();
            S.SellCharacterSearch packet = new S.SellCharacterSearch()
            {
                Count = characterShops.Count,
                Results = new List<ClientAccountConsignmentInfo>(),
            };
            for (int i = 0; i < characterShops.Count; i++)
            {
                CharacterInfo target = characterShops[i].Character;
                ClientAccountConsignmentInfo tem = new ClientAccountConsignmentInfo()
                {
                    Index = target.Index,
                    Name = target.CharacterName,
                    Partner = target.Partner?.CharacterName,
                    Class = target.Class,
                    Gender = target.Gender,
                    Stats = target.LastStats,
                    HermitStats = target.HermitStats,
                    HermitPoints = Math.Max(0, target.Level - 39 - target.SpentPoints),
                    Level = target.Level,
                    Hair = target.HairType,
                    HairColour = target.HairColour,
                    Items = new List<ClientUserItem>(),
                    ObserverPacket = false,
                    HideHelmet = target.HideHelmet,
                    HideFashion = target.HideFashion,
                    HideShield = target.HideShield,
                    Price = characterShops[i].Price,
                    Horse = target.Horse,
                    Myself = target.TotalRewardPoolCoinEarned - target.TotalRewardPoolCoinCashedOut,
                };

                foreach (UserItem item in characterShops[i].Character.Items)
                {
                    if (item == null || item.Slot < 0 || item.Slot < Globals.EquipmentOffSet || item.Slot >= Globals.PatchOffSet) continue;

                    ClientUserItem clientItem = item.ToClientInfo();
                    clientItem.Slot -= Globals.EquipmentOffSet;
                    tem.Items.Add(clientItem);
                }
                packet.Results.Add(tem);
            }
            Enqueue(packet);
        }


        public void Process(C.InspectPackSack p)
        {
            if (Stage == GameStage.Game)
            {
                Player.InspectPackSack(p.Index, this);
            }
        }

        public void Process(C.InspectMagery p)
        {
            if (Stage == GameStage.Game)
            {
                Player.InspectMagery(p.Index, this);
            }
        }

        public void Process(C.MarketPlaceJiaoseBuy p)
        {
            if (Stage == GameStage.Game)
            {
                Player.MarketPlaceJiaoseBuy(p);
            }
        }
    }

    /// <summary>
    /// 游戏状态
    /// </summary>
    public enum GameStage
    {
        /// <summary>
        /// 没有
        /// </summary>
        None,
        /// <summary>
        /// 登录
        /// </summary>
        Login,
        /// <summary>
        /// 选择
        /// </summary>
        Select,
        /// <summary>
        /// 在游戏中
        /// </summary>
        Game,
        /// <summary>
        /// 观察者
        /// </summary>
        Observer,
        /// <summary>
        /// 离线的
        /// </summary>
        Disconnected,
    }
}
