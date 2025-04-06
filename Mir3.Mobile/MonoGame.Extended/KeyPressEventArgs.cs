using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Extended
{
    /// <summary>
    ///  Provides data for the event.
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='KeyPressEventArgs'/>
        ///  class.
        /// </summary>
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }

        public KeyPressEventArgs(Keys keyData)
        {
            KeyChar = (char)keyData;
        }

        /// <summary>
        ///  Gets the character corresponding to the key pressed.
        /// </summary>
        public char KeyChar { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the
        ///  event was handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}