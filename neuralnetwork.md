### Probabiliaties
The game has a finite set of possible states all randomly possible to occur.  To get a picture of outcomes are most likely, 100M games were run (10 years each) and the state of the market were captured.  Below are the probabilities.

Security | Increases in 1K | Percentage | Decreases in 1K | Percentage | Above Starting Value in 1K | Percentage | Below Starting Value in 1K | Percentage | Splits in 1K | Percentage | Worthless in 1K | Percentage | No Dividend in 1K | Percentage |
---------|-----------------|------------|-----------------|------------|----------------------------|------------|----------------------------|------------|--------------|------------|-----------------|------------|-------------------|------------|
CentralCityMunicipalBonds | 0 | 0% | 0 | 0% | 0 | 0% | 0 | 0% | 0 | 0% | 0 | 0.0% | 0 | 0.0%
GrowthCorporationOfAmerica | 646 | 65% | 248 | 25% | 885 | 88% | 34 | 3% | 115 | 11% | 0 | 0.0% | 1 | 0.0%
MetroPropertiesInc | 667 | 67% | 231 | 23% | 890 | 89% | 27 | 3% | 136 | 14% | 0 | 0.0% | 1000 | 100.0%
PioneerMutualFund | 584 | 58% | 317 | 32% | 832 | 83% | 72 | 7% | 69 | 7% | 0 | 0.0% | 1 | 0.0%
ShadyBrooksDevelopment | 440 | 44% | 461 | 46% | 602 | 60% | 287 | 29% | 10 | 1% | 0 | 0.0% | 1 | 0.0%
StrykerDrillingCompany | 314 | 31% | 586 | 59% | 676 | 68% | 225 | 22% | 155 | 15% | 7 | 0.6% | 1000 | 100.0%
TriCityTransportCompany | 709 | 71% | 187 | 19% | 883 | 88% | 31 | 3% | 133 | 13% | 1 | 0.0% | 1000 | 100.0%
UnitedAutoCompany | 569 | 57% | 329 | 33% | 818 | 82% | 100 | 10% | 110 | 11% | 1 | 0.0% | 7 | 0.7%
UraniumEnterprisesInc | 382 | 38% | 512 | 51% | 602 | 60% | 301 | 30% | 56 | 6% | 1 | 0.1% | 39 | 3.9%
ValleyPowerAndLightCompany | 684 | 68% | 217 | 22% | 877 | 88% | 29 | 3% | 90 | 9% | 0 | 0.0% | 1 | 0.0%

```
./market.console -mode 1 -policy 5 -iterations 100000000
```

The game has 2 primary sources of randomization: 38 market situation cards, and 10 rolls of 2 dice.  The following represents how many possible games exist:

```
C = number of market situation cards (default 38)
N = number of years (default 10)
D = dice outcomes (default 11)
Possible games = (C * C-1 * ... * C-N) * (D * N) 
               = 5,283,605,261,620,224,000
```

Looking at the probabilities, Growth Corporation Of America and Valley Power And Light Company offers good odds for increases (>64%), most of the time are over starting price (>85%), have a chance to split, and rarely provide no dividend.  With that said, Stryker Drilling Company, Metro Properties Inc, and Tri-City Transport Company have the highest likihood to split.

### Network Setup
The neural network bots consist of two neural networks per security (one for making buy and sell decisions).  The order in which these networks are considered is controlled by a randomized (configurable) list of securities.  The order is set after first initialization.

The input for each network is the same: 
 * cash balance (normalized as a percentage of initial cash balance)
 * current secuirty price (normalized by stock split percentage, eg. max)
 * current holding (normalized by maximum holding)
 * current secrutiy cost basis (normalized by split percentage, eg. max)
 * current security margin holding (normalized by maximum holding)
 * indicator of bull (1) or bear (0) market
 * security price for all 10 securities (normalized by stock split percentage, eg. max)

The output is a series of choices. The basic choices are:
 * Zero shares
 * 10 shares
 * 20 shares
 * 50% cash balance
 * 100% cash balance

When trading with margins, the additional options are added:
 * 10 shares on margin
 * 20 shares on margin
 * 50% cash balance on margin
 * 100% cash balance on margin

The bots run all sell/buy networks in parallel and then consider them in the order given by the security order list.  If there is enough monty to satisfy the request, or enough shares held the order is added to the queue.

After each round, the networks are evaluated by their average networths across all runs.  The top are kept, and the bottom are removed to be replaced by new networks.

### Experiments

Based on the networks and experiments, here is a set of recommendations:
 * The following securities have a high chance of at least 1 split per game:
   * Stryker Drilling Company
   * Metro Properties Inc
   * Tri-City Transport Company
   * Growth Corporation Of America
   * United Auto Company
 * Networks were trained over multiple games, and favor the companies that split.
 * In gerneral securities rise year over year, and timing split prices well yields the most money
 * Top earnings for a 10 year game is ~$20K

#### Best strategies (no-margin)
The following experiment trains 20 networks in parallel, keeping the networks with the highest average networth.  The order in which securities are considered is randomized, independently for buys and sells, and used throughout the 10 years.

This experiment can be re-run with the following command line (though seeds are randomized and results will vary).

```
./market.console -mode 1 -policy 3 -itearations 1000000
```

The results below highlight that the highest yielded strategy involves investing in Stryker Drilling Company.  However, as the frist three executions below highlight, Stryker is boom and bust.  The network witht he hightest average only purchased Stryker and ended up with a networth of $0 on the last run.  (This is the same for the top 3 bots).

Interestingly, the last two networks leveraged United Auto Company for the largest winnings.  This is a bit non-intuiative as that security has a relatively low yield.  However, there may be other influences like the situation cards and bull/bear flucations which make this a good long term buy.

A few other observations.  All the networks exhibited the behavior of selling a security and buying the same amount during the same year.  This has a $0 net effect, and is a just busy work.  But all the networks exhibited this behavior.  The network with the highest survival (263 runs) is a very small percentage of the total runs (1M).  This may point to the highly random nature of the game which offers high variability.

The results below are the 5 networks which yielded the highest average networth.  The ledger shown is from the last run in the series.  The top 3 ended up with a networth of $0, but still had the highest average networth.

<details>
<summary>full output</summary>
<pre>
AdjustStartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 1000000
Policy                     = NeuralBots
NeuralMargin               = False
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = False
NeuralRandomResults        = 0

Avg: 17066.63212435233 over 193 runs
Buys: ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany PioneerMutualFund ValleyPowerAndLightCompany UraniumEnterprisesInc CentralCityMunicipalBonds MetroPropertiesInc UnitedAutoCompany GrowthCorporationOfAmerica
Sells: TriCityTransportCompany ValleyPowerAndLightCompany PioneerMutualFund ShadyBrooksDevelopment CentralCityMunicipalBonds UnitedAutoCompany MetroPropertiesInc GrowthCorporationOfAmerica StrykerDrillingCompany UraniumEnterprisesInc
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Stryker Drilling Com    50       $100   -$5000   $0      $0              $0              $0             Buy
2       Stryker Drilling Com    10       $60     $600    $0      $0              $600            $0             Sell
2       Stryker Drilling Com    10       $60    -$600    $0      $0              $0              $0             Buy
3       Stryker Drilling Com    10       $20     $200    $0      $0              $200            $0             Sell
3       Stryker Drilling Com    10       $20    -$200    $0      $0              $0              $0             Buy
4       Stryker Drilling Com    0        $0      $0      $0      $0              $0              $0             Worthless
Avg: 16892.661596958176 over 263 runs
Buys: StrykerDrillingCompany UnitedAutoCompany GrowthCorporationOfAmerica PioneerMutualFund ValleyPowerAndLightCompany TriCityTransportCompany UraniumEnterprisesInc ShadyBrooksDevelopment MetroPropertiesInc CentralCityMunicipalBonds
Sells: UraniumEnterprisesInc UnitedAutoCompany PioneerMutualFund TriCityTransportCompany CentralCityMunicipalBonds ValleyPowerAndLightCompany StrykerDrillingCompany MetroPropertiesInc ShadyBrooksDevelopment GrowthCorporationOfAmerica
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Stryker Drilling Com    50       $100   -$5000   $0      $0              $0              $0             Buy
2       Stryker Drilling Com    10       $60     $600    $0      $0              $600            $0             Sell
2       Stryker Drilling Com    10       $60    -$600    $0      $0              $0              $0             Buy
3       Stryker Drilling Com    10       $20     $200    $0      $0              $200            $0             Sell
3       Stryker Drilling Com    10       $20    -$200    $0      $0              $0              $0             Buy
4       Stryker Drilling Com    0        $0      $0      $0      $0              $0              $0             Worthless
Avg: 16542.80991735537 over 121 runs
Buys: UraniumEnterprisesInc StrykerDrillingCompany MetroPropertiesInc UnitedAutoCompany ShadyBrooksDevelopment PioneerMutualFund ValleyPowerAndLightCompany CentralCityMunicipalBonds GrowthCorporationOfAmerica TriCityTransportCompany
Sells: ValleyPowerAndLightCompany ShadyBrooksDevelopment StrykerDrillingCompany PioneerMutualFund UraniumEnterprisesInc TriCityTransportCompany GrowthCorporationOfAmerica MetroPropertiesInc CentralCityMunicipalBonds UnitedAutoCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Stryker Drilling Com    50       $100   -$5000   $0      $0              $0              $0             Buy
2       Stryker Drilling Com    10       $60     $600    $0      $0              $600            $0             Sell
2       Stryker Drilling Com    10       $60    -$600    $0      $0              $0              $0             Buy
3       Stryker Drilling Com    10       $20     $200    $0      $0              $200            $0             Sell
3       Stryker Drilling Com    10       $20    -$200    $0      $0              $0              $0             Buy
4       Stryker Drilling Com    0        $0      $0      $0      $0              $0              $0             Worthless
Avg: 15330 over 8 runs
Buys: ValleyPowerAndLightCompany UnitedAutoCompany CentralCityMunicipalBonds UraniumEnterprisesInc MetroPropertiesInc TriCityTransportCompany GrowthCorporationOfAmerica PioneerMutualFund StrykerDrillingCompany ShadyBrooksDevelopment
Sells: PioneerMutualFund ShadyBrooksDevelopment GrowthCorporationOfAmerica StrykerDrillingCompany UraniumEnterprisesInc TriCityTransportCompany ValleyPowerAndLightCompany CentralCityMunicipalBonds MetroPropertiesInc UnitedAutoCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Valley Power And Lig    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       United Auto Company     30       $100   -$3000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $120    $0              $120            $0             DividendAndInterest
2       Valley Power And Lig    10       $104    $1040   $0      $0              $1160           $0             Sell
2       Metro Properties Inc    10       $108   -$1080   $0      $0              $80             $0             Buy
2                               0        $0      $0      $90     $0              $170            $0             DividendAndInterest
3       Valley Power And Lig    10       $108    $1080   $0      $0              $1250           $0             Sell
3       Stryker Drilling Com    10       $20    -$200    $0      $0              $1050           $0             Buy
3                               0        $0      $0      $60     $0              $1110           $0             DividendAndInterest
4       Stryker Drilling Com    0        $0      $0      $0      $0              $1110           $0             Worthless
4       Stryker Drilling Com    10       $100   -$1000   $0      $0              $110            $0             Buy
4                               0        $0      $0      $60     $0              $170            $0             DividendAndInterest
5       Stryker Drilling Com    20       $84     $0      $0      $0              $170            $0             Split
5       United Auto Company     60       $82     $0      $0      $0              $170            $0             Split
5                               0        $0      $0      $120    $0              $290            $0             DividendAndInterest
6       Metro Properties Inc    20       $77     $0      $0      $0              $290            $0             Split
6                               0        $0      $0      $120    $0              $410            $0             DividendAndInterest
7                               0        $0      $0      $120    $0              $530            $0             DividendAndInterest
8                               0        $0      $0      $120    $0              $650            $0             DividendAndInterest
9       Stryker Drilling Com    10       $89     $890    $0      $0              $1540           $0             Sell
9       Metro Properties Inc    10       $115   -$1150   $0      $0              $390            $0             Buy
9                               0        $0      $0      $120    $0              $510            $0             DividendAndInterest
10                              0        $0      $0      $120    $0              $630            $0             DividendAndInterest
Avg: 15178.57142857143 over 7 runs
Buys: UnitedAutoCompany GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund TriCityTransportCompany UraniumEnterprisesInc CentralCityMunicipalBonds ValleyPowerAndLightCompany ShadyBrooksDevelopment StrykerDrillingCompany
Sells: CentralCityMunicipalBonds ShadyBrooksDevelopment PioneerMutualFund ValleyPowerAndLightCompany StrykerDrillingCompany UraniumEnterprisesInc MetroPropertiesInc UnitedAutoCompany GrowthCorporationOfAmerica TriCityTransportCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       United Auto Company     50       $100   -$5000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $100    $0              $100            $0             DividendAndInterest
2       United Auto Company     10       $116    $1160   $0      $0              $1260           $0             Sell
2       United Auto Company     10       $116   -$1160   $0      $0              $100            $0             Buy
2                               0        $0      $0      $100    $0              $200            $0             DividendAndInterest
3       United Auto Company     10       $132    $1320   $0      $0              $1520           $0             Sell
3       United Auto Company     10       $132   -$1320   $0      $0              $200            $0             Buy
3       Stryker Drilling Com    10       $20    -$200    $0      $0              $0              $0             Buy
3                               0        $0      $0      $100    $0              $100            $0             DividendAndInterest
4       Stryker Drilling Com    0        $0      $0      $0      $0              $100            $0             Worthless
4       United Auto Company     10       $142    $1420   $0      $0              $1520           $0             Sell
4       United Auto Company     10       $142   -$1420   $0      $0              $100            $0             Buy
4                               0        $0      $0      $100    $0              $200            $0             DividendAndInterest
5       United Auto Company     100      $82     $0      $0      $0              $200            $0             Split
5       United Auto Company     10       $82     $820    $0      $0              $1020           $0             Sell
5       United Auto Company     10       $82    -$820    $0      $0              $200            $0             Buy
5                               0        $0      $0      $200    $0              $400            $0             DividendAndInterest
6       United Auto Company     10       $100    $1000   $0      $0              $1400           $0             Sell
6       United Auto Company     10       $100   -$1000   $0      $0              $400            $0             Buy
6                               0        $0      $0      $200    $0              $600            $0             DividendAndInterest
7       United Auto Company     10       $104    $1040   $0      $0              $1640           $0             Sell
7       United Auto Company     10       $104   -$1040   $0      $0              $600            $0             Buy
7                               0        $0      $0      $200    $0              $800            $0             DividendAndInterest
8       United Auto Company     10       $117    $1170   $0      $0              $1970           $0             Sell
8       United Auto Company     10       $117   -$1170   $0      $0              $800            $0             Buy
8                               0        $0      $0      $200    $0              $1000           $0             DividendAndInterest
9       United Auto Company     10       $133    $1330   $0      $0              $2330           $0             Sell
9       United Auto Company     10       $133   -$1330   $0      $0              $1000           $0             Buy
9       Uranium Enterprises     10       $88    -$880    $0      $0              $120            $0             Buy
9                               0        $0      $0      $260    $0              $380            $0             DividendAndInterest
10      United Auto Company     10       $122    $1220   $0      $0              $1600           $0             Sell
10      United Auto Company     10       $122   -$1220   $0      $0              $380            $0             Buy
10                              0        $0      $0      $260    $0              $640            $0             DividendAndInterest
</pre>
</details>

#### Best strategies (allowing margin)
The following experiment has the same shape/parameters as the experiment above, except networks are able to buy/sell on margin.

```
market.console.exe -mode 1 -policy 3 -iterations 1000000 -neuralmargin
```

The average net worth when buying/selling on margin is ~25% higher than when not.  The game disallows buying on margin in the first and last year.  As a result, the model uses all their cash in the opening year and then starts to buy on margin.  Once able to buy on margin, they max out their maring purchases.  Accumulating $10K to $20K in outstanding margin costs.  The sets of securities purchased are similar to when buying not on margin, however, more diversity is seen on which to purchase.

<details>
<summary>full output</summary>
<pre>
AdjustStartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 1000000
Policy                     = NeuralBots
NeuralMargin               = True
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = False
NeuralRandomResults        = 0

Avg: 23161.262531860662 over 1177 runs
Buys: ValleyPowerAndLightCompany TriCityTransportCompany UnitedAutoCompany ShadyBrooksDevelopment GrowthCorporationOfAmerica UraniumEnterprisesInc MetroPropertiesInc StrykerDrillingCompany PioneerMutualFund CentralCityMunicipalBonds
Sells: PioneerMutualFund UraniumEnterprisesInc TriCityTransportCompany StrykerDrillingCompany UnitedAutoCompany ShadyBrooksDevelopment MetroPropertiesInc ValleyPowerAndLightCompany CentralCityMunicipalBonds GrowthCorporationOfAmerica
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Metro Properties Inc    50       $100   -$5000   $0      $0              $0              $0             Buy
2       Metro Properties Inc    20       $121    $2420   $0      $0              $2420           $0             Sell
2       Valley Power And Lig    10       $119   -$595    $0      $0              $1825           $595           Buy
2       United Auto Company     10       $123   -$615    $0      $0              $1210           $1210          Buy
2       Uranium Enterprises     20       $119   -$1190   $0      $0              $20             $2400          Buy
2                               0        $0      $0      $170    $0              $190            $0             DividendAndInterest
2                               0        $0      $0      $0     -$120            $70             $2400          MarginInterestCharge
3       Metro Properties Inc    60       $76     $0      $0      $0              $70             $0             Split
3       Uranium Enterprises     10       $105    $1050   $0      $0              $1120           $2400          Sell
3       Metro Properties Inc    20       $76     $1520   $0      $0              $2640           $2400          Sell
3       Valley Power And Lig    10       $129    $695    $0      $595            $3335           $1805          Sell
3       Valley Power And Lig    10       $129   -$645    $0      $0              $2690           $2450          Buy
3       United Auto Company     10       $141   -$705    $0      $0              $1985           $3155          Buy
3       Uranium Enterprises     20       $105   -$1050   $0      $0              $935            $4205          Buy
3       Metro Properties Inc    10       $76    -$760    $0      $0              $175            $4205          Buy
3                               0        $0      $0      $250    $0              $425            $0             DividendAndInterest
3                               0        $0      $0      $0     -$211            $214            $4205          MarginInterestCharge
4       Uranium Enterprises     10       $104    $1040   $0      $0              $1254           $4205          Sell
4       United Auto Company     20       $142    $1520   $0      $1320           $2774           $2885          Sell
4       Metro Properties Inc    20       $84     $1680   $0      $0              $4454           $2885          Sell
4       Valley Power And Lig    10       $133    $685    $0      $645            $5139           $2240          Sell
4       Valley Power And Lig    10       $133   -$665    $0      $0              $4474           $2905          Buy
4       United Auto Company     30       $142   -$2130   $0      $0              $2344           $5035          Buy
4       Uranium Enterprises     20       $104   -$1040   $0      $0              $1304           $6075          Buy
4       Metro Properties Inc    10       $84    -$840    $0      $0              $464            $6075          Buy
4                               0        $0      $0      $330    $0              $794            $0             DividendAndInterest
4                               0        $0      $0      $0     -$304            $490            $6075          MarginInterestCharge
5       United Auto Company     60       $88     $0      $0      $0              $490            $0             Split
5       Uranium Enterprises     10       $123    $1230   $0      $0              $1720           $6075          Sell
5       United Auto Company     20       $88     $1760   $0      $0              $3480           $6075          Sell
5       Metro Properties Inc    20       $105    $2100   $0      $0              $5580           $6075          Sell
5       Valley Power And Lig    10       $147    $805    $0      $665            $6385           $5410          Sell
5       Valley Power And Lig    20       $147   -$1470   $0      $0              $4915           $6880          Buy
5       United Auto Company     50       $88    -$2200   $0      $0              $2715           $9080          Buy
5       Uranium Enterprises     20       $123   -$1230   $0      $0              $1485           $10310         Buy
5       Metro Properties Inc    10       $105   -$1050   $0      $0              $435            $10310         Buy
5                               0        $0      $0      $540    $0              $975            $0             DividendAndInterest
5                               0        $0      $0      $0     -$516            $459            $10310         MarginInterestCharge
6       Valley Power And Lig    40       $80     $0      $0      $0              $459            $0             Split
6       Uranium Enterprises     10       $113    $1130   $0      $0              $1589           $10320         Sell
6       United Auto Company     20       $101    $2020   $0      $0              $3609           $10320         Sell
6       Metro Properties Inc    20       $120    $2400   $0      $0              $6009           $10320         Sell
6       Valley Power And Lig    10       $80     $800    $0      $0              $6809           $10320         Sell
6       Valley Power And Lig    40       $80    -$1600   $0      $0              $5209           $11920         Buy
6       United Auto Company     50       $101   -$2525   $0      $0              $2684           $14445         Buy
6       Uranium Enterprises     20       $113   -$1130   $0      $0              $1554           $15575         Buy
6       Metro Properties Inc    10       $120   -$1200   $0      $0              $354            $15575         Buy
6                               0        $0      $0      $810    $0              $1164           $0             DividendAndInterest
6                               0        $0      $0      $0     -$779            $385            $15575         MarginInterestCharge
7       Uranium Enterprises     10       $95     $950    $0      $0              $1335           $15575         Sell
7       United Auto Company     20       $75     $1500   $0      $0              $2835           $15575         Sell
7       Metro Properties Inc    20       $131    $2620   $0      $0              $5455           $15575         Sell
7       Valley Power And Lig    10       $76     $760    $0      $0              $6215           $15575         Sell
7       Valley Power And Lig    40       $76    -$1520   $0      $0              $4695           $17095         Buy
7       United Auto Company     60       $75    -$2250   $0      $0              $2445           $19345         Buy
7       Uranium Enterprises     20       $95    -$950    $0      $0              $1495           $20295         Buy
7       Metro Properties Inc    10       $131   -$1310   $0      $0              $185            $20295         Buy
7                               0        $0      $0      $1040   $0              $1225           $0             DividendAndInterest
7                               0        $0      $0      $0     -$1015           $210            $20295         MarginInterestCharge
8       Uranium Enterprises     10       $117    $1170   $0      $0              $1380           $20295         Sell
8       United Auto Company     20       $56     $1120   $0      $0              $2500           $20295         Sell
8       Valley Power And Lig    10       $60     $600    $0      $0              $3100           $20295         Sell
8       Valley Power And Lig    20       $60    -$600    $0      $0              $2500           $20895         Buy
8       United Auto Company     40       $56    -$1120   $0      $0              $1380           $22015         Buy
8       Uranium Enterprises     20       $117   -$1170   $0      $0              $210            $23185         Buy
8                               0        $0      $0      $1170   $0              $1380           $0             DividendAndInterest
8                               0        $0      $0      $0     -$1160           $220            $23185         MarginInterestCharge
9       Uranium Enterprises     10       $130    $1300   $0      $0              $1520           $23185         Sell
9       United Auto Company     20       $46     $920    $0      $0              $2440           $23185         Sell
9       Valley Power And Lig    10       $79     $790    $0      $0              $3230           $23185         Sell
9       Valley Power And Lig    20       $79    -$790    $0      $0              $2440           $23975         Buy
9       United Auto Company     50       $46    -$1150   $0      $0              $1290           $25125         Buy
9       Metro Properties Inc    10       $117   -$1170   $0      $0              $120            $25125         Buy
9                               0        $0      $0      $780    $0              $900            $0             DividendAndInterest
9                               0        $0      $0      $0     -$1257          -$357            $25125         MarginInterestCharge
9                               0        $0      $0      $0     -$11375         -$11732          $13750         MarginInterestCharge
9                               0        $0      $0      $0     -$7760          -$19492          $5990          MarginInterestCharge
9                               0        $0      $0      $0     -$5990          -$25482          $0             MarginInterestCharge
10      Uranium Enterprises     10       $140    $1400   $0      $0             -$24082          $0             Sell
10      United Auto Company     20       $36     $720    $0      $0             -$23362          $0             Sell
10      Metro Properties Inc    10       $119    $1190   $0      $0             -$22172          $0             Sell
10      Valley Power And Lig    20       $83     $1660   $0      $0             -$20512          $0             Sell
10                              0        $0      $0      $660    $0             -$19852          $0             DividendAndInterest
Avg: 21462.636363636364 over 11 runs
Buys: StrykerDrillingCompany ValleyPowerAndLightCompany CentralCityMunicipalBonds ShadyBrooksDevelopment MetroPropertiesInc TriCityTransportCompany PioneerMutualFund UraniumEnterprisesInc GrowthCorporationOfAmerica UnitedAutoCompany
Sells: MetroPropertiesInc UraniumEnterprisesInc StrykerDrillingCompany ShadyBrooksDevelopment CentralCityMunicipalBonds TriCityTransportCompany ValleyPowerAndLightCompany PioneerMutualFund GrowthCorporationOfAmerica UnitedAutoCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Central City Municip    50       $100   -$5000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $250    $0              $250            $0             DividendAndInterest
2       Central City Municip    10       $100    $1000   $0      $0              $1250           $0             Sell
2       Central City Municip    10       $100   -$1000   $0      $0              $250            $0             Buy
2                               0        $0      $0      $250    $0              $500            $0             DividendAndInterest
3       Central City Municip    10       $100    $1000   $0      $0              $1500           $0             Sell
3       Stryker Drilling Com    20       $143   -$1430   $0      $0              $70             $1430          Buy
3                               0        $0      $0      $200    $0              $270            $0             DividendAndInterest
3                               0        $0      $0      $0     -$72             $198            $1430          MarginInterestCharge
4       Central City Municip    10       $100    $1000   $0      $0              $1198           $1430          Sell
4       Stryker Drilling Com    20       $103   -$1030   $0      $0              $168            $2460          Buy
4                               0        $0      $0      $150    $0              $318            $0             DividendAndInterest
4                               0        $0      $0      $0     -$123            $195            $2460          MarginInterestCharge
5       Central City Municip    10       $100    $1000   $0      $0              $1195           $2460          Sell
5       Central City Municip    10       $100   -$1000   $0      $0              $195            $2460          Buy
5                               0        $0      $0      $150    $0              $345            $0             DividendAndInterest
5                               0        $0      $0      $0     -$123            $222            $2460          MarginInterestCharge
6       Central City Municip    10       $100    $1000   $0      $0              $1222           $2460          Sell
6       Stryker Drilling Com    20       $120   -$1200   $0      $0              $22             $3660          Buy
6                               0        $0      $0      $100    $0              $122            $0             DividendAndInterest
6                               0        $0      $0      $0     -$183           -$61             $3660          MarginInterestCharge
7       Stryker Drilling Com    120      $75     $0      $0      $0             -$61             $0             Split
7       Central City Municip    10       $100    $1000   $0      $0              $939            $3680          Sell
7       Stryker Drilling Com    20       $75    -$750    $0      $0              $189            $4430          Buy
7                               0        $0      $0      $50     $0              $239            $0             DividendAndInterest
7                               0        $0      $0      $0     -$222            $17             $4430          MarginInterestCharge
8       Central City Municip    10       $100    $1000   $0      $0              $1017           $4430          Sell
8       Central City Municip    10       $100   -$1000   $0      $0              $17             $4430          Buy
8                               0        $0      $0      $50     $0              $67             $0             DividendAndInterest
8                               0        $0      $0      $0     -$222           -$155            $4430          MarginInterestCharge
9       Central City Municip    10       $100    $1000   $0      $0              $845            $4430          Sell
9       Metro Properties Inc    10       $117   -$585    $0      $0              $260            $5015          Buy
9                               0        $0      $0      $0     -$251            $9              $5015          MarginInterestCharge
9                               0        $0      $0      $0     -$585           -$576            $4430          MarginInterestCharge
9                               0        $0      $0      $0     -$4430          -$5006           $0             MarginInterestCharge
Avg: 20381.106643356645 over 572 runs
Buys: UraniumEnterprisesInc UnitedAutoCompany StrykerDrillingCompany GrowthCorporationOfAmerica MetroPropertiesInc TriCityTransportCompany CentralCityMunicipalBonds ValleyPowerAndLightCompany ShadyBrooksDevelopment PioneerMutualFund
Sells: MetroPropertiesInc ShadyBrooksDevelopment PioneerMutualFund StrykerDrillingCompany TriCityTransportCompany ValleyPowerAndLightCompany UnitedAutoCompany GrowthCorporationOfAmerica CentralCityMunicipalBonds UraniumEnterprisesInc
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
2       United Auto Company     40       $123   -$2460   $0      $0              $2540           $2460          Buy
2       Stryker Drilling Com    20       $137   -$1370   $0      $0              $1170           $3830          Buy
2                               0        $0      $0      $80     $0              $1250           $0             DividendAndInterest
2                               0        $0      $0      $0     -$192            $1058           $3830          MarginInterestCharge
3       Stryker Drilling Com    10       $143    $1430   $0      $0              $2488           $3830          Sell
3       United Auto Company     10       $141    $1410   $0      $0              $3898           $3830          Sell
3       United Auto Company     20       $141   -$1410   $0      $0              $2488           $5240          Buy
3       Stryker Drilling Com    20       $143   -$1430   $0      $0              $1058           $6670          Buy
3                               0        $0      $0      $100    $0              $1158           $0             DividendAndInterest
3                               0        $0      $0      $0     -$334            $824            $6670          MarginInterestCharge
4       Stryker Drilling Com    10       $103    $1030   $0      $0              $1854           $6670          Sell
4       United Auto Company     10       $142    $1420   $0      $0              $3274           $6670          Sell
4       United Auto Company     20       $142   -$1420   $0      $0              $1854           $8090          Buy
4       Stryker Drilling Com    20       $103   -$1030   $0      $0              $824            $9120          Buy
4                               0        $0      $0      $120    $0              $944            $0             DividendAndInterest
4                               0        $0      $0      $0     -$456            $488            $9120          MarginInterestCharge
5       United Auto Company     120      $88     $0      $0      $0              $488            $0             Split
5       Stryker Drilling Com    10       $140    $1400   $0      $0              $1888           $9150          Sell
5       United Auto Company     10       $88     $880    $0      $0              $2768           $9150          Sell
5       United Auto Company     30       $88    -$1320   $0      $0              $1448           $10470         Buy
5       Stryker Drilling Com    20       $140   -$1400   $0      $0              $48             $11870         Buy
5                               0        $0      $0      $280    $0              $328            $0             DividendAndInterest
5                               0        $0      $0      $0     -$594           -$266            $11870         MarginInterestCharge
6       Stryker Drilling Com    10       $120    $1200   $0      $0              $934            $11870         Sell
6       United Auto Company     10       $101    $1010   $0      $0              $1944           $11870         Sell
6       United Auto Company     10       $101   -$505    $0      $0              $1439           $12375         Buy
6       Stryker Drilling Com    20       $120   -$1200   $0      $0              $239            $13575         Buy
6                               0        $0      $0      $280    $0              $519            $0             DividendAndInterest
6                               0        $0      $0      $0     -$679           -$160            $13575         MarginInterestCharge
7       Stryker Drilling Com    120      $75     $0      $0      $0             -$160            $0             Split
7       Stryker Drilling Com    10       $75     $750    $0      $0              $590            $13605         Sell
7       United Auto Company     10       $75     $750    $0      $0              $1340           $13605         Sell
7       United Auto Company     10       $75    -$375    $0      $0              $965            $13980         Buy
7       Stryker Drilling Com    20       $75    -$750    $0      $0              $215            $14730         Buy
7                               0        $0      $0      $280    $0              $495            $0             DividendAndInterest
7                               0        $0      $0      $0     -$737           -$242            $14730         MarginInterestCharge
8       Stryker Drilling Com    10       $105    $1050   $0      $0              $808            $14730         Sell
8       United Auto Company     10       $56     $560    $0      $0              $1368           $14730         Sell
8       United Auto Company     20       $56    -$560    $0      $0              $808            $15290         Buy
8                               0        $0      $0      $300    $0              $1108           $0             DividendAndInterest
8                               0        $0      $0      $0     -$765            $343            $15290         MarginInterestCharge
9       Stryker Drilling Com    10       $96     $960    $0      $0              $1303           $15290         Sell
9       United Auto Company     10       $46     $460    $0      $0              $1763           $15290         Sell
9       United Auto Company     30       $46    -$690    $0      $0              $1073           $15980         Buy
9       Stryker Drilling Com    20       $96    -$960    $0      $0              $113            $16940         Buy
9                               0        $0      $0      $0     -$847           -$734            $16940         MarginInterestCharge
9                               0        $0      $0      $0     -$8170          -$8904           $8770          MarginInterestCharge
9                               0        $0      $0      $0     -$8770          -$17674          $0             MarginInterestCharge
10      Stryker Drilling Com    10       $141    $1410   $0      $0             -$16264          $0             Sell
10      United Auto Company     10       $36     $360    $0      $0             -$15904          $0             Sell
Avg: 20361.371959942775 over 699 runs
Buys: PioneerMutualFund MetroPropertiesInc StrykerDrillingCompany ShadyBrooksDevelopment TriCityTransportCompany UraniumEnterprisesInc CentralCityMunicipalBonds UnitedAutoCompany GrowthCorporationOfAmerica ValleyPowerAndLightCompany
Sells: ShadyBrooksDevelopment UnitedAutoCompany StrykerDrillingCompany UraniumEnterprisesInc PioneerMutualFund TriCityTransportCompany ValleyPowerAndLightCompany GrowthCorporationOfAmerica MetroPropertiesInc CentralCityMunicipalBonds
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Stryker Drilling Com    20       $100   -$2000   $0      $0              $3000           $0             Buy
2       Stryker Drilling Com    20       $137    $2740   $0      $0              $5740           $0             Sell
2       Pioneer Mutual Fund     20       $113   -$1130   $0      $0              $4610           $1130          Buy
2       Metro Properties Inc    30       $121   -$1815   $0      $0              $2795           $2945          Buy
2       Stryker Drilling Com    20       $137   -$2740   $0      $0              $55             $2945          Buy
2                               0        $0      $0      $80     $0              $135            $0             DividendAndInterest
2                               0        $0      $0      $0     -$148           -$13             $2945          MarginInterestCharge
3       Metro Properties Inc    60       $76     $0      $0      $0             -$13             $0             Split
3       Stryker Drilling Com    20       $143    $2860   $0      $0              $2847           $2960          Sell
3       Pioneer Mutual Fund     10       $114    $1140   $0      $0              $3987           $2960          Sell
3       Metro Properties Inc    20       $76     $1520   $0      $0              $5507           $2960          Sell
3       Pioneer Mutual Fund     20       $114   -$1140   $0      $0              $4367           $4100          Buy
3       Metro Properties Inc    50       $76    -$1900   $0      $0              $2467           $6000          Buy
3                               0        $0      $0      $120    $0              $2587           $0             DividendAndInterest
3                               0        $0      $0      $0     -$300            $2287           $6000          MarginInterestCharge
4       Pioneer Mutual Fund     10       $119    $1190   $0      $0              $3477           $6000          Sell
4       Metro Properties Inc    20       $84     $1680   $0      $0              $5157           $6000          Sell
4       Pioneer Mutual Fund     20       $119   -$1190   $0      $0              $3967           $7190          Buy
4       Metro Properties Inc    40       $84    -$1680   $0      $0              $2287           $8870          Buy
4       Stryker Drilling Com    20       $103   -$2060   $0      $0              $227            $8870          Buy
4                               0        $0      $0      $160    $0              $387            $0             DividendAndInterest
4                               0        $0      $0      $0     -$444           -$57             $8870          MarginInterestCharge
5       Stryker Drilling Com    20       $140    $2800   $0      $0              $2743           $8870          Sell
5       Pioneer Mutual Fund     10       $132    $1320   $0      $0              $4063           $8870          Sell
5       Metro Properties Inc    20       $105    $2100   $0      $0              $6163           $8870          Sell
5       Pioneer Mutual Fund     20       $132   -$1320   $0      $0              $4843           $10190         Buy
5       Metro Properties Inc    40       $105   -$2100   $0      $0              $2743           $12290         Buy
5                               0        $0      $0      $200    $0              $2943           $0             DividendAndInterest
5                               0        $0      $0      $0     -$615            $2328           $12290         MarginInterestCharge
6       Pioneer Mutual Fund     10       $147    $1470   $0      $0              $3798           $12290         Sell
6       Metro Properties Inc    20       $120    $2400   $0      $0              $6198           $12290         Sell
6       Pioneer Mutual Fund     20       $147   -$1470   $0      $0              $4728           $13760         Buy
6       Metro Properties Inc    30       $120   -$1800   $0      $0              $2928           $15560         Buy
6       Stryker Drilling Com    20       $120   -$2400   $0      $0              $528            $15560         Buy
6                               0        $0      $0      $240    $0              $768            $0             DividendAndInterest
6                               0        $0      $0      $0     -$778           -$10             $15560         MarginInterestCharge
7       Stryker Drilling Com    40       $75     $0      $0      $0             -$10             $0             Split
7       Stryker Drilling Com    20       $75     $1500   $0      $0              $1490           $15560         Sell
7       Pioneer Mutual Fund     10       $142    $1420   $0      $0              $2910           $15560         Sell
7       Metro Properties Inc    20       $131    $2620   $0      $0              $5530           $15560         Sell
7       Pioneer Mutual Fund     10       $142   -$710    $0      $0              $4820           $16270         Buy
7       Metro Properties Inc    30       $131   -$1965   $0      $0              $2855           $18235         Buy
7       Stryker Drilling Com    20       $75    -$1500   $0      $0              $1355           $18235         Buy
7                               0        $0      $0      $240    $0              $1595           $0             DividendAndInterest
7                               0        $0      $0      $0     -$912            $683            $18235         MarginInterestCharge
8       Pioneer Mutual Fund     120      $76     $0      $0      $0              $683            $0             Split
8       Stryker Drilling Com    20       $105    $2100   $0      $0              $2783           $18265         Sell
8       Pioneer Mutual Fund     10       $76     $760    $0      $0              $3543           $18265         Sell
8       Metro Properties Inc    20       $125    $2500   $0      $0              $6043           $18265         Sell
8       Pioneer Mutual Fund     30       $76    -$1140   $0      $0              $4903           $19405         Buy
8       Metro Properties Inc    30       $125   -$1875   $0      $0              $3028           $21280         Buy
8       Stryker Drilling Com    20       $105   -$2100   $0      $0              $928            $21280         Buy
8                               0        $0      $0      $560    $0              $1488           $0             DividendAndInterest
8                               0        $0      $0      $0     -$1064           $424            $21280         MarginInterestCharge
9       Stryker Drilling Com    20       $96     $1920   $0      $0              $2344           $21280         Sell
9       Pioneer Mutual Fund     10       $95     $950    $0      $0              $3294           $21280         Sell
9       Metro Properties Inc    20       $117    $2340   $0      $0              $5634           $21280         Sell
9       Pioneer Mutual Fund     20       $95    -$950    $0      $0              $4684           $22230         Buy
9       Metro Properties Inc    40       $117   -$2340   $0      $0              $2344           $24570         Buy
9       Stryker Drilling Com    20       $96    -$1920   $0      $0              $424            $24570         Buy
9                               0        $0      $0      $600    $0              $1024           $0             DividendAndInterest
9                               0        $0      $0      $0     -$1229          -$205            $24570         MarginInterestCharge
9                               0        $0      $0      $0     -$15490         -$15695          $9080          MarginInterestCharge
9                               0        $0      $0      $0     -$9080          -$24775          $0             MarginInterestCharge
10      Stryker Drilling Com    20       $141    $2820   $0      $0             -$21955          $0             Sell
10      Pioneer Mutual Fund     10       $94     $940    $0      $0             -$21015          $0             Sell
10      Metro Properties Inc    20       $119    $2380   $0      $0             -$18635          $0             Sell
10                              0        $0      $0      $560    $0             -$18075          $0             DividendAndInterest
Avg: 20355.49354005168 over 387 runs
Buys: TriCityTransportCompany MetroPropertiesInc UraniumEnterprisesInc GrowthCorporationOfAmerica StrykerDrillingCompany UnitedAutoCompany CentralCityMunicipalBonds PioneerMutualFund ValleyPowerAndLightCompany ShadyBrooksDevelopment
Sells: GrowthCorporationOfAmerica MetroPropertiesInc TriCityTransportCompany UraniumEnterprisesInc ValleyPowerAndLightCompany ShadyBrooksDevelopment PioneerMutualFund CentralCityMunicipalBonds UnitedAutoCompany StrykerDrillingCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
2       Tri-City Transport C    40       $123   -$2460   $0      $0              $2540           $2460          Buy
2       Metro Properties Inc    10       $121   -$605    $0      $0              $1935           $3065          Buy
2       Stryker Drilling Com    20       $137   -$1370   $0      $0              $565            $4435          Buy
2                               0        $0      $0      $0     -$222            $343            $4435          MarginInterestCharge
3       Metro Properties Inc    20       $76     $0      $0      $0              $343            $0             Split
3       Tri-City Transport C    20       $141    $2820   $0      $0              $3163           $4440          Sell
3       Stryker Drilling Com    20       $143    $1490   $0      $1370           $4653           $3070          Sell
3       Tri-City Transport C    30       $141   -$2115   $0      $0              $2538           $5185          Buy
3       Metro Properties Inc    10       $76    -$380    $0      $0              $2158           $5565          Buy
3       Stryker Drilling Com    20       $143   -$1430   $0      $0              $728            $6995          Buy
3                               0        $0      $0      $0     -$350            $378            $6995          MarginInterestCharge
4       Tri-City Transport C    20       $144    $2265   $0      $615            $2643           $6380          Sell
4       Stryker Drilling Com    20       $103    $630    $0      $1430           $3273           $4950          Sell
4       Tri-City Transport C    20       $144   -$1440   $0      $0              $1833           $6390          Buy
4       Metro Properties Inc    10       $84    -$420    $0      $0              $1413           $6810          Buy
4       Stryker Drilling Com    20       $103   -$1030   $0      $0              $383            $7840          Buy
4                               0        $0      $0      $0     -$392           -$9              $7840          MarginInterestCharge
5       Tri-City Transport C    100      $84     $0      $0      $0             -$9              $0             Split
5       Tri-City Transport C    20       $84     $1680   $0      $0              $1671           $7870          Sell
5       Stryker Drilling Com    20       $140    $1770   $0      $1030           $3441           $6840          Sell
5       Tri-City Transport C    40       $84    -$1680   $0      $0              $1761           $8520          Buy
5       Metro Properties Inc    10       $105   -$525    $0      $0              $1236           $9045          Buy
5                               0        $0      $0      $0     -$453            $783            $9045          MarginInterestCharge
6       Tri-City Transport C    20       $99     $1980   $0      $0              $2763           $9045          Sell
6       Tri-City Transport C    20       $99    -$990    $0      $0              $1773           $10035         Buy
6       Metro Properties Inc    10       $120   -$600    $0      $0              $1173           $10635         Buy
6                               0        $0      $0      $0     -$532            $641            $10635         MarginInterestCharge
7       Tri-City Transport C    20       $109    $1560   $0      $620            $2201           $10015         Sell
7       Tri-City Transport C    20       $109   -$1090   $0      $0              $1111           $11105         Buy
7       Metro Properties Inc    10       $131   -$655    $0      $0              $456            $11760         Buy
7                               0        $0      $0      $0     -$588           -$132            $11760         MarginInterestCharge
8       Tri-City Transport C    20       $115    $1680   $0      $620            $1548           $11140         Sell
8       Tri-City Transport C    10       $115   -$575    $0      $0              $973            $11715         Buy
8       Metro Properties Inc    10       $125   -$625    $0      $0              $348            $12340         Buy
8                               0        $0      $0      $0     -$617           -$269            $12340         MarginInterestCharge
9       Tri-City Transport C    220      $78     $0      $0      $0             -$269            $0             Split
9       Tri-City Transport C    20       $78     $1250   $0      $310            $981            $12085         Sell
9       Tri-City Transport C    10       $78    -$390    $0      $0              $591            $12475         Buy
9       Metro Properties Inc    10       $117   -$585    $0      $0              $6              $13060         Buy
9                               0        $0      $0      $0     -$653           -$647            $13060         MarginInterestCharge
9                               0        $0      $0      $0     -$4400          -$5047           $8660          MarginInterestCharge
9                               0        $0      $0      $0     -$8660          -$13707          $0             MarginInterestCharge
10      Tri-City Transport C    20       $84     $1680   $0      $0             -$12027          $0             Sell
</pre>
</details>

#### Buy and Hold

##### Always buy right away
This policy will buy as much of a given security as soon as there is available cash, and never sell.

Insights:
 * Stryker Drilling Company has the largest possible return, but also can become worthless.
 * Metro Properties Inc has a decent average with potential for high returns.
 * Stryker Drilling Company, United Auto Company, and Uranium Enterprises Inc all become worthless during at least one game.

```
./market.console -mode 1 -policy 0 -iterations 10000
```

<details>
<summary>full output</summary>
<pre>
AdjustStartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 10000
Policy                     = AlwaysBuy
NeuralMargin               = False
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = False
NeuralRandomResults        = 0

Security                   Policy     Average Min     Max
CentralCityMunicipalBonds  AlwaysBuy  $7900   $7900   $7900
GrowthCorporationOfAmerica AlwaysBuy  $13141  $3810   $27900
MetroPropertiesInc         AlwaysBuy  $14061  $4960   $34000
PioneerMutualFund          AlwaysBuy  $11430  $5320   $27940
ShadyBrooksDevelopment     AlwaysBuy  $10541  $4320   $20610
StrykerDrillingCompany     AlwaysBuy  $15984  $0      $281600
TriCityTransportCompany    AlwaysBuy  $13819  $2600   $30400
UnitedAutoCompany          AlwaysBuy  $13288  $0      $43820
UraniumEnterprisesInc      AlwaysBuy  $12223  $0      $45220
ValleyPowerAndLightCompany AlwaysBuy  $12003  $6300   $22390
</pre>
</details>

#### Always buy low
This policy waits for the security to be below starting value and then buys, and never sells.

Insights:
 * Stryker Drilling Company has the largest possible returns, and can also become worthless.
 * Tri-City Transport Company and Metro Properties Inc seem like a reasonable balance of longterm growth with less risk.
 * Stryker Drilling Company, United Auto Company, and Uranium Enterprises Inc all become worthless during at least one game.

```
./market.console -mode 1 -policy 1 -iterations 10000
```

<details>
<summary>full output</summary>
<pre>
AdjustStartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 10000
Policy                     = AlwaysBuyLow
NeuralMargin               = False
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = False
NeuralRandomResults        = 0

Security                   Policy          Average Min     Max
CentralCityMunicipalBonds  AlwaysBuyLow    $7900   $7900   $7900
GrowthCorporationOfAmerica AlwaysBuyLow    $12767  $4520   $29550
MetroPropertiesInc         AlwaysBuyLow    $13665  $4040   $32000
PioneerMutualFund          AlwaysBuyLow    $11154  $5000   $25470
ShadyBrooksDevelopment     AlwaysBuyLow    $10036  $4160   $19580
StrykerDrillingCompany     AlwaysBuyLow    $15700  $0      $227200
TriCityTransportCompany    AlwaysBuyLow    $13601  $3740   $35200
UnitedAutoCompany          AlwaysBuyLow    $12821  $10     $49080
UraniumEnterprisesInc      AlwaysBuyLow    $11744  $0      $46460
ValleyPowerAndLightCompany AlwaysBuyLow    $11683  $4490   $19980
</pre>
</details>

##### Always buy on margin
Always buy on margin will buy as much of the security on margin as soon as possible, and never sell.

Insights:
 * Valley Power and Light Company seems like a reasonable balance of good average returns and a decent miniumum.
 * Stryker Drilling Company, United Auto Company, and Uranium Enterprises Inc all become worthless during at least one game.

```
./market.console.exe -mode 1 -policy 2 -iterations 10000
```

<details>
<summary>full output</summary>
<pre>
AdjustStartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 10000
Policy                     = AlwaysOnMargin
NeuralMargin               = False
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = False
NeuralRandomResults        = 0

Security                   Policy          Average Min     Max
CentralCityMunicipalBonds  AlwaysOnMargin  $7400   $7400   $7400
GrowthCorporationOfAmerica AlwaysOnMargin  $13325  $3945   $31252
MetroPropertiesInc         AlwaysOnMargin  $14028  $1352   $35636
PioneerMutualFund          AlwaysOnMargin  $11711  $3389   $27403
ShadyBrooksDevelopment     AlwaysOnMargin  $10927  $554    $23835
StrykerDrillingCompany     AlwaysOnMargin  $16852  $-5013  $198869
TriCityTransportCompany    AlwaysOnMargin  $13829  $206    $36279
UnitedAutoCompany          AlwaysOnMargin  $13861  $-4374  $52356
UraniumEnterprisesInc      AlwaysOnMargin  $13394  $-7279  $65937
ValleyPowerAndLightCompany AlwaysOnMargin  $12249  $4152   $22139
</pre>
</details>

#### Random
The random bots make all decisions completely randomly.  A not so surprising outcome happened, when the security order was determined randomly, the order that had Stryker Drilling Company first would top perform.  The experiment was run with static security ordering, and the results are more expected.  The random bot used all its money the first, sold everything on the second and bought new things.  The average networth were lower than the neural networks.

```
./market.console -mode 1 -policy 3 -iterations 1000 -neuralrandom 1 -neuralstatic
```

<details>
<summary>full output</summary>
<pre>
AdjustStartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 1000
Policy                     = NeuralBots
NeuralMargin               = False
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = True
NeuralRandomResults        = 1

Avg: 12700 over 2 runs
Buys: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Sells: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Central City Municip    10       $100   -$1000   $0      $0              $4000           $0             Buy
1       Growth Corporation O    20       $100   -$2000   $0      $0              $2000           $0             Buy
1       Metro Properties Inc    20       $100   -$2000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $70     $0              $70             $0             DividendAndInterest
2                               0        $0      $0      $70     $0              $140            $0             DividendAndInterest
3       Metro Properties Inc    40       $78     $0      $0      $0              $140            $0             Split
3       Central City Municip    10       $100    $1000   $0      $0              $1140           $0             Sell
3       Growth Corporation O    10       $131    $1310   $0      $0              $2450           $0             Sell
3       Metro Properties Inc    20       $78     $1560   $0      $0              $4010           $0             Sell
3       Central City Municip    10       $100   -$1000   $0      $0              $3010           $0             Buy
3       Metro Properties Inc    20       $78    -$1560   $0      $0              $1450           $0             Buy
3       Pioneer Mutual Fund     10       $109   -$1090   $0      $0              $360            $0             Buy
3                               0        $0      $0      $100    $0              $460            $0             DividendAndInterest
4       Metro Properties Inc    10       $109    $1090   $0      $0              $1550           $0             Sell
4       Growth Corporation O    10       $144   -$1440   $0      $0              $110            $0             Buy
4                               0        $0      $0      $110    $0              $220            $0             DividendAndInterest
5       Growth Corporation O    40       $81     $0      $0      $0              $220            $0             Split
5       Central City Municip    10       $100    $1000   $0      $0              $1220           $0             Sell
5       Metro Properties Inc    10       $130    $1300   $0      $0              $2520           $0             Sell
5       Pioneer Mutual Fund     10       $123    $1230   $0      $0              $3750           $0             Sell
5       Central City Municip    20       $100   -$2000   $0      $0              $1750           $0             Buy
5       Pioneer Mutual Fund     10       $123   -$1230   $0      $0              $520            $0             Buy
5                               0        $0      $0      $180    $0              $700            $0             DividendAndInterest
6       Central City Municip    20       $100    $2000   $0      $0              $2700           $0             Sell
6       Metro Properties Inc    10       $122    $1220   $0      $0              $3920           $0             Sell
6       Pioneer Mutual Fund     10       $142    $1420   $0      $0              $5340           $0             Sell
6       Central City Municip    10       $100   -$1000   $0      $0              $4340           $0             Buy
6       Growth Corporation O    10       $95    -$950    $0      $0              $3390           $0             Buy
6       Metro Properties Inc    20       $122   -$2440   $0      $0              $950            $0             Buy
6                               0        $0      $0      $100    $0              $1050           $0             DividendAndInterest
7       Growth Corporation O    10       $101    $1010   $0      $0              $2060           $0             Sell
7       Central City Municip    20       $100   -$2000   $0      $0              $60             $0             Buy
7                               0        $0      $0      $190    $0              $250            $0             DividendAndInterest
8                               0        $0      $0      $190    $0              $440            $0             DividendAndInterest
9       Metro Properties Inc    60       $84     $0      $0      $0              $440            $0             Split
9                               0        $0      $0      $190    $0              $630            $0             DividendAndInterest
10      Central City Municip    20       $100    $2000   $0      $0              $2630           $0             Sell
10      Metro Properties Inc    10       $88    -$880    $0      $0              $1750           $0             Buy
10      Pioneer Mutual Fund     20       $83    -$1660   $0      $0              $90             $0             Buy
10                              0        $0      $0      $170    $0              $260            $0             DividendAndInterest
Avg: 12496.666666666666 over 162 runs
Buys: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Sells: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Central City Municip    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Metro Properties Inc    10       $100   -$1000   $0      $0              $2000           $0             Buy
1       Pioneer Mutual Fund     20       $100   -$2000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $180    $0              $180            $0             DividendAndInterest
2                               0        $0      $0      $180    $0              $360            $0             DividendAndInterest
3       Metro Properties Inc    20       $78     $0      $0      $0              $360            $0             Split
3       Central City Municip    10       $100    $1000   $0      $0              $1360           $0             Sell
3       Growth Corporation O    10       $131   -$1310   $0      $0              $50             $0             Buy
3                               0        $0      $0      $140    $0              $190            $0             DividendAndInterest
4       Pioneer Mutual Fund     10       $110    $1100   $0      $0              $1290           $0             Sell
4       Central City Municip    10       $100   -$1000   $0      $0              $290            $0             Buy
4                               0        $0      $0      $150    $0              $440            $0             DividendAndInterest
5       Growth Corporation O    20       $81     $0      $0      $0              $440            $0             Split
5       Metro Properties Inc    10       $130    $1300   $0      $0              $1740           $0             Sell
5       Growth Corporation O    20       $81    -$1620   $0      $0              $120            $0             Buy
5                               0        $0      $0      $180    $0              $300            $0             DividendAndInterest
6       Central City Municip    20       $100    $2000   $0      $0              $2300           $0             Sell
6       Growth Corporation O    10       $95     $950    $0      $0              $3250           $0             Sell
6       Pioneer Mutual Fund     10       $142    $1420   $0      $0              $4670           $0             Sell
6       Central City Municip    40       $100   -$4000   $0      $0              $670            $0             Buy
6                               0        $0      $0      $230    $0              $900            $0             DividendAndInterest
7       Central City Municip    10       $100    $1000   $0      $0              $1900           $0             Sell
7       Metro Properties Inc    10       $126   -$1260   $0      $0              $640            $0             Buy
7       Stryker Drilling Com    10       $60    -$600    $0      $0              $40             $0             Buy
7                               0        $0      $0      $180    $0              $220            $0             DividendAndInterest
8       Growth Corporation O    10       $88     $880    $0      $0              $1100           $0             Sell
8       Metro Properties Inc    20       $139    $2780   $0      $0              $3880           $0             Sell
8       Growth Corporation O    40       $88    -$3520   $0      $0              $360            $0             Buy
8                               0        $0      $0      $210    $0              $570            $0             DividendAndInterest
9       Growth Corporation O    10       $115    $1150   $0      $0              $1720           $0             Sell
9       Central City Municip    10       $100   -$1000   $0      $0              $720            $0             Buy
9                               0        $0      $0      $250    $0              $970            $0             DividendAndInterest
10      Growth Corporation O    10       $121    $1210   $0      $0              $2180           $0             Sell
10      Central City Municip    10       $100   -$1000   $0      $0              $1180           $0             Buy
10      Uranium Enterprises     10       $55    -$550    $0      $0              $630            $0             Buy
10                              0        $0      $0      $350    $0              $980            $0             DividendAndInterest
Avg: 12262.545454545454 over 165 runs
Buys: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Sells: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Central City Municip    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Growth Corporation O    10       $100   -$1000   $0      $0              $2000           $0             Buy
1       Metro Properties Inc    10       $100   -$1000   $0      $0              $1000           $0             Buy
1       Shady Brooks Develop    10       $100   -$1000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $180    $0              $180            $0             DividendAndInterest
2       Central City Municip    10       $100    $1000   $0      $0              $1180           $0             Sell
2       Central City Municip    10       $100   -$1000   $0      $0              $180            $0             Buy
2                               0        $0      $0      $180    $0              $360            $0             DividendAndInterest
3       Metro Properties Inc    20       $78     $0      $0      $0              $360            $0             Split
3       Growth Corporation O    10       $131    $1310   $0      $0              $1670           $0             Sell
3       Central City Municip    10       $100   -$1000   $0      $0              $670            $0             Buy
3                               0        $0      $0      $220    $0              $890            $0             DividendAndInterest
4       Central City Municip    20       $100    $2000   $0      $0              $2890           $0             Sell
4       Central City Municip    10       $100   -$1000   $0      $0              $1890           $0             Buy
4       Shady Brooks Develop    10       $137   -$1370   $0      $0              $520            $0             Buy
4                               0        $0      $0      $240    $0              $760            $0             DividendAndInterest
5       Central City Municip    20       $100    $2000   $0      $0              $2760           $0             Sell
5       Central City Municip    20       $100   -$2000   $0      $0              $760            $0             Buy
5                               0        $0      $0      $240    $0              $1000           $0             DividendAndInterest
6       Central City Municip    10       $100    $1000   $0      $0              $2000           $0             Sell
6       Shady Brooks Develop    10       $134    $1340   $0      $0              $3340           $0             Sell
6       Central City Municip    20       $100   -$2000   $0      $0              $1340           $0             Buy
6       Growth Corporation O    10       $95    -$950    $0      $0              $390            $0             Buy
6                               0        $0      $0      $230    $0              $620            $0             DividendAndInterest
7       Central City Municip    10       $100    $1000   $0      $0              $1620           $0             Sell
7       Growth Corporation O    10       $101   -$1010   $0      $0              $610            $0             Buy
7       Stryker Drilling Com    10       $60    -$600    $0      $0              $10             $0             Buy
7                               0        $0      $0      $190    $0              $200            $0             DividendAndInterest
8       Growth Corporation O    10       $88     $880    $0      $0              $1080           $0             Sell
8       Metro Properties Inc    10       $139    $1390   $0      $0              $2470           $0             Sell
8       Central City Municip    20       $100   -$2000   $0      $0              $470            $0             Buy
8                               0        $0      $0      $280    $0              $750            $0             DividendAndInterest
9       Metro Properties Inc    20       $84     $0      $0      $0              $750            $0             Split
9       Shady Brooks Develop    20       $76     $0      $0      $0              $750            $0             Split
9       Growth Corporation O    10       $115    $1150   $0      $0              $1900           $0             Sell
9       Metro Properties Inc    20       $84     $1680   $0      $0              $3580           $0             Sell
9       Shady Brooks Develop    20       $76     $1520   $0      $0              $5100           $0             Sell
9       Central City Municip    50       $100   -$5000   $0      $0              $100            $0             Buy
9                               0        $0      $0      $450    $0              $550            $0             DividendAndInterest
10      Central City Municip    10       $100    $1000   $0      $0              $1550           $0             Sell
10      Stryker Drilling Com    10       $65     $650    $0      $0              $2200           $0             Sell
10      Central City Municip    10       $100   -$1000   $0      $0              $1200           $0             Buy
10      Shady Brooks Develop    10       $79    -$790    $0      $0              $410            $0             Buy
10                              0        $0      $0      $520    $0              $930            $0             DividendAndInterest
Avg: 12250 over 1 runs
Buys: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Sells: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Central City Municip    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Growth Corporation O    10       $100   -$1000   $0      $0              $2000           $0             Buy
1       Metro Properties Inc    20       $100   -$2000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $110    $0              $110            $0             DividendAndInterest
2       Growth Corporation O    10       $113    $1130   $0      $0              $1240           $0             Sell
2       Central City Municip    10       $100   -$1000   $0      $0              $240            $0             Buy
2                               0        $0      $0      $150    $0              $390            $0             DividendAndInterest
3       Metro Properties Inc    40       $78     $0      $0      $0              $390            $0             Split
3       Metro Properties Inc    10       $78     $780    $0      $0              $1170           $0             Sell
3       Central City Municip    10       $100   -$1000   $0      $0              $170            $0             Buy
3                               0        $0      $0      $200    $0              $370            $0             DividendAndInterest
4       Central City Municip    10       $100    $1000   $0      $0              $1370           $0             Sell
4       Stryker Drilling Com    10       $113   -$1130   $0      $0              $240            $0             Buy
4                               0        $0      $0      $150    $0              $390            $0             DividendAndInterest
5       Stryker Drilling Com    20       $84     $0      $0      $0              $390            $0             Split
5       Central City Municip    10       $100    $1000   $0      $0              $1390           $0             Sell
5       Metro Properties Inc    20       $130    $2600   $0      $0              $3990           $0             Sell
5       Central City Municip    10       $100   -$1000   $0      $0              $2990           $0             Buy
5       Growth Corporation O    10       $81    -$810    $0      $0              $2180           $0             Buy
5       Pioneer Mutual Fund     10       $123   -$1230   $0      $0              $950            $0             Buy
5       United Auto Company     10       $83    -$830    $0      $0              $120            $0             Buy
5                               0        $0      $0      $220    $0              $340            $0             DividendAndInterest
6       Pioneer Mutual Fund     10       $142    $1420   $0      $0              $1760           $0             Sell
6       Stryker Drilling Com    20       $75    -$1500   $0      $0              $260            $0             Buy
6                               0        $0      $0      $180    $0              $440            $0             DividendAndInterest
7                               0        $0      $0      $180    $0              $620            $0             DividendAndInterest
8       Central City Municip    20       $100    $2000   $0      $0              $2620           $0             Sell
8       Stryker Drilling Com    20       $85     $1700   $0      $0              $4320           $0             Sell
8       Metro Properties Inc    30       $139   -$4170   $0      $0              $150            $0             Buy
8                               0        $0      $0      $80     $0              $230            $0             DividendAndInterest
9       Metro Properties Inc    80       $84     $0      $0      $0              $230            $0             Split
9       Growth Corporation O    10       $115    $1150   $0      $0              $1380           $0             Sell
9       Central City Municip    10       $100   -$1000   $0      $0              $380            $0             Buy
9                               0        $0      $0      $120    $0              $500            $0             DividendAndInterest
10      United Auto Company     10       $127    $1270   $0      $0              $1770           $0             Sell
10      Metro Properties Inc    10       $88    -$880    $0      $0              $890            $0             Buy
10      Pioneer Mutual Fund     10       $83    -$830    $0      $0              $60             $0             Buy
10                              0        $0      $0      $140    $0              $200            $0             DividendAndInterest
Avg: 12023.636363636364 over 11 runs
Buys: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Sells: CentralCityMunicipalBonds GrowthCorporationOfAmerica MetroPropertiesInc PioneerMutualFund ShadyBrooksDevelopment StrykerDrillingCompany TriCityTransportCompany UnitedAutoCompany UraniumEnterprisesInc ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Central City Municip    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Growth Corporation O    20       $100   -$2000   $0      $0              $1000           $0             Buy
1       Stryker Drilling Com    10       $100   -$1000   $0      $0              $0              $0             Buy
1                               0        $0      $0      $120    $0              $120            $0             DividendAndInterest
2       Stryker Drilling Com    20       $78     $0      $0      $0              $120            $0             Split
2       Stryker Drilling Com    10       $78     $780    $0      $0              $900            $0             Sell
2       Stryker Drilling Com    10       $78    -$780    $0      $0              $120            $0             Buy
2                               0        $0      $0      $120    $0              $240            $0             DividendAndInterest
3       Central City Municip    20       $100    $2000   $0      $0              $2240           $0             Sell
3       Growth Corporation O    10       $131    $1310   $0      $0              $3550           $0             Sell
3       Central City Municip    30       $100   -$3000   $0      $0              $550            $0             Buy
3                               0        $0      $0      $160    $0              $710            $0             DividendAndInterest
4       Central City Municip    10       $100    $1000   $0      $0              $1710           $0             Sell
4       Growth Corporation O    10       $144    $1440   $0      $0              $3150           $0             Sell
4       Central City Municip    30       $100   -$3000   $0      $0              $150            $0             Buy
4                               0        $0      $0      $250    $0              $400            $0             DividendAndInterest
5       Stryker Drilling Com    40       $84     $0      $0      $0              $400            $0             Split
5                               0        $0      $0      $250    $0              $650            $0             DividendAndInterest
6                               0        $0      $0      $250    $0              $900            $0             DividendAndInterest
7       Stryker Drilling Com    10       $60     $600    $0      $0              $1500           $0             Sell
7       Metro Properties Inc    10       $126   -$1260   $0      $0              $240            $0             Buy
7                               0        $0      $0      $250    $0              $490            $0             DividendAndInterest
8       Central City Municip    10       $100    $1000   $0      $0              $1490           $0             Sell
8       Central City Municip    10       $100   -$1000   $0      $0              $490            $0             Buy
8                               0        $0      $0      $250    $0              $740            $0             DividendAndInterest
9       Metro Properties Inc    20       $84     $0      $0      $0              $740            $0             Split
9                               0        $0      $0      $250    $0              $990            $0             DividendAndInterest
10      Central City Municip    10       $100    $1000   $0      $0              $1990           $0             Sell
10      Metro Properties Inc    20       $88     $1760   $0      $0              $3750           $0             Sell
10      Stryker Drilling Com    10       $65     $650    $0      $0              $4400           $0             Sell
10      Central City Municip    20       $100   -$2000   $0      $0              $2400           $0             Buy
10      Growth Corporation O    10       $121   -$1210   $0      $0              $1190           $0             Buy
10      Pioneer Mutual Fund     10       $83    -$830    $0      $0              $360            $0             Buy
10                              0        $0      $0      $350    $0              $710            $0             DividendAndInterest
</pre>
</details>

#### Constant seed
This experiment set the seed as a constant, so the only variability between runs was the networks initial strating values.

```
./market.console -mode 1 -policy 3 -iterations 100000 -seed 123456
```

This seed has Tri-City Transport Company receiving a 2-for-1 split on year 5, and Stryker Drilling Company splittig on years 3 and 6.  The models that were able to accumuate the most of these two companies had the best networths.  The 5 best networks all retained their top positions for at least 11K iterations (max 96K).

Price schedule for this seed:

Security              | Year 1 | Year 2 | Year 3 | Year 4 | Year 5 | Year 6 | Year 7 | Year 8 | Year 9 | Year 10 |
----------------------|--------|--------|--------|--------|--------|--------|--------|--------|--------|---------|
Market Situation      | Bull   | Bear   | Bear   | Bear   | Bull   | Bear   | Bull   | Bull   | Bear   | Bear    |
Central City Municip  | $100   | $100   | $100   | $100   | $100   | $100   | $100   | $100   | $100   | $100    |
Growth Corporation O  | $100   | $108   | $115   | $123   | $136   | $141   | S$80   | $106   | $113   | $121    |
Metro Properties Inc  | $105   | $111   | $115   | $121   | S$76   | $83    | $107   | $123   | $121   | $127    |
Pioneer Mutual Fund   | $100   | $104   | $114   | $118   | $119   | $118   | $135   | S$80   | $85    | $89     |
Shady Brooks Develop  | $100   | $96    | $86    | $82    | $96    | $93    | $102   | $110   | $104   | $100    |
Stryker Drilling Com  | $100   | $140   | S$85   | $125   | $114   | S$80   | $75    | $61    | $21    | $61     |
Tri-City Transport C  | $100   | $108   | $114   | $122   | S$75   | $86    | $112   | $133   | $136   | $144    |
United Auto Company   | $100   | $89    | $70    | $74    | $92    | $82    | $95    | $109   | $125   | $114    |
Uranium Enterprises   | $100   | $88    | $110   | $73    | $59    | $69    | $62    | $68    | $67    | $55     |
Valley Power And Lig  | $100   | $103   | $101   | $104   | $114   | $118   | $138   | S$78   | $82    | $85     |

Buy Stryker and hold - Net worth = $12K
Buy Striker (30) and Tri-City (20) and hold - Net worth = $13K
The network found that if every turn it sells 20 Stryker and buy 20 Pioneer and at the end buy as much Stryker as possible, resulting in a end networth >$20K.

Network pattern:
_       | Sell      | Buy       |
--------|-----------|-----------|
Year 1  |           | 6,20 5,30 |
Year 2  | 5,20 6,10 | 6,20 5,10 |
Year 3  | 5,20 6,10 | 6,20 5,10 |
Year 4  | 5,20 6,10 | 6,20 5,10 |
Year 5  | 5,20 6,10 | 6,20 5,10 |
Year 6  | 5,20 6,10 | 6,20 5,10 |
Year 7  |      6,10 | 5,20      |
Year 8  | 5,20 6,10 | 5,40      |
Year 9  | 5,20 6,10 | 5,90      |
Year 10 | 5,20 6,10 | 5,40      |

The network had perfect hindsight, as it had played this exact same game 10K times.  The networked that learned how to play this pattern resulted in the highest average networth.

<details>
<summary>full output</summary>
<pre>
Seed                       = 123456
Adjus6StartingPrices       = False
DividendBasedOnMarketPrice = False
InitialCashBalance         = 5000
MarginInterestDue          = 5
MarginSplitRatio           = 2
MarginStockMustBeBought    = 25
LastYear                   = 10
NoDividendPrice            = 50
ParValue                   = 100
PurchaseDivisor            = 10
StockSplitPrice            = 150
WorthlessStockPrice        = 0
WithDebugValidation        = False
Iterations                 = 100000
Policy                     = NeuralBots
NeuralMargin               = False
NeuralLearningRate         = 0.00015
NeuralStaticSecurityOrder  = False
NeuralRandomResults        = 0

Avg: 20290 over 14225 runs
Buys: TriCityTransportCompany StrykerDrillingCompany CentralCityMunicipalBonds ShadyBrooksDevelopment UnitedAutoCompany UraniumEnterprisesInc MetroPropertiesInc GrowthCorporationOfAmerica ValleyPowerAndLightCompany PioneerMutualFund
Sells: ValleyPowerAndLightCompany UnitedAutoCompany GrowthCorporationOfAmerica ShadyBrooksDevelopment CentralCityMunicipalBonds UraniumEnterprisesInc StrykerDrillingCompany PioneerMutualFund TriCityTransportCompany MetroPropertiesInc
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Tri-City Transport C    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Stryker Drilling Com    30       $100   -$3000   $0      $0              $0              $0             Buy
2       Stryker Drilling Com    20       $140    $2800   $0      $0              $2800           $0             Sell
2       Tri-City Transport C    10       $108    $1080   $0      $0              $3880           $0             Sell
2       Tri-City Transport C    20       $108   -$2160   $0      $0              $1720           $0             Buy
2       Stryker Drilling Com    10       $140   -$1400   $0      $0              $320            $0             Buy
3       Stryker Drilling Com    40       $85     $0      $0      $0              $320            $0             Split
3       Stryker Drilling Com    20       $85     $1700   $0      $0              $2020           $0             Sell
3       Tri-City Transport C    10       $114    $1140   $0      $0              $3160           $0             Sell
3       Tri-City Transport C    20       $114   -$2280   $0      $0              $880            $0             Buy
3       Stryker Drilling Com    10       $85    -$850    $0      $0              $30             $0             Buy
4       Stryker Drilling Com    20       $125    $2500   $0      $0              $2530           $0             Sell
4       Tri-City Transport C    10       $122    $1220   $0      $0              $3750           $0             Sell
4       Tri-City Transport C    20       $122   -$2440   $0      $0              $1310           $0             Buy
4       Stryker Drilling Com    10       $125   -$1250   $0      $0              $60             $0             Buy
5       Tri-City Transport C    100      $75     $0      $0      $0              $60             $0             Split
5       Stryker Drilling Com    20       $114    $2280   $0      $0              $2340           $0             Sell
5       Tri-City Transport C    10       $75     $750    $0      $0              $3090           $0             Sell
5       Tri-City Transport C    20       $75    -$1500   $0      $0              $1590           $0             Buy
5       Stryker Drilling Com    10       $114   -$1140   $0      $0              $450            $0             Buy
6       Stryker Drilling Com    20       $80     $0      $0      $0              $450            $0             Split
6       Stryker Drilling Com    20       $80     $1600   $0      $0              $2050           $0             Sell
6       Tri-City Transport C    10       $86     $860    $0      $0              $2910           $0             Sell
6       Tri-City Transport C    20       $86    -$1720   $0      $0              $1190           $0             Buy
6       Stryker Drilling Com    10       $80    -$800    $0      $0              $390            $0             Buy
7       Tri-City Transport C    10       $112    $1120   $0      $0              $1510           $0             Sell
7       Stryker Drilling Com    20       $75    -$1500   $0      $0              $10             $0             Buy
8       Stryker Drilling Com    20       $61     $1220   $0      $0              $1230           $0             Sell
8       Tri-City Transport C    10       $133    $1330   $0      $0              $2560           $0             Sell
8       Stryker Drilling Com    40       $61    -$2440   $0      $0              $120            $0             Buy
9       Stryker Drilling Com    20       $21     $420    $0      $0              $540            $0             Sell
9       Tri-City Transport C    10       $136    $1360   $0      $0              $1900           $0             Sell
9       Stryker Drilling Com    90       $21    -$1890   $0      $0              $10             $0             Buy
10      Stryker Drilling Com    20       $61     $1220   $0      $0              $1230           $0             Sell
10      Tri-City Transport C    10       $144    $1440   $0      $0              $2670           $0             Sell
10      Stryker Drilling Com    40       $61    -$2440   $0      $0              $230            $0             Buy
Avg: 20290 over 12949 runs
Buys: TriCityTransportCompany StrykerDrillingCompany UraniumEnterprisesInc ValleyPowerAndLightCompany CentralCityMunicipalBonds PioneerMutualFund UnitedAutoCompany ShadyBrooksDevelopment GrowthCorporationOfAmerica MetroPropertiesInc
Sells: StrykerDrillingCompany MetroPropertiesInc GrowthCorporationOfAmerica UnitedAutoCompany CentralCityMunicipalBonds TriCityTransportCompany ShadyBrooksDevelopment UraniumEnterprisesInc PioneerMutualFund ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Tri-City Transport C    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Stryker Drilling Com    30       $100   -$3000   $0      $0              $0              $0             Buy
2       Stryker Drilling Com    20       $140    $2800   $0      $0              $2800           $0             Sell
2       Tri-City Transport C    10       $108    $1080   $0      $0              $3880           $0             Sell
2       Tri-City Transport C    20       $108   -$2160   $0      $0              $1720           $0             Buy
2       Stryker Drilling Com    10       $140   -$1400   $0      $0              $320            $0             Buy
3       Stryker Drilling Com    40       $85     $0      $0      $0              $320            $0             Split
3       Stryker Drilling Com    20       $85     $1700   $0      $0              $2020           $0             Sell
3       Tri-City Transport C    10       $114    $1140   $0      $0              $3160           $0             Sell
3       Tri-City Transport C    20       $114   -$2280   $0      $0              $880            $0             Buy
3       Stryker Drilling Com    10       $85    -$850    $0      $0              $30             $0             Buy
4       Stryker Drilling Com    20       $125    $2500   $0      $0              $2530           $0             Sell
4       Tri-City Transport C    10       $122    $1220   $0      $0              $3750           $0             Sell
4       Tri-City Transport C    20       $122   -$2440   $0      $0              $1310           $0             Buy
4       Stryker Drilling Com    10       $125   -$1250   $0      $0              $60             $0             Buy
5       Tri-City Transport C    100      $75     $0      $0      $0              $60             $0             Split
5       Stryker Drilling Com    20       $114    $2280   $0      $0              $2340           $0             Sell
5       Tri-City Transport C    10       $75     $750    $0      $0              $3090           $0             Sell
5       Tri-City Transport C    20       $75    -$1500   $0      $0              $1590           $0             Buy
5       Stryker Drilling Com    10       $114   -$1140   $0      $0              $450            $0             Buy
6       Stryker Drilling Com    20       $80     $0      $0      $0              $450            $0             Split
6       Stryker Drilling Com    20       $80     $1600   $0      $0              $2050           $0             Sell
6       Tri-City Transport C    10       $86     $860    $0      $0              $2910           $0             Sell
6       Tri-City Transport C    20       $86    -$1720   $0      $0              $1190           $0             Buy
6       Stryker Drilling Com    10       $80    -$800    $0      $0              $390            $0             Buy
7       Tri-City Transport C    10       $112    $1120   $0      $0              $1510           $0             Sell
7       Stryker Drilling Com    20       $75    -$1500   $0      $0              $10             $0             Buy
8       Stryker Drilling Com    20       $61     $1220   $0      $0              $1230           $0             Sell
8       Tri-City Transport C    10       $133    $1330   $0      $0              $2560           $0             Sell
8       Stryker Drilling Com    40       $61    -$2440   $0      $0              $120            $0             Buy
9       Stryker Drilling Com    20       $21     $420    $0      $0              $540            $0             Sell
9       Tri-City Transport C    10       $136    $1360   $0      $0              $1900           $0             Sell
9       Stryker Drilling Com    90       $21    -$1890   $0      $0              $10             $0             Buy
10      Stryker Drilling Com    20       $61     $1220   $0      $0              $1230           $0             Sell
10      Tri-City Transport C    10       $144    $1440   $0      $0              $2670           $0             Sell
10      Stryker Drilling Com    40       $61    -$2440   $0      $0              $230            $0             Buy
Avg: 20289.99622562928 over 76834 runs
Buys: TriCityTransportCompany StrykerDrillingCompany ShadyBrooksDevelopment GrowthCorporationOfAmerica ValleyPowerAndLightCompany CentralCityMunicipalBonds MetroPropertiesInc UnitedAutoCompany PioneerMutualFund UraniumEnterprisesInc
Sells: TriCityTransportCompany StrykerDrillingCompany CentralCityMunicipalBonds UnitedAutoCompany UraniumEnterprisesInc MetroPropertiesInc ValleyPowerAndLightCompany ShadyBrooksDevelopment PioneerMutualFund GrowthCorporationOfAmerica
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Tri-City Transport C    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Stryker Drilling Com    30       $100   -$3000   $0      $0              $0              $0             Buy
2       Tri-City Transport C    10       $108    $1080   $0      $0              $1080           $0             Sell
2       Stryker Drilling Com    20       $140    $2800   $0      $0              $3880           $0             Sell
2       Tri-City Transport C    20       $108   -$2160   $0      $0              $1720           $0             Buy
2       Stryker Drilling Com    10       $140   -$1400   $0      $0              $320            $0             Buy
3       Stryker Drilling Com    40       $85     $0      $0      $0              $320            $0             Split
3       Tri-City Transport C    10       $114    $1140   $0      $0              $1460           $0             Sell
3       Stryker Drilling Com    20       $85     $1700   $0      $0              $3160           $0             Sell
3       Tri-City Transport C    20       $114   -$2280   $0      $0              $880            $0             Buy
3       Stryker Drilling Com    10       $85    -$850    $0      $0              $30             $0             Buy
4       Tri-City Transport C    10       $122    $1220   $0      $0              $1250           $0             Sell
4       Stryker Drilling Com    20       $125    $2500   $0      $0              $3750           $0             Sell
4       Tri-City Transport C    20       $122   -$2440   $0      $0              $1310           $0             Buy
4       Stryker Drilling Com    10       $125   -$1250   $0      $0              $60             $0             Buy
5       Tri-City Transport C    100      $75     $0      $0      $0              $60             $0             Split
5       Tri-City Transport C    10       $75     $750    $0      $0              $810            $0             Sell
5       Stryker Drilling Com    20       $114    $2280   $0      $0              $3090           $0             Sell
5       Tri-City Transport C    20       $75    -$1500   $0      $0              $1590           $0             Buy
5       Stryker Drilling Com    10       $114   -$1140   $0      $0              $450            $0             Buy
6       Stryker Drilling Com    20       $80     $0      $0      $0              $450            $0             Split
6       Tri-City Transport C    10       $86     $860    $0      $0              $1310           $0             Sell
6       Stryker Drilling Com    20       $80     $1600   $0      $0              $2910           $0             Sell
6       Tri-City Transport C    20       $86    -$1720   $0      $0              $1190           $0             Buy
6       Stryker Drilling Com    10       $80    -$800    $0      $0              $390            $0             Buy
7       Tri-City Transport C    10       $112    $1120   $0      $0              $1510           $0             Sell
7       Stryker Drilling Com    20       $75    -$1500   $0      $0              $10             $0             Buy
8       Tri-City Transport C    10       $133    $1330   $0      $0              $1340           $0             Sell
8       Stryker Drilling Com    20       $61     $1220   $0      $0              $2560           $0             Sell
8       Stryker Drilling Com    40       $61    -$2440   $0      $0              $120            $0             Buy
9       Tri-City Transport C    10       $136    $1360   $0      $0              $1480           $0             Sell
9       Stryker Drilling Com    20       $21     $420    $0      $0              $1900           $0             Sell
9       Stryker Drilling Com    90       $21    -$1890   $0      $0              $10             $0             Buy
10      Tri-City Transport C    10       $144    $1440   $0      $0              $1450           $0             Sell
10      Stryker Drilling Com    20       $61     $1220   $0      $0              $2670           $0             Sell
10      Stryker Drilling Com    40       $61    -$2440   $0      $0              $230            $0             Buy
Avg: 20289.44512154586 over 90254 runs
Buys: TriCityTransportCompany StrykerDrillingCompany MetroPropertiesInc CentralCityMunicipalBonds ShadyBrooksDevelopment UnitedAutoCompany UraniumEnterprisesInc GrowthCorporationOfAmerica ValleyPowerAndLightCompany PioneerMutualFund
Sells: TriCityTransportCompany PioneerMutualFund CentralCityMunicipalBonds ShadyBrooksDevelopment StrykerDrillingCompany MetroPropertiesInc GrowthCorporationOfAmerica UraniumEnterprisesInc ValleyPowerAndLightCompany UnitedAutoCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Tri-City Transport C    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Stryker Drilling Com    30       $100   -$3000   $0      $0              $0              $0             Buy
2       Tri-City Transport C    10       $108    $1080   $0      $0              $1080           $0             Sell
2       Stryker Drilling Com    20       $140    $2800   $0      $0              $3880           $0             Sell
2       Tri-City Transport C    20       $108   -$2160   $0      $0              $1720           $0             Buy
2       Stryker Drilling Com    10       $140   -$1400   $0      $0              $320            $0             Buy
3       Stryker Drilling Com    40       $85     $0      $0      $0              $320            $0             Split
3       Tri-City Transport C    10       $114    $1140   $0      $0              $1460           $0             Sell
3       Stryker Drilling Com    20       $85     $1700   $0      $0              $3160           $0             Sell
3       Tri-City Transport C    20       $114   -$2280   $0      $0              $880            $0             Buy
3       Stryker Drilling Com    10       $85    -$850    $0      $0              $30             $0             Buy
4       Tri-City Transport C    10       $122    $1220   $0      $0              $1250           $0             Sell
4       Stryker Drilling Com    20       $125    $2500   $0      $0              $3750           $0             Sell
4       Tri-City Transport C    20       $122   -$2440   $0      $0              $1310           $0             Buy
4       Stryker Drilling Com    10       $125   -$1250   $0      $0              $60             $0             Buy
5       Tri-City Transport C    100      $75     $0      $0      $0              $60             $0             Split
5       Tri-City Transport C    10       $75     $750    $0      $0              $810            $0             Sell
5       Stryker Drilling Com    20       $114    $2280   $0      $0              $3090           $0             Sell
5       Tri-City Transport C    20       $75    -$1500   $0      $0              $1590           $0             Buy
5       Stryker Drilling Com    10       $114   -$1140   $0      $0              $450            $0             Buy
6       Stryker Drilling Com    20       $80     $0      $0      $0              $450            $0             Split
6       Tri-City Transport C    10       $86     $860    $0      $0              $1310           $0             Sell
6       Stryker Drilling Com    20       $80     $1600   $0      $0              $2910           $0             Sell
6       Tri-City Transport C    20       $86    -$1720   $0      $0              $1190           $0             Buy
6       Stryker Drilling Com    10       $80    -$800    $0      $0              $390            $0             Buy
7       Tri-City Transport C    10       $112    $1120   $0      $0              $1510           $0             Sell
7       Stryker Drilling Com    20       $75    -$1500   $0      $0              $10             $0             Buy
8       Tri-City Transport C    10       $133    $1330   $0      $0              $1340           $0             Sell
8       Stryker Drilling Com    20       $61     $1220   $0      $0              $2560           $0             Sell
8       Stryker Drilling Com    40       $61    -$2440   $0      $0              $120            $0             Buy
9       Tri-City Transport C    10       $136    $1360   $0      $0              $1480           $0             Sell
9       Stryker Drilling Com    20       $21     $420    $0      $0              $1900           $0             Sell
9       Stryker Drilling Com    90       $21    -$1890   $0      $0              $10             $0             Buy
10      Tri-City Transport C    10       $144    $1440   $0      $0              $1450           $0             Sell
10      Stryker Drilling Com    20       $61     $1220   $0      $0              $2670           $0             Sell
10      Stryker Drilling Com    40       $61    -$2440   $0      $0              $230            $0             Buy
Avg: 20287.44678956274 over 11229 runs
Buys: TriCityTransportCompany StrykerDrillingCompany GrowthCorporationOfAmerica PioneerMutualFund UraniumEnterprisesInc CentralCityMunicipalBonds ShadyBrooksDevelopment ValleyPowerAndLightCompany UnitedAutoCompany MetroPropertiesInc
Sells: UnitedAutoCompany TriCityTransportCompany PioneerMutualFund GrowthCorporationOfAmerica UraniumEnterprisesInc StrykerDrillingCompany ShadyBrooksDevelopment CentralCityMunicipalBonds MetroPropertiesInc ValleyPowerAndLightCompany
Year    Security                Amount  Price   Cost    DivInt  MarginCharges   Cash            MarginTotal     TransactionType
1       Tri-City Transport C    20       $100   -$2000   $0      $0              $3000           $0             Buy
1       Stryker Drilling Com    30       $100   -$3000   $0      $0              $0              $0             Buy
2       Tri-City Transport C    10       $108    $1080   $0      $0              $1080           $0             Sell
2       Stryker Drilling Com    20       $140    $2800   $0      $0              $3880           $0             Sell
2       Tri-City Transport C    20       $108   -$2160   $0      $0              $1720           $0             Buy
2       Stryker Drilling Com    10       $140   -$1400   $0      $0              $320            $0             Buy
3       Stryker Drilling Com    40       $85     $0      $0      $0              $320            $0             Split
3       Tri-City Transport C    10       $114    $1140   $0      $0              $1460           $0             Sell
3       Stryker Drilling Com    20       $85     $1700   $0      $0              $3160           $0             Sell
3       Tri-City Transport C    20       $114   -$2280   $0      $0              $880            $0             Buy
3       Stryker Drilling Com    10       $85    -$850    $0      $0              $30             $0             Buy
4       Tri-City Transport C    10       $122    $1220   $0      $0              $1250           $0             Sell
4       Stryker Drilling Com    20       $125    $2500   $0      $0              $3750           $0             Sell
4       Tri-City Transport C    20       $122   -$2440   $0      $0              $1310           $0             Buy
4       Stryker Drilling Com    10       $125   -$1250   $0      $0              $60             $0             Buy
5       Tri-City Transport C    100      $75     $0      $0      $0              $60             $0             Split
5       Tri-City Transport C    10       $75     $750    $0      $0              $810            $0             Sell
5       Stryker Drilling Com    20       $114    $2280   $0      $0              $3090           $0             Sell
5       Tri-City Transport C    20       $75    -$1500   $0      $0              $1590           $0             Buy
5       Stryker Drilling Com    10       $114   -$1140   $0      $0              $450            $0             Buy
6       Stryker Drilling Com    20       $80     $0      $0      $0              $450            $0             Split
6       Tri-City Transport C    10       $86     $860    $0      $0              $1310           $0             Sell
6       Stryker Drilling Com    20       $80     $1600   $0      $0              $2910           $0             Sell
6       Tri-City Transport C    20       $86    -$1720   $0      $0              $1190           $0             Buy
6       Stryker Drilling Com    10       $80    -$800    $0      $0              $390            $0             Buy
7       Tri-City Transport C    10       $112    $1120   $0      $0              $1510           $0             Sell
7       Stryker Drilling Com    20       $75    -$1500   $0      $0              $10             $0             Buy
8       Tri-City Transport C    10       $133    $1330   $0      $0              $1340           $0             Sell
8       Stryker Drilling Com    20       $61     $1220   $0      $0              $2560           $0             Sell
8       Stryker Drilling Com    40       $61    -$2440   $0      $0              $120            $0             Buy
9       Tri-City Transport C    10       $136    $1360   $0      $0              $1480           $0             Sell
9       Stryker Drilling Com    20       $21     $420    $0      $0              $1900           $0             Sell
9       Stryker Drilling Com    90       $21    -$1890   $0      $0              $10             $0             Buy
10      Tri-City Transport C    10       $144    $1440   $0      $0              $1450           $0             Sell
10      Stryker Drilling Com    20       $61     $1220   $0      $0              $2670           $0             Sell
10      Stryker Drilling Com    40       $61    -$2440   $0      $0              $230            $0             Buy
</pre>
</details>


