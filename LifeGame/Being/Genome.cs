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
        public NNGenome NNGenome { get; set; }
        public BodyGenome BodyGenome { get; set; }

        public Genome(NNGenome nnGenome, BodyGenome bodyGenome)
        {
            NNGenome = nnGenome;
            BodyGenome = bodyGenome;
        }

        public Genome(Simulation simulation)
        {
            NNGenome = new NNGenome(simulation);
            //  BodyGenome=new BodyGenome()
        }
    }
}
