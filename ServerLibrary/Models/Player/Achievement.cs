using Library;
using Library.SystemModels;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager;
using Server.Models.EventManager.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //成就
    {
        /*
         * 成就中的所有要求都要满足 才可以更新进度
         * 例如 用木剑造成1000伤害 必须要同时佩戴木剑并且造成了伤害
         * 每添加1个类别 就要更新这个函数(仅限状态类)
         */
        public bool AchievementRequirementSatisfied(UserAchievement achievement) //是否可以更新成就进度
        {
            foreach (var userRequirement in achievement.AchievementRequirements)
            {
                AchievementRequirement requirement = userRequirement.Requirement;
                if (requirement == null) return false;

                bool reverse = requirement.Reverse;
                switch (requirement.RequirementType) //仅限状态类
                {
                    case AchievementRequirementType.InMap: //身处于某地图
                        if (requirement.MapParameter != null)
                        {
                            if (!reverse && CurrentMap.Info.Index != requirement.MapParameter.Index)
                            {
                                return false;
                            }
                            if (reverse && CurrentMap.Info.Index == requirement.MapParameter.Index)
                            {
                                return false;
                            }
                        }
                        break;
                    case AchievementRequirementType.WearingItem: //佩戴着某装备
                        if (requirement.ItemParameter != null && !WearingItem(requirement.ItemParameter))
                        {
                            return false;
                        }
                        break;
                    case AchievementRequirementType.CarryingItem: //携带着某装备
                        if (requirement.ItemParameter != null)
                        {
                            if (!reverse && GetItemCount(requirement.ItemParameter) < 1)
                            {
                                return false;
                            }
                            if (reverse && GetItemCount(requirement.ItemParameter) >= 1)
                            {
                                return false;
                            }
                        }
                        break;
                    case AchievementRequirementType.LevelLessThan: //小于某级别
                        if (!reverse && Level > requirement.RequiredAmount)
                        {
                            return false;
                        }
                        break;
                    case AchievementRequirementType.LevelGreaterOrEqualThan: //大于等于某级别
                        if (Level <= requirement.RequiredAmount)
                        {
                            return false;
                        }
                        break;
                    case AchievementRequirementType.UseMagic: //TODO 需要测试
                        if (CurrentMagic != requirement?.MagicParameter?.Magic)
                        {
                            return false;
                        }
                        break;
                }
            }

            return true;
        }

        public List<UserAchievement> GetUserAchievements(AchievementRequirementType type)
        {
            return Character.Achievements.Where(
                x => x.AchievementRequirements.Any(
                         y => y.Requirement.RequirementType == type) && AchievementRequirementSatisfied(x)).ToList();
        }

        public List<UserAchievementRequirement> GetAchievementRequirements(AchievementRequirementType type)
        {
            return Character.Achievements.SelectMany(
                x => x.AchievementRequirements).Where(
                y => y.Requirement.RequirementType == type && AchievementRequirementSatisfied(y.Achievement)).ToList();
        }

        public void SendAchievementUpdates()
        {
            if (SEnvir.ChangedAchievementIndices.Count > 0)
            {
                List<UserAchievement> temp = Character.Achievements.Where(x => SEnvir.ChangedAchievementIndices.Contains(x.Index)).ToList();

                foreach (UserAchievement userAchievement in temp)
                {
                    if (!userAchievement.Completed && userAchievement.IsComplete)
                    {
                        userAchievement.Completed = true;
                        SendRewards(userAchievement);
                        //todo 发送动画指示？
                    }
                }

                List<ClientUserAchievement> list = temp.Select(x => x.ToClientInfo()).ToList();

                Enqueue(new S.AchievementProgressChanged
                {
                    Achievements = list
                });

                SEnvir.ChangedAchievementIndices.Clear();
            }
        }

        public void SendRewards(UserAchievement achievement)
        {
            if (!achievement.Completed) return;
            List<AchievementReward> rewards = null;
            switch (Class)
            {
                case MirClass.Warrior:
                    rewards = achievement.AchievementName.AchievementRewards.Where(x =>
                        x.Class.HasFlag(RequiredClass.Warrior)).ToList();
                    break;
                case MirClass.Wizard:
                    rewards = achievement.AchievementName.AchievementRewards.Where(x =>
                        x.Class.HasFlag(RequiredClass.Wizard)).ToList();
                    break;
                case MirClass.Taoist:
                    rewards = achievement.AchievementName.AchievementRewards.Where(x =>
                        x.Class.HasFlag(RequiredClass.Taoist)).ToList();
                    break;
                case MirClass.Assassin:
                    rewards = achievement.AchievementName.AchievementRewards.Where(x =>
                        x.Class.HasFlag(RequiredClass.Assassin)).ToList();
                    break;
            }

            if (rewards == null || rewards.Count == 0) return;

            foreach (AchievementReward reward in rewards)
            {
                UserItemFlags flag = reward.Bound ? UserItemFlags.Bound : UserItemFlags.None;
                TimeSpan timeLimit = reward.Duration > 0 ? TimeSpan.FromSeconds(reward.Duration) : TimeSpan.Zero;

                flag = reward.Duration > 0 ? flag | UserItemFlags.Expirable : flag;

                ItemCheck check = new ItemCheck(reward.Item, reward.Amount, flag, timeLimit);

                //todo 极品几率
                UserItem item = SEnvir.CreateDropItem(check, 2);
                //记录物品来源
                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.None, "成就系统".Lang(Connection.Language), Character?.CharacterName);

                //给不了物品 改用信件发送
                if (!CanGainItems(false, check))
                {
                    //信件发送 
                    //todo 只发1个邮件就够了 最多7个物品
                    MailInfo mail = SEnvir.MailInfoList.CreateNewObject();

                    mail.Account = Character.Account;
                    mail.Subject = "达成成就".Lang(Connection.Language);
                    mail.Sender = "系统".Lang(Connection.Language);
                    mail.Message = $"PlayerObject.Achievement".Lang(Connection.Language, (item.Info.ItemName, item.Count == 1 ? "" : "x" + item.Count));

                    item.Mail = mail;
                    item.Slot = 0;
                    mail.HasItem = true;

                    Enqueue(new S.MailNew
                    {
                        Mail = mail.ToClientInfo(),
                        ObserverPacket = false,
                    });
                }
                else
                {
                    GainItem(item);
                }
            }

        }

        #region 事件handlers

        #region 人物移动事件
        public void OnPlayerMove(IEvent evt)
        {
            if (!(evt.EventData is PlayerMoveEventArgs args))
            {
                SEnvir.Log("PlayerMoveEventArgs is null!");
                return;
            }

            if (Character == null) return;

            switch (args.numSteps)
            {
                case 1:
                    GetAchievementRequirements(AchievementRequirementType.WalkSteps).ForEach(x => x.CurrentValue += 1);
                    break;
                case 2:
                    GetAchievementRequirements(AchievementRequirementType.RunSteps).ForEach(x => x.CurrentValue += 2);
                    break;
                case 3:
                    GetAchievementRequirements(AchievementRequirementType.RideSteps).ForEach(x => x.CurrentValue += 3);
                    break;
            }
            GetAchievementRequirements(AchievementRequirementType.TotalSteps).ForEach(x => x.CurrentValue += args.numSteps);
        }
        #endregion


        #region 人物挖肉事件
        public void OnPlayerHarvest(IEvent evt)
        {
            if (!(evt.EventData is PlayerHarvestEventArgs args))
            {
                SEnvir.Log("PlayerHarvestEventArgs is null!");
                return;
            }
            if (Character == null) return;

            if (args.success)
            {
                GetAchievementRequirements(AchievementRequirementType.HarvestSuccess).ForEach(x => x.CurrentValue += 1);
            }
            GetAchievementRequirements(AchievementRequirementType.TotalHarvest).ForEach(x => x.CurrentValue += 1);
        }
        #endregion

        #region 人物杀怪事件

        public void OnPlayerKillMonster(IEvent evt)
        {
            if (!(evt.EventData is PlayerKillMonsterEventArgs args))
            {
                SEnvir.Log("PlayerKillMonsterEventArgs is null!");
                return;
            }

            if (Character == null) return;

            if (args.KilledMonster != null)
            {
                foreach (var req in GetAchievementRequirements(AchievementRequirementType.KillMonster))
                {
                    if (req.Requirement.MonsterParameter != null)
                    {
                        if (args.KilledMonster.Index == req.Requirement.MonsterParameter.Index)
                        {
                            req.CurrentValue += 1;
                        }
                    }
                }
            }
        }

        #endregion

        #region 人物平A

        public void OnPlayerAttack(IEvent evt)
        {
            if (!(evt.EventData is PlayerAttackEventArgs args))
            {
                SEnvir.Log("PlayerAttackEventArgs is null!");
                return;
            }
            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.BasicAttack).ForEach(x => x.CurrentValue += 1);

            switch (args.element)
            {
                case Element.Fire:
                    GetAchievementRequirements(AchievementRequirementType.FireDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Ice:
                    GetAchievementRequirements(AchievementRequirementType.IceDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Lightning:
                    GetAchievementRequirements(AchievementRequirementType.LightingDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Wind:
                    GetAchievementRequirements(AchievementRequirementType.WindDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Holy:
                    GetAchievementRequirements(AchievementRequirementType.HolyDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Dark:
                    GetAchievementRequirements(AchievementRequirementType.DarkDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Phantom:
                    GetAchievementRequirements(AchievementRequirementType.PhantomDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
            }

            if (args.target != null)
            {
                switch (args.target.Race)
                {
                    case ObjectType.Player:
                        GetAchievementRequirements(AchievementRequirementType.TotalPlayerDamage).ForEach(x => x.CurrentValue += args.damage);
                        break;
                    case ObjectType.Monster:
                        GetAchievementRequirements(AchievementRequirementType.TotalMonsterDamage).ForEach(x => x.CurrentValue += args.damage);
                        break;
                }
            }
            GetAchievementRequirements(AchievementRequirementType.TotalDamage).ForEach(x => x.CurrentValue += args.damage);
        }

        #endregion

        #region 人物技能

        public void OnPlayerMagic(IEvent evt)
        {
            if (!(evt.EventData is PlayerMagicEventArgs args))
            {
                SEnvir.Log("PlayerMagicEventArgs is null!");
                return;
            }
            if (Character == null) return;

            // public MagicType magic { get; set; } todo
            GetAchievementRequirements(AchievementRequirementType.MagicAttack).ForEach(x => x.CurrentValue += 1);
            switch (args.element)
            {
                case Element.Fire:
                    GetAchievementRequirements(AchievementRequirementType.FireDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Ice:
                    GetAchievementRequirements(AchievementRequirementType.IceDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Lightning:
                    GetAchievementRequirements(AchievementRequirementType.LightingDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Wind:
                    GetAchievementRequirements(AchievementRequirementType.WindDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Holy:
                    GetAchievementRequirements(AchievementRequirementType.HolyDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Dark:
                    GetAchievementRequirements(AchievementRequirementType.DarkDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
                case Element.Phantom:
                    GetAchievementRequirements(AchievementRequirementType.PhantomDamage).ForEach(x => x.CurrentValue += args.damage);
                    break;
            }
            if (args.target != null)
            {
                switch (args.target.Race)
                {
                    case ObjectType.Player:
                        GetAchievementRequirements(AchievementRequirementType.TotalPlayerDamage).ForEach(x => x.CurrentValue += args.damage);
                        break;
                    case ObjectType.Monster:
                        GetAchievementRequirements(AchievementRequirementType.TotalMonsterDamage).ForEach(x => x.CurrentValue += args.damage);
                        break;
                }
            }
            GetAchievementRequirements(AchievementRequirementType.TotalDamage).ForEach(x => x.CurrentValue += args.damage);
        }

        #endregion

        #region 人物吸血

        public void OnPlayerLifeSteal(IEvent evt)
        {
            if (!(evt.EventData is PlayerLifeStealEventArgs args))
            {
                SEnvir.Log("PlayerLifeStealEventArgs is null!");
                return;
            }

            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.TotalLifeSteal).ForEach(x => x.CurrentValue += args.amount);
            GetAchievementRequirements(AchievementRequirementType.TotalHPRegen).ForEach(x => x.CurrentValue += args.amount);
        }

        #endregion

        #region 人物在线
        public void OnPlayerOnline(IEvent evt)
        {
            if (!(evt.EventData is PlayerOnlineEventArgs args))
            {
                SEnvir.Log("PlayerOnlineEventArgs is null!");
                return;
            }

            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.OnlineTime).ForEach(x => x.CurrentValue += (decimal)(args.OnlineTime.TotalMinutes));
        }

        #endregion

        #region 人物挖矿
        public void OnPlayerMine(IEvent evt)
        {
            if (!(evt.EventData is PlayerMineEventArgs args))
            {
                SEnvir.Log("PlayerMineEventArgs is null!");
                return;
            }

            if (Character == null) return;
            if (args.item?.Info == null) return;

            foreach (var req in GetAchievementRequirements(AchievementRequirementType.DigTimes))
            {
                if (req.Requirement.ItemParameter != null)
                {
                    if (args.item.Info.Index == req.Requirement.ItemParameter.Index)
                    {
                        req.CurrentValue += 1;
                    }
                }
                else
                {
                    req.CurrentValue += 1;
                }
            }
        }

        #endregion

        #region 人物获得物品
        public void OnPlayerGainItem(IEvent evt)
        {
            if (!(evt.EventData is PlayerGainItemEventArgs args))
            {
                SEnvir.Log("PlayerGainItemEventArgs is null!");
                return;
            }
            if (Character == null) return;
            if (args.items == null) return;

            List<int> indices = (args.items).Select(x => x?.Info?.Index ?? 0).ToList();

            foreach (var req in GetAchievementRequirements(AchievementRequirementType.GainItem))
            {
                if (req.Requirement.ItemParameter != null)
                {
                    if (indices.Contains(req.Requirement.ItemParameter.Index))
                    {
                        req.CurrentValue += 1;
                    }
                }
            }
        }

        #endregion

        #region 人物闪避
        public void OnPlayerDodge(IEvent evt)
        {
            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.TotalDodge).ForEach(x => x.CurrentValue += 1);
        }

        #endregion

        #region 人物排行变化
        public void OnPlayerPlayerRankingChange(IEvent evt)
        {
            if (Character == null) return;

            int rank = 9999;
            switch (Class)
            {
                case MirClass.Warrior:
                    rank = SEnvir.Rankings.Where(x => x.Class == MirClass.Warrior).OrderByDescending(y => y.Level).TakeWhile(z => z.Index != Character.Index).Count();
                    GetAchievementRequirements(AchievementRequirementType.RankWarrior).ForEach(x => x.CurrentValue = rank);
                    break;
                case MirClass.Wizard:
                    rank = SEnvir.Rankings.Where(x => x.Class == MirClass.Wizard).OrderByDescending(y => y.Level).TakeWhile(z => z.Index != Character.Index).Count();
                    GetAchievementRequirements(AchievementRequirementType.RankWizard).ForEach(x => x.CurrentValue = rank);
                    break;
                case MirClass.Taoist:
                    rank = SEnvir.Rankings.Where(x => x.Class == MirClass.Taoist).OrderByDescending(y => y.Level).TakeWhile(z => z.Index != Character.Index).Count();
                    GetAchievementRequirements(AchievementRequirementType.RankTaoist).ForEach(x => x.CurrentValue = rank);
                    break;
                case MirClass.Assassin:
                    rank = SEnvir.Rankings.Where(x => x.Class == MirClass.Assassin).OrderByDescending(y => y.Level).TakeWhile(z => z.Index != Character.Index).Count();
                    GetAchievementRequirements(AchievementRequirementType.RankAssassin).ForEach(x => x.CurrentValue = rank);
                    break;
            }
            int allRank = SEnvir.Rankings.OrderByDescending(y => y.Level).TakeWhile(z => z.Index != Character.Index).Count();
            GetAchievementRequirements(AchievementRequirementType.RankAll).ForEach(x => x.CurrentValue = allRank);
        }

        #endregion


        #region 人物武器锻造
        public void OnPlayerWeaponRefine(IEvent evt)
        {
            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.WeaponRefine).ForEach(x => x.CurrentValue += 1);
        }

        #endregion

        #region 人物武器冶炼
        public void OnPlayerWeaponReset(IEvent evt)
        {
            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.WeaponReset).ForEach(x => x.CurrentValue += 1);
        }

        #endregion

        #region 人物首饰升级
        public void OnPlayerAccessoryRefineLevel(IEvent evt)
        {
            if (Character == null) return;

            GetAchievementRequirements(AchievementRequirementType.AccessoryRefineLevel).ForEach(x => x.CurrentValue += 1);
        }

        #endregion

        #region 人物结婚离婚
        public void OnPlayerMarriage(IEvent evt)
        {
            if (!(evt.EventData is PlayerMarriageEventArgs args))
            {
                SEnvir.Log("PlayerMarriageEventArgs is null!");
                return;
            }
            if (Character == null) return;

            if (!args.IsDivorce)
            {
                GetAchievementRequirements(AchievementRequirementType.Marriage).ForEach(x => x.CurrentValue += 1);
                /*
                args.Partner?.Achievements.SelectMany(
                    x => x.AchievementRequirements).Where(
                    y => y.Requirement.RequirementType == AchievementRequirementType.Marriage).ToList().ForEach(
                    z => z.CurrentValue += 1);

                */
            }
            else
            {
                GetAchievementRequirements(AchievementRequirementType.Divorce).ForEach(x => x.CurrentValue += 1);

                /*
                args.Partner?.Achievements.SelectMany(
                    x => x.AchievementRequirements).Where(
                    y => y.Requirement.RequirementType == AchievementRequirementType.Divorce).ToList().ForEach(
                    z => z.CurrentValue += 1);

                */
            }
        }

        #endregion

        #region 人物被杀
        public void OnPlayerKilled(IEvent evt)
        {
            if (!(evt.EventData is PlayerKilledEventArgs args))
            {
                SEnvir.Log("PlayerKilledEventArgs is null!");
                return;
            }
            if (Character == null) return;

            if (args.Killer != null)
            {
                switch (args.Killer.Race)
                {
                    case ObjectType.Player:
                        GetAchievementRequirements(AchievementRequirementType.TotalKilledByPlayer).ForEach(x => x.CurrentValue += 1);
                        PlayerObject killer = (PlayerObject)args.Killer;

                        killer.Character?.Achievements.SelectMany(
                            x => x.AchievementRequirements).Where(
                            y => y.Requirement.RequirementType == AchievementRequirementType.TotalPlayerKill).ToList().ForEach(
                            z => z.CurrentValue += 1);

                        switch (Class)
                        {
                            case MirClass.Warrior:
                                killer.Character?.Achievements.SelectMany(
                                    x => x.AchievementRequirements).Where(
                                    y => y.Requirement.RequirementType == AchievementRequirementType.TotalWarriorKill).ToList().ForEach(
                                    z => z.CurrentValue += 1);
                                break;
                            case MirClass.Wizard:
                                killer.Character?.Achievements.SelectMany(
                                    x => x.AchievementRequirements).Where(
                                    y => y.Requirement.RequirementType == AchievementRequirementType.TotalWizardKill).ToList().ForEach(
                                    z => z.CurrentValue += 1);
                                break;
                            case MirClass.Taoist:
                                killer.Character?.Achievements.SelectMany(
                                    x => x.AchievementRequirements).Where(
                                    y => y.Requirement.RequirementType == AchievementRequirementType.TotalTaoistKill).ToList().ForEach(
                                    z => z.CurrentValue += 1);
                                break;
                            case MirClass.Assassin:
                                killer.Character?.Achievements.SelectMany(
                                    x => x.AchievementRequirements).Where(
                                    y => y.Requirement.RequirementType == AchievementRequirementType.TotalAssassinKill).ToList().ForEach(
                                    z => z.CurrentValue += 1);
                                break;
                        }

                        break;
                    case ObjectType.Monster:
                        GetAchievementRequirements(AchievementRequirementType.TotalKilledByMonster).ForEach(x => x.CurrentValue += 1);
                        break;
                }
            }

            GetAchievementRequirements(AchievementRequirementType.TotalDeath).ForEach(x => x.CurrentValue += 1);
        }

        #endregion

        #region 道具移动
        public void OnPlayerItemMove(IEvent evt)
        {
            if (!(evt.EventData is PlayerItemMoveEventArgs args))
            {
                SEnvir.Log("PlayerItemMove is null!");
                return;
            }
            if (Character == null) return;

            if (args.ToGridType == GridType.Equipment) //穿上某装备
            {
                if (args.Item != null)
                {
                    GetAchievementRequirements(AchievementRequirementType.PutOnItem).ForEach(x => x.CurrentValue += 1);
                }
            }
        }

        #endregion

        #endregion
    }
}
