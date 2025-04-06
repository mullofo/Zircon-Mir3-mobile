using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Library;
using Library.SystemModels;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using C = Library.Network.ClientPackets;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 手机UI右边按钮区域
    /// </summary>
    public sealed class PhoneRightButtonsPanel : DXControl
    {
        #region Properties

        /// <summary>
        /// 角色按钮
        /// </summary>
        //private DXButton CharacterButton;
        /// <summary>
        /// 背包按钮
        /// </summary>
        //private DXButton InventoryButton;
        /// <summary>
        /// 交易按钮
        /// </summary>
        //private DXButton TradeButton;
        /// <summary>
        /// 邮件按钮
        /// </summary>
        //private DXButton MailButton;

        #endregion

        public PhoneRightButtonsPanel()
        {
            DrawTexture = false;
            Size = new Size(535, 370);
            PassThrough = true;  //穿透开启


            //MailButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 59,
            //};
            //MailButton.Location = new Point(Size.Width - MailButton.Size.Width - 30, 15);
            //MailButton.TouchUp += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.CommunicationBox.Visible = !GameScene.Game.CommunicationBox.Visible;
            //    //GameScene.Game.CommunicationBox.Location = new Point((GameScene.Game.Size.Width - GameScene.Game.CommunicationBox.Size.Width) / 2, (GameScene.Game.Size.Height - GameScene.Game.CommunicationBox.Size.Height) / 2);
            //};

            //TradeButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 57,
            //};
            //TradeButton.Location = new Point(MailButton.Location.X - TradeButton.Size.Width - 15, MailButton.Location.Y);
            //TradeButton.TouchUp += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;

            //    CEnvir.Enqueue(new C.TradeRequest());
            //};


            //InventoryButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 56,
            //};
            //InventoryButton.Location = new Point(MailButton.Location.X - InventoryButton.Size.Width - 15, MailButton.Location.Y);
            //InventoryButton.TouchUp += (o, e) => GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;


            //CharacterButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 55,
            //};
            //CharacterButton.Location = new Point(InventoryButton.Location.X - CharacterButton.Size.Width - 15, InventoryButton.Location.Y);
            //CharacterButton.TouchUp += (o, e) => GameScene.Game.CharacterBox.Visible = !GameScene.Game.CharacterBox.Visible;

        }


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {

                //if (MailButton != null)
                //{
                //    if (!MailButton.IsDisposed)
                //        MailButton.Dispose();

                //    MailButton = null;
                //}


                //if (CharacterButton != null)
                //{
                //    if (!CharacterButton.IsDisposed)
                //        CharacterButton.Dispose();

                //    CharacterButton = null;
                //}

                //if (InventoryButton != null)
                //{
                //    if (!InventoryButton.IsDisposed)
                //        InventoryButton.Dispose();

                //    InventoryButton = null;
                //}

                //if (TradeButton != null)
                //{
                //    if (!TradeButton.IsDisposed)
                //        TradeButton.Dispose();

                //    TradeButton = null;
                //}
            }
        }
        #endregion

    }

    /// <summary>
    /// 手机UI右边魔法按钮区域
    /// </summary>
    public class PhoneMagicBarControl : DXControl
    {
        #region SpellSet
        /// <summary>
        /// 翻页页码
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

            //SetLabel.Text = SpellSet.ToString();  //设置标签文本=技能标签到字符串
            UpButton.Index = 76 + SpellSet - 1;

            UpdateIcons();   //更新技能图标

            //遍历 （键值对应<技能信息 技能设置>）配对 游戏场景 技能魔法设置
            foreach (KeyValuePair<MagicInfo, MagicCell> pair in GameScene.Game.MagicBox.Magics)
            {
                pair.Value.Refresh();  //键值刷新
            }
        }
        #endregion

        /// <summary>
        /// 攻击按钮
        /// </summary>
        private DXButton AttackButton;
        /// <summary>
        /// 是否物理攻击
        /// </summary>
        public bool PhysicalAttack;
        /// <summary>
        /// 选人按钮
        /// </summary>
        private DXImageControl SelectPlayerButton;
        /// <summary>
        /// 选怪按钮
        /// </summary>
        private DXImageControl SelectMonsterButton;
        /// <summary>
        /// 骑马按钮
        /// </summary>
        //private DXButton HorseButton;
        /// <summary>
        /// 角色按钮
        /// </summary>
        private DXButton CharacterButton;
        /// <summary>
        /// 背包按钮
        /// </summary>
        private DXButton InventoryButton;
        /// <summary>
        /// 强攻按钮
        /// </summary>
        //private DXButton ForceAttackButton;
        /// <summary>
        /// 挖肉按钮
        /// </summary>
        //private DXButton HarvestButton;
        public bool SelectPlayer;
        /// <summary>
        /// 自动按钮
        /// </summary>
        //public DXButton AutoAttackButton;

        public DXButton UpButton/*, DownButton*/;
        /// <summary>
        /// 页码Label
        /// </summary>
        //public DXLabel SetLabel;

        /// <summary>
        /// 映射键值对应魔法图标
        /// </summary>
        Dictionary<SpellKey, DXButton> Icons = new Dictionary<SpellKey, DXButton>();
        /// <summary>
        /// 映射键值 可以锁定魔法底图
        /// </summary>
        Dictionary<SpellKey, DXImageControl> IconsOrder = new Dictionary<SpellKey, DXImageControl>();
        /// <summary>
        /// 映射键值 可以8方向魔法底图
        /// </summary>
        Dictionary<SpellKey, DXImageControl> IconsDir = new Dictionary<SpellKey, DXImageControl>();
        /// <summary>
        /// 映射键值对应魔法冷却时间标签
        /// </summary>
        Dictionary<SpellKey, DXLabel> Cooldowns = new Dictionary<SpellKey, DXLabel>();

        private DXAnimatedControl LockMagicAnimation;
        /// <summary>
        /// 技能按钮
        /// </summary>
        private DXButton MagicButton;

        //private DXControl SelectPlayerPanel;
        //private DXButton MinBlood, NeerBy, AttackBack;

        public PhoneMagicBarControl()
        {
            DrawTexture = false;

            Size = new Size(490, 310);
            PassThrough = true;  //穿透开启
            _SpellSet = 1;

            //HorseButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 72,
            //    Location = new Point(142, 192),
            //};
            //HorseButton.TouchUp += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;

            //    if (CEnvir.Now < GameScene.Game.User.NextActionTime || GameScene.Game.User.ActionQueue.Count > 0) return;
            //    if (CEnvir.Now < GameScene.Game.User.ServerTime) return;
            //    if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && GameScene.Game.User.Horse == HorseType.None)
            //    {
            //        GameScene.Game.ReceiveChat("战斗中无法骑马".Lang(), MessageType.System);
            //        return;
            //    }

            //    GameScene.Game.User.ServerTime = CEnvir.Now.AddSeconds(5);
            //    CEnvir.Enqueue(new C.Mount());
            //};

            MagicButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 155,
            };
            MagicButton.Location = new Point(0, Size.Height - MagicButton.Size.Height);
            MagicButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.MagicBox.Visible = !GameScene.Game.MagicBox.Visible;
            };

            #region 魔法按钮
            int space = 3;
            IconsDir[SpellKey.Spell01] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(86, 215),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell01].Location = new Point(IconsDir[SpellKey.Spell01].Location.X + ((iconsize - IconsDir[SpellKey.Spell01].Size.Width) / 2), IconsDir[SpellKey.Spell01].Location.Y + ((iconsize - IconsDir[SpellKey.Spell01].Size.Height) / 2));
            IconsDir[SpellKey.Spell01].Location = new Point(MagicButton.Location.X + MagicButton.Size.Height + 5, Size.Height - IconsDir[SpellKey.Spell01].Size.Height);

            IconsDir[SpellKey.Spell02] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(104, 133),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell02].Location = new Point(IconsDir[SpellKey.Spell02].Location.X + ((iconsize - IconsDir[SpellKey.Spell02].Size.Width) / 2), IconsDir[SpellKey.Spell02].Location.Y + ((iconsize - IconsDir[SpellKey.Spell02].Size.Height) / 2));
            IconsDir[SpellKey.Spell02].Location = new Point(IconsDir[SpellKey.Spell01].Location.X + IconsDir[SpellKey.Spell01].Size.Width + space, IconsDir[SpellKey.Spell01].Location.Y);

            IconsDir[SpellKey.Spell03] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(173, 77),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell03].Location = new Point(IconsDir[SpellKey.Spell03].Location.X + ((iconsize - IconsDir[SpellKey.Spell03].Size.Width) / 2), IconsDir[SpellKey.Spell03].Location.Y + ((iconsize - IconsDir[SpellKey.Spell03].Size.Height) / 2));
            IconsDir[SpellKey.Spell03].Location = new Point(IconsDir[SpellKey.Spell02].Location.X + IconsDir[SpellKey.Spell02].Size.Width + space, IconsDir[SpellKey.Spell02].Location.Y);

            IconsDir[SpellKey.Spell04] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(10, 223),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell04].Location = new Point(IconsDir[SpellKey.Spell04].Location.X + ((iconsize - IconsDir[SpellKey.Spell04].Size.Width) / 2), IconsDir[SpellKey.Spell04].Location.Y + ((iconsize - IconsDir[SpellKey.Spell04].Size.Height) / 2));
            IconsDir[SpellKey.Spell04].Location = new Point(IconsDir[SpellKey.Spell03].Location.X + IconsDir[SpellKey.Spell03].Size.Width + space, IconsDir[SpellKey.Spell03].Location.Y);

            IconsDir[SpellKey.Spell05] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(23, 147),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell05].Location = new Point(IconsDir[SpellKey.Spell05].Location.X + ((iconsize - IconsDir[SpellKey.Spell05].Size.Width) / 2), IconsDir[SpellKey.Spell05].Location.Y + ((iconsize - IconsDir[SpellKey.Spell05].Size.Height) / 2));
            IconsDir[SpellKey.Spell05].Location = new Point(IconsDir[SpellKey.Spell04].Location.X + IconsDir[SpellKey.Spell04].Size.Width + space, IconsDir[SpellKey.Spell04].Location.Y);

            IconsDir[SpellKey.Spell06] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(59, 78),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell06].Location = new Point(IconsDir[SpellKey.Spell06].Location.X + ((iconsize - IconsDir[SpellKey.Spell06].Size.Width) / 2), IconsDir[SpellKey.Spell06].Location.Y + ((iconsize - IconsDir[SpellKey.Spell06].Size.Height) / 2));
            IconsDir[SpellKey.Spell06].Location = new Point(IconsDir[SpellKey.Spell05].Location.X + IconsDir[SpellKey.Spell05].Size.Width + space, IconsDir[SpellKey.Spell05].Location.Y);

            IconsDir[SpellKey.Spell07] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                //Location = new Point(122, 34),
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            //IconsDir[SpellKey.Spell07].Location = new Point(IconsDir[SpellKey.Spell07].Location.X + ((iconsize - IconsDir[SpellKey.Spell07].Size.Width) / 2), IconsDir[SpellKey.Spell07].Location.Y + ((iconsize - IconsDir[SpellKey.Spell07].Size.Height) / 2));
            IconsDir[SpellKey.Spell07].Location = new Point(IconsDir[SpellKey.Spell01].Location.X, IconsDir[SpellKey.Spell01].Location.Y - IconsDir[SpellKey.Spell07].Size.Height);

            IconsDir[SpellKey.Spell08] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            IconsDir[SpellKey.Spell08].Location = new Point(IconsDir[SpellKey.Spell07].Location.X + IconsDir[SpellKey.Spell07].Size.Width + space, IconsDir[SpellKey.Spell07].Location.Y);


            IconsDir[SpellKey.Spell09] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            IconsDir[SpellKey.Spell09].Location = new Point(IconsDir[SpellKey.Spell08].Location.X + IconsDir[SpellKey.Spell08].Size.Width + space, IconsDir[SpellKey.Spell08].Location.Y);

            IconsDir[SpellKey.Spell10] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            IconsDir[SpellKey.Spell10].Location = new Point(IconsDir[SpellKey.Spell09].Location.X + IconsDir[SpellKey.Spell09].Size.Width + space, IconsDir[SpellKey.Spell09].Location.Y);

            IconsDir[SpellKey.Spell11] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            IconsDir[SpellKey.Spell11].Location = new Point(IconsDir[SpellKey.Spell10].Location.X + IconsDir[SpellKey.Spell10].Size.Width + space, IconsDir[SpellKey.Spell10].Location.Y);

            IconsDir[SpellKey.Spell12] = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 188,
                ImageOpacity = 0.5F,
                Visible = false,
                PassThrough = true,
                IsControl = false,
            };
            IconsDir[SpellKey.Spell12].Location = new Point(IconsDir[SpellKey.Spell11].Location.X + IconsDir[SpellKey.Spell11].Size.Width + space, IconsDir[SpellKey.Spell11].Location.Y);

            //遍历 键值<按键，图像> 对应 图标
            foreach (KeyValuePair<SpellKey, DXImageControl> pair in IconsDir)
            {
                Icons[pair.Key] = new DXButton
                {
                    Parent = this,
                    LibraryFile = LibraryFile.PhoneUI,
                    Index = 65,
                    //Opacity = 0.6F
                };
                Icons[pair.Key].Location = new Point(pair.Value.Location.X + ((pair.Value.Size.Width - Icons[pair.Key].Size.Width) / 2), pair.Value.Location.Y + ((pair.Value.Size.Height - Icons[pair.Key].Size.Height) / 2));
                Icons[pair.Key].FreeDrag += (o, e) => Icons_TouchFreeDrag(o, e, pair.Key);
                Icons[pair.Key].Tap += (o, e) => Icons_Tap(pair.Key);
                Icons[pair.Key].DoubleTap += (o, e) => Icons_DoubleTap(pair.Key);

                Cooldowns[pair.Key] = new DXLabel  //技能信息标签
                {
                    AutoSize = false,
                    BackColour = Color.FromArgb(125, 50, 50, 50),
                    Parent = Icons[pair.Key],
                    Location = new Point((Icons[pair.Key].Size.Width - 40) / 2, (Icons[pair.Key].Size.Height - 40) / 2),
                    IsControl = false,
                    Size = new Size(40, 40),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    ForeColour = Color.Gold,
                    Outline = true,
                    OutlineColour = Color.Black,
                };

                IconsOrder[pair.Key] = new DXImageControl
                {
                    Index = 189,
                    LibraryFile = LibraryFile.PhoneUI,
                    Parent = this,
                    Visible = false,
                    PassThrough = true,
                    IsControl = false,
                };
                IconsOrder[pair.Key].Location = new Point(pair.Value.Location.X + ((pair.Value.Size.Width - IconsOrder[pair.Key].Size.Width) / 2), pair.Value.Location.Y + ((pair.Value.Size.Height - IconsOrder[pair.Key].Size.Height) / 2));
            }
            #endregion

            UpButton = new DXButton    //向上翻页按钮
            {
                Parent = this,
                //Location = new Point(267, 139),
                LibraryFile = LibraryFile.PhoneUI,
                Index = 76,
            };
            UpButton.Location = new Point(IconsDir[SpellKey.Spell06].Location.X + IconsDir[SpellKey.Spell06].Size.Width, IconsDir[SpellKey.Spell12].Location.Y + (IconsDir[SpellKey.Spell12].Size.Height * 2 - UpButton.Size.Height) / 2);
            //UpButton.TouchUp += (o, e) => SpellSet = Math.Max(1, SpellSet - 1);
            UpButton.TouchUp += (o, e) => SpellSet = Math.Max(1, (SpellSet + 1) % 5);

            //SetLabel = new DXLabel   //数字标签
            //{
            //    Parent = UpButton,
            //    Text = SpellSet.ToString(),
            //    IsControl = false,
            //    ForeColour = Color.White,
            //};
            //SetLabel.Location = new Point((UpButton.Size.Width - SetLabel.Size.Width) / 2, (UpButton.Size.Height - SetLabel.Size.Height) / 2);

            //DownButton = new DXButton   //向下翻页按钮
            //{
            //    Parent = this,
            //    //Location = new Point(ClientArea.X + 461, ClientArea.X + 37 - UpButton.Size.Height),
            //    LibraryFile = LibraryFile.Interface,
            //    Index = 46,
            //    Visible = false,
            //};
            //DownButton.MouseClick += (o, e) => SpellSet = Math.Min(4, SpellSet + 1);

            AttackButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 66,
            };
            AttackButton.Location = new Point(IconsDir[SpellKey.Spell12].Location.X - 10, IconsDir[SpellKey.Spell12].Location.Y - AttackButton.Size.Height - 5);
            AttackButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (CEnvir.Shift)
                {
                    CEnvir.Shift = false;
                    GameScene.Game.MapControl.MapButtons = MouseButtons.None;
                    return;
                }

                if (GameScene.Game.AutoAttack)
                {
                    PhysicalAttack = true;
                    return;
                }

                if (GameScene.Game.MagicObject == null || GameScene.Game.MagicObject.Dead)
                {
                    GameScene.Game.SetTargetObject(SelectPlayer ? ObjectType.Player : ObjectType.Monster);
                }
                else
                {
                    GameScene.Game.TargetObject = GameScene.Game.MagicObject;
                }
            };
            AttackButton.FreeDrag += AttackButton_TouchFreeDrag;

            //HarvestButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 153,
            //    //Location = new Point(243, 192),
            //};
            //HarvestButton.Location = new Point(AttackButton.Location.X + AttackButton.Size.Width + space, AttackButton.Location.Y + (AttackButton.Size.Height - HarvestButton.Size.Height) / 2);
            //HarvestButton.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.MapControl.Harvest = !GameScene.Game.MapControl.Harvest;
            //};

            SelectMonsterButton = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 62,
            };
            SelectMonsterButton.Location = new Point(AttackButton.Location.X + AttackButton.Size.Width + space + 10, AttackButton.Location.Y + (AttackButton.Size.Height - SelectMonsterButton.Size.Height) / 2);
            SelectMonsterButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.SetMagicTargetObject(ObjectType.Monster);
                SelectPlayer = false;
                //SelectPlayerButton.Index = 68;
                //SelectMonsterButton.Index = 71;
                SelectPlayerButton.ForeColour = Color.DarkGray;
                SelectMonsterButton.ForeColour = Color.White;
            };

            //AutoAttackButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 63,
            //    //Location = new Point(267, 252),
            //    ForeColour = Color.DarkGray,
            //};
            //AutoAttackButton.Location = new Point(HarvestButton.Location.X, HarvestButton.Location.Y + HarvestButton.Size.Height + space);
            //AutoAttackButton.TouchUp += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.AutoAttack = !GameScene.Game.AutoAttack;

            //    if (!GameScene.Game.AutoAttack)
            //    {
            //        MapObject.TargetObject = null;
            //        GameScene.Game.FocusObject = null;
            //        MapObject.MagicObject = null;
            //        MapObject.MouseObject = null;
            //        GameScene.Game.AutoAttack = false;
            //        ForceAttack = false;
            //    }
            //    else if (GameScene.Game.User.Class == MirClass.Warrior)
            //        ForceAttack = true;
            //};

            //ForceAttackButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 157,
            //};
            //ForceAttackButton.Location = new Point(AttackButton.Location.X - ForceAttackButton.Size.Width - space, AttackButton.Location.Y + (AttackButton.Size.Height - ForceAttackButton.Size.Height) / 2);
            //ForceAttackButton.FreeDrag += ForceAttackButton_TouchFreeDrag;

            InventoryButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 56,
            };
            InventoryButton.Location = new Point(AttackButton.Location.X - InventoryButton.Size.Width - space - 10, AttackButton.Location.Y + (AttackButton.Size.Height - InventoryButton.Size.Height) / 2);
            InventoryButton.TouchUp += (o, e) => GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;

            SelectPlayerButton = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 61,
                ForeColour = Color.DarkGray,
            };
            SelectPlayerButton.Location = new Point(AttackButton.Location.X + (AttackButton.Size.Width + 10) / 2 + 5, AttackButton.Location.Y - SelectPlayerButton.Size.Height - 10);
            //SelectPlayerButton.FreeDrag += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;

            //    MirDirection direction = GameScene.Game.MapControl.StickDirection(e.Delta.X, e.Delta.Y);
            //    if (direction == MirDirection.Up)
            //        SelectPlayerPanel.Visible = true;
            //};
            SelectPlayerButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                GameScene.Game.SetMagicTargetObject(ObjectType.Player);
                SelectPlayer = true;
                //SelectPlayerButton.Index = 69;
                //SelectMonsterButton.Index = 70;
                SelectPlayerButton.ForeColour = Color.White;
                SelectMonsterButton.ForeColour = Color.DarkGray;
            };

            CharacterButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 55,
            };
            CharacterButton.Location = new Point(SelectPlayerButton.Location.X - CharacterButton.Size.Width - 10 - 5, SelectPlayerButton.Location.Y);
            CharacterButton.TouchUp += (o, e) => GameScene.Game.CharacterBox.Visible = !GameScene.Game.CharacterBox.Visible;

            LockMagicAnimation = new DXAnimatedControl
            {
                BaseIndex = 190,
                Index = 190,
                LibraryFile = LibraryFile.PhoneUI,
                AnimationDelay = TimeSpan.FromMilliseconds(500),
                AnimationStart = DateTime.MinValue,
                FrameCount = 7,
                Parent = this,
                Loop = true,
                Visible = false,
                PassThrough = true,
                IsControl = false,
                Tag = SpellKey.None,
            };

            //SelectPlayerPanel = new DXControl
            //{
            //    Parent = this,
            //    //DrawTexture = true,
            //    //BackColour = Color.FromArgb(250, 215, 195),
            //    Size = new Size(170, 38),
            //    Visible = false,
            //};
            //SelectPlayerPanel.Location = new Point(SelectPlayerButton.Location.X - (SelectPlayerPanel.Size.Width - SelectPlayerButton.Size.Width) / 2, SelectPlayerButton.Location.Y - SelectPlayerPanel.Size.Height);

            //MinBlood = new DXButton
            //{
            //    Parent = SelectPlayerPanel,
            //    //Location = new Point(0, 0),
            //    //LibraryFile = LibraryFile.PhoneUI,
            //    //Index = 74,
            //    Size = new Size(50, 25),
            //    ButtonType = ButtonType.Default,
            //    Label = { Text = "残血" },
            //};
            //MinBlood.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;

            //    GameScene.Game.SelectPlayerType = SelectPlayerType.MinBlood;
            //    GameScene.Game.SetMagicTargetObject(ObjectType.Player);
            //    MinBlood.ForeColour = Color.DarkRed;
            //    NeerBy.ForeColour = Color.White;
            //    AttackBack.ForeColour = Color.White;
            //    SelectPlayerPanel.Visible = false;
            //};

            //NeerBy = new DXButton
            //{
            //    Parent = SelectPlayerPanel,
            //    Location = new Point(MinBlood.Location.X + MinBlood.Size.Width + 2, 0),
            //    //LibraryFile = LibraryFile.PhoneUI,
            //    //Index = 74,
            //    Size = new Size(50, 25),
            //    ButtonType = ButtonType.Default,
            //    Label = { Text = "附近" },
            //    ForeColour = Color.DarkRed,
            //};
            //NeerBy.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;

            //    GameScene.Game.SelectPlayerType = SelectPlayerType.NeerBy;
            //    GameScene.Game.SetMagicTargetObject(ObjectType.Player);
            //    MinBlood.ForeColour = Color.White;
            //    NeerBy.ForeColour = Color.DarkRed;
            //    AttackBack.ForeColour = Color.White;
            //    SelectPlayerPanel.Visible = false;
            //};

            //AttackBack = new DXButton
            //{
            //    Parent = SelectPlayerPanel,
            //    Location = new Point(NeerBy.Location.X + NeerBy.Size.Width + 2, 0),
            //    //LibraryFile = LibraryFile.PhoneUI,
            //    //Index = 74,
            //    Size = new Size(50, 25),
            //    ButtonType = ButtonType.Default,
            //    Label = { Text = "反击" },
            //};
            //AttackBack.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;

            //    GameScene.Game.SelectPlayerType = SelectPlayerType.AttackBack;
            //    GameScene.Game.SetMagicTargetObject(ObjectType.Player);
            //    MinBlood.ForeColour = Color.White;
            //    NeerBy.ForeColour = Color.White;
            //    AttackBack.ForeColour = Color.DarkRed;
            //    SelectPlayerPanel.Visible = false;
            //};
        }

        private void AttackButton_TouchFreeDrag(object sender, TouchEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            if (Math.Abs(e.Delta.X) > 0 || Math.Abs(e.Delta.Y) > 0)
            {
                MirDirection direction = GameScene.Game.MapControl.StickDirection(e.Delta.X, e.Delta.Y);

                //if (CEnvir.Shift)
                //{
                //    CEnvir.Shift = false;
                //    GameScene.Game.MapControl.MapButtons = MouseButtons.None;
                //    return;
                //}

                //if (GameScene.Game.AutoAttack)
                //{
                //    PhysicalAttack = true;
                //    return;
                //}

                if (MapObject.User.Dead || (MapObject.User.Poison & PoisonType.ThousandBlades) == PoisonType.ThousandBlades || (MapObject.User.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (MapObject.User.Poison & PoisonType.StunnedStrike) == PoisonType.StunnedStrike || MapObject.User.Buffs.Any(x => x.Type == BuffType.DragonRepulse || x.Type == BuffType.FrostBite)) return;

                if (MapObject.User.MagicAction != null)
                {
                    if (CEnvir.Now < MapObject.User.NextActionTime || MapObject.User.ActionQueue.Count != 0) return;
                }

                bool haselementalhurricane = MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane);

                if (CEnvir.Now > MapObject.User.AttackTime && MapObject.User.Horse == HorseType.None && !haselementalhurricane)
                    MapObject.User.AttemptAction(new ObjectAction(
                        MirAction.Attack,
                        direction,
                        MapObject.User.CurrentLocation,
                        0, //远程攻击目标ID
                        MagicType.None,
                        Element.None));
            }
        }

        //private void ForceAttackButton_TouchFreeDrag(object sender, TouchEventArgs e)
        //{
        //    if (GameScene.Game.Observer) return;

        //    if (Math.Abs(e.Delta.X) > 0 || Math.Abs(e.Delta.Y) > 0)
        //    {
        //        MirDirection direction = GameScene.Game.MapControl.StickDirection(e.Delta.X, e.Delta.Y);

        //        //if (CEnvir.Shift)
        //        //{
        //        //    CEnvir.Shift = false;
        //        //    GameScene.Game.MapControl.MapButtons = MouseButtons.None;
        //        //    return;
        //        //}

        //        //if (GameScene.Game.AutoAttack)
        //        //{
        //        //    PhysicalAttack = true;
        //        //    return;
        //        //}

        //        if (MapObject.User.Dead || (MapObject.User.Poison & PoisonType.ThousandBlades) == PoisonType.ThousandBlades || (MapObject.User.Poison & PoisonType.Paralysis) == PoisonType.Paralysis || (MapObject.User.Poison & PoisonType.StunnedStrike) == PoisonType.StunnedStrike || MapObject.User.Buffs.Any(x => x.Type == BuffType.DragonRepulse || x.Type == BuffType.FrostBite)) return;

        //        if (MapObject.User.MagicAction != null)
        //        {
        //            if (CEnvir.Now < MapObject.User.NextActionTime || MapObject.User.ActionQueue.Count != 0) return;
        //        }

        //        bool haselementalhurricane = MapObject.User.VisibleBuffs.Contains(BuffType.ElementalHurricane);

        //        if (CEnvir.Now > MapObject.User.AttackTime && MapObject.User.Horse == HorseType.None && !haselementalhurricane)
        //            MapObject.User.AttemptAction(new ObjectAction(
        //                MirAction.Attack,
        //                direction,
        //                MapObject.User.CurrentLocation,
        //                0, //远程攻击目标ID
        //                MagicType.None,
        //                Element.None));
        //    }
        //}

        private MirDirection LineMagicDir = MirDirection.Down;
        private int PoisonDustType = 0;
        private void Icons_TouchFreeDrag(object sender, TouchEventArgs e, SpellKey key)
        {
            if (GameScene.Game.Observer) return;

            MagicInfo info = ((DXButton)sender).Tag as MagicInfo;
            if (info == null) return;

            if (Math.Abs(e.Delta.X) > 0 || Math.Abs(e.Delta.Y) > 0)
            {
                //LockMagicAnimation.Visible = false; //释放线性魔法  取消已有锁定魔法 和 自动打怪
                //GameScene.Game.LockMagicType = MagicType.None;
                //LockMagicAnimation.Tag = SpellKey.None;
                GameScene.Game.AutoAttack = false;
                PhysicalAttack = false;

                MirDirection direction = GameScene.Game.MapControl.StickDirection(e.Delta.X, e.Delta.Y);

                //上下滑开关持续施法
                //if (GameScene.Game.CanContinuouslyMagic(info.Magic))
                //{
                //    if (direction == MirDirection.Down)
                //    {
                //        GameScene.Game.ContinuouslyMagic = false;
                //    }
                //    else if (direction == MirDirection.Up)
                //    {
                //        GameScene.Game.ContinuouslyMagicType = info.Magic;
                //        GameScene.Game.ContinuouslyMagic = true;

                //    }
                //    return;
                //}
                //else
                GameScene.Game.ContinuouslyMagic = false;

                if (CEnvir.Now < MapObject.User.NextMagicTime) return;
                if (MapObject.User.MagicAction != null) return;

                if (CanLockMagic(info.Magic))
                {
                    //切换不同技能 激活锁定
                    if ((SpellKey)LockMagicAnimation.Tag != key)
                    {
                        GameScene.Game.LockMagicType = info.Magic;
                        LockMagicAnimation.Tag = key;
                        LockMagicAnimation.Location = new Point(Icons[key].Location.X + ((Icons[key].Size.Width - LockMagicAnimation.Size.Width) / 2), Icons[key].Location.Y + ((Icons[key].Size.Height - LockMagicAnimation.Size.Height) / 2));
                        LockMagicAnimation.Visible = true;
                    }
                }
                else  //如果切换非锁定魔法 取消现有锁定
                {
                    LockMagicAnimation.Visible = false;
                    GameScene.Game.LockMagicType = MagicType.None;
                    LockMagicAnimation.Tag = SpellKey.None;
                    //GameScene.Game.AutoAttack = false; //取消自动打怪
                }

                //线性魔法 8方向魔法
                if (Is8DirMagic(info.Magic))
                {
                    LineMagicDir = direction;
                    GameScene.Game.MapControl.MouseLocation = ZoomRate == 1F ? Functions.Move(new Point((CEnvir.Target.Width / 2), (CEnvir.Target.Height / 2)), direction, 48) : Functions.Move(new Point((int)Math.Round(((CEnvir.Target.Width / 2) - UI_Offset_X) / ZoomRate), (int)Math.Round((CEnvir.Target.Height / 2) / ZoomRate)), direction, 48);
                    GameScene.Game.UseMagic(info.Magic);
                    return;
                }

                //如果是是施毒术 上滑红毒，下滑绿毒
                if (info.Magic == MagicType.PoisonDust)
                {
                    if (direction == MirDirection.Down)
                        PoisonDustType = 0;
                    else if (direction == MirDirection.Up)
                        PoisonDustType = 1;
                    else
                        return;
                    UsePoisonDustMagic(PoisonDustType, false);

                    return;
                }

                switch (info.Magic)
                {
                    //道士辅助技能下滑对自己释放
                    case MagicType.Heal:
                    case MagicType.MagicResistance:
                    case MagicType.Resilience:
                    case MagicType.ElementalSuperiority:
                    case MagicType.MassHeal:
                    case MagicType.BloodLust:
                    case MagicType.TrapOctagon:
                    case MagicType.MassInvisibility:
                        if (direction == MirDirection.Down)
                        {
                            var ob = GameScene.Game.MagicObject;
                            var mob = GameScene.Game.MouseObject;
                            var lo = GameScene.Game.MapControl.MouseLocation;

                            GameScene.Game.MagicObject = null;
                            GameScene.Game.MouseObject = null;
                            GameScene.Game.MapControl.MouseLocation = ZoomRate == 1F ? new Point((CEnvir.Target.Width / 2), (CEnvir.Target.Height / 2)) : new Point((int)Math.Round(((CEnvir.Target.Width / 2) - UI_Offset_X) / ZoomRate), (int)Math.Round((CEnvir.Target.Height / 2) / ZoomRate));
                            GameScene.Game.UseMagic(info.Magic);

                            GameScene.Game.MagicObject = ob;
                            GameScene.Game.MouseObject = mob;
                            GameScene.Game.MapControl.MouseLocation = lo;
                        }
                        return;
                }
            }

        }
        private void Icons_Tap(SpellKey key)
        {
            if (GameScene.Game.Observer) return;

            if (GameScene.Game.MagicBox != null && GameScene.Game.MagicBox.Visible)
            {
                MagicInfo info = Icons[key].Tag as MagicInfo;
                if (GameScene.Game.MagicBox.SelectedMagic != null && GameScene.Game.MagicBox.SelectedMagic.Info != null)
                {
                    if (GameScene.Game.MagicBox.SelectedMagic.Info == info)
                        key = SpellKey.None;
                    info = GameScene.Game.MagicBox.SelectedMagic.Info;
                }
                else if (info != null)
                {
                    key = SpellKey.None;
                }
                else
                    return;

                ClientUserMagic magic;
                if (!MapObject.User.Magics.TryGetValue(info, out magic)) return;

                switch (SpellSet)
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
                CEnvir.Enqueue(new C.MagicKey { Magic = magic.Info.Magic, Set1Key = magic.Set1Key, Set2Key = magic.Set2Key, Set3Key = magic.Set3Key, Set4Key = magic.Set4Key });
                GameScene.Game.MagicBox.SelectedMagic?.Refresh();
                UpdateIcons();
            }
            else
            {
                MagicInfo info = Icons[key].Tag as MagicInfo;

                if (info == null) return;

                if (GameScene.Game.CanContinuouslyMagic(info.Magic))
                {
                    GameScene.Game.ContinuouslyMagicType = info.Magic;
                    GameScene.Game.ContinuouslyMagic = true;
                }
                else
                {
                    GameScene.Game.ContinuouslyMagic = false;
                }

                if (CanLockMagic(info.Magic))
                {
                    //切换不同技能 激活锁定
                    if ((SpellKey)LockMagicAnimation.Tag != key)
                    {
                        GameScene.Game.LockMagicType = info.Magic;
                        LockMagicAnimation.Tag = key;
                        LockMagicAnimation.Location = new Point(Icons[key].Location.X + ((Icons[key].Size.Width - LockMagicAnimation.Size.Width) / 2), Icons[key].Location.Y + ((Icons[key].Size.Height - LockMagicAnimation.Size.Height) / 2));
                        LockMagicAnimation.Visible = true;

                        if (!GameScene.Game.CanContinuouslyMagic(info.Magic))
                            return;
                    }
                    //else  //按同一技能 开始和取消锁定
                    //{
                    //    LockMagicAnimation.Visible = !LockMagicAnimation.Visible;
                    //    if (!LockMagicAnimation.Visible)
                    //    {
                    //        GameScene.Game.LockMagicType = MagicType.None;
                    //        LockMagicAnimation.Tag = SpellKey.None;
                    //    }
                    //    else
                    //    {
                    //        GameScene.Game.LockMagicType = info.Magic;
                    //        LockMagicAnimation.Tag = key;
                    //    }
                    //}
                }
                else  //如果切换非锁定魔法 取消现有锁定
                {
                    LockMagicAnimation.Visible = false;
                    GameScene.Game.LockMagicType = MagicType.None;
                    LockMagicAnimation.Tag = SpellKey.None;
                    //GameScene.Game.AutoAttack = false; //取消自动打怪
                }

                if (Is8DirMagic(info.Magic))
                {
                    GameScene.Game.MapControl.MouseLocation = ZoomRate == 1F ? Functions.Move(new Point((CEnvir.Target.Width / 2), (CEnvir.Target.Height / 2)), LineMagicDir, 48) : Functions.Move(new Point((int)Math.Round(((CEnvir.Target.Width / 2) - UI_Offset_X) / ZoomRate), (int)Math.Round((CEnvir.Target.Height / 2) / ZoomRate)), LineMagicDir, 48);
                    GameScene.Game.UseMagic(info.Magic);
                    return;
                }

                if (info.Magic == MagicType.PoisonDust)
                {
                    UsePoisonDustMagic(PoisonDustType, true);
                    return;
                }

                if (GameScene.Game.MagicObject == null || GameScene.Game.MagicObject.Dead)
                {
                    GameScene.Game.SetMagicTargetObject(SelectPlayer ? ObjectType.Player : ObjectType.Monster);
                }

                GameScene.Game.UseMagic(key);
            }
        }
        private void Icons_DoubleTap(SpellKey key)
        {
            if (GameScene.Game.Observer) return;

            MagicInfo info = Icons[key].Tag as MagicInfo;

            if (info == null) return;

            if (CanLockMagic(info.Magic))
            {
                //双击取消锁定技能
                LockMagicAnimation.Visible = false;
                GameScene.Game.LockMagicType = MagicType.None;
                LockMagicAnimation.Tag = SpellKey.None;
            }
        }

        private void UsePoisonDustMagic(int needShape, bool switchob)
        {
            int shape = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].Item?.Info.Shape ?? -1;

            if (needShape != shape || shape == -1)
            {
                for (int i = 0; i < GameScene.Game.Inventory.Length; i++)
                {
                    if ((GameScene.Game.Inventory[i]?.Info.ItemType ?? 0) == ItemType.Poison && (GameScene.Game.Inventory[i]?.Info.Shape ?? -1) != shape && needShape == -1
                        || (GameScene.Game.Inventory[i]?.Info.ItemType ?? 0) == ItemType.Poison && (GameScene.Game.Inventory[i]?.Info.Shape ?? -1) == needShape
                        )
                    {
                        DXItemCell[] grid = GameScene.Game.InventoryBox.Grid.Grid;
                        GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].ToEquipment(grid[i]);
                        break;
                    }
                }
            }

            shape = GameScene.Game.CharacterBox.Grid[(int)EquipmentSlot.Poison].Item?.Info.Shape ?? -1;
            if (shape == -1)
            {
                GameScene.Game.ReceiveChat($"GameScene.AutoPoison".Lang(), MessageType.Hint);
            }
            else
            {
                if (GameScene.Game.MagicObject == null || GameScene.Game.MagicObject.Dead || (switchob && ((shape == 0 && (GameScene.Game.MagicObject.Poison & PoisonType.Green) == PoisonType.Green) || (shape == 1 && (GameScene.Game.MagicObject.Poison & PoisonType.Red) == PoisonType.Red))))
                    GameScene.Game.SetMagicTargetObject(SelectPlayer ? ObjectType.Player : ObjectType.Monster, shape);

                GameScene.Game.UseMagic(MagicType.PoisonDust);
            }
        }
        /// <summary>
        /// 更新图标
        /// </summary>
        public void UpdateIcons()
        {
            //遍历 键值<按键，图像> 对应 图标
            foreach (KeyValuePair<SpellKey, DXButton> pair in Icons)
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
                    pair.Value.Index = 200 + magic.Info.Icon;   //对应的序号=技能信息里的图标

                    if (CanLockMagic(magic.Info.Magic))
                    {
                        IconsOrder[pair.Key].Visible = true;

                        if ((SpellKey)LockMagicAnimation.Tag == pair.Key)
                        {
                            LockMagicAnimation.Location = new Point(pair.Value.Location.X + ((pair.Value.Size.Width - LockMagicAnimation.Size.Width) / 2), pair.Value.Location.Y + ((pair.Value.Size.Height - LockMagicAnimation.Size.Height) / 2));
                            LockMagicAnimation.Visible = true;
                        }
                    }
                    else
                    {
                        IconsOrder[pair.Key].Visible = false;

                        if ((SpellKey)LockMagicAnimation.Tag == pair.Key)
                            LockMagicAnimation.Visible = false;
                    }

                    if (Is8DirMagic(magic.Info.Magic))
                    {
                        IconsDir[pair.Key].Visible = true;
                    }
                    else
                    {
                        IconsDir[pair.Key].Visible = false;
                    }
                }
                else
                {
                    pair.Value.Index = -1;
                    Cooldowns[pair.Key].Visible = false;  //技能值冷却时间
                    IconsOrder[pair.Key].Visible = false;
                    IconsDir[pair.Key].Visible = false;

                    if ((SpellKey)LockMagicAnimation.Tag == pair.Key)
                        LockMagicAnimation.Visible = false;
                }

            }

        }

        private bool CanLockMagic(MagicType type)
        {
            switch (type)
            {
                case MagicType.Repulsion:
                case MagicType.Teleportation:
                case MagicType.AdamantineFireBall:
                case MagicType.ThunderBolt:
                case MagicType.IceBlades:
                case MagicType.Cyclone:
                case MagicType.MagicShield:
                case MagicType.Renounce:
                case MagicType.ScortchedEarth:
                case MagicType.GreaterFrozenEarth:
                case MagicType.FrozenEarth:
                case MagicType.LightningBeam:
                case MagicType.BlowEarth:

                case MagicType.Heal:
                case MagicType.SpiritSword:
                case MagicType.PoisonDust:
                case MagicType.ExplosiveTalisman:
                case MagicType.EvilSlayer:
                //case MagicType.SummonSkeleton:
                case MagicType.Invisibility:
                case MagicType.GreaterEvilSlayer:
                //case MagicType.MagicResistance:
                //case MagicType.MassInvisibility:
                //case MagicType.Resilience:
                //case MagicType.TrapOctagon:
                //case MagicType.ElementalSuperiority:
                //case MagicType.SummonShinsu:
                //case MagicType.MassHeal:
                //case MagicType.SummonJinSkeleton:
                //case MagicType.BloodLust:
                case MagicType.TaoistCombatKick:
                case MagicType.Resurrection:
                case MagicType.Purification:
                case MagicType.StrengthOfFaith:
                case MagicType.Transparency:
                case MagicType.CelestialLight:

                case MagicType.Swordsmanship:
                case MagicType.Slaying:
                case MagicType.Thrusting:
                case MagicType.HalfMoon:
                case MagicType.ShoulderDash:
                case MagicType.FlamingSword:
                case MagicType.DragonRise:
                case MagicType.BladeStorm:
                case MagicType.DestructiveSurge:
                case MagicType.Defiance:
                case MagicType.Might:
                    return false;
                default:
                    return true;
            }
        }

        private bool Is8DirMagic(MagicType type)
        {
            switch (type)
            {
                case MagicType.ScortchedEarth:  //地狱火
                case MagicType.GreaterFrozenEarth: //魄冰刺
                case MagicType.FrozenEarth: //冰沙掌
                case MagicType.LightningBeam: //疾光电影
                case MagicType.BlowEarth: //风震天

                case MagicType.TaoistCombatKick: //空拳刀法
                case MagicType.SummonSkeleton: //召唤骷髅
                case MagicType.SummonShinsu: //召唤神兽
                case MagicType.SummonJinSkeleton: //召唤超强骷髅

                case MagicType.ShoulderDash: //野蛮冲撞
                    return true;
                default:
                    return false;
            }
        }

        public override void Process()
        {
            base.Process();

            if (!Visible) return;

            foreach (KeyValuePair<SpellKey, DXButton> pair in Icons)
            {
                MagicInfo info = pair.Value.Tag as MagicInfo;

                if (info == null)
                {
                    if (GameScene.Game.MagicBox != null && GameScene.Game.MagicBox.Visible)
                        pair.Value.Index = 65;
                    else
                        pair.Value.Index = -1;
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

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _SpellSet = 1;
                SpellSetChanged = null;

                if (MagicButton != null)
                {
                    if (!MagicButton.IsDisposed)
                        MagicButton.Dispose();

                    MagicButton = null;
                }

                if (AttackButton != null)
                {
                    if (!AttackButton.IsDisposed)
                        AttackButton.Dispose();

                    AttackButton = null;
                }

                //if (ForceAttackButton != null)
                //{
                //    if (!ForceAttackButton.IsDisposed)
                //        ForceAttackButton.Dispose();

                //    ForceAttackButton = null;
                //}

                if (SelectPlayerButton != null)
                {
                    if (!SelectPlayerButton.IsDisposed)
                        SelectPlayerButton.Dispose();

                    SelectPlayerButton = null;
                }

                if (SelectMonsterButton != null)
                {
                    if (!SelectMonsterButton.IsDisposed)
                        SelectMonsterButton.Dispose();

                    SelectMonsterButton = null;
                }

                //if (HorseButton != null)
                //{
                //    if (!HorseButton.IsDisposed)
                //        HorseButton.Dispose();

                //    HorseButton = null;
                //}

                //if (HarvestButton != null)
                //{
                //    if (!HarvestButton.IsDisposed)
                //        HarvestButton.Dispose();

                //    HarvestButton = null;
                //}

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                //if (DownButton != null)
                //{
                //    if (!DownButton.IsDisposed)
                //        DownButton.Dispose();

                //    DownButton = null;
                //}

                //if (SetLabel != null)
                //{
                //    if (!SetLabel.IsDisposed)
                //        SetLabel.Dispose();

                //    SetLabel = null;
                //}

                foreach (KeyValuePair<SpellKey, DXButton> pair in Icons)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                Icons.Clear();
                Icons = null;

                foreach (KeyValuePair<SpellKey, DXLabel> pair in Cooldowns)
                {
                    if (pair.Value == null) continue;
                    if (pair.Value.IsDisposed) continue;

                    pair.Value.Dispose();
                }

                Cooldowns.Clear();
                Cooldowns = null;

            }

            if (InventoryButton != null)
            {
                if (!InventoryButton.IsDisposed)
                    InventoryButton.Dispose();

                InventoryButton = null;
            }

            if (CharacterButton != null)
            {
                if (!CharacterButton.IsDisposed)
                    CharacterButton.Dispose();

                CharacterButton = null;
            }

            if (LockMagicAnimation != null)
            {
                if (!LockMagicAnimation.IsDisposed)
                    LockMagicAnimation.Dispose();

                LockMagicAnimation = null;
            }

            //if (SelectPlayerPanel != null)
            //{
            //    if (!SelectPlayerPanel.IsDisposed)
            //        SelectPlayerPanel.Dispose();

            //    SelectPlayerPanel = null;
            //}

            //if (MinBlood != null)
            //{
            //    if (!MinBlood.IsDisposed)
            //        MinBlood.Dispose();

            //    MinBlood = null;
            //}

            //if (NeerBy != null)
            //{
            //    if (!NeerBy.IsDisposed)
            //        NeerBy.Dispose();

            //    NeerBy = null;
            //}

            //if (AttackBack != null)
            //{
            //    if (!AttackBack.IsDisposed)
            //        AttackBack.Dispose();

            //    AttackBack = null;
            //}
        }
        #endregion
    }
    public enum SelectPlayerType : byte
    {
        /// <summary>
        /// 附近
        /// </summary>
        NeerBy,
        /// <summary>
        /// 残血
        /// </summary>
        MinBlood,
        /// <summary>
        /// 反击
        /// </summary>
        AttackBack,
    }
}
