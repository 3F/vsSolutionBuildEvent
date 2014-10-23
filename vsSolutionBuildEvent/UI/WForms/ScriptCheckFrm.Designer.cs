namespace net.r_eg.vsSBE.UI
{
    partial class ScriptCheckFrm
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
            this.components = new System.ComponentModel.Container();
            this.btnExecute = new System.Windows.Forms.Button();
            this.groupBoxExecuted = new System.Windows.Forms.GroupBox();
            this.richTextBoxExecuted = new System.Windows.Forms.RichTextBox();
            this.groupBoxCommand = new System.Windows.Forms.GroupBox();
            this.richTextBoxCommand = new System.Windows.Forms.RichTextBox();
            this.groupBoxUVariables = new System.Windows.Forms.GroupBox();
            this.splitContainerUVariables = new System.Windows.Forms.SplitContainer();
            this.listBoxUVariables = new System.Windows.Forms.ListBox();
            this.richTextBoxUVariables = new System.Windows.Forms.RichTextBox();
            this.contextMenuUVariables = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemUVarUnsetSel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUVarUnsetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.checkBoxMSBuildSupport = new System.Windows.Forms.CheckBox();
            this.groupBoxExecuted.SuspendLayout();
            this.groupBoxCommand.SuspendLayout();
            this.groupBoxUVariables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUVariables)).BeginInit();
            this.splitContainerUVariables.Panel1.SuspendLayout();
            this.splitContainerUVariables.Panel2.SuspendLayout();
            this.splitContainerUVariables.SuspendLayout();
            this.contextMenuUVariables.SuspendLayout();
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
            this.groupBoxExecuted.Size = new System.Drawing.Size(430, 141);
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
            this.richTextBoxExecuted.Size = new System.Drawing.Size(424, 122);
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
            this.groupBoxCommand.Size = new System.Drawing.Size(433, 102);
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
            this.richTextBoxCommand.Size = new System.Drawing.Size(427, 83);
            this.richTextBoxCommand.TabIndex = 0;
            this.richTextBoxCommand.Text = "";
            this.richTextBoxCommand.WordWrap = false;
            this.richTextBoxCommand.Click += new System.EventHandler(this.richTextBoxCommand_Click);
            // 
            // groupBoxUVariables
            // 
            this.groupBoxUVariables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxUVariables.Controls.Add(this.splitContainerUVariables);
            this.groupBoxUVariables.Location = new System.Drawing.Point(442, 2);
            this.groupBoxUVariables.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxUVariables.Name = "groupBoxUVariables";
            this.groupBoxUVariables.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxUVariables.Size = new System.Drawing.Size(139, 260);
            this.groupBoxUVariables.TabIndex = 6;
            this.groupBoxUVariables.TabStop = false;
            this.groupBoxUVariables.Text = "User-Variables:";
            // 
            // splitContainerUVariables
            // 
            this.splitContainerUVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerUVariables.Location = new System.Drawing.Point(0, 13);
            this.splitContainerUVariables.Name = "splitContainerUVariables";
            this.splitContainerUVariables.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerUVariables.Panel1
            // 
            this.splitContainerUVariables.Panel1.Controls.Add(this.listBoxUVariables);
            // 
            // splitContainerUVariables.Panel2
            // 
            this.splitContainerUVariables.Panel2.Controls.Add(this.richTextBoxUVariables);
            this.splitContainerUVariables.Size = new System.Drawing.Size(139, 247);
            this.splitContainerUVariables.SplitterDistance = 157;
            this.splitContainerUVariables.TabIndex = 0;
            // 
            // listBoxUVariables
            // 
            this.listBoxUVariables.ContextMenuStrip = this.contextMenuUVariables;
            this.listBoxUVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxUVariables.FormattingEnabled = true;
            this.listBoxUVariables.IntegralHeight = false;
            this.listBoxUVariables.Location = new System.Drawing.Point(0, 0);
            this.listBoxUVariables.Margin = new System.Windows.Forms.Padding(0);
            this.listBoxUVariables.Name = "listBoxUVariables";
            this.listBoxUVariables.Size = new System.Drawing.Size(139, 157);
            this.listBoxUVariables.TabIndex = 0;
            this.listBoxUVariables.SelectedIndexChanged += new System.EventHandler(this.listBoxUVariables_SelectedIndexChanged);
            // 
            // richTextBoxUVariables
            // 
            this.richTextBoxUVariables.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxUVariables.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxUVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxUVariables.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxUVariables.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxUVariables.Name = "richTextBoxUVariables";
            this.richTextBoxUVariables.ReadOnly = true;
            this.richTextBoxUVariables.Size = new System.Drawing.Size(139, 86);
            this.richTextBoxUVariables.TabIndex = 1;
            this.richTextBoxUVariables.Text = "";
            // 
            // contextMenuUVariables
            // 
            this.contextMenuUVariables.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemUVarUnsetSel,
            this.menuItemUVarUnsetAll});
            this.contextMenuUVariables.Name = "contextMenuUVariables";
            this.contextMenuUVariables.Size = new System.Drawing.Size(172, 48);
            // 
            // menuItemUVarUnsetSel
            // 
            this.menuItemUVarUnsetSel.Name = "menuItemUVarUnsetSel";
            this.menuItemUVarUnsetSel.Size = new System.Drawing.Size(171, 22);
            this.menuItemUVarUnsetSel.Text = "Unset Selected";
            this.menuItemUVarUnsetSel.Click += new System.EventHandler(this.menuItemUVarUnsetSel_Click);
            // 
            // menuItemUVarUnsetAll
            // 
            this.menuItemUVarUnsetAll.Name = "menuItemUVarUnsetAll";
            this.menuItemUVarUnsetAll.Size = new System.Drawing.Size(171, 22);
            this.menuItemUVarUnsetAll.Text = "Unset All Variables";
            this.menuItemUVarUnsetAll.Click += new System.EventHandler(this.menuItemUVarUnsetAll_Click);
            // 
            // checkBoxMSBuildSupport
            // 
            this.checkBoxMSBuildSupport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxMSBuildSupport.AutoSize = true;
            this.checkBoxMSBuildSupport.Location = new System.Drawing.Point(336, 104);
            this.checkBoxMSBuildSupport.Name = "checkBoxMSBuildSupport";
            this.checkBoxMSBuildSupport.Size = new System.Drawing.Size(103, 17);
            this.checkBoxMSBuildSupport.TabIndex = 8;
            this.checkBoxMSBuildSupport.Text = "MSBuild support";
            this.checkBoxMSBuildSupport.UseVisualStyleBackColor = true;
            // 
            // ScriptCheckFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 267);
            this.Controls.Add(this.checkBoxMSBuildSupport);
            this.Controls.Add(this.groupBoxUVariables);
            this.Controls.Add(this.btnExecute);
            this.Controls.Add(this.groupBoxExecuted);
            this.Controls.Add(this.groupBoxCommand);
            this.MinimumSize = new System.Drawing.Size(400, 210);
            this.Name = "ScriptCheckFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Tool:  SBE-Scripts";
            this.Load += new System.EventHandler(this.DTECheckFrm_Load);
            this.groupBoxExecuted.ResumeLayout(false);
            this.groupBoxCommand.ResumeLayout(false);
            this.groupBoxUVariables.ResumeLayout(false);
            this.splitContainerUVariables.Panel1.ResumeLayout(false);
            this.splitContainerUVariables.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUVariables)).EndInit();
            this.splitContainerUVariables.ResumeLayout(false);
            this.contextMenuUVariables.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExecute;
        private System.Windows.Forms.GroupBox groupBoxExecuted;
        private System.Windows.Forms.RichTextBox richTextBoxExecuted;
        private System.Windows.Forms.GroupBox groupBoxCommand;
        private System.Windows.Forms.RichTextBox richTextBoxCommand;
        private System.Windows.Forms.GroupBox groupBoxUVariables;
        private System.Windows.Forms.RichTextBox richTextBoxUVariables;
        private System.Windows.Forms.ListBox listBoxUVariables;
        private System.Windows.Forms.SplitContainer splitContainerUVariables;
        private System.Windows.Forms.ContextMenuStrip contextMenuUVariables;
        private System.Windows.Forms.ToolStripMenuItem menuItemUVarUnsetSel;
        private System.Windows.Forms.ToolStripMenuItem menuItemUVarUnsetAll;
        private System.Windows.Forms.CheckBox checkBoxMSBuildSupport;
    }
}