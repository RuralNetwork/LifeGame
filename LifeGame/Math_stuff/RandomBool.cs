using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    static class RandomBool
    {
        static FastRandom rand = new FastRandom();
        static BitArray bitArr;
        static int idx = 31;

        static RandomBool()
        {
            Next();
        }

        static public bool Next()
        {
            idx++;
            if (idx == 32)
            {
                bitArr = new BitArray(new int[] { rand.Next() });
                idx = 0;
            }
            return bitArr[idx];
        }
    }
}
