using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine
{
    public class SecurityDetail
    {
        public SecurityNames Name { get; set; }
        public long Price { get; set; }
        public long Amount { get; set; }
    }
}
