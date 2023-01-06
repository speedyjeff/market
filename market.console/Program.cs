using market.engine;

namespace market.console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length> 0)
            {
                // running simulations
                RunAll(BaselinePolicy.AlwaysBuy, iterations: 100);
            }
            else
            {
                // running one round
                RunOnce();
            }
        }

        #region private
        private static void RunAll(BaselinePolicy policy, int iterations)
        {
            var rand = new Random();

            // run the market with default policies for all securities
            var sum = new List<long>[Security.Count];
            for (int i = 0; i < iterations; i++)
            {
                var config = new MarketConfiguration() { Seed = rand.Next(), AdjustStartingPrices = true, LastYear = 10 };

                foreach (var security in Security.EnumerateAll())
                {
                    if (sum[(int)security.Name] == null) sum[(int)security.Name] = new List<long>();
                    var market = BaselineMarket.Run(config, security.Name, policy);
                    sum[(int)security.Name].Add(market.TotalNetWorth());
                }
            }

            foreach (var security in Security.EnumerateAll())
            {
                Console.WriteLine($"{TrimName(security.Fullname, 20)}\t{policy}\t${sum[(int)security.Name].Average():f0}\t${sum[(int)security.Name].Min()}\t${sum[(int)security.Name].Max()}");
            }
        }

        private static void RunOnce()
        {
            var rand = new Random();
            var config = new MarketConfiguration() { Seed = rand.Next() };
            var market = new Market(config);

            Console.WriteLine($"seed = {market.Config.Seed}");
            do
            {
                // start
                market.StartYear();

                // display market information
                Console.WriteLine($"Year {market.Year} / {market.Config.LastYear}");
                Console.WriteLine($"{market.MarketSituation.Market}");
                Console.WriteLine($"   {market.MarketSituation.Description}");
                foreach (var p in market.MarketSituation.Price)
                {
                    Console.WriteLine($"    Adjustment: {Security.ByName(p.Key).Fullname} {p.Value}");
                }
                foreach (var c in market.MarketSituation.Cash)
                {
                    Console.WriteLine($"    Cash dividend: {Security.ByName(c.Key).Fullname} ${c.Value}");
                }
                foreach (var s in Security.EnumerateAll())
                {
                    Console.WriteLine($"{TrimName(s.Fullname, 20)}\t${market.Prices.ByName(s.Name)}\t{market.My.Holdings.ByName(s.Name)}");
                }
                Console.WriteLine($"${market.My.CashBalance} (net worth: ${market.TotalNetWorth()})");

                Console.ReadLine();

                // sell
                market.SellSecurities(new List<Transaction>());

                // buy
                var price = market.Prices.ByName(SecurityNames.CentralCityMunicipalBonds);
                var amount = (int)(market.My.CashBalance / price);
                // ensure round number
                amount -= (amount % market.Config.PurchaseDivisor);
                if (amount > 0)
                {
                    market.BuySecurities(new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Security = SecurityNames.CentralCityMunicipalBonds,
                            Amount = amount
                        }
                    });
                }
                else market.BuySecurities(new List<Transaction>());
            }
            while (market.EndYear());

            // calculate total net worth
            Console.WriteLine($"final net worth: ${market.TotalNetWorth()}");

            // display ledger
            foreach(var row in market.My.RecordSheet)
            {
                var fullname = row.Name == SecurityNames.None ? "                    " : Security.ByName(row.Name).Fullname;
                Console.WriteLine($"{row.Year}\t{TrimName(fullname, 20)}\t{row.Amount}\t{row.Price}\t{row.Cost}\t{row.DividendInterest}\t{row.CashBalance}");
            }
        }

        private static string TrimName(string name, int length)
        {
            return (name.Length > length ? name.Substring(0, length) : name);
        }
        #endregion
    }
}