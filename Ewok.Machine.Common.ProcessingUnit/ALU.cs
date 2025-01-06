using System;

namespace Ewok.Machine.Common.ProcessingUnit {

    /// <summary>
    /// Arithmetic Logic Unit - A library class that contains all the low level functionality that operations use
    /// </summary>
    public class ALU {

        /// <summary>
        /// Determines if the operand is equal to zero
        /// </summary>
        public static bool isZero(bool[] operand) {
            for(int i=0;i<operand.Length;i++) {
                if(operand[i]==true) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Determines if the Register data is equal to zero
        /// </summary>
        public static bool isZero(Register operand) {
            return isZero(operand.Bits);
        }

        /// <summary>
        /// Using 2's complement, bit 7 of an 8 bit value represents if the value is a negitive value
        /// 1=negitive, 0=positive
        /// </summary>
        public static bool isNegativeNumber(Register R) {
            return isNegativeNumber(R.Bits); // Bit 7 is the first element of the array
        }

        /// <summary>
        /// Using 2's complement, bit 7 of an 8 bit value represents if the value is a negitive value
        /// 1=negitive, 0=positive
        /// </summary>
        public static bool isNegativeNumber(bool[] bits) {
            return (bits[0]==true); // Bit 7 is the first element of the array
        }

        /// <summary>
        /// Using 2's complement, bit 7 of an 8 bit value represents if the value is a negitive value
        /// 1=negitive, 0=positive
        /// </summary>
        public static bool isNegativeNumber(short number) {
            bool[] bits=convertAnythingToBits(number);
            return (bits[7]==true);
        }

        public static Accumulator8 cloneAccumulatorRegister(Accumulator8 X) {
            Accumulator8 newRegister=new Accumulator8(X.getDReference(),X.getAccumulatorType());
            newRegister.Bits=X.Bits;
            return newRegister;
        }
        public static Accumulator16 cloneAccumulatorRegister(Accumulator16 X) {
            Accumulator16 newRegister=new Accumulator16();
            newRegister.Bits=X.Bits;
            return newRegister;
        }

        public static Register16 cloneRegister(Register16 X) {
            Register16 newRegister=new Register16();
            newRegister.Bits=X.Bits;
            return newRegister;
        }

        public static Register8 cloneRegister(Register8 X) {
            Register8 newRegister=new Register8();
            newRegister.Bits=X.Bits;
            return newRegister;
        }

        /// <summary>
        /// Adds a byte to an 8 bit Register
        /// </summary>
        public static Register8 add(Register8 X,byte b,out bool overflow,out bool negitive,out bool carry,out bool halfCarry) {
            bool[] mBits;
            bool[] result;

            mBits=ALU.convertAnythingToBits(b); // <- Convert the byte data to bit form
            result=ALU.add(mBits,X.Bits,out overflow,out negitive,out carry,out halfCarry); // <- Add the data to the register data

            return convert8bitsToRegister(result);
        }

        /// <summary>
        /// Adds a byte to a 16 bit Register
        /// </summary>
        public static Register16 add(Register16 X,byte b,out bool overflow,out bool negitive,out bool carry,out bool halfCarry) {
            bool[] mBits;
            bool[] result;

            mBits=ALU.convertAnythingToBits(b); // <- Convert the byte data to bit form
            result=ALU.add(mBits,X.Bits,out overflow,out negitive,out carry,out halfCarry); // <- Add the data to the register data

            return convert16bitsToRegister(result);
        }

        public static void negate(ref Int16 i) {
            bool[] bits=convertAnythingToBits(i);
            negate(ref bits);
            i=convertBitsToInt16(bits);
        }

        public static void negate(ref byte b) {
            bool[] bits=convertAnythingToBits(b);
            negate(ref bits);
            b=convertBitsToByte(bits);
        }

        public static void negate(ref Register R) {
            bool[] bits=R.Bits;
            negate(ref bits);
            R.Bits=bits;
        }

        /// <summary>
        /// Negates bits passed to it
        /// 0's become 1's.
        /// 1's become 0's
        /// </summary>
        public static void negate(ref bool[] bits) {
            for(int i=0;i<bits.Length;i++) {
                if(bits[i]) {
                    bits[i]=false;
                }
                else {
                    bits[i]=true;
                }
            }
        }

        private static Register8 convert8bitsToRegister(bool[] bits) {
            Register8 X=new Register8();
            X.Bits=bits;
            return X;
        }

        private static Register16 convert16bitsToRegister(bool[] bits) {
            Register16 X=new Register16();
            X.Bits=bits;
            return X;
        }

        /// <summary>
        /// Performs binary addition of the two operands and returns the result returns operand1+operand2
        /// </summary>
        /// <param name="overflow">
        /// Returns the result of the addition causes an overflow (carry bit)
        /// </param>
        /// <param name="negitive">
        /// If the number is a 2's complement number, returns if the result is negitive
        /// </param>
        /// <returns>
        /// Returns the result of the operation
        /// </returns>
        public static bool[] add(bool[] operand1,bool[] operand2,out bool overflow, out bool negitive, out bool carry, out bool halfCarry) {
            bool[] r=new bool[operand1.Length];
            bool carryFromLast=false;
            bool j,k,result;
            bool HC=false;

            makeOperandsSameBitLength(ref operand1,ref operand2);

            for(int i=operand1.Length-1;i>=0;i--) {
                j=operand1[i];
                k=operand2[i];

                fullAdd(j,k,out result,out carryFromLast,carryFromLast); // Full adder

                if((operand1.Length==8)&&(i==3)&&(carryFromLast)){
                    HC=true; // Since there is a carry out from bit position 3 to bit position 4, and this is is a 8 bit addition
                }

                r[i]=result;
            }

            carry=carryFromLast;
            negitive=r[0]==true;
            overflow=(r[0]==operand1[0])&&(r[0]==operand2[0]); // The sign must match both the signs of the operands
            halfCarry=HC;

            return r;
        }

        public static bool[] add(bool[] operand1,bool[] operand2) {
            bool overflow,negitive,carry,halfCarry;
            return add(operand1,operand2,out overflow,out negitive,out carry,out halfCarry);
        }

        /// <summary>
        /// Makes two operands have the same bit length by padding either of them
        /// </summary>
        public static void makeOperandsSameBitLength(ref bool[] operand1,ref bool[] operand2) {
            bool[] r;
            
            if(operand1.Length>=operand2.Length) {
                r=new bool[operand1.Length];
                for(int i=0;i<operand2.Length;i++) {
                    int k=(operand2.Length-1)-i;
                    r[(operand1.Length-1)-i]=operand2[k];
                }
                operand2=r;
            }
            else {
                r=new bool[operand2.Length];
                for(int i=0;i<operand1.Length;i++) {
                    int k=operand1.Length-i;
                    r[operand2.Length-i]=operand1[k];
                }
                operand1=r;
            }

            
        }

        /// <summary>
        /// A Half Adder
        /// </summary>
        private static void halfAdd(bool a,bool b,out bool result,out bool carry) {
            result=a^b;
            carry=a&&b;
        }

        /// <summary>
        /// A ` Adder
        /// </summary>
        private static void fullAdd(bool a,bool b,out bool result,out bool carry,bool carryInitial) {
            bool carryA;
            halfAdd(a,b,out result,out carryA);
            halfAdd(carryInitial,result,out result,out carry);
            carry=carryA||carry;
        }

        /// <summary>
        /// Performs binary subtraction with the two operands and returns the result
        /// returns operand1-operand2
        /// </summary>
        public static bool[] subtract(bool[] operand1,bool[] operand2,out bool overflow,out bool negitive,out bool carry,out bool halfCarry) {
            bool[] returnValue;

            // Saftey mechanism to make sure operands have same length:
            makeOperandsSameBitLength(ref operand1,ref operand2);

            // Negate operand2:
            negate(ref operand2); // (1's complement)

            // Add 1 to operand2:
            operand2=add(operand2,new bool[] { true },out overflow,out negitive,out carry,out halfCarry); // (2's complement)

            // Add the operands together(since operand2 has been 2's complemented, the result of the addition should be negitive)
            returnValue=add(operand1,operand2,out overflow,out negitive,out carry,out halfCarry);

            return returnValue;
        }

        public static bool[] subtract(bool[] operand1,bool[] operand2) {
            bool overflow, negitive, carry, halfCarry;
            return subtract(operand1,operand2,out overflow,out negitive,out carry,out halfCarry);
        }

        public static void increment(ref bool[] operand) { // TODO Test
            bool[] r=new bool[operand.Length];

            bool[] one=new bool[]{true};
            operand=add(operand,one); // TODO Overflows etc..
        }

        public static void decrement(ref bool[] operand) { // TODO Test
            bool[] r=new bool[operand.Length];

            bool[] one=new bool[] { true };
            operand=subtract(operand,one); // TODO Overflows etc..
        }


        public static bool[] bitwiseAnd(bool[] operand1,bool[] operand2) {
            bool[] r=new bool[operand1.Length];

            for(int i=0;i<operand1.Length;i++) {
                r[i]=operand1[i] && operand2[i];
            }

            return r;
        }

        public static bool[] bitwiseOr(bool[] operand1,bool[] operand2) {
            bool[] r=new bool[operand1.Length];

            for(int i=0;i<operand1.Length;i++) {
                r[i]=operand1[i] || operand2[i];
            }

            return r;
        }

        public static bool[] bitwiseXor(bool[] operand1,bool[] operand2) {
            bool[] r=new bool[operand1.Length];

            for(int i=0;i<operand1.Length;i++) {

                r[i]=operand1[i] ^ operand2[i]; // Boolean XOR

            }

            return r;
        }

        public static bool[] bitwiseNot(bool[] operand) {
            bool[] r=new bool[operand.Length];

            for(int i=0;i<operand.Length;i++) {
                if(operand[i]==true) {
                    r[i]=false;
                }
                else {
                    r[i]=true;
                }
            }

            return r;
        }

        public static bool[] bitwiseNor(bool[] operand1,bool[] operand2) {
            bool[] r=new bool[operand1.Length];

            for(int i=0;i<operand1.Length;i++) {
                r[i]=operand1[i]||operand2[i]; // OR
                if(r[i]==true) { // NOT
                    r[i]=false;
                }
                else {
                    r[i]=true;
                }
            }

            return r;
        }

        public static bool[] bitwiseNand(bool[] operand1,bool[] operand2) {
            bool[] r=new bool[operand1.Length];

            for(int i=0;i<operand1.Length;i++) {
                r[i]=operand1[i]&&operand2[i]; // AND
                if(r[i]==true) { // NOT
                    r[i]=false;
                }
                else {
                    r[i]=true;
                }
            }

            return r;
        }

        public static bool[] setLowNibble(bool[] bits,bool[] nibbleBits,int bitLength) { // Tested
            int nibbleWidth=bitLength/2;

            for(int i=nibbleWidth;i<bitLength;i++) {
                bits[i]=nibbleBits[i-nibbleWidth];
            }

            return bits;
        }

        public static bool[] setHighNibble(bool[] bits,bool[] nibbleBits,int bitLength) { // Tested
            int nibbleWidth=bitLength/2;

            for(int i=0;i<nibbleWidth;i++) {
                bits[i]=nibbleBits[i];
            }

            return bits;
        }

        public static bool[] getHighNibble(bool[] bits, int bitLength) { // Tested
            int nibbleWidth=bitLength/2;
            bool[] returnValue=new bool[nibbleWidth];

            paddBitsIfRequired(ref bits);

            for(int i=0;i<nibbleWidth;i++) {
                returnValue[i]=bits[i];
            }

            return returnValue;
        }

        public static bool[] getLowNibble(bool[] bits, int bitLength) { // Tested
            int nibbleWidth=bitLength/2;
            bool[] returnValue=new bool[nibbleWidth];

            paddBitsIfRequired(ref bits);

            for(int i=nibbleWidth;i<bitLength;i++) {
                returnValue[i-nibbleWidth]=bits[i];
            }

            return returnValue;
        }

        public static void paddBitsIfRequired(ref bool[] bits) {
            int bl=0;
            int bitLength=bits.Length;
            bool paddBits=false;

            if(bitLength<8) {
                bl=8;
                paddBits=true;
            }
            else if((bitLength>8)&&(bitLength<16)) {
                bl=16;
                paddBits=true;
            }
            else if((bitLength>16)&&(bitLength<32)) {
                bl=32;
                paddBits=true;
            }
            else if((bitLength>32)&&(bitLength<64)) {
                bl=64;
                paddBits=true;
            }
            if(paddBits&&(bl!=0)) {
                bits=convertStringToBits(padBinaryString(convertBitsToString(bits),bl));
            }
        }

        public static byte getLowNibbleByte8Bit(byte byteValue) {
            return (byte)(
                byteValue & 0x0f
            );
        }

        public static byte getHighNibbleByte8Bit(byte byteValue) {
            return (byte)(
                (byteValue>>4) & 0x0f
            );
        }


        public static Int16 convertBitsToInt16(bool[] bits) {
            string s=convertBitsToString(bits);
            return Convert.ToInt16(s,2);
        }

        public static string convertBitsToHex(bool[] bits) {
            return convertAnythingToHex(bits);
        }

        public static byte convertBitsToByte(bool[] bits) {
            string s=convertBitsToString(bits);
            return Convert.ToByte(s,2); // Convert from base 2
        }

        public static int convertBitsToInt(bool[] bits) {
            string s=convertAnythingToHex(bits);
            return Convert.ToInt32(s,16); // Convert from base 16
        }

        public static string convertBitsToString(bool[] bits) {
            string s="";
            for(int i=0;i<bits.Length;i++) {
                if(bits[i]) {
                    s+="1";
                    continue;
                }
                s+="0";
            }
            return s;
        }


        public static bool[] convertStringToBits(String binary) {
            bool[] r=new bool[binary.Length];
            for(int i=0;i<binary.Length;i++) {
                if(binary[i]=='1') {
                    r[i]=true;
                    continue;
                }
                r[i]=false;
            }
            return r;
        }

        public static int convertHexToInt(string hex) {
            return Convert.ToInt32(hex,16);
        }

        public static byte convertHexToByte(string hex) {
            return Convert.ToByte(hex,16);
        }

        public static string convertByteToBinaryString(byte b) {
            return Convert.ToString(b,2); // Converts to base 2 representation
        }

        private static string padBinaryString(string binary,int bitLength) {
            // Pad to bit length:
            int a=bitLength-binary.Length;
            return binary.PadLeft(a+binary.Length,'0');
        }


        public static bool[] convertAnythingToBits(byte b) {
            string s=Convert.ToString(b,2);
            return convertAnythingToBits(s);
        }

        public static bool[] convertAnythingToBits(byte b,int bitLength) {
            string s=Convert.ToString(b,2);
            s=padBinaryString(s,bitLength);
            return convertAnythingToBits(s);
        }

        public static bool[] convertAnythingToBits(Int16 b) {
            string s=Convert.ToString(b,2);
            return convertAnythingToBits(s);
        }

        public static bool[] convertAnythingToBits(Int16 b,int bitLength) {
            string s=Convert.ToString(b,2);
            s=padBinaryString(s,bitLength);
            return convertAnythingToBits(s);
        }
        
        public static bool[] convertAnythingToBits(Int32 b) {
            string s=Convert.ToString(b,2);
            return convertAnythingToBits(s);
        }

        public static bool[] convertAnythingToBits(Int32 b,int bitLength) {
            string s=Convert.ToString(b,2);
            s=padBinaryString(s,bitLength);
            return convertAnythingToBits(s);
        }

        public static bool[] convertAnythingToBits(String s) {
            bool[] r=new bool[s.Length];

            for(int i=0;i<s.Length;i++) {
                string a=s.Substring(i,1);
                if(a=="1") {
                    r[i]=true;
                    continue;
                }
                r[i]=false;
            }

            return r;
        }


        public static string convertAnythingToHex(bool[] bits) {
            string s=convertBitsToString(bits);
            Int32 i=Convert.ToInt32(s,2);
            return Convert.ToString(i,16);
        }

        public static string convertAnythingToHex(byte byteValue) {
            return Convert.ToString(byteValue,16);
        }

        public static string convertAnythingToHex(Int16 int16Value) {
            return Convert.ToString(int16Value,16);
        }

        public static string convertAnythingToHex(int intValue) {
            return Convert.ToString(intValue,16);
        }

        public static int convertAnythingToInt(bool[] bits) {
            string s=convertBitsToString(bits);
            return Convert.ToInt32(s,2);
        }

        public static int convertAnythingToInt(byte byteValue) {
            return Convert.ToInt32(byteValue);
        }

        public static int convertAnythingToInt(Int16 int16Value) {
            return Convert.ToInt32(int16Value);
        }

    }
}
