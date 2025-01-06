using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.Machine.Debug {
    public partial class FrmMemoryViewer:Form {
        public FrmMemoryViewer(Ref<ProcessingUnit> processingUnitReference) {
            InitializeComponent();

            FrmWatch=new FrmMemoryWatchDebugger(processingUnitReference);
            FrmWatch.TopLevel=false;
            FrmWatch.Visible=true;
            FrmWatch.Dock=DockStyle.Fill;
            FrmWatch.Show();
            tabPageWatch.Controls.Add(FrmWatch);

            FrmScanner=new FrmMemoryScannerDebugger(processingUnitReference);
            FrmScanner.TopLevel=false;
            FrmScanner.Visible=true;
            FrmScanner.Dock=DockStyle.Fill;
            FrmScanner.Show();
            tabPageScanner.Controls.Add(FrmScanner);
        }

        public FrmMemoryWatchDebugger FrmWatch;
        public FrmMemoryScannerDebugger FrmScanner;

        private void tabPageMap_Click(object sender,EventArgs e) {
            
        }

        private void tabPageScanner_Click(object sender,EventArgs e) {

        }

        private void tabPageWatch_Click(object sender,EventArgs e) {
            
        }

    }
}
