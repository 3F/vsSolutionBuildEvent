namespace net.r_eg.vsSBE.UI.WForms
{
    partial class PropertiesFrm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewVariables = new System.Windows.Forms.DataGridView();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuItemExportList = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemFiltersType = new System.Windows.Forms.ToolStripMenuItem();
            this.mFilterRegexp = new System.Windows.Forms.ToolStripMenuItem();
            this.comboBoxProjects = new System.Windows.Forms.ComboBox();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.labelFiler = new System.Windows.Forms.Label();
            this.labelPropCount = new System.Windows.Forms.Label();
            this.labelFilerVal = new System.Windows.Forms.Label();
            this.textBoxFilterVal = new System.Windows.Forms.TextBox();
            this.splitContainerFilters = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVariables)).BeginInit();
            this.contextMenuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFilters)).BeginInit();
            this.splitContainerFilters.Panel1.SuspendLayout();
            this.splitContainerFilters.Panel2.SuspendLayout();
            this.splitContainerFilters.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewVariables
            // 
            this.dataGridViewVariables.AllowUserToAddRows = false;
            this.dataGridViewVariables.AllowUserToDeleteRows = false;
            this.dataGridViewVariables.AllowUserToOrderColumns = true;
            this.dataGridViewVariables.AllowUserToResizeRows = false;
            this.dataGridViewVariables.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewVariables.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dataGridViewVariables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewVariables.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colName,
            this.colValue});
            this.dataGridViewVariables.ContextMenuStrip = this.contextMenuMain;
            this.dataGridViewVariables.Location = new System.Drawing.Point(0, 27);
            this.dataGridViewVariables.MultiSelect = false;
            this.dataGridViewVariables.Name = "dataGridViewVariables";
            this.dataGridViewVariables.ReadOnly = true;
            this.dataGridViewVariables.RowHeadersVisible = false;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(241)))), ((int)(((byte)(253)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(36)))), ((int)(((byte)(47)))));
            this.dataGridViewVariables.RowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewVariables.Size = new System.Drawing.Size(560, 183);
            this.dataGridViewVariables.StandardTab = true;
            this.dataGridViewVariables.TabIndex = 2;
            this.dataGridViewVariables.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVariables_CellClick);
            this.dataGridViewVariables.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVariables_CellDoubleClick);
            this.dataGridViewVariables.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewVariables_KeyDown);
            this.dataGridViewVariables.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewVariables_KeyUp);
            // 
            // colName
            // 
            this.colName.HeaderText = "Name";
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            this.colName.Width = 200;
            // 
            // colValue
            // 
            this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colValue.HeaderText = "Value";
            this.colValue.Name = "colValue";
            this.colValue.ReadOnly = true;
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemExportList,
            this.toolStripSeparator1,
            this.menuItemFiltersType});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(195, 54);
            // 
            // menuItemExportList
            // 
            this.menuItemExportList.Name = "menuItemExportList";
            this.menuItemExportList.Size = new System.Drawing.Size(194, 22);
            this.menuItemExportList.Text = "Export list to Clipboard";
            this.menuItemExportList.Click += new System.EventHandler(this.menuItemExportList_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(191, 6);
            // 
            // menuItemFiltersType
            // 
            this.menuItemFiltersType.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mFilterRegexp});
            this.menuItemFiltersType.Name = "menuItemFiltersType";
            this.menuItemFiltersType.Size = new System.Drawing.Size(194, 22);
            this.menuItemFiltersType.Text = "Filters type";
            // 
            // mFilterRegexp
            // 
            this.mFilterRegexp.Name = "mFilterRegexp";
            this.mFilterRegexp.Size = new System.Drawing.Size(112, 22);
            this.mFilterRegexp.Text = "Regexp";
            this.mFilterRegexp.Click += new System.EventHandler(this.menuItemFilterRegexp_Click);
            // 
            // comboBoxProjects
            // 
            this.comboBoxProjects.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxProjects.FormattingEnabled = true;
            this.comboBoxProjects.Location = new System.Drawing.Point(0, 0);
            this.comboBoxProjects.Name = "comboBoxProjects";
            this.comboBoxProjects.Size = new System.Drawing.Size(560, 21);
            this.comboBoxProjects.TabIndex = 1;
            this.comboBoxProjects.SelectedIndexChanged += new System.EventHandler(this.comboBoxProjects_SelectedIndexChanged);
            this.comboBoxProjects.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboBoxProjects_KeyUp);
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxFilter.ContextMenuStrip = this.contextMenuMain;
            this.textBoxFilter.Location = new System.Drawing.Point(0, 22);
            this.textBoxFilter.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(276, 20);
            this.textBoxFilter.TabIndex = 0;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            this.textBoxFilter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyDown);
            this.textBoxFilter.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyUp);
            // 
            // labelFiler
            // 
            this.labelFiler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFiler.AutoSize = true;
            this.labelFiler.Location = new System.Drawing.Point(0, 5);
            this.labelFiler.Name = "labelFiler";
            this.labelFiler.Size = new System.Drawing.Size(38, 13);
            this.labelFiler.TabIndex = 3;
            this.labelFiler.Text = "Name:";
            // 
            // labelPropCount
            // 
            this.labelPropCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPropCount.Location = new System.Drawing.Point(444, 213);
            this.labelPropCount.Name = "labelPropCount";
            this.labelPropCount.Size = new System.Drawing.Size(116, 18);
            this.labelPropCount.TabIndex = 4;
            this.labelPropCount.Text = "0";
            this.labelPropCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelFilerVal
            // 
            this.labelFilerVal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFilerVal.AutoSize = true;
            this.labelFilerVal.Location = new System.Drawing.Point(0, 5);
            this.labelFilerVal.Name = "labelFilerVal";
            this.labelFilerVal.Size = new System.Drawing.Size(37, 13);
            this.labelFilerVal.TabIndex = 5;
            this.labelFilerVal.Text = "Value:";
            // 
            // textBoxFilterVal
            // 
            this.textBoxFilterVal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilterVal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxFilterVal.ContextMenuStrip = this.contextMenuMain;
            this.textBoxFilterVal.Location = new System.Drawing.Point(0, 22);
            this.textBoxFilterVal.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxFilterVal.Name = "textBoxFilterVal";
            this.textBoxFilterVal.Size = new System.Drawing.Size(281, 20);
            this.textBoxFilterVal.TabIndex = 6;
            this.textBoxFilterVal.TextChanged += new System.EventHandler(this.textBoxFilterVal_TextChanged);
            this.textBoxFilterVal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBoxFilterVal_KeyDown);
            this.textBoxFilterVal.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxFilterVal_KeyUp);
            // 
            // splitContainerFilters
            // 
            this.splitContainerFilters.ContextMenuStrip = this.contextMenuMain;
            this.splitContainerFilters.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainerFilters.Location = new System.Drawing.Point(0, 213);
            this.splitContainerFilters.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerFilters.Name = "splitContainerFilters";
            // 
            // splitContainerFilters.Panel1
            // 
            this.splitContainerFilters.Panel1.Controls.Add(this.labelFiler);
            this.splitContainerFilters.Panel1.Controls.Add(this.textBoxFilter);
            this.splitContainerFilters.Panel1MinSize = 100;
            // 
            // splitContainerFilters.Panel2
            // 
            this.splitContainerFilters.Panel2.Controls.Add(this.labelFilerVal);
            this.splitContainerFilters.Panel2.Controls.Add(this.textBoxFilterVal);
            this.splitContainerFilters.Panel2MinSize = 190;
            this.splitContainerFilters.Size = new System.Drawing.Size(561, 43);
            this.splitContainerFilters.SplitterDistance = 276;
            this.splitContainerFilters.TabIndex = 7;
            // 
            // PropertiesFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 256);
            this.ContextMenuStrip = this.contextMenuMain;
            this.Controls.Add(this.labelPropCount);
            this.Controls.Add(this.splitContainerFilters);
            this.Controls.Add(this.comboBoxProjects);
            this.Controls.Add(this.dataGridViewVariables);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(390, 170);
            this.Name = "PropertiesFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MSBuild Properties";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EnvironmentVariablesFrm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EnvironmentVariablesFrm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVariables)).EndInit();
            this.contextMenuMain.ResumeLayout(false);
            this.splitContainerFilters.Panel1.ResumeLayout(false);
            this.splitContainerFilters.Panel1.PerformLayout();
            this.splitContainerFilters.Panel2.ResumeLayout(false);
            this.splitContainerFilters.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerFilters)).EndInit();
            this.splitContainerFilters.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewVariables;
        private System.Windows.Forms.ComboBox comboBoxProjects;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label labelFiler;
        private System.Windows.Forms.Label labelPropCount;
        private System.Windows.Forms.Label labelFilerVal;
        private System.Windows.Forms.TextBox textBoxFilterVal;
        private System.Windows.Forms.SplitContainer splitContainerFilters;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem menuItemExportList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemFiltersType;
        private System.Windows.Forms.ToolStripMenuItem mFilterRegexp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
    }
}