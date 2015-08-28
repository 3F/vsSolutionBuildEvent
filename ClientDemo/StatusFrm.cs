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
            richTextBoxMain.Text += message;
        }

        /// <summary>
        /// Show form
        /// </summary>
        public void show()
        {
            Show();
        }

        public StatusFrm()
        {
            InitializeComponent();
        }

        private void btnAPI_Click(object sender, System.EventArgs e)
        {
            Process.Start("http://vssbe.r-eg.net/doc/API/");
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
    }
}
