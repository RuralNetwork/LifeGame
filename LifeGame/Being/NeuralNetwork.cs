using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public struct Connection
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public float Weight { get; set; }
    }

    //TODO: test if is better to have one bias node or the bias in every node, for best evolution speed/computation speed
    public class NeuralNetwork
    {
        // parameters to be adjusted
        const int CYCLES_COUNT = 10;         // V-- bias
        const int INPUTS_AND_BIAS_COUNT = 135 + 1;
        // environment:                                   R, G, B,         painful,         warmth,                   smellintensity, smell       7
        // carried object:                                R, G, B, moving, painful, weight, warmth, amplitude, pitch, smellintensity, smell       11
        // current cell:                                  R, G, B, moving, painful,         warmth, amplitude, pitch, smellintensity, smell       10
        // 5 visible nearby cells:                        R, G, B, moving, painful,         warmth, amplitude, pitch, smellintensity, smell       50
        // 5 average of cells for each direction:         R, G, B, moving,                          amplitude, pitch, smellintensity, smell       40
        // autoperception:                                                          weight, warmth,                   smellintensity, smell, 
        //                                                health, integrity, thirst, hunger, wet, height, sex, direction sine & cosine            13
        // circadian, circa-annual sine & cosine:                                                                                                 4
        //                                                                                                                                   TOT: 135

        const int OUTPUTS = 000;
        // actions:
        Connection[] connections;

        float[] preActivationArray;
        public float[] State { get; set; }// old postActivationArray;

        //public SignalArray Inputs { get; private set; }     // theese are redundant, just use State prop
        //public SignalArray Outputs { get; private set; }

        public NeuralNetwork(NNGenome genome)
        {

        }

        public void Calculate()
        {
            for (int i = 0; i < CYCLES_COUNT; i++)
            {
                for (int j = 0; j < connections.Length; j++)
                {
                    preActivationArray[connections[j].Target] += State[connections[j].Source] * connections[j].Weight;
                }

                for (int j = INPUTS_AND_BIAS_COUNT; j < preActivationArray.Length; j++)
                {
                    State[j] = 1.0f / (1.0f + ((float)Math.Exp(-preActivationArray[j])));// standard sigmoid
                    // TODO: reconsider the activation function, can "0.5f+(x/(2.0f*(0.2f+Math.Abs(x))))" be better for performance/quality?

                    preActivationArray[j] = 0.0f;
                }
            }
        }
    }
}
