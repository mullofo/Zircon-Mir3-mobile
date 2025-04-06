using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Scenes;
using Client.Scenes.Configs;
using Client.Scenes.Views;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using Frame = Library.Frame;
using Point = System.Drawing.Point;

namespace Client.Models
{
    /// <summary>
    /// 地图对象
    /// </summary>
    public abstract class MapObject
    {
        /// <summary>
        /// 名字标签
        /// </summary>
        public static SortedDictionary<string, List<DXLabel>> NameLabels = new SortedDictionary<string, List<DXLabel>>();
        /// <summary>
        /// 聊天标签
        /// </summary>
        public static List<DXLabel> ChatLabels = new List<DXLabel>();
        /// <summary>
        /// 角色用户
        /// </summary>
        public static UserObject User => GameScene.Game.User;
        /// <summary>
        /// 鼠标对象
        /// </summary>
        public static MapObject MouseObject
        {
            get { return GameScene.Game.MouseObject; }
            set
            {
                if (GameScene.Game.MouseObject == value) return;

                GameScene.Game.MouseObject = value;
                //GameScene.Game.HuntingLogPanel.MouseMonsterIndex = value?.MonsterIndex ?? 0;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }
        /// <summary>
        /// 目标对象
        /// </summary>
        public static MapObject TargetObject
        {
            get { return GameScene.Game.TargetObject; }
            set
            {
                if (GameScene.Game.TargetObject == value) return;

                GameScene.Game.TargetObject = value;
                //if (value != null) GameScene.Game.OldTargetObjectID = value.ObjectID;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }
        /// <summary>
        /// 魔法技能对象
        /// </summary>
        public static MapObject MagicObject
        {
            get { return GameScene.Game.MagicObject; }
            set
            {
                if (GameScene.Game.MagicObject == value) return;

                GameScene.Game.MagicObject = value;
                //if (value != null) GameScene.Game.OldTargetObjectID = value.ObjectID;
                GameScene.Game.MapControl.TextureValid = false;
            }
        }

        /// <summary>
        /// 阴影纹理
        /// </summary>
        public static Texture ShadowTexture;

        public abstract ObjectType Race { get; }
        public virtual bool Blocking => Visible && !Dead;
        public bool Visible = true;
        /// <summary>
        /// 对象ID
        /// </summary>
        public uint ObjectID;

        /// <summary>
        /// 等级
        /// </summary>
        public virtual int Level { get; set; }
        /// <summary>
        /// 当前HP
        /// </summary>
        public virtual int CurrentHP { get; set; }
        /// <summary>
        /// 当前MP
        /// </summary>
        public virtual int CurrentMP { get; set; }

        /// <summary>
        /// 攻击者ID
        /// </summary>
        public uint AttackerID;
        /// <summary>
        /// 魔法技能类型
        /// </summary>
        public MagicType MagicType;
        /// <summary>
        /// 攻击元素
        /// </summary>
        public Element AttackElement;
        /// <summary>
        /// 攻击目标
        /// </summary>
        public List<MapObject> AttackTargets;
        /// <summary>
        /// 魔法技能坐标
        /// </summary>
        public List<Point> MagicLocations;
        /// <summary>
        /// 施放魔法技能
        /// </summary>
        public bool MagicCast;
        /// <summary>
        /// 性别
        /// </summary>
        public MirGender Gender;
        /// <summary>
        /// 挖矿效果
        /// </summary>
        public bool MiningEffect;
        /// <summary>
        /// 当前位置
        /// </summary>
        public Point CurrentLocation
        {
            get { return _CurrentLocation; }
            set
            {
                if (_CurrentLocation == value) return;

                _CurrentLocation = value;

                LocationChanged();
            }
        }
        private Point _CurrentLocation;
        /// <summary>
        /// 当前单元格
        /// </summary>
        public Cell CurrentCell;
        /// <summary>
        /// 方向
        /// </summary>
        public MirDirection Direction;
        /// <summary>
        /// 名字标签可见
        /// </summary>
        public virtual bool NameLableVisable
        {
            get { return _NameLableVisable; }
            set
            {
                if (_NameLableVisable == value) return;
                _NameLableVisable = value;
                NameLableVisableChanged();
            }
        }
        private bool _NameLableVisable = true;
        /// <summary>
        /// 名字
        /// </summary>
        public virtual string Name
        {
            get { return _Name; }
            set
            {
                if (_Name == value) return;

                _Name = value;

                NameChanged();
            }
        }
        private string _Name;
        /// <summary>
        /// 标题
        /// </summary>
        public virtual string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value) return;

                _Title = value;

                NameChanged();
            }
        }
        private string _Title;
        /// <summary>
        /// 宠物主人
        /// </summary>
        public string PetOwner
        {
            get { return _PetOwner; }
            set
            {
                if (_PetOwner == value) return;

                _PetOwner = value;

                NameChanged();
            }
        }
        private string _PetOwner;
        /// <summary>
        /// 名字颜色
        /// </summary>
        public Color NameColour
        {
            get
            {
                if (Race != ObjectType.Player) return _NameColour;  //如果对象不是玩家 赋值对应的名字颜色

                foreach (CastleInfo castle in GameScene.Game.ConquestWars)   //攻城时的颜色设置 遍历行会攻城站
                {
                    if (castle.Map != GameScene.Game.MapControl.MapInfo) continue;   //如果攻城站地图信息不对 继续

                    if (GameScene.Game.CastleOwners.TryGetValue(castle, out string ownerGuild))
                    {
                        if (ownerGuild == Title)
                        {
                            //todo 如果是守城方 颜色为 深蓝色 Color.FromArgb(25, 25, 200)
                            return Color.FromArgb(25, 110, 200);  //守城方联盟浅蓝色
                        }
                        else
                        {
                            //todo 如果是攻城方 颜色为 红色 Red 
                            return Color.OrangeRed;  //攻城方联盟 桔红色
                        }
                    }
                }

                if (GameScene.Game.GuildAlliances.Contains(Title)) return Color.FromArgb(25, 70, 25);   //联盟行会的颜色

                if (GameScene.Game.GuildWars.Count == 0) return _NameColour;

                if (User.Title == Title)
                {
                    if (GameScene.Game.MapControl.MapInfo.CanPlayName == true)   //PK场
                    {
                        return _NameColour;
                    }
                    else
                    {
                        return Color.DarkCyan;   //普通玩家为深青色
                    }
                }

                if (GameScene.Game.GuildWars.Contains(Title))  //行会战
                {
                    if (GameScene.Game.MapControl.MapInfo.CanPlayName == true)  //PK场
                    {
                        return _NameColour;
                    }
                    else
                    {
                        return Color.OrangeRed;  //桔红色
                    }
                }

                return _NameColour;
            }
            set
            {
                if (_NameColour == value) return;

                _NameColour = value;

                NameChanged();
            }
        }
        private Color _NameColour;
        /// <summary>
        /// 聊天时间
        /// </summary>
        public DateTime ChatTime;
        /// <summary>
        /// 名字标签
        /// </summary>
        public DXLabel NameLabel;
        /// <summary>
        /// 聊天标签
        /// </summary>
        public DXLabel ChatLabel;
        /// <summary>
        /// 称号标签
        /// </summary>
        public DXLabel TitleNameLabel;
        /// <summary>
        /// 成就称号标签
        /// </summary>
        public DXLabel AchievementTitleLabel;
        /// <summary>
        /// 受伤信息列表
        /// </summary>
        public List<DamageInfo> DamageList = new List<DamageInfo>();
        /// <summary>
        /// 实现效果
        /// </summary>
        public List<MirEffect> Effects = new List<MirEffect>();
        /// <summary>
        /// 狂涛泉涌效果
        /// </summary>
        public List<MirEffect> DragonRepulseEffects = new List<MirEffect>();
        /// <summary>
        /// 显示BUFF类型
        /// </summary>
        public List<BuffType> VisibleBuffs = new List<BuffType>();
        /// <summary>
        /// 显示自定义BUFF信息
        /// </summary>
        public List<CustomBuffInfo> VisibleCustomBuffs = new List<CustomBuffInfo>();
        /// <summary>
        /// 毒类型
        /// </summary>
        public PoisonType Poison;
        /// <summary>
        /// 移动距离
        /// </summary>
        public int MoveDistance;
        /// <summary>
        /// 绘制骨架
        /// </summary>
        public int DrawFrame
        {
            get { return _DrawFrame; }
            set
            {
                if (_DrawFrame == value) return;

                _DrawFrame = value;
                DrawFrameChanged();
            }
        }
        private int _DrawFrame;
        /// <summary>
        /// 骨架索引
        /// </summary>
        public int FrameIndex
        {
            get { return _FrameIndex; }
            set
            {
                if (_FrameIndex == value) return;

                _FrameIndex = value;
                FrameIndexChanged();
            }
        }
        private int _FrameIndex;
        /// <summary>
        /// 绘制X坐标
        /// </summary>
        public int DrawX;
        /// <summary>
        /// 绘制Y坐标
        /// </summary>
        public int DrawY;

        public DateTime DrawHealthTime, StanceTime;
        /// <summary>
        /// 钓鱼时间
        /// </summary>
        public DateTime FishingTime;

        private Point _MovingOffSet;
        /// <summary>
        /// 坐标移动偏移
        /// </summary>
        public Point MovingOffSet
        {
            get
            {
                return _MovingOffSet;
            }
            set
            {
                if (_MovingOffSet == value) return;

                _MovingOffSet = value;
                MovingOffSetChanged();
            }
        }
        /// <summary>
        /// 光效
        /// </summary>
        public int Light;
        /// <summary>
        /// 透明度
        /// </summary>
        public float Opacity = 1F;
        /// <summary>
        /// 光效颜色
        /// </summary>
        public Color LightColour = Globals.NoneColour;

        public MirEffect MagicShieldEffect, WraithGripEffect, WraithGripEffect2, AssaultEffect, CelestialLightEffect, LifeStealEffect, SilenceEffect, StunnedStrikeEffect, BlindEffect, AbyssEffect, DragonRepulseEffect, DragonRepulseEffect1,
                         RankingEffect, DeveloperEffect, FrostBiteEffect, InfectionEffect, ReigningStepEffect, NeutralizeEffect, SuperiorMagicShieldEffect, HeadTopEffect,
                         ElectricShockEffect, BurnEffect;

        /// <summary>
        /// 显示亡灵束缚
        /// </summary>
        public bool CanShowWraithGrip = true;

        /// <summary>
        /// 显示千刃杀风
        /// </summary>
        public bool CanShowThousandBlades = true;
        public MirEffect ThousandBladesEffect;

        /// <summary>
        /// 骨架尸体
        /// </summary>
        public bool Skeleton;
        /// <summary>
        /// 死亡
        /// </summary>
        public bool Dead
        {
            get { return _Dead; }
            set
            {
                if (_Dead == value) return;

                _Dead = value;

                DeadChanged();
            }
        }
        private bool _Dead;

        #region 成就称号显示
        /// <summary>
        /// 成就称号
        /// </summary>
        public string AchievementTitle
        {
            get { return _AchievementTitle; }
            set
            {
                if (_AchievementTitle == value)
                {
                    return;
                }

                if (GameScene.Game.QuestBox != null)
                {
                    GameScene.Game.QuestBox.TitleTab.ChangeTitle(value, false);
                }

                _AchievementTitle = value;

                NameChanged();
            }
        }
        private string _AchievementTitle;

        /// <summary>
        /// 成就称号颜色
        /// </summary>
        public Color AchievementTitleColour
        {
            get
            {
                AchievementInfo achievement = Globals.AchievementInfoList.Binding.FirstOrDefault(x => x.Title == AchievementTitle);
                if (achievement != null)
                {
                    switch (achievement.Grade)
                    {
                        case 0:
                            return Color.White;
                        case 1:
                            return Color.MediumBlue;
                        case 2:
                            return Color.Red;
                    }
                }

                return _AchievementTitleColour;
            }
            set
            {
                if (_AchievementTitleColour == value) return;

                _AchievementTitleColour = value;

                NameChanged();
            }
        }
        private Color _AchievementTitleColour;
        #endregion

        /// <summary>
        /// 属性状态
        /// </summary>
        public virtual Stats Stats { get; set; }
        /// <summary>
        /// 中断
        /// </summary>
        public bool Interupt;
        /// <summary>
        /// 当前动作
        /// </summary>
        public MirAction CurrentAction;
        /// <summary>
        /// 当前动画
        /// </summary>
        public MirAnimation CurrentAnimation;
        /// <summary>
        /// 当前帧
        /// </summary>
        public Frame CurrentFrame;
        /// <summary>
        /// 帧启动时间
        /// </summary>
        public DateTime FrameStart;
        /// <summary>
        /// 框架
        /// </summary>
        public Dictionary<MirAnimation, Frame> Frames;
        /// <summary>
        /// 绘制颜色
        /// </summary>
        public Color DrawColour;
        /// <summary>
        /// 默认颜色 白色
        /// </summary>
        public Color DefaultColour = Color.White;
        /// <summary>
        /// 最大护身法盾
        /// </summary>
        public int MaximumSuperiorMagicShield;
        /// <summary>
        /// 渲染Y
        /// </summary>
        public virtual int RenderY
        {
            get
            {
                if (MovingOffSet.IsEmpty)
                    return CurrentLocation.Y;

                switch (Direction)
                {
                    case MirDirection.Up:
                    case MirDirection.UpRight:
                    case MirDirection.UpLeft:
                        return CurrentLocation.Y + MoveDistance;
                    default:
                        return CurrentLocation.Y;
                }
            }
        }
        /// <summary>
        /// 单元格宽度
        /// </summary>
        public static int CellWidth => MapControl.CellWidth;
        /// <summary>
        /// 单元格高度
        /// </summary>
        public static int CellHeight => MapControl.CellHeight;
        /// <summary>
        /// X偏移量
        /// </summary>
        public static int OffSetX => MapControl.OffSetX;
        /// <summary>
        /// Y偏移量
        /// </summary>
        public static int OffSetY => MapControl.OffSetY;
        /// <summary>
        /// 操作队列
        /// </summary>
        public List<ObjectAction> ActionQueue = new List<ObjectAction>();

        /// <summary>
        /// 过程
        /// </summary>
        public virtual void Process()
        {
            DamageInfo previous = null;
            for (int index = 0; index < DamageList.Count; index++)
            {
                DamageInfo damageInfo = DamageList[index];
                if (DamageList.Count - index > 3 && CEnvir.Now - damageInfo.StartTime > damageInfo.AppearDelay && CEnvir.Now - damageInfo.StartTime < damageInfo.AppearDelay + damageInfo.ShowDelay)
                    damageInfo.StartTime = CEnvir.Now - damageInfo.AppearDelay - damageInfo.ShowDelay;


                damageInfo.Process(previous);

                previous = damageInfo;
            }

            for (int i = DamageList.Count - 1; i >= 0; i--)
            {
                if (!DamageList[i].Visible)
                    DamageList.RemoveAt(i);
            }
            UpdateFrame();
            if (Config.SmoothRendering)
                SmoothUpdateFrame();

            DrawX = CurrentLocation.X - User.CurrentLocation.X + MapControl.OffSetX;
            DrawY = CurrentLocation.Y - User.CurrentLocation.Y + MapControl.OffSetY;

            DrawX *= MapControl.CellWidth;
            DrawY *= MapControl.CellHeight;


            if (this != User)
            {
                DrawX += MovingOffSet.X - User.MovingOffSet.X - User.ShakeScreenOffset.X;
                DrawY += MovingOffSet.Y - User.MovingOffSet.Y - User.ShakeScreenOffset.Y;
            }

            DrawColour = DefaultColour;

            if (TargetObject != null && this != User && BigPatchConfig.ChkColourTarget)   //如果目标不是空  不是自己 开启攻击变色
            {
                TargetObject.DrawColour = Color.FromArgb(153, 153, 204);  //绘制锁定颜色
            }

            if ((Poison & PoisonType.Red) == PoisonType.Red)    //绘制红毒
                DrawColour = Color.IndianRed;  //印度红

            if ((Poison & PoisonType.Green) == PoisonType.Green)   //绘制绿毒
                DrawColour = Color.SeaGreen;   //海绿

            if ((Poison & PoisonType.Slow) == PoisonType.Slow)    //绘制减速
                DrawColour = Color.CornflowerBlue;   //浅蓝

            if ((Poison & PoisonType.Paralysis) == PoisonType.Paralysis)   //绘制麻痹
                DrawColour = Color.DimGray;   //暗灰

            if ((Poison & PoisonType.ThousandBlades) == PoisonType.ThousandBlades)
            {
                if (CanShowThousandBlades && ThousandBladesEffect == null)
                    ThousandBladesCreate();
            }
            else
            {
                if (ThousandBladesEffect != null)
                    ThousandBladesEnd();
            }

            if ((Poison & PoisonType.WraithGrip) == PoisonType.WraithGrip)   //绘制石化
            {
                if (CanShowWraithGrip && WraithGripEffect == null)
                    WraithGripCreate();
            }
            else
            {
                if (WraithGripEffect != null)
                    WraithGripEnd();
            }

            if ((Poison & PoisonType.Silenced) == PoisonType.Silenced)   //绘制沉默
            {
                if (SilenceEffect == null)
                    SilenceCreate();
            }
            else
            {
                if (SilenceEffect != null)
                    SilenceEnd();
            }

            if ((Poison & PoisonType.StunnedStrike) == PoisonType.StunnedStrike)  //绘制晕击
            {
                if (StunnedStrikeEffect == null)
                    StunnedStrikeCreate();
            }
            else
            {
                if (StunnedStrikeEffect != null)
                    StunnedStrikeEnd();
            }

            if ((Poison & PoisonType.Abyss) == PoisonType.Abyss)   //绘制深渊
            {
                if (BlindEffect == null)
                    BlindCreate();
            }
            else
            {
                if (BlindEffect != null)
                    BlindEnd();
            }

            if ((Poison & PoisonType.Infection) == PoisonType.Infection)   //绘制传染
            {
                if (InfectionEffect == null)
                    InfectionCreate();
            }
            else
            {
                if (InfectionEffect != null)
                    InfectionEnd();
            }

            if ((Poison & PoisonType.Neutralize) == PoisonType.Neutralize)
            {
                if (NeutralizeEffect == null)
                    NeutralizeCreate();
            }
            else
            {
                if (NeutralizeEffect != null)
                    NeutralizeEnd();
            }

            if ((Poison & PoisonType.ElectricShock) == PoisonType.ElectricShock)
            {
                if (ElectricShockEffect == null)
                    ElectricShockCreate();
            }
            else
            {
                if (ElectricShockEffect != null)
                    ElectricShockEnd();
            }

            if ((Poison & PoisonType.HellFire) == PoisonType.HellFire)
            {
                if (BurnEffect == null)
                    BurnCreate();
            }
            else
            {
                if (BurnEffect != null)
                    BurnEnd();
            }

            if (VisibleBuffs.Contains(BuffType.SuperTransparency))
            {
                Opacity = 0.0f;
            }
            else
            {
                if (Stats?[Stat.ClearRing] > 0 || VisibleBuffs.Contains(BuffType.Invisibility) || VisibleBuffs.Contains(BuffType.Cloak) || VisibleBuffs.Contains(BuffType.Transparency))  //绘制隐身
                    Opacity = 0.5f;
                else
                    Opacity = 1f;
            }

            if (VisibleBuffs.Contains(BuffType.MagicShield) && (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User)))  //绘制魔法盾 
            {
                if (MagicShieldEffect == null)
                    MagicShieldCreate();
            }
            else
            {
                if (MagicShieldEffect != null)
                    MagicShieldEnd();
            }

            if (VisibleBuffs.Contains(BuffType.SuperiorMagicShield))  //绘制护身法盾
            {
                if (SuperiorMagicShieldEffect == null)
                    SuperiorMagicShieldCreate();
            }
            else
            {
                if (SuperiorMagicShieldEffect != null)
                    SuperiorMagicShieldEnd();
            }

            if (VisibleBuffs.Contains(BuffType.Developer))   //绘制管理员
            {
                if (RankingEffect != null)
                    RankingEnd();

                if (DeveloperEffect == null)
                    DeveloperCreate();
            }
            else if (HeadTopEffect == null)
            {
                if (DeveloperEffect != null)
                    DeveloperEnd();

                //if (VisibleBuffs.Contains(BuffType.Ranking))  //绘制排行榜
                //{
                //    if (RankingEffect == null)
                //        RankingCreate();
                //}
                //else if (RankingEffect != null)
                //    RankingEnd();
            }

            if (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User))
            {
                foreach (CustomBuffInfo customBuff in VisibleCustomBuffs)
                {
                    if (customBuff != null)
                    {
                        if (customBuff.OverheadTitle != 0)
                        {
                            if (HeadTopEffect == null)
                                HeadTopCreate(customBuff.OverheadTitle * 20, customBuff.frameCount, customBuff.offSetX, customBuff.offSetY);
                        }
                        //else if (HeadTopEffect != null)
                        //    HeadTopEnd();
                    }
                }
            }

            if (VisibleBuffs.Contains(BuffType.LifeSteal))   //绘制吸星大法
            {
                if (LifeStealEffect == null)
                    LifeStealCreate();
            }
            else if (LifeStealEffect != null)
                LifeStealEnd();

            if (VisibleBuffs.Contains(BuffType.CelestialLight) && (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User)))  //绘制阴阳法环
            {
                if (CelestialLightEffect == null)
                    CelestialLightCreate();
            }
            else if (CelestialLightEffect != null)
                CelestialLightEnd();

            if (VisibleBuffs.Contains(BuffType.FrostBite))   //绘制护身冰环
            {
                if (FrostBiteEffect == null)
                    FrostBiteCreate();
            }
            else if (FrostBiteEffect != null)
                FrostBiteEnd();

        }
        /// <summary>
        /// 更新平滑帧
        /// </summary>
        private void SmoothUpdateFrame()
        {
            int x = 0;
            int y = 0;
            switch (CurrentAction)
            {
                case MirAction.Moving:
                    //case MirAction.Pushed:
                    double fram = 1.0 - ((CEnvir.Now - FrameStart).Ticks / 10000L) / 600.0;
                    if (fram < 0.0)
                    {
                        fram = 0.0;
                    }
                    switch (Direction)
                    {
                        case MirDirection.Up:
                            x = 0;
                            y = (int)((CellHeight * MoveDistance) * fram);
                            break;
                        case MirDirection.UpRight:
                            x = -(int)((CellWidth * MoveDistance) * fram);
                            y = (int)((CellHeight * MoveDistance) * fram);
                            break;
                        case MirDirection.Right:
                            x = -(int)((CellWidth * MoveDistance) * fram);
                            y = 0;
                            break;
                        case MirDirection.DownRight:
                            x = -(int)((CellWidth * MoveDistance) * fram);
                            y = -(int)((CellHeight * MoveDistance) * fram);
                            break;
                        case MirDirection.Down:
                            x = 0;
                            y = -(int)((CellHeight * MoveDistance) * fram);
                            break;
                        case MirDirection.DownLeft:
                            x = (int)((CellWidth * MoveDistance) * fram);
                            y = -(int)((CellHeight * MoveDistance) * fram);
                            break;
                        case MirDirection.Left:
                            x = (int)((CellWidth * MoveDistance) * fram);
                            y = 0;
                            break;
                        case MirDirection.UpLeft:
                            x = (int)((CellWidth * MoveDistance) * fram);
                            y = (int)((CellHeight * MoveDistance) * fram);
                            break;
                    }

                    MovingOffSet = new Point(x, y);
                    break;
            }
        }

        /// <summary>
        /// 更新帧
        /// </summary>
        public virtual void UpdateFrame()
        {
            if (Frames == null || CurrentFrame == null) return;

            switch (CurrentAction)
            {
                case MirAction.Moving:
                case MirAction.Pushed:
                    if (!GameScene.Game.MoveFrame) return;
                    Visible = true;
                    break;
                case MirAction.Attack:
                    Visible = true;
                    break;
                case MirAction.Dead:
                    if (Visible && CEnvir.ClientControl.OnClearBodyCheck && BigPatchConfig.ChkCleanCorpse)   //清理尸体
                        Visible = false;
                    else if (!Visible && !CEnvir.ClientControl.OnClearBodyCheck && !BigPatchConfig.ChkCleanCorpse)
                        Visible = true;
                    break;
            }

            //拿到当前时间对应的那1帧
            int frame = CurrentFrame.GetFrame(FrameStart, CEnvir.Now, (this != User || GameScene.Game.Observer) && ActionQueue.Count > 1);

            if (frame == CurrentFrame.FrameCount || (Interupt && ActionQueue.Count > 0))
            {
                DoNextAction();
                frame = CurrentFrame.GetFrame(FrameStart, CEnvir.Now, (this != User || GameScene.Game.Observer) && ActionQueue.Count > 1);
                if (frame == CurrentFrame.FrameCount)
                    frame -= 1;
            }

            int x = 0, y = 0;
            switch (CurrentAction)
            {
                case MirAction.Moving:
                case MirAction.Pushed:
                    switch (Direction)
                    {
                        case MirDirection.Up:
                            x = 0;
                            y = (int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.UpRight:
                            x = -(int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = (int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.Right:
                            x = -(int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = 0;
                            break;
                        case MirDirection.DownRight:
                            x = -(int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = -(int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.Down:
                            x = 0;
                            y = -(int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.DownLeft:
                            x = (int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = -(int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                        case MirDirection.Left:
                            x = (int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = 0;
                            break;
                        case MirDirection.UpLeft:
                            x = (int)(CellWidth * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            y = (int)(CellHeight * MoveDistance / (float)CurrentFrame.FrameCount * (CurrentFrame.FrameCount - (frame + 1)));
                            break;
                    }
                    break;
            }

            x -= x % 2;
            y -= y % 2;

            if (CurrentFrame.Reversed)
            {
                frame = CurrentFrame.FrameCount - frame - 1;
                x *= -1;
                y *= -1;
            }

            MovingOffSet = new Point(x, y);

            if (Race == ObjectType.Player && CurrentAction == MirAction.Pushed)
                frame = 0;

            FrameIndex = frame;

            DrawFrame = FrameIndex + CurrentFrame.StartIndex + CurrentFrame.OffSet * (int)Direction;

        }
        /// <summary>
        /// 设置动画
        /// </summary>
        /// <param name="action"></param>
        public abstract void SetAnimation(ObjectAction action);
        /// <summary>
        /// 设置框架
        /// </summary>
        /// <param name="action"></param>
        public virtual void SetFrame(ObjectAction action)
        {
            SetAnimation(action);

            FrameIndex = -1;
            CurrentAction = action.Action;
            FrameStart = CEnvir.Now;

            switch (action.Action)
            {
                case MirAction.Standing:
                case MirAction.Dead:
                    Interupt = true;
                    break;
                default:
                    Interupt = false;
                    break;
            }
        }
        /// <summary>
        /// 设置各类动作动画
        /// </summary>
        /// <param name="action"></param>
        public virtual void SetAction(ObjectAction action)
        {
            MirEffect spell;   //特效定义赋值 spell

            switch (CurrentAction)   //表达式（当前动作）
            {
                case MirAction.Mining:
                    if (!MiningEffect) break;       //如果不开启挖矿效果 退出循环

                    Effects.Add(new MirEffect(3470, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 70, Globals.NoneColour)
                    {
                        Blend = true,
                        MapTarget = CurrentLocation,
                        Direction = Direction,
                        Skip = 10,
                    });
                    break;
                //怪物自定义
                case MirAction.DiySpell://原有技能释放初
                case MirAction.Spell:
                    if (!MagicCast) break;  //如果不是释放技能 退出循环

                    Color attackColour = Globals.NoneColour;
                    switch (AttackElement)  //攻击时元素效果
                    {
                        case Element.None:
                            attackColour = Globals.NoneColour;
                            break;
                        case Element.Fire:
                            attackColour = Globals.FireColour;
                            break;
                        case Element.Ice:
                            attackColour = Globals.IceColour;
                            break;
                        case Element.Lightning:
                            attackColour = Globals.LightningColour;
                            break;
                        case Element.Wind:
                            attackColour = Globals.WindColour;
                            break;
                        case Element.Holy:
                            attackColour = Globals.HolyColour;
                            break;
                        case Element.Dark:
                            attackColour = Globals.DarkColour;
                            break;
                        case Element.Phantom:
                            attackColour = Globals.PhantomColour;
                            break;
                    }

                    switch (MagicType)   //表达式（技能类型） 释放到怪物身上的效果
                    {
                        #region 战士

                        //Swordsmanship

                        //Potion Mastery 

                        //Slaying

                        //Thrusting

                        //Half Moon

                        //Shoulder Dash

                        //Flaming Sword

                        //Dragon Rise

                        //Blade Storm

                        //Destructive Surge

                        //Interchange

                        //Defiance

                        //Beckon

                        //Might

                        #region 快刀斩马

                        case MagicType.SwiftBlade:

                            foreach (Point point in MagicLocations)  //遍历数组（魔法坐标）
                            {
                                spell = new MirEffect(2330, 16, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    BlendRate = 0.3F,
                                    DrawColour = Color.Red,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.SwiftBladeEnd);
                            break;

                        #endregion

                        //Assault

                        //Endurance

                        //Reflect Damage

                        //Fetter

                        #endregion

                        #region 法师

                        #region 火球术

                        case MagicType.FireBall:
                            foreach (Point point in MagicLocations)  //遍历数组（魔法坐标）
                            {
                                Effects.Add(spell = new MirProjectile(420, 5, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.FireColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)  //遍历数组（攻击目标）
                            {
                                Effects.Add(spell = new MirProjectile(420, 5, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.FireColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(580, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.FireBallEnd);
                                };

                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)  //如果魔法坐标数大于0   攻击目标大于0
                                DXSoundManager.Play(SoundIndex.FireBallTravel);
                            break;

                        #endregion

                        #region 霹雳掌

                        case MagicType.LightningBall:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(3070, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.LightningColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(3070, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.LightningColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(3230, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.ThunderBoltEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.ThunderBoltTravel);

                            break;

                        #endregion

                        #region 冰月神掌

                        case MagicType.IceBolt:

                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(2700, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.IceColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(2700, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.IceColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(2860, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.IceColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.IceBoltEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.IceBoltTravel);

                            break;

                        #endregion

                        #region 风掌

                        case MagicType.GustBlast:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(430, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 35, 35, Globals.WindColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(430, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 35, 35, Globals.WindColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(590, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.GustBlastEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.GustBlastTravel);

                            break;

                        #endregion

                        #region 诱惑之光

                        case MagicType.ElectricShock:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(10, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(10, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.ElectricShockEnd);
                            break;

                        #endregion

                        //Teleportation

                        #region 大火球 & 焰天火雨

                        case MagicType.AdamantineFireBall:
                        case MagicType.MeteorShower:

                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(1640, 6, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 35, 35, Globals.FireColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(1640, 6, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 35, 35, Globals.FireColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(1800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.GreaterFireBallEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.GreaterFireBallTravel);
                            break;

                        #endregion

                        #region 雷电术 & 电闪雷鸣

                        case MagicType.ThunderBolt:
                        case MagicType.ThunderStrike:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(1450, 3, TimeSpan.FromMilliseconds(150), LibraryFile.Magic, 150, 50, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = point
                                };
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                spell = new MirEffect(1450, 3, TimeSpan.FromMilliseconds(150), LibraryFile.Magic, 150, 50, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = attackTarget
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.LightningStrikeEnd);
                            break;

                        #endregion

                        #region 冰月震天

                        case MagicType.IceBlades:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(2960, 6, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 35, 35, Globals.IceColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Skip = 0,
                                    BlendRate = 1F,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(2960, 6, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 35, 35, Globals.IceColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Skip = 0,
                                    BlendRate = 1F,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(2970, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.IceColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.GreaterIceBoltEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.GreaterIceBoltTravel);
                            break;

                        #endregion

                        #region 击风

                        case MagicType.Cyclone:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(1990, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.WindColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                };

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(2000, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.WindColour)
                                    {
                                        Blend = true,
                                        MapTarget = point
                                    };
                                    spell.Process();
                                };

                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                spell = new MirEffect(1990, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    CParticleType = MagicType,
                                };

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(2000, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.WindColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget
                                    };
                                    spell.Process();
                                };

                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.CycloneEnd);
                            break;

                        #endregion

                        #region 地狱火

                        case MagicType.ScortchedEarth:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    //烧焦的地面
                                    Effects.Add(new MirEffect(220, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
                                    {
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, CurrentLocation) * 50),
                                        Opacity = 0.8F,
                                        DrawType = DrawType.Floor,
                                    });
                                    //冒烟的动画
                                    Effects.Add(new MirEffect(2450 + CEnvir.Random.Next(5) * 10, 10, TimeSpan.FromMilliseconds(250), LibraryFile.Magic, 0, 0, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, CurrentLocation) * 50),
                                        DrawType = DrawType.Floor,
                                    });
                                    //地狱火动画
                                    Effects.Add(new MirEffect(1900, 30, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 20, 70, Globals.FireColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(Functions.Distance(point, CurrentLocation) * 50),
                                        BlendRate = 1F,
                                    });
                                }
                            break;

                        #endregion

                        #region 疾光电影

                        case MagicType.LightningBeam:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    Effects.Add(spell = new MirEffect(1180, 4, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 150, 150, Globals.LightningColour)
                                    {
                                        Blend = true,
                                        Target = this,
                                        Direction = Functions.DirectionFromPoint(CurrentLocation, point)
                                    });
                                    spell.Process();
                                }

                            DXSoundManager.Play(SoundIndex.LightningBeamEnd);
                            break;

                        #endregion

                        #region 冰沙掌

                        case MagicType.FrozenEarth:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    Effects.Add(spell = new MirEffect(90, 20, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 20, 70, Globals.IceColour)
                                    {
                                        //Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(Functions.Distance(point, CurrentLocation) * 150),
                                        Opacity = 0.8F,
                                        CParticleType = MagicType,
                                    });

                                    spell.CompleteAction = () =>
                                    {
                                        Effects.Add(spell = new MirEffect(260, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.IceColour)
                                        {
                                            MapTarget = point,
                                            Opacity = 0.8F,
                                            DrawType = DrawType.Floor,
                                        });
                                        spell.Process();
                                    };
                                    spell.Process();
                                }
                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.FrozenEarthEnd);
                            break;

                        #endregion

                        #region 风震天

                        case MagicType.BlowEarth:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    Effects.Add(spell = new MirProjectile(1990, 5, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.WindColour, CurrentLocation)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        Skip = 0,
                                        Explode = true,
                                        CParticleType = MagicType,
                                    });

                                    spell.CompleteAction = () =>
                                    {
                                        spell = new MirEffect(2000, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.WindColour)
                                        {
                                            Blend = true,
                                            MapTarget = point
                                        };
                                        spell.Process();
                                        DXSoundManager.Play(SoundIndex.BlowEarthEnd);
                                    };

                                    spell.Process();
                                }

                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.BlowEarthTravel);
                            break;

                        #endregion

                        //Fire Wall

                        #region 圣言术

                        case MagicType.ExpelUndead:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                spell = new MirEffect(140, 5, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 50, 80, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.ExpelUndeadEnd);
                            break;

                        #endregion

                        //Geo Manipulation

                        //Magic Shield

                        #region 爆裂火焰

                        case MagicType.FireStorm:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(950, 7, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.FireStormEnd);
                            break;

                        #endregion

                        #region 地狱雷光

                        case MagicType.LightningWave:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(980, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 50, 80, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.LightningWaveEnd);
                            break;

                        #endregion

                        #region 冰咆哮

                        case MagicType.IceStorm:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(780, 7, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.IceStormEnd);
                            break;

                        #endregion

                        #region 龙卷风

                        case MagicType.DragonTornado:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(1040, 16, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    CParticleType = MagicType,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.DragonTornadoEnd);
                            break;

                        #endregion

                        #region 魄冰刺

                        case MagicType.GreaterFrozenEarth:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirEffect(90, 20, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 20, 70, Globals.IceColour)
                                {
                                    //Blend = true,
                                    MapTarget = point,
                                    StartTime = CEnvir.Now.AddMilliseconds(Functions.Distance(point, CurrentLocation) * 150),
                                    Opacity = 0.8F,
                                    CParticleType = MagicType,
                                });
                                spell.CompleteAction = () =>
                                {
                                    Effects.Add(spell = new MirEffect(260, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
                                    {
                                        MapTarget = point,
                                        Opacity = 0.8F,
                                        DrawType = DrawType.Floor,
                                    });
                                    spell.Process();
                                };
                                spell.Process();
                            }
                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.GreaterFrozenEarthEnd);
                            break;

                        #endregion

                        #region 怒神霹雳

                        case MagicType.ChainLightning:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(470, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 50, 80, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.ChainLightningEnd);
                            break;

                        #endregion

                        #region 天之怒火

                        case MagicType.Asteroid:

                            foreach (Point point in MagicLocations)
                            {
                                MirProjectile eff;
                                Point p = new Point(point.X + 4, point.Y - 10);
                                Effects.Add(eff = new MirProjectile(1300, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 50, 80, Globals.FireColour, p)
                                {
                                    MapTarget = point,
                                    Skip = 0,
                                    Explode = true,
                                    Blend = true,
                                });

                                eff.CompleteAction = () =>
                                {
                                    Effects.Add(new MirEffect(1320, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 100, 100, Globals.NoneColour)
                                    {
                                        MapTarget = eff.MapTarget,
                                        Blend = true,
                                    });
                                };
                            }
                            break;

                        #endregion

                        //Meteor Shower -> Adam Fire Ball

                        //Renounce

                        //Tempest

                        //Judgement Of Heaven

                        //Thunder Strike -> Thunder Bolt

                        //MirrorImage

                        #endregion

                        #region Taoist  道士

                        #region 治愈术

                        case MagicType.Heal:

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(610, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.HealEnd);
                            break;

                        #endregion

                        //SpiritSword

                        #region 施毒术 & 施毒大法

                        case MagicType.PoisonDust:
                        case MagicType.GreaterPoisonDust:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(70, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.PoisonDustEnd);
                            break;

                        #endregion

                        #region 灵魂火符

                        case MagicType.ExplosiveTalisman:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.DarkColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.DarkColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(1140, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 50, Globals.DarkColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.ExplosiveTalismanEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.ExplosiveTalismanTravel);

                            break;

                        #endregion

                        #region 月魂断玉

                        case MagicType.EvilSlayer:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(3330, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.HolyColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Skip = 0,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(3330, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.HolyColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Skip = 0,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(3340, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 50, Globals.HolyColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.HolyStrikeEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.HolyStrikeTravel);

                            break;

                        #endregion

                        //SummonSkeleton

                        //Invisibility

                        #region 幽灵盾

                        case MagicType.MagicResistance:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.NoneColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Explode = true,
                                });

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(200, 8, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 80, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.MagicResistanceEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.MagicResistanceTravel);

                            break;

                        #endregion

                        #region 集体隐身术

                        case MagicType.MassInvisibility:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.PhantomColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Explode = true,
                                });

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(820, 7, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 80, Globals.PhantomColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.MassInvisibilityEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.MassInvisibilityTravel);
                            break;

                        #endregion

                        #region 月魂灵波

                        case MagicType.GreaterEvilSlayer:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(3440, 6, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 35, 35, Globals.HolyColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Skip = 0,
                                    CParticleType = MagicType,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(3440, 6, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 35, 35, Globals.HolyColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Skip = 0,
                                    CParticleType = MagicType,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(3450, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 50, Globals.HolyColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeTravel);

                            break;

                        #endregion

                        #region 神圣战甲术

                        case MagicType.Resilience:

                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.NoneColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Explode = true,
                                });

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(170, 8, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 80, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.ResilienceEnd);
                                };

                                spell.Process();
                            }

                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.ResilienceTravel);

                            break;

                        #endregion

                        #region 困魔咒

                        case MagicType.TrapOctagon:
                            DXSoundManager.Play(SoundIndex.ShacklingTalismanEnd);
                            break;

                        #endregion

                        //Taoist Combat Kick

                        #region 强震魔法

                        case MagicType.ElementalSuperiority:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.NoneColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Explode = true,
                                });

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(640, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 20, 80, attackColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        DrawColour = attackColour,
                                    };
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.BloodLustTravel);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.BloodLustEnd);

                            break;

                        #endregion

                        //Shinsu

                        #region 群体治愈术

                        case MagicType.MassHeal:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(670, 7, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 40, 60, Globals.HolyColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.MassHealEnd);

                            break;

                        #endregion

                        //Summon Jin Skeleton

                        #region 猛虎强势

                        case MagicType.BloodLust:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 35, 35, Globals.DarkColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Explode = true,
                                });

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(140, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 20, 80, Globals.DarkColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.BloodLustEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0)
                                DXSoundManager.Play(SoundIndex.BloodLustTravel);
                            break;

                        #endregion

                        #region 回生术

                        case MagicType.Resurrection:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(320, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 60, 60, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            break;

                        #endregion

                        #region 云寂术

                        case MagicType.Purification:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(230, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 40, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.PurificationEnd);
                            break;

                        #endregion

                        #region 移花接玉

                        case MagicType.StrengthOfFaith:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(370, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 40, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.StrengthOfFaithEnd);
                            break;

                        #endregion

                        //Transparency

                        #region 阴阳法环

                        case MagicType.CelestialLight:
                            if (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User))
                            {
                                foreach (MapObject attackTarget in AttackTargets)
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(290, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 40, Globals.HolyColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();
                                }
                            }
                            break;

                        #endregion

                        //Empowered Healing

                        #region 吸星大法

                        case MagicType.LifeSteal:

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(2500, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.HolyStrikeEnd);
                            break;

                        #endregion

                        #region 灭魂火符

                        case MagicType.ImprovedExplosiveTalisman:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 35, 35, Globals.DarkColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Has16Directions = false
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(980, 3, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 35, 35, Globals.DarkColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Has16Directions = false
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(1160, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 50, Globals.DarkColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.FireStormEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.ExplosiveTalismanTravel);

                            break;

                        #endregion

                        //Greater Poison Dust -> Poison Dust

                        //Summon Demon

                        //Scarecrow

                        //Demon Explosion

                        #region 横扫千军

                        case MagicType.ThunderKick:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(1190, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 40, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.FireStormEnd);
                            break;

                        #endregion

                        #region 传染

                        case MagicType.Infection:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(800, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 35, 35, Globals.NoneColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(800, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 35, 35, Globals.NoneColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });

                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.FireBallTravel);
                            break;

                        #endregion

                        #region 虚弱化

                        case MagicType.Neutralize:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(300, 4, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx7, 35, 35, Globals.FireColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(300, 4, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx7, 35, 35, Globals.FireColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(460, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx7, 0, 0, Globals.FireColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.NeutralizeEnd);
                                };
                                spell.Process();
                            }
                            break;

                        #endregion

                        #region 隐魂术

                        case MagicType.MassTransparency:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(700, 12, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 40, 60, Globals.HolyColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                                spell.Process();
                            }

                            DXSoundManager.Play(SoundIndex.MassInvisibilityTravel);
                            break;

                        #endregion

                        #region 月明波

                        case MagicType.GreaterHolyStrike:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(920, 6, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx3, 35, 35, Globals.HolyColour, CurrentLocation)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    Skip = 0,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(920, 6, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx3, 35, 35, Globals.HolyColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Skip = 0,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(930, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 20, 50, Globals.HolyColour)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeEnd);
                                };
                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeTravel);

                            break;

                        #endregion

                        #endregion

                        #region Assassin  刺客

                        //Willow Dance

                        //Vine Tree Dance

                        //Discipline

                        //Poisonous Cloud

                        //Full Bloom

                        //Cloak

                        //White Lotus

                        //Calamity Of Full Moon

                        #region 亡灵束缚

                        case MagicType.WraithGrip:

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.CanShowWraithGrip = false;
                                attackTarget.Effects.Add(spell = new MirEffect(1420, 14, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    BlendRate = 0.4f,
                                });
                                spell.CompleteAction = () => attackTarget.CanShowWraithGrip = true;

                                spell.Process();

                                attackTarget.Effects.Add(spell = new MirEffect(1440, 14, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    BlendRate = 0.4f,
                                });
                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.WraithGripEnd);
                            break;

                        #endregion

                        #region 烈焰

                        case MagicType.HellFire:
                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(1500, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                });

                                spell.Process();
                            }

                            if (AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.WraithGripEnd);
                            break;

                        #endregion

                        //Pledge Of Blood

                        //Rake

                        //Sweet Brier

                        //Summon Puppet

                        //Karma - Removed

                        //Touch Of Departed 

                        //Waning Moon

                        //Ghost Walk

                        //Elemental Puppet

                        //Rejuvenation

                        //Resolution

                        //Change Of Seasons

                        //Release

                        //Flame Splash

                        //Bloody Flower

                        //The New Beginning

                        //Dance Of Swallows

                        //Dark Conversion

                        //Dragon Repulsion

                        //Advent Of Demon

                        //Advent Of Devil

                        //Abyss

                        //Flash Of Light

                        //Stealth

                        //Evasion

                        //Raging Wind

                        #region 业火

                        case MagicType.SwordOfVengeance:
                            foreach (Point point in MagicLocations)
                            {
                                spell = new MirEffect(900, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 50, 80, Globals.FireColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                };
                            }
                            DXSoundManager.Play(SoundIndex.IceStormStart);
                            break;

                        #endregion

                        #endregion

                        #region 怪物特效

                        #region Fire Ball

                        case MagicType.PinkFireBall:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(1500, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx20, 35, 35, Color.Purple, CurrentLocation)
                                {
                                    Blend = true,
                                    Direction = action.Direction,
                                    MapTarget = point,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(1600, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx20, 35, 35, Color.Purple, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Has16Directions = false,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(1700, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx20, 35, 35, Color.Purple)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                        Direction = action.Direction
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.FireBallEnd);
                                };

                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.FireBallTravel);
                            break;

                        #endregion

                        #region Fire Ball

                        case MagicType.GreenSludgeBall:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(spell = new MirProjectile(2600, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx23, 35, 35, Color.GreenYellow, CurrentLocation)
                                {
                                    Blend = true,
                                    Direction = action.Direction,
                                    MapTarget = point,
                                });
                                spell.Process();
                            }

                            foreach (MapObject attackTarget in AttackTargets)
                            {
                                Effects.Add(spell = new MirProjectile(2600, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx23, 35, 35, Color.GreenYellow, CurrentLocation)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    Has16Directions = false,
                                });

                                spell.CompleteAction = () =>
                                {
                                    attackTarget.Effects.Add(spell = new MirEffect(2780, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx23, 35, 35, Color.GreenYellow)
                                    {
                                        Blend = true,
                                        Target = attackTarget,
                                        Direction = action.Direction
                                    });
                                    spell.Process();

                                    DXSoundManager.Play(SoundIndex.FireBallEnd);
                                };

                                spell.Process();
                            }

                            if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                                DXSoundManager.Play(SoundIndex.FireBallTravel);
                            break;

                        #endregion

                        #region Monster Scortched Earth

                        case MagicType.MonsterScortchedEarth:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    Effects.Add(new MirEffect(220, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
                                    {
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, CurrentLocation) * 50),
                                        Opacity = 0.8F,
                                        DrawType = DrawType.Floor,
                                    });

                                    Effects.Add(new MirEffect(2450 + CEnvir.Random.Next(5) * 10, 10, TimeSpan.FromMilliseconds(250), LibraryFile.Magic, 0, 0, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, CurrentLocation) * 50),
                                        DrawType = DrawType.Floor,
                                    });

                                    Effects.Add(new MirEffect(1930, 30, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 20, 70, Globals.FireColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(Functions.Distance(point, CurrentLocation) * 50),
                                        BlendRate = 1F,
                                    });
                                }

                            // if (MagicLocations.Count > 0)
                            //   DXSoundManager.Play(SoundIndex.LavaStrikeEnd);

                            break;

                        #endregion

                        #region MonsterIceStorm

                        case MagicType.MonsterIceStorm:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(new MirEffect(6230, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx3, 20, 70, Globals.IceColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    BlendRate = 1F,
                                });
                            }
                            break;


                        case MagicType.MonsterThunderStorm:
                            foreach (Point point in MagicLocations)
                            {
                                Effects.Add(new MirEffect(650, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx5, 20, 70, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    BlendRate = 1F,
                                });
                            }
                            break;

                        #endregion

                        case MagicType.SamaGuardianFire:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4000, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.FireColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaGuardianIce:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.IceColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaGuardianLightning:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4200, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.LightningColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaGuardianWind:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4300, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.WindColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;

                        case MagicType.SamaPhoenixFire:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4500, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.FireColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaBlackIce:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4600, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.IceColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaBlueLightning:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4700, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.LightningColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaWhiteWind:
                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    spell = new MirEffect(4800, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.WindColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                    };
                                    spell.Process();
                                }
                            break;
                        case MagicType.SamaProphetFire:
                            //   foreach (Point point in MagicLocations)

                            spell = new MirEffect(5600, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.FireColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();

                            break;
                        case MagicType.SamaProphetLightning:
                            //    foreach (Point point in MagicLocations)

                            spell = new MirEffect(5200, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.LightningColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();

                            break;
                        case MagicType.SamaProphetWind:
                            //     foreach (Point point in MagicLocations)

                            spell = new MirEffect(5400, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx9, 30, 80, Globals.WindColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();

                            break;

                            #endregion
                    }
                    break;
                case MirAction.Dead:
                    GameScene.Game.MapControl.SortObject(this);
                    break;
            }

            MoveDistance = 0;

            SetFrame(action);

            Direction = action.Direction;
            CurrentLocation = action.Location;

            AssaultEnd();
            ReigningStepEnd();

            List<uint> targets;

            if (action.Action != MirAction.Standing)
                DragonRepulseEnd();

            switch (action.Action)  //动作
            {
                case MirAction.Harvest:
                case MirAction.Die:
                case MirAction.Dead:
                case MirAction.Show:
                case MirAction.Hide:
                case MirAction.Mount:
                    break;
                case MirAction.Mining:
                    MiningEffect = (bool)action.Extra[0];
                    break;
                case MirAction.Moving:
                    MoveDistance = (int)action.Extra[0];
                    MagicType = (MagicType)action.Extra[1];

                    switch (MagicType)
                    {
                        case MagicType.Assault:
                            DXSoundManager.Play(SoundIndex.AssaultStart);
                            AssaultCreate();
                            break;
                        case MagicType.ReigningStep:
                            DXSoundManager.Play(SoundIndex.ReigningStepStart);
                            ReigningStepCreate();
                            break;
                    }
                    break;
                case MirAction.Standing:
                    bool hasdragonrepulse = VisibleBuffs.Contains(BuffType.DragonRepulse);
                    bool haselementalhurricane = VisibleBuffs.Contains(BuffType.ElementalHurricane);
                    if (!hasdragonrepulse && !haselementalhurricane)
                    {
                        if (DragonRepulseEffect != null)
                            DragonRepulseEnd();
                        break;
                    }

                    if (DragonRepulseEffect == null)
                        DragonRepulseCreate();
                    else if (haselementalhurricane && DragonRepulseEffect != null)
                        DragonRepulseEffect.Direction = Direction;
                    break;
                case MirAction.Pushed:
                    MoveDistance = 1;
                    break;
                case MirAction.RangeAttack:

                    targets = (List<uint>)action.Extra[0];
                    AttackTargets = new List<MapObject>();
                    foreach (uint target in targets)
                    {
                        MapObject attackTarget = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == target);
                        if (attackTarget == null) continue;
                        AttackTargets.Add(attackTarget);
                    }
                    break;
                case MirAction.Struck:
                    if (VisibleBuffs.Contains(BuffType.MagicShield))
                        MagicShieldStruck();

                    if (VisibleBuffs.Contains(BuffType.CelestialLight))
                        CelestialLightStruck();

                    AttackerID = (uint)action.Extra[0];

                    Element element = (Element)action.Extra[1];
                    switch (element)  //击中元素效果
                    {
                        case Element.None:
                            Effects.Add(new MirEffect(930, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Fire:
                            Effects.Add(new MirEffect(790, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Ice:
                            Effects.Add(new MirEffect(810, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Lightning:
                            Effects.Add(new MirEffect(830, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Wind:
                            Effects.Add(new MirEffect(850, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Holy:
                            Effects.Add(new MirEffect(870, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Dark:
                            Effects.Add(new MirEffect(890, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                        case Element.Phantom:
                            Effects.Add(new MirEffect(910, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;
                    }
                    break;
                case MirAction.Attack:
                    MagicType = (MagicType)action.Extra[1];
                    AttackElement = (Element)action.Extra[2];

                    Color attackColour = Globals.NoneColour;
                    switch (AttackElement)  //攻击时元素效果
                    {
                        case Element.Fire:
                            attackColour = Globals.FireColour;
                            break;
                        case Element.Ice:
                            attackColour = Globals.IceColour;
                            break;
                        case Element.Lightning:
                            attackColour = Globals.LightningColour;
                            break;
                        case Element.Wind:
                            attackColour = Globals.WindColour;
                            break;
                        case Element.Holy:
                            attackColour = Globals.HolyColour;
                            break;
                        case Element.Dark:
                            attackColour = Globals.DarkColour;
                            break;
                        case Element.Phantom:
                            attackColour = Globals.PhantomColour;
                            break;
                    }

                    switch (MagicType)  //魔法技能 元素效果
                    {
                        case MagicType.None:
                            if (Race != ObjectType.Player || CurrentAnimation != MirAnimation.Combat3 || AttackElement == Element.None) break;

                            Effects.Add(new MirEffect(1090, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 25, attackColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour
                            });
                            break;

                        #region 攻杀剑法

                        case MagicType.Slaying:
                            Effects.Add(new MirEffect(1350, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 50, attackColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour
                            });
                            if (Gender == MirGender.Male)
                                DXSoundManager.Play(SoundIndex.SlayingMale);
                            else
                                DXSoundManager.Play(SoundIndex.SlayingFemale);
                            break;

                        #endregion

                        #region 刺杀剑法

                        case MagicType.Thrusting:
                            Effects.Add(new MirEffect(0, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 20, 70, attackColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour,
                            });
                            DXSoundManager.Play(SoundIndex.EnergyBlast);
                            break;

                        #endregion

                        #region 半月弯刀

                        case MagicType.HalfMoon:

                            Effects.Add(new MirEffect(230, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 70, attackColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour
                            });
                            DXSoundManager.Play(SoundIndex.HalfMoon);
                            break;

                        #endregion

                        #region 十方斩

                        case MagicType.DestructiveSurge:

                            Effects.Add(new MirEffect(1420, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 70, attackColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                DrawColour = attackColour
                            });
                            DXSoundManager.Play(SoundIndex.DestructiveBlow);
                            break;

                        #endregion

                        #region 烈火剑法

                        case MagicType.FlamingSword:
                            Effects.Add(new MirEffect(1470, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 50, Globals.FireColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                            });

                            DXSoundManager.Play(SoundIndex.FlamingSword);
                            break;

                        #endregion

                        #region 翔空剑法

                        case MagicType.DragonRise:
                            Effects.Add(new MirEffect(2180, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 70, attackColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour,
                                //StartTime = CEnvir.Now.AddMilliseconds(500)
                            });

                            DXSoundManager.Play(SoundIndex.DragonRise);
                            break;

                        #endregion

                        #region 莲月剑法

                        case MagicType.BladeStorm:  //莲月剑法   调整莲月显示变慢
                            Effects.Add(new MirEffect(1780, 10, TimeSpan.FromMilliseconds(90), LibraryFile.MagicEx, 20, 70, attackColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour,
                            });

                            DXSoundManager.Play(SoundIndex.BladeStorm);
                            break;

                        #endregion

                        #region 屠龙斩

                        case MagicType.MaelstromBlade:
                            Effects.Add(new MirEffect(100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 20, 70, attackColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                DrawColour = attackColour,
                            });

                            DXSoundManager.Play(SoundIndex.BladeStorm);
                            break;

                        #endregion

                        #region 新月炎龙爆

                        case MagicType.FlameSplash:
                            Effects.Add(new MirEffect(900, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.FireColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                            });

                            DXSoundManager.Play(SoundIndex.BladeStorm);
                            break;

                        #endregion

                        #region 鹰击

                        case MagicType.DanceOfSwallow:
                            break;

                            #endregion

                    }
                    break;
                case MirAction.DiyAttack:
                    AttackElement = (Element)action.Extra[2];
                    MagicType = MagicType.None;
                    if (Enum.IsDefined(typeof(MagicType), (object)action.Extra[1]))
                    {
                        MagicType = (MagicType)action.Extra[1];
                        attackColour = Globals.NoneColour;
                        switch (AttackElement)
                        {
                            case Element.Fire:
                                attackColour = Globals.FireColour;
                                break;
                            case Element.Ice:
                                attackColour = Globals.IceColour;
                                break;
                            case Element.Lightning:
                                attackColour = Globals.LightningColour;
                                break;
                            case Element.Wind:
                                attackColour = Globals.WindColour;
                                break;
                            case Element.Holy:
                                attackColour = Globals.HolyColour;
                                break;
                            case Element.Dark:
                                attackColour = Globals.DarkColour;
                                break;
                            case Element.Phantom:
                                attackColour = Globals.PhantomColour;
                                break;
                        }

                        switch (MagicType)
                        {
                            case MagicType.None:
                                if (Race != ObjectType.Player || CurrentAnimation != MirAnimation.Combat3 || AttackElement == Element.None) break;

                                Effects.Add(new MirEffect(1090, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 25, attackColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour
                                });
                                break;

                            #region 攻杀剑术

                            case MagicType.Slaying:
                                Effects.Add(new MirEffect(1350, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 50, attackColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour
                                });
                                if (Gender == MirGender.Male)
                                    DXSoundManager.Play(SoundIndex.SlayingMale);
                                else
                                    DXSoundManager.Play(SoundIndex.SlayingFemale);
                                break;

                            #endregion

                            #region 刺杀剑术

                            case MagicType.Thrusting:
                                Effects.Add(new MirEffect(0, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour,
                                });
                                DXSoundManager.Play(SoundIndex.EnergyBlast);
                                break;

                            #endregion

                            #region 半月弯刀

                            case MagicType.HalfMoon:

                                Effects.Add(new MirEffect(230, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour
                                });
                                DXSoundManager.Play(SoundIndex.HalfMoon);
                                break;

                            #endregion

                            #region 旋风斩

                            case MagicType.SwirlingBlade:

                                Effects.Add(new MirEffect(220, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    DrawColour = attackColour
                                });
                                DXSoundManager.Play(SoundIndex.DestructiveBlow);
                                break;

                            #endregion

                            #region 十方斩

                            case MagicType.DestructiveSurge:

                                Effects.Add(new MirEffect(1420, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    DrawColour = attackColour
                                });
                                DXSoundManager.Play(SoundIndex.DestructiveBlow);
                                break;

                            #endregion

                            #region 烈火剑法

                            case MagicType.FlamingSword:
                                Effects.Add(new MirEffect(1470, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 50, Globals.FireColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                });

                                DXSoundManager.Play(SoundIndex.FlamingSword);
                                break;

                            #endregion

                            #region 翔空剑法

                            case MagicType.DragonRise:
                                Effects.Add(new MirEffect(2180, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour,
                                    //StartTime = CEnvir.Now.AddMilliseconds(500)
                                });

                                DXSoundManager.Play(SoundIndex.DragonRise);
                                break;

                            #endregion

                            #region 屠龙斩

                            case MagicType.MaelstromBlade:
                                Effects.Add(new MirEffect(100, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour,
                                });

                                DXSoundManager.Play(SoundIndex.BladeStorm);
                                break;

                            #endregion

                            #region 莲月剑法

                            case MagicType.BladeStorm:
                                Effects.Add(new MirEffect(1780, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 20, 70, attackColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    DrawColour = attackColour,
                                });

                                DXSoundManager.Play(SoundIndex.BladeStorm);
                                break;

                            #endregion

                            #region 新月炎龙爆

                            case MagicType.FlameSplash:
                                Effects.Add(new MirEffect(900, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.FireColour) //Element style?
                                {
                                    Blend = true,
                                    Target = this,
                                });

                                DXSoundManager.Play(SoundIndex.BladeStorm);
                                break;

                            #endregion

                            #region 鹰击

                            case MagicType.DanceOfSwallow:
                                break;

                                #endregion

                        }
                    }
                    else//自定义技能--TODO
                    {
                        DiyMagicEffect MagicEffectinfo = Globals.DiyMagicEffectList.Binding.FirstOrDefault(x => x.MagicID == (int)action.Extra[1] && x.MagicType <= DiyMagicType.ExplosionMagic);
                        if (MagicEffectinfo != null)
                        {
                            ShowDiyMagicEffect(MagicEffectinfo, AttackElement);
                        }
                    }

                    break;
                case MirAction.Spell:   //施法动作
                    MagicType = (MagicType)action.Extra[0];

                    targets = (List<uint>)action.Extra[1];
                    AttackTargets = new List<MapObject>();
                    foreach (uint target in targets)
                    {
                        MapObject attackTarget = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == target);
                        if (attackTarget == null) continue;
                        AttackTargets.Add(attackTarget);
                    }
                    MagicLocations = (List<Point>)action.Extra[2];
                    MagicCast = (bool)action.Extra[3];

                    AttackElement = (Element)action.Extra[4];

                    Point location;
                    switch (MagicType)  //技能起始释放效果
                    {

                        #region Warrior 战士
                        //Swordsmanship

                        //Potion Mastery 

                        //Slaying

                        //Thrusting

                        //Half Moon

                        //Shoulder Dash

                        //Flaming Sword

                        //Dragon Rise

                        //Blade Storm

                        //Destructive Surge

                        #region 乾坤大挪移

                        case MagicType.Interchange:
                            Effects.Add(new MirEffect(0, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.TeleportationStart);
                            break;

                        #endregion

                        #region 铁布衫 & 无敌

                        case MagicType.Defiance:
                        case MagicType.Invincibility:
                            Effects.Add(new MirEffect(40, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.DefianceStart);
                            break;

                        #endregion

                        #region 斗转星移 & 挑衅

                        case MagicType.Beckon:
                        case MagicType.MassBeckon:
                            Effects.Add(new MirEffect(580, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            DXSoundManager.Play(SoundIndex.TeleportationStart);
                            break;

                        #endregion

                        #region 破血狂杀

                        case MagicType.Might:
                            Effects.Add(new MirEffect(60, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.DragonRise); //Same file as Beckon
                            break;

                        #endregion

                        #region 天雷锤

                        case MagicType.SeismicSlam:
                            Effects.Add(spell = new MirEffect(4900, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                            });
                            break;

                        #endregion

                        #region 空破斩

                        case MagicType.CrushingWave:
                            Effects.Add(spell = new MirEffect(100, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 0, 0, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                            });
                            break;

                        #endregion

                        //Swift Blade

                        //Assault - will be passive?

                        //Endurance

                        #region 移花接木

                        case MagicType.ReflectDamage:
                            Effects.Add(new MirEffect(1220, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this
                            });
                            DXSoundManager.Play(SoundIndex.DefianceStart);
                            break;

                        #endregion

                        #region 泰山压顶

                        case MagicType.Fetter:
                            Effects.Add(new MirEffect(2370, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this
                            });
                            break;

                        #endregion

                        #endregion

                        #region Wizard 法师

                        #region 火球术

                        case MagicType.FireBall:
                            Effects.Add(spell = new MirEffect(1820, 8, TimeSpan.FromMilliseconds(70), LibraryFile.Magic, 10, 35, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.FireBallStart);
                            break;

                        #endregion

                        #region 霹雳掌

                        case MagicType.LightningBall:
                            Effects.Add(spell = new MirEffect(2990, 6, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            DXSoundManager.Play(SoundIndex.ThunderBoltStart);
                            break;

                        #endregion

                        #region 冰月神掌

                        case MagicType.IceBolt:
                            Effects.Add(spell = new MirEffect(2620, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.IceBoltStart);
                            break;

                        #endregion

                        #region 风掌

                        case MagicType.GustBlast:
                            Effects.Add(spell = new MirEffect(350, 7, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            DXSoundManager.Play(SoundIndex.GustBlastStart);
                            break;

                        #endregion

                        #region 抗拒火环

                        case MagicType.Repulsion:
                            Effects.Add(new MirEffect(90, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.RepulsionEnd);
                            break;

                        #endregion

                        #region 诱惑之光

                        case MagicType.ElectricShock:
                            Effects.Add(spell = new MirEffect(0, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.ElectricShockStart);
                            break;

                        #endregion

                        #region 瞬息移动

                        case MagicType.Teleportation:
                            Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.TeleportationStart);
                            break;

                        #endregion

                        #region 大火球l & 焰天火雨

                        case MagicType.AdamantineFireBall:
                        case MagicType.MeteorShower:
                            Effects.Add(spell = new MirEffect(1560, 9, TimeSpan.FromMilliseconds(65), LibraryFile.Magic, 10, 35, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.GreaterFireBallStart);
                            break;

                        #endregion

                        #region 雷电术

                        case MagicType.ThunderBolt:
                            Effects.Add(spell = new MirEffect(1430, 12, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.LightningStrikeStart);
                            break;

                        #endregion

                        #region 冰月震天

                        case MagicType.IceBlades:
                            Effects.Add(spell = new MirEffect(2880, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.GreaterIceBoltStart);
                            break;

                        #endregion

                        #region 击风

                        case MagicType.Cyclone:
                            Effects.Add(spell = new MirEffect(1970, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.CycloneStart);
                            break;

                        #endregion

                        #region 地狱火

                        case MagicType.ScortchedEarth:
                            if (Config.DrawEffects)
                            {
                                Effects.Add(spell = new MirEffect(1820, 8, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                });
                                DXSoundManager.Play(SoundIndex.LavaStrikeStart);
                            }

                            break;

                        #endregion

                        #region 疾光电影

                        case MagicType.LightningBeam:
                            Effects.Add(spell = new MirEffect(1970, 10, TimeSpan.FromMilliseconds(30), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            break;

                        #endregion

                        #region 冰沙掌

                        case MagicType.FrozenEarth:
                            Effects.Add(spell = new MirEffect(0, 10, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 10, 35, Globals.IceColour)
                            {
                                //Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            DXSoundManager.Play(SoundIndex.FrozenEarthStart);
                            break;

                        #endregion

                        #region 风震天

                        case MagicType.BlowEarth:
                            Effects.Add(spell = new MirEffect(1970, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.BlowEarthStart);
                            break;

                        #endregion

                        #region 火墙

                        case MagicType.FireWall:
                            Effects.Add(new MirEffect(910, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.FireWallStart);
                            break;

                        #endregion

                        #region 冰雨

                        case MagicType.IceRain:
                            Effects.Add(new MirEffect(770, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.IceStormStart);
                            break;

                        #endregion

                        #region 圣言术

                        case MagicType.ExpelUndead:
                            Effects.Add(spell = new MirEffect(130, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.ExpelUndeadStart);
                            break;

                        #endregion

                        #region 移形换位

                        case MagicType.GeoManipulation:
                            Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.TeleportationStart);
                            break;

                        #endregion

                        #region 魔法盾

                        case MagicType.MagicShield:
                            if (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User))
                            {
                                Effects.Add(new MirEffect(830, 19, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                            }
                            DXSoundManager.Play(SoundIndex.MagicShieldStart);
                            break;

                        #endregion

                        #region 爆裂火焰

                        case MagicType.FireStorm:
                            Effects.Add(spell = new MirEffect(940, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.FireStormStart);
                            break;

                        #endregion

                        #region 地狱雷光

                        case MagicType.LightningWave:
                            Effects.Add(spell = new MirEffect(1430, 12, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this
                            });
                            DXSoundManager.Play(SoundIndex.LightningWaveStart);
                            break;

                        #endregion

                        #region 冰咆哮

                        case MagicType.IceStorm:
                            Effects.Add(spell = new MirEffect(770, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.IceStormStart);
                            break;

                        #endregion

                        #region 龙卷风

                        case MagicType.DragonTornado:
                            Effects.Add(spell = new MirEffect(1030, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.DragonTornadoStart);
                            break;

                        #endregion

                        #region 魄冰刺

                        case MagicType.GreaterFrozenEarth:
                            Effects.Add(spell = new MirEffect(0, 10, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 10, 35, Globals.IceColour)
                            {
                                //Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            DXSoundManager.Play(SoundIndex.GreaterFrozenEarthStart);
                            break;

                        #endregion

                        #region 怒神霹雳

                        case MagicType.ChainLightning:
                            Effects.Add(spell = new MirEffect(1430, 12, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this
                            });
                            DXSoundManager.Play(SoundIndex.ChainLightningStart);
                            break;

                        #endregion

                        //Meteor Strike -> Great Fire Ball

                        #region 凝血离魂

                        case MagicType.Renounce:
                            if (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User))
                            {
                                Effects.Add(new MirEffect(80, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                            }
                            DXSoundManager.Play(SoundIndex.DefianceStart);
                            break;

                        #endregion

                        #region 旋风墙

                        case MagicType.Tempest:
                            Effects.Add(new MirEffect(910, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx2, 10, 35, Globals.WindColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.BlowEarthStart);
                            break;

                        #endregion

                        #region 天打雷劈

                        case MagicType.JudgementOfHeaven:
                            DXSoundManager.Play(SoundIndex.LightningStrikeEnd);
                            break;

                        #endregion

                        #region 电闪雷鸣

                        case MagicType.ThunderStrike:
                            DXSoundManager.Play(SoundIndex.LightningStrikeStart);
                            break;

                        #endregion

                        #region 分身术

                        case MagicType.MirrorImage:
                            Effects.Add(new MirEffect(1260, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.ShacklingTalismanStart);
                            break;

                        #endregion

                        #region 护身冰环

                        case MagicType.FrostBite:
                            Effects.Add(new MirEffect(500, 16, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx5, 10, 35, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.FrostBiteStart);
                            break;

                        #endregion

                        #region 护身法盾

                        case MagicType.SuperiorMagicShield:
                            Effects.Add(new MirEffect(1900, 17, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx2, 10, 35, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.MagicShieldStart);
                            break;

                        #endregion

                        #endregion

                        #region Taoist 道士

                        #region 治愈术

                        case MagicType.Heal:
                            Effects.Add(spell = new MirEffect(660, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.HealStart);
                            break;

                        #endregion

                        //Spirit Sword

                        #region 施毒术

                        case MagicType.PoisonDust:
                            Effects.Add(spell = new MirEffect(60, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.PoisonDustStart);
                            break;

                        #endregion

                        #region 灵魂火符

                        case MagicType.ExplosiveTalisman:
                            Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.ExplosiveTalismanStart);
                            break;

                        #endregion

                        #region 月魂断玉

                        case MagicType.EvilSlayer:
                            Effects.Add(spell = new MirEffect(3250, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.HolyStrikeStart);
                            break;

                        #endregion

                        #region 召唤骷髅 & 超强召唤骷髅 & 迷魂大法

                        case MagicType.SummonSkeleton:
                        case MagicType.SummonJinSkeleton:
                        case MagicType.Scarecrow:
                            Effects.Add(new MirEffect(740, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.SummonSkeletonStart);
                            break;

                        #endregion

                        #region 隐身术

                        case MagicType.Invisibility:
                            Effects.Add(new MirEffect(810, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.InvisibilityEnd);
                            break;

                        #endregion

                        #region 幽灵盾

                        case MagicType.MagicResistance:
                            Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            break;

                        #endregion

                        #region 集体隐身术

                        case MagicType.MassInvisibility:
                            Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            break;

                        #endregion

                        #region 月魂灵波

                        case MagicType.GreaterEvilSlayer:
                            Effects.Add(spell = new MirEffect(3360, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeStart);
                            break;

                        #endregion

                        #region 神圣战甲术

                        case MagicType.Resilience:
                            Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            break;

                        #endregion

                        #region 困魔咒

                        case MagicType.TrapOctagon:
                            Effects.Add(spell = new MirEffect(630, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                            });

                            DXSoundManager.Play(SoundIndex.ShacklingTalismanStart);
                            break;

                        #endregion

                        #region 空拳刀法

                        case MagicType.TaoistCombatKick:
                            DXSoundManager.Play(SoundIndex.TaoistCombatKickStart);
                            break;

                        #endregion

                        #region 强震魔法

                        case MagicType.ElementalSuperiority:
                            Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            break;

                        #endregion

                        #region 召唤神兽

                        case MagicType.SummonShinsu:
                            Effects.Add(new MirEffect(2590, 19, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                            {
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.SummonShinsuStart);
                            break;

                        #endregion

                        #region 群体治愈术

                        case MagicType.MassHeal:
                            Effects.Add(spell = new MirEffect(660, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.MassHealStart);
                            break;

                        #endregion

                        //Summon Jin Skeleton -> Summon Skeleton

                        #region 猛虎强势

                        case MagicType.BloodLust:
                            Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });
                            break;

                        #endregion

                        #region 回生术

                        case MagicType.Resurrection:
                            Effects.Add(spell = new MirEffect(310, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 60, 60, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.ResurrectionStart);
                            break;

                        #endregion

                        #region 云寂术

                        case MagicType.Purification:
                            Effects.Add(spell = new MirEffect(220, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx2, 20, 40, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.PurificationStart);
                            break;

                        #endregion

                        #region 移花接玉

                        case MagicType.StrengthOfFaith:
                            Effects.Add(spell = new MirEffect(360, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 40, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.StrengthOfFaithStart);
                            break;

                        #endregion

                        #region 妙影无踪

                        case MagicType.Transparency:
                            Effects.Add(new MirEffect(430, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.InvisibilityEnd);
                            break;

                        #endregion

                        #region 阴阳法环

                        case MagicType.CelestialLight:
                            if (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User))
                            {
                                Effects.Add(new MirEffect(280, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    DrawColour = Color.Yellow,
                                });
                            }
                            DXSoundManager.Play(SoundIndex.MagicShieldStart);
                            break;

                        #endregion

                        #region 吸星大法

                        case MagicType.LifeSteal:
                            Effects.Add(new MirEffect(2410, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                            });
                            DXSoundManager.Play(SoundIndex.HolyStrikeStart);
                            break;

                        #endregion

                        #region 灭魂火符

                        case MagicType.ImprovedExplosiveTalisman:
                            Effects.Add(spell = new MirEffect(980, 6, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx2, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.ExplosiveTalismanStart);
                            break;

                        #endregion

                        //Greater Poison Dust -> Poison Dust

                        //Scarecrow -> Summon Skeleton

                        #region 横扫千军

                        case MagicType.ThunderKick:
                            DXSoundManager.Play(SoundIndex.TaoistCombatKickStart);
                            break;

                        #endregion

                        //Neutralize

                        #region 暗鬼阵

                        case MagicType.DarkSoulPrison:
                            Effects.Add(new MirEffect(600, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.DarkSoulPrison);
                            break;

                        #endregion

                        #region 隐魂术

                        case MagicType.MassTransparency:
                            Effects.Add(spell = new MirEffect(690, 10, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx3, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                Target = this,
                            });

                            break;

                        #endregion

                        #region 月明波

                        case MagicType.GreaterHolyStrike:
                            Effects.Add(spell = new MirEffect(840, 7, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx3, 10, 35, Globals.HolyColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction
                            });

                            DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeStart);
                            break;

                        #endregion

                        #endregion

                        #region Assassin 刺客

                        //Willow Dance

                        //Vine Tree Dance

                        //Discipline

                        //Poisonous Cloud

                        //Full Bloom

                        #region 潜行

                        case MagicType.Cloak:
                            Effects.Add(new MirEffect(600, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 10, 35, Globals.PhantomColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            });
                            DXSoundManager.Play(SoundIndex.CloakStart);
                            break;

                        #endregion

                        //White Lotus

                        //Calamity Of Full Moon

                        #region 亡灵束缚

                        case MagicType.WraithGrip:
                            Effects.Add(spell = new MirEffect(1460, 15, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                            {
                                Blend = true,
                                Target = this,
                                BlendRate = 0.4f,
                            });
                            DXSoundManager.Play(SoundIndex.WraithGripStart);
                            break;

                        #endregion

                        #region 烈焰

                        case MagicType.HellFire:
                            Effects.Add(spell = new MirEffect(1520, 15, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 60, 60, Globals.FireColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.WraithGripStart);
                            break;

                        #endregion

                        //Pledge Of Blood

                        #region 血之盟约

                        case MagicType.Rake:
                            Effects.Add(spell = new MirEffect(1200, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.IceColour)
                            {
                                Blend = true,
                                Target = this,
                                Direction = action.Direction,
                                Skip = 10,
                            });
                            DXSoundManager.Play(SoundIndex.RakeStart);
                            break;

                        #endregion

                        //Sweet Brier

                        #region 亡灵替身

                        case MagicType.SummonPuppet:
                            Effects.Add(new MirEffect(800, 16, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 80, 50, Globals.PhantomColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            });
                            DXSoundManager.Play(SoundIndex.SummonPuppet);
                            break;

                        #endregion

                        //Karma - Removed

                        //Touch Of Departed 

                        //Waning Moon

                        //Ghost Walk

                        //Elemental Puppet

                        //Rejuvenation

                        //Resolution

                        //Change Of Seasons

                        //Release

                        //Flame Splash

                        //Bloody Flower

                        #region 心机一转

                        case MagicType.TheNewBeginning:
                            if (User.Stats[Stat.TheNewBeginning] < 1)
                            {
                                Effects.Add(spell = new MirEffect(500, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    //Direction = action.Direction
                                });
                            }
                            if (User.Stats[Stat.TheNewBeginning] == 1)
                            {
                                Effects.Add(spell = new MirEffect(510, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    //Direction = action.Direction
                                });
                            }
                            if (User.Stats[Stat.TheNewBeginning] == 2)
                            {
                                Effects.Add(spell = new MirEffect(520, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    //Direction = action.Direction
                                });
                            }
                            if (User.Stats[Stat.TheNewBeginning] >= 3)
                            {
                                Effects.Add(spell = new MirEffect(530, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    //Direction = action.Direction
                                });
                            }
                            DXSoundManager.Play(SoundIndex.TheNewBeginning);
                            break;


                        #endregion

                        //Dance Of Swallows

                        //Dark Conversion

                        #region 狂涛泉涌

                        case MagicType.DragonRepulse:
                            Effects.Add(new MirEffect(1000, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 0, 0, Globals.NoneColour)
                            {
                                MapTarget = CurrentLocation,
                            });
                            Effects.Add(new MirEffect(1020, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 80, 50, Globals.LightningColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            });
                            DXSoundManager.Play(SoundIndex.DragonRepulseStart);
                            break;

                        #endregion

                        //Advent Of Demon

                        //Advent Of Devil

                        #region 深渊苦海

                        case MagicType.Abyss:
                            Effects.Add(new MirEffect(2000, 14, TimeSpan.FromMilliseconds(70), LibraryFile.MagicEx4, 80, 50, Globals.PhantomColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            });
                            DXSoundManager.Play(SoundIndex.AbyssStart);
                            break;

                        #endregion

                        #region 日闪

                        case MagicType.FlashOfLight:
                            DXSoundManager.Play(SoundIndex.FlashOfLightStart);
                            break;

                        #endregion

                        //Stealth

                        #region 风之闪避

                        case MagicType.Evasion:
                            Effects.Add(new MirEffect(2500, 12, TimeSpan.FromMilliseconds(70), LibraryFile.MagicEx4, 80, 50, Globals.NoneColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            });
                            DXSoundManager.Play(SoundIndex.EvasionStart);
                            break;

                        #endregion

                        #region 风之守护

                        case MagicType.RagingWind:
                            Effects.Add(new MirEffect(2600, 12, TimeSpan.FromMilliseconds(70), LibraryFile.MagicEx4, 80, 50, Globals.NoneColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            });
                            DXSoundManager.Play(SoundIndex.RagingWindStart);
                            break;

                        #endregion

                        #region 集中

                        case MagicType.Concentration:
                            Effects.Add(new MirEffect(300, 15, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 10, 35, Globals.DarkColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            DXSoundManager.Play(SoundIndex.Concentration);
                            break;

                        #endregion

                        #region 千刃杀风

                        case MagicType.ThousandBlades:
                            Effects.Add(spell = new MirEffect(590, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 10, 35, Globals.LightningColour)
                            {
                                Blend = true,
                                Target = this,
                            });
                            break;

                        #endregion

                        #endregion

                        #region 怪物特效

                        case MagicType.MonsterScortchedEarth:

                            location = CurrentLocation;

                            if (Config.DrawEffects)
                                foreach (Point point in MagicLocations)
                                {
                                    Effects.Add(new MirEffect(220, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
                                    {
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, location) * 50),
                                        Opacity = 0.8F,
                                        DrawType = DrawType.Floor,
                                    });

                                    Effects.Add(new MirEffect(2450 + CEnvir.Random.Next(5) * 10, 10, TimeSpan.FromMilliseconds(250), LibraryFile.Magic, 0, 0, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, location) * 50),
                                        DrawType = DrawType.Floor,
                                    });

                                    Effects.Add(new MirEffect(1930, 30, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 20, 70, Globals.FireColour)
                                    {
                                        Blend = true,
                                        MapTarget = point,
                                        StartTime = CEnvir.Now.AddMilliseconds(Functions.Distance(point, location) * 50),
                                        BlendRate = 1F,
                                    });
                                }

                            // if (MagicLocations.Count > 0)
                            //   DXSoundManager.Play(SoundIndex.LavaStrikeEnd);

                            break;
                        case MagicType.DoomClawRightPinch:

                            spell = new MirEffect(2640, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();

                            spell.CompleteAction = () =>
                            {
                                spell = new MirEffect(2680, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = Functions.Move(Functions.Move(CurrentLocation, MirDirection.Down, 0), MirDirection.Right, 5),
                                };
                                spell.Process();
                            };

                            break;
                        case MagicType.DoomClawLeftPinch:

                            spell = new MirEffect(2660, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();

                            spell.CompleteAction = () =>
                            {
                                spell = new MirEffect(2680, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = Functions.Move(CurrentLocation, MirDirection.Right, 5),
                                };
                                spell.Process();
                            };
                            break;
                        case MagicType.DoomClawRightSwipe:

                            spell = new MirEffect(2700, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();
                            break;
                        case MagicType.DoomClawLeftSwipe:

                            spell = new MirEffect(2720, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                            {
                                Blend = true,
                                MapTarget = CurrentLocation,
                            };
                            spell.Process();
                            break;
                        case MagicType.DoomClawSpit:
                            foreach (Point point in MagicLocations)
                            {
                                MirProjectile eff;  //投射效果
                                Point p = new Point(point.X, point.Y - 10);  //坐标偏移
                                Effects.Add(eff = new MirProjectile(2500, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour, p)
                                {
                                    MapTarget = point,
                                    Skip = 0,
                                    Explode = true,
                                    Blend = true,
                                });

                                eff.CompleteAction = () =>
                                {
                                    Effects.Add(new MirEffect(2520, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                    {
                                        MapTarget = eff.MapTarget,
                                        Blend = true,
                                    });
                                };

                            }
                            break;

                            #endregion
                    }
                    break;

                case MirAction.DiySpell:
                    targets = (List<uint>)action.Extra[1];
                    AttackTargets = new List<MapObject>();
                    foreach (uint target in targets)
                    {
                        MapObject attackTarget = GameScene.Game.MapControl.Objects.FirstOrDefault(x => x.ObjectID == target);
                        if (attackTarget == null) continue;
                        AttackTargets.Add(attackTarget);
                    }
                    MagicLocations = (List<Point>)action.Extra[2];
                    MagicCast = (bool)action.Extra[3];

                    AttackElement = (Element)action.Extra[4];

                    MagicType = MagicType.None;

                    if (Enum.IsDefined(typeof(MagicType), (object)action.Extra[0]))
                    {
                        MagicType = (MagicType)action.Extra[0];

                        switch (MagicType)
                        {

                            #region 战士

                            //Swordsmanship

                            //Potion Mastery 

                            //Slaying

                            //Thrusting

                            //Half Moon

                            //Shoulder Dash

                            //Flaming Sword

                            //Dragon Rise

                            //Blade Storm

                            //Destructive Surge

                            #region 乾坤大挪移

                            case MagicType.Interchange:
                                Effects.Add(new MirEffect(0, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.TeleportationStart);
                                break;

                            #endregion

                            #region 铁布衫 无敌

                            case MagicType.Defiance:

                            case MagicType.Invincibility:

                                Effects.Add(new MirEffect(40, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.DefianceStart);
                                break;

                            #endregion

                            #region 斗转星移 挑衅

                            case MagicType.Beckon:
                            case MagicType.MassBeckon:
                                Effects.Add(new MirEffect(580, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                DXSoundManager.Play(SoundIndex.TeleportationStart);
                                break;

                            #endregion

                            #region 破血狂杀

                            case MagicType.Might:
                                Effects.Add(new MirEffect(60, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.DragonRise); //Same file as Beckon
                                break;

                            #endregion

                            #region 天雷锤

                            case MagicType.SeismicSlam:
                                Effects.Add(spell = new MirEffect(4900, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                });
                                break;

                            #endregion

                            #region 空破斩

                            case MagicType.CrushingWave:
                                Effects.Add(spell = new MirEffect(100, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 0, 0, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                });
                                break;

                            #endregion

                            //Swift Blade

                            //Assault - will be passive?

                            //Endurance

                            #region 移花接木

                            case MagicType.ReflectDamage:
                                Effects.Add(new MirEffect(1220, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this
                                });
                                DXSoundManager.Play(SoundIndex.DefianceStart);
                                break;

                            #endregion

                            #region 泰山压顶

                            case MagicType.Fetter:
                                Effects.Add(new MirEffect(2370, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this
                                });
                                break;

                            #endregion

                            #endregion

                            #region 法师

                            #region 火球术

                            case MagicType.FireBall:
                                Effects.Add(spell = new MirEffect(1820, 8, TimeSpan.FromMilliseconds(70), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.FireBallStart);
                                break;

                            #endregion

                            #region 霹雳掌

                            case MagicType.LightningBall:
                                Effects.Add(spell = new MirEffect(2990, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                DXSoundManager.Play(SoundIndex.ThunderBoltStart);
                                break;

                            #endregion

                            #region 冰月神掌

                            case MagicType.IceBolt:
                                Effects.Add(spell = new MirEffect(2620, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.IceBoltStart);
                                break;

                            #endregion

                            #region 风掌

                            case MagicType.GustBlast:
                                Effects.Add(spell = new MirEffect(350, 7, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                DXSoundManager.Play(SoundIndex.GustBlastStart);
                                break;

                            #endregion

                            #region 抗拒火环

                            case MagicType.Repulsion:
                                Effects.Add(new MirEffect(90, 10, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.RepulsionEnd);
                                break;

                            #endregion

                            #region 诱惑之光

                            case MagicType.ElectricShock:
                                Effects.Add(spell = new MirEffect(0, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.ElectricShockStart);
                                break;

                            #endregion

                            #region 瞬息移动

                            case MagicType.Teleportation:
                                Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.TeleportationStart);
                                break;

                            #endregion

                            #region 大火球 焰天火雨

                            case MagicType.AdamantineFireBall:
                            case MagicType.MeteorShower:
                                Effects.Add(spell = new MirEffect(1560, 9, TimeSpan.FromMilliseconds(65), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.GreaterFireBallStart);
                                break;

                            #endregion

                            #region 雷电术

                            case MagicType.ThunderBolt:
                                Effects.Add(spell = new MirEffect(1430, 12, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.LightningStrikeStart);
                                break;

                            #endregion

                            #region 冰月震天

                            case MagicType.IceBlades:
                                Effects.Add(spell = new MirEffect(2880, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.GreaterIceBoltStart);
                                break;

                            #endregion

                            #region 击风

                            case MagicType.Cyclone:
                                Effects.Add(spell = new MirEffect(1970, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.CycloneStart);
                                break;

                            #endregion

                            #region 地狱火

                            case MagicType.ScortchedEarth:
                                if (Config.DrawEffects && Race != ObjectType.Monster)
                                {
                                    Effects.Add(spell = new MirEffect(1820, 8, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                    {
                                        Blend = true,
                                        Target = this,
                                        Direction = action.Direction,
                                    });
                                    DXSoundManager.Play(SoundIndex.LavaStrikeStart);
                                }

                                break;

                            #endregion

                            #region 疾光电影

                            case MagicType.LightningBeam:
                                Effects.Add(spell = new MirEffect(1970, 10, TimeSpan.FromMilliseconds(30), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                break;

                            #endregion

                            #region 冰沙掌

                            case MagicType.FrozenEarth:
                                Effects.Add(spell = new MirEffect(0, 10, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                DXSoundManager.Play(SoundIndex.FrozenEarthStart);
                                break;

                            #endregion

                            #region 风震天

                            case MagicType.BlowEarth:
                                Effects.Add(spell = new MirEffect(1970, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.BlowEarthStart);
                                break;

                            #endregion

                            #region 火墙

                            case MagicType.FireWall:
                                Effects.Add(new MirEffect(910, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.FireWallStart);
                                break;

                            #endregion

                            #region 圣言术

                            case MagicType.ExpelUndead:
                                Effects.Add(spell = new MirEffect(130, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.ExpelUndeadStart);
                                break;

                            #endregion

                            #region 移形换位

                            case MagicType.GeoManipulation:
                                Effects.Add(new MirEffect(110, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.TeleportationStart);
                                break;

                            #endregion

                            #region 魔法盾

                            case MagicType.MagicShield:
                                Effects.Add(new MirEffect(830, 19, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.MagicShieldStart);
                                break;

                            #endregion

                            #region 护身法盾

                            case MagicType.SuperiorMagicShield:
                                Effects.Add(new MirEffect(1900, 17, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx2, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.MagicShieldStart);
                                break;

                            #endregion

                            #region 爆裂火焰

                            case MagicType.FireStorm:
                                Effects.Add(spell = new MirEffect(940, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.FireStormStart);
                                break;

                            #endregion

                            #region 地狱雷光

                            case MagicType.LightningWave:
                                Effects.Add(spell = new MirEffect(1430, 12, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this
                                });
                                DXSoundManager.Play(SoundIndex.LightningWaveStart);
                                break;

                            #endregion

                            #region 冰咆哮

                            case MagicType.IceStorm:
                                Effects.Add(spell = new MirEffect(770, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.IceStormStart);
                                break;

                            #endregion

                            #region 龙卷风

                            case MagicType.DragonTornado:
                                Effects.Add(spell = new MirEffect(1030, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.DragonTornadoStart);
                                break;

                            #endregion

                            #region 魄冰刺

                            case MagicType.GreaterFrozenEarth:
                                Effects.Add(spell = new MirEffect(0, 10, TimeSpan.FromMilliseconds(50), LibraryFile.MagicEx, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                DXSoundManager.Play(SoundIndex.GreaterFrozenEarthStart);
                                break;

                            #endregion

                            #region 怒神霹雳

                            case MagicType.ChainLightning:
                                Effects.Add(spell = new MirEffect(1430, 12, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this
                                });
                                DXSoundManager.Play(SoundIndex.ChainLightningStart);
                                break;

                            #endregion

                            //Meteor Strike -> Great Fire Ball

                            #region 凝血离魂

                            case MagicType.Renounce:
                                Effects.Add(new MirEffect(80, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.DefianceStart);
                                break;

                            #endregion

                            #region 地狱魔焰
                            case MagicType.TempestOfUnstableEnergy:
                                Effects.Add(new MirEffect(590, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.BlowEarthStart);
                                break;

                            #endregion

                            #region 旋风墙

                            case MagicType.Tempest:
                                Effects.Add(new MirEffect(910, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx2, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.BlowEarthStart);
                                break;

                            #endregion

                            #region 冰雨

                            case MagicType.IceRain:
                                Effects.Add(new MirEffect(770, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.WindColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.IceStormStart);
                                break;

                            #endregion

                            #region 天打雷劈

                            case MagicType.JudgementOfHeaven:
                                DXSoundManager.Play(SoundIndex.LightningStrikeEnd);
                                break;

                            #endregion

                            #region 电闪雷鸣

                            case MagicType.ThunderStrike:
                                DXSoundManager.Play(SoundIndex.LightningStrikeStart);
                                break;

                            #endregion

                            #region 分身术

                            case MagicType.MirrorImage:
                                DXSoundManager.Play(SoundIndex.ShacklingTalismanStart);
                                break;

                            #endregion

                            #region 护身冰环

                            case MagicType.FrostBite:
                                Effects.Add(new MirEffect(500, 16, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx5, 10, 35, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.FrostBiteStart);
                                break;

                            #endregion

                            #endregion

                            #region 道士

                            #region 治愈术

                            case MagicType.Heal:
                                Effects.Add(spell = new MirEffect(660, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.HealStart);
                                break;

                            #endregion

                            //Spirit Sword

                            #region 施毒术

                            case MagicType.PoisonDust:
                                Effects.Add(spell = new MirEffect(60, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.PoisonDustStart);
                                break;

                            #endregion

                            #region 灵魂火符

                            case MagicType.ExplosiveTalisman:
                                Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.ExplosiveTalismanStart);
                                break;

                            #endregion

                            #region 月魂断玉

                            case MagicType.EvilSlayer:
                                Effects.Add(spell = new MirEffect(3250, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.HolyStrikeStart);
                                break;

                            #endregion

                            #region 召唤骷髅 超强骷髅 迷魂大法

                            case MagicType.SummonSkeleton:
                            case MagicType.SummonJinSkeleton:
                            case MagicType.Scarecrow:
                                Effects.Add(new MirEffect(740, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.SummonSkeletonStart);
                                break;

                            #endregion

                            #region 隐身术

                            case MagicType.Invisibility:
                                Effects.Add(new MirEffect(810, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.InvisibilityEnd);
                                break;

                            #endregion

                            #region 幽灵盾

                            case MagicType.MagicResistance:
                                Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                break;

                            #endregion

                            #region 集体隐身术

                            case MagicType.MassInvisibility:
                                Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                break;

                            #endregion

                            #region 隐魂术

                            case MagicType.MassTransparency:
                                Effects.Add(spell = new MirEffect(690, 10, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx3, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });

                                break;

                            #endregion

                            #region 月魂灵波

                            case MagicType.GreaterEvilSlayer:
                                Effects.Add(spell = new MirEffect(3360, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeStart);
                                break;

                            #endregion

                            #region 月明波

                            case MagicType.GreaterHolyStrike:
                                Effects.Add(spell = new MirEffect(840, 6, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx3, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.ImprovedHolyStrikeStart);
                                break;

                            #endregion

                            #region 神圣战甲术

                            case MagicType.Resilience:
                                Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                break;

                            #endregion

                            #region 困魔咒

                            case MagicType.TrapOctagon:
                                Effects.Add(spell = new MirEffect(630, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });

                                DXSoundManager.Play(SoundIndex.ShacklingTalismanStart);
                                break;

                            #endregion

                            #region 空拳刀法

                            case MagicType.TaoistCombatKick:
                                DXSoundManager.Play(SoundIndex.TaoistCombatKickStart);
                                break;

                            #endregion

                            #region 强震魔法

                            case MagicType.ElementalSuperiority:
                                Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                break;

                            #endregion

                            #region 召唤神兽
                            case MagicType.SummonShinsu:
                                Effects.Add(new MirEffect(2590, 19, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.PhantomColour)
                                {
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.SummonShinsuStart);
                                break;
                            #endregion

                            #region 群体治愈术

                            case MagicType.MassHeal:
                                Effects.Add(spell = new MirEffect(660, 10, TimeSpan.FromMilliseconds(60), LibraryFile.Magic, 10, 35, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.MassHealStart);
                                break;

                            #endregion

                            //Summon Jin Skeleton -> Summon Skeleton

                            #region 猛虎强势

                            case MagicType.BloodLust:
                                Effects.Add(spell = new MirEffect(2080, 6, TimeSpan.FromMilliseconds(80), LibraryFile.Magic, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                break;

                            #endregion

                            #region 回生术

                            case MagicType.Resurrection:
                                Effects.Add(spell = new MirEffect(310, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx, 60, 60, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.ResurrectionStart);
                                break;

                            #endregion

                            #region 云寂术

                            case MagicType.Purification:
                                Effects.Add(spell = new MirEffect(220, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx2, 20, 40, Globals.HolyColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.PurificationStart);
                                break;

                            #endregion

                            #region 移花接玉

                            case MagicType.StrengthOfFaith:
                                Effects.Add(spell = new MirEffect(360, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 20, 40, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.StrengthOfFaithStart);
                                break;

                            #endregion

                            #region 妙影无踪

                            case MagicType.Transparency:
                                Effects.Add(new MirEffect(430, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.InvisibilityEnd);
                                break;

                            #endregion

                            #region 阴阳法环

                            case MagicType.CelestialLight:
                                if (GameScene.Game.MapControl.MapInfo.CanPlayName != true || (GameScene.Game.MapControl.MapInfo.CanPlayName == true && this == User))
                                {
                                    Effects.Add(new MirEffect(280, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.HolyColour)
                                    {
                                        Blend = true,
                                        Target = this,
                                        DrawColour = Color.Yellow,
                                    });
                                }
                                DXSoundManager.Play(SoundIndex.MagicShieldStart);
                                break;

                            #endregion

                            #region 吸星大法

                            case MagicType.LifeSteal:
                                Effects.Add(new MirEffect(2410, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                });
                                DXSoundManager.Play(SoundIndex.HolyStrikeStart);
                                break;

                            #endregion

                            #region 灭魂火符

                            case MagicType.ImprovedExplosiveTalisman:
                                Effects.Add(spell = new MirEffect(980, 6, TimeSpan.FromMilliseconds(80), LibraryFile.MagicEx2, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });

                                DXSoundManager.Play(SoundIndex.ExplosiveTalismanStart);
                                break;

                            #endregion

                            //Greater Poison Dust -> Poison Dust

                            //Scarecrow -> Summon Skeleton

                            #region 横扫千军

                            case MagicType.ThunderKick:
                                DXSoundManager.Play(SoundIndex.TaoistCombatKickStart);
                                break;

                            #endregion

                            //Neutralize

                            #region 暗鬼阵

                            case MagicType.DarkSoulPrison:
                                Effects.Add(new MirEffect(600, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.DarkSoulPrison);
                                break;

                            #endregion

                            #endregion

                            #region 刺客

                            //Willow Dance

                            //Vine Tree Dance

                            //Discipline

                            //Poisonous Cloud

                            //Full Bloom

                            #region 潜行

                            case MagicType.Cloak:
                                Effects.Add(new MirEffect(600, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 10, 35, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                });
                                DXSoundManager.Play(SoundIndex.CloakStart);
                                break;

                            #endregion

                            //White Lotus

                            //Calamity Of Full Moon

                            #region 亡灵束缚

                            case MagicType.WraithGrip:
                                Effects.Add(spell = new MirEffect(1460, 15, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    BlendRate = 0.4f,
                                });
                                DXSoundManager.Play(SoundIndex.WraithGripStart);
                                break;

                            #endregion

                            #region 烈焰

                            case MagicType.HellFire:
                                Effects.Add(spell = new MirEffect(1520, 15, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 60, 60, Globals.FireColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.WraithGripStart);
                                break;

                            #endregion

                            //Pledge Of Blood

                            #region 血之盟约

                            case MagicType.Rake:
                                Effects.Add(spell = new MirEffect(1200, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.IceColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction,
                                    Skip = 10,
                                });
                                DXSoundManager.Play(SoundIndex.RakeStart);
                                break;

                            #endregion

                            //Sweet Brier

                            #region 亡灵替身

                            case MagicType.SummonPuppet:
                                Effects.Add(new MirEffect(800, 16, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 80, 50, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                });
                                DXSoundManager.Play(SoundIndex.SummonPuppet);
                                break;

                            #endregion

                            //Karma - Removed

                            //Touch Of Departed 

                            //Waning Moon

                            //Ghost Walk

                            //Elemental Puppet

                            //Rejuvenation

                            //Resolution

                            //Change Of Seasons

                            //Release

                            //Flame Splash

                            //Bloody Flower

                            #region 心机一转

                            case MagicType.TheNewBeginning:
                                Effects.Add(spell = new MirEffect(2300, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 60, 60, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    Direction = action.Direction
                                });
                                DXSoundManager.Play(SoundIndex.TheNewBeginning);
                                break;

                            #endregion

                            //Dance Of Swallows

                            //Dark Conversion

                            #region 狂涛泉涌

                            case MagicType.DragonRepulse:
                                Effects.Add(new MirEffect(1000, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 0, 0, Globals.NoneColour)
                                {
                                    MapTarget = CurrentLocation,
                                });
                                Effects.Add(new MirEffect(1020, 10, TimeSpan.FromMilliseconds(60), LibraryFile.MagicEx4, 80, 50, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                });
                                DXSoundManager.Play(SoundIndex.DragonRepulseStart);
                                break;

                            #endregion

                            //Advent Of Demon

                            //Advent Of Devil

                            #region 深渊苦海

                            case MagicType.Abyss:
                                Effects.Add(new MirEffect(2000, 14, TimeSpan.FromMilliseconds(70), LibraryFile.MagicEx4, 80, 50, Globals.PhantomColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                });
                                DXSoundManager.Play(SoundIndex.AbyssStart);
                                break;

                            #endregion

                            #region 日闪

                            case MagicType.FlashOfLight:
                                break;

                            #endregion

                            //Stealth

                            #region 风之闪避

                            case MagicType.Evasion:
                                Effects.Add(new MirEffect(2500, 12, TimeSpan.FromMilliseconds(70), LibraryFile.MagicEx4, 80, 50, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                });
                                DXSoundManager.Play(SoundIndex.EvasionStart);
                                break;

                            #endregion

                            #region 风之守护

                            case MagicType.RagingWind:
                                Effects.Add(new MirEffect(2600, 12, TimeSpan.FromMilliseconds(70), LibraryFile.MagicEx4, 80, 50, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                });
                                DXSoundManager.Play(SoundIndex.RagingWindStart);
                                break;

                            #endregion

                            #region 集中

                            case MagicType.Concentration:
                                Effects.Add(new MirEffect(300, 15, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 10, 35, Globals.DarkColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                DXSoundManager.Play(SoundIndex.Concentration);
                                break;

                            #endregion

                            #region 千刃杀风

                            case MagicType.ThousandBlades:
                                Effects.Add(spell = new MirEffect(590, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Target = this,
                                });
                                break;

                            #endregion

                            #endregion

                            #region 怪物烧焦地面

                            case MagicType.MonsterScortchedEarth:

                                location = CurrentLocation;

                                if (Config.DrawEffects && Race != ObjectType.Monster)
                                    foreach (Point point in MagicLocations)
                                    {
                                        Effects.Add(new MirEffect(220, 1, TimeSpan.FromMilliseconds(2500), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
                                        {
                                            MapTarget = point,
                                            StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, location) * 50),
                                            Opacity = 0.8F,
                                            DrawType = DrawType.Floor,
                                        });

                                        Effects.Add(new MirEffect(2450 + CEnvir.Random.Next(5) * 10, 10, TimeSpan.FromMilliseconds(250), LibraryFile.Magic, 0, 0, Globals.NoneColour)
                                        {
                                            Blend = true,
                                            MapTarget = point,
                                            StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, location) * 50),
                                            DrawType = DrawType.Floor,
                                        });

                                        Effects.Add(new MirEffect(1930, 30, TimeSpan.FromMilliseconds(50), LibraryFile.Magic, 20, 70, Globals.FireColour)
                                        {
                                            Blend = true,
                                            MapTarget = point,
                                            StartTime = CEnvir.Now.AddMilliseconds(Functions.Distance(point, location) * 50),
                                            BlendRate = 1F,
                                        });
                                    }

                                // if (MagicLocations.Count > 0)
                                //   DXSoundManager.Play(SoundIndex.LavaStrikeEnd);

                                break;
                            case MagicType.DoomClawRightPinch:

                                spell = new MirEffect(2640, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                };
                                spell.Process();

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(2680, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = Functions.Move(Functions.Move(CurrentLocation, MirDirection.Down, 0), MirDirection.Right, 5),
                                    };
                                    spell.Process();
                                };

                                break;
                            case MagicType.DoomClawLeftPinch:

                                spell = new MirEffect(2660, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                };
                                spell.Process();

                                spell.CompleteAction = () =>
                                {
                                    spell = new MirEffect(2680, 9, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                    {
                                        Blend = true,
                                        MapTarget = Functions.Move(CurrentLocation, MirDirection.Right, 5),
                                    };
                                    spell.Process();
                                };
                                break;
                            case MagicType.DoomClawRightSwipe:

                                spell = new MirEffect(2700, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                };
                                spell.Process();
                                break;
                            case MagicType.DoomClawLeftSwipe:

                                spell = new MirEffect(2720, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                {
                                    Blend = true,
                                    MapTarget = CurrentLocation,
                                };
                                spell.Process();
                                break;
                            case MagicType.DoomClawSpit:
                                foreach (Point point in MagicLocations)
                                {
                                    MirProjectile eff;
                                    Point p = new Point(point.X, point.Y - 10);
                                    Effects.Add(eff = new MirProjectile(2500, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour, p)
                                    {
                                        MapTarget = point,
                                        Skip = 0,
                                        Explode = true,
                                        Blend = true,
                                    });

                                    eff.CompleteAction = () =>
                                    {
                                        Effects.Add(new MirEffect(2520, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MonMagicEx19, 0, 0, Globals.NoneColour)
                                        {
                                            MapTarget = eff.MapTarget,
                                            Blend = true,
                                        });
                                    };

                                }
                                break;

                                #endregion

                        }
                    }
                    else//自定义技能--TODO
                    {
                        DiyMagicEffect MagicEffectinfo = Globals.DiyMagicEffectList.Binding.FirstOrDefault(x => x.MagicID == (int)action.Extra[0] && x.MagicType <= DiyMagicType.ExplosionMagic);
                        if (MagicEffectinfo != null)
                        {
                            ShowDiyMagicEffect(MagicEffectinfo, AttackElement, AttackTargets, MagicLocations);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 显示自定义技能动画特效
        /// </summary>
        /// <param name="MagicEffectinfo"></param>
        /// <param name="AttackElement"></param>
        /// <param name="AttackTargets"></param>
        /// <param name="MagicLocations"></param>
        public void ShowDiyMagicEffect(DiyMagicEffect MagicEffectinfo, Element AttackElement, List<MapObject> AttackTargets, List<Point> MagicLocations)
        {
            Color attackColour = Globals.NoneColour;
            MirEffect spell;
            switch (AttackElement)
            {
                case Element.Fire:
                    attackColour = Globals.FireColour;
                    break;
                case Element.Ice:
                    attackColour = Globals.IceColour;
                    break;
                case Element.Lightning:
                    attackColour = Globals.LightningColour;
                    break;
                case Element.Wind:
                    attackColour = Globals.WindColour;
                    break;
                case Element.Holy:
                    attackColour = Globals.HolyColour;
                    break;
                case Element.Dark:
                    attackColour = Globals.DarkColour;
                    break;
                case Element.Phantom:
                    attackColour = Globals.PhantomColour;
                    break;
            }
            switch (MagicEffectinfo.MagicType)
            {
                case DiyMagicType.Point:
                    foreach (MapObject target in AttackTargets)
                    {
                        if (MagicEffectinfo.magicDir > 0)
                        {
                            Effects.Add(new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                            {
                                Blend = true,
                                Target = this,
                                Direction = Direction,//8方向
                                DrawColour = attackColour
                            });
                        }
                        else
                        {
                            Effects.Add(new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                            {
                                Blend = true,
                                Target = target,
                                DrawColour = attackColour
                            });
                        }
                    }
                    DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    break;
                case DiyMagicType.Line://地面线魔法
                    foreach (Point point in MagicLocations)
                    {
                        Effects.Add(new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour)
                        {
                            Blend = true,
                            MapTarget = point,
                            StartTime = CEnvir.Now.AddMilliseconds(500 + Functions.Distance(point, CurrentLocation) * 50),
                            Opacity = 0.8F,
                            DrawType = DrawType.Floor,
                            DrawColour = attackColour
                        });
                    }
                    DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    break;
                case DiyMagicType.FlyDestinationExplosion:
                    foreach (Point point in MagicLocations)
                    {
                        Effects.Add(spell = new MirProjectile(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour, CurrentLocation)
                        {
                            Blend = true,
                            MapTarget = point,
                            Explode = true,
                            DrawColour = attackColour
                        });

                        spell.CompleteAction = () =>
                        {
                            DiyMagicEffect ExplosionMagicEffectinfo = Globals.DiyMagicEffectList.Binding.FirstOrDefault(x => x.MagicID == MagicEffectinfo.ExplosionMagicID && x.MagicType == DiyMagicType.ExplosionMagic);
                            if (ExplosionMagicEffectinfo != null)
                            {
                                spell = new MirEffect(ExplosionMagicEffectinfo.startIndex, ExplosionMagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(ExplosionMagicEffectinfo.frameDelay), ExplosionMagicEffectinfo.file, ExplosionMagicEffectinfo.startLight, ExplosionMagicEffectinfo.endLight, ExplosionMagicEffectinfo.lightColour)
                                {
                                    Blend = true,
                                    MapTarget = point,
                                    DrawColour = attackColour
                                };
                                spell.Process();

                                DXSoundManager.Play(ExplosionMagicEffectinfo.MagicSound);
                            }
                        };
                        spell.Process();
                        if (MagicLocations.Count > 0)
                            DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    }
                    break;
                case DiyMagicType.FiyHitTargetExplosion:
                    foreach (Point point in MagicLocations)
                    {
                        Effects.Add(spell = new MirProjectile(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour, CurrentLocation)
                        {
                            Blend = true,
                            MapTarget = point,
                            DrawColour = attackColour
                        });
                        spell.Process();
                    }

                    foreach (MapObject attackTarget in AttackTargets)
                    {
                        Effects.Add(spell = new MirProjectile(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour, CurrentLocation)
                        {
                            Blend = true,
                            Target = attackTarget,
                            DrawColour = attackColour
                        });

                        spell.CompleteAction = () =>
                        {
                            DiyMagicEffect ExplosionMagicEffectinfo = Globals.DiyMagicEffectList.Binding.FirstOrDefault(x => x.MagicID == MagicEffectinfo.ExplosionMagicID && x.MagicType == DiyMagicType.ExplosionMagic);
                            if (ExplosionMagicEffectinfo != null)
                            {
                                attackTarget.Effects.Add(spell = new MirEffect(ExplosionMagicEffectinfo.startIndex, ExplosionMagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(ExplosionMagicEffectinfo.frameDelay), ExplosionMagicEffectinfo.file, ExplosionMagicEffectinfo.startLight, ExplosionMagicEffectinfo.endLight, ExplosionMagicEffectinfo.lightColour)
                                {
                                    Blend = true,
                                    Target = attackTarget,
                                    DrawColour = attackColour
                                });
                                spell.Process();
                                DXSoundManager.Play(ExplosionMagicEffectinfo.MagicSound);
                            }
                        };
                        spell.Process();
                    }
                    if (MagicLocations.Count > 0 || AttackTargets.Count > 0)
                        DXSoundManager.Play(MagicEffectinfo.MagicSound);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 显示自定义技能动画特效
        /// </summary>
        /// <param name="MagicEffectinfo"></param>
        /// <param name="AttackElement"></param>
		public void ShowDiyMagicEffect(DiyMagicEffect MagicEffectinfo, Element AttackElement)
        {

            if (MagicEffectinfo.magicDir > 0)
            {
                Effects.Add(new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                {
                    Blend = true,
                    Target = this,
                    Direction = Direction,//8方向
                });
            }
            else
            {
                Effects.Add(new MirEffect(MagicEffectinfo.startIndex, MagicEffectinfo.frameCount, TimeSpan.FromMilliseconds(MagicEffectinfo.frameDelay), MagicEffectinfo.file, MagicEffectinfo.startLight, MagicEffectinfo.endLight, MagicEffectinfo.lightColour) //Element style?
                {
                    Blend = true,
                    Target = this,
                });
            }
        }
        /// <summary>
        /// 执行下一个操作
        /// </summary>
        public virtual void DoNextAction()
        {
            if (Race == ObjectType.Player)
            {
                if (this is PlayerObject { IsFishing: true })
                {
                    if (ActionQueue.Count == 0 || ActionQueue[0].Action != MirAction.FishingWait)
                    {
                        ActionQueue.Add(new ObjectAction(MirAction.FishingWait, this.Direction, this.CurrentLocation, MagicType.None));
                        //继续播放水波动画
                        //GameScene.Game.FishingStatusBox.ShowRipple();
                    }
                }
            }

            if (ActionQueue.Count == 0)
            {
                switch (CurrentAction)
                {
                    //Die, Attack,..
                    case MirAction.Die:
                    case MirAction.Dead:
                        ActionQueue.Add(new ObjectAction(MirAction.Dead, Direction, CurrentLocation));
                        break;
                    default:
                        ActionQueue.Add(new ObjectAction(MirAction.Standing, Direction, CurrentLocation));
                        break;
                }
            }

            switch (ActionQueue[0].Action)
            {
                case MirAction.Moving:
                // case MirAction.DashL:
                // case MirAction.DashR:
                case MirAction.Pushed:
                    if (!GameScene.Game.MoveFrame) return;
                    break;

            }
            SetAction(ActionQueue[0]);
            ActionQueue.RemoveAt(0);
        }
        /// <summary>
        /// 绘制框架改变
        /// </summary>
        public virtual void DrawFrameChanged()
        {
            GameScene.Game.MapControl.TextureValid = false;

        }
        /// <summary>
        /// 框架索引改变
        /// </summary>
        public virtual void FrameIndexChanged()
        {
            switch (CurrentAction)
            {
                case MirAction.Attack:
                    if (FrameIndex != 1) return;
                    PlayAttackSound();
                    break;
                case MirAction.RangeAttack:
                    if (FrameIndex != 4) return;
                    CreateProjectile();
                    PlayAttackSound();
                    break;
                /*  case MirAction.Struck:
                      if (FrameIndex == 0)
                          PlayStruckSound();
                      break;*/
                case MirAction.Die:
                    if (FrameIndex == 0)
                        PlayDieSound();
                    break;
            }
        }
        /// <summary>
        /// 创建投射物
        /// </summary>
        public virtual void CreateProjectile() { }
        /// <summary>
        /// 移动偏移已更改
        /// </summary>
        public virtual void MovingOffSetChanged()
        {
            GameScene.Game.MapControl.TextureValid = false;
        }
        /// <summary>
        /// 位置已更改
        /// </summary>
        public virtual void LocationChanged()
        {
            if (CurrentCell == null) return;

            CurrentCell.RemoveObject(this);

            if (CurrentLocation.X < GameScene.Game.MapControl.Width && CurrentLocation.Y < GameScene.Game.MapControl.Height)
                GameScene.Game.MapControl.Cells[CurrentLocation.X, CurrentLocation.Y].AddObject(this);

        }
        /// <summary>
        /// 死亡变化
        /// </summary>
        public virtual void DeadChanged()
        {
            //GameScene.Game.BigMapBox.Update(this);
            //GameScene.Game.MiniMapBox.Update(this);
        }
        /// <summary>
        /// 击中 攻击目标 元素
        /// </summary>
        /// <param name="attackerID"></param>
        /// <param name="element"></param>
        public void Struck(uint attackerID, Element element, uint AttackObImage = 0)
        {
            AttackerID = attackerID;

            PlayStruckSound();

            if (VisibleBuffs.Contains(BuffType.MagicShield) || VisibleBuffs.Contains(BuffType.SuperiorMagicShield))
                MagicShieldStruck();

            if (VisibleBuffs.Contains(BuffType.CelestialLight))
                CelestialLightStruck();

            switch (element)  //集中目标的元素变化
            {
                case Element.None:
                    Effects.Add(new MirEffect(930, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.NoneColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Fire:
                    Effects.Add(new MirEffect(790, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.FireColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Ice:
                    Effects.Add(new MirEffect(810, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.IceColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Lightning:
                    Effects.Add(new MirEffect(830, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.LightningColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Wind:
                    Effects.Add(new MirEffect(850, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.WindColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Holy:
                    Effects.Add(new MirEffect(870, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.HolyColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Dark:
                    Effects.Add(new MirEffect(890, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.DarkColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
                case Element.Phantom:
                    Effects.Add(new MirEffect(910, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx, 10, 30, Globals.PhantomColour)
                    {
                        Blend = true,
                        Target = this,
                    });
                    break;
            }

            DiyMagicEffect ActSpellMagic = Globals.DiyMagicEffectList.Binding.FirstOrDefault(p => p.MagicType == DiyMagicType.MagicAttackPlayer && p.MagicID == AttackObImage);
            if (ActSpellMagic != null)
            {
                ShowDiyMagicEffect(ActSpellMagic, Element.None);
            }
        }
        //身体环绕动画自定义
        public void DrawBodyEffect(int EffectId, DiyMagicType EffectTyep)
        {
            DiyMagicEffect Effectinfo = Globals.DiyMagicEffectList.Binding.FirstOrDefault(x => x.MagicID == EffectId && x.MagicType == EffectTyep);

            if (Effectinfo != null)
            {
                MirLibrary Effectlibrary;
                if (!CEnvir.LibraryList.TryGetValue(Effectinfo.file, out Effectlibrary)) return;
                switch (Effectinfo.magicDir)
                {
                    case MagicDir.None://无方向
                        Effectlibrary.DrawBlend(Effectinfo.startIndex + (GameScene.Game.MapControl.Animation * (Effectinfo.frameDelay / 100)) % (Effectinfo.frameCount/* + 1*/), DrawX, DrawY, Effectinfo.lightColour, true, 0.7f, ImageType.Image);
                        break;
                    case MagicDir.Dir8://随人物方向
                        Effectlibrary.DrawBlend(Effectinfo.startIndex + (GameScene.Game.MapControl.Animation * (Effectinfo.frameDelay / 100)) % (Effectinfo.frameCount /*+ 1*/) + (int)Direction * ((int)Math.Ceiling((decimal)Effectinfo.frameCount / 10)) * 10, DrawX, DrawY, Effectinfo.lightColour, true, 1f, ImageType.Image);
                        break;
                    case MagicDir.Dir16://随人物动作
                        Effectlibrary.DrawBlend(Effectinfo.startIndex + DrawFrame, DrawX, DrawY, Effectinfo.lightColour, true, 0.7f, ImageType.Image);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public virtual void Draw() { }
        /// <summary>
        /// 混合绘制
        /// </summary>
        public virtual void DrawBlend() { }
        /// <summary>
        /// 绘制尸体
        /// </summary>
        /// <param name="shadow"></param>
        public virtual void DrawBody(bool shadow) { }
        /// <summary>
        /// 聊天内容
        /// </summary>
        /// <param name="text"></param>
        public void Chat(string text)
        {
            const int chatWidth = 200;   //聊天显示文字的宽度

            Color colour = Dead ? Color.Gray : Color.White;   //如果死亡  文字灰色  否者 文字白色
            ChatLabel = ChatLabels.FirstOrDefault(x => x.Text == text && x.ForeColour == colour);

            ChatTime = CEnvir.Now.AddSeconds(5);

            if (ChatLabel != null) return;
            //每行显示25个字符

            //int start = 25;
            //int cnt = text.Length / start;
            //for (int i = 0; i < cnt; i++)
            //{
            //    text = text.Insert(start, "\r\n");
            //    start += 25 + 2;
            //}

            ChatLabel = new DXLabel
            {
                AutoSize = false,
                Outline = true,
                OutlineColour = Color.Black,
                ForeColour = colour,
                Text = text,
                IsVisible = true,
                DrawFormat = TextFormatFlags.TextBoxControl | TextFormatFlags.WordBreak |
                              TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding,
                UI_Offset_X = 0,
            };
            ChatLabel.Size = DXLabel.GetHeight(ChatLabel, chatWidth);
            ChatLabel.Disposing += (o, e) => ChatLabels.Remove(ChatLabel);
            ChatLabels.Add(ChatLabel);

        }
        /// <summary>
        /// 名字标签可见改变
        /// </summary>
        public virtual void NameLableVisableChanged()
        {
            if (string.IsNullOrEmpty(Name))
            {
                NameLabel = null;
            }
            else
            {
                if (NameLabels.TryGetValue(Name, out List<DXLabel> names))
                {
                    NameLabel = names.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == Color.Empty);
                    if (NameLabel != null)
                    {
                        NameLabel.Visible = NameLableVisable;
                    }
                }
            }
        }
        /// <summary>
        /// 名字变化
        /// </summary>
        public virtual void NameChanged()
        {
            if (string.IsNullOrEmpty(Name))  //名字
            {
                NameLabel = null;
            }
            else
            {
                if (!NameLabels.TryGetValue(Name, out List<DXLabel> names))
                    NameLabels[Name] = names = new List<DXLabel>();

                NameLabel = names.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == Color.Empty);

                if (NameLabel == null)
                {
                    NameLabel = new DXLabel
                    {
                        BackColour = Color.Empty,
                        ForeColour = NameColour,
                        Outline = true,
                        OutlineColour = Color.Black,
                        Text = Name,
                        IsControl = false,
                        IsVisible = true,
                        DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.HorizontalCenter, //名字居中显示
                        UI_Offset_X = 0,
                    };

                    NameLabel.Disposing += (o, e) => names.Remove(NameLabel);
                    names.Add(NameLabel);
                }
            }

            #region 行会称号

            if (string.IsNullOrEmpty(Title))   //标题
            {
                TitleNameLabel = null;
            }
            else
            {
                string title = Title == Globals.StarterGuildName ? Title.Lang() : Title;

                if (Race == ObjectType.Player)
                {
                    //城主/沙巴克
                    foreach (KeyValuePair<CastleInfo, string> pair in GameScene.Game.CastleOwners)
                    {
                        if (pair.Value != Title) continue;

                        title += $" ({pair.Key.Name})";
                    }
                }

                if (!NameLabels.TryGetValue(title, out List<DXLabel> titles))
                    NameLabels[title] = titles = new List<DXLabel>();
                //称号标签
                TitleNameLabel = titles.FirstOrDefault(x => x.ForeColour == NameColour && x.BackColour == Color.Empty);

                if (TitleNameLabel == null)
                {
                    TitleNameLabel = new DXLabel
                    {
                        BackColour = Color.Empty,
                        ForeColour = Race != ObjectType.Player ? Color.Orange : NameColour,
                        Outline = true,
                        OutlineColour = Color.Black,
                        Text = title,
                        IsControl = false,
                        IsVisible = true,
                        UI_Offset_X = 0,
                    };

                    TitleNameLabel.Disposing += (o, e) => titles.Remove(TitleNameLabel);
                    titles.Add(TitleNameLabel);
                }
            }

            #endregion

            #region 成就称号

            if (Race == ObjectType.Player)
            {
                PlayerObject player = (PlayerObject)this;

                if (string.IsNullOrEmpty(player.AchievementTitle))
                {
                    AchievementTitleLabel = null;
                }
                else
                {
                    if (!NameLabels.TryGetValue(player.AchievementTitle, out List<DXLabel> achievementTitles))
                        NameLabels[player.AchievementTitle] = achievementTitles = new List<DXLabel>();
                    //成就称号标签
                    AchievementTitleLabel = achievementTitles.FirstOrDefault(x => x.BackColour == Color.Empty && x.Text == "『" + player.AchievementTitle + "』");
                    if (AchievementTitleLabel == null)
                    {
                        AchievementTitleLabel = new DXLabel
                        {
                            Outline = true,
                            OutlineColour = Color.Gray,
                            ForeColour = player.AchievementTitleColour,
                            BackColour = Color.Empty,
                            Text = "『" + player.AchievementTitle + "』",
                            IsControl = false,
                            IsVisible = true,
                            UI_Offset_X = 0,
                        };

                        AchievementTitleLabel.Disposing += (o, e) => achievementTitles.Remove(AchievementTitleLabel);
                        achievementTitles.Add(AchievementTitleLabel);
                    }
                }
            }

            #endregion

        }
        /// <summary>
        /// 绘制名字
        /// </summary>
        public virtual void DrawName()
        {
            // 名字标签不空  在竞技场  对象是玩家   显示竞技场
            if (NameLabel != null && GameScene.Game.MapControl.MapInfo.CanPlayName == true && Race == ObjectType.Player)
            {
                NameLabel.Text = $"竞技场勇士";
            }
            else
            {
                // 名字标签不空 不在竞技场 显示普通名字
                if (NameLabel != null && GameScene.Game.MapControl.MapInfo.CanPlayName != true)
                    NameLabel.Text = Name;
                else
                    if (NameLabel != null && Race == ObjectType.Player)
                    NameLabel.Text = $"竞技场勇士";
            }

            if (NameLabel != null)   //名字标签
            {
                int x = DrawX + (48 - NameLabel.Size.Width) / 2;
                int y = DrawY - (32 - NameLabel.Size.Height) / 2;    //名字坐标

                if (Race == ObjectType.Monster && NameLabel.Text.Contains("\r\n"))   //如果是怪物，并且怪物有加-符号的，那么怪物名字坐标上调
                    y -= 18;
                else if (Race == ObjectType.Player)
                    y -= 13;

                if (Race == ObjectType.NPC && NameLabel.Text.Contains("\r\n"))  //如果是NPC，并区NPC有加-符号，那么NPC名字坐标上调
                    y -= 15;
                else if (Race == ObjectType.NPC)
                    y -= 10;

                if (Dead)       //如果死亡
                    y += 21;
                else
                    y -= 6;

                if (Race != ObjectType.Player && TitleNameLabel != null)
                    y -= 13;

                NameLabel.Location = new Point(x, y);
                NameLabel.Draw();
            }
            //绘制行会名字
            if (TitleNameLabel != null && GameScene.Game.MapControl.MapInfo.CanPlayName != true)
            {
                int x = DrawX + (48 - TitleNameLabel.Size.Width) / 2;
                //int y = DrawY - (32 - TitleNameLabel.Size.Height) / 2;

                //if (Dead)
                //    y += 21;
                //else
                //    y -= 6;
                int y = NameLabel.Location.Y + NameLabel.Size.Height;

                TitleNameLabel.Location = new Point(x, y);
                TitleNameLabel.Draw();
            }
            //成就称号标签
            if (AchievementTitleLabel != null)
            {
                int x = DrawX + (48 - AchievementTitleLabel.Size.Width) / 2;
                int y = DrawY - (80 - AchievementTitleLabel.Size.Height) / 2;

                if (Dead)
                    y += 21;
                else
                    y -= 6;

                AchievementTitleLabel.Location = new Point(x, y);
                AchievementTitleLabel.Draw();
            }

        }
        /// <summary>
        /// 绘制伤害
        /// </summary>
        public virtual void DrawDamage()
        {

            foreach (DamageInfo damageInfo in DamageList)
                damageInfo.Draw(DrawX, DrawY);
        }
        /// <summary>
        /// 绘制聊天信息
        /// </summary>
        public void DrawChat()
        {
            if (ChatLabel == null || ChatLabel.IsDisposed) return;

            if (CEnvir.Now > ChatTime) return;

            int x = DrawX + (48 - ChatLabel.Size.Width) / 2;
            int y = DrawY - (76 + ChatLabel.Size.Height);    //聊天文字显示高度

            //绘制血条或者绘制数字显血
            if (this is PlayerObject && !BigPatchConfig.ChkShowHPBar && !BigPatchConfig.ShowHealth)
            {
                y = DrawY - (76 + ChatLabel.Size.Height) + 20;
            }
            //管理员标识 或者 排行榜标识
            if (this == User && (User.VisibleBuffs.Contains(BuffType.Developer) || User.VisibleBuffs.Contains(BuffType.Ranking)))
            {
                y = DrawY - (76 + ChatLabel.Size.Height) - 20;
            }
            //护身冰环
            if (this == User && User.VisibleBuffs.Contains(BuffType.FrostBite))
            {
                y = DrawY - (76 + ChatLabel.Size.Height) - 10;
            }
            //死亡时
            if (Dead)
            {
                y = DrawY - (76 + ChatLabel.Size.Height) + 25;
            }

            ChatLabel.ForeColour = Dead ? Color.Gray : Color.White;
            ChatLabel.Location = new Point(x, y);
            ChatLabel.Draw();
        }
        /// <summary>
        /// 播放攻击的音效
        /// </summary>
        public virtual void PlayAttackSound() { }
        /// <summary>
        /// 播放击打的音效
        /// </summary>
        public virtual void PlayStruckSound() { }
        /// <summary>
        /// 播放死亡的音效
        /// </summary>
        public virtual void PlayDieSound() { }
        /// <summary>
        /// 绘制毒
        /// </summary>
        public void DrawPoison()
        {
            //if (this is SpellObject || Dead) return;
            if (Dead) return;

            int count = 0;

            if ((Poison & PoisonType.Paralysis) == PoisonType.Paralysis)  //麻痹
            {
                DXManager.Sprite.Draw(DXManager.PoisonTexture, Vector2.Zero, new Vector2(DrawX + count * 5, DrawY - 50), Color.DimGray);
                count++;
            }
            if ((Poison & PoisonType.Slow) == PoisonType.Slow)    //减速
            {
                DXManager.Sprite.Draw(DXManager.PoisonTexture, Vector2.Zero, new Vector2(DrawX + count * 5, DrawY - 50), Color.CornflowerBlue);
                count++;
            }

            if ((Poison & PoisonType.Red) == PoisonType.Red)    //红毒
            {
                DXManager.Sprite.Draw(DXManager.PoisonTexture, Vector2.Zero, new Vector2(DrawX + count * 5, DrawY - 50), Color.IndianRed);
                count++;
            }

            if ((Poison & PoisonType.Green) == PoisonType.Green)   //绿毒
                DXManager.Sprite.Draw(DXManager.PoisonTexture, Vector2.Zero, new Vector2(DrawX + count * 5, DrawY - 50), Color.SeaGreen);
        }
        /// <summary>
        /// 绘制健康状态
        /// </summary>
        public virtual void DrawHealth() { }
        /// <summary>
        /// 鼠标悬停坐标
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public abstract bool MouseOver(Point p);

        public abstract void OnRemoved();

        /// <summary>
        /// 更新任务
        /// </summary>
        public virtual void UpdateQuests() { }

        /// <summary>
        /// 千刃杀风开始
        /// </summary>
        public void ThousandBladesCreate()
        {
            ThousandBladesEffect = new MirEffect(600, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 40, 40, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                BlendRate = 0.4f,
            };
        }
        /// <summary>
        /// 千刃杀风结束
        /// </summary>
        public void ThousandBladesEnd()
        {
            ThousandBladesEffect?.Remove();
            ThousandBladesEffect = null;
        }

        /// <summary>
        /// 亡灵束缚开始
        /// </summary>
        public void WraithGripCreate()
        {
            WraithGripEffect = new MirEffect(1424, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 40, 40, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                BlendRate = 0.4f,
            };
            WraithGripEffect2 = new MirEffect(1444, 10, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 40, 40, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                BlendRate = 0.4f,
            };
        }
        /// <summary>
        /// 亡灵束缚结束
        /// </summary>
        public void WraithGripEnd()
        {
            WraithGripEffect?.Remove();
            WraithGripEffect = null;
            WraithGripEffect2?.Remove();
            WraithGripEffect2 = null;
        }
        /// <summary>
        /// 魔法盾开始
        /// </summary>
        public void MagicShieldCreate()
        {
            MagicShieldEffect = new MirEffect(850, 3, TimeSpan.FromMilliseconds(200), LibraryFile.Magic, 40, 40, Globals.WindColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
            MagicShieldEffect.Process();
        }
        /// <summary>
        /// 魔法盾被攻击
        /// </summary>
        public void MagicShieldStruck()
        {
            MagicShieldEnd();

            MagicShieldEffect = new MirEffect(853, 3, TimeSpan.FromMilliseconds(100), LibraryFile.Magic, 40, 40, Globals.WindColour)
            {
                Blend = true,
                Target = this,
                CompleteAction = MagicShieldCreate,   //完成操作
            };
            MagicShieldEffect.Process();
        }
        /// <summary>
        /// 魔法盾结束
        /// </summary>
        public void MagicShieldEnd()
        {
            MagicShieldEffect?.Remove();
            MagicShieldEffect = null;
        }
        /// <summary>
        /// 护身法盾开始
        /// </summary>
        public void SuperiorMagicShieldCreate()
        {
            SuperiorMagicShieldEffect = new MirEffect(570, 3, TimeSpan.FromMilliseconds(200), LibraryFile.MagicEx3, 40, 40, Globals.WindColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
            SuperiorMagicShieldEffect.Process();
        }
        /// <summary>
        /// 护身法盾被攻击
        /// </summary>
        public void SuperiorMagicShieldStruck()
        {
            SuperiorMagicShieldEnd();

            SuperiorMagicShieldEffect = new MirEffect(573, 3, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx3, 40, 40, Globals.WindColour)
            {
                Blend = true,
                Target = this,
                CompleteAction = SuperiorMagicShieldCreate,  //完成操作
            };
            SuperiorMagicShieldEffect.Process();
        }
        /// <summary>
        /// 护身法盾结束
        /// </summary>
        public void SuperiorMagicShieldEnd()
        {
            SuperiorMagicShieldEffect?.Remove();
            SuperiorMagicShieldEffect = null;
        }
        /// <summary>
        /// 狂暴冲撞开始
        /// </summary>
        public void AssaultCreate()
        {
            AssaultEffect = new MirEffect(740, 3, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 40, 40, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                Direction = Direction,
            };
            AssaultEffect.Process();
        }
        /// <summary>
        /// 狂暴冲撞结束
        /// </summary>
        public void AssaultEnd()
        {
            AssaultEffect?.Remove();
            AssaultEffect = null;
        }
        /// <summary>
        /// 君临步开始
        /// </summary>
        public void ReigningStepCreate()
        {
            ReigningStepEffect = new MirEffect(740, 3, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 40, 40, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                Direction = Direction,
            };
            ReigningStepEffect.Process();
        }
        /// <summary>
        /// 君临步结束
        /// </summary>
        public void ReigningStepEnd()
        {
            ReigningStepEffect?.Remove();
            ReigningStepEffect = null;
        }
        /// <summary>
        /// 阴阳法环开始
        /// </summary>
        public void CelestialLightCreate()
        {
            CelestialLightEffect = new MirEffect(300, 3, TimeSpan.FromMilliseconds(200), LibraryFile.MagicEx2, 40, 40, Globals.HolyColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
            CelestialLightEffect.Process();
        }
        /// <summary>
        /// 阴阳法环被攻击
        /// </summary>
        public void CelestialLightStruck()
        {
            CelestialLightEnd();

            CelestialLightEffect = new MirEffect(303, 3, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx2, 40, 40, Globals.HolyColour)
            {
                Blend = true,
                Target = this,
                CompleteAction = CelestialLightCreate,
            };
            CelestialLightEffect.Process();

        }
        /// <summary>
        /// 阴阳法环结束
        /// </summary>
        public void CelestialLightEnd()
        {
            CelestialLightEffect?.Remove();
            CelestialLightEffect = null;
        }
        /// <summary>
        /// 吸星大法开始
        /// </summary>
        public void LifeStealCreate()
        {
            LifeStealEffect = new MirEffect(1260, 6, TimeSpan.FromMilliseconds(150), LibraryFile.MagicEx2, 40, 40, Globals.DarkColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
        }
        /// <summary>
        /// 吸星大法结束
        /// </summary>
        public void LifeStealEnd()
        {
            LifeStealEffect?.Remove();
            LifeStealEffect = null;
        }
        /// <summary>
        /// 护身冰环开始
        /// </summary>
        public void FrostBiteCreate()
        {
            FrostBiteEffect = new MirEffect(600, 7, TimeSpan.FromMilliseconds(150), LibraryFile.MagicEx5, 40, 40, Globals.IceColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
        }
        /// <summary>
        /// 护身冰环结束
        /// </summary>
        public void FrostBiteEnd()
        {
            FrostBiteEffect?.Remove();
            FrostBiteEffect = null;
        }
        /// <summary>
        /// 沉默开始
        /// </summary>
        public void SilenceCreate()
        {
            SilenceEffect = new MirEffect(680, 6, TimeSpan.FromMilliseconds(150), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
        }
        /// <summary>
        /// 沉默结束
        /// </summary>
        public void SilenceEnd()
        {
            SilenceEffect?.Remove();
            SilenceEffect = null;
        }
        /// <summary>
        /// 晕击开始
        /// </summary>
        public void StunnedStrikeCreate()
        {
            StunnedStrikeEffect = new MirEffect(680, 6, TimeSpan.FromMilliseconds(150), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
        }
        /// <summary>
        /// 晕击结束
        /// </summary>
        public void StunnedStrikeEnd()
        {
            StunnedStrikeEffect?.Remove();
            StunnedStrikeEffect = null;
        }
        /// <summary>
        /// 深渊开始
        /// </summary>
        public void BlindCreate()
        {
            BlindEffect = new MirEffect(680, 6, TimeSpan.FromMilliseconds(150), LibraryFile.ProgUse, 0, 0, Globals.NoneColour)
            {
                //Blend = true,
                Target = this,
                Loop = true,
                DrawColour = Color.Black,
                Opacity = 0.8F
            };

            if (this != User) return;

            AbyssEffect = new MirEffect(2100, 19, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                AdditionalOffSet = new Point(0, -64)
            };
        }
        /// <summary>
        /// 深渊结束
        /// </summary>
        public void BlindEnd()
        {
            BlindEffect?.Remove();
            BlindEffect = null;
            AbyssEffect?.Remove();
            AbyssEffect = null;
        }
        /// <summary>
        /// 传染开始
        /// </summary>
        public void InfectionCreate()
        {
            InfectionEffect = new MirEffect(900, 7, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx5, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                //DrawColour = Color.SaddleBrown,
                Opacity = 0.8F
            };
        }
        /// <summary>
        /// 传染结束
        /// </summary>
        public void InfectionEnd()
        {
            InfectionEffect?.Remove();
            InfectionEffect = null;
        }

        public void NeutralizeCreate()
        {
            NeutralizeEffect = new MirEffect(470, 6, TimeSpan.FromMilliseconds(120), LibraryFile.MagicEx7, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
                Opacity = 0.8F
            };
        }
        public void NeutralizeEnd()
        {
            NeutralizeEffect?.Remove();
            NeutralizeEffect = null;
        }

        public void ElectricShockCreate()
        {
            ElectricShockEffect = new MirEffect(2971, 10, TimeSpan.FromMilliseconds(120), LibraryFile.Magic, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
        }
        public void ElectricShockEnd()
        {
            ElectricShockEffect?.Remove();
            ElectricShockEffect = null;
        }

        public void BurnCreate()
        {
            BurnEffect = new MirEffect(5012, 10, TimeSpan.FromMilliseconds(120), LibraryFile.MagicEx5, 0, 0, Globals.NoneColour)
            {
                Blend = true,
                Target = this,
                Loop = true,
            };
        }
        public void BurnEnd()
        {
            BurnEffect?.Remove();
            BurnEffect = null;
        }

        /// <summary>
        /// 狂涛泉涌开始
        /// </summary>
        public void DragonRepulseCreate()
        {
            if (VisibleBuffs.Contains(BuffType.DragonRepulse))
            {
                //MirEffect(索引，帧数，时间，库文件，开始光效，结束光效，光效颜色)
                DragonRepulseEffect = new MirEffect(1011, 4, TimeSpan.FromMilliseconds(150), LibraryFile.MagicEx4, 0, 0, Globals.NoneColour)
                {
                    Target = this,
                    Loop = true,
                };
                DragonRepulseEffect1 = new MirEffect(1031, 4, TimeSpan.FromMilliseconds(150), LibraryFile.MagicEx4, 80, 80, Globals.LightningColour)
                {
                    Blend = true,
                    Target = this,
                    Loop = true,
                };
                Random ra = new Random();
                Task.Factory.StartNew(() =>
                {

                    for (int i = 0; i < 12; i++)
                    {
                        DragonRepulseAddPoints(ra.Next(-3, 4), ra.Next(-3, 4), ra.Next(200, 500));

                        Thread.Sleep(ra.Next(150, 500));
                    }
                });
            }
            else if (VisibleBuffs.Contains(BuffType.ElementalHurricane))
            {
                if (Config.DrawEffects && Race != ObjectType.Monster)
                {
                    Color attackColour = Globals.NoneColour;
                    switch (AttackElement)
                    {
                        case Element.Fire:
                            attackColour = Globals.FireColour;
                            break;
                        case Element.Ice:
                            attackColour = Globals.IceColour;
                            break;
                        case Element.Lightning:
                            attackColour = Globals.LightningColour;
                            break;
                        case Element.Wind:
                            attackColour = Globals.WindColour;
                            break;
                        case Element.Holy:
                            attackColour = Globals.HolyColour;
                            break;
                        case Element.Dark:
                            attackColour = Globals.DarkColour;
                            break;
                        case Element.Phantom:
                            attackColour = Globals.PhantomColour;
                            break;
                    }

                    DragonRepulseEffect = new MirEffect(370, 4, TimeSpan.FromMilliseconds(140), LibraryFile.MagicEx3, 0, 0, Globals.LightningColour)
                    {
                        Blend = true,
                        Target = this,
                        Direction = Direction,
                        DrawColour = attackColour,
                        Loop = true,
                    };
                    DragonRepulseEffect.FrameIndexAction = () =>
                    {
                        if (DragonRepulseEffect.FrameIndex == 0)
                            DXSoundManager.Play(SoundIndex.ElementalHurricane);
                    };
                    DragonRepulseEffect.Process();
                }
            }
        }
        /// <summary>
        /// 狂涛泉涌增加点坐标
        /// </summary>
        /// <param name="deviationX"></param>
        /// <param name="deviationY"></param>
        /// <param name="delay"></param>
        private void DragonRepulseAddPoints(int deviationX, int deviationY, int delay)
        {
            DragonRepulseEffects.Add(new MirEffect(1050, 7, TimeSpan.FromMilliseconds(delay), LibraryFile.MagicEx4, 0, 0, Globals.NoneColour)
            {
                //Blend = true,  //混合 开
                MapTarget = new Point(CurrentLocation.X + deviationX, CurrentLocation.Y + deviationY), //目标  坐标
                //Loop = true,  //循环  开             
            });
            DragonRepulseEffects.Add(new MirEffect(1040, 7, TimeSpan.FromMilliseconds(delay), LibraryFile.MagicEx4, 80, 80, Globals.LightningColour)
            {
                Blend = true,  //混合 开
                MapTarget = new Point(CurrentLocation.X + deviationX, CurrentLocation.Y + deviationY),  //目标  坐标
                //Loop = true,  //循环  开
                BlendRate = 0.6F,
            });
            DragonRepulseEffects.Add(new MirEffect(1060, 7, TimeSpan.FromMilliseconds(delay), LibraryFile.MagicEx4, 80, 80, Globals.LightningColour)
            {
                Blend = true,  //混合 开
                MapTarget = new Point(CurrentLocation.X + deviationX, CurrentLocation.Y + deviationY),  //目标  坐标
                //Loop = true,  //循环  开
                BlendRate = 0.6F,
            });

        }
        /// <summary>
        /// 狂涛泉涌结束
        /// </summary>
        public void DragonRepulseEnd()
        {
            DragonRepulseEffect?.Remove();
            DragonRepulseEffect = null;
            DragonRepulseEffect1?.Remove();
            DragonRepulseEffect1 = null;
            for (int i = DragonRepulseEffects.Count - 1; i >= 0; i--)
            {
                MirEffect effect = DragonRepulseEffects[i];
                effect.Remove();
            }
        }
        /// <summary>
        /// 排行榜创建
        /// </summary>
        public void RankingCreate()
        {
            RankingEffect = new MirEffect(3420, 7, TimeSpan.FromMilliseconds(150), LibraryFile.GameInter, 0, 0, Globals.NoneColour)  //排行榜人物称号特效
            {
                Blend = true,
                Target = this,
                Loop = true,
                AdditionalOffSet = new Point(0, -35)  //称号坐标
            };
            RankingEffect.Process();
        }
        /// <summary>
        /// 排行榜结束
        /// </summary>
        public void RankingEnd()
        {
            RankingEffect?.Remove();
            RankingEffect = null;
        }
        /// <summary>
        /// 管理员创建
        /// </summary>
        public void DeveloperCreate()
        {
            DeveloperEffect = new MirEffect(3400, 7, TimeSpan.FromMilliseconds(150), LibraryFile.GameInter, 0, 0, Globals.NoneColour)  //GM人物称号特效
            {
                Blend = true,
                Target = this,
                Loop = true,
                AdditionalOffSet = new Point(10, -35)  //称号坐标
            };
            DeveloperEffect.Process();
        }
        /// <summary>
        /// 管理员结束
        /// </summary>
        public void DeveloperEnd()
        {
            DeveloperEffect?.Remove();
            DeveloperEffect = null;
        }
        /// <summary>
        /// 头顶花翎创建
        /// </summary>
        public void HeadTopCreate(int index, int fps, int offSetX, int offSetY)
        {
            HeadTopEffect = new MirEffect(index, fps, TimeSpan.FromMilliseconds(150), LibraryFile.Title, 0, 0, Globals.NoneColour)  //GM人物称号特效
            {
                //Blend = true,  //注意32位图不要blend绘制
                Target = this,
                Loop = true,
                AdditionalOffSet = new Point(offSetX, offSetY)  //称号坐标
            };
            HeadTopEffect.Process();
        }
        /// <summary>
        /// 头顶花翎结束
        /// </summary>
        public void HeadTopEnd()
        {
            HeadTopEffect?.Remove();
            HeadTopEffect = null;
        }
        /// <summary>
        /// 删除
        /// </summary>
        public virtual void Remove()
        {
            GameScene.Game.MapControl.RemoveObject(this);

            MagicShieldEnd();
            SuperiorMagicShieldEnd();
            CelestialLightEnd();
            WraithGripEnd();
            ThousandBladesEnd();
            LifeStealEnd();
            SilenceEnd();
            StunnedStrikeEnd();
            BlindEnd();
            DragonRepulseEnd();
            RankingEnd();
            DeveloperEnd();
            HeadTopEnd();
            AssaultEnd();
            ReigningStepEnd();
            FrostBiteEnd();
            InfectionEnd();
            ElectricShockEnd();
            BurnEnd();

            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                MirEffect effect = Effects[i];
                effect.Remove();
            }
        }

        public virtual void RemoveEffects()
        {
            for (int i = Effects.Count - 1; i >= 0; i--)
            {
                MirEffect effect = Effects[i];
                effect.Remove();
            }
            Effects.Clear();
        }
    }
}
