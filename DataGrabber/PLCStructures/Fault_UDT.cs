using JPLC;
using System;

namespace DataGrabber.PLCStructures
{
    public class Fault_UDT : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<short> Alarm_No { get; set; }
        [Order(2)]
        public JPLCProperty<byte> Alarm_ID { get; set; }
        [Order(3)]
        public JPLCProperty<byte> Alarm_Severity { get; set; }
        [Order(4)]
        public JPLCProperty<DateTime> Alarm_Timestamp { get; set; }

        public Fault_UDT(int address = 0) : base(address) { }
    }
}
