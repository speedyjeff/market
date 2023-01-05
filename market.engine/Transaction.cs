using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.engine
{
    public class Transaction
    {
        public Player Player { get; set; }
        public SecurityNames Security { get; set; }
        public int Amount { get; set; }
    }
}
