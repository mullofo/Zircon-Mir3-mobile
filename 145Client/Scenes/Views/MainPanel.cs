using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// UI底部主面板
    /// </summary>
    public sealed class MainPanel : DXImageControl
    {
        #region Properties

        //左边的按钮
        public DXImageControl ExperienceBar;

        public DXControl HealthBar, ManaBar, BagWeightBar;

        public DXLabel HealthLabel, ManaLabel, ExperienceLabel, WeightLabel, GridLabel;

        //右边的按钮
        public DXImageControl NewMailIcon, CompletedQuestIcon, AvailableQuestIcon;

        public DXLabel ACLabel, MRLabel, DCLabel, MCLabel, SCLabel, AttackModeLabel, PetModeLabel;
        public DXButton Trade, MiniMap, MagicBar, CompanionButton;
        public DXImageControl AClabel, MRlabel, DClabel, MClabel, SClabel;

        #endregion

        public static float _percent = 0.0f;
        /// <summary>
        /// 底部UI界面左边框
        /// </summary>
        public MainPanel()
        {
            LibraryFile = LibraryFile.UI1;  //UI面板主图的索引位置
            Index = 1100;  //UI主图序号
            PassThrough = true;  //穿透开启

            #region 左边组建

            ExperienceBar = new DXImageControl  //经验值槽
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,  //经验槽底图
                Index = 1156,
            };
            ExperienceBar.Location = new Point(150, 71);  //经验槽底图所在坐标
            ExperienceBar.AfterDraw += (o, e) =>  //在经验槽底图前面绘制
            {
                if (ExperienceBar.Library == null) return;  //如果经验槽不存在 返回

                if (MapObject.User.Level >= Globals.GamePlayEXPInfoList.Count) return;  //如果玩家的等级大于或等于经验列表的等级 返回

                decimal MaxExperience = Globals.GamePlayEXPInfoList[MapObject.User.Level].Exp;  //最大经验值 = 经验列表里的等级经验值

                if (MaxExperience == 0) return;  //如果 最大经验值为零 返回

                //浮动百分比 = 浮动数字最小值（1，最大值（0，玩家经验值/最大经验值））
                float percent = (float)Math.Min(1, Math.Max(0, MapObject.User.Experience / MaxExperience));

                if (percent == 0) return;  //如果浮动百分比 为零 返回

                //获取百分比
                Size image = ExperienceBar.Library.GetSize(1166);

                if (image == Size.Empty) return;

                int offset = (int)(image.Height * (1.0 - percent));
                Rectangle area = new Rectangle(0, offset, image.Width, image.Height - offset);
                ExperienceBar.Library.Draw(1166, ExperienceBar.DisplayArea.X + 3, ExperienceBar.DisplayArea.Y + 2 + offset, Color.White, area, 1F, ImageType.Image);
            };

            HealthBar = new DXControl  //血量槽
            {
                Parent = this,
                Location = new Point(33, 55),
                Size = ExperienceBar.Library.GetSize(1163),
            };
            HealthBar.BeforeDraw += (o, e) =>
            {
                if (ExperienceBar.Library == null) return;

                if (MapObject.User.Stats[Stat.Health] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.CurrentHP / (float)MapObject.User.Stats[Stat.Health]));

                if (percent == 0) return;

                Size image = ExperienceBar.Library.GetSize(1163);

                if (image == Size.Empty) return;

                int offset = (int)(image.Height * (1.0 - percent));
                Rectangle area = new Rectangle(0, offset, image.Width, image.Height - offset);
                ExperienceBar.Library.Draw(1163, HealthBar.DisplayArea.X, HealthBar.DisplayArea.Y + offset, Color.White, area, 1F, ImageType.Image);
            };

            ManaBar = new DXControl  //蓝槽
            {
                Parent = this,
                Location = new Point(73, 55),
                Size = ExperienceBar.Library.GetSize(1164),
            };
            ManaBar.BeforeDraw += (o, e) =>
            {
                if (ExperienceBar.Library == null) return;

                if (MapObject.User.Stats[Stat.Mana] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.CurrentMP / (float)MapObject.User.Stats[Stat.Mana]));

                if (percent == 0) return;

                Size image = ExperienceBar.Library.GetSize(1164);

                if (image == Size.Empty) return;

                int offset = (int)(image.Height * (1.0 - percent));
                Rectangle area = new Rectangle(0, offset, image.Width, image.Height - offset);
                ExperienceBar.Library.Draw(1164, ManaBar.DisplayArea.X, ManaBar.DisplayArea.Y + offset, Color.White, area, 1F, ImageType.Image);
            };

            BagWeightBar = new DXControl      //负重经验条
            {
                Parent = this,
                Location = new Point(162, 71),
                Size = ExperienceBar.Library.GetSize(1167),
            };
            BagWeightBar.BeforeDraw += (o, e) =>
            {
                if (ExperienceBar.Library == null) return;

                if (MapObject.User.Stats[Stat.BagWeight] == 0) return;

                float percent = Math.Min(1, Math.Max(0, MapObject.User.BagWeight / (float)MapObject.User.Stats[Stat.BagWeight]));

                if (percent == 0) return;

                Size image = ExperienceBar.Library.GetSize(1167);

                if (image == Size.Empty) return;

                int offset = (int)(image.Height * (1.0 - percent));
                Rectangle area = new Rectangle(0, offset, image.Width, image.Height - offset);
                ExperienceBar.Library.Draw(1167, BagWeightBar.DisplayArea.X + 3, BagWeightBar.DisplayArea.Y + 2 + offset, Color.White, area, 1F, ImageType.Image);
            };

            DXButton BeltButton = new DXButton  //药物快捷栏按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1205,
                Parent = this,
                Location = new Point(148, 48),
                Hint = "物品快捷栏".Lang() + "(Ctrl+Z, Z)"
            };
            BeltButton.MouseClick += (o, e) => GameScene.Game.BeltBox.Visible = !GameScene.Game.BeltBox.Visible;

            DXButton ToSelectButton = new DXButton   //小退按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1178,
                Parent = this,
                Location = new Point(108, 120),
                Hint = "退出".Lang() + "(Alt+X)"
            };
            ToSelectButton.MouseClick += (o, e) =>
            {
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && !BigPatchConfig.ChkQuickSelect)
                {
                    GameScene.Game.ReceiveChat("战斗中无法退出游戏".Lang(), MessageType.System);
                    return;
                }

                if (BigPatchConfig.ChkQuickSelect)
                {
                    CEnvir.Enqueue(new C.Logout());
                }
                else
                {
                    GameScene.Game.ExitBox.Visible = true;
                    GameScene.Game.ExitBox.BringToFront();
                }
            };

            DXButton ExitButton = new DXButton   //结束游戏按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1176,
                Parent = this,
                Location = new Point(7, 120),
                Hint = "结束游戏".Lang() + "(Alt+Q)"
            };
            ExitButton.MouseClick += (o, e) =>
            {
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && !BigPatchConfig.ChkQuickSelect)
                {
                    GameScene.Game.ReceiveChat("战斗中无法退出游戏".Lang(), MessageType.System);
                    return;
                }

                if (BigPatchConfig.ChkQuickSelect)
                {
                    CEnvir.Enqueue(new C.Logout());
                }
                else
                {
                    GameScene.Game.ExitBox.Visible = true;
                    GameScene.Game.ExitBox.BringToFront();
                }
            };

            HealthLabel = new DXLabel   //血量标签
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.Cyan,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(60, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Location = new Point(2, 139),
                Text = "红".Lang(),
            };

            ManaLabel = new DXLabel   //蓝标签
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.Cyan,
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(60, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Left,
                Location = new Point(78, 139),
                Text = "蓝".Lang(),
            };

            ExperienceLabel = new DXLabel   //经验值标签
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            ExperienceLabel.SizeChanged += (o, e) =>
            {
                ExperienceLabel.Location = new Point(ExperienceBar.Location.X + (ExperienceBar.Size.Width - ExperienceLabel.Size.Width) / 2, ExperienceBar.Location.Y + (ExperienceBar.Size.Height - ExperienceLabel.Size.Height) / 2);
            };

            WeightLabel = new DXLabel   //负重标签
            {
                Parent = this,
                ForeColour = Color.White,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            WeightLabel.SizeChanged += (o, e) =>
            {
                WeightLabel.Location = new Point(BagWeightBar.Location.X + (BagWeightBar.Size.Width - WeightLabel.Size.Width) / 2, BagWeightBar.Location.Y + (BagWeightBar.Size.Height - WeightLabel.Size.Height) / 2);
            };

            GridLabel = new DXLabel               //坐标颜色字体
            {
                Parent = this,
                AutoSize = false,
                ForeColour = Color.FromArgb(255, 255, 200),
                Outline = true,
                OutlineColour = Color.Black,
                Size = new Size(150, 20),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Location = new Point(1, 155),
                Text = "坐标".Lang(),
            };
            //GameScene.Game.MapControl.MapInfoChanged += MapControl_MapInfoChanged;

            #endregion

            #region 右边组建

            DXButton CharacterButton = new DXButton  //人物属性按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1196,
                Parent = this,
                Location = new Point(934, 78),
                Hint = "角色窗口".Lang() + "(Ctrl+Q, Q)"
            };
            CharacterButton.MouseClick += (o, e) =>
            {
                GameScene.Game.CharacterBox.Visible = !GameScene.Game.CharacterBox.Visible;
            };

            DXButton InventoryButton = new DXButton  //背包按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1194,
                Parent = this,
                Location = new Point(905, 80),
                Hint = "包裹窗口".Lang() + "(Ctrl+W, W)"
            };
            InventoryButton.MouseClick += (o, e) => GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;

            DXButton SpellButton = new DXButton  //技能按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1184,
                Parent = this,
                Location = new Point(961, 80),
                Hint = "魔法窗口".Lang() + "(Ctrl+E, E)"
            };
            SpellButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;   //如果是观察者 返回
                GameScene.Game.MagicBox.Visible = !GameScene.Game.MagicBox.Visible;
            };

            DXButton QuestButton = new DXButton  //任务按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1186,
                Parent = this,
                Location = new Point(965, 58),
                Hint = "任务窗口".Lang() + "(D)"
            };
            QuestButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                if (GameScene.Game.NPCBox.Visible == true)
                {
                    GameScene.Game.NPCBox.Visible = false;
                    return;
                }
                if (GameScene.Game.NPCDKeyBox.Visible == true)
                {
                    GameScene.Game.NPCDKeyBox.Visible = false;
                    return;
                }
                CEnvir.Enqueue(new C.DKey { });
            };

            DXButton MailButton = new DXButton  //邮件按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1190,
                Parent = this,
                Location = new Point(966, 108),
                Hint = "邮件窗口".Lang() + "(L)"
            };
            MailButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.CommunicationBox.Visible = !GameScene.Game.CommunicationBox.Visible;
            };

            NewMailIcon = new DXImageControl   //新邮件提示图片
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 240,
                Parent = MailButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };

            AvailableQuestIcon = new DXImageControl  //任务提示图片
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 240,
                Parent = QuestButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };
            AvailableQuestIcon.VisibleChanged += (o, e) =>
            {
                if (AvailableQuestIcon.Visible)
                    CompletedQuestIcon.Location = new Point(849, QuestButton.Size.Height - CompletedQuestIcon.Size.Height);
                else
                    CompletedQuestIcon.Location = new Point(849, 10);
            };

            CompletedQuestIcon = new DXImageControl  //完成任务提示图片
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 241,
                Parent = QuestButton,
                IsControl = false,
                Location = new Point(2, 2),
                Visible = false,
            };

            DXButton GuildButton = new DXButton  //行会按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1182,
                Parent = this,
                Location = new Point(897, 58),
                Hint = "行会".Lang() + "(Ctrl+F, F)"
            };
            GuildButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.GuildBox.Visible = !GameScene.Game.GuildBox.Visible;
            };

            DXButton GroupButton = new DXButton  //组队按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1180,
                Parent = this,
                Location = new Point(929, 54),
                Hint = "小组".Lang() + "(G)"
            };
            GroupButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;
                GameScene.Game.GroupBox.Visible = !GameScene.Game.GroupBox.Visible;
            };

            DXButton ConfigButton = new DXButton  //主菜单
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1188,
                Parent = this,
                Location = new Point(929, 119),
                Hint = "环境设置".Lang() + "(N)"
            };
            ConfigButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;   //如果是观察者 返回
                GameScene.Game.ConfigBox.Visible = !GameScene.Game.ConfigBox.Visible;
            };

            DXButton CashShopButton = new DXButton     //商城按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1192,
                Parent = this,
                Location = new Point(897, 108),
                Hint = "商城".Lang() + "(Ctrl+Y)"
            };
            CashShopButton.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.MarketPlaceBox.Visible = !GameScene.Game.MarketPlaceBox.Visible;
            };

            AClabel = new DXImageControl     //物防图片
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1500,
                Parent = this,
                Location = new Point(857, 152),
                Hint = "物防".Lang(),
            };
            AClabel.MouseClick += (o, e) =>
            {
                AClabel.Visible = false;
                MRlabel.Visible = true;
                ACLabel.Visible = false;
                MRLabel.Visible = true;
            };

            MRlabel = new DXImageControl     //魔防图片
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1502,
                Parent = this,
                Location = new Point(857, 152),
                Hint = "魔防".Lang(),
                Visible = false,
            };
            MRlabel.MouseClick += (o, e) =>
            {
                AClabel.Visible = true;
                MRlabel.Visible = false;
                ACLabel.Visible = true;
                MRLabel.Visible = false;
            };

            DClabel = new DXImageControl   //攻击图片
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1501,
                Parent = this,
                Hint = "破坏".Lang(),
            };
            DClabel.Location = new Point(941, 152);
            DClabel.MouseClick += (o, e) =>
             {
                 DClabel.Visible = false;
                 DCLabel.Visible = false;
                 if (GameScene.Game.User.Class != MirClass.Taoist)
                 {
                     MClabel.Visible = true;
                     MCLabel.Visible = true;
                 }
                 else
                 {
                     SClabel.Visible = true;
                     SCLabel.Visible = true;
                 };
             };

            MClabel = new DXImageControl   //自然图片
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1503,
                Parent = this,
                Hint = "自然".Lang(),
            };
            MClabel.Location = new Point(941, 152);
            MClabel.MouseClick += (o, e) =>
            {
                DClabel.Visible = true;
                DCLabel.Visible = true;
                MClabel.Visible = false;
                MCLabel.Visible = false;
            };

            SClabel = new DXImageControl   //灵魂图片
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1504,
                Parent = this,
                Hint = "灵魂".Lang(),
            };
            SClabel.Location = new Point(941, 152);
            SClabel.MouseClick += (o, e) =>
            {
                DClabel.Visible = true;
                DCLabel.Visible = true;
                SClabel.Visible = false;
                SCLabel.Visible = false;
            };

            ACLabel = new DXLabel   //防御值标签
            {
                AutoSize = false,
                Parent = this,
                ForeColour = Color.FromArgb(255, 255, 200, 50),
                Location = new Point(884, 154),
                Size = new Size(50, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            MRLabel = new DXLabel   //魔防值标签
            {
                AutoSize = false,
                Parent = this,
                ForeColour = Color.FromArgb(255, 255, 200, 50),
                Location = new Point(884, 154),
                Size = new Size(50, 16),
                Visible = false,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };

            DCLabel = new DXLabel    //攻击值标签
            {
                AutoSize = false,
                Parent = this,
                ForeColour = Color.FromArgb(255, 255, 200, 50),
                Location = new Point(969, 154),
                Size = new Size(50, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            DCLabel.VisibleChanged += (o, e) => DClabel.Visible = DCLabel.Visible;

            MCLabel = new DXLabel   //自然值标签
            {
                AutoSize = false,
                Parent = this,
                ForeColour = Color.FromArgb(255, 255, 200, 50),
                Location = new Point(969, 154),
                Size = new Size(50, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            MCLabel.VisibleChanged += (o, e) => MClabel.Visible = MCLabel.Visible;

            SCLabel = new DXLabel   //灵魂值标签
            {
                AutoSize = false,
                Parent = this,
                ForeColour = Color.FromArgb(255, 255, 200, 50),
                Location = new Point(969, 154),
                Size = new Size(50, 16),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter
            };
            SCLabel.VisibleChanged += (o, e) => SClabel.Visible = SCLabel.Visible;

            Trade = new DXButton      //交易快捷
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1170,
                Parent = this,
                Location = new Point(861, 56),
                Hint = "交易窗口".Lang() + "(Ctrl+C, C)"
            };
            Trade.MouseClick += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                //如果玩家 为空  或者 玩家不是面对面交易
                string ErrorStr = "需要选定目标".Lang();
                do
                {
                    if (GameScene.Game.TargetObject == null) break;

                    ErrorStr = "选定目标是玩家".Lang();
                    MapObject ob = GameScene.Game.TargetObject;// as PlayerObject;
                    if (!(ob is PlayerObject)) break;

                    //反方向
                    ErrorStr = "目标必须要与自己面对面".Lang();
                    if (ob.Direction != Functions.ShiftDirection(MapObject.User.Direction, 4)) break;

                    //距一步
                    ErrorStr = "必须靠近目标".Lang();
                    if (1 != Functions.Distance(ob.CurrentLocation, MapObject.User.CurrentLocation)) break;

                    if (Functions.Move(ob.CurrentLocation, ob.Direction, 1) != MapObject.User.CurrentLocation) break;

                    //GameScene.Game.TradeBox.IsVisible = true;

                    CEnvir.Enqueue(new C.TradeRequest());
                    return;
                } while (false);

                DXMessageBox.Show($"MainPanel.TradeShow".Lang(ErrorStr), "交易无法进行".Lang());

                GameScene.Game.ReceiveChat("MainPanel.TradeReceiveChat".Lang(), MessageType.System);
            };

            MiniMap = new DXButton   //小地图快捷
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1172,
                Parent = this,
                Location = new Point(861, 89),
                Hint = "小地图".Lang() + "(V)"
            };
            MiniMap.MouseClick += (o, e) => GameScene.Game.MiniMapBox.Visible = !GameScene.Game.MiniMapBox.Visible;

            MagicBar = new DXButton  //技能快捷栏
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1174,
                Parent = this,
                Location = new Point(862, 124),
                Hint = "魔法快捷键窗口".Lang() + "(Ctrl+B, B)"
            };
            MagicBar.MouseClick += (o, e) => GameScene.Game.MagicBarBox.Visible = !GameScene.Game.MagicBarBox.Visible;

            AttackModeLabel = new DXLabel   //攻击模式切换
            {
                Parent = this,
                ForeColour = Color.Cyan,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
            };
            AttackModeLabel.SizeChanged += (o, e) =>
            {
                AttackModeLabel.Location = new Point(955, 20 + 15);
            };

            PetModeLabel = new DXLabel   //宠物模式切换
            {
                Parent = this,
                ForeColour = Color.Cyan,
                Outline = true,
                OutlineColour = Color.Black,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Visible = false,
            };
            PetModeLabel.SizeChanged += (o, e) =>
            {
                PetModeLabel.Location = new Point(955, 20);
            };

            DXButton CompanionButton = new DXButton  //宠物按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1199,
                Parent = this,
                Location = new Point(996, 88),
                Visible = false,
            };
            if (Globals.CompanionInfoList.Count == 0)
            {
                CompanionButton.Visible = false;
            }
            else
            {
                CompanionButton.Visible = true;
                CompanionButton.Hint = "宠物".Lang() + "(I)";
                CompanionButton.MouseClick += (o, e) => GameScene.Game.CompanionBox.Visible = !GameScene.Game.CompanionBox.Visible;
            }

            #endregion
        }

        public void MapControl_MapInfoChanged(object sender, EventArgs e)
        {
            if (GameScene.Game.MapControl.MapInfo == null) return;

            if (MapObject.User == null) return;
            //GridLabel.Text = GameScene.Game.MapControl.MapInfo.Lang(p => p.Description) + $" {MapObject.User.CurrentLocation.X}:{MapObject.User.CurrentLocation.Y}";   //地图坐标

        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ExperienceBar != null)
                {
                    if (!ExperienceBar.IsDisposed)
                        ExperienceBar.Dispose();

                    ExperienceBar = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

                if (ManaLabel != null)
                {
                    if (!ManaLabel.IsDisposed)
                        ManaLabel.Dispose();

                    ManaLabel = null;
                }

                if (ExperienceLabel != null)
                {
                    if (!ExperienceLabel.IsDisposed)
                        ExperienceLabel.Dispose();

                    ExperienceLabel = null;
                }

                if (NewMailIcon != null)
                {
                    if (!NewMailIcon.IsDisposed)
                        NewMailIcon.Dispose();

                    NewMailIcon = null;
                }

                if (ACLabel != null)
                {
                    if (!ACLabel.IsDisposed)
                        ACLabel.Dispose();

                    ACLabel = null;
                }

                if (MRLabel != null)
                {
                    if (!MRLabel.IsDisposed)
                        MRLabel.Dispose();

                    MRLabel = null;
                }

                if (DCLabel != null)
                {
                    if (!DCLabel.IsDisposed)
                        DCLabel.Dispose();

                    DCLabel = null;
                }

                if (MCLabel != null)
                {
                    if (!MCLabel.IsDisposed)
                        MCLabel.Dispose();

                    MCLabel = null;
                }

                if (SCLabel != null)
                {
                    if (!SCLabel.IsDisposed)
                        SCLabel.Dispose();

                    SCLabel = null;
                }

                if (AClabel != null)
                {
                    if (!AClabel.IsDisposed)
                        AClabel.Dispose();

                    AClabel = null;
                }

                if (MRlabel != null)
                {
                    if (!MRlabel.IsDisposed)
                        MRlabel.Dispose();

                    MRlabel = null;
                }

                if (DClabel != null)
                {
                    if (!DClabel.IsDisposed)
                        DClabel.Dispose();

                    DClabel = null;
                }

                if (MClabel != null)
                {
                    if (!MClabel.IsDisposed)
                        MClabel.Dispose();

                    MClabel = null;
                }

                if (SClabel != null)
                {
                    if (!SClabel.IsDisposed)
                        SClabel.Dispose();

                    SClabel = null;
                }

                if (AttackModeLabel != null)
                {
                    if (!AttackModeLabel.IsDisposed)
                        AttackModeLabel.Dispose();

                    AttackModeLabel = null;
                }

                if (PetModeLabel != null)
                {
                    if (!PetModeLabel.IsDisposed)
                        PetModeLabel.Dispose();

                    PetModeLabel = null;
                }
            }
        }
        #endregion
    }
}
