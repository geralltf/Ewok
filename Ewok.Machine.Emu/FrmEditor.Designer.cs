namespace Ewok.Machine.Emu {
    partial class FrmEditor {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEditor));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.txtASM = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtOperationEditor = new System.Windows.Forms.TextBox();
            this.gvLexed = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnExecuteHalt = new System.Windows.Forms.Button();
            this.btnRamMemMap = new System.Windows.Forms.Button();
            this.btnROMMemMap = new System.Windows.Forms.Button();
            this.contextmnuLexedOperation = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editOperationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvLexed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.contextmnuLexedOperation.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtASM
            // 
            this.txtASM.BackColor = System.Drawing.Color.Navy;
            this.txtASM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtASM.ForeColor = System.Drawing.Color.White;
            this.txtASM.Location = new System.Drawing.Point(0, 0);
            this.txtASM.Multiline = true;
            this.txtASM.Name = "txtASM";
            this.txtASM.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtASM.Size = new System.Drawing.Size(747, 608);
            this.txtASM.TabIndex = 24;
            this.txtASM.Text = resources.GetString("txtASM.Text");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtOperationEditor);
            this.panel1.Controls.Add(this.txtASM);
            this.panel1.Controls.Add(this.gvLexed);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(747, 608);
            this.panel1.TabIndex = 26;
            // 
            // txtOperationEditor
            // 
            this.txtOperationEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.txtOperationEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtOperationEditor.ForeColor = System.Drawing.Color.White;
            this.txtOperationEditor.Location = new System.Drawing.Point(131, 18);
            this.txtOperationEditor.Name = "txtOperationEditor";
            this.txtOperationEditor.Size = new System.Drawing.Size(206, 13);
            this.txtOperationEditor.TabIndex = 32;
            this.txtOperationEditor.Visible = false;
            this.txtOperationEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOperationEditor_KeyDown);
            this.txtOperationEditor.LostFocus += new System.EventHandler(this.txtOperationEditor_LostFocus);
            // 
            // gvLexed
            // 
            this.gvLexed.AllowUserToAddRows = false;
            this.gvLexed.AllowUserToDeleteRows = false;
            this.gvLexed.AllowUserToResizeColumns = false;
            this.gvLexed.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.gvLexed.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gvLexed.BackgroundColor = System.Drawing.Color.Navy;
            this.gvLexed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gvLexed.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.gvLexed.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvLexed.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvLexed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvLexed.ColumnHeadersVisible = false;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvLexed.DefaultCellStyle = dataGridViewCellStyle2;
            this.gvLexed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gvLexed.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gvLexed.GridColor = System.Drawing.Color.White;
            this.gvLexed.Location = new System.Drawing.Point(0, 0);
            this.gvLexed.MultiSelect = false;
            this.gvLexed.Name = "gvLexed";
            this.gvLexed.ReadOnly = true;
            this.gvLexed.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvLexed.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.gvLexed.RowHeadersVisible = false;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Aqua;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Black;
            this.gvLexed.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.gvLexed.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvLexed.ShowCellErrors = false;
            this.gvLexed.ShowCellToolTips = false;
            this.gvLexed.ShowEditingIcon = false;
            this.gvLexed.ShowRowErrors = false;
            this.gvLexed.Size = new System.Drawing.Size(747, 608);
            this.gvLexed.TabIndex = 25;
            this.gvLexed.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gvLexed_CellMouseClick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.btnExecuteHalt);
            this.splitContainer1.Panel1.Controls.Add(this.btnRamMemMap);
            this.splitContainer1.Panel1.Controls.Add(this.btnROMMemMap);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(747, 692);
            this.splitContainer1.SplitterDistance = 80;
            this.splitContainer1.TabIndex = 27;
            // 
            // btnExecuteHalt
            // 
            this.btnExecuteHalt.Location = new System.Drawing.Point(275, 12);
            this.btnExecuteHalt.Name = "btnExecuteHalt";
            this.btnExecuteHalt.Size = new System.Drawing.Size(74, 23);
            this.btnExecuteHalt.TabIndex = 28;
            this.btnExecuteHalt.Text = "Execute";
            this.btnExecuteHalt.UseVisualStyleBackColor = true;
            this.btnExecuteHalt.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // btnRamMemMap
            // 
            this.btnRamMemMap.Enabled = false;
            this.btnRamMemMap.Location = new System.Drawing.Point(12, 12);
            this.btnRamMemMap.Name = "btnRamMemMap";
            this.btnRamMemMap.Size = new System.Drawing.Size(113, 23);
            this.btnRamMemMap.TabIndex = 30;
            this.btnRamMemMap.Text = "RAM Memory Map";
            this.btnRamMemMap.UseVisualStyleBackColor = true;
            // 
            // btnROMMemMap
            // 
            this.btnROMMemMap.Enabled = false;
            this.btnROMMemMap.Location = new System.Drawing.Point(131, 12);
            this.btnROMMemMap.Name = "btnROMMemMap";
            this.btnROMMemMap.Size = new System.Drawing.Size(113, 23);
            this.btnROMMemMap.TabIndex = 31;
            this.btnROMMemMap.Text = "ROM Memory Map";
            this.btnROMMemMap.UseVisualStyleBackColor = true;
            // 
            // contextmnuLexedOperation
            // 
            this.contextmnuLexedOperation.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editOperationToolStripMenuItem});
            this.contextmnuLexedOperation.Name = "contextmnuLexedOperation";
            this.contextmnuLexedOperation.Size = new System.Drawing.Size(149, 26);
            this.contextmnuLexedOperation.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.contextmnuLexedOperation_ItemClicked);
            // 
            // editOperationToolStripMenuItem
            // 
            this.editOperationToolStripMenuItem.Name = "editOperationToolStripMenuItem";
            this.editOperationToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.editOperationToolStripMenuItem.Text = "Edit operation";
            // 
            // FrmEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(747, 692);
            this.ControlBox = false;
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "FrmEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Debugger";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvLexed)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.contextmnuLexedOperation.ResumeLayout(false);
            this.ResumeLayout(false);

        }



        #endregion

        private System.Windows.Forms.TextBox txtASM;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btnExecuteHalt;
        private System.Windows.Forms.Button btnRamMemMap;
        private System.Windows.Forms.Button btnROMMemMap;
        private System.Windows.Forms.DataGridView gvLexed;
        private System.Windows.Forms.ContextMenuStrip contextmnuLexedOperation;
        private System.Windows.Forms.ToolStripMenuItem editOperationToolStripMenuItem;
        private System.Windows.Forms.TextBox txtOperationEditor;

    }
}

