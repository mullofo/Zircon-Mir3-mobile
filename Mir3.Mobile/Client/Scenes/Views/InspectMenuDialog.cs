using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.Models;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using System.Drawing;
using C = Library.Network.ClientPackets;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 查看角色菜单
    /// </summary>
    public sealed class InspectMenuDialog : DXWindow
    {
        /// <summary>
        /// 背景框
        /// </summary>
        public DXImageControl BackGround;
        public DXLabel TitleName;
        /// <summary>
        /// 查看角色按钮
        /// </summary>
        public DXButton InspectButton;
        /// <summary>
        /// 组队按钮
        /// </summary>
        public DXButton GroupButton;
        /// <summary>
        /// 交易按钮
        /// </summary>
        public DXButton TradeButton;
        /// <summary>
        /// 私聊按钮
        /// </summary>
        public DXButton PrivateButton;
        /// <summary>
        /// 邮件按钮
        /// </summary>
        public DXButton MailButton;
        /// <summary>
        /// 好友按钮
        /// </summary>
        public DXButton FriendsButton;
        /// <summary>
        /// 拜师按钮
        /// </summary>
        public DXButton ApprenticeshipButton;
        /// <summary>
        /// 收徒按钮
        /// </summary>
        public DXButton ApprenticeButton;
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public DXButton Close1Button;

        #region Propetries
        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        public InspectMenuDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;
            Opacity = 1F;

            Size = GameInterLibrary.GetSize(3000);

            #region 界面

            BackGround = new DXImageControl
            {
                Parent = this,
                LibraryFile = LibraryFile.GameInter,
                Index = 3000,
                Opacity = 1F,
                Location = new Point(0, 0),
                IsControl = true,
                PassThrough = true,
            };

            TitleName = new DXLabel
            {
                Parent = this,
                Outline = false,
                ForeColour = Color.White,
                Location = new Point(55, 13),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Size = new Size(65, 18),
            };

            InspectButton = new DXButton  //查看角色按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3012,
                Parent = this,
                Location = new Point(27, 40),
            };
            InspectButton.MouseClick += (o, e) =>   //鼠标点击时
             {
                 if (GameScene.Game.Observer) return;

                 PlayerObject player = MapObject.MouseObject as PlayerObject;

                 if (player == null || player == MapObject.User) return;
                 if (CEnvir.Now <= GameScene.Game.InspectTime && player.ObjectID == GameScene.Game.InspectID) return;

                 GameScene.Game.InspectTime = CEnvir.Now.AddMilliseconds(2500);
                 GameScene.Game.InspectID = player.ObjectID;
                 CEnvir.Enqueue(new C.Inspect { Index = player.CharacterIndex });
                 Visible = false;
             };

            GroupButton = new DXButton   //组队按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3017,
                Parent = this,
                Location = new Point(27, 70),
            };
            GroupButton.MouseClick += (o, e) =>   //鼠标点击时
             {
                 if (GameScene.Game.Observer) return;

                 PlayerObject p = MapObject.MouseObject as PlayerObject;

                 if (p != null)
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

                     CEnvir.Enqueue(new C.GroupInvite { Name = p.Name });
                 }
             };

            TradeButton = new DXButton   //交易按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3052,
                Parent = this,
                Location = new Point(27, 100),
            };
            TradeButton.MouseClick += (o, e) =>   //鼠标点击时
             {
                 if (GameScene.Game.Observer) return;
                 //如果玩家 为空  或者 玩家不是面对面交易
                 string ErrorStr = "需要选定目标".Lang();
                 do
                 {
                     if (GameScene.Game.TargetObject == null) break;

                     ErrorStr = "选定目标是玩家".Lang();
                     MapObject ob = GameScene.Game.TargetObject;// as PlayerObject;
                     if (!(ob is PlayerObject)) break;

                     //反方向
                     ErrorStr = "目标必须要与自己面对面".Lang();
                     if (ob.Direction != Functions.ShiftDirection(MapObject.User.Direction, 4)) break;

                     //距一步
                     ErrorStr = "必须靠近目标".Lang();
                     if (1 != Functions.Distance(ob.CurrentLocation, MapObject.User.CurrentLocation)) break;

                     if (Functions.Move(ob.CurrentLocation, ob.Direction, 1) != MapObject.User.CurrentLocation) break;

                     //GameScene.Game.TradeBox.IsVisible = true;

                     CEnvir.Enqueue(new C.TradeRequest());
                     return;
                 } while (false);

                 DXConfirmWindow.Show($"MainPanel.TradeShow".Lang(ErrorStr));

                 GameScene.Game.ReceiveChat("MainPanel.TradeReceiveChat".Lang(), MessageType.System);
             };

            PrivateButton = new DXButton   //私聊按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3022,
                Parent = this,
                Location = new Point(27, 130),
            };
            PrivateButton.MouseClick += (o, e) =>   //鼠标点击时
             {
                 if (GameScene.Game.Observer) return;
                 PlayerObject p = MapObject.MouseObject as PlayerObject;
                 GameScene.Game.ChatTextBox.Visible = true;
                 GameScene.Game.ChatTextBox.StartPM(p.Name);
                 Visible = false;
             };

            MailButton = new DXButton   //邮件按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3027,
                Parent = this,
                Location = new Point(27, 160),
            };
            MailButton.MouseClick += (o, e) =>   //鼠标点击时
             {
                 if (GameScene.Game.Observer) return;
                 PlayerObject p = MapObject.MouseObject as PlayerObject;
                 GameScene.Game.CommunicationBox.Visible = true;
                 GameScene.Game.CommunicationBox.BringToFront();
                 GameScene.Game.CommunicationBox.GoodFriendButton.Opacity = 0F;
                 GameScene.Game.CommunicationBox.ReceivingButton.Opacity = 0F;
                 GameScene.Game.CommunicationBox.PostalMatterButton.Opacity = 1F;
                 GameScene.Game.CommunicationBox.BlacklistButton.Opacity = 0F;
                 GameScene.Game.CommunicationBox.FriendPanel.Visible = false;
                 GameScene.Game.CommunicationBox.ReceivingPanel.Visible = false;
                 GameScene.Game.CommunicationBox.PostalMatterPanel.Visible = true;
                 GameScene.Game.CommunicationBox.BlacklistPanel.Visible = false;
                 GameScene.Game.CommunicationBox.Close1Button.Visible = false;
                 GameScene.Game.CommunicationBox.SendButton.Visible = true;
                 GameScene.Game.CommunicationBox.CollectAllButton.Visible = false;
                 GameScene.Game.CommunicationBox.DeleteAll.Visible = false;
                 GameScene.Game.CommunicationBox.NewButton.Visible = false;
                 GameScene.Game.CommunicationBox.RecipientBox.TextBox.Text = p.Name;
             };

            FriendsButton = new DXButton  //好友按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3032,
                Parent = this,
                Location = new Point(27, 190),
            };
            FriendsButton.MouseClick += (o, e) =>   //鼠标点击时
             {
                 if (GameScene.Game.Observer) return;
                 PlayerObject p = MapObject.MouseObject as PlayerObject;
                 CEnvir.Enqueue(new C.FriendRequest { Name = p.Name });
             };

            ApprenticeshipButton = new DXButton  //拜师按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3037,
                Parent = this,
                Location = new Point(27, 220),
            };
            ApprenticeshipButton.MouseClick += (o, e) =>   //鼠标点击时
             {
             };

            ApprenticeButton = new DXButton    //收徒按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3042,
                Parent = this,
                Location = new Point(27, 250),
            };
            ApprenticeButton.MouseClick += (o, e) =>   //鼠标点击时
             {
             };

            Close1Button = new DXButton       //关闭按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 3047,
                Parent = this,
                Location = new Point(27, 280),
            };
            Close1Button.MouseClick += (o, e) => Visible = false;  //鼠标点击时关闭
            #endregion
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (BackGround != null)
                {
                    if (!BackGround.IsDisposed)
                        BackGround.Dispose();

                    BackGround = null;
                }

                if (TitleName != null)
                {
                    if (!TitleName.IsDisposed)
                        TitleName.Dispose();

                    TitleName = null;
                }

                if (InspectButton != null)
                {
                    if (!InspectButton.IsDisposed)
                        InspectButton.Dispose();

                    InspectButton = null;
                }

                if (GroupButton != null)
                {
                    if (!GroupButton.IsDisposed)
                        GroupButton.Dispose();

                    GroupButton = null;
                }

                if (TradeButton != null)
                {
                    if (!TradeButton.IsDisposed)
                        TradeButton.Dispose();

                    TradeButton = null;
                }

                if (PrivateButton != null)
                {
                    if (!PrivateButton.IsDisposed)
                        PrivateButton.Dispose();

                    PrivateButton = null;
                }

                if (MailButton != null)
                {
                    if (!MailButton.IsDisposed)
                        MailButton.Dispose();

                    MailButton = null;
                }

                if (FriendsButton != null)
                {
                    if (!FriendsButton.IsDisposed)
                        FriendsButton.Dispose();

                    FriendsButton = null;
                }

                if (ApprenticeshipButton != null)
                {
                    if (!ApprenticeshipButton.IsDisposed)
                        ApprenticeshipButton.Dispose();

                    ApprenticeshipButton = null;
                }

                if (ApprenticeButton != null)
                {
                    if (!ApprenticeButton.IsDisposed)
                        ApprenticeButton.Dispose();

                    ApprenticeButton = null;
                }
            }
        }
        #endregion
    }
}
