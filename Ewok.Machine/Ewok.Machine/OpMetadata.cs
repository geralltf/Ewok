using System;
using System.Reflection;
using System.Collections.Generic;

namespace Ewok.Machine.Common {

    [AttributeUsage(AttributeTargets.Method,Inherited=false,AllowMultiple=true)]
    public sealed class OpMetadata:Attribute {

        public OpMetadata(int opCode,int clockCycles,int opCodeSize,int operandSize) {
            this.opCode=opCode;
            this.clockCycles=clockCycles;
            this.opCodeSize=opCodeSize;
            this.operandSize=operandSize;
        }

        /// <summary>
        /// For assembler directives
        /// </summary>
        public OpMetadata() {}

        public string addressModeFullName;
        public string addressMode;  // Derived from the name of the method
        public string name;         // Derived from the name of the method
        /// <summary>
        /// The Operation Code of the instruction
        /// </summary>
        public int opCode;

        /// <summary>
        /// Number of clock cycles an operation consumes from the processor
        /// </summary>
        public int clockCycles;

        /// <summary>
        /// OpCodeSize stores how many bytes an indivudual OpCode consumes in memory
        /// </summary>
        public int opCodeSize; 

        /// <summary>
        /// OpCodeOperandSize stores how many bytes the operands of an OpCode consume in memory
        /// </summary>
        public int operandSize;

        public string methodName;
        public MethodInfo method;

        public string Description { get; set; }

        public int OperationSize {
            get {
                return this.opCodeSize+this.operandSize;
            }
        }

        /// <summary>
        /// Used to fetch the instructions a type of ProcessingUnit implements.
        /// The parser for the processing unit must call this method in it's constructor,
        /// and may then be able to use the static methods defined in OperationMD2 to query the instruction set
        /// for information on an operation.
        /// </summary>
        /// <param name="ImplementedProcessingUnit">
        /// A decendant class of ProcessingUnit, otherwise known as a type of ProcessingUnit.
        /// This type of ProcessingUnit must code it's instructions of the form: MNEMONIC_ADDRESSMODE(OperandType operandType)
        /// And for each instruction, there must be an OperationMD2 attribute with metadata about the instruction.
        /// </param>
        public static void InitiliseOperationMetadata(Type ImplementedProcessingUnit,AddressMode addressingMode) {
            MethodInfo[] mpu_methods_col=ImplementedProcessingUnit.GetMethods();
            object[] attributes;
            OpMetadata instructionType;

            OpMetadata.InstructionSet=new Dictionary<string,OpMetadata>();

            foreach(MethodInfo method in mpu_methods_col) {
                attributes=method.GetCustomAttributes(false);
                if(attributes.Length>0) {
                    foreach(object attribute in attributes) {
                        if(attribute.GetType()==typeof(OpMetadata)) {
                            instructionType=(OpMetadata)attribute;
                            instructionType.methodName=method.Name;
                            instructionType.method=method;

                            instructionType.name=method.Name.Split('_')[0];
                            try {
                                instructionType.addressMode=method.Name.Split('_')[1];
                                instructionType.addressModeFullName=addressingMode.getAddressModeFull(instructionType.addressMode);
                            }
                            catch {
                                instructionType.addressMode="ADDR_MODE_UNDEFINED";
                            }

                            OpMetadata.InstructionSet.Add(method.Name,instructionType);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// An set of instructions for a processor found by reflecting its processing unit OperationMD2 method attributes
        /// </summary>
        public static Dictionary<string,OpMetadata> InstructionSet;
        

        public static OpMetadata getInstructionByOpCode(int opcode) {
            foreach(OpMetadata instruction in InstructionSet.Values) {
                if(instruction.opCode==opcode) {
                    return instruction;
                }
            }
            throw new InvalidCommonOperationException((int)opcode);
        }

        public static bool hasMnemonic(string mnemonic) {
            foreach(OpMetadata instruction in InstructionSet.Values) {
                if(instruction.name==mnemonic) {
                    return true;
                }
            }
            return false;
        }

        public static OpMetadata getInstructionByOpCode(string hexOpCode) {
            int opcode=getOpMachineCodeById(hexOpCode);
            foreach(OpMetadata instruction in InstructionSet.Values) {
                if(instruction.opCode==opcode) {
                    return instruction;
                }
            }
            throw new Exception("InvalidCommonOperation: "+hexOpCode);
        }

        public static int getNumClockCycles(int machineOpCode) {
            return getInstructionByOpCode(machineOpCode).clockCycles;
        }

        public static int getNumClockCycles(string hexMachineOpCode) {
            return getNumClockCycles(Convert.ToInt32(hexMachineOpCode,16));
        }

        public static string getOpCodeNameById(int id) {
            return getInstructionByOpCode(id).methodName;
        }

        public static string getOpCodeNameById(string id) {
            return getOpMachineCodeById(id).ToString();
        }

        public static int getOpCodeSize(int machineOpCode) {
            try {
                return getInstructionByOpCode(machineOpCode).opCodeSize;
            }
            catch {
                throw new InvalidCommonOperationException(machineOpCode);
            }
        }

        public static int getOpCodeOperandSize(int machineOpCode) {
            try {
                return getInstructionByOpCode(machineOpCode).operandSize;
            }
            catch {
                throw new InvalidCommonOperationException(machineOpCode);
            }
        }

        public static int getOpMachineCodeById(string hexOpCode) {
            return Convert.ToInt32(hexOpCode,16);
        }

        public static int getSizeOfOperation(int machineOpCode) {
            try {
                OpMetadata instruction=getInstructionByOpCode(machineOpCode);
                return instruction.OperationSize;
            }
            catch {
                throw new InvalidCommonOperationException(machineOpCode);
            }
        }

        public static bool hasOpCode(string machineOpCode) {  /* returns if the opcode exists */
            try {
                getInstructionByOpCode(getOpMachineCodeById(machineOpCode)); // This throws an exception when an instruction cant be found
                return true;
            }
            catch {
                return false;
            }
        }

        public static string getOperationAddressMode(int opcode) { // AddressMode
            string operationName;

            try {
                operationName=getOpCodeNameById((int)opcode);
                OpMetadata instruction=getInstructionByOpCode(opcode);

                return instruction.addressMode;

                //return (AddressMode)Enum.Parse(typeof(AddressMode),sAddrMode);
            }
            catch {
                return "";
                //return AddressMode.NOT_SPECIFIED;
            }
        }
    }
}
