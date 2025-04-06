using Client.Controls;
using Client.Envir;
using Client.UserModels;
using System.Drawing;
using Font = MonoGame.Extended.Font;
using FontStyle = MonoGame.Extended.FontStyle;

namespace Client.Scenes.Views
{
    /// <summary>
    /// 顶部信息框
    /// </summary>
    public class TopInfoDialog : DXWindow
    {
        /// <summary>
        /// 时间标签
        /// </summary>
        public DXLabel TimeLabel;
        /// <summary>
        /// 经验标签
        /// </summary>
        public DXLabel EXPLabel;
        /// <summary>
        /// 武器经验标签
        /// </summary>
        public DXLabel WeaponEXPLabel;
        /// <summary>
        /// 背包负重标签
        /// </summary>
        public DXLabel BagWeightLabel;
        /// <summary>
        /// 模式标签
        /// </summary>
        public DXLabel ModeLabel;
        /// <summary>
        /// 异界之门开启时间
        /// </summary>
        public DXLabel NetherworldGateLabel;
        /// <summary>
        /// 赤龙石门开启时间
        /// </summary>
        public DXLabel JinamStoneGateLabel;

        #region Properties
        public override WindowType Type => WindowType.None;

        public override bool CustomSize => false;

        public override bool AutomaticVisibility => true;

        #endregion

        /// <summary>
        /// 顶部信息框
        /// </summary>
        public TopInfoDialog()
        {
            Size = new Size(800, 60);
            Movable = false;
            Opacity = 0F;
            PassThrough = true;
            CloseButton.Visible = false;
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;

            TimeLabel = new DXLabel
            {
                Parent = this,
                //Location = new Point(2, 2),
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8.5F), FontStyle.Regular),
            };

            EXPLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8F), FontStyle.Regular),
            };

            WeaponEXPLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8F), FontStyle.Regular),
            };

            BagWeightLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8F), FontStyle.Regular),
            };

            ModeLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8F), FontStyle.Regular),
            };

            NetherworldGateLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8F), FontStyle.Regular),
            };

            JinamStoneGateLabel = new DXLabel
            {
                Parent = this,
                ForeColour = Color.Yellow,
                Font = new Font("MS Sans Serif", CEnvir.FontSize(8F), FontStyle.Regular),
            };
        }

        #region IDisposable
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (TimeLabel != null)
                {
                    if (!TimeLabel.IsDisposed)
                        TimeLabel.Dispose();

                    TimeLabel = null;
                }

                if (EXPLabel != null)
                {
                    if (!EXPLabel.IsDisposed)
                        EXPLabel.Dispose();

                    EXPLabel = null;
                }

                if (WeaponEXPLabel != null)
                {
                    if (!WeaponEXPLabel.IsDisposed)
                        WeaponEXPLabel.Dispose();

                    WeaponEXPLabel = null;
                }

                if (BagWeightLabel != null)
                {
                    if (!BagWeightLabel.IsDisposed)
                        BagWeightLabel.Dispose();

                    BagWeightLabel = null;
                }

                if (ModeLabel != null)
                {
                    if (!ModeLabel.IsDisposed)
                        ModeLabel.Dispose();

                    ModeLabel = null;
                }

                if (NetherworldGateLabel != null)
                {
                    if (!NetherworldGateLabel.IsDisposed)
                        NetherworldGateLabel.Dispose();

                    NetherworldGateLabel = null;
                }

                if (JinamStoneGateLabel != null)
                {
                    if (!JinamStoneGateLabel.IsDisposed)
                        JinamStoneGateLabel.Dispose();

                    JinamStoneGateLabel = null;
                }
            }
        }
        #endregion
    }
}
