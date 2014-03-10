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
        private class _SBEWrap
        {
            public enum SBEEvetnType
            {
                SBEEvent,
                SBEEventEW,
                SBEEventOWP
            }

            public SBEEvent evt;
            public SBEEvetnType subtype = SBEEvetnType.SBEEvent;

            public _SBEWrap(SBEEvent evt, SBEEvetnType subtype)
            {
                this.evt = evt;
                this.subtype = subtype;
            }

            public _SBEWrap(SBEEvent evt)
            {
                this.evt = evt;
            }
        }

        /// <summary>
        /// UI-helper for MSBuild Environment Variables
        /// </summary>
        protected EnvironmentVariablesFrm envVariables;

        /// <summary>
        /// all types of SBE events
        /// </summary>
        private List<_SBEWrap> _solutionEvents = new List<_SBEWrap>();

        public EventsFrm()
        {
            InitializeComponent();
            foreach(DataGridViewRow row in dataGridViewOutput.Rows) {
                row.Height = dataGridViewOutput.RowTemplate.Height;
            }
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
            _SBEWrap sbe = _solutionEvents[comboBoxEvents.SelectedIndex];

            try
            {
                _saveData(sbe.evt);

                switch(sbe.subtype) {
                    case _SBEWrap.SBEEvetnType.SBEEventEW: {
                        _saveData((SBEEventEW)sbe.evt);
                        break;
                    }
                    case _SBEWrap.SBEEvetnType.SBEEventOWP: {
                        _saveData((SBEEventOWP)sbe.evt);
                        break;
                    }
                }

                Config.save();
            }
            catch(Exception e) {
                MessageBox.Show("Failed applying settings:\n" + e.Message, "Configuration of event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void _saveData(SBEEvent evt)
        {
            evt.enabled                 = checkBoxStatus.Checked;
            evt.command                 = textBoxCommand.Text;
            evt.caption                 = textBoxCaption.Text;
            evt.interpreter             = comboBoxInterpreter.Text;
            evt.processHide             = checkBoxProcessHide.Checked;
            evt.waitForExit             = checkBoxWaitForExit.Checked;
            evt.processKeep             = checkBoxProcessKeep.Checked;
            evt.newline                 = comboBoxNewline.Text.Trim();
            evt.wrapper                 = comboBoxWrapper.Text.Trim();
            evt.parseVariablesMSBuild   = checkBoxParseVariables.Checked;

            if(radioModeScript.Checked) {
                evt.mode = TModeCommands.Interpreter;
            }
            else if(radioModeFiles.Checked) {
                evt.mode = TModeCommands.File;
            }
            else if(radioModeOperation.Checked) {
                evt.mode = TModeCommands.Operation;
            }
        }

        private void _saveData(SBEEventEW evt)
        {
            evt.codes       = listBoxEW.Items.Cast<string>().ToList();
            evt.isWhitelist = radioCodesWhitelist.Checked;
        }

        private void _saveData(SBEEventOWP evt)
        {
            evt.eventsOWP = new List<TEventOWP>();
            foreach(DataGridViewRow row in dataGridViewOutput.Rows)
            {
                if(row.Cells[0].Value == null || row.Cells[1].Value == null) {
                    continue;
                }
                TEventOWP owp = new TEventOWP();
                owp.term      = row.Cells[0].Value.ToString();
                owp.type      = (TEventOWPTerm)Enum.Parse(typeof(TEventOWPTerm), row.Cells[1].Value.ToString());
                evt.eventsOWP.Add(owp);
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
            _solutionEvents.Add(new _SBEWrap(Config.data.preBuild));
            comboBoxEvents.Items.Add("Pre-Build     (Before building solution)");

            _solutionEvents.Add(new _SBEWrap(Config.data.postBuild));
            comboBoxEvents.Items.Add("Post-Build    (After building solution)");

            _solutionEvents.Add(new _SBEWrap(Config.data.cancelBuild));
            comboBoxEvents.Items.Add("Cancel-Build  (by user or it's errors of compilation)");

            _solutionEvents.Add(new _SBEWrap(Config.data.warningBuild, _SBEWrap.SBEEvetnType.SBEEventEW));
            comboBoxEvents.Items.Add("Warning-Build (Warnings during assembly)");

            _solutionEvents.Add(new _SBEWrap(Config.data.errorsBuild, _SBEWrap.SBEEvetnType.SBEEventEW));
            comboBoxEvents.Items.Add("Errors-Build  (Errors during assembly)");

            _solutionEvents.Add(new _SBEWrap(Config.data.outputCustomBuild, _SBEWrap.SBEEvetnType.SBEEventOWP));
            comboBoxEvents.Items.Add("Output-Build customization (Full control)");

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
            if(checkBoxStatus.Checked) {
                checkBoxStatus.Text = "Enabled";
                this.textBoxCommand.BackColor = System.Drawing.Color.FromArgb(242, 250, 241);
            }
            else{
                checkBoxStatus.Text = "Disabled";
                this.textBoxCommand.BackColor = System.Drawing.Color.FromArgb(248, 243, 243);
            }
        }

        private void _notice(bool isOn)
        {
            if(isOn) {
                btnApply.FlatAppearance.BorderColor = Color.FromArgb(255, 0, 0);
                return;
            }
            btnApply.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0);
        }

        private void _renderData()
        {
            _SBEWrap sbe = _solutionEvents[comboBoxEvents.SelectedIndex];
            _renderData(sbe.evt);

            switch(sbe.subtype) {
                case _SBEWrap.SBEEvetnType.SBEEventEW: {
                    _renderData((SBEEventEW)sbe.evt);
                    groupBoxEW.Enabled = true;
                    groupBoxOutputControl.Enabled = false;
                    return;
                }
                case _SBEWrap.SBEEvetnType.SBEEventOWP: {
                    _renderData((SBEEventOWP)sbe.evt);
                    groupBoxEW.Enabled = false;
                    groupBoxOutputControl.Enabled = true;
                    return;
                }
                default:{
                    groupBoxEW.Enabled = false;
                    groupBoxOutputControl.Enabled = false;
                    return;
                }
            }
        }

        private void _renderData(SBEEvent evt)
        {
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

            switch(evt.mode) {
                case TModeCommands.Interpreter: {
                    radioModeScript.Checked = true;
                    break;
                }
                case TModeCommands.File: {
                    radioModeFiles.Checked = true;
                    break;
                }
                case TModeCommands.Operation: {
                    radioModeOperation.Checked = true;
                    break;
                }
            }
        }

        private void _renderData(SBEEventEW evt)
        {
            listBoxEW.Items.Clear();
            listBoxEW.Items.AddRange(evt.codes.ToArray());

            if(evt.isWhitelist) {
                radioCodesWhitelist.Checked = true;
            }
            else {
                radioCodesBlacklist.Checked = true;
            }
        }

        private void _renderData(SBEEventOWP evt)
        {
            dataGridViewOutput.Rows.Clear();
            foreach(TEventOWP owp in evt.eventsOWP) {
                dataGridViewOutput.Rows.Add(owp.term, owp.type.ToString());
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
            labelToCommandBox.Text      = "Command script:";
            groupBoxInterpreter.Enabled = true;
            listBoxOperation.Enabled    = false;
            //listBoxOperation.ClearSelected();
        }

        private void radioModeFiles_CheckedChanged(object sender, EventArgs e)
        {
            labelToCommandBox.Text      = "Files to execute (separated by enter key):";            
            groupBoxInterpreter.Enabled = false;
            listBoxOperation.Enabled    = false;
            //listBoxOperation.ClearSelected();
        }

        private void radioModeOperation_CheckedChanged(object sender, EventArgs e)
        {
            labelToCommandBox.Text      = "DTE execute (separated by enter key):";            
            groupBoxInterpreter.Enabled = true;
            listBoxOperation.Enabled    = true;
        }

        private void listBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxOperation.SelectedIndex == listBoxOperation.Items.Count - 1) {
                textBoxCommand.Enabled = true;
            }
            else {
                textBoxCommand.Enabled = false;
            }
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

        private void dataGridViewOutput_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 2 && e.RowIndex + 1 < dataGridViewOutput.Rows.Count) {
                dataGridViewOutput.Rows.Remove(dataGridViewOutput.Rows[e.RowIndex]);
            }
        }

        private void btnEWAdd_Click(object sender, EventArgs e)
        {
            string code = textBoxEW.Text.Trim();
            if(code.Length < 1) {
                return;
            }
            if(!listBoxEW.Items.Contains(code)) {
                listBoxEW.Items.Add(code);
            }
        }

        private void btnEWRemove_Click(object sender, EventArgs e)
        {
            if(listBoxEW.SelectedIndex < 0) {
                return;
            }
            listBoxEW.Items.RemoveAt(listBoxEW.SelectedIndex);
        }
    }
}
