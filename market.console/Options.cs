using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.console
{
    enum Modes { RunOnce = 0, RunLoop = 1 };

    class Options
    {
        public bool Help { get; set; }
        public Modes Mode { get; set; }
        public BaselinePolicy Policy { get; set; }
        public int Iterations { get; set; }

        public int Seed { get; set; }
        public long ParValue { get; set; }
        public long NoDividendPrice { get; set; }
        public long StockSplitPrice { get; set; }
        public long WorthlessStockPrice { get; set; }
        public int LastYear { get; set; }
        public long PurchaseDivisor { get; set; }
        public bool AdjustStartingPrices { get; set; }
        public long InitialCashBalance { get; set; }
        public bool DividendBasedOnMarketPrice { get; set; }
        public long TransactionFee { get; set; }

        public long MarginInterestDue { get; set; }
        public long MarginSplitRatio { get; set; }
        public long MarginStockMustBeBought { get; set; }

        public bool NeuralMargin { get; set; }
        public float NeuralLearingRate { get; set; }
        public bool NeuralStaticSecurityOrder { get; set; }
        public float NeuralRandomResults { get; set; }

        public bool WithDebugValidation { get; set; }

        public static void ShowHelp()
        {
            Console.WriteLine("./market.console");
            Console.WriteLine(" -(h)elp                       - this help");
            Console.WriteLine(" -(m)ode                       - play an interactive game (0) or simulate multiple games (1) (default 0)");
            Console.WriteLine(" -(po)licy                     - 0: always buy (default)");
            Console.WriteLine("                                 1: always buy low");
            Console.WriteLine("                                 2: always buy margin");
            Console.WriteLine("                                 3: learning neural bots");
            Console.WriteLine("                                 4: random");
            Console.WriteLine("                                 5: stats only");
            Console.WriteLine(" -(it)erations                 - number of loops when policy is not 0 (default 1000)");
            Console.WriteLine(" -(se)ed                       - pseudo random seed (default 0 - eg. random)");
            Console.WriteLine(" -(pa)rvalue                   - starting stock price (default $100");
            Console.WriteLine(" -(no)DividendPrice            - price where stocks do not provide dividends (default $50)");
            Console.WriteLine(" -(st)ockSplitPrice            - price at stock split (default $150)");
            Console.WriteLine(" -(wo)rthlessStockPrice        - price at which companies are bankrupt and all shares liquidated (default $0)");
            Console.WriteLine(" -(l)astYear                   - game starts at year 1 and end at last year (default 10)");
            Console.WriteLine(" -(pu)rchaseDivisor            - divisor for chunks of shares (default 10 - eg. must buy chunks of 10)");
            Console.WriteLine(" -(a)djustStartingPrices       - adjust initial stock prices from parvalue (default not set)");
            Console.WriteLine(" -(in)itialCashBalance         - initial cash balance (default $5000)");
            Console.WriteLine(" -(d)ividendBasedOnMarketPrice - compute dividend based on market prices not parvalue (default not set)");
            Console.WriteLine(" -(t)ransactionfee             - cost per buy/sell (default $0)");
            Console.WriteLine(" -(marginI)nterestDue          - annual interest due on margin total (default 5%)");
            Console.WriteLine(" -(marginSp)litRatio           - divisor of how much stock is bought up front (default 2 - eg. 50%)");
            Console.WriteLine(" -(marginSt)ockMustBeBought    - stock price at which margin purchases must pay up (default $25)");
            Console.WriteLine(" -(wi)thDebugValidation        - turn on internal validation (default not set)");
            Console.WriteLine(" Neural bot options:");
            Console.WriteLine("  -(neuralm)argin              - allow bots to do margin trading (default false)");
            Console.WriteLine("  -(neurall)earningrate        - set learning rate for neural bots (default 0.00015)");
            Console.WriteLine("  -(neurals)taticsecurityorder - keep security order static (default false)");
            Console.WriteLine("  -(neuralr)andomresults       - 0 (no random) to 1 (full random) injection of results (default 0)");
        }

        public static Options Parse(string[] args)
        {
            var options = new Options()
            {
                Help = false,
                Mode = Modes.RunOnce,
                Policy = BaselinePolicy.AlwaysBuy,
                Seed = 0,
                ParValue = 100,
                NoDividendPrice = 50,
                StockSplitPrice = 150,
                WorthlessStockPrice = 0,
                LastYear = 10,
                PurchaseDivisor = 10,
                AdjustStartingPrices = false,
                InitialCashBalance = 5000,
                DividendBasedOnMarketPrice = false,
                MarginInterestDue = 5,
                MarginSplitRatio = 2,
                MarginStockMustBeBought = 25,
                WithDebugValidation = false,
                NeuralMargin = false,
                NeuralLearingRate = 0.00015f,
                NeuralStaticSecurityOrder = false,
                NeuralRandomResults = 0f
            };

            for(int i=0; i<args.Length; i++)
            {
                if (args[i].StartsWith("-h", StringComparison.OrdinalIgnoreCase) || args[i].StartsWith("-?"))
                {
                    options.Help = true;
                }
                else if (args[i].StartsWith("-m",StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.Mode = (Modes)Int32.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-po", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i+1) < args.Length) options.Policy = (BaselinePolicy)Int32.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-it", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.Iterations = Int32.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-se", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.Seed = Int32.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-pa", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.ParValue = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-no", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.NoDividendPrice = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-st", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.StockSplitPrice = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-wo", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.WorthlessStockPrice = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-l", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.LastYear = Int32.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-pu", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.PurchaseDivisor = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-a", StringComparison.OrdinalIgnoreCase))
                {
                    options.AdjustStartingPrices = true;
                }
                else if (args[i].StartsWith("-in", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.InitialCashBalance = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-d", StringComparison.OrdinalIgnoreCase))
                {
                    options.DividendBasedOnMarketPrice = true;
                }
                else if (args[i].StartsWith("-t", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.TransactionFee = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-margini", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.MarginInterestDue = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-marginst", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.MarginStockMustBeBought = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-marginsp", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.MarginSplitRatio = Int64.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-neuralm", StringComparison.OrdinalIgnoreCase))
                {
                    options.NeuralMargin = true;
                }
                else if (args[i].StartsWith("-neurall", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.NeuralLearingRate = Single.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-neurals", StringComparison.OrdinalIgnoreCase))
                {
                    options.NeuralStaticSecurityOrder = true;
                }
                else if (args[i].StartsWith("-neuralr", StringComparison.OrdinalIgnoreCase))
                {
                    if ((i + 1) < args.Length) options.NeuralRandomResults = Single.Parse(args[++i]);
                }
                else if (args[i].StartsWith("-wi", StringComparison.OrdinalIgnoreCase))
                {
                    options.WithDebugValidation = true;
                }
                else
                {
                    Console.WriteLine($"unknown command : {args[i]}");
                }
            }

            if (options.NeuralRandomResults < 0 || options.NeuralRandomResults > 1f)
            {
                Console.WriteLine(" * learning rate must be between 0 and 1");
                options.Help = true;
            }

            return options;
        }
    }
}
