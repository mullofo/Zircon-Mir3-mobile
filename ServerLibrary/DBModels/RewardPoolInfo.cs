using Library;
using MirDB;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.DBModels
{
    [UserObject]
    public sealed class RewardPoolInfo : DBObject
    {
        /// <summary>
        /// 奖金池内货币的名字 例如“彩池币”
        /// </summary>
        public string CurrencyName
        {
            get { return _CurrencyName; }
            set
            {
                if (_CurrencyName == value) return;

                var oldValue = _CurrencyName;
                _CurrencyName = value;

                OnChanged(oldValue, value, "CurrencyName");
            }
        }
        private string _CurrencyName;

        /// <summary>
        /// 当前奖池内货币数量
        /// </summary>
        public decimal Balance
        {
            get { return _Balance; }
            set
            {
                if (_Balance == value) return;

                var oldValue = _Balance;
                _Balance = value;

                OnChanged(oldValue, value, "Balance");
            }
        }
        private decimal _Balance;

        /// <summary>
        /// 超过这个值不再累计
        /// </summary>
        public int MaxAmount
        {
            get { return _MaxAmount; }
            set
            {
                if (_MaxAmount == value) return;

                var oldValue = _MaxAmount;
                _MaxAmount = value;

                OnChanged(oldValue, value, "MaxAmount");
            }
        }
        private int _MaxAmount;

        /// <summary>
        /// 档位1数量上限 小于此数值即档位1
        /// </summary>
        public int Tier1UpperLimit
        {
            get { return _Tier1UpperLimit; }
            set
            {
                if (_Tier1UpperLimit == value) return;

                var oldValue = _Tier1UpperLimit;
                _Tier1UpperLimit = value;

                OnChanged(oldValue, value, "Tier1UpperLimit");
            }
        }
        private int _Tier1UpperLimit;
        /// <summary>
        /// 档位2数量上限 小于此数值即档位2
        /// </summary>
        public int Tier2UpperLimit
        {
            get { return _Tier2UpperLimit; }
            set
            {
                if (_Tier2UpperLimit == value) return;

                var oldValue = _Tier2UpperLimit;
                _Tier2UpperLimit = value;

                OnChanged(oldValue, value, "Tier2UpperLimit");
            }
        }
        private int _Tier2UpperLimit;
        /// <summary>
        /// 档位3数量上限 小于此数值即档位3
        /// </summary>
        public int Tier3UpperLimit
        {
            get { return _Tier3UpperLimit; }
            set
            {
                if (_Tier3UpperLimit == value) return;

                var oldValue = _Tier3UpperLimit;
                _Tier3UpperLimit = value;

                OnChanged(oldValue, value, "Tier3UpperLimit");
            }
        }
        private int _Tier3UpperLimit;
        /// <summary>
        /// 档位4数量上限 小于此数值即档位4
        /// </summary>
        public int Tier4UpperLimit
        {
            get { return _Tier4UpperLimit; }
            set
            {
                if (_Tier4UpperLimit == value) return;

                var oldValue = _Tier4UpperLimit;
                _Tier4UpperLimit = value;

                OnChanged(oldValue, value, "Tier4UpperLimit");
            }
        }
        private int _Tier4UpperLimit;
        /// <summary>
        /// 档位5数量上限 小于此数值即档位5
        /// </summary>
        public int Tier5UpperLimit
        {
            get { return _Tier5UpperLimit; }
            set
            {
                if (_Tier5UpperLimit == value) return;

                var oldValue = _Tier5UpperLimit;
                _Tier5UpperLimit = value;

                OnChanged(oldValue, value, "Tier5UpperLimit");
            }
        }
        private int _Tier5UpperLimit;

        /// <summary>
        /// 当前档位
        /// </summary>
        [IgnoreProperty]
        public int CurrentTier
        {
            get
            {
                if (Balance == 0) return 1;

                if (Balance < Tier1UpperLimit)
                {
                    return 1;
                }
                if (Balance < Tier2UpperLimit)
                {
                    return 2;
                }
                if (Balance < Tier3UpperLimit)
                {
                    return 3;
                }
                if (Balance < Tier4UpperLimit)
                {
                    return 4;
                }
                if (Balance < Tier5UpperLimit)
                {
                    return 5;
                }

                return 5;
            }
        }

        /// <summary>
        /// 当前档位的上限值
        /// </summary>
        [IgnoreProperty]
        public int CurrentUpperLimit
        {
            get
            {
                if (Balance == 0) return Tier1UpperLimit;

                if (Balance < Tier1UpperLimit)
                {
                    return Tier1UpperLimit;
                }
                if (Balance < Tier2UpperLimit)
                {
                    return Tier2UpperLimit;
                }
                if (Balance < Tier3UpperLimit)
                {
                    return Tier3UpperLimit;
                }
                if (Balance < Tier4UpperLimit)
                {
                    return Tier4UpperLimit;
                }
                if (Balance < Tier5UpperLimit)
                {
                    return Tier5UpperLimit;
                }

                return Tier5UpperLimit;
            }
        }
        /// <summary>
        /// 当前档位的下限值
        /// </summary>
        [IgnoreProperty]
        public int CurrentLowerLimit
        {
            get
            {
                if (Balance == 0) return 0;

                if (Balance < Tier1UpperLimit)
                {
                    return 0;
                }
                if (Balance < Tier2UpperLimit)
                {
                    return Tier1UpperLimit;
                }
                if (Balance < Tier3UpperLimit)
                {
                    return Tier2UpperLimit;
                }
                if (Balance < Tier4UpperLimit)
                {
                    return Tier3UpperLimit;
                }
                if (Balance < Tier5UpperLimit)
                {
                    return Tier4UpperLimit;
                }

                return Tier5UpperLimit;
            }
        }

        /// <summary>
        /// 上次给客户端发送更新的时间
        /// </summary>
        [IgnoreProperty]
        public DateTime LastSendUpdateTime { get; set; } = DateTime.MinValue;

#if !ServerTool
        public ClientRewardPoolInfo ToClientInfo()
        {
            return new ClientRewardPoolInfo
            {
                CurrencyName = CurrencyName,
                Balance = Balance,
                PoolMax = MaxAmount,
                CurrentTier = CurrentTier,
                CurrentLowerLimit = CurrentLowerLimit,
                CurrentUpperLimit = CurrentUpperLimit
            };
        }
#endif

        public void BroadcastRewardPoolUpdate()
        {
#if !ServerTool
            SEnvir.Broadcast(new S.RewardPoolUpdate
            {
                RewardPoolInfo = ToClientInfo()
            });

            LastSendUpdateTime = SEnvir.Now;
#endif
        }

    }
}