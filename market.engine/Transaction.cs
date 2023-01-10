namespace market.engine
{
    public class Transaction
    {
        public SecurityNames Security { get; set; }
        public long Amount { get; set; }
        public bool OnMargin { get; set; }
    }
}
