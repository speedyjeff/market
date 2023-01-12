using market.bots;
using market.engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace market.console
{
    public class NeuralBotContainer
    {
        public NeuralBot Bot;
        public Market Market;
        public List<long> Networths;

        public NeuralBotContainer()
        {
            Networths = new List<long>();
            Bot = new NeuralBot();
        }
    }
}
