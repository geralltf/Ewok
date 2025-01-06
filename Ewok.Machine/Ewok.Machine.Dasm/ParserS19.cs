/* At the moment the S19 parser is limited only to parsing out 6811 opcodes, 
 * if the S19 binary was not compiled for the 6811, then the parser will have problems disassembling the binary */

using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;
using Ewok.M68HC11.CPU.Operation;
using System.Reflection;
using Ewok.M68HC11;
using Ewok.Machine.Debug;
using System.Text.RegularExpressions;

namespace Ewok.Machine.Dasm.S19 {
    public class ParserS19 {

        public ParserS19(Ref<MPU> ProcessingUnitReference,Ref<Debugger> debuggerReference) { // needs to reference accessor
            this.ProcessingUnitReference=ProcessingUnitReference;
            this.DebuggerReference=debuggerReference;

            AddressModeFull addressingMode=new AddressModeFull();
            OpMetadata.InitiliseOperationMetadata(ProcessingUnitReference.Value.GetType(),addressingMode);
        }

        private Ref<MPU> ProcessingUnitReference;
        private MPU MPU {
            get {
                return ProcessingUnitReference.Value;

            }
            set {
                ProcessingUnitReference.Value=value;
            }
        }

        private Ref<Debugger> DebuggerReference;
        private Debugger debugger {
            get {
                return DebuggerReference.Value;
            }
            set {
                DebuggerReference.Value=value;
            }
        }

        public Dictionary<int,CommonOperation> Parse(string s19File,out Register startingAddress) {
            Dictionary<int,CommonOperation> operations;
            Collection<S19Record> s19Records;
            //Dictionary<int,CommonOperation> recordDataOps;
            string[] lines;
            int pc;

            operations=new Dictionary<int,CommonOperation>();
            s19Records=new Collection<S19Record>();
            lines=File.ReadAllLines(s19File);
            foreach(string line in lines) {
                s19Records.Add(new S19Record(line));
            }

            if(s19Records.Count>0) {
                if(s19Records[0].RecordType!=S19Record.S19RecordType.S1_DATA_SEQUENCE) {
                    throw new InvalidProgramException(S19Record.INV_S19_FILE_FORMAT);
                }

                pc=Convert.ToInt32(s19Records[0].Address,16); // The start address of the program

                for(int i=0;i<s19Records.Count;i++ ) {
                    S19Record r=s19Records[i];
                    S19Record nextR;

                    if(r.RecordType==S19Record.S19RecordType.S1_DATA_SEQUENCE) {
                        if(i!=s19Records.Count) {
                            nextR=s19Records[i+1];
                            parseS19RecordData(r,ref pc,ref nextR,ref operations);
                        }
                        else{
                            S19Record crap=null;
                            parseS19RecordData(r,ref pc,ref crap,ref operations);
                        }
                    }
                }

            }

            startingAddress=new Register16();
            if(!getORGDirective(s19File,out startingAddress)) {
                if(operations.Count>0) {
                    startingAddress.Bits=ALU.convertAnythingToBits(operations[0].InstructionAddress);
                }
                else {
                    startingAddress=getDefaultStartAddress();
                }
            }
           
            return operations;
        }

        private bool getORGDirective(string s19File,out Register startingAddress) {
            startingAddress=null;
            FileStream fs=File.OpenRead(s19File.Replace(".s19",".tmp"));
            byte[] bData=new byte[20];
            fs.Read(bData,0,bData.Length);
            fs.Dispose();
            string sOrgDirective=System.Text.ASCIIEncoding.ASCII.GetString(bData).Trim().Split('\r')[0];
            if(sOrgDirective.ToUpper().StartsWith("ORG")) {
                Collection<CommonOperand> operands=getORGOperand(sOrgDirective);

                if(operands.Count==1) {
                    int addr;
                    if(operands[0].isHexadecimalValue) {
                        addr=Convert.ToInt32(operands[0].Value.ToString(),16);
                    }
                    else {
                        addr=(int)operands[0].Value;
                    }
                    startingAddress=new Register16();
                    startingAddress.Bits=ALU.convertAnythingToBits(addr);

                    return true;
                }
            }
            return false;
        }

        private Collection<CommonOperand> getORGOperand(string orgDirective) {
            Collection<CommonOperand> ret=new Collection<CommonOperand>();
            Regex r;
            MatchCollection mc;
            string v;
            Operand o;

            if(orgDirective.Contains(",")) {
                if(orgDirective.Contains(", ")) {

                }
                else {
                    orgDirective=orgDirective.Replace(",",", ");
                }
            }

            r=new Regex("(?<=[A-Z]*[\\s])(?:[\\s]*)(?<=[\\s]?)[#]?([$]?)([a-zA-Z0-9]*(?=[,]?))"); 
            // This pattern does not work yet with address mode specifier '#'

            mc=r.Matches(orgDirective);

            foreach(Match m in mc) {
                v=m.Groups[2].Value;

                if(v.Length>0) {
                    v=v.Trim();
                    o=new Operand();

                    o.isHexadecimalValue=m.Groups[1].Value=="$";
                    o.Value=v;

                    ret.Add(o);
                }
            }

            return ret;
        }

        private Register getDefaultStartAddress() {
            Register startingAddress=new Register16();
            startingAddress.Bits=ALU.convertAnythingToBits(Convert.ToInt32("c000",16));
            return startingAddress;
        }

        private void parseS19RecordData(S19Record r,ref int currentPCValue,ref S19Record nextRecord,ref Dictionary<int,CommonOperation> operations) {
            string data;
            string hOp;
            string hOp_1byte;
            string hOp_2byte;
            int machineCode;
            int prevOperationSize; // The size in bytes of the operation in memory
            CommonOperation op;
            int operandSize;
            
            string operands;

            //operations=new Dictionary<int,CommonOperation>();
            data=r.Data;

            // get each operation, determine its type, and increment the PC accordingly:

            while(data!=""){
            //for(int i=0;i<data.Length;i+=prevOperationSize*2) {
                hOp_1byte=data.Substring(0,2); // Determine correct opcode value from these two values

                try {
                    hOp_2byte=data.Substring(0,4);}catch { hOp_2byte=""; }

                if(OpMetadata.hasOpCode(hOp_1byte)) {
                    hOp=hOp_1byte;
                }
                else {
                    if(OpMetadata.hasOpCode(hOp_2byte)) {
                        /* If the opcode cant be found if we assume it is 1 byte long, then it must be a 2 byte long opcode */
                        hOp=hOp_2byte;
                    }
                    else {
                        throw new InvalidOperationException("<S19 Parser Exception> OpCode: "+hOp_2byte+" does not exist in 6811 opcode database");
                    }
                }

                OpMetadata instruction=OpMetadata.getInstructionByOpCode(hOp);


                machineCode=instruction.opCode; //OpMetaData.getOpMachineCodeById(hOp);
                prevOperationSize=instruction.OperationSize;//OpMetaData.getSizeOfOperation(machineCode);
                operandSize=instruction.operandSize;//OpMetaData.getOpCodeOperandSize(machineCode);
                try {
                    if(operandSize==2) {
                        operands=data.Substring(0+prevOperationSize-1,operandSize*2);
                    }
                    else {
                        operands=data.Substring(0+prevOperationSize,operandSize*2);
                    }
                }
                catch {
                    // Sometimes operands aren't cleanly found sequentially on the same S19Record 
                    //try {
                        operands=data.Substring(0+prevOperationSize-1,2)+nextRecord.Data.Substring(0,(operandSize-1)*2); // I think bugs that occur here are because of the assembler!
                    //}
                    //catch {
                        //operands=data.Substring(0,2)+nextRecord.Data.Substring(0,(operandSize-1)*2);
                    //}
                    nextRecord.Data=nextRecord.Data.Remove(0,(operandSize-1)*2);
                }

                op=FormOperation(machineCode,operandSize,operands);

                op.InstructionAddress=currentPCValue;
                op.addressSpace=instruction.OperationSize;// getOperationAddressSpace(op);

                operations.Add(op.InstructionAddress,op);

                currentPCValue+=op.addressSpace;//OpMetaData.getSizeOfOperation((OpMachineCode)op.GetCodeValue());

                try {
                    data=data.Substring(prevOperationSize*2,data.Length-prevOperationSize*2);
                }
                catch {
                    data="";
                }
            }

            if(operations.Count>0) { // TODO: use currentPCValue and build it as operations are found
                //operations=buildOperationAddresses(operations);
            }
        }
        
        public CommonOperation FormOperation(int opCode, int operandCount,string operands) {
            CommonOperation op=new Operation();
            // Thanks to the assembler there are no invalid opcodes or addresses, and labels are already resolved.
            // Obviously operations here are not assembler directives

            op.hasLabelSpecifier=false;// The assembler has processed out labels, resolving them to addresses
            OpMetadata instruction=OpMetadata.getInstructionByOpCode(opCode);
            
            string opn=instruction.name;// OperationMD2.getOpCodeNameById(opCode);
            //string opn=OpMetaData.getOpCodeNameById((int)opCode);
            //int i=opn.IndexOf('_');
            //opn=opn.Remove(i,opn.Length-i);
            op.setOperationMetaData(instruction);

            //op.setOpCode((int)Enum.Parse(typeof(OpCode),opn));
            //op.setCodeValue((int)opCode);
            //op.setAddressMode((int)OpMetaData.getOperationAddressMode(opCode));

            op.operands=getOperands(op,operands,operandCount,ref instruction);

            // Create coresponding method reference for Operation:
            op.methods=buildInstructionMethodReference(op,ref instruction);

            return op;
        }

        private Collection<CommonOperand> getOperands(CommonOperation operation,string operands,int count,ref OpMetadata instruction) {
            Collection<CommonOperand> ret=new Collection<CommonOperand>();
            CommonOperand operand;
            string v;
            Object opr;
            string addressMode;

            //int operandCount=OpMetaData.getOpCodeOperandSize((OpMachineCode)operation.GetCodeValue());
            int operandCount=instruction.operandSize;

            addressMode=instruction.addressMode;
            //addressMode=(AddressMode)operation.GetAddressModeValue();

            if(count>0) {
                int len;
                if(count==1) {
                    len=2;
                }
                else if(count==2) {
                    count=1;
                    len=4;
                }
                else {
                    len=2;
                }

                //for(int i=0;i<count*2;i+=2) {
                operand=new Operand();
                v=operands.Substring(0,len); //v=operands.Substring(i,2);
                opr=v;
                operand.isHexadecimalValue=true; // The assembler always returns hexed operands'

                switch(addressMode) {
                    case "DIR":
                        if(v.GetType()==typeof(string)) {
                            opr=v;
                            operand.isHexadecimalValue=true;
                        }
                        else {
                            opr=Convert.ToByte(v);
                            operand.isHexadecimalValue=false;
                        }
                        break;
                    case "EXT":
                        if(v.GetType()==typeof(string)) {
                            opr=v;
                        }
                        else {
                            opr=Convert.ToInt32(v);
                            operand.isHexadecimalValue=false;
                        }
                        break;
                    case "IMM":
                        if(Convert.ToInt32(v,16)>255) {
                            opr=Convert.ToInt32(v,16);
                            operand.isHexadecimalValue=false;
                        }
                        else {
                            opr=Convert.ToByte(v,16);
                            operand.isHexadecimalValue=false;
                        }
                        break;
                    case "INDX":
                        opr=operand.Value;
                        break;
                    case "INDY":
                        opr=operand.Value;
                        break;
                    case "INHR":
                        break;
                    case "REL":
                        //opr=Convert.ToByte(operand.Value);
                        opr=Convert.ToByte(v,16);
                        operand.isHexadecimalValue=false;
                        break;
                }

                operand.Value=opr;

                ret.Add(operand);
                //}

            }

            return ret;
        }


        private int getOperationAddressSpace(CommonOperation operation) {
            return operation.getOperationMetaData().OperationSize;
            //return OpMetaData.getSizeOfOperation((OpMachineCode)operation.GetCodeValue());
        }

        /// <summary>
        /// Goes through each operation and determines an instruction address 
        /// based on the previous address and how many operands there are for the operation
        /// </summary>
        private Collection<CommonOperation> buildOperationAddresses(Collection<CommonOperation> operations) {
            int startingAddress;
            CommonOperand oprStartAddr=operations[0].operands[0]; // BUG: assembly does not contain directives
            Object obj;

            if(oprStartAddr.isHexadecimalValue) {
                obj=ALU.convertHexToInt(oprStartAddr.Value.ToString());
            }
            else {
                obj=oprStartAddr.Value;
            }

            startingAddress=Convert.ToInt32(obj);

            int j=0;
            int k;
            for(int i=0;i<operations.Count;i++) {
                CommonOperation op=operations[i];
                if((op.GetOpCode()!="")&&(op.isASMDirective==false)) {
                    operations[i].InstructionAddress=(int)(startingAddress+j);
                    k=0;
                    if(op.operands.Count>0) {
                        switch(op.GetAddressMode()) {
                            case "DIR":
                                k+=1; // 1 byte operand
                                break;
                            case "EXT":
                                k+=2; // 2 byte operand
                                break;
                            case "IMM":
                                k+=1; // 1 byte operand (not sure)
                                break;
                            case "INDX":
                                k+=1; // 1 byte operand
                                break;
                            case "INDY":
                                k+=1; // 1 byte operand
                                break;
                            case "INHR":
                                break;
                            case "REL":
                                k+=1; // 1 byte operand
                                break;
                        }
                    }
                    // Need to add size of the opcode (in bytes):
                    Int16 opCode=Convert.ToInt16(op.GetCodeValue());
                    if(opCode>Math.Pow(2,8)) {
                        // its a 16 bit opcode (2 byte opcode)
                        k+=2;
                    }
                    else {
                        // its a 8 bit opcode (1 byte opcode)
                        k++;
                    }
                    op.addressSpace=k;
                    j+=k;
                }
                // HACK Need to determine how many operands there are for the operation, and have it affect its instruction address
                // if an operand is 16 bit, then it is 2 bytes long, and therefore requires an extra 2 addresses
            }

            return operations;
        }


        private InstructionMethodInterop.Method[] buildInstructionMethodReference(CommonOperation op,ref OpMetadata instruction) {
            InstructionMethodInterop.Method i;
            InstructionMethodInterop.Method lbl;
            InstructionMethodInterop.Method[] ins;
            string methodName;
            MethodInfo mi;

            i=delegate() { };
            lbl=delegate() { };

            // If its not an operation, dont assign any instructions
            if(op.GetOpCode()=="") { // NOT_AN_OPERATION
                goto returns;
            }

            // If there is a lable for this operation, then make sure to add it
            lbl=delegate() {
                if(op.hasLabelSpecifier) {
                    MPU.LABEL=op.labelSpecifier; // TODO: Fix this, use assembler's address for jumping
                }
            };

            // Find the associated method for the operation. 
            // There is a method naming syntax: OPCODE_ADDRESSINGMODE(operands/parameters) i.e. "ABA_INHR()"
            // If the method naming syntax is not followed, or the method for an operation is not implemented,
            // then the parser can not continue.
            methodName=instruction.methodName;
            //methodName=op.GetOpCode()+"_"+op.GetAddressMode();
            //methodName=((OpMachineCode)Convert.ToInt32(op.GetOpCode())).ToString();
            //OperationMD2 _instruction;
            //if(!OperationMD2.InstructionSet.TryGetValue(methodName,out _instruction)) {
                //throw new Exception("Operation, '"+methodName+"' method data not found in the Processing Unit.\nAdd it so the parser can continue.");
            //}
            
            //mi=_instruction.method;
            mi=instruction.method;
            //mi=MPU.GetType().GetMethod(methodName);

            if(mi!=null) {
                object[] methodParameters=new object[1];

                i=delegate() { // This delegate gets executed when an associated operation is invoked at runtime
                    aquireMethodParameters(op,ref methodParameters); // Must be done on post of Parse
                    try {
                        mi.Invoke(MPU,methodParameters); // Invoke the operation, passing in the correct operands
                    }
                    catch(Exception ex) { // Determine the type of error by how the operation was addressed:
                        throw ex;
                        //processRuntimeException(op,ex);
                    }
                };

            }
            else {
                //processParsingException(new Exception("Operation, '"+methodName+"' method not found in the Processing Unit.\nImplement it so the parser can continue."));
                throw new Exception("Operation, '"+methodName+"' method not found in the Processing Unit.\nImplement it so the parser can continue.");
            }

        returns:
            ins=new InstructionMethodInterop.Method[2];
            ins[0]=lbl;
            ins[1]=i;
            return ins;
        }

        private void aquireMethodParameters(CommonOperation op,ref object[] methodParameters) {
            Object opValue=null;

            if(op.operands!=null&&op.operands.Count==1) {
                opValue=op.operands[0].Value;
                if(opValue.GetType()==typeof(string)) {
                    if(op.operands[0].isHexadecimalValue) {
                        opValue=ALU.convertHexToInt(opValue.ToString());
                    }
                    else {
                        opValue=op.operands[0].Value;
                    }
                }
            }

            switch(op.GetAddressMode()) { // Cast operands relative to the addressing mode of the operation
                case "DIR":
                    methodParameters[0]=Convert.ToByte(opValue);
                    break;
                case "EXT":
                    if(opValue.GetType()==typeof(string)) {
                        methodParameters[0]=op.operands[0].Value;
                    }
                    else if(opValue.GetType()==typeof(byte)) {
                        methodParameters[0]=Convert.ToByte(opValue);
                    }
                    else if(opValue.GetType()==typeof(Int16)) {
                        methodParameters[0]=Convert.ToInt16(opValue);
                    }
                    else if(opValue.GetType()==typeof(Int32)) {
                        try {
                            methodParameters[0]=Convert.ToInt16(opValue);
                        }
                        catch {// TODO Refactor for compiler errors:
                            methodParameters[0]=Convert.ToInt32(opValue);

                            break;
                            string insAddr=ALU.convertAnythingToHex(op.InstructionAddress);
                            Int32 v=(Int32)opValue;
                            string hv=ALU.convertAnythingToHex(v);

                            throw new Exception("["+insAddr+"] Operand '$"+hv+"' a 32 bit value, must be a 16 bit or 8 bit value. Since the operation implies extended addressing");
                            //processParsingException(new Exception("["+insAddr+"] Operand '$"+hv+"' a 32 bit value, must be a 16 bit or 8 bit value. Since the operation implies extended addressing"));
                        }
                    }
                    break;
                case "IMM": // just found out the operand may be 16 bit or 8 bit depending on the operation
                    if(Convert.ToInt32(opValue)>255) { // operand is not 8 bit
                        string insAddr=ALU.convertAnythingToHex(op.InstructionAddress);
                        Int32 v=(Int32)opValue;
                        Object v2;
                        string hv=ALU.convertAnythingToHex(v);
                        Type t;

                        // Determine what type the value is:
                        try {
                            v2=Convert.ToByte(opValue);
                            t=typeof(byte);
                        }
                        catch {
                            try {
                                v2=Convert.ToInt16(opValue);
                                t=typeof(Int16);
                            }
                            catch {
                                v2=Convert.ToInt32(opValue);
                                t=t=typeof(Int32); ;
                            }
                        }

                        string methodName=op.GetOpCode()+"_"+op.GetAddressMode();
                        MethodInfo mi=MPU.GetType().GetMethod(methodName);
                        Type pi=mi.GetParameters()[0].ParameterType;

                        if(pi==typeof(Int32)) { } // N/A
                        else if(pi==typeof(Int16)) { // Method requires int 16 parameter
                            if(t==typeof(Int32)) {
                                if(!op.isASMDirective) {
                                    //processParsingException(new Exception("["+insAddr+"] Operand '$"+hv+"' a "+t+" bit value, can not be a "+t+" bit value. Since the operation implies immediate addressing"));
                                    throw new Exception("["+insAddr+"] Operand '$"+hv+"' a "+t+" bit value, can not be a "+t+" bit value. Since the operation implies immediate addressing");
                                }
                            }
                            else if(t==typeof(Int16)) {
                                methodParameters[0]=v2;
                            }
                        }
                        else if(pi==typeof(byte)) { // Method requires int 8/byte parameter
                            methodParameters[0]=v2;
                        }

                        if(op.isASMDirective) {
                            methodParameters[0]=v2;
                        }
                    }
                    else {
                        methodParameters[0]=Convert.ToByte(opValue);
                    }
                    break;
                case "INDX":
                    methodParameters[0]=Convert.ToByte(op.operands[0].Value);
                    break;
                case "INDY":
                    methodParameters[0]=Convert.ToByte(op.operands[0].Value);
                    break;
                case "INHR":
                    methodParameters=null;
                    break;
                case "REL":
                    methodParameters[0]=Convert.ToByte(op.operands[0].Value);
                    break;
            }
        }

    }
}
