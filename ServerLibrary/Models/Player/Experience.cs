using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject // 经验与升级
    {
        public void GainExperience(decimal amount, bool huntGold, int gainLevel = Int32.MaxValue, bool rateEffected = true, bool GiveWeapon = true, decimal bonusEx = 0)  //获得经验
        {
            if (rateEffected)  //如果有属性加成状态
            {
                amount *= 1M + Stats[Stat.ExperienceRate] / 100M;

                amount *= 1M + Stats[Stat.BaseExperienceRate] / 100M;

                if (Config.RebirthReduceExp > 0)
                {
                    for (int i = 0; i < Character.Rebirth; i++)   //转生获得经验
                        amount *= Config.RebirthReduceExp;
                }
            }

            if (amount == 0) return;  //经验为0就跳过

            decimal petamount = amount; //定义宝宝经验变量

            if (Level >= Config.MaxLevel && Config.MaxLevelLimit)
            {
                Experience = 0;
                Enqueue(new S.GainedExperience { Amount = 0, WeapEx = amount / 10, BonusEx = bonusEx, });   //发包数值到客户端
            }
            else
            {
                Experience += amount;    //经验加等数值
                Enqueue(new S.GainedExperience { Amount = amount, WeapEx = amount / 10, BonusEx = bonusEx, });   //发包数值到客户端
            }
            if (Character.Account.GuildMember != null)//存在行会
            {
                int perExp = Config.PersonalExpRatio;//经验兑换活跃度
                int maxCount = Config.ActivationCeiling; //活跃度上限
                if (DayExpAdd / perExp <= maxCount && (DayExpAdd + (int)amount) / perExp > DayExpAdd / perExp && DayActiveCount < maxCount)
                {
                    var t = (DayExpAdd + (int)amount) / perExp - DayExpAdd / perExp;
                    if (DayActiveCount + t > maxCount) t = maxCount - DayActiveCount; //保证只加10点每天
                    TotalActiveCount += t;
                    GuildTotalActiveCount += t;
                    GuildTotalDailyActiveCount += t;
                    DayActiveCount += t;
                    DayExpAdd += (long)amount;

                    GuildMemberInfo info = Character.Account.GuildMember;
                    foreach (GuildMemberInfo member in info.Guild.Members)
                    {
                        if (member.Account.Connection?.Player == null) continue;

                        member.Account.Connection.Enqueue(new S.GuildMemberContribution
                        {
                            Index = info.Index,
                            Contribution = 0,
                            ActiveCount = t,
                            IsVoluntary = true,
                            ObserverPacket = false
                        });
                    }
                    Enqueue(new S.GuildActiveCountChange
                    {
                        DailyActiveCount = DayActiveCount,
                        TotalActiveCount = TotalActiveCount
                    });

                    S.GuildUpdate update = Character.Account.GuildMember.Guild.GetUpdatePacket();

                    foreach (GuildMemberInfo member in Character.Account.GuildMember.Guild.Members)
                        member.Account.Connection?.Player?.Enqueue(update);
                }
                else
                    DayExpAdd += (long)amount;

            }


            for (int i = 0; i < Pets.Count; i++)   //宝宝经验获取
            {
                MonsterObject monster = Pets[i];
                //如果宝宝是在当前地图 在角色的范围内 且怪物死亡
                if (monster.CurrentMap == CurrentMap && Functions.InRange(monster.CurrentLocation, CurrentLocation, Globals.MagicRange) && !monster.Dead)
                    monster.PetExp(petamount);  //宝宝增加经验数值
            }

            UserItem weapon = Equipment[(int)EquipmentSlot.Weapon];  //武器获得精炼值
            //武器不为空  不是挖矿工具 可以精炼的 不是不可精炼的 不是无法获得经验的
            if (weapon != null && weapon.Info.Effect != ItemEffect.PickAxe && (weapon.Flags & UserItemFlags.Refinable) != UserItemFlags.Refinable && (weapon.Flags & UserItemFlags.NonRefinable) != UserItemFlags.NonRefinable && weapon.Level < Globals.GameWeaponEXPInfoList.Count && GiveWeapon)
            {
                if (weapon.Info.Index == 80360 || weapon.Info.Index == 80361 || weapon.Info.Index == 80362)
                {
                    if (weapon.Level < 13)
                    {
                        weapon.Experience += amount / 10;  //武器经验加 数值的十分之一

                        if (weapon.Experience >= Globals.GameWeaponEXPInfoList[weapon.Level].Exp)
                        {
                            weapon.Experience = 0;
                            weapon.Level++;

                            if (weapon.Level < 13)
                                weapon.Flags |= UserItemFlags.Refinable;
                        }
                    }
                }
                else
                {
                    weapon.Experience += amount / 10;  //武器经验加 数值的十分之一

                    if (weapon.Experience >= Globals.GameWeaponEXPInfoList[weapon.Level].Exp)
                    {
                        weapon.Experience = 0;
                        weapon.Level++;

                        if (weapon.Level < Globals.GameWeaponEXPInfoList.Count)
                            weapon.Flags |= UserItemFlags.Refinable;
                    }
                }
            }

            if (huntGold)   //获得赏金值
            {
                BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.HuntGold);

                if (buff?.Stats[Stat.AvailableHuntGold] > 0)
                {
                    buff.Stats[Stat.AvailableHuntGold]--;
                    Character.Account.HuntGold++;
                    Enqueue(new S.HuntGoldChanged { HuntGold = Character.Account.HuntGold });
                    Enqueue(new S.BuffChanged { Index = buff.Index, Stats = buff.Stats });
                }
            }

            while (Experience >= MaxExperience && Level < Config.MaxLevel)
            {
                Experience -= MaxExperience;
                Level++;
                LevelUp();
            }
        }
        public void LevelUp()  //升级
        {
            SEnvir.RankingSort(Character);

            RefreshStats();

            SetHP(Stats[Stat.Health]);
            SetMP(Stats[Stat.Mana]);

            Enqueue(new S.LevelChanged { Level = Level, Experience = Experience });
            Broadcast(new S.ObjectLeveled { ObjectID = ObjectID });

            SEnvir.RankingSort(Character);

            if (Character.Account.Characters.Max(x => x.Level) <= Level)
                BuffRemove(BuffType.Veteran);

            ApplyGuildBuff();

            //Item Links
            try
            {
                dynamic trig_play;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_play))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, });
                    IronPython.Runtime.List rewardsList = SEnvir.ExecutePyWithTimer(trig_play, this, "OnLevelUp", args);
                    //IronPython.Runtime.List rewardsList= trig_play(this, "OnLevelUp", args);

                    if (rewardsList != null && rewardsList.Count > 0)
                    {
                        // 邮件发送奖励
                        PYMailSend("升级奖励".Lang(Connection.Language), "系统".Lang(Connection.Language), "升级奖励已发放请查收".Lang(Connection.Language), rewardsList);
                    }
                }

            }
            catch (SyntaxErrorException e)
            {
                string msg = "Player事件（同步错误） : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {

                string msg = "Player事件（系统退出） : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {

                string msg = "Player事件（加载插件时错误）: \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }
        }
    }
}
