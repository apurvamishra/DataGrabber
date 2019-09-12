using JPLC;


namespace DataGrabber.PLCStructures
{
    public class Beam_Process_Stages_UDT : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<bool> weld_1_beam_on_jig { get; set; }
        [Order(2)]
        public JPLCProperty<short> weld_1_beam_on_jig_number { get; set; }
        [Order(3)]
        public JPLCProperty<bool> weld_1_in_progress { get; set; }
        [Order(4)]
        public JPLCProperty<bool> weld_1_complete { get; set; }
        [Order(5)]
        public JPLCProperty<bool> moving_to_weld_2  { get; set; }
        [Order(6)]
        public JPLCProperty<bool> weld_2_beam_on_jig  { get; set; }
        [Order(7)]
        public JPLCProperty<short> weld_2_beam_on_jig_number  { get; set; }
        [Order(8)]
        public JPLCProperty<bool> weld_2_in_progress  { get; set; }
        [Order(9)]
        public JPLCProperty<bool> weld_2_complete  { get; set; }
        [Order(10)]
        public JPLCProperty<bool> moving_to_stillage  { get; set; }
        [Order(11)]
        public JPLCProperty<bool> moving_to_paintline  { get; set; }
        [Order(12)]
        public JPLCProperty<bool> moving_to_reject  { get; set; }
        [Order(13)]
        public JPLCProperty<bool> beam_at_stillage  { get; set; }
        [Order(14)]
        public JPLCProperty<bool> beam_on_paintline  { get; set; }
        [Order(15)]
        public JPLCProperty<bool> beam_at_reject  { get; set; }

        public Beam_Process_Stages_UDT(int address = 0) : base(address) { }
    }
}
