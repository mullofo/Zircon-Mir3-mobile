using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Extended
{
    /// <summary>
    ///  Provides data for the  or
    ///  event.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private bool _suppressKeyPress;

        /// <summary>
        ///  Initializes a new instance of the <see cref='KeyEventArgs'/> class.
        /// </summary>
        public KeyEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }

        /// <summary>
        ///  Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        public virtual bool Alt => (KeyData & Keys.LeftAlt) == Keys.LeftAlt || (KeyData & Keys.RightAlt) == Keys.RightAlt;

        /// <summary>
        ///  Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        public bool Control => (KeyData & Keys.LeftControl) == Keys.LeftControl || (KeyData & Keys.RightControl) == Keys.RightControl;

        /// <summary>
        ///  Gets or sets a value indicating whether the event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        ///  Gets the keyboard code for a or
        ///  event.
        /// </summary>
        public Keys KeyCode
        {
            get
            {
                Keys keyGenerated = (Keys)((int)KeyData & 0x0000FFFF);

                // since Keys can be discontiguous, keeping Enum.IsDefined.
                if (!Enum.IsDefined(typeof(Keys), (int)keyGenerated))
                {
                    return Keys.None;
                }

                return keyGenerated;
            }
        }

        /// <summary>
        ///  Gets the keyboard value for a or
        ///  event.
        /// </summary>
        public int KeyValue => (int)KeyData & 0x0000FFFF;

        /// <summary>
        ///  Gets the key data for a or
        ///  event.
        /// </summary>
        public Keys KeyData { get; }

        /// <summary>
        ///  Gets the modifier flags for aor
        ///  event.
        ///  This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
        /// </summary>
        public Keys Modifiers => (Keys)((int)KeyData & unchecked((int)0xFFFF0000));

        /// <summary>
        ///  Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
        public virtual bool Shift => (KeyData & Keys.LeftShift) == Keys.LeftShift || (KeyData & Keys.RightShift) == Keys.RightShift;

        public bool SuppressKeyPress
        {
            get => _suppressKeyPress;
            set
            {
                _suppressKeyPress = value;
                Handled = value;
            }
        }
    }
}