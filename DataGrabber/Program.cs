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
using System.Reflection;

namespace DataGrabber
{
    class Program
    {


        static void WriteToSqlDB( SqlConnection cnn,Beams_for_SCADA Beamsdb, Faults_for_SCADA Faultsdb)
        {
            //The 'SQLCommand' is a class defined within C#. 
            //This class defines objects which are used to perform SQL operations against a database. 
            //This object will hold the SQL command which will run against our SQL Server database.
            SqlCommand command;
            //DataReader object is used to get all the data returned by an SQL query. 
            //We can then read all the table rows one by one using the data reader.
            SqlDataReader dataReader;
            int currentBeamPointer = Beamsdb.Beams.Value.Beam_Pointer.Value - 1;
            string currentBeamID = Beamsdb.Beams.Value.Beam[currentBeamPointer].Value.Beam_Parameters.Value.ID.Value.ToString();
            string currentBeamDateStamp = Beamsdb.Beams.Value.Beam[currentBeamPointer].Value.Beam_Processing_Complete_Timestamp.Value.ToString("s", new CultureInfo("en-US"));
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
                    command = new SqlCommand(Beamsdb.SqlQueryInserting(), cnn);
                    //we now associate the insert SQL command to our adapter
                    adapter.InsertCommand = new SqlCommand(Beamsdb.SqlQueryInserting(), cnn);
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


            // Read Faults from PLC and write to DB
            //PropertyInfo[] robots = Faultsdb.Fault.GetType().GetProperties();
            Type robots = Faultsdb.Faults.Value.GetType();
            foreach (PropertyInfo robot in robots.GetProperties())
            {

                Console.WriteLine("{0}", robot);
                

                
            }
            //foreach (Faults_UDT robot in Faultsdb.Faults.Value)
            //{

             //   Console.WriteLine("{0}", robot);

            //}

            Console.ReadKey();
            /*foreach (var i in Faultsdb.Fault.Value.Robot_Buffering)
            {
                Console.WriteLine("{0}-{1}-{2}-{3}",i.Value.Alarm_No.Value, i.Value.Alarm_ID.Value, i.Value.Alarm_Severity.Value, i.Value.Alarm_Timestamp.Value);
            }*/
        }

        static void Main(string[] args)
        {
            Console.WriteLine("One Piece Flow Data Grabber V6.3");
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
            double timeBetweenRetries = 6; // seconds
            double timeBetweenReads = 5; //seconds

            // =================================================
            // Setup program
            // =================================================
            var scadaDB = new Beams_for_SCADA();
            var faultDB = new Faults_for_SCADA();
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
            var readFaultDBObservable = Observable.Create<Faults_for_SCADA>(o =>
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


            var observable = Observable.Create<(Beams_for_SCADA, Faults_for_SCADA)>(o =>
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
                    WriteToSqlDB(cnn , beamdb, faultdb);

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


