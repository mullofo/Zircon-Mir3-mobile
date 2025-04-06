using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGame.Extended.Input
{
    public enum MouseButtons
    {
        None = 0,
        Left = 0x100000,
        Right = 0x200000,
        Middle = 0x400000,
        XButton1 = 0x800000,
        XButton2 = 0x1000000
    }

    public class MouseEventArgs : EventArgs
    {
        public MouseButtons Button { get; }

        public int Clicks { get; }

        public int X { get; }

        public int Y { get; }

        public int Delta { get; }

        public System.Drawing.Point Location { get; set; }

        public TimeSpan Time { get; }

        public MouseState PreviousState { get; }

        public MouseState CurrentState { get; }

        public Microsoft.Xna.Framework.Point Position { get; }

        public int ScrollWheelValue { get; }

        public int ScrollWheelDelta { get; }

        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            Delta = delta;
            X = x;
            Y = y;
            Location = new System.Drawing.Point(x, y);
        }

        public MouseEventArgs(ViewportAdapter viewportAdapter, TimeSpan time, MouseState previousState, MouseState currentState, MouseButtons button = MouseButtons.None)
        {
            PreviousState = previousState;
            CurrentState = currentState;
            Position = viewportAdapter?.PointToScreen(currentState.X, currentState.Y) ?? new Microsoft.Xna.Framework.Point(currentState.X, currentState.Y);
            Button = button;
            ScrollWheelValue = currentState.ScrollWheelValue;
            ScrollWheelDelta = currentState.ScrollWheelValue - previousState.ScrollWheelValue;
            Time = time;
        }
    }
}
