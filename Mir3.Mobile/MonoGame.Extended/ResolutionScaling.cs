using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace MonoGame.Extended
{
    public class ResolutionScaling
    {
        public static RenderTarget2D renderTarget2D;

        private static Matrix _scalingMatrix;

        public static int deviceX;

        public static int deviceY;

        public static int startX;

        private static float scalingPositionX = 1f;

        private static float scalingPositionY = 1f;

        private static Microsoft.Xna.Framework.Point _sourceResolution;

        private static Microsoft.Xna.Framework.Point _destinationResolution;

        private static float scaleX;

        private static float scaleY;

        public static Matrix ScalingMatrix
        {
            get
            {
                _ = _scalingMatrix;
                return _scalingMatrix;
            }
            set
            {
                _scalingMatrix = value;
            }
        }

        public static void Initialize(Microsoft.Xna.Framework.Point sourceResolution, Microsoft.Xna.Framework.Point destinationResolution)
        {
            _sourceResolution = sourceResolution;
            _destinationResolution = destinationResolution;
            float num = _sourceResolution.X;
            float num2 = _sourceResolution.Y;
            float num3 = destinationResolution.X;
            float num4 = destinationResolution.Y;
            scaleX = num3 / num;
            scaleY = num4 / num2;
            scalingPositionX = num / num3;
            scalingPositionY = num2 / num4;
            _scalingMatrix = Matrix.CreateScale(scaleX, scaleY, 1f);
        }

        public static void Initialize(Game game, Microsoft.Xna.Framework.Point sourceResolution)
        {
            Microsoft.Xna.Framework.Point destinationResolution = new Microsoft.Xna.Framework.Point(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            Initialize(sourceResolution, destinationResolution);
        }

        public static void LoadContent(GraphicsDevice game, Microsoft.Xna.Framework.Point sourceResolution)
        {
            renderTarget2D = new RenderTarget2D(game, sourceResolution.X, sourceResolution.Y, mipMap: false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            startX = (int)(_destinationResolution.X - (sourceResolution.X * _destinationResolution.Y / sourceResolution.Y + 0.5f)) / 2;
        }

        public static void BeginDraw(Game game)
        {
            game.GraphicsDevice.SetRenderTarget(renderTarget2D);
        }

        public static void EndDraw(Game game, SpriteBatch spriteBatch)
        {
            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget2D, new Rectangle(startX, 0, game.GraphicsDevice.Viewport.Bounds.Width - 2 * startX, game.GraphicsDevice.Viewport.Height), Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();
        }

        public static void EndDraw(Game game, SpriteBatch spriteBatch, Microsoft.Xna.Framework.Point destinationResolution)
        {
            game.GraphicsDevice.SetRenderTarget(null);
            game.GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(renderTarget2D, new Rectangle(startX, 0, destinationResolution.X, destinationResolution.Y), Microsoft.Xna.Framework.Color.White);
            spriteBatch.End();
        }

        public static void Draw(Microsoft.Xna.Framework.Point sourceResolution, Microsoft.Xna.Framework.Point destinationResolution)
        {
            Initialize(sourceResolution, destinationResolution);
        }

        public static void Draw(Game game, Microsoft.Xna.Framework.Point sourceResolution)
        {
            Microsoft.Xna.Framework.Point destinationResolution = new Microsoft.Xna.Framework.Point(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            Initialize(sourceResolution, destinationResolution);
        }

        public static Microsoft.Xna.Framework.Point Position(Microsoft.Xna.Framework.Point point)
        {
            return new Microsoft.Xna.Framework.Point((int)(point.X * scalingPositionX), (int)(point.Y * scalingPositionY));
        }

        public static Vector2 Position(Vector2 vector2)
        {
            return new Vector2(vector2.X * scalingPositionX, vector2.Y * scalingPositionY);
        }

        public static float X(float x)
        {
            return (x - startX) * scalingPositionY;
        }

        public static float Y(float y)
        {
            return y * scalingPositionY;
        }
    }
}
