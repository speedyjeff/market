using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine.tests
{
    static class SecuritiesContainerTest
    {
        public static void TestAdd()
        {
            var c1 = new SecuritiesContainer();

            // check that empty
            var count = 1;
            foreach (var security in Security.EnumerateAll())
            {
                if (c1.ByName(security.Name) != 0) throw new Exception("invalid row");
                // add to this element
                c1.Add(security.Name, count);

                count++;
            }

            // add to a different one (twice)
            var c2 = new SecuritiesContainer();
            c2.Add(c1);
            c2.Add(c1);

            // check again
            count = 1;
            foreach (var security in Security.EnumerateAll())
            {
                if (c1.ByName(security.Name) != count) throw new Exception("invalid row");
                if (c2.ByName(security.Name) != count*2) throw new Exception("invalid row");
                count++;
            }
        }
    }
}
