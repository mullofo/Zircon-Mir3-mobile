using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    /// <summary>
    /// 观察者
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        /// <summary>
        /// 设置观察者
        /// </summary>
        /// <param name="con"></param>
        public void SetUpObserver(SConnection con)
        {
            con.Stage = GameStage.Observer;
            con.Observed = Connection;
            Connection.Observers.Add(con);

            con.Enqueue(new S.StartObserver
            {
                StartInformation = GetStartInformation(),
                ClientControl = SEnvir.ClientControl,
                Items = Character.Account.Items.Select(x => x.ToClientInfo()).ToList(),
            });
            //Send Items

            foreach (MapObject ob in VisibleObjects)
            {
                if (ob == this) continue;

                con.Enqueue(ob.GetInfoPacket(this));
            }

            List<ClientRefineInfo> refines = new List<ClientRefineInfo>();
            List<ClientRefineInfo> newRefines = new List<ClientRefineInfo>();
            foreach (RefineInfo info in Character.Refines)  //精炼部分
            {
                if (info.IsNewWeaponUpgrade)
                    newRefines.Add(info.ToClientInfo());
                else
                    refines.Add(info.ToClientInfo());
            }

            if (refines.Count > 0)
                con.Enqueue(new S.RefineList { List = refines });
            if (newRefines.Count > 0)
                con.Enqueue(new S.RefineList { List = newRefines, IsNewWeaponUpgrade = true });

            con.Enqueue(new S.StatsUpdate { Stats = Stats, HermitStats = Character.HermitStats, HermitPoints = Math.Max(0, Level - 39 - Character.SpentPoints) });

            con.Enqueue(new S.WeightUpdate { BagWeight = BagWeight, WearWeight = WearWeight, HandWeight = HandWeight });

            Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });

            if (TradePartner != null)   //交易部分
            {
                con.Enqueue(new S.TradeOpen { Name = TradePartner.Name });

                if (TradeGold > 0)
                    con.Enqueue(new S.TradeAddGold { Gold = TradeGold });

                foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradeItems)
                    con.Enqueue(new S.TradeAddItem { Cell = pair.Value, Success = true });


                if (TradePartner.TradeGold > 0)
                    con.Enqueue(new S.TradeGoldAdded { Gold = TradePartner.TradeGold });

                foreach (KeyValuePair<UserItem, CellLinkInfo> pair in TradePartner.TradeItems)
                {
                    S.TradeItemAdded packet = new S.TradeItemAdded
                    {
                        Item = pair.Key.ToClientInfo()
                    };
                    packet.Item.Count = pair.Value.Count;
                    con.Enqueue(packet);
                }
            }

            if (NPCPage != null)  //NPC部分
                con.Enqueue(new S.NPCResponse { ObjectID = NPC.ObjectID, Index = 0, NpcPage = NPCPage as ClientNPCPage });

            UpdateReviveTimers(con);

            if (Companion != null)  //宠物部分
                con.Enqueue(new S.CompanionWeightUpdate { BagWeight = Companion.BagWeight, MaxBagWeight = Companion.Stats[Stat.CompanionBagWeight], InventorySize = Companion.Stats[Stat.CompanionInventory] });

            con.Enqueue(GetMarriageInfo());

            foreach (MapObject ob in VisibleDataObjects)
            {
                // if (ob.Race == ObjectType.Player) continue;

                con.Enqueue(ob.GetDataPacket(this));
            }

            if (GroupMembers != null)   //组队部分
                foreach (PlayerObject ob in GroupMembers)
                    con.Enqueue(new S.GroupMember { ObjectID = ob.ObjectID, Name = ob.Name });

            con.ReceiveChat("System.WelcomeObserver".Lang(con.Language, Name), MessageType.Announcement);

            if (Character.Account.GuildMember != null)   //行会部分
                foreach (GuildWarInfo warInfo in SEnvir.GuildWarInfoList.Binding)
                {
                    if (warInfo.Guild1 == Character.Account.GuildMember.Guild)
                        con.Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild2.GuildName, Duration = warInfo.Duration });

                    if (warInfo.Guild2 == Character.Account.GuildMember.Guild)
                        con.Enqueue(new S.GuildWarStarted { GuildName = warInfo.Guild1.GuildName, Duration = warInfo.Duration });
                }

            foreach (CastleInfo castle in SEnvir.CastleInfoList.Binding)  //城堡信息
            {
                GuildInfo ownerGuild = SEnvir.GuildInfoList.Binding.FirstOrDefault(x => x.Castle == castle);

                con.Enqueue(new S.GuildCastleInfo { Index = castle.Index, Owner = ownerGuild?.GuildName ?? String.Empty });
            }

            foreach (ConquestWar conquest in SEnvir.ConquestWars)
            {
                Point flagLocation = Point.Empty;
                if (conquest is ConquestWarFlag warFlag)
                {
                    flagLocation = warFlag.Flag?.CurrentLocation ?? Point.Empty;
                }

                Enqueue(new S.GuildConquestStarted { Index = conquest.info.Index, FlagLocation = flagLocation });
            }

            con.Enqueue(new S.FortuneUpdate { Fortunes = Character.Account.Fortunes.Select(x => x.ToClientInfo()).ToList() });
        }

        /// <summary>
        /// 是否可观察开关
        /// </summary>
        /// <param name="allow"></param>
        public void ObservableSwitch(bool allow)
        {
            if (Character.Account.ItemBot || Character.Account.GoldBot) allow = true;

            if (allow == Character.Observable) return;

            if (!InSafeZone)
            {
                Connection.ReceiveChat("System.ObserverChangeFail".Lang(Connection.Language), MessageType.System);
                return;
            }

            Character.Observable = allow;
            Enqueue(new S.ObservableSwitch { Allow = Character.Observable, ObserverPacket = false });

            for (int i = Connection.Observers.Count - 1; i >= 0; i--)
            {
                if (Connection.Observers[i].Account != null && (Connection.Observers[i].Account.Observer || Connection.Observers[i].Account.TempAdmin)) continue;

                Connection.Observers[i].EndObservation();
            }

            //ApplyObserverBuff();
        }
    }
}
