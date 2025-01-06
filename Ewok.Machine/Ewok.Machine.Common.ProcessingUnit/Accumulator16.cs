namespace Ewok.Machine.Common.ProcessingUnit {

    /// <summary>
    /// SetDerivative(a,b) makes the Register derive its contents from the result of a binary concatenation of the two 8 bit registers.
    /// When this register is set, register 'a' is set to the high nibble of the input value,
    /// and register 'b' is set to the low nibble.
    /// Of course registers 'a' & 'b' need to update this register when they are individually set.
    /// This register essentially is a mirror of what 'a' and 'b' contain.
    /// </summary>
    public class Accumulator16:Register16 {
        public void SetDerivative(Ref<Accumulator8> a,Ref<Accumulator8> b) {
            this.a=a;
            this.b=b;
        }

        private Ref<Accumulator8> a,b;

        public override bool[] Bits {
            get {
                return base.Bits;
            }
            set {
                base.Bits=value;

                if(a!=null&&b!=null) {   
                    //a.Value.Bits=ALU.getHighNibble(value,base.BitLength);
                    //b.Value.Bits=ALU.getLowNibble(value,base.BitLength);
                }
            }
        }
    }
}
