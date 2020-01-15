using JPLC;


namespace DataGrabber.PLCStructures
{
    public class Beams_UDT : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<short> Beam_Pointer { get; set; }
        [Order(2)]
        [ArraySize(201)]
        public JPLCProperty<Beam_UDT>[] Beam { get; set; }

        public Beams_UDT(int address = 0) : base(address) { }
    }
}
