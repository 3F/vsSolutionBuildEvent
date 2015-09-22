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
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
using net.r_eg.vsSBE.Configuration.User;
using net.r_eg.vsSBE.SBEScripts.Dom;
using net.r_eg.vsSBE.UI.WForms.Controls.TextEditorElements;
using AvalonEditorWPF = ICSharpCode.AvalonEdit.TextEditor;
using InputModifierKeys = System.Windows.Input.ModifierKeys;

namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    /// <summary>
    /// Wrapper of WPF AvalonEdit for use with the System.Windows.Forms
    /// </summary>
    public partial class TextEditor: UserControl
    {
        public enum ColorSchema
        {
            Default,
            FilesMode,
            OperationMode,
            InterpreterMode,
            ScriptMode,            
            SBEScript,
            MSBuildTargets,
            CSharpLang,
        }

        /// <summary>
        /// Accessor to AvalonEdit
        /// </summary>
        public AvalonEditorWPF _
        {
            get { return editor; }
        }
        protected AvalonEditorWPF editor;

        /// <summary>
        /// Gets/Sets the text of the current document
        /// </summary>
        [Browsable(true)]
        [Category("AvalonEdit"), Description("Gets/Sets the text of the current document")]
        [EditorAttribute(typeof(MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public override string Text
        {
            get { return editor.Text; }
            set { editor.Text = value; }
        }

        /// <summary>
        /// Enabling the code completion
        /// </summary>
        [Browsable(true)]
        [Category("CodeCompletion"), Description("Enabling the code completion\nUse the codeCompletionInit() before")]
        public bool CodeCompletionEnabled
        {
            get { return codeCompletionEnabled; }
            set
            {
                codeCompletionEnabled = value;
                if(value) {
                    attachCodeCompletionEvents();
                    return;
                }
                detachCodeCompletionEvents();
            }
        }
        protected bool codeCompletionEnabled = false;

        /// <summary>
        /// Code completion window
        /// </summary>
        protected CompletionWindow completionWindow;

        /// <summary>
        /// Analyzer of all registered IComponent
        /// </summary>
        protected DomParser dom;

        /// <summary>
        /// Original value of the FontSize.
        /// Used as base for zooming
        /// </summary>
        protected double initFontSize;

        /// <summary>
        /// Standard panel for searching
        /// </summary>
        protected SearchPanel searchPanel;

        /// <summary>
        /// Stores a list of foldings
        /// </summary>
        protected FoldingManager foldingManager;

        /// <summary>
        /// Current schema
        /// </summary>
        protected ColorSchema schema = ColorSchema.Default;

        /// <summary>
        /// Determines folds for an xml string
        /// </summary>
        protected XmlFoldingStrategy xmlFoldingStrategy;

        /// <summary>
        /// Determines folds for braces
        /// </summary>
        protected BraceFoldingStrategy braceFoldingStrategy;

        /// <summary>
        /// Common user configuration.
        /// </summary>
        protected ICommon commonCfg;

        /// <summary>
        /// object synch.
        /// </summary>
        private Object _lock = new Object();

        /// <summary>
        /// Updating model of the data for code completion
        /// </summary>
        /// <param name="inspector"></param>
        public void codeCompletionInit(IInspector inspector)
        {
            dom = new DomParser(inspector);
            Log.Trace("Code completion has been initialized for '{0}'", Name);
        }

        public void colorize(ColorSchema schema)
        {
            this.schema = schema;
            IHighlightingDefinition def = null;
            switch(schema)
            {
                case ColorSchema.ScriptMode:
                case ColorSchema.SBEScript: {
                    def = HighlightingManager.Instance.GetDefinition("SBEScripts");
                    break;
                }
                case ColorSchema.Default:
                case ColorSchema.FilesMode: {
                    def = HighlightingManager.Instance.GetDefinition("FilesMode");
                    break;
                }
                case ColorSchema.OperationMode: {
                    def = HighlightingManager.Instance.GetDefinition("OperationMode");
                    break;
                }
                case ColorSchema.InterpreterMode: {
                    def = HighlightingManager.Instance.GetDefinition("InterpreterMode");
                    break;
                }
                case ColorSchema.MSBuildTargets: {
                    def = HighlightingManager.Instance.GetDefinition("MSBuildTargets");
                    break;
                }
                case ColorSchema.CSharpLang: {
                    def = HighlightingManager.Instance.GetDefinition("CSharpLang");
                    break;
                }
                default: {
                    Log.Debug("colorize: schema '{0}' is invalid", schema);
                    return;
                }
            }
            _.SyntaxHighlighting = def;
            updateFoldings();
        }

        public void setBackgroundFromString(string color)
        {
            _.Background = brushColorFromString(color);
            updateCurrentLineBackground(_.Background);
        }

        public void config(ICommon cfg)
        {
            commonCfg = cfg;

            if(commonCfg.Zoomed <= 0) {
                commonCfg.Zoomed = 100;
            }

            _.WordWrap              = commonCfg.WordWrap;
            menuComboBoxZoom.Text   = String.Format("{0} %", commonCfg.Zoomed);
        }

        public void insertToSelection(string text, bool select = true)
        {
            _.Select(_.SelectionStart, 0);
            _.SelectedText = text;
            if(!select) {
                _.Select(_.SelectionStart + _.SelectionLength, 0);
            }
            Focus();
        }

        public TextEditor()
        {
            InitializeComponent();

            editor = new AvalonEditorWPF();
            attachWpfEditor(editor);

            ContextMenuStrip    = contextMenuEditor;
            foldingManager      = FoldingManager.Install(_.TextArea);
            searchPanel         = SearchPanel.Install(_.TextArea);
            init();
        }

        protected void attachCodeCompletionEvents()
        {
            lock(_lock) {
                detachCodeCompletionEvents();
                _.TextArea.KeyDown      += new System.Windows.Input.KeyEventHandler(editorTextArea_KeyDown);
                _.TextArea.TextEntering += new TextCompositionEventHandler(editorTextArea_TextEntering);
                _.TextArea.TextEntered  += new TextCompositionEventHandler(editorTextArea_TextEntered);
            }
        }

        protected void detachCodeCompletionEvents()
        {
            lock(_lock) {
                _.TextArea.KeyDown      -= new System.Windows.Input.KeyEventHandler(editorTextArea_KeyDown);
                _.TextArea.TextEntering -= new TextCompositionEventHandler(editorTextArea_TextEntering);
                _.TextArea.TextEntered  -= new TextCompositionEventHandler(editorTextArea_TextEntered);
            }
        }

        protected void updateFoldings()
        {
            if(foldingManager == null) {
                Log.Debug("foldingManager is null");
                return;
            }

            switch(schema)
            {
                case ColorSchema.MSBuildTargets:
                {
                    if(xmlFoldingStrategy == null) {
                        xmlFoldingStrategy = new XmlFoldingStrategy();
                    }
                    xmlFoldingStrategy.UpdateFoldings(foldingManager, _.Document);
                    return;
                }
                case ColorSchema.InterpreterMode:
                case ColorSchema.ScriptMode:
                case ColorSchema.SBEScript:
                case ColorSchema.CSharpLang:
                {
                    if(braceFoldingStrategy == null) {
                        braceFoldingStrategy = new BraceFoldingStrategy();
                    }
                    braceFoldingStrategy.UpdateFoldings(foldingManager, _.Document);
                    return;
                }
            }
            foldingManager.Clear();
        }

        protected void showCodeCompletion(DomParser.KeysCommand cmd)
        {
            if(completionWindow != null) {
                return;
            }
            if(dom == null) {
                Log.Debug("Use the codeCompletionInit() for work with Code Completion");
                return;
            }

            IEnumerable<ICompletionData> data = dom.find(_.TextArea.Document.Text, _.TextArea.Caret.Offset, cmd);
            if(data == null) {
                return;
            }

            completionWindow = new CompletionWindow(_.TextArea) { Width = 270 };
            completionWindow.Closed += delegate {
                completionWindow = null;
            };

            foreach(ICompletionData item in data) {
                completionWindow.CompletionList.CompletionData.Add(item);
            }
            completionWindow.Show();
        }

        protected void attachWpfEditor(AvalonEditorWPF editor)
        {
            Controls.Add(new ElementHost()
            {
                Dock    = DockStyle.Fill,
                Child   = editor
            });
        }

        /// <param name="data">xshd content</param>
        /// <param name="name">Schema name</param>
        protected virtual void loadXshdSchema(string data, string name)
        {
            using(XmlReader reader = new XmlTextReader(new StringReader(data))) {
                HighlightingManager.Instance.RegisterHighlighting(name, new string[]{}, HighlightingLoader.Load(reader, HighlightingManager.Instance));
            }
        }

        protected Brush brushColorFromString(string color)
        {
            return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
        }

        protected string zoomedValue(double fontSize)
        {
            return Math.Round((fontSize / Math.Max(1, initFontSize)) * 100) + " %";
        }

        private void init()
        {
            loadXshdSchema(xshd.XshdResource.SBEScripts, "SBEScripts");
            loadXshdSchema(xshd.XshdResource.FilesMode, "FilesMode");
            loadXshdSchema(xshd.XshdResource.OperationMode, "OperationMode");
            loadXshdSchema(xshd.XshdResource.InterpreterMode, "InterpreterMode");
            loadXshdSchema(xshd.XshdResource.MSBuildTargets, "MSBuildTargets");
            loadXshdSchema(xshd.XshdResource.CSharpLang, "CSharpLang");
            
            colorize(ColorSchema.Default);

            _.Options.ConvertTabsToSpaces                       = true;
            _.Options.IndentationSize                           = 4;
            _.Options.ShowTabs                                  = true;
            _.Options.EnableRectangularSelection                = true;
            _.ShowLineNumbers                                   = true;
            _.WordWrap                                          = true;
            _.TextArea.TextView.Options.HighlightCurrentLine    = true;
            _.TextArea.SelectionCornerRadius                    = 0.2f;
            _.TextArea.SelectionBrush                           = brushColorFromString("#E5EBF1");
            _.TextArea.SelectionBorder                          = new Pen(brushColorFromString("#D0DBE6"), 0.3f);               
            _.TextArea.SelectionForeground                      = brushColorFromString("#000000");
            _.FontFamily                                        = new FontFamily("Consolas");
            _.TextArea.TextView.CurrentLineBorder               = new Pen(brushColorFromString("#8E9BBC"), 0.3f);

            updateCurrentLineBackground(_.Background);

            _.TextArea.TextView.BackgroundRenderers.Add(new SimilarWordsRenderer(_));
            _.TextArea.TextView.BackgroundRenderers.Add(new HighlightBracesRenderer(_));
            _.TextArea.Caret.PositionChanged += (sender, e) => { _.TextArea.TextView.InvalidateLayer(KnownLayer.Selection); };

            _.TextChanged               += textEditor_TextChanged;
            menuItemWordWrap.Checked    = _.WordWrap;
            initFontSize                = _.FontSize;
            _.TextArea.MouseWheel       += new System.Windows.Input.MouseWheelEventHandler(textEditor_MouseWheel);
            menuComboBoxZoom.Text       = zoomedValue(_.FontSize);
        }

        private void updateCurrentLineBackground(Brush brush)
        {
            _.TextArea.TextView.CurrentLineBackground = brush;
            _.TextArea.TextView.InvalidateLayer(KnownLayer.Selection);
        }

        private void editorTextArea_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(InputModifierKeys.Control == (Keyboard.Modifiers & InputModifierKeys.Control) && e.Key == Key.Space) {
                e.Handled = true;
                showCodeCompletion(DomParser.KeysCommand.CtrlSpace);
            }
        }

        private void editorTextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if(completionWindow == null) {
                return;
            }

            //if(e.Text.Length == 1 && !char.IsLetterOrDigit(e.Text[0])) {
            if(e.Text.Length == 1 && (e.Text[0] == ' ' || e.Text[0] == '.')) {
                completionWindow.CompletionList.RequestInsertion(e);
            }
        }

        private void editorTextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            DomParser.KeysCommand cmd = DomParser.KeysCommand.Default;
            if(e.Text == "[" && _.TextArea.Document.Text[Math.Max(0, _.TextArea.Caret.Offset - 2)] == '#') {
                cmd = DomParser.KeysCommand.Container;
            }
            else if(e.Text == ".") {
                cmd = DomParser.KeysCommand.LevelByDot;
            }
            else if(e.Text == " ") {
                cmd = DomParser.KeysCommand.Space;
            }

            if(cmd != DomParser.KeysCommand.Default) {
                showCodeCompletion(cmd);
            }
        }

        private void menuComboBoxZoom_TextChanged(object sender, EventArgs e)
        {
            try {
                Match m = Regex.Match(menuComboBoxZoom.Text, @"(\d+)");
                if(!m.Groups[1].Success) {
                    return;
                }

                int nval    = int.Parse(m.Groups[1].Value);
                _.FontSize  = initFontSize * (Math.Max(1, Math.Min(nval, 10000)) / 100f);
                menuComboBoxZoom.Text = nval + " %";

                if(commonCfg != null) {
                    commonCfg.Zoomed = nval;
                }
            }
            catch(Exception ex){
                Log.Debug("Failed change font size: '{0}'", ex.Message);
            }
        }

        private void textEditor_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if(Keyboard.Modifiers != InputModifierKeys.Control) {
                return;
            }
            e.Handled = true;

            double nval = _.FontSize;
            if(e.Delta > 0) {
                nval *= 1.1f;
            }
            else {
                nval /= 1.1f;
            }
            _.FontSize = Math.Max(1, Math.Min(nval, 10000));
            menuComboBoxZoom.Text = zoomedValue(_.FontSize);
        }

        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            updateFoldings();
        }

        private void menuItemCut_Click(object sender, EventArgs e)
        {
            _.Cut();
        }

        private void menuItemCopy_Click(object sender, EventArgs e)
        {
            _.Copy();
        }

        private void menuItemPaste_Click(object sender, EventArgs e)
        {
            _.Paste();
        }

        private void menuItemUndo_Click(object sender, EventArgs e)
        {
            _.Undo();
        }

        private void menuItemRedo_Click(object sender, EventArgs e)
        {
            _.Redo();
        }

        private void menuItemWordWrap_Click(object sender, EventArgs e)
        {
            menuItemWordWrap.Checked = _.WordWrap = !_.WordWrap;
            if(commonCfg != null) {
                commonCfg.WordWrap = _.WordWrap;
            }
        }

        private void contextMenuEditor_Opened(object sender, EventArgs e)
        {
            menuComboBoxZoom.Text       = zoomedValue(_.FontSize);
            menuItemWordWrap.Checked    = _.WordWrap;
        }

        private void menuItemSearch_Click(object sender, EventArgs e)
        {
            searchPanel.Open();
        }
    }
}
