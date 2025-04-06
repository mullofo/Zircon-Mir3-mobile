using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace MonoGame.Extended.Input
{
    public enum KeyboardModifiers
    {
        Control = 1,
        Shift = 2,
        Alt = 4,
        None = 0
    }

    public class KeyboardListener : InputListener
    {
        private Array _keysValues = Enum.GetValues(typeof(Keys));

        private bool _isInitial;

        private TimeSpan _lastPressTime;

        private Keys _previousKey;

        private KeyboardState _previousState;

        public bool RepeatPress { get; }

        public int InitialDelay { get; }

        public int RepeatDelay { get; }

        public event EventHandler<KeyboardEventArgs> KeyTyped;

        public event EventHandler<KeyboardEventArgs> KeyPressed;

        public event EventHandler<KeyboardEventArgs> KeyReleased;

        public KeyboardListener()
            : this(new KeyboardListenerSettings())
        {
        }

        public KeyboardListener(KeyboardListenerSettings settings)
        {
            RepeatPress = settings.RepeatPress;
            InitialDelay = settings.InitialDelayMilliseconds;
            RepeatDelay = settings.RepeatDelayMilliseconds;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            RaisePressedEvents(gameTime, state);
            RaiseReleasedEvents(state);
            if (RepeatPress)
            {
                RaiseRepeatEvents(gameTime, state);
            }
            _previousState = state;
        }

        private void RaisePressedEvents(GameTime gameTime, KeyboardState currentState)
        {
            if (currentState.IsKeyDown(Keys.LeftAlt) || currentState.IsKeyDown(Keys.RightAlt))
            {
                return;
            }
            foreach (Keys item in from Keys key in _keysValues
                                  where currentState.IsKeyDown(key) && _previousState.IsKeyUp(key)
                                  select key)
            {
                KeyboardEventArgs keyboardEventArgs = new KeyboardEventArgs(item, currentState);
                this.KeyPressed?.Invoke(this, keyboardEventArgs);
                if (keyboardEventArgs.Character.HasValue)
                {
                    this.KeyTyped?.Invoke(this, keyboardEventArgs);
                }
                _previousKey = item;
                _lastPressTime = gameTime.TotalGameTime;
                _isInitial = true;
            }
        }

        private void RaiseReleasedEvents(KeyboardState currentState)
        {
            foreach (Keys item in from Keys key in _keysValues
                                  where currentState.IsKeyUp(key) && _previousState.IsKeyDown(key)
                                  select key)
            {
                this.KeyReleased?.Invoke(this, new KeyboardEventArgs(item, currentState));
            }
        }

        private void RaiseRepeatEvents(GameTime gameTime, KeyboardState currentState)
        {
            double totalMilliseconds = (gameTime.TotalGameTime - _lastPressTime).TotalMilliseconds;
            if (currentState.IsKeyDown(_previousKey) && ((_isInitial && totalMilliseconds > (double)InitialDelay) || (!_isInitial && totalMilliseconds > (double)RepeatDelay)))
            {
                KeyboardEventArgs keyboardEventArgs = new KeyboardEventArgs(_previousKey, currentState);
                this.KeyPressed?.Invoke(this, keyboardEventArgs);
                if (keyboardEventArgs.Character.HasValue)
                {
                    this.KeyTyped?.Invoke(this, keyboardEventArgs);
                }
                _lastPressTime = gameTime.TotalGameTime;
                _isInitial = false;
            }
        }
    }

    public class KeyboardListenerSettings : InputListenerSettings<KeyboardListener>
    {
        public bool RepeatPress { get; set; }

        public int InitialDelayMilliseconds { get; set; }

        public int RepeatDelayMilliseconds { get; set; }

        public KeyboardListenerSettings()
        {
            RepeatPress = true;
            InitialDelayMilliseconds = 800;
            RepeatDelayMilliseconds = 50;
        }

        public override KeyboardListener CreateListener()
        {
            return new KeyboardListener(this);
        }
    }
}
