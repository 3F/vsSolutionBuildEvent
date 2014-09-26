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
using net.r_eg.vsSBE.Events;

namespace net.r_eg.vsSBE.UI
{
    public partial class EventsFrm: Form, ITransferDataProperty, ITransferDataCommand
    {
        private class _SBEWrap
        {
            public enum UIType
            {
                General,
                EW,
                OWP,
                Transmitter,
                Post,
                Pre
            }

            public SBEEvent evt;
            public UIType subtype = UIType.General;

            public _SBEWrap(SBEEvent evt, UIType subtype)
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
        /// UI-helper for MSBuild Properties
        /// </summary>
        private PropertiesFrm _frmProperties;

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
        /// <summary>
        /// Testing tool - Evaluating Property
        /// </summary>
        private PropertyCheckFrm _frmPropertyCheck;

        private Environment _env;

        public EventsFrm(Environment env)
        {
            _env = env;
            InitializeComponent();

#if DEBUG
            this.Text                       += " [Debug version]";
            toolStripMenuDebugMode.Checked  = true;
            toolStripMenuDebugMode.Enabled  = false;
            toolStripMenuVersion.Text       = string.Format("based on {0}", Version.branchSha1);
#else
            toolStripMenuDebugMode.Checked  = Settings.debugMode;
            toolStripMenuVersion.Text       = string.Format("v{0} [ {1} ]", Version.numberString, Version.branchSha1);
#endif

            _fixHeight(dataGridViewOutput);
            _fixHeight(dataGridViewOrder);

            _notice(typeof(CheckBox));
            _notice(typeof(RadioButton));
            _notice(typeof(TextBox));
            _notice(typeof(RichTextBox));
            _notice(typeof(ListBox));
            _notice(typeof(ComboBox));
        }

        /// <summary>
        /// Implements transport for MSBuild property
        /// TODO: highlighting
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        public void property(string name, string project = null)
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

        /// <summary>
        /// Implements transport for DTE command
        /// </summary>
        /// <param name="name"></param>
        public void command(string name)
        {
            textBoxCommand.Select(textBoxCommand.SelectionStart, 0);
            textBoxCommand.SelectedText = name;
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
                    case _SBEWrap.UIType.EW: {
                        _saveData((SBEEventEW)_SBE.evt);
                        break;
                    }
                    case _SBEWrap.UIType.OWP: {
                        _saveData((SBEEventOWP)_SBE.evt);
                        break;
                    }
                }

                Config._.save();
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

            evt.dteExec.abortOnFirstError   = checkBoxOperationsAbort.Checked;
            evt.toConfiguration             = checkedListBoxSpecCfg.CheckedItems.OfType<string>().ToArray();
            evt.executionOrder              = _getExecutionOrder();
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
                if(row.Cells["owpTerm"].Value == null || row.Cells["owpType"].Value == null) {
                    continue;
                }
                TEventOWP owp = new TEventOWP();
                owp.term      = row.Cells["owpTerm"].Value.ToString();
                owp.type      = (TEventOWPTerm)Enum.Parse(typeof(TEventOWPTerm), row.Cells["owpType"].Value.ToString());
                evt.eventsOWP.Add(owp);
            }
        }

        private void toolStripMenuReset_Click(object sender, EventArgs e)
        {
            comboBoxEvents_SelectedIndexChanged(sender, e);
        }

        private void EventsFrm_Load(object sender, EventArgs e)
        {
            _solutionEvents.Add(new _SBEWrap(Config._.Data.preBuild, _SBEWrap.UIType.Pre));
            comboBoxEvents.Items.Add(":: Pre-Build :: Before assembly");

            _solutionEvents.Add(new _SBEWrap(Config._.Data.postBuild, _SBEWrap.UIType.Post));
            comboBoxEvents.Items.Add(":: Post-Build :: After assembly");

            _solutionEvents.Add(new _SBEWrap(Config._.Data.cancelBuild));
            comboBoxEvents.Items.Add(":: Cancel-Build :: by user or compilation errors");

            _solutionEvents.Add(new _SBEWrap(Config._.Data.warningsBuild, _SBEWrap.UIType.EW));
            comboBoxEvents.Items.Add(":: Warnings-Build :: Warnings during assembly");

            _solutionEvents.Add(new _SBEWrap(Config._.Data.errorsBuild, _SBEWrap.UIType.EW));
            comboBoxEvents.Items.Add(":: Errors-Build :: Errors during assembly");

            _solutionEvents.Add(new _SBEWrap(Config._.Data.outputCustomBuild, _SBEWrap.UIType.OWP));
            comboBoxEvents.Items.Add(":: Output-Build customization :: Full control");

            _solutionEvents.Add(new _SBEWrap(Config._.Data.transmitter, _SBEWrap.UIType.Transmitter));
            comboBoxEvents.Items.Add(":: Transmitter :: Transfer output data to outer handler");

            comboBoxEvents.SelectedIndex = 0;
            _operationsInit();
            _renderData();
            _onlyFor(true);
            _executionOrder(true);
        }

        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            _operationsSelect();
            _renderData();
            _onlyFor(false);
            _executionOrder(false);
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
                else if(component == typeof(RichTextBox)) {
                    ((RichTextBox)ctrl).TextChanged += call;
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
                case _SBEWrap.UIType.EW:
                {
                    _renderData((SBEEventEW)_SBE.evt);
                    groupBoxEW.Enabled = true;
                    break;
                }
                case _SBEWrap.UIType.OWP:
                {
                    _renderData((SBEEventOWP)_SBE.evt);                    
                    groupBoxOutputControl.Enabled = true;
                    break;
                }
                case _SBEWrap.UIType.Pre:
                case _SBEWrap.UIType.Post:
                {
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
            checkBoxOperationsAbort.Checked = evt.dteExec.abortOnFirstError;

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

        private void _onlyFor(bool isNew)
        {
            if(isNew) {
                checkedListBoxSpecCfg.Items.Clear();
                foreach(EnvDTE80.SolutionConfiguration2 cfg in _env.SolutionConfigurations)
                {
                    string name = _env.SolutionConfigurationFormat(cfg);
                    bool state  = _SBE.evt.toConfiguration == null ? false : _SBE.evt.toConfiguration.Where(s => s == name).Count() > 0;
                    checkedListBoxSpecCfg.Items.Add(name, state);
                }
                return;
            }

            for(int i = 0; i < checkedListBoxSpecCfg.Items.Count; ++i)
            {
                string name = checkedListBoxSpecCfg.Items[i].ToString();
                bool state  = _SBE.evt.toConfiguration == null ? false : _SBE.evt.toConfiguration.Where(s => s == name).Count() > 0;
                checkedListBoxSpecCfg.SetItemChecked(i, state);
            }
        }

        private void _executionOrder(bool isNew)
        {
            if(isNew) {
                dataGridViewOrder.Rows.Clear();
                foreach(string name in _env.DTEProjectsList)
                {
                    if(_SBE.evt.executionOrder == null) {
                        dataGridViewOrder.Rows.Add(false, name, dgvOrderComboBoxType.Items[0]);
                        continue;
                    }
                    TExecutionOrder v = _SBE.evt.executionOrder.Where(s => s.project == name).FirstOrDefault();
                    dataGridViewOrder.Rows.Add(!String.IsNullOrEmpty(v.project), name, v.order.ToString());
                }
                return;
            }

            foreach(DataGridViewRow row in dataGridViewOrder.Rows)
            {
                if(_SBE.evt.executionOrder == null) {
                    row.Cells["dgvOrderCheckBoxEnabled"].Value  = false;
                    row.Cells["dgvOrderComboBoxType"].Value     = TExecutionOrder.Type.Before.ToString();
                    continue;
                }

                TExecutionOrder v = _SBE.evt.executionOrder.Where(s => 
                                                                    s.project == row.Cells["dgvOrderTextBoxProject"].Value.ToString()
                                                                  ).FirstOrDefault();

                row.Cells["dgvOrderCheckBoxEnabled"].Value  = !String.IsNullOrEmpty(v.project);
                row.Cells["dgvOrderComboBoxType"].Value     = v.order.ToString();
            }
        }

        private TExecutionOrder[] _getExecutionOrder()
        {
            List<TExecutionOrder> ret = new List<TExecutionOrder>(dataGridViewOrder.Rows.Count);
            foreach(DataGridViewRow row in dataGridViewOrder.Rows)
            {
                if(!Convert.ToBoolean(row.Cells["dgvOrderCheckBoxEnabled"].Value)) {
                    continue;
                }
                TExecutionOrder order = new TExecutionOrder();
                order.project   = row.Cells["dgvOrderTextBoxProject"].Value.ToString();
                order.order     = (TExecutionOrder.Type)Enum.Parse(typeof(TExecutionOrder.Type), row.Cells["dgvOrderComboBoxType"].Value.ToString());
                ret.Add(order);
            }
            return ret.ToArray();
        }

        private void envVariablesUIHelper()
        {
            if(_frmProperties != null && !_frmProperties.IsDisposed)
            {
                if(_frmProperties.WindowState != FormWindowState.Minimized) {
                    _frmProperties.Dispose();
                    _frmProperties = null;
                    return;
                }
                _frmProperties.Focus();
                return;
            }
            _frmProperties = new PropertiesFrm(this);
            _frmProperties.Show();
        }

        private void radioModeScript_CheckedChanged(object sender, EventArgs e)
        {
            labelToCommandBox.Text      = "Command script:";
            groupBoxInterpreter.Enabled = true;
            listBoxOperation.Enabled    = false;
            textBoxCommand.Enabled      = true;
            _renderDataStubCommand(false);
            panelControlByOperation.Enabled = true;
            checkBoxOperationsAbort.Enabled = false;
        }

        private void radioModeFiles_CheckedChanged(object sender, EventArgs e)
        {
            labelToCommandBox.Text      = "Files to execute (separated by enter key):";            
            groupBoxInterpreter.Enabled = false;
            listBoxOperation.Enabled    = false;
            textBoxCommand.Enabled      = true;
            _renderDataStubCommand(false);
            panelControlByOperation.Enabled = true;
            checkBoxOperationsAbort.Enabled = false;
        }

        private void radioModeOperation_CheckedChanged(object sender, EventArgs e)
        {
            _operationsAction();
            groupBoxInterpreter.Enabled     = false;
            listBoxOperation.Enabled        = true;
            panelControlByOperation.Enabled = false;
            checkBoxOperationsAbort.Enabled = true;
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

        private void toolStripMenuDoc_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Help is contained on VS Gallery Page - scripts, solutions, etc.,\nClick 'Yes' to go to the page",
                                                this.Text,
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Information);

            if(ret == DialogResult.Yes) {
                _openUrl("http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
            }
        }

        private void toolStripMenuChangelog_Click(object sender, EventArgs e)
        {
            _openUrl("https://bitbucket.org/3F/vssolutionbuildevent/raw/master/changelog.txt");
        }

        private void toolStripMenuWiki_Click(object sender, EventArgs e)
        {
            _openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki");
        }

        private void toolStripMenuIssue_Click(object sender, EventArgs e)
        {
            _openUrl("https://bitbucket.org/3F/vssolutionbuildevent/issues");
        }

        private void toolStripMenuSources_Click(object sender, EventArgs e)
        {
            _openUrl("https://bitbucket.org/3F/vssolutionbuildevent/commits/all");
        }

        private void toolStripMenuForkGithub_Click(object sender, EventArgs e)
        {
            _openUrl("https://github.com/3F/vsSolutionBuildEvent");
        }

        private void toolStripMenuForkBitbucket_Click(object sender, EventArgs e)
        {
            _openUrl("https://bitbucket.org/3F/vssolutionbuildevent/overview");
        }

        private void toolStripMenuLicense_Click(object sender, EventArgs e)
        {
            _openUrl("https://bitbucket.org/3F/vssolutionbuildevent/raw/master/LICENSE");
        }

        private void toolStripMenuAbout_Click(object sender, EventArgs e)
        {
            string inc = "This product includes:\n * NLog (nlog-project.org)\n\n All about graphical resources see /Resources/License";
            MessageBox.Show(String.Format(
                                    "Copyright (c) 2013-{0} Developed by reg < entry.reg@gmail.com >\n\n{1}",
                                    DateTime.Now.Year, inc
                            ),
                            toolStripMenuAbout.Text,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
            );
        }

        private void toolStripMenuReport_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Click 'Yes' if you found error or have a proposal", this.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(ret == DialogResult.Yes) {
                System.Diagnostics.Process.Start("https://bitbucket.org/3F/vssolutionbuildevent/issues/new");
            }
        }

        private void menuItemEditorCut_Click(object sender, EventArgs e)
        {
            textBoxCommand.Cut();
        }

        private void menuItemEditorCopy_Click(object sender, EventArgs e)
        {
            textBoxCommand.Copy();
        }

        private void menuItemEditorPaste_Click(object sender, EventArgs e)
        {
            textBoxCommand.Paste();
        }

        private void _openUrl(string url)
        {
            try {
                System.Diagnostics.Process.Start(url);
            }
            catch(Exception ex) {
                Log.nlog.Warn(ex.Message);
            }
        }

        private void _fixHeight(DataGridView grid)
        {
            foreach(DataGridViewRow row in grid.Rows) {
                row.Height = grid.RowTemplate.Height;
            }
        }

        private void toolStripMenuDebugMode_Click(object sender, EventArgs e)
        {
#if !DEBUG
            Settings.debugMode = (toolStripMenuDebugMode.Checked = !toolStripMenuDebugMode.Checked);
#endif
        }

        private void btnDteCmd_Click(object sender, EventArgs e)
        {
            IEnumerable<EnvDTE.Command> commands = _env.DTE2.Commands.Cast<EnvDTE.Command>();
            if(_frmDTECommands != null && !_frmDTECommands.IsDisposed) {
                if(_frmDTECommands.WindowState != FormWindowState.Minimized) {
                    _frmDTECommands.Dispose();
                    _frmDTECommands = null;
                    return;
                }
                _frmDTECommands.Focus();
                return;
            }

            _frmDTECommands = new DTECommandsFrm(commands, this);
            _frmDTECommands.Show();
        }

        private void checkedListBoxSpecCfg_Click(object sender, EventArgs e)
        {
            int max = dataGridViewOrder.Width + dataGridViewOrder.Location.X;
            (new System.Threading.Tasks.Task(() =>
            {
                int step = (int)(max * 0.052f);
                while(checkedListBoxSpecCfg.Width < max)
                {
                    BeginInvoke((MethodInvoker)delegate {
                        checkedListBoxSpecCfg.Width += step;
                    });
                    System.Threading.Thread.Sleep(4);
                }
                BeginInvoke((MethodInvoker)delegate {
                    checkedListBoxSpecCfg.Width = max;
                });
            })).Start();
        }

        private void checkedListBoxSpecCfg_MouseLeave(object sender, EventArgs e)
        {
            checkedListBoxSpecCfg.Width = checkedListBoxSpecCfg.MinimumSize.Width;
        }

        private void toolStripMenuApply_Click(object sender, EventArgs e)
        {
            btnApply_Click(sender, e);
        }

        private void toolStripMenuMSBuildProp_Click(object sender, EventArgs e)
        {
            envVariablesUIHelper();
        }

        private void toolStripMenuDTECmd_Click(object sender, EventArgs e)
        {
            btnDteCmd_Click(sender, e);
        }

        private void toolStripMenuEvaluatingProperty_Click(object sender, EventArgs e)
        {
            if(_frmPropertyCheck != null && !_frmPropertyCheck.IsDisposed)
            {
                if(_frmPropertyCheck.WindowState != FormWindowState.Minimized) {
                    _frmPropertyCheck.Dispose();
                    _frmPropertyCheck = null;
                    return;
                }
                _frmPropertyCheck.Focus();
                return;
            }
            _frmPropertyCheck = new PropertyCheckFrm(_env);
            _frmPropertyCheck.Show();
        }
    }
}
