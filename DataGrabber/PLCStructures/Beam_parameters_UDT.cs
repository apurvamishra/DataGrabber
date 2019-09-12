using JPLC;

namespace DataGrabber.PLCStructures
{
    public class Beam_parameters_UDT : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<short> ID { get; set; }
        [Order(2)]
        public JPLCProperty<short> length { get; set; }
        [Order(3)]
        public JPLCProperty<short> width { get; set; }
        [Order(4)]
        public JPLCProperty<short> height { get; set; }
        [Order(5)]
        public JPLCProperty<short> fillet { get; set; }
        [Order(6)]
        public JPLCProperty<short> drop { get; set; }

        public Beam_parameters_UDT(int address = 0) : base(address) { }
    }
}
