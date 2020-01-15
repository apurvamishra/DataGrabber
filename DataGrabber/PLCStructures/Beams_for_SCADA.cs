using JPLC;
using System.Globalization;

namespace DataGrabber.PLCStructures
{
    public class Beams_for_SCADA : JPLC_BASE
    {
        // [Order(1)]
        //public JPLCProperty<Beam_UDT> Beam { get; set; }
        [Order(1)]
        public JPLCProperty<Beams_UDT> Beams { get; set; }

        public Beams_for_SCADA(int address = 0) : base(address) { }


        public string CreateHeader()
        {
            return  "BeamID" + "," +
                    "Length" + "," +
                    "Width" + "," +
                    "Height" + "," +
                    "Fillet" + "," +
                    "Drop" + "," +
                    "RH Lock weld time" + "," +
                    "RH Lock weld score" + "," +
                    "LH Lock weld time" + "," +
                    "LH Lock weld score" + "," +
                    "Beam rejected ?" + "," +
                    "Reject reason" + "," +
                    "Beam Processed Timestamp" + "," +
                    "For welding RH Lock: beam on jig ?" + "," +
                    "For welding RH Lock: beam on jig number" + "," +
                    "Welding RH Lock: in progress ?" + "," +
                    "Welding RH Lock: completed ?" + "," +
                    "Progressing to weld 2 ?" + "," +
                    "For welding LH Lock: beam on jig ?" + "," +
                    "For welding LH Lock: beam on jig number" + "," +
                    "Welding LH Lock: in progress ?" + "," +
                    "Welding LH Lock: completed ?" + "," +
                    "Beam moving towards stillage ?" + "," +
                    "Beam moving towards paintline ?" + "," +
                    "Beam moving towards reject bin ?" + "," +
                    "Beam at stillage ?" + "," +
                    "Beam at paintline ?" + "," +
                    "Beam at reject bin ?";

        }


    }
}

