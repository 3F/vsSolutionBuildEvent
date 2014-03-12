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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.comboBoxEvents = new System.Windows.Forms.ComboBox();
            this.labelToCommandBox = new System.Windows.Forms.Label();
            this.panelCommand = new System.Windows.Forms.Panel();
            this.textBoxCommand = new System.Windows.Forms.RichTextBox();
            this.groupBoxPMode = new System.Windows.Forms.GroupBox();
            this.radioModeOperation = new System.Windows.Forms.RadioButton();
            this.radioModeScript = new System.Windows.Forms.RadioButton();
            this.radioModeFiles = new System.Windows.Forms.RadioButton();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.panelControlByOperation = new System.Windows.Forms.Panel();
            this.checkBoxProcessKeep = new System.Windows.Forms.CheckBox();
            this.checkBoxWaitForExit = new System.Windows.Forms.CheckBox();
            this.checkBoxProcessHide = new System.Windows.Forms.CheckBox();
            this.checkBoxParseVariables = new System.Windows.Forms.CheckBox();
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
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonEnvVariables = new System.Windows.Forms.Button();
            this.labelCaption = new System.Windows.Forms.Label();
            this.textBoxCaption = new System.Windows.Forms.TextBox();
            this.btnExample = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnDteCmd = new System.Windows.Forms.Button();
            this.panelCommand.SuspendLayout();
            this.groupBoxPMode.SuspendLayout();
            this.groupBoxSettings.SuspendLayout();
            this.panelControlByOperation.SuspendLayout();
            this.groupBoxEW.SuspendLayout();
            this.groupBoxOutputControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOutput)).BeginInit();
            this.groupBoxInterpreter.SuspendLayout();
            this.groupBoxVariants.SuspendLayout();
            this.panelBottom.SuspendLayout();
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
            this.textBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCommand.Location = new System.Drawing.Point(0, 0);
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.textBoxCommand.Size = new System.Drawing.Size(764, 152);
            this.textBoxCommand.TabIndex = 4;
            this.textBoxCommand.Text = "";
            this.textBoxCommand.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxCommand_KeyDown);
            this.textBoxCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCaption_KeyPress);
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
            this.groupBoxSettings.Controls.Add(this.panelControlByOperation);
            this.groupBoxSettings.Controls.Add(this.checkBoxParseVariables);
            this.groupBoxSettings.Controls.Add(this.checkBoxStatus);
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 27);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(197, 142);
            this.groupBoxSettings.TabIndex = 25;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Control";
            // 
            // panelControlByOperation
            // 
            this.panelControlByOperation.Controls.Add(this.checkBoxProcessKeep);
            this.panelControlByOperation.Controls.Add(this.checkBoxWaitForExit);
            this.panelControlByOperation.Controls.Add(this.checkBoxProcessHide);
            this.panelControlByOperation.Location = new System.Drawing.Point(4, 36);
            this.panelControlByOperation.Name = "panelControlByOperation";
            this.panelControlByOperation.Size = new System.Drawing.Size(181, 72);
            this.panelControlByOperation.TabIndex = 18;
            // 
            // checkBoxProcessKeep
            // 
            this.checkBoxProcessKeep.AutoSize = true;
            this.checkBoxProcessKeep.Checked = true;
            this.checkBoxProcessKeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProcessKeep.Enabled = false;
            this.checkBoxProcessKeep.Location = new System.Drawing.Point(8, 51);
            this.checkBoxProcessKeep.Name = "checkBoxProcessKeep";
            this.checkBoxProcessKeep.Size = new System.Drawing.Size(164, 17);
            this.checkBoxProcessKeep.TabIndex = 18;
            this.checkBoxProcessKeep.Text = "Do not close after completion";
            this.checkBoxProcessKeep.UseVisualStyleBackColor = true;
            // 
            // checkBoxWaitForExit
            // 
            this.checkBoxWaitForExit.AutoSize = true;
            this.checkBoxWaitForExit.Checked = true;
            this.checkBoxWaitForExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWaitForExit.Location = new System.Drawing.Point(8, 5);
            this.checkBoxWaitForExit.Name = "checkBoxWaitForExit";
            this.checkBoxWaitForExit.Size = new System.Drawing.Size(145, 17);
            this.checkBoxWaitForExit.TabIndex = 16;
            this.checkBoxWaitForExit.Text = "Wait for completion script";
            this.checkBoxWaitForExit.UseVisualStyleBackColor = true;
            // 
            // checkBoxProcessHide
            // 
            this.checkBoxProcessHide.AutoSize = true;
            this.checkBoxProcessHide.Checked = true;
            this.checkBoxProcessHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProcessHide.Location = new System.Drawing.Point(8, 28);
            this.checkBoxProcessHide.Name = "checkBoxProcessHide";
            this.checkBoxProcessHide.Size = new System.Drawing.Size(88, 17);
            this.checkBoxProcessHide.TabIndex = 17;
            this.checkBoxProcessHide.Text = "Hide process";
            this.checkBoxProcessHide.UseVisualStyleBackColor = true;
            this.checkBoxProcessHide.CheckedChanged += new System.EventHandler(this.checkBoxProcessHide_CheckedChanged);
            // 
            // checkBoxParseVariables
            // 
            this.checkBoxParseVariables.AutoSize = true;
            this.checkBoxParseVariables.Checked = true;
            this.checkBoxParseVariables.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxParseVariables.Location = new System.Drawing.Point(12, 110);
            this.checkBoxParseVariables.Name = "checkBoxParseVariables";
            this.checkBoxParseVariables.Size = new System.Drawing.Size(111, 17);
            this.checkBoxParseVariables.TabIndex = 17;
            this.checkBoxParseVariables.Text = "MSBuild Variables";
            this.checkBoxParseVariables.UseVisualStyleBackColor = true;
            // 
            // checkBoxStatus
            // 
            this.checkBoxStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.checkBoxStatus.Location = new System.Drawing.Point(12, 18);
            this.checkBoxStatus.Name = "checkBoxStatus";
            this.checkBoxStatus.Size = new System.Drawing.Size(145, 17);
            this.checkBoxStatus.TabIndex = 0;
            this.checkBoxStatus.Text = "Disabled";
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
            this.groupBoxOutputControl.Name = "groupBoxOutputControl";
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
            this.dataGridViewOutput.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewOutput.MultiSelect = false;
            this.dataGridViewOutput.Name = "dataGridViewOutput";
            this.dataGridViewOutput.RowHeadersVisible = false;
            this.dataGridViewOutput.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            this.dataGridViewOutput.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dataGridViewOutput.RowTemplate.Height = 17;
            this.dataGridViewOutput.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridViewOutput.Size = new System.Drawing.Size(289, 128);
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
            "`"});
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
            "\\"});
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
            // panelBottom
            // 
            this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBottom.Controls.Add(this.btnDteCmd);
            this.panelBottom.Controls.Add(this.buttonEnvVariables);
            this.panelBottom.Controls.Add(this.labelCaption);
            this.panelBottom.Controls.Add(this.textBoxCaption);
            this.panelBottom.Controls.Add(this.btnExample);
            this.panelBottom.Controls.Add(this.btnClear);
            this.panelBottom.Controls.Add(this.btnApply);
            this.panelBottom.Location = new System.Drawing.Point(340, 440);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(432, 70);
            this.panelBottom.TabIndex = 30;
            // 
            // buttonEnvVariables
            // 
            this.buttonEnvVariables.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonEnvVariables.Location = new System.Drawing.Point(38, 42);
            this.buttonEnvVariables.Name = "buttonEnvVariables";
            this.buttonEnvVariables.Size = new System.Drawing.Size(110, 23);
            this.buttonEnvVariables.TabIndex = 24;
            this.buttonEnvVariables.Text = "MSBuild Properties";
            this.buttonEnvVariables.UseVisualStyleBackColor = true;
            this.buttonEnvVariables.Click += new System.EventHandler(this.buttonEnvVariables_Click);
            // 
            // labelCaption
            // 
            this.labelCaption.AutoSize = true;
            this.labelCaption.Location = new System.Drawing.Point(3, 0);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(92, 13);
            this.labelCaption.TabIndex = 23;
            this.labelCaption.Text = "Caption (optional):";
            // 
            // textBoxCaption
            // 
            this.textBoxCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCaption.Location = new System.Drawing.Point(6, 16);
            this.textBoxCaption.Name = "textBoxCaption";
            this.textBoxCaption.Size = new System.Drawing.Size(419, 20);
            this.textBoxCaption.TabIndex = 22;
            // 
            // btnExample
            // 
            this.btnExample.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExample.Location = new System.Drawing.Point(6, 42);
            this.btnExample.Name = "btnExample";
            this.btnExample.Size = new System.Drawing.Size(26, 23);
            this.btnExample.TabIndex = 21;
            this.btnExample.Text = "?";
            this.btnExample.UseVisualStyleBackColor = true;
            this.btnExample.Click += new System.EventHandler(this.btnExample_Click);
            // 
            // btnClear
            // 
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(258, 42);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(46, 23);
            this.btnClear.TabIndex = 20;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnApply
            // 
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(350, 42);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 19;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnDteCmd
            // 
            this.btnDteCmd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDteCmd.Location = new System.Drawing.Point(154, 42);
            this.btnDteCmd.Name = "btnDteCmd";
            this.btnDteCmd.Size = new System.Drawing.Size(98, 23);
            this.btnDteCmd.TabIndex = 25;
            this.btnDteCmd.Text = "DTE Commands";
            this.btnDteCmd.UseVisualStyleBackColor = true;
            this.btnDteCmd.Click += new System.EventHandler(this.btnDteCmd_Click);
            // 
            // EventsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(772, 509);
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.groupBoxVariants);
            this.Controls.Add(this.groupBoxInterpreter);
            this.Controls.Add(this.groupBoxOutputControl);
            this.Controls.Add(this.groupBoxEW);
            this.Controls.Add(this.panelCommand);
            this.Controls.Add(this.groupBoxPMode);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.comboBoxEvents);
            this.Controls.Add(this.labelToCommandBox);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(780, 400);
            this.Name = "EventsFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Solution Build-Events";
            this.Load += new System.EventHandler(this.EventsFrm_Load);
            this.panelCommand.ResumeLayout(false);
            this.groupBoxPMode.ResumeLayout(false);
            this.groupBoxPMode.PerformLayout();
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.panelControlByOperation.ResumeLayout(false);
            this.panelControlByOperation.PerformLayout();
            this.groupBoxEW.ResumeLayout(false);
            this.groupBoxEW.PerformLayout();
            this.groupBoxOutputControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewOutput)).EndInit();
            this.groupBoxInterpreter.ResumeLayout(false);
            this.groupBoxInterpreter.PerformLayout();
            this.groupBoxVariants.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
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
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonEnvVariables;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.TextBox textBoxCaption;
        private System.Windows.Forms.Button btnExample;
        private System.Windows.Forms.Button btnClear;
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
    }
}