using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 宠物功能
    /// </summary>
    public sealed class CompanionDialog : DXWindow
    {
        #region Properties
        public DXImageControl CompanionBackGround, WarPetBackGround, GridBackGround,  //宠物主框  战宠主框  宠物包裹容器
               WeightBar, HungerBar, ExpBar;             //负重条 饥饿条 经验条 

        public DXItemCell[] EquipmentGrid;     //宠物道具格子
        public DXItemGrid InventoryGrid;    //宠物背包
        public DXVScrollBar InventoryGridScrollBar;  //宠物背包滚动条

        public MonsterObject CompanionDisplay;  //显示宠物图片
        public Point CompanionDisplayPoint;  //显示宠物图片的坐标

        public DXLabel WeightLabel, HungerLabel, NameLabel, LevelLabel, ExperienceLabel,                            //负重 饥饿度  名字   宠物等级  宠物经验
                       Level1Label, Level3Label, Level5Label, Level7Label, Level10Label, Level11Label, Level13Label, Level15Label;  //宠物等级属性
        public DXComboBox ModeComboBox;   //宠物模式
        public DXButton ConfigButton, UnlockButton, ViewSkillsButton, ViewGridButton, NextButton, PreviousButton, RefreshGrid; //设置 解锁 查看技能 查看背包 查看技能下一页 查看技能上一页 刷新包裹
        public int BagWeight, MaxBagWeight, InventorySize;  //背包负重  最大背包负重  背包大小

        public override WindowType Type => WindowType.CompanionBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 宠物面板
        /// </summary>
        public CompanionDialog()
        {
            HasTitle = false;              //不显示标题
            HasFooter = false;             //不显示页脚
            HasTopBorder = false;          //不显示上边框
            TitleLabel.Visible = false;    //标题标签不显示
            CloseButton.Visible = false;   //关闭按钮不显示
            Movable = true;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size s;
            s = GameInterLibrary.GetSize(4300);  //主界面的大小按图片的大小设定
            Size = s;

            CompanionDisplayPoint = new Point(ClientArea.X + 80, ClientArea.Y + 120);  //宠物图片在框架里显示的坐标

            CompanionBackGround = new DXImageControl   //捡取宠界面
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4300,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,        //是否控制
                PassThrough = true,     //是否能穿透
                Visible = true,
            };

            WarPetBackGround = new DXImageControl     //战宠界面
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 4301,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = false,
            };

            CloseButton = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 113,
                LibraryFile = LibraryFile.Interface,
                Hint = "关闭".Lang(),
            };
            CloseButton.MouseEnter += (o, e) => CloseButton.Index = 112;
            CloseButton.MouseLeave += (o, e) => CloseButton.Index = 113;
            CloseButton.MouseClick += (o, e) => Visible = false;

            DXButton CompanionButton = new DXButton    //宠物按钮
            {
                Index = 4316,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(10, 38),
                Parent = this,
            };
            CompanionButton.MouseClick += (o, e) =>
            {
                CompanionBackGround.Visible = true;
                WarPetBackGround.Visible = false;
            };

            DXButton WarPetButton = new DXButton   //战宠按钮
            {
                Index = 4317,
                LibraryFile = LibraryFile.GameInter,
                Location = new Point(79, 38),
                Parent = this,
            };
            WarPetButton.MouseClick += (o, e) =>
            {
                CompanionBackGround.Visible = true;
                WarPetBackGround.Visible = false;
            };

            GridBackGround = new DXImageControl  //宠物包裹容器
            {
                Index = 4327,
                LibraryFile = LibraryFile.GameInter,
                Parent = CompanionBackGround,
                Size = new Size(210, 300),
                Location = new Point(252, 62),
                Visible = true,
            };

            InventoryGrid = new DXItemGrid              //宠物包裹
            {
                GridSize = new Size(5, 6),
                Parent = GridBackGround,
                GridType = GridType.CompanionInventory,
                ItemGrid = CEnvir.CompanionGrid,    //道具类型
                Location = new Point(9, 14),
                VisibleHeight = 6,                  //格子高度6个格子位
                Border = false,
            };
            InventoryGrid.GridSizeChanged += InventoryGrid_GridSizeChanged;    //格子改变

            InventoryGridScrollBar = new DXVScrollBar
            {
                Parent = GridBackGround,
                Size = new Size(15, 248),
                Location = new Point(194, 3),
                Border = false,
                VisibleSize = 6,                     //可见尺寸为6格
                Change = 1,                          //改变为1格
            };
            InventoryGridScrollBar.ValueChanged += InventoryGridScrollBar_ValueChanged;  //滚动值变化
            //为滚动条自定义皮肤 -1为不设置
            InventoryGridScrollBar.SetSkin(LibraryFile.GameInter, -1, -1, -1, 1225);

            EquipmentGrid = new DXItemCell[Globals.CompanionEquipmentSize];   //宠物装备格子设定
            DXItemCell cell;
            EquipmentGrid[(int)CompanionSlot.Bag] = cell = new DXItemCell         //宠物格子扩展
            {
                Location = new Point(ClientArea.X + 188, ClientArea.Y + 70),
                Parent = CompanionBackGround,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Bag,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 99);

            EquipmentGrid[(int)CompanionSlot.Head] = cell = new DXItemCell          //宠物头饰
            {
                Location = new Point(ClientArea.X + 188, ClientArea.Y + 112),
                Parent = CompanionBackGround,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Head,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 100);

            EquipmentGrid[(int)CompanionSlot.Back] = cell = new DXItemCell     //宠物背饰
            {
                Location = new Point(ClientArea.X + 188, ClientArea.Y + 155),
                Parent = CompanionBackGround,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Back,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 101);

            EquipmentGrid[(int)CompanionSlot.Food] = cell = new DXItemCell   //宠物口粮
            {
                Location = new Point(ClientArea.X + 144, ClientArea.Y + 153),
                Parent = CompanionBackGround,
                FixedBorder = true,
                Border = true,
                Slot = (int)CompanionSlot.Food,
                GridType = GridType.CompanionEquipment,
            };
            cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 102);

            DXCheckBox PickUpCheckBox = new DXCheckBox
            {
                Parent = this,
                Label = { Text = "拾取物品".Lang() },
                Visible = false
            };
            PickUpCheckBox.Location = new Point(ClientArea.Right - PickUpCheckBox.Size.Width + 3, ClientArea.Y + 45);

            ConfigButton = new DXButton            //宠物捡取过滤
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 7917,
                Parent = GridBackGround,
                Location = new Point(90, 258),
            };
            ConfigButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.BigPatchBox.Visible == false)
                {
                    GameScene.Game.BigPatchBox.Visible = true;
                    GameScene.Game.BigPatchBox.AutoPick.TabButton.InvokeMouseClick();
                }
                else
                {
                    GameScene.Game.BigPatchBox.Visible = false;
                }
            };

            DXControl AttPanel = new DXControl   //属性面板  显示技能1-10级
            {
                Parent = CompanionBackGround,
                Size = new Size(200, 290),
                Location = new Point(255, 65),
                Visible = false,
            };

            DXControl AttPanel1 = new DXControl  //属性面板1  显示技能11-15级
            {
                Parent = CompanionBackGround,
                Size = new Size(200, 290),
                Location = new Point(255, 65),
                Visible = false,
            };

            DXLabel label = new DXLabel
            {
                Parent = AttPanel,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级1".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 13);

            Level1Label = new DXLabel
            {
                Parent = AttPanel,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 35),
                Text = "自动捡取物品".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级3".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 70);

            Level3Label = new DXLabel
            {
                Parent = AttPanel,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 93),
                Text = "无法使用".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级5".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 127);

            Level5Label = new DXLabel
            {
                Parent = AttPanel,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 149),
                Text = "无法使用".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级7".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 184);

            Level7Label = new DXLabel
            {
                Parent = AttPanel,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 207),
                Text = "无法使用".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级10".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 242);

            Level10Label = new DXLabel
            {
                Parent = AttPanel,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 264),
                Text = "无法使用".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel1,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级11".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 13);

            Level11Label = new DXLabel
            {
                Parent = AttPanel1,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 35),
                Text = "无法使用".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel1,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级13".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 70);

            Level13Label = new DXLabel
            {
                Parent = AttPanel1,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 93),
                Text = "无法使用".Lang(),
            };

            label = new DXLabel
            {
                Parent = AttPanel1,
                Outline = true,
                ForeColour = Color.White,
                OutlineColour = Color.Black,
                IsControl = false,
                Text = "宠物等级15".Lang() + "  " + "属性".Lang(),
            };
            label.Location = new Point(15, 127);

            Level15Label = new DXLabel
            {
                Parent = AttPanel1,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(15, 149),
                Text = "无法使用".Lang(),
            };

            NameLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CompanionBackGround,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(100, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 5, CompanionDisplayPoint.Y + 87)
            };

            LevelLabel = new DXLabel          //等级
            {
                AutoSize = false,
                Parent = CompanionBackGround,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(100, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 5, CompanionDisplayPoint.Y + 109)
            };

            ExpBar = new DXImageControl  //经验进度条
            {
                Parent = CompanionBackGround,
                LibraryFile = LibraryFile.GameInter,
                Index = 3,
                Location = new Point(CompanionDisplayPoint.X - 13, CompanionDisplayPoint.Y + 134),
            };
            ExpBar.AfterDraw += (o, e) =>
            {
                if (GameScene.Game.Companion == null) return;   //如果没有宠物，那么跳过

                if (ExpBar.Library == null) return;

                if (GameScene.Game.Companion.Experience == 0) return;    //如果经验等0，那么跳过

                CompanionLevelInfo info = Globals.CompanionLevelInfoList.Binding.First(x => x.Level == GameScene.Game.Companion.Level);

                float percent = Math.Min(1, Math.Max(0, GameScene.Game.Companion.Experience / (float)info.MaxExperience));  //如果 百分比 = 数值最小值（1，数值最大值（负重/最大负重））

                if (percent == 0) return;  //如果 百分比等0，那么跳过

                MirImage image = ExpBar.Library.CreateImage(4310, ImageType.Image);  //图像= 背包负重指定库 创建图像（图片序号，图片类型）

                if (image == null) return;  //如果图片等空，那么跳过

                PresentMirImage(image.Image, CompanionBackGround, new Rectangle(ExpBar.DisplayArea.X - 2, ExpBar.DisplayArea.Y + 1, (int)(image.Width * percent), image.Height), Color.White, ExpBar);
            };

            ExperienceLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CompanionBackGround,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(100, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 5, CompanionDisplayPoint.Y + 131)
            };

            HungerBar = new DXImageControl  //饥饿度进度条
            {
                Parent = CompanionBackGround,
                LibraryFile = LibraryFile.GameInter,
                Index = 3,
                Location = new Point(CompanionDisplayPoint.X - 13, CompanionDisplayPoint.Y + 156),
            };
            HungerBar.AfterDraw += (o, e) =>
            {
                if (GameScene.Game.Companion == null) return;    //如果没有宠物，那么跳过

                if (HungerBar.Library == null) return;

                if (GameScene.Game.Companion.Hunger == 0) return;    //如果饥饿度等0，那么跳过

                CompanionLevelInfo info = Globals.CompanionLevelInfoList.Binding.First(x => x.Level == GameScene.Game.Companion.Level);

                float percent = Math.Min(1, Math.Max(0, GameScene.Game.Companion.Hunger / (float)info.MaxHunger));  //如果 百分比 = 数值最小值（1，数值最大值（负重/最大负重））

                if (percent == 0) return;  //如果 百分比等0，那么跳过

                MirImage image = HungerBar.Library.CreateImage(4311, ImageType.Image);  //图像= 背包负重指定库 创建图像（图片序号，图片类型）

                if (image == null) return;  //如果图片等空，那么跳过

                PresentMirImage(image.Image, CompanionBackGround, new Rectangle(HungerBar.DisplayArea.X - 2, HungerBar.DisplayArea.Y + 1, (int)(image.Width * percent), image.Height), Color.White, HungerBar);
            };

            HungerLabel = new DXLabel
            {
                AutoSize = false,
                Parent = CompanionBackGround,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(100, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                IsControl = false,
                Location = new Point(CompanionDisplayPoint.X + 5, CompanionDisplayPoint.Y + 153)
            };

            WeightBar = new DXImageControl       //包裹负重进度条
            {
                Parent = GridBackGround,
                LibraryFile = LibraryFile.GameInter,
                Index = 3,
                Location = new Point(10, 265),
            };
            WeightBar.AfterDraw += (o, e) =>
            {
                if (GameScene.Game.Companion == null) return;   //如果没有宠物，那么跳过

                if (WeightBar.Library == null) return;
                if (BagWeight == 0) return;    //如果负重等0，那么跳过

                float percent = Math.Min(1, Math.Max(0, BagWeight / (float)MaxBagWeight));  //如果 百分比 = 数值最小值（1，数值最大值（负重/最大负重））

                if (percent == 0) return;  //如果 百分比等0，那么跳过

                MirImage image = WeightBar.Library.CreateImage(4312, ImageType.Image);  //图像= 背包负重指定库 创建图像（图片序号，图片类型）

                if (image == null) return;  //如果图片等空，那么跳过

                PresentMirImage(image.Image, GridBackGround, new Rectangle(WeightBar.DisplayArea.X - 2, WeightBar.DisplayArea.Y + 1, (int)(image.Width * percent), image.Height), Color.White, WeightBar, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            };

            WeightLabel = new DXLabel
            {
                Parent = GridBackGround,
                ForeColour = Color.White,
                Size = new Size(85, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Location = new Point(18, 264)
            };

            UnlockButton = new DXButton            //宠物解锁自动喂养
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4373,
                Parent = CompanionBackGround,
                Location = new Point(151, 160),
                //Visible = true
            };
            /*UnlockButton.AfterDraw += (o, e) =>
            {
                UnlockButton.Visible = !GameScene.Game.Companion.AutoFeed;
            };*/

            UnlockButton.MouseClick += UnlockButton_MouseClick;

            ViewSkillsButton = new DXButton      //查看技能按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4337,
                Parent = CompanionBackGround,
                Location = new Point(35, 325),
                Visible = true
            };
            ViewSkillsButton.MouseClick += (o, e) =>
            {
                ViewSkillsButton.Visible = false;
                ViewGridButton.Visible = true;
                GridBackGround.Visible = false;
                AttPanel.Visible = true;
                if (GameScene.Game.Companion != null && GameScene.Game.Companion.Level > 10)   //如果宠物的等级设置大于10
                {
                    NextButton.Visible = true;    //显示属性下一页
                }
            };

            ViewGridButton = new DXButton        //查看背包按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 4332,
                Parent = CompanionBackGround,
                Location = new Point(35, 325),
                Visible = false
            };
            ViewGridButton.MouseClick += (o, e) =>
            {
                ViewSkillsButton.Visible = true;
                ViewGridButton.Visible = false;
                GridBackGround.Visible = true;
                AttPanel.Visible = false;
                AttPanel1.Visible = false;
                NextButton.Visible = false;
            };

            NextButton = new DXButton
            {
                Parent = CompanionBackGround,
                Location = new Point(135, 325),
                Size = new Size(100, DefaultHeight),
                Label = { Text = "属性下一页".Lang() },
                Visible = false
            };
            NextButton.MouseClick += (o, e) =>
            {
                AttPanel.Visible = false;
                NextButton.Visible = false;
                AttPanel1.Visible = true;
                PreviousButton.Visible = true;
            };

            PreviousButton = new DXButton
            {
                Parent = CompanionBackGround,
                Location = new Point(135, 325),
                Size = new Size(100, DefaultHeight),
                Label = { Text = "属性上一页".Lang() },
                Visible = false
            };
            PreviousButton.MouseClick += (o, e) =>
            {
                AttPanel.Visible = true;
                NextButton.Visible = true;
                AttPanel1.Visible = false;
                PreviousButton.Visible = false;
            };

            RefreshGrid = new DXButton
            {
                Parent = CompanionBackGround,
                Location = new Point(135, 325),
                Size = new Size(100, DefaultHeight),
                Label = { Text = "刷新包裹".Lang() },
            };
            RefreshGrid.MouseClick += (o, e) =>
            {
                CEnvir.Enqueue(new C.CompanionGridRefresh { });
            };
        }
        /// <summary>
        /// 鼠标点击解锁按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UnlockButton_MouseClick(object sender, MouseEventArgs e)   //购买自动喂食功能
        {
            if (GameScene.Game.Companion == null) return;  //如果没有宠物，直接跳过

            if (GameScene.Game.Inventory.All(x => x == null || x.Info.Effect != ItemEffect.CompanionAutoBarn))   //如果 玩家背包库存为空 或者 道具不是宠物自动粮仓劵
            {
                GameScene.Game.ReceiveChat("你需要在商城购买一张宠物粮仓解锁券来解锁这只宠物的自动粮仓".Lang(), MessageType.System);   //提示并跳过
                return;
            }

            DXMessageBox box = new DXMessageBox($"你确定想要使用一张宠物自动粮仓解锁券".Lang() + "？\n\n" + $"" + $"这将解锁宠物的自动粮仓功能".Lang(), "解锁宠物自动粮仓".Lang(), DXMessageBoxButtons.YesNo);

            box.YesButton.MouseClick += (o1, e1) =>   //点YES购买
            {
                CEnvir.Enqueue(new C.CompanionAutoFeedUnlock { Index = GameScene.Game.Companion.Index });
                UnlockButton.Visible = false;
            };
        }

        #region Methods
        /// <summary>
        /// 宠物改变
        /// </summary>
        public void CompanionChanged()
        {
            if (GameScene.Game.Companion == null)
            {
                Visible = false;
                return;
            }
            ConfigButton.Visible = GameScene.Game.Companion.CompanionInfo.Sorting;
            InventoryGrid.ItemGrid = GameScene.Game.Companion.InventoryArray;

            foreach (DXItemCell cell in EquipmentGrid)
                cell.ItemGrid = GameScene.Game.Companion.EquipmentArray;

            CompanionDisplay = new MonsterObject(GameScene.Game.Companion.CompanionInfo);
            NameLabel.Text = GameScene.Game.Companion.Name;

            //刷新解锁按钮显示状态
            UnlockButton.Visible = !GameScene.Game.Companion.AutoFeed;

            Refresh();
        }
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            if (cell.Item != null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.2F, ImageType.Image);
        }
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            CompanionDisplay?.Process();
        }
        /// <summary>
        /// 在图片后绘制
        /// </summary>
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            if (CompanionDisplay == null) return;

            int x = DisplayArea.X + CompanionDisplayPoint.X;
            int y = DisplayArea.Y + CompanionDisplayPoint.Y;

            if (CompanionDisplay.Image == MonsterImage.Companion_Donkey)
            {
                x += 10;
            }

            if (CompanionDisplay.Image == MonsterImage.Companion_BanyoLordGuzak)   //如果是霸主，坐标修正
            {
                y += 10;
            }

            CompanionDisplay.DrawShadow(x, y);
            CompanionDisplay.DrawBody(x, y);
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            LevelLabel.Text = GameScene.Game.Companion.Level.ToString();

            CompanionLevelInfo info = Globals.CompanionLevelInfoList.Binding.First(x => x.Level == GameScene.Game.Companion.Level);

            ExperienceLabel.Text = info.MaxExperience > 0 ? $"{GameScene.Game.Companion.Experience / (decimal)info.MaxExperience:p2}" : "100%";

            HungerLabel.Text = $"{GameScene.Game.Companion.Hunger} / {info.MaxHunger}";

            WeightLabel.Text = $"{BagWeight} / {MaxBagWeight}";

            WeightLabel.ForeColour = BagWeight >= MaxBagWeight ? Color.Red : Color.White;

            Level3Label.Text = GameScene.Game.Companion.Level3 == null ? "暂无属性".Lang() : GameScene.Game.Companion.Level3.GetDisplay(GameScene.Game.Companion.Level3.Values.Keys.First());

            Level5Label.Text = GameScene.Game.Companion.Level5 == null ? "暂无属性".Lang() : GameScene.Game.Companion.Level5.GetDisplay(GameScene.Game.Companion.Level5.Values.Keys.First());

            Level7Label.Text = GameScene.Game.Companion.Level7 == null ? "暂无属性".Lang() : GameScene.Game.Companion.Level7.GetDisplay(GameScene.Game.Companion.Level7.Values.Keys.First());

            Level10Label.Text = GameScene.Game.Companion.Level10 == null ? "暂无属性".Lang() : GameScene.Game.Companion.Level10.GetDisplay(GameScene.Game.Companion.Level10.Values.Keys.First());

            Level11Label.Text = GameScene.Game.Companion.Level11 == null || GameScene.Game.Companion.Level11.Values.Count == 0 ? "暂无属性".Lang() : GameScene.Game.Companion.Level11.GetDisplay(GameScene.Game.Companion.Level11.Values.Keys.First());

            Level13Label.Text = GameScene.Game.Companion.Level13 == null || GameScene.Game.Companion.Level13.Values.Count == 0 ? "暂无属性".Lang() : GameScene.Game.Companion.Level13.GetDisplay(GameScene.Game.Companion.Level13.Values.Keys.First());

            Level15Label.Text = GameScene.Game.Companion.Level15 == null || GameScene.Game.Companion.Level15.Values.Count == 0 ? "暂无属性".Lang() : GameScene.Game.Companion.Level15.GetDisplay(GameScene.Game.Companion.Level15.Values.Keys.First());

            for (int i = 0; i < InventoryGrid.Grid.Length; i++)
                InventoryGrid.Grid[i].Enabled = i < InventorySize;

            RefreshCompanionGrid();
        }
        /// <summary>
        /// 刷新宠物包裹
        /// </summary>
        private void RefreshCompanionGrid()
        {
            InventoryGrid.GridSize = new Size(5, Math.Max(6, (int)Math.Ceiling(InventorySize / (float)5)));

            InventoryGridScrollBar.MaxValue = InventoryGrid.GridSize.Height;
            ApplyInventoryGridFilter();
        }
        /// <summary>
        /// 包裹格子过滤
        /// </summary>
        public void ApplyInventoryGridFilter()
        {
            foreach (DXItemCell cell in InventoryGrid.Grid)
                FilterCell(cell);
        }
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="cell"></param>
        public void FilterCell(DXItemCell cell)
        {
            if (cell.Slot >= InventorySize)
            {
                cell.Enabled = false;
                return;
            }

            cell.Enabled = true;
        }
        /// <summary>
        /// 宠物包裹容量大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InventoryGrid_GridSizeChanged(object sender, EventArgs e)
        {
            foreach (DXItemCell cell in InventoryGrid.Grid)
                cell.ItemChanged += (o, e1) => FilterCell(cell);

            foreach (DXItemCell cell in InventoryGrid.Grid)
                cell.MouseWheel += InventoryGridScrollBar.DoMouseWheel;
        }
        /// <summary>
        /// 包裹滚动条更改值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InventoryGridScrollBar_ValueChanged(object sender, EventArgs e)
        {
            InventoryGrid.ScrollValue = InventoryGridScrollBar.Value;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                CompanionDisplay = null;
                CompanionDisplayPoint = Point.Empty;

                if (EquipmentGrid != null)
                {
                    for (int i = 0; i < EquipmentGrid.Length; i++)
                    {
                        if (EquipmentGrid[i] != null)
                        {
                            if (!EquipmentGrid[i].IsDisposed)
                                EquipmentGrid[i].Dispose();

                            EquipmentGrid[i] = null;
                        }
                    }

                    EquipmentGrid = null;
                }

                if (CompanionBackGround != null)
                {
                    if (!CompanionBackGround.IsDisposed)
                        CompanionBackGround.Dispose();

                    CompanionBackGround = null;
                }

                if (WarPetBackGround != null)
                {
                    if (!WarPetBackGround.IsDisposed)
                        WarPetBackGround.Dispose();

                    WarPetBackGround = null;
                }

                if (GridBackGround != null)
                {
                    if (!GridBackGround.IsDisposed)
                        GridBackGround.Dispose();

                    GridBackGround = null;
                }

                if (InventoryGrid != null)
                {
                    if (!InventoryGrid.IsDisposed)
                        InventoryGrid.Dispose();

                    InventoryGrid = null;
                }

                if (InventoryGridScrollBar != null)
                {
                    if (!InventoryGridScrollBar.IsDisposed)
                        InventoryGridScrollBar.Dispose();

                    InventoryGridScrollBar = null;
                }

                if (WeightLabel != null)
                {
                    if (!WeightLabel.IsDisposed)
                        WeightLabel.Dispose();

                    WeightLabel = null;
                }

                if (HungerLabel != null)
                {
                    if (!HungerLabel.IsDisposed)
                        HungerLabel.Dispose();

                    HungerLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (ExperienceLabel != null)
                {
                    if (!ExperienceLabel.IsDisposed)
                        ExperienceLabel.Dispose();

                    ExperienceLabel = null;
                }

                if (Level3Label != null)
                {
                    if (!Level3Label.IsDisposed)
                        Level3Label.Dispose();

                    Level3Label = null;
                }

                if (Level5Label != null)
                {
                    if (!Level5Label.IsDisposed)
                        Level5Label.Dispose();

                    Level5Label = null;
                }

                if (Level7Label != null)
                {
                    if (!Level7Label.IsDisposed)
                        Level7Label.Dispose();

                    Level7Label = null;
                }

                if (Level10Label != null)
                {
                    if (!Level10Label.IsDisposed)
                        Level10Label.Dispose();

                    Level10Label = null;
                }

                if (ModeComboBox != null)
                {
                    if (!ModeComboBox.IsDisposed)
                        ModeComboBox.Dispose();

                    ModeComboBox = null;
                }
                BagWeight = 0;
                MaxBagWeight = 0;
                InventorySize = 0;
            }
        }
        #endregion
    }
}
