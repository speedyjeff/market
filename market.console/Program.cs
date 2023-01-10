using market.engine;

namespace market.console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 0)
            {
                // running simulations
                RunAll(BaselinePolicy.AlwaysOnMargin, iterations: 100);
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
                var config = new MarketConfiguration() { Seed = rand.Next(), AdjustStartingPrices = true, LastYear = 10, WithDebugValidation = true };

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

            // share configuration
            Console.WriteLine($"seed is {market.Config.Seed}");
            Console.WriteLine($"stocks split at {market.Config.StockSplitPrice} provide no dividend at {market.Config.NoDividendPrice} and are worthless at {market.Config.WorthlessStockPrice}");
            Console.WriteLine($"all transactions must be divisible by {market.Config.PurchaseDivisor}");
            if (market.Config.AdjustStartingPrices) Console.WriteLine($"stocks starting price various from {market.Config.ParValue}");
            else Console.WriteLine($"stocks start at {market.Config.ParValue}");
            Console.WriteLine($"[optional] When buying stock on margin {100d / (double)market.Config.MarginSplitRatio:f0}% is bought on margin at {market.Config.MarginInterestDue}% per year");
            Console.WriteLine($" margin buys can only occur between years 2 and {market.Config.LastYear - 1} and must be paid in full when below {market.Config.MarginStockMustBeBought}");
            Console.WriteLine("  when selling/buying below, cash balance is only an estimate");
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
                Console.WriteLine($"Id\t{TrimName("Name",20)}\tYield\tPrice\tHolding\tCostBasis\tHoldingsOnMargin");
                foreach (var s in Security.EnumerateAll())
                {
                    Console.WriteLine($"{(int)s.Name}\t{TrimName(s.Fullname, 20)}\t{s.Yield}%\t{AdditionalInfo(market, s.Name)}${market.Prices.ByName(s.Name)}\t{market.My.Holdings.ByName(s.Name)}\t${TrimName(market.My.CostBasisByName(s.Name)+"",10)}\t{market.My.MarginTotalByName(s.Name)}");
                }
                Console.WriteLine($"cash: {AsMoney(market.My.CashBalance,10)}, net worth: {AsMoney(market.TotalNetWorth(),10)}");

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
            Console.WriteLine($"Year\t{TrimName("Security", 20)}\tAmount\tPrice\tCost\tDivInt\tMarginCharges\t{TrimName("Cash",10)}\tMarginTotal\tTransactionType");
            foreach(var row in market.My.RecordSheet)
            {
                var fullname = row.Name == SecurityNames.None ? "" : Security.ByName(row.Name).Fullname;
                Console.WriteLine($"{row.Year}\t{TrimName(fullname, 20)}\t{row.Amount}\t{AsMoney(row.Price,5)}\t{AsMoney(row.Cost,7)}\t{AsMoney(row.DividendInterest,6)}\t{AsMoney(row.MarginChargesPaid,10)}\t{AsMoney(row.CashBalance,10)}\t{AsMoney(row.MarginTotal,10)}\t{row.Type}");
            }
        }

        private static List<Transaction> GetUserInput(Market market, bool isbuy)
        {
            // skip the first selling, as the user does not have anything to sell
            if (!isbuy && market.Year == 1) return null;

            var transactions = new List<Transaction>();
            var cash = market.My.CashBalance;
            var sellamounts = isbuy ? null : new long[Security.Count];
            while(true)
            {
                Console.WriteLine($"{(isbuy ? "Buy" : "Sell")}: Select a security and amount 'id,amount': ({(isbuy ? "" : "~")}${cash}) ['enter' when done, 'u' to undo, prefix with 'm' for margin]");
                var line = Console.ReadLine();

                // check for exit
                if (string.IsNullOrWhiteSpace(line)) break;

                // check if on margin
                var onmargin = false;
                if (line.Contains("m", StringComparison.OrdinalIgnoreCase))
                {
                    if (market.Year == 1 || market.Year == market.Config.LastYear)
                    {
                        Console.WriteLine(" * cannot purchase on margin on the first and last year");
                        continue;
                    }
                    onmargin = true;
                    line = line.Replace("m", "").Replace("M", "");
                }

                // undo
                if (line.Trim().Equals("u", StringComparison.OrdinalIgnoreCase))
                {
                    if (transactions.Count == 0) continue;

                    // undo the local tracking
                    var lasttransacton = transactions[transactions.Count - 1];

                    // add back to the local cash balance
                    if (isbuy)
                    {
                        var cost = lasttransacton.Amount * market.Prices.ByName(lasttransacton.Security);
                        if (lasttransacton.OnMargin) cost /= market.Config.MarginSplitRatio;
                        cash += cost;
                    }
                    // add the amount back to the local amount tracking
                    else
                    {
                        cash -= (lasttransacton.Amount * market.Prices.ByName(lasttransacton.Security));
                        sellamounts[(int)lasttransacton.Security] -= lasttransacton.Amount;
                    }

                    // remove last transaction
                    transactions.RemoveAt( transactions.Count - 1 );

                    continue;
                }

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
                            if (Int64.TryParse(parts[1], out long amount))
                            {
                                if (amount > 0 && (amount % market.Config.PurchaseDivisor) == 0)
                                {
                                    // check if the transaction is viable
                                    if (isbuy)
                                    {
                                        // check that there is enough money
                                        var price = market.Prices.ByName(name);
                                        var cost = (price * amount);
                                        if (onmargin) cost /= market.Config.MarginSplitRatio;
                                        if (cash >= cost)
                                        {
                                            // valid
                                            cash -= cost;
                                            transactions.Add(new Transaction() { Security = name, Amount = amount, OnMargin = onmargin });
                                        }
                                        else
                                        {
                                            Console.WriteLine(" * not enough cash balance");
                                        }
                                    }  
                                    else
                                    {
                                        // check that we hold this amount
                                        var holding = market.My.Holdings.ByName(name) - sellamounts[(int)name];
                                        if (amount <= holding)
                                        {
                                            // valid
                                            var price = market.Prices.ByName(name);
                                            var cost = (price * amount);
                                            if (onmargin) cost /= market.Config.MarginSplitRatio;
                                            cash += cost;
                                            sellamounts[(int)name] += amount;
                                            transactions.Add(new Transaction() { Security = name, Amount = amount, OnMargin = onmargin });
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

        private static string AdditionalInfo(Market market, SecurityNames name)
        {
            if (market.SplitSecurities.Contains(name)) return "S";
            else if (market.WorthlesSecurities.Contains(name)) return "X";
            else return "";
        }

        private static string AsMoney(long value, int length)
        {
            if (value >= 0) return TrimName($" ${value}", length);
            else return TrimName($"-${-1 * value}", length);
        }
        #endregion
    }
}