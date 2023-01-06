namespace market.engine
{
    public class Player
    {
        public Player(int initialCashBalance)
        {
            Holdings = new SecuritiesContainer();
            CashBalance = initialCashBalance;
            RecordSheet = new List<LedgerRow>();
        }

        public long CashBalance { get; internal set; }
        public SecuritiesContainer Holdings { get; private set; }
        public List<LedgerRow> RecordSheet { get; private set; }
    }
}
