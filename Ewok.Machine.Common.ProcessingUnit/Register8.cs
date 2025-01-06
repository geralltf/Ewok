using System;

namespace Ewok.Machine.Common.ProcessingUnit {
    public class Register8:Register {
        public Register8() {
            Bits=new bool[BitLength];
        }

        public override int BitLength {
            get {
                return 8;
            }
        }

        public byte ToByte() {
            return ALU.convertBitsToByte(Bits);
        }

        public byte ToInt8() {
            return ToByte();
        }
    }
}
