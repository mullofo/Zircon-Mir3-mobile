using Library;
using Server.Envir;

namespace Server.Models.Monsters
{
    public class ZumaKing : ZumaGuardian   //祖玛教主
    {
        public int MaxStage = 7;
        public int Stage;

        public ZumaKing()
        {
            AvoidFireWall = false;  //躲避火墙
        }


        protected override void OnSpawned()  //在生成时
        {
            base.OnSpawned();

            Stage = MaxStage;
        }

        public override void Wake()  //唤醒
        {
            base.Wake();

            ActionTime = SEnvir.Now.AddSeconds(2);
        }

        public override void Process()  //过程
        {
            base.Process();

            if (Dead) return;

            if (CurrentHP * MaxStage / Stats[Stat.Health] >= Stage || Stage <= 0) return;

            Stage--;
            SpawnMinions(4, 8, Target);
        }

        public override void ProcessTarget()  //目标过程
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                if (CanAttack)
                {
                    if (SEnvir.Random.Next(5) == 0)
                        RangeAttack();
                }


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
                else
                    MoveTo(Target.CurrentLocation);
            }

            if (!CanAttack) return;

            if (SEnvir.Random.Next(5) > 0)
            {
                if (InAttackRange())
                    Attack();
            }
            else RangeAttack();
        }

        public void RangeAttack()  //远程攻击
        {
            switch (SEnvir.Random.Next(3))
            {
                case 0:
                    FireWall();
                    break;
                default:
                    LineAoE(12, -2, 2, MagicType.MonsterScortchedEarth, Element.Fire);
                    break;
            }
        }
    }
}
