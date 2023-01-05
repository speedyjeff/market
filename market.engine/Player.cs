using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine
{
    public class Player
    {
        public Player()
        {
            Id = GetNextId();
            Holdings = new MarketPrices() { Deltas = new int[Security.Count] };
        }

        public int CashBalance { get; internal set; }
        public MarketPrices Holdings { get; private set; }

        #region private
        private volatile int NextId;
        private int GetNextId()
        {
            return System.Threading.Interlocked.Increment(ref NextId);
        }
        #endregion

        #region internal
        internal int Id;
        #endregion
    }
}
