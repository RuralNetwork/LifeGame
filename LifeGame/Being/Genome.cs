using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    public class Genome
    {
        public float Fitness { get; set; }
        public NNGenome NNGenome { get; private set; }
        public BodyGenome BodyGenome { get; private set; }

        public Genome(NNGenome nnGenome, BodyGenome bodyGenome)
        {
            NNGenome = nnGenome;
            BodyGenome = bodyGenome;
        }
    }
}
