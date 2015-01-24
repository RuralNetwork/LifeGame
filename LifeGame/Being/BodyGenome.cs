using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifeGame
{
    class BodyGenome
    {
        public float[] Genome { get; set; }// Contains:
        //R
        //G
        //B
        //Sex (0 male, 1 female)
        //Height multiplicator
        //Sight multiplicator
        //Herbivore/carnivore
        //
        //Consider switching to a more realistic rapresentation of genes

        public static float ColorProb;
        public static float HeightMulProb;
        public static float SightMulProb;
        public static float HerbCarnProb;
        RouletteWeel rw = new RouletteWeel(ColorProb, HeightMulProb, SightMulProb, HerbCarnProb);

        public BodyGenome(BodyGenome parent1, BodyGenome parent2)
        {

        }

        void mutate()
        {

        }


    }
}
