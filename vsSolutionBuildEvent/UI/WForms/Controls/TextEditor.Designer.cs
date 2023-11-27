namespace net.r_eg.vsSBE.UI.WForms.Controls
{
    partial class TextEditor
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuEditor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuComboBoxZoom = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemCut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemWordWrap = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuEditor.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuEditor
            // 
            this.contextMenuEditor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuComboBoxZoom,
            this.toolStripSeparator1,
            this.menuItemCut,
            this.menuItemCopy,
            this.menuItemPaste,
            this.toolStripSeparator2,
            this.menuItemUndo,
            this.menuItemRedo,
            this.toolStripSeparator3,
            this.menuItemSearch,
            this.menuItemWordWrap});
            this.contextMenuEditor.Name = "contextMenuEditor";
            this.contextMenuEditor.Size = new System.Drawing.Size(182, 225);
            this.contextMenuEditor.Opened += new System.EventHandler(this.contextMenuEditor_Opened);
            // 
            // menuComboBoxZoom
            // 
            this.menuComboBoxZoom.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.menuComboBoxZoom.Items.AddRange(new object[] {
            "400 %",
            "200 %",
            "150 %",
            "100 %",
            "70 %",
            "50 %",
            "20 %"});
            this.menuComboBoxZoom.Name = "menuComboBoxZoom";
            this.menuComboBoxZoom.Size = new System.Drawing.Size(121, 23);
            this.menuComboBoxZoom.TextChanged += new System.EventHandler(this.menuComboBoxZoom_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // menuItemCut
            // 
            this.menuItemCut.Name = "menuItemCut";
            this.menuItemCut.Size = new System.Drawing.Size(181, 22);
            this.menuItemCut.Text = "Cut";
            this.menuItemCut.Click += new System.EventHandler(this.menuItemCut_Click);
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Name = "menuItemCopy";
            this.menuItemCopy.Size = new System.Drawing.Size(181, 22);
            this.menuItemCopy.Text = "Copy";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemPaste
            // 
            this.menuItemPaste.Name = "menuItemPaste";
            this.menuItemPaste.Size = new System.Drawing.Size(181, 22);
            this.menuItemPaste.Text = "Paste";
            this.menuItemPaste.Click += new System.EventHandler(this.menuItemPaste_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(178, 6);
            // 
            // menuItemUndo
            // 
            this.menuItemUndo.Name = "menuItemUndo";
            this.menuItemUndo.Size = new System.Drawing.Size(181, 22);
            this.menuItemUndo.Text = "Undo";
            this.menuItemUndo.Click += new System.EventHandler(this.menuItemUndo_Click);
            // 
            // menuItemRedo
            // 
            this.menuItemRedo.Name = "menuItemRedo";
            this.menuItemRedo.Size = new System.Drawing.Size(181, 22);
            this.menuItemRedo.Text = "Redo";
            this.menuItemRedo.Click += new System.EventHandler(this.menuItemRedo_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(178, 6);
            // 
            // menuItemWordWrap
            // 
            this.menuItemWordWrap.Name = "menuItemWordWrap";
            this.menuItemWordWrap.Size = new System.Drawing.Size(181, 22);
            this.menuItemWordWrap.Text = "Word wrapping";
            this.menuItemWordWrap.Click += new System.EventHandler(this.menuItemWordWrap_Click);
            // 
            // menuItemSearch
            // 
            this.menuItemSearch.Name = "menuItemSearch";
            this.menuItemSearch.Size = new System.Drawing.Size(181, 22);
            this.menuItemSearch.Text = "Search";
            this.menuItemSearch.Click += new System.EventHandler(this.menuItemSearch_Click);
            // 
            // TextEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TextEditor";
            this.Size = new System.Drawing.Size(111, 68);
            this.contextMenuEditor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuEditor;
        private System.Windows.Forms.ToolStripComboBox menuComboBoxZoom;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem menuItemCut;
        private System.Windows.Forms.ToolStripMenuItem menuItemCopy;
        private System.Windows.Forms.ToolStripMenuItem menuItemPaste;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem menuItemUndo;
        private System.Windows.Forms.ToolStripMenuItem menuItemRedo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem menuItemWordWrap;
        private System.Windows.Forms.ToolStripMenuItem menuItemSearch;
    }
}
