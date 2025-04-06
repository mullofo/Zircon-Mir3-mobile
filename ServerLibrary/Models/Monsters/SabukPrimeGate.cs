using Library;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 沙巴克城主门AI
    /// </summary>
    public class SabukPrimeGate : MonsterObject
    {
        /// <summary>
        /// 不能移动
        /// </summary>
        public override bool CanMove => false;
        public override bool CanAttack => false;
        public MirDirection CurrentDir = MirDirection.Up;
        public Point DoorBlock1, DoorBlock2;
        /// <summary>
        /// 怪物刷新朝向
        /// </summary>
        public SabukPrimeGate()
        {
            Direction = MirDirection.Up;
        }

        public override void Die()
        {
            base.Die();
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (Dead || Direction == MirDirection.UpLeft) return 0;
            if (attacker != null)
            {
                if (attacker.Race == ObjectType.Player)
                {
                    if ((attacker as PlayerObject).Character.Account.GuildMember != null && (attacker as PlayerObject).Character.Account.GuildMember.Guild.Castle != null) return 0;   //城堡信息不为空
                }
                else if (attacker.Race == ObjectType.Monster)
                {
                    if ((attacker as MonsterObject).PetOwner != null && (attacker as MonsterObject).PetOwner.Character.Account.GuildMember != null && (attacker as MonsterObject).PetOwner.Character.Account.GuildMember.Guild.Castle != null) return 0;
                }
            }

            double hpLostPercent = (1d * CurrentHP) / Stats[Stat.Health] * 100d;
            if (hpLostPercent < 25)
            {
                Direction = MirDirection.DownRight;
                if (CurrentDir != MirDirection.DownRight)
                {
                    CurrentDir = MirDirection.DownRight;
                    Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                }
            }

            else if (hpLostPercent < 50)
            {
                Direction = MirDirection.Right;
                if (CurrentDir != MirDirection.Right)
                {
                    CurrentDir = MirDirection.Right;
                    Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                }
            }

            else if (hpLostPercent < 75)
            {
                Direction = MirDirection.UpRight;
                if (CurrentDir != MirDirection.UpRight)
                {
                    CurrentDir = MirDirection.UpRight;
                    Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                }
            }
            else
            {
                Direction = MirDirection.Up;
                if (CurrentDir != MirDirection.Up)
                {
                    CurrentDir = MirDirection.Up;
                    Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
                }
            }
            if (attacker.Race == ObjectType.Player || (attacker.Race == ObjectType.Monster && ((attacker as MonsterObject).PetOwner != null && (attacker as MonsterObject).MonsterInfo.AI != 146)))
                return base.Attacked(attacker, 1, element, canReflect, ignoreShield, canCrit, canStruck);
            int damage = base.Attacked(attacker, power, element, canReflect, ignoreShield, canCrit, canStruck);
            return damage;
        }
        public void Open()
        {
            Direction = MirDirection.UpLeft;
            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
            //Broadcast(new S.ObjectEffect { ObjectID = ObjectID, Effect = Effect.TeleportIn });
        }
        public void Close()
        {

        }
        public override void ProcessRegen()
        {

        }
    }
}
