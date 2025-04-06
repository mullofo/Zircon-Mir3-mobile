using Client.Extentions;
using Client.UserModels;
using Library;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using System;
using System.Drawing;

namespace Client.Controls
{
    /// <summary>
    /// 道具交易窗口
    /// </summary>
    public sealed class DXItemAmountWindow : DXWindow
    {
        #region Properties

        public DXButton ConfirmButton;
        public DXNumberBox AmountBox;

        private DXItemCell ItemCell;

        public long Amount;

        public override WindowType Type => WindowType.ItemAmountBox;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;
        #endregion

        /// <summary>
        /// 道具交易窗口
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="item"></param>
        public DXItemAmountWindow(string caption, ClientUserItem item)
        {
            HasFooter = false;

            TitleLabel.Text = caption;
            SetClientSize(new Size(220, 150));

            Parent = ActiveScene;
            MessageBoxList.Add(this);
            Modal = true;

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);

            ConfirmButton = new DXButton
            {
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "确认".Lang() },
            };
            ConfirmButton.Location = new Point((Size.Width - ConfirmButton.Size.Width) / 2, Size.Height - ConfirmButton.Size.Height - 25);
            ConfirmButton.MouseClick += (o, e) => Dispose();

            ItemCell = new DXItemCell
            {
                Parent = this,
                FixedBorder = true,
                Border = true,
                ItemGrid = new[] { item },
                Slot = 0,
                GridType = GridType.None,
                ReadOnly = true,
            };
            ItemCell.Location = new Point((Size.Width - ItemCell.Size.Width) / 2, 55);

            AmountBox = new DXNumberBox
            {
                Size = new Size(130, 23),
                ValueTextBox = { Location = new Point(40, 2), },
                UpButton = { Location = new Point(107, 0), },
                Parent = this,
                MaxValue = item.Count,
                Change = Math.Max(1, item.Count / 5),
            };
            AmountBox.ValueTextBox.ValueChanged += (o, e) =>
            {
                Amount = AmountBox.Value;

                if (Amount <= 0)
                    AmountBox.ValueTextBox.BorderColour = Color.Red;
                else if (Amount == AmountBox.MaxValue)
                    AmountBox.ValueTextBox.BorderColour = Color.Orange;
                else
                    AmountBox.ValueTextBox.BorderColour = Color.Green;

                ConfirmButton.Enabled = Amount > 0;

                if (item.Info.Effect == ItemEffect.Gold)
                {
                    item.Count = Amount;
                    ItemCell.RefreshItem();
                }
            };
            AmountBox.Location = new Point((Size.Width - AmountBox.Size.Width) / 2, ItemCell.Location.Y + ItemCell.Size.Height + 15);
            AmountBox.Value = 1;
            //AmountBox.ValueTextBox.KeepFocus = true;
            //AmountBox.ValueTextBox.SetFocus();
            AmountBox.ValueTextBox.TextBox.KeyPress += AmountBox_KeyPress;

        }

        #region Methods
        /// <summary>
        /// 金额框 按键
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AmountBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char)Keys.Enter:    //回车键事件
                    e.Handled = true;
                    ConfirmButton.InvokeMouseClick();
                    DXTextBox.ActiveTextBox = null;
                    break;
                case (char)Keys.Escape:   //ESC键事件
                    e.Handled = true;
                    CloseButton.InvokeMouseClick();
                    DXTextBox.ActiveTextBox = null;
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
                if (ConfirmButton != null)
                {
                    if (!ConfirmButton.IsDisposed)
                        ConfirmButton.Dispose();
                    ConfirmButton = null;
                }

                if (AmountBox != null)
                {
                    if (!AmountBox.IsDisposed)
                        AmountBox.Dispose();
                    AmountBox = null;
                }

                if (ItemCell != null)
                {
                    if (!ItemCell.IsDisposed)
                        ItemCell.Dispose();
                    ItemCell = null;
                }
            }
        }
        #endregion
    }
}
