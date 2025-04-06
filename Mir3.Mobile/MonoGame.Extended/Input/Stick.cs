using Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace MonoGame.Extended.Input
{
    public struct TapStart
    {
        public int Id;

        public double Time;

        public Vector2 Pos;

        public TapStart(int id, double time, Vector2 pos)
        {
            Id = id;
            Time = time;
            Pos = pos;
        }
    }

    public class Stick
    {
        public readonly float deadZoneSize;

        public readonly float aliveZoneSize;

        public readonly float aliveZoneFollowFactor;

        public readonly float aliveZoneFollowSpeed;

        public readonly float edgeSpacing;

        public TouchLocation? touchLocation;

        public readonly Rectangle StartRegion;

        public Vector2 StartLocation;

        public Vector2 FixedLocation;

        public Vector2 Direction;

        public Vector2 Pos;

        public float Magnitude;

        public int lastExcludedRightTouchId = -1;

        public List<Rectangle> startExcludeRegions = new List<Rectangle>(5);

        public TouchStickStyle Style { get; private set; }

        public Stick(float deadZoneSize, Rectangle StartRegion, float aliveZoneSize, float aliveZoneFollowFactor, float aliveZoneFollowSpeed, float edgeSpacing)
        {
            this.deadZoneSize = deadZoneSize;
            this.StartRegion = StartRegion;
            this.aliveZoneFollowFactor = aliveZoneFollowFactor;
            this.aliveZoneFollowSpeed = aliveZoneFollowSpeed;
            this.aliveZoneSize = aliveZoneSize;
            this.edgeSpacing = edgeSpacing;
            Style = TouchStickStyle.Free;
        }

        public void SetAsFixed(Vector2 fixedLocation = default(Vector2))
        {
            if (fixedLocation == default(Vector2))
            {
                fixedLocation = FixedLocation;
            }
            FixedLocation = fixedLocation;
            StartLocation = fixedLocation;
            Style = TouchStickStyle.Fixed;
        }

        public void SetAsFreefollow()
        {
            Style = TouchStickStyle.FreeFollow;
        }

        public void SetAsFree()
        {
            Style = TouchStickStyle.Free;
        }

        public Vector2 GetPositionVector(float aliveZoneSize)
        {
            return StartLocation + Direction * new Vector2(1f, -1f) * Magnitude * aliveZoneSize;
        }

        public Vector2 GetRelativeVector(float aliveZoneSize)
        {
            return Direction * new Vector2(1f, -1f) * Magnitude * aliveZoneSize;
        }

        public void Update(TouchCollection state, TouchLocation? continueLocation, float dt)
        {
            if (continueLocation.HasValue)
            {
                touchLocation = continueLocation;
                Pos = continueLocation.Value.Position;
                EvaluatePoint(dt);
                return;
            }
            bool flag = false;
            if (touchLocation.HasValue)
            {
                foreach (TouchLocation item in state)
                {
                    Vector2 value = item.Position;
                    Vector2.DistanceSquared(ref value, ref Pos, out var result);
                    if (result < 100f)
                    {
                        flag = true;
                        touchLocation = item;
                        Pos = item.Position;
                        EvaluatePoint(dt);
                    }
                }
            }
            if (!flag)
            {
                touchLocation = null;
                Direction = Vector2.Zero;
                Magnitude = 0f;
            }
        }

        public void EvaluatePoint(float dt)
        {
            Direction = Pos - StartLocation;
            float num = Direction.Length();
            if (num <= deadZoneSize)
            {
                Direction = Vector2.Zero;
                Magnitude = 0f;
                return;
            }
            Direction.Normalize();
            Direction.Y *= -1f;
            if (num < aliveZoneSize)
            {
                Magnitude = num / aliveZoneSize;
                Direction = new Vector2(Direction.X * Magnitude, Direction.Y * Magnitude);
                return;
            }
            Magnitude = 1f;
            if (Style == TouchStickStyle.FreeFollow && num > aliveZoneSize * aliveZoneFollowFactor)
            {
                Vector2 value = new Vector2(Pos.X - Direction.X * aliveZoneSize * aliveZoneFollowFactor, Pos.Y + Direction.Y * aliveZoneSize * aliveZoneFollowFactor);
                Vector2.Lerp(ref StartLocation, ref value, (num - aliveZoneSize * aliveZoneFollowFactor) * aliveZoneFollowSpeed * dt, out StartLocation);
                if (StartLocation.X < (float)StartRegion.Left)
                {
                    StartLocation.X = StartRegion.Left;
                }
                if (StartLocation.Y < (float)StartRegion.Top)
                {
                    StartLocation.Y = StartRegion.Top;
                }
                if (StartLocation.X > (float)StartRegion.Right - edgeSpacing)
                {
                    StartLocation.X = (float)StartRegion.Right - edgeSpacing;
                }
                if (StartLocation.Y > (float)StartRegion.Bottom - edgeSpacing)
                {
                    StartLocation.Y = (float)StartRegion.Bottom - edgeSpacing;
                }
            }
        }
    }

    public class StickTexture
    {
        public readonly float aliveZoneFollowSpeed;

        public readonly float aliveZoneFollowFactor;

        public readonly float edgeSpacing;

        public readonly float aliveZoneSize;

        private readonly TapStart[] tapStarts = new TapStart[4];

        private int tapStartCount;

        private double totalTime;

        public Stick LeftStick { get; set; }

        public StickTexture(float aliveZoneFollowFactor = 1f, float aliveZoneFollowSpeed = 0.05f, float edgeSpacing = 15f, float aliveZoneSize = 35f, float deadZoneSize = 15f)
        {
            this.aliveZoneFollowFactor = aliveZoneFollowFactor;
            this.aliveZoneFollowSpeed = aliveZoneFollowSpeed;
            this.edgeSpacing = edgeSpacing;
            this.aliveZoneSize = aliveZoneSize;
            LeftStick = new Stick(deadZoneSize, new Rectangle(0, 200, (int)((float)TouchPanel.DisplayWidth * 0.3f), TouchPanel.DisplayHeight - 200), aliveZoneSize, aliveZoneFollowFactor, aliveZoneFollowSpeed, edgeSpacing)
            {
                FixedLocation = new Vector2(aliveZoneSize * aliveZoneFollowFactor, (float)TouchPanel.DisplayHeight - aliveZoneSize * aliveZoneFollowFactor)
            };
        }

        public void Update(GameTime gameTime)
        {
            float num = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
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
        }

        public void DrawStick(SpriteBatch spriteBatch, Texture2D back, Texture2D stick, float scale = 1f)
        {
            if (GameScene.Game.StickMode == StickMode.Walk)
            {
                DrawStickCentered(back, LeftStick.StartLocation, Color.White, spriteBatch, scale);
                DrawStickCentered(stick, LeftStick.GetPositionVector(aliveZoneSize * scale), Color.White, spriteBatch, scale);
                return;
            }
            if (!(LeftStick.GetRelativeVector(aliveZoneSize) == new Vector2(0f, 0f)))
            {
                //GameScene.Game.DrawStick = true;
                DrawStickCentered(back, LeftStick.StartLocation, Color.White, spriteBatch, scale);
                DrawStickCentered(stick, LeftStick.GetPositionVector(aliveZoneSize * scale), Color.White, spriteBatch, scale);
            }
            //else
            //    GameScene.Game.DrawStick = false;

        }

        private void DrawStickCentered(Texture2D texture, Vector2 position, Color color, SpriteBatch spriteBatch, float scale = 1f)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, position, null, color, 0f, origin, scale, SpriteEffects.None, 0f);
        }
    }
}
