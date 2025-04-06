using Library;
using Library.SystemModels;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    [UserObject]
    /// <summary>
    /// 角色魔法技能
    /// </summary>
    public sealed class UserMagic : DBObject
    {
        /// <summary>
        /// 魔法技能信息
        /// </summary>
        public MagicInfo Info
        {
            get { return _Info; }
            set
            {
                if (_Info == value) return;

                var oldValue = _Info;
                _Info = value;

                OnChanged(oldValue, value, "Info");
            }
        }
        private MagicInfo _Info;

        [Association("Magics")]
        /// <summary>
        /// 角色
        /// </summary>
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
        /// 键值第1列
        /// </summary>
        public SpellKey Set1Key
        {
            get { return _Set1Key; }
            set
            {
                if (_Set1Key == value) return;

                var oldValue = _Set1Key;
                _Set1Key = value;

                OnChanged(oldValue, value, "Set1Key");
            }
        }
        private SpellKey _Set1Key;
        /// <summary>
        /// 键值第2列
        /// </summary>
        public SpellKey Set2Key
        {
            get { return _Set2Key; }
            set
            {
                if (_Set2Key == value) return;

                var oldValue = _Set2Key;
                _Set2Key = value;

                OnChanged(oldValue, value, "Set2Key");
            }
        }
        private SpellKey _Set2Key;
        /// <summary>
        /// 键值第3列
        /// </summary>
        public SpellKey Set3Key
        {
            get { return _Set3Key; }
            set
            {
                if (_Set3Key == value) return;

                var oldValue = _Set3Key;
                _Set3Key = value;

                OnChanged(oldValue, value, "Set3Key");
            }
        }
        private SpellKey _Set3Key;
        /// <summary>
        /// 键值第4列
        /// </summary>
        public SpellKey Set4Key
        {
            get { return _Set4Key; }
            set
            {
                if (_Set4Key == value) return;

                var oldValue = _Set4Key;
                _Set4Key = value;

                OnChanged(oldValue, value, "Set4Key");
            }
        }
        private SpellKey _Set4Key;
        /// <summary>
        /// 魔法技能等级
        /// </summary>
        public int Level
        {
            get { return _Level; }
            set
            {
                if (_Level == value) return;

                var oldValue = _Level;
                _Level = value;

                OnChanged(oldValue, value, "Level");
            }
        }
        private int _Level;
        /// <summary>
        /// 魔法技能经验
        /// </summary>
        public long Experience
        {
            get { return _Experience; }
            set
            {
                if (_Experience == value) return;

                var oldValue = _Experience;
                _Experience = value;

                OnChanged(oldValue, value, "Experience");
            }
        }
        private long _Experience;
        /// <summary>
        /// 魔法技能冷却时间
        /// </summary>
        public DateTime Cooldown;

        /// <summary>
        /// 消耗=技能基础消耗+等级*等级消耗/3
        /// </summary>
        [IgnoreProperty]
        public int Cost => Info.BaseCost + Level * Info.LevelCost / 3;

        /// <summary>
        /// 删除时
        /// </summary>
        protected override internal void OnDeleted()
        {
            Info = null;
            Character = null;

            base.OnDeleted();
        }

#if !ServerTool
        /// <summary>
        /// 获取攻击值
        /// </summary>
        /// <returns></returns>
        public int GetPower()
        {
            int min = Info.MinBasePower + Level * Info.MinLevelPower / 3;          //最小值 = 最低基础攻击值 + 技能等级*最低技能升级攻击值/3
            int max = Info.MaxBasePower + Level * Info.MaxLevelPower / 3;      //最高值 = 最高基础攻击值 + 技能等级*最高技能升级攻击值/3

            if (min < 0) min = 0;     //如果 最小值小于0 那么最小值等0
            if (min >= max) return min;  //如果 （最小值 大于或者等 最高值）  返回 最小值

            return SEnvir.Random.Next(min, max + 1); //返回 随机值（最小值，最高值+1）
        }

        /// <summary>
        /// 更新客户端信息
        /// </summary>
        /// <returns></returns>
        public ClientUserMagic ToClientInfo()
        {
            return new ClientUserMagic
            {
                Index = Index,
                InfoIndex = Info.Index,

                Set1Key = Set1Key,
                Set2Key = Set2Key,
                Set3Key = Set3Key,
                Set4Key = Set4Key,

                Level = Level,
                Experience = Experience,

                Cooldown = Cooldown > SEnvir.Now ? Cooldown - SEnvir.Now : TimeSpan.Zero,
            };
        }
#endif
    }
}
