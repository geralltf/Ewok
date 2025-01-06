using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;
using Ewok.Machine.Debug;
using Ewok.Machine.Assembler;

using Ewok.Machine.Dasm.S19;

using Ewok.M68HC11;
using Ewok.M68HC11.Registers;

namespace Ewok.Machine.Emu {
    public partial class FrmEditor:Form {
        public FrmEditor() {
            InitializeComponent();

            M6811MPU=new MPU();
            M6811MPUReference=new Ref<MPU>(() => M6811MPU,z => { M6811MPU=z; });
            PU=M6811MPU;
            PUReference=new Ref<ProcessingUnit>(() => PU,z => { PU=z; });

            debugger=new Debugger(PUReference);
            debugger.breakpointTriggeredEventHandler+=new Debugger.breakpointTriggeredDelegate(CPU_breakpointTriggeredEventHandler);
            //debugger.RuntimeExceptionEventHandler+=new Debugger.RuntimeExceptionEventDelegate(debugger_RuntimeExceptionEventHandler);
            debuggerReference=new Ref<Debugger>(() => debugger,z => { debugger=z; });

            interpreter=new Interpreter(PUReference,debuggerReference);
            interpreter.programCounterUpdatedEventHandler+=new Interpreter.programCounterUpdatedEventDelegate(PC_programCounterUpdatedEventHandler);
            interpreter.ProcessingUnit.startedEventHandler+=new M68HC11.MPU.startedDelegate(CPU_startedEventHandler);
            interpreter.ProcessingUnit.finishedEventHandler+=new M68HC11.MPU.finishedDelegate(CPU_finishedEventHandler);

            interpreterReference=new Ref<Interpreter>(() => interpreter,z => { interpreter=z; });

            parser=new ParserS19(M6811MPUReference,debuggerReference);
            //parser.ParsingExceptionEventHandler+=new M68HC11.Parser.Parser.ParsingExceptionEventDelegate(parser_ParsingExceptionEventHandler);

            System.IO.FileInfo fiApp=new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
            appPath=fiApp.Directory.FullName;
        }

        private FrmMainIDE IDE;
        public string appPath;
        public Interpreter interpreter;
        public Debugger debugger;
        private ProcessingUnit PU;
        private ParserS19 parser;
        private int gvLexed_selectedRowIndex;
        private DataGridViewCell cellEditing;

        private MPU M6811MPU;
        public Ref<Interpreter> interpreterReference;
        public Ref<Debugger> debuggerReference;
        public Ref<ProcessingUnit> PUReference;
        public Ref<MPU> M6811MPUReference;
        
        private bool executing=false;
        private FrmMemoryMap FrmRAMMemMap;
        private Dictionary<int,CommonOperation> operations;
        //private Collection<int> addr;
        private DataGridViewColumn colOperation;

        public void setIDE(FrmMainIDE ide) {
            this.IDE=ide;
            this.IDE.frmMemoryWatchDebugger=new FrmMemoryViewer(PUReference);
        }

        public void CPU_finishedEventHandler() {
            
        }

        public void CPU_startedEventHandler() {
            
        }

        public void parser_ParsingExceptionEventHandler(Exception ex) {
            executing=true;
            toggleUI();
            executing=false;
            MessageBox.Show(ex.Message.ToString());
        }

        private void debugger_RuntimeExceptionEventHandler(int lineOfCode,Exception ex) {
            MessageBox.Show("["+lineOfCode+"] "+ex.Message.ToString());
        }

        public void ResetPC() {
            //interpreter.ProcessingUnit.GetStartAddress().getInt();
            gvLexed.ClearSelection();
            gvLexed.Rows[0].Selected=true;
        }

        public void PC_programCounterUpdatedEventHandler() {
            if(InvokeRequired) {
                //this.BeginInvoke(new MethodInvoker(updatePUInfo));
                this.Invoke(new MethodInvoker(PC_programCounterUpdatedEventHandler));
                return;
            }

            // Get Current Register Status:
            this.IDE.RegisterProperies=interpreter.ProcessingUnit.GetRegisterStatus();

            Collection<Register> callStack=interpreter.ProcessingUnit.GetCallStack();
            if(this.IDE.GetCallStackSize()<callStack.Count) {
                this.IDE.frmProcessingUnit.Push(callStack[callStack.Count-1]);
            }
            else if(this.IDE.GetCallStackSize()>callStack.Count) {
                this.IDE.frmProcessingUnit.Pop();
            }
            if(callStack.Count==0) {
                this.IDE.ClearStack();
            }
            //this.IDE.CallStack=callStack;

            // Trace code using PC:
            int pc=interpreter.ProcessingUnit.GetCurrentProgramCounterAddress();

            gvLexed.ClearSelection();
            CommonOperation op;

            if(operations.TryGetValue(pc,out op)) {

                this.gvLexed.Rows[op.GetLineNumber()].Selected=true;
                //lstBreakpoints.SelectedIndex=i;
                //break;
            }
            else {
                //M6811MPU=new MPU();
                //throw new Exception("PC can not be used to locate operations!");
                //toggleExecution(); // ???????????????? do this ?????
            }
        }

        public void CPU_breakpointTriggeredEventHandler(int breakpointAddress) {
            
        }

        private void Form1_Load(object sender,EventArgs e) {
            //this.lstLexed.ScrollAlwaysVisible=true;
            //this.lstLexed.HorizontalScrollbar=true;
        }

        private void btnExecute_Click(object sender,EventArgs e) {
            toggleExecution();
        }

        private bool compileS19(out string s19File) {
            // Execute s19 assembler:
            
            string tmpFileName="ewok.asm";
            string tmpFile;
            string assemblerOutput;

            tmpFile=appPath+"\\"+tmpFileName+".tmp";
            s19File=appPath+"\\"+tmpFileName+".s19";
            File.WriteAllText(tmpFile,this.txtASM.Text);

            assemblerOutput=Task.StartTask("\""+appPath+"\\Ewok.Machine.Asm.S19.exe\"","-asm \""+tmpFile+"\"",false);

            if(assemblerOutput.StartsWith("COMPILATION ERRORS")) {
                this.IDE.Output=assemblerOutput.Remove(0,"COMPILATION ERRORS".Length);
                this.IDE.OutputMode=FrmMainIDE.OutputModeValue.ErrorsWithWarnings;
                return false;
            }
            return true;
        }

        private void toggleExecution() {
            if(!executing) { // Execute
                string s19File;

                if(compileS19(out s19File)) {
                    Register startingAddress;

                    this.IDE.Output="Source code assembled with no issues";
                    this.IDE.OutputMode=FrmMainIDE.OutputModeValue.Normal;
                    operations=parser.Parse(s19File,out startingAddress); // Should be able to parse s19 binary with out any exceptions
                    
                    toggleLexView(false);

                    // Execute lexed code:
                    if(operations.Count>0) {

                        ResetPC();

                        // Set processing unit frequency to default value of adjuster control:
                        interpreter.setFrequency(this.IDE.frmControls.adjCPUFreq.Value);

                        try {
                            interpreter.Execute(startingAddress,operations);
                        }
                        catch(Exception ex) {
                            MessageBox.Show(ex.ToString(),"Interpreter Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                        }
                    }
                    else {
                        toggleExecution();
                        return;
                    }

                    toggleUI();
                    executing=true;
                }
                else {
                    //toggleUI();
                    this.IDE.frmOutputMessages.Visible=true;
                    this.IDE.containerHoriz.SplitterDistance=this.IDE.panel2Height;
                    this.btnExecuteHalt.Text="Execute";
                    interpreter.halt();
                    this.gvLexed.Visible=false;
                    this.txtASM.Visible=true;
                    this.IDE.ClearProcessingUnitStatus();
                    this.IDE.frmControls.grpSimulator.Visible=false;
                    this.IDE.frmControls.chkViewAssembly.Checked=false;
                    this.IDE.frmControls.chkViewDetails.Checked=false;

                    executing=false;
                }
            }
            else {
                toggleUI();
                executing=false;
            }
        }

        private void toggleUI() {
            if(InvokeRequired) {
                this.Invoke(new MethodInvoker(toggleUI));
                return;
            }

            if(executing) { // Halt
                this.IDE.frmOutputMessages.Visible=false;
                this.IDE.containerHoriz.SplitterDistance=this.IDE.containerHoriz.Height;

                btnExecuteHalt.Text="Execute";
                interpreter.halt();
                //panelSimulator.Visible=false;
                gvLexed.Visible=false;
                //panelEditor.Visible=true;
                txtASM.Visible=true;
                this.IDE.ClearProcessingUnitStatus();
                this.IDE.frmControls.grpSimulator.Visible=false;
                this.IDE.frmControls.chkViewAssembly.Checked=false;
                this.IDE.frmControls.chkViewDetails.Checked=false;
            }
            else { // Execute
                btnExecuteHalt.Text="Halt";
                this.IDE.frmControls.grpSimulator.Visible=true;

                //panelEditor.Visible=false;
                //panelSimulator.Visible=true;
                this.gvLexed.Visible=true;
                this.txtASM.Visible=false;
                this.IDE.frmOutputMessages.Visible=true;
                this.IDE.containerHoriz.SplitterDistance=this.IDE.panel2Height;
                this.IDE.frmControls.btnPause.Enabled=false;
                this.IDE.frmControls.btnReset.Enabled=false;
                this.IDE.frmControls.isPaused=false;
                this.IDE.frmControls.btnPause.Text="Pause";
            }
        }

        private void btnRamMemMap_Click(object sender,EventArgs e) {
            FrmRAMMemMap=new FrmMemoryMap();
            FrmRAMMemMap.Show();

            // Setup Memory Map according to RAM specification
            FrmRAMMemMap.MemorySize=32*32;//(int)Math.Pow(2,16); // 2^16=64000kB
        }

        public void toggleLexView(bool keepSelectedRows) {
            this.txtOperationEditor.Visible=false;

            if((operations!=null)&&(operations.Count>0)) {
                int selectedRow;
                int i;
                string lineno;
                DataGridViewColumn c0,c1,c2,c3,c4,c5;
                CommonOperation op;

                selectedRow=-1;
                if(keepSelectedRows) {
                    if(this.gvLexed.SelectedRows.Count>0) {
                        selectedRow=this.gvLexed.SelectedRows[0].Index;
                    }
                }

                this.gvLexed.Rows.Clear();
                this.gvLexed.Columns.Clear();

                c0=appendColumn("Line","Lns",this.IDE.frmControls.chkViewLineNums.Checked);                    // View Line Numbers
                c1=appendColumn("Address","Addr",this.IDE.frmControls.chkViewAssembly.Checked);                // View address of operation
                c2=appendColumn("MachineCode","Mcs",this.IDE.frmControls.chkViewAssembly.Checked);             // View machine code
                c3=appendColumn("Operations","Ops",true);                                                      // View just the operations
                c4=appendColumn("Details","Dts",this.IDE.frmControls.chkViewDetails.Checked);                  // View details
                c5=appendColumn("AddressingModes","Ams",this.IDE.frmControls.chkViewAddressingModes.Checked);  // View addressing modes

                colOperation=c3; // For single live operation editing

                for(i=0;i<operations.Count;i++){
                //foreach(KeyValuePair<int,CommonOperation> keyvalue in operations) {
                    op=operations[operations.Keys.ElementAt<int>(i)];
                    //op=keyvalue.Value;
                    op.SetLineNumber(i);

                    if(!op.isASMDirective) {
                        lineno=i.ToString().PadLeft(4,'0');

                        DataGridViewRow row=new DataGridViewRow();
                        object[] cells;
                        
                        cells=new object[gvLexed.Columns.Count];
                        
                        appendCell(ref cells,lineno,this.IDE.frmControls.chkViewLineNums.Checked,c0);
                        appendCell(ref cells,op.ToHexAddress(),this.IDE.frmControls.chkViewAssembly.Checked,c1);
                        appendCell(ref cells,op.ToMachineCode(),this.IDE.frmControls.chkViewAssembly.Checked,c2);
                        appendCell(ref cells,op.GetOperation(),true,c3);
                        appendCell(ref cells,op.ToMachineCodeDetails(),this.IDE.frmControls.chkViewDetails.Checked,c4);
                        appendCell(ref cells,op.GetAddressModeFullName(),this.IDE.frmControls.chkViewAddressingModes.Checked,c5);

                        row.CreateCells(gvLexed,cells);
                        
                        this.gvLexed.Rows.Add(row);
                    }
                }

                if(keepSelectedRows&&(selectedRow!=-1)) {
                    this.gvLexed.Rows[selectedRow].Selected=true;
                }
            }
        }

        public DataGridViewColumn appendColumn(string header,string name,bool condition) {
            if(condition) {
                DataGridViewColumn col=new DataGridViewTextBoxColumn();
                
                col.DataPropertyName=name;
                col.HeaderText=header;
                col.Name=name;
                this.gvLexed.Columns.Add(col);
                Padding p;
                if(gvLexed.Columns.Count==1) {
                    p=new Padding(0,0,0,0);
                    
                }
                else {
                    p=new Padding(20,0,0,0);
                }
                col.DefaultCellStyle.Padding=p;
                return col;
            }
            return null;
        }

        public void appendCell(ref object[] cells,string value,bool condition,DataGridViewColumn col) {
            if(condition) {
                cells[col.Index]=value;
            }
        }

        private void gvLexed_CellMouseClick(object sender,System.Windows.Forms.DataGridViewCellMouseEventArgs e) {
            if(e.Button==System.Windows.Forms.MouseButtons.Right) {
                System.Drawing.Point p;
                System.Drawing.Rectangle rect;

                rect=gvLexed.GetCellDisplayRectangle(e.ColumnIndex,e.RowIndex,false);
                p=rect.Location;

                // Get length measured in pixels of occupied row in grid:
                p.X+=rect.Width;

                // Apply relative offset to context menu
                p.Y=p.Y+10;

                this.gvLexed_selectedRowIndex=e.RowIndex;

                this.contextmnuLexedOperation.Show(gvLexed,p);//.Rows[e.RowIndex]
            }
            else {
                this.txtOperationEditor.Visible=false;
            }
        }

        private void contextmnuLexedOperation_ItemClicked(object sender,System.Windows.Forms.ToolStripItemClickedEventArgs e) {
            System.Drawing.Point p;
            System.Drawing.Rectangle rect;
            DataGridViewRow row=this.gvLexed.Rows[this.gvLexed_selectedRowIndex];
            DataGridViewCell cell=row.Cells["Ops"];
            this.cellEditing=cell;

            rect=gvLexed.GetCellDisplayRectangle(colOperation.Index,this.gvLexed_selectedRowIndex,false);
            p=rect.Location;

            // Get row index

            //MessageBox.Show(cell.Value.ToString());

            // Display a single line TextBox right where the user clicks so they may edit the operation:

            string operation=cell.Value.ToString();
            this.txtOperationEditor.Text=operation;
            this.txtOperationEditor.Width=rect.Width;
            this.txtOperationEditor.Height=rect.Height;
            this.txtOperationEditor.Location=p;
            this.txtOperationEditor.Visible=true;
        }

        private void commitLiveOperationChange() {
            int row=this.cellEditing.RowIndex;
            this.txtOperationEditor.Visible=false;

            MessageBox.Show("Not implemented");
            //TODO: validate user input: use metadata or assembler to parse user input and replace operation with new one
            // if invalid input show messagebox

            //operations.ElementAt<CommonOperation>(row)
            //this.cellEditing.Value=this.txtOperationEditor.Text;
        }

        private void txtOperationEditor_LostFocus(object sender,EventArgs e) {
            commitLiveOperationChange();
        }

        private void txtOperationEditor_KeyDown(object sender,System.Windows.Forms.KeyEventArgs e) {
            if(e.KeyCode==Keys.Enter) {
                commitLiveOperationChange();
            }
        }
    }
}
