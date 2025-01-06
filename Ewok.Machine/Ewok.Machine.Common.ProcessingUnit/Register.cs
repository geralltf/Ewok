using System;

using Ewok.Machine.Common;

namespace Ewok.Machine.Common.ProcessingUnit {
    public abstract class Register {

        public abstract int BitLength {
            get;
        }

	    private bool[] bits;

        public virtual bool[] Bits {
            get {
                return bits;
            }
            set {
                bits=value;
            }
        }

        public bool[] HighNibble {
            get {
                return ALU.getHighNibble(Bits,BitLength);
            }
            set {
                Bits=ALU.setHighNibble(Bits,value,BitLength);
            }
        }

        public bool[] LowNibble {
            get {
                return ALU.getLowNibble(Bits,BitLength);
            }
            set {
                Bits=ALU.setLowNibble(Bits,value,BitLength);
            }
        }

        /// <summary>
        /// Clears the Register bits
        /// </summary>
        public void Clear() {
            bits=new bool[BitLength];
        }

	    private void setByte(byte byteValue){
            Bits=ALU.convertAnythingToBits(byteValue,BitLength);
	    }
        public void set16BitValue(Int16 byteValue) {
            Bits=ALU.convertAnythingToBits(byteValue);
        }
        public void set8BitValue(byte byteValue) {
            setByte(byteValue);
        }
        public void setBit(int index,bool bit) {
		    Bits[index]=bit;
	    }

        public bool getBit(int index) {
            return bits[index];
        }
	    public int getBitLength(){
            return BitLength;
	    }
        public byte getByte() {
            return ALU.convertBitsToByte(bits);
        }
        public Int16 getInt16() {
            return ALU.convertBitsToInt16(bits);
        }

        public int getInt() {
            return ALU.convertBitsToInt(bits);
        }

        /// <summary>
        /// Loads a byte value into a Register
        /// </summary>
        public static void load(Register R,byte byteValue) {
            R.setByte(byteValue);
        }

        public override string ToString() {
            return " ("+BitLength+" bit) => ["+ALU.convertBitsToString(bits)+"] [$"+ALU.convertAnythingToHex(bits)+"] ["+ALU.convertBitsToInt(bits).ToString()+"]";
        }

        public string ToHex() {
            return ALU.convertAnythingToHex(bits);
        }

        public int ToInteger() {
            return ALU.convertAnythingToInt(bits);
        }

        public byte ToByte() {
            return ALU.convertBitsToByte(bits);
        }

        public Int16 ToInt16() {
            return ALU.convertBitsToInt16(bits);
        }

        public string ToBinaryString() {
            return ALU.convertBitsToString(Bits);
        }
    }
}
