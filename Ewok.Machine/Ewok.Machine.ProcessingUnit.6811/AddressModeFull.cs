using Ewok.Machine.Common;

namespace Ewok.M68HC11.CPU.Operation {
    public class AddressModeFull:AddressMode {

        public string getAddressModeFull(string addressMode) {
            switch(addressMode.ToUpper()) {
                case "IMM":
                    return "Immediate";
                case "DIR":
                    return "Direct";
                case "EXT":
                    return "Extended";
                case "INDX":
                    return "IndexedX";
                case "INDY":
                    return "IndexedY";
                case "INHR":
                    return "Inherent";
                case "REL":
                    return "Relative";
                default:
                    return "NOT_SPECIFIED";
            }
        }
    }
}
