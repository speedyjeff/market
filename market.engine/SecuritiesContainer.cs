using System;

namespace market.engine
{
    public struct SecuritiesContainer
    {
        public SecuritiesContainer()
        {
            Deltas = new long[Security.Count];
        }
        
        public long ByName(SecurityNames name)
        {
            if (Deltas == null) throw new Exception("not initialized");
            if ((int)name < 0 || (int)name > Deltas.Length) return 0;
            return Deltas[(int)name];
        }

        #region private
        private long[] Deltas;
        #endregion

        #region private internal
        internal void Add(SecuritiesContainer other)
        {
            if (other.Deltas == null) return;
            if (Deltas == null) Deltas = new long[other.Deltas.Length];

            // combine
            for (int i = 0; i < Deltas.Length; i++)
            {
                Deltas[i] += (i < other.Deltas.Length) ? other.Deltas[i] : 0L;
            }
        }

        internal void Add(SecurityNames security, long amount)
        {
            if ((int)security < 0 || (int)security >= Deltas.Length) throw new Exception("invalid security");
            if (Deltas == null) throw new Exception("must be initialized");
            Deltas[(int)security] += amount;
        }
        #endregion
    }
}
