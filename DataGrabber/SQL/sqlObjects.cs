using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
//using JPLC;
using System.Globalization;
using DataGrabber.PLCStructures;

namespace DataGrabber.SQL
{
    class sqlObjects
    {
        public string SqlQueryInsertIntoBeams(Beam_UDT Beam, int pointer)
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
        public string SqlQueryInsertIntoAlarms(Alarm_UDT Alarm, int pointer)
        {
            return "INSERT INTO Alarms (" +
                                  "UniqueID" + "," +
                                  "RobotName" + "," +
                                  "AlarmID" + "," +
                                  "AlarmNo" + "," +
                                  "AlarmSeverity" + "," +
                                  "AlarmTimestamp" + "," +
                                  "Ticks" + "," +
                                  "Pointer" + "," +
                                  ")" +
                               " Values (" +
                                    "\'" + Alarm.Alarm_ID.Value + "-" + Alarm.Alarm_Timestamp.Value.ToString("s", new CultureInfo("en-US")) + "\'" + "," +
                                    Alarm.Robot_Name.Value + "," +
                                    Alarm.Alarm_ID.Value + "," +
                                    Alarm.Alarm_No.Value + "," +
                                    Alarm.Alarm_Severity.Value + "," +
                                    // date format in SQL DB 09/13/2019 9:17:56 AM - US
                                    // date format from PLC 13/09/2019 9:17:56 AM - British
                                    "\'" + Alarm.Alarm_Timestamp.Value.ToString("G", new CultureInfo("en-US")) + "\'" + "," +
                                    Alarm.Alarm_Timestamp.Value.Ticks + "," +
                                    pointer + "," +
                                    ")";
        }

        public void writeBeamToDB(Beam_UDT Beam, SqlCommand command, int Pointer, SqlConnection cnn)
        {
            // Write everything from PLC buffer to DB except for anything with BeamID= 0 
            if (Beam.Beam_Parameters.Value.ID.Value != 0)
            {
                Console.WriteLine("Writing Beam with ID:{0} to the database.", Beam.Beam_Parameters.Value.ID.Value);
                //command is use to perform read and write operations in the database 
                //Command object, which is used to execute the SQL statement against the database
                command = new SqlCommand(SqlQueryInsertIntoBeams(Beam, Pointer), cnn);
                //DataAdapter object is used to perform specific SQL operations such as insert, delete and update commands
                SqlDataAdapter adapter = new SqlDataAdapter();
                //we now associate the insert SQL command to our adapter
                adapter.InsertCommand = new SqlCommand(SqlQueryInsertIntoBeams(Beam, Pointer), cnn);
                //then issue the ExecuteNonQuery method which is used to execute the Insert statement against our database
                adapter.InsertCommand.ExecuteNonQuery();
                //Dispose of all temp objects created
                adapter.Dispose();
                command.Dispose();
            }
        }
        public void writeAlarmToDB(Alarm_UDT Alarms, SqlCommand command, int Pointer, SqlConnection cnn)
        {
            // Write everything from PLC buffer to DB except for anything with AlarmID= 0 
            if (Alarms.Alarm_ID.Value != 0)
            {
                Console.WriteLine("Writing Alarm with ID:{0} to the database.", Alarms.Alarm_ID.Value);
                //command is use to perform read and write operations in the database 
                //Command object, which is used to execute the SQL statement against the database
                command = new SqlCommand(SqlQueryInsertIntoAlarms(Alarms, Pointer), cnn);
                //DataAdapter object is used to perform specific SQL operations such as insert, delete and update commands
                SqlDataAdapter adapter = new SqlDataAdapter();
                //we now associate the insert SQL command to our adapter
                adapter.InsertCommand = new SqlCommand(SqlQueryInsertIntoAlarms(Alarms, Pointer), cnn);
                //then issue the ExecuteNonQuery method which is used to execute the Insert statement against our database
                adapter.InsertCommand.ExecuteNonQuery();
                //Dispose of all temp objects created
                adapter.Dispose();
                command.Dispose();
            }
        }
        public string SqlDBconnetionString()
        {
            return "Data Source=localhost\\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True";
        }
    }
}
