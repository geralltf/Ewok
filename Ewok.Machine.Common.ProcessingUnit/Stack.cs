using System;

using Ewok.Machine.Common;

namespace Ewok.Machine.Common.ProcessingUnit {
    public class Stack<T> {

        public Stack() {
            bos=null;
        }

        private StackItem<T> bos; // Bottom of stack

        /// <summary>
        /// Pulls a Register from the stack (Same as pop)
        /// </summary>
        /// <returns></returns>
        public StackItem<T> pull() {
            return pop() as StackItem<T>;
        }

        private void push(StackItem<T> i) {
            if(nos()) {
                bos=i;
            }
            else {
                StackItem<T> currentTos=topItem();
                currentTos.child=i;
            }
        }

        /// <summary>
        /// Pushes a Register on the stack
        /// </summary>
        /// <param name="r"></param>
        public void push(T r) {
            StackItem<T> i=new StackItem<T>(r);
            push(i);
        }

        /// <summary>
        /// Pop a Register off the stack (Same as pull)
        /// </summary>
        /// <returns></returns>
        public StackItem<T> pop() {
            StackItem<T> returnValue=default(StackItem<T>);
            if(!nos()) {
                StackItem<T> oldTos;
                StackItem<T> previous=null;
                StackItem<T> current=bos;
                StackItem<T> next=null;

                do {// A B C, current=previous=bos=A (A is the bottom of the stack and the first item there)
                    previous=current; // 1A, 
                    current=current.child; //1B, 
                    if(current==null) {
                        // Only A in stack
                        //  previous=A (top of stack)
                        //  current=null
                        oldTos=new StackItem<T>(previous);
                        cls();
                        break;
                    }
                    else {
                        // if there is A,B & C  in stack:
                        //  next becomes C
                        next=current.child;
                        if(next==null) {
                            // if there is only a&b in stack:
                            //  next becomes null
                            //  previous=A
                            //  current=B (top of stack)
                            oldTos=new StackItem<T>(current);
                            previous.child=null; // Pop current top of stack

                            break;
                        }
                    }
                } while(1==1);
                // current is now the top of the stack
                // previous is now what the top of the stack will be once current is removed from the stack
                // previous=B, current=C, next=null
                returnValue=oldTos;
            }
            return returnValue;
        }

        /// <summary>
        /// Gets the Register on the top most of the stack
        /// </summary>
        /// <returns></returns>
        public T top() {
            if(!nos()) {
                StackItem<T> c=bos;
                do {
                    if(c.child==null) {
                        return c.data;
                    }
                    c=c.child;
                } while(c!=null);
            }
            return default(T);
        }

        /// <summary>
        /// Gets the Item on the top most of the stack
        /// </summary>
        /// <returns></returns>
        private StackItem<T> topItem() {
            if(!nos()) {
                StackItem<T> c=bos;
                do {
                    if(c.child==null) {
                        return c;
                    }
                    c=c.child;
                } while(c!=null);
            }
            return null;
        }

        /// <summary>
        /// Checks if there are no stack items available
        /// </summary>
        /// <returns></returns>
        public bool nos() {
            return bos==null;
        }

        /// <summary>
        /// Clears the stack
        /// </summary>
        public void cls() {
            bos=null;
        }

        /// <summary>
        /// Lists all the items in the stack
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            string r="Empty";
            if(!nos()) {
                StackItem<T> c=bos;
                int stackRelativeAddress=0;
                //string hexValue;
                r="";
                Register R;
                while(c!=null) {
                    if(c.child!=null) {
                        r+="("+stackRelativeAddress+"): "+c.data.ToString()+" ";
                    }
                    stackRelativeAddress++;
                    c=c.child; 
                }
            }

            return r;
        }

        public System.Collections.ObjectModel.Collection<T> ToList() {
            System.Collections.ObjectModel.Collection<T> ret=new System.Collections.ObjectModel.Collection<T>();
            if(!nos()) {
                StackItem<T> c=bos;
                ret.Clear();
                while(c!=null) {
                    if(c!=null) {
                        ret.Add(c.data);
                    }
                    c=c.child;
                }
            }
            return ret;
        }

    }
}
