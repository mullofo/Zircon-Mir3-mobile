using Library;
using System.Drawing;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 小数点数字框控件
    /// </summary>
    public sealed class DXDecimalNumberBox : DXControl
    {
        #region Properties
        public DXButton UpButton, DownButton;
        public DXDecimalNumberTextBox ValueTextBox;
        public decimal Value
        {
            get => ValueTextBox.Value;
            set => ValueTextBox.Value = value;
        }
        public decimal MinValue
        {
            get => ValueTextBox.MinValue;
            set => ValueTextBox.MinValue = value;
        }
        public decimal MaxValue
        {
            get => ValueTextBox.MaxValue;
            set => ValueTextBox.MaxValue = value;
        }
        public decimal Change = 0.01m;
        #endregion

        /// <summary>
        /// 数字框控件
        /// </summary>
        public DXDecimalNumberBox()
        {
            Size = new Size(90, 18);

            ValueTextBox = new DXDecimalNumberTextBox
            {
                Size = new Size(50, 20),
                Location = new Point(19, 1),
                Parent = this,
                TextBox = { Text = "0.00" }
            };


            DownButton = new DXButton   //向下按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 1011,
                Location = new Point(0, 1),
                Parent = this,
            };
            DownButton.MouseClick += (o, e) => ValueTextBox.TextBox.Text = (Value - Change).ToString();

            UpButton = new DXButton    //向上按钮
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 1010,
                Location = new Point(73, 1),
                Parent = this,
            };
            UpButton.MouseClick += (o, e) => ValueTextBox.TextBox.Text = (Value + Change).ToString();
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Change = 0;

                if (UpButton != null)
                {
                    if (!UpButton.IsDisposed)
                        UpButton.Dispose();

                    UpButton = null;
                }

                if (DownButton != null)
                {
                    if (!DownButton.IsDisposed)
                        DownButton.Dispose();

                    DownButton = null;
                }

                if (ValueTextBox != null)
                {
                    if (!ValueTextBox.IsDisposed)
                        ValueTextBox.Dispose();

                    ValueTextBox = null;
                }
            }
        }
        #endregion
    }
}
