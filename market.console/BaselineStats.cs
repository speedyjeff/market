using market.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.console
{
    struct BaselineStats
    {
        public long[] Increase;
        public long[] Decrease;
        public long[] AbovePar;
        public long[] BelowPar;
        public long[] Split;
        public long[] Worthless;
        public long[] NoDividend;

        public long YearsPerIteration;
        public long TotalYears;

        public BaselineStats()
        {
            Increase = new long[Security.Count];
            Decrease = new long[Security.Count];
            AbovePar = new long[Security.Count];
            BelowPar = new long[Security.Count];
            Split = new long[Security.Count];
            Worthless = new long[Security.Count];
            NoDividend = new long[Security.Count];
        }
    }
}
