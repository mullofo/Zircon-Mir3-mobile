using Library;
using MirDB;
using Server.Envir;
using System;

namespace Server.DBModels
{
    /// <summary>
    /// 精炼信息
    /// </summary>
    [UserObject]
    public class RefineInfo : DBObject
    {
        /// <summary>
        /// 精炼道具的角色信息
        /// </summary>
        [Association("Refines")]
        public CharacterInfo Character      //角色
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
        /// 精炼武器
        /// </summary>
        [Association("Refine")]
        public UserItem Weapon
        {
            get { return _Weapon; }
            set
            {
                if (_Weapon == value) return;

                var oldValue = _Weapon;
                _Weapon = value;

                OnChanged(oldValue, value, "Weapon");
            }
        }
        private UserItem _Weapon;
        /// <summary>
        /// 武器精炼品质
        /// </summary>
        public RefineQuality Quality
        {
            get { return _Quality; }
            set
            {
                if (_Quality == value) return;

                var oldValue = _Quality;
                _Quality = value;

                OnChanged(oldValue, value, "Quality");
            }
        }
        private RefineQuality _Quality;
        /// <summary>
        /// 武器精炼类型
        /// </summary>
        public RefineType Type
        {
            get { return _Type; }
            set
            {
                if (_Type == value) return;

                var oldValue = _Type;
                _Type = value;

                OnChanged(oldValue, value, "Type");
            }
        }
        private RefineType _Type;
        /// <summary>
        /// 武器精炼时间
        /// </summary>
        public DateTime RetrieveTime
        {
            get { return _RetrieveTime; }
            set
            {
                if (_RetrieveTime == value) return;

                var oldValue = _RetrieveTime;
                _RetrieveTime = value;

                OnChanged(oldValue, value, "RetrieveTime");
            }
        }
        private DateTime _RetrieveTime;
        /// <summary>
        /// 武器精炼几率
        /// </summary>
        public int Chance
        {
            get { return _Chance; }
            set
            {
                if (_Chance == value) return;

                var oldValue = _Chance;
                _Chance = value;

                OnChanged(oldValue, value, "Chance");
            }
        }
        private int _Chance;
        /// <summary>
        /// 武器精炼最大几率
        /// </summary>
        public int MaxChance
        {
            get { return _MaxChance; }
            set
            {
                if (_MaxChance == value) return;

                var oldValue = _MaxChance;
                _MaxChance = value;

                OnChanged(oldValue, value, "MaxChance");
            }
        }
        private int _MaxChance;
        /// <summary>
        /// 判断是否新版的武器升级
        /// </summary>
        public bool IsNewWeaponUpgrade
        {
            get { return _IsNewWeaponUpgrade; }
            set
            {
                if (_IsNewWeaponUpgrade == value) return;

                var oldValue = _IsNewWeaponUpgrade;
                _IsNewWeaponUpgrade = value;

                OnChanged(oldValue, value, "IsNewWeaponUpgrade");
            }
        }
        private bool _IsNewWeaponUpgrade;


        protected override internal void OnDeleted()   //删除时
        {
            Character = null;
            Weapon = null;

            base.OnDeleted();
        }

#if !ServerTool
        public ClientRefineInfo ToClientInfo()    //更新到客户端信息
        {
            return new ClientRefineInfo
            {
                Index = Index,
                Type = Type,
                Quality = Quality,
                Weapon = Weapon.ToClientInfo(),
                Chance = Chance,
                MaxChance = MaxChance,
                ReadyDuration = RetrieveTime > SEnvir.Now ? RetrieveTime - SEnvir.Now : TimeSpan.Zero,
            };

        }
#endif
    }
}
