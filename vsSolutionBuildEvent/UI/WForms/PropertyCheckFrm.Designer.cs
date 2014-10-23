namespace net.r_eg.vsSBE.UI
{
    partial class PropertyCheckFrm
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
            this.btnEvaluate = new System.Windows.Forms.Button();
            this.groupBoxUnevaluated = new System.Windows.Forms.GroupBox();
            this.textBoxUnevaluated = new System.Windows.Forms.TextBox();
            this.groupBoxEvaluated = new System.Windows.Forms.GroupBox();
            this.richTextBoxEvaluated = new System.Windows.Forms.RichTextBox();
            this.groupBoxUnevaluated.SuspendLayout();
            this.groupBoxEvaluated.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEvaluate
            // 
            this.btnEvaluate.Location = new System.Drawing.Point(8, 53);
            this.btnEvaluate.Name = "btnEvaluate";
            this.btnEvaluate.Size = new System.Drawing.Size(85, 23);
            this.btnEvaluate.TabIndex = 0;
            this.btnEvaluate.Text = "Evaluate";
            this.btnEvaluate.UseVisualStyleBackColor = true;
            this.btnEvaluate.Click += new System.EventHandler(this.btnEvaluate_Click);
            // 
            // groupBoxUnevaluated
            // 
            this.groupBoxUnevaluated.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxUnevaluated.Controls.Add(this.textBoxUnevaluated);
            this.groupBoxUnevaluated.Location = new System.Drawing.Point(2, 2);
            this.groupBoxUnevaluated.Name = "groupBoxUnevaluated";
            this.groupBoxUnevaluated.Size = new System.Drawing.Size(489, 45);
            this.groupBoxUnevaluated.TabIndex = 1;
            this.groupBoxUnevaluated.TabStop = false;
            this.groupBoxUnevaluated.Text = "To evaluation:";
            // 
            // textBoxUnevaluated
            // 
            this.textBoxUnevaluated.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxUnevaluated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxUnevaluated.Location = new System.Drawing.Point(3, 16);
            this.textBoxUnevaluated.Name = "textBoxUnevaluated";
            this.textBoxUnevaluated.Size = new System.Drawing.Size(483, 20);
            this.textBoxUnevaluated.TabIndex = 0;
            this.textBoxUnevaluated.Click += new System.EventHandler(this.textBoxUnevaluated_Click);
            // 
            // groupBoxEvaluated
            // 
            this.groupBoxEvaluated.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxEvaluated.Controls.Add(this.richTextBoxEvaluated);
            this.groupBoxEvaluated.Location = new System.Drawing.Point(2, 63);
            this.groupBoxEvaluated.Name = "groupBoxEvaluated";
            this.groupBoxEvaluated.Size = new System.Drawing.Size(489, 156);
            this.groupBoxEvaluated.TabIndex = 2;
            this.groupBoxEvaluated.TabStop = false;
            // 
            // richTextBoxEvaluated
            // 
            this.richTextBoxEvaluated.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxEvaluated.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxEvaluated.Location = new System.Drawing.Point(3, 16);
            this.richTextBoxEvaluated.Name = "richTextBoxEvaluated";
            this.richTextBoxEvaluated.ReadOnly = true;
            this.richTextBoxEvaluated.Size = new System.Drawing.Size(483, 137);
            this.richTextBoxEvaluated.TabIndex = 0;
            this.richTextBoxEvaluated.Text = "";
            // 
            // PropertyCheckFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 223);
            this.Controls.Add(this.btnEvaluate);
            this.Controls.Add(this.groupBoxUnevaluated);
            this.Controls.Add(this.groupBoxEvaluated);
            this.MinimumSize = new System.Drawing.Size(250, 160);
            this.Name = "PropertyCheckFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Testing Tool:  Evaluating Property";
            this.Load += new System.EventHandler(this.PropertyCheckFrm_Load);
            this.groupBoxUnevaluated.ResumeLayout(false);
            this.groupBoxUnevaluated.PerformLayout();
            this.groupBoxEvaluated.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEvaluate;
        private System.Windows.Forms.GroupBox groupBoxUnevaluated;
        private System.Windows.Forms.TextBox textBoxUnevaluated;
        private System.Windows.Forms.GroupBox groupBoxEvaluated;
        private System.Windows.Forms.RichTextBox richTextBoxEvaluated;
    }
}