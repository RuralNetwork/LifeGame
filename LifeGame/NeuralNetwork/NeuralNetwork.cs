using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public struct Connection
    {
        public int Source;
        public int Target;
        public int Weight;
    }

    //TODO: test if is better to have one bias node or the bias in every node, for best evolution speed/computation speed
    public class NeuralNetwork
    {
        Connection[] _connections;

        float[] _preActivationArray;
        float[] _postActivationArray;

        public SignalArray Inputs { get; private set; }
        public SignalArray Outputs { get; private set; }

        public NeuralNetwork()
        {

        }
    }
}
