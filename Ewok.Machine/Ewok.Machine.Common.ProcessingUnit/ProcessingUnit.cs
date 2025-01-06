using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;

namespace Ewok.Machine.Common.ProcessingUnit {
    public abstract class ProcessingUnit {
        public Collection<int> breakpointAddresses;
        private bool pauseExecution;
        public bool doSingleExecution;
        public bool enableBreakPoints;
        public bool endExecutionInterrupt;
        public bool isFinished;
        public Register startAddress;
        public Thread clock;
        private CancellationTokenSource taskTokenSource;
        private CancellationToken cancellationFlag;
        public Dictionary<int,CommonOperation> operations; // The list of current operations on the CPU
        public CommonOperation CurrentOperation;
        private bool delayProcessing;
        private int delayFrequency;
        private EventWaitHandle wh=new AutoResetEvent(false);

        public abstract RAM VirtualRAM {
            get;
        }

        public abstract void SetMemoryDisp(ref MemoryDebugger memoryDebugger);
        public abstract void ClearRegisterMemory();
        public abstract void setStartAddress(Register startAddress);
        public abstract void ClearStacks();
        public abstract Dictionary<string,object> GetRegisterStatus();
        public abstract Collection<Register> GetCallStack();
        public abstract Collection<Register> GetAccumulatorStack();
        public abstract int GetCurrentProgramCounterAddress();

        public Register GetStartAddress() {
            return this.startAddress;
        }

        public void init(Register startAddress,Collection<int> breakpointAddresses) {
            this.breakpointAddresses=breakpointAddresses;
            //int[] brk=new int[breakpointAddresses.Count];
            //for(int i=0;i<breakpointAddresses.Count;i++) {
            //    brk[i]=breakpointAddresses[i];
            //}

            //this.breakpointAddresses=brk;

            taskTokenSource=new CancellationTokenSource();
            cancellationFlag=taskTokenSource.Token;

            this.startAddress=startAddress;
            pauseExecution=true;
        }

        public void reset() {
            CurrentOperation=null;
            clock=new Thread(new ThreadStart(delegate() { cycle(); }));
        }

        public void execute() {
            Dictionary<int,CommonOperation> _operations=operations;
            bool _enableBreakPoints=enableBreakPoints;
            bool _doSingleExecution=doSingleExecution;

            setStartAddress(this.startAddress);
            reset();
            //pauseExecution=true;
            //doSingleExecution=true;
            execute(_operations,_enableBreakPoints,_doSingleExecution);
        }

        public void execute(Dictionary<int,CommonOperation> operations,bool enableBreakPoints,bool doSingleExecution) {
            this.operations=operations; // <- Stores the operations for processing
            this.enableBreakPoints=enableBreakPoints;
            this.doSingleExecution=doSingleExecution;

            reset();
            clock.Start();
        }


        /// <summary>
        /// Fetches an instruction that is next to execute
        /// </summary>
        /// <returns>
        /// If there are no more instructions left to execute,
        /// fetch process returns null.
        /// </returns>
        public CommonOperation fetch() {
            // Get the current address of the program counter:
            int addr=GetCurrentProgramCounterAddress();
            CommonOperation op;

            if(endExecutionInterrupt) { // If End operation issued, stop executing by fetching nothing
                return null;
            }

            if(operations.TryGetValue(addr,out op)) { // Much quicker to pull an op from a dictionary
                return op;
            }
            else {
                return null;
            }
        }

        // cycle() method must be in a separate thread: (breakExecutionCheck() and next() locks current thread)
        private void cycle() {
            CommonOperation op;

            // Reset if we need to
            endExecutionInterrupt=false;
            ClearStacks();
            ClearRegisterMemory();
            setStartAddress(this.startAddress);

            if(startedEventHandler!=null) {
                startedEventHandler();
            }            

            /* Start the fetch -> decode -> execute cycle */
            while(!cancellationFlag.IsCancellationRequested) {
                op=fetch(); // Fetch an instruction from memory

                if(op==null) {
                    // Stop executing fetched instructions if there are none left
                    taskTokenSource.Cancel();
                    break;
                }

                CurrentOperation=op;

                if(enableBreakPoints) {
                    breakExecutionCheck(op); // Checks if there is a breakpoint for this operation, and breaks execution accordingly
                }

                PauseExecution(); // Dont do any cycles until user calls CPU interaction functions
                if(doSingleExecution) {
                    
                }
                else {
                    if(delayProcessing&&delayFrequency>0) {
                        System.Threading.Thread.Sleep(delayFrequency*10);
                    }
                }

                op.Invoke(); // Execute the instruction (the instruction was already decoded)
                //PauseExecution(); // Dont do any cycles until user calls CPU interaction functions
            }
        }

        public void Pause() {
            pauseExecution=true;
            //doSingleExecution=true;
        }

        public void Resume() {
            if(pauseExecution) {
                pauseExecution=false;

                //try {
                    wh.Set();
                    //clock.Resume();
                //}
                //catch { }
                /*
                lock(locker) {
                    Monitor.PulseAll(this.locker);
                }*/
            }
        }

        private void PauseExecution() {
            if(pauseExecution) {
                /*lock(locker) {
                    Monitor.Wait(this.locker);
                }*/

                wh.WaitOne();
                //try {
                    //clock.Suspend();
                //}
                //catch { }

                //
                //while(pauseExecution) { }
            }
        }

        private void breakExecutionCheck(CommonOperation op) {
            int addr;
            for(int i=0;i<breakpointAddresses.Count;i++) {
                addr=breakpointAddresses[i];
                if(addr==op.InstructionAddress) { // If this operation has to pause execution
                    PauseExecution();

                    if(breakpointTriggeredEventHandler!=null) {
                        breakpointTriggeredEventHandler(addr);
                    }
                    break;
                }
            }
        }

        public void Halt() {
            doSingleExecution=true;
            pauseExecution=true;
            
            taskTokenSource.Cancel();
            clock=null;//processor=null;

            ClearStacks();
            ClearRegisterMemory();
        }

        #region CPU interaction functions
        /// <summary>
        /// Tells the CPU to proceed to the next operation, and then pause execution
        /// </summary>
        public void next() {
            Resume();
            enableBreakPoints=false;
            doSingleExecution=true;
            Pause();
        }

        /// <summary>
        /// Tells the CPU to continue executing operations until it reaches a known breakpoint
        /// </summary>
        /// <param name="breakpointAddresses">
        /// A list of known breakpoints to pause execution
        /// </param>
        public void continueUntilBreakpoint() {
            enableBreakPoints=true;
            doSingleExecution=false;
            Resume();
        }

        /// <summary>
        /// Tells the CPU to continue execution until the PC is at a specific address
        /// </summary>
        /// <param name="address"></param>
        public void continueUntilAddress(Int16 address) {
            // TODO Continue execution until PC at address
            throw new NotImplementedException();
        }

        public void setFrequency(int freq) {
            if(freq<1) {
                delayProcessing=false;
            }
            else {
                delayProcessing=true;
                delayFrequency=freq;
            }
        }

        #endregion CPU interaction functions

        public event finishedDelegate finishedEventHandler;
        public delegate void finishedDelegate();


        public event startedDelegate startedEventHandler;
        public delegate void startedDelegate();

        public event breakpointTriggeredDelegate breakpointTriggeredEventHandler;
        public delegate void breakpointTriggeredDelegate(int breakpointAddress);

        public abstract event programCounterUpdatedEventDelegate programCounterUpdatedEventHandler;
        public delegate void programCounterUpdatedEventDelegate(bool[] currentAddressBits);


    }
}
