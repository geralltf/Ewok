using System;

namespace Ewok.Machine.Common {
    /// <summary>
    /// Gives the functionalilty to be able to change a variable when away from the context of the variable by accessing it through its reference
    /// </summary>
    public sealed class Ref<T> {
        private readonly Func<T> getter;
        private readonly Action<T> setter;

        public Ref(Func<T> getter,Action<T> setter) {
            this.getter=getter;
            this.setter=setter;
        }

        public T Value {
            get {
                return getter();
            }
            set {
                setter(value);
            }
        }
    }
}
