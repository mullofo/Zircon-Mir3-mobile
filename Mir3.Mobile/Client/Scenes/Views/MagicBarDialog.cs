using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 魔法技能快捷栏功能
    /// </summary>
    public sealed class MagicBarDialog : DXWindow
    {
        #region SpellSet
        /// <summary>
        /// 设置
        /// </summary>
        public int SpellSet
        {
            get => _SpellSet;
            set
            {
                if (_SpellSet == value) return;

                int oldValue = _SpellSet;
                _SpellSet = value;

                OnSpellSetChanged(oldValue, value);
            }
        }
        private int _SpellSet;
        public event EventHandler<EventArgs> SpellSetChanged;

        public void OnSpellSetChanged(int oValue, int nValue)  //技能设置更改时
        {
            SpellSetChanged?.Invoke(this, EventArgs.Empty);  //技能设置改变了？ 调用（这个事件参数 空);

            UpdateIcons();   //更新技能图标

            //遍历 （键值对应<技能信息 技能设置>）配对 游戏场景 技能魔法设置
            foreach (KeyValuePair<MagicInfo, MagicCell> pair in GameScene.Game.MagicBox.Magics)
            {
                pair.Value.Refresh();  //键值刷新
            }
        }
        #endregion

        public DXButton UpButton, DownButton;
        public DXLabel SetLabel;

        /// <summary>
        /// 映射键值对应底图
        /// </summary>
        Dictionary<SpellKey, DXImageControl> IconsOrder = new Dictionary<SpellKey, DXImageControl>();
        /// <summary>
        /// 映射键值对应图标
        /// </summary>
        Dictionary<SpellKey, DXImageControl> Icons = new Dictionary<SpellKey, DXImageControl>();
        /// <summary>
        /// 映射键值对应标签
        /// </summary>
        Dictionary<SpellKey, DXLabel> Cooldowns = new Dictionary<SpellKey, DXLabel>();

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        /// <summary>
        /// 魔法技能快捷栏界面
        /// </summary>
        public MagicBarDialog()
        {
            _SpellSet = 1;

            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            IgnoreMoveBounds = true;
            Opacity = 0F;

            SetClientSize(new Size(37 * 12 + 15 + 25 + 37 + 37, 37 - 8));
            Location = new Point(0, 25);

            int x = ClientArea.X;
            IconsOrder[SpellKey.Spell01] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x - 14, ClientArea.Y - 16),
                Index = 1450,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell01] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell02] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 37 - 14, ClientArea.Y - 16),
                Index = 1451,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell02] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 37, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell03] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 74 - 14, ClientArea.Y - 16),
                Index = 1452,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell03] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 74, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell04] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 111 - 14, ClientArea.Y - 16),
                Index = 1453,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell04] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 111, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            x += 37;
            IconsOrder[SpellKey.Spell05] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 148 - 14, ClientArea.Y - 16),
                Index = 1454,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell05] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 148, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell06] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 185 - 14, ClientArea.Y - 16),
                Index = 1455,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell06] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 185, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell07] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 222 - 14, ClientArea.Y - 16),
                Index = 1456,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell07] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 222, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell08] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 259 - 14, ClientArea.Y - 16),
                Index = 1457,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell08] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 259, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            x += 37;
            IconsOrder[SpellKey.Spell09] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 296 - 14, ClientArea.Y - 16),
                Index = 1458,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell09] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 296, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell10] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 333 - 14, ClientArea.Y - 16),
                Index = 1459,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell10] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 333, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell11] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 370 - 14, ClientArea.Y - 16),
                Index = 1460,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell11] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 370, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell12] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 407 - 14, ClientArea.Y - 16),
                Index = 1461,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell12] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 407, ClientArea.Y),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            x = ClientArea.X;
            IconsOrder[SpellKey.Spell13] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1450,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell13] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell14] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 37 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1451,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell14] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 37, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell15] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 74 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1452,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell15] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 74, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell16] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 111 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1453,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell16] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 111, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            x += 37;
            IconsOrder[SpellKey.Spell17] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 148 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1454,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell17] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 148, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell18] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 185 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1455,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell18] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 185, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell19] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 222 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1456,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell19] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 222, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell20] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 259 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1457,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell20] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 259, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            x += 37;
            IconsOrder[SpellKey.Spell21] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 296 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1458,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell21] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 296, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell22] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 333 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1459,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell22] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 333, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell23] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 370 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1460,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell23] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 370, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            IconsOrder[SpellKey.Spell24] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.UI1,
                Location = new Point(x + 407 - 14, ClientArea.Y - 16 + 37 + 5),
                Index = 1461,
                ImageOpacity = 0.5F,
            };
            Icons[SpellKey.Spell24] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.MagicIcon145,
                Location = new Point(x + 407, ClientArea.Y + 37 + 5),
                //DrawTexture = true,
                //BackColour = Color.FromArgb(20, 20, 20),
                //Border = true,
                //BorderColour = Color.FromArgb(198, 166, 99),
                Size = new Size(36, 36),
                //Opacity = 0.6F
            };

            int count = 1;
            //遍历 键值<按键，图像> 对应 图标
            foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
            {
                //键值 鼠标进入  游戏设置鼠标进入显示魔法技能标签=图片标记的技能信息
                pair.Value.MouseEnter += (o, e) => GameScene.Game.MouseMagic = ((DXImageControl)o).Tag as MagicInfo;
                //键值 鼠标离开  游戏设置鼠标离开不显示魔法技能标签
                pair.Value.MouseLeave += (o, e) => GameScene.Game.MouseMagic = null;

                DXLabel label = new DXLabel
                {
                    Parent = pair.Value,
                    //Text = "F" + count.ToString(),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F), FontStyle.Italic),
                    IsControl = false,
                };
                label.Location = new Point(37 - label.Size.Width, 37 - label.Size.Height);

                Cooldowns[pair.Key] = new DXLabel  //技能信息标签
                {

                    AutoSize = false,
                    BackColour = Color.FromArgb(125, 50, 50, 50),
                    Parent = pair.Value,
                    Location = new Point(1, 1),
                    IsControl = false,
                    Size = new Size(36, 36),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    ForeColour = Color.Gold,
                    Outline = true,
                    OutlineColour = Color.Black,
                };

                count++;
            }

            UpButton = new DXButton    //向上翻页按钮
            {
                Parent = this,
                Location = new Point(ClientArea.X + 461, ClientArea.Y),
                LibraryFile = LibraryFile.Interface,
                Index = 44,
                Visible = false,
            };
            UpButton.MouseClick += (o, e) => SpellSet = Math.Max(1, SpellSet - 1);

            SetLabel = new DXLabel   //数字标签
            {
                Parent = this,
                Text = SpellSet.ToString(),
                IsControl = false,
                Location = new Point(ClientArea.X + 460, ClientArea.Y + UpButton.Size.Height - 1),
                ForeColour = Color.White,
                Visible = false,
            };

            DownButton = new DXButton   //向下翻页按钮
            {
                Parent = this,
                Location = new Point(ClientArea.X + 461, ClientArea.X + 37 - UpButton.Size.Height),
                LibraryFile = LibraryFile.Interface,
                Index = 46,
                Visible = false,
            };
            DownButton.MouseClick += (o, e) => SpellSet = Math.Min(4, SpellSet + 1);
        }

        /// <summary>
        /// 更新图标
        /// </summary>
        public void UpdateIcons()
        {
            SpellKey maxKey = SpellKey.None; //快捷键最大值为空

            //遍历 键值<按键，图像> 对应 图标
            foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
            {
                //客户端角色技能 = 游戏场景 游戏 用户 技能 魔法值 第一个或者默认值
                ClientUserMagic magic = GameScene.Game?.User?.Magics.Values.FirstOrDefault(x =>
                {
                    switch (SpellSet)  //切换（技能设置）
                    {
                        case 1:
                            return x.Set1Key == pair.Key;  //返回快捷栏对应的技能值
                        case 2:
                            return x.Set2Key == pair.Key;
                        case 3:
                            return x.Set3Key == pair.Key;
                        case 4:
                            return x.Set4Key == pair.Key;
                        default:
                            return false;
                    }
                });

                pair.Value.Tag = magic?.Info;  //键值对应的标签=技能信息

                if (magic != null)  //如果技能不为空
                {
                    maxKey = pair.Key;   //最大键值=对应的技能值
                    pair.Value.Index = magic.Info.Icon;   //对应的序号=技能信息里的图标
                }
                else
                {
                    pair.Value.Index = -1;
                    Cooldowns[pair.Key].Visible = false;  //技能值冷却时间

                }
                pair.Value.Index = magic?.Info.Icon ?? -1;

            }

            SetLabel.Text = SpellSet.ToString();  //设置标签文本=技能标签到字符串

            if (maxKey >= SpellKey.Spell13)
            {
                SetClientSize(new Size(37 * 12 + 15 + 25 + 37 + 37, 37 * 2 + 5));

                Icons[SpellKey.Spell13].Visible = true;
                Icons[SpellKey.Spell14].Visible = true;
                Icons[SpellKey.Spell15].Visible = true;
                Icons[SpellKey.Spell16].Visible = true;
                Icons[SpellKey.Spell17].Visible = true;
                Icons[SpellKey.Spell18].Visible = true;
                Icons[SpellKey.Spell19].Visible = true;
                Icons[SpellKey.Spell20].Visible = true;
                Icons[SpellKey.Spell21].Visible = true;
                Icons[SpellKey.Spell22].Visible = true;
                Icons[SpellKey.Spell23].Visible = true;
                Icons[SpellKey.Spell24].Visible = true;
            }
            else
            {
                SetClientSize(new Size(37 * 12 + 15 + 25 + 37 + 37, 37 - 8));

                Icons[SpellKey.Spell13].Visible = false;
                Icons[SpellKey.Spell14].Visible = false;
                Icons[SpellKey.Spell15].Visible = false;
                Icons[SpellKey.Spell16].Visible = false;
                Icons[SpellKey.Spell17].Visible = false;
                Icons[SpellKey.Spell18].Visible = false;
                Icons[SpellKey.Spell19].Visible = false;
                Icons[SpellKey.Spell20].Visible = false;
                Icons[SpellKey.Spell21].Visible = false;
                Icons[SpellKey.Spell22].Visible = false;
                Icons[SpellKey.Spell23].Visible = false;
                Icons[SpellKey.Spell24].Visible = false;
            }
        }

        public override void Process()
        {
            base.Process();

            if (!Visible) return;

            foreach (KeyValuePair<SpellKey, DXImageControl> pair in Icons)
            {
                MagicInfo info = pair.Value.Tag as MagicInfo;

                if (info == null)
                {
                    Cooldowns[pair.Key].Visible = false;
                    continue;
                }

                ClientUserMagic magic = GameScene.Game.User.Magics[info];

                if (CEnvir.Now >= magic.NextCast)
                {
                    Cooldowns[pair.Key].Visible = false;
                    continue;
                }

                Cooldowns[pair.Key].Visible = true;
                TimeSpan remaining = magic.NextCast - CEnvir.Now;
                Cooldowns[pair.Key].Text = $"{Math.Ceiling(remaining.TotalSeconds)}s";

                if (remaining.TotalSeconds > 5)
                    Cooldowns[pair.Key].ForeColour = Color.Gold;
                else
                    Cooldowns[pair.Key].ForeColour = Color.Red;
            }
        }
    }
}
