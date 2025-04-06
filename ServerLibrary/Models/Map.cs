using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Server.Envir;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using S = Library.Network.ServerPackets;

namespace Server.Models
{
    /// <summary>
    /// 地图
    /// </summary>
    public class Map
    {
        /*
        delegate void ScriptOnCreate(Map map);
        delegate void ScriptOnEnter(Map map, PlayerObject sender);
        delegate void ScriptOnLeave(Map map, PlayerObject sender);
        

        public ScriptSource NpcSource;
        public ScriptScope Npcscope;
        public Func<object, object> OnCreate;
        public Func<object, object, object> OnEnter;
        public Func<object, object, object> OnLeave;
        */
        /// <summary>
        /// 地图信息
        /// </summary>
        public MapInfo Info { get; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; private set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; private set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime Expiry
        {
            get { return _Expiry; }
            set
            {
                if (_Expiry == value) return;

                var oldValue = _Expiry;
                _Expiry = value;

                OnExpirChanged(oldValue, value);
            }
        }
        private DateTime _Expiry;
        public void OnExpirChanged(DateTime oValue, DateTime nValue)
        {
            Broadcast(new S.MapTime
            {
                OnOff = MapTime > SEnvir.Now,
                MapRemaining = MapTime - SEnvir.Now,
                ExpiryOnff = nValue > SEnvir.Now,
                ExpiryRemaining = nValue - SEnvir.Now,
            });
        }
        /// <summary>
        /// 地图时间
        /// </summary>
        public DateTime MapTime
        {
            get { return _MapTime; }
            set
            {
                if (_MapTime == value) return;

                var oldValue = _MapTime;
                _MapTime = value;

                OnMapTimeChanged(oldValue, value);
            }
        }
        private DateTime _MapTime;

        public void OnMapTimeChanged(DateTime oValue, DateTime nValue)
        {
            Broadcast(new S.MapTime
            {
                OnOff = nValue > SEnvir.Now,
                MapRemaining = nValue - SEnvir.Now,
                ExpiryOnff = Expiry > SEnvir.Now,
                ExpiryRemaining = Expiry - SEnvir.Now,
            });
        }
        /// <summary>
        /// 是否安全区
        /// </summary>
        public bool HasSafeZone { get; set; }
        /// <summary>
        /// 地图单元格
        /// </summary>
        public Cell[,] Cells { get; private set; }
        /// <summary>
        /// 是否钓鱼点
        /// </summary>
        public bool[,] FishingCells { get; private set; }
        /// <summary>
        /// 有效单元格
        /// </summary>
        public List<Cell> ValidCells { get; } = new List<Cell>();
        /// <summary>
        /// 地图对象列表
        /// </summary>
        public List<MapObject> Objects { get; } = new List<MapObject>();
        /// <summary>
        /// 玩家对象列表
        /// </summary>
        public List<PlayerObject> Players { get; } = new List<PlayerObject>();


        public List<MonsterObject> Pets { get; } = new List<MonsterObject>();
        /// <summary>
        /// BOSS对象列表
        /// </summary>
        public List<MonsterObject> Bosses { get; } = new List<MonsterObject>();
        /// <summary>
        /// NPC对象列表
        /// </summary>
        public List<NPCObject> NPCs { get; } = new List<NPCObject>();
        /// <summary>
        /// 组织对象
        /// </summary>
        public HashSet<MapObject>[] OrderedObjects;
        /// <summary>
        /// 最后过程时间
        /// </summary>
        public DateTime LastProcess;
        /// <summary>
        /// 万圣节活动时间
        /// </summary>
        public DateTime HalloweenEventTime;
        /// <summary>
        /// 圣诞节活动时间
        /// </summary>
        public DateTime ChristmasEventTime;
        /// <summary>
        /// 本人玩家对象
        /// </summary>
        public PlayerObject OwnPlay;
        /// <summary>
        /// 临时值
        /// </summary>
        public Dictionary<int, object> TempValue = new Dictionary<int, object>();
        /// <summary>
        /// 怪物数量
        /// </summary>
        public int MonsterCount
        {
            get
            {
                int count = 0;
                foreach (MapObject ob in Objects)
                {
                    Companion com = ob as Companion;
                    if (com != null) continue;
                    MonsterObject mob = ob as MonsterObject;

                    if (mob == null) continue;

                    if (mob.PetOwner != null) continue;
                    if (mob.Dead) continue;
                    count++;
                }
                return count;
            }
        }
        /// <summary>
        /// 玩家数量
        /// </summary>
        public int PlayerCount
        {
            get
            {
                return Players.Count;
            }
        }

        /*
        public void LoadScript()
        {
            try
            {
                if (!string.IsNullOrEmpty(Info.Script))
                {
                    NpcSource = SEnvir.engine.CreateScriptSourceFromFile("./Scripts" + Info.Script);
                    Npcscope = SEnvir.engine.CreateScope();

                    Npcscope.SetVariable("SEnvir_MapInfoList", SEnvir.MapInfoList);
                    Npcscope.SetVariable("SEnvir_CreateMap", (Func<int, Map>)SEnvir.CreateMap);
                    Npcscope.SetVariable("SEnvir_CloseMap", (Func<MapInfo, int>)SEnvir.CloseMap);
                    NpcSource.Execute(Npcscope);
                    Npcscope.TryGetVariable("OnCreate", out OnCreate);
                    Npcscope.TryGetVariable("OnEnter", out OnEnter);
                    Npcscope.TryGetVariable("OnLeave", out OnLeave);
                }
            }
            catch (Exception ex)
            {
                SEnvir.Log(ex.ToString());
            }
        }
        */
        /// <summary>
        /// 地图信息
        /// </summary>
        /// <param name="info"></param>
        public Map(MapInfo info)
        {
            Info = info;
            //LoadScript();
        }

        #region 传奇2和传奇3的地图支持
        /// <summary>
        /// 读取地图
        /// </summary>
        public void Load()
        {
            string fileName = Path.Combine(AppContext.BaseDirectory, $"{Config.MapPath}{Info.FileName}.map");

            if (!File.Exists(fileName))
            {
                SEnvir.Log($"Map: {fileName} 找不到.");
                return;
            }

            byte[] fileBytes = File.ReadAllBytes(fileName);

            #region 吉米原版
            /*
            Width = fileBytes[23] << 8 | fileBytes[22];
            Height = fileBytes[25] << 8 | fileBytes[24];

            Cells = new Cell[Width, Height];

            int offSet = 28 + Width * Height / 4 * 3;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    byte flag = fileBytes[offSet + (x * Height + y) * 14];

                    if ((flag & 0x02) != 2 || (flag & 0x01) != 1) continue;

                    ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
            */
            #endregion

            switch (FindType(fileBytes))
            {
                case 0:
                    LoadMapCellsv0(fileBytes);
                    break;
                case 1:
                    LoadMapCellsv1(fileBytes);
                    break;
                case 2:
                    LoadMapCellsv2(fileBytes);
                    break;
                case 3:
                    LoadMapCellsv3(fileBytes);
                    break;
                case 4:
                    LoadMapCellsv4(fileBytes);
                    break;
                case 5:
                    LoadMapCellsv5(fileBytes);
                    break;
                case 6:
                    LoadMapCellsv6(fileBytes);
                    break;
                case 7:
                    LoadMapCellsv7(fileBytes);
                    break;
                case 8:
                    LoadMapCellsv8(fileBytes);
                    break;
                case 100:
                    LoadMapCellsV100(fileBytes);
                    break;
            }

            OrderedObjects = new HashSet<MapObject>[Width];
            for (int i = 0; i < OrderedObjects.Length; i++)
                OrderedObjects[i] = new HashSet<MapObject>();
        }
        /// <summary>
        /// 查找类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private byte FindType(byte[] input)
        {
            //c#自定义地图格式 title:
            if ((input[2] == 0x43) && (input[3] == 0x23))
                return 100;

            //韩版传奇3 title:
            if (input[0] == 0)
                return 5;

            //盛大传奇3 title: (C) SNDA, MIR3.
            if ((input[0] == 0x0F) && (input[5] == 0x53) && (input[14] == 0x33))
                return 6;

            //应该是盛大传奇3第二种格式，未知？无参考
            if (input[0] == 0x0F && input[5] == 0x4D && input[14] == 0x33)
                return 8;

            //wemades antihack map (laby maps) title start with: Mir2 AntiHack
            if ((input[0] == 0x15) && (input[4] == 0x32) && (input[6] == 0x41) && (input[19] == 0x31))
                return 4;

            //wemades 2010 map format i guess title starts with: Map 2010 Ver 1.0
            if ((input[0] == 0x10) && (input[2] == 0x61) && (input[7] == 0x31) && (input[14] == 0x31))
                return 1;

            //shanda's 2012 format and one of shandas(wemades) older formats share same header info, only difference is the filesize
            if ((input[4] == 0x0F) && (input[18] == 0x0D) && (input[19] == 0x0A))
            {
                int W = input[0] + (input[1] << 8);
                int H = input[2] + (input[3] << 8);
                if (input.Length > (52 + (W * H * 14)))
                    return 3;
                else
                    return 2;
            }

            //3/4 heroes map format (myth/lifcos i guess)
            if ((input[0] == 0x0D) && (input[1] == 0x4C) && (input[7] == 0x20) && (input[11] == 0x6D))
                return 7;

            //if it's none of the above load the default old school format
            return 0;
        }

        private void LoadMapCellsv0(byte[] fileBytes)
        {
            int offSet = 0;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 52;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 12
                    bool flag = true;

                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 2;
                    //if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                    //    flag = false;

                    offSet += 2;

                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 8;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv1(byte[] fileBytes)
        {
            int offSet = 21;
            int w = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            int xor = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            int h = BitConverter.ToInt16(fileBytes, offSet);
            Width = w ^ xor;
            Height = h ^ xor;
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 54;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    bool flag = true;
                    if (((BitConverter.ToInt32(fileBytes, offSet) ^ 0xAA38AA38) & 0x20000000) != 0)
                        flag = false;

                    offSet += 6;
                    if (((BitConverter.ToInt16(fileBytes, offSet) ^ xor) & 0x8000) != 0)
                        flag = false;

                    offSet += 9;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv2(byte[] fileBytes)
        {
            int offSet = 0;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 52;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 14
                    bool flag = true;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 2;
                    //if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                    //    flag = false;

                    offSet += 2;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 10;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv3(byte[] fileBytes)
        {
            int offSet = 0;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 52;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 36
                    bool flag = true;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 2;
                    //if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                    //    flag = false;

                    offSet += 2;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 32;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv4(byte[] fileBytes)
        {
            int offSet = 31;
            int w = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            int xor = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            int h = BitConverter.ToInt16(fileBytes, offSet);
            Width = w ^ xor;
            Height = h ^ xor;
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 64;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 12
                    bool flag = true;
                    if (((BitConverter.ToInt16(fileBytes, offSet) ^ xor) & 0x8000) != 0)
                        flag = false;

                    offSet += 2;
                    if (((BitConverter.ToInt16(fileBytes, offSet) ^ xor) & 0x8000) != 0)
                        flag = false;

                    offSet += 10;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv5(byte[] fileBytes)
        {
            int offSet = 22;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 28 + (3 * ((Width / 2) + (Width % 2)) * (Height / 2));
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 14
                    bool flag = true;

                    if ((fileBytes[offSet] & 0x01) != 1)
                        flag = false;
                    else if ((fileBytes[offSet] & 0x02) != 2)
                        flag = false;

                    offSet += 14;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv6(byte[] fileBytes)
        {
            int offSet = 16;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 40;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 20
                    bool flag = true;

                    if ((fileBytes[offSet] & 0x01) != 1)
                        flag = false;
                    else if ((fileBytes[offSet] & 0x02) != 2)
                        flag = false;

                    offSet += 20;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv7(byte[] fileBytes)
        {
            int offSet = 21;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 4;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 54;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 15
                    bool flag = true;

                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;
                    offSet += 6;
                    if ((BitConverter.ToInt16(fileBytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 9;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsv8(byte[] fileBytes)
        {
            int offSet = 16;
            Width = BitConverter.ToInt16(fileBytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(fileBytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 40;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {//total 20
                    bool flag = true;

                    if ((fileBytes[offSet] & 0x01) != 1)
                        flag = false;
                    else if ((fileBytes[offSet] & 0x02) != 2)
                        flag = false;

                    offSet += 20;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }
        }

        private void LoadMapCellsV100(byte[] Bytes)
        {
            int offSet = 4;
            if ((Bytes[0] != 1) || (Bytes[1] != 0)) return;//only support version 1 atm
            Width = BitConverter.ToInt16(Bytes, offSet);
            offSet += 2;
            Height = BitConverter.ToInt16(Bytes, offSet);
            Cells = new Cell[Width, Height];
            FishingCells = new bool[Width, Height];
            offSet = 8;

            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    bool flag = true;
                    offSet += 2;
                    if ((BitConverter.ToInt32(Bytes, offSet) & 0x20000000) != 0)
                        flag = false;
                    offSet += 10;
                    if ((BitConverter.ToInt16(Bytes, offSet) & 0x8000) != 0)
                        flag = false;

                    offSet += 14;

                    if (flag)
                        ValidCells.Add(Cells[x, y] = new Cell(new Point(x, y)) { Map = this });
                }

        }
        #endregion

        /// <summary>
        /// 地图设置
        /// </summary>
        public void Setup()
        {
            CreateGuards();
            //if (OnCreate == null) return;
            //OnCreate(this);
            try
            {
                dynamic trig_map;
                if (SEnvir.PythonEvent.TryGetValue("MapEvent_trig_map", out trig_map))
                {
                    //var argss = new Tuple<object>(this);
                    PythonTuple args = PythonOps.MakeTuple(new object[] { this, OwnPlay, });
                    SEnvir.ExecutePyWithTimer(trig_map, this.Info.Index, "OnCreate", args);
                    //trig_map(this.Info.Index, "OnCreate", args);
                }
            }
            catch (SyntaxErrorException e)
            {
                string msg = "地图事件（同步错误） : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (SystemExitException e)
            {
                string msg = "地图事件（系统退出） : \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(e);
                SEnvir.Log(string.Format(msg, error));
            }
            catch (Exception ex)
            {
                string msg = "地图事件（加载插件时错误）: \"{0}\"";
                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                string error = eo.FormatException(ex);
                SEnvir.Log(string.Format(msg, error));
            }
        }

        /// <summary>
        /// 创建守卫
        /// </summary>
        private void CreateGuards()
        {
            foreach (GuardInfo info in Info.Guards)
            {
                MonsterObject mob = MonsterObject.GetMonster(info.Monster);
                mob.Direction = info.Direction;

                if (!mob.Spawn(Info, new Point(info.X, info.Y)))
                {
                    //SEnvir.Log($"Failed to spawn Guard Map:{Info.Description}, Location: {info.X}, {info.Y}");
                    SEnvir.Log($"地图事件（守卫生成失败）:{Info.Description}, 位置: {info.X}, {info.Y}");
                    continue;
                }
            }
        }

        /// <summary>
        /// 过程
        /// </summary>
        public void Process() { }
        /// <summary>
        /// 添加地图对象
        /// </summary>
        /// <param name="ob"></param>
        public void AddObject(MapObject ob)
        {
            Objects.Add(ob);

            switch (ob.Race)
            {
                case ObjectType.Player:
                    Players.Add((PlayerObject)ob);

                    PlayerObject player = ob as PlayerObject;

                    //OnEnter?.Invoke(this, player);
                    try
                    {
                        dynamic trig_map;
                        if (SEnvir.PythonEvent.TryGetValue("MapEvent_trig_map", out trig_map))
                        {
                            //var argss = new Tuple<object,object>(this,player);
                            PythonTuple args = PythonOps.MakeTuple(new object[] { this, player, });
                            SEnvir.ExecutePyWithTimer(trig_map, this.Info.Index, "OnEnter", args);
                            //trig_map(this.Info.Index, "OnEnter", args);
                        }
                    }
                    catch (SyntaxErrorException e)
                    {
                        string msg = "地图事件（同步错误） : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    catch (SystemExitException e)
                    {
                        string msg = "地图事件（系统退出） : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    catch (Exception ex)
                    {
                        string msg = "地图事件（加载插件时错误）: \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(ex);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    break;
                case ObjectType.Item:
                    break;
                case ObjectType.NPC:
                    NPCs.Add((NPCObject)ob);
                    break;
                case ObjectType.Spell:
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss)
                        Bosses.Add(mob);
                    break;
            }
        }
        /// <summary>
        /// 移出地图对象
        /// </summary>
        /// <param name="ob"></param>
        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            switch (ob.Race)
            {
                case ObjectType.Player:
                    Players.Remove((PlayerObject)ob);

                    PlayerObject player = ob as PlayerObject;
                    for (int i = 0; i < player.Pets.Count; i++)
                    {
                        Pets.Add(player.Pets[i]);
                    }
                    //OnLeave?.Invoke(this, player);
                    try
                    {
                        dynamic trig_map;
                        if (SEnvir.PythonEvent.TryGetValue("MapEvent_trig_map", out trig_map))
                        {
                            //var argss = new Tuple<object, object>(this, player);
                            PythonTuple args = PythonOps.MakeTuple(new object[] { this, player, });
                            SEnvir.ExecutePyWithTimer(trig_map, this.Info.Index, "OnLeave", args);
                            //trig_map(this.Info.Index, "OnLeave", args);
                        }
                    }
                    catch (SyntaxErrorException e)
                    {
                        string msg = "地图事件（同步错误） : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    catch (SystemExitException e)
                    {
                        string msg = "地图事件（系统退出） : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    catch (Exception ex)
                    {
                        string msg = "地图事件（加载插件时错误）: \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(ex);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    break;
                case ObjectType.Item:
                    break;
                case ObjectType.NPC:
                    NPCs.Remove((NPCObject)ob);
                    break;
                case ObjectType.Spell:
                    break;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;
                    if (mob.MonsterInfo.IsBoss)
                        Bosses.Remove(mob);
                    Pets.Remove(mob);
                    break;
            }
        }
        /// <summary>
        /// 创建NPC名字
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="NpcName"></param>
        public void CreateNpc(int x, int y, string NpcName)
        {
            NPCInfo info = SEnvir.GetNpcInfo(NpcName);  //NPC名字
            NPCObject ob = new NPCObject
            {
                NPCInfo = info,
            };
            //ob.LoadScript();
            ob.Spawn(Info, new Point(x, y));
        }
        /// <summary>
        /// 创建NPC索引
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="NpcIndex"></param>
        public void CreateNpc(int x, int y, int NpcIndex)
        {
            NPCInfo info = SEnvir.GetNpcInfo(NpcIndex); //NPC ID
            NPCObject ob = new NPCObject
            {
                NPCInfo = info,
            };
            //ob.LoadScript();
            ob.Spawn(Info, new Point(x, y));
        }
        /// <summary>
        /// 删除NPC名字
        /// </summary>
        /// <param name="NpcName"></param>
        public void DeleteNpc(string NpcName)
        {
            NPCInfo info = SEnvir.GetNpcInfo(NpcName);  //NPC 名字
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                NPCObject npc = Objects[i] as NPCObject;

                if (npc.NPCInfo.Equals(info))
                {
                    RemoveObject(npc);
                }
            }
        }
        /// <summary>
        /// 删除NPC索引
        /// </summary>
        /// <param name="NpcIndex"></param>
        public void DeleteNpc(int NpcIndex)
        {
            NPCInfo info = SEnvir.GetNpcInfo(NpcIndex);  //NPC ID
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                NPCObject npc = Objects[i] as NPCObject;

                if (npc.NPCInfo.Equals(info))
                {
                    RemoveObject(npc);
                }
            }
        }
        /// <summary>
        /// 创建怪物名字
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <param name="monname"></param>
        /// <param name="count"></param>
        public List<MonsterObject> CreateMon(int x, int y, int range, string monname, int count = 1)
        {
            var monsterInfo = SEnvir.GetMonsterInfo(monname);

            if (monsterInfo == null)
                return null;
            var monsters = new List<MonsterObject>();
            while (count > 0)
            {
                var monster = MonsterObject.GetMonster(monsterInfo);
                for (int i = 0; i < 20; i++)
                {
                    int minn = Math.Max(x - range, 10);
                    int maxn = Math.Min(x + range, Width - 10);
                    int maxnn = Math.Min(y + range, Height - 10);
                    int minnn = Math.Max(y - range, 10);
                    if (minn > maxn)
                    {
                        minn ^= maxn;
                        maxn ^= minn;
                        minn ^= maxn;
                    }
                    if (minnn > maxnn)
                    {
                        minnn ^= maxnn;
                        maxnn ^= minnn;
                        minnn ^= maxnn;
                    }
                    if (monster.Spawn(Info, new Point(SEnvir.Random.Next(minn, maxn), SEnvir.Random.Next(minnn, maxnn)))) break;
                }
                monsters.Add(monster);
                count -= 1;
            }
            return monsters;
        }
        /// <summary>
        /// 创建怪物索引
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="range"></param>
        /// <param name="monindex"></param>
        /// <param name="count"></param>
        public List<MonsterObject> CreateMon(int x, int y, int range, int monindex, int count = 1)
        {
            var monsterInfo = SEnvir.GetMonsterInfo(monindex);

            if (monsterInfo == null)
                return null;
            var monsters = new List<MonsterObject>();
            while (count > 0)
            {
                var monster = MonsterObject.GetMonster(monsterInfo);
                for (int i = 0; i < 20; i++)
                {
                    int minn = Math.Max(x - range, 10);
                    int maxn = Math.Min(x + range, Width - 10);
                    int maxnn = Math.Min(y + range, Height - 10);
                    int minnn = Math.Max(y - range, 10);
                    if (minn > maxn)
                    {
                        minn ^= maxn;
                        maxn ^= minn;
                        minn ^= maxn;
                    }
                    if (minnn > maxnn)
                    {
                        minnn ^= maxnn;
                        maxnn ^= minnn;
                        minnn ^= maxnn;
                    }
                    if (monster.Spawn(Info, new Point(SEnvir.Random.Next(minn, maxn), SEnvir.Random.Next(minnn, maxnn)))) break;
                }
                monsters.Add(monster);
                count -= 1;
            }
            return monsters;
        }
        /// <summary>
        /// 清除全部怪物
        /// </summary>
        public void ClearAllMonsters()
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                if (Objects[i] != null && Objects[i].Race != ObjectType.Monster) continue;

                MonsterObject mob = Objects[i] as MonsterObject;

                if (mob == null) continue;

                if (mob.PetOwner != null) continue;

                if (mob.Dead) continue;

                if (mob.MonsterInfo.AI == -1) continue;  //怪物AI是卫士 忽略

                if (mob.MonsterInfo.AI == -2) continue;  //怪物AI是宠物 忽略

                mob.EXPOwner = null;
                mob.Die();
                mob.Despawn();
            }
        }
        /// <summary>
        /// 清除全部NPC
        /// </summary>
        public void ClearAllNpcs()
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                NPCObject mob = Objects[i] as NPCObject;

                if (mob == null) continue;
                if (mob.Dead) continue;
                mob.Die();
                mob.Despawn();
            }
        }
        /// <summary>
        /// 清除全部玩家
        /// </summary>
        public void ClearAllPlayers()
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                PlayerObject mob = Objects[i] as PlayerObject;

                if (mob == null) continue;
                if (mob.Dead) continue;
                mob.Teleport(SEnvir.Maps[mob.Character.BindPoint.BindRegion.Map], mob.Character.BindPoint.ValidBindPoints[SEnvir.Random.Next(mob.Character.BindPoint.ValidBindPoints.Count)]);

            }
        }
        /// <summary>
        /// 清除全部道具
        /// </summary>
        public void ClearAllItems()  //清理所有道具
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                ItemObject mob = Objects[i] as ItemObject;

                if (mob == null) continue;
                mob.Despawn();
            }
        }
        /// <summary>
        /// 清除地图效果对象
        /// </summary>
        public void ClearSpellObjects()
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                SpellObject mob = Objects[i] as SpellObject;

                if (mob == null) continue;
                mob.Despawn();
            }
        }
        /// <summary>
        /// 清除全部对象
        /// </summary>
        public void ClearAllObjects()
        {
            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                MapObject mapObject = Objects[i];

                //关闭地图 不应该清理宠物
                if (mapObject is MonsterObject mob)
                {
                    if (mob.MonsterInfo.AI == -2) continue;
                    if (mob.PetOwner != null) continue;
                }

                if (mapObject == null) continue;
                if (mapObject.Dead) continue;
                mapObject.Die();
                mapObject.Despawn();
            }
        }
        /// <summary>
        /// 获取地图单元格XY坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Cell GetCell(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height) return null;

            return Cells[x, y];
        }
        /// <summary>
        /// 获取地图单元格坐标
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Cell GetCell(Point location)
        {
            return GetCell(location.X, location.Y);
        }
        /// <summary>
        /// 获取地图单元格范围
        /// </summary>
        /// <param name="location"></param>
        /// <param name="minRadius"></param>
        /// <param name="maxRadius"></param>
        /// <returns></returns>
        public List<Cell> GetCells(Point location, int minRadius, int maxRadius)
        {
            List<Cell> cells = new List<Cell>();

            for (int d = 0; d <= maxRadius; d++)
            {
                for (int y = location.Y - d; y <= location.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= Height) break;

                    for (int x = location.X - d; x <= location.X + d; x += Math.Abs(y - location.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= Width) break;

                        Cell cell = Cells[x, y]; //直接进入已经检查的范围

                        if (cell == null) continue;

                        cells.Add(cell);
                    }
                }
            }
            return cells;
        }

        /// <summary>
        /// 获取随机位置
        /// </summary>
        /// <returns></returns>
        public Point GetRandomLocation()
        {
            return ValidCells.Count > 0 ? ValidCells[SEnvir.Random.Next(ValidCells.Count)].Location : Point.Empty;
        }
        /// <summary>
        /// 获取随机位置 范围
        /// </summary>
        /// <param name="location"></param>
        /// <param name="range"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public Point GetRandomLocation(Point location, int range, int attempts = 25)
        {
            int minX = Math.Max(0, location.X - range);
            int maxX = Math.Min(Width, location.X + range);
            int minY = Math.Max(0, location.Y - range);
            int maxY = Math.Min(Height, location.Y + range);

            for (int i = 0; i < attempts; i++)
            {
                Point test = new Point(SEnvir.Random.Next(Math.Min(minX, maxX), Math.Max(minX, maxX)), SEnvir.Random.Next(Math.Min(minY, maxY), Math.Max(minY, maxY)));

                if (GetCell(test) != null)
                    return test;
            }
            return Point.Empty;
        }
        /// <summary>
        /// 获取随机位置XY坐标
        /// </summary>
        /// <param name="minX"></param>
        /// <param name="maxX"></param>
        /// <param name="minY"></param>
        /// <param name="maxY"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        public Point GetRandomLocation(int minX, int maxX, int minY, int maxY, int attempts = 25)
        {
            for (int i = 0; i < attempts; i++)
            {
                Point test = new Point(SEnvir.Random.Next(minX, maxX), SEnvir.Random.Next(minY, maxY));

                if (GetCell(test) != null)
                    return test;
            }

            return Point.Empty;
        }
        /// <summary>
        /// 发送坐标数据包
        /// </summary>
        /// <param name="location"></param>
        /// <param name="p"></param>
        public void Broadcast(Point location, Packet p)
        {
            foreach (PlayerObject player in Players)
            {
                if (!Functions.InRange(location, player.CurrentLocation, Config.MaxViewRange)) continue;
                player.Enqueue(p);
            }
        }
        /// <summary>
        /// 发送数据包
        /// </summary>
        /// <param name="p"></param>
        public void Broadcast(Packet p)
        {
            foreach (PlayerObject player in Players)
                player.Enqueue(p);
        }
        /// <summary>
        /// 发送地图对话消息
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        public void MapMsg(string msg, MessageType type)
        {
            Broadcast(new S.Chat { Text = msg, Type = type });
        }
        /// <summary>
        /// 创建地图魔法效果事件
        /// </summary>
        /// <param name="nPosX"></param>
        /// <param name="nPosY"></param>
        /// <param name="nMagicId"></param>
        /// <param name="nMagicLv"></param>
        /// <param name="btOverlap"></param>
        /// <param name="nRepeateCnt"></param>
        /// <param name="nDamageType"></param>
        /// <param name="nDir"></param>
        /// <param name="nPercent"></param>
        /// <param name="btAnimal"></param>
        /// <returns></returns>
        public bool CreateMagicEvent(int nPosX, int nPosY, int nMagicId, int nMagicLv, int btOverlap, int nRepeateCnt, int nDamageType, int nDir, int nPercent, int btAnimal)
        {
            return true;
        }

        //获取当前地图存活怪物个数 monIndex为怪物的index
        public int GetAliveMonsterCount(int monIndex)
        {
            int count = 0;
            foreach (MapObject ob in Objects)
            {
                if (ob is MonsterObject mob && !mob.Dead)
                {
                    if (mob.MonsterInfo.Index == monIndex)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
        /// <summary>
        /// 根据怪物名称获取数量 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetAliveMonsterCount(string name)
        {
            int count = 0;
            for (var i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob is MonsterObject mob && !mob.Dead)
                {
                    if (!mob.Dead && mob.MonsterInfo.MonsterName == name)
                    {
                        count++;
                    }
                }
            }

            return count;
        }
    }
    /// <summary>
    /// 怪物刷新信息
    /// </summary>
    public class SpawnInfo
    {
        /// <summary>
        /// 刷新的怪物信息
        /// </summary>
        public RespawnInfo Info;
        /// <summary>
        /// 当前地图
        /// </summary>
        public Map CurrentMap;
        /// <summary>
        /// 怪物刚创建时间
        /// </summary>
        public DateTime SpawnTime;
        /// <summary>
        /// 下一次刷新时间
        /// </summary>
        public DateTime NextSpawn;
        /// <summary>
        /// 活的怪物计数
        /// </summary>
        public int AliveCount;
        /// <summary>
        /// 最后一次检查时间
        /// </summary>
        public DateTime LastCheck;
        /// <summary>
        /// 使用MapID刷怪
        /// </summary>
        public bool UseMapID;
        /// <summary>
        /// 刷新怪物信息
        /// </summary>
        /// <param name="info"></param>
        public SpawnInfo(RespawnInfo info, bool useMapID)
        {
            Info = info;
            UseMapID = useMapID;
            CurrentMap = useMapID ? SEnvir.GetMap(info.MapID) : SEnvir.GetMap(info.Region.Map);
            LastCheck = SEnvir.Now;
        }
        /// <summary>
        /// 刷怪
        /// </summary>
        /// <param name="eventSpawn">对应事件玩法</param>
        public void DoSpawn(bool eventSpawn)
        {
            if (!eventSpawn)  //如果不是事件玩法刷怪
            {
                //  刷怪信息为事件刷怪 或者  时间小于 下一次刷怪时间 跳出
                if (Info.EventSpawn || SEnvir.Now < NextSpawn) return;

                if (Info.Delay >= 1000000)   //怪物刷新的时间大于100万分钟
                {
                    TimeSpan timeofDay = TimeSpan.FromMinutes(Info.Delay - 1000000);   //设置的刷新时间-100万

                    if (LastCheck.TimeOfDay >= timeofDay || SEnvir.Now.TimeOfDay < timeofDay)
                    {
                        LastCheck = SEnvir.Now;
                        return;
                    }

                    LastCheck = SEnvir.Now;
                }
                else
                {
                    if (Info.Announce)  //如果勾选了怪物刷新提示
                    {
                        //那么下一次的刷新时间就是固定值增加秒(刷新延迟的分钟数*60 加 额外增加的随机刷新时间)
                        NextSpawn = SEnvir.Now.AddSeconds(Info.Delay * 60 + SEnvir.Random.Next(Info.RandomTime * 60));
                    }
                    else
                    {
                        if (Info.Punctual)  //如果勾选了准时刷怪
                        {
                            NextSpawn = SEnvir.Now.AddSeconds(Info.Delay * 60);  //刷新时间按设置的时间定时刷新
                        }
                        else
                        {
                            //没有勾选怪物刷新提示的话，那么刷新值为随机值增加秒(随机值刷新延迟的分钟数*60 加 刷新延迟的分钟数*30)
                            NextSpawn = SEnvir.Now.AddSeconds(SEnvir.Random.Next(Info.Delay * 60) + Info.Delay * 30);
                        }
                    }
                }

                SpawnTime = SEnvir.Now;
            }

            for (int i = AliveCount; i < Info.Count; i++)
            {
                MonsterObject mob = MonsterObject.GetMonster(Info.Monster);

                if (!Info.Monster.IsBoss) //如果不是BOSS
                {
                    //万圣节活动
                    if (SEnvir.Now > CurrentMap.HalloweenEventTime && SEnvir.Now <= Config.HalloweenEventEnd)
                    {
                        mob = new HalloweenMonster { MonsterInfo = Info.Monster, HalloweenEventMob = true };
                        CurrentMap.HalloweenEventTime = SEnvir.Now.AddHours(1);
                    }
                    //圣诞节活动
                    else if (SEnvir.Now > CurrentMap.ChristmasEventTime && SEnvir.Now <= Config.ChristmasEventEnd)
                    {
                        mob = new ChristmasMonster { MonsterInfo = Info.Monster, ChristmasEventMob = true };
                        CurrentMap.ChristmasEventTime = SEnvir.Now.AddMinutes(20);
                    }
                }

                mob.SpawnInfo = this;

                bool successSpawn = UseMapID
                    ? mob.Spawn(Info.MapID, Info.MapX, Info.MapY, Info.Range)
                    : mob.Spawn(Info.Region);

                if (!successSpawn)
                {
                    mob.SpawnInfo = null;
                    continue;
                }

                // 重置怪物的个人变量
                //if (SEnvir.WathchingMonsters.TryGetValue(this.Info.Monster.Index, out var variable))
                //{
                //    mob.TempVariables = variable;
                //}

                if (Info.Announce)  //怪物刷新提示
                {
                    if (Info.Delay >= 1000000)
                    {
                        string _temname;
                        // 只过滤结尾的数字
                        _temname = mob.MonsterInfo.MonsterName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });
                        foreach (SConnection con in SEnvir.Connections)
                            con.ReceiveChat($"{_temname} " + "已经出现".Lang(con.Language), MessageType.System);
                    }
                    //else
                    //{
                    //    foreach (SConnection con in SEnvir.Connections)
                    //        con.ReceiveChat("Monster.BossSpawn".Lang(con.Language, CurrentMap.Info.Description), MessageType.System);
                    //}
                }

                mob.DropSet = Info.DropSet;
                AliveCount++;
            }
        }
    }
    public sealed class FubenMovements
    {
        public Map Map { get; set; }
        public MovementInfo Movement { get; set; }

    }
    /// <summary>
    /// 地图单元格
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// 地图位置
        /// </summary>
        public Point Location;
        /// <summary>
        /// 地图
        /// </summary>
        public Map Map;
        /// <summary>
        /// 地图对象列表
        /// </summary>
        public List<MapObject> Objects;
        /// <summary>
        /// 地图安全区
        /// </summary>
        public SafeZoneInfo SafeZone;
        /// <summary>
        /// 地图门点坐标链接信息
        /// </summary>
        public List<MovementInfo> Movements;
        public List<FubenMovements> FubenMovements;
        /// <summary>
        /// 是否链接点
        /// </summary>
        public bool HasMovement;
        /// <summary>
        /// 地图单元格位置
        /// </summary>
        /// <param name="location"></param>
        public Cell(Point location)
        {
            Location = location;
        }

        /// <summary>
        /// 增加地图对象
        /// </summary>
        /// <param name="ob"></param>
        public void AddObject(MapObject ob)
        {
            if (Objects == null)
                Objects = new List<MapObject>();

            Objects.Add(ob);

            ob.CurrentMap = Map;
            ob.CurrentLocation = Location;

            Map.OrderedObjects[Location.X].Add(ob);
        }
        /// <summary>
        /// 移除地图对象
        /// </summary>
        /// <param name="ob"></param>
        public void RemoveObject(MapObject ob)
        {
            Objects.Remove(ob);

            if (Objects.Count == 0)
                Objects = null;

            Map.OrderedObjects[Location.X].Remove(ob);
        }
        /// <summary>
        /// 有阻碍
        /// </summary>
        /// <param name="checker"></param>
        /// <param name="cellTime"></param>
        /// <returns></returns>
        public bool IsBlocking(MapObject checker, bool cellTime)
        {

            if (Objects == null) return false;

            foreach (MapObject ob in Objects)
            {
                if (!ob.Blocking) continue;
                if (cellTime && SEnvir.Now < ob.CellTime) continue;

                if (ob.Race == ObjectType.Monster && (ob as MonsterObject).PetOwner == checker) continue;

                if (ob.Stats == null) return true;

                if (ob.Buffs.Any(x => x.Type == BuffType.Cloak || x.Type == BuffType.Transparency) && !ob.InGroup(checker)) continue;  //&& ob.Level > checker.Level

                return true;
            }

            return false;
        }
        public FubenMovements SetMovements(Map map, IronPython.Runtime.PythonDictionary pydic)
        {
            if (FubenMovements == null) FubenMovements = new List<FubenMovements>();
            FubenMovements movement = new FubenMovements();
            movement.Movement = new MovementInfo();
            movement.Movement.DestinationRegion = new MapRegion();
            movement.Movement.DestinationRegion.Map = map.Info;
            movement.Map = map;
            if (movement.Movement.DestinationRegion.PointList == null) movement.Movement.DestinationRegion.PointList = new List<Point>();
            //List<ItemType> typelist = new List<ItemType>();
            object temp;
            if (pydic.TryGetValue("Points", out temp))
            {
                IronPython.Runtime.List types = temp as IronPython.Runtime.List;
                if (types != null)
                {

                    for (int i = 0; i < types.Count; i++)
                    {
                        IronPython.Runtime.PythonTuple tuple = types[i] as IronPython.Runtime.PythonTuple;
                        if (tuple != null)
                            movement.Movement.DestinationRegion.PointList.Add(new Point(Convert.ToInt32(tuple[0]), Convert.ToInt32(tuple[1])));

                    }
                }
            }

            FubenMovements.Add(movement);
            return movement;
        }
        /// <summary>
        /// 地图移动
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public Cell GetMovement(MapObject ob)
        {
            if ((FubenMovements == null || FubenMovements.Count == 0) && (Movements == null || Movements.Count == 0))  //如果坐标链接信息为空   或者  坐标链接信息门点数为0 无法进入
                return this;

            for (int i = 0; i < 5; i++) //尝试移动20次
            {
                MovementInfo movement = null;
                Map map = null;
                if (FubenMovements != null && FubenMovements.Count > 0)
                {
                    int index = SEnvir.Random.Next(FubenMovements.Count);
                    movement = FubenMovements[index].Movement;
                    map = FubenMovements[index].Map;
                }
                else
                {
                    movement = Movements[SEnvir.Random.Next(Movements.Count)];
                    map = SEnvir.GetMap(movement.DestinationRegion.Map);
                }
                Cell cell = map.GetCell(movement.DestinationRegion.PointList[SEnvir.Random.Next(movement.DestinationRegion.PointList.Count)]);
                if (cell == null) continue;

                if (ob.Race == ObjectType.Player)
                {
                    PlayerObject player = (PlayerObject)ob;

                    //地图限制级别判断
                    if (movement.DestinationRegion.Map.MinimumLevel > ob.Level && !player.Character.Account.TempAdmin)
                    {
                        player.Connection.ReceiveChat("Movement.NeedLevel".Lang(player.Connection.Language, movement.DestinationRegion.Map.MinimumLevel), MessageType.System);

                        foreach (SConnection con in player.Connection.Observers)
                            con.ReceiveChat("Movement.NeedLevel".Lang(con.Language, movement.DestinationRegion.Map.MinimumLevel), MessageType.System);

                        break;
                    }
                    if (movement.DestinationRegion.Map.MaximumLevel > 0 && movement.DestinationRegion.Map.MaximumLevel < ob.Level && !player.Character.Account.TempAdmin)
                    {
                        player.Connection.ReceiveChat("Movement.NeedMaxLevel".Lang(player.Connection.Language, movement.DestinationRegion.Map.MaximumLevel), MessageType.System);

                        foreach (SConnection con in player.Connection.Observers)
                            con.ReceiveChat("Movement.NeedMaxLevel".Lang(con.Language, movement.DestinationRegion.Map.MaximumLevel), MessageType.System);

                        break;
                    }
                    //地图需要刷新指定的怪物才能进入
                    if (movement.NeedSpawn != null)
                    {
                        SpawnInfo spawn = SEnvir.Spawns.FirstOrDefault(x => x.Info == movement.NeedSpawn);

                        int waitMin = movement.SpanTime;

                        if (spawn == null) break;

                        if (waitMin > 0 && spawn.SpawnTime.AddMinutes(waitMin) < SEnvir.Now) break;

                        if (spawn.AliveCount == 0)
                        {
                            if (!movement.CanLinkTips)
                            {
                                player.Connection.ReceiveChat("Movement.NeedMonster".Lang(player.Connection.Language), MessageType.System);
                            }

                            foreach (SConnection con in player.Connection.Observers)
                                con.ReceiveChat("Movement.NeedMonster".Lang(con.Language), MessageType.System);

                            break;
                        }
                    }
                    //地图需要指定道具进入
                    if (movement.NeedItem != null)
                    {
                        if (player.GetItemCount(movement.NeedItem) == 0)
                        {
                            if (!movement.CanLinkTips)
                            {
                                player.Connection.ReceiveChat("Movement.NeedItem".Lang(player.Connection.Language, movement.NeedItem.ItemName), MessageType.System);
                            }

                            foreach (SConnection con in player.Connection.Observers)
                                con.ReceiveChat("Movement.NeedItem".Lang(con.Language, movement.NeedItem.ItemName), MessageType.System);
                            break;
                        }

                        player.TakeItem(movement.NeedItem, 1);
                    }

                    try
                    {
                        dynamic trig_map;
                        if (SEnvir.PythonEvent.TryGetValue("MapEvent_trig_map", out trig_map))
                        {
                            //var argss = new Tuple<object>(this);
                            PythonTuple args = PythonOps.MakeTuple(new object[] { movement, player, this });
                            Nullable<bool> canmovment = SEnvir.ExecutePyWithTimer(trig_map, movement.Index, "OnMovement", args);
                            //Nullable<bool> canmovment = trig_map(movement.Index, "OnMovement", args);
                            if (canmovment != null)
                            {
                                if (!canmovment.Value)
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch (SyntaxErrorException e)
                    {
                        string msg = "地图事件（同步错误） : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    catch (SystemExitException e)
                    {
                        string msg = "地图事件（系统退出） : \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(e);
                        SEnvir.Log(string.Format(msg, error));
                    }
                    catch (Exception ex)
                    {
                        string msg = "地图事件（加载插件时错误）: \"{0}\"";
                        ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                        string error = eo.FormatException(ex);
                        SEnvir.Log(string.Format(msg, error));
                    }

                    switch (movement.Effect)
                    {
                        //特殊定义 过门点自动特修
                        case MovementEffect.SpecialRepair:
                            player.SpecialRepair(EquipmentSlot.Weapon);
                            player.SpecialRepair(EquipmentSlot.Shield);
                            player.SpecialRepair(EquipmentSlot.Helmet);
                            player.SpecialRepair(EquipmentSlot.Armour);
                            player.SpecialRepair(EquipmentSlot.Necklace);
                            player.SpecialRepair(EquipmentSlot.BraceletL);
                            player.SpecialRepair(EquipmentSlot.BraceletR);
                            player.SpecialRepair(EquipmentSlot.RingL);
                            player.SpecialRepair(EquipmentSlot.RingR);
                            player.SpecialRepair(EquipmentSlot.Shoes);

                            player.RefreshStats();
                            break;
                    }
                }
                return cell.GetMovement(ob);
            }
            return this;
        }
    }
}
