using System;
using System.Collections.Generic;
using System.Globalization;
using cAlgo.API;
using cAlgo;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.WEuropeStandardTime, AccessRights = AccessRights.FullAccess)]
        /// <summary>
        /// Represents a bar price in the ForexDataCollect robot.
        /// </summary>
        public class BarPrice
        {

            public Bars Bars { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="BarPrice"/> class.
            /// </summary>
            /// <param name="robot">The robot instance.</param>
            public BarPrice(Robot robot)
            {
                Bars = robot.Bars;
            }

            /// <summary>
            /// Adds bar data to the given list of data values.
            /// </summary>
            /// <param name="dataValues">The list of data values to add the bar data to.</param>
            /// <param name="culture">The culture used for formatting the bar data.</param>
            /// <param name="index">The index of the bar data to add.</param>
            /// <returns>The updated list of data values with the added bar data.</returns>
            public List<string> AddBarData(List<string> dataValues, CultureInfo culture, int index)
            {
                dataValues.Add(Bars.ClosePrices.Last(index).ToString("F6", culture));
                dataValues.Add(Bars.OpenPrices.Last(index).ToString("F6", culture));
                dataValues.Add(Bars.HighPrices.Last(index).ToString("F6", culture));
                dataValues.Add(Bars.LowPrices.Last(index).ToString("F6", culture));
                return dataValues;
            }
        }
}