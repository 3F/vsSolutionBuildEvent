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
            this.dataGridViewVariables = new System.Windows.Forms.DataGridView();
            this.ColumnVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comboBoxProjects = new System.Windows.Forms.ComboBox();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.labelFiler = new System.Windows.Forms.Label();
            this.labelPropCount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVariables)).BeginInit();
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
            this.ColumnVariable,
            this.ColumnValue});
            this.dataGridViewVariables.Location = new System.Drawing.Point(0, 27);
            this.dataGridViewVariables.MultiSelect = false;
            this.dataGridViewVariables.Name = "dataGridViewVariables";
            this.dataGridViewVariables.ReadOnly = true;
            this.dataGridViewVariables.RowHeadersVisible = false;
            this.dataGridViewVariables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewVariables.Size = new System.Drawing.Size(560, 183);
            this.dataGridViewVariables.StandardTab = true;
            this.dataGridViewVariables.TabIndex = 2;
            this.dataGridViewVariables.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewVariables_CellDoubleClick);
            this.dataGridViewVariables.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewVariables_KeyDown);
            this.dataGridViewVariables.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridViewVariables_KeyUp);
            // 
            // ColumnVariable
            // 
            this.ColumnVariable.HeaderText = "Variable";
            this.ColumnVariable.Name = "ColumnVariable";
            this.ColumnVariable.ReadOnly = true;
            this.ColumnVariable.Width = 200;
            // 
            // ColumnValue
            // 
            this.ColumnValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnValue.HeaderText = "Value";
            this.ColumnValue.Name = "ColumnValue";
            this.ColumnValue.ReadOnly = true;
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
            this.textBoxFilter.Location = new System.Drawing.Point(0, 234);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(560, 20);
            this.textBoxFilter.TabIndex = 0;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            this.textBoxFilter.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxFilter_KeyUp);
            // 
            // labelFiler
            // 
            this.labelFiler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFiler.AutoSize = true;
            this.labelFiler.Location = new System.Drawing.Point(-3, 218);
            this.labelFiler.Name = "labelFiler";
            this.labelFiler.Size = new System.Drawing.Size(32, 13);
            this.labelFiler.TabIndex = 3;
            this.labelFiler.Text = "Filter:";
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
            // PropertiesFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 256);
            this.Controls.Add(this.labelPropCount);
            this.Controls.Add(this.labelFiler);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.comboBoxProjects);
            this.Controls.Add(this.dataGridViewVariables);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(390, 170);
            this.Name = "PropertiesFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MSBuild Properties";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.EnvironmentVariablesFrm_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.EnvironmentVariablesFrm_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewVariables)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewVariables;
        private System.Windows.Forms.ComboBox comboBoxProjects;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.Label labelFiler;
        private System.Windows.Forms.Label labelPropCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnVariable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnValue;
    }
}