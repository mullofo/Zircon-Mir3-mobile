using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //一些测试
    {
        //防挂
        public DateTime HackTime;
        public int HackCount;
        //模拟击杀面前的怪物N次 统计掉落
        public void TestDrop(int times, int players, decimal rate)
        {
            if (!Character.Account.TempAdmin) return;
            Point front = Functions.Move(CurrentLocation, Direction, 1);
            Cell cell = CurrentMap.Cells[front.X, front.Y];

            if (cell?.Objects?[0] != null && cell.Objects[0].Race == ObjectType.Monster)
            {
                if (!((cell.Objects[0]) is MonsterObject mob))
                {
                    Connection.ReceiveChat($"怪物为null", MessageType.Global);
                    return;
                }

                mob.EXPOwner = this;

                Dictionary<string, long> totalRewards = new Dictionary<string, long>();
                for (int i = 0; i < times; i++)
                {
                    mob.Drop(this, players, rate, true);
                    if (mob.DropTestDict == null || mob.DropTestDict.Count < 1)
                    {
                        continue;
                    }

                    foreach (var kvp in mob.DropTestDict)
                    {
                        if (!totalRewards.ContainsKey(kvp.Key))
                        {
                            totalRewards[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            totalRewards[kvp.Key] += kvp.Value;
                        }
                    }
                }

                //个人爆率和正常爆率重复计算 这里剔除
                Dictionary<string, long> finalTotalRewards = totalRewards.ToDictionary(entry => entry.Key,
                    entry => entry.Value);

                foreach (var kvp in totalRewards)
                {
                    if (kvp.Key.EndsWith("个人爆率"))
                    {
                        finalTotalRewards[kvp.Key.Substring(0, kvp.Key.IndexOf(' '))] -= kvp.Value;
                    }
                }

                Connection.ReceiveChat($"运行了{times}次模拟", MessageType.Global);
                foreach (var kvp in finalTotalRewards)
                {
                    Connection.ReceiveChat($"{kvp.Key}, 数目 {kvp.Value}", MessageType.Hint);
                }

                finalTotalRewards.Clear();
                totalRewards.Clear();
            }

        }
    }
}
