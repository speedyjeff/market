using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// options - adjust starting stock price
//           buy any denomination of stock (not just by 10's)

namespace market.engine
{
    class Market
    {
        public Market(int seed = 0)
        {
            // init
            Rand = seed > 0 ? new Random(seed) : new Random();
            State = States.EstablishMarketSituation;
            Year = 1;

            // set all values to ParValue
            var deltas = new int[Security.Count];
            for (int i = 0; i < Security.Count; i++) deltas[i] = ParValue;
            Prices = new MarketPrices() { Deltas = deltas };

            // choose a starting market situation
            ChooseMarketSituation();

            // set par value prices
            SetMarketPrices();
        }

        public MarketPrices Prices { get; private set; }
        public SituationCard MarketSituation { get; private set; }
        public int Year { get; private set; }

        public bool StartYear(Player[] players)
        {
            if (State != States.CurrentMarketPrices) throw new Exception("invalid state");

            // stock split & worthless stock
            foreach(var security in Security.EnumerateAll())
            {
                var price = Prices.ByName(security.Name);

                // 2-for-1 split
                if (price >= StockSplitPrice)
                {
                    // halve the price (round up)
                    var half = (int)Math.Ceiling((float)price / 2f);
                    Prices.Add(security.Name, -1 * (price - half));
                    
                    // double the holdings per player 
                    foreach(var player in players)
                    {
                        var amount = player.Holdings.ByName(security.Name);
                        if (amount > 0) player.Holdings.Add(security.Name, amount);
                    }
                }

                // worthless
                else if (price <= WorthlessStockPrice)
                {
                    // all players lose their shares
                    foreach(var player in players)
                    {
                        var amount = player.Holdings.ByName(security.Name);
                        if (amount > 0) player.Holdings.Add(security.Name, -1 * amount);
                    }

                    // reset price to ParValue
                    Prices.Add(security.Name, ParValue - price);
                }
            }

            // apply any cash earnings
            // order matters a lot on where this happens
            // (this must happen after the prices reflect both the market situation and prices changes)
            foreach (var kvp in MarketSituation.Cash)
            {
                foreach (var player in players)
                {
                    var amount = player.Holdings.ByName(kvp.Key);
                    if (amount > 0) player.CashBalance += (amount * kvp.Value);
                }
            }

            // update state
            UpdateState(Year > 1 ? States.SellingSecurities : States.BuyingSecurities);

            return true;
        }


        public bool SellSecurity(List<Transaction> transactions)
        {
            if (Year == 1) return false;
            if (State != States.SellingSecurities) throw new Exception("invalid state");

            if (transactions != null && transactions.Count > 0)
            {
                foreach (var t in transactions)
                {
                    // validate that this is an valid transaction
                    if (t.Player == null) throw new Exception("invalid player");
                    if (t.Amount % 10 != 0 || t.Amount == 0) throw new Exception("invalid amount");
                    var amount = t.Player.Holdings.ByName(t.Security);
                    if (amount < t.Amount) throw new Exception("invalid amount");

                    // sell these shares and credit the money
                    var price = Prices.ByName(t.Security);
                    t.Player.CashBalance += (t.Amount * price);

                    // reduce the amount of shares
                    t.Player.Holdings.Add(t.Security, -1 * t.Amount);
                }
            }

            // update state
            UpdateState(States.BuyingSecurities);

            return true;
        }

        public bool BuySecurity(List<Transaction> transactions)
        {
            if (State != States.BuyingSecurities) throw new Exception("not valid to buy securities");

            if (transactions != null && transactions.Count > 0)
            {
                foreach(var t in transactions)
                {
                    // validate that this is an valid transaction
                    if (t.Player == null) throw new Exception("invalid player");
                    if (t.Amount % 10 != 0 || t.Amount == 0) throw new Exception("invalid amount");
                    var price = Prices.ByName(t.Security);
                    var cost = t.Amount * price;
                    if (cost > t.Player.CashBalance) throw new Exception("invalid amount");
                
                    // deduct the amount
                    t.Player.CashBalance -= cost;

                    // add the shares
                    t.Player.Holdings.Add(t.Security, t.Amount);
                }
            }

            // update state
            UpdateState(States.PostingDividensAndInterest);

            return true;
        }

        public bool EndYear(Player[] players)
        {
            if (State != States.PostingDividensAndInterest) throw new Exception("invalid state");

            // pay out dividends
            PostDividendsAndInterest(players);

            // update the Year
            Year++;

            // end of game
            if (Year > 10)
            {
                UpdateState(States.EndOfGame);
                return false;
            }

            // choose market situation
            ChooseMarketSituation();

            // set prices
            SetMarketPrices();

            return true;
        }

        #region private
        private Random Rand;
        private bool[] UsedSituations;
        private States State;

        // starting value for all security
        private const int ParValue = 100;
        private const int NoDividendPrice = 50;
        private const int StockSplitPrice = 150;
        private const int WorthlessStockPrice = 0;

        // Game state machine              User calls:
        // |-> EstablishMarketSituation   
        // |     |                           
        // |     \/                          
        // |  CurrentMarketPrices          StartDay     
        // |     |                           
        // |  (year 1)  (rest)               
        // |     |        |                  
        // |     |        \/                 
        // |     |   SellingSecurities     SellSecurities   
        // |     |        |                  
        // |     \/       \/                 
        // |  BuyingSecurities             BuySecurities   
        // |     |                           
        // |     \/                          
        // |-- PostingDividendsAndInterest EndDay
        //        |
        //    (Year > 10)
        //        |
        //        \/
        //    EndOfGame
        enum States { EstablishMarketSituation, CurrentMarketPrices, BuyingSecurities, PostingDividensAndInterest, SellingSecurities, EndOfGame }

        private void UpdateState(States nextState)
        {
            // check state transitions
            if (State == States.EstablishMarketSituation)
            {
                if (nextState != States.CurrentMarketPrices) throw new Exception("invalid state transition");
            }
            else if (State == States.CurrentMarketPrices)
            {
                if (Year == 1 && nextState != States.BuyingSecurities) throw new Exception("invalid state transition");
                else if (nextState != States.SellingSecurities) throw new Exception("invalid state transition");
            }
            else if (State == States.BuyingSecurities)
            {
                if (nextState != States.PostingDividensAndInterest) throw new Exception("invalid state transition");
            }
            else if (State == States.PostingDividensAndInterest)
            {
                if (Year > 10 && nextState != States.EndOfGame) throw new Exception("invalid state transition");
                if (nextState != States.EstablishMarketSituation) throw new Exception("invalid state transition");
            }
            else throw new Exception("invalid state transition");

            // change state
            State = nextState;
            return;
        }

        private void ChooseMarketSituation()
        {
            if (State != States.EstablishMarketSituation) throw new Exception("invalid state");

            // init
            if (UsedSituations == null) UsedSituations = new bool[SituationCard.Count];

            // choose an available situation
            var index = 0;
            do
            {
                index = Rand.Next() % UsedSituations.Length;
            }
            while (UsedSituations[index]);

            // claim it
            UsedSituations[index] = true;
            MarketSituation = SituationCard.ByIndex(index);

            // update prices, based on the market situation
            foreach (var kvp in MarketSituation.Price)
            {
                Prices.Add(kvp.Key, kvp.Value);
            }

            // update state
            UpdateState(States.CurrentMarketPrices);
        }

        private void SetMarketPrices()
        {
            if (State != States.CurrentMarketPrices) throw new Exception("invalid state");

            if (Year > 1)
            {
                // 2-12
                var diceroll = ((Rand.Next() % 10) + 2);
                var adjustments = MarketPriceCard.Generate(MarketSituation.Market, diceroll);

                // combine the prices
                Prices.Add(adjustments);
            }

            // no state update, waiting on user input
        }

        private void PostDividendsAndInterest(Player[] players)
        {
            // add dividends and interest (these are calculated based on the ParValue - not current value)
            foreach(var security in Security.EnumerateAll())
            {
                // check if there is interest/dividends
                if (security.Yield > 0)
                {
                    // stocks <$50 receive no dividend
                    var price = Prices.ByName(security.Name);
                    if (price >= NoDividendPrice)
                    {
                        foreach (var player in players)
                        {
                            var amount = player.Holdings.ByName(security.Name);
                            if (amount > 0)
                            {
                                // add ParValue (starting) dividend/interest
                                player.CashBalance += (int)((amount * ParValue) * ((float)security.Yield/100f));
                            }
                        } // foreach player
                    } // if (price >= NoDividendPrice)
                } // if (yield > 0)
            } // foreach security

            // update
            UpdateState(States.EstablishMarketSituation);
        }
        #endregion
    }
}
