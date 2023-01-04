using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine
{
    public struct MarketPrices
    {
        public int ByName(SecurityNames name)
        {
            if (Deltas == null) throw new Exception("not initialized");
            if ((int)name < 0 || (int)name > Deltas.Length) return 0;
            return Deltas[(int)name];
        }
        

        #region private internal
        internal int[] Deltas;
        #endregion
    }
}
