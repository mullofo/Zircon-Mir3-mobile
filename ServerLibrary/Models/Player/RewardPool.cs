using Library;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //奖金池玩法相关
    {

        /// <summary>
        /// 激活奖金池buff
        /// </summary>
        public void ApplyRewardPoolBuff()
        {
            BuffRemove(BuffType.RewardPool);
            if (SEnvir.RewardPoolBuffs.Count < 1)
            {
                return;
            }

            Stats buffStats = new Stats();
            foreach (var kvp in SEnvir.RewardPoolBuffs)
            {
                buffStats[kvp.Key] = kvp.Value;
            }

            BuffAdd(BuffType.RewardPool, TimeSpan.MaxValue, buffStats, false, false, TimeSpan.Zero);
        }

        /// <summary>
        /// 检查领取资格
        /// </summary>
        /// <param name="isEXPOwner">是否是经验拥有者</param>
        /// <param name="monster">击杀的怪物</param>
        /// <returns></returns>
        public bool CheckRewardPoolClaimEligibility(bool isEXPOwner, MonsterObject monster)
        {
            // 先检查领取资格
            try
            {
                // 池子信息
                var input1 = Utils.RewardPool.RewardPoolRuleUtils.GetRewardPoolDetails();
                // 击杀信息
                var input2 = new Utils.RewardPool.RewardPoolClaimRewardsParams
                {
                    玩家 = this,
                    是否为经验所有者 = isEXPOwner,
                    怪物 = monster,
                    组队人数 = GroupMembers?.Count ?? 0,
                    击杀时间 = SEnvir.Now,
                };
                // 传给规则引擎
                var resultList = SEnvir.GlobalRulesEngine
                    .ExecuteAllRulesAsync(Config.RewardPoolClaimCheckFileName, input1, input2).Result;

                return resultList.First()?.IsSuccess == true;
                //return resultList.TrueForAll(x => x.IsSuccess);
            }
            catch (Exception e)
            {
                SEnvir.Log("奖金池玩法: 检查领取资格发生错误");
                SEnvir.Log(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 尝试领取奖励
        /// </summary>
        /// <param name="isEXPOwner">是否为经验所有者</param>
        /// <param name="monster">杀了哪只怪物</param>
        /// <returns></returns>
        public decimal ClaimRewardPoolRewards(bool isEXPOwner, MonsterObject monster)
        {
            try
            {
                // 池子信息
                var input1 = Utils.RewardPool.RewardPoolRuleUtils.GetRewardPoolDetails();
                // 击杀信息
                var input2 = new Utils.RewardPool.RewardPoolClaimRewardsParams
                {
                    玩家 = this,
                    是否为经验所有者 = isEXPOwner,
                    怪物 = monster,
                    组队人数 = GroupMembers?.Count ?? 0,
                    击杀时间 = SEnvir.Now,
                };
                // 传给规则引擎
                var resultList = SEnvir.GlobalRulesEngine
                    .ExecuteAllRulesAsync(Config.RewardPoolClaimFileName, input1, input2).Result;
                var result = resultList.First();
                // 成功么
                if (result.IsSuccess)
                {
                    // 拿回具体数值
                    decimal amount = (decimal)result.ActionResult.Output;
                    // 防止扣成负数
                    amount = Math.Min(SEnvir.TheRewardPoolInfo.Balance, amount);
                    // 扣除池子
                    SEnvir.RewardPoolAdjustBalance(-amount);
                    // 加给个人
                    SEnvir.AdjustPersonalReward(this.Character, amount, CurrencySource.KillMobAdd,
                        $"怪物名：{monster?.MonsterInfo?.MonsterName}, 组队人数：{GroupMembers?.Count ?? 0}, 是否为队长：{isEXPOwner}");

                    return amount;
                }

                return 0;
            }
            catch (Exception e)
            {
                SEnvir.Log("奖金池玩法: 尝试领取奖励发生错误");
                SEnvir.Log(e.StackTrace.ToString());
                return 0;
            }
        }
        /// <summary>
        /// 给当前玩家发送最新的奖金池信息
        /// </summary>
        public void SendRewardPoolUpdate()
        {
            Enqueue(new S.RewardPoolUpdate
            {
                RewardPoolInfo = SEnvir.TheRewardPoolInfo.ToClientInfo(),
            });
        }

        /// <summary>
        /// 更新排行榜
        /// </summary>
        public void SendRewardPoolRanks()
        {
            var packet = new S.RewardPoolCoinRankChanged
            {
                First = SEnvir.TotalRPCoinEarnedTop1,
                Second = SEnvir.TotalRPCoinEarnedTop2,
                Third = SEnvir.TotalRPCoinEarnedTop3,
                Myself = SEnvir.GetRPRanksOfCharacter(Character)
            };
            Enqueue(packet);
        }
    }
}
