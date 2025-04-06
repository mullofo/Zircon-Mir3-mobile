using Client.Controls;
using Client.Envir;
using Library;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Point = System.Drawing.Point;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 手机UI左边中间按钮区域
    /// </summary>
    public sealed class PhoneLeftCentButtonsPanel : DXControl
    {
        #region Properties
        /// <summary>
        /// 切换按钮
        /// </summary>
        private DXButton SwitchButton;
        private bool switchsmall;
        /// <summary>
        /// D键按钮
        /// </summary>
        private DXButton DKeyButton;
        /// <summary>
        /// 任务按钮
        /// </summary>
        //private DXButton QuestButton;
        /// <summary>
        /// 邮件按钮
        /// </summary>
        private DXButton MailButton;
        /// <summary>
        /// 组队按钮
        /// </summary>
        private DXButton GroupButton;
        /// <summary>
        /// 行会按钮
        /// </summary>
        private DXButton GuildButton;

        #endregion

        public PhoneLeftCentButtonsPanel()
        {
            DrawTexture = false;
            Size = new Size(40, 500);
            PassThrough = true;  //穿透开启

            SwitchButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 83,
            };
            SwitchButton.TouchUp += (o, e) =>
            {
                switchsmall = !switchsmall;
                SwitchButton.Index = switchsmall ? 84 : 83;
                DKeyButton.Visible = !switchsmall;
                MailButton.Visible = !switchsmall;
                GroupButton.Visible = !switchsmall;
                GuildButton.Visible = !switchsmall;
            };

            DKeyButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 89,
            };
            DKeyButton.Location = new Point(SwitchButton.Location.X, SwitchButton.Location.Y + SwitchButton.Size.Height);
            DKeyButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                if (GameScene.Game.NPCBox.Visible == true)
                {
                    GameScene.Game.NPCBox.Visible = false;
                    return;
                }
                if (GameScene.Game.NPCDKeyBox.Visible == true)
                {
                    GameScene.Game.NPCDKeyBox.Visible = false;
                    return;
                }
                //发包 D键
                CEnvir.Enqueue(new C.DKey { });
            };

            //QuestButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 89,
            //};
            //QuestButton.Location = new Point(DKeyButton.Location.X, DKeyButton.Location.Y + DKeyButton.Size.Height);
            //QuestButton.Tap += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.QuestBox.Visible = !GameScene.Game.QuestBox.Visible;
            //};

            MailButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 88,
            };
            MailButton.Location = new Point(DKeyButton.Location.X, DKeyButton.Location.Y + DKeyButton.Size.Height);
            MailButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.CommunicationBox.Visible = !GameScene.Game.CommunicationBox.Visible;
            };

            GroupButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 90,
            };
            GroupButton.Location = new Point(MailButton.Location.X, MailButton.Location.Y + MailButton.Size.Height);
            GroupButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                if (GameScene.Game.MapControl.MapInfo.CanPlayName == true) return;
                GameScene.Game.GroupBox.Visible = !GameScene.Game.GroupBox.Visible;
            };

            GuildButton = new DXButton
            {
                Parent = this,
                LibraryFile = LibraryFile.PhoneUI,
                Index = 87,
            };
            GuildButton.Location = new Point(GroupButton.Location.X, GroupButton.Location.Y + GroupButton.Size.Height);
            GuildButton.TouchUp += (o, e) =>
            {
                if (GameScene.Game.Observer) return;
                GameScene.Game.GuildBox.Visible = !GameScene.Game.GuildBox.Visible;
            };

        }


        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (SwitchButton != null)
                {
                    if (!SwitchButton.IsDisposed)
                        SwitchButton.Dispose();

                    SwitchButton = null;
                }

                if (DKeyButton != null)
                {
                    if (!DKeyButton.IsDisposed)
                        DKeyButton.Dispose();

                    DKeyButton = null;
                }

                if (GroupButton != null)
                {
                    if (!GroupButton.IsDisposed)
                        GroupButton.Dispose();

                    GroupButton = null;
                }


                //if (QuestButton != null)
                //{
                //    if (!QuestButton.IsDisposed)
                //        QuestButton.Dispose();

                //    QuestButton = null;
                //}

                if (MailButton != null)
                {
                    if (!MailButton.IsDisposed)
                        MailButton.Dispose();

                    MailButton = null;
                }

                if (GuildButton != null)
                {
                    if (!GuildButton.IsDisposed)
                        GuildButton.Dispose();

                    GuildButton = null;
                }
            }
        }
        #endregion

    }
}
