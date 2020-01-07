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

        public string SqlQueryInserting()
        {
            return "INSERT INTO Beams (" +
                                  "BeamID" + "," +
                                  "UniqueID" + "," +
                                  "Length" + "," +
                                  "Width" + "," +
                                  "Height" + "," +
                                  "Fillet" + "," +
                                  "[Drop]" + "," +
                                  "[RH Lock weld time]" + "," +
                                  "[RH Lock weld score]" + "," +
                                  "[LH Lock weld time]" + "," +
                                  "[LH Lock weld score]" + "," +
                                  "[Beam rejected ?]" + "," +
                                  "[Reject reason]" + "," +
                                  "[Beam Processed Timestamp]" + "," +
                                  "[For welding RH Lock: beam on jig ?]" + "," +
                                  "[For welding RH Lock: beam on jig number]" + "," +
                                  "[Welding RH Lock: in progress ?]" + "," +
                                  "[Welding RH Lock: completed ?]" + "," +
                                  "[Progressing to weld 2 ?]" + "," +
                                  "[For welding LH Lock: beam on jig ?]" + "," +
                                  "[For welding LH Lock: beam on jig number]" + "," +
                                  "[Welding LH Lock: in progress ?]" + "," +
                                  "[Welding LH Lock: completed ?]" + "," +
                                  "[Beam moving towards stillage ?]" + "," +
                                  "[Beam moving towards paintline ?]" + "," +
                                  "[Beam moving towards reject bin ?]" + "," +
                                  "[Beam at stillage ?]" + "," +
                                  "[Beam at paintline ?]" + "," +
                                  "[Beam at reject bin ?]" +
                                  ")" +
                               " Values (" +
                                    Beams.Value.Beam[1].Value.Beam_Parameters.Value.ID.Value + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Parameters.Value.ID.Value + "-" + Beams.Value.Beam[1].Value.Beam_Processing_Complete_Timestamp.Value.ToString("s", new CultureInfo("en-US"))+ "\'" + "," +
                                    Beams.Value.Beam[1].Value.Beam_Parameters.Value.length.Value + "," +
                                    Beams.Value.Beam[1].Value.Beam_Parameters.Value.width.Value + "," +
                                    Beams.Value.Beam[1].Value.Beam_Parameters.Value.height.Value + "," +
                                    Beams.Value.Beam[1].Value.Beam_Parameters.Value.fillet.Value + "," +
                                    Beams.Value.Beam[1].Value.Beam_Parameters.Value.drop.Value + "," +
                                    Beams.Value.Beam[1].Value.Weld1_Time.Value + "," +
                                    Beams.Value.Beam[1].Value.Weld1_Score.Value + "," +
                                    Beams.Value.Beam[1].Value.Weld2_Time.Value + "," +
                                    Beams.Value.Beam[1].Value.Weld2_Score.Value + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Reject.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Reject_Reason.Value + "\'" + "," +
                                    // date format in SQL DB 09/13/2019 9:17:56 AM - US
                                    // date format from PLC 13/09/2019 9:17:56 AM - British
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Processing_Complete_Timestamp.Value.ToString("G", new CultureInfo("en-US")) + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_1_beam_on_jig.Value + "\'" + "," +
                                    Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_1_beam_on_jig_number.Value + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_1_in_progress.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_1_complete.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.moving_to_weld_2.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_2_beam_on_jig.Value + "\'" + "," +
                                    Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_2_beam_on_jig_number.Value + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_2_in_progress.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.weld_2_complete.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.moving_to_stillage.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.moving_to_paintline.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.moving_to_reject.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.beam_at_stillage.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.beam_on_paintline.Value + "\'" + "," +
                                    "\'" + Beams.Value.Beam[1].Value.Beam_Process_Stage.Value.beam_at_reject.Value + "\'" +
                                    ")";
        }

        public string SqlDBconnetionString()
        {
            return "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True";
        }
    }
}

