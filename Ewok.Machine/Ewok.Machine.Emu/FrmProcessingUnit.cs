using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ewok.Machine.Common;
using Ewok.Machine.Debug;

using Flobbster.Windows.Forms;
using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.Machine.Emu {
    public partial class FrmProcessingUnit:Form {
        public FrmProcessingUnit(Ref<Interpreter> interpreterReference) {
            InitializeComponent();
            this.interpreterReference=interpreterReference;
        }

        private Ref<Interpreter> interpreterReference;
        private static Dictionary<string,object> __properties;

        public Dictionary<string,object> RegisterProperies {
            set {
                __properties=value;
                PropertyBag bag=new PropertyBag();
                bag.GetValue+=new PropertySpecEventHandler(bag_GetValue);
                foreach(KeyValuePair<string,object> pair in value) {
                    PropertySpec ps;
                    ps=new PropertySpec(pair.Key,typeof(Ewok.Machine.Common.ProcessingUnit.Register),"Machine State","");
                    bag.Properties.Add(ps);
                }


                this.propertyGrid1.SelectedObject=bag;
            }
            get {
                return (Dictionary<string,object>)this.propertyGrid1.SelectedObject;
            }
        }

        void bag_GetValue(object sender,PropertySpecEventArgs e) {
            e.Value=__properties[e.Property.Name];
        }

        //public Collection<Register> CallStack { /* todo: Test correct items are pushed onto CallStack*/
            //set {
            //    int stackRelativeAddr;

            //    if(lstCallStack.Items.Count>2000) {
            //        lstCallStack.Items.Clear(); // use listbox as cache
            //    }

            //    //this.lstCallStack.Items.Clear();
            //    //stackRelativeAddr=0;
            //    Collection<Register> _CallStack=interpreterReference.Value.ProcessingUnit.GetCallStack();

            //    if((lastStackSize>0)&&(lastStackSize<_CallStack.Count)) {
            //        _CallStack.Remove(_CallStack[_CallStack.Count-1]);
            //    }

            //    lastStackSize=_CallStack.Count;
            //    stackRelativeAddr=_CallStack.Count-1;
            //    if(_CallStack.Count>0) {
            //        Register stackItem=_CallStack[_CallStack.Count-1];
            //        lstCallStack.Items.Add("("+stackRelativeAddr+") "+stackItem.ToString());
            //        stackRelativeAddr++;
            //    }
            //    else {
            //        //lstCallStack.Items.Clear();
            //    }
            //    //foreach(Register stackItem in _CallStack) {
            //    //    lstCallStack.Items.Add("("+stackRelativeAddr+") "+stackItem.ToString());
            //    //    stackRelativeAddr++;
            //    //}
            //    if(lstCallStack.Items.Count>0) {
            //        lstCallStack.SelectedIndex=lstCallStack.Items.Count-1;
            //    }
            //}
            //get {
            //    return interpreterReference.Value.ProcessingUnit.GetCallStack();
            //}
        //}

        public void ClearStack() {
            this.lstCallStack.Items.Clear();
        }

        public int GetCallStackSize() {
            return this.lstCallStack.Items.Count;
        }

        public void Push(Register reg) {
            int stackRelativeAddr=lstCallStack.Items.Count;
            lstCallStack.Items.Add("("+stackRelativeAddr+") "+reg.ToString());
            lstCallStack.SelectedIndex=lstCallStack.Items.Count-1;
        }

        public void Pop() {
            if(lstCallStack.Items.Count>0) {
                lstCallStack.Items.Remove(lstCallStack.Items[lstCallStack.Items.Count-1]);
            }
        }

        public void ClearUI() {
            ClearStack();
            this.propertyGrid1.SelectedObject=null;
        }
    }
}
