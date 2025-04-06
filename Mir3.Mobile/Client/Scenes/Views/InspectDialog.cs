using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using System;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 查看其它玩家角色属性界面
    /// </summary>
    public sealed class InspectDialog : DXWindow
    {
        #region Properties
        private DXImageControl CharacterTab;   //查看玩家属性主界面
        public DXLabel CharacterNameLabel, GuildNameLabel, GuildRankLabel, MarriageLabel;   //名字 行会名字 行会等级 婚姻名字 标签
        public DXImageControl MarriageIcon;  //婚姻图标
        public DXButton Close1Button, PMButton, GroupButton;  //私聊 组队 按钮

        public DXItemCell[] Grid;   //装备格子

        public ClientUserItem[] Equipment = new ClientUserItem[Globals.EquipmentSize];  //装备格子定义
        public MirClass Class;
        public MirGender Gender;
        public int HairType;
        public Color HairColour;
        public int Level;

        public DXImageControl GuildFlag; //旗帜显示

        private bool HideHelmet, HideFashion, HideShield;

        public override WindowType Type => WindowType.InspectBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 查看其它玩家角色属性界面
        /// </summary>
        public InspectDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size = UI1Library.GetSize(1288);
            Location = new Point(0, 0);

            CharacterTab = new DXImageControl   //角色
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1288,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };
            CharacterTab.AfterDraw += CharacterTab_AfterDraw;

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1425,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(248, 310),
            };
            Close1Button.TouchUp += (o, e) => Visible = false;

            DXControl namePanel = new DXControl
            {
                Parent = CharacterTab,
                Size = new Size(150, 75),
                Location = new Point((CharacterTab.Size.Width - 150) / 2 + 3, 14),

            };
            CharacterNameLabel = new DXLabel     //角色名字标签
            {
                AutoSize = false,
                Size = new Size(150, 20),
                ForeColour = Color.FromArgb(222, 255, 222),
                Outline = false,
                Parent = namePanel,
                //Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            var s = CEnvir.GetFontSize(CharacterNameLabel.Font);
            CharacterNameLabel.Size = new Size(150, s.Height);
            GuildNameLabel = new DXLabel        //角色行会名字标签
            {
                AutoSize = false,
                Size = new Size(150, s.Height),
                ForeColour = Color.FromArgb(255, 255, 181),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height - 2),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            GuildRankLabel = new DXLabel        //角色行会等级标签
            {
                AutoSize = false,
                Size = new Size(150, s.Height),
                ForeColour = Color.FromArgb(255, 206, 148),
                Outline = false,
                Parent = namePanel,
                Location = new Point(0, CharacterNameLabel.Size.Height + GuildNameLabel.Size.Height - 4),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            namePanel.Size = new Size(150, GuildRankLabel.Location.Y + GuildRankLabel.Size.Height);

            MarriageIcon = new DXImageControl     //结婚图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 1298,
                Location = new Point(12 + 131, namePanel.Size.Height - 8),
                Visible = false,
            };

            MarriageLabel = new DXLabel      //结婚对象名字
            {
                AutoSize = false,
                ForeColour = Color.Magenta,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(30 + 131, namePanel.Size.Height - 11),
                Size = new Size(110, 15),
                IsControl = false,
            };

            GuildFlag = new DXImageControl
            {
                Parent = CharacterTab,
                Visible = false,
                LibraryFile = LibraryFile.UI1,
                Movable = false,
                IsControl = false,
                Location = new Point(5, 30)
            };

            Grid = new DXItemCell[Globals.EquipmentSize];

            DXItemCell cell, weaponCell, helmetCell, armourCell, necklaceCell;
            Grid[(int)EquipmentSlot.Weapon] = weaponCell = new DXItemCell  //武器
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(60, 58),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                Slot = (int)EquipmentSlot.Weapon,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Size = new Size(55, 151),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 35);

            Grid[(int)EquipmentSlot.Helmet] = helmetCell = new DXItemCell  //头盔
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(125, 80),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                Slot = (int)EquipmentSlot.Helmet,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Size = new Size(71, 50),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 37);

            Grid[(int)EquipmentSlot.Armour] = armourCell = new DXItemCell   //衣服
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(120, 120),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                Slot = (int)EquipmentSlot.Armour,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Size = new Size(helmetCell.Size.Width, 155),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 34);

            Grid[(int)EquipmentSlot.Shield] = cell = new DXItemCell   //盾牌
            {
                DrawItemType = DrawItemType.Null,
                Location = new Point(170, 138),
                Parent = CharacterTab,
                FixedBorder = true,
                Border = false,
                Opacity = 0F,
                Slot = (int)EquipmentSlot.Shield,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Size = new Size(40, armourCell.Size.Height),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 105);         

            Grid[(int)EquipmentSlot.Torch] = cell = new DXItemCell      //火把
            {
                Location = new Point(98, 293),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Torch,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 38);

            Grid[(int)EquipmentSlot.Necklace] = necklaceCell = new DXItemCell   //项链
            {
                DrawItemType = DrawItemType.OnlyImage,
                Location = new Point(221, 90),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                BorderColour = Color.Red,
                Slot = (int)EquipmentSlot.Necklace,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 33);

            Grid[(int)EquipmentSlot.BraceletL] = cell = new DXItemCell   //左手镯
            {
                Location = new Point(21, 181),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.BraceletL,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);

            Grid[(int)EquipmentSlot.BraceletR] = cell = new DXItemCell   //右手镯
            {
                Location = new Point(223, 181),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.BraceletR,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 32);

            Grid[(int)EquipmentSlot.RingL] = cell = new DXItemCell    //左戒指
            {
                Location = new Point(21, 221),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.RingL,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);

            Grid[(int)EquipmentSlot.RingR] = cell = new DXItemCell    //右戒指
            {
                Location = new Point(223, 221),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.RingR,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 31);

            Grid[(int)EquipmentSlot.Emblem] = cell = new DXItemCell   //勋章
            {
                Location = new Point(223, 137),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Emblem,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 104);

            Grid[(int)EquipmentSlot.Shoes] = cell = new DXItemCell   //鞋子
            {
                LibraryFile = LibraryFile.Inventory,
                Location = new Point(22, 264),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Shoes,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Size = new Size(necklaceCell.Size.Width, 72),
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 36);

            Grid[(int)EquipmentSlot.Poison] = cell = new DXItemCell   //毒
            {
                Location = new Point(180, 293),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Poison,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 40);

            Grid[(int)EquipmentSlot.Amulet] = cell = new DXItemCell  //符
            {
                Location = new Point(139, 293),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Amulet,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 39);

            Grid[(int)EquipmentSlot.Flower] = cell = new DXItemCell  //花
            {
                Location = new Point(223, 293),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Flower,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 81);

            Grid[(int)EquipmentSlot.HorseArmour] = cell = new DXItemCell  //马具
            {
                Location = new Point(221, 48),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.HorseArmour,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 82);

            Grid[(int)EquipmentSlot.FameTitle] = cell = new DXItemCell  //声望称号
            {
                Location = new Point(212 - 16, 15 - 16),
                Size = new Size(76, 76),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.FameTitle,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 81);

            Grid[(int)EquipmentSlot.Fashion] = cell = new DXItemCell  //时装
            {
                Location = new Point(13, 163),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Fashion,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Visible = false,
            };
            //cell.BeforeDraw += (o, e) => Draw((DXItemCell)o, 34);

            Grid[(int)EquipmentSlot.Medicament] = cell = new DXItemCell  //自动恢复药剂
            {
                Location = new Point(227, 94),
                Parent = CharacterTab,
                FixedBorder = false,
                Border = false,
                Slot = (int)EquipmentSlot.Medicament,
                ItemGrid = Equipment,
                GridType = GridType.Inspect,
                ReadOnly = true,
                Visible = false,
            };

            PMButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1256,
                Parent = CharacterTab,
                Location = new Point(35, 354),
                Hint = "点我私聊".Lang(),
            };

            GroupButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1255,
                Parent = CharacterTab,
                Location = new Point(PMButton.Location.X + PMButton.Size.Width + 10, PMButton.Location.Y),
                Hint = "点我组队".Lang(),
            };
        }

        #region Methods
        /// <summary>
        /// 绘制
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void Draw(DXItemCell cell, int index)
        {
            if (InterfaceLibrary == null) return;

            Size s = InterfaceLibrary.GetSize(index);
            int x = (cell.Size.Width - s.Width) / 2 + cell.DisplayArea.X;
            int y = (cell.Size.Height - s.Height) / 2 + cell.DisplayArea.Y;

            InterfaceLibrary.Draw(index, x, y, Color.White, false, 0.3F, ImageType.Image);
        }

        /// <summary>
        /// 在图像前绘制 装备内观
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CharacterTab_AfterDraw(object sender, EventArgs e)
        {
            MirLibrary library;
            int x = 120;
            int y = 230;

            #region 逻辑
            bool drawBody = true, drawArmour = false, drawFashion = false, drawWeapon = false, drawShield = false, drawHelmet = false, drawHair = false;
            DXItemCell armour = Grid[(int)EquipmentSlot.Armour],
                           fashion = Grid[(int)EquipmentSlot.Fashion],
                           weapon = Grid[(int)EquipmentSlot.Weapon],
                           shield = Grid[(int)EquipmentSlot.Shield],
                           helmet = Grid[(int)EquipmentSlot.Helmet];
            bool fishshape = armour.Item != null && armour.Item.Info.Shape == 16; //是不是钓鱼服
            //时装
            //如果 已装配时装 并且 不隐藏时装 并且 不是钓鱼服 就画时装
            //if (fashion.Item != null && Player.FashionShape >= 0 && !fishshape)
            if (fashion.Item != null && !HideFashion && !fishshape)
                drawFashion = true;

            //衣服 
            //如果 已装配衣服 并且 (隐藏时装 或者 时装为空 或者 是钓鱼服) 就画衣服
            //if (armour.Item != null && !drawFashion)
            if (armour.Item != null && (HideFashion || fashion.Item == null || fishshape))
                drawArmour = true;

            //裸体  
            //如果 画时装 并且 时装在不显示本体列表中) 就不显示裸体
            if (drawFashion && Globals.ArmourWithBodyList.Contains(fashion.Item.Info.Image))
                drawBody = false;

            //武器
            //如果 已装配武器 并且 (隐藏时装 或者 未装配时装 或者 时装不在自带武器衣服列表中 或者 钓鱼服） 就画武器
            //if (weapon.Item != null && (Player.FashionShape < 0 || fashion.Item == null || !Globals.ArmourWithWeaponList.Contains(fashion.Item.Info.Image) || fishshape))
            if (weapon.Item != null && (HideFashion || fashion.Item == null || !Globals.ArmourWithWeaponList.Contains(fashion.Item.Info.Image) || fishshape))
                drawWeapon = true;

            //盾牌
            //如果 已装配盾牌 并且 不隐藏盾牌 并且 盾牌shape小于1000 就画盾牌
            //if (shield.Item != null && Player.ShieldShape > 0 && Player.ShieldShape < 1000)
            if (shield.Item != null && !HideShield)
                drawShield = true;
            shield.Visible = drawShield;

            //头盔
            //如果 装配头盔 并且 不隐藏头盔 并且 不是钓鱼服 并且 不画时装 就画头盔
            //if (helmet.Item != null && Player.HelmetShape > 0 && !fishshape && !drawFashion)
            if (helmet.Item != null && !fishshape && !drawFashion)  //&& !HideHelmet 
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
                        //bool oldBlend = DXManager.Blending;
                        //float oldRate = DXManager.BlendRate;

                        //DXManager.SetBlend(true, 0.8F);
                        PresentMirImage(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                        //DXManager.SetBlend(oldBlend, oldRate);
                    }
                }
                #endregion
            }
            #endregion

            if (CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out library))
            {
                //女刺客的长辫子 放到最底层
                if (drawHair)
                    if (Class == MirClass.Assassin && Gender == MirGender.Female && HairType == 1)
                        library.Draw(1160, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, HairColour, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                #region 裸体
                if (drawBody)
                {
                    switch (Gender)
                    {
                        case MirGender.Male:  //男
                            library.Draw(0, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                            break;
                        case MirGender.Female:  //女
                            library.Draw(1, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
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
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, armour.Item.Colour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    #endregion
                }
                #endregion

                #region 时装内观
                if (drawFashion)
                {
                    int index = fashion.Item.Info.Image;

                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, fashion.Item.Colour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
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
                            if (Class == MirClass.Warrior)
                            {
                                image = effectLibrary.CreateImage(600, ImageType.Image);
                            }
                            if (Class == MirClass.Wizard)
                            {
                                image = effectLibrary.CreateImage(601, ImageType.Image);
                            }
                            if (Class == MirClass.Taoist)
                            {
                                image = effectLibrary.CreateImage(602, ImageType.Image);
                            }
                            break;
                        case 996:
                            if (Class == MirClass.Warrior)
                            {
                                image = effectLibrary.CreateImage(620, ImageType.Image);
                            }
                            if (Class == MirClass.Wizard)
                            {
                                image = effectLibrary.CreateImage(621, ImageType.Image);
                            }
                            if (Class == MirClass.Taoist)
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
                        PresentMirImage(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
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
                    if (HairType > 0)    //发型 单独处理 不能用自定义特效功能
                    {
                        switch (Class)
                        {
                            case MirClass.Warrior:
                            case MirClass.Wizard:
                            case MirClass.Taoist:
                                switch (Gender)
                                {
                                    case MirGender.Male:
                                        library.Draw(60 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        library.Draw(60 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, HairColour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        break;
                                    case MirGender.Female:
                                        library.Draw(80 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        library.Draw(80 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, HairColour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        break;
                                }
                                break;
                            case MirClass.Assassin:
                                switch (Gender)
                                {
                                    case MirGender.Male:
                                        library.Draw(1100 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        library.Draw(1100 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, HairColour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        break;
                                    case MirGender.Female:
                                        library.Draw(1120 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                                        library.Draw(1120 + HairType - 1, DisplayArea.X + x - 1, DisplayArea.Y + y - 1, HairColour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
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
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    library.Draw(index, DisplayArea.X + x, DisplayArea.Y + y, helmet.Item.Colour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                }
                #endregion

                #region 武器内观
                if (drawWeapon)
                {
                    //int imageIndex = weapon.Item.Info.Image;
                    int imageIndex = CEnvir.GetItemIllusionItemInfo(Grid[(int)EquipmentSlot.Weapon].Item);

                    #region 画武器
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, weapon.Item.Colour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
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

                            PresentMirImage(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

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
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, Color.White, true, 1F, ImageType.Image, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                    library.Draw(imageIndex, DisplayArea.X + x, DisplayArea.Y + y, shield.Item.Colour, true, 1F, ImageType.Overlay, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
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

                            PresentMirImage(image.Image, CharacterTab, new Rectangle(DisplayArea.X + x + image.OffSetX, DisplayArea.Y + y + image.OffSetY, image.Width, image.Height), ForeColour, this, 0, 0, true, 0.8f, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                            //DXManager.SetBlend(oldBlend, oldRate);
                        }
                    }
                    #endregion
                }
                #endregion
            }
        }

        /// <summary>
        /// 更新角色信息
        /// </summary>
        /// <param name="p"></param>
        public void NewInformation(S.Inspect p)
        {
            Visible = true;
            CharacterNameLabel.Text = p.Name;   //角色名字标签
            GuildNameLabel.Text = p.GuildName;  //行会名字标签
            GuildRankLabel.Text = p.GuildRank;  //行会等级标签

            if (p.GuildName != "")
            {
                GuildFlag.Visible = true;
                GuildFlag.Index = 1690 + p.GuildFlag;
                GuildFlag.AfterDraw += (o, e) =>
                {
                    MirLibrary library;
                    if (!CEnvir.LibraryList.TryGetValue(LibraryFile.UI1, out library)) return;
                    library.Draw(1690 + p.GuildFlag, GuildFlag.Location.X, GuildFlag.Location.Y, p.GuildFlagColor, false, 1f, ImageType.Overlay);
                };
            }
            else
            {
                GuildFlag.Visible = false;
            }

            Gender = p.Gender;  //性别
            Class = p.Class;    //职业
            Level = p.Level;    //等级

            MarriageIcon.Visible = !string.IsNullOrEmpty(p.Partner);  //结婚图标
            MarriageLabel.Text = p.Partner;   //配偶名字

            HairColour = p.HairColour;   //发型颜色
            HairType = p.Hair;           //发型类型

            HideHelmet = p.HideHelmet; //隐藏头盔
            HideFashion = p.HideFashion;  //隐藏时装
            HideShield = p.HideShield; //隐藏盾牌

            foreach (DXItemCell cell in Grid)
                cell.Item = null;

            foreach (ClientUserItem item in p.Items)
                Grid[item.Slot].Item = item;

            PMButton.MouseClick += (o, e) =>                  //点我私聊
            {
                GameScene.Game.ChatTextBox.StartPM(p.Name);
            };

            GroupButton.MouseClick += (o, e) =>              //点我组队
            {
                if (GameScene.Game.Observer) return;

                if (GameScene.Game.GroupBox.Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat("组队人数已达到上限".Lang(), MessageType.System);
                    return;
                }

                if (GameScene.Game.GroupBox.Members.Count >= Globals.GroupLimit)
                {
                    GameScene.Game.ReceiveChat("你不是队长无权限操作".Lang(), MessageType.System);
                    return;
                }
                CEnvir.Enqueue(new C.GroupInvite { Name = CharacterNameLabel.Text });
            };
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Equipment != null)
                {
                    for (int i = 0; i < Equipment.Length; i++)
                    {
                        Equipment[i] = null;
                    }

                    Equipment = null;
                }

                Class = 0;
                Gender = 0;
                HairType = 0;
                HairColour = Color.Empty;
                Level = 0;

                if (CharacterTab != null)
                {
                    if (!CharacterTab.IsDisposed)
                        CharacterTab.Dispose();

                    CharacterTab = null;
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

                if (GuildFlag != null)
                {
                    if (!GuildFlag.IsDisposed)
                        GuildFlag.Dispose();

                    GuildFlag = null;
                }
            }
        }
        #endregion
    }
}
