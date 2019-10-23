using JPLC;
using System;

namespace DataGrabber.PLCStructures
{
    public class Faults_UDT : JPLC_BASE
    {
        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<AlarmCodeUDT>[] Robot_Buffering { get; set; }

        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<AlarmCodeUDT>[] Robot_Handling { get; set; }

        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<AlarmCodeUDT>[] Robot_Welding { get; set; }

        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<AlarmCodeUDT>[] Robot_Picking { get; set; }

        public Faults_UDT(int address = 0) : base(address) { }
    }
}
