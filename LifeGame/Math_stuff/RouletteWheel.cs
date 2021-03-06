﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
        [Serializable]
    class RouletteWheel
    {
        FastRandom rand = new FastRandom();
        float[] stair;
        int count;

        public RouletteWheel(params float[] probs)
        {
            count = probs.Length;
            var sum = probs.Sum();
            for (int i = 0; i < count; i++) // normalize
            {
                probs[i] /= sum;
            }
            stair = new float[count];
            var accumulator = 0.0f;
            for (int i = 0; i < count; i++)// put the values as a stair (for each: step + previous steps)
            {
                accumulator += probs[i];
                stair[i] = accumulator;
            }
        }
        ///<summary>
        ///Generated a new number based on initialized values
        ///</summary>
        public int Spin()
        {
            var rng = (float)rand.NextDouble();
            for (int i = 0; i < count; i++)
            {
                if (rng < stair[i])
                {
                    return i;
                }
            }
            return 0;
        }
    }
}
