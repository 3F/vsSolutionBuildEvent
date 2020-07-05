/*
 * Copyright (c) 2013-2016,2019-2020  Denis Kuzmin < x-3F@outlook.com > GitHub/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
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
