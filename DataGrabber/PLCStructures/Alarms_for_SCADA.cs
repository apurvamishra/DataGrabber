using JPLC;
using System.Globalization;
using System;
using System.Reflection;

namespace DataGrabber.PLCStructures
{
    public class Alarms_for_SCADA : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<Alarms_UDT> Alarms { get; set; }
        public Alarms_for_SCADA(int address = 0) : base(address) { }

        public override string ToString()
        {
            return " Hi fault db read is working ";

        }

    }
}
