using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.M68HC11.Registers {
    public class ConditionCodeRegister:Register8 {

        /// <summary>
        /// Stop disable, bit 7
        /// </summary>
        public bool S {
            get {
                return getBit(7);
            }
            set{
                setBit(7,value);
            }
        }

        /// <summary>
        /// X interrupt mask, bit 6
        /// </summary>
        public bool X {
            get {
                return getBit(6);
            }
            set{
                setBit(6,value);
            }
        }

        /// <summary>
        /// Half carry, bit 5
        /// </summary>
        public bool H {
            get {
                return getBit(5);
            }
            set{
                setBit(5,value);
            }
        }

        /// <summary>
        /// I interrupt mask, bit 4
        /// </summary>
        public bool I {
            get {
                return getBit(4);
            }
            set{
                setBit(4,value);
            }
        }

        /// <summary>
        /// Negative indicator, bit 3
        /// </summary>
        public bool N {
            get {
                return getBit(3);
            }
            set{
                setBit(3,value);
            }
        }

        /// <summary>
        /// Zero indicator, bit 2
        /// </summary>
        public bool Z {
            get {
                return getBit(2);
            }
            set{
                setBit(2,value);
            }
        }

        /// <summary>
        /// 2's complement overflow indicator, bit 1
        /// </summary>
        public bool V {
            get {
                return getBit(1);
            }
            set{
                setBit(1,value);
            }
        }

        /// <summary>
        /// Carry/Borrow, bit 0
        /// </summary>
        public bool C {
            get {
                return getBit(0);
            }
            set{
                setBit(0,value);
            }
        }

        /// <summary>
        /// Returns the current state of the CCR flags register
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "Flags => "
                +"S="+(S?"1":"0")+" "
                +"X="+(X?"1":"0")+" "
                +"H="+(H?"1":"0")+" "
                +"I="+(I?"1":"0")+" "
                +"N="+(N?"1":"0")+" "
                +"Z="+(Z?"1":"0")+" "
                +"V="+(V?"1":"0")+" "
                +"C="+(C?"1":"0")
            ;
        }

    }
}
