using System;
using System.Collections.ObjectModel;

//using Ewok.M68HC11.CPU.Operation;

namespace Ewok.Machine.Common {
    public abstract class CommonOperation {

        public Collection<CommonOperand> operands;
        public bool hasLabelSpecifier;
        public string labelSpecifier;
        public bool isASMDirective;
        public int addressSpace; // The number of addresses the operation consumes (set when we know address mode)
        public string comments;  // Any comments to the right of the operation
        public int InstructionAddress;
        public InstructionMethodInterop.Method[] methods;

        public abstract OpMetadata getOperationMetaData();
        public abstract void setOperationMetaData(OpMetadata operationMetaData);
        public abstract int getClockCycles();
        public abstract string GetAddressMode();
        public abstract string GetAddressModeFullName();
        //public abstract int GetAddressModeValue();
        public abstract string GetOpCode();
        //public abstract int GetOpCodeValue();
        public abstract void Invoke();
        public abstract string GetOperation();
        public abstract string ToHexAddress();
        public abstract string ToMachineCode();
        public abstract string ToMachineCodeDetails();
        public abstract int GetCodeValue();
        public abstract int GetLineNumber();
        public abstract void SetLineNumber(int line);
        //public abstract void setCodeValue(int value);
        //public abstract void setAddressMode(int value);
        //public abstract void setOpCode(int value);
    }
}
