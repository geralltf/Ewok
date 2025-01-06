using System;
//using Ewok.Machine.Common.ProcessingUnit.Operation;

namespace Ewok.Machine.Common {
    public abstract class CommonOperand {

        /// <summary>
        /// The value of the Operand
        /// </summary>
        public Object Value;
        public Object SecondaryValue;
        public CommonOperation LinkedOperation;

        public bool isHexadecimalValue;


        /// <summary>
        /// Returns a string representation of the operand
        /// </summary>
        /// <returns></returns>
        public abstract string GetOperand();
    }
}