using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ewok.Machine.Common;

namespace Ewok.Machine.Emu {
    public partial class FrmControls:Form {
        public FrmControls(Ref<FrmEditor> editorReference) {
            InitializeComponent();
            this.editorReference=editorReference;
            this.btnPause.Enabled=false;
            this.btnReset.Enabled=false;
        }

        public bool isPaused;
        private Ref<FrmEditor> editorReference;
        public FrmEditor Editor {
            get {
                return editorReference.Value;
            }
            set {
                editorReference.Value=value;
            }
        }

        private void chkViewAssembly_CheckedChanged(object sender,EventArgs e) {
            this.Editor.toggleLexView(true);
        }

        private void chkViewDetails_CheckedChanged(object sender,EventArgs e) {
            this.Editor.toggleLexView(true);
        }

        private void chkViewLineNums_CheckedChanged(object sender,EventArgs e) {
            this.Editor.toggleLexView(true);
        }

        private void chkViewAddressingModes_CheckedChanged(object sender,EventArgs e) {
            this.Editor.toggleLexView(true);
        }

        private void btnReset_Click(object sender,EventArgs e) {
            this.btnContinue.Enabled=true;
            this.btnReset.Enabled=false;
            this.btnPause.Enabled=false;
            Editor.interpreter.reset();
            Editor.ResetPC();
        }

        private void btnPause_Click(object sender,EventArgs e) {
            if(isPaused) {
                this.btnPause.Text="Pause";
                Editor.interpreterReference.Value.resume();
            }
            else {
                this.btnPause.Text="Resume";
                Editor.interpreterReference.Value.pause();
            }
            isPaused=!isPaused;
        }

        private void btnBreakpoints_Click(object sender,EventArgs e) {
            Editor.debugger.BreakpointAddresses.Clear();
        }

        private void btnContinue_Click(object sender,EventArgs e) {
            this.btnContinue.Enabled=false;
            this.btnPause.Enabled=true;
            this.btnReset.Enabled=true;
            this.btnPause.Text="Pause";
            this.isPaused=false;
            Editor.interpreterReference.Value.continueUntilBreakpoint();
        }

        private void btnNext_Click(object sender,EventArgs e) {
            this.btnContinue.Enabled=true;
            this.btnPause.Enabled=false;
            this.btnReset.Enabled=true;
            Editor.interpreter.next();
        }

        private void adjCPUFreq_Scroll(object sender,EventArgs e) {
            Editor.interpreterReference.Value.setFrequency(this.adjCPUFreq.Value);
        }
    }
}
