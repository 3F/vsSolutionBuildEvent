/*
 * Copyright (c) 2013-2015  Denis Kuzmin (reg) <entry.reg@gmail.com>
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using net.r_eg.vsSBE.MSBuild;
using net.r_eg.vsSBE.SBEScripts;
using net.r_eg.vsSBE.SBEScripts.Components;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.Scripts;
using net.r_eg.vsSBE.UI.WForms.Controls;
using AvalonEditWPF = ICSharpCode.AvalonEdit.TextEditor;

namespace net.r_eg.vsSBE.UI.WForms
{
    public partial class ScriptCheckFrm: Form
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
            public IUserVariable uvariable = new UserVariable();

            /// <summary>
            /// Work with MSBuild
            /// </summary>
            public IMSBuild msbuild;

            /// <summary>
            /// Work with SBE-Scripts
            /// </summary>
            public ISBEScript script;

            /// <summary>
            /// Loader of the IComponent's
            /// </summary>
            public IBootloader bootloader;

            /// <summary>
            /// Mapper of the available components
            /// </summary>
            public IInspector inspector;

            public ToolContext(IEnvironment env)
            {
                Log.Trace("Initialization of the clean context for testing.");

                bootloader = new Bootloader(env, uvariable);
                bootloader.register();

                inspector   = new Inspector(bootloader);
                script      = new Script(bootloader);
                msbuild     = new MSBuild.Parser(env, uvariable);
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
            string exDate   = DateTime.Now.AddDays((new Random()).Next(-30, -2)).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern);
            _editor.Text    = Resource.StringScriptExampleSBE.Replace("%mdate%", exDate);
            textEditor.colorize(TextEditor.ColorSchema.SBEScripts);
            textEditor.codeCompletionInit(context.inspector, context.msbuild);
        }

        protected void fillComponents()
        {
            chkListComponents.Items.Clear();
            foreach(IComponent c in context.bootloader.Registered)
            {
                Type type = c.GetType();
                if(!Inspector.isComponent(type)) {
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
                IComponent found = context.bootloader.Registered.Where(c => c.GetType().Name == name).FirstOrDefault();
                if(found != null) {
                    found.Enabled = (index == i)? newValue : chkListComponents.GetItemChecked(i);
                }
            }
        }

        protected void updateVariableList()
        {
            listBoxUVariables.Items.Clear();
            richTextBoxUVariables.Text = String.Empty;

            foreach(string var in context.uvariable.Definitions) {
                listBoxUVariables.Items.Add(var);
            }
        }

        protected string getVariable(string ident)
        {
            try {
                evaluateVariable(ident);
                return context.uvariable.get(ident);
            }
            catch(Exception ex) {
                return String.Format("Fail: {0}", ex.Message);
            }
        }

        protected void evaluateVariable(string ident)
        {
            if(!context.uvariable.isUnevaluated(ident)) {
                return;
            }

            context.uvariable.evaluate(ident, (IEvaluator)context.script, true);
            if(MSBuildSupport) {
                context.uvariable.evaluate(ident, (IEvaluator)context.msbuild, false);
            }
        }

        protected string execute(string data)
        {
            try {
                string ret;
                if(MSBuildSupport) {
                    ret = context.msbuild.parse(context.script.parse(data, true));
                }
                else {
                    ret = context.script.parse(data);
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
            context.uvariable.unset(listBoxUVariables.Text);
            listBoxUVariables.Items.RemoveAt(listBoxUVariables.SelectedIndex);
        }

        private void menuItemUVarUnsetAll_Click(object sender, EventArgs e)
        {
            context.uvariable.unsetAll();
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
            Util.openUrl("http://vssbe.r-eg.net/doc/Scripts/SBE-Scripts/");
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
            ((IUserVariableDebug)context.uvariable).debSetEvaluated(listBoxUVariables.Text, richTextBoxUVariables.Text);
        }

        private void listBoxUVariables_DoubleClick(object sender, EventArgs e)
        {
            mItemUVarEdit_Click(sender, e);
        }
    }
}
