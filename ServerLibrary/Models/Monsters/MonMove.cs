using Library;
using Server.Envir;
using System.Collections.Generic;
using System.Drawing;

namespace Server.Models.Monsters
{

    public class MonMove : MonsterObject //定义一个巡查型boss
    {
        public int MoveX;
        public int MoveY;

        public List<Point> WayPoints = new List<Point>();
        public int WayIndex;

        public override void DeActivate()
        {
            return;
        }

        public override void ProcessRoam()
        {
            if (SEnvir.Now < RoamTime) return;
            if (WayPoints.Count > WayIndex)
            {
                var location = WayPoints[WayIndex];
                if (location == Point.Empty) return;
                if (CurrentLocation != location)//没有到达目标点
                {
                    RoamTime = SEnvir.Now + RoamDelay;
                    MirDirection direction = Functions.DirectionFromPoint(CurrentLocation, location);
                    int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;
                    for (int d = 0; d < 8; d++)
                    {
                        if (Walk(direction)) return;
                        //direction = Functions.ShiftDirection(direction, rotation);
                    }
                }
                else
                {
                    WayIndex++;
                }
            }
            else
            {
                if (WayIndex > 0)  //到达指定位置 移除对象
                {
                    Despawn();
                }
            }
        }
    }
}
