using Library;
using Server.Envir;
using System;
using System.Collections.Generic;
using S = Library.Network.ServerPackets;
namespace Server.Models
{
    public partial class PlayerObject : MapObject //玩家对象与地图对象的相关逻辑
    {
        public HashSet<MapObject> VisibleObjects = new HashSet<MapObject>();
        public HashSet<MapObject> VisibleDataObjects = new HashSet<MapObject>();
        public HashSet<MonsterObject> TaggedMonsters = new HashSet<MonsterObject>();
        public HashSet<MapObject> NearByObjects = new HashSet<MapObject>();

        #region Objects View
        public override void AddAllObjects() //添加分配对象
        {
            try
            {
                base.AddAllObjects();

                int minX = Math.Max(0, CurrentLocation.X - Config.MaxViewRange);
                int maxX = Math.Min(CurrentMap.Width - 1, CurrentLocation.X + Config.MaxViewRange);

                for (int i = minX; i <= maxX; i++)
                    foreach (MapObject ob in CurrentMap.OrderedObjects[i])
                    {
                        if (ob.IsNearBy(this))
                            AddNearBy(ob);
                    }

                foreach (MapObject ob in NearByObjects)
                {
                    if (ob.CanBeSeenBy(this))
                        AddObject(ob);

                    if (ob.CanDataBeSeenBy(this))
                        AddDataObject(ob);
                }

                if (Stats[Stat.BossTracker] > 0)
                {
                    foreach (MonsterObject ob in CurrentMap.Bosses)
                    {
                        if (ob.CanDataBeSeenBy(this))
                            AddDataObject(ob);
                    }
                }

                foreach (PlayerObject ob in SEnvir.Players)
                {
                    if (ob.CanDataBeSeenBy(this))
                        AddDataObject(ob);
                }
            }
            catch { }
        }
        public override void RemoveAllObjects()  //删除所有对象
        {
            base.RemoveAllObjects();

            HashSet<MapObject> templist = new HashSet<MapObject>();

            foreach (MapObject ob in VisibleObjects)
            {
                if (ob == null) continue;
                if (ob.CanBeSeenBy(this)) continue;

                templist.Add(ob);
            }
            foreach (MapObject ob in templist)
                RemoveObject(ob);


            templist = new HashSet<MapObject>();
            foreach (MapObject ob in VisibleDataObjects)
            {
                if (ob == null) continue;
                if (ob.CanDataBeSeenBy(this)) continue;

                templist.Add(ob);
            }
            foreach (MapObject ob in templist)
                RemoveDataObject(ob);

            templist = new HashSet<MapObject>();
            foreach (MapObject ob in NearByObjects)
            {
                if (ob.IsNearBy(this)) continue;

                templist.Add(ob);
            }
            foreach (MapObject ob in templist)
                RemoveNearBy(ob);
        }

        public void AddObject(MapObject ob)  //添加对象
        {
            if (ob.SeenByPlayers.Contains(this)) return;

            ob.SeenByPlayers.Add(this);
            VisibleObjects.Add(ob);

            Enqueue(ob.GetInfoPacket(this));
        }
        public void AddNearBy(MapObject ob)  //添加附近
        {
            if (ob.NearByPlayers.Contains(this)) return;

            NearByObjects.Add(ob);
            ob.NearByPlayers.Add(this);

            ob.Activate();
        }
        public void AddDataObject(MapObject ob)  //添加数据对象
        {
            if (ob.DataSeenByPlayers.Contains(this)) return;

            ob.DataSeenByPlayers.Add(this);
            VisibleDataObjects.Add(ob);

            Enqueue(ob.GetDataPacket(this));
        }
        public void RemoveObject(MapObject ob)  //删除对象
        {
            if (!ob.SeenByPlayers.Contains(this)) return;

            ob.SeenByPlayers.Remove(this);
            VisibleObjects.Remove(ob);

            if (ob == NPC)
            {
                NPC = null;
                NPCPage = null;
            }

            if (ob.Race == ObjectType.Spell)
            {
                SpellObject spell = (SpellObject)ob;

                if (spell.Effect == SpellEffect.Rubble)  //挖矿时候的碎石效果BUFF判断
                    PauseBuffs();
            }

            if (ob.Race == ObjectType.Monster)
                foreach (MonsterObject mob in TaggedMonsters)
                {
                    if (mob != ob) continue;

                    mob.EXPOwner = null;
                    break;
                }

            Enqueue(new S.ObjectRemove { ObjectID = ob.ObjectID });
        }
        public void RemoveNearBy(MapObject ob)  //移除附近
        {
            if (!ob.NearByPlayers.Contains(this)) return;

            ob.NearByPlayers.Remove(this);
            NearByObjects.Remove(ob);
        }
        public void RemoveDataObject(MapObject ob)  //删除数据对象
        {
            if (!ob.DataSeenByPlayers.Contains(this)) return;

            ob.DataSeenByPlayers.Remove(this);
            VisibleDataObjects.Remove(ob);

            Enqueue(new S.DataObjectRemove { ObjectID = ob.ObjectID });
        }
        #endregion
    }
}
