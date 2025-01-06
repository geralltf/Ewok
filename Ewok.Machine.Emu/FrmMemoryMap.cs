using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ewok.Machine.Common.ProcessingUnit;

namespace Ewok.Machine.Emu {
    public partial class FrmMemoryMap:Form {
        public FrmMemoryMap() {
            InitializeComponent();
        }

        private int memorySize;
        private int width;
        private int height;

        public int MemorySize {
            get {
                return memorySize;
            }
            set {
                this.memorySize=value;
                setupGrid(value);
                populateGrid();
            }
        }

        private void setupGrid(int memSize) {
            int width;
            int height;
            int lblWidth=21;
            int lblHeight=15;

            width=(int)Math.Sqrt(memSize);
            height=width;
            this.width=height;
            this.height=height;

            this.Controls.Clear();

            // Create new Grid layout
            Label lbl;

            for(int x=0;x<width;x++) {
                for(int y=0;y<height;y++) {
                    lbl=new Label();

                    lbl.BackColor=Color.LightGreen;
                    lbl.BorderStyle=BorderStyle.FixedSingle;
                    lbl.Size=new System.Drawing.Size(lblWidth,lblHeight);
                    lbl.Name="lbl"+(x.ToString()+"-"+y.ToString());

                    lbl.Location=new Point(x*lbl.Width,y*lbl.Height);

                    this.Controls.Add(lbl);
                }
            }

            this.Width=(lblWidth+1)*width;
            this.Height=(lblHeight+3)*height;
        }

        public void populateGrid() {
            Label lblx,lbly;
            Label lbl;
            for(int x=0;x<this.width;x++) {
                for(int y=0;y<this.height;y++) {
                    string X,Y;
                    X=x.ToString();
                    Y=y.ToString();
                    lbl=(Label)this.Controls.Find("lbl"+X+"-"+Y,false)[0];

                    if(y==0) {
                        lblx=(Label)this.Controls.Find("lbl"+X+"-"+"0",false)[0];
                        lblx.Text=ALU.convertAnythingToHex(x);
                    }
                    if(x==0) {
                        lbly=(Label)this.Controls.Find("lbl"+"0"+"-"+Y,false)[0];
                        lbly.Text=ALU.convertAnythingToHex(y);
                    }

                }
            }
        }

        public void setMemoryLocation(Int16 address,byte data) {

        }

        public void setMemoryLocation(int index) {

        }

        public void setMemoryLocation(int x, int y) {

        }
    }
}
