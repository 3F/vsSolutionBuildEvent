namespace net.r_eg.vsSBE.UI.WForms
{
    partial class DTECommandsFrm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.labelPropCount = new System.Windows.Forms.Label();
            this.labelFiler = new System.Windows.Forms.Label();
            this.textBoxFilter = new System.Windows.Forms.TextBox();
            this.ColumnCommand = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewDTE = new net.r_eg.vsSBE.UI.WForms.Components.DataGridViewExt();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDTE)).BeginInit();
            this.SuspendLayout();
            // 
            // labelPropCount
            // 
            this.labelPropCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.labelPropCount.Location = new System.Drawing.Point(444, 206);
            this.labelPropCount.Name = "labelPropCount";
            this.labelPropCount.Size = new System.Drawing.Size(105, 18);
            this.labelPropCount.TabIndex = 8;
            this.labelPropCount.Text = "0";
            this.labelPropCount.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelFiler
            // 
            this.labelFiler.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelFiler.AutoSize = true;
            this.labelFiler.Location = new System.Drawing.Point(-3, 211);
            this.labelFiler.Name = "labelFiler";
            this.labelFiler.Size = new System.Drawing.Size(32, 13);
            this.labelFiler.TabIndex = 7;
            this.labelFiler.Text = "Filter:";
            // 
            // textBoxFilter
            // 
            this.textBoxFilter.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxFilter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxFilter.Location = new System.Drawing.Point(0, 227);
            this.textBoxFilter.Name = "textBoxFilter";
            this.textBoxFilter.Size = new System.Drawing.Size(549, 20);
            this.textBoxFilter.TabIndex = 5;
            this.textBoxFilter.TextChanged += new System.EventHandler(this.textBoxFilter_TextChanged);
            // 
            // ColumnCommand
            // 
            this.ColumnCommand.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnCommand.HeaderText = "Command";
            this.ColumnCommand.Name = "ColumnCommand";
            this.ColumnCommand.ReadOnly = true;
            // 
            // dataGridViewDTE
            // 
            this.dataGridViewDTE.AllowUserToAddRows = false;
            this.dataGridViewDTE.AllowUserToDeleteRows = false;
            this.dataGridViewDTE.AllowUserToOrderColumns = true;
            this.dataGridViewDTE.AllowUserToResizeRows = false;
            this.dataGridViewDTE.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewDTE.BackgroundColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(36)))), ((int)(((byte)(47)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewDTE.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewDTE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewDTE.ColumnHeadersVisible = false;
            this.dataGridViewDTE.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnCommand});
            this.dataGridViewDTE.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewDTE.MultiSelect = false;
            this.dataGridViewDTE.Name = "dataGridViewDTE";
            this.dataGridViewDTE.ReadOnly = true;
            this.dataGridViewDTE.RowHeadersVisible = false;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(36)))), ((int)(((byte)(47)))));
            this.dataGridViewDTE.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewDTE.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewDTE.Size = new System.Drawing.Size(549, 201);
            this.dataGridViewDTE.StandardTab = true;
            this.dataGridViewDTE.TabIndex = 6;
            this.dataGridViewDTE.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDTE_CellClick);
            this.dataGridViewDTE.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewDTE_CellDoubleClick);
            this.dataGridViewDTE.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewDTE_KeyDown);
            // 
            // DTECommandsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(550, 249);
            this.Controls.Add(this.labelPropCount);
            this.Controls.Add(this.labelFiler);
            this.Controls.Add(this.textBoxFilter);
            this.Controls.Add(this.dataGridViewDTE);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(240, 110);
            this.Name = "DTECommandsFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DTE Commands";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.DTECommandsFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewDTE)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelPropCount;
        private System.Windows.Forms.Label labelFiler;
        private System.Windows.Forms.TextBox textBoxFilter;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnCommand;
        private System.Windows.Forms.DataGridView dataGridViewDTE;
    }
}