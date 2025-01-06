namespace Ewok.Machine.Common.ProcessingUnit {
    public class Accumulator8:Register8 {

        /// <param name="D"></param>
        /// <param name="accumulatorType">
        /// Set the accumulator type so this accumulator knows how to update Accumulator D
        /// </param>
        public Accumulator8(Ref<Accumulator16> dReference,Accumulator8Type accumulatorType) {
            this.dReference=dReference;
            this.accumulatorType=accumulatorType;
        }

        private Accumulator8Type accumulatorType;
        private Ref<Accumulator16> dReference; // TODO: Access external reference of accumulator D, to be able to get and set its value
        

        // The Bits of this 8 bit Accumulator must be able to update the D accumulator,
        // that is achieved by only updating D's low and high nibble
        public override bool[] Bits {
            get {
                return base.Bits; // Return the 8bit Register bit data
            }
            set {
                if(dReference!=null) {
                    if(accumulatorType==Accumulator8Type.A) { // Update D's high nibble with the current value of Accumulator A
                        dReference.Value.HighNibble=value; // To update D we use its reference
                    }
                    else if(accumulatorType==Accumulator8Type.B) { // Update D's low nibble with the current value of Accumulator B
                        dReference.Value.LowNibble=value; // To update D we use its reference
                    }
                }
                base.Bits=value; // Update the 8bit Register bit data
            }
        }

        public Ref<Accumulator16> getDReference() {
            return dReference;
        }

        public Accumulator8Type getAccumulatorType() {
            return accumulatorType;
        }
    }
}