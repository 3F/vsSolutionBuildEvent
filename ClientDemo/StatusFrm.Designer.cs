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
            this.rtbMain = new System.Windows.Forms.RichTextBox();
            this.btnAPI = new System.Windows.Forms.Button();
            this.btnSrc = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.chkAnchor = new System.Windows.Forms.CheckBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.chkPin = new System.Windows.Forms.CheckBox();
            this.btnCopy = new System.Windows.Forms.Button();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelTop.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbMain
            // 
            this.rtbMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(231)))), ((int)(((byte)(232)))));
            this.rtbMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.rtbMain.Location = new System.Drawing.Point(0, 0);
            this.rtbMain.Name = "rtbMain";
            this.rtbMain.ReadOnly = true;
            this.rtbMain.Size = new System.Drawing.Size(660, 142);
            this.rtbMain.TabIndex = 0;
            this.rtbMain.Text = "";
            this.rtbMain.WordWrap = false;
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
            this.btnSrc.Text = "Source code";
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
            this.panelTop.Controls.Add(this.chkAnchor);
            this.panelTop.Controls.Add(this.btnPause);
            this.panelTop.Controls.Add(this.chkPin);
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
            // chkAnchor
            // 
            this.chkAnchor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAnchor.AutoSize = true;
            this.chkAnchor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkAnchor.Location = new System.Drawing.Point(579, 3);
            this.chkAnchor.Name = "chkAnchor";
            this.chkAnchor.Size = new System.Drawing.Size(36, 17);
            this.chkAnchor.TabIndex = 7;
            this.chkAnchor.Text = "⚓";
            this.chkAnchor.UseVisualStyleBackColor = true;
            this.chkAnchor.CheckedChanged += new System.EventHandler(this.chkAnchor_CheckedChanged);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(336, 0);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(75, 23);
            this.btnPause.TabIndex = 6;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // chkPin
            // 
            this.chkPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPin.AutoSize = true;
            this.chkPin.Checked = true;
            this.chkPin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPin.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkPin.Location = new System.Drawing.Point(621, 3);
            this.chkPin.Name = "chkPin";
            this.chkPin.Size = new System.Drawing.Size(36, 17);
            this.chkPin.TabIndex = 5;
            this.chkPin.Text = "📌";
            this.chkPin.UseVisualStyleBackColor = true;
            this.chkPin.CheckedChanged += new System.EventHandler(this.chkPin_CheckedChanged);
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
            // panelMain
            // 
            this.panelMain.Controls.Add(this.rtbMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 23);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(660, 142);
            this.panelMain.TabIndex = 5;
            // 
            // StatusFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(660, 165);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(450, 70);
            this.Name = "StatusFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Demo client to vsSolutionBuildEvent";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatusFrm_FormClosing);
            this.Load += new System.EventHandler(this.StatusFrm_Load);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbMain;
        private System.Windows.Forms.Button btnAPI;
        private System.Windows.Forms.Button btnSrc;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.CheckBox chkPin;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.CheckBox chkAnchor;
    }
}