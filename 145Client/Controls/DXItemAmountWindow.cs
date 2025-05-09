﻿using Client.Extentions;
using Client.UserModels;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;


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
            HasFooter = true;

            TitleLabel.Text = caption;
            SetClientSize(new Size(200, DXItemCell.CellHeight + 10));

            Parent = ActiveScene;
            MessageBoxList.Add(this);
            Modal = true;

            Location = new Point((ActiveScene.DisplayArea.Width - DisplayArea.Width) / 2, (ActiveScene.DisplayArea.Height - DisplayArea.Height) / 2);

            ConfirmButton = new DXButton
            {
                Location = new Point(Size.Width / 2 + 10, Size.Height - 43),
                Size = new Size(80, DefaultHeight),
                Parent = this,
                Label = { Text = "确认".Lang() }
            };
            ConfirmButton.MouseClick += (o, e) => Dispose();

            AmountBox = new DXNumberBox
            {
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

            ItemCell = new DXItemCell
            {
                Location = new Point(ClientArea.Location.X + (ClientArea.Width - DXItemCell.CellWidth - AmountBox.Size.Width - 5) / 2, ClientArea.Location.Y + 5),
                Parent = this,
                FixedBorder = true,
                Border = true,
                ItemGrid = new[] { item },
                Slot = 0,
                GridType = GridType.None,
                ReadOnly = true,
            };

            AmountBox.Location = new Point(ItemCell.Location.X + ItemCell.Size.Width + 10, ItemCell.Location.Y + (ItemCell.Size.Height - AmountBox.Size.Height) / 2);

            AmountBox.Value = 1;
            AmountBox.ValueTextBox.KeepFocus = true;
            AmountBox.ValueTextBox.SetFocus();
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
