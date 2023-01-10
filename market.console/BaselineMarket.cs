using market.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.console
{
    public static class BaselineMarket
    {
        public static Market Run(MarketConfiguration config, SecurityNames security, BaselinePolicy policy)
        {
            // run the market with the specific policy
            var market = new Market(config);

            do
            {
                // start
                market.StartYear();

                // sell
                market.SellSecurities(new List<Transaction>());

                // calculate opportunity
                var price = market.Prices.ByName(security);
                var amount = (market.My.CashBalance / price);
                // ensure round number
                amount -= (amount % market.Config.PurchaseDivisor);

                var placeOrder = false;
                if (amount > 0)
                {
                    // buy based on policy
                    if (policy == BaselinePolicy.AlwaysBuy) placeOrder = true;
                    else if (policy == BaselinePolicy.AlwaysBuyLow && price <= market.Config.ParValue) placeOrder = true;
                    else if (policy == BaselinePolicy.AlwaysOnMargin && market.Year != 1 && market.Year != market.Config.LastYear) placeOrder = true;
                }

                // place the order
                if (placeOrder)
                { 
                    market.BuySecurities(new List<Transaction>()
                    {
                        new Transaction()
                        {
                            Security = security,
                            Amount = amount,
                            OnMargin = (policy == BaselinePolicy.AlwaysOnMargin)
                        }
                    });
                }
                else market.BuySecurities(new List<Transaction>());
            }
            while (market.EndYear());

            return market;
        }
    }
}
