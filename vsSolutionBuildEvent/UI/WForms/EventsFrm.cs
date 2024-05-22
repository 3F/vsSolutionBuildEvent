/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.vsSBE.Bridge;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Events.CommandEvents;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UI.WForms.Components;
using net.r_eg.vsSBE.UI.WForms.Controls;
using NLog;

namespace net.r_eg.vsSBE.UI.WForms
{
    // I remind,
    // The Forms here (almost entire UI namespace) have functional minimalist view/GUI logic;
    // I really didn't want to complicate this by using MVVM or such (n. more components, more abstraction between, ...etc)
    //   because the whole GUI didn't bother me at all initially;
    //
    // I mean, I pay more attention to the programming interface rather than the graphical one.
    // The Forms (subscription model as a technology) are still very compact for fast drafting in my opinion (*yeah, deepest backend guy unwilling to spend much time on graphical parts or like).
    //
    // Some IoC improvements here are needed but this is not what I want to spend my time for creating just a more scalable GUI here.
    // You can, however, try to change the rails below because here is still no logic of the model as noted.
    internal partial class EventsFrm: Form, ITransfer, ICodeInspector
    {
        public const int WM_SYSCOMMAND  = 0x0112;
        public const int SC_RESTORE     = 0xF120;
        public const int SC_RESTORE2    = 0xF122; //double-click on caption

        /// <summary>
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
        /// Wizard - Automatic Version Numbering
        /// </summary>
        protected Wizards.VersionFrm frmWizVersion;

        /// <summary>
        /// UI-helper - EnvDTE Sniffer
        /// </summary>
        protected EnvDteSniffer frmSniffer;

        /// <summary>
        /// Size of buffer for existing records in sniffer pane.
        /// </summary>
        protected int snifferRcBuffer = 2048;

        /// <summary>
        /// Flag of notification if it's required
        /// </summary>
        private bool requiresNotification;

        /// <summary>
        /// Is notified about changes
        /// </summary>
        private bool isNotified;

        private MetricDefault metric;

        internal IAppSettings App => Settings._;

        /// <summary>
        /// Implements transport for MSBuild property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="project"></param>
        public void property(string name, string project = null)
        {
            textEditor.insertToSelection(logic.formatMSBuildProperty(name, project));
            Focus();
        }

        /// <summary>
        /// Implements transport for DTE command
        /// </summary>
        /// <param name="name"></param>
        public void command(string name)
        {
            if(radioModeOperation.Checked) {
                textEditor.insertToSelection(name + System.Environment.NewLine, false);
            }
            else {
                textEditor.insertToSelection(name);
            }
            Focus();
        }

        public void activateAction(string name, SolutionEventType type)
        {
            if(!switchEvent(type)) return;

            int index = Util.GetRowIndexByColumn(dgvActions, dgvActionName, name);
            if(index == -1)
            {
                Log.Trace($"UI.Activation. the name: {name} = -1");
                return;
            }
            selectAction(index, refreshSettings: true);
        }

        /// <summary>
        /// Implements transport for new action by event type.
        /// </summary>
        /// <param name="type">The type of event.</param>
        /// <param name="cfg">The event configuration for action.</param>
        public void action(SolutionEventType type, ISolutionEvent cfg)
        {
            switchEvent(type);
            ISolutionEvent evt = addAction(-1);

            if(evt == null || cfg == null) {
                Log.Debug("UI.action for `{0}` - cfg or evt is null /skip", type);
                return;
            }

            cfg.CloneByReflectionInto(evt, true);

            refreshActions(true);
            refreshSettings();
            notice(true);

            MessageBox.Show($"The new action `{evt.Name}`:\n`{evt.Caption}` has been added.", "New action");
        }

        public EventsFrm(Bootloader loader)
        {
            InitializeComponent();
            defaultSizes();
            updateColors();

            IInspector inspector    = new Inspector(loader.Soba);
            logic                   = new Logic.Events(loader, inspector);
            textEditor.codeCompletionInit(inspector, loader.Soba.EvMSBuild);
            
            Icon = Resource.Package_32;
            toolTip.SetToolTip(pictureBoxWarnWait, Resource.StringWarnForWaiting);

            Text = $"{loader.Env.SolutionFileName} - {Settings.APP_NAME}";

#if DEBUG
            Text += " [Debug version]";
            toolStripMenuVersion.Text = $"based on {Version.B_SHA1}";
#else
            toolStripMenuVersion.Text = $"v{Version.S_NUM}+{Version.B_SHA1}";
#endif

            //TODO: it was before with original dataGridView... need to check with DataGridViewExt and move it inside if still needed
            foreach(Control ctrl in Util.getControls(this, c => c.GetType() == typeof(DataGridViewExt))) {
                Util.fixDGVRowHeight((DataGridViewExt)ctrl); // solves problem with the height property
            }
        }

        /// <summary>
        /// Retrieve data from UI
        /// </summary>
        protected void saveData(bool onlyInRAM = false)
        {
            if(logic.SBEItem == null)
            {
                if(!onlyInRAM) {
                    logic.saveData();
                }
                return;
            }

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
                    case SolutionEventType.CommandEvent: {
                        saveData((CommandEvent)logic.SBEItem);
                        break;
                    }
                }
                componentApply();

                if(!onlyInRAM) {
                    logic.saveData();
                }
                requiresNotification = onlyInRAM && isNotified;
            }
            catch(Exception ex) {
                MessageBox.Show("Failed applying settings:\n" + ex.Message, "Configuration of event", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        protected void saveData(ISolutionEvent evt)
        {
            evt.Enabled                 = checkBoxStatus.Checked;
            evt.Name                    = string.IsNullOrWhiteSpace(evt.Name)? logic.UniqueNameForAction : evt.Name;
            evt.Caption                 = textBoxCaption.Text;
            evt.SupportMSBuild          = checkBoxMSBuildSupport.Checked;
            evt.SupportSBEScripts       = checkBoxSBEScriptSupport.Checked;
            evt.IgnoreIfBuildFailed     = checkBoxIgnoreIfFailed.Checked;
            evt.Process.Waiting         = checkBoxWaitForExit.Checked;
            evt.Process.Hidden          = checkBoxProcessHide.Checked;
            evt.Process.TimeLimit       = (int)numericTimeLimit.Value;
            evt.Confirmation            = chkConfirmation.Checked;
            evt.ToConfiguration         = checkedListBoxSpecCfg.CheckedItems.OfType<string>().ToArray();
            evt.ExecutionOrder          = getExecutionOrder();
            evt.BuildType               = chkBuildContext.Checked ? logic.getBuildTypeBy(comboBoxBuildContext.SelectedIndex) : BuildType.Common;

            if(evt.Mode.Type == ModeType.CSharp && !radioModeCSharp.Checked) {
                logic.cacheToRemove(evt.Mode);
            }

            if(radioModeInterpreter.Checked)
            {
                evt.Mode = new ModeInterpreter() {
                    Command = textEditor.Text,
                    Handler = comboBoxInterpreter.Text,
                    Newline = comboBoxNewline.Text,
                    Wrapper = comboBoxWrapper.Text.Trim()
                };
            }
            else if(radioModeFiles.Checked)
            {
                evt.Mode = new ModeFile() {
                    Command = textEditor.Text
                };
            }
            else if(radioModeScript.Checked)
            {
                evt.Mode = new ModeScript() {
                    Command = textEditor.Text
                };
            }
            else if(radioModeTargets.Checked) {
                evt.Mode = new ModeTargets() {
                    Command = textEditor.Text
                };
            }
            else if(radioModeCSharp.Checked)
            {
                evt.Mode        = (IMode)pGridCompilerCfg.SelectedObject;
                IModeCSharp cfg = (IModeCSharp)evt.Mode;

                cfg.Command = textEditor.Text;
                if(!cfg.CachingBytecode) {
                    logic.cacheToRemove(evt.Mode);
                }
                else {
                    // probably a new data - reset for recalculation later.
                    logic.cacheReset(evt.Mode);
                }
            }
            else if(radioModeOperation.Checked)
            {
                string[] command;
                if(isOperationCustom(listBoxOperation)) {
                    command = logic.splitOperations(textEditor.Text);
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

        protected void saveData(ISolutionEventEW evt)
        {
            evt.Codes       = listBoxEW.Items.Cast<string>().ToList();
            evt.IsWhitelist = radioCodesWhitelist.Checked;
        }

        protected void saveData(ISolutionEventOWP evt)
        {
            var list = new List<MatchWords>();
            foreach(DataGridViewRow row in dataGridViewOutput.Rows)
            {
                if(row.Cells[owpTerm.Name].Value == null || row.Cells[owpType.Name].Value == null) {
                    continue;
                }
                list.Add(new MatchWords
                {
                    Condition = (row.Cells[owpTerm.Name].Value == null) ? "" : row.Cells[owpTerm.Name].Value.ToString(),
                    Type = (ComparisonType)Enum.Parse(typeof(ComparisonType), row.Cells[owpType.Name].Value.ToString())
                });
            }
            evt.Match = list.ToArray();
        }

        protected void saveData(ICommandEvent evt)
        {
            List<Filter> list = new List<Filter>();
            foreach(DataGridViewRow row in dgvCEFilters.Rows)
            {
                if(row.IsNewRow) {
                    continue;
                }

                object customIn  = Value.PackArgument(row.Cells[dgvCEFiltersColumnCustomIn.Name].Value);
                object customOut = Value.PackArgument(row.Cells[dgvCEFiltersColumnCustomOut.Name].Value);
                object guid      = row.Cells[dgvCEFiltersColumnGuid.Name].Value;

                list.Add(new Filter()
                {
                    Guid        = (guid == null)? string.Empty : ((string)guid).Trim(),
                    CustomIn    = customIn.IsNullOrEmptyString() ? null : customIn,
                    CustomOut   = customOut.IsNullOrEmptyString() ? null : customOut,
                    Description = (string)row.Cells[dgvCEFiltersColumnDescription.Name].Value,
                    Id          = Convert.ToInt32(row.Cells[dgvCEFiltersColumnId.Name].Value),
                    Cancel      = Convert.ToBoolean(row.Cells[dgvCEFiltersColumnCancel.Name].Value),
                    Pre         = Convert.ToBoolean(row.Cells[dgvCEFiltersColumnPre.Name].Value),
                    Post        = Convert.ToBoolean(row.Cells[dgvCEFiltersColumnPost.Name].Value),
                });
            }
            evt.Filters = list.ToArray();
        }

        /// <summary>
        /// Rendering data into UI elements
        /// </summary>
        protected void renderData()
        {
            if(logic.SBEItem == null) {
                controlsLock(true);
                return;
            }
            controlsLock(false);

            renderData(logic.SBEItem);

            checkBoxIgnoreIfFailed.Enabled  = false;
            groupBoxOutputControl.Enabled   = false;
            groupBoxEW.Enabled              = false;
            pictureBoxWarnWait.Visible      = true;
            groupBoxCommandEvents.Enabled   = false;

            textEditor._.Select(0, 0);
            textEditor._.ScrollToHome();
            toolTip.SetToolTip(checkBoxWaitForExit, string.Empty);
            checkBoxWaitForExit.Cursor = Cursors.Default;

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
                case SolutionEventType.SlnClosed:
                case SolutionEventType.Post:
                {
                    checkBoxIgnoreIfFailed.Enabled  = true;
                    pictureBoxWarnWait.Visible      = false;
                    break;
                }
                case SolutionEventType.SlnOpened: {
                    pictureBoxWarnWait.Visible = false;
                    break;
                }
                case SolutionEventType.Logging:
                {
                    toolTip.SetToolTip(checkBoxWaitForExit, "Enable, Only if you know what you're doing with Logging event!");
                    checkBoxWaitForExit.Cursor = Cursors.Help;
                    break;
                }
                case SolutionEventType.CommandEvent: {
                    renderData((CommandEvent)logic.SBEItem);
                    groupBoxCommandEvents.Enabled = true;
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
            numericTimeLimit.Value              = evt.Process.TimeLimit;
            checkBoxProcessHide.Checked         = evt.Process.Hidden;
            chkConfirmation.Checked             = evt.Confirmation;
            buildTypeSelect(evt.BuildType);

            if(evt.Mode == null) {
                Log.Warn("Mode is corrupt, reinitialized with default type - '{0}'", logic.DefaultMode.Type);
                evt.Mode = logic.DefaultMode;
            }
            pGridCompilerCfg.SelectedObject = (evt.Mode.Type == ModeType.CSharp)? (IModeCSharp)evt.Mode : new ModeCSharp();

            // update settings for editor
            if(!isChangingMode(evt.Mode.Type)) {
                textEditor.config(logic.getCommonCfg(evt.Mode.Type));
            }

            switch(evt.Mode.Type)
            {
                case ModeType.Interpreter:
                {
                    radioModeInterpreter.Checked    = true;
                    textEditor.Text                 = ((IModeInterpreter)evt.Mode).Command;
                    comboBoxInterpreter.Text        = ((IModeInterpreter)evt.Mode).Handler;
                    comboBoxNewline.Text            = ((IModeInterpreter)evt.Mode).Newline;
                    comboBoxWrapper.Text            = ((IModeInterpreter)evt.Mode).Wrapper;
                    return;
                }
                case ModeType.File:
                {
                    radioModeFiles.Checked = true;
                    textEditor.Text = ((IModeFile)evt.Mode).Command;
                    return;
                }
                case ModeType.Script: {
                    radioModeScript.Checked = true;
                    textEditor.Text = ((IModeScript)evt.Mode).Command;
                    return;
                }
                case ModeType.Targets: {
                    radioModeTargets.Checked = true;
                    textEditor.Text = ((IModeTargets)evt.Mode).Command;
                    return;
                }
                case ModeType.CSharp: {
                    radioModeCSharp.Checked = true;
                    textEditor.Text = ((IModeCSharp)evt.Mode).Command;
                    return;
                }
                case ModeType.Operation:
                {
                    radioModeOperation.Checked  = true;
                    IModeOperation mode         = (IModeOperation)evt.Mode;

                    if(logic.isDefOperation(mode.Caption)) {
                        textEditor.Text = "> " + mode.Caption;
                    }
                    else {
                        textEditor.Text = (mode.Command == null)? "" : logic.joinOperations(mode.Command);
                    }
                    checkBoxOperationsAbort.Checked = mode.AbortOnFirstError;
                    return;
                }
            }
        }

        protected void renderData(ISolutionEventEW evt)
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

        protected void renderData(ISolutionEventOWP evt)
        {
            dataGridViewOutput.Rows.Clear();
            if(evt.Match == null) {
                return;
            }
            foreach(MatchWords m in evt.Match) {
                dataGridViewOutput.Rows.Add(m.Condition, m.Type.ToString());
            }
        }

        protected void renderData(ICommandEvent evt)
        {
            dgvCEFilters.Rows.Clear();
            if(evt.Filters == null) {
                return;
            }
            foreach(IFilter f in evt.Filters) {
                dgvCEFilters.Rows.Add(f.Guid, f.Id, Value.Pack(f.CustomIn), Value.Pack(f.CustomOut), f.Description, f.Cancel, f.Pre, f.Post);
            }
        }

        protected void fillActionsList()
        {
            dgvActions.Rows.Clear();
            foreach(ISolutionEvent item in logic.SBE.evt) addDgvAction(item.Enabled, item.Name, item.Caption);
        }

        protected void refreshSettings()
        {
            clearControls();
            operationsList(listBoxOperation);
            renderData();
            onlyFor();
            executionOrder();
            updateColors();

            if(!requiresNotification) {
                notice(false);
            }
        }

        protected void refreshActions(bool rememberIndex = true)
        {
            int selectedRowIndex = (rememberIndex)? currentActionIndex() : 0;
            fillActionsList();
            selectAction(selectedRowIndex);
        }

        protected void selectAction(int index, bool refreshSettings = false)
        {
            if(dgvActions.Rows.Count < 1) {
                return;
            }

            index = Math.Max(0, Math.Min(index, dgvActions.Rows.Count - 1));
            dgvActions.ClearSelection();
            dgvActions.Rows[index].Selected = true;
            logic.setEventIndexes(comboBoxEvents.SelectedIndex, index);

            if(refreshSettings) {
                this.refreshSettings();
            }
        }

        protected void refreshSettingsWithIndex(int index)
        {
            logic.setEventIndexes(comboBoxEvents.SelectedIndex, index);
            refreshSettings();
        }

        protected ISolutionEvent addAction(int copyFrom = -1)
        {
            try {
                ISolutionEvent evt = logic.addEventItem(copyFrom);
                addDgvAction(evt.Enabled, evt.Name, evt.Caption);
                selectAction(dgvActions.Rows.Count - 1, true);
                return evt;
            }
            catch(Exception ex) {
                Log.Error($"Failed to add event-item: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
            finally {
                notice(true);
            }
            return null;
        }

        protected void removeRow(DataGridViewExt dgv, DataGridViewButtonColumn btn, DataGridViewCellEventArgs idx)
        {
            if(idx.RowIndex == -1 || idx.ColumnIndex == -1) {
                return; //headers
            }

            if(idx.ColumnIndex == dgv.Columns.IndexOf(btn) && idx.RowIndex < dgv.Rows.Count - 1) {
                dgv.Rows.Remove(dgv.Rows[idx.RowIndex]);
                notice(true);
            }
        }

        protected void componentApply()
        {
            var list = new List<Configuration.Component>();
            foreach(DataGridViewRow row in dgvComponents.Rows)
            {
                if(row.ReadOnly) {
                    continue;
                }

                var c = row.Cells[dgvComponentsClass.Name].Value?.ToString();
                if(c == null || list.Any(p => p.ClassName == c)) {
                    continue;
                }

                list.Add(new Configuration.Component() { 
                    ClassName   = c,
                    Enabled     = Boolean.Parse(row.Cells[dgvComponentsEnabled.Name].Value.ToString()),
                });
            }

            logic.updateComponents(list.ToArray());
        }

        protected void clearControls()
        {
            textBoxEW.Text  = string.Empty;
            textEditor.Text = string.Empty;
            listBoxEW.Items.Clear();
            dataGridViewOutput.Rows.Clear();
            dgvCEFilters.Rows.Clear();
        }

        protected void controlsLock(bool disabled)
        {
            tabControl.Enabled = !disabled;

            if(disabled) {
                panelCommand.BackColor = Color.Gray;
                textEditor.setBackgroundFromString("#DDDDDD");
            }
            else {
                updateColors();
            }
        }

        protected void notice(bool isOn)
        {
            isNotified = isOn;
            if(isOn) {
                btnApply.FlatAppearance.BorderColor = Color.FromArgb(255, 0, 0);
                return;
            }
            btnApply.FlatAppearance.BorderColor = Color.FromArgb(0, 0, 0);
        }

        /// <summary>
        /// UI Selector of modes
        /// </summary>
        protected void uiViewMode(ModeType type)
        {
            textEditor.config(logic.getCommonCfg(type));
            updateColors();
            groupBoxInterpreter.Enabled         = false;
            listBoxOperation.Enabled            = false;
            textEditor.Enabled                  = true;
            checkBoxProcessHide.Enabled         = true;
            checkBoxOperationsAbort.Enabled     = false;
            checkBoxSBEScriptSupport.Enabled    = true;
            pGridCompilerCfg.Enabled            = false;
            btnActivateCSharp.Visible           = true;
            updateTimeLimitField();
            updateCodeCompletionStatus();

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
                checkBoxProcessHide.Enabled      = false;
                return;
            }
            if(type == ModeType.Targets) {
                labelToCommandBox.Text = ".targets:";
                if(textEditor.Text.Length < 1) {
                    textEditor.Text = Resource.StringDefaultValueForTargetsMode;
                }
                return;
            }
            if(type == ModeType.CSharp) {
                labelToCommandBox.Text = "C# code:";
                pGridCompilerCfg.Enabled    = true;
                btnActivateCSharp.Visible   = false;
                checkBoxProcessHide.Enabled = false;
                if(textEditor.Text.Length < 1) {
                    textEditor.Text = Resource.StringCSharpModeCodeByDefault;
                }
                return;
            }
            if(type == ModeType.Operation)
            {
                operationsAction(listBoxOperation);
                listBoxOperation.Enabled        = true;
                checkBoxOperationsAbort.Enabled = true;
                checkBoxProcessHide.Enabled     = false;
                return;
            }
        }

        protected bool isChangingMode(ModeType current)
        {
            switch(current) {
                case ModeType.Interpreter: { return !radioModeInterpreter.Checked; }
                case ModeType.File: { return !radioModeFiles.Checked; }
                case ModeType.Script: { return !radioModeScript.Checked; }
                case ModeType.Targets: { return !radioModeTargets.Checked; }
                case ModeType.CSharp: { return !radioModeCSharp.Checked; }
                case ModeType.Operation: { return !radioModeOperation.Checked; }
            }
            return true;
        }

        protected void updateTimeLimitField()
        {
            if(checkBoxWaitForExit.Checked 
                && (radioModeFiles.Checked || radioModeInterpreter.Checked))
            {
                numericTimeLimit.Enabled = true;
                return;
            }
            numericTimeLimit.Enabled = false;
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
            if(logic.SBEItem == null || logic.SBEItem.Mode.Type != ModeType.Operation) {
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
                textEditor.Enabled      = true;
            }
            else {
                labelToCommandBox.Text  = "~";
                textEditor.Enabled      = false;
                textEditor.Text         = ":: " + logic.DefOperations[list.SelectedIndex].Caption;
            }
        }

        protected int currentActionIndex() => (dgvActions.SelectedRows.Count < 1) ? 0 : dgvActions.SelectedRows[0].Index;

        protected void updateCodeCompletionStatus() => textEditor.CodeCompletionEnabled = checkBoxSBEScriptSupport.Checked;

        protected bool isOperationCustom(ListBox list) => list.SelectedIndex == list.Items.Count - 1;

        protected void onlyFor()
        {
            if(checkedListBoxSpecCfg.Items.Count < 1) {
                foreach(var cfg in logic.Env.SolutionConfigurations) {
                    checkedListBoxSpecCfg.Items.Add(logic.Env.SolutionCfgFormat(cfg), false);
                }
            }

            if(logic.SBEItem == null) {
                return;
            }

            string[] toConf = logic.SBEItem.ToConfiguration;
            for(int i = 0; i < checkedListBoxSpecCfg.Items.Count; ++i)
            {
                string name = checkedListBoxSpecCfg.Items[i].ToString();
                bool state  = toConf != null && toConf.Any(s => s == name);
                checkedListBoxSpecCfg.SetItemChecked(i, state);
            }
        }

        protected void executionOrder()
        {
            setExecutionOrderRecords();

            if(logic.SBEItem == null) {
                return;
            }

            IExecutionOrder[] list = logic.SBEItem.ExecutionOrder;
            foreach(DataGridViewRow row in dataGridViewOrder.Rows)
            {
                if(list == null || list.Length < 1) {
                    row.Cells[dgvOrderEnabled.Name].Value   = false;
                    row.Cells[dgvOrderType.Name].Value      = ExecutionOrderType.Before.ToString();
                    continue;
                }

                IExecutionOrder v = list.Where(s => s.Project == row.Cells[dgvOrderProject.Name].Value.ToString()).FirstOrDefault();
                if(v == null) {
                    continue;
                }
                row.Cells[dgvOrderEnabled.Name].Value = !string.IsNullOrEmpty(v.Project);
                row.Cells[dgvOrderType.Name].Value = v.Order.ToString();
            }
        }

        protected void setExecutionOrderRecords()
        {
            if(dataGridViewOrder.Rows.Count > 0) {
                return;
            }

            var projects = logic.Env.ProjectsList;
            if(projects.Count < 1) {
                return;
            }

            foreach(string name in projects) {
                addExecutionOrderRecord(name, Color.Empty);
            }

            // special records

            Color sColor = Color.FromArgb(243, 239, 252);

            addExecutionOrderRecord(ExecutionOrder.FIRST_PROJECT, sColor);
            if(projects.Count > 1) {
                addExecutionOrderRecord(ExecutionOrder.FIRST_TYPE, sColor);
                addExecutionOrderRecord(ExecutionOrder.LAST_PROJECT, sColor);
                addExecutionOrderRecord(ExecutionOrder.LAST_TYPE, sColor);
            }
        }

        protected void addExecutionOrderRecord(string name, Color rowcolor)
        {
            int idx = dataGridViewOrder.Rows.Add(false, name, dgvOrderType.Items[0]);
            if(rowcolor != Color.Empty) {
                dataGridViewOrder.Rows[idx].DefaultCellStyle.BackColor = rowcolor;
            }
        }

        protected ExecutionOrder[] getExecutionOrder()
        {
            List<ExecutionOrder> ret = new List<ExecutionOrder>(dataGridViewOrder.Rows.Count);
            foreach(DataGridViewRow row in dataGridViewOrder.Rows)
            {
                if(!Convert.ToBoolean(row.Cells[dgvOrderEnabled.Name].Value)) {
                    continue;
                }

                ret.Add(new ExecutionOrder() {
                    Project = row.Cells[dgvOrderProject.Name].Value.ToString(),
                    Order   = (ExecutionOrderType)Enum.Parse(typeof(ExecutionOrderType), row.Cells[dgvOrderType.Name].Value.ToString())
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

        protected void snifferEnabled(bool status)
        {
            if(status) {
                logic.attachCommandEvents(commandEventBefore, commandEventAfter);
                return;
            }
            logic.detachCommandEvents(commandEventBefore, commandEventAfter);
        }

        protected void commandEvent(bool pre, string guid, int id, object customIn, object customOut)
        {
            if(dgvCESniffer == null) {
                return;
            }

            if(dgvCESniffer.Rows.Count > snifferRcBuffer) {
                dgvCESniffer.Rows.RemoveAt(0);
            }

            string tFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + " .fff";
            dgvCESniffer.Rows.Add
            (
                DateTime.Now.ToString(tFormat),
                pre,
                guid,
                id,
                Value.Pack(customIn),
                Value.Pack(customOut),
                EnumDecor.Shorten(Util.enumViewBy(guid, id))
            );
        }

        protected void addFilterFromSniffer(DataGridView sniffer, DataGridView filter)
        {
            if(sniffer.Rows.Count < 1 || sniffer.SelectedRows.Count < 1 || !filter.Enabled) {
                return;
            }
            DataGridViewRow rc = sniffer.SelectedRows[0];
            filter.Rows.Add(rc.Cells[dgvCESnifferColumnGuid.Name].Value,
                            rc.Cells[dgvCESnifferColumnId.Name].Value, 
                            rc.Cells[dgvCESnifferColumnCustomIn.Name].Value,
                            rc.Cells[dgvCESnifferColumnCustomOut.Name].Value,
                            rc.Cells[dgvCESnifferColumnSrc.Name].Value);
        }

        protected void envVariablesUIHelper()
        {
            if(Util.focusForm(frmProperties)) {
                return;
            }
            frmProperties = new PropertiesFrm(logic.Env, this);
            frmProperties.Show();
        }

        protected void updateColors()
        {
            checkBoxStatus.BackColor = (checkBoxStatus.Checked)? Color.FromArgb(242, 250, 241) : Color.FromArgb(248, 243, 243);
            panelCommand.BackColor   = (checkBoxStatus.Checked)? Color.FromArgb(111, 145, 6) : Color.FromArgb(168, 47, 17);

            if(radioModeScript.Checked || radioModeTargets.Checked || radioModeCSharp.Checked) {
                textEditor.setBackgroundFromString("#FFFFFF");
                return;
            }

            if(checkBoxStatus.Checked) {
                textEditor.setBackgroundFromString("#F2FAF1");
            }
            else {
                textEditor.setBackgroundFromString("#F8F3F3");
            }
        }

        protected void expandActionsList(bool open)
        {
            // Possible next bug:
            // The Width property may be unchangeable and less then Panel1MinSize or Panel2MinSize.
            // For example: 160px (after click on preview box in taskbar on Win8.1 at least):
            // Then! the next condition or requirements - 'SplitterDistance must be between Panel1MinSize and Width - Panel2MinSize' is unreachable at all.
            // but, maybe it possible only for old logic of SC_RESTORE/2 (WndProc)... hmm,
            try
            {
                if(!open) {
                    splitContainer.SplitterDistance = splitContainer.Panel1MinSize; // may be an exception due to immutable Width and 160px (n. above)
                    Width = metric.formCollapsed.Width;
                    return;
                }
                Width = metric.form.Width;
                splitContainer.SplitterDistance = Math.Max(splitContainer.Panel1MinSize, Math.Min(metric.splitter, Width - splitContainer.Panel2MinSize));
            }
            catch(Exception ex) {
                Log.Debug("UI. bug with splitContainer({0}) /Width: '{1}' :: '{2}' ", open, Width, ex.ToString());
                //Size    = metric.formCollapsed; // we still can't resize, but we try...
                //Width   = metric.form.Width;  // and try...
            }
        }

        protected void defaultSizes()
        {
            metric.form = Size;
            metric.splitter = splitContainer.SplitterDistance;
            metric.formCollapsed = new Size(Width - (metric.splitter), Height);
        }

        protected bool switchEvent(SolutionEventType type)
        {
            int pos = logic.getDefIndexByEventType(type);
            if(pos == -1) {
                Log.Trace($"UI.Activation. the type {type} = -1");
                return false;
            }
            comboBoxEvents.SelectedIndex = pos;
            return true;
        }

        protected void fillEvents(ComboBox combo)
        {
            //TODO: exclude saveData(true) for event:
            combo.SelectedIndexChanged -= comboBoxEvents_SelectedIndexChanged;
            logic.fillEvents(combo);
            combo.SelectedIndexChanged += comboBoxEvents_SelectedIndexChanged;

            fillCfgForSelectedEvent();
        }

        protected void fillCfgForSelectedEvent()
        {
            logic.setEventIndexes(comboBoxEvents.SelectedIndex, 0);
            refreshActions(false);
            refreshSettings();
        }

        protected int addDgvAction(bool enabled, string name, string caption)
        {
            int idx = dgvActions.Rows.Add(enabled, name);
            dgvActions.Rows[idx].HeaderCell.ToolTipText = caption;
            dgvActions.Rows[idx].Cells[dgvActionName.Name].ToolTipText = caption;
            return idx;
        }

        protected override void WndProc(ref Message mes)
        {
            base.WndProc(ref mes); // should be before, otherwise may cause problem with Width property after restoring from taskbar (always 160px for example)

            if(mes.Msg == WM_SYSCOMMAND)
            {
                if(mes.WParam == (IntPtr)SC_RESTORE || mes.WParam == (IntPtr)SC_RESTORE2) {
                    expandActionsList(false);
                }
            }
        }

        private void EventsFrm_Load(object sender, EventArgs e)
        {
            if(!App.Config.IsLoadedConfigs)
            {
                Log.Fatal($"Corrupted configuration. {App.Config}");
                MessageBox.Show("We can't continue. See log for details with activated debug mode.", "Corrupted configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
                FormClosing -= EventsFrm_FormClosing;
                Close();
                return;
            }

            void _call(object csender, EventArgs ce) => notice(true);

            Util.noticeAboutChanges(typeof(CheckBox), this, _call, new string[] { checkBoxCESniffer.Name });
            Util.noticeAboutChanges(typeof(RadioButton), this, _call);
            Util.noticeAboutChanges(typeof(TextBox), this, _call);
            Util.noticeAboutChanges(typeof(RichTextBox), this, _call);
            Util.noticeAboutChanges(typeof(ListBox), this, _call, new string[] { listBoxEW.Name });
            Util.noticeAboutChanges(typeof(ComboBox), this, _call, new string[] { comboBoxEvents.Name });
            Util.noticeAboutChanges(typeof(CheckedListBox), this, _call);
            Util.noticeAboutChanges(typeof(DataGridViewExt), this, _call, new string[] { dgvCESniffer.Name });
            Util.noticeAboutChanges(typeof(PropertyGrid), this, _call);
            textEditor._.TextChanged += _call;

            // Load data
            try {
                expandActionsList(false);
                logic.fillComponents(dgvComponents);
                logic.fillBuildTypes(comboBoxBuildContext);
                fillEvents(comboBoxEvents);
            }
            catch(Exception ex) {
                Log.Error($"Failed to load form: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }

            TopMost = toolStripMenuPin.Toggle(App.Config.Sys.Data.PinMainWindow);
            toolStripMenuSuppressInitOwp.Toggle(App.Config.Sys.Data.SuppressInitOwp);

            notice(false);
        }

        private void comboBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {
            try {
                saveData(true); //TODO: exclude from 'reset' & for 'load'
                fillCfgForSelectedEvent();
            }
            catch(Exception ex) {
                Log.Error($"Failed to select event type: {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        private void textBoxCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Modifiers == Keys.Control && e.KeyCode == Keys.Space) {
                e.SuppressKeyPress = true;
                envVariablesUIHelper();
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            dgvActions.EndEdit();
            dgvComponents.EndEdit();
            dgvCEFilters.EndEdit();
            
            saveData();
            refreshActions();
            notice(false);
        }

        private void toolStripMenuReset_Click(object sender, EventArgs e)
        {
            logic.restoreData();
            fillEvents(comboBoxEvents);
            logic.fillComponents(dgvComponents);
            logic.setEventIndexes(comboBoxEvents.SelectedIndex, 0);
            refreshActions(false);
            refreshSettings();

            notice(false);
            requiresNotification = false;
        }

        private void radioModeFiles_CheckedChanged(object sender, EventArgs e)
        {
            if(!radioModeFiles.Checked) {
                return;
            }
            uiViewMode(ModeType.File);
            textEditor.colorize(TextEditor.ColorSchema.FilesMode);
            textEditor._.ShowLineNumbers = false;
        }

        private void radioModeOperation_CheckedChanged(object sender, EventArgs e)
        {
            if(!radioModeOperation.Checked) {
                return;
            }
            uiViewMode(ModeType.Operation);
            textEditor.colorize(TextEditor.ColorSchema.OperationMode);
            textEditor._.ShowLineNumbers = false;
        }

        private void radioModeInterpreter_CheckedChanged(object sender, EventArgs e)
        {
            if(!radioModeInterpreter.Checked) {
                return;
            }
            uiViewMode(ModeType.Interpreter);
            textEditor.colorize(TextEditor.ColorSchema.InterpreterMode);
            textEditor._.ShowLineNumbers = false;
        }

        private void radioModeScript_CheckedChanged(object sender, EventArgs e)
        {
            if(!radioModeScript.Checked) {
                return;
            }
            uiViewMode(ModeType.Script);
            textEditor.colorize(TextEditor.ColorSchema.SBEScripts);
            textEditor._.ShowLineNumbers        = true;
            textEditor.CodeCompletionEnabled    = true;
        }

        private void radioModeTargets_CheckedChanged(object sender, EventArgs e)
        {
            if(!radioModeTargets.Checked) {
                return;
            }
            uiViewMode(ModeType.Targets);
            textEditor.colorize(TextEditor.ColorSchema.MSBuildTargets);
            textEditor._.ShowLineNumbers = true;
        }

        private void radioModeCSharp_CheckedChanged(object sender, EventArgs e)
        {
            if(!radioModeCSharp.Checked) {
                return;
            }
            uiViewMode(ModeType.CSharp);
            textEditor.colorize(TextEditor.ColorSchema.CSharpLang);
            textEditor._.ShowLineNumbers = true;
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

        private void toolStripMenuReport_Click(object sender, EventArgs e)
        {
            DialogResult ret = MessageBox.Show("Seen error or have a question - Click 'Yes'", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if(ret == DialogResult.Yes) {
                Util.openUrl("https://github.com/3F/vsSolutionBuildEvent/issues/new");
            }
        }

        private void toolStripMenuDebugMode_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.DebugMode = App.DebugMode = toolStripMenuDebugMode.Toggle();
            App.Config.Sys.save();
        }

        private void dgvComponents_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == dgvComponents.Columns.IndexOf(dgvComponentsEnabled) || e.RowIndex < 0) {
                return;
            }
            componentInfo(dgvComponents.Rows[e.RowIndex].Cells[dgvComponentsClass.Name].Value.ToString());
        }

        private void dgvComponents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex == -1 || e.ColumnIndex != dgvComponents.Columns.IndexOf(dgvComponentsIcon)) {
                return;
            }
            string name = dgvComponents.Rows[e.RowIndex].Cells[dgvComponentsClass.Name].Value.ToString();

            foreach(DataGridViewRow row in dgvComponents.Rows) {
                if(row.Cells[dgvComponentsClass.Name].Value.ToString() == name) {
                    row.Cells[dgvComponentsEnabled.Name].Value = dgvComponents.Rows[e.RowIndex].Cells[dgvComponentsEnabled.Name].Value;
                }
            }
        }

        private void dgvComponents_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == dgvComponents.Columns.IndexOf(dgvComponentsIcon)) {
                dgvComponents.EndEdit();
            }
        }

        private void dgvComponents_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) {
                return;
            }

            dgvComponentInfo.Rows.Clear();
            foreach(INodeInfo info in logic.infoByComponent(dgvComponents.Rows[e.RowIndex].Cells[dgvComponentsClass.Name].Value.ToString()))
            {
                Bitmap bmap = DomIcon.definition;
                switch(info.Type) {
                    case NodeType.Property: {
                        bmap = DomIcon.property;
                        break;
                    }
                    case NodeType.Method: {
                        bmap = DomIcon.function;
                        break;
                    }
                    case NodeType.Definition: {
                        bmap = DomIcon.definition;
                        break;
                    }
                }
                dgvComponentInfo.Rows.Add(bmap, info.Overname, (info.Signature == null)? "" : info.Signature.Replace("\n", "  \n"), info.Description);
            }
        }

        private void menuItemCompDoc_Click(object sender, EventArgs e)
        {
            if(dgvComponents.SelectedRows.Count < 1) {
                return;
            }
            componentInfo(dgvComponents.SelectedRows[0].Cells[dgvComponentsClass.Name].Value.ToString());
        }

        private void btnDteCmd_Click(object sender, EventArgs e)
        {
            if(Util.focusForm(frmDTECommands)) {
                return;
            }
            frmDTECommands = new DTECommandsFrm(logic.Env.Commands?.Cast<EnvDTE.Command>(), this);
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

        private void menuItemSniffer_Click(object sender, EventArgs e)
        {
            if(Util.focusForm(frmSniffer)) {
                return;
            }
            frmSniffer = new EnvDteSniffer(logic.Env);
            frmSniffer.Show();
        }

        private void menuWizardVersion_Click(object sender, EventArgs e)
        {
            if(Util.focusForm(frmWizVersion)) {
                return;
            }
            frmWizVersion = new Wizards.VersionFrm(logic.Loader, this);
            frmWizVersion.Show();
        }

        private void EventsFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            snifferEnabled(false);
            Util.closeTool(frmProperties);
            Util.closeTool(frmPropertyCheck);
            Util.closeTool(frmDTECommands);
            Util.closeTool(frmDTECheck);
            Util.closeTool(frmSBEScript);
            Util.closeTool(frmSniffer);
            Util.closeTool(frmWizVersion);
            logic.restoreData();
        }

        private void menuActionsEdit_Click(object sender, EventArgs e)
        {
            if(dgvActions.Rows.Count < 1) {
                return;
            }
            dgvActions.CurrentCell = dgvActions.Rows[currentActionIndex()].Cells[dgvActionName.Name];
            dgvActions.BeginEdit(true);
        }

        private void menuActionsRemove_Click(object sender, EventArgs e)
        {
            if(dgvActions.Rows.Count < 1) {
                return;
            }
            int index = currentActionIndex();
            dgvActions.Rows.RemoveAt(index);
            logic.removeEventItem(index);
            refreshSettingsWithIndex(currentActionIndex());
            notice(true);
        }

        private void dgvActions_Click(object sender, EventArgs e)
        {
            if(splitContainer.SplitterDistance < metric.splitter) {
                expandActionsList(true);
            }
        }

        private void EventsFrm_ClientSizeChanged(object sender, EventArgs e)
        {
            if(WindowState == FormWindowState.Maximized) {
                expandActionsList(true);
            }
        }

        private void dgvActions_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if(e.ColumnIndex != dgvActions.Columns.IndexOf(dgvActionName)) {
                return;
            }

            string origin = logic.SBE.evt[e.RowIndex].Name;
            try
            {
                logic.SBE.evt[e.RowIndex].Name = null;
                e.Value = logic.genUniqueName(logic.validateName(e.Value.ToString()), logic.SBE.evt);
                logic.SBE.evt[e.RowIndex].Name = (string)e.Value;
            }
            catch(Exception ex) {
                Log.Debug("Name for action: failed parsing - '{0}'", ex.Message);
                e.Value = origin;
            }
            e.ParsingApplied = true;
        }

        private void dgvActions_MouseDown(object sender, MouseEventArgs e)
        {
            // MouseDown because the CellClick event may not be called for some rows
            // the RowEnter called is too late..
            if(e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                DataGridView.HitTestInfo inf = dgvActions.HitTest(e.X, e.Y);
                if(inf.RowIndex == -1) {
                    return;
                }
                saveData(true);

                if(inf.ColumnIndex != dgvActions.Columns.IndexOf(dgvActionEnabled)) {
                    refreshSettingsWithIndex(inf.RowIndex);
                    return;
                }
                logic.setEventIndexes(comboBoxEvents.SelectedIndex, inf.RowIndex);
            }
        }

        private void dgvActions_MouseUp(object sender, MouseEventArgs e)
        {
            if(dgvActions.HitTest(e.X, e.Y).ColumnIndex == dgvActions.Columns.IndexOf(dgvActionEnabled)) {
                refreshSettings();
            }
        }

        private void dgvActions_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Up || e.KeyCode == Keys.Down) {
                saveData(true);
                refreshSettingsWithIndex(currentActionIndex());
            }
        }

        private void dgvActions_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.KeyCode)
            {
                case Keys.F2: {
                    menuActionsEdit_Click(this, e);
                    break;
                }
                case Keys.Space: {
                    dgvActions.Rows[currentActionIndex()].Cells[dgvActionEnabled.Name].Value = !logic.SBEItem.Enabled;
                    break;
                }
                case Keys.Enter: {
                    e.SuppressKeyPress = true;
                    break;
                }
            }
            e.Handled = true;
        }

        private void dgvActions_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if(e.ColumnIndex == dgvActions.Columns.IndexOf(dgvActionEnabled)) {
                dgvActions.EndEdit();
            }
        }

        private void dgvActions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex < 0) {
                return;
            }
            bool enabled = bool.Parse(dgvActions.Rows[e.RowIndex].Cells[dgvActionEnabled.Name].Value.ToString());
            object oname = dgvActions.Rows[e.RowIndex].Cells[dgvActionName.Name].Value;

            logic.updateInfo(e.RowIndex, (oname == null)? "" : oname.ToString(), enabled);
            requiresNotification = true;
            refreshSettings();
        }

        private void dgvActions_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if(dgvActions.Rows.Count < 1) {
                linkAddAction.Visible = true;
            }
        }

        private void menuTplTargetsDefault_Click(object sender, EventArgs e)
        {
            radioModeTargets.Checked = true;
            textEditor.Text = Resource.StringDefaultValueForTargetsMode;
        }

        private void menuTplCSharpDefault_Click(object sender, EventArgs e)
        {
            radioModeCSharp.Checked = true;
            textEditor.Text = Resource.StringCSharpModeCodeByDefault;
        }

        private void toolStripMenuVersion_MouseHover(object sender, EventArgs e)
        {
            toolStripMenuVersion.BorderSides = ToolStripStatusLabelBorderSides.Bottom;
            statusStrip.Cursor = Cursors.Hand;
        }

        private void toolStripMenuVersion_MouseLeave(object sender, EventArgs e)
        {
            toolStripMenuVersion.BorderSides = ToolStripStatusLabelBorderSides.None;
            statusStrip.Cursor = Cursors.Default;
        }

        private void linkAddAction_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            expandActionsList(true);
            addAction();
        }

        private void menuLogIgnoreTrace_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.LogIgnoreLevels[LogLevel.Trace.Name] = menuLogIgnoreTrace.Toggle();
            App.Config.Sys.save();
        }

        private void menuLogIgnoreDebug_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.LogIgnoreLevels[LogLevel.Debug.Name] = menuLogIgnoreDebug.Toggle();
            App.Config.Sys.save();
        }

        private void menuLogIgnoreInfo_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.LogIgnoreLevels[LogLevel.Info.Name] = menuLogIgnoreInfo.Toggle();
            App.Config.Sys.save();
        }

        private void menuLogIgnoreWarn_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.LogIgnoreLevels[LogLevel.Warn.Name] = menuLogIgnoreWarn.Toggle();
            App.Config.Sys.save();
        }

        private void menuLogIgnoreError_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.LogIgnoreLevels[LogLevel.Error.Name] = menuLogIgnoreError.Toggle();
            App.Config.Sys.save();
        }

        private void toolStripMenuBug_DropDownOpening(object sender, EventArgs e)
        {
            toolStripMenuDebugMode.Checked = App.DebugMode;

            var ignore = App.Config.Sys.Data.LogIgnoreLevels;
            menuLogIgnoreTrace.Checked  = ignore.GetOrDefault(LogLevel.Trace.Name);
            menuLogIgnoreDebug.Checked  = ignore.GetOrDefault(LogLevel.Debug.Name);
            menuLogIgnoreInfo.Checked   = ignore.GetOrDefault(LogLevel.Info.Name);
            menuLogIgnoreWarn.Checked   = ignore.GetOrDefault(LogLevel.Warn.Name);
            menuLogIgnoreError.Checked  = ignore.GetOrDefault(LogLevel.Error.Name);
        }

        private void toolStripMenuSuppressInitOwp_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.SuppressInitOwp = toolStripMenuSuppressInitOwp.Toggle();
            App.Config.Sys.save();
        }

        private void toolStripMenuPin_Click(object sender, EventArgs e)
        {
            App.Config.Sys.Data.PinMainWindow
                = TopMost
                = toolStripMenuPin.Toggle();

            App.Config.Sys.save();
        }

        #region one-line binding
        protected void commandEventBefore(string guid, int id, object customIn, object customOut, ref bool cancelDefault) => commandEvent(true, guid, id, customIn, customOut);
        protected void commandEventAfter(string guid, int id, object customIn, object customOut) => commandEvent(false, guid, id, customIn, customOut);
        private void listBoxOperation_SelectedIndexChanged(object sender, EventArgs e) => operationsAction(listBoxOperation);
        private void buttonEnvVariables_Click(object sender, EventArgs e) => envVariablesUIHelper();
        private void checkBoxStatus_CheckedChanged(object sender, EventArgs e) => updateColors();
        private void chkBuildContext_CheckedChanged(object sender, EventArgs e) => comboBoxBuildContext.Enabled = chkBuildContext.Checked;
        private void checkBoxSBEScriptSupport_CheckedChanged(object sender, EventArgs e) => updateCodeCompletionStatus();
        private void checkBoxWaitForExit_CheckedChanged(object sender, EventArgs e) => updateTimeLimitField();
        private void dataGridViewOutput_CellClick(object sender, DataGridViewCellEventArgs e) => removeRow(dataGridViewOutput, owpRemove, e);
        private void toolStripMenuHelp_ButtonClick(object sender, EventArgs e) => toolStripMenuHelp.ShowDropDown();
        private void toolStripMenuSettings_ButtonClick(object sender, EventArgs e) => toolStripMenuSettings.ShowDropDown();
        private void toolStripMenuBug_ButtonClick(object sender, EventArgs e) => toolStripMenuBug.ShowDropDown();
        private void toolStripMenuAbout_Click(object sender, EventArgs e) => new AboutFrm().Show();
        private void toolStripMenuSBEPanel_Click(object sender, EventArgs e) => logic.Env.exec("View.vsSBE.Panel");
        private void menuItemCompNew_Click(object sender, EventArgs e) => btnCompNew_Click(sender, e);
        private void checkedListBoxSpecCfg_MouseLeave(object sender, EventArgs e) => checkedListBoxSpecCfg.Width = checkedListBoxSpecCfg.MinimumSize.Width;
        private void toolStripMenuApply_Click(object sender, EventArgs e) => btnApply_Click(sender, e);
        private void toolStripMenuMSBuildProp_Click(object sender, EventArgs e) => envVariablesUIHelper();
        private void toolStripMenuDTECmd_Click(object sender, EventArgs e) => btnDteCmd_Click(sender, e);
        private void splitContainer_SplitterMoved(object sender, SplitterEventArgs e) => pictureBoxToggle.Visible = !(e.SplitX < metric.splitter);
        private void menuActionsAdd_Click(object sender, EventArgs e) => addAction();
        private void menuActionsClone_Click(object sender, EventArgs e) => addAction((dgvActions.Rows.Count < 1) ? -1 : currentActionIndex());
        private void menuActionsTogglePanel_Click(object sender, EventArgs e) => expandActionsList(splitContainer.SplitterDistance < metric.splitter);
        private void menuActionsReset_Click(object sender, EventArgs e) => toolStripMenuReset_Click(sender, e);
        private void pictureBoxToggle_Click(object sender, EventArgs e) => expandActionsList(false);
        private void dgvActions_DragDropSortedRow(object sender, DataGridViewExt.MovingRowArgs e) => logic.moveEventItem(e.Data.from, e.Data.to);
        private void dgvActions_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e) => linkAddAction.Visible = false;
        private void dgvCEFilters_CellClick(object sender, DataGridViewCellEventArgs e) => removeRow(dgvCEFilters, dgvCEFiltersColumnRemove, e);
        private void checkBoxCESniffer_CheckedChanged(object sender, EventArgs e) => snifferEnabled(checkBoxCESniffer.Checked);
        private void menuSnifferAdd_Click(object sender, EventArgs e) => addFilterFromSniffer(dgvCESniffer, dgvCEFilters);
        private void menuSnifferFlush_Click(object sender, EventArgs e) => dgvCESniffer.Rows.Clear();
        private void menuSnifferActivateCE_Click(object sender, EventArgs e) => switchEvent(SolutionEventType.CommandEvent);
        private void contextMenuSniffer_Opening(object sender, System.ComponentModel.CancelEventArgs e) => menuSnifferAdd.Enabled = dgvCEFilters.Enabled;
        private void btnActivateCSharp_Click(object sender, EventArgs e) => radioModeCSharp.Checked = true;
        private void btnActionExec_Click(object sender, EventArgs e) => menuActionExec_Click(sender, e);
        private void menuActionExec_Click(object sender, EventArgs e) => logic.execAction();
        private void toolStripMenuVersion_Click(object sender, EventArgs e) => toolStripMenuAbout_Click(sender, e);
        #endregion

        #region urls
        private void toolStripMenuGalleryPage_Click(object sender, EventArgs e) => Util.openUrl("https://marketplace.visualstudio.com/items?itemName=GitHub3F.vsSolutionBuildEvent-11615");
        private void toolStripMenuChangelog_Click(object sender, EventArgs e) => Util.openUrl("https://3F.github.io/web.vsSBE/Changelist/#vsix");
        private void toolStripMenuWiki_Click(object sender, EventArgs e) => Util.openUrl("https://3F.github.io/web.vsSBE/");
        private void toolStripMenuSources_Click(object sender, EventArgs e) => Util.openUrl("https://github.com/3F/vsSolutionBuildEvent");
        private void toolStripMenuSysDir_Click(object sender, EventArgs e) => Util.openUrl($"\"{App.CommonPath}\"");
        private void toolStripMenuPluginDir_Click(object sender, EventArgs e) => Util.openUrl($"\"{App.LibPath}\"");
        private void toolStripMenuAPI_Click(object sender, EventArgs e) => Util.openUrl("https://3F.github.io/web.vsSBE/doc/API/");
        private void btnDownloadVSCE_Click(object sender, EventArgs e) => Util.openUrl("https://marketplace.visualstudio.com/items?itemName=GitHub3F.vsCommandEvent");
        private void componentInfo(string name) => Util.openUrl($"https://3F.github.io/web.vsSBE/doc/Scripts/SBE-Scripts/Components/{name}/");
        private void btnCompNew_Click(object sender, EventArgs e) => Util.openUrl("https://3F.github.io/web.vsSBE/doc/Dev/New%20Component/");
        #endregion
    }
}