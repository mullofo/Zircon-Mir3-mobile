using Client.Controls;
using Client.Extentions;
using Client.UserModels;
using Library;
using System.Drawing;
using System.Linq;


namespace Client.Scenes.Views
{
    /// <summary>
    /// 药品快捷栏功能
    /// </summary>
    public sealed class BeltDialog : DXWindow
    {
        #region Properties

        public ClientBeltLink[] Links;

        public DXItemGrid Grid;

        public DXImageControl BeltsGround;

        public DXButton CloseBtn;

        public override WindowType Type => WindowType.BeltBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        #endregion

        /// <summary>
        /// 药品快捷栏界面
        /// </summary>
        public BeltDialog()
        {
            Size = new Size(280, 50);
            Opacity = 0F;
            //CloseButton.Visible = false;
            HasTitle = false;
            PassThrough = true;  //穿透开启
            IgnoreMoveBounds = true;
            CloseButton.Visible = false;

            Links = new ClientBeltLink[Globals.MaxBeltCount];
            for (int i = 0; i < Globals.MaxBeltCount; i++)
                Links[i] = new ClientBeltLink { Slot = i };

            BeltsGround = new DXImageControl   //背景图
            {
                Index = 1210,
                Size = new Size(280, 50),
                LibraryFile = LibraryFile.UI1,
                ImageOpacity = 0.85F,
                Parent = this,
            };
            BeltsGround.MouseDown += Background_MouseDown;

            Grid = new DXItemGrid   //道具格子
            {
                Location = new Point(17, 6),
                Parent = this,
                GridSpace = new Size(4, 0),
                Opacity = 0F,
                Border = false,
                GridSize = new Size(6, 1),   //数量6个
                GridType = GridType.Belt,
                AllowLink = false,
            };
            //Size = new Size(Background.Size.Width, Background.Size.Height + 16);

            CloseBtn = new DXButton  //关闭按钮
            {
                Parent = this,
                Location = new Point(263, 30),
                LibraryFile = LibraryFile.UI1,
                Index = 1211,
                Hint = "关闭".Lang(),
            };
            //CloseBtn.MouseEnter += (o, e) => CloseBtn.Index = 1212;
            //CloseBtn.MouseLeave += (o, e) => CloseBtn.Index = 1211;
            CloseBtn.MouseClick += (o, e) => Visible = false;
        }
        /// <summary>
        /// 背景鼠标事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Background_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnMouseDown(e);
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
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
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

                if (BeltsGround != null)
                {
                    if (!BeltsGround.IsDisposed)
                        BeltsGround.Dispose();

                    BeltsGround = null;
                }

                if (CloseBtn != null)
                {
                    if (!CloseBtn.IsDisposed)
                        CloseBtn.Dispose();

                    CloseBtn = null;
                }
            }
        }
        #endregion
    }
}
