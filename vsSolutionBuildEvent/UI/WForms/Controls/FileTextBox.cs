/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    public partial class FileTextBox: UserControl
    {
        /// <summary>
        /// Initial Width property from textBox fname.
        /// </summary>
        private int fnameTextBoxWidth;

        /// <summary>
        /// Access to file name.
        /// </summary>
        [Browsable(true)]
        [Category("Control"), Description("Gets/Sets the file name from textBox.")]
        public string FileName
        {
            get { return fname.Text; }
            set { fname.Text = value; }
        }

        /// <summary>
        /// Flag of hidding the side button.
        /// </summary>
        [Browsable(true)]
        [Category("Control"), Description("Hide side button of calling dialog.")]
        public bool HideButton
        {
            get {
                return hideButton;
            }
            set {
                hideButton = value;
                buttonDialog(!value);
            }
        }
        protected bool hideButton;

        /// <summary>
        /// OpenFileDialog or SaveFileDialog
        /// </summary>
        [Browsable(true)]
        [Category("Control"), Description("Use FileDialog for saving file if true, otherwise for opening.")]
        public bool IsSaveDialog
        {
            get { return isSaveDialog; }
            set
            {
                isSaveDialog = value;
                if(isSaveDialog) {
                    Dialog = new SaveFileDialog() { OverwritePrompt = false };
                    return;
                }
                Dialog = new OpenFileDialog();
            }
        }
        protected bool isSaveDialog;

        /// <summary>
        /// Access to FileDialog.
        /// </summary>
        [Browsable(true)]
        [Category("Control"), Description("FileDialog settings.")]
        public FileDialog Dialog
        {
            get;
            protected set;
        }

        /// <summary>
        /// Flag of supressing InitialDirectory from path.
        /// </summary>
        [Browsable(true)]
        [Category("Control"), Description("Supress 'InitialDirectory' from path to file name.")]
        public bool SupressInitialDirectoryFromPath
        {
            get;
            set;
        }

        /// <summary>
        /// Use prefix for selected path if an is not rooted.
        /// </summary>
        [Browsable(true)]
        [Category("Control"), Description("Use prefix for selected path if an is not rooted.")]
        public string PrefixToPath
        {
            get;
            set;
        }

        /// <summary>
        /// Alias to FileName
        /// </summary>
        public override string Text
        {
            get { return FileName; }
            set { FileName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public FileTextBox()
        {
            InitializeComponent();
            fnameTextBoxWidth   = fname.Width;
            IsSaveDialog        = false;
        }

        protected void buttonDialog(bool enabled)
        {
            fdialog.Visible = enabled;
            fname.Width     = (enabled)? fnameTextBoxWidth : Width;
        }

        private void fdialog_Click(object sender, EventArgs e)
        {
            if(Dialog.ShowDialog() != DialogResult.OK) {
                return;
            }

            if(SupressInitialDirectoryFromPath && !String.IsNullOrEmpty(Dialog.InitialDirectory)) {
                fname.Text = Dialog.FileName.Replace(Dialog.InitialDirectory, String.Empty);
            }
            else {
                fname.Text = Dialog.FileName;
            }

            if(!Path.IsPathRooted(fname.Text) && !String.IsNullOrEmpty(PrefixToPath)) {
                fname.Text = PrefixToPath + fname.Text;
            }
        }
    }
}
