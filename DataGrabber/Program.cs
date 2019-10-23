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
using System.Reactive.Disposables;

namespace DataGrabber
{
    class Program
    {

        static void WriteToCSV(string fileNamePath, Beam_Data_for_SCADA db)
        {
            //CSV Builder
            StringBuilder header = new StringBuilder();
            StringBuilder entries = new StringBuilder();

            //If file doesn't exit create one and add header
            if (!File.Exists(fileNamePath))
            {
                header.AppendLine(db.CreateHeader());
                File.AppendAllText(fileNamePath, header.ToString());
            }
            else
            {
                //If file exits but is empty, i.e. without a header/content then add a header
                try
                {
                    File.ReadLines(fileNamePath).First();
                }
                catch (InvalidOperationException ex)
                {
                    Console.WriteLine("CSV file exits but is empty : Creating header and performing data entry.", ex.GetType().Name);
                    header.AppendLine(db.CreateHeader());
                    File.AppendAllText(fileNamePath, header.ToString());
                }
                catch (IOException ex)
                {
                    Console.WriteLine( "{0}: The CSV write operation could not be performed because the specified file is OPEN : Close all programs accessing the CSV file, skipping CSV write operations. \n", ex.GetType().Name);
                    return;
                }
            }

            // Check for new beam
            string currentBeamID = db.Beam.Value.Beam_Parameters.Value.ID.Value.ToString();
            string lastCSVBeamID = File.ReadLines(fileNamePath).Last().Split(',')[0];
            Console.WriteLine("Checking for new Beam ID in CSV ....");
            Console.WriteLine((lastCSVBeamID == "BeamID" ? "Old Beam ID = None " : "Old Beam ID = " + lastCSVBeamID));
            Console.WriteLine("Current Beam ID = " + currentBeamID);
            
            if ((currentBeamID != lastCSVBeamID))
            {
                if (currentBeamID != "0")
                {
                    Console.WriteLine($"New Beam ID {currentBeamID}.....Storing in CSV." + "\n");
                    entries.AppendLine(db.ToShortString());
                    File.AppendAllText(fileNamePath, entries.ToString());
                }
                else
                {
                    Console.WriteLine($"...new Beam ID Not detected, Null ID detected!" + "\n");
                }
            }
            else
            {
                Console.WriteLine($"...new Beam ID Not detected!" + "\n");
            }
        }
        static void WriteToSqlDB( SqlConnection cnn,Beam_Data_for_SCADA db)
        {
            //The 'SQLCommand' is a class defined within C#. 
            //This class defines objects which are used to perform SQL operations against a database. 
            //This object will hold the SQL command which will run against our SQL Server database.
            SqlCommand command;
            //DataReader object is used to get all the data returned by an SQL query. 
            //We can then read all the table rows one by one using the data reader.
            SqlDataReader dataReader;
            string currentBeamID = db.Beam.Value.Beam_Parameters.Value.ID.Value.ToString();
            string lastDBEntryResult = "", lastDBEntryQuery = "SELECT BeamID FROM Beams where [Beam Processed Timestamp]=(SELECT MAX ([Beam Processed Timestamp]) FROM Beams)";

            //Read last entry from Database
            //Command object, which is used to execute the SQL statement against the database
            command = new SqlCommand(lastDBEntryQuery, cnn );
            dataReader = command.ExecuteReader();
            while (dataReader.Read())
            {
                lastDBEntryResult = Convert.ToString(dataReader.GetValue(0));
            };
            dataReader.Close();
            command.Dispose();

            //Check if the Beam ID from current data stream is different to the last entry in Database
            //If new beam detected writing to SQL database

            Console.WriteLine("Checking for new Beam ID in Database ....");
            Console.WriteLine((lastDBEntryResult == "" ? "Old Beam ID in Database = None " : "Old Beam ID in Database = " + lastDBEntryResult));
            Console.WriteLine("Current Beam ID = " + currentBeamID);
            if ((currentBeamID != lastDBEntryResult))
            {
                if (currentBeamID != "0")
                {
                    Console.WriteLine($" ....new ID Detected. Storing in Database .... ");
                    //DataAdapter object is used to perform specific SQL operations such as insert, delete and update commands
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    //Command object, which is used to execute the SQL statement against the database
                    command = new SqlCommand(db.SqlQueryInserting(), cnn);
                    //we now associate the insert SQL command to our adapter
                    adapter.InsertCommand = new SqlCommand(db.SqlQueryInserting(), cnn);
                    //then issue the ExecuteNonQuery method which is used to execute the Insert statement against our database
                    adapter.InsertCommand.ExecuteNonQuery();
                    //Dispose of all temp objects created
                    adapter.Dispose();
                    command.Dispose();
                }
                else
                {
                    Console.WriteLine($" ....new Beam ID Not Detected, Null ID detected!");
                }
            }
            else
            {
                Console.WriteLine($" ....new Beam ID Not Detected!");
            }

        }

        static void Main(string[] args)
        {
            Console.WriteLine("One Piece Flow Data Grabber V6.2");
            Console.WriteLine("===========================");

            // =================================================
            // Connecting to Database
            // =================================================
            string connetionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=BeamDatabase;Integrated Security=True";
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
            double timeBetweenRetries = 70; // seconds
            double timeBetweenReads = 5; //seconds

            // =================================================
            // Setup program
            // =================================================
            var scadaDB = new Beam_Data_for_SCADA();
            var faultDB = new Beam_Data_for_SCADA();
            var scadaDBNumber = 113;
            var faultDBNumber = 114;

            // =================================================
            // Reading From PLC
            // =================================================

            var readDBObservable = Observable.Create<Beam_Data_for_SCADA>(o =>
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


            var observable = Observable.Create<Beam_Data_for_SCADA>(o =>
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
            .Concat(Observable.Interval(TimeSpan.FromSeconds(timeBetweenReads)).Zip(readDBObservable.Repeat(), (i, db) => db));


            var disposable = observable
                .ObserveOn(System.Reactive.Concurrency.TaskPoolScheduler.Default)
                .RetryWhen(errors => errors.SelectMany(Observable.Timer(TimeSpan.FromSeconds(timeBetweenRetries))))
                
                .Subscribe(yourdb =>
                {

                    //===============================================================================
                    // THIS SECTION OF CODE WILL REPEAT

                    Console.WriteLine("\n" + $"Reading from PLC DataBase on {DateTime.Now}");
                    Console.WriteLine(yourdb);
                    WriteToCSV(csvFilepath, yourdb);
                    WriteToSqlDB(cnn ,yourdb);


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


