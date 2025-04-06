using Library;
using Library.SystemModels;
using Server.Envir;
using System;
using System.Collections.Generic;
using System.Drawing;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject //处理宠物跟随
    {
        /// <summary>
        /// 处理宠物跟随
        /// </summary>
        private void ProcessPetsFollow()
        {
            if (Pets != null && Pets.Count > 1) //1个以下宠物不用管
            {
                Point targetPoint = Functions.Move(CurrentLocation, Direction, -1);
                int distanceToOwner = Functions.Distance(CurrentLocation, targetPoint);

                //人物周边8个点 优先考虑身后
                Queue<Point> candidates = new Queue<Point>();
                candidates.Enqueue(targetPoint);
                for (int i = 3; i >= 1; i--)
                {
                    Point p1 = Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, i), distanceToOwner);
                    candidates.Enqueue(p1);
                    Point p2 = Functions.Move(CurrentLocation, Functions.ShiftDirection(Direction, -i), distanceToOwner);
                    candidates.Enqueue(p2);
                }
                //重新移动宠物
                for (int i = Pets.Count - 1; i >= 0; i--)
                {
                    if (Pets[i] == null) continue;
                    MonsterObject pet = Pets[i];
                    if (!pet.CanMove) continue;
                    if (pet.Target != null) continue;

                    Point target = candidates.Dequeue();
                    if (pet.CurrentLocation == target) continue;

                    pet.MoveTo(target);
                }
            }
        }

        public void AddPet(string monsterName, int amount, int seconds)
        {
            MonsterInfo monInfo = SEnvir.GetMonsterInfo(monsterName);
            if (monInfo == null)
            {
                Connection.ReceiveChat($"数据库中不存在此怪物".Lang(Connection.Language) + $": {monsterName}", MessageType.System);
                return;
            }

            for (int i = 0; i < amount; i++)
            {
                MonsterObject monster = MonsterObject.GetMonster(monInfo);  //怪物信息
                if (monster == null) return;   //如果怪物为空 跳开
                monster.SummonLevel = 0;   //怪物等级
                monster.PetOwner = this;    //怪物主人= 玩家
                monster.TameTime = SEnvir.Now.AddSeconds(seconds);  //宠物的时间
                monster.RageTime = DateTime.MinValue;
                monster.ShockTime = DateTime.MinValue;
                monster.Spawn(Character.CurrentMap, CurrentLocation);   //再生  角色当前地图 当前坐标
                Pets.Add(monster);   //宠物增加(怪物名)

                //发送封包 给对应的角色 增加宠物
                monster.Broadcast(new S.ObjectPetOwnerChanged { ObjectID = monster.ObjectID, PetOwner = Name });
            }
        }
    }
}
