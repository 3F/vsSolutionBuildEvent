namespace net.r_eg.vsSBE.UI.WForms
{
    partial class EnvDteSniffer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.chkActivate = new System.Windows.Forms.CheckBox();
            this.groupBoxCESniffer = new System.Windows.Forms.GroupBox();
            this.btnVSCE = new System.Windows.Forms.Button();
            this.buttonFlush = new System.Windows.Forms.Button();
            this.labelSep1 = new System.Windows.Forms.Label();
            this.lightsTraffic = new net.r_eg.vsSBE.UI.WForms.Controls.Lights();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.dgvCESniffer = new net.r_eg.vsSBE.UI.WForms.Components.DataGridViewExt();
            this.dgvCESnifferColumnStamp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCESnifferColumnPre = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvCESnifferColumnGuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCESnifferColumnId = new net.r_eg.vsSBE.UI.WForms.Components.DataGridViewExt.NumericColumn();
            this.dgvCESnifferColumnCustomIn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCESnifferColumnCustomOut = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCESnifferColumnEnum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuFlush = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxCESniffer.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCESniffer)).BeginInit();
            this.contextMenuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkActivate
            // 
            this.chkActivate.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkActivate.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
            this.chkActivate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkActivate.Location = new System.Drawing.Point(3, 10);
            this.chkActivate.Name = "chkActivate";
            this.chkActivate.Size = new System.Drawing.Size(64, 23);
            this.chkActivate.TabIndex = 8;
            this.chkActivate.Text = "Activate";
            this.chkActivate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkActivate.UseVisualStyleBackColor = true;
            this.chkActivate.CheckedChanged += new System.EventHandler(this.chkActivate_CheckedChanged);
            // 
            // groupBoxCESniffer
            // 
            this.groupBoxCESniffer.Controls.Add(this.btnVSCE);
            this.groupBoxCESniffer.Controls.Add(this.buttonFlush);
            this.groupBoxCESniffer.Controls.Add(this.labelSep1);
            this.groupBoxCESniffer.Controls.Add(this.lightsTraffic);
            this.groupBoxCESniffer.Controls.Add(this.chkActivate);
            this.groupBoxCESniffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxCESniffer.Location = new System.Drawing.Point(2, 2);
            this.groupBoxCESniffer.Margin = new System.Windows.Forms.Padding(0);
            this.groupBoxCESniffer.Name = "groupBoxCESniffer";
            this.groupBoxCESniffer.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxCESniffer.Size = new System.Drawing.Size(743, 37);
            this.groupBoxCESniffer.TabIndex = 64;
            this.groupBoxCESniffer.TabStop = false;
            // 
            // btnVSCE
            // 
            this.btnVSCE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVSCE.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVSCE.Location = new System.Drawing.Point(609, 10);
            this.btnVSCE.Name = "btnVSCE";
            this.btnVSCE.Size = new System.Drawing.Size(124, 23);
            this.btnVSCE.TabIndex = 69;
            this.btnVSCE.Text = "Advanced handler";
            this.btnVSCE.UseVisualStyleBackColor = true;
            this.btnVSCE.Click += new System.EventHandler(this.btnVSCE_Click);
            // 
            // buttonFlush
            // 
            this.buttonFlush.Location = new System.Drawing.Point(92, 10);
            this.buttonFlush.Name = "buttonFlush";
            this.buttonFlush.Size = new System.Drawing.Size(75, 23);
            this.buttonFlush.TabIndex = 68;
            this.buttonFlush.Text = "Flush";
            this.buttonFlush.UseVisualStyleBackColor = true;
            this.buttonFlush.Click += new System.EventHandler(this.buttonFlush_Click);
            // 
            // labelSep1
            // 
            this.labelSep1.AutoSize = true;
            this.labelSep1.Location = new System.Drawing.Point(73, 15);
            this.labelSep1.Name = "labelSep1";
            this.labelSep1.Size = new System.Drawing.Size(13, 13);
            this.labelSep1.TabIndex = 67;
            this.labelSep1.Text = "::";
            // 
            // lightsTraffic
            // 
            this.lightsTraffic.Location = new System.Drawing.Point(4, 0);
            this.lightsTraffic.Name = "lightsTraffic";
            this.lightsTraffic.Size = new System.Drawing.Size(38, 9);
            this.lightsTraffic.TabIndex = 65;
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.groupBoxCESniffer);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(2);
            this.panelTop.Size = new System.Drawing.Size(747, 41);
            this.panelTop.TabIndex = 65;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.dgvCESniffer);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 41);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(747, 254);
            this.panelMain.TabIndex = 66;
            // 
            // dgvCESniffer
            // 
            this.dgvCESniffer.AllowUserToAddRows = false;
            this.dgvCESniffer.AllowUserToResizeRows = false;
            this.dgvCESniffer.AlwaysSelected = false;
            this.dgvCESniffer.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvCESniffer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvCESniffer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCESniffer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvCESnifferColumnStamp,
            this.dgvCESnifferColumnPre,
            this.dgvCESnifferColumnGuid,
            this.dgvCESnifferColumnId,
            this.dgvCESnifferColumnCustomIn,
            this.dgvCESnifferColumnCustomOut,
            this.dgvCESnifferColumnEnum});
            this.dgvCESniffer.ContextMenuStrip = this.contextMenuMain;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCESniffer.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCESniffer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCESniffer.DragDropSortable = false;
            this.dgvCESniffer.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvCESniffer.Location = new System.Drawing.Point(0, 0);
            this.dgvCESniffer.Name = "dgvCESniffer";
            this.dgvCESniffer.NumberingForRowsHeader = false;
            this.dgvCESniffer.ReadOnly = true;
            this.dgvCESniffer.RowHeadersVisible = false;
            this.dgvCESniffer.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            this.dgvCESniffer.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvCESniffer.RowTemplate.Height = 17;
            this.dgvCESniffer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCESniffer.Size = new System.Drawing.Size(747, 254);
            this.dgvCESniffer.TabIndex = 7;
            // 
            // dgvCESnifferColumnStamp
            // 
            this.dgvCESnifferColumnStamp.HeaderText = "Time stamp";
            this.dgvCESnifferColumnStamp.MinimumWidth = 50;
            this.dgvCESnifferColumnStamp.Name = "dgvCESnifferColumnStamp";
            this.dgvCESnifferColumnStamp.ReadOnly = true;
            this.dgvCESnifferColumnStamp.Width = 84;
            // 
            // dgvCESnifferColumnPre
            // 
            this.dgvCESnifferColumnPre.FalseValue = "False";
            this.dgvCESnifferColumnPre.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dgvCESnifferColumnPre.HeaderText = "Pre";
            this.dgvCESnifferColumnPre.IndeterminateValue = "False";
            this.dgvCESnifferColumnPre.MinimumWidth = 24;
            this.dgvCESnifferColumnPre.Name = "dgvCESnifferColumnPre";
            this.dgvCESnifferColumnPre.ReadOnly = true;
            this.dgvCESnifferColumnPre.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCESnifferColumnPre.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvCESnifferColumnPre.ToolTipText = "Before / After executing";
            this.dgvCESnifferColumnPre.TrueValue = "True";
            this.dgvCESnifferColumnPre.Width = 28;
            // 
            // dgvCESnifferColumnGuid
            // 
            this.dgvCESnifferColumnGuid.HeaderText = "Guid";
            this.dgvCESnifferColumnGuid.MinimumWidth = 100;
            this.dgvCESnifferColumnGuid.Name = "dgvCESnifferColumnGuid";
            this.dgvCESnifferColumnGuid.ReadOnly = true;
            this.dgvCESnifferColumnGuid.ToolTipText = "Scope by GUID";
            this.dgvCESnifferColumnGuid.Width = 240;
            // 
            // dgvCESnifferColumnId
            // 
            this.dgvCESnifferColumnId.Decimal = false;
            dataGridViewCellStyle1.NullValue = "0";
            this.dgvCESnifferColumnId.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCESnifferColumnId.HeaderText = "Id";
            this.dgvCESnifferColumnId.MinimumWidth = 70;
            this.dgvCESnifferColumnId.Name = "dgvCESnifferColumnId";
            this.dgvCESnifferColumnId.Negative = false;
            this.dgvCESnifferColumnId.ReadOnly = true;
            this.dgvCESnifferColumnId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCESnifferColumnId.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvCESnifferColumnId.ToolTipText = "Command ID";
            this.dgvCESnifferColumnId.Width = 70;
            // 
            // dgvCESnifferColumnCustomIn
            // 
            this.dgvCESnifferColumnCustomIn.HeaderText = "CustomIn";
            this.dgvCESnifferColumnCustomIn.MinimumWidth = 70;
            this.dgvCESnifferColumnCustomIn.Name = "dgvCESnifferColumnCustomIn";
            this.dgvCESnifferColumnCustomIn.ReadOnly = true;
            this.dgvCESnifferColumnCustomIn.ToolTipText = "Filter by Custom input parameter";
            this.dgvCESnifferColumnCustomIn.Width = 110;
            // 
            // dgvCESnifferColumnCustomOut
            // 
            this.dgvCESnifferColumnCustomOut.HeaderText = "CustomOut";
            this.dgvCESnifferColumnCustomOut.MinimumWidth = 70;
            this.dgvCESnifferColumnCustomOut.Name = "dgvCESnifferColumnCustomOut";
            this.dgvCESnifferColumnCustomOut.ReadOnly = true;
            this.dgvCESnifferColumnCustomOut.ToolTipText = "Filter by Custom output parameter";
            this.dgvCESnifferColumnCustomOut.Width = 110;
            // 
            // dgvCESnifferColumnEnum
            // 
            this.dgvCESnifferColumnEnum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCESnifferColumnEnum.HeaderText = "Enum";
            this.dgvCESnifferColumnEnum.MinimumWidth = 100;
            this.dgvCESnifferColumnEnum.Name = "dgvCESnifferColumnEnum";
            this.dgvCESnifferColumnEnum.ReadOnly = true;
            this.dgvCESnifferColumnEnum.ToolTipText = "Equivalent with Enum";
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuCopy,
            this.menuRemove,
            this.toolStripSeparator1,
            this.menuFlush});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(118, 76);
            // 
            // menuCopy
            // 
            this.menuCopy.Name = "menuCopy";
            this.menuCopy.Size = new System.Drawing.Size(117, 22);
            this.menuCopy.Text = "Copy";
            this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
            // 
            // menuRemove
            // 
            this.menuRemove.Name = "menuRemove";
            this.menuRemove.Size = new System.Drawing.Size(117, 22);
            this.menuRemove.Text = "Remove";
            this.menuRemove.Click += new System.EventHandler(this.menuRemove_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // menuFlush
            // 
            this.menuFlush.Name = "menuFlush";
            this.menuFlush.Size = new System.Drawing.Size(117, 22);
            this.menuFlush.Text = "Flush";
            this.menuFlush.Click += new System.EventHandler(this.menuFlush_Click);
            // 
            // EnvDteSniffer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 295);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MinimumSize = new System.Drawing.Size(437, 210);
            this.Name = "EnvDteSniffer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Tool:  EnvDTE Sniffer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EnvDteSniffer_FormClosing);
            this.Load += new System.EventHandler(this.EnvDteSniffer_Load);
            this.groupBoxCESniffer.ResumeLayout(false);
            this.groupBoxCESniffer.PerformLayout();
            this.panelTop.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCESniffer)).EndInit();
            this.contextMenuMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox chkActivate;
        private System.Windows.Forms.GroupBox groupBoxCESniffer;
        private Components.DataGridViewExt dgvCESniffer;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCESnifferColumnStamp;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvCESnifferColumnPre;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCESnifferColumnGuid;
        private Components.DataGridViewExt.NumericColumn dgvCESnifferColumnId;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCESnifferColumnCustomIn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCESnifferColumnCustomOut;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCESnifferColumnEnum;
        private Controls.Lights lightsTraffic;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button buttonFlush;
        private System.Windows.Forms.Label labelSep1;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem menuCopy;
        private System.Windows.Forms.ToolStripMenuItem menuRemove;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuFlush;
        private System.Windows.Forms.Button btnVSCE;
    }
}