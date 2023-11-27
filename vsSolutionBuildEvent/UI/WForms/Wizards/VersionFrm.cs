/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using net.r_eg.MvsSln.Extensions;
using net.r_eg.SobaScript.Exceptions;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.vsSBE.Events;
using net.r_eg.vsSBE.Extensions;
using net.r_eg.vsSBE.UI.WForms.Controls;
using net.r_eg.vsSBE.UI.WForms.Wizards.Version;

namespace net.r_eg.vsSBE.UI.WForms.Wizards
{
    internal partial class VersionFrm: Form
    {
        private readonly Manager manager;

        private readonly ITransfer _pin;

        /// <param name="loader"></param>
        /// <param name="pin"></param>
        public VersionFrm(Bootloader loader, ITransfer pin)
        {
            manager = new Manager(loader.Env);
            _pin    = pin;

            InitializeComponent();
            Icon = Resource.Package_32;

            editorStepGen._.WordWrap        = false;
            editorStepGen._.IsReadOnly      = true;
            editorStepGen._.FontSize        = 9.25f;
            editorStepGen.setBackgroundFromString("#F4F4F4");

            editorFinalScript.colorize(TextEditor.ColorSchema.SBEScripts);
            editorFinalScript.codeCompletionInit(new Inspector(loader.Soba), loader.Soba.EvMSBuild);
            editorFinalScript.CodeCompletionEnabled = true;
            editorFinalScript._.WordWrap            = false;

            int hidepx = this.GetValueUsingDpi(22);
            tabControlMain.Top      = -hidepx;
            tabControlMain.Height   += hidepx;

            tcRevNumber.SizeMode    = TabSizeMode.Fixed;
            tcReplType.SizeMode     = TabSizeMode.Fixed;
            btnPrevStep.Visible     = false;

            string spath = loader.Env.SolutionPath ?? Settings.WPath;

            ftbInputNum.Dialog.InitialDirectory = ftbOutputFile.Dialog.InitialDirectory
                                                = ftbReplFile.Dialog.InitialDirectory
                                                = spath.DirectoryPathFormat();
        }

        private void render(StepsType type)
        {
            switch(type)
            {
                case StepsType.Gen: {
                    render(manager.StepGen);
                    return;
                }
                case StepsType.Struct: {
                    render(manager.StepStruct);
                    return;
                }
                case StepsType.DirectRepl: {
                    render(manager.StepRepl);
                    return;
                }
                case StepsType.CfgData: {
                    render(manager.StepCfgData);
                    return;
                }
                case StepsType.Fields: {
                    render(manager.StepFields);
                    return;
                }
                case StepsType.Final: {
                    render(manager.StepFinal);
                    return;
                }
            }
        }

        private void render(StepGen s)
        {
            switch(s.gtype)
            {
                case GenType.CSharpStruct: {
                    radioGenCSharpStruct.Checked = true;
                    return;
                }
                case GenType.CppStruct: {
                    radioGenCppStruct.Checked = true;
                    return;
                }
                case GenType.CppDefinitions: {
                    radioGenCppDefine.Checked = true;
                    return;
                }
                case GenType.Direct: {
                    radioGenDirect.Checked = true;
                    return;
                }
            }
        }

        private void render(StepStruct s)
        {
            tbNamespace.Text        = s.namspace;
            tbClassName.Text        = s.name;
            chkUpperCase.Checked    = s.upperCase;

            cbNumberType.Items.Clear();
            if(manager.StepGen.gtype == GenType.CSharpStruct) {
                cbNumberType.Enabled = false; // support only System.Version
                s.fnumber = StepStruct.NumberType.SystemVersion;
            }
            else {
                cbNumberType.Enabled = true;
                if(manager.StepGen.gtype == GenType.CppStruct) {
                    s.fnumber = StepStruct.NumberType.NativeStruct; // to start with native
                }
            }

            cbNumberType.Items.AddRange(s.NumbersList.Select(i => i.Value).ToArray());
            cbNumberType.SelectedIndex = s.NumbersList.FindIndex(i => i.Key == s.fnumber);
        }

        private void render(StepRepl s)
        {
            ftbReplFile.FileName    = s.file;
            tbReplPattern.Text      = s.pattern;
            tbReplPrefix.Text       = s.prefix;
            tbReplPostfix.Text      = s.postfix;

            cbReplType.Items.Clear();
            cbReplType.Items.AddRange(s.TypeList.Select(i => i.Value).ToArray());
            cbReplType.SelectedIndex = s.TypeList.FindIndex(i => i.Key == s.rtype);

            cbReplSource.Items.Clear();
            cbReplSource.Items.AddRange(s.SourceList.Select(i => i.Value).ToArray());
            cbReplSource.SelectedIndex = s.SourceList.FindIndex(i => i.Key == s.source);
        }

        private void render(StepCfgData s)
        {
            ftbInputNum.FileName = s.inputNumber;

            if(manager.StepGen.gtype == GenType.Direct) {
                ftbOutputFile.Enabled   = false;
                ftbOutputFile.FileName  = manager.StepRepl.file;
            }
            else {
                ftbOutputFile.Enabled   = true;
                ftbOutputFile.FileName  = s.output;
            }

            // Type of input number:

            cbInputNum.Items.Clear();
            cbInputNum.Items.AddRange(s.InputNumberTypeList.Select(i => i.Value).ToArray());
            if(manager.StepGen.gtype == GenType.Direct && manager.StepRepl.IsSourceNotRequiresInputNum) {
                cbInputNum.SelectedIndex = s.InputNumberTypeList.FindIndex(i => i.Key == StepCfgData.InputNumberType.MSBuildProp);
            }
            else {
                cbInputNum.SelectedIndex = s.InputNumberTypeList.FindIndex(i => i.Key == s.inputNumberType);
            }

            // Use SCM data:

            cbSCM.Items.Clear();
            cbSCM.Items.AddRange(s.SCMTypeList.Select(i => i.Value).ToArray());

            if(manager.StepGen.gtype == GenType.Direct) {
                if(manager.StepRepl.IsSourceSCM) {
                    cbSCM.Enabled = true;
                    cbSCM.SelectedIndex = s.SCMTypeList.FindIndex(i => i.Key != StepCfgData.SCMType.None);
                }
                else {
                    cbSCM.Enabled = false;
                }
            }
            else {
                cbSCM.SelectedIndex = s.SCMTypeList.FindIndex(i => i.Key == s.scm);
                cbSCM.Enabled = true;
            }

            // Type of revision number:

            cbTypeRev.Items.Clear();
            cbTypeRev.Items.AddRange(s.RevTypeList.Select(i => i.Value).ToArray());
            cbTypeRev.SelectedIndex = s.RevTypeList.FindIndex(i => i.Key == s.revType);

            render(s, s.RevTypeList[cbTypeRev.SelectedIndex].Key);
        }

        private void render(StepCfgData s, Version.RevNumber.Type type)
        {
            if(type == Version.RevNumber.Type.DeltaTime)
            {
                var rev = (Version.RevNumber.DeltaTime)s.revVal;

                cbRevTimeType.Items.Clear();
                cbRevTimeType.Items.AddRange(rev.IntervalTypeList.Select(i => i.Value).ToArray());
                cbRevTimeType.SelectedIndex = rev.IntervalTypeList.FindIndex(i => i.Key == rev.interval);

                dtRevTimeBase.Value = rev.timeBase;

                chkRevTimeMod.Checked   = rev.revMod.enabled;
                numRevTimeMin.Value     = rev.revMod.min;
                numRevTimeMax.Value     = rev.revMod.max;
            }
        }

        private void render(StepFields s)
        {
            var cfg = manager.StepCfgData;

            dgvFields.Rows.Clear();
            foreach(StepFields.Items i in s.items)
            {
                string origin;
                if(manager.StepGen.gtype == GenType.CppDefinitions) {
                    origin = i.originConst;
                }
                else{
                    origin = (manager.StepStruct.upperCase)? i.originUpperCase : i.origin;
                }
                int idx = dgvFields.Rows.Add(!i.disabled, origin, i.newname, i.description);

                if(!s.isAllow(i.type, cfg.scm) 
                    || !s.isAllow(i.type, cfg.revType) 
                    || !s.isAllow(i.type, manager.StepGen.gtype))
                {
                    dgvFields.Rows[idx].ReadOnly = true;
                    dgvFields.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                    dgvFields.Rows[idx].Cells[0] = new DataGridViewCheckBoxCell() {
                                                                                    Style = {
                                                                                        ForeColor = Color.Transparent, 
                                                                                        SelectionForeColor = Color.Transparent
                                                                                    }};
                }
            }
        }

        private void render(StepFinal s)
        {
            editorFinalScript._.Select(0, 0);
            try {
                editorFinalScript.Text = s.construct();
            }
            catch(Exception ex) {
                Log.Error($"Failed when generating the user script '{Text}': {ex.Message}");
                Log.Debug(ex.StackTrace);
            }
        }

        private void save(StepsType type)
        {
            switch(type)
            {
                case StepsType.Gen: {
                    save(manager.StepGen);
                    return;
                }
                case StepsType.Struct: {
                    save(manager.StepStruct);
                    return;
                }
                case StepsType.DirectRepl: {
                    save(manager.StepRepl);
                    return;
                }
                case StepsType.CfgData: {
                    save(manager.StepCfgData);
                    return;
                }
                case StepsType.Fields: {
                    save(manager.StepFields);
                    return;
                }
            }
        }

        private void save(StepGen s)
        {
            if(radioGenCSharpStruct.Checked) {
                s.gtype = GenType.CSharpStruct;
            }
            else if(radioGenCppStruct.Checked) {
                s.gtype = GenType.CppStruct;
            }
            else if(radioGenCppDefine.Checked) {
                s.gtype = GenType.CppDefinitions;
            }
            else if(radioGenDirect.Checked) {
                s.gtype = GenType.Direct;
            }
        }

        private void save(StepStruct s)
        {
            s.namspace  = tbNamespace.Text;
            s.name      = tbClassName.Text;
            s.upperCase = chkUpperCase.Checked;
            s.fnumber   = s.NumbersList[cbNumberType.SelectedIndex].Key;
        }

        private void save(StepRepl s)
        {
            s.file      = ftbReplFile.FileName;
            s.pattern   = tbReplPattern.Text;
            s.prefix    = tbReplPrefix.Text;
            s.postfix   = tbReplPostfix.Text;
            s.rtype     = s.TypeList[cbReplType.SelectedIndex].Key;
            s.source    = s.SourceList[cbReplSource.SelectedIndex].Key;
        }

        private void save(StepCfgData s)
        {
            s.inputNumber       = ftbInputNum.FileName;
            s.output            = ftbOutputFile.FileName;
            s.inputNumberType   = s.InputNumberTypeList[cbInputNum.SelectedIndex].Key;
            s.revType           = s.RevTypeList[cbTypeRev.SelectedIndex].Key;

            s.scm = (cbSCM.SelectedIndex != -1)? s.SCMTypeList[cbSCM.SelectedIndex].Key : StepCfgData.SCMType.None;

            save(s, s.RevTypeList[cbTypeRev.SelectedIndex].Key);
        }

        private void save(StepCfgData s, Version.RevNumber.Type type)
        {
            if(type == Version.RevNumber.Type.DeltaTime)
            {
                var rev = (Version.RevNumber.DeltaTime)s.revVal;

                rev.interval = rev.IntervalTypeList[cbRevTimeType.SelectedIndex].Key;
                rev.timeBase = dtRevTimeBase.Value;

                rev.revMod.enabled  = chkRevTimeMod.Checked;
                rev.revMod.min      = (int)numRevTimeMin.Value;
                rev.revMod.max      = (int)numRevTimeMax.Value;
            }
        }

        private void save(StepFields s)
        {
            foreach(DataGridViewRow row in dgvFields.Rows) {
                s.items[row.Index].disabled = !Convert.ToBoolean(row.Cells[dgvFieldsEnabled.Name].Value);
                s.items[row.Index].newname  = (string)row.Cells[dgvFieldsNameNew.Name].Value;
            }
        }

        private StepsType getStepTypeBy(TabPage page)
        {
            if(page == tabPageGen) {
                return StepsType.Gen;
            }
            if(page == tabPageStruct) {
                return StepsType.Struct;
            }
            if(page == tabPageRepl) {
                return StepsType.DirectRepl;
            }
            if(page == tabPageCfgData) {
                return StepsType.CfgData;
            }
            if(page == tabPageFields) {
                return StepsType.Fields;
            }
            if(page == tabPageFinal) {
                return StepsType.Final;
            }
            throw new NotFoundException(page.Name);
        }

        private void nextStep()
        {
            StepsType type = getStepTypeBy(tabControlMain.SelectedTab);
            nextStep(type);
        }

        private void nextStep(StepsType type, bool saving = true)
        {
            if(saving) {
                save(type);
            }

            if(type == StepsType.Gen)
            {
                btnPrevStep.Visible = true;
                if(radioGenDirect.Checked) {
                    tabControlMain.SelectedTab = tabPageRepl;
                    render(StepsType.DirectRepl);
                }
                else if(radioGenCppDefine.Checked) {
                    tabControlMain.SelectedTab = tabPageCfgData;
                    render(StepsType.CfgData);
                }
                else {
                    tabControlMain.SelectedTab = tabPageStruct;
                    render(StepsType.Struct);
                }
            }
            else if(type == StepsType.Struct || type == StepsType.DirectRepl)
            {
                tabControlMain.SelectedTab = tabPageCfgData;
                render(StepsType.CfgData);
            }
            else if(type == StepsType.CfgData)
            {
                if(manager.StepGen.gtype == GenType.Direct) {
                    nextStep(StepsType.Fields, false);
                    return;
                }
                tabControlMain.SelectedTab = tabPageFields;
                render(StepsType.Fields);
            }
            else if(type == StepsType.Fields)
            {
                render(StepsType.Final);
                tabControlMain.SelectedTab  = tabPageFinal;
                btnNextStep.Visible         = false;
            }
        }

        private void prevStep()
        {
            StepsType type = getStepTypeBy(tabControlMain.SelectedTab);
            prevStep(type);
        }

        private void prevStep(StepsType type)
        {
            if(type == StepsType.Struct || type == StepsType.DirectRepl)
            {
                tabControlMain.SelectedTab  = tabPageGen;
                btnPrevStep.Visible         = false;
            }
            else if(type == StepsType.CfgData)
            {
                if(radioGenDirect.Checked) {
                    tabControlMain.SelectedTab = tabPageRepl;
                }
                else if(radioGenCppDefine.Checked) {
                    tabControlMain.SelectedTab = tabPageGen;
                }
                else {
                    tabControlMain.SelectedTab = tabPageStruct;
                }
            }
            else if(type == StepsType.Fields)
            {
                tabControlMain.SelectedTab = tabPageCfgData;
            }
            else if(type == StepsType.Final)
            {
                btnNextStep.Visible = true;
                if(manager.StepGen.gtype == GenType.Direct) {
                    prevStep(StepsType.Fields);
                    return;
                }
                tabControlMain.SelectedTab = tabPageFields;
            }
        }

        private void restoreFields()
        {
            //TODO:
            manager.StepFields.resetDisabled();
        }

        private void VersionFrm_Load(object sender, EventArgs e)
        {
            render(StepsType.Gen);
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            nextStep();
        }

        private void btnPrevStep_Click(object sender, EventArgs e)
        {
            prevStep();
        }

        private void cbInputNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            var s = manager.StepCfgData;
            if(s.InputNumberTypeList[cbInputNum.SelectedIndex].Key == StepCfgData.InputNumberType.File) {
                ftbInputNum.HideButton = false;
            }
            else {
                ftbInputNum.HideButton = true;
            }

            ftbInputNum.FileName = String.Empty;
        }

        private void cbTypeRev_SelectedIndexChanged(object sender, EventArgs e)
        {
            var s       = manager.StepCfgData;
            var type    = s.RevTypeList[cbTypeRev.SelectedIndex].Key;

            if(type == Version.RevNumber.Type.DeltaTime)
            {
                tcRevNumber.SelectedTab = tpRevDeltaTime;

                var rev     = new Version.RevNumber.DeltaTime();
                s.revVal    = rev;
                s.revType   = Version.RevNumber.Type.DeltaTime;
            }
            else if(type == Version.RevNumber.Type.Raw)
            {
                tcRevNumber.SelectedTab = tpRevRaw;

                s.revVal    = new Version.RevNumber.Raw();
                s.revType   = Version.RevNumber.Type.Raw;
            }

            render(s, s.RevTypeList[cbTypeRev.SelectedIndex].Key);
            restoreFields();
        }

        private void cbSCM_SelectedIndexChanged(object sender, EventArgs e)
        {
            restoreFields();

            if(manager.StepGen.gtype == GenType.Direct
                && manager.StepRepl.IsSourceSCM
                && manager.StepCfgData.SCMTypeList[cbSCM.SelectedIndex].Key == StepCfgData.SCMType.None)
            {
                prevStep();
            }
        }

        private void radioGenCSharpStruct_CheckedChanged(object sender, EventArgs e)
        {
            editorStepGen.colorize(TextEditor.ColorSchema.CSharpLang);
            editorStepGen.Text = Resource.WizardVerCSharpStruct;
        }

        private void radioGenCppStruct_CheckedChanged(object sender, EventArgs e)
        {
            editorStepGen.colorize(TextEditor.ColorSchema.CppLang);
            editorStepGen.Text = Resource.WizardVerCppStruct;
        }

        private void radioGenCppDefine_CheckedChanged(object sender, EventArgs e)
        {
            editorStepGen.colorize(TextEditor.ColorSchema.CppLang);
            editorStepGen.Text = Resource.WizardVerCppDefine;
        }

        private void radioGenDirect_CheckedChanged(object sender, EventArgs e)
        {
            editorStepGen.colorize(TextEditor.ColorSchema.MSBuildTargets);
            editorStepGen.Text = Resource.WizardVerDirectRepl;
        }

        private void cbReplType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(manager.StepRepl.TypeList[cbReplType.SelectedIndex].Key) {
                case StepRepl.ReplType.Regex: {
                    tcReplType.SelectedTab = tpReplRegex;
                    return;
                }
                case StepRepl.ReplType.Wildcards: {
                    tcReplType.SelectedTab = tpReplWildcards;
                    return;
                }
            }
        }

        private void cbReplSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(manager.StepRepl.SourceList[cbReplSource.SelectedIndex].Key) {
                case Fields.Type.BranchName: {
                    labelExampleSource.Text = "develop";
                    return;
                }
                case Fields.Type.BranchRevCount: {
                    labelExampleSource.Text = "296";
                    return;
                }
                case Fields.Type.BranchSha1: {
                    labelExampleSource.Text = "e3de826";
                    return;
                }
                case Fields.Type.Informational: {
                    labelExampleSource.Text = "0.12.4.17639 [ e3de826 ]";
                    return;
                }
                case Fields.Type.InformationalFull: {
                    labelExampleSource.Text = "0.12.4.17639 [ e3de826 ] /'develop':296";
                    return;
                }
                case Fields.Type.Null: {
                    labelExampleSource.Text = "";
                    return;
                }
                case Fields.Type.NumberString: {
                    labelExampleSource.Text = "0.12.4";
                    return;
                }
                case Fields.Type.NumberWithRevString: {
                    labelExampleSource.Text = "0.12.4.17639";
                    return;
                }
            }
            labelExampleSource.Text = String.Empty;
        }

        private void linkRegex_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Util.openUrl("https://msdn.microsoft.com/en-us/library/az24scfc.aspx");
        }

        private void btnManually_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://3F.github.io/web.vsSBE/doc/Examples/Version/Manually/");
        }

        private void btnFinalCopy_Click(object sender, EventArgs e)
        {
            editorFinalScript._.SelectAll();
            editorFinalScript._.Copy();
        }

        private void btnFinalCreateAction_Click(object sender, EventArgs e)
        {
            _pin.action(SolutionEventType.Pre, new SBEEvent()
            {
                Mode = new ModeScript() {
                    Command = editorFinalScript.Text
                },
                SupportMSBuild      = true,
                SupportSBEScripts   = true,
                Enabled             = true,
                Caption             = "Final script from 'Automatic Version Numbering'",
            });
            Close();
        }
    }
}
