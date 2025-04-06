using Library;
using MirDB;
using System;

namespace Server.DBModels
{
    /// <summary>
    /// 角色战争统计
    /// </summary>
    [UserObject]
    public class UserConquestStats : DBObject
    {
        /// <summary>
        /// 参与攻城的角色信息
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
        /// 攻城战开始的时间
        /// </summary>
        public DateTime WarStartDate
        {
            get { return _WarStartDate; }
            set
            {
                if (_WarStartDate == value) return;

                var oldValue = _WarStartDate;
                _WarStartDate = value;

                OnChanged(oldValue, value, "WarStartDate");
            }
        }
        private DateTime _WarStartDate;
        /// <summary>
        /// 攻城城堡的名称
        /// </summary>
        public string CastleName
        {
            get { return _CastleName; }
            set
            {
                if (_CastleName == value) return;

                var oldValue = _CastleName;
                _CastleName = value;

                OnChanged(oldValue, value, "CastleName");
            }
        }
        private string _CastleName;

        /// <summary>
        /// 参与攻城的角色名字
        /// </summary>
        public string CharacterName
        {
            get { return _CharacterName; }
            set
            {
                if (_CharacterName == value) return;

                var oldValue = _CharacterName;
                _CharacterName = value;

                OnChanged(oldValue, value, "CharacterName");
            }
        }
        private string _CharacterName;
        /// <summary>
        /// 参与攻城的行会名字
        /// </summary>
        public string GuildName
        {
            get { return _GuildName; }
            set
            {
                if (_GuildName == value) return;

                var oldValue = _GuildName;
                _GuildName = value;

                OnChanged(oldValue, value, "GuildName");
            }
        }
        private string _GuildName;
        /// <summary>
        /// 参与攻城的角色等级
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
        /// 参与攻城的角色职业
        /// </summary>
        public MirClass Class
        {
            get { return _Class; }
            set
            {
                if (_Class == value) return;

                var oldValue = _Class;
                _Class = value;

                OnChanged(oldValue, value, "Class");
            }
        }
        private MirClass _Class;
        /// <summary>
        /// BOSS受到的伤害
        /// </summary>
        public int BossDamageTaken
        {
            get { return _BossDamageTaken; }
            set
            {
                if (_BossDamageTaken == value) return;

                var oldValue = _BossDamageTaken;
                _BossDamageTaken = value;

                OnChanged(oldValue, value, "BossDamageTaken");
            }
        }
        private int _BossDamageTaken;
        /// <summary>
        /// BOSS造成的伤害
        /// </summary>
        public int BossDamageDealt
        {
            get { return _BossDamageDealt; }
            set
            {
                if (_BossDamageDealt == value) return;

                var oldValue = _BossDamageDealt;
                _BossDamageDealt = value;

                OnChanged(oldValue, value, "BossDamageDealt");
            }
        }
        private int _BossDamageDealt;
        /// <summary>
        /// BOSS死亡计数
        /// </summary>
        public int BossDeathCount
        {
            get { return _BossDeathCount; }
            set
            {
                if (_BossDeathCount == value) return;

                var oldValue = _BossDeathCount;
                _BossDeathCount = value;

                OnChanged(oldValue, value, "BossDeathCount");
            }
        }
        private int _BossDeathCount;
        /// <summary>
        /// BOSS击杀计数
        /// </summary>
        public int BossKillCount
        {
            get { return _BossKillCount; }
            set
            {
                if (_BossKillCount == value) return;

                var oldValue = _BossKillCount;
                _BossKillCount = value;

                OnChanged(oldValue, value, "BossKillCount");
            }
        }
        private int _BossKillCount;
        /// <summary>
        /// PVP受到的伤害
        /// </summary>
        public int PvPDamageTaken
        {
            get { return _PvPDamageTaken; }
            set
            {
                if (_PvPDamageTaken == value) return;

                var oldValue = _PvPDamageTaken;
                _PvPDamageTaken = value;

                OnChanged(oldValue, value, "PvPDamageTaken");
            }
        }
        private int _PvPDamageTaken;
        /// <summary>
        /// PVP造成的伤害
        /// </summary>
        public int PvPDamageDealt
        {
            get { return _PvPDamageDealt; }
            set
            {
                if (_PvPDamageDealt == value) return;

                var oldValue = _PvPDamageDealt;
                _PvPDamageDealt = value;

                OnChanged(oldValue, value, "PvPDamageDealt");
            }
        }
        private int _PvPDamageDealt;
        /// <summary>
        /// PVP击杀计数
        /// </summary>
        public int PvPKillCount
        {
            get { return _PvPKillCount; }
            set
            {
                if (_PvPKillCount == value) return;

                var oldValue = _PvPKillCount;
                _PvPKillCount = value;

                OnChanged(oldValue, value, "PvPKillCount");
            }
        }
        private int _PvPKillCount;
        /// <summary>
        /// PVP死亡计数
        /// </summary>
        public int PvPDeathCount
        {
            get { return _PvPDeathCount; }
            set
            {
                if (_PvPDeathCount == value) return;

                var oldValue = _PvPDeathCount;
                _PvPDeathCount = value;

                OnChanged(oldValue, value, "PvPDeathCount");
            }
        }
        private int _PvPDeathCount;
    }
}
