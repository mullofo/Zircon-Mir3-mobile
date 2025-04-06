using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using System.Drawing;

namespace MonoGame.Extended
{
    public enum BorderStyle
    {
        None,
        FixedSingle,
        Fixed3D
    }

    public class TextBox : IDisposable
    {

        private string _text = "";

        private Size _size;

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (!(_text == value))
                {
                    string text = _text;
                    _text = value;
                    OnTextChanged(text, value);
                    OnTextChanged(EventArgs.Empty);
                }
            }
        }

        public bool UseSystemPasswordChar { get; set; }

        public Color ForeColor { get; set; }

        public Color BackColor { get; set; }

        public bool Visible { get; set; }

        public Point Location { get; set; }

        public bool ReadOnly { get; set; }

        public bool Multiline { get; set; }

        public bool AcceptsReturn { get; set; }

        public int MaxLength { get; set; }

        public IntPtr Handle { get; }

        public int SelectionStart { get; set; }

        public int SelectionLength { get; set; }

        public BorderStyle BorderStyle { get; set; }

        public virtual Font Font { get; set; }

        public Size Size
        {
            get
            {
                return _size;
            }
            set
            {
                if (!(_size == value))
                {
                    _ = _size;
                    _size = value;
                    OnSizeChanged(EventArgs.Empty);
                }
            }
        }

        public bool IsDisposed { get; private set; }

        public event EventHandler<EventArgs> TextChanged;

        public event EventHandler<EventArgs> SizeChanged;

        public event EventHandler<KeyPressEventArgs> KeyPress;

        public event EventHandler<MouseEventArgs> MouseMove;

        public event EventHandler<MouseEventArgs> MouseDown;

        public event EventHandler<MouseEventArgs> MouseUp;

        public event EventHandler<KeyEventArgs> KeyDown;

        public event EventHandler<KeyEventArgs> KeyUp;

        public event EventHandler<EventArgs> MouseWheel;

        public event EventHandler<EventArgs> LostFocus;

        public event EventHandler<EventArgs> GotFocus;

        public void OnEnter()
        {
            OnKeyPress(new KeyPressEventArgs((char)Keys.Enter));
        }

        protected virtual void OnTextChanged(string oValue, string nValue)
        {
            if (!string.IsNullOrEmpty(Text) && SelectionStart > Text.Length)
            {
                SelectionStart = Text.Length;
            }
            TextChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnTextChanged(EventArgs e)
        {
        }

        protected virtual void OnMouseClick(MouseEventArgs e)
        {

        }

        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyDown?.Invoke(this, e);
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyUp?.Invoke(this, e);
        }

        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            KeyPress?.Invoke(this, e);
        }


        protected virtual void OnSizeChanged(EventArgs e)
        {
            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SelectAll()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(!IsDisposed);
            GC.SuppressFinalize(this);
        }

        ~TextBox()
        {
            Dispose(disposing: false);
        }
    }
}
