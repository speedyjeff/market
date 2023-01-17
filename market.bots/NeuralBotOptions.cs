using System;
using System.Collections.Generic;
using System.Text;

namespace market.bots
{
    public struct NeuralBotOptions
    {
        public bool ShuffleBuySecurityOrder = true;
        public bool ShuffleSellSecurityOrder = true;
        public float PcntRandomResults = 0f;
        public int[] HiddenNetworks = new int[] { 16 };
        public float LearningRate = 0.00015f;
        public int MiniBatchCount = 1;
        public bool AllowOnMargin = false;

        public NeuralBotOptions()
        {
        }
    }
}
