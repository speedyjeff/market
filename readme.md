### Stocks and Bonds
[Stocks and Bonds](https://boardgamegeek.com/boardgame/1590/stocks-bonds) is a classic board game from the 1960's, where players time the market and the player with the most money wins.  The [rules](https://github.com/speedyjeff/market/tree/main/rules.md) layout how to sell and buy securities over the course of 10 years.  Each year is a Bear or Bull market, with impacting conditions from the broader macro-economic environment.

The game is played on the command line.

```
seed is 1121022145
stocks split at 150 provide no dividend at 50 and are worthless at 0
all transactions must be divisible by 10
stocks start at 100
stock dividends are based on par value
[optional] When buying stock on margin 50% is bought on margin at 5% per year
 margin buys can only occur between years 2 and 9 and must be paid in full when below 25
  when selling/buying below, cash balance is only an estimate

Available securities:
Central City Municipal Bonds    Yield: 5%
   An AAA bond with a good yield.  This investment represents extreme security and good income, but, of course, no appreciation.
Growth Corporation Of America   Yield: 1%
   A well established company with a phenomenal growth record.  it is an expanding industry, spends a lot of money on research and is expected to continue its growth pattern.  The Companys policy of reinvesting earnings causes low yield.  The price-to-earnings ratio of this stock is extremely high.
Metro Properties Inc            Yield: 0%
   An investment representing good prospects of high appreciation.  No dividends are expected in the immediate future.  However, the City's proposed Urban Renewal Program could have great effect on earnings.
Pioneer Mutual Fund             Yield: 4%
   A common stock income mutual fund offered by a large mutual fund company.  It has a good yield of 4%.  Stock in this fund represents a good, steady income, but only a fair appreciation investment.
Shady Brooks Development        Yield: 7%
   A real estate investment representing extremely high income, but steadily depreciating capital assets.
Stryker Drilling Company        Yield: 0%
   A stock representing extremely speculative investment.  Profits go toward drilling new oil wells, so no dividends are expected.  This stock could be worth rags or riches.
Tri-City Transport Company      Yield: 0%
   A stock representing a high appreciation investment prospect with a good stable outlook depending on the administrative ability and ambition of its respected, ethical and energetic young president.  As all profits go back into the Company, dividends are not expected to be declared in the foreseeable future.
United Auto Company             Yield: 2%
   A medium large company in the large, oligopolistic automobile industry.  United Auto, like others in the industry, is subject to whims of public fancy.  Stock in this company represents a somewhat speculative investment with a good growth pattern.  Because it is popular with the investing public, it has fairly high price-to-earning ratio and low yield.
Uranium Enterprises Inc         Yield: 6%
   A highly speculative, high income stock ideal for the short or medium term investor.  Its long term prospects are fair to poor.
Valley Power And Light Company  Yield: 3%
   A stable, steadily growing public utility company located in a well established, healthy industrial area.  Stock in this company represents a safe, medium yield, medium growth investment.

Year 1 of 10
Bear
   Internal Revenue depletion allowance reduced 50%.
    Adjustment: Stryker Drilling Company -15
Current market prices:
Id      Name                    Yield   Price   Holding CostBasis       HoldingsOnMargin
0       Central City Municip    5%      $100    0       $0              0
1       Growth Corporation O    1%      $100    0       $0              0
2       Metro Properties Inc    0%      $100    0       $0              0
3       Pioneer Mutual Fund     4%      $100    0       $0              0
4       Shady Brooks Develop    7%      $100    0       $0              0
5       Stryker Drilling Com    0%      $85     0       $0              0
6       Tri-City Transport C    0%      $100    0       $0              0
7       United Auto Company     2%      $100    0       $0              0
8       Uranium Enterprises     6%      $100    0       $0              0
9       Valley Power And Lig    3%      $100    0       $0              0
cash:  $5000    , net worth:  $5000
Buy: Select a security and amount 'id,amount': ($5000) ['enter' when done, 'u' to undo, prefix with 'm' for margin]
```
Which accepts a number of configurations to modify the default game play.

```
./market.console
 -(h)elp                       - this help
 -(m)ode                       - play an interactive game (0) or simulate multiple games (1) (default 0)
 -(po)licy                     - 0: always buy, 1: always buy low, 2: always buy margin (default 0)
 -(it)erations                 - number of loops when policy is not 0 (default 1000)
 -(se)ed                       - pseudo random seed (default 0 - eg. random)
 -(pa)rvalue                   - starting stock price (default $100
 -(n)oDividendPrice            - price where stocks do not provide dividends (default $50)
 -(st)ockSplitPrice            - price at stock split (default $150)
 -(wo)rthlessStockPrice        - price at which companies are bankrupt and all shares liquidated (default $0)
 -(l)astYear                   - game starts at year 1 and end at last year (default 10)
 -(pu)rchaseDivisor            - divisor for chunks of shares (default 10 - eg. must buy chunks of 10)
 -(a)djustStartingPrices       - adjust initial stock prices from parvalue (default not set)
 -(in)itialCashBalance         - initial cash balance (default $5000)
 -(d)ividendBasedOnMarketPrice - compute dividend based on market prices not parvalue (default not set)
 -(marginI)nterestDue          - annual interest due on margin total (default 5%)
 -(marginSp)litRatio           - divisor of how much stock is bought up front (default 2 - eg. 50%)
 -(marginSt)ockMustBeBought    - stock price at which margin purchases must pay up (default $25)
 -(wi)thDebugValidation        - turn on internal validation (default not set)
```

#### Setup
```
git submodule init
git submodule update
```
