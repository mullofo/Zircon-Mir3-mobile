using Library;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Monsters
{
    public sealed class Practitioner : MonsterObject   //练功木桩
    {
        public override bool Blocking => true;
        public override bool CanMove => false;

        public Practitioner()
        {
            NameColour = Color.Red;
        }

        public override void ProcessNameColour()
        {
            NameColour = Color.Red;
        }

        /// <summary>
        /// 被攻击时
        /// </summary>
        /// <param name="ob">目标</param>
        /// <param name="power">伤害</param>
        /// <param name="element">元素</param>
        /// <param name="canReflect">反射攻击</param>
        /// <param name="ignoreShield">忽略魔法盾</param>
        /// <param name="canCrit">暴击</param>
        /// <param name="canStruck">推</param>
        /// <returns></returns>
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker?.Node == null || power == 0 || Dead || attacker.CurrentMap != CurrentMap || !Functions.InRange(attacker.CurrentLocation, CurrentLocation, Config.MaxViewRange) || Stats[Stat.Invincibility] > 0) return 0;

            PlayerObject player;

            switch (attacker.Race)
            {
                case ObjectType.Player:
                    PlayerTagged = true;
                    player = (PlayerObject)attacker;
                    break;
                case ObjectType.Monster:
                    player = ((MonsterObject)attacker).PetOwner;
                    break;
                default:
                    throw new NotImplementedException();
            }

            //伤害计数统计
            if ((Poison & PoisonType.Red) == PoisonType.Red || Stats[Stat.RedPoison] > 0)  //红毒
                power = (int)(power * Config.RedPoisonAttackRate);

            for (int i = 0; i < attacker.Stats[Stat.Rebirth]; i++)   //转生PVE伤害
                power = (int)(power * Config.RebirthPVE);

            if (attacker.Stats[Stat.DieExtraDamage] > 0 && !MonsterInfo.Undead)
                power += attacker.Stats[Stat.DieExtraDamage];  //追加死系伤害

            if (attacker.Stats[Stat.LifeExtraDamage] > 0 && MonsterInfo.Undead)
                power += attacker.Stats[Stat.LifeExtraDamage];  //追加生系伤害

            if (SEnvir.Random.Next(100) < (attacker.Stats[Stat.CriticalChance] + attacker.Stats[Stat.WeponCriticalChance]) && canCrit)  //暴击几率
            {
                power += power * 30 / 100 + power * (attacker.Stats[Stat.CriticalDamage] + attacker.Stats[Stat.WeponCriticalChance]) / 100;
                Critical();  //显示暴击效果
            }
            else if (SEnvir.Random.Next(100) < 30 && attacker.Stats[Stat.CriticalHit] > 0 && !canCrit)  //会心一击
            {
                power += power * attacker.Stats[Stat.CriticalHit] / 100;
                CriticalHit();  //显示会心一击效果
            }

            power += attacker.Stats[Stat.ExtraDamage];  //额外伤害

            if (attacker == null || attacker.Race != ObjectType.Player) return 0;
            player.Connection?.ReceiveChat($"当前攻击值 {power}", MessageType.Hint);
            return 0;
        }
    }
}
