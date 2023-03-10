using System;

namespace market.engine
{
    static class MarketPriceCard
    {
        public static SecuritiesContainer Generate(Situation sentiment, int rvalue)
        {
            if (rvalue < 2 || rvalue > 12) throw new Exception("invalid random value");
            if (sentiment != Situation.Bull && sentiment != Situation.Bear) throw new Exception("invalid sentiment");

            // return data from table for this sentiment and rvalue-2
            var sc = new SecuritiesContainer();
            foreach(var security in Security.EnumerateAll())
            {
                sc.Add(security.Name, Deltas[(int)security.Name][(int)sentiment][rvalue - 2]);
            }
            return sc;
        }

        #region private
        // security x sentiment x [0-12]
        private static long[][][] Deltas = new long[][][]
            {
                // Securities.CentralCityMunicipalBonds
                new long[][] 
                {
                    new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
                    new long[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
                },

                // Securities.GrowthCorporationOfAmerica
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -2 , 26 , 18 , 23 , 20 , 17 , 19 , 11 , 13 , 14 , 24},
                    // Sentiment.Bear
                    new long[] { 12, 7, 9, 7, 8, 6, 5, -2, 11, -5, -8 }
                },

                // Securities.MetroPropertiesInc
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -10 , 16 , 23 , 28 , 15 , 21 , 24 , 18 , 31 , -8 , 24},
                    // Sentiment.Bear
                    new long[] { 14, -6, 10, 8, 6, 4, 7, 6, 11, 13, -10 }
                },

                // Securities.PioneerMutualFund
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -8 , 25 , 11 , -2 , 15 , 13 , 17 , 14 , 1 , 19 , 23},
                    // Sentiment.Bear
                    new long[] { 13, 10, 7, 5, 4, 3, -1, -3, -5, -8, -10 }
                },

                // Securities.ShadyBrooksDevelopment
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -9 , 8 , 12 , 11 , 7 , -2 , 9 , 11 , 14 , -1 , 20},
                    // Sentiment.Bear
                    new long[] { 10, -10, -5, -6, -4, 3, -3, -8, -7, 6, -15 }
                },

                // Securities.StrykerDrillingCompany
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -2 , -14 , 46 , 56 , -20 , 37 , -5 , 67 , -11 , -9 , 51},
                    // Sentiment.Bear
                    new long[] { 10, 30, -20, -40, 40, -15, 45, -20, 30, 25, -20 }
                },

                // Securities.TriCityTransportCompany
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -9 , 21 , 18 , 19 , 15 , 23 , 26 , 15 , 18 , 25 , 27},
                    // Sentiment.Bear
                    new long[] { 20, 6, 12, 3, 8, 5, 6, 7, 10, 4, -20 }
                },

               // Securities.UnitedAutoCompany
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -7 , 14 , -5 , 30 , 13 , 23 , 13 , 22 , 18 , -10 , 38},
                    // Sentiment.Bear
                    new long[] { 21, -19, 21, 16, 4, 8, -10, 10, -11, 18, -23 }
                },

                // Securities.UraniumEnterprisesInc
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -16 , -4 , 34 , 29 , -10 , 19 , -7 , 18 , -14 , 13 , 33},
                    // Sentiment.Bear
                    new long[] { 25, 22, 18, -1, -12, -8, 10, 14, -18, -22, -25 }
                },

                // Securities.ValleyPowerAndLightCompany
                new long[][]
                {
                    // Sentiment.Bull
                    new long[] { -4 , 17 , 15 , 14 , 12 , 14 , 15 , 13 , 10 , 19 , 18},
                    // Sentiment.Bear
                    new long[] { 8, -2, 7, 4, 3, 5, 4, 6, -4, -4, -7 }
                }
            };
        #endregion
    }
}
