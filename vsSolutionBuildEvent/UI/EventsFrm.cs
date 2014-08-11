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
using EnvDTE80;

namespace net.r_eg.vsSBE.UI
{
    public partial class EventsFrm: Form, ITransferEnvironmentVariable
    {
        private class _SBEWrap
        {
            public enum SBEEvetnType
            {
                SBEEvent,
                SBEEventEW,
                SBEEventOWP,
                SBETransmitter,
                SBEEventPost
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

        /// <summary>
        /// Current SBE
        /// </summary>
        private _SBEWrap _SBE
        {
            get { return _solutionEvents[comboBoxEvents.SelectedIndex]; }
        }

        /// <summary>
        /// Predefined operations
        /// TODO:
        /// </summary>
        private List<TOperation> _listOperations = DefCommandsDTE.operations();

        /// <summary>
        /// UI-helper for DTE Commands
        /// </summary>
        private DTECommandsFrm _frmDTECommands;
        private DTE2 _dte;

        public EventsFrm(DTE2 dte)
        {
            _dte = dte;
            InitializeComponent();
            foreach(DataGridViewRow row in dataGridViewOutput.Rows) {
                row.Height = dataGridViewOutput.RowTemplate.Height;
            }
            _notice(typeof(CheckBox));
            _notice(typeof(RadioButton));
            _notice(typeof(TextBox));
            _notice(typeof(ListBox));
            _notice(typeof(ComboBox));
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
            try
            {
                _saveData(_SBE.evt);

                switch(_SBE.subtype) {
                    case _SBEWrap.SBEEvetnType.SBEEventEW: {
                        _saveData((SBEEventEW)_SBE.evt);
                        break;
                    }
                    case _SBEWrap.SBEEvetnType.SBEEventOWP: {
                        _saveData((SBEEventOWP)_SBE.evt);
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
            evt.caption                 = textBoxCaption.Text;
            evt.interpreter             = comboBoxInterpreter.Text;
            evt.processHide             = checkBoxProcessHide.Checked;
            evt.waitForExit             = checkBoxWaitForExit.Checked;
            evt.processKeep             = checkBoxProcessKeep.Checked;
            evt.newline                 = comboBoxNewline.Text;
            evt.wrapper                 = comboBoxWrapper.Text.Trim();
            evt.parseVariablesMSBuild   = checkBoxParseVariables.Checked;
            evt.buildFailedIgnore       = checkBoxIgnoreIfFailed.Checked;

            if(radioModeScript.Checked) {
                evt.mode = TModeCommands.Interpreter;
            }
            else if(radioModeFiles.Checked) {
                evt.mode = TModeCommands.File;
            }
            else if(radioModeOperation.Checked) {
                evt.mode = TModeCommands.Operation;
            }

            //joint .. TODO:
            if(!radioModeOperation.Checked) {
                evt.command = textBoxCommand.Text;
            }
            else {
                if(_isOperationCustomUse()){
                    evt.dteExec.cmd = textBoxCommand.Text.Split('\n');
                }
                else{
                    evt.dteExec.cmd = _listOperations[listBoxOperation.SelectedIndex].cmd;
                }
                evt.dteExec.caption = listBoxOperation.Text;
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
            comboBoxEvents_SelectedIndexChanged(sender, e);
        }

        private void EventsFrm_Load(object sender, EventArgs e)
        {
            _solutionEvents.Add(new _SBEWrap(Config.Data.preBuild));
            comboBoxEvents.Items.Add(":: Pre-Build :: Before assembly");

            _solutionEvents.Add(new _SBEWrap(Config.Data.postBuild, _SBEWrap.SBEEvetnType.SBEEventPost));
            comboBoxEvents.Items.Add(":: Post-Build :: After assembly");

            _solutionEvents.Add(new _SBEWrap(Config.Data.cancelBuild));
            comboBoxEvents.Items.Add(":: Cancel-Build :: by user or compilation errors");

            _solutionEvents.Add(new _SBEWrap(Config.Data.warningsBuild, _SBEWrap.SBEEvetnType.SBEEventEW));
            comboBoxEvents.Items.Add(":: Warnings-Build :: Warnings during assembly");

            _solutionEvents.Add(new _SBEWrap(Config.Data.errorsBuild, _SBEWrap.SBEEvetnType.SBEEventEW));
            comboBoxEvents.Items.Add(":: Errors-Build :: Errors during assembly");

            _solutionEvents.Add(new _SBEWrap(Config.Data.outputCustomBuild, _SBEWrap.SBEEvetnType.SBEEventOWP));
            comboBoxEvents.Items.Add(":: Output-Build customization :: Full control");

            _solutionEvents.Add(new _SBEWrap(Config.Data.transmitter, _SBEWrap.SBEEvetnType.SBETransmitter));
            comboBoxEvents.Items.Add(":: Transmitter :: Transfer output data to outer handler");

            comboBoxEvents.SelectedIndex = 0;
            _operationsInit();
            _renderData();

#if DEBUG
            this.Text += " [Debug version]";
#endif
        }

        private void btnExample_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show(String.Format("{0}\n{1}\n\n{2}\n{3}",
                                                    "Help is contained on VS Gallery Page - scripts, solutions, etc.,",
                                                    "Click 'Yes' to go to the page",
                                                    "Other detail, see on: bitbucket.org/3F",
                                                    "entry.reg@gmail.com"
                                                    ),
                                                this.Text, 
                                                MessageBoxButtons.YesNo, 
                                                MessageBoxIcon.Information);

            if(ret == DialogResult.Yes) {
                System.Diagnostics.Process.Start("http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
            }
        }

        private void btnBugReport_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Click 'Yes' if you found error or have a proposal", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(ret == DialogResult.Yes) {
                System.Diagnostics.Process.Start("https://bitbucket.org/3F/vssolutionbuildevent/issues/new");
            }
        }

        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            _operationsSelect();
            _renderData();
        }

        private void textBoxCommand_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBoxCaption_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void checkBoxStatus_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxStatus.Checked) {
                //checkBoxStatus.Text = "Enabled";
                checkBoxStatus.BackColor = textBoxCommand.BackColor = System.Drawing.Color.FromArgb(242, 250, 241);
                
            }
            else{
                //checkBoxStatus.Text = "Disabled";
                checkBoxStatus.BackColor = textBoxCommand.BackColor = System.Drawing.Color.FromArgb(248, 243, 243);
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
        
        private void _notice(Type component)
        {
            EventHandler call = (sender, e) => { _notice(true); };
            foreach(Control ctrl in getControls(this, c => c.GetType() == component))
            {
                if(component == typeof(CheckBox)){
                    ((CheckBox)ctrl).CheckedChanged += call;
                }
                else if(component == typeof(RadioButton)){
                    ((RadioButton)ctrl).CheckedChanged += call;
                }
                else if(component == typeof(TextBox)){
                    ((TextBox)ctrl).TextChanged += call;
                }
                else if(component == typeof(ListBox) && ctrl.Name != "listBoxEW") {
                    ((ListBox)ctrl).SelectedIndexChanged += call;
                }
                else if(component == typeof(ComboBox)) {
                    ((ComboBox)ctrl).TextChanged += call;
                }
            }
        }

        protected IEnumerable<Control> getControls(Control ctrl, Func<Control, bool> predicate)
        {
            IEnumerable<Control> tctrl = ctrl.Controls.Cast<Control>();
            return tctrl.SelectMany(c => getControls(c, predicate)).Concat(tctrl).Where(predicate);
        }

        private void _renderData()
        {
            foreach(RadioButton rb in getControls(groupBoxPMode, c => c.GetType() == typeof(RadioButton))) {
                rb.Checked = false;
            }
            _renderData(_SBE.evt);

            checkBoxIgnoreIfFailed.Enabled  = false;
            groupBoxOutputControl.Enabled   = false;
            groupBoxEW.Enabled              = false;

            switch(_SBE.subtype)
            {
                case _SBEWrap.SBEEvetnType.SBEEventEW: {
                    _renderData((SBEEventEW)_SBE.evt);
                    groupBoxEW.Enabled = true;
                    break;
                }
                case _SBEWrap.SBEEvetnType.SBEEventOWP: {
                    _renderData((SBEEventOWP)_SBE.evt);                    
                    groupBoxOutputControl.Enabled = true;
                    break;
                }
                case _SBEWrap.SBEEvetnType.SBEEventPost: {
                    checkBoxIgnoreIfFailed.Enabled = true;
                    break;
                }
            }
            _notice(false);
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
            checkBoxIgnoreIfFailed.Checked  = evt.buildFailedIgnore;

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

        //TODO: additional component
        private void _renderDataStubCommand(bool isOperation)
        {
            if(!isOperation) {
                textBoxCommand.Text = _SBE.evt.command;
                return;
            }
            textBoxCommand.Text = String.Join("\n", _SBE.evt.dteExec.cmd);
        }

        // TODO: ~ DefCommandsDTE
        private void _operationsInit()
        {            
            listBoxOperation.Items.Clear();
            foreach(TOperation operation in _listOperations) {
                listBoxOperation.Items.Add(operation.caption);

            }
            listBoxOperation.Items.Add(">> User custom <<");
            _operationsSelect();
        }

        private void _operationsSelect()
        {
            int idx = 0;
            foreach(string caption in listBoxOperation.Items) {
                if(_SBE.evt.dteExec.caption == caption) {
                    listBoxOperation.SelectedIndex = idx;
                    return;
                }
                ++idx;
            }
            listBoxOperation.SelectedIndex = idx - 1;
        }

        private void _operationsAction()
        {
            if(_isOperationCustomUse()) {
                labelToCommandBox.Text = "DTE execute (separated by enter key):";
                textBoxCommand.Enabled = true;
                _renderDataStubCommand(true);
            }
            else {
                labelToCommandBox.Text = "~";
                textBoxCommand.Enabled = false;
                textBoxCommand.Text    = "";
            }
        }

        private bool _isOperationCustomUse()
        {
            if(listBoxOperation.SelectedIndex == listBoxOperation.Items.Count - 1) {
                return true;
            }
            return false;
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
            textBoxCommand.Enabled      = true;
            _renderDataStubCommand(false);
            panelControlByOperation.Enabled = true;
        }

        private void radioModeFiles_CheckedChanged(object sender, EventArgs e)
        {
            labelToCommandBox.Text      = "Files to execute (separated by enter key):";            
            groupBoxInterpreter.Enabled = false;
            listBoxOperation.Enabled    = false;
            textBoxCommand.Enabled      = true;
            _renderDataStubCommand(false);
            panelControlByOperation.Enabled = true;
        }

        private void radioModeOperation_CheckedChanged(object sender, EventArgs e)
        {
            _operationsAction();
            groupBoxInterpreter.Enabled     = false;
            listBoxOperation.Enabled        = true;
            panelControlByOperation.Enabled = false;
        }

        private void listBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            _operationsAction();
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

        private void btnDteCmd_Click(object sender, EventArgs e)
        {
            IEnumerable<EnvDTE.Command> commands = _dte.Commands.Cast<EnvDTE.Command>();
            if(_frmDTECommands != null && !_frmDTECommands.IsDisposed) {
                if(_frmDTECommands.WindowState != FormWindowState.Minimized) {
                    _frmDTECommands.Dispose();
                    _frmDTECommands = null;
                    return;
                }
                _frmDTECommands.Focus();
                return;
            }

            _frmDTECommands = new DTECommandsFrm(commands);
            _frmDTECommands.Show();
        }
    }
}
