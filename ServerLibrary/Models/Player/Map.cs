using Library;
using Library.SystemModels;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    public partial class PlayerObject : MapObject // 地图相关
    {
        public override Point CurrentLocation   //当前位置
        {
            get { return Character.CurrentLocation; }
            set { Character.CurrentLocation = value; }
        }

        public override void OnSafeDespawn()  //离开安全区
        {
            throw new NotImplementedException();
        }

        public bool TeleportByMapIndex(int MapIndex, int dstx, int dsty)  //地图索引传送
        {
            MapInfo destInfo = SEnvir.MapInfoList.Binding.FirstOrDefault(x => x.Index == MapIndex);
            Map destMap = null;

            if (destInfo == null)
            {
                foreach (KeyValuePair<MapInfo, Map> kv in SEnvir.Maps)
                {
                    if (kv.Key.fubenIndex == MapIndex)
                    {
                        destMap = kv.Value;
                        break;
                    }

                }
                if (destMap == null) return false;
            }
            else
            {
                destMap = SEnvir.GetMap(destInfo);
            }

            if (!Teleport(destMap, new Point(dstx, dsty))) return false;
            return true;
        }

        public bool Teleport(Map map, int x, int y)   //传送
        {
            return Teleport(map, new Point(x, y));
        }

        public override bool Teleport(Map map, Point location, bool leaveEffect = true)
        {
            bool res = base.Teleport(map, location, leaveEffect);

            if (res)
            {
                BuffRemove(BuffType.Cloak);
                //BuffRemove(BuffType.Transparency);
                Companion?.Recall();
            }

            if (map.Info.AttackMode != AttackMode.Peace)
            {
                Character.AttackMode = map.Info.AttackMode;
                Enqueue(new S.ChangeAttackMode { Mode = map.Info.AttackMode });
            }

            return res;
        }
        //传送
        public void TeleportRing(Point location, int MapIndex)  //传送戒指
        {
            MapInfo destInfo = SEnvir.MapInfoList.Binding.FirstOrDefault(x => x.Index == MapIndex);

            if (destInfo == null) return;

            if (Dead) return;   //如果 死亡 返回

            //动态地图不允许传送
            if (destInfo.IsDynamic) return;

            if (!Character.Account.TempAdmin)
            {
                if (!Config.TestServer && Stats[Stat.TeleportRing] == 0) return;

                if (Config.TeleportIimit)  //传送地图限制
                {
                    if (!CurrentMap.Info.AllowRT || !CurrentMap.Info.AllowTT || CurrentMap.Info.SkillDelay > 0)
                    {
                        Connection.ReceiveChat("PlayerObject.TeleportIimit".Lang(Connection.Language), MessageType.Hint);
                        return;
                    }
                }

                if (!destInfo.AllowRT || !destInfo.AllowTT) return;

                if (SEnvir.Now < TeleportTime)
                {
                    Connection.ReceiveChat("PlayerObject.TeleportTime".Lang(Connection.Language), MessageType.Hint);
                    return;
                }

                TeleportTime = SEnvir.Now.AddSeconds(Config.TeleportTime);
            }

            Map destMap = SEnvir.GetMap(destInfo);

            if (!Teleport(destMap, destMap.GetRandomLocation(location, 10, 25))) return;

            TeleportTime = SEnvir.Now.AddMinutes(Config.TeleportRingCooling);//延时默认1分钟
        }

        public Map CreateMap(int MapIndex)
        {
            Map map = null;
            MapInfo destInfo = SEnvir.MapInfoList.Binding.FirstOrDefault(x => x.Index == MapIndex);
            MapInfo mapinfo = destInfo.Clone() as MapInfo;
            mapinfo.fubenIndex++;
            map = new Map(mapinfo);
            SEnvir.Maps[mapinfo] = map;
            map.Load();
            map.OwnPlay = this;
            map.Setup();
            Parallel.ForEach(SEnvir.MapRegionList.Binding, x =>
            {
                if (x.Map.Index != MapIndex || x.PointList != null) return;

                x.CreatePoints(map.Width);
            });
            foreach (RespawnInfo info in SEnvir.RespawnInfoList.Binding)
            {
                if (info.Region != null)
                {
                    if (info.Region.Map.Index == MapIndex)
                    {
                        for (int i = 0; i < info.Count; i++)
                        {
                            MonsterObject mob = MonsterObject.GetMonster(info.Monster);
                            MapRegion region = info.Region;
                            if (region.PointList.Count == 0) continue;

                            for (int j = 0; j < 20; j++)
                                if (mob.Spawn(mapinfo, region.PointList[SEnvir.Random.Next(region.PointList.Count)])) break;

                        }
                    }
                }
                else if (info.MapID > 0)
                {
                    if (info.MapID == MapIndex)
                    {
                        for (int i = 0; i < info.Count; i++)
                        {
                            MonsterObject mob = MonsterObject.GetMonster(info.Monster);

                            for (int j = 0; j < 20; j++)
                                if (mob.Spawn(MapIndex, info.MapX, info.MapY, info.Range)) break;
                        }
                    }
                }
            }

            foreach (NPCInfo info in SEnvir.NPCInfoList.Binding)
            {
                if (info.Region == null) continue;
                if (info.Region.Map.Index == MapIndex)
                {
                    try
                    {
                        NPCObject ob = new NPCObject
                        {
                            NPCInfo = info,

                        };
                        //ob.LoadScript();

                        MapRegion region = info.Region;
                        if (region.PointList.Count == 0) continue;

                        for (int j = 0; j < 20; j++)
                            if (ob.Spawn(mapinfo, region.PointList[SEnvir.Random.Next(region.PointList.Count)])) break;
                    }
                    catch (Exception ex)
                    {
                        SEnvir.Log(ex.ToString());
                    }
                }

            }
            SEnvir.FubenMaps.Add(map);
            return map;

        }

        protected override void OnMapChanged()  //地图上的更改
        {
            base.OnMapChanged();

            if (CurrentMap == null) return;

            Character.CurrentMap = CurrentMap.Info;

            if (!Spawned) return;

            for (int i = SpellList.Count - 1; i >= 0; i--)
                if (SpellList[i].CurrentMap != CurrentMap)
                    SpellList[i].Despawn();

            Enqueue(new S.MapChanged
            {
                //OnOff = CurrentMap.MapTime > SEnvir.Now,
                //Remaining = CurrentMap.MapTime - SEnvir.Now, 
                MapIndex = CurrentMap.Info.Index
            });
            Enqueue(new S.MapTime
            {
                OnOff = CurrentMap.MapTime > SEnvir.Now,
                MapRemaining = CurrentMap.MapTime - SEnvir.Now,
                ExpiryOnff = CurrentMap.Expiry > SEnvir.Now,
                ExpiryRemaining = CurrentMap.Expiry - SEnvir.Now,
            });

            if (!CurrentMap.Info.CanHorse)
                RemoveMount();

            ApplyMapBuff();
        }

        protected override void OnLocationChanged()  //位置改变时
        {
            base.OnLocationChanged();

            TradeClose();

            if (CurrentCell == null) return;

            if (Companion != null)
                Companion.SearchTime = DateTime.MinValue;

            for (int i = SpellList.Count - 1; i >= 0; i--)
                if (SpellList[i].CurrentMap != CurrentMap) // || !Functions.InRange(SpellList[i].DisplayLocation, CurrentLocation, Config.MaxViewRange)) //技能出视野就清除效果
                    SpellList[i].Despawn();

            if (CurrentCell.SafeZone != null && CurrentCell.SafeZone.ValidBindPoints.Count > 0 && Stats[Stat.PKPoint] < Config.RedPoint)
                Character.BindPoint = CurrentCell.SafeZone;

            if (InSafeZone != (CurrentCell.SafeZone != null))
            {
                InSafeZone = CurrentCell.SafeZone != null;

                if (!Spawned) return;

                Enqueue(new S.SafeZoneChanged { InSafeZone = InSafeZone });

                PauseBuffs();  //安全区BUFF判断
            }
            else if (Spawned && CurrentMap.Info.CanMine)
                PauseBuffs();  //挖矿时候BUFF判断

            //TODO 判断当前地图是否为沙巴克
            //if (CurrentMap.Info.Index == 25)
            //{
            //    ////检查当天有没有攻城，没有直接return
            //    var canWar = SEnvir.UserConquestList.Binding.Any(p => p.WarDate.Date == SEnvir.Now.Date);
            //    if (!canWar) return;

            //    CastleInfo castle = SEnvir.CastleInfoList.Binding.FirstOrDefault(x => x.Index == 1);
            //    if (castle == null) return;   //如果城堡信息为空 跳开

            //  //  ConquestWar war = SEnvir.ConquestWars.FirstOrDefault(x => x.info == castle);

            //   // if (war == null) return;  //如果攻城对象列表为空 跳开
            //    //没有攻城 时间大于等于指定攻城时间 时间小于结束时间 开启攻城
            //    if (SEnvir.Now.TimeOfDay < castle.StartTime && SEnvir.Now.TimeOfDay.Add(TimeSpan.FromHours(1)) >= castle.StartTime)
            //    {
            //        InSafeZone = true;
            //    }
            //}
        }
    }
}
