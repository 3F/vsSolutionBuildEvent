/*!
 * Copyright (c) 2013  Denis Kuzmin <x-3F@outlook.com> github/3F
 * Copyright (c) vsSolutionBuildEvent contributors https://github.com/3F/vsSolutionBuildEvent/graphs/contributors
 * Licensed under the LGPLv3.
 * See accompanying LICENSE file or visit https://github.com/3F/vsSolutionBuildEvent
*/

using System;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using net.r_eg.EvMSBuild;
using net.r_eg.SobaScript;
using net.r_eg.SobaScript.Components;
using net.r_eg.SobaScript.Mapper;
using net.r_eg.Varhead;
using net.r_eg.vsSBE.UI.WForms.Controls;

namespace net.r_eg.vsSBE.UI.WForms
{
    using AvalonEditWPF = ICSharpCode.AvalonEdit.TextEditor;

    internal partial class ScriptCheckFrm: Form
    {
        /// <summary>
        /// Flag of supporting MSBuild
        /// </summary>
        protected bool MSBuildSupport
        {
            get { return checkBoxMSBuildSupport.Checked; }
        }

        /// <summary>
        /// Clean context for testing
        /// </summary>
        protected sealed class ToolContext
        {
            /// <summary>
            /// Container of user-variables
            /// </summary>
            public IUVars uvars = new UVars();

            /// <summary>
            /// Work with MSBuild
            /// </summary>
            public IEvMSBuild msbuild;

            /// <summary>
            /// Work with SBE-Scripts
            /// </summary>
            public ISobaScript script;

            /// <summary>
            /// Loader of the IComponent's
            /// </summary>
            public ISobaCLoader cloader;

            /// <summary>
            /// Mapper of the available components
            /// </summary>
            public IInspector inspector;

            public ToolContext(IEnvironment env)
            {
                Log.Trace("Initialization of the clean context for testing.");

                var soba = new Soba(MSBuild.MakeEvaluator(env, uvars), uvars);
                Bootloader._.Configure(soba);

                cloader     = soba;
                inspector   = new Inspector(soba);
                script      = soba;
                msbuild     = soba.EvMSBuild;
            }
        }
        private static ToolContext context;

        /// <summary>
        /// Alias to AvalonEdit
        /// </summary>
        private AvalonEditWPF _editor
        {
            get { return textEditor._; }
        }


        public ScriptCheckFrm(IEnvironment env)
        {
            InitializeComponent();
            Icon = Resource.Package_32;

            if(context == null) {
                context = new ToolContext(env);
            }
            else {
                updateVariableList();
            }

            initEditor();
            fillComponents();
        }

        protected void initEditor()
        {
            // sortable format for InvariantCulture
            string exDate   = DateTime.Now.AddDays((new Random()).Next(-30, -2)).ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            _editor.Text    = Resource.StringScriptExampleSBE.Replace("%mdate%", exDate);

            textEditor.colorize(TextEditor.ColorSchema.SBEScripts);
            textEditor.codeCompletionInit(context.inspector, context.msbuild);
        }

        protected void fillComponents()
        {
            chkListComponents.Items.Clear();
            foreach(IComponent c in context.cloader.Registered)
            {
                Type type = c.GetType();
                if(!Inspector.IsComponent(type)) {
                    continue;
                }
                chkListComponents.Items.Add(type.Name, c.Enabled);
            }
        }

        protected void updateComponents(int index = -1, bool newValue = true)
        {
            for(int i = 0; i < chkListComponents.Items.Count; ++i)
            {
                string name = chkListComponents.Items[i].ToString();
                IComponent found = context.cloader.Registered.Where(c => c.GetType().Name == name).FirstOrDefault();
                if(found != null) {
                    found.Enabled = (index == i)? newValue : chkListComponents.GetItemChecked(i);
                }
            }
        }

        protected void updateVariableList()
        {
            listBoxUVariables.Items.Clear();
            richTextBoxUVariables.Text = String.Empty;

            foreach(string var in context.uvars.Definitions) {
                listBoxUVariables.Items.Add(var);
            }
        }

        protected string getVariable(string ident)
        {
            try {
                evaluateVariable(ident);
                return context.uvars.GetValue(ident);
            }
            catch(Exception ex) {
                return String.Format("Fail: {0}", ex.Message);
            }
        }

        protected void evaluateVariable(string ident)
        {
            if(!context.uvars.IsUnevaluated(ident)) {
                return;
            }

            context.uvars.Evaluate(ident, (IEvaluator)context.script, true);
            if(MSBuildSupport) {
                context.uvars.Evaluate(ident, (IEvaluator)context.msbuild, false);
            }
        }

        protected string execute(string data)
        {
            try {
                string ret;
                if(MSBuildSupport) {
                    ret = context.msbuild.Eval(context.script.Eval(data, true));
                }
                else {
                    ret = context.script.Eval(data);
                }
                return ret;
            }
            catch(Exception ex) {
                if(chkStackTrace.Checked) {
                    return String.Format("Fail: `{0}`\n\n{1}\n{2}", ex.Message, new String('-', 15), ex.StackTrace);
                }
                return String.Format("Fail: {0}", ex.Message);
            }
            finally {
                updateVariableList();
            }
        }

        private void setClipboardText(string text)
        {
            if(!String.IsNullOrWhiteSpace(text)) {
                Clipboard.SetText(text);
            }
        }

        private void _lockUVarEditor(RichTextBox editor, bool disabled)
        {
            if(disabled) {
                editor.ReadOnly     = true;
                editor.BackColor    = System.Drawing.SystemColors.Control;
                return;
            }
            editor.BackColor = System.Drawing.SystemColors.Window;
            editor.Focus();
            editor.ReadOnly = false;
        }

        private void btnExecute_Click(object sender, EventArgs e)
        {
            richTextBoxExecuted.Text = execute(textEditor.Text);
        }

        private void menuItemUVarUnsetSel_Click(object sender, EventArgs e)
        {
            richTextBoxUVariables.Text = String.Empty;
            _lockUVarEditor(richTextBoxUVariables, true);

            if(listBoxUVariables.SelectedIndex == -1) {
                return;
            }
            context.uvars.Unset(listBoxUVariables.Text);
            listBoxUVariables.Items.RemoveAt(listBoxUVariables.SelectedIndex);
        }

        private void menuItemUVarUnsetAll_Click(object sender, EventArgs e)
        {
            context.uvars.UnsetAll();
            listBoxUVariables.Items.Clear();
            _lockUVarEditor(richTextBoxUVariables, true);
            richTextBoxUVariables.Text = String.Empty;
        }

        private void listBoxUVariables_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxUVariables.SelectedIndex == -1) {
                return;
            }
            richTextBoxUVariables.Text = getVariable(listBoxUVariables.Text);
        }

        private void btnDoc_Click(object sender, EventArgs e)
        {
            Util.openUrl("https://3F.github.io/web.vsSBE/doc/Scripts/SBE-Scripts/");
        }

        private void chkListComponents_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            updateComponents(e.Index, e.NewValue == CheckState.Checked);
        }

        private void mItemUVarEdit_Click(object sender, EventArgs e)
        {
            if(listBoxUVariables.SelectedIndex == -1) {
                return;
            }
            _lockUVarEditor(richTextBoxUVariables, false);
        }

        private void richTextBoxUVariables_Leave(object sender, EventArgs e)
        {
            if(listBoxUVariables.SelectedIndex == -1) {
                return;
            }
            _lockUVarEditor(richTextBoxUVariables, true);
            ((IUVarsExt)context.uvars).SetEvaluated(listBoxUVariables.Text, richTextBoxUVariables.Text);
        }

        private void listBoxUVariables_DoubleClick(object sender, EventArgs e)
        {
            mItemUVarEdit_Click(sender, e);
        }

        private void mItemClear_Click(object sender, EventArgs e)
        {
            richTextBoxExecuted.Clear();
        }

        private void mItemCopySel_Click(object sender, EventArgs e)
        {
            setClipboardText(richTextBoxExecuted.SelectedText);
        }

        private void mItemCopyAll_Click(object sender, EventArgs e)
        {
            setClipboardText(richTextBoxExecuted.Text);
        }
    }
}
