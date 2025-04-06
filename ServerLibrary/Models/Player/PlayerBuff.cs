using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    public partial class PlayerObject : MapObject //BUFF
    {
        #region Buffs
        /// <summary>
        /// 应用地图BUFF
        /// </summary>
        public void ApplyMapBuff()
        {
            BuffRemove(BuffType.MapEffect);

            if (CurrentMap == null) return;

            Stats stats = new Stats();

            stats[Stat.MonsterHealth] = CurrentMap.Info.MonsterHealth;
            stats[Stat.MonsterDamage] = CurrentMap.Info.MonsterDamage;
            stats[Stat.MonsterExperience] = CurrentMap.Info.ExperienceRate;
            stats[Stat.MonsterDrop] = CurrentMap.Info.DropRate;
            stats[Stat.MonsterGold] = CurrentMap.Info.GoldRate;

            stats[Stat.MaxMonsterHealth] = CurrentMap.Info.MaxMonsterHealth;
            stats[Stat.MaxMonsterDamage] = CurrentMap.Info.MaxMonsterDamage;
            stats[Stat.MaxMonsterExperience] = CurrentMap.Info.MaxExperienceRate;
            stats[Stat.MaxMonsterDrop] = CurrentMap.Info.MaxDropRate;
            stats[Stat.MaxMonsterGold] = CurrentMap.Info.MaxGoldRate;

            stats[Stat.BHealth] = CurrentMap.Info.HealthRate;   //地图按秒+ -角色血值  黑炎之力BUFF显示

            if (stats.Count == 0) return;

            BuffAdd(BuffType.MapEffect, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 应用服务器设置BUFF
        /// </summary>
        public void ApplyServerBuff()
        {
            BuffRemove(BuffType.Server);

            Stats stats = new Stats();

            stats[Stat.BaseExperienceRate] += Config.ExperienceRate;
            stats[Stat.BaseDropRate] += Config.DropRate;
            stats[Stat.BaseGoldRate] += Config.GoldRate;
            stats[Stat.SkillRate] = Config.SkillRate;
            stats[Stat.CompanionRate] = Config.CompanionRate;

            if (stats.Count == 0) return;
            BuffAdd(BuffType.Server, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 应用自定义活动BUFF
        /// </summary>
        public void ApplyEventBuff()
        {
            //如果玩家拥有已经被移除的活动buff 则删除
            List<int> existingEventBuffs = Buffs.Where(x => x.Type == BuffType.EventBuff).Select(y => y.FromCustomBuff).ToList();
            foreach (int activeBuff in existingEventBuffs)
            {
                if (!Globals.ActiveEventCustomBuffs.Keys.Contains(activeBuff))
                {
                    CustomBuffRemove(activeBuff);
                }
            }

            foreach (KeyValuePair<int, DateTime> kvp in Globals.ActiveEventCustomBuffs)
            {
                CustomBuffInfo buff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == kvp.Key);
                if (buff == null)
                {
                    SEnvir.Log($"找不到活动buff: {kvp.Key}");
                    continue;
                }

                // 计算剩余buff时间
                TimeSpan remainingTime = buff.Duration > TimeSpan.Zero ? buff.Duration - (SEnvir.Now - kvp.Value) : TimeSpan.MaxValue;
                if (remainingTime > TimeSpan.Zero)
                {
                    CustomBuffRemove(kvp.Key);
                    BuffInfo addedBuff = CustomBuffAdd(kvp.Key);
                    addedBuff.RemainingTime = remainingTime;
                    // 告知客户端
                    Enqueue(new S.BuffChanged { Index = addedBuff.Index, Stats = addedBuff.Stats, RemainingTime = remainingTime });
                }
            }
        }
        /// <summary>
        /// 增加观察者BUFF
        /// </summary>
        public void ApplyObserverBuff()
        {
            if (!Config.AllowObservation) return;

            BuffRemove(BuffType.Observable);

            if (!Character.Observable) return;
            if (Character.Account.ItemBot || Character.Account.GoldBot) return;

            Stats stats = new Stats();

            stats[Stat.ExperienceRate] += Config.StatExperienceRate;
            stats[Stat.DropRate] += Config.StatDropRate;
            stats[Stat.GoldRate] += Config.StatGoldRate;

            BuffAdd(BuffType.Observable, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 增加沙巴克BUFF
        /// </summary>
        public void ApplyCastleBuff()
        {
            BuffRemove(BuffType.Castle);

            if (Character.Account.GuildMember?.Guild.Castle == null) return;

            Stats stats = new Stats();

            stats[Stat.ExperienceRate] += Config.CastleExperienceRate;
            stats[Stat.DropRate] += Config.CastleDropRate;
            stats[Stat.GoldRate] += Config.CastleGoldRate;

            BuffAdd(BuffType.Castle, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 增加行会BUFF
        /// </summary>
        public void ApplyGuildBuff()
        {
            BuffRemove(BuffType.Guild);

            if (Character.Account.GuildMember == null) return;

            Stats stats = new Stats();

            if (Character.Account.GuildMember.Guild.StarterGuild)  //新手公会判断
            {
                if (Level < Config.StarterGuildLevelRate && Character.Rebirth < Config.StarterGuildRebirth)   //级别低于50级获得的BUFF加成
                {
                    stats[Stat.ExperienceRate] += Config.StarterGuildExperienceRate;   //经验+
                    stats[Stat.DropRate] += Config.StarterGuildDropRate;         //爆率+
                    stats[Stat.GoldRate] += Config.StarterGuildGoldRate;         //金币+
                }
                else                                    //超过50级惩罚
                {
                    stats[Stat.ExperienceRate] -= Config.StarterGuildExperienceRate;   //经验- 
                    stats[Stat.DropRate] -= Config.StarterGuildDropRate;         //爆率-
                    stats[Stat.GoldRate] -= Config.StarterGuildGoldRate;         //金币-
                }
            }
            //角色帐户公会成员 公会成员计数
            else
            {
                if (Character.Account.GuildMember.Guild.Members.Count <= Config.GuildLevelRate)    //普通行会低于15人
                {
                    stats[Stat.ExperienceRate] += Config.GuildExperienceRate;
                    stats[Stat.DropRate] += Config.GuildDropRate;
                    stats[Stat.GoldRate] += Config.GuildGoldRate;
                }
                else if (Character.Account.GuildMember.Guild.Members.Count <= Config.GuildLevel1Rate)   //普通行会低于30人
                {
                    stats[Stat.ExperienceRate] += Config.GuildExperience1Rate;
                    stats[Stat.DropRate] += Config.GuildDrop1Rate;
                    stats[Stat.GoldRate] += Config.GuildGold1Rate;
                }
                else if (Character.Account.GuildMember.Guild.Members.Count <= Config.GuildLevel2Rate)   //普通行会低于45人
                {
                    stats[Stat.ExperienceRate] += Config.GuildExperience2Rate;
                    stats[Stat.DropRate] += Config.GuildDrop2Rate;
                    stats[Stat.GoldRate] += Config.GuildGold2Rate;
                }
                else                                                               //普通行会超过45人以后
                {
                    stats[Stat.ExperienceRate] += Config.GuildExperience3Rate;
                    stats[Stat.DropRate] += Config.GuildDrop3Rate;
                    stats[Stat.GoldRate] += Config.GuildGold3Rate;
                }
            }

            if (Character.Account.GuildMember.Guild.GuildLevel > 0)   //行会等级如果大于0级，对应增加经验%加成
            {
                //stats[Stat.ExperienceRate] = (Character.Account.GuildMember.Guild.GuildLevel + 1) / 2; //跳级加倍数 1 2级1% 3 4级2% 类推
                stats[Stat.ExperienceRate] = Character.Account.GuildMember.Guild.GuildLevel;
            }

            if (stats.Count == 0) return;

            //if (!Character.Account.GuildMember.Guild.StarterGuild && GroupMembers != null)  //组队不给行会BUFF
            //{
            //    foreach (PlayerObject member in GroupMembers)
            //    {
            //        if (member.Character.Account.GuildMember != null && member.Character.Account.GuildMember.Guild.StarterGuild) continue;

            //        if (member.Character.Account.GuildMember?.Guild != Character.Account.GuildMember.Guild) return;
            //    }
            //}

            BuffAdd(BuffType.Guild, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 增加组队BUFF
        /// </summary>
        public void ApplyGroupBuff()
        {
            BuffRemove(BuffType.Group);

            if (GroupMembers == null) return;

            int dWarrior = 0, dWizard = 0, dTaoist = 0;//, dAssassin = 0;

            foreach (PlayerObject ob in GroupMembers)
            {
                switch (ob.Class)
                {
                    case MirClass.Warrior:
                        dWarrior++;
                        break;
                    case MirClass.Wizard:
                        dWizard++;
                        break;
                    case MirClass.Taoist:
                        dTaoist++;
                        break;
                        //case MirClass.Assassin:
                        //    dAssassin++;
                        //    break;
                }
            }

            decimal dRate = 1M;
            //if (Config.AllowAssassin)
            //{
            //    if ((dWarrior == 0) || (dWizard == 0) || (dTaoist == 0) || (dAssassin == 0)) return;

            //    switch (Class)
            //    {
            //        case MirClass.Warrior:
            //            dRate *= Config.GroupAddWarRate;  //战士加成倍率
            //            break;
            //        case MirClass.Wizard:
            //            dRate *= Config.GroupAddWizRate;  //法师加成倍率
            //            break;
            //        case MirClass.Taoist:
            //            dRate *= Config.GroupAddTaoRate;  //道士加成倍率
            //            break;
            //        case MirClass.Assassin:
            //            dRate *= Config.GroupAddAssRate;  //刺客加成倍率
            //            break;
            //    }
            //}
            //else
            //{
            if ((dWarrior == 0) || (dWizard == 0) || (dTaoist == 0)) return;

            switch (Class)
            {
                case MirClass.Warrior:
                    dRate *= Config.GroupAddWarRate;  //战士加成倍率
                    break;
                case MirClass.Wizard:
                    dRate *= Config.GroupAddWizRate;  //法师加成倍率
                    break;
                case MirClass.Taoist:
                    dRate *= Config.GroupAddTaoRate;  //道士加成倍率
                    break;
            }
            //}

            Stats stats = new Stats();

            stats[Stat.ExperienceRate] += (int)(Config.GroupAddBaseExp * dRate);   //附加组队经验加成
            stats[Stat.DropRate] += (int)(Config.GroupAddBaseDrop * dRate);        //附加组队爆率加成
            stats[Stat.GoldRate] += (int)(Config.GroupAddBaseGold * dRate);        //附加组队经验加成

            stats[Stat.Health] += Stats[Stat.BaseHealth] * (int)(Config.GroupAddBaseHp * dRate) / 100;  //附加组队血量加成
            stats[Stat.Mana] += Stats[Stat.Mana] * (int)(Config.GroupAddBaseHp * dRate) / 100;          //附加组队魔力加成

            BuffAdd(BuffType.Group, TimeSpan.MaxValue, stats, false, false, TimeSpan.Zero);
        }
        /// <summary>
        /// 道具BUFF添加
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool ItemBuffAdd(ItemInfo info)
        {
            switch (info.Effect)
            {
                case ItemEffect.DestructionElixir:
                case ItemEffect.HasteElixir:
                case ItemEffect.LifeElixir:
                case ItemEffect.ManaElixir:
                case ItemEffect.NatureElixir:
                case ItemEffect.SpiritElixir:

                    for (int i = Buffs.Count - 1; i >= 0; i--)
                    {
                        BuffInfo buff = Buffs[i];
                        if (buff.Type != BuffType.ItemBuff || info.Index == buff.ItemIndex) continue; //相同的项目不删除，而是扩展 

                        ItemInfo buffItemInfo = SEnvir.ItemInfoList.Binding.First(x => x.Index == buff.ItemIndex);

                        if (buffItemInfo.Effect == info.Effect)
                            BuffRemove(buff);
                    }
                    break;
            }

            BuffInfo currentBuff = Buffs.FirstOrDefault(x => x.Type == BuffType.ItemBuff && x.ItemIndex == info.Index);

            if (currentBuff != null) //扩展buff
            {
                if (info.Stats[Stat.Duration] >= 0)
                {
                    TimeSpan duration = TimeSpan.FromSeconds(info.Stats[Stat.Duration]);

                    long ticks = currentBuff.RemainingTime.Ticks - long.MaxValue + duration.Ticks; //Check for Overflow (Probably never going to happen) 403x MaxValue durations refreshes.

                    if (ticks >= 0)
                        currentBuff.RemainingTime = TimeSpan.MaxValue;
                    else
                        currentBuff.RemainingTime += duration;
                }
                else
                    currentBuff.RemainingTime = TimeSpan.MaxValue;

                Enqueue(new S.BuffTime { Index = currentBuff.Index, Time = currentBuff.RemainingTime });
                return true;
            }

            currentBuff = SEnvir.BuffInfoList.CreateNewObject();

            currentBuff.Type = BuffType.ItemBuff;
            currentBuff.ItemIndex = info.Index;
            currentBuff.RemainingTime = info.Stats[Stat.Duration] > 0 ? TimeSpan.FromSeconds(info.Stats[Stat.Duration]) : TimeSpan.MaxValue;

            if (info.RequiredAmount == 0 && info.RequiredClass == RequiredClass.All)
                currentBuff.Account = Character.Account;
            else
                currentBuff.Character = Character;

            currentBuff.Pause = InSafeZone;  //当前道具BUFF 暂停=安全区

            Buffs.Add(currentBuff);
            Enqueue(new S.BuffAdd { Buff = currentBuff.ToClientInfo() });

            RefreshStats();
            AddAllObjects();

            return true;
        }

        /// <summary>
        /// 重写BUFF信息 BUFF添加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="remainingTicks"></param>
        /// <param name="stats"></param>
        /// <param name="visible"></param>
        /// <param name="pause"></param>
        /// <param name="tickRate"></param>
        /// <param name="fromCustomBuff"></param>
        /// <returns></returns>
        public override BuffInfo BuffAdd(BuffType type, TimeSpan remainingTicks, Stats stats, bool visible, bool pause, TimeSpan tickRate, int fromCustomBuff = 0)
        {
            BuffInfo info = base.BuffAdd(type, remainingTicks, stats, visible, pause, tickRate, fromCustomBuff);

            info.Character = Character;

            switch (type)
            {
                case BuffType.ItemBuff:
                    info.Pause = InSafeZone;
                    break;
            }

            Enqueue(new S.BuffAdd { Buff = info.ToClientInfo() });

            switch (type)
            {
                case BuffType.StrengthOfFaith:
                    for (int i = Pets.Count - 1; i >= 0; i--)
                    {
                        if (Pets[i] == null) continue;
                        MonsterObject pet = Pets[i];
                        pet.RefreshStats();
                        pet.Magics.Add(Magics[MagicType.StrengthOfFaith]);
                    }
                    break;
                case BuffType.DragonRepulse: //狂涛涌泉
                case BuffType.Companion:  //宠物
                case BuffType.Server:  //服务器
                case BuffType.EventBuff:
                case BuffType.MapEffect: //地图BUFF
                case BuffType.Guild: //行会
                case BuffType.Group: //组队
                case BuffType.Ranking: //排行榜
                case BuffType.Developer: //管理
                case BuffType.Castle:  //城堡
                case BuffType.ElementalHurricane:  //离魂邪风
                case BuffType.SuperiorMagicShield: //护身法盾
                    info.IsTemporary = true;
                    break;
            }

            return info;
        }
        /// <summary>
        /// 覆盖 空BUFF移除 
        /// </summary>
        /// <param name="info">BUFF信息</param>
        public override void BuffRemove(BuffInfo info)
        {
            try
            {
                int oldHealth = Stats[Stat.Health];

                base.BuffRemove(info);

                Enqueue(new S.BuffRemove { Index = info.Index });


                switch (info.Type)
                {
                    case BuffType.StrengthOfFaith:
                        for (int i = Pets.Count - 1; i >= 0; i--)
                        {
                            if (Pets[i] == null) continue;
                            MonsterObject pet = Pets[i];
                            pet.Magics.Remove(Magics[MagicType.StrengthOfFaith]);
                            pet.RefreshStats();
                        }
                        break;
                    case BuffType.Renounce:
                        if (Dead) return;
                        ChangeHP(info.Stats[Stat.RenounceHPLost]);
                        break;
                    case BuffType.ItemBuff:
                        RefreshStats();
                        RemoveAllObjects();
                        break;
                }
            }
            catch { }
        }
        /// <summary>
        /// 暂停BUFF，一般为安全区缓冲
        /// </summary>
        public void PauseBuffs()
        {
            if (CurrentCell == null) return;  //BUFF格子为空 跳过

            bool change = false;  //变量  改变判断

            bool pause = Config.SafeZoneBuffPause && InSafeZone;   //变量 安全区判断

            foreach (MapObject ob in CurrentCell.Objects) //遍历（当前地图单元格映射对象ob）
            {
                if (ob.Race != ObjectType.Spell) continue;   //如果对象不等于施法对象 继续

                SpellObject spell = (SpellObject)ob;

                if (spell.Effect != SpellEffect.Rubble) continue;

                pause = true;
                break;
            }

            foreach (BuffInfo buff in Buffs)  //变量BUFF信息
            {
                bool buffPause = pause;  //变量 暂停

                switch (buff.Type)
                {
                    case BuffType.ItemBuff:
                        buffPause = buff.RemainingTime != TimeSpan.MaxValue && pause;
                        break;
                    case BuffType.HuntGold:
                        break;
                    case BuffType.CustomBuff:
                        //自定义buff
                        CustomBuffInfo customBuff =
                            Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buff.FromCustomBuff);
                        if (customBuff != null)
                        {
                            buffPause = buffPause && customBuff.PauseInSafeZone;
                        }
                        break;
                    default:
                        continue;
                }

                if (buff.Pause == buffPause) continue;

                buff.Pause = buffPause;
                change = true;

                Enqueue(new S.BuffPaused { Index = buff.Index, Paused = buffPause });
            }

            if (change)
                RefreshStats();
        }
        /// <summary>
        /// 暂停BUFF
        /// </summary>
        /// <param name="buffType">BUFF类型</param>
        public void PauseBuff(BuffType buffType)
        {
            bool changed = false;
            foreach (BuffInfo buff in Buffs)
            {
                if (buff.Type != buffType) continue;
                changed = true;
                buff.Pause = true;
                Enqueue(new S.BuffPaused { Index = buff.Index, Paused = true });
            }
            if (changed)
                RefreshStats();
        }
        /// <summary>
        /// 移除BUFF
        /// </summary>
        /// <param name="buffType">BUFF类型</param>
        public void ResumeBuff(BuffType buffType)
        {
            bool changed = false;
            foreach (BuffInfo buff in Buffs)
            {
                if (buff.Type != buffType) continue;
                changed = true;
                buff.Pause = false;
                Enqueue(new S.BuffPaused { Index = buff.Index, Paused = false });
            }
            if (changed)
                RefreshStats();
        }

        /// <summary>
        /// 计算离线时的BUFF
        /// </summary>
        public void ProcessOfflineBuff()
        {
            if (Character.Account?.LastCharacter == null) return;
            if (Character.Index != Character.Account.LastCharacter.Index) return;
            if (Character.LastLogin.Year <= 1970) return;

            TimeSpan ticks = BuffTime - Character.LastLogin;
            if (ticks <= TimeSpan.Zero)
            {
                return;
            }
            List<BuffInfo> expiredBuffs = new List<BuffInfo>();

            foreach (BuffInfo buff in Buffs)
            {
                CustomBuffInfo customBuff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buff.FromCustomBuff);

                if (buff.Type == BuffType.PKPoint || buff.Type == BuffType.ItemBuff) continue;   //如果是PK的BUFF类型直接跳过

                if (customBuff != null)
                {
                    if (buff.Type == BuffType.CustomBuff && !customBuff.OfflineTicking) continue;
                }

                buff.TickTime -= ticks;

                if (buff.RemainingTime != TimeSpan.MaxValue)
                {
                    buff.RemainingTime -= ticks;

                    if (buff.RemainingTime <= TimeSpan.Zero)
                        expiredBuffs.Add(buff);
                }
            }

            foreach (BuffInfo buff in expiredBuffs)
                BuffRemove(buff);
        }
        #endregion
    }
}
