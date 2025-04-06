using IronPython.Runtime;
using IronPython.Runtime.Exceptions;
using IronPython.Runtime.Operations;
using Library;
using Library.Network;
using Library.SystemModels;
using Microsoft.Scripting.Hosting;
using Server.DBModels;
using Server.Envir;
using Server.Models.EventManager.Events;
using Server.Models.Monsters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using S = Library.Network.ServerPackets;


namespace Server.Models
{
    /// <summary>
    /// 怪物对象 地图对象
    /// </summary>
    public class MonsterObject : MapObject
    {
        /// <summary>
        /// 怪物类型 怪物
        /// </summary>
        public override ObjectType Race => ObjectType.Monster;
        /// <summary>
        /// 方向
        /// </summary>
        public sealed override MirDirection Direction { get; set; }
        /// <summary>
        /// 搜索时间
        /// </summary>
        public DateTime SearchTime;
        /// <summary>
        /// 切换目标时间
        /// </summary>
        public DateTime RoamTime;
        /// <summary>
        /// 经验获得时间
        /// </summary>
        public DateTime EXPOwnerTime;
        /// <summary>
        /// 死亡时间
        /// </summary>
        public DateTime DeadTime;
        /// <summary>
        /// 狂暴时间
        /// </summary>
        public DateTime RageTime;
        /// <summary>
        /// 驯服的时间
        /// </summary>
        public DateTime TameTime;
        /// <summary>
        /// 搜索目标延时3秒
        /// </summary>
        public TimeSpan SearchDelay = TimeSpan.FromSeconds(2);
        /// <summary>
        /// 切换目标延时2秒
        /// </summary>
        public TimeSpan RoamDelay = TimeSpan.FromSeconds(2);
        /// <summary>
        /// 经验获得者延时5秒
        /// </summary>
        public TimeSpan EXPOwnerDelay = TimeSpan.FromSeconds(5);
        /// <summary>
        /// 怪物信息
        /// </summary>
        public MonsterInfo MonsterInfo;
        /// <summary>
        /// 生成信息
        /// </summary>
        public SpawnInfo SpawnInfo;
        /// <summary>
        /// 爆率设置数值
        /// </summary>
        public int DropSet;
        /// <summary>
        /// 目标
        /// </summary>
        public MapObject Target
        {
            get { return _Target; }
            set
            {
                if (_Target == value) return;
                _Target = value;

                if (_Target == null)
                    SearchTime = DateTime.MinValue;
            }
        }
        private MapObject _Target;
        /// <summary>
        /// 目标攻击时间
        /// </summary>
		private uint _TargetAttackTick = 0;
        /// <summary>
        /// 是否标记玩家
        /// </summary>
        public bool PlayerTagged;
        /// <summary>
        /// 刷怪列表
        /// </summary>
        public Dictionary<MonsterInfo, int> SpawnList = new Dictionary<MonsterInfo, int>();
        /// <summary>
        /// 骨架尸体
        /// </summary>
        public bool Skeleton;

        #region 经验获得者
        /// <summary>
        /// 经验获得者
        /// </summary>
        public PlayerObject EXPOwner
        {
            get { return _EXPOwner; }
            set
            {
                if (_EXPOwner == value) return;

                PlayerObject oldValue = _EXPOwner;
                _EXPOwner = value;

                OnEXPOwnerChanged(oldValue, value);
            }
        }
        private PlayerObject _EXPOwner;
        public virtual void OnEXPOwnerChanged(PlayerObject oValue, PlayerObject nValue)
        {
            oValue?.TaggedMonsters.Remove(this);

            nValue?.TaggedMonsters.Add(this);
        }

        #endregion

        /// <summary>
        /// 账户信息 角色道具 爆率
        /// </summary>
        public Dictionary<AccountInfo, List<UserItem>> Drops;
        /// <summary>
        /// 存储爆率测试数据
        /// </summary>
        public Dictionary<string, long> DropTestDict;
        /// <summary>
        /// 怪物经验
        /// </summary>
        public virtual decimal Experience => MonsterInfo.Experience;
        /// <summary>
        /// 额外经验比率
        /// </summary>
        public decimal ExtraExperienceRate = 0;
        /// <summary>
        /// 是否被动
        /// </summary>
        public bool Passive;
        /// <summary>
        /// 是否能收割 割肉
        /// </summary>
        public bool NeedHarvest;
        /// <summary>
        /// 是否躲避火墙
        /// </summary>
        public bool AvoidFireWall;
        /// <summary>
        /// 收获计数
        /// </summary>
        public int HarvestCount;
        /// <summary>
        /// 死亡云持续时间
        /// </summary>
        public int DeathCloudDurationMin = 4000, DeathCloudDurationRandom = 0;
        /// <summary>
        /// 移动延迟
        /// </summary>
        public int MoveDelay => MoveDelayBase + MoveDelayOffset;
        /// <summary>
        /// 移动延迟 基础值
        /// </summary>
        public int MoveDelayBase;
        /// <summary>
        /// 移动延迟 额外调整值
        /// </summary>
        public int MoveDelayOffset = 0;

        /// <summary>
        /// 攻击延迟
        /// </summary>
        public int AttackDelay => AttackDelayBase + AttackDelayOffset;


        public DateTime AttackFirstDelay;
        /// <summary>
        /// 攻击延迟 基础值
        /// </summary>HasMonsterBuff
        public int AttackDelayBase;
        /// <summary>
        /// 攻击延迟 额外调整值
        /// </summary>
        public int AttackDelayOffset = 0;

        /// <summary>
        /// 召唤出来的怪物仆从列表
        /// </summary>
        public List<MonsterObject> MinionList = new List<MonsterObject>();
        /// <summary>
        /// 怪物对象的主人
        /// </summary>
        public MonsterObject Master;
        /// <summary>
        /// 仆从的最大数量
        /// </summary>
        public int MaxMinions = 20;
        /// <summary>
        /// 宠物主人
        /// </summary>
        public PlayerObject PetOwner;
        /// <summary>
        /// 角色魔法技能表
        /// </summary>
        public HashSet<UserMagic> Magics = new HashSet<UserMagic>();
        /// <summary>
        /// 宝宝等级
        /// </summary>
        public int SummonLevel;
        /// <summary>
        /// 宝宝最大等级
        /// </summary>
        public int MaxSummonLevel = Config.PetMaxLevel;
        /// <summary>
        /// 宝宝经验
        /// </summary>
        public decimal PetExperience;
        /// <summary>
        /// 怪物的视野范围
        /// </summary>
        public int ViewRange
        {
            //如果是深渊状态，只有2格视野  其他都按照怪物设定的视野值
            get { return PoisonList.Any(x => x.Type == PoisonType.Abyss) ? 2 : MonsterInfo.ViewRange; }
        }
        /// <summary>
        /// 中毒类型
        /// </summary>
        public PoisonType PoisonType;
        /// <summary>
        /// 中毒几率 值越大中毒几率越小
        /// </summary>
        public int PoisonRate = 10;
        /// <summary>
        /// 中毒滴答数 中毒减血持续次数
        /// </summary>
        public int PoisonTicks = 5;
        /// <summary>
        /// 中毒频率 中毒扣减间隔时间
        /// </summary>
        public int PoisonFrequency = 2;
        /// <summary>
        /// 忽略魔法盾
        /// </summary>
        public bool IgnoreShield;
        /// <summary>
        /// 复活节活动怪物
        /// </summary>
        public bool EasterEventMob;
        /// <summary>
        /// 万圣节活动怪物
        /// </summary>
        public bool HalloweenEventMob;
        /// <summary>
        /// 圣诞节活动怪物
        /// </summary>
        public bool ChristmasEventMob;
        /// <summary>
        /// 地图血值比率
        /// </summary>
        public int MapHealthRate;
        /// <summary>
        /// 地图伤害值比率
        /// </summary>
        public int MapDamageRate;
        /// <summary>
        /// 地图防魔值比率
        /// </summary>
        public int MapBewareRate;
        /// <summary>
        /// 地图经验值比率
        /// </summary>
        public int MapExperienceRate;
        /// <summary>
        /// 地图爆率值比率
        /// </summary>
        public int MapDropRate;
        /// <summary>
        /// 地图金币爆率值比率
        /// </summary>
        public int MapGoldRate;
        /// <summary>
        /// 攻击元素
        /// </summary>
        public Element AttackElement => Stats.GetAffinityElement();
        /// <summary>
        /// 是否宠物主人
        /// </summary>
        public bool isPetOwner = false;
        // 怪物自己的临时变量
        public Dictionary<string, int> TempVariables = new Dictionary<string, int>();
        // 触发AI脚本的间隔
        public DateTime PyAITime;
        // 怪物是否有怪物buff
        public bool HasMonsterBuff(string name)
        {
            return Buffs.Any(x => x.Name == name);
        }

        /// <summary>
        /// 是否可以转向
        /// </summary>
        public virtual bool CanTurn => true;
        /// <summary>
        /// 是否可以移动
        /// </summary>
        public override bool CanMove
        {
            get
            {
                if (MoveDelay > 0 && base.CanMove)
                {
                    isPetOwner = (PetOwner == null
                    || PetOwner.PetMode == PetMode.Both
                    || PetOwner.PetMode == PetMode.Move
                    || PetOwner.PetMode == PetMode.PvP);

                    if (isPetOwner && (Poison & PoisonType.Silenced) != PoisonType.Silenced)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public override bool CanAttack
        {
            get
            {
                return base.CanAttack && (Poison & PoisonType.Silenced) != PoisonType.Silenced && AttackDelay > 0 && (PetOwner == null || PetOwner.PetMode == PetMode.Both || PetOwner.PetMode == PetMode.Attack || PetOwner.PetMode == PetMode.PvP);
            }
        }

        #region 怪物AI定义
        /// <summary>
        /// 获取怪物信息
        /// </summary>
        /// <param name="monsterInfo"></param>
        /// <returns></returns>
        public static MonsterObject GetMonster(MonsterInfo monsterInfo)
        {
            //怪物等于自定义怪物 且AI设置大于1000 那么跳转到自定义怪物
            if (monsterInfo.Image == MonsterImage.DiyMonsMon && monsterInfo.AI > 999)
            {
                return new DiyMonster(monsterInfo) { };
            }

            switch (monsterInfo.AI)             //怪物AI
            {
                case -1:
                    return new Guard       //卫士
                    {
                        MonsterInfo = monsterInfo     //怪物信息
                    };
                case 1:
                    return new MonsterObject              //鸡
                    {
                        MonsterInfo = monsterInfo,
                        Passive = true,          //被动
                        NeedHarvest = true,      //挖
                        HarvestCount = 2         //收割次数
                    };
                case 2:
                    return new MonsterObject             //猪羊
                    {
                        MonsterInfo = monsterInfo,
                        Passive = true,          //被动
                        NeedHarvest = true,      //挖
                        HarvestCount = 3         //收割次数
                    };
                case 3:
                    return new MonsterObject        //狼 蝎子
                    {
                        MonsterInfo = monsterInfo,
                        NeedHarvest = true,       //挖
                        HarvestCount = 3          //收割次数
                    };
                case 4:
                    return new TreeMonster         //树 无法移动的
                    {
                        MonsterInfo = monsterInfo
                    };
                case 5:
                    return new CarnivorousPlant    //食人花 不动 攻击
                    {
                        MonsterInfo = monsterInfo,
                        NeedHarvest = true,       //挖
                        HarvestCount = 2          //收割次数
                    };
                case 6:
                    return new SpittingSpider     //毒蜘蛛 绿毒攻击
                    {
                        MonsterInfo = monsterInfo,
                        NeedHarvest = true,         //挖
                        HarvestCount = 2,           //收割次数
                        PoisonType = PoisonType.Green     //绿毒
                    };
                case 7:
                    return new SkeletonAxeThrower   //掷斧骷髅 抛物攻击
                    {
                        MonsterInfo = monsterInfo
                    };
                case 8:
                    return new MonsterObject  //洞蛆月魔 石化麻痹
                    {
                        MonsterInfo = monsterInfo,
                        NeedHarvest = true,         //挖
                        HarvestCount = 2,           //收割次数
                        PoisonType = PoisonType.Paralysis,   //中毒麻痹
                        PoisonTicks = 1,        //中毒的滴答数
                        PoisonFrequency = 5,    //中毒频率
                        PoisonRate = 15         //中毒几率
                    };
                case 9:
                    return new GhostSorcerer    //雷电僵尸  电攻击
                    {
                        MonsterInfo = monsterInfo
                    };
                case 10:
                    return new GhostMage      //钻地僵尸
                    {
                        MonsterInfo = monsterInfo
                    };
                case 11:
                    return new VoraciousGhost   //复活僵尸
                    {
                        MonsterInfo = monsterInfo
                    };
                case 12:
                    return new HealerAnt       //蚂蚁道士 治愈术
                    {
                        MonsterInfo = monsterInfo
                    };
                case 13:
                    return new LordNiJae      //触龙神
                    {
                        MonsterInfo = monsterInfo
                    };
                case 14:
                    return new SpittingSpider   //腐蚀人鬼 花色蜘蛛  喷毒
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Green
                    };
                case 15:
                    return new MonsterObject    //火焰沃玛  喷火
                    {
                        MonsterInfo = monsterInfo
                    };
                case 16:
                    return new UmaKing     //沃玛教主 雷电攻击
                    {
                        MonsterInfo = monsterInfo
                    };
                case 17:
                    return new ArachnidGrazer //幻影蜘蛛  招小怪
                    {
                        MonsterInfo = monsterInfo, //怪物信息
                        //生成列表= 怪物信息列表 有绑定 怪物 标签 爆裂蜘蛛 数量1
                        SpawnList = { [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Larva)] = 1 }
                    };
                case 18:
                    return new Larva  //爆裂蜘蛛  绿毒 自爆
                    {
                        MonsterInfo = monsterInfo, //怪物信息
                        PoisonType = PoisonType.Green  //攻击状态类型 绿毒
                    };
                case 19:
                    return new RedMoonTheFallen   //赤月恶魔
                    {
                        MonsterInfo = monsterInfo
                    };
                case 20:
                    return new SkeletonAxeThrower    //祖玛弓箭手
                    {
                        MonsterInfo = monsterInfo,
                        FearRate = 2,           //保持距离
                        FearDuration = 4        //距离超出范围（追击）
                    };
                case 21:
                    return new ZumaGuardian      //祖玛卫士 石化效果人靠近才恢复原状开始攻击
                    {
                        MonsterInfo = monsterInfo
                    };
                case 22:
                    return new ZumaKing       //祖玛教主
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.ZumaArcherMonster)] = 50,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.ZumaFanaticMonster)] = 25,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.ZumaGuardianMonster)] = 25,
                            //[SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.ZumaKeeperMonster)] = 1
                        }
                    };
                case 23:
                    return new Monkey    //猿猴战士 雪狼 绿毒
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Green
                    };
                case 24:
                    return new Monkey    //猿猴战将 冰魂卫士 红毒
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Red
                    };
                case 25:
                    return new EvilElephant         //巨象兽 沙鱼
                    {
                        MonsterInfo = monsterInfo
                    };
                case 26:
                    return new NumaMage          //法老 真幻鬼 雷系
                    {
                        MonsterInfo = monsterInfo
                    };
                case 27:
                    return new GhostMage          //沙漠石人 地底钻出
                    {
                        MonsterInfo = monsterInfo
                    };
                case 28:
                    return new WindfurySorcerer     //沙漠风魔  旋风攻击
                    {
                        MonsterInfo = monsterInfo
                    };
                case 29:
                    return new SkeletonAxeThrower   //沙漠树魔  喷箭攻击
                    {
                        MonsterInfo = monsterInfo
                    };
                case 30:
                    return new NetherworldGate     //异界之门
                    {
                        MonsterInfo = monsterInfo
                    };
                case 31:
                    return new SonicLizard         //魔石咆哮者
                    {
                        MonsterInfo = monsterInfo
                    };
                case 33:
                    return new GiantLizard         //诺玛骑兵
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 6,            //攻击距离
                        //IgnoreShield = true         //破盾
                    };
                case 34:
                    return new SkeletonAxeThrower   //冰魂弓箭手
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 9             //攻击距离9
                    };
                case 35:
                    return new MonsterObject        //潘夜冰魔
                    {
                        MonsterInfo = monsterInfo
                    };
                case 36:
                    return new NumaMage            //潘夜右护卫
                    {
                        MonsterInfo = monsterInfo
                    };
                case 37:
                    return new MonsterObject      //潘夜云魔
                    {
                        MonsterInfo = monsterInfo
                    };
                case 38:
                    return new BanyaLeftGuard      //潘夜左护卫
                    {
                        MonsterInfo = monsterInfo
                    };
                case 39:
                    return new MonsterObject         //潘夜风魔
                    {
                        MonsterInfo = monsterInfo
                    };
                case 40:
                    return new MonsterObject         //潘夜火魔
                    {
                        MonsterInfo = monsterInfo
                    };
                case 41:
                    return new EmperorSaWoo          //潘夜牛魔王
                    {
                        MonsterInfo = monsterInfo
                    };
                case 42:
                    return new SpittingSpider       //骷髅士兵 水晶蝙蝠
                    {
                        MonsterInfo = monsterInfo
                    };
                case 43:
                    return new ArchLichTaedu         //骷髅教主
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.BoneArcher)] = 90,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.BoneSoldier)] = 15,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.BoneBladesman)] = 15,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.BoneCaptain)] = 15,
                            //[SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.SkeletonEnforcer)] = 1
                        }
                    };
                case 44:
                    return new WedgeMothLarva        //角蝇
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList = { [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.LesserWedgeMoth)] = 1 }
                    };
                case 45:
                    return new RazorTusk           //超级黑野猪
                    {
                        MonsterInfo = monsterInfo
                    };
                case 46:
                    return new SpittingSpider        //紫红女神 天狼蜘蛛 喷红毒
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Red,  //红毒
                        PoisonTicks = 1,               //毒的滴答数
                        PoisonFrequency = 10,         //中毒频率
                        PoisonRate = 25              //中毒率
                    };
                case 47:
                    return new SpittingSpider      //绿荫女神  喷绿毒
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Green,  //绿毒
                        PoisonTicks = 7,                //毒的滴答数
                        PoisonRate = 15                //中毒率
                    };
                case 48:
                    return new SonicLizard        //首将 武力神将 诺玛斧兵
                    {
                        MonsterInfo = monsterInfo,
                        //IgnoreShield = true        //破盾
                    };
                case 49:
                    return new GiantLizard           //石像狮子
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 8,                       //攻击距离8
                        PoisonType = PoisonType.Paralysis,    //麻痹
                        PoisonTicks = 1,                     //毒滴答数
                        PoisonFrequency = 5,                  //中毒频率
                        PoisonRate = 15,                     //中毒率15
                    };
                case 50:
                    return new GiantLizard           //火焰狮子
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 8             //攻击距离8
                    };
                case 52:
                    return new WhiteBone()               //变异骷髅 超强骷髅
                    {
                        MonsterInfo = monsterInfo
                    };
                case 53:
                    return new Shinsu               //神兽
                    {
                        MonsterInfo = monsterInfo
                    };
                case 54:
                    return new GiantLizard          //巨蜥
                    {
                        MonsterInfo = monsterInfo,
                        RangeCooldown = TimeSpan.FromSeconds(5)     //射程冷却5秒
                    };
                case 56:
                    return new CorrosivePoisonSpitter   //爆毒神魔
                    {
                        MonsterInfo = monsterInfo,
                        //PoisonType = PoisonType.Green,   //绿毒
                        //PoisonTicks = 7,                 //滴答数7
                        //PoisonRate = 15,                //中毒率15
                        //IgnoreShield = true             //破盾
                    };
                case 57:
                    return new CorrosivePoisonSpitter    //触角神魔
                    {
                        MonsterInfo = monsterInfo
                    };
                case 58:
                    return new Stomper              //海神将领
                    {
                        MonsterInfo = monsterInfo
                    };
                case 59:
                    return new CrimsonNecromancer()   //红衣法师
                    {
                        MonsterInfo = monsterInfo
                    };
                case 60:
                    return new ChaosKnight()         //霸王守卫
                    {
                        MonsterInfo = monsterInfo
                    };
                case 61:
                    return new PachontheChaosbringer   //霸王教主
                    {
                        MonsterInfo = monsterInfo
                    };
                case 62:
                    return new NumaHighMage            //诺玛司令
                    {
                        MonsterInfo = monsterInfo
                    };
                case 63:
                    return new NumaStoneThrower        //诺玛抛石兵
                    {
                        MonsterInfo = monsterInfo
                    };
                case 64:
                    return new Monkey                //魔大将 魔小将
                    {
                        MonsterInfo = monsterInfo
                    };
                case 65:
                    return new IcyGoddess            //魄冰女神
                    {
                        MonsterInfo = monsterInfo,
                        FindRange = 3                 //查找范围3
                    };
                case 66:
                    return new IcySpiritWarrior        //冰魄鬼武将
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,  //麻痹
                        PoisonTicks = 1,                   //毒滴答数
                        PoisonFrequency = 5,               //中毒频率5
                        PoisonRate = 25                   //中毒率25
                    };
                case 67:
                    return new IcySpiritGeneral         //冰魂鬼武士
                    {
                        MonsterInfo = monsterInfo,
                        IgnoreShield = true,            //破盾
                    };
                case 68:
                    return new Warewolf                //狼人 雪虎
                    {
                        MonsterInfo = monsterInfo,
                        IgnoreShield = true,          //破盾
                    };
                case 69:
                    return new JinamStoneGate           //赤龙石门
                    {
                        MonsterInfo = monsterInfo
                    };
                case 70:
                    return new FrostLordHwa             //火影
                    {
                        MonsterInfo = monsterInfo
                    };
                case 71:
                    return new BanyoWarrior          //剑客神徒
                    {
                        MonsterInfo = monsterInfo
                    };
                case 72:
                    return new BanyoCaptain        //海盗武将 海盗骑兵
                    {
                        MonsterInfo = monsterInfo
                    };
                case 74:
                    return new BanyoLordGuzak    //海盗天马
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.BanyoCaptain)] = 2,
                        }
                    };
                case 75:
                    return new DepartedMonster     //死去的怪物
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList = { [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MatureEarwig)] = 1 }
                    };
                case 76:
                    return new DepartedMonster    //死去的怪物
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList = { [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.GoldenArmouredBeetle)] = 1 }
                    };
                case 77:
                    return new EnragedLordNiJae   //天龙窝主
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList = { [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Millipede)] = 1 },
                        MaxMinions = 200,      //最大几率召唤小怪
                    };
                case 78:
                    return new JinchonDevil    //黑度魔神  震天魔神
                    {
                        MonsterInfo = monsterInfo
                    };
                case 79:
                    return new GiantLizard          //黄铜黑耀武士
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 10,                  //攻击范围10
                        RangeCooldown = TimeSpan.FromSeconds(5)  //射程冷却5秒
                    };
                case 80:
                    return new SunFeralWarrior      //金阳武将
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.FerociousFlameDemon)] = 5,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.FlameDemon)] = 1,
                        }
                    };
                case 81:
                    return new MoonFeralWarrior       //银月武将
                    {
                        MonsterInfo = monsterInfo
                    };
                case 82:
                    return new OxFeralGeneral        //狂牛鬼将
                    {
                        MonsterInfo = monsterInfo,
                        IgnoreShield = true,         //破盾
                    };
                case 83:
                    return new FlameDemon             //火灵牛鬼
                    {
                        MonsterInfo = monsterInfo,
                        Min = -2,
                        Max = 2,
                    };
                case 84:
                    return new WingedHorror         //灵牛鬼将军
                    {
                        MonsterInfo = monsterInfo,
                        RangeChance = 1,          //射程机会
                    };
                case 85:
                    return new EmperorSaWoo           //金牛大将军
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,  //麻痹
                        PoisonTicks = 1,                     //滴答数
                        PoisonFrequency = 5,                //中毒频率5
                        PoisonRate = 8                      //中毒率8
                    };
                case 86:
                    return new FlameDemon             //凶恶火灵牛鬼
                    {
                        MonsterInfo = monsterInfo,
                        //Passive = true,
                        Min = 0,
                        Max = 8,
                    };
                case 87:
                    return new OmaWarlord                  //半兽首将
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Abyss,    //深渊效果
                        PoisonTicks = 1,                  //滴答数
                        PoisonFrequency = 7,              //中毒频率7
                        PoisonRate = 15                   //中毒率15
                    };
                case 88:
                    return new GoruSpearman              //骷髅魔卒
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 89:
                    return new GoruArcher                   //超强骷髅弓箭手
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Silenced,   //沉默
                        PoisonTicks = 1,                    //滴答数
                        PoisonFrequency = 5,                //中毒频率5
                        PoisonRate = 10                     //中毒率10
                    };
                case 90:
                    return new OmaWarlord               //骷髅鬼将
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,   //麻痹
                        PoisonTicks = 1,                      //滴答数
                        PoisonFrequency = 5,                  //中毒频率5
                        PoisonRate = 25                      //中毒率25
                    };
                case 91:
                    return new EnragedArchLichTaedu         //骷髅魔王
                    {
                        MonsterInfo = monsterInfo,

                        MinSpawn = 5,                       //最小刷怪5
                        RandomSpawn = 5,                    //随机刷怪5

                        PoisonType = PoisonType.Red,        //红毒
                        PoisonTicks = 1,                    //滴答数
                        PoisonFrequency = 25,               //中毒频率25
                        PoisonRate = 5,                     //中毒率5

                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.GoruArcher)] = 10,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.GoruGeneral)] = 5,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.GoruSpearman)] = 5,
                        }
                    };
                case 92:
                    return new GiantLizard                //巨蜥
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 9                  //攻击范围9
                    };
                case 93:
                    return new EscortCommander         //卫护将首
                    {
                        MonsterInfo = monsterInfo
                    };
                case 94:
                    return new FieryDancer           //冰宫红舞姬
                    {
                        MonsterInfo = monsterInfo
                    };
                case 95:
                    return new FieryDancer           //冰宫绿舞姬
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,   //麻痹
                        PoisonTicks = 1,                     //滴答数
                        PoisonFrequency = 5,                 //中毒频率5
                        PoisonRate = 15,                     //中毒率15
                    };
                case 96:
                    return new QueenOfDawn              //黎明女王
                    {
                        MonsterInfo = monsterInfo
                    };
                case 97:
                    return new SonicLizard          //魔小将
                    {
                        MonsterInfo = monsterInfo,
                        IgnoreShield = true,         //破盾
                        Range = 5                   //几率5
                    };
                case 98:
                    return new YumgonWitch           //魔女
                    {
                        MonsterInfo = monsterInfo,
                        AoEElement = Element.Lightning       //雷元素
                    };
                case 99:
                    return new JinhwanSpirit         //真幻鬼
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [monsterInfo] = 1,
                        }
                    };
                case 100:
                    return new YumgonWitch         //蚩尤将军
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 101:
                    return new DragonQueen   //赤龙女王
                    {
                        MonsterInfo = monsterInfo,
                        DragonLordInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.DragonLord),

                        SpawnList =
                         {
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.OYoungBeast)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.YumgonWitch)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MaWarden)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MaWarlord)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.JinhwanSpirit)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.JinhwanGuardian)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.OyoungGeneral)] = 2,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.YumgonGeneral)] = 2,
                         }
                    };
                case 102:
                    return new DragonLord        //赤龙魔王
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                         {
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.OYoungBeast)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.YumgonWitch)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MaWarden)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.MaWarlord)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.JinhwanSpirit)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.JinhwanGuardian)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.OyoungGeneral)] = 10000,
                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.YumgonGeneral)] = 10000,

                             [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.DragonLord)] = 1,
                         }
                    };
                case 103:
                    return new GiantLizard    //炎魔
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 5      //攻击范围5
                    };
                case 104:
                    return new FerociousIceTiger   //冰湖白魔兽
                    {
                        MonsterInfo = monsterInfo
                    };
                case 105:
                    return new GiantLizard           //法术神徒
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 5,            //攻击范围5
                        IgnoreShield = true,        //破盾
                        CanPvPRange = true          //能PVP
                    };
                case 106:
                    return new GiantLizard           //烈火神徒
                    {
                        MonsterInfo = monsterInfo,
                        AttackRange = 7,            //攻击范围7
                        CanPvPRange = true          //能PVP
                    };
                case 107:
                    return new SamaFireGuardian   //火系士兵
                    {
                        MonsterInfo = monsterInfo
                    };
                case 108:
                    return new SamaIceGuardian   //冰系士兵
                    {
                        MonsterInfo = monsterInfo
                    };
                case 109:
                    return new SamaLightningGuardian   //雷系士兵
                    {
                        MonsterInfo = monsterInfo
                    };
                case 110:
                    return new SamaWindGuardian    //风系士兵
                    {
                        MonsterInfo = monsterInfo
                    };
                case 111:
                    return new SamaPhoenix      //朱雀天王
                    {
                        MonsterInfo = monsterInfo
                    };
                case 112:
                    return new SamaBlack    //玄武天王
                    {
                        MonsterInfo = monsterInfo
                    };
                case 113:
                    return new SamaBlue    //青龙天王
                    {
                        MonsterInfo = monsterInfo
                    };
                case 114:
                    return new SamaWhite     //白虎天王
                    {
                        MonsterInfo = monsterInfo
                    };

                case 115:
                    return new SamaProphet   //魔灵神主
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.SamaSorcerer)] = 1,
                        }
                    };
                case 116:
                    return new SamaScorcer()    //魔法师
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 117:
                    return new BanyoWarrior          //海盗武士
                    {
                        MonsterInfo = monsterInfo,
                        DoubleDamage = true        //双重伤害
                    };
                case 118:
                    return new OmaMage            //半兽法师
                    {
                        MonsterInfo = monsterInfo
                    };
                case 119:
                    return new MonsterObject        //绿洲蝎子 古代法鬼
                    {
                        MonsterInfo = monsterInfo,

                        PoisonType = PoisonType.Silenced,  //沉默
                        PoisonTicks = 1,                  //滴答数
                        PoisonFrequency = 5,              //中毒频率5
                        PoisonRate = 10                  //中毒率10
                    };
                case 120:
                    return new DoomClaw()           //龙虾王
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 121:
                    return new PinkBat                //水晶火虫
                    {
                        MonsterInfo = monsterInfo
                    };
                case 122:
                    return new QuartzTurtleSub         //水晶玄武
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.QuartzMiniTurtle)] = 2,
                        }
                    };
                case 123:
                    return new Larva       //幼虫
                    {
                        MonsterInfo = monsterInfo,
                        Range = 3,
                    };
                case 124:
                    return new QuartzTree   //水晶树
                    {
                        MonsterInfo = monsterInfo,
                        SubBossInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.QuartzTurtleSub),
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.QuartzBlueBat)] = 20,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.QuartzPinkBat)] = 20,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.QuartzBlueCrystal)] = 20,
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.QuartzRedHood)] = 2,
                        }
                    };
                case 125:
                    return new CarnivorousPlant   //水晶蠕虫
                    {
                        MonsterInfo = monsterInfo,
                        HideRange = 1,          //隐匿范围
                        FindRange = 1           //查找范围
                    };
                case 126:
                    return new MonasteryBoss    //魔道-道士
                    {
                        MonsterInfo = monsterInfo,
                        SpawnList =
                        {
                            [SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.Sacrafice)] = 1,
                        }
                    };
                case 127:
                    return new JinchonDevil    //魔气大僵尸-陆江
                    {
                        MonsterInfo = monsterInfo,
                        CastDelay = TimeSpan.FromSeconds(8),   //投射范围8秒
                        DeathCloudDurationMin = 2000,         //死亡云
                        DeathCloudDurationRandom = 5000
                    };
                case 128:
                    return new HellBringer        //六角魔兽
                    {
                        MonsterInfo = monsterInfo,
                        BatInfo = SEnvir.MonsterInfoList.Binding.First(x => x.Flag == MonsterFlag.HellishBat),
                    };
                case 129:
                    return new DuelHitMonster    //鬼蜮怪物物理攻击 二次攻击带元素
                    {
                        MonsterInfo = monsterInfo
                    };
                case 130:
                    return new CrawlerSlave
                    {
                        MonsterInfo = monsterInfo
                    };
                case 131:
                    return new CursedSlave     //影软鬼 虎形鬼
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,   //麻痹
                        PoisonTicks = 1,                     //滴答数
                        PoisonFrequency = 5,                 //中毒频率5
                        PoisonRate = 15,                     //中毒率15
                    };
                case 132:
                    return new EvilCursedSlave     //盲鬼
                    {
                        MonsterInfo = monsterInfo
                    };
                case 133:
                    return new PoisonousGolem    //斩决鬼
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Green,
                        PoisonTicks = 20,
                        PoisonRate = 5
                    };
                case 134:
                    return new GardenSoldier          //花园战士
                    {
                        MonsterInfo = monsterInfo
                    };
                case 135:
                    return new GardenDefender        //花园剑士
                    {
                        MonsterInfo = monsterInfo
                    };
                case 136:
                    return new RedBlossom           //红芭蕉
                    {
                        MonsterInfo = monsterInfo
                    };
                case 137:
                    return new BlueBlossom          //青芭蕉
                    {
                        MonsterInfo = monsterInfo
                    };
                case 138:
                    return new FireBird             //火冥鸢
                    {
                        MonsterInfo = monsterInfo
                    };
                case 139:
                    return new Terracotta            //古墓小怪
                    {
                        MonsterInfo = monsterInfo
                    };
                case 140:
                    return new Terracotta            //古墓骑兵类
                    {
                        MonsterInfo = monsterInfo,
                        CanPhase = true
                    };
                case 141:
                    return new TerracottaSub         //古墓小BOSS
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,  //麻痹
                        PoisonTicks = 1,
                        PoisonFrequency = 5,
                        PoisonRate = 15,
                    };
                case 142:
                    return new TerracottaBoss           //古墓BOSS古墓主人
                    {
                        MonsterInfo = monsterInfo,
                        PoisonType = PoisonType.Paralysis,  //麻痹
                        PoisonTicks = 1,
                        PoisonFrequency = 5,
                        PoisonRate = 15,
                    };
                case 143:
                    return new Practitioner  //圆木桩
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 144:
                    return new Changer    //嫦娥
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 145:
                    return new SabukPrimeGate   //沙巴克城门
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 146:
                    return new SiegeEquipment    //攻城器械
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 147:
                    return new MasterNorma     //诺玛教主
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 148:
                    return new PatrolCaptain        //巡逻队长
                    {
                        MonsterInfo = monsterInfo,
                        IgnoreShield = true        //破盾
                    };
                case 149:
                    return new SabakGuard    //沙巴克守卫
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 150:
                    return new SabakGuardian         //沙巴克守护者
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 151:
                    return new CelebrationCakes   //庆典蛋糕 不动 随机1-10血
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 152:
                    return new AntQueen
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 153:
                    return new BaqunStatue   //霸群雕像
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 154:
                    return new CivilianNorma   //平民诺玛
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 155:
                    return new NormaArmorKing   //诺玛装甲王
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 156:
                    return new NormaRiprapKing   //诺玛抛石王
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 157:
                    return new NormaPaladin   //诺玛圣骑士
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 158:
                    return new NomaAxeWarriorKing  //诺玛斧兵王  诺玛族长老
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 159:
                    return new NomaCommander  //诺玛大司令
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 160:
                    return new CommanderNoma  //诺玛统领
                    {
                        MonsterInfo = monsterInfo,
                    };
                case 161:
                    return new Fence     //栏栅
                    {
                        MonsterInfo = monsterInfo,
                    };
                default:
                    return new MonsterObject
                    {
                        MonsterInfo = monsterInfo
                    };
            }
        }
        #endregion

        /// <summary>
        /// 怪物对象刷新方向随机值
        /// </summary>
        public MonsterObject()
        {
            Stats = new Stats();
            Direction = (MirDirection)SEnvir.Random.Next(8);
        }
        /// <summary>
        /// 怪物刷新时
        /// </summary>
        protected override void OnSpawned()
        {
            base.OnSpawned();

            if (SpawnInfo != null && SpawnInfo.Info.EasterEventChance > 0 && SEnvir.Now < Config.EasterEventEnd)
                EasterEventMob = SEnvir.Random.Next(SpawnInfo.Info.EasterEventChance) == 0;

            int offset = 1000000;

            MapHealthRate = SEnvir.Random.Next(CurrentMap.Info.MonsterHealth + offset, CurrentMap.Info.MaxMonsterHealth + offset);   //血值
            MapDamageRate = SEnvir.Random.Next(CurrentMap.Info.MonsterDamage + offset, CurrentMap.Info.MaxMonsterDamage + offset);   //伤害值
            MapBewareRate = SEnvir.Random.Next(CurrentMap.Info.MonsterBeware + offset, CurrentMap.Info.MaxMonsterBeware + offset);   //防魔值

            if (MapHealthRate >= CurrentMap.Info.ExperienceRate && MapHealthRate <= CurrentMap.Info.MaxExperienceRate)
                MapExperienceRate = MapHealthRate;
            else
                MapExperienceRate = SEnvir.Random.Next(CurrentMap.Info.ExperienceRate + offset, CurrentMap.Info.MaxExperienceRate + offset);

            MapDropRate = SEnvir.Random.Next(CurrentMap.Info.DropRate + offset, CurrentMap.Info.MaxDropRate + offset);
            MapGoldRate = SEnvir.Random.Next(CurrentMap.Info.GoldRate + offset, CurrentMap.Info.MaxGoldRate + offset);

            MapHealthRate -= offset;
            MapDamageRate -= offset;
            MapBewareRate -= offset;
            MapExperienceRate -= offset;
            MapDropRate -= offset;
            MapGoldRate -= offset;

            RefreshStats();
            CurrentHP = Stats[Stat.Health];
            DisplayHP = CurrentHP;

            RegenTime = SEnvir.Now.AddMilliseconds(SEnvir.Random.Next((int)RegenDelay.TotalMilliseconds));
            SearchTime = SEnvir.Now.AddMilliseconds(SEnvir.Random.Next((int)SearchDelay.TotalMilliseconds));
            RoamTime = SEnvir.Now.AddMilliseconds(SEnvir.Random.Next((int)RoamDelay.TotalMilliseconds));

            ActionTime = SEnvir.Now.AddSeconds(1);

            Level = MonsterInfo.Level;

            CoolEye = SEnvir.Random.Next(100) < MonsterInfo.CoolEye;

            AddAllObjects();

            Activate();
        }
        /// <summary>
        /// 宠物经验
        /// </summary>
        /// <param name="amount"></param>
        public void PetExp(decimal amount)
        {
            //如果是投石车或者弩车直接跳出
            if (MonsterInfo.Image == MonsterImage.Catapult || MonsterInfo.Image == MonsterImage.Ballista) return;

            if (MonsterInfo.Flag == MonsterFlag.Shinsu)
            {
                if (SummonLevel >= PetOwner.Magics[MagicType.SummonShinsu].Level * 2 + 1)
                    return;
            }
            else if (MonsterInfo.Flag == MonsterFlag.Skeleton)
            {
                if (SummonLevel >= PetOwner.Magics[MagicType.SummonSkeleton].Level * 2 + 1)
                    return;
            }

            else if (MonsterInfo.Flag == MonsterFlag.JinSkeleton)
            {
                if (SummonLevel >= PetOwner.Magics[MagicType.SummonJinSkeleton].Level * 2 + 1)
                    return;
            }

            //如果宝宝等于大于或等于最大等级 跳出  3-7级的判断
            if (SummonLevel >= MaxSummonLevel) return;
            //如果怪物的标签是 炎魔 神兽 骷髅 变异骷髅  数值*=3
            if (MonsterInfo.Flag == MonsterFlag.InfernalSoldier || MonsterInfo.Flag == MonsterFlag.Shinsu || MonsterInfo.Flag == MonsterFlag.Skeleton || MonsterInfo.Flag == MonsterFlag.JinSkeleton)
                amount *= 1;
            ////宝宝经验+=数值*=1
            PetExperience += amount;

            //如果宝宝经验小于（宝宝等级+1）*设置的宝宝经验值 跳出
            if (PetExperience < (SummonLevel + 1) * Config.UpgradePetExe) return;
            //宝宝经验=宝宝经验-（（宝宝等级+1）*设置的宝宝经验值）
            PetExperience -= (SummonLevel + 1) * Config.UpgradePetExe;
            //宝宝等级升级
            SummonLevel++;
            //刷新宝宝的属性状态
            RefreshStats();
        }
        /// <summary>
        /// 刷新怪物的属性状态数值
        /// </summary>
        public override void RefreshStats()
        {
            base.RefreshStats();

            Stats.Clear();
            Stats.Add(MonsterInfo.Stats);

            ApplyBonusStats();

            // 刷新怪物移速攻速的基础值
            MoveDelayBase = MonsterInfo.MoveDelay;
            AttackDelayBase = MonsterInfo.AttackDelay;

            if (!Config.UpgradePetAdd)
            {
                if (SummonLevel > 0)   //宝宝等级加的属性
                {
                    Stats[Stat.Health] += Stats[Stat.Health] * SummonLevel / 10;

                    Stats[Stat.MinAC] += Stats[Stat.MinAC] * SummonLevel / 10;
                    Stats[Stat.MaxAC] += Stats[Stat.MaxAC] * SummonLevel / 10;

                    Stats[Stat.MinMR] += Stats[Stat.MinMR] * SummonLevel / 10;
                    Stats[Stat.MaxMR] += Stats[Stat.MaxMR] * SummonLevel / 10;

                    Stats[Stat.MinDC] += Stats[Stat.MinDC] * SummonLevel / 10;
                    Stats[Stat.MaxDC] += Stats[Stat.MaxDC] * SummonLevel / 10;

                    Stats[Stat.MinMC] += Stats[Stat.MinMC] * SummonLevel / 10;
                    Stats[Stat.MaxMC] += Stats[Stat.MaxMC] * SummonLevel / 10;

                    Stats[Stat.MinSC] += Stats[Stat.MinSC] * SummonLevel / 10;
                    Stats[Stat.MaxSC] += Stats[Stat.MaxSC] * SummonLevel / 10;

                    //如果怪物的标签是 炎魔 神兽 骷髅 变异骷髅  数值*=3
                    if (MonsterInfo.Flag == MonsterFlag.InfernalSoldier || MonsterInfo.Flag == MonsterFlag.Shinsu || MonsterInfo.Flag == MonsterFlag.Skeleton || MonsterInfo.Flag == MonsterFlag.JinSkeleton)
                    {
                        Stats[Stat.Accuracy] += Stats[Stat.Accuracy] * SummonLevel / 10;
                        Stats[Stat.Agility] += Stats[Stat.Agility] * SummonLevel / 10;
                    }
                    else
                    {
                        Stats[Stat.Accuracy] += Stats[Stat.Accuracy] * SummonLevel / 20;
                        Stats[Stat.Agility] += Stats[Stat.Agility] * SummonLevel / 20;
                    }
                }
            }
            else
            {
                if (SummonLevel > 0)   //宝宝等级加的属性
                {
                    if (MonsterInfo.Flag == MonsterFlag.Skeleton)   //骷髅
                    {
                        Stats[Stat.Health] = (4 + (SummonLevel + 1) * ((SummonLevel + 1) + 1) / 2) * 20;

                        Stats[Stat.MaxDC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MaxMC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MaxSC] += SummonLevel * 1 * 120 / 100;

                        Stats[Stat.MaxAC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MaxMR] += SummonLevel * 1 * 120 / 100;
                    }

                    if (MonsterInfo.Flag == MonsterFlag.Shinsu)   //神兽
                    {
                        Stats[Stat.Health] = (4 + (SummonLevel + 1) * ((SummonLevel + 1) + 1) / 2) * 20;

                        Stats[Stat.MinDC] += SummonLevel * 1;
                        Stats[Stat.MaxDC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MinMC] += SummonLevel * 1;
                        Stats[Stat.MaxMC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MinSC] += SummonLevel * 1;
                        Stats[Stat.MaxSC] += SummonLevel * 1 * 120 / 100;

                        Stats[Stat.MaxAC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MaxMR] += SummonLevel * 1 * 120 / 100;
                    }

                    if (MonsterInfo.Flag == MonsterFlag.JinSkeleton || MonsterInfo.Flag == MonsterFlag.InfernalSoldier)   //强排 焰魔
                    {
                        Stats[Stat.Health] = (4 + (SummonLevel + 1) * ((SummonLevel + 1) + 1) / 2) * 36;

                        Stats[Stat.MaxDC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MaxMC] += SummonLevel * 1 * 120 / 100;
                        Stats[Stat.MaxSC] += SummonLevel * 1 * 120 / 100;

                        Stats[Stat.MaxAC] += SummonLevel * 2 * 120 / 100;
                        Stats[Stat.MaxMR] += SummonLevel * 2 * 120 / 100;
                    }

                    //诱惑的宝宝  或者是召唤的宝宝
                    if (MonsterInfo.Flag != MonsterFlag.InfernalSoldier && MonsterInfo.Flag != MonsterFlag.Shinsu && MonsterInfo.Flag != MonsterFlag.Skeleton && MonsterInfo.Flag != MonsterFlag.JinSkeleton)
                    {
                        Stats[Stat.Health] = Stats[Stat.Health] + (int)(Stats[Stat.Health] / 6.7) * SummonLevel;

                        Stats[Stat.MaxDC] += SummonLevel * Stats[Stat.MaxDC] * 3 / 100;
                        Stats[Stat.MaxMC] += SummonLevel * Stats[Stat.MaxMC] * 3 / 100;
                        Stats[Stat.MaxSC] += SummonLevel * Stats[Stat.MaxSC] * 3 / 100;

                        Stats[Stat.MaxAC] += SummonLevel * Stats[Stat.MaxAC] * 12 / 100;
                        Stats[Stat.MaxMR] += SummonLevel * Stats[Stat.MaxMR] * 12 / 100;
                    }

                    //if (MonsterInfo.Flag == MonsterFlag.InfernalSoldier || MonsterInfo.Flag == MonsterFlag.Shinsu || MonsterInfo.Flag == MonsterFlag.Skeleton || MonsterInfo.Flag == MonsterFlag.JinSkeleton)
                    //{
                    //    Stats[Stat.MinAC] += SummonLevel * Config.PetMinAC;
                    //    Stats[Stat.MaxAC] += SummonLevel * Config.PetMaxAC;

                    //    Stats[Stat.MinMR] += SummonLevel * Config.PetMinMR;
                    //    Stats[Stat.MaxMR] += SummonLevel * Config.PetMaxMR;

                    //    Stats[Stat.MinDC] += SummonLevel * Config.PetMinDC;
                    //    Stats[Stat.MaxDC] += SummonLevel * Config.PetMaxDC;

                    //    Stats[Stat.MinMC] += SummonLevel * Config.PetMinMC;
                    //    Stats[Stat.MaxMC] += SummonLevel * Config.PetMaxMC;

                    //    Stats[Stat.MinSC] += SummonLevel * Config.PetMinSC;
                    //    Stats[Stat.MaxSC] += SummonLevel * Config.PetMaxSC;
                    //}

                    //如果怪物的标签是 炎魔 神兽 骷髅 变异骷髅  数值*=3
                    //if (MonsterInfo.Flag == MonsterFlag.InfernalSoldier || MonsterInfo.Flag == MonsterFlag.Shinsu || MonsterInfo.Flag == MonsterFlag.Skeleton || MonsterInfo.Flag == MonsterFlag.JinSkeleton)
                    //{
                    //    Stats[Stat.Accuracy] += Stats[Stat.Accuracy] * SummonLevel / 10;
                    //    Stats[Stat.Agility] += Stats[Stat.Agility] * SummonLevel / 10;
                    //}
                    //else
                    //{
                    //    Stats[Stat.Accuracy] += Stats[Stat.Accuracy] * SummonLevel / 20;
                    //    Stats[Stat.Agility] += Stats[Stat.Agility] * SummonLevel / 20;
                    //}
                }
            }

            Stats[Stat.CriticalChance] = 1;

            if (Buffs.Any(x => x.Type == BuffType.MagicWeakness))
            {
                Stats[Stat.MinMR] = 0;
                Stats[Stat.MaxMR] = 0;
            }

            if (Buffs.Any(x => x.Type == BuffType.PierceBuff))
            {
                Stats[Stat.FireResistance] = 0;
                Stats[Stat.IceResistance] = 0;
                Stats[Stat.LightningResistance] = 0;
                Stats[Stat.WindResistance] = 0;
                Stats[Stat.HolyResistance] = 0;
                Stats[Stat.DarkResistance] = 0;
                Stats[Stat.PhantomResistance] = 0;
            }

            if (Buffs.Any(x => x.Type == BuffType.BurnBuff))
            {
                Stats[Stat.MinDC] = Stats[Stat.MinDC] / 2;
                Stats[Stat.MaxDC] = Stats[Stat.MaxDC] / 2;
                Stats[Stat.MinMC] = Stats[Stat.MinMC] / 2;
                Stats[Stat.MaxMC] = Stats[Stat.MaxMC] / 2;
                Stats[Stat.MinSC] = Stats[Stat.MinSC] / 2;
                Stats[Stat.MaxSC] = Stats[Stat.MaxSC] / 2;
            }

            foreach (BuffInfo buff in Buffs)
            {
                if (buff.Stats == null) continue;
                Stats.Add(buff.Stats);
            }

            if (PetOwner != null && PetOwner.Stats[Stat.PetDCPercent] > 0)
            {
                Stats[Stat.MinDC] += Stats[Stat.MinDC] * PetOwner.Stats[Stat.PetDCPercent] / 100;
                Stats[Stat.MaxDC] += Stats[Stat.MaxDC] * PetOwner.Stats[Stat.PetDCPercent] / 100;

                foreach (UserMagic magic in Magics)
                {
                    switch (magic.Info.Magic)
                    {
                        case MagicType.DemonicRecovery:
                            Stats[Stat.Health] += (magic.Level + 1) * 300;
                            break;
                    }
                }
            }

            if (PetOwner != null && PetOwner.Stats[Stat.PetMCPercent] > 0)
            {
                Stats[Stat.MinAC] += Stats[Stat.MinAC] * PetOwner.Stats[Stat.PetMCPercent] / 100;
                Stats[Stat.MaxAC] += Stats[Stat.MaxAC] * PetOwner.Stats[Stat.PetMCPercent] / 100;

                Stats[Stat.MinMR] += Stats[Stat.MinMR] * PetOwner.Stats[Stat.PetMCPercent] / 100;
                Stats[Stat.MaxMR] += Stats[Stat.MaxMR] * PetOwner.Stats[Stat.PetMCPercent] / 100;
            }

            /*
            Stats[Stat.FireResistance] = Math.Min(5, Stats[Stat.FireResistance]);
            Stats[Stat.IceResistance] = Math.Min(5, Stats[Stat.IceResistance]);
            Stats[Stat.LightningResistance] = Math.Min(5, Stats[Stat.LightningResistance]);
            Stats[Stat.WindResistance] = Math.Min(5, Stats[Stat.WindResistance]);
            Stats[Stat.HolyResistance] = Math.Min(5, Stats[Stat.HolyResistance]);
            Stats[Stat.DarkResistance] = Math.Min(5, Stats[Stat.DarkResistance]);
            Stats[Stat.PhantomResistance] = Math.Min(5, Stats[Stat.PhantomResistance]);
            */

            Stats[Stat.Health] += (int)(Stats[Stat.Health] * (Stats[Stat.HealthPercent] / 100));  //血量%
            Stats[Stat.Mana] += (int)(Stats[Stat.Mana] * (Stats[Stat.ManaPercent] / 100));   //蓝量%

            Stats[Stat.MinAC] += (int)(Stats[Stat.MinAC] * (Stats[Stat.ACPercent] / 100));   //物防%
            Stats[Stat.MaxAC] += (int)(Stats[Stat.MaxAC] * (Stats[Stat.ACPercent] / 100));

            Stats[Stat.MinMR] += (int)(Stats[Stat.MinMR] * (Stats[Stat.MRPercent] / 100));   //魔防%
            Stats[Stat.MaxMR] += (int)(Stats[Stat.MaxMR] * (Stats[Stat.MRPercent] / 100));

            Stats[Stat.MinDC] += (int)(Stats[Stat.MinDC] * (Stats[Stat.DCPercent] / 100));   //攻击%
            Stats[Stat.MaxDC] += (int)(Stats[Stat.MaxDC] * (Stats[Stat.DCPercent] / 100));

            Stats[Stat.MinMC] += (int)(Stats[Stat.MinMC] * (Stats[Stat.MCPercent] / 100));   //自然%
            Stats[Stat.MaxMC] += (int)(Stats[Stat.MaxMC] * (Stats[Stat.MCPercent] / 100));

            Stats[Stat.MinSC] += (int)(Stats[Stat.MinSC] * (Stats[Stat.SCPercent] / 100));   //灵魂%
            Stats[Stat.MaxSC] += (int)(Stats[Stat.MaxSC] * (Stats[Stat.SCPercent] / 100));

            if (PetOwner == null && CurrentMap != null)  //不是宠物  是对应得地图
            {
                Stats[Stat.Health] += (int)(Stats[Stat.Health] * (MapHealthRate / 100.0f));  //加血

                Stats[Stat.MinDC] += (int)(Stats[Stat.MinDC] * (MapDamageRate / 100.0f));    //加攻击
                Stats[Stat.MaxDC] += (int)(Stats[Stat.MaxDC] * (MapDamageRate / 100.0f));
                Stats[Stat.MinMC] += (int)(Stats[Stat.MinMC] * (MapDamageRate / 100.0f));
                Stats[Stat.MaxMC] += (int)(Stats[Stat.MaxMC] * (MapDamageRate / 100.0f));

                Stats[Stat.MinAC] += (int)(Stats[Stat.MinAC] * (MapBewareRate / 100.0f));    //加防魔
                Stats[Stat.MaxAC] += (int)(Stats[Stat.MaxAC] * (MapBewareRate / 100.0f));
                Stats[Stat.MinMR] += (int)(Stats[Stat.MinMR] * (MapBewareRate / 100.0f));
                Stats[Stat.MaxMR] += (int)(Stats[Stat.MaxMR] * (MapBewareRate / 100.0f));
            }

            Stats[Stat.Health] = Math.Max(1, Stats[Stat.Health]);
            Stats[Stat.Mana] = Math.Max(1, Stats[Stat.Mana]);

            Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC]);
            Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC]);
            Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR]);
            Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR]);
            Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC]);
            Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC]);
            Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC]);
            Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC]);

            //如果怪物级别大于1级 且怪物的等级小于或者等于设置的等级 且 不是BOSS
            if (MonsterInfo.Level > 1 && MonsterInfo.Level <= Config.MonLvMin1 && !MonsterInfo.IsBoss)
            {
                Stats[Stat.Health] = Math.Max(0, Stats[Stat.Health] * (Config.HPDifficulty1 / 100));
                Stats[Stat.Mana] = Math.Max(0, Stats[Stat.Mana] * (Config.HPDifficulty1 / 100));

                Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC] * (Config.ACDifficulty1 / 100));
                Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC] * (Config.ACDifficulty1 / 100));
                Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR] * (Config.ACDifficulty11 / 100));
                Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR] * (Config.ACDifficulty11 / 100));
                Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC] * (Config.PWDifficulty1 / 100));
                Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC] * (Config.PWDifficulty1 / 100));
                Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC] * (Config.PWDifficulty1 / 100));
                Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC] * (Config.PWDifficulty1 / 100));
                Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC] * (Config.PWDifficulty1 / 100));
                Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC] * (Config.PWDifficulty1 / 100));
            }

            //如果怪物级别大于1级 且怪物的等级小于或者等于设置的等级 且 不是BOSS
            if (MonsterInfo.Level > Config.MonLvMin1 && MonsterInfo.Level <= Config.MonLvMin2 && !MonsterInfo.IsBoss)
            {
                Stats[Stat.Health] = Math.Max(0, Stats[Stat.Health] * (Config.HPDifficulty2 / 100));
                Stats[Stat.Mana] = Math.Max(0, Stats[Stat.Mana] * (Config.HPDifficulty2 / 100));

                Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC] * (Config.ACDifficulty2 / 100));
                Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC] * (Config.ACDifficulty2 / 100));
                Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR] * (Config.ACDifficulty22 / 100));
                Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR] * (Config.ACDifficulty22 / 100));
                Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC] * (Config.PWDifficulty2 / 100));
                Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC] * (Config.PWDifficulty2 / 100));
                Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC] * (Config.PWDifficulty2 / 100));
                Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC] * (Config.PWDifficulty2 / 100));
                Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC] * (Config.PWDifficulty2 / 100));
                Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC] * (Config.PWDifficulty2 / 100));
            }

            //如果怪物级别大于1级 且怪物的等级小于或者等于设置的等级 且 不是BOSS
            if (MonsterInfo.Level > Config.MonLvMin2 && MonsterInfo.Level <= Config.MonLvMin3 && !MonsterInfo.IsBoss)
            {
                Stats[Stat.Health] = Math.Max(0, Stats[Stat.Health] * (Config.HPDifficulty3 / 100));
                Stats[Stat.Mana] = Math.Max(0, Stats[Stat.Mana] * (Config.HPDifficulty3 / 100));

                Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC] * (Config.ACDifficulty3 / 100));
                Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC] * (Config.ACDifficulty3 / 100));
                Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR] * (Config.ACDifficulty33 / 100));
                Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR] * (Config.ACDifficulty33 / 100));
                Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC] * (Config.PWDifficulty3 / 100));
                Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC] * (Config.PWDifficulty3 / 100));
                Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC] * (Config.PWDifficulty3 / 100));
                Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC] * (Config.PWDifficulty3 / 100));
                Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC] * (Config.PWDifficulty3 / 100));
                Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC] * (Config.PWDifficulty3 / 100));
            }

            //如果怪物级别大于1级 且怪物的等级小于或者等于设置的等级 且 是BOSS
            if (MonsterInfo.Level > 1 && MonsterInfo.Level <= Config.BOSSMonLvMin1 && MonsterInfo.IsBoss)
            {
                Stats[Stat.Health] = Math.Max(0, Stats[Stat.Health] * (Config.BOSSHPDifficulty1 / 100));
                Stats[Stat.Mana] = Math.Max(0, Stats[Stat.Mana] * (Config.BOSSHPDifficulty1 / 100));

                Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC] * (Config.BOSSACDifficulty1 / 100));
                Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC] * (Config.BOSSACDifficulty1 / 100));
                Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR] * (Config.BOSSACDifficulty11 / 100));
                Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR] * (Config.BOSSACDifficulty11 / 100));
                Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC] * (Config.BOSSPWDifficulty1 / 100));
                Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC] * (Config.BOSSPWDifficulty1 / 100));
                Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC] * (Config.BOSSPWDifficulty1 / 100));
                Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC] * (Config.BOSSPWDifficulty1 / 100));
                Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC] * (Config.BOSSPWDifficulty1 / 100));
                Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC] * (Config.BOSSPWDifficulty1 / 100));
            }

            //如果怪物级别大于1级 且怪物的等级小于或者等于设置的等级 且 是BOSS
            if (MonsterInfo.Level > Config.BOSSMonLvMin1 && MonsterInfo.Level <= Config.BOSSMonLvMin2 && MonsterInfo.IsBoss)
            {
                Stats[Stat.Health] = Math.Max(0, Stats[Stat.Health] * (Config.BOSSHPDifficulty2 / 100));
                Stats[Stat.Mana] = Math.Max(0, Stats[Stat.Mana] * (Config.BOSSHPDifficulty2 / 100));

                Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC] * (Config.BOSSACDifficulty2 / 100));
                Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC] * (Config.BOSSACDifficulty2 / 100));
                Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR] * (Config.BOSSACDifficulty22 / 100));
                Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR] * (Config.BOSSACDifficulty22 / 100));
                Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC] * (Config.BOSSPWDifficulty2 / 100));
                Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC] * (Config.BOSSPWDifficulty2 / 100));
                Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC] * (Config.BOSSPWDifficulty2 / 100));
                Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC] * (Config.BOSSPWDifficulty2 / 100));
                Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC] * (Config.BOSSPWDifficulty2 / 100));
                Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC] * (Config.BOSSPWDifficulty2 / 100));
            }

            //如果怪物级别大于1级 且怪物的等级小于或者等于设置的等级 且 是BOSS
            if (MonsterInfo.Level > Config.BOSSMonLvMin2 && MonsterInfo.Level <= Config.BOSSMonLvMin3 && MonsterInfo.IsBoss)
            {
                Stats[Stat.Health] = Math.Max(0, Stats[Stat.Health] * (Config.BOSSHPDifficulty3 / 100));
                Stats[Stat.Mana] = Math.Max(0, Stats[Stat.Mana] * (Config.BOSSHPDifficulty3 / 100));

                Stats[Stat.MinAC] = Math.Max(0, Stats[Stat.MinAC] * (Config.BOSSACDifficulty3 / 100));
                Stats[Stat.MaxAC] = Math.Max(0, Stats[Stat.MaxAC] * (Config.BOSSACDifficulty3 / 100));
                Stats[Stat.MinMR] = Math.Max(0, Stats[Stat.MinMR] * (Config.BOSSACDifficulty33 / 100));
                Stats[Stat.MaxMR] = Math.Max(0, Stats[Stat.MaxMR] * (Config.BOSSACDifficulty33 / 100));
                Stats[Stat.MinDC] = Math.Max(0, Stats[Stat.MinDC] * (Config.BOSSPWDifficulty3 / 100));
                Stats[Stat.MaxDC] = Math.Max(0, Stats[Stat.MaxDC] * (Config.BOSSPWDifficulty3 / 100));
                Stats[Stat.MinMC] = Math.Max(0, Stats[Stat.MinMC] * (Config.BOSSPWDifficulty3 / 100));
                Stats[Stat.MaxMC] = Math.Max(0, Stats[Stat.MaxMC] * (Config.BOSSPWDifficulty3 / 100));
                Stats[Stat.MinSC] = Math.Max(0, Stats[Stat.MinSC] * (Config.BOSSPWDifficulty3 / 100));
                Stats[Stat.MaxSC] = Math.Max(0, Stats[Stat.MaxSC] * (Config.BOSSPWDifficulty3 / 100));
            }

            Stats[Stat.MinDC] = Math.Min(Stats[Stat.MinDC], Stats[Stat.MaxDC]);
            Stats[Stat.MinMC] = Math.Min(Stats[Stat.MinMC], Stats[Stat.MaxMC]);
            Stats[Stat.MinSC] = Math.Min(Stats[Stat.MinSC], Stats[Stat.MaxSC]);

            if (EasterEventMob)
                Stats[Stat.Health] = 1;

            if (ChristmasEventMob)
                Stats[Stat.Health] = 10;

            S.DataObjectMaxHealthMana p = new S.DataObjectMaxHealthMana { ObjectID = ObjectID, Stats = Stats };

            foreach (PlayerObject player in DataSeenByPlayers)
                player.Enqueue(p);


            if (CurrentHP > Stats[Stat.Health]) SetHP(Stats[Stat.Health]);
            if (CurrentMP > Stats[Stat.Mana]) SetMP(Stats[Stat.Mana]);
        }
        /// <summary>
        /// 得到额外的属性状态
        /// </summary>
        public virtual void ApplyBonusStats() { }

        /// <summary>
        /// 清理
        /// </summary>
        public override void CleanUp()
        {
            base.CleanUp();

            _Target = null;

            SpawnList?.Clear();
            SpawnList = null;

            _EXPOwner = null;

            Drops?.Clear();

            Magics?.Clear();

            TempVariables?.Clear();
            TempVariables = null;
        }
        /// <summary>
        /// 激活
        /// </summary>
        public override void Activate()
        {
            if (Activated) return;

            if (NearByPlayers.Count == 0 && MonsterInfo.ViewRange <= Config.MaxViewRange && !MonsterInfo.IsBoss && PetOwner == null) return;

            Activated = true;
            SEnvir.ActiveObjects.Add(this);
        }
        /// <summary>
        /// 停用
        /// </summary>
        public override void DeActivate()
        {
            if (!Activated) return;

            if (NearByPlayers.Count > 0 || MonsterInfo.ViewRange > Config.MaxViewRange || Target != null || MonsterInfo.IsBoss || PetOwner != null || ActionList.Count > 0 || CurrentHP < Stats[Stat.Health]) return;

            Activated = false;
            SEnvir.ActiveObjects.Remove(this);
        }
        /// <summary>
        /// 行为过程操作
        /// </summary>
        /// <param name="action"></param>
        public override void ProcessAction(DelayedAction action)
        {
            switch (action.Type)
            {
                case ActionType.DelayAttack:
                    Attack((MapObject)action.Data[0], (int)action.Data[1], (Element)action.Data[2]);
                    return;
                case ActionType.DelayMagic:
                    switch ((MagicType)action.Data[0])
                    {
                        case MagicType.FireWall:
                            FireWallEnd((Cell)action.Data[1]);
                            break;
                        case MagicType.DragonRepulse:
                            DragonRepulseEnd((MapObject)action.Data[1]);
                            break;
                        case MagicType.Purification:
                            Purify((MapObject)action.Data[1]);
                            break;
                        case MagicType.MonsterDeathCloud:
                            DeathCloudEnd((Cell)action.Data[1], (bool)action.Data[2], (Point)action.Data[3]);
                            break;
                    }
                    break;
            }

            base.ProcessAction(action);
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (Dead)
            {
                Target = null;

                if (SEnvir.Now > DeadTime)
                {
                    Despawn();
                    return;
                }
            }

            if (Target?.Node == null || Target.Dead || Target.CurrentMap != CurrentMap || !Functions.InRange(CurrentLocation, Target.CurrentLocation, Config.MaxViewRange) ||
               ((Poison & PoisonType.Abyss) == PoisonType.Abyss && !Functions.InRange(CurrentLocation, Target.CurrentLocation, ViewRange)) || !CanAttackTarget(Target))
                Target = null;

            if (Target != null && Target.Buffs.Any(x => x.Type == BuffType.Cloak) && !Functions.InRange(CurrentLocation, Target.CurrentLocation, 2) && Stats[Stat.IgnoreStealth] == 0)
                Target = null;

            if (Target != null && Target.Buffs.Any(x => x.Type == BuffType.Transparency))
                Target = null;

            ProcessAI();
        }
        /// <summary>
        /// 怪物名字颜色改变
        /// </summary>
        public override void ProcessNameColour()
        {
            NameColour = Color.White;

            switch (SummonLevel)    //宝宝等级 不同级别颜色
            {
                case 1:
                    NameColour = Color.Aqua;
                    break;
                case 2:
                    NameColour = Color.Aquamarine;
                    break;
                case 3:
                    NameColour = Color.LightSeaGreen;
                    break;
                case 4:
                    NameColour = Color.SlateBlue;
                    break;
                case 5:
                    NameColour = Color.SteelBlue;
                    break;
                case 6:
                    NameColour = Color.Navy;
                    break;
                case 7:
                    NameColour = Color.Blue;
                    break;
            }

            if (SEnvir.Now < ShockTime)
                NameColour = Color.Peru;
            else if (SEnvir.Now < RageTime)
                NameColour = Color.Red;
        }
        /// <summary>
        /// AI过程
        /// </summary>
        public virtual void ProcessAI()
        {
            if (Dead) return;

            // 处理py脚本的AI
            if (SEnvir.Now > PyAITime)
            {
                // 2秒检查一次 不能太快
                PyAITime = SEnvir.Now.AddSeconds(3);
                ProcessPyAISetting();
            }

            if (PetOwner?.Node != null)
            {
                if (Target != null)
                {
                    if (PetOwner.PetMode == PetMode.PvP && Target.Race != ObjectType.Player)
                        Target = null;

                    if (PetOwner.PetMode == PetMode.None || PetOwner.PetMode == PetMode.Move)
                        Target = null;
                }
                if (SEnvir.Now > TameTime)
                    UnTame();
                else if (Visible && !PetOwner.VisibleObjects.Contains(this) && (PetOwner.PetMode == PetMode.Both || PetOwner.PetMode == PetMode.Move || PetOwner.PetMode == PetMode.PvP))

                    PetRecall(true);
            }

            ProcessRegen();
            ProcessSearch();
            ProcessRoam();
            ProcessTarget();

        }
        /// <summary>
        /// 开启所保护的时候刷新仆从
        /// </summary>
        public override void OnSafeDespawn()
        {
            base.OnSafeDespawn();

            Master?.MinionList.Remove(this);
            Master = null;

            PetOwner?.Pets.Remove(this);
            PetOwner = null;

            if (MinionList != null)
            {
                for (int i = MinionList.Count - 1; i >= 0; i--)
                    MinionList[i].Master = null;

                MinionList.Clear();
            }

            if (SpawnInfo != null)
                SpawnInfo.AliveCount--;

            ProcessEvents();

            SpawnInfo = null;

            EXPOwner = null;
        }
        /// <summary>
        /// 宠物叛变，取消主人标记
        /// </summary>
        public void UnTame()
        {
            PetOwner?.Pets.Remove(this);          //宠物主人移除
            PetOwner = null;                      //宠物主人等空
            Target = null;                        //攻击目标等空
            SearchTime = DateTime.MinValue;       //搜索时间等最小值
            Magics.Clear();                       //技能清除
            SummonLevel = 0;                      //宠物的等级为0
            RefreshStats();                       //刷新属性

            if (MonsterInfo.Image == MonsterImage.Catapult || MonsterInfo.Image == MonsterImage.Ballista)
                SetHP(0);
            else
                //设置HP为原始HP的十分之一血量
                SetHP(Math.Min(CurrentHP, Stats[Stat.Health] / 10));
            //发送封包宠物主人改变
            Broadcast(new S.ObjectPetOwnerChanged { ObjectID = ObjectID });
        }
        /// <summary>
        /// 宠物召唤
        /// </summary>
        public void PetRecall(bool isPassiv = false)
        {
            //如果宠物主人为空直接跳出
            if (PetOwner?.CurrentMap == null) return;
            if ((MonsterInfo.Image == MonsterImage.Catapult || MonsterInfo.Image == MonsterImage.Ballista) && PetOwner.CurrentMap == CurrentMap) return;

            Cell targetCell = null;

            if (isPassiv)
            {
                //人物周边1格找空位
                for (int i = 0; i < 8; i++)
                {
                    targetCell = PetOwner.CurrentMap.GetCell(Functions.Move(PetOwner.CurrentLocation, (MirDirection)i));
                    if (targetCell != null && targetCell.Movements == null) break;
                }
            }
            else
            {
                //人物周边1格找空位
                //for (int i = 0; i < 8; i++)
                //{
                targetCell = PetOwner.CurrentMap.GetCell(Functions.Move(PetOwner.CurrentLocation, (MirDirection)PetOwner.Direction));
                // if (targetCell != null && targetCell.Movements == null)
                // {
                //  break;
                // }
                //}
            }

            // 如果 单元格为空
            if (targetCell == null)
            {
                //单元格 = 宠物主人当前地图的范围6格内随机坐标
                if (SEnvir.Random.Next(100) < Config.SummonRandomValue)
                    targetCell = PetOwner.CurrentMap.GetCell(CurrentMap.GetRandomLocation(PetOwner.CurrentLocation, 12));
                else
                    targetCell = PetOwner.CurrentMap.GetCell(CurrentMap.GetRandomLocation(PetOwner.CurrentLocation, 6));
            }

            //正常的话
            if (targetCell != null)
            {
                Teleport(PetOwner.CurrentMap, targetCell.Location);
            }
            //else
            //{
            //    //这里或许可以让宠物传送到人物位置 与人物重合
            //    SEnvir.Log($"处理宠物跟随出错，无法找到空闲位置。玩家{PetOwner.Name}");
            //}

        }
        /// <summary>
        /// 怪物回血处理
        /// </summary>
        public virtual void ProcessRegen()
        {
            if (SEnvir.Now < RegenTime) return;

            RegenTime = SEnvir.Now + RegenDelay;

            if (CurrentHP >= Stats[Stat.Health]) return;

            int regen = (int)Math.Max(1, Stats[Stat.Health] * 0.02F); //2% 大约每10秒

            ChangeHP(regen);
        }
        /// <summary>
        /// 搜索过程
        /// </summary>
        public virtual void ProcessSearch()
        {
            if (CurrentMap?.Players.Count == 0)
            {
                return;
            }
            if (PetOwner != null)   //如果 宠物主人 不等 空
            {
                ProperSearch();  //跳转 正确搜索
                return;          //返回
            }

            if (Target != null)   //如果 目标 不等 空
            {
                if (!Visible)   //如果 不可见
                {
                    if (SEnvir.Now < SearchTime) return;  //如果 系统时间小于搜索时间 返回
                }
                else if (!CanMove && !CanAttack) return;  //否则 如果 不可以移动 并且 不可以攻击 返回

                //当前已经有攻击目标了 十秒未攻击 则丢失仇恨
                if (MonsterInfo.AI != 18 && Config.MonHatred)  //如果 怪物的AI不是定义为18 爆裂蜘蛛的AI  并且模式是韩版脱怪模式
                {
                    UInt32 nTick = (UInt32)Environment.TickCount - _TargetAttackTick;  //定义nTick = 环境时间 - 目标攻击时间

                    if (nTick < (Config.MonHatredTime * 1000) && CanAttackTarget(Target)) return;  //如果 定义时间小于 4000 并且 可以攻击目标（目标） 返回

                    //Target = null;   //目标等空
                    //return;      //返回
                }
            }
            else if (SEnvir.Now < SearchTime) return;  //否则 如果  系统时间小于搜索时间 或 当前地图玩家计数等0  返回

            SearchTime = SEnvir.Now + SearchDelay;  //搜索时间=系统时间+搜索目标延迟定义

            int bestDistance = int.MaxValue;
            List<MapObject> closest = new List<MapObject>();
            List<MapObject> closestA = new List<MapObject>();

            //保存资源
            foreach (PlayerObject player in CurrentMap.Players)
            {
                int distance;
                for (int i = player.Pets.Count - 1; i >= 0; i--)
                {
                    if (player.Pets[i] == null) continue;
                    MonsterObject pet = player.Pets[i];

                    if (pet.CurrentMap != CurrentMap) continue;

                    distance = Functions.Distance(pet.CurrentLocation, CurrentLocation);

                    if (distance > ViewRange) continue;

                    if (distance > bestDistance || !ShouldAttackTarget(pet)) continue;

                    if (distance != bestDistance) closest.Clear();

                    closest.Add(pet);
                    bestDistance = distance;
                }

                distance = Functions.Distance(player.CurrentLocation, CurrentLocation);

                if (distance > ViewRange) continue;
                if (distance > bestDistance || !ShouldAttackTarget(player)) continue;
                //如果是妙影，怪物延迟搜寻目前1500毫秒
                //if (player.TraOverTime.AddMilliseconds(Config.TraOverTime) > SEnvir.Now) continue;

                if (distance != bestDistance) closest.Clear();

                closest.Add(player);
                bestDistance = distance;
            }
            //bestDistance = int.MaxValue;
            foreach (var pet in CurrentMap.Pets)
            {
                int distance;
                if (pet.PetOwner == null) continue;
                if (pet.CurrentMap != CurrentMap) continue;
                distance = Functions.Distance(pet.CurrentLocation, CurrentLocation);
                if (distance > ViewRange) continue;
                if (distance > bestDistance || !ShouldAttackTarget(pet)) continue;
                if (distance != bestDistance) closest.Clear();
                closest.Add(pet);
                bestDistance = distance;
            }

            if (closest.Count == 0) return;
            if (!closest.Contains(Target))
            {
                var temTarget = closest[SEnvir.Random.Next(closest.Count)];
                if (temTarget != Target)
                {
                    int[] timer = { 0, 1200, 1250 };
                    AttackFirstDelay = SEnvir.Now.AddMilliseconds(timer[SEnvir.Random.Next(3)]);
                }
                Target = temTarget;
            }

        }
        /// <summary>
        /// 正确的搜索
        /// </summary>
        public void ProperSearch()
        {
            if (Target != null)
            {
                if (!CanMove && !CanAttack && Visible) return;
            }
            else
            {
                if (SEnvir.Now < SearchTime) return;

                SearchTime = SEnvir.Now + SearchDelay;

                if (CurrentMap.Players.Count == 0 && !HalloweenEventMob) return;
            }

            for (int d = 0; d <= ViewRange; d++)
            {
                List<MapObject> closest = new List<MapObject>();
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.Cells[x, y];

                        if (cell?.Objects == null) continue;

                        foreach (MapObject ob in cell.Objects)
                        {
                            if (!ShouldAttackTarget(ob)) continue;

                            closest.Add(ob);
                        }
                    }
                }
                if (closest.Count == 0) continue;

                Target = closest[SEnvir.Random.Next(closest.Count)];

                return;
            }
        }
        /// <summary>
        /// 怪物巡逻
        /// </summary>
        public virtual void ProcessRoam()
        {
            if ((SeenByPlayers.Count == 0 && (PetOwner != null && (MonsterInfo.Image != MonsterImage.Catapult && MonsterInfo.Image != MonsterImage.Ballista))) || SEnvir.Now < RoamTime) return;

            if (SeenByPlayers.Count == 1 && SeenByPlayers[0].Character.Account.TempAdmin) return;  //如果只有GM一人时 不做移动

            if (!CanMove) return;

            if (PetOwner != null)
            {
                if (Target == null)
                    if (MonsterInfo.Image != MonsterImage.Catapult && MonsterInfo.Image != MonsterImage.Ballista)
                        MoveTo(Functions.Move(PetOwner.CurrentLocation, PetOwner.Direction, -1));
                    else
                    {
                        MirDirection direction = Functions.DirectionFromPoint(CurrentLocation, PetOwner.CurrentLocation);
                        int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;
                        for (int d = 0; d < 8; d++)
                        {
                            if (Walk(direction)) return;

                            direction = Functions.ShiftDirection(direction, rotation);
                        }

                    }

                return;
            }

            RoamTime = SEnvir.Now + RoamDelay;

            foreach (MapObject ob in CurrentCell.Objects)
            {
                if (ob == this || !ob.Blocking) continue;

                MirDirection direction = (MirDirection)SEnvir.Random.Next(8);
                int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                for (int d = 0; d < 8; d++)
                {
                    if (Walk(direction)) return;

                    direction = Functions.ShiftDirection(direction, rotation);
                }
                return;
            }

            if (Target != null || SEnvir.Random.Next(10) > 0) return;

            if (SEnvir.Random.Next(3) > 0)
                Walk(Direction);
            else
                Turn((MirDirection)SEnvir.Random.Next(8));
        }
        /// <summary>
        /// 进程目标
        /// </summary>
        public virtual void ProcessTarget()
        {
            if (Target == null) return;

            if (!InAttackRange())
            {
                int[] timer = { 0, 1200, 1250 };
                AttackFirstDelay = SEnvir.Now.AddMilliseconds(timer[SEnvir.Random.Next(3)]);
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

                return;
            }

            if (!CanAttack) return;
            if (AttackFirstDelay < SEnvir.Now)
                Attack();

        }
        /// <summary>
        /// 怪物刷新仆从
        /// </summary>
        /// <param name="fixedCount"></param>
        /// <param name="randomCount"></param>
        /// <param name="target"></param>
        public void SpawnMinions(int fixedCount, int randomCount, MapObject target)
        {
            int count = Math.Min(MaxMinions - MinionList.Count, SEnvir.Random.Next(randomCount + 1) + fixedCount);

            for (int i = 0; i < count; i++)
            {
                MonsterInfo info = SEnvir.GetMonsterInfo(SpawnList);

                if (info == null) continue;

                MonsterObject mob = GetMonster(info);

                if (!SpawnMinion(mob)) return;

                mob.Target = target;
                mob.Master = this;
                MinionList.Add(mob);
            }
        }
        /// <summary>
        /// 刷新随从小怪
        /// </summary>
        /// <param name="mob"></param>
        /// <returns></returns>
        public virtual bool SpawnMinion(MonsterObject mob)
        {
            return mob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, 6));
        }
        /// <summary>
        /// 推
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public override int Pushed(MirDirection direction, int distance)
        {
            if (!MonsterInfo.CanPush) return 0;

            return base.Pushed(direction, distance);
        }
        /// <summary>
        /// 不可攻击的范围
        /// </summary>
        /// <returns></returns>
        protected virtual bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);
        }
        /// <summary>
        /// 可以攻击的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanAttackTarget(MapObject ob)
        {
            //如果对象是自己   对象不为空   对象死亡    对象看不到   对象是卫士   对象是沙巴克BOSS
            if (ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord)
            {
                return false;
            }

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                    return false;
            }

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;

                    //if (player.GameMaster) return false;
                    if (player.Observer) return false;

                    if (PetOwner == null) return true;
                    if (PetOwner == player) return false;

                    if (InSafeZone || player.InSafeZone) return false;

                    switch (PetOwner.PetMode)
                    {
                        case PetMode.Move:
                        case PetMode.None:
                            return false;
                    }

                    switch (PetOwner.AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (PetOwner.InGroup(player))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (PetOwner.InGuild(player))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            // 不是红名 跳出         是队友跳出         是行会成员跳出      处于战争中且是行会成员跳出
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !PetOwner.AtWar(player))
                                return false;
                            else if (player.Stats[Stat.PKPoint] >= Config.RedPoint && (PetOwner.InGuild(player) || PetOwner.InGroup(player)))
                                return false;
                            break;
                    }

                    //是否有宠物攻击目标或玩家的宠物攻击宠物？

                    return true;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (PetOwner == null)
                    {
                        if (mob.PetOwner == null)
                            return SEnvir.Now < RageTime; //野兽VS野兽

                        return true; //野兽VS宠物
                    }

                    switch (PetOwner.PetMode)
                    {
                        case PetMode.Move:
                        case PetMode.None:
                        case PetMode.PvP:
                            return false;
                    }

                    //宠物VS野兽
                    if (mob.PetOwner == null) return true;

                    //宠物VS宠物
                    if (mob.InSafeZone || InSafeZone) return false;

                    if (PetOwner == mob.PetOwner) return false;

                    switch (PetOwner.AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (PetOwner.InGroup(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (PetOwner.InGuild(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !PetOwner.AtWar(mob.PetOwner))
                                return false;
                            else if (mob.PetOwner.Stats[Stat.PKPoint] >= Config.RedPoint && (PetOwner.InGuild(mob.PetOwner) || PetOwner.InGroup(mob.PetOwner)))
                                return false;
                            break;
                    }

                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 可以帮助的目标 比方治疗
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override bool CanHelpTarget(MapObject ob)
        {
            if (ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;

            if (ob == this) return true;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    if (PetOwner == null) return false;

                    PlayerObject player = (PlayerObject)ob;

                    switch (PetOwner.AttackMode)
                    {
                        case AttackMode.Peace:
                            return true;

                        case AttackMode.Group:
                            if (PetOwner.InGroup(player))
                                return true;
                            break;

                        case AttackMode.Guild:
                            if (PetOwner.InGuild(player))
                                return true;
                            break;

                        case AttackMode.WarRedBrown:
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !PetOwner.AtWar(player))
                                return true;
                            break;
                    }

                    return true;

                case ObjectType.Monster:

                    MonsterObject mob = (MonsterObject)ob;

                    if (PetOwner == null) return mob.PetOwner == null;

                    if (mob.PetOwner == null) return false;

                    switch (PetOwner.AttackMode)
                    {
                        case AttackMode.Peace:
                            return true;

                        case AttackMode.Group:
                            if (PetOwner.InGroup(mob.PetOwner))
                                return true;
                            break;

                        case AttackMode.Guild:
                            if (PetOwner.InGuild(mob.PetOwner))
                                return true;
                            break;

                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !PetOwner.AtWar(mob.PetOwner))
                                return true;
                            break;
                    }

                    return true;

                default:
                    return false;
            }
        }
        /// <summary>
        /// 应该攻击的目标
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public virtual bool ShouldAttackTarget(MapObject ob)
        {
            if (Passive || ob == this || ob?.Node == null || ob.Dead || !ob.Visible || ob is Guard || ob is CastleLord) return false;

            switch (ob.Race)
            {
                case ObjectType.Item:
                case ObjectType.NPC:
                case ObjectType.Spell:
                    return false;
            }

            if (ob.Buffs.Any(x => x.Type == BuffType.FishingMaster && !x.Pause)) return false;

            if (ob.Buffs.Any(x => x.Type == BuffType.Invisibility) && !CoolEye) return false;

            if (ob.Buffs.Any(x => x.Type == BuffType.Cloak) && Stats[Stat.IgnoreStealth] == 0)  //如果BUFF=潜行 并且 属性是 忽略隐身=0
            {
                if (!CoolEye) return false;
                if (!Functions.InRange(ob.CurrentLocation, CurrentLocation, 2)) return false;
                if (ob.Level >= Level) return false;
            }

            if (ob.Buffs.Any(x => x.Type == BuffType.Transparency) && ((Poison & PoisonType.Infection) != PoisonType.Infection || Level < 100)) return false;

            switch (ob.Race)
            {
                case ObjectType.Player:
                    PlayerObject player = (PlayerObject)ob;
                    //if (player.GameMaster) return false;
                    if (player.Observer) return false;

                    if (player.Stats[Stat.ClearRing] > 0 && !CoolEye) return false;  //隐身戒指

                    if (PetOwner == null) return true;
                    if (PetOwner == player) return false;

                    if (InSafeZone || player.InSafeZone) return false;

                    switch (PetOwner.PetMode)
                    {
                        case PetMode.Move:
                        case PetMode.None:
                            return false;
                    }

                    switch (PetOwner.AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (PetOwner.InGroup(player))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (PetOwner.InGuild(player))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (player.Stats[Stat.Brown] == 0 && player.Stats[Stat.PKPoint] < Config.RedPoint && !PetOwner.AtWar(player))
                                return false;
                            else if (player.Stats[Stat.PKPoint] >= Config.RedPoint && (PetOwner.InGuild(player) || PetOwner.InGroup(player)))
                                return false;
                            break;
                    }

                    //是否有宠物攻击目标或玩家的宠物攻击宠物？

                    if (PetOwner.Pets.Any(x =>
                    {
                        if (x.Target == null) return false;

                        switch (x.Target.Race)
                        {
                            case ObjectType.Player:
                                return x.Target == player;
                            case ObjectType.Monster:
                                return ((MonsterObject)x.Target).PetOwner == player;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    })) return true;

                    if (player.Pets.Any(x =>
                    {
                        if (x.Target == null) return false;

                        switch (x.Target.Race)
                        {
                            case ObjectType.Player:
                                return x.Target == PetOwner;
                            case ObjectType.Monster:
                                return ((MonsterObject)x.Target).PetOwner == PetOwner;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    })) return true;

                    return false;
                case ObjectType.Monster:
                    MonsterObject mob = (MonsterObject)ob;

                    if (PetOwner == null)
                    {
                        if (mob.PetOwner == null)
                            return SEnvir.Now < RageTime; //野兽VS野兽

                        return true; //野兽VS宠物
                    }

                    switch (PetOwner.PetMode)
                    {
                        case PetMode.Move:
                        case PetMode.None:
                        case PetMode.PvP:
                            return false;
                    }

                    //宠物VS野兽
                    if (mob.PetOwner == null)
                    {
                        //沙巴克城门
                        if (mob.MonsterInfo.AI == 145 && (PetOwner?.Character.Account.GuildMember != null && PetOwner?.Character.Account.GuildMember.Guild.Castle != null || mob.Direction == MirDirection.UpLeft)) return false;

                        if (mob.EXPOwner == PetOwner || PetOwner.InGroup(mob.EXPOwner) || PetOwner.InGuild(mob.EXPOwner))
                            return true;

                        //是暴徒的目标=主人或团体/公会成员

                        if (mob.EXPOwner != null) return false; //其他人的暴徒

                        if (mob.Target == null) return false;

                        PlayerObject mobTarget;

                        if (mob.Target.Race == ObjectType.Monster)
                            mobTarget = ((MonsterObject)mob.Target).PetOwner;
                        else
                            mobTarget = (PlayerObject)mob.Target;

                        if (mobTarget?.Node == null) return false;

                        if (mobTarget == PetOwner || PetOwner.InGroup(mobTarget) || PetOwner.InGuild(mobTarget))
                            return true;

                        return false;
                    }

                    //宠物VS宠物
                    if (mob.InSafeZone || InSafeZone) return false;

                    if (PetOwner == mob.PetOwner) return false;

                    switch (PetOwner.AttackMode)
                    {
                        case AttackMode.Peace:
                            return false;
                        case AttackMode.Group:
                            if (PetOwner.InGroup(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.Guild:
                            if (PetOwner.InGuild(mob.PetOwner))
                                return false;
                            break;
                        case AttackMode.WarRedBrown:
                            if (mob.PetOwner.Stats[Stat.Brown] == 0 && mob.PetOwner.Stats[Stat.PKPoint] < Config.RedPoint && !PetOwner.AtWar(mob.PetOwner))
                                return false;
                            else if (mob.PetOwner.Stats[Stat.PKPoint] >= Config.RedPoint && (PetOwner.InGuild(mob.PetOwner) || PetOwner.InGroup(mob.PetOwner)))
                                return false;
                            break;
                    }


                    if (PetOwner.Pets.Any(x =>
                    {
                        if (x.Target == null) return false;

                        switch (x.Target.Race)
                        {
                            case ObjectType.Player:
                                return x.Target == mob.PetOwner;
                            case ObjectType.Monster:
                                return ((MonsterObject)x.Target).PetOwner == mob.PetOwner;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    })) return true;

                    if (mob.PetOwner.Pets.Any(x =>
                    {
                        if (x.Target == null) return false;

                        switch (x.Target.Race)
                        {
                            case ObjectType.Player:
                                return x.Target == PetOwner;
                            case ObjectType.Monster:
                                return ((MonsterObject)x.Target).PetOwner == PetOwner;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    })) return true;

                    return false;
                default:
                    throw new NotImplementedException();
            }
        }
        /// <summary>
        /// 攻击
        /// </summary>
        protected virtual void Attack()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);  //方向=目标当前位置
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            UpdateAttackTime();  //更新攻击时间

            ActionList.Add(new DelayedAction(
                               SEnvir.Now.AddMilliseconds(400),
                               ActionType.DelayAttack,
                               Target,
                               GetDC() > 0 ? GetDC() : GetMC(),  //获取攻击数值 如果攻击大于0调用攻击，否则调用魔法攻击
                               AttackElement));

        }
        /// <summary>
        /// 攻击地图对象
        /// </summary>
        /// <param name="ob">目标对象</param>
        /// <param name="power">攻击值</param>
        /// <param name="element">元素</param>
        /// <returns></returns>
        public virtual int Attack(MapObject ob, int power, Element element)
        {
            if (ob?.Node == null || ob.Dead) return 0;


            if (PetOwner != null && SEnvir.Random.Next(100) < PetOwner.Stats[Stat.PetACPower])
            {
                if (!Buffs.Any(x => x.Type == BuffType.MonsterBuff))
                {
                    Stats buffStats = new Stats
                    {
                        //[Stat.MaxAC] = Stats[Stat.MaxAC] * Config.PetACPowerRate,
                        //[Stat.MaxMR] = Stats[Stat.MaxMR] * Config.PetACPowerRate,
                        [Stat.MaxDC] = (int)(Stats[Stat.MaxDC] * Config.PetACPowerRate),
                        [Stat.MaxMC] = (int)(Stats[Stat.MaxMC] * Config.PetACPowerRate),
                        [Stat.MaxSC] = (int)(Stats[Stat.MaxSC] * Config.PetACPowerRate),
                    };

                    BuffAdd(BuffType.MonsterBuff, TimeSpan.FromSeconds(Config.PetACPowerTime), buffStats, true, false, TimeSpan.Zero);
                }
            }

            int damage;

            if (PoisonList.Any(x => x.Type == PoisonType.Abyss) && SEnvir.Random.Next(2) > 0)
            {
                ob.Dodged();
                return 0;
            }

            if (element == Element.None)   //无元素攻击
            {
                int accuracy = Stats[Stat.Accuracy];

                if (SEnvir.Random.Next(ob.Stats[Stat.Agility]) > accuracy)   //如果角色的敏捷随机值 大于 准确
                {
                    ob.Dodged();            //出躲避信息
                    return 0;               //0伤害
                }
                if (SEnvir.Random.Next(200) >= Stats[Stat.ACIgnoreRate])
                    damage = power - ob.GetAC();   //伤害-防御
                else
                    damage = power;
            }
            else
            {
                if (SEnvir.Random.Next(300) < ob.Stats[Stat.MagicEvade])  // 魔法躲避
                {
                    ob.Dodged();
                    return 0;
                }
                else
                {
                    if (SEnvir.Random.Next(100) >= Stats[Stat.MRIgnoreRate])
                        damage = power - ob.GetMR();   //伤害-魔防
                    else
                        damage = power;
                }
            }

            int res = ob.Stats.GetResistanceValue(element); //res = 获取的元素值

            if (res > 0)                       //如果大于0 等强元素
                damage -= damage * res / (Config.ElementResistance * 2);   //伤害 -= 伤害*强元素 /10
            else if (res < 0)                  //如果小于0 等弱元素 负值
                damage -= damage * res / Config.ElementResistance;    //伤害 -= 伤害*弱元素 /5

            if (damage <= 0)
            {
                ob.Blocked();
                return 0;
            }

            if (SEnvir.Random.Next(100) < ob.Stats[Stat.InvincibilityChance])
            {
                return 0;
            }

            damage = ob.Attacked(this, damage, element, true, IgnoreShield);

            if (damage <= 0) return damage;

            LifeSteal += damage * Stats[Stat.LifeSteal] / 100M;

            if (LifeSteal > 1)
            {
                int heal = (int)Math.Floor(LifeSteal);
                LifeSteal -= heal;
                ChangeHP(heal);
            }

            //foreach (UserMagic magic in Magics)
            //PetOwner?.LevelMagic(magic);  //攻击怪物就加技能经验？

            if (PoisonType == PoisonType.None || SEnvir.Random.Next(PoisonRate) > 0) return damage;

            ob.ApplyPoison(new Poison
            {
                Owner = this,
                Type = PoisonType,
                Value = GetMC(),            //魔法攻击 GetSC(),
                TickFrequency = TimeSpan.FromSeconds(PoisonFrequency),
                TickCount = PoisonTicks,
            });
            return damage;
        }

        #region Magics  怪物魔法技能
        /// <summary>
        /// 魔法攻击
        /// </summary>
        /// <param name="magic">技能</param>
        /// <param name="element">元素</param>
        /// <param name="travel">穿透</param>
        /// <param name="damage">附加伤害</param>
        public void AttackMagic(MagicType magic, Element element, bool travel, int damage = 0)
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = magic, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500 + (travel ? Functions.Distance(CurrentLocation, Target.CurrentLocation) * 48 : 0)),
                ActionType.DelayAttack,
                Target,
                damage == 0 ? GetMC() : damage,   //如果指定攻击值为0，那么调用魔法攻击
                element));
        }
        /// <summary>
        /// 魔法范围攻击
        /// </summary>
        /// <param name="radius">半径范围</param>
        /// <param name="magic">技能</param>
        /// <param name="element">元素</param>
        /// <param name="damage">附加伤害</param>
        public void AttackAoE(int radius, MagicType magic, Element element, int damage = 0)
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = magic, Locations = new List<Point> { Target.CurrentLocation } });

            UpdateAttackTime();

            List<MapObject> targets = GetTargets(CurrentMap, Target.CurrentLocation, radius);

            foreach (MapObject ob in targets)
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500),
                    ActionType.DelayAttack,
                    ob,
                    damage == 0 ? GetMC() : damage,  //如果指定攻击值为0，那么调用魔法攻击
                    element));
            }
        }
        /// <summary>
        /// 火系攻击
        /// </summary>
        public void SamaGuardianFire()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.SamaGuardianFire, Locations = new List<Point> { Target.CurrentLocation } });

            UpdateAttackTime();

            List<MapObject> targets = GetTargets(CurrentMap, Target.CurrentLocation, 5);

            foreach (MapObject ob in targets)
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500),
                    ActionType.DelayAttack,
                    ob,
                    GetMC(),
                    Element.Fire));
            }
        }
        /// <summary>
        /// 线性魔法攻击
        /// </summary>
        /// <param name="distance">距离</param>
        /// <param name="min">距离范围最小值</param>
        /// <param name="max">距离范围最大值</param>
        /// <param name="magic">调用魔法技能</param>
        /// <param name="element">元素</param>
        public void LineAoE(int distance, int min, int max, MagicType magic, Element element)
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();


            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = magic, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();


            for (int d = min; d <= max; d++)
            {
                MirDirection direction = Functions.ShiftDirection(Direction, d);

                if (magic == MagicType.LightningBeam || magic == MagicType.BlowEarth || magic == MagicType.ElementalHurricane)
                    locations.Add(Functions.Move(CurrentLocation, direction, distance));

                for (int i = 1; i <= distance; i++)
                {
                    Point location = Functions.Move(CurrentLocation, direction, i);
                    Cell cell = CurrentMap.GetCell(location);

                    if (cell == null) continue;

                    if (magic != MagicType.LightningBeam && magic != MagicType.BlowEarth && magic != MagicType.ElementalHurricane)
                        locations.Add(cell.Location);

                    if (cell.Objects != null)
                    {
                        foreach (MapObject ob in cell.Objects)
                        {
                            if (!CanAttackTarget(ob)) continue;

                            ActionList.Add(new DelayedAction(
                                SEnvir.Now.AddMilliseconds(500 + i * 75),
                                ActionType.DelayAttack,
                                ob,
                                GetDC() > 0 ? GetDC() : GetMC(),  //获取攻击数值 如果攻击大于0调用攻击，否则调用魔法攻击
                                element));
                        }
                    }

                    switch (direction)
                    {
                        case MirDirection.Up:
                        case MirDirection.Right:
                        case MirDirection.Down:
                        case MirDirection.Left:
                            cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -2)));

                            if (cell?.Objects != null)
                            {
                                foreach (MapObject ob in cell.Objects)
                                {
                                    if (!CanAttackTarget(ob)) continue;

                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500 + i * 75),
                                        ActionType.DelayAttack,
                                        ob,
                                        GetDC() / 2,
                                        element));
                                }
                            }
                            cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 2)));

                            if (cell?.Objects != null)
                            {
                                foreach (MapObject ob in cell.Objects)
                                {
                                    if (!CanAttackTarget(ob)) continue;

                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500 + i * 75),
                                        ActionType.DelayAttack,
                                        ob,
                                        GetDC() / 2,
                                        element));
                                }
                            }
                            break;
                        case MirDirection.UpRight:
                        case MirDirection.DownRight:
                        case MirDirection.DownLeft:
                        case MirDirection.UpLeft:
                            cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, -1)));

                            if (cell?.Objects != null)
                            {
                                foreach (MapObject ob in cell.Objects)
                                {
                                    if (!CanAttackTarget(ob)) continue;

                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500 + i * 75),
                                        ActionType.DelayAttack,
                                        ob,
                                        GetDC() / 2,
                                        element));
                                }
                            }
                            cell = CurrentMap.GetCell(Functions.Move(location, Functions.ShiftDirection(direction, 1)));

                            if (cell?.Objects != null)
                            {
                                foreach (MapObject ob in cell.Objects)
                                {
                                    if (!CanAttackTarget(ob)) continue;

                                    ActionList.Add(new DelayedAction(
                                        SEnvir.Now.AddMilliseconds(500 + i * 75),
                                        ActionType.DelayAttack,
                                        ob,
                                        GetDC() / 2,
                                        element));
                                }
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 火墙攻击 祖玛教主居多
        /// </summary>
        public void FireWall()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.FireWall, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, 20);

            if (targets.Count == 0) return;

            Point location = targets[SEnvir.Random.Next(targets.Count)].CurrentLocation;

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.DelayMagic,
                MagicType.FireWall,
                CurrentMap.GetCell(location)));

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.DelayMagic,
                MagicType.FireWall,
                CurrentMap.GetCell(Functions.Move(location, MirDirection.Up))));

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.DelayMagic,
                MagicType.FireWall,
                CurrentMap.GetCell(Functions.Move(location, MirDirection.Down))));

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.DelayMagic,
                MagicType.FireWall,
                CurrentMap.GetCell(Functions.Move(location, MirDirection.Left))));

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.DelayMagic,
                MagicType.FireWall,
                CurrentMap.GetCell(Functions.Move(location, MirDirection.Right))));
        }
        /// <summary>
        /// 火墙攻击结束
        /// </summary>
        /// <param name="cell"></param>
        public void FireWallEnd(Cell cell)
        {
            if (cell == null) return;

            if (cell.Objects != null)
            {
                for (int i = cell.Objects.Count - 1; i >= 0; i--)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;

                    SpellObject spell = (SpellObject)cell.Objects[i];

                    if (spell.Effect != SpellEffect.FireWall && spell.Effect != SpellEffect.MonsterFireWall && spell.Effect != SpellEffect.Tempest) continue;

                    spell.Despawn();
                }
            }

            SpellObject ob = new SpellObject
            {
                DisplayLocation = cell.Location,
                TickCount = 15,
                TickFrequency = TimeSpan.FromSeconds(2),
                Owner = this,
                Effect = SpellEffect.MonsterFireWall
            };

            ob.Spawn(cell.Map.Info, cell.Location);

        }
        /// <summary>
        /// 死亡之云   震天魔神调用
        /// </summary>
        /// <param name="location"></param>
        public void DeathCloud(Point location)
        {
            bool visible = true;
            foreach (Cell cell in CurrentMap.GetCells(location, 0, 2))
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500),
                    ActionType.DelayMagic,
                    MagicType.MonsterDeathCloud,
                    cell,
                    visible,
                    location));

                visible = false;
            }
        }
        /// <summary>
        /// 死亡之云结束
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="visible"></param>
        /// <param name="displaylocation"></param>
        public void DeathCloudEnd(Cell cell, bool visible, Point displaylocation)
        {
            if (cell == null) return;

            SpellObject ob = new SpellObject
            {
                DisplayLocation = displaylocation,
                TickCount = 1,
                TickTime = SEnvir.Now.AddMilliseconds(DeathCloudDurationMin + SEnvir.Random.Next(DeathCloudDurationRandom)),
                Owner = this,
                Effect = SpellEffect.MonsterDeathCloud,
                Visible = visible,
            };

            ob.Spawn(cell.Map.Info, cell.Location);

        }
        /// <summary>
        /// 雷电攻击  沃玛教主调用
        /// </summary>
        public void MassLightningBall()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.LightningBall, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            for (int i = -20; i < 20; i += 5)
                locations.Add(new Point(CurrentLocation.X - 20, CurrentLocation.Y - i));

            for (int i = -20; i < 20; i += 5)
                locations.Add(new Point(CurrentLocation.X + 20, CurrentLocation.Y - i));

            for (int i = -20; i < 20; i += 5)
                locations.Add(new Point(CurrentLocation.X + i, CurrentLocation.Y - 20));

            for (int i = -20; i < 20; i += 5)
                locations.Add(new Point(CurrentLocation.X + i, CurrentLocation.Y + 20));

            List<MapObject> targets = GetTargets(CurrentMap, CurrentLocation, Config.MaxViewRange);

            foreach (MapObject ob in targets)
            {
                targetIDs.Add(ob.ObjectID);

                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500 + Functions.Distance(ob.CurrentLocation, CurrentLocation) * 48),
                    ActionType.DelayAttack,
                    ob,
                    GetDC() > 0 ? GetDC() : GetMC(),  //获取攻击数值 如果攻击大于0调用攻击，否则调用魔法攻击
                    Element.Lightning));
            }
        }
        /// <summary>
        /// 群雷攻击 沃玛教主调用
        /// </summary>
        public void MassThunderBolt()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.ThunderBolt, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();


            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, Config.MaxViewRange);
            foreach (Cell cell in cells)
            {
                if (cell.Objects == null)
                {
                    if (SEnvir.Random.Next(50) == 0)
                        locations.Add(cell.Location);

                    continue;
                }

                foreach (MapObject ob in cell.Objects)
                {
                    if (SEnvir.Random.Next(2) > 0) continue;
                    if (!CanAttackTarget(ob)) continue;

                    targetIDs.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayAttack,
                        ob,
                        GetDC() > 0 ? GetDC() : GetMC(),  //获取攻击数值 如果攻击大于0调用攻击，否则调用魔法攻击
                        Element.Lightning));
                }
            }
        }
        /* public void ThunderBolt(int damage)   //霹雳闪电
         {
             Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

             Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.ThunderBolt, Targets = new List<uint> { Target.ObjectID } });

             UpdateAttackTime();

             ActionList.Add(new DelayedAction(
                 SEnvir.Now.AddMilliseconds(500),
                 ActionType.DelayAttack,
                 Target,
                 GetDC(),
                 Element.Lightning));
         }*/

        /// <summary>
        /// 雷电攻击  护卫首将调用
        /// </summary>
        /// <param name="damage"></param>
        public void MonsterThunderStorm(int damage)
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.MonsterThunderStorm, Locations = new List<Point> { CurrentLocation } });

            UpdateAttackTime();

            foreach (MapObject target in GetTargets(CurrentMap, CurrentLocation, 2))
            {
                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500),
                    ActionType.DelayAttack,
                    target,
                    damage,
                    Element.Lightning));
            }

        }
        /// <summary>
        /// 净化   海盗怪物调用
        /// </summary>
        public void Purification()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.Purification, Targets = new List<uint> { Target.ObjectID } });

            UpdateAttackTime();

            ActionList.Add(new DelayedAction(
                SEnvir.Now.AddMilliseconds(500),
                ActionType.DelayMagic,
                MagicType.Purification,
                Target));
        }
        /// <summary>
        /// 群体净化
        /// </summary>
        public void MassPurification()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targets = new List<uint>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.Purification, Targets = targets });

            UpdateAttackTime();

            List<MapObject> obs = GetAllObjects(CurrentLocation, Globals.MagicRange);

            foreach (MapObject ob in obs)
            {
                if (!CanHelpTarget(ob) && !CanAttackTarget(ob)) continue;

                targets.Add(ob.ObjectID);

                ActionList.Add(new DelayedAction(
                    SEnvir.Now.AddMilliseconds(500),
                    ActionType.DelayMagic,
                    MagicType.Purification,
                    ob));
            }
        }

        /// <summary>
        /// 大量的旋风
        /// </summary>
        /// <param name="type"></param>
        /// <param name="chance"></param>
        public void MassCyclone(MagicType type, int chance = 30)
        {
            if (CanTurn)
                Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = type, Targets = targetIDs, Locations = locations, AttackElement = Element.None });

            UpdateAttackTime();

            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, Config.MaxViewRange);
            foreach (Cell cell in cells)
            {
                if (cell.Objects == null)
                {
                    if (SEnvir.Random.Next(chance) == 0)
                        locations.Add(cell.Location);

                    continue;
                }

                foreach (MapObject ob in cell.Objects)
                {
                    if (SEnvir.Random.Next(4) == 0) continue;
                    if (!CanAttackTarget(ob)) continue;

                    targetIDs.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayAttack,
                        ob,
                        GetDC(),
                        Element.Wind));
                }
            }
        }

        /// <summary>
        /// 群体旋风  潘夜牛魔王调用
        /// </summary>
        public void MassCyclone()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.Cyclone, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, Config.MaxViewRange);
            foreach (Cell cell in cells)
            {
                if (cell.Objects == null)
                {
                    if (SEnvir.Random.Next(30) == 0)
                        locations.Add(cell.Location);

                    continue;
                }

                foreach (MapObject ob in cell.Objects)
                {
                    if (SEnvir.Random.Next(4) == 0) continue;
                    if (!CanAttackTarget(ob)) continue;

                    targetIDs.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayAttack,
                        ob,
                        GetDC() > 0 ? GetDC() : GetMC(),  //获取攻击数值 如果攻击大于0调用攻击，否则调用魔法攻击
                        Element.Wind));
                }
            }
        }

        /*
                public void MonsterIceStorm()   //怪物冰风暴
                {
                    Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

                    List<uint> targetIDs = new List<uint>();
                    List<Point> locations = new List<Point> { Target.CurrentLocation };

                    Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.MonsterIceStorm, Targets = targetIDs, Locations = locations });

                    UpdateAttackTime();

                    List<MapObject> targets = GetTargets(CurrentMap, Target.CurrentLocation, 1);

                    foreach (MapObject target in targets)
                    {
                        ActionList.Add(new DelayedAction(
                            SEnvir.Now.AddMilliseconds(500),
                            ActionType.DelayAttack,
                            target,
                            GetDC(),
                            Element.Ice));
                    }
                }*/

        /// <summary>
        /// 毒云    霸王守卫调用
        /// </summary>
        public void PoisonousCloud()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.PoisonousCloud, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, 2);

            foreach (Cell cell in cells)
            {
                SpellObject ob = new SpellObject
                {
                    Visible = cell == CurrentCell,
                    DisplayLocation = CurrentLocation,
                    TickCount = 1,
                    TickFrequency = TimeSpan.FromSeconds(20),
                    Owner = this,
                    Effect = SpellEffect.PoisonousCloud,
                    Power = 20
                };

                ob.Spawn(CurrentMap.Info, cell.Location);
            }

        }
        /// <summary>
        /// 击退    霸王教主调用
        /// </summary>
        public void DragonRepulse(List<MapObject> targets)
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            //发包 创建对象魔法 对象ID 方向 当前坐标 释放开启 技能类型调用 攻击目标 目标当前位置
            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.Repulsion, Targets = targetIDs, Locations = locations });

            foreach (MapObject ob in targets)
            {
                if (ob.Race == ObjectType.Player && ob.Buffs.Any(x => x.Type == BuffType.Transparency)) continue;
                ob.Pushed(Direction, 8);
            }

            UpdateAttackTime();

            //BuffInfo buff = BuffAdd(BuffType.DragonRepulse, TimeSpan.FromSeconds(6), null, true, false, TimeSpan.FromSeconds(1));
            //buff.TickTime = TimeSpan.FromMilliseconds(500);
            List<Cell> cells = CurrentMap.GetCells(CurrentLocation, 0, Config.MaxViewRange);
            foreach (Cell cell in cells)
            {
                if (cell.Objects == null)
                {
                    if (SEnvir.Random.Next(30) == 0)
                        locations.Add(cell.Location);

                    continue;
                }

                foreach (MapObject ob in cell.Objects)
                {
                    if (SEnvir.Random.Next(4) == 0) continue;
                    if (!CanAttackTarget(ob)) continue;

                    targetIDs.Add(ob.ObjectID);

                    ActionList.Add(new DelayedAction(
                        SEnvir.Now.AddMilliseconds(500),
                        ActionType.DelayAttack,
                        ob,
                        GetDC() > 0 ? GetDC() : GetMC(),  //获取攻击数值 如果攻击大于0调用攻击，否则调用魔法攻击
                        Element.Lightning));
                }
            }
        }

        /// <summary>
        /// 击退   海盗天马调用
        /// </summary>
        public void DragonRepulse()
        {
            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);

            List<uint> targetIDs = new List<uint>();
            List<Point> locations = new List<Point>();

            Broadcast(new S.ObjectMagic { ObjectID = ObjectID, Direction = Direction, CurrentLocation = CurrentLocation, Cast = true, Type = MagicType.DragonRepulse, Targets = targetIDs, Locations = locations });

            UpdateAttackTime();

            BuffInfo buff = BuffAdd(BuffType.DragonRepulse, TimeSpan.FromSeconds(6), null, true, false, TimeSpan.FromSeconds(1));
            buff.TickTime = TimeSpan.FromMilliseconds(500);
        }

        /// <summary>
        /// 击退结束
        /// </summary>
        /// <param name="ob"></param>
        public void DragonRepulseEnd(MapObject ob)
        {
            if (Attack(ob, GetDC(), AttackElement) > 0)
            {
                MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, ob.CurrentLocation);
                if (ob.Pushed(dir, 1) == 0)
                {
                    int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

                    for (int i = 1; i < 2; i++)
                    {
                        if (ob.Pushed(Functions.ShiftDirection(dir, i * rotation), 1) > 0) break;
                        if (ob.Pushed(Functions.ShiftDirection(dir, i * -rotation), 1) > 0) break;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// 更新攻击的时间
        /// </summary>
        public void UpdateAttackTime()
        {
            AttackTime = SEnvir.Now.AddMilliseconds(AttackDelay);
            ActionTime = SEnvir.Now.AddMilliseconds(Math.Min(MoveDelay, AttackDelay - 100));

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            if (poison != null)
            {
                AttackTime += TimeSpan.FromMilliseconds(poison.Value * 500);
                ActionTime += TimeSpan.FromMilliseconds(poison.Value * 500);
            }

            if (PoisonList.Any(x => x.Type == PoisonType.Neutralize))
            {
                AttackTime += TimeSpan.FromMilliseconds(AttackDelay);
                ActionTime += TimeSpan.FromMilliseconds(Math.Min(MoveDelay, AttackDelay - 100));
            }
        }
        /// <summary>
        /// 更新移动的时间
        /// </summary>
        public virtual void UpdateMoveTime()
        {
            MoveTime = SEnvir.Now.AddMilliseconds(MoveDelay + 500);
            ActionTime = SEnvir.Now.AddMilliseconds(Math.Min(MoveDelay - 100, AttackDelay));

            Poison poison = PoisonList.FirstOrDefault(x => x.Type == PoisonType.Slow);
            if (poison != null)
            {
                AttackTime += TimeSpan.FromMilliseconds(poison.Value * 500);
                ActionTime += TimeSpan.FromMilliseconds(poison.Value * 500);
            }

            if (PoisonList.Any(x => x.Type == PoisonType.Neutralize))
            {
                AttackTime += TimeSpan.FromMilliseconds(MoveDelay);
                ActionTime += TimeSpan.FromMilliseconds(Math.Min(MoveDelay - 100, AttackDelay));
            }
        }
        /// <summary>
        /// 怪物被攻击时
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="power"></param>
        /// <param name="element"></param>
        /// <param name="canReflect"></param>
        /// <param name="ignoreShield"></param>
        /// <param name="canCrit"></param>
        /// <param name="canStruck"></param>
        /// <returns></returns>
        public override int Attacked(MapObject attacker, int power, Element element, bool canReflect = true, bool ignoreShield = false, bool canCrit = true, bool canStruck = true)
        {
            if (attacker?.Node == null || power == 0 || Dead || attacker.CurrentMap != CurrentMap ||
               (!Functions.InRange(attacker.CurrentLocation, CurrentLocation, Config.MaxViewRange) && (attacker.Race == ObjectType.Monster && (attacker as MonsterObject).MonsterInfo.AI != 146))
                || Stats[Stat.Invincibility] > 0) return 0;

            PlayerObject player;

            switch (attacker.Race)
            {
                case ObjectType.Player:
                    PlayerTagged = true;
                    player = (PlayerObject)attacker;
                    break;
                case ObjectType.Monster:
                    player = ((MonsterObject)attacker).PetOwner;
                    break;
                default:
                    throw new NotImplementedException();
            }

            ShockTime = DateTime.MinValue;

            //MoveTime = SEnvir.Now.AddMilliseconds(MoveDelay + 500);

            if (EXPOwner == null && PetOwner == null)
                EXPOwner = player;

            if (EXPOwner == player && player != null)
                EXPOwnerTime = SEnvir.Now + EXPOwnerDelay;

            //伤害计数统计

            if (StruckTime != DateTime.MaxValue && SEnvir.Now > StruckTime.AddMilliseconds(300))
            {
                StruckTime = SEnvir.Now;
                Broadcast(new S.ObjectStruck { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, AttackerID = attacker.ObjectID, Element = element });
            }

            if ((Poison & PoisonType.Red) == PoisonType.Red || Stats[Stat.RedPoison] > 0)  //红毒
                power = (int)(power * Config.RedPoisonAttackRate);

            for (int i = 0; i < attacker.Stats[Stat.Rebirth]; i++)   //转生PVE伤害
                power = (int)(power * Config.RebirthPVE);

            BuffInfo buff = Buffs.FirstOrDefault(x => x.Type == BuffType.MagicShield);

            if (buff != null)
                buff.RemainingTime -= TimeSpan.FromMilliseconds(power * 10);

            power -= power * Stats[Stat.MagicShield] / 100;   //魔法盾减伤

            if (attacker.Stats[Stat.DieExtraDamage] > 0 && !MonsterInfo.Undead)
                power += attacker.Stats[Stat.DieExtraDamage];  //追加死系伤害

            if (attacker.Stats[Stat.LifeExtraDamage] > 0 && MonsterInfo.Undead)
                power += attacker.Stats[Stat.LifeExtraDamage];  //追加生系伤害

            if (SEnvir.Random.Next(100) < (attacker.Stats[Stat.CriticalChance] + attacker.Stats[Stat.WeponCriticalChance]) && canCrit)  //暴击几率
            {
                power += power * 30 / 100 + power * (attacker.Stats[Stat.CriticalDamage] + attacker.Stats[Stat.WeponCriticalChance]) / 100;
                Critical();  //显示暴击效果
            }
            else if (SEnvir.Random.Next(100) < 30 && attacker.Stats[Stat.CriticalHit] > 0)  //会心一击
            {
                power += power * attacker.Stats[Stat.CriticalHit] / 100;
                CriticalHit();  //显示会心一击效果
            }

            if (MonsterInfo.AI != 144 && MonsterInfo.AI != 4 && MonsterInfo.AI != 145 && (MonsterInfo.BodyShape < 179 || MonsterInfo.BodyShape > 186))   //如果是嫦娥 或者 树  或者 城门 跳出
                power += attacker.Stats[Stat.ExtraDamage];  //额外伤害

            //if (SEnvir.Random.Next(100) < attacker.Stats[Stat.DamageAdd])
            //{
            //    power = power * 150 / 100;
            //}

            buff = Buffs.FirstOrDefault(x => x.Type == BuffType.SuperiorMagicShield);

            if (buff != null)
            {
                Stats[Stat.SuperiorMagicShield] -= (int)Math.Min(int.MaxValue, power);
                if (Stats[Stat.SuperiorMagicShield] <= 0)
                    BuffRemove(buff);
            }
            else
                ChangeHP(-power);  //改变HP（-伤害值）

            if (Dead) return power;  //如果死亡 返回 伤害值

            if (Target == null)   //如果 目标 等空
            {
                Target = attacker;  //目标 = 攻击者
                _TargetAttackTick = (UInt32)Environment.TickCount; //攻击目标时间= 环境计时
            }
            else if (Target != null) //否则 如果 目标 不为空
            {
                //存在攻击目标的前提 
                UInt32 nTick = (UInt32)Environment.TickCount - _TargetAttackTick; //定义nTick=环境计时-攻击目标时间

                //SEnvir.Log(string.Format("上次受到攻击是:{0:G}MS.", nTick));

                if (Target == attacker)  //如果 目标等攻击者
                {
                    //攻击目标未变化
                    _TargetAttackTick = (UInt32)Environment.TickCount;  //攻击目标时间=环境计时
                }
                else if (nTick > (Config.MonHatredTime * 1000) && SEnvir.Random.Next(10000) <= 9000)//否则 如果定义时间大于4500 并且 随机函数10000小于或等于9000 10秒拉走仇恨
                {
                    //判断当前攻击目标上次攻击时间间隔 超过10秒可以换新的攻击目标
                    Target = attacker;  //目标 = 攻击者
                    _TargetAttackTick = (UInt32)Environment.TickCount;  //攻击目标时间= 环境计时
                }
                else if (CanAttackTarget(attacker) && PetOwner == null && SEnvir.Random.Next(100) <= 40) //否则 如果 宠物主人 为 空 并区 随机函数100小于或等于40
                {
                    Target = attacker; //目标 = 攻击者
                    _TargetAttackTick = (UInt32)Environment.TickCount; //攻击目标时间 = 环境计时
                }
            }

            return power;  //返回 伤害值
        }
        /// <summary>
        /// 使用毒效果
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool ApplyPoison(Poison p)
        {
            bool res = base.ApplyPoison(p);

            if (res && CanAttackTarget(p.Owner) && (Target == null || SEnvir.Random.Next(100) < 45))
            {
                Target = p.Owner;
                _TargetAttackTick = (UInt32)Environment.TickCount; //攻击目标时间 = 环境计时
                SearchTime = SEnvir.Now.AddSeconds(5);
            }

            if (p.Owner.Race == ObjectType.Player)
                PlayerTagged = true;

            return res;
        }
        /// <summary>
        /// 怪物死亡
        /// </summary>
        public override void Die()
        {
            base.Die();

            YieldReward();  //爆出奖励

            if (EXPOwner != null)
            {
                #region 人物杀怪

                //队列一个事件, 不要忘记添加listener
                SEnvir.EventManager.QueueEvent(
                    new PlayerKillMonster(EventTypes.PlayerKillMonster,
                        new PlayerKillMonsterEventArgs { KilledMonster = MonsterInfo }));

                //python 触发
                try
                {
                    dynamic trig_player;
                    if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { EXPOwner, MonsterInfo });
                        SEnvir.ExecutePyWithTimer(trig_player, this, "OnKillMon", args);
                        //trig_player(this, "OnKillMon", args);
                    }
                }
                catch (SyntaxErrorException e)
                {
                    string msg = "PlayerEvent Syntax error : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (SystemExitException e)
                {
                    string msg = "PlayerEvent SystemExit : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (Exception ex)
                {
                    string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, error));
                }

                #endregion

                //记录击杀信息
                if (MonsterInfo.IsBoss)
                {
                    var bossInfo = SEnvir.BossTrackerList.FirstOrDefault(x => x.BossInfo.Index == MonsterInfo.Index);
                    if (bossInfo != null)
                    {
                        bossInfo.LastKiller = EXPOwner.Character;
                        bossInfo.LastKillTime = SEnvir.Now;
                    }
                }
            }

            Master?.MinionList.Remove(this);
            Master = null;

            PetOwner?.Pets.Remove(this);
            PetOwner = null;

            for (int i = MinionList.Count - 1; i >= 0; i--)
                MinionList[i].Master = null;

            MinionList.Clear();

            DeadTime = SEnvir.Now + Config.DeadDuration;  //死亡持续时间

            if (Drops != null)
                DeadTime += Config.HarvestDuration;  //割肉时间

            if (SpawnInfo != null)
                SpawnInfo.AliveCount--;

            ProcessEvents();  //跳转处理事件

            SpawnInfo = null;  //刷怪信息

            EXPOwner = null;  //经验所有者

        }
        /// <summary>
        /// 处理事件
        /// </summary>
        private void ProcessEvents()
        {
            if (SpawnInfo == null) return;   //如果刷怪信息为空跳出

            foreach (MonsterEventTarget target in MonsterInfo.Events)  //遍历（怪物信息事件中的事件目标）
            {
                if ((DropSet & target.DropSet) != target.DropSet) continue;

                int start = target.MonsterEvent.CurrentValue;
                int end = Math.Min(target.MonsterEvent.MaxValue, Math.Max(0, start + target.Value));

                target.MonsterEvent.CurrentValue = end;

                foreach (MonsterEventAction action in target.MonsterEvent.Actions)
                {
                    if (start >= action.TriggerValue || end < action.TriggerValue) continue;

                    Map map;
                    switch (action.Type)
                    {
                        case MonsterEventActionType.GlobalMessage:
                            SEnvir.Broadcast(new S.Chat { Text = action.StringParameter1, Type = MessageType.System });
                            break;
                        case MonsterEventActionType.MapMessage:
                            map = SEnvir.GetMap(action.MapParameter1);
                            if (map == null) continue;

                            map.Broadcast(new S.Chat { Text = action.StringParameter1, Type = MessageType.System });
                            break;
                        case MonsterEventActionType.PlayerMessage:
                            if (EXPOwner == null) continue;

                            EXPOwner.Broadcast(new S.Chat { Text = action.StringParameter1, Type = MessageType.System });
                            break;
                        case MonsterEventActionType.MonsterSpawn:
                            SpawnInfo spawn = SEnvir.Spawns.FirstOrDefault(x => x.Info == action.RespawnParameter1);
                            if (spawn == null) continue;

                            spawn.DoSpawn(true);
                            break;
                        case MonsterEventActionType.MonsterPlayerSpawn:

                            MonsterObject mob = GetMonster(action.MonsterParameter1);
                            mob.Spawn(CurrentMap.Info, CurrentMap.GetRandomLocation(CurrentLocation, 10));
                            break;
                        case MonsterEventActionType.MovementSettings:
                            break;
                        case MonsterEventActionType.PlayerRecall:
                            map = SEnvir.GetMap(action.MapParameter1);
                            if (map == null) continue;

                            for (int i = map.Players.Count - 1; i >= 0; i--)
                            {
                                PlayerObject player = map.Players[i];
                                player.Teleport(action.RegionParameter1);
                            }
                            break;
                        case MonsterEventActionType.PlayerEscape:
                            map = SEnvir.GetMap(action.MapParameter1);
                            if (map == null) continue;

                            for (int i = map.Players.Count - 1; i >= 0; i--)
                            {
                                PlayerObject player = map.Players[i];
                                player.Teleport(player.Character.BindPoint.BindRegion);
                            }
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// 爆出奖励 经验值
        /// </summary>
        protected void YieldReward()
        {
            if (EXPOwner == null || PetOwner != null) return;   //如果经验所有者为空  或  宠物主人 为空，那么结束

            decimal eRate = (1M + ExtraExperienceRate) * Config.OverallExp / 100M;  //增加全局经验百分比调整
            decimal dRate = 1M;
            int totalLevels = 0;
            List<PlayerObject> ePlayers = new List<PlayerObject>(); //没有死亡的队友             经验爆率？
            List<PlayerObject> dPlayers = new List<PlayerObject>(); //所有队员，包含死亡的队友   物品爆率？

            if (EXPOwner.GroupMembers == null && EXPOwner.Stats[Stat.TeamExperienceRate] > 0)
            {
                eRate += EXPOwner.Stats[Stat.TeamExperienceRate] / 100M;
            }

            //组队获得经验处理
            if (EXPOwner.GroupMembers != null)   //如果 经验所有者.组成员 不为空
            {
                int eWarrior = 0, eWizard = 0, eTaoist = 0;//, eAssassin = 0;
                int dWarrior = 0, dWizard = 0, dTaoist = 0;//, dAssassin = 0;

                foreach (PlayerObject ob in EXPOwner.GroupMembers)   //组队判断
                {
                    //判断是否在同一张地图   是否在视野范围内
                    if (ob.CurrentMap != CurrentMap || !Functions.InRange(ob.CurrentLocation, CurrentLocation, Config.MaxViewRange)) continue;

                    switch (ob.Class)  //爆率++
                    {
                        case MirClass.Warrior:
                            dWarrior++;
                            break;
                        case MirClass.Wizard:
                            dWizard++;
                            break;
                        case MirClass.Taoist:
                            dTaoist++;
                            break;
                            //case MirClass.Assassin:
                            //    dAssassin++;
                            //    break;
                    }

                    dPlayers.Add(ob);

                    if (ob.Dead) continue;

                    switch (ob.Class)  //经验++
                    {
                        case MirClass.Warrior:
                            eWarrior++;
                            break;
                        case MirClass.Wizard:
                            eWizard++;
                            break;
                        case MirClass.Taoist:
                            eTaoist++;
                            break;
                            //case MirClass.Assassin:
                            //    eAssassin++;
                            //    break;
                    }
                    ePlayers.Add(ob);
                    totalLevels += ob.Level;
                }

                //if (Config.AllowAssassin)
                //{
                //    if ((dWarrior > 0) && (dWizard > 0) && (dTaoist > 0) && (dAssassin > 0))
                //    {
                //        if (Buffs.Any(p => p.Type == BuffType.Group)) //组队buff额外加成
                //            dRate *= Math.Max(Config.GroupInZoneAddDrop * (dWarrior + dWizard + dTaoist + dAssassin) + 100, 100) / 100;  //同屏杀怪加爆率
                //    }
                //    if ((eWarrior > 0) && (eWizard > 0) && (eTaoist > 0) && (eAssassin > 0))
                //    {
                //        if (Buffs.Any(p => p.Type == BuffType.Group)) //组队buff额外加成
                //            eRate *= Math.Max(Config.GroupInZoneAddExp + 100, 100) / 100;          //同屏杀怪加经验
                //    }
                //}
                //else
                //{
                if ((dWarrior > 0) && (dWizard > 0) && (dTaoist > 0))
                {
                    if (Buffs.Any(p => p.Type == BuffType.Group)) //组队buff额外加成
                        dRate *= Math.Max(Config.GroupInZoneAddDrop + 100, 100) / 100.0M;  //同屏杀怪加爆率
                }
                if ((eWarrior > 0) && (eWizard > 0) && (eTaoist > 0))
                {
                    if (Buffs.Any(p => p.Type == BuffType.Group)) //组队buff额外加成
                        eRate *= Math.Max(Config.GroupInZoneAddExp + 100, 100) / 100.0M;          //同屏杀怪加经验
                }
                //}

                //判断每个类型职业最少的数量
                switch (Math.Min(dWarrior, Math.Min(dWizard, Math.Min(dTaoist, 2))))//Config.AllowAssassin ? dAssassin : 3))))   //爆率加成
                {
                    case 1:
                        dRate *= Config.DZBLRate;
                        break;
                    case 2:
                        dRate *= Config.SZBLRate;
                        break;
                    case 3:
                        dRate *= Config.DRZBLRate;
                        break;
                }
                switch (Math.Min(eWarrior, Math.Min(eWizard, Math.Min(eTaoist, 2))))//Config.AllowAssassin ? eAssassin : 3))))   //经验加成
                {
                    case 1:
                        eRate *= Config.DZEXPRate;
                        break;
                    case 2:
                        eRate *= Config.SZEXPRate;
                        break;
                    case 3:
                        eRate *= Config.DRZEXPRate;
                        break;
                }
            }

            //增加地图经验比例，判断不是宠物
            if (PetOwner == null && CurrentMap != null)
                eRate *= 1 + MapExperienceRate / 100M;
            //这里有一个获得的最大经验限制
            decimal exp = Math.Min(Experience * eRate, 500000000);

            if (ePlayers.Count == 0)  //如果组队玩家计数为0
            {
                if (!EXPOwner.Dead && EXPOwner.CurrentMap == CurrentMap && Functions.InRange(EXPOwner.CurrentLocation, CurrentLocation, Config.MaxViewRange))
                {
                    //转身产生的经验衰减
                    if (EXPOwner.Stats[Stat.Rebirth] > 0 && ExtraExperienceRate > 0)
                        exp /= ExtraExperienceRate;

                    //人物与怪物等级差
                    int levelDifference = Math.Max(0, EXPOwner.Level - MonsterInfo.Level);
                    //人物每高1级经验-5%
                    exp *= 1M - (Config.LevelDifference * levelDifference);
                    //避免经验出现负值,最小获取1经验
                    exp = Math.Max(1, exp);

                    // 奖金池玩法
                    decimal bonusEx = 0;
                    if (Config.EnableRewardPool)
                    {
                        // 检查领取资格
                        bool canClaimReward =
                            EXPOwner.CheckRewardPoolClaimEligibility(true, this);
                        if (canClaimReward)
                        {
                            bonusEx = EXPOwner.ClaimRewardPoolRewards(true, this);
                        }
                    }

                    EXPOwner.GainExperience(exp, PlayerTagged, Level, bonusEx: bonusEx);
                }
            }
            else
            {
                if (ePlayers.Count > 1)
                    exp += exp * Config.ZDEXPRate * ePlayers.Count; //6%每个附近的成员

                //组队成员分经验
                foreach (PlayerObject player in ePlayers)
                {
                    //根据等级高低分经验值
                    decimal expfinal = exp * player.Level / totalLevels;
                    //转身产生的经验衰减
                    if (player.Stats[Stat.Rebirth] > 0 && ExtraExperienceRate > 0)
                        expfinal /= ExtraExperienceRate;

                    //人物与怪物等级差
                    int levelDifference = Math.Max(0, player.Level - MonsterInfo.Level);
                    //人物每高1级经验-5%
                    expfinal *= 1M - (Config.LevelDifference * levelDifference);

                    //避免经验出现负值,最小获取1经验
                    expfinal = Math.Max(1, expfinal);

                    // 奖金池玩法
                    decimal bonusEx = 0;
                    if (Config.EnableRewardPool)
                    {
                        // 检查领取资格
                        bool canClaimReward =
                            player.CheckRewardPoolClaimEligibility(player.ObjectID == EXPOwner?.ObjectID, this);
                        if (canClaimReward)
                        {
                            bonusEx = player.ClaimRewardPoolRewards(player.ObjectID == EXPOwner?.ObjectID, this);
                        }
                    }
                    player.GainExperience(expfinal, PlayerTagged, Level, bonusEx: bonusEx);
                }
            }

            if (dPlayers.Count == 0)
            {
                if (!EXPOwner.Dead && EXPOwner.CurrentMap == CurrentMap && Functions.InRange(EXPOwner.CurrentLocation, CurrentLocation, Config.MaxViewRange))
                    Drop(EXPOwner, 1, dRate);
            }
            else
            {
                /*
                 * dPlayers 存的是所有在视野范围内 未死亡的组队队友
                 * 怪物爆率由EXPOwner的爆率属性决定
                 * 传入dPlayers.Count计算组队额外爆率加成（仅限旧版爆率有用)
                 * 传入dRate 作为加成过的组队爆率
                 */
                Drop(EXPOwner, dPlayers.Count, dRate);

                //处理组队任务物品爆率
                foreach (PlayerObject player in dPlayers)
                {
                    QuestDrop(player);
                }
            }
        }

        /// <summary>
        /// 爆出奖励 物品 所有者
        /// </summary>
        /// <param name="owner">所有者</param>
        /// <param name="players">玩家数</param>
        /// <param name="rate">爆率</param>
        /// <param name="isTest">是否为测试</param>
        public virtual void Drop(PlayerObject owner, int players, decimal rate, bool isTest = false)
        {
            //存储爆率测试结果 仅当isTest==True时有用
            DropTestDict = new Dictionary<string, long>();

            if (!Config.UseOldItemDrop)
            {
                //吉米原版
                rate *= 1M + owner.Stats[Stat.DropRate] / 100M;       //乘加爆率加成
                rate *= 1M + owner.Stats[Stat.BaseDropRate] / 100M;   //乘加基础爆率加成

                if (owner.setConfArr[(int)AutoSetConf.SetAutoOnHookBox])
                    rate *= 1 + Config.HooKDrop / 100M;                  //乘加挂机爆率加成

                if (PetOwner == null && CurrentMap != null)
                    rate *= 1 + MapDropRate / 100M;                   //乘加地图爆率 
            }
            else
            {
                //新加
                rate += owner.Stats[Stat.DropRate] / 100M;       //加爆率加成
                rate += owner.Stats[Stat.BaseDropRate] / 100M;   //加基础爆率加成

                if (owner.setConfArr[(int)AutoSetConf.SetAutoOnHookBox])
                    rate += Config.HooKDrop / 100M;                  //加挂机爆率加成

                if (PetOwner == null && CurrentMap != null)
                    rate += MapDropRate / 100M;                   //加地图爆率
            }

            bool result = false;   //结果 = false
            List<UserItem> drops = null;  //掉落 = 空

            //爆率分组判断
            var DropSetGroup = MonsterInfo.Drops.GroupBy(o => o.DropGroup).Select(o => o.Key).ToList();

            Dictionary<int, List<DropInfo>> dicDropSetGroup = new Dictionary<int, List<DropInfo>>();

            for (int i = 0; i < DropSetGroup.Count; i++)
            {
                List<DropInfo> ListDropInfo = MonsterInfo.Drops.Where(o => o.DropGroup == DropSetGroup[i]).ToList();
                dicDropSetGroup.Add(DropSetGroup[i], ListDropInfo);
            }

            Dictionary<int, List<DropInfo>> dicDropSetGroupResult = new Dictionary<int, List<DropInfo>>();

            foreach (int DropSetTemp in dicDropSetGroup.Keys)
            {

                List<DropInfo> ListDropInfo = dicDropSetGroup[DropSetTemp];

                List<DropInfo> ListDropInfoRestul = new List<DropInfo>();

                foreach (DropInfo drop in ListDropInfo)  //循环遍历当前怪物掉落的物品
                {
                    if (rate < 0) continue;
                    //如果 掉落道具为空  掉落道具为永不爆出 掉落记录为零 掉落设置不等 
                    if (drop?.Item == null || drop?.Item.NoMake == true || drop.Chance == 0 || (DropSet & drop.DropSet) != drop.DropSet) continue;
                    //复活节活动
                    if (drop.EasterEvent && !EasterEventMob) continue;
                    //被标记为itembot的账号不给爆东西
                    if (owner.Character.Account.ItemBot) continue;

                    //金币随机掉落数 = 最大取值（数量的一半+数量的随机值）
                    long amount = Math.Max(1, drop.Amount / 2 + SEnvir.Random.Next(drop.Amount));

                    /*
                     * 爆率为 chance分之一 即越高越难爆
                     * 物品爆率是int 但是注意乘以爆率加成后可能超过int.maxvalue
                     */
                    int chance = drop.Chance;

                    if (drop.Item.Effect == ItemEffect.Gold)  //掉落金币
                    {
                        if (owner.Character.Account.GoldBot && Level < owner.Level) continue;   //如果账号金币限制 或者级别超过对应等级 继续

                        chance = drop.Chance;  //几率=爆率几率

                        if (!Config.UseOldItemDrop)
                        {
                            //吉米原版计算玩家数
                            amount /= Math.Max(1, players);    //掉落值/=玩家数
                        }

                        amount += (int)(amount * owner.Stats[Stat.GoldRate] / 100M);  //掉落值+=金币爆率%

                        amount += (int)(amount * owner.Stats[Stat.BaseGoldRate] / 100M);  //掉落值+=基础金币爆率%

                        if (PetOwner == null && CurrentMap != null)  //如果宠物主人空 地图不为空
                            amount += (int)(amount * MapGoldRate / 100M);   //掉落值+=掉落值*地图金币爆率%

                        if (amount == 0) continue;  //如果掉落值是0 继续
                    }
                    else   //道具掉落计算
                    {
                        if (!Config.UseOldItemDrop)
                        {
                            //吉米原版计算玩家数
                            chance /= (int)Math.Max(1, Math.Floor(players * rate));
                        }
                        else
                        {
                            //新加
                            chance /= (int)Math.Max(1, rate);
                        }
                    }

                    //个人爆率
                    UserDrop userDrop = owner.Character.Account.UserDrops.FirstOrDefault(x => x.Item == drop.Item);
                    if (!Config.PersonalDropDisabled)   //这里好像只是起到个人爆率后台计数的记录，如果关闭就不记录个人爆率信息
                    {
                        if (userDrop == null)   //角色爆率为空
                        {
                            userDrop = SEnvir.UserDropList.CreateNewObject();   //如果该道具爆率是空的，创建新的记录
                            userDrop.Item = drop.Item;                          //对应为空的道具名
                            userDrop.Account = owner.Character.Account;         //对应记录到角色账号里
                        }
                    }

                    if (drop.PartOnly && drop.Item.PartCount <= 1)
                    {
                        //这属于数据库设置错误
                        SEnvir.Log($"怪物 {MonsterInfo?.MonsterName} 的爆率 {drop.Item?.ItemName} 设置错误, 请检查PartOnly和PartCount");
                        continue;
                    }

                    //爆不爆随机值
                    if (SEnvir.Random.Next(chance) == 0)
                    {
                        //确定要爆
                        result = true;

                        /*
                         * 确定要爆 要么碎片要么成品
                         * 如果设置了只爆碎片 那么再随机一次drop.Item.PartCount
                         * 如果这个随机数==0，那么爆出成品 否则是碎片
                         * 这样碎片的爆率为原爆率，成品爆率为 1/(原爆率*合成所需碎片个数)
                         * 即 直接出成品的概率更低
                         *---------------------------------------------------------------------------------------------------------------
                         * 	                 |       服务端爆碎片开关开启	    |              服务端爆碎片开关关闭                     |            
                         *--------------------------------------------------------------------------------------------------------------- 	                  
                         *  道具只爆碎片开启 |	只爆碎片 不爆成品	            |  只爆成品                                             |
                         *                   |  碎片按碎片爆率 永不爆成品       |  这里应该不管碎片是否开启开关，只按成品的爆率去计算   |
                         *---------------------------------------------------------------------------------------------------------------  
                         *  道具只爆碎片关闭 |	爆碎片   也爆成品	            |  只爆成品                                             |
                         *                   |  碎片按碎片爆率 成品按成品爆率   |  只按成品的爆率去计算                                 |
                         *---------------------------------------------------------------------------------------------------------------
                         */

                        //爆碎片必须建立在该物品可以有碎片的前提上
                        if (drop.Item.PartCount > 1)
                        {
                            if (Config.FallPartOnly)
                            {
                                //可以爆碎片
                                if (drop.PartOnly)
                                {
                                    //只爆碎片 不爆成品
                                    //碎片
                                    if (isTest)
                                    {
                                        //进行爆率测试 不创建真实道具 仅记录
                                        string itemName = drop.Item.ItemName + " 碎片";
                                        if (DropTestDict.ContainsKey(itemName))
                                        {
                                            DropTestDict[itemName] += amount;
                                        }
                                        else
                                        {
                                            DropTestDict.Add(itemName, amount);
                                        }
                                        continue;
                                    }
                                    else
                                    {
                                        UserItem item = SEnvir.CreateDropItem(SEnvir.ItemPartInfo);
                                        //记录物品来源
                                        SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.Monster, MonsterInfo?.MonsterName, owner?.Name);

                                        item.AddStat(Stat.ItemIndex, drop.Item.Index, StatSource.Added);  //道具增加额外属性
                                        item.StatsChanged();  //道具刷新属性变化

                                        item.IsTemporary = true;

                                        if (NeedHarvest)
                                        {
                                            if (drops == null)
                                                drops = new List<UserItem>();

                                            if (drop.Item.Rarity != Rarity.Common)
                                            {
                                                owner.Connection.ReceiveChat(
                                                    "Monster.HarvestRare".Lang(owner.Connection.Language, MonsterInfo.MonsterName),
                                                    MessageType.System);

                                                foreach (SConnection con in owner.Connection.Observers)
                                                    con.ReceiveChat("Monster.HarvestRare".Lang(con.Language, MonsterInfo.MonsterName),
                                                        MessageType.System);
                                            }

                                            drops.Add(item);
                                            continue;
                                        }

                                        Cell cell = GetDropLocation(Config.DropDistance, owner) ?? CurrentCell;  //获取物品爆出的位置（范围，物品主人）

                                        ItemObject ob = new ItemObject
                                        {
                                            Item = item,
                                            Account = owner.Character.Account,
                                            ProtectTime = SEnvir.Now + Config.CanItemPickup,  //物品捡取时间  120秒
                                            MonsterDrop = true,
                                        };

                                        ob.Spawn(CurrentMap.Info, cell.Location);

                                        if (owner.Stats[Stat.CompanionCollection] > 0 && owner.Companion != null)   //宠物捡取碎片部分
                                        {
                                            long goldAmount = 0;

                                            if (ob.Item.Info.Effect == ItemEffect.Gold && ob.Account.GuildMember != null &&
                                                ob.Account.GuildMember.Guild.GuildTax > 0)
                                                goldAmount = (long)Math.Ceiling(ob.Item.Count * ob.Account.GuildMember.Guild.GuildTax);

                                            ItemCheck check = new ItemCheck(ob.Item, ob.Item.Count - goldAmount, ob.Item.Flags,
                                                ob.Item.ExpireTime);

                                            if (owner.Companion.CanGainItems(true, check)) ob.PickUpItem(owner.Companion);
                                        }

                                        //碎片爆了 这条爆率信息处理完毕
                                        continue;
                                    }
                                }
                                else
                                {
                                    //只爆成品  
                                    //再随机一次drop.Item.PartCount
                                    //结合上面的SEnvir.Random.Next(chance)
                                    //可以得出 成品爆率为 1/(原爆率*合成所需碎片个数)
                                    if (SEnvir.Random.Next(drop.Item.PartCount) == 0)
                                    {
                                        //成品
                                    }
                                    else
                                    {
                                        //只爆成品  
                                        //再随机一次drop.Item.PartCount
                                        //结合上面的SEnvir.Random.Next(chance)
                                        //可以得出 成品爆率为 1/(原爆率*合成所需碎片个数)
                                        if (SEnvir.Random.Next(drop.Item.PartCount) == 0)
                                        {
                                            //成品
                                        }
                                        else
                                        {
                                            //碎片
                                            if (isTest)
                                            {
                                                //进行爆率测试 不创建真实道具 仅记录
                                                string itemName = drop.Item.ItemName + " 碎片";
                                                if (DropTestDict.ContainsKey(itemName))
                                                {
                                                    DropTestDict[itemName] += amount;
                                                }
                                                else
                                                {
                                                    DropTestDict.Add(itemName, amount);
                                                }
                                                continue;
                                            }
                                            else
                                            {
                                                // 注意 这个item是碎片 而不是完整的物品
                                                UserItem item = SEnvir.CreateDropItem(SEnvir.ItemPartInfo);
                                                //记录物品来源
                                                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.Monster, MonsterInfo?.MonsterName, owner?.Name);

                                                item.AddStat(Stat.ItemIndex, drop.Item.Index, StatSource.Added);  //道具增加额外属性
                                                item.StatsChanged();  //道具刷新属性变化

                                                item.IsTemporary = true;

                                                if (NeedHarvest)
                                                {
                                                    if (drops == null)
                                                        drops = new List<UserItem>();

                                                    if (drop.Item.Rarity != Rarity.Common)
                                                    {
                                                        owner.Connection.ReceiveChat(
                                                            "Monster.HarvestRare".Lang(owner.Connection.Language, MonsterInfo.MonsterName),
                                                            MessageType.System);

                                                        foreach (SConnection con in owner.Connection.Observers)
                                                            con.ReceiveChat("Monster.HarvestRare".Lang(con.Language, MonsterInfo.MonsterName),
                                                                MessageType.System);
                                                    }

                                                    drops.Add(item);
                                                    continue;
                                                }

                                                Cell cell = GetDropLocation(Config.DropDistance, owner) ?? CurrentCell;  //获取物品爆出的位置（范围，物品主人）

                                                ItemObject ob = new ItemObject
                                                {
                                                    Item = item,
                                                    Account = owner.Character.Account,
                                                    ProtectTime = SEnvir.Now + Config.CanItemPickup,  //物品捡取时间  120秒
                                                    MonsterDrop = true,
                                                };

                                                ob.Spawn(CurrentMap.Info, cell.Location);

                                                if (owner.Stats[Stat.CompanionCollection] > 0 && owner.Companion != null)   //宠物捡取碎片部分
                                                {
                                                    long goldAmount = 0;

                                                    if (ob.Item.Info.Effect == ItemEffect.Gold && ob.Account.GuildMember != null &&
                                                        ob.Account.GuildMember.Guild.GuildTax > 0)
                                                        goldAmount = (long)Math.Ceiling(ob.Item.Count * ob.Account.GuildMember.Guild.GuildTax);

                                                    ItemCheck check = new ItemCheck(ob.Item, ob.Item.Count - goldAmount, ob.Item.Flags,
                                                        ob.Item.ExpireTime);

                                                    if (owner.Companion.CanGainItems(true, check)) ob.PickUpItem(owner.Companion);
                                                }

                                                //碎片爆了 这条爆率信息处理完毕
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        //成品数目
                        if (amount > 0)
                        {
                            drop.AmountTemp = amount;
                            ListDropInfoRestul.Add(drop);
                        }
                    }
                    else
                    {
                        //不爆
                        //更新个人爆率进度
                        if (!Config.PersonalDropDisabled)   //这里好像只起到个人爆率进度更新
                        {
                            if (userDrop != null)  //如果爆率不为空
                            {
                                if (drop.PartOnly)   //只爆碎片开关
                                {
                                    userDrop.Progress += 1 / ((decimal)chance * amount * drop.Item.PartCount);
                                }
                                else
                                {
                                    userDrop.Progress += 1 / ((decimal)chance * amount);
                                }
                            }
                        }
                        //个人爆率达到必爆了吗？
                        if (!Config.PersonalDropDisabled)   //这里个人爆率达到以后，出成品
                        {
                            if (userDrop != null)   //如果个人爆率不为空
                            {
                                if (drop.Item.Effect != ItemEffect.Gold && userDrop.Progress > userDrop.DropCount + amount)
                                {
                                    amount = (long)(userDrop.Progress - userDrop.DropCount);

                                    //owner.Connection.ReceiveChat($" {drop.Item.ItemName} 的Progress: {userDrop.Progress}, DropCount: {userDrop.DropCount}", MessageType.Hint);

                                    userDrop.DropCount += amount;
                                    result = true;
                                    //数目
                                    if (amount > 0)
                                    {
                                        drop.AmountTemp = amount;
                                        ListDropInfoRestul.Add(drop);
                                        /*if (!isTest)
                                        {
                                            owner.Connection.ReceiveChat($"个人爆率累积达到必爆, {drop.Item.ItemName} 被爆出了", MessageType.Hint);
                                        }*/
                                        if (isTest)
                                        {
                                            //进行爆率测试 不创建真实道具 仅记录
                                            string itemName = drop.Item.ItemName + " 个人爆率";
                                            if (DropTestDict.ContainsKey(itemName))
                                            {
                                                DropTestDict[itemName] += amount;
                                            }
                                            else
                                            {
                                                DropTestDict.Add(itemName, amount);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //处理下一条

                } //end of foreach (DropInfo drop in ListDropInfo) 

                if (ListDropInfoRestul.Count > 0)
                    dicDropSetGroupResult.Add(DropSetTemp, ListDropInfoRestul);
            }

            foreach (var DropSetIndex in dicDropSetGroupResult.Keys)
            {
                List<DropInfo> LisDropInroRandom = dicDropSetGroupResult[DropSetIndex];
                for (int k = 0; k < LisDropInroRandom.Count; k++)
                {
                    DropInfo DropInfoReuslt = null;
                    if (DropSetIndex == 0)
                    {
                        DropInfoReuslt = LisDropInroRandom[k];
                    }
                    else
                    {
                        int randindex = SEnvir.Random.Next(LisDropInroRandom.Count);
                        DropInfoReuslt = LisDropInroRandom[randindex];
                    }

                    var amount = DropInfoReuslt.AmountTemp;

                    if (isTest)
                    {
                        //进行爆率测试 不创建真实道具 仅记录
                        if (DropTestDict.ContainsKey(DropInfoReuslt.Item.ItemName))
                        {
                            DropTestDict[DropInfoReuslt.Item.ItemName] += amount;
                        }
                        else
                        {
                            DropTestDict.Add(DropInfoReuslt.Item.ItemName, amount);
                        }
                    }
                    else
                    {
                        while (amount > 0)  //如果数量大于0 循环
                        {
                            // 爆出成品
                            UserItem item = SEnvir.CreateDropItem(DropInfoReuslt.Item);  //道具是否增加极品

                            //记录物品来源
                            SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.Monster, MonsterInfo?.MonsterName, owner?.Name);

                            #region 怪物掉落物品

                            //python 触发
                            try
                            {
                                dynamic trig_player;
                                if (SEnvir.PythonEvent.TryGetValue("PlayerEvent_trig_player", out trig_player))
                                {
                                    PythonTuple args = PythonOps.MakeTuple(new object[] { owner, CurrentMap?.Info, MonsterInfo, item });
                                    SEnvir.ExecutePyWithTimer(trig_player, owner, "OnMonDropItem", args);
                                    //trig_player(owner, "OnMonDropItem", args);
                                }
                            }
                            catch (System.Data.SyntaxErrorException e)
                            {
                                string msg = "PlayerEvent Syntax error : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                            }
                            catch (SystemExitException e)
                            {
                                string msg = "PlayerEvent SystemExit : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(e);
                                SEnvir.Log(string.Format(msg, error));
                            }
                            catch (Exception ex)
                            {
                                string msg = "PlayerEvent Error loading plugin : \"{0}\"";
                                ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                                string error = eo.FormatException(ex);
                                SEnvir.Log(string.Format(msg, error));
                            }

                            #endregion

                            item.Count = Math.Min(DropInfoReuslt.Item.StackSize, amount);  //道具叠加数量
                            amount -= item.Count;

                            item.IsTemporary = true; //临时性道具

                            //道具掉落提示
                            string _temname;
                            _temname = System.Text.RegularExpressions.Regex.Replace(MonsterInfo.MonsterName, "[^A-Za-z\u4e00-\u9fa5]", "");
                            //MonsterInfo.MonsterName = _temname;    //怪物名字不显示数字

                            if (Config.ItemNotice)   //如果系统开启了高级道具掉落提示
                            {
                                if (item.Info.Rarity != Rarity.Common) //道具不是普通道具
                                {
                                    foreach (SConnection con in SEnvir.Connections)
                                        con.ReceiveChat("PlayerObject.MonsterItemNotice".Lang(con.Language, owner.Name, CurrentMap.Info.Description, _temname, item.Info.ItemName), MessageType.System);
                                }
                            }

                            if (NeedHarvest)  //割肉挖肉
                            {
                                if (drops == null)
                                    drops = new List<UserItem>();

                                if (item.Info.Rarity != Rarity.Common)
                                {
                                    owner.Connection.ReceiveChat("Monster.HarvestRare".Lang(owner.Connection.Language, MonsterInfo.MonsterName), MessageType.System);

                                    foreach (SConnection con in owner.Connection.Observers)
                                        con.ReceiveChat("Monster.HarvestRare".Lang(con.Language, MonsterInfo.MonsterName), MessageType.System);
                                }

                                drops.Add(item);
                                continue;
                            }

                            Cell cell = GetDropLocation(Config.DropDistance, owner) ?? CurrentCell;  //获取物品爆出的位置（范围，物品主人）
                            ItemObject ob = new ItemObject
                            {
                                Item = item,
                                Account = owner.Character.Account,
                                ProtectTime = SEnvir.Now + Config.CanItemPickup,   //物品捡取时间  120秒
                                MonsterDrop = true,
                            };

                            ob.Spawn(CurrentMap.Info, cell.Location);

                            if (owner.Stats[Stat.CompanionCollection] > 0 && owner.Companion != null)     //宠物捡取
                            {
                                long goldAmount = 0;

                                if (ob.Item.Info.Effect == ItemEffect.Gold && ob.Account.GuildMember != null &&
                                    ob.Account.GuildMember.Guild.GuildTax > 0)
                                    goldAmount = (long)Math.Ceiling(ob.Item.Count * ob.Account.GuildMember.Guild.GuildTax);

                                ItemCheck check = new ItemCheck(ob.Item, ob.Item.Count - goldAmount, ob.Item.Flags,
                                    ob.Item.ExpireTime);

                                if (owner.Companion.CanGainItems(true, check)) ob.PickUpItem(owner.Companion);
                            }
                        }
                    }
                    if (DropSetIndex > 0) break;
                }
            }

            if (EXPOwner.GroupMembers == null)  //如果没有队伍
            {
                foreach (UserQuest quest in owner.Character.Quests)
                {
                    if (quest.Completed) continue;
                    bool changed = false;

                    foreach (QuestTask task in quest.QuestInfo.Tasks)
                    {
                        bool valid = false;
                        int count = 0;
                        foreach (QuestTaskMonsterDetails details in task.MonsterDetails)
                        {
                            if (details.Monster.Index != MonsterInfo.Index) continue;
                            if (details.Map != null && CurrentMap.Info != details.Map) continue;

                            if (SEnvir.Random.Next(details.Chance) > 0) continue;

                            if ((DropSet & details.DropSet) != details.DropSet) continue;

                            valid = true;
                            count = details.Amount;
                            break;
                        }

                        if (!valid) continue;

                        UserQuestTask userTask = quest.Tasks.FirstOrDefault(x => x.Task == task);

                        if (userTask == null)
                        {
                            userTask = SEnvir.UserQuestTaskList.CreateNewObject();
                            userTask.Task = task;
                            userTask.Quest = quest;
                        }

                        if (userTask.Completed) continue;

                        switch (task.Task)
                        {
                            case QuestTaskType.KillMonster:
                                userTask.Amount = Math.Min(task.Amount, userTask.Amount + count);
                                changed = true;
                                break;
                            case QuestTaskType.GainItem:
                                if (task.ItemParameter == null) continue;

                                UserItem item = SEnvir.CreateDropItem(task.ItemParameter);
                                //记录物品来源
                                SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.Monster, MonsterInfo?.MonsterName + "-" + "任务道具".Lang(owner.Connection.Language), owner?.Name);

                                item.Count = count;
                                item.UserTask = userTask;
                                item.Flags |= UserItemFlags.QuestItem;

                                item.IsTemporary = true;

                                if (NeedHarvest)
                                {
                                    if (drops == null)
                                        drops = new List<UserItem>();

                                    drops.Add(item);
                                    continue;
                                }

                                Cell cell = GetDropLocation(Config.DropDistance, owner) ?? CurrentCell;
                                ItemObject ob = new ItemObject
                                {
                                    Item = item,
                                    Account = owner.Character.Account,
                                    MonsterDrop = true,
                                };

                                ob.Spawn(CurrentMap.Info, cell.Location);

                                userTask.Objects.Add(ob);

                                if (owner.Stats[Stat.CompanionCollection] > 0 && owner.Companion != null)
                                {
                                    long goldAmount = 0;

                                    if (ob.Item.Info.Effect == ItemEffect.Gold && ob.Account.GuildMember != null &&
                                        ob.Account.GuildMember.Guild.GuildTax > 0)
                                        goldAmount = (long)Math.Ceiling(ob.Item.Count * ob.Account.GuildMember.Guild.GuildTax);

                                    ItemCheck check = new ItemCheck(ob.Item, ob.Item.Count - goldAmount, ob.Item.Flags,
                                        ob.Item.ExpireTime);

                                    if (owner.Companion.CanGainItems(true, check)) ob.PickUpItem(owner.Companion);
                                }
                                break;
                        }
                    }
                    if (changed)
                        owner.Enqueue(new S.QuestChanged { Quest = quest.ToClientInfo() });
                }
            }

            if (result && owner.Companion != null)
                owner.Companion.SearchTime = DateTime.MinValue;

            if (!NeedHarvest) return;

            if (Drops == null)
                Drops = new Dictionary<AccountInfo, List<UserItem>>();

            Drops[owner.Character.Account] = drops;
        }

        /// <summary>
        /// 任务道具掉落 组队下
        /// </summary>
        /// <param name="owner">成员</param>
        public void QuestDrop(PlayerObject owner)
        {
            bool result = false;

            List<UserItem> drops = null;

            Drops?.TryGetValue(owner.Character.Account, out drops); //获得之前drops 然后和任务掉落物品合并

            foreach (UserQuest quest in owner.Character.Quests)
            {
                if (quest.Completed) continue;
                bool changed = false;

                foreach (QuestTask task in quest.QuestInfo.Tasks)
                {
                    bool valid = false;
                    int count = 0;
                    foreach (QuestTaskMonsterDetails details in task.MonsterDetails)
                    {
                        if (details.Monster.Index != MonsterInfo.Index) continue;
                        if (details.Map != null && CurrentMap.Info.Index != details.Map.Index) continue;

                        if (SEnvir.Random.Next(details.Chance) > 0) continue;

                        if ((DropSet & details.DropSet) != details.DropSet) continue;

                        valid = true;
                        count = details.Amount;
                        break;
                    }

                    if (!valid) continue;

                    UserQuestTask userTask = quest.Tasks.FirstOrDefault(x => x.Task == task);

                    if (userTask == null)
                    {
                        userTask = SEnvir.UserQuestTaskList.CreateNewObject();
                        userTask.Task = task;
                        userTask.Quest = quest;
                    }

                    if (userTask.Completed) continue;

                    switch (task.Task)
                    {
                        case QuestTaskType.KillMonster:
                            userTask.Amount = Math.Min(task.Amount, userTask.Amount + count);
                            changed = true;
                            break;
                        case QuestTaskType.GainItem:
                            if (task.ItemParameter == null) continue;

                            UserItem item = SEnvir.CreateDropItem(task.ItemParameter);
                            //记录物品来源
                            SEnvir.RecordTrackingInfo(item, CurrentMap?.Info?.Description, ObjectType.Monster, MonsterInfo?.MonsterName + "-" + "任务道具".Lang(owner.Connection.Language), owner?.Name);

                            item.Count = count;
                            item.UserTask = userTask;
                            item.Flags |= UserItemFlags.QuestItem;

                            item.IsTemporary = true;

                            if (NeedHarvest)
                            {
                                if (drops == null)
                                    drops = new List<UserItem>();

                                drops.Add(item);
                                continue;
                            }

                            Cell cell = GetDropLocation(Config.DropDistance, owner) ?? CurrentCell;
                            ItemObject ob = new ItemObject
                            {
                                Item = item,
                                Account = owner.Character.Account,
                                MonsterDrop = true,
                            };

                            ob.Spawn(CurrentMap.Info, cell.Location);

                            userTask.Objects.Add(ob);

                            if (owner.Stats[Stat.CompanionCollection] > 0 && owner.Companion != null)
                            {
                                long goldAmount = 0;

                                if (ob.Item.Info.Effect == ItemEffect.Gold && ob.Account.GuildMember != null &&
                                    ob.Account.GuildMember.Guild.GuildTax > 0)
                                    goldAmount = (long)Math.Ceiling(ob.Item.Count * ob.Account.GuildMember.Guild.GuildTax);

                                ItemCheck check = new ItemCheck(ob.Item, ob.Item.Count - goldAmount, ob.Item.Flags,
                                    ob.Item.ExpireTime);

                                if (owner.Companion.CanGainItems(true, check)) ob.PickUpItem(owner.Companion);
                            }
                            break;
                    }
                }
                if (changed)
                    owner.Enqueue(new S.QuestChanged { Quest = quest.ToClientInfo() });
            }

            if (result && owner.Companion != null)
                owner.Companion.SearchTime = DateTime.MinValue;

            if (!NeedHarvest) return;

            if (Drops == null)
                Drops = new Dictionary<AccountInfo, List<UserItem>>();

            Drops[owner.Character.Account] = drops;
        }

        /// <summary>
        /// 转动 旋转 冲撞
        /// </summary>
        /// <param name="direction"></param>
        public virtual void Turn(MirDirection direction)
        {
            if (!CanMove) return;

            UpdateMoveTime();

            Direction = direction;

            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }
        /// <summary>
        /// 走动 移动 行走
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public virtual bool Walk(MirDirection direction)
        {
            //如果不能移动
            if (!CanMove) return false;
            //如果 单元格 = 当前地图.取得单元格(移动(当前位置 方向))
            Cell cell = CurrentMap.GetCell(Functions.Move(CurrentLocation, direction));
            //如果 单元格 为空 
            if (cell == null) return false;
            //如果 单元格 有玩家
            if (cell.IsBlocking(this, false) && !MonsterInfo.IsBoss && !(MonsterInfo.AI == 149)) return false;  //怪物穿人

            //如果 躲避火墙 对象不为空
            if (AvoidFireWall && cell.Objects != null)
            {
                foreach (MapObject ob in cell.Objects)
                {
                    if (ob.Race != ObjectType.Spell) continue;
                    SpellObject spell = (SpellObject)ob;

                    switch (spell.Effect)
                    {
                        case SpellEffect.FireWall:
                        case SpellEffect.MonsterFireWall:
                        case SpellEffect.Tempest:
                        case SpellEffect.IceRain:
                            break;
                        default:
                            continue;
                    }

                    if (spell.Owner == null || !spell.Owner.CanAttackTarget(this)) continue;

                    return false;
                }
            }

            BuffRemove(BuffType.Invisibility);
            BuffRemove(BuffType.Transparency);

            Direction = direction;

            UpdateMoveTime();

            PreventSpellCheck = true;
            CurrentCell = cell; //.GetMovement(this);
            PreventSpellCheck = false;

            RemoveAllObjects();
            AddAllObjects();

            Broadcast(new S.ObjectMove { ObjectID = ObjectID, Direction = direction, Location = CurrentLocation, Distance = 1 });
            if (PetOwner != null && Functions.Distance(CurrentLocation, PetOwner.CurrentLocation) > Config.MaxViewRange && PetOwner.CurrentMap == CurrentMap)
            {
                PetOwner.Connection.Enqueue(new S.WarWeapLocation { Location = CurrentLocation, ObjectID = ObjectID });
            }
            CheckSpellObjects();
            return true;
        }
        /// <summary>
        /// 移动到指定的坐标
        /// </summary>
        /// <param name="target"></param>
        public virtual void MoveTo(Point target)  //移动到指定坐标
        {
            if (CurrentLocation == target) return;

            if (Functions.InRange(target, CurrentLocation, 1))
            {
                Cell cell = CurrentMap.GetCell(target);

                if (cell == null || cell.IsBlocking(this, false)) return;    //怪物穿人
            }

            MirDirection direction = Functions.DirectionFromPoint(CurrentLocation, target);

            int rotation = SEnvir.Random.Next(2) == 0 ? 1 : -1;

            for (int d = 0; d < 8; d++)
            {
                if (Walk(direction)) return;

                direction = Functions.ShiftDirection(direction, rotation);
            }
        }

        /// <summary>
        /// BUFF增加
        /// </summary>
        /// <param name="type"></param>
        /// <param name="remainingTicks"></param>
        /// <param name="stats"></param>
        /// <param name="visible"></param>
        /// <param name="pause"></param>
        /// <param name="tickRate"></param>
        /// <param name="fromCustomBuff"></param>
        /// <returns></returns>
        public override BuffInfo BuffAdd(BuffType type, TimeSpan remainingTicks, Stats stats, bool visible, bool pause, TimeSpan tickRate, int fromCustomBuff = 0)
        {
            BuffInfo info = base.BuffAdd(type, remainingTicks, stats, visible, pause, tickRate, fromCustomBuff);

            info.IsTemporary = true;

            return info;
        }
        /// <summary>
        /// 位置改变时
        /// </summary>
        protected override void OnLocationChanged()
        {
            base.OnLocationChanged();

            if (CurrentCell == null) return;

            InSafeZone = CurrentCell.SafeZone != null;
        }
        /// <summary>
        /// 收获改变时
        /// </summary>
        public void HarvestChanged()
        {
            Skeleton = true;

            if (Drops == null)
                DeadTime -= Config.HarvestDuration;

            foreach (PlayerObject player in SeenByPlayers)  //遍历  玩家对象 玩家 在范围内 被玩家看到
                if (Drops == null || !Drops.ContainsKey(player.Character.Account))
                    player.Enqueue(new S.ObjectHarvested { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }
        /// <summary>
        /// 获取信息包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override Packet GetInfoPacket(PlayerObject ob)
        {
            return new S.ObjectMonster
            {
                ObjectID = ObjectID,
                MonsterIndex = MonsterInfo.Index,

                Location = CurrentLocation,

                NameColour = NameColour,
                Direction = Direction,
                Dead = Dead,

                PetOwner = PetOwner?.Name,

                Skeleton = NeedHarvest && Skeleton && (Drops == null || !Drops.ContainsKey(ob.Character.Account)),

                Poison = Poison,

                EasterEvent = EasterEventMob,
                HalloweenEvent = HalloweenEventMob,
                ChristmasEvent = ChristmasEventMob,

                Buffs = Buffs.Where(x => x.Visible).Select(x => x.Type).ToList()
            };
        }
        /// <summary>
        /// 获取数据包
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public override Packet GetDataPacket(PlayerObject ob)
        {
            return new S.DataObjectMonster
            {
                ObjectID = ObjectID,

                MonsterIndex = MonsterInfo.Index,

                MapIndex = CurrentMap.Info.Index,
                CurrentLocation = CurrentLocation,

                Health = DisplayHP,
                Stats = Stats,
                Dead = Dead,

                PetOwner = PetOwner?.Name,
            };
        }

        // 处理py脚本AI
        public void ProcessPyAISetting()
        {
            if (MonsterInfo == null) return;

            // 检查此怪物是否在监视列表
            if (SEnvir.WathchingMonsters.ContainsKey(MonsterInfo.Index))
            {
                try
                {
                    if (SEnvir.PythonEvent.TryGetValue("MonsterEvent_trig_mon", out dynamic trigMon))
                    {
                        PythonTuple args = PythonOps.MakeTuple(new object[] { this });
                        SEnvir.ExecutePyWithTimer(trigMon, this, "OnProcessAI", args);
                    }
                }
                catch (SyntaxErrorException e)
                {
                    string msg = "怪物事件（语法错误） : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (SystemExitException e)
                {
                    string msg = "怪物事件（系统退出） : \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);
                    SEnvir.Log(string.Format(msg, error));
                }
                catch (Exception ex)
                {
                    string msg = "怪物事件（其他错误）: \"{0}\"";
                    ExceptionOperations eo = SEnvir.engine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(ex);
                    SEnvir.Log(string.Format(msg, error));
                }
            }
        }

        // 给怪物加buff
        public void AddBuff(string name, PythonDictionary stats, int tickSeconds, int tickRateSeconds = 1, int customBuffIndex = 0)
        {
            Stats buffStats = new Stats();
            foreach (var kvp in stats)
            {
                buffStats[(Stat)kvp.Key] = (int)kvp.Value;
            }

            if (customBuffIndex != 0)
            {
                BuffInfo buff = BuffAdd(BuffType.CustomBuff, TimeSpan.FromSeconds(tickSeconds), buffStats, true, false, TimeSpan.FromSeconds(tickRateSeconds), customBuffIndex);
                buff.Name = name;
            }
            else
            {
                BuffInfo buff = BuffAdd(BuffType.MonsterBuff, TimeSpan.FromSeconds(tickSeconds), buffStats, true, false, TimeSpan.FromSeconds(tickRateSeconds), customBuffIndex);
                buff.Name = name;
            }
            // 默认的 BuffTime 是怪物刷新的时间
            // 这里进行更新
            BuffTime = SEnvir.Now;
        }

        /// <summary>
		/// 怪物说话
		/// </summary>
		/// <param name="SayStr"></param>
		public void MonSay(string SayStr)
        {
            if (SayStr == "") return;

            string _temname;
            // 只过滤结尾的数字
            _temname = MonsterInfo.MonsterName.TrimEnd(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' });

            string text = string.Format("{0}: {1}", _temname, SayStr);

            foreach (PlayerObject eplayer in SeenByPlayers)
            {
                if (!Functions.InRange(CurrentLocation, eplayer.CurrentLocation, Config.MaxViewRange)) continue;

                eplayer.Connection.ReceiveChat(text, MessageType.Normal, ObjectID);

                foreach (SConnection observer in eplayer.Connection.Observers)
                {
                    observer.ReceiveChat(text, MessageType.Normal, ObjectID);
                }
            }
        }
    }
}