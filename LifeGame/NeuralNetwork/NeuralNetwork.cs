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
    /*public class NeuralNetwork
    {
        // parameters to be adjusted
        const int CYCLES_COUNT = 10;                                              // V-- bias
        const int INPUTS_AND_BIAS_COUNT = 1 * 7 + 1 * 11 + 1 * 10 + 5 * 10 + 5 * 8 + 1;
        // 1 environment:                                 R, G, B,         painful,         warmth,                   smellintensity, smell
        // 1 carried object:                              R, G, B, moving, painful, weight, warmth, amplitude, pitch, smellintensity, smell
        // 1 current cell:                                R, G, B, moving, painful,         warmth, amplitude, pitch, smellintensity, smell
        // 5 visible nearby cells:                        R, G, B, moving, painful,         warmth, amplitude, pitch, smellintensity, smell
        // 5 average of cells for each direction:         R, G, B, moving,                          amplitude, pitch, smellintensity, smell

        Connection[] _connections;

        float[] _preActivationArray;
        float[] _postActivationArray;

        public SignalArray Inputs { get; private set; }
        public SignalArray Outputs { get; private set; }

        public NeuralNetwork(NNGenome genome)
        {

        }

        public void Calculate()
        {
            for (int i = 0; i < CYCLES_COUNT; i++)
            {
                for (int j = 0; j < _connections.Length; j++)
                {
                    _preActivationArray[_connections[j].Target] += _postActivationArray[_connections[j].Source] * _connections[j].Weight;
                }

                for (int j = INPUTS_AND_BIAS_COUNT; j < _preActivationArray.Length; j++)
                {
                    _postActivationArray[j] = 1.0f / (1.0f + ((float)Math.Exp(-_preActivationArray[j])));// standard sigmoid
                    // TODO: reconsider the activation function, can "0.5f+(x/(2.0f*(0.2f+Math.Abs(x))))" be better for performance/quality?

                    _preActivationArray[j] = 0.0f;
                }
            }
        }
    }*/
}
