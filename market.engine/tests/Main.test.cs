using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine.tests
{
    static class MainTest
    {
        public static void Run()
        {
            SecuritiesContainerTest.TestAdd();
            MarketPriceCardTest.BullPriceTest();
            MarketPriceCardTest.BearPriceTest();
            RandomValuesTest.Distribution();
            RandomValuesTest.Unique();
            RandomValuesTest.Identical();
        }
    }
}
