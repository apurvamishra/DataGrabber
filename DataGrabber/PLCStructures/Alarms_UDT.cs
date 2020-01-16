using JPLC;
using System;
using System.Collections;

namespace DataGrabber.PLCStructures
{
    public class Alarms_UDT : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<short> Alarm_Pointer { get; set; }

        [Order(2)]
        [ArraySize(201)]
        public JPLCProperty<Alarm_UDT>[] Alarm { get; set; }

        public Alarms_UDT(int address = 0) : base(address) { }
        
    }
}
