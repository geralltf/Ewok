using System;

namespace Ewok.Machine.Common.ProcessingUnit {
    public class Register16:Register {
        public Register16() {
            Bits=new bool[BitLength];
        }

        public override int BitLength {
            get {
                return 16;
            }
        }

        public short ToShort() {
            return ALU.convertBitsToInt16(Bits);
        }
    }
}
