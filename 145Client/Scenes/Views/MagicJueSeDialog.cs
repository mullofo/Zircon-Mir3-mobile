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


namespace Client.Scenes.Views
{
    /// <summary>
    /// 魔法技能窗口
    /// </summary>
    public sealed class MagicJueSeDialog : DXWindow
    {
        #region Properties        

        private DXImageControl CureentSchool;
        public DXImageControl MagicGround;
        public DXButton Close1Button;
        public SortedDictionary<MagicSchool, MagicJueSeTab> SchoolTabs = new SortedDictionary<MagicSchool, MagicJueSeTab>();   //整理排序 按数据库魔法技能树

        public Dictionary<MagicInfo, MagicJueSeCell> Magics = new Dictionary<MagicInfo, MagicJueSeCell>(); //排序 按魔法信息
        private DXVScrollBar ScrollBar;
        /// <summary>
        /// 技能页面
        /// </summary>
        public DXImageControl FirePage, IcePage, LightningPage, WindPage, HolyPage, DarkPage, PhantomPage, WeaponSkillsPage;

        /// <summary>
        /// 技能树切换
        /// </summary>
        public DXButton FireButton, IceButton, LightningButton, WindButton, HolyButton, DarkButton, PhantomButton, WeaponSkillsButton;

        public DXLabel MagicNameLabel, MagicLevelLabel, MagicExperienceLabel;

        public override WindowType Type => WindowType.MagicBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion                   

        /// <summary>
        /// 魔法技能面板
        /// </summary>
        public MagicJueSeDialog()
        {
            HasTitle = false;  //不显示标题
            HasFooter = false;  //不显示页脚
            HasTopBorder = false;  //不显示边框
            TitleLabel.Visible = false;  //标题标签不用
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size s;
            s = UI1Library.GetSize(1620);
            Size = s;
            Location = ClientArea.Location;

            MagicGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1620,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            Close1Button = new DXButton       //关闭按钮
            {
                Parent = this,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(317, 400),
                Hint = "关闭",
            };
            Close1Button.MouseClick += (o, e) => Visible = false;

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
                Location = new Point(Size.Width - 42, 58),
                Change = 50,
                Size = new Size(20, 285),
                VisibleSize = 335
            };
            ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();

            MagicNameLabel = new DXLabel
            {
                Location = new Point(35, 393),
                Text = "魔法名称",
                Parent = this,
                Visible = false,
                ForeColour = Color.White
            };

            MagicLevelLabel = new DXLabel
            {
                Location = new Point(25, 408),
                Text = "Level : ",
                Parent = this,
                Visible = false,
                ForeColour = Color.White
            };

            MagicExperienceLabel = new DXLabel
            {
                Location = new Point(100, 408),
                Text = "Exp : ",
                Parent = this,
                Visible = false,
                ForeColour = Color.White
            };

            FireButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1640,
                Parent = this,
                Location = new Point(45, 10),
                Hint = "火",
                Tag = MagicSchool.Fire
            };
            FireButton.MouseClick += School_Click;

            IceButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1642,
                Parent = this,
                Location = new Point(79, 10),
                Hint = "冰",
                Tag = MagicSchool.Ice
            };
            IceButton.MouseClick += School_Click;

            LightningButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1644,
                Parent = this,
                Location = new Point(113, 10),
                Hint = "雷",
                Tag = MagicSchool.Lightning
            };
            LightningButton.MouseClick += School_Click;

            WindButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1646,
                Parent = this,
                Location = new Point(147, 10),
                Hint = "风",
                Tag = MagicSchool.Wind
            };
            WindButton.MouseClick += School_Click;

            HolyButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1648,
                Parent = this,
                Location = new Point(181, 10),
                Hint = "神圣",
                Tag = MagicSchool.Holy
            };
            HolyButton.MouseClick += School_Click;

            DarkButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1650,
                Parent = this,
                Location = new Point(215, 10),
                Hint = "暗黑",
                Tag = MagicSchool.Dark
            };
            DarkButton.MouseClick += School_Click;

            PhantomButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1652,
                Parent = this,
                Location = new Point(249, 10),
                Hint = "幻影",
                Tag = MagicSchool.Phantom
            };
            PhantomButton.MouseClick += School_Click;

            WeaponSkillsButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1654,
                Parent = this,
                Location = new Point(283, 10),
                Hint = "?",
                Tag = MagicSchool.WeaponSkills
            };
            WeaponSkillsButton.MouseClick += School_Click;

            var treepage = new DXColourControl
            {
                Location = new Point(18, 44),
                Size = new Size(304, 330),
                Parent = this,
            };
            FirePage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1622,
                Parent = treepage,
                Visible = true,
            };
            CureentSchool = FirePage;
            AddMagics(MagicSchool.Fire, FirePage);
            FirePage.MouseWheel += Page_MouseWheel;

            IcePage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1624,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.Ice, IcePage);
            IcePage.MouseWheel += Page_MouseWheel;

            LightningPage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1626,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.Lightning, LightningPage);
            LightningPage.MouseWheel += Page_MouseWheel;

            WindPage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1628,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.Wind, WindPage);
            WindPage.MouseWheel += Page_MouseWheel;

            HolyPage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1630,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.Holy, HolyPage);
            HolyPage.MouseWheel += Page_MouseWheel;

            DarkPage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1632,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.Dark, DarkPage);
            DarkPage.MouseWheel += Page_MouseWheel;

            PhantomPage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1634,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.Phantom, PhantomPage);
            PhantomPage.MouseWheel += Page_MouseWheel;

            WeaponSkillsPage = new DXImageControl
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1636,
                Parent = treepage,
                Visible = false,
            };
            AddMagics(MagicSchool.WeaponSkills, WeaponSkillsPage);
            WeaponSkillsPage.MouseWheel += Page_MouseWheel;

            //TabControl = new DXTabControl   //选项卡控件      
            //{
            //    Parent = this,
            //    Size = new Size(325, 369),           
            //    Location = new Point(18, 6),               
            //};          
        }


        #region Methods

        private void UpdateLocations()
        {
            int y = -ScrollBar.Value;
            CureentSchool.Location = new Point(0, y);
        }
        private void School_Click(object sender, MouseEventArgs e)
        {
            var button = sender as DXButton;
            if (sender == null) return;
            Enum.TryParse(button.Tag.ToString(), out MagicSchool school);

            FireButton.Index = school == MagicSchool.Fire ? 1641 : 1640;
            IceButton.Index = school == MagicSchool.Ice ? 1643 : 1642;
            LightningButton.Index = school == MagicSchool.Lightning ? 1645 : 1644;
            WindButton.Index = school == MagicSchool.Wind ? 1647 : 1646;
            HolyButton.Index = school == MagicSchool.Holy ? 1649 : 1648;
            DarkButton.Index = school == MagicSchool.Dark ? 1651 : 1650;
            PhantomButton.Index = school == MagicSchool.Phantom ? 1653 : 1652;
            WeaponSkillsButton.Index = school == MagicSchool.WeaponSkills ? 1655 : 1654;
            FirePage.Visible = school == MagicSchool.Fire;
            if (FirePage.Visible)
            {
                CureentSchool = FirePage;
                ScrollBar.MaxValue = FirePage.Size.Height;
            }
            IcePage.Visible = school == MagicSchool.Ice;
            if (IcePage.Visible)
            {
                CureentSchool = IcePage;
            }
            LightningPage.Visible = school == MagicSchool.Lightning;
            if (LightningPage.Visible)
            {
                CureentSchool = LightningPage;
            }
            WindPage.Visible = school == MagicSchool.Wind;
            if (WindPage.Visible)
            {
                CureentSchool = WindPage;
            }
            HolyPage.Visible = school == MagicSchool.Holy;
            if (HolyPage.Visible)
            {
                CureentSchool = HolyPage;
            }
            DarkPage.Visible = school == MagicSchool.Dark;
            if (DarkPage.Visible)
            {
                CureentSchool = DarkPage;
            }
            PhantomPage.Visible = school == MagicSchool.Phantom;
            if (PhantomPage.Visible)
            {
                CureentSchool = PhantomPage;
            }
            WeaponSkillsPage.Visible = school == MagicSchool.WeaponSkills;
            if (WeaponSkillsPage.Visible)
            {
                CureentSchool = WeaponSkillsPage;
            }
            ScrollBar.MaxValue = CureentSchool.Size.Height;

            ScrollBar.Value = 0;
        }

        private void AddMagics(MagicSchool school, DXControl control)
        {
            List<MagicInfo> magics = school == MagicSchool.WeaponSkills ?
                Globals.MagicInfoList.Binding.Where(p => p.School == MagicSchool.Assassinatie
                    || p.School == MagicSchool.Assassination
                    || p.School == MagicSchool.Combat
                    || p.School == MagicSchool.WeaponSkills
                    || p.School == MagicSchool.Neutral
                    || p.School == MagicSchool.Passive
                    || p.School == MagicSchool.Unconditional).ToList() :
                    Globals.MagicInfoList.Binding.Where(p => p.School == school).ToList();
            foreach (var magic in magics)
            {
                MagicJueSeCell cell = new MagicJueSeCell
                {
                    Parent = control,
                    Info = magic,
                    Location = new Point(magic.X, magic.Y)
                };
                Magics.Add(magic, cell);
                cell.MouseWheel += ScrollBar.DoMouseWheel;
            }
        }

        public void RefreshAll()
        {
            if (MapObject.User == null) return;
            foreach (var item in MapObject.User.Magics)
            {
                var magic = item.Value?.Info;
                // Magics[magic]?.Refresh();
            }
        }

        public void Page_MouseWheel(object sender, MouseEventArgs e)
        {
            ScrollBar.DoMouseWheel(sender, e);
        }
        #endregion

        public void NewInformation(global::Library.Network.ServerPackets.InspectMagery p)
        {

            base.Visible = true;
            if (SchoolTabs != null)
            {
                foreach (KeyValuePair<MagicSchool, MagicJueSeTab> schoolTab in SchoolTabs)
                {
                    if (schoolTab.Value != null && !schoolTab.Value.IsDisposed)
                    {
                        schoolTab.Value.Dispose();
                    }
                }
                SchoolTabs.Clear();
                SchoolTabs = null;
            }
            if (Magics == null)
            {
                return;
            }
            foreach (KeyValuePair<MagicInfo, MagicJueSeCell> magic in Magics)
            {
                if (magic.Value != null && !magic.Value.IsDisposed)
                {
                    magic.Value.Dispose();
                }
            }
            Magics.Clear();


            foreach (ClientUserMagic userMagic in p.Items)
            {
                if (userMagic.Info.School == MagicSchool.Fire)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = FirePage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    // magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.Ice)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = IcePage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.Lightning)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = LightningPage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.Wind)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = WindPage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.Holy)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = HolyPage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.Dark)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = DarkPage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.Phantom)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = PhantomPage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }
                else if (userMagic.Info.School == MagicSchool.WeaponSkills)
                {
                    MagicJueSeCell magicCell1 = new MagicJueSeCell();
                    magicCell1.Parent = WeaponSkillsPage;
                    magicCell1.Info = userMagic.Info;
                    magicCell1.MagicLevel.Text = userMagic.Level.ToString();
                    //magicCell1.Experience = userMagic.Experience;
                    magicCell1.Location = new Point(userMagic.Info.X, userMagic.Info.Y);
                    MagicJueSeCell magicCell2 = magicCell1;
                    this.Magics.Add(userMagic.Info, magicCell2);
                    //magicCell2.MouseWheel += new EventHandler<MouseEventArgs>(dxvscrollBar_0.DoMouseWheel);
                }

            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

                if (SchoolTabs != null)
                {
                    foreach (KeyValuePair<MagicSchool, MagicJueSeTab> pair in SchoolTabs)
                    {
                        if (pair.Value == null) continue;
                        if (pair.Value.IsDisposed) continue;

                        pair.Value.Dispose();
                    }
                    SchoolTabs.Clear();
                    SchoolTabs = null;
                }

                if (Magics != null)
                {
                    foreach (KeyValuePair<MagicInfo, MagicJueSeCell> pair in Magics)
                    {
                        if (pair.Value == null) continue;
                        if (pair.Value.IsDisposed) continue;

                        pair.Value.Dispose();
                    }
                    Magics.Clear();
                    Magics = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 魔法技能选项
    /// </summary>
    public sealed class MagicJueSeTab : DXTab
    {
        #region Properties           
        public DXVScrollBar ScrollBar;            //技能标签滚动条

        public override void OnSizeChanged(Size oValue, Size nValue)
        {
            base.OnSizeChanged(oValue, nValue);

            ScrollBar.Size = new Size(15, Size.Height - 12);
            ScrollBar.Location = new Point(Size.Width - 22, 2);
            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1672);

            int height = 2;

            foreach (DXControl control in Controls)
            {
                if (!(control is MagicJueSeCell)) continue;

                height += control.Size.Height + 4;
            }

            ScrollBar.MaxValue = height;         //滚动条最大值=高度；

            ScrollBar.VisibleSize = Size.Height;  //滚动条可见尺寸=尺寸的高度
            UpdateLocations();                   //更新位置
        }

        #endregion

        /// <summary>
        /// 魔法技能选项
        /// </summary>
        /// <param name="school"></param>
        public MagicJueSeTab(MagicSchool school)
        {
            TabButton.LibraryFile = LibraryFile.UI1;//技能树图标定义
            TabButton.Hint = school.ToString();
            //Border = true;      //不绘制边框
            BackColour = Color.Empty;  //背景图置空
            Location = new Point(-5, 39);

            switch (school)
            {
                case MagicSchool.Passive:
                    TabButton.Index = 1601;
                    TabButton.Hint = "被动".Lang();
                    break;
                case MagicSchool.WeaponSkills:
                    TabButton.Index = 1600;
                    TabButton.Hint = "武技".Lang();
                    break;
                case MagicSchool.Neutral:
                    TabButton.Index = 1602;
                    TabButton.Hint = "转换".Lang();
                    break;
                case MagicSchool.Fire:
                    TabButton.Index = 1603;
                    TabButton.Hint = "火".Lang();
                    break;
                case MagicSchool.Ice:
                    TabButton.Index = 1604;
                    TabButton.Hint = "冰".Lang();
                    break;
                case MagicSchool.Lightning:
                    TabButton.Index = 1605;
                    TabButton.Hint = "雷".Lang();
                    break;
                case MagicSchool.Wind:
                    TabButton.Index = 1606;
                    TabButton.Hint = "风".Lang();
                    break;
                case MagicSchool.Holy:
                    TabButton.Index = 1607;
                    TabButton.Hint = "神圣".Lang();
                    break;
                case MagicSchool.Dark:
                    TabButton.Index = 1608;
                    TabButton.Hint = "暗黑".Lang();
                    break;
                case MagicSchool.Phantom:
                    TabButton.Index = 1609;
                    TabButton.Hint = "幻影".Lang();
                    break;
                case MagicSchool.Unconditional:
                    TabButton.Index = 1610;
                    TabButton.Hint = "无条件".Lang();
                    break;
                case MagicSchool.Combat:
                    TabButton.Index = 1611;
                    TabButton.Hint = "格斗".Lang();
                    break;
                case MagicSchool.Assassination:
                    TabButton.Index = 1612;
                    TabButton.Hint = "刺杀".Lang();
                    break;
                case MagicSchool.Assassinatie:
                    TabButton.Index = 1613;
                    TabButton.Hint = "暗杀".Lang();
                    break;
                case MagicSchool.None:
                    TabButton.Index = 1610;
                    TabButton.Hint = "空置".Lang();
                    break;
                case MagicSchool.InternalSkill:
                    TabButton.Index = 1610;
                    TabButton.Hint = "内功";
                    TabButton.Visible = false;
                    break;
            }

            ScrollBar = new DXVScrollBar
            {
                Parent = this,
            };
            ScrollBar.ValueChanged += (o, e) => UpdateLocations();
        }

        #region Methods
        /// <summary>
        /// 更新位置
        /// </summary>
        public void UpdateLocations()
        {
            int y = -ScrollBar.Value + 5;

            foreach (DXControl control in Controls)
            {
                if (!(control is MagicJueSeCell)) continue;

                control.Location = new Point(15, y);
                y += control.Size.Height + 4;
            }
        }
        /// <summary>
        /// 鼠标滚轮使用
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            UpdateLocations();
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }
            }
        }
        #endregion
    }

    /// <summary>
    /// 魔法技能单元格
    /// </summary>
    public sealed class MagicJueSeCell : DXControl
    {
        #region Properties

        #region Info
        /// <summary>
        /// 魔法技能信息
        /// </summary>
        public MagicInfo Info
        {
            get => _Info;
            set
            {
                if (_Info == value) return;

                MagicInfo oldValue = _Info;
                _Info = value;

                OnInfoChanged(oldValue, value);
            }
        }
        private MagicInfo _Info;
        public event EventHandler<EventArgs> InfoChanged;
        public void OnInfoChanged(MagicInfo oValue, MagicInfo nValue)  //信息变化
        {
            Image.Index = Info.Icon;              //技能图标

            //NameLabel.Text = Info.Lang(p => p.Name);           //技能名字

            ///Type type = nValue.Action.GetType();
            //MemberInfo[] infos = type.GetMember(nValue.Action.ToString());

            //DescriptionAttribute description = infos[0].GetCustomAttribute<DescriptionAttribute>();
            //ActionLabel.Text = nValue.Action.Lang();       //主动被动标签

            //  Refresh();                            //刷新
            InfoChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        public DXImageControl Image, Image1;     //技能图标
        public DXLabel MagicLevel; //技能名字标签 等级标签 经验标签 快捷键标签 主动被动标签
        #endregion

        /// <summary>
        /// 魔法技能单元格
        /// </summary>
        public MagicJueSeCell()
        {
            Size = new Size(58, 58);    //单元格大小

            DrawTexture = true;          //绘制纹理
            //BackColour = Color.FromArgb(150, 25, 20, 0);  //背景颜色
            Image1 = new DXImageControl        //技能单元格背景图
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = 107,
                Location = new Point(0, 0),
                IsControl = false,
                PassThrough = true,
                Visible = false,
            };
            Image1.MouseEnter += ShowMagic;
            Image1.MouseLeave += HideMagic;
            Image1.MouseWheel += (s, e) => OnMouseWheel(e);

            Image = new DXImageControl       //技能图标
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(7, 7),
                BorderColour = Color.Yellow,
                Border = false,
            };
            Image.MouseEnter += ShowMagic;
            Image.MouseLeave += HideMagic;
            Image.MouseClick += Image_MouseClick;   //技能图标鼠标点击
            //Image.KeyDown += Image_KeyDown;         //技能图标按下
            Image.MouseWheel += (s, e) => OnMouseWheel(e);

            MagicLevel = new DXLabel
            {
                DrawFormat = TextFormatFlags.Default,
                Parent = this,
                Location = new Point(8 + 35, 8 + 35),
                Size = new Size(10, 12),
                IsControl = false,
                Text = "",
                ForeColour = Color.Yellow,
                Visible = true,
            };
        }

        private void HideMagic(object sender, EventArgs e)
        {
            GameScene.Game.MouseMagic = null;
        }

        private void ShowMagic(object sender, EventArgs e)
        {
            GameScene.Game.MouseMagic = Info;
        }

        #region Methods

        /// <summary>
        /// 鼠标点击技能图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            //GameScene.Game.MagicBox.SelectedMagic = this;
            GameScene.Game.MagicBox.MagicNameLabel.Text = magic.Info.Name;
            GameScene.Game.MagicBox.MagicNameLabel.Visible = true;
            int level = magic.Level;
            string text = (level == 3) ? "Level : MAX" : string.Format("Level : {0}", level);
            GameScene.Game.MagicBox.MagicLevelLabel.Text = text;
            GameScene.Game.MagicBox.MagicLevelLabel.Visible = true;
            if (level < 3)
            {
                switch (magic.Level)
                {
                    case 0:
                        text = string.Format("Exp : {0}/{1}", magic.Experience, magic.Info.Experience1);
                        break;
                    case 1:
                        text = string.Format("Exp : {0}/{1}", magic.Experience, magic.Info.Experience2);
                        break;
                    case 2:
                        text = string.Format("Exp : {0}/{1}", magic.Experience, magic.Info.Experience3);
                        break;
                    default:
                        text = string.Format("Exp : {0}/{1}", magic.Experience, (magic.Level - 2) * 500);
                        break;
                }
                GameScene.Game.MagicBox.MagicExperienceLabel.Text = text;
                GameScene.Game.MagicBox.MagicExperienceLabel.Visible = true;
            }
            else
            {
                GameScene.Game.MagicBox.MagicExperienceLabel.Visible = false;
            }
            /*
            switch (GameScene.Game.MagicBarBox.SpellSet)   //技能快捷栏
            {
                case 1:
                    magic.Set1Key = SpellKey.None;
                    break;
                case 2:
                    magic.Set2Key = SpellKey.None;
                    break;
                case 3:
                    magic.Set3Key = SpellKey.None;
                    break;
                case 4:
                    magic.Set4Key = SpellKey.None;
                    break;
            }

            CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
            Refresh();
            GameScene.Game.MagicBarBox.UpdateIcons();
            */
        }
        /// <summary>
        /// 技能图标被按下时绑定快捷
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_KeyDown(object sender, KeyEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            if (e.Handled) return;
            if (MouseControl != Image) return;

            SpellKey key = SpellKey.None;

            foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
            {
                switch (action)
                {
                    case KeyBindAction.SpellUse01:
                        key = SpellKey.Spell01;
                        break;
                    case KeyBindAction.SpellUse02:
                        key = SpellKey.Spell02;
                        break;
                    case KeyBindAction.SpellUse03:
                        key = SpellKey.Spell03;
                        break;
                    case KeyBindAction.SpellUse04:
                        key = SpellKey.Spell04;
                        break;
                    case KeyBindAction.SpellUse05:
                        key = SpellKey.Spell05;
                        break;
                    case KeyBindAction.SpellUse06:
                        key = SpellKey.Spell06;
                        break;
                    case KeyBindAction.SpellUse07:
                        key = SpellKey.Spell07;
                        break;
                    case KeyBindAction.SpellUse08:
                        key = SpellKey.Spell08;
                        break;
                    case KeyBindAction.SpellUse09:
                        key = SpellKey.Spell09;
                        break;
                    case KeyBindAction.SpellUse10:
                        key = SpellKey.Spell10;
                        break;
                    case KeyBindAction.SpellUse11:
                        key = SpellKey.Spell11;
                        break;
                    case KeyBindAction.SpellUse12:
                        key = SpellKey.Spell12;
                        break;
                    case KeyBindAction.SpellUse13:
                        key = SpellKey.Spell13;
                        break;
                    case KeyBindAction.SpellUse14:
                        key = SpellKey.Spell14;
                        break;
                    case KeyBindAction.SpellUse15:
                        key = SpellKey.Spell15;
                        break;
                    case KeyBindAction.SpellUse16:
                        key = SpellKey.Spell16;
                        break;
                    case KeyBindAction.SpellUse17:
                        key = SpellKey.Spell17;
                        break;
                    case KeyBindAction.SpellUse18:
                        key = SpellKey.Spell18;
                        break;
                    case KeyBindAction.SpellUse19:
                        key = SpellKey.Spell19;
                        break;
                    case KeyBindAction.SpellUse20:
                        key = SpellKey.Spell20;
                        break;
                    case KeyBindAction.SpellUse21:
                        key = SpellKey.Spell21;
                        break;
                    case KeyBindAction.SpellUse22:
                        key = SpellKey.Spell22;
                        break;
                    case KeyBindAction.SpellUse23:
                        key = SpellKey.Spell23;
                        break;
                    case KeyBindAction.SpellUse24:
                        key = SpellKey.Spell24;
                        break;
                    default:
                        continue;
                }
                e.Handled = true;
            }

            if (key == SpellKey.None) return;

            ClientUserMagic magic;

            if (!MapObject.User.Magics.TryGetValue(Info, out magic)) return;

            switch (GameScene.Game.MagicBarBox.SpellSet)
            {
                case 1:
                    magic.Set1Key = key;
                    break;
                case 2:
                    magic.Set2Key = key;
                    break;
                case 3:
                    magic.Set3Key = key;
                    break;
                case 4:
                    magic.Set4Key = key;
                    break;
            }

            foreach (KeyValuePair<MagicInfo, ClientUserMagic> pair in MapObject.User.Magics)
            {
                if (pair.Key == magic.Info) continue;

                if (pair.Value.Set1Key == magic.Set1Key && magic.Set1Key != SpellKey.None)
                {
                    pair.Value.Set1Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }

                if (pair.Value.Set2Key == magic.Set2Key && magic.Set2Key != SpellKey.None)
                {
                    pair.Value.Set2Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }

                if (pair.Value.Set3Key == magic.Set3Key && magic.Set3Key != SpellKey.None)
                {
                    pair.Value.Set3Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }

                if (pair.Value.Set4Key == magic.Set4Key && magic.Set4Key != SpellKey.None)
                {
                    pair.Value.Set4Key = SpellKey.None;
                    GameScene.Game.MagicBox.Magics[pair.Key].Refresh();
                }
            }
            //   CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
            //  Refresh();
            GameScene.Game.MagicBarBox.UpdateIcons();
        }
        /// <summary>
        /// 刷新
        /// </summary>
        public void Refresh()
        {
            if (MapObject.User == null) return;

            ClientUserMagic magic;

            if (MapObject.User.Magics.TryGetValue(Info, out magic))
            {
                SpellKey key = SpellKey.None;
                switch (GameScene.Game.MagicBarBox.SpellSet)
                {
                    case 1:
                        key = magic.Set1Key;
                        break;
                    case 2:
                        key = magic.Set2Key;
                        break;
                    case 3:
                        key = magic.Set3Key;
                        break;
                    case 4:
                        key = magic.Set4Key;
                        break;
                }
                //KeyImage.Visible = true;
                //KeyImage.Index = 1659 + ((int)key);
                MagicLevel.Visible = true;
                Image.Index = Info.Icon;
                Image.ImageOpacity = 1F;
                MagicLevel.Text = magic.Level.ToString();
            }
            else
            {
                Image.ImageOpacity = 0F;
            }
            if (this == MouseControl)
            {
                GameScene.Game.MouseMagic = null;
                GameScene.Game.MouseMagic = Info;
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Info = null;
                InfoChanged = null;

                if (Image != null)
                {
                    if (!Image.IsDisposed)
                        Image.Dispose();

                    Image = null;
                }


            }
        }
        #endregion
    }
}
