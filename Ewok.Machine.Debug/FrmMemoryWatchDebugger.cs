using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.Machine.Debug {
    public partial class FrmMemoryWatchDebugger:Form {
        public FrmMemoryWatchDebugger(Ref<ProcessingUnit> processingUnitReference) {
            InitializeComponent();

            MemoryDebugger=new MemoryDebugger();
            MemoryDebugger.RAMUpdatedEventHandler+=new RAM.MemoryLocationUpdatedEventDelegate(MemoryWatchDebugger_RAMUpdatedEventHandler);
            processingUnitReference.Value.SetMemoryDisp(ref this.MemoryDebugger);
            this.processingUnitReference=processingUnitReference;
            
            setupWatch();
        }

        public MemoryDebugger MemoryDebugger;
        private Ref<ProcessingUnit> processingUnitReference;

        private void setupWatch() {
            DataGridViewColumn c0,c1,c2;

            this.gvWatch.Rows.Clear();
            this.gvWatch.Columns.Clear();

            // Add columns to gridview
            // Name (rw) | Address (rw) | Data (r)
            c0=appendColumn("Name","name",false);
            c1=appendColumn("Data","data",true);
            c2=appendColumn("Address","addr",false);
        }

        public DataGridViewColumn appendColumn(string header,string name,bool _readonly) {
            DataGridViewColumn col=new DataGridViewTextBoxColumn();
            
            col.ReadOnly=_readonly;
            col.DataPropertyName=name;
            col.HeaderText=header;
            col.Name=name;
            this.gvWatch.Columns.Add(col);
            Padding p;
            if(gvWatch.Columns.Count==1) {
                p=new Padding(0,0,0,0);

            }
            else {
                p=new Padding(20,0,0,0);
            }
            col.DefaultCellStyle.Padding=p;
            return col;
        }

        public void appendCell(ref object[] cells,string value,bool condition,DataGridViewColumn col) {
            if(condition) {
                cells[col.Index]=value;
            }
        }

        private void MemoryWatchDebugger_RAMUpdatedEventHandler(int address) {
            int addr;
            DataGridViewRow row;
            DataType addrType;
            string saddr,error;
            object val;

            for(int i=0;i<gvWatch.Rows.Count;i++) {
                row=gvWatch.Rows[i];
                val=row.Cells["addr"].Value;
                if(val!=null) {
                    saddr=val.ToString();
                    if(validateAddressInput(saddr,out addrType,out error)&&tryGetAddrFromString(saddr,out addr)) {
                        if(addr==address) {
                            row.Cells["data"].Value=getDataFromRAMByType(address);
                            break;
                        }
                    }
                }
            }
        }

        private object getDataFromRAMByType(int address) { // TODO: allow data to be of other type (UI data type selection)
            return processingUnitReference.Value.VirtualRAM.get(address);
        }

        private void gvWatch_CellValidating(object sender,System.Windows.Forms.DataGridViewCellValidatingEventArgs e) {
            DataType dataType;
            string error;

            if(e.ColumnIndex!=this.gvWatch.Columns["addr"].Index) {
                return;
            }

            if(!validateAddressInput(e.FormattedValue.ToString(),out dataType,out error)) {
                //this.gvWatch.Rows[e.RowIndex].Cells[e.ColumnIndex].Value="";
                this.gvWatch.ShowCellErrors=true;
                this.gvWatch.EndEdit(DataGridViewDataErrorContexts.Parsing);

                if(dataType==DataType.UNKNOWN) {
                    this.gvWatch.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText="Address does not follow correct format.\nFor hexadecimal either preceed number with '$' or '0x'\nOtherwise value is treated as decimal";
                }
                else if((dataType==DataType.NULL)||(dataType==DataType.INCOMPLETE)) {
                    this.gvWatch.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText=error;
                }
                this.gvWatch.UpdateCellErrorText(e.ColumnIndex,e.RowIndex);
                e.Cancel=true;
            }
        }

        private void gvWatch_CellEndEdit(object sender,System.Windows.Forms.DataGridViewCellEventArgs e) {
            this.gvWatch.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText=string.Empty;
        }

        public enum DataType {
            HEX,
            DEC,
            NULL,
            INCOMPLETE,
            UNKNOWN
        }

        private bool validateAddressInput(string input,out DataType dataType,out string error) {
            Regex expHex1,expHex2,expDec1;

            error="";

            if(string.IsNullOrEmpty(input)) {
                dataType=DataType.NULL;
                error="Address must have a value";
                return true;
            }

            if((input.ToLower()=="0x")||(input=="$")) {
                dataType=DataType.INCOMPLETE;
                error="Address is incomplete. Finish entering in the value";
                return false;
            }

            expHex1=new Regex("^[$][0-9abcdef]+",RegexOptions.Compiled);
            expHex2=new Regex("^[0][x]{1}[0-9abcdef]+",RegexOptions.Compiled|RegexOptions.IgnoreCase);
            expDec1=new Regex("^[0-9]+",RegexOptions.Compiled);

            if(expHex1.IsMatch(input)||expHex2.IsMatch(input)) {
                dataType=DataType.HEX;
            }
            else if(expDec1.IsMatch(input)) {
                dataType=DataType.DEC;
            }
            else {
                dataType=DataType.UNKNOWN;
                error="Address value could not be parsed";
                return false;
            }
            return true;
        }

        private bool tryGetAddrFromString(string saddr,out int addr) {
            DataType dataType;
            string conversionSubject;

            dataType=DataType.UNKNOWN;
            addr=-1;

            if(string.IsNullOrEmpty(saddr)) {
                return false;
            }

            if(saddr.StartsWith("$")) {
                dataType=DataType.HEX;
                conversionSubject=saddr.Remove(0,1);
            }
            else {
                if(saddr.StartsWith("0x")) {
                    dataType=DataType.HEX;
                    conversionSubject=saddr.Remove(0,2);
                }
                else {
                    dataType=DataType.DEC;
                    conversionSubject=saddr;
                }
            }

            if(dataType==DataType.HEX) {
                try {
                    addr=Convert.ToInt32(conversionSubject,16);
                    return true;
                }
                catch {
                    return false;
                }
            }
            else if(dataType==DataType.DEC) {
                if(Int32.TryParse(conversionSubject,out addr)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            return false;
        }

        private void btnClear_Click(object sender,EventArgs e) {
            this.gvWatch.Rows.Clear();
        }
    }
}
