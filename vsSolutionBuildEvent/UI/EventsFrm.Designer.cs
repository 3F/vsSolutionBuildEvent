namespace net.r_eg.vsSBE.UI
{
    partial class EventsFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EventsFrm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.comboBoxEvents = new System.Windows.Forms.ComboBox();
            this.labelToCommandBox = new System.Windows.Forms.Label();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.textBoxCommand = new System.Windows.Forms.RichTextBox();
            this.contextMenuEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemEditorCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemEditorCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemEditorPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxPMode = new System.Windows.Forms.GroupBox();
            this.radioModeOperation = new System.Windows.Forms.RadioButton();
            this.radioModeScript = new System.Windows.Forms.RadioButton();
            this.radioModeFiles = new System.Windows.Forms.RadioButton();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.panelControl = new System.Windows.Forms.Panel();
            this.checkBoxOperationsAbort = new System.Windows.Forms.CheckBox();
            this.checkBoxIgnoreIfFailed = new System.Windows.Forms.CheckBox();
            this.checkBoxParseVariables = new System.Windows.Forms.CheckBox();
            this.panelControlByOperation = new System.Windows.Forms.Panel();
            this.checkBoxProcessKeep = new System.Windows.Forms.CheckBox();
            this.checkBoxWaitForExit = new System.Windows.Forms.CheckBox();
            this.checkBoxProcessHide = new System.Windows.Forms.CheckBox();
            this.checkBoxStatus = new System.Windows.Forms.CheckBox();
            this.groupBoxEW = new System.Windows.Forms.GroupBox();
            this.textBoxEW = new System.Windows.Forms.TextBox();
            this.btnEWRemove = new System.Windows.Forms.Button();
            this.btnEWAdd = new System.Windows.Forms.Button();
            this.listBoxEW = new System.Windows.Forms.ListBox();
            this.radioCodesBlacklist = new System.Windows.Forms.RadioButton();
            this.radioCodesWhitelist = new System.Windows.Forms.RadioButton();
            this.groupBoxOutputControl = new System.Windows.Forms.GroupBox();
            this.dataGridViewOutput = new System.Windows.Forms.DataGridView();
            this.owpTerm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.owpType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.owpRemove = new System.Windows.Forms.DataGridViewButtonColumn();
            this.groupBoxInterpreter = new System.Windows.Forms.GroupBox();
            this.comboBoxWrapper = new System.Windows.Forms.ComboBox();
            this.labelWrapper = new System.Windows.Forms.Label();
            this.labelTreatNewline = new System.Windows.Forms.Label();
            this.comboBoxNewline = new System.Windows.Forms.ComboBox();
            this.comboBoxInterpreter = new System.Windows.Forms.ComboBox();
            this.groupBoxVariants = new System.Windows.Forms.GroupBox();
            this.listBoxOperation = new System.Windows.Forms.ListBox();
            this.btnDteCmd = new System.Windows.Forms.Button();
            this.labelCaption = new System.Windows.Forms.Label();
            this.textBoxCaption = new System.Windows.Forms.TextBox();
            this.buttonEnvVariables = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripMenuSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripMenuSettings = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuApply = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuReset = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuMSBuildProp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuEvaluatingProperty = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuDTECmd = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuDTECmdExec = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuHelp = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuDoc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuChangelog = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuWiki = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuIssue = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuSources = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuForkGithub = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuForkBitbucket = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuLicense = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuBug = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripMenuReport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuDebugMode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuVersion = new System.Windows.Forms.ToolStripStatusLabel();
            this.checkedListBoxSpecCfg = new System.Windows.Forms.CheckedListBox();
            this.labelOnlyFor = new System.Windows.Forms.Label();
            this.dataGridViewOrder = new System.Windows.Forms.DataGridView();
            this.dgvOrderCheckBoxEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvOrderTextBoxProject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvOrderComboBoxType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.labelOrder = new System.Windows.Forms.Label();
            this.panelLineBottom = new System.Windows.Forms.Panel();
            this.panelLineBottom2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panelCommand.SuspendLayout();
            this.contextMenuEditor.SuspendLayout();
            this.groupBoxPMode.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.panelControl.SuspendLayout();
            this.panelControlByOperation.SuspendLayout();
            this.groupBoxEW.SuspendLayout();
            this.groupBoxOutputControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOutput)).BeginInit();
            this.groupBoxInterpreter.SuspendLayout();
            this.groupBoxVariants.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrder)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxEvents
            // 
            this.comboBoxEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEvents.FormattingEnabled = true;
            this.comboBoxEvents.Location = new System.Drawing.Point(0, 0);
            this.comboBoxEvents.Name = "comboBoxEvents";
            this.comboBoxEvents.Size = new System.Drawing.Size(772, 21);
            this.comboBoxEvents.TabIndex = 1;
            this.comboBoxEvents.SelectedIndexChanged += new System.EventHandler(this.comboBoxEvents_SelectedIndexChanged);
            // 
            // labelToCommandBox
            // 
            this.labelToCommandBox.AutoSize = true;
            this.labelToCommandBox.Location = new System.Drawing.Point(1, 264);
            this.labelToCommandBox.Name = "labelToCommandBox";
            this.labelToCommandBox.Size = new System.Drawing.Size(22, 13);
            this.labelToCommandBox.TabIndex = 15;
            this.labelToCommandBox.Text = "-----";
            // 
            // panelCommand
            // 
            this.panelCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCommand.Controls.Add(this.textBoxCommand);
            this.panelCommand.Location = new System.Drawing.Point(3, 283);
            this.panelCommand.Name = "panelCommand";
            this.panelCommand.Size = new System.Drawing.Size(766, 154);
            this.panelCommand.TabIndex = 16;
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.textBoxCommand.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCommand.ContextMenuStrip = this.contextMenuEditor;
            this.textBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCommand.Location = new System.Drawing.Point(0, 0);
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBoxCommand.Size = new System.Drawing.Size(764, 152);
            this.textBoxCommand.TabIndex = 4;
            this.textBoxCommand.Text = "";
            this.textBoxCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCommand_KeyDown);
            // 
            // contextMenuEditor
            // 
            this.contextMenuEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemEditorCut,
            this.menuItemEditorCopy,
            this.toolStripSeparator1,
            this.menuItemEditorPaste});
            this.contextMenuEditor.Name = "contextMenuEditor";
            this.contextMenuEditor.Size = new System.Drawing.Size(103, 76);
            // 
            // menuItemEditorCut
            // 
            this.menuItemEditorCut.Name = "menuItemEditorCut";
            this.menuItemEditorCut.Size = new System.Drawing.Size(102, 22);
            this.menuItemEditorCut.Text = "Cut";
            this.menuItemEditorCut.Click += new System.EventHandler(this.menuItemEditorCut_Click);
            // 
            // menuItemEditorCopy
            // 
            this.menuItemEditorCopy.Name = "menuItemEditorCopy";
            this.menuItemEditorCopy.Size = new System.Drawing.Size(102, 22);
            this.menuItemEditorCopy.Text = "Copy";
            this.menuItemEditorCopy.Click += new System.EventHandler(this.menuItemEditorCopy_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(99, 6);
            // 
            // menuItemEditorPaste
            // 
            this.menuItemEditorPaste.Name = "menuItemEditorPaste";
            this.menuItemEditorPaste.Size = new System.Drawing.Size(102, 22);
            this.menuItemEditorPaste.Text = "Paste";
            this.menuItemEditorPaste.Click += new System.EventHandler(this.menuItemEditorPaste_Click);
            // 
            // groupBoxPMode
            // 
            this.groupBoxPMode.Controls.Add(this.radioModeOperation);
            this.groupBoxPMode.Controls.Add(this.radioModeScript);
            this.groupBoxPMode.Controls.Add(this.radioModeFiles);
            this.groupBoxPMode.Location = new System.Drawing.Point(203, 27);
            this.groupBoxPMode.Name = "groupBoxPMode";
            this.groupBoxPMode.Size = new System.Drawing.Size(124, 142);
            this.groupBoxPMode.TabIndex = 26;
            this.groupBoxPMode.TabStop = false;
            this.groupBoxPMode.Text = "Processing mode";
            // 
            // radioModeOperation
            // 
            this.radioModeOperation.AutoSize = true;
            this.radioModeOperation.Location = new System.Drawing.Point(6, 43);
            this.radioModeOperation.Name = "radioModeOperation";
            this.radioModeOperation.Size = new System.Drawing.Size(101, 17);
            this.radioModeOperation.TabIndex = 25;
            this.radioModeOperation.TabStop = true;
            this.radioModeOperation.Text = "Operation Mode";
            this.radioModeOperation.UseVisualStyleBackColor = true;
            this.radioModeOperation.CheckedChanged += new System.EventHandler(this.radioModeOperation_CheckedChanged);
            // 
            // radioModeScript
            // 
            this.radioModeScript.AutoSize = true;
            this.radioModeScript.Location = new System.Drawing.Point(6, 67);
            this.radioModeScript.Name = "radioModeScript";
            this.radioModeScript.Size = new System.Drawing.Size(103, 17);
            this.radioModeScript.TabIndex = 23;
            this.radioModeScript.Text = "Interpreter Mode";
            this.radioModeScript.UseVisualStyleBackColor = true;
            this.radioModeScript.CheckedChanged += new System.EventHandler(this.radioModeScript_CheckedChanged);
            // 
            // radioModeFiles
            // 
            this.radioModeFiles.AutoSize = true;
            this.radioModeFiles.Location = new System.Drawing.Point(6, 19);
            this.radioModeFiles.Name = "radioModeFiles";
            this.radioModeFiles.Size = new System.Drawing.Size(76, 17);
            this.radioModeFiles.TabIndex = 24;
            this.radioModeFiles.Text = "Files Mode";
            this.radioModeFiles.UseVisualStyleBackColor = true;
            this.radioModeFiles.CheckedChanged += new System.EventHandler(this.radioModeFiles_CheckedChanged);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Controls.Add(this.panelControl);
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 27);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(197, 142);
            this.groupBoxSettings.TabIndex = 25;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Control";
            // 
            // panelControl
            // 
            this.panelControl.AutoScroll = true;
            this.panelControl.Controls.Add(this.checkBoxOperationsAbort);
            this.panelControl.Controls.Add(this.checkBoxIgnoreIfFailed);
            this.panelControl.Controls.Add(this.checkBoxParseVariables);
            this.panelControl.Controls.Add(this.panelControlByOperation);
            this.panelControl.Controls.Add(this.checkBoxStatus);
            this.panelControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl.Location = new System.Drawing.Point(3, 16);
            this.panelControl.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl.Name = "panelControl";
            this.panelControl.Size = new System.Drawing.Size(191, 123);
            this.panelControl.TabIndex = 32;
            // 
            // checkBoxOperationsAbort
            // 
            this.checkBoxOperationsAbort.AutoSize = true;
            this.checkBoxOperationsAbort.Checked = true;
            this.checkBoxOperationsAbort.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOperationsAbort.Location = new System.Drawing.Point(8, 126);
            this.checkBoxOperationsAbort.Name = "checkBoxOperationsAbort";
            this.checkBoxOperationsAbort.Size = new System.Drawing.Size(161, 17);
            this.checkBoxOperationsAbort.TabIndex = 20;
            this.checkBoxOperationsAbort.Text = "Abort operations on first error";
            this.checkBoxOperationsAbort.UseVisualStyleBackColor = true;
            // 
            // checkBoxIgnoreIfFailed
            // 
            this.checkBoxIgnoreIfFailed.AutoSize = true;
            this.checkBoxIgnoreIfFailed.Checked = true;
            this.checkBoxIgnoreIfFailed.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxIgnoreIfFailed.Location = new System.Drawing.Point(8, 84);
            this.checkBoxIgnoreIfFailed.Name = "checkBoxIgnoreIfFailed";
            this.checkBoxIgnoreIfFailed.Size = new System.Drawing.Size(135, 17);
            this.checkBoxIgnoreIfFailed.TabIndex = 19;
            this.checkBoxIgnoreIfFailed.Text = "Ignore if the build failed";
            this.checkBoxIgnoreIfFailed.UseVisualStyleBackColor = true;
            // 
            // checkBoxParseVariables
            // 
            this.checkBoxParseVariables.AutoSize = true;
            this.checkBoxParseVariables.Checked = true;
            this.checkBoxParseVariables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxParseVariables.Location = new System.Drawing.Point(8, 105);
            this.checkBoxParseVariables.Name = "checkBoxParseVariables";
            this.checkBoxParseVariables.Size = new System.Drawing.Size(103, 17);
            this.checkBoxParseVariables.TabIndex = 17;
            this.checkBoxParseVariables.Text = "MSBuild support";
            this.checkBoxParseVariables.UseVisualStyleBackColor = true;
            // 
            // panelControlByOperation
            // 
            this.panelControlByOperation.Controls.Add(this.checkBoxProcessKeep);
            this.panelControlByOperation.Controls.Add(this.checkBoxWaitForExit);
            this.panelControlByOperation.Controls.Add(this.checkBoxProcessHide);
            this.panelControlByOperation.Location = new System.Drawing.Point(0, 21);
            this.panelControlByOperation.Margin = new System.Windows.Forms.Padding(0);
            this.panelControlByOperation.Name = "panelControlByOperation";
            this.panelControlByOperation.Size = new System.Drawing.Size(171, 63);
            this.panelControlByOperation.TabIndex = 18;
            // 
            // checkBoxProcessKeep
            // 
            this.checkBoxProcessKeep.AutoSize = true;
            this.checkBoxProcessKeep.Checked = true;
            this.checkBoxProcessKeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProcessKeep.Enabled = false;
            this.checkBoxProcessKeep.Location = new System.Drawing.Point(8, 42);
            this.checkBoxProcessKeep.Name = "checkBoxProcessKeep";
            this.checkBoxProcessKeep.Size = new System.Drawing.Size(140, 17);
            this.checkBoxProcessKeep.TabIndex = 18;
            this.checkBoxProcessKeep.Text = "Keep window with result";
            this.checkBoxProcessKeep.UseVisualStyleBackColor = true;
            // 
            // checkBoxWaitForExit
            // 
            this.checkBoxWaitForExit.AutoSize = true;
            this.checkBoxWaitForExit.Checked = true;
            this.checkBoxWaitForExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWaitForExit.Location = new System.Drawing.Point(8, 0);
            this.checkBoxWaitForExit.Name = "checkBoxWaitForExit";
            this.checkBoxWaitForExit.Size = new System.Drawing.Size(131, 17);
            this.checkBoxWaitForExit.TabIndex = 16;
            this.checkBoxWaitForExit.Text = "Waiting for completion";
            this.checkBoxWaitForExit.UseVisualStyleBackColor = true;
            // 
            // checkBoxProcessHide
            // 
            this.checkBoxProcessHide.AutoSize = true;
            this.checkBoxProcessHide.Checked = true;
            this.checkBoxProcessHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProcessHide.Location = new System.Drawing.Point(8, 21);
            this.checkBoxProcessHide.Name = "checkBoxProcessHide";
            this.checkBoxProcessHide.Size = new System.Drawing.Size(88, 17);
            this.checkBoxProcessHide.TabIndex = 17;
            this.checkBoxProcessHide.Text = "Hide process";
            this.checkBoxProcessHide.UseVisualStyleBackColor = true;
            this.checkBoxProcessHide.CheckedChanged += new System.EventHandler(this.checkBoxProcessHide_CheckedChanged);
            // 
            // checkBoxStatus
            // 
            this.checkBoxStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.checkBoxStatus.Location = new System.Drawing.Point(8, 0);
            this.checkBoxStatus.Name = "checkBoxStatus";
            this.checkBoxStatus.Size = new System.Drawing.Size(145, 17);
            this.checkBoxStatus.TabIndex = 0;
            this.checkBoxStatus.Text = "Enabled";
            this.checkBoxStatus.UseVisualStyleBackColor = false;
            this.checkBoxStatus.CheckedChanged += new System.EventHandler(this.checkBoxStatus_CheckedChanged);
            // 
            // groupBoxEW
            // 
            this.groupBoxEW.Controls.Add(this.textBoxEW);
            this.groupBoxEW.Controls.Add(this.btnEWRemove);
            this.groupBoxEW.Controls.Add(this.btnEWAdd);
            this.groupBoxEW.Controls.Add(this.listBoxEW);
            this.groupBoxEW.Controls.Add(this.radioCodesBlacklist);
            this.groupBoxEW.Controls.Add(this.radioCodesWhitelist);
            this.groupBoxEW.Location = new System.Drawing.Point(333, 27);
            this.groupBoxEW.Name = "groupBoxEW";
            this.groupBoxEW.Size = new System.Drawing.Size(138, 142);
            this.groupBoxEW.TabIndex = 27;
            this.groupBoxEW.TabStop = false;
            this.groupBoxEW.Text = "Errors / Warnings";
            // 
            // textBoxEW
            // 
            this.textBoxEW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxEW.Location = new System.Drawing.Point(0, 15);
            this.textBoxEW.Name = "textBoxEW";
            this.textBoxEW.Size = new System.Drawing.Size(108, 20);
            this.textBoxEW.TabIndex = 5;
            // 
            // btnEWRemove
            // 
            this.btnEWRemove.Location = new System.Drawing.Point(112, 37);
            this.btnEWRemove.Name = "btnEWRemove";
            this.btnEWRemove.Size = new System.Drawing.Size(24, 20);
            this.btnEWRemove.TabIndex = 4;
            this.btnEWRemove.Text = "-";
            this.btnEWRemove.UseVisualStyleBackColor = true;
            this.btnEWRemove.Click += new System.EventHandler(this.btnEWRemove_Click);
            // 
            // btnEWAdd
            // 
            this.btnEWAdd.Location = new System.Drawing.Point(112, 15);
            this.btnEWAdd.Name = "btnEWAdd";
            this.btnEWAdd.Size = new System.Drawing.Size(24, 21);
            this.btnEWAdd.TabIndex = 3;
            this.btnEWAdd.Text = "+";
            this.btnEWAdd.UseVisualStyleBackColor = true;
            this.btnEWAdd.Click += new System.EventHandler(this.btnEWAdd_Click);
            // 
            // listBoxEW
            // 
            this.listBoxEW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxEW.FormattingEnabled = true;
            this.listBoxEW.Location = new System.Drawing.Point(0, 37);
            this.listBoxEW.Name = "listBoxEW";
            this.listBoxEW.Size = new System.Drawing.Size(108, 80);
            this.listBoxEW.TabIndex = 2;
            // 
            // radioCodesBlacklist
            // 
            this.radioCodesBlacklist.AutoSize = true;
            this.radioCodesBlacklist.Location = new System.Drawing.Point(74, 119);
            this.radioCodesBlacklist.Name = "radioCodesBlacklist";
            this.radioCodesBlacklist.Size = new System.Drawing.Size(64, 17);
            this.radioCodesBlacklist.TabIndex = 1;
            this.radioCodesBlacklist.TabStop = true;
            this.radioCodesBlacklist.Text = "Blacklist";
            this.radioCodesBlacklist.UseVisualStyleBackColor = true;
            // 
            // radioCodesWhitelist
            // 
            this.radioCodesWhitelist.AutoSize = true;
            this.radioCodesWhitelist.Location = new System.Drawing.Point(0, 119);
            this.radioCodesWhitelist.Name = "radioCodesWhitelist";
            this.radioCodesWhitelist.Size = new System.Drawing.Size(73, 17);
            this.radioCodesWhitelist.TabIndex = 0;
            this.radioCodesWhitelist.Text = "Whitelist /";
            this.radioCodesWhitelist.UseVisualStyleBackColor = true;
            // 
            // groupBoxOutputControl
            // 
            this.groupBoxOutputControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxOutputControl.Controls.Add(this.dataGridViewOutput);
            this.groupBoxOutputControl.Location = new System.Drawing.Point(477, 27);
            this.groupBoxOutputControl.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxOutputControl.Name = "groupBoxOutputControl";
            this.groupBoxOutputControl.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxOutputControl.Size = new System.Drawing.Size(295, 147);
            this.groupBoxOutputControl.TabIndex = 6;
            this.groupBoxOutputControl.TabStop = false;
            this.groupBoxOutputControl.Text = "Output customization";
            // 
            // dataGridViewOutput
            // 
            this.dataGridViewOutput.AllowUserToResizeColumns = false;
            this.dataGridViewOutput.AllowUserToResizeRows = false;
            this.dataGridViewOutput.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewOutput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOutput.ColumnHeadersVisible = false;
            this.dataGridViewOutput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.owpTerm,
            this.owpType,
            this.owpRemove});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewOutput.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewOutput.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewOutput.Location = new System.Drawing.Point(0, 13);
            this.dataGridViewOutput.MultiSelect = false;
            this.dataGridViewOutput.Name = "dataGridViewOutput";
            this.dataGridViewOutput.RowHeadersVisible = false;
            this.dataGridViewOutput.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            this.dataGridViewOutput.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dataGridViewOutput.RowTemplate.Height = 17;
            this.dataGridViewOutput.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewOutput.Size = new System.Drawing.Size(295, 134);
            this.dataGridViewOutput.TabIndex = 6;
            this.dataGridViewOutput.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewOutput_CellClick);
            // 
            // owpTerm
            // 
            this.owpTerm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.owpTerm.HeaderText = "term";
            this.owpTerm.Name = "owpTerm";
            // 
            // owpType
            // 
            this.owpType.HeaderText = "type";
            this.owpType.Items.AddRange(new object[] {
            "Default",
            "Regexp",
            "Wildcards"});
            this.owpType.MinimumWidth = 76;
            this.owpType.Name = "owpType";
            this.owpType.Width = 76;
            // 
            // owpRemove
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Maroon;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Maroon;
            this.owpRemove.DefaultCellStyle = dataGridViewCellStyle1;
            this.owpRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.owpRemove.HeaderText = "remove";
            this.owpRemove.MinimumWidth = 16;
            this.owpRemove.Name = "owpRemove";
            this.owpRemove.Text = "-";
            this.owpRemove.UseColumnTextForButtonValue = true;
            this.owpRemove.Width = 16;
            // 
            // groupBoxInterpreter
            // 
            this.groupBoxInterpreter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxInterpreter.Controls.Add(this.comboBoxWrapper);
            this.groupBoxInterpreter.Controls.Add(this.labelWrapper);
            this.groupBoxInterpreter.Controls.Add(this.labelTreatNewline);
            this.groupBoxInterpreter.Controls.Add(this.comboBoxNewline);
            this.groupBoxInterpreter.Controls.Add(this.comboBoxInterpreter);
            this.groupBoxInterpreter.Location = new System.Drawing.Point(203, 175);
            this.groupBoxInterpreter.Name = "groupBoxInterpreter";
            this.groupBoxInterpreter.Size = new System.Drawing.Size(569, 102);
            this.groupBoxInterpreter.TabIndex = 28;
            this.groupBoxInterpreter.TabStop = false;
            this.groupBoxInterpreter.Text = "Interpreter settings:";
            // 
            // comboBoxWrapper
            // 
            this.comboBoxWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxWrapper.FormattingEnabled = true;
            this.comboBoxWrapper.Items.AddRange(new object[] {
            "\"",
            "",
            "\'",
            "`",
            "()",
            "{}",
            "[]"});
            this.comboBoxWrapper.Location = new System.Drawing.Point(104, 69);
            this.comboBoxWrapper.Name = "comboBoxWrapper";
            this.comboBoxWrapper.Size = new System.Drawing.Size(462, 21);
            this.comboBoxWrapper.TabIndex = 28;
            // 
            // labelWrapper
            // 
            this.labelWrapper.AutoSize = true;
            this.labelWrapper.Location = new System.Drawing.Point(14, 69);
            this.labelWrapper.Name = "labelWrapper";
            this.labelWrapper.Size = new System.Drawing.Size(48, 13);
            this.labelWrapper.TabIndex = 27;
            this.labelWrapper.Text = "wrapper:";
            // 
            // labelTreatNewline
            // 
            this.labelTreatNewline.AutoSize = true;
            this.labelTreatNewline.Location = new System.Drawing.Point(14, 47);
            this.labelTreatNewline.Name = "labelTreatNewline";
            this.labelTreatNewline.Size = new System.Drawing.Size(84, 13);
            this.labelTreatNewline.TabIndex = 26;
            this.labelTreatNewline.Text = "treat newline as:";
            // 
            // comboBoxNewline
            // 
            this.comboBoxNewline.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxNewline.FormattingEnabled = true;
            this.comboBoxNewline.Items.AddRange(new object[] {
            "&",
            "",
            "\\",
            "&echo.&echo "});
            this.comboBoxNewline.Location = new System.Drawing.Point(104, 44);
            this.comboBoxNewline.Name = "comboBoxNewline";
            this.comboBoxNewline.Size = new System.Drawing.Size(462, 21);
            this.comboBoxNewline.TabIndex = 25;
            // 
            // comboBoxInterpreter
            // 
            this.comboBoxInterpreter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxInterpreter.FormattingEnabled = true;
            this.comboBoxInterpreter.Items.AddRange(new object[] {
            "cmd.exe /C",
            "php -r"});
            this.comboBoxInterpreter.Location = new System.Drawing.Point(3, 19);
            this.comboBoxInterpreter.Name = "comboBoxInterpreter";
            this.comboBoxInterpreter.Size = new System.Drawing.Size(563, 21);
            this.comboBoxInterpreter.TabIndex = 24;
            // 
            // groupBoxVariants
            // 
            this.groupBoxVariants.Controls.Add(this.listBoxOperation);
            this.groupBoxVariants.Location = new System.Drawing.Point(0, 175);
            this.groupBoxVariants.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxVariants.Name = "groupBoxVariants";
            this.groupBoxVariants.Size = new System.Drawing.Size(197, 87);
            this.groupBoxVariants.TabIndex = 29;
            this.groupBoxVariants.TabStop = false;
            this.groupBoxVariants.Text = "Operation variants:";
            // 
            // listBoxOperation
            // 
            this.listBoxOperation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listBoxOperation.FormattingEnabled = true;
            this.listBoxOperation.Location = new System.Drawing.Point(4, 16);
            this.listBoxOperation.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxOperation.Name = "listBoxOperation";
            this.listBoxOperation.Size = new System.Drawing.Size(190, 67);
            this.listBoxOperation.TabIndex = 31;
            this.listBoxOperation.SelectedIndexChanged += new System.EventHandler(this.listBoxOperation_SelectedIndexChanged);
            // 
            // btnDteCmd
            // 
            this.btnDteCmd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDteCmd.Location = new System.Drawing.Point(115, 7);
            this.btnDteCmd.Name = "btnDteCmd";
            this.btnDteCmd.Size = new System.Drawing.Size(102, 20);
            this.btnDteCmd.TabIndex = 25;
            this.btnDteCmd.Text = "DTE Commands";
            this.btnDteCmd.UseVisualStyleBackColor = true;
            this.btnDteCmd.Click += new System.EventHandler(this.btnDteCmd_Click);
            // 
            // labelCaption
            // 
            this.labelCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCaption.AutoSize = true;
            this.labelCaption.Location = new System.Drawing.Point(466, 461);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(46, 13);
            this.labelCaption.TabIndex = 23;
            this.labelCaption.Text = "Caption:";
            // 
            // textBoxCaption
            // 
            this.textBoxCaption.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCaption.Location = new System.Drawing.Point(466, 477);
            this.textBoxCaption.Name = "textBoxCaption";
            this.textBoxCaption.Size = new System.Drawing.Size(221, 20);
            this.textBoxCaption.TabIndex = 22;
            // 
            // buttonEnvVariables
            // 
            this.buttonEnvVariables.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.buttonEnvVariables.Location = new System.Drawing.Point(3, 6);
            this.buttonEnvVariables.Name = "buttonEnvVariables";
            this.buttonEnvVariables.Size = new System.Drawing.Size(106, 20);
            this.buttonEnvVariables.TabIndex = 24;
            this.buttonEnvVariables.Text = "MSBuild Properties";
            this.buttonEnvVariables.UseVisualStyleBackColor = true;
            this.buttonEnvVariables.Click += new System.EventHandler(this.buttonEnvVariables_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(694, 477);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 19;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuSpring,
            this.toolStripMenuSettings,
            this.toolStripMenuHelp,
            this.toolStripMenuBug,
            this.toolStripMenuVersion});
            this.statusStrip.Location = new System.Drawing.Point(614, 513);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(158, 22);
            this.statusStrip.TabIndex = 31;
            // 
            // toolStripMenuSpring
            // 
            this.toolStripMenuSpring.Name = "toolStripMenuSpring";
            this.toolStripMenuSpring.Size = new System.Drawing.Size(2, 17);
            this.toolStripMenuSpring.Spring = true;
            // 
            // toolStripMenuSettings
            // 
            this.toolStripMenuSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuApply,
            this.toolStripMenuReset,
            this.toolStripSeparator6,
            this.toolStripMenuTools});
            this.toolStripMenuSettings.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuSettings.Image")));
            this.toolStripMenuSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuSettings.Name = "toolStripMenuSettings";
            this.toolStripMenuSettings.Size = new System.Drawing.Size(32, 20);
            this.toolStripMenuSettings.Text = "S";
            // 
            // toolStripMenuApply
            // 
            this.toolStripMenuApply.Name = "toolStripMenuApply";
            this.toolStripMenuApply.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuApply.Text = "Apply";
            this.toolStripMenuApply.Click += new System.EventHandler(this.toolStripMenuApply_Click);
            // 
            // toolStripMenuReset
            // 
            this.toolStripMenuReset.Name = "toolStripMenuReset";
            this.toolStripMenuReset.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuReset.Text = "Reset";
            this.toolStripMenuReset.Click += new System.EventHandler(this.toolStripMenuReset_Click);
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(102, 6);
            // 
            // toolStripMenuTools
            // 
            this.toolStripMenuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuMSBuildProp,
            this.toolStripMenuEvaluatingProperty,
            this.toolStripMenuDTECmd,
            this.toolStripMenuDTECmdExec});
            this.toolStripMenuTools.Name = "toolStripMenuTools";
            this.toolStripMenuTools.Size = new System.Drawing.Size(105, 22);
            this.toolStripMenuTools.Text = "Tools";
            // 
            // toolStripMenuMSBuildProp
            // 
            this.toolStripMenuMSBuildProp.Name = "toolStripMenuMSBuildProp";
            this.toolStripMenuMSBuildProp.Size = new System.Drawing.Size(203, 22);
            this.toolStripMenuMSBuildProp.Text = "MSBuild Properties";
            this.toolStripMenuMSBuildProp.Click += new System.EventHandler(this.toolStripMenuMSBuildProp_Click);
            // 
            // toolStripMenuEvaluatingProperty
            // 
            this.toolStripMenuEvaluatingProperty.Name = "toolStripMenuEvaluatingProperty";
            this.toolStripMenuEvaluatingProperty.Size = new System.Drawing.Size(203, 22);
            this.toolStripMenuEvaluatingProperty.Text = "Evaluating Property";
            this.toolStripMenuEvaluatingProperty.Click += new System.EventHandler(this.toolStripMenuEvaluatingProperty_Click);
            // 
            // toolStripMenuDTECmd
            // 
            this.toolStripMenuDTECmd.Name = "toolStripMenuDTECmd";
            this.toolStripMenuDTECmd.Size = new System.Drawing.Size(203, 22);
            this.toolStripMenuDTECmd.Text = "DTE Commands";
            this.toolStripMenuDTECmd.Click += new System.EventHandler(this.toolStripMenuDTECmd_Click);
            // 
            // toolStripMenuDTECmdExec
            // 
            this.toolStripMenuDTECmdExec.Name = "toolStripMenuDTECmdExec";
            this.toolStripMenuDTECmdExec.Size = new System.Drawing.Size(203, 22);
            this.toolStripMenuDTECmdExec.Text = "Execute DTE Commands";
            this.toolStripMenuDTECmdExec.Click += new System.EventHandler(this.toolStripMenuDTECmdExec_Click);
            // 
            // toolStripMenuHelp
            // 
            this.toolStripMenuHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuDoc,
            this.toolStripMenuChangelog,
            this.toolStripMenuWiki,
            this.toolStripSeparator3,
            this.toolStripMenuIssue,
            this.toolStripMenuSources,
            this.toolStripSeparator4,
            this.toolStripMenuForkGithub,
            this.toolStripMenuForkBitbucket,
            this.toolStripSeparator2,
            this.toolStripMenuLicense,
            this.toolStripMenuAbout});
            this.toolStripMenuHelp.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuHelp.Image")));
            this.toolStripMenuHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuHelp.Name = "toolStripMenuHelp";
            this.toolStripMenuHelp.Size = new System.Drawing.Size(32, 20);
            this.toolStripMenuHelp.Text = "?";
            // 
            // toolStripMenuDoc
            // 
            this.toolStripMenuDoc.Name = "toolStripMenuDoc";
            this.toolStripMenuDoc.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuDoc.Text = "Documentation";
            this.toolStripMenuDoc.Click += new System.EventHandler(this.toolStripMenuDoc_Click);
            // 
            // toolStripMenuChangelog
            // 
            this.toolStripMenuChangelog.Name = "toolStripMenuChangelog";
            this.toolStripMenuChangelog.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuChangelog.Text = "Change List";
            this.toolStripMenuChangelog.Click += new System.EventHandler(this.toolStripMenuChangelog_Click);
            // 
            // toolStripMenuWiki
            // 
            this.toolStripMenuWiki.Name = "toolStripMenuWiki";
            this.toolStripMenuWiki.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuWiki.Text = "Wiki";
            this.toolStripMenuWiki.Click += new System.EventHandler(this.toolStripMenuWiki_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(175, 6);
            // 
            // toolStripMenuIssue
            // 
            this.toolStripMenuIssue.Name = "toolStripMenuIssue";
            this.toolStripMenuIssue.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuIssue.Text = "Public Issue Tracker";
            this.toolStripMenuIssue.Click += new System.EventHandler(this.toolStripMenuIssue_Click);
            // 
            // toolStripMenuSources
            // 
            this.toolStripMenuSources.Name = "toolStripMenuSources";
            this.toolStripMenuSources.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuSources.Text = "Sources";
            this.toolStripMenuSources.Click += new System.EventHandler(this.toolStripMenuSources_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(175, 6);
            // 
            // toolStripMenuForkGithub
            // 
            this.toolStripMenuForkGithub.Name = "toolStripMenuForkGithub";
            this.toolStripMenuForkGithub.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuForkGithub.Text = "Fork on GitHub";
            this.toolStripMenuForkGithub.Click += new System.EventHandler(this.toolStripMenuForkGithub_Click);
            // 
            // toolStripMenuForkBitbucket
            // 
            this.toolStripMenuForkBitbucket.Name = "toolStripMenuForkBitbucket";
            this.toolStripMenuForkBitbucket.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuForkBitbucket.Text = "Fork on Bitbucket";
            this.toolStripMenuForkBitbucket.Click += new System.EventHandler(this.toolStripMenuForkBitbucket_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(175, 6);
            // 
            // toolStripMenuLicense
            // 
            this.toolStripMenuLicense.Name = "toolStripMenuLicense";
            this.toolStripMenuLicense.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuLicense.Text = "License";
            this.toolStripMenuLicense.Click += new System.EventHandler(this.toolStripMenuLicense_Click);
            // 
            // toolStripMenuAbout
            // 
            this.toolStripMenuAbout.Name = "toolStripMenuAbout";
            this.toolStripMenuAbout.Size = new System.Drawing.Size(178, 22);
            this.toolStripMenuAbout.Text = "About";
            this.toolStripMenuAbout.Click += new System.EventHandler(this.toolStripMenuAbout_Click);
            // 
            // toolStripMenuBug
            // 
            this.toolStripMenuBug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripMenuBug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuReport,
            this.toolStripMenuDebugMode});
            this.toolStripMenuBug.Image = ((System.Drawing.Image)(resources.GetObject("toolStripMenuBug.Image")));
            this.toolStripMenuBug.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripMenuBug.Name = "toolStripMenuBug";
            this.toolStripMenuBug.Size = new System.Drawing.Size(32, 20);
            // 
            // toolStripMenuReport
            // 
            this.toolStripMenuReport.Name = "toolStripMenuReport";
            this.toolStripMenuReport.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuReport.Text = "Report";
            this.toolStripMenuReport.Click += new System.EventHandler(this.toolStripMenuReport_Click);
            // 
            // toolStripMenuDebugMode
            // 
            this.toolStripMenuDebugMode.Name = "toolStripMenuDebugMode";
            this.toolStripMenuDebugMode.Size = new System.Drawing.Size(143, 22);
            this.toolStripMenuDebugMode.Text = "Debug Mode";
            this.toolStripMenuDebugMode.Click += new System.EventHandler(this.toolStripMenuDebugMode_Click);
            // 
            // toolStripMenuVersion
            // 
            this.toolStripMenuVersion.Name = "toolStripMenuVersion";
            this.toolStripMenuVersion.Size = new System.Drawing.Size(45, 17);
            this.toolStripMenuVersion.Text = "version";
            // 
            // checkedListBoxSpecCfg
            // 
            this.checkedListBoxSpecCfg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkedListBoxSpecCfg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.checkedListBoxSpecCfg.FormattingEnabled = true;
            this.checkedListBoxSpecCfg.Location = new System.Drawing.Point(3, 452);
            this.checkedListBoxSpecCfg.Margin = new System.Windows.Forms.Padding(0);
            this.checkedListBoxSpecCfg.MinimumSize = new System.Drawing.Size(191, 77);
            this.checkedListBoxSpecCfg.Name = "checkedListBoxSpecCfg";
            this.checkedListBoxSpecCfg.ScrollAlwaysVisible = true;
            this.checkedListBoxSpecCfg.Size = new System.Drawing.Size(191, 77);
            this.checkedListBoxSpecCfg.TabIndex = 0;
            this.checkedListBoxSpecCfg.Click += new System.EventHandler(this.checkedListBoxSpecCfg_Click);
            this.checkedListBoxSpecCfg.MouseLeave += new System.EventHandler(this.checkedListBoxSpecCfg_MouseLeave);
            // 
            // labelOnlyFor
            // 
            this.labelOnlyFor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOnlyFor.AutoSize = true;
            this.labelOnlyFor.Location = new System.Drawing.Point(1, 438);
            this.labelOnlyFor.Name = "labelOnlyFor";
            this.labelOnlyFor.Size = new System.Drawing.Size(46, 13);
            this.labelOnlyFor.TabIndex = 33;
            this.labelOnlyFor.Text = "Only for:";
            // 
            // dataGridViewOrder
            // 
            this.dataGridViewOrder.AllowUserToAddRows = false;
            this.dataGridViewOrder.AllowUserToDeleteRows = false;
            this.dataGridViewOrder.AllowUserToResizeColumns = false;
            this.dataGridViewOrder.AllowUserToResizeRows = false;
            this.dataGridViewOrder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewOrder.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewOrder.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewOrder.ColumnHeadersVisible = false;
            this.dataGridViewOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvOrderCheckBoxEnabled,
            this.dgvOrderTextBoxProject,
            this.dgvOrderComboBoxType});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewOrder.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewOrder.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dataGridViewOrder.Location = new System.Drawing.Point(197, 452);
            this.dataGridViewOrder.MultiSelect = false;
            this.dataGridViewOrder.Name = "dataGridViewOrder";
            this.dataGridViewOrder.RowHeadersVisible = false;
            this.dataGridViewOrder.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            this.dataGridViewOrder.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dataGridViewOrder.RowTemplate.Height = 17;
            this.dataGridViewOrder.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridViewOrder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewOrder.Size = new System.Drawing.Size(263, 78);
            this.dataGridViewOrder.TabIndex = 34;
            // 
            // dgvOrderCheckBoxEnabled
            // 
            this.dgvOrderCheckBoxEnabled.FillWeight = 21F;
            this.dgvOrderCheckBoxEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvOrderCheckBoxEnabled.HeaderText = "";
            this.dgvOrderCheckBoxEnabled.MinimumWidth = 16;
            this.dgvOrderCheckBoxEnabled.Name = "dgvOrderCheckBoxEnabled";
            this.dgvOrderCheckBoxEnabled.Width = 21;
            // 
            // dgvOrderTextBoxProject
            // 
            this.dgvOrderTextBoxProject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvOrderTextBoxProject.HeaderText = "project";
            this.dgvOrderTextBoxProject.Name = "dgvOrderTextBoxProject";
            this.dgvOrderTextBoxProject.ReadOnly = true;
            // 
            // dgvOrderComboBoxType
            // 
            this.dgvOrderComboBoxType.HeaderText = "type";
            this.dgvOrderComboBoxType.Items.AddRange(new object[] {
            "Before",
            "After"});
            this.dgvOrderComboBoxType.MinimumWidth = 76;
            this.dgvOrderComboBoxType.Name = "dgvOrderComboBoxType";
            this.dgvOrderComboBoxType.Width = 76;
            // 
            // labelOrder
            // 
            this.labelOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelOrder.AutoSize = true;
            this.labelOrder.Location = new System.Drawing.Point(197, 439);
            this.labelOrder.Name = "labelOrder";
            this.labelOrder.Size = new System.Drawing.Size(84, 13);
            this.labelOrder.TabIndex = 35;
            this.labelOrder.Text = "Execution order:";
            // 
            // panelLineBottom
            // 
            this.panelLineBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLineBottom.BackColor = System.Drawing.Color.DimGray;
            this.panelLineBottom.Location = new System.Drawing.Point(540, 513);
            this.panelLineBottom.Name = "panelLineBottom";
            this.panelLineBottom.Size = new System.Drawing.Size(232, 1);
            this.panelLineBottom.TabIndex = 36;
            // 
            // panelLineBottom2
            // 
            this.panelLineBottom2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelLineBottom2.BackColor = System.Drawing.Color.DimGray;
            this.panelLineBottom2.Location = new System.Drawing.Point(477, 506);
            this.panelLineBottom2.Name = "panelLineBottom2";
            this.panelLineBottom2.Size = new System.Drawing.Size(295, 1);
            this.panelLineBottom2.TabIndex = 37;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.buttonEnvVariables);
            this.panel1.Controls.Add(this.btnDteCmd);
            this.panel1.Location = new System.Drawing.Point(545, 435);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(223, 32);
            this.panel1.TabIndex = 38;
            // 
            // EventsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 535);
            this.Controls.Add(this.checkedListBoxSpecCfg);
            this.Controls.Add(this.panelLineBottom2);
            this.Controls.Add(this.panelLineBottom);
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.textBoxCaption);
            this.Controls.Add(this.labelOrder);
            this.Controls.Add(this.dataGridViewOrder);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.labelOnlyFor);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.groupBoxVariants);
            this.Controls.Add(this.groupBoxInterpreter);
            this.Controls.Add(this.groupBoxOutputControl);
            this.Controls.Add(this.groupBoxEW);
            this.Controls.Add(this.panelCommand);
            this.Controls.Add(this.groupBoxPMode);
            this.Controls.Add(this.comboBoxEvents);
            this.Controls.Add(this.labelToCommandBox);
            this.Controls.Add(this.panel1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(780, 428);
            this.Name = "EventsFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Solution Build-Events";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EventsFrm_FormClosing);
            this.Load += new System.EventHandler(this.EventsFrm_Load);
            this.panelCommand.ResumeLayout(false);
            this.contextMenuEditor.ResumeLayout(false);
            this.groupBoxPMode.ResumeLayout(false);
            this.groupBoxPMode.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.panelControl.ResumeLayout(false);
            this.panelControl.PerformLayout();
            this.panelControlByOperation.ResumeLayout(false);
            this.panelControlByOperation.PerformLayout();
            this.groupBoxEW.ResumeLayout(false);
            this.groupBoxEW.PerformLayout();
            this.groupBoxOutputControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOutput)).EndInit();
            this.groupBoxInterpreter.ResumeLayout(false);
            this.groupBoxInterpreter.PerformLayout();
            this.groupBoxVariants.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOrder)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxEvents;
        private System.Windows.Forms.Label labelToCommandBox;
        private System.Windows.Forms.Panel panelCommand;
        private System.Windows.Forms.RichTextBox textBoxCommand;
        private System.Windows.Forms.GroupBox groupBoxPMode;
        private System.Windows.Forms.RadioButton radioModeOperation;
        private System.Windows.Forms.RadioButton radioModeScript;
        private System.Windows.Forms.RadioButton radioModeFiles;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.CheckBox checkBoxParseVariables;
        private System.Windows.Forms.CheckBox checkBoxStatus;
        private System.Windows.Forms.GroupBox groupBoxEW;
        private System.Windows.Forms.Button btnEWRemove;
        private System.Windows.Forms.Button btnEWAdd;
        private System.Windows.Forms.ListBox listBoxEW;
        private System.Windows.Forms.RadioButton radioCodesBlacklist;
        private System.Windows.Forms.RadioButton radioCodesWhitelist;
        private System.Windows.Forms.GroupBox groupBoxOutputControl;
        private System.Windows.Forms.DataGridView dataGridViewOutput;
        private System.Windows.Forms.GroupBox groupBoxInterpreter;
        private System.Windows.Forms.GroupBox groupBoxVariants;
        private System.Windows.Forms.Button buttonEnvVariables;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.TextBox textBoxCaption;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ListBox listBoxOperation;
        private System.Windows.Forms.ComboBox comboBoxWrapper;
        private System.Windows.Forms.Label labelWrapper;
        private System.Windows.Forms.Label labelTreatNewline;
        private System.Windows.Forms.ComboBox comboBoxNewline;
        private System.Windows.Forms.ComboBox comboBoxInterpreter;
        private System.Windows.Forms.DataGridViewTextBoxColumn owpTerm;
        private System.Windows.Forms.DataGridViewComboBoxColumn owpType;
        private System.Windows.Forms.DataGridViewButtonColumn owpRemove;
        private System.Windows.Forms.TextBox textBoxEW;
        private System.Windows.Forms.Panel panelControlByOperation;
        private System.Windows.Forms.CheckBox checkBoxProcessKeep;
        private System.Windows.Forms.CheckBox checkBoxWaitForExit;
        private System.Windows.Forms.CheckBox checkBoxProcessHide;
        private System.Windows.Forms.Button btnDteCmd;
        private System.Windows.Forms.Panel panelControl;
        private System.Windows.Forms.CheckBox checkBoxIgnoreIfFailed;
        private System.Windows.Forms.ContextMenuStrip contextMenuEditor;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditorCut;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditorCopy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemEditorPaste;
        private System.Windows.Forms.CheckBox checkBoxOperationsAbort;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMenuSpring;
        private System.Windows.Forms.ToolStripSplitButton toolStripMenuHelp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDoc;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuChangelog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuWiki;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuIssue;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuSources;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuForkGithub;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuForkBitbucket;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuLicense;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuAbout;
        private System.Windows.Forms.ToolStripSplitButton toolStripMenuBug;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuReport;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDebugMode;
        private System.Windows.Forms.ToolStripStatusLabel toolStripMenuVersion;
        private System.Windows.Forms.CheckedListBox checkedListBoxSpecCfg;
        private System.Windows.Forms.Label labelOnlyFor;
        private System.Windows.Forms.DataGridView dataGridViewOrder;
        private System.Windows.Forms.Label labelOrder;
        private System.Windows.Forms.Panel panelLineBottom;
        private System.Windows.Forms.Panel panelLineBottom2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSplitButton toolStripMenuSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuReset;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuApply;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuTools;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuMSBuildProp;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDTECmd;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvOrderCheckBoxEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvOrderTextBoxProject;
        private System.Windows.Forms.DataGridViewComboBoxColumn dgvOrderComboBoxType;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuEvaluatingProperty;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuDTECmdExec;
    }
}