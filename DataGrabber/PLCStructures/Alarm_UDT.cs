using JPLC;
using System;

namespace DataGrabber.PLCStructures
{
    public class Alarm_UDT : JPLC_BASE
    {
        [Order(1)]
        [PLCString(254)]
        public JPLCProperty<string> Robot_Name { get; set; }
        [Order(2)]
        public JPLCProperty<short> Alarm_No { get; set; }
        [Order(3)]
        public JPLCProperty<byte> Alarm_ID { get; set; }
        [Order(4)]
        public JPLCProperty<byte> Alarm_Severity { get; set; }
        [Order(5)]
        public JPLCProperty<DateTime> Alarm_Timestamp { get; set; }

        public Alarm_UDT(int address = 0) : base(address) { }
    }
}
