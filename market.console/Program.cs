using market.engine;
using Microsoft.VisualBasic;
using System.Net.Http.Headers;

namespace market.console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
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

            // show table of results
            foreach (var security in Security.EnumerateAll())
            {
                Console.WriteLine($"{TrimName(security.Fullname, 20)}\t{policy}\t${sum[(int)security.Name].Average():f0}\t${sum[(int)security.Name].Min()}\t${sum[(int)security.Name].Max()}");
            }
        }

        private static void RunOnce()
        {
            var rand = new Random();
            var config = new MarketConfiguration() { Seed = rand.Next(), WithDebugValidation = true };
            var market = new Market(config);

            Console.WriteLine($"seed = {market.Config.Seed}");
            Console.WriteLine();

            Console.WriteLine("Available securities:");
            foreach (var s in Security.EnumerateAll())
            {
                Console.WriteLine($"{TrimName(s.Fullname, 30)}\tYield: {s.Yield}%");
                Console.WriteLine($"   {s.Description}");
            }
            Console.WriteLine();

            // play the game
            do
            {
                // start
                market.StartYear();

                // display market information
                Console.WriteLine($"Year {market.Year} of {market.Config.LastYear}");
                Console.WriteLine($"{market.MarketSituation.Market}");
                Console.WriteLine($"   {market.MarketSituation.Description}");
                foreach (var p in market.MarketSituation.Price)
                {
                    Console.WriteLine($"    Adjustment: {Security.ByName(p.Key).Fullname} {p.Value}");
                }
                foreach (var c in market.MarketSituation.Cash)
                {
                    Console.WriteLine($"    Cash dividend (end of year): {Security.ByName(c.Key).Fullname} ${c.Value}");
                }
                Console.WriteLine("Current market prices:");
                Console.WriteLine("Id\tName\tYield\tPrice\tHolding\tCostBasis");
                foreach (var s in Security.EnumerateAll())
                {
                    Console.WriteLine($"{(int)s.Name}\t{TrimName(s.Fullname, 20)}\t{s.Yield}%\t${market.Prices.ByName(s.Name)}\t{market.My.Holdings.ByName(s.Name)}\t${market.My.CostBasisByName(s.Name)}");
                }
                Console.WriteLine($"${market.My.CashBalance} (net worth: ${market.TotalNetWorth()})");

                // sell
                var transactions = GetUserInput(market, isbuy: false);
                market.SellSecurities(transactions);

                // buy
                transactions = GetUserInput(market, isbuy: true);
                market.BuySecurities(transactions);
            }
            while (market.EndYear());

            // calculate total net worth
            Console.WriteLine($"final net worth: ${market.TotalNetWorth()}");

            // display ledger
            Console.WriteLine("Year\tSecurity\tAmount\tPrice\tCost\tDivInt\tCash\tTransactionType");
            foreach(var row in market.My.RecordSheet)
            {
                var fullname = row.Name == SecurityNames.None ? "" : Security.ByName(row.Name).Fullname;
                Console.WriteLine($"{row.Year}\t{TrimName(fullname, 20)}\t{row.Amount}\t{row.Price}\t{row.Cost}\t{row.DividendInterest}\t{row.CashBalance}\t{row.Type}");
            }
        }

        private static List<Transaction> GetUserInput(Market market, bool isbuy)
        {
            // skip the first selling, as the user does not have anything to sell
            if (!isbuy && market.Year == 1) return null;

            var transactions = new List<Transaction>();
            var cash = market.My.CashBalance;
            while(cash > 0)
            {
                if (isbuy) Console.WriteLine($"Buy: Select a security and amount 'id,amount': (${cash}) [enter when done]");
                else Console.WriteLine($"Sell: Select a security and amount 'id,amount': [enter when done]");
                var line = Console.ReadLine();

                // check for exit
                if (string.IsNullOrWhiteSpace(line)) break;

                // parse input
                var parts = line.Split(',');
                if (parts.Length == 2)
                {
                    if (Int32.TryParse(parts[0], out int id))
                    {
                        // check that this is a valid id
                        if (id >= 0 && id < Security.Count)
                        {
                            var name = (SecurityNames)id;

                            // check if valid amount
                            if (Int32.TryParse(parts[1], out int amount))
                            {
                                if (amount > 0 && (amount % market.Config.PurchaseDivisor) == 0)
                                {
                                    // check if the transaction is viable
                                    if (isbuy)
                                    {
                                        // check that there is enough money
                                        var price = market.Prices.ByName(name);
                                        var cost = (price * amount);
                                        if (cash >= cost)
                                        {
                                            // valid
                                            cash -= cost;
                                            transactions.Add(new Transaction() { Security = name, Amount = amount });
                                        }
                                        else
                                        {
                                            Console.WriteLine(" * not enough cash balance");
                                        }
                                    }  
                                    else
                                    {
                                        // check that we hold this amount
                                        var holding = market.My.Holdings.ByName(name);
                                        if (amount <= holding)
                                        {
                                            // valid
                                            transactions.Add(new Transaction() { Security = name, Amount = amount });
                                        }
                                        else
                                        {
                                            Console.WriteLine(" * not enough shares");
                                        }
                                    }
                                } // if amount > 0 and divisor
                                else
                                {
                                    Console.WriteLine($" * invalid amount (ensure divisible by {market.Config.PurchaseDivisor})");
                                }
                            } // if parse amount
                            else
                            {
                                Console.WriteLine(" * invalid amount");
                            }
                        } // if valid securityname
                        else
                        {
                            Console.WriteLine(" * invalid id");
                        }
                    } // if parse id
                    else
                    {
                        Console.WriteLine(" * invalid id");
                    }
                } // if parts.length == 2
                else
                {
                    Console.WriteLine(" * must pass in a pair 'id,amount' (no quotes)");
                }
            }

            return transactions;
        }

        private static string TrimName(string name, int length)
        {
            if (name.Length == length) return name;
            else if (name.Length > length) return name.Substring(0, length);
            else return $"{name}{new string(' ', length-name.Length)}";
        }
        #endregion
    }
}