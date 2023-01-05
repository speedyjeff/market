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

        internal void Add(MarketPrices other)
        {
            if (other.Deltas == null) return;
            if (Deltas == null) Deltas = new int[other.Deltas.Length];

            // combine
            for (int i = 0; i < Deltas.Length; i++)
            {
                Deltas[i] += (i < other.Deltas.Length) ? other.Deltas[i] : 0;
            }
        }

        internal void Add(SecurityNames security, int amount)
        {
            if ((int)security < 0 || (int)security >= Deltas.Length) throw new Exception("invalid security");
            if (Deltas == null) throw new Exception("must be initialized");
            Deltas[(int)security] += amount;
        }
        #endregion
    }
}
