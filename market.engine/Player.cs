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

        public int CostBasisByName(SecurityNames name)
        {
            // go through the ledger and calculate the cost basis
            // using the average cost basis method (choosing highest price to sell first)
            var scratch = new List<int[]>();
            foreach(var row in RecordSheet)
            {
                if (row.Name != name) continue;

                switch (row.Type)
                {
                    case LedgerRowType.Buy:
                        // push a buy
                        scratch.Add(new int[] { row.Price, row.Amount });
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
                                if (scratch[i][(int)CostBasisSlot.Price] > maxPrice && scratch[i][(int)CostBasisSlot.Amount] > 0)
                                {
                                    index = i;
                                    maxPrice = scratch[i][(int)CostBasisSlot.Price];
                                }
                            }

                            // remove 'amount' of these
                            if (amount <= scratch[index][(int)CostBasisSlot.Amount])
                            {
                                // remove all of amount
                                scratch[index][(int)CostBasisSlot.Amount] -= amount;
                                amount = 0;
                            }
                            else
                            {
                                // remove part of amount and loop again
                                amount -= scratch[index][(int)CostBasisSlot.Amount];
                                scratch[index][(int)CostBasisSlot.Amount] = 0;
                            }
                        }
                        break;
                    case LedgerRowType.Split:
                        // double all the amounts and halve the prices
                        for(int i=0; i<scratch.Count; i++)
                        {
                            scratch[i][(int)CostBasisSlot.Price] = (int)Math.Ceiling((float)scratch[i][(int)CostBasisSlot.Price] / 2f);
                            scratch[i][(int)CostBasisSlot.Amount] *= 2;
                        }
                        break;
                    case LedgerRowType.Worthless:
                        // zero out the cost basis
                        scratch.Clear();
                        break;
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
                totalCost += (scratch[i][(int)CostBasisSlot.Price] * scratch[i][(int)CostBasisSlot.Amount]);
                totalAmount += scratch[i][(int)CostBasisSlot.Amount];
            }

            // no remaining stock
            if (totalAmount == 0) return 0;

            // average price per share
            return (int)Math.Ceiling((float)totalCost/(float)totalAmount);
        }

        #region private
        private enum CostBasisSlot { Price, Amount};
        #endregion
    }
}
