using market.bots;
using market.engine;

namespace market.console
{
    static class BaselineMarket
    {
        public static Market RunOnce(MarketConfiguration config, SecurityNames security, BaselinePolicy policy)
        {
            // run the market with the specific policy
            var market = new Market(config);
            var random = (policy == BaselinePolicy.Random) ? new Random() : null;

            var bot = (policy == BaselinePolicy.NeuralBots) ? new NeuralBot( new NeuralBotOptions()) : null;

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
                    else if (policy == BaselinePolicy.Random)
                    {
                        // choose if a purchase should occur
                        if (random.Next() % 2 == 0)
                        {
                            placeOrder = true;
                            // choose how many shares
                            amount = buys[0].Amount;
                            // ensure round number
                            amount -= (amount % market.Config.PurchaseDivisor);
                        }
                    }
                }

                // place the order
                if (!placeOrder || amount <= 0) buys.Clear();
                market.BuySecurities(buys);
            }
            while (market.EndYear());

            return market;
        }

        public static BaselineStats RunStats(MarketConfiguration config, Options options)
        {
            // run iteratively and gather stats
            var stats = new BaselineStats()
            {
                YearsPerIteration = options.LastYear,
                TotalYears = options.LastYear * options.Iterations
            };
            var parvalues = new long[Security.Count];
            var prevvalues = new long[Security.Count];
            var rand = new Random();

            for (int i = 0; i < options.Iterations; i++)
            {
                // set parvalues to options.ParValue
                foreach (var security in Security.EnumerateAll())
                {
                    parvalues[(int)security.Name] = options.ParValue;
                    prevvalues[(int)security.Name] = 0;
                }

                // run the market with the specific policy
                config.Seed = rand.Next();
                var market = new Market(config);
                do
                {
                    // start
                    market.StartYear();

                    // gather stats
                    foreach (var security in Security.EnumerateAll())
                    {
                        // split
                        if (market.SplitSecurities.Contains(security.Name))
                        {
                            // adjust parvalue
                            parvalues[(int)security.Name] /= 2;
                            // stats
                            stats.Split[(int)security.Name]++;
                        }

                        // worthless
                        if (market.WorthlesSecurities.Contains(security.Name))
                        {
                            // adjust parvalue
                            parvalues[(int)security.Name] = options.ParValue;
                            // stats
                            stats.Worthless[(int)security.Name]++;
                        }

                        // stats
                        var price = market.Prices.ByName(security.Name);
                        if (price > parvalues[(int)security.Name]) stats.AbovePar[(int)security.Name]++;
                        if (price < parvalues[(int)security.Name]) stats.BelowPar[(int)security.Name]++;
                        if (prevvalues[(int)security.Name] > 0 && price > prevvalues[(int)security.Name]) stats.Increase[(int)security.Name]++;
                        if (prevvalues[(int)security.Name] > 0 && price < prevvalues[(int)security.Name]) stats.Decrease[(int)security.Name]++;
                        if (price <= options.NoDividendPrice || security.Yield <= 0) stats.NoDividend[(int)security.Name]++;

                        // set prevvalues for this security
                        prevvalues[(int)security.Name] = price;
                    }

                    // advance
                    market.SellSecurities(new List<Transaction>());
                    market.BuySecurities(new List<Transaction>());
                }
                while (market.EndYear());
            }

            return stats;
        }

        public static NeuralBotContainer[] RunIteratively(MarketConfiguration config, Options options, int keep)
        {
            // run multiple iterations, keeping the best models
            // return n of the best models after iterations

            var bots = new NeuralBotContainer[keep*4];
            var rand = new Random();

            // run each iteration
            for (int i = 0; i < options.Iterations; i++)
            {
                // random seed
                config.Seed = options.Seed != 0 ? options.Seed : rand.Next();

                // run all models
                Parallel.For(fromInclusive: 0, toExclusive: bots.Length, (i) =>
                {
                    if (bots[i] == null) bots[i] = new NeuralBotContainer(
                            new NeuralBotOptions()
                            {
                                AllowOnMargin = options.NeuralMargin,
                                LearningRate = options.NeuralLearingRate,
                                PcntRandomResults = options.NeuralRandomResults,
                                ShuffleBuySecurityOrder = !options.NeuralStaticSecurityOrder,
                                ShuffleSellSecurityOrder = !options.NeuralStaticSecurityOrder
                            }
                        );

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
    }
}
