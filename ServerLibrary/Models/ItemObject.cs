using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Microsoft.Scripting.Hosting;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 道具对象
    /// </summary>
    public sealed class ItemObject : MapObject
    {
        /// <summary>
        /// 对象类型道具
        /// </summary>
        public override ObjectType Race => ObjectType.Item;
        /// <summary>
        /// 道具锁定
        /// </summary>
        public override bool Blocking => false;
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 捡取限制时间
        /// </summary>
        public DateTime ProtectTime { get; set; }
        /// <summary>
        /// 角色道具
        /// </summary>
        public UserItem Item { get; set; }
        /// <summary>
        /// 账户信息
        /// </summary>
        public AccountInfo Account { get; set; } //断开连接时使用帐户而不是playerObject
        /// <summary>
        /// 怪物爆出
        /// </summary>
        public bool MonsterDrop { get; set; }

        public override void Process()
        {
            base.Process();

            if (SEnvir.Now > ExpireTime)
            {
                Despawn();
                return;
            }

        }
        /// <summary>
        /// 已经生成
        /// </summary>
        public override void OnDespawned()
        {
            base.OnDespawned();

            if (Item.UserTask != null)
            {
                Item.UserTask.Objects.Remove(this);
                Item.UserTask = null;
                Item.Flags &= ~UserItemFlags.QuestItem;
            }

            Item = null;
            Account = null;
        }
        /// <summary>
        /// 安全生成
        /// </summary>
        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();

            if (Item.UserTask != null)
            {
                Item.UserTask.Objects.Remove(this);
                Item.UserTask = null;
                Item.Flags &= ~UserItemFlags.QuestItem;
            }

            Item = null;
            Account = null;
        }
        /// <summary>
        /// 玩家捡取道具
        /// </summary>
        /// <param name="ob">玩家</param>
        /// <returns></returns>
        public bool PickUpItem(PlayerObject ob)
        {
            if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound)        //如果是绑定物品
            {
                ob.Connection.ReceiveChat("绑定物品不能捡取", MessageType.System);
                return false;
            }

            if (Account != null && Account != ob.Character.Account && ProtectTime > SEnvir.Now) //如果（账号 为 空  且  账号不是个人账号  且 物品捡取时间 > 设定能捡取时间） 
            {
                bool canpick = false;
                if (ob.GroupMembers != null)//这里判断是否组队
                {
                    foreach (PlayerObject player in ob.GroupMembers)
                    {
                        if (player.Character.Account == Account)
                        {
                            canpick = true;
                            break;
                        }
                    }
                }
                if (!canpick)
                {
                    ob.Connection.ReceiveChat("一定时间内不能捡取", MessageType.System);
                    return false;  // 返回 false
                }
            }

            long amount = 0;

            if (Account != null && Item.Info.Effect == ItemEffect.Gold && Account.GuildMember != null && Account.GuildMember.Guild.GuildTax > 0)
                amount = (long)Math.Ceiling(Item.Count * Account.GuildMember.Guild.GuildTax);

            ItemCheck check = new ItemCheck(Item, Item.Count - amount, Item.Flags, Item.ExpireTime);

            // 超重后不再拾取
            if (ob.CanGainItems(true, check))
            {
                if (amount > 0)
                {
                    Item.Count -= amount;

                    Account.GuildMember.Guild.GuildFunds += amount;
                    Account.GuildMember.Guild.DailyGrowth += amount;

                    Account.GuildMember.Guild.DailyContribution += amount;
                    Account.GuildMember.Guild.TotalContribution += amount;

                    Account.GuildMember.DailyContribution += amount;
                    Account.GuildMember.TotalContribution += amount;

                    foreach (GuildMemberInfo member in Account.GuildMember.Guild.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildMemberContribution { Index = Account.GuildMember.Index, Contribution = amount, ObserverPacket = false });
                    }
                }

                Item.UserTask?.Objects.Remove(this);

                #region 玩家拾取物品

                //python 触发
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { ob, CurrentMap.Info, Item });
                        SEnvir.ExecutePyWithTimer(trig_player, ob, "OnPickUpItem", args);
                        //trig_player(ob, "OnPickUpItem", args);
                    }
                }
                catch (System.Data.SyntaxErrorException e)
                {
                    string msg = "PlayerEvent Syntax error : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (SystemExitException e)
                {
                    string msg = "PlayerEvent SystemExit : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (Exception ex)
                {
                    string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, error));
                }

                #endregion

                ob.GainItem(Item);

                Despawn();

                return true;
            }

            //获取类型的最大进位
            //按类型减少数量。
            //发送更新的楼层计数
            //获得新的/部分项目
            return false;
        }
        public void PickUpItem(Companion ob)  //宠物捡取道具
        {
            if ((Item.Flags & UserItemFlags.Bound) == UserItemFlags.Bound) return;  //绑定物品就 返回

            if (Account != null && Account != ob.CompanionOwner.Character.Account) return;

            long amount = 0;

            if (Account != null && Item.Info.Effect == ItemEffect.Gold && Account.GuildMember != null && Account.GuildMember.Guild.GuildTax > 0)
                amount = (long)Math.Ceiling(Item.Count * Account.GuildMember.Guild.GuildTax);

            ItemCheck check = new ItemCheck(Item, Item.Count - amount, Item.Flags, Item.ExpireTime);

            if (ob.CanGainItems(false, check))
            {
                // 是否被过滤了
                if (ob.CompanionOwner.CompanionPickUpSkips.Contains(Item.Info.Index))
                {
                    return;
                }
                if (amount > 0)
                {
                    Item.Count -= amount;

                    Account.GuildMember.Guild.GuildFunds += amount;
                    Account.GuildMember.Guild.DailyGrowth += amount;

                    Account.GuildMember.Guild.DailyContribution += amount;
                    Account.GuildMember.Guild.TotalContribution += amount;

                    Account.GuildMember.DailyContribution += amount;
                    Account.GuildMember.TotalContribution += amount;

                    foreach (GuildMemberInfo member in Account.GuildMember.Guild.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildMemberContribution { Index = Account.GuildMember.Index, Contribution = amount, ObserverPacket = false });
                    }
                }

                #region 玩家拾取物品

                //python 触发
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { ob.CompanionOwner, CurrentMap.Info, Item });
                        SEnvir.ExecutePyWithTimer(trig_player, ob.CompanionOwner, "OnPickUpItem", args);
                        //trig_player(ob.CompanionOwner, "OnPickUpItem", args);
                    }
                }
                catch (System.Data.SyntaxErrorException e)
                {
                    string msg = "PlayerEvent Syntax error : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (SystemExitException e)
                {
                    string msg = "PlayerEvent SystemExit : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (Exception ex)
                {
                    string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, error));
                }

                #endregion

                Item.UserTask?.Objects.Remove(this);

                ob.GainItem(Item);

                Despawn();
                return;
            }


            return;
        }


        public override void ProcessHPMP()
        {
        }
        public override void ProcessNameColour()
        {
        }
        public override void ProcessBuff()
        {
        }
        public override void ProcessPoison()
        {
        }

        /// <summary>
        /// 道具可以让所有人看到
        /// </summary>
        /// <param name="ob">玩家</param>
        /// <returns></returns>
        public override bool CanBeSeenBy(PlayerObject ob)
        {
            //if (Account != null && ob.Character.Account != Account) return false;   // 如果（账号 为 空  且  账号不是个人账号 ） 返回 false
            if (ob?.Character == null) return false;
            if (Item == null) return false;

            if (Item.UserTask != null && Item.UserTask?.Quest?.Character != ob.Character) return false;

            return base.CanBeSeenBy(ob);
        }
        /// <summary>
        /// 激活
        /// </summary>
        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        /// <summary>
        /// 停用
        /// </summary>
        public override void DeActivate()
        {
            return;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();

            ExpireTime = SEnvir.Now + Config.DropDuration;  //过期时间=系统时间+设置的时间

            AddAllObjects();  //添加所有对象

            Activate();  //激活
        }
        public override Packet GetInfoPacket(PlayerObject ob)  //获取信息包
        {
            return new S.ObjectItem
            {
                ObjectID = ObjectID,
                Item = Item.ToClientInfo(),
                Location = CurrentLocation,
            };
        }
        public override Packet GetDataPacket(PlayerObject ob)  //获取数据包
        {
            return new S.DataObjectItem
            {
                ObjectID = ObjectID,

                MapIndex = CurrentMap.Info.Index,
                CurrentLocation = CurrentLocation,

                ItemIndex = Item.Info.Index,
            };
        }
    }
}
