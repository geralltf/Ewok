using System;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.Machine.Common.ProcessingUnit {
    public class MemoryDebugger {

        public void SetRAM(ref RAM ram) {
            ram.MemoryLocationUpdatedEventHandler+=new RAM.MemoryLocationUpdatedEventDelegate(ram_RAMUpdatedEventHandler);
        }

        private void ram_RAMUpdatedEventHandler(int address) {
            if(RAMUpdatedEventHandler!=null) {
                RAMUpdatedEventHandler(address);
            }
        }

        public event RAM.MemoryLocationUpdatedEventDelegate RAMUpdatedEventHandler;
    }
}