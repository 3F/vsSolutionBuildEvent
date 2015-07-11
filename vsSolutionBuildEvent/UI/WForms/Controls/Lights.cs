using System.Drawing;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    public partial class Lights: UserControl
    {
        public class PanelDt: Panel
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.Gray, ButtonBorderStyle.Dotted);
            }
        }

        public enum FlashType
        {
            Red,
            Yellow,
            Green,
        }

        public void flush()
        {
            red(false);
            yellow(false);
            green(false);
        }

        public void switchOn(FlashType type)
        {
            switch(type) {
                case FlashType.Red: {
                    switchOnRed();
                    return;
                }
                case FlashType.Yellow: {
                    switchOnYellow();
                    return;
                }
                case FlashType.Green: {
                    switchOnGreen();
                    return;
                }
            }
        }

        public void switchOnRed()
        {
            red(true);
            yellow(false);
            green(false);
        }

        public void switchOnYellow()
        {
            red(false);
            yellow(true);
            green(false);
        }

        public void switchOnGreen()
        {
            red(false);
            yellow(false);
            green(true);
        }

        public void red(bool enabled)
        {
            pRed.BackColor = (enabled)? Color.FromArgb(224, 13, 1) : Color.FromArgb(223, 210, 210);
        }

        public void yellow(bool enabled)
        {
            pYellow.BackColor = (enabled)? Color.FromArgb(255, 248, 29) : Color.FromArgb(240, 239, 193);
        }

        public void green(bool enabled)
        {
            pGreen.BackColor = (enabled)? Color.FromArgb(144, 191, 105) : Color.FromArgb(203, 225, 185);
        }

        public Lights()
        {
            InitializeComponent();
            flush();
        }
    }
}
