using Client.Controls;
using Client.Envir;
using Client.UserModels;
using Library;
using System.Collections.Generic;
using System.Drawing;
using C = Library.Network.ClientPackets;


namespace Client.Scenes.Views
{
    /// <summary>
    /// 中间置顶快捷图标按钮功能
    /// </summary>
    public sealed class ShortcutDialog : DXWindow
    {
        public DXButton ExpandButton;

        public List<ShortcutIcon> Icons;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => true;

        /// <summary>
        /// 图标快捷按钮界面
        /// </summary>
        public ShortcutDialog()
        {
            HasTitle = false;              //不显示标题
            HasFooter = false;             //不显示页脚
            HasTopBorder = false;          //不显示上边框
            TitleLabel.Visible = false;    //不显示标签文本
            CloseButton.Visible = false;   //不显示关闭按钮
            Opacity = 0F;                  //完全透明
            PassThrough = true;            //穿透开启
            AllowDragOut = true;           //允许拖动

            Size = new Size(1200, 100);

            /*ExpandButton = new DXButton   //显示按钮
            {
                Index = 90,
                LibraryFile = LibraryFile.Interface1c,
                Location = new Point(300, 5),
                Parent = this,
                Visible = true,
                Opacity = 0F,
            };
            ExpandButton.MouseClick += (sender, args) =>  //鼠标点击
            {
                CEnvir.Enqueue(new C.ShortcutDialogClicked { });

                if (ExpandButton.Hint == "显示")
                {
                    ExpandButton.Hint = "隐藏";
                    ExpandButton.Index = 85;
                    ToggleIcons(true);
                }
                else
                {
                    ExpandButton.Hint = "显示";
                    ExpandButton.Index = 90;
                    ToggleIcons(false);
                }
            };*/

            Icons = new List<ShortcutIcon>();
            //CEnvir.Enqueue(new C.ShortcutDialogClicked { });
        }
        /// <summary>
        /// 切换图标
        /// </summary>
        /// <param name="visible"></param>
        public void ToggleIcons(bool visible)
        {
            for (int i = 0; i < Icons.Count; i++)
            {
                ShortcutIcon icon = Icons[i];
                icon.Shortcut.Location = new Point(270 + 40 + icon.Size.Width * i, (icon.Size.Height - 2) / 2 + 8);
                icon.Shortcut.Parent = this;
                icon.Shortcut.Visible = visible;
            }
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                /*if (ExpandButton != null)
                {
                    if (!ExpandButton.IsDisposed)
                        ExpandButton.Dispose();

                    ExpandButton = null;
                }*/

                if (Icons != null)
                {
                    foreach (ShortcutIcon icon in Icons)
                    {
                        icon.Dispose();
                    }
                }

            }
        }
        #endregion
    }

    /// <summary>
    /// 快捷按钮图标
    /// </summary>
    public sealed class ShortcutIcon : DXControl
    {
        public DXImageControl Shortcut;
        /// <summary>
        /// 快捷按钮图标
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="npcIndex"></param>
        public ShortcutIcon(int index, Size size, uint npcIndex)
        {
            IsControl = true;
            Size = size;
            Opacity = 1F;
            AllowDragOut = true;

            Shortcut = new DXImageControl   //快捷按钮
            {
                Parent = this,
                LibraryFile = LibraryFile.Interface,
                Index = index,
                Opacity = 1F,
                Location = Location,
                IsControl = true,
                PassThrough = false,
                Visible = Visible,
                ForeColour = Color.LightGray
            };

            Shortcut.MouseClick += (sender, args) =>  //鼠标点击时
            {
                CEnvir.Enqueue(new C.NPCCall { ObjectID = npcIndex, isDKey = true });
            };

            Shortcut.MouseEnter += (sender, args) =>   //鼠标进入时
            {
                Shortcut.ForeColour = Color.White;
            };

            Shortcut.MouseLeave += (sender, args) =>   //鼠标离开时
            {
                Shortcut.ForeColour = Color.LightGray;
            };

            Shortcut.MouseDown += (sender, args) =>   //鼠标向下移动
            {
                Shortcut.Location = new Point(Shortcut.Location.X, Shortcut.Location.Y + 1);
            };

            Shortcut.MouseUp += (sender, args) =>   //鼠标向上移动
            {
                Shortcut.Location = new Point(Shortcut.Location.X, Shortcut.Location.Y - 1);
            };
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Shortcut != null)
                {
                    if (!Shortcut.IsDisposed)
                        Shortcut.Dispose();

                    Shortcut = null;
                }

            }
        }
        #endregion
    }
}
