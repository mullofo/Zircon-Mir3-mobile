﻿using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using S = Library.Network.ServerPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 排行版功能
    /// </summary>
    public sealed class RankingDialog : DXWindow
    {
        #region Properties

        #region StartIndex
        /// <summary>
        /// 起始索引
        /// </summary>
        public int StartIndex
        {
            get => _StartIndex;
            set
            {
                if (_StartIndex == value) return;

                int oldValue = _StartIndex;
                _StartIndex = value;

                OnStartIndexChanged(oldValue, value);
            }
        }
        private int _StartIndex;
        public event EventHandler<EventArgs> StartIndexChanged;
        public void OnStartIndexChanged(int oValue, int nValue)
        {
            UpdateTime = CEnvir.Now.AddMilliseconds(250);

            if (nValue > oValue)
                for (int i = 0; i < Lines.Length; i++)
                {
                    if (nValue - oValue + i < Lines.Length)
                    {
                        if (Lines[i + nValue - oValue].Rank != null)
                        {
                            Lines[i].Rank = Lines[i + nValue - oValue].Rank;
                        }
                        else
                            Lines[i].Loading = true;
                    }
                    else
                        Lines[i].Loading = true;

                }
            else
                for (int i = Lines.Length - 1; i >= 0; i--)
                {
                    if (nValue - oValue + i >= 0)
                    {
                        if (Lines[i + nValue - oValue].Rank != null)
                            Lines[i].Rank = Lines[i + nValue - oValue].Rank;
                        else
                            Lines[i].Loading = true;
                    }
                    else
                        Lines[i].Loading = true;
                }

            StartIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Class
        /// <summary>
        /// 职业
        /// </summary>
        public RequiredClass Class
        {
            get => _Class;
            set
            {
                if (_Class == value) return;

                RequiredClass oldValue = _Class;
                _Class = value;

                OnClassChanged(oldValue, value);
            }
        }
        private RequiredClass _Class;
        public event EventHandler<EventArgs> ClassChanged;
        public void OnClassChanged(RequiredClass oValue, RequiredClass nValue)
        {
            ScrollBar.Value = 0;
            UpdateTime = CEnvir.Now;

            foreach (RankingLine line in Lines)
                line.Loading = true;

            ClassChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region OnlineOnly
        /// <summary>
        /// 在线状态
        /// </summary>
        public bool OnlineOnly
        {
            get => _OnlineOnly;
            set
            {
                if (_OnlineOnly == value) return;

                bool oldValue = _OnlineOnly;
                _OnlineOnly = value;

                OnOnlineOnlyChanged(oldValue, value);
            }
        }
        private bool _OnlineOnly;
        public event EventHandler<EventArgs> OnlineOnlyChanged;
        public void OnOnlineOnlyChanged(bool oValue, bool nValue)
        {
            ScrollBar.Value = 0;
            UpdateTime = CEnvir.Now;

            foreach (RankingLine line in Lines)
                line.Loading = true;

            OnlineOnlyChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Observable
        /// <summary>
        /// 观察者
        /// </summary>
        public bool Observable
        {
            get => _Observable;
            set
            {
                if (_Observable == value) return;

                bool oldValue = _Observable;
                _Observable = value;

                OnObserverableChanged(oldValue, value);
            }
        }
        private bool _Observable;
        public event EventHandler<EventArgs> ObserverableChanged;
        public void OnObserverableChanged(bool oValue, bool nValue)
        {
            if (Observable)
            {
                ObserverButton.Index = 121;
                ObserverButton.Hint = "Ranking.ObserverOn".Lang();
            }
            else
            {
                ObserverButton.Index = 141;
                ObserverButton.Hint = "Ranking.ObserverOff".Lang();
            }

            ObserverableChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DateTime UpdateTime;

        public DXImageControl RakingsGround;
        public DXLabel Title1Label;

        private DXControl Panel;
        private DXVScrollBar ScrollBar;

        public DXComboBox RequiredClassBox;
        public DXCheckBox OnlineOnlyBox;
        public DXButton ObserverButton;

        public RankingLine[] Lines;

        public override WindowType Type => WindowType.RankingBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 排行榜界面
        /// </summary>
        public RankingDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            Size = new Size(450, 540);
            Location = ClientArea.Location;

            RakingsGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 700,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };

            CloseButton.Parent = RakingsGround;

            Title1Label = new DXLabel
            {
                Text = "排行榜",
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                ForeColour = Color.FromArgb(198, 166, 99),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
            };
            Title1Label.Location = new Point((Size.Width - Title1Label.Size.Width) / 2, 15);

            Panel = new DXControl
            {
                Location = new Point(ClientArea.Location.X + 6, ClientArea.Location.Y + 50),
                Size = new Size(ClientArea.Size.Width - 10, ClientArea.Size.Height - 20),
                Parent = this,
            };

            Lines = new RankingLine[20];
            ScrollBar = new DXVScrollBar       //滚动条
            {
                Parent = this,
                Size = new Size(25, 490),
                Location = new Point(Panel.Size.Width - 10, 30),
                VisibleSize = Lines.Length,
                Change = 5,
            };
            ScrollBar.ValueChanged += (o, e) => StartIndex = ScrollBar.Value;
            MouseWheel += ScrollBar.DoMouseWheel;

            //为滚动条自定义皮肤 -1为不设置
            ScrollBar.SetSkin(LibraryFile.UI1, -1, -1, -1, 1225);

            new RankingLine
            {
                Parent = Panel,
                Header = true,
            };

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i] = new RankingLine
                {
                    Parent = Panel,
                    Location = new Point(0, 22 * (i + 1)),
                    Visible = false,
                };
                Lines[i].MouseWheel += ScrollBar.DoMouseWheel;
            }

            ObserverButton = new DXButton   //是否被观察开关
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 141,
                Hint = "Ranking.ObserverOff".Lang(),
            };
            ObserverButton.MouseClick += (o, e) =>  //鼠标点击
            {
                if (GameScene.Game == null) return;
                if (GameScene.Game.Observer) return;
                if (!GameScene.Game.User.InSafeZone)
                {
                    GameScene.Game.ReceiveChat("你只能在安全区内更改观察者模式".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.ObservableSwitch { Allow = !Observable });
            };
            ObserverButton.Location = new Point(ClientArea.Right - ObserverButton.Size.Width - 50, ClientArea.Top + 30);
            if (!CEnvir.ClientControl.ObserverSwitchCheck)
            {
                ObserverButton.Visible = false;
            }
            else
            {
                ObserverButton.Visible = true;
            }

            OnlineOnlyBox = new DXCheckBox   //在线选择
            {
                Parent = this,
                Label = { Text = "在线".Lang() },
            };
            OnlineOnlyBox.CheckedChanged += (o, e) =>
            {
                OnlineOnly = OnlineOnlyBox.Checked;
                Config.RankingOnline = OnlineOnly;
            };
            OnlineOnlyBox.Location = new Point(269 - OnlineOnlyBox.Size.Width + ClientArea.X, ClientArea.Y + 30);

            RequiredClassBox = new DXComboBox    //职业选择
            {
                Parent = this,
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
                Location = new Point(ClientArea.X + 43, ClientArea.Y + 30)
            };
            RequiredClassBox.SelectedItemChanged += (o, e) =>
            {
                Class = (RequiredClass?)RequiredClassBox.SelectedItem ?? RequiredClass.All;
                Config.RankingClass = (int)Class;
            };
            //  RequiredClassBox.Location = new Point(ClientArea.Right - RefineQualityBox.Size.Width - 160, BlackIronGrid.Location.Y);

            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = "全部".Lang() },
                Item = RequiredClass.All
            };

            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = "战士".Lang() },
                Item = RequiredClass.Warrior
            };
            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = "法师".Lang() },
                Item = RequiredClass.Wizard
            };
            new DXListBoxItem
            {
                Parent = RequiredClassBox.ListBox,
                Label = { Text = "道士".Lang() },
                Item = RequiredClass.Taoist
            };

            //new DXListBoxItem
            //{
            //    Parent = RequiredClassBox.ListBox,
            //    Label = { Text = "刺客".Lang() },
            //    Item = RequiredClass.Assassin
            //};

            DXLabel label = new DXLabel   //职业标签
            {
                Parent = this,
                Text = "职业".Lang(),
            };
            label.Location = new Point(RequiredClassBox.Location.X - label.Size.Width - 5, RequiredClassBox.Location.Y + (RequiredClassBox.Size.Height - label.Size.Height) / 2);

            RequiredClassBox.ListBox.SelectItem((RequiredClass)Config.RankingClass);
            OnlineOnlyBox.Checked = Config.RankingOnline;
        }

        #region Methods
        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            if (CEnvir.Now < UpdateTime) return;

            UpdateTime = CEnvir.Now.AddSeconds(10);

            CEnvir.Enqueue(new C.RankRequest
            {
                Class = Class,
                OnlineOnly = OnlineOnly,
                StartIndex = StartIndex,
            });
        }
        /// <summary>
        /// 更新排行榜数据
        /// </summary>
        /// <param name="p"></param>
        public void Update(S.Rankings p)
        {
            if (p.Class != Class || p.OnlineOnly != OnlineOnly) return;

            ScrollBar.MaxValue = p.Total;

            for (int i = 0; i < Lines.Length; i++)
            {
                Lines[i].Loading = false;
                Lines[i].Rank = i >= p.Ranks.Count ? null : p.Ranks[i];
            }
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _StartIndex = 0;
                StartIndexChanged = null;

                _Class = 0;
                ClassChanged = null;

                _OnlineOnly = false;
                OnlineOnlyChanged = null;

                _Observable = false;
                ObserverableChanged = null;

                UpdateTime = DateTime.MinValue;

                if (RakingsGround != null)
                {
                    if (!RakingsGround.IsDisposed)
                        RakingsGround.Dispose();

                    RakingsGround = null;
                }

                if (Title1Label != null)
                {
                    if (!Title1Label.IsDisposed)
                        Title1Label.Dispose();

                    Title1Label = null;
                }

                if (Panel != null)
                {
                    if (!Panel.IsDisposed)
                        Panel.Dispose();

                    Panel = null;
                }

                if (ScrollBar != null)
                {
                    if (!ScrollBar.IsDisposed)
                        ScrollBar.Dispose();

                    ScrollBar = null;
                }

                if (RequiredClassBox != null)
                {
                    if (!RequiredClassBox.IsDisposed)
                        RequiredClassBox.Dispose();

                    RequiredClassBox = null;
                }

                if (OnlineOnlyBox != null)
                {
                    if (!OnlineOnlyBox.IsDisposed)
                        OnlineOnlyBox.Dispose();

                    OnlineOnlyBox = null;
                }

                if (ObserverButton != null)
                {
                    if (!ObserverButton.IsDisposed)
                        ObserverButton.Dispose();

                    ObserverButton = null;
                }

                if (Lines != null)
                {
                    for (int i = 0; i < Lines.Length; i++)
                    {
                        if (Lines[i] != null)
                        {
                            if (!Lines[i].IsDisposed)
                                Lines[i].Dispose();

                            Lines[i] = null;
                        }
                    }
                    Lines = null;
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 在线排名更新
    /// </summary>
    public sealed class RankingLine : DXControl
    {
        #region Properties

        #region Header
        /// <summary>
        /// 头文件
        /// </summary>
        public bool Header
        {
            get => _Header;
            set
            {
                if (_Header == value) return;

                bool oldValue = _Header;
                _Header = value;

                OnHeaderChanged(oldValue, value);
            }
        }
        private bool _Header;
        public event EventHandler<EventArgs> HeaderChanged;
        public void OnHeaderChanged(bool oValue, bool nValue)
        {
            RankLabel.Text = "排行".Lang();
            NameLabel.Text = "名字".Lang();
            ClassLabel.Text = "职业".Lang();
            LevelLabel.Text = "等级".Lang();
            //RebirthLabel.Text = "转生".Lang();

            DrawTexture = false;

            RankLabel.ForeColour = Color.FromArgb(198, 166, 99);
            NameLabel.ForeColour = Color.FromArgb(198, 166, 99);
            ClassLabel.ForeColour = Color.FromArgb(198, 166, 99);
            LevelLabel.ForeColour = Color.FromArgb(198, 166, 99);
            //RebirthLabel.ForeColour = Color.FromArgb(198, 166, 99);

            OnlineImage.Visible = false;

            ObseverButton.Dispose();
            HeaderChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Rank
        /// <summary>
        /// 排行信息
        /// </summary>
        public RankInfo Rank
        {
            get => _Rank;
            set
            {
                if (_Rank == value) return;

                RankInfo oldValue = _Rank;
                _Rank = value;

                OnRankChanged(oldValue, value);
            }
        }
        private RankInfo _Rank;
        public event EventHandler<EventArgs> RankChanged;
        public void OnRankChanged(RankInfo oValue, RankInfo nValue)
        {
            if (Rank == null)
            {
                RankLabel.Text = "";
                NameLabel.Text = "";
                ClassLabel.Text = "";
                LevelLabel.Text = "";
                //RebirthLabel.Text = "";
                ObseverButton.Enabled = false;
                Visible = !Loading;
            }
            else
            {
                Visible = true;
                RankLabel.Text = Rank.Rank.ToString();
                NameLabel.Text = Rank.Name;


                ClassLabel.Text = Rank.Class.Lang();
                RankLabel.ForeColour = Color.Silver;
                NameLabel.ForeColour = Color.Silver;
                ClassLabel.ForeColour = Color.Silver;
                LevelLabel.ForeColour = Color.Silver;
                //RebirthLabel.ForeColour = Color.OrangeRed;
                OnlineImage.Index = Rank.Online ? 3625 : 3624;

                ObseverButton.Enabled = Rank.Online && Rank.Observable;

                decimal percent = 0;

                Rank.Level -= Rank.Rebirth * 5000;

                if (Rank.Level < 0) return;

                if (Rank.Level < Globals.GamePlayEXPInfoList.Count)         //等级经验
                    percent = Math.Min(1, Math.Max(0, Globals.GamePlayEXPInfoList[Rank.Level].Exp > 0 ? Rank.Experience / Globals.GamePlayEXPInfoList[Rank.Level].Exp : 0));

                LevelLabel.Text = $"{Rank.Level}"; // - {percent:0.##%}";
                //if (Rank.Rebirth > 0)
                //    RebirthLabel.Text = Rank.Rebirth == 1 ? string.Format("({0} " + "转".Lang() + ")", Rank.Rebirth) : string.Format("({0} " + "转".Lang() + ")", Rank.Rebirth);
                //else RebirthLabel.Text = "";
            }

            RankChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Loading
        /// <summary>
        /// 读取信息
        /// </summary>
        public bool Loading
        {
            get => _Loading;
            set
            {
                if (_Loading == value) return;

                bool oldValue = _Loading;
                _Loading = value;

                OnLoadingChanged(oldValue, value);
            }
        }
        private bool _Loading;
        public event EventHandler<EventArgs> LoadingChanged;
        public void OnLoadingChanged(bool oValue, bool nValue)
        {
            if (!Loading)
            {
                RankLabel.Text = "";
                NameLabel.Text = "";
                ClassLabel.Text = "";
                LevelLabel.Text = "";
                //RebirthLabel.Text = "";

                Visible = false;
                return;
            }

            Rank = null;
            NameLabel.Text = "更新中".Lang();
            NameLabel.ForeColour = Color.Orange;
            Visible = true;

            LoadingChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        public DXLabel RankLabel, NameLabel, ClassLabel, LevelLabel, RebirthLabel;
        public DXButton ObseverButton;
        public DXImageControl OnlineImage;

        #endregion

        /// <summary>
        /// 排行榜更新
        /// </summary>
        public RankingLine()
        {
            Size = new Size(395, 20);
            DrawTexture = true;
            //BackColour = Color.FromArgb(25, 20, 0);

            OnlineImage = new DXImageControl  //在线图标
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3624,
                Location = new Point(8, 4),
                IsControl = false,
            };

            RankLabel = new DXLabel   //排行榜标签
            {
                Parent = this,
                //  Border = true,
                AutoSize = false,
                Location = new Point(OnlineImage.Location.X + OnlineImage.Size.Width - 4, 0),
                Size = new Size(40, 18),
                // BorderColour = Color.FromArgb(198, 166, 98),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            NameLabel = new DXLabel   //名字标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(RankLabel.Location.X + RankLabel.Size.Width + 1, 0),
                Size = new Size(150, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            ClassLabel = new DXLabel   //职业标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(NameLabel.Location.X + NameLabel.Size.Width + 1, 0),
                Size = new Size(50, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            LevelLabel = new DXLabel   //等级标签
            {
                Parent = this,
                AutoSize = false,
                Location = new Point(ClassLabel.Location.X + ClassLabel.Size.Width + 1, 0),
                Size = new Size(70, 18),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                IsControl = false,
            };

            //RebirthLabel = new DXLabel   //转生标签
            //{
            //    Parent = this,
            //    AutoSize = false,
            //    Location = new Point(LevelLabel.Location.X + LevelLabel.Size.Width + 1, 0),
            //    Size = new Size(55, 18),
            //    ForeColour = Color.White,
            //    DrawFormat = TextFormatFlags.VerticalCenter,
            //    IsControl = false,
            //};

            ObseverButton = new DXButton   //观察者按钮
            {
                Parent = this,
                Location = new Point(LevelLabel.Location.X + LevelLabel.Size.Width + 5, 1),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "观察".Lang() },
                Enabled = false,
                Size = new Size(53, SmallButtonHeight)
            };
            ObseverButton.MouseClick += (o, e) =>   //鼠标点击
            {
                if (GameScene.Game != null && CEnvir.Now < GameScene.Game.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer)
                {
                    GameScene.Game.ReceiveChat("战斗状态下不能观察".Lang(), MessageType.System);
                    return;
                }

                CEnvir.Enqueue(new C.ObserverRequest { Name = Rank.Name });
            };
            if (!CEnvir.ClientControl.ObserverSwitchCheck)
            {
                ObseverButton.Visible = false;
            }
            else
            {
                ObseverButton.Visible = true;
            }
        }

        #region Methods
        /// <summary>
        /// 鼠标点击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            if (GameScene.Game == null || Rank == null) return;

            CEnvir.Enqueue(new C.Inspect { Index = Rank.Index });
        }
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            if (Header) return;
            BackColour = Color.FromArgb(80, 80, 125);
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();

            if (Header) return;

            BackColour = Color.Empty;
        }
        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Header = false;
                HeaderChanged = null;

                _Rank = null;
                RankChanged = null;

                _Loading = false;
                LoadingChanged = null;

                if (RankLabel != null)
                {
                    if (!RankLabel.IsDisposed)
                        RankLabel.Dispose();

                    RankLabel = null;
                }

                if (NameLabel != null)
                {
                    if (!NameLabel.IsDisposed)
                        NameLabel.Dispose();

                    NameLabel = null;
                }

                if (ClassLabel != null)
                {
                    if (!ClassLabel.IsDisposed)
                        ClassLabel.Dispose();

                    ClassLabel = null;
                }

                if (LevelLabel != null)
                {
                    if (!LevelLabel.IsDisposed)
                        LevelLabel.Dispose();

                    LevelLabel = null;
                }

                if (RebirthLabel != null)
                {
                    if (!RebirthLabel.IsDisposed)
                        RebirthLabel.Dispose();

                    RebirthLabel = null;
                }

                if (ObseverButton != null)
                {
                    if (!ObseverButton.IsDisposed)
                        ObseverButton.Dispose();

                    ObseverButton = null;
                }

                if (OnlineImage != null)
                {
                    if (!OnlineImage.IsDisposed)
                        OnlineImage.Dispose();

                    OnlineImage = null;
                }
            }
        }
        #endregion
    }
}
