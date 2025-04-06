using Library;
using Server.Envir;
using System;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class VoraciousGhost : MonsterObject   //复活僵尸
    {
        public int DeathCount;  //死亡次数
        public int ReviveCount;  //复活次数
        public DateTime ReviveTime;  //复活时间

        public override decimal Experience => base.Experience / (decimal)Math.Pow(2, DeathCount);  //经验值

        public VoraciousGhost()
        {
            ReviveCount = SEnvir.Random.Next(4);
        }


        public override void Process()
        {
            base.Process();

            if (!Dead || ReviveCount == 0 || SEnvir.Now < ReviveTime) return;

            Broadcast(new S.ObjectShow { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            ActionTime = SEnvir.Now.AddMilliseconds(1500);

            Dead = false;
            SetHP((int)(Stats[Stat.Health] / Math.Pow(2, DeathCount)));
            ReviveCount--;
        }

        public override void Drop(PlayerObject owner, int players, decimal rate, bool isTest = false)   //掉落
        {
            if (ReviveCount != 0) return;

            base.Drop(owner, players, rate);
        }

        public override void Die()   //死亡
        {
            base.Die();

            ReviveTime = SEnvir.Now.AddSeconds(SEnvir.Random.Next(5) + 3);
            DeadTime = ReviveTime.Add(Config.DeadDuration);

            DeathCount++;
        }
    }
}
