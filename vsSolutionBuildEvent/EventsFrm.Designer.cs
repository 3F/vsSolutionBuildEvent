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
            this.comboBoxEvents = new System.Windows.Forms.ComboBox();
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.checkBoxStatus = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnExample = new System.Windows.Forms.Button();
            this.textBoxCaption = new System.Windows.Forms.TextBox();
            this.labelCaption = new System.Windows.Forms.Label();
            this.labelCmd = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBoxEvents
            // 
            this.comboBoxEvents.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxEvents.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBoxEvents.FormattingEnabled = true;
            this.comboBoxEvents.Location = new System.Drawing.Point(2, 2);
            this.comboBoxEvents.Name = "comboBoxEvents";
            this.comboBoxEvents.Size = new System.Drawing.Size(351, 21);
            this.comboBoxEvents.TabIndex = 1;
            this.comboBoxEvents.SelectedIndexChanged += new System.EventHandler(this.comboBoxEvents_SelectedIndexChanged);
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCommand.Location = new System.Drawing.Point(2, 43);
            this.textBoxCommand.Multiline = true;
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.Size = new System.Drawing.Size(419, 145);
            this.textBoxCommand.TabIndex = 3;
            this.textBoxCommand.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCommand_KeyPress);
            // 
            // checkBoxStatus
            // 
            this.checkBoxStatus.AutoSize = true;
            this.checkBoxStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxStatus.Location = new System.Drawing.Point(359, 6);
            this.checkBoxStatus.Name = "checkBoxStatus";
            this.checkBoxStatus.Size = new System.Drawing.Size(64, 17);
            this.checkBoxStatus.TabIndex = 0;
            this.checkBoxStatus.Text = "Disabled";
            this.checkBoxStatus.UseVisualStyleBackColor = true;
            this.checkBoxStatus.CheckedChanged += new System.EventHandler(this.checkBoxStatus_CheckedChanged);
            // 
            // btnApply
            // 
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.Location = new System.Drawing.Point(346, 236);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnClear
            // 
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Location = new System.Drawing.Point(265, 236);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnExample
            // 
            this.btnExample.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExample.Location = new System.Drawing.Point(2, 236);
            this.btnExample.Name = "btnExample";
            this.btnExample.Size = new System.Drawing.Size(100, 23);
            this.btnExample.TabIndex = 6;
            this.btnExample.Text = "Example / detail";
            this.btnExample.UseVisualStyleBackColor = true;
            this.btnExample.Click += new System.EventHandler(this.btnExample_Click);
            // 
            // textBoxCaption
            // 
            this.textBoxCaption.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxCaption.Location = new System.Drawing.Point(2, 210);
            this.textBoxCaption.Name = "textBoxCaption";
            this.textBoxCaption.Size = new System.Drawing.Size(419, 20);
            this.textBoxCaption.TabIndex = 7;
            this.textBoxCaption.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxCaption_KeyPress);
            // 
            // labelCaption
            // 
            this.labelCaption.AutoSize = true;
            this.labelCaption.Location = new System.Drawing.Point(-1, 194);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Size = new System.Drawing.Size(92, 13);
            this.labelCaption.TabIndex = 8;
            this.labelCaption.Text = "Caption (optional):";
            // 
            // labelCmd
            // 
            this.labelCmd.AutoSize = true;
            this.labelCmd.Location = new System.Drawing.Point(-1, 27);
            this.labelCmd.Name = "labelCmd";
            this.labelCmd.Size = new System.Drawing.Size(85, 13);
            this.labelCmd.TabIndex = 9;
            this.labelCmd.Text = "Command script:";
            // 
            // EventsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(422, 264);
            this.Controls.Add(this.labelCmd);
            this.Controls.Add(this.labelCaption);
            this.Controls.Add(this.textBoxCaption);
            this.Controls.Add(this.btnExample);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.checkBoxStatus);
            this.Controls.Add(this.textBoxCommand);
            this.Controls.Add(this.comboBoxEvents);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EventsFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Solution BuildEvent\'s :: scripts for all projects";
            this.Load += new System.EventHandler(this.EventsFrm_Load);
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
        private System.Windows.Forms.Label labelCmd;
    }
}