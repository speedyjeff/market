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
            if (costbasis != 100L) throw new Exception("invalid cost basis");
        }

        public static void VariableCostBasis()
        {
            var market = new Market(new MarketConfiguration() { Seed = 65622, AdjustStartingPrices = true, WithDebugValidation = false });
            var name = SecurityNames.StrykerDrillingCompany;

            // buy over years
            BuyUntilBust(market, name);

            // calculate the cost basis
            var costbasis = market.My.CostBasisByName(name);
            if (costbasis != 58L) throw new Exception("invalid cost basis");
        }

        #region private
        private static void BuyUntilBust(Market market, SecurityNames name)
        {
            try
            {
                // add some holdings
                for (int i = 0; i < market.Config.LastYear; i++)
                {
                    // start
                    market.StartYear();

                    // selling
                    var tosell = new List<Transaction>();
                    if (i % 2 != 0)
                    {
                        var holding = market.My.Holdings.ByName(name);
                        if (holding > 10L)
                        {
                            tosell.Add(new Transaction() { Security = name, Amount = 10L });
                        }
                    }
                    market.SellSecurities(tosell);

                    // buying
                    var tobuy = new List<Transaction>();
                    var price = market.Prices.ByName(name);
                    if (market.My.CashBalance >= (price * 20L))
                    {
                        tobuy.Add(new Transaction() { Security = name, Amount = 20L });
                    }
                    market.BuySecurities(tobuy);

                    // end year
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
