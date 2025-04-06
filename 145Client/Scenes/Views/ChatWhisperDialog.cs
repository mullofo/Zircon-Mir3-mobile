using Client.Controls;
using Client.Envir;
using Client.UserModels;
using System.Drawing;

namespace Client.Scenes.Views
{
    public sealed class ChatWhisperDialog : DXWindow
    {
        public DXLabel[] WhispersLabel;

        public DXLabel Selected;

        public override WindowType Type => WindowType.None;
        public override bool CustomSize => false;
        public override bool AutomaticVisibility => false;

        public ChatWhisperDialog()
        {
            HasTitle = false;
            HasFooter = false;
            HasTopBorder = false;
            TitleLabel.Visible = false;
            CloseButton.Visible = false;

            WhispersLabel = new DXLabel[10];
            BackColour = Color.FromArgb(247, 140, 33);
            Opacity = 0.8f;
            Border = true;
            BorderColour = Color.FromArgb(165, 82, 82);
            Size = new Size(110, 162);
            DrawTexture = true;
            IsControl = true;
            Visible = false;

            for (int i = 0; i < WhispersLabel.Length; i++)
            {
                WhispersLabel[i] = new DXLabel
                {
                    AutoSize = false,
                    ForeColour = Color.White,
                    Size = new Size(98, 16),
                    BackColour = Color.FromArgb(255, 66, 8),
                    Location = new Point(1, 1 + 16 * i),
                    Font = new Font(Config.FontName, CEnvir.FontSize(9F)),
                    Parent = this,
                    Visible = !string.IsNullOrEmpty(Text)
                };
            }
            if (!string.IsNullOrWhiteSpace(WhispersLabel[0].Text))
            {
                Selected = WhispersLabel[0];
            }
            UpdateInferface();
        }

        public void JoinWhisper(string s)
        {
            foreach (DXLabel dxlabel in WhispersLabel)
            {
                if (dxlabel.Text == s)
                {
                    return;
                }
            }
            for (int j = 0; j < WhispersLabel.Length; j++)
            {
                if (string.IsNullOrWhiteSpace(WhispersLabel[j].Text))
                {
                    WhispersLabel[j].Text = s;
                    WhispersLabel[j].Visible = !string.IsNullOrEmpty(WhispersLabel[j].Text);
                    return;
                }
            }
            for (int k = 0; k < WhispersLabel.Length; k++)
            {
                if (k == WhispersLabel.Length - 1)
                {
                    WhispersLabel[k].Text = s;
                }
                else
                {
                    WhispersLabel[k].Text = WhispersLabel[k + 1].Text;
                }
                WhispersLabel[k].Visible = !string.IsNullOrEmpty(WhispersLabel[k].Text);
            }
        }

        public void MoveDown()
        {
            int num = -1;
            for (int i = 0; i < WhispersLabel.Length; i++)
            {
                if (WhispersLabel[i] == Selected)
                {
                    num = i;
                }
            }
            if (num + 1 < WhispersLabel.Length && !string.IsNullOrWhiteSpace(WhispersLabel[num + 1].Text))
            {
                Selected = WhispersLabel[num + 1];
            }
            UpdateInferface();
        }

        public void MoveUp()
        {
            int num = -1;
            for (int i = 0; i < WhispersLabel.Length; i++)
            {
                if (WhispersLabel[i] == Selected)
                {
                    num = i;
                }
            }
            if (num - 1 >= 0 && !string.IsNullOrWhiteSpace(WhispersLabel[num - 1].Text))
            {
                Selected = WhispersLabel[num - 1];
            }
            UpdateInferface();
        }

        public void UpdateInferface()
        {
            foreach (DXLabel dxlabel in WhispersLabel)
            {
                dxlabel.Visible = !string.IsNullOrEmpty(dxlabel.Text);
                if (dxlabel == Selected && !string.IsNullOrWhiteSpace(Selected.Text))
                {
                    dxlabel.BackColour = Color.FromArgb(255, 66, 8);
                }
                else
                {
                    dxlabel.BackColour = Color.Empty;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                for (int i = 0; i < WhispersLabel.Length; i++)
                {
                    if (WhispersLabel[i] != null && WhispersLabel[i].IsDisposed)
                    {
                        WhispersLabel[i].Dispose();
                    }
                }
            }
        }
    }
}
