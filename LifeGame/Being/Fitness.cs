using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    /// <summary>
    /// Class for managing being fitness.
    /// </summary>
    [Serializable]
    public class Fitness
    {
        public Average[] Parameters;

        public float this[FitnessParam param]
        {
            get
            {
                return Parameters[(int)param].Value;
            }
        }

        public Fitness()
        {
            Parameters = new Average[Constants.FITNESS_PARAM_COUNT];
            for (int i = 0; i < Constants.FITNESS_PARAM_COUNT; i++)
            {
                Parameters[i] = new Average();
            }

        }

        public void Update(FitnessParam param, float value)
        {
            Parameters[(int)param].Add(value);
        }
    }
}
