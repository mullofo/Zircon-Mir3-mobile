using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Drawing;

namespace MonoGame.Extended
{
    public enum TextFormatFlags
    {
        Default = 0,
        Left = 0,
        Top = 0,
        GlyphOverhangPadding = 0,
        HorizontalCenter = 1,
        Right = 2,
        VerticalCenter = 4,
        Bottom = 8,
        WordBreak = 16,
        SingleLine = 32,
        ExpandTabs = 64,
        NoClipping = 256,
        ExternalLeading = 512,
        NoPrefix = 2048,
        Internal = 4096,
        TextBoxControl = 8192,
        PathEllipsis = 16384,
        EndEllipsis = 32768,
        ModifyString = 65536,
        RightToLeft = 131072,
        WordEllipsis = 262144,
        NoFullWidthCharacterBreak = 524288,
        HidePrefix = 1048576,
        PrefixOnly = 2097152,
        PreserveGraphicsClipping = 16777216,
        PreserveGraphicsTranslateTransform = 33554432,
        NoPadding = 268435456,
        LeftAndRightPadding = 536870912
    }

    public sealed class TextRenderer
    {
        public static Size MeasureText(IDeviceContext g, string text, Font font)
        {
            Vector2 vector = font.SpriteFont.MeasureString(text);
            return new Size((int)vector.X, (int)vector.Y);
        }

        public static Size MeasureText(IDeviceContext dc, string text, Font font, Size proposedSize, TextFormatFlags flags)
        {
            if ((flags & TextFormatFlags.WordBreak) == TextFormatFlags.WordBreak)
            {
                Vector2 vector = font.SpriteFont.MeasureString(text);
                if (vector.X > proposedSize.Width && text.Length > 1)
                {
                    float x = font.SpriteFont.MeasureString("字").X;
                    List<string> list = PorcessText(text, font, proposedSize.Width - 0.9f * x);
                    string text2 = string.Empty;
                    for (int i = 0; i < list.Count; i++)
                    {
                        text2 = i == list.Count - 1 ? text2 + list[i] : text2 + list[i] + "\n";
                    }
                    vector = font.SpriteFont.MeasureString(text2);
                }
                return new Size((int)vector.X, (int)vector.Y);
            }
            Vector2 vector2 = font.SpriteFont.MeasureString(text);
            return new Size((int)vector2.X, (int)vector2.Y);
        }

        public static void DrawText(SpriteBatch spriteBatch, string text, Font font, bool outline, System.Drawing.Rectangle displayArea, System.Drawing.Color foreColor, TextFormatFlags flags)
        {
            int x = displayArea.X;
            int y = displayArea.Y;
            Vector2 vector = font.SpriteFont.MeasureString(text);
            if ((flags & TextFormatFlags.HorizontalCenter) == TextFormatFlags.HorizontalCenter)
            {
                x = (displayArea.Width - (int)(displayArea.X + vector.X)) / 2;
            }
            else if ((flags & TextFormatFlags.Right) == TextFormatFlags.Right)
            {
                x = displayArea.Width - (int)(displayArea.X + vector.X);
            }
            if ((flags & TextFormatFlags.VerticalCenter) == TextFormatFlags.VerticalCenter)
            {
                y = (displayArea.Height - (int)(displayArea.Y + vector.Y)) / 2;
            }
            if ((flags & TextFormatFlags.WordBreak) == TextFormatFlags.WordBreak && vector.X > displayArea.Width + displayArea.X && text.Length > 1)
            {
                float sizeX = font.SpriteFont.MeasureString("字").X;
                List<string> list = PorcessText(text, font, displayArea.Width - 0.9f * sizeX);
                string text2 = string.Empty;
                for (int i = 0; i < list.Count; i++)
                {
                    text2 = i == list.Count - 1 ? text2 + list[i] : text2 + list[i] + "\n";
                }
                text = text2;
            }
            spriteBatch.DrawString(font.SpriteFont, text, new Vector2(x, y), new Microsoft.Xna.Framework.Color(foreColor.R, foreColor.G, foreColor.B, foreColor.A), effect: outline ? FontSystemEffect.Stroked : FontSystemEffect.None, effectAmount: 1);
        }

        public static List<string> PorcessText(string text, Font font, float X)
        {
            List<string> list = new List<string>();
            for (int i = 1; i <= text.Length; i++)
            {
                if (font.SpriteFont.MeasureString(text.Substring(0, i)).X < X)
                {
                    continue;
                }
                list.Add(text.Substring(0, i));
                string text2 = text.Substring(i, text.Length - i);
                if (font.SpriteFont.MeasureString(text2).X < X)
                {
                    list.Add(text2);
                    break;
                }
                {
                    foreach (string item in PorcessText(text.Substring(i, text.Length - i), font, X))
                    {
                        list.Add(item);
                    }
                    return list;
                }
            }
            return list;
        }
    }
}
