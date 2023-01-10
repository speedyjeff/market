namespace market.engine
{
    public struct MarketConfiguration
    {
        public int Seed = 0;
        public long ParValue = 100;
        public long NoDividendPrice = 50;
        public long StockSplitPrice = 150;
        public long WorthlessStockPrice = 0;
        public int LastYear = 10;
        public long PurchaseDivisor = 10;
        public bool AdjustStartingPrices = false;
        public long InitialCashBalance = 5000;
        public bool DividendBasedOnMarketPrice = false;

        public long MarginInterestDue = 5;
        public long MarginSplitRatio = 2;
        public long MarginStockMustBeBought = 25;

        public bool WithDebugValidation = false;

        public MarketConfiguration()
        {
        }
    }
}
