/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ClientDemo
{
    public partial class StatusFrm: Form, IStatus
    {
        private bool stop, anchor;

        public void Report(string message)
        {
            if(stop) return;

            if(rtbMain.InvokeRequired)
            {
                rtbMain.Invoke(() => Append(message));
            }
            else
            {
                Append(message);
            }
        }

        public StatusFrm()
        {
            InitializeComponent();
        }

        protected void Append(string message, bool newLine = false)
        {
            rtbMain.AppendText(message);
            if(newLine) rtbMain.AppendText(Environment.NewLine);

            if(anchor)
            {
                rtbMain.Select(rtbMain.TextLength, 0);
                rtbMain.ScrollToCaret();
            }
        }

        private void btnAPI_Click(object sender, EventArgs e)
            => Process.Start("https://3F.github.io/web.vsSBE/doc/API/");

        private void btnSrc_Click(object sender, EventArgs e)
            => Process.Start("https://github.com/3F/vsSolutionBuildEvent/tree/master/ClientDemo");

        private void btnCopy_Click(object sender, EventArgs e)
        {
            rtbMain.SelectAll();
            rtbMain.Copy();
            rtbMain.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e) => rtbMain.Clear();

        private void chkPin_CheckedChanged(object sender, EventArgs e) => TopMost = chkPin.Checked;

        private void StatusFrm_Load(object sender, EventArgs e) => chkPin_CheckedChanged(sender, e);

        private void StatusFrm_FormClosing(object sender, FormClosingEventArgs e) => stop = true;

        private void btnPause_Click(object sender, EventArgs e)
        {
            stop = !stop;
            Append($"{btnPause.Text} clicked", newLine: true);
            btnPause.Text = stop ? "Resume" : "Pause";
        }

        private void chkAnchor_CheckedChanged(object sender, EventArgs e) => anchor = chkAnchor.Checked;
    }
}
