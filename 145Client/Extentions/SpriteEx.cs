using SharpDX.Direct3D9;
using System;

namespace SharpDX
{
    public static class SpriteEx
    {
        public static void Draw(this Sprite sprite, Texture texture, Vector3 center, Vector3 postion, System.Drawing.Color color)
        {
            sprite.Draw(texture, color.ToRawColorBGRA(), centerRef: center.ToRawVector3(), positionRef: postion.ToRawVector3());
        }

        public static void Draw(this Sprite sprite, Texture texture, System.Drawing.Rectangle rectangle, Vector3 center, Vector3 postion, System.Drawing.Color color)
        {
            try
            {
                sprite.Draw(texture, color.ToRawColorBGRA(), rectangle.ToRawRectangle(), centerRef: center.ToRawVector3(), positionRef: postion.ToRawVector3());
            }
            catch (Exception) { }
        }
    }
}
