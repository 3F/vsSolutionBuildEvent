namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    partial class FileTextBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fdialog = new System.Windows.Forms.Button();
            this.fname = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // fdialog
            // 
            this.fdialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.fdialog.Location = new System.Drawing.Point(130, 0);
            this.fdialog.Name = "fdialog";
            this.fdialog.Size = new System.Drawing.Size(29, 20);
            this.fdialog.TabIndex = 30;
            this.fdialog.Text = "...";
            this.fdialog.UseVisualStyleBackColor = true;
            this.fdialog.Click += new System.EventHandler(this.fdialog_Click);
            // 
            // fname
            // 
            this.fname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fname.Location = new System.Drawing.Point(0, 0);
            this.fname.Name = "fname";
            this.fname.Size = new System.Drawing.Size(131, 20);
            this.fname.TabIndex = 29;
            // 
            // FileTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fdialog);
            this.Controls.Add(this.fname);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.Name = "FileTextBox";
            this.Size = new System.Drawing.Size(159, 20);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button fdialog;
        private System.Windows.Forms.TextBox fname;
    }
}
