using JPLC;
using System.Globalization;
using System;
using System.Reflection;

namespace DataGrabber.PLCStructures
{
    public class Faults_for_SCADA : JPLC_BASE
    {
        [Order(1)]
        public JPLCProperty<Faults_UDT> Faults { get; set; }
        public Faults_for_SCADA(int address = 0) : base(address) { }

        public override string ToString()
        {
            return " Hi Apu fault db read is working ";

        }

       /* public string SqlQueryInserting()
        {
           
            foreach (var i in Fault.Value.Robot_Buffering)
            {
                Console.WriteLine("{0}-{1}-{2}-{3}", i.Value.Alarm_No.Value, i.Value.Alarm_ID.Value, i.Value.Alarm_Severity.Value, i.Value.Alarm_Timestamp.Value);
                return "INSERT INTO Faults (" +
                                  "UniqueID" + "," +
                                  "Robot" + "," +
                                  "AlarmID" + "," +
                                  "AlarmNo" + "," +
                                  "AlarmSeverity" + "," +
                                  "AlarmTimeStamp" + "," +
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

                                    "\'" + i.Value.Alarm_No.Value + "-" + Beam.Value.Beam_Processing_Complete_Timestamp.Value.ToString("s", new CultureInfo("en-US")) + "\'" + "," +
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

            
        }*/
    }
}
