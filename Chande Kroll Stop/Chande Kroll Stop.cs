// ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
//
//    Chande Kroll Volatility Stop is a technical analysis indicator created by Tushar Chande and Stanley Kroll suggested the Chande Kroll Stop indicator in their book “The New Technical Trader”.
//    It is a trend-following indicator, detecting stops by calculating the average true range of the volatility of the recent market.
//
// ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ChandeKrollStop : Indicator
    {
        private AverageTrueRange _atr;

        private IndicatorDataSeries _preliminaryHighStop;
        private IndicatorDataSeries _preliminaryLowStop;

        [Parameter("ATR Periods", DefaultValue = 10, MinValue = 1)]
        public int AtrPeriods { get; set; }

        [Parameter("ATR MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType AtrMaType { get; set; }

        [Parameter("Stop Periods", DefaultValue = 9, MinValue = 1)]
        public int StopPeriods { get; set; }

        [Parameter("Multiplier ", DefaultValue = 1, MinValue = 1)]
        public int Multiplier { get; set; }

        [Output("Short Stop", LineColor = "Red", PlotType = PlotType.Line)]
        public IndicatorDataSeries ShortStop { get; set; }

        [Output("Long Stop", LineColor = "Green", PlotType = PlotType.Line)]
        public IndicatorDataSeries LongStop { get; set; }

        protected override void Initialize()
        {
            _atr = Indicators.AverageTrueRange(AtrPeriods, AtrMaType);

            _preliminaryHighStop = CreateDataSeries();
            _preliminaryLowStop = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            var atrMultiplied = _atr.Result[index] * Multiplier;

            _preliminaryHighStop[index] = Bars.HighPrices.Maximum(AtrPeriods) - atrMultiplied;
            _preliminaryLowStop[index] = Bars.LowPrices.Minimum(AtrPeriods) + atrMultiplied;

            LongStop[index] = _preliminaryLowStop.Minimum(StopPeriods);
            ShortStop[index] = _preliminaryHighStop.Maximum(StopPeriods);
        }
    }
}