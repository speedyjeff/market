namespace market.engine
{
    public enum LedgerRowType { Buy, Sell, Split, Worthless, DividendAndInterest, MarginInterestCharge};

    public struct LedgerRow
    {
        public LedgerRowType Type;
        public int Year;
        public SecurityNames Name;
        public long Amount;
        public long Price;
        public long Cost;
        public long DividendInterest;
        public long MarginChargesPaid;
        public long CashBalance;
        public long MarginTotal;
    }
}
