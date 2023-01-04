using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine
{
    public struct SituationCard
    {
        public readonly Situation Market;
        public readonly string Description;
        public readonly Dictionary<SecurityNames,int> Price;
        public readonly Dictionary<SecurityNames,int> Cash;

        public static int Count { get { return All.Length; } }

        public SituationCard ByIndex(int index)
        {
            if (index < 0 || index >= All.Length) throw new Exception("invalid index");
            return All[index];
        }

        #region private
        private SituationCard(Situation market, string description, Dictionary<SecurityNames,int> price, Dictionary<SecurityNames,int> cash)
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
                price: new Dictionary<SecurityNames,int>() {{SecurityNames.ShadyBrooksDevelopment, 5}},
                cash: new Dictionary<SecurityNames, int>()
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "Large petroleum corporation offers to buy all assets for cash.  Offer is well above book value.  Directors approve and will submit recommendation to stockholders. ",
                price: new Dictionary<SecurityNames,int>() { {SecurityNames.StrykerDrillingCompany, 17 } },
                cash: new Dictionary<SecurityNames,int>()
            ),


            new SituationCard(
                market: Situation.Bull,
                description: "Company prospectors find huge, new high grade ore deposits.",
                price: new Dictionary<SecurityNames,int>() { { SecurityNames.UraniumEnterprisesInc, 10 } },
                cash: new Dictionary<SecurityNames,int>()
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "War scare promotes mixed activity on Wall Street.",
                price: new Dictionary<SecurityNames,int>() 
                {
                    {SecurityNames.PioneerMutualFund, -8 },
                    {SecurityNames.StrykerDrillingCompany, 8 },
                    {SecurityNames.UraniumEnterprisesInc, 5 }
                },
                cash: new Dictionary<SecurityNames,int>()
            ),

            new SituationCard(
                market: Situation.Bull,
                description: "President announces expansion of plans to increase productive capacity 30%.",
                price: new Dictionary<SecurityNames,int>() { { SecurityNames.UnitedAutoCompany, 15 } },
                cash: new Dictionary<SecurityNames,int>()
            ),

            // Bear
            new SituationCard(
                market: Situation.Bear,
                description: "Government suddenly announces it will no longer support ore prices, since it has large stockpiles.",
                price: new Dictionary<SecurityNames,int>() { { SecurityNames.UraniumEnterprisesInc, -25 } },
                cash: new Dictionary<SecurityNames,int>()
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "City Council considers the Company’s choicest property for large industrial fair.",
                price: new Dictionary<SecurityNames,int>() { { SecurityNames.MetroPropertiesInc, 10 } },
                cash: new Dictionary<SecurityNames,int>()
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "President hospitalized in sanitorium for an indefinite period.",
                price: new Dictionary<SecurityNames,int>() { {SecurityNames.TriCityTransportCompany, -5 } },
                cash: new Dictionary<SecurityNames,int>()
            ),

            new SituationCard(
                market: Situation.Bear,
                description: "Extra year end dividend of $2 per share declared by the Board of Directors.",
                price: new Dictionary<SecurityNames,int>() { {SecurityNames.GrowthCorporationOfAmerica, 10 } },
                cash: new Dictionary<SecurityNames,int>()  { { SecurityNames.GrowthCorporationOfAmerica, 2 } }
            )
        };
        #endregion
    }
}
