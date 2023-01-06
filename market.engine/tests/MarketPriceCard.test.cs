using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine.tests
{
    static class MarketPriceCardTest
    {
        public static void BullPriceTest()
        {
            var expected = new int[] { 0, 18, 23, 11, 12, 46, 18, -5, 34, 15 };
            var prices = MarketPriceCard.Generate(Situation.Bull, rvalue: 4);

            foreach(var security in Security.EnumerateAll())
            {
                if (expected[(int)security.Name] != prices.ByName(security.Name)) throw new Exception("invalid entry");
            }
        }

        public static void BearPriceTest()
        {
            var expected = new int[] { 0, -8, -10, -10, -15, -20, -20, -23, -25, -7 };
            var prices = MarketPriceCard.Generate(Situation.Bear, rvalue: 12);

            foreach (var security in Security.EnumerateAll())
            {
                if (expected[(int)security.Name] != prices.ByName(security.Name)) throw new Exception("invalid entry");
            }
        }
    }
}
