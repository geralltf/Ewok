using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ewok.Machine.Emu {
    public partial class FrmOutputMessages:Form {
        public FrmOutputMessages() {
            InitializeComponent();
        }

        public string Output {
            set {
                this.txtOutput.Text=value;
            }
        }

        public Ewok.Machine.Emu.FrmMainIDE.OutputModeValue OutputMode {
            set {
                switch(value) {
                    case FrmMainIDE.OutputModeValue.Errors:
                        this.txtOutput.BackColor=Color.OrangeRed;
                        this.txtOutput.ForeColor=Color.White;
                        break;
                    case FrmMainIDE.OutputModeValue.ErrorsWithWarnings:
                        this.txtOutput.BackColor=Color.Maroon;
                        this.txtOutput.ForeColor=Color.White;
                        break;
                    case FrmMainIDE.OutputModeValue.Normal:
                        this.txtOutput.BackColor=Color.DarkBlue;
                        this.txtOutput.ForeColor=Color.White;
                        break;
                    case FrmMainIDE.OutputModeValue.Warnings:
                        this.txtOutput.BackColor=Color.Orange;
                        this.txtOutput.ForeColor=Color.White;
                        break;
                    default:
                        this.txtOutput.BackColor=Color.FromArgb(0,0,192);
                        this.txtOutput.ForeColor=Color.White;
                        break;
                }
            }
        }
    }
}
