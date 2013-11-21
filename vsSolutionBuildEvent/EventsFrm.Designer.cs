namespace reg.ext.vsSolutionBuildEvent
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
            this.comboBoxEvents = new System.Windows.Forms.ComboBox();
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.checkBoxStatus = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExample = new System.Windows.Forms.Button();
            this.textBoxCaption = new System.Windows.Forms.TextBox();
            this.labelCaption = new System.Windows.Forms.Label();
            this.labelToInterpreterMode = new System.Windows.Forms.Label();
            this.checkBoxWaitForExit = new System.Windows.Forms.CheckBox();
            this.checkBoxProcessHide = new System.Windows.Forms.CheckBox();
            this.groupBoxSettings = new System.Windows.Forms.GroupBox();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.comboBoxWrapper = new System.Windows.Forms.ComboBox();
            this.labelWrapper = new System.Windows.Forms.Label();
            this.labelTreatNewline = new System.Windows.Forms.Label();
            this.comboBoxNewline = new System.Windows.Forms.ComboBox();
            this.comboBoxInterpreter = new System.Windows.Forms.ComboBox();
            this.radioModeScript = new System.Windows.Forms.RadioButton();
            this.radioModeFiles = new System.Windows.Forms.RadioButton();
            this.checkBoxProcessKeep = new System.Windows.Forms.CheckBox();
            this.labelToFilesMode = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBoxSettings.SuspendLayout();
            this.groupBoxMode.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBoxEvents
            // 
            this.comboBoxEvents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEvents.FormattingEnabled = true;
            this.comboBoxEvents.Location = new System.Drawing.Point(0, 0);
            this.comboBoxEvents.Name = "comboBoxEvents";
            this.comboBoxEvents.Size = new System.Drawing.Size(425, 21);
            this.comboBoxEvents.TabIndex = 1;
            this.comboBoxEvents.SelectedIndexChanged += new System.EventHandler(this.comboBoxEvents_SelectedIndexChanged);
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCommand.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.textBoxCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCommand.Location = new System.Drawing.Point(3, 222);
            this.textBoxCommand.Multiline = true;
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.Size = new System.Drawing.Size(419, 125);
            this.textBoxCommand.TabIndex = 3;
            this.textBoxCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCommand_KeyPress);
            // 
            // checkBoxStatus
            // 
            this.checkBoxStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxStatus.Location = new System.Drawing.Point(3, 199);
            this.checkBoxStatus.Name = "checkBoxStatus";
            this.checkBoxStatus.Size = new System.Drawing.Size(15, 17);
            this.checkBoxStatus.TabIndex = 0;
            this.checkBoxStatus.Text = "Disabled";
            this.checkBoxStatus.UseVisualStyleBackColor = true;
            this.checkBoxStatus.CheckedChanged += new System.EventHandler(this.checkBoxStatus_CheckedChanged);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(347, 395);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(109, 395);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnExample
            // 
            this.btnExample.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnExample.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExample.Location = new System.Drawing.Point(3, 395);
            this.btnExample.Name = "btnExample";
            this.btnExample.Size = new System.Drawing.Size(100, 23);
            this.btnExample.TabIndex = 6;
            this.btnExample.Text = "Example / detail";
            this.btnExample.UseVisualStyleBackColor = true;
            this.btnExample.Click += new System.EventHandler(this.btnExample_Click);
            // 
            // textBoxCaption
            // 
            this.textBoxCaption.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.textBoxCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCaption.Location = new System.Drawing.Point(3, 369);
            this.textBoxCaption.Name = "textBoxCaption";
            this.textBoxCaption.Size = new System.Drawing.Size(419, 20);
            this.textBoxCaption.TabIndex = 7;
            this.textBoxCaption.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCaption_KeyPress);
            // 
            // labelCaption
            // 
            this.labelCaption.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelCaption.AutoSize = true;
            this.labelCaption.Location = new System.Drawing.Point(0, 353);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(92, 13);
            this.labelCaption.TabIndex = 8;
            this.labelCaption.Text = "Caption (optional):";
            // 
            // labelToInterpreterMode
            // 
            this.labelToInterpreterMode.AutoSize = true;
            this.labelToInterpreterMode.Location = new System.Drawing.Point(18, 201);
            this.labelToInterpreterMode.Name = "labelToInterpreterMode";
            this.labelToInterpreterMode.Size = new System.Drawing.Size(85, 13);
            this.labelToInterpreterMode.TabIndex = 9;
            this.labelToInterpreterMode.Text = "Command script:";
            // 
            // checkBoxWaitForExit
            // 
            this.checkBoxWaitForExit.AutoSize = true;
            this.checkBoxWaitForExit.Checked = true;
            this.checkBoxWaitForExit.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxWaitForExit.Location = new System.Drawing.Point(12, 19);
            this.checkBoxWaitForExit.Name = "checkBoxWaitForExit";
            this.checkBoxWaitForExit.Size = new System.Drawing.Size(189, 17);
            this.checkBoxWaitForExit.TabIndex = 13;
            this.checkBoxWaitForExit.Text = "wait until terminates script handling";
            this.checkBoxWaitForExit.UseVisualStyleBackColor = true;
            // 
            // checkBoxProcessHide
            // 
            this.checkBoxProcessHide.AutoSize = true;
            this.checkBoxProcessHide.Checked = true;
            this.checkBoxProcessHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProcessHide.Location = new System.Drawing.Point(12, 42);
            this.checkBoxProcessHide.Name = "checkBoxProcessHide";
            this.checkBoxProcessHide.Size = new System.Drawing.Size(86, 17);
            this.checkBoxProcessHide.TabIndex = 14;
            this.checkBoxProcessHide.Text = "hide process";
            this.checkBoxProcessHide.UseVisualStyleBackColor = true;
            this.checkBoxProcessHide.CheckedChanged += new System.EventHandler(this.checkBoxProcessHide_CheckedChanged);
            // 
            // groupBoxSettings
            // 
            this.groupBoxSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxSettings.Controls.Add(this.groupBoxMode);
            this.groupBoxSettings.Controls.Add(this.radioModeScript);
            this.groupBoxSettings.Controls.Add(this.radioModeFiles);
            this.groupBoxSettings.Controls.Add(this.checkBoxProcessKeep);
            this.groupBoxSettings.Controls.Add(this.checkBoxWaitForExit);
            this.groupBoxSettings.Controls.Add(this.checkBoxProcessHide);
            this.groupBoxSettings.Location = new System.Drawing.Point(0, 27);
            this.groupBoxSettings.Name = "groupBoxSettings";
            this.groupBoxSettings.Size = new System.Drawing.Size(425, 165);
            this.groupBoxSettings.TabIndex = 14;
            this.groupBoxSettings.TabStop = false;
            this.groupBoxSettings.Text = "Settings";
            // 
            // groupBoxMode
            // 
            this.groupBoxMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxMode.Controls.Add(this.comboBoxWrapper);
            this.groupBoxMode.Controls.Add(this.labelWrapper);
            this.groupBoxMode.Controls.Add(this.labelTreatNewline);
            this.groupBoxMode.Controls.Add(this.comboBoxNewline);
            this.groupBoxMode.Controls.Add(this.comboBoxInterpreter);
            this.groupBoxMode.Location = new System.Drawing.Point(12, 80);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Size = new System.Drawing.Size(410, 81);
            this.groupBoxMode.TabIndex = 15;
            this.groupBoxMode.TabStop = false;
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
            this.comboBoxWrapper.Location = new System.Drawing.Point(101, 56);
            this.comboBoxWrapper.Name = "comboBoxWrapper";
            this.comboBoxWrapper.Size = new System.Drawing.Size(309, 21);
            this.comboBoxWrapper.TabIndex = 23;
            // 
            // labelWrapper
            // 
            this.labelWrapper.AutoSize = true;
            this.labelWrapper.Location = new System.Drawing.Point(11, 56);
            this.labelWrapper.Name = "labelWrapper";
            this.labelWrapper.Size = new System.Drawing.Size(48, 13);
            this.labelWrapper.TabIndex = 22;
            this.labelWrapper.Text = "wrapper:";
            // 
            // labelTreatNewline
            // 
            this.labelTreatNewline.AutoSize = true;
            this.labelTreatNewline.Location = new System.Drawing.Point(11, 34);
            this.labelTreatNewline.Name = "labelTreatNewline";
            this.labelTreatNewline.Size = new System.Drawing.Size(84, 13);
            this.labelTreatNewline.TabIndex = 21;
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
            this.comboBoxNewline.Location = new System.Drawing.Point(101, 31);
            this.comboBoxNewline.Name = "comboBoxNewline";
            this.comboBoxNewline.Size = new System.Drawing.Size(309, 21);
            this.comboBoxNewline.TabIndex = 20;
            // 
            // comboBoxInterpreter
            // 
            this.comboBoxInterpreter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxInterpreter.FormattingEnabled = true;
            this.comboBoxInterpreter.Items.AddRange(new object[] {
            "cmd.exe /C",
            "php -r"});
            this.comboBoxInterpreter.Location = new System.Drawing.Point(0, 6);
            this.comboBoxInterpreter.Name = "comboBoxInterpreter";
            this.comboBoxInterpreter.Size = new System.Drawing.Size(410, 21);
            this.comboBoxInterpreter.TabIndex = 19;
            // 
            // radioModeScript
            // 
            this.radioModeScript.AutoSize = true;
            this.radioModeScript.Checked = true;
            this.radioModeScript.Location = new System.Drawing.Point(95, 64);
            this.radioModeScript.Name = "radioModeScript";
            this.radioModeScript.Size = new System.Drawing.Size(106, 17);
            this.radioModeScript.TabIndex = 20;
            this.radioModeScript.TabStop = true;
            this.radioModeScript.Text = "Interpreter Mode:";
            this.radioModeScript.UseVisualStyleBackColor = true;
            this.radioModeScript.CheckedChanged += new System.EventHandler(this.radioModeScript_CheckedChanged);
            // 
            // radioModeFiles
            // 
            this.radioModeFiles.AutoSize = true;
            this.radioModeFiles.Location = new System.Drawing.Point(12, 64);
            this.radioModeFiles.Name = "radioModeFiles";
            this.radioModeFiles.Size = new System.Drawing.Size(87, 17);
            this.radioModeFiles.TabIndex = 21;
            this.radioModeFiles.Text = "Files Mode / ";
            this.radioModeFiles.UseVisualStyleBackColor = true;
            this.radioModeFiles.CheckedChanged += new System.EventHandler(this.radioModeFiles_CheckedChanged);
            // 
            // checkBoxProcessKeep
            // 
            this.checkBoxProcessKeep.AutoSize = true;
            this.checkBoxProcessKeep.Checked = true;
            this.checkBoxProcessKeep.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxProcessKeep.Enabled = false;
            this.checkBoxProcessKeep.Location = new System.Drawing.Point(109, 42);
            this.checkBoxProcessKeep.Name = "checkBoxProcessKeep";
            this.checkBoxProcessKeep.Size = new System.Drawing.Size(147, 17);
            this.checkBoxProcessKeep.TabIndex = 15;
            this.checkBoxProcessKeep.Text = "not close after completion";
            this.checkBoxProcessKeep.UseVisualStyleBackColor = true;
            // 
            // labelToFilesMode
            // 
            this.labelToFilesMode.AutoSize = true;
            this.labelToFilesMode.Location = new System.Drawing.Point(18, 201);
            this.labelToFilesMode.Name = "labelToFilesMode";
            this.labelToFilesMode.Size = new System.Drawing.Size(201, 13);
            this.labelToFilesMode.TabIndex = 15;
            this.labelToFilesMode.Text = "Files to execute (separated by enter key):";
            this.labelToFilesMode.Visible = false;
            // 
            // EventsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 422);
            this.Controls.Add(this.groupBoxSettings);
            this.Controls.Add(this.labelToInterpreterMode);
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.textBoxCaption);
            this.Controls.Add(this.btnExample);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.checkBoxStatus);
            this.Controls.Add(this.textBoxCommand);
            this.Controls.Add(this.comboBoxEvents);
            this.Controls.Add(this.labelToFilesMode);
            this.MinimizeBox = false;
            this.Name = "EventsFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Solution Build-Events";
            this.Load += new System.EventHandler(this.EventsFrm_Load);
            this.groupBoxSettings.ResumeLayout(false);
            this.groupBoxSettings.PerformLayout();
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxEvents;
        private System.Windows.Forms.TextBox textBoxCommand;
        private System.Windows.Forms.CheckBox checkBoxStatus;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExample;
        private System.Windows.Forms.TextBox textBoxCaption;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Label labelToInterpreterMode;
        private System.Windows.Forms.CheckBox checkBoxWaitForExit;
        private System.Windows.Forms.CheckBox checkBoxProcessHide;
        private System.Windows.Forms.GroupBox groupBoxSettings;
        private System.Windows.Forms.CheckBox checkBoxProcessKeep;
        private System.Windows.Forms.GroupBox groupBoxMode;
        private System.Windows.Forms.Label labelTreatNewline;
        private System.Windows.Forms.ComboBox comboBoxNewline;
        private System.Windows.Forms.ComboBox comboBoxInterpreter;
        private System.Windows.Forms.RadioButton radioModeScript;
        private System.Windows.Forms.RadioButton radioModeFiles;
        private System.Windows.Forms.Label labelToFilesMode;
        private System.Windows.Forms.ComboBox comboBoxWrapper;
        private System.Windows.Forms.Label labelWrapper;
        private System.Windows.Forms.ToolTip toolTip;
    }
}