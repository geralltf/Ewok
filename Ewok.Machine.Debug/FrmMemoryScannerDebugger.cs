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
    public partial class FrmMemoryScannerDebugger:Form {
        public FrmMemoryScannerDebugger(Ref<ProcessingUnit> processingUnitReference) {
            InitializeComponent();

            MemoryDebugger=new MemoryDebugger();
            MemoryDebugger.RAMUpdatedEventHandler+=new RAM.MemoryLocationUpdatedEventDelegate(MemoryWatchDebugger_RAMUpdatedEventHandler);
            processingUnitReference.Value.SetMemoryDisp(ref this.MemoryDebugger);
            this.processingUnitReference=processingUnitReference;
        }

        public MemoryDebugger MemoryDebugger;
        private Ref<ProcessingUnit> processingUnitReference;

        private void MemoryWatchDebugger_RAMUpdatedEventHandler(int address) {

        }
    }
}
