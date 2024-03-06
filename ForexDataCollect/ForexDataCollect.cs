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



namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.WEuropeStandardTime, AccessRights = AccessRights.FullAccess)]



    public class Datacollect : Robot
    {

        [Parameter(DefaultValue = 0.0)]
        public double Parameter { get; set; }

        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("Periods", DefaultValue = 14)]
        public int Periods { get; set; }




        // CSV File
        private readonly string DataDir = "C:\\Users\\bartw\\Documents\\cAlgo\\Sources\\Robots\\ForexDataCollect";
        private System.IO.FileStream fstream;
        private System.IO.StreamWriter fwriter;
 

        private CommodityChannelIndex _CCI;
        private LinearRegressionRSquared _linearRegressionRS { get; set; }
        private RelativeStrengthIndex _rsi;
        private SimpleMovingAverage _sma { get; set; }
        private WilliamsPctR _WilliamsP;
        private TickVolume _TV;

        private string csvHeader;




        private readonly List<string> features_list = new List<string>();



        protected void DataCollect()
        {


            // Set culture for consistent formatting
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");


            List<string> dataValues = new List<string>();
            DateTime serverTime = Server.Time;

            dataValues.Add(serverTime.ToString("dd.MM.yyyy HH:mm:ss"));

            // Add headers
            csvHeader = "Datum,";


            for (int i = 1; i < 150; i++) 
            {
                // Add close, open, high, low prices
                AddBarData(dataValues, culture, i);

                // Add RSI, MFI, TV, SMA, Williams, LinearRegression, CCI
                AddIndicatorData(dataValues, culture, i);

                // Add headers for each bar data
                csvHeader += GetHeader(i);
            }

            csvHeader = csvHeader.TrimEnd(',');

            // Combine dataValues into a single string separated by commas
            string outputString = string.Join(",", dataValues);
            // Add the data to the features list
            features_list.Add(outputString);

        }

        protected override void OnStart()
        {

            // Put your initialization logic here

            // Alles naar US notatie!!!
            CultureInfo nonInvariantCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = nonInvariantCulture;

            // Aanroep indicatoren
            _CCI = Indicators.CommodityChannelIndex(30);
            _linearRegressionRS = Indicators.LinearRegressionRSquared(Source, Periods);
            _rsi = Indicators.RelativeStrengthIndex(Source, Periods);
            _sma = Indicators.SimpleMovingAverage(Source, Periods);
            _WilliamsP = Indicators.WilliamsPctR(Periods);
            _TV = Indicators.TickVolume();

            Create_files();

        }

        protected override void OnBar()
        {
            // Put your core logic here
            DataCollect();
            string feature_line = Get_data_line();
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

        public string Get_data_line()
        /* 
         * Get the last line of the features list
         * 
         */
        {
            
            string feature_line = features_list[^1];
            return feature_line;

        }

    

        

        // Helper method to add bar data to the dataValues list
        private void AddBarData(List<string> dataValues, CultureInfo culture, int index)
        {
            dataValues.Add(Bars.ClosePrices.Last(index).ToString("F6", culture));
            dataValues.Add(Bars.OpenPrices.Last(index).ToString("F6", culture));
            dataValues.Add(Bars.HighPrices.Last(index).ToString("F6", culture));
            dataValues.Add(Bars.LowPrices.Last(index).ToString("F6", culture));
        }

        // Helper method to add indicator data to the dataValues list
        private void AddIndicatorData(List<string> dataValues, CultureInfo culture, int index)
        {
            dataValues.Add(_rsi.Result.Last(index).ToString("F1", culture));
            dataValues.Add(_TV.Result.Last(index).ToString("F6", culture));
            dataValues.Add(_sma.Result.Last(index).ToString("F6", culture));
            dataValues.Add(_WilliamsP.Result.Last(index).ToString("F6", culture));
            dataValues.Add(_linearRegressionRS.Result.Last(index).ToString("F6", culture));
            dataValues.Add(_CCI.Result.Last(index).ToString("F6", culture));
        }

        // Helper method to get the header for each bar data
        private string GetHeader(int index)
        {
            return string.Format("close_price{0},open_price{0},high_price{0},low_price{0},rsi{0},mfi{0},tv{0},sma{0},williams{0},regrs{0},cci{0},", index);
        }

        protected void Create_files()
        {

            DataCollect();
         
            csvHeader += "\n";
            string fiName = DataDir + "\\Unprocessed_Data.csv";
            // Create directory if it does not exist
            if (System.IO.Directory.Exists(DataDir))
            {
                System.IO.Directory.CreateDirectory(DataDir);
            }

            System.IO.File.WriteAllText(fiName, csvHeader);
            fstream = File.Open(fiName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            fstream.Seek(0, SeekOrigin.End);
            fwriter = new System.IO.StreamWriter(fstream, System.Text.Encoding.UTF8, 1)
            {
                AutoFlush = true
            };

        }
    }
}

