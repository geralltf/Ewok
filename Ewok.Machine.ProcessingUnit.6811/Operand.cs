using System;
using Ewok.Machine.Common;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.M68HC11.CPU.Operation {
    public class Operand:CommonOperand {

        /// <summary>
        /// Returns a string representation of the operand
        /// </summary>
        /// <returns></returns>
        public override string GetOperand() {
            Object v;
            string r="";
            if(isHexadecimalValue) {
                r="$";
            }

            if(SecondaryValue==null) {
                v=Value;
            }
            else {
                v=SecondaryValue;
            }
            Type vType=v.GetType();

            if(vType==typeof(int)) {
                if(isHexadecimalValue) {
                    r+=ALU.convertAnythingToHex((int)v);
                }
                else {
                    r+=((int)v).ToString();
                }
                return r;
            }
            if(vType==typeof(Int16)) {
                r+=ALU.convertAnythingToHex(Convert.ToInt32(v));
                return r;
            }
            else if(vType==typeof(string)) {
                return r+((string)v);
            }

            return r+v.ToString();
        }
    }
}