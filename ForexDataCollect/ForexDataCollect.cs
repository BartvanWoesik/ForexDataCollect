using System;
using System.IO;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Text;




namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.WEuropeStandardTime, AccessRights = AccessRights.FullAccess)]



    public class Datacollect : Robot
    {

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Periods", DefaultValue = 14)]
        public int Periods { get; set; }

        [Parameter("Depth", DefaultValue = 100)]
        public int Depth { get; set; }




        // CSV File
        private readonly string DataDir = "C:\\Users\\bartw\\Documents\\cAlgo\\Sources\\Robots\\ForexDataCollect";
        private System.IO.FileStream fstream;
        private System.IO.StreamWriter fwriter;
 
        private Indicators _indicators;
        private BarPrice _barPrice;
 
        private readonly CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");

        protected void DataCollect()
        {
            OnStart();
        }
        protected override void OnStart()
        {

            // Put your initialization logic here

            // Alles naar US notatie!!!
            CultureInfo nonInvariantCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = nonInvariantCulture;
            _indicators = new Indicators(this, Source, Periods);
            _barPrice = new BarPrice(this);

            Create_files();

        }

        protected override void OnBar()
        {
            string feature_line = Create_Data_Line();
            if (feature_line != null)
            {
                string joined = feature_line;
                fwriter.WriteLine(joined);
            }

            

        }
        protected override void OnStop()
        {
            fwriter.Close();
            fstream.Close();
        }

        private string Create_Data_Line()
        {


            List<string> dataValues = new List<string>();
            DateTime serverTime = Server.Time;
            dataValues.Add(serverTime.ToString("dd.MM.yyyy HH:mm:ss"));


            for (int i = 1; i < 150; i++) 
            {
                _indicators.AddIndicatorData(dataValues, culture, i);
                _barPrice.AddBarData(dataValues, culture, i);
      
            }

          

            // Combine dataValues into a single string separated by commas
            string outputString = string.Join(",", dataValues);
            // Add the data to the features list
         
            return outputString;

        }

        

        private string Create_Header()
        {
            StringBuilder csvHeader = new StringBuilder("Datum,");
            for (int i = 1; i < Depth; i++)
            {
                csvHeader.Append(GetHeader(i));
            }
            csvHeader.AppendLine();
            return csvHeader.ToString();
        }

        // Helper method to get the header for each bar data
        private string GetHeader(int index)
        {
            return string.Format("close_price{0},open_price{0},high_price{0},low_price{0},rsi{0},tv{0},sma{0},williams{0},regrs{0},cci{0},", index);
        }






        protected void Create_files()
        {

            string fiName = DataDir + "\\Unprocessed_Data.csv";
            // Create directory if it does not exist
            if (System.IO.Directory.Exists(DataDir))
            {
                System.IO.Directory.CreateDirectory(DataDir);
            }

            System.IO.File.WriteAllText(fiName, Create_Header());
            fstream = File.Open(fiName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            fstream.Seek(0, SeekOrigin.End);
            fwriter = new System.IO.StreamWriter(fstream, System.Text.Encoding.UTF8, 1)
            {
                AutoFlush = true
            };

        }
    }
}

