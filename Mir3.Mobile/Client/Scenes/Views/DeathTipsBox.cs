using Client.Envir;
using Client.Extentions;
using Library;
using MonoGame.Extended;
using System.Drawing;
using C = Library.Network.ClientPackets;

namespace Client.Controls
{
    /// <summary>
    /// 死亡提示面板
    /// </summary>
    public class DeathTipsBox : DXImageControl
    {
        /// <summary>
        /// 信息
        /// </summary>
        public DXLabel Label;
        /// <summary>
        /// 复活按钮
        /// </summary>
        public DXButton ReviveButton;

        public override void OnParentChanged(DXControl oValue, DXControl nValue)
        {
            base.OnParentChanged(oValue, nValue);
            if (Parent != null)
            {
                Location = new Point(0, 0);
            }
        }

        /// <summary>
        /// 死亡复活面板
        /// </summary>
        public DeathTipsBox()
        {
            LibraryFile = LibraryFile.GameInter;  //UI面板主图的索引位置
            Index = 663;  //UI主图序号
            PassThrough = true;  //穿透开启
            Movable = false;

            Label = new DXLabel
            {
                Location = new Point(65, 25),
                Parent = this,
                Text = "Chat.Revive".Lang(),
                ForeColour = Color.White,
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
            };

            ReviveButton = new DXButton
            {
                LibraryFile = LibraryFile.GameInter,
                Index = 657,
                Parent = this,
                Location = new Point(100, 70),
            };
            ReviveButton.MouseClick += (s, e) =>
            {
                CEnvir.Enqueue(new C.TownRevive());
                Visible = false;
            };
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (Label != null)
                {
                    if (!Label.IsDisposed)
                        Label.Dispose();
                    Label = null;
                }

                if (ReviveButton != null)
                {
                    if (!ReviveButton.IsDisposed)
                        ReviveButton.Dispose();
                    ReviveButton = null;
                }
            }
        }
        #endregion
    }
}