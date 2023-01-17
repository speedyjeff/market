using Learning;
using market.engine;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace market.bots
{
    public class NeuralBot
    {
        public NeuralBot(NeuralBotOptions options)
        {
            // init
            Options = options;
            BuySecurityOrder = options.ShuffleBuySecurityOrder ? CreateShuffledList(Security.Count) : CreateList(Security.Count);
            SellSecurityOrder = options.ShuffleSellSecurityOrder ? CreateShuffledList(Security.Count) : CreateList(Security.Count);
            InputCount = ConstInputCount;
            OutputCount = (Options.AllowOnMargin ? ConstOutputOnMarginCount : ConstOutputCount);

            // initialize individual networks
            BuyNetworks = new NeuralNetwork[Security.Count];
            SellNetworks = new NeuralNetwork[Security.Count];
            foreach (var security in Security.EnumerateAll())
            {
                BuyNetworks[(int)security.Name] = new NeuralNetwork(
                    new NeuralOptions()
                    {
                        InputNumber = InputCount,
                        OutputNumber = OutputCount,
                        HiddenLayerNumber = Options.HiddenNetworks,
                        LearningRate = Options.LearningRate,
                        MinibatchCount = Options.MiniBatchCount
                    });
                SellNetworks[(int)security.Name] = new NeuralNetwork(
                    new NeuralOptions()
                    {
                        InputNumber = InputCount,
                        OutputNumber = OutputCount,
                        HiddenLayerNumber = Options.HiddenNetworks,
                        LearningRate = Options.LearningRate,
                        MinibatchCount = Options.MiniBatchCount
                    });
            }
        }

        public int[] BuySecurityOrder { get; private set; }
        public int[] SellSecurityOrder { get; private set; }

        public List<Transaction> Buy(Market market)
        {
            return CreateOrder(Options, BuySecurityOrder, BuyNetworks, market, isbuy: true);
        }

        public List<Transaction> Sell(Market market)
        {
            return CreateOrder(Options, SellSecurityOrder, SellNetworks, market, isbuy: false);
        }

        #region private
        private static RandomNumberGenerator Random;
        private NeuralBotOptions Options;
        private NeuralNetwork[] BuyNetworks;
        private NeuralNetwork[] SellNetworks;
        private int InputCount;
        private int OutputCount;

        private enum Outputs { Zero = 0, Ten = 1, Twenty = 2, FiftyPercent = 3, HundredPercent = 4, TenOnMargin = 5, TwentyOnMargin = 6, FiftyPercentOnMargin = 7, HundredPercentOnMargin = 8 };

        private const int ConstInputCount = 16;
        private const int ConstOutputCount = 5;
        private const int ConstOutputOnMarginCount = 9;

        static NeuralBot()
        {
            Random = RandomNumberGenerator.Create();
        }

        private static int GetZeroBasedRandom(int exclusiveTo)
        {
            var int32buffer = new byte[4];
            Random.GetBytes(int32buffer);
            // ensure positive
            int32buffer[3] &= 0x7f;
            var number = BitConverter.ToInt32(int32buffer);
            // get a random float between 0 and exclusiveTo
            return (int)Math.Floor(Math.Abs(((float)number / (float)Int32.MaxValue) - 0.00001f) * exclusiveTo);
        }

        private static int[] CreateShuffledList(int length)
        {
            // shuffle the contents of the array
            var indexes = new int[length];
            for (int i = 0; i < indexes.Length; i++) indexes[i] = i;
            for (int i=0; i<indexes.Length; i++)
            {
                var j = i;
                while (j == i) j = GetZeroBasedRandom(indexes.Length);
                var tmp = indexes[i];
                indexes[i] = indexes[j];
                indexes[j] = tmp;
            }
            return indexes;
        }

        private static int[] CreateList(int length)
        {
            // create index array
            var indexes = new int[length];
            for (int i = 0; i < indexes.Length; i++) indexes[i] = i;
            return indexes;
        }

        private static float[] ToInput(Market market, SecurityNames name, int inputCount)
        {
            // cash balance
            // security price
            // security amount
            // cost basis
            // margin holding
            // bull/bear
            // each security price

            // todo - holding for each security, inverted value for stock (low better)

            var input = new float[inputCount];

            // each security price
            var index = 6;
            var maxholding = 1L;
            foreach (var security in Security.EnumerateAll())
            {
                // capture the price
                input[index++] = (float)market.Prices.ByName(security.Name) / (float)market.Config.StockSplitPrice;

                // find the max holding
                maxholding = Math.Max(market.My.Holdings.ByName(security.Name), maxholding);
            }

            // cash balance
            input[0] = (float)market.My.CashBalance / (float)market.Config.InitialCashBalance;
            if (input[0] > 1f) input[0] = 1f;
            if (input[0] < -1f) input[0] = -1f;
            // security price
            input[1] = (float)market.Prices.ByName(name) / (float)market.Config.StockSplitPrice;
            // security amount
            input[2] = (float)market.My.Holdings.ByName(name) / (float)maxholding;
            // cost basis
            input[3] = (float)market.My.CostBasisByName(name) / (float)market.Config.StockSplitPrice;
            // margin holding
            input[4] = market.My.MarginTotalByName(name) / (float)maxholding;
            // bear (0) / bull (1)
            input[5] = market.MarketSituation.Market == Situation.Bull ? 1f : 0f;

            return input;
        }

        private static List<Transaction> CreateOrder(NeuralBotOptions options, int[] indexes, NeuralNetwork[] networks, Market market, bool isbuy)
        {
            // run networks for each security
            var outputs = new NeuralOutput[networks.Length];
            Parallel.For(fromInclusive: 0, toExclusive: networks.Length, (i) =>
            {
                outputs[i] = networks[i].Evaluate(ToInput(market, (SecurityNames)i, networks[i].InputNumber));
            });

            // create share transactions
            var transactions = new List<Transaction>();
            var cash = market.My.CashBalance;
            foreach (var index in indexes)
            {
                // get prices
                var price = market.Prices.ByName((SecurityNames)index);
                var amount = 0L;
                var result = (Outputs)outputs[index].Result;

                // inject random choices
                if (options.PcntRandomResults > 0 && GetZeroBasedRandom(exclusiveTo: 101) < (int)(100*options.PcntRandomResults)) result = (Outputs)GetZeroBasedRandom(networks[index].OutputNumber);

                // make the selection
                var onmargin = false;
                switch (result)
                {
                    case Outputs.Zero: amount = 0L; break;
                    case Outputs.Ten: amount = 10L; break;
                    case Outputs.Twenty: amount = 20L; break;
                    case Outputs.FiftyPercent:
                        amount = (long)Math.Floor((double)cash / (2d * (double)price));
                        break;
                    case Outputs.HundredPercent:
                        amount = (long)Math.Floor((double)cash / (double)price);
                        break;
                    case Outputs.TenOnMargin:
                        amount = 10L;
                        onmargin = true;
                        break;
                    case Outputs.TwentyOnMargin: 
                        amount = 20L;
                        onmargin = true;
                        break;
                    case Outputs.FiftyPercentOnMargin:
                        amount = (long)Math.Floor((double)cash / (2d * (double)price));
                        onmargin = true;
                        break;
                    case Outputs.HundredPercentOnMargin:
                        amount = (long)Math.Floor((double)cash / (double)price);
                        onmargin = true;
                        break;
                    default: throw new Exception("unknown output");
                }

                // ensure a round divisor
                amount -= (amount % market.Config.PurchaseDivisor);

                // check for margin buy rules
                if (isbuy && (market.Year == 1 || market.Year == market.Config.LastYear) && onmargin) amount = 0;

                // check if we have the cash to purchase
                var successful = false;
                if (amount > 0)
                {
                    if (isbuy)
                    {
                        // check if there are cash funds
                        var cost = price * amount;
                        if (onmargin) cost /= market.Config.MarginSplitRatio;
                        if (cash >= cost)
                        {
                            transactions.Add(new Transaction() { Security = (SecurityNames)index, Amount = amount, OnMargin = onmargin });
                            cash -= cost;
                            successful = true;
                        }
                    }
                    else
                    {
                        // check if we can sell this number of shares
                        var holding = market.My.Holdings.ByName((SecurityNames)index);
                        if (amount <= holding)
                        {
                            // do not need to track local holdings, as this security is only considered once per iteration
                            transactions.Add(new Transaction() { Security = (SecurityNames)index, Amount = amount });
                            successful = true;
                        }
                    }
                }

                // train
                if (!successful || amount == 0) result = Outputs.Zero;
                networks[index].Learn(outputs[index], (int)result);
            } // foreach index

            return transactions;
        }
        #endregion
    }
}
