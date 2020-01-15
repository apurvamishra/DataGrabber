using JPLC;
using System;
using System.Collections;

namespace DataGrabber.PLCStructures
{
    public class Faults_UDT : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<short> Fault_Pointer { get; set; }

        [Order(2)]
        [ArraySize(201)]
        public JPLCProperty<Fault_UDT>[] Robot_Faults { get; set; }

        public Faults_UDT(int address = 0) : base(address) { }
        
    }
}
