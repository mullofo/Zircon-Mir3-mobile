using Client.Envir;
using Client.Scenes.Views;
using Library;
using System;
using System.Drawing;
using System.Windows.Forms;


namespace Client.Controls
{
    /// <summary>
    /// 场景控件
    /// </summary>
    public abstract class DXScene : DXControl
    {
        #region Properties
        /// <summary>
        /// 确认窗界面
        /// </summary>
        public DXConfirmWindow ConfirmWindow;

        public DXControl ClickControl;
        public DateTime ClickTime;
        public MouseButtons Buttons;
        /// <summary>
        /// 大小
        /// </summary>
        public sealed override Size Size
        {
            get => base.Size;
            set => base.Size = value;
        }
        /// <summary>
        /// 外景更改时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnLocationChanged(Point oValue, Point nValue)
        {
            base.OnLocationChanged(oValue, nValue);

            if (DebugLabel == null || PingLabel == null) return;

            DebugLabel.Location = new Point(Location.X + 5, Location.Y + 5);

            PingLabel.Location = new Point(Location.X + 5, Location.Y + 19);
        }
        /// <summary>
        /// 可见改变时
        /// </summary>
        /// <param name="oValue"></param>
        /// <param name="nValue"></param>
        public override void OnIsVisibleChanged(bool oValue, bool nValue)
        {
            base.OnIsVisibleChanged(oValue, nValue);

            if (!IsVisible) return;

            foreach (DXComboBox box in DXComboBox.ComboBoxes)
                box.ListBox.Parent = this;
        }
        #endregion
        /// <summary>
        /// 场景控件
        /// </summary>
        /// <param name="size"></param>
        protected DXScene(Size size)
        {
            DrawTexture = false;

            Size = size;

            DXManager.SetResolution(size);
        }

        #region Methods
        /// <summary>
        /// 鼠标按下时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDown(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            if (MouseControl != null && MouseControl != this)
                MouseControl.OnMouseDown(e);
            else
                base.OnMouseDown(e);

            DXControl listbox = MouseControl;

            while (listbox != null)
            {
                if (listbox is DXListBox) break;

                listbox = listbox.Parent;
            }

            foreach (DXComboBox box in DXComboBox.ComboBoxes)
            {
                if (box.ListBox != listbox)
                    box.Showing = false;
            }
        }
        /// <summary>
        /// 鼠标向上时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseUp(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            if (MouseControl != null && MouseControl != this)
                MouseControl.OnMouseUp(e);
            else
                base.OnMouseUp(e);
        }
        /// <summary>
        /// 鼠标移动时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseMove(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            if (FocusControl != null && FocusControl != this && FocusControl is MapControl)
                FocusControl.OnMouseMove(e);
            else if (MouseControl != null && MouseControl != this && (MouseControl.IsMoving || MouseControl.IsResizing))
                MouseControl.OnMouseMove(e);
            else
                base.OnMouseMove(e);
        }
        /// <summary>
        /// 鼠标单击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            if (Buttons == e.Button)
            {
                if (ClickTime.AddMilliseconds(SystemInformation.DoubleClickTime) >= Time.Now)
                {
                    OnMouseDoubleClick(e);
                    return;
                }
            }
            else ClickTime = DateTime.MinValue;

            if (MouseControl != null && MouseControl != this)
            {
                if (MouseControl == FocusControl)
                    MouseControl.OnMouseClick(e);
            }
            else
                base.OnMouseClick(e);

            ClickControl = MouseControl;

            ClickTime = CEnvir.Now;
            Buttons = e.Button;
        }
        /// <summary>
        /// 鼠标双击时
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (!IsEnabled) return;


            if (MouseControl != null && MouseControl != this)
            {
                if (MouseControl == ClickControl)
                {
                    MouseControl.OnMouseDoubleClick(e);
                    ClickTime = DateTime.MinValue;
                }
                else
                {
                    MouseControl.OnMouseClick(e);
                    ClickTime = CEnvir.Now;
                }
            }

            ClickControl = MouseControl;
        }
        /// <summary>
        /// 鼠标滚轮
        /// </summary>
        /// <param name="e"></param>
        public override void OnMouseWheel(MouseEventArgs e)
        {
            if (!IsEnabled) return;

            if (MouseControl != null && MouseControl != this)
                MouseControl.OnMouseWheel(e);
            else
                base.OnMouseWheel(e);
        }
        /// <summary>
        /// 拉伸绘制
        /// </summary>
        protected override void OnAfterDraw()
        {
            base.OnAfterDraw();

            /*
            DXManager.Sprite.Flush();
            if (!Location.IsEmpty)
                DXManager.Device.Clear(ClearFlags.Target, Color.Black, 1, 0, new[]
                {
                    new Rectangle(0, 0, Location.X > 0 ? Location.X : ScreenSize.Width, Location.X == 0 ? Location.Y : ScreenSize.Height),
                    new Rectangle(Location.X > 0 ? Size.Width + Location.X : 0,
                                  Location.X == 0 ? Size.Height + Location.Y : 0,
                                  Location.X > 0 ? Location.X : ScreenSize.Width,
                                  Location.X == 0 ? Location.Y : ScreenSize.Height)
                });
            */

            DebugLabel.Draw();

            if (!string.IsNullOrEmpty(HintLabel.Text))
                HintLabel.Draw();

            if (!string.IsNullOrEmpty(PingLabel.Text))
                PingLabel.Draw();
        }
        /// <summary>
        /// 检查是否可见
        /// </summary>
        protected internal sealed override void CheckIsVisible()
        {
            IsVisible = Visible && ActiveScene == this;

            foreach (DXControl control in Controls)
                control.CheckIsVisible();

        }
        #endregion

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                ClickControl = null;
                ClickTime = DateTime.MinValue;
                Buttons = MouseButtons.None;
            }
        }
        #endregion
    }
}
