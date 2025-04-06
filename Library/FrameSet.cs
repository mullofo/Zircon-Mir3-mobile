using System;
using System.Collections.Generic;

namespace Library
{
    /// <summary>
    /// 动画框架组成
    /// </summary>
    public sealed class FrameSet
    {
        /// <summary>
        /// 传奇3角色
        /// </summary>
        public static Dictionary<MirAnimation, Frame> Players;
        /// <summary>
        /// 传奇2角色
        /// </summary>
        public static Dictionary<MirAnimation, Frame> Players_Mir2;
        /// <summary>
        /// 默认道具
        /// </summary>
        public static Dictionary<MirAnimation, Frame> DefaultItem;
        /// <summary>
        /// 默认NPC
        /// </summary>
        public static Dictionary<MirAnimation, Frame> DefaultNPC;
        /// <summary>
        /// 默认怪物
        /// </summary>
        public static Dictionary<MirAnimation, Frame> DefaultMonster;
        /// <summary>
        /// 指定怪物的动画帧
        /// </summary>
        public static Dictionary<MirAnimation, Frame>
            ForestYeti, ChestnutTree, CarnivorousPlant,
            DevouringGhost,
            Larva,
            ZumaGuardian, ZumaKing,
            Monkey,
            NumaMage, CursedCactus, NetherWorldGate,
            WestDesertLizard,
            BanyaGuard, EmperorSaWoo,
            JinchonDevil,
            ArchLichTaeda,
            ShinsuBig,
            PachonTheChaosBringer,
            IcySpiritGeneral,
            FieryDancer, EmeraldDancer, QueenOfDawn,
            JinamStoneGate, OYoungBeast, YumgonWitch, JinhwanSpirit, ChiwooGeneral, DragonQueen, DragonLord,
            FerociousIceTiger,
            SamaFireGuardian, Phoenix, EnshrinementBox, BloodStone, SamaCursedBladesman, SamaCursedSlave, SamaProphet, SamaSorcerer,
            EasterEvent,
            OrangeTiger, RedTiger, OrangeBossTiger, BigBossTiger,

            SDMob3, SDMob8, SDMob15, SDMob16, SDMob17, SDMob18, SDMob19, SDMob21, SDMob22, SDMob23, SDMob24, SDMob25, SDMob26,

            LobsterLord, LobsterSpawn,

            DeadTree, BobbitWorm,
            MonasteryMon1, MonasteryMon3,

            //密林
            CrazedPrimate, HellBringer, YurinMon0, YurinMon1, WhiteBeardedTiger, HardenedRhino, Mammoth, CursedSlave1, CursedSlave2, CursedSlave3, PoisonousGolem,

            //花园
            GardenSoldier, GardenDefender, RedBlossom, BlueBlossom, FireBird,

            //古墓
            Terracotta1, Terracotta2, Terracotta3, TerracottaSub, TerracottaBoss,

            //练功木桩
            Practitioner,

            //沙巴克主门     沙巴克边门
            SabukPrimeGate, SabukLeftDoor, SabukRightDoor, Catapult, Ballista,

            //马
            BrownHorse, WhiteHorse, BlackHorse, RedHorse,
            //诺玛教主    沙城守护者
            MasterNorma, SabakGuardian,
            //栏栅
            Fence,
             //诺玛圣骑士  诺玛大司令     诺玛斧兵王          诺玛装甲王      诺玛统领       PharaohNorma
             NormaPaladin, NomaCommander, NomaAxeWarriorKing, NormaArmorKing, CommanderNoma, PharaohNorma;

        /// <summary>
        /// 所有动画帧定义
        /// </summary>
        static FrameSet()
        {
            Players = new Dictionary<MirAnimation, Frame>   //玩家
            {
                [MirAnimation.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Running] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.CreepStanding] = new Frame(1680, 4, 10, TimeSpan.FromMilliseconds(500)), //Stealth Standing
                [MirAnimation.CreepWalkFast] = new Frame(1760, 6, 10, TimeSpan.FromMilliseconds(100)), //Stealth Walk
                [MirAnimation.CreepWalkSlow] = new Frame(1760, 6, 10, TimeSpan.FromMilliseconds(200)), //Stealth Walk
                [MirAnimation.Pushed] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                //[MirAnimation.Pushed2] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true },
                [MirAnimation.Stance] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(500)),                             //站立姿势                           
                [MirAnimation.Harvest] = new Frame(480, 2, 10, TimeSpan.FromMilliseconds(300)),                            //割肉
                [MirAnimation.Combat1] = new Frame(560, 5, 10, TimeSpan.FromMilliseconds(60)), //Proj Spell                 平推魔法动作
                [MirAnimation.Combat2] = new Frame(640, 5, 10, TimeSpan.FromMilliseconds(100)), //Target Spell               举手魔法动作
                [MirAnimation.Combat3] = new Frame(720, 6, 10, TimeSpan.FromMilliseconds(100)), //Default Attack (WWT)       平A
                [MirAnimation.Combat4] = new Frame(800, 6, 10, TimeSpan.FromMilliseconds(100)), //Default 1 Handed (Sin)     十方斩
                [MirAnimation.Combat5] = new Frame(880, 10, 10, TimeSpan.FromMilliseconds(60)), //Lotus 1 Handed            翔空
                [MirAnimation.Combat6] = new Frame(960, 10, 10, TimeSpan.FromMilliseconds(90)), //                          莲月剑法
                [MirAnimation.Combat7] = new Frame(1040, 10, 10, TimeSpan.FromMilliseconds(100)), //Kick                    空拳刀法
                [MirAnimation.Combat8] = new Frame(1120, 6, 10, TimeSpan.FromMilliseconds(50)) { StaticSpeed = true }, //Dash 野蛮冲撞
                [MirAnimation.Combat9] = new Frame(1200, 10, 10, TimeSpan.FromMilliseconds(100)), //Evasion Cast 闪避
                [MirAnimation.Combat10] = new Frame(1280, 10, 10, TimeSpan.FromMilliseconds(60)), //Sweet Brier 1 Handed 投掷物品 打水漂
                [MirAnimation.Combat11] = new Frame(1360, 10, 10, TimeSpan.FromMilliseconds(60)), //Duel Wield default 双手武器挥舞
                [MirAnimation.Combat12] = new Frame(1440, 10, 10, TimeSpan.FromMilliseconds(60)), //Sweet brier Duel wield
                [MirAnimation.Combat13] = new Frame(1520, 6, 10, TimeSpan.FromMilliseconds(100)), //Lotus Duel wield
                [MirAnimation.Combat14] = new Frame(1600, 8, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.Combat15] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(200)),

                //狂涛泉涌动作帧
                [MirAnimation.DragonRepulseStart] = new Frame(1600, 6, 10, TimeSpan.FromMilliseconds(100)),   //狂涛泉涌开始
                [MirAnimation.DragonRepulseMiddle] = new Frame(1605, 1, 10, TimeSpan.FromMilliseconds(1000)), //狂涛泉涌过程
                [MirAnimation.DragonRepulseEnd] = new Frame(1606, 2, 10, TimeSpan.FromMilliseconds(100)),     //狂涛泉涌结束

                [MirAnimation.Struck] = new Frame(1840, 3, 10, TimeSpan.FromMilliseconds(50)),             //后仰 应该说是被打中的效果
                [MirAnimation.Die] = new Frame(1920, 10, 10, TimeSpan.FromMilliseconds(100)),              //死亡消失过程
                [MirAnimation.Dead] = new Frame(1929, 1, 10, TimeSpan.FromMilliseconds(1000)),             //地面尸体

                //骑马动作帧
                [MirAnimation.HorseStanding] = new Frame(2240, 4, 10, TimeSpan.FromMilliseconds(500)), //骑马站立
                [MirAnimation.HorseWalking] = new Frame(2320, 6, 10, TimeSpan.FromMilliseconds(100)),  //骑马走
                [MirAnimation.HorseRunning] = new Frame(2400, 6, 10, TimeSpan.FromMilliseconds(100)),  //骑马跑
                [MirAnimation.HorseStruck] = new Frame(2480, 3, 10, TimeSpan.FromMilliseconds(100)),   //骑马撞

                //钓鱼动作帧
                [MirAnimation.FishingCast] = new Frame(2000, 8, 10, TimeSpan.FromMilliseconds(100)),   //钓鱼抛竿
                [MirAnimation.FishingWait] = new Frame(2080, 6, 10, TimeSpan.FromMilliseconds(100)),   //钓鱼等待
                [MirAnimation.FishingReel] = new Frame(2160, 8, 10, TimeSpan.FromMilliseconds(100)),   //钓鱼收线

                //离魂邪风动作帧
                [MirAnimation.ChannellingStart] = new Frame(560, 4, 10, TimeSpan.FromMilliseconds(100)),   //离魂邪风开始
                [MirAnimation.ChannellingMiddle] = new Frame(563, 1, 10, TimeSpan.FromMilliseconds(1000)), //离魂邪风过程
                [MirAnimation.ChannellingEnd] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(60)),        //离魂邪风结束
            };
            Players[MirAnimation.Combat1].Delays[1] = TimeSpan.FromMilliseconds(200);
            Players[MirAnimation.Combat2].Delays[3] = TimeSpan.FromMilliseconds(200);

            #region 传奇2动作

            Players_Mir2 = new Dictionary<MirAnimation, Frame>   //玩家
            {
                [MirAnimation.Standing] = new Frame(0, 4, 8, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(64, 6, 8, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Running] = new Frame(128, 6, 8, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Stance] = new Frame(192, 1, 1, TimeSpan.FromMilliseconds(500)),  //僵持
                [MirAnimation.Harvest] = new Frame(456, 2, 2, TimeSpan.FromMilliseconds(300)), //割肉
                [MirAnimation.Combat3] = new Frame(200, 6, 8, TimeSpan.FromMilliseconds(100)), //平A
                [MirAnimation.Combat2] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //举手魔法动作
                [MirAnimation.Combat1] = new Frame(392, 6, 8, TimeSpan.FromMilliseconds(60)), //no                 平推魔法动作
                [MirAnimation.Struck] = new Frame(472, 3, 8, TimeSpan.FromMilliseconds(50)), //挨打
                [MirAnimation.Die] = new Frame(536, 4, 8, TimeSpan.FromMilliseconds(100)), //死亡倒地
                [MirAnimation.Dead] = new Frame(539, 1, 8, TimeSpan.FromMilliseconds(1000)),//死后躺地

                [MirAnimation.CreepStanding] = new Frame(128, 6, 8, TimeSpan.FromMilliseconds(100)), //no Stealth Standing
                [MirAnimation.CreepWalkFast] = new Frame(128, 6, 8, TimeSpan.FromMilliseconds(100)), //no Stealth Walk
                [MirAnimation.CreepWalkSlow] = new Frame(128, 6, 8, TimeSpan.FromMilliseconds(100)), //no Stealth Walk
                [MirAnimation.Pushed] = new Frame(472, 3, 8, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true }, //no

                [MirAnimation.Combat4] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //Default 1 Handed (Sin)     十方斩
                [MirAnimation.Combat5] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //Lotus 1 Handed            翔空
                [MirAnimation.Combat6] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //                          莲月剑法

                [MirAnimation.Combat7] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //Kick                    空拳刀法
                [MirAnimation.Combat8] = new Frame(128, 6, 8, TimeSpan.FromMilliseconds(100)) { StaticSpeed = true }, //Dash 野蛮冲撞
                [MirAnimation.Combat9] = new Frame(192, 1, 1, TimeSpan.FromMilliseconds(500)), //Evasion Cast 闪避

                [MirAnimation.Combat10] = new Frame(264, 6, 8, TimeSpan.FromMilliseconds(60)), //Sweet Brier 1 Handed 投掷物品 打水漂
                [MirAnimation.Combat11] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //Duel Wield default 双手武器挥舞
                [MirAnimation.Combat12] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //Sweet brier Duel wield
                [MirAnimation.Combat13] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //Lotus Duel wield

                [MirAnimation.Combat14] = new Frame(392, 6, 8, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.Combat15] = new Frame(128, 6, 8, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.DragonRepulseStart] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //狂涛泉涌开始
                [MirAnimation.DragonRepulseMiddle] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //狂涛泉涌过程
                [MirAnimation.DragonRepulseEnd] = new Frame(328, 8, 8, TimeSpan.FromMilliseconds(100)), //狂涛泉涌结束

                //todo 这里需要看传奇2素材的动作位置
                [MirAnimation.HorseStanding] = new Frame(2240, 4, 10, TimeSpan.FromMilliseconds(500)), //Horse Standing
                [MirAnimation.HorseWalking] = new Frame(2320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.HorseRunning] = new Frame(2400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.HorseStruck] = new Frame(2480, 3, 10, TimeSpan.FromMilliseconds(100)),

                //钓鱼动作帧
                [MirAnimation.FishingCast] = new Frame(2000, 8, 10, TimeSpan.FromMilliseconds(100)),   //钓鱼抛竿
                [MirAnimation.FishingWait] = new Frame(2080, 6, 10, TimeSpan.FromMilliseconds(100)),   //钓鱼等待
                [MirAnimation.FishingReel] = new Frame(2160, 8, 10, TimeSpan.FromMilliseconds(100)),   //钓鱼收线

                //离魂邪风动作帧
                [MirAnimation.ChannellingStart] = new Frame(560, 4, 10, TimeSpan.FromMilliseconds(100)),   //离魂邪风开始
                [MirAnimation.ChannellingMiddle] = new Frame(563, 1, 10, TimeSpan.FromMilliseconds(1000)), //离魂邪风过程
                [MirAnimation.ChannellingEnd] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(60)),        //离魂邪风结束
            };
            //TODO WHAT IS THIS???
            Players_Mir2[MirAnimation.Combat1].Delays[1] = TimeSpan.FromMilliseconds(200);
            Players_Mir2[MirAnimation.Combat2].Delays[3] = TimeSpan.FromMilliseconds(200);

            #endregion

            /*
            Assassin = new Dictionary<MirAction, Frame>
            {
                [MirAction.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAction.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Running] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Stance] = new Frame(560, 3, 10, TimeSpan.FromMilliseconds(500)),

                [MirAction.Harvest] = new Frame(480, 2, 10, TimeSpan.FromMilliseconds(300)),
                [MirAction.Attack1] = new Frame(720, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Struck] = new Frame(1840, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Die] = new Frame(1920, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAction.Dead] = new Frame(1929, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };*/

            DefaultItem = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(1000)),
            };

            DefaultNPC = new Dictionary<MirAnimation, Frame>   //默认NPC 4图站立
            {
                [MirAnimation.Standing] = new Frame(0, 4, 0, TimeSpan.FromMilliseconds(200)),   //NPC帧速度
            };

            DefaultMonster = new Dictionary<MirAnimation, Frame>  //默认怪物
            {
                [MirAnimation.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(500)),                   //站立 0开始  4图 10帧
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(100)),                   //行走 80开始 6图 10帧
                [MirAnimation.Pushed] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },  //推 80开始 6图 10帧
                [MirAnimation.Combat1] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),                  //攻击  160开始  6图  10帧
                [MirAnimation.Combat2] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                //修正自定义怪物spell动作
                [MirAnimation.Combat4] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat5] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat6] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat7] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat8] = new Frame(160, 6, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),                  //被攻击  240开始 2图 10帧
                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),                    //死亡效果 320开始 10图  10帧
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(1000)),                   //死亡地面 329开始  1图  10帧
                [MirAnimation.Skeleton] = new Frame(880, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Show] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.StoneStanding] = new Frame(640, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            ForestYeti = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Die] = new Frame(320, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(323, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            ChestnutTree = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            CarnivorousPlant = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Show] = new Frame(640, 8, 0, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
                [MirAnimation.Hide] = new Frame(640, 8, 0, TimeSpan.FromMilliseconds(100)),
            };

            DevouringGhost = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            Larva = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(500)),
            };

            ZumaGuardian = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(640, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            ZumaKing = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(640, 20, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.StoneStanding] = new Frame(640, 1, 0, TimeSpan.FromMilliseconds(500)),
            };

            Monkey = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            NetherWorldGate = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 0, TimeSpan.FromMilliseconds(200)),
            };

            CursedCactus = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            NumaMage = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            WestDesertLizard = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            BanyaGuard = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            JinchonDevil = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Combat3] = new Frame(480, 8, 10, TimeSpan.FromMilliseconds(70)),
            };

            EmperorSaWoo = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            ArchLichTaeda = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(720, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(739, 1, 20, TimeSpan.FromMilliseconds(500)),
            };

            PachonTheChaosBringer = new Dictionary<MirAnimation, Frame>    //霸王教主
            {
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                //[MirAnimation.DragonRepulseStart] = new Frame(480, 7, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                //[MirAnimation.DragonRepulseMiddle] = new Frame(486, 1, 10, TimeSpan.FromMilliseconds(1000)), //Summon Puppet ?
                //[MirAnimation.DragonRepulseEnd] = new Frame(487, 3, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
            };

            IcySpiritGeneral = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            FieryDancer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
            };

            EmeraldDancer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 20, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(569, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            QueenOfDawn = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            OYoungBeast = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),
            };

            YumgonWitch = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
            };

            JinhwanSpirit = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            ChiwooGeneral = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            DragonQueen = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            DragonLord = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
            };

            FerociousIceTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(560, 16, 0, TimeSpan.FromMilliseconds(40)),
                [MirAnimation.Combat3] = new Frame(560, 16, 0, TimeSpan.FromMilliseconds(100)),
            };
            SamaFireGuardian = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            Phoenix = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(489, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            EnshrinementBox = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Die] = new Frame(80, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(89, 1, 0, TimeSpan.FromMilliseconds(500)),
            };

            BloodStone = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 4, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(240, 2, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Die] = new Frame(320, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 0, TimeSpan.FromMilliseconds(500)),
            };
            SamaCursedBladesman = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            SamaCursedSlave = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SamaProphet = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(50, 4, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(130, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(210, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(290, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(370, 3, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(450, 10, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(459, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            SamaSorcerer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(489, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            EasterEvent = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Show] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.StoneStanding] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.DragonRepulseStart] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
                [MirAnimation.DragonRepulseMiddle] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(1000)), //Summon Puppet ?
                [MirAnimation.DragonRepulseEnd] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(100)), //Summon Puppet ?
            };

            OrangeTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            RedTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            OrangeBossTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(406, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            BigBossTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat2] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)), //Stab
                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),//Roar
                [MirAnimation.Combat4] = new Frame(560, 10, 10, TimeSpan.FromMilliseconds(100)),//Roar 2
            };

            SDMob3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
                [MirAnimation.Hide] = new Frame(640, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            SDMob8 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            SDMob15 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SDMob16 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Walking] = new Frame(80, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 7, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)), //Bugged?
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };
            SDMob17 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SDMob18 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            SDMob19 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
                //[MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //[MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };

            SDMob21 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
                //[MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //[MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };

            SDMob22 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(325, 1, 10, TimeSpan.FromMilliseconds(500)),
                //[MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //[MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };
            SDMob23 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(500)),
                //[MirAnimation.Show] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)),
                //[MirAnimation.Hide] = new Frame(640, 8, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true },
            };

            SDMob24 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 9, 10, TimeSpan.FromMilliseconds(70)),
            };

            SDMob25 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(70)),
            };

            SDMob26 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(70)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(500)),
            };

            LobsterLord = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(20, 6, 0, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(30, 7, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(40, 7, 0, TimeSpan.FromMilliseconds(100)), //Right Side, Left Claw
                [MirAnimation.Combat3] = new Frame(60, 7, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat4] = new Frame(70, 7, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Combat5] = new Frame(80, 7, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Combat6] = new Frame(110, 8, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Combat7] = new Frame(120, 4, 0, TimeSpan.FromMilliseconds(100)), //Left Side, Right Claw?
                [MirAnimation.Struck] = new Frame(50, 4, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(130, 9, 0, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(138, 1, 0, TimeSpan.FromMilliseconds(500)),
            };

            JinamStoneGate = new Dictionary<MirAnimation, Frame>  //赤龙石门
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
            };


            DeadTree = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Die] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Dead] = new Frame(0, 1, 0, TimeSpan.FromMilliseconds(200)),
            };

            MonasteryMon1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 15, 20, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(488, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };
            MonasteryMon3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 15, 20, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 4, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(568, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            BobbitWorm = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Show] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Hide] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            CrazedPrimate = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            HellBringer = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(480, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat4] = new Frame(560, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),
            };

            YurinMon0 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            YurinMon1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            WhiteBeardedTiger = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            HardenedRhino = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            Mammoth = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            CursedSlave1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            CursedSlave2 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            CursedSlave3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Pushed] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            PoisonousGolem = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 6, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(80)),
                [MirAnimation.Pushed] = new Frame(80, 10, 10, TimeSpan.FromMilliseconds(50)) { Reversed = true, StaticSpeed = true },
                [MirAnimation.Combat3] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            GardenSoldier = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(488, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            GardenDefender = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 9, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            RedBlossom = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat1] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(240, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(409, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            BlueBlossom = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 10, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Combat2] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            FireBird = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(160, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat2] = new Frame(320, 5, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(240, 5, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat4] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(569, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            Terracotta1 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 11, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(490, 1, 20, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            Terracotta2 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 12, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(491, 1, 20, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            Terracotta3 = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(400, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(480, 10, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(489, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            TerracottaSub = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(160, 4, 10, TimeSpan.FromMilliseconds(500)),
                [MirAnimation.Walking] = new Frame(240, 6, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat1] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(480, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Show] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(560, 13, 20, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Dead] = new Frame(572, 1, 20, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.Hide] = new Frame(0, 13, 20, TimeSpan.FromMilliseconds(100)) { Reversed = true, },
            };

            TerracottaBoss = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Combat1] = new Frame(240, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(160, 9, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(320, 3, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(400, 11, 20, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.Dead] = new Frame(411, 1, 20, TimeSpan.FromMilliseconds(1000)),
            };

            Practitioner = new Dictionary<MirAnimation, Frame>
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(80, 4, 10, TimeSpan.FromMilliseconds(200)),
            };

            SabukPrimeGate = new Dictionary<MirAnimation, Frame>     //沙巴克主门
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.OpenDoor] = new Frame(640, 7, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.CloseDoor] = new Frame(640, 7, 10, TimeSpan.FromMilliseconds(1000)) { Reversed = true, },
            };

            SabukLeftDoor = new Dictionary<MirAnimation, Frame>        //沙巴克左门
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.OpenDoor] = new Frame(640, 7, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.CloseDoor] = new Frame(640, 7, 10, TimeSpan.FromMilliseconds(1000)) { Reversed = true, },
            };

            SabukRightDoor = new Dictionary<MirAnimation, Frame>      //沙巴克右门
            {
                [MirAnimation.Standing] = new Frame(0, 1, 10, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 8, 10, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.Dead] = new Frame(327, 1, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.OpenDoor] = new Frame(640, 7, 10, TimeSpan.FromMilliseconds(1000)),
                [MirAnimation.CloseDoor] = new Frame(640, 7, 10, TimeSpan.FromMilliseconds(1000)) { Reversed = true, },
            };

            Catapult = new Dictionary<MirAnimation, Frame>  //投石车
            {
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            Ballista = new Dictionary<MirAnimation, Frame>  //弩车
            {
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),
            };

            BrownHorse = new Dictionary<MirAnimation, Frame>  //棕马
            {
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };
            WhiteHorse = new Dictionary<MirAnimation, Frame>  //白马
            {
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            BlackHorse = new Dictionary<MirAnimation, Frame>  //黑马
            {
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            RedHorse = new Dictionary<MirAnimation, Frame>  //红马
            {
                [MirAnimation.Struck] = new Frame(240, 3, 10, TimeSpan.FromMilliseconds(100)),
            };

            MasterNorma = new Dictionary<MirAnimation, Frame>  //诺玛教主
            {
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),           //攻击
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),             //死亡过程
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),            //死亡地面尸体效果
            };

            SabakGuardian = new Dictionary<MirAnimation, Frame>  //沙城守护者
            {
                [MirAnimation.Standing] = new Frame(0, 7, 10, TimeSpan.FromMilliseconds(500)),                   //站立
                [MirAnimation.Walking] = new Frame(80, 8, 10, TimeSpan.FromMilliseconds(100)),                   //行走
                [MirAnimation.Combat1] = new Frame(160, 10, 10, TimeSpan.FromMilliseconds(100)),                  //攻击
                [MirAnimation.Combat2] = new Frame(400, 7, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Combat3] = new Frame(480, 8, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Struck] = new Frame(240, 4, 10, TimeSpan.FromMilliseconds(100)),                  //被攻击
                [MirAnimation.Die] = new Frame(320, 7, 10, TimeSpan.FromMilliseconds(100)),                    //死亡效果
                [MirAnimation.Dead] = new Frame(326, 1, 10, TimeSpan.FromMilliseconds(1000)),                   //死亡地面
            };

            Fence = new Dictionary<MirAnimation, Frame>     //栏栅
            {
                [MirAnimation.Standing] = new Frame(0, 4, 10, TimeSpan.FromMilliseconds(200)),
                [MirAnimation.Struck] = new Frame(240, 2, 10, TimeSpan.FromMilliseconds(100)),
                [MirAnimation.Die] = new Frame(320, 10, 10, TimeSpan.FromMilliseconds(120)),
                [MirAnimation.Dead] = new Frame(329, 1, 10, TimeSpan.FromMilliseconds(1000)),
            };

            NormaPaladin = new Dictionary<MirAnimation, Frame>  //诺玛圣骑士
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),                  //攻击
            };

            NomaCommander = new Dictionary<MirAnimation, Frame>  //诺玛大司令
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            NomaAxeWarriorKing = new Dictionary<MirAnimation, Frame>  //诺玛斧兵王
            {
                [MirAnimation.Combat2] = new Frame(480, 6, 10, TimeSpan.FromMilliseconds(100)),
            };

            NormaArmorKing = new Dictionary<MirAnimation, Frame>  //诺玛装甲王
            {
                [MirAnimation.Combat2] = new Frame(400, 6, 10, TimeSpan.FromMilliseconds(100)),                  //攻击
            };

            CommanderNoma = new Dictionary<MirAnimation, Frame>  //诺玛统领
            {
                [MirAnimation.Combat1] = new Frame(160, 8, 10, TimeSpan.FromMilliseconds(100)),                  //攻击
                [MirAnimation.Combat2] = new Frame(400, 8, 10, TimeSpan.FromMilliseconds(100)),

                [MirAnimation.Die] = new Frame(320, 9, 10, TimeSpan.FromMilliseconds(100)),                    //死亡效果
                [MirAnimation.Dead] = new Frame(328, 1, 10, TimeSpan.FromMilliseconds(1000)),                   //死亡地面
            };

            PharaohNorma = new Dictionary<MirAnimation, Frame>  //诺玛大法老
            {
                [MirAnimation.Combat2] = new Frame(480, 7, 10, TimeSpan.FromMilliseconds(100)),                  //攻击
            };
        }
    }

    /// <summary>
    /// 绘制帧
    /// </summary>
    public sealed class Frame
    {
        public static Frame EmptyFrame = new Frame(0, 0, 0, TimeSpan.Zero);

        public int StartIndex;
        public int FrameCount;
        public int OffSet;

        /// <summary>
        /// 是否反转播放
        /// </summary>
        public bool Reversed;
        /// <summary>
        /// 是否静态速度播放
        /// </summary>
        public bool StaticSpeed;
        /// <summary>
        /// 延迟时间
        /// </summary>
        public TimeSpan[] Delays;

        public Frame(int startIndex, int frameCount, int offSet, TimeSpan frameDelay, int repeatTimes = 1)
        {
            StartIndex = startIndex;
            FrameCount = frameCount;
            OffSet = offSet;

            Delays = new TimeSpan[FrameCount];
            for (int i = 0; i < Delays.Length; i++)
                Delays[i] = frameDelay;
        }

        public Frame(Frame frame)
        {
            StartIndex = frame.StartIndex;
            FrameCount = frame.FrameCount;
            OffSet = frame.OffSet;

            Delays = new TimeSpan[FrameCount];
            for (int i = 0; i < Delays.Length; i++)
                Delays[i] = frame.Delays[i];
        }

        public int GetFrame(DateTime start, DateTime now, bool doubleSpeed)
        {
            TimeSpan enlapsed = now - start;

            if (doubleSpeed && !StaticSpeed)
                enlapsed += enlapsed;

            if (Reversed)
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    enlapsed -= Delays[Delays.Length - 1 - i];
                    if (enlapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }
            else
            {
                for (int i = 0; i < Delays.Length; i++)
                {
                    enlapsed -= Delays[i];
                    if (enlapsed >= TimeSpan.Zero) continue;

                    return i;
                }
            }
            return FrameCount;
        }
    }
}
