using JPLC;
using System.Globalization;

using System;

namespace DataGrabber.PLCStructures
{
    public class Faults_for_SCADA : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<Faults_UDT> Faults { get; set; }
        public Faults_for_SCADA(int address = 0) : base(address) { }


    }
}
