using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using Library.Network.ClientPackets;
using Library.SystemModels;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 泰山许愿BUFF功能
    /// </summary>
    public sealed class VowDialog : DXWindow
    {
        /// <summary>
        /// 距离值
        /// </summary>
        public DXLabel Distance;
        /// <summary>
        /// 角度值
        /// </summary>
        public DXLabel Angle;
        /// <summary>
        /// 免费的次数值
        /// </summary>
        public DXLabel FreeCount;
        /// <summary>
        /// 包裹内幸运币数量
        /// </summary>
        public DXLabel LuckyCoinCount;
        /// <summary>
        /// 背景底图
        /// </summary>
        public DXImageControl Vowdsground;
        /// <summary>
        /// 幸运币图标
        /// </summary>
        public DXImageControl LuckyCoin;
        /// <summary>
        /// 选择投币次数的背景框
        /// </summary>
        public DXImageControl ComBoBack;
        /// <summary>
        /// 幸运币效果
        /// </summary>
        public DXImageControl CoinCursor;
        /// <summary>
        /// 选择投币次数的列表框
        /// </summary>
        public DXComboBox CombThrowCount;
        /// <summary>
        /// 投币开始
        /// </summary>
        public DXAnimatedControl ThrowAni;
        /// <summary>
        /// 投币结束
        /// </summary>
        [AutoDispose]
        public DXAnimatedControl ThrowSuccess, ThrowFail;
        /// <summary>
        /// 投掷角度
        /// </summary>
        public int angle;

        [AutoDispose] public DXImageControl BuffIcon1, BuffIcon2, BuffIcon3, BuffIcon4, BuffIcon5, BuffIcon6;
        [AutoDispose] public DXImageControl BuffLevel1, BuffLevel2, BuffLevel3, BuffLevel4, BuffLevel5, BuffLevel6;
        [AutoDispose] public DXImageControl BuffLevelNum1, BuffLevelNum2, BuffLevelNum3, BuffLevelNum4, BuffLevelNum5, BuffLevelNum6;

        public int BuffIndex1
        {
            get => _buffIndex1;
            set
            {
                _buffIndex1 = value;
                UpdateBuffIcon(1, _buffIndex1);
            }
        }

        public int BuffIndex2
        {
            get => _buffIndex2;
            set
            {
                _buffIndex2 = value;
                UpdateBuffIcon(2, _buffIndex2);
            }
        }

        public int BuffIndex3
        {
            get => _buffIndex3;
            set
            {
                _buffIndex3 = value;
                UpdateBuffIcon(3, _buffIndex3);
            }
        }

        public int BuffIndex4
        {
            get => _buffIndex4;
            set
            {
                _buffIndex4 = value;
                UpdateBuffIcon(4, _buffIndex4);
            }
        }

        public int BuffIndex5
        {
            get => _buffIndex5;
            set
            {
                _buffIndex5 = value;
                UpdateBuffIcon(5, _buffIndex5);
            }
        }

        public int BuffIndex6
        {
            get => _buffIndex6;
            set
            {
                _buffIndex6 = value;
                UpdateBuffIcon(6, _buffIndex6);
            }
        }

        private int _remainingFreeTosses;
        public int RemainingFreeTosses
        {
            get => _remainingFreeTosses;
            set
            {
                _remainingFreeTosses = value;
                if (FreeCount != null)
                {
                    //Free tosses: {}
                    FreeCount.Text = $"免费次数".Lang() + $": {_remainingFreeTosses}";
                }
            }
        }

        /// <summary>
        /// 投掷阶段
        /// </summary>
        private ThrowingStage _stage;

        public ThrowingStage Stage
        {
            get => _stage;
            set
            {
                if (_stage == value) return;
                OnStageChange(_stage, value);
                _stage = value;
            }
        }
        /// <summary>
        /// 银币第二阶段移动的方向值
        /// </summary>
        public int dir;
        /// <summary>
        /// 初始距离
        /// </summary>
        public double initialDistance;
        /// <summary>
        /// 用户选的距离
        /// </summary>
        public double selectedDistance;

        public bool IsOnTarget = false;

        public override WindowType Type => WindowType.VowBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        /// <summary>
        /// 距离数值
        /// </summary>
        private string describe = "距离".Lang() + "：{0}m";
        /// <summary>
        /// 鼠标进入对话框显示幸运币图标指针
        /// </summary>
        private bool MouseInDialog => CoinCursor.Visible;

        public DateTime ProcessTime = DateTime.MinValue;
        private int _buffIndex1;
        private int _buffIndex2;
        private int _buffIndex3;
        private int _buffIndex4;
        private int _buffIndex5;
        private int _buffIndex6;

        /// <summary>
        /// 泰山许愿BUFF界面
        /// </summary>
        public VowDialog()
        {
            Movable = true;
            Size = new Size(256, 372);

            angle = 0;
            dir = 1;
            Stage = ThrowingStage.Tbegin;

            Vowdsground = new DXImageControl   //底图
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1900,
                Opacity = 0.7F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            Distance = new DXLabel   //距离值
            {
                ForeColour = Color.Yellow,
                Location = new Point(5, 40),
                Parent = this,
                Text = describe
            };

            Angle = new DXLabel  //角度值
            {
                ForeColour = Color.Yellow,
                Location = new Point(5, 60),
                Parent = this,
                Text = $"角度".Lang() + $"：{0}" + "度".Lang()
            };

            FreeCount = new DXLabel  //免费次数
            {
                ForeColour = Color.Yellow,
                Location = new Point(168, 40),
                Parent = this,
                Text = $"免费次数".Lang() + $"：{0}"
            };

            LuckyCoin = new DXImageControl   //幸运币图标
            {
                LibraryFile = LibraryFile.GameInter2,
                Index = 1903,
                Parent = this,
                Location = new Point(195, 65),
                Hint = "幸运币数量".Lang(),
            };

            LuckyCoinCount = new DXLabel   //包裹内幸运币数量
            {
                ForeColour = Color.Yellow,
                Location = new Point(220, 70),
                DrawFormat = TextFormatFlags.Right,
                Parent = this,
                Text = $"{0}"
            };

            ComBoBack = new DXImageControl  //选择投掷次数的背景框
            {
                LibraryFile = LibraryFile.Interface,
                Index = 144,
                TilingMode = TilingMode.Vertically,
                ImageOpacity = 1F,
                Border = false,
                Parent = this,
                Location = new Point(47, 292),
                Size = new Size(162, 18),
                FixedSize = true,
            };

            CombThrowCount = new DXComboBox  //选择投掷的次数
            {
                Parent = ComBoBack,
                Border = true,
                BorderColour = Color.FromArgb(67, 64, 57),
            };
            CombThrowCount.Location = new Point(1, 1);
            new DXListBoxItem
            {
                Parent = CombThrowCount.ListBox,
                Label = { Text = $"1次投掷".Lang() },
                Item = TossCoinOption.Once,
            };
            new DXListBoxItem
            {
                Parent = CombThrowCount.ListBox,
                Label = { Text = $"10次连续(免费 + 一般)".Lang() },
                Item = TossCoinOption.TenTimes,
            };
            new DXListBoxItem
            {
                Parent = CombThrowCount.ListBox,
                Label = { Text = $"100次连续(免费 + 一般)".Lang() },
                Item = TossCoinOption.HundredTimes,
            };
            new DXListBoxItem
            {
                Parent = CombThrowCount.ListBox,
                Label = { Text = $"10次连续(高级硬币)".Lang() },
                Item = TossCoinOption.TenTimesAdvanced,
            };
            new DXListBoxItem
            {
                Parent = CombThrowCount.ListBox,
                Label = { Text = $"20次连续(高级硬币)".Lang() },
                Item = TossCoinOption.HundredTimesAdvanced,
            };

            CombThrowCount.SelectedItemChanged += (o, e) =>
            {
                DXComboBox comb = o as DXComboBox;
                if (comb?.ListBox.SelectedItem.Item != null)
                {
                    Config.ThrowSetting = (int)(TossCoinOption)comb.ListBox.SelectedItem.Item;

                    if (ThrowAni != null)
                    {
                        if ((TossCoinOption)comb.ListBox.SelectedItem.Item != TossCoinOption.Once)
                        {
                            ThrowAni.BaseIndex = 4060;
                            ThrowAni.FrameCount = 25;
                            ThrowAni.Location = new Point(-130, -50);
                        }
                        else
                        {
                            ThrowAni.BaseIndex = 4050;
                            ThrowAni.FrameCount = 6;
                            ThrowAni.Location = new Point(-175, 170);
                        }
                    }
                }
            };

            CombThrowCount.ListBox.SelectItem((TossCoinOption)Config.ThrowSetting);
            CombThrowCount.Size = new Size(160, 18);

            //硬币做光标
            CoinCursor = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1902,
                Opacity = 1F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
                Visible = false,
            };

            ThrowAni = new DXAnimatedControl  //成功
            {
                LibraryFile = LibraryFile.GameInter2,
                BaseIndex = 4050,
                Animated = true,
                AnimationDelay = TimeSpan.FromMilliseconds(800),
                AnimationStart = DateTime.MinValue,
                FrameCount = 6,
                Parent = this,
                Location = new Point(0, 0),
                Blend = true,
                Loop = false
            };
            ThrowAni.AfterAnimation += (sender, args) =>
            {
                ThrowAni.Visible = false;
                Stage = ThrowingStage.TJudging;
            };

            ThrowSuccess = new DXAnimatedControl  //失败
            {
                LibraryFile = LibraryFile.GameInter2,
                BaseIndex = 4100,
                Animated = true,
                AnimationDelay = TimeSpan.FromMilliseconds(800),
                AnimationStart = DateTime.MinValue,
                FrameCount = 12,
                Parent = this,
                Location = new Point(-130, -50),
                Blend = true,
                Loop = false,
                Visible = false,
            };
            ThrowSuccess.AfterAnimation += (sender, args) =>
            {
                ThrowSuccess.Visible = false;
                Stage = ThrowingStage.TDone;
            };

            ThrowFail = new DXAnimatedControl  //失败
            {
                LibraryFile = LibraryFile.GameInter2,
                BaseIndex = 4090,
                Animated = true,
                AnimationDelay = TimeSpan.FromMilliseconds(800),
                AnimationStart = DateTime.MinValue,
                FrameCount = 8,
                Parent = this,
                Location = new Point(0, 0),
                Blend = true,
                Loop = false,
                Visible = false,
            };
            ThrowFail.AfterAnimation += (sender, args) =>
            {
                ThrowFail.Visible = false;
                Stage = ThrowingStage.TDone;
            };

            initialDistance = (float)CEnvir.Random.Next(30, 49) / 10;  //初始距离
            if (null != Distance) Distance.Text = $"距离".Lang() + $"：{initialDistance:0.##}m";

            BuffIcon1 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.CBIcon,
                Index = 251,
                Opacity = 1F,
                Location = new Point(12, 320),
                IsControl = true,
                PassThrough = false,
                Visible = false,
                Tag = BuffIndex1,
            };
            BuffIcon1.MouseClick += BuffIconOnMouseClick;
            BuffIcon1.TagChanged += BuffIconOnTagChanged;

            BuffLevel1 = new DXImageControl
            {
                Parent = BuffIcon1,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1901,
                Opacity = 1F,
                Location = new Point(8, 25),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
            BuffLevelNum1 = new DXImageControl
            {
                Parent = BuffIcon1,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1910,
                Opacity = 1F,
                Location = new Point(26, 22),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };

            BuffIcon2 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.CBIcon,
                Index = 251,
                Opacity = 1F,
                Location = new Point(50, 320),
                IsControl = true,
                PassThrough = false,
                Visible = false,
                Tag = BuffIndex2,
            };
            BuffIcon2.MouseClick += BuffIconOnMouseClick;
            BuffIcon2.TagChanged += BuffIconOnTagChanged;

            BuffLevel2 = new DXImageControl
            {
                Parent = BuffIcon2,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1901,
                Opacity = 1F,
                Location = new Point(8, 25),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
            BuffLevelNum2 = new DXImageControl
            {
                Parent = BuffIcon2,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1910,
                Opacity = 1F,
                Location = new Point(26, 22),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };

            BuffIcon3 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.CBIcon,
                Index = 251,
                Opacity = 1F,
                Location = new Point(90, 320),
                IsControl = true,
                PassThrough = false,
                Visible = false,
                Tag = BuffIndex3,
            };
            BuffIcon3.MouseClick += BuffIconOnMouseClick;
            BuffIcon3.TagChanged += BuffIconOnTagChanged;

            BuffLevel3 = new DXImageControl
            {
                Parent = BuffIcon3,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1901,
                Opacity = 1F,
                Location = new Point(8, 25),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
            BuffLevelNum3 = new DXImageControl
            {
                Parent = BuffIcon3,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1910,
                Opacity = 1F,
                Location = new Point(26, 22),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };

            BuffIcon4 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.CBIcon,
                Index = 251,
                Opacity = 1F,
                Location = new Point(130, 320),
                IsControl = true,
                PassThrough = false,
                Visible = false,
                Tag = BuffIndex4,
            };
            BuffIcon4.MouseClick += BuffIconOnMouseClick;
            BuffIcon4.TagChanged += BuffIconOnTagChanged;

            BuffLevel4 = new DXImageControl
            {
                Parent = BuffIcon4,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1901,
                Opacity = 1F,
                Location = new Point(8, 25),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
            BuffLevelNum4 = new DXImageControl
            {
                Parent = BuffIcon4,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1910,
                Opacity = 1F,
                Location = new Point(26, 22),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };

            BuffIcon5 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.CBIcon,
                Index = 251,
                Opacity = 1F,
                Location = new Point(170, 320),
                IsControl = true,
                PassThrough = false,
                Visible = false,
                Tag = BuffIndex5,
            };
            BuffIcon5.MouseClick += BuffIconOnMouseClick;
            BuffIcon5.TagChanged += BuffIconOnTagChanged;

            BuffLevel5 = new DXImageControl
            {
                Parent = BuffIcon5,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1901,
                Opacity = 1F,
                Location = new Point(8, 25),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
            BuffLevelNum5 = new DXImageControl
            {
                Parent = BuffIcon5,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1910,
                Opacity = 1F,
                Location = new Point(26, 22),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };

            BuffIcon6 = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.CBIcon,
                Index = 251,
                Opacity = 1F,
                Location = new Point(210, 320),
                IsControl = true,
                PassThrough = false,
                Visible = false,
                Tag = BuffIndex6,
            };
            BuffIcon6.MouseClick += BuffIconOnMouseClick;
            BuffIcon6.TagChanged += BuffIconOnTagChanged;

            BuffLevel6 = new DXImageControl
            {
                Parent = BuffIcon6,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1901,
                Opacity = 1F,
                Location = new Point(8, 25),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
            BuffLevelNum6 = new DXImageControl
            {
                Parent = BuffIcon6,
                LibraryFile = LibraryFile.GameInter2,
                Index = 1910,
                Opacity = 1F,
                Location = new Point(26, 22),
                IsControl = false,
                PassThrough = true,
                Visible = true,
            };
        }

        #region Methods

        private void BuffIconOnTagChanged(object sender, EventArgs e)
        {
            DXImageControl source = sender as DXImageControl;
            if (source == null) return;
            //todo 显示获得特效？

        }

        private void BuffIconOnMouseClick(object sender, MouseEventArgs e)
        {
            DXImageControl source = sender as DXImageControl;
            if (source == null || source.Tag == null) return;
            CustomBuffInfo buffInfo = source.Tag as CustomBuffInfo;
            if (buffInfo == null) return;

            new DXConfirmWindow("确认删除此BUFF?".Lang(), DXMessageBoxButtons.YesNo, () =>
            {
                CEnvir.Enqueue(new RemoveTaishanBuff { BuffIndex = buffInfo.Index });
            });

        }

        public void UpdateBuffIcon(int index, int buffIndex)
        {
            DXImageControl icon, level, levelNum;
            switch (index)
            {
                case 1:
                    icon = BuffIcon1;
                    level = BuffLevel1;
                    levelNum = BuffLevelNum1;
                    break;
                case 2:
                    icon = BuffIcon2;
                    level = BuffLevel2;
                    levelNum = BuffLevelNum2;
                    break;
                case 3:
                    icon = BuffIcon3;
                    level = BuffLevel3;
                    levelNum = BuffLevelNum3;
                    break;
                case 4:
                    icon = BuffIcon4;
                    level = BuffLevel4;
                    levelNum = BuffLevelNum4;
                    break;
                case 5:
                    icon = BuffIcon5;
                    level = BuffLevel5;
                    levelNum = BuffLevelNum5;
                    break;
                case 6:
                    icon = BuffIcon6;
                    level = BuffLevel6;
                    levelNum = BuffLevelNum6;
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (icon == null || level == null || levelNum == null) return;
            if (buffIndex <= 0)
            {
                icon.Visible = false;
                icon.Tag = null;
                icon.Hint = string.Empty;
                return;
            }

            CustomBuffInfo buffInfo = Globals.CustomBuffInfoList.Binding.FirstOrDefault(x => x.Index == buffIndex);
            if (buffInfo != null)
            {
                icon.Visible = true;
                icon.Tag = buffInfo;
                icon.Index = buffInfo.BigBuffIcon;
                icon.Hint = buffInfo.Lang(p => p.BuffName) + "\n";
                foreach (CustomBuffInfoStat stat in buffInfo.BuffStats)
                {
                    string temp = stat.Stat.Lang() + $": +{stat.Amount}";
                    icon.Hint += $"\n{temp}";
                }

                levelNum.Index = buffInfo.BuffLV + 1910;
            }
            else
            {
                GameScene.Game.ReceiveChat("找不到BUFF信息，请更新数据库".Lang(), MessageType.System);
            }
        }

        private void OnStageChange(ThrowingStage oldStage, ThrowingStage newStage)
        {
            RefreshCoinCount();



        }

        public void RefreshCoinCount()
        {
            if (LuckyCoinCount != null)
            {
                //更新剩余硬币数量
                LuckyCoinCount.Text = GameScene.Game.InventoryBox.Grid.Grid.Where(x =>
                    x.Item != null && x.Item.Info.Effect == ItemEffect.LuckyCoins).Sum(y => y.Item.Count).ToString();
            }
        }
        public void Reset()
        {
            angle = 0;
            dir = 1;
            Stage = ThrowingStage.Tbegin;
            //initialDistance = (float)CEnvir.Random.Next(30, 49) / 10;
            //if (null != Distance) Distance.Text = $"距离：{initialDistance:0.##}m";

            if (CoinCursor != null) CoinCursor.Visible = true;
            if (ThrowAni != null)
            {
                ThrowAni.AnimationStart = DateTime.MinValue;
                ThrowAni.Visible = false;
                ThrowAni.Animated = false;
            }

            if (ThrowSuccess != null)
            {
                ThrowSuccess.AnimationStart = DateTime.MinValue;
                ThrowSuccess.Visible = false;
                ThrowSuccess.Animated = false;
            }

            if (ThrowFail != null)
            {
                ThrowFail.AnimationStart = DateTime.MinValue;
                ThrowFail.Visible = false;
                ThrowFail.Animated = false;
            }
        }

        /// <summary>
        /// 过程
        /// </summary>
        public override void Process()
        {
            base.Process();

            //限制硬币移动的速度
            if (CEnvir.FPSCount < 70 || CEnvir.Now > ProcessTime)
            {
                switch (Stage)
                {
                    case ThrowingStage.Tbegin:
                        break;
                    case ThrowingStage.TDisChose:
                        {
                            CoinCursor.Visible = true;
                            if (CoinCursor.Location.X > 220) dir = -1;
                            else if (CoinCursor.Location.X < 0) dir = 1;

                            CoinCursor.Location = new Point(CoinCursor.Location.X + dir, 265);

                            selectedDistance = (float)CoinCursor.Location.X / 220 * 4 + 1.0;
                            break;
                        }
                    case ThrowingStage.TThrowing:
                        CoinCursor.Visible = false;  //硬币图标关闭
                        ThrowAni.Visible = true;   //开启投掷动画
                        ThrowAni.Animated = true;   //开启投掷动画
                        Stage = ThrowingStage.TIntermediate;
                        //todo 发包
                        CEnvir.Enqueue(new TossCoin
                        {
                            Angle = angle,
                            InitialDistance = initialDistance,
                            SelectedDistance = selectedDistance,
                            TossOption = (TossCoinOption)Config.ThrowSetting
                        });

                        break;
                    case ThrowingStage.TJudging:
                        if (IsOnTarget)
                        {
                            ThrowSuccess.Animated = true;
                            ThrowSuccess.Visible = true;
                        }
                        else
                        {
                            ThrowFail.Animated = true;
                            ThrowFail.Visible = true;
                        }
                        //Stage = ThrowingStage.TIntermediate;
                        break;
                }

                ProcessTime = CEnvir.Now.AddMilliseconds(16);
            }

        }
        /// <summary>
        /// 可见更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);
            if (Visible)
            {
                Stage = ThrowingStage.Tbegin;
                initialDistance = (float)CEnvir.Random.Next(30, 49) / 10;
                if (null != Distance) Distance.Text = $"距离".Lang() + $"：{initialDistance:0.##}m";

                RefreshCoinCount();
                Reset();
            }
            if (null != ThrowAni) ThrowAni.Visible = false;
            if (null != CoinCursor) CoinCursor.Visible = false;
        }
        /// <summary>
        /// 鼠标进入时
        /// </summary>
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            CoinCursor.Visible = true;
        }
        /// <summary>
        /// 鼠标离开时
        /// </summary>
        public override void OnMouseLeave()
        {
            base.OnMouseLeave();
            CoinCursor.Visible = false;
        }
        /// <summary>
        /// 鼠标移动时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (Stage != ThrowingStage.Tbegin) return;
            if (MouseInDialog)
            {
                CoinCursor.Location = new Point(e.X - DisplayArea.Left - 20, e.Y - DisplayArea.Top - 19);  //硬币光标的位置
                //更新角度
                if (IsInValidArea(e.Location))
                {
                    angle = (245 - (e.Y - DisplayArea.Top)) / 4;  //角度

                    if (angle < 0)
                        angle = 0;
                    if (angle > 55)
                        angle = 55;

                    Angle.Text = $"角度".Lang() + $"：{angle}" + "度".Lang();
                }
            }
        }
        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            if (IsInValidArea(e.Location))
            {
                switch (Stage)
                {
                    case ThrowingStage.Tbegin:
                        Stage = ThrowingStage.TDisChose;
                        break;
                    case ThrowingStage.TDisChose:
                        Stage = ThrowingStage.TThrowing;
                        break;
                    case ThrowingStage.TThrowing:
                        break;
                    case ThrowingStage.TJudging:
                        break;
                    case ThrowingStage.TDone:
                        Reset();
                        break;
                }
            }
        }
        /// <summary>
        /// 有效区域（点鼠标位置）
        /// </summary>
        /// <param name="mouseLocation"></param>
        /// <returns></returns>
        private bool IsInValidArea(Point mouseLocation)
        {
            Point relativePoint = new Point(mouseLocation.X - DisplayArea.Left, mouseLocation.Y - DisplayArea.Top);

            return relativePoint.X > 0 && relativePoint.X < 256 &&
                   relativePoint.Y >= 30 && relativePoint.Y <= 245;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Vowdsground != null)
                {
                    if (!Vowdsground.IsDisposed)
                        Vowdsground.Dispose();

                    Vowdsground = null;
                }

                if (CoinCursor != null)
                {
                    if (!CoinCursor.IsDisposed)
                        CoinCursor.Dispose();

                    CoinCursor = null;
                }

                if (Distance != null)
                {
                    if (!Distance.IsDisposed)
                        Distance.Dispose();

                    Distance = null;
                }

                if (Angle != null)
                {
                    if (!Angle.IsDisposed)
                        Angle.Dispose();

                    Angle = null;
                }

                if (FreeCount != null)
                {
                    if (!FreeCount.IsDisposed)
                        FreeCount.Dispose();

                    FreeCount = null;
                }

                if (LuckyCoin != null)
                {
                    if (!LuckyCoin.IsDisposed)
                        LuckyCoin.Dispose();

                    LuckyCoin = null;
                }

                if (LuckyCoinCount != null)
                {
                    if (!LuckyCoinCount.IsDisposed)
                        LuckyCoinCount.Dispose();

                    LuckyCoinCount = null;
                }

                if (ComBoBack != null)
                {
                    if (!ComBoBack.IsDisposed)
                        ComBoBack.Dispose();

                    ComBoBack = null;
                }

                if (CombThrowCount != null)
                {
                    if (!CombThrowCount.IsDisposed)
                        CombThrowCount.Dispose();

                    CombThrowCount = null;
                }
            }
        }

        #endregion

        /// <summary>
        /// 投币阶段
        /// </summary>
        public enum ThrowingStage
        {
            /// <summary>
            /// 开始选择角度阶段
            /// </summary>
            Tbegin,
            /// <summary>
            /// 开始选择距离阶段
            /// </summary>
            TDisChose,
            /// <summary>
            /// 开始投掷硬币阶段
            /// </summary>
            TThrowing,
            /// <summary>
            /// 判断中
            /// </summary>
            TJudging,
            /// <summary>
            /// 投掷完毕
            /// </summary>
            TDone,
            /// <summary>
            /// 间歇
            /// </summary>
            TIntermediate,
        }
    }
}


