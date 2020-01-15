using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using JPLC;
using System.Globalization;
using DataGrabber.PLCStructures;

namespace DataGrabber.SQL
{
    class sqlObjects
    {
        public string SqlQueryInserting(Beam_UDT Beam, int pointer)
        {
            return "INSERT INTO Beams (" +
                                  "BeamID" + "," +
                                  "UniqueID" + "," +
                                  "Pointer" + "," +
                                  "Ticks" + "," +
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
                                    Beam.Beam_Parameters.Value.ID.Value + "," +
                                    "\'" + Beam.Beam_Parameters.Value.ID.Value + "-" + Beam.Beam_Processing_Complete_Timestamp.Value.ToString("s", new CultureInfo("en-US")) + "\'" + "," +
                                    pointer + "," +
                                    Beam.Beam_Processing_Complete_Timestamp.Value.Ticks + "," +
                                    Beam.Beam_Parameters.Value.length.Value + "," +
                                    Beam.Beam_Parameters.Value.width.Value + "," +
                                    Beam.Beam_Parameters.Value.height.Value + "," +
                                    Beam.Beam_Parameters.Value.fillet.Value + "," +
                                    Beam.Beam_Parameters.Value.drop.Value + "," +
                                    Beam.Weld1_Time.Value + "," +
                                    Beam.Weld1_Score.Value + "," +
                                    Beam.Weld2_Time.Value + "," +
                                    Beam.Weld2_Score.Value + "," +
                                    "\'" + Beam.Beam_Reject.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Reject_Reason.Value + "\'" + "," +
                                    // date format in SQL DB 09/13/2019 9:17:56 AM - US
                                    // date format from PLC 13/09/2019 9:17:56 AM - British
                                    "\'" + Beam.Beam_Processing_Complete_Timestamp.Value.ToString("G", new CultureInfo("en-US")) + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.weld_1_beam_on_jig.Value + "\'" + "," +
                                    Beam.Beam_Process_Stage.Value.weld_1_beam_on_jig_number.Value + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.weld_1_in_progress.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.weld_1_complete.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.moving_to_weld_2.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.weld_2_beam_on_jig.Value + "\'" + "," +
                                    Beam.Beam_Process_Stage.Value.weld_2_beam_on_jig_number.Value + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.weld_2_in_progress.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.weld_2_complete.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.moving_to_stillage.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.moving_to_paintline.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.moving_to_reject.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.beam_at_stillage.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.beam_on_paintline.Value + "\'" + "," +
                                    "\'" + Beam.Beam_Process_Stage.Value.beam_at_reject.Value + "\'" +
                                    ")";
        }

        public string SqlDBconnetionString()
        {
            return "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True";
        }
    }
}
