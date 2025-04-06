using Client.Controls;
using Client.Envir;
using Client.Extentions;
using Client.UserModels;
using Library;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using System.Drawing;
using C = Library.Network.ClientPackets;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

//Cleaned
namespace Client.Scenes.Views
{
    /// <summary>
    /// 交易功能
    /// </summary>
    public sealed class TradeDialog : DXWindow
    {
        #region Properties
        public DXImageControl TradeGround;
        public DXLabel UserLabel, PlayerLabel;
        public DXItemGrid UserGrid, PlayerGrid;
        public DXLabel UserGoldLabel, PlayerGoldLabel, PointOu;
        public DXButton ConfirmButton;

        public ClientUserItem[] PlayerItems;
        public bool IsTrading;

        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            UserGrid.ClearLinks();

            if (!IsTrading || GameScene.Game.Observer) return;

            CEnvir.Enqueue(new C.TradeClose());
        }

        public override WindowType Type => WindowType.TradeBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 交易界面
        /// </summary>
        public TradeDialog()
        {
            //TitleLabel.Text = "交易窗口";
            HasTitle = false;             //字幕标题不显示
            HasFooter = false;            //不显示页脚
            HasTopBorder = false;         //不显示上边框
            TitleLabel.Visible = false;   //不显示标题
            CloseButton.Visible = false;  //不显示关闭按钮            
            AllowResize = false;          //不允许调整大小
            Opacity = 0F;

            TradeGround = new DXImageControl        //交易底图
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1390,
                Parent = this,
                IsControl = true,
                PassThrough = true,
                ImageOpacity = 0.85F,
                Location = new Point(0, 0)
            };

            Location = new Point(40, 40);

            UserGrid = new DXItemGrid      //用户道具框
            {
                GridSize = new Size(5, 5),
                Parent = this,
                Border = false,
                Location = new Point(ClientArea.X + 4, ClientArea.Y + 42),
                GridType = GridType.TradeUser,
                Linked = true,
            };

            foreach (DXItemCell cell in UserGrid.Grid)
            {
                cell.LinkChanged += (o, e) =>
                {
                    cell.ReadOnly = cell.Item != null;

                    if (cell.Item == null || GameScene.Game.Observer) return;

                    CEnvir.Enqueue(new C.TradeAddItem
                    {
                        Cell = new CellLinkInfo { Slot = cell.Link.Slot, Count = cell.LinkedCount, GridType = cell.Link.GridType }
                    });
                };
            }

            PlayerGrid = new DXItemGrid   //玩家道具框
            {
                GridSize = new Size(5, 5),
                Parent = this,
                Border = false,
                Location = new Point(UserGrid.Location.X + UserGrid.Size.Width + 20 + 40, ClientArea.Y + 42),
                ItemGrid = PlayerItems = new ClientUserItem[25],
                GridType = GridType.TradePlayer,
                ReadOnly = true,
            };

            UserLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "角色自己".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                ForeColour = Color.FromArgb(204, 204, 255),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(110, 20),
            };
            //UserLabel.SizeChanged += (o, e) => UserLabel.Location = new Point(125, ClientArea.Y);
            UserLabel.Location = new Point(120, ClientArea.Y - 5);


            PlayerLabel = new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Text = "交易对象".Lang(),
                Parent = this,
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                ForeColour = Color.FromArgb(204, 204, 255),
                Outline = true,
                OutlineColour = Color.Black,
                IsControl = false,
                Size = new Size(110, 20),
            };
            //PlayerLabel.SizeChanged += (o, e) => PlayerLabel.Location = new Point(270, ClientArea.Y);
            PlayerLabel.Location = new Point(265, ClientArea.Y - 5);

            UserGoldLabel = new DXLabel   //用户金币标签
            {
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Parent = this,
                Location = new Point(UserGrid.Location.X + 30, UserGrid.Location.Y + UserGrid.Size.Height + 43),
                Text = "0",
                Size = new Size(UserGrid.Size.Width - 61, 20),
                Sound = SoundIndex.GoldPickUp
            };
            UserGoldLabel.MouseClick += UserGoldLabel_MouseClick;

            PointOu = new DXLabel
            {
                Font = new Font(Config.FontName, CEnvir.FontSize(8F), FontStyle.Regular),
                ForeColour = Color.FromArgb(204, 204, 255),
                Parent = this,
                Text = "在下面输入金币数量".Lang(),
            };
            PointOu.Location = new Point(60, 271);

            PlayerGoldLabel = new DXLabel   //玩家金币标签
            {
                AutoSize = false,
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.Right,
                Parent = this,
                Location = new Point(PlayerGrid.Location.X + 33, UserGrid.Location.Y + UserGrid.Size.Height + 43),
                Text = "0",
                Size = new Size(UserGrid.Size.Width - 61, 20),
                Sound = SoundIndex.GoldPickUp
            };

            /*new DXLabel
            {
                AutoSize = false,
                Border = true,
                BorderColour = Color.FromArgb(99, 83, 50),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                Parent = this,
                Location = new Point(PlayerGrid.Location.X + 1, UserGrid.Location.Y + UserGrid.Size.Height + 5),
                Text = "金币",
                Size = new Size(58, 20),
                IsControl = false,
            };*/

            ConfirmButton = new DXButton   //确认交易按钮
            {
                LibraryFile = LibraryFile.UI1,
                Index = 1392,
                Parent = this,
                Location = new Point(226, 275),
                Hint = "确认交易".Lang(),
            };
            ConfirmButton.MouseClick += (o, e) =>   //鼠标点击时
            {
                if (GameScene.Game.Observer) return;

                ConfirmButton.Enabled = false;

                CEnvir.Enqueue(new C.TradeConfirm());
            };

            SetClientSize(new Size(475, 306));
        }

        #region Methods
        /// <summary>
        /// 鼠标点击角色金币标签
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserGoldLabel_MouseClick(object sender, MouseEventArgs e)
        {
            if (GameScene.Game.Observer) return;

            DXItemAmountWindow window = new DXItemAmountWindow("交易金币".Lang(), new ClientUserItem(Globals.GoldInfo, GameScene.Game.User.Gold));

            window.ConfirmButton.MouseClick += (o1, e1) =>
            {
                if (window.Amount <= 0) return;

                CEnvir.Enqueue(new C.TradeAddGold { Gold = window.Amount });
            };
        }
        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            UserGoldLabel.Text = "0";
            PlayerGoldLabel.Text = "0";
            ConfirmButton.Enabled = true;

            IsTrading = false;

            foreach (DXItemCell cell in PlayerGrid.Grid)
                cell.Item = null;
        }

        #endregion

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                IsTrading = false;
                PlayerItems = null;

                if (UserLabel != null)
                {
                    if (!UserLabel.IsDisposed)
                        UserLabel.Dispose();

                    UserLabel = null;
                }

                if (PlayerLabel != null)
                {
                    if (!PlayerLabel.IsDisposed)
                        PlayerLabel.Dispose();

                    PlayerLabel = null;
                }

                if (UserGrid != null)
                {
                    if (!UserGrid.IsDisposed)
                        UserGrid.Dispose();

                    UserGrid = null;
                }

                if (PlayerGrid != null)
                {
                    if (!PlayerGrid.IsDisposed)
                        PlayerGrid.Dispose();

                    PlayerGrid = null;
                }

                if (UserGoldLabel != null)
                {
                    if (!UserGoldLabel.IsDisposed)
                        UserGoldLabel.Dispose();

                    UserGoldLabel = null;
                }

                if (PlayerGoldLabel != null)
                {
                    if (!PlayerGoldLabel.IsDisposed)
                        PlayerGoldLabel.Dispose();

                    PlayerGoldLabel = null;
                }

                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();

                    ConfirmButton = null;
                }

                if (PointOu != null)
                {
                    if (!PointOu.IsDisposed)
                        PointOu.Dispose();

                    PointOu = null;
                }
            }
        }
        #endregion
    }
}
