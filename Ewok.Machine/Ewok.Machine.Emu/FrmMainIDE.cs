using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;
using Ewok.Machine.Debug;

namespace Ewok.Machine.Emu {
    public partial class FrmMainIDE:Form {
        public FrmMainIDE() {
            InitializeComponent();
            loadIDE();
        }

        public FrmProcessingUnit frmProcessingUnit;
        public FrmControls frmControls;
        public FrmEditor frmEditor;
        public FrmOutputMessages frmOutputMessages;
        public int panel2Height;
        private Ref<FrmEditor> editorReference;
        public FrmMemoryViewer frmMemoryWatchDebugger;

        private void loadIDE() {
            frmOutputMessages=new FrmOutputMessages();
            frmOutputMessages.TopLevel=false;
            frmOutputMessages.Dock=DockStyle.Fill;
            this.containerHoriz.Panel2.Controls.Add(frmOutputMessages);
            frmOutputMessages.Show();
            frmOutputMessages.Visible=false;
            panel2Height=720;// this.containerHoriz.SplitterDistance;
            this.containerHoriz.SplitterDistance=this.containerHoriz.Height;

            frmEditor=new FrmEditor();
            frmEditor.TopLevel=false;
            frmEditor.setIDE(this);
            frmEditor.Dock=DockStyle.Fill;
            this.containerVert.Panel1.Controls.Add(frmEditor);
            frmEditor.Show();

            editorReference=new Ref<FrmEditor>(() => frmEditor,z => { frmEditor=z; });

            frmProcessingUnit=new FrmProcessingUnit(frmEditor.interpreterReference);
            frmProcessingUnit.TopLevel=false;
            frmProcessingUnit.Dock=DockStyle.Fill;
            this.containerVertControls.Panel2.Controls.Add(frmProcessingUnit);
            frmProcessingUnit.Show();

            frmControls=new FrmControls(this.editorReference);
            frmControls.TopLevel=false;
            frmControls.Dock=DockStyle.Fill;
            this.containerVertControls.Panel1.Controls.Add(frmControls);
            frmControls.Show();
        }

        public string Output {
            set {
                this.frmOutputMessages.Output=value;
                this.frmOutputMessages.Show();
            }
        }

        public Dictionary<string,object> RegisterProperies {
            set {
                this.frmProcessingUnit.RegisterProperies=value;
            }
            get {
                return this.frmProcessingUnit.RegisterProperies;
            }
        }

        public int GetCallStackSize() {
            return this.frmProcessingUnit.GetCallStackSize();
        }

        public void ClearStack() {
            this.frmProcessingUnit.ClearStack();
        }

        //public Collection<Register> CallStack {
        //    //get {
        //        //return this.frmProcessingUnit.CallStack;
        //    //}
        //    set {
        //        this.frmProcessingUnit.CallStack=value;
        //    }
        //}

        public void Push(Register reg) {
            this.frmProcessingUnit.Push(reg);
        }

        public void Pop() {
            this.frmProcessingUnit.Pop();
        }

        public OutputModeValue OutputMode {
            set {
                this.frmOutputMessages.OutputMode=value;
            }
        }

        public enum OutputModeValue {
            UNDEFINED=0,
            Errors,
            ErrorsWithWarnings,
            Normal,
            Warnings
        }

        public void ClearProcessingUnitStatus() {
            this.frmProcessingUnit.ClearUI();
        }

        private void FrmMainIDE_FormClosing(object sender,System.Windows.Forms.FormClosingEventArgs e) {
            //this.frmEditor.interpreterReference.Value.halt();
            System.Environment.Exit(0);
        }

        private void watchToolStripMenuItem_Click(object sender,EventArgs e) {
            if(this.frmMemoryWatchDebugger==null) {
                this.frmMemoryWatchDebugger=new FrmMemoryViewer(this.frmEditor.PUReference);
            }
            this.frmMemoryWatchDebugger.Show();
        }

    }
}
