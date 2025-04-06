using Library;
using Server.Envir;
using System;
using System.Drawing;

namespace Server.Models.Monsters
{
    /// <summary>
    /// 异界之门
    /// </summary>
    public class NetherworldGate : MonsterObject
    {
        /// <summary>
        /// 不能移动
        /// </summary>
        public override bool CanMove => false;
        /// <summary>
        /// 不能攻击
        /// </summary>
        public override bool CanAttack => false;
        /// <summary>
        /// 消失时间
        /// </summary>
        public DateTime DespawnTime;

        /// <summary>
        /// 怪物刷新朝向
        /// </summary>
        public NetherworldGate()
        {
            Direction = MirDirection.Up;
        }
        /// <summary>
        /// 名字颜色
        /// </summary>
        public override void ProcessNameColour()
        {
            NameColour = Color.Lime;
        }
        /// <summary>
        /// 刷新时
        /// </summary>
        protected override void OnSpawned()
        {
            base.OnSpawned();

            DespawnTime = SEnvir.Now.AddMinutes(20);  //消失时间20分钟

            foreach (SConnection con in SEnvir.Connections)
                con.ReceiveChat("Monster.NetherGateOpen".Lang(con.Language, CurrentMap.Info.Description, CurrentLocation.X, CurrentLocation.Y), MessageType.Global);

            SEnvir.NetherworldLocation = CurrentLocation;
            SEnvir.NetherworldCloseTime = DespawnTime;
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (SEnvir.Now >= DespawnTime)  //如果系统时间大于等于消失时间
            {
                if (SpawnInfo != null)   //怪物信息不为空
                    SpawnInfo.AliveCount--;  //活着的减

                foreach (SConnection con in SEnvir.Connections)
                    con.ReceiveChat("Monster.NetherGateClosed".Lang(con.Language), MessageType.System);

                SpawnInfo = null;
                Despawn();
                return;
            }

            if (SEnvir.Now >= SearchTime && SEnvir.MysteryShipMapRegion != null && SEnvir.MysteryShipMapRegion.PointList.Count > 0)
            {
                SearchTime = SEnvir.Now.AddSeconds(3);
                Map map = SEnvir.GetMap(SEnvir.MysteryShipMapRegion.Map);

                if (map == null)
                {
                    SearchTime = SEnvir.Now.AddSeconds(60);
                    return;
                }

                for (int i = CurrentMap.Objects.Count - 1; i >= 0; i--)
                {
                    MapObject ob = CurrentMap.Objects[i];

                    if (ob == this) continue;

                    if (ob is Guard) continue;

                    switch (ob.Race)
                    {
                        case ObjectType.Player:
                            //case ObjectType.Monster:
                            if (ob.InSafeZone) continue;

                            if (ob.Dead || !Functions.InRange(ob.CurrentLocation, CurrentLocation, MonsterInfo.ViewRange)) continue;

                            ob.Teleport(map, SEnvir.MysteryShipMapRegion.PointList[SEnvir.Random.Next(SEnvir.MysteryShipMapRegion.PointList.Count)]);
                            break;
                        default:
                            continue;
                    }
                }
            }
        }

        public override void ProcessSearch()
        {
        }

        public override void Activate()
        {
            if (Activated) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        public override void DeActivate()
        {
            return;
        }

        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            return 0;
        }
        public override bool ApplyPoison(Poison p)
        {
            return false;
        }
    }
}
