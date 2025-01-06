using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ewok.Machine.Dasm.S19 {

    public class S19Record {

        public const string INV_S19_FILE_FORMAT="Invalid S19 file format";

        public S19Record(string record) {
            int recordType;

            StartCode=record.Substring(0,1);
            recordType=Int32.Parse(record.Substring(1,1));
            ByteCount=Convert.ToInt32(record.Substring(2,2),16);
            DataLength=(ByteCount*2)-4-2; // minus the address and checksum
            Address=record.Substring(4,4);
            Data=record.Substring(8,DataLength);
            Checksum=record.Substring(DataLength+8,2);

            switch(StartCode+recordType) {
                default:
                    throw new InvalidProgramException(INV_S19_FILE_FORMAT);
                case "S0":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S1":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S2":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S3":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S4":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S7":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S8":
                    RecordType=(S19RecordType)recordType;
                    break;
                case "S9":
                    RecordType=(S19RecordType)recordType;
                    break;
            }
        }

        public string StartCode;

        public S19RecordType RecordType;

        public int ByteCount;

        public int DataLength;

        public string Address;

        public string Data;

        public string Checksum;

        public enum S19RecordType {
            //         Address bytes     Data sequence
            S0_BLOCK_HEADER=0, //   2     Yes
            S1_DATA_SEQUENCE=1,//   2     Yes
            S2_DATA_SEQUENCE=2,//   3     Yes
            S3_DATA_SEQUENCE=3,//   4     Yes
            S4_RECORD_COUNT=5, //   2     No
            S7_END_OF_BLOCK=7, //   4     No
            S8_END_OF_BLOCK=8, //   3     No
            S9_END_OF_BLOCK=9  //   2     No
        }

    }

}
