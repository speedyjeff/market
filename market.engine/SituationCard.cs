using System;
using System.Collections.Generic;

namespace market.engine
{
    public struct SituationCard
    {
        public readonly Situation Market;
        public readonly string Description;
        public readonly Dictionary<SecurityNames,long> Price;
        public readonly Dictionary<SecurityNames,long> Cash;

        #region internal
        internal static int Count { get { return All.Length; } }

        internal static SituationCard ByIndex(int index)
        {
            if (index < 0 || index >= All.Length) throw new Exception("invalid index");
            return All[index];
        }
        #endregion

        #region private
        private SituationCard(Situation market, string description, Dictionary<SecurityNames,long> price, Dictionary<SecurityNames,long> cash)
        {
            Market = market;
            Description = description;
            Price = price;
            Cash = cash;
        }

        private static SituationCard[] All = new SituationCard[]
        {
            // Bull
            new SituationCard(
                market: Situation.Bull,
                description: "Influx of personnel of new company in nearby town creates a severe housing shortage.",
                price: new Dictionary<SecurityNames,long>() {{SecurityNames.ShadyBrooksDevelopment, 5L}},
                cash: new Dictionary<SecurityNames, long>()
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Large petroleum corporation offers to buy all assets for cash.  Offer is well above book value.  Directors approve and will submit recommendation to stockholders. ",
                price: new Dictionary<SecurityNames,long>() { {SecurityNames.StrykerDrillingCompany, 17L } },
                cash: new Dictionary<SecurityNames,long>()
            ),


            new SituationCard(
                market: Situation.Bull,
                description: "Company prospectors find huge, new high grade ore deposits.",
                price: new Dictionary<SecurityNames,long>() { { SecurityNames.UraniumEnterprisesInc, 10L } },
                cash: new Dictionary<SecurityNames,long>()
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "War scare promotes mixed activity on Wall Street.",
                price: new Dictionary<SecurityNames,long>() { {SecurityNames.PioneerMutualFund, -8L }, {SecurityNames.StrykerDrillingCompany, 8L }, {SecurityNames.UraniumEnterprisesInc, 5L }},
                cash: new Dictionary<SecurityNames,long>()
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "President announces expansion of plans to increase productive capacity 30%.",
                price: new Dictionary<SecurityNames,long>() { { SecurityNames.UnitedAutoCompany, 15L } },
                cash: new Dictionary<SecurityNames,long>()
            ),

            // Bear
            new SituationCard(
                market: Situation.Bear,
                description: "Government suddenly announces it will no longer support ore prices, since it has large stockpiles.",
                price: new Dictionary<SecurityNames,long>() { { SecurityNames.UraniumEnterprisesInc, -25L } },
                cash: new Dictionary<SecurityNames,long>()
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "City Council considers the Company’s choicest property for large industrial fair.",
                price: new Dictionary<SecurityNames,long>() { { SecurityNames.MetroPropertiesInc, 10L } },
                cash: new Dictionary<SecurityNames,long>()
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "President hospitalized in sanitorium for an indefinite period.",
                price: new Dictionary<SecurityNames,long>() { {SecurityNames.TriCityTransportCompany, -5L } },
                cash: new Dictionary<SecurityNames,long>()
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Extra year end dividend of $2 per share declared by the Board of Directors.",
                price: new Dictionary<SecurityNames,long>() { {SecurityNames.GrowthCorporationOfAmerica, 10L } },
                cash: new Dictionary<SecurityNames,long>()  { { SecurityNames.GrowthCorporationOfAmerica, 2L } }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Large terminal destroyed by fire; insufficient insurance on building due to Company's delayed move to new location.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.TriCityTransportCompany,-25L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Land rights litigation holds up progress.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.StrykerDrillingCompany,-10L }},
                cash: new Dictionary<SecurityNames, long>() { }
			),

            new SituationCard(
                market: Situation.Bull,
                description: "National firm leases Company's largest office building.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.MetroPropertiesInc,5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Company lands ten-year contract with large industrial equipment corporation.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.TriCityTransportCompany,15L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Intensive advertising campaign gains Company three major, long-term contracts.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.TriCityTransportCompany,10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Company moves to a new excellent location.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.TriCityTransportCompany,5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "United Auto announces new advanced-design auto entry in the mini-car field.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UnitedAutoCompany,10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "President, Vice-President and Chief Counsel of Growth Corporation of America reach retirement age.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,-10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Corporation releases high profit and sales financial report and announces plans to invest an additional $2 million on special research projects next year.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,8L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Major coal company announces reduced coal prices to electric power utilities.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.ValleyPowerAndLightCompany,5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Community steadily deteriorates. The management is forced to lower rental rates to attract tenants.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.ShadyBrooksDevelopment,-5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Two founders and major stockholders of the Corporation disagree on policy. One sells out his entire stockholdings.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,-8L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Urban Renewal Program delayed by indecision of City Planning Commission.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.MetroPropertiesInc,-10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Foreign car rage hits the buying public. Big cars in slow demand.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UnitedAutoCompany,-15L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Surge of profit-taking drops stock market.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,-8L },{ SecurityNames.MetroPropertiesInc,-5L },{ SecurityNames.UnitedAutoCompany,-7L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Corporation announces new metal forming process which it claims will revolutionize all metal-working industries covered by U.S. and foreign patents.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Experimental nuclear power station proves more economical than anticipated. Three electrical power companies announce plans to build large-scale nuclear power plants.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UraniumEnterprisesInc,10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Company's Annual Report shows net earnings off during fourth quarter.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.MetroPropertiesInc,-5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Internal Revenue depletion allowance reduced 50%.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.StrykerDrillingCompany,-15L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Buying wave raises the market.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.PioneerMutualFund,3L },{ SecurityNames.ValleyPowerAndLightCompany,4L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "President announces expansion plans to increase productive capacity 30%.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UnitedAutoCompany,15L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Commission grants permission to construct a new nuclear generating plant of great capacity and efficiency.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.ValleyPowerAndLightCompany,5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "General market rise over the last two months.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,8L },{ SecurityNames.MetroPropertiesInc,5L },{ SecurityNames.PioneerMutualFund,5L },{ SecurityNames.UnitedAutoCompany,7L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Strikes halt production in all eight United Auto plants as UAW and Company officially fail to reach agreement on labor contract.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UnitedAutoCompany,-15L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Corporation unexpectedly relinquishes its monopoly on its major product after a lengthy anti-trust suit.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.GrowthCorporationOfAmerica,-10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Three-for-one split rumored.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UnitedAutoCompany,10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Public Utility Commission rejects Company's request for rate hike.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.ValleyPowerAndLightCompany,-14L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Competitor invents a new economical automatic transmission.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.UnitedAutoCompany,-5L }},
                cash: new Dictionary<SecurityNames, long>() { }
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "City Council considers the Company's choicest property for large industrial fair.",
                price: new Dictionary<SecurityNames, long>() {{ SecurityNames.MetroPropertiesInc,10L }},
                cash: new Dictionary<SecurityNames, long>() { }
            )
        };
        #endregion
    }
}
