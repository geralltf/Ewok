using System;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.M68HC11.Registers {
    public class ProgramCounter:Register {

        public int Address {
            get {
                return ALU.convertBitsToInt(Bits);
            }
            set {
                Bits=ALU.convertAnythingToBits(value);

                if(programCounterUpdatedEventHandler!=null) {
                    programCounterUpdatedEventHandler(Bits); // Send notification of change of Program Counter to user interface
                }
            }
        }

        public ProgramCounter() {
            Bits=new bool[BitLength];
        }

        public override int BitLength {
            get {
                return 16; // Hack the PC should just be an offset not an absolute address
            }
        }

        public void setEntryPoint(int entryPointAddress) {
            Bits=ALU.convertAnythingToBits(entryPointAddress);
        }

        ///// <summary>
        ///// Returns the current state of the PC
        ///// </summary>
        //public override string ToString() {
        //    return "Addr => "+ALU.convertAnythingToHex(Address);
        //}

        public event programCounterUpdatedEventDelegate programCounterUpdatedEventHandler;
        public delegate void programCounterUpdatedEventDelegate(bool[] currentAddressBits);
    }
}
