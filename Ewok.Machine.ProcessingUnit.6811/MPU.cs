using System;
using System.Collections.ObjectModel;
using System.Threading;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;
using Ewok.M68HC11.Registers;

namespace Ewok.M68HC11 {
    public class MPU:ProcessingUnit {

        #region Micro Processing Unit

        public MPU() {
            D=new Accumulator16();
            dReference=new Ref<Accumulator16>(() => D,z => { D=z; });
            A=new Accumulator8(dReference,Accumulator8Type.A); // Accumulator A needs to update high nibble of Accumulator D by accessing D through its reference
            B=new Accumulator8(dReference,Accumulator8Type.B); // Accumulator B needs to update low  nibble of Accumulator D by accessing D through its reference
            aReference=new Ref<Accumulator8>(() => A,z => { A=z; });
            bReference=new Ref<Accumulator8>(() => B,z => { B=z; });
            D.SetDerivative(aReference,bReference);

            IX=new IndexRegister();
            IY=new IndexRegister();
            PC=new ProgramCounter();
            SP=new StackPointer();
            CCR=new ConditionCodeRegister();

            RAM=new RAM(64000); // Creates an new RAM instance with 64kB of memory (64000 bytes of memory)
            AccumulatorStack=new Stack<Register>(); // The stack increments and decrements the StackPointer (so it needs this SP object)
            CallStack=new Stack<Register>(); // Keeps track of addresses to return to after calling a subroutine
            // TODO: link CallStack to VRAM
            CCR.set8BitValue(0x00); // Sets all CCR flags to false/0

            branchingLabels=new System.Collections.ObjectModel.Collection<BranchingLabel>();

            endExecutionInterrupt=false;
            incrementThePC=true;

            PC.programCounterUpdatedEventHandler+=new ProgramCounter.programCounterUpdatedEventDelegate(PC_programCounterUpdatedEventHandler);
        }

        private void PC_programCounterUpdatedEventHandler(bool[] currentAddressBits) {
            if(programCounterUpdatedEventHandler!=null) {
                programCounterUpdatedEventHandler(currentAddressBits);
            }
        }
        public override event ProcessingUnit.programCounterUpdatedEventDelegate programCounterUpdatedEventHandler;

        public override System.Collections.Generic.Dictionary<string,object> GetRegisterStatus() {
            System.Collections.Generic.Dictionary<string,object> machineState;

            machineState=new System.Collections.Generic.Dictionary<string,object>();
            machineState.Add("A",A.ToString());
            machineState.Add("B",B.ToString());
            machineState.Add("D",D.ToString());
            machineState.Add("X",IX.ToString());
            machineState.Add("Y",IY.ToString());
            machineState.Add("CCR",CCR.ToString());
            machineState.Add("SP",SP.ToString());
            machineState.Add("PC",PC.ToString());

            return machineState;
        }

        public override string ToString() {
            return "A "+A.ToString()+"\n"
                +"B "+B.ToString()+"\n"
                +"D "+D.ToString()+"\n"
                +"X "+IX.ToString()+"\n"
                +"Y "+IX.ToString()+"\n"
                +"CCR "+CCR.ToString()+"\n"
                +"SP "+SP.ToString()+"\n"
                +"PC "+PC.ToString()
            ;
        }

        public override Collection<Register> GetAccumulatorStack() {
            return AccumulatorStack.ToList();
        }

        public override Collection<Register> GetCallStack() {
            return CallStack.ToList();
        }

        public override int GetCurrentProgramCounterAddress() {
            return PC.ToInteger();
        }

        public override RAM VirtualRAM {
            get {
                return this.RAM;
            }
        }

        public override void SetMemoryDisp(ref MemoryDebugger memoryDebugger) {
            memoryDebugger.SetRAM(ref this.RAM);
        }

        public override void setStartAddress(Register startAddress) {
            PC=new ProgramCounter();
            PC.programCounterUpdatedEventHandler+=new ProgramCounter.programCounterUpdatedEventDelegate(PC_programCounterUpdatedEventHandler);
            //PC.Bits=startAddress.Bits;
            
            this.entryPointAddress=startAddress.getInt();
            PC.setEntryPoint(this.entryPointAddress);

            if(programCounterUpdatedEventHandler!=null) {
                programCounterUpdatedEventHandler(PC.Bits); // Send notification of change of Program Counter to user interface
            }

            //this method body should do what ORG_IMM() does
        }

        public override void ClearStacks() {
            AccumulatorStack.cls();
            CallStack.cls();
        }

        public override void ClearRegisterMemory() {
            A.Clear();
            B.Clear();
            D.Clear();
            IX.Clear();
            IY.Clear();
            PC.Clear();
            SP.Clear();
            CCR.Clear();
            PC.Address=0;
        }

        #region Registers
        // These are the registers the instruction set can access:
        public Accumulator8 A;
        public Accumulator8 B;
        public Accumulator16 D;
        public IndexRegister IX;
        public IndexRegister IY;
        public ProgramCounter PC;
        public StackPointer SP;
        public ConditionCodeRegister CCR;
        #endregion Registers

        #region Variables
        private Ref<Accumulator16> dReference; // Holds a reference to Accumulator D (So that D can be accessed from a different context just by its reference)
        private Ref<Accumulator8> aReference;
        private Ref<Accumulator8> bReference;
        private bool incrementThePC;

        /// <summary>
        /// This is a memory structure that the instruction set can access
        /// </summary>
        public RAM RAM;

        /// <summary>
        /// This is a data structure that the instruction set can access for accumulator operations
        /// </summary>
        public Stack<Register> AccumulatorStack;

        /// <summary>
        /// This is a data structure that the instruction set can access for subroutine calls.
        /// It is otherwise known as the Stack Register
        /// </summary>
        public Stack<Register> CallStack;

        /// <summary>
        /// This is the main entry point that the CPU starts executing instructions
        /// </summary>
        public int entryPointAddress;

        #endregion Variables

        private void incPC() {
            if(incrementThePC) { // Increment the PC only if the operation was not a Branch or Branch to Subroutine operation
                 // TODO get the current operation by looking it up from the current PC value
                //PC.Address+=1; // Old and crude way to increment the PC

                if(CurrentOperation==null) {
                    PC.Address++; // Unit testing
                }
                else {
                    PC.Address+=CurrentOperation.addressSpace; // Increment the Program Counter register relative to how many bytes the current operation consumes
                    // get the current instruction address and add 1 to it
                    // (current operation address should variate in proportion to the addressing mode and number of operands)
                }
            }
        }

        #endregion Micro Processing Unit

        #region Operand value fetchers
        private Int16 getIMMOperand(Int16 operand) {
            return operand;
        }
        private byte getIMMOperand(byte operand) {
            return operand;
        }

        private byte getDIROperand(byte address) {
            // Retrieve the operand from memory:
            return RAM.get((Int16)address);
        }

        private byte getEXTOperand(Int16 address) {
            // Retrieve the operand from memory:
            return RAM.get(address);
        }

        private byte getINDXOperand(byte offset) {
            bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            return ALU.convertBitsToByte(ALU.add(IX.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));
        }

        private byte getINDYOperand(byte offset) {
            bool carry,negitive,overflow,halfCarry;
            // Add offset to Y register:
            return ALU.convertBitsToByte(ALU.add(IY.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));
        }
        #endregion

        #region Assembler Directives
        // Yes, even Assembler directives have to be defined here and need to follow method naming syntax

        [OpMetadata()]
        public void ORG_IMM(int entryPointAddress) { // Keep this for Unit testing purposes (old parser invokes this directive as an operation)
            this.entryPointAddress=entryPointAddress;
            PC.setEntryPoint(entryPointAddress);
        }

        [OpMetadata()]
        public void End() { // Assembler directive
            endExecutionInterrupt=true;
        }

        #endregion

        #region LDAA

        /// <summary>
        /// Immediatly load accumulator A with a value
        /// </summary>
        /// <param name="value"></param>
        [OpMetadata(opCode:0x86,clockCycles:2,opCodeSize:1,operandSize:1,
            Description="Immediatly load accumulator A with a value")]
        public void LDAA_IMM(byte value) { // Implemented
            byte operand=getIMMOperand(value);

            Register.load(A,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(A);
            CCR.Z=(value==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Retrieves a byte from memory to load into accumulator A
        /// </summary>
        [OpMetadata(opCode:0xb6,clockCycles:4,opCodeSize:1,operandSize:2,
            Description="Retrieves a byte from memory to load into accumulator A")]
        public void LDAA_EXT(Int16 value) { // Implemented
            // Retrieve the operand from memory:
            byte operand=getEXTOperand(value);

            Register.load(A,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(A);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator A from memory location 0x00XX where XX is value
        /// </summary>
        [OpMetadata(opCode:0x96,clockCycles:3,opCodeSize:1,operandSize:1,
            Description="Loads accumulator A from memory location 0x00XX where XX is value")]
        public void LDAA_DIR(byte value) {
            byte operand=getDIROperand(value);

            Register.load(A,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(A);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator A from an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X
        /// </param>
        [OpMetadata(opCode :0xa6,clockCycles :4,opCodeSize :1,operandSize :1,
            Description="Loads accumulator A from an indexed memory location (offset+X)")]
        public void LDAA_INDX(byte offset) { // Implemented
            //bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte operand=getINDXOperand(offset);

            Register.load(A,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(A);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator A from an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y
        /// </param>
        [OpMetadata(opCode :0x18a6,clockCycles :5,opCodeSize :2,operandSize :1,
            Description="Loads accumulator A from an indexed memory location (offset+Y)")]
        public void LDAA_INDY(byte offset) { // Implemented
            //bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte operand=getINDYOperand(offset);

            Register.load(A,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(A);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region LDAB

        /// <summary>
        /// Immediatly load accumulator B with a value
        /// </summary>
        /// <param name="value"></param>
        [OpMetadata(opCode:0xc6,clockCycles:2,opCodeSize:1,operandSize:1,
            Description="Immediatly load accumulator B with a value")]
        public void LDAB_IMM(byte value) {
            byte operand=getIMMOperand(value);

            Register.load(B,operand);
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(B);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Retrieves a byte from memory to load into accumulator B
        /// </summary>
        [OpMetadata(opCode:0xf6,clockCycles:4,opCodeSize:1,operandSize:2,
            Description="Retrieves a byte from memory to load into accumulator B")]
        public void LDAB_EXT(Int16 value) { // Implemented
            // Retrieve the operand from memory:
            byte operand=getEXTOperand(value);

            Register.load(B,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(B);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator B from memory location 0x00XX where XX is value
        /// </summary>
        [OpMetadata(opCode:0xd6,clockCycles:3,opCodeSize:1,operandSize:1,
            Description="Loads accumulator B from memory location 0x00XX where XX is value")]
        public void LDAB_DIR(byte value) { // Implemented
            // Retrieve the operand from memory:
            byte operand=getDIROperand(value);

            Register.load(B,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(B);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator B from an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X
        /// </param>
        [OpMetadata(opCode:0xe6,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Loads accumulator B from an indexed memory location (offset+X)")]
        public void LDAB_INDX(byte offset) { // Implemented
            //bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte operand=getINDXOperand(offset); //ALU.convertBitsToByte(ALU.add(IX.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));

            Register.load(B,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(B);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator B from an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y
        /// </param>
        [OpMetadata(opCode:0x18e6,clockCycles:5,opCodeSize:2,operandSize :1,
            Description="Loads accumulator B from an indexed memory location (offset+Y)")]
        public void LDAB_INDY(byte offset) { // Implemented
            //bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte operand=getINDYOperand(offset);//ALU.convertBitsToByte(ALU.add(IY.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));

            Register.load(B,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(B);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region LDD
        /// <summary>
        /// Immediatly load accumulator D with a value
        /// </summary>
        /// <param name="value"></param>
        [OpMetadata(opCode :0xcc,clockCycles:3,opCodeSize:1,operandSize:1,
            Description="Immediatly load accumulator D with a value")]
        public void LDD_IMM(byte value) {
            byte operand=getIMMOperand(value);
            Register.load(D,operand);
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(D);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Retrieves a byte from memory to load into accumulator D
        /// </summary>
        [OpMetadata(opCode :0xfc,clockCycles:5,opCodeSize:1,operandSize:2,
            Description="Retrieves a byte from memory to load into accumulator D")]
        public void LDD_EXT(Int16 value) {
            byte operand=getEXTOperand(value);
            Register.load(D,operand);
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(D);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator D from memory location 0x00XX where XX is value
        /// </summary>
        [OpMetadata(opCode :0xdc,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Loads accumulator D from memory location 0x00XX where XX is value")]
        public void LDD_DIR(byte value) {
            byte operand=getDIROperand(value);
            Register.load(D,operand);
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(D);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator D from an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X
        /// </param>
        [OpMetadata(opCode :0xec,clockCycles:5,opCodeSize:1,operandSize:1,
            Description="Loads accumulator D from an indexed memory location (offset+X)")]
        public void LDD_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            Register.load(D,operand);
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(D);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads accumulator D from an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y
        /// </param>
        [OpMetadata(opCode :0x18ec,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Loads accumulator D from an indexed memory location (offset+Y)")]
        public void LDD_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            Register.load(D,operand);
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(D);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }
        #endregion

        #region LDS

        [OpMetadata(opCode:0x8e,clockCycles:3,opCodeSize:1,operandSize:2,
            Description="Immediatly load Stack Pointer with a value")]
        public void LDS_IMM(Int16 operand) {
            // Assign the new stack pointer:
            SP.Address=getIMMOperand(operand);
            
            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Retrieves a byte from memory to load into the stack pointer
        /// </summary>
        [OpMetadata(opCode:0x9e,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Retrieves a byte from memory to load into the stack pointer")]
        public void LDS_DIR(byte address) {
            byte operand=getDIROperand(address);

            // Assign the new stack pointer:
            SP.Address=operand;

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        [OpMetadata(opCode:0xbe,clockCycles:5,opCodeSize:1,operandSize:2,
            Description="Load Stack Pointer with a value from a memory location")]
        public void LDS_EXT(Int16 address) {
            byte operand=getEXTOperand(address);

            // Assign the new stack pointer:
            SP.Address=operand;

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }
        
        /// <summary>
        /// Loads the stack pointer from an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X
        /// </param>
        [OpMetadata(opCode:0xae,clockCycles:5,opCodeSize:1,operandSize:1,
            Description="Loads the stack pointer from an indexed memory location (offset+X)")]
        public void LDS_INDX(byte offset) {
            byte operand=getINDXOperand(offset);

            // Assign the new stack pointer:
            SP.Address=operand;

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Loads the stack pointer from an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y
        /// </param>
        [OpMetadata(opCode:0x18ae,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Loads the stack pointer from an indexed memory location (offset+Y)")]
        public void LDS_INDY(byte offset){
            byte operand=getINDYOperand(offset);

            // Assign the new stack pointer:
            SP.Address=operand;

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }
        #endregion

        #region LDX

        /// <summary>
        /// Loads Index Register X with the value of the operand.
        /// </summary>
        /// <param name="operand"></param>
        [OpMetadata(opCode:0xce,clockCycles:3,opCodeSize:1,operandSize:2,
            Description="Loads Index Register X with the value of the operand.")]
        public void LDX_IMM(Int16 operand) {
            IX.Bits=ALU.convertAnythingToBits(operand) ;// might not update UI??

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Load Index Register X from a memory location specified by it's address.
        /// The address is an absolute address, a 16 bit value, with the high nibble set to zeros.
        /// e.g. 00CE
        /// </summary>
        [OpMetadata(opCode:0xde,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Load Index Register X from a memory location specified by it's address.")]
        public void LDX_DIR(byte address) {
            // Retrieve the operand from memory:
            byte operand=RAM.get(address);

            Register.load(IX,operand);

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Load Index Register X from a memory location specified by it's address.
        /// The address is an absolute address, a 16 bit value
        /// </summary>
        [OpMetadata(opCode:0xfe,clockCycles:5,opCodeSize:1,operandSize:2,
            Description="Load Index Register X from a memory location specified by it's address.")]
        public void LDX_EXT(Int16 address) {
            // Retrieve the operand from memory:
            byte operand=RAM.get(address);

            Register.load(IX,operand);

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Loads Index Register X 
        /// </summary>
        [OpMetadata(opCode:0xee,clockCycles:5,opCodeSize:1,operandSize:1,
            Description="Loads Index Register X ")]
        public void LDX_INDX(byte offset) {
            bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte address=ALU.convertBitsToByte(ALU.add(IX.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));
            byte operand=RAM.get(address);

            Register.load(IX,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads Index Register Y
        /// </summary>
        [OpMetadata(opCode:0xcdee,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Loads Index Register X")]
        public void LDX_INDY(byte offset) {
            bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte address=ALU.convertBitsToByte(ALU.add(IX.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));
            byte operand=RAM.get(address);

            Register.load(IX,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region LDY

        /// <summary>
        /// Loads Index Register Y with the value of the operand.
        /// </summary>
        /// <param name="operand"></param>
        [OpMetadata(opCode:0x18ce,clockCycles:4,opCodeSize:2,operandSize:2,
            Description="Loads Index Register Y with the value of the operand.")]
        public void LDY_IMM(Int16 operand) {
            IY.Bits=ALU.convertAnythingToBits(operand);

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Load Index Register Y from a memory location specified by it's address.
        /// The address is an absolute address, a 16 bit value, with the high nibble set to zeros.
        /// e.g. 00CE
        /// </summary>
        [OpMetadata(opCode:0x18de,clockCycles:5,opCodeSize:2,operandSize:1,
            Description="Load Index Register Y from a memory location specified by it's address.")]
        public void LDY_DIR(byte address) {
            // Retrieve the operand from memory:
            byte operand=RAM.get(address);

            Register.load(IY,operand);

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Load Index Register Y from a memory location specified by it's address.
        /// The address is an absolute address, a 16 bit value
        /// </summary>
        [OpMetadata(opCode:0x18fe,clockCycles:6,opCodeSize:2,operandSize:2,
            Description="Load Index Register Y from a memory location specified by it's address.")]
        public void LDY_EXT(Int16 address) {
            // Retrieve the operand from memory:
            byte operand=RAM.get(address);

            Register.load(IY,operand);

            // Update the CCR:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=operand==0x00;
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Loads Index Register Y 
        /// </summary>
        [OpMetadata(opCode:0x1aee,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Loads Index Register Y")]
        public void LDY_INDX(byte offset) {
            bool carry,negitive,overflow,halfCarry;
            // Add offset to Y register:
            byte address=ALU.convertBitsToByte(ALU.add(IY.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));
            byte operand=RAM.get(address);

            Register.load(IY,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Loads Index Register Y
        /// </summary>
        [OpMetadata(opCode:0x18ee,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Loads Index Register Y")]
        public void LDY_INDY(byte offset) {
            bool carry,negitive,overflow,halfCarry;
            // Add offset to X register:
            byte address=ALU.convertBitsToByte(ALU.add(IY.Bits,ALU.convertAnythingToBits(offset),out overflow,out negitive,out carry,out halfCarry));
            byte operand=RAM.get(address);

            Register.load(IY,operand);
            // Need to update CCR flags register:
            // The following flags change in respect to the data
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=(operand==0x00); // If value is zero
            CCR.V=false; // Nothing can overflow

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region STA
        /// <summary>
        /// Stores accumulator A in memory(RAM) given its new address
        /// </summary>
        /// <param name="address">
        /// The Memory Location to store the current value of accumulator A
        /// </param>
        [OpMetadata(opCode :0xb7,clockCycles :4,opCodeSize :1,operandSize :2,
            Description="Stores accumulator A in memory(RAM) given its new address")]
        public void STAA_EXT(Int16 address) {
            byte registerData=ALU.convertBitsToByte(A.Bits); // Get the data of the Accumulator A register
            RAM.set(address,registerData);                   // Store accumulator A at a memory location

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(A.Bits);
            CCR.Z=ALU.isZero(A);
            CCR.V=false;

            incPC();
            
        }

        /// <summary>
        /// Stores accumulator A in memory(RAM) given its new address
        /// </summary>
        /// <param name="addressL">
        /// The Memory Location to store the current value of accumulator A
        /// </param>
        [OpMetadata(opCode :0x97,clockCycles :3,opCodeSize :1,operandSize :1,
            Description="Stores accumulator A in memory(RAM) given its new address")]
        public void STAA_DIR(byte addressL) {
            byte registerData=ALU.convertBitsToByte(A.Bits); // Get the data of the Accumulator A register
            RAM.set(addressL,registerData);                   // Store accumulator A at a memory location

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(A.Bits);
            CCR.Z=ALU.isZero(A);
            CCR.V=false;

            incPC();
            
        }

        /// <summary>
        /// Stores accumulator A in an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X which will be used as the memory location, an address to store Acc A
        /// </param>
        [OpMetadata(opCode :0xa7,clockCycles :4,opCodeSize :1,operandSize :1,
            Description="Stores accumulator A in an indexed memory location (offset+X)")]
        public void STAA_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            byte registerData=ALU.convertBitsToByte(A.Bits);
            RAM.set(operand,registerData);

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(A.Bits);
            CCR.Z=ALU.isZero(A);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores accumulator A in an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y which will be used as the memory location, an address to store Acc A
        /// </param>
        [OpMetadata(opCode :0x18a7,clockCycles :5,opCodeSize :2,operandSize :1,
            Description="Stores accumulator A in an indexed memory location (offset+Y)")]
        public void STAA_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            byte registerData=ALU.convertBitsToByte(A.Bits);
            RAM.set(operand,registerData);

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(A.Bits);
            CCR.Z=ALU.isZero(A);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores accumulator B in memory(RAM) given its new address
        /// </summary>
        /// <param name="address">
        /// The Memory Location to store the current value of accumulator B
        /// </param>
        [OpMetadata(opCode :0xf7,clockCycles :4,opCodeSize :1,operandSize :2,
            Description="Stores accumulator B in memory(RAM) given its new address")]
        public void STAB_EXT(Int16 address) {
            byte registerData=ALU.convertBitsToByte(B.Bits); // Get the data of the Accumulator B register
            RAM.set(address,registerData);                   // Store accumulator B at a memory location

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(B.Bits);
            CCR.Z=ALU.isZero(B);
            CCR.V=false;

            incPC();
            
        }

        /// <summary>
        /// Stores accumulator B in memory(RAM) given its new address
        /// </summary>
        /// <param name="addressL">
        /// The Memory Location to store the current value of accumulator B
        /// </param>
        [OpMetadata(opCode :0xd7,clockCycles :3,opCodeSize :1,operandSize :1,
            Description="Stores accumulator B in memory(RAM) given its new address")]
        public void STAB_DIR(byte addressL) {
            byte registerData=ALU.convertBitsToByte(B.Bits); // Get the data of the Accumulator B register
            RAM.set(addressL,registerData);                   // Store accumulator B at a memory location

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(B.Bits);
            CCR.Z=ALU.isZero(B);
            CCR.V=false;

            incPC();
            
        }

        /// <summary>
        /// Stores accumulator B in an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X which will be used as the memory location, an address to store Acc B
        /// </param>
        [OpMetadata(opCode :0xe7,clockCycles :4,opCodeSize :1,operandSize :1,
            Description="Stores accumulator B in an indexed memory location (offset+X)")]
        public void STAB_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            byte registerData=ALU.convertBitsToByte(B.Bits);
            RAM.set(operand,registerData);

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(B.Bits);
            CCR.Z=ALU.isZero(B);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores accumulator B in an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y which will be used as the memory location, an address to store Acc B
        /// </param>
        [OpMetadata(opCode :0x18e7,clockCycles :5,opCodeSize :2,operandSize :1,
            Description="Stores accumulator B in an indexed memory location (offset+Y)")]
        public void STAB_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            byte registerData=ALU.convertBitsToByte(B.Bits);
            RAM.set(operand,registerData);

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(B.Bits);
            CCR.Z=ALU.isZero(B);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region STD

        /// <summary>
        /// Stores accumulator D in memory(RAM) given its new address
        /// </summary>
        /// <param name="addressL">
        /// The Memory Location to store the current value of accumulator D
        /// </param>
        [OpMetadata(opCode:0xdd,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Stores accumulator D in memory(RAM) given its new address")]
        public void STD_DIR(byte addressL) {
            byte registerData=ALU.convertBitsToByte(D.Bits); 
            RAM.set(addressL,registerData);                  

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(D.Bits);
            CCR.Z=ALU.isZero(D);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores accumulator D in memory(RAM) given its new address
        /// </summary>
        /// <param name="address">
        /// The Memory Location to store the current value of accumulator D
        /// </param>
        [OpMetadata(opCode:0xfd,clockCycles:5,opCodeSize:1,operandSize :2,
            Description="Stores accumulator D in memory(RAM) given its new address")]
        public void STD_EXT(Int16 address) {
            byte registerData=ALU.convertBitsToByte(D.Bits);
            RAM.set(address,registerData);                  

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(D.Bits);
            CCR.Z=ALU.isZero(D);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores accumulator D in an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X which will be used as the memory location, an address to store Acc D
        /// </param>
        [OpMetadata(opCode:0xed,clockCycles:5,opCodeSize:1,operandSize:1,
            Description="Stores accumulator D in an indexed memory location (offset+X)")]
        public void STD_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            byte registerData=ALU.convertBitsToByte(D.Bits);
            RAM.set(operand,registerData);

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(D.Bits);
            CCR.Z=ALU.isZero(D);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores accumulator D in an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y which will be used as the memory location, an address to store Acc D
        /// </param>
        [OpMetadata(opCode:0x18ed,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Stores accumulator D in an indexed memory location (offset+Y)")]
        public void STD_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            byte registerData=ALU.convertBitsToByte(D.Bits);
            RAM.set(operand,registerData);

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(D.Bits);
            CCR.Z=ALU.isZero(D);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region STS

        /// <summary>
        /// Stores stack pointer in memory(RAM) given its new address
        /// </summary>
        /// <param name="addressL">
        /// The Memory Location to store the current value of stack pointer
        /// </param>
        [OpMetadata(opCode:0x9f,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Stores stack pointer in memory(RAM) given its new address")]
        public void STS_DIR(byte addressL) {
            RAM.set(addressL,SP.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=ALU.isZero(SP);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores stack pointer in memory(RAM) given its new address
        /// </summary>
        /// <param name="address">
        /// The Memory Location to store the current value of stack pointer
        /// </param>
        [OpMetadata(opCode:0xbf,clockCycles:5,opCodeSize:1,operandSize:2,
            Description="Stores stack pointer in memory(RAM) given its new address")]
        public void STS_EXT(Int16 address) {
            RAM.set(address,SP.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=ALU.isZero(SP);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores stack pointer in an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X which will be used as the memory location, an address to store stack pointer
        /// </param>
        [OpMetadata(opCode:0xaf,clockCycles:5,opCodeSize:1,operandSize:1,
            Description="Stores stack pointer in an indexed memory location (offset+X)")]
        public void STS_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            RAM.set(operand,SP.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=ALU.isZero(SP);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores stack pointer in an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y which will be used as the memory location, an address to store stack pointer
        /// </param>
        [OpMetadata(opCode:0x18af,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Stores stack pointer in an indexed memory location (offset+Y)")]
        public void STS_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            RAM.set(operand,SP.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(SP);
            CCR.Z=ALU.isZero(SP);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region STX

        /// <summary>
        /// Stores index register x in memory(RAM) given its new address
        /// </summary>
        /// <param name="addressL">
        /// The Memory Location to store the current value of index register x
        /// </param>
        [OpMetadata(opCode:0xdf,clockCycles:4,opCodeSize:1,operandSize:1,
            Description="Stores index register x in memory(RAM) given its new address")]
        public void STX_DIR(byte addressL) {
            RAM.set(addressL,IX.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=ALU.isZero(IX);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores index register x in memory(RAM) given its new address
        /// </summary>
        /// <param name="address">
        /// The Memory Location to store the current value of index register x
        /// </param>
        [OpMetadata(opCode:0xff,clockCycles:5,opCodeSize:1,operandSize:2,
            Description="Stores index register x in memory(RAM) given its new address")]
        public void STX_EXT(Int16 address) {
            RAM.set(address,IX.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=ALU.isZero(IX);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores index register x in an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X which will be used as the memory location, an address to store index register x
        /// </param>
        [OpMetadata(opCode:0xef,clockCycles:5,opCodeSize:1,operandSize:1,
            Description="Stores index register x in an indexed memory location (offset+X)")]
        public void STX_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            RAM.set(operand,IX.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=ALU.isZero(IX);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores index register x in an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y which will be used as the memory location, an address to store index register x
        /// </param>
        [OpMetadata(opCode:0xcdef,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Stores index register x in an indexed memory location (offset+Y)")]
        public void STX_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            RAM.set(operand,IX.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IX);
            CCR.Z=ALU.isZero(IX);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region STY

        /// <summary>
        /// Stores index register y in memory(RAM) given its new address
        /// </summary>
        /// <param name="addressL">
        /// The Memory Location to store the current value of index register y
        /// </param>
        [OpMetadata(opCode:0x18df,clockCycles:5,opCodeSize:2,operandSize:1,
            Description="Stores index register y in memory(RAM) given its new address")]
        public void STY_DIR(byte addressL) {
            RAM.set(addressL,IY.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=ALU.isZero(IY);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores index register y in memory(RAM) given its new address
        /// </summary>
        /// <param name="address">
        /// The Memory Location to store the current value of index register y
        /// </param>
        [OpMetadata(opCode:0x18ff,clockCycles:6,opCodeSize:2,operandSize:2,
            Description="Stores index register y in memory(RAM) given its new address")]
        public void STY_EXT(Int16 address) {
            RAM.set(address,IY.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=ALU.isZero(IY);
            CCR.V=false;

            incPC();
        }

        /// <summary>
        /// Stores index register y in an indexed memory location (offset+X)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register X which will be used as the memory location, an address to store index register y
        /// </param>
        [OpMetadata(opCode:0x1aef,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Stores index register y in an indexed memory location (offset+X)")]
        public void STY_INDX(byte offset) {
            byte operand=getINDXOperand(offset);
            RAM.set(operand,IY.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=ALU.isZero(IY);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        /// <summary>
        /// Stores index register y in an indexed memory location (offset+Y)
        /// </summary>
        /// <param name="offset">
        /// The offset byte to add to index register Y which will be used as the memory location, an address to store index register y
        /// </param>
        [OpMetadata(opCode:0x18ef,clockCycles:6,opCodeSize:2,operandSize:1,
            Description="Stores index register y in an indexed memory location (offset+Y)")]
        public void STY_INDY(byte offset) {
            byte operand=getINDYOperand(offset);
            RAM.set(operand,IY.ToByte());

            // Update CCR flags register:
            CCR.N=ALU.isNegativeNumber(IY);
            CCR.Z=ALU.isZero(IY);
            CCR.V=false;

            // Need to increment the PC
            incPC();
        }

        #endregion

        #region Math operations
        /// <summary>
        /// Adds accumulator B to A, storing the result in accumulator A
        /// </summary>
        [OpMetadata(opCode :0x1b,clockCycles :2,opCodeSize :1,operandSize :0,
            Description="Adds accumulator B to A, storing the result in accumulator A")]
        public void ABA_INHR() { // Implemented
            bool carry,negitive,overflow,halfCarry;
            A.Bits=ALU.add(A.Bits,B.Bits,out overflow,out negitive,out carry,out halfCarry);

            CCR.V=overflow;
            CCR.C=carry;
            CCR.H=halfCarry;
            CCR.Z=ALU.isZero(A);
            CCR.N=negitive;

            incPC();
        }
        #endregion

        #region Stack Operations

        /// <summary>
        /// Pushes accumulator A on to the stack
        /// (accumulator A is not changed)
        /// </summary>
        [OpMetadata(opCode :0x36,clockCycles:3,opCodeSize:1,operandSize:0,
            Description="Pushes accumulator A on to the stack")]
        public void PSHA_INHR() { // Implemented
            Register8 acc=ALU.cloneAccumulatorRegister(A);
            AccumulatorStack.push(acc);

            SP.Address++; // Increment the stack pointer

            incPC();
        }

        /// <summary>
        /// Pushes accumulator B on to the stack
        /// (accumulator B is not changed)
        /// </summary>
        [OpMetadata(opCode:0x37,clockCycles:3,opCodeSize:1,operandSize:0,
            Description="Pushes accumulator B on to the stack")]
        public void PSHB_INHR() { // Implemented
            Accumulator8 acc=ALU.cloneAccumulatorRegister(B);
            AccumulatorStack.push(acc);

            SP.Address++; // Increment the stack pointer

            incPC();
        }

        /// <summary>
        /// Pulls the current value from the stack to accumulator A
        /// (accumulator A is changed)
        /// </summary>
        [OpMetadata(opCode:0x32,clockCycles:4,opCodeSize:1,operandSize:0,
            Description="Pulls the current value from the stack to accumulator A")]
        public void PULA_INHR() { // Implemented
            StackItem<Register> R;
            // Replacing an accumulator instance introduces the bug of not updating the D accumulator,
            // So instead of replacing an accumulator instance, just update it!
            R=AccumulatorStack.pull();
            Accumulator8 acc=R.data as Accumulator8; // Force it to be an 8 bit accumulator (A or B) even if it isn't
            A.Bits=acc.Bits; // Update the current accumulator in memory with accumulator found on stack

            //if(SP.Address>0) {
            SP.Address--; // decrement the stack pointer if we can, since there is 1 less item on the stack
            //}

            incPC();
        }

        /// <summary>
        /// Pulls the current value from the stack to accumulator B
        /// (accumulator B is changed)
        /// </summary>
        [OpMetadata(opCode:0x33,clockCycles:4,opCodeSize:1,operandSize:0,
            Description="Pulls the current value from the stack to accumulator B")]
        public void PULB_INHR() { // Implemented
            StackItem<Register> R;
            // Replacing an accumulator instance introduces the bug of not updating the D accumulator,
            // So instead of replacing an accumulator instance, just update it!
            R=AccumulatorStack.pull();
            Accumulator8 acc=R.data as Accumulator8; // Force it to be an 8 bit accumulator (A or B) even if it isn't
            B.Bits=acc.Bits; // Update the current accumulator in memory with accumulator found on stack

            //if(SP.Address>0) {
            SP.Address--; // decrement the stack pointer if we can, since there is 1 less item on the stack
            //}

            incPC();
        }

        #endregion Stack Operations

        #region Branching operations

        // Not implemented
        //public void JMP(AddressMode addressMode,byte offset) {


        //    // No change in CCR
        //    incPC();
        //}

        [OpMetadata(opCode:0x7e,clockCycles:3,opCodeSize:1,operandSize:2,
            Description="Jumps to an extended address")]
        public void JMP_EXT(int absoluteAddress) { // (Int16 relativeAddress) {
            //bool[] entryBits=ALU.convertAnythingToBits(entryPointAddress);
            //byte entryPointHighNibble=ALU.convertBitsToByte(ALU.getHighNibble(entryBits,16));
            //string h=ALU.convertAnythingToHex(relativeAddress);
            //h=h.Length==1?"0"+h:h;
            //string h0x=ALU.convertAnythingToHex(entryPointHighNibble)+h;
            //int absoluteAddress=ALU.convertHexToInt(h0x);

            // Find the label by its  associated address:
            PC.Address=absoluteAddress; // ..And make the PC jump to that address

            // No change in CCR
            // Dont increment the PC (we have just made the PC jump)
        }

        //[OperationMD2()]
        public void JMP(string label) {
            // Find the label's  associated address:
            foreach(BranchingLabel lbl in branchingLabels) {
                if(lbl.label==label) {
                    PC.Address=lbl.address; // ..And make the PC jump to that address
                    break;
                }
            }

            // No change in CCR
            // Dont increment the PC (we have just made the PC jump)
        }

        public System.Collections.ObjectModel.Collection<BranchingLabel> branchingLabels;

        public class BranchingLabel {
            public BranchingLabel(string lbl) {
                this.label=lbl;
            }
            public BranchingLabel(int addr) {
                this.address=addr;
            }
            public BranchingLabel(string lbl,int addr) {
                this.label=lbl;
                this.address=addr;
            }

            public string label;
            public int address;
        }

        public string LABEL {
            set {
                BranchingLabel i=new BranchingLabel(value,PC.Address);
                if(!branchingLabels.Contains(i)) {
                    branchingLabels.Add(i);
                }
            }
        }

        #endregion Branching operations

        #region Subroutine branching

        //public void BSR_REL(string label) {
        //    int addr;

        //    foreach(BranchingLabel lbl in this.branchingLabels) {
        //        if(lbl.label==label) {
        //            addr=lbl.address;
        //            break;
        //        }
        //        else {
        //            throw new Exception("Label passes to BSR_REL could not be located in code");
        //        }
        //    }
    
        //    //BSR_REL(addr);
        //}

        [OpMetadata(opCode:0x8d,clockCycles:6,opCodeSize:1,operandSize:1,
            Description="Branches to a sub routine")]
        public void BSR_REL(byte offset) { // Implemented
            // Advance the PC to the return address
            PC.Bits=ALU.add(PC.Bits,new bool[] { true, false }); 

            // Push the return address for the PC to the Call Stack
            Register16 reg=new Register16();
            reg.Bits=PC.Bits;
            CallStack.push(reg); // originally was: CallStack.push(PC.Address)
            
            // Decrement the SP by 1
            SP.Address--;

            // Calculate operation address from just the offset byte
            byte before=offset;
            int rel=offset;
            if(offset>127) {
                // The offset value is negitive so..
                // Lets "reverse 2's complement" it:
                bool[] offsetBits=ALU.convertAnythingToBits(offset);
                bool[] one=ALU.convertAnythingToBits(1);
                ALU.negate(ref offsetBits); // Negate the offset
                offsetBits=ALU.add(offsetBits,one); // Subtract one from the offset (should bring the offset back to 1's complement)
                offset=ALU.convertBitsToByte(offsetBits);
                rel=-1*offset;
            }

            // Add the offset to the return address to find the operation address to branch to:
            PC.Address=PC.Address+rel; 
        }

        [OpMetadata(opCode :0x39,clockCycles:5,opCodeSize:1,operandSize:0,
            Description="returns to the last subroutine")]
        public void RTS_INHR() { // Implemented
            // Increment the Stack Pointer by 1:
            SP.Address++;

            // Pull the last subroutine return address from the Call Stack:
            StackItem<Register> i=CallStack.pull();

            // Advance the program counter to the return address:
            if(i!=null) {
                //PC.Bits=i.data.Bits;
                PC.Address=ALU.convertAnythingToInt(i.data.Bits);
                //PC.Address=i.data;
            }
            else {
                incPC();
            }

            // No need to update the CCR
        }

        #endregion

        #region Interrupts

        /* Dont forget to implement SEI & CLI */

        [OpMetadata(opCode:0x3f,clockCycles:14,opCodeSize:1,operandSize:0,
            Description="Software Interrupt")]
        public void SWI_INHR() { /* not tested */
            PC.Bits=ALU.convertAnythingToBits(ALU.convertAnythingToInt(PC.Bits)+1); // Increments the PC without raising its event

            // Clone the PC:
            Register16 _PC=new Register16();_PC.Bits=PC.Bits;

            AccumulatorStack.push(_PC);
            SP.Address++;
            AccumulatorStack.push(ALU.cloneRegister(IY));
            SP.Address++;
            AccumulatorStack.push(ALU.cloneRegister(IX));
            SP.Address++;
            AccumulatorStack.push(ALU.cloneRegister(A));
            SP.Address++;
            AccumulatorStack.push(ALU.cloneRegister(B));
            SP.Address++;
            AccumulatorStack.push(ALU.cloneRegister(CCR));
            SP.Address++;

            CCR.I=true;

            // Load the PC with the address stored in the SWI vector:
            Int16 SWIVector=RAM.get16(0xfff6);

            PC.Address=SWIVector;
        }

        [OpMetadata(opCode:0x3b,clockCycles:12,opCodeSize:1,operandSize:0,
            Description="Return from interrupt")]
        public void RTI_INHR() { /* not tested */
            CCR=(ConditionCodeRegister)AccumulatorStack.pull().data;
            SP.Address--;
            B=(Accumulator8)AccumulatorStack.pull().data;
            SP.Address--;
            A=(Accumulator8)AccumulatorStack.pull().data;
            SP.Address--;
            IX=(IndexRegister)AccumulatorStack.pull().data;
            SP.Address--;
            IY=(IndexRegister)AccumulatorStack.pull().data;
            SP.Address--;

            PC.Bits=AccumulatorStack.pull().data.Bits;
            SP.Address--;

            CCR.X=false; // Is this correct?

            if(programCounterUpdatedEventHandler!=null) {
                programCounterUpdatedEventHandler(PC.Bits); // Send notification of change of Program Counter to user interface
            }
        }

        #endregion

        #region MISC operations
        [OpMetadata(opCode:0xcf,clockCycles:2,opCodeSize:1,operandSize:0,Description="Stops the execution of the processor")]
        public void STOP_INHR() {
            if(CCR.S) { // If the stop diable bit of the CCR is set
                incPC(); // Operate like the NOP operation
            }
            else {
                // Cause all system clocks to halt, thereby placing system in a minimum-power standby mode
                // Recovery from STOP may be accomplised by: RESET, XIRQ, IRQ
                // No registers or IO pins should be affected

                //TODO: implement this stuff
            }

        }

        [OpMetadata(opCode :0x01,clockCycles:2,opCodeSize:1,operandSize:0,Description="Not an operation")]
        public void NOP_INHR() { 
            incPC(); 
        }
        
        #endregion MISC operations

    }
}
