using Client.Controls;
using Client.Envir;
using Client.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGame.Extended.Input
{
    public class MouseListener : InputListener
    {
        private MouseState _currentState;

        private bool _dragging;

        private GameTime _gameTime;

        private bool _hasDoubleClicked;

        private MouseEventArgs _mouseDownArgs;

        private MouseEventArgs _previousClickArgs;

        private MouseState _previousState;

        public ViewportAdapter ViewportAdapter { get; }

        public int DoubleClickMilliseconds { get; }

        public int DragThreshold { get; }

        public bool HasMouseMoved
        {
            get
            {
                if (_previousState.X == _currentState.X)
                {
                    return _previousState.Y != _currentState.Y;
                }
                return true;
            }
        }

        public event EventHandler<MouseEventArgs> MouseDown;

        public event EventHandler<MouseEventArgs> MouseUp;

        public event EventHandler<MouseEventArgs> MouseClicked;

        public event EventHandler<MouseEventArgs> MouseDoubleClicked;

        public event EventHandler<MouseEventArgs> MouseMoved;

        public event EventHandler<MouseEventArgs> MouseWheelMoved;

        public event EventHandler<MouseEventArgs> MouseDragStart;

        public event EventHandler<MouseEventArgs> MouseDrag;

        public event EventHandler<MouseEventArgs> MouseDragEnd;

        public MouseListener()
            : this(new MouseListenerSettings())
        {
        }

        public MouseListener(ViewportAdapter viewportAdapter)
            : this(new MouseListenerSettings())
        {
            ViewportAdapter = viewportAdapter;
        }

        public MouseListener(MouseListenerSettings settings)
        {
            ViewportAdapter = settings.ViewportAdapter;
            DoubleClickMilliseconds = settings.DoubleClickMilliseconds;
            DragThreshold = settings.DragThreshold;
        }

        private void CheckButtonPressed(Func<MouseState, ButtonState> getButtonState, MouseButtons button)
        {
            if (getButtonState(_currentState) != ButtonState.Pressed || getButtonState(_previousState) != 0)
            {
                return;
            }
            MouseEventArgs mouseEventArgs = new MouseEventArgs(ViewportAdapter, _gameTime.TotalGameTime, _previousState, _currentState, button);
            this.MouseDown?.Invoke(this, mouseEventArgs);
            _mouseDownArgs = mouseEventArgs;
            if (_previousClickArgs != null)
            {
                if ((mouseEventArgs.Time - _previousClickArgs.Time).TotalMilliseconds <= (double)DoubleClickMilliseconds)
                {
                    this.MouseDoubleClicked?.Invoke(this, mouseEventArgs);
                    _hasDoubleClicked = true;
                }
                _previousClickArgs = null;
            }
        }

        private void CheckButtonReleased(Func<MouseState, ButtonState> getButtonState, MouseButtons button)
        {
            if (getButtonState(_currentState) != 0 || getButtonState(_previousState) != ButtonState.Pressed)
            {
                return;
            }
            MouseEventArgs mouseEventArgs = new MouseEventArgs(ViewportAdapter, _gameTime.TotalGameTime, _previousState, _currentState, button);
            if (_mouseDownArgs.Button == mouseEventArgs.Button)
            {
                if (DistanceBetween(mouseEventArgs.Position, _mouseDownArgs.Position) < DragThreshold)
                {
                    if (!_hasDoubleClicked)
                    {
                        this.MouseClicked?.Invoke(this, mouseEventArgs);
                    }
                }
                else
                {
                    this.MouseDragEnd?.Invoke(this, mouseEventArgs);
                    _dragging = false;
                }
            }
            this.MouseUp?.Invoke(this, mouseEventArgs);
            _hasDoubleClicked = false;
            _previousClickArgs = mouseEventArgs;
        }

        private void CheckMouseDragged(Func<MouseState, ButtonState> getButtonState, MouseButtons button)
        {
            if (getButtonState(_currentState) != ButtonState.Pressed || getButtonState(_previousState) != ButtonState.Pressed)
            {
                return;
            }
            MouseEventArgs mouseEventArgs = new MouseEventArgs(ViewportAdapter, _gameTime.TotalGameTime, _previousState, _currentState, button);
            if (_mouseDownArgs.Button == mouseEventArgs.Button)
            {
                if (_dragging)
                {
                    this.MouseDrag?.Invoke(this, mouseEventArgs);
                }
                else if (DistanceBetween(mouseEventArgs.Position, _mouseDownArgs.Position) > DragThreshold)
                {
                    _dragging = true;
                    this.MouseDragStart?.Invoke(this, mouseEventArgs);
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            _gameTime = gameTime;
            _currentState = Mouse.GetState();
            CheckButtonPressed((MouseState s) => s.LeftButton, MouseButtons.Left);
            CheckButtonPressed((MouseState s) => s.MiddleButton, MouseButtons.Middle);
            CheckButtonPressed((MouseState s) => s.RightButton, MouseButtons.Right);
            CheckButtonPressed((MouseState s) => s.XButton1, MouseButtons.XButton1);
            CheckButtonPressed((MouseState s) => s.XButton2, MouseButtons.XButton2);
            CheckButtonReleased((MouseState s) => s.LeftButton, MouseButtons.Left);
            CheckButtonReleased((MouseState s) => s.MiddleButton, MouseButtons.Middle);
            CheckButtonReleased((MouseState s) => s.RightButton, MouseButtons.Right);
            CheckButtonReleased((MouseState s) => s.XButton1, MouseButtons.XButton1);
            CheckButtonReleased((MouseState s) => s.XButton2, MouseButtons.XButton2);
            if (HasMouseMoved)
            {
                this.MouseMoved?.Invoke(this, new MouseEventArgs(ViewportAdapter, gameTime.TotalGameTime, _previousState, _currentState));
                CheckMouseDragged((MouseState s) => s.LeftButton, MouseButtons.Left);
                CheckMouseDragged((MouseState s) => s.MiddleButton, MouseButtons.Middle);
                CheckMouseDragged((MouseState s) => s.RightButton, MouseButtons.Right);
                CheckMouseDragged((MouseState s) => s.XButton1, MouseButtons.XButton1);
                CheckMouseDragged((MouseState s) => s.XButton2, MouseButtons.XButton2);
                CheckMouseMove(new MouseEventArgs(ViewportAdapter, gameTime.TotalGameTime, _previousState, _currentState));
                CheckMouseDown(new MouseEventArgs(ViewportAdapter, gameTime.TotalGameTime, _previousState, _currentState));
            }
            if (_previousState.ScrollWheelValue != _currentState.ScrollWheelValue)
            {
                this.MouseWheelMoved?.Invoke(this, new MouseEventArgs(ViewportAdapter, gameTime.TotalGameTime, _previousState, _currentState));
            }
            _previousState = _currentState;
        }

        private static int DistanceBetween(Point a, Point b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        private void CheckMouseMove(MouseEventArgs e)
        {
            CEnvir.MouseLocation = CEnvir.UIScale == 1F ? e.Location : new System.Drawing.Point((int)Math.Round((e.Location.X - CEnvir.UI_Offset_X) / CEnvir.UIScale), (int)Math.Round(e.Location.Y / CEnvir.UIScale));
            try
            {
                DXControl.ActiveScene?.OnMouseMove(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckMouseDown(MouseEventArgs e)
        {
            if (GameScene.Game != null && e.Button == MouseButtons.Right && (GameScene.Game.SelectedCell != null || GameScene.Game.GoldPickedUp))
            {
                GameScene.Game.SelectedCell = null;
                GameScene.Game.GoldPickedUp = false;
                return;
            }
            try
            {
                DXControl.ActiveScene?.OnMouseDown(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
    }

    public class MouseListenerSettings : InputListenerSettings<MouseListener>
    {
        public int DragThreshold { get; set; }

        public int DoubleClickMilliseconds { get; set; }

        public ViewportAdapter ViewportAdapter { get; set; }

        public MouseListenerSettings()
        {
            DoubleClickMilliseconds = 500;
            DragThreshold = 2;
        }

        public override MouseListener CreateListener()
        {
            return new MouseListener(this);
        }
    }
}
