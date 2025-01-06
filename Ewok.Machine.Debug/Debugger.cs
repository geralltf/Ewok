using System;
using System.Collections.ObjectModel;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.Machine.Debug {
    public class Debugger {

        public Debugger(Ref<ProcessingUnit> processingUnitReference) {
            BreakpointAddresses=new Collection<int>();
            this.processingUnitReference=processingUnitReference;

            ProcessingUnit.breakpointTriggeredEventHandler+=new ProcessingUnit.breakpointTriggeredDelegate(ProcessingUnit_breakpointTriggeredEventHandler);
        }

        private Ref<ProcessingUnit> processingUnitReference;
        public ProcessingUnit ProcessingUnit {
            get {
                return processingUnitReference.Value;
            }
            set {
                processingUnitReference.Value=value;
            }
        }

        public Collection<int> BreakpointAddresses;

        public void setBreakpoint(int index) {
            if(!BreakpointAddresses.Contains((Int16)index)) {
                BreakpointAddresses.Add((Int16)index);
            }
        }

        private void ProcessingUnit_breakpointTriggeredEventHandler(int breakpointAddress) {
            if(breakpointTriggeredEventHandler!=null) {
                breakpointTriggeredEventHandler(breakpointAddress);
            }
        }

        public void RuntimeExceptionCaller(int lineOfCode,Exception ex) {
            if(RuntimeExceptionEventHandler!=null){
                RuntimeExceptionEventHandler(lineOfCode,ex);
            }
            else{
                throw ex;
            }
        }

        public event breakpointTriggeredDelegate breakpointTriggeredEventHandler;
        public delegate void breakpointTriggeredDelegate(int breakpointAddress);

        public event RuntimeExceptionEventDelegate RuntimeExceptionEventHandler;
        public delegate void RuntimeExceptionEventDelegate(int lineOfCode,Exception ex);
    }
}