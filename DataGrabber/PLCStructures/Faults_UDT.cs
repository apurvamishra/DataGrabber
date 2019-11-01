using JPLC;
using System;
using System.Collections;

namespace DataGrabber.PLCStructures
{
    public class Faults_UDT : JPLC_BASE , IEnumerable
    {
        [ArraySize(100)]
        public JPLCProperty<Alarm_CodeUDT>[] Robot_Buffering { get; set; }

        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<Alarm_CodeUDT>[] Robot_Handling { get; set; }

        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<Alarm_CodeUDT>[] Robot_Welding { get; set; }

        [Order(1)]
        [ArraySize(100)]
        public JPLCProperty<Alarm_CodeUDT>[] Robot_Picking { get; set; }

        public Faults_UDT(int address = 0) : base(address) { }

        // Implementation for the GetEnumerator method.
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

      
    }
}
