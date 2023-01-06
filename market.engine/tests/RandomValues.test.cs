using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine.tests
{
    static class RandomValuesTest
    {
        public static void Distribution()
        {
            var rand = new Random(Seed: 123132352);
            var values = Market.RandomValues(rand, length: 1000, inclusiveFrom: 1, inclusiveTo: 6, allowdups: true);

            // ensure that frequency of the sum of pairs is 6, 7, or 8
            var freq = new Dictionary<int, int>();
            var maxfreq = 0;
            var maxv = 0;
            for (int i = 0; i < values.Length; i += 2)
            {
                // sum the pair
                var v = values[i] + values[i + 1];

                // track frequency
                if (!freq.ContainsKey(v)) freq.Add(v, 1);
                else freq[v]++;

                // find highest frequency
                if (freq[v] > maxfreq)
                {
                    maxfreq = freq[v];
                    maxv = v;
                }
            }

            // check that 6,7,8 are the most frequent
            if (maxv != 6 && maxv != 7 && maxv != 8) throw new Exception("invalid distribution");
        }

        public static void Unique()
        {
            var rand = new Random(Seed: 847529472);
            var values = Market.RandomValues(rand, length: 1000, inclusiveFrom: 0, inclusiveTo: 999, allowdups: false);

            // ensure there are no duplicates
            var dups = new HashSet<int>();
            var adjacent = 0;
            for (int i = 0; i < values.Length; i++)
            { 
                // check uniqueness
                if (dups.Contains(values[i])) throw new Exception("not unique");
                dups.Add(values[i]);

                // check for adjacency
                if (i > 0 && Math.Abs(values[i - 1] - values[i]) == 1) adjacent++;
            }

            if (adjacent > (0.05f * values.Length)) throw new Exception("too many adjacent values");
        }

        public static void Identical()
        {
            // gather two sets, and ensure they are identical
            int[][][] results = new int[2][][];

            for (int i = 0; i < 2; i++)
            {
                // setup
                var rand = new Random(Seed: 12345678);
                results[i] = new int[2][];

                // gather values
                results[i][0] = Market.RandomValues(rand, length: 1000, inclusiveFrom: 1, inclusiveTo: 6, allowdups: true);
                results[i][1] = Market.RandomValues(rand, length: 1000, inclusiveFrom: 0, inclusiveTo: 50, allowdups: false);
            }

            // check that they are identical
            for(int i=0; i < results[0].Length; i++)
            {
                for(int j=0; j< results[0][i].Length; j++)
                {
                    if (results[0][i][j] != results[1][i][j]) throw new Exception("not equal");
                }
            }
        }
    }
}
