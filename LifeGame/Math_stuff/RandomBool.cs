using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    //this is a temporary class (waiting for cudafy implementation) that yields 32 random booleans with one random shot, one per call
    class RandomBool
    {
        FastRandom rand = new FastRandom();
        BitArray bitArr;
        int idx = 31;

        public RandomBool()
        {
            Next();
        }

        public bool Next()
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
