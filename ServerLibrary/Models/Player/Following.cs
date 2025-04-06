using Library;
using Server.DBModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //背号 互飞 跟随
    {
        private PlayerObject _master;

        /// <summary>
        /// 跟随我的人(CharacterInfo的index)
        /// </summary>
        public HashSet<int> Followers = new HashSet<int>();
        /// <summary>
        /// 我跟随的人
        /// </summary>
        public PlayerObject Master
        {
            get => _master;
            set => _master = value;
        }

        public CharacterInfo MasterCharacterInfo;

        /// <summary>
        /// 是否处于跟随状态
        /// </summary>
        public bool IsFollowing { get; set; } = false;
        /// <summary>
        /// 传送延迟
        /// </summary>
        public int FollowDelay { get; set; } = 5;
        /// <summary>
        /// 下次传送的时间
        /// </summary>
        public DateTime NextFollowTime { get; set; } = DateTime.MaxValue;
        /// <summary>
        /// 禁止跟随的地图
        /// 当对方的地图在此列表中时，禁止跟随
        /// </summary>
        public HashSet<int> NoFollowMaps = new HashSet<int>();

        public bool SetupFollowing(string masterName, int followDelay, int buffIndex,
            IronPython.Runtime.List noFollowList)
        {
            return SetupFollowing(SEnvir.GetCharacter(masterName), followDelay, buffIndex, noFollowList);
        }
        // 配置跟随
        public bool SetupFollowing(CharacterInfo masterChar, int followDelay, int buffIndex, IronPython.Runtime.List noFollowList)
        {
            if (!Config.EnableFollowing)
            {
                Connection.ReceiveChat($"跟随配置失败: 服务器跟随功能处于关闭状态", MessageType.System);
                return false;
            }

            if (IsFollowing)
            {
                Connection.ReceiveChat($"跟随配置失败: 你已经在跟随了, 跟随对象: {Master.Character.CharacterName}", MessageType.System);
                return false;
            }

            if (IsFishing)
            {
                Connection.ReceiveChat($"跟随配置失败: 钓鱼时禁止跟随", MessageType.System);
                return false;
            }

            if (masterChar == null)
            {
                Connection.ReceiveChat($"跟随配置失败: 跟随对象为空", MessageType.System);
                return false;
            }

            if (masterChar.Friends.All(x => x.Name != Character.CharacterName))
            {
                Connection.ReceiveChat($"跟随配置失败: 跟随对象不是你的好友", MessageType.System);
                return false;
            }

            if (masterChar.Player == null)
            {
                Connection.ReceiveChat($"跟随配置失败: 跟随对象不在线", MessageType.System);
                return false;
            }

            if (!HasCustomBuff(buffIndex))
            {
                Connection.ReceiveChat($"跟随配置失败: 你没有要求的Buff", MessageType.System);
                return false;
            }

            if (followDelay < 1)
            {
                Connection.ReceiveChat($"跟随配置失败: 传送延迟小于1秒", MessageType.System);
                return false;
            }

            if (Followers.Count > 0)
            {
                Connection.ReceiveChat($"跟随配置失败: 有{Followers.Count}个玩家在跟随你，你无法再跟随其他玩家", MessageType.System);
                return false;
            }

            // ok
            // 禁飞地图列表更新
            if (noFollowList != null)
            {
                foreach (var mapIndex in noFollowList)
                {
                    NoFollowMaps.Add((int)mapIndex);
                }
            }

            FollowDelay = followDelay;
            IsFollowing = true;

            // 设置自己的跟随对象
            MasterCharacterInfo = masterChar;
            Connection.ReceiveChat($"跟随配置成功", MessageType.System);

            return true;
        }

        public void StartFollowing()
        {
            if (MasterCharacterInfo == null)
            {
                Connection.ReceiveChat($"跟随失败: 请先设置跟随对象", MessageType.System);
                return;
            }

            if (MasterCharacterInfo?.Player == null)
            {
                Connection.ReceiveChat($"跟随失败: 跟随对象不在线", MessageType.System);
                return;
            }
            Master = MasterCharacterInfo.Player;

            if (!MasterCharacterInfo.AllowFollowing)
            {
                Connection.ReceiveChat($"跟随失败: 跟随对象禁止跟随", MessageType.System);
                return;
            }

            if (Master.Followers.Count >= MasterCharacterInfo.MaxFollower)
            {
                Connection.ReceiveChat($"跟随失败: 对方最多可以有{MasterCharacterInfo.MaxFollower}个跟随者, 已满", MessageType.System);
                return;
            }
            // 防止A背B, B再背C的情况
            if (Master.IsFollowing)
            {
                Connection.ReceiveChat($"跟随失败: 对方在跟随其他玩家", MessageType.System);
                return;
            }

            // 对方添加自己为跟随者
            Master.Followers.Add(Character.Index);
            Connection.ReceiveChat($"即将开始跟随...", MessageType.System);
            NextFollowTime = SEnvir.Now + TimeSpan.FromSeconds(1);
        }
        /// <summary>
        /// 停止跟随别人
        /// </summary>
        public void StopFollowing()
        {
            if (Master.Connection != null)
            {
                // 对方依然在线 
                Master.Connection.ReceiveChat($"{Character.CharacterName} 已经停止跟随你", MessageType.System);
                Master.Followers.Remove(Character.Index);
            }

            Master = null;
            IsFollowing = false;
            NextFollowTime = DateTime.MaxValue;

            Connection.ReceiveChat($"已停止跟随", MessageType.System);
        }

        /// <summary>
        /// 停止某个玩家跟随自己
        /// </summary>
        /// <param name="characterName">角色名字</param>
        public void StopBeingFollowed(string characterName)
        {
            var follower = SEnvir.GetCharacter(characterName);
            if (follower == null) return;
            if (!Followers.Contains(follower.Index)) return;
            if (follower.Player == null) return;

            follower.Player.StopFollowing();
        }

        /// <summary>
        /// 处理跟随
        /// </summary>
        public void ProcessFollowing()
        {
            bool retry = false;

            if (NextFollowTime > SEnvir.Now)
            {
                return;
            }

            if (Dead)
            {
                TownRevive();
            }

            if (Followers.Count > 0)
            {
                // 我是被跟随的
                Connection.ReceiveChat($"当前有{Followers.Count}人跟随你", MessageType.System);
            }
            else
            {
                // 我是跟随别人的
                if (Master?.Connection == null)
                {
                    // 对方不在线
                    Connection.ReceiveChat($"已终止跟随, 对方不在线", MessageType.System);
                    StopFollowing();
                }
                else if (!Master.Character.AllowFollowing)
                {
                    // 对方禁止跟随
                    Connection.ReceiveChat($"已终止跟随, 对方禁止跟随", MessageType.System);
                    StopFollowing();
                }
                else if (Master?.CurrentMap?.Info == null)
                {
                    // 无法获取对方位置
                    Connection.ReceiveChat($"已暂停跟随, 无法获取对方位置, {FollowDelay} 秒后重试", MessageType.System);
                    retry = true;
                }
                else if (NoFollowMaps.Contains(Master.CurrentMap.Info.Index))
                {
                    Connection.ReceiveChat($"已暂停跟随, 对方在限制地图, {FollowDelay} 秒后重试", MessageType.System);
                    retry = true;
                }
                else if (Master?.Dead == true)
                {
                    // 对方死亡
                    Connection.ReceiveChat($"已终止跟随, 对方死亡", MessageType.System);
                    StopFollowing();
                }
                else
                {
                    // 飞过去
                    Teleport(Master.CurrentMap, Master.CurrentLocation);
                    NextFollowTime = SEnvir.Now + TimeSpan.FromSeconds(FollowDelay);
                }
            }

            if (retry)
            {
                // 设置下次传送的时间
                NextFollowTime = SEnvir.Now + TimeSpan.FromSeconds(FollowDelay);
            }
        }
    }
}