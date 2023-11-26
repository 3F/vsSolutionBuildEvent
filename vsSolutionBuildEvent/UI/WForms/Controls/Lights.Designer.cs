namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    partial class Lights
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
            this.pRed = new net.r_eg.vsSBE.UI.WForms.Controls.Lights.PanelDt();
            this.pYellow = new net.r_eg.vsSBE.UI.WForms.Controls.Lights.PanelDt();
            this.pGreen = new net.r_eg.vsSBE.UI.WForms.Controls.Lights.PanelDt();
            this.SuspendLayout();
            // 
            // pRed
            // 
            this.pRed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.pRed.Location = new System.Drawing.Point(0, 0);
            this.pRed.Name = "pRed";
            this.pRed.Size = new System.Drawing.Size(9, 9);
            this.pRed.TabIndex = 67;
            // 
            // pYellow
            // 
            this.pYellow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(239)))), ((int)(((byte)(193)))));
            this.pYellow.Location = new System.Drawing.Point(14, 0);
            this.pYellow.Name = "pYellow";
            this.pYellow.Size = new System.Drawing.Size(9, 9);
            this.pYellow.TabIndex = 68;
            // 
            // pGreen
            // 
            this.pGreen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(225)))), ((int)(((byte)(185)))));
            this.pGreen.Location = new System.Drawing.Point(28, 0);
            this.pGreen.Name = "pGreen";
            this.pGreen.Size = new System.Drawing.Size(9, 9);
            this.pGreen.TabIndex = 69;
            // 
            // Lights
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pGreen);
            this.Controls.Add(this.pYellow);
            this.Controls.Add(this.pRed);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Lights";
            this.Size = new System.Drawing.Size(41, 10);
            this.ResumeLayout(false);

        }

        #endregion

        private PanelDt pRed;
        private PanelDt pYellow;
        private PanelDt pGreen;
    }
}
