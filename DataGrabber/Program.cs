using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataGrabber.PLCStructures;
using JPLC;
using System.Reactive.Linq;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Globalization;

namespace DataGrabber
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("One Piece Flow Data Grabber");
            Console.WriteLine("===========================");
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

            // =================================================
            // Connecting to PLC
            // =================================================
            JPLCConnection.IPAddress = "192.168.0.1";
            JPLCConnection.Rack = 0;
            JPLCConnection.Slot = 1;
            Console.WriteLine($"Connecting to PLC on IPAddress {JPLCConnection.IPAddress} with Rack {JPLCConnection.Rack} and Slot {JPLCConnection.Slot}");
            JPLCConnection.Instance.Connect();
            Console.WriteLine($"PLC Connected = {JPLCConnection.Instance.Connected}");

            if (!JPLCConnection.Instance.Connected)
            {
                Console.WriteLine($"{JPLCConnection.Instance.LastError}");
                Console.ReadKey();
                return;
            }

            // =================================================
            // Setup program
            // =================================================
            var beamDBscada = new Beam_Data_for_SCADA();
            var dbNumber = 113;

            // Open or create csv file and get file handle f

            // =================================================
            // Reading From PLC
            // =================================================

            var disposable = Observable.Interval(TimeSpan.FromSeconds(5)).ObserveOn(System.Reactive.Concurrency.TaskPoolScheduler.Default).Select(_ => {
                var result = beamDBscada.ReadFromDB(dbNumber);
                if (result != 0)
                {
                    Console.WriteLine("\n" + "...cannot read from PLC database, trying to reconnect.");
                    JPLCConnection.Instance.Connect();
                    Console.WriteLine($"PLC Connected = {JPLCConnection.Instance.Connected}");
                    //Console.ReadKey();
                    return beamDBscada;
                }
                else
                {
                    return beamDBscada;
                }

            })
                .Subscribe(yourdb =>
            {
                //===============================================================================
                // THIS SECTION OF CODE WILL REPEAT
                Console.WriteLine("\n" + $"Reading from PLC DataBase on {DateTime.Now}");
                
                //Current data stream from PLC
                Console.WriteLine("   Beam Parameters: " + "\n" +
                                  "     BeamID - " + $"{yourdb.Beam.Value.Beam_Parameters.Value.ID.Value}" + "\n" +
                                  "     Length - "+ $"{yourdb.Beam.Value.Beam_Parameters.Value.length.Value}" + "\n" +
                                  "     Width - " + $"{yourdb.Beam.Value.Beam_Parameters.Value.width.Value}" + "\n" +
                                  "     Height - " + $"{yourdb.Beam.Value.Beam_Parameters.Value.height.Value}" + "\n" +
                                  "     Fillet - " + $"{yourdb.Beam.Value.Beam_Parameters.Value.fillet.Value}" + "\n" +
                                  "     Drop - " + $"{yourdb.Beam.Value.Beam_Parameters.Value.drop.Value}" + "\n" +
                                  "   Beam Data: " + "\n" +
                                  "     RH Lock weld time - " + $"{yourdb.Beam.Value.Weld1_Time.Value}" + "\n" +
                                  "     RH Lock weld score - " + $"{yourdb.Beam.Value.Weld1_Score.Value}" + "\n" +
                                  "     LH Lock weld time - " + $"{yourdb.Beam.Value.Weld2_Time.Value}" + "\n" +
                                  "     LH Lock weld score - " + $"{yourdb.Beam.Value.Weld2_Score.Value}" + "\n" +
                                  "     Beam rejected ? " + $"{yourdb.Beam.Value.Beam_Reject.Value}" + "\n" +
                                  "     Reject reason - " + $"{yourdb.Beam.Value.Beam_Reject_Reason.Value}" + "\n" +
                                  "     Beam Processed Timestamp - " + $"{yourdb.Beam.Value.Beam_Processing_Complete_Timestamp.Value}" + "\n" +
                                  "     Beam at stillage ? " + $"{yourdb.Beam.Value.Beam_Process_Stage.Value.beam_at_stillage.Value}" + "\n" +
                                  "     Beam at paintline ? " + $"{yourdb.Beam.Value.Beam_Process_Stage.Value.beam_on_paintline.Value}" + "\n" +
                                  "     Beam at reject bin ? " + $"{yourdb.Beam.Value.Beam_Process_Stage.Value.beam_at_reject.Value}" + "\n"
                                 );

                //CSV Builder
                StringBuilder header = new StringBuilder();
                StringBuilder entries = new StringBuilder();
                //CSV Path and filename
                string fileNamePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OPF.csv";

                //If file doesn't exit create one and add header
                if (!File.Exists(fileNamePath))
                {
                    CreateFileWithHeaderOrAddHeader();
                }
                else
                {
                    //If file exits but is empty, i.e. without a header/content then add a header
                    try
                    {
                        File.ReadLines(fileNamePath).First();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("file was empty : "+ ex.Message);
                        CreateFileWithHeaderOrAddHeader();
                    }
                }
                

                //Check if the Beam ID from current data stream is different to the last entry in CSV file
                string currentBeamID = System.Convert.ToString(yourdb.Beam.Value.Beam_Parameters.Value.ID.Value);
                string lastCSVEntry = File.ReadLines(fileNamePath).Last();
                string[] lastCSVBeamID = lastCSVEntry.Split(',');
                Console.WriteLine("Checking for new Beam ID in CSV....");
                Console.WriteLine((lastCSVBeamID[0] == "id" ? "Old Beam ID in CSV file = None " : "Old Beam ID in CSV file = " + lastCSVBeamID[0]));
                Console.WriteLine("Current Beam ID = " + currentBeamID);
                //Check for new beam ID generated 
                bool newEntryResult = currentBeamID.Equals(lastCSVBeamID[0]);
                Console.WriteLine($" ....new ID {(newEntryResult ? "Not Detected!" : "Detected. Storing in CSV .... " + AddBeamDatatoCSV())}");

                //Check if the Beam ID from current data stream is different to the last entry in SQL Database
                //Establish connection to database
                string connetionString;
                SqlConnection cnn;
                connetionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True";
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                Console.WriteLine("\n" + "Connection to Database ....");
                
                //Read last entry from Database 
                SqlCommand command;
                //DataReader object is used to get all the data specified by the SQL query. We can then read all the table rows one by one using the data reader.
                SqlDataReader dataReader;
                string lastDBEntryResult = "";
                string lastDBEntry = "SELECT BeamID FROM Beams WHERE BeamID not in (SELECT TOP (SELECT COUNT(1)-1 FROM Beams) BeamID FROM Beams)";
                command = new SqlCommand(lastDBEntry, cnn);
                dataReader = command.ExecuteReader();
                //reading data row by row if needed  change GetValue to (0)(1)(2)...
                while (dataReader.Read())
                {
                    lastDBEntryResult = Convert.ToString(dataReader.GetValue(0));
                };
                dataReader.Close();
                command.Dispose();

                //Check if the Beam ID from current data stream is different to the last entry in Database
                Console.WriteLine("Checking for new Beam ID in Database ....");
                Console.WriteLine((lastDBEntryResult == null ? "Old Beam ID in Database = None " : "Old Beam ID in Database = " + lastDBEntryResult));
                Console.WriteLine("Current Beam ID = " + currentBeamID); 
                newEntryResult = currentBeamID.Equals(lastDBEntryResult);
                Console.WriteLine($" ....new ID {(newEntryResult ? "Not Detected!" : "Detected. Storing in Database .... " + AddBeamDatatoDatabase())}");
                //Close DB connection and dispose of all temp objects created
                command.Dispose();
                cnn.Close();

                //sub-programs
                void CreateFileWithHeaderOrAddHeader()
                {
                    header.AppendLine(
                                          "BeamID" + "," +
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
                                          "Beam at reject bin ?"
                                          );
                    File.AppendAllText(fileNamePath, header.ToString());
                }
                string AddBeamDatatoCSV()
                {
                    entries.AppendLine(
                                               yourdb.Beam.Value.Beam_Parameters.Value.ID.Value + "," +
                                               yourdb.Beam.Value.Beam_Parameters.Value.length.Value + "," +
                                               yourdb.Beam.Value.Beam_Parameters.Value.width.Value + "," +
                                               yourdb.Beam.Value.Beam_Parameters.Value.height.Value + "," +
                                               yourdb.Beam.Value.Beam_Parameters.Value.fillet.Value + "," +
                                               yourdb.Beam.Value.Beam_Parameters.Value.drop.Value + "," +
                                               yourdb.Beam.Value.Weld1_Time.Value + "," +
                                               yourdb.Beam.Value.Weld1_Score.Value + "," +
                                               yourdb.Beam.Value.Weld2_Time.Value + "," +
                                               yourdb.Beam.Value.Weld2_Score.Value + "," +
                                               yourdb.Beam.Value.Beam_Reject.Value + "," +
                                               yourdb.Beam.Value.Beam_Reject_Reason.Value + "," +
                                               yourdb.Beam.Value.Beam_Processing_Complete_Timestamp.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig_number.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_in_progress.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_complete.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_weld_2.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig_number.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_in_progress.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_complete.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_stillage.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_paintline.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_reject.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.beam_at_stillage.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.beam_on_paintline.Value + "," +
                                               yourdb.Beam.Value.Beam_Process_Stage.Value.beam_at_reject.Value
                                              );
                    File.AppendAllText(fileNamePath, entries.ToString());
                    Console.WriteLine("Writing to location: " + System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\OPF.csv");

                    return "Done!";
                }
                string AddBeamDatatoDatabase()
                {
                    //Write to Database
                    string sql = "";
                    sql = ("INSERT INTO Beams (" +
                                          "BeamID" + "," +
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
                                            yourdb.Beam.Value.Beam_Parameters.Value.ID.Value + "," +
                                            yourdb.Beam.Value.Beam_Parameters.Value.length.Value + "," +
                                            yourdb.Beam.Value.Beam_Parameters.Value.width.Value + "," +
                                            yourdb.Beam.Value.Beam_Parameters.Value.height.Value + "," +
                                            yourdb.Beam.Value.Beam_Parameters.Value.fillet.Value + "," +
                                            yourdb.Beam.Value.Beam_Parameters.Value.drop.Value + "," +
                                            yourdb.Beam.Value.Weld1_Time.Value + "," +
                                            yourdb.Beam.Value.Weld1_Score.Value + "," +
                                            yourdb.Beam.Value.Weld2_Time.Value + "," +
                                            yourdb.Beam.Value.Weld2_Score.Value + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Reject.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Reject_Reason.Value + "\'" + "," +
                                            // date format in SQL DB 09/13/2019 9:17:56 AM - US
                                            // date format from PLC 13/09/2019 9:17:56 AM - British
                                            "\'" + yourdb.Beam.Value.Beam_Processing_Complete_Timestamp.Value.ToString("G", new CultureInfo("en-US")) + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig.Value + "\'" + "," +
                                            yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_beam_on_jig_number.Value + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_in_progress.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.weld_1_complete.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_weld_2.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig.Value + "\'" + "," +
                                            yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_beam_on_jig_number.Value + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_in_progress.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.weld_2_complete.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_stillage.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_paintline.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.moving_to_reject.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.beam_at_stillage.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.beam_on_paintline.Value + "\'" + "," +
                                            "\'" + yourdb.Beam.Value.Beam_Process_Stage.Value.beam_at_reject.Value + "\'" +
                                            ")");
                    Console.WriteLine(sql);

                    //DataAdapter object is used to perform specific SQL operations such as insert, delete and update commands
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    command = new SqlCommand(sql, cnn);
                    //we now associate the insert SQL command to our adapter
                    adapter.InsertCommand = new SqlCommand(sql, cnn);
                    //then issue the ExecuteNonQuery method which is used to execute the Insert statement against our database
                    adapter.InsertCommand.ExecuteNonQuery();
                    return "Done!";
                };
            });



            
            // =================================================
            // Prepare to exit
            // =================================================

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
            disposable.Dispose();
            Console.WriteLine("Disconnecting");
            JPLCConnection.Instance.Disconnect();

        }
    }
}
