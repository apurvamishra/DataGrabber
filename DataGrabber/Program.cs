using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGrabber.PLCStructures;
using DataGrabber.SQL;
using JPLC;
using System.Reactive.Linq;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;
using System.Reactive.Disposables;
using System.Reflection;

namespace DataGrabber
{
    class Program
    {


        static void WriteToBeamsSqlDB( SqlConnection cnn, sqlObjects sql , Beams_for_SCADA Beamsdb)
        {
            //The 'SQLCommand' is a class defined within C#. 
            //This class defines objects which are used to perform SQL operations against a database. 
            //This object will hold the SQL command which will run against our SQL Server database.
            SqlCommand command;
            //DataReader object is used to get all the data returned by an SQL query. 
            //We can then read all the table rows one by one using the data reader.
            SqlDataReader dataReader;
            //Data from PLC
            int currentBeamPointer = Beamsdb.Beams.Value.Beam_Pointer.Value - 1;
            int currentBeamID = Beamsdb.Beams.Value.Beam[currentBeamPointer].Value.Beam_Parameters.Value.ID.Value;
            DateTime currentBeamDateStamp = Beamsdb.Beams.Value.Beam[currentBeamPointer].Value.Beam_Processing_Complete_Timestamp.Value;
            long currentBeamDateStampTicks = currentBeamDateStamp.Ticks;
            //Data from last entry in the database
            int lastDBEntryBeamPointer = 999; //can never be 999 as the buffer can fit 200 entires only
            bool lastDBEntryBeamPointerNULL = false, lastDBEntryBeamIDNULL = false, lastDBEntryBeamDateStampNULL = false, lastDBEntryBeamDateStampTicksNULL = false, databaseEmpty = false; ;
            int lastDBEntryBeamID = 0; //beam being a physical quantity should not have a ID = 0
            DateTime lastDBEntryBeamDateStamp = new DateTime();
            long lastDBEntryBeamDateStampTicks = 0;
            String databaseEmptyQuery = "SELECT Pointer, BeamID, [Beam Processed Timestamp] FROM Beams";
            string lastDBEntryQuery = "SELECT Pointer, BeamID, [Beam Processed Timestamp], Ticks FROM Beams where [Beam Processed Timestamp]=(SELECT MAX ([Beam Processed Timestamp]) FROM Beams)";

            //Read last entry from Database
            //Command object, which is used to execute the SQL statement against the database
            command = new SqlCommand(lastDBEntryQuery, cnn );
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (Convert.ToString(dataReader.GetValue(0)) != "") // check if pointer entry is null, this is for backward compatibility
                {
                    lastDBEntryBeamPointer = Convert.ToInt16(dataReader.GetValue(0));
                }
                else
                {
                    lastDBEntryBeamPointerNULL = true;
                }
                if (Convert.ToString(dataReader.GetValue(1)) != "") // check if beamID is Null
                {
                    lastDBEntryBeamID = Convert.ToInt16(dataReader.GetValue(1));
                }
                else
                {
                    lastDBEntryBeamIDNULL = true;
                }
                if (Convert.ToString(dataReader.GetValue(2)) != "") // check if Datetime is Null,
                {
                    lastDBEntryBeamDateStamp = Convert.ToDateTime(dataReader.GetValue(2));
                }
                else
                {
                    lastDBEntryBeamDateStampNULL = true;
                }
                if (Convert.ToString(dataReader.GetValue(3)) != "") // check if Datetime is Null,
                {
                    lastDBEntryBeamDateStampTicks = Convert.ToInt64(dataReader.GetValue(3));
                }
                else
                {
                    lastDBEntryBeamDateStampTicksNULL = true;
                }
            };
            //Dispose of all temp objects created
            dataReader.Close();
            command.Dispose();

            //Check to see if Database is empty or new 
            //Command object, which is used to execute the SQL statement against the database
            command = new SqlCommand(databaseEmptyQuery, cnn);
            dataReader = command.ExecuteReader();
            //dataReader.Read() will return a False if no entries were found in database
            if (!dataReader.Read())
            {
                databaseEmpty = true;
            };
            //Dispose of all temp objects created
            dataReader.Close();
            command.Dispose();

            //If database is empty
            //Case 1
            if (databaseEmpty)
            {
                // Write everything from PLC buffer to DB except for anything with beamID= 0 
                foreach (var Beams in Beamsdb.Beams.Value.Beam)
                {
                    sql.writeBeamToDB(Beams.Value, command, currentBeamPointer, cnn);
                    /*if (Beams.Value.Beam_Parameters.Value.ID.Value != 0)
                    {
                        Console.WriteLine("Writing Beam with ID:{0} to the database.", Beams.Value.Beam_Parameters.Value.ID.Value);
                        //command is use to perform read and write operations in the database 
                        //Command object, which is used to execute the SQL statement against the database
                        command = new SqlCommand(sql.SqlQueryInsertIntoBeam(Beams.Value, currentBeamPointer), cnn);
                        //DataAdapter object is used to perform specific SQL operations such as insert, delete and update commands
                        SqlDataAdapter adapter = new SqlDataAdapter();
                        //we now associate the insert SQL command to our adapter
                        adapter.InsertCommand = new SqlCommand(sql.SqlQueryInsertIntoBeam(Beams.Value, currentBeamPointer), cnn);
                        //then issue the ExecuteNonQuery method which is used to execute the Insert statement against our database
                        adapter.InsertCommand.ExecuteNonQuery();
                        //Dispose of all temp objects created
                        adapter.Dispose();
                        command.Dispose();
                    }*/
                }
            }
                
            // backward compatibility, when the last entry in Db has no pointer information
            //Case 2
            if (lastDBEntryBeamPointerNULL)
            {
                // iterate thru plc buffer, search for entries not in database and write them to Database
                foreach (var Beams in Beamsdb.Beams.Value.Beam)
                {
                    string BeamDateStampToSearch = Beams.Value.Beam_Processing_Complete_Timestamp.Value.ToString("s", new CultureInfo("en-US"));
                    string searchQuery = "SELECT [Beam Processed Timestamp] FROM Beams where [Beam Processed Timestamp]='"+ BeamDateStampToSearch + "'";
                    //string searchQuery = "SELECT [Beam Processed Timestamp] FROM Beams where [Beam Processed Timestamp]='2019-09-23T12:41:00'";
                    //string searchQuery = "SELECT[Beam Processed Timestamp] FROM Beams where[BeamID] = 15";
                    command = new SqlCommand(searchQuery, cnn);
                    dataReader = command.ExecuteReader();
                    //dataReader.Read() will return a False if no entries were found in database
                    if (!dataReader.Read())
                    {
                        sql.writeBeamToDB(Beams.Value, command, currentBeamPointer, cnn);
                    }
                    //Dispose of all temp objects created
                    dataReader.Close();
                    command.Dispose();
                }
                
            }

            //Check where the pointer from last DB entry sits with respect to current pointer in PLC buffer
            //Case 3
            if (currentBeamPointer == lastDBEntryBeamPointer)
            {
                if (currentBeamDateStampTicks < lastDBEntryBeamDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp shouldn't be older than the last record in database, entry rejected!");
                }
                if ((currentBeamID == lastDBEntryBeamID) && (currentBeamDateStampTicks == lastDBEntryBeamDateStampTicks))
                {
                    // do nothing 
                    Console.WriteLine($" ....new Beam ID Not Detected, no entry made in Database.");
                }
                if (currentBeamDateStampTicks > lastDBEntryBeamDateStampTicks)
                {
                    // means PLC filled up the buffer and the buffer rolled over all the way back to the current pointer
                    // Write everything from PLC buffer to DB except for anything with beamID= 0 
                    foreach (var Beams in Beamsdb.Beams.Value.Beam)
                    {
                        sql.writeBeamToDB(Beams.Value, command, currentBeamPointer, cnn);
                    }

                }
            }

            //Check where the pointer from last DB entry sits with respect to current pointer in PLC buffer
            //Case 4
            if (currentBeamPointer > lastDBEntryBeamPointer)
            {
                if (currentBeamDateStampTicks < lastDBEntryBeamDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp shouldn't be older than the last record in database, entry rejected!");
                }
                if (currentBeamDateStampTicks == lastDBEntryBeamDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp should not be the same as the last recorded entry in database if the pointer has moved, entry rejected!");
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Beamsdb.Beams.Value.Beam[lastDBEntryBeamPointer].Value.Beam_Processing_Complete_Timestamp.Value.Ticks == lastDBEntryBeamDateStampTicks)
                {
                    // means PLC buffer is ahead of the Database and the PLC buffer has not rolled over all the way to the current PLC pointer
                    // Write block of entries from the last DB entry pointer till the current pointer on PLC buffer to DB except for anything with beamID= 0 
                    for (int i= lastDBEntryBeamPointer+1; i <= currentBeamPointer; i++)
                    {
                        sql.writeBeamToDB(Beamsdb.Beams.Value.Beam[i].Value, command, currentBeamPointer, cnn);
                        /*if (Beamsdb.Beams.Value.Beam[i].Value.Beam_Parameters.Value.ID.Value != 0)
                        {
                            Console.WriteLine("Writing Beam with ID:{0} to the database.", Beamsdb.Beams.Value.Beam[i].Value.Beam_Parameters.Value.ID.Value);
                            //command is use to perform read and write operations in the database 
                            //Command object, which is used to execute the SQL statement against the database
                            command = new SqlCommand(sql.SqlQueryInsertIntoBeams(Beamsdb.Beams.Value.Beam[i].Value, currentBeamPointer), cnn);
                            //DataAdapter object is used to perform specific SQL operations such as insert, delete and update commands
                            SqlDataAdapter adapter = new SqlDataAdapter();
                            //we now associate the insert SQL command to our adapter
                            adapter.InsertCommand = new SqlCommand(sql.SqlQueryInsertIntoBeams(Beamsdb.Beams.Value.Beam[i].Value, currentBeamPointer), cnn);
                            //then issue the ExecuteNonQuery method which is used to execute the Insert statement against our database
                            adapter.InsertCommand.ExecuteNonQuery();
                            //Dispose of all temp objects created
                            adapter.Dispose();
                            command.Dispose();
                        }*/
                    }
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Beamsdb.Beams.Value.Beam[lastDBEntryBeamPointer].Value.Beam_Processing_Complete_Timestamp.Value.Ticks > lastDBEntryBeamDateStampTicks)
                {
                    // means PLC filled up the buffer and the buffer rolled over all the way back to the current pointer
                    // Write everything from PLC buffer to DB except for anything with beamID= 0 
                    foreach (var Beams in Beamsdb.Beams.Value.Beam)
                    {
                        sql.writeBeamToDB(Beams.Value, command, currentBeamPointer, cnn);
                    }
                }
            }

            //Check where the pointer from last DB entry sits with respect to current pointer in PLC buffer
            //Case 5
            if (currentBeamPointer < lastDBEntryBeamPointer)
            {
                if (currentBeamDateStampTicks < lastDBEntryBeamDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp shouldn't be older than the last record in database, entry rejected!");
                }
                if (currentBeamDateStampTicks == lastDBEntryBeamDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp should not be the same as the last recorded entry in database if the pointer has moved, entry rejected!");
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Beamsdb.Beams.Value.Beam[lastDBEntryBeamPointer].Value.Beam_Processing_Complete_Timestamp.Value.Ticks == lastDBEntryBeamDateStampTicks)
                {
                    // means PLC buffer filled up and now the PLC buffer pointers sits above the pointer from DB entry
                    // PLC buffer reached pointer 200 and reset to 0 before incrementing again
                    // Write block of entries from the last DB entry pointer till pointer 200 from PLC buffer and then again from 0 till current pointer from PLC buffer 
                    for (int i = lastDBEntryBeamPointer + 1; i <= 200; i++)
                    {
                        sql.writeBeamToDB(Beamsdb.Beams.Value.Beam[i].Value, command, currentBeamPointer, cnn);
                    }
                    for (int i = 0 + 1; i <= currentBeamPointer; i++)
                    {
                        sql.writeBeamToDB(Beamsdb.Beams.Value.Beam[i].Value, command, currentBeamPointer, cnn);
                    }
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Beamsdb.Beams.Value.Beam[lastDBEntryBeamPointer].Value.Beam_Processing_Complete_Timestamp.Value.Ticks > lastDBEntryBeamDateStampTicks)
                {
                    // means PLC filled up the buffer and the buffer rolled over all the way back to the current pointer
                    // Write everything from PLC buffer to DB except for anything with beamID= 0 
                    foreach (var Beams in Beamsdb.Beams.Value.Beam)
                    {
                        sql.writeBeamToDB(Beams.Value, command, currentBeamPointer, cnn);
                    }
                }
            }

        }
        static void WriteToAlarmsSqlDB(SqlConnection cnn, sqlObjects sql, Alarms_for_SCADA Alarmsdb)
        {
            //The 'SQLCommand' is a class defined within C#. 
            //This class defines objects which are used to perform SQL operations against a database. 
            //This object will hold the SQL command which will run against our SQL Server database.
            SqlCommand command;
            //DataReader object is used to get all the data returned by an SQL query. 
            //We can then read all the table rows one by one using the data reader.
            SqlDataReader dataReader;
            //Data from PLC
            int currentAlarmPointer = Alarmsdb.Alarms.Value.Alarm_Pointer.Value - 1;
            int currentAlarmID = Alarmsdb.Alarms.Value.Alarm[currentAlarmPointer].Value.Alarm_ID.Value;
            DateTime currentAlarmDateStamp = Alarmsdb.Alarms.Value.Alarm[currentAlarmPointer].Value.Alarm_Timestamp.Value;
            long currentAlarmDateStampTicks = currentAlarmDateStamp.Ticks;
            //Data from last entry in the database
            int lastDBEntryAlarmPointer = 999; //can never be 999 as the buffer can fit 200 entires only
            bool lastDBEntryAlarmPointerNULL = false, lastDBEntryAlarmIDNULL = false, lastDBEntryAlarmDateStampNULL = false, lastDBEntryAlarmDateStampTicksNULL = false, databaseEmpty = false; ;
            int lastDBEntryAlarmID = 0; //Alarm being a physical quantity should not have a ID = 0
            DateTime lastDBEntryAlarmDateStamp = new DateTime();
            long lastDBEntryAlarmDateStampTicks = 0;
            String databaseEmptyQuery = "SELECT Pointer, AlarmID, AlarmTimestamp FROM Alarms";
            string lastDBEntryQuery = "SELECT Pointer, AlarmID, AlarmTimestamp, Ticks FROM Alarms where AlarmTimestamp=(SELECT MAX (AlarmTimestamp) FROM Alarms)";

            //Read last entry from Database
            //Command object, which is used to execute the SQL statement against the database
            command = new SqlCommand(lastDBEntryQuery, cnn);
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                if (Convert.ToString(dataReader.GetValue(0)) != "") // check if pointer entry is null, this is for backward compatibility
                {
                    lastDBEntryAlarmPointer = Convert.ToInt16(dataReader.GetValue(0));
                }
                else
                {
                    lastDBEntryAlarmPointerNULL = true;
                }
                if (Convert.ToString(dataReader.GetValue(1)) != "") // check if beamID is Null
                {
                    lastDBEntryAlarmID = Convert.ToInt16(dataReader.GetValue(1));
                }
                else
                {
                    lastDBEntryAlarmIDNULL = true;
                }
                if (Convert.ToString(dataReader.GetValue(2)) != "") // check if Datetime is Null,
                {
                    lastDBEntryAlarmDateStamp = Convert.ToDateTime(dataReader.GetValue(2));
                }
                else
                {
                    lastDBEntryAlarmDateStampNULL = true;
                }
                if (Convert.ToString(dataReader.GetValue(3)) != "") // check if Datetime is Null,
                {
                    lastDBEntryAlarmDateStampTicks = Convert.ToInt64(dataReader.GetValue(3));
                }
                else
                {
                    lastDBEntryAlarmDateStampTicksNULL = true;
                }
            };
            //Dispose of all temp objects created
            dataReader.Close();
            command.Dispose();

            //Check to see if Database is empty or new 
            //Command object, which is used to execute the SQL statement against the database
            command = new SqlCommand(databaseEmptyQuery, cnn);
            dataReader = command.ExecuteReader();
            //dataReader.Read() will return a False if no entries were found in database
            if (!dataReader.Read())
            {
                databaseEmpty = true;
            };
            //Dispose of all temp objects created
            dataReader.Close();
            command.Dispose();

            //If database is empty
            //Case 1
            if (databaseEmpty)
            {
                // Write everything from PLC buffer to DB except for anything with AlarmID= 0 
                foreach (var Alarms in Alarmsdb.Alarms.Value.Alarm)
                {
                    sql.writeAlarmToDB(Alarms.Value, command, currentAlarmPointer, cnn);
                }
            }

            // backward compatibility, when the last entry in Db has no pointer information
            //Case 2
            if (lastDBEntryAlarmPointerNULL)
            {
                // iterate thru plc buffer, search for entries not in database and write them to Database
                foreach (var Alarms in Alarmsdb.Alarms.Value.Alarm)
                {
                    string AlarmDateStampToSearch = Alarms.Value.Alarm_Timestamp.Value.ToString("s", new CultureInfo("en-US"));
                    string searchQuery = "SELECT AlarmTimestamp FROM Alarms where AlarmTimestamp='" + AlarmDateStampToSearch + "'";
                    //string searchQuery = "SELECT AlarmTimestamp FROM Alarms where AlarmTimestamp='2019-09-23T12:41:00'";
                    //string searchQuery = "SELECTAlarmTimestamp FROM Alarms where AlarmID = 15";
                    command = new SqlCommand(searchQuery, cnn);
                    dataReader = command.ExecuteReader();
                    //dataReader.Read() will return a False if no entries were found in database
                    if (!dataReader.Read())
                    {
                        sql.writeAlarmToDB(Alarms.Value, command, currentAlarmPointer, cnn);
                    }
                    //Dispose of all temp objects created
                    dataReader.Close();
                    command.Dispose();
                }

            }

            //Check where the pointer from last DB entry sits with respect to current pointer in PLC buffer
            //Case 3
            if (currentAlarmPointer == lastDBEntryAlarmPointer)
            {
                if (currentAlarmDateStampTicks < lastDBEntryAlarmDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp shouldn't be older than the last record in database, entry rejected!");
                }
                if ((currentAlarmID == lastDBEntryAlarmID) && (currentAlarmDateStampTicks == lastDBEntryAlarmDateStampTicks))
                {
                    // do nothing 
                    Console.WriteLine($" ....new Alarm ID Not Detected, no entry made in Database.");
                }
                if (currentAlarmDateStampTicks > lastDBEntryAlarmDateStampTicks)
                {
                    // means PLC filled up the buffer and the buffer rolled over all the way back to the current pointer
                    // Write everything from PLC buffer to DB except for anything with AlarmID= 0 
                    foreach (var Alarms in Alarmsdb.Alarms.Value.Alarm)
                    {
                        sql.writeAlarmToDB(Alarms.Value, command, currentAlarmPointer, cnn);
                    }

                }
            }

            //Check where the pointer from last DB entry sits with respect to current pointer in PLC buffer
            //Case 4
            if (currentAlarmPointer > lastDBEntryAlarmPointer)
            {
                if (currentAlarmDateStampTicks < lastDBEntryAlarmDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp shouldn't be older than the last record in database, entry rejected!");
                }
                if (currentAlarmDateStampTicks == lastDBEntryAlarmDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp should not be the same as the last recorded entry in database if the pointer has moved, entry rejected!");
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Alarmsdb.Alarms.Value.Alarm[lastDBEntryAlarmPointer].Value.Alarm_Timestamp.Value.Ticks == lastDBEntryAlarmDateStampTicks)
                {
                    // means PLC buffer is ahead of the Database and the PLC buffer has not rolled over all the way to the current PLC pointer
                    // Write block of entries from the last DB entry pointer till the current pointer on PLC buffer to DB except for anything with AlarmID= 0 
                    for (int i = lastDBEntryAlarmPointer + 1; i <= currentAlarmPointer; i++)
                    {
                        sql.writeAlarmToDB(Alarmsdb.Alarms.Value.Alarm[i].Value, command, currentAlarmPointer, cnn);
                    }
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Alarmsdb.Alarms.Value.Alarm[lastDBEntryAlarmPointer].Value.Alarm_Timestamp.Value.Ticks > lastDBEntryAlarmDateStampTicks)
                {
                    // means PLC filled up the buffer and the buffer rolled over all the way back to the current pointer
                    // Write everything from PLC buffer to DB except for anything with AlarmID= 0 
                    foreach (var Alarms in Alarmsdb.Alarms.Value.Alarm)
                    {
                        sql.writeAlarmToDB(Alarms.Value, command, currentAlarmPointer, cnn);
                    }
                }
            }

            //Check where the pointer from last DB entry sits with respect to current pointer in PLC buffer
            //Case 5
            if (currentAlarmPointer < lastDBEntryAlarmPointer)
            {
                if (currentAlarmDateStampTicks < lastDBEntryAlarmDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp shouldn't be older than the last record in database, entry rejected!");
                }
                if (currentAlarmDateStampTicks == lastDBEntryAlarmDateStampTicks)
                {
                    // PLC datetime configuration error 
                    Console.WriteLine($" ....PLC datetime settings error! PLC datestamp should not be the same as the last recorded entry in database if the pointer has moved, entry rejected!");
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Alarmsdb.Alarms.Value.Alarm[lastDBEntryAlarmPointer].Value.Alarm_Timestamp.Value.Ticks == lastDBEntryAlarmDateStampTicks)
                {
                    // means PLC buffer filled up and now the PLC buffer pointers sits above the pointer from DB entry
                    // PLC buffer reached pointer 200 and reset to 0 before incrementing again
                    // Write block of entries from the last DB entry pointer till pointer 200 from PLC buffer and then again from 0 till current pointer from PLC buffer 
                    for (int i = lastDBEntryAlarmPointer + 1; i <= 200; i++)
                    {
                        sql.writeAlarmToDB(Alarmsdb.Alarms.Value.Alarm[i].Value, command, currentAlarmPointer, cnn);
                    }
                    for (int i = 0 + 1; i <= currentAlarmPointer; i++)
                    {
                        sql.writeAlarmToDB(Alarmsdb.Alarms.Value.Alarm[i].Value, command, currentAlarmPointer, cnn);
                    }
                }
                // compare last DB entry with the same location on PLC buffer to determine roll overs
                if (Alarmsdb.Alarms.Value.Alarm[lastDBEntryAlarmPointer].Value.Alarm_Timestamp.Value.Ticks > lastDBEntryAlarmDateStampTicks)
                {
                    // means PLC filled up the buffer and the buffer rolled over all the way back to the current pointer
                    // Write everything from PLC buffer to DB except for anything with AlarmID= 0 
                    foreach (var Alarms in Alarmsdb.Alarms.Value.Alarm)
                    {
                        sql.writeAlarmToDB(Alarms.Value, command, currentAlarmPointer, cnn);
                    }
                }
            }

        }
        static void Main(string[] args)
        {
            Console.WriteLine("One Piece Flow Data Grabber V6.3");
            Console.WriteLine("===========================");

            // =================================================
            // Connecting to Database
            // =================================================
            string connetionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True;MultipleActiveResultSets=true";
            bool useDatabase = true;

            SqlConnection cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
            }
            catch (Exception e)
            {
                useDatabase = false;
            }

            if (useDatabase)
            {
                Console.WriteLine("Using SQL Database");
            } else
            {
                Console.WriteLine("Could not use SQL Database");
            }


            // =================================================
            // Connecting to PLC
            // =================================================

            JPLCConnection plc = new JPLCConnection("192.168.0.1", 0, 1);
            string csvFilepath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OPF.csv";
            double timeBetweenRetries = 6; // seconds
            double timeBetweenReads = 5; //seconds

            // =================================================
            // Setup program
            // =================================================
            var scadaDB = new Beams_for_SCADA();
            var faultDB = new Alarms_for_SCADA();
            var sql = new sqlObjects();
            var scadaDBNumber = 113;
            var faultDBNumber = 114;

            // =================================================
            // Reading From PLC
            // =================================================

            // BEAM DATA
            var readDBObservable = Observable.Create<Beams_for_SCADA>(o =>
            {
                var result = scadaDB.ReadFromDB(plc, scadaDBNumber);
                if (result != 0)
                {
                    o.OnError(new Exception("Could not read from DB"));
                    Console.WriteLine("Read failure");
                }
                else
                {
                    o.OnNext(scadaDB);
                    o.OnCompleted();
                }
                return Disposable.Empty;
            });
            // FAULT DATA
            var readFaultDBObservable = Observable.Create<Alarms_for_SCADA>(o =>
            {
                var result = faultDB.ReadFromDB(plc, faultDBNumber);
                if (result != 0)
                {
                    o.OnError(new Exception("Could not read from DB"));
                    Console.WriteLine("Read failure");
                }
                else
                {
                    o.OnNext(faultDB);
                    o.OnCompleted();
                }
                return Disposable.Empty;
            });

            var combinedDBObservable = Observable.Zip(readDBObservable, readFaultDBObservable, (beamdb, faultdb) => (beamdb, faultdb));


            var observable = Observable.Create<(Beams_for_SCADA, Alarms_for_SCADA)>(o =>
            {
                Console.WriteLine($"Attempting to connect to PLC on ip={plc.IPAddress}, rack={plc.Rack}, slot={plc.Slot}");
                if (plc.Connect() != 0)
                {
                    Console.WriteLine("Failed to connect");
                    o.OnError(new Exception("Failed to connect to PLC"));
                }
                else
                {
                    Console.WriteLine("Successfully Connected");
                    o.OnCompleted();
                }
                return Disposable.Empty;
            })
            .Concat(
                Observable.Interval(TimeSpan.FromSeconds(timeBetweenReads))
                .Zip(combinedDBObservable, (interval, dbs) => dbs).Repeat()
            );


            var disposable = observable
                .ObserveOn(System.Reactive.Concurrency.TaskPoolScheduler.Default)
                .RetryWhen(errors => errors.SelectMany(Observable.Timer(TimeSpan.FromSeconds(timeBetweenRetries))))
                .Subscribe(dbs =>
                {
                    var (beamdb, faultdb) = dbs;
                    //===============================================================================
                    // THIS SECTION OF CODE WILL REPEAT

                    Console.WriteLine("\n" + $"Reading from PLC Beam / Fault DataBase on {DateTime.Now}");
                    Console.WriteLine(beamdb);
                    WriteToBeamsSqlDB(cnn , sql, beamdb);
                    WriteToAlarmsSqlDB(cnn, sql, faultdb);
                    Console.WriteLine("\n"+"---------------------------------------------------------"+ "\n");
                });                 







            // =================================================
            // Prepare to exit
            // =================================================

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            disposable.Dispose();
            Console.WriteLine("Disconnecting");
            plc.Disconnect();
            cnn.Close();
        }
    }

}


