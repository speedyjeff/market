namespace market.engine
{
    public enum LedgerRowType { Buy, Sell, Split, Worthless, DividendAndInterest, MarginInterestCharge};

    public struct LedgerRow
    {
        public LedgerRowType Type;
        public int Year;
        public SecurityNames Name;
        public int Amount;
        public int Price;
        public int Cost;
        public int DividendInterest;
        public int MarginChargesPaid;
        public long CashBalance;
        public int MarginTotal;
    }
}
