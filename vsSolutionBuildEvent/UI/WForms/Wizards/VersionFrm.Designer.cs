namespace net.r_eg.vsSBE.UI.WForms.Wizards
{
    partial class VersionFrm
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageGen = new System.Windows.Forms.TabPage();
            this.gbGenSample = new System.Windows.Forms.GroupBox();
            this.editorStepGen = new net.r_eg.vsSBE.UI.WForms.Controls.TextEditor();
            this.labelStepType = new System.Windows.Forms.Label();
            this.gbStepGen = new System.Windows.Forms.GroupBox();
            this.radioGenDirect = new System.Windows.Forms.RadioButton();
            this.radioGenCppDefine = new System.Windows.Forms.RadioButton();
            this.radioGenCppStruct = new System.Windows.Forms.RadioButton();
            this.radioGenCSharpStruct = new System.Windows.Forms.RadioButton();
            this.tabPageStruct = new System.Windows.Forms.TabPage();
            this.labelStructStep = new System.Windows.Forms.Label();
            this.gbStructStep = new System.Windows.Forms.GroupBox();
            this.labelUpperCase = new System.Windows.Forms.Label();
            this.chkUpperCase = new System.Windows.Forms.CheckBox();
            this.labelTypeOfNumber = new System.Windows.Forms.Label();
            this.cbNumberType = new System.Windows.Forms.ComboBox();
            this.tbClassName = new System.Windows.Forms.TextBox();
            this.labelStructName = new System.Windows.Forms.Label();
            this.tbNamespace = new System.Windows.Forms.TextBox();
            this.labelNamespace = new System.Windows.Forms.Label();
            this.tabPageCfgData = new System.Windows.Forms.TabPage();
            this.gbCfgData = new System.Windows.Forms.GroupBox();
            this.pFixBorder = new System.Windows.Forms.Panel();
            this.ftbOutputFile = new net.r_eg.vsSBE.UI.WForms.Controls.FileTextBox();
            this.ftbInputNum = new net.r_eg.vsSBE.UI.WForms.Controls.FileTextBox();
            this.tcRevNumber = new System.Windows.Forms.TabControl();
            this.tpRevRaw = new System.Windows.Forms.TabPage();
            this.tpRevDeltaTime = new System.Windows.Forms.TabPage();
            this.numRevTimeMax = new System.Windows.Forms.NumericUpDown();
            this.numRevTimeMin = new System.Windows.Forms.NumericUpDown();
            this.chkRevTimeMod = new System.Windows.Forms.CheckBox();
            this.cbRevTimeType = new System.Windows.Forms.ComboBox();
            this.labelRevTimeType = new System.Windows.Forms.Label();
            this.dtRevTimeBase = new System.Windows.Forms.DateTimePicker();
            this.labelRevTimeBase = new System.Windows.Forms.Label();
            this.labelInputNum = new System.Windows.Forms.Label();
            this.cbTypeRev = new System.Windows.Forms.ComboBox();
            this.labelTypeRev = new System.Windows.Forms.Label();
            this.labelSCM = new System.Windows.Forms.Label();
            this.labelOutputFile = new System.Windows.Forms.Label();
            this.cbSCM = new System.Windows.Forms.ComboBox();
            this.cbInputNum = new System.Windows.Forms.ComboBox();
            this.labelInputNumType = new System.Windows.Forms.Label();
            this.labelCfgDataStep = new System.Windows.Forms.Label();
            this.tabPageRepl = new System.Windows.Forms.TabPage();
            this.labelSampleSource = new System.Windows.Forms.Label();
            this.labelExampleSource = new System.Windows.Forms.Label();
            this.tcReplType = new System.Windows.Forms.TabControl();
            this.tpReplRegex = new System.Windows.Forms.TabPage();
            this.labelHelpRegexLangCaption = new System.Windows.Forms.Label();
            this.linkRegex = new System.Windows.Forms.LinkLabel();
            this.tpReplWildcards = new System.Windows.Forms.TabPage();
            this.tbHelpWildcards = new System.Windows.Forms.TextBox();
            this.gbReplacement = new System.Windows.Forms.GroupBox();
            this.labelReplPostfix = new System.Windows.Forms.Label();
            this.tbReplPrefix = new System.Windows.Forms.TextBox();
            this.labelReplPrefix = new System.Windows.Forms.Label();
            this.labelReplSource = new System.Windows.Forms.Label();
            this.tbReplPostfix = new System.Windows.Forms.TextBox();
            this.cbReplSource = new System.Windows.Forms.ComboBox();
            this.gbReplPattern = new System.Windows.Forms.GroupBox();
            this.tbReplPattern = new System.Windows.Forms.TextBox();
            this.gbReplStep = new System.Windows.Forms.GroupBox();
            this.ftbReplFile = new net.r_eg.vsSBE.UI.WForms.Controls.FileTextBox();
            this.cbReplType = new System.Windows.Forms.ComboBox();
            this.labelReplFile = new System.Windows.Forms.Label();
            this.labelReplType = new System.Windows.Forms.Label();
            this.labelReplStep = new System.Windows.Forms.Label();
            this.tabPageFields = new System.Windows.Forms.TabPage();
            this.labelFields = new System.Windows.Forms.Label();
            this.dgvFields = new net.r_eg.vsSBE.UI.WForms.Components.DataGridViewExt();
            this.dgvFieldsEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvFieldsNameOrigin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFieldsNameNew = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFieldsDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageFinal = new System.Windows.Forms.TabPage();
            this.labelFinal = new System.Windows.Forms.Label();
            this.btnFinalCreateAction = new System.Windows.Forms.Button();
            this.btnFinalCopy = new System.Windows.Forms.Button();
            this.gbFinalResult = new System.Windows.Forms.GroupBox();
            this.editorFinalScript = new net.r_eg.vsSBE.UI.WForms.Controls.TextEditor();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnManually = new System.Windows.Forms.Button();
            this.btnNextStep = new System.Windows.Forms.Button();
            this.btnPrevStep = new System.Windows.Forms.Button();
            this.labelDone = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.tabControlMain.SuspendLayout();
            this.tabPageGen.SuspendLayout();
            this.gbGenSample.SuspendLayout();
            this.gbStepGen.SuspendLayout();
            this.tabPageStruct.SuspendLayout();
            this.gbStructStep.SuspendLayout();
            this.tabPageCfgData.SuspendLayout();
            this.gbCfgData.SuspendLayout();
            this.tcRevNumber.SuspendLayout();
            this.tpRevDeltaTime.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRevTimeMax)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRevTimeMin)).BeginInit();
            this.tabPageRepl.SuspendLayout();
            this.tcReplType.SuspendLayout();
            this.tpReplRegex.SuspendLayout();
            this.tpReplWildcards.SuspendLayout();
            this.gbReplacement.SuspendLayout();
            this.gbReplPattern.SuspendLayout();
            this.gbReplStep.SuspendLayout();
            this.tabPageFields.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).BeginInit();
            this.tabPageFinal.SuspendLayout();
            this.gbFinalResult.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControlMain.Controls.Add(this.tabPageGen);
            this.tabControlMain.Controls.Add(this.tabPageStruct);
            this.tabControlMain.Controls.Add(this.tabPageCfgData);
            this.tabControlMain.Controls.Add(this.tabPageRepl);
            this.tabControlMain.Controls.Add(this.tabPageFields);
            this.tabControlMain.Controls.Add(this.tabPageFinal);
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(594, 278);
            this.tabControlMain.TabIndex = 0;
            this.tabControlMain.TabStop = false;
            // 
            // tabPageGen
            // 
            this.tabPageGen.Controls.Add(this.gbGenSample);
            this.tabPageGen.Controls.Add(this.labelStepType);
            this.tabPageGen.Controls.Add(this.gbStepGen);
            this.tabPageGen.Location = new System.Drawing.Point(4, 25);
            this.tabPageGen.Margin = new System.Windows.Forms.Padding(0);
            this.tabPageGen.Name = "tabPageGen";
            this.tabPageGen.Size = new System.Drawing.Size(586, 249);
            this.tabPageGen.TabIndex = 0;
            this.tabPageGen.Text = "Step 1 - Gen";
            this.tabPageGen.UseVisualStyleBackColor = true;
            // 
            // gbGenSample
            // 
            this.gbGenSample.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbGenSample.Controls.Add(this.editorStepGen);
            this.gbGenSample.Location = new System.Drawing.Point(196, 24);
            this.gbGenSample.Name = "gbGenSample";
            this.gbGenSample.Size = new System.Drawing.Size(394, 222);
            this.gbGenSample.TabIndex = 3;
            this.gbGenSample.TabStop = false;
            this.gbGenSample.Text = "Sample:";
            // 
            // editorStepGen
            // 
            this.editorStepGen.CodeCompletionEnabled = false;
            this.editorStepGen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorStepGen.Location = new System.Drawing.Point(3, 16);
            this.editorStepGen.Name = "editorStepGen";
            this.editorStepGen.Size = new System.Drawing.Size(388, 203);
            this.editorStepGen.TabIndex = 2;
            // 
            // labelStepType
            // 
            this.labelStepType.BackColor = System.Drawing.Color.Gray;
            this.labelStepType.ForeColor = System.Drawing.Color.White;
            this.labelStepType.Location = new System.Drawing.Point(0, 0);
            this.labelStepType.Name = "labelStepType";
            this.labelStepType.Size = new System.Drawing.Size(290, 21);
            this.labelStepType.TabIndex = 1;
            this.labelStepType.Text = "    Select type of generation:";
            this.labelStepType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbStepGen
            // 
            this.gbStepGen.Controls.Add(this.radioGenDirect);
            this.gbStepGen.Controls.Add(this.radioGenCppDefine);
            this.gbStepGen.Controls.Add(this.radioGenCppStruct);
            this.gbStepGen.Controls.Add(this.radioGenCSharpStruct);
            this.gbStepGen.Location = new System.Drawing.Point(7, 44);
            this.gbStepGen.Name = "gbStepGen";
            this.gbStepGen.Size = new System.Drawing.Size(185, 173);
            this.gbStepGen.TabIndex = 0;
            this.gbStepGen.TabStop = false;
            this.gbStepGen.Text = "Type:";
            // 
            // radioGenDirect
            // 
            this.radioGenDirect.AutoSize = true;
            this.radioGenDirect.Location = new System.Drawing.Point(6, 88);
            this.radioGenDirect.Name = "radioGenDirect";
            this.radioGenDirect.Size = new System.Drawing.Size(114, 17);
            this.radioGenDirect.TabIndex = 3;
            this.radioGenDirect.TabStop = true;
            this.radioGenDirect.Text = "Direct replacement";
            this.radioGenDirect.UseVisualStyleBackColor = true;
            this.radioGenDirect.CheckedChanged += new System.EventHandler(this.radioGenDirect_CheckedChanged);
            // 
            // radioGenCppDefine
            // 
            this.radioGenCppDefine.AutoSize = true;
            this.radioGenCppDefine.Location = new System.Drawing.Point(6, 65);
            this.radioGenCppDefine.Name = "radioGenCppDefine";
            this.radioGenCppDefine.Size = new System.Drawing.Size(177, 17);
            this.radioGenCppDefine.TabIndex = 2;
            this.radioGenCppDefine.TabStop = true;
            this.radioGenCppDefine.Text = "C++ macro definitions ( #define )";
            this.radioGenCppDefine.UseVisualStyleBackColor = true;
            this.radioGenCppDefine.CheckedChanged += new System.EventHandler(this.radioGenCppDefine_CheckedChanged);
            // 
            // radioGenCppStruct
            // 
            this.radioGenCppStruct.AutoSize = true;
            this.radioGenCppStruct.Location = new System.Drawing.Point(6, 42);
            this.radioGenCppStruct.Name = "radioGenCppStruct";
            this.radioGenCppStruct.Size = new System.Drawing.Size(75, 17);
            this.radioGenCppStruct.TabIndex = 1;
            this.radioGenCppStruct.TabStop = true;
            this.radioGenCppStruct.Text = "C++ Struct";
            this.radioGenCppStruct.UseVisualStyleBackColor = true;
            this.radioGenCppStruct.CheckedChanged += new System.EventHandler(this.radioGenCppStruct_CheckedChanged);
            // 
            // radioGenCSharpStruct
            // 
            this.radioGenCSharpStruct.AutoSize = true;
            this.radioGenCSharpStruct.Location = new System.Drawing.Point(6, 19);
            this.radioGenCSharpStruct.Name = "radioGenCSharpStruct";
            this.radioGenCSharpStruct.Size = new System.Drawing.Size(73, 17);
            this.radioGenCSharpStruct.TabIndex = 0;
            this.radioGenCSharpStruct.TabStop = true;
            this.radioGenCSharpStruct.Text = "C#  Struct";
            this.radioGenCSharpStruct.UseVisualStyleBackColor = true;
            this.radioGenCSharpStruct.CheckedChanged += new System.EventHandler(this.radioGenCSharpStruct_CheckedChanged);
            // 
            // tabPageStruct
            // 
            this.tabPageStruct.Controls.Add(this.labelStructStep);
            this.tabPageStruct.Controls.Add(this.gbStructStep);
            this.tabPageStruct.Location = new System.Drawing.Point(4, 25);
            this.tabPageStruct.Name = "tabPageStruct";
            this.tabPageStruct.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageStruct.Size = new System.Drawing.Size(586, 249);
            this.tabPageStruct.TabIndex = 1;
            this.tabPageStruct.Text = "Step 2 - Struct";
            this.tabPageStruct.UseVisualStyleBackColor = true;
            // 
            // labelStructStep
            // 
            this.labelStructStep.BackColor = System.Drawing.Color.Gray;
            this.labelStructStep.ForeColor = System.Drawing.Color.White;
            this.labelStructStep.Location = new System.Drawing.Point(0, 0);
            this.labelStructStep.Name = "labelStructStep";
            this.labelStructStep.Size = new System.Drawing.Size(290, 21);
            this.labelStructStep.TabIndex = 4;
            this.labelStructStep.Text = "    Configure struct or class:";
            this.labelStructStep.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // gbStructStep
            // 
            this.gbStructStep.Controls.Add(this.labelUpperCase);
            this.gbStructStep.Controls.Add(this.chkUpperCase);
            this.gbStructStep.Controls.Add(this.labelTypeOfNumber);
            this.gbStructStep.Controls.Add(this.cbNumberType);
            this.gbStructStep.Controls.Add(this.tbClassName);
            this.gbStructStep.Controls.Add(this.labelStructName);
            this.gbStructStep.Controls.Add(this.tbNamespace);
            this.gbStructStep.Controls.Add(this.labelNamespace);
            this.gbStructStep.Location = new System.Drawing.Point(100, 44);
            this.gbStructStep.Name = "gbStructStep";
            this.gbStructStep.Size = new System.Drawing.Size(367, 173);
            this.gbStructStep.TabIndex = 3;
            this.gbStructStep.TabStop = false;
            this.gbStructStep.Text = "Settings:";
            // 
            // labelUpperCase
            // 
            this.labelUpperCase.AutoSize = true;
            this.labelUpperCase.Location = new System.Drawing.Point(9, 120);
            this.labelUpperCase.Name = "labelUpperCase";
            this.labelUpperCase.Size = new System.Drawing.Size(107, 13);
            this.labelUpperCase.TabIndex = 8;
            this.labelUpperCase.Text = "Upper case for fields:";
            // 
            // chkUpperCase
            // 
            this.chkUpperCase.AutoSize = true;
            this.chkUpperCase.Location = new System.Drawing.Point(141, 121);
            this.chkUpperCase.Name = "chkUpperCase";
            this.chkUpperCase.Size = new System.Drawing.Size(15, 14);
            this.chkUpperCase.TabIndex = 7;
            this.chkUpperCase.UseVisualStyleBackColor = true;
            // 
            // labelTypeOfNumber
            // 
            this.labelTypeOfNumber.AutoSize = true;
            this.labelTypeOfNumber.Location = new System.Drawing.Point(9, 88);
            this.labelTypeOfNumber.Name = "labelTypeOfNumber";
            this.labelTypeOfNumber.Size = new System.Drawing.Size(84, 13);
            this.labelTypeOfNumber.TabIndex = 5;
            this.labelTypeOfNumber.Text = "Type of number:";
            // 
            // cbNumberType
            // 
            this.cbNumberType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNumberType.FormattingEnabled = true;
            this.cbNumberType.Location = new System.Drawing.Point(141, 88);
            this.cbNumberType.Name = "cbNumberType";
            this.cbNumberType.Size = new System.Drawing.Size(208, 21);
            this.cbNumberType.TabIndex = 4;
            // 
            // tbClassName
            // 
            this.tbClassName.Location = new System.Drawing.Point(141, 56);
            this.tbClassName.Name = "tbClassName";
            this.tbClassName.Size = new System.Drawing.Size(208, 20);
            this.tbClassName.TabIndex = 3;
            // 
            // labelStructName
            // 
            this.labelStructName.AutoSize = true;
            this.labelStructName.Location = new System.Drawing.Point(9, 56);
            this.labelStructName.Name = "labelStructName";
            this.labelStructName.Size = new System.Drawing.Size(67, 13);
            this.labelStructName.TabIndex = 2;
            this.labelStructName.Text = "Struct name:";
            // 
            // tbNamespace
            // 
            this.tbNamespace.Location = new System.Drawing.Point(141, 24);
            this.tbNamespace.Name = "tbNamespace";
            this.tbNamespace.Size = new System.Drawing.Size(208, 20);
            this.tbNamespace.TabIndex = 1;
            // 
            // labelNamespace
            // 
            this.labelNamespace.AutoSize = true;
            this.labelNamespace.Location = new System.Drawing.Point(9, 24);
            this.labelNamespace.Name = "labelNamespace";
            this.labelNamespace.Size = new System.Drawing.Size(67, 13);
            this.labelNamespace.TabIndex = 0;
            this.labelNamespace.Text = "Namespace:";
            // 
            // tabPageCfgData
            // 
            this.tabPageCfgData.Controls.Add(this.gbCfgData);
            this.tabPageCfgData.Controls.Add(this.labelCfgDataStep);
            this.tabPageCfgData.Location = new System.Drawing.Point(4, 25);
            this.tabPageCfgData.Name = "tabPageCfgData";
            this.tabPageCfgData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCfgData.Size = new System.Drawing.Size(586, 249);
            this.tabPageCfgData.TabIndex = 2;
            this.tabPageCfgData.Text = "Step 3 - Cfg Data";
            this.tabPageCfgData.UseVisualStyleBackColor = true;
            // 
            // gbCfgData
            // 
            this.gbCfgData.Controls.Add(this.pFixBorder);
            this.gbCfgData.Controls.Add(this.ftbOutputFile);
            this.gbCfgData.Controls.Add(this.ftbInputNum);
            this.gbCfgData.Controls.Add(this.tcRevNumber);
            this.gbCfgData.Controls.Add(this.labelInputNum);
            this.gbCfgData.Controls.Add(this.cbTypeRev);
            this.gbCfgData.Controls.Add(this.labelTypeRev);
            this.gbCfgData.Controls.Add(this.labelSCM);
            this.gbCfgData.Controls.Add(this.labelOutputFile);
            this.gbCfgData.Controls.Add(this.cbSCM);
            this.gbCfgData.Controls.Add(this.cbInputNum);
            this.gbCfgData.Controls.Add(this.labelInputNumType);
            this.gbCfgData.Location = new System.Drawing.Point(11, 24);
            this.gbCfgData.Name = "gbCfgData";
            this.gbCfgData.Size = new System.Drawing.Size(565, 200);
            this.gbCfgData.TabIndex = 19;
            this.gbCfgData.TabStop = false;
            this.gbCfgData.Text = "Data";
            // 
            // pFixBorder
            // 
            this.pFixBorder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(221)))), ((int)(((byte)(221)))), ((int)(((byte)(221)))));
            this.pFixBorder.Location = new System.Drawing.Point(280, 6);
            this.pFixBorder.Name = "pFixBorder";
            this.pFixBorder.Size = new System.Drawing.Size(280, 1);
            this.pFixBorder.TabIndex = 31;
            // 
            // ftbOutputFile
            // 
            this.ftbOutputFile.FileName = "";
            this.ftbOutputFile.HideButton = false;
            this.ftbOutputFile.IsSaveDialog = true;
            this.ftbOutputFile.Location = new System.Drawing.Point(135, 145);
            this.ftbOutputFile.Name = "ftbOutputFile";
            this.ftbOutputFile.PrefixToPath = "$(SolutionDir)";
            this.ftbOutputFile.Size = new System.Drawing.Size(158, 20);
            this.ftbOutputFile.SupressInitialDirectoryFromPath = true;
            this.ftbOutputFile.TabIndex = 30;
            // 
            // ftbInputNum
            // 
            this.ftbInputNum.FileName = "";
            this.ftbInputNum.HideButton = false;
            this.ftbInputNum.IsSaveDialog = false;
            this.ftbInputNum.Location = new System.Drawing.Point(135, 86);
            this.ftbInputNum.Name = "ftbInputNum";
            this.ftbInputNum.PrefixToPath = "$(SolutionDir)";
            this.ftbInputNum.Size = new System.Drawing.Size(158, 20);
            this.ftbInputNum.SupressInitialDirectoryFromPath = true;
            this.ftbInputNum.TabIndex = 29;
            // 
            // tcRevNumber
            // 
            this.tcRevNumber.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcRevNumber.Controls.Add(this.tpRevRaw);
            this.tcRevNumber.Controls.Add(this.tpRevDeltaTime);
            this.tcRevNumber.ItemSize = new System.Drawing.Size(0, 21);
            this.tcRevNumber.Location = new System.Drawing.Point(299, -5);
            this.tcRevNumber.Multiline = true;
            this.tcRevNumber.Name = "tcRevNumber";
            this.tcRevNumber.SelectedIndex = 0;
            this.tcRevNumber.Size = new System.Drawing.Size(260, 117);
            this.tcRevNumber.TabIndex = 26;
            this.tcRevNumber.TabStop = false;
            // 
            // tpRevRaw
            // 
            this.tpRevRaw.Location = new System.Drawing.Point(4, 25);
            this.tpRevRaw.Name = "tpRevRaw";
            this.tpRevRaw.Padding = new System.Windows.Forms.Padding(3);
            this.tpRevRaw.Size = new System.Drawing.Size(252, 88);
            this.tpRevRaw.TabIndex = 1;
            this.tpRevRaw.Text = "Raw";
            this.tpRevRaw.UseVisualStyleBackColor = true;
            // 
            // tpRevDeltaTime
            // 
            this.tpRevDeltaTime.Controls.Add(this.numRevTimeMax);
            this.tpRevDeltaTime.Controls.Add(this.numRevTimeMin);
            this.tpRevDeltaTime.Controls.Add(this.chkRevTimeMod);
            this.tpRevDeltaTime.Controls.Add(this.cbRevTimeType);
            this.tpRevDeltaTime.Controls.Add(this.labelRevTimeType);
            this.tpRevDeltaTime.Controls.Add(this.dtRevTimeBase);
            this.tpRevDeltaTime.Controls.Add(this.labelRevTimeBase);
            this.tpRevDeltaTime.Location = new System.Drawing.Point(4, 25);
            this.tpRevDeltaTime.Name = "tpRevDeltaTime";
            this.tpRevDeltaTime.Padding = new System.Windows.Forms.Padding(3);
            this.tpRevDeltaTime.Size = new System.Drawing.Size(252, 88);
            this.tpRevDeltaTime.TabIndex = 0;
            this.tpRevDeltaTime.Text = "DeltaTime";
            this.tpRevDeltaTime.UseVisualStyleBackColor = true;
            // 
            // numRevTimeMax
            // 
            this.numRevTimeMax.Location = new System.Drawing.Point(173, 58);
            this.numRevTimeMax.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numRevTimeMax.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numRevTimeMax.Name = "numRevTimeMax";
            this.numRevTimeMax.Size = new System.Drawing.Size(72, 20);
            this.numRevTimeMax.TabIndex = 27;
            this.numRevTimeMax.Value = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            // 
            // numRevTimeMin
            // 
            this.numRevTimeMin.Location = new System.Drawing.Point(106, 58);
            this.numRevTimeMin.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.numRevTimeMin.Minimum = new decimal(new int[] {
            100000000,
            0,
            0,
            -2147483648});
            this.numRevTimeMin.Name = "numRevTimeMin";
            this.numRevTimeMin.Size = new System.Drawing.Size(61, 20);
            this.numRevTimeMin.TabIndex = 26;
            this.numRevTimeMin.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // chkRevTimeMod
            // 
            this.chkRevTimeMod.AutoSize = true;
            this.chkRevTimeMod.Location = new System.Drawing.Point(6, 62);
            this.chkRevTimeMod.Name = "chkRevTimeMod";
            this.chkRevTimeMod.Size = new System.Drawing.Size(105, 17);
            this.chkRevTimeMod.TabIndex = 25;
            this.chkRevTimeMod.Text = "Modulo min-max:";
            this.chkRevTimeMod.UseVisualStyleBackColor = true;
            // 
            // cbRevTimeType
            // 
            this.cbRevTimeType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbRevTimeType.FormattingEnabled = true;
            this.cbRevTimeType.Location = new System.Drawing.Point(91, 6);
            this.cbRevTimeType.Name = "cbRevTimeType";
            this.cbRevTimeType.Size = new System.Drawing.Size(155, 21);
            this.cbRevTimeType.TabIndex = 21;
            // 
            // labelRevTimeType
            // 
            this.labelRevTimeType.AutoSize = true;
            this.labelRevTimeType.Location = new System.Drawing.Point(3, 9);
            this.labelRevTimeType.Name = "labelRevTimeType";
            this.labelRevTimeType.Size = new System.Drawing.Size(82, 13);
            this.labelRevTimeType.TabIndex = 20;
            this.labelRevTimeType.Text = "Interval for time:";
            // 
            // dtRevTimeBase
            // 
            this.dtRevTimeBase.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtRevTimeBase.Location = new System.Drawing.Point(91, 36);
            this.dtRevTimeBase.Name = "dtRevTimeBase";
            this.dtRevTimeBase.Size = new System.Drawing.Size(154, 20);
            this.dtRevTimeBase.TabIndex = 24;
            // 
            // labelRevTimeBase
            // 
            this.labelRevTimeBase.AutoSize = true;
            this.labelRevTimeBase.Location = new System.Drawing.Point(3, 36);
            this.labelRevTimeBase.Name = "labelRevTimeBase";
            this.labelRevTimeBase.Size = new System.Drawing.Size(56, 13);
            this.labelRevTimeBase.TabIndex = 22;
            this.labelRevTimeBase.Text = "The Base:";
            // 
            // labelInputNum
            // 
            this.labelInputNum.AutoSize = true;
            this.labelInputNum.Location = new System.Drawing.Point(6, 86);
            this.labelInputNum.Name = "labelInputNum";
            this.labelInputNum.Size = new System.Drawing.Size(72, 13);
            this.labelInputNum.TabIndex = 25;
            this.labelInputNum.Text = "Input number:";
            // 
            // cbTypeRev
            // 
            this.cbTypeRev.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTypeRev.FormattingEnabled = true;
            this.cbTypeRev.Location = new System.Drawing.Point(135, 26);
            this.cbTypeRev.Name = "cbTypeRev";
            this.cbTypeRev.Size = new System.Drawing.Size(158, 21);
            this.cbTypeRev.TabIndex = 19;
            this.cbTypeRev.SelectedIndexChanged += new System.EventHandler(this.cbTypeRev_SelectedIndexChanged);
            // 
            // labelTypeRev
            // 
            this.labelTypeRev.AutoSize = true;
            this.labelTypeRev.Location = new System.Drawing.Point(6, 29);
            this.labelTypeRev.Name = "labelTypeRev";
            this.labelTypeRev.Size = new System.Drawing.Size(123, 13);
            this.labelTypeRev.TabIndex = 18;
            this.labelTypeRev.Text = "Type of revision number:";
            // 
            // labelSCM
            // 
            this.labelSCM.AutoSize = true;
            this.labelSCM.Location = new System.Drawing.Point(6, 115);
            this.labelSCM.Name = "labelSCM";
            this.labelSCM.Size = new System.Drawing.Size(79, 13);
            this.labelSCM.TabIndex = 8;
            this.labelSCM.Text = "Use SCM data:";
            // 
            // labelOutputFile
            // 
            this.labelOutputFile.AutoSize = true;
            this.labelOutputFile.Location = new System.Drawing.Point(6, 145);
            this.labelOutputFile.Name = "labelOutputFile";
            this.labelOutputFile.Size = new System.Drawing.Size(58, 13);
            this.labelOutputFile.TabIndex = 13;
            this.labelOutputFile.Text = "Output file:";
            // 
            // cbSCM
            // 
            this.cbSCM.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSCM.FormattingEnabled = true;
            this.cbSCM.Location = new System.Drawing.Point(135, 115);
            this.cbSCM.Name = "cbSCM";
            this.cbSCM.Size = new System.Drawing.Size(158, 21);
            this.cbSCM.TabIndex = 7;
            this.cbSCM.SelectedIndexChanged += new System.EventHandler(this.cbSCM_SelectedIndexChanged);
            // 
            // cbInputNum
            // 
            this.cbInputNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbInputNum.FormattingEnabled = true;
            this.cbInputNum.Location = new System.Drawing.Point(135, 56);
            this.cbInputNum.Name = "cbInputNum";
            this.cbInputNum.Size = new System.Drawing.Size(158, 21);
            this.cbInputNum.TabIndex = 10;
            this.cbInputNum.SelectedIndexChanged += new System.EventHandler(this.cbInputNum_SelectedIndexChanged);
            // 
            // labelInputNumType
            // 
            this.labelInputNumType.AutoSize = true;
            this.labelInputNumType.Location = new System.Drawing.Point(6, 56);
            this.labelInputNumType.Name = "labelInputNumType";
            this.labelInputNumType.Size = new System.Drawing.Size(110, 13);
            this.labelInputNumType.TabIndex = 11;
            this.labelInputNumType.Text = "Type of input number:";
            // 
            // labelCfgDataStep
            // 
            this.labelCfgDataStep.BackColor = System.Drawing.Color.Gray;
            this.labelCfgDataStep.ForeColor = System.Drawing.Color.White;
            this.labelCfgDataStep.Location = new System.Drawing.Point(0, 0);
            this.labelCfgDataStep.Name = "labelCfgDataStep";
            this.labelCfgDataStep.Size = new System.Drawing.Size(290, 21);
            this.labelCfgDataStep.TabIndex = 18;
            this.labelCfgDataStep.Text = "    Configure data:";
            this.labelCfgDataStep.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageRepl
            // 
            this.tabPageRepl.Controls.Add(this.labelSampleSource);
            this.tabPageRepl.Controls.Add(this.labelExampleSource);
            this.tabPageRepl.Controls.Add(this.tcReplType);
            this.tabPageRepl.Controls.Add(this.gbReplacement);
            this.tabPageRepl.Controls.Add(this.gbReplPattern);
            this.tabPageRepl.Controls.Add(this.gbReplStep);
            this.tabPageRepl.Controls.Add(this.labelReplStep);
            this.tabPageRepl.Location = new System.Drawing.Point(4, 25);
            this.tabPageRepl.Name = "tabPageRepl";
            this.tabPageRepl.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRepl.Size = new System.Drawing.Size(586, 249);
            this.tabPageRepl.TabIndex = 3;
            this.tabPageRepl.Text = "Step 2 - Repl";
            this.tabPageRepl.UseVisualStyleBackColor = true;
            // 
            // labelSampleSource
            // 
            this.labelSampleSource.AutoSize = true;
            this.labelSampleSource.Location = new System.Drawing.Point(143, 227);
            this.labelSampleSource.Name = "labelSampleSource";
            this.labelSampleSource.Size = new System.Drawing.Size(45, 13);
            this.labelSampleSource.TabIndex = 29;
            this.labelSampleSource.Text = "Sample:";
            // 
            // labelExampleSource
            // 
            this.labelExampleSource.AutoSize = true;
            this.labelExampleSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.labelExampleSource.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.labelExampleSource.Location = new System.Drawing.Point(194, 227);
            this.labelExampleSource.Name = "labelExampleSource";
            this.labelExampleSource.Size = new System.Drawing.Size(12, 15);
            this.labelExampleSource.TabIndex = 28;
            this.labelExampleSource.Text = "-";
            // 
            // tcReplType
            // 
            this.tcReplType.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcReplType.Controls.Add(this.tpReplRegex);
            this.tcReplType.Controls.Add(this.tpReplWildcards);
            this.tcReplType.ItemSize = new System.Drawing.Size(0, 21);
            this.tcReplType.Location = new System.Drawing.Point(392, 9);
            this.tcReplType.Multiline = true;
            this.tcReplType.Name = "tcReplType";
            this.tcReplType.SelectedIndex = 0;
            this.tcReplType.Size = new System.Drawing.Size(173, 100);
            this.tcReplType.TabIndex = 27;
            this.tcReplType.TabStop = false;
            // 
            // tpReplRegex
            // 
            this.tpReplRegex.Controls.Add(this.labelHelpRegexLangCaption);
            this.tpReplRegex.Controls.Add(this.linkRegex);
            this.tpReplRegex.Location = new System.Drawing.Point(4, 25);
            this.tpReplRegex.Name = "tpReplRegex";
            this.tpReplRegex.Padding = new System.Windows.Forms.Padding(3);
            this.tpReplRegex.Size = new System.Drawing.Size(165, 71);
            this.tpReplRegex.TabIndex = 1;
            this.tpReplRegex.Text = "Regex";
            this.tpReplRegex.UseVisualStyleBackColor = true;
            // 
            // labelHelpRegexLangCaption
            // 
            this.labelHelpRegexLangCaption.AutoSize = true;
            this.labelHelpRegexLangCaption.Location = new System.Drawing.Point(2, 20);
            this.labelHelpRegexLangCaption.Name = "labelHelpRegexLangCaption";
            this.labelHelpRegexLangCaption.Size = new System.Drawing.Size(152, 13);
            this.labelHelpRegexLangCaption.TabIndex = 1;
            this.labelHelpRegexLangCaption.Text = "Regular Expression Language ";
            // 
            // linkRegex
            // 
            this.linkRegex.AutoSize = true;
            this.linkRegex.Location = new System.Drawing.Point(66, 46);
            this.linkRegex.Name = "linkRegex";
            this.linkRegex.Size = new System.Drawing.Size(88, 13);
            this.linkRegex.TabIndex = 0;
            this.linkRegex.TabStop = true;
            this.linkRegex.Text = "Quick Reference";
            this.linkRegex.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRegex_LinkClicked);
            // 
            // tpReplWildcards
            // 
            this.tpReplWildcards.Controls.Add(this.tbHelpWildcards);
            this.tpReplWildcards.Location = new System.Drawing.Point(4, 25);
            this.tpReplWildcards.Name = "tpReplWildcards";
            this.tpReplWildcards.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.tpReplWildcards.Size = new System.Drawing.Size(165, 71);
            this.tpReplWildcards.TabIndex = 2;
            this.tpReplWildcards.Text = "Wildcards";
            this.tpReplWildcards.UseVisualStyleBackColor = true;
            // 
            // tbHelpWildcards
            // 
            this.tbHelpWildcards.BackColor = System.Drawing.SystemColors.Control;
            this.tbHelpWildcards.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbHelpWildcards.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHelpWildcards.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbHelpWildcards.Location = new System.Drawing.Point(0, 10);
            this.tbHelpWildcards.Multiline = true;
            this.tbHelpWildcards.Name = "tbHelpWildcards";
            this.tbHelpWildcards.ReadOnly = true;
            this.tbHelpWildcards.Size = new System.Drawing.Size(165, 61);
            this.tbHelpWildcards.TabIndex = 0;
            this.tbHelpWildcards.TabStop = false;
            this.tbHelpWildcards.Text = "Matches any character:\r\n *   - 0 or more times.\r\n +   - 1 or more times.\r\n ?   - " +
    "1 single.";
            // 
            // gbReplacement
            // 
            this.gbReplacement.Controls.Add(this.labelReplPostfix);
            this.gbReplacement.Controls.Add(this.tbReplPrefix);
            this.gbReplacement.Controls.Add(this.labelReplPrefix);
            this.gbReplacement.Controls.Add(this.labelReplSource);
            this.gbReplacement.Controls.Add(this.tbReplPostfix);
            this.gbReplacement.Controls.Add(this.cbReplSource);
            this.gbReplacement.Location = new System.Drawing.Point(38, 162);
            this.gbReplacement.Name = "gbReplacement";
            this.gbReplacement.Size = new System.Drawing.Size(446, 62);
            this.gbReplacement.TabIndex = 19;
            this.gbReplacement.TabStop = false;
            this.gbReplacement.Text = "Replacement:";
            // 
            // labelReplPostfix
            // 
            this.labelReplPostfix.AutoSize = true;
            this.labelReplPostfix.Location = new System.Drawing.Point(297, 19);
            this.labelReplPostfix.Name = "labelReplPostfix";
            this.labelReplPostfix.Size = new System.Drawing.Size(41, 13);
            this.labelReplPostfix.TabIndex = 25;
            this.labelReplPostfix.Text = "Postfix:";
            // 
            // tbReplPrefix
            // 
            this.tbReplPrefix.Location = new System.Drawing.Point(6, 35);
            this.tbReplPrefix.Name = "tbReplPrefix";
            this.tbReplPrefix.Size = new System.Drawing.Size(144, 20);
            this.tbReplPrefix.TabIndex = 21;
            // 
            // labelReplPrefix
            // 
            this.labelReplPrefix.AutoSize = true;
            this.labelReplPrefix.Location = new System.Drawing.Point(3, 19);
            this.labelReplPrefix.Name = "labelReplPrefix";
            this.labelReplPrefix.Size = new System.Drawing.Size(36, 13);
            this.labelReplPrefix.TabIndex = 24;
            this.labelReplPrefix.Text = "Prefix:";
            // 
            // labelReplSource
            // 
            this.labelReplSource.AutoSize = true;
            this.labelReplSource.Location = new System.Drawing.Point(153, 18);
            this.labelReplSource.Name = "labelReplSource";
            this.labelReplSource.Size = new System.Drawing.Size(44, 13);
            this.labelReplSource.TabIndex = 14;
            this.labelReplSource.Text = "Source:";
            // 
            // tbReplPostfix
            // 
            this.tbReplPostfix.Location = new System.Drawing.Point(297, 35);
            this.tbReplPostfix.Name = "tbReplPostfix";
            this.tbReplPostfix.Size = new System.Drawing.Size(141, 20);
            this.tbReplPostfix.TabIndex = 22;
            // 
            // cbReplSource
            // 
            this.cbReplSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReplSource.DropDownWidth = 200;
            this.cbReplSource.FormattingEnabled = true;
            this.cbReplSource.Location = new System.Drawing.Point(156, 34);
            this.cbReplSource.Name = "cbReplSource";
            this.cbReplSource.Size = new System.Drawing.Size(135, 21);
            this.cbReplSource.TabIndex = 18;
            this.cbReplSource.SelectedIndexChanged += new System.EventHandler(this.cbReplSource_SelectedIndexChanged);
            // 
            // gbReplPattern
            // 
            this.gbReplPattern.Controls.Add(this.tbReplPattern);
            this.gbReplPattern.Location = new System.Drawing.Point(38, 112);
            this.gbReplPattern.Name = "gbReplPattern";
            this.gbReplPattern.Size = new System.Drawing.Size(446, 44);
            this.gbReplPattern.TabIndex = 18;
            this.gbReplPattern.TabStop = false;
            this.gbReplPattern.Text = "Pattern for replacing:";
            // 
            // tbReplPattern
            // 
            this.tbReplPattern.Location = new System.Drawing.Point(6, 19);
            this.tbReplPattern.Name = "tbReplPattern";
            this.tbReplPattern.Size = new System.Drawing.Size(432, 20);
            this.tbReplPattern.TabIndex = 19;
            // 
            // gbReplStep
            // 
            this.gbReplStep.Controls.Add(this.ftbReplFile);
            this.gbReplStep.Controls.Add(this.cbReplType);
            this.gbReplStep.Controls.Add(this.labelReplFile);
            this.gbReplStep.Controls.Add(this.labelReplType);
            this.gbReplStep.Location = new System.Drawing.Point(38, 35);
            this.gbReplStep.Name = "gbReplStep";
            this.gbReplStep.Size = new System.Drawing.Size(350, 71);
            this.gbReplStep.TabIndex = 17;
            this.gbReplStep.TabStop = false;
            // 
            // ftbReplFile
            // 
            this.ftbReplFile.FileName = "";
            this.ftbReplFile.HideButton = false;
            this.ftbReplFile.IsSaveDialog = false;
            this.ftbReplFile.Location = new System.Drawing.Point(123, 16);
            this.ftbReplFile.Name = "ftbReplFile";
            this.ftbReplFile.PrefixToPath = "$(SolutionDir)";
            this.ftbReplFile.Size = new System.Drawing.Size(219, 20);
            this.ftbReplFile.SupressInitialDirectoryFromPath = true;
            this.ftbReplFile.TabIndex = 31;
            // 
            // cbReplType
            // 
            this.cbReplType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbReplType.FormattingEnabled = true;
            this.cbReplType.Location = new System.Drawing.Point(123, 42);
            this.cbReplType.Name = "cbReplType";
            this.cbReplType.Size = new System.Drawing.Size(219, 21);
            this.cbReplType.TabIndex = 17;
            this.cbReplType.SelectedIndexChanged += new System.EventHandler(this.cbReplType_SelectedIndexChanged);
            // 
            // labelReplFile
            // 
            this.labelReplFile.AutoSize = true;
            this.labelReplFile.Location = new System.Drawing.Point(6, 16);
            this.labelReplFile.Name = "labelReplFile";
            this.labelReplFile.Size = new System.Drawing.Size(111, 13);
            this.labelReplFile.TabIndex = 12;
            this.labelReplFile.Text = "File for replacing data:";
            // 
            // labelReplType
            // 
            this.labelReplType.AutoSize = true;
            this.labelReplType.Location = new System.Drawing.Point(6, 42);
            this.labelReplType.Name = "labelReplType";
            this.labelReplType.Size = new System.Drawing.Size(107, 13);
            this.labelReplType.TabIndex = 13;
            this.labelReplType.Text = "Type of replacement:";
            // 
            // labelReplStep
            // 
            this.labelReplStep.BackColor = System.Drawing.Color.Gray;
            this.labelReplStep.ForeColor = System.Drawing.Color.White;
            this.labelReplStep.Location = new System.Drawing.Point(0, 0);
            this.labelReplStep.Name = "labelReplStep";
            this.labelReplStep.Size = new System.Drawing.Size(290, 21);
            this.labelReplStep.TabIndex = 15;
            this.labelReplStep.Text = "    Configure of replacement:";
            this.labelReplStep.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tabPageFields
            // 
            this.tabPageFields.Controls.Add(this.labelFields);
            this.tabPageFields.Controls.Add(this.dgvFields);
            this.tabPageFields.Location = new System.Drawing.Point(4, 25);
            this.tabPageFields.Name = "tabPageFields";
            this.tabPageFields.Padding = new System.Windows.Forms.Padding(0, 22, 0, 0);
            this.tabPageFields.Size = new System.Drawing.Size(586, 249);
            this.tabPageFields.TabIndex = 4;
            this.tabPageFields.Text = "Step 4 - Fields";
            this.tabPageFields.UseVisualStyleBackColor = true;
            // 
            // labelFields
            // 
            this.labelFields.BackColor = System.Drawing.Color.Gray;
            this.labelFields.ForeColor = System.Drawing.Color.White;
            this.labelFields.Location = new System.Drawing.Point(0, 0);
            this.labelFields.Name = "labelFields";
            this.labelFields.Size = new System.Drawing.Size(290, 21);
            this.labelFields.TabIndex = 16;
            this.labelFields.Text = "    Optional step. Reconfiguring of available fields:";
            this.labelFields.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // dgvFields
            // 
            this.dgvFields.AllowUserToAddRows = false;
            this.dgvFields.AllowUserToDeleteRows = false;
            this.dgvFields.AllowUserToResizeRows = false;
            this.dgvFields.AlwaysSelected = false;
            this.dgvFields.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvFields.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvFieldsEnabled,
            this.dgvFieldsNameOrigin,
            this.dgvFieldsNameNew,
            this.dgvFieldsDescription});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFields.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFields.DragDropSortable = false;
            this.dgvFields.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvFields.Location = new System.Drawing.Point(0, 22);
            this.dgvFields.MultiSelect = false;
            this.dgvFields.Name = "dgvFields";
            this.dgvFields.NumberingForRowsHeader = false;
            this.dgvFields.RowHeadersVisible = false;
            this.dgvFields.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(252)))), ((int)(((byte)(248)))));
            this.dgvFields.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgvFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvFields.Size = new System.Drawing.Size(586, 227);
            this.dgvFields.TabIndex = 8;
            // 
            // dgvFieldsEnabled
            // 
            this.dgvFieldsEnabled.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dgvFieldsEnabled.FalseValue = "False";
            this.dgvFieldsEnabled.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.dgvFieldsEnabled.HeaderText = "Use";
            this.dgvFieldsEnabled.IndeterminateValue = "False";
            this.dgvFieldsEnabled.MinimumWidth = 40;
            this.dgvFieldsEnabled.Name = "dgvFieldsEnabled";
            this.dgvFieldsEnabled.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvFieldsEnabled.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.dgvFieldsEnabled.TrueValue = "True";
            this.dgvFieldsEnabled.Width = 40;
            // 
            // dgvFieldsNameOrigin
            // 
            this.dgvFieldsNameOrigin.HeaderText = "Original";
            this.dgvFieldsNameOrigin.MinimumWidth = 100;
            this.dgvFieldsNameOrigin.Name = "dgvFieldsNameOrigin";
            this.dgvFieldsNameOrigin.ReadOnly = true;
            this.dgvFieldsNameOrigin.ToolTipText = "Original name";
            this.dgvFieldsNameOrigin.Width = 190;
            // 
            // dgvFieldsNameNew
            // 
            this.dgvFieldsNameNew.HeaderText = "New";
            this.dgvFieldsNameNew.MinimumWidth = 100;
            this.dgvFieldsNameNew.Name = "dgvFieldsNameNew";
            this.dgvFieldsNameNew.ToolTipText = "New name if needed";
            this.dgvFieldsNameNew.Width = 180;
            // 
            // dgvFieldsDescription
            // 
            this.dgvFieldsDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvFieldsDescription.HeaderText = "Description";
            this.dgvFieldsDescription.MinimumWidth = 40;
            this.dgvFieldsDescription.Name = "dgvFieldsDescription";
            this.dgvFieldsDescription.ReadOnly = true;
            // 
            // tabPageFinal
            // 
            this.tabPageFinal.Controls.Add(this.labelFinal);
            this.tabPageFinal.Controls.Add(this.btnFinalCreateAction);
            this.tabPageFinal.Controls.Add(this.btnFinalCopy);
            this.tabPageFinal.Controls.Add(this.gbFinalResult);
            this.tabPageFinal.Location = new System.Drawing.Point(4, 25);
            this.tabPageFinal.Name = "tabPageFinal";
            this.tabPageFinal.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFinal.Size = new System.Drawing.Size(586, 249);
            this.tabPageFinal.TabIndex = 5;
            this.tabPageFinal.Text = "Step 5 - Final";
            this.tabPageFinal.UseVisualStyleBackColor = true;
            // 
            // labelFinal
            // 
            this.labelFinal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelFinal.BackColor = System.Drawing.Color.Gray;
            this.labelFinal.ForeColor = System.Drawing.Color.White;
            this.labelFinal.Location = new System.Drawing.Point(0, 0);
            this.labelFinal.Name = "labelFinal";
            this.labelFinal.Size = new System.Drawing.Size(586, 21);
            this.labelFinal.TabIndex = 17;
            this.labelFinal.Text = "    Done. Use final script below or create new action from it.";
            this.labelFinal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnFinalCreateAction
            // 
            this.btnFinalCreateAction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinalCreateAction.Location = new System.Drawing.Point(501, 220);
            this.btnFinalCreateAction.Name = "btnFinalCreateAction";
            this.btnFinalCreateAction.Size = new System.Drawing.Size(82, 23);
            this.btnFinalCreateAction.TabIndex = 6;
            this.btnFinalCreateAction.Text = "Create Action";
            this.btnFinalCreateAction.UseVisualStyleBackColor = true;
            this.btnFinalCreateAction.Click += new System.EventHandler(this.btnFinalCreateAction_Click);
            // 
            // btnFinalCopy
            // 
            this.btnFinalCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFinalCopy.Location = new System.Drawing.Point(501, 191);
            this.btnFinalCopy.Name = "btnFinalCopy";
            this.btnFinalCopy.Size = new System.Drawing.Size(82, 23);
            this.btnFinalCopy.TabIndex = 5;
            this.btnFinalCopy.Text = "Copy script";
            this.btnFinalCopy.UseVisualStyleBackColor = true;
            this.btnFinalCopy.Click += new System.EventHandler(this.btnFinalCopy_Click);
            // 
            // gbFinalResult
            // 
            this.gbFinalResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbFinalResult.Controls.Add(this.editorFinalScript);
            this.gbFinalResult.Location = new System.Drawing.Point(0, 22);
            this.gbFinalResult.Name = "gbFinalResult";
            this.gbFinalResult.Size = new System.Drawing.Size(495, 221);
            this.gbFinalResult.TabIndex = 4;
            this.gbFinalResult.TabStop = false;
            this.gbFinalResult.Text = "Result:";
            // 
            // editorFinalScript
            // 
            this.editorFinalScript.CodeCompletionEnabled = false;
            this.editorFinalScript.Dock = System.Windows.Forms.DockStyle.Fill;
            this.editorFinalScript.Location = new System.Drawing.Point(3, 16);
            this.editorFinalScript.Name = "editorFinalScript";
            this.editorFinalScript.Size = new System.Drawing.Size(489, 202);
            this.editorFinalScript.TabIndex = 2;
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.btnManually);
            this.panelBottom.Controls.Add(this.btnNextStep);
            this.panelBottom.Controls.Add(this.btnPrevStep);
            this.panelBottom.Controls.Add(this.labelDone);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 278);
            this.panelBottom.Margin = new System.Windows.Forms.Padding(0);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(594, 35);
            this.panelBottom.TabIndex = 1;
            // 
            // btnManually
            // 
            this.btnManually.Location = new System.Drawing.Point(4, 6);
            this.btnManually.Name = "btnManually";
            this.btnManually.Size = new System.Drawing.Size(75, 23);
            this.btnManually.TabIndex = 2;
            this.btnManually.Text = "Manually";
            this.btnManually.UseVisualStyleBackColor = true;
            this.btnManually.Click += new System.EventHandler(this.btnManually_Click);
            // 
            // btnNextStep
            // 
            this.btnNextStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextStep.Location = new System.Drawing.Point(505, 6);
            this.btnNextStep.Name = "btnNextStep";
            this.btnNextStep.Size = new System.Drawing.Size(85, 23);
            this.btnNextStep.TabIndex = 0;
            this.btnNextStep.Text = "Next step";
            this.btnNextStep.UseVisualStyleBackColor = true;
            this.btnNextStep.Click += new System.EventHandler(this.btnNextStep_Click);
            // 
            // btnPrevStep
            // 
            this.btnPrevStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevStep.Location = new System.Drawing.Point(424, 6);
            this.btnPrevStep.Name = "btnPrevStep";
            this.btnPrevStep.Size = new System.Drawing.Size(75, 23);
            this.btnPrevStep.TabIndex = 1;
            this.btnPrevStep.Text = "Back";
            this.btnPrevStep.UseVisualStyleBackColor = true;
            this.btnPrevStep.Click += new System.EventHandler(this.btnPrevStep_Click);
            // 
            // labelDone
            // 
            this.labelDone.AutoSize = true;
            this.labelDone.Location = new System.Drawing.Point(530, 11);
            this.labelDone.Name = "labelDone";
            this.labelDone.Size = new System.Drawing.Size(33, 13);
            this.labelDone.TabIndex = 3;
            this.labelDone.Text = "Done";
            // 
            // panelMain
            // 
            this.panelMain.Controls.Add(this.tabControlMain);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(0, 0);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(594, 278);
            this.panelMain.TabIndex = 2;
            // 
            // VersionFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(594, 313);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelBottom);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(600, 342);
            this.Name = "VersionFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Automatic Version Numbering";
            this.Load += new System.EventHandler(this.VersionFrm_Load);
            this.tabControlMain.ResumeLayout(false);
            this.tabPageGen.ResumeLayout(false);
            this.gbGenSample.ResumeLayout(false);
            this.gbStepGen.ResumeLayout(false);
            this.gbStepGen.PerformLayout();
            this.tabPageStruct.ResumeLayout(false);
            this.gbStructStep.ResumeLayout(false);
            this.gbStructStep.PerformLayout();
            this.tabPageCfgData.ResumeLayout(false);
            this.gbCfgData.ResumeLayout(false);
            this.gbCfgData.PerformLayout();
            this.tcRevNumber.ResumeLayout(false);
            this.tpRevDeltaTime.ResumeLayout(false);
            this.tpRevDeltaTime.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRevTimeMax)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRevTimeMin)).EndInit();
            this.tabPageRepl.ResumeLayout(false);
            this.tabPageRepl.PerformLayout();
            this.tcReplType.ResumeLayout(false);
            this.tpReplRegex.ResumeLayout(false);
            this.tpReplRegex.PerformLayout();
            this.tpReplWildcards.ResumeLayout(false);
            this.tpReplWildcards.PerformLayout();
            this.gbReplacement.ResumeLayout(false);
            this.gbReplacement.PerformLayout();
            this.gbReplPattern.ResumeLayout(false);
            this.gbReplPattern.PerformLayout();
            this.gbReplStep.ResumeLayout(false);
            this.gbReplStep.PerformLayout();
            this.tabPageFields.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFields)).EndInit();
            this.tabPageFinal.ResumeLayout(false);
            this.gbFinalResult.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageGen;
        private System.Windows.Forms.TabPage tabPageStruct;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnManually;
        private System.Windows.Forms.Button btnNextStep;
        private System.Windows.Forms.Button btnPrevStep;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label labelStepType;
        private System.Windows.Forms.GroupBox gbStepGen;
        private System.Windows.Forms.RadioButton radioGenDirect;
        private System.Windows.Forms.RadioButton radioGenCppDefine;
        private System.Windows.Forms.RadioButton radioGenCppStruct;
        private System.Windows.Forms.RadioButton radioGenCSharpStruct;
        private System.Windows.Forms.GroupBox gbGenSample;
        private Controls.TextEditor editorStepGen;
        private System.Windows.Forms.GroupBox gbStructStep;
        private System.Windows.Forms.ComboBox cbNumberType;
        private System.Windows.Forms.TextBox tbClassName;
        private System.Windows.Forms.Label labelStructName;
        private System.Windows.Forms.TextBox tbNamespace;
        private System.Windows.Forms.Label labelNamespace;
        private System.Windows.Forms.TabPage tabPageCfgData;
        private System.Windows.Forms.ComboBox cbSCM;
        private System.Windows.Forms.TabPage tabPageRepl;
        private System.Windows.Forms.Label labelTypeOfNumber;
        private System.Windows.Forms.Label labelOutputFile;
        private System.Windows.Forms.Label labelInputNumType;
        private System.Windows.Forms.ComboBox cbInputNum;
        private System.Windows.Forms.Label labelSCM;
        private System.Windows.Forms.Label labelReplSource;
        private System.Windows.Forms.Label labelReplType;
        private System.Windows.Forms.Label labelReplFile;
        private System.Windows.Forms.Label labelStructStep;
        private System.Windows.Forms.CheckBox chkUpperCase;
        private System.Windows.Forms.Label labelUpperCase;
        private System.Windows.Forms.Label labelCfgDataStep;
        private System.Windows.Forms.Label labelReplStep;
        private System.Windows.Forms.GroupBox gbCfgData;
        private System.Windows.Forms.DateTimePicker dtRevTimeBase;
        private System.Windows.Forms.Label labelRevTimeBase;
        private System.Windows.Forms.ComboBox cbRevTimeType;
        private System.Windows.Forms.Label labelRevTimeType;
        private System.Windows.Forms.ComboBox cbTypeRev;
        private System.Windows.Forms.Label labelTypeRev;
        private System.Windows.Forms.Label labelInputNum;
        private System.Windows.Forms.TabPage tabPageFields;
        private System.Windows.Forms.TabPage tabPageFinal;
        private Components.DataGridViewExt dgvFields;
        private System.Windows.Forms.Label labelFields;
        private System.Windows.Forms.Button btnFinalCreateAction;
        private System.Windows.Forms.Button btnFinalCopy;
        private System.Windows.Forms.GroupBox gbFinalResult;
        private Controls.TextEditor editorFinalScript;
        private System.Windows.Forms.Label labelFinal;
        private System.Windows.Forms.GroupBox gbReplStep;
        private System.Windows.Forms.ComboBox cbReplSource;
        private System.Windows.Forms.ComboBox cbReplType;
        private System.Windows.Forms.TextBox tbReplPattern;
        private System.Windows.Forms.Label labelDone;
        private System.Windows.Forms.Label labelReplPostfix;
        private System.Windows.Forms.Label labelReplPrefix;
        private System.Windows.Forms.TextBox tbReplPostfix;
        private System.Windows.Forms.TextBox tbReplPrefix;
        private System.Windows.Forms.GroupBox gbReplacement;
        private System.Windows.Forms.GroupBox gbReplPattern;
        private System.Windows.Forms.TabControl tcRevNumber;
        private System.Windows.Forms.TabPage tpRevDeltaTime;
        private System.Windows.Forms.TabPage tpRevRaw;
        private Controls.FileTextBox ftbInputNum;
        private Controls.FileTextBox ftbOutputFile;
        private Controls.FileTextBox ftbReplFile;
        private System.Windows.Forms.Panel pFixBorder;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvFieldsEnabled;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvFieldsNameOrigin;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvFieldsNameNew;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvFieldsDescription;
        private System.Windows.Forms.TabControl tcReplType;
        private System.Windows.Forms.TabPage tpReplRegex;
        private System.Windows.Forms.Label labelHelpRegexLangCaption;
        private System.Windows.Forms.LinkLabel linkRegex;
        private System.Windows.Forms.TabPage tpReplWildcards;
        private System.Windows.Forms.TextBox tbHelpWildcards;
        private System.Windows.Forms.Label labelExampleSource;
        private System.Windows.Forms.Label labelSampleSource;
        private System.Windows.Forms.NumericUpDown numRevTimeMax;
        private System.Windows.Forms.NumericUpDown numRevTimeMin;
        private System.Windows.Forms.CheckBox chkRevTimeMod;
    }
}