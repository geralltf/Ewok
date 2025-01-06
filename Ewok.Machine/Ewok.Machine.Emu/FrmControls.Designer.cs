namespace Ewok.Machine.Emu {
    partial class FrmControls {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components=null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing&&(components!=null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.grpSimulator = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.adjCPUFreq = new System.Windows.Forms.TrackBar();
            this.chkViewAddressingModes = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkViewLineNums = new System.Windows.Forms.CheckBox();
            this.chkViewDetails = new System.Windows.Forms.CheckBox();
            this.chkViewAssembly = new System.Windows.Forms.CheckBox();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnBreakpoints = new System.Windows.Forms.Button();
            this.grpSimulator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adjCPUFreq)).BeginInit();
            this.SuspendLayout();
            // 
            // grpSimulator
            // 
            this.grpSimulator.Controls.Add(this.label2);
            this.grpSimulator.Controls.Add(this.adjCPUFreq);
            this.grpSimulator.Controls.Add(this.chkViewAddressingModes);
            this.grpSimulator.Controls.Add(this.label1);
            this.grpSimulator.Controls.Add(this.chkViewLineNums);
            this.grpSimulator.Controls.Add(this.chkViewDetails);
            this.grpSimulator.Controls.Add(this.chkViewAssembly);
            this.grpSimulator.Controls.Add(this.btnPause);
            this.grpSimulator.Controls.Add(this.btnNext);
            this.grpSimulator.Controls.Add(this.btnContinue);
            this.grpSimulator.Controls.Add(this.btnReset);
            this.grpSimulator.Controls.Add(this.btnBreakpoints);
            this.grpSimulator.Location = new System.Drawing.Point(12, 12);
            this.grpSimulator.Name = "grpSimulator";
            this.grpSimulator.Size = new System.Drawing.Size(356, 163);
            this.grpSimulator.TabIndex = 17;
            this.grpSimulator.TabStop = false;
            this.grpSimulator.Text = "Simulator Interaction";
            this.grpSimulator.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "CPU Freq";
            // 
            // adjCPUFreq
            // 
            this.adjCPUFreq.Location = new System.Drawing.Point(230, 48);
            this.adjCPUFreq.Maximum = 100;
            this.adjCPUFreq.Name = "adjCPUFreq";
            this.adjCPUFreq.Size = new System.Drawing.Size(104, 45);
            this.adjCPUFreq.TabIndex = 29;
            this.adjCPUFreq.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.adjCPUFreq.Value = 50;
            this.adjCPUFreq.Scroll += new System.EventHandler(this.adjCPUFreq_Scroll);
            // 
            // chkViewAddressingModes
            // 
            this.chkViewAddressingModes.AutoSize = true;
            this.chkViewAddressingModes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkViewAddressingModes.Location = new System.Drawing.Point(136, 127);
            this.chkViewAddressingModes.Name = "chkViewAddressingModes";
            this.chkViewAddressingModes.Size = new System.Drawing.Size(139, 17);
            this.chkViewAddressingModes.TabIndex = 28;
            this.chkViewAddressingModes.Text = "View Addressing Modes";
            this.chkViewAddressingModes.UseVisualStyleBackColor = true;
            this.chkViewAddressingModes.CheckedChanged += new System.EventHandler(this.chkViewAddressingModes_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 27;
            this.label1.Text = "Options:";
            // 
            // chkViewLineNums
            // 
            this.chkViewLineNums.AutoSize = true;
            this.chkViewLineNums.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkViewLineNums.Location = new System.Drawing.Point(13, 127);
            this.chkViewLineNums.Name = "chkViewLineNums";
            this.chkViewLineNums.Size = new System.Drawing.Size(117, 17);
            this.chkViewLineNums.TabIndex = 26;
            this.chkViewLineNums.Text = "View Line Numbers";
            this.chkViewLineNums.UseVisualStyleBackColor = true;
            this.chkViewLineNums.CheckedChanged += new System.EventHandler(this.chkViewLineNums_CheckedChanged);
            // 
            // chkViewDetails
            // 
            this.chkViewDetails.AutoSize = true;
            this.chkViewDetails.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkViewDetails.Location = new System.Drawing.Point(136, 104);
            this.chkViewDetails.Name = "chkViewDetails";
            this.chkViewDetails.Size = new System.Drawing.Size(84, 17);
            this.chkViewDetails.TabIndex = 25;
            this.chkViewDetails.Text = "View Details";
            this.chkViewDetails.UseVisualStyleBackColor = true;
            this.chkViewDetails.CheckedChanged += new System.EventHandler(this.chkViewDetails_CheckedChanged);
            // 
            // chkViewAssembly
            // 
            this.chkViewAssembly.AutoSize = true;
            this.chkViewAssembly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkViewAssembly.Location = new System.Drawing.Point(34, 104);
            this.chkViewAssembly.Name = "chkViewAssembly";
            this.chkViewAssembly.Size = new System.Drawing.Size(96, 17);
            this.chkViewAssembly.TabIndex = 24;
            this.chkViewAssembly.Text = "View Assembly";
            this.chkViewAssembly.UseVisualStyleBackColor = true;
            this.chkViewAssembly.CheckedChanged += new System.EventHandler(this.chkViewAssembly_CheckedChanged);
            // 
            // btnPause
            // 
            this.btnPause.Location = new System.Drawing.Point(7, 49);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(87, 23);
            this.btnPause.TabIndex = 13;
            this.btnPause.Text = "Pause";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(6, 19);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(88, 23);
            this.btnNext.TabIndex = 0;
            this.btnNext.Text = "Next operation";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnContinue
            // 
            this.btnContinue.Location = new System.Drawing.Point(100, 19);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(124, 23);
            this.btnContinue.TabIndex = 12;
            this.btnContinue.Text = "Continue(until break)";
            this.btnContinue.UseVisualStyleBackColor = true;
            this.btnContinue.Click += new System.EventHandler(this.btnContinue_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(100, 48);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(124, 23);
            this.btnReset.TabIndex = 4;
            this.btnReset.Text = "Reset Processor";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnBreakpoints
            // 
            this.btnBreakpoints.Location = new System.Drawing.Point(230, 19);
            this.btnBreakpoints.Name = "btnBreakpoints";
            this.btnBreakpoints.Size = new System.Drawing.Size(113, 23);
            this.btnBreakpoints.TabIndex = 3;
            this.btnBreakpoints.Text = "Clear all breakpoints";
            this.btnBreakpoints.UseVisualStyleBackColor = true;
            this.btnBreakpoints.Visible = false;
            this.btnBreakpoints.Click += new System.EventHandler(this.btnBreakpoints_Click);
            // 
            // FrmControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 178);
            this.ControlBox = false;
            this.Controls.Add(this.grpSimulator);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FrmControls";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Controls";
            this.grpSimulator.ResumeLayout(false);
            this.grpSimulator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.adjCPUFreq)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox grpSimulator;
        public System.Windows.Forms.CheckBox chkViewAddressingModes;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox chkViewLineNums;
        public System.Windows.Forms.CheckBox chkViewDetails;
        public System.Windows.Forms.CheckBox chkViewAssembly;
        public System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnContinue;
        public System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnBreakpoints;
        public System.Windows.Forms.TrackBar adjCPUFreq;
        private System.Windows.Forms.Label label2;
    }
}