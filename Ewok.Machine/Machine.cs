using System;
using System.Text;
using System.IO;

namespace Ewok.Machine.Common {
    public abstract class Machine { /* not implemented */

        public Machine(Stream bin) {
            this.bin=bin;
        }

        private Stream bin;

        /* Returns an OperationStream for a ProcessingUnit */
        public abstract OperationStream Load();

    }
}
