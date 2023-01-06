namespace market.engine
{
    public struct MarketConfiguration
    {
        public int Seed = 0;
        public int ParValue = 100;
        public int NoDividendPrice = 50;
        public int StockSplitPrice = 150;
        public int WorthlessStockPrice = 0;
        public int LastYear = 10;
        public int PurchaseDivisor = 10;
        public bool AdjustStartingPrices = false;
        public int InitialCashBalance = 5000;

        public MarketConfiguration()
        {
        }
    }
}
