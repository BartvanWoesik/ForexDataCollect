using System;
using System.IO;
using System.Linq;
using cAlgo.API;
using cAlgo;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.WEuropeStandardTime, AccessRights = AccessRights.FullAccess)]
    /// <summary>
    /// Represents a collection of technical indicators used in a trading robot.
    /// </summary>
    public class Indicators
    {
        public CommodityChannelIndex CCI { get; private set; }
        public LinearRegressionRSquared LinearRegressionRS { get; private set; }
        public RelativeStrengthIndex RSI { get; private set; }
        public SimpleMovingAverage SMA { get; private set; }
        public WilliamsPctR WilliamsP { get; private set; }
        public TickVolume TV { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Indicators"/> class.
        /// </summary>
        /// <param name="robot">The robot instance.</param>
        /// <param name="source">The data series.</param>
        /// <param name="periods">The number of periods.</param>
        public Indicators(Robot robot, DataSeries source, int periods)
        {
            CCI = robot.Indicators.CommodityChannelIndex(30);
            LinearRegressionRS = robot.Indicators.LinearRegressionRSquared(source, periods);
            RSI = robot.Indicators.RelativeStrengthIndex(source, periods);
            SMA = robot.Indicators.SimpleMovingAverage(source, periods);
            WilliamsP = robot.Indicators.WilliamsPctR(periods);
            TV = robot.Indicators.TickVolume();
        }

        /// <summary>
        /// Adds indicator data to the given list of data values.
        /// </summary>
        /// <param name="dataValues">The list of data values to add the indicator data to.</param>
        /// <param name="culture">The culture used for formatting the indicator values.</param>
        /// <param name="index">The index of the indicator values to add.</param>
        /// <returns>The updated list of data values with the added indicator data.</returns>
        public List<string> AddIndicatorData(List<string> dataValues, CultureInfo culture, int index)
        {
            dataValues.Add(RSI.Result.Last(index).ToString("F1", culture));
            dataValues.Add(TV.Result.Last(index).ToString("F6", culture));
            dataValues.Add(SMA.Result.Last(index).ToString("F6", culture));
            dataValues.Add(WilliamsP.Result.Last(index).ToString("F6", culture));
            dataValues.Add(LinearRegressionRS.Result.Last(index).ToString("F6", culture));
            dataValues.Add(CCI.Result.Last(index).ToString("F6", culture));
            return dataValues;
        }
    }


}