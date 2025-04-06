using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 韩版角色属性面板
    /// </summary>
    public sealed class CharacterDialog : DXWindow
    {
        #region Properties
        public DXImageControl CharacterTabBackGround, MakeTabBackGround;  //主面板 角色 制造

        public DXButton MakePage;  //角色按钮 制造按钮
        public DXButton MakeAButton, OPPMekDialogButton;
        public DXLabel CraftLevelLabel, CraftExpLabel, ItemNameLabel, SuccessRateLabel;
        public DXItemCell TargetItemCell;
        public DXLabel Name1, Name2, Name3, Name4, Name5;
        public DXLabel Amount1, Amount2, Amount3, Amount4, Amount5;

        public DXLabel CharacterNameLabel, GuildNameLabel, GuildRankLabel, MarriageLabel, MarriageIconLabel;  //名字 行会名字 行会登记 结婚 结婚图标 标签

        public DXLabel LevelLabel, ExceptionLabel, HealthLabel, ManaLabel; //等级 经验 角色   血  蓝
        public DXLabel PackLabel, WearWeightLabel, HandWeightLabel; //背包负重 穿戴 腕力
        public DXLabel AccuracyLabel, AgilityLabel;    //准确 敏捷   

        public DXImageControl MarriageIcon;  //结婚图标
        public DXButton Close1Button;
        public DXItemCell[] Grid;   //装备道具格子

        public DXImageControl CharacterPage;
        /// <summary>
        /// 显示头盔
        /// </summary>
        public DXCheckBox ShowHelmetBox;
        /// <summary>
        /// 显示盾牌
        /// </summary>
        public DXCheckBox ShowShieldBox;
        /// <summary>
        /// 显示时装
        /// </summary>
        public DXCheckBox ShowFashionBox;
        public Dictionary<Stat, DXLabel> DisplayStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> AttackStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> AdvantageStats = new Dictionary<Stat, DXLabel>();
        public Dictionary<Stat, DXLabel> DisadvantageStats = new Dictionary<Stat, DXLabel>();

        public DXImageControl Fireicon, Iceicon, Lightningicon, Windicon, Holyicon, Darkicon, Phantomicon;
        public DXImageControl StrongFireicon, StrongIceicon, StrongLightningicon, StrongWindicon, StrongHolyicon, StrongDarkicon, StrongPhantomicon;
        public DXImageControl WeakFireicon, WeakIceicon, WeakLightningicon, WeakWindicon, WeakHolyicon, WeakDarkicon, WeakPhantomicon;

        public int FashionShape;  //时装

        public override WindowType Type => WindowType.CharacterBox;
        public override bool CustomSize => false;  //自定义大小
        public override bool AutomaticVisibility => false;  //自动可见性
        #endregion

        /// <summary>
        /// 韩版角色属性面板
        /// </summary>
        public CharacterDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size = new Size(328, 487);
            Location = ClientArea.Location;

            CharacterTabBackGround = new DXImageControl   //角色
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1280,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };
            CharacterTabBackGround.AfterDraw += Character_AfterDraw;

            MakeTabBackGround = new DXImageControl      //制造
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1279,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = false,
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(288, 10)
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            CharacterPage = new DXImageControl
            {
                Index = 1537,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(220, 337),
                Parent = CharacterTabBackGround,
                ZoomSize = new Size(32, 24),   //缩放图片
                Zoom = true,
                Hint = "打开制造界面".Lang(),
            };
            CharacterPage.MouseClick += (o, e) =>
            {
                CharacterTabBackGround.Visible = false;
                MakeTabBackGround.Visible = true;
            };

            MakePage = new DXButton
            {
                Index = 1285,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(15, 19),
                Parent = MakeTabBackGround,
                Hint = "返回角色界面".Lang(),
            };
            MakePage.MouseClick += (o, e) =>
            {
                CharacterTabBackGround.Visible = true;
                MakeTabBackGround.Visible = false;
            };


            #region 制作
            MakeAButton = new DXButton //制作按钮
            {
                Index = 6242,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(30, 445),
                Parent = MakeTabBackGround,
            };
            MakeAButton.MouseClick += (sender, args) =>
            {
                GameScene.Game.CraftResultBox.Visible = true;
                if (GameScene.Game.User.BookmarkedCraftItemInfo == null) return;
                CEnvir.Enqueue(new C.CraftStart { TargetItemIndex = GameScene.Game.User.BookmarkedCraftItemInfo.Index });
            };

            OPPMekDialogButton = new DXButton //打开制作列表按钮
            {
                Index = 6247,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(190, 445),
                Parent = MakeTabBackGround,
            };
            OPPMekDialogButton.MouseClick += (sender, args) => { GameScene.Game.CraftBox.Visible = true; };

            CraftLevelLabel = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                //Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(45, 135),
                Text = "0",
            };

            CraftExpLabel = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                //Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(225, 135),
                Text = "0/???"
            };

            TargetItemCell = new DXItemCell
            {
                Parent = MakeTabBackGround,
                Location = new Point(30, 205),
                FixedBorder = true,
                Border = true,
                ReadOnly = true,
                ItemGrid = new ClientUserItem[1],
                Slot = 0,
                FixedBorderColour = true,
                Visible = true,
            };

            ItemNameLabel = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.Gold,
                Outline = false,
                Parent = MakeTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(80, 205),
                Text = "空白物品".Lang(),
                Visible = false,
            };

            SuccessRateLabel = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                //Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(80, 225),
                Text = "制作成功率".Lang() + ": 100 %",
                Visible = false,
            };
            Name1 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(60, 290),
                Text = "原料1".Lang(),
                Visible = false,
            };

            Amount1 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(220, 290),
                Text = "0/100",
                Visible = false,
            };

            Name2 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(60, 320),
                Text = "原料2".Lang(),
                Visible = false,
            };

            Amount2 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(220, 320),
                Text = "0/100",
                Visible = false,
            };

            Name3 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(60, 350),
                Text = "原料3".Lang(),
                Visible = false,
            };

            Amount3 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(220, 350),
                Text = "0/100",
                Visible = false,
            };

            Name4 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(60, 380),
                Text = "原料4".Lang(),
                Visible = false,
            };

            Amount4 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(220, 380),
                Text = "0/100",
                Visible = false,
            };

            Name5 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(60, 410),
                Text = "原料5".Lang(),
                Visible = false,
            };

            Amount5 = new DXLabel
            {
                AutoSize = true,
                ForeColour = Color.White,
                Outline = false,
                Parent = MakeTabBackGround,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(220, 410),
                Text = "0/100",
                Visible = false,
            };

            #endregion

            #region 基本信息
            DXControl namePanel = new DXControl
            {
                Parent = this,
                Size = new Size(150, 45),
                Location = new Point((Size.Width - 150) / 2, 15),
            };
            // 角色名
            CharacterNameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(150, 20),
                ForeColour = Color.FromArgb(222, 255, 222),
                Outline = false,
                Parent = namePanel,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildNameLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(150, 15),
                ForeColour = Color.FromArgb(255, 255, 181),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height - 2),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildRankLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(150, 15),
                ForeColour = Color.FromArgb(255, 206, 148),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height + GuildNameLabel.Size.Height - 4),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            MarriageLabel = new DXLabel      //结婚对象名字外框
            {
                AutoSize = false,
                ForeColour = Color.Magenta,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = CharacterTabBackGround,
                Location = new Point((Size.Width + 8) / 2 + 25, 63),
                Size = new Size(110, 15),
                IsControl = false,
            };
            MarriageIconLabel = new DXLabel   //结婚图标外框
            {
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = CharacterTabBackGround,
                Location = new Point((Size.Width + 8) / 2 + 11, 65),
                Size = new Size(15, 15),
                IsControl = false,
            };

            MarriageIcon = new DXImageControl   //结婚图标
            {
                Parent = MarriageIconLabel,
                LibraryFile = LibraryFile.GameInter,
                Index = 1298,
                Location = new Point(1, 1),
                Visible = false,
            };

            #endregion

            #region 人物装备
            Grid = new DXItemCell[Globals.EquipmentSize];

            DXItemCell cell, weaponCell, helmetCell, armourCell, necklaceCell;
            Grid[(int)EquipmentSlot.Weapon] = weaponCell = new DXItemCell(false)  //武器
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(40, 88),
                Parent = CharacterTabBackGround,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Weapon,
                GridType = GridType.Equipment,
                Size = new Size(55, 151),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell) o, 35);

            Grid[(int)EquipmentSlot.Helmet] = helmetCell = new DXItemCell(false)  //头盔
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(85, 88),
                Parent = CharacterTabBackGround,
                FixedBorder = true,
                Opacity = 0F,
                Border = false,
                Size = new Size(71, 70),
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Helmet,
                GridType = GridType.Equipment,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 37);

            Grid[(int)EquipmentSlot.Armour] = armourCell = new DXItemCell(false)  //衣服
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(80, 108),
                Parent = CharacterTabBackGround,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Armour,
                GridType = GridType.Equipment,
                Size = new Size(helmetCell.Size.Width, 135),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 34);

            Grid[(int)EquipmentSlot.Shield] = cell = new DXItemCell   //盾牌
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(130, 128),
                Size = new Size(40, armourCell.Size.Height),
                Parent = CharacterTabBackGround,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Shield,
                GridType = GridType.Equipment,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 105);

            Grid[(int)EquipmentSlot.Torch] = cell = new DXItemCell   //火把
            {
                Location = new Point(90, 265),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Torch,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 38);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Torch)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.Necklace] = necklaceCell = new DXItemCell  //项链
            {
                Location = new Point(169, 89),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                Border = false,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Necklace,
                GridType = GridType.Equipment,
            };
            necklaceCell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 33);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Necklace)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.BraceletL] = cell = new DXItemCell  //左手镯
            {
                Location = new Point(12, 155),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.BraceletL,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 32);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Bracelet)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.BraceletR] = cell = new DXItemCell  //右手镯
            {
                Location = new Point(170, 155),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.BraceletR,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 32);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Bracelet)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.RingL] = cell = new DXItemCell  //左戒指
            {
                Location = new Point(12, 195),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.RingL,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 31);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Ring)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.RingR] = cell = new DXItemCell   //右戒指
            {
                Location = new Point(170, 195),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.RingR,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 31);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Ring)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };


            Grid[(int)EquipmentSlot.Emblem] = cell = new DXItemCell  //徽章
            {
                Location = new Point(232, 42),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Emblem,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 33, LibraryFile.Interface, 0.3F);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Emblem)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };


            Grid[(int)EquipmentSlot.Shoes] = cell = new DXItemCell  //鞋子
            {
                LibraryFile = LibraryFile.Inventory,
                Location = new Point(12, 233),
                Size = new Size(necklaceCell.Size.Width, 72),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                Border = false,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Shoes,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 36);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Shoes)
                    Draw((DXItemCell)o, 1236, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.Poison] = cell = new DXItemCell  //毒
            {
                Location = new Point(170, 265),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Poison,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 40, LibraryFile.Interface, 0.3F);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Poison)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.Amulet] = cell = new DXItemCell  ///符
            {
                Location = new Point(130, 265),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Amulet,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 39);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Amulet || GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.DarkStone)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };


            Grid[(int)EquipmentSlot.HorseArmour] = cell = new DXItemCell  //马甲
            {
                Location = new Point(273, 42),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.HorseArmour,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 82, LibraryFile.Interface, 0.3F);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.HorseArmour)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.Flower] = cell = new DXItemCell  //花
            {
                Location = new Point(172, 265),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Flower,
                GridType = GridType.Equipment,
                Visible = false,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 81, LibraryFile.Interface, 0.3F);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Flower)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.FameTitle] = cell = new DXItemCell  //声望称号
            {
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(230 - 16, 15 - 16),
                Parent = CharacterTabBackGround,
                Size = new Size(76, 76),
                //FixedBorder = true,
                //Border = true,
                FixedBorderColour = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.FameTitle,
                GridType = GridType.Equipment,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 81);

            Grid[(int)EquipmentSlot.Fashion] = cell = new DXItemCell  //时装
            {
                Location = new Point(12, 116),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Fashion,
                GridType = GridType.Equipment,
            };
            cell.BeforeDraw += (o, e) =>
            {
                //Draw((DXItemCell)o, 82);

                if (GameScene.Game.SelectedCell?.Item.Info.ItemType == ItemType.Fashion)
                    Draw((DXItemCell)o, 1235, LibraryFile.UI1, 0.6f);
            };

            Grid[(int)EquipmentSlot.Medicament] = cell = new DXItemCell  //自动恢复药剂
            {
                Location = new Point(11, 17),
                Parent = CharacterTabBackGround,
                //FixedBorder = true,
                //Border = true,
                ItemGrid = GameScene.Game.Equipment,
                Slot = (int)EquipmentSlot.Medicament,
                GridType = GridType.Equipment,
            };

            #endregion

            #region 显示开关
            ShowHelmetBox = new DXCheckBox
            {
                Parent = CharacterTabBackGround,
                Hint = "显示头盔".Lang(),
                ReadOnly = true,
            };
            ShowHelmetBox.Location = new Point(87 - ShowHelmetBox.Size.Width, 35 - ShowHelmetBox.Size.Height);
            ShowHelmetBox.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.HelmetToggle { HideHelmet = ShowHelmetBox.Checked });
            };

            ShowShieldBox = new DXCheckBox
            {
                Parent = CharacterTabBackGround,
                Hint = "显示盾牌".Lang(),
                ReadOnly = true,
                Visible = false,
            };
            ShowShieldBox.Location = new Point(87 - ShowHelmetBox.Size.Width, 52 - ShowShieldBox.Size.Height);
            ShowShieldBox.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.ShieldToggle { HideShield = ShowShieldBox.Checked });
            };

            ShowFashionBox = new DXCheckBox
            {
                Parent = CharacterTabBackGround,
                Hint = "显示时装".Lang(),
                ReadOnly = true,
                Visible = false,
            };
            ShowFashionBox.Location = new Point(87 - ShowHelmetBox.Size.Width, 69 - ShowFashionBox.Size.Height);
            ShowFashionBox.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.FashionToggle { HideFashion = ShowFashionBox.Checked });
            };

            #endregion

            #region 属性详情
            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                ForeColour = Color.White,
                Location = new Point(225, 88),
                Size = new Size(100, 16),
                Hint = "等级".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            ExceptionLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                ForeColour = Color.White,
                Location = new Point(225, 113),
                Size = new Size(100, 16),
                Hint = "经验".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            HealthLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                ForeColour = Color.White,
                Location = new Point(225, 136),
                Size = new Size(100, 16),
                Hint = "生命HP".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            ManaLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                ForeColour = Color.White,
                Location = new Point(225, 161),
                Size = new Size(100, 16),
                Hint = "魔法MP".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            PackLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                ForeColour = Color.White,
                Location = new Point(225, 185),
                Size = new Size(100, 16),
                Hint = "包裹负重".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            WearWeightLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                Location = new Point(225, 210),
                Size = new Size(100, 16),
                ForeColour = Color.White,
                Text = "0",
                Hint = "穿戴负重".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            HandWeightLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                Location = new Point(225, 234),
                Size = new Size(100, 16),
                ForeColour = Color.White,
                Text = "0",
                Hint = "腕力".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            DisplayStats[Stat.Accuracy] = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                Location = new Point(225, 258),
                Size = new Size(100, 16),
                ForeColour = Color.White,
                Text = "0",
                Hint = "准确".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            DisplayStats[Stat.Agility] = new DXLabel
            {
                AutoSize = false,
                Parent = CharacterTabBackGround,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                Location = new Point(225, 282),
                Size = new Size(100, 16),
                ForeColour = Color.White,
                Text = "0",
                Hint = "敏捷".Lang(),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            //攻击
            DXLabel PHlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "破坏".Lang(),
            };
            PHlabel.Location = new Point(5, 318);

            DisplayStats[Stat.MaxDC] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(65, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(PHlabel.Size.Width + 5, 318),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0-0"
            };

            DXLabel FYlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "防御".Lang(),
            };
            FYlabel.Location = new Point(113, 318);

            DisplayStats[Stat.MaxAC] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(65, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(FYlabel.Size.Width + 92 + 20, 318),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0-0"
            };

            DXLabel ZRlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "自然".Lang(),
            };
            ZRlabel.Location = new Point(5, 346);

            DisplayStats[Stat.MaxMC] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(65, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(ZRlabel.Size.Width + 5, 346),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0-0"
            };

            DXLabel LHlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "灵魂".Lang(),
            };
            LHlabel.Location = new Point(113, 346);

            DisplayStats[Stat.MaxSC] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(65, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(LHlabel.Size.Width + 92 + 20, 346),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0-0"
            };

            DXLabel MYlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "魔御".Lang(),
            };
            MYlabel.Location = new Point(221, 346);

            DisplayStats[Stat.MaxMR] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(65, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(MYlabel.Size.Width + 198 + 20, 346),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "0-0"
            };

            //攻元素
            DXLabel GJYSlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "攻击元素".Lang(),
            };
            GJYSlabel.Location = new Point(5, 376);

            Fireicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1510,
                Visible = false,
                Hint = "火".Lang(),
            };
            Fireicon.Location = new Point(62, 372);

            Iceicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1511,
                Visible = false,
                Hint = "冰".Lang(),
            };
            Iceicon.Location = new Point(99, 372);

            Lightningicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1512,
                Visible = false,
                Hint = "雷".Lang(),
            };
            Lightningicon.Location = new Point(136, 372);

            Windicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1513,
                Visible = false,
                Hint = "风".Lang(),
            };
            Windicon.Location = new Point(173, 372);

            Holyicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1514,
                Visible = false,
                Hint = "神圣".Lang(),
            };
            Holyicon.Location = new Point(210, 372);

            Darkicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1515,
                Visible = false,
                Hint = "暗黑".Lang(),
            };
            Darkicon.Location = new Point(247, 372);

            Phantomicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                //Blend = true,
                Index = 1516,
                Visible = false,
                Hint = "幻影".Lang(),
            };
            Phantomicon.Location = new Point(284, 372);

            AttackStats[Stat.FireAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(80, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Fireicon,
            };

            AttackStats[Stat.IceAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(117, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Iceicon,
            };

            AttackStats[Stat.LightningAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(154, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Lightningicon,
            };

            AttackStats[Stat.WindAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(191, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Windicon,
            };

            AttackStats[Stat.HolyAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(228, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Holyicon,
            };

            AttackStats[Stat.DarkAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(265, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Darkicon,
            };

            AttackStats[Stat.PhantomAttack] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(302, 379),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = Phantomicon,
            };

            //强元素
            DXLabel QYSlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "强元素".Lang(),
            };
            QYSlabel.Location = new Point(12, 406);

            StrongFireicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1510,
                Visible = false,
                Hint = "火".Lang(),
            };
            StrongFireicon.Location = new Point(62, 404);

            StrongIceicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1511,
                Visible = false,
                Hint = "冰".Lang(),
            };
            StrongIceicon.Location = new Point(99, 404);

            StrongLightningicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1512,
                Visible = false,
                Hint = "雷".Lang(),
            };
            StrongLightningicon.Location = new Point(136, 404);

            StrongWindicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1513,
                Visible = false,
                Hint = "风".Lang(),
            };
            StrongWindicon.Location = new Point(173, 404);

            StrongHolyicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1514,
                Visible = false,
                Hint = "神圣".Lang(),
            };
            StrongHolyicon.Location = new Point(210, 404);

            StrongDarkicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1515,
                Visible = false,
                Hint = "暗黑".Lang(),
            };
            StrongDarkicon.Location = new Point(247, 404);

            StrongPhantomicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1516,
                Visible = false,
                Hint = "幻影".Lang(),
            };
            StrongPhantomicon.Location = new Point(284, 404);

            AdvantageStats[Stat.FireResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(80, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongFireicon,
            };

            AdvantageStats[Stat.IceResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(117, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongIceicon,
            };

            AdvantageStats[Stat.LightningResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(154, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongLightningicon,
            };

            AdvantageStats[Stat.WindResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(191, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongWindicon,
            };

            AdvantageStats[Stat.HolyResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(228, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongHolyicon,
            };

            AdvantageStats[Stat.DarkResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(265, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongDarkicon,
            };

            AdvantageStats[Stat.PhantomResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(302, 409),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = StrongPhantomicon,
            };

            //弱元素
            DXLabel LYSlabel = new DXLabel
            {
                Parent = CharacterTabBackGround,
                ForeColour = Color.White,
                Text = "弱元素".Lang(),
            };
            LYSlabel.Location = new Point(12, 436);

            WeakFireicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1510,
                Visible = false,
                Hint = "火".Lang(),
            };
            WeakFireicon.Location = new Point(62, 434);

            WeakIceicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1511,
                Visible = false,
                Hint = "冰".Lang(),
            };
            WeakIceicon.Location = new Point(99, 434);

            WeakLightningicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1512,
                Visible = false,
                Hint = "雷".Lang(),
            };
            WeakLightningicon.Location = new Point(136, 434);

            WeakWindicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1513,
                Visible = false,
                Hint = "风".Lang(),
            };
            WeakWindicon.Location = new Point(173, 434);

            WeakHolyicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1514,
                Visible = false,
                Hint = "神圣".Lang(),
            };
            WeakHolyicon.Location = new Point(210, 434);

            WeakDarkicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1515,
                Visible = false,
                Hint = "暗黑".Lang(),
            };
            WeakDarkicon.Location = new Point(247, 434);

            WeakPhantomicon = new DXImageControl
            {
                Parent = CharacterTabBackGround,
                LibraryFile = LibraryFile.UI1,
                Blend = true,
                Index = 1516,
                Visible = false,
                Hint = "幻影".Lang(),
            };
            WeakPhantomicon.Location = new Point(284, 434);

            DisadvantageStats[Stat.FireResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(80, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakFireicon,
            };

            DisadvantageStats[Stat.IceResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(117, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakIceicon,
            };

            DisadvantageStats[Stat.LightningResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(154, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakLightningicon,
            };

            DisadvantageStats[Stat.WindResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(191, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakWindicon,
            };

            DisadvantageStats[Stat.HolyResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(228, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakHolyicon,
            };

            DisadvantageStats[Stat.DarkResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(265, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakDarkicon,
            };

            DisadvantageStats[Stat.PhantomResistance] = new DXLabel
            {
                AutoSize = false,
                Size = new Size(20, 16),
                Parent = CharacterTabBackGround,
                Location = new Point(302, 439),
                ForeColour = Color.FromArgb(60, 60, 60),
                Text = "0",
                Tag = WeakPhantomicon,
            };
            #endregion
        }

        #region Methods	
        /// <summary>
        /// 更新收藏的制造物品信息
        /// </summary>
        public void UpdateBookmarkedCraftItem()
        {
            CraftItemInfo item = GameScene.Game.User.BookmarkedCraftItemInfo;
            if (item == null) return;
            TargetItemCell.Item = new ClientUserItem(item.Item, item.TargetAmount);
            TargetItemCell.RefreshItem();

            ItemNameLabel.Text = item.Item.Lang(p => p.ItemName);
            SuccessRateLabel.Text = "制作成功率".Lang() + ": " + item.SuccessRate.ToString() + "%";
            ItemNameLabel.Visible = true;
            SuccessRateLabel.Visible = true;

            if (item.Item1 != null)
            {
                Name1.Text = item.Item1.Lang(p => p.ItemName);
                Amount1.Text = GameScene.Game.CountItemInventory(item.Item1) + "/" + item.Amount1.ToString();
                Name1.Visible = true;
                Amount1.Visible = true;
            }

            if (item.Item2 != null)
            {
                Name2.Text = item.Item2.Lang(p => p.ItemName);
                Amount2.Text = GameScene.Game.CountItemInventory(item.Item2) + "/" + item.Amount2.ToString();
                Name2.Visible = true;
                Amount2.Visible = true;
            }

            if (item.Item3 != null)
            {
                Name3.Text = item.Item3.Lang(p => p.ItemName);
                Amount3.Text = GameScene.Game.CountItemInventory(item.Item3) + "/" + item.Amount3.ToString();
                Name3.Visible = true;
                Amount3.Visible = true;
            }

            if (item.Item4 != null)
            {
                Name4.Text = item.Item4.Lang(p => p.ItemName);
                Amount4.Text = GameScene.Game.CountItemInventory(item.Item4) + "/" + item.Amount4.ToString();
                Name4.Visible = true;
                Amount4.Visible = true;
            }

            if (item.Item5 != null)
            {
                Name5.Text = item.Item5.Lang(p => p.ItemName);
                Amount5.Text = GameScene.Game.CountItemInventory(item.Item5) + "/" + item.Amount5.ToString();
                Name5.Visible = true;
                Amount5.Visible = true;
            }
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void Draw(DXItemCell cell, int index, LibraryFile libfile, float blendrate)
        {
            MirLibrary library;
            if (!CEnvir.LibraryList.TryGetValue(libfile, out library)) return;

            Size s = library.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            library.Draw(index, x, y, Color.White, false, blendrate, ImageType.Image);
        }

        /// <summary>
        /// 装备内观
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Character_AfterDraw(object sender, EventArgs e)
        {
            MirLibrary library;

            int x = 90;
            int y = 208;

            #region 逻辑
            bool drawBody = true, drawArmour = false, drawFashion = false, drawWeapon = false, drawShield = false, drawHelmet = false, drawHair = false;
            DXItemCell armour = Grid[(int)EquipmentSlot.Armour],
                           fashion = Grid[(int)EquipmentSlot.Fashion],
                           weapon = Grid[(int)EquipmentSlot.Weapon],
                           shield = Grid[(int)EquipmentSlot.Shield],
                           helmet = Grid[(int)EquipmentSlot.Helmet];
            bool fishshape = armour.Item != null && armour.Item.Info.Shape == 16; //是不是钓鱼服
            //时装
            //如果 已装配时装 并且 显示时装 并且 不是钓鱼服 就画时装
            if (fashion.Item != null && ShowFashionBox.Checked && !fishshape)
                drawFashion = true;

            //衣服 
            //如果 已装配衣服 并且 (不显示时装 或者 时装为空 或者 是钓鱼服) 就画衣服
            //if (armour.Item != null && (!ShowFashionBox.Checked || fashion.Item == null || fishshape))
            if (armour.Item != null && !drawFashion)
                drawArmour = true;

            //裸体  
            //如果 已装配时装 并且 显示时装 并且 时装在不显示本体列表中) 就不显示裸体
            //if (fashion.Item != null && ShowFashionBox.Checked && Globals.ArmourWithBodyList.Contains(fashion.Item.Info.Image))
            if (drawFashion && Globals.ArmourWithBodyList.Contains(fashion.Item.Info.Image))
                drawBody = false;

            //武器
            //如果 已装配武器 并且 (不显示时装 或者 未装配时装 或者 时装不在自带武器衣服列表中或者 是钓鱼服） 就画武器
            if (weapon.Item != null && (!ShowFashionBox.Checked || fashion.Item == null || !Globals.ArmourWithWeaponList.Contains(fashion.Item.Info.Image) || fishshape))
                drawWeapon = true;

            //盾牌
            //如果 已装配盾牌 并且 显示盾牌 并且 盾牌shape小于1000 就画盾牌
            if (shield.Item != null && ShowShieldBox.Checked)
                drawShield = true;
            shield.Visible = drawShield;

            //头盔
            //如果 装配头盔 并且 显示头盔 并且 不是钓鱼服 并且 不显示时装 就画头盔
            if (helmet.Item != null && !fishshape && !drawFashion)  //&& ShowHelmetBox.Checked
                drawHelmet = true;

            //发型
            //如果 不画头盔 并且 不是钓鱼服 并且 (画时装且时装不在自带头盔衣服的列表中)
            if (!drawHelmet && !fishshape && !(drawFashion && Globals.ArmourWithHelmetList.Contains(fashion.Item.Info.Image)))
                drawHair = true;

            if (drawArmour && armour.Selected)
            {
                drawArmour = false;
                if (!drawHelmet) drawHair = true;
            }
            if (drawWeapon && weapon.Selected) drawWeapon = false;
            if (drawShield && shield.Selected) drawShield = false;
            if (drawHelmet && helmet.Selected)
            {
                drawHelmet = false;
                drawHair = true;
            }
            #endregion

            #region 底层衣服内观特效
            if (drawArmour)
            {
                //int index = armour.Item.Info.Image;
                int index = CEnvir.GetItemIllusionItemInfo(Grid[(int)EquipmentSlot.Armour].Item);

                #region 画衣服内观特效
                MirLibrary effectLibrary;
                if (CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out effectLibrary))
                {
                    MirImage image = null;

                    switch (index)
                    {
                        //翅膀
                        case 3320:
                        case 3330:
                            image = effectLibrary.CreateImage(2100, ImageType.Image);
                            break;
                        case 3340:
                        case 3350:
                            image = effectLibrary.CreateImage(2101, ImageType.Image);
                            break;
                        case 3360:
                        case 3370:
                            image = effectLibrary.CreateImage(2102, ImageType.Image);
                            break;
                        case 3380:
                        case 3390:
                            image = effectLibrary.CreateImage(2103, ImageType.Image);
                            break;
                        case 3321:
                        case 3331:
                        case 3341:
                        case 3351:
                        case 3361:
                        case 3371:
                        case 3381:
                        case 3391:
                            image = effectLibrary.CreateImage(2200 + (GameScene.Game.MapControl.Animation % 11), ImageType.Image);
                            break;
                        case 947:
                        case 957:
                            image = effectLibrary.CreateImage(2300 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 5345:
                            image = effectLibrary.CreateImage(3000, ImageType.Image);
                            break;
                        case 984:
                            image = effectLibrary.CreateImage(1100, ImageType.Image);
                            break;
                        case 994:
                            image = effectLibrary.CreateImage(1120, ImageType.Image);
                            break;
                        case 983:
                            image = effectLibrary.CreateImage(1200, ImageType.Image);
                            break;
                        case 993:
                            image = effectLibrary.CreateImage(1220, ImageType.Image);
                            break;
                        case 1023:
                            image = effectLibrary.CreateImage(1300, ImageType.Image);
                            break;
                        case 1033:
                            image = effectLibrary.CreateImage(1320, ImageType.Image);
                            break;
                        case 1003:
                            image = effectLibrary.CreateImage(1400, ImageType.Image);
                            break;
                        case 1013:
                            image = effectLibrary.CreateImage(1420, ImageType.Image);
                            break;
                        default:
                            //画自定义特效
                            ItemDisplayEffect innerDisplayEffect = Globals.ItemDisplayEffectList.Binding.FirstOrDefault(X => X.Info.Index == Grid[(int)EquipmentSlot.Armour].Item.Info.Index);
                            if (innerDisplayEffect != null && innerDisplayEffect.DrawInnerEffect && !innerDisplayEffect.EffectBehindImage)
                            {
                                MirLibrary customInnerEffectLib;
                                if (CEnvir.LibraryList.TryGetValue(innerDisplayEffect.InnerEffectLibrary, out customInnerEffectLib))
                                {
                                    image = customInnerEffectLib.CreateImage(innerDisplayEffect.InnerImageStartIndex + (GameScene.Game.MapControl.Animation % innerDisplayEffect.InnerImageCount), ImageType.Image);
                                    x = innerDisplayEffect.InnerX;
                                    y = innerDisplayEffect.InnerY;
                                }
                            }
                            break;
                    }
                    if (image != null)
                    {
                        PresentTexture(image.Image, CharacterTabBackGround, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f);
                    }
                }
                #endregion
            }
            #endregion

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out library))
            {
                //女刺客的长辫子 放到最底层
                if (drawHair)
                    if (MapObject.User.Class == MirClass.Assassin && MapObject.User.Gender == MirGender.Female && MapObject.User.HairType == 1)
                        library.Draw(1160, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, MapObject.User.HairColour, true, 1F, ImageType.Image);

                #region 裸体
                if (drawBody)
                {
                    switch (MapObject.User.Gender)
                    {
                        case MirGender.Male:  //男
                            library.Draw(0, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                            break;
                        case MirGender.Female:  //女
                            library.Draw(1, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                            break;
                    }
                }
                #endregion
            }

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Equip, out library))
            {
                #region 衣服内观
                if (drawArmour)
                {
                    //int index = armour.Item.Info.Image;
                    int index = CEnvir.GetItemIllusionItemInfo(Grid[(int)EquipmentSlot.Armour].Item);

                    #region 画衣服
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, armour.Item.Colour, true, 1F, ImageType.Overlay);
                    #endregion
                }
                #endregion

                #region 时装内观
                if (drawFashion)
                {
                    int index = fashion.Item.Info.Image;

                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, fashion.Item.Colour, true, 1F, ImageType.Overlay);
                }
                #endregion
            }

            #region 上层衣服内观特效
            if (drawArmour)
            {
                //int index = armour.Item.Info.Image;
                int index = CEnvir.GetItemIllusionItemInfo(Grid[(int)EquipmentSlot.Armour].Item);

                #region 画衣服内观特效
                MirLibrary effectLibrary;
                if (CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out effectLibrary))
                {
                    MirImage image = null;

                    switch (index)
                    {
                        case 966:
                            image = effectLibrary.CreateImage(100 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                            break;
                        case 976:
                            image = effectLibrary.CreateImage(120 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                            break;
                        case 965:
                            image = effectLibrary.CreateImage(200 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 975:
                            image = effectLibrary.CreateImage(220 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 964:
                            image = effectLibrary.CreateImage(300 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 974:
                            image = effectLibrary.CreateImage(320 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 963:
                            image = effectLibrary.CreateImage(400 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 973:
                            image = effectLibrary.CreateImage(420 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 2007:
                            image = effectLibrary.CreateImage(500 + (GameScene.Game.MapControl.Animation % 13), ImageType.Image);
                            break;
                        case 2017:
                            image = effectLibrary.CreateImage(520 + (GameScene.Game.MapControl.Animation % 13), ImageType.Image);
                            break;
                        case 986:
                            if (MapObject.User.Class == MirClass.Warrior)
                            {
                                image = effectLibrary.CreateImage(600, ImageType.Image);
                            }
                            if (MapObject.User.Class == MirClass.Wizard)
                            {
                                image = effectLibrary.CreateImage(601, ImageType.Image);
                            }
                            if (MapObject.User.Class == MirClass.Taoist)
                            {
                                image = effectLibrary.CreateImage(602, ImageType.Image);
                            }
                            break;
                        case 996:
                            if (MapObject.User.Class == MirClass.Warrior)
                            {
                                image = effectLibrary.CreateImage(620, ImageType.Image);
                            }
                            if (MapObject.User.Class == MirClass.Wizard)
                            {
                                image = effectLibrary.CreateImage(621, ImageType.Image);
                            }
                            if (MapObject.User.Class == MirClass.Taoist)
                            {
                                image = effectLibrary.CreateImage(622, ImageType.Image);
                            }
                            break;
                        case 942:
                            image = effectLibrary.CreateImage(700, ImageType.Image);
                            break;
                        case 952:
                            image = effectLibrary.CreateImage(720, ImageType.Image);
                            break;
                        case 982:
                            image = effectLibrary.CreateImage(800, ImageType.Image);
                            break;
                        case 992:
                            image = effectLibrary.CreateImage(820, ImageType.Image);
                            break;
                        case 1022:
                            image = effectLibrary.CreateImage(900, ImageType.Image);
                            break;
                        case 1032:
                            image = effectLibrary.CreateImage(920, ImageType.Image);
                            break;
                        case 1002:
                            image = effectLibrary.CreateImage(1000, ImageType.Image);
                            break;
                        case 1012:
                            image = effectLibrary.CreateImage(1020, ImageType.Image);
                            break;
                        case 985:
                            image = effectLibrary.CreateImage(1500, ImageType.Image);
                            break;
                        case 995:
                            image = effectLibrary.CreateImage(1520, ImageType.Image);
                            break;
                        case 961:
                            image = effectLibrary.CreateImage(1600, ImageType.Image);
                            break;
                        case 971:
                            image = effectLibrary.CreateImage(1620, ImageType.Image);
                            break;
                        case 962:
                            image = effectLibrary.CreateImage(1700 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                            break;
                        case 972:
                            image = effectLibrary.CreateImage(1720 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                            break;
                        case 944:
                            image = effectLibrary.CreateImage(1800, ImageType.Image);
                            break;
                        case 954:
                            image = effectLibrary.CreateImage(1820, ImageType.Image);
                            break;
                        case 3325:
                            image = effectLibrary.CreateImage(2400 + (GameScene.Game.MapControl.Animation % 14), ImageType.Image);
                            break;
                        case 3335:
                            image = effectLibrary.CreateImage(2500 + (GameScene.Game.MapControl.Animation % 14), ImageType.Image);
                            break;
                        case 3342:
                            image = effectLibrary.CreateImage(2600 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        case 3352:
                            image = effectLibrary.CreateImage(2700 + (GameScene.Game.MapControl.Animation % 15), ImageType.Image);
                            break;
                        default:
                            //画自定义特效
                            ItemDisplayEffect innerDisplayEffect = Globals.ItemDisplayEffectList.Binding.FirstOrDefault(X => X.Info.Index == Grid[(int)EquipmentSlot.Armour].Item.Info.Index);
                            if (innerDisplayEffect != null && innerDisplayEffect.DrawInnerEffect && innerDisplayEffect.EffectBehindImage)
                            {
                                MirLibrary customInnerEffectLib;
                                if (CEnvir.LibraryList.TryGetValue(innerDisplayEffect.InnerEffectLibrary, out customInnerEffectLib))
                                {
                                    image = customInnerEffectLib.CreateImage(innerDisplayEffect.InnerImageStartIndex + (GameScene.Game.MapControl.Animation % innerDisplayEffect.InnerImageCount), ImageType.Image);
                                    x = innerDisplayEffect.InnerX;
                                    y = innerDisplayEffect.InnerY;
                                }
                            }
                            break;
                    }
                    if (image != null)
                    {
                        PresentTexture(image.Image, CharacterTabBackGround, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f);
                    }
                }
                #endregion
            }
            #endregion

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out library))
            {
                #region 发型
                if (drawHair)
                {
                    if (MapObject.User.HairType > 0)    //发型 单独处理 不能用自定义特效功能
                    {
                        switch (MapObject.User.Class)
                        {
                            case MirClass.Warrior:
                            case MirClass.Wizard:
                            case MirClass.Taoist:
                                switch (MapObject.User.Gender)
                                {
                                    case MirGender.Male:
                                        library.Draw(60 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image);
                                        library.Draw(60 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, MapObject.User.HairColour, true, 1F, ImageType.Overlay);
                                        break;
                                    case MirGender.Female:
                                        library.Draw(80 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image);
                                        library.Draw(80 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, MapObject.User.HairColour, true, 1F, ImageType.Overlay);
                                        break;
                                }
                                break;
                            case MirClass.Assassin:
                                switch (MapObject.User.Gender)
                                {
                                    case MirGender.Male:
                                        library.Draw(1100 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image);
                                        library.Draw(1100 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, MapObject.User.HairColour, true, 1F, ImageType.Overlay);
                                        break;
                                    case MirGender.Female:
                                        library.Draw(1120 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image);
                                        library.Draw(1120 + MapObject.User.HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, MapObject.User.HairColour, true, 1F, ImageType.Overlay);
                                        break;
                                }
                                break;
                        }
                    }
                }
                #endregion
            }

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.Equip, out library))
            {
                #region 头盔内观
                if (drawHelmet)
                {
                    int index = helmet.Item.Info.Image;
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    //library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, helmet.Item.Colour, true, 1F, ImageType.Overlay);
                }
                #endregion

                #region 武器内观
                if (drawWeapon)
                {
                    //int imageIndex = weapon.Item.Info.Image;
                    int imageIndex = CEnvir.GetItemIllusionItemInfo(Grid[(int)EquipmentSlot.Weapon].Item);

                    #region 画武器
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, weapon.Item.Colour, true, 1F, ImageType.Overlay);
                    #endregion

                    #region 画武器特效
                    MirLibrary effectLibrary;
                    if (CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out effectLibrary))
                    {
                        MirImage image = null;
                        switch (imageIndex)
                        {
                            //战/法/道
                            case 1076:  //影魅之刃
                                image = effectLibrary.CreateImage(2000 + (GameScene.Game.MapControl.Animation % 10), ImageType.Image);
                                break;
                            //刺客
                            case 2550:
                                image = effectLibrary.CreateImage(1920 + (GameScene.Game.MapControl.Animation % 12), ImageType.Image);
                                break;
                            //全职业	
                            case 2530:
                                image = effectLibrary.CreateImage(1900 + (GameScene.Game.MapControl.Animation % 12), ImageType.Image);
                                break;
                            default:
                                //画自定义特效
                                ItemDisplayEffect innerDisplayEffect = Globals.ItemDisplayEffectList.Binding.FirstOrDefault(X => X.Info.Index == Grid[(int)EquipmentSlot.Weapon].Item.Info.Index);
                                if (innerDisplayEffect != null && innerDisplayEffect.DrawInnerEffect && !innerDisplayEffect.EffectBehindImage)
                                {
                                    MirLibrary customInnerEffectLib;
                                    if (CEnvir.LibraryList.TryGetValue(innerDisplayEffect.InnerEffectLibrary, out customInnerEffectLib))
                                    {
                                        image = customInnerEffectLib.CreateImage(innerDisplayEffect.InnerImageStartIndex + (GameScene.Game.MapControl.Animation % innerDisplayEffect.InnerImageCount), ImageType.Image);
                                        x = innerDisplayEffect.InnerX;
                                        y = innerDisplayEffect.InnerY;
                                    }
                                }
                                break;
                        }
                        if (image != null)
                        {
                            //bool oldBlend = DXManager.Blending;
                            //float oldRate = DXManager.BlendRate;

                            //DXManager.SetBlend(true, 0.8F);

                            PresentTexture(image.Image, CharacterTabBackGround, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f);

                            //DXManager.SetBlend(oldBlend, oldRate);
                        }
                    }
                    #endregion
                }
                #endregion

                #region 盾牌内观
                if (drawShield)
                {
                    int imageIndex = shield.Item.Info.Image;

                    #region 画盾牌
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image);
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, shield.Item.Colour, true, 1F, ImageType.Overlay);
                    #endregion

                    #region 画盾牌特效
                    MirLibrary effectLibrary;
                    if (CEnvir.LibraryList.TryGetValue(LibraryFile.EquipEffect_UI, out effectLibrary))
                    {
                        MirImage image = null;
                        switch (imageIndex)
                        {
                            case 2886:
                                image = effectLibrary.CreateImage(2800 + (GameScene.Game.MapControl.Animation) % 10, ImageType.Image);
                                break;
                            case 2887:
                                image = effectLibrary.CreateImage(2810 + (GameScene.Game.MapControl.Animation) % 10, ImageType.Image);
                                break;
                            case 2888:
                                image = effectLibrary.CreateImage(2820 + (GameScene.Game.MapControl.Animation) % 10, ImageType.Image);
                                break;
                            case 2889:
                                image = effectLibrary.CreateImage(2830 + (GameScene.Game.MapControl.Animation) % 10, ImageType.Image);
                                break;
                            default:
                                //画自定义特效
                                ItemDisplayEffect innerDisplayEffect = Globals.ItemDisplayEffectList.Binding.FirstOrDefault(X => X.Info.Index == Grid[(int)EquipmentSlot.Shield].Item.Info.Index);
                                if (innerDisplayEffect != null && innerDisplayEffect.DrawInnerEffect && !innerDisplayEffect.EffectBehindImage)
                                {
                                    MirLibrary customInnerEffectLib;
                                    if (CEnvir.LibraryList.TryGetValue(innerDisplayEffect.InnerEffectLibrary, out customInnerEffectLib))
                                    {
                                        image = customInnerEffectLib.CreateImage(innerDisplayEffect.InnerImageStartIndex + (GameScene.Game.MapControl.Animation % innerDisplayEffect.InnerImageCount), ImageType.Image);
                                        x = innerDisplayEffect.InnerX;
                                        y = innerDisplayEffect.InnerY;
                                    }
                                }
                                break;
                        }
                        if (image != null)
                        {
                            //bool oldBlend = DXManager.Blending;
                            //float oldRate = DXManager.BlendRate;

                            //DXManager.SetBlend(true, 0.8F);

                            PresentTexture(image.Image, CharacterTabBackGround, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f);

                            //DXManager.SetBlend(oldBlend, oldRate);
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }
        /// <summary>
        /// 更新属性状态
        /// </summary>
        public void UpdateStats()
        {
            foreach (KeyValuePair<Stat, DXLabel> pair in DisplayStats)
                pair.Value.Text = MapObject.User.Stats.GetFormat(pair.Key);

            foreach (KeyValuePair<Stat, DXLabel> pair in AttackStats)
            {
                if (MapObject.User.Stats[pair.Key] > 0)
                {
                    pair.Value.Text = $"+{MapObject.User.Stats[pair.Key]}";
                    pair.Value.ForeColour = Color.White;
                    ((DXImageControl)pair.Value.Tag).Visible = true;
                }
                else
                {
                    pair.Value.Text = "";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).Visible = false;
                }
            }

            foreach (KeyValuePair<Stat, DXLabel> pair in AdvantageStats)
            {
                if (MapObject.User.Stats[pair.Key] > 0)
                {
                    pair.Value.Text = $"x{MapObject.User.Stats[pair.Key]}";
                    pair.Value.ForeColour = Color.White;
                    ((DXImageControl)pair.Value.Tag).Visible = true;
                }
                else
                {
                    pair.Value.Text = "";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).Visible = false;
                }
            }

            foreach (KeyValuePair<Stat, DXLabel> pair in DisadvantageStats)
            {
                pair.Value.Text = MapObject.User.Stats.GetFormat(pair.Key);

                if (MapObject.User.Stats[pair.Key] < 0)
                {
                    pair.Value.Text = $"x{Math.Abs(MapObject.User.Stats[pair.Key])}";
                    pair.Value.ForeColour = Color.White;
                    ((DXImageControl)pair.Value.Tag).Visible = true;
                }
                else
                {
                    pair.Value.Text = "";
                    pair.Value.ForeColour = Color.FromArgb(60, 60, 60);
                    ((DXImageControl)pair.Value.Tag).Visible = false;
                }
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (CharacterTabBackGround != null)
                {
                    if (!CharacterTabBackGround.IsDisposed)
                        CharacterTabBackGround.Dispose();

                    CharacterTabBackGround = null;
                }

                if (MakeTabBackGround != null)
                {
                    if (!MakeTabBackGround.IsDisposed)
                        MakeTabBackGround.Dispose();

                    MakeTabBackGround = null;
                }

                if (CharacterNameLabel != null)
                {
                    if (!CharacterNameLabel.IsDisposed)
                        CharacterNameLabel.Dispose();

                    CharacterNameLabel = null;
                }

                if (GuildNameLabel != null)
                {
                    if (!GuildNameLabel.IsDisposed)
                        GuildNameLabel.Dispose();

                    GuildNameLabel = null;
                }

                if (GuildRankLabel != null)
                {
                    if (!GuildRankLabel.IsDisposed)
                        GuildRankLabel.Dispose();

                    GuildRankLabel = null;
                }

                if (MarriageIcon != null)
                {
                    if (!MarriageIcon.IsDisposed)
                        MarriageIcon.Dispose();

                    MarriageIcon = null;
                }

                if (CharacterPage != null)
                {
                    if (!CharacterPage.IsDisposed)
                        CharacterPage.Dispose();

                    CharacterPage = null;
                }

                if (MakePage != null)
                {
                    if (!MakePage.IsDisposed)
                        MakePage.Dispose();

                    MakePage = null;
                }

                if (MakeAButton != null)
                {
                    if (!MakeAButton.IsDisposed)
                        MakeAButton.Dispose();

                    MakeAButton = null;
                }

                if (OPPMekDialogButton != null)
                {
                    if (!OPPMekDialogButton.IsDisposed)
                        OPPMekDialogButton.Dispose();

                    OPPMekDialogButton = null;
                }

                if (MakeAButton != null)
                {
                    if (!MakeAButton.IsDisposed)
                        MakeAButton.Dispose();

                    MakeAButton = null;
                }

                if (CraftLevelLabel != null)
                {
                    if (!CraftLevelLabel.IsDisposed)
                        CraftLevelLabel.Dispose();

                    CraftLevelLabel = null;
                }

                if (CraftExpLabel != null)
                {
                    if (!CraftExpLabel.IsDisposed)
                        CraftExpLabel.Dispose();

                    CraftExpLabel = null;
                }

                if (ItemNameLabel != null)
                {
                    if (!ItemNameLabel.IsDisposed)
                        ItemNameLabel.Dispose();

                    ItemNameLabel = null;
                }

                if (SuccessRateLabel != null)
                {
                    if (!SuccessRateLabel.IsDisposed)
                        SuccessRateLabel.Dispose();

                    SuccessRateLabel = null;
                }

                if (TargetItemCell != null)
                {
                    if (!TargetItemCell.IsDisposed)
                        TargetItemCell.Dispose();

                    TargetItemCell = null;
                }
                if (Name1 != null)
                {
                    if (!Name1.IsDisposed)
                        Name1.Dispose();

                    Name1 = null;
                }

                if (Name2 != null)
                {
                    if (!Name2.IsDisposed)
                        Name2.Dispose();

                    Name2 = null;
                }

                if (Name3 != null)
                {
                    if (!Name3.IsDisposed)
                        Name3.Dispose();

                    Name3 = null;
                }

                if (Name4 != null)
                {
                    if (!Name4.IsDisposed)
                        Name4.Dispose();

                    Name4 = null;
                }

                if (Name5 != null)
                {
                    if (!Name5.IsDisposed)
                        Name5.Dispose();

                    Name5 = null;
                }

                if (Amount1 != null)
                {
                    if (!Amount1.IsDisposed)
                        Amount1.Dispose();

                    Amount1 = null;
                }

                if (Amount2 != null)
                {
                    if (!Amount2.IsDisposed)
                        Amount2.Dispose();

                    Amount2 = null;
                }

                if (Amount3 != null)
                {
                    if (!Amount3.IsDisposed)
                        Amount3.Dispose();

                    Amount3 = null;
                }

                if (Amount4 != null)
                {
                    if (!Amount4.IsDisposed)
                        Amount4.Dispose();

                    Amount4 = null;
                }

                if (Amount5 != null)
                {
                    if (!Amount5.IsDisposed)
                        Amount5.Dispose();

                    Amount5 = null;
                }

                if (Grid != null)
                {
                    for (int i = 0; i < Grid.Length; i++)
                    {
                        if (Grid[i] != null)
                        {
                            if (!Grid[i].IsDisposed)
                                Grid[i].Dispose();

                            Grid[i] = null;
                        }
                    }
                    Grid = null;
                }

                if (WearWeightLabel != null)
                {
                    if (!WearWeightLabel.IsDisposed)
                        WearWeightLabel.Dispose();

                    WearWeightLabel = null;
                }

                if (HandWeightLabel != null)
                {
                    if (!HandWeightLabel.IsDisposed)
                        HandWeightLabel.Dispose();

                    HandWeightLabel = null;
                }

                foreach (KeyValuePair<Stat, DXLabel> pair in DisplayStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                DisplayStats.Clear();
                DisplayStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in AttackStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                AttackStats.Clear();
                AttackStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in AdvantageStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                AdvantageStats.Clear();
                AdvantageStats = null;

                foreach (KeyValuePair<Stat, DXLabel> pair in DisadvantageStats)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }
                DisadvantageStats.Clear();
                DisadvantageStats = null;
            }
        }
        #endregion
    }
}
