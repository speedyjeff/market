namespace market.engine.tests
{
    static class PlayerTest
    {
        public static void StaticCostBasis()
        {
            var market = new Market(new MarketConfiguration() { Seed = 65622, AdjustStartingPrices = true, WithDebugValidation = false});
            var name = SecurityNames.CentralCityMunicipalBonds;

            // buy over years
            BuyUntilBust(market, name);

            // calculate the cost basis
            var costbasis = market.My.CostBasisByName(name);
            if (costbasis != 100) throw new Exception("invalid cost basis");
        }

        public static void VariableCostBasis()
        {
            var market = new Market(new MarketConfiguration() { Seed = 65622, AdjustStartingPrices = true, WithDebugValidation = false });
            var name = SecurityNames.StrykerDrillingCompany;

            // buy over years
            BuyUntilBust(market, name);

            // calculate the cost basis
            var costbasis = market.My.CostBasisByName(name);
            if (costbasis != 112) throw new Exception("invalid cost basis");
        }

        #region private
        private static void BuyUntilBust(Market market, SecurityNames name)
        {
            try
            {
                // add some holdings
                for (int i = 0; i < market.Config.LastYear; i++)
                {
                    market.StartYear();
                    var tosell = new List<Transaction>();
                    if (i % 2 != 0)
                    {
                        // sell 10
                        tosell.Add(new Transaction() { Security = name, Amount = 10 });
                    }
                    market.SellSecurities(tosell);
                    market.BuySecurities(new List<Transaction>()
                    {
                        new Transaction() { Security = name, Amount = 20 }
                    });
                    market.EndYear();
                }
            }
            catch (Exception)
            {
            }

            return;
        }
        #endregion
    }
}
