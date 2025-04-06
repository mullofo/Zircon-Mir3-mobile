using System;
using System.Globalization;
using System.Threading;

//Cleaned
namespace Client.Controls
{
    /// <summary>
    /// 数字文本输入框
    /// </summary>
    public sealed class DXDecimalNumberTextBox : DXTextBox
    {
        #region Properties

        #region Value
        public decimal Value
        {
            get { return _Value; }
            set
            {
                if (_Value == value) return;

                decimal oldValue = _Value;
                _Value = value;

                OnValueChanged(oldValue, value);
            }
        }
        private decimal _Value;

        public event EventHandler<EventArgs> ValueChanged;
        public void OnValueChanged(decimal oValue, decimal nValue)
        {
            ValueChanged?.Invoke(this, EventArgs.Empty);

            TextBox.Text = Value.ToString("#,##0.00", Thread.CurrentThread.CurrentCulture.NumberFormat);
            //TextBox.SelectionStart = TextBox.Text.Length;
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region MaxValue
        public decimal MaxValue
        {
            get { return _MaxValue; }
            set
            {
                if (_MaxValue == value) return;

                decimal oldValue = _MaxValue;
                _MaxValue = value;

                OnMaxValueChanged(oldValue, value);
            }
        }
        private decimal _MaxValue;
        public event EventHandler<EventArgs> MaxValueChanged;
        public void OnMaxValueChanged(decimal oValue, decimal nValue)
        {
            MaxValueChanged?.Invoke(this, EventArgs.Empty);

            if (Value >= MaxValue)
                TextBox.Text = MaxValue.ToString();
        }
        #endregion

        #region MinValue
        public decimal MinValue
        {
            get { return _MinValue; }
            set
            {
                if (_MinValue == value) return;

                decimal oldValue = _MinValue;
                _MinValue = value;

                OnMinValueChanged(oldValue, value);
            }
        }
        private decimal _MinValue;
        public event EventHandler<EventArgs> MinValueChanged;
        public void OnMinValueChanged(decimal oValue, decimal nValue)
        {
            MinValueChanged?.Invoke(this, EventArgs.Empty);

            if (Value < MinValue)
                TextBox.Text = MinValue.ToString();
        }
        #endregion

        #endregion

        /// <summary>
        /// 数字文本输入框
        /// </summary>
        public DXDecimalNumberTextBox()
        {
            TextBox.TextChanged += TextBox_TextChanged;
            TextBox.KeyPress += TextBox_KeyPress;
            TextBox.Text = "0.00";
        }

        #region Methods
        /// <summary>
        /// 按键状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
        }
        /// <summary>
        /// 文本改变时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            decimal vol;

            if (decimal.TryParse(TextBox.Text, NumberStyles.Float | NumberStyles.AllowThousands, Thread.CurrentThread.CurrentCulture.NumberFormat, out vol))
            {
                if (vol < MinValue)
                    vol = MinValue;

                if (vol > MaxValue)
                    vol = MaxValue;

                Value = vol;
            }
            else
            {
                Value = MinValue;
            }
        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _Value = 0;
                _MaxValue = 0;
                _MinValue = 0;

                ValueChanged = null;
                MaxValueChanged = null;
                MinValueChanged = null;
            }
        }
        #endregion
    }
}

