using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 怪物属性面板
    /// </summary>
    public sealed class MonsterDialog : DXWindow
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

            Location = new Point(GameScene.Game.ChatBox.Photo.DisplayArea.Location.X, GameScene.Game.ChatBox.Photo.DisplayArea.Location.Y - 5);

            MonsterChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
        public DXControl PortraitPanel, panel;

        public DXImageControl AttackIcon, Portrait; //增加怪物图像

        public DXLabel LevelLabel, NameLabel, HealthLabel, ACLabel, MRLabel, DCLabel;
        public DXLabel FireResistLabel, IceResistLabel, LightningResistLabel, WindResistLabel, HolyResistLabel, DarkResistLabel, PhantomResistLabel, PhysicalResistLabel;

        public override WindowType Type => WindowType.MonsterBox;
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

            Size = new Size(240, 120);

            PortraitPanel = new DXControl  //1.头像面板
            {
                Size = new Size(84, 94),
                Location = new Point(0, 4),
                Border = false,
                BorderColour = Color.FromArgb(198, 166, 99),
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
                PresentTexture(image.Image, this, new Rectangle(PortraitPanel.DisplayArea.X, PortraitPanel.DisplayArea.Y, image.Width, image.Height), Color.White, PortraitPanel);

                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib)) return;
                if (Monster.MonsterInfo.Undead) image = lib.CreateImage(635, ImageType.Image);
                else image = lib.CreateImage(634, ImageType.Image);
                if (image == null) return;
                PresentTexture(image.Image, this, new Rectangle(PortraitPanel.DisplayArea.X + 64, PortraitPanel.DisplayArea.Y + 75, image.Width, image.Height), Color.White, PortraitPanel);
            };

            panel = new DXControl  //1.等级面板
            {
                Size = new Size(31, 20),
                Location = new Point(2, 3),
                Border = false,
                Parent = this,
                Opacity = 0F,//0.6F,
                DrawTexture = true,
                IsControl = false
            };

            /*LevelLabel = new DXLabel
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

            panel = new DXControl //2. 名称面板
            {
                Size = new Size(96, 18),  //Size(140, 20),
                Location = new Point(41, 5),
                Border = false,
                BorderColour = Color.FromArgb(198, 166, 99),
                BackColour = Color.Black,
                Parent = this,
                Opacity = 0.6F,
                Visible = false,  //不可见
                DrawTexture = true,
                IsControl = false
            };

            NameLabel = new DXLabel    //怪物名字标签
            {
                AutoSize = false,
                Size = new Size(95, 18),  //Size(139, 18)
                Location = new Point(0, 0),
                Font = new Font(Config.FontName, CEnvir.FontSize(7F)),
                Border = true,
                Parent = panel,
                Visible = false,  //不可见
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            DXImageControl HealthBorder = new DXImageControl
            {
                Size = new Size(84, 6),
                LibraryFile = LibraryFile.ProgUse,
                Index = 660,
                Location = new Point(0, 98),
                Parent = this,
                Opacity = 1F,
                DrawTexture = true,
                IsControl = false
            };

            HealthLabel = new DXLabel   //血量标签
            {
                AutoSize = false,
                Size = new Size(78, 18),  //18
                Location = new Point(0, 84),
                Font = new Font(Config.FontName, CEnvir.FontSize(8F)),
                Border = false,
                Parent = this, //
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false
            };

            panel = new DXControl  //3. 血量面板
            {
                Size = new Size(84, 4),//Size(121, 16),
                Location = new Point(0, 97),
                Border = false,
                Parent = this,
                Opacity = 1F,
                DrawTexture = true,
                IsControl = false
            };
            panel.AfterDraw += (o, e) =>   //画血条
            {
                if (Monster == null) return;
                MirLibrary lib;
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib)) return;
                ClientObjectData data;
                GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data);
                float percent = Monster.CompanionObject != null ? 1 : 0;
                if (data != null && data.MaxHealth > 0)
                    percent = Math.Min(1, Math.Max(0, data.Health / (float)data.MaxHealth));
                if (percent == 0) return;
                MirImage image = lib.CreateImage(661, ImageType.Image);
                if (image == null) return;
                PresentTexture(image.Image, this, new Rectangle(panel.DisplayArea.X, panel.DisplayArea.Y + 2, (int)(image.Width * percent), image.Height), Color.White, panel);
            };

            DXControl panel2 = new DXControl  //右边主界面
            {
                Size = new Size(104, 84),
                Location = new Point(107, 0),
                Border = true,
                BorderColour = Color.Gray,
                BackColour = Color.FromArgb(55, 55, 55),
                Parent = this,
                Opacity = 0.8F,
                DrawTexture = true,
                PassThrough = true,
            };
            panel2.AfterDraw += (o, e) =>
            {
                if (Monster == null) return;
                MirLibrary lib;
                if (!CEnvir.LibraryList.TryGetValue(LibraryFile.ProgUse, out lib)) return;
                MirImage image;
                if (Monster.MonsterInfo.CanTame) image = lib.CreateImage(631, ImageType.Image);
                else image = lib.CreateImage(632, ImageType.Image);
                if (image == null) return;
                PresentTexture(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2);

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
                PresentTexture(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4 + 40, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2);

                speed = Monster.MonsterInfo.AttackDelay;
                if (speed == 0) image = lib.CreateImage(590, ImageType.Image);
                else if (speed >= 4000) image = lib.CreateImage(591, ImageType.Image);
                else if (speed >= 3400) image = lib.CreateImage(592, ImageType.Image);
                else if (speed >= 2600) image = lib.CreateImage(593, ImageType.Image);
                else if (speed >= 1800) image = lib.CreateImage(594, ImageType.Image);
                else if (speed >= 1000) image = lib.CreateImage(595, ImageType.Image);
                else image = lib.CreateImage(596, ImageType.Image);
                if (image == null) return;
                PresentTexture(image.Image, this, new Rectangle(panel2.DisplayArea.X + 4 + 60, panel2.DisplayArea.Y + 2, image.Width, image.Height), Color.White, panel2);
            };

            AttackIcon = new DXImageControl   //攻击属性图标
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Opacity = 0.7F,
                Location = new Point(24, 2),
                IsControl = false
            };

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
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1521,
                Location = new Point(icon.Location.X + 32, 23),
                Hint = "冰".Lang(),
            };

            IceResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1522,
                Location = new Point(icon.Location.X + 32, 23),
                Hint = "雷".Lang(),
            };

            LightningResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1523,
                Location = new Point(4, 23 + 20),
                Hint = "风".Lang(),
            };

            WindResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25 + 20),
                Tag = icon,
            };


            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1524,
                Location = new Point(icon.Location.X + 32, 23 + 20),
                Hint = "神圣".Lang(),
            };

            HolyResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25 + 20),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1525,
                Location = new Point(icon.Location.X + 32, 23 + 20),
                Hint = "暗黑".Lang(),
            };

            DarkResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25 + 20),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                LibraryFile = LibraryFile.GameInter,
                Index = 1526,
                Location = new Point(4, 23 + 40),
                Hint = "幻影".Lang(),
            };

            PhantomResistLabel = new DXLabel
            {
                Parent = panel2,
                Location = new Point(icon.Location.X + icon.Size.Width - 5, 25 + 40),
                Tag = icon,
            };

            icon = new DXImageControl
            {
                Parent = panel2,
                Visible = false,
                LibraryFile = LibraryFile.GameInter,
                Index = 1517,
                Location = new Point(icon.Location.X + 32, 23),
                Hint = "体质".Lang(),
            };

            PhysicalResistLabel = new DXLabel
            {
                Parent = panel2,
                Visible = false,
                Location = new Point(icon.Location.X + icon.Size.Width, 25),
                Tag = icon,
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
        /// 刷新血量值
        /// </summary>
        public void RefreshHealth()
        {
            ClientObjectData data;
            HealthLabel.Text = !GameScene.Game.DataDictionary.TryGetValue(Monster.ObjectID, out data) ? string.Empty : $"{Math.Max(0, data.Health)} / {data.MaxHealth}";
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
}
