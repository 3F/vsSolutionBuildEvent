/*
 * Copyright (c) 2013-2014  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.UI.WForms.Components;
using net.r_eg.vsSBE.UI.WForms.Logic;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class EventsFrm: Form, ITransferDataProperty, ITransferDataCommand
    {
        public const int WM_SYSCOMMAND  = 0x0112;
        public const int SC_RESTORE     = 0xF120;
        public const int SC_RESTORE2    = 0xF122; //double-click on caption

        /// <summary>
        /// Logic for this UI
        /// Operations with events etc.,
        /// </summary>
        protected Logic.Events logic;

        /// <summary>
        /// UI-helper for MSBuild Properties
        /// </summary>
        protected PropertiesFrm frmProperties;
        /// <summary>
        /// Testing tool - Evaluating Property
        /// </summary>
        protected PropertyCheckFrm frmPropertyCheck;
        /// <summary>
        /// UI-helper for DTE Commands
        /// </summary>
        protected DTECommandsFrm frmDTECommands;
        /// <summary>
        /// Testing tool - DTE Commands
        /// </summary>
        protected DTECheckFrm frmDTECheck;
        /// <summary>
        /// Testing tool - SBE-Scripts
        /// </summary>
        protected ScriptCheckFrm frmSBEScript;

        /// <summary>
        /// Default sizes for controls
        /// </summary>
        protected struct MetricDefault
        {
            public Size form;
            public Size formCollapsed;
            public int splitter;
        }
        protected MetricDefault metric;

        public EventsFrm(Environment env)
        {
            logic = new Logic.Events(env);

            InitializeComponent();
            defaultSizes();

#if DEBUG
            this.Text                       += " [Debug version]";
            toolStripMenuDebugMode.Checked  = true;
            toolStripMenuDebugMode.Enabled  = false;
            toolStripMenuVersion.Text       = string.Format("based on {0}", Version.branchSha1);
#else
            toolStripMenuDebugMode.Checked  = Settings.debugMode;
            toolStripMenuVersion.Text       = string.Format("v{0} [ {1} ]", Version.numberString, Version.branchSha1);
#endif

            Util.fixDGVRowHeight(dataGridViewOutput);
            Util.fixDGVRowHeight(dataGridViewOrder);
            Util.fixDGVRowHeight(dgvActions);
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
            textBoxCommand.SelectedText = logic.formatMSBuildProperty(name, project);
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

        protected void saveData()
        {
            try {
                saveData(logic.SBEItem);

                switch(logic.SBE.type) {
                    case SolutionEventType.Errors:
                    case SolutionEventType.Warnings: {
                        saveData((SBEEventEW)logic.SBEItem);
                        break;
                    }
                    case SolutionEventType.OWP: {
                        saveData((SBEEventOWP)logic.SBEItem);
                        break;
                    }
                }
                logic.saveData();
            }
            catch(Exception ex) {
                MessageBox.Show("Failed applying settings:\n" + ex.Message, "Configuration of event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void saveData(ISolutionEvent evt)
        {
            evt.Enabled                 = checkBoxStatus.Checked;
            evt.Caption                 = textBoxCaption.Text;
            evt.SupportMSBuild          = checkBoxMSBuildSupport.Checked;
            evt.SupportSBEScripts       = checkBoxSBEScriptSupport.Checked;
            evt.IgnoreIfBuildFailed     = checkBoxIgnoreIfFailed.Checked;
            evt.Process.Waiting         = checkBoxWaitForExit.Checked;
            evt.Process.Hidden          = checkBoxProcessHide.Checked;
            evt.Confirmation            = chkConfirmation.Checked;
            evt.ToConfiguration         = checkedListBoxSpecCfg.CheckedItems.OfType<string>().ToArray();
            evt.ExecutionOrder          = getExecutionOrder();
            evt.BuildType               = (chkBuildContext.Checked)? logic.getBuildTypeBy(comboBoxBuildContext.SelectedIndex) : BuildType.Common;

            if(radioModeInterpreter.Checked)
            {
                evt.Mode = new ModeInterpreter() {
                    Command = textBoxCommand.Text,
                    Handler = comboBoxInterpreter.Text,
                    Newline = comboBoxNewline.Text,
                    Wrapper = comboBoxWrapper.Text.Trim()
                };
            }
            else if(radioModeFiles.Checked)
            {
                evt.Mode = new ModeFile() {
                    Command = textBoxCommand.Text
                };
            }
            else if(radioModeScript.Checked)
            {
                evt.Mode = new ModeScript() {
                    Command = textBoxCommand.Text
                };
            }
            else if(radioModeOperation.Checked)
            {
                string[] command;
                if(isOperationCustom(listBoxOperation)) {
                    command = logic.splitOperations(textBoxCommand.Text);
                }
                else {
                    //TODO:
                    command = logic.DefOperations[listBoxOperation.SelectedIndex].Command;
                }

                evt.Mode = new ModeOperation() {
                    Command = command,
                    Caption = listBoxOperation.Text,
                    AbortOnFirstError = checkBoxOperationsAbort.Checked
                };
            }
        }

        protected void saveData(SBEEventEW evt)
        {
            evt.Codes       = listBoxEW.Items.Cast<string>().ToList();
            evt.IsWhitelist = radioCodesWhitelist.Checked;
        }

        protected void saveData(SBEEventOWP evt)
        {
            List<MatchWords> list = new List<MatchWords>();
            foreach(DataGridViewRow row in dataGridViewOutput.Rows)
            {
                if(row.Cells["owpTerm"].Value == null || row.Cells["owpType"].Value == null) {
                    continue;
                }
                MatchWords m  = new MatchWords();
                m.Condition   = (row.Cells["owpTerm"].Value == null)? "" : row.Cells["owpTerm"].Value.ToString();
                m.Type        = (ComparisonType)Enum.Parse(typeof(ComparisonType), row.Cells["owpType"].Value.ToString());
                list.Add(m);
            }
            evt.Match = list.ToArray();
        }

        protected void renderData()
        {
            //foreach(RadioButton rb in Util.getControls(groupBoxPMode, c => c.GetType() == typeof(RadioButton))) {
            //    rb.Checked = false; // see renderData(SBEEvent evt)
            //}
            renderData(logic.SBEItem);

            checkBoxIgnoreIfFailed.Enabled  = false;
            groupBoxOutputControl.Enabled   = false;
            groupBoxEW.Enabled              = false;
            checkBoxWaitForExit.Enabled     = false;

            switch(logic.SBE.type)
            {
                case SolutionEventType.Errors:
                case SolutionEventType.Warnings:
                {
                    renderData((SBEEventEW)logic.SBEItem);
                    groupBoxEW.Enabled = true;
                    break;
                }
                case SolutionEventType.OWP:
                {
                    renderData((SBEEventOWP)logic.SBEItem);
                    groupBoxOutputControl.Enabled = true;
                    break;
                }
                case SolutionEventType.Pre:
                case SolutionEventType.Cancel:
                case SolutionEventType.Post:
                {
                    checkBoxIgnoreIfFailed.Enabled  = true;
                    checkBoxWaitForExit.Enabled     = true;
                    break;
                }
            }
        }

        protected void renderData(ISolutionEvent evt)
        {
            checkBoxStatus.Checked              = evt.Enabled;
            textBoxCaption.Text                 = evt.Caption;
            checkBoxMSBuildSupport.Checked      = evt.SupportMSBuild;
            checkBoxSBEScriptSupport.Checked    = evt.SupportSBEScripts;
            checkBoxIgnoreIfFailed.Checked      = evt.IgnoreIfBuildFailed;
            checkBoxWaitForExit.Checked         = evt.Process.Waiting;
            checkBoxProcessHide.Checked         = evt.Process.Hidden;
            chkConfirmation.Checked             = evt.Confirmation;
            buildTypeSelect(evt.BuildType);

            if(evt.Mode == null) {
                Log.nlog.Warn("The Mode is corrupt, reinitialized with default type");
                evt.Mode = logic.DefaultMode;
            }

            switch(evt.Mode.Type) {
                case ModeType.Interpreter:
                {
                    radioModeInterpreter.Checked    = true;
                    textBoxCommand.Text             = ((IModeInterpreter)evt.Mode).Command;
                    comboBoxInterpreter.Text        = ((IModeInterpreter)evt.Mode).Handler;
                    comboBoxNewline.Text            = ((IModeInterpreter)evt.Mode).Newline;
                    comboBoxWrapper.Text            = ((IModeInterpreter)evt.Mode).Wrapper;
                    return;
                }
                case ModeType.File:
                {
                    radioModeFiles.Checked = true;
                    textBoxCommand.Text = ((IModeFile)evt.Mode).Command;
                    return;
                }
                case ModeType.Script: {
                    radioModeScript.Checked = true;
                    textBoxCommand.Text = ((IModeScript)evt.Mode).Command;
                    return;
                }
                case ModeType.Operation:
                {
                    radioModeOperation.Checked  = true;
                    IModeOperation mode         = (IModeOperation)evt.Mode;

                    if(logic.isDefOperation(mode.Caption)) {
                        textBoxCommand.Text = "> " + mode.Caption;
                    }
                    else {
                        textBoxCommand.Text = (mode.Command == null)? "" : logic.joinOperations(mode.Command);
                    }
                    checkBoxOperationsAbort.Checked = mode.AbortOnFirstError;
                    return;
                }
            }
        }

        protected void renderData(SBEEventEW evt)
        {
            listBoxEW.Items.Clear();
            listBoxEW.Items.AddRange(evt.Codes.ToArray());

            if(evt.IsWhitelist) {
                radioCodesWhitelist.Checked = true;
            }
            else {
                radioCodesBlacklist.Checked = true;
            }
        }

        protected void renderData(SBEEventOWP evt)
        {
            dataGridViewOutput.Rows.Clear();
            if(evt.Match == null) {
                return;
            }
            foreach(MatchWords m in evt.Match) {
                dataGridViewOutput.Rows.Add(m.Condition, m.Type.ToString());
            }
        }

        protected void fillActionsList()
        {
            dgvActions.Rows.Clear();
            logic.protectMinEventItems();
            foreach(SBEEvent item in logic.SBE.evt) {
                dgvActions.Rows.Add(item.Enabled, item.Name, item.Caption);
            }
        }

        protected void refreshSettings()
        {
            clearControls();
            operationsList(listBoxOperation);
            renderData();
            onlyFor();
            executionOrder();
            notice(false);
        }

        protected void refreshActions(bool rememberIndex = true)
        {
            int selectedRowIndex = (rememberIndex)? currentActionIndex() : 0;
            fillActionsList();
            selectAction(selectedRowIndex);
        }

        protected void selectAction(int index, bool refreshSettings = false)
        {
            index = Math.Max(0, Math.Min(index, dgvActions.Rows.Count - 1));
            dgvActions.ClearSelection();
            dgvActions.Rows[index].Selected = true;
            logic.setEventIndexes(comboBoxEvents.SelectedIndex, index);

            if(refreshSettings) {
                this.refreshSettings();
            }
        }

        protected int currentActionIndex()
        {
            //return (dgvActions.CurrentRow == null)? 0 : dgvActions.CurrentRow.Index;
            return (dgvActions.SelectedRows.Count < 1) ? 0 : dgvActions.SelectedRows[0].Index;
        }

        protected void refreshSettingsWithIndex(int index)
        {
            logic.setEventIndexes(comboBoxEvents.SelectedIndex, index);
            refreshSettings();
        }

        protected void addAction(int copyFrom = -1)
        {
            try {
                ISolutionEvent evt = logic.addEventItem(copyFrom);
                dgvActions.Rows.Add(evt.Enabled, evt.Name, evt.Caption);
                selectAction(dgvActions.Rows.Count - 1, true);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed to add event-item: '{0}'", ex.Message);
            }
        }

        protected void clearControls()
        {
            textBoxEW.Text = String.Empty;
            listBoxEW.Items.Clear();
            dataGridViewOutput.Rows.Clear();
        }

        protected void notice(bool isOn)
        {
            if(isOn) {
                btnApply.FlatAppearance.BorderColor = Color.FromArgb(255, 0, 0);
                return;
            }
            btnApply.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0);
        }

        protected void uiViewMode(ModeType type)
        {
            groupBoxInterpreter.Enabled         = false;
            listBoxOperation.Enabled            = false;
            textBoxCommand.Enabled              = true;
            textBoxCommand.Text                 = String.Empty;
            panelControlByOperation.Enabled     = true;
            checkBoxOperationsAbort.Enabled     = false;
            checkBoxSBEScriptSupport.Enabled    = true;

            if(type == ModeType.Interpreter)
            {
                labelToCommandBox.Text = "Command script for stream processor:";
                groupBoxInterpreter.Enabled = true;
                return;
            }
            if(type == ModeType.File)
            {
                labelToCommandBox.Text = "Files to execute (separated by enter key):";
                return;
            }
            if(type == ModeType.Script) {
                labelToCommandBox.Text = "Script:";
                checkBoxSBEScriptSupport.Enabled = false;
                checkBoxSBEScriptSupport.Checked = true;
                return;
            }
            if(type == ModeType.Operation)
            {
                operationsAction(listBoxOperation);
                listBoxOperation.Enabled        = true;
                panelControlByOperation.Enabled = false;
                checkBoxOperationsAbort.Enabled = true;
                return;
            }
        }

        protected void operationsList(ListBox list)
        {
            if(list.Items.Count < 1) {
                foreach(ModeOperation operation in logic.DefOperations) {
                    list.Items.Add(operation.Caption);

                }
                list.Items.Add(">> User custom <<"); //TODO
            }

            list.SelectedIndex = list.Items.Count - 1;
            if(logic.SBEItem.Mode.Type != ModeType.Operation) {
                return;
            }

            int idx = 0;
            foreach(string caption in list.Items) {
                if(((IModeOperation)logic.SBEItem.Mode).Caption == caption) {
                    list.SelectedIndex = idx;
                    return;
                }
                ++idx;
            }
        }

        protected void operationsAction(ListBox list)
        {
            if(isOperationCustom(list)) {
                labelToCommandBox.Text  = "DTE execute (separated by enter key):";
                textBoxCommand.Enabled  = true;
                textBoxCommand.Text     = String.Empty;
            }
            else {
                labelToCommandBox.Text  = "~";
                textBoxCommand.Enabled  = false;
                textBoxCommand.Text     = "> " + logic.DefOperations[list.SelectedIndex].Caption;
            }
        }

        protected bool isOperationCustom(ListBox list)
        {
            if(list.SelectedIndex == list.Items.Count - 1) {
                return true;
            }
            return false;
        }

        protected void onlyFor()
        {
            if(checkedListBoxSpecCfg.Items.Count < 1) {
                foreach(EnvDTE80.SolutionConfiguration2 cfg in logic.Env.SolutionConfigurations) {
                    checkedListBoxSpecCfg.Items.Add(logic.Env.SolutionConfigurationFormat(cfg), false);
                }
            }

            string[] toConf = logic.SBEItem.ToConfiguration;
            for(int i = 0; i < checkedListBoxSpecCfg.Items.Count; ++i)
            {
                string name = checkedListBoxSpecCfg.Items[i].ToString();
                bool state  = (toConf == null)? false : toConf.Where(s => s == name).Count() > 0;
                checkedListBoxSpecCfg.SetItemChecked(i, state);
            }
        }

        protected void executionOrder()
        {
            if(dataGridViewOrder.Rows.Count < 1) {
                foreach(string name in logic.Env.DTEProjectsList) {
                    dataGridViewOrder.Rows.Add(false, name, dgvOrderType.Items[0]);
                }
            }

            IExecutionOrder[] list = logic.SBEItem.ExecutionOrder;
            foreach(DataGridViewRow row in dataGridViewOrder.Rows)
            {
                if(list == null || list.Length < 1) {
                    row.Cells["dgvOrderEnabled"].Value  = false;
                    row.Cells["dgvOrderType"].Value     = ExecutionOrderType.Before.ToString();
                    continue;
                }

                IExecutionOrder v = list.Where(s => s.Project == row.Cells["dgvOrderProject"].Value.ToString()).FirstOrDefault();
                if(v == null) {
                    continue;
                }
                row.Cells["dgvOrderEnabled"].Value  = !String.IsNullOrEmpty(v.Project);
                row.Cells["dgvOrderType"].Value     = v.Order.ToString();
            }
        }

        protected ExecutionOrder[] getExecutionOrder()
        {
            List<ExecutionOrder> ret = new List<ExecutionOrder>(dataGridViewOrder.Rows.Count);
            foreach(DataGridViewRow row in dataGridViewOrder.Rows)
            {
                if(!Convert.ToBoolean(row.Cells["dgvOrderEnabled"].Value)) {
                    continue;
                }

                ret.Add(new ExecutionOrder() {
                    Project = row.Cells["dgvOrderProject"].Value.ToString(),
                    Order   = (ExecutionOrderType)Enum.Parse(typeof(ExecutionOrderType), row.Cells["dgvOrderType"].Value.ToString())
                });
            }
            return ret.ToArray();
        }

        protected void buildTypeSelect(BuildType type)
        {
            chkBuildContext.Checked         = type != BuildType.Common;
            comboBoxBuildContext.Enabled    = chkBuildContext.Checked;

            int index = logic.getBuildTypeIndex(type);
            if(index >= 0) {
                comboBoxBuildContext.SelectedIndex = index;
            }
        }

        protected void envVariablesUIHelper()
        {
            if(Util.focusForm(frmProperties)) {
                return;
            }
            frmProperties = new PropertiesFrm(this);
            frmProperties.Show();
        }

        protected void expandActionsList(bool flag)
        {
            if(!flag) {
                splitContainer.SplitterDistance = splitContainer.Panel1MinSize;
                this.Size = metric.formCollapsed;
                //this.Width = metric.formCollapsed.Width; // -> pictureBoxToggle
                return;
            }
            this.Width = metric.form.Width;
            splitContainer.SplitterDistance = metric.splitter;
        }

        protected void defaultSizes()
        {
            metric.form = Size;
            metric.splitter = splitContainer.SplitterDistance;
            metric.formCollapsed = new Size(Width - (metric.splitter), Height);
        }

        protected override void WndProc(ref Message mes)
        {
            if(mes.Msg == WM_SYSCOMMAND)
            {
                if(mes.WParam == (IntPtr)SC_RESTORE || mes.WParam == (IntPtr)SC_RESTORE2) {
                    expandActionsList(false);
                }
            }
            base.WndProc(ref mes);
        }

        private void EventsFrm_Load(object sender, EventArgs e)
        {
            EventHandler call = (csender, ce) => { notice(true); };

            Util.noticeAboutChanges(typeof(CheckBox), this, call);
            Util.noticeAboutChanges(typeof(RadioButton), this, call);
            Util.noticeAboutChanges(typeof(TextBox), this, call);
            Util.noticeAboutChanges(typeof(RichTextBox), this, call);
            Util.noticeAboutChanges(typeof(ListBox), this, call, new string[] { "listBoxEW" });
            Util.noticeAboutChanges(typeof(ComboBox), this, call);
            Util.noticeAboutChanges(typeof(CheckedListBox), this, call);
            Util.noticeAboutChanges(typeof(DataGridView), this, call);

            try {
                expandActionsList(false);
                logic.fillBuildTypes(comboBoxBuildContext);
                logic.fillEvents(comboBoxEvents);
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed to load form: {0}", ex.Message);
            }
        }

        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                logic.setEventIndexes(comboBoxEvents.SelectedIndex, 0);
                refreshActions(false);
                refreshSettings();
            }
            catch(Exception ex) {
                Log.nlog.Error("Failed to select event type: {0}", ex.Message);
            }
        }

        private void listBoxOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            operationsAction(listBoxOperation);
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

        private void checkBoxStatus_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBoxStatus.Checked) {
                checkBoxStatus.BackColor = textBoxCommand.BackColor = System.Drawing.Color.FromArgb(242, 250, 241);
                return;
            }
            checkBoxStatus.BackColor = textBoxCommand.BackColor = System.Drawing.Color.FromArgb(248, 243, 243);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            saveData();
            refreshActions();
            notice(false);
        }

        private void toolStripMenuReset_Click(object sender, EventArgs e)
        {
            logic.restoreData();
            logic.fillEvents(comboBoxEvents);
            renderData();
            comboBoxEvents_SelectedIndexChanged(sender, e);
        }

        private void radioModeFiles_CheckedChanged(object sender, EventArgs e)
        {
            uiViewMode(ModeType.File);
        }

        private void radioModeOperation_CheckedChanged(object sender, EventArgs e)
        {
            uiViewMode(ModeType.Operation);
        }

        private void radioModeInterpreter_CheckedChanged(object sender, EventArgs e)
        {
            uiViewMode(ModeType.Interpreter);
        }

        private void radioModeScript_CheckedChanged(object sender, EventArgs e)
        {
            uiViewMode(ModeType.Script);
        }

        private void chkBuildContext_CheckedChanged(object sender, EventArgs e)
        {
            comboBoxBuildContext.Enabled = chkBuildContext.Checked;
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

        private void toolStripMenuGalleryPage_Click(object sender, EventArgs e)
        {
            Util.openUrl("http://visualstudiogallery.msdn.microsoft.com/0d1dbfd7-ed8a-40af-ae39-281bfeca2334/");
        }

        private void toolStripMenuDocDte_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/DTE-Commands");
        }

        private void toolStripMenuDocMSBuild_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/MSBuild");
        }

        private void toolStripMenuDocSBE_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Scripts_&_Commands/SBE-Scripts");
        }

        private void toolStripMenuDocDev_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki/Developer%20Zone");
        }

        private void toolStripMenuChangelog_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/raw/master/changelog.txt");
        }

        private void toolStripMenuWiki_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/wiki");
        }

        private void toolStripMenuIssue_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/issues");
        }

        private void toolStripMenuSources_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/commits/all");
        }

        private void toolStripMenuForkGithub_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://github.com/3F/vsSolutionBuildEvent");
        }

        private void toolStripMenuForkBitbucket_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/overview");
        }

        private void toolStripMenuLicense_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/raw/master/LICENSE");
        }

        private void toolStripMenuAbout_Click(object sender, EventArgs e)
        {
            string inc       = "This product includes:{0}{1}\n{2}";
            string nlog      = "\n * NLog (nlog-project.org)";
            string json      = "\n * Json.NET (json.codeplex.com)";
            string resources = "\n All about graphical resources see /Resources/License";

            MessageBox.Show(
                String.Format(
                        "Copyright (c) 2013-{0}\n Developed by reg [Denis Kuzmin] < entry.reg@gmail.com >\n\n{1}",
                        DateTime.Now.Year, 
                        String.Format(inc, nlog, json, resources)
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
                Util.openUrl("https://bitbucket.org/3F/vssolutionbuildevent/issues/new");
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

        private void toolStripMenuDebugMode_Click(object sender, EventArgs e)
        {
#if !DEBUG
            Settings.debugMode = (toolStripMenuDebugMode.Checked = !toolStripMenuDebugMode.Checked);
#endif
        }

        private void btnDteCmd_Click(object sender, EventArgs e)
        {
            if(Util.focusForm(frmDTECommands)) {
                return;
            }
            IEnumerable<EnvDTE.Command> commands = logic.Env.DTE2.Commands.Cast<EnvDTE.Command>();
            frmDTECommands = new DTECommandsFrm(commands, this);
            frmDTECommands.Show();
        }

        private void toolStripMenuDTECmdExec_Click(object sender, EventArgs e)
        {
            if(Util.focusForm(frmDTECheck)) {
                return;
            }
            frmDTECheck = new DTECheckFrm(logic.Env);
            frmDTECheck.Show();
        }

        private void checkedListBoxSpecCfg_Click(object sender, EventArgs e)
        {
            int max = dataGridViewOrder.Width + dataGridViewOrder.Location.X;
            Util.effectSmoothChangeWidth(checkedListBoxSpecCfg, max, 0.032f, 4);
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
            if(Util.focusForm(frmPropertyCheck)) {
                return;
            }
            frmPropertyCheck = new PropertyCheckFrm(logic.Env);
            frmPropertyCheck.Show();
        }

        private void menuSBEScript_Click(object sender, EventArgs e)
        {
            if(Util.focusForm(frmSBEScript)) {
                return;
            }
            frmSBEScript = new ScriptCheckFrm(logic.Env);
            frmSBEScript.Show();
        }

        private void EventsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Util.closeTool(frmProperties);
            Util.closeTool(frmPropertyCheck);
            Util.closeTool(frmDTECommands);
            Util.closeTool(frmDTECheck);
            Util.closeTool(frmSBEScript);
            logic.restoreData();
        }

        private void dgvActions_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            e.Cancel = (dgvActions.Rows.Count < 2);
        }

        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {
            pictureBoxToggle.Visible = !(e.SplitX < metric.splitter);
        }

        private void menuActionsAdd_Click(object sender, EventArgs e)
        {
            addAction();
        }

        private void menuActionsClone_Click(object sender, EventArgs e)
        {
            addAction(currentActionIndex());
        }

        private void menuActionsEdit_Click(object sender, EventArgs e)
        {
            dgvActions.CurrentCell = dgvActions.Rows[currentActionIndex()].Cells["dgvActionName"];
            dgvActions.BeginEdit(true);
        }

        private void menuActionsTogglePanel_Click(object sender, EventArgs e)
        {
            expandActionsList(splitContainer.SplitterDistance < metric.splitter);
        }

        private void menuActionsReset_Click(object sender, EventArgs e)
        {
            toolStripMenuReset_Click(sender, e);
        }

        private void menuActionsRemove_Click(object sender, EventArgs e)
        {
            int index = currentActionIndex();
            if(dgvActions.Rows.Count < 2) {
                addAction();
            }
            dgvActions.Rows.RemoveAt(index);
            logic.removeEventItem(index);
            refreshSettingsWithIndex(currentActionIndex());
        }

        private void pictureBoxToggle_Click(object sender, EventArgs e)
        {
            expandActionsList(false);
        }

        private void dgvActions_Click(object sender, EventArgs e)
        {
            if(splitContainer.SplitterDistance < metric.splitter) {
                expandActionsList(true);
            }
        }

        private void EventsFrm_ClientSizeChanged(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Maximized) {
                expandActionsList(true);
            }
        }

        private void dgvActions_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            e.Value = logic.validateName(e.Value.ToString());
            e.ParsingApplied = true;
        }

        private void dgvActions_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == System.Windows.Forms.MouseButtons.Left) {
                // MouseDown because the CellClick event may not be called for some rows
                // the RowEnter called is too late..
                int idx = dgvActions.HitTest(e.X, e.Y).RowIndex;
                if(idx != -1) {
                    refreshSettingsWithIndex(idx);
                }
            }
        }

        private void dgvActions_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) {
                refreshSettingsWithIndex(currentActionIndex());
            }
        }

        private void dgvActions_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter) {
                e.SuppressKeyPress = true;
            }
        }

        private void dgvActions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == 0) {
                dgvActions.EndEdit();
            }
        }

        private void dgvActions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) {
                return;
            }
            bool enabled = Boolean.Parse(dgvActions.Rows[e.RowIndex].Cells["dgvActionEnabled"].Value.ToString());
            object oname = dgvActions.Rows[e.RowIndex].Cells["dgvActionName"].Value;

            logic.updateInfo(e.RowIndex, (oname == null)? "" : oname.ToString(), enabled);
            refreshSettings();
        }

        private void dgvActions_DragDropSortedRow(MovingRow index)
        {
            logic.moveEventItem(index.from, index.to);
        }
    }
}