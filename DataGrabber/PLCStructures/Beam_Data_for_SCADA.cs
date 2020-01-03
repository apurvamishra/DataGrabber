using JPLC;
using System.Globalization;

namespace DataGrabber.PLCStructures
{
    public class Beam_Data_for_SCADA : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<Beam_Data_UDT> Beam { get; set; }
        public Beam_Data_for_SCADA(int address = 0) : base(address) { }

        public override string ToString()
        {
            return "   Beam Parameters: " + "\n" +
                "     BeamID - " + $"{Beam.Value.Beam_Parameters.Value.ID.Value}" + "\n" +
                "     Length - " + $"{Beam.Value.Beam_Parameters.Value.length.Value}" + "\n" +
                "     Width - " + $"{Beam.Value.Beam_Parameters.Value.width.Value}" + "\n" +
                "     Height - " + $"{Beam.Value.Beam_Parameters.Value.height.Value}" + "\n" +
                "     Fillet - " + $"{Beam.Value.Beam_Parameters.Value.fillet.Value}" + "\n" +
                "     Drop - " + $"{Beam.Value.Beam_Parameters.Value.drop.Value}" + "\n" +
                "     Beam Data: " + "\n" +
                "     RH Lock weld time - " + $"{Beam.Value.Weld1_Time.Value}" + "\n" +
                "     RH Lock weld score - " + $"{Beam.Value.Weld1_Score.Value}" + "\n" +
                "     LH Lock weld time - " + $"{Beam.Value.Weld2_Time.Value}" + "\n" +
                "     LH Lock weld score - " + $"{Beam.Value.Weld2_Score.Value}" + "\n" +
                "     Beam rejected ? " + $"{Beam.Value.Beam_Reject.Value}" + "\n" +
                "     Reject reason - " + $"{Beam.Value.Beam_Reject_Reason.Value}" + "\n" +
                "     Beam Processed Timestamp - " + $"{Beam.Value.Beam_Processing_Complete_Timestamp.Value}" + "\n" +
                "     Beam at stillage ? " + $"{Beam.Value.Beam_Process_Stage.Value.beam_at_stillage.Value}" + "\n" +
                "     Beam at paintline ? " + $"{Beam.Value.Beam_Process_Stage.Value.beam_on_paintline.Value}" + "\n" +
                "     Beam at reject bin ? " + $"{Beam.Value.Beam_Process_Stage.Value.beam_at_reject.Value}" + "\n";

        }

        public string ToShortString()
        {
            return Beam.Value.Beam_Parameters.Value.ID.Value + "," +
                   Beam.Value.Beam_Parameters.Value.length.Value + "," +
                   Beam.Value.Beam_Parameters.Value.width.Value + "," +
                   Beam.Value.Beam_Parameters.Value.height.Value + "," +
                   Beam.Value.Beam_Parameters.Value.fillet.Value + "," +
                   Beam.Value.Beam_Parameters.Value.drop.Value + "," +
                   Beam.Value.Weld1_Time.Value + "," +
                   Beam.Value.Weld1_Score.Value + "," +
                   Beam.Value.Weld2_Time.Value + "," +
                   Beam.Value.Weld2_Score.Value + "," +
                   Beam.Value.Beam_Reject.Value + "," +
                   Beam.Value.Beam_Reject_Reason.Value + "," +
                   Beam.Value.Beam_Processing_Complete_Timestamp.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig_number.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_1_in_progress.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_1_complete.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.moving_to_weld_2.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig_number.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_2_in_progress.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.weld_2_complete.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.moving_to_stillage.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.moving_to_paintline.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.moving_to_reject.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.beam_at_stillage.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.beam_on_paintline.Value + "," +
                   Beam.Value.Beam_Process_Stage.Value.beam_at_reject.Value ;
        }

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
                                    Beam.Value.Beam_Parameters.Value.ID.Value + "," +
                                    "\'" + Beam.Value.Beam_Parameters.Value.ID.Value + "-" + Beam.Value.Beam_Processing_Complete_Timestamp.Value.ToString("s", new CultureInfo("en-US"))+ "\'" + "," +                                
                                    Beam.Value.Beam_Parameters.Value.length.Value + "," +
                                    Beam.Value.Beam_Parameters.Value.width.Value + "," +
                                    Beam.Value.Beam_Parameters.Value.height.Value + "," +
                                    Beam.Value.Beam_Parameters.Value.fillet.Value + "," +
                                    Beam.Value.Beam_Parameters.Value.drop.Value + "," +
                                    Beam.Value.Weld1_Time.Value + "," +
                                    Beam.Value.Weld1_Score.Value + "," +
                                    Beam.Value.Weld2_Time.Value + "," +
                                    Beam.Value.Weld2_Score.Value + "," +
                                    "\'" + Beam.Value.Beam_Reject.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Reject_Reason.Value + "\'" + "," +
                                    // date format in SQL DB 09/13/2019 9:17:56 AM - US
                                    // date format from PLC 13/09/2019 9:17:56 AM - British
                                    "\'" + Beam.Value.Beam_Processing_Complete_Timestamp.Value.ToString("G", new CultureInfo("en-US")) + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig.Value + "\'" + "," +
                                    Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig_number.Value + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.weld_1_in_progress.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.weld_1_complete.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.moving_to_weld_2.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig.Value + "\'" + "," +
                                    Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig_number.Value + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.weld_2_in_progress.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.weld_2_complete.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.moving_to_stillage.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.moving_to_paintline.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.moving_to_reject.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.beam_at_stillage.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.beam_on_paintline.Value + "\'" + "," +
                                    "\'" + Beam.Value.Beam_Process_Stage.Value.beam_at_reject.Value + "\'" +
                                    ")";
        }

        public string SqlDBconnetionString()
        {
            return "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True";
        }
    }
}

