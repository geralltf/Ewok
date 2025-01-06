using System;
using System.Collections.Generic;
using Ewok.Machine.Common;

namespace Ewok.Machine.Common {
    public class InvalidCommonOperationException:Exception {

        private int opcode;
        private string operands;
            
        public InvalidCommonOperationException(int opcode) {
            this.opcode=opcode;
        }

        public InvalidCommonOperationException(int opcode,string operands) {
            this.opcode=opcode;
            this.operands=operands;
        }
        
        //public InvalidCommonOperationException(string message) : base(message) { 
        
        //}
        
        //public InvalidCommonOperationException(string message,Exception inner) : base(message,inner) { 
        
        //}

        //protected InvalidCommonOperationException(
        //  System.Runtime.Serialization.SerializationInfo info,
        //  System.Runtime.Serialization.StreamingContext context)
        //    : base(info,context) {

        //}

        private string getMsg() {
            if(string.IsNullOrEmpty(operands)) {
                return "Invalid CommonOperation: "+Convert.ToString(this.opcode,16);
            }
            else {
                return "Invalid CommonOperation: "+Convert.ToString(this.opcode,16)+" "+operands;
            }
        }

        public override string Message {
            get {
                return getMsg();
            }
        }

        public override string ToString() {
            return getMsg();
        }

        public override System.Collections.IDictionary Data {
            get {
                System.Collections.IDictionary helpfulStuff;
                helpfulStuff=new System.Collections.Generic.Dictionary<string,string>();

                helpfulStuff.Add("OpCode",opcode.ToString());
                if(string.IsNullOrEmpty(operands)) {
                    operands="<none set>";
                }
                helpfulStuff.Add("Operands",operands);

                return helpfulStuff;
            }
        }
    }
}
