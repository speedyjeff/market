using market.bots;
using market.engine;

namespace market.console
{
    public static class BaselineMarket
    {
        public static Market RunOnce(MarketConfiguration config, SecurityNames security, BaselinePolicy policy)
        {
            // run the market with the specific policy
            var market = new Market(config);

            var bot = (policy == BaselinePolicy.NeuralBots) ? new NeuralBot() : null;

            do
            {
                // start
                market.StartYear();

                var sales = new List<Transaction>();
                if (policy == BaselinePolicy.NeuralBots) sales = bot.Sell(market);

                // sell
                market.SellSecurities(sales);

                // calculate opportunity
                var price = market.Prices.ByName(security);
                var amount = (market.My.CashBalance / price);
                // ensure round number
                amount -= (amount % market.Config.PurchaseDivisor);
                var buys = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Security = security,
                            Amount = amount,
                            OnMargin = (policy == BaselinePolicy.AlwaysOnMargin)
                        }
                    };

                // check if the policy says buy
                var placeOrder = false;
                if (amount > 0)
                {
                    // buy based on policy
                    if (policy == BaselinePolicy.AlwaysBuy) placeOrder = true;
                    else if (policy == BaselinePolicy.AlwaysBuyLow && price <= market.Config.ParValue) placeOrder = true;
                    else if (policy == BaselinePolicy.AlwaysOnMargin && market.Year != 1 && market.Year != market.Config.LastYear) placeOrder = true;
                    else if (policy == BaselinePolicy.NeuralBots)
                    {
                        buys = bot.Buy(market);
                        placeOrder = true;
                    }
                }

                // place the order
                if (!placeOrder) buys.Clear();
                market.BuySecurities(buys);
            }
            while (market.EndYear());

            return market;
        }

        public static NeuralBotContainer[] RunIteratively(MarketConfiguration config, int iterations, int keep)
        {
            // run multiple iterations, keeping the best models
            // return n of the best models after iterations

            var bots = new NeuralBotContainer[keep*4];
            var rand = new Random();

            // run each iteration
            for (int i = 0; i < iterations; i++)
            {
                // change random seed
                config.Seed = rand.Next();

                // run all models
                Parallel.For(fromInclusive: 0, toExclusive: bots.Length, (i) =>
                {
                    if (bots[i] == null) bots[i] = new NeuralBotContainer();

                    // run the market with the specific policy
                    var market = new Market(config);

                    do
                    {
                        // start
                        market.StartYear();

                        // sell
                        market.SellSecurities(bots[i].Bot.Sell(market));

                        // buy
                        market.BuySecurities(bots[i].Bot.Buy(market));
                    }
                    while (market.EndYear());

                    // capture the networth
                    bots[i].Market = market;
                    bots[i].Networths.Add(market.TotalNetWorth());
                });

                // sort bots by networth
                bots = bots.OrderByDescending(b => b.Networths.Average()).ToArray();

                // take the bottom performers and remove (bias towards consistency)
                for (int j = bots.Length - keep; j < bots.Length; j++) bots[j] = null;
            }

            // keep the first 'keep' bots
            return bots.Take(keep).ToArray();
        }

        #region private

        #endregion
    }
}
