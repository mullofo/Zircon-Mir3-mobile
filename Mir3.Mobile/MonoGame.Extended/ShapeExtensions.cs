using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MonoGame.Extended
{
    public static class ShapeExtensions
    {
        private static Texture2D _whitePixelTexture;

        private static Texture2D GetTexture(SpriteBatch spriteBatch)
        {
            if (_whitePixelTexture == null)
            {
                _whitePixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1, mipmap: false, SurfaceFormat.Color);
                _whitePixelTexture.SetData(new Color[1] { Color.White });
                spriteBatch.Disposing += delegate
                {
                    _whitePixelTexture?.Dispose();
                    _whitePixelTexture = null;
                };
            }
            return _whitePixelTexture;
        }

        public static Texture2D GetCircleTexture(SpriteBatch spriteBatch, int radius)
        {
            int num = 2 * radius - 1;
            int num2 = num;
            Texture2D texture2D = new Texture2D(spriteBatch.GraphicsDevice, num, num2, mipmap: false, SurfaceFormat.Color);
            Color[] array = new Color[num * num2];
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num2; j++)
                {
                    if ((radius - i) * (radius - i) + (radius - j) * (radius - j) > radius * radius)
                    {
                        array[i + j * num] = Color.Transparent;
                    }
                    else
                    {
                        array[i + j * num] = Color.Red;
                    }
                }
            }
            texture2D.SetData(array);
            return texture2D;
        }

        public static void DrawLine(this SpriteBatch spriteBatch, float x1, float y1, float x2, float y2, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, thickness, layerDepth);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point1, Vector2 point2, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            float length = Vector2.Distance(point1, point2);
            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            spriteBatch.DrawLine(point1, length, angle, color, thickness, layerDepth);
        }

        public static void DrawLine(this SpriteBatch spriteBatch, Vector2 point, float length, float angle, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            Vector2 vector = new Vector2(0f, 0f);
            Vector2 vector2 = new Vector2(length, thickness);
            spriteBatch.Draw(GetTexture(spriteBatch), point, null, color, angle, vector, vector2, SpriteEffects.None, layerDepth);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, float x, float y, Color color, float size = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawPoint(new Vector2(x, y), color, size, layerDepth);
        }

        public static void DrawPoint(this SpriteBatch spriteBatch, Vector2 position, Color color, float size = 1f, float layerDepth = 0f)
        {
            Vector2 vector = Vector2.One * size;
            Vector2 vector2 = new Vector2(0.5f) - new Vector2(size * 0.5f);
            spriteBatch.Draw(GetTexture(spriteBatch), (position + vector2), null, color, 0f, Vector2.Zero, vector, SpriteEffects.None, layerDepth);
        }

        public static void DrawPolygon(this SpriteBatch spriteBatch, Vector2 offset, IReadOnlyList<Vector2> points, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            if (points.Count == 0)
            {
                return;
            }
            if (points.Count == 1)
            {
                spriteBatch.DrawPoint(points[0], color, (int)thickness);
                return;
            }
            Texture2D texture = GetTexture(spriteBatch);
            for (int i = 0; i < points.Count - 1; i++)
            {
                DrawPolygonEdge(spriteBatch, texture, points[i] + offset, points[i + 1] + offset, color, thickness, layerDepth);
            }
            DrawPolygonEdge(spriteBatch, texture, points[points.Count - 1] + offset, points[0] + offset, color, thickness, layerDepth);
        }

        private static void DrawPolygonEdge(SpriteBatch spriteBatch, Texture2D texture, Vector2 point1, Vector2 point2, Color color, float thickness, float layerDepth)
        {
            float x = Vector2.Distance(point1, point2);
            float rotation = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            Vector2 vector = new Vector2(x, thickness);
            spriteBatch.Draw(texture, point1, null, color, rotation, Vector2.Zero, vector, SpriteEffects.None, layerDepth);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, RectangleF rectangle, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            Texture2D texture = GetTexture(spriteBatch);
            Vector2 vector = new Vector2(rectangle.X, rectangle.Y);
            Vector2 vector2 = new Vector2(rectangle.Right - thickness, rectangle.Y);
            Vector2 vector3 = new Vector2(rectangle.X, rectangle.Bottom - thickness);
            Vector2 vector4 = new Vector2(rectangle.Width, thickness);
            Vector2 vector5 = new Vector2(thickness, rectangle.Height);
            spriteBatch.Draw(texture, vector, null, color, 0f, Vector2.Zero, vector4, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture, vector, null, color, 0f, Vector2.Zero, vector5, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture, vector2, null, color, 0f, Vector2.Zero, vector5, SpriteEffects.None, layerDepth);
            spriteBatch.Draw(texture, vector3, null, color, 0f, Vector2.Zero, vector4, SpriteEffects.None, layerDepth);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, Vector2 location, Size2 size, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawRectangle(new RectangleF(location.X, location.Y, size.Width, size.Height), color, thickness, layerDepth);
        }

        public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawRectangle(new RectangleF(x, y, width, height), color, thickness, layerDepth);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, Vector2 center, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawPolygon(center, CreateCircle(radius, sides), color, thickness, layerDepth);
        }

        public static void DrawCircle(this SpriteBatch spriteBatch, float x, float y, float radius, int sides, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawPolygon(new Vector2(x, y), CreateCircle(radius, sides), color, thickness, layerDepth);
        }

        public static void DrawEllipse(this SpriteBatch spriteBatch, Vector2 center, Vector2 radius, int sides, Color color, float thickness = 1f, float layerDepth = 0f)
        {
            spriteBatch.DrawPolygon(center, CreateEllipse(radius.X, radius.Y, sides), color, thickness, layerDepth);
        }

        private static Vector2[] CreateCircle(double radius, int sides)
        {
            Vector2[] array = new Vector2[sides];
            double num = Math.PI * 2.0 / sides;
            double num2 = 0.0;
            for (int i = 0; i < sides; i++)
            {
                array[i] = new Vector2((float)(radius * Math.Cos(num2)), (float)(radius * Math.Sin(num2)));
                num2 += num;
            }
            return array;
        }

        private static Vector2[] CreateEllipse(float rx, float ry, int sides)
        {
            Vector2[] array = new Vector2[sides];
            double num = 0.0;
            double num2 = Math.PI * 2.0 / sides;
            int num3 = 0;
            while (num3 < sides)
            {
                float x = (float)((double)rx * Math.Cos(num));
                float y = (float)((double)ry * Math.Sin(num));
                array[num3] = new Vector2(x, y);
                num3++;
                num += num2;
            }
            return array;
        }
    }
}
