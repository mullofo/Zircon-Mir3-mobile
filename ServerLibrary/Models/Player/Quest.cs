using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.SystemModels;
using Microsoft.Scripting.Hosting;
using Sentry;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 任务部分
    /// </summary>
    public partial class PlayerObject : MapObject
    {
        #region Quests

        /// <summary>
        /// 判断每日任务或者任务是否完成
        /// </summary>
        public bool HasDaily => Character.Quests.Any(x => x.QuestInfo.QuestType == QuestType.Daily && !x.Completed);
        /// <summary>
        /// 剩余每日任务计数
        /// </summary>
        public int RemainingDailyCount => Math.Max(0, Config.DailyQuestLimit - Character.DailyQuestCount);
        /// <summary>
        /// 处理不需要打怪获得物品的任务
        /// </summary>
        public void UpdateItemOnlyQuestTasks()
        {
            foreach (UserQuest quest in Character.Quests)
            {
                if (quest.Completed) continue;

                foreach (QuestTask task in quest.QuestInfo.Tasks)
                {
                    if (task.MonsterDetails.Count > 0)
                    {
                        continue;
                    }

                    int currentItemCount = GetItemCountFromInventory(task.ItemParameter);

                    UserQuestTask userTask = quest.Tasks.FirstOrDefault(x => x.Task == task);

                    if (userTask == null)
                    {
                        userTask = SEnvir.UserQuestTaskList.CreateNewObject();
                        userTask.Task = task;
                        userTask.Quest = quest;
                    }

                    userTask.Amount = currentItemCount;

                    Enqueue(new S.QuestChanged { Quest = quest.ToClientInfo() });
                }
            }
        }

        /// <summary>
        /// 任务接受
        /// </summary>
        /// <param name="index">任务ID</param>
        public void QuestAccept(int index)
        {
            if (Dead) return;   //如果死亡跳过

            QuestInfo quest = Globals.QuestInfoList.Binding.FirstOrDefault(x => x.Index == index);
            if (quest == null) return;     //如果任务为空跳过

            if (quest.StartItem == null)   //如果任务使用道具为空
            {
                //不是重复任务   不是万事通任务   不是奇遇任务
                if (quest.QuestType != QuestType.Repeatable && quest.QuestType != QuestType.Daily && quest.QuestType != QuestType.Hidden)
                {
                    if (NPC == null) return;       //如果NPC是空的跳过
                    quest = NPC.NPCInfo.StartQuests.FirstOrDefault(x => x.Index == index);
                    if (quest == null) return;     //如果任务等空跳过
                }
            }

            if (!QuestCanAccept(quest)) return;  //如果不是可以接受的任务 跳过

            bool deleteCompleted = false;  //删除完成任务判断

            if (quest.QuestType == QuestType.Daily)   //万事通任务每日数量增加
            {
                Character.DailyQuestCount++;    //完成每日任务次数增加
                deleteCompleted = true;         //删除任务
            }

            if (deleteCompleted)  //如果删除任务
            {
                UserQuest completedQuest = Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == index && x.Completed);

                if (completedQuest != null)  //如果删除任务不为空 执行任务删除对应的任务IDX
                {
                    QuestRemoveByUserQuestIndex(completedQuest.Index);
                }
            }

            UserQuest userQuest = SEnvir.UserQuestList.CreateNewObject();

            userQuest.QuestInfo = quest;
            userQuest.Character = Character;

            Enqueue(new S.QuestChanged { Quest = userQuest.ToClientInfo() });

            UpdateItemOnlyQuestTasks();

            #region 玩家接受任务

            //python 触发
            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, quest });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnAcceptQuest", args);
                    //trig_player(this, "OnAcceptQuest", args);
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

        }
        /// <summary>
        /// 可以接受的任务
        /// </summary>
        /// <param name="quest">任务信息</param>
        /// <returns></returns>
        public bool QuestCanAccept(QuestInfo quest)
        {
            //角色的任务中 如果有一个相同的任务并且此任务并未完成 不可以再重复接
            if (Character.Quests.Any(x => x.QuestInfo == quest && !x.Completed)) return false;

            /*
            if (quest.QuestType != QuestType.Repeatable &&             //如果不是重复任务
                quest.QuestType != QuestType.Daily &&                  //如果不是万事通任务
                Character.Quests.Any(x => x.QuestInfo == quest))       //角色的任务中 是否至少有一个任务 对应任务信息里的任务(已经有这个任务了)
                return false;
            */

            foreach (QuestRequirement requirement in quest.Requirements)
            {
                switch (requirement.Requirement)
                {
                    case QuestRequirementType.MinLevel:
                        if (Level < requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.MaxLevel:
                        if (Level > requirement.IntParameter1) return false;
                        break;
                    case QuestRequirementType.NotAccepted:
                        if (Character.Quests.Any(x => x.QuestInfo.Index == requirement.QuestParameter.Index)) return false;

                        break;
                    case QuestRequirementType.HaveCompleted:
                        if (Character.Quests.Any(x => x.QuestInfo.Index == requirement.QuestParameter.Index && x.Completed)) break;

                        return false;
                    case QuestRequirementType.HaveNotCompleted:
                        if (Character.Quests.Any(x => x.QuestInfo.Index == requirement.QuestParameter.Index && x.Completed)) return false;

                        break;
                    case QuestRequirementType.Class:
                        switch (Class)
                        {
                            case MirClass.Warrior:
                                if ((requirement.Class & RequiredClass.Warrior) != RequiredClass.Warrior) return false;
                                break;
                            case MirClass.Wizard:
                                if ((requirement.Class & RequiredClass.Wizard) != RequiredClass.Wizard) return false;
                                break;
                            case MirClass.Taoist:
                                if ((requirement.Class & RequiredClass.Taoist) != RequiredClass.Taoist) return false;
                                break;
                            case MirClass.Assassin:
                                if ((requirement.Class & RequiredClass.Assassin) != RequiredClass.Assassin) return false;
                                break;
                        }
                        break;
                }
            }

            if (quest.QuestType == QuestType.Repeatable && Character.RepeatableQuestCount >= Config.RepeatableQuestLimit)
            {
                return false;
            }

            if (quest.QuestType == QuestType.Daily && Character.DailyQuestCount >= Config.DailyQuestLimit)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 任务完成
        /// </summary>
        /// <param name="p">完成的任务</param>
        public void QuestComplete(C.QuestComplete p)
        {
            if (Dead) return;

            QuestInfo completedQuest = Globals.QuestInfoList.Binding.FirstOrDefault(x => x.Index == p.Index);
            if (completedQuest == null)
            {
                SEnvir.Log($"完成任务时出错, 找不到任务信息: 任务index={p.Index}");
                SentrySdk.CaptureMessage($"完成任务时出错, 找不到任务信息: 任务index={p.Index}");
                return;
            }

            int choiceIndex = p.ChoiceIndex;

            UserQuest userQuest = null;
            if (completedQuest.FinishNPC == null ||          //完成NPC为空时
                completedQuest.FinishNPC == null && completedQuest.QuestType == QuestType.Repeatable ||   //完成NPC为空 且 是重复任务  可以随身领取奖励
                completedQuest.FinishNPC == null && completedQuest.QuestType == QuestType.Hidden ||       //完成NPC为空 且 是奇遇任务  可以随时领取奖励
                completedQuest.QuestType == QuestType.Event)           //系统定时任务
            {
                //可以随身领奖
                userQuest = Character.Quests.FirstOrDefault(x => x.QuestInfo == completedQuest);
            }
            else
            {
                if (NPC == null) return;
                foreach (QuestInfo quest in NPC.NPCInfo.FinishQuests)
                {
                    if (quest.Index != p.Index) continue;
                    userQuest = Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == quest.Index);
                    break;
                }
            }

            if (userQuest == null)
            {
                SEnvir.Log($"完成任务时出错, 找不到玩家任务信息: 角色名字={Character.CharacterName} 任务index={p.Index}");
                SentrySdk.CaptureMessage($"完成任务时出错, 找不到玩家任务信息: 角色名字={Character.CharacterName} 任务index={p.Index}");
                return;
            }

            if (userQuest.Completed || !userQuest.IsComplete) return;

            //拿取对应物品
            foreach (QuestTask task in completedQuest.Tasks)
            {
                if (task.MonsterDetails == null || task.MonsterDetails.Count > 0) continue;

                if (GetItemCountFromInventory(task.ItemParameter) < task.Amount)
                {
                    Connection.ReceiveChat("包裹中需要的物品不足".Lang(Connection.Language), MessageType.System);
                    return;
                }
            }

            foreach (QuestTask task in completedQuest.Tasks)
            {
                if (task.MonsterDetails == null || task.MonsterDetails.Count > 0) continue;
                TakeItem(task.ItemParameter, task.Amount);
            }

            //发奖励
            List<ItemCheck> checks = new List<ItemCheck>();

            bool hasChoice = false;
            bool hasChosen = false;

            //随机抽取一个随机奖励
            if (userQuest.QuestInfo?.Rewards == null)
            {
                SEnvir.Log($"完成任务时出错, 任务奖励为null。 任务index={p.Index}");
                SentrySdk.CaptureMessage($"完成任务时出错, 任务奖励为null。 任务index={p.Index}");
                return;
            }
            List<QuestReward> randomRewards = userQuest.QuestInfo.Rewards.Where(x => x.Random).ToList();
            if (randomRewards.Count > 0)
            {
                QuestReward randomReward = randomRewards[SEnvir.Random.Next(randomRewards.Count)];
                choiceIndex = randomReward.Index;
            }

            foreach (QuestReward reward in userQuest.QuestInfo.Rewards)
            {
                switch (Class)
                {
                    case MirClass.Warrior:
                        if ((reward.Class & RequiredClass.Warrior) != RequiredClass.Warrior) continue;
                        break;
                    case MirClass.Wizard:
                        if ((reward.Class & RequiredClass.Wizard) != RequiredClass.Wizard) continue;
                        break;
                    case MirClass.Taoist:
                        if ((reward.Class & RequiredClass.Taoist) != RequiredClass.Taoist) continue;
                        break;
                    case MirClass.Assassin:
                        if ((reward.Class & RequiredClass.Assassin) != RequiredClass.Assassin) continue;
                        break;
                }

                if (reward.Choice)  //任务里选择奖励
                {
                    hasChoice = true;
                    if (reward.Index != choiceIndex) continue;

                    hasChosen = true;
                }

                if (reward.Random)
                {
                    if (reward.Index != choiceIndex) continue;

                    hasChosen = true;
                }

                UserItemFlags flags = UserItemFlags.None;
                TimeSpan duration = TimeSpan.FromSeconds(reward.Duration);

                if (reward.Bound)
                    flags |= UserItemFlags.Bound;

                if (duration != TimeSpan.Zero)
                    flags |= UserItemFlags.Expirable;

                ItemCheck check = new ItemCheck(reward.Item, reward.Amount, flags, duration);

                checks.Add(check);
            }

            if (hasChoice && !hasChosen)
            {
                Connection.ReceiveChat("Quest.QuestSelectReward".Lang(Connection.Language), MessageType.System);
                return;
            }

            if (!CanGainItems(false, checks.ToArray()))
            {
                Connection.ReceiveChat("Quest.QuestNeedSpace".Lang(Connection.Language), MessageType.System);
                return;
            }

            foreach (ItemCheck check in checks)
            {
                while (check.Count > 0)
                {
                    UserItem newItem = SEnvir.CreateFreshItem(check);
                    //记录物品来源
                    SEnvir.RecordTrackingInfo(newItem, completedQuest?.FinishNPC?.Region?.Map?.Description, ObjectType.NPC, completedQuest?.FinishNPC?.NPCName + "- " + "任务奖励".Lang(Connection.Language), Character?.CharacterName);

                    GainItem(newItem);
                }
            }

            userQuest.Track = false;
            userQuest.Completed = true;
            if (hasChosen)
                userQuest.SelectedReward = choiceIndex;

            bool deleteCompleted = false;  //删除已完成
            if (completedQuest.QuestType == QuestType.Repeatable)  //重复任务每日数量增加
            {
                Character.RepeatableQuestCount++;
                deleteCompleted = true;
            }

            //不删除奇遇任务
            //if (completedQuest.QuestType == QuestType.Hidden)  //奇遇任务完成
            //{
            //    deleteCompleted = true;
            //}

            #region 玩家完成任务

            //python 触发
            try
            {
                dynamic trig_player;
                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                {
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, userQuest });
                    SEnvir.ExecutePyWithTimer(trig_player, this, "OnCompleteQuest", args);
                    //trig_player(this, "OnCompleteQuest", args);
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

            if (deleteCompleted)  //如果删除任务
            {
                //执行任务删除对应的任务IDX
                QuestRemoveByUserQuestIndex(userQuest.Index);
            }
            else
            {
                Enqueue(new S.QuestChanged { Quest = userQuest.ToClientInfo() });
            }
        }
        /// <summary>
        /// 任务进度跟踪
        /// </summary>
        /// <param name="p">任务跟踪</param>
        public void QuestTrack(C.QuestTrack p)
        {
            UserQuest quest = Character.Quests.FirstOrDefault(x => x.Index == p.Index);

            if (quest == null || quest.Completed) return;

            quest.Track = p.Track;
        }
        /// <summary>
        /// 获取每日任务
        /// </summary>
        public void GetDailyQuest()
        {
            if (HasDaily)
            {
                Connection.ReceiveChat("你已经有每日任务了".Lang(Connection.Language), MessageType.System);
                return;
            }
            List<QuestInfo> Dailies = Globals.QuestInfoList.Binding.Where(quest => quest.QuestType == QuestType.Daily && QuestCanAccept(quest)).ToList();

            if (Dailies.Count > 0)
            {
                QuestInfo daily = Dailies[SEnvir.Random.Next(Dailies.Count)];
                QuestAccept(daily.Index);
                Connection.ReceiveChat($"PlayerObject.GetDailyQuest".Lang(Connection.Language, daily.QuestName), MessageType.System);
            }
            else
            {
                Connection.ReceiveChat("已经没有新的每日任务给你了".Lang(Connection.Language), MessageType.System);
            }
        }
        /// <summary>
        /// 每日任务重置
        /// </summary>
        public void DailyQuestReset()
        {
            if (!HasDaily)
            {
                Connection.ReceiveChat("你没有每日任务".Lang(Connection.Language), MessageType.System);
                return;
            }

            UserQuest daily = Character.Quests.FirstOrDefault(x => x.QuestInfo.QuestType == QuestType.Daily && !x.Completed);
            if (daily != null)
            {
                QuestRemoveByUserQuestIndex(daily.Index);
                GetDailyQuest();
            }
            else
            {
                Connection.ReceiveChat("重置失败, 没有获取到每日任务".Lang(Connection.Language), MessageType.System);
            }
        }
        /// <summary>
        /// 按用户任务索引删除任务
        /// </summary>
        /// <param name="index">任务ID</param>
        public void QuestRemoveByUserQuestIndex(int index)
        {
            foreach (UserQuest quest in Character.Quests)
            {
                if (quest.Index == index)
                {
                    Enqueue(new S.QuestRemoved
                    {
                        Quest = quest.ToClientInfo(),
                        DailyQuestRemains = Config.DailyQuestLimit - Character.DailyQuestCount,
                        RepeatableQuestRemains = Config.RepeatableQuestLimit - Character.RepeatableQuestCount
                    });
                    Character.Quests.Remove(quest);
                    quest.Delete();
                    return;
                }
            }
        }

        /// <summary>
        /// 获取UserQuest
        /// </summary>
        public UserQuest GetUserQuestByQuestIndex(int index)
        {
            return Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == index);
        }

        public List<UserQuest> GetUserQuestsByQuestIndex(int index)
        {
            return Character.Quests.Where(x => x.QuestInfo.Index == index).ToList();
        }

        public void GiveUpQuest(int index)
        {
            UserQuest quest = Character.Quests.FirstOrDefault(x => x.QuestInfo.Index == index);
            if (quest == null)
            {
                // Cannot give up this quest, UserQuest not found.
                Connection.ReceiveChat("无法放弃任务，找不到此任务的信息", MessageType.System);
                return;
            }

            switch (quest.QuestInfo.QuestType)
            {
                case QuestType.Daily:
                    break;
                case QuestType.Repeatable:
                    Character.RepeatableQuestCount++;
                    break;
                default:
                    break;
            }

            QuestRemoveByUserQuestIndex(quest.Index);
            // The selected quest has been removed
            Connection.ReceiveChat("任务已删除", MessageType.System);

        }
        #endregion
    }
}
