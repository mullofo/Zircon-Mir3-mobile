using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;

namespace Server.Models.Monsters
{
    public class TerracottaBoss : MonsterObject   //古墓BOSS古墓主人
    {
        public int AttackRange = 11;
        public bool CanFirstInvincibility, CanSecondInvincibility, CanThirdInvincibility;
        public List<KeyValuePair<string, Point>> BossSpawnPoints = new List<KeyValuePair<string, Point>>();

        protected override void OnSpawned()
        {
            base.OnSpawned();
            CanFirstInvincibility = true;
            CanSecondInvincibility = true;
            CanThirdInvincibility = true;
            BossSpawnPoints.Add(new KeyValuePair<string, Point>("上方", new Point(33, 35)));
            BossSpawnPoints.Add(new KeyValuePair<string, Point>("左侧", new Point(30, 125)));
            BossSpawnPoints.Add(new KeyValuePair<string, Point>("右侧", new Point(120, 30)));
        }

        protected override bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;
            if (Target.CurrentLocation == CurrentLocation) return false;

            return Functions.InRange(CurrentLocation, Target.CurrentLocation, AttackRange);
        }

        protected bool InMeleeAttackRange()
        {
            if (!InAttackRange()) return false;

            int x = Math.Abs(Target.CurrentLocation.X - CurrentLocation.X);
            int y = Math.Abs(Target.CurrentLocation.Y - CurrentLocation.Y);

            if (x > 2 || y > 2) return false;

            return x == 0 || x == y || y == 0;
        }

        public override void ProcessTarget()
        {
            TerracottaBossCheck();
            if (Target == null) return;

            if (CurrentLocation == Target.CurrentLocation)
            {
                MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int d = 0; d < 8; d++)
                {
                    if (Walk(direction)) break;

                    direction = Functions.ShiftDirection(direction, rotation);
                }
            }
            else if (!InMeleeAttackRange())
                MoveTo(Target.CurrentLocation);

            if (InAttackRange() && CanAttack)
                Attack();
        }

        protected override void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            UpdateAttackTime();

            if (!InMeleeAttackRange() || SEnvir.Random.Next(5) > 0)
                LineAoE(12, 1, 1, MagicType.None, Element.Dark);
            else
            {

                Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                foreach (MapObject ob in GetTargets(CurrentMap, Functions.Move(CurrentLocation, Direction, 3), 2))
                {
                    ActionList.Add(new DelayedAction(
                                   SEnvir.Now.AddMilliseconds(400),
                                   ActionType.DelayAttack,
                                   ob,
                                   GetMC(),
                                   AttackElement));
                }
            }
        }

        private void TerracottaBossCheck()
        {
            if (Dead) return;
            if (CurrentMap == null) return;
            if (CurrentMap.GetAliveMonsterCount(389) < 1)
            {
                //当前地图没有小boss 古墓土偶护卫武士
                Stats[Stat.Invincibility] = 0;
            }

            double hpLostPercent = (1d * Stats[Stat.Health] - CurrentHP) / Stats[Stat.Health] * 100d;
            if (hpLostPercent > 40 && CanFirstInvincibility)
            {
                //掉血超过40% 第1次无敌还没用
                Stats[Stat.Invincibility] = 1;
                CanFirstInvincibility = false;
                //刷小BOSS
                KeyValuePair<string, Point> spawnPoint = BossSpawnPoints[SEnvir.Random.Next(BossSpawnPoints.Count)];
                CurrentMap.CreateMon(spawnPoint.Value.X, spawnPoint.Value.Y, 2, "古墓土偶护卫武士", 1);
                CurrentMap.MapMsg($"古墓土偶护卫武士已刷新在：{spawnPoint.Key}", MessageType.Hint);
                BossSpawnPoints.Remove(spawnPoint);

                //开启对应门点
                switch (spawnPoint.Key)
                {
                    case "上方":
                        SEnvir.GetMovementInfo(3251).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3252).ExtraInfo = "开启";
                        break;
                    case "左侧":
                        SEnvir.GetMovementInfo(3253).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3254).ExtraInfo = "开启";
                        break;
                    case "右侧":
                        SEnvir.GetMovementInfo(3255).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3256).ExtraInfo = "开启";
                        break;
                }

            }
            else if (hpLostPercent > 60 && CanSecondInvincibility && !CanFirstInvincibility)
            {
                //掉血超过60% 第2次无敌还没用
                Stats[Stat.Invincibility] = 1;
                CanSecondInvincibility = false;
                //刷小BOSS
                KeyValuePair<string, Point> spawnPoint = BossSpawnPoints[SEnvir.Random.Next(BossSpawnPoints.Count)];
                CurrentMap.CreateMon(spawnPoint.Value.X, spawnPoint.Value.Y, 2, "古墓土偶护卫武士", 1);
                CurrentMap.MapMsg($"古墓土偶护卫武士已刷新在：{spawnPoint.Key}", MessageType.Hint);
                BossSpawnPoints.Remove(spawnPoint);

                //开启对应门点
                switch (spawnPoint.Key)
                {
                    case "上方":
                        SEnvir.GetMovementInfo(3251).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3252).ExtraInfo = "开启";
                        break;
                    case "左侧":
                        SEnvir.GetMovementInfo(3253).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3254).ExtraInfo = "开启";
                        break;
                    case "右侧":
                        SEnvir.GetMovementInfo(3255).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3256).ExtraInfo = "开启";
                        break;
                }
            }
            else if (hpLostPercent > 80 && CanThirdInvincibility && !CanFirstInvincibility && !CanSecondInvincibility)
            {
                //掉血超过80% 第3次无敌还没用
                Stats[Stat.Invincibility] = 1;
                CanThirdInvincibility = false;
                //刷小BOSS
                KeyValuePair<string, Point> spawnPoint = BossSpawnPoints[SEnvir.Random.Next(BossSpawnPoints.Count)];
                CurrentMap.CreateMon(spawnPoint.Value.X, spawnPoint.Value.Y, 2, "古墓土偶护卫武士", 1);
                CurrentMap.MapMsg($"古墓土偶护卫武士已刷新在：{spawnPoint.Key}", MessageType.Hint);
                BossSpawnPoints.Remove(spawnPoint);

                //开启对应门点
                switch (spawnPoint.Key)
                {
                    case "上方":
                        SEnvir.GetMovementInfo(3251).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3252).ExtraInfo = "开启";
                        break;
                    case "左侧":
                        SEnvir.GetMovementInfo(3253).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3254).ExtraInfo = "开启";
                        break;
                    case "右侧":
                        SEnvir.GetMovementInfo(3255).ExtraInfo = "开启";
                        SEnvir.GetMovementInfo(3256).ExtraInfo = "开启";
                        break;
                }
            }

        }

        public override void Die()
        {
            base.Die();

            //重置门点
            SEnvir.GetMovementInfo(3251).ExtraInfo = "关闭";
            SEnvir.GetMovementInfo(3252).ExtraInfo = "关闭";
            SEnvir.GetMovementInfo(3253).ExtraInfo = "关闭";
            SEnvir.GetMovementInfo(3254).ExtraInfo = "关闭";
            SEnvir.GetMovementInfo(3255).ExtraInfo = "关闭";
            SEnvir.GetMovementInfo(3256).ExtraInfo = "关闭";
        }
    }
}
