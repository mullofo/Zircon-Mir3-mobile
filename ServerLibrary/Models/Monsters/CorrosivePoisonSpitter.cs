using Library;
using Server.Envir;
using System;

namespace Server.Models.Monsters
{
    public class CorrosivePoisonSpitter : MonsterObject   //爆毒  触角
    {
        public DateTime TeleportTime { get; set; }

        public CorrosivePoisonSpitter()
        {
            SearchDelay = TimeSpan.FromSeconds(1);
        }
        public override void ProcessSearch()
        {
            base.ProcessSearch();
            if (Target != null && Target.Race == ObjectType.Player && !Functions.InRange(Target.CurrentLocation, CurrentLocation, 16)) Target = null;
        }

        public override void ProcessTarget()
        {
            if (Target == null) return;

            if (!Functions.InRange(Target.CurrentLocation, CurrentLocation, 3) /*&& SEnvir.Now > TeleportTime*/)
            {
                MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
                Cell cell = null;
                for (int i = 0; i < 8; i++)
                {
                    cell = CurrentMap.GetCell(Functions.Move(Target.CurrentLocation, Functions.ShiftDirection(dir, i), 1));

                    if (cell == null || cell.Movements != null)
                    {
                        cell = null;
                        continue;
                    }
                    break;
                }

                if (cell != null)
                {
                    Direction = Functions.DirectionFromPoint(cell.Location, Target.CurrentLocation);
                    Teleport(CurrentMap, cell.Location);

                    TeleportTime = SEnvir.Now.AddSeconds(5);
                }
            }

            base.ProcessTarget();
        }

        public override int Attack(MapObject ob, int power, Element element)
        {
            int result = base.Attack(ob, power, element);

            if (result <= 0 || MonsterInfo.AI == 57) return result;

            if (SEnvir.Random.Next(15) == 0)
            {
                ob.ApplyPoison(new Poison
                {
                    Owner = this,
                    Type = PoisonType.Green,
                    Value = GetMC() / 2,
                    TickFrequency = TimeSpan.FromSeconds(2),
                    TickCount = 7,
                });
            }
            return result;
        }
    }
}
