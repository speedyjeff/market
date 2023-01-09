namespace market.engine
{
    public class Transaction
    {
        public SecurityNames Security { get; set; }
        public int Amount { get; set; }
        public bool OnMargin { get; set; }
    }
}
