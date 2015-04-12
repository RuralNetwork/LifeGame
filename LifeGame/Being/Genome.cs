using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LifeGame
{
    public class Genome
    {
        public float Fitness { get; set; }
        public NNGenome NNGenome { get; set; }
        public BodyGenome BodyGenome { get; set; }

        public Genome(Genome dadGen, Genome mumGen)
        {
           NNGenome=new NNGenome(dadGen.NNGenome,mumGen.NNGenome,dadGen.Fitness,mumGen.Fitness);
           BodyGenome = new BodyGenome(dadGen.BodyGenome, mumGen.BodyGenome);
        }

        public Genome(Simulation simulation)
        {
            NNGenome = new NNGenome(simulation);
            //  BodyGenome=new BodyGenome()
        }

        public Genome(Genome genome)
        {
            NNGenome = new NNGenome(genome.NNGenome);
            //BodyGenome = new BodyGenome();
        }
    }
}
