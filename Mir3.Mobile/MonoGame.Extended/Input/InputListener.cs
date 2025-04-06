using Microsoft.Xna.Framework;

namespace MonoGame.Extended.Input
{
    public abstract class InputListener
    {
        public abstract void Update(GameTime gameTime);
    }

    public abstract class InputListenerSettings<T> where T : InputListener
    {
        public abstract T CreateListener();
    }
}
