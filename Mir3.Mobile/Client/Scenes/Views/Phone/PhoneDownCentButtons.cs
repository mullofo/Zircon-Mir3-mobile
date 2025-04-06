using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Library;
using System.Drawing;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 屏幕中下按钮栏
    /// </summary>
    public sealed class PhoneDownCentPanel : DXControl
    {
        #region Properties
        /// <summary>
        /// 挖肉按钮
        /// </summary>
        private DXButton HarvestButton;
        /// <summary>
        /// 自动按钮
        /// </summary>
        public DXButton AutoAttackButton;
        /// <summary>
        /// 骑马按钮
        /// </summary>
        private DXButton HorseButton;
        /// <summary>
        /// 背包按钮
        /// </summary>
        //private DXButton InventoryButton;
        /// <summary>
        /// 技能按钮
        /// </summary>
        //private DXButton MagicButton;
        /// <summary>
        /// 商城按钮
        /// </summary>
        //private DXButton MarketPlace;
        /// <summary>
        /// 排行榜按钮
        /// </summary>
        private DXButton RankingButton;
        /// <summary>
        /// 设置按钮
        /// </summary>
        private DXButton ConfigButton;
        /// <summary>
        /// 聊天按钮
        /// </summary>
        private DXButton ChatButton;

        #endregion

        /// <summary>
        /// 屏幕中下按钮栏
        /// </summary>
        public PhoneDownCentPanel()
        {
            DrawTexture = false;
            PassThrough = true;  //穿透开启

            HorseButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 154,
            };
            HorseButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;

                if (CEnvir.Now < GameScene.Game.User.NextActionTime || GameScene.Game.User.ActionQueue.Count > 0) return;
                if (CEnvir.Now < GameScene.Game.User.ServerTime) return;
                if (CEnvir.Now < MapObject.User.CombatTime.AddSeconds(10) && !GameScene.Game.Observer && GameScene.Game.User.Horse == HorseType.None)
                {
                    GameScene.Game.ReceiveChat("战斗中无法骑马".Lang(), MessageType.System);
                    return;
                }

                GameScene.Game.User.ServerTime = CEnvir.Now.AddSeconds(5);
                CEnvir.Enqueue(new C.Mount());
            };

            AutoAttackButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 148,
                ForeColour = Color.DarkGray,
            };
            AutoAttackButton.Location = new Point(HorseButton.Location.X + HorseButton.Size.Width + 5, HorseButton.Location.Y);
            AutoAttackButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                if (Configs.BigPatchConfig.AndroidPlayer)
                {
                    GameScene.Game.ReceiveChat("你已开启自动挂机，无法打开自动打怪".Lang(), MessageType.System);   //提示并跳过
                    return;
                }

                GameScene.Game.AutoAttack = !GameScene.Game.AutoAttack;

                AutoAttackButton.ForeColour = GameScene.Game.AutoAttack ? Color.White : Color.DarkGray;

                if (!GameScene.Game.AutoAttack)
                {
                    MapObject.TargetObject = null;
                    GameScene.Game.FocusObject = null;
                    MapObject.MagicObject = null;
                    MapObject.MouseObject = null;
                    GameScene.Game.AutoAttack = false;
                    GameScene.Game.MagicBarBox.PhysicalAttack = false;
                }
                else if (GameScene.Game.User.Class == MirClass.Warrior)
                    GameScene.Game.MagicBarBox.PhysicalAttack = true;
            };

            HarvestButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 153,
            };
            HarvestButton.Location = new Point(AutoAttackButton.Location.X + AutoAttackButton.Size.Width + 5, AutoAttackButton.Location.Y);
            HarvestButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.MapControl.Harvest = !GameScene.Game.MapControl.Harvest;
            };

            //InventoryButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 152,
            //};
            //InventoryButton.Location = new Point(HarvestButton.Location.X + HarvestButton.Size.Width + 5, HarvestButton.Location.Y);
            //InventoryButton.Tap += (o, e) => GameScene.Game.InventoryBox.Visible = !GameScene.Game.InventoryBox.Visible;

            //MarketPlace = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 151,
            //};
            //MarketPlace.Location = new Point(InventoryButton.Location.X + InventoryButton.Size.Width + 5, InventoryButton.Location.Y);
            //MarketPlace.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.MarketPlaceBox.Visible = !GameScene.Game.MarketPlaceBox.Visible;
            //};

            //MagicButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 150,
            //};
            //MagicButton.Location = new Point(MarketPlace.Location.X + MarketPlace.Size.Width + 5, MarketPlace.Location.Y);
            //MagicButton.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.MagicBox.Visible = !GameScene.Game.MagicBox.Visible;
            //};

            RankingButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 146,
            };
            RankingButton.Location = new Point(HarvestButton.Location.X + HarvestButton.Size.Width + 5, HarvestButton.Location.Y);
            RankingButton.TouchUp += (o, e) =>
            {
                if (!CEnvir.ClientControl.RankingShowCheck) return;  //排行榜设置不显示就不设置快捷
                GameScene.Game.RankingBox.Visible = !GameScene.Game.RankingBox.Visible && CEnvir.Connection != null;
                //GameScene.Game.RankingBox.Location = new Point((GameScene.Game.Size.Width - GameScene.Game.RankingBox.Size.Width) / 2, (GameScene.Game.Size.Height - GameScene.Game.RankingBox.Size.Height) / 2);
            };

            ConfigButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 147,
            };
            ConfigButton.Location = new Point(RankingButton.Location.X + RankingButton.Size.Width + 5, RankingButton.Location.Y);
            ConfigButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;   //如果是观察者 返回
                GameScene.Game.ConfigBox.Visible = !GameScene.Game.ConfigBox.Visible;
                //GameScene.Game.ConfigBox.Location = new Point((GameScene.Game.Size.Width - GameScene.Game.ConfigBox.Size.Width) / 2, (GameScene.Game.Size.Height - GameScene.Game.ConfigBox.Size.Height) / 2);
            };

            ChatButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 149,
            };
            ChatButton.Location = new Point(ConfigButton.Location.X + ConfigButton.Size.Width + 5, ConfigButton.Location.Y);
            ChatButton.Tap += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.ChatBox.Visible = !GameScene.Game.ChatBox.Visible;
            };

            Size = new Size(ChatButton.Location.X + ChatButton.Size.Width, ChatButton.Size.Height);
        }
        #region Methods

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (HorseButton != null)
                {
                    if (!HorseButton.IsDisposed)
                        HorseButton.Dispose();

                    HorseButton = null;
                }

                if (HarvestButton != null)
                {
                    if (!HarvestButton.IsDisposed)
                        HarvestButton.Dispose();

                    HarvestButton = null;
                }

                //if (InventoryButton != null)
                //{
                //    if (!InventoryButton.IsDisposed)
                //        InventoryButton.Dispose();

                //    InventoryButton = null;
                //}

                //if (MagicButton != null)
                //{
                //    if (!MagicButton.IsDisposed)
                //        MagicButton.Dispose();

                //    MagicButton = null;
                //}

                //if (MarketPlace != null)
                //{
                //    if (!MarketPlace.IsDisposed)
                //        MarketPlace.Dispose();

                //    MarketPlace = null;
                //}

                if (RankingButton != null)
                {
                    if (!RankingButton.IsDisposed)
                        RankingButton.Dispose();

                    RankingButton = null;
                }

                if (ConfigButton != null)
                {
                    if (!ConfigButton.IsDisposed)
                        ConfigButton.Dispose();

                    ConfigButton = null;
                }

                if (ChatButton != null)
                {
                    if (!ChatButton.IsDisposed)
                        ChatButton.Dispose();

                    ChatButton = null;
                }

            }
        }
        #endregion
    }
}
