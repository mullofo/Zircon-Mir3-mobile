using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using System;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;

//Cleaned
namespace Client.Scenes.Views
{
    public sealed class MonsterDialog : DXWindow
    {
        #region Properties

        /// <summary>
        /// 怪物对象
        /// </summary>
        public MapObject Object
        {
            get => _Object;
            set
            {
                if (_Object == value) return;

                MapObject oldValue = _Object;
                _Object = value;

                OnObjectChanged(oldValue, value);
            }
        }
        private MapObject _Object;
        public event EventHandler<EventArgs> ObjectChanged;
        public void OnObjectChanged(MapObject oValue, MapObject nValue)  //怪物属性变化
        {
            if (GameScene.Game.MapControl.MapInfo.CanPlayName)
            {
                Visible = false;
                _Object = null;
                Monster.Monster = null;
                Player.Player = null;
                return;
            }

            Visible = Object != null;

            if (Object == null)
            {
                Monster.Monster = null;
                Player.Player = null;
                return;
            }

            if (nValue is PlayerObject)
            {
                if (oValue is MonsterObject)
                    Monster.Monster = null;

                Player.Player = nValue as PlayerObject;
                Size = new Size(310, 85);
            }
            else if (nValue is MonsterObject)
            {
                if (oValue is PlayerObject)
                    Player.Player = null;

                Monster.Monster = nValue as MonsterObject;
                Size = new Size(300, 94);
            }

            Location = new Point((GameScene.Game.Size.Width - Size.Width) / 2, 10);

            ObjectChanged?.Invoke(this, EventArgs.Empty);
        }

        private MonsterControl Monster;
        private PlayerControl Player;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 怪物属性显示面板
        /// </summary>
        public MonsterDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 0F;
            Border = false;
            Movable = false;

            Size = new Size(310, 85);

            Monster = new MonsterControl
            {
                Parent = this,
                DrawTexture = false,
                Visible = false,
            };

            Player = new PlayerControl
            {
                Parent = this,
                DrawTexture = false,
                Visible = false,
            };

        }

        #region Methods
        /// <summary>
        /// 刷新血量值
        /// </summary>
        public void RefreshHealth()
        {
            if (Object == null) return;

            DXLabel label = Object is PlayerObject ? Player.HealthLabel : Monster.HealthLabel;
            ClientObjectData data;
            label.Text = !GameScene.Game.DataDictionary.TryGetValue(Object.ObjectID, out data) ? string.Empty : $"{Math.Max(0, data.Health)} / {data.MaxHealth}";
        }
        /// <summary>
        /// 刷新属性状态
        /// </summary>
        public void RefreshStats()
        {
            if (Object == null) return;

            if (Object is MonsterObject)
                Monster.RefreshStats();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Object = null;
                ObjectChanged = null;
                if (Monster != null)
                {
                    if (!Monster.IsDisposed)
                        Monster.Dispose();

                    Monster = null;
                }

                if (Player != null)
                {
                    if (!Player.IsDisposed)
                        Player.Dispose();

                    Player = null;
                }


            }
        }
        #endregion
    }
    /// <summary>
    /// 怪物属性面板
    /// </summary>
    public sealed class MonsterControl : DXControl
    {
        #region Properties

        #region Monster
        /// <summary>
        /// 怪物对象
        /// </summary>
        public MonsterObject Monster
        {
            get => _Monster;
            set
            {
                if (_Monster == value) return;

                MonsterObject oldValue = _Monster;
                _Monster = value;

                OnMonsterChanged(oldValue, value);
            }
        }
        private MonsterObject _Monster;
        public event EventHandler<EventArgs> MonsterChanged;
        public void OnMonsterChanged(MonsterObject oValue, MonsterObject nValue)  //怪物属性变化
        {
            Visible = Monster != null && BigPatchConfig.ChkMonsterInfo;

            if (Monster == null) return;

            string _temname;
            _temname = System.Text.RegularExpressions.Regex.Replace(Monster.MonsterInfo.Lang(p => p.MonsterName), "[^A-Za-z\u4e00-\u9fa5]", "");
            NameLabel.Text = _temname;    //怪物名字
            //LevelLabel.Text = Monster.MonsterInfo.Level.ToString();

            RefreshStats();

            MonsterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        public DXControl PortraitPanel, HealthBar;

        public DXImageControl AttackIcon, Portrait; //增加怪物图像

        public DXLabel LevelLabel, NameLabel, HealthLabel;
#if Mobile
#else
        public DXLabel ACLabel, MRLabel, DCLabel;
#endif
        public DXLabel FireResistLabel, IceResistLabel, LightningResistLabel, WindResistLabel, HolyResistLabel, DarkResistLabel, PhantomResistLabel, PhysicalResistLabel;

        #endregion

        /// <summary>
        /// 怪物属性显示面板
        /// </summary>
        public MonsterControl()
        {
            Opacity = 0F;
            Border = false;
            Movable = false;

            Size = new Size(300, 94);

            PortraitPanel = new DXControl  //1.头像面板
            {
                //Size = new Size((int)(84 * 0.75F), (int)(94 * 0.75F)),
                Size = new Size(84, 94),
                Location = new Point(1, 0),
                Border = false,
                BackColour = Color.Black,
                Parent = this,
                Opacity = 1F,
                DrawTexture = true,
                IsControl = false
            };
            PortraitPanel.AfterDraw += (o, e) =>
            {
                if (Monster == null) return;
                MirLibrary lib;
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.MonImg, out lib)) return;
                MirImage image = lib.CreateImage(Monster.Portrait, ImageType.Image);
                if (image == null) return;
                //PresentMirImage(image.Image, this, new Rectangle(PortraitPanel.DisplayArea.X, PortraitPanel.DisplayArea.Y, image.Width, image.Height), Color.White, PortraitPanel, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
                //lib.Draw(Monster.Portrait, PortraitPanel.DisplayArea.X, PortraitPanel.DisplayArea.Y + 2, Color.White, false, 1F, ImageType.Image, 0.75F * ZoomRate, (int)(PortraitPanel.DisplayArea.X * ZoomRate - PortraitPanel.DisplayArea.X * 0.75F * ZoomRate));
                lib.Draw(Monster.Portrait, PortraitPanel.DisplayArea.X, PortraitPanel.DisplayArea.Y + 2, Color.White, false, 1F, ImageType.Image, ZoomRate, (int)(PortraitPanel.DisplayArea.X * ZoomRate - PortraitPanel.DisplayArea.X * ZoomRate));
            };

            /*panel = new DXControl  //1.等级面板
            {
                Size = new Size(31, 20),
                Location = new Point(2, 3),
                Border = false,
                Parent = this,
                Opacity = 0F,//0.6F,
                DrawTexture = true,
                IsControl = false
            };

            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(30,18),
                Location = new Point(0,0),
                Border = false,
                Parent = panel,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };*/

            //DXImageControl HealthBorder = new DXImageControl
            //{
            //    Size = new Size(84, 6),
            //    LibraryFile = LibraryFile.ProgUse,
            //    Index = 660,
            //    Location = new Point(0, 98),
            //    Parent = this,
            //    Opacity = 1F,
            //    DrawTexture = true,
            //    IsControl = false
            //};

            DXControl panel2 = new DXControl  //右边主界面
            {
                Size = new Size(216, 94),
                Border = true,
                BorderColour = Color.Gray,
                BackColour = Color.FromArgb(55, 55, 55),
                Parent = this,
                Opacity = 0.8F,
                DrawTexture = true,
                PassThrough = true,
            };
            panel2.Location = new Point(PortraitPanel.DisplayArea.Right, 0);
            panel2.AfterDraw += (o, e) =>
            {
                if (Monster == null) return;
                MirLibrary lib;
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib)) return;
                MirImage image;
                if (Monster.MonsterInfo.CanTame) image = lib.CreateImage(631, ImageType.Image);
                else image = lib.CreateImage(632, ImageType.Image);
                if (image == null) return;
                PresentMirImage(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                int speed = Monster.MonsterInfo.MoveDelay;
                if (speed == 0) image = lib.CreateImage(620, ImageType.Image);
                else if (speed >= 2300) image = lib.CreateImage(621, ImageType.Image);
                else if (speed >= 2100) image = lib.CreateImage(622, ImageType.Image);
                else if (speed >= 1800) image = lib.CreateImage(623, ImageType.Image);
                else if (speed >= 1400) image = lib.CreateImage(624, ImageType.Image);
                else if (speed >= 1000) image = lib.CreateImage(625, ImageType.Image);
                else if (speed >= 600) image = lib.CreateImage(626, ImageType.Image);
                else image = lib.CreateImage(627, ImageType.Image);
                if (image == null) return;
                PresentMirImage(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4 + 40, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                speed = Monster.MonsterInfo.AttackDelay;
                if (speed == 0) image = lib.CreateImage(590, ImageType.Image);
                else if (speed >= 4000) image = lib.CreateImage(591, ImageType.Image);
                else if (speed >= 3400) image = lib.CreateImage(592, ImageType.Image);
                else if (speed >= 2600) image = lib.CreateImage(593, ImageType.Image);
                else if (speed >= 1800) image = lib.CreateImage(594, ImageType.Image);
                else if (speed >= 1000) image = lib.CreateImage(595, ImageType.Image);
                else image = lib.CreateImage(596, ImageType.Image);
                if (image == null) return;
                PresentMirImage(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4 + 60, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);

                if (Monster.MonsterInfo.Undead) image = lib.CreateImage(635, ImageType.Image);
                else image = lib.CreateImage(634, ImageType.Image);
                if (image == null) return;
                PresentMirImage(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4 + 80, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2, zoomRate: ZoomRate, uiOffsetX: UI_Offset_X);
            };

            AttackIcon = new DXImageControl   //攻击属性图标
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Opacity = 0.7F,
                Location = new Point(24, 2),
                IsControl = false
            };

            NameLabel = new DXLabel    //怪物名字标签
            {
                AutoSize = false,
                Size = new Size(150, 18),
                Location = new Point(84, 2),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                Border = true,
                Parent = panel2,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

#if Mobile
#else
            DXLabel label = new DXLabel
            {
                Parent = panel2,
                Visible = false, //不显示
                IsControl = false,
                Text = "防御".Lang(),  //AC  防御
            };
            label.Location = new Point(100, 55);

            ACLabel = new DXLabel
            {
                Parent = panel2,
                Visible = false,  //不显示
                Location = new Point(55, 45),
                ForeColour = Color.White,
            };

            label = new DXLabel
            {
                Parent = panel2,
                Visible = false,
                IsControl = false,
                Text = "魔御".Lang(),
            };
            label.Location = new Point(125 - label.Size.Width, 5);

            MRLabel = new DXLabel
            {
                Parent = panel2,
                Visible = false,
                Location = new Point(125, 5),
                ForeColour = Color.White,
            };
            label = new DXLabel
            {
                Parent = panel2,
                Visible = false,
                IsControl = false,
                Text = "破坏".Lang()
            };
            label.Location = new Point(36 - label.Size.Width, 22);

            DCLabel = new DXLabel
            {
                Parent = panel2,
                Visible = false,
                Location = new Point(36, 22),
                ForeColour = Color.White,
            };
#endif

            #region 元素
            DXImageControl icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1520,
                Location = new Point(4, 23),//Point(5, 39)
                Hint = "火".Lang(),
            };

            FireResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 25),
                Tag = icon,
                Text = "+00",
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1521,
                Location = new Point(FireResistLabel.Location.X + FireResistLabel.Size.Width, icon.Location.Y),
                Hint = "冰".Lang(),
            };

            IceResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 25),
                Tag = icon,
                Text = "+00",
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1522,
                Location = new Point(IceResistLabel.Location.X + IceResistLabel.Size.Width, icon.Location.Y),
                Hint = "雷".Lang(),
            };

            LightningResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 25),
                Tag = icon,
                Text = "+00",
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1523,
                Location = new Point(LightningResistLabel.Location.X + LightningResistLabel.Size.Width, icon.Location.Y),
                Hint = "风".Lang(),
            };

            WindResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 25),
                Tag = icon,
                Text = "+00",
            };


            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1524,
                Location = new Point(4, 45),
                Hint = "神圣".Lang(),
            };

            HolyResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 47),
                Tag = icon,
                Text = "+00",
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1525,
                Location = new Point(HolyResistLabel.Location.X + HolyResistLabel.Size.Width, icon.Location.Y),
                Hint = "暗黑".Lang(),
            };

            DarkResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 47),
                Tag = icon,
                Text = "+00",
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1526,
                Location = new Point(DarkResistLabel.Location.X + DarkResistLabel.Size.Width, icon.Location.Y),
                Hint = "幻影".Lang(),
            };

            PhantomResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width, 47),
                Tag = icon,
                Text = "+00",
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                Visible = false,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                Location = new Point(PhantomResistLabel.Location.X + PhantomResistLabel.Size.Width, icon.Location.Y),
                Hint = "体质".Lang(),
            };

            PhysicalResistLabel = new DXLabel
            {
                Parent = panel2,
                Visible = false,
                Location = new Point(icon.Location.X + icon.Size.Width, 47),
                Tag = icon,
                Text = "+00",
            };
            #endregion

            HealthBar = new DXControl  //3. 血量面板
            {
                Location = new Point(2, icon.DisplayArea.Bottom + 7),
                Size = new Size(1, 1),
                Parent = panel2,
                BackColour = Color.FromArgb(255, 0, 9),
                Opacity = 1F,
                DrawTexture = true,
                IsControl = false
            };
            HealthBar.BeforeDraw += (o, e) =>   //画血条
            {
                if (Monster == null) return;
                ClientObjectData data;
                GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data);
                float percent = Monster.CompanionObject != null ? 1 : 0;
                if (data != null && data.MaxHealth > 0)
                    percent = Math.Min(1, Math.Max(0, data.Health / (float)data.MaxHealth));
                if (percent == 0)
                {
                    HealthBar.Size = new Size(1, 1);
                    return;
                }
                HealthBar.Size = new Size((int)Math.Round((panel2.Size.Width - 5) * percent), 12);
            };

            HealthLabel = new DXLabel   //血量标签
            {
                AutoSize = false,
                Size = new Size(panel2.Size.Width - 4, 18),  //18
                Location = new Point(2, icon.DisplayArea.Bottom + 2),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Border = false,
                Parent = panel2,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

        }

        #region Methods
        /// <summary>
        /// 填充标签
        /// </summary>
        /// <param name="stat"></param>
        /// <param name="label"></param>
        /// <param name="stats"></param>
        private void PopulateLabel(Stat stat, DXLabel label, Stats stats)
        {
            if (stats[stat] == 0) label.Text = "+0";
            else if (stats[stat] > 0) label.Text = $"+{(stats[stat]):0}";
            else label.Text = $"-{Math.Abs(stats[stat]):0}";
            label.ForeColour = Color.White;
        }
        /// <summary>
        /// 刷新属性状态
        /// </summary>
        public void RefreshStats()
        {
            ClientObjectData data;
            if (!GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data) || data.Stats == null)
            {
                HealthLabel.Text = string.Empty;  //怪物标签 生命值

                PopulateLabel(Stat.FireResistance, FireResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.IceResistance, IceResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.LightningResistance, LightningResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.WindResistance, WindResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.HolyResistance, HolyResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.DarkResistance, DarkResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.PhantomResistance, PhantomResistLabel, Monster.MonsterInfo.Stats);
                PopulateLabel(Stat.PhysicalResistance, PhysicalResistLabel, Monster.MonsterInfo.Stats);

                switch (Monster.Stats.GetAffinityElement())
                {
                    case Element.None:
                        AttackIcon.Index = 1517;
                        AttackIcon.Hint = "体质".Lang();
                        break;
                    case Element.Fire:
                        AttackIcon.Index = 1510;
                        AttackIcon.Hint = "火".Lang();
                        break;
                    case Element.Ice:
                        AttackIcon.Index = 1511;
                        AttackIcon.Hint = "冰".Lang();
                        break;
                    case Element.Lightning:
                        AttackIcon.Index = 1512;
                        AttackIcon.Hint = "雷".Lang();
                        break;
                    case Element.Wind:
                        AttackIcon.Index = 1513;
                        AttackIcon.Hint = "风".Lang();
                        break;
                    case Element.Holy:
                        AttackIcon.Index = 1514;
                        AttackIcon.Hint = "神圣".Lang();
                        break;
                    case Element.Dark:
                        AttackIcon.Index = 1515;
                        AttackIcon.Hint = "暗黑".Lang();
                        break;
                    case Element.Phantom:
                        AttackIcon.Index = 1516;
                        AttackIcon.Hint = "幻影".Lang();
                        break;
                }
            }
            else
            {
                HealthLabel.Text = $"{Math.Max(0, data.Health)} / {data.MaxHealth}";  //怪物标签 生命值

                PopulateLabel(Stat.FireResistance, FireResistLabel, data.Stats);
                PopulateLabel(Stat.IceResistance, IceResistLabel, data.Stats);
                PopulateLabel(Stat.LightningResistance, LightningResistLabel, data.Stats);
                PopulateLabel(Stat.WindResistance, WindResistLabel, data.Stats);
                PopulateLabel(Stat.HolyResistance, HolyResistLabel, data.Stats);
                PopulateLabel(Stat.DarkResistance, DarkResistLabel, data.Stats);
                PopulateLabel(Stat.PhantomResistance, PhantomResistLabel, data.Stats);
                PopulateLabel(Stat.PhysicalResistance, PhysicalResistLabel, data.Stats);

                switch (data.Stats.GetAffinityElement())
                {
                    case Element.None:
                        AttackIcon.Index = 1517;
                        AttackIcon.Hint = "体质".Lang();
                        break;
                    case Element.Fire:
                        AttackIcon.Index = 1510;
                        AttackIcon.Hint = "火".Lang();
                        break;
                    case Element.Ice:
                        AttackIcon.Index = 1511;
                        AttackIcon.Hint = "冰".Lang();
                        break;
                    case Element.Lightning:
                        AttackIcon.Index = 1512;
                        AttackIcon.Hint = "雷".Lang();
                        break;
                    case Element.Wind:
                        AttackIcon.Index = 1513;
                        AttackIcon.Hint = "风".Lang();
                        break;
                    case Element.Holy:
                        AttackIcon.Index = 1514;
                        AttackIcon.Hint = "神圣".Lang();
                        break;
                    case Element.Dark:
                        AttackIcon.Index = 1515;
                        AttackIcon.Hint = "暗黑".Lang();
                        break;
                    case Element.Phantom:
                        AttackIcon.Index = 1516;
                        AttackIcon.Hint = "幻影".Lang();
                        break;
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
                _Monster = null;
                MonsterChanged = null;
                if (AttackIcon != null)
                {
                    if (!AttackIcon.IsDisposed)
                        AttackIcon.Dispose();

                    AttackIcon = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

#if Mobile
#else
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
#endif

                if (FireResistLabel != null)
                {
                    if (!FireResistLabel.IsDisposed)
                        FireResistLabel.Dispose();

                    FireResistLabel = null;
                }

                if (IceResistLabel != null)
                {
                    if (!IceResistLabel.IsDisposed)
                        IceResistLabel.Dispose();

                    IceResistLabel = null;
                }

                if (LightningResistLabel != null)
                {
                    if (!LightningResistLabel.IsDisposed)
                        LightningResistLabel.Dispose();

                    LightningResistLabel = null;
                }

                if (WindResistLabel != null)
                {
                    if (!WindResistLabel.IsDisposed)
                        WindResistLabel.Dispose();

                    WindResistLabel = null;
                }

                if (HolyResistLabel != null)
                {
                    if (!HolyResistLabel.IsDisposed)
                        HolyResistLabel.Dispose();

                    HolyResistLabel = null;
                }

                if (DarkResistLabel != null)
                {
                    if (!DarkResistLabel.IsDisposed)
                        DarkResistLabel.Dispose();

                    DarkResistLabel = null;
                }

                if (PhantomResistLabel != null)
                {
                    if (!PhantomResistLabel.IsDisposed)
                        PhantomResistLabel.Dispose();

                    PhantomResistLabel = null;
                }

                if (Portrait != null)
                {
                    if (!Portrait.IsDisposed)
                        Portrait.Dispose();

                    Portrait = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 人物属性面板
    /// </summary>
    public sealed class PlayerControl : DXControl
    {
        #region Properties

        #region Player
        /// <summary>
        /// 怪物对象
        /// </summary>
        public PlayerObject Player
        {
            get => _Player;
            set
            {
                if (_Player == value) return;

                PlayerObject oldValue = _Player;
                _Player = value;

                OnPlayerChanged(oldValue, value);
            }
        }
        private PlayerObject _Player;
        public event EventHandler<EventArgs> PlayerChanged;
        public void OnPlayerChanged(PlayerObject oValue, PlayerObject nValue)  //怪物属性变化
        {
            Visible = Player != null;

            if (Player == null) return;

            //string _temname;
            NameLabel.Text = Player.Name;    //怪物名字
            //LevelLabel.Text = Monster.MonsterInfo.Level.ToString();
            HealthLabel.Text = !GameScene.Game.DataDictionary.TryGetValue(Player.ObjectID, out var data) ? string.Empty : $"{Math.Max(0, data.Health)} / {data.MaxHealth}";

            PlayerChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        public DXControl RightPanel, HealthBar;
        private DXImageControl PhotoImage;
        private DXButton GroupButton, PrivateButton, FriendsButton, TradeButton;
        public DXLabel LevelLabel, NameLabel, HealthLabel;

        #endregion

        /// <summary>
        /// 怪物属性显示面板
        /// </summary>
        public PlayerControl()
        {
            Opacity = 0F;
            Border = false;
            Movable = false;

            Size = new Size(310, 85);



            /*
            LevelLabel = new DXLabel
            {
                AutoSize = false,
                Size = new Size(30,18),
                Location = new Point(0,0),
                Border = false,
                Parent = panel,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };*/

            RightPanel = new DXControl  //右边主界面
            {
                Size = new Size(265, 64),
                Border = true,
                BorderColour = Color.Gray,
                BackColour = Color.FromArgb(55, 55, 55),
                Parent = this,
                Opacity = 0.8F,
                DrawTexture = true,
                PassThrough = true,
            };
            RightPanel.Location = new Point(85 / 2, (Size.Height - RightPanel.Size.Height) / 2);

            NameLabel = new DXLabel    //怪物名字标签
            {
                AutoSize = false,
                Size = new Size(RightPanel.Size.Width - 85 / 2, 18),
                Location = new Point(85 / 2, 0),
                Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                Border = true,
                Parent = RightPanel,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            HealthBar = new DXControl  //3. 血量面板
            {
                Location = new Point(NameLabel.Location.X, NameLabel.Location.Y + NameLabel.Size.Height + 2),
                Size = new Size(1, 1),
                Parent = RightPanel,
                BackColour = Color.FromArgb(255, 0, 9),
                Opacity = 1F,
                DrawTexture = true,
                IsControl = false
            };
            HealthBar.BeforeDraw += (o, e) =>   //画血条
            {
                if (Player == null) return;
                ClientObjectData data;
                GameScene.Game.DataDictionary.TryGetValue(Player.ObjectID, out data);
                float percent = 0;
                if (data != null && data.MaxHealth > 0)
                    percent = Math.Min(1, Math.Max(0, data.Health / (float)data.MaxHealth));
                if (percent == 0)
                {
                    HealthBar.Size = new Size(1, 1);
                    return;
                }
                HealthBar.Size = new Size((int)Math.Round((RightPanel.Size.Width - (85 / 2) - 5) * percent), 12);
            };

            HealthLabel = new DXLabel   //血量标签
            {
                AutoSize = false,
                Size = new Size(NameLabel.Size.Width, 18),  //18
                Location = new Point(HealthBar.Location.X, HealthBar.Location.Y - 4),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Border = false,
                Parent = RightPanel,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            GroupButton = new DXButton
            {
                Parent = RightPanel,
                Size = new Size(50, 25),
                ButtonType = ButtonType.Default,
                Label = { Text = "组队" },
                Location = new Point(HealthLabel.Location.X, HealthLabel.Location.Y + HealthLabel.Size.Height + 3),
            };
            GroupButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (Player != null)
                {
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

                    CEnvir.Enqueue(new C.GroupInvite { Name = Player.Name });
                }
            };

            PrivateButton = new DXButton
            {
                Parent = RightPanel,
                Size = new Size(50, 25),
                ButtonType = ButtonType.Default,
                Label = { Text = "私聊" },
                Location = new Point(GroupButton.Location.X + GroupButton.Size.Width + 5, GroupButton.Location.Y),
            };
            PrivateButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (!GameScene.Game.ChatTextBox.Visible)
                {
                    GameScene.Game.ChatTextBox.Visible = true;
                    GameScene.Game.ChatTextBox.StartPM(Player.Name);
                    FocusControl = GameScene.Game.ChatTextBox.TextBox;
                }
                else
                {
                    GameScene.Game.ChatTextBox.SendMessage();
                    GameScene.Game.ChatTextBox.Visible = false;
                }
            };

            FriendsButton = new DXButton  //好友按钮
            {
                Parent = RightPanel,
                Size = new Size(50, 25),
                ButtonType = ButtonType.Default,
                Label = { Text = "好友" },
                Location = new Point(PrivateButton.Location.X + PrivateButton.Size.Width + 5, PrivateButton.Location.Y),
            };
            FriendsButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                CEnvir.Enqueue(new C.FriendRequest { Name = Player.Name });
            };

            TradeButton = new DXButton   //交易按钮
            {
                Parent = RightPanel,
                Size = new Size(50, 25),
                ButtonType = ButtonType.Default,
                Label = { Text = "交易" },
                Location = new Point(FriendsButton.Location.X + FriendsButton.Size.Width + 5, FriendsButton.Location.Y),
            };
            TradeButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                //距一步
                if (Functions.Distance(MapObject.User.CurrentLocation, Player.CurrentLocation) >= 8)
                {
                    DXConfirmWindow.Show($"MainPanel.TradeShow".Lang("不在交易范围内！！！"));
                    return;
                }

                CEnvir.Enqueue(new C.TradeRequest { Index = Player.CharacterIndex });

            };

            PhotoImage = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 40,
                ImageOpacity = 1.0F,
                Visible = true,
                Location = new Point(0, 0),
            };
            PhotoImage.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (Player == null || Player == MapObject.User) return;
                if (CEnvir.Now <= GameScene.Game.InspectTime && Player.ObjectID == GameScene.Game.InspectID) return;

                GameScene.Game.InspectTime = CEnvir.Now.AddMilliseconds(2500);
                GameScene.Game.InspectID = Player.ObjectID;
                CEnvir.Enqueue(new C.Inspect { Index = Player.CharacterIndex });
            };
            PhotoImage.BeforeDraw += (o, e) =>
            {
                if (Player == null) return;

                PhotoImage.Index = (Player.Class == MirClass.Warrior && Player.Gender == MirGender.Male) ? 40 :
                        ((Player.Class == MirClass.Warrior && Player.Gender == MirGender.Female) ? 41 :
                        ((Player.Class == MirClass.Wizard && Player.Gender == MirGender.Male) ? 42 :
                        ((Player.Class == MirClass.Wizard && Player.Gender == MirGender.Female) ? 43 :
                        ((Player.Class == MirClass.Taoist && Player.Gender == MirGender.Male) ? 44 :
                        ((Player.Class == MirClass.Taoist && Player.Gender == MirGender.Female) ? 45 :
                        ((Player.Class == MirClass.Assassin && Player.Gender == MirGender.Male) ? 46 : 47))))));
            };

        }

        #region Methods


        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Player = null;
                PlayerChanged = null;
                if (RightPanel != null)
                {
                    if (!RightPanel.IsDisposed)
                        RightPanel.Dispose();

                    RightPanel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (HealthLabel != null)
                {
                    if (!HealthLabel.IsDisposed)
                        HealthLabel.Dispose();

                    HealthLabel = null;
                }

                if (HealthBar != null)
                {
                    if (!HealthBar.IsDisposed)
                        HealthBar.Dispose();

                    HealthBar = null;
                }

                if (PrivateButton != null)
                {
                    if (!PrivateButton.IsDisposed)
                        PrivateButton.Dispose();

                    PrivateButton = null;
                }

                if (GroupButton != null)
                {
                    if (!GroupButton.IsDisposed)
                        GroupButton.Dispose();

                    GroupButton = null;
                }

                if (PhotoImage != null)
                {
                    if (!PhotoImage.IsDisposed)
                        PhotoImage.Dispose();

                    PhotoImage = null;
                }


            }
        }
        #endregion
    }
}
