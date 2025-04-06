using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Client.Scenes.Configs;
using Library;
using Library.SystemModels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Color = System.Drawing.Color;
using Frame = Library.Frame;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;
using S = Library.Network.ServerPackets;

namespace Client.Models
{
    /// <summary>
    /// 玩家对象
    /// </summary>
    public class PlayerObject : MapObject
    {
        /// <summary>
        /// 对象类型 玩家
        /// </summary>
        public override ObjectType Race => ObjectType.Player;

        public const int FemaleOffSet = 5000, AssassinOffSet = 50000, RightHandOffSet = 50;

        #region Weaopon Librarys  武器资源库
        /// <summary>
        /// 武器资源库
        /// </summary>
        public Dictionary<int, LibraryFile> WeaponList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Weapon1,
            [1] = LibraryFile.M_Weapon2,
            [2] = LibraryFile.M_Weapon3,
            [3] = LibraryFile.M_Weapon4,
            [4] = LibraryFile.M_Weapon5,
            [5] = LibraryFile.M_Weapon6,
            [6] = LibraryFile.M_Weapon7,
            [10] = LibraryFile.M_Weapon10,
            [11] = LibraryFile.M_Weapon11,
            [12] = LibraryFile.M_Weapon12,
            [13] = LibraryFile.M_Weapon13,
            [14] = LibraryFile.M_Weapon14,
            [15] = LibraryFile.M_Weapon15,
            [16] = LibraryFile.M_Weapon16,

            [200] = LibraryFile.传奇2武器外观,

            [0 + FemaleOffSet] = LibraryFile.WM_Weapon1,
            [1 + FemaleOffSet] = LibraryFile.WM_Weapon2,
            [2 + FemaleOffSet] = LibraryFile.WM_Weapon3,
            [3 + FemaleOffSet] = LibraryFile.WM_Weapon4,
            [4 + FemaleOffSet] = LibraryFile.WM_Weapon5,
            [5 + FemaleOffSet] = LibraryFile.WM_Weapon6,
            [6 + FemaleOffSet] = LibraryFile.WM_Weapon7,
            [10 + FemaleOffSet] = LibraryFile.WM_Weapon10,
            [11 + FemaleOffSet] = LibraryFile.WM_Weapon11,
            [12 + FemaleOffSet] = LibraryFile.WM_Weapon12,
            [13 + FemaleOffSet] = LibraryFile.WM_Weapon13,
            [14 + FemaleOffSet] = LibraryFile.WM_Weapon14,
            [15 + FemaleOffSet] = LibraryFile.WM_Weapon15,
            [16 + FemaleOffSet] = LibraryFile.WM_Weapon16,

            [120] = LibraryFile.M_WeaponADL1,
            [122] = LibraryFile.M_WeaponADL2,
            [126] = LibraryFile.M_WeaponADL6,
            [120 + RightHandOffSet] = LibraryFile.M_WeaponADR1,
            [122 + RightHandOffSet] = LibraryFile.M_WeaponADR2,
            [126 + RightHandOffSet] = LibraryFile.M_WeaponADR6,

            [110] = LibraryFile.M_WeaponAOH1,
            [111] = LibraryFile.M_WeaponAOH2,
            [112] = LibraryFile.M_WeaponAOH3,
            [113] = LibraryFile.M_WeaponAOH3,
            [114] = LibraryFile.M_WeaponAOH4,
            [115] = LibraryFile.M_WeaponAOH5,
            [116] = LibraryFile.M_WeaponAOH6,

            [120 + FemaleOffSet] = LibraryFile.WM_WeaponADL1,
            [122 + FemaleOffSet] = LibraryFile.WM_WeaponADL2,
            [126 + FemaleOffSet] = LibraryFile.WM_WeaponADL6,
            [120 + FemaleOffSet + RightHandOffSet] = LibraryFile.WM_WeaponADR1,
            [122 + FemaleOffSet + RightHandOffSet] = LibraryFile.WM_WeaponADR2,
            [126 + FemaleOffSet + RightHandOffSet] = LibraryFile.WM_WeaponADR6,

            [110 + FemaleOffSet] = LibraryFile.WM_WeaponAOH1,
            [111 + FemaleOffSet] = LibraryFile.WM_WeaponAOH2,
            [112 + FemaleOffSet] = LibraryFile.WM_WeaponAOH3,
            [113 + FemaleOffSet] = LibraryFile.WM_WeaponAOH3,
            [114 + FemaleOffSet] = LibraryFile.WM_WeaponAOH4,
            [115 + FemaleOffSet] = LibraryFile.WM_WeaponAOH5,
            [116 + FemaleOffSet] = LibraryFile.WM_WeaponAOH6,
        };
        #endregion

        #region Helmet Librarys  头盔资源库
        /// <summary>
        /// 头盔资源库
        /// </summary>
        public Dictionary<int, LibraryFile> HelmetList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Helmet1,
            [1] = LibraryFile.M_Helmet2,
            [2] = LibraryFile.M_Helmet3,
            [3] = LibraryFile.M_Helmet4,
            [4] = LibraryFile.M_Helmet5,

            [10] = LibraryFile.M_Helmet11,
            [11] = LibraryFile.M_Helmet12,
            [12] = LibraryFile.M_Helmet13,
            [13] = LibraryFile.M_Helmet14,

            [0 + FemaleOffSet] = LibraryFile.WM_Helmet1,
            [1 + FemaleOffSet] = LibraryFile.WM_Helmet2,
            [2 + FemaleOffSet] = LibraryFile.WM_Helmet3,
            [3 + FemaleOffSet] = LibraryFile.WM_Helmet4,
            [4 + FemaleOffSet] = LibraryFile.WM_Helmet5,

            [10 + FemaleOffSet] = LibraryFile.WM_Helmet11,
            [11 + FemaleOffSet] = LibraryFile.WM_Helmet12,
            [12 + FemaleOffSet] = LibraryFile.WM_Helmet13,
            [13 + FemaleOffSet] = LibraryFile.WM_Helmet14,

            [0 + AssassinOffSet] = LibraryFile.M_HelmetA1,
            [1 + AssassinOffSet] = LibraryFile.M_HelmetA2,
            [2 + AssassinOffSet] = LibraryFile.M_HelmetA3,
            [3 + AssassinOffSet] = LibraryFile.M_HelmetA4,

            [0 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HelmetA1,
            [1 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HelmetA2,
            [2 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HelmetA3,
            [3 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HelmetA4,
        };
        #endregion

        #region Armour Librarys  衣服资源库
        /// <summary>
        /// 衣服资源库
        /// </summary>
        public Dictionary<int, LibraryFile> ArmourList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Hum,      //三职业男
            [1] = LibraryFile.M_HumEx1,
            [2] = LibraryFile.M_HumEx2,
            [3] = LibraryFile.M_HumEx3,
            [4] = LibraryFile.M_HumEx4,
            [10] = LibraryFile.M_HumEx10,
            [11] = LibraryFile.M_HumEx11,
            [12] = LibraryFile.M_HumEx12,
            [13] = LibraryFile.M_HumEx13,

            [200] = LibraryFile.传奇2衣服外观,

            [0 + FemaleOffSet] = LibraryFile.WM_Hum,     //三职业女
            [1 + FemaleOffSet] = LibraryFile.WM_HumEx1,
            [2 + FemaleOffSet] = LibraryFile.WM_HumEx2,
            [3 + FemaleOffSet] = LibraryFile.WM_HumEx3,
            [4 + FemaleOffSet] = LibraryFile.WM_HumEx4,
            [10 + FemaleOffSet] = LibraryFile.WM_HumEx10,
            [11 + FemaleOffSet] = LibraryFile.WM_HumEx11,
            [12 + FemaleOffSet] = LibraryFile.WM_HumEx12,
            [13 + FemaleOffSet] = LibraryFile.WM_HumEx13,

            [0 + AssassinOffSet] = LibraryFile.M_HumA,   //刺客男
            [1 + AssassinOffSet] = LibraryFile.M_HumAEx1,
            [2 + AssassinOffSet] = LibraryFile.M_HumAEx2,
            [3 + AssassinOffSet] = LibraryFile.M_HumAEx3,

            [0 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumA,   //刺客女
            [1 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumAEx1,
            [2 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumAEx2,
            [3 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_HumAEx3,
        };
        #endregion

        #region Shield Librarys  盾牌资源库
        /// <summary>
        /// 盾牌资源库
        /// </summary>
        public Dictionary<int, LibraryFile> ShieldList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Shield1,
            [1] = LibraryFile.M_Shield2,

            [0 + FemaleOffSet] = LibraryFile.WM_Shield1,
            [1 + FemaleOffSet] = LibraryFile.WM_Shield2,

            [100] = LibraryFile.EquipEffect_Part,

            [100 + FemaleOffSet] = LibraryFile.EquipEffect_Part,
        };
        #endregion

        #region Fashion Librarys  时装资源库
        /// <summary>
        /// 时装资源库
        /// </summary>
        public Dictionary<int, LibraryFile> FashionList = new Dictionary<int, LibraryFile>
        {
            [0] = LibraryFile.M_Costume,    //时装 三职业男
            [1] = LibraryFile.M_Costume1,
            [0 + FemaleOffSet] = LibraryFile.WM_Costume,  //时装 三职业女
            [1 + FemaleOffSet] = LibraryFile.WM_Costume1,

            [0 + AssassinOffSet] = LibraryFile.M_CostumeA,   //时装  刺客男
            [1 + AssassinOffSet] = LibraryFile.M_CostumeA1,
            [0 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_CostumeA,  //时装 刺客女
            [1 + AssassinOffSet + FemaleOffSet] = LibraryFile.WM_CostumeA1,
        };
        #endregion

        /// <summary>
        /// 行会等级
        /// </summary>
        public string GuildRank
        {
            get => _GuildRank;
            set
            {
                if (_GuildRank == value) return;

                _GuildRank = value;

                NameChanged();
            }
        }
        private string _GuildRank;

        /// <summary>
        /// 职业
        /// </summary>
        public virtual MirClass Class { get; set; }
        /// <summary>
        /// 发型库 头盔库
        /// </summary>
        public MirLibrary HairLibrary, HelmetLibrary;
        /// <summary>
        /// 头发类型
        /// </summary>
        public int HairType;
        /// <summary>
        /// 头发颜色
        /// </summary>
        public Color HairColour;
        /// <summary>
        /// 头发类型偏移
        /// </summary>
        public int HairTypeOffSet;
        /// <summary>
        /// 头盔形状
        /// </summary>
        public int HelmetShape;
        /// <summary>
        /// 时装形状
        /// </summary>
        public int FashionShape;
        /// <summary>
        /// 时装形状偏移量
        /// </summary>
        public int FashionShapeOffset;
        /// <summary>
        /// 时装图片
        /// </summary>
        public int FashionImage;
        /// <summary>
        /// 时装素材库
        /// </summary>
        public MirLibrary FashionLibrary;
        /// <summary>
        /// 时装框架
        /// </summary>
        public int FashionFrame => DrawFrame + (FashionShape % 11) * FashionShapeOffset + ArmourShift;
        /// <summary>
        /// 发型框架
        /// </summary>
        public int HairFrame => DrawFrame + (HairType - 1) * HairTypeOffSet;
        /// <summary>
        /// 头盔框架
        /// </summary>
        public int HelmetFrame => DrawFrame + (HelmetShape - 1) % 10 * ArmourShapeOffSet + ArmourShift;
        /// <summary>
        /// 武器素材库
        /// </summary>
        public MirLibrary WeaponLibrary1, WeaponLibrary2;
        /// <summary>
        /// 武器形状偏移量
        /// </summary>
        public int WeaponShapeOffSet;
        /// <summary>
        /// 武器形状
        /// </summary>
        public int WeaponShape, LibraryWeaponShape;
        /// <summary>
        /// 武器帧
        /// </summary>
        public int WeaponFrame => (LibraryWeaponShape >= 2000 ? Mir2DrawFrame : DrawFrame) + (WeaponShape % 10) * WeaponShapeOffSet; //TODO 这里不对
        /// <summary>
        /// 角色身体素材库
        /// </summary>
        public MirLibrary BodyLibrary;
        /// <summary>
        /// 衣服形状偏移量
        /// </summary>
        public int ArmourShapeOffSet;
        /// <summary>
        /// 衣服形状
        /// </summary>
        public int ArmourShape
        {
            get => GameScene.Game.MapControl.MapInfo.CanPlayName ? 2 : _ArmourShape; //竞技场只显示布衣
            set
            {
                if (_ArmourShape == value) return;

                _ArmourShape = value;
            }
        }
        private int _ArmourShape;
        /// <summary>
        /// 衣服转换
        /// </summary>
        public int ArmourShift;
        /// <summary>
        /// 衣服颜色
        /// </summary>
        public Color ArmourColour;
        /// <summary>
        /// 衣服帧
        /// </summary>
        public int ArmourFrame => (ArmourShape >= 2200 ? Mir2DrawFrame : DrawFrame) + (ArmourShape % 11) * ArmourShapeOffSet + ArmourShift;

        public int ArmourFrame1 => (ArmourShape >= 2200 ? Mir2DrawFrame : DrawFrame) + (2 % 11) * ArmourShapeOffSet + ArmourShift;
        /// <summary>
        /// 坐骑素材库
        /// </summary>
        public MirLibrary HorseLibrary, HorseShapeLibrary, HorseShapeLibrary2;
        /// <summary>
        /// 坐骑形状
        /// </summary>
        public int HorseShape;
        /// <summary>
        /// 坐骑框架
        /// </summary>
        public int HorseFrame => DrawFrame + ((int)Horse - 1) * 5000;
        /// <summary>
        /// 坐骑类型
        /// </summary>
        public HorseType Horse;

        public HorseType HorseType;
        /// <summary>
        /// 盾牌素材库
        /// </summary>
        public MirLibrary ShieldLibrary;
        /// <summary>
        /// 盾牌形状
        /// </summary>
        public int ShieldShape;
        /// <summary>
        /// 盾牌框架
        /// </summary>
        public int ShieldFrame
        {
            get
            {
                if (ShieldShape < 1000)
                    return DrawFrame + ((ShieldShape % 10) - 1) * WeaponShapeOffSet;
                else
                    return 900 + 200 * ((ShieldShape % 10) - 1) + 10 * (byte)Direction + (GameScene.Game.MapControl.Animation % 4);
            }
        }
        /// <summary>
        /// 徽章效果库
        /// </summary>
        public MirLibrary EmblemLibrary;
        /// <summary>
        /// 徽章形态
        /// </summary>
        public int EmblemShape;
        /// <summary>
        /// 衣服图像 武器图像
        /// </summary>
        public int ArmourImage, WeaponImage;
        /// <summary>
        /// 衣服序号 武器序号
        /// </summary>
        public int ArmourIndex, WeaponIndex;
        /// <summary>
        /// 绘制武器
        /// </summary>
        public bool DrawWeapon;
        /// <summary>
        /// 钓鱼竿
        /// </summary>
        public bool HasFishingRod => WeaponShape == 125 || WeaponShape == 126; //武器Shape值定义为125绿色鱼杆 126红色鱼杆  

        /// <summary>
        /// 钓鱼衣服
        /// </summary>
        public bool HasFishingArmour => ArmourShape == 16; //只有钓鱼蓑衣拥有挥杆 等待 收杆的动作素材

        public bool IsFishing;

        /// <summary>
        /// 角色序号
        /// </summary>
        public int CharacterIndex;
        public struct AFTERIMAGE
        {
            public int m_WeaponFrame;
            public int m_UserPointX;
            public int m_UserPointY;
            public int MovingOffSetX;
            public int MovingOffSetY;
            public int m_ArmourFrame;
            public int m_FashionFrame;
            public int m_HelmetFrame;
            public int m_HairFrame;
            public int m_HorseFrame;
            public MirDirection m_Direction;
            public MirAnimation m_CurrentAnimation;
            public int m_DrawFrame;
            public DateTime DrawTime;
        }
        public AFTERIMAGE[] m_xAfterImage = new AFTERIMAGE[10];
        public DateTime m_dwAfterTimer = CEnvir.Now;
        public int m_nSaveIdx, m_nPlayIdx1, m_nPlayIdx2;
        public bool m_bSetIdx = false;
        /// <summary>
        /// 血量标签
        /// </summary>
        public DXLabel HPratioLabel;

        #region 传奇2相关

        public Dictionary<MirAnimation, Frame> Mir2Frames;
        public Frame Mir2CurrentFrame;

        public bool WearingMir2Equip
        {
            get => ArmourShape >= 2200 || LibraryWeaponShape >= 2000;
        }


        private bool _mir2EquipHasWeapon = false;
        public bool Mir2EquipHasWeapon
        {
            get => _mir2EquipHasWeapon;
            set
            {
                if (_mir2EquipHasWeapon == value) return;

                _mir2EquipHasWeapon = value;
            }
        }

        public int Mir2DrawFrame
        {
            get { return _Mir2DrawFrame; }
            set
            {
                if (_Mir2DrawFrame == value) return;

                _Mir2DrawFrame = value;
                DrawFrameChanged();
            }
        }
        private int _Mir2DrawFrame;

        #endregion

        #region 制造系统
        /// <summary>
        /// 制作等级
        /// </summary>
        public virtual int CraftLevel { get; set; }
        /// <summary>
        /// 制作经验
        /// </summary>
        public virtual int CraftExp { get; set; }
        /// <summary>
        /// 制作完成时间
        /// </summary>
        public virtual DateTime CraftFinishTime { get; set; }
        /// <summary>
        /// 收藏的制作信息列表
        /// </summary>
        public virtual CraftItemInfo BookmarkedCraftItemInfo { get; set; }
        /// <summary>
        /// 正在制作的物品
        /// </summary>
        public virtual CraftItemInfo CraftingItem { get; set; }
        #endregion

        /// <summary>
        /// 可重复任务
        /// </summary>
        public virtual int RepeatableQuestRemains { get; set; }
        /// <summary>
        /// 每日任务
        /// </summary>
        public virtual int DailyQuestRemains { get; set; }

        /// <summary>
        /// 玩家对象
        /// </summary>
        public PlayerObject()
        {
            if (HPratioLabel == null) InitHPratioLabel();

        }
        /// <summary>
        /// 玩家对象信息
        /// </summary>
        /// <param name="info"></param>
        public PlayerObject(S.ObjectPlayer info)
        {
            CharacterIndex = info.Index;

            ObjectID = info.ObjectID;

            Name = info.Name;
            NameColour = info.NameColour;
            AchievementTitle = info.AchievementTitle;

            Class = info.Class;
            Gender = info.Gender;

            Poison = info.Poison;

            foreach (BuffType type in info.Buffs)
                VisibleBuffs.Add(type);
            foreach (int index in info.CustomIndexs)
            {
                CustomBuffInfo customBuff = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == index);
                if (customBuff != null)
                    VisibleCustomBuffs.Add(customBuff);
            }

            Title = info.GuildName;

            CurrentLocation = info.Location;
            Direction = info.Direction;

            HairType = info.HairType;
            HairColour = info.HairColour;

            ArmourShape = info.Armour;
            ArmourColour = info.ArmourColour;
            LibraryWeaponShape = info.Weapon;
            HorseShape = info.HorseShape;
            HelmetShape = info.Helmet;
            ShieldShape = info.Shield;
            EmblemShape = info.Emblem;
            FashionShape = info.Fashion;
            FashionImage = info.FashionImage;

            ArmourImage = info.ArmourImage;
            WeaponImage = info.WeaponImage;
            ArmourIndex = info.ArmourIndex;
            WeaponIndex = info.WeaponIndex;

            Light = info.Light;

            Dead = info.Dead;
            Horse = info.Horse;
            HorseType = info.HorseType;

            CraftLevel = info.CraftLevel;
            CraftExp = info.CraftExp;
            CraftFinishTime = info.CraftFinishTime;
            BookmarkedCraftItemInfo = info.BookmarkedCraftItemInfo;

            if (HPratioLabel == null) InitHPratioLabel();

            UpdateLibraries();

            SetFrame(new ObjectAction(!Dead ? MirAction.Standing : MirAction.Dead, MirDirection.Up, CurrentLocation));

            GameScene.Game.MapControl.AddObject(this);
        }

        /// <summary>
        /// 血量标签
        /// </summary>
        public void InitHPratioLabel()
        {
            HPratioLabel = new DXLabel()
            {
                BackColour = Color.Empty,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                IsVisible = true,
                UI_Offset_X = 0,
            };
        }

        /// <summary>
        /// 更新库
        /// </summary>
        public void UpdateLibraries()
        {
            LibraryFile file;

            WeaponLibrary2 = null;

            Frames = new Dictionary<MirAnimation, Frame>(FrameSet.Players);
            Mir2Frames = new Dictionary<MirAnimation, Frame>(FrameSet.Players_Mir2);

            CEnvir.LibraryList.TryGetValue(LibraryFile.Horse, out HorseLibrary);

            HorseShapeLibrary = null;
            HorseShapeLibrary2 = null;

            if (LibraryWeaponShape >= 2000)
                WeaponShape = LibraryWeaponShape - 2000;
            else if (LibraryWeaponShape >= 1000)
                WeaponShape = LibraryWeaponShape - 1000;
            else
                WeaponShape = LibraryWeaponShape;

            switch (HorseShape)  //马甲外形
            {
                case 1:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.HorseIron, out HorseShapeLibrary);
                    break;
                case 2:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.HorseSilver, out HorseShapeLibrary);
                    break;
                case 3:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.HorseGold, out HorseShapeLibrary);
                    break;
                case 4:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.HorseBlue, out HorseShapeLibrary);
                    break;
                case 5:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.HorseDark, out HorseShapeLibrary);
                    CEnvir.LibraryList.TryGetValue(LibraryFile.HorseDarkEffect, out HorseShapeLibrary2);
                    break;
                default:
                    if ((HorseShape > 5) && (HorseShape < 26))
                    {
                        CEnvir.LibraryList.TryGetValue((LibraryFile)((int)LibraryFile.HorseDarkEffect + (HorseShape - 5)), out HorseShapeLibrary);
                    }
                    break;
            }

            switch (EmblemShape)     //徽章道具特效
            {
                case 1:
                case 2:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter, out EmblemLibrary);
                    break;
                case 3:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.GameInter2, out EmblemLibrary);
                    break;
                case 4:
                    CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Item, out EmblemLibrary);
                    break;
            }

            switch (Class)  //职业
            {
                case MirClass.Warrior:
                case MirClass.Wizard:
                case MirClass.Taoist:
                    ArmourShapeOffSet = ArmourShape >= 2200 ? 600 : 5000;
                    WeaponShapeOffSet = LibraryWeaponShape >= 2000 ? 600 : 5000;
                    HairTypeOffSet = 5000;
                    FashionShapeOffset = 5000;

                    switch (Gender)
                    {
                        case MirGender.Male:   //男
                            if (!ArmourList.TryGetValue(ArmourShape / 11, out file))  //衣服绘制
                            {
                                file = LibraryFile.M_Hum;
                                ArmourShape = 0;
                            }

                            CEnvir.LibraryList.TryGetValue(file, out BodyLibrary);

                            CEnvir.LibraryList.TryGetValue(LibraryFile.M_Hair, out HairLibrary);         //头发

                            if (!HelmetList.TryGetValue((HelmetShape - 1) / 10, out file)) file = LibraryFile.None;   //头盔
                            CEnvir.LibraryList.TryGetValue(file, out HelmetLibrary);

                            if (!WeaponList.TryGetValue(LibraryWeaponShape / 10, out file)) file = LibraryFile.None;  //武器
                            CEnvir.LibraryList.TryGetValue(file, out WeaponLibrary1);

                            if (FashionShape >= 0)
                            {
                                if (!FashionList.TryGetValue(FashionShape / 11, out file))      //时装绘制
                                {
                                    file = LibraryFile.M_Costume;
                                    FashionShape = 0;
                                }
                                CEnvir.LibraryList.TryGetValue(file, out FashionLibrary);
                            }

                            if (ShieldShape >= 0)   //盾牌
                            {
                                if (!ShieldList.TryGetValue(ShieldShape / 10, out file)) file = LibraryFile.None;
                                CEnvir.LibraryList.TryGetValue(file, out ShieldLibrary);
                            }
                            break;
                        case MirGender.Female:   //女
                            if (!ArmourList.TryGetValue(ArmourShape / 11 + FemaleOffSet, out file))
                            {
                                file = LibraryFile.WM_Hum;
                                ArmourShape = 0;
                            }

                            CEnvir.LibraryList.TryGetValue(file, out BodyLibrary);

                            CEnvir.LibraryList.TryGetValue(LibraryFile.WM_Hair, out HairLibrary);

                            if (!HelmetList.TryGetValue(HelmetShape / 10 + FemaleOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out HelmetLibrary);

                            if (!WeaponList.TryGetValue(LibraryWeaponShape / 10 + FemaleOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out WeaponLibrary1);

                            if (FashionShape >= 0)
                            {
                                if (!FashionList.TryGetValue(FashionShape / 11 + FemaleOffSet, out file))  // 时装
                                {
                                    file = LibraryFile.WM_Costume;
                                    FashionShape = 0;
                                }
                                CEnvir.LibraryList.TryGetValue(file, out FashionLibrary);
                            }

                            if (ShieldShape >= 0)
                            {
                                if (!ShieldList.TryGetValue(ShieldShape / 10 + FemaleOffSet, out file)) file = LibraryFile.None;
                                CEnvir.LibraryList.TryGetValue(file, out ShieldLibrary);
                            }
                            break;
                    }
                    break;
                case MirClass.Assassin:  //刺客
                    ArmourShapeOffSet = ArmourShape >= 2200 ? 600 : 3000;
                    WeaponShapeOffSet = LibraryWeaponShape >= 2000 ? 600 : 5000;
                    HairTypeOffSet = 5000;
                    FashionShapeOffset = 3000;

                    //todo 头盔咋办
                    int AdjustedAssassinOffset = ArmourShape >= 2200 ? 0 : AssassinOffSet;

                    switch (Gender)
                    {
                        case MirGender.Male:
                            if (!ArmourList.TryGetValue(ArmourShape / 11 + AdjustedAssassinOffset, out file))
                            {
                                file = LibraryFile.M_HumA;
                                ArmourShape = 0;
                            }

                            CEnvir.LibraryList.TryGetValue(file, out BodyLibrary);

                            CEnvir.LibraryList.TryGetValue(LibraryFile.M_HairA, out HairLibrary);

                            if (!HelmetList.TryGetValue(HelmetShape / 10 + AssassinOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out HelmetLibrary);

                            if (!WeaponList.TryGetValue(LibraryWeaponShape / 10, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out WeaponLibrary1);

                            if (FashionShape >= 0)
                            {
                                if (!FashionList.TryGetValue(FashionShape / 11 + AssassinOffSet, out file))  //时装
                                {
                                    file = LibraryFile.M_CostumeA;
                                    ArmourShape = 0;
                                }
                                CEnvir.LibraryList.TryGetValue(file, out FashionLibrary);
                            }

                            if (!ShieldList.TryGetValue(ShieldShape / 10, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out ShieldLibrary);

                            if (LibraryWeaponShape < 1200) break;

                            if (!WeaponList.TryGetValue(LibraryWeaponShape / 10 + RightHandOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out WeaponLibrary2);
                            break;
                        case MirGender.Female:
                            if (!ArmourList.TryGetValue(ArmourShape / 11 + AdjustedAssassinOffset + FemaleOffSet, out file))
                            {
                                file = LibraryFile.WM_HumA;
                                FashionShape = 0;
                            }

                            CEnvir.LibraryList.TryGetValue(file, out BodyLibrary);
                            CEnvir.LibraryList.TryGetValue(LibraryFile.WM_HairA, out HairLibrary);

                            if (!HelmetList.TryGetValue(HelmetShape / 10 + AssassinOffSet + FemaleOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out HelmetLibrary);

                            if (!WeaponList.TryGetValue(LibraryWeaponShape / 10 + FemaleOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out WeaponLibrary1);

                            if (FashionShape >= 0)
                            {
                                if (!FashionList.TryGetValue(FashionShape / 11 + AssassinOffSet + FemaleOffSet, out file))   //时装
                                {
                                    file = LibraryFile.WM_CostumeA;
                                    FashionShape = 0;
                                }
                                CEnvir.LibraryList.TryGetValue(file, out FashionLibrary);
                            }

                            if (!ShieldList.TryGetValue(ShieldShape / 10 + FemaleOffSet, out file)) file = LibraryFile.None;
                            CEnvir.LibraryList.TryGetValue(file, out ShieldLibrary);

                            if (LibraryWeaponShape < 1200) break;

                            if (!WeaponList.TryGetValue(LibraryWeaponShape / 10 + FemaleOffSet + RightHandOffSet, out file)) file = LibraryFile.None;

                            CEnvir.LibraryList.TryGetValue(file, out WeaponLibrary2);
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 设置动画
        /// </summary>
        /// <param name="action"></param>
        public override void SetAnimation(ObjectAction action)
        {
            MirAnimation animation;
            DrawWeapon = true;
            MagicType type;
            switch (action.Action)
            {
                case MirAction.Standing:
                    //if(VisibleBuffs.Contains(BuffType.Stealth))
                    animation = MirAnimation.Standing;

                    if (CEnvir.Now < StanceTime)
                        animation = MirAnimation.Stance;

                    if (VisibleBuffs.Contains(BuffType.Cloak))
                        animation = MirAnimation.CreepStanding;

                    if (Horse != HorseType.None)
                        animation = MirAnimation.HorseStanding;

                    if (VisibleBuffs.Contains(BuffType.DragonRepulse))
                        animation = MirAnimation.DragonRepulseMiddle;
                    else if (CurrentAnimation == MirAnimation.DragonRepulseMiddle)
                        animation = MirAnimation.DragonRepulseEnd;

                    if (VisibleBuffs.Contains(BuffType.ElementalHurricane))
                        animation = MirAnimation.ChannellingMiddle;
                    break;
                case MirAction.Moving:
                    //if(VisibleBuffs.Contains(BuffType.Stealth))

                    animation = MirAnimation.Walking;

                    if (Horse != HorseType.None)
                        animation = MirAnimation.HorseWalking;

                    if ((MagicType)action.Extra[1] == MagicType.ShoulderDash || (MagicType)action.Extra[1] == MagicType.Assault || (MagicType)action.Extra[1] == MagicType.ReigningStep)
                        animation = MirAnimation.Combat8;
                    else if (VisibleBuffs.Contains(BuffType.Cloak))
                        animation = VisibleBuffs.Contains(BuffType.GhostWalk) ? MirAnimation.CreepWalkFast : MirAnimation.CreepWalkSlow;
                    else if ((int)action.Extra[0] >= 2)
                    {
                        animation = MirAnimation.Running;
                        if (Horse != HorseType.None)
                            animation = MirAnimation.HorseRunning;
                    }
                    break;
                case MirAction.Pushed:
                    animation = MirAnimation.Pushed;
                    break;
                case MirAction.Attack:
                    type = (MagicType)action.Extra[1];
                    animation = Functions.GetAttackAnimation(Class, LibraryWeaponShape, type);
                    break;
                case MirAction.Mining:
                    animation = Functions.GetAttackAnimation(Class, LibraryWeaponShape, MagicType.None);
                    break;
                case MirAction.RangeAttack:
                    animation = MirAnimation.Combat1;
                    break;
                case MirAction.Spell:
                    type = (MagicType)action.Extra[0];

                    animation = Functions.GetMagicAnimation(type);

                    if (type == MagicType.PoisonousCloud)
                        DrawWeapon = false;

                    if (VisibleBuffs.Contains(BuffType.ElementalHurricane))
                        animation = MirAnimation.ChannellingEnd;
                    break;
                case MirAction.Struck:
                    animation = MirAnimation.Struck;
                    if (Horse != HorseType.None)
                        animation = MirAnimation.HorseStruck;
                    break;
                case MirAction.Die:
                    animation = MirAnimation.Die;
                    break;
                case MirAction.Dead:
                    animation = MirAnimation.Dead;
                    break;
                case MirAction.Harvest:
                    animation = MirAnimation.Harvest;
                    break;
                case MirAction.FishingCast:
                    animation = MirAnimation.FishingCast;
                    break;
                case MirAction.FishingWait:
                    animation = MirAnimation.FishingWait;
                    break;
                case MirAction.FishingReel:
                    animation = MirAnimation.FishingReel;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            CurrentAnimation = animation;
            if (!Frames.TryGetValue(CurrentAnimation, out CurrentFrame))
                CurrentFrame = Frame.EmptyFrame;
            //传奇2当前帧
            if (!FrameSet.Players_Mir2.TryGetValue(CurrentAnimation, out Mir2CurrentFrame))
                Mir2CurrentFrame = Frame.EmptyFrame;
        }
        /// <summary>
        /// 设置帧
        /// </summary>
        /// <param name="action"></param>
        public sealed override void SetFrame(ObjectAction action)
        {
            base.SetFrame(action);

            switch (action.Action)
            {
                case MirAction.Spell:
                case MirAction.Attack:
                    StanceTime = CEnvir.Now.AddSeconds(3);
                    break;
            }

            switch (Class)
            {
                case MirClass.Assassin:
                    if (ArmourShape >= 2200 || LibraryWeaponShape >= 2000) break;
                    switch (CurrentAnimation)
                    {
                        case MirAnimation.Standing:
                            ArmourShift = 0;
                            break;
                        case MirAnimation.Walking:
                            ArmourShift = 1600;
                            break;
                        case MirAnimation.Running:
                            ArmourShift = 1600;
                            break;
                        case MirAnimation.CreepStanding:
                            ArmourShift = 240;
                            break;
                        case MirAnimation.CreepWalkSlow:
                        case MirAnimation.CreepWalkFast:
                            ArmourShift = 240;
                            break;
                        case MirAnimation.Pushed:
                            ArmourShift = 160;
                            //pushed 2 = 160
                            break;
                        case MirAnimation.Combat1: //双手攻击
                            ArmourShift = -400;
                            break;
                        case MirAnimation.Combat2:
                            ;//  throw new NotImplementedException();
                            break;
                        case MirAnimation.Combat3:
                            ArmourShift = 0;
                            break;
                        case MirAnimation.Combat4:
                            ArmourShift = 80;
                            break;
                        case MirAnimation.Combat5:
                            ArmourShift = 400;
                            break;
                        case MirAnimation.Combat6:
                            ArmourShift = 400;
                            break;
                        case MirAnimation.Combat7:
                            ArmourShift = 400;
                            break;
                        case MirAnimation.Combat8:
                            ArmourShift = 720;
                            break;
                        case MirAnimation.Combat9:
                            ArmourShift = -960;
                            break;
                        case MirAnimation.Combat10:
                            ArmourShift = -480;
                            break;
                        case MirAnimation.Combat11:
                            ArmourShift = -400;
                            break;
                        case MirAnimation.Combat12:
                            ArmourShift = -400;
                            break;
                        case MirAnimation.Combat13:
                            ArmourShift = -400;
                            break;
                        case MirAnimation.Combat14:
                        case MirAnimation.DragonRepulseStart:
                        case MirAnimation.DragonRepulseMiddle:
                        case MirAnimation.DragonRepulseEnd:
                            ArmourShift = 0;
                            break;
                        case MirAnimation.Harvest:
                            ArmourShift = 160;
                            break;
                        case MirAnimation.Stance:
                            ArmourShift = 160;
                            break;
                        case MirAnimation.Struck:
                            ArmourShift = -640;
                            break;
                        case MirAnimation.Die:
                            ArmourShift = -400;
                            break;
                        case MirAnimation.Dead:
                            ArmourShift = -400;
                            break;
                        case MirAnimation.HorseStanding:
                            ArmourShift = 80;
                            break;
                        case MirAnimation.HorseWalking:
                            ArmourShift = 80;
                            break;
                        case MirAnimation.HorseRunning:
                            ArmourShift = 80;
                            break;
                        case MirAnimation.HorseStruck:
                            ArmourShift = 80;
                            break;
                        //刺客钓鱼 动作偏移
                        case MirAnimation.FishingCast:
                            ArmourShift = 80;
                            break;
                        case MirAnimation.FishingWait:
                            ArmourShift = 80;
                            break;
                        case MirAnimation.FishingReel:
                            ArmourShift = 80;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    break;
            }
        }
        /// <summary>
        /// 帧索引改变
        /// </summary>
        public override void FrameIndexChanged()
        {
            switch (CurrentAction)
            {
                case MirAction.Spell:
                    switch (MagicType)
                    {
                        case MagicType.SeismicSlam:   //天雷锤
                            if (FrameIndex == 4)
                            {
                                Effects.Add(new MirEffect(700, 7, TimeSpan.FromMilliseconds(120), LibraryFile.MonMagicEx7, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    MapTarget = Functions.Move(CurrentLocation, Direction, 2),
                                });
                            }
                            break;
                        case MagicType.CrushingWave:
                            if (FrameIndex == 4)
                            {
                                MirEffect spell;
                                Effects.Add(spell = new MirProjectile(200, 8, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx6, 35, 35, Globals.LightningColour, CurrentLocation)
                                {
                                    Blend = true,
                                    Has16Directions = false,
                                    MapTarget = Functions.Move(CurrentLocation, Direction, Globals.MagicRange),
                                    Speed = 100,
                                });
                                spell.Process();
                                DXSoundManager.Play(SoundIndex.DestructiveBlow);

                                Effects.Add(new MirEffect(300, 9, TimeSpan.FromMilliseconds(150), LibraryFile.MagicEx6, 10, 35, Globals.LightningColour)
                                {
                                    Blend = true,
                                    Direction = Direction,
                                    MapTarget = Functions.Move(CurrentLocation, Direction, 1),
                                });
                            }
                            break;
                        default:
                            base.FrameIndexChanged();
                            break;
                    }
                    break;
                default:
                    base.FrameIndexChanged();
                    break;
            }
        }
        /// <summary>
        /// 设置动作
        /// </summary>
        /// <param name="action"></param>
        public override void SetAction(ObjectAction action)  //设置动作
        {
            base.SetAction(action);

            switch (CurrentAction)
            {
                case MirAction.Attack:
                    switch (MagicType)
                    {
                        #region Sweet Brier and Karma (Karma should have different attack will do later if can be bothered)
                        case MagicType.SweetBrier:
                        case MagicType.Karma:
                            if (LibraryWeaponShape >= 1200)
                            {
                                Effects.Add(new MirEffect(300, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    DrawColour = Globals.NoneColour,
                                    Direction = Direction,
                                });
                            }
                            else if (LibraryWeaponShape >= 1100)
                            {
                                Effects.Add(new MirEffect(100, 6, TimeSpan.FromMilliseconds(100), LibraryFile.MagicEx4, 20, 70, Globals.NoneColour)
                                {
                                    Blend = true,
                                    Target = this,
                                    DrawColour = Globals.NoneColour,
                                    Direction = Direction,
                                });
                            }

                            if (Gender == MirGender.Male)
                                DXSoundManager.Play(SoundIndex.SweetBrierMale);
                            else
                                DXSoundManager.Play(SoundIndex.SweetBrierFemale);
                            break;
                            #endregion
                    }
                    break;
            }
        }
        /// <summary>
        /// 画自定义外观特效
        /// 注意传入参数是道具的index而不是image
        /// </summary>
        /// <param name="itemIndex"></param>
        public void DrawCustomOuterEffect(int itemIndex)
        {
            ItemDisplayEffect outerDisplayEffect = Globals.ItemDisplayEffectList.Binding.FirstOrDefault(x => x.Info.Index == itemIndex);
            if (outerDisplayEffect != null && outerDisplayEffect.DrawOuterEffect)
            {
                if (!CEnvir.LibraryList.TryGetValue(outerDisplayEffect.OuterEffectLibrary, out MirLibrary outerEffectLib)) return;
                if (outerDisplayEffect.IsUnisex)
                {
                    outerEffectLib.DrawBlend(outerDisplayEffect.OuterImageStartIndex + (GameScene.Game.MapControl.Animation / 2) % outerDisplayEffect.OuterImageCount + (int)Direction * 20, DrawX + outerDisplayEffect.OuterX, DrawY + outerDisplayEffect.OuterY, Color.White, true, 1f, ImageType.Image);
                }
                else
                {
                    outerEffectLib.DrawBlend(outerDisplayEffect.OuterImageStartIndex + ((byte)Gender * 5000) + DrawFrame, DrawX + outerDisplayEffect.OuterX, DrawY + outerDisplayEffect.OuterY, Color.White, true, 1f, ImageType.Image);
                }
            }
        }
        /// <summary>
        /// 绘制身后的翅膀
        /// </summary>
        /// <returns></returns>
        public bool DrawWingsBehind()
        {
            switch (Direction)
            {
                case MirDirection.Up:
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    return false;
                case MirDirection.DownRight:
                case MirDirection.Down:
                case MirDirection.DownLeft:
                    switch (ArmourImage)
                    {
                        //All
                        case 986:
                        case 996:
                            return false;
                        default:
                            return true;
                    }
            }
            return false;
        }
        /// <summary>
        /// 绘制身后的武器效果
        /// </summary>
        /// <returns></returns>
        public bool DrawWeaponEffectBehind()
        {
            switch (Direction)
            {
                case MirDirection.Up:
                case MirDirection.DownLeft:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 绘制身后的盾牌效果
        /// </summary>
        /// <returns></returns>
        public bool DrawShieldEffectBehind()
        {
            switch (Direction)
            {
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.DownRight:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 绘制身前的翅膀
        /// </summary>
        /// <returns></returns>
        public bool DrawWingsInfront()
        {
            switch (Direction)
            {
                case MirDirection.Up:
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    return true;
                case MirDirection.DownRight:
                case MirDirection.Down:
                case MirDirection.DownLeft:
                    switch (ArmourImage)
                    {
                        //All
                        case 986:
                        case 996:
                            return true;
                    }
                    break;
            }
            return false;
        }
        /// <summary>
        /// 绘制身前的武器效果
        /// </summary>
        /// <returns></returns>
        public bool DrawWeaponEffectInfront()
        {
            switch (Direction)
            {
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.DownRight:
                case MirDirection.Down:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 绘制身前的盾牌效果
        /// </summary>
        /// <returns></returns>
        public bool DrawShieldEffectInfront()
        {
            switch (Direction)
            {
                case MirDirection.Up:
                case MirDirection.Down:
                case MirDirection.DownLeft:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (BodyLibrary == null) return;

            if (DrawWingsBehind())
                DrawWings();

            if (DrawWeaponEffectBehind())
                DrawWeaponEffect();

            if (DrawShieldEffectBehind())
                DrawShieldEffect();

            //修复徽章外观图层问题，应在画形体之前绘画
            DrawEmblemEffect();  //画徽章效果

            DrawBody(true);  //画形体

            if (DrawWingsInfront())
                DrawWings();

            if (DrawWeaponEffectInfront())
                DrawWeaponEffect();

            if (DrawShieldEffectInfront())
                DrawShieldEffect();

        }
        /// <summary>
        /// 混合绘制
        /// </summary>
        public override void DrawBlend()
        {
            if (BodyLibrary == null || VisibleBuffs.Contains(BuffType.Invisibility) || VisibleBuffs.Contains(BuffType.Transparency) || VisibleBuffs.Contains(BuffType.Cloak)) return;

            DXManager.SetBlend(true, 0.60F, blendtype: BlendType.HIGHLIGHT);
            DrawBody(false);
            DXManager.SetBlend(false);
        }
        public void DrawAfterBody(int nOrder)
        {
            int m_WeaponFrame;
            int m_DrawX;
            int m_DrawY;
            int m_UserPointX;
            int m_UserPointY;
            int MovingOffSetX;
            int MovingOffSetY;
            int m_ArmourFrame;
            int m_FashionFrame;
            int m_HelmetFrame;
            int m_HairFrame;
            int m_HorseFrame;
            MirDirection m_Direction;
            MirAnimation m_CurrentAnimation;
            int m_DrawFrame;
            float BodyOpacity;
            if (nOrder == 1)
            {
                m_WeaponFrame = m_xAfterImage[m_nPlayIdx1].m_WeaponFrame;
                m_UserPointX = m_xAfterImage[m_nPlayIdx1].m_UserPointX;
                m_UserPointY = m_xAfterImage[m_nPlayIdx1].m_UserPointY;
                MovingOffSetX = m_xAfterImage[m_nPlayIdx1].MovingOffSetX;
                MovingOffSetY = m_xAfterImage[m_nPlayIdx1].MovingOffSetY;
                m_ArmourFrame = m_xAfterImage[m_nPlayIdx1].m_ArmourFrame;
                m_FashionFrame = m_xAfterImage[m_nPlayIdx1].m_FashionFrame;
                m_HelmetFrame = m_xAfterImage[m_nPlayIdx1].m_HelmetFrame;
                m_HairFrame = m_xAfterImage[m_nPlayIdx1].m_HairFrame;
                m_HorseFrame = m_xAfterImage[m_nPlayIdx1].m_HorseFrame;
                m_Direction = m_xAfterImage[m_nPlayIdx1].m_Direction;
                m_CurrentAnimation = m_xAfterImage[m_nPlayIdx1].m_CurrentAnimation;
                m_DrawFrame = m_xAfterImage[m_nPlayIdx1].m_DrawFrame;
                //    CEnvir.DebugX = (int)((CEnvir.Now - m_xAfterImage[m_nPlayIdx1].DrawTime).TotalMilliseconds);
                BodyOpacity = 1 - (float)((CEnvir.Now - m_xAfterImage[m_nPlayIdx1].DrawTime).TotalMilliseconds) / (330);
            }
            else
            {
                m_WeaponFrame = m_xAfterImage[m_nPlayIdx2].m_WeaponFrame;
                m_UserPointX = m_xAfterImage[m_nPlayIdx2].m_UserPointX;
                m_UserPointY = m_xAfterImage[m_nPlayIdx2].m_UserPointY;
                MovingOffSetX = m_xAfterImage[m_nPlayIdx2].MovingOffSetX;
                MovingOffSetY = m_xAfterImage[m_nPlayIdx2].MovingOffSetY;
                m_ArmourFrame = m_xAfterImage[m_nPlayIdx2].m_ArmourFrame;
                m_FashionFrame = m_xAfterImage[m_nPlayIdx2].m_FashionFrame;
                m_HelmetFrame = m_xAfterImage[m_nPlayIdx2].m_HelmetFrame;
                m_HairFrame = m_xAfterImage[m_nPlayIdx2].m_HairFrame;
                m_HorseFrame = m_xAfterImage[m_nPlayIdx2].m_HorseFrame;
                m_Direction = m_xAfterImage[m_nPlayIdx2].m_Direction;
                m_CurrentAnimation = m_xAfterImage[m_nPlayIdx2].m_CurrentAnimation;
                m_DrawFrame = m_xAfterImage[m_nPlayIdx2].m_DrawFrame;
                //     CEnvir.DebugY = (int)((CEnvir.Now - m_xAfterImage[m_nPlayIdx2].DrawTime).TotalMilliseconds);
                BodyOpacity = 1 - (float)((CEnvir.Now - m_xAfterImage[m_nPlayIdx2].DrawTime).TotalMilliseconds) / (330);
            }

            //   BodyOpacity = 1;

            m_DrawX = m_UserPointX - User.CurrentLocation.X + Scenes.Views.MapControl.OffSetX;
            m_DrawY = m_UserPointY - User.CurrentLocation.Y + Scenes.Views.MapControl.OffSetY;

            m_DrawX *= Scenes.Views.MapControl.CellWidth;
            m_DrawY *= Scenes.Views.MapControl.CellHeight;

            m_DrawX += MovingOffSetX - User.MovingOffSet.X;
            m_DrawY += MovingOffSetY - User.MovingOffSet.Y;

            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.ScratchSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0);
            //DXManager.Sprite.Flush();

            int l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;

            MirImage image;
            switch (m_Direction)
            {
                case MirDirection.Up:
                case MirDirection.DownLeft:
                case MirDirection.Left:
                case MirDirection.UpLeft:
                    if (!DrawWeapon) break;
                    image = WeaponLibrary1?.GetImage(m_WeaponFrame);
                    if (image == null) break;

                    WeaponLibrary1.Draw(m_WeaponFrame, m_DrawX, m_DrawY, Color.White, true, /*BodyOpacity*/1, ImageType.Image);

                    l = Math.Min(l, m_DrawX + image.OffSetX);
                    t = Math.Min(t, m_DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                    break;
                default:
                    if (!DrawWeapon) break;

                    var frame = ShieldShape >= 0 ? ShieldFrame : m_WeaponFrame;
                    image = WeaponLibrary2?.GetImage(frame);

                    if (image == null) break;

                    WeaponLibrary2.Draw(frame, m_DrawX, m_DrawY, Color.White, true,  /*BodyOpacity*/1, ImageType.Image);

                    l = Math.Min(l, m_DrawX + image.OffSetX);
                    t = Math.Min(t, m_DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                    break;
            }

            if (FashionShape >= 0 && ArmourShape != 16)
            {
                image = FashionLibrary?.GetImage(m_FashionFrame);
                if (image != null)
                {
                    FashionLibrary.Draw(m_FashionFrame, m_DrawX, m_DrawY, Color.White, true,  /*BodyOpacity*/1, ImageType.Image);

                    l = Math.Min(l, m_DrawX + image.OffSetX);
                    t = Math.Min(t, m_DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                }
            }
            else
            {
                image = BodyLibrary?.GetImage(m_ArmourFrame);
                if (image != null)
                {
                    BodyLibrary.Draw(m_ArmourFrame, m_DrawX, m_DrawY, Color.White, true,  /*BodyOpacity*/1, ImageType.Image);

                    if (ArmourColour.ToArgb() != 0)
                        BodyLibrary.Draw(m_ArmourFrame, m_DrawX, m_DrawY, ArmourColour, true,  /*BodyOpacity*/1, ImageType.Overlay);

                    l = Math.Min(l, m_DrawX + image.OffSetX);
                    t = Math.Min(t, m_DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                }
            }

            if (FashionShape < 0)
            {
                if (HelmetShape > 0)
                {
                    image = HelmetLibrary?.GetImage(m_HelmetFrame);
                    if (image != null)
                    {
                        HelmetLibrary.Draw(m_HelmetFrame, m_DrawX, m_DrawY, Color.White, true,  /*BodyOpacity*/1, ImageType.Image);

                        l = Math.Min(l, m_DrawX + image.OffSetX);
                        t = Math.Min(t, m_DrawY + image.OffSetY);
                        r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                        b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                    }
                }
                else
                {
                    image = HairLibrary.GetImage(m_HairFrame);
                    if (HairType > 0 && image != null)
                    {
                        HairLibrary.Draw(m_HairFrame, m_DrawX, m_DrawY, HairColour, true,  /*BodyOpacity*/1, ImageType.Image);

                        l = Math.Min(l, m_DrawX + image.OffSetX);
                        t = Math.Min(t, m_DrawY + image.OffSetY);
                        r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                        b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                    }
                }
            }

            switch (m_Direction)
            {
                case MirDirection.UpRight:
                case MirDirection.Right:
                case MirDirection.DownRight:
                case MirDirection.Down:
                    if (!DrawWeapon) break;
                    image = WeaponLibrary1?.GetImage(m_WeaponFrame);
                    if (image == null) break;

                    WeaponLibrary1.Draw(m_WeaponFrame, m_DrawX, m_DrawY, Color.White, true,  /*BodyOpacity*/1, ImageType.Image);

                    l = Math.Min(l, m_DrawX + image.OffSetX);
                    t = Math.Min(t, m_DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                    break;
                default:
                    if (!DrawWeapon) break;

                    var frame = ShieldShape >= 0 ? ShieldFrame : m_WeaponFrame;
                    image = WeaponLibrary2?.GetImage(frame);

                    if (image == null) break;

                    WeaponLibrary2.Draw(frame, m_DrawX, m_DrawY, Color.White, true,  /*BodyOpacity*/1, ImageType.Image);

                    l = Math.Min(l, m_DrawX + image.OffSetX);
                    t = Math.Min(t, m_DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + m_DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + m_DrawY + image.OffSetY);
                    break;
            }

            DXManager.SetSurface(oldSurface);
            float oldOpacity = DXManager.Opacity;

            if (oldOpacity != Opacity && !DXManager.Blending) DXManager.SetOpacity(Opacity);

            Color ShowColor = Color.FromArgb((int)(255 * (BodyOpacity)), (int)((255) * (BodyOpacity)), (int)((255) * (BodyOpacity)), 255);

            switch (m_CurrentAnimation)
            {
                case MirAnimation.HorseStanding:
                case MirAnimation.HorseWalking:
                case MirAnimation.HorseRunning:
                case MirAnimation.HorseStruck:

                    switch (HorseShape)
                    {
                        case 0:
                            HorseLibrary?.Draw(m_HorseFrame, m_DrawX, m_DrawY, ShowColor/*Color.White*/, true, /*BodyOpacity * */Opacity, ImageType.Image);
                            break;
                        case 1:
                        case 2:
                        case 3:
                            HorseShapeLibrary?.Draw(m_HorseFrame, m_DrawX, m_DrawY, ShowColor/*Color.White*/, true, /*BodyOpacity **/ Opacity, ImageType.Image);
                            break;
                        case 4:
                            HorseShapeLibrary?.Draw(m_DrawFrame, m_DrawX, m_DrawY, ShowColor/*Color.White*/, true, /*BodyOpacity **/ Opacity, ImageType.Image);
                            break;
                        case 5:
                            HorseShapeLibrary?.Draw(m_DrawFrame, m_DrawX, m_DrawY, ShowColor/*Color.White*/, true, /*BodyOpacity **/ Opacity, ImageType.Image);
                            break;
                        default:
                            if ((HorseShape > 5) && (HorseShape < 26))
                                HorseShapeLibrary?.Draw(m_HorseFrame, m_DrawX, m_DrawY, ShowColor/*Color.White*/, true, /*BodyOpacity **/ Opacity, ImageType.Image);
                            break;
                    }
                    break;
            }

            DXManager.Sprite.Draw(DXManager.ScratchTexture, Rectangle.FromLTRB(l, t, r, b), Vector2.Zero, new Vector2(l, t), ShowColor);
            CEnvir.DPSCounter++;
            if (oldOpacity != Opacity && !DXManager.Blending) DXManager.SetOpacity(oldOpacity);

        }
        /// <summary>
        /// 绘制形态 外观
        /// </summary>
        /// <param name="shadow"></param>
        public override void DrawBody(bool shadow)
        {
            if (VisibleBuffs.Contains(BuffType.AfterImages) && BigPatchConfig.ChkShowRebirthShow)
            {
                if (CEnvir.Now > m_dwAfterTimer)
                {
                    m_xAfterImage[m_nSaveIdx].m_WeaponFrame = WeaponFrame;

                    m_xAfterImage[m_nSaveIdx].MovingOffSetX = MovingOffSet.X;
                    m_xAfterImage[m_nSaveIdx].MovingOffSetY = MovingOffSet.Y;

                    m_xAfterImage[m_nSaveIdx].m_UserPointX = CurrentLocation.X;
                    m_xAfterImage[m_nSaveIdx].m_UserPointY = CurrentLocation.Y;

                    m_xAfterImage[m_nSaveIdx].m_ArmourFrame = ArmourFrame;
                    m_xAfterImage[m_nSaveIdx].m_FashionFrame = FashionFrame;
                    m_xAfterImage[m_nSaveIdx].m_HelmetFrame = HelmetFrame;
                    m_xAfterImage[m_nSaveIdx].m_HairFrame = HairFrame;
                    m_xAfterImage[m_nSaveIdx].m_HorseFrame = HorseFrame;
                    m_xAfterImage[m_nSaveIdx].m_Direction = Direction;
                    m_xAfterImage[m_nSaveIdx].m_CurrentAnimation = CurrentAnimation;
                    m_xAfterImage[m_nSaveIdx].m_DrawFrame = DrawFrame;
                    m_xAfterImage[m_nSaveIdx].DrawTime = CEnvir.Now;

                    m_dwAfterTimer = CEnvir.Now.AddMilliseconds(30);

                    if (m_bSetIdx == false)
                    {
                        if (m_nSaveIdx == 6)
                        {
                            m_nPlayIdx1 = 3;
                            m_nPlayIdx2 = 0;
                            m_bSetIdx = true;
                        }
                    }

                    if (m_bSetIdx == true)
                    {
                        m_nPlayIdx1++;
                        m_nPlayIdx2++;
                    }

                    m_nSaveIdx++;

                    if (m_nSaveIdx >= 10)
                        m_nSaveIdx = 0;

                    if (m_nPlayIdx1 >= 10)
                        m_nPlayIdx1 = 0;

                    if (m_nPlayIdx2 >= 10)
                        m_nPlayIdx2 = 0;
                }

                DrawAfterBody(2);
                DrawAfterBody(1);
            }
            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(DXManager.ScratchSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Empty, 0, 0);
            //DXManager.Sprite.Flush();

            int l = int.MaxValue, t = int.MaxValue, r = int.MinValue, b = int.MinValue;

            MirImage image;

            //竞技场只画布衣
            if (GameScene.Game.MapControl.MapInfo.CanPlayName)
            {
                image = BodyLibrary?.GetImage(ArmourFrame);  //画布衣
                if (image != null)
                {
                    BodyLibrary.Draw(ArmourFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                    if (ArmourColour.ToArgb() != 0)   //自定义颜色
                        BodyLibrary.Draw(ArmourFrame, DrawX, DrawY, ArmourColour, true, 1F, ImageType.Overlay);

                    l = Math.Min(l, DrawX + image.OffSetX);
                    t = Math.Min(t, DrawY + image.OffSetY);
                    r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                    b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                }
            }
            else
            {
                //武器 --shape 0 开始， -1 代表没装备武器
                //如果不显示时装并且装备了武器 或者 是钓鱼服 或者 显示时装并且时装不在自带武器衣服列表中  就画武器  有些时装是自带武器的
                if ((FashionShape < 0 && WeaponShape >= 0) || ArmourShape == 16 || (FashionShape >= 0 && !Globals.ArmourWithWeaponList.Contains(FashionImage)))
                {
                    switch (Direction)
                    {
                        case MirDirection.Up:
                        case MirDirection.DownLeft:
                        case MirDirection.Left:
                        case MirDirection.UpLeft:
                            if (!DrawWeapon) break;
                            image = WeaponLibrary1?.GetImage(WeaponFrame);
                            if (image == null) break;

                            WeaponLibrary1.Draw(WeaponFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            //System.Diagnostics.Debug.Write($"Animation = {Functions.GetEnumDescription(CurrentAnimation)}， WeaponFrame = {WeaponFrame}, Mir2DrawFrame = {Mir2DrawFrame}, DrawFrame = {DrawFrame}\n");

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            break;
                        default:
                            if (!DrawWeapon) break;
                            image = WeaponLibrary2?.GetImage(WeaponFrame);
                            if (image == null) break;

                            WeaponLibrary2.Draw(WeaponFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            break;
                    }
                }

                //盾牌 -- shape 1 开始， 0 代表没装备盾牌
                //如果显示盾牌 并且 盾牌Shape 小于 1000 就画盾牌
                if (ShieldShape > 0 && ShieldShape < 1000)
                    switch (Direction)       //方向      
                    {
                        case MirDirection.UpRight: //向上向左
                        case MirDirection.Right:   //向右
                        case MirDirection.DownRight: //右下
                            image = ShieldLibrary?.GetImage(ShieldFrame);  //图片= 盾图库  获取图像（盾框架）
                            if (image != null)  //如果  图片 不为 零
                            {
                                //盾图库 绘制（盾框架 绘制X 绘制Y 颜色 白色  透明度  图片类型.图片）
                                ShieldLibrary.Draw(ShieldFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                                l = Math.Min(l, DrawX + image.OffSetX);
                                t = Math.Min(t, DrawY + image.OffSetY);
                                r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                                b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            }
                            break;
                    }

                //衣服 -- shape 0 开始， 0  代表裸体（注意衣服素材是11为一组文件）
                //时装 -- shape 0 开始， -1 代表没装备时装
                //如果显示时装 并且 不是钓鱼服 就画时装
                if (FashionShape >= 0 && ArmourShape != 16)
                {
                    image = FashionLibrary?.GetImage(FashionFrame);  //画时装
                    if (image != null)
                    {
                        FashionLibrary.Draw(FashionFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                        l = Math.Min(l, DrawX + image.OffSetX);
                        t = Math.Min(t, DrawY + image.OffSetY);
                        r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                        b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                    }
                }
                //如果不显示时装 就画衣服
                else
                {
                    image = BodyLibrary?.GetImage(ArmourFrame);  //画衣服

                    if (image != null)
                    {
                        if (image != null)
                        {
                            BodyLibrary.Draw(ArmourFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            if (ArmourColour.ToArgb() != 0)   //自定义颜色
                                BodyLibrary.Draw(ArmourFrame, DrawX, DrawY, ArmourColour, true, 1F, ImageType.Overlay);

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                        }
                        else
                        {
                            BodyLibrary.Draw(ArmourFrame1, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                        }
                    }
                }

                //时装外观和钓鱼服外观一律不显示头盔和头发
                if (FashionShape < 0 || FashionShape > 10)
                {
                    //头盔 -- shape 1 开始， 0 代表没装备头盔  
                    //如果显示头盔 并且 不是钓鱼服 就画头盔
                    if (HelmetShape > 0 && ArmourShape != 16)
                    {
                        image = HelmetLibrary?.GetImage(HelmetFrame);
                        if (image != null)
                        {
                            HelmetLibrary.Draw(HelmetFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                        }
                    }
                    //如果不显示头盔 就画发型
                    else
                    {
                        if (HairType > 0 && ArmourShape != 16)
                        {
                            image = HairLibrary.GetImage(HairFrame);
                            if (image != null)
                            {
                                HairLibrary.Draw(HairFrame, DrawX, DrawY, HairColour, true, 1F, ImageType.Image);

                                l = Math.Min(l, DrawX + image.OffSetX);
                                t = Math.Min(t, DrawY + image.OffSetY);
                                r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                                b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            }
                        }
                    }
                }

                //武器 --shape 0 开始， -1 代表没装备武器
                //如果不显示时装并且装备了武器 或者 显示时装并且时装shape小于等于5  就画武器  有些时装是自带武器的
                if ((FashionShape < 0 && WeaponShape >= 0) || ArmourShape == 16 || (FashionShape >= 0 && !Globals.ArmourWithWeaponList.Contains(FashionImage)))
                {
                    switch (Direction)   //画武器
                    {
                        case MirDirection.UpRight:  //向上向左
                        case MirDirection.Right: //向右
                        case MirDirection.DownRight: //右下
                        case MirDirection.Down: // 向下
                            if (!DrawWeapon) break;  //如果 不是绘制武器 那么间断
                            image = WeaponLibrary1?.GetImage(WeaponFrame);  //图片= 武器图库 获取图像（武器框架）
                            if (image == null) break; //如果 图片为零 那么间断

                            WeaponLibrary1.Draw(WeaponFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            break;
                        default:
                            if (!DrawWeapon) break;   //如果 不是绘制武器 那么间断
                            image = WeaponLibrary2?.GetImage(WeaponFrame);
                            if (image == null) break;

                            WeaponLibrary2.Draw(WeaponFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                            l = Math.Min(l, DrawX + image.OffSetX);
                            t = Math.Min(t, DrawY + image.OffSetY);
                            r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                            b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            break;
                    }
                }

                //盾牌 -- shape 1 开始， 0 代表没装备盾牌
                //如果显示盾牌 并且 盾牌Shape 小于 1000 并且 不是钓鱼服 并且 不显示时装 就画盾牌
                if (ShieldShape > 0 && ShieldShape < 1000)
                    switch (Direction)
                    {
                        case MirDirection.Up:
                        case MirDirection.Down:
                        case MirDirection.DownLeft:
                        case MirDirection.Left:
                        case MirDirection.UpLeft:
                            image = ShieldLibrary?.GetImage(ShieldFrame);
                            if (image != null)
                            {
                                ShieldLibrary.Draw(ShieldFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);

                                l = Math.Min(l, DrawX + image.OffSetX);
                                t = Math.Min(t, DrawY + image.OffSetY);
                                r = Math.Max(r, image.Width + DrawX + image.OffSetX);
                                b = Math.Max(b, image.Height + DrawY + image.OffSetY);
                            }
                            break;
                    }
            }

            DXManager.SetSurface(oldSurface);
            float oldOpacity = DXManager.Opacity;

            if (shadow)
            {
                switch (CurrentAnimation)
                {
                    case MirAnimation.HorseStanding:
                    case MirAnimation.HorseWalking:
                    case MirAnimation.HorseRunning:
                    case MirAnimation.HorseStruck:
                        float opacity = 0.5f;
                        if (VisibleBuffs.Contains(BuffType.Invisibility) || VisibleBuffs.Contains(BuffType.Transparency) || VisibleBuffs.Contains(BuffType.Cloak))
                        {
                            opacity = 0.2f;
                        }
                        HorseLibrary?.Draw(HorseFrame, DrawX, DrawY, Color.Black, true, opacity, ImageType.Shadow);
                        break;
                    default:
                        DrawShadow2(l, t, r, b);
                        break;
                }
            }

            if (oldOpacity != Opacity && !DXManager.Blending) DXManager.SetOpacity(Opacity);

            switch (CurrentAnimation)  //当前骑马动画
            {
                case MirAnimation.HorseStanding:
                case MirAnimation.HorseWalking:
                case MirAnimation.HorseRunning:
                case MirAnimation.HorseStruck:

                    switch (HorseShape)
                    {
                        case 0:
                            HorseLibrary?.Draw(HorseFrame, DrawX, DrawY, Color.White, true, Opacity, ImageType.Image);
                            break;
                        case 1:
                        case 2:
                        case 3:
                            HorseShapeLibrary?.Draw(HorseFrame, DrawX, DrawY, Color.White, true, Opacity, ImageType.Image);
                            break;
                        case 4:
                            HorseShapeLibrary?.Draw(DrawFrame, DrawX, DrawY, Color.White, true, Opacity, ImageType.Image);
                            break;
                        case 5:
                            HorseShapeLibrary?.Draw(DrawFrame, DrawX, DrawY, Color.White, true, Opacity, ImageType.Image);
                            if (shadow)
                                HorseShapeLibrary2?.DrawBlend(DrawFrame, DrawX, DrawY, Color.White, true, Opacity, ImageType.Image);
                            break;
                        default:
                            if ((HorseShape > 5) && (HorseShape < 26))
                                HorseShapeLibrary?.Draw(HorseFrame, DrawX, DrawY, Color.White, true, Opacity, ImageType.Image);
                            break;
                    }
                    break;
            }

            DXManager.Sprite.Draw(DXManager.ScratchTexture, Rectangle.FromLTRB(l, t, r, b), Vector2.Zero, new Vector2(l, t), DrawColour);
            CEnvir.DPSCounter++;
            if (oldOpacity != Opacity && !DXManager.Blending) DXManager.SetOpacity(oldOpacity);

        }
        /// <summary>
        /// 绘制阴影
        /// </summary>
        /// <param name="l"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        public void DrawShadow2(int l, int t, int r, int b)
        {
            float opacity = 0.5f;
            if (VisibleBuffs.Contains(BuffType.Invisibility) || VisibleBuffs.Contains(BuffType.Transparency) || VisibleBuffs.Contains(BuffType.Cloak))
            {
                opacity = 0.2f;
            }

            MirImage image = BodyLibrary?.GetImage(ArmourFrame);

            if (image == null) return;

            Vector2 position = Vector2.Zero;
            if (CurrentAction == MirAction.Dead)
            {
                switch (Direction)
                {
                    case MirDirection.Right:
                        position = new Vector2(DrawX + image.ShadowOffSetX - 27, DrawY + image.ShadowOffSetY - 18);
                        break;
                    case MirDirection.DownRight:
                        position = new Vector2(DrawX + image.ShadowOffSetX - 9, DrawY + image.ShadowOffSetY - 18);
                        break;
                    case MirDirection.Down:
                        position = new Vector2(DrawX + image.ShadowOffSetX, DrawY + image.ShadowOffSetY - 36);
                        break;
                    case MirDirection.DownLeft:
                        position = new Vector2(DrawX + image.ShadowOffSetX, DrawY + image.ShadowOffSetY - 27);
                        break;
                    default:
                        position = new Vector2(DrawX + image.ShadowOffSetX, DrawY + image.ShadowOffSetY - 18);
                        break;
                }
            }
            else
            {
                int w = (DrawX + image.OffSetX) - l;
                int h = (DrawY + image.OffSetY) - t;
                Matrix m = Matrix.CreateScale(1F, 0.5f, 0);

                m.M21 = -0.50F;
                DXManager.Sprite.Transform = m * Matrix.CreateTranslation(DrawX + image.ShadowOffSetX - w + (image.Height) / 2 + h / 2, DrawY + image.ShadowOffSetY - h / 2, 0);
            }
            DXManager.Sprite.samplerState = null;

            float oldOpacity = DXManager.Opacity;
            DXManager.SetOpacity(opacity);
            DXManager.Sprite.Draw(DXManager.ScratchTexture, Rectangle.FromLTRB(l, t, r, b), Vector2.Zero, position, Color.Black);

            DXManager.Sprite.Transform = Matrix.Identity;
            DXManager.Sprite.samplerState = SamplerState.PointClamp;

            DXManager.SetOpacity(oldOpacity);

        }
        /// <summary>
        /// 绘制血条
        /// </summary>
        public override void DrawHealth()
        {
            int cy = 62;

            //没有玩家对象数据直接跳出
            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(ObjectID, out data)) return;

            //如果地图设置为PK场，就无法查看血条和数字显血
            if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;

            //如果辅助没开启数字显血 且血量不为0
            if (BigPatchConfig.ChkShowHPBar && data.MaxHealth > 0)
            {
                HPratioLabel.Text = $"{Math.Max(0, data.Health)}/{data.MaxHealth}";
                int x = DrawX + (48 - HPratioLabel.Size.Width) / 2;
                int y = DrawY - 58 - HPratioLabel.Size.Height;
                HPratioLabel.Location = new Point(x, y);
                HPratioLabel.Draw();
            }

            if (!BigPatchConfig.ShowHealth) return;

            //如果辅助未显血 并且 不是盟友 并且 用户bufxxx 不显示血量
            if (!BigPatchConfig.ShowHealth && !GameScene.Game.IsAlly(ObjectID) && User.Buffs.All(x => x.Type != BuffType.Developer)) return;  //辅助设置显血

            if (this == User && User.Buffs.Any(x => x.Type == BuffType.SuperiorMagicShield))
            {
                MirLibrary library;

                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out library)) return;

                float percent = Math.Min(1, Math.Max(0, User.Buffs.First(x => x.Type == BuffType.SuperiorMagicShield).Stats[Stat.SuperiorMagicShield] / (float)User.MaximumSuperiorMagicShield));

                if (percent == 0) return;

                Size size = library.GetSize(79);

                Color color = Color.Goldenrod;

                library.Draw(80, DrawX, DrawY - 59, Color.White, false, 1F, ImageType.Image, zoomRate: CEnvir.UIScale, uiOffsetX: CEnvir.UI_Offset_X);
                library.Draw(79, DrawX + 1, DrawY - 59 + 1, color, new Rectangle(0, 0, (int)(size.Width * percent), size.Height), 1F, ImageType.Image, zoomRate: CEnvir.UIScale, uiOffsetX: CEnvir.UI_Offset_X);
            }

            if (data.MaxHealth > 0)
            {
                MirLibrary library;

                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out library)) return;

                float percent = Math.Min(1, Math.Max(0, data.Health / (float)data.MaxHealth));

                //if (percent == 0) return;

                Size size = library.GetSize(79);

                Color color = Color.OrangeRed;

                //只画红不画蓝
                if (this != User && (!BigPatchConfig.ShowHealth || !GameScene.Game.IsAlly(ObjectID) || data.MaxMana < 0))
                {
                    cy = 58;
                }

                library.Draw(80, DrawX, DrawY - cy, Color.White, false, 1F, ImageType.Image, zoomRate: CEnvir.UIScale, uiOffsetX: CEnvir.UI_Offset_X);
                library.Draw(79, DrawX + 1, DrawY - cy + 1, color, new Rectangle(0, 0, (int)(size.Width * percent), size.Height), 1F, ImageType.Image, zoomRate: CEnvir.UIScale, uiOffsetX: CEnvir.UI_Offset_X);
            }

            //如果辅助显示血量 && 不是盟友不显示血量
            if (BigPatchConfig.ShowHealth && !GameScene.Game.IsAlly(ObjectID)) return;

            if (data.MaxMana > 0)
            {
                MirLibrary library;

                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.Interface, out library)) return;


                float percent = Math.Min(1, Math.Max(0, data.Mana / (float)data.MaxMana));

                //if (percent == 0) return;

                Size size = library.GetSize(79);

                Color color = Color.DodgerBlue;

                library.Draw(80, DrawX, DrawY - 58, Color.White, false, 1F, ImageType.Image, zoomRate: CEnvir.UIScale, uiOffsetX: CEnvir.UI_Offset_X);
                library.Draw(79, DrawX + 1, DrawY - 58 + 1, color, new Rectangle(0, 0, (int)(size.Width * percent), size.Height), 1F, ImageType.Image, zoomRate: CEnvir.UIScale, uiOffsetX: CEnvir.UI_Offset_X);
            }
        }
        /// <summary>
        /// 绘制翅膀
        /// </summary>
        public void DrawWings()
        {
            if (!Config.DrawEffects) return;

            switch (CurrentAction)
            {
                case MirAction.Die:
                case MirAction.Dead:
                    break;
                default:
                    MirLibrary library;

                    switch (ArmourImage)
                    {
                        //全部
                        case 966:
                        case 976:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(0 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 944:
                        case 954:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(10000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 5345:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx1, out library)) return;
                            library.DrawBlend(10000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 3342:
                        case 3352:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx3, out library)) return;
                            library.DrawBlend(0 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 3325:
                        case 3335:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx2, out library)) return;
                            library.DrawBlend(0 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 947:
                        case 957:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_FullEx1, out library)) return;
                            library.DrawBlend(0 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 3321:
                        case 3331:
                        case 3341:
                        case 3351:
                        case 3361:
                        case 3371:
                        case 3381:
                        case 3391:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(90000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        //战士
                        case 963:
                        case 973:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                            library.DrawBlend(400 + (GameScene.Game.MapControl.Animation / 2) % 15 + (int)Direction * 20, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            // 400 - 414 // 420 ....
                            break;
                        case 3320:
                        case 3330:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(50000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        //法师
                        case 964:
                        case 974:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                            library.DrawBlend(200 + (GameScene.Game.MapControl.Animation / 2) % 15 + (int)Direction * 20, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            // 200 - 214 // 220 ....
                            break;
                        case 3340:
                        case 3350:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(60000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        //道士
                        case 965:
                        case 975:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                            library.DrawBlend(0 + (GameScene.Game.MapControl.Animation / 2) % 15 + (int)Direction * 20, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            // 000 - 014 // 020 ....
                            break;
                        case 3360:
                        case 3370:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(70000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        //刺客
                        case 2007:
                        case 2017:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                            library.DrawBlend(600 + (GameScene.Game.MapControl.Animation / 2) % 13 + (int)Direction * 20, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            // 600 - 614 // 620 ....
                            break;
                        case 3380:
                        case 3390:
                            if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;
                            library.DrawBlend(80000 + ((byte)Gender * 5000) + DrawFrame, DrawX, DrawY, Color.White, true, 1f, ImageType.Image);
                            break;
                        case 986:
                        case 996:
                            if (User.Class == MirClass.Warrior)
                            {
                                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                                library.DrawBlend(800 + (GameScene.Game.MapControl.Animation / 2) % 13, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            }
                            if (User.Class == MirClass.Wizard)
                            {
                                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                                library.DrawBlend(820 + (GameScene.Game.MapControl.Animation / 2) % 13, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            }
                            if (User.Class == MirClass.Taoist)
                            {
                                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                                library.DrawBlend(840 + (GameScene.Game.MapControl.Animation / 2) % 13, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            }
                            break;
                        default:
                            //画自定义外观特效
                            DrawCustomOuterEffect(ArmourIndex);
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 绘制武器特效
        /// </summary>
        public void DrawWeaponEffect()
        {
            if (!GameScene.Game.CharacterBox.ShowFashionBox.Checked || FashionShape <= 5)
            {
                if (!Config.DrawEffects) return;

                switch (CurrentAction)
                {
                    case MirAction.Die:
                    case MirAction.Dead:
                        break;
                    default:
                        MirLibrary library;

                        if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Full, out library)) return;

                        switch (WeaponImage)
                        {
                            //战士/法师/道士
                            case 1076: //影魅之刃
                                library.DrawBlend(40000 + 5000 * (byte)Gender + DrawFrame, DrawX, DrawY, Color.White, true, 0.8f, ImageType.Image);
                                break;

                            //刺客
                            case 2550:
                                library.DrawBlend(20000 + 5000 * (byte)Gender + DrawFrame, DrawX, DrawY, Color.White, true, 0.8f, ImageType.Image);
                                break;
                            default:
                                //画自定义外观特效
                                DrawCustomOuterEffect(WeaponIndex);
                                break;
                        }
                        break;
                }
            }
        }
        /// <summary>
        /// 绘制盾牌特效
        /// </summary>
        public void DrawShieldEffect()
        {
            if (!Config.DrawEffects) return;

            switch (CurrentAction)
            {
                case MirAction.Die:
                case MirAction.Dead:
                    break;
                default:
                    if (ShieldShape >= 1000)
                    {
                        ShieldLibrary.DrawBlend(ShieldFrame + 100, DrawX, DrawY, Color.White, true, 0.8f, ImageType.Image);
                        ShieldLibrary.Draw(ShieldFrame, DrawX, DrawY, Color.White, true, 1F, ImageType.Image);
                    }

                    break;
            }
        }
        /// <summary>
        /// 绘制徽章特效
        /// </summary>
        public void DrawEmblemEffect()
        {
            if (!Config.DrawEffects) return;

            if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;

            switch (CurrentAction)
            {
                case MirAction.Die:
                case MirAction.Dead:
                    break;
                default:
                    MirLibrary library;

                    if (Horse == HorseType.None && HorseType == HorseType.DiyHorse2)   //坐骑增加对应光效
                    {
                        if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                        library.DrawBlend(2000 + (GameScene.Game.MapControl.Animation / 2) % 12, DrawX - 50, DrawY - 25, Color.White, true, 1f, ImageType.Image);
                        //break;
                    }

                    if (Horse == HorseType.None && HorseType == HorseType.DiyHorse14)   //坐骑增加对应光效
                    {
                        if (!CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_Part, out library)) return;
                        library.DrawBlend(2320 + (GameScene.Game.MapControl.Animation / 2) % 12, DrawX - 68, DrawY - 33, Color.White, true, 1f, ImageType.Image);
                        //break;
                    }

                    if (EmblemLibrary == null) return;

                    switch (EmblemShape)       //徽章道具特效
                    {
                        //八卦徽章
                        case 1:
                            EmblemLibrary.DrawBlend(500 + (GameScene.Game.MapControl.Animation) % 40, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            break;

                        //至尊徽章
                        case 2:
                            EmblemLibrary.DrawBlend(3800 + (GameScene.Game.MapControl.Animation) % 32, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            EmblemLibrary.DrawBlend(3850 + (GameScene.Game.MapControl.Animation) % 28, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            break;

                        //无极徽章
                        case 3:
                            EmblemLibrary.DrawBlend(640 + (GameScene.Game.MapControl.Animation) % 33, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            EmblemLibrary.DrawBlend(600 + (GameScene.Game.MapControl.Animation) % 28, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            break;
                        //泰尚徽章
                        case 4:
                            EmblemLibrary.DrawBlend(200 + (GameScene.Game.MapControl.Animation) % 28, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            EmblemLibrary.DrawBlend(260 + (GameScene.Game.MapControl.Animation) % 20, DrawX, DrawY, Color.White, true, 0.7f, ImageType.Image);
                            break;
                    }
                    break;
            }
        }
        /// <summary>
        /// 鼠标悬停
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public override bool MouseOver(Point p)
        {
            if (BodyLibrary != null && BodyLibrary.VisiblePixel(ArmourFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            //竞技场只显示布衣
            if (GameScene.Game.MapControl.MapInfo.CanPlayName) return false;

            if (HairType >= 0 && HairLibrary != null && HairLibrary.VisiblePixel(HairFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            //头盔
            if (HelmetShape >= 0 && HelmetLibrary != null && HelmetLibrary.VisiblePixel(HelmetFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            if (LibraryWeaponShape >= 0 && WeaponLibrary1 != null && WeaponLibrary1.VisiblePixel(WeaponFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            if (LibraryWeaponShape >= 0 && WeaponLibrary2 != null && WeaponLibrary2.VisiblePixel(WeaponFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            //时装
            if (FashionLibrary != null && FashionLibrary.VisiblePixel(FashionFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            //盾牌
            if (ShieldShape >= 0 && ShieldLibrary != null && ShieldLibrary.VisiblePixel(ShieldFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                return true;

            switch (CurrentAnimation)
            {
                case MirAnimation.HorseStanding:
                case MirAnimation.HorseWalking:
                case MirAnimation.HorseRunning:
                case MirAnimation.HorseStruck:
                    if (HorseLibrary != null && HorseLibrary.VisiblePixel(HorseFrame, new Point(p.X - DrawX, p.Y - DrawY), false, true))
                        return true;
                    break;
            }
            return false;
        }

        public override void OnRemoved()
        {
        }

        /// <summary>
        /// 播放击中声效
        /// </summary>
        public override void PlayStruckSound()
        {
            DXSoundManager.Play(Gender == MirGender.Male ? SoundIndex.MaleStruck : SoundIndex.FemaleStruck);
            DXSoundManager.Play(SoundIndex.GenericStruckPlayer);
        }
        /// <summary>
        /// 播放死亡声效
        /// </summary>
        public override void PlayDieSound()
        {
            DXSoundManager.Play(Gender == MirGender.Male ? SoundIndex.MaleDie : SoundIndex.FemaleDie);
        }
        /// <summary>
        /// 播放攻击声效
        /// </summary>
        public override void PlayAttackSound()
        {
            if (Class != MirClass.Assassin)
                PlayCommonSounds();
            else
                PlayAssassinSounds();
        }
        /// <summary>
        /// 播放刺客声效
        /// </summary>
        private void PlayAssassinSounds()
        {

            if (LibraryWeaponShape >= 1200)
                DXSoundManager.Play(SoundIndex.ClawAttack);
            else if (LibraryWeaponShape >= 1100)
                DXSoundManager.Play(SoundIndex.GlaiveAttack);
            else
                PlayCommonSounds();
        }
        /// <summary>
        /// 播放常见声效
        /// </summary>
        private void PlayCommonSounds()
        {
            switch (LibraryWeaponShape)
            {
                case 100:
                    DXSoundManager.Play(SoundIndex.WandSwing);
                    break;
                case 9:
                case 101:
                    DXSoundManager.Play(SoundIndex.WoodSwing);
                    break;
                case 102:
                    DXSoundManager.Play(SoundIndex.AxeSwing);
                    break;
                case 103:
                    DXSoundManager.Play(SoundIndex.DaggerSwing);
                    break;
                case 104:
                    DXSoundManager.Play(SoundIndex.ShortSwordSwing);
                    break;
                case 26:
                case 105:
                    DXSoundManager.Play(SoundIndex.IronSwordSwing);
                    break;
                default:
                    DXSoundManager.Play(SoundIndex.FistSwing);
                    break;
            }
        }
    }
}
