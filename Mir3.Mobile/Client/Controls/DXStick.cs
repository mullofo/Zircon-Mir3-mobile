using Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.Input;
using System;
using System.Drawing;
using Point = System.Drawing.Point;

namespace Client.Controls
{
    public class DXStick : DXControl
    {
        DXImageControl JoyStickBack;
        DXImageControl JoyStick;
        public DXStick(float aliveZoneFollowFactor = 1f, float aliveZoneFollowSpeed = 0.05f, float edgeSpacing = 15f, float aliveZoneSize = 35f, float deadZoneSize = 15f)
        {

            JoyStickBack = new DXImageControl()
            {
                Parent = this,
                LibraryFile = Library.LibraryFile.PhoneUI,
                Index = 50,
                PassThrough = true
            };
            Size = new Size(JoyStickBack.Size.Width + 20, JoyStickBack.Size.Height + 20);
            JoyStickBack.Location = new Point((Size.Width - JoyStickBack.Size.Width) / 2, (Size.Height - JoyStickBack.Size.Width) / 2);
            JoyStick = new DXImageControl
            {
                Parent = this,
                LibraryFile = Library.LibraryFile.PhoneUI,
                Index = 51,
                PassThrough = true
            };
            ReLocation();

            this.aliveZoneFollowFactor = aliveZoneFollowFactor;
            this.aliveZoneFollowSpeed = aliveZoneFollowSpeed;
            this.edgeSpacing = edgeSpacing;
            this.aliveZoneSize = aliveZoneSize;
            LeftStick = new Stick(deadZoneSize, new Microsoft.Xna.Framework.Rectangle(0, JoyStickBack.Size.Width, (int)((float)TouchPanel.DisplayWidth * 0.3f), TouchPanel.DisplayHeight - JoyStickBack.Size.Width), aliveZoneSize, aliveZoneFollowFactor, aliveZoneFollowSpeed, edgeSpacing)
            {
                FixedLocation = new Vector2(aliveZoneSize * aliveZoneFollowFactor, (float)TouchPanel.DisplayHeight - aliveZoneSize * aliveZoneFollowFactor)
            };
        }

        public readonly float aliveZoneFollowSpeed;

        public readonly float aliveZoneFollowFactor;

        public readonly float edgeSpacing;

        public readonly float aliveZoneSize;

        private readonly TapStart[] tapStarts = new TapStart[4];

        private int tapStartCount;

        private double totalTime;

        public Stick LeftStick { get; set; }


        private Point NormalLocation => new Point((Size.Width - JoyStick.Size.Width) / 2, (Size.Height - JoyStick.Size.Width) / 2);

        bool _isEnd;
        public bool IsEnd
        {
            get { return _isEnd; }
            set
            {
                _isEnd = value;
                if (_isEnd)
                {
                    ReLocation();
                }
            }
        }

        public override void OnTouchDown(TouchEventArgs e)
        {
            base.OnTouchDown(e);
            IsEnd = false;
        }

        private void ReLocation()
        {
            if (JoyStick != null)
                JoyStick.Location = NormalLocation;
        }
        DateTime _now = Envir.CEnvir.Now;
        public override void Process()
        {
            if (IsEnd) return;
            float num = (float)(Envir.CEnvir.Now - _now).TotalMilliseconds;
            totalTime += num;
            TouchCollection state = TouchPanel.GetState();
            TouchLocation? continueLocation = null;
            if (tapStartCount > state.Count)
            {
                tapStartCount = state.Count;
            }
            foreach (TouchLocation item in state)
            {
                if (item.State == TouchLocationState.Released)
                {
                    int num2 = -1;
                    for (int i = 0; i < tapStartCount; i++)
                    {
                        if (tapStarts[i].Id == item.Id)
                        {
                            num2 = i;
                            break;
                        }
                    }
                    if (num2 >= 0)
                    {
                        for (int j = num2; j < tapStartCount - 1; j++)
                        {
                            tapStarts[j] = tapStarts[j + 1];
                        }
                        tapStartCount--;
                    }
                    continue;
                }
                if (item.State == TouchLocationState.Pressed && tapStartCount < tapStarts.Length)
                {
                    tapStarts[tapStartCount] = new TapStart(item.Id, totalTime, item.Position);
                    tapStartCount++;
                }
                if (LeftStick.touchLocation.HasValue && item.Id == LeftStick.touchLocation.Value.Id)
                {
                    continueLocation = item;
                    continue;
                }
                if (!item.TryGetPreviousLocation(out var aPreviousLocation))
                {
                    aPreviousLocation = item;
                }
                if (LeftStick.touchLocation.HasValue || !LeftStick.StartRegion.Contains((int)aPreviousLocation.Position.X, (int)aPreviousLocation.Position.Y))
                {
                    continue;
                }
                if (LeftStick.Style == TouchStickStyle.Fixed)
                {
                    if (Vector2.Distance(aPreviousLocation.Position, LeftStick.StartLocation) < aliveZoneSize)
                    {
                        continueLocation = aPreviousLocation;
                    }
                    continue;
                }
                continueLocation = aPreviousLocation;
                LeftStick.StartLocation = continueLocation.Value.Position;
                if (LeftStick.StartLocation.X < (float)LeftStick.StartRegion.Left + edgeSpacing)
                {
                    LeftStick.StartLocation.X = (float)LeftStick.StartRegion.Left + edgeSpacing;
                }
                if (LeftStick.StartLocation.Y > (float)LeftStick.StartRegion.Bottom - edgeSpacing)
                {
                    LeftStick.StartLocation.Y = (float)LeftStick.StartRegion.Bottom - edgeSpacing;
                }

            }
            LeftStick.Update(state, continueLocation, num);

            var location = LeftStick.GetPositionVector(aliveZoneSize * ZoomRate);
            if (location != Vector2.Zero)
            {
                var offset = new Point((int)((location.X - LeftStick.StartLocation.X) / ZoomRate), (int)((location.Y - LeftStick.StartLocation.Y) / ZoomRate));
                var x = NormalLocation.X + offset.X;
                var y = NormalLocation.Y + offset.Y;
                if ((x + JoyStick.Size.Width) > JoyStickBack.Size.Width)
                {
                    x = JoyStickBack.Size.Width - JoyStick.Size.Width;
                }
                if ((y + JoyStick.Size.Height) > JoyStickBack.Size.Height)
                {
                    y = JoyStickBack.Size.Height - JoyStick.Size.Height;
                }
                JoyStick.Location = new Point(x, y);
            }
            else
            {
                ReLocation();
            }
            var relativeVector = LeftStick.GetRelativeVector(aliveZoneSize);
            (ActiveScene as GameScene).MapControl.SetStick(relativeVector.X, relativeVector.Y);

            _now = Envir.CEnvir.Now;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            JoyStick?.TryDispose();
            JoyStickBack?.TryDispose();
        }
    }
}

