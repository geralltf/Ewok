using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;
using Ewok.Machine.Debug;
using Ewok.M68HC11;

namespace Ewok.Lang.Test {

    /// <summary>
    /// Tests the instruction set of the M68HC11 Micro Processing Unit
    /// Either by directly invoking the instructions,
    /// or by using the operation Parser and Interpreter
    /// </summary>
    [TestClass]
    public class Lang_6811:MPU {

        #region Unit Test logic
        public Lang_6811() {
            //
            // TODO: Add constructor logic here
            //
            resetProcessingUnit();
        }

        private TestContext testContextInstance;

        private Interpreter interpreter;
        private Debugger debugger;
        private ProcessingUnit PU;
        //private Ewok.M68HC11.Parser.Parser parser;
        private Ewok.Machine.Asm.S19.Assembler assembler;
        private Ewok.Machine.Dasm.S19.ParserS19 parser;

        private MPU M6811MPU;
        private Ref<Debugger> debuggerReference;
        private Ref<ProcessingUnit> PUReference;
        private Ref<MPU> M6811MPUReference;
        private string linesOfCode;
        private Dictionary<int,CommonOperation> ops;        

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance=value;
            }
        }

        [TestCleanup]
        public void resetProcessingUnit() {
            // Setup instruction set test grounds:
            ClearRegisterMemory();
            ClearStacks();
            PC.Address=0;

            // Setup virtual processing unit:
            M6811MPU=new MPU();
            M6811MPUReference=new Ref<MPU>(() => M6811MPU,z => { M6811MPU=z; });
            PU=M6811MPU;
            PUReference=new Ref<ProcessingUnit>(() => PU,z => { PU=z; });
            debugger=new Debugger(PUReference);
            debuggerReference=new Ref<Debugger>(() => debugger,z => { debugger=z; }); ;
            interpreter=new Interpreter(PUReference,debuggerReference);
            //parser=new Ewok.M68HC11.Parser.Parser(M6811MPUReference,debuggerReference);
            parser=new Machine.Dasm.S19.ParserS19(M6811MPUReference,debuggerReference);
            assembler=new Machine.Asm.S19.Assembler();

            //linesOfCode="";
            ops=null;
        }
        
        
        private void processTest() {
            Register startAddress;


            string s19file=assembler.assemble(linesOfCode);
            ops=parser.Parse(s19file+".s19",out startAddress);

            if(System.IO.File.Exists(s19file+".s19")) {
                System.IO.File.Delete(s19file+".s19");
                if(System.IO.File.Exists(s19file+".tmp")) {
                    System.IO.File.Delete(s19file+".tmp");
                }
            }



            // need to invoke parser on test case:
            //ops=parser.Parse(linesOfCode,out startAddress);

            // execute parsed operations on virtual processing unit (VPU)
            interpreter.ExecuteNoWait(startAddress,ops);

            // Wait until all instructions execute: TODO Fix bug in test engine - if an infinite loop occurs then after 10 seconds, terminate the thread
            interpreter.WaitUntilFinished();
        }

        #endregion Unit Test logic

        [TestMethod]
        public void TestLoadAccumulators() {
            LDAA_IMM(6);
            LDAB_IMM(6);
            
            Assert.IsTrue(A.ToBinaryString()=="00000110");
        }

        [TestMethod]
        /* Dependant on memory operations */
        public void TestLoadIndexRegisters() {
            LDAA_IMM(4);
            STAA_DIR(0x10);
            LDX_DIR(0x10);
            Assert.IsTrue(IX.ToBinaryString()=="0000000000000100"); // IX Must be equal to 4
            resetProcessingUnit();

            LDAA_IMM(5);
            STAA_EXT(0x1010);
            LDX_EXT(0x1010);
            Assert.IsTrue(IX.ToBinaryString()=="0000000000000101"); // IX Must be equal to 5
            resetProcessingUnit();

            LDX_IMM(6);
            Assert.IsTrue(IX.ToBinaryString()=="110"); // IX Must be equal to 6
            resetProcessingUnit();

            LDAA_IMM(7);
            STAA_DIR(0x0a);
            LDX_IMM(8);
            LDX_INDX(0x02); // load X from 0x08 + 0x02 = 0x0a
            Assert.IsTrue(IX.ToBinaryString()=="0000000000000111"); // IX Must be equal to 7
            resetProcessingUnit();

            LDAA_IMM(8);
            STAA_DIR(0x0c);
            LDX_IMM(9);
            LDX_INDX(0x03); // load X from 0x08 + 0x02 = 0x0a
            Assert.IsTrue(IX.ToBinaryString()=="0000000000001000"); // IX Must be equal to 8
        }

        [TestMethod]
        public void TestAddition() {
            LDAA_IMM(6);
            LDAB_IMM(7);
            ABA_INHR();
            Assert.IsTrue(A.ToBinaryString()=="00001101"); // 13
            resetProcessingUnit();

            LDAA_IMM(0xab);
            LDAB_IMM(0x45);
            ABA_INHR();
            Assert.IsTrue(A.ToBinaryString()=="11110000"); // $F0
            resetProcessingUnit();

            LDAA_IMM(0xab);
            LDAB_IMM(0x06);
            ABA_INHR();
            Assert.IsTrue(A.ToBinaryString()=="10110001"); // $B1
            resetProcessingUnit();

            LDAA_IMM(0x00);
            LDAB_IMM(0xab);
            ABA_INHR();
            Assert.IsTrue(A.ToBinaryString()=="10101011"); // $AB
        }

        [TestMethod]
        public void TestSubtraction() {
            LDAA_IMM(6);
            LDAB_IMM(7);
            A.Bits=ALU.subtract(A.Bits,B.Bits);
            Assert.IsTrue(A.ToBinaryString()=="11111111"); // 255 (-1)
            resetProcessingUnit();

            LDAA_IMM(7);
            LDAB_IMM(6);
            A.Bits=ALU.subtract(A.Bits,B.Bits);
            Assert.IsTrue(A.ToBinaryString()=="00000001"); // +1

            LDAA_IMM(17);
            LDAB_IMM(70);
            A.Bits=ALU.subtract(A.Bits,B.Bits);
            Assert.IsTrue(A.ToBinaryString()=="11001011"); // (203) -53
        }

        [TestMethod]
        public void TestNegation() {
            bool[] b1=ALU.convertAnythingToBits("11111111");
            ALU.negate(ref b1);
            Assert.IsTrue(ALU.convertBitsToString(b1)=="00000000");

            b1=ALU.convertAnythingToBits("11011110");
            ALU.negate(ref b1);
            Assert.IsTrue(ALU.convertBitsToString(b1)=="00100001");

            b1=ALU.convertAnythingToBits("011011110");
            ALU.negate(ref b1);
            Assert.IsTrue(ALU.convertBitsToString(b1)=="100100001");
        }

        [TestMethod]
        public void TestJumping() {
            PC.Address=0;
            LDAA_IMM(12);
            LDAA_IMM(12);
            LDAA_IMM(12);
            LDAA_IMM(12);
            LABEL="lbl"; LDAA_IMM(12); // PC should be here on jump
            LDAA_IMM(12);
            ABA_INHR();
            JMP("lbl");

            Assert.IsTrue(PC.Address==4);
        }

        [TestMethod]
        public void TestSubroutines() {
            string afterState;

            // Inline ASM does not work in this senario because future LABELS must be evaluated by an assembler before code is emulated
            //LDAA_IMM(0x06);
            //LDAB_IMM(0x07);
            //LDS_IMM(0x0100);
            //JMP("start");
            //LABEL="lbl";
            //ABA_INHR();
            //ABA_INHR();
            //ABA_INHR();
            //RTS_INHR();
            //LABEL="start";
            //BSR_REL("lbl");

            /**** BEGIN ASSEMBLY CODE TEST ****/
            linesOfCode=@"
ORG $C000
LDAA #$06
LDAB #$07
LDS #$0100
JMP start

lbl: ABA
ABA
ABA
RTS

start:BSR lbl
ABA
";

            processTest();
            // Check the after affects of the operations on the VPU by assertions with the available virtual Registers and stacks

            afterState=
@"A  (8 bit) => [00100010] [$22] [34]
B  (8 bit) => [00000111] [$7] [7]
D  (16 bit) => [0010001000000111] [$2207] [8711]
X  (16 bit) => [0000000000000000] [$0] [0]
Y  (16 bit) => [0000000000000000] [$0] [0]
CCR Flags => S=0 X=0 H=1 I=0 N=0 Z=0 V=1 C=0
SP  (16 bit) => [100000000] [$100] [256]
PC  (16 bit) => [1100000000010001] [$c011] [49169]";
            afterState=afterState.Replace("\r","");
            Assert.IsTrue(PU.ToString()==afterState); // Hack: not the best way to do this test: need to get at the VPU's internals

            resetProcessingUnit();
            /**** END OF ASSEMBLY CODE TEST ****/


            /**** BEGIN ASSEMBLY CODE TEST ****/
            linesOfCode=@"
ORG $C000
LDAA #$06
LDAB #$07
LDS #$0100
BSR lbl
JMP end

lbl: ABA
ABA
ABA
ABA
ABA
ABA
ABA
RTS

end:ABA
";

            processTest();

            afterState=
@"A  (8 bit) => [00111110] [$3e] [62]
B  (8 bit) => [00000111] [$7] [7]
D  (16 bit) => [0011111000000111] [$3e07] [15879]
X  (16 bit) => [0000000000000000] [$0] [0]
Y  (16 bit) => [0000000000000000] [$0] [0]
CCR Flags => S=0 X=0 H=0 I=0 N=0 Z=0 V=1 C=0
SP  (16 bit) => [100000000] [$100] [256]
PC  (16 bit) => [1100000000010101] [$c015] [49173]";
            afterState=afterState.Replace("\r","");
            Assert.IsTrue(PU.ToString()==afterState); // Hack: not the best way to do this test: need to get at the VPU's internals
            /**** END OF ASSEMBLY CODE TEST ****/
        }

        [TestMethod]
        public void TestStackOperations() {
            string afterState;

            /**** BEGIN ASSEMBLY CODE TEST ****/
            linesOfCode=@"
ORG $C000
LDAA #$06
LDAB #$07
PSHA
PSHB
LDS #$0100
BSR lbl
JMP end

lbl: ABA
ABA
ABA
RTS

end:ABA
PULB
PULA
";

            processTest();
            // Check the after affects of the operations on the VPU by assertions with the available virtual Registers and stacks

            afterState=
@"A  (8 bit) => [00000110] [$6] [6]
B  (8 bit) => [00000111] [$7] [7]
D  (16 bit) => [0000011000000111] [$607] [1543]
X  (16 bit) => [0000000000000000] [$0] [0]
Y  (16 bit) => [0000000000000000] [$0] [0]
CCR Flags => S=0 X=0 H=1 I=0 N=0 Z=0 V=1 C=0
SP  (16 bit) => [11111110] [$fe] [254]
PC  (16 bit) => [1100000000010101] [$c015] [49173]";
            afterState=afterState.Replace("\r","");
            Assert.IsTrue(PU.ToString()==afterState); // Hack: not the best way to do this test: need to get at the VPU's internals
            Assert.IsTrue(PU.GetAccumulatorStack().Count==0);
            resetProcessingUnit();

            /**** BEGIN ASSEMBLY CODE TEST ****/
            linesOfCode=@"
ORG $C000
LDAA #$06
LDAB #$07
PSHA
PSHB
LDS #$0100
BSR lbl
JMP end

lbl: ABA
ABA
ABA
RTS

end:ABA
";

            processTest();
            // Check the after affects of the operations on the VPU by assertions with the available virtual Registers and stacks

            afterState=
@"A  (8 bit) => [00100010] [$22] [34]
B  (8 bit) => [00000111] [$7] [7]
D  (16 bit) => [0010001000000111] [$2207] [8711]
X  (16 bit) => [0000000000000000] [$0] [0]
Y  (16 bit) => [0000000000000000] [$0] [0]
CCR Flags => S=0 X=0 H=1 I=0 N=0 Z=0 V=1 C=0
SP  (16 bit) => [100000000] [$100] [256]
PC  (16 bit) => [1100000000010011] [$c013] [49171]";
            afterState=afterState.Replace("\r","");
            Assert.IsTrue(PU.ToString()==afterState); // Hack: not the best way to do this test: need to get at the VPU's internals
            Assert.IsTrue(PU.GetAccumulatorStack().Count==2);
            Assert.IsTrue(PU.GetAccumulatorStack()[0].ToInteger()==6); // Acc A=6
            Assert.IsTrue(PU.GetAccumulatorStack()[1].ToInteger()==7); // Acc B=7
        }

        // TODO:
        [TestMethod]
        public void TestRAMOperations() {
            string afterState;

            /**** BEGIN ASSEMBLY CODE TEST ****/
            linesOfCode=@"
ORG $C000
LDAA #$22
STAA $200
LDAA #$33
STAA $201
LDAA #$44
STAA $2FE
LDAA #$55
STAA $2FF
LDAA #$66
STAA $300
LDAA #$77
STAA $301

LDS #$0300
LDAA $0200
LDAB $0201

BSR SSUM
STAA $202
JMP end
SSUM: ABA
RTS
end: ABA
";

            processTest();
            // Check the after affects of the operations on the VPU by assertions with the available virtual Registers and stacks

            afterState=
@"A  (8 bit) => [10001000] [$88] [136]
B  (8 bit) => [00110011] [$33] [51]
D  (16 bit) => [1000100000110011] [$8833] [34867]
X  (16 bit) => [0000000000000000] [$0] [0]
Y  (16 bit) => [0000000000000000] [$0] [0]
CCR Flags => S=0 X=0 H=1 I=0 N=1 Z=0 V=0 C=0
SP  (16 bit) => [1100000000] [$300] [768]
PC  (16 bit) => [1100000000110010] [$c032] [49202]";
            afterState=afterState.Replace("\r","");
            Assert.IsTrue(PU.ToString()==afterState); // Hack: not the best way to do this test: need to get at the VPU's internals

            // Use assertions to confirm the end state of the RAM:
            Assert.IsTrue(PU.VirtualRAM.get(0x200)==0x22);
            Assert.IsTrue(PU.VirtualRAM.get(0x201)==0x33);
            Assert.IsTrue(PU.VirtualRAM.get(0x2fe)==0x44);
            Assert.IsTrue(PU.VirtualRAM.get(0x2ff)==0x55);
            Assert.IsTrue(PU.VirtualRAM.get(0x300)==0x66);
            Assert.IsTrue(PU.VirtualRAM.get(0x301)==0x77);
        }

        [TestMethod]
        public void TestInterrupts() {
            Assert.Inconclusive("Interrupt operations not implemented at all yet");
            string afterState;

            /**** BEGIN ASSEMBLY CODE TEST ****/
            linesOfCode=@"
ORG $C000
LDAA #$22
STAA $200
LDX #$300
SWI
INX
ABA
STAA $20,X
RTI
";

            processTest();
            // Check the after affects of the operations on the VPU by assertions with the available virtual Registers and stacks

            afterState=@"";
            afterState=afterState.Replace("\r","");
            Assert.IsTrue(PU.ToString()==afterState);
        }
    }
}
