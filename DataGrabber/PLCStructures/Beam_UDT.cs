using JPLC;
using System;

namespace DataGrabber.PLCStructures
{
    public class Beam_UDT : JPLC_BASE
    {
       [Order(1)]
       public JPLCProperty<Beam_parameters_UDT> Beam_Parameters { get; set; }
       [Order(2)]
       public JPLCProperty<Beam_Process_Stages_UDT> Beam_Process_Stage { get; set; }
       [Order(3)]
       public JPLCProperty<short> Weld1_Time { get; set; }
       [Order(4)]
       public JPLCProperty<short> Weld1_Score { get; set; }
       [Order(5)]
       public JPLCProperty<short> Weld2_Time { get; set; }
       [Order(6)]
       public JPLCProperty<short> Weld2_Score { get; set; }
       [Order(7)]
       public JPLCProperty<bool> Beam_Reject { get; set; }
       [Order(8)]
       [PLCString(254)]
       public JPLCProperty<string> Beam_Reject_Reason { get; set; }
       [Order(9)]
       public JPLCProperty<DateTime> Beam_Processing_Complete_Timestamp { get; set; }
  
       public Beam_UDT(int address = 0) : base(address) { }
    }
}
