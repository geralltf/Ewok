namespace Ewok.Machine.Debug {
    partial class FrmMemoryViewer {
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageWatch = new System.Windows.Forms.TabPage();
            this.tabPageScanner = new System.Windows.Forms.TabPage();
            this.tabPageMap = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageWatch);
            this.tabControl1.Controls.Add(this.tabPageScanner);
            this.tabControl1.Controls.Add(this.tabPageMap);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(664, 452);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageWatch
            // 
            this.tabPageWatch.Location = new System.Drawing.Point(4, 22);
            this.tabPageWatch.Name = "tabPageWatch";
            this.tabPageWatch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWatch.Size = new System.Drawing.Size(656, 426);
            this.tabPageWatch.TabIndex = 0;
            this.tabPageWatch.Text = "Watch";
            this.tabPageWatch.UseVisualStyleBackColor = true;
            this.tabPageWatch.Click += new System.EventHandler(this.tabPageWatch_Click);
            // 
            // tabPageScanner
            // 
            this.tabPageScanner.Location = new System.Drawing.Point(4, 22);
            this.tabPageScanner.Name = "tabPageScanner";
            this.tabPageScanner.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageScanner.Size = new System.Drawing.Size(656, 426);
            this.tabPageScanner.TabIndex = 1;
            this.tabPageScanner.Text = "Scanner";
            this.tabPageScanner.UseVisualStyleBackColor = true;
            this.tabPageScanner.Click += new System.EventHandler(this.tabPageScanner_Click);
            // 
            // tabPageMap
            // 
            this.tabPageMap.Location = new System.Drawing.Point(4, 22);
            this.tabPageMap.Name = "tabPageMap";
            this.tabPageMap.Size = new System.Drawing.Size(656, 426);
            this.tabPageMap.TabIndex = 2;
            this.tabPageMap.Text = "Map";
            this.tabPageMap.UseVisualStyleBackColor = true;
            this.tabPageMap.Click += new System.EventHandler(this.tabPageMap_Click);
            // 
            // FrmMemoryViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(664, 452);
            this.ControlBox = false;
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FrmMemoryViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Memory Viewer";
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageWatch;
        private System.Windows.Forms.TabPage tabPageScanner;
        private System.Windows.Forms.TabPage tabPageMap;

    }
}