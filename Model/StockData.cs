namespace Shares.Model
{
    public class StockData
    {
        public string symbol { get; set; }
        public string identifier { get; set; }
        public decimal open { get; set; }
        public decimal dayHigh { get; set; }
        public decimal dayLow { get; set; }
        public decimal lastPrice { get; set; }
        public decimal previousClose { get; set; }
        public decimal change { get; set; }
        public decimal pChange { get; set; }
        public decimal yearHigh { get; set; }
        public decimal yearLow { get; set; }
        public decimal totalTradedVolume { get; set; }
        public decimal totalTradedValue { get; set; }
        public string lastUpdateTime { get; set; }
        public string perChange365d { get; set; }
        public string perChange30d { get; set; }

    }
}
