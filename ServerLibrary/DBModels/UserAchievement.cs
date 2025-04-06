using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;
using System;
using System.Linq;


namespace Server.DBModels
{
    [UserObject]
    public sealed class UserAchievement : DBObject //账号单个成就信息
    {
        [Association("Achievements")]
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

        public AchievementInfo AchievementName
        {
            get { return _AchievementName; }
            set
            {
                if (_AchievementName == value) return;

                var oldValue = _AchievementName;
                _AchievementName = value;

                OnChanged(oldValue, value, "AchievementName");
            }
        }
        private AchievementInfo _AchievementName;

        public bool Completed
        {
            get { return _Completed; }
            set
            {
                if (_Completed == value) return;

                var oldValue = _Completed;
                _Completed = value;

                if (_Completed) CompleteDate = SEnvir.Now;

                OnChanged(oldValue, value, "Completed");
            }
        }
        private bool _Completed;

        public DateTime CompleteDate
        {
            get { return _CompleteDate; }
            set
            {
                if (_CompleteDate == value) return;

                var oldValue = _CompleteDate;
                _CompleteDate = value;

                OnChanged(oldValue, value, "CompleteDate");
            }
        }
        private DateTime _CompleteDate;

        [Association("AchievementRequirements", true)]
        public DBBindingList<UserAchievementRequirement> AchievementRequirements { get; set; }

        [IgnoreProperty]
        public bool IsComplete => AchievementRequirements.Count == AchievementName.AchievementRequirements.Count && AchievementRequirements.All(x => x.Completed);

        protected override internal void OnDeleted()
        {
            Character = null;
            AchievementName = null;

            foreach (var requirement in AchievementRequirements)
                requirement.Delete();

            base.OnDeleted();
        }

#if !ServerTool
        public ClientUserAchievement ToClientInfo()
        {
            return new ClientUserAchievement
            {
                Index = Index,
                AchievementIndex = AchievementName.Index,
                Completed = Completed,
                CompleteDate = CompleteDate.ToString("yyyy.MM.dd"),
                Requirements = AchievementRequirements.Select(x => x.ToClientInfo()).ToList(),
            };
        }
#endif

    }

    [UserObject]
    public sealed class UserAchievementRequirement : DBObject
    {
        [Association("AchievementRequirements")]
        public UserAchievement Achievement
        {
            get { return _Achievement; }
            set
            {
                if (_Achievement == value) return;

                var oldValue = _Achievement;
                _Achievement = value;

                OnChanged(oldValue, value, "Achievement");
            }
        }
        private UserAchievement _Achievement;

        public AchievementRequirement Requirement
        {
            get { return _Requirement; }
            set
            {
                if (_Requirement == value) return;

                var oldValue = _Requirement;
                _Requirement = value;

                OnChanged(oldValue, value, "Requirement");
            }
        }
        private AchievementRequirement _Requirement;

        public decimal CurrentValue
        {
            get { return _CurrentValue; }
            set
            {
                if (_CurrentValue == value) return;

                var oldValue = _CurrentValue;
                _CurrentValue = value;

                OnChanged(oldValue, value, "CurrentValue");

#if !ServerTool
                //改变的成就 加入set
                if (SEnvir.ChangedAchievementIndices != null && Achievement != null)
                    SEnvir.ChangedAchievementIndices.Add(Achievement.Index);
#endif
            }
        }
        private decimal _CurrentValue;

        [IgnoreProperty]
        public bool Completed => Globals.StatusAchievementRequirementTypeList.Contains(Requirement.RequirementType) ||
                                 (!Requirement.Reverse && CurrentValue >= Requirement.RequiredAmount) ||
                                 (Requirement.Reverse && CurrentValue <= Requirement.RequiredAmount);

        protected override internal void OnDeleted()
        {
            Achievement = null;
            Requirement = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientUserAchievementRequirement ToClientInfo()
        {
            return new ClientUserAchievementRequirement
            {
                Index = Index,
                RequirementIndex = Requirement.Index,
                CurrentValue = CurrentValue,
            };
        }
#endif

    }
}
