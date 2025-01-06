namespace Ewok.Machine.Emu {
    partial class FrmMainIDE {
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
            this.containerHoriz = new System.Windows.Forms.SplitContainer();
            this.containerVert = new System.Windows.Forms.SplitContainer();
            this.containerVertControls = new System.Windows.Forms.SplitContainer();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.memoryViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.containerHoriz)).BeginInit();
            this.containerHoriz.Panel1.SuspendLayout();
            this.containerHoriz.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.containerVert)).BeginInit();
            this.containerVert.Panel1.SuspendLayout();
            this.containerVert.Panel2.SuspendLayout();
            this.containerVert.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.containerVertControls)).BeginInit();
            this.containerVertControls.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // containerHoriz
            // 
            this.containerHoriz.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerHoriz.Location = new System.Drawing.Point(0, 0);
            this.containerHoriz.Name = "containerHoriz";
            this.containerHoriz.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // containerHoriz.Panel1
            // 
            this.containerHoriz.Panel1.Controls.Add(this.containerVert);
            this.containerHoriz.Size = new System.Drawing.Size(945, 1059);
            this.containerHoriz.SplitterDistance = 770;
            this.containerHoriz.TabIndex = 1;
            // 
            // containerVert
            // 
            this.containerVert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerVert.Location = new System.Drawing.Point(0, 0);
            this.containerVert.Name = "containerVert";
            // 
            // containerVert.Panel1
            // 
            this.containerVert.Panel1.Controls.Add(this.menuStrip1);
            // 
            // containerVert.Panel2
            // 
            this.containerVert.Panel2.Controls.Add(this.containerVertControls);
            this.containerVert.Size = new System.Drawing.Size(945, 770);
            this.containerVert.SplitterDistance = 601;
            this.containerVert.TabIndex = 0;
            // 
            // containerVertControls
            // 
            this.containerVertControls.Dock = System.Windows.Forms.DockStyle.Fill;
            this.containerVertControls.Location = new System.Drawing.Point(0, 0);
            this.containerVertControls.Name = "containerVertControls";
            this.containerVertControls.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.containerVertControls.Size = new System.Drawing.Size(340, 770);
            this.containerVertControls.SplitterDistance = 221;
            this.containerVertControls.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(601, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.memoryViewToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // memoryViewToolStripMenuItem
            // 
            this.memoryViewToolStripMenuItem.Name = "memoryViewToolStripMenuItem";
            this.memoryViewToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.memoryViewToolStripMenuItem.Text = "Memory Viewer";
            this.memoryViewToolStripMenuItem.Click += new System.EventHandler(this.watchToolStripMenuItem_Click);
            // 
            // FrmMainIDE
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 1059);
            this.Controls.Add(this.containerHoriz);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmMainIDE";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ewok.Machine.Emu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMainIDE_FormClosing);
            this.containerHoriz.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.containerHoriz)).EndInit();
            this.containerHoriz.ResumeLayout(false);
            this.containerVert.Panel1.ResumeLayout(false);
            this.containerVert.Panel1.PerformLayout();
            this.containerVert.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.containerVert)).EndInit();
            this.containerVert.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.containerVertControls)).EndInit();
            this.containerVertControls.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.SplitContainer containerHoriz;
        public System.Windows.Forms.SplitContainer containerVert;
        public System.Windows.Forms.SplitContainer containerVertControls;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem memoryViewToolStripMenuItem;
    }
}