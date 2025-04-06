using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject // 红包相关
    {
        /// <summary>
        /// 领取红包
        /// </summary>
        /// <param name="redPacket"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public bool OpenRedPacket(RedPacketInfo redPacket)
        {
            if (redPacket == null)
            {
                Connection.ReceiveChat("红包领取失败: 红包不存在", MessageType.System);
                return false;
            }

            if (redPacket.HasExpired)
            {
                Connection.ReceiveChat("红包领取失败: 红包已过期", MessageType.System);
                return false;
            }

            if (redPacket.RemainingValue <= 0)
            {
                Connection.ReceiveChat("红包领取失败: 红包余额为0", MessageType.System);
                return false;
            }

            if (redPacket.RemainingCount < 1)
            {
                Connection.ReceiveChat("红包领取失败: 红包已被领完", MessageType.System);
                return false;
            }

            if (redPacket.ClaimRecords.Any(x => x.Claimer.Index == Character.Index))
            {
                Connection.ReceiveChat("红包领取失败: 你已经领过一次了", MessageType.System);
                return false;
            }

            // 角色是否满足领取要求
            switch (redPacket.Scope)
            {
                case RedPacketScope.None:
                    SEnvir.Log("红包出错：范围未定义");
                    return false;
                case RedPacketScope.Server:
                    break;
                case RedPacketScope.Guild:
                    if (redPacket.Sender == null)
                    {
                        SEnvir.Log("红包出错：红包类型为行会红包, 发送者为空");
                        return false;
                    }
                    if (!SEnvir.InSameGuild(Character, redPacket.Sender))
                    {
                        Connection.ReceiveChat($"此红包仅限行会 {redPacket.Sender.Account.GuildMember.Guild.GuildName} 成员领取", MessageType.System);
                        return false;
                    }
                    break;
                case RedPacketScope.Group:
                    if (redPacket.Sender == null)
                    {
                        SEnvir.Log("红包出错：红包类型为小队红包, 发送者为空");
                        return false;
                    }
                    if (redPacket.Sender.Account?.Connection?.Player == null)
                    {
                        Connection.ReceiveChat($"红包出错：红包类型为小队红包, 发送者 {redPacket.Sender.CharacterName} 不在线", MessageType.System);
                        return false;
                    }

                    if (!redPacket.Sender.Account.Connection.Player.GroupMembers.Select(x => x.Character.Index)
                        .Contains(Character.Index))
                    {
                        Connection.ReceiveChat($"红包出错：红包类型为小队红包, 你不在发送者 {redPacket.Sender.CharacterName} 的小队", MessageType.System);
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            // 计算领取金额
            decimal claimValue = 0M;
            // 最后一个人领取全部剩余余额
            if (redPacket.RemainingCount == 1)
            {
                claimValue = redPacket.RemainingValue;
            }
            else
            {
                switch (redPacket.Type)
                {
                    case RedPacketType.None:
                        SEnvir.Log("红包出错：类型未定义");
                        return false;
                    case RedPacketType.Randomly:
                        // 2倍平均值法
                        // 红包只保留2位小数 所以先乘100便于取随机数
                        int upperLimit = (int)(redPacket.RemainingValue / redPacket.RemainingCount * 2 * 100M);
                        claimValue = SEnvir.Random.Next(upperLimit) / 100M;
                        break;
                    case RedPacketType.Evenly:
                        // 每人领取的数额都一样
                        claimValue = redPacket.FaceValue / redPacket.TotalCount;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            // 记录
            claimValue = Math.Round(claimValue, 2);
            if (redPacket.Claim(this, claimValue))
            {
                string sender = redPacket.Sender == null ? "系统" : redPacket.Sender.CharacterName;
                Connection.ReceiveChat($"恭喜你领取了 {sender} 的红包, 抢到了 {claimValue} 个 {redPacket.GetCurrencyName()}", MessageType.System);
                Enqueue(new S.RedPacketUpdate
                {
                    RedPacket = redPacket.ToClientInfo(),
                });
                return true;
            }
            else
            {
                Connection.ReceiveChat("红包领取出错", MessageType.System);
                SEnvir.Log("红包出错：领取失败");
                return false;
            }
        }

        /// <summary>
        /// 发红包
        /// </summary>
        /// <param name="value">总面额</param>
        /// <param name="count">个数</param>
        /// <param name="currency">货币类型</param>
        /// <param name="type">红包类型</param>
        /// <param name="scope">哪些人可以领</param>
        /// <param name="message">祝福语</param>
        /// <param name="durationInSec">多少秒后过期</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public RedPacketInfo SendRedPacket(decimal value, int count,
            CurrencyType currency = CurrencyType.RewardPoolCurrency,
            RedPacketType type = RedPacketType.Randomly,
            RedPacketScope scope = RedPacketScope.Server,
            string message = "",
            int durationInSec = 60)
        {
            if (Config.EnableRedPacket)
            {
                bool paid = false;

                switch (currency)
                {
                    case CurrencyType.None:
                        SEnvir.Log($"玩家 {Character.CharacterName} 发送红包失败: 货币类型未设置");
                        break;
                    case CurrencyType.Gold:
                        if (Gold >= value)
                        {
                            ChangeGold(Convert.ToInt64(-value));
                            paid = true;
                        }
                        break;
                    case CurrencyType.GameGold:
                        if (GameGold >= value)
                        {
                            ChangeGameGold(Convert.ToInt32(-value), "红包系统", CurrencySource.RedPacketDeduct, "SendRedPacket()调用");
                            paid = true;
                        }
                        break;
                    case CurrencyType.Prestige:
                        if (Prestige >= value)
                        {
                            ChangePrestige(Convert.ToInt32(-value));
                            paid = true;
                        }
                        break;
                    case CurrencyType.Contribute:
                        if (Contribute >= value)
                        {
                            ChangeContribute(Convert.ToInt32(-value));
                            paid = true;
                        }
                        break;
                    case CurrencyType.RewardPoolCurrency:
                        if (Character.RewardPoolCoin >= value)
                        {
                            SEnvir.AdjustPersonalReward(Character, -value, CurrencySource.RedPacketDeduct);
                            paid = true;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(currency), currency, null);
                }

                if (!paid)
                {
                    Connection.ReceiveChat("创建红包失败: 余额不足", MessageType.System);
                    return null;
                }

                var redPacket = SEnvir.RedPacketInfoList.CreateNewObject();
                redPacket.Sender = this.Character;
                redPacket.FaceValue = value;
                redPacket.TotalCount = count;
                redPacket.Currency = currency;
                redPacket.Type = type;
                redPacket.Scope = scope;
                redPacket.RemainingValue = value;
                redPacket.SendTime = SEnvir.Now;
                redPacket.ExpireTime = SEnvir.Now + TimeSpan.FromSeconds(durationInSec);
                if (!string.IsNullOrEmpty(message))
                {
                    redPacket.Message = message;
                }
                return redPacket;
            }

            Connection.ReceiveChat("创建红包失败：功能处于关闭状态", MessageType.System);
            return null;
        }

        /// <summary>
        /// 向客户端更新单个红包的信息
        /// </summary>
        /// <param name="redPacket"></param>
        public void SendRedPacketUpdate(RedPacketInfo redPacket)
        {
            if (redPacket == null) return;
            Enqueue(new S.RedPacketUpdate { RedPacket = redPacket.ToClientInfo() });
        }
        /// <summary>
        /// 向客户端更新最近1天的红包信息
        /// </summary>
        public void SendRecentRedPacketsUpdate()
        {
            Enqueue(new S.RecentRedPackets
            {
                RedPacketList = SEnvir.RedPacketInfoList.Binding.Where(
                    x => x.SendTime + TimeSpan.FromDays(1) > SEnvir.Now).Select(
                    y => y.ToClientInfo()).ToList()
            });
        }
    }
}