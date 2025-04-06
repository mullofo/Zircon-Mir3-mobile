using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGame.Extended.Input
{
    public class TouchEventArgs : EventArgs
    {
        public ViewportAdapter ViewportAdapter { get; }

        public TouchLocation RawTouchLocation { get; }

        public TimeSpan Time { get; }

        public System.Drawing.Point Location
        {
            get => _Location;
            set
            {
                if (_Location == value) return;

                _Location = value;

            }
        }
        private System.Drawing.Point _Location;

        public Vector2 Delta { get; }

        public TouchEventArgs(ViewportAdapter viewportAdapter, TimeSpan time, TouchLocation location)
        {
            ViewportAdapter = viewportAdapter;
            RawTouchLocation = location;
            Time = time;
            Location = new System.Drawing.Point((int)location.Position.X, (int)location.Position.Y);
        }

        public TouchEventArgs(ViewportAdapter viewportAdapter, TimeSpan time, Vector2 location, Vector2 delta)
        {
            ViewportAdapter = viewportAdapter;
            Time = time;
            Location = new System.Drawing.Point((int)location.X, (int)location.Y);
            Delta = delta;
        }

        public override bool Equals(object other)
        {
            if (!(other is TouchEventArgs touchEventArgs))
            {
                return false;
            }
            if (this != touchEventArgs)
            {
                return RawTouchLocation.Id.Equals(touchEventArgs.RawTouchLocation.Id);
            }
            return true;
        }

        public override int GetHashCode()
        {
            return RawTouchLocation.Id.GetHashCode();
        }
    }
}
