using JPLC;


namespace DataGrabber.PLCStructures
{
    public class Beam_History : JPLC_BASE
    {
        [Order(1)]
        [ArraySize(30)]
        public JPLCProperty<Beam_Data_UDT>[] Beam { get; set; }

        [Order(2)]
        public JPLCProperty<short> Last_Introduced_Beam_ID { get; set; }
        public Beam_History(int address = 0) : base(address) { }
    }
}
