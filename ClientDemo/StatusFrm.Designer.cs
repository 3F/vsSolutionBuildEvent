namespace ClientDemo
{
    partial class StatusFrm
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
            this.richTextBoxMain = new System.Windows.Forms.RichTextBox();
            this.btnAPI = new System.Windows.Forms.Button();
            this.btnSrc = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.btnCopy = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // richTextBoxMain
            // 
            this.richTextBoxMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(231)))), ((int)(((byte)(232)))));
            this.richTextBoxMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBoxMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.richTextBoxMain.Location = new System.Drawing.Point(0, 0);
            this.richTextBoxMain.Name = "richTextBoxMain";
            this.richTextBoxMain.ReadOnly = true;
            this.richTextBoxMain.Size = new System.Drawing.Size(660, 142);
            this.richTextBoxMain.TabIndex = 0;
            this.richTextBoxMain.Text = "";
            this.richTextBoxMain.WordWrap = false;
            // 
            // btnAPI
            // 
            this.btnAPI.Location = new System.Drawing.Point(0, 0);
            this.btnAPI.Name = "btnAPI";
            this.btnAPI.Size = new System.Drawing.Size(75, 23);
            this.btnAPI.TabIndex = 1;
            this.btnAPI.Text = "API";
            this.btnAPI.UseVisualStyleBackColor = true;
            this.btnAPI.Click += new System.EventHandler(this.btnAPI_Click);
            // 
            // btnSrc
            // 
            this.btnSrc.Location = new System.Drawing.Point(81, 0);
            this.btnSrc.Name = "btnSrc";
            this.btnSrc.Size = new System.Drawing.Size(87, 23);
            this.btnSrc.TabIndex = 2;
            this.btnSrc.Text = "Source Code";
            this.btnSrc.UseVisualStyleBackColor = true;
            this.btnSrc.Click += new System.EventHandler(this.btnSrc_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(255, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.btnCopy);
            this.panelTop.Controls.Add(this.btnAPI);
            this.panelTop.Controls.Add(this.btnClear);
            this.panelTop.Controls.Add(this.btnSrc);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Margin = new System.Windows.Forms.Padding(0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(660, 23);
            this.panelTop.TabIndex = 4;
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.richTextBoxMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 23);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(660, 142);
            this.panelMain.TabIndex = 5;
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(174, 0);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 4;
            this.btnCopy.Text = "Copy all";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // StatusFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 165);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.MinimumSize = new System.Drawing.Size(450, 70);
            this.Name = "StatusFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo client to vsSolutionBuildEvent";
            this.TopMost = true;
            this.panelTop.ResumeLayout(false);
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox richTextBoxMain;
        private System.Windows.Forms.Button btnAPI;
        private System.Windows.Forms.Button btnSrc;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Panel panelMain;
    }
}