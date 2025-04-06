using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Microsoft.Scripting.Hosting;
using NLog;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager.Events;
using Server.Utils.Logging;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject // 结婚
    {
        #region Marriage
        //结婚
        private long MarriageCost = 0;
        public void MarriageRequest(int needlev = 22, long cost = 500000)  //要求等级  金币
        {
            MarriageCost = cost;
            if (Character.Partner != null)   //角色配偶不为空 跳出
            {
                Connection.ReceiveChat("Marriage.MarryAlreadyMarried".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (Level < needlev)  //等于小于设置的等级 跳出
            {
                Connection.ReceiveChat("Marriage.MarryNeedLevel".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (Gold < cost)  //钱少于设置的值 跳出
            {
                Connection.ReceiveChat("Marriage.MarryNeedGold".Lang(Connection.Language), MessageType.System);
                return;
            }

            Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, Direction));

            if (cell?.Objects == null)  //对面的位置没人 跳出
            {
                Connection.ReceiveChat("Marriage.MarryNotFacing".Lang(Connection.Language), MessageType.System);
                return;
            }

            PlayerObject player = null;
            foreach (MapObject ob in cell.Objects)
            {
                if (ob.Race != ObjectType.Player) continue;
                player = (PlayerObject)ob;
                break;
            }

            if (player == null || player.Direction != Functions.ShiftDirection(Direction, 4))   //玩家为空  或者 不是站在对面 跳出
            {
                Connection.ReceiveChat("Marriage.MarryNotFacing".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (player.Character.Gender == Character.Gender)  //相同性别不能结婚 跳出
            {
                Connection.ReceiveChat("Marriage.MarryTargetSameSex".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.Character.Partner != null)   //玩家的配偶信息不为空 跳出
            {
                Connection.ReceiveChat("Marriage.MarryTargetAlreadyMarried".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.MarriageInvitation != null)  //玩家结婚不为空  跳出
            {
                Connection.ReceiveChat("Marriage.MarryTargetHasProposal".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);
                return;
            }

            if (player.Level < needlev)   //玩家的等级小于设置等级 跳出
            {
                Connection.ReceiveChat("Marriage.MarryTargetNeedLevel".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);
                player.Connection.ReceiveChat("Marriage.MarryTargetNeedLevel".Lang(player.Connection.Language), MessageType.System);
                return;
            }

            if (player.Gold < cost)  //玩家的金币小于设置金币 跳出
            {
                Connection.ReceiveChat("Marriage.MarryTargetNeedGold".Lang(Connection.Language, player.Character.CharacterName), MessageType.System);
                player.Connection.ReceiveChat("Marriage.MarryNeedGold".Lang(player.Connection.Language), MessageType.System);
                return;
            }
            if (player.Dead || Dead)  //玩家死亡 跳出
            {
                Connection.ReceiveChat("Marriage.MarryDead".Lang(Connection.Language), MessageType.System);
                player.Connection.ReceiveChat("Marriage.MarryDead".Lang(player.Connection.Language), MessageType.System);
                return;
            }

            player.MarriageInvitation = this;
            player.MarriageCost = cost;
            player.Enqueue(new S.MarriageInvite { Name = Name });
        }
        public void MarriageJoin()
        {
            if (MarriageInvitation != null && MarriageInvitation.Node == null) MarriageInvitation = null;

            if (MarriageInvitation == null || Character.Partner != null || MarriageInvitation.Character.Partner != null) return;

            long cost = MarriageCost;

            if (Gold < cost)
            {
                Connection.ReceiveChat("Marriage.MarryNeedGold".Lang(Connection.Language), MessageType.System);
                MarriageInvitation.Connection.ReceiveChat("Marriage.MarryTargetNeedGold".Lang(MarriageInvitation.Connection.Language, Character.CharacterName), MessageType.System);
                return;
            }

            if (MarriageInvitation.Gold < cost)
            {
                Connection.ReceiveChat("Marriage.MarryTargetNeedGold".Lang(Connection.Language, MarriageInvitation.Character.CharacterName), MessageType.System);
                MarriageInvitation.Connection.ReceiveChat("Marriage.MarryNeedGold".Lang(MarriageInvitation.Connection.Language), MessageType.System);
                return;
            }

            Character.Partner = MarriageInvitation.Character;

            Connection.ReceiveChat("Marriage.MarryComplete".Lang(Connection.Language, MarriageInvitation.Character.CharacterName), MessageType.System);
            MarriageInvitation.Connection.ReceiveChat("Marriage.MarryComplete".Lang(MarriageInvitation.Connection.Language, Character.CharacterName), MessageType.System);

            Gold -= cost;
            MarriageInvitation.Gold -= cost;

            GoldChanged();
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "结婚系统",
                Time = SEnvir.Now,
                Character = Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = cost,
                ExtraInfo = $"结婚对象{MarriageInvitation.Character}"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry);

            MarriageInvitation.GoldChanged();
            // 记录
            // 构造日志条目
            CurrencyLogEntry logEntry1 = new CurrencyLogEntry()
            {
                LogLevel = LogLevel.Info,
                Component = "结婚系统",
                Time = SEnvir.Now,
                Character = MarriageInvitation.Character,
                Currency = CurrencyType.Gold,
                Action = CurrencyAction.Deduct,
                Source = CurrencySource.ItemAdd,
                Amount = cost,
                ExtraInfo = $"结婚对象{Character}"
            };
            // 存入日志
            SEnvir.LogToViewAndCSV(logEntry1);

            AddAllObjects();

            Enqueue(GetMarriageInfo());
            MarriageInvitation.Enqueue(MarriageInvitation.GetMarriageInfo());

            //结婚事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerMarriage(EventTypes.PlayerMarry,
                    new PlayerMarriageEventArgs { IsDivorce = false, Partner = Character.Partner }));
        }
        public void MarriageLeave()
        {
            if (Character.Partner == null) return;

            CharacterInfo partner = Character.Partner;

            Character.Partner = null;

            MarriageRemoveRing();
            Connection.ReceiveChat("Marriage.MarryDivorce".Lang(Connection.Language, partner.CharacterName), MessageType.System);

            Enqueue(GetMarriageInfo());


            if (partner.Player != null)
            {
                partner.Player.MarriageRemoveRing();
                partner.Player.Connection.ReceiveChat("Marriage.MarryDivorce".Lang(partner.Player.Connection.Language, Character.CharacterName), MessageType.System);
                partner.Player.Enqueue(partner.Player.GetMarriageInfo());
            }
            else
                foreach (UserItem item in partner.Items)
                {
                    if (item.Slot != Globals.EquipmentOffSet + (int)EquipmentSlot.RingL) continue;

                    item.Flags &= ~UserItemFlags.Marriage;
                }

            //离婚事件
            //队列一个事件, 不要忘记添加listener
            SEnvir.EventManager.QueueEvent(
                new PlayerMarriage(EventTypes.PlayerDivorce,
                    new PlayerMarriageEventArgs { IsDivorce = true, Partner = partner }));
        }
        public void MarriageMakeRing(int index)
        {
            if (Character.Partner == null) return; //未婚

            if (Equipment[(int)EquipmentSlot.RingL] != null && (Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) == UserItemFlags.Marriage) return;

            if (index < 0 || index >= Inventory.Length) return;

            UserItem ring = Inventory[index];

            if (ring == null || ring.Info.ItemType != ItemType.Ring) return;

            ring.Flags |= UserItemFlags.Marriage;

            Inventory[index] = Equipment[(int)EquipmentSlot.RingL];

            if (Inventory[index] != null)
            {
                Inventory[index].Slot = index;
                //ToDo脱装备
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args;
                        var toItem = Inventory[index];
                        args = PythonOps.MakeTuple(new object[] { this, toItem, (int)EquipmentSlot.RingL });
                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnTakeOffItem", args);
                        //trig_player(this, "OnTakeOffItem", args);
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
            }

            Equipment[(int)EquipmentSlot.RingL] = ring;
            ring.Slot = Globals.EquipmentOffSet + (int)EquipmentSlot.RingL;

            Enqueue(new S.ItemMove { FromGrid = GridType.Inventory, FromSlot = index, ToGrid = GridType.Equipment, ToSlot = (int)EquipmentSlot.RingL, Success = true });

            //穿装备
            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, ring, (int)EquipmentSlot.RingL });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnPutOnItem", args);
                    //trig_player(this, "OnPutOnItem", args);
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

            Enqueue(new S.MarriageMakeRing());
            RefreshStats();
            Enqueue(new S.NPCClose());
        }
        public void MarriageTeleport()
        {
            if (Character.Partner == null) return; //未婚

            if (Equipment[(int)EquipmentSlot.RingL] == null || (Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) != UserItemFlags.Marriage) return;

            if (Dead)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportDead".Lang(Connection.Language), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportDead".Lang(con.Language), MessageType.System);
                return;
            }

            if (Stats[Stat.PKPoint] >= Config.RedPoint)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportPK".Lang(Connection.Language), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportPK".Lang(con.Language), MessageType.System);
                return;
            }

            if (SEnvir.Now < Character.MarriageTeleportTime)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportDelay".Lang(Connection.Language, (Character.MarriageTeleportTime - SEnvir.Now).Lang(Connection.Language, true)), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportDelay".Lang(con.Language, (Character.MarriageTeleportTime - SEnvir.Now).Lang(con.Language, true)), MessageType.System);
                return;
            }

            if (Character.Partner.Player == null)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportOffline".Lang(Connection.Language), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportOffline".Lang(con.Language), MessageType.System);
                return;
            }
            if (Character.Partner.Player.Dead && !Config.PartnerDeadTeleport)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportPartnerDead".Lang(Connection.Language), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportPartnerDead".Lang(con.Language), MessageType.System);
                return;
            }

            if (!Character.Partner.Player.CurrentMap.Info.CanMarriageRecall)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportMap".Lang(Connection.Language), MessageType.System);
                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportMap".Lang(con.Language), MessageType.System);
                return;
            }

            if (!CurrentMap.Info.AllowTT)
            {
                Connection.ReceiveChat("Marriage.MarryTeleportMapEscape".Lang(Connection.Language), MessageType.System);

                foreach (SConnection con in Connection.Observers)
                    con.ReceiveChat("Marriage.MarryTeleportMapEscape".Lang(con.Language), MessageType.System);
                return;
            }


            if (Teleport(Character.Partner.Player.CurrentMap, Character.Partner.Player.CurrentMap.GetRandomLocation(Character.Partner.Player.CurrentLocation, Config.MarriageTeleportLocation)))
                Character.MarriageTeleportTime = SEnvir.Now + Config.MarriageTeleportDelay;  //夫妻传送时间 120秒
        }

        public void MarriageRemoveRing()
        {
            if (Equipment[(int)EquipmentSlot.RingL] == null || (Equipment[(int)EquipmentSlot.RingL].Flags & UserItemFlags.Marriage) != UserItemFlags.Marriage) return;

            Equipment[(int)EquipmentSlot.RingL].Flags &= ~UserItemFlags.Marriage;
            Enqueue(new S.MarriageRemoveRing());
        }
        public S.MarriageInfo GetMarriageInfo()
        {
            return new S.MarriageInfo
            {
                Partner = new ClientPlayerInfo { Name = Character.Partner?.CharacterName, ObjectID = Character.Partner?.Player != null ? Character.Partner.Player.ObjectID : 0 }
            };
        }
        #endregion
    }
}
