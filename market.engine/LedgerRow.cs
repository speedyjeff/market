namespace market.engine
{
    public struct LedgerRow
    {
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
