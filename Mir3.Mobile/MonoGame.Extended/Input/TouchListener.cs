using Client.Controls;
using Client.Envir;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MonoGame.Extended.ViewportAdapters;
using System;

namespace MonoGame.Extended.Input
{
    public enum TouchStickStyle
    {
        FreeFollow,
        Free,
        Fixed
    }

    public class TouchListener : InputListener
    {
        private bool _isDragged;

        private Vector2 _dragAnchor = Vector2.Zero;

        public ViewportAdapter ViewportAdapter { get; set; }

        public event EventHandler<TouchEventArgs> TouchStarted;

        public event EventHandler<TouchEventArgs> TouchEnded;

        public event EventHandler<TouchEventArgs> TouchMoved;

        public event EventHandler<TouchEventArgs> TouchCancelled;

        public TouchListener()
            : this(new TouchListenerSettings())
        {
        }

        public TouchListener(ViewportAdapter viewportAdapter)
            : this(new TouchListenerSettings())
        {
            ViewportAdapter = viewportAdapter;
        }

        public TouchListener(TouchListenerSettings settings)
        {
            ViewportAdapter = settings.ViewportAdapter;
        }
#if ANDROID
        private ButtonState? _preButtonState;
#endif
        public override void Update(GameTime gameTime)
        {
            TouchCollection state = TouchPanel.GetState();
            if (state.Count > 0)
            {
                //事件顺序 TouchDown TouchMove(手指按住后有移动) TouchUp
                for (int i = 0; i < state.Count; i++)
                {
                    TouchLocation location = state[i];
                    if (TouchLocationState.Pressed == location.State)
                    {
                        CheckTouchDown(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, location));
                        _dragAnchor = location.Position;
                        _isDragged = true;
                    }
                    if (_isDragged && TouchLocationState.Moved == location.State)
                    {
                        CheckTouchMoved(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, location.Position, location.Position - _dragAnchor));
                        _dragAnchor = location.Position;
                    }
                    if (_isDragged && TouchLocationState.Released == location.State)
                    {
                        CheckTouchUp(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, location));
                        _isDragged = false;
                    }
                }
            }
            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gestureSample = TouchPanel.ReadGesture();
                switch (gestureSample.GestureType)
                {
                    case GestureType.Tap:
                        CheckTap(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, gestureSample.Position, gestureSample.Delta));
                        break;
                    case GestureType.DoubleTap:
                        CheckDoubleTap(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, gestureSample.Position, gestureSample.Delta));
                        break;
                    case GestureType.FreeDrag:
                        CheckFreeDrag(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, gestureSample.Position, gestureSample.Delta));
                        break;
                    case GestureType.DragComplete:
                        CheckDragComplete(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, gestureSample.Position, gestureSample.Delta));
                        break;
                    case GestureType.Hold:
                        CheckHold(new TouchEventArgs(ViewportAdapter, gameTime.TotalGameTime, gestureSample.Position, gestureSample.Delta));
                        break;
                }
            }
#if ANDROID
            if (_preButtonState.HasValue && _preButtonState == ButtonState.Pressed && GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Released)
            {
                DXControl.ActiveScene.PreSceneEvent?.Invoke();
            }

            _preButtonState = GamePad.GetState(PlayerIndex.One).Buttons.Back;
#endif
        }

        private void CheckTouchDown(TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"down{e.Location.ToString()}");
            CEnvir.MouseLocation = CEnvir.UIScale == 1F ? e.Location : new System.Drawing.Point((int)Math.Round((e.Location.X - CEnvir.UI_Offset_X) / CEnvir.UIScale), (int)Math.Round(e.Location.Y / CEnvir.UIScale));
            try
            {
                DXControl.ActiveScene?.OnTouchDown(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckTouchUp(TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"up{e.Location.ToString()}");
            try
            {
                DXControl.ActiveScene?.OnTouchUp(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckTap(TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"tap{e.Location.ToString()}");
            try
            {
                DXControl.ActiveScene?.OnTap(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckDoubleTap(TouchEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"doubletap{e.Location.ToString()}");
            try
            {
                DXControl.ActiveScene?.OnDoubleTap(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckTouchMoved(TouchEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine($"move{e.Location.ToString()}");
            //if (DXControl.ActiveScene != null && (DXControl.ActiveScene is not GameScene || !GameScene.Game.DrawStick))
            CEnvir.MouseLocation = CEnvir.UIScale == 1F ? e.Location : new System.Drawing.Point((int)Math.Round((e.Location.X - CEnvir.UI_Offset_X) / CEnvir.UIScale), (int)Math.Round(e.Location.Y / CEnvir.UIScale));
            try
            {
                DXControl.ActiveScene?.OnTouchMoved(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckFreeDrag(TouchEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnFreeDrag(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckDragComplete(TouchEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnDragComplete(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }

        private void CheckHold(TouchEventArgs e)
        {
            try
            {
                DXControl.ActiveScene?.OnCheckHold(e);
            }
            catch (Exception ex)
            {
                CEnvir.SaveError(ex.ToString());
            }
        }
    }

    public class TouchListenerSettings : InputListenerSettings<TouchListener>
    {
        public ViewportAdapter ViewportAdapter { get; set; }

        public override TouchListener CreateListener()
        {
            return new TouchListener(this);
        }
    }
}
