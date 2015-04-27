using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    [Serializable]
    public class Genome
    {
        public float[] Fitness { get; set; }
        public NNGenome NNGenome { get; set; }
        public BodyGenome BodyGenome { get; set; }

        public Genome(Genome dadGen, Genome mumGen)
        {
            Fitness = new float[Constants.FITNESS_PARAM_COUNT];
            NNGenome = new NNGenome(dadGen.NNGenome, mumGen.NNGenome);
            BodyGenome = new BodyGenome(dadGen.BodyGenome, mumGen.BodyGenome);
        }

        public Genome()
        {
            Fitness = new float[Constants.FITNESS_PARAM_COUNT];
            NNGenome = new NNGenome();
            //  BodyGenome=new BodyGenome()
        }

        public Genome(Genome genome)
        {
            Fitness = new float[Constants.FITNESS_PARAM_COUNT];
            NNGenome = new NNGenome(genome.NNGenome);
            //BodyGenome = new BodyGenome();
        }

        /// <summary>
        /// Magnitude in Manhattan metrics -> sum of components
        /// </summary>
        public float Magnitude
        {
            get
            {
                return Fitness.Sum();
            }
        }

        /// <summary>
        /// Manhattan distance. It is unreliable but much faster to calculate than euclidean distance.
        /// </summary>
        public float DistanceTo(Genome otherGen)
        {
            var d = 0f;
            for (int i = 0; i < Constants.FITNESS_PARAM_COUNT; i++)
            {
                d += Math.Abs(Fitness[i] - otherGen.Fitness[i]);
            }
            return d;
        }
    }
}
