using System;

namespace Ewok.Machine.Common.ProcessingUnit {
    public class RAM {

        /// <param name="size">
        /// Mesured in B(bytes)
        /// </param>
        public RAM(int size) {
            mem=new byte[size];
            this.size=size;
        }

        public byte[] mem;
        private int size;

        /// <summary>
        /// Notifies a debugger interface when a memory location is modified
        /// </summary>
        private void ifnotify(int address) {
            if(this.MemoryLocationUpdatedEventHandler!=null) {
                this.MemoryLocationUpdatedEventHandler(address);
            }
        }

        public int getSize() {
            return size;
        }

        public void set(short address,byte data) {
            set((int)address,data);
        }

        public void set(int address,byte data) { // Sometimes addresses are really long
            mem[address]=data;
            ifnotify(address);
        }

        public void set(short address,Int16 data) {
            set((int)address,data);
        }

        public void set(int address,Int16 data){ // Sometimes addresses are really long
            bool[] bHighNibble;
            bool[] bLowNibble;
            bool[] bits;
            int bitLength;

            bitLength=16; // 16 bit data
            bits=ALU.convertAnythingToBits(data,bitLength);
            bHighNibble=ALU.getHighNibble(bits,bitLength);
            bLowNibble=ALU.getLowNibble(bits,bitLength);

            mem[address]=ALU.convertBitsToByte(bHighNibble);
            mem[address+1]=ALU.convertBitsToByte(bLowNibble);

            ifnotify(address);
        }

        public byte get(short address) {
            return mem[address];
        }

        public byte get(int address) {
            return mem[address];
        }

        /// <summary>
        /// The high nibble is located at: address
        /// The low nibble is located at: address+1
        /// </summary>
        public Int16 get16(short address) {
            return get16((int)address);
        }

        /// <summary>
        /// The high nibble is located at: address
        /// The low nibble is located at: address+1
        /// </summary>
        public Int16 get16(int address) {
            byte highNibble=this.get(address);
            byte lowNibble=this.get(address+1);
            string hJoin=ALU.convertAnythingToHex(highNibble)+ALU.convertAnythingToHex(lowNibble);// Hacky but this should work!
            int vint32=ALU.convertHexToInt(hJoin);
            return (Int16)vint32;
        }

        public delegate void MemoryLocationUpdatedEventDelegate(int address);
        public event MemoryLocationUpdatedEventDelegate MemoryLocationUpdatedEventHandler;
    }
}