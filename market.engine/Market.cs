using market.engine.tests;

namespace market.engine
{
    public class Market
    {
        public Market(MarketConfiguration config)
        {
            // inline validation
            if (WithDebugValidation) MainTest.Run();

            // sanity check the configs
            Config = config;
            if (Config.LastYear < 1) throw new Exception("invalid max years");
            if (Config.ParValue < 0) throw new Exception("invalid parvalue");
            if (Config.NoDividendPrice < 0) throw new Exception("invalid no dividend price");
            if (Config.PurchaseDivisor <= 0) throw new Exception("invalid purchase divisor");
            if (Config.StockSplitPrice < 0) throw new Exception("invalid stock split price");
            if (Config.WorthlessStockPrice < 0) throw new Exception("invalid worthless stock price");
            if (Config.InitialCashBalance < 0) throw new Exception("invalid initial cash balance");

            // init
            My = new Player(Config.InitialCashBalance);
            State = States.EstablishMarketSituation;
            Year = 1;

            // initialize random - all random is pseudo and done up front
            var rand = new Random(Config.Seed);
            // two independent dice rolls (two per year)
            RandomDiceRollIndex = 0;
            RandomDiceRolls = RandomValues(rand, length: Config.LastYear * 2, inclusiveFrom: 1, inclusiveTo: 6, allowdups: true);
            RandomSituationIndex = 0;
            RandomSituations = RandomValues(rand, length: Config.LastYear, inclusiveFrom: 0, inclusiveTo: SituationCard.Count-1, allowdups: false);

            // if MaxYears > SituationCards.Count - the cards are reused from the start

            // set all values to ParValue
            Prices = new SecuritiesContainer();
            foreach(var security in Security.EnumerateAll())
            {
                Prices.Add(security.Name, Config.ParValue);
            }
        }

        public MarketConfiguration Config { get; private set; }
        public SecuritiesContainer Prices { get; private set; }
        public SituationCard MarketSituation { get; private set; }
        public int Year { get; private set; }
        public Player My { get; private set; }

        public long TotalNetWorth()
        {
            // calculate the total net worth of the player
            var total = My.CashBalance;
            foreach(var security in Security.EnumerateAll())
            {
                var amount = My.Holdings.ByName(security.Name);
                var price = Prices.ByName(security.Name);
                total += ((long)amount * (long)price);
            }

            if (WithDebugValidation && total < 0) throw new Exception("invalid net worth");

            return total;
        }

        public bool StartYear()
        {
            // choose market situation
            ChooseMarketSituation();

            // set prices
            SetMarketPrices();

            return true;
        }

        public bool SellSecurities(List<Transaction> transactions)
        {
            if (Year == 1) return false;
            return SellingSecuritiesAtMarketPrice(transactions);
        }

        public bool BuySecurities(List<Transaction> transactions)
        {
            return BuyingSecuritiesAtMarketPrice(transactions);
        }

        public bool EndYear()
        {
            // pay out dividends
            PostDividendsAndInterest();

            // update the Year
            Year++;

            // end of game
            if (Year > Config.LastYear)
            {
                if (State != States.EndOfGame) throw new Exception("invalid state");
                return false;
            }

            return true;
        }

        #region private
        private States State;

        private int[] RandomDiceRolls;
        private int RandomDiceRollIndex;
        private int[] RandomSituations;
        private int RandomSituationIndex;

        private const bool WithDebugValidation = true;

        //
        // State machine
        //

        // Game state machine              User calls:
        // |-> EstablishMarketSituation   
        // |     |                           
        // |     \/                          
        // |  CurrentMarketPrices          StartYear  
        // |     |        |                  
        // |  (year 1)  (rest)               
        // |     |        |                  
        // |     |        \/                 
        // |     |   SellingSecurities     SellSecurities   
        // |     |        |                  
        // |     \/       \/                 
        // |  BuyingSecurities             BuySecurities   
        // |     |                           
        // |     \/                          
        // |-- PostingDividendsAndInterest EndYear
        //        |
        //    (Year > 10)
        //        |
        //        \/
        //    EndOfGame
        enum States { EstablishMarketSituation, CurrentMarketPrices, BuyingSecurities, PostingDividendsAndInterest, SellingSecurities, EndOfGame }

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
                else if (Year > 1 && nextState != States.SellingSecurities) throw new Exception("invalid state transition");
            }
            else if (State == States.SellingSecurities)
            {
                if (nextState != States.BuyingSecurities) throw new Exception("invalid state transition");
            }
            else if (State == States.BuyingSecurities)
            {
                if (nextState != States.PostingDividendsAndInterest) throw new Exception("invalid state transition");
            }
            else if (State == States.PostingDividendsAndInterest)
            {
                if (Year >= Config.LastYear && nextState != States.EndOfGame) throw new Exception("invalid state transition");
                if (Year < Config.LastYear && nextState != States.EstablishMarketSituation) throw new Exception("invalid state transition");
            }
            else throw new Exception("invalid state transition");

            // change state
            State = nextState;
            return;
        }

        //
        // Primary game mechanics
        //

        // YEAR 1:    1. ESTABLISHING BULL OR BEAR MARKET
        // YEAR 2-10: 2. ESTABLISHING BULL OR BEAR MARKET
        private void ChooseMarketSituation()
        {
            if (State != States.EstablishMarketSituation) throw new Exception("invalid state");

            // choose an available situation
            var index = RandomSituations[RandomSituationIndex++];
            MarketSituation = SituationCard.ByIndex(index);

            // update prices, based on the market situation
            foreach (var kvp in MarketSituation.Price)
            {
                Prices.Add(kvp.Key, kvp.Value);
            }

            // update state
            UpdateState(States.CurrentMarketPrices);
        }

        // YEAR 1:    2. DETERMINING CURRENT MARKET PRICE OF STOCKS
        // YEAR 2-10: 3. DETERMINING CURRENT MARKET PRICE OF STOCKS
        private void SetMarketPrices()
        {
            if (State != States.CurrentMarketPrices) throw new Exception("invalid state");

            if (Year > 1 || Config.AdjustStartingPrices)
            {
                // random number between [2-12]
                var diceroll = RandomDiceRolls[RandomDiceRollIndex++] + RandomDiceRolls[RandomDiceRollIndex++];
                var adjustments = MarketPriceCard.Generate(MarketSituation.Market, diceroll);

                // combine the prices
                Prices.Add(adjustments);
            }

            // stock split & worthless stock
            foreach (var security in Security.EnumerateAll())
            {
                var price = Prices.ByName(security.Name);

                // 2-for-1 split
                if (price >= Config.StockSplitPrice)
                {
                    // halve the price (round up)
                    var half = (int)Math.Ceiling((float)price / 2f);
                    Prices.Add(security.Name, (-1 * price) + half);

                    if (WithDebugValidation && Prices.ByName(security.Name) != half) throw new Exception("incorrect price");

                    // double the holdings for player 
                    var amount = My.Holdings.ByName(security.Name);
                    if (amount > 0) My.Holdings.Add(security.Name, amount);

                    if (WithDebugValidation && My.Holdings.ByName(security.Name) != (amount * 2)) throw new Exception("invalid amount");

                    // add ledger item
                    if (amount > 0)
                    {
                        My.RecordSheet.Add(new LedgerRow()
                        {
                            Year = Year,
                            Name = security.Name,
                            Amount = amount * 2, // split
                            Price = half,
                            Cost = 0,
                            CashBalance = My.CashBalance
                        });
                    }
                }

                // worthless
                else if (price <= Config.WorthlessStockPrice)
                {
                    // player loses their shares
                    var amount = My.Holdings.ByName(security.Name);
                    if (amount > 0) My.Holdings.Add(security.Name, -1 * amount);

                    if (WithDebugValidation && My.Holdings.ByName(security.Name) != 0) throw new Exception("should be zero");

                    // reset price to ParValue
                    Prices.Add(security.Name, (-1 * price) + Config.ParValue);

                    if (WithDebugValidation && Prices.ByName(security.Name) != Config.ParValue) throw new Exception("should be parvalue");

                    // add ledger item
                    if (amount > 0)
                    {
                        My.RecordSheet.Add(new LedgerRow()
                        {
                            Year = Year,
                            Name = security.Name,
                            Amount = 0, // worthless 
                            Price = 0,
                            Cost = 0,
                            CashBalance = My.CashBalance
                        });
                    }
                }
            }

            // update state
            UpdateState(Year > 1 ? States.SellingSecurities : States.BuyingSecurities);
        }

        // YEARS 2-10: 4. SELLING SECURITIES
        private bool SellingSecuritiesAtMarketPrice(List<Transaction> transactions)
        {
            if (State != States.SellingSecurities) throw new Exception("invalid state");

            if (transactions != null && transactions.Count > 0)
            {
                foreach (var t in transactions)
                {
                    // validate that this is an valid transaction
                    if (t.Amount % Config.PurchaseDivisor != 0 || t.Amount <= 0) throw new Exception("invalid amount");
                    var amount = My.Holdings.ByName(t.Security);
                    if (amount < 0 || amount < t.Amount) throw new Exception("invalid amount");

                    // sell these shares and credit the money
                    var price = Prices.ByName(t.Security);
                    var cost = (t.Amount * price);
                    My.CashBalance += cost;

                    if (WithDebugValidation && price < 0) throw new Exception("invalid price");

                    // reduce the amount of shares
                    My.Holdings.Add(t.Security, -1 * t.Amount);

                    if (WithDebugValidation && My.Holdings.ByName(t.Security) != (amount - t.Amount)) throw new Exception("invalid amount");
                    if (WithDebugValidation && My.Holdings.ByName(t.Security) < 0) throw new Exception("invalid negative amount");

                    // add ledger item
                    My.RecordSheet.Add(new LedgerRow()
                    {
                        Year = Year,
                        Name = t.Security,
                        Amount = t.Amount,
                        Price = price,
                        Cost = cost,
                        CashBalance = My.CashBalance
                    });
                }
            }

            // update state
            UpdateState(States.BuyingSecurities);

            return true;
        }

        // YEAR 1:     3. BUYING SECURITIES
        // YEARS 2-10: 5. BUYING SECURITIES
        private bool BuyingSecuritiesAtMarketPrice(List<Transaction> transactions)
        {
            if (State != States.BuyingSecurities) throw new Exception("not valid to buy securities");

            if (transactions != null && transactions.Count > 0)
            {
                foreach (var t in transactions)
                {
                    // validate that this is an valid transaction
                    if (t.Amount % Config.PurchaseDivisor != 0 || t.Amount <= 0) throw new Exception("invalid amount");
                    var price = Prices.ByName(t.Security);
                    var cost = t.Amount * price;
                    if (cost <= 0 || cost > My.CashBalance) throw new Exception("invalid amount");

                    // deduct the amount
                    My.CashBalance -= cost;

                    if (WithDebugValidation && My.CashBalance < 0) throw new Exception("invalid cash balance");
                    if (WithDebugValidation && My.Holdings.ByName(t.Security) < 0) throw new Exception("invalid negative amount");

                    // add the shares
                    My.Holdings.Add(t.Security, t.Amount);

                    // add ledger item
                    My.RecordSheet.Add(new LedgerRow()
                    {
                        Year = Year,
                        Name = t.Security,
                        Amount = t.Amount,
                        Price = price,
                        Cost = (-1 * cost),
                        CashBalance = My.CashBalance
                    });
                }
            }

            // update state
            UpdateState(States.PostingDividendsAndInterest);

            return true;
        }

        // YEARS 2-10: 1. POSTING DIVIDENS AND INTEREST
        private void PostDividendsAndInterest()
        {
            if (State != States.PostingDividendsAndInterest) throw new Exception("invalid state");

            // apply any cash earnings (these are applied at year end)
            foreach (var kvp in MarketSituation.Cash)
            {
                var amount = My.Holdings.ByName(kvp.Key);
                var divInt = (amount * kvp.Value);
                if (amount > 0) My.CashBalance += divInt;

                if (WithDebugValidation && kvp.Value < 0) throw new Exception("invalid cash bonus");

                // add ledger item
                if (amount > 0)
                {
                    My.RecordSheet.Add(new LedgerRow()
                    {
                        Year = Year,
                        Name = SecurityNames.None,
                        DividendInterest = divInt,
                        CashBalance = My.CashBalance
                    });
                }
            }

            // add dividends and interest (these are calculated based on the ParValue - not current value)
            var totalDivInt = 0;
            foreach (var security in Security.EnumerateAll())
            {
                // check if there is interest/dividends
                if (security.Yield > 0)
                {
                    // stocks <$50 receive no dividend
                    var price = Prices.ByName(security.Name);
                    if (price >= Config.NoDividendPrice)
                    {
                        var amount = My.Holdings.ByName(security.Name);
                        if (amount > 0)
                        {
                            // add ParValue (starting) dividend/interest
                            var divInt = (int)((amount * Config.ParValue) * ((float)security.Yield / 100f));
                            My.CashBalance += divInt;

                            if (WithDebugValidation && divInt <= 0) throw new Exception("invalid dividend/interest");

                            // save for ledger
                            totalDivInt += divInt;
                        }

                        if (WithDebugValidation && amount < 0) throw new Exception("invalid negative amount");
                    } // if (price >= NoDividendPrice)
                } // if (yield > 0)
            } // foreach security

            // add ledger item
            if (totalDivInt > 0)
            {
                My.RecordSheet.Add(new LedgerRow()
                {
                    Year = Year,
                    Name = SecurityNames.None,
                    DividendInterest = totalDivInt,
                    CashBalance = My.CashBalance
                });
            }

            // update
            UpdateState(Year < Config.LastYear ? States.EstablishMarketSituation : States.EndOfGame);
        }
        #endregion

        #region internal
        //
        // Utility
        //

        internal static int[] RandomValues(Random rand, int length, int inclusiveFrom, int inclusiveTo, bool allowdups)
        {
            var result = new int[length];
            if (allowdups)
            {
                // generate 'length' random numbers between [from,to]
                // allowing duplicates
                for (int i = 0; i < length; i++)
                {
                    result[i] = (rand.Next() % (inclusiveTo + 1 - inclusiveFrom)) + inclusiveFrom;
                }
            }
            else
            {
                // shuffle the domain of numbers and take 'length' of them
                var allvalues = new int[inclusiveTo - inclusiveFrom + 1];
                for (int i = 0; i < (inclusiveTo + 1 - inclusiveFrom); i++) allvalues[i] = i + inclusiveFrom;

                // shuffle
                for (int i = 0; i < allvalues.Length; i++)
                {
                    // randomly choose another index
                    var j = i;
                    while (j == i) j = rand.Next() % allvalues.Length;

                    // swap
                    var tmp = allvalues[i];
                    allvalues[i] = allvalues[j];
                    allvalues[j] = tmp;
                }

                // take 'length' of them
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = allvalues[i % allvalues.Length];
                }
            }

            return result;
        }
        #endregion
    }
}
