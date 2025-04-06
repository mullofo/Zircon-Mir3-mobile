using Library;
using MirDB;
using Server.Envir;
using System;
using System.Linq;
#if !ServerTool
using Server.Models;
#endif


namespace Server.DBModels
{
    /// <summary>
    /// 红包信息
    /// </summary>
    [UserObject]
    public sealed class RedPacketInfo : DBObject
    {
        public CurrencyType Currency
        {
            get { return _Currency; }
            set
            {
                if (_Currency == value) return;

                var oldValue = _Currency;
                _Currency = value;

                OnChanged(oldValue, value, "Currency");
            }
        }
        private CurrencyType _Currency;

        /// <summary>
        /// 红包面额
        /// </summary>
        public decimal FaceValue
        {
            get { return _FaceValue; }
            set
            {
                if (_FaceValue == value) return;

                var oldValue = _FaceValue;
                _FaceValue = Math.Round(value, 2);

                OnChanged(oldValue, _FaceValue, "FaceValue");
            }
        }
        private decimal _FaceValue;

        /// <summary>
        /// 未领取的金额
        /// </summary>
        public decimal RemainingValue
        {
            get { return _RemainingValue; }
            set
            {
                if (_RemainingValue == value) return;

                var oldValue = _RemainingValue;
                _RemainingValue = Math.Round(value, 2);

                OnChanged(oldValue, _RemainingValue, "RemainingValue");
            }
        }
        private decimal _RemainingValue;

        /// <summary>
        /// 个数
        /// </summary>
        public int TotalCount
        {
            get { return _TotalCount; }
            set
            {
                if (_TotalCount == value) return;

                var oldValue = _TotalCount;
                _TotalCount = value;

                OnChanged(oldValue, value, "TotalCount");
            }
        }
        private int _TotalCount;

        /// <summary>
        /// 红包类型-拼手气 平均分
        /// </summary>
        public RedPacketType Type
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
        private RedPacketType _Type;

        /// <summary>
        /// 红包领取范围-全服 行会 小队
        /// </summary>
        public RedPacketScope Scope
        {
            get { return _Scope; }
            set
            {
                if (_Scope == value) return;

                var oldValue = _Scope;
                _Scope = value;

                OnChanged(oldValue, value, "Scope");
            }
        }
        private RedPacketScope _Scope;
        /// <summary>
        /// 红包祝福语
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set
            {
                if (_Message == value) return;

                var oldValue = _Message;
                _Message = value;

                OnChanged(oldValue, value, "Message");
            }
        }
        private string _Message;
        /// <summary>
        /// 发送者
        /// </summary>
        public CharacterInfo Sender
        {
            get { return _Sender; }
            set
            {
                if (_Sender == value) return;

                var oldValue = _Sender;
                _Sender = value;

                OnChanged(oldValue, value, "Sender");
            }
        }
        private CharacterInfo _Sender;

        /// <summary>
        /// 发送时间
        /// </summary>
        public DateTime SendTime
        {
            get { return _SendTime; }
            set
            {
                if (_SendTime == value) return;

                var oldValue = _SendTime;
                _SendTime = value;

                OnChanged(oldValue, value, "SendTime");
            }
        }
        private DateTime _SendTime;

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireTime
        {
            get { return _ExpireTime; }
            set
            {
                if (_ExpireTime == value) return;

                var oldValue = _ExpireTime;
                _ExpireTime = value;

                OnChanged(oldValue, value, "ExpireTime");
            }
        }
        private DateTime _ExpireTime;

        /// <summary>
        /// 是否已过期
        /// </summary>
        [IgnoreProperty]
        public bool HasExpired => ExpireTime < SEnvir.Now;
        /// <summary>
        /// 剩余个数
        /// </summary>
        [IgnoreProperty]
        public int RemainingCount => TotalCount - ClaimRecords.Count;

        [Association("ClaimRecords", true)]
        public DBBindingList<RedPacketClaimInfo> ClaimRecords { get; set; }

        protected internal override void OnCreated()
        {
            base.OnCreated();

            SendTime = SEnvir.Now;
            RemainingValue = FaceValue;
            Message = "恭喜发财，大吉大利";
        }

#if !ServerTool
        /// <summary>
        /// 领取
        /// </summary>
        /// <param name="claimer">领取者</param>
        /// <param name="amount">领取数额</param>
        public bool Claim(PlayerObject claimer, decimal amount)
        {
            if (claimer == null)
            {
                SEnvir.Log("红包领取者不能是null");
                return false;
            }
            if (amount < 0)
            {
                SEnvir.Log("红包领取金额不能是负数");
                return false;
            }
            if (amount > this.RemainingValue)
            {
                SEnvir.Log("红包领取金额大于红包余额");
                return false;
            }
            var record = SEnvir.RedPacketClaimInfoList.CreateNewObject();
            record.RedPacket = this;
            record.Claimer = claimer.Character;
            record.Amount = amount;
            record.ClaimTime = SEnvir.Now;

            // 给人物添加
            switch (Currency)
            {
                case CurrencyType.None:
                    break;
                case CurrencyType.Gold:
                    claimer.ChangeGold(Convert.ToInt64(amount));
                    break;
                case CurrencyType.GameGold:
                    claimer.ChangeGameGold(Convert.ToInt32(amount), "红包系统", Library.CurrencySource.RedPacketAdd);
                    break;
                case CurrencyType.Prestige:
                    claimer.ChangePrestige(Convert.ToInt32(amount));
                    break;
                case CurrencyType.Contribute:
                    claimer.ChangeContribute(Convert.ToInt32(amount));
                    break;
                case CurrencyType.RewardPoolCurrency:
                    SEnvir.AdjustPersonalReward(claimer.Character, amount, CurrencySource.RedPacketAdd);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        /// <summary>
        /// 获取红包货币类型的名字
        /// </summary>
        /// <returns>货币名字</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetCurrencyName()
        {
            switch (Currency)
            {
                case CurrencyType.None:
                    return "无";
                case CurrencyType.Gold:
                    return "金币";
                case CurrencyType.GameGold:
                    return "赞助币";
                case CurrencyType.Prestige:
                    return "声望";
                case CurrencyType.Contribute:
                    return "贡献";
                case CurrencyType.RewardPoolCurrency:
                    return SEnvir.TheRewardPoolInfo.CurrencyName;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ClientRedPacketInfo ToClientInfo()
        {
            return new ClientRedPacketInfo
            {
                Index = Index,
                Currency = Currency,
                FaceValue = FaceValue,
                RemainingValue = RemainingValue,
                TotalCount = TotalCount,
                Type = Type,
                Scope = Scope,
                Message = Message,
                SenderName = Sender == null ? "系统" : Sender.CharacterName,
                SendTime = SendTime,
                ExpireTime = ExpireTime,
                RemainingCount = RemainingCount,
                ClaimInfoList = ClaimRecords.Select(x => x.ToClientInfo()).ToList(),
                HasExpired = HasExpired
            };
        }
#endif

    }

    /// <summary>
    /// 红包领取记录
    /// </summary>
    [UserObject]
    public sealed class RedPacketClaimInfo : DBObject
    {
        [Association("ClaimRecords")]
        public RedPacketInfo RedPacket
        {
            get { return _RedPacket; }
            set
            {
                if (_RedPacket == value) return;

                var oldValue = _RedPacket;
                _RedPacket = value;

                OnChanged(oldValue, value, "RedPacket");
            }
        }
        private RedPacketInfo _RedPacket;

        /// <summary>
        /// 领取人
        /// </summary>
        [Association("RedPocketClaimRecords")]
        public CharacterInfo Claimer
        {
            get { return _Claimer; }
            set
            {
                if (_Claimer == value) return;

                var oldValue = _Claimer;
                _Claimer = value;

                OnChanged(oldValue, value, "Claimer");
            }
        }
        private CharacterInfo _Claimer;

        /// <summary>
        /// 数额
        /// </summary>
        public decimal Amount
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
        private decimal _Amount;

        /// <summary>
        /// 领取时间
        /// </summary>
        public DateTime ClaimTime
        {
            get { return _ClaimTime; }
            set
            {
                if (_ClaimTime == value) return;

                var oldValue = _ClaimTime;
                _ClaimTime = value;

                OnChanged(oldValue, value, "ClaimTime");
            }
        }
        private DateTime _ClaimTime;

        protected internal override void OnDeleted()
        {
            RedPacket = null;
            Claimer = null;
            base.OnDeleted();
        }

#if !ServerTool
        public ClientRedPacketClaimInfo ToClientInfo()
        {
            return new ClientRedPacketClaimInfo
            {
                RedPacketIndex = RedPacket.Index,
                Amount = Amount,
                ClaimerName = Claimer.CharacterName,
                ClaimTime = ClaimTime,
                Currency = RedPacket.Currency
            };
        }
#endif
    }
}