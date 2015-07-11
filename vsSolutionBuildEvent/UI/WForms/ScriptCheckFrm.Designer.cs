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
            this.mItemUVarEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemUVarUnsetSel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemUVarUnsetAll = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainerMVertical = new System.Windows.Forms.SplitContainer();
            this.splitContainerMHorizontal = new System.Windows.Forms.SplitContainer();
            this.panelTopMain = new System.Windows.Forms.Panel();
            this.groupBoxCommand = new System.Windows.Forms.GroupBox();
            this.textEditor = new net.r_eg.vsSBE.UI.WForms.Controls.TextEditor();
            this.panelBottomMain = new System.Windows.Forms.Panel();
            this.btnDoc = new System.Windows.Forms.Button();
            this.checkBoxMSBuildSupport = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.richTextBoxExecuted = new System.Windows.Forms.RichTextBox();
            this.groupBoxUVariables = new System.Windows.Forms.GroupBox();
            this.splitContainerUVariables = new System.Windows.Forms.SplitContainer();
            this.listBoxUVariables = new System.Windows.Forms.ListBox();
            this.splitContainerComponents = new System.Windows.Forms.SplitContainer();
            this.richTextBoxUVariables = new System.Windows.Forms.RichTextBox();
            this.groupBoxComponents = new System.Windows.Forms.GroupBox();
            this.chkListComponents = new System.Windows.Forms.CheckedListBox();
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
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerComponents)).BeginInit();
            this.splitContainerComponents.Panel1.SuspendLayout();
            this.splitContainerComponents.Panel2.SuspendLayout();
            this.splitContainerComponents.SuspendLayout();
            this.groupBoxComponents.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuUVariables
            // 
            this.contextMenuUVariables.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mItemUVarEdit,
            this.toolStripSeparator1,
            this.menuItemUVarUnsetSel,
            this.menuItemUVarUnsetAll});
            this.contextMenuUVariables.Name = "contextMenuUVariables";
            this.contextMenuUVariables.Size = new System.Drawing.Size(206, 76);
            // 
            // mItemUVarEdit
            // 
            this.mItemUVarEdit.Name = "mItemUVarEdit";
            this.mItemUVarEdit.Size = new System.Drawing.Size(205, 22);
            this.mItemUVarEdit.Text = "Edit with evaluated value";
            this.mItemUVarEdit.Click += new System.EventHandler(this.mItemUVarEdit_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(202, 6);
            // 
            // menuItemUVarUnsetSel
            // 
            this.menuItemUVarUnsetSel.Name = "menuItemUVarUnsetSel";
            this.menuItemUVarUnsetSel.Size = new System.Drawing.Size(205, 22);
            this.menuItemUVarUnsetSel.Text = "Unset Selected";
            this.menuItemUVarUnsetSel.Click += new System.EventHandler(this.menuItemUVarUnsetSel_Click);
            // 
            // menuItemUVarUnsetAll
            // 
            this.menuItemUVarUnsetAll.Name = "menuItemUVarUnsetAll";
            this.menuItemUVarUnsetAll.Size = new System.Drawing.Size(205, 22);
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
            this.splitContainerMVertical.Size = new System.Drawing.Size(750, 360);
            this.splitContainerMVertical.SplitterDistance = 550;
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
            this.splitContainerMHorizontal.Size = new System.Drawing.Size(550, 360);
            this.splitContainerMHorizontal.SplitterDistance = 191;
            this.splitContainerMHorizontal.TabIndex = 0;
            // 
            // panelTopMain
            // 
            this.panelTopMain.Controls.Add(this.groupBoxCommand);
            this.panelTopMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTopMain.Location = new System.Drawing.Point(0, 0);
            this.panelTopMain.Name = "panelTopMain";
            this.panelTopMain.Size = new System.Drawing.Size(550, 166);
            this.panelTopMain.TabIndex = 7;
            // 
            // groupBoxCommand
            // 
            this.groupBoxCommand.Controls.Add(this.textEditor);
            this.groupBoxCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCommand.Location = new System.Drawing.Point(0, 0);
            this.groupBoxCommand.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxCommand.Name = "groupBoxCommand";
            this.groupBoxCommand.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxCommand.Size = new System.Drawing.Size(550, 166);
            this.groupBoxCommand.TabIndex = 6;
            this.groupBoxCommand.TabStop = false;
            this.groupBoxCommand.Text = "To execution:";
            // 
            // textEditor
            // 
            this.textEditor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.textEditor.CodeCompletionEnabled = true;
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.Location = new System.Drawing.Point(0, 13);
            this.textEditor.Name = "textEditor";
            this.textEditor.Size = new System.Drawing.Size(550, 153);
            this.textEditor.TabIndex = 3;
            // 
            // panelBottomMain
            // 
            this.panelBottomMain.Controls.Add(this.btnDoc);
            this.panelBottomMain.Controls.Add(this.checkBoxMSBuildSupport);
            this.panelBottomMain.Controls.Add(this.button1);
            this.panelBottomMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottomMain.Location = new System.Drawing.Point(0, 166);
            this.panelBottomMain.Name = "panelBottomMain";
            this.panelBottomMain.Size = new System.Drawing.Size(550, 25);
            this.panelBottomMain.TabIndex = 6;
            // 
            // btnDoc
            // 
            this.btnDoc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDoc.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDoc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDoc.Image = ((System.Drawing.Image)(resources.GetObject("btnDoc.Image")));
            this.btnDoc.Location = new System.Drawing.Point(509, 2);
            this.btnDoc.Name = "btnDoc";
            this.btnDoc.Size = new System.Drawing.Size(38, 23);
            this.btnDoc.TabIndex = 10;
            this.btnDoc.UseVisualStyleBackColor = true;
            this.btnDoc.Click += new System.EventHandler(this.btnDoc_Click);
            // 
            // checkBoxMSBuildSupport
            // 
            this.checkBoxMSBuildSupport.AutoSize = true;
            this.checkBoxMSBuildSupport.Checked = true;
            this.checkBoxMSBuildSupport.CheckState = System.Windows.Forms.CheckState.Checked;
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
            this.richTextBoxExecuted.Size = new System.Drawing.Size(550, 165);
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
            this.groupBoxUVariables.Size = new System.Drawing.Size(196, 360);
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
            this.splitContainerUVariables.Panel2.Controls.Add(this.splitContainerComponents);
            this.splitContainerUVariables.Panel2MinSize = 40;
            this.splitContainerUVariables.Size = new System.Drawing.Size(196, 347);
            this.splitContainerUVariables.SplitterDistance = 140;
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
            this.listBoxUVariables.Size = new System.Drawing.Size(196, 140);
            this.listBoxUVariables.TabIndex = 0;
            this.listBoxUVariables.SelectedIndexChanged += new System.EventHandler(this.listBoxUVariables_SelectedIndexChanged);
            this.listBoxUVariables.DoubleClick += new System.EventHandler(this.listBoxUVariables_DoubleClick);
            // 
            // splitContainerComponents
            // 
            this.splitContainerComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerComponents.Location = new System.Drawing.Point(0, 0);
            this.splitContainerComponents.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerComponents.Name = "splitContainerComponents";
            this.splitContainerComponents.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerComponents.Panel1
            // 
            this.splitContainerComponents.Panel1.Controls.Add(this.richTextBoxUVariables);
            // 
            // splitContainerComponents.Panel2
            // 
            this.splitContainerComponents.Panel2.Controls.Add(this.groupBoxComponents);
            this.splitContainerComponents.Size = new System.Drawing.Size(196, 203);
            this.splitContainerComponents.SplitterDistance = 66;
            this.splitContainerComponents.TabIndex = 2;
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
            this.richTextBoxUVariables.Size = new System.Drawing.Size(196, 66);
            this.richTextBoxUVariables.TabIndex = 1;
            this.richTextBoxUVariables.Text = "";
            this.richTextBoxUVariables.Leave += new System.EventHandler(this.richTextBoxUVariables_Leave);
            // 
            // groupBoxComponents
            // 
            this.groupBoxComponents.Controls.Add(this.chkListComponents);
            this.groupBoxComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxComponents.Location = new System.Drawing.Point(0, 0);
            this.groupBoxComponents.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxComponents.Name = "groupBoxComponents";
            this.groupBoxComponents.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxComponents.Size = new System.Drawing.Size(196, 133);
            this.groupBoxComponents.TabIndex = 0;
            this.groupBoxComponents.TabStop = false;
            this.groupBoxComponents.Text = "Components:";
            // 
            // chkListComponents
            // 
            this.chkListComponents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chkListComponents.FormattingEnabled = true;
            this.chkListComponents.IntegralHeight = false;
            this.chkListComponents.Location = new System.Drawing.Point(0, 13);
            this.chkListComponents.Margin = new System.Windows.Forms.Padding(0);
            this.chkListComponents.Name = "chkListComponents";
            this.chkListComponents.Size = new System.Drawing.Size(196, 120);
            this.chkListComponents.TabIndex = 0;
            this.chkListComponents.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.chkListComponents_ItemCheck);
            // 
            // ScriptCheckFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 360);
            this.Controls.Add(this.splitContainerMVertical);
            this.MinimumSize = new System.Drawing.Size(400, 210);
            this.Name = "ScriptCheckFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Tool:  SBE-Scripts";
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
            this.splitContainerComponents.Panel1.ResumeLayout(false);
            this.splitContainerComponents.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerComponents)).EndInit();
            this.splitContainerComponents.ResumeLayout(false);
            this.groupBoxComponents.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panelBottomMain;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBoxMSBuildSupport;
        private System.Windows.Forms.RichTextBox richTextBoxExecuted;
        private System.Windows.Forms.Button btnDoc;
        private Controls.TextEditor textEditor;
        private System.Windows.Forms.SplitContainer splitContainerComponents;
        private System.Windows.Forms.GroupBox groupBoxComponents;
        private System.Windows.Forms.CheckedListBox chkListComponents;
        private System.Windows.Forms.ToolStripMenuItem mItemUVarEdit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}