namespace net.r_eg.vsSBE.UI
{
    partial class DTECheckFrm
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
            if(disposing && (components != null)) {
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
            this.btnExecute = new System.Windows.Forms.Button();
            this.groupBoxExecuted = new System.Windows.Forms.GroupBox();
            this.richTextBoxExecuted = new System.Windows.Forms.RichTextBox();
            this.groupBoxCommand = new System.Windows.Forms.GroupBox();
            this.richTextBoxCommand = new System.Windows.Forms.RichTextBox();
            this.groupBoxExecuted.SuspendLayout();
            this.groupBoxCommand.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExecute
            // 
            this.btnExecute.Location = new System.Drawing.Point(12, 111);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(85, 23);
            this.btnExecute.TabIndex = 3;
            this.btnExecute.Text = "Execute";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // groupBoxExecuted
            // 
            this.groupBoxExecuted.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxExecuted.Controls.Add(this.richTextBoxExecuted);
            this.groupBoxExecuted.Location = new System.Drawing.Point(6, 121);
            this.groupBoxExecuted.Name = "groupBoxExecuted";
            this.groupBoxExecuted.Size = new System.Drawing.Size(486, 152);
            this.groupBoxExecuted.TabIndex = 5;
            this.groupBoxExecuted.TabStop = false;
            // 
            // richTextBoxExecuted
            // 
            this.richTextBoxExecuted.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxExecuted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxExecuted.Location = new System.Drawing.Point(3, 16);
            this.richTextBoxExecuted.Name = "richTextBoxExecuted";
            this.richTextBoxExecuted.ReadOnly = true;
            this.richTextBoxExecuted.Size = new System.Drawing.Size(480, 133);
            this.richTextBoxExecuted.TabIndex = 0;
            this.richTextBoxExecuted.Text = "";
            // 
            // groupBoxCommand
            // 
            this.groupBoxCommand.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxCommand.Controls.Add(this.richTextBoxCommand);
            this.groupBoxCommand.Location = new System.Drawing.Point(3, 2);
            this.groupBoxCommand.Name = "groupBoxCommand";
            this.groupBoxCommand.Size = new System.Drawing.Size(489, 102);
            this.groupBoxCommand.TabIndex = 4;
            this.groupBoxCommand.TabStop = false;
            this.groupBoxCommand.Text = "To execution:";
            // 
            // richTextBoxCommand
            // 
            this.richTextBoxCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxCommand.Location = new System.Drawing.Point(3, 16);
            this.richTextBoxCommand.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxCommand.Name = "richTextBoxCommand";
            this.richTextBoxCommand.Size = new System.Drawing.Size(483, 83);
            this.richTextBoxCommand.TabIndex = 0;
            this.richTextBoxCommand.Text = "";
            this.richTextBoxCommand.WordWrap = false;
            this.richTextBoxCommand.Click += new System.EventHandler(this.richTextBoxCommand_Click);
            // 
            // DTECheckFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(497, 278);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.groupBoxExecuted);
            this.Controls.Add(this.groupBoxCommand);
            this.MinimumSize = new System.Drawing.Size(280, 210);
            this.Name = "DTECheckFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Tool:  DTE Commands";
            this.Load += new System.EventHandler(this.DTECheckFrm_Load);
            this.groupBoxExecuted.ResumeLayout(false);
            this.groupBoxCommand.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox groupBoxExecuted;
        private System.Windows.Forms.RichTextBox richTextBoxExecuted;
        private System.Windows.Forms.GroupBox groupBoxCommand;
        private System.Windows.Forms.RichTextBox richTextBoxCommand;
    }
}