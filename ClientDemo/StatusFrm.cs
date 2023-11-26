﻿/*! ClientDemo project
 *  Public domain.
 *  Example of using the API https://github.com/3F/vsSolutionBuildEvent
*/

using System.Diagnostics;
using System.Windows.Forms;

namespace ClientDemo
{
    public partial class StatusFrm: Form, IStatus
    {
        /// <summary>
        /// Report about status
        /// </summary>
        /// <param name="message"></param>
        public void report(string message)
        {
            push(richTextBoxMain, message);
        }

        /// <summary>
        /// Show form
        /// </summary>
        public void show()
        {
            ShowDialog();
        }

        public StatusFrm()
        {
            InitializeComponent();
        }

        protected void push(RichTextBox box, string message)
        {
            // box.InvokeRequired may does not check properly
            try {
                box.Text += message;
            }
            catch
            {
                box.Invoke((MethodInvoker)delegate {
                    box.Text += message;
                });
            }
        }

        private void btnAPI_Click(object sender, System.EventArgs e)
        {
            Process.Start("https://3F.github.io/web.vsSBE/doc/API/");
        }

        private void btnSrc_Click(object sender, System.EventArgs e)
        {
            Process.Start("https://github.com/3F/vsSolutionBuildEvent/tree/master/ClientDemo");
        }

        private void btnCopy_Click(object sender, System.EventArgs e)
        {
            richTextBoxMain.SelectAll();
            richTextBoxMain.Copy();
            richTextBoxMain.Focus();
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            richTextBoxMain.Clear();
        }

        private void chkPin_CheckedChanged(object sender, System.EventArgs e)
        {
            TopMost = chkPin.Checked;
        }

        private void StatusFrm_Load(object sender, System.EventArgs e)
        {
            chkPin_CheckedChanged(sender, e);
        }
    }
}
