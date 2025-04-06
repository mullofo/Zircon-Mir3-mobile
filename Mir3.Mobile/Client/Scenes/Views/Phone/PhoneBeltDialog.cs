using Client.Controls;
using Library;
using System.Drawing;
using System.Linq;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 药品快捷栏功能
    /// </summary>
    public sealed class PhoneBeltDialog : DXControl
    {
        #region Properties

        public ClientBeltLink[] Links;

        public DXItemGrid Grid;

        public DXImageControl BletBackground;

        /// <summary>
        /// 骑马按钮
        /// </summary>
        //private DXButton HorseButton;
        /// <summary>
        /// 聊天按钮
        /// </summary>
        //private DXButton ChatButton;
        #endregion

        /// <summary>
        /// 药品快捷栏界面
        /// </summary>
        public PhoneBeltDialog()
        {
            DrawTexture = false;
            PassThrough = true;  //穿透开启

            Links = new ClientBeltLink[Globals.MaxBeltCount];
            for (int i = 0; i < Globals.MaxBeltCount; i++)
                Links[i] = new ClientBeltLink { Slot = i };

            //HorseButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 154,
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

            //ChatButton = new DXButton
            //{
            //    Parent = this,
            //    LibraryFile = LibraryFile.PhoneUI,
            //    Index = 149,
            //};
            //ChatButton.Location = new Point(HorseButton.Location.X + HorseButton.Size.Width + 5, HorseButton.Location.Y);
            //ChatButton.TouchUp += (o, e) =>
            //{
            //    if (GameScene.Game.Observer) return;
            //    GameScene.Game.ChatBox.Visible = !GameScene.Game.ChatBox.Visible;
            //};

            BletBackground = new DXImageControl   //背景图
            {
                FixedSize = true,
                Size = new Size(180, 48),
                Index = 998,
                LibraryFile = LibraryFile.PhoneUI,
                ImageOpacity = 0.85F,
                Parent = this,
            };
            //BletBackground.Location = new Point(ChatButton.Location.X + ChatButton.Size.Width + 5, ChatButton.Location.Y);

            Grid = new DXItemGrid   //道具格子
            {
                Parent = BletBackground,
                GridSpace = new Size(7, 0),
                Opacity = 0F,
                Border = false,
                GridSize = new Size(4, 1),   //数量6个
                GridType = GridType.Belt,
                AllowLink = false,
            };
            Grid.Location = new Point(4, (BletBackground.Size.Height - Grid.Size.Height) / 2);

            Size = new Size(BletBackground.Location.X + BletBackground.Size.Width, 48);

        }

        #region Methods

        /// <summary>
        /// 更新道具链接
        /// </summary>
        public void UpdateLinks()
        {
            if (Grid == null) return;
            foreach (ClientBeltLink link in Links)
            {
                if (link.Slot < 0 || link.Slot >= Grid.Grid.Length) continue;

                if (link.LinkInfoIndex > 0)
                    Grid.Grid[link.Slot].QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == link.LinkInfoIndex);
                else if (link.LinkItemIndex >= 0)
                    Grid.Grid[link.Slot].QuickItem = GameScene.Game.Inventory.FirstOrDefault(x => x?.Index == link.LinkItemIndex);
            }
            //锁死4个快捷栏药品
            //Grid.Grid[0].QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == 243); //万年雪霜
            //Grid.Grid[1].QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == 196); //强效太阳水
            //Grid.Grid[2].QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == 135); //回城卷
            //Grid.Grid[3].QuickInfo = Globals.ItemInfoList.Binding.FirstOrDefault(x => x.Index == 165); //随机传送卷
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                //if (HorseButton != null)
                //{
                //    if (!HorseButton.IsDisposed)
                //        HorseButton.Dispose();

                //    HorseButton = null;
                //}

                //if (ChatButton != null)
                //{
                //    if (!ChatButton.IsDisposed)
                //        ChatButton.Dispose();

                //    ChatButton = null;
                //}

                if (BletBackground != null)
                {
                    if (!BletBackground.IsDisposed)
                        BletBackground.Dispose();

                    BletBackground = null;
                }

                if (Links != null)
                {
                    for (int i = 0; i < Links.Length; i++)
                        Links[i] = null;

                    Links = null;
                }

                if (Grid != null)
                {
                    if (!Grid.IsDisposed)
                        Grid.Dispose();

                    Grid = null;
                }
            }
        }
        #endregion
    }
}
