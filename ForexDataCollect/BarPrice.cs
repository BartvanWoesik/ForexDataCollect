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
    public class BarPrice
    {

        public Bars Bars { get; private set; }

        public BarPrice(Robot robot)
        {
            Bars = robot.Bars;
        }

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