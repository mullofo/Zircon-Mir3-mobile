using Server.DBModels;
using Server.Envir;
using Server.Models;
using System;

namespace Server.Utils.RewardPool
{
    public static class RewardPoolRuleUtils
    {
        public static RewardPoolDetails GetRewardPoolDetails(string currencyName = null)
        {
            if (SEnvir.TheRewardPoolInfo == null)
            {
                return null;
            }

            return new RewardPoolDetails
            {
                奖金池币种 = currencyName,
                当前奖金池余额 = SEnvir.TheRewardPoolInfo.Balance,
                奖金池余额上限 = SEnvir.TheRewardPoolInfo.MaxAmount,
                当前档位 = SEnvir.TheRewardPoolInfo.CurrentTier,
                当前档位上限 = SEnvir.TheRewardPoolInfo.CurrentUpperLimit
            };
        }
    }

    public class RewardPoolDetails
    {
        public string 奖金池币种 { get; set; }
        public decimal 当前奖金池余额 { get; set; }
        public int 奖金池余额上限 { get; set; }
        public int 当前档位 { get; set; }
        public int 当前档位上限 { get; set; }
    }

    public class RewardPoolAddBalanceParams
    {
        public CharacterInfo 角色 { get; set; }
        public int 数额 { get; set; }
        public string 来源 { get; set; }
    }

    public class RewardPoolClaimRewardsParams
    {
        public PlayerObject 玩家 { get; set; }
        public bool 是否为经验所有者 { get; set; }
        public int 组队人数 { get; set; }
        public MonsterObject 怪物 { get; set; }
        public DateTime 击杀时间 { get; set; }
    }
}