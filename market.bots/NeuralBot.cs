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
        public NeuralBot()
        {
            // init
            BuySecurityOrder = CreateShuffledList(Security.Count);
            SellSecurityOrder = CreateShuffledList(Security.Count);

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
                        HiddenLayerNumber = new int[] { 16 },
                        LearningRate = 0.00015f,
                        MinibatchCount = 1
                    });
                SellNetworks[(int)security.Name] = new NeuralNetwork(
                    new NeuralOptions()
                    {
                        InputNumber = InputCount,
                        OutputNumber = OutputCount,
                        HiddenLayerNumber = new int[] { 16 },
                        LearningRate = 0.00015f,
                        MinibatchCount = 1
                    });
            }
        }

        public int[] BuySecurityOrder { get; private set; }
        public int[] SellSecurityOrder { get; private set; }

        public List<Transaction> Buy(Market market)
        {
            return CreateOrder(BuySecurityOrder, BuyNetworks, market, isbuy: true);
        }

        public List<Transaction> Sell(Market market)
        {
            return CreateOrder(SellSecurityOrder, SellNetworks, market, isbuy: false);
        }

        #region private
        private static RandomNumberGenerator Random;
        private NeuralNetwork[] BuyNetworks;
        private NeuralNetwork[] SellNetworks;

        // todo margin

        private enum Outputs { Zero = 0, Ten = 1, Twenty = 2, FiftyPercent = 3, HundredPercent = 4 };

        private const int InputCount = 15;
        private const int OutputCount = 5;

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

        private static float[] ToInput(Market market, SecurityNames name)
        {
            // cash balance
            // security price
            // security amount
            // cost basis
            // margin holding
            // each security price

            var input = new float[InputCount];

            // each security price
            var index = 5;
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
            // security price
            input[1] = (float)market.Prices.ByName(name) / (float)market.Config.StockSplitPrice;
            // security amount
            input[2] = (float)market.My.Holdings.ByName(name) / (float)maxholding;
            // cost basis
            input[3] = (float)market.My.CostBasisByName(name) / (float)market.Config.StockSplitPrice;
            // margin holding
            input[4] = market.My.MarginTotalByName(name) / (float)maxholding;

            return input;
        }

        private static List<Transaction> CreateOrder(int[] indexes, NeuralNetwork[] networks, Market market, bool isbuy)
        {
            // run networks for each security
            var outputs = new NeuralOutput[networks.Length];
            Parallel.For(fromInclusive: 0, toExclusive: networks.Length, (i) =>
            {
                outputs[i] =networks[i].Evaluate(ToInput(market, (SecurityNames)i));
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
                if (GetZeroBasedRandom(exclusiveTo: 11) < 3) result = (Outputs)GetZeroBasedRandom(OutputCount);

                // make the selection
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
                    default: throw new Exception("unknown output");
                }

                // ensure a round divisor
                amount -= (amount % market.Config.PurchaseDivisor);

                // check if we have the cash to purchase
                var successful = false;
                if (amount > 0)
                {
                    if (isbuy)
                    {
                        // check if there are cash funds
                        var cost = price * amount;
                        if (cash >= cost)
                        {
                            transactions.Add(new Transaction() { Security = (SecurityNames)index, Amount = amount });
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
                if (!successful) result = Outputs.Zero;
                networks[index].Learn(outputs[index], (int)result);
            } // foreach index

            return transactions;
        }
        #endregion
    }
}
