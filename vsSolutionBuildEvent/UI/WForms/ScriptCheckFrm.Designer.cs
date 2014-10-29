namespace net.r_eg.vsSBE.UI.WForms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptCheckFrm));
            this.contextMenuUVariables = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemUVarUnsetSel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUVarUnsetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMVertical = new System.Windows.Forms.SplitContainer();
            this.splitContainerMHorizontal = new System.Windows.Forms.SplitContainer();
            this.panelTopMain = new System.Windows.Forms.Panel();
            this.groupBoxCommand = new System.Windows.Forms.GroupBox();
            this.richTextBoxCommand = new System.Windows.Forms.RichTextBox();
            this.panelBottomMain = new System.Windows.Forms.Panel();
            this.checkBoxMSBuildSupport = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBoxExecuted = new System.Windows.Forms.RichTextBox();
            this.groupBoxUVariables = new System.Windows.Forms.GroupBox();
            this.splitContainerUVariables = new System.Windows.Forms.SplitContainer();
            this.listBoxUVariables = new System.Windows.Forms.ListBox();
            this.richTextBoxUVariables = new System.Windows.Forms.RichTextBox();
            this.btnDoc = new System.Windows.Forms.Button();
            this.contextMenuUVariables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMVertical)).BeginInit();
            this.splitContainerMVertical.Panel1.SuspendLayout();
            this.splitContainerMVertical.Panel2.SuspendLayout();
            this.splitContainerMVertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMHorizontal)).BeginInit();
            this.splitContainerMHorizontal.Panel1.SuspendLayout();
            this.splitContainerMHorizontal.Panel2.SuspendLayout();
            this.splitContainerMHorizontal.SuspendLayout();
            this.panelTopMain.SuspendLayout();
            this.groupBoxCommand.SuspendLayout();
            this.panelBottomMain.SuspendLayout();
            this.groupBoxUVariables.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUVariables)).BeginInit();
            this.splitContainerUVariables.Panel1.SuspendLayout();
            this.splitContainerUVariables.Panel2.SuspendLayout();
            this.splitContainerUVariables.SuspendLayout();
            this.SuspendLayout();
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
            // splitContainerMVertical
            // 
            this.splitContainerMVertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMVertical.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMVertical.Name = "splitContainerMVertical";
            // 
            // splitContainerMVertical.Panel1
            // 
            this.splitContainerMVertical.Panel1.Controls.Add(this.splitContainerMHorizontal);
            this.splitContainerMVertical.Panel1MinSize = 300;
            // 
            // splitContainerMVertical.Panel2
            // 
            this.splitContainerMVertical.Panel2.Controls.Add(this.groupBoxUVariables);
            this.splitContainerMVertical.Panel2MinSize = 50;
            this.splitContainerMVertical.Size = new System.Drawing.Size(582, 267);
            this.splitContainerMVertical.SplitterDistance = 427;
            this.splitContainerMVertical.TabIndex = 1;
            // 
            // splitContainerMHorizontal
            // 
            this.splitContainerMHorizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerMHorizontal.Location = new System.Drawing.Point(0, 0);
            this.splitContainerMHorizontal.Name = "splitContainerMHorizontal";
            this.splitContainerMHorizontal.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerMHorizontal.Panel1
            // 
            this.splitContainerMHorizontal.Panel1.Controls.Add(this.panelTopMain);
            this.splitContainerMHorizontal.Panel1.Controls.Add(this.panelBottomMain);
            this.splitContainerMHorizontal.Panel1MinSize = 120;
            // 
            // splitContainerMHorizontal.Panel2
            // 
            this.splitContainerMHorizontal.Panel2.Controls.Add(this.richTextBoxExecuted);
            this.splitContainerMHorizontal.Panel2MinSize = 40;
            this.splitContainerMHorizontal.Size = new System.Drawing.Size(427, 267);
            this.splitContainerMHorizontal.SplitterDistance = 142;
            this.splitContainerMHorizontal.TabIndex = 0;
            // 
            // panelTopMain
            // 
            this.panelTopMain.Controls.Add(this.groupBoxCommand);
            this.panelTopMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTopMain.Location = new System.Drawing.Point(0, 0);
            this.panelTopMain.Name = "panelTopMain";
            this.panelTopMain.Size = new System.Drawing.Size(427, 117);
            this.panelTopMain.TabIndex = 7;
            // 
            // groupBoxCommand
            // 
            this.groupBoxCommand.Controls.Add(this.richTextBoxCommand);
            this.groupBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCommand.Location = new System.Drawing.Point(0, 0);
            this.groupBoxCommand.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxCommand.Name = "groupBoxCommand";
            this.groupBoxCommand.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxCommand.Size = new System.Drawing.Size(427, 117);
            this.groupBoxCommand.TabIndex = 6;
            this.groupBoxCommand.TabStop = false;
            this.groupBoxCommand.Text = "To execution:";
            // 
            // richTextBoxCommand
            // 
            this.richTextBoxCommand.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxCommand.Location = new System.Drawing.Point(0, 13);
            this.richTextBoxCommand.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBoxCommand.Name = "richTextBoxCommand";
            this.richTextBoxCommand.Size = new System.Drawing.Size(427, 104);
            this.richTextBoxCommand.TabIndex = 0;
            this.richTextBoxCommand.Text = "";
            this.richTextBoxCommand.WordWrap = false;
            this.richTextBoxCommand.Click += new System.EventHandler(this.richTextBoxCommand_Click);
            // 
            // panelBottomMain
            // 
            this.panelBottomMain.Controls.Add(this.btnDoc);
            this.panelBottomMain.Controls.Add(this.checkBoxMSBuildSupport);
            this.panelBottomMain.Controls.Add(this.button1);
            this.panelBottomMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomMain.Location = new System.Drawing.Point(0, 117);
            this.panelBottomMain.Name = "panelBottomMain";
            this.panelBottomMain.Size = new System.Drawing.Size(427, 25);
            this.panelBottomMain.TabIndex = 6;
            // 
            // checkBoxMSBuildSupport
            // 
            this.checkBoxMSBuildSupport.AutoSize = true;
            this.checkBoxMSBuildSupport.Location = new System.Drawing.Point(94, 6);
            this.checkBoxMSBuildSupport.Name = "checkBoxMSBuildSupport";
            this.checkBoxMSBuildSupport.Size = new System.Drawing.Size(89, 17);
            this.checkBoxMSBuildSupport.TabIndex = 9;
            this.checkBoxMSBuildSupport.Text = "MSBuild core";
            this.checkBoxMSBuildSupport.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(85, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Execute";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // richTextBoxExecuted
            // 
            this.richTextBoxExecuted.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxExecuted.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxExecuted.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxExecuted.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxExecuted.Name = "richTextBoxExecuted";
            this.richTextBoxExecuted.ReadOnly = true;
            this.richTextBoxExecuted.Size = new System.Drawing.Size(427, 121);
            this.richTextBoxExecuted.TabIndex = 2;
            this.richTextBoxExecuted.Text = "";
            // 
            // groupBoxUVariables
            // 
            this.groupBoxUVariables.Controls.Add(this.splitContainerUVariables);
            this.groupBoxUVariables.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxUVariables.Location = new System.Drawing.Point(0, 0);
            this.groupBoxUVariables.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxUVariables.Name = "groupBoxUVariables";
            this.groupBoxUVariables.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxUVariables.Size = new System.Drawing.Size(151, 267);
            this.groupBoxUVariables.TabIndex = 12;
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
            this.splitContainerUVariables.Panel1MinSize = 140;
            // 
            // splitContainerUVariables.Panel2
            // 
            this.splitContainerUVariables.Panel2.Controls.Add(this.richTextBoxUVariables);
            this.splitContainerUVariables.Panel2MinSize = 40;
            this.splitContainerUVariables.Size = new System.Drawing.Size(151, 254);
            this.splitContainerUVariables.SplitterDistance = 154;
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
            this.listBoxUVariables.Size = new System.Drawing.Size(151, 154);
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
            this.richTextBoxUVariables.Size = new System.Drawing.Size(151, 96);
            this.richTextBoxUVariables.TabIndex = 1;
            this.richTextBoxUVariables.Text = "";
            // 
            // btnDoc
            // 
            this.btnDoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDoc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDoc.Image = ((System.Drawing.Image)(resources.GetObject("btnDoc.Image")));
            this.btnDoc.Location = new System.Drawing.Point(386, 2);
            this.btnDoc.Name = "btnDoc";
            this.btnDoc.Size = new System.Drawing.Size(38, 23);
            this.btnDoc.TabIndex = 10;
            this.btnDoc.UseVisualStyleBackColor = true;
            this.btnDoc.Click += new System.EventHandler(this.btnDoc_Click);
            // 
            // ScriptCheckFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 267);
            this.Controls.Add(this.splitContainerMVertical);
            this.MinimumSize = new System.Drawing.Size(400, 210);
            this.Name = "ScriptCheckFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Tool:  SBE-Scripts";
            this.Load += new System.EventHandler(this.DTECheckFrm_Load);
            this.contextMenuUVariables.ResumeLayout(false);
            this.splitContainerMVertical.Panel1.ResumeLayout(false);
            this.splitContainerMVertical.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMVertical)).EndInit();
            this.splitContainerMVertical.ResumeLayout(false);
            this.splitContainerMHorizontal.Panel1.ResumeLayout(false);
            this.splitContainerMHorizontal.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMHorizontal)).EndInit();
            this.splitContainerMHorizontal.ResumeLayout(false);
            this.panelTopMain.ResumeLayout(false);
            this.groupBoxCommand.ResumeLayout(false);
            this.panelBottomMain.ResumeLayout(false);
            this.panelBottomMain.PerformLayout();
            this.groupBoxUVariables.ResumeLayout(false);
            this.splitContainerUVariables.Panel1.ResumeLayout(false);
            this.splitContainerUVariables.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerUVariables)).EndInit();
            this.splitContainerUVariables.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuUVariables;
        private System.Windows.Forms.ToolStripMenuItem menuItemUVarUnsetSel;
        private System.Windows.Forms.ToolStripMenuItem menuItemUVarUnsetAll;
        private System.Windows.Forms.SplitContainer splitContainerMVertical;
        private System.Windows.Forms.GroupBox groupBoxUVariables;
        private System.Windows.Forms.SplitContainer splitContainerUVariables;
        private System.Windows.Forms.ListBox listBoxUVariables;
        private System.Windows.Forms.RichTextBox richTextBoxUVariables;
        private System.Windows.Forms.SplitContainer splitContainerMHorizontal;
        private System.Windows.Forms.Panel panelTopMain;
        private System.Windows.Forms.GroupBox groupBoxCommand;
        private System.Windows.Forms.RichTextBox richTextBoxCommand;
        private System.Windows.Forms.Panel panelBottomMain;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxMSBuildSupport;
        private System.Windows.Forms.RichTextBox richTextBoxExecuted;
        private System.Windows.Forms.Button btnDoc;
    }
}