namespace market.engine
{
    public class Player
    {
        public Player(int initialCashBalance)
        {
            Holdings = new SecuritiesContainer();
            CashBalance = initialCashBalance;
            RecordSheet = new List<LedgerRow>();
            Margins = new List<SecurityDetail>();
        }

        public long CashBalance { get; internal set; }
        public SecuritiesContainer Holdings { get; private set; }
        public List<LedgerRow> RecordSheet { get; private set; }

        public int CostBasisByName(SecurityNames name)
        {
            // go through the ledger and calculate the cost basis
            // using the average cost basis method (choosing highest price to sell first)
            var scratch = new List<SecurityDetail>();
            foreach(var row in RecordSheet)
            {
                if (row.Name != name) continue;

                switch (row.Type)
                {
                    case LedgerRowType.Buy:
                        // push a buy
                        scratch.Add(new SecurityDetail() { Price = row.Price, Amount = row.Amount });
                        break;
                    case LedgerRowType.Sell:
                        // subtract from the highest purchase
                        var amount = row.Amount;
                        while (amount > 0)
                        {
                            // find the largest price that has shares to sell
                            var index = 0;
                            var maxPrice = 0;
                            for(int i=0; i<scratch.Count; i++)
                            {
                                // largest stock price with shares to sell
                                if (scratch[i].Price > maxPrice && scratch[i].Amount > 0)
                                {
                                    index = i;
                                    maxPrice = scratch[i].Price;
                                }
                            }

                            // remove 'amount' of these
                            if (amount <= scratch[index].Amount)
                            {
                                // remove all of amount
                                scratch[index].Amount -= amount;
                                amount = 0;
                            }
                            else
                            {
                                // remove part of amount and loop again
                                amount -= scratch[index].Amount;
                                scratch[index].Amount = 0;
                            }
                        }
                        break;
                    case LedgerRowType.Split:
                        // double all the amounts and halve the prices
                        for(int i=0; i<scratch.Count; i++)
                        {
                            scratch[i].Price = (int)Math.Ceiling((float)scratch[i].Price / 2f);
                            scratch[i].Amount *= 2;
                        }
                        break;
                    case LedgerRowType.Worthless:
                        // zero out the cost basis
                        scratch.Clear();
                        break;
                    case LedgerRowType.MarginInterestCharge:
                    case LedgerRowType.DividendAndInterest:
                        // skip
                        break;
                    default: throw new Exception("invalid type");
                }
            }

            // no purchases
            if (scratch.Count == 0) return 0;

            // calculate the cost basis via the average method
            var totalCost = 0;
            var totalAmount = 0;
            for (int i = 0; i < scratch.Count; i++)
            {
                totalCost += (scratch[i].Price * scratch[i].Amount);
                totalAmount += scratch[i].Amount;
            }

            // no remaining stock
            if (totalAmount == 0) return 0;

            // average price per share
            return (int)Math.Ceiling((float)totalCost/(float)totalAmount);
        }

        public int MarginTotalByName(SecurityNames name)
        {
            // iterate through and total the amount of shares margined in this security
            var total = 0;
            foreach(var p in Margins)
            {
                if (p.Name == name) total += p.Amount;
            }
            return total;
        }

        public int MarginTotal
        {
            get
            {
                // iterate through and total the outstanding cost
                var total = 0;
                foreach (var p in Margins)
                {
                    total += (p.Price * p.Amount);
                }
                return total;
            }
        }

        #region internal
        internal List<SecurityDetail> Margins;

        internal int RemoveMarginSecurity(SecurityNames name, int amount)
        {
            // remove 'amount' of this security, and return the cost
            var cost = 0;
            for(int i=0; i<Margins.Count && amount > 0; i++)
            {
                if (Margins[i].Name == name)
                {
                    var minamount = Math.Min(amount, Margins[i].Amount);
                    cost += (Margins[i].Price * minamount);

                    // reduce the amout for this margin purchased security
                    Margins[i].Amount -= minamount;

                    // keep looping until all the shares are sold from this security
                    amount -= minamount;
                }
            }

            if (amount > 0) throw new Exception("invalid amount");

            // remove the empty line items
            for(int i=Margins.Count - 1; i>= 0; i--)
            {
                if (Margins[i].Amount == 0) Margins.RemoveAt(i);
            }

            return cost;
        }
        #endregion
    }
}
