using Client.Envir;
using Client.Extentions;
using Client.Scenes;
using Client.Scenes.Configs;
using Client.UserModels;
using Library;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 游戏设置窗口
    /// </summary>
    public sealed class DXConfigWindow : DXWindow
    {
        #region Properties
        public DXImageControl ConfigGround;
        public static DXConfigWindow ActiveConfig;
        public DXKeyBindWindow KeyBindWindow;

        private DXTabControl TabControl;

        public DXLabel AttackModeLabel;

        //图像
        public DXTab GraphicsTab;
        public DXCheckBox FullScreenCheckBox, ClipMouseCheckBox, DebugLabelCheckBox;
        private DXComboBox GameSizeComboBox, LanguageComboBox;

        //声音
        public DXTab SoundTab;
        private DXNumberBox SystemVolumeBox, MusicVolumeBox, SpellVolumeBox, PlayerVolumeBox, MonsterVolumeBox;
        private DXCheckBox BackgroundSoundBox;

        //游戏 
        public DXTab GameTab;
        private DXCheckBox LogChatCheckBox, PartyListVisibleCheckBox;
        public DXButton KeyBindButton;

        //网络
        public DXTab NetworkTab;
        private DXCheckBox UseNetworkConfigCheckBox;
        private DXTextBox IPAddressTextBox;
        private DXNumberBox PortBox;

        //聊天颜色
        public DXImageControl ColourTab;
        public DXColourControl LocalColourBox, GMWhisperInColourBox, WhisperInColourBox, WhisperOutColourBox, GroupColourBox, GuildColourBox, ShoutColourBox, GlobalColourBox, ObserverColourBox, HintColourBox, SystemColourBox, GainsColourBox, AnnouncementColourBox;
        public DXButton ResetColoursButton, Close2Button;

        private DXButton SaveButton, CancelButton, Close1Button;

        public DXButton AllowHuiShengButton, AllowRecallButton, AllowTradeButton, AllowWhisperOutButton, AllowGuildChatButton, AllowGuildButton, AllowFriendButton;
        public DXButton AttackModeButton, VSyncButton, LimitFPSButton, DrawEffectsButton, EnableParticleButton, SmoothRenderingButton;
        public DXButton EscapeCloseAllButton, BGMButton, MusicButton, BGMPositionBar, MusicPositionBar, ColoursButton, PlayerNameButton, UserHealthButton, MonHealthButton, ExpTipsButton;

        /// <summary>
        /// 可见更改
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnVisibleChanged(bool oValue, bool nValue)
        {
            base.OnVisibleChanged(oValue, nValue);

            if (!IsVisible) return;
            //图像设置
            FullScreenCheckBox.Checked = Config.FullScreen;            //全屏
            GameSizeComboBox.ListBox.SelectItem(Config.GameSize);      //分辨率                   
            VSyncButton.Index = Config.VSync ? 1292 : 1295;            //垂直同步            
            LimitFPSButton.Index = Config.LimitFPS ? 1292 : 1295;      //帧数限制
            ClipMouseCheckBox.Checked = Config.ClipMouse;              //鼠标形态
            DebugLabelCheckBox.Checked = Config.DebugLabel;            //BUG标签顶部
            LanguageComboBox.ListBox.SelectItem(Config.Language);      //语言选择 
            DrawEffectsButton.Index = Config.DrawEffects ? 1292 : 1295;  //绘图效果
            SmoothRenderingButton.Index = Config.SmoothRendering ? 1292 : 1295;  //平滑绘制   
            EnableParticleButton.Index = Config.EnableParticle ? 1292 : 1295;  //粒子效果
            //声音设置
            //BackgroundSoundBox.Checked = Config.SoundInBackground;
            //SystemVolumeBox.ValueTextBox.TextBox.Text = Config.SystemVolume.ToString();     //系统音乐
            //MusicVolumeBox.ValueTextBox.TextBox.Text = Config.MusicVolume.ToString();       //音乐音量
            //PlayerVolumeBox.ValueTextBox.TextBox.Text = Config.PlayerVolume.ToString();     //玩家音量
            //MonsterVolumeBox.ValueTextBox.TextBox.Text = Config.MonsterVolume.ToString();   //怪物音量
            //SpellVolumeBox.ValueTextBox.TextBox.Text = Config.MagicVolume.ToString();       //技能音量
            BGMButton.Index = (Config.BGM ? 1292 : 1295);                                   //音乐开关
            MusicButton.Index = (Config.OtherMusic ? 1292 : 1295);                          //音效开关
            BGMPositionBar.Location = new Point(275 + Config.BGMVolume * 187 / 100, 118);   //音乐条位置
            MusicPositionBar.Location = new Point(275 + Config.OtherMusicVolume * 187 / 100, 178);  //音效条位置
            //网络设置
            UseNetworkConfigCheckBox.Checked = Config.UseNetworkConfig;        //使用网络配置
            IPAddressTextBox.TextBox.Text = Config.IPAddress;                  //IP地址
            PortBox.ValueTextBox.TextBox.Text = Config.Port.ToString();        //端口
            //游戏设置    
            EscapeCloseAllButton.Index = Config.EscapeCloseAll ? 1292 : 1295;  //ESC关闭所有窗口
            LogChatCheckBox.Checked = Config.LogChat;                          //聊天记录
            PlayerNameButton.Index = BigPatchConfig.ShowPlayerNames ? 1292 : 1295;     //名字显示
            UserHealthButton.Index = BigPatchConfig.ChkShowHPBar ? 1292 : 1295;        //血量显示
            MonHealthButton.Index = BigPatchConfig.ShowMonHealth ? 1292 : 1295;        //怪物血量显示
            ExpTipsButton.Index = BigPatchConfig.ChkCloseExpTips ? 1292 : 1295;        //经验提示
            //颜色设置
            LocalColourBox.BackColour = Config.LocalTextColour;                //本地信息
            GMWhisperInColourBox.BackColour = Config.GMWhisperInTextColour;    //GM信息
            WhisperInColourBox.BackColour = Config.WhisperInTextColour;        //收到私聊信息
            WhisperOutColourBox.BackColour = Config.WhisperOutTextColour;      //发出私聊信息
            GroupColourBox.BackColour = Config.GroupTextColour;                //组队信息
            GuildColourBox.BackColour = Config.GuildTextColour;                //行会信息
            ShoutColourBox.BackColour = Config.ShoutTextColour;                //喊话信息
            GlobalColourBox.BackColour = Config.GlobalTextColour;              //全服信息
            ObserverColourBox.BackColour = Config.ObserverTextColour;          //观察者信息
            HintColourBox.BackColour = Config.HintTextColour;                  //提示信息
            SystemColourBox.BackColour = Config.SystemTextColour;              //系统信息
            GainsColourBox.BackColour = Config.GainsTextColour;                //收益信息
            AnnouncementColourBox.BackColour = Config.AnnouncementTextColour;  //公告信息
        }
        /// <summary>
        /// 按键改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);

            KeyBindWindow.Parent = nValue;
        }

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 游戏设置窗口
        /// </summary>
        public DXConfigWindow()
        {
            ActiveConfig = this;   //动态配置

            HasTitle = false;  //字幕标题不显示
            HasTopBorder = false; //不显示上边框
            TitleLabel.Visible = false; //不显示标题
            CloseButton.Visible = false; //不显示关闭按钮            
            AllowResize = false; //不允许调整大小
            HasFooter = false;  //不显示页脚
            Opacity = 0F;

            Size = new Size(512, 468);

            ConfigGround = new DXImageControl()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1290,
                Parent = this,
            };
            ConfigGround.MouseDown += ConfigGround_MouseDown;

            TabControl = new DXTabControl()
            {
                Parent = ConfigGround,
                Location = ClientArea.Location,
                Size = ClientArea.Size,
            };

            NetworkTab = new DXTab()
            {
                Parent = TabControl,
                Border = false,
                BackColour = Color.Empty,
                TabButton = { Label = { Text = "网络".Lang() }, Visible = false },
                Visible = false,
            };

            ColourTab = new DXImageControl()
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Index = 1289,
                Visible = false,
            };

            KeyBindWindow = new DXKeyBindWindow()  //按键设置
            {
                Visible = false
            };

            DXLabel label = new DXLabel
            {
                Text = "[选项调节窗]".Lang(),
                Outline = true,
                ForeColour = Color.White,
                Parent = ConfigGround,
                Location = new Point(210, 30),
            };

            label = new DXLabel
            {
                Text = "[颜色调节窗]".Lang(),
                Outline = true,
                ForeColour = Color.White,
                Parent = ColourTab,
                Location = new Point(210, 30),
            };
            #region Graphics  图像

            FullScreenCheckBox = new DXCheckBox()
            {
                Label = { Text = "全屏".Lang() },
                Parent = this,
                Checked = Config.FullScreen,
                Visible = false
            };
            FullScreenCheckBox.Location = new Point(120 - FullScreenCheckBox.Size.Width, 10);

            GameSizeComboBox = new DXComboBox()
            {
                Parent = ConfigGround,
                Location = new Point(270, 61),
                Size = new Size(150, DXComboBox.DefaultNormalHeight),
                Border = false,
            };

            foreach (Size resolution in Globals.ValidResolutions)
                new DXListBoxItem
                {
                    Parent = GameSizeComboBox.ListBox,
                    Label = { Text = $"{resolution.Width} x {resolution.Height}" },
                    Item = resolution,
                };

            label = new DXLabel
            {
                Text = "允许回生术".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(65, 62),
            };
            AllowHuiShengButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 62 - 7)
            };
            AllowHuiShengButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    AllowHuiShengButton.Index = (AllowHuiShengButton.Index == 1292) ? 1295 : 1292;
                    CEnvir.Enqueue(new C.HuiShengToggle
                    {
                        HuiSheng = (AllowHuiShengButton.Index == 1292)
                    });
                }
            };

            label = new DXLabel
            {
                Text = "允许天地合一".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(60, 92),
            };
            AllowRecallButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Location = new Point(186, 92 - 7),
                Parent = ConfigGround,
                Enabled = true
            };
            AllowRecallButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    AllowRecallButton.Index = (AllowRecallButton.Index == 1292) ? 1295 : 1292;
                    CEnvir.Enqueue(new C.ReCallToggle
                    {
                        Recall = (AllowRecallButton.Index == 1292)
                    });
                }
            };

            label = new DXLabel
            {
                Text = "允许交易".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(70, 122),
            };
            AllowTradeButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Location = new Point(186, 122 - 7),
                Parent = ConfigGround,
            };
            AllowTradeButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    AllowTradeButton.Index = (AllowTradeButton.Index == 1292) ? 1295 : 1292;
                    CEnvir.Enqueue(new C.TradeToggle
                    {
                        Trade = (AllowTradeButton.Index == 1292)
                    });
                }
            };

            label = new DXLabel
            {
                Text = "允许加入行会".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(60, 152),
            };
            AllowGuildButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 152 - 7)
            };
            AllowGuildButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    AllowGuildButton.Index = (AllowGuildButton.Index == 1292) ? 1295 : 1292;
                    CEnvir.Enqueue(new C.GuildToggle
                    {
                        Guild = (AllowGuildButton.Index == 1292)
                    });
                }
            };

            label = new DXLabel
            {
                Text = "允许加好友".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(65, 182),
            };
            AllowFriendButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 182 - 7)
            };
            AllowFriendButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    AllowFriendButton.Index = (AllowFriendButton.Index == 1292) ? 1295 : 1292;
                    CEnvir.Enqueue(new C.FriendToggle
                    {
                        Friend = (AllowFriendButton.Index == 1292)
                    });
                }
            };

            label = new DXLabel
            {
                Text = "垂直同步".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(70, 212),
            };
            VSyncButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 212 - 7)
            };
            VSyncButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    VSyncButton.Index = (VSyncButton.Index == 1292) ? 1295 : 1292;
                    Config.VSync = (VSyncButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "限制帧数".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(70, 242),
            };
            LimitFPSButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 242 - 7)
            };
            LimitFPSButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    LimitFPSButton.Index = (LimitFPSButton.Index == 1292) ? 1295 : 1292;
                    Config.LimitFPS = (LimitFPSButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "魔法光效显示".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(60, 272),
            };
            DrawEffectsButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 272 - 7)
            };
            DrawEffectsButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    DrawEffectsButton.Index = (DrawEffectsButton.Index == 1292) ? 1295 : 1292;
                    Config.DrawEffects = (DrawEffectsButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "魔法粒子显示".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(60, 302),
            };
            EnableParticleButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 302 - 7)
            };
            EnableParticleButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    EnableParticleButton.Index = (EnableParticleButton.Index == 1292) ? 1295 : 1292;
                    Config.EnableParticle = (EnableParticleButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "图像平滑移动".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(60, 332),
            };
            SmoothRenderingButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 332 - 7)
            };
            SmoothRenderingButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    SmoothRenderingButton.Index = (SmoothRenderingButton.Index == 1292) ? 1295 : 1292;
                    Config.SmoothRendering = (SmoothRenderingButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "玩家名字".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(70, 362),
            };
            PlayerNameButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 362 - 7)
            };
            PlayerNameButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    PlayerNameButton.Index = (PlayerNameButton.Index == 1292) ? 1295 : 1292;
                    BigPatchConfig.ShowPlayerNames = (PlayerNameButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "角色显血".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(70, 392),
            };
            UserHealthButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(186, 392 - 7)
            };
            UserHealthButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    UserHealthButton.Index = (UserHealthButton.Index == 1292) ? 1295 : 1292;
                    BigPatchConfig.ChkShowHPBar = (UserHealthButton.Index == 1292);
                    BigPatchConfig.ShowHealth = (UserHealthButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "怪物显血".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(318, 392),
            };
            MonHealthButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(431, 392 - 7)
            };
            MonHealthButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    MonHealthButton.Index = (MonHealthButton.Index == 1292) ? 1295 : 1292;
                    BigPatchConfig.ShowMonHealth = (MonHealthButton.Index == 1292);
                }
            };

            label = new DXLabel
            {
                Text = "经验提示".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(318, 362),
            };
            ExpTipsButton = new DXButton
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(431, 362 - 7)
            };
            ExpTipsButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    ExpTipsButton.Index = (ExpTipsButton.Index == 1292) ? 1295 : 1292;
                    BigPatchConfig.ChkCloseExpTips = (ExpTipsButton.Index == 1292);
                }
            };

            ClipMouseCheckBox = new DXCheckBox()
            {
                Label = { Text = "鼠标限制".Lang() },
                Parent = this,
                Visible = false,
            };
            ClipMouseCheckBox.Location = new Point(230 - ClipMouseCheckBox.Size.Width, 100);

            DebugLabelCheckBox = new DXCheckBox()
            {
                Label = { Text = "顶部调试信息".Lang() },
                Parent = this,
                Visible = false,
            };
            DebugLabelCheckBox.Location = new Point(120 - DebugLabelCheckBox.Size.Width, 120);

            label = new DXLabel
            {
                Text = "语言".Lang(),
                Outline = true,
                Parent = GraphicsTab,
                Visible = false,
            };
            label.Location = new Point(104 - label.Size.Width, 140);

            LanguageComboBox = new DXComboBox()
            {
                Parent = GraphicsTab,
                Location = new Point(104, 140),
                Size = new Size(100, DXComboBox.DefaultNormalHeight),
                Visible = false,
            };
            foreach (Language language in Enum.GetValues(typeof(Language)))
                new DXListBoxItem
                {
                    Parent = LanguageComboBox.ListBox,
                    Label = { Text = language.Description() },
                    Item = language.ToString()
                };
            #endregion

            #region Sound  声音
            BackgroundSoundBox = new DXCheckBox()
            {
                Label = { Text = "后台声音".Lang() },
                Parent = this,
                Checked = Config.SoundInBackground,
                Visible = false,
            };
            BackgroundSoundBox.Location = new Point(120 - BackgroundSoundBox.Size.Width, 10);

            label = new DXLabel
            {
                Text = "背景音量".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
            };
            label.Location = new Point(318, 92);
            BGMButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(431, 92 - 7)
            };
            BGMButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    BGMButton.Index = ((BGMButton.Index == 1292) ? 1295 : 1292);
                    if (BGMButton.Index == 1292)
                    {
                        int music = 275 + (462 - 275) / 2;
                        BGMPositionBar.Location = new Point(music, 118);
                        Config.BGMVolume = (music - 275) * 100 / 187;
                    }
                    else
                    {
                        BGMPositionBar.Location = new Point(275, 118);
                        Config.BGMVolume = 0;
                    }
                    DXSoundManager.AdjustVolume();
                }
            };
            BGMPositionBar = new DXButton()
            {
                Index = 1291,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(432, 118),
                Parent = ConfigGround,
                Movable = true,
            };
            BGMPositionBar.Moving += BGMPositionBar_OnMoving;

            SystemVolumeBox = new DXNumberBox()
            {
                Parent = this,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 35),
                Visible = false,
            };

            label = new DXLabel
            {
                Text = "音效".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
            };
            label.Location = new Point(330, 152);
            MusicButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(431, 152 - 7)
            };
            MusicButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    MusicButton.Index = ((MusicButton.Index == 1292) ? 1295 : 1292);
                    if (MusicButton.Index == 1292)
                    {
                        int music = 275 + (462 - 275) / 2;
                        MusicPositionBar.Location = new Point(music, 178);
                        Config.OtherMusicVolume = (music - 275) * 100 / 187;
                    }
                    else
                    {
                        MusicPositionBar.Location = new Point(275, 178);
                        Config.OtherMusicVolume = 0;
                    }
                    DXSoundManager.AdjustVolume();
                }
            };
            MusicPositionBar = new DXButton()
            {
                Index = 1291,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(432, 178),
                Parent = ConfigGround,
                Movable = true,
            };
            MusicPositionBar.Moving += MusicPositionBar_OnMoving;

            MusicVolumeBox = new DXNumberBox()
            {
                Parent = this,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 60),
                Visible = false,
            };

            label = new DXLabel
            {
                Text = "玩家音量".Lang(),
                Outline = true,
                Parent = this,
                Visible = false,
            };
            label.Location = new Point(104 - label.Size.Width, 85);

            PlayerVolumeBox = new DXNumberBox()
            {
                Parent = this,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 85),
                Visible = false,
            };
            label = new DXLabel
            {
                Text = "怪物音量".Lang(),
                Outline = true,
                Parent = this,
                Visible = false,
            };
            label.Location = new Point(104 - label.Size.Width, 110);

            MonsterVolumeBox = new DXNumberBox()
            {
                Parent = this,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 110),
                Visible = false,
            };

            label = new DXLabel
            {
                Text = "技能音量".Lang(),
                Outline = true,
                Parent = this,
                Visible = false,
            };
            label.Location = new Point(104 - label.Size.Width, 135);

            SpellVolumeBox = new DXNumberBox()
            {
                Parent = this,
                MinValue = 0,
                MaxValue = 100,
                Location = new Point(104, 135),
                Visible = false,
            };
            #endregion

            #region Game  游戏

            AttackModeLabel = new DXLabel
            {
                Outline = true,
                ForeColour = Color.Orange,
                Parent = ConfigGround,
                Location = new Point(318 - 30, 242),
            };
            AttackModeButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1296,
                Parent = ConfigGround,
                Location = new Point(431, 242 - 7),
                Hint = "切换攻击模式".Lang(),
            };
            AttackModeButton.MouseDown += (o, e) =>
            {
                AttackModeButton.Index = 1297;
            };
            AttackModeButton.MouseUp += (o, e) =>
            {
                AttackModeButton.Index = 1296;
            };
            AttackModeButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    GameScene.Game.User.AttackMode = (AttackMode)(((int)GameScene.Game.User.AttackMode + 1) % 5);
                    CEnvir.Enqueue(new C.ChangeAttackMode { Mode = GameScene.Game.User.AttackMode });
                }
            };

            label = new DXLabel
            {
                Parent = ConfigGround,
                Text = "ESC一键关闭所有窗口".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Location = new Point(285, 212),
            };
            EscapeCloseAllButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1292,
                Parent = ConfigGround,
                Location = new Point(431, 212 - 7)
            };
            EscapeCloseAllButton.MouseClick += (o, e) =>
            {
                if (!GameScene.Game.Observer)
                {
                    EscapeCloseAllButton.Index = (EscapeCloseAllButton.Index == 1292) ? 1295 : 1292;
                    Config.EscapeCloseAll = (EscapeCloseAllButton.Index == 1292);
                }
            };

            LogChatCheckBox = new DXCheckBox()
            {
                Label = { Text = "聊天记录".Lang() },
                Parent = this,
                Visible = false,
            };
            LogChatCheckBox.Location = new Point(270 - LogChatCheckBox.Size.Width, 40);


            label = new DXLabel
            {
                Parent = ConfigGround,
                Text = "按键快捷".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Location = new Point(318, 272),
            };
            KeyBindButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1296,
                Parent = ConfigGround,
                Location = new Point(431, 272 - 7),
                Hint = "设置快捷键".Lang(),
            };
            KeyBindButton.MouseDown += (o, e) =>
            {
                KeyBindButton.Index = 1297;
            };
            KeyBindButton.MouseUp += (o, e) =>
            {
                KeyBindButton.Index = 1296;
            };
            KeyBindButton.MouseClick += (o, e) => KeyBindWindow.Visible = !KeyBindWindow.Visible;

            label = new DXLabel
            {
                Parent = ConfigGround,
                Text = "文字颜色".Lang(),
                Outline = true,
                ForeColour = Color.Orange,
                Location = new Point(318, 302),
            };
            ColoursButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1296,
                Parent = ConfigGround,
                Location = new Point(431, 302 - 7),
                Hint = "设置文字颜色".Lang(),
            };
            ColoursButton.MouseDown += (o, e) =>
            {
                ColoursButton.Index = 1297;
            };
            ColoursButton.MouseUp += (o, e) =>
            {
                ColoursButton.Index = 1296;
            };
            ColoursButton.MouseClick += (o, e) =>
            {
                ColourTab.Visible = true;
                ConfigGround.Visible = false;
            };

            #endregion

            #region Network  网络

            UseNetworkConfigCheckBox = new DXCheckBox()
            {
                Label = { Text = "运用配置".Lang() },
                Parent = NetworkTab,
                Checked = Config.FullScreen,
            };
            UseNetworkConfigCheckBox.Location = new Point(120 - UseNetworkConfigCheckBox.Size.Width, 10);

            label = new DXLabel
            {
                Text = "IP地址".Lang(),
                Outline = true,
                Parent = NetworkTab,
            };
            label.Location = new Point(104 - label.Size.Width, 35);

            IPAddressTextBox = new DXTextBox
            {
                Location = new Point(104, 35),
                Size = new Size(100, 16),
                Parent = NetworkTab,
            };

            label = new DXLabel
            {
                Text = "端口".Lang(),
                Outline = true,
                Parent = NetworkTab,
            };
            label.Location = new Point(104 - label.Size.Width, 60);

            PortBox = new DXNumberBox()
            {
                Parent = NetworkTab,
                Change = 100,
                MaxValue = ushort.MaxValue,
                Location = new Point(104, 60)
            };
            #endregion

            #region Colours  颜色

            label = new DXLabel
            {
                Text = "本地信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(70, 62);

            LocalColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 62),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "GM信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(322, 62);

            GMWhisperInColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(435, 62),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "收到信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(70, 92);

            WhisperInColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 92),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "发出信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(318, 92);

            WhisperOutColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(435, 92),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "组队信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(70, 122);

            GroupColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 122),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "公会信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(318, 122);

            GuildColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(435, 122),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "喊话信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(70, 152);

            ShoutColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 152),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "全服信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(318, 152);

            GlobalColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(435, 152),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "观察者信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(65, 182);

            ObserverColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 182),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "提示信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(318, 182);

            HintColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(435, 182),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "系统信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(70, 212);

            SystemColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 212),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "收益信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(318, 212);

            GainsColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(435, 212),
                Size = new Size(40, label.Size.Height),
            };

            label = new DXLabel
            {
                Text = "公告信息".Lang(),
                Outline = true,
                Parent = ColourTab,
                ForeColour = Color.Orange,
            };
            label.Location = new Point(70, 242);

            AnnouncementColourBox = new DXColourControl()
            {
                Parent = ColourTab,
                Location = new Point(190, 242),
                Size = new Size(40, label.Size.Height),
            };

            ResetColoursButton = new DXButton()   //颜色重置
            {
                Parent = ColourTab,
                Location = new Point(215, 425),
                Size = new Size(80, SmallButtonHeight),
                ButtonType = ButtonType.SmallButton,
                Label = { Text = "重置全部".Lang() }
            };
            ResetColoursButton.MouseClick += (o, e) =>   //默认重置的颜色
            {
                LocalColourBox.BackColour = Color.White;                          //本地信息 白色
                GMWhisperInColourBox.BackColour = Color.Red;                      //GM信息 红色
                WhisperInColourBox.BackColour = Color.FromArgb(230, 120, 5);      //收到私聊 金黄色
                WhisperOutColourBox.BackColour = Color.Yellow;                    //发出私聊 黄色
                GroupColourBox.BackColour = Color.Yellow;                         //组队聊天 黄色
                GuildColourBox.BackColour = Color.Lime;                           //公会信息 绿色
                ShoutColourBox.BackColour = Color.Black;                          //区域喊话 黑色
                GlobalColourBox.BackColour = Color.Black;                         //世界喊话 黑色
                ObserverColourBox.BackColour = Color.Silver;                      //观察者信息 浅白
                HintColourBox.BackColour = Color.AntiqueWhite;                    //提示信息 粉色
                SystemColourBox.BackColour = Color.Red;                           //系统信息 红色
                GainsColourBox.BackColour = Color.GreenYellow;                    //收益信息 浅绿
                AnnouncementColourBox.BackColour = Color.White;                   //公告信息 白色
            };
            #endregion

            SaveButton = new DXButton()
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1297,
                Location = new Point(431, 61 - 7),
                Parent = ConfigGround,
                Hint = "保存".Lang(),
            };
            SaveButton.MouseClick += SaveSettings;

            CancelButton = new DXButton()
            {
                Location = new Point(Size.Width - 190, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Visible = false,
                Label = { Text = "取消".Lang() }
            };
            CancelButton.MouseClick += CancelSettings;

            Close1Button = new DXButton()       //关闭按钮
            {
                Parent = ConfigGround,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            Close1Button.Location = new Point(Size.Width - Close1Button.Size.Width - 31, Size.Height - 53);
            Close1Button.MouseClick += (o, e) => Visible = false;

            Close2Button = new DXButton()       //关闭按钮
            {
                Parent = ColourTab,
                Index = 1221,
                LibraryFile = LibraryFile.UI1,
                Hint = "关闭",
            };
            Close2Button.Location = new Point(Size.Width - Close2Button.Size.Width - 31, Size.Height - 53);
            Close2Button.MouseClick += (o, e) =>
            {
                ColourTab.Visible = false;
                ConfigGround.Visible = true;
            };
        }

        #region Methods
        private void ConfigGround_MouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        private void BGMPositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int bgm = BGMPositionBar.Location.X;
            if (bgm >= 462)
            {
                bgm = 462;
            }
            if (bgm <= 275)
            {
                bgm = 275;
            }
            BGMPositionBar.Location = new Point(bgm, 118);
            Config.BGMVolume = (bgm - 275) * 100 / 187;
            DXSoundManager.AdjustVolume();
        }

        private void MusicPositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int music = MusicPositionBar.Location.X;
            if (music >= 462)
            {
                music = 462;
            }
            if (music <= 275)
            {
                music = 275;
            }
            MusicPositionBar.Location = new Point(music, 178);
            Config.OtherMusicVolume = (music - 275) * 100 / 187;
            DXSoundManager.AdjustVolume();
        }

        /// <summary>
        /// 取消设置
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void CancelSettings(object o, MouseEventArgs e)
        {
            Visible = false;
        }
        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void SaveSettings(object o, MouseEventArgs e)
        {
            //if (Config.FullScreen != FullScreenCheckBox.Checked)
            //{
            //    //DXManager.ToggleFullScreen();
            //}

            //if (GameSizeComboBox.SelectedItem is Size && Config.GameSize != (Size)GameSizeComboBox.SelectedItem)
            //{
            //    Config.GameSize = (Size)GameSizeComboBox.SelectedItem;

            //    if (ActiveScene is GameScene)
            //    {
            //        ActiveScene.Size = Config.GameSize;
            //        DXManager.SetResolution(ActiveScene.Size);
            //    }
            //}

            if (LanguageComboBox.SelectedItem is string && Config.Language != (string)LanguageComboBox.SelectedItem)
            {

                Config.Language = (string)LanguageComboBox.SelectedItem;
                if (!Enum.TryParse(Config.Language, out Language lang))
                {
                    lang = Language.SimplifiedChinese;
                }
                if (CEnvir.Connection != null && CEnvir.Connection.ServerConnected)
                    CEnvir.Enqueue(new C.SelectLanguage { Language = lang });

            }

            if (Config.VSync != (VSyncButton.Index == 1292) ? true : false)
            {
                Config.VSync = (VSyncButton.Index == 1292) ? true : false;
                DXManager.ResetDevice();
            }

            Config.ClipMouse = ClipMouseCheckBox.Checked;                    //鼠标样式
            Config.DebugLabel = DebugLabelCheckBox.Checked;                  //顶部调试信息

            DebugLabel.IsVisible = Config.DebugLabel;
            PingLabel.IsVisible = Config.DebugLabel;

            //if (Config.SoundInBackground != BackgroundSoundBox.Checked)
            //{
            //    Config.SoundInBackground = BackgroundSoundBox.Checked;

            //    DXSoundManager.UpdateFlags();
            //}

            //bool volumeChanged = false;

            //if (Config.SystemVolume != SystemVolumeBox.Value)
            //{
            //    Config.SystemVolume = (int)SystemVolumeBox.Value;
            //    volumeChanged = true;
            //}

            //if (Config.MusicVolume != MusicVolumeBox.Value)
            //{
            //    Config.MusicVolume = (int)MusicVolumeBox.Value;
            //    volumeChanged = true;
            //}

            //if (Config.PlayerVolume != PlayerVolumeBox.Value)
            //{
            //    Config.PlayerVolume = (int)PlayerVolumeBox.Value;
            //    volumeChanged = true;
            //}

            //if (Config.MonsterVolume != MonsterVolumeBox.Value)
            //{
            //    Config.MonsterVolume = (int)MonsterVolumeBox.Value;
            //    volumeChanged = true;
            //}

            //if (Config.MagicVolume != SpellVolumeBox.Value)
            //{
            //    Config.MagicVolume = (int)SpellVolumeBox.Value;
            //    volumeChanged = true;
            //}

            Config.LogChat = LogChatCheckBox.Checked;                        //聊天记录

            //if (volumeChanged)
            //    DXSoundManager.AdjustVolume();

            Config.UseNetworkConfig = UseNetworkConfigCheckBox.Checked;
            Config.IPAddress = IPAddressTextBox.TextBox.Text;
            Config.Port = (int)PortBox.Value;


            bool coloursChanged = false;

            if (Config.LocalTextColour != LocalColourBox.BackColour)
            {
                Config.LocalTextColour = LocalColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GMWhisperInTextColour != GMWhisperInColourBox.BackColour)
            {
                Config.GMWhisperInTextColour = GMWhisperInColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.WhisperInTextColour != WhisperInColourBox.BackColour)
            {
                Config.WhisperInTextColour = WhisperInColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.WhisperOutTextColour != WhisperOutColourBox.BackColour)
            {
                Config.WhisperOutTextColour = WhisperOutColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GroupTextColour != GroupColourBox.BackColour)
            {
                Config.GroupTextColour = GroupColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GuildTextColour != GuildColourBox.BackColour)
            {
                Config.GuildTextColour = GuildColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ShoutTextColour != ShoutColourBox.BackColour)
            {
                Config.ShoutTextColour = ShoutColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GlobalTextColour != GlobalColourBox.BackColour)
            {
                Config.GlobalTextColour = GlobalColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.ObserverTextColour != ObserverColourBox.BackColour)
            {
                Config.ObserverTextColour = ObserverColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.HintTextColour != HintColourBox.BackColour)
            {
                Config.HintTextColour = HintColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.SystemTextColour != SystemColourBox.BackColour)
            {
                Config.SystemTextColour = SystemColourBox.BackColour;
                coloursChanged = true;
            }

            if (Config.GainsTextColour != GainsColourBox.BackColour)
            {
                Config.GainsTextColour = GainsColourBox.BackColour;
                coloursChanged = true;
            }
            if (Config.AnnouncementTextColour != AnnouncementColourBox.BackColour)
            {
                Config.AnnouncementTextColour = AnnouncementColourBox.BackColour;
                coloursChanged = true;
            }

            if (coloursChanged && GameScene.Game != null)
            {
                //foreach (ChatTab tab in ChatTab.Tabs)
                //tab.UpdateColours();
            }
        }

        /// <summary>
        /// 按下按键时
        /// </summary>
        /// <param name="e"></param>
        public override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Escape:           //ESC键
                    Visible = false;
                    break;
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (ActiveConfig == this)
                    ActiveConfig = null;

                if (ConfigGround != null)
                {
                    if (!ConfigGround.IsDisposed)
                        ConfigGround.Dispose();

                    ConfigGround = null;
                }

                if (TabControl != null)
                {
                    if (!TabControl.IsDisposed)
                        TabControl.Dispose();

                    TabControl = null;
                }

                if (KeyBindWindow != null)
                {
                    if (!KeyBindWindow.IsDisposed)
                        KeyBindWindow.Dispose();

                    KeyBindWindow = null;
                }

                #region Graphics
                if (GraphicsTab != null)
                {
                    if (!GraphicsTab.IsDisposed)
                        GraphicsTab.Dispose();

                    GraphicsTab = null;
                }

                if (FullScreenCheckBox != null)
                {
                    if (!FullScreenCheckBox.IsDisposed)
                        FullScreenCheckBox.Dispose();

                    FullScreenCheckBox = null;
                }

                if (ClipMouseCheckBox != null)
                {
                    if (!ClipMouseCheckBox.IsDisposed)
                        ClipMouseCheckBox.Dispose();

                    ClipMouseCheckBox = null;
                }

                if (DebugLabelCheckBox != null)
                {
                    if (!DebugLabelCheckBox.IsDisposed)
                        DebugLabelCheckBox.Dispose();

                    DebugLabelCheckBox = null;
                }

                if (GameSizeComboBox != null)
                {
                    if (!GameSizeComboBox.IsDisposed)
                        GameSizeComboBox.Dispose();

                    GameSizeComboBox = null;
                }

                if (LanguageComboBox != null)
                {
                    if (!LanguageComboBox.IsDisposed)
                        LanguageComboBox.Dispose();

                    LanguageComboBox = null;
                }
                #endregion

                #region Sound
                if (SoundTab != null)
                {
                    if (!SoundTab.IsDisposed)
                        SoundTab.Dispose();

                    SoundTab = null;
                }

                if (SystemVolumeBox != null)
                {
                    if (!SystemVolumeBox.IsDisposed)
                        SystemVolumeBox.Dispose();

                    SystemVolumeBox = null;
                }

                if (MusicVolumeBox != null)
                {
                    if (!MusicVolumeBox.IsDisposed)
                        MusicVolumeBox.Dispose();

                    MusicVolumeBox = null;
                }

                if (PlayerVolumeBox != null)
                {
                    if (!PlayerVolumeBox.IsDisposed)
                        PlayerVolumeBox.Dispose();

                    PlayerVolumeBox = null;
                }

                if (MonsterVolumeBox != null)
                {
                    if (!MonsterVolumeBox.IsDisposed)
                        MonsterVolumeBox.Dispose();

                    MonsterVolumeBox = null;
                }

                if (SpellVolumeBox != null)
                {
                    if (!SpellVolumeBox.IsDisposed)
                        SpellVolumeBox.Dispose();

                    SpellVolumeBox = null;
                }

                if (BackgroundSoundBox != null)
                {
                    if (!BackgroundSoundBox.IsDisposed)
                        BackgroundSoundBox.Dispose();

                    BackgroundSoundBox = null;
                }
                #endregion

                #region Game
                if (GameTab != null)
                {
                    if (!GameTab.IsDisposed)
                        GameTab.Dispose();

                    GameTab = null;
                }

                if (LogChatCheckBox != null)
                {
                    if (!LogChatCheckBox.IsDisposed)
                        LogChatCheckBox.Dispose();

                    LogChatCheckBox = null;
                }

                if (PartyListVisibleCheckBox != null)
                {
                    if (!PartyListVisibleCheckBox.IsDisposed)
                        PartyListVisibleCheckBox.Dispose();

                    PartyListVisibleCheckBox = null;
                }

                if (KeyBindButton != null)
                {
                    if (!KeyBindButton.IsDisposed)
                        KeyBindButton.Dispose();

                    KeyBindButton = null;
                }
                #endregion

                #region Network
                if (NetworkTab != null)
                {
                    if (!NetworkTab.IsDisposed)
                        NetworkTab.Dispose();

                    NetworkTab = null;
                }

                if (UseNetworkConfigCheckBox != null)
                {
                    if (!UseNetworkConfigCheckBox.IsDisposed)
                        UseNetworkConfigCheckBox.Dispose();

                    UseNetworkConfigCheckBox = null;
                }

                if (IPAddressTextBox != null)
                {
                    if (!IPAddressTextBox.IsDisposed)
                        IPAddressTextBox.Dispose();

                    IPAddressTextBox = null;
                }

                if (PortBox != null)
                {
                    if (!PortBox.IsDisposed)
                        PortBox.Dispose();

                    PortBox = null;
                }
                #endregion

                #region Colours
                if (ColourTab != null)
                {
                    if (!ColourTab.IsDisposed)
                        ColourTab.Dispose();

                    ColourTab = null;
                }

                if (LocalColourBox != null)
                {
                    if (!LocalColourBox.IsDisposed)
                        LocalColourBox.Dispose();

                    LocalColourBox = null;
                }

                if (GMWhisperInColourBox != null)
                {
                    if (!GMWhisperInColourBox.IsDisposed)
                        GMWhisperInColourBox.Dispose();

                    GMWhisperInColourBox = null;
                }

                if (WhisperInColourBox != null)
                {
                    if (!WhisperInColourBox.IsDisposed)
                        WhisperInColourBox.Dispose();

                    WhisperInColourBox = null;
                }

                if (WhisperOutColourBox != null)
                {
                    if (!WhisperOutColourBox.IsDisposed)
                        WhisperOutColourBox.Dispose();

                    WhisperOutColourBox = null;
                }

                if (GroupColourBox != null)
                {
                    if (!GroupColourBox.IsDisposed)
                        GroupColourBox.Dispose();

                    GroupColourBox = null;
                }

                if (GuildColourBox != null)
                {
                    if (!GuildColourBox.IsDisposed)
                        GuildColourBox.Dispose();

                    GuildColourBox = null;
                }

                if (ShoutColourBox != null)
                {
                    if (!ShoutColourBox.IsDisposed)
                        ShoutColourBox.Dispose();

                    ShoutColourBox = null;
                }

                if (GlobalColourBox != null)
                {
                    if (!GlobalColourBox.IsDisposed)
                        GlobalColourBox.Dispose();

                    GlobalColourBox = null;
                }

                if (ObserverColourBox != null)
                {
                    if (!ObserverColourBox.IsDisposed)
                        ObserverColourBox.Dispose();

                    ObserverColourBox = null;
                }

                if (HintColourBox != null)
                {
                    if (!HintColourBox.IsDisposed)
                        HintColourBox.Dispose();

                    HintColourBox = null;
                }

                if (SystemColourBox != null)
                {
                    if (!SystemColourBox.IsDisposed)
                        SystemColourBox.Dispose();

                    SystemColourBox = null;
                }

                if (GainsColourBox != null)
                {
                    if (!GainsColourBox.IsDisposed)
                        GainsColourBox.Dispose();

                    GainsColourBox = null;
                }
                #endregion

                if (SaveButton != null)
                {
                    if (!SaveButton.IsDisposed)
                        SaveButton.Dispose();

                    SaveButton = null;
                }

                if (CancelButton != null)
                {
                    if (!CancelButton.IsDisposed)
                        CancelButton.Dispose();

                    CancelButton = null;
                }

                if (AllowHuiShengButton != null)
                {
                    if (!AllowHuiShengButton.IsDisposed)
                        AllowHuiShengButton.Dispose();

                    AllowHuiShengButton = null;
                }

                if (AllowRecallButton != null)
                {
                    if (!AllowRecallButton.IsDisposed)
                        AllowRecallButton.Dispose();

                    AllowRecallButton = null;
                }

                if (AllowTradeButton != null)
                {
                    if (!AllowTradeButton.IsDisposed)
                        AllowTradeButton.Dispose();

                    AllowTradeButton = null;
                }

                if (AllowWhisperOutButton != null)
                {
                    if (!AllowWhisperOutButton.IsDisposed)
                        AllowWhisperOutButton.Dispose();

                    AllowWhisperOutButton = null;
                }

                if (AllowGuildChatButton != null)
                {
                    if (!AllowGuildChatButton.IsDisposed)
                        AllowGuildChatButton.Dispose();

                    AllowGuildChatButton = null;
                }

                if (AllowGuildButton != null)
                {
                    if (!AllowGuildButton.IsDisposed)
                        AllowGuildButton.Dispose();

                    AllowGuildButton = null;
                }

                if (AllowFriendButton != null)
                {
                    if (!AllowFriendButton.IsDisposed)
                        AllowFriendButton.Dispose();

                    AllowFriendButton = null;
                }

                if (AttackModeButton != null)
                {
                    if (!AttackModeButton.IsDisposed)
                        AttackModeButton.Dispose();

                    AttackModeButton = null;
                }

                if (VSyncButton != null)
                {
                    if (!VSyncButton.IsDisposed)
                        VSyncButton.Dispose();

                    VSyncButton = null;
                }

                if (LimitFPSButton != null)
                {
                    if (!LimitFPSButton.IsDisposed)
                        LimitFPSButton.Dispose();

                    LimitFPSButton = null;
                }

                if (DrawEffectsButton != null)
                {
                    if (!DrawEffectsButton.IsDisposed)
                        DrawEffectsButton.Dispose();

                    DrawEffectsButton = null;
                }

                if (EnableParticleButton != null)
                {
                    if (!EnableParticleButton.IsDisposed)
                        EnableParticleButton.Dispose();

                    EnableParticleButton = null;
                }

                if (SmoothRenderingButton != null)
                {
                    if (!SmoothRenderingButton.IsDisposed)
                        SmoothRenderingButton.Dispose();

                    SmoothRenderingButton = null;
                }

                if (EscapeCloseAllButton != null)
                {
                    if (!EscapeCloseAllButton.IsDisposed)
                        EscapeCloseAllButton.Dispose();

                    EscapeCloseAllButton = null;
                }

                if (BGMButton != null)
                {
                    if (!BGMButton.IsDisposed)
                        BGMButton.Dispose();

                    BGMButton = null;
                }

                if (MusicButton != null)
                {
                    if (!MusicButton.IsDisposed)
                        MusicButton.Dispose();

                    MusicButton = null;
                }

                if (BGMPositionBar != null)
                {
                    if (!BGMPositionBar.IsDisposed)
                        BGMPositionBar.Dispose();

                    BGMPositionBar = null;
                }

                if (MusicPositionBar != null)
                {
                    if (!MusicPositionBar.IsDisposed)
                        MusicPositionBar.Dispose();

                    MusicPositionBar = null;
                }

                if (ColoursButton != null)
                {
                    if (!ColoursButton.IsDisposed)
                        ColoursButton.Dispose();

                    ColoursButton = null;
                }
            }
        }
        #endregion
    }
}
