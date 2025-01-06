using System;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.M68HC11.Registers {
    public class StackPointer:Register16 {

        public Int16 Address {
            get {
                return ALU.convertBitsToInt16(Bits);
            }
            set {
                Bits=ALU.convertAnythingToBits(value);
            }
        }

        //public override string ToString() {
        //    return "Address => "+Address.ToString();
        //}
    }
}
