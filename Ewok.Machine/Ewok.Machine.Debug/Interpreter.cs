using System;
using System.Collections.ObjectModel;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;
using System.Collections.Generic;

namespace Ewok.Machine.Debug {

    public class Interpreter {

        public Interpreter(Ref<ProcessingUnit> processingUnitReference, Ref<Debugger> debuggerReference) {
            this.processingUnitReference=processingUnitReference;
            this.debuggerReference=debuggerReference;

            ProcessingUnit.programCounterUpdatedEventHandler+=new ProcessingUnit.programCounterUpdatedEventDelegate(ProcessingUnit_programCounterUpdatedEventHandler);
        }

        private Ref<Debugger> debuggerReference;
        public Debugger debugger {
            get {
                return debuggerReference.Value;
            }
            set {
                debuggerReference.Value=value;
            }
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

        public void Execute(Register startAddress,Dictionary<int,CommonOperation> operations) {
            // Initilise the CPU
            //Int16[] breakpointAddresses=new Int16[]{0x02,0x03};
            ProcessingUnit.init(startAddress,debugger.BreakpointAddresses);

            // Execute the instructions with the processing unit:
            ProcessingUnit.execute(operations,true,true);
        }

        public void ExecuteNoWait(Register startAddress,Dictionary<int,CommonOperation> operations) {
            // Initilise the CPU
            //Int16[] breakpointAddresses=new Int16[]{0x02,0x03};
            ProcessingUnit.init(startAddress,debugger.BreakpointAddresses);

            // Execute the instructions with the processing unit:
            ProcessingUnit.execute(operations,false,false);
        }

        public void WaitUntilFinished() {
            ProcessingUnit.clock.Join();
            //ProcessingUnit.processor.Wait();
        }

        public void reset() {
            ProcessingUnit.execute();
        }

        public void next() {
            ProcessingUnit.next();
        }

        public void continueUntilBreakpoint() {
            ProcessingUnit.continueUntilBreakpoint();
        }

        public void halt() {
            ProcessingUnit.Halt();
        }

        public void pause() {
            ProcessingUnit.Pause();
        }

        public void resume() {
            ProcessingUnit.Resume();
        }

        /// <summary>
        /// Adjusts
        /// </summary>
        /// <param name="freq">
        /// Setting to 0 makes interpreter run without an imposed time delay.
        /// Otherwise value must be between 1 and 100
        /// </param>
        public void setFrequency(int freq) {
            ProcessingUnit.setFrequency(freq);
        }

        private void ProcessingUnit_programCounterUpdatedEventHandler(bool[] currentAddressBits) {
            if(programCounterUpdatedEventHandler!=null) {
                programCounterUpdatedEventHandler();
            }
        }

        public event programCounterUpdatedEventDelegate programCounterUpdatedEventHandler;
        public delegate void programCounterUpdatedEventDelegate();


    }
}
