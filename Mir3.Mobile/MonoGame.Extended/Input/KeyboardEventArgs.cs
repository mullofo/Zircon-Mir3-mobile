using Microsoft.Xna.Framework.Input;
using System;

namespace MonoGame.Extended.Input
{
    public class KeyboardEventArgs : EventArgs
    {
        public Keys Key { get; }

        public KeyboardModifiers Modifiers { get; }

        public char? Character => ToChar(Key, Modifiers);

        public KeyboardEventArgs(Keys key, KeyboardState keyboardState)
        {
            Key = key;
            Modifiers = KeyboardModifiers.None;
            if (keyboardState.IsKeyDown(Keys.LeftControl) || keyboardState.IsKeyDown(Keys.RightControl))
            {
                Modifiers |= KeyboardModifiers.Control;
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift))
            {
                Modifiers |= KeyboardModifiers.Shift;
            }
            if (keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt))
            {
                Modifiers |= KeyboardModifiers.Alt;
            }
        }

        private static char? ToChar(Keys key, KeyboardModifiers modifiers = KeyboardModifiers.None)
        {
            bool flag = (modifiers & KeyboardModifiers.Shift) == KeyboardModifiers.Shift;
            if (key == Keys.A)
            {
                return flag ? 'A' : 'a';
            }
            if (key == Keys.B)
            {
                return flag ? 'B' : 'b';
            }
            if (key == Keys.C)
            {
                return flag ? 'C' : 'c';
            }
            if (key == Keys.D)
            {
                return flag ? 'D' : 'd';
            }
            if (key == Keys.E)
            {
                return flag ? 'E' : 'e';
            }
            if (key == Keys.F)
            {
                return flag ? 'F' : 'f';
            }
            if (key == Keys.G)
            {
                return flag ? 'G' : 'g';
            }
            if (key == Keys.H)
            {
                return flag ? 'H' : 'h';
            }
            if (key == Keys.I)
            {
                return flag ? 'I' : 'i';
            }
            if (key == Keys.J)
            {
                return flag ? 'J' : 'j';
            }
            if (key == Keys.K)
            {
                return flag ? 'K' : 'k';
            }
            if (key == Keys.L)
            {
                return flag ? 'L' : 'l';
            }
            if (key == Keys.M)
            {
                return flag ? 'M' : 'm';
            }
            if (key == Keys.N)
            {
                return flag ? 'N' : 'n';
            }
            if (key == Keys.O)
            {
                return flag ? 'O' : 'o';
            }
            if (key == Keys.P)
            {
                return flag ? 'P' : 'p';
            }
            if (key == Keys.Q)
            {
                return flag ? 'Q' : 'q';
            }
            if (key == Keys.R)
            {
                return flag ? 'R' : 'r';
            }
            if (key == Keys.S)
            {
                return flag ? 'S' : 's';
            }
            if (key == Keys.T)
            {
                return flag ? 'T' : 't';
            }
            if (key == Keys.U)
            {
                return flag ? 'U' : 'u';
            }
            if (key == Keys.V)
            {
                return flag ? 'V' : 'v';
            }
            if (key == Keys.W)
            {
                return flag ? 'W' : 'w';
            }
            if (key == Keys.X)
            {
                return flag ? 'X' : 'x';
            }
            if (key == Keys.Y)
            {
                return flag ? 'Y' : 'y';
            }
            if (key == Keys.Z)
            {
                return flag ? 'Z' : 'z';
            }
            if ((key == Keys.D0 && !flag) || key == Keys.NumPad0)
            {
                return '0';
            }
            if ((key == Keys.D1 && !flag) || key == Keys.NumPad1)
            {
                return '1';
            }
            if ((key == Keys.D2 && !flag) || key == Keys.NumPad2)
            {
                return '2';
            }
            if ((key == Keys.D3 && !flag) || key == Keys.NumPad3)
            {
                return '3';
            }
            if ((key == Keys.D4 && !flag) || key == Keys.NumPad4)
            {
                return '4';
            }
            if ((key == Keys.D5 && !flag) || key == Keys.NumPad5)
            {
                return '5';
            }
            if ((key == Keys.D6 && !flag) || key == Keys.NumPad6)
            {
                return '6';
            }
            if ((key == Keys.D7 && !flag) || key == Keys.NumPad7)
            {
                return '7';
            }
            if ((key == Keys.D8 && !flag) || key == Keys.NumPad8)
            {
                return '8';
            }
            if ((key == Keys.D9 && !flag) || key == Keys.NumPad9)
            {
                return '9';
            }
            if (key == Keys.D0 && flag)
            {
                return ')';
            }
            if (key == Keys.D1 && flag)
            {
                return '!';
            }
            if (key == Keys.D2 && flag)
            {
                return '@';
            }
            if (key == Keys.D3 && flag)
            {
                return '#';
            }
            if (key == Keys.D4 && flag)
            {
                return '$';
            }
            if (key == Keys.D5 && flag)
            {
                return '%';
            }
            if (key == Keys.D6 && flag)
            {
                return '^';
            }
            if (key == Keys.D7 && flag)
            {
                return '&';
            }
            if (key == Keys.D8 && flag)
            {
                return '*';
            }
            if (key == Keys.D9 && flag)
            {
                return '(';
            }
            switch (key)
            {
                case Keys.Space:
                    return ' ';
                case Keys.Tab:
                    return '\t';
                case Keys.Enter:
                    return '\r';
                case Keys.Back:
                    return '\b';
                case Keys.Add:
                    return '+';
                case Keys.Decimal:
                    return '.';
                case Keys.Divide:
                    return '/';
                case Keys.Multiply:
                    return '*';
                case Keys.OemBackslash:
                    return '\\';
                case Keys.OemComma:
                    if (!flag)
                    {
                        return ',';
                    }
                    break;
            }
            if (key == Keys.OemComma && flag)
            {
                return '<';
            }
            if (key == Keys.OemOpenBrackets && !flag)
            {
                return '[';
            }
            if (key == Keys.OemOpenBrackets && flag)
            {
                return '{';
            }
            if (key == Keys.OemCloseBrackets && !flag)
            {
                return ']';
            }
            if (key == Keys.OemCloseBrackets && flag)
            {
                return '}';
            }
            if (key == Keys.OemPeriod && !flag)
            {
                return '.';
            }
            if (key == Keys.OemPeriod && flag)
            {
                return '>';
            }
            if (key == Keys.OemPipe && !flag)
            {
                return '\\';
            }
            if (key == Keys.OemPipe && flag)
            {
                return '|';
            }
            if (key == Keys.OemPlus && !flag)
            {
                return '=';
            }
            if (key == Keys.OemPlus && flag)
            {
                return '+';
            }
            if (key == Keys.OemMinus && !flag)
            {
                return '-';
            }
            if (key == Keys.OemMinus && flag)
            {
                return '_';
            }
            if (key == Keys.OemQuestion && !flag)
            {
                return '/';
            }
            if (key == Keys.OemQuestion && flag)
            {
                return '?';
            }
            if (key == Keys.OemQuotes && !flag)
            {
                return '\'';
            }
            if (key == Keys.OemQuotes && flag)
            {
                return '"';
            }
            if (key == Keys.OemSemicolon && !flag)
            {
                return ';';
            }
            if (key == Keys.OemSemicolon && flag)
            {
                return ':';
            }
            if (key == Keys.OemTilde && !flag)
            {
                return '`';
            }
            if (key == Keys.OemTilde && flag)
            {
                return '~';
            }
            if (key == Keys.Subtract)
            {
                return '-';
            }
            return null;
        }
    }
}
