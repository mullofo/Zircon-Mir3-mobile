using Library;
using Library.SystemModels;
using MirDB;
using System.Collections.Generic;
using System.Linq;
#if !ServerTool
using Server.Models;
#endif

namespace Server.DBModels
{
    /// <summary>
    /// 角色任务
    /// </summary>
    [UserObject]
    public sealed class UserQuest : DBObject
    {
        /// <summary>
        /// 任务信息
        /// </summary>
        public QuestInfo QuestInfo
        {
            get { return _QuestInfo; }
            set
            {
                if (_QuestInfo == value) return;

                var oldValue = _QuestInfo;
                _QuestInfo = value;

                OnChanged(oldValue, value, "QuestInfo");
            }
        }
        private QuestInfo _QuestInfo;
        /// <summary>
        /// 任务角色信息
        /// </summary>
        [Association("Quests")]
        public CharacterInfo Character
        {
            get { return _Character; }
            set
            {
                if (_Character == value) return;

                var oldValue = _Character;
                _Character = value;

                OnChanged(oldValue, value, "Character");
            }
        }
        private CharacterInfo _Character;
        /// <summary>
        /// 任务是否完成
        /// </summary>
        public bool Completed
        {
            get { return _Completed; }
            set
            {
                if (_Completed == value) return;

                var oldValue = _Completed;
                _Completed = value;

                OnChanged(oldValue, value, "Completed");
            }
        }
        private bool _Completed;
        /// <summary>
        /// 任务选择奖励
        /// </summary>
        public int SelectedReward
        {
            get { return _SelectedReward; }
            set
            {
                if (_SelectedReward == value) return;

                var oldValue = _SelectedReward;
                _SelectedReward = value;

                OnChanged(oldValue, value, "SelectedReward");
            }
        }
        private int _SelectedReward;
        /// <summary>
        /// 是否开启任务跟踪
        /// </summary>
        public bool Track
        {
            get { return _Track; }
            set
            {
                if (_Track == value) return;

                var oldValue = _Track;
                _Track = value;

                OnChanged(oldValue, value, "Track");
            }
        }
        private bool _Track;

        /// <summary>
        /// 存储额外信息
        /// </summary>
        public string ExtraInfo
        {
            get { return _ExtraInfo; }
            set
            {
                if (_ExtraInfo == value) return;

                var oldValue = _ExtraInfo;
                _ExtraInfo = value;

                OnChanged(oldValue, value, "ExtraInfo");
            }
        }
        private string _ExtraInfo;

        [IgnoreProperty]
        public bool IsComplete => Tasks.Count == QuestInfo.Tasks.Count && Tasks.All(x => x.Completed);

        [Association("Tasks", true)]
        public DBBindingList<UserQuestTask> Tasks { get; set; }

        protected override internal void OnDeleted()   //删除时
        {
            QuestInfo = null;
            Character = null;
            ExtraInfo = null;

            for (int i = Tasks.Count - 1; i >= 0; i--)
                Tasks[i].Delete();

            base.OnDeleted();
        }
        protected override internal void OnCreated()   //创建时
        {
            base.OnCreated();

            Track = true;
        }

#if !ServerTool
        public ClientUserQuest ToClientInfo()   //更新客户端信息
        {
            return new ClientUserQuest
            {
                Index = Index,
                QuestIndex = QuestInfo.Index,
                Completed = Completed,
                SelectedReward = SelectedReward,
                Track = Track,
                ItemIndex = QuestInfo.StartItem?.Index ?? 0,
                ExtraInfo = ExtraInfo,

                Tasks = Tasks.Select(x => x.ToClientInfo()).ToList(),
            };
        }
#endif
    }


    [UserObject]
    public sealed class UserQuestTask : DBObject
    {
        [Association("Tasks")]
        public UserQuest Quest       //任务
        {
            get { return _Quest; }
            set
            {
                if (_Quest == value) return;

                var oldValue = _Quest;
                _Quest = value;

                OnChanged(oldValue, value, "Quest");
            }
        }
        private UserQuest _Quest;

        public QuestTask Task     //交接
        {
            get { return _Task; }
            set
            {
                if (_Task == value) return;

                var oldValue = _Task;
                _Task = value;

                OnChanged(oldValue, value, "Task");
            }
        }
        private QuestTask _Task;

        public long Amount      //数量
        {
            get { return _Amount; }
            set
            {
                if (_Amount == value) return;

                var oldValue = _Amount;
                _Amount = value;

                OnChanged(oldValue, value, "Amount");
            }
        }
        private long _Amount;

        [IgnoreProperty]
        public bool Completed => Amount >= Task.Amount;

        protected override internal void OnDeleted()       //删除时
        {
            Quest = null;
            Task = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientUserQuestTask ToClientInfo()   //更新客户端信息
        {
            return new ClientUserQuestTask
            {
                Index = Index,

                TaskIndex = Task.Index,

                Amount = Amount
            };
        }

        public List<ItemObject> Objects = new List<ItemObject>();
#endif
    }
}
