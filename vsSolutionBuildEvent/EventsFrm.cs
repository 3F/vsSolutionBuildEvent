/*
    * The MIT License (MIT)
    * 
    * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
    * 
    * Permission is hereby granted, free of charge, to any person obtaining a copy
    * of this software and associated documentation files (the "Software"), to deal
    * in the Software without restriction, including without limitation the rights
    * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    * copies of the Software, and to permit persons to whom the Software is
    * furnished to do so, subject to the following conditions:
    * 
    * The above copyright notice and this permission notice shall be included in
    * all copies or substantial portions of the Software.
    * 
    * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    * THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace reg.ext.vsSolutionBuildEvent
{
    public partial class EventsFrm : Form
    {
        private List<vsSolutionBuildEvent.Event> _solutionEvents    = new List<vsSolutionBuildEvent.Event>();
        private readonly List<string> _checkedStatus                = new List<string> { "Disabled", "Enabled" };

        public EventsFrm()
        {
            InitializeComponent();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            _saveData();
            _notice(false);
        }

        private void _saveData()
        {
            Event evt           = _solutionEvents[comboBoxEvents.SelectedIndex];
            evt.enabled         = checkBoxStatus.Checked;
            evt.command         = textBoxCommand.Text;
            evt.caption         = textBoxCaption.Text;
            evt.interpreter     = comboBoxInterpreter.Text;
            evt.processHide     = checkBoxProcessHide.Checked;
            evt.waitForExit     = checkBoxWaitForExit.Checked;
            evt.processKeep     = checkBoxProcessKeep.Checked;
            evt.newline         = comboBoxNewline.Text.Trim();
            evt.wrapper         = comboBoxWrapper.Text.Trim();
            evt.modeScript      = radioModeScript.Checked;
            Config.save();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textBoxCommand.Clear();
            textBoxCaption.Clear();
            checkBoxStatus.Checked = false;
            _notice(true);
        }

        private void EventsFrm_Load(object sender, EventArgs e)
        {
            _solutionEvents.Add(Config.data.preBuild);
            comboBoxEvents.Items.Add("Pre-Build (Before building solution)");

            _solutionEvents.Add(Config.data.postBuild);
            comboBoxEvents.Items.Add("Post-Build (After building solution)");

            _solutionEvents.Add(Config.data.cancelBuild);
            comboBoxEvents.Items.Add("Cancel-Build (When cancel building solution)");

            comboBoxEvents.SelectedIndex = 0;
            _renderData();
        }

        private void btnExample_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "examples of how to use it, see on: bitbucket.org/3F \n\n\t\t\tentry.reg@gmail.com",
                this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            _renderData();
            _notice(false);
        }

        private void textBoxCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            _notice(true);
        }

        private void textBoxCaption_KeyPress(object sender, KeyPressEventArgs e)
        {
            _notice(true);
        }

        private void checkBoxStatus_CheckedChanged(object sender, EventArgs e)
        {
            toolTip.SetToolTip(checkBoxStatus, checkBoxStatus.Checked ? _checkedStatus[1] : _checkedStatus[0]);
            if(checkBoxStatus.Checked) {
                this.textBoxCommand.BackColor = System.Drawing.Color.FromArgb(242, 250, 241);
            }
            else{
                this.textBoxCommand.BackColor = System.Drawing.Color.FromArgb(248, 243, 243);
            }
        }

        private void _notice(bool isOn)
        {
            if (isOn)
            {
                btnApply.FlatAppearance.BorderColor = Color.FromArgb(255, 0, 0);
                return;
            }
            btnApply.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0);
        }

        private void _renderData()
        {
            Event evt                       = _solutionEvents[comboBoxEvents.SelectedIndex];
            checkBoxStatus.Checked          = evt.enabled;
            textBoxCommand.Text             = evt.command.Replace("\n", "\r\n");
            textBoxCaption.Text             = evt.caption;
            comboBoxInterpreter.Text        = evt.interpreter;
            checkBoxProcessHide.Checked     = evt.processHide;
            checkBoxWaitForExit.Checked     = evt.waitForExit;
            checkBoxProcessKeep.Checked     = evt.processKeep;
            comboBoxNewline.Text            = evt.newline;
            comboBoxWrapper.Text            = evt.wrapper;

            if(evt.modeScript) {
                radioModeScript.Checked = true; 
            }
            else{
                radioModeFiles.Checked = true;
            }
        }

        private void radioModeScript_CheckedChanged(object sender, EventArgs e)
        {
            labelToInterpreterMode.Visible = true;
            labelToFilesMode.Visible = false;
            groupBoxMode.Enabled = true;
        }

        private void radioModeFiles_CheckedChanged(object sender, EventArgs e)
        {
            labelToInterpreterMode.Visible = false;
            labelToFilesMode.Visible = true;
            groupBoxMode.Enabled = false;
        }

        private void checkBoxProcessHide_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxProcessKeep.Enabled = !checkBoxProcessHide.Checked;
        }
    }
}
