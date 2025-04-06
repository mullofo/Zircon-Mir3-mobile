using Library;
using Library.Network;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 木偶傀儡
    /// </summary>
    public sealed class Puppet : MonsterObject
    {
        /// <summary>
        /// 玩家对象定义
        /// </summary>
        public PlayerObject Player;
        /// <summary>
        /// 不能移动
        /// </summary>
        public override bool CanMove => false;
        /// <summary>
        /// 不能攻击
        /// </summary>
        public override bool CanAttack => false;
        /// <summary>
        /// 石头属性
        /// </summary>
        public Stats DarkStoneStats;
        /// <summary>
        /// 延迟消失时间设置
        /// </summary>
        public DateTime ExplodeTime = SEnvir.Now.AddSeconds(5);
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (Player?.Node == null)  //玩家等空 移除对象
            {
                Despawn();
                return;
            }

            //傀儡死亡 或 延迟时间到 或 角色死亡 
            if (Dead || SEnvir.Now < ExplodeTime || Player.Dead) return;

            SetHP(0); //设置傀儡血量为0
        }
        /// <summary>
        /// 目标过程
        /// </summary>
        public override void ProcessTarget()
        {
            if (Target == null) return;  //如果目标为空

            SetHP(0);
        }
        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="attacker">攻击对象</param>
        /// <param name="power">攻击力</param>
        /// <param name="element">元素</param>
        /// <param name="canReflect">是否反射伤害</param>
        /// <param name="ignoreShield">是否破盾</param>
        /// <param name="canCrit">是否暴击</param>
        /// <param name="canStruck">是否冲撞</param>
        /// <returns></returns>
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            int value = base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);

            SetHP(0);

            return value;
        }
        /// <summary>
        /// 搜索过程
        /// </summary>
        public override void ProcessSearch()
        {
        }
        /// <summary>
        /// 已经生成
        /// </summary>
        public override void OnDespawned()
        {
            base.OnDespawned();

            Player?.Pets.Remove(this);
        }
        /// <summary>
        /// 安全生成
        /// </summary>
        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();

            Player?.Pets.Remove(this);
        }
        /// <summary>
        /// 激活
        /// </summary>
        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        /// <summary>
        /// 停止
        /// </summary>
        public override void DeActivate()
        {
            return;
        }
        /// <summary>
        /// 死亡
        /// </summary>
        public override void Die()
        {
            base.Die();

            if (Player?.Node == null) return;  //如果角色为空

            List<MapObject> targets = Player.GetTargets(CurrentMap, CurrentLocation, 2);
            foreach (MapObject target in targets)
            {
                Player.ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(800),
                    ActionType.DelayedMagicDamage,
                    Magics.ToList(),
                    target,
                    Functions.InRange(target.CurrentLocation, CurrentLocation, 1),
                    DarkStoneStats,
                    0));
            }

            Effect effect = Effect.Puppet;

            if (DarkStoneStats.GetAffinityValue(Element.Fire) > 0)
                effect = Effect.PuppetFire;
            else if (DarkStoneStats.GetAffinityValue(Element.Ice) > 0)
                effect = Effect.PuppetIce;
            else if (DarkStoneStats.GetAffinityValue(Element.Lightning) > 0)
                effect = Effect.PuppetLightning;
            else if (DarkStoneStats.GetAffinityValue(Element.Wind) > 0)
                effect = Effect.PuppetWind;

            Broadcast(new S.ObjectEffect { Effect = effect, ObjectID = ObjectID });

            DeadTime = SEnvir.Now.AddSeconds(2);
        }
        /// <summary>
        /// 获取信息封包
        /// </summary>
        /// <param name="ob">玩家对象</param>
        /// <returns></returns>
        public override Packet GetInfoPacket(PlayerObject ob)
        {
            if (Player?.Node == null) return null;

            S.ObjectPlayer packet = (S.ObjectPlayer)Player.GetInfoPacket(null);

            packet.ObjectID = ObjectID;
            packet.Location = CurrentLocation;
            packet.Direction = Direction;
            packet.Dead = Dead;
            packet.Buffs.Remove(BuffType.Cloak);
            packet.Buffs.Remove(BuffType.Transparency);

            return packet;
        }
    }
}
