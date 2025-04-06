using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Library;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using C = Library.Network.ClientPackets;
using Color = System.Drawing.Color;
using Font = System.Drawing.Font;
using Point = System.Drawing.Point;
using Rectangle = System.Drawing.Rectangle;


namespace Client.Scenes
{
    /// <summary>
    /// 选择角色界面
    /// </summary>
    public sealed class SelectScene : DXScene
    {
        public class CAnimation
        {
            public int BaseIndex;
            public int FrameCount;
            public bool Loop;
            public TimeSpan Delay;
            public bool Animated;
            public CAnimation(int index, int count, bool loop, TimeSpan delay, bool anmimated = true)
            {
                BaseIndex = index;
                FrameCount = count;
                Loop = loop;
                Delay = delay;
                Animated = anmimated;
            }
        };

        #region Properties
        //public DXConfigWindow ConfigBox;
        //public DXButton ConfigButton;  //设置按钮

        public SelectDialog SelectBox;
        public NewCharacterDialog CharacterBox;
        public bool NextScence;
        public int CharIndex;
        #endregion

        /// <summary>
        /// 选择角色的游戏场景
        /// </summary>
        /// <param name="size"></param>
        public SelectScene(Size size) : base(size)
        {
            #region _Configs
            //ConfigButton = new DXButton                  //设置按钮
            //{
            //    LibraryFile = LibraryFile.GameInter,
            //    Index = 116,
            //    Parent = this,
            //    Visible = false
            //};
            //ConfigButton.Location = new Point(Size.Width - ConfigButton.Size.Width - 10, 10);
            //ConfigButton.MouseClick += (o, e) => ConfigBox.Visible = !ConfigBox.Visible;

            //ConfigBox = new DXConfigWindow              //设置内容面板
            //{
            //    Parent = this,
            //    Visible = false,
            //    NetworkTab = { Enabled = false, TabButton = { Visible = false } },
            //};
            //ConfigBox.Location = new Point((Size.Width - ConfigBox.Size.Width) / 2, (Size.Height - ConfigBox.Size.Height) / 2);
            #endregion _Configs

            SelectBox = new SelectDialog
            {
                Parent = this
            };
            SelectBox.Location = new Point((Size.Width - SelectBox.Size.Width) / 2, (Size.Height - SelectBox.Size.Height) / 2);

            CharacterBox = new NewCharacterDialog
            {
                Parent = this,
            };
            CharacterBox.Location = new Point((Size.Width - CharacterBox.Size.Width) / 2, (Size.Height - CharacterBox.Size.Height) / 2);

            //foreach (DXWindow window in DXWindow.Windows)
            //{
            //    window.LoadSettings();
            //}
        }
        public override void Process()
        {
            base.Process();

            if (!SelectBox.StartGameAttempted && NextScence)
                NextSecene();
        }
        /// <summary>
        /// 下一个场景
        /// </summary>
        void NextSecene()
        {
            ActiveScene = new LoadScene(Config.GameSize) { CharacterIndex = CharIndex };
            NextScence = false;
            CharIndex = -1;
            Dispose();
        }
        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                //if (ConfigBox != null)
                //{
                //    if (!ConfigBox.IsDisposed)
                //        ConfigBox.Dispose();
                //    ConfigBox = null;
                //}

                //if (ConfigButton != null)
                //{
                //    if (!ConfigButton.IsDisposed)
                //        ConfigButton.Dispose();
                //    ConfigButton = null;
                //}

                if (SelectBox != null)
                {
                    if (!SelectBox.IsDisposed)
                    {
                        SelectBox.Dispose();
                    }
                    SelectBox = null;
                }
                if (CharacterBox != null)
                {
                    if (!CharacterBox.IsDisposed)
                    {
                        CharacterBox.Dispose();
                    }
                    CharacterBox = null;
                }

                NextScence = false;
                CharIndex = -1;
            }
        }

        #endregion


        public sealed class SelectDialog : DXImageControl
        {
            #region Properties

            public DXButton StartButton;
            public DXButton DeleteButton;
            public DXButton CreateButton;
            public DXButton ExitGameButton;
            public DXButton PrevPageButton;
            public DXButton NextPageButton;
            public DXLabel ConsignmentLabel;
            public DXButton CancelButton;

            public int CurrentPage;

            public List<SelectInfo> CharacterList;
            public CAnimation[] Animations;
            private DXAnimatedControl[] ChrAnimations = new DXAnimatedControl[2];

            public SelectInfo CurrSelCharacter;
            public DXControl CharacterPanel;
            public DXLabel NameLabel, LevelLabel, JobLabel;

            public bool CanStartGame => CurrSelCharacter != null;
            public bool StartGameAttempted, DeleteAttempted;

            //public override WindowType Type => WindowType.None;
            //public override bool CustomSize => false;
            //public override bool AutomaticVisibility => false;
            #endregion

            public SelectDialog()
            {
                Size = new Size(640, 480);
                //HasFooter = false;  //不显示页脚
                ////TitleLabel.Text = "创建角色";
                //HasTitle = false;  //字幕标题不显示
                //HasTopBorder = false; //不显示上边框
                //TitleLabel.Visible = false; //不显示标题
                //CloseButton.Visible = false; //不显示关闭按钮 
                Movable = false; //不能移动
                Visible = true;
                //Opacity = 0F;
                Index = 50;            //素材图片序号
                LibraryFile = LibraryFile.Interface1c145;   //素材调用库

                #region _BackGround
                //DXImageControl background = new DXImageControl             //人物创建背景图
                //{
                //    Index = 50,            //素材图片序号
                //    LibraryFile = LibraryFile.Interface1c145,   //素材调用库
                //    Parent = this,
                //};
                #endregion _BackGround

                #region _CharacterPanel 人物信息框
                CharacterPanel = new DXControl
                {
                    Parent = this,
                    Border = true,
                    DrawTexture = true,
                    Opacity = 0.4F,
                    Size = new Size(140, 70),
                    BackColour = Color.FromArgb(50, 50, 25),
                    BorderColour = Color.FromArgb(100, 100, 80),
                    Location = new Point(79, 108),
                    Visible = false,
                };
                //名字
                NameLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(90, 20),
                    ForeColour = Color.FromArgb(255, 200, 150),
                    Parent = CharacterPanel,
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    Location = new Point(50, 10),
                    IsControl = false,
                };

                DXLabel label = new DXLabel
                {
                    Parent = CharacterPanel,
                    Text = "名字".Lang(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(255, 200, 150),
                    IsControl = false,
                };
                label.Location = new Point(10, 10);

                //等级
                LevelLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(50, 20),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(150, 240, 150),
                    Parent = CharacterPanel,
                    Location = new Point(50, 30),
                    IsControl = false,
                };

                label = new DXLabel
                {
                    Parent = CharacterPanel,
                    Text = "等级".Lang(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(150, 240, 150),
                    IsControl = false,
                };
                label.Location = new Point(10, 30);

                //职业
                JobLabel = new DXLabel
                {
                    AutoSize = false,
                    Size = new Size(53, 20),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(255, 255, 175),
                    Parent = CharacterPanel,
                    Location = new Point(50, 50),
                    IsControl = false,
                };

                label = new DXLabel
                {
                    Parent = CharacterPanel,
                    Text = "职业".Lang(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(255, 255, 175),
                    IsControl = false,
                };
                label.Location = new Point(10, 50);
                #endregion _CharacterPanel

                ConsignmentLabel = new DXLabel
                {
                    Parent = this,
                    Text = "当前角色已经寄售".Lang(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(10F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(150, 240, 150),
                    IsControl = false,
                    Visible = false,
                };
                ConsignmentLabel.Location = new Point((Size.Width - ConsignmentLabel.Size.Width) / 2, (Size.Height - ConsignmentLabel.Size.Height) / 4);

                CancelButton = new DXButton
                {
                    LibraryFile = LibraryFile.GameInter,
                    Index = 3652,
                    Parent = this,
                    Visible = false,
                };
                CancelButton.Location = new Point((Size.Width - CancelButton.Size.Width) / 2, ConsignmentLabel.Location.Y + CancelButton.Size.Height + 10);
                CancelButton.MouseClick += (o, e) =>
                {
                    if (CurrSelCharacter != null && CurrSelCharacter.CharacterState == (int)CharacterState.Sell)
                    {
                        CEnvir.Enqueue(new C.CanelSellCharacter()
                        {
                            CharacterIndex = CurrSelCharacter.CharacterIndex,
                        });
                    }

                };

                #region _Buttons
                //结束游戏
                ExitGameButton = new DXButton
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 58,
                    Parent = this,
                };
                ExitGameButton.Location = new Point(28, 438);
                ExitGameButton.MouseClick += (o, e) => DoLogOff();
                ExitGameButton.MouseEnter += Button_MouseEnter;
                ExitGameButton.MouseLeave += Button_MouseLeave;

                //开始游戏
                StartButton = new DXButton    //开始游戏按钮
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 56,
                    Parent = this,
                };
                StartButton.Location = new Point(259, 49);
                StartButton.MouseClick += (o, e) => DoStartGame();
                StartButton.MouseEnter += Button_MouseEnter;
                StartButton.MouseLeave += Button_MouseLeave;

                //创建角色
                CreateButton = new DXButton  //创建人物
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 52,
                    Parent = this,
                };
                CreateButton.Location = new Point(440, 93);
                CreateButton.MouseClick += (o, e) =>
                {
                    if (CharacterList.Count < 2)
                    {
                        DoCreateCharacter();
                    }
                    else
                    {
                        DXMessageBox.Show("已经达到最大角色数，无法在创建".Lang(), "创建人物".Lang());
                    }
                };
                CreateButton.MouseEnter += Button_MouseEnter;
                CreateButton.MouseLeave += Button_MouseLeave;

                //删除角色
                DeleteButton = new DXButton  //删除角色
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 54,
                    Parent = this,
                };
                DeleteButton.Location = new Point(79, 243);
                DeleteButton.MouseClick += (o, e) => DoDeleteCharacter();
                DeleteButton.MouseEnter += Button_MouseEnter;
                DeleteButton.MouseLeave += Button_MouseLeave;

                //上一个预览角色 箭头
                PrevPageButton = new DXButton
                {
                    LibraryFile = LibraryFile.UI1,
                    Index = 1152,
                    Movable = false,
                    Parent = this,
                    Visible = false
                };
                PrevPageButton.Location = new Point(500, 400);
                PrevPageButton.MouseClick += (o, e) => DoPreviousPage();
                PrevPageButton.MouseEnter += (o, e) =>     //鼠标移动上去
                {
                    PrevPageButton.Index = 1151;
                };
                PrevPageButton.MouseLeave += (o, e) =>   //鼠标离开
                {
                    PrevPageButton.Index = CurrentPage == 0 ? 1152 : 1150;
                };

                //下一个预览角色 箭头
                NextPageButton = new DXButton
                {
                    LibraryFile = LibraryFile.UI1,
                    Index = 1153,
                    Parent = this,
                    Movable = false,
                    Visible = false
                };
                NextPageButton.Location = new Point(550, 400);
                NextPageButton.MouseClick += (o, e) => DoNextPage();
                NextPageButton.MouseEnter += (o, e) =>     //鼠标移动上去
                {
                    NextPageButton.Index = 1154;
                };
                NextPageButton.MouseLeave += (o, e) =>   //鼠标离开
                {
                    NextPageButton.Index = CurrentPage == 1 ? 1155 : 1153;
                };

                #endregion _Buttons

                #region _Animations

                Animations = new CAnimation[]
                {
                //////////////////////////小图 站立待选 静止///////////////////////////////////////
                new CAnimation(200,11,false,TimeSpan.FromMilliseconds(1320)),//战士男
                new CAnimation(500,11,false,TimeSpan.FromMilliseconds(1320)),//战士女
                new CAnimation(800,11,false,TimeSpan.FromMilliseconds(1320)),//法师男
                new CAnimation(1100,11,false,TimeSpan.FromMilliseconds(1320)),//法师女
                new CAnimation(1400,11,false,TimeSpan.FromMilliseconds(1320)),//道士男
                new CAnimation(1700,11,false,TimeSpan.FromMilliseconds(1320)),//道士女
                new CAnimation(2100,12,false,TimeSpan.FromMilliseconds(1440)),//刺客男
                new CAnimation(2400,11,false,TimeSpan.FromMilliseconds(1320)),//刺客女
                ///////////////////////////小图 选中 动画///////////////////////////////////////
                new CAnimation(260,20,false,TimeSpan.FromMilliseconds(2400)),//战士男
                new CAnimation(560,11,false,TimeSpan.FromMilliseconds(1320)),//战士女
                new CAnimation(860,12,false,TimeSpan.FromMilliseconds(1440)),//法师男
                new CAnimation(1160,18,false,TimeSpan.FromMilliseconds(2160)),//法师女
                new CAnimation(1460,17,false,TimeSpan.FromMilliseconds(2040)),//道士男
                new CAnimation(1760,17,false,TimeSpan.FromMilliseconds(2040)),//道士女
                new CAnimation(2160,20,false,TimeSpan.FromMilliseconds(2400)),//刺客男
                new CAnimation(2460,17,false,TimeSpan.FromMilliseconds(2040)),//刺客女
                ///////////////////////////小图 选中 静止//////////////////////////////////
                new CAnimation(320,12,true,TimeSpan.FromMilliseconds(1440)),//战士男
                new CAnimation(620,11,true,TimeSpan.FromMilliseconds(1320)),//战士女
                new CAnimation(920,11,true,TimeSpan.FromMilliseconds(1320)),//法师男
                new CAnimation(1220,11,true,TimeSpan.FromMilliseconds(1320)),//法师女
                new CAnimation(1520,20,true,TimeSpan.FromMilliseconds(2400)),//道士男
                new CAnimation(1820,11,true,TimeSpan.FromMilliseconds(1320)),//道士女
                new CAnimation(2220,11,true,TimeSpan.FromMilliseconds(1320)),//刺客男
                new CAnimation(2520,13,true,TimeSpan.FromMilliseconds(1560)),//刺客女
                ///////////////////////////小图 收刀 动画//////////////////////////////////
                new CAnimation(380,8,false,TimeSpan.FromMilliseconds(960)),//战士男
                new CAnimation(680,12,false,TimeSpan.FromMilliseconds(1440)),//战士女
                new CAnimation(980,11,false,TimeSpan.FromMilliseconds(1320)),//法师男
                new CAnimation(1280,9,false,TimeSpan.FromMilliseconds(1080)),//法师女
                new CAnimation(1580,12,false,TimeSpan.FromMilliseconds(1440)),//道士男
                new CAnimation(1880,10,false,TimeSpan.FromMilliseconds(1200)),//道士女
                new CAnimation(2280,11,false,TimeSpan.FromMilliseconds(1320)),//刺客男
                new CAnimation(2580,11,false,TimeSpan.FromMilliseconds(1320)),//刺客女
                };

                Point[] locations =
                {
                new Point(250,210), //选中的角色位置
                new Point(350,250),  //第二个角色位置
            };
                for (int i = 0; i < 2; i++)
                {
                    ChrAnimations[i] = new DXAnimatedControl
                    {
                        Parent = this,
                        LibraryFile = LibraryFile.Interface1c145, //动画调用的素材库
                        Animated = false, //动画关闭
                        AnimationDelay = TimeSpan.FromSeconds(10), //动画时间 10秒
                        FrameCount = 100, //动画帧数 100帧
                        UseOffSet = true, //使用偏移量
                        Visible = false,
                        Location = locations[i],
                    };
                }
                ChrAnimations[0].MouseClick += (o, e) => SelectCharacter(0);
                ChrAnimations[0].BeforeDraw += (o, e) => Animation_BeforeDraw(ChrAnimations[0]);
                ChrAnimations[0].AfterDraw += (o, e) => Animation_AfterDraw(ChrAnimations[0]);
                ChrAnimations[1].MouseClick += (o, e) => SelectCharacter(1);
                ChrAnimations[1].BeforeDraw += (o, e) => Animation_BeforeDraw(ChrAnimations[1]);
                ChrAnimations[1].AfterDraw += (o, e) => Animation_AfterDraw(ChrAnimations[1]);
                #endregion _Animations


                CurrentPage = 0;
            }

            #region Methods
            /// <summary>
            /// 选择角色
            /// </summary>
            /// <param name="index"></param>
            public void SelectCharacter(int index)
            {
                if (index + CurrentPage * 2 >= CharacterList.Count) return;
                //选了自己，返回
                if (CurrSelCharacter == CharacterList[index + CurrentPage * 2]) return;

                //
                for (int i = 0; i < CharacterList.Count; i++)
                {
                    if (i != (index + CurrentPage * 2) && CurrSelCharacter == CharacterList[i])
                    {
                        if (i % 2 == index) continue;
                        UnSelectedAnimation(ChrAnimations[i % 2], CharacterList[i]);
                    }
                }

                CurrSelCharacter = CharacterList[index + CurrentPage * 2];
                SelectedAnimation(ChrAnimations[index], CurrSelCharacter);
                UpdateCharacterPanel();

            }
            /// <summary>
            /// 站立待选动画
            /// </summary>
            /// <param name="ChrAnimation"></param>
            /// <param name="info"></param>
            public void ToBeSelectAnimation(DXAnimatedControl ChrAnimation, SelectInfo info)
            {
                if (info == null)
                {
                    ChrAnimation.Visible = false;
                    return;
                }

                int idx = (int)info.Class * 2 + (int)info.Gender;
                if (idx >= Animations.Length) return;

                CAnimation actualAnimation = Animations[idx];

                ChrAnimation.Loop = true;
                ChrAnimation.AnimationStart = DateTime.MinValue;
                ChrAnimation.Animated = true;
                ChrAnimation.Visible = true;
                ChrAnimation.ForeColour = Color.DimGray;
                ChrAnimation.GrayScale = true;
                ChrAnimation.AnimationDelay = actualAnimation.Delay;
                ChrAnimation.BaseIndex = actualAnimation.BaseIndex;
                ChrAnimation.FrameCount = actualAnimation.FrameCount;

            }
            /// <summary>
            /// 选中动画
            /// </summary>
            /// <param name="ChrAnimation"></param>
            /// <param name="info"></param>
            public void SelectedAnimation(DXAnimatedControl ChrAnimation, SelectInfo info)
            {
                if (info == null)
                {
                    ChrAnimation.Visible = false;
                    return;
                }

                int idx = (int)info.Class * 2 + (int)info.Gender + 8;
                if (idx >= Animations.Length) return;

                CAnimation actualAnimation = Animations[idx];

                ChrAnimation.Loop = false;
                ChrAnimation.AnimationStart = DateTime.MinValue;
                ChrAnimation.Animated = true;
                ChrAnimation.Visible = true;
                ChrAnimation.ForeColour = Color.White;
                ChrAnimation.GrayScale = false;
                ChrAnimation.AnimationDelay = actualAnimation.Delay;
                ChrAnimation.BaseIndex = actualAnimation.BaseIndex;
                ChrAnimation.FrameCount = actualAnimation.FrameCount;
                ChrAnimation.AfterAnimation += (o, e) =>
                {
                    int idx = (int)info.Class * 2 + (int)info.Gender + 16;
                    if (idx >= Animations.Length) return;

                    CAnimation actualAnimation = Animations[idx];

                    ChrAnimation.AnimationDelay = actualAnimation.Delay;
                    ChrAnimation.Loop = true;
                    ChrAnimation.AnimationStart = DateTime.MinValue;
                    ChrAnimation.Animated = true;
                    ChrAnimation.BaseIndex = actualAnimation.BaseIndex;
                    ChrAnimation.FrameCount = actualAnimation.FrameCount;
                    ChrAnimation.ForeColour = Color.White;
                    ChrAnimation.GrayScale = false;
                };
                switch (info.Class)
                {
                    case MirClass.Warrior:
                        JobLabel.Text = "战士".Lang();
                        switch (info.Gender)
                        {
                            case MirGender.Male://男
                                DXSoundManager.Play(SoundIndex.SelectWarriorMale145);
                                break;
                            case MirGender.Female://女
                                DXSoundManager.Play(SoundIndex.SelectWarriorFemale145);
                                break;
                        }
                        break;
                    case MirClass.Wizard:
                        JobLabel.Text = "法师".Lang();
                        switch (info.Gender)
                        {
                            case MirGender.Male://男
                                DXSoundManager.Play(SoundIndex.SelectWizardMale145);
                                break;
                            case MirGender.Female://女
                                DXSoundManager.Play(SoundIndex.SelectWizardFemale145);
                                break;
                        }
                        break;
                    case MirClass.Taoist:
                        JobLabel.Text = "道士".Lang();
                        switch (info.Gender)
                        {
                            case MirGender.Male://男
                                DXSoundManager.Play(SoundIndex.SelectTaoistMale145);
                                break;
                            case MirGender.Female://女
                                DXSoundManager.Play(SoundIndex.SelectTaoistFemale145);
                                break;
                        }
                        break;
                    case MirClass.Assassin:
                        JobLabel.Text = "刺客".Lang();
                        switch (info.Gender)
                        {
                            case MirGender.Male://男
                                DXSoundManager.Play(SoundIndex.SelectAssassinMale);
                                break;
                            case MirGender.Female://女
                                DXSoundManager.Play(SoundIndex.SelectAssassinFemale);
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            /// <summary>
            /// 反选收到动画
            /// </summary>
            /// <param name="ChrAnimation"></param>
            /// <param name="info"></param>
            public void UnSelectedAnimation(DXAnimatedControl ChrAnimation, SelectInfo info)
            {
                if (info == null)
                {
                    ChrAnimation.Visible = false;
                    return;
                }

                int idx = (int)info.Class * 2 + (int)info.Gender + 24;
                if (idx >= Animations.Length) return;
                CAnimation actualAnimation = Animations[idx];

                ChrAnimation.Loop = false;
                ChrAnimation.AnimationStart = DateTime.MinValue;
                ChrAnimation.Animated = true;
                ChrAnimation.Visible = true;
                ChrAnimation.AnimationDelay = actualAnimation.Delay;
                ChrAnimation.BaseIndex = actualAnimation.BaseIndex;
                ChrAnimation.FrameCount = actualAnimation.FrameCount;
                ChrAnimation.AfterAnimation += (o, e) =>
                {
                    int idx = (int)info.Class * 2 + (int)info.Gender;
                    if (idx >= Animations.Length) return;
                    CAnimation actualAnimation = Animations[idx];

                    ChrAnimation.AnimationDelay = actualAnimation.Delay;
                    ChrAnimation.Loop = true;
                    ChrAnimation.AnimationStart = DateTime.MinValue;
                    ChrAnimation.Animated = true;
                    ChrAnimation.BaseIndex = actualAnimation.BaseIndex;
                    ChrAnimation.FrameCount = actualAnimation.FrameCount;
                    ChrAnimation.ForeColour = Color.DimGray;
                    ChrAnimation.GrayScale = true;
                };
            }
            /// <summary>
            /// 更新角色
            /// </summary>
            public void UpdateCharacters()
            {
                if (CharacterList == null) return;

                //如果删除了角色  导致页码 * 2个角色 超过了 角色数量  回退页码
                if (CharacterList.Count != 0 && CurrentPage * 2 >= CharacterList.Count)
                    CurrentPage = (CharacterList.Count / 2) + (CharacterList.Count % 2) - 1;

                //更新翻页图标
                if (CurrentPage <= 0)
                {
                    PrevPageButton.Index = 1152;
                    NextPageButton.Index = 1153;
                }
                else if (CurrentPage == (CharacterList.Count / 2) + (CharacterList.Count % 2) - 1)
                {
                    PrevPageButton.Index = 1150;
                    NextPageButton.Index = 1155;
                }
                PrevPageButton.Visible = CharacterList.Count > 2;
                NextPageButton.Visible = CharacterList.Count > 2;

                ChrAnimations[0].Visible = false;
                ChrAnimations[1].Visible = false;
                CurrSelCharacter = ((CharacterList.Count == 0) ? null : CharacterList[CurrentPage * 2]);

                UpdateCharacterPanel();

                if (CurrSelCharacter != null)
                {
                    if (CharacterList.Count == 1)
                    {
                        CurrSelCharacter = CharacterList[0];
                        SelectedAnimation(ChrAnimations[0], CurrSelCharacter);
                        ChrAnimations[0].Location = new Point(300, 210);
                        ChrAnimations[1].Visible = false;
                    }
                    else
                    {
                        SelectedAnimation(ChrAnimations[0], CurrSelCharacter);
                        ChrAnimations[0].Location = new Point(250, 210);
                        if (1 + CurrentPage * 2 < CharacterList.Count)
                            ToBeSelectAnimation(ChrAnimations[1], CharacterList[1 + CurrentPage * 2]);
                    }
                }
            }
            /// <summary>
            /// 更新角色
            /// </summary>
            public void UpdateCharacterPanel()
            {
                if (CurrSelCharacter == null)
                {
                    CharacterPanel.Visible = false;
                    return;
                }

                //显示人物信息框
                CharacterPanel.Visible = true;

                NameLabel.Text = CurrSelCharacter.CharacterName;
                LevelLabel.Text = CurrSelCharacter.Level.ToString();
                ConsignmentLabel.Visible = false;
                CancelButton.Visible = false;
                if (CurrSelCharacter.CharacterState == (int)CharacterState.Sell)
                {
                    ConsignmentLabel.Visible = true;
                    CancelButton.Visible = true;
                }
            }
            private void Button_MouseEnter(object sender, EventArgs e)
            {
                DXButton btn = sender as DXButton;
                if (btn == null) return;

                btn.BorderColour = Color.FromArgb(198, 150, 99);
                btn.BackColour = Color.FromArgb(150, 100, 50);
                btn.DrawTexture = true;
                btn.Border = true;
            }

            private void Button_MouseLeave(object sender, EventArgs e)
            {
                DXButton btn = sender as DXButton;
                if (btn == null) return;

                btn.BackColour = Color.Empty;
                btn.DrawTexture = false;
                btn.Border = false;
            }
            /// <summary>
            /// 退出
            /// </summary>
            public void DoLogOff()
            {
                CEnvir.Enqueue(new C.Logout());
            }
            /// <summary>
            /// 开始游戏
            /// </summary>
            public void DoStartGame()
            {
                if (StartGameAttempted) return;

                if (CurrSelCharacter != null)
                {
                    StartGameAttempted = true;

                    CEnvir.Enqueue(new C.RequestStartGame
                    {
                        CharacterIndex = CurrSelCharacter.CharacterIndex,
                        ClientMACInfo = CEnvir.MacInfo,
                        ClientCPUInfo = CEnvir.CpuInfo,
                        ClientHDDInfo = CEnvir.HDDnfo,
                    });
                }
            }

            /// <summary>
            /// 进入角色创建页面
            /// </summary>
            public void DoCreateCharacter()
            {
                SelectScene scene = ActiveScene as SelectScene;
                if (scene == null) return;
                if (scene.CharacterBox.IsVisible) return;

                DXAnimatedControl create = new DXAnimatedControl
                {
                    LibraryFile = LibraryFile.Wemade,
                    AnimationDelay = TimeSpan.FromMilliseconds(1300),
                    Parent = this,
                    Location = new Point(0, 0),
                    Loop = false,
                    AnimationStart = DateTime.MinValue,
                    Visible = true,
                    BaseIndex = 400,
                    FrameCount = 40,
                };
                DXSoundManager.StopAllSounds();
                DXSoundManager.Play(SoundIndex.CreateChr145);
                create.AfterAnimation += (o, e) =>
                {
                    Visible = false;
                    create.Visible = false;
                    scene.CharacterBox.Visible = true;
                    scene.CharacterBox.CharacterNameTextBox.SetFocus();
                    DXSoundManager.Play(SoundIndex.CreateChrBgm145);
                };
            }
            /// <summary>
            /// 删除角色
            /// </summary>
            public void DoDeleteCharacter()
            {
                if (DeleteAttempted) return;
                if (CurrSelCharacter == null) return;

                DeleteAttempted = true;
                DateTime deleteTime = CEnvir.Now.AddSeconds(5);

                DXMessageBox box = new DXMessageBox($"SelectScene.DeleteCharacter".Lang(CurrSelCharacter.CharacterName, (deleteTime - CEnvir.Now).TotalSeconds.ToString("0.0")), "确认删除角色".Lang(), DXMessageBoxButtons.YesNo);

                box.YesButton.MouseClick += (o, e1) => CEnvir.Enqueue(new C.DeleteCharacter { CharacterIndex = CurrSelCharacter.CharacterIndex, CheckSum = CEnvir.C, });
                box.YesButton.Enabled = false;

                box.ProcessAction = () =>
                {
                    if (CEnvir.Now > deleteTime)
                    {
                        box.Label.Text = $"SelectScene.DeleteCharacterName".Lang(CurrSelCharacter.CharacterName);
                        box.YesButton.Enabled = true;
                        box.ProcessAction = null;
                    }
                    else
                        box.Label.Text = $"SelectScene.DeleteCharacter".Lang(CurrSelCharacter.CharacterName, (deleteTime - CEnvir.Now).TotalSeconds.ToString("0.0"));
                };
            }
            /// <summary>
            /// 显示头2个角色
            /// </summary>
            public void DoPreviousPage()
            {
                if (CurrentPage <= 0) return;

                CurrentPage -= 1;

                UpdateCharacters();
            }
            /// <summary>
            /// 下一页
            /// </summary>
            public void DoNextPage()
            {
                if (CurrentPage == (CharacterList.Count / 2) + (CharacterList.Count % 2) - 1) return;

                CurrentPage += 1;

                UpdateCharacters();
            }


            /// <summary>
            /// 角色动画 绘制后
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Animation_AfterDraw(DXAnimatedControl ChrAnimation)
            {
                DXManager.SetBlend(true);

                MirImage image = ChrAnimation.Library?.GetImage(ChrAnimation.Index);
                if (image == null) return;

                int x = ChrAnimation.DisplayArea.X - image.OffSetX;
                int y = ChrAnimation.DisplayArea.Y - image.OffSetY;

                image = ChrAnimation.Library?.CreateImage(ChrAnimation.Index + 40, ImageType.Image);
                if (image != null)
                    PresentTexture(image.Image, ChrAnimation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.White, this);

                DXManager.SetBlend(false);
            }
            /// <summary>
            /// 绘制前画阴影
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void Animation_BeforeDraw(DXAnimatedControl ChrAnimation)
            {
                MirImage image = ChrAnimation.Library?.GetImage(ChrAnimation.Index);

                if (image == null) return;

                int x = ChrAnimation.DisplayArea.X - image.OffSetX;
                int y = ChrAnimation.DisplayArea.Y - image.OffSetY;

                image = ChrAnimation.Library?.CreateImage(ChrAnimation.Index + 20, ImageType.Image);
                if (image?.Image != null)
                    PresentTexture(image.Image, ChrAnimation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.FromArgb(180, 255, 255, 255), this);

            }

            public override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);

                if (e.Handled) return;

                //foreach (KeyBindAction action in CEnvir.GetKeyAction(e.KeyCode))
                //{
                //    switch (action)
                //    {
                //        case KeyBindAction.ConfigWindow:
                //            ConfigBox.Visible = !ConfigBox.Visible;
                //            break;
                //        default:
                //            continue;
                //    }

                //    e.Handled = true;
                //    return;
                //}
                if (e.KeyCode == Keys.PageUp || e.KeyCode == Keys.Up || e.KeyCode == Keys.Left)
                {
                    DoPreviousPage();
                }
                else if (e.KeyCode == Keys.PageDown || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right)
                {
                    DoNextPage();
                }
                if (e.KeyCode == Keys.Enter)
                {
                    DoStartGame();
                }
            }
            #endregion

            #region IDisposable

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    if (CharacterPanel != null)
                    {
                        if (!CharacterPanel.IsDisposed)
                        {
                            CharacterPanel.Dispose();
                        }
                        CharacterPanel = null;
                    }
                    if (NameLabel != null)
                    {
                        if (!NameLabel.IsDisposed)
                        {
                            NameLabel.Dispose();
                        }
                        NameLabel = null;
                    }
                    if (LevelLabel != null)
                    {
                        if (!LevelLabel.IsDisposed)
                        {
                            LevelLabel.Dispose();
                        }
                        LevelLabel = null;
                    }
                    if (JobLabel != null)
                    {
                        if (!JobLabel.IsDisposed)
                        {
                            JobLabel.Dispose();
                        }
                        JobLabel = null;
                    }

                    if (ConsignmentLabel != null)
                    {
                        if (!ConsignmentLabel.IsDisposed)
                        {
                            ConsignmentLabel.Dispose();
                        }
                        ConsignmentLabel = null;
                    }

                    if (CancelButton != null)
                    {
                        if (!CancelButton.IsDisposed)
                        {
                            CancelButton.Dispose();
                        }
                        CancelButton = null;
                    }

                    if (StartButton != null)
                    {
                        if (!StartButton.IsDisposed)
                        {
                            StartButton.Dispose();
                        }
                        StartButton = null;
                    }
                    if (CreateButton != null)
                    {
                        if (!CreateButton.IsDisposed)
                        {
                            CreateButton.Dispose();
                        }
                        CreateButton = null;
                    }
                    if (DeleteButton != null)
                    {
                        if (!DeleteButton.IsDisposed)
                        {
                            DeleteButton.Dispose();
                        }
                        DeleteButton = null;
                    }
                    if (ExitGameButton != null)
                    {
                        if (!ExitGameButton.IsDisposed)
                        {
                            ExitGameButton.Dispose();
                        }
                        ExitGameButton = null;
                    }
                    if (PrevPageButton != null)
                    {
                        if (!PrevPageButton.IsDisposed)
                        {
                            PrevPageButton.Dispose();
                        }
                        PrevPageButton = null;
                    }
                    if (NextPageButton != null)
                    {
                        if (!NextPageButton.IsDisposed)
                        {
                            NextPageButton.Dispose();
                        }
                        NextPageButton = null;
                    }
                    for (int i = 0; i < ChrAnimations.Length; i++)
                    {
                        if (ChrAnimations[i] != null)
                        {
                            if (!ChrAnimations[i].IsDisposed)
                            {
                                ChrAnimations[i].Dispose();
                            }
                            ChrAnimations[i] = null;
                        }
                    }
                    Animations = null;
                    ChrAnimations = null;
                    CharacterList.Clear();
                    CharacterList = null;
                    CurrSelCharacter = null;
                    CurrentPage = 0;
                }
            }

            #endregion
        }
        /// <summary>
        /// 新建角色界面
        /// </summary>
        public sealed class NewCharacterDialog : DXImageControl
        {
            #region Properties

            #region SelectedClass
            /// <summary>
            /// 选择职业
            /// </summary>
            public MirClass SelectedClass
            {
                get => _SelectedClass;
                set
                {
                    if (_SelectedClass == value) return;

                    MirClass oldValue = _SelectedClass;
                    _SelectedClass = value;

                    OnSelectedClassChanged(oldValue, value);
                }
            }
            private MirClass _SelectedClass = MirClass.Warrior;
            public event EventHandler<EventArgs> SelectedClassChanged;
            public void OnSelectedClassChanged(MirClass oValue, MirClass nValue)
            {
                SelectedClassChanged?.Invoke(this, EventArgs.Empty);

                switch (oValue)
                {
                    case MirClass.Warrior:
                        WarriorButton.Index = 91;
                        break;
                    case MirClass.Wizard:
                        WizardButton.Index = 94;
                        break;
                    case MirClass.Taoist:
                        TaoistButton.Index = 97;
                        break;
                    case MirClass.Assassin:
                        AssassinButton.Index = 100;
                        break;
                }

                _SelectedGender = MirGender.Male;
                UpdateCharacterDisplay();

            }

            #endregion

            #region SelectedGender
            /// <summary>
            /// 选择性别
            /// </summary>
            public MirGender SelectedGender
            {
                get => _SelectedGender;
                set
                {
                    if (_SelectedGender == value) return;

                    MirGender oldValue = _SelectedGender;
                    _SelectedGender = value;

                    OnSelectedGenderChanged(oldValue, value);
                }
            }
            private MirGender _SelectedGender = MirGender.Male;
            public event EventHandler<EventArgs> SelectedGenderChanged;
            public void OnSelectedGenderChanged(MirGender oValue, MirGender nValue)
            {
                SelectedGenderChanged?.Invoke(this, EventArgs.Empty);

                UpdateCharacterDisplay();
            }

            #endregion

            #region CharacterNameValid
            /// <summary>
            /// 角色名字是否有效
            /// </summary>
            public bool CharacterNameValid
            {
                get => _CharacterNameValid;
                set
                {
                    if (_CharacterNameValid == value) return;

                    bool oldValue = _CharacterNameValid;
                    _CharacterNameValid = value;

                    OnCharacterNameValidChanged(oldValue, value);
                }
            }
            private bool _CharacterNameValid;
            public event EventHandler<EventArgs> CharacterNameValidChanged;
            public void OnCharacterNameValidChanged(bool oValue, bool nValue)
            {
                CharacterNameValidChanged?.Invoke(this, EventArgs.Empty);
                CreateNewCharacterButton.Enabled = CanCreate;
            }
            #endregion

            #region CreateAttempted
            /// <summary>
            /// 尝试创建角色
            /// </summary>
            public bool CreateAttempted
            {
                get => _CreateAttempted;
                set
                {
                    if (_CreateAttempted == value) return;

                    bool oldValue = _CreateAttempted;
                    _CreateAttempted = value;

                    OnCreateAttemptedChanged(oldValue, value);
                }
            }
            private bool _CreateAttempted;
            public event EventHandler<EventArgs> CreateAttemptedChanged;
            public void OnCreateAttemptedChanged(bool oValue, bool nValue)
            {
                CreateAttemptedChanged?.Invoke(this, EventArgs.Empty);
                CreateNewCharacterButton.Enabled = CanCreate;
            }
            #endregion

            #region variables
            public bool CanCreate => !CreateAttempted && CharacterNameValid;

            public DXImageControl BackImage;
            public DXLabel CharacterNameHelpLabel;
            public DXTextBox CharacterNameTextBox;

            public DXButton CreateNewCharacterButton, CloseNewCharacterButton;

            public DXButton WarriorButton, WizardButton, TaoistButton, AssassinButton;

            public DXLabel ClassNameLabel;
            public DXLabel ClassDetail;

            private DXAnimatedControl MaleChar, FemaleChar;
            private CAnimation[] Animations;
            private Point[] AnimationPoints;
            //public override WindowType Type => WindowType.None;
            //public override bool CustomSize => false;
            //public override bool AutomaticVisibility => false;
            #endregion

            #endregion

            /// <summary>
            /// 新建角色
            /// </summary>
            public NewCharacterDialog()
            {
                Size = new Size(640, 480);
                //HasFooter = false;  //不显示页脚
                ////TitleLabel.Text = "创建角色";
                //HasTitle = false;  //字幕标题不显示
                //HasTopBorder = false; //不显示上边框
                //TitleLabel.Visible = false; //不显示标题
                //CloseButton.Visible = false; //不显示关闭按钮 
                Movable = false; //不能移动
                Visible = false;
                //Opacity = 0F;
                LibraryFile = LibraryFile.Interface1c145;
                Index = 80;

                //BackImage = new DXImageControl  //背景图
                //{
                //    LibraryFile = LibraryFile.Interface1c145,
                //    Index = 80,
                //    Parent = this,
                //    Location = new Point(0, 0),
                //};

                #region 动画
                AnimationPoints = new Point[]
                {
                    new Point(110, 110),//战士男
                    new Point(400, 160),//战士女
                    new Point(110, 120),//法师男
                    new Point(420, 115),//法师女
                    new Point(110, 120),//道士男
                    new Point(425, 118),//道士女
                    //new Point(110, 120),//刺客男
                    //new Point(420, 118),//刺客女
                    new Point(130, 285),//刺客男
                    new Point(440, 290),//刺客女
                };
                Animations = new CAnimation[]
                {
                new CAnimation(440,18,false,TimeSpan.FromMilliseconds(2160)), //战士男
                new CAnimation(740,16,false,TimeSpan.FromMilliseconds(1920)), //战士女
                new CAnimation(1040,15,false,TimeSpan.FromMilliseconds(1800)),//法师男
                new CAnimation(1340,17,false,TimeSpan.FromMilliseconds(2040)),//法师女
                new CAnimation(1640,17,false,TimeSpan.FromMilliseconds(2040)),//道士男
                new CAnimation(1940,15,false,TimeSpan.FromMilliseconds(1800)),//道士女
                new CAnimation(2340,18,false,TimeSpan.FromMilliseconds(2160)),//刺客男
                new CAnimation(2640,20,false,TimeSpan.FromMilliseconds(2400)),//刺客女
                };

                MaleChar = new DXAnimatedControl
                {
                    BaseIndex = 440,
                    LibraryFile = LibraryFile.Interface1c145,
                    AnimationDelay = TimeSpan.FromMilliseconds(3000.0),
                    FrameCount = 18,
                    Parent = this,
                    UseOffSet = true,
                    Visible = true,
                    Location = new Point(110, 110)
                };
                MaleChar.MouseClick += (o, e) =>
                {
                    SelectedGender = MirGender.Male;
                };
                MaleChar.BeforeDraw += CharacterCreate_BeforeDraw;
                MaleChar.AfterDraw += CharacterCreate_AfterDraw;
                MaleChar.IndexChanged += (o, e) => UpdateSelectedCharSound();
                FemaleChar = new DXAnimatedControl
                {
                    BaseIndex = 740,
                    LibraryFile = LibraryFile.Interface1c145,
                    AnimationDelay = TimeSpan.FromMilliseconds(2000.0),
                    FrameCount = 1,
                    ForeColour = Color.DimGray,
                    GrayScale = true,
                    Visible = true,
                    Parent = this,
                    UseOffSet = true,
                    Location = new Point(400, 160)
                };
                FemaleChar.MouseClick += (o, e) =>
                {
                    SelectedGender = MirGender.Female;
                };
                FemaleChar.BeforeDraw += CharacterCreate_BeforeDraw;
                FemaleChar.AfterDraw += CharacterCreate_AfterDraw;
                FemaleChar.IndexChanged += (o, e) => UpdateSelectedCharSound();
                #endregion

                #region 职业描述

                DXControl parent = new DXControl
                {
                    Parent = this,
                    Border = true,
                    DrawTexture = true,
                    Opacity = 0.2F,
                    Size = new Size(480, 100),
                    BorderColour = Color.Gray,
                    BackColour = Color.FromArgb(150, 150, 150),
                    IsControl = false,
                    BorderSize = 1.5F,
                    Location = new Point(70, 15),
                    Visible = true,
                };

                ClassNameLabel = new DXLabel
                {
                    Parent = parent,
                    Text = "[ " + "男战士".Lang() + " ]",
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Bold),
                    ForeColour = Color.FromArgb(250, 200, 150),
                    IsControl = false,
                };
                ClassNameLabel.Location = new Point(10, 10);

                ClassDetail = new DXLabel
                {
                    Parent = parent,
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Regular),
                    //DrawFormat = TextFormatFlags.WordBreak | TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
                    ForeColour = Color.FromArgb(250, 250, 255),
                    Size = new Size(430, 100),
                    IsControl = false,
                };
                ClassDetail.Location = new Point(10, 35);
                ClassDetail.Text = "ServerScene.NewWarriorDetails".Lang();

                #endregion

                #region 石头角色类型选择按钮

                DXImageControl CareerSpot = new DXImageControl  //选择职业背景阴影
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 82,
                    Parent = this,
                };
                CareerSpot.Location = new Point(201, 434);

                DXImageControl Career = new DXImageControl  //选择职业背景
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 81,
                    Parent = this,
                    IsControl = true,
                };
                Career.Location = new Point(247, 384);

                WarriorButton = new DXButton
                {
                    Index = 92,
                    LibraryFile = LibraryFile.Interface1c145,
                    Parent = Career,
                    Hint = "战士".Lang(),
                    Location = new Point(260 - 247, 420 - 384),
                };
                WarriorButton.MouseClick += (o, e) =>
                {
                    SelectedClass = MirClass.Warrior;
                };

                WizardButton = new DXButton
                {
                    Index = 95,
                    LibraryFile = LibraryFile.Interface1c145,
                    Hint = "法师".Lang(),
                    Parent = Career,
                    Location = new Point(296 - 247, 420 - 384),
                };
                WizardButton.MouseClick += (o, e) =>
                {
                    SelectedClass = MirClass.Wizard;
                };

                TaoistButton = new DXButton
                {
                    Index = 98,
                    LibraryFile = LibraryFile.Interface1c145,
                    Location = new Point(333 - 247, 420 - 384),
                    Hint = "道士".Lang(),
                    Parent = Career,
                };
                TaoistButton.MouseClick += (o, e) =>
                {
                    SelectedClass = MirClass.Taoist;
                };

                AssassinButton = new DXButton
                {
                    Index = 101,
                    LibraryFile = LibraryFile.Interface1c145,
                    Location = new Point(369 - 247, 420 - 384),
                    Hint = "暂未开放".Lang(),
                    Parent = Career,
                };
                //AssassinButton.MouseClick += (o, e) =>
                //{
                //    SelectedClass = MirClass.Assassin;
                //};

                CharacterNameTextBox = new DXTextBox
                {
                    Location = new Point(287 - 247, 404 - 384),
                    Parent = Career,
                    BackColour = Color.Empty,
                    Border = false,
                    Size = new Size(75, 15),
                };
                CharacterNameTextBox.TextBox.TextChanged += CharacterNameTextBox_TextChanged;
                CharacterNameTextBox.TextBox.GotFocus += (o, e) => CharacterNameHelpLabel.Visible = true;
                CharacterNameTextBox.TextBox.LostFocus += (o, e) => CharacterNameHelpLabel.Visible = false;
                CharacterNameTextBox.TextBox.KeyPress += TextBox_KeyPress;

                CharacterNameHelpLabel = new DXLabel
                {
                    Visible = false,
                    Parent = Career,
                    Text = "[?]",
                    Hint = $"SelectScene.CharacterNameHelpLabel".Lang(Globals.MinCharacterNameLength, Globals.MaxCharacterNameLength),
                };
                CharacterNameHelpLabel.Location = new Point(CharacterNameTextBox.Location.X + CharacterNameTextBox.Size.Width + 2, (CharacterNameTextBox.Size.Height - CharacterNameHelpLabel.Size.Height) / 2 + CharacterNameTextBox.Location.Y);

                #endregion

                CreateNewCharacterButton = new DXButton  //创建按钮
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 85,
                    Parent = this,
                    Location = new Point(450, 444),
                };
                CreateNewCharacterButton.MouseClick += (o, e) => Create();
                CreateNewCharacterButton.MouseEnter += (o, e) =>     //鼠标移动上去
                {
                    CreateNewCharacterButton.Index = 86;
                };
                CreateNewCharacterButton.MouseLeave += (o, e) =>   //鼠标离开
                {
                    CreateNewCharacterButton.Index = 85;
                };

                CloseNewCharacterButton = new DXButton  //关闭按钮
                {
                    LibraryFile = LibraryFile.Interface1c145,
                    Index = 88,
                    Parent = this,
                    Location = new Point(491, 444),
                };
                CloseNewCharacterButton.MouseClick += (o, e) => CancelCreate();  //关闭按钮
                CloseNewCharacterButton.MouseEnter += (o, e) =>     //鼠标移动上去
                {
                    CloseNewCharacterButton.Index = 89;
                };
                CloseNewCharacterButton.MouseLeave += (o, e) =>   //鼠标离开
                {
                    CloseNewCharacterButton.Index = 88;
                };

            }

            #region Method
            #region 更新动画以及说明文字
            /// <summary>
            /// 循环播放角色动画
            /// </summary>
            private void UpdateSelectedCharSound()
            {
                int idx = (int)SelectedClass * 2 + (int)SelectedGender;
                if (idx >= Animations.Length) return;

                //循环播放声效
                switch (SelectedGender)
                {
                    case MirGender.Male://男
                        if (MaleChar.Index == Animations[idx].BaseIndex + 1)
                        {
                            switch (SelectedClass)
                            {
                                case MirClass.Warrior:
                                    DXSoundManager.Play(SoundIndex.CreateWarriorMale145);
                                    break;
                                case MirClass.Wizard:
                                    DXSoundManager.Play(SoundIndex.CreateWizardMale145);
                                    break;
                                case MirClass.Taoist:
                                    DXSoundManager.Play(SoundIndex.CreateTaoistMale145);
                                    break;
                                case MirClass.Assassin:
                                    DXSoundManager.Play(SoundIndex.CreateWarriorMale145);
                                    break;
                            }
                        }
                        break;
                    case MirGender.Female://女
                        if (FemaleChar.Index == Animations[idx].BaseIndex)
                        {
                            switch (SelectedClass)
                            {
                                case MirClass.Warrior:
                                    DXSoundManager.Play(SoundIndex.CreateWarriorFemale145);
                                    break;
                                case MirClass.Wizard:
                                    DXSoundManager.Play(SoundIndex.CreateWizardFemale145);
                                    break;
                                case MirClass.Taoist:
                                    DXSoundManager.Play(SoundIndex.CreateTaoistFemale145);
                                    break;
                                case MirClass.Assassin:
                                    DXSoundManager.Play(SoundIndex.CreateWarriorFemale145);
                                    break;
                            }
                        }
                        break;
                }

            }
            /// <summary>
            /// 角色绘制前画阴影
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void CharacterCreate_BeforeDraw(object sender, EventArgs e)
            {
                DXAnimatedControl animation = sender as DXAnimatedControl;

                MirImage image = animation?.Library?.GetImage(animation.Index);
                if (image == null) return;

                int x = animation.DisplayArea.X - image.OffSetX;
                int y = animation.DisplayArea.Y - image.OffSetY;

                image = animation.Library?.CreateImage(animation.Index + 20, ImageType.Image);
                if (image?.Image != null)
                    PresentTexture(image.Image, animation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.FromArgb(180, 255, 255, 255), this);

            }
            /// <summary>
            /// 角色绘制后画技能动画
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void CharacterCreate_AfterDraw(object sender, EventArgs e)
            {
                DXManager.SetBlend(true);

                DXAnimatedControl animation = sender as DXAnimatedControl;

                MirImage image = animation.Library?.GetImage(animation.Index);

                if (image == null) return;

                int x = animation.DisplayArea.X - image.OffSetX;
                int y = animation.DisplayArea.Y - image.OffSetY;

                image = animation.Library?.CreateImage(animation.Index + 40, ImageType.Image);

                if (image != null)
                    PresentTexture(image.Image, animation.Parent, new Rectangle(x + image.OffSetX, y + image.OffSetY, image.Width, image.Height), Color.FromArgb(180, 255, 255, 255), this);

                DXManager.SetBlend(false);
            }
            /// <summary>
            /// 更新动画显示
            /// </summary>
            public void UpdateCharacterDisplay()
            {
                int idx = (int)SelectedClass * 2 + (int)SelectedGender;
                if (idx >= Animations.Length) return;

                //更新动画
                switch (SelectedGender)
                {
                    case MirGender.Male://男
                        CAnimation man = Animations[idx];
                        CAnimation woman = Animations[idx + 1];
                        MaleChar.AnimationStart = DateTime.MinValue;
                        MaleChar.AnimationDelay = man.Delay;
                        MaleChar.BaseIndex = man.BaseIndex;
                        MaleChar.FrameCount = man.FrameCount;
                        MaleChar.ForeColour = Color.White;
                        MaleChar.GrayScale = false;
                        MaleChar.Location = AnimationPoints[idx];
                        FemaleChar.AnimationStart = DateTime.MinValue;
                        FemaleChar.BaseIndex = woman.BaseIndex;
                        FemaleChar.FrameCount = 1;
                        FemaleChar.ForeColour = Color.DimGray;
                        FemaleChar.GrayScale = true;
                        FemaleChar.Location = AnimationPoints[idx + 1];
                        break;
                    case MirGender.Female://女
                        CAnimation man1 = Animations[idx - 1];
                        CAnimation woman1 = Animations[idx];
                        FemaleChar.AnimationStart = DateTime.MinValue;
                        FemaleChar.AnimationDelay = woman1.Delay;
                        FemaleChar.BaseIndex = woman1.BaseIndex;
                        FemaleChar.FrameCount = woman1.FrameCount;
                        FemaleChar.ForeColour = Color.White;
                        FemaleChar.GrayScale = false;
                        FemaleChar.Location = AnimationPoints[idx];
                        MaleChar.AnimationStart = DateTime.MinValue;
                        MaleChar.BaseIndex = man1.BaseIndex;
                        MaleChar.FrameCount = 1;
                        MaleChar.ForeColour = Color.DimGray;
                        MaleChar.GrayScale = true;
                        MaleChar.Location = AnimationPoints[idx - 1];
                        break;
                }

                //更新职业描述信息
                switch (SelectedClass)
                {
                    case MirClass.Warrior:
                        WarriorButton.Index = 93;
                        ClassNameLabel.Text = SelectedGender == MirGender.Male ? "[ " + "男战士".Lang() + " ]" : "[ " + "女战士".Lang() + " ]";
                        ClassNameLabel.ForeColour = Color.FromArgb(250, 200, 150);
                        ClassDetail.Text = "ServerScene.NewWarriorDetails".Lang();
                        break;
                    case MirClass.Wizard:
                        WizardButton.Index = 96;
                        ClassNameLabel.Text = SelectedGender == MirGender.Male ? "[ " + "男法师".Lang() + " ]" : "[ " + "女法师".Lang() + " ]";
                        ClassNameLabel.ForeColour = Color.FromArgb(250, 170, 170);
                        ClassDetail.Text = "ServerScene.NewWizardDetails".Lang();
                        break;
                    case MirClass.Taoist:
                        TaoistButton.Index = 99;
                        ClassNameLabel.Text = SelectedGender == MirGender.Male ? "[ " + "男道士".Lang() + " ]" : "[ " + "女道士".Lang() + " ]";
                        ClassNameLabel.ForeColour = Color.FromArgb(150, 220, 150);
                        ClassDetail.Text = "ServerScene.NewTaoistDetails".Lang();
                        break;
                    case MirClass.Assassin:
                        AssassinButton.Index = 102;
                        ClassNameLabel.Text = SelectedGender == MirGender.Male ? "[ " + "男刺客".Lang() + " ]" : "[ " + "女刺客".Lang() + " ]";
                        ClassNameLabel.ForeColour = Color.FromArgb(150, 220, 250);
                        ClassDetail.Text = "ServerScene.NewAssassinDetails".Lang();
                        break;
                }
            }

            #endregion
            /// <summary>
            /// 创建
            /// </summary>
            public void Create()
            {
                if (!CanCreate) return;

                CreateAttempted = true;

                C.NewCharacter p = new C.NewCharacter
                {
                    CharacterName = CharacterNameTextBox.TextBox.Text,
                    Class = SelectedClass,
                    Gender = SelectedGender,
                    HairType = 1,
                    HairColour = Color.FromArgb(CEnvir.Random.Next(256), CEnvir.Random.Next(256), CEnvir.Random.Next(256)),
                    ArmourColour = SelectedClass == MirClass.Assassin ? Color.Empty : Color.FromArgb(CEnvir.Random.Next(256), CEnvir.Random.Next(256), CEnvir.Random.Next(256)),
                    CheckSum = CEnvir.C,
                };
                CEnvir.Enqueue(p);
            }
            private void CancelCreate()
            {
                DXAnimatedControl back = new DXAnimatedControl
                {
                    LibraryFile = LibraryFile.Wemade,
                    AnimationDelay = TimeSpan.FromMilliseconds(1300),
                    Parent = this,
                    Location = new Point(0, 0),
                    Loop = false,
                    AnimationStart = DateTime.MinValue,
                    Visible = true,
                    BaseIndex = 400,
                    FrameCount = 40,
                    Reversed = true
                };
                DXSoundManager.StopAllSounds();
                DXSoundManager.Play(SoundIndex.SelChrRe145);
                back.AfterAnimation += (o, e) =>
                {
                    Close();
                    back.Visible = false;
                };
            }
            /// <summary>
            /// 关闭
            /// </summary>
            public void Close()
            {
                SelectScene scene = ActiveScene as SelectScene;
                if (scene == null) return;

                DXSoundManager.StopAllSounds();
                DXSoundManager.Play(SoundIndex.SelChrBgm145);

                SelectedClass = MirClass.Warrior;
                _SelectedGender = MirGender.Male;
                CharacterNameTextBox.TextBox.Text = string.Empty;
                UpdateCharacterDisplay();

                Visible = false;
                CreateAttempted = false;
                scene.SelectBox.Visible = true;
            }
            /// <summary>
            /// 文本框按键过程
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
            {
                if (e.KeyChar != (char)Keys.Enter) return;

                e.Handled = true;

                if (CreateNewCharacterButton.Enabled)
                    Create();
            }
            /// <summary>
            /// 新建角色名字内容变化时
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void CharacterNameTextBox_TextChanged(object sender, EventArgs e)
            {
                CharacterNameValid = Globals.CharacterReg.IsMatch(CharacterNameTextBox.TextBox.Text);

                if (string.IsNullOrEmpty(CharacterNameTextBox.TextBox.Text))
                    CharacterNameTextBox.BorderColour = Color.FromArgb(198, 166, 99);
                else
                    CharacterNameTextBox.BorderColour = CharacterNameValid ? Color.Green : Color.Red;

            }

            #endregion

            #region IDisposable
            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    _SelectedClass = MirClass.Warrior;
                    _SelectedGender = MirGender.Male;
                    _CharacterNameValid = false;
                    _CreateAttempted = false;

                    if (BackImage != null)
                    {
                        if (!BackImage.IsDisposed)
                            BackImage.Dispose();

                        BackImage = null;
                    }

                    if (CharacterNameHelpLabel != null)
                    {
                        if (!CharacterNameHelpLabel.IsDisposed)
                            CharacterNameHelpLabel.Dispose();

                        CharacterNameHelpLabel = null;
                    }

                    if (CharacterNameTextBox != null)
                    {
                        if (!CharacterNameTextBox.IsDisposed)
                            CharacterNameTextBox.Dispose();

                        CharacterNameTextBox = null;
                    }

                    //if (CharacterDisplay != null)
                    //{
                    //    if (!CharacterDisplay.IsDisposed)
                    //        CharacterDisplay.Dispose();

                    //    CharacterDisplay = null;
                    //}


                    if (CreateNewCharacterButton != null)
                    {
                        if (!CreateNewCharacterButton.IsDisposed)
                            CreateNewCharacterButton.Dispose();

                        CreateNewCharacterButton = null;
                    }

                    if (CloseNewCharacterButton != null)
                    {
                        if (!CloseNewCharacterButton.IsDisposed)
                            CloseNewCharacterButton.Dispose();

                        CloseNewCharacterButton = null;
                    }

                    if (WarriorButton != null)
                    {
                        if (!WarriorButton.IsDisposed)
                            WarriorButton.Dispose();

                        WarriorButton = null;
                    }

                    if (WizardButton != null)
                    {
                        if (!WizardButton.IsDisposed)
                            WizardButton.Dispose();

                        WizardButton = null;
                    }

                    if (TaoistButton != null)
                    {
                        if (!TaoistButton.IsDisposed)
                            TaoistButton.Dispose();

                        TaoistButton = null;
                    }

                    if (AssassinButton != null)
                    {
                        if (!AssassinButton.IsDisposed)
                            AssassinButton.Dispose();

                        AssassinButton = null;
                    }

                    if (MaleChar != null)
                    {
                        if (!MaleChar.IsDisposed)
                        {
                            MaleChar.Dispose();
                        }
                        MaleChar = null;
                    }
                    if (FemaleChar != null)
                    {
                        if (!FemaleChar.IsDisposed)
                        {
                            FemaleChar.Dispose();
                        }
                        FemaleChar = null;
                    }
                    Animations = null;
                    AnimationPoints = null;
                }
            }

            #endregion
        }
    }
}
