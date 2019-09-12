using JPLC;


namespace DataGrabber.PLCStructures
{
    public class Beam_Data_for_SCADA : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<Beam_Data_UDT> Beam { get; set; }
        public Beam_Data_for_SCADA(int address = 0) : base(address) { }
    }
}
