using System;

namespace Ewok.Machine.Common.ProcessingUnit {
    public class StackItem<T> {

        public StackItem() {
            this.data=default(T);
            this.child=null;
        }

        public StackItem(StackItem<T> itemToDuplicate) {
            this.data=itemToDuplicate.data;
            this.child=itemToDuplicate.child;
        }

        public StackItem(T data) {
            this.data=data;
            this.child=null;
        }


        public StackItem(T data,StackItem<T> child) {
            this.data=data;
            this.child=child;
        }

        public T data;
        //public Int16 address; 
        public StackItem<T> child;
    }
}
