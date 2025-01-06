using System;
using System.Collections.ObjectModel;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.M68HC11.CPU.Operation {
    public class Operation:CommonOperation {

        public Operation() {}

        // SHIT:
        /*public Operation(InstructionMethodInterop.Instruction[] instructions) {
            this.instructions=instructions;
            hasLabelSpecifier=false;
        }*/

        //private AddressMode addressMode=AddressMode.NOT_SPECIFIED;
        //private AddressModeFull addressModeFull=AddressModeFull.NOT_SPECIFIED;
        //private string OpCodeName;
        //private int Code;
        //private int clockCycles;
        private OpMetadata operationMetaData;
        private int lineNumber;

        public override int GetLineNumber() {
            return this.lineNumber;
        }

        public override void SetLineNumber(int line) {
            this.lineNumber=line;
        }

        public override int getClockCycles() {  // The number of clock cycles the instruction would take to fully execute it (Not relevant to virtual processing unit)
            return this.operationMetaData.clockCycles;
            //return (int)clockCycles;
        }

        public override void setOperationMetaData(OpMetadata operationMetaData) {
            this.operationMetaData=operationMetaData;
        }

        public override OpMetadata getOperationMetaData() {
            return this.operationMetaData;
        }

        //public override void setAddressMode(int value) {
        //    addressMode=(AddressMode)value;
        //    addressModeFull=(AddressModeFull)value;

        //    //if(value!=(int)AddressMode.NOT_SPECIFIED) {
        //    //    try {
        //    //        clockCycles=(OpClockCycles)Enum.Parse(typeof(OpClockCycles),GetOpCode()+"_"+GetAddressMode());
        //    //    }
        //    //    catch { }
        //    //}
        //}
        //public override void setOpCode(string value) {
        //    OpCodeName=value;
        //}

        public override string GetAddressMode() {
            return this.operationMetaData.addressMode;
            //return addressMode.ToString();
        }

        public override string GetAddressModeFullName() {
            return this.operationMetaData.addressModeFullName;

            //return addressModeFull.ToString();
        }

        //public override int GetAddressModeValue() {
        //    return (int)addressMode;
        //}

        public override string GetOpCode() {
            return this.operationMetaData.name;
            //return OpCodeName.ToString();
        }

        //public override int GetOpCodeValue() {
        //    return (int)OpCodeName;
        //}

        public override int GetCodeValue(){
            return this.operationMetaData.opCode;
            //return (int)Code;
        }
        
        //public override void setCodeValue(int value) {
        //    Code=(OpMachineCode)value;
        //}

        public override void Invoke() {
            if(methods!=null) {
                methods[0]();
                methods[1]();                
            }
        }

      

        /// <returns>
        /// Forms a string representation of the operation
        /// </returns>
        public override string GetOperation() {
            string returnValue;

            if(isASMDirective) {
                returnValue="ORG ";
            }
            else {
                returnValue=(hasLabelSpecifier?labelSpecifier+": ":"")+this.operationMetaData.name.ToString()+" ";
            }

            if(operands!=null) {
                for(int i=0;i<operands.Count;i++) {
                    CommonOperand operand=operands[i];

                    if(i==0) {
                        if(this.operationMetaData.addressMode=="IMM") {
                            returnValue+="#";
                        }
                    }

                    returnValue+=operand.GetOperand();

                    if(i!=operands.Count-1) {
                        returnValue+=" ";
                    }
                }
            }

            if((comments!="")&&(comments!=null)) {
                returnValue+=" ;"+comments;
            }

            return returnValue;
        }

        public override string ToString() {
            return ALU.convertAnythingToHex(InstructionAddress)+"  "+GetOperation();
        }

        public override string ToHexAddress() {
            return ALU.convertAnythingToHex(InstructionAddress);
        }

        public override string ToMachineCode() {// TODO: JMP instruction needs to show offset byte operand
            string returnValue="";

            string hexOpCode=ALU.convertAnythingToHex(this.operationMetaData.opCode);
            //string hexAddress=ALU.convertAnythingToHex(InstructionAddress);
            //returnValue=hexAddress+"  "+hexOpCode+" ";
            returnValue=hexOpCode+" ";

            if((operands!=null)&&(operands.Count>0)) {
                Object opValue=null;

                if(operands!=null&&operands.Count==1) {
                    opValue=operands[0].Value;
                    if(operands[0].Value.GetType()==typeof(string)) {
                        if(operands[0].isHexadecimalValue) {
                            opValue=ALU.convertHexToInt(operands[0].Value.ToString());
                        }
                    }
                }

                for(int i=0;i<operands.Count;i++) {
                    CommonOperand operand=operands[i]; // TODO: serialise operands and put beside opcode

                    Object opr=null;
                    Type oprT=null;

                    opr=operand.Value;
                    /*
                     * switch(addressMode) { // Cast operands relative to the addressing mode of the operation
                        case AddressMode.DIR:
                            opr=Convert.ToByte(operand.Value);
                            break;
                        case AddressMode.EXT:
                            if(opValue.GetType()==typeof(string)) {
                                opr=operands[0].Value;
                            }
                            else {
                                opr=Convert.ToInt32(opValue);
                            }
                            break;
                        case AddressMode.IMM:
                            if(Convert.ToInt32(opValue)>255) {
                                opr=Convert.ToInt32(opValue);
                            }
                            else {
                                opr=Convert.ToByte(opValue);
                            }
                            break;
                        case AddressMode.INDX:
                            opr=operand.Value;
                            break;
                        case AddressMode.INDY:
                            opr=operand.Value;
                            break;
                        case AddressMode.INHR:
                            break;
                        case AddressMode.REL:
                            opr=Convert.ToByte(operand.Value);
                            break;
                    }*/

                    if(opr!=null) {
                        oprT=opr.GetType();
                        if(oprT==typeof(byte)) {
                            byte v=(byte)opr;

                            string h=ALU.convertAnythingToHex(v);
                            h=h.Length==1?"0"+h:h;
                            returnValue+=h;
                        }
                        else if(oprT==typeof(Int16)) {
                            Int16 v=(Int16)opr;

                            if(v>(Math.Pow(2,8))) {
                                bool[] bits=ALU.convertAnythingToBits(v);
                                bool[] highBits=ALU.getHighNibble(bits,16);
                                bool[] lowBits=ALU.getLowNibble(bits,16);
                                byte highB=ALU.convertBitsToByte(highBits);
                                byte lowB=ALU.convertBitsToByte(lowBits);
                                string highHex=ALU.convertAnythingToHex(highB);
                                string lowHex=ALU.convertAnythingToHex(lowB);

                                highHex=highHex.Length==1?"0"+highHex:highHex;
                                lowHex=lowHex.Length==1?"0"+lowHex:lowHex;
                                returnValue+=highHex+" "+lowHex;
                            }
                            else {
                                string h=ALU.convertAnythingToHex(v);
                                h=h.Length==1?"0"+h:h;
                                returnValue+="00 "+h;
                            }
                        }
                        else if(oprT==typeof(Int32)) {
                            Int32 v=(Int32)opr;
                            bool[] bits=ALU.convertAnythingToBits(v);
                            ALU.paddBitsIfRequired(ref bits);

                            if(bits.Length==16) { // If 16 bit value (2 byte)
                                bool[] highBits=ALU.getHighNibble(bits,16);
                                bool[] lowBits=ALU.getLowNibble(bits,16);
                                byte highB=ALU.convertBitsToByte(highBits);
                                byte lowB=ALU.convertBitsToByte(lowBits);
                                string highHex=ALU.convertAnythingToHex(highB);
                                string lowHex=ALU.convertAnythingToHex(lowB);

                                highHex=highHex.Length==1?"0"+highHex:highHex;
                                lowHex=lowHex.Length==1?"0"+lowHex:lowHex;
                                returnValue+=highHex+" "+lowHex;
                            }
                            else if(bits.Length==8) {
                                byte b=ALU.convertBitsToByte(bits);
                                string bHex=ALU.convertAnythingToHex(b);

                                if(GetOpCode()=="JMP") {
                                    bool[] bb=ALU.convertAnythingToBits(operand.LinkedOperation.InstructionAddress);
                                    bb=ALU.getHighNibble(bb,16);
                                    bHex=bHex.Length==1?"0"+bHex:bHex;
                                    bHex=ALU.convertBitsToHex(bb)+" "+bHex;
                                }

                                returnValue+=bHex;
                            }
                        }
                        else if(oprT==typeof(string)) {
                            if(opr.ToString().Length==4) {
                                opr=opr.ToString().Substring(0,2)+" "+opr.ToString().Substring(2,2);
                            }
                            returnValue+=opr;
                        }
                    }
                    //returnValue+="\t\t";
                    //returnValue+=(hasLabelSpecifier?labelSpecifier+": ":"")+OpCode.ToString()+" ";
                    //returnValue+=addressMode==AddressMode.IMM?"#":"";
                    //returnValue+=operand.GetOperand()+" ";
                }
            }
            else { // Has no operands
                //if(hasLabelSpecifier) {
                //    //returnValue+="\t"+labelSpecifier+": \t";
                //}
                //else {
                //    //returnValue+="\t\t";
                //}
                //returnValue+=this.operationMetaData.name;//returnValue+=OpCodeName.ToString();
            }

            return returnValue;
        }

        public override string ToMachineCodeDetails() {
            string returnValue="";

            returnValue+="bytes: "+addressSpace+", cycles: "+this.operationMetaData.clockCycles;
            //returnValue+="bytes: "+addressSpace+", cycles: "+getClockCycles();

            return returnValue;
        }
    }

}
