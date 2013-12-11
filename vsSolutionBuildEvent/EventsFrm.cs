/*
 * Copyright (c) 2013 Developed by reg <entry.reg@gmail.com>
 * Distributed under the Boost Software License, Version 1.0
 * (See accompanying file LICENSE or copy at http://www.boost.org/LICENSE_1_0.txt)
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
    public partial class EventsFrm: Form, ITransferEnvironmentVariable
    {
        /// <summary>
        /// UI-helper for MSBuild Environment Variables
        /// </summary>
        protected EnvironmentVariablesFrm envVariables;

        private List<vsSolutionBuildEvent.SBEEvent> _solutionEvents = new List<vsSolutionBuildEvent.SBEEvent>();
        private readonly List<string> _checkedStatus                = new List<string> { "Disabled", "Enabled" };

        public EventsFrm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// implements main output for variable
        /// TODO: highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        public void outputName(string name, string project = null)
        {
            textBoxCommand.Select(textBoxCommand.SelectionStart, 0);

            if(project == null) {
                textBoxCommand.SelectedText = String.Format("$({0})", name);
            }
            else {
                textBoxCommand.SelectedText = String.Format("$({0}:{1})", name, project);
            }

            this.Focus();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            _saveData();
            _notice(false);
        }

        private void _saveData()
        {
            SBEEvent evt                = _solutionEvents[comboBoxEvents.SelectedIndex];
            evt.enabled                 = checkBoxStatus.Checked;
            evt.command                 = textBoxCommand.Text;
            evt.caption                 = textBoxCaption.Text;
            evt.interpreter             = comboBoxInterpreter.Text;
            evt.processHide             = checkBoxProcessHide.Checked;
            evt.waitForExit             = checkBoxWaitForExit.Checked;
            evt.processKeep             = checkBoxProcessKeep.Checked;
            evt.newline                 = comboBoxNewline.Text.Trim();
            evt.wrapper                 = comboBoxWrapper.Text.Trim();
            evt.modeScript              = radioModeScript.Checked;
            evt.parseVariablesMSBuild   = checkBoxParseVariables.Checked;

            try {
                Config.save();
            }
            catch(Exception e) {
                MessageBox.Show("Failed applying settings:\n" + e.Message, "Configuration of event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                "examples of how to use it and other detail, see on: bitbucket.org/3F \n\n\t\t\tentry.reg@gmail.com",
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
            SBEEvent evt                    = _solutionEvents[comboBoxEvents.SelectedIndex];
            checkBoxStatus.Checked          = evt.enabled;
            textBoxCommand.Text             = evt.command.Replace("\n", "\r\n");
            textBoxCaption.Text             = evt.caption;
            comboBoxInterpreter.Text        = evt.interpreter;
            checkBoxProcessHide.Checked     = evt.processHide;
            checkBoxWaitForExit.Checked     = evt.waitForExit;
            checkBoxProcessKeep.Checked     = evt.processKeep;
            comboBoxNewline.Text            = evt.newline;
            comboBoxWrapper.Text            = evt.wrapper;
            checkBoxParseVariables.Checked  = evt.parseVariablesMSBuild;

            if(evt.modeScript) {
                radioModeScript.Checked = true; 
            }
            else{
                radioModeFiles.Checked = true;
            }
        }

        private void envVariablesUIHelper()
        {
            if(envVariables != null && !envVariables.IsDisposed) {
                if(envVariables.WindowState != FormWindowState.Minimized) {
                    envVariables.Dispose();
                    envVariables = null;
                    return;
                }
                envVariables.Focus();
                return;
            }
            envVariables = new EnvironmentVariablesFrm(this);
            envVariables.Show();
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

        private void buttonEnvVariables_Click(object sender, EventArgs e)
        {
            envVariablesUIHelper();
        }

        private void textBoxCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Modifiers == Keys.Control && e.KeyCode == Keys.Space) {
                e.SuppressKeyPress = true;
                envVariablesUIHelper();
            }
        }
    }
}
